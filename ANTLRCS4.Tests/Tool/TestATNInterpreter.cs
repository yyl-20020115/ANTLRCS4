/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.automata;
using org.antlr.v4.runtime;
using org.antlr.v4.runtime.atn;
using org.antlr.v4.runtime.dfa;
using org.antlr.v4.runtime.misc;
using org.antlr.v4.tool;

namespace org.antlr.v4.test.tool;

// NOTICE: TOKENS IN LEXER, PARSER MUST BE SAME OR TOKEN TYPE MISMATCH
// NOTICE: TOKENS IN LEXER, PARSER MUST BE SAME OR TOKEN TYPE MISMATCH
// NOTICE: TOKENS IN LEXER, PARSER MUST BE SAME OR TOKEN TYPE MISMATCH
[TestClass]
public class TestATNInterpreter
{
    [TestMethod]
    public void TestSimpleNoBlock()
    {
        var lg = new LexerGrammar(
            "lexer grammar L;\n" +
            "A : 'a' ;\n" +
            "B : 'b' ;\n" +
            "C : 'c' ;\n");
        var g = new Grammar(
            "parser grammar T;\n" +
            "a : A B ;");
        CheckMatchedAlt(lg, g, "ab", 1);
    }

    [TestMethod]
    public void TestSet()
    {
        var lg = new LexerGrammar(
            "lexer grammar L;\n" +
            "A : 'a' ;\n" +
            "B : 'b' ;\n" +
            "C : 'c' ;\n");
        var g = new Grammar(
            "parser grammar T;\n" +
            "tokens {A,B,C}\n" +
            "a : ~A ;");
        CheckMatchedAlt(lg, g, "b", 1);
    }

    [TestMethod]
    public void TestPEGAchillesHeel()
    {
        var lg = new LexerGrammar(
            "lexer grammar L;\n" +
            "A : 'a' ;\n" +
            "B : 'b' ;\n" +
            "C : 'c' ;\n");
        var g = new Grammar(
            "parser grammar T;\n" +
            "a : A | A B ;");
        CheckMatchedAlt(lg, g, "a", 1);
        CheckMatchedAlt(lg, g, "ab", 2);
        CheckMatchedAlt(lg, g, "abc", 2);
    }

    [TestMethod]
    public void TestMustTrackPreviousGoodAlt()
    {
        var lg = new LexerGrammar(
            "lexer grammar L;\n" +
            "A : 'a' ;\n" +
            "B : 'b' ;\n" +
            "C : 'c' ;\n");
        var g = new Grammar(
            "parser grammar T;\n" +
            "a : A | A B ;");

        CheckMatchedAlt(lg, g, "a", 1);
        CheckMatchedAlt(lg, g, "ab", 2);

        CheckMatchedAlt(lg, g, "ac", 1);
        CheckMatchedAlt(lg, g, "abc", 2);
    }

    [TestMethod]
    public void TestMustTrackPreviousGoodAltWithEOF()
    {
        var lg = new LexerGrammar(
            "lexer grammar L;\n" +
            "A : 'a' ;\n" +
            "B : 'b' ;\n" +
            "C : 'c' ;\n");
        var g = new Grammar(
            "parser grammar T;\n" +
            "a : (A | A B) EOF;");

        CheckMatchedAlt(lg, g, "a", 1);
        CheckMatchedAlt(lg, g, "ab", 2);

        try
        {
            CheckMatchedAlt(lg, g, "ac", 1);
            Assert.Fail();
        }
        catch (NoViableAltException re)
        {
            Assert.AreEqual(1, re.getOffendingToken().getTokenIndex());
            Assert.AreEqual(3, re.getOffendingToken().getType());
        }
    }

    [TestMethod]
    public void TestMustTrackPreviousGoodAlt2()
    {
        var lg = new LexerGrammar(
            "lexer grammar L;\n" +
            "A : 'a' ;\n" +
            "B : 'b' ;\n" +
            "C : 'c' ;\n" +
            "D : 'd' ;\n");
        var g = new Grammar(
            "parser grammar T;\n" +
            "a : A | A B | A B C ;");

        CheckMatchedAlt(lg, g, "a", 1);
        CheckMatchedAlt(lg, g, "ab", 2);
        CheckMatchedAlt(lg, g, "abc", 3);

        CheckMatchedAlt(lg, g, "ad", 1);
        CheckMatchedAlt(lg, g, "abd", 2);
        CheckMatchedAlt(lg, g, "abcd", 3);
    }

