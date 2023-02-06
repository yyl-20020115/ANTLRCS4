/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime;
using org.antlr.v4.tool;

namespace org.antlr.v4.test.tool;

[TestClass]
public class TestBufferedTokenStream
{
    protected static TokenStream CreateTokenStream(TokenSource src) 
        => new BufferedTokenStream(src);

    [TestMethod]
    public void TestFirstToken()
    {
        var g = new LexerGrammar(
            "lexer grammar t;\n" +
            "ID : 'a'..'z'+;\n" +
            "INT : '0'..'9'+;\n" +
            "SEMI : ';';\n" +
            "ASSIGN : '=';\n" +
            "PLUS : '+';\n" +
            "MULT : '*';\n" +
            "WS : ' '+;\n");
        // Tokens: 012345678901234567
        // Input:  x = 3 * 0 + 2 * 0;
        var input = new ANTLRInputStream("x = 3 * 0 + 2 * 0;");
        var lexEngine = g.createLexerInterpreter(input);
        var tokens = CreateTokenStream(lexEngine);

        var result = tokens.LT(1).getText();
        var expecting = "x";
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    public void Test2ndToken()
    {
        var g = new LexerGrammar(
            "lexer grammar t;\n" +
            "ID : 'a'..'z'+;\n" +
            "INT : '0'..'9'+;\n" +
            "SEMI : ';';\n" +
            "ASSIGN : '=';\n" +
            "PLUS : '+';\n" +
            "MULT : '*';\n" +
            "WS : ' '+;\n");
        // Tokens: 012345678901234567
        // Input:  x = 3 * 0 + 2 * 0;
        var input = new ANTLRInputStream("x = 3 * 0 + 2 * 0;");
        var lexEngine = g.createLexerInterpreter(input);
        var tokens = CreateTokenStream(lexEngine);

        var result = tokens.LT(2).getText();
        var expecting = " ";
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    public void TestCompleteBuffer()
    {
        var g = new LexerGrammar(
            "lexer grammar t;\n" +
            "ID : 'a'..'z'+;\n" +
            "INT : '0'..'9'+;\n" +
            "SEMI : ';';\n" +
            "ASSIGN : '=';\n" +
            "PLUS : '+';\n" +
            "MULT : '*';\n" +
            "WS : ' '+;\n");
        // Tokens: 012345678901234567
        // Input:  x = 3 * 0 + 2 * 0;
        var input = new ANTLRInputStream("x = 3 * 0 + 2 * 0;");
        var lexEngine = g.createLexerInterpreter(input);
        var tokens = CreateTokenStream(lexEngine);

        int i = 1;
        var t = tokens.LT(i);
        while (t.getType() != Token.EOF)
        {
            i++;
            t = tokens.LT(i);
        }
        tokens.LT(i++); // push it past end
        tokens.LT(i++);

        var result = tokens.getText();
        var expecting = "x = 3 * 0 + 2 * 0;";
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    public void TestCompleteBufferAfterConsuming()
    {
        var g = new LexerGrammar(
            "lexer grammar t;\n" +
            "ID : 'a'..'z'+;\n" +
            "INT : '0'..'9'+;\n" +
            "SEMI : ';';\n" +
            "ASSIGN : '=';\n" +
            "PLUS : '+';\n" +
            "MULT : '*';\n" +
            "WS : ' '+;\n");
        // Tokens: 012345678901234567
        // Input:  x = 3 * 0 + 2 * 0;
        var input = new ANTLRInputStream("x = 3 * 0 + 2 * 0;");
        var lexEngine = g.createLexerInterpreter(input);
        var tokens = CreateTokenStream(lexEngine);

        var t = tokens.LT(1);
        while (t.getType() != Token.EOF)
        {
            tokens.consume();
            t = tokens.LT(1);
        }

        var result = tokens.getText();
        var expecting = "x = 3 * 0 + 2 * 0;";
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    public void TestLookback()
    {
        var g = new LexerGrammar(
            "lexer grammar t;\n" +
            "ID : 'a'..'z'+;\n" +
            "INT : '0'..'9'+;\n" +
            "SEMI : ';';\n" +
            "ASSIGN : '=';\n" +
            "PLUS : '+';\n" +
            "MULT : '*';\n" +
            "WS : ' '+;\n");
        // Tokens: 012345678901234567
        // Input:  x = 3 * 0 + 2 * 0;
        var input = new ANTLRInputStream("x = 3 * 0 + 2 * 0;");
        var lexEngine = g.createLexerInterpreter(input);
        var tokens = CreateTokenStream(lexEngine);

        tokens.consume(); // get x into buffer
        var t = tokens.LT(-1);
        Assert.AreEqual("x", t.getText());

        tokens.consume();
        tokens.consume(); // consume '='
        t = tokens.LT(-3);
        Assert.AreEqual("x", t.getText());
        t = tokens.LT(-2);
        Assert.AreEqual(" ", t.getText());
        t = tokens.LT(-1);
        Assert.AreEqual("=", t.getText());
    }
}
