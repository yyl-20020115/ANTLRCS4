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
public class LexerPopModeAction : LexerAction
{
    /**
	 * Provides a singleton instance of this parameterless lexer action.
	 */
    public static readonly LexerPopModeAction INSTANCE = new ();

    /**
	 * Constructs the singleton instance of the lexer {@code popMode} command.
	 */
    private LexerPopModeAction() { }

    /**
	 * {@inheritDoc}
	 * @return This method returns {@link LexerActionType#POP_MODE}.
	 */
    
    public LexerActionType ActionType => LexerActionType.POP_MODE;

    /**
	 * {@inheritDoc}
	 * @return This method returns {@code false}.
	 */
    
    public bool IsPositionDependent => false;

    /**
	 * {@inheritDoc}
	 *
	 * <p>This action is implemented by calling {@link Lexer#popMode}.</p>
	 */
    
    public void Execute(Lexer lexer) => lexer.PopMode();

    
    public override int GetHashCode()
    {
        int hash = MurmurHash.Initialize();
        hash = MurmurHash.Update(hash, ActionType);
        return MurmurHash.Finish(hash, 1);
    }

    public override bool Equals(object? obj) => obj == this;

    
    public override string ToString() => "popMode";
}
