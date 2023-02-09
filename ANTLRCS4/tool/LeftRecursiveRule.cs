/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.analysis;
using org.antlr.v4.misc;
using org.antlr.v4.runtime.misc;
using org.antlr.v4.tool.ast;

namespace org.antlr.v4.tool;

public class LeftRecursiveRule : Rule
{
    public List<LeftRecursiveRuleAltInfo> recPrimaryAlts;
    public OrderedHashMap<int, LeftRecursiveRuleAltInfo> recOpAlts;
    public RuleAST originalAST;

    /** Did we delete any labels on direct left-recur refs? Points at ID of ^(= ID el) */
    public List<Pair<GrammarAST, string>> leftRecursiveRuleRefLabels =
        new();

    public LeftRecursiveRule(Grammar g, string name, RuleAST ast) : base(g, name, ast, 1)
    {
        originalAST = ast;
        alt = new Alternative[numberOfAlts + 1]; // always just one
        for (int i = 1; i <= numberOfAlts; i++) alt[i] = new Alternative(this, i);
    }

    public override bool HasAltSpecificContexts() => base.HasAltSpecificContexts() || GetAltLabels() != null;

    public override int OriginalNumberOfAlts
    {
        get
        {
            int n = 0;
            if (recPrimaryAlts != null) n += recPrimaryAlts.Count;
            if (recOpAlts != null) n += recOpAlts.Count;
            return n;
        }
    }

    public RuleAST OriginalAST => originalAST;

    public override List<AltAST> GetUnlabeledAltASTs()
    {
        List<AltAST> alts = new();
        foreach (var altInfo in recPrimaryAlts)
        {
            if (altInfo.altLabel == null) alts.Add(altInfo.originalAltAST);
        }
        for (int i = 0; i < recOpAlts.Count; i++)
        {
            var altInfo = recOpAlts.GetElement(i);
            if (altInfo.altLabel == null) alts.Add(altInfo.originalAltAST);
        }
        if (alts.Count == 0) return null;
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
    public int[] GetPrimaryAlts()
    {
        if (recPrimaryAlts.Count == 0) return null;
        int[] alts = new int[recPrimaryAlts.Count + 1];
        for (int i = 0; i < recPrimaryAlts.Count; i++)
        { // recPrimaryAlts is a List not Map like recOpAlts
            var altInfo = recPrimaryAlts[(i)];
            alts[i + 1] = altInfo.altNum;
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
    public int[] GetRecursiveOpAlts()
    {
        if (recOpAlts.Count == 0) return null;
        int[] alts = new int[recOpAlts.Count + 1];
        int alt = 1;
        foreach (var altInfo in recOpAlts.Values)
        {
            alts[alt] = altInfo.altNum;
            alt++; // recOpAlts has alts possibly with gaps
        }
        return alts;
    }

    /** Get -&gt; labels from those alts we deleted for left-recursive rules. */
    public override Dictionary<string, List<Pair<int, AltAST>>> GetAltLabels()
    {
        Dictionary<string, List<Pair<int, AltAST>>> labels = new();
        var normalAltLabels = base.GetAltLabels();
        if (normalAltLabels != null)
        {
            labels = new(normalAltLabels);
        }
        if (recPrimaryAlts != null)
        {
            foreach (var altInfo in recPrimaryAlts)
            {
                if (altInfo.altLabel != null)
                {
                    if (!labels.TryGetValue(altInfo.altLabel, out var pairs))
                    {
                        pairs = new();
                        labels[altInfo.altLabel] = pairs;
                    }

                    pairs.Add(new (altInfo.altNum, altInfo.originalAltAST));
                }
            }
        }
        if (recOpAlts != null)
        {
            for (int i = 0; i < recOpAlts.Count; i++)
            {
                var altInfo = recOpAlts.GetElement(i);
                if (altInfo.altLabel != null)
                {
                    if (!labels.TryGetValue(altInfo.altLabel, out var pairs))
                    {
                        pairs = new();
                        labels.Add(altInfo.altLabel, pairs);
                    }

                    pairs.Add(new (altInfo.altNum, altInfo.originalAltAST));
                }
            }
        }
        if (labels.Count == 0) return null;
        return labels;
    }
}
