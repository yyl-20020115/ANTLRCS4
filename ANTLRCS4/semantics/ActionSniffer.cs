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
public class ActionSniffer : BlankActionSplitterListener {
	public Grammar g;
	public Rule r;          // null if action outside of rule
	public Alternative alt; // null if action outside of alt; could be in rule
	public ActionAST node;
	public Token actionToken; // token within action
	public ErrorManager errMgr;

	public ActionSniffer(Grammar g, Rule r, Alternative alt, ActionAST node, Token actionToken) {
		this.g = g;
		this.r = r;
		this.alt = alt;
		this.node = node;
		this.actionToken = actionToken;
		this.errMgr = g.Tools.ErrMgr;
	}

	public void examineAction() {
		//Console.Out.WriteLine("examine "+actionToken);
		ANTLRStringStream @in = new ANTLRStringStream(actionToken.getText());
        @in.setLine(actionToken.getLine());
        @in.setCharPositionInLine(actionToken.getCharPositionInLine());
		ActionSplitter splitter = new ActionSplitter(@in, this);
		// forces eval, triggers listener methods
		node.chunks = splitter.getActionTokens();
	}

	public void processNested(Token actionToken) {
		ANTLRStringStream @in = new ANTLRStringStream(actionToken.getText());
        @in.setLine(actionToken.getLine());
        @in.setCharPositionInLine(actionToken.getCharPositionInLine());
		ActionSplitter splitter = new ActionSplitter(@in, this);
		// forces eval, triggers listener methods
		splitter.getActionTokens();
	}


	//@Override
	public void attr(String expr, Token x) { trackRef(x); }

	//@Override
	public void qualifiedAttr(String expr, Token x, Token y) { trackRef(x); }

	//@Override
	public void setAttr(String expr, Token x, Token rhs) {
		trackRef(x);
		processNested(rhs);
	}

	//@Override
	public void setNonLocalAttr(String expr, Token x, Token y, Token rhs) {
		processNested(rhs);
	}

	public void trackRef(Token x) {
		List<TerminalAST> xRefs = alt.tokenRefs[x.getText()];
		if ( xRefs!=null ) {
			alt.tokenRefsInActions.map(x.getText(), node);
		}
		List<GrammarAST> rRefs = alt.ruleRefs[x.getText()];
		if ( rRefs!=null ) {
			alt.ruleRefsInActions.map(x.getText(), node);
		}
	}
}
