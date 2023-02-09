/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime;
using org.antlr.v4.runtime.dfa;
using org.antlr.v4.runtime.misc;
using org.antlr.v4.runtime.tree.pattern;
using org.antlr.v4.tool;
using System.Text;

namespace org.antlr.v4.test.tool;
//@SuppressWarnings("unused")
[TestClass]
public class TestUnbufferedCharStream
{
    [TestMethod]
    public void TestNoChar()
    {
        var input = CreateStream("");
        Assert.AreEqual(IntStream.EOF, input.LA(1));
        Assert.AreEqual(IntStream.EOF, input.LA(2));
    }

    /**
	 * The {@link IntStream} interface does not specify the behavior when the
	 * EOF symbol is consumed, but {@link UnbufferedCharStream} handles this
	 * particular case by throwing an {@link IllegalStateException}.
	 */
    [TestMethod]
    public void TestConsumeEOF()
    {
        var input = CreateStream("");
        Assert.AreEqual(IntStream.EOF, input.LA(1));

        //assertThrows(IllegalStateException, input::consume);
        Assert.ThrowsException<IllegalStateException>(() => input.Consume());

    }

    [TestMethod]
    public void TestNegativeSeek()
    {

        var input = CreateStream("");
        //assertThrows(typeof(ArgumentException), () => input.seek(-1));
        Assert.ThrowsException<ArgumentException>(() => input.Seek(-1));
    }

    [TestMethod]
    public void TestSeekPastEOF()
    {
        var input = CreateStream("");
        Assert.AreEqual(0, input.Index);
        input.Seek(1);
        Assert.AreEqual(0, input.Index);
    }

    /**
	 * The {@link IntStream} interface does not specify the behavior when marks
	 * are not released in the reversed order they were created, but
	 * {@link UnbufferedCharStream} handles this case by throwing an
	 * {@link IllegalStateException}.
	 */
    [TestMethod]
    public void TestMarkReleaseOutOfOrder()
    {
        var input = CreateStream("");
        int m1 = input.Mark();
        int m2 = input.Mark();
        //assertThrows(IllegalStateException, () => input.release(m1));
        Assert.ThrowsException<IllegalStateException>(() => input.Release(m1));

    }

    /**
	 * The {@link IntStream} interface does not specify the behavior when a mark
	 * is released twice, but {@link UnbufferedCharStream} handles this case by
	 * throwing an {@link IllegalStateException}.
	 */
    [TestMethod]
    public void TestMarkReleasedTwice()
    {
        var input = CreateStream("");
        int m1 = input.Mark();
        input.Release(m1);
        //assertThrows(IllegalStateException, () => input.release(m1));
        Assert.ThrowsException<IllegalStateException>(() => input.Release(m1));
    }

    /**
	 * The {@link IntStream} interface does not specify the behavior when a mark
	 * is released twice, but {@link UnbufferedCharStream} handles this case by
	 * throwing an {@link IllegalStateException}.
	 */
    [TestMethod]
    public void TestNestedMarkReleasedTwice()
    {
        var input = CreateStream("");
        int m1 = input.Mark();
        int m2 = input.Mark();
        input.Release(m2);
        //assertThrows(IllegalStateException, () => input.release(m2));
        Assert.ThrowsException<IllegalStateException>(() => input.Release(m2));
    }

    /**
	 * It is not valid to pass a mark to {@link IntStream#seek}, but
	 * {@link UnbufferedCharStream} creates marks in such a way that this
	 * invalid usage results in an {@link ArgumentException}.
	 */
    [TestMethod]
    public void TestMarkPassedToSeek()
    {
        var input = CreateStream("");
        int m1 = input.Mark();
        //assertThrows(ArgumentException, () => input.seek(m1));
        Assert.ThrowsException<ArgumentException>(() => input.Seek(m1));
    }

    [TestMethod]
    public void TestSeekBeforeBufferStart()
    {
        var input = CreateStream("xyz");
        input.Consume();
        int m1 = input.Mark();
        Assert.AreEqual(1, input.Index);
        input.Consume();
        //assertThrows(ArgumentException, () => input.seek(0));
        Assert.ThrowsException<ArgumentException>(() => input.Seek(0));
    }

    [TestMethod]
    public void TestGetTextBeforeBufferStart()
    {
        var input = CreateStream("xyz");
        input.Consume();
        int m1 = input.Mark();
        Assert.AreEqual(1, input.Index);
        //assertThrows(UnsupportedOperationException, () => input.getText(new Interval(0, 1)));
        Assert.ThrowsException<ArgumentException>(() => input.GetText(new Interval(0, 1)));
    }

    [TestMethod]
    public void TestGetTextInMarkedRange()
    {
        var input = CreateStream("xyz");
        input.Consume();
        int m1 = input.Mark();
        Assert.AreEqual(1, input.Index);
        input.Consume();
        input.Consume();
        Assert.AreEqual("yz", input.GetText(new Interval(1, 2)));
    }

