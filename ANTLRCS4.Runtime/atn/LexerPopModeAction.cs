/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime.misc;

namespace org.antlr.v4.runtime.atn;

/**
 * Implements the {@code popMode} lexer action by calling {@link Lexer#popMode}.
 *
 * <p>The {@code popMode} command does not have any parameters, so this action is
 * implemented as a singleton instance exposed by {@link #INSTANCE}.</p>
 *
 * @author Sam Harwell
 * @since 4.2
 */
public class LexerPopModeAction : LexerAction {
	/**
	 * Provides a singleton instance of this parameterless lexer action.
	 */
	public static readonly LexerPopModeAction INSTANCE = new LexerPopModeAction();

	/**
	 * Constructs the singleton instance of the lexer {@code popMode} command.
	 */
	private LexerPopModeAction() {
	}

	/**
	 * {@inheritDoc}
	 * @return This method returns {@link LexerActionType#POP_MODE}.
	 */
	//@Override
	public LexerActionType getActionType() {
		return LexerActionType.POP_MODE;
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
	 * <p>This action is implemented by calling {@link Lexer#popMode}.</p>
	 */
	//@Override
	public void execute(Lexer lexer) {
		lexer.popMode();
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
		return "popMode";
	}
}
