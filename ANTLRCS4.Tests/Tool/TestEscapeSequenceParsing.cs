/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.misc;
using org.antlr.v4.runtime.misc;
using static org.antlr.v4.misc.EscapeSequenceParsing;

namespace org.antlr.v4.test.tool;

[TestClass]
public class TestEscapeSequenceParsing {
	[TestMethod]
	public void testParseEmpty() {
		Assert.AreEqual(
				EscapeSequenceParsing.Result.Type.INVALID,
				EscapeSequenceParsing.parseEscape("", 0).type);
	}

	[TestMethod]
	public void testParseJustBackslash() {
		Assert.AreEqual(
				EscapeSequenceParsing.Result.Type.INVALID,
				EscapeSequenceParsing.parseEscape("\\", 0).type);
	}

	[TestMethod]
	public void testParseInvalidEscape() {
		Assert.AreEqual(
				EscapeSequenceParsing.Result.Type.INVALID,
				EscapeSequenceParsing.parseEscape("\\z", 0).type);
	}

	[TestMethod]
	public void testParseNewline() {
		Assert.AreEqual(
				new Result(Result.Type.CODE_POINT, '\n', IntervalSet.EMPTY_SET, 0,2),
				EscapeSequenceParsing.parseEscape("\\n", 0));
	}

	[TestMethod]
	public void testParseTab() {
		Assert.AreEqual(
				new Result(Result.Type.CODE_POINT, '\t', IntervalSet.EMPTY_SET, 0,2),
				EscapeSequenceParsing.parseEscape("\\t", 0));
	}

	[TestMethod]
	public void testParseUnicodeTooShort() {
		Assert.AreEqual(
				EscapeSequenceParsing.Result.Type.INVALID,
				EscapeSequenceParsing.parseEscape("\\uABC", 0).type);
	}

	[TestMethod]
	public void testParseUnicodeBMP() {
		Assert.AreEqual(
				new Result(Result.Type.CODE_POINT, 0xABCD, IntervalSet.EMPTY_SET, 0,6),
				EscapeSequenceParsing.parseEscape("\\uABCD", 0));
	}

	[TestMethod]
	public void testParseUnicodeSMPTooShort() {
		Assert.AreEqual(
				EscapeSequenceParsing.Result.Type.INVALID,
				EscapeSequenceParsing.parseEscape("\\u{}", 0).type);
	}

	[TestMethod]
	public void testParseUnicodeSMPMissingCloseBrace() {
		Assert.AreEqual(
				EscapeSequenceParsing.Result.Type.INVALID,
				EscapeSequenceParsing.parseEscape("\\u{12345", 0).type);
	}

	[TestMethod]
	public void testParseUnicodeTooBig() {
		Assert.AreEqual(
				EscapeSequenceParsing.Result.Type.INVALID,
				EscapeSequenceParsing.parseEscape("\\u{110000}", 0).type);
	}

	[TestMethod]
	public void testParseUnicodeSMP() {
		Assert.AreEqual(
				new Result(Result.Type.CODE_POINT, 0x10ABCD, IntervalSet.EMPTY_SET, 0,10),
				EscapeSequenceParsing.parseEscape("\\u{10ABCD}", 0));
	}

	[TestMethod]
	public void testParseUnicodePropertyTooShort() {
		Assert.AreEqual(
				EscapeSequenceParsing.Result.Type.INVALID,
				EscapeSequenceParsing.parseEscape("\\p{}", 0).type);
	}

	[TestMethod]
	public void testParseUnicodePropertyMissingCloseBrace() {
		Assert.AreEqual(
				EscapeSequenceParsing.Result.Type.INVALID,
				EscapeSequenceParsing.parseEscape("\\p{1234", 0).type);
	}

	[TestMethod]
	public void testParseUnicodeProperty() {
		Assert.AreEqual(
				new Result(Result.Type.PROPERTY, -1, IntervalSet.of(66560, 66639), 0,11),
				EscapeSequenceParsing.parseEscape("\\p{Deseret}", 0));
	}

	[TestMethod]
	public void testParseUnicodePropertyInvertedTooShort() {
		Assert.AreEqual(
				EscapeSequenceParsing.Result.Type.INVALID,
				EscapeSequenceParsing.parseEscape("\\P{}", 0).type);
	}

	[TestMethod]
	public void testParseUnicodePropertyInvertedMissingCloseBrace() {
		Assert.AreEqual(
				EscapeSequenceParsing.Result.Type.INVALID,
				EscapeSequenceParsing.parseEscape("\\P{Deseret", 0).type);
	}

	public const int MAX_CODE_POINT = 0X10FFFF;
    [TestMethod]
	public void testParseUnicodePropertyInverted() {
		IntervalSet expected = IntervalSet.of(0, 66559);
		expected.Add(66640, MAX_CODE_POINT);
		Assert.AreEqual(
				new Result(Result.Type.PROPERTY, -1, expected, 0, 11),
				EscapeSequenceParsing.parseEscape("\\P{Deseret}", 0));
	}
}
