/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using Antlr4.StringTemplate;
using org.antlr.v4.misc;
using org.antlr.v4.runtime.atn;
using org.antlr.v4.runtime.dfa;
using org.antlr.v4.runtime.misc;
using System.Text;

namespace org.antlr.v4.tool;

/** The DOT (part of graphviz) generation aspect. */
public class DOTGenerator
{
    public static readonly bool STRIP_NONREDUCED_STATES = false;

    protected string arrowhead = "normal";
    protected string rankdir = "LR";

    /** Library of output templates; use {@code <attrname>} format. */
    public readonly static TemplateGroupFile stlib = new("org/antlr/v4/tool/templates/dot/graphs.stg");

    protected readonly Grammar grammar;

    /** This aspect is associated with a grammar */
    public DOTGenerator(Grammar grammar)
    {
        this.grammar = grammar;
    }

    public string GetDOT(DFA dfa, bool isLexer)
    {
        if (dfa.s0 == null) return null;

        var dot = stlib.GetInstanceOf("dfa");
        dot.Add("name", "DFA" + dfa.decision);
        dot.Add("startState", dfa.s0.stateNumber);
        //		dot.add("useBox", Tool.internalOption_ShowATNConfigsInDFA);
        dot.Add("rankdir", rankdir);

        // define stop states first; seems to be a bug in DOT where doublecircle
        foreach (var d in dfa.states.Keys)
        {
            if (!d.isAcceptState) continue;
            var st = stlib.GetInstanceOf("stopstate");
            st.Add("name", "s" + d.stateNumber);
            st.Add("label", GetStateLabel(d));
            dot.Add("states", st);
        }

        foreach (var d in dfa.states.Keys)
        {
            if (d.isAcceptState) continue;
            if (d.stateNumber == int.MaxValue) continue;
            var st = stlib.GetInstanceOf("state");
            st.Add("name", "s" + d.stateNumber);
            st.Add("label", GetStateLabel(d));
            dot.Add("states", st);
        }

        foreach (var d in dfa.states.Keys)
        {
            if (d.edges != null)
            {
                for (int i = 0; i < d.edges.Length; i++)
                {
                    var target = d.edges[i];
                    if (target == null) continue;
                    if (target.stateNumber == int.MaxValue) continue;
                    int ttype = i - 1; // we shift up for EOF as -1 for parser
                    var label = ttype.ToString();
                    if (isLexer) label = "'" + GetEdgeLabel(new StringBuilder().Append(char.ConvertFromUtf32(i)).ToString()) + "'";
                    else if (grammar != null) label = grammar.GetTokenDisplayName(ttype);
                    var st = stlib.GetInstanceOf("edge");
                    st.Add("label", label);
                    st.Add("src", "s" + d.stateNumber);
                    st.Add("target", "s" + target.stateNumber);
                    st.Add("arrowhead", arrowhead);
                    dot.Add("edges", st);
                }
            }
        }

        var output = dot.Render();
        return Utils.SortLinesInString(output);
    }

    protected string GetStateLabel(DFAState s)
    {
        if (s == null) return "null";
        var buffer = new StringBuilder(250);
        buffer.Append('s');
        buffer.Append(s.stateNumber);
        if (s.isAcceptState)
        {
            buffer.Append("=>").Append(s.prediction);
        }
        if (s.requiresFullContext)
        {
            buffer.Append("^");
        }
        if (grammar != null)
        {
            var alts = s.GetAltSet();
            if (alts != null)
            {
                buffer.Append("\\n");
                // separate alts
                var altList = new IntegerList();
                altList.AddAll(alts);
                altList.Sort();
                var configurations = s.configs;
                for (int altIndex = 0; altIndex < altList.Count; altIndex++)
                {
                    int alt = altList.Get(altIndex);
                    if (altIndex > 0)
                    {
                        buffer.Append("\\n");
                    }
                    buffer.Append("alt");
                    buffer.Append(alt);
                    buffer.Append(':');
                    // get a list of configs for just this alt
                    // it will help us print better later
                    List<ATNConfig> configsInAlt = new();
                    foreach (ATNConfig c in configurations)
                    {
                        if (c.alt != alt) continue;
                        configsInAlt.Add(c);
                    }
                    int n = 0;
                    for (int cIndex = 0; cIndex < configsInAlt.Count; cIndex++)
                    {
                        var c = configsInAlt[(cIndex)];
                        n++;
                        buffer.Append(c.ToString(null, false));
                        if ((cIndex + 1) < configsInAlt.Count)
                        {
                            buffer.Append(", ");
                        }
                        if (n % 5 == 0 && (configsInAlt.Count - cIndex) > 3)
                        {
                            buffer.Append("\\n");
                        }
                    }
                }
            }
        }
        string stateLabel = buffer.ToString();
        return stateLabel;
    }

