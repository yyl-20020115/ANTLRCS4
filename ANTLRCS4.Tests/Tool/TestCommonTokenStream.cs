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
    protected override TokenStream CreateTokenStream(TokenSource src) 
        => new CommonTokenStream(src);

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
        public Token NextToken() => tokens[i++];

        
        public string SourceName => "test";
        
        public int CharPositionInLine => 0;
        
        public int Line => 0;

        
        public CharStream CharInputStream => null;

        
        
        public TokenFactory TokenFactory
        {
            get => null;
            set
            {
            }
        }
    }
    [TestMethod]
    public void TestOffChannel()
    {
        var lexer = // simulate input " x =34  ;\n"
            new TS1();

        var tokens = new CommonTokenStream(lexer);

        Assert.AreEqual("x", tokens.LT(1).Text); // must skip first off channel token
        tokens.Consume();
        Assert.AreEqual("=", tokens.LT(1).Text);
        Assert.AreEqual("x", tokens.LT(-1).Text);

        tokens.Consume();
        Assert.AreEqual("34", tokens.LT(1).Text);
        Assert.AreEqual("=", tokens.LT(-1).Text);

        tokens.Consume();
        Assert.AreEqual(";", tokens.LT(1).Text);
        Assert.AreEqual("34", tokens.LT(-1).Text);

        tokens.Consume();
        Assert.AreEqual(Token.EOF, tokens.LA(1));
        Assert.AreEqual(";", tokens.LT(-1).Text);

        Assert.AreEqual("34", tokens.LT(-2).Text);
        Assert.AreEqual("=", tokens.LT(-3).Text);
        Assert.AreEqual("x", tokens.LT(-4).Text);
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
        
        public Token NextToken()
        {
            return tokens[i++];
        }
        
        public string SourceName => "test";         
        public int CharPositionInLine => 0;
        
        public int Line => 0;

        
        public CharStream CharInputStream => null;

        
        
        public TokenFactory TokenFactory
        {
            get => null;
            set
            {
            }
        }
    }

    [TestMethod]
    public void TestFetchOffChannel()
    {
        var lexer = // simulate input " x =34  ; \n"
                            // token indexes   01234 56789
            new TS();

        var tokens = new CommonTokenStream(lexer);
        tokens.Fill();
        Assert.AreEqual(null, tokens.GetHiddenTokensToLeft(0));
        Assert.AreEqual(null, tokens.GetHiddenTokensToRight(0));

        Assert.AreEqual("[[@0,0:0=' ',<1>,channel=1,0:-1]]",
                     tokens.GetHiddenTokensToLeft(1).ToString());
        Assert.AreEqual("[[@2,0:0=' ',<1>,channel=1,0:-1]]",
                     tokens.GetHiddenTokensToRight(1).ToString());

        Assert.AreEqual(null, tokens.GetHiddenTokensToLeft(2));
        Assert.AreEqual(null, tokens.GetHiddenTokensToRight(2));

        Assert.AreEqual("[[@2,0:0=' ',<1>,channel=1,0:-1]]",
                     tokens.GetHiddenTokensToLeft(3).ToString());
        Assert.AreEqual(null, tokens.GetHiddenTokensToRight(3));

        Assert.AreEqual(null, tokens.GetHiddenTokensToLeft(4));
        Assert.AreEqual("[[@5,0:0=' ',<1>,channel=1,0:-1], [@6,0:0=' ',<1>,channel=1,0:-1]]",
                     tokens.GetHiddenTokensToRight(4).ToString());

        Assert.AreEqual(null, tokens.GetHiddenTokensToLeft(5));
        Assert.AreEqual("[[@6,0:0=' ',<1>,channel=1,0:-1]]",
                     tokens.GetHiddenTokensToRight(5).ToString());

        Assert.AreEqual("[[@5,0:0=' ',<1>,channel=1,0:-1]]",
                     tokens.GetHiddenTokensToLeft(6).ToString());
        Assert.AreEqual(null, tokens.GetHiddenTokensToRight(6));

        Assert.AreEqual("[[@5,0:0=' ',<1>,channel=1,0:-1], [@6,0:0=' ',<1>,channel=1,0:-1]]",
                     tokens.GetHiddenTokensToLeft(7).ToString());
        Assert.AreEqual("[[@8,0:0=' ',<1>,channel=1,0:-1], [@9,0:0='\\n',<1>,channel=1,0:-1]]",
                     tokens.GetHiddenTokensToRight(7).ToString());

        Assert.AreEqual(null, tokens.GetHiddenTokensToLeft(8));
        Assert.AreEqual("[[@9,0:0='\\n',<1>,channel=1,0:-1]]",
                     tokens.GetHiddenTokensToRight(8).ToString());

        Assert.AreEqual("[[@8,0:0=' ',<1>,channel=1,0:-1]]",
                     tokens.GetHiddenTokensToLeft(9).ToString());
        Assert.AreEqual(null, tokens.GetHiddenTokensToRight(9));
    }
    public class TS2 : TokenSource
    {

        
        public Token NextToken()
        {
            return new CommonToken(Token.EOF);
        }

        
        public int Line => 0;

        
        public int CharPositionInLine => 0;

        
        public CharStream CharInputStream => null;

        
        public string SourceName => IntStream.UNKNOWN_SOURCE_NAME;

        
        
        public TokenFactory TokenFactory { get => throw new UnsupportedOperationException("Not supported yet."); set => throw new UnsupportedOperationException("Not supported yet."); }
    }
    [TestMethod]
    public void TestSingleEOF()
    {
        var lexer = new TS2();

        var tokens = new CommonTokenStream(lexer);
        tokens.Fill();

        Assert.AreEqual(Token.EOF, tokens.LA(1));
        Assert.AreEqual(0, tokens.Index);
        Assert.AreEqual(1, tokens.Count);
    }

    public class TS3 : TokenSource
    {

        public Token NextToken() => new CommonToken(Token.EOF);

        public int Line => 0;

        
        public int CharPositionInLine => 0;

        
        public CharStream CharInputStream => null;

        public string SourceName => IntStream.UNKNOWN_SOURCE_NAME;

        public TokenFactory TokenFactory { get => throw new UnsupportedOperationException("Not supported yet."); set => throw new UnsupportedOperationException("Not supported yet."); }
    }
    [TestMethod]
    public void TestCannotConsumeEOF()
    {
        var lexer = new TS3();

        var tokens = new CommonTokenStream(lexer);
        tokens.Fill();

        Assert.AreEqual(Token.EOF, tokens.LA(1));
        Assert.AreEqual(0, tokens.Index);
        Assert.AreEqual(1, tokens.Count);
        //assertThrows(IllegalStateException, tokens::consume);
        Assert.ThrowsException<IllegalStateException>(() => tokens.Consume());

    }
}
