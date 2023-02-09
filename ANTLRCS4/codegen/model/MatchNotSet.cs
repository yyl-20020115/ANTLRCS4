/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.tool.ast;

namespace org.antlr.v4.codegen.model;

public class MatchNotSet : MatchSet
{
    public string varName = "_la";
    public MatchNotSet(OutputModelFactory factory, GrammarAST ast) : base(factory, ast) { }
}
