/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

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
	public static readonly int EPSILON			= 1;
	public static readonly int RANGE			= 2;
	public static readonly int RULE			= 3;
	public static readonly int PREDICATE		= 4; // e.g., {isType(input.LT(1))}?
	public static readonly int ATOM			= 5;
	public static readonly int ACTION			= 6;
	public static readonly int SET				= 7; // ~(A|B) or ~atom, wildcard, which convert to next 2
	public static readonly int NOT_SET			= 8;
	public static readonly int WILDCARD		= 9;
	public static readonly int PRECEDENCE		= 10;


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


	public IntervalSet label() { return null; }

	public abstract bool matches(int symbol, int minVocabSymbol, int maxVocabSymbol);
}
