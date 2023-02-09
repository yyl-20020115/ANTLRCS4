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
    public static void TrackTokenRuleRefsInActions(Grammar g)
    {
        foreach (var r in g.rules.Values)
        {
            for (int i = 1; i <= r.numberOfAlts; i++)
            {
                var alt = r.alt[i];
                foreach (var a in alt.actions)
                {
                    var sniffer = new ActionSniffer(g, r, alt, a, a.token);
                    sniffer.ExamineAction();
                }
            }
        }
    }

    public class BASListener : BlankActionSplitterListener
    {
        //@Override
        public override void NonLocalAttr(string expr, Token x, Token y) { dependent[0] = true; }
        //@Override
        public override void QualifiedAttr(string expr, Token x, Token y) { dependent[0] = true; }
        //@Override
        public override void SetAttr(string expr, Token x, Token rhs) { dependent[0] = true; }
        //@Override
        public override void SetExprAttribute(string expr) { dependent[0] = true; }
        //@Override
        public override void SetNonLocalAttr(string expr, Token x, Token y, Token rhs) { dependent[0] = true; }
        //@Override
        public override void Attr(string expr, Token x) { dependent[0] = true; }
    }
    static readonly bool[] dependent = new bool[] { false };
    public static bool ActionIsContextDependent(ActionAST actionAST)
    {
        var @in = new ANTLRStringStream(actionAST.token.Text);
        @in.SetLine(actionAST.token.Line);
        @in.SetCharPositionInLine(actionAST.token.CharPositionInLine);
        // can't be simple bool with anon class
        var listener = new BASListener();
        var splitter = new ActionSplitter(@in, listener);
        // forces eval, triggers listener methods
        splitter.GetActionTokens();
        return dependent[0];
    }

    /** Find all rules reachable from r directly or indirectly for all r in g */
    public static Dictionary<Rule, HashSet<Rule>> GetRuleDependencies(Grammar g) => GetRuleDependencies(g, g.rules.Values);

    public static Dictionary<Rule, HashSet<Rule>> GetRuleDependencies(LexerGrammar g, String modeName) => GetRuleDependencies(g, g.modes.TryGetValue(modeName, out var r) ? r : new());

    public static Dictionary<Rule, HashSet<Rule>> GetRuleDependencies(Grammar g, ICollection<Rule> rules)
    {
        Dictionary<Rule, HashSet<Rule>> dependencies = new();

        foreach (var r in rules)
        {
            var tokenRefs = r.ast.getNodesWithType(ANTLRParser.TOKEN_REF);
            foreach (var tref in tokenRefs)
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
