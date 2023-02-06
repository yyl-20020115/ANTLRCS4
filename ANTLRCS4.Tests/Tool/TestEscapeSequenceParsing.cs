/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime.misc;
using static org.antlr.v4.misc.EscapeSequenceParsing;

namespace org.antlr.v4.test.tool;

[TestClass]
public class TestEscapeSequenceParsing
{
    [TestMethod]
    public void TestParseEmpty()
    {
        Assert.AreEqual(
                Result.Type.INVALID,
                ParseEscape("", 0).type);
    }

    [TestMethod]
    public void TestParseJustBackslash()
    {
        Assert.AreEqual(
                Result.Type.INVALID,
                ParseEscape("\\", 0).type);
    }

    [TestMethod]
    public void TestParseInvalidEscape()
    {
        Assert.AreEqual(
                Result.Type.INVALID,
                ParseEscape("\\z", 0).type);
    }

    [TestMethod]
    public void TestParseNewline()
    {
        Assert.AreEqual(
                new Result(Result.Type.CODE_POINT, '\n', IntervalSet.EMPTY_SET, 0, 2),
                ParseEscape("\\n", 0));
    }

    [TestMethod]
    public void TestParseTab()
    {
        Assert.AreEqual(
                new Result(Result.Type.CODE_POINT, '\t', IntervalSet.EMPTY_SET, 0, 2),
                ParseEscape("\\t", 0));
    }

    [TestMethod]
    public void TestParseUnicodeTooShort()
    {
        Assert.AreEqual(
                Result.Type.INVALID,
                ParseEscape("\\uABC", 0).type);
    }

    [TestMethod]
    public void TestParseUnicodeBMP()
    {
        Assert.AreEqual(
                new Result(Result.Type.CODE_POINT, 0xABCD, IntervalSet.EMPTY_SET, 0, 6),
                ParseEscape("\\uABCD", 0));
    }

    [TestMethod]
    public void TestParseUnicodeSMPTooShort()
    {
        Assert.AreEqual(
                Result.Type.INVALID,
                ParseEscape("\\u{}", 0).type);
    }

    [TestMethod]
    public void TestParseUnicodeSMPMissingCloseBrace()
    {
        Assert.AreEqual(
                Result.Type.INVALID,
                ParseEscape("\\u{12345", 0).type);
    }

    [TestMethod]
    public void TestParseUnicodeTooBig()
    {
        Assert.AreEqual(
                Result.Type.INVALID,
                ParseEscape("\\u{110000}", 0).type);
    }

    [TestMethod]
    public void TestParseUnicodeSMP()
    {
        Assert.AreEqual(
                new Result(Result.Type.CODE_POINT, 0x10ABCD, IntervalSet.EMPTY_SET, 0, 10),
                ParseEscape("\\u{10ABCD}", 0));
    }

    [TestMethod]
    public void TestParseUnicodePropertyTooShort()
    {
        Assert.AreEqual(
                Result.Type.INVALID,
                ParseEscape("\\p{}", 0).type);
    }

    [TestMethod]
    public void TestParseUnicodePropertyMissingCloseBrace()
    {
        Assert.AreEqual(
                Result.Type.INVALID,
                ParseEscape("\\p{1234", 0).type);
    }

    [TestMethod]
    public void TestParseUnicodeProperty()
    {
        Assert.AreEqual(
                new Result(Result.Type.PROPERTY, -1, IntervalSet.Of(66560, 66639), 0, 11),
                ParseEscape("\\p{Deseret}", 0));
    }

    [TestMethod]
    public void TestParseUnicodePropertyInvertedTooShort()
    {
        Assert.AreEqual(
                Result.Type.INVALID,
                ParseEscape("\\P{}", 0).type);
    }

    [TestMethod]
    public void TestParseUnicodePropertyInvertedMissingCloseBrace()
    {
        Assert.AreEqual(
                Result.Type.INVALID,
                ParseEscape("\\P{Deseret", 0).type);
    }

    public const int MAX_CODE_POINT = 0X10FFFF;
    [TestMethod]
    public void TestParseUnicodePropertyInverted()
    {
        var expected = IntervalSet.Of(0, 66559);
        expected.Add(66640, MAX_CODE_POINT);
        Assert.AreEqual(
                new Result(Result.Type.PROPERTY, -1, expected, 0, 11),
                ParseEscape("\\P{Deseret}", 0));
    }
}
