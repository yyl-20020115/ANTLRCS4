/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime;
using org.antlr.v4.runtime.atn;
using org.antlr.v4.runtime.tree;
using org.antlr.v4.tool;

namespace org.antlr.v4.test.tool;

[TestClass]
public class TestAmbigParseTrees
{
    [TestMethod]
    public void TestParseDecisionWithinAmbiguousStartRule()
    {
        var lg = new LexerGrammar(
            "lexer grammar L;\n" +
            "A : 'a' ;\n" +
            "B : 'b' ;\n" +
            "C : 'c' ;\n");
        var g = new Grammar(
            "parser grammar T;\n" +
            "s : A x C" +
            "  | A B C" +
            "  ;" +
            "x : B ; \n",
            lg);

        TestInterpAtSpecificAlt(lg, g, "s", 1, "abc", "(s:1 a (x:1 b) c)");
        TestInterpAtSpecificAlt(lg, g, "s", 2, "abc", "(s:2 a b c)");
    }

    [TestMethod]
    public void TestAmbigAltsAtRoot()
    {
        var lg = new LexerGrammar(
            "lexer grammar L;\n" +
            "A : 'a' ;\n" +
            "B : 'b' ;\n" +
            "C : 'c' ;\n");
        var g = new Grammar(
            "parser grammar T;\n" +
            "s : A x C" +
            "  | A B C" +
            "  ;" +
            "x : B ; \n",
            lg);

        var startRule = "s";
        var input = "abc";
        var expectedAmbigAlts = "{1, 2}";
        int decision = 0;
        var expectedOverallTree = "(s:1 a (x:1 b) c)";
        string[] expectedParseTrees = {"(s:1 a (x:1 b) c)",
                                       "(s:2 a b c)"};

        TestAmbiguousTrees(lg, g, startRule, input, decision,
                           expectedAmbigAlts,
                           expectedOverallTree, expectedParseTrees);
    }

    [TestMethod]
    public void TestAmbigAltsNotAtRoot()
    {
        var lg = new LexerGrammar(
            "lexer grammar L;\n" +
            "A : 'a' ;\n" +
            "B : 'b' ;\n" +
            "C : 'c' ;\n");
        var g = new Grammar(
            "parser grammar T;\n" +
            "s : x ;" +
            "x : y ;" +
            "y : A z C" +
            "  | A B C" +
            "  ;" +
            "z : B ; \n",
            lg);

        var startRule = "s";
        var input = "abc";
        var expectedAmbigAlts = "{1, 2}";
        int decision = 0;
        var expectedOverallTree = "(s:1 (x:1 (y:1 a (z:1 b) c)))";
        string[] expectedParseTrees = {"(y:1 a (z:1 b) c)",
                                       "(y:2 a b c)"};

        TestAmbiguousTrees(lg, g, startRule, input, decision,
                           expectedAmbigAlts,
                           expectedOverallTree, expectedParseTrees);
    }

    [TestMethod]
    public void TestAmbigAltDipsIntoOuterContextToRoot()
    {
        var lg = new LexerGrammar(
            "lexer grammar L;\n" +
            "SELF : 'self' ;\n" +
            "ID : [a-z]+ ;\n" +
            "DOT : '.' ;\n");
        var g = new Grammar(
            "parser grammar T;\n" +
            "e : p (DOT ID)* ;\n" +
            "p : SELF" +
            "  | SELF DOT ID" +
            "  ;",
            lg);

        var startRule = "e";
        var input = "self.x";
        var expectedAmbigAlts = "{1, 2}";
        int decision = 1; // decision in p
        var expectedOverallTree = "(e:1 (p:1 self) . x)";
        string[] expectedParseTrees = {"(e:1 (p:1 self) . x)",
                                       "(p:2 self . x)"};

        TestAmbiguousTrees(lg, g, startRule, input, decision,
                           expectedAmbigAlts,
                           expectedOverallTree, expectedParseTrees);
    }

    [TestMethod]
    public void TestAmbigAltDipsIntoOuterContextBelowRoot()
    {
        var lg = new LexerGrammar(
            "lexer grammar L;\n" +
            "SELF : 'self' ;\n" +
            "ID : [a-z]+ ;\n" +
            "DOT : '.' ;\n");
        var g = new Grammar(
            "parser grammar T;\n" +
            "s : e ;\n" +
            "e : p (DOT ID)* ;\n" +
            "p : SELF" +
            "  | SELF DOT ID" +
            "  ;",
            lg);

        var startRule = "s";
        var input = "self.x";
        var expectedAmbigAlts = "{1, 2}";
        int decision = 1; // decision in p
        var expectedOverallTree = "(s:1 (e:1 (p:1 self) . x))";
        string[] expectedParseTrees = {"(e:1 (p:1 self) . x)", // shouldn't include s
									   "(p:2 self . x)"};      // shouldn't include e

        TestAmbiguousTrees(lg, g, startRule, input, decision,
                           expectedAmbigAlts,
                           expectedOverallTree, expectedParseTrees);
    }

