/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

namespace org.antlr.v4.test.tool;

[TestClass]
public class TestEscapeSequenceParsing {
	[TestMethod]
	public void testParseEmpty() {
		assertEquals(
				EscapeSequenceParsing.Result.Type.INVALID,
				EscapeSequenceParsing.parseEscape("", 0).type);
	}

	[TestMethod]
	public void testParseJustBackslash() {
		assertEquals(
				EscapeSequenceParsing.Result.Type.INVALID,
				EscapeSequenceParsing.parseEscape("\\", 0).type);
	}

	[TestMethod]
	public void testParseInvalidEscape() {
		assertEquals(
				EscapeSequenceParsing.Result.Type.INVALID,
				EscapeSequenceParsing.parseEscape("\\z", 0).type);
	}

	[TestMethod]
	public void testParseNewline() {
		assertEquals(
				new Result(Result.Type.CODE_POINT, '\n', IntervalSet.EMPTY_SET, 0,2),
				EscapeSequenceParsing.parseEscape("\\n", 0));
	}

	[TestMethod]
	public void testParseTab() {
		assertEquals(
				new Result(Result.Type.CODE_POINT, '\t', IntervalSet.EMPTY_SET, 0,2),
				EscapeSequenceParsing.parseEscape("\\t", 0));
	}

	[TestMethod]
	public void testParseUnicodeTooShort() {
		assertEquals(
				EscapeSequenceParsing.Result.Type.INVALID,
				EscapeSequenceParsing.parseEscape("\\uABC", 0).type);
	}

	[TestMethod]
	public void testParseUnicodeBMP() {
		assertEquals(
				new Result(Result.Type.CODE_POINT, 0xABCD, IntervalSet.EMPTY_SET, 0,6),
				EscapeSequenceParsing.parseEscape("\\uABCD", 0));
	}

	[TestMethod]
	public void testParseUnicodeSMPTooShort() {
		assertEquals(
				EscapeSequenceParsing.Result.Type.INVALID,
				EscapeSequenceParsing.parseEscape("\\u{}", 0).type);
	}

	[TestMethod]
	public void testParseUnicodeSMPMissingCloseBrace() {
		assertEquals(
				EscapeSequenceParsing.Result.Type.INVALID,
				EscapeSequenceParsing.parseEscape("\\u{12345", 0).type);
	}

	[TestMethod]
	public void testParseUnicodeTooBig() {
		assertEquals(
				EscapeSequenceParsing.Result.Type.INVALID,
				EscapeSequenceParsing.parseEscape("\\u{110000}", 0).type);
	}

	[TestMethod]
	public void testParseUnicodeSMP() {
		assertEquals(
				new Result(Result.Type.CODE_POINT, 0x10ABCD, IntervalSet.EMPTY_SET, 0,10),
				EscapeSequenceParsing.parseEscape("\\u{10ABCD}", 0));
	}

	[TestMethod]
	public void testParseUnicodePropertyTooShort() {
		assertEquals(
				EscapeSequenceParsing.Result.Type.INVALID,
				EscapeSequenceParsing.parseEscape("\\p{}", 0).type);
	}

	[TestMethod]
	public void testParseUnicodePropertyMissingCloseBrace() {
		assertEquals(
				EscapeSequenceParsing.Result.Type.INVALID,
				EscapeSequenceParsing.parseEscape("\\p{1234", 0).type);
	}

	[TestMethod]
	public void testParseUnicodeProperty() {
		assertEquals(
				new Result(Result.Type.PROPERTY, -1, IntervalSet.of(66560, 66639), 0,11),
				EscapeSequenceParsing.parseEscape("\\p{Deseret}", 0));
	}

	[TestMethod]
	public void testParseUnicodePropertyInvertedTooShort() {
		assertEquals(
				EscapeSequenceParsing.Result.Type.INVALID,
				EscapeSequenceParsing.parseEscape("\\P{}", 0).type);
	}

	[TestMethod]
	public void testParseUnicodePropertyInvertedMissingCloseBrace() {
		assertEquals(
				EscapeSequenceParsing.Result.Type.INVALID,
				EscapeSequenceParsing.parseEscape("\\P{Deseret", 0).type);
	}

	[TestMethod]
	public void testParseUnicodePropertyInverted() {
		IntervalSet expected = IntervalSet.of(0, 66559);
		expected.add(66640, Character.MAX_CODE_POINT);
		assertEquals(
				new Result(Result.Type.PROPERTY, -1, expected, 0, 11),
				EscapeSequenceParsing.parseEscape("\\P{Deseret}", 0));
	}
}
