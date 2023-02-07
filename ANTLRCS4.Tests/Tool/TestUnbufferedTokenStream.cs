/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime;
using org.antlr.v4.tool;

namespace org.antlr.v4.test.tool;

//@SuppressWarnings("unused")
[TestClass]
public class TestUnbufferedTokenStream
{
    [TestMethod]
    public void TestLookahead()
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
        // Input:  x = 302;
        var input = new ANTLRInputStream(
            new StringReader("x = 302;")
        );
        var lexEngine = g.createLexerInterpreter(input);
        var tokens = new UnbufferedTokenStream(lexEngine);

        Assert.AreEqual("x", tokens.LT(1).Text);
        Assert.AreEqual(" ", tokens.LT(2).Text);
        Assert.AreEqual("=", tokens.LT(3).Text);
        Assert.AreEqual(" ", tokens.LT(4).Text);
        Assert.AreEqual("302", tokens.LT(5).Text);
        Assert.AreEqual(";", tokens.LT(6).Text);
    }

    [TestMethod]
    public void TestNoBuffering()
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
        // Input:  x = 302;
        var input = new ANTLRInputStream(
            new StringReader("x = 302;")
        );
        var lexEngine = g.createLexerInterpreter(input);
        var tokens = new TestingUnbufferedTokenStream(lexEngine);

        Assert.AreEqual("[[@0,0:0='x',<1>,1:0]]", tokens.GetBuffer().ToString());
        Assert.AreEqual("x", tokens.LT(1).Text);
        tokens.Consume(); // move to WS
        Assert.AreEqual(" ", tokens.LT(1).Text);
        Assert.AreEqual("[[@1,1:1=' ',<7>,1:1]]", tokens.GetRemainingBuffer().ToString());
        tokens.Consume();
        Assert.AreEqual("=", tokens.LT(1).Text);
        Assert.AreEqual("[[@2,2:2='=',<4>,1:2]]", tokens.GetRemainingBuffer().ToString());
        tokens.Consume();
        Assert.AreEqual(" ", tokens.LT(1).Text);
        Assert.AreEqual("[[@3,3:3=' ',<7>,1:3]]", tokens.GetRemainingBuffer().ToString());
        tokens.Consume();
        Assert.AreEqual("302", tokens.LT(1).Text);
        Assert.AreEqual("[[@4,4:6='302',<2>,1:4]]", tokens.GetRemainingBuffer().ToString());
        tokens.Consume();
        Assert.AreEqual(";", tokens.LT(1).Text);
        Assert.AreEqual("[[@5,7:7=';',<3>,1:7]]", tokens.GetRemainingBuffer().ToString());
    }

    [TestMethod]
    public void TestMarkStart()
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
        // Input:  x = 302;
        var input = new ANTLRInputStream(
            new StringReader("x = 302;")
        );
        var lexEngine = g.createLexerInterpreter(input);
        var tokens = new TestingUnbufferedTokenStream(lexEngine);

        int m = tokens.Mark();
        Assert.AreEqual("[[@0,0:0='x',<1>,1:0]]", tokens.GetBuffer().ToString());
        Assert.AreEqual("x", tokens.LT(1).Text);
        tokens.Consume(); // consume x
        Assert.AreEqual("[[@0,0:0='x',<1>,1:0], [@1,1:1=' ',<7>,1:1]]", tokens.GetBuffer().ToString());
        tokens.Consume(); // ' '
        tokens.Consume(); // =
        tokens.Consume(); // ' '
        tokens.Consume(); // 302
        tokens.Consume(); // ;
        Assert.AreEqual("[[@0,0:0='x',<1>,1:0], [@1,1:1=' ',<7>,1:1]," +
                     " [@2,2:2='=',<4>,1:2], [@3,3:3=' ',<7>,1:3]," +
                     " [@4,4:6='302',<2>,1:4], [@5,7:7=';',<3>,1:7]," +
                     " [@6,8:7='<EOF>',<-1>,1:8]]",
                     tokens.GetBuffer().ToString());
    }

    [TestMethod]
    public void TestMarkThenRelease()
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
        // Input:  x = 302;
        var input = new ANTLRInputStream(
            new StringReader("x = 302 + 1;")
        );
        var lexEngine = g.createLexerInterpreter(input);
        var tokens = new TestingUnbufferedTokenStream(lexEngine);

        int m = tokens.Mark();
        Assert.AreEqual("[[@0,0:0='x',<1>,1:0]]", tokens.GetBuffer().ToString());
        Assert.AreEqual("x", tokens.LT(1).Text);
        tokens.Consume(); // consume x
        Assert.AreEqual("[[@0,0:0='x',<1>,1:0], [@1,1:1=' ',<7>,1:1]]", tokens.GetBuffer().ToString());
        tokens.Consume(); // ' '
        tokens.Consume(); // =
        tokens.Consume(); // ' '
        Assert.AreEqual("302", tokens.LT(1).Text);
        tokens.Release(m); // "x = 302" is in buffer. will kill buffer
        tokens.Consume(); // 302
        tokens.Consume(); // ' '
        m = tokens.Mark(); // mark at the +
        Assert.AreEqual("+", tokens.LT(1).Text);
        tokens.Consume(); // '+'
        tokens.Consume(); // ' '
        tokens.Consume(); // 1
        tokens.Consume(); // ;
        Assert.AreEqual("<EOF>", tokens.LT(1).Text);
        // we marked at the +, so that should be the start of the buffer
        Assert.AreEqual("[[@6,8:8='+',<5>,1:8], [@7,9:9=' ',<7>,1:9]," +
                     " [@8,10:10='1',<2>,1:10], [@9,11:11=';',<3>,1:11]," +
                     " [@10,12:11='<EOF>',<-1>,1:12]]",
                     tokens.GetBuffer().ToString());
        tokens.Release(m);
    }

    protected class TestingUnbufferedTokenStream : UnbufferedTokenStream
    {

        public TestingUnbufferedTokenStream(TokenSource tokenSource) : base(tokenSource)
        {
        }

        /** For testing.  What's in moving window into token stream from
		 *  current index, LT(1) or tokens[p], to end of buffer?
		 */
        public List<Token> GetRemainingBuffer()
        {
            if (n == 0) return new();

            return tokens[p..n].ToList();// Arrays.AsList(tokens).subList(p, n);
        }

        /** For testing.  What's in moving window buffer into data stream.
		 *  From 0..p-1 have been consume.
		 */
        public List<Token> GetBuffer()
        {
            if (n == 0) return new();

            return tokens[..n].ToList();// Arrays.AsList(tokens).subList(0, n);
        }
    }
}