    [TestMethod]
    public void TestAmbigAltInLeftRecursiveBelowStartRule()
    {
        var lg = new LexerGrammar(
            "lexer grammar L;\n" +
            "SELF : 'self' ;\n" +
            "ID : [a-z]+ ;\n" +
            "DOT : '.' ;\n");
        var g = new Grammar(
            "parser grammar T;\n" +
            "s : e ;\n" +
            "e : p | e DOT ID ;\n" +
            "p : SELF" +
            "  | SELF DOT ID" +
            "  ;",
            lg);

        var startRule = "s";
        var input = "self.x";
        var expectedAmbigAlts = "{1, 2}";
        int decision = 1; // decision in p
        var expectedOverallTree = "(s:1 (e:2 (e:1 (p:1 self)) . x))";
        string[] expectedParseTrees = {"(e:2 (e:1 (p:1 self)) . x)",
                                       "(p:2 self . x)"};

        TestAmbiguousTrees(lg, g, startRule, input, decision,
                           expectedAmbigAlts,
                           expectedOverallTree, expectedParseTrees);
    }

    [TestMethod]
    public void TestAmbigAltInLeftRecursiveStartRule()
    {
        var lg = new LexerGrammar(
            "lexer grammar L;\n" +
            "SELF : 'self' ;\n" +
            "ID : [a-z]+ ;\n" +
            "DOT : '.' ;\n");
        var g = new Grammar(
            "parser grammar T;\n" +
            "e : p | e DOT ID ;\n" +
            "p : SELF" +
            "  | SELF DOT ID" +
            "  ;",
            lg);

        var startRule = "e";
        var input = "self.x";
        var expectedAmbigAlts = "{1, 2}";
        int decision = 1; // decision in p
        var expectedOverallTree = "(e:2 (e:1 (p:1 self)) . x)";
        string[] expectedParseTrees = {"(e:2 (e:1 (p:1 self)) . x)",
                                       "(p:2 self . x)"}; // shows just enough for self.x

        TestAmbiguousTrees(lg, g, startRule, input, decision,
                           expectedAmbigAlts,
                           expectedOverallTree, expectedParseTrees);
    }

    public static void TestAmbiguousTrees(LexerGrammar lg, Grammar g,
                                   string startRule, string input, int decision,
                                   string expectedAmbigAlts,
                                   string overallTree,
                                   string[] expectedParseTrees)
    {
        var nodeTextProvider = new InterpreterTreeTextProvider(g.getRuleNames());

        var lexEngine = lg.createLexerInterpreter(new ANTLRInputStream(input));
        var tokens = new CommonTokenStream(lexEngine);
        var parser = g.createGrammarParserInterpreter(tokens);
        parser.SetProfile(true);
        parser.GetInterpreter().        PredictionMode = PredictionMode.LL_EXACT_AMBIG_DETECTION;

        // PARSE
        int ruleIndex = g.rules[(startRule)].index;
        var parseTree = parser.parse(ruleIndex);
        Assert.AreEqual(overallTree, Trees.ToStringTree(parseTree, nodeTextProvider));
        Console.Out.WriteLine();

        var decisionInfo = parser.GetParseInfo().GetDecisionInfo();
        var ambiguities = decisionInfo[decision].ambiguities;
        Assert.AreEqual(1, ambiguities.Count);
        var ambiguityInfo = ambiguities[(0)];

        var ambiguousParseTrees =
            GrammarParserInterpreter.getAllPossibleParseTrees(g,
                                                              parser,
                                                              tokens,
                                                              decision,
                                                              ambiguityInfo.ambigAlts,
                                                              ambiguityInfo.startIndex,
                                                              ambiguityInfo.stopIndex,
                                                              ruleIndex);
        Assert.AreEqual(expectedAmbigAlts, ambiguityInfo.ambigAlts.ToString());
        Assert.AreEqual(ambiguityInfo.ambigAlts.Cardinality(), ambiguousParseTrees.Count);

        for (int i = 0; i < ambiguousParseTrees.Count; i++)
        {
            ParserRuleContext t = ambiguousParseTrees[i];
            Assert.AreEqual(expectedParseTrees[i], Trees.ToStringTree(t, nodeTextProvider));
        }
    }

    static void TestInterpAtSpecificAlt(LexerGrammar lg, Grammar g,
                                 string startRule, int startAlt,
                                 string input,
                                 string expectedParseTree)
    {
        var lexEngine = lg.createLexerInterpreter(new ANTLRInputStream(input));
        var tokens = new CommonTokenStream(lexEngine);
        var parser = g.createGrammarParserInterpreter(tokens);
        var ruleStartState = g.atn.ruleToStartState[g.getRule(startRule).index];
        var tr = ruleStartState.Transition(0);
        var t2 = tr.target;
        if (t2 is not BasicBlockStartState)
        {
            throw new ArgumentException("rule has no decision: " + startRule);
        }
        parser.addDecisionOverride(((DecisionState)t2).decision, 0, startAlt);
        var t = parser.parse(g.rules[(startRule)].index);
        var nodeTextProvider = new InterpreterTreeTextProvider(g.getRuleNames());
        Assert.AreEqual(expectedParseTree, Trees.ToStringTree(t, nodeTextProvider));
    }
}
