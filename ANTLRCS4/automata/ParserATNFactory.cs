/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.runtime.tree;
using org.antlr.v4.analysis;
using org.antlr.v4.misc;
using org.antlr.v4.parse;
using org.antlr.v4.runtime;
using org.antlr.v4.runtime.atn;
using org.antlr.v4.runtime.dfa;
using org.antlr.v4.runtime.misc;
using org.antlr.v4.runtime.tree;
using org.antlr.v4.semantics;
using org.antlr.v4.tool;
using org.antlr.v4.tool.ast;
using System.Reflection;
using static org.antlr.v4.automata.ATNFactory;

namespace org.antlr.v4.automata;

/** ATN construction routines triggered by ATNBuilder.g.
 *
 *  No side-effects. It builds an {@link ATN} object and returns it.
 */
public class ParserATNFactory : ATNFactory
{
    public readonly Grammar g;

    public readonly ATN atn;

    public Rule currentRule;

    public int currentOuterAlt;


    protected readonly List<Triple<Rule, ATNState, ATNState>> preventEpsilonClosureBlocks =
        new();


    protected readonly List<Triple<Rule, ATNState, ATNState>> preventEpsilonOptionalBlocks =
        new();

    public ParserATNFactory(Grammar g)
    {
        this.g = g ?? throw new NullReferenceException(nameof(g));

        var atnType = g is LexerGrammar ? ATNType.LEXER : ATNType.PARSER;
        int maxTokenType = g.getMaxTokenType();
        this.atn = new ATN(atnType, maxTokenType);
    }


    ////@Override
    public ATN CreateATN()
    {
        CreateATN(g.rules.Values);
        //assert atn.maxTokenType == g.getMaxTokenType();
        AddRuleFollowLinks();
        addEOFTransitionToStartRules();
        ATNOptimizer.Optimize(g, atn);
        CheckEpsilonClosure();

    optionalCheck:
        foreach (var pair in preventEpsilonOptionalBlocks)
        {
            int bypassCount = 0;
            for (int i = 0; i < pair.b.NumberOfTransitions; i++)
            {
                var startState = pair.b.Transition(i).target;
                if (startState == pair.c)
                {
                    bypassCount++;
                    continue;
                }

                var analyzer = new LL1Analyzer(atn);
                if (analyzer.LOOK(startState, pair.c, null).Contains(org.antlr.v4.runtime.Token.EPSILON))
                {
                    g.Tools.ErrMgr.GrammarError(ErrorType.EPSILON_OPTIONAL, g.fileName, ((GrammarAST)pair.a.ast.getChild(0)).getToken(), pair.a.name);
                    break;
                    //continue optionalCheck;
                }
            }

            if (bypassCount != 1)
                throw new UnsupportedOperationException("Expected optional block with exactly 1 bypass alternative.");
        }

        return atn;
    }

    protected void CheckEpsilonClosure()
    {
        foreach (var pair in preventEpsilonClosureBlocks)
        {
            var analyzer = new LL1Analyzer(atn);
            var blkStart = pair.b;
            var blkStop = pair.c;
            var lookahead = analyzer.LOOK(blkStart, blkStop, null);
            if (lookahead.Contains(org.antlr.v4.runtime.Token.EPSILON))
            {
                ErrorType errorType = pair.a is LeftRecursiveRule ? ErrorType.EPSILON_LR_FOLLOW : ErrorType.EPSILON_CLOSURE;
                g.Tools.ErrMgr.GrammarError(errorType, g.fileName, ((GrammarAST)pair.a.ast.getChild(0)).getToken(), pair.a.name);
            }
            if (lookahead.Contains(org.antlr.v4.runtime.Token.EOF))
            {
                g.Tools.ErrMgr.GrammarError(ErrorType.EOF_CLOSURE, g.fileName, ((GrammarAST)pair.a.ast.getChild(0)).getToken(), pair.a.name);
            }
        }
    }

    protected void CreateATN(ICollection<Rule> rules)
    {
        CreateRuleStartAndStopATNStates();

        var adaptor = new GrammarASTAdaptor();
        foreach (var r in rules)
        {
            // find rule's block
            var blk = (GrammarAST)r.ast.getFirstChildWithType(ANTLRParser.BLOCK);
            var nodes = new CommonTreeNodeStream(adaptor, blk);
            var b = new ATNBuilder(nodes, this);
            try
            {
                SetCurrentRuleName(r.name);
                var h = b.ruleBlock(null);
                Rule(r.ast, r.name, h);
            }
            catch (RecognitionException re)
            {
                ErrorManager.fatalInternalError("bad grammar AST structure", re);
            }
        }
    }

