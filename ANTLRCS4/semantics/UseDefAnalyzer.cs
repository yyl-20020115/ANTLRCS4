/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.runtime;
using org.antlr.v4.parse;
using org.antlr.v4.runtime;
using org.antlr.v4.tool;
using org.antlr.v4.tool.ast;

namespace org.antlr.v4.semantics;
/** Look for errors and deadcode stuff */
public class UseDefAnalyzer
{
    // side-effect: updates Alternative with refs in actions
    public static void trackTokenRuleRefsInActions(Grammar g)
    {
        foreach (Rule r in g.rules.Values)
        {
            for (int i = 1; i <= r.numberOfAlts; i++)
            {
                Alternative alt = r.alt[i];
                foreach (ActionAST a in alt.actions)
                {
                    ActionSniffer sniffer = new ActionSniffer(g, r, alt, a, a.token);
                    sniffer.ExamineAction();
                }
            }
        }
    }

    public class BASListener : BlankActionSplitterListener
    {
        //@Override
        public void nonLocalAttr(string expr, Token x, Token y) { dependent[0] = true; }
        //@Override
        public void qualifiedAttr(string expr, Token x, Token y) { dependent[0] = true; }
        //@Override
        public void setAttr(string expr, Token x, Token rhs) { dependent[0] = true; }
        //@Override
        public void setExprAttribute(string expr) { dependent[0] = true; }
        //@Override
        public void setNonLocalAttr(string expr, Token x, Token y, Token rhs) { dependent[0] = true; }
        //@Override
        public void attr(string expr, Token x) { dependent[0] = true; }
    }
    static bool[] dependent = new bool[] { false };
    public static bool actionIsContextDependent(ActionAST actionAST)
    {
        ANTLRStringStream @in = new ANTLRStringStream(actionAST.token.Text);
        @in.SetLine(actionAST.token.Line);
        @in.SetCharPositionInLine(actionAST.token.CharPositionInLine);
        // can't be simple bool with anon class
        ActionSplitterListener listener = new BASListener();
        ActionSplitter splitter = new ActionSplitter(@in, listener);
        // forces eval, triggers listener methods
        splitter.GetActionTokens();
        return dependent[0];
    }

    /** Find all rules reachable from r directly or indirectly for all r in g */
    public static Dictionary<Rule, HashSet<Rule>> getRuleDependencies(Grammar g)
    {
        return getRuleDependencies(g, g.rules.Values);
    }

    public static Dictionary<Rule, HashSet<Rule>> getRuleDependencies(LexerGrammar g, String modeName)
    {
        return getRuleDependencies(g, g.modes.TryGetValue(modeName, out var r) ? r : new());
    }

    public static Dictionary<Rule, HashSet<Rule>> getRuleDependencies(Grammar g, ICollection<Rule> rules)
    {
        Dictionary<Rule, HashSet<Rule>> dependencies = new Dictionary<Rule, HashSet<Rule>>();

        foreach (Rule r in rules)
        {
            List<GrammarAST> tokenRefs = r.ast.getNodesWithType(ANTLRParser.TOKEN_REF);
            foreach (GrammarAST tref in tokenRefs)
            {
                if (!dependencies.TryGetValue(r, out var calls))
                {
                    calls = new HashSet<Rule>();
                    dependencies[r] = calls;
                }
                calls.Add(g.GetRule(tref.getText()));
            }
        }

        return dependencies;
    }

}
