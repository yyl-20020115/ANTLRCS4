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
    public void Process(GrammarAST ast) => VisitGrammar(ast);

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
        currentRule = g.GetRule(ID.getText());
    }

    public override void DiscoverLexerRule(RuleAST rule, GrammarAST ID, List<GrammarAST> modifiers, GrammarAST options,
                                  GrammarAST block)
    {
        currentRule = g.GetRule(ID.getText());
    }

    public override void DiscoverOuterAlt(AltAST alt)
    {
        currentRule.alt[currentOuterAltNumber].ast = alt;
    }

    public override void ActionInAlt(ActionAST action)
    {
        currentRule.DefineActionInAlt(currentOuterAltNumber, action);
        action.resolver = currentRule.alt[currentOuterAltNumber];
    }

    public override void SempredInAlt(PredAST pred)
    {
        currentRule.DefinePredicateInAlt(currentOuterAltNumber, pred);
        pred.resolver = currentRule.alt[currentOuterAltNumber];
    }

    public override void RuleCatch(GrammarAST arg, ActionAST action)
    {
        var catchme = (GrammarAST)action.getParent();
        currentRule.exceptions.Add(catchme);
        action.resolver = currentRule;
    }

    public override void FinallyAction(ActionAST action)
    {
        currentRule.finallyAction = action;
        action.resolver = currentRule;
    }

    public override void Label(GrammarAST op, GrammarAST ID, GrammarAST element)
    {
        var lp = new LabelElementPair(g, ID, element, op.getType());
        currentRule.alt[currentOuterAltNumber].labelDefs.Map(ID.getText(), lp);
    }

    public override void StringRef(TerminalAST @ref)
    {
        terminals.Add(@ref);
        strings.Add(@ref.getText());
        currentRule?.alt[currentOuterAltNumber].tokenRefs.Map(@ref.getText(), @ref);
    }

    public override void TokenRef(TerminalAST @ref)
    {
        terminals.Add(@ref);
        tokenIDRefs.Add(@ref);
        currentRule?.alt[currentOuterAltNumber].tokenRefs.Map(@ref.getText(), @ref);
    }

    public override void RuleRef(GrammarAST @ref, ActionAST arg)
    {
        //		if ( inContext("DOT ...") ) qualifiedRulerefs.add((GrammarAST)@ref.getParent());
        rulerefs.Add(@ref);
        currentRule?.alt[currentOuterAltNumber].ruleRefs.Map(@ref.getText(), @ref);
    }

    public override void GrammarOption(GrammarAST ID, GrammarAST valueAST)
    {
        SetActionResolver(valueAST);
    }

    public override void RuleOption(GrammarAST ID, GrammarAST valueAST)
    {
        SetActionResolver(valueAST);
    }

    public override void BlockOption(GrammarAST ID, GrammarAST valueAST)
    {
        SetActionResolver(valueAST);
    }

    public override void ElementOption(GrammarASTWithOptions t, GrammarAST ID, GrammarAST valueAST)
    {
        SetActionResolver(valueAST);
    }

    /** In case of option id={...}, set resolve in case they use $foo */
    private void SetActionResolver(GrammarAST valueAST)
    {
        if (valueAST is ActionAST aST)
        {
            aST.resolver = currentRule.alt[currentOuterAltNumber];
        }
    }
}
