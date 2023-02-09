/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime;

namespace org.antlr.v4.tool.ast;



public class ActionAST : GrammarASTWithOptions, RuleElementAST
{
    // Alt, rule, grammar space
    GrammarAST scope = null;
    public AttributeResolver resolver;
    public List<Token> chunks; // useful for ANTLR IDE developers

    public ActionAST(ActionAST node) : base(node)
    {
        this.resolver = node.resolver;
        this.chunks = node.chunks;
    }

    public ActionAST(Token t) : base(t) { }
    public ActionAST(int type) : base(type) { }
    public ActionAST(int type, Token t) : base(type, t) { }

    public override ActionAST dupNode() { return new ActionAST(this); }

    //@Override
    public object visit(GrammarASTVisitor v) { return v.visit(this); }

    public GrammarAST Scope { get => scope; set => this.scope = value; }
}
