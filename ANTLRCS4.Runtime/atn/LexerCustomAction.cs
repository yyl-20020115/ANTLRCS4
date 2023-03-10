/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime.misc;

namespace org.antlr.v4.runtime.atn;

/**
 * Executes a custom lexer action by calling {@link Recognizer#action} with the
 * rule and action indexes assigned to the custom action. The implementation of
 * a custom action is added to the generated code for the lexer in an override
 * of {@link Recognizer#action} when the grammar is compiled.
 *
 * <p>This class may represent embedded actions created with the <code>{...}</code>
 * syntax in ANTLR 4, as well as actions created for lexer commands where the
 * command argument could not be evaluated when the grammar was compiled.</p>
 *
 * @author Sam Harwell
 * @since 4.2
 */
public class LexerCustomAction : LexerAction
{
    private readonly int ruleIndex;
    private readonly int actionIndex;

    /**
	 * Constructs a custom lexer action with the specified rule and action
	 * indexes.
	 *
	 * @param ruleIndex The rule index to use for calls to
	 * {@link Recognizer#action}.
	 * @param actionIndex The action index to use for calls to
	 * {@link Recognizer#action}.
	 */
    public LexerCustomAction(int ruleIndex, int actionIndex)
    {
        this.ruleIndex = ruleIndex;
        this.actionIndex = actionIndex;
    }

    /**
	 * Gets the rule index to use for calls to {@link Recognizer#action}.
	 *
	 * @return The rule index for the custom action.
	 */
    public int RuleIndex => ruleIndex;

    /**
	 * Gets the action index to use for calls to {@link Recognizer#action}.
	 *
	 * @return The action index for the custom action.
	 */
    public int ActionIndex => actionIndex;

    /**
	 * {@inheritDoc}
	 *
	 * @return This method returns {@link LexerActionType#CUSTOM}.
	 */
    
    public LexerActionType ActionType => LexerActionType.CUSTOM;

    /**
	 * Gets whether the lexer action is position-dependent. Position-dependent
	 * actions may have different semantics depending on the {@link CharStream}
	 * index at the time the action is executed.
	 *
	 * <p>Custom actions are position-dependent since they may represent a
	 * user-defined embedded action which makes calls to methods like
	 * {@link Lexer#getText}.</p>
	 *
	 * @return This method returns {@code true}.
	 */
    
    public bool IsPositionDependent => true;

    /**
	 * {@inheritDoc}
	 *
	 * <p>Custom actions are implemented by calling {@link Lexer#action} with the
	 * appropriate rule and action indexes.</p>
	 */
    
    public void Execute(Lexer lexer)
    {
        lexer.Action(null, ruleIndex, actionIndex);
    }

    
    public override int GetHashCode()
    {
        int hash = MurmurHash.Initialize();
        hash = MurmurHash.Update(hash, ActionType);
        hash = MurmurHash.Update(hash, ruleIndex);
        hash = MurmurHash.Update(hash, actionIndex);
        return MurmurHash.Finish(hash, 3);
    }

    
    public override bool Equals(object? o) 
        => o == this || (o is LexerCustomAction other) && (ruleIndex == other.ruleIndex
            && actionIndex == other.actionIndex);
}
