/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime;
using org.antlr.v4.runtime.tree;
using org.antlr.v4.tool;

namespace org.antlr.v4.test.tool;

[TestClass]
public class TestParserInterpreter
{
    [TestMethod]
    public void TestEmptyStartRule()
    {
        var lg = new LexerGrammar(
            "lexer grammar L;\n" +
            "A : 'a' ;\n");
        var g = new Grammar(
            "parser grammar T;\n" +
            "s :  ;",
            lg);

        TestInterp(lg, g, "s", "", "s");
        TestInterp(lg, g, "s", "a", "s");
    }

    [TestMethod]
    public void TestA()
    {
        var lg = new LexerGrammar(
            "lexer grammar L;\n" +
            "A : 'a' ;\n");
        var g = new Grammar(
            "parser grammar T;\n" +
            "s : A ;",
            lg);

        var t = TestInterp(lg, g, "s", "a", "(s a)");
        Assert.AreEqual("0..0", t.SourceInterval.ToString());
    }

    [TestMethod]
    public void TestEOF()
    {
        var lg = new LexerGrammar(
            "lexer grammar L;\n" +
            "A : 'a' ;\n");
        var g = new Grammar(
            "parser grammar T;\n" +
            "s : A EOF ;",
            lg);

        ParseTree t = TestInterp(lg, g, "s", "a", "(s a <EOF>)");
        Assert.AreEqual("0..1", t.SourceInterval.ToString());
    }

    [TestMethod]
    public void TestEOFInChild()
    {
        var lg = new LexerGrammar(
            "lexer grammar L;\n" +
            "A : 'a' ;\n");
        var g = new Grammar(
            "parser grammar T;\n" +
            "s : x ;\n" +
            "x : A EOF ;",
            lg);

        var t = TestInterp(lg, g, "s", "a", "(s (x a <EOF>))");
        Assert.AreEqual("0..1", t.SourceInterval.ToString());
        Assert.AreEqual("0..1", t.GetChild(0).SourceInterval.ToString());
    }

    [TestMethod]
    public void TestEmptyRuleAfterEOFInChild()
    {
        var lg = new LexerGrammar(
            "lexer grammar L;\n" +
            "A : 'a' ;\n");
        var g = new Grammar(
            "parser grammar T;\n" +
            "s : x y;\n" +
            "x : A EOF ;\n" +
            "y : ;",
            lg);

        var t = TestInterp(lg, g, "s", "a", "(s (x a <EOF>) y)");
        Assert.AreEqual("0..1", t.SourceInterval.ToString()); // s
        Assert.AreEqual("0..1", t.GetChild(0).SourceInterval.ToString()); // x
                                                                               // unspecified		Assert.AreEqual("1..0", t.getChild(1).getSourceInterval().ToString()); // y
    }

    [TestMethod]
    public void TestEmptyRuleAfterJustEOFInChild()
    {
        var lg = new LexerGrammar(
            "lexer grammar L;\n" +
            "A : 'a' ;\n");
        var g = new Grammar(
            "parser grammar T;\n" +
            "s : x y;\n" +
            "x : EOF ;\n" +
            "y : ;",
            lg);

        var t = TestInterp(lg, g, "s", "", "(s (x <EOF>) y)");
        Assert.AreEqual("0..0", t.SourceInterval.ToString()); // s
        Assert.AreEqual("0..0", t.GetChild(0).SourceInterval.ToString()); // x
                                                                               // this next one is a weird special case where somebody tries to match beyond in the file
                                                                               // unspecified		Assert.AreEqual("0..-1", t.getChild(1).getSourceInterval().ToString()); // y
    }

    [TestMethod]
    public void TestEmptyInput()
    {
        var lg = new LexerGrammar(
            "lexer grammar L;\n" +
            "A : 'a' ;\n");
        var g = new Grammar(
            "parser grammar T;\n" +
            "s : x EOF ;\n" +
            "x : ;\n",
            lg);

        var t = TestInterp(lg, g, "s", "", "(s x <EOF>)");
        Assert.AreEqual("0..0", t.SourceInterval.ToString()); // s
        Assert.AreEqual("0..-1", t.GetChild(0).SourceInterval.ToString()); // x
    }

