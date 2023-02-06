/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime.atn;
using org.antlr.v4.runtime.misc;
using org.antlr.v4.tool;

namespace org.antlr.v4.test.tool;

[TestClass]
public class TestATNDeserialization
{
    [TestMethod]
    public void TestSimpleNoBlock()
    {
        var g = new Grammar(
            "parser grammar T;\n" +
            "a : A B ;");
        CheckDeserializationIsStable(g);
    }

    [TestMethod]
    public void TestEOF()
    {
        var g = new Grammar(
            "parser grammar T;\n" +
            "a : EOF ;");
        CheckDeserializationIsStable(g);
    }

    [TestMethod]
    public void TestEOFInSet()
    {
        var g = new Grammar(
            "parser grammar T;\n" +
            "a : (EOF|A) ;");
        CheckDeserializationIsStable(g);
    }

    [TestMethod]
    public void TestNot()
    {
        var g = new Grammar(
            "parser grammar T;\n" +
            "tokens {A, B, C}\n" +
            "a : ~A ;");
        CheckDeserializationIsStable(g);
    }

    [TestMethod]
    public void TestWildcard()
    {
        var g = new Grammar(
            "parser grammar T;\n" +
            "tokens {A, B, C}\n" +
            "a : . ;");
        CheckDeserializationIsStable(g);
    }

    [TestMethod]
    public void TestPEGAchillesHeel()
    {
        var g = new Grammar(
            "parser grammar T;\n" +
            "a : A | A B ;");
        CheckDeserializationIsStable(g);
    }

    [TestMethod]
    public void Test3Alts()
    {
        var g = new Grammar(
            "parser grammar T;\n" +
            "a : A | A B | A B C ;");
        CheckDeserializationIsStable(g);
    }

    [TestMethod]
    public void TestSimpleLoop()
    {
        var g = new Grammar(
            "parser grammar T;\n" +
            "a : A+ B ;");
        CheckDeserializationIsStable(g);
    }

    [TestMethod]
    public void TestRuleRef()
    {
        var g = new Grammar(
            "parser grammar T;\n" +
            "a : e ;\n" +
            "e : E ;\n");
        CheckDeserializationIsStable(g);
    }

    [TestMethod]
    public void TestLexerTwoRules()
    {
        var lg = new LexerGrammar(
            "lexer grammar L;\n" +
            "A : 'a' ;\n" +
            "B : 'b' ;\n");
        CheckDeserializationIsStable(lg);
    }

    [TestMethod]
    public void TestLexerEOF()
    {
        var lg = new LexerGrammar(
            "lexer grammar L;\n" +
            "A : 'a' EOF ;\n");
        CheckDeserializationIsStable(lg);
    }

    [TestMethod]
    public void TestLexerEOFInSet()
    {
        var lg = new LexerGrammar(
            "lexer grammar L;\n" +
            "A : 'a' (EOF|'\\n') ;\n");
        CheckDeserializationIsStable(lg);
    }

    [TestMethod]
    public void TestLexerRange()
    {
        var lg = new LexerGrammar(
            "lexer grammar L;\n" +
            "INT : '0'..'9' ;\n");
        CheckDeserializationIsStable(lg);
    }

    [TestMethod]
    public void TestLexerLoops()
    {
        var lg = new LexerGrammar(
            "lexer grammar L;\n" +
            "INT : '0'..'9'+ ;\n");
        CheckDeserializationIsStable(lg);
    }

    [TestMethod]
    public void TestLexerNotSet()
    {
        var lg = new LexerGrammar(
            "lexer grammar L;\n" +
            "ID : ~('a'|'b')\n ;");
        CheckDeserializationIsStable(lg);
    }

    [TestMethod]
    public void TestLexerNotSetWithRange()
    {
        var lg = new LexerGrammar(
            "lexer grammar L;\n" +
            "ID : ~('a'|'b'|'e'|'p'..'t')\n ;");
        CheckDeserializationIsStable(lg);
    }

    [TestMethod]
    public void TestLexerNotSetWithRange2()
    {
        var lg = new LexerGrammar(
            "lexer grammar L;\n" +
            "ID : ~('a'|'b') ~('e'|'p'..'t')\n ;");
        CheckDeserializationIsStable(lg);
    }

    [TestMethod]
    public void Test2ModesInLexer()
    {
        var lg = new LexerGrammar(
                "lexer grammar L;\n" +
                        "A : 'a'\n ;\n" +
                        "mode M;\n" +
                        "B : 'b';\n" +
                        "mode M2;\n" +
                        "C : 'c';\n");
        CheckDeserializationIsStable(lg);
    }

    [TestMethod]
    public void TestLastValidBMPCharInSet()
    {
        var lg = new LexerGrammar(
                "lexer grammar L;\n" +
                        "ID : 'Ā'..'\\uFFFC'; // FFFD+ are not valid char\n");
        CheckDeserializationIsStable(lg);
    }

    protected static void CheckDeserializationIsStable(Grammar g)
    {
        var atn = ToolTestUtils.CreateATN(g, false);
        var serialized = ATNSerializer.getSerialized(atn);
        var atnData = new ATNDescriber(atn, Arrays.AsList(g.getTokenNames())).Decode(serialized.toArray());

        var serialized16 = ATNDeserializer.encodeIntsWith16BitWords(serialized);
        int[] ints16 = serialized16.toArray();
        char[] chars = new char[ints16.Length];
        for (int i = 0; i < ints16.Length; i++)
        {
            chars[i] = (char)ints16[i];
        }
        int[] serialized32 = ATNDeserializer.decodeIntsEncodedAs16BitWords(chars, true);

        Assert.IsTrue(Enumerable.SequenceEqual(serialized.toArray(), serialized32));

        var atn2 = new ATNDeserializer().deserialize(serialized.toArray());
        var serialized1 = ATNSerializer.getSerialized(atn2);
        var atn2Data = new ATNDescriber(atn2, Arrays.AsList(g.getTokenNames())).Decode(serialized1.toArray());

        Assert.AreEqual(atnData, atn2Data);
    }
}
