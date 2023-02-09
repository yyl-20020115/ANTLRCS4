/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime;

namespace org.antlr.v4.tool.ast;


public class StarBlockAST : GrammarAST, RuleElementAST, QuantifierAST
{
    private readonly bool greedy;

    public StarBlockAST(StarBlockAST node) : base(node) => greedy = node.greedy;

    public StarBlockAST(int type, Token t, Token nongreedy) : base(type, t) => greedy = nongreedy == null;

    public bool IsGreedy => greedy;

    public override StarBlockAST DupNode() => new (this);

    public override object Visit(GrammarASTVisitor v) => v.Visit(this);
}
