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
public class LexerModeAction : LexerAction
{
    private readonly int mode;

    /**
	 * Constructs a new {@code mode} action with the specified mode value.
	 * @param mode The mode value to pass to {@link Lexer#mode}.
	 */
    public LexerModeAction(int mode)
    {
        this.mode = mode;
    }

    /**
	 * Get the lexer mode this action should transition the lexer to.
	 *
	 * @return The lexer mode for this {@code mode} command.
	 */
    public int Mode => mode;

    /**
	 * {@inheritDoc}
	 * @return This method returns {@link LexerActionType#MODE}.
	 */
    //@Override
    public LexerActionType ActionType => LexerActionType.MODE;

    /**
	 * {@inheritDoc}
	 * @return This method returns {@code false}.
	 */
    //@Override
    public bool IsPositionDependent => false;

    /**
	 * {@inheritDoc}
	 *
	 * <p>This action is implemented by calling {@link Lexer#mode} with the
	 * value provided by {@link #getMode}.</p>
	 */
    //@Override
    public void Execute(Lexer lexer)
    {
        lexer.Mode(mode);
    }

    //@Override
    public override int GetHashCode()
    {
        int hash = MurmurHash.Initialize();
        hash = MurmurHash.Update(hash, ActionType);
        hash = MurmurHash.Update(hash, mode);
        return MurmurHash.Finish(hash, 2);
    }

    //@Override
    public override bool Equals(object? obj)
    {
        if (obj == this)
        {
            return true;
        }
        else if (obj is LexerModeAction action)
        {
            return mode == action.mode;
        }
        return false;

    }

    //@Override
    public override string ToString() => $"mode({mode})";
}