    //@Override
    public void SetCurrentRuleName(string name)
    {
        this.currentRule = g.getRule(name);
    }

    //@Override
    public void SetCurrentOuterAlt(int alt)
    {
        currentOuterAlt = alt;
    }

    /* start->ruleblock->end */

    //@Override
    public Handle Rule(GrammarAST ruleAST, String name, Handle blk)
    {
        var r = g.getRule(name);
        var start = atn.ruleToStartState[r.index];
        Epsilon(start, blk.left);
        var stop = atn.ruleToStopState[r.index];
        Epsilon(blk.right, stop);
        var h = new Handle(start, stop);
        //		ATNPrinter ser = new ATNPrinter(g, h.left);
        //		Console.Out.WriteLine(ruleAST.toStringTree()+":\n"+ser.asString());
        ruleAST.atnState = start;
        return h;
    }

    /** From label {@code A} build graph {@code o-A->o}. */

    //@Override
    public Handle TokenRef(TerminalAST node)
    {
        var left = NewState(node);
        var right = NewState(node);
        int ttype = g.getTokenType(node.getText());
        left.AddTransition(new AtomTransition(right, ttype));
        node.atnState = left;
        return new Handle(left, right);
    }

    /** From set build single edge graph {@code o->o-set->o}.  To conform to
     *  what an alt block looks like, must have extra state on left.
	 *  This also handles {@code ~A}, converted to {@code ~{A}} set.
     */

    //@Override
    public Handle Set(GrammarAST associatedAST, List<GrammarAST> terminals, bool invert)
    {
        var left = NewState(associatedAST);
        var right = NewState(associatedAST);
        var set = new IntervalSet();
        foreach (var t in terminals)
        {
            int ttype = g.getTokenType(t.getText());
            set.Add(ttype);
        }
        if (invert)
        {
            left.AddTransition(new NotSetTransition(right, set));
        }
        else
        {
            left.AddTransition(new SetTransition(right, set));
        }
        associatedAST.atnState = left;
        return new Handle(left, right);
    }

    /** Not valid for non-lexers. */

    //@Override
    public Handle Range(GrammarAST a, GrammarAST b)
    {
        g.Tools.ErrMgr.GrammarError(ErrorType.TOKEN_RANGE_IN_PARSER, g.fileName,
                                   a.getToken(),
                                   a.getToken().getText(),
                                   b.getToken().getText());
        // From a..b, yield ATN for just a.
        return TokenRef((TerminalAST)a);
    }

    protected int GetTokenType(GrammarAST atom) 
        => g.isLexer() ? CharSupport.GetCharValueFromGrammarCharLiteral(atom.getText()) : g.getTokenType(atom.getText());

    /** For a non-lexer, just build a simple token reference atom. */

    //@Override
    public Handle StringLiteral(TerminalAST stringLiteralAST)
        => TokenRef(stringLiteralAST);

    /** {@code [Aa]} char sets not allowed in parser */

    //@Override
    public Handle CharSetLiteral(GrammarAST charSetAST) => null;

    /**
	 * For reference to rule {@code r}, build
	 *
	 * <pre>
	 *  o-&gt;(r)  o
	 * </pre>
	 *
	 * where {@code (r)} is the start of rule {@code r} and the trailing
	 * {@code o} is not linked to from rule ref state directly (uses
	 * {@link RuleTransition#followState}).
	 */

    //@Override
    public Handle RuleRef(GrammarAST node) => GetRuleRef(node);


    public Handle GetRuleRef(GrammarAST node)
    {
        var r = g.getRule(node.getText());
        if (r == null)
        {
            g.Tools.ErrMgr.GrammarError(ErrorType.INTERNAL_ERROR, g.fileName, node.getToken(), "Rule " + node.getText() + " undefined");
            return null;
        }
        var start = atn.ruleToStartState[r.index];
        var left = NewState(node);
        var right = NewState(node);
        int precedence = 0;
        if (((GrammarASTWithOptions)node).getOptionString(LeftRecursiveRuleTransformer.PRECEDENCE_OPTION_NAME) != null)
        {
            if (int.TryParse(((GrammarASTWithOptions)node).getOptionString(LeftRecursiveRuleTransformer.PRECEDENCE_OPTION_NAME)
                , out var pre))
            {
                precedence = pre;
            }
            else
            {
                throw new InvalidOperationException(nameof(LeftRecursiveRuleTransformer.PRECEDENCE_OPTION_NAME));
            }
        }
        var call = new RuleTransition(start, r.index, precedence, right);
        left.AddTransition(call);

        node.atnState = left;
        return new Handle(left, right);
    }

