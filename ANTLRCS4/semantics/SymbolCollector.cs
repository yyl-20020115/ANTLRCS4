/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.parse;
using org.antlr.v4.tool;
using org.antlr.v4.tool.ast;

namespace org.antlr.v4.semantics;

/** Collects (create) rules, terminals, strings, actions, scopes etc... from AST
 *  side-effects: sets resolver field of asts for actions and
 *  defines predicates via definePredicateInAlt(), collects actions and stores
 *  in alts.
 *  TODO: remove side-effects!
 */
public class SymbolCollector : GrammarTreeVisitor
{
    /** which grammar are we checking */
    public readonly Grammar g;

    // stuff to collect
    public List<GrammarAST> rulerefs = new();
    public List<GrammarAST> qualifiedRulerefs = new();
    public List<GrammarAST> terminals = new();
    public List<GrammarAST> tokenIDRefs = new();
    public HashSet<string> strings = new();
    public List<GrammarAST> tokensDefs = new();
    public List<GrammarAST> channelDefs = new();

    /** Track action name node in @parser::members {...} or @members {...} */
    public List<GrammarAST> namedActions = new();

    public ErrorManager errMgr;

    // context
    public Rule currentRule;

    public SymbolCollector(Grammar g)
    {
        this.g = g;
        this.errMgr = g.Tools.ErrMgr;
    }

    public override ErrorManager ErrorManager => errMgr;
    public void Process(GrammarAST ast) { VisitGrammar(ast); }

    public override void GlobalNamedAction(GrammarAST scope, GrammarAST ID, ActionAST action)
    {
        action.Scope = scope;
        namedActions.Add((GrammarAST)ID.getParent());
        action.resolver = g;
    }

    public override void DefineToken(GrammarAST ID)
    {
        terminals.Add(ID);
        tokenIDRefs.Add(ID);
        tokensDefs.Add(ID);
    }

    public override void DefineChannel(GrammarAST ID)
    {
        channelDefs.Add(ID);
    }

    public override void DiscoverRule(RuleAST rule, GrammarAST ID,
                             List<GrammarAST> modifiers, ActionAST arg,
                             ActionAST returns, GrammarAST thrws,
                             GrammarAST options, ActionAST locals,
                             List<GrammarAST> actions,
                             GrammarAST block)
    {
        currentRule = g.getRule(ID.getText());
    }

    public override void DiscoverLexerRule(RuleAST rule, GrammarAST ID, List<GrammarAST> modifiers, GrammarAST options,
                                  GrammarAST block)
    {
        currentRule = g.getRule(ID.getText());
    }

    //@Override
    public void discoverOuterAlt(AltAST alt)
    {
        currentRule.alt[currentOuterAltNumber].ast = alt;
    }

    //@Override
    public void actionInAlt(ActionAST action)
    {
        currentRule.DefineActionInAlt(currentOuterAltNumber, action);
        action.resolver = currentRule.alt[currentOuterAltNumber];
    }

    //@Override
    public void sempredInAlt(PredAST pred)
    {
        currentRule.DefinePredicateInAlt(currentOuterAltNumber, pred);
        pred.resolver = currentRule.alt[currentOuterAltNumber];
    }

    //@Override
    public void ruleCatch(GrammarAST arg, ActionAST action)
    {
        GrammarAST catchme = (GrammarAST)action.getParent();
        currentRule.exceptions.Add(catchme);
        action.resolver = currentRule;
    }

    //@Override
    public void finallyAction(ActionAST action)
    {
        currentRule.finallyAction = action;
        action.resolver = currentRule;
    }

    //@Override
    public void label(GrammarAST op, GrammarAST ID, GrammarAST element)
    {
        LabelElementPair lp = new LabelElementPair(g, ID, element, op.getType());
        currentRule.alt[currentOuterAltNumber].labelDefs.Map(ID.getText(), lp);
    }

    //@Override
    public void stringRef(TerminalAST @ref)
    {
        terminals.Add(@ref);
        strings.Add(@ref.getText());
        if (currentRule != null)
        {
            currentRule.alt[currentOuterAltNumber].tokenRefs.Map(@ref.getText(), @ref);
        }
    }

    //@Override
    public void tokenRef(TerminalAST @ref)
    {
        terminals.Add(@ref);
        tokenIDRefs.Add(@ref);
        if (currentRule != null)
        {
            currentRule.alt[currentOuterAltNumber].tokenRefs.Map(@ref.getText(), @ref);
        }
    }

    //@Override
    public void ruleRef(GrammarAST @ref, ActionAST arg)
    {
        //		if ( inContext("DOT ...") ) qualifiedRulerefs.add((GrammarAST)@ref.getParent());
        rulerefs.Add(@ref);
        if (currentRule != null)
        {
            currentRule.alt[currentOuterAltNumber].ruleRefs.Map(@ref.getText(), @ref);
        }
    }

    //@Override
    public void grammarOption(GrammarAST ID, GrammarAST valueAST)
    {
        setActionResolver(valueAST);
    }

    //@Override
    public void ruleOption(GrammarAST ID, GrammarAST valueAST)
    {
        setActionResolver(valueAST);
    }

    //@Override
    public void blockOption(GrammarAST ID, GrammarAST valueAST)
    {
        setActionResolver(valueAST);
    }

    //@Override
    public void elementOption(GrammarASTWithOptions t, GrammarAST ID, GrammarAST valueAST)
    {
        setActionResolver(valueAST);
    }

    /** In case of option id={...}, set resolve in case they use $foo */
    private void setActionResolver(GrammarAST valueAST)
    {
        if (valueAST is ActionAST)
        {
            ((ActionAST)valueAST).resolver = currentRule.alt[currentOuterAltNumber];
        }
    }
}
