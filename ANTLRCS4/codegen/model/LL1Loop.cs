/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime.misc;
using org.antlr.v4.tool.ast;

namespace org.antlr.v4.codegen.model;

/** */
public abstract class LL1Loop : Choice {
	/** The state associated wih the (A|B|...) block not loopback, which
	 *  is base.stateNumber
	 */
	public int blockStartStateNumber;
	public int loopBackStateNumber;

	[ModelElement] 
	public OutputModelObject loopExpr;
	[ModelElement]
	public List<SrcOp> iteration;

	public LL1Loop(OutputModelFactory factory,
				   GrammarAST blkAST,
				   List<CodeBlockForAlt> alts)
		: base(factory, blkAST, alts)
    {
	}

	public void addIterationOp(SrcOp op) {
		if ( iteration==null ) iteration = new ();
		iteration.Add(op);
	}

	public SrcOp addCodeForLoopLookaheadTempVar(IntervalSet look) {
		TestSetInline expr = addCodeForLookaheadTempVar(look);
		if (expr != null) {
			CaptureNextTokenType nextType = new CaptureNextTokenType(factory, expr.varName);
			addIterationOp(nextType);
		}
		return expr;
	}
}
