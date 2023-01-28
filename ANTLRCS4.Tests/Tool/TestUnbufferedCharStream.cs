/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime;
using org.antlr.v4.runtime.tree.pattern;
using org.antlr.v4.tool;

namespace org.antlr.v4.test.tool;
//@SuppressWarnings("unused")
[TestClass]
public class TestUnbufferedCharStream {
	[TestMethod] public void testNoChar() {
		CharStream input = createStream("");
		Assert.AreEqual(IntStream.EOF, input.LA(1));
		Assert.AreEqual(IntStream.EOF, input.LA(2));
	}

	/**
	 * The {@link IntStream} interface does not specify the behavior when the
	 * EOF symbol is consumed, but {@link UnbufferedCharStream} handles this
	 * particular case by throwing an {@link IllegalStateException}.
	 */
	[TestMethod]
	public void testConsumeEOF() {
		CharStream input = createStream("");
		Assert.AreEqual(IntStream.EOF, input.LA(1));
		assertThrows(IllegalStateException, input::consume);
	}

	[TestMethod]
	public void testNegativeSeek() {
		CharStream input = createStream("");
		assertThrows(typeof(ArgumentException), () => input.seek(-1));
	}

	[TestMethod]
	public void testSeekPastEOF() {
		CharStream input = createStream("");
		Assert.AreEqual(0, input.index());
		input.seek(1);
		Assert.AreEqual(0, input.index());
	}

	/**
	 * The {@link IntStream} interface does not specify the behavior when marks
	 * are not released in the reversed order they were created, but
	 * {@link UnbufferedCharStream} handles this case by throwing an
	 * {@link IllegalStateException}.
	 */
	[TestMethod]
	public void testMarkReleaseOutOfOrder() {
		CharStream input = createStream("");
		int m1 = input.mark();
		int m2 = input.mark();
		assertThrows(IllegalStateException, () => input.release(m1));
	}

	/**
	 * The {@link IntStream} interface does not specify the behavior when a mark
	 * is released twice, but {@link UnbufferedCharStream} handles this case by
	 * throwing an {@link IllegalStateException}.
	 */
	[TestMethod]
	public void testMarkReleasedTwice() {
		CharStream input = createStream("");
		int m1 = input.mark();
		input.release(m1);
		assertThrows(IllegalStateException, () => input.release(m1));
	}

	/**
	 * The {@link IntStream} interface does not specify the behavior when a mark
	 * is released twice, but {@link UnbufferedCharStream} handles this case by
	 * throwing an {@link IllegalStateException}.
	 */
	[TestMethod]
	public void testNestedMarkReleasedTwice() {
		CharStream input = createStream("");
		int m1 = input.mark();
		int m2 = input.mark();
		input.release(m2);
		assertThrows(IllegalStateException, () => input.release(m2));
	}

	/**
	 * It is not valid to pass a mark to {@link IntStream#seek}, but
	 * {@link UnbufferedCharStream} creates marks in such a way that this
	 * invalid usage results in an {@link ArgumentException}.
	 */
	[TestMethod]
	public void testMarkPassedToSeek() {
		CharStream input = createStream("");
		int m1 = input.mark();
		assertThrows(ArgumentException, () => input.seek(m1));
	}

	[TestMethod]
	public void testSeekBeforeBufferStart() {
		CharStream input = createStream("xyz");
		input.consume();
		int m1 = input.mark();
		Assert.AreEqual(1, input.index());
		input.consume();
		assertThrows(ArgumentException, () => input.seek(0));
	}

	[TestMethod]
	public void testGetTextBeforeBufferStart() {
		CharStream input = createStream("xyz");
		input.consume();
		int m1 = input.mark();
		Assert.AreEqual(1, input.index());
		assertThrows(UnsupportedOperationException, () => input.getText(new Interval(0, 1)));
	}

	[TestMethod]
	public void testGetTextInMarkedRange() {
		CharStream input = createStream("xyz");
		input.consume();
		int m1 = input.mark();
		Assert.AreEqual(1, input.index());
		input.consume();
		input.consume();
		Assert.AreEqual("yz", input.getText(new Interval(1, 2)));
	}

