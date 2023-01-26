/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using System.Text;

namespace org.antlr.v4.runtime.dfa;


/** A DFA walker that knows how to dump them to serialized strings. */
public class DFASerializer {

	private readonly DFA dfa;

	private readonly Vocabulary vocabulary;

	/**
	 * @deprecated Use {@link #DFASerializer(DFA, Vocabulary)} instead.
	 */
	public DFASerializer(DFA dfa, String[] tokenNames): this(dfa, VocabularyImpl.fromTokenNames(tokenNames))
    {
	}

	public DFASerializer(DFA dfa, Vocabulary vocabulary) {
		this.dfa = dfa;
		this.vocabulary = vocabulary;
	}

	public override String ToString() {
		if ( dfa.s0==null ) return null;
		StringBuilder buf = new StringBuilder();
		List<DFAState> states = dfa.getStates();
		foreach (DFAState s in states) {
			int n = 0;
			if ( s.edges!=null ) n = s.edges.Length;
			for (int i=0; i<n; i++) {
				DFAState t = s.edges[i];
				if ( t!=null && t.stateNumber != int.MaxValue ) {
					buf.Append(getStateString(s));
					String label = getEdgeLabel(i);
					buf.Append('-').Append(label).Append("->").Append(getStateString(t)).Append('\n');
				}
			}
		}

		String output = buf.ToString();
		if ( output.Length==0 ) return null;
		//return Utils.sortLinesInString(output);
		return output;
	}

	protected virtual String getEdgeLabel(int i) {
		return vocabulary.getDisplayName(i - 1);
	}


	protected virtual String getStateString(DFAState s) {
		int n = s.stateNumber;
		String baseStateStr = (s.isAcceptState ? ":" : "") + "s" + n + (s.requiresFullContext ? "^" : "");
		if ( s.isAcceptState ) {
            if ( s.predicates!=null ) {
				return baseStateStr + "=>" + string.Join<DFAState.PredPrediction>(',', s.predicates);// Arrays.toString(s.predicates);
            }
            else {
                return baseStateStr + "=>" + s.prediction;
            }
		}
		else {
			return baseStateStr;
		}
	}
}
