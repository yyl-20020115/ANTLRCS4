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
public class ATNSerializer
{
    public ATN atn;
    private readonly IntegerList data = new();
    /** Note that we use a LinkedHashMap as a set to mainintain insertion order while deduplicating
	    entries with the same key. */
    private readonly Dictionary<IntervalSet, bool> sets = new();
    private readonly IntegerList nonGreedyStates = new();
    private readonly IntegerList _precedenceStates = new();

    public ATNSerializer(ATN atn) =>
        //assert atn.grammarType != null;
        this.atn = atn;

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
    public IntegerList Serialize()
    {
        AddPreamble();
        int nedges = AddEdges();
        AddNonGreedyStates();
        AddPrecedenceStates();
        AddRuleStatesAndLexerTokenTypes();
        AddModeStartStates();
        var setIndices = AddSets();
        AddEdges(nedges, setIndices);
        AddDecisionStartStates();
        AddLexerActions();

        return data;
    }

    private void AddPreamble()
    {
        data.Add(ATNDeserializer.SERIALIZED_VERSION);
        // convert grammar type to ATN const to avoid dependence on ANTLRParser
        data.Add((int)atn.grammarType);
        data.Add(atn.maxTokenType);
    }

    private void AddLexerActions()
    {
        if (atn.grammarType == ATNType.LEXER)
        {
            data.Add(atn.lexerActions.Length);
            foreach (var action in atn.lexerActions)
            {
                data.Add((int)action.ActionType);
                switch (action.ActionType)
                {
                    case LexerActionType.CHANNEL:
                        int channel = ((LexerChannelAction)action).Channel;
                        data.Add(channel);
                        data.Add(0);
                        break;

                    case LexerActionType.CUSTOM:
                        int ruleIndex = ((LexerCustomAction)action).RuleIndex;
                        int actionIndex = ((LexerCustomAction)action).ActionIndex;
                        data.Add(ruleIndex);
                        data.Add(actionIndex);
                        break;

                    case LexerActionType.MODE:
                        int mode = ((LexerModeAction)action).Mode;
                        data.Add(mode);
                        data.Add(0);
                        break;

                    case LexerActionType.MORE:
                        data.Add(0);
                        data.Add(0);
                        break;

                    case LexerActionType.POP_MODE:
                        data.Add(0);
                        data.Add(0);
                        break;

                    case LexerActionType.PUSH_MODE:
                        mode = ((LexerPushModeAction)action).Mode;
                        data.Add(mode);
                        data.Add(0);
                        break;

                    case LexerActionType.SKIP:
                        data.Add(0);
                        data.Add(0);
                        break;

                    case LexerActionType.TYPE:
                        int type = ((LexerTypeAction)action).Type;
                        data.Add(type);
                        data.Add(0);
                        break;

                    default:
                        throw new ArgumentException($"The specified lexer action type {action.ActionType} is not valid.");
                }
            }
        }
    }

    private void AddDecisionStartStates()
    {
        int ndecisions = atn.decisionToState.Count;
        data.Add(ndecisions);
        foreach (var decStartState in atn.decisionToState)
            data.Add(decStartState.stateNumber);
    }

    private void AddEdges(int nedges, Dictionary<IntervalSet, int> setIndices)
    {
        data.Add(nedges);
        foreach (var s in atn.states)
        {
            // might be optimized away
            if (s == null) continue;
            if (s.StateType == ATNState.RULE_STOP) continue;
            for (int i = 0; i < s.NumberOfTransitions; i++)
            {
                var t = s.Transition(i);
                if (atn.states[(t.target.stateNumber)] == null)
                    throw new IllegalStateException("Cannot serialize a transition to a removed state.");

                int src = s.stateNumber;
                int trg = t.target.stateNumber;
                int edgeType = Transition.serializationTypes[(t.GetType())];
                int arg1 = 0;
                int arg2 = 0;
                int arg3 = 0;
                switch (edgeType)
                {
                    case Transition.RULE:
                        trg = ((RuleTransition)t).followState.stateNumber;
                        arg1 = ((RuleTransition)t).target.stateNumber;
                        arg2 = ((RuleTransition)t).ruleIndex;
                        arg3 = ((RuleTransition)t).precedence;
                        break;
                    case Transition.PRECEDENCE:
                        PrecedencePredicateTransition ppt = (PrecedencePredicateTransition)t;
                        arg1 = ppt.precedence;
                        break;
                    case Transition.PREDICATE:
                        PredicateTransition pt = (PredicateTransition)t;
                        arg1 = pt.ruleIndex;
                        arg2 = pt.predIndex;
                        arg3 = pt.isCtxDependent ? 1 : 0;
                        break;
                    case Transition.RANGE:
                        arg1 = ((RangeTransition)t).from;
                        arg2 = ((RangeTransition)t).to;
                        if (arg1 == Token.EOF)
                        {
                            arg1 = 0;
                            arg3 = 1;
                        }
                        break;
                    case Transition.ATOM:
                        arg1 = ((AtomTransition)t)._label;
                        if (arg1 == Token.EOF)
                        {
                            arg1 = 0;
                            arg3 = 1;
                        }
                        break;
                    case Transition.ACTION:
                        ActionTransition at = (ActionTransition)t;
                        arg1 = at.ruleIndex;
                        arg2 = at.actionIndex;
                        arg3 = at.isCtxDependent ? 1 : 0;
                        break;
                    case Transition.SET:
                    case Transition.NOT_SET:
                        if (setIndices.TryGetValue(((SetTransition)t).label, out var ret))
                        {
                            arg1 = ret;
                        }
                        break;
                    case Transition.WILDCARD:
                        break;
                }

                data.Add(src);
                data.Add(trg);
                data.Add(edgeType);
                data.Add(arg1);
                data.Add(arg2);
                data.Add(arg3);
            }
        }
    }

