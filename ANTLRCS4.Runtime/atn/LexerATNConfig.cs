/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime.misc;

namespace org.antlr.v4.runtime.atn;

public class LexerATNConfig : ATNConfig
{
    /**
	 * This is the backing field for {@link #getLexerActionExecutor}.
	 */
    private readonly LexerActionExecutor lexerActionExecutor;
    private readonly bool passedThroughNonGreedyDecision;

    public LexerATNConfig(ATNState state,
                          int alt,
                          PredictionContext context)
        : base(state, alt, context, SemanticContext.Empty.Instance)
    {
        this.passedThroughNonGreedyDecision = false;
        this.lexerActionExecutor = null;
    }

    public LexerATNConfig(ATNState state,
                          int alt,
                          PredictionContext context,
                          LexerActionExecutor lexerActionExecutor)
        : base(state, alt, context, SemanticContext.Empty.Instance)
    {
        this.lexerActionExecutor = lexerActionExecutor;
        this.passedThroughNonGreedyDecision = false;
    }

    public LexerATNConfig(LexerATNConfig c, ATNState state)
        : base(c, state, c.context, c.semanticContext)
    {
        this.lexerActionExecutor = c.lexerActionExecutor;
        this.passedThroughNonGreedyDecision = CheckNonGreedyDecision(c, state);
    }

    public LexerATNConfig(LexerATNConfig c, ATNState state,
                          LexerActionExecutor lexerActionExecutor)
        : base(c, state, c.context, c.semanticContext)
    {
        this.lexerActionExecutor = lexerActionExecutor;
        this.passedThroughNonGreedyDecision = CheckNonGreedyDecision(c, state);
    }

    public LexerATNConfig(LexerATNConfig c, ATNState state,
                          PredictionContext context)
    : base(c, state, context, c.semanticContext)
    {
        this.lexerActionExecutor = c.lexerActionExecutor;
        this.passedThroughNonGreedyDecision = CheckNonGreedyDecision(c, state);
    }

    /**
	 * Gets the {@link LexerActionExecutor} capable of executing the embedded
	 * action(s) for the current configuration.
	 */
    public LexerActionExecutor GetLexerActionExecutor() => lexerActionExecutor;

    public bool HasPassedThroughNonGreedyDecision() => passedThroughNonGreedyDecision;

    //@Override
    public override int GetHashCode()
    {
        int hashCode = MurmurHash.Initialize(7);
        hashCode = MurmurHash.Update(hashCode, state.stateNumber);
        hashCode = MurmurHash.Update(hashCode, alt);
        hashCode = MurmurHash.Update(hashCode, context);
        hashCode = MurmurHash.Update(hashCode, semanticContext);
        hashCode = MurmurHash.Update(hashCode, passedThroughNonGreedyDecision ? 1 : 0);
        hashCode = MurmurHash.Update(hashCode, lexerActionExecutor);
        hashCode = MurmurHash.Finish(hashCode, 6);
        return hashCode;
    }

    //@Override
    public new bool Equals(ATNConfig other)
        => this == other || (other is LexerATNConfig lexerOther) && (passedThroughNonGreedyDecision == lexerOther.passedThroughNonGreedyDecision)
                && (ObjectEqualityComparator.INSTANCE.Equals(lexerActionExecutor, lexerOther.lexerActionExecutor) && base.Equals(other));

    private static bool CheckNonGreedyDecision(LexerATNConfig source, ATNState target) 
        => source.passedThroughNonGreedyDecision
            || target is DecisionState state1 && state1.nonGreedy;
}
