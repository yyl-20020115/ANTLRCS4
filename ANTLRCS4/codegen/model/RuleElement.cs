/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.tool.ast;

namespace org.antlr.v4.codegen.model;

public class RuleElement : SrcOp
{
    /** Associated ATN state for this rule elements (action, token, ruleref, ...) */
    public int stateNumber;

    public RuleElement(OutputModelFactory factory, GrammarAST ast) : base(factory, ast)
    {
        if (ast != null && ast.atnState != null) stateNumber = ast.atnState.stateNumber;
    }
}
