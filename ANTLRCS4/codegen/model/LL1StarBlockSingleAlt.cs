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
public class LL1StarBlockSingleAlt : LL1Loop {
	public LL1StarBlockSingleAlt(OutputModelFactory factory, GrammarAST starRoot, List<CodeBlockForAlt> alts) {
		base(factory, starRoot, alts);

		StarLoopEntryState star = (StarLoopEntryState)starRoot.atnState;
		loopBackStateNumber = star.loopBackState.stateNumber;
		this.decision = star.decision;
		IntervalSet[] altLookSets = factory.getGrammar().decisionLOOK.get(decision);
		//assert altLookSets.length == 2;
		IntervalSet enterLook = altLookSets[0];
		IntervalSet exitLook = altLookSets[1];
		loopExpr = addCodeForLoopLookaheadTempVar(enterLook);
	}
}