    public void AddFollowLink(int ruleIndex, ATNState right)
    {
        // add follow edge from end of invoked rule
        var stop = atn.ruleToStopState[ruleIndex];
        //        Console.Out.WriteLine("add follow link from "+ruleIndex+" to "+right);
        Epsilon(stop, right);
    }

    /** From an empty alternative build {@code o-e->o}. */

    //@Override
    public Handle Epsilon(GrammarAST node)
    {
        var left = NewState(node);
        var right = NewState(node);
        Epsilon(left, right);
        node.atnState = left;
        return new Handle(left, right);
    }

    /** Build what amounts to an epsilon transition with a semantic
	 *  predicate action.  The {@code pred} is a pointer into the AST of
	 *  the {@link ANTLRParser#SEMPRED} token.
	 */

    //@Override
    public Handle Sempred(PredAST pred)
    {
        //Console.Out.WriteLine("sempred: "+ pred);
        var left = NewState(pred);
        var right = NewState(pred);

        AbstractPredicateTransition p;
        if (pred.getOptionString(LeftRecursiveRuleTransformer.PRECEDENCE_OPTION_NAME) != null)
        {
            if (int.TryParse(pred.getOptionString(LeftRecursiveRuleTransformer.PRECEDENCE_OPTION_NAME), out var pre))
            {
                int precedence = pre;

                p = new PrecedencePredicateTransition(right, precedence);
            }
            else
            {
                throw new InvalidOperationException(nameof(LeftRecursiveRuleTransformer.PRECEDENCE_OPTION_NAME));
            }
        }
        else
        {
            bool isCtxDependent = UseDefAnalyzer.actionIsContextDependent(pred);
            if (g.sempreds.TryGetValue(pred, out var r))
            {
                p = new PredicateTransition(right, currentRule.index, r, isCtxDependent);
            }
            else
            {
                throw new InvalidOperationException(nameof(g.sempreds));
            }
        }

        left.AddTransition(p);
        pred.atnState = left;
        return new Handle(left, right);
    }

    /** Build what amounts to an epsilon transition with an action.
	 *  The action goes into ATN though it is ignored during prediction
	 *  if {@link ActionTransition#actionIndex actionIndex}{@code <0}.
	 */

    //@Override
    public Handle Action(ActionAST action)
    {
        //Console.Out.WriteLine("action: "+action);
        var left = NewState(action);
        var right = NewState(action);
        var a = new ActionTransition(right, currentRule.index);
        left.AddTransition(a);
        action.atnState = left;
        return new Handle(left, right);
    }


    //@Override
    public Handle Action(String action)
        => throw new UnsupportedOperationException("This element is not valid in parsers.");

    /**
	 * From {@code A|B|..|Z} alternative block build
	 *
	 * <pre>
	 *  o-&gt;o-A-&gt;o-&gt;o (last ATNState is BlockEndState pointed to by all alts)
	 *  |          ^
	 *  |-&gt;o-B-&gt;o--|
	 *  |          |
	 *  ...        |
	 *  |          |
	 *  |-&gt;o-Z-&gt;o--|
	 * </pre>
	 *
	 * So start node points at every alternative with epsilon transition and
	 * every alt right side points at a block end ATNState.
	 * <p>
	 * Special case: only one alternative: don't make a block with alt
	 * begin/end.
	 * <p>
	 * Special case: if just a list of tokens/chars/sets, then collapse to a
	 * single edged o-set-&gt;o graph.
	 * <p>
	 * TODO: Set alt number (1..n) in the states?
	 */

