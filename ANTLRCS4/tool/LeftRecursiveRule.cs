/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.analysis;
using org.antlr.v4.misc;
using org.antlr.v4.runtime.misc;
using org.antlr.v4.tool;
using org.antlr.v4.tool.ast;

namespace org.antlr.v4.tool;

public class LeftRecursiveRule : Rule {
	public List<LeftRecursiveRuleAltInfo> recPrimaryAlts;
	public OrderedHashMap<int, LeftRecursiveRuleAltInfo> recOpAlts;
	public RuleAST originalAST;

	/** Did we delete any labels on direct left-recur refs? Points at ID of ^(= ID el) */
	public List<Pair<GrammarAST,String>> leftRecursiveRuleRefLabels =
		new ();

	public LeftRecursiveRule(Grammar g, String name, RuleAST ast):base(g, name, ast, 1)
    {
		originalAST = ast;
		alt = new Alternative[numberOfAlts+1]; // always just one
		for (int i=1; i<=numberOfAlts; i++) alt[i] = new Alternative(this, i);
	}

	//@Override
	public bool hasAltSpecificContexts() {
		return base.hasAltSpecificContexts() || getAltLabels()!=null;
	}

	////@Override
	public int getOriginalNumberOfAlts() {
		int n = 0;
		if ( recPrimaryAlts!=null ) n += recPrimaryAlts.Count;
		if ( recOpAlts!=null ) n += recOpAlts.Count;
		return n;
	}

	public RuleAST getOriginalAST() {
		return originalAST;
	}

	//@Override
	public List<AltAST> getUnlabeledAltASTs() {
		List<AltAST> alts = new ();
		foreach (LeftRecursiveRuleAltInfo altInfo in recPrimaryAlts) {
			if (altInfo.altLabel == null) alts.Add(altInfo.originalAltAST);
		}
		for (int i = 0; i < recOpAlts.Count; i++) {
			LeftRecursiveRuleAltInfo altInfo = recOpAlts.getElement(i);
			if ( altInfo.altLabel==null ) alts.Add(altInfo.originalAltAST);
		}
		if ( alts.Count==0 ) return null;
		return alts;
	}

	/** Return an array that maps predicted alt from primary decision
	 *  to original alt of rule. For following rule, return [0, 2, 4]
	 *
		e : e '*' e
		  | INT
		  | e '+' e
		  | ID
		  ;

	 *  That maps predicted alt 1 to original alt 2 and predicted 2 to alt 4.
	 *
	 *  @since 4.5.1
	 */
	public int[] getPrimaryAlts() {
		if ( recPrimaryAlts.Count==0 ) return null;
		int[] alts = new int[recPrimaryAlts.Count+1];
		for (int i = 0; i < recPrimaryAlts.Count; i++) { // recPrimaryAlts is a List not Map like recOpAlts
			LeftRecursiveRuleAltInfo altInfo = recPrimaryAlts[(i)];
			alts[i+1] = altInfo.altNum;
		}
		return alts;
	}

	/** Return an array that maps predicted alt from recursive op decision
	 *  to original alt of rule. For following rule, return [0, 1, 3]
	 *
		e : e '*' e
		  | INT
		  | e '+' e
		  | ID
		  ;

	 *  That maps predicted alt 1 to original alt 1 and predicted 2 to alt 3.
	 *
	 *  @since 4.5.1
	 */
	public int[] getRecursiveOpAlts() {
		if ( recOpAlts.Count==0 ) return null;
		int[] alts = new int[recOpAlts.Count+1];
		int alt = 1;
		foreach (LeftRecursiveRuleAltInfo altInfo in recOpAlts.Values) {
			alts[alt] = altInfo.altNum;
			alt++; // recOpAlts has alts possibly with gaps
		}
		return alts;
	}

	/** Get -&gt; labels from those alts we deleted for left-recursive rules. */
	//@Override
	public Dictionary<String, List<Pair<int, AltAST>>> getAltLabels() {
        Dictionary<String, List<Pair<int, AltAST>>> labels = new ();
        Dictionary<String, List<Pair<int, AltAST>>> normalAltLabels = base.getAltLabels();
		if (normalAltLabels != null)
		{
			labels= new(normalAltLabels);
		}
		if ( recPrimaryAlts!=null ) {
			foreach (LeftRecursiveRuleAltInfo altInfo in recPrimaryAlts) {
				if (altInfo.altLabel != null) {
					if (!labels.TryGetValue(altInfo.altLabel,out var pairs)) {
						pairs = new ();
						labels[altInfo.altLabel]= pairs;
					}

					pairs.Add(new Pair<int, AltAST>(altInfo.altNum, altInfo.originalAltAST));
				}
			}
		}
		if ( recOpAlts!=null ) {
			for (int i = 0; i < recOpAlts.Count; i++) {
				LeftRecursiveRuleAltInfo altInfo = recOpAlts.getElement(i);
				if ( altInfo.altLabel!=null ) {
					if (!labels.TryGetValue(altInfo.altLabel,out var pairs)) {
						pairs = new ();
						labels.Add(altInfo.altLabel, pairs);
					}

					pairs.Add(new Pair<int, AltAST>(altInfo.altNum, altInfo.originalAltAST));
				}
			}
		}
		if ( labels.Count==0 ) return null;
		return labels;
	}
}
