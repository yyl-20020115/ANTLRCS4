/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime.misc;

namespace org.antlr.v4.runtime.atn;

/**
 * Implements the {@code channel} lexer action by calling
 * {@link Lexer#setChannel} with the assigned channel.
 *
 * @author Sam Harwell
 * @since 4.2
 */
public class LexerChannelAction : LexerAction
{
    private readonly int channel;

    /**
	 * Constructs a new {@code channel} action with the specified channel value.
	 * @param channel The channel value to pass to {@link Lexer#setChannel}.
	 */
    public LexerChannelAction(int channel) => this.channel = channel;

    /**
	 * Gets the channel to use for the {@link Token} created by the lexer.
	 *
	 * @return The channel to use for the {@link Token} created by the lexer.
	 */
    public int Channel => channel;

    /**
	 * {@inheritDoc}
	 * @return This method returns {@link LexerActionType#CHANNEL}.
	 */
    
    public LexerActionType ActionType => LexerActionType.CHANNEL;

    /**
	 * {@inheritDoc}
	 * @return This method returns {@code false}.
	 */
    
    public bool IsPositionDependent => false;

    /**
	 * {@inheritDoc}
	 *
	 * <p>This action is implemented by calling {@link Lexer#setChannel} with the
	 * value provided by {@link #getChannel}.</p>
	 */
    
    public void Execute(Lexer lexer) => lexer.Channel = channel;

    
    public override int GetHashCode()
    {
        int hash = MurmurHash.Initialize();
        hash = MurmurHash.Update(hash, ActionType);
        hash = MurmurHash.Update(hash, channel);
        return MurmurHash.Finish(hash, 2);
    }

    
    public override bool Equals(object? o) 
        => o == this || (o is LexerChannelAction action) && (channel == action.channel);

    
    public override string ToString() => $"channel({channel})";
}