    //@Override
    public Handle Block(BlockAST blkAST, GrammarAST ebnfRoot, List<Handle> alts)
    {
        if (ebnfRoot == null)
        {
            if (alts.Count == 1)
            {
                var h = alts[0];
                blkAST.atnState = h.left;
                return h;
            }
            var start = newState<BasicBlockStartState>(typeof(BasicBlockStartState), blkAST);
            if (alts.Count > 1) atn.DefineDecisionState(start);
            return MakeBlock(start, blkAST, alts);
        }
        switch (ebnfRoot.getType())
        {
            case ANTLRParser.OPTIONAL:
                var start = newState<BasicBlockStartState>(typeof(BasicBlockStartState), blkAST);
                atn.DefineDecisionState(start);
                Handle h = MakeBlock(start, blkAST, alts);
                return Optional(ebnfRoot, h);
            case ANTLRParser.CLOSURE:
                var _star = newState<BasicBlockStartState>(typeof(StarBlockStartState), ebnfRoot);
                if (alts.Count > 1) atn.DefineDecisionState(_star);
                h = MakeBlock(_star, blkAST, alts);
                return Star(ebnfRoot, h);
            case ANTLRParser.POSITIVE_CLOSURE:
                var _plus = newState<PlusBlockStartState>(typeof(PlusBlockStartState), ebnfRoot);
                if (alts.Count > 1) atn.DefineDecisionState(_plus);
                h = MakeBlock(_plus, blkAST, alts);
                return Plus(ebnfRoot, h);
        }
        return null;
    }


    protected Handle MakeBlock(BlockStartState start, BlockAST blkAST, List<Handle> alts)
    {
        var end = newState<BlockEndState>(typeof(BlockEndState), blkAST);
        start.endState = end;
        foreach (Handle alt in alts)
        {
            // hook alts up to decision block
            Epsilon(start, alt.left);
            Epsilon(alt.right, end);
            // no back link in ATN so must walk entire alt to see if we can
            // strip out the epsilon to 'end' state
            TailEpsilonRemover opt = new TailEpsilonRemover(atn);
            opt.Visit(alt.left);
        }
        var h = new Handle(start, end);
        //		FASerializer ser = new FASerializer(g, h.left);
        //		Console.Out.WriteLine(blkAST.toStringTree()+":\n"+ser);
        blkAST.atnState = start;

        return h;
    }


    //@Override
    public Handle Alt(List<Handle> els) => ElemList(els);


    public Handle ElemList(List<Handle> els)
    {
        int n = els.Count;
        for (int i = 0; i < n - 1; i++)
        {   // hook up elements (visit all but last)
            var el = els[(i)];
            // if el is of form o-x->o for x in {rule, action, pred, token, ...}
            // and not last in alt
            Transition tr = null;
            if (el.left.NumberOfTransitions == 1) tr = el.left.Transition(0);
            bool isRuleTrans = tr is RuleTransition;
            if (el.left.StateType == ATNState.BASIC &&
                el.right != null &&
                el.right.                StateType == ATNState.BASIC &&
                tr != null && (isRuleTrans && ((RuleTransition)tr).followState == el.right || tr.target == el.right))
            {
                // we can avoid epsilon edge to next el
                Handle handle = null;
                if (i + 1 < els.Count)
                {
                    handle = els[(i + 1)];
                }
                if (handle != null)
                {
                    if (isRuleTrans)
                    {
                        ((RuleTransition)tr).followState = handle.left;
                    }
                    else
                    {
                        tr.target = handle.left;
                    }
                }
                atn.RemoveState(el.right); // we skipped over this state
            }
            else
            { // need epsilon if previous block's right end node is complicated
                Epsilon(el.right, els[(i + 1)].left);
            }
        }
        var first = els[(0)];
        var last = els[(n - 1)];
        ATNState left = null;
        if (first != null)
        {
            left = first.left;
        }
        ATNState right = null;
        if (last != null)
        {
            right = last.right;
        }
        return new Handle(left, right);
    }

    /**
	 * From {@code (A)?} build either:
	 *
	 * <pre>
	 *  o--A-&gt;o
	 *  |     ^
	 *  o----&gt;|
	 * </pre>
	 *
	 * or, if {@code A} is a block, just add an empty alt to the end of the
	 * block
	 */

    //@Override
    public Handle Optional(GrammarAST optAST, Handle blk)
    {
        var blkStart = (BlockStartState)blk.left;
        var blkEnd = blk.right;
        preventEpsilonOptionalBlocks.Add(new (currentRule, blkStart, blkEnd));

        bool greedy = ((QuantifierAST)optAST).isGreedy();
        blkStart.nonGreedy = !greedy;
        Epsilon(blkStart, blk.right, !greedy);

        optAST.atnState = blk.left;
        return blk;
    }

    /**
	 * From {@code (blk)+} build
	 *
	 * <pre>
	 *   |---------|
	 *   v         |
	 *  [o-blk-o]-&gt;o-&gt;o
	 * </pre>
	 *
	 * We add a decision for loop back node to the existing one at {@code blk}
	 * start.
	 */

