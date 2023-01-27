/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime;

namespace org.antlr.v4.tool.ast;


public class PlusBlockAST : GrammarAST , RuleElementAST, QuantifierAST {
	private readonly bool _greedy;

	public PlusBlockAST(PlusBlockAST node):base(node) {
		_greedy = node._greedy;
	}

	public PlusBlockAST(int type, Token t, Token nongreedy) :base(type,t){
		_greedy = nongreedy == null;
	}

	//@Override
	public bool isGreedy() {
		return _greedy;
	}
	
    //@Override
    public PlusBlockAST dupNode() { return new PlusBlockAST(this); }

	//@Override
	public Object visit(GrammarASTVisitor v) { return v.visit(this); }
}