    [TestMethod]
    public void TestLastChar()
    {
        var input = CreateStream("abcdef");

        input.Consume();
        Assert.AreEqual('a', input.LA(-1));

        int m1 = input.Mark();
        input.Consume();
        input.Consume();
        input.Consume();
        Assert.AreEqual('d', input.LA(-1));

        input.Seek(2);
        Assert.AreEqual('b', input.LA(-1));

        input.Release(m1);
        input.Seek(3);
        Assert.AreEqual('c', input.LA(-1));
        // this special case is not required by the IntStream interface, but
        // UnbufferedCharStream allows it so we have to make sure the resulting
        // state is consistent
        input.Seek(2);
        Assert.AreEqual('b', input.LA(-1));
    }

    [TestMethod]
    public void Test1Char()
    {
        var input = CreateStream("x");
        Assert.AreEqual('x', input.LA(1));
        input.Consume();
        Assert.AreEqual(IntStream.EOF, input.LA(1));
        var r = input.GetRemainingBuffer();
        Assert.AreEqual("\uFFFF", r); // shouldn't include x
        Assert.AreEqual("\uFFFF", input.Buffer); // whole buffer
    }

    [TestMethod]
    public void Test2Char()
    {
        var input = CreateStream("xy");
        Assert.AreEqual('x', input.LA(1));
        input.Consume();
        Assert.AreEqual('y', input.LA(1));
        Assert.AreEqual("y", input.GetRemainingBuffer()); // shouldn't include x
        Assert.AreEqual("y", input.Buffer);
        input.Consume();
        Assert.AreEqual(IntStream.EOF, input.LA(1));
        Assert.AreEqual("\uFFFF", input.Buffer);
    }

    [TestMethod]
    public void Test2CharAhead()
    {
        var input = CreateStream("xy");
        Assert.AreEqual('x', input.LA(1));
        Assert.AreEqual('y', input.LA(2));
        Assert.AreEqual(IntStream.EOF, input.LA(3));
    }

    [TestMethod]
    public void TestBufferExpand()
    {
        var input = CreateStream("01234", 2);
        Assert.AreEqual('0', input.LA(1));
        Assert.AreEqual('1', input.LA(2));
        Assert.AreEqual('2', input.LA(3));
        Assert.AreEqual('3', input.LA(4));
        Assert.AreEqual('4', input.LA(5));
        Assert.AreEqual("01234", input.Buffer);
        Assert.AreEqual(IntStream.EOF, input.LA(6));
    }

    [TestMethod]
    public void TestBufferWrapSize1()
    {
        var input = CreateStream("01234", 1);
        Assert.AreEqual('0', input.LA(1));
        input.Consume();
        Assert.AreEqual('1', input.LA(1));
        input.Consume();
        Assert.AreEqual('2', input.LA(1));
        input.Consume();
        Assert.AreEqual('3', input.LA(1));
        input.Consume();
        Assert.AreEqual('4', input.LA(1));
        input.Consume();
        Assert.AreEqual(IntStream.EOF, input.LA(1));
    }

    [TestMethod]
    public void TestBufferWrapSize2()
    {
        var input = CreateStream("01234", 2);
        Assert.AreEqual('0', input.LA(1));
        input.Consume();
        Assert.AreEqual('1', input.LA(1));
        input.Consume();
        Assert.AreEqual('2', input.LA(1));
        input.Consume();
        Assert.AreEqual('3', input.LA(1));
        input.Consume();
        Assert.AreEqual('4', input.LA(1));
        input.Consume();
        Assert.AreEqual(IntStream.EOF, input.LA(1));
    }

    [TestMethod]
    public void Test1Mark()
    {
        var input = CreateStream("xyz");
        int m = input.Mark();
        Assert.AreEqual('x', input.LA(1));
        Assert.AreEqual('y', input.LA(2));
        Assert.AreEqual('z', input.LA(3));
        input.Release(m);
        Assert.AreEqual(IntStream.EOF, input.LA(4));
        Assert.AreEqual("xyz\uFFFF", input.Buffer);
    }

    [TestMethod]
    public void Test1MarkWithConsumesInSequence()
    {
        var input = CreateStream("xyz");
        int m = input.Mark();
        input.Consume(); // x, moves to y
        input.Consume(); // y
        input.Consume(); // z, moves to EOF
        Assert.AreEqual(IntStream.EOF, input.LA(1));
        Assert.AreEqual("xyz\uFFFF", input.Buffer);
        input.Release(m); // wipes buffer
        Assert.AreEqual("\uFFFF", input.Buffer);
    }

    [TestMethod]
    public void Test2Mark()
    {
        var input = CreateStream("xyz", 100);
        Assert.AreEqual('x', input.LA(1));
        input.Consume(); // reset buffer index (p) to 0
        int m1 = input.Mark();
        Assert.AreEqual('y', input.LA(1));
        input.Consume();
        int m2 = input.Mark();
        Assert.AreEqual("yz", input.Buffer);
        input.Release(m2); // drop to 1 marker
        input.Consume();
        input.Release(m1); // shifts remaining char to beginning
        Assert.AreEqual(IntStream.EOF, input.LA(1));
        Assert.AreEqual("\uFFFF", input.Buffer);
    }

