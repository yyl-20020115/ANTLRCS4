/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.misc;
using org.antlr.v4.runtime;
using org.antlr.v4.runtime.atn;
using org.antlr.v4.runtime.misc;
using org.antlr.v4.tool;
using org.antlr.v4.tool.ast;

namespace org.antlr.v4.analysis;

public class AnalysisPipeline
{
    public readonly Grammar g;

    public AnalysisPipeline(Grammar g) => this.g = g;

    public void Process()
    {
        // LEFT-RECURSION CHECK
        var lr = new LeftRecursionDetector(g, g.atn);
        lr.Check();
        if (lr.listOfRecursiveCycles.Count > 0) return; // bail out

        if (g.isLexer())
        {
            ProcessLexer();
        }
        else
        {
            // BUILD DFA FOR EACH DECISION
            ProcessParser();
        }
    }

    protected void ProcessLexer()
    {
        // make sure all non-fragment lexer rules must match at least one symbol
        foreach (var rule in g.rules.Values)
        {
            if (rule.isFragment())
                continue;

            var analyzer = new LL1Analyzer(g.atn);
            var look = analyzer.LOOK(g.atn.ruleToStartState[rule.index], null);
            if (look.Contains(Token.EPSILON))
                g.Tools.ErrMgr.GrammarError(ErrorType.EPSILON_TOKEN, g.fileName, ((GrammarAST)rule.ast.getChild(0)).getToken(), rule.name);
        }
    }

    protected void ProcessParser()
    {
        g.decisionLOOK = new(g.atn.getNumberOfDecisions() + 1);
        foreach (var state in g.atn.decisionToState)
        {
            g.Tools.Log("LL1", "\nDECISION " + state.decision + " in rule " + g.getRule(state.ruleIndex).name);
            IntervalSet[] look;
            if (state.nonGreedy)
            { // nongreedy decisions can't be LL(1)
                look = new IntervalSet[state.getNumberOfTransitions() + 1];
            }
            else
            {
                var anal = new LL1Analyzer(g.atn);
                look = anal.getDecisionLookahead(state);
                g.Tools.Log("LL1", "look=" + RuntimeUtils.Join(look, ","));
            }

            //assert s.decision + 1 >= g.decisionLOOK.size();
            Utils.SetSize(g.decisionLOOK, state.decision + 1);
            g.decisionLOOK[state.decision] = look;
            g.Tools.Log("LL1", "LL(1)? " + Disjoint(look));
        }
    }

    /** Return whether lookahead sets are disjoint; no lookahead ⇒ not disjoint */
    public static bool Disjoint(IntervalSet[] altLook)
    {
        bool collision = false;
        var combined = new IntervalSet();
        if (altLook == null) return false;
        foreach (var look in altLook)
        {
            if (look == null) return false; // lookahead must've computation failed
            if (!look.and(combined).IsNil)
            {
                collision = true;
                break;
            }
            combined.addAll(look);
        }
        return !collision;
    }
}
