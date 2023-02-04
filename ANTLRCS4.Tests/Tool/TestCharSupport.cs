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
			CharSupport.GetANTLRCharLiteralForChar(-1));
		Assert.AreEqual("'\\n'",
			CharSupport.GetANTLRCharLiteralForChar('\n'));
		Assert.AreEqual("'\\\\'",
			CharSupport.GetANTLRCharLiteralForChar('\\'));
		Assert.AreEqual("'\\''",
			CharSupport.GetANTLRCharLiteralForChar('\''));
		Assert.AreEqual("'b'",
			CharSupport.GetANTLRCharLiteralForChar('b'));
		Assert.AreEqual("'\\uFFFF'",
			CharSupport.GetANTLRCharLiteralForChar(0xFFFF));
		Assert.AreEqual("'\\u{10FFFF}'",
			CharSupport.GetANTLRCharLiteralForChar(0x10FFFF));
	}

	[TestMethod]
	public void testGetCharValueFromGrammarCharLiteral() {
		Assert.AreEqual(-1,
			CharSupport.GetCharValueFromGrammarCharLiteral(null));
		Assert.AreEqual(-1,
			CharSupport.GetCharValueFromGrammarCharLiteral(""));
		Assert.AreEqual(-1,
			CharSupport.GetCharValueFromGrammarCharLiteral("b"));
		Assert.AreEqual(111,
			CharSupport.GetCharValueFromGrammarCharLiteral("foo"));
	}

	[TestMethod]
	public void testGetStringFromGrammarStringLiteral() {
		Assert.IsNull(CharSupport
			.GetStringFromGrammarStringLiteral("foo\\u{bbb"));
		Assert.IsNull(CharSupport
			.GetStringFromGrammarStringLiteral("foo\\u{[]bb"));
		Assert.IsNull(CharSupport
			.GetStringFromGrammarStringLiteral("foo\\u[]bb"));
		Assert.IsNull(CharSupport
			.GetStringFromGrammarStringLiteral("foo\\ubb"));

		Assert.AreEqual("oo»b", CharSupport
			.GetStringFromGrammarStringLiteral("foo\\u{bb}bb"));
	}

	[TestMethod]
	public void testGetCharValueFromCharInGrammarLiteral() {
		Assert.AreEqual(102,
			CharSupport.GetCharValueFromCharInGrammarLiteral("f"));

		Assert.AreEqual(-1,
			CharSupport.GetCharValueFromCharInGrammarLiteral("\' "));
		Assert.AreEqual(-1,
			CharSupport.GetCharValueFromCharInGrammarLiteral("\\ "));
		Assert.AreEqual(39,
			CharSupport.GetCharValueFromCharInGrammarLiteral("\\\'"));
		Assert.AreEqual(10,
			CharSupport.GetCharValueFromCharInGrammarLiteral("\\n"));

		Assert.AreEqual(-1,
			CharSupport.GetCharValueFromCharInGrammarLiteral("foobar"));
		Assert.AreEqual(4660,
			CharSupport.GetCharValueFromCharInGrammarLiteral("\\u1234"));
		Assert.AreEqual(18,
			CharSupport.GetCharValueFromCharInGrammarLiteral("\\u{12}"));

		Assert.AreEqual(-1,
			CharSupport.GetCharValueFromCharInGrammarLiteral("\\u{"));
		Assert.AreEqual(-1,
			CharSupport.GetCharValueFromCharInGrammarLiteral("foo"));
	}

	[TestMethod]
	public void testParseHexValue() {
		Assert.AreEqual(-1, CharSupport.ParseHexValue("foobar", -1, 3));
		Assert.AreEqual(-1, CharSupport.ParseHexValue("foobar", 1, -1));
		Assert.AreEqual(-1, CharSupport.ParseHexValue("foobar", 1, 3));
		Assert.AreEqual(35, CharSupport.ParseHexValue("123456", 1, 3));
	}

	[TestMethod]
	public void testCapitalize() {
		Assert.AreEqual("Foo", CharSupport.Capitalize("foo"));
	}

	[TestMethod]
	public void testGetIntervalSetEscapedString() {
		Assert.AreEqual("",
			CharSupport.GetIntervalSetEscapedString(new IntervalSet()));
		Assert.AreEqual("'\\u0000'",
			CharSupport.GetIntervalSetEscapedString(new IntervalSet(0)));
		Assert.AreEqual("'\\u0001'..'\\u0003'",
			CharSupport.GetIntervalSetEscapedString(new IntervalSet(3, 1, 2)));
	}

	[TestMethod]
	public void testGetRangeEscapedString() {
		Assert.AreEqual("'\\u0002'..'\\u0004'",
			CharSupport.GetRangeEscapedString(2, 4));
		Assert.AreEqual("'\\u0002'",
			CharSupport.GetRangeEscapedString(2, 2));
	}
}
