/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.codegen.model.decl;
using org.antlr.v4.tool.ast;

namespace org.antlr.v4.codegen.model;

/** */
public class MatchToken : RuleElement, LabeledOp
{
    public readonly string name;
    public readonly string escapedName;
    public readonly int ttype;
    public readonly List<Decl> labels = new();

    public MatchToken(OutputModelFactory factory, TerminalAST ast) 
        : base(factory, ast)
    {
        var g = factory.Grammar;
        var gen = factory.Generator;
        ttype = g.GetTokenType(ast.Text);
        var target = gen.Target;
        name = target.GetTokenTypeAsTargetLabel(g, ttype);
        escapedName = target.EscapeIfNeeded(name);
    }

    public MatchToken(OutputModelFactory factory, GrammarAST ast) : base(factory, ast)
    {
        ttype = 0;
        name = null;
        escapedName = null;
    }

    public virtual List<Decl> Labels => labels;
}
