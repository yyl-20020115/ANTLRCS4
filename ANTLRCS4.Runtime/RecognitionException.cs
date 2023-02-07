/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */
using org.antlr.v4.runtime.misc;
using org.antlr.v4.runtime.tree.pattern;

namespace org.antlr.v4.runtime;

/** The root of the ANTLR exception hierarchy. In general, ANTLR tracks just
 *  3 kinds of errors: prediction errors, failed predicate errors, and
 *  mismatched input errors. In each case, the parser knows where it is
 *  in the input, where it is in the ATN, the rule invocation stack,
 *  and what kind of problem occurred.
 */
public class RecognitionException : RuntimeException {
	/** The {@link Recognizer} where this exception originated. */
	private readonly Recognizer recognizer;

	private readonly RuleContext ctx;

	private readonly IntStream input;

	public readonly int c;
	/**
	 * The current {@link Token} when an error occurred. Since not all streams
	 * support accessing symbols by index, we have to track the {@link Token}
	 * instance itself.
	 */
	private Token offendingToken;

	private int offendingState = -1;
    internal bool approximateLineInfo;
    internal string line;
    internal string charPositionInLine;
    public Token token;
    internal object node;

    public RecognitionException(Recognizer recognizer,
								IntStream input,
								ParserRuleContext ctx)
	{
        this.c = 0;
        this.recognizer = recognizer;
		this.input = input;
		this.ctx = ctx;
		if ( recognizer!=null ) this.offendingState = recognizer.GetState();
	}
	public RecognitionException(String message,
								Recognizer recognizer,
								IntStream input,
								ParserRuleContext ctx)
		: base(message) 
	{
		this.c = 0;
		this.recognizer = recognizer;
		this.input = input;
		this.ctx = ctx;
		if ( recognizer!=null ) this.offendingState = recognizer.GetState();
	}

	/**
	 * Get the ATN state number the parser was in at the time the error
	 * occurred. For {@link NoViableAltException} and
	 * {@link LexerNoViableAltException} exceptions, this is the
	 * {@link DecisionState} number. For others, it is the state whose outgoing
	 * edge we couldn't match.
	 *
	 * <p>If the state number is not known, this method returns -1.</p>
	 */
	public int getOffendingState() {
		return offendingState;
	}

	protected void setOffendingState(int offendingState) {
		this.offendingState = offendingState;
	}

	/**
	 * Gets the set of input symbols which could potentially follow the
	 * previously matched symbol at the time this exception was thrown.
	 *
	 * <p>If the set of expected tokens is not known and could not be computed,
	 * this method returns {@code null}.</p>
	 *
	 * @return The set of token types that could potentially follow the current
	 * state in the ATN, or {@code null} if the information is not available.
	 */
	public IntervalSet getExpectedTokens() {
		if (recognizer != null) {
			return recognizer.GetATN().GetExpectedTokens(offendingState, ctx);
		}

		return null;
	}

	/**
	 * Gets the {@link RuleContext} at the time this exception was thrown.
	 *
	 * <p>If the context is not available, this method returns {@code null}.</p>
	 *
	 * @return The {@link RuleContext} at the time this exception was thrown.
	 * If the context is not available, this method returns {@code null}.
	 */
	public RuleContext getCtx() {
		return ctx;
	}

	/**
	 * Gets the input stream which is the symbol source for the recognizer where
	 * this exception was thrown.
	 *
	 * <p>If the input stream is not available, this method returns {@code null}.</p>
	 *
	 * @return The input stream which is the symbol source for the recognizer
	 * where this exception was thrown, or {@code null} if the stream is not
	 * available.
	 */
	public IntStream getInputStream() {
		return input;
	}


	public Token getOffendingToken() {
		return offendingToken;
	}

	protected void setOffendingToken(Token offendingToken) {
		this.offendingToken = offendingToken;
	}

	/**
	 * Gets the {@link Recognizer} where this exception occurred.
	 *
	 * <p>If the recognizer is not available, this method returns {@code null}.</p>
	 *
	 * @return The recognizer where this exception occurred, or {@code null} if
	 * the recognizer is not available.
	 */
	public Recognizer getRecognizer() {
		return recognizer;
	}
}
