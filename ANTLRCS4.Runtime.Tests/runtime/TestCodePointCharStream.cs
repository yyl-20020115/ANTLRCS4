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
public class TestCodePointCharStream
{
    [TestMethod]
    public void EmptyBytesHasSize0()
    {
        var s = CharStreams.fromString("");
        Assert.AreEqual(0, s.size());
        Assert.AreEqual(0, s.index());
        Assert.AreEqual("", s.ToString());
    }

    [TestMethod]
    public void EmptyBytesLookAheadReturnsEOF()
    {
        var s = CharStreams.fromString("");
        Assert.AreEqual(IntStream.EOF, s.LA(1));
        Assert.AreEqual(0, s.index());
    }

    [TestMethod]
    public void ConsumingEmptyStreamShouldThrow()
    {
        var s = CharStreams.fromString("");
        IllegalStateException illegalStateException = Assert.ThrowsException<
                IllegalStateException>(
                () => s.consume()
        );
        Assert.AreEqual("cannot consume EOF", illegalStateException.Message);
    }

    [TestMethod]
    public void SingleLatinCodePointHasSize1()
    {
        var s = CharStreams.fromString("X");
        Assert.AreEqual(1, s.size());
    }

    [TestMethod]
    public void ConsumingSingleLatinCodePointShouldMoveIndex()
    {
        var s = CharStreams.fromString("X");
        Assert.AreEqual(0, s.index());
        s.consume();
        Assert.AreEqual(1, s.index());
    }

    [TestMethod]
    public void ConsumingPastSingleLatinCodePointShouldThrow()
    {
        var s = CharStreams.fromString("X");
        s.consume();
        IllegalStateException illegalStateException = Assert.ThrowsException<IllegalStateException>(() => s.consume());
        Assert.AreEqual("cannot consume EOF", illegalStateException.Message);
    }

    [TestMethod]
    public void SingleLatinCodePointLookAheadShouldReturnCodePoint()
    {
        var s = CharStreams.fromString("X");
        Assert.AreEqual('X', s.LA(1));
        Assert.AreEqual(0, s.index());
    }

    [TestMethod]
    public void MultipleLatinCodePointsLookAheadShouldReturnCodePoints()
    {
        var s = CharStreams.fromString("XYZ");
        Assert.AreEqual('X', s.LA(1));
        Assert.AreEqual(0, s.index());
        Assert.AreEqual('Y', s.LA(2));
        Assert.AreEqual(0, s.index());
        Assert.AreEqual('Z', s.LA(3));
        Assert.AreEqual(0, s.index());
    }

    [TestMethod]
    public void SingleLatinCodePointLookAheadPastEndShouldReturnEOF()
    {
        var s = CharStreams.fromString("X");
        Assert.AreEqual(IntStream.EOF, s.LA(2));
    }

    [TestMethod]
    public void SingleCJKCodePointHasSize1()
    {
        var s = CharStreams.fromString("\u611B");
        Assert.AreEqual(1, s.size());
        Assert.AreEqual(0, s.index());
    }

    [TestMethod]
    public void ConsumingSingleCJKCodePointShouldMoveIndex()
    {
        var s = CharStreams.fromString("\u611B");
        Assert.AreEqual(0, s.index());
        s.consume();
        Assert.AreEqual(1, s.index());
    }

    [TestMethod]
    public void ConsumingPastSingleCJKCodePointShouldThrow()
    {
        var s = CharStreams.fromString("\u611B");
        s.consume();
        IllegalStateException illegalStateException = Assert.ThrowsException<IllegalStateException>(() => s.consume());
        Assert.AreEqual("cannot consume EOF", illegalStateException.Message);
    }

    [TestMethod]
    public void SingleCJKCodePointLookAheadShouldReturnCodePoint()
    {
        var s = CharStreams.fromString("\u611B");
        Assert.AreEqual(0x611B, s.LA(1));
        Assert.AreEqual(0, s.index());
    }

    [TestMethod]
    public void SingleCJKCodePointLookAheadPastEndShouldReturnEOF()
    {
        var s = CharStreams.fromString("\u611B");
        Assert.AreEqual(IntStream.EOF, s.LA(2));
        Assert.AreEqual(0, s.index());
    }

    [TestMethod]
    public void SingleEmojiCodePointHasSize1()
    {
        var s = CharStreams.fromString(
                new StringBuilder().Append(char.ConvertFromUtf32(0x1F4A9)).ToString());
        Assert.AreEqual(1, s.size());
        Assert.AreEqual(0, s.index());
    }

    [TestMethod]
    public void ConsumingSingleEmojiCodePointShouldMoveIndex()
    {
        var s = CharStreams.fromString(
                new StringBuilder().Append(char.ConvertFromUtf32(0x1F4A9)).ToString());
        Assert.AreEqual(0, s.index());
        s.consume();
        Assert.AreEqual(1, s.index());
    }

    [TestMethod]
    public void ConsumingPastEndOfEmojiCodePointWithShouldThrow()
    {
        var s = CharStreams.fromString(
                new StringBuilder().Append(char.ConvertFromUtf32(0x1F4A9)).ToString());
        Assert.AreEqual(0, s.index());
        s.consume();
        Assert.AreEqual(1, s.index());
        IllegalStateException illegalStateException = Assert.ThrowsException<IllegalStateException>(
            () => s.consume());
        Assert.AreEqual("cannot consume EOF", illegalStateException.Message);
    }

