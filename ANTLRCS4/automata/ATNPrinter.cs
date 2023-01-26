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
public class ATNPrinter {
	List<ATNState> work;
	HashSet<ATNState> marked;
	Grammar g;
	ATNState start;

	public ATNPrinter(Grammar g, ATNState start) {
		this.g = g;
		this.start = start;
	}

	public String asString() {
		if ( start==null ) return null;
		marked = new HashSet<ATNState>();

		work = new ();
		work.Add(start);

		StringBuilder buf = new StringBuilder();
		ATNState s;

		while ( !work.isEmpty() ) {
			s = work.remove(0);
			if ( marked.contains(s) ) continue;
			int n = s.getNumberOfTransitions();
//			System.out.println("visit "+s+"; edges="+n);
			marked.Add(s);
			for (int i=0; i<n; i++) {
				Transition t = s.transition(i);
				if ( !(s is RuleStopState) ) { // don't add follow states to work
					if ( t is RuleTransition ) work.Add(((RuleTransition)t).followState);
					else work.Add( t.target );
				}
				buf.Append(getStateString(s));
				if ( t is EpsilonTransition ) {
					buf.Append("->").Append(getStateString(t.target)).Append('\n');
				}
				else if ( t is RuleTransition ) {
					buf.Append("-").Append(g.getRule(((RuleTransition)t).ruleIndex).name).append("->").Append(getStateString(t.target)).append('\n');
				}
				else if ( t is ActionTransition ) {
					ActionTransition a = (ActionTransition)t;
					buf.Append("-").Append(a.ToString()).Append("->").Append(getStateString(t.target)).Append('\n');
				}
				else if ( t is SetTransition ) {
					SetTransition st = (SetTransition)t;
					bool not = st is NotSetTransition;
					if ( g.isLexer() ) {
						buf.Append("-").Append(not?"~":"").Append(st.ToString()).Append("->").Append(getStateString(t.target)).Append('\n');
					}
					else {
						buf.Append("-").Append(not?"~":"").Append(st.label().toString(g.getVocabulary())).Append("->").Append(getStateString(t.target)).append('\n');
					}
				}
				else if ( t is AtomTransition ) {
					AtomTransition a = (AtomTransition)t;
					String label = g.getTokenDisplayName(a.label);
					buf.Append("-").Append(label).Append("->").Append(getStateString(t.target)).Append('\n');
				}
				else {
					buf.Append("-").Append(t.toString()).Append("->").Append(getStateString(t.target)).Append('\n');
				}
			}
		}
		return buf.toString();
	}

	String getStateString(ATNState s) {
		int n = s.stateNumber;
		String stateStr = "s"+n;
		if ( s is StarBlockStartState ) stateStr = "StarBlockStart_"+n;
		else if ( s is PlusBlockStartState ) stateStr = "PlusBlockStart_"+n;
		else if ( s is BlockStartState) stateStr = "BlockStart_"+n;
		else if ( s is BlockEndState ) stateStr = "BlockEnd_"+n;
		else if ( s is RuleStartState) stateStr = "RuleStart_"+g.getRule(s.ruleIndex).name+"_"+n;
		else if ( s is RuleStopState ) stateStr = "RuleStop_"+g.getRule(s.ruleIndex).name+"_"+n;
		else if ( s is PlusLoopbackState) stateStr = "PlusLoopBack_"+n;
		else if ( s is StarLoopbackState) stateStr = "StarLoopBack_"+n;
		else if ( s is StarLoopEntryState) stateStr = "StarLoopEntry_"+n;
		return stateStr;
	}
}
