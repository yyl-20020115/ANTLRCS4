/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime.dfa;
using org.antlr.v4.runtime.misc;

namespace org.antlr.v4.runtime.atn;

/** This class represents a target neutral serializer for ATNs. An ATN is converted to a list of integers
 *  that can be converted back to and ATN. We compute the list of integers and then generate an array
 *  into the target language for a particular lexer or parser.  Java is a special case where we must
 *  generate strings instead of arrays, but that is handled outside of this class.
 *  See {@link ATNDeserializer#encodeIntsWith16BitWords(IntegerList)} and
 *  {@link org.antlr.v4.codegen.model.SerializedJavaATN}.
 */
public class ATNSerializer {
	public ATN atn;

	private readonly IntegerList data = new ();
	/** Note that we use a LinkedHashMap as a set to mainintain insertion order while deduplicating
	    entries with the same key. */
	private readonly Dictionary<IntervalSet, Boolean> sets = new ();
	private readonly IntegerList nonGreedyStates = new ();
	private readonly IntegerList precedenceStates = new ();

	public ATNSerializer(ATN atn) {
		//assert atn.grammarType != null;
		this.atn = atn;
	}

	/** Serialize state descriptors, edge descriptors, and decision&rarr;state map
	 *  into list of ints.  Likely out of date, but keeping as it could be helpful:
	 *
	 *      SERIALIZED_VERSION
	 *      UUID (2 longs)
	 * 		grammar-type, (ANTLRParser.LEXER, ...)
	 *  	max token type,
	 *  	num states,
	 *  	state-0-type ruleIndex, state-1-type ruleIndex, ... state-i-type ruleIndex optional-arg ...
	 *  	num rules,
	 *  	rule-1-start-state rule-1-args, rule-2-start-state  rule-2-args, ...
	 *  	(args are token type,actionIndex in lexer else 0,0)
	 *      num modes,
	 *      mode-0-start-state, mode-1-start-state, ... (parser has 0 modes)
	 *      num unicode-bmp-sets
	 *      bmp-set-0-interval-count intervals, bmp-set-1-interval-count intervals, ...
	 *      num unicode-smp-sets
	 *      smp-set-0-interval-count intervals, smp-set-1-interval-count intervals, ...
	 *	num total edges,
	 *      src, trg, edge-type, edge arg1, optional edge arg2 (present always), ...
	 *      num decisions,
	 *      decision-0-start-state, decision-1-start-state, ...
	 *
	 *  Convenient to pack into unsigned shorts to make as Java string.
	 */
	public IntegerList serialize() {
		addPreamble();
		int nedges = addEdges();
		addNonGreedyStates();
		addPrecedenceStates();
		addRuleStatesAndLexerTokenTypes();
		addModeStartStates();
		Dictionary<IntervalSet, int> setIndices = null;
		setIndices = addSets();
		addEdges(nedges, setIndices);
		addDecisionStartStates();
		addLexerActions();

		return data;
	}

	private void addPreamble() {
		data.add(ATNDeserializer.SERIALIZED_VERSION);

		// convert grammar type to ATN const to avoid dependence on ANTLRParser
		data.add((int)atn.grammarType);
		data.add(atn.maxTokenType);
	}

	private void addLexerActions() {
		if (atn.grammarType == ATNType.LEXER) {
			data.add(atn.lexerActions.Length);
			foreach (LexerAction action in atn.lexerActions) {
				data.add((int)action.getActionType());
				switch (action.getActionType()) {
				case LexerActionType.CHANNEL:
					int channel = ((LexerChannelAction)action).getChannel();
					data.add(channel);
					data.add(0);
					break;

				case LexerActionType.CUSTOM:
					int ruleIndex = ((LexerCustomAction)action).getRuleIndex();
					int actionIndex = ((LexerCustomAction)action).getActionIndex();
					data.add(ruleIndex);
					data.add(actionIndex);
					break;

				case LexerActionType.MODE:
					int mode = ((LexerModeAction)action).getMode();
					data.add(mode);
					data.add(0);
					break;

				case LexerActionType.MORE:
					data.add(0);
					data.add(0);
					break;

				case LexerActionType.POP_MODE:
					data.add(0);
					data.add(0);
					break;

				case LexerActionType.PUSH_MODE:
					mode = ((LexerPushModeAction)action).getMode();
					data.add(mode);
					data.add(0);
					break;

				case LexerActionType.SKIP:
					data.add(0);
					data.add(0);
					break;

				case LexerActionType.TYPE:
					int type = ((LexerTypeAction)action).getType();
					data.add(type);
					data.add(0);
					break;

				default:
					String message = String.format(Locale.getDefault(), "The specified lexer action type %s is not valid.", action.getActionType());
					throw new ArgumentException(message);
				}
			}
		}
	}

	private void addDecisionStartStates() {
		int ndecisions = atn.decisionToState.Count;
		data.add(ndecisions);
		foreach (DecisionState decStartState in atn.decisionToState) {
			data.add(decStartState.stateNumber);
		}
	}