    //@Override
    public Handle Plus(GrammarAST plusAST, Handle blk)
    {
        var blkStart = (PlusBlockStartState)blk.left;
        var blkEnd = (BlockEndState)blk.right;
        preventEpsilonClosureBlocks.Add(new Triple<Rule, ATNState, ATNState>(currentRule, blkStart, blkEnd));

        var loop = newState<PlusLoopbackState>(plusAST);
        loop.nonGreedy = !((QuantifierAST)plusAST).isGreedy();
        atn.DefineDecisionState(loop);
        var end = newState<LoopEndState>(plusAST);
        blkStart.loopBackState = loop;
        end.loopBackState = loop;

        plusAST.atnState = loop;
        Epsilon(blkEnd, loop);      // blk can see loop back

        var blkAST = (BlockAST)plusAST.getChild(0);
        if (((QuantifierAST)plusAST).isGreedy())
        {
            if (expectNonGreedy(blkAST))
            {
                g.Tools.ErrMgr.GrammarError(ErrorType.EXPECTED_NON_GREEDY_WILDCARD_BLOCK, g.fileName, plusAST.getToken(), plusAST.getToken().getText());
            }

            Epsilon(loop, blkStart);    // loop back to start
            Epsilon(loop, end);         // or exit
        }
        else
        {
            // if not greedy, priority to exit branch; make it first
            Epsilon(loop, end);         // exit
            Epsilon(loop, blkStart);    // loop back to start
        }

        return new Handle(blkStart, end);
    }

    /**
	 * From {@code (blk)*} build {@code ( blk+ )?} with *two* decisions, one for
	 * entry and one for choosing alts of {@code blk}.
	 *
	 * <pre>
	 *   |-------------|
	 *   v             |
	 *   o--[o-blk-o]-&gt;o  o
	 *   |                ^
	 *   -----------------|
	 * </pre>
	 *
	 * Note that the optional bypass must jump outside the loop as
	 * {@code (A|B)*} is not the same thing as {@code (A|B|)+}.
	 */

    //@Override
    public Handle Star(GrammarAST starAST, Handle elem)
    {
        var blkStart = (StarBlockStartState)elem.left;
        var blkEnd = (BlockEndState)elem.right;
        preventEpsilonClosureBlocks.Add(new Triple<Rule, ATNState, ATNState>(currentRule, blkStart, blkEnd));

        var entry = newState<StarLoopEntryState>(starAST);
        entry.nonGreedy = !((QuantifierAST)starAST).isGreedy();
        atn.DefineDecisionState(entry);
        LoopEndState end = newState<LoopEndState>(starAST);
        StarLoopbackState loop = newState<StarLoopbackState>(starAST);
        entry.loopBackState = loop;
        end.loopBackState = loop;

        var blkAST = (BlockAST)starAST.getChild(0);
        if (((QuantifierAST)starAST).isGreedy())
        {
            if (expectNonGreedy(blkAST))
            {
                g.Tools.ErrMgr.GrammarError(ErrorType.EXPECTED_NON_GREEDY_WILDCARD_BLOCK, g.fileName, starAST.getToken(), starAST.getToken().getText());
            }

            Epsilon(entry, blkStart);   // loop enter edge (alt 1)
            Epsilon(entry, end);        // bypass loop edge (alt 2)
        }
        else
        {
            // if not greedy, priority to exit branch; make it first
            Epsilon(entry, end);        // bypass loop edge (alt 1)
            Epsilon(entry, blkStart);   // loop enter edge (alt 2)
        }
        Epsilon(blkEnd, loop);      // block end hits loop back
        Epsilon(loop, entry);       // loop back to entry/exit decision

        starAST.atnState = entry;   // decision is to enter/exit; blk is its own decision
        return new Handle(entry, end);
    }

    /** Build an atom with all possible values in its label. */

    //@Override
    public Handle Wildcard(GrammarAST node)
    {
        var left = NewState(node);
        var right = NewState(node);
        left.AddTransition(new WildcardTransition(right));
        node.atnState = left;
        return new (left, right);
    }

    protected void Epsilon(ATNState a, ATNState b) => Epsilon(a, b, false);

    protected void Epsilon(ATNState a, ATNState b, bool prepend)
    {
        if (a != null)
        {
            int index = prepend ? 0 : a.NumberOfTransitions;
            a.AddTransition(index, new EpsilonTransition(b));
        }
    }

