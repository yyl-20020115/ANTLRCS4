/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

namespace org.antlr.v4.test.tool;

[TestClass]
public class TestUnicodeEscapes {
	[TestMethod]
	public void latinJavaEscape() {
		checkUnicodeEscape("\\u0061", 0x0061, "Java");
	}

	[TestMethod]
	public void latinPythonEscape() {
		checkUnicodeEscape("\\u0061", 0x0061, "Python2");
		checkUnicodeEscape("\\u0061", 0x0061, "Python3");
	}

	[TestMethod]
	public void latinSwiftEscape() {
		checkUnicodeEscape("\\u{0061}", 0x0061, "Swift");
	}

	[TestMethod]
	public void bmpJavaEscape() {
		checkUnicodeEscape("\\uABCD", 0xABCD, "Java");
	}

	[TestMethod]
	public void bmpPythonEscape() {
		checkUnicodeEscape("\\uABCD", 0xABCD, "Python2");
		checkUnicodeEscape("\\uABCD", 0xABCD, "Python3");
	}

	[TestMethod]
	public void bmpSwiftEscape() {
		checkUnicodeEscape("\\u{ABCD}", 0xABCD, "Swift");
	}

	[TestMethod]
	public void smpJavaEscape() {
		checkUnicodeEscape("\\uD83D\\uDCA9", 0x1F4A9, "Java");
	}

	[TestMethod]
	public void smpPythonEscape() {
		checkUnicodeEscape("\\U0001F4A9", 0x1F4A9, "Python2");
		checkUnicodeEscape("\\U0001F4A9", 0x1F4A9, "Python3");
	}

	[TestMethod]
	public void smpSwiftEscape() {
		checkUnicodeEscape("\\u{1F4A9}", 0x1F4A9, "Swift");
	}

	private void checkUnicodeEscape(String expected, int input, String language) {
		Assert.AreEqual(expected, UnicodeEscapes.escapeCodePoint(input, language));
	}
}
