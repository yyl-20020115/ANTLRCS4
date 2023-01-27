/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime;

namespace org.antlr.v4.tool.ast;

public class RuleRefAST : GrammarASTWithOptions , RuleElementAST {
	public RuleRefAST(RuleRefAST node):base(node) {
	}

	public RuleRefAST(Token t):base(t) {  }
    public RuleRefAST(int type):base(type) {  }
    public RuleRefAST(int type, Token t):base(type,t) {  }

	/** Dup token too since we overwrite during LR rule transform */
	//@Override
	public RuleRefAST dupNode() {
		RuleRefAST r = new RuleRefAST(this);
		// In LR transform, we alter original token stream to make e -> e[n]
		// Since we will be altering the dup, we need dup to have the
		// original token.  We can set this tree (the original) to have
		// a new token.
		r.token = this.token;
		this.token = new CommonToken(r.token);
		return r;
	}

	//@Override
	public Object visit(GrammarASTVisitor v) { return v.visit(this); }
}
