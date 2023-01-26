/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime.misc;

namespace org.antlr.v4.runtime.atn;

public class LexerATNConfig : ATNConfig {
	/**
	 * This is the backing field for {@link #getLexerActionExecutor}.
	 */
	private readonly LexerActionExecutor lexerActionExecutor;

	private readonly bool passedThroughNonGreedyDecision;

	public LexerATNConfig(ATNState state,
						  int alt,
						  PredictionContext context)
		:base(state, alt, context, SemanticContext.Empty.Instance)
    {
		;
		this.passedThroughNonGreedyDecision = false;
		this.lexerActionExecutor = null;
	}

	public LexerATNConfig(ATNState state,
						  int alt,
						  PredictionContext context,
						  LexerActionExecutor lexerActionExecutor)
		:base(state, alt, context, SemanticContext.Empty.Instance)
    {
		;
		this.lexerActionExecutor = lexerActionExecutor;
		this.passedThroughNonGreedyDecision = false;
	}

	public LexerATNConfig(LexerATNConfig c, ATNState state)
        :base(c, state, c.context, c.semanticContext)
    {
		;
		this.lexerActionExecutor = c.lexerActionExecutor;
		this.passedThroughNonGreedyDecision = checkNonGreedyDecision(c, state);
	}

	public LexerATNConfig(LexerATNConfig c, ATNState state,
						  LexerActionExecutor lexerActionExecutor)
		:base(c, state, c.context, c.semanticContext)
	{
		
		this.lexerActionExecutor = lexerActionExecutor;
		this.passedThroughNonGreedyDecision = checkNonGreedyDecision(c, state);
	}

	public LexerATNConfig(LexerATNConfig c, ATNState state,
						  PredictionContext context) 
	:base(c, state, context, c.semanticContext)
	{
	

        this.lexerActionExecutor = c.lexerActionExecutor;
		this.passedThroughNonGreedyDecision = checkNonGreedyDecision(c, state);
	}

	/**
	 * Gets the {@link LexerActionExecutor} capable of executing the embedded
	 * action(s) for the current configuration.
	 */
	public LexerActionExecutor getLexerActionExecutor() {
		return lexerActionExecutor;
	}

	public bool hasPassedThroughNonGreedyDecision() {
		return passedThroughNonGreedyDecision;
	}

	//@Override
	public int hashCode() {
		int hashCode = MurmurHash.initialize(7);
		hashCode = MurmurHash.update(hashCode, state.stateNumber);
		hashCode = MurmurHash.update(hashCode, alt);
		hashCode = MurmurHash.update(hashCode, context);
		hashCode = MurmurHash.update(hashCode, semanticContext);
		hashCode = MurmurHash.update(hashCode, passedThroughNonGreedyDecision ? 1 : 0);
		hashCode = MurmurHash.update(hashCode, lexerActionExecutor);
		hashCode = MurmurHash.finish(hashCode, 6);
		return hashCode;
	}

	//@Override
	public bool Equals(ATNConfig other) {
		if (this == other) {
			return true;
		}
		else if (!(other is LexerATNConfig)) {
			return false;
		}

		LexerATNConfig lexerOther = (LexerATNConfig)other;
		if (passedThroughNonGreedyDecision != lexerOther.passedThroughNonGreedyDecision) {
			return false;
		}

		if (!ObjectEqualityComparator.INSTANCE.Equals(lexerActionExecutor, lexerOther.lexerActionExecutor)) {
			return false;
		}

		return base.Equals(other);
	}

	private static bool checkNonGreedyDecision(LexerATNConfig source, ATNState target) {
		return source.passedThroughNonGreedyDecision
			|| target is DecisionState state1 && state1.nonGreedy;
	}
}
