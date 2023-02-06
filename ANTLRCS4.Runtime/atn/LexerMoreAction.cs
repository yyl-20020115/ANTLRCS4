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
public class LexerMoreAction : LexerAction
{
    /**
	 * Provides a singleton instance of this parameterless lexer action.
	 */
    public static readonly LexerMoreAction INSTANCE = new ();

    /**
	 * Constructs the singleton instance of the lexer {@code more} command.
	 */
    private LexerMoreAction() { }

    /**
	 * {@inheritDoc}
	 * @return This method returns {@link LexerActionType#MORE}.
	 */
    //@Override
    public LexerActionType ActionType => LexerActionType.MORE;

    /**
	 * {@inheritDoc}
	 * @return This method returns {@code false}.
	 */
    //@Override
    public bool IsPositionDependent => false;

    /**
	 * {@inheritDoc}
	 *
	 * <p>This action is implemented by calling {@link Lexer#more}.</p>
	 */
    //@Override
    public void Execute(Lexer lexer)
    {
        lexer.more();
    }

    //@Override
    public override int GetHashCode()
    {
        int hash = MurmurHash.Initialize();
        hash = MurmurHash.Update(hash, ActionType);
        return MurmurHash.Finish(hash, 1);
    }

    //@Override
    //@SuppressWarnings("EqualsWhichDoesntCheckParameterClass")
    public override bool Equals(object? o) => o == this;

    //@Override
    public override string ToString() => "more";
}
