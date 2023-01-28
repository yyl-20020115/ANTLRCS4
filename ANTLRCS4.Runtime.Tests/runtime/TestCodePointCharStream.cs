/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

namespace org.antlr.v4.runtime;

[TestClass]
public class TestCodePointCharStream {
	[TestMethod]
	public void emptyBytesHasSize0() {
		CodePointCharStream s = CharStreams.fromString("");
		assertEquals(0, s.size());
		assertEquals(0, s.index());
		assertEquals("", s.ToString());
	}

	[TestMethod]
	public void emptyBytesLookAheadReturnsEOF() {
		CodePointCharStream s = CharStreams.fromString("");
		assertEquals(IntStream.EOF, s.LA(1));
		assertEquals(0, s.index());
	}

	[TestMethod]
	public void consumingEmptyStreamShouldThrow() {
		CodePointCharStream s = CharStreams.fromString("");
		IllegalStateException illegalStateException = assertThrows(
				IllegalStateException,
				s::consume
		);
		assertEquals("cannot consume EOF", illegalStateException.getMessage());
	}

	[TestMethod]
	public void singleLatinCodePointHasSize1() {
		CodePointCharStream s = CharStreams.fromString("X");
		assertEquals(1, s.size());
	}

	[TestMethod]
	public void consumingSingleLatinCodePointShouldMoveIndex() {
		CodePointCharStream s = CharStreams.fromString("X");
		assertEquals(0, s.index());
		s.consume();
		assertEquals(1, s.index());
	}

	[TestMethod]
	public void consumingPastSingleLatinCodePointShouldThrow() {
		CodePointCharStream s = CharStreams.fromString("X");
		s.consume();
		IllegalStateException illegalStateException = assertThrows(IllegalStateException, s::consume);
		assertEquals("cannot consume EOF", illegalStateException.getMessage());
	}

	[TestMethod]
	public void singleLatinCodePointLookAheadShouldReturnCodePoint() {
		CodePointCharStream s = CharStreams.fromString("X");
		assertEquals('X', s.LA(1));
		assertEquals(0, s.index());
	}

	[TestMethod]
	public void multipleLatinCodePointsLookAheadShouldReturnCodePoints() {
		CodePointCharStream s = CharStreams.fromString("XYZ");
		assertEquals('X', s.LA(1));
		assertEquals(0, s.index());
		assertEquals('Y', s.LA(2));
		assertEquals(0, s.index());
		assertEquals('Z', s.LA(3));
		assertEquals(0, s.index());
	}

	[TestMethod]
	public void singleLatinCodePointLookAheadPastEndShouldReturnEOF() {
		CodePointCharStream s = CharStreams.fromString("X");
		assertEquals(IntStream.EOF, s.LA(2));
	}

	[TestMethod]
	public void singleCJKCodePointHasSize1() {
		CodePointCharStream s = CharStreams.fromString("\u611B");
		assertEquals(1, s.size());
		assertEquals(0, s.index());
	}

	[TestMethod]
	public void consumingSingleCJKCodePointShouldMoveIndex() {
		CodePointCharStream s = CharStreams.fromString("\u611B");
		assertEquals(0, s.index());
		s.consume();
		assertEquals(1, s.index());
	}

	[TestMethod]
	public void consumingPastSingleCJKCodePointShouldThrow() {
		CodePointCharStream s = CharStreams.fromString("\u611B");
		s.consume();
		IllegalStateException illegalStateException = assertThrows(IllegalStateException, s::consume);
		assertEquals("cannot consume EOF", illegalStateException.getMessage());
	}

	[TestMethod]
	public void singleCJKCodePointLookAheadShouldReturnCodePoint() {
		CodePointCharStream s = CharStreams.fromString("\u611B");
		assertEquals(0x611B, s.LA(1));
		assertEquals(0, s.index());
	}

	[TestMethod]
	public void singleCJKCodePointLookAheadPastEndShouldReturnEOF() {
		CodePointCharStream s = CharStreams.fromString("\u611B");
		assertEquals(IntStream.EOF, s.LA(2));
		assertEquals(0, s.index());
	}

	[TestMethod]
	public void singleEmojiCodePointHasSize1() {
		CodePointCharStream s = CharStreams.fromString(
				new StringBuilder().appendCodePoint(0x1F4A9).ToString());
		assertEquals(1, s.size());
		assertEquals(0, s.index());
	}

	[TestMethod]
	public void consumingSingleEmojiCodePointShouldMoveIndex() {
		CodePointCharStream s = CharStreams.fromString(
				new StringBuilder().appendCodePoint(0x1F4A9).ToString());
		assertEquals(0, s.index());
		s.consume();
		assertEquals(1, s.index());
	}

	[TestMethod]
	public void consumingPastEndOfEmojiCodePointWithShouldThrow() {
		CodePointCharStream s = CharStreams.fromString(
				new StringBuilder().appendCodePoint(0x1F4A9).ToString());
		assertEquals(0, s.index());
		s.consume();
		assertEquals(1, s.index());
		IllegalStateException illegalStateException = assertThrows(IllegalStateException, s::consume);
		assertEquals("cannot consume EOF", illegalStateException.getMessage());
	}