	[TestMethod]
	public void testLastChar() {
		CharStream input = createStream("abcdef");

		input.consume();
		Assert.AreEqual('a', input.LA(-1));

		int m1 = input.mark();
		input.consume();
		input.consume();
		input.consume();
		Assert.AreEqual('d', input.LA(-1));

		input.seek(2);
		Assert.AreEqual('b', input.LA(-1));

		input.release(m1);
		input.seek(3);
		Assert.AreEqual('c', input.LA(-1));
		// this special case is not required by the IntStream interface, but
		// UnbufferedCharStream allows it so we have to make sure the resulting
		// state is consistent
		input.seek(2);
		Assert.AreEqual('b', input.LA(-1));
	}

	[TestMethod] public void test1Char(){
		TestingUnbufferedCharStream input = createStream("x");
		Assert.AreEqual('x', input.LA(1));
		input.consume();
		Assert.AreEqual(IntStream.EOF, input.LA(1));
		String r = input.getRemainingBuffer();
		Assert.AreEqual("\uFFFF", r); // shouldn't include x
		Assert.AreEqual("\uFFFF", input.getBuffer()); // whole buffer
	}

	[TestMethod] public void test2Char(){
		TestingUnbufferedCharStream input = createStream("xy");
		Assert.AreEqual('x', input.LA(1));
		input.consume();
		Assert.AreEqual('y', input.LA(1));
		Assert.AreEqual("y", input.getRemainingBuffer()); // shouldn't include x
		Assert.AreEqual("y", input.getBuffer());
		input.consume();
		Assert.AreEqual(IntStream.EOF, input.LA(1));
		Assert.AreEqual("\uFFFF", input.getBuffer());
	}

    [TestMethod] public void test2CharAhead(){
   		CharStream input = createStream("xy");
   		Assert.AreEqual('x', input.LA(1));
   		Assert.AreEqual('y', input.LA(2));
   		Assert.AreEqual(IntStream.EOF, input.LA(3));
   	}

    [TestMethod] public void testBufferExpand(){
		TestingUnbufferedCharStream input = createStream("01234", 2);
   		Assert.AreEqual('0', input.LA(1));
        Assert.AreEqual('1', input.LA(2));
        Assert.AreEqual('2', input.LA(3));
        Assert.AreEqual('3', input.LA(4));
        Assert.AreEqual('4', input.LA(5));
		Assert.AreEqual("01234", input.getBuffer());
   		Assert.AreEqual(IntStream.EOF, input.LA(6));
   	}

    [TestMethod] public void testBufferWrapSize1(){
   		CharStream input = createStream("01234", 1);
        Assert.AreEqual('0', input.LA(1));
        input.consume();
        Assert.AreEqual('1', input.LA(1));
        input.consume();
        Assert.AreEqual('2', input.LA(1));
        input.consume();
        Assert.AreEqual('3', input.LA(1));
        input.consume();
        Assert.AreEqual('4', input.LA(1));
        input.consume();
   		Assert.AreEqual(IntStream.EOF, input.LA(1));
   	}

    [TestMethod] public void testBufferWrapSize2(){
   		CharStream input = createStream("01234", 2);
        Assert.AreEqual('0', input.LA(1));
        input.consume();
        Assert.AreEqual('1', input.LA(1));
        input.consume();
        Assert.AreEqual('2', input.LA(1));
        input.consume();
        Assert.AreEqual('3', input.LA(1));
        input.consume();
        Assert.AreEqual('4', input.LA(1));
        input.consume();
   		Assert.AreEqual(IntStream.EOF, input.LA(1));
   	}

	[TestMethod] public void test1Mark(){
		TestingUnbufferedCharStream input = createStream("xyz");
		int m = input.mark();
		Assert.AreEqual('x', input.LA(1));
		Assert.AreEqual('y', input.LA(2));
		Assert.AreEqual('z', input.LA(3));
		input.release(m);
		Assert.AreEqual(IntStream.EOF, input.LA(4));
		Assert.AreEqual("xyz\uFFFF", input.getBuffer());
	}

	[TestMethod] public void test1MarkWithConsumesInSequence(){
		TestingUnbufferedCharStream input = createStream("xyz");
		int m = input.mark();
		input.consume(); // x, moves to y
		input.consume(); // y
		input.consume(); // z, moves to EOF
		Assert.AreEqual(IntStream.EOF, input.LA(1));
		Assert.AreEqual("xyz\uFFFF", input.getBuffer());
		input.release(m); // wipes buffer
		Assert.AreEqual("\uFFFF", input.getBuffer());
	}

