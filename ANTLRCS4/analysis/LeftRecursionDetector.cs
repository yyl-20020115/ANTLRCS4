/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime.atn;
using org.antlr.v4.runtime.misc;
using org.antlr.v4.tool;
using Rule = org.antlr.v4.tool.Rule;

namespace org.antlr.v4.analysis;
public class LeftRecursionDetector
{
    public readonly Grammar g;
    public readonly ATN atn;

    /** Holds a list of cycles (sets of rule names). */
    public readonly List<HashSet<Rule>> listOfRecursiveCycles = new();

    /** Which rule start states have we visited while looking for a single
	 * 	left-recursion check?
	 */
    public readonly HashSet<RuleStartState> rulesVisitedPerRuleCheck = new ();

    public LeftRecursionDetector(Grammar g, ATN atn)
    {
        this.g = g;
        this.atn = atn;
    }

    public void Check()
    {
        foreach (var start in atn.ruleToStartState)
        {
            //System.out.print("check "+start.rule.name);
            rulesVisitedPerRuleCheck.Clear();
            rulesVisitedPerRuleCheck.Add(start);
            //FASerializer ser = new FASerializer(atn.g, start);
            //System.out.print(":\n"+ser+"\n");

            Check(g.getRule(start.ruleIndex), start, new HashSet<ATNState>());
        }
        //Console.Out.WriteLine("cycles="+listOfRecursiveCycles);
        if (listOfRecursiveCycles.Count > 0)
        {
            g.Tools.ErrMgr.leftRecursionCycles(g.fileName, listOfRecursiveCycles);
        }
    }

    /** From state s, look for any transition to a rule that is currently
	 *  being traced.  When tracing r, visitedPerRuleCheck has r
	 *  initially.  If you reach a rule stop state, return but notify the
	 *  invoking rule that the called rule is nullable. This implies that
	 *  invoking rule must look at follow transition for that invoking state.
	 *
	 *  The visitedStates tracks visited states within a single rule so
	 *  we can avoid epsilon-loop-induced infinite recursion here.  Keep
	 *  filling the cycles in listOfRecursiveCycles and also, as a
	 *  side-effect, set leftRecursiveRules.
	 */
    public bool Check(Rule enclosingRule, ATNState s, HashSet<ATNState> visitedStates)
    {
        if (s is RuleStopState) return true;
        if (visitedStates.Contains(s)) return false;
        visitedStates.Add(s);

        //Console.Out.WriteLine("visit "+s);
        int n = s.NumberOfTransitions;
        bool stateReachesStopState = false;
        for (int i = 0; i < n; i++)
        {
            var t = s.Transition(i);
            if (t is RuleTransition rt)
            {
                var r = g.getRule(rt.ruleIndex);
                if (rulesVisitedPerRuleCheck.Contains(rt.target))
                {
                    AddRulesToCycle(enclosingRule, r);
                }
                else
                {
                    // must visit if not already visited; mark target, pop when done
                    rulesVisitedPerRuleCheck.Add((RuleStartState)t.target);
                    // send new visitedStates set per rule invocation
                    bool nullable = Check(r, t.target, new ());
                    // we're back from visiting that rule
                    rulesVisitedPerRuleCheck.Remove((RuleStartState)t.target);
                    if (nullable)
                    {
                        stateReachesStopState |= Check(enclosingRule, rt.followState, visitedStates);
                    }
                }
            }
            else if (t.IsEpsilon)
            {
                stateReachesStopState |= Check(enclosingRule, t.target, visitedStates);
            }
            // else ignore non-epsilon transitions
        }
        return stateReachesStopState;
    }

    /** enclosingRule calls targetRule. Find the cycle containing
	 *  the target and add the caller.  Find the cycle containing the caller
	 *  and add the target.  If no cycles contain either, then create a new
	 *  cycle.
	 */
    protected void AddRulesToCycle(Rule enclosingRule, Rule targetRule)
    {
        //Console.Error.WriteLine("left-recursion to "+targetRule.name+" from "+enclosingRule.name);
        bool foundCycle = false;
        foreach (var rulesInCycle in listOfRecursiveCycles)
        {
            // ensure both rules are in same cycle
            if (rulesInCycle.Contains(targetRule))
            {
                rulesInCycle.Add(enclosingRule);
                foundCycle = true;
            }
            if (rulesInCycle.Contains(enclosingRule))
            {
                rulesInCycle.Add(targetRule);
                foundCycle = true;
            }
        }
        if (!foundCycle)
        {
            var cycle = new OrderedHashSet<Rule>();
            cycle.add(targetRule);
            cycle.add(enclosingRule);
            listOfRecursiveCycles.Add(cycle);
        }
    }
}
