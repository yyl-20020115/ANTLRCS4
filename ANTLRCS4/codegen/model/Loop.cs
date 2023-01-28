/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.tool.ast;

namespace org.antlr.v4.codegen.model;

public class Loop : Choice {
	public int blockStartStateNumber;
	public int loopBackStateNumber;
	public readonly int exitAlt;

	//@ModelElement 
		public List<SrcOp> iteration;

	public Loop(OutputModelFactory factory,
				GrammarAST blkOrEbnfRootAST,
				List<CodeBlockForAlt> alts)
	{
		base(factory, blkOrEbnfRootAST, alts);
		bool nongreedy = (blkOrEbnfRootAST is QuantifierAST) && !((QuantifierAST)blkOrEbnfRootAST).isGreedy();
		exitAlt = nongreedy ? 1 : alts.size() + 1;
	}

	public void addIterationOp(SrcOp op) {
		if ( iteration==null ) iteration = new ();
		iteration.Add(op);
	}
}
