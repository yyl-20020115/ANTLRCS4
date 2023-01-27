/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */
using org.antlr.v4.runtime.atn;
using org.antlr.v4.runtime.dfa;

namespace org.antlr.v4.runtime;

/**
 * This implementation of {@link ANTLRErrorListener} dispatches all calls to a
 * collection of delegate listeners. This reduces the effort required to support multiple
 * listeners.
 *
 * @author Sam Harwell
 */
public class ProxyErrorListener : ANTLRErrorListener {
	private readonly ICollection<ANTLRErrorListener> delegates;

	public ProxyErrorListener(ICollection<ANTLRErrorListener> delegates) {
		if (delegates == null) {
			throw new NullReferenceException("delegates");
		}

		this.delegates = delegates;
	}

	//@Override
	public void syntaxError(Recognizer recognizer,
							Object offendingSymbol,
							int line,
							int charPositionInLine,
							String msg,
							RecognitionException e)
	{
		foreach (ANTLRErrorListener listener in delegates) {
			listener.syntaxError(recognizer, offendingSymbol, line, charPositionInLine, msg, e);
		}
	}

	//@Override
	public void reportAmbiguity(Parser recognizer,
								DFA dfa,
								int startIndex,
								int stopIndex,
								bool exact,
								BitSet ambigAlts,
								ATNConfigSet configs)
	{
		foreach (ANTLRErrorListener listener in delegates) {
			listener.reportAmbiguity(recognizer, dfa, startIndex, stopIndex, exact, ambigAlts, configs);
		}
	}

	//@Override
	public void reportAttemptingFullContext(Parser recognizer,
											DFA dfa,
											int startIndex,
											int stopIndex,
											BitSet conflictingAlts,
											ATNConfigSet configs)
	{
		foreach (ANTLRErrorListener listener in delegates) {
			listener.reportAttemptingFullContext(recognizer, dfa, startIndex, stopIndex, conflictingAlts, configs);
		}
	}

	//@Override
	public void reportContextSensitivity(Parser recognizer,
										 DFA dfa,
										 int startIndex,
										 int stopIndex,
										 int prediction,
										 ATNConfigSet configs)
	{
		foreach (ANTLRErrorListener listener in delegates) {
			listener.reportContextSensitivity(recognizer, dfa, startIndex, stopIndex, prediction, configs);
		}
	}
}
