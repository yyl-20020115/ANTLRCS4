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
public class LL1OptionalBlockSingleAlt : LL1Choice {
	[ModelElement] 
		public SrcOp expr;
	[ModelElement] 
		public List<SrcOp> followExpr; // might not work in template if size>1

	public LL1OptionalBlockSingleAlt(OutputModelFactory factory,
									 GrammarAST blkAST,
									 List<CodeBlockForAlt> alts)
		: base(factory, blkAST, alts)
    {
		this.decision = ((DecisionState)blkAST.atnState).decision;

		/** Lookahead for each alt 1..n */
//		IntervalSet[] altLookSets = LinearApproximator.getLL1LookaheadSets(dfa);
		IntervalSet[] altLookSets = factory.getGrammar().decisionLOOK[(decision)];
		altLook = getAltLookaheadAsStringLists(altLookSets);
		IntervalSet look = altLookSets[0];
		IntervalSet followLook = altLookSets[1];

		IntervalSet expecting = look.or(followLook);
		this.error = getThrowNoViableAlt(factory, blkAST, expecting);

		expr = addCodeForLookaheadTempVar(look);
		followExpr = factory.getLL1Test(followLook, blkAST);
	}
}
