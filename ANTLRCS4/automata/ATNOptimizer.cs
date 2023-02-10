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
public static class ATNOptimizer
{

    public static void Optimize(Grammar g, ATN atn)
    {
        OptimizeSets(g, atn);
        OptimizeStates(atn);
    }

    private static void OptimizeSets(Grammar g, ATN atn)
    {
        if (g.IsParser)
        {
            // parser codegen doesn't currently support SetTransition
            return;
        }

        int removedStates = 0;
        var decisions = atn.decisionToState;
        foreach (var decision in decisions)
        {
            if (decision.ruleIndex >= 0)
            {
                var rule = g.GetRule(decision.ruleIndex);
                if (char.IsLower(rule.name[0]))
                {
                    // parser codegen doesn't currently support SetTransition
                    continue;
                }
            }

            var setTransitions = new IntervalSet();
            for (int i = 0; i < decision.NumberOfTransitions; i++)
            {
                var epsTransition = decision.Transition(i);
                if (epsTransition is not EpsilonTransition)
                {
                    continue;
                }

                if (epsTransition.target.NumberOfTransitions != 1)
                {
                    continue;
                }

                var transition = epsTransition.target.Transition(0);
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
                    setTransitions.Add(i);
                }
            }

            // due to min alt resolution policies, can only collapse sequential alts
            for (int i = setTransitions.GetIntervals().Count - 1; i >= 0; i--)
            {
                var interval = setTransitions.GetIntervals()[i];
                if (interval.Length <= 1)
                {
                    continue;
                }

                var blockEndState = decision.Transition(interval.a).target.Transition(0).target;
                var matchSet = new IntervalSet();
                for (int j = interval.a; j <= interval.b; j++)
                {
                    var matchTransition = decision.Transition(j).target.Transition(0);
                    if (matchTransition is NotSetTransition)
                    {
                        throw new UnsupportedOperationException("Not yet implemented.");
                    }
                    var set = matchTransition.Label;
                    var intervals = set.GetIntervals();
                    int n = intervals.Count;
                    for (int k = 0; k < n; k++)
                    {
                        var setInterval = intervals[(k)];
                        int a = setInterval.a;
                        int b = setInterval.b;
                        if (a != -1 && b != -1)
                        {
                            for (int v = a; v <= b; v++)
                            {
                                if (matchSet.Contains(v))
                                {
                                    // TODO: Token is missing (i.e. position in source is not displayed).
                                    g.Tools.ErrMgr.GrammarError(ErrorType.CHARACTERS_COLLISION_IN_SET, g.fileName,
                                            null,
                                            CharSupport.GetANTLRCharLiteralForChar(v),
                                            CharSupport.GetIntervalSetEscapedString(matchSet));
                                    break;
                                }
                            }
                        }
                    }
                    matchSet.AddAll(set);
                }

                Transition newTransition;
                if (matchSet.GetIntervals().Count == 1)
                {
                    if (matchSet.Count == 1)
                    {
                        newTransition = CodePointTransitions.CreateWithCodePoint(blockEndState, matchSet.GetMinElement());
                    }
                    else
                    {
                        var matchInterval = matchSet.GetIntervals()[0];
                        newTransition = CodePointTransitions.CreateWithCodePointRange(blockEndState, matchInterval.a, matchInterval.b);
                    }
                }
                else
                {
                    newTransition = new SetTransition(blockEndState, matchSet);
                }

                decision.Transition(interval.a).target.SetTransition(0, newTransition);
                for (int j = interval.a + 1; j <= interval.b; j++)
                {
                    var removed = decision.RemoveTransition(interval.a + 1);
                    atn.RemoveState(removed.target);
                    removedStates++;
                }
            }
        }

        //		Console.Out.WriteLine("ATN optimizer removed " + removedStates + " states by collapsing sets.");
    }

    private static void OptimizeStates(ATN atn)
    {
        //		Console.Out.WriteLine(atn.states);
        List<ATNState> compressed = new();
        int i = 0; // new state number
        foreach (var s in atn.states)
        {
            if (s != null)
            {
                compressed.Add(s);
                s.stateNumber = i; // reset state number as we shift to new position
                i++;
            }
        }
        //		Console.Out.WriteLine(compressed);
        //		Console.Out.WriteLine("ATN optimizer removed " + (atn.states.size() - compressed.size()) + " null states.");
        atn.states.Clear();
        atn.states.AddRange(compressed);
    }
}
