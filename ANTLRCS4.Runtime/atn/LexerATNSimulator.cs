/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime.dfa;
using org.antlr.v4.runtime.misc;

namespace org.antlr.v4.runtime.atn;

/** "dup" of ParserInterpreter */
public class LexerATNSimulator : ATNSimulator
{
    public static readonly bool debug = false;
    public static readonly bool dfa_debug = false;

    public static readonly int MIN_DFA_EDGE = 0;
    public static readonly int MAX_DFA_EDGE = 127; // forces unicode to stay in ATN

    /** When we hit an accept state in either the DFA or the ATN, we
	 *  have to notify the character stream to start buffering characters
	 *  via {@link IntStream#mark} and record the current state. The current sim state
	 *  includes the current index into the input, the current line,
	 *  and current character position in that line. Note that the Lexer is
	 *  tracking the starting line and characterization of the token. These
	 *  variables track the "state" of the simulator when it hits an accept state.
	 *
	 *  <p>We track these variables separately for the DFA and ATN simulation
	 *  because the DFA simulation often has to fail over to the ATN
	 *  simulation. If the ATN simulation fails, we need the DFA to fall
	 *  back to its previously accepted state, if any. If the ATN succeeds,
	 *  then the ATN does the accept and the DFA simulator that invoked it
	 *  can simply return the predicted token type.</p>
	 */
    public class SimState
    {
        public int index = -1;
        public int line = 0;
        public int charPos = -1;
        public DFAState dfaState;

        public void Reset()
        {
            index = -1;
            line = 0;
            charPos = -1;
            dfaState = null;
        }
    }


    protected readonly Lexer recog;

    /** The current token's starting index into the character stream.
	 *  Shared across DFA to ATN simulation in case the ATN fails and the
	 *  DFA did not have a previous accept state. In this case, we use the
	 *  ATN-generated exception object.
	 */
    protected int startIndex = -1;

    /** line number 1..n within the input */
    protected int line = 1;

    /** The index of the character relative to the beginning of the line 0..n-1 */
    protected int charPositionInLine = 0;


    public readonly DFA[] decisionToDFA;
    protected int mode = Lexer.DEFAULT_MODE;

    /** Used during DFA/ATN exec to record the most recent accept configuration info */

    protected readonly SimState prevAccept = new ();

    public LexerATNSimulator(ATN atn, DFA[] decisionToDFA,
                             PredictionContextCache sharedContextCache)
        : this(null, atn, decisionToDFA, sharedContextCache)
    {
    }

    public LexerATNSimulator(Lexer recog, ATN atn,
                             DFA[] decisionToDFA,
                             PredictionContextCache sharedContextCache)
        : base(atn, sharedContextCache)
    {
        this.decisionToDFA = decisionToDFA;
        this.recog = recog;
    }

    public void CopyState(LexerATNSimulator simulator)
    {
        this.charPositionInLine = simulator.charPositionInLine;
        this.line = simulator.line;
        this.mode = simulator.mode;
        this.startIndex = simulator.startIndex;
    }

    public int Match(CharStream input, int mode)
    {
        this.mode = mode;
        int mark = input.Mark();
        try
        {
            this.startIndex = input.Index;
            this.prevAccept.Reset();
            DFA dfa = decisionToDFA[mode];
            return dfa.s0 == null ? MatchATN(input) : ExecATN(input, dfa.s0);
        }
        finally
        {
            input.Release(mark);
        }
    }

    public override void Reset()
    {
        prevAccept.Reset();
        startIndex = -1;
        line = 1;
        charPositionInLine = 0;
        mode = Lexer.DEFAULT_MODE;
    }

    public override void ClearDFA()
    {
        for (int d = 0; d < decisionToDFA.Length; d++)
            decisionToDFA[d] = new DFA(atn.GetDecisionState(d), d);
    }