    [TestMethod]
    public void TestAFewTokens()
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
        var input = CreateStream("x = 302 * 91 + 20234234 * 0;");
        var lexEngine = g.CreateLexerInterpreter(input);
        // copy text into tokens from char stream
        lexEngine.        // copy text into tokens from char stream
        TokenFactory = new CommonTokenFactory(true);
        var tokens = new CommonTokenStream(lexEngine);
        var result = tokens.LT(1).Text;
        var expecting = "x";
        Assert.AreEqual(expecting, result);
        tokens.Fill();
        expecting =
            "[[@0,0:0='x',<1>,1:0], [@1,1:1=' ',<7>,1:1], [@2,2:2='=',<4>,1:2]," +
            " [@3,3:3=' ',<7>,1:3], [@4,4:6='302',<2>,1:4], [@5,7:7=' ',<7>,1:7]," +
            " [@6,8:8='*',<6>,1:8], [@7,9:9=' ',<7>,1:9], [@8,10:11='91',<2>,1:10]," +
            " [@9,12:12=' ',<7>,1:12], [@10,13:13='+',<5>,1:13], [@11,14:14=' ',<7>,1:14]," +
            " [@12,15:22='20234234',<2>,1:15], [@13,23:23=' ',<7>,1:23]," +
            " [@14,24:24='*',<6>,1:24], [@15,25:25=' ',<7>,1:25], [@16,26:26='0',<2>,1:26]," +
            " [@17,27:27=';',<3>,1:27], [@18,28:27='',<-1>,1:28]]";
        Assert.AreEqual(expecting, tokens.GetTokens().ToString());
    }

    [TestMethod]
    public void TestUnicodeSMP()
    {
        var input = CreateStream("\uD83C\uDF0E");
        Assert.AreEqual(0x1F30E, input.LA(1));
        Assert.AreEqual("\uD83C\uDF0E", input.Buffer);
        input.Consume();
        Assert.AreEqual(IntStream.EOF, input.LA(1));
        Assert.AreEqual("\uFFFF", input.Buffer);
    }

    [TestMethod]
    public void TestDanglingHighSurrogateAtEOFThrows()
    {
        //assertThrows(RuntimeException, () => createStream("\uD83C"));
        Assert.ThrowsException<RuntimeException>(() => CreateStream("\uD83C"));
    }

    [TestMethod]
    public void TestDanglingHighSurrogateThrows()
    {
        //assertThrows(RuntimeException, () => createStream("\uD83C\u0123"));
        Assert.ThrowsException<RuntimeException>(() => CreateStream("\uD83C\u0123"));
    }

    [TestMethod]
    public void TestDanglingLowSurrogateThrows()
    {
        //assertThrows(RuntimeException, () => createStream("\uDF0E"));
        Assert.ThrowsException<RuntimeException>(() => CreateStream("\uDF0E"));
    }

    protected static TestingUnbufferedCharStream CreateStream(string text)
    {
        return new TestingUnbufferedCharStream(new StringReader(text));
    }

    protected static TestingUnbufferedCharStream CreateStream(string text, int bufferSize)
    {
        return new TestingUnbufferedCharStream(new StringReader(text), bufferSize);
    }

    protected class TestingUnbufferedCharStream : UnbufferedCharStream
    {

        public TestingUnbufferedCharStream(TextReader input) : base(input)
        {
        }

        public TestingUnbufferedCharStream(TextReader input, int bufferSize) : base(input, bufferSize)
        {
        }

        /** For testing.  What's in moving window into data stream from
		 *  current index, LA(1) or data[p], to end of buffer?
		 */
        public string GetRemainingBuffer()
        {
            if (n == 0) return "";
            int len = n;
            if (data[len - 1] == IntStream.EOF)
            {
                // Don't pass -1 to new string().
                return string.Join("", data[p..(len - 1)].Select(d => new Rune(d).ToString())) + "\uffff";
                //return new string(data,p,len-p-1) + "\uFFFF";
            }
            else
            {
                return string.Join("", data[p..].Select(d => new Rune(d).ToString()));
            }
        }

        /** For testing.  What's in moving window buffer into data stream.
		 *  From 0..p-1 have been consume.
		 */
        public string Buffer
        {
            get
            {
                if (n == 0) return "";
                int len = n;
                // Don't pass -1 to new string().
                if (data[len - 1] == IntStream.EOF)
                {
                    // Don't pass -1 to new string().
                    return string.Join("", data[..(len - 1)].Select(d => new Rune(d).ToString())) + "\uffff";
                }
                else
                {
                    return string.Join("", data.Select(d => new Rune(d).ToString()));
                }
            }
        }
    }
}
