/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime.atn;
using org.antlr.v4.runtime.dfa;
using org.antlr.v4.runtime.misc;
using org.antlr.v4.runtime.tree;

namespace org.antlr.v4.runtime;


/** A parser simulator that mimics what ANTLR's generated
 *  parser code does. A ParserATNSimulator is used to make
 *  predictions via adaptivePredict but this class moves a pointer through the
 *  ATN to simulate parsing. ParserATNSimulator just
 *  makes us efficient rather than having to backtrack, for example.
 *
 *  This properly creates parse trees even for left recursive rules.
 *
 *  We rely on the left recursive rule invocation and special predicate
 *  transitions to make left recursive rules work.
 *
 *  See TestParserInterpreter for examples.
 */
public class ParserInterpreter : Parser
{
    protected readonly String grammarFileName;
    protected readonly ATN atn;

    protected readonly DFA[] decisionToDFA; // not shared like it is for generated parsers
    protected readonly PredictionContextCache sharedContextCache = new PredictionContextCache();

    //@Deprecated
    protected readonly String[] tokenNames;
    protected readonly String[] ruleNames;

    private readonly Vocabulary vocabulary;

    /** This stack corresponds to the _parentctx, _parentState pair of locals
	 *  that would exist on call stack frames with a recursive descent parser;
	 *  in the generated function for a left-recursive rule you'd see:
	 *
	 *  private EContext e(int _p) throws RecognitionException {
	 *      ParserRuleContext _parentctx = _ctx;    // Pair.a
	 *      int _parentState = getState();          // Pair.b
	 *      ...
	 *  }
	 *
	 *  Those values are used to create new recursive rule invocation contexts
	 *  associated with left operand of an alt like "expr '*' expr".
	 */
    protected readonly ArrayDeque<Pair<ParserRuleContext, int>> _parentContextStack =
        new ();

    /** We need a map from (decision,inputIndex)->forced alt for computing ambiguous
	 *  parse trees. For now, we allow exactly one override.
	 */
    protected int overrideDecision = -1;
    protected int overrideDecisionInputIndex = -1;
    protected int overrideDecisionAlt = -1;
    protected bool overrideDecisionReached = false; // latch and only override once; error might trigger infinite loop

    /** What is the current context when we override a decisions?  This tells
	 *  us what the root of the parse tree is when using override
	 *  for an ambiguity/lookahead check.
	 */
    protected InterpreterRuleContext overrideDecisionRoot = null;


    protected InterpreterRuleContext rootContext;

    /**
	 * @deprecated Use {@link #ParserInterpreter(String, Vocabulary, Collection, ATN, TokenStream)} instead.
	 */
    //@Deprecated
    public ParserInterpreter(String grammarFileName, ICollection<String> tokenNames,
                             ICollection<String> ruleNames, ATN atn, TokenStream input) : this(grammarFileName, VocabularyImpl.FromTokenNames(tokenNames.ToArray()), ruleNames, atn, input)
    {
        ;
    }

    public ParserInterpreter(String grammarFileName, Vocabulary vocabulary,
                             ICollection<String> ruleNames, ATN atn, TokenStream input)
        : base(input)
    {
        ;
        this.grammarFileName = grammarFileName;
        this.atn = atn;
        this.tokenNames = new String[atn.maxTokenType];
        for (int i = 0; i < tokenNames.Length; i++)
        {
            tokenNames[i] = vocabulary.GetDisplayName(i);
        }

        this.ruleNames = ruleNames.ToArray();
        this.vocabulary = vocabulary;

        // init decision DFA
        int numberOfDecisions = atn.NumberOfDecisions();
        this.decisionToDFA = new DFA[numberOfDecisions];
        for (int i = 0; i < numberOfDecisions; i++)
        {
            DecisionState decisionState = atn.GetDecisionState(i);
            decisionToDFA[i] = new DFA(decisionState, i);
        }

        // get atn simulator that knows how to do predictions
        SetInterpreter(new ParserATNSimulator(this, atn,
                                              decisionToDFA,
                                              sharedContextCache));
    }

