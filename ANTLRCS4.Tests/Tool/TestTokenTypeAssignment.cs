/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime;
using org.antlr.v4.tool;

namespace org.antlr.v4.test.tool;

[TestClass]
public class TestTokenTypeAssignment
{
    [TestMethod]
    public void TestParserSimpleTokens()
    {
        var g = new Grammar(
                "parser grammar t;\n" +
                "a : A | B;\n" +
                "b : C ;");
        var rules = "a, b";
        var tokenNames = "A, B, C";
        CheckSymbols(g, rules, tokenNames);
    }

    [TestMethod]
    public void TestParserTokensSection()
    {
        var g = new Grammar(
                "parser grammar t;\n" +
                "tokens {\n" +
                "  C,\n" +
                "  D" +
                "}\n" +
                "a : A | B;\n" +
                "b : C ;");
        var rules = "a, b";
        var tokenNames = "A, B, C, D";
        CheckSymbols(g, rules, tokenNames);
    }

    [TestMethod]
    public void TestLexerTokensSection()
    {
        var g = new LexerGrammar(
                "lexer grammar t;\n" +
                "tokens {\n" +
                "  C,\n" +
                "  D" +
                "}\n" +
                "A : 'a';\n" +
                "C : 'c' ;");
        var rules = "A, C";
        var tokenNames = "A, C, D";
        CheckSymbols(g, rules, tokenNames);
    }

    [TestMethod]
    public void TestCombinedGrammarLiterals()
    {
        var g = new Grammar(
                "grammar t;\n" +
                "a : 'begin' b 'end';\n" +
                "b : C ';' ;\n" +
                "ID : 'a' ;\n" +
                "FOO : 'foo' ;\n" +  // "foo" is not a token name
                "C : 'c' ;\n");        // nor is 'c'
        var rules = "a, b";
        var tokenNames = "C, FOO, ID, 'begin', 'end', ';'";
        CheckSymbols(g, rules, tokenNames);
    }

    [TestMethod]
    public void TestLiteralInParserAndLexer()
    {
        // 'x' is token and char in lexer rule
        var g = new Grammar(
                "grammar t;\n" +
                "a : 'x' E ; \n" +
                "E: 'x' '0' ;\n");

        var literals = "['x']";
        var foundLiterals = g.stringLiteralToTypeMap.Keys.ToString();
        Assert.AreEqual(literals, foundLiterals);

        foundLiterals = g.implicitLexer.stringLiteralToTypeMap.Keys.ToString();
        Assert.AreEqual("['x']", foundLiterals); // pushed in lexer from parser

        string[] typeToTokenName = g.getTokenDisplayNames();
        HashSet<string> tokens = new();
        foreach (var t in typeToTokenName) if (t != null) tokens.Add(t);
        Assert.AreEqual("[<INVALID>, 'x', E]", tokens.ToString());
    }

    [TestMethod]
    public void TestPredDoesNotHideNameToLiteralMapInLexer()
    {
        // 'x' is token and char in lexer rule
        var g = new Grammar(
                "grammar t;\n" +
                "a : 'x' X ; \n" +
                "X: 'x' {true}?;\n"); // must match as alias even with pred

        Assert.AreEqual("{'x'=1}", g.stringLiteralToTypeMap.ToString());
        Assert.AreEqual("{EOF=-1, X=1}", g.tokenNameToTypeMap.ToString());

        // pushed in lexer from parser
        Assert.AreEqual("{'x'=1}", g.implicitLexer.stringLiteralToTypeMap.ToString());
        Assert.AreEqual("{EOF=-1, X=1}", g.implicitLexer.tokenNameToTypeMap.ToString());
    }

    [TestMethod]
    public void TestCombinedGrammarWithRefToLiteralButNoTokenIDRef()
    {
        var g = new Grammar(
                "grammar t;\n" +
                "a : 'a' ;\n" +
                "A : 'a' ;\n");
        var rules = "a";
        var tokenNames = "A, 'a'";
        CheckSymbols(g, rules, tokenNames);
    }

    [TestMethod]
    public void TestSetDoesNotMissTokenAliases()
    {
        var g = new Grammar(
                "grammar t;\n" +
                "a : 'a'|'b' ;\n" +
                "A : 'a' ;\n" +
                "B : 'b' ;\n");
        var rules = "a";
        var tokenNames = "A, 'a', B, 'b'";
        CheckSymbols(g, rules, tokenNames);
    }

    // T E S T  L I T E R A L  E S C A P E S

    [TestMethod]
    public void TestParserCharLiteralWithEscape()
    {
        var g = new Grammar(
                "grammar t;\n" +
                "a : '\\n';\n");
        var literals = g.stringLiteralToTypeMap.Keys;
        // must store literals how they appear in the antlr grammar
        Assert.AreEqual("'\\n'", literals.ToArray()[0]);
    }

    [TestMethod]
    public void TestParserCharLiteralWithBasicUnicodeEscape()
    {
        var g = new Grammar(
                "grammar t;\n" +
                "a : '\\uABCD';\n");
        var literals = g.stringLiteralToTypeMap.Keys;
        // must store literals how they appear in the antlr grammar
        Assert.AreEqual("'\\uABCD'", literals.ToArray()[0]);
    }

    [TestMethod]
    public void TestParserCharLiteralWithExtendedUnicodeEscape()
    {
        var g = new Grammar(
                "grammar t;\n" +
                "a : '\\u{1ABCD}';\n");
        var literals = g.stringLiteralToTypeMap.Keys;
        // must store literals how they appear in the antlr grammar
        Assert.AreEqual("'\\u{1ABCD}'", literals.ToArray()[0]);
    }

    protected static void CheckSymbols(Grammar g,
                                string rulesStr,
                                string allValidTokensStr)

    {
        var typeToTokenName = g.getTokenNames();
        var tokens = new HashSet<string>();
        for (int i = 0; i < typeToTokenName.Length; i++)
        {
            var t = typeToTokenName[i];
            if (t != null)
            {
                if (t.StartsWith(Grammar.AUTO_GENERATED_TOKEN_NAME_PREFIX))
                {
                    tokens.Add(g.getTokenDisplayName(i));
                }
                else
                {
                    tokens.Add(t);
                }
            }
        }

        // make sure expected tokens are there
        var parts = allValidTokensStr.Split(", ");
        foreach (var tokenName in parts)
        {
            Assert.IsTrue(g.getTokenType(tokenName) != Token.INVALID_TYPE, "token " + tokenName + " expected, but was undefined");
            tokens.Remove(tokenName);
        }
        // make sure there are not any others (other than <EOF> etc...)
        foreach (var tokenName in tokens)
        {
            Assert.IsTrue(g.getTokenType(tokenName) < Token.MIN_USER_TOKEN_TYPE, "unexpected token name " + tokenName);
        }
        parts = rulesStr.Split(", ");
        int n = 0;
        foreach (var ruleName in parts)
        {
            Assert.IsNotNull(g.getRule(ruleName), "rule " + ruleName + " expected");
            n++;
        }
        //Console.Out.WriteLine("rules="+rules);
        // make sure there are no extra rules
        Assert.AreEqual(n, g.rules.Count, "number of rules mismatch; expecting " + n + "; found " + g.rules.Count);
    }
}
