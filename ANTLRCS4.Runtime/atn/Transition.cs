/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime.misc;

namespace org.antlr.v4.runtime.atn;

/** An ATN transition between any two ATN states.  Subclasses define
 *  atom, set, epsilon, action, predicate, rule transitions.
 *
 *  <p>This is a one way link.  It emanates from a state (usually via a list of
 *  transitions) and has a target state.</p>
 *
 *  <p>Since we never have to change the ATN transitions once we construct it,
 *  we can fix these transitions as specific classes. The DFA transitions
 *  on the other hand need to update the labels as it adds transitions to
 *  the states. We'll use the term Edge for the DFA to distinguish them from
 *  ATN transitions.</p>
 */
public abstract class Transition {
	// constants for serialization
	public const int EPSILON			= 1;
	public const int RANGE			= 2;
	public const int RULE			= 3;
	public const int PREDICATE		= 4; // e.g., {isType(input.LT(1))}?
	public const int ATOM			= 5;
	public const int ACTION			= 6;
	public const int SET				= 7; // ~(A|B) or ~atom, wildcard, which convert to next 2
	public const int NOT_SET			= 8;
	public const int WILDCARD		= 9;
	public const int PRECEDENCE		= 10;


	public static readonly List<String> serializationNames =
		new () {
			"INVALID",
			"EPSILON",
			"RANGE",
			"RULE",
			"PREDICATE",
			"ATOM",
			"ACTION",
			"SET",
			"NOT_SET",
			"WILDCARD",
			"PRECEDENCE"
		};

	public static readonly Dictionary<Type, int> serializationTypes = new Dictionary<Type, int>()
	{
		[typeof(EpsilonTransition)]=EPSILON,
		[typeof(RangeTransition)]=RANGE,
		[typeof(RuleTransition)]=RULE,
        [typeof(PredicateTransition)] = PREDICATE,
		[typeof(AtomTransition)] = ATOM,
		[typeof(ActionTransition)] = ACTION,
        [typeof(SetTransition)] = SET,
        [typeof(NotSetTransition)] = NOT_SET,
        [typeof(WildcardTransition)] = WILDCARD,
        [typeof(PrecedencePredicateTransition)] = PRECEDENCE,

    };

	/** The target of this transition. */

	public ATNState target;

	protected Transition(ATNState target) {
        this.target = target ?? throw new NullReferenceException(nameof(target));
	}

	public abstract int getSerializationType();

	/**
	 * Determines if the transition is an "epsilon" transition.
	 *
	 * <p>The default implementation returns {@code false}.</p>
	 *
	 * @return {@code true} if traversing this transition in the ATN does not
	 * consume an input symbol; otherwise, {@code false} if traversing this
	 * transition consumes (matches) an input symbol.
	 */
	public virtual bool isEpsilon() {
		return false;
	}


	public virtual IntervalSet label() { return null; }

	public abstract bool matches(int symbol, int minVocabSymbol, int maxVocabSymbol);
}