    protected virtual int MatchATN(CharStream input)
    {
        var startState = atn.modeToStartState[mode];

        if (debug)
        {
            Console.Out.WriteLine($"matchATN mode {mode} start: {startState}");
        }

        int old_mode = mode;

        var s0_closure = ComputeStartState(input, startState);
        bool suppressEdge = s0_closure.hasSemanticContext;
        s0_closure.hasSemanticContext = false;

        var next = AddDFAState(s0_closure);
        if (!suppressEdge)
        {
            decisionToDFA[mode].s0 = next;
        }

        int predict = ExecATN(input, next);

        if (debug)
        {
            Console.Out.WriteLine("DFA after matchATN: %s", decisionToDFA[old_mode].ToLexerString());
        }

        return predict;
    }

    protected virtual int ExecATN(CharStream input, DFAState ds0)
    {
        //Console.Out.println("enter exec index "+input.index()+" from "+ds0.configs);
        if (debug)
        {
            Console.Out.WriteLine("start state closure=%s", ds0.configs);
        }

        if (ds0.isAcceptState)
        {
            // allow zero-length tokens
            CaptureSimState(prevAccept, input, ds0);
        }

        int t = input.LA(1);

        var s = ds0; // s is current/from DFA state

        while (true)
        { // while more work
            if (debug)
            {
                Console.Out.WriteLine($"execATN loop starting closure: {s.configs}");
            }

            // As we move src->trg, src->trg, we keep track of the previous trg to
            // avoid looking up the DFA state again, which is expensive.
            // If the previous target was already part of the DFA, we might
            // be able to avoid doing a reach operation upon t. If s!=null,
            // it means that semantic predicates didn't prevent us from
            // creating a DFA state. Once we know s!=null, we check to see if
            // the DFA state has an edge already for t. If so, we can just reuse
            // it's configuration set; there's no point in re-computing it.
            // This is kind of like doing DFA simulation within the ATN
            // simulation because DFA simulation is really just a way to avoid
            // computing reach/closure sets. Technically, once we know that
            // we have a previously added DFA state, we could jump over to
            // the DFA simulator. But, that would mean popping back and forth
            // a lot and making things more complicated algorithmically.
            // This optimization makes a lot of sense for loops within DFA.
            // A character will take us back to an existing DFA state
            // that already has lots of edges out of it. e.g., .* in comments.
            var target = GetExistingTargetState(s, t);
            target ??= ComputeTargetState(input, s, t);

            if (target == ERROR) break;

            // If this is a consumable input element, make sure to consume before
            // capturing the accept state so the input index, line, and char
            // position accurately reflect the state of the interpreter at the
            // end of the token.
            if (t != IntStream.EOF) Consume(input);
            if (target.isAcceptState)
            {
                CaptureSimState(prevAccept, input, target);
                if (t == IntStream.EOF) break;
            }

            t = input.LA(1);
            s = target; // flip; current DFA target becomes new src/from state
        }

        return FailOrAccept(prevAccept, input, s.configs, t);
    }

    /**
	 * Get an existing target state for an edge in the DFA. If the target state
	 * for the edge has not yet been computed or is otherwise not available,
	 * this method returns {@code null}.
	 *
	 * @param s The current DFA state
	 * @param t The next input symbol
	 * @return The existing target DFA state for the given input symbol
	 * {@code t}, or {@code null} if the target state for this edge is not
	 * already cached
	 */

    protected virtual DFAState GetExistingTargetState(DFAState s, int t)
    {
        if (s.edges == null || t < MIN_DFA_EDGE || t > MAX_DFA_EDGE) return null;
        var target = s.edges[t - MIN_DFA_EDGE];
        if (debug && target != null)
        {
            Console.Out.WriteLine("reuse state " + s.stateNumber +
                               " edge to " + target.stateNumber);
        }

        return target;
    }

    /**
	 * Compute a target state for an edge in the DFA, and attempt to add the
	 * computed state and corresponding edge to the DFA.
	 *
	 * @param input The input stream
	 * @param s The current DFA state
	 * @param t The next input symbol
	 *
	 * @return The computed target DFA state for the given input symbol
	 * {@code t}. If {@code t} does not lead to a valid DFA state, this method
	 * returns {@link #ERROR}.
	 */

