/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime;
using org.antlr.v4.runtime.dfa;

namespace org.antlr.v4.test.tool;

public class TestCommonTokenStream : TestBufferedTokenStream
{
    //@Override
    protected static TokenStream CreateTokenStream(TokenSource src)
    {
        return new CommonTokenStream(src);
    }

    public class TS1 : TokenSource
    {
        int i = 0;


        public WritableToken[] tokens;

        public TS1()
        {
            this.tokens =
                new WritableToken[]{
                    new CommonToken(1," ") {channel = Lexer.HIDDEN },
                    new CommonToken(1,"x"),
                    new CommonToken(1," ") {channel = Lexer.HIDDEN  },
                    new CommonToken(1,"="),
                    new CommonToken(1,"34"),
                    new CommonToken(1," ") { channel = Lexer.HIDDEN },
                    new CommonToken(1," ") { channel = Lexer.HIDDEN },
                    new CommonToken(1,";"),
                    new CommonToken(1,"\n") { channel = Lexer.HIDDEN },
                    new CommonToken(Token.EOF,"")
            };
        }
        //@Override
        public Token NextToken() => tokens[i++];

        ////@Override
        public string GetSourceName() => "test";
        ////@Override
        public int getCharPositionInLine()
        {
            return 0;
        }
        ////@Override
        public int getLine()
        {
            return 0;
        }
        ////@Override
        public CharStream getInputStream()
        {
            return null;
        }

        ////@Override
        public void setTokenFactory(TokenFactory factory)
        {
        }

        ////@Override
        public TokenFactory getTokenFactory()
        {
            return null;
        }
    }
    [TestMethod]
    public void TestOffChannel()
    {
        var lexer = // simulate input " x =34  ;\n"
            new TS1();

        var tokens = new CommonTokenStream(lexer);

        Assert.AreEqual("x", tokens.LT(1).getText()); // must skip first off channel token
        tokens.consume();
        Assert.AreEqual("=", tokens.LT(1).getText());
        Assert.AreEqual("x", tokens.LT(-1).getText());

        tokens.consume();
        Assert.AreEqual("34", tokens.LT(1).getText());
        Assert.AreEqual("=", tokens.LT(-1).getText());

        tokens.consume();
        Assert.AreEqual(";", tokens.LT(1).getText());
        Assert.AreEqual("34", tokens.LT(-1).getText());

        tokens.consume();
        Assert.AreEqual(Token.EOF, tokens.LA(1));
        Assert.AreEqual(";", tokens.LT(-1).getText());

        Assert.AreEqual("34", tokens.LT(-2).getText());
        Assert.AreEqual("=", tokens.LT(-3).getText());
        Assert.AreEqual("x", tokens.LT(-4).getText());
    }
    public class TS : TokenSource
    {
        int i = 0;
        readonly WritableToken[] tokens = {
                new CommonToken(1," ") {channel = Lexer.HIDDEN }, // 0
				new CommonToken(1,"x"),								// 1
				new CommonToken(1," ") {channel = Lexer.HIDDEN},	// 2
				new CommonToken(1,"="),								// 3
				new CommonToken(1,"34"),							// 4
				new CommonToken(1," ") { channel = Lexer.HIDDEN },	// 5
				new CommonToken(1," ") { channel = Lexer.HIDDEN }, // 6
				new CommonToken(1,";"),								// 7
				new CommonToken(1," ") { channel = Lexer.HIDDEN },// 8
				new CommonToken(1,"\n") { channel = Lexer.HIDDEN },// 9
				new CommonToken(Token.EOF,"")						// 10
				};
        //@Override
        public Token NextToken()
        {
            return tokens[i++];
        }
        //@Override
        public string GetSourceName() { return "test"; }
        //@Override
        public int getCharPositionInLine()
        {
            return 0;
        }
        //@Override
        public int getLine()
        {
            return 0;
        }
        //@Override
        public CharStream getInputStream()
        {
            return null;
        }

        //@Override
        public void setTokenFactory(TokenFactory factory)
        {
        }

        //@Override
        public TokenFactory getTokenFactory()
        {
            return null;
        }
    }