    //@Override
    public void reset()
    {
        base.Reset();
        overrideDecisionReached = false;
        overrideDecisionRoot = null;
    }

    //@Override
    public override ATN ATN => atn;

    //@Override
    //@Deprecated
    public override String[] TokenNames => tokenNames;

    //@Override
    public virtual Vocabulary getVocabulary()
    {
        return vocabulary;
    }

    //@Override
    public override String[] GetRuleNames()
    {
        return ruleNames;
    }

    //@Override
    public override String GrammarFileName => grammarFileName;

    /** Begin parsing at startRuleIndex */
    public ParserRuleContext parse(int startRuleIndex)
    {
        RuleStartState startRuleStartState = atn.ruleToStartState[startRuleIndex];

        rootContext = createInterpreterRuleContext(null, ATNState.INVALID_STATE_NUMBER, startRuleIndex);
        if (startRuleStartState.isLeftRecursiveRule)
        {
            enterRecursionRule(rootContext, startRuleStartState.stateNumber, startRuleIndex, 0);
        }
        else
        {
            EnterRule(rootContext, startRuleStartState.stateNumber, startRuleIndex);
        }

        while (true)
        {
            ATNState p = getATNState();
            switch (p.StateType)
            {
                case ATNState.RULE_STOP:
                    // pop; return from rule
                    if (_ctx.IsEmpty)
                    {
                        if (startRuleStartState.isLeftRecursiveRule)
                        {
                            ParserRuleContext result = _ctx;
                            Pair<ParserRuleContext, int> parentContext = _parentContextStack.Pop();
                            UnrollRecursionContexts(parentContext.a);
                            return result;
                        }
                        else
                        {
                            ExitRule();
                            return rootContext;
                        }
                    }

                    visitRuleStopState(p);
                    break;

                default:
                    try
                    {
                        visitState(p);
                    }
                    catch (RecognitionException e)
                    {
                        State = atn.ruleToStopState[p.ruleIndex].stateNumber;
                        Context.exception = e;
                        ErrorHandler.ReportError(this, e);
                        recover(e);
                    }

                    break;
            }
        }
    }

    //@Override
    public void enterRecursionRule(ParserRuleContext localctx, int state, int ruleIndex, int precedence)
    {
        Pair<ParserRuleContext, int> pair = new Pair<ParserRuleContext, int>(_ctx, localctx.invokingState);
        _parentContextStack.Push(pair);
        base.EnterRecursionRule(localctx, state, ruleIndex, precedence);
    }

    protected ATNState getATNState()
    {
        return atn.states[(State)];
    }

