/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.tool.ast;

namespace org.antlr.v4.codegen.model;

public class PlusBlock : Loop {
    //@ModelElement 
    public ThrowNoViableAlt error;

	public PlusBlock(OutputModelFactory factory,
					 GrammarAST plusRoot,
					 List<CodeBlockForAlt> alts)
	{
		super(factory, plusRoot, alts);
		BlockAST blkAST = (BlockAST)plusRoot.getChild(0);
		PlusBlockStartState blkStart = (PlusBlockStartState)blkAST.atnState;
		PlusLoopbackState loop = blkStart.loopBackState;
		stateNumber = blkStart.loopBackState.stateNumber;
		blockStartStateNumber = blkStart.stateNumber;
		loopBackStateNumber = loop.stateNumber;
		this.error = getThrowNoViableAlt(factory, plusRoot, null);
		decision = loop.decision;
	}
}
