/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

namespace org.antlr.v4.test.tool;

public class TestBufferedTokenStream {
	protected TokenStream createTokenStream(TokenSource src) {
		return new BufferedTokenStream(src);
	}

	[TestMethod] public void testFirstToken(){
        LexerGrammar g = new LexerGrammar(
            "lexer grammar t;\n"+
            "ID : 'a'..'z'+;\n" +
            "INT : '0'..'9'+;\n" +
            "SEMI : ';';\n" +
            "ASSIGN : '=';\n" +
            "PLUS : '+';\n" +
            "MULT : '*';\n" +
            "WS : ' '+;\n");
        // Tokens: 012345678901234567
        // Input:  x = 3 * 0 + 2 * 0;
        CharStream input = new ANTLRInputStream("x = 3 * 0 + 2 * 0;");
        LexerInterpreter lexEngine = g.createLexerInterpreter(input);
        TokenStream tokens = createTokenStream(lexEngine);

        String result = tokens.LT(1).getText();
        String expecting = "x";
        Assert.AreEqual(expecting, result);
    }

    [TestMethod] public void test2ndToken(){
        LexerGrammar g = new LexerGrammar(
            "lexer grammar t;\n"+
            "ID : 'a'..'z'+;\n" +
            "INT : '0'..'9'+;\n" +
            "SEMI : ';';\n" +
            "ASSIGN : '=';\n" +
            "PLUS : '+';\n" +
            "MULT : '*';\n" +
            "WS : ' '+;\n");
        // Tokens: 012345678901234567
        // Input:  x = 3 * 0 + 2 * 0;
        CharStream input = new ANTLRInputStream("x = 3 * 0 + 2 * 0;");
        LexerInterpreter lexEngine = g.createLexerInterpreter(input);
        TokenStream tokens = createTokenStream(lexEngine);

        String result = tokens.LT(2).getText();
        String expecting = " ";
        Assert.AreEqual(expecting, result);
    }

    [TestMethod] public void testCompleteBuffer(){
        LexerGrammar g = new LexerGrammar(
            "lexer grammar t;\n"+
            "ID : 'a'..'z'+;\n" +
            "INT : '0'..'9'+;\n" +
            "SEMI : ';';\n" +
            "ASSIGN : '=';\n" +
            "PLUS : '+';\n" +
            "MULT : '*';\n" +
            "WS : ' '+;\n");
        // Tokens: 012345678901234567
        // Input:  x = 3 * 0 + 2 * 0;
        CharStream input = new ANTLRInputStream("x = 3 * 0 + 2 * 0;");
        LexerInterpreter lexEngine = g.createLexerInterpreter(input);
        TokenStream tokens = createTokenStream(lexEngine);

        int i = 1;
        Token t = tokens.LT(i);
        while ( t.getType()!=Token.EOF ) {
            i++;
            t = tokens.LT(i);
        }
        tokens.LT(i++); // push it past end
        tokens.LT(i++);

        String result = tokens.getText();
        String expecting = "x = 3 * 0 + 2 * 0;";
        Assert.AreEqual(expecting, result);
    }

    [TestMethod] public void testCompleteBufferAfterConsuming(){
        LexerGrammar g = new LexerGrammar(
            "lexer grammar t;\n"+
            "ID : 'a'..'z'+;\n" +
            "INT : '0'..'9'+;\n" +
            "SEMI : ';';\n" +
            "ASSIGN : '=';\n" +
            "PLUS : '+';\n" +
            "MULT : '*';\n" +
            "WS : ' '+;\n");
        // Tokens: 012345678901234567
        // Input:  x = 3 * 0 + 2 * 0;
        CharStream input = new ANTLRInputStream("x = 3 * 0 + 2 * 0;");
        LexerInterpreter lexEngine = g.createLexerInterpreter(input);
        TokenStream tokens = createTokenStream(lexEngine);

        Token t = tokens.LT(1);
        while ( t.getType()!=Token.EOF ) {
            tokens.consume();
            t = tokens.LT(1);
        }

        String result = tokens.getText();
        String expecting = "x = 3 * 0 + 2 * 0;";
        Assert.AreEqual(expecting, result);
    }

    [TestMethod] public void testLookback(){
        LexerGrammar g = new LexerGrammar(
            "lexer grammar t;\n"+
            "ID : 'a'..'z'+;\n" +
            "INT : '0'..'9'+;\n" +
            "SEMI : ';';\n" +
            "ASSIGN : '=';\n" +
            "PLUS : '+';\n" +
            "MULT : '*';\n" +
            "WS : ' '+;\n");
        // Tokens: 012345678901234567
        // Input:  x = 3 * 0 + 2 * 0;
        CharStream input = new ANTLRInputStream("x = 3 * 0 + 2 * 0;");
        LexerInterpreter lexEngine = g.createLexerInterpreter(input);
        TokenStream tokens = createTokenStream(lexEngine);

        tokens.consume(); // get x into buffer
        Token t = tokens.LT(-1);
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
