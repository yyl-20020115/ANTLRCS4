/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime.atn;
using org.antlr.v4.runtime.misc;
using org.antlr.v4.tool.ast;

namespace org.antlr.v4.codegen.model;

/** */
public class LL1PlusBlockSingleAlt : LL1Loop
{
    public LL1PlusBlockSingleAlt(OutputModelFactory factory, GrammarAST plusRoot, List<CodeBlockForAlt> alts) : base(factory, plusRoot, alts)
    {
        var blkAST = plusRoot.getChild(0) as BlockAST;
        var blkStart = blkAST.atnState as PlusBlockStartState;
        stateNumber = blkStart.loopBackState.stateNumber;
        blockStartStateNumber = blkStart.stateNumber;
        var plus = blkAST.atnState as PlusBlockStartState;
        this.decision = plus.loopBackState.decision;
        var altLookSets = factory.GetGrammar().decisionLOOK[(decision)];
        var loopBackLook = altLookSets[0];
        loopExpr = AddCodeForLoopLookaheadTempVar(loopBackLook);
    }
}
