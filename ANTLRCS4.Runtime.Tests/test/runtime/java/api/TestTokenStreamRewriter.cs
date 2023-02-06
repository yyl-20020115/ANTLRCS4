/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */
using org.antlr.v4.runtime;
using org.antlr.v4.runtime.misc;
using org.antlr.v4.tool;

namespace org.antlr.v4.test.runtime.java.api;

[TestClass]
public class TestTokenStreamRewriter
{

    /** Public default constructor used by TestRig */
    public TestTokenStreamRewriter()
    {
    }

    [TestMethod]
    public void TestInsertBeforeIndex0()
    {
        var g = new LexerGrammar(
                                             "lexer grammar T;\n" +
                                             "A : 'a';\n" +
                                             "B : 'b';\n" +
                                             "C : 'c';\n");
        var lexEngine = g.createLexerInterpreter(new ANTLRInputStream("abc"));
        var stream = new CommonTokenStream(lexEngine);
        stream.fill();
        var tokens = new TokenStreamRewriter(stream);
        tokens.insertBefore(0, "0");
        var result = tokens.getText();
        var expecting = "0abc";
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    public void TestInsertAfterLastIndex()
    {
        var g = new LexerGrammar(
                                             "lexer grammar T;\n" +
                                             "A : 'a';\n" +
                                             "B : 'b';\n" +
                                             "C : 'c';\n");
        var input = "abc";
        var lexEngine = g.createLexerInterpreter(new ANTLRInputStream(input));
        var stream = new CommonTokenStream(lexEngine);
        stream.fill();
        var tokens = new TokenStreamRewriter(stream);
        tokens.insertAfter(2, "x");
        var result = tokens.getText();
        var expecting = "abcx";
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    public void Test2InsertBeforeAfterMiddleIndex()
    {
        var g = new LexerGrammar(
                                             "lexer grammar T;\n" +
                                             "A : 'a';\n" +
                                             "B : 'b';\n" +
                                             "C : 'c';\n");
        var input = "abc";
        var lexEngine = g.createLexerInterpreter(new ANTLRInputStream(input));
        var stream = new CommonTokenStream(lexEngine);
        stream.fill();
        var tokens = new TokenStreamRewriter(stream);
        tokens.insertBefore(1, "x");
        tokens.insertAfter(1, "x");
        var result = tokens.getText();
        var expecting = "axbxc";
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    public void TestReplaceIndex0()
    {
        var g = new LexerGrammar(
                                             "lexer grammar T;\n" +
                                             "A : 'a';\n" +
                                             "B : 'b';\n" +
                                             "C : 'c';\n");
        var input = "abc";
        var lexEngine = g.createLexerInterpreter(new ANTLRInputStream(input));
        var stream = new CommonTokenStream(lexEngine);
        stream.fill();
        var tokens = new TokenStreamRewriter(stream);
        tokens.replace(0, "x");
        var result = tokens.getText();
        var expecting = "xbc";
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    public void TestReplaceLastIndex()
    {
        var g = new LexerGrammar(
                                             "lexer grammar T;\n" +
                                             "A : 'a';\n" +
                                             "B : 'b';\n" +
                                             "C : 'c';\n");
        var input = "abc";
        var lexEngine = g.createLexerInterpreter(new ANTLRInputStream(input));
        var stream = new CommonTokenStream(lexEngine);
        stream.fill();
        var tokens = new TokenStreamRewriter(stream);
        tokens.replace(2, "x");
        var result = tokens.getText();
        var expecting = "abx";
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    public void TestReplaceMiddleIndex()
    {
        var g = new LexerGrammar(
                                             "lexer grammar T;\n" +
                                             "A : 'a';\n" +
                                             "B : 'b';\n" +
                                             "C : 'c';\n");
        var input = "abc";
        var lexEngine = g.createLexerInterpreter(new ANTLRInputStream(input));
        var stream = new CommonTokenStream(lexEngine);
        stream.fill();
        var tokens = new TokenStreamRewriter(stream);
        tokens.replace(1, "x");
        var result = tokens.getText();
        var expecting = "axc";
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    public void TestToStringStartStop()
    {
        var g = new LexerGrammar(
                                             "lexer grammar T;\n" +
                                             "ID : 'a'..'z'+;\n" +
                                             "INT : '0'..'9'+;\n" +
                                             "SEMI : ';';\n" +
                                             "MUL : '*';\n" +
                                             "ASSIGN : '=';\n" +
                                             "WS : ' '+;\n");
        // Tokens: 0123456789
        // Input:  x = 3 * 0;
        var input = "x = 3 * 0;";
        var lexEngine = g.createLexerInterpreter(new ANTLRInputStream(input));
        var stream = new CommonTokenStream(lexEngine);
        stream.fill();
        var tokens = new TokenStreamRewriter(stream);
        tokens.replace(4, 8, "0");
        stream.fill();
        // replace 3 * 0 with 0

        var result = tokens.getTokenStream().getText();
        var expecting = "x = 3 * 0;";
        Assert.AreEqual(expecting, result);

        result = tokens.getText();
        expecting = "x = 0;";
        Assert.AreEqual(expecting, result);

        result = tokens.getText(Interval.of(0, 9));
        expecting = "x = 0;";
        Assert.AreEqual(expecting, result);

        result = tokens.getText(Interval.of(4, 8));
        expecting = "0";
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    public void TestToStringStartStop2()
    {
        var g = new LexerGrammar(
                                             "lexer grammar T;\n" +
                                             "ID : 'a'..'z'+;\n" +
                                             "INT : '0'..'9'+;\n" +
                                             "SEMI : ';';\n" +
                                             "ASSIGN : '=';\n" +
                                             "PLUS : '+';\n" +
                                             "MULT : '*';\n" +
                                             "WS : ' '+;\n");
        // Tokens: 012345678901234567
        // Input:  x = 3 * 0 + 2 * 0;
        var input = "x = 3 * 0 + 2 * 0;";
        var lexEngine = g.createLexerInterpreter(new ANTLRInputStream(input));
        var stream = new CommonTokenStream(lexEngine);
        stream.fill();
        var tokens = new TokenStreamRewriter(stream);

        var result = tokens.getTokenStream().getText();
        var expecting = "x = 3 * 0 + 2 * 0;";
        Assert.AreEqual(expecting, result);

        tokens.replace(4, 8, "0");
        stream.fill();
        // replace 3 * 0 with 0
        result = tokens.getText();
        expecting = "x = 0 + 2 * 0;";
        Assert.AreEqual(expecting, result);

        result = tokens.getText(Interval.of(0, 17));
        expecting = "x = 0 + 2 * 0;";
        Assert.AreEqual(expecting, result);

        result = tokens.getText(Interval.of(4, 8));
        expecting = "0";
        Assert.AreEqual(expecting, result);

        result = tokens.getText(Interval.of(0, 8));
        expecting = "x = 0";
        Assert.AreEqual(expecting, result);

        result = tokens.getText(Interval.of(12, 16));
        expecting = "2 * 0";
        Assert.AreEqual(expecting, result);

        tokens.insertAfter(17, "// comment");
        result = tokens.getText(Interval.of(12, 18));
        expecting = "2 * 0;// comment";
        Assert.AreEqual(expecting, result);

        result = tokens.getText(Interval.of(0, 8));
        stream.fill();
        // try again after insert at end
        expecting = "x = 0";
        Assert.AreEqual(expecting, result);
    }


    [TestMethod]
    public void Test2ReplaceMiddleIndex()
    {
        var g = new LexerGrammar(
                                             "lexer grammar T;\n" +
                                             "A : 'a';\n" +
                                             "B : 'b';\n" +
                                             "C : 'c';\n");
        var input = "abc";
        var lexEngine = g.createLexerInterpreter(new ANTLRInputStream(input));
        var stream = new CommonTokenStream(lexEngine);
        stream.fill();
        var tokens = new TokenStreamRewriter(stream);
        tokens.replace(1, "x");
        tokens.replace(1, "y");
        var result = tokens.getText();
        var expecting = "ayc";
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    public void Test2ReplaceMiddleIndex1InsertBefore()
    {
        var g = new LexerGrammar(
                                             "lexer grammar T;\n" +
                                             "A : 'a';\n" +
                                             "B : 'b';\n" +
                                             "C : 'c';\n");
        var input = "abc";
        var lexEngine = g.createLexerInterpreter(new ANTLRInputStream(input));
        var stream = new CommonTokenStream(lexEngine);
        stream.fill();
        var tokens = new TokenStreamRewriter(stream);
        tokens.insertBefore(0, "_");
        tokens.replace(1, "x");
        tokens.replace(1, "y");
        var result = tokens.getText();
        var expecting = "_ayc";
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    public void TestReplaceThenDeleteMiddleIndex()
    {
        var g = new LexerGrammar(
                                             "lexer grammar T;\n" +
                                             "A : 'a';\n" +
                                             "B : 'b';\n" +
                                             "C : 'c';\n");
        var input = "abc";
        var lexEngine = g.createLexerInterpreter(new ANTLRInputStream(input));
        var stream = new CommonTokenStream(lexEngine);
        stream.fill();
        var tokens = new TokenStreamRewriter(stream);
        tokens.replace(1, "x");
        tokens.delete(1);
        var result = tokens.getText();
        var expecting = "ac";
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    public void testInsertInPriorReplace()
    {
        LexerGrammar g = new LexerGrammar(
                                             "lexer grammar T;\n" +
                                             "A : 'a';\n" +
                                             "B : 'b';\n" +
                                             "C : 'c';\n");
        String input = "abc";
        LexerInterpreter lexEngine = g.createLexerInterpreter(new ANTLRInputStream(input));
        CommonTokenStream stream = new CommonTokenStream(lexEngine);
        stream.fill();
        TokenStreamRewriter tokens = new TokenStreamRewriter(stream);
        tokens.replace(0, 2, "x");
        tokens.insertBefore(1, "0");
        Exception exc = null;
        try
        {
            tokens.getText();
        }
        catch (ArgumentException iae)
        {
            exc = iae;
        }
        String expecting = "insert op <InsertBeforeOp@[@1,1:1='b',<2>,1:1]:\"0\"> within boundaries of previous <ReplaceOp@[@0,0:0='a',<1>,1:0]..[@2,2:2='c',<3>,1:2]:\"x\">";
        Assert.IsNotNull(exc);
        Assert.AreEqual(expecting, exc.Message);
    }

    [TestMethod]
    public void testInsertThenReplaceSameIndex()
    {
        LexerGrammar g = new LexerGrammar(
                                             "lexer grammar T;\n" +
                                             "A : 'a';\n" +
                                             "B : 'b';\n" +
                                             "C : 'c';\n");
        String input = "abc";
        LexerInterpreter lexEngine = g.createLexerInterpreter(new ANTLRInputStream(input));
        CommonTokenStream stream = new CommonTokenStream(lexEngine);
        stream.fill();
        TokenStreamRewriter tokens = new TokenStreamRewriter(stream);
        tokens.insertBefore(0, "0");
        tokens.replace(0, "x");
        stream.fill();
        // supercedes insert at 0
        String result = tokens.getText();
        String expecting = "0xbc";
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    public void test2InsertMiddleIndex()
    {
        LexerGrammar g = new LexerGrammar(
                                             "lexer grammar T;\n" +
                                             "A : 'a';\n" +
                                             "B : 'b';\n" +
                                             "C : 'c';\n");
        String input = "abc";
        LexerInterpreter lexEngine = g.createLexerInterpreter(new ANTLRInputStream(input));
        CommonTokenStream stream = new CommonTokenStream(lexEngine);
        stream.fill();
        TokenStreamRewriter tokens = new TokenStreamRewriter(stream);
        tokens.insertBefore(1, "x");
        tokens.insertBefore(1, "y");
        String result = tokens.getText();
        String expecting = "ayxbc";
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    public void test2InsertThenReplaceIndex0()
    {
        LexerGrammar g = new LexerGrammar(
                                             "lexer grammar T;\n" +
                                             "A : 'a';\n" +
                                             "B : 'b';\n" +
                                             "C : 'c';\n");
        String input = "abc";
        LexerInterpreter lexEngine = g.createLexerInterpreter(new ANTLRInputStream(input));
        CommonTokenStream stream = new CommonTokenStream(lexEngine);
        stream.fill();
        TokenStreamRewriter tokens = new TokenStreamRewriter(stream);
        tokens.insertBefore(0, "x");
        tokens.insertBefore(0, "y");
        tokens.replace(0, "z");
        String result = tokens.getText();
        String expecting = "yxzbc";
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    public void testReplaceThenInsertBeforeLastIndex()
    {
        LexerGrammar g = new LexerGrammar(
                                             "lexer grammar T;\n" +
                                             "A : 'a';\n" +
                                             "B : 'b';\n" +
                                             "C : 'c';\n");
        String input = "abc";
        LexerInterpreter lexEngine = g.createLexerInterpreter(new ANTLRInputStream(input));
        CommonTokenStream stream = new CommonTokenStream(lexEngine);
        stream.fill();
        TokenStreamRewriter tokens = new TokenStreamRewriter(stream);
        tokens.replace(2, "x");
        tokens.insertBefore(2, "y");
        String result = tokens.getText();
        String expecting = "abyx";
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    public void testInsertThenReplaceLastIndex()
    {
        LexerGrammar g = new LexerGrammar(
                                             "lexer grammar T;\n" +
                                             "A : 'a';\n" +
                                             "B : 'b';\n" +
                                             "C : 'c';\n");
        String input = "abc";
        LexerInterpreter lexEngine = g.createLexerInterpreter(new ANTLRInputStream(input));
        CommonTokenStream stream = new CommonTokenStream(lexEngine);
        stream.fill();
        TokenStreamRewriter tokens = new TokenStreamRewriter(stream);
        tokens.insertBefore(2, "y");
        tokens.replace(2, "x");
        String result = tokens.getText();
        String expecting = "abyx";
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    public void testReplaceThenInsertAfterLastIndex()
    {
        LexerGrammar g = new LexerGrammar(
                                             "lexer grammar T;\n" +
                                             "A : 'a';\n" +
                                             "B : 'b';\n" +
                                             "C : 'c';\n");
        String input = "abc";
        LexerInterpreter lexEngine = g.createLexerInterpreter(new ANTLRInputStream(input));
        CommonTokenStream stream = new CommonTokenStream(lexEngine);
        stream.fill();
        TokenStreamRewriter tokens = new TokenStreamRewriter(stream);
        tokens.replace(2, "x");
        tokens.insertAfter(2, "y");
        String result = tokens.getText();
        String expecting = "abxy";
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    public void testReplaceRangeThenInsertAtLeftEdge()
    {
        LexerGrammar g = new LexerGrammar(
                                             "lexer grammar T;\n" +
                                             "A : 'a';\n" +
                                             "B : 'b';\n" +
                                             "C : 'c';\n");
        String input = "abcccba";
        LexerInterpreter lexEngine = g.createLexerInterpreter(new ANTLRInputStream(input));
        CommonTokenStream stream = new CommonTokenStream(lexEngine);
        stream.fill();
        TokenStreamRewriter tokens = new TokenStreamRewriter(stream);
        tokens.replace(2, 4, "x");
        tokens.insertBefore(2, "y");
        String result = tokens.getText();
        String expecting = "abyxba";
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    public void testReplaceRangeThenInsertAtRightEdge()
    {
        LexerGrammar g = new LexerGrammar(
                                             "lexer grammar T;\n" +
                                             "A : 'a';\n" +
                                             "B : 'b';\n" +
                                             "C : 'c';\n");
        String input = "abcccba";
        LexerInterpreter lexEngine = g.createLexerInterpreter(new ANTLRInputStream(input));
        CommonTokenStream stream = new CommonTokenStream(lexEngine);
        stream.fill();
        TokenStreamRewriter tokens = new TokenStreamRewriter(stream);
        tokens.replace(2, 4, "x");
        tokens.insertBefore(4, "y");
        stream.fill(); // no effect; within range of a replace
        Exception exc = null;
        try
        {
            tokens.getText();
        }
        catch (ArgumentException iae)
        {
            exc = iae;
        }
        String expecting = "insert op <InsertBeforeOp@[@4,4:4='c',<3>,1:4]:\"y\"> within boundaries of previous <ReplaceOp@[@2,2:2='c',<3>,1:2]..[@4,4:4='c',<3>,1:4]:\"x\">";
        Assert.IsNotNull(exc);
        Assert.AreEqual(expecting, exc.Message);
    }

    [TestMethod]
    public void testReplaceRangeThenInsertAfterRightEdge()
    {
        LexerGrammar g = new LexerGrammar(
                                             "lexer grammar T;\n" +
                                             "A : 'a';\n" +
                                             "B : 'b';\n" +
                                             "C : 'c';\n");
        String input = "abcccba";
        LexerInterpreter lexEngine = g.createLexerInterpreter(new ANTLRInputStream(input));
        CommonTokenStream stream = new CommonTokenStream(lexEngine);
        stream.fill();
        TokenStreamRewriter tokens = new TokenStreamRewriter(stream);
        tokens.replace(2, 4, "x");
        tokens.insertAfter(4, "y");
        String result = tokens.getText();
        String expecting = "abxyba";
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    public void testReplaceAll()
    {
        LexerGrammar g = new LexerGrammar(
                                             "lexer grammar T;\n" +
                                             "A : 'a';\n" +
                                             "B : 'b';\n" +
                                             "C : 'c';\n");
        String input = "abcccba";
        LexerInterpreter lexEngine = g.createLexerInterpreter(new ANTLRInputStream(input));
        CommonTokenStream stream = new CommonTokenStream(lexEngine);
        stream.fill();
        TokenStreamRewriter tokens = new TokenStreamRewriter(stream);
        tokens.replace(0, 6, "x");
        String result = tokens.getText();
        String expecting = "x";
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    public void testReplaceSubsetThenFetch()
    {
        LexerGrammar g = new LexerGrammar(
                                             "lexer grammar T;\n" +
                                             "A : 'a';\n" +
                                             "B : 'b';\n" +
                                             "C : 'c';\n");
        String input = "abcccba";
        LexerInterpreter lexEngine = g.createLexerInterpreter(new ANTLRInputStream(input));
        CommonTokenStream stream = new CommonTokenStream(lexEngine);
        stream.fill();
        TokenStreamRewriter tokens = new TokenStreamRewriter(stream);
        tokens.replace(2, 4, "xyz");
        String result = tokens.getText(Interval.of(0, 6));
        String expecting = "abxyzba";
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    public void testReplaceThenReplaceSuperset()
    {
        LexerGrammar g = new LexerGrammar(
                                             "lexer grammar T;\n" +
                                             "A : 'a';\n" +
                                             "B : 'b';\n" +
                                             "C : 'c';\n");
        String input = "abcccba";
        LexerInterpreter lexEngine = g.createLexerInterpreter(new ANTLRInputStream(input));
        CommonTokenStream stream = new CommonTokenStream(lexEngine);
        stream.fill();
        TokenStreamRewriter tokens = new TokenStreamRewriter(stream);
        tokens.replace(2, 4, "xyz");
        tokens.replace(3, 5, "foo");
        stream.fill();
        // overlaps, error
        Exception exc = null;
        try
        {
            tokens.getText();
        }
        catch (ArgumentException iae)
        {
            exc = iae;
        }
        String expecting = "replace op boundaries of <ReplaceOp@[@3,3:3='c',<3>,1:3]..[@5,5:5='b',<2>,1:5]:\"foo\"> overlap with previous <ReplaceOp@[@2,2:2='c',<3>,1:2]..[@4,4:4='c',<3>,1:4]:\"xyz\">";
        Assert.IsNotNull(exc);
        Assert.AreEqual(expecting, exc.Message);
    }

    [TestMethod]
    public void testReplaceThenReplaceLowerIndexedSuperset()
    {
        LexerGrammar g = new LexerGrammar(
                                             "lexer grammar T;\n" +
                                             "A : 'a';\n" +
                                             "B : 'b';\n" +
                                             "C : 'c';\n");
        String input = "abcccba";
        LexerInterpreter lexEngine = g.createLexerInterpreter(new ANTLRInputStream(input));
        CommonTokenStream stream = new CommonTokenStream(lexEngine);
        stream.fill();
        TokenStreamRewriter tokens = new TokenStreamRewriter(stream);
        tokens.replace(2, 4, "xyz");
        tokens.replace(1, 3, "foo");
        stream.fill();
        // overlap, error
        Exception exc = null;
        try
        {
            tokens.getText();
        }
        catch (ArgumentException iae)
        {
            exc = iae;
        }
        String expecting = "replace op boundaries of <ReplaceOp@[@1,1:1='b',<2>,1:1]..[@3,3:3='c',<3>,1:3]:\"foo\"> overlap with previous <ReplaceOp@[@2,2:2='c',<3>,1:2]..[@4,4:4='c',<3>,1:4]:\"xyz\">";
        Assert.IsNotNull(exc);
        Assert.AreEqual(expecting, exc.Message);
    }

    [TestMethod]
    public void testReplaceSingleMiddleThenOverlappingSuperset()
    {
        LexerGrammar g = new LexerGrammar(
                                             "lexer grammar T;\n" +
                                             "A : 'a';\n" +
                                             "B : 'b';\n" +
                                             "C : 'c';\n");
        String input = "abcba";
        LexerInterpreter lexEngine = g.createLexerInterpreter(new ANTLRInputStream(input));
        CommonTokenStream stream = new CommonTokenStream(lexEngine);
        stream.fill();
        TokenStreamRewriter tokens = new TokenStreamRewriter(stream);
        tokens.replace(2, 2, "xyz");
        tokens.replace(0, 3, "foo");
        String result = tokens.getText();
        String expecting = "fooa";
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    public void testCombineInserts()
    {
        LexerGrammar g = new LexerGrammar(
                                             "lexer grammar T;\n" +
                                             "A : 'a';\n" +
                                             "B : 'b';\n" +
                                             "C : 'c';\n");
        String input = "abc";
        LexerInterpreter lexEngine = g.createLexerInterpreter(new ANTLRInputStream(input));
        CommonTokenStream stream = new CommonTokenStream(lexEngine);
        stream.fill();
        TokenStreamRewriter tokens = new TokenStreamRewriter(stream);
        tokens.insertBefore(0, "x");
        tokens.insertBefore(0, "y");
        String result = tokens.getText();
        String expecting = "yxabc";
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    public void testCombine3Inserts()
    {
        LexerGrammar g = new LexerGrammar(
                                             "lexer grammar T;\n" +
                                             "A : 'a';\n" +
                                             "B : 'b';\n" +
                                             "C : 'c';\n");
        String input = "abc";
        LexerInterpreter lexEngine = g.createLexerInterpreter(new ANTLRInputStream(input));
        CommonTokenStream stream = new CommonTokenStream(lexEngine);
        stream.fill();
        TokenStreamRewriter tokens = new TokenStreamRewriter(stream);
        tokens.insertBefore(1, "x");
        tokens.insertBefore(0, "y");
        tokens.insertBefore(1, "z");
        String result = tokens.getText();
        String expecting = "yazxbc";
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    public void testCombineInsertOnLeftWithReplace()
    {
        LexerGrammar g = new LexerGrammar(
                                             "lexer grammar T;\n" +
                                             "A : 'a';\n" +
                                             "B : 'b';\n" +
                                             "C : 'c';\n");
        String input = "abc";
        LexerInterpreter lexEngine = g.createLexerInterpreter(new ANTLRInputStream(input));
        CommonTokenStream stream = new CommonTokenStream(lexEngine);
        stream.fill();
        TokenStreamRewriter tokens = new TokenStreamRewriter(stream);
        tokens.replace(0, 2, "foo");
        tokens.insertBefore(0, "z");
        stream.fill();
        // combine with left edge of rewrite
        String result = tokens.getText();
        String expecting = "zfoo";
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    public void testCombineInsertOnLeftWithDelete()
    {
        LexerGrammar g = new LexerGrammar(
                                             "lexer grammar T;\n" +
                                             "A : 'a';\n" +
                                             "B : 'b';\n" +
                                             "C : 'c';\n");
        String input = "abc";
        LexerInterpreter lexEngine = g.createLexerInterpreter(new ANTLRInputStream(input));
        CommonTokenStream stream = new CommonTokenStream(lexEngine);
        stream.fill();
        TokenStreamRewriter tokens = new TokenStreamRewriter(stream);
        tokens.delete(0, 2);
        tokens.insertBefore(0, "z");
        stream.fill();
        // combine with left edge of rewrite
        String result = tokens.getText();
        String expecting = "z";
        stream.fill();
        // make sure combo is not znull
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    public void testDisjointInserts()
    {
        LexerGrammar g = new LexerGrammar(
                                             "lexer grammar T;\n" +
                                             "A : 'a';\n" +
                                             "B : 'b';\n" +
                                             "C : 'c';\n");
        String input = "abc";
        LexerInterpreter lexEngine = g.createLexerInterpreter(new ANTLRInputStream(input));
        CommonTokenStream stream = new CommonTokenStream(lexEngine);
        stream.fill();
        TokenStreamRewriter tokens = new TokenStreamRewriter(stream);
        tokens.insertBefore(1, "x");
        tokens.insertBefore(2, "y");
        tokens.insertBefore(0, "z");
        String result = tokens.getText();
        String expecting = "zaxbyc";
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    public void testOverlappingReplace()
    {
        LexerGrammar g = new LexerGrammar(
                                             "lexer grammar T;\n" +
                                             "A : 'a';\n" +
                                             "B : 'b';\n" +
                                             "C : 'c';\n");
        String input = "abcc";
        LexerInterpreter lexEngine = g.createLexerInterpreter(new ANTLRInputStream(input));
        CommonTokenStream stream = new CommonTokenStream(lexEngine);
        stream.fill();
        TokenStreamRewriter tokens = new TokenStreamRewriter(stream);
        tokens.replace(1, 2, "foo");
        tokens.replace(0, 3, "bar");
        stream.fill();
        // wipes prior nested replace
        String result = tokens.getText();
        String expecting = "bar";
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    public void testOverlappingReplace2()
    {
        LexerGrammar g = new LexerGrammar(
                                             "lexer grammar T;\n" +
                                             "A : 'a';\n" +
                                             "B : 'b';\n" +
                                             "C : 'c';\n");
        String input = "abcc";
        LexerInterpreter lexEngine = g.createLexerInterpreter(new ANTLRInputStream(input));
        CommonTokenStream stream = new CommonTokenStream(lexEngine);
        stream.fill();
        TokenStreamRewriter tokens = new TokenStreamRewriter(stream);
        tokens.replace(0, 3, "bar");
        tokens.replace(1, 2, "foo");
        stream.fill();
        // cannot split earlier replace
        Exception exc = null;
        try
        {
            tokens.getText();
        }
        catch (ArgumentException iae)
        {
            exc = iae;
        }
        String expecting = "replace op boundaries of <ReplaceOp@[@1,1:1='b',<2>,1:1]..[@2,2:2='c',<3>,1:2]:\"foo\"> overlap with previous <ReplaceOp@[@0,0:0='a',<1>,1:0]..[@3,3:3='c',<3>,1:3]:\"bar\">";
        Assert.IsNotNull(exc);
        Assert.AreEqual(expecting, exc.Message);
    }

    [TestMethod]
    public void testOverlappingReplace3()
    {
        LexerGrammar g = new LexerGrammar(
                                             "lexer grammar T;\n" +
                                             "A : 'a';\n" +
                                             "B : 'b';\n" +
                                             "C : 'c';\n");
        String input = "abcc";
        LexerInterpreter lexEngine = g.createLexerInterpreter(new ANTLRInputStream(input));
        CommonTokenStream stream = new CommonTokenStream(lexEngine);
        stream.fill();
        TokenStreamRewriter tokens = new TokenStreamRewriter(stream);
        tokens.replace(1, 2, "foo");
        tokens.replace(0, 2, "bar");
        stream.fill();
        // wipes prior nested replace
        String result = tokens.getText();
        String expecting = "barc";
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    public void testOverlappingReplace4()
    {
        LexerGrammar g = new LexerGrammar(
                                             "lexer grammar T;\n" +
                                             "A : 'a';\n" +
                                             "B : 'b';\n" +
                                             "C : 'c';\n");
        String input = "abcc";
        LexerInterpreter lexEngine = g.createLexerInterpreter(new ANTLRInputStream(input));
        CommonTokenStream stream = new CommonTokenStream(lexEngine);
        stream.fill();
        TokenStreamRewriter tokens = new TokenStreamRewriter(stream);
        tokens.replace(1, 2, "foo");
        tokens.replace(1, 3, "bar");
        stream.fill();
        // wipes prior nested replace
        String result = tokens.getText();
        String expecting = "abar";
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    public void testDropIdenticalReplace()
    {
        LexerGrammar g = new LexerGrammar(
                                             "lexer grammar T;\n" +
                                             "A : 'a';\n" +
                                             "B : 'b';\n" +
                                             "C : 'c';\n");
        String input = "abcc";
        LexerInterpreter lexEngine = g.createLexerInterpreter(new ANTLRInputStream(input));
        CommonTokenStream stream = new CommonTokenStream(lexEngine);
        stream.fill();
        TokenStreamRewriter tokens = new TokenStreamRewriter(stream);
        tokens.replace(1, 2, "foo");
        tokens.replace(1, 2, "foo");
        stream.fill();
        // drop previous, identical
        String result = tokens.getText();
        String expecting = "afooc";
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    public void testDropPrevCoveredInsert()
    {
        LexerGrammar g = new LexerGrammar(
                                             "lexer grammar T;\n" +
                                             "A : 'a';\n" +
                                             "B : 'b';\n" +
                                             "C : 'c';\n");
        String input = "abc";
        LexerInterpreter lexEngine = g.createLexerInterpreter(new ANTLRInputStream(input));
        CommonTokenStream stream = new CommonTokenStream(lexEngine);
        stream.fill();
        TokenStreamRewriter tokens = new TokenStreamRewriter(stream);
        tokens.insertBefore(1, "foo");
        tokens.replace(1, 2, "foo");
        stream.fill();
        // kill prev insert
        String result = tokens.getText();
        String expecting = "afoofoo";
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    public void testLeaveAloneDisjointInsert()
    {
        LexerGrammar g = new LexerGrammar(
                                             "lexer grammar T;\n" +
                                             "A : 'a';\n" +
                                             "B : 'b';\n" +
                                             "C : 'c';\n");
        String input = "abcc";
        LexerInterpreter lexEngine = g.createLexerInterpreter(new ANTLRInputStream(input));
        CommonTokenStream stream = new CommonTokenStream(lexEngine);
        stream.fill();
        TokenStreamRewriter tokens = new TokenStreamRewriter(stream);
        tokens.insertBefore(1, "x");
        tokens.replace(2, 3, "foo");
        String result = tokens.getText();
        String expecting = "axbfoo";
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    public void testLeaveAloneDisjointInsert2()
    {
        LexerGrammar g = new LexerGrammar(
                                             "lexer grammar T;\n" +
                                             "A : 'a';\n" +
                                             "B : 'b';\n" +
                                             "C : 'c';\n");
        String input = "abcc";
        LexerInterpreter lexEngine = g.createLexerInterpreter(new ANTLRInputStream(input));
        CommonTokenStream stream = new CommonTokenStream(lexEngine);
        stream.fill();
        TokenStreamRewriter tokens = new TokenStreamRewriter(stream);
        tokens.replace(2, 3, "foo");
        tokens.insertBefore(1, "x");
        String result = tokens.getText();
        String expecting = "axbfoo";
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    public void testInsertBeforeTokenThenDeleteThatToken()
    {
        LexerGrammar g = new LexerGrammar(
                                             "lexer grammar T;\n" +
                                             "A : 'a';\n" +
                                             "B : 'b';\n" +
                                             "C : 'c';\n");
        String input = "abc";
        LexerInterpreter lexEngine = g.createLexerInterpreter(new ANTLRInputStream(input));
        CommonTokenStream stream = new CommonTokenStream(lexEngine);
        stream.fill();
        TokenStreamRewriter tokens = new TokenStreamRewriter(stream);
        tokens.insertBefore(2, "y");
        tokens.delete(2);
        String result = tokens.getText();
        String expecting = "aby";
        Assert.AreEqual(expecting, result);
    }

    // Test Fix for https://github.com/antlr/antlr4/issues/550
    [TestMethod]
    public void testDistinguishBetweenInsertAfterAndInsertBeforeToPreserverOrder()
    {
        LexerGrammar g = new LexerGrammar(
                                             "lexer grammar T;\n" +
                                             "A : 'a';\n" +
                                             "B : 'b';\n" +
                                             "C : 'c';\n");
        String input = "aa";
        LexerInterpreter lexEngine = g.createLexerInterpreter(new ANTLRInputStream(input));
        CommonTokenStream stream = new CommonTokenStream(lexEngine);
        stream.fill();
        TokenStreamRewriter tokens = new TokenStreamRewriter(stream);
        tokens.insertBefore(0, "<b>");
        tokens.insertAfter(0, "</b>");
        tokens.insertBefore(1, "<b>");
        tokens.insertAfter(1, "</b>");
        String result = tokens.getText();
        String expecting = "<b>a</b><b>a</b>"; // fails with <b>a<b></b>a</b>"
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    public void testDistinguishBetweenInsertAfterAndInsertBeforeToPreserverOrder2()
    {
        LexerGrammar g = new LexerGrammar(
                                             "lexer grammar T;\n" +
                                             "A : 'a';\n" +
                                             "B : 'b';\n" +
                                             "C : 'c';\n");
        String input = "aa";
        LexerInterpreter lexEngine = g.createLexerInterpreter(new ANTLRInputStream(input));
        CommonTokenStream stream = new CommonTokenStream(lexEngine);
        stream.fill();
        TokenStreamRewriter tokens = new TokenStreamRewriter(stream);
        tokens.insertBefore(0, "<p>");
        tokens.insertBefore(0, "<b>");
        tokens.insertAfter(0, "</p>");
        tokens.insertAfter(0, "</b>");
        tokens.insertBefore(1, "<b>");
        tokens.insertAfter(1, "</b>");
        String result = tokens.getText();
        String expecting = "<b><p>a</p></b><b>a</b>";
        Assert.AreEqual(expecting, result);
    }

    // Test Fix for https://github.com/antlr/antlr4/issues/550
    [TestMethod]
    public void testPreservesOrderOfContiguousInserts()
    {
        LexerGrammar g = new LexerGrammar(
                                             "lexer grammar T;\n" +
                                             "A : 'a';\n" +
                                             "B : 'b';\n" +
                                             "C : 'c';\n");
        String input = "ab";
        LexerInterpreter lexEngine = g.createLexerInterpreter(new ANTLRInputStream(input));
        CommonTokenStream stream = new CommonTokenStream(lexEngine);
        stream.fill();
        TokenStreamRewriter tokens = new TokenStreamRewriter(stream);
        tokens.insertBefore(0, "<p>");
        tokens.insertBefore(0, "<b>");
        tokens.insertBefore(0, "<div>");
        tokens.insertAfter(0, "</p>");
        tokens.insertAfter(0, "</b>");
        tokens.insertAfter(0, "</div>");
        tokens.insertBefore(1, "!");
        String result = tokens.getText();
        String expecting = "<div><b><p>a</p></b></div>!b";
        Assert.AreEqual(expecting, result);
    }

}
