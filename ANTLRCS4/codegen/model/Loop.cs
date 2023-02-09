/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.tool.ast;

namespace org.antlr.v4.codegen.model;

public class Loop : Choice
{
    public int blockStartStateNumber;
    public int loopBackStateNumber;
    public readonly int exitAlt;

    [ModelElement]
    public List<SrcOp> iteration;

    public Loop(OutputModelFactory factory,
                GrammarAST blkOrEbnfRootAST,
                List<CodeBlockForAlt> alts)
        : base(factory, blkOrEbnfRootAST, alts)
    {
        var nongreedy = (blkOrEbnfRootAST is QuantifierAST ast) && !ast.IsGreedy;
        exitAlt = nongreedy ? 1 : alts.Count + 1;
    }

    public void AddIterationOp(SrcOp op)
    {
        iteration ??= new();
        iteration.Add(op);
    }
}