    [TestMethod]
    public void TestMustTrackPreviousGoodAlt2WithEOF()
    {
        var lg = new LexerGrammar(
            "lexer grammar L;\n" +
            "A : 'a' ;\n" +
            "B : 'b' ;\n" +
            "C : 'c' ;\n" +
            "D : 'd' ;\n");
        var g = new Grammar(
            "parser grammar T;\n" +
            "a : (A | A B | A B C) EOF;");

        CheckMatchedAlt(lg, g, "a", 1);
        CheckMatchedAlt(lg, g, "ab", 2);
        CheckMatchedAlt(lg, g, "abc", 3);

        try
        {
            CheckMatchedAlt(lg, g, "abd", 1);
            Assert.Fail();
        }
        catch (NoViableAltException re)
        {
            Assert.AreEqual(2, re.getOffendingToken().getTokenIndex());
            Assert.AreEqual(4, re.getOffendingToken().getType());
        }
    }

    [TestMethod]
    public void TestMustTrackPreviousGoodAlt3()
    {
        var lg = new LexerGrammar(
            "lexer grammar L;\n" +
            "A : 'a' ;\n" +
            "B : 'b' ;\n" +
            "C : 'c' ;\n" +
            "D : 'd' ;\n");
        var g = new Grammar(
            "parser grammar T;\n" +
            "a : A B | A | A B C ;");

        CheckMatchedAlt(lg, g, "a", 2);
        CheckMatchedAlt(lg, g, "ab", 1);
        CheckMatchedAlt(lg, g, "abc", 3);

        CheckMatchedAlt(lg, g, "ad", 2);
        CheckMatchedAlt(lg, g, "abd", 1);
        CheckMatchedAlt(lg, g, "abcd", 3);
    }

    [TestMethod]
    public void TestMustTrackPreviousGoodAlt3WithEOF()
    {
        var lg = new LexerGrammar(
            "lexer grammar L;\n" +
            "A : 'a' ;\n" +
            "B : 'b' ;\n" +
            "C : 'c' ;\n" +
            "D : 'd' ;\n");
        var g = new Grammar(
            "parser grammar T;\n" +
            "a : (A B | A | A B C) EOF;");

        CheckMatchedAlt(lg, g, "a", 2);
        CheckMatchedAlt(lg, g, "ab", 1);
        CheckMatchedAlt(lg, g, "abc", 3);

        try
        {
            CheckMatchedAlt(lg, g, "abd", 1);
            Assert.Fail();
        }
        catch (NoViableAltException re)
        {
            Assert.AreEqual(2, re.getOffendingToken().getTokenIndex());
            Assert.AreEqual(4, re.getOffendingToken().getType());
        }
    }

    [TestMethod]
    public void TestAmbigAltChooseFirst()
    {
        var lg = new LexerGrammar(
            "lexer grammar L;\n" +
            "A : 'a' ;\n" +
            "B : 'b' ;\n" +
            "C : 'c' ;\n" +
            "D : 'd' ;\n");
        var g = new Grammar(
            "parser grammar T;\n" +
            "a : A B | A B ;"); // first alt
        CheckMatchedAlt(lg, g, "ab", 1);
        CheckMatchedAlt(lg, g, "abc", 1);
    }

    [TestMethod]
    public void TestAmbigAltChooseFirstWithFollowingToken()
    {
        var lg = new LexerGrammar(
            "lexer grammar L;\n" +
            "A : 'a' ;\n" +
            "B : 'b' ;\n" +
            "C : 'c' ;\n" +
            "D : 'd' ;\n");
        var g = new Grammar(
            "parser grammar T;\n" +
            "a : (A B | A B) C ;"); // first alt
        CheckMatchedAlt(lg, g, "abc", 1);
        CheckMatchedAlt(lg, g, "abcd", 1);
    }

    [TestMethod]
    public void TestAmbigAltChooseFirstWithFollowingToken2()
    {
        var lg = new LexerGrammar(
            "lexer grammar L;\n" +
            "A : 'a' ;\n" +
            "B : 'b' ;\n" +
            "C : 'c' ;\n" +
            "D : 'd' ;\n");
        var g = new Grammar(
            "parser grammar T;\n" +
            "a : (A B | A B | C) D ;");
        CheckMatchedAlt(lg, g, "abd", 1);
        CheckMatchedAlt(lg, g, "abdc", 1);
        CheckMatchedAlt(lg, g, "cd", 3);
    }

    [TestMethod]
    public void TestAmbigAltChooseFirst2()
    {
        var lg = new LexerGrammar(
            "lexer grammar L;\n" +
            "A : 'a' ;\n" +
            "B : 'b' ;\n" +
            "C : 'c' ;\n" +
            "D : 'd' ;\n");
        var g = new Grammar(
            "parser grammar T;\n" +
            "a : A B | A B | A B C ;");

        CheckMatchedAlt(lg, g, "ab", 1);
        CheckMatchedAlt(lg, g, "abc", 3);

        CheckMatchedAlt(lg, g, "abd", 1);
        CheckMatchedAlt(lg, g, "abcd", 3);
    }

