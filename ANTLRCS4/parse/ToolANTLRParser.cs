/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime;
using org.antlr.v4.tool;

namespace org.antlr.v4.parse;


/** Override error handling for use with ANTLR tool itself; leaves
 *  nothing in grammar associated with Tool so others can use in IDEs, ...
 */
public class ToolANTLRParser : ANTLRParser {
	public Tool tool;

	public ToolANTLRParser(TokenStream input, Tool tool):base(input) {
		this.tool = tool;
	}

	//@Override
	public void displayRecognitionError(String[] tokenNames,
										RecognitionException e)
	{
		String msg = getParserErrorMessage(this, e);
		if ( !paraphrases.isEmpty() ) {
			String paraphrase = paraphrases.peek();
			msg = msg+" while "+paraphrase;
		}
	//	List stack = getRuleInvocationStack(e, this.getClass().getName());
	//	msg += ", rule stack = "+stack;
		tool.ErrMgr.syntaxError(ErrorType.SYNTAX_ERROR, GetSourceName(), e.token, e, msg);
	}

	public String getParserErrorMessage(antlr.runtime.Parser parser, RecognitionException e) {
		String msg;
		if ( e is NoViableAltException) {
			String name = GetTokenErrorDisplay(e.token);
			msg = name+" came as a complete surprise to me";
		}
		else if ( e is v4ParserException) {
			msg = ((v4ParserException)e).msg;
		}
		else {
			msg = parser.GetErrorMessage(e, parser.GetTokenNames());
		}
		return msg;
	}

	//@Override
	public void grammarError(ErrorType etype, Token token, params Object[] args) {
		tool.ErrMgr.GrammarError(etype, GetSourceName(), token, args);
	}
}
