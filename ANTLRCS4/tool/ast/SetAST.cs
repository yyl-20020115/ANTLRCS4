/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime;

namespace org.antlr.v4.tool.ast;

public class SetAST : GrammarAST, RuleElementAST
{

    public SetAST(SetAST node) : base(node) { }

    public SetAST(int type, Token t, String text) : base(type, t, text) { }

    public override SetAST DupNode() => new (this);

    public override object Visit(GrammarASTVisitor v) => v.Visit(this);
}
