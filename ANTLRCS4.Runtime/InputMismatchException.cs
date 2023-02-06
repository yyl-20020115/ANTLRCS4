/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */
namespace org.antlr.v4.runtime;

/** This signifies any kind of mismatched input exceptions such as
 *  when the current input does not match the expected token.
 */
public class InputMismatchException : RecognitionException {
	public InputMismatchException(Parser recognizer) : base(recognizer, recognizer.InputStream, recognizer.GetCtx())
    {
		this.setOffendingToken(recognizer.getCurrentToken());
	}

	public InputMismatchException(Parser recognizer, int state, ParserRuleContext ctx): base(recognizer, recognizer.InputStream, ctx)
    {
		;
		this.setOffendingState(state);
		this.setOffendingToken(recognizer.getCurrentToken());
	}
}
