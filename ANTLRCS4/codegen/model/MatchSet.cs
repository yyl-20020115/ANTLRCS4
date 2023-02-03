/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.codegen.model.decl;
using org.antlr.v4.runtime.atn;
using org.antlr.v4.tool.ast;

namespace org.antlr.v4.codegen.model;

public class MatchSet : MatchToken
{
    [ModelElement]
    public TestSetInline expr;
    [ModelElement]
    public CaptureNextTokenType capture;

    public MatchSet(OutputModelFactory factory, GrammarAST ast)
        : base(factory, ast)
    {
        var st = ast.atnState.transition(0) as SetTransition;
        int wordSize = factory.GetGenerator().Target.GetInlineTestSetWordSize();
        expr = new TestSetInline(factory, null, st.set, wordSize);
        var d = new TokenTypeDecl(factory, expr.varName);
        factory.GetCurrentRuleFunction().AddLocalDecl(d);
        capture = new CaptureNextTokenType(factory, expr.varName);
    }
}