    [TestMethod]
    public void SingleEmojiCodePointLookAheadShouldReturnCodePoint()
    {
        var s = CharStreams.fromString(
                new StringBuilder().Append(char.ConvertFromUtf32(0x1F4A9)).ToString());
        Assert.AreEqual(0x1F4A9, s.LA(1));
        Assert.AreEqual(0, s.index());
    }

    [TestMethod]
    public void SingleEmojiCodePointLookAheadPastEndShouldReturnEOF()
    {
        var s = CharStreams.fromString(
                new StringBuilder().Append(char.ConvertFromUtf32(0x1F4A9)).ToString());
        Assert.AreEqual(IntStream.EOF, s.LA(2));
        Assert.AreEqual(0, s.index());
    }

    [TestMethod]
    public void GetTextWithLatin()
    {
        var s = CharStreams.fromString("0123456789");
        Assert.AreEqual("34567", s.getText(Interval.of(3, 7)));
    }

    [TestMethod]
    public void GetTextWithCJK()
    {
        var s = CharStreams.fromString("01234\u40946789");
        Assert.AreEqual("34\u409467", s.getText(Interval.of(3, 7)));
    }

    [TestMethod]
    public void GetTextWithEmoji()
    {
        var s = CharStreams.fromString(
                new StringBuilder("01234")
                    .Append(char.ConvertFromUtf32(0x1F522))
                    .Append("6789")
                    .ToString());
        Assert.AreEqual("34\uD83D\uDD2267", s.getText(Interval.of(3, 7)));
    }

    [TestMethod]
    public void ToStringWithLatin()
    {
        var s = CharStreams.fromString("0123456789");
        Assert.AreEqual("0123456789", s.ToString());
    }

    [TestMethod]
    public void ToStringWithCJK()
    {
        var s = CharStreams.fromString("01234\u40946789");
        Assert.AreEqual("01234\u40946789", s.ToString());
    }

    [TestMethod]
    public void ToStringWithEmoji()
    {
        var s = CharStreams.fromString(
                new StringBuilder("01234").Append(char.ConvertFromUtf32(0x1F522))
                    .Append("6789")
                    .ToString());
        Assert.AreEqual("01234\uD83D\uDD226789", s.ToString());
    }

    [TestMethod]
    public void LookAheadWithLatin()
    {
        var s = CharStreams.fromString("0123456789");
        Assert.AreEqual('5', s.LA(6));
    }

    [TestMethod]
    public void LookAheadWithCJK()
    {
        var s = CharStreams.fromString("01234\u40946789");
        Assert.AreEqual(0x4094, s.LA(6));
    }

    [TestMethod]
    public void LookAheadWithEmoji()
    {
        var s = CharStreams.fromString(
                new StringBuilder("01234").Append(char.ConvertFromUtf32(0x1F522))
                    .Append("6789")
                    .ToString());
        Assert.AreEqual(0x1F522, s.LA(6));
    }

    [TestMethod]
    public void SeekWithLatin()
    {
        var s = CharStreams.fromString("0123456789");
        s.seek(5);
        Assert.AreEqual('5', s.LA(1));
    }

    [TestMethod]
    public void SeekWithCJK()
    {
        var s = CharStreams.fromString("01234\u40946789");
        s.seek(5);
        Assert.AreEqual(0x4094, s.LA(1));
    }

    [TestMethod]
    public void SeekWithEmoji()
    {
        var s = CharStreams.fromString(
                new StringBuilder("01234").Append(char.ConvertFromUtf32(0x1F522))
                    .Append("6789")
                    .ToString());
        s.seek(5);
        Assert.AreEqual(0x1F522, s.LA(1));
    }

    [TestMethod]
    public void LookBehindWithLatin()
    {
        var s = CharStreams.fromString("0123456789");
        s.seek(6);
        Assert.AreEqual('5', s.LA(-1));
    }

    [TestMethod]
    public void LookBehindWithCJK()
    {
        var s = CharStreams.fromString("01234\u40946789");
        s.seek(6);
        Assert.AreEqual(0x4094, s.LA(-1));
    }

    [TestMethod]
    public void LookBehindWithEmoji()
    {
        var s = CharStreams.fromString(
                new StringBuilder("01234").Append(char.ConvertFromUtf32(0x1F522))
                    .Append("6789")
                    .ToString());
        s.seek(6);
        Assert.AreEqual(0x1F522, s.LA(-1));
    }

    [TestMethod]
    public void AsciiContentsShouldUse8BitBuffer()
    {
        var s = CharStreams.fromString("hello");
        Assert.IsTrue(s.getInternalStorage() is byte[]);
        Assert.AreEqual(5, s.size());
    }

    [TestMethod]
    public void BmpContentsShouldUse16BitBuffer()
    {
        var s = CharStreams.fromString("hello \u4E16\u754C");
        Assert.IsTrue(s.getInternalStorage() is char[]);
        Assert.AreEqual(8, s.size());
    }

    [TestMethod]
    public void SmpContentsShouldUse32BitBuffer()
    {
        var s = CharStreams.fromString("hello \uD83C\uDF0D");
        Assert.IsTrue(s.getInternalStorage() is int[]);
        Assert.AreEqual(7, s.size());
    }
}
