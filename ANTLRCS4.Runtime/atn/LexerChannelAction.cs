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
    public LexerChannelAction(int channel)
    {
        this.channel = channel;
    }

    /**
	 * Gets the channel to use for the {@link Token} created by the lexer.
	 *
	 * @return The channel to use for the {@link Token} created by the lexer.
	 */
    public int GetChannel() => channel;

    /**
	 * {@inheritDoc}
	 * @return This method returns {@link LexerActionType#CHANNEL}.
	 */
    //@Override
    public LexerActionType ActionType => LexerActionType.CHANNEL;

    /**
	 * {@inheritDoc}
	 * @return This method returns {@code false}.
	 */
    //@Override
    public bool IsPositionDependent => false;

    /**
	 * {@inheritDoc}
	 *
	 * <p>This action is implemented by calling {@link Lexer#setChannel} with the
	 * value provided by {@link #getChannel}.</p>
	 */
    //@Override
    public void Execute(Lexer lexer)
    {
        lexer.setChannel(channel);
    }

    //@Override
    public override int GetHashCode()
    {
        int hash = MurmurHash.Initialize();
        hash = MurmurHash.Update(hash, ActionType);
        hash = MurmurHash.Update(hash, channel);
        return MurmurHash.Finish(hash, 2);
    }

    //@Override
    public override bool Equals(Object? obj)
    {
        if (obj == this)
        {
            return true;
        }
        else if (obj is LexerChannelAction action)
        {
            return channel == action.channel;
        }
        return false;

    }

    //@Override
    public override string ToString() => $"channel({channel})";
}
