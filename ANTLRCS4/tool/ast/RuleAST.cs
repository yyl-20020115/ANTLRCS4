/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.parse;
using org.antlr.v4.runtime;
using org.antlr.v4.runtime.tree;

namespace org.antlr.v4.tool.ast;

public class RuleAST : GrammarASTWithOptions {
	public RuleAST(RuleAST node) :base(node)
    {
		;
	}

	public RuleAST(Token t) :base(t){ ; }
    public RuleAST(int type) :base(type){ ; }

	public bool isLexerRule() {
		String name = getRuleName();
		return name!=null && Grammar.isTokenName(name);
	}

	public String getRuleName() {
		GrammarAST nameNode = (GrammarAST)getChild(0);
		if ( nameNode!=null ) return nameNode.getText();
		return null;
	}

	//@Override
	public override RuleAST dupNode() { return new RuleAST(this); }

	public ActionAST getLexerAction() {
		Tree blk = getFirstChildWithType(ANTLRParser.BLOCK);
		if ( blk.getChildCount()==1 ) {
			Tree onlyAlt = blk.getChild(0);
			Tree lastChild = onlyAlt.getChild(onlyAlt.getChildCount()-1);
			if ( lastChild.getType()==ANTLRParser.ACTION ) {
				return (ActionAST)lastChild;
			}
		}
		return null;
	}

	//@Override
	public Object visit(GrammarASTVisitor v) { return v.visit(this); }
}
