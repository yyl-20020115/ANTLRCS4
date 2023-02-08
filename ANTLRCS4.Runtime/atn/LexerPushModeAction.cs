/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime.misc;

namespace org.antlr.v4.runtime.atn;

/**
 * Implements the {@code pushMode} lexer action by calling
 * {@link Lexer#pushMode} with the assigned mode.
 *
 * @author Sam Harwell
 * @since 4.2
 */
public class LexerPushModeAction : LexerAction
{
    private readonly int mode;

    /**
	 * Constructs a new {@code pushMode} action with the specified mode value.
	 * @param mode The mode value to pass to {@link Lexer#pushMode}.
	 */
    public LexerPushModeAction(int mode) 
        => this.mode = mode;

    /**
	 * Get the lexer mode this action should transition the lexer to.
	 *
	 * @return The lexer mode for this {@code pushMode} command.
	 */
    public int Mode => mode;

    /**
	 * {@inheritDoc}
	 * @return This method returns {@link LexerActionType#PUSH_MODE}.
	 */
    //@Override
    public LexerActionType ActionType => LexerActionType.PUSH_MODE;

    /**
	 * {@inheritDoc}
	 * @return This method returns {@code false}.
	 */
    //@Override
    public bool IsPositionDependent => false;

    /**
	 * {@inheritDoc}
	 *
	 * <p>This action is implemented by calling {@link Lexer#pushMode} with the
	 * value provided by {@link #getMode}.</p>
	 */
    //@Override
    public void Execute(Lexer lexer) 
        => lexer.PushMode(mode);

    //@Override
    public override int GetHashCode()
    {
        int hash = MurmurHash.Initialize();
        hash = MurmurHash.Update(hash, ActionType);
        hash = MurmurHash.Update(hash, mode);
        return MurmurHash.Finish(hash, 2);
    }

    //@Override
    public override bool Equals(object? o)
        => o == this || (o is LexerPushModeAction action) && (mode == action.mode);

    //@Override
    public override string ToString() => $"pushMode({mode})";
}
