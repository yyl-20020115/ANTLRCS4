/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime.misc;

namespace org.antlr.v4.runtime.atn;

/**
 * Implements the {@code type} lexer action by calling {@link Lexer#setType}
 * with the assigned type.
 *
 * @author Sam Harwell
 * @since 4.2
 */
public class LexerTypeAction : LexerAction {
	private readonly int type;

	/**
	 * Constructs a new {@code type} action with the specified token type value.
	 * @param type The type to assign to the token using {@link Lexer#setType}.
	 */
	public LexerTypeAction(int type) {
		this.type = type;
	}

	/**
	 * Gets the type to assign to a token created by the lexer.
	 * @return The type to assign to a token created by the lexer.
	 */
	public int getType() {
		return type;
	}

	/**
	 * {@inheritDoc}
	 * @return This method returns {@link LexerActionType#TYPE}.
	 */
	//@Override
	public LexerActionType getActionType() {
		return LexerActionType.TYPE;
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
	 * <p>This action is implemented by calling {@link Lexer#setType} with the
	 * value provided by {@link #getType}.</p>
	 */
	//@Override
	public void execute(Lexer lexer) {
		lexer.setType(type);
	}

	//@Override
	public int GetHashCode() {
		int hash = MurmurHash.initialize();
		hash = MurmurHash.update(hash, getActionType());
		hash = MurmurHash.update(hash, type);
		return MurmurHash.finish(hash, 2);
	}

	//@Override
	public override bool Equals(Object? obj) {
		if (obj == this) {
			return true;
		}
		else if (!(obj is LexerTypeAction)) {
			return false;
		}

		return type == ((LexerTypeAction)obj).type;
	}

	//@Override
	public override String ToString() {
		return $"type({type})";
	}
}