	[TestMethod]
	public void singleEmojiCodePointLookAheadShouldReturnCodePoint() {
		CodePointCharStream s = CharStreams.fromString(
				new StringBuilder().appendCodePoint(0x1F4A9).ToString());
		assertEquals(0x1F4A9, s.LA(1));
		assertEquals(0, s.index());
	}

	[TestMethod]
	public void singleEmojiCodePointLookAheadPastEndShouldReturnEOF() {
		CodePointCharStream s = CharStreams.fromString(
				new StringBuilder().appendCodePoint(0x1F4A9).ToString());
		assertEquals(IntStream.EOF, s.LA(2));
		assertEquals(0, s.index());
	}

	[TestMethod]
	public void getTextWithLatin() {
		CodePointCharStream s = CharStreams.fromString("0123456789");
		assertEquals("34567", s.getText(Interval.of(3, 7)));
	}

	[TestMethod]
	public void getTextWithCJK() {
		CodePointCharStream s = CharStreams.fromString("01234\u40946789");
		assertEquals("34\u409467", s.getText(Interval.of(3, 7)));
	}

	[TestMethod]
	public void getTextWithEmoji() {
		CodePointCharStream s = CharStreams.fromString(
				new StringBuilder("01234")
					.appendCodePoint(0x1F522)
					.Append("6789")
					.ToString());
		assertEquals("34\uD83D\uDD2267", s.getText(Interval.of(3, 7)));
	}

	[TestMethod]
	public void toStringWithLatin() {
		CodePointCharStream s = CharStreams.fromString("0123456789");
		assertEquals("0123456789", s.ToString());
	}

	[TestMethod]
	public void toStringWithCJK() {
		CodePointCharStream s = CharStreams.fromString("01234\u40946789");
		assertEquals("01234\u40946789", s.ToString());
	}

	[TestMethod]
	public void toStringWithEmoji() {
		CodePointCharStream s = CharStreams.fromString(
				new StringBuilder("01234")
					.appendCodePoint(0x1F522)
					.Append("6789")
					.ToString());
		assertEquals("01234\uD83D\uDD226789", s.ToString());
	}

	[TestMethod]
	public void lookAheadWithLatin() {
		CodePointCharStream s = CharStreams.fromString("0123456789");
		assertEquals('5', s.LA(6));
	}

	[TestMethod]
	public void lookAheadWithCJK() {
		CodePointCharStream s = CharStreams.fromString("01234\u40946789");
		assertEquals(0x4094, s.LA(6));
	}

	[TestMethod]
	public void lookAheadWithEmoji() {
		CodePointCharStream s = CharStreams.fromString(
				new StringBuilder("01234")
					.appendCodePoint(0x1F522)
					.Append("6789")
					.ToString());
		assertEquals(0x1F522, s.LA(6));
	}

	[TestMethod]
	public void seekWithLatin() {
		CodePointCharStream s = CharStreams.fromString("0123456789");
		s.seek(5);
		assertEquals('5', s.LA(1));
	}

	[TestMethod]
	public void seekWithCJK() {
		CodePointCharStream s = CharStreams.fromString("01234\u40946789");
		s.seek(5);
		assertEquals(0x4094, s.LA(1));
	}

	[TestMethod]
	public void seekWithEmoji() {
		CodePointCharStream s = CharStreams.fromString(
				new StringBuilder("01234")
					.appendCodePoint(0x1F522)
					.Append("6789")
					.ToString());
		s.seek(5);
		assertEquals(0x1F522, s.LA(1));
	}

	[TestMethod]
	public void lookBehindWithLatin() {
		CodePointCharStream s = CharStreams.fromString("0123456789");
		s.seek(6);
		assertEquals('5', s.LA(-1));
	}

	[TestMethod]
	public void lookBehindWithCJK() {
		CodePointCharStream s = CharStreams.fromString("01234\u40946789");
		s.seek(6);
		assertEquals(0x4094, s.LA(-1));
	}

	[TestMethod]
	public void lookBehindWithEmoji() {
		CodePointCharStream s = CharStreams.fromString(
				new StringBuilder("01234")
					.appendCodePoint(0x1F522)
					.Append("6789")
					.ToString());
		s.seek(6);
		assertEquals(0x1F522, s.LA(-1));
	}

	[TestMethod]
	public void asciiContentsShouldUse8BitBuffer() {
		CodePointCharStream s = CharStreams.fromString("hello");
		assertTrue(s.getInternalStorage() instanceof byte[]);
		assertEquals(5, s.size());
	}

	[TestMethod]
	public void bmpContentsShouldUse16BitBuffer() {
		CodePointCharStream s = CharStreams.fromString("hello \u4E16\u754C");
		assertTrue(s.getInternalStorage() instanceof char[]);
		assertEquals(8, s.size());
	}

	[TestMethod]
	public void smpContentsShouldUse32BitBuffer() {
		CodePointCharStream s = CharStreams.fromString("hello \uD83C\uDF0D");
		assertTrue(s.getInternalStorage() instanceof int[]);
		assertEquals(7, s.size());
	}
}
