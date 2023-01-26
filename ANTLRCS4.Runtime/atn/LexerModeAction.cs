/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime.misc;

namespace org.antlr.v4.runtime.atn;

/**
 * Implements the {@code mode} lexer action by calling {@link Lexer#mode} with
 * the assigned mode.
 *
 * @author Sam Harwell
 * @since 4.2
 */
public  class LexerModeAction : LexerAction {
	private readonly int mode;

	/**
	 * Constructs a new {@code mode} action with the specified mode value.
	 * @param mode The mode value to pass to {@link Lexer#mode}.
	 */
	public LexerModeAction(int mode) {
		this.mode = mode;
	}

	/**
	 * Get the lexer mode this action should transition the lexer to.
	 *
	 * @return The lexer mode for this {@code mode} command.
	 */
	public int getMode() {
		return mode;
	}

	/**
	 * {@inheritDoc}
	 * @return This method returns {@link LexerActionType#MODE}.
	 */
	//@Override
	public LexerActionType getActionType() {
		return LexerActionType.MODE;
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
	 * <p>This action is implemented by calling {@link Lexer#mode} with the
	 * value provided by {@link #getMode}.</p>
	 */
    //@Override
    public void execute(Lexer lexer) {
		lexer.mode(mode);
	}

    //@Override
    public override int GetHashCode() {
		int hash = MurmurHash.initialize();
		hash = MurmurHash.update(hash, getActionType());
		hash = MurmurHash.update(hash, mode);
		return MurmurHash.finish(hash, 2);
	}

    //@Override
    public override bool Equals(Object? obj) {
		if (obj == this) {
			return true;
		}
		else if (!(obj is LexerModeAction)) {
			return false;
		}

		return mode == ((LexerModeAction)obj).mode;
	}

    //@Override
    public override String ToString() {
		return $"mode({mode})";
	}
}
