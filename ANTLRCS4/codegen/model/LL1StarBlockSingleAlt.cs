/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime.atn;
using org.antlr.v4.tool.ast;

namespace org.antlr.v4.codegen.model;

/** */
public class LL1StarBlockSingleAlt : LL1Loop {
	public LL1StarBlockSingleAlt(OutputModelFactory factory, GrammarAST starRoot, List<CodeBlockForAlt> alts)
	: base(factory, starRoot, alts)
    {

		var star = starRoot.atnState as StarLoopEntryState;
		loopBackStateNumber = star.loopBackState.stateNumber;
		this.decision = star.decision;
		var altLookSets = factory.GetGrammar().decisionLOOK[(decision)];
		//assert altLookSets.length == 2;
		var enterLook = altLookSets[0];
		var exitLook = altLookSets[1];
		loopExpr = AddCodeForLoopLookaheadTempVar(enterLook);
	}
}