    protected virtual DFAState ComputeTargetState(CharStream input, DFAState s, int t)
    {
        var reach = new OrderedATNConfigSet();

        // if we don't find an existing DFA state
        // Fill reach starting from closure, following t transitions
        GetReachableConfigSet(input, s.configs, reach, t);

        if (reach.IsEmpty)
        { // we got nowhere on t from s
            if (!reach.hasSemanticContext)
            {
                // we got nowhere on t, don't throw out this knowledge; it'd
                // cause a failover from DFA later.
                AddDFAEdge(s, t, ERROR);
            }

            // stop when we can't match any more char
            return ERROR;
        }

        // Add an edge from s to target DFA found/created for reach
        return AddDFAEdge(s, t, reach);
    }

    protected virtual int FailOrAccept(SimState prevAccept, CharStream input,
                               ATNConfigSet reach, int t)
    {
        if (prevAccept.dfaState != null)
        {
            var lexerActionExecutor = prevAccept.dfaState.lexerActionExecutor;
            Accept(input, lexerActionExecutor, startIndex,
                prevAccept.index, prevAccept.line, prevAccept.charPos);
            return prevAccept.dfaState.prediction;
        }
        else
        {
            // if no accept and EOF is first char, return EOF
            if (t == IntStream.EOF && input.Index == startIndex) return Token.EOF;
            throw new LexerNoViableAltException(recog, input, startIndex, reach);
        }
    }

    /** Given a starting configuration set, figure out all ATN configurations
	 *  we can reach upon input {@code t}. Parameter {@code reach} is a return
	 *  parameter.
	 */
    protected virtual void GetReachableConfigSet(CharStream input, ATNConfigSet _closure, ATNConfigSet reach, int t)
    {
        // this is used to skip processing for configs which have a lower priority
        // than a config that already reached an accept state for the same rule
        int skipAlt = ATN.INVALID_ALT_NUMBER;
        foreach (var c in _closure)
        {
            bool currentAltReachedAcceptState = c.alt == skipAlt;
            if (currentAltReachedAcceptState && ((LexerATNConfig)c).HasPassedThroughNonGreedyDecision())
                continue;
            if (debug)
            {
                Console.Out.WriteLine("testing %s at %s", GetTokenName(t), c.ToString(recog, true));
            }

            int n = c.state.NumberOfTransitions;
            for (int ti = 0; ti < n; ti++)
            {               // for each transition
                var trans = c.state.Transition(ti);
                var target = GetReachableTarget(trans, t);
                if (target != null)
                {
                    var lexerActionExecutor = ((LexerATNConfig)c).GetLexerActionExecutor();
                    if (lexerActionExecutor != null)
                    {
                        lexerActionExecutor = lexerActionExecutor.FixOffsetBeforeMatch(input.Index - startIndex);
                    }

                    bool treatEofAsEpsilon = t == CharStream.EOF;
                    if (Closure(input, new LexerATNConfig((LexerATNConfig)c, target, lexerActionExecutor), reach, currentAltReachedAcceptState, true, treatEofAsEpsilon))
                    {
                        // any remaining configs for this alt have a lower priority than
                        // the one that just reached an accept state.
                        skipAlt = c.alt;
                        break;
                    }
                }
            }
        }
    }

    protected virtual void Accept(CharStream input, LexerActionExecutor lexerActionExecutor,
                          int startIndex, int index, int line, int charPos)
    {
        if (debug)
        {
            Console.Out.WriteLine("ACTION %s", lexerActionExecutor);
        }

        // seek to after last char in token
        input.Seek(index);
        this.line = line;
        this.charPositionInLine = charPos;

        if (lexerActionExecutor != null && recog != null)
        {
            lexerActionExecutor.Execute(recog, input, startIndex);
        }
    }


    protected virtual ATNState GetReachableTarget(Transition trans, int t) 
        => trans.Matches(t, Lexer.MIN_CHAR_VALUE, Lexer.MAX_CHAR_VALUE) ? trans.target : null;


    protected virtual ATNConfigSet ComputeStartState(CharStream input,
                                             ATNState p)
    {
        var initialContext = EmptyPredictionContext.Instance;
        var configs = new OrderedATNConfigSet();
        for (int i = 0; i < p.NumberOfTransitions; i++)
        {
            var target = p.Transition(i).target;
            var c = new LexerATNConfig(target, i + 1, initialContext);
            Closure(input, c, configs, false, false, false);
        }
        return configs;
    }

