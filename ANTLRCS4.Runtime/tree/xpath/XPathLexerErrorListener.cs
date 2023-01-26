/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime.atn;

namespace org.antlr.v4.runtime.tree.xpath;


public class XPathLexerErrorListener : BaseErrorListener {
	public override void syntaxError(Recognizer<Token, ATNSimulator> recognizer, Object offendingSymbol,
							int line, int charPositionInLine, String msg,
							RecognitionException e)
	{
	}
}
