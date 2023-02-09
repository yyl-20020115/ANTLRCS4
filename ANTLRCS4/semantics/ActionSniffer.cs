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



/** Find token and rule refs plus refs to them in actions;
 *  side-effect: update Alternatives
 */
public class ActionSniffer : BlankActionSplitterListener
{
    public Grammar g;
    public Rule r;          // null if action outside of rule
    public Alternative alt; // null if action outside of alt; could be in rule
    public ActionAST node;
    public Token actionToken; // token within action
    public ErrorManager errMgr;

    public ActionSniffer(Grammar g, Rule r, Alternative alt, ActionAST node, Token actionToken)
    {
        this.g = g;
        this.r = r;
        this.alt = alt;
        this.node = node;
        this.actionToken = actionToken;
        this.errMgr = g.Tools.ErrMgr;
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

    public void ProcessNested(Token actionToken)
    {
        var @in = new ANTLRStringStream(actionToken.Text);
        @in.SetLine(actionToken.Line);
        @in.SetCharPositionInLine(actionToken.CharPositionInLine);
        var splitter = new ActionSplitter(@in, this);
        // forces eval, triggers listener methods
        splitter.GetActionTokens();
    }


    public override void Attr(string expr, Token x) { TrackRef(x); }

    public override void QualifiedAttr(string expr, Token x, Token y) { TrackRef(x); }

    public override void SetAttr(string expr, Token x, Token rhs)
    {
        TrackRef(x);
        ProcessNested(rhs);
    }

    public override void SetNonLocalAttr(string expr, Token x, Token y, Token rhs)
    {
        ProcessNested(rhs);
    }

    public virtual void TrackRef(Token x)
    {
        var xRefs = alt.tokenRefs[x.Text];
        if (xRefs != null)
        {
            alt.tokenRefsInActions.Map(x.Text, node);
        }
        var rRefs = alt.ruleRefs[x.Text];
        if (rRefs != null)
        {
            alt.ruleRefsInActions.Map(x.Text, node);
        }
    }
}
