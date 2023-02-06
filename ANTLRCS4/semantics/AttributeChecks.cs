/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.runtime;
using org.antlr.v4.parse;
using org.antlr.v4.runtime;
using org.antlr.v4.tool;
using org.antlr.v4.tool.ast;

namespace org.antlr.v4.semantics;



/** Trigger checks for various kinds of attribute expressions.
 *  no side-effects.
 */
public class AttributeChecks : ActionSplitterListener
{
    public Grammar g;
    public Rule r;          // null if action outside of rule
    public Alternative alt; // null if action outside of alt; could be in rule
    public ActionAST node;
    public Token actionToken; // token within action
    public ErrorManager errMgr;

    public AttributeChecks(Grammar g, Rule r, Alternative alt, ActionAST node, Token actionToken)
    {
        this.g = g;
        this.r = r;
        this.alt = alt;
        this.node = node;
        this.actionToken = actionToken;
        this.errMgr = g.Tools.ErrMgr;
    }

    public static void checkAllAttributeExpressions(Grammar g)
    {
        foreach (ActionAST act in g.namedActions.Values)
        {
            AttributeChecks checker = new AttributeChecks(g, null, null, act, act.token);
            checker.examineAction();
        }

        foreach (Rule r in g.rules.Values)
        {
            foreach (ActionAST a in r.namedActions.Values)
            {
                AttributeChecks checker = new AttributeChecks(g, r, null, a, a.token);
                checker.examineAction();
            }
            for (int i = 1; i <= r.numberOfAlts; i++)
            {
                Alternative alt = r.alt[i];
                foreach (ActionAST a in alt.actions)
                {
                    AttributeChecks checker =
                        new AttributeChecks(g, r, alt, a, a.token);
                    checker.examineAction();
                }
            }
            foreach (GrammarAST e in r.exceptions)
            {
                ActionAST a = (ActionAST)e.getChild(1);
                AttributeChecks checker = new AttributeChecks(g, r, null, a, a.token);
                checker.examineAction();
            }
            if (r.finallyAction != null)
            {
                AttributeChecks checker =
                    new AttributeChecks(g, r, null, r.finallyAction, r.finallyAction.token);
                checker.examineAction();
            }
        }
    }

    public void examineAction()
    {
        //Console.Out.WriteLine("examine "+actionToken);
        ANTLRStringStream @in = new ANTLRStringStream(actionToken.getText());
        @in.setLine(actionToken.getLine());
        @in.setCharPositionInLine(actionToken.getCharPositionInLine());
        ActionSplitter splitter = new ActionSplitter(@in, this);
        // forces eval, triggers listener methods
        node.chunks = splitter.GetActionTokens();
    }

    // LISTENER METHODS

    // $x.y
    //@Override
    public void QualifiedAttr(String expr, Token x, Token y)
    {
        if (g.isLexer())
        {
            errMgr.GrammarError(ErrorType.ATTRIBUTE_IN_LEXER_ACTION,
                                g.fileName, x, x.getText() + "." + y.getText(), expr);
            return;
        }
        if (node.resolver.resolveToAttribute(x.getText(), node) != null)
        {
            // must be a member access to a predefined attribute like $ctx.foo
            Attr(expr, x);
            return;
        }

        if (node.resolver.resolveToAttribute(x.getText(), y.getText(), node) == null)
        {
            Rule rref = isolatedRuleRef(x.getText());
            if (rref != null)
            {
                if (rref.args != null && rref.args.get(y.getText()) != null)
                {
                    g.Tools.ErrMgr.GrammarError(ErrorType.INVALID_RULE_PARAMETER_REF,
                                              g.fileName, y, y.getText(), rref.name, expr);
                }
                else
                {
                    errMgr.GrammarError(ErrorType.UNKNOWN_RULE_ATTRIBUTE,
                                              g.fileName, y, y.getText(), rref.name, expr);
                }
            }
            else if (!node.resolver.resolvesToAttributeDict(x.getText(), node))
            {
                errMgr.GrammarError(ErrorType.UNKNOWN_SIMPLE_ATTRIBUTE,
                                          g.fileName, x, x.getText(), expr);
            }
            else
            {
                errMgr.GrammarError(ErrorType.UNKNOWN_ATTRIBUTE_IN_SCOPE,
                                          g.fileName, y, y.getText(), expr);
            }
        }
    }

