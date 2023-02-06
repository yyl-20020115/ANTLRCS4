/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */
using org.antlr.v4.runtime.atn;
using org.antlr.v4.runtime.dfa;
using org.antlr.v4.runtime.misc;

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
	public void SyntaxError(Recognizer recognizer,
							Object offendingSymbol,
							int line,
							int charPositionInLine,
							String msg,
							RecognitionException e)
	{
		foreach (ANTLRErrorListener listener in delegates) {
			listener.SyntaxError(recognizer, offendingSymbol, line, charPositionInLine, msg, e);
		}
	}

	//@Override
	public void ReportAmbiguity(Parser recognizer,
								DFA dfa,
								int startIndex,
								int stopIndex,
								bool exact,
								BitSet ambigAlts,
								ATNConfigSet configs)
	{
		foreach (ANTLRErrorListener listener in delegates) {
			listener.ReportAmbiguity(recognizer, dfa, startIndex, stopIndex, exact, ambigAlts, configs);
		}
	}

	//@Override
	public void ReportAttemptingFullContext(Parser recognizer,
											DFA dfa,
											int startIndex,
											int stopIndex,
											BitSet conflictingAlts,
											ATNConfigSet configs)
	{
		foreach (ANTLRErrorListener listener in delegates) {
			listener.ReportAttemptingFullContext(recognizer, dfa, startIndex, stopIndex, conflictingAlts, configs);
		}
	}

	//@Override
	public void ReportContextSensitivity(Parser recognizer,
										 DFA dfa,
										 int startIndex,
										 int stopIndex,
										 int prediction,
										 ATNConfigSet configs)
	{
		foreach (ANTLRErrorListener listener in delegates) {
			listener.ReportContextSensitivity(recognizer, dfa, startIndex, stopIndex, prediction, configs);
		}
	}
}