    protected void visitState(ATNState p)
    {
        //		Console.Out.WriteLine("visitState "+p.stateNumber);
        int predictedAlt = 1;
        if (p is DecisionState)
        {
            predictedAlt = visitDecisionState((DecisionState)p);
        }

        Transition transition = p.Transition(predictedAlt - 1);
        switch (transition.SerializationType)
        {
            case Transition.EPSILON:
                if (p.StateType == ATNState.STAR_LOOP_ENTRY &&
                     ((StarLoopEntryState)p).isPrecedenceDecision &&
                     !(transition.target is LoopEndState))
                {
                    // We are at the start of a left recursive rule's (...)* loop
                    // and we're not taking the exit branch of loop.
                    InterpreterRuleContext localctx =
                        createInterpreterRuleContext(_parentContextStack.Peek().a,
                                                     _parentContextStack.Peek().b,
                                                     _ctx.                                                     RuleIndex);
                    PushNewRecursionContext(localctx,
                                            atn.ruleToStartState[p.ruleIndex].stateNumber,
                                            _ctx.                                            RuleIndex);
                }
                break;

            case Transition.ATOM:
                Match(((AtomTransition)transition)._label);
                break;

            case Transition.RANGE:
            case Transition.SET:
            case Transition.NOT_SET:
                if (!transition.Matches(input.LA(1), Token.MIN_USER_TOKEN_TYPE, 65535))
                {
                    recoverInline();
                }
                MatchWildcard();
                break;

            case Transition.WILDCARD:
                MatchWildcard();
                break;

            case Transition.RULE:
                RuleStartState ruleStartState = (RuleStartState)transition.target;
                int ruleIndex = ruleStartState.ruleIndex;
                InterpreterRuleContext newctx = createInterpreterRuleContext(_ctx, p.stateNumber, ruleIndex);
                if (ruleStartState.isLeftRecursiveRule)
                {
                    enterRecursionRule(newctx, ruleStartState.stateNumber, ruleIndex, ((RuleTransition)transition).precedence);
                }
                else
                {
                    EnterRule(newctx, transition.target.stateNumber, ruleIndex);
                }
                break;

            case Transition.PREDICATE:
                PredicateTransition predicateTransition = (PredicateTransition)transition;
                if (!Sempred(_ctx, predicateTransition.ruleIndex, predicateTransition.predIndex))
                {
                    throw new FailedPredicateException(this);
                }

                break;

            case Transition.ACTION:
                ActionTransition actionTransition = (ActionTransition)transition;
                Action(_ctx, actionTransition.ruleIndex, actionTransition.actionIndex);
                break;

            case Transition.PRECEDENCE:
                if (!Precpred(_ctx, ((PrecedencePredicateTransition)transition).precedence))
                {
                    throw new FailedPredicateException(this, $"precpred(_ctx, {(((PrecedencePredicateTransition)transition).precedence)})");
                }
                break;

            default:
                throw new UnsupportedOperationException("Unrecognized ATN transition type.");
        }

        State = transition.target.stateNumber;
    }

    /** Method visitDecisionState() is called when the interpreter reaches
	 *  a decision state (instance of DecisionState). It gives an opportunity
	 *  for subclasses to track interesting things.
	 */
    protected int visitDecisionState(DecisionState p)
    {
        int predictedAlt = 1;
        if (p.NumberOfTransitions > 1)
        {
            ErrorHandler.Sync(this);
            int decision = p.decision;
            if (decision == overrideDecision && input.Index == overrideDecisionInputIndex &&
                 !overrideDecisionReached)
            {
                predictedAlt = overrideDecisionAlt;
                overrideDecisionReached = true;
            }
            else
            {
                predictedAlt = GetInterpreter().AdaptivePredict(input, decision, _ctx);
            }
        }
        return predictedAlt;
    }

    /** Provide simple "factory" for InterpreterRuleContext's.
	 *  @since 4.5.1
	 */
    protected InterpreterRuleContext createInterpreterRuleContext(
        ParserRuleContext parent,
        int invokingStateNumber,
        int ruleIndex)
    {
        return new InterpreterRuleContext(parent, invokingStateNumber, ruleIndex);
    }

    protected void visitRuleStopState(ATNState p)
    {
        RuleStartState ruleStartState = atn.ruleToStartState[p.ruleIndex];
        if (ruleStartState.isLeftRecursiveRule)
        {
            Pair<ParserRuleContext, int> parentContext = _parentContextStack.Pop();
            UnrollRecursionContexts(parentContext.a);
            State = parentContext.b;
        }
        else
        {
            ExitRule();
        }

        RuleTransition ruleTransition = (RuleTransition)atn.states[(State)].Transition(0);
        State = ruleTransition.followState.stateNumber;
    }

