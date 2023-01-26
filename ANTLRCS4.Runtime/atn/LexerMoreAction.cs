/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime.misc;

namespace org.antlr.v4.runtime.atn;


/**
 * Implements the {@code more} lexer action by calling {@link Lexer#more}.
 *
 * <p>The {@code more} command does not have any parameters, so this action is
 * implemented as a singleton instance exposed by {@link #INSTANCE}.</p>
 *
 * @author Sam Harwell
 * @since 4.2
 */
public class LexerMoreAction : LexerAction {
	/**
	 * Provides a singleton instance of this parameterless lexer action.
	 */
	public static readonly LexerMoreAction INSTANCE = new LexerMoreAction();

	/**
	 * Constructs the singleton instance of the lexer {@code more} command.
	 */
	private LexerMoreAction() {
	}

	/**
	 * {@inheritDoc}
	 * @return This method returns {@link LexerActionType#MORE}.
	 */
	//@Override
	public LexerActionType getActionType() {
		return LexerActionType.MORE;
	}

	/**
	 * {@inheritDoc}
	 * @return This method returns {@code false}.
	 */
	//@Override
	public bool isPositionDependent() {
		return false;
	}

	/**
	 * {@inheritDoc}
	 *
	 * <p>This action is implemented by calling {@link Lexer#more}.</p>
	 */
	//@Override
	public void execute(Lexer lexer) {
		lexer.more();
	}

	//@Override
	public override int GetHashCode() {
		int hash = MurmurHash.initialize();
		hash = MurmurHash.update(hash, getActionType());
		return MurmurHash.finish(hash, 1);
	}

	//@Override
	//@SuppressWarnings("EqualsWhichDoesntCheckParameterClass")
	public override bool Equals(Object? obj) {
		return obj == this;
	}

	//@Override
	public override String ToString() {
		return "more";
	}
}
