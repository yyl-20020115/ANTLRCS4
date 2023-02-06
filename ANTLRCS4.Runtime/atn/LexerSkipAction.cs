/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime.misc;

namespace org.antlr.v4.runtime.atn;

/**
 * Implements the {@code skip} lexer action by calling {@link Lexer#skip}.
 *
 * <p>The {@code skip} command does not have any parameters, so this action is
 * implemented as a singleton instance exposed by {@link #INSTANCE}.</p>
 *
 * @author Sam Harwell
 * @since 4.2
 */
public class LexerSkipAction : LexerAction {
	/**
	 * Provides a singleton instance of this parameterless lexer action.
	 */
	public static readonly LexerSkipAction INSTANCE = new LexerSkipAction();

	/**
	 * Constructs the singleton instance of the lexer {@code skip} command.
	 */
	private LexerSkipAction() {
	}

    /**
	 * {@inheritDoc}
	 * @return This method returns {@link LexerActionType#SKIP}.
	 */
    //@Override
    public LexerActionType ActionType => LexerActionType.SKIP;

    /**
	 * {@inheritDoc}
	 * @return This method returns {@code false}.
	 */
    //@Override
    public bool IsPositionDependent => false;

    /**
	 * {@inheritDoc}
	 *
	 * <p>This action is implemented by calling {@link Lexer#skip}.</p>
	 */
    //@Override
    public void Execute(Lexer lexer) {
		lexer.skip();
	}

	//@Override
	public override int GetHashCode() {
		int hash = MurmurHash.Initialize();
		hash = MurmurHash.Update(hash, ActionType);
		return MurmurHash.Finish(hash, 1);
	}

	//@Override
	//@SuppressWarnings("EqualsWhichDoesntCheckParameterClass")
	public override bool Equals(Object? obj) {
		return obj == this;
	}

	//@Override
	public override String ToString() {
		return "skip";
	}
}