    [TestMethod]
    public void TestEmptyInputWithCallsAfter()
    {
        var lg = new LexerGrammar(
            "lexer grammar L;\n" +
            "A : 'a' ;\n");
        var g = new Grammar(
            "parser grammar T;\n" +
            "s : x y ;\n" +
            "x : EOF ;\n" +
            "y : z ;\n" +
            "z : ;",
            lg);

        var t = TestInterp(lg, g, "s", "", "(s (x <EOF>) (y z))");
        Assert.AreEqual("0..0", t.SourceInterval.ToString()); // s
        Assert.AreEqual("0..0", t.GetChild(0).SourceInterval.ToString()); // x
                                                                               // unspecified		Assert.AreEqual("0..-1", t.getChild(1).getSourceInterval().ToString()); // x
    }

    [TestMethod]
    public void TestEmptyFirstRule()
    {
        var lg = new LexerGrammar(
            "lexer grammar L;\n" +
            "A : 'a' ;\n");
        var g = new Grammar(
            "parser grammar T;\n" +
            "s : x A ;\n" +
            "x : ;\n",
            lg);

        var t = TestInterp(lg, g, "s", "a", "(s x a)");
        Assert.AreEqual("0..0", t.SourceInterval.ToString()); // s
                                                                   // This gets an empty interval because the stop token is null for x
        Assert.AreEqual("0..-1", t.GetChild(0).SourceInterval.ToString()); // x
    }

    [TestMethod]
    public void TestAorB()
    {
        var lg = new LexerGrammar(
            "lexer grammar L;\n" +
            "A : 'a' ;\n" +
            "B : 'b' ;\n" +
            "C : 'c' ;\n");
        var g = new Grammar(
            "parser grammar T;\n" +
            "s : A{;} | B ;",
            lg);
        TestInterp(lg, g, "s", "a", "(s a)");
        TestInterp(lg, g, "s", "b", "(s b)");
    }

    [TestMethod]
    public void TestCall()
    {
        var lg = new LexerGrammar(
            "lexer grammar L;\n" +
            "A : 'a' ;\n" +
            "B : 'b' ;\n" +
            "C : 'c' ;\n");
        var g = new Grammar(
            "parser grammar T;\n" +
            "s : t C ;\n" +
            "t : A{;} | B ;\n",
            lg);

        TestInterp(lg, g, "s", "ac", "(s (t a) c)");
        TestInterp(lg, g, "s", "bc", "(s (t b) c)");
    }

    [TestMethod]
    public void TestCall2()
    {
        var lg = new LexerGrammar(
            "lexer grammar L;\n" +
            "A : 'a' ;\n" +
            "B : 'b' ;\n" +
            "C : 'c' ;\n");
        var g = new Grammar(
            "parser grammar T;\n" +
            "s : t C ;\n" +
            "t : u ;\n" +
            "u : A{;} | B ;\n",
            lg);

        TestInterp(lg, g, "s", "ac", "(s (t (u a)) c)");
        TestInterp(lg, g, "s", "bc", "(s (t (u b)) c)");
    }

    [TestMethod]
    public void TestOptionalA()
    {
        var lg = new LexerGrammar(
            "lexer grammar L;\n" +
            "A : 'a' ;\n" +
            "B : 'b' ;\n" +
            "C : 'c' ;\n");
        var g = new Grammar(
            "parser grammar T;\n" +
            "s : A? B ;\n",
            lg);

        TestInterp(lg, g, "s", "b", "(s b)");
        TestInterp(lg, g, "s", "ab", "(s a b)");
    }

    [TestMethod]
    public void TestOptionalAorB()
    {
        var lg = new LexerGrammar(
            "lexer grammar L;\n" +
            "A : 'a' ;\n" +
            "B : 'b' ;\n" +
            "C : 'c' ;\n");
        var g = new Grammar(
            "parser grammar T;\n" +
            "s : (A{;}|B)? C ;\n",
            lg);

        TestInterp(lg, g, "s", "c", "(s c)");
        TestInterp(lg, g, "s", "ac", "(s a c)");
        TestInterp(lg, g, "s", "bc", "(s b c)");
    }

    [TestMethod]
    public void TestStarA()
    {
        var lg = new LexerGrammar(
            "lexer grammar L;\n" +
            "A : 'a' ;\n" +
            "B : 'b' ;\n" +
            "C : 'c' ;\n");
        var g = new Grammar(
            "parser grammar T;\n" +
            "s : A* B ;\n",
            lg);

        TestInterp(lg, g, "s", "b", "(s b)");
        TestInterp(lg, g, "s", "ab", "(s a b)");
        TestInterp(lg, g, "s", "aaaaaab", "(s a a a a a a b)");
    }