    public string GetDOT(ATNState startState)
    {
        return GetDOT(startState, false);
    }

    public string GetDOT(ATNState startState, bool isLexer)
    {
        var ruleNames = grammar.rules.Keys.ToHashSet();
        var names = new string[ruleNames.Count + 1];
        int i = 0;
        foreach (var s in ruleNames) names[i++] = s;
        return GetDOT(startState, names, isLexer);
    }

    /** Return a string containing a DOT description that, when displayed,
     *  will show the incoming state machine visually.  All nodes reachable
     *  from startState will be included.
     */
    public string GetDOT(ATNState startState, string[] ruleNames, bool isLexer)
    {
        if (startState == null) return null;

        // The output DOT graph for visualization
        HashSet<ATNState> markedStates = new();
        var dot = stlib.GetInstanceOf("atn");
        dot.Add("startState", startState.stateNumber);
        dot.Add("rankdir", rankdir);

        List<ATNState> work = new();

        work.Add(startState);
        while (work.Count > 0)
        {
            var s = work[0];
            if (markedStates.Contains(s)) { work.RemoveAt(0); continue; }
            markedStates.Add(s);

            // don't go past end of rule node to the follow states
            if (s is RuleStopState) continue;

            // special case: if decision point, then line up the alt start states
            // unless it's an end of block
            //			if ( s is BlockStartState ) {
            //				ST rankST = stlib.GetInstanceOf("decision-rank");
            //				DecisionState alt = (DecisionState)s;
            //				for (int i=0; i<alt.getNumberOfTransitions(); i++) {
            //					ATNState target = alt.transition(i).target;
            //					if ( target!=null ) {
            //						rankST.add("states", target.stateNumber);
            //					}
            //				}
            //				dot.add("decisionRanks", rankST);
            //			}

            // make a DOT edge for each transition
            Template edgeST;
            for (int i = 0; i < s.NumberOfTransitions; i++)
            {
                var edge = s.Transition(i);
                if (edge is RuleTransition rr)
                {
                    // don't jump to other rules, but display edge to follow node
                    edgeST = stlib.GetInstanceOf("edge");

                    var label = "<" + ruleNames[rr.ruleIndex];
                    if (((RuleStartState)rr.target).isLeftRecursiveRule)
                    {
                        label += "[" + rr.precedence + "]";
                    }
                    label += ">";

                    edgeST.Add("label", label);
                    edgeST.Add("src", "s" + s.stateNumber);
                    edgeST.Add("target", "s" + rr.followState.stateNumber);
                    edgeST.Add("arrowhead", arrowhead);
                    dot.Add("edges", edgeST);
                    work.Add(rr.followState);
                    continue;
                }
                if (edge is ActionTransition)
                {
                    edgeST = stlib.GetInstanceOf("action-edge");
                    edgeST.Add("label", GetEdgeLabel(edge.ToString()));
                }
                else if (edge is AbstractPredicateTransition)
                {
                    edgeST = stlib.GetInstanceOf("edge");
                    edgeST.Add("label", GetEdgeLabel(edge.ToString()));
                }
                else if (edge.IsEpsilon)
                {
                    edgeST = stlib.GetInstanceOf("epsilon-edge");
                    edgeST.Add("label", GetEdgeLabel(edge.ToString()));
                    bool loopback = false;
                    if (edge.target is PlusBlockStartState state)
                    {
                        loopback = s.Equals(state.loopBackState);
                    }
                    else if (edge.target is StarLoopEntryState state1)
                    {
                        loopback = s.Equals(state1.loopBackState);
                    }
                    edgeST.Add("loopback", loopback);
                }
                else if (edge is AtomTransition)
                {
                    edgeST = stlib.GetInstanceOf("edge");
                    var atom = (AtomTransition)edge;
                    var label = atom._label.ToString();
                    if (isLexer) label = "'" + GetEdgeLabel(new StringBuilder()
                        .Append(char.ConvertFromUtf32(atom._label)).ToString()) + "'";
                    else if (grammar != null) label = grammar.GetTokenDisplayName(atom._label);
                    edgeST.Add("label", GetEdgeLabel(label));
                }
                else if (edge is SetTransition transition)
                {
                    edgeST = stlib.GetInstanceOf("edge");
                    var set = transition;
                    var label = set.Label.ToString();
                    if (isLexer) label = set.Label.ToString(true);
                    else if (grammar != null) label = set.Label.ToString(grammar.Vocabulary);
                    if (edge is NotSetTransition) label = "~" + label;
                    edgeST.Add("label", GetEdgeLabel(label));
                }
                else if (edge is RangeTransition transition1)
                {
                    edgeST = stlib.GetInstanceOf("edge");
                    var range = transition1;
                    var label = range.Label.ToString();
                    if (isLexer) label = range.ToString();
                    else if (grammar != null) label = range.Label.ToString(grammar.Vocabulary);
                    edgeST.Add("label", GetEdgeLabel(label));
                }
                else
                {
                    edgeST = stlib.GetInstanceOf("edge");
                    edgeST.Add("label", GetEdgeLabel(edge.ToString()));
                }
                edgeST.Add("src", "s" + s.stateNumber);
                edgeST.Add("target", "s" + edge.target.stateNumber);
                edgeST.Add("arrowhead", arrowhead);
                if (s.NumberOfTransitions > 1)
                {
                    edgeST.Add("transitionIndex", i);
                }
                else
                {
                    edgeST.Add("transitionIndex", false);
                }
                dot.Add("edges", edgeST);
                work.Add(edge.target);
            }
        }

        // define nodes we visited (they will appear first in DOT output)
        // this is an example of ST's lazy eval :)
        // define stop state first; seems to be a bug in DOT where doublecircle
        // shape only works if we define them first. weird.
        //		ATNState stopState = startState.atn.ruleToStopState.get(startState.rule);
        //		if ( stopState!=null ) {
        //			ST st = stlib.GetInstanceOf("stopstate");
        //			st.add("name", "s"+stopState.stateNumber);
        //			st.add("label", getStateLabel(stopState));
        //			dot.add("states", st);
        //		}
        foreach (var s in markedStates)
        {
            if (s is not RuleStopState) continue;
            var st = stlib.GetInstanceOf("stopstate");
            st.Add("name", "s" + s.stateNumber);
            st.Add("label", GetStateLabel(s));
            dot.Add("states", st);
        }

        foreach (ATNState s in markedStates)
        {
            if (s is RuleStopState) continue;
            var st = stlib.GetInstanceOf("state");
            st.Add("name", "s" + s.stateNumber);
            st.Add("label", GetStateLabel(s));
            st.Add("transitions", s.GetTransitions());
            dot.Add("states", st);
        }

        return dot.Render();
    }


