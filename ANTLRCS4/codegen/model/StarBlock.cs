/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime.atn;
using org.antlr.v4.tool.ast;

namespace org.antlr.v4.codegen.model;

public class StarBlock : Loop {
	public String loopLabel;

	public StarBlock(OutputModelFactory factory,
					 GrammarAST blkOrEbnfRootAST,
					 List<CodeBlockForAlt> alts)
		:base(factory, blkOrEbnfRootAST, alts)
    {
		;
		loopLabel = factory.getGenerator().getTarget().getLoopLabel(blkOrEbnfRootAST);
		StarLoopEntryState star = (StarLoopEntryState)blkOrEbnfRootAST.atnState;
		loopBackStateNumber = star.loopBackState.stateNumber;
		decision = star.decision;
	}
}
