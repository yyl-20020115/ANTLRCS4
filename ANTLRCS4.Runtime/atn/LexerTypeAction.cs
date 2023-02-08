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
public class LexerTypeAction : LexerAction
{
    private readonly int type;

    /**
	 * Constructs a new {@code type} action with the specified token type value.
	 * @param type The type to assign to the token using {@link Lexer#setType}.
	 */
    public LexerTypeAction(int type) => this.type = type;

    /**
	 * Gets the type to assign to a token created by the lexer.
	 * @return The type to assign to a token created by the lexer.
	 */
    public int Type => type;

    /**
	 * {@inheritDoc}
	 * @return This method returns {@link LexerActionType#TYPE}.
	 */
    //@Override
    public LexerActionType ActionType => LexerActionType.TYPE;

    /**
	 * {@inheritDoc}
	 * @return This method returns {@code false}.
	 */
    //@Override
    public bool IsPositionDependent => false;

    /**
	 * {@inheritDoc}
	 *
	 * <p>This action is implemented by calling {@link Lexer#setType} with the
	 * value provided by {@link #getType}.</p>
	 */
    //@Override
    public void Execute(Lexer lexer) 
        => lexer.Type = type;

    //@Override
    public override int GetHashCode()
    {
        int hash = MurmurHash.Initialize();
        hash = MurmurHash.Update(hash, ActionType);
        hash = MurmurHash.Update(hash, type);
        return MurmurHash.Finish(hash, 2);
    }

    //@Override
    public override bool Equals(object? o) 
        => o == this || (o is LexerTypeAction a) && (type == a.type);

    //@Override
    public override string ToString() => $"type({type})";
}