    [TestMethod] public void test2Mark(){
		TestingUnbufferedCharStream input = createStream("xyz", 100);
   		Assert.AreEqual('x', input.LA(1));
        input.consume(); // reset buffer index (p) to 0
        int m1 = input.mark();
   		Assert.AreEqual('y', input.LA(1));
        input.consume();
        int m2 = input.mark();
		Assert.AreEqual("yz", input.getBuffer());
        input.release(m2); // drop to 1 marker
        input.consume();
        input.release(m1); // shifts remaining char to beginning
   		Assert.AreEqual(IntStream.EOF, input.LA(1));
		Assert.AreEqual("\uFFFF", input.getBuffer());
   	}

    [TestMethod] public void testAFewTokens(){
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
		TestingUnbufferedCharStream input = createStream("x = 302 * 91 + 20234234 * 0;");
        LexerInterpreter lexEngine = g.createLexerInterpreter(input);
		// copy text into tokens from char stream
		lexEngine.setTokenFactory(new CommonTokenFactory(true));
		CommonTokenStream tokens = new CommonTokenStream(lexEngine);
        String result = tokens.LT(1).getText();
        String expecting = "x";
        Assert.AreEqual(expecting, result);
		tokens.fill();
		expecting =
			"[[@0,0:0='x',<1>,1:0], [@1,1:1=' ',<7>,1:1], [@2,2:2='=',<4>,1:2]," +
			" [@3,3:3=' ',<7>,1:3], [@4,4:6='302',<2>,1:4], [@5,7:7=' ',<7>,1:7]," +
			" [@6,8:8='*',<6>,1:8], [@7,9:9=' ',<7>,1:9], [@8,10:11='91',<2>,1:10]," +
			" [@9,12:12=' ',<7>,1:12], [@10,13:13='+',<5>,1:13], [@11,14:14=' ',<7>,1:14]," +
			" [@12,15:22='20234234',<2>,1:15], [@13,23:23=' ',<7>,1:23]," +
			" [@14,24:24='*',<6>,1:24], [@15,25:25=' ',<7>,1:25], [@16,26:26='0',<2>,1:26]," +
			" [@17,27:27=';',<3>,1:27], [@18,28:27='',<-1>,1:28]]";
		Assert.AreEqual(expecting, tokens.getTokens().ToString());
    }

	[TestMethod] public void testUnicodeSMP(){
		TestingUnbufferedCharStream input = createStream("\uD83C\uDF0E");
		Assert.AreEqual(0x1F30E, input.LA(1));
		Assert.AreEqual("\uD83C\uDF0E", input.getBuffer());
		input.consume();
		Assert.AreEqual(IntStream.EOF, input.LA(1));
		Assert.AreEqual("\uFFFF", input.getBuffer());
	}

	[TestMethod]
	public void testDanglingHighSurrogateAtEOFThrows() {
		assertThrows(RuntimeException, () => createStream("\uD83C"));
	}

	[TestMethod]
	public void testDanglingHighSurrogateThrows() {
		assertThrows(RuntimeException, () => createStream("\uD83C\u0123"));
	}

	[TestMethod]
	public void testDanglingLowSurrogateThrows() {
		assertThrows(RuntimeException, () => createStream("\uDF0E"));
	}

	protected static TestingUnbufferedCharStream createStream(String text) {
		return new TestingUnbufferedCharStream(new StringReader(text));
	}

	protected static TestingUnbufferedCharStream createStream(String text, int bufferSize) {
		return new TestingUnbufferedCharStream(new StringReader(text), bufferSize);
	}

	protected class TestingUnbufferedCharStream : UnbufferedCharStream {

		public TestingUnbufferedCharStream(TextReader input): base(input)
    {
			;
		}

		public TestingUnbufferedCharStream(TextReader input, int bufferSize): base(input, bufferSize)
        {
			;
		}

		/** For testing.  What's in moving window into data stream from
		 *  current index, LA(1) or data[p], to end of buffer?
		 */
		public String getRemainingBuffer() {
			if ( n==0 ) return "";
			int len = n;
			if (data[len-1] == IntStream.EOF) {
				// Don't pass -1 to new String().
				return new String(data,p,len-p-1) + "\uFFFF";
			} else {
				return new String(data,p,len-p);
			}
		}

		/** For testing.  What's in moving window buffer into data stream.
		 *  From 0..p-1 have been consume.
		 */
		public String getBuffer() {
			if ( n==0 ) return "";
			int len = n;
			// Don't pass -1 to new String().
			if (data[len-1] == IntStream.EOF) {
				// Don't pass -1 to new String().
				return new String(data,0,len-1) + "\uFFFF";
			} else {
				return new String(data,0,len);
			}
		}

	}
}