    [TestMethod]
    public void TestAmbigAltChooseFirst2WithEOF()
    {
        var lg = new LexerGrammar(
            "lexer grammar L;\n" +
            "A : 'a' ;\n" +
            "B : 'b' ;\n" +
            "C : 'c' ;\n" +
            "D : 'd' ;\n");
        var g = new Grammar(
            "parser grammar T;\n" +
            "a : (A B | A B | A B C) EOF;");

        CheckMatchedAlt(lg, g, "ab", 1);
        CheckMatchedAlt(lg, g, "abc", 3);

        try
        {
            CheckMatchedAlt(lg, g, "abd", 1);
            Assert.Fail();
        }
        catch (NoViableAltException re)
        {
            Assert.AreEqual(2, re.getOffendingToken().getTokenIndex());
            Assert.AreEqual(4, re.getOffendingToken().getType());
        }
    }

    [TestMethod]
    public void TestSimpleLoop()
    {
        var lg = new LexerGrammar(
            "lexer grammar L;\n" +
            "A : 'a' ;\n" +
            "B : 'b' ;\n" +
            "C : 'c' ;\n" +
            "D : 'd' ;\n");
        var g = new Grammar(
            "parser grammar T;\n" +
            "a : A+ B ;");
        CheckMatchedAlt(lg, g, "ab", 1);
        CheckMatchedAlt(lg, g, "aab", 1);
        CheckMatchedAlt(lg, g, "aaaaaab", 1);
        CheckMatchedAlt(lg, g, "aabd", 1);
    }

    [TestMethod]
    public void TestCommonLeftPrefix()
    {
        var lg = new LexerGrammar(
            "lexer grammar L;\n" +
            "A : 'a' ;\n" +
            "B : 'b' ;\n" +
            "C : 'c' ;\n");
        var g = new Grammar(
            "parser grammar T;\n" +
            "a : A B | A C ;");
        CheckMatchedAlt(lg, g, "ab", 1);
        CheckMatchedAlt(lg, g, "ac", 2);
    }

    [TestMethod]
    public void TestArbitraryLeftPrefix()
    {
        var lg = new LexerGrammar(
            "lexer grammar L;\n" +
            "A : 'a' ;\n" +
            "B : 'b' ;\n" +
            "C : 'c' ;\n");
        var g = new Grammar(
            "parser grammar T;\n" +
            "a : A+ B | A+ C ;");
        CheckMatchedAlt(lg, g, "aac", 2);
    }

    [TestMethod]
    public void TestRecursiveLeftPrefix()
    {
        var lg = new LexerGrammar(
            "lexer grammar L;\n" +
            "A : 'a' ;\n" +
            "B : 'b' ;\n" +
            "C : 'c' ;\n" +
            "LP : '(' ;\n" +
            "RP : ')' ;\n" +
            "INT : '0'..'9'+ ;\n"
        );
        var g = new Grammar(
            "parser grammar T;\n" +
            "tokens {A,B,C,LP,RP,INT}\n" +
            "a : e B | e C ;\n" +
            "e : LP e RP\n" +
            "  | INT\n" +
            "  ;");
        CheckMatchedAlt(lg, g, "34b", 1);
        CheckMatchedAlt(lg, g, "34c", 2);
        CheckMatchedAlt(lg, g, "(34)b", 1);
        CheckMatchedAlt(lg, g, "(34)c", 2);
        CheckMatchedAlt(lg, g, "((34))b", 1);
        CheckMatchedAlt(lg, g, "((34))c", 2);
    }

    public static void CheckMatchedAlt(LexerGrammar lg, Grammar g,
                                string inputString,
                                int expected)
    {
        var lexatn = ToolTestUtils.CreateATN(lg, true);
        var lexInterp = new LexerATNSimulator(lexatn, new DFA[] { new DFA(lexatn.modeToStartState[(Lexer.DEFAULT_MODE)]) }, null);
        var types = ToolTestUtils.GetTokenTypesViaATN(inputString, lexInterp);
        //		Console.Out.WriteLine(types);

        g.importVocab(lg);

        var f = new ParserATNFactory(g);
        var atn = f.CreateATN();

        var input = new MockIntTokenStream(types);
        //		Console.Out.WriteLine("input="+input.types);
        var interp = new ParserInterpreterForTesting(g, input);
        ATNState startState = atn.ruleToStartState[g.getRule("a").index];
        if (startState.Transition(0).target is BlockStartState)
        {
            startState = startState.Transition(0).target;
        }

        var dot = new DOTGenerator(g);
        //		Console.Out.WriteLine(dot.getDOT(atn.ruleToStartState[g.getRule("a").index]));
        var r = g.getRule("e");
        //		if ( r!=null ) Console.Out.WriteLine(dot.getDOT(atn.ruleToStartState[r.index]));

        int result = interp.MatchATN(input, startState);
        Assert.AreEqual(expected, result);
    }
}
