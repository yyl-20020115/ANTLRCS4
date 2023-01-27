/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.misc;
using org.antlr.v4.runtime.atn;
using org.antlr.v4.runtime.dfa;
using org.antlr.v4.runtime.misc;
using org.antlr.v4.tool;

namespace org.antlr.v4.automata;


/**
 *
 * @author Sam Harwell
 */
public class ATNOptimizer
{

    public static void optimize(Grammar g, ATN atn)
    {
        optimizeSets(g, atn);
        optimizeStates(atn);
    }

    private static void optimizeSets(Grammar g, ATN atn)
    {
        if (g.isParser())
        {
            // parser codegen doesn't currently support SetTransition
            return;
        }

        int removedStates = 0;
        List<DecisionState> decisions = atn.decisionToState;
        foreach (DecisionState decision in decisions)
        {
            if (decision.ruleIndex >= 0)
            {
                Rule rule = g.getRule(decision.ruleIndex);
                if (char.IsLower(rule.name[0]))
                {
                    // parser codegen doesn't currently support SetTransition
                    continue;
                }
            }

            IntervalSet setTransitions = new IntervalSet();
            for (int i = 0; i < decision.getNumberOfTransitions(); i++)
            {
                Transition epsTransition = decision.transition(i);
                if (!(epsTransition is EpsilonTransition))
                {
                    continue;
                }

                if (epsTransition.target.getNumberOfTransitions() != 1)
                {
                    continue;
                }

                Transition transition = epsTransition.target.transition(0);
                if (transition.target is not BlockEndState)
                {
                    continue;
                }

                if (transition is NotSetTransition)
                {
                    // TODO: not yet implemented
                    continue;
                }

                if (transition is AtomTransition
                    || transition is RangeTransition
                    || transition is SetTransition)
                {
                    setTransitions.add(i);
                }
            }

            // due to min alt resolution policies, can only collapse sequential alts
            for (int i = setTransitions.getIntervals().Count - 1; i >= 0; i--)
            {
                Interval interval = setTransitions.getIntervals()[i];
                if (interval.length() <= 1)
                {
                    continue;
                }

                ATNState blockEndState = decision.transition(interval.a).target.transition(0).target;
                IntervalSet matchSet = new IntervalSet();
                for (int j = interval.a; j <= interval.b; j++)
                {
                    Transition matchTransition = decision.transition(j).target.transition(0);
                    if (matchTransition is NotSetTransition)
                    {
                        throw new UnsupportedOperationException("Not yet implemented.");
                    }
                    IntervalSet set = matchTransition.label();
                    List<Interval> intervals = set.getIntervals();
                    int n = intervals.Count;
                    for (int k = 0; k < n; k++)
                    {
                        Interval setInterval = intervals[(k)];
                        int a = setInterval.a;
                        int b = setInterval.b;
                        if (a != -1 && b != -1)
                        {
                            for (int v = a; v <= b; v++)
                            {
                                if (matchSet.contains(v))
                                {
                                    // TODO: Token is missing (i.e. position in source is not displayed).
                                    g.tool.errMgr.grammarError(ErrorType.CHARACTERS_COLLISION_IN_SET, g.fileName,
                                            null,
                                            CharSupport.getANTLRCharLiteralForChar(v),
                                            CharSupport.getIntervalSetEscapedString(matchSet));
                                    break;
                                }
                            }
                        }
                    }
                    matchSet.addAll(set);
                }

                Transition newTransition;
                if (matchSet.getIntervals().Count == 1)
                {
                    if (matchSet.size() == 1)
                    {
                        newTransition = CodePointTransitions.createWithCodePoint(blockEndState, matchSet.getMinElement());
                    }
                    else
                    {
                        Interval matchInterval = matchSet.getIntervals()[0];
                        newTransition = CodePointTransitions.createWithCodePointRange(blockEndState, matchInterval.a, matchInterval.b);
                    }
                }
                else
                {
                    newTransition = new SetTransition(blockEndState, matchSet);
                }

                decision.transition(interval.a).target.setTransition(0, newTransition);
                for (int j = interval.a + 1; j <= interval.b; j++)
                {
                    Transition removed = decision.removeTransition(interval.a + 1);
                    atn.removeState(removed.target);
                    removedStates++;
                }
            }
        }

        //		System.out.println("ATN optimizer removed " + removedStates + " states by collapsing sets.");
    }

    private static void optimizeStates(ATN atn)
    {
        //		System.out.println(atn.states);
        List<ATNState> compressed = new();
        int i = 0; // new state number
        foreach (ATNState s in atn.states)
        {
            if (s != null)
            {
                compressed.Add(s);
                s.stateNumber = i; // reset state number as we shift to new position
                i++;
            }
        }
        //		System.out.println(compressed);
        //		System.out.println("ATN optimizer removed " + (atn.states.size() - compressed.size()) + " null states.");
        atn.states.Clear();
        atn.states.AddRange(compressed);
    }

    private ATNOptimizer()
    {
    }

}
