/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime.misc;

namespace org.antlr.v4.runtime.atn;

/**
 * This implementation of {@link LexerAction} is used for tracking input offsets
 * for position-dependent actions within a {@link LexerActionExecutor}.
 *
 * <p>This action is not serialized as part of the ATN, and is only required for
 * position-dependent lexer actions which appear at a location other than the
 * end of a rule. For more information about DFA optimizations employed for
 * lexer actions, see {@link LexerActionExecutor#append} and
 * {@link LexerActionExecutor#fixOffsetBeforeMatch}.</p>
 *
 * @author Sam Harwell
 * @since 4.2
 */
public class LexerIndexedCustomAction : LexerAction
{
    private readonly int offset;
    private readonly LexerAction action;

    /**
	 * Constructs a new indexed custom action by associating a character offset
	 * with a {@link LexerAction}.
	 *
	 * <p>Note: This class is only required for lexer actions for which
	 * {@link LexerAction#isPositionDependent} returns {@code true}.</p>
	 *
	 * @param offset The offset into the input {@link CharStream}, relative to
	 * the token start index, at which the specified lexer action should be
	 * executed.
	 * @param action The lexer action to execute at a particular offset in the
	 * input {@link CharStream}.
	 */
    public LexerIndexedCustomAction(int offset, LexerAction action)
    {
        this.offset = offset;
        this.action = action;
    }

    /**
	 * Gets the location in the input {@link CharStream} at which the lexer
	 * action should be executed. The value is interpreted as an offset relative
	 * to the token start index.
	 *
	 * @return The location in the input {@link CharStream} at which the lexer
	 * action should be executed.
	 */
    public int Offset => offset;

    /**
	 * Gets the lexer action to execute.
	 *
	 * @return A {@link LexerAction} object which executes the lexer action.
	 */
    public LexerAction Action => action;

    /**
	 * {@inheritDoc}
	 *
	 * @return This method returns the result of calling {@link #getActionType}
	 * on the {@link LexerAction} returned by {@link #getAction}.
	 */
    
    public LexerActionType ActionType => action.ActionType;

    /**
	 * {@inheritDoc}
	 * @return This method returns {@code true}.
	 */
    
    public bool IsPositionDependent => true;

    /**
	 * {@inheritDoc}
	 *
	 * <p>This method calls {@link #execute} on the result of {@link #getAction}
	 * using the provided {@code lexer}.</p>
	 */
    
    public void Execute(Lexer lexer) =>
        // assume the input stream position was properly set by the calling code
        action.Execute(lexer);

    
    public override int GetHashCode()
    {
        var hash = MurmurHash.Initialize();
        hash = MurmurHash.Update(hash, offset);
        hash = MurmurHash.Update(hash, action);
        return MurmurHash.Finish(hash, 2);
    }

    
    public override bool Equals(object? o) 
		=> o == this || (o is LexerIndexedCustomAction other) && (offset == other.offset
                && action.Equals(other.action));

}
