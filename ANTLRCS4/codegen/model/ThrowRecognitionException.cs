/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime.misc;
using org.antlr.v4.tool.ast;

namespace org.antlr.v4.codegen.model;

/** */
public class ThrowRecognitionException : SrcOp {
	public int decision;
	public String grammarFile;
	public int grammarLine;
	public int grammarCharPosInLine;

	public ThrowRecognitionException(OutputModelFactory factory, GrammarAST ast, IntervalSet expecting): base(factory, ast)
    {
		//this.decision = ((BlockStartState)ast.ATNState).decision;
		grammarLine = ast.getLine();
		grammarLine = ast.getCharPositionInLine();
		grammarFile = factory.getGrammar().fileName;
		//this.expecting = factory.createExpectingBitSet(ast, decision, expecting, "error");
//		factory.defineBitSet(this.expecting);
	}
}
