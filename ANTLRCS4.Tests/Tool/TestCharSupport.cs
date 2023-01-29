/*
 * Copyright (c) 2012-2019 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.misc;
using org.antlr.v4.runtime.misc;

namespace org.antlr.v4.test.tool;

[TestClass]
public class TestCharSupport {
	[TestMethod]
	public void testGetANTLRCharLiteralForChar() {
		Assert.AreEqual("'<INVALID>'",
			CharSupport.getANTLRCharLiteralForChar(-1));
		Assert.AreEqual("'\\n'",
			CharSupport.getANTLRCharLiteralForChar('\n'));
		Assert.AreEqual("'\\\\'",
			CharSupport.getANTLRCharLiteralForChar('\\'));
		Assert.AreEqual("'\\''",
			CharSupport.getANTLRCharLiteralForChar('\''));
		Assert.AreEqual("'b'",
			CharSupport.getANTLRCharLiteralForChar('b'));
		Assert.AreEqual("'\\uFFFF'",
			CharSupport.getANTLRCharLiteralForChar(0xFFFF));
		Assert.AreEqual("'\\u{10FFFF}'",
			CharSupport.getANTLRCharLiteralForChar(0x10FFFF));
	}

	[TestMethod]
	public void testGetCharValueFromGrammarCharLiteral() {
		Assert.AreEqual(-1,
			CharSupport.getCharValueFromGrammarCharLiteral(null));
		Assert.AreEqual(-1,
			CharSupport.getCharValueFromGrammarCharLiteral(""));
		Assert.AreEqual(-1,
			CharSupport.getCharValueFromGrammarCharLiteral("b"));
		Assert.AreEqual(111,
			CharSupport.getCharValueFromGrammarCharLiteral("foo"));
	}

	[TestMethod]
	public void testGetStringFromGrammarStringLiteral() {
		Assert.IsNull(CharSupport
			.getStringFromGrammarStringLiteral("foo\\u{bbb"));
		Assert.IsNull(CharSupport
			.getStringFromGrammarStringLiteral("foo\\u{[]bb"));
		Assert.IsNull(CharSupport
			.getStringFromGrammarStringLiteral("foo\\u[]bb"));
		Assert.IsNull(CharSupport
			.getStringFromGrammarStringLiteral("foo\\ubb"));

		Assert.AreEqual("oo»b", CharSupport
			.getStringFromGrammarStringLiteral("foo\\u{bb}bb"));
	}

	[TestMethod]
	public void testGetCharValueFromCharInGrammarLiteral() {
		Assert.AreEqual(102,
			CharSupport.getCharValueFromCharInGrammarLiteral("f"));

		Assert.AreEqual(-1,
			CharSupport.getCharValueFromCharInGrammarLiteral("\' "));
		Assert.AreEqual(-1,
			CharSupport.getCharValueFromCharInGrammarLiteral("\\ "));
		Assert.AreEqual(39,
			CharSupport.getCharValueFromCharInGrammarLiteral("\\\'"));
		Assert.AreEqual(10,
			CharSupport.getCharValueFromCharInGrammarLiteral("\\n"));

		Assert.AreEqual(-1,
			CharSupport.getCharValueFromCharInGrammarLiteral("foobar"));
		Assert.AreEqual(4660,
			CharSupport.getCharValueFromCharInGrammarLiteral("\\u1234"));
		Assert.AreEqual(18,
			CharSupport.getCharValueFromCharInGrammarLiteral("\\u{12}"));

		Assert.AreEqual(-1,
			CharSupport.getCharValueFromCharInGrammarLiteral("\\u{"));
		Assert.AreEqual(-1,
			CharSupport.getCharValueFromCharInGrammarLiteral("foo"));
	}

	[TestMethod]
	public void testParseHexValue() {
		Assert.AreEqual(-1, CharSupport.parseHexValue("foobar", -1, 3));
		Assert.AreEqual(-1, CharSupport.parseHexValue("foobar", 1, -1));
		Assert.AreEqual(-1, CharSupport.parseHexValue("foobar", 1, 3));
		Assert.AreEqual(35, CharSupport.parseHexValue("123456", 1, 3));
	}

	[TestMethod]
	public void testCapitalize() {
		Assert.AreEqual("Foo", CharSupport.capitalize("foo"));
	}

	[TestMethod]
	public void testGetIntervalSetEscapedString() {
		Assert.AreEqual("",
			CharSupport.getIntervalSetEscapedString(new IntervalSet()));
		Assert.AreEqual("'\\u0000'",
			CharSupport.getIntervalSetEscapedString(new IntervalSet(0)));
		Assert.AreEqual("'\\u0001'..'\\u0003'",
			CharSupport.getIntervalSetEscapedString(new IntervalSet(3, 1, 2)));
	}

	[TestMethod]
	public void testGetRangeEscapedString() {
		Assert.AreEqual("'\\u0002'..'\\u0004'",
			CharSupport.getRangeEscapedString(2, 4));
		Assert.AreEqual("'\\u0002'",
			CharSupport.getRangeEscapedString(2, 2));
	}
}
