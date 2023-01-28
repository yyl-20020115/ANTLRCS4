/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

namespace org.antlr.v4.codegen.model;



public class AltBlock : Choice {
//	@ModelElement public ThrowNoViableAlt error;

	public AltBlock(OutputModelFactory factory,
					GrammarAST blkOrEbnfRootAST,
					List<CodeBlockForAlt> alts)
	{
		base(factory, blkOrEbnfRootAST, alts);
		decision = ((BlockStartState)blkOrEbnfRootAST.atnState).decision;
		// interp.predict() throws exception
//		this.error = new ThrowNoViableAlt(factory, blkOrEbnfRootAST, null);
	}
}
