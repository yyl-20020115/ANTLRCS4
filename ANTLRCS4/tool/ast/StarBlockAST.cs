/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime;

namespace org.antlr.v4.tool.ast;


public class StarBlockAST : GrammarAST , RuleElementAST, QuantifierAST {
	private readonly bool _greedy;

	public StarBlockAST(StarBlockAST node) : base(node)
    {
		_greedy = node._greedy;
	}

	public StarBlockAST(int type, Token t, Token nongreedy): base(type, t)
    {
		_greedy = nongreedy == null;
	}

	//@Override
	public bool isGreedy() {
		return _greedy;
	}

    //@Override
    public StarBlockAST dupNode() { return new StarBlockAST(this); }

	//@Override
	public Object visit(GrammarASTVisitor v) { return v.visit(this); }
}
