/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using System.Text;

namespace org.antlr.v4.runtime.dfa;


/** A DFA walker that knows how to dump them to serialized strings. */
public class DFASerializer
{
    private readonly DFA dfa;

    private readonly Vocabulary vocabulary;

    /**
	 * @deprecated Use {@link #DFASerializer(DFA, Vocabulary)} instead.
	 */
    public DFASerializer(DFA dfa, string[] tokenNames) 
        : this(dfa, VocabularyImpl.FromTokenNames(tokenNames))
    {
    }

    public DFASerializer(DFA dfa, Vocabulary vocabulary)
    {
        this.dfa = dfa;
        this.vocabulary = vocabulary;
    }

    public override string ToString()
    {
        if (dfa.s0 == null) return null;
        var buffer = new StringBuilder();
        var states = dfa.GetStates();
        foreach (var s in states)
        {
            int n = 0;
            if (s.edges != null) n = s.edges.Length;
            for (int i = 0; i < n; i++)
            {
                var t = s.edges[i];
                if (t != null && t.stateNumber != int.MaxValue)
                {
                    buffer.Append(GetStateString(s));
                    var label = GetEdgeLabel(i);
                    buffer.Append('-').Append(label).Append("->").Append(GetStateString(t)).Append('\n');
                }
            }
        }

        var output = buffer.ToString();
        if (output.Length == 0) return null;
        //return Utils.sortLinesInString(output);
        return output;
    }

    protected virtual string GetEdgeLabel(int i) => vocabulary.GetDisplayName(i - 1);

    protected virtual string GetStateString(DFAState s)
    {
        int n = s.stateNumber;
        var baseStateStr = (s.isAcceptState ? ":" : "") + "s" + n + (s.requiresFullContext ? "^" : "");
        if (s.isAcceptState)
        {
            if (s.predicates != null)
            {
                return baseStateStr + "=>" + string.Join<DFAState.PredPrediction>(',', s.predicates);// Arrays.toString(s.predicates);
            }
            else
            {
                return baseStateStr + "=>" + s.prediction;
            }
        }
        else
        {
            return baseStateStr;
        }
    }
}