    /** Define all the rule begin/end ATNStates to solve forward reference
	 *  issues.
	 */
    void CreateRuleStartAndStopATNStates()
    {
        atn.ruleToStartState = new RuleStartState[g.rules.Count];
        atn.ruleToStopState = new RuleStopState[g.rules.Count];
        foreach (var r in g.rules.Values)
        {
            var start = newState<RuleStartState>(r.ast);
            var stop = newState<RuleStopState>(r.ast);
            start.stopState = stop;
            start.isLeftRecursiveRule = r is LeftRecursiveRule;
            start.SetRuleIndex(r.index);
            stop.SetRuleIndex(r.index);
            atn.ruleToStartState[r.index] = start;
            atn.ruleToStopState[r.index] = stop;
        }
    }

    public void AddRuleFollowLinks()
    {
        foreach (var p in atn.states)
        {
            if (p != null &&
                 p.                 StateType == ATNState.BASIC && p.NumberOfTransitions == 1 &&
                 p.Transition(0) is RuleTransition)
            {
                var rt = (RuleTransition)p.Transition(0);
                AddFollowLink(rt.ruleIndex, rt.followState);
            }
        }
    }

    /** Add an EOF transition to any rule end ATNState that points to nothing
     *  (i.e., for all those rules not invoked by another rule).  These
     *  are start symbols then.
	 *
	 *  Return the number of grammar entry points; i.e., how many rules are
	 *  not invoked by another rule (they can only be invoked from outside).
	 *  These are the start rules.
     */
    public int addEOFTransitionToStartRules()
    {
        int n = 0;
        ATNState eofTarget = NewState(null); // one unique EOF target for all rules
        foreach (Rule r in g.rules.Values)
        {
            ATNState stop = atn.ruleToStopState[r.index];
            if (stop.NumberOfTransitions > 0) continue;
            n++;
            Transition t = new AtomTransition(eofTarget, Token.EOF);
            stop.AddTransition(t);
        }
        return n;
    }


    //@Override
    public Handle Label(Handle t)
    {
        return t;
    }


    //@Override
    public Handle ListLabel(Handle t)
    {
        return t;
    }

    public T newState<T>(GrammarAST node) where T : ATNState => newState<T>(typeof(T), node);

    protected T newState<T>(Type nodeType, GrammarAST node) where T : ATNState
    {
        Exception cause;
        try
        {
            ConstructorInfo ctor = nodeType.GetConstructor(Array.Empty<Type>());
            T s = ctor.Invoke(Array.Empty<object>()) as T;
            if (currentRule == null) s.SetRuleIndex(-1);
            else s.SetRuleIndex(currentRule.index);
            atn.AddState(s);
            return s;
        }
        catch (Exception ex)
        {
            cause = ex;
        }

        String message = $"Could not create {typeof(ATNState).Name} of type {nodeType.Name}.";
        throw new UnsupportedOperationException(message, cause);
    }


    public ATNState NewState(GrammarAST node)
    {
        ATNState n = new BasicState();
        n.SetRuleIndex(currentRule.index);
        atn.AddState(n);
        return n;
    }


    //@Override
    public ATNState NewState() { return NewState(null); }

    public bool expectNonGreedy(BlockAST blkAST)
    {
        if (blockHasWildcardAlt(blkAST))
        {
            return true;
        }

        return false;
    }

    /**
	 * {@code (BLOCK (ALT .))} or {@code (BLOCK (ALT 'a') (ALT .))}.
	 */
    public static bool blockHasWildcardAlt(GrammarAST block)
    {
        foreach (Object alt in block.getChildren())
        {
            if (!(alt is AltAST)) continue;
            AltAST altAST = (AltAST)alt;
            if (altAST.getChildCount() == 1 || (altAST.getChildCount() == 2 && altAST.getChild(0).getType() == ANTLRParser.ELEMENT_OPTIONS))
            {
                Tree e = altAST.getChild(altAST.getChildCount() - 1);
                if (e.getType() == ANTLRParser.WILDCARD)
                {
                    return true;
                }
            }
        }
        return false;
    }


    //@Override
    public Handle LexerAltCommands(Handle alt, Handle cmds)
    {
        throw new UnsupportedOperationException("This element is not allowed in parsers.");
    }


    //@Override
    public Handle LexerCallCommand(GrammarAST ID, GrammarAST arg)
    {
        throw new UnsupportedOperationException("This element is not allowed in parsers.");
    }


    //@Override
    public Handle LexerCommand(GrammarAST ID)
    {
        throw new UnsupportedOperationException("This element is not allowed in parsers.");
    }
}