    //@Override
    public void SetAttr(String expr, Token x, Token rhs)
    {
        if (g.isLexer())
        {
            errMgr.GrammarError(ErrorType.ATTRIBUTE_IN_LEXER_ACTION,
                                g.fileName, x, x.getText(), expr);
            return;
        }
        if (node.resolver.resolveToAttribute(x.getText(), node) == null)
        {
            ErrorType errorType = ErrorType.UNKNOWN_SIMPLE_ATTRIBUTE;
            if (node.resolver.resolvesToListLabel(x.getText(), node))
            {
                // $ids for ids+=ID etc...
                errorType = ErrorType.ASSIGNMENT_TO_LIST_LABEL;
            }

            errMgr.GrammarError(errorType,
                                g.fileName, x, x.getText(), expr);
        }
        new AttributeChecks(g, r, alt, node, rhs).examineAction();
    }

    //@Override
    public void Attr(String expr, Token x)
    {
        if (g.isLexer())
        {
            errMgr.GrammarError(ErrorType.ATTRIBUTE_IN_LEXER_ACTION,
                                g.fileName, x, x.getText(), expr);
            return;
        }
        if (node.resolver.resolveToAttribute(x.getText(), node) == null)
        {
            if (node.resolver.resolvesToToken(x.getText(), node))
            {
                return; // $ID for token ref or label of token
            }
            if (node.resolver.resolvesToListLabel(x.getText(), node))
            {
                return; // $ids for ids+=ID etc...
            }
            if (isolatedRuleRef(x.getText()) != null)
            {
                errMgr.GrammarError(ErrorType.ISOLATED_RULE_REF,
                                    g.fileName, x, x.getText(), expr);
                return;
            }
            errMgr.GrammarError(ErrorType.UNKNOWN_SIMPLE_ATTRIBUTE,
                                g.fileName, x, x.getText(), expr);
        }
    }

    //@Override
    public void NonLocalAttr(String expr, Token x, Token y)
    {
        Rule r = g.getRule(x.getText());
        if (r == null)
        {
            errMgr.GrammarError(ErrorType.UNDEFINED_RULE_IN_NONLOCAL_REF,
                                g.fileName, x, x.getText(), y.getText(), expr);
        }
        else if (r.resolveToAttribute(y.getText(), null) == null)
        {
            errMgr.GrammarError(ErrorType.UNKNOWN_RULE_ATTRIBUTE,
                                g.fileName, y, y.getText(), x.getText(), expr);

        }
    }

    //@Override
    public void SetNonLocalAttr(String expr, Token x, Token y, Token rhs)
    {
        Rule r = g.getRule(x.getText());
        if (r == null)
        {
            errMgr.GrammarError(ErrorType.UNDEFINED_RULE_IN_NONLOCAL_REF,
                                g.fileName, x, x.getText(), y.getText(), expr);
        }
        else if (r.resolveToAttribute(y.getText(), null) == null)
        {
            errMgr.GrammarError(ErrorType.UNKNOWN_RULE_ATTRIBUTE,
                                g.fileName, y, y.getText(), x.getText(), expr);

        }
    }

    //@Override
    public void Text(String text) { }

    // don't care
    public void templateInstance(String expr) { }
    public void indirectTemplateInstance(String expr) { }
    public void setExprAttribute(String expr) { }
    public void setSTAttribute(String expr) { }
    public void templateExpr(String expr) { }

    // SUPPORT

    public Rule isolatedRuleRef(String x)
    {
        if (node.resolver is Grammar) return null;

        if (x.Equals(r.name)) return r;
        List<LabelElementPair> labels = null;
        if (node.resolver is Rule)
        {
            labels = r.getElementLabelDefs().TryGetValue(x, out var ret) ? ret : new List<LabelElementPair>();
        }
        else if (node.resolver is Alternative)
        {
            labels = ((Alternative)node.resolver).labelDefs.TryGetValue(x, out var ret) ? ret : new();
        }
        if (labels != null)
        {  // it's a label ref. is it a rule label?
            LabelElementPair anyLabelDef = labels[(0)];
            if (anyLabelDef.type == LabelType.RULE_LABEL)
            {
                return g.getRule(anyLabelDef.element.getText());
            }
        }
        if (node.resolver is Alternative alternative)
        {
            if (alternative.ruleRefs.ContainsKey(x))
            {
                return g.getRule(x);
            }
        }
        return null;
    }

}