    [TestMethod]
    public void TestStarAorB()
    {
        var lg = new LexerGrammar(
            "lexer grammar L;\n" +
            "A : 'a' ;\n" +
            "B : 'b' ;\n" +
            "C : 'c' ;\n");
        var g = new Grammar(
            "parser grammar T;\n" +
            "s : (A{;}|B)* C ;\n",
            lg);

        TestInterp(lg, g, "s", "c", "(s c)");
        TestInterp(lg, g, "s", "ac", "(s a c)");
        TestInterp(lg, g, "s", "bc", "(s b c)");
        TestInterp(lg, g, "s", "abaaabc", "(s a b a a a b c)");
        TestInterp(lg, g, "s", "babac", "(s b a b a c)");
    }

    [TestMethod]
    public void TestLeftRecursion()
    {
        var lg = new LexerGrammar(
            "lexer grammar L;\n" +
            "A : 'a' ;\n" +
            "B : 'b' ;\n" +
            "C : 'c' ;\n" +
            "PLUS : '+' ;\n" +
            "MULT : '*' ;\n");
        var g = new Grammar(
            "parser grammar T;\n" +
            "s : e ;\n" +
            "e : e MULT e\n" +
            "  | e PLUS e\n" +
            "  | A\n" +
            "  ;\n",
            lg);

        TestInterp(lg, g, "s", "a", "(s (e a))");
        TestInterp(lg, g, "s", "a+a", "(s (e (e a) + (e a)))");
        TestInterp(lg, g, "s", "a*a", "(s (e (e a) * (e a)))");
        TestInterp(lg, g, "s", "a+a+a", "(s (e (e (e a) + (e a)) + (e a)))");
        TestInterp(lg, g, "s", "a*a+a", "(s (e (e (e a) * (e a)) + (e a)))");
        TestInterp(lg, g, "s", "a+a*a", "(s (e (e a) + (e (e a) * (e a))))");
    }

    /**
	 * This is a regression test for antlr/antlr4#461.
	 * https://github.com/antlr/antlr4/issues/461
	 */
    [TestMethod]
    public void TestLeftRecursiveStartRule()
    {
        var lg = new LexerGrammar(
            "lexer grammar L;\n" +
            "A : 'a' ;\n" +
            "B : 'b' ;\n" +
            "C : 'c' ;\n" +
            "PLUS : '+' ;\n" +
            "MULT : '*' ;\n");
        var g = new Grammar(
            "parser grammar T;\n" +
            "s : e ;\n" +
            "e : e MULT e\n" +
            "  | e PLUS e\n" +
            "  | A\n" +
            "  ;\n",
            lg);

        TestInterp(lg, g, "e", "a", "(e a)");
        TestInterp(lg, g, "e", "a+a", "(e (e a) + (e a))");
        TestInterp(lg, g, "e", "a*a", "(e (e a) * (e a))");
        TestInterp(lg, g, "e", "a+a+a", "(e (e (e a) + (e a)) + (e a))");
        TestInterp(lg, g, "e", "a*a+a", "(e (e (e a) * (e a)) + (e a))");
        TestInterp(lg, g, "e", "a+a*a", "(e (e a) + (e (e a) * (e a)))");
    }

    [TestMethod]
    public void TestCaseInsensitiveTokensInParser()
    {
        var lg = new LexerGrammar(
                "lexer grammar L;\n" +
                "options { caseInsensitive = true; }\n" +
                "NOT: 'not';\n" +
                "AND: 'and';\n" +
                "NEW: 'new';\n" +
                "LB:  '(';\n" +
                "RB:  ')';\n" +
                "ID: [a-z_][a-z_0-9]*;\n" +
                "WS: [ \\t\\n\\r]+ -> skip;");
        var g = new Grammar(
                "parser grammar T;\n" +
                "options { caseInsensitive = true; }\n" +
                "e\n" +
                "    : ID\n" +
                "    | 'not' e\n" +
                "    | e 'and' e\n" +
                "    | 'new' ID '(' e ')'\n" +
                "    ;", lg);

        TestInterp(lg, g, "e", "NEW Abc (Not a AND not B)", "(e NEW Abc ( (e (e Not (e a)) AND (e not (e B))) ))");
    }

    static ParseTree TestInterp(LexerGrammar lg, Grammar g,
                    string startRule, string input,
                    string expectedParseTree)
    {
        var lexEngine = lg.createLexerInterpreter(new ANTLRInputStream(input));
        var tokens = new CommonTokenStream(lexEngine);
        var parser = g.createParserInterpreter(tokens);
        var t = parser.parse(g.rules[(startRule)].index);
        Assert.AreEqual(expectedParseTree, t.ToStringTree(parser));
        return t;
    }
}