    private Dictionary<IntervalSet, int> AddSets()
    {
        SerializeSets(data, sets.Keys);
        var setIndices = new Dictionary<IntervalSet, int>();
        int setIndex = 0;
        foreach (var s in sets.Keys)
            setIndices[s] = setIndex++;
        return setIndices;
    }

    private void AddModeStartStates()
    {
        int nmodes = atn.modeToStartState.Count;
        data.Add(nmodes);
        if (nmodes > 0)
            foreach (var modeStartState in atn.modeToStartState)
                data.Add(modeStartState.stateNumber);
    }

    private void AddRuleStatesAndLexerTokenTypes()
    {
        int nrules = atn.ruleToStartState.Length;
        data.Add(nrules);
        for (int r = 0; r < nrules; r++)
        {
            var ruleStartState = atn.ruleToStartState[r];
            data.Add(ruleStartState.stateNumber);
            if (atn.grammarType == ATNType.LEXER)
            {
                //assert atn.ruleToTokenType[r]>=0; // 0 implies fragment rule, other token types > 0
                data.Add(atn.ruleToTokenType[r]);
            }
        }
    }

    private void AddPrecedenceStates()
    {
        data.Add(_precedenceStates.Size);
        for (int i = 0; i < _precedenceStates.Size; i++)
            data.Add(_precedenceStates.Get(i));
    }

    private void AddNonGreedyStates()
    {
        data.Add(nonGreedyStates.Size);
        for (int i = 0; i < nonGreedyStates.Size; i++)
            data.Add(nonGreedyStates.Get(i));
    }

    private int AddEdges()
    {
        int nedges = 0;
        data.Add(atn.states.Count);
        foreach (var s in atn.states)
        {
            if (s == null)
            { // might be optimized away
                data.Add(ATNState.INVALID_TYPE);
                continue;
            }

            int stateType = s.StateType;
            if (s is DecisionState state && state.nonGreedy)
                nonGreedyStates.Add(s.stateNumber);

            if (s is RuleStartState state1 && state1.isLeftRecursiveRule)
                _precedenceStates.Add(s.stateNumber);

            data.Add(stateType);
            data.Add(s.ruleIndex);

            if (s.StateType == ATNState.LOOP_END)
            {
                data.Add(((LoopEndState)s).loopBackState.stateNumber);
            }
            else if (s is BlockStartState state2)
            {
                data.Add(state2.endState.stateNumber);
            }

            if (s.StateType != ATNState.RULE_STOP)
            {
                // the deserializer can trivially derive these edges, so there's no need to serialize them
                nedges += s.NumberOfTransitions;
            }

            for (int i = 0; i < s.NumberOfTransitions; i++)
            {
                var t = s.Transition(i);
                if (Transition.serializationTypes.TryGetValue(t.GetType(), out var edgeType))
                {
                    if (edgeType == Transition.SET || edgeType == Transition.NOT_SET)
                    {
                        var st = (SetTransition)t;
                        sets[st.label] = true;
                    }
                }
            }
        }
        return nedges;
    }

    private static void SerializeSets(IntegerList data, ICollection<IntervalSet> sets)
    {
        int nSets = sets.Count;
        data.Add(nSets);

        foreach (var set in sets)
        {
            bool containsEof = set.Contains(Token.EOF);
            if (containsEof && set.GetIntervals()[(0)].b == Token.EOF)
            {
                data.Add(set.GetIntervals().Count - 1);
            }
            else
            {
                data.Add(set.GetIntervals().Count);
            }

            data.Add(containsEof ? 1 : 0);
            foreach (var I in set.GetIntervals())
            {
                if (I.a == Token.EOF)
                {
                    if (I.b == Token.EOF)
                    {
                        continue;
                    }
                    else
                    {
                        data.Add(0);
                    }
                }
                else
                {
                    data.Add(I.a);
                }
                data.Add(I.b);
            }
        }
    }

    public static IntegerList GetSerialized(ATN atn) 
        => new ATNSerializer(atn).Serialize();
}