    /** Do a depth-first walk of the state machine graph and
     *  fill a DOT description template.  Keep filling the
     *  states and edges attributes.  We know this is an ATN
     *  for a rule so don't traverse edges to other rules and
     *  don't go past rule end state.
     */
    //    protected void walkRuleATNCreatingDOT(ST dot,
    //                                          ATNState s)
    //    {
    //        if ( markedStates.contains(s) ) {
    //            return; // already visited this node
    //        }
    //
    //        markedStates.add(s.stateNumber); // mark this node as completed.
    //
    //        // first add this node
    //        ST stateST;
    //        if ( s is RuleStopState ) {
    //            stateST = stlib.GetInstanceOf("stopstate");
    //        }
    //        else {
    //            stateST = stlib.GetInstanceOf("state");
    //        }
    //        stateST.add("name", getStateLabel(s));
    //        dot.add("states", stateST);
    //
    //        if ( s is RuleStopState )  {
    //            return; // don't go past end of rule node to the follow states
    //        }
    //
    //        // special case: if decision point, then line up the alt start states
    //        // unless it's an end of block
    //		if ( s is DecisionState ) {
    //			GrammarAST n = ((ATNState)s).ast;
    //			if ( n!=null && s is BlockEndState ) {
    //				ST rankST = stlib.GetInstanceOf("decision-rank");
    //				ATNState alt = (ATNState)s;
    //				while ( alt!=null ) {
    //					rankST.add("states", getStateLabel(alt));
    //					if ( alt.transition(1) !=null ) {
    //						alt = (ATNState)alt.transition(1).target;
    //					}
    //					else {
    //						alt=null;
    //					}
    //				}
    //				dot.add("decisionRanks", rankST);
    //			}
    //		}
    //
    //        // make a DOT edge for each transition
    //		ST edgeST = null;
    //		for (int i = 0; i < s.getNumberOfTransitions(); i++) {
    //            Transition edge = (Transition) s.transition(i);
    //            if ( edge is RuleTransition ) {
    //                RuleTransition rr = ((RuleTransition)edge);
    //                // don't jump to other rules, but display edge to follow node
    //                edgeST = stlib.GetInstanceOf("edge");
    //				if ( rr.rule.g != grammar ) {
    //					edgeST.add("label", "<"+rr.rule.g.name+"."+rr.rule.name+">");
    //				}
    //				else {
    //					edgeST.add("label", "<"+rr.rule.name+">");
    //				}
    //				edgeST.add("src", getStateLabel(s));
    //				edgeST.add("target", getStateLabel(rr.followState));
    //				edgeST.add("arrowhead", arrowhead);
    //                dot.add("edges", edgeST);
    //				walkRuleATNCreatingDOT(dot, rr.followState);
    //                continue;
    //            }
    //			if ( edge is ActionTransition ) {
    //				edgeST = stlib.GetInstanceOf("action-edge");
    //			}
    //			else if ( edge is PredicateTransition ) {
    //				edgeST = stlib.GetInstanceOf("edge");
    //			}
    //			else if ( edge.isEpsilon() ) {
    //				edgeST = stlib.GetInstanceOf("epsilon-edge");
    //			}
    //			else {
    //				edgeST = stlib.GetInstanceOf("edge");
    //			}
    //			edgeST.add("label", getEdgeLabel(edge.ToString(grammar)));
    //            edgeST.add("src", getStateLabel(s));
    //			edgeST.add("target", getStateLabel(edge.target));
    //			edgeST.add("arrowhead", arrowhead);
    //            dot.add("edges", edgeST);
    //            walkRuleATNCreatingDOT(dot, edge.target); // keep walkin'
    //        }
    //    }

    /** Fix edge strings so they print out in DOT properly;
	 *  generate any gated predicates on edge too.
	 */
    protected static string GetEdgeLabel(string label)
    {
        label = label.Replace("\\", "\\\\");
        label = label.Replace("\"", "\\\"");
        label = label.Replace("\n", "\\\\n");
        label = label.Replace("\r", "");
        return label;
    }

    protected static string GetStateLabel(ATNState s)
    {
        if (s == null) return "null";
        string stateLabel = "";

        if (s is BlockStartState)
        {
            stateLabel += "&rarr;\\n";
        }
        else if (s is BlockEndState)
        {
            stateLabel += "&larr;\\n";
        }

        stateLabel += (s.stateNumber.ToString());

        if (s is PlusBlockStartState || s is PlusLoopbackState)
        {
            stateLabel += "+";
        }
        else if (s is StarBlockStartState || s is StarLoopEntryState || s is StarLoopbackState)
        {
            stateLabel += "*";
        }

        if (s is DecisionState && ((DecisionState)s).decision >= 0)
        {
            stateLabel = stateLabel + "\\nd=" + ((DecisionState)s).decision;
        }

        return stateLabel;
    }

}
