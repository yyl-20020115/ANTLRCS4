/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.codegen;

namespace org.antlr.v4.test.tool;

[TestClass]
public class TestUnicodeEscapes
{
    [TestMethod]
    public void LatinJavaEscape()
    {
        CheckUnicodeEscape("\\u0061", 0x0061, "Java");
    }

    [TestMethod]
    public void LatinPythonEscape()
    {
        CheckUnicodeEscape("\\u0061", 0x0061, "Python2");
        CheckUnicodeEscape("\\u0061", 0x0061, "Python3");
    }

    [TestMethod]
    public void LatinSwiftEscape()
    {
        CheckUnicodeEscape("\\u{0061}", 0x0061, "Swift");
    }

    [TestMethod]
    public void BmpJavaEscape()
    {
        CheckUnicodeEscape("\\uABCD", 0xABCD, "Java");
    }

    [TestMethod]
    public void BmpPythonEscape()
    {
        CheckUnicodeEscape("\\uABCD", 0xABCD, "Python2");
        CheckUnicodeEscape("\\uABCD", 0xABCD, "Python3");
    }

    [TestMethod]
    public void BmpSwiftEscape()
    {
        CheckUnicodeEscape("\\u{ABCD}", 0xABCD, "Swift");
    }

    [TestMethod]
    public void SmpJavaEscape()
    {
        CheckUnicodeEscape("\\uD83D\\uDCA9", 0x1F4A9, "Java");
    }

    [TestMethod]
    public void SmpPythonEscape()
    {
        CheckUnicodeEscape("\\U0001F4A9", 0x1F4A9, "Python2");
        CheckUnicodeEscape("\\U0001F4A9", 0x1F4A9, "Python3");
    }

    [TestMethod]
    public void SmpSwiftEscape()
    {
        CheckUnicodeEscape("\\u{1F4A9}", 0x1F4A9, "Swift");
    }

    private static void CheckUnicodeEscape(string expected, int input, string language)
    {
        Assert.AreEqual(expected, UnicodeEscapes.EscapeCodePoint(input, language));
    }
}