    /**
	 * Since the alternatives within any lexer decision are ordered by
	 * preference, this method stops pursuing the closure as soon as an accept
	 * state is reached. After the first accept state is reached by depth-first
	 * search from {@code config}, all other (potentially reachable) states for
	 * this rule would have a lower priority.
	 *
	 * @return {@code true} if an accept state is reached, otherwise
	 * {@code false}.
	 */
    protected virtual bool Closure(CharStream input, LexerATNConfig config, ATNConfigSet configs, bool currentAltReachedAcceptState, bool speculative, bool treatEofAsEpsilon)
    {
        if (debug)
        {
            Console.Out.WriteLine("closure(" + config.ToString(recog, true) + ")");
        }

        if (config.state is RuleStopState)
        {
            if (debug)
            {
                if (recog != null)
                {
                    Console.Out.WriteLine("closure at %s rule stop %s", recog.RuleNames[config.state.ruleIndex], config);
                }
                else
                {
                    Console.Out.WriteLine("closure at rule stop %s", config);
                }
            }

            if (config.context == null || config.context.HasEmptyPath)
            {
                if (config.context == null || config.context.IsEmpty)
                {
                    configs.Add(config);
                    return true;
                }
                else
                {
                    configs.Add(new LexerATNConfig(config, config.state, EmptyPredictionContext.Instance));
                    currentAltReachedAcceptState = true;
                }
            }

            if (config.context != null && !config.context.IsEmpty)
            {
                for (int i = 0; i < config.context.Count; i++)
                {
                    if (config.context.GetReturnState(i) != PredictionContext.EMPTY_RETURN_STATE)
                    {
                        var newContext = config.context.GetParent(i); // "pop" return state
                        var returnState = atn.states[config.context.GetReturnState(i)];
                        var c = new LexerATNConfig(config, returnState, newContext);
                        currentAltReachedAcceptState = Closure(input, c, configs, currentAltReachedAcceptState, speculative, treatEofAsEpsilon);
                    }
                }
            }

            return currentAltReachedAcceptState;
        }

        // optimization
        if (!config.state.OnlyHasEpsilonTransitions())
        {
            if (!currentAltReachedAcceptState || !config.HasPassedThroughNonGreedyDecision())
                configs.Add(config);
        }

        var p = config.state;
        for (int i = 0; i < p.NumberOfTransitions; i++)
        {
            var t = p.Transition(i);
            var c = GetEpsilonTarget(input, config, t, configs, speculative, treatEofAsEpsilon);
            if (c != null)
                currentAltReachedAcceptState = Closure(input, c, configs, currentAltReachedAcceptState, speculative, treatEofAsEpsilon);
        }

        return currentAltReachedAcceptState;
    }

    // side-effect: can alter configs.hasSemanticContext

