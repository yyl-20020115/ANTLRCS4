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

    public static void CheckAllAttributeExpressions(Grammar g)
    {
        foreach (var act in g.namedActions.Values)
        {
            var checker = new AttributeChecks(g, null, null, act, act.token);
            checker.ExamineAction();
        }

        foreach (var r in g.rules.Values)
        {
            foreach (var a in r.namedActions.Values)
            {
                var checker = new AttributeChecks(g, r, null, a, a.token);
                checker.ExamineAction();
            }
            for (int i = 1; i <= r.numberOfAlts; i++)
            {
                var alt = r.alt[i];
                foreach (var a in alt.actions)
                {
                    var checker =
                        new AttributeChecks(g, r, alt, a, a.token);
                    checker.ExamineAction();
                }
            }
            foreach (var e in r.exceptions)
            {
                var a = (ActionAST)e.GetChild(1);
                var checker = new AttributeChecks(g, r, null, a, a.token);
                checker.ExamineAction();
            }
            if (r.finallyAction != null)
            {
                var checker =
                    new AttributeChecks(g, r, null, r.finallyAction, r.finallyAction.token);
                checker.ExamineAction();
            }
        }
    }

    public void ExamineAction()
    {
        //Console.Out.WriteLine("examine "+actionToken);
        var @in = new ANTLRStringStream(actionToken.Text);
        @in.SetLine(actionToken.Line);
        @in.SetCharPositionInLine(actionToken.CharPositionInLine);
        var splitter = new ActionSplitter(@in, this);
        // forces eval, triggers listener methods
        node.chunks = splitter.GetActionTokens();
    }

    // LISTENER METHODS

    // $x.y
    public void QualifiedAttr(string expr, Token x, Token y)
    {
        if (g.IsLexer)
        {
            errMgr.GrammarError(ErrorType.ATTRIBUTE_IN_LEXER_ACTION,
                                g.fileName, x, x.Text + "." + y.Text, expr);
            return;
        }
        if (node.resolver.ResolveToAttribute(x.Text, node) != null)
        {
            // must be a member access to a predefined attribute like $ctx.foo
            Attr(expr, x);
            return;
        }

        if (node.resolver.ResolveToAttribute(x.Text, y.Text, node) == null)
        {
            var rref = IsolatedRuleRef(x.Text);
            if (rref != null)
            {
                if (rref.args != null && rref.args.Get(y.Text) != null)
                {
                    g.Tools.ErrMgr.GrammarError(ErrorType.INVALID_RULE_PARAMETER_REF,
                                              g.fileName, y, y.Text, rref.name, expr);
                }
                else
                {
                    errMgr.GrammarError(ErrorType.UNKNOWN_RULE_ATTRIBUTE,
                                              g.fileName, y, y.Text, rref.name, expr);
                }
            }
            else if (!node.resolver.ResolvesToAttributeDict(x.Text, node))
            {
                errMgr.GrammarError(ErrorType.UNKNOWN_SIMPLE_ATTRIBUTE,
                                          g.fileName, x, x.Text, expr);
            }
            else
            {
                errMgr.GrammarError(ErrorType.UNKNOWN_ATTRIBUTE_IN_SCOPE,
                                          g.fileName, y, y.Text, expr);
            }
        }
    }

    public void SetAttr(string expr, Token x, Token rhs)
    {
        if (g.IsLexer)
        {
            errMgr.GrammarError(ErrorType.ATTRIBUTE_IN_LEXER_ACTION,
                                g.fileName, x, x.Text, expr);
            return;
        }
        if (node.resolver.ResolveToAttribute(x.Text, node) == null)
        {
            ErrorType errorType = ErrorType.UNKNOWN_SIMPLE_ATTRIBUTE;
            if (node.resolver.ResolvesToListLabel(x.Text, node))
            {
                // $ids for ids+=ID etc...
                errorType = ErrorType.ASSIGNMENT_TO_LIST_LABEL;
            }

            errMgr.GrammarError(errorType,
                                g.fileName, x, x.Text, expr);
        }
        new AttributeChecks(g, r, alt, node, rhs).ExamineAction();
    }

    public void Attr(string expr, Token x)
    {
        if (g.IsLexer)
        {
            errMgr.GrammarError(ErrorType.ATTRIBUTE_IN_LEXER_ACTION,
                                g.fileName, x, x.Text, expr);
            return;
        }
        if (node.resolver.ResolveToAttribute(x.Text, node) == null)
        {
            if (node.resolver.ResolvesToToken(x.Text, node))
            {
                return; // $ID for token ref or label of token
            }
            if (node.resolver.ResolvesToListLabel(x.Text, node))
            {
                return; // $ids for ids+=ID etc...
            }
            if (IsolatedRuleRef(x.Text) != null)
            {
                errMgr.GrammarError(ErrorType.ISOLATED_RULE_REF,
                                    g.fileName, x, x.Text, expr);
                return;
            }
            errMgr.GrammarError(ErrorType.UNKNOWN_SIMPLE_ATTRIBUTE,
                                g.fileName, x, x.Text, expr);
        }
    }

    public void NonLocalAttr(string expr, Token x, Token y)
    {
        var r = g.GetRule(x.Text);
        if (r == null)
        {
            errMgr.GrammarError(ErrorType.UNDEFINED_RULE_IN_NONLOCAL_REF,
                                g.fileName, x, x.Text, y.Text, expr);
        }
        else if (r.ResolveToAttribute(y.Text, null) == null)
        {
            errMgr.GrammarError(ErrorType.UNKNOWN_RULE_ATTRIBUTE,
                                g.fileName, y, y.Text, x.Text, expr);

        }
    }

    public void SetNonLocalAttr(string expr, Token x, Token y, Token rhs)
    {
        Rule r = g.GetRule(x.Text);
        if (r == null)
        {
            errMgr.GrammarError(ErrorType.UNDEFINED_RULE_IN_NONLOCAL_REF,
                                g.fileName, x, x.Text, y.Text, expr);
        }
        else if (r.ResolveToAttribute(y.Text, null) == null)
        {
            errMgr.GrammarError(ErrorType.UNKNOWN_RULE_ATTRIBUTE,
                                g.fileName, y, y.Text, x.Text, expr);

        }
    }

    public void Text(string text) { }

    public void TemplateInstance(string expr) { }
    public void IndirectTemplateInstance(string expr) { }
    public void SetExprAttribute(string expr) { }
    public void SetSTAttribute(string expr) { }
    public void TemplateExpr(string expr) { }

    // SUPPORT

    public Rule IsolatedRuleRef(string x)
    {
        if (node.resolver is Grammar) return null;

        if (x.Equals(r.name)) return r;
        List<LabelElementPair> labels = null;
        if (node.resolver is Rule)
        {
            labels = r.GetElementLabelDefs().TryGetValue(x, out var ret) ? ret : new List<LabelElementPair>();
        }
        else if (node.resolver is Alternative)
        {
            labels = ((Alternative)node.resolver).labelDefs.TryGetValue(x, out var ret) ? ret : new();
        }
        if (labels != null)
        {  // it's a label ref. is it a rule label?
            var anyLabelDef = labels[(0)];
            if (anyLabelDef.type == LabelType.RULE_LABEL)
            {
                return g.GetRule(anyLabelDef.element.Text);
            }
        }
        if (node.resolver is Alternative alternative)
        {
            if (alternative.ruleRefs.ContainsKey(x))
            {
                return g.GetRule(x);
            }
        }
        return null;
    }

}