    [TestMethod]
    public void TestFetchOffChannel()
    {
        var lexer = // simulate input " x =34  ; \n"
                            // token indexes   01234 56789
            new TS();

        var tokens = new CommonTokenStream(lexer);
        tokens.fill();
        Assert.AreEqual(null, tokens.getHiddenTokensToLeft(0));
        Assert.AreEqual(null, tokens.getHiddenTokensToRight(0));

        Assert.AreEqual("[[@0,0:0=' ',<1>,channel=1,0:-1]]",
                     tokens.getHiddenTokensToLeft(1).ToString());
        Assert.AreEqual("[[@2,0:0=' ',<1>,channel=1,0:-1]]",
                     tokens.getHiddenTokensToRight(1).ToString());

        Assert.AreEqual(null, tokens.getHiddenTokensToLeft(2));
        Assert.AreEqual(null, tokens.getHiddenTokensToRight(2));

        Assert.AreEqual("[[@2,0:0=' ',<1>,channel=1,0:-1]]",
                     tokens.getHiddenTokensToLeft(3).ToString());
        Assert.AreEqual(null, tokens.getHiddenTokensToRight(3));

        Assert.AreEqual(null, tokens.getHiddenTokensToLeft(4));
        Assert.AreEqual("[[@5,0:0=' ',<1>,channel=1,0:-1], [@6,0:0=' ',<1>,channel=1,0:-1]]",
                     tokens.getHiddenTokensToRight(4).ToString());

        Assert.AreEqual(null, tokens.getHiddenTokensToLeft(5));
        Assert.AreEqual("[[@6,0:0=' ',<1>,channel=1,0:-1]]",
                     tokens.getHiddenTokensToRight(5).ToString());

        Assert.AreEqual("[[@5,0:0=' ',<1>,channel=1,0:-1]]",
                     tokens.getHiddenTokensToLeft(6).ToString());
        Assert.AreEqual(null, tokens.getHiddenTokensToRight(6));

        Assert.AreEqual("[[@5,0:0=' ',<1>,channel=1,0:-1], [@6,0:0=' ',<1>,channel=1,0:-1]]",
                     tokens.getHiddenTokensToLeft(7).ToString());
        Assert.AreEqual("[[@8,0:0=' ',<1>,channel=1,0:-1], [@9,0:0='\\n',<1>,channel=1,0:-1]]",
                     tokens.getHiddenTokensToRight(7).ToString());

        Assert.AreEqual(null, tokens.getHiddenTokensToLeft(8));
        Assert.AreEqual("[[@9,0:0='\\n',<1>,channel=1,0:-1]]",
                     tokens.getHiddenTokensToRight(8).ToString());

        Assert.AreEqual("[[@8,0:0=' ',<1>,channel=1,0:-1]]",
                     tokens.getHiddenTokensToLeft(9).ToString());
        Assert.AreEqual(null, tokens.getHiddenTokensToRight(9));
    }
    public class TS2 : TokenSource
    {

        //@Override
        public Token NextToken()
        {
            return new CommonToken(Token.EOF);
        }

        //@Override
        public int getLine()
        {
            return 0;
        }

        //@Override
        public int getCharPositionInLine()
        {
            return 0;
        }

        //@Override
        public CharStream getInputStream()
        {
            return null;
        }

        //@Override
        public String GetSourceName()
        {
            return IntStream.UNKNOWN_SOURCE_NAME;
        }

        //@Override
        public TokenFactory getTokenFactory()
        {
            throw new UnsupportedOperationException("Not supported yet.");
        }

        //@Override
        public void setTokenFactory(TokenFactory factory)
        {
            throw new UnsupportedOperationException("Not supported yet.");
        }
    }
    [TestMethod]
    public void TestSingleEOF()
    {
        var lexer = new TS2();

        var tokens = new CommonTokenStream(lexer);
        tokens.fill();

        Assert.AreEqual(Token.EOF, tokens.LA(1));
        Assert.AreEqual(0, tokens.index());
        Assert.AreEqual(1, tokens.Count);
    }

    public class TS3 : TokenSource
    {

        //@Override
        public Token NextToken()
        {
            return new CommonToken(Token.EOF);
        }

        //@Override
        public int getLine()
        {
            return 0;
        }

        //@Override
        public int getCharPositionInLine()
        {
            return 0;
        }

        //@Override
        public CharStream getInputStream()
        {
            return null;
        }

        //@Override
        public String GetSourceName()
        {
            return IntStream.UNKNOWN_SOURCE_NAME;
        }

        //@Override
        public TokenFactory getTokenFactory()
        {
            throw new UnsupportedOperationException("Not supported yet.");
        }

        //@Override
        public void setTokenFactory(TokenFactory factory)
        {
            throw new UnsupportedOperationException("Not supported yet.");
        }
    }
    [TestMethod]
    public void TestCannotConsumeEOF()
    {
        var lexer = new TS3();

        var tokens = new CommonTokenStream(lexer);
        tokens.fill();

        Assert.AreEqual(Token.EOF, tokens.LA(1));
        Assert.AreEqual(0, tokens.index());
        Assert.AreEqual(1, tokens.Count);
        //assertThrows(IllegalStateException, tokens::consume);
        Assert.ThrowsException<IllegalStateException>(() => tokens.consume());

    }
}
