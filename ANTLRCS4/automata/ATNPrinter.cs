/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime.atn;
using org.antlr.v4.tool;
using System.Text;

namespace org.antlr.v4.automata;


/** An ATN walker that knows how to dump them to serialized strings. */
public class ATNPrinter
{
    private List<ATNState> work;
    private HashSet<ATNState> marked;
    private readonly Grammar g;
    private readonly ATNState start;

    public ATNPrinter(Grammar g, ATNState start)
    {
        this.g = g;
        this.start = start;
    }

    public string AsString()
    {
        if (start == null) return null;
        marked = new HashSet<ATNState>();
        work = new();
        work.Add(start);

        var builder = new StringBuilder();
        ATNState s;

        while (work.Count > 0)
        {
            s = work[0];
            work.RemoveAt(0);
            if (marked.Contains(s)) continue;
            int n = s.NumberOfTransitions;
            //			Console.Out.WriteLine("visit "+s+"; edges="+n);
            marked.Add(s);
            for (int i = 0; i < n; i++)
            {
                var t = s.Transition(i);
                if (s is not RuleStopState)
                { // don't add follow states to work
                    if (t is RuleTransition transition) work.Add(transition.followState);
                    else work.Add(t.target);
                }
                builder.Append(GetStateString(s));
                if (t is EpsilonTransition)
                {
                    builder.Append("->").Append(GetStateString(t.target)).Append('\n');
                }
                else if (t is RuleTransition transition)
                {
                    builder.Append('-').Append(g.GetRule(transition.ruleIndex).name).Append("->").Append(GetStateString(t.target)).Append('\n');
                }
                else if (t is ActionTransition a)
                {
                    builder.Append('-').Append(a.ToString()).Append("->").Append(GetStateString(t.target)).Append('\n');
                }
                else if (t is SetTransition st)
                {
                    bool not = st is NotSetTransition;
                    if (g.IsLexer)
                    {
                        builder.Append('-').Append(not ? "~" : "").Append(st.ToString()).Append("->").Append(GetStateString(t.target)).Append('\n');
                    }
                    else
                    {
                        builder.Append('-').Append(not ? "~" : "").Append(st.Label.ToString(g.Vocabulary)).Append("->").Append(GetStateString(t.target)).Append('\n');
                    }
                }
                else if (t is AtomTransition a2)
                {
                    var label = g.GetTokenDisplayName(a2._label);
                    builder.Append('-').Append(label).Append("->").Append(GetStateString(t.target)).Append('\n');
                }
                else
                {
                    builder.Append('-').Append(t.ToString()).Append("->").Append(GetStateString(t.target)).Append('\n');
                }
            }
        }
        return builder.ToString();
    }

    public string GetStateString(ATNState s)
    {
        int n = s.stateNumber;
        return s switch
        {
            StarBlockStartState => "StarBlockStart_" + n,
            PlusBlockStartState => "PlusBlockStart_" + n,
            BlockStartState => "BlockStart_" + n,
            BlockEndState => "BlockEnd_" + n,
            RuleStartState => "RuleStart_" + g.GetRule(s.ruleIndex).name + "_" + n,
            RuleStopState => "RuleStop_" + g.GetRule(s.ruleIndex).name + "_" + n,
            PlusLoopbackState => "PlusLoopBack_" + n,
            StarLoopbackState => "StarLoopBack_" + n,
            StarLoopEntryState => "StarLoopEntry_" + n,
            _ => "s" + n,
        };
    }
}
