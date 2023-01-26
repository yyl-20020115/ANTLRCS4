/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */
using org.antlr.v4.runtime;
using org.antlr.v4.runtime.atn;
using org.antlr.v4.runtime.dfa;

namespace org.antlr.v4.runtime;

/**
 * Provides an empty default implementation of {@link ANTLRErrorListener}. The
 * default implementation of each method does nothing, but can be overridden as
 * necessary.
 *
 * @author Sam Harwell
 */
public class BaseErrorListener : ANTLRErrorListener {
	public virtual void syntaxError<T1, T2>(Recognizer<T1, T2> recognizer,
							Object offendingSymbol,
							int line,
							int charPositionInLine,
							String msg,
							RecognitionException<T1,T2> e)
	{
	}

	public virtual void reportAmbiguity(Parser recognizer,
								DFA dfa,
								int startIndex,
								int stopIndex,
								bool exact,
								BitSet ambigAlts,
								ATNConfigSet configs)
	{
	}

	
	public virtual void reportAttemptingFullContext(Parser recognizer,
											DFA dfa,
											int startIndex,
											int stopIndex,
											BitSet conflictingAlts,
											ATNConfigSet configs)
	{
	}

	
	public virtual void reportContextSensitivity(Parser recognizer,
										 DFA dfa,
										 int startIndex,
										 int stopIndex,
										 int prediction,
										 ATNConfigSet configs)
	{
	}
}