    /** Override this parser interpreters normal decision-making process
	 *  at a particular decision and input token index. Instead of
	 *  allowing the adaptive prediction mechanism to choose the
	 *  first alternative within a block that leads to a successful parse,
	 *  force it to take the alternative, 1..n for n alternatives.
	 *
	 *  As an implementation limitation right now, you can only specify one
	 *  override. This is sufficient to allow construction of different
	 *  parse trees for ambiguous input. It means re-parsing the entire input
	 *  in general because you're never sure where an ambiguous sequence would
	 *  live in the various parse trees. For example, in one interpretation,
	 *  an ambiguous input sequence would be matched completely in expression
	 *  but in another it could match all the way back to the root.
	 *
	 *  s : e '!'? ;
	 *  e : ID
	 *    | ID '!'
	 *    ;
	 *
	 *  Here, x! can be matched as (s (e ID) !) or (s (e ID !)). In the first
	 *  case, the ambiguous sequence is fully contained only by the root.
	 *  In the second case, the ambiguous sequences fully contained within just
	 *  e, as in: (e ID !).
	 *
	 *  Rather than trying to optimize this and make
	 *  some intelligent decisions for optimization purposes, I settled on
	 *  just re-parsing the whole input and then using
	 *  {link Trees#getRootOfSubtreeEnclosingRegion} to find the minimal
	 *  subtree that contains the ambiguous sequence. I originally tried to
	 *  record the call stack at the point the parser detected and ambiguity but
	 *  left recursive rules create a parse tree stack that does not reflect
	 *  the actual call stack. That impedance mismatch was enough to make
	 *  it it challenging to restart the parser at a deeply nested rule
	 *  invocation.
	 *
	 *  Only parser interpreters can override decisions so as to avoid inserting
	 *  override checking code in the critical ALL(*) prediction execution path.
	 *
	 *  @since 4.5.1
	 */
    public void addDecisionOverride(int decision, int tokenIndex, int forcedAlt)
    {
        overrideDecision = decision;
        overrideDecisionInputIndex = tokenIndex;
        overrideDecisionAlt = forcedAlt;
    }

    public InterpreterRuleContext getOverrideDecisionRoot()
    {
        return overrideDecisionRoot;
    }

    /** Rely on the error handler for this parser but, if no tokens are consumed
	 *  to recover, add an error node. Otherwise, nothing is seen in the parse
	 *  tree.
	 */
    protected void recover(RecognitionException e)
    {
        int i = input.Index;
        ErrorHandler.Recover(this, e);
        if (input.Index == i)
        {
            // no input consumed, better add an error node
            if (e is InputMismatchException)
            {
                InputMismatchException ime = (InputMismatchException)e;
                Token tok = e.OffendingToken;
                int expectedTokenType = Token.INVALID_TYPE;
                if (!ime.GetExpectedTokens().IsNil)
                {
                    expectedTokenType = ime.GetExpectedTokens().GetMinElement(); // get any element
                }
                Token errToken =
                    (TokenFactory as TokenFactory<Token>).Create(new Pair<TokenSource, CharStream>(tok.TokenSource, tok.TokenSource.InputStream),
                                             expectedTokenType, tok.Text,
                                             Token.DEFAULT_CHANNEL,
                                            -1, -1, // invalid start/stop
                                             tok.Line, tok.CharPositionInLine);
                _ctx.addErrorNode(CreateErrorNode(_ctx, errToken));
            }
            else
            { // NoViableAlt
                Token tok = e.OffendingToken;
                Token errToken =
                    (TokenFactory as TokenFactory<Token>).Create(new Pair<TokenSource, CharStream>(tok.TokenSource, tok.TokenSource.InputStream),
                                             Token.INVALID_TYPE, tok.Text,
                                             Token.DEFAULT_CHANNEL,
                                            -1, -1, // invalid start/stop
                                             tok.Line, tok.CharPositionInLine);
                _ctx.addErrorNode(CreateErrorNode(_ctx, errToken));
            }
        }
    }

    protected Token recoverInline()
    {
        return _errHandler.RecoverInline(this);
    }

    /** Return the root of the parse, which can be useful if the parser
	 *  bails out. You still can access the top node. Note that,
	 *  because of the way left recursive rules add children, it's possible
	 *  that the root will not have any children if the start rule immediately
	 *  called and left recursive rule that fails.
	 *
	 * @since 4.5.1
	 */
    public InterpreterRuleContext getRootContext()
    {
        return rootContext;
    }
}