    protected virtual LexerATNConfig GetEpsilonTarget(CharStream input,
                                           LexerATNConfig config,
                                           Transition t,
                                           ATNConfigSet configs,
                                           bool speculative,
                                           bool treatEofAsEpsilon)
    {
        LexerATNConfig c = null;
        switch (t.SerializationType)
        {
            case Transition.RULE:
                var ruleTransition = (RuleTransition)t;
                var newContext =
                    SingletonPredictionContext.Create(config.context, ruleTransition.followState.stateNumber);
                c = new LexerATNConfig(config, t.target, newContext);
                break;

            case Transition.PRECEDENCE:
                throw new UnsupportedOperationException("Precedence predicates are not supported in lexers.");

            case Transition.PREDICATE:
                /*  Track traversing semantic predicates. If we traverse,
				 we cannot add a DFA state for this "reach" computation
				 because the DFA would not test the predicate again in the
				 future. Rather than creating collections of semantic predicates
				 like v3 and testing them on prediction, v4 will test them on the
				 fly all the time using the ATN not the DFA. This is slower but
				 semantically it's not used that often. One of the key elements to
				 this predicate mechanism is not adding DFA states that see
				 predicates immediately afterwards in the ATN. For example,

				 a : ID {p1}? | ID {p2}? ;

				 should create the start state for rule 'a' (to save start state
				 competition), but should not create target of ID state. The
				 collection of ATN states the following ID references includes
				 states reached by traversing predicates. Since this is when we
				 test them, we cannot cash the DFA state target of ID.
			 */
                var pt = (PredicateTransition)t;
                if (debug)
                {
                    Console.Out.WriteLine("EVAL rule " + pt.ruleIndex + ":" + pt.predIndex);
                }
                configs.hasSemanticContext = true;
                if (EvaluatePredicate(input, pt.ruleIndex, pt.predIndex, speculative))
                {
                    c = new LexerATNConfig(config, t.target);
                }
                break;

            case Transition.ACTION:
                if (config.context == null || config.context.HasEmptyPath)
                {
                    // execute actions anywhere in the start rule for a token.
                    //
                    // TODO: if the entry rule is invoked recursively, some
                    // actions may be executed during the recursive call. The
                    // problem can appear when hasEmptyPath() is true but
                    // isEmpty() is false. In this case, the config needs to be
                    // split into two contexts - one with just the empty path
                    // and another with everything but the empty path.
                    // Unfortunately, the current algorithm does not allow
                    // getEpsilonTarget to return two configurations, so
                    // additional modifications are needed before we can support
                    // the split operation.
                    var lexerActionExecutor = LexerActionExecutor.Append(config.GetLexerActionExecutor(), atn.lexerActions[((ActionTransition)t).actionIndex]);
                    c = new LexerATNConfig(config, t.target, lexerActionExecutor);
                    break;
                }
                else
                {
                    // ignore actions in referenced rules
                    c = new LexerATNConfig(config, t.target);
                    break;
                }

            case Transition.EPSILON:
                c = new LexerATNConfig(config, t.target);
                break;

            case Transition.ATOM:
            case Transition.RANGE:
            case Transition.SET:
                if (treatEofAsEpsilon)
                {
                    if (t.Matches(CharStream.EOF, Lexer.MIN_CHAR_VALUE, Lexer.MAX_CHAR_VALUE))
                    {
                        c = new LexerATNConfig(config, t.target);
                        break;
                    }
                }

                break;
        }

        return c;
    }

    /**
	 * Evaluate a predicate specified in the lexer.
	 *
	 * <p>If {@code speculative} is {@code true}, this method was called before
	 * {@link #consume} for the matched character. This method should call
	 * {@link #consume} before evaluating the predicate to ensure position
	 * sensitive values, including {@link Lexer#getText}, {@link Lexer#getLine},
	 * and {@link Lexer#getCharPositionInLine}, properly reflect the current
	 * lexer state. This method should restore {@code input} and the simulator
	 * to the original state before returning (i.e. undo the actions made by the
	 * call to {@link #consume}.</p>
	 *
	 * @param input The input stream.
	 * @param ruleIndex The rule containing the predicate.
	 * @param predIndex The index of the predicate within the rule.
	 * @param speculative {@code true} if the current index in {@code input} is
	 * one character before the predicate's location.
	 *
	 * @return {@code true} if the specified predicate evaluates to
	 * {@code true}.
	 */
    protected virtual bool EvaluatePredicate(CharStream input, int ruleIndex, int predIndex, bool speculative)
    {
        // assume true if no recognizer was provided
        if (recog == null)
        {
            return true;
        }

        if (!speculative)
        {
            return recog.Sempred(null, ruleIndex, predIndex);
        }

        int savedCharPositionInLine = charPositionInLine;
        int savedLine = line;
        int index = input.Index;
        int marker = input.Mark();
        try
        {
            Consume(input);
            return recog.Sempred(null, ruleIndex, predIndex);
        }
        finally
        {
            charPositionInLine = savedCharPositionInLine;
            line = savedLine;
            input.Seek(index);
            input.Release(marker);
        }
    }

