/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime.misc;

namespace org.antlr.v4.runtime.atn;

/**
 * The following images show the relation of states and
 * {@link ATNState#transitions} for various grammar constructs.
 *
 * <ul>
 *
 * <li>Solid edges marked with an &#0949; indicate a required
 * {@link EpsilonTransition}.</li>
 *
 * <li>Dashed edges indicate locations where any transition derived from
 * {@link Transition} might appear.</li>
 *
 * <li>Dashed nodes are place holders for either a sequence of linked
 * {@link BasicState} states or the inclusion of a block representing a nested
 * construct in one of the forms below.</li>
 *
 * <li>Nodes showing multiple outgoing alternatives with a {@code ...} support
 * any number of alternatives (one or more). Nodes without the {@code ...} only
 * support the exact number of alternatives shown in the diagram.</li>
 *
 * </ul>
 *
 * <h2>Basic Blocks</h2>
 *
 * <h3>Rule</h3>
 *
 * <embed src="images/Rule.svg" type="image/svg+xml"/>
 *
 * <h3>Block of 1 or more alternatives</h3>
 *
 * <embed src="images/Block.svg" type="image/svg+xml"/>
 *
 * <h2>Greedy Loops</h2>
 *
 * <h3>Greedy Closure: {@code (...)*}</h3>
 *
 * <embed src="images/ClosureGreedy.svg" type="image/svg+xml"/>
 *
 * <h3>Greedy Positive Closure: {@code (...)+}</h3>
 *
 * <embed src="images/PositiveClosureGreedy.svg" type="image/svg+xml"/>
 *
 * <h3>Greedy Optional: {@code (...)?}</h3>
 *
 * <embed src="images/OptionalGreedy.svg" type="image/svg+xml"/>
 *
 * <h2>Non-Greedy Loops</h2>
 *
 * <h3>Non-Greedy Closure: {@code (...)*?}</h3>
 *
 * <embed src="images/ClosureNonGreedy.svg" type="image/svg+xml"/>
 *
 * <h3>Non-Greedy Positive Closure: {@code (...)+?}</h3>
 *
 * <embed src="images/PositiveClosureNonGreedy.svg" type="image/svg+xml"/>
 *
 * <h3>Non-Greedy Optional: {@code (...)??}</h3>
 *
 * <embed src="images/OptionalNonGreedy.svg" type="image/svg+xml"/>
 */
public abstract class ATNState {
	public const int INITIAL_NUM_TRANSITIONS = 4;

	// constants for serialization
	public const int INVALID_TYPE = 0;
	public const int BASIC = 1;
	public const int RULE_START = 2;
	public const int BLOCK_START = 3;
	public const int PLUS_BLOCK_START = 4;
	public const int STAR_BLOCK_START = 5;
	public const int TOKEN_START = 6;
	public const int RULE_STOP = 7;
	public const int BLOCK_END = 8;
	public const int STAR_LOOP_BACK = 9;
	public const int STAR_LOOP_ENTRY = 10;
	public const int PLUS_LOOP_BACK = 11;
	public const int LOOP_END = 12;
		
	public static readonly List<String> serializationNames = new ()
		{
			"INVALID",
			"BASIC",
			"RULE_START",
			"BLOCK_START",
			"PLUS_BLOCK_START",
			"STAR_BLOCK_START",
			"TOKEN_START",
			"RULE_STOP",
			"BLOCK_END",
			"STAR_LOOP_BACK",
			"STAR_LOOP_ENTRY",
			"PLUS_LOOP_BACK",
			"LOOP_END"
	};

	public const int INVALID_STATE_NUMBER = -1;

    /** Which ATN are we in? */
   	public ATN atn = null;

	public int stateNumber = INVALID_STATE_NUMBER;

	public int ruleIndex; // at runtime, we don't have Rule objects

	public bool epsilonOnlyTransitions = false;

	/** Track the transitions emanating from this ATN state. */
	public readonly List<Transition> transitions =
		new List<Transition>(INITIAL_NUM_TRANSITIONS);

	/** Used to cache lookahead during parsing, not used during construction */
    public IntervalSet nextTokenWithinRule;

	public override int GetHashCode() { return stateNumber; }

	public override bool Equals(Object? o) {
		// are these states same object?
		if ( o is ATNState ) return stateNumber==((ATNState)o).stateNumber;
		return false;
	}

	public bool isNonGreedyExitState() {
		return false;
	}

	public override String ToString() {
		return stateNumber.ToString();
	}

	public Transition[] getTransitions() {
		return transitions.ToArray();
	}

	public int getNumberOfTransitions() {
		return transitions.Count;
	}

	public void addTransition(Transition e) {
		addTransition(transitions.Count, e);
	}

	public void addTransition(int index, Transition e) {
		if (transitions.Count==0) {
			epsilonOnlyTransitions = e.isEpsilon();
		}
		else if (epsilonOnlyTransitions != e.isEpsilon()) {
			Console.Error.WriteLine($"ATN state {stateNumber} has both epsilon and non-epsilon transitions.\n");
			epsilonOnlyTransitions = false;
		}

		bool alreadyPresent = false;
		foreach (Transition t in transitions) {
			if ( t.target.stateNumber == e.target.stateNumber ) {
				if ( t.label()!=null && e.label()!=null && t.label().equals(e.label()) ) {
//					Console.Error.println("Repeated transition upon "+e.label()+" from "+stateNumber+"->"+t.target.stateNumber);
					alreadyPresent = true;
					break;
				}
				else if ( t.isEpsilon() && e.isEpsilon() ) {
//					Console.Error.println("Repeated epsilon transition from "+stateNumber+"->"+t.target.stateNumber);
					alreadyPresent = true;
					break;
				}
			}
		}
		if ( !alreadyPresent ) {
			transitions.Insert(index, e);
		}
	}

	public Transition transition(int i) { return transitions[i]; }

	public void setTransition(int i, Transition e) {
		transitions[i]=e;
	}

	public Transition removeTransition(int index) {
		var t = transitions[index];
		transitions.RemoveAt(index);
		return t;
	}

	public abstract int getStateType();

	public bool onlyHasEpsilonTransitions() {
		return epsilonOnlyTransitions;
	}

	public void setRuleIndex(int ruleIndex) { this.ruleIndex = ruleIndex; }
}
