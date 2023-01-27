/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.tool;

namespace org.antlr.v4.parse;

public class ToolANTLRLexer : ANTLRLexer {
	public Tool tool;

	public ToolANTLRLexer(CharStream input, Tool tool):base(input) {
		this.tool = tool;
	}

	//@Override
	public void displayRecognitionError(String[] tokenNames, RecognitionException e) {
		String msg = getErrorMessage(e, tokenNames);
		tool.errMgr.syntaxError(ErrorType.SYNTAX_ERROR, getSourceName(), e.token, e, msg);
	}

	//@Override
	public void grammarError(ErrorType etype, Token token, params Object[] args) {
		tool.errMgr.grammarError(etype, getSourceName(), token, args);
	}
}
