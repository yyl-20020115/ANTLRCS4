/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime.atn;
using org.antlr.v4.tool.ast;

namespace org.antlr.v4.codegen.model;

public class PlusBlock : Loop
{
    [ModelElement]
    public ThrowNoViableAlt error;

    public PlusBlock(OutputModelFactory factory,
                     GrammarAST plusRoot,
                     List<CodeBlockForAlt> alts)
        : base(factory, plusRoot, alts)
    {
        var blkAST = plusRoot.getChild(0) as BlockAST;
        var blkStart = blkAST.atnState as PlusBlockStartState;
        var loop = blkStart.loopBackState;
        stateNumber = blkStart.loopBackState.stateNumber;
        blockStartStateNumber = blkStart.stateNumber;
        loopBackStateNumber = loop.stateNumber;
        this.error = GetThrowNoViableAlt(factory, plusRoot, null);
        decision = loop.decision;
    }
}
