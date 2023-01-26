/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

namespace org.antlr.v4.tool.ast;



public class ActionAST : GrammarASTWithOptions , RuleElementAST {
    // Alt, rule, grammar space
	GrammarAST scope = null;
	public AttributeResolver resolver;
	public List<Token> chunks; // useful for ANTLR IDE developers

	public ActionAST(ActionAST node) :base(node){
		this.resolver = node.resolver;
		this.chunks = node.chunks;
	}

	public ActionAST(Token t) { super(t); }
    public ActionAST(int type) { super(type); }
    public ActionAST(int type, Token t) { super(type, t); }

	//Override
	public ActionAST dupNode() { return new ActionAST(this); }

	//Override
	public Object visit(GrammarASTVisitor v) { return v.visit(this); }

	public void setScope(GrammarAST scope) {
		this.scope = scope;
	}

	public GrammarAST getScope() {
		return scope;
	}


}