	private void addEdges(int nedges, Dictionary<IntervalSet, int> setIndices) {
		data.add(nedges);
		foreach (ATNState s in atn.states) {
			if ( s==null ) {
				// might be optimized away
				continue;
			}

			if (s.getStateType() == ATNState.RULE_STOP) {
				continue;
			}

			for (int i=0; i<s.getNumberOfTransitions(); i++) {
				Transition t = s.transition(i);

				if (atn.states.get(t.target.stateNumber) == null) {
					throw new IllegalStateException("Cannot serialize a transition to a removed state.");
				}

				int src = s.stateNumber;
				int trg = t.target.stateNumber;
				int edgeType = Transition.serializationTypes.get(t.getClass());
				int arg1 = 0;
				int arg2 = 0;
				int arg3 = 0;
				switch ( edgeType ) {
					case Transition.RULE :
						trg = ((RuleTransition)t).followState.stateNumber;
						arg1 = ((RuleTransition)t).target.stateNumber;
						arg2 = ((RuleTransition)t).ruleIndex;
						arg3 = ((RuleTransition)t).precedence;
						break;
					case Transition.PRECEDENCE:
						PrecedencePredicateTransition ppt = (PrecedencePredicateTransition)t;
						arg1 = ppt.precedence;
						break;
					case Transition.PREDICATE :
						PredicateTransition pt = (PredicateTransition)t;
						arg1 = pt.ruleIndex;
						arg2 = pt.predIndex;
						arg3 = pt.isCtxDependent ? 1 : 0 ;
						break;
					case Transition.RANGE :
						arg1 = ((RangeTransition)t).from;
						arg2 = ((RangeTransition)t).to;
						if (arg1 == Token.EOF) {
							arg1 = 0;
							arg3 = 1;
						}
						break;
					case Transition.ATOM :
						arg1 = ((AtomTransition)t).label;
						if (arg1 == Token.EOF) {
							arg1 = 0;
							arg3 = 1;
						}
						break;
					case Transition.ACTION :
						ActionTransition at = (ActionTransition)t;
						arg1 = at.ruleIndex;
						arg2 = at.actionIndex;
						arg3 = at.isCtxDependent ? 1 : 0 ;
						break;
					case Transition.SET :
						arg1 = setIndices.get(((SetTransition)t).set);
						break;
					case Transition.NOT_SET :
						arg1 = setIndices.get(((SetTransition)t).set);
						break;
					case Transition.WILDCARD :
						break;
				}

				data.add(src);
				data.add(trg);
				data.add(edgeType);
				data.add(arg1);
				data.add(arg2);
				data.add(arg3);
			}
		}
	}

	private Dictionary<IntervalSet, int> addSets() {
		serializeSets(data,	sets.keySet());
        Dictionary<IntervalSet, int> setIndices = new ();
		int setIndex = 0;
		foreach (IntervalSet s in sets.keySet()) {
			setIndices.put(s, setIndex++);
		}
		return setIndices;
	}

	private void addModeStartStates() {
		int nmodes = atn.modeToStartState.Count;
		data.add(nmodes);
		if ( nmodes>0 ) {
			foreach (ATNState modeStartState in atn.modeToStartState) {
				data.add(modeStartState.stateNumber);
			}
		}
	}

	private void addRuleStatesAndLexerTokenTypes() {
		int nrules = atn.ruleToStartState.length;
		data.add(nrules);
		for (int r=0; r<nrules; r++) {
			ATNState ruleStartState = atn.ruleToStartState[r];
			data.add(ruleStartState.stateNumber);
			if (atn.grammarType == ATNType.LEXER) {
				assert atn.ruleToTokenType[r]>=0; // 0 implies fragment rule, other token types > 0
				data.add(atn.ruleToTokenType[r]);
			}
		}
	}

	private void addPrecedenceStates() {
		data.add(precedenceStates.Count);
		for (int i = 0; i < precedenceStates.Count; i++) {
			data.add(precedenceStates.get(i));
		}
	}

	private void addNonGreedyStates() {
		data.add(nonGreedyStates.Count);
		for (int i = 0; i < nonGreedyStates.Count; i++) {
			data.add(nonGreedyStates.get(i));
		}
	}

	private int addEdges() {
		int nedges = 0;
		data.add(atn.states.Count);
		foreach (ATNState s in atn.states) {
			if ( s==null ) { // might be optimized away
				data.add(ATNState.INVALID_TYPE);
				continue;
			}

			int stateType = s.getStateType();
			if (s is DecisionState && ((DecisionState)s).nonGreedy) {
				nonGreedyStates.add(s.stateNumber);
			}

			if (s is RuleStartState && ((RuleStartState)s).isLeftRecursiveRule) {
				precedenceStates.add(s.stateNumber);
			}

			data.add(stateType);

			data.add(s.ruleIndex);

			if ( s.getStateType() == ATNState.LOOP_END ) {
				data.add(((LoopEndState)s).loopBackState.stateNumber);
			}
			else if ( s is BlockStartState ) {
				data.add(((BlockStartState)s).endState.stateNumber);
			}

			if (s.getStateType() != ATNState.RULE_STOP) {
				// the deserializer can trivially derive these edges, so there's no need to serialize them
				nedges += s.getNumberOfTransitions();
			}

			for (int i=0; i<s.getNumberOfTransitions(); i++) {
				Transition t = s.transition(i);
				int edgeType = Transition.serializationTypes.get(t.getClass());
				if ( edgeType == Transition.SET || edgeType == Transition.NOT_SET ) {
					SetTransition st = (SetTransition)t;
					sets.put(st.set, true);
				}
			}
		}
		return nedges;
	}

	private static void serializeSets(IntegerList data, ICollection<IntervalSet> sets) {
		int nSets = sets.Count;
		data.add(nSets);

		foreach (IntervalSet set in sets) {
			bool containsEof = set.contains(Token.EOF);
			if (containsEof && set.getIntervals().get(0).b == Token.EOF) {
				data.add(set.getIntervals().Count - 1);
			}
			else {
				data.add(set.getIntervals().Count);
			}

			data.add(containsEof ? 1 : 0);
			foreach (Interval I in set.getIntervals()) {
				if (I.a == Token.EOF) {
					if (I.b == Token.EOF) {
						continue;
					}
					else {
						data.add(0);
					}
				}
				else {
					data.add(I.a);
				}
				data.add(I.b);
			}
		}
	}

	public static IntegerList getSerialized(ATN atn) {
		return new ATNSerializer(atn).serialize();
	}
}
