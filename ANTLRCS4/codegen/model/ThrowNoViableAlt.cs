/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime.misc;
using org.antlr.v4.tool.ast;

namespace org.antlr.v4.codegen.model;


/** */
public class ThrowNoViableAlt : ThrowRecognitionException {
	public ThrowNoViableAlt(OutputModelFactory factory, GrammarAST blkOrEbnfRootAST,
							IntervalSet expecting)
        : base(factory, blkOrEbnfRootAST, expecting)
	{
	}
}