    protected virtual void CaptureSimState(SimState settings,
                                   CharStream input,
                                   DFAState dfaState)
    {
        settings.index = input.Index;
        settings.line = line;
        settings.charPos = charPositionInLine;
        settings.dfaState = dfaState;
    }


    protected virtual DFAState AddDFAEdge(DFAState from,
                                  int t,
                                  ATNConfigSet q)
    {
        /* leading to this call, ATNConfigSet.hasSemanticContext is used as a
		 * marker indicating dynamic predicate evaluation makes this edge
		 * dependent on the specific input sequence, so the static edge in the
		 * DFA should be omitted. The target DFAState is still created since
		 * execATN has the ability to resynchronize with the DFA state cache
		 * following the predicate evaluation step.
		 *
		 * TJP notes: next time through the DFA, we see a pred again and eval.
		 * If that gets us to a previously created (but dangling) DFA
		 * state, we can continue in pure DFA mode from there.
		 */
        bool suppressEdge = q.hasSemanticContext;
        q.hasSemanticContext = false;


        var to = AddDFAState(q);

        if (suppressEdge) return to;
        AddDFAEdge(from, t, to);
        return to;
    }

    protected static void AddDFAEdge(DFAState p, int t, DFAState q)
    {
        if (t < MIN_DFA_EDGE || t > MAX_DFA_EDGE)
        {
            // Only track edges within the DFA bounds
            return;
        }

        if (debug)
        {
            Console.Out.WriteLine("EDGE " + p + " -> " + q + " upon " + ((char)t));
        }

        lock (p)
        {
            //  make room for tokens 1..n and -1 masquerading as index 0
            p.edges ??= new DFAState[MAX_DFA_EDGE - MIN_DFA_EDGE + 1];
            p.edges[t - MIN_DFA_EDGE] = q; // connect
        }
    }

    /** Add a new DFA state if there isn't one with this set of
		configurations already. This method also detects the first
		configuration containing an ATN rule stop state. Later, when
		traversing the DFA, we will know which rule to accept.
	 */

    protected virtual DFAState AddDFAState(ATNConfigSet configs)
    {
        /* the lexer evaluates predicates on-the-fly; by this point configs
		 * should not contain any configurations with unevaluated predicates.
		 */
        //assert !configs.hasSemanticContext;

        var proposed = new DFAState(configs);
        ATNConfig firstConfigWithRuleStopState = null;
        foreach (var c in configs)
        {
            if (c.state is RuleStopState)
            {
                firstConfigWithRuleStopState = c;
                break;
            }
        }

        if (firstConfigWithRuleStopState != null)
        {
            proposed.isAcceptState = true;
            proposed.lexerActionExecutor = ((LexerATNConfig)firstConfigWithRuleStopState).GetLexerActionExecutor();
            proposed.prediction = atn.ruleToTokenType[firstConfigWithRuleStopState.state.ruleIndex];
        }

        var dfa = decisionToDFA[mode];
        lock (dfa.states)
        {
            if (dfa.states.TryGetValue(proposed, out var existing)) return existing;

            var newState = proposed;

            newState.stateNumber = dfa.states.Count;
            configs.            Readonly = true;
            newState.configs = configs;
            dfa.states[newState] = newState;
            return newState;
        }
    }


    public virtual DFA GetDFA(int mode) => decisionToDFA[mode];

    /** Get the text matched so far for the current token.
	 */

    public virtual string GetText(CharStream input) =>
        // index is first lookahead char, don't include.
        input.GetText(Interval.Of(startIndex, input.Index - 1));

    public virtual int Line { get => line; set => this.line = value; }

    public virtual int CharPositionInLine { get => charPositionInLine; set => this.charPositionInLine = value; }

    public virtual void Consume(CharStream input)
    {
        int curChar = input.LA(1);
        if (curChar == '\n')
        {
            line++;
            charPositionInLine = 0;
        }
        else
        {
            charPositionInLine++;
        }
        input.Consume();
    }


    public static string GetTokenName(int t) =>
        t == -1 ? "EOF" : "'" + char.ConvertFromUtf32(t) + "'";
    //if ( atn.g!=null ) return atn.g.getTokenDisplayName(t);
}
