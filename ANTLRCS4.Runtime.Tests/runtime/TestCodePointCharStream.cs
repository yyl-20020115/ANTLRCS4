/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime.dfa;
using org.antlr.v4.runtime.misc;
using System.Text;

namespace org.antlr.v4.runtime;

[TestClass]
public class TestCodePointCharStream {
	[TestMethod]
	public void emptyBytesHasSize0() {
		CodePointCharStream s = CharStreams.fromString("");
		Assert.AreEqual(0, s.size());
		Assert.AreEqual(0, s.index());
		Assert.AreEqual("", s.ToString());
	}

	[TestMethod]
	public void emptyBytesLookAheadReturnsEOF() {
		CodePointCharStream s = CharStreams.fromString("");
		Assert.AreEqual(IntStream.EOF, s.LA(1));
		Assert.AreEqual(0, s.index());
	}

	[TestMethod]
	public void consumingEmptyStreamShouldThrow() {
		CodePointCharStream s = CharStreams.fromString("");
		IllegalStateException illegalStateException = Assert.ThrowsException<
				IllegalStateException>(
				() => s.consume()
		);
		Assert.AreEqual("cannot consume EOF", illegalStateException.Message);
	}

	[TestMethod]
	public void singleLatinCodePointHasSize1() {
		CodePointCharStream s = CharStreams.fromString("X");
		Assert.AreEqual(1, s.size());
	}

	[TestMethod]
	public void consumingSingleLatinCodePointShouldMoveIndex() {
		CodePointCharStream s = CharStreams.fromString("X");
		Assert.AreEqual(0, s.index());
		s.consume();
		Assert.AreEqual(1, s.index());
	}

	[TestMethod]
	public void consumingPastSingleLatinCodePointShouldThrow() {
		CodePointCharStream s = CharStreams.fromString("X");
		s.consume();
		IllegalStateException illegalStateException = Assert.ThrowsException<IllegalStateException>(() => s.consume());
		Assert.AreEqual("cannot consume EOF", illegalStateException.Message);
	}

	[TestMethod]
	public void singleLatinCodePointLookAheadShouldReturnCodePoint() {
		CodePointCharStream s = CharStreams.fromString("X");
		Assert.AreEqual('X', s.LA(1));
		Assert.AreEqual(0, s.index());
	}

	[TestMethod]
	public void multipleLatinCodePointsLookAheadShouldReturnCodePoints() {
		CodePointCharStream s = CharStreams.fromString("XYZ");
		Assert.AreEqual('X', s.LA(1));
		Assert.AreEqual(0, s.index());
		Assert.AreEqual('Y', s.LA(2));
		Assert.AreEqual(0, s.index());
		Assert.AreEqual('Z', s.LA(3));
		Assert.AreEqual(0, s.index());
	}

	[TestMethod]
	public void singleLatinCodePointLookAheadPastEndShouldReturnEOF() {
		CodePointCharStream s = CharStreams.fromString("X");
		Assert.AreEqual(IntStream.EOF, s.LA(2));
	}

	[TestMethod]
	public void singleCJKCodePointHasSize1() {
		CodePointCharStream s = CharStreams.fromString("\u611B");
		Assert.AreEqual(1, s.size());
		Assert.AreEqual(0, s.index());
	}

	[TestMethod]
	public void consumingSingleCJKCodePointShouldMoveIndex() {
		CodePointCharStream s = CharStreams.fromString("\u611B");
		Assert.AreEqual(0, s.index());
		s.consume();
		Assert.AreEqual(1, s.index());
	}

	[TestMethod]
	public void consumingPastSingleCJKCodePointShouldThrow() {
		CodePointCharStream s = CharStreams.fromString("\u611B");
		s.consume();
		IllegalStateException illegalStateException = Assert.ThrowsException<IllegalStateException>(()=>s.consume());
		Assert.AreEqual("cannot consume EOF", illegalStateException.Message);
	}

	[TestMethod]
	public void singleCJKCodePointLookAheadShouldReturnCodePoint() {
		CodePointCharStream s = CharStreams.fromString("\u611B");
		Assert.AreEqual(0x611B, s.LA(1));
		Assert.AreEqual(0, s.index());
	}

	[TestMethod]
	public void singleCJKCodePointLookAheadPastEndShouldReturnEOF() {
		CodePointCharStream s = CharStreams.fromString("\u611B");
		Assert.AreEqual(IntStream.EOF, s.LA(2));
		Assert.AreEqual(0, s.index());
	}

	[TestMethod]
	public void singleEmojiCodePointHasSize1() {
		CodePointCharStream s = CharStreams.fromString(
				new StringBuilder().Append(char.ConvertFromUtf32(0x1F4A9)).ToString());
		Assert.AreEqual(1, s.size());
		Assert.AreEqual(0, s.index());
	}

	[TestMethod]
	public void consumingSingleEmojiCodePointShouldMoveIndex() {
		CodePointCharStream s = CharStreams.fromString(
				new StringBuilder().Append(char.ConvertFromUtf32(0x1F4A9)).ToString());
		Assert.AreEqual(0, s.index());
		s.consume();
		Assert.AreEqual(1, s.index());
	}

	[TestMethod]
	public void consumingPastEndOfEmojiCodePointWithShouldThrow() {
		CodePointCharStream s = CharStreams.fromString(
				new StringBuilder().Append(char.ConvertFromUtf32(0x1F4A9)).ToString());
		Assert.AreEqual(0, s.index());
		s.consume();
		Assert.AreEqual(1, s.index());
		IllegalStateException illegalStateException = Assert.ThrowsException<IllegalStateException>(
			()=>s.consume());
		Assert.AreEqual("cannot consume EOF", illegalStateException.Message);
	}

