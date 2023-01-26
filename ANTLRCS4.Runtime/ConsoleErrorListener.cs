/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */
using org.antlr.v4.runtime.atn;

namespace org.antlr.v4.runtime;

/**
 *
 * @author Sam Harwell
 */
public class ConsoleErrorListener : BaseErrorListener {
	/**
	 * Provides a default instance of {@link ConsoleErrorListener}.
	 */
	public static readonly ConsoleErrorListener INSTANCE = new ConsoleErrorListener();

	/**
	 * {@inheritDoc}
	 *
	 * <p>
	 * This implementation prints messages to {@link System#err} containing the
	 * values of {@code line}, {@code charPositionInLine}, and {@code msg} using
	 * the following format.</p>
	 *
	 * <pre>
	 * line <em>line</em>:<em>charPositionInLine</em> <em>msg</em>
	 * </pre>
	 */
	//@Override
	public override void syntaxError(Recognizer<Token, ATNSimulator> recognizer,
							Object offendingSymbol,
							int line,
							int charPositionInLine,
							String msg,
							RecognitionException e)
	{
		Console.Error.WriteLine("line " + line + ":" + charPositionInLine + " " + msg);
	}

}
