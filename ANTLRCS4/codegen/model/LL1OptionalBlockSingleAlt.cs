/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime.atn;
using org.antlr.v4.runtime.misc;
using org.antlr.v4.tool.ast;

namespace org.antlr.v4.codegen.model;

/** (A B C)? */
public class LL1OptionalBlockSingleAlt : LL1Choice
{
    [ModelElement]
    public SrcOp expr;
    [ModelElement]
    public List<SrcOp> followExpr; // might not work in template if size>1

    public LL1OptionalBlockSingleAlt(OutputModelFactory factory,
                                     GrammarAST blkAST,
                                     List<CodeBlockForAlt> alts)
        : base(factory, blkAST, alts)
    {
        this.decision = (blkAST.atnState as DecisionState).decision;

        /** Lookahead for each alt 1..n */
        //		IntervalSet[] altLookSets = LinearApproximator.getLL1LookaheadSets(dfa);
        var altLookSets = factory.GetGrammar().decisionLOOK[(decision)];
        altLook = GetAltLookaheadAsStringLists(altLookSets);
        var look = altLookSets[0];
        var followLook = altLookSets[1];
        var expecting = look.or(followLook);
        this.error = GetThrowNoViableAlt(factory, blkAST, expecting);

        expr = AddCodeForLookaheadTempVar(look);
        followExpr = factory.GetLL1Test(followLook, blkAST);
    }
}
