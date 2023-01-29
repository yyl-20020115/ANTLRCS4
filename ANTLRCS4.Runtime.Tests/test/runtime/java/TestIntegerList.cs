/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime.misc;

namespace org.antlr.v4.test.runtime.java;

public class TestIntegerList {
	[TestMethod]
	public void emptyListToEmptyCharArray() {
		IntegerList l = new IntegerList();
		assertArrayEquals(new char[0], l.toCharArray());
	}

	[TestMethod]
	public void negativeIntegerToCharArrayThrows() {
		IntegerList l = new IntegerList();
		l.add(-42);
		assertThrows(
				ArgumentException,
				l::toCharArray
		);
	}

	[TestMethod]
	public void surrogateRangeIntegerToCharArray() {
		IntegerList l = new IntegerList();
		// Java allows dangling surrogates, so (currently) we do
		// as well. We could change this if desired.
		l.add(0xDC00);
		char[] expected = new char[] { 0xDC00 };
		assertArrayEquals(expected, l.toCharArray());
	}

	[TestMethod]
	public void tooLargeIntegerToCharArrayThrows() {
		IntegerList l = new IntegerList();
		l.add(0x110000);
		assertThrows(
				ArgumentException,
				l::toCharArray
		);
	}

	[TestMethod]
	public void unicodeBMPIntegerListToCharArray() {
		IntegerList l = new IntegerList();
		l.add(0x35);
		l.add(0x4E94);
		l.add(0xFF15);
		char[] expected = new char[] { 0x35, 0x4E94, 0xFF15 };
		assertArrayEquals(expected, l.toCharArray());
	}

	[TestMethod]
	public void unicodeSMPIntegerListToCharArray() {
		IntegerList l = new IntegerList();
		l.add(0x104A5);
		l.add(0x116C5);
		l.add(0x1D7FB);
		char[] expected = new char[] { 0xD801, 0xDCA5, 0xD805, 0xDEC5, 0xD835, 0xDFFB };
		assertArrayEquals(expected, l.toCharArray());
	}
}