	[TestMethod]
	public void singleEmojiCodePointLookAheadShouldReturnCodePoint() {
		CodePointCharStream s = CharStreams.fromString(
				new StringBuilder().Append(char.ConvertFromUtf32(0x1F4A9)).ToString());
		Assert.AreEqual(0x1F4A9, s.LA(1));
		Assert.AreEqual(0, s.index());
	}

	[TestMethod]
	public void singleEmojiCodePointLookAheadPastEndShouldReturnEOF() {
		CodePointCharStream s = CharStreams.fromString(
				new StringBuilder().Append(char.ConvertFromUtf32(0x1F4A9)).ToString());
		Assert.AreEqual(IntStream.EOF, s.LA(2));
		Assert.AreEqual(0, s.index());
	}

	[TestMethod]
	public void getTextWithLatin() {
		CodePointCharStream s = CharStreams.fromString("0123456789");
		Assert.AreEqual("34567", s.getText(Interval.of(3, 7)));
	}

	[TestMethod]
	public void getTextWithCJK() {
		CodePointCharStream s = CharStreams.fromString("01234\u40946789");
		Assert.AreEqual("34\u409467", s.getText(Interval.of(3, 7)));
	}

	[TestMethod]
	public void getTextWithEmoji() {
		CodePointCharStream s = CharStreams.fromString(
				new StringBuilder("01234")
					.Append(char.ConvertFromUtf32(0x1F522))
					.Append("6789")
					.ToString());
		Assert.AreEqual("34\uD83D\uDD2267", s.getText(Interval.of(3, 7)));
	}

	[TestMethod]
	public void toStringWithLatin() {
		CodePointCharStream s = CharStreams.fromString("0123456789");
		Assert.AreEqual("0123456789", s.ToString());
	}

	[TestMethod]
	public void toStringWithCJK() {
		CodePointCharStream s = CharStreams.fromString("01234\u40946789");
		Assert.AreEqual("01234\u40946789", s.ToString());
	}

	[TestMethod]
	public void toStringWithEmoji() {
		CodePointCharStream s = CharStreams.fromString(
				new StringBuilder("01234").Append(char.ConvertFromUtf32(0x1F522))
					.Append("6789")
					.ToString());
		Assert.AreEqual("01234\uD83D\uDD226789", s.ToString());
	}

	[TestMethod]
	public void lookAheadWithLatin() {
		CodePointCharStream s = CharStreams.fromString("0123456789");
		Assert.AreEqual('5', s.LA(6));
	}

	[TestMethod]
	public void lookAheadWithCJK() {
		CodePointCharStream s = CharStreams.fromString("01234\u40946789");
		Assert.AreEqual(0x4094, s.LA(6));
	}

	[TestMethod]
	public void lookAheadWithEmoji() {
		CodePointCharStream s = CharStreams.fromString(
				new StringBuilder("01234").Append(char.ConvertFromUtf32(0x1F522))
					.Append("6789")
					.ToString());
		Assert.AreEqual(0x1F522, s.LA(6));
	}

	[TestMethod]
	public void seekWithLatin() {
		CodePointCharStream s = CharStreams.fromString("0123456789");
		s.seek(5);
		Assert.AreEqual('5', s.LA(1));
	}

	[TestMethod]
	public void seekWithCJK() {
		CodePointCharStream s = CharStreams.fromString("01234\u40946789");
		s.seek(5);
		Assert.AreEqual(0x4094, s.LA(1));
	}

	[TestMethod]
	public void seekWithEmoji() {
		CodePointCharStream s = CharStreams.fromString(
				new StringBuilder("01234").Append(char.ConvertFromUtf32(0x1F522))
					.Append("6789")
					.ToString());
		s.seek(5);
		Assert.AreEqual(0x1F522, s.LA(1));
	}

	[TestMethod]
	public void lookBehindWithLatin() {
		CodePointCharStream s = CharStreams.fromString("0123456789");
		s.seek(6);
		Assert.AreEqual('5', s.LA(-1));
	}

	[TestMethod]
	public void lookBehindWithCJK() {
		CodePointCharStream s = CharStreams.fromString("01234\u40946789");
		s.seek(6);
		Assert.AreEqual(0x4094, s.LA(-1));
	}

	[TestMethod]
	public void lookBehindWithEmoji() {
		CodePointCharStream s = CharStreams.fromString(
				new StringBuilder("01234").Append(char.ConvertFromUtf32(0x1F522))
					.Append("6789")
					.ToString());
		s.seek(6);
		Assert.AreEqual(0x1F522, s.LA(-1));
	}

	[TestMethod]
	public void asciiContentsShouldUse8BitBuffer() {
		CodePointCharStream s = CharStreams.fromString("hello");
		Assert.IsTrue(s.getInternalStorage() is byte[]);
		Assert.AreEqual(5, s.size());
	}

	[TestMethod]
	public void bmpContentsShouldUse16BitBuffer() {
		CodePointCharStream s = CharStreams.fromString("hello \u4E16\u754C");
		Assert.IsTrue(s.getInternalStorage() is char[]);
		Assert.AreEqual(8, s.size());
	}

	[TestMethod]
	public void smpContentsShouldUse32BitBuffer() {
		CodePointCharStream s = CharStreams.fromString("hello \uD83C\uDF0D");
		Assert.IsTrue(s.getInternalStorage() is int[]);
		Assert.AreEqual(7, s.size());
	}
}
