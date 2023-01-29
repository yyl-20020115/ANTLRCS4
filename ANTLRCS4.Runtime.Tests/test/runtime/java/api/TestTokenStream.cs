/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */
using org.antlr.v4.runtime;

namespace org.antlr.v4.test.runtime.java.api;

/**
 * This class contains tests for specific API functionality in {@link TokenStream} and derived types.
 */
[TestClass]
public class TestTokenStream {

	/**
	 * This is a targeted regression test for antlr/antlr4#1584 ({@link BufferedTokenStream} cannot be reused after EOF).
	 */
	[TestMethod]
	public void testBufferedTokenStreamReuseAfterFill() {
		CharStream firstInput = new ANTLRInputStream("A");
		BufferedTokenStream tokenStream = new BufferedTokenStream(
			new VisitorBasicLexer(firstInput));
		tokenStream.fill();
		Assert.AreEqual(2, tokenStream.size());
		Assert.AreEqual(VisitorBasicLexer.A, tokenStream.get(0).getType());
		Assert.AreEqual(Token.EOF, tokenStream.get(1).getType());

		CharStream secondInput = new ANTLRInputStream("AA");
		tokenStream.setTokenSource(new VisitorBasicLexer(secondInput));
		tokenStream.fill();
		Assert.AreEqual(3, tokenStream.size());
		Assert.AreEqual(VisitorBasicLexer.A, tokenStream.get(0).getType());
		Assert.AreEqual(VisitorBasicLexer.A, tokenStream.get(1).getType());
		Assert.AreEqual(Token.EOF, tokenStream.get(2).getType());
	}
}
