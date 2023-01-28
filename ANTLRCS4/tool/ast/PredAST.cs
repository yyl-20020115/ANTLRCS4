/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime;

namespace org.antlr.v4.tool.ast;

public class PredAST : ActionAST {
	public PredAST(PredAST node) : base(node)
    {
	}

	public PredAST(Token t): base(t) { }
    public PredAST(int type): base(type) { ; }
    public PredAST(int type, Token t): base(type,t) {  }

	//@Override
	public override PredAST dupNode() { return new PredAST(this); }

	//@Override
	public Object visit(GrammarASTVisitor v) { return v.visit(this); }
}
