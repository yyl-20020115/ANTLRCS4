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
        var s = CharStreams.FromString("");
        Assert.AreEqual(0, s.Count);
        Assert.AreEqual(0, s.Index);
        Assert.AreEqual("", s.ToString());
    }

    [TestMethod]
    public void EmptyBytesLookAheadReturnsEOF()
    {
        var s = CharStreams.FromString("");
        Assert.AreEqual(IntStream.EOF, s.LA(1));
        Assert.AreEqual(0, s.Index);
    }

    [TestMethod]
    public void ConsumingEmptyStreamShouldThrow()
    {
        var s = CharStreams.FromString("");
        IllegalStateException illegalStateException = Assert.ThrowsException<
                IllegalStateException>(
                () => s.Consume()
        );
        Assert.AreEqual("cannot consume EOF", illegalStateException.Message);
    }

    [TestMethod]
    public void SingleLatinCodePointHasSize1()
    {
        var s = CharStreams.FromString("X");
        Assert.AreEqual(1, s.Count);
    }

    [TestMethod]
    public void ConsumingSingleLatinCodePointShouldMoveIndex()
    {
        var s = CharStreams.FromString("X");
        Assert.AreEqual(0, s.Index);
        s.Consume();
        Assert.AreEqual(1, s.Index);
    }

    [TestMethod]
    public void ConsumingPastSingleLatinCodePointShouldThrow()
    {
        var s = CharStreams.FromString("X");
        s.Consume();
        IllegalStateException illegalStateException = Assert.ThrowsException<IllegalStateException>(() => s.Consume());
        Assert.AreEqual("cannot consume EOF", illegalStateException.Message);
    }

    [TestMethod]
    public void SingleLatinCodePointLookAheadShouldReturnCodePoint()
    {
        var s = CharStreams.FromString("X");
        Assert.AreEqual('X', s.LA(1));
        Assert.AreEqual(0, s.Index);
    }

    [TestMethod]
    public void MultipleLatinCodePointsLookAheadShouldReturnCodePoints()
    {
        var s = CharStreams.FromString("XYZ");
        Assert.AreEqual('X', s.LA(1));
        Assert.AreEqual(0, s.Index);
        Assert.AreEqual('Y', s.LA(2));
        Assert.AreEqual(0, s.Index);
        Assert.AreEqual('Z', s.LA(3));
        Assert.AreEqual(0, s.Index);
    }

    [TestMethod]
    public void SingleLatinCodePointLookAheadPastEndShouldReturnEOF()
    {
        var s = CharStreams.FromString("X");
        Assert.AreEqual(IntStream.EOF, s.LA(2));
    }

    [TestMethod]
    public void SingleCJKCodePointHasSize1()
    {
        var s = CharStreams.FromString("\u611B");
        Assert.AreEqual(1, s.Count);
        Assert.AreEqual(0, s.Index);
    }

    [TestMethod]
    public void ConsumingSingleCJKCodePointShouldMoveIndex()
    {
        var s = CharStreams.FromString("\u611B");
        Assert.AreEqual(0, s.Index);
        s.Consume();
        Assert.AreEqual(1, s.Index);
    }

    [TestMethod]
    public void ConsumingPastSingleCJKCodePointShouldThrow()
    {
        var s = CharStreams.FromString("\u611B");
        s.Consume();
        IllegalStateException illegalStateException = Assert.ThrowsException<IllegalStateException>(() => s.Consume());
        Assert.AreEqual("cannot consume EOF", illegalStateException.Message);
    }

    [TestMethod]
    public void SingleCJKCodePointLookAheadShouldReturnCodePoint()
    {
        var s = CharStreams.FromString("\u611B");
        Assert.AreEqual(0x611B, s.LA(1));
        Assert.AreEqual(0, s.Index);
    }

    [TestMethod]
    public void SingleCJKCodePointLookAheadPastEndShouldReturnEOF()
    {
        var s = CharStreams.FromString("\u611B");
        Assert.AreEqual(IntStream.EOF, s.LA(2));
        Assert.AreEqual(0, s.Index);
    }

    [TestMethod]
    public void SingleEmojiCodePointHasSize1()
    {
        var s = CharStreams.FromString(
                new StringBuilder().Append(char.ConvertFromUtf32(0x1F4A9)).ToString());
        Assert.AreEqual(1, s.Count);
        Assert.AreEqual(0, s.Index);
    }

    [TestMethod]
    public void ConsumingSingleEmojiCodePointShouldMoveIndex()
    {
        var s = CharStreams.FromString(
                new StringBuilder().Append(char.ConvertFromUtf32(0x1F4A9)).ToString());
        Assert.AreEqual(0, s.Index);
        s.Consume();
        Assert.AreEqual(1, s.Index);
    }

    [TestMethod]
    public void ConsumingPastEndOfEmojiCodePointWithShouldThrow()
    {
        var s = CharStreams.FromString(
                new StringBuilder().Append(char.ConvertFromUtf32(0x1F4A9)).ToString());
        Assert.AreEqual(0, s.Index);
        s.Consume();
        Assert.AreEqual(1, s.Index);
        IllegalStateException illegalStateException = Assert.ThrowsException<IllegalStateException>(
            () => s.Consume());
        Assert.AreEqual("cannot consume EOF", illegalStateException.Message);
    }

    [TestMethod]
    public void SingleEmojiCodePointLookAheadShouldReturnCodePoint()
    {
        var s = CharStreams.FromString(
                new StringBuilder().Append(char.ConvertFromUtf32(0x1F4A9)).ToString());
        Assert.AreEqual(0x1F4A9, s.LA(1));
        Assert.AreEqual(0, s.Index);
    }

    [TestMethod]
    public void SingleEmojiCodePointLookAheadPastEndShouldReturnEOF()
    {
        var s = CharStreams.FromString(
                new StringBuilder().Append(char.ConvertFromUtf32(0x1F4A9)).ToString());
        Assert.AreEqual(IntStream.EOF, s.LA(2));
        Assert.AreEqual(0, s.Index);
    }

    [TestMethod]
    public void GetTextWithLatin()
    {
        var s = CharStreams.FromString("0123456789");
        Assert.AreEqual("34567", s.GetText(Interval.Of(3, 7)));
    }

    [TestMethod]
    public void GetTextWithCJK()
    {
        var s = CharStreams.FromString("01234\u40946789");
        Assert.AreEqual("34\u409467", s.GetText(Interval.Of(3, 7)));
    }

    [TestMethod]
    public void GetTextWithEmoji()
    {
        var s = CharStreams.FromString(
                new StringBuilder("01234")
                    .Append(char.ConvertFromUtf32(0x1F522))
                    .Append("6789")
                    .ToString());
        Assert.AreEqual("34\uD83D\uDD2267", s.GetText(Interval.Of(3, 7)));
    }

    [TestMethod]
    public void ToStringWithLatin()
    {
        var s = CharStreams.FromString("0123456789");
        Assert.AreEqual("0123456789", s.ToString());
    }

    [TestMethod]
    public void ToStringWithCJK()
    {
        var s = CharStreams.FromString("01234\u40946789");
        Assert.AreEqual("01234\u40946789", s.ToString());
    }

    [TestMethod]
    public void ToStringWithEmoji()
    {
        var s = CharStreams.FromString(
                new StringBuilder("01234").Append(char.ConvertFromUtf32(0x1F522))
                    .Append("6789")
                    .ToString());
        Assert.AreEqual("01234\uD83D\uDD226789", s.ToString());
    }

    [TestMethod]
    public void LookAheadWithLatin()
    {
        var s = CharStreams.FromString("0123456789");
        Assert.AreEqual('5', s.LA(6));
    }

    [TestMethod]
    public void LookAheadWithCJK()
    {
        var s = CharStreams.FromString("01234\u40946789");
        Assert.AreEqual(0x4094, s.LA(6));
    }

    [TestMethod]
    public void LookAheadWithEmoji()
    {
        var s = CharStreams.FromString(
                new StringBuilder("01234").Append(char.ConvertFromUtf32(0x1F522))
                    .Append("6789")
                    .ToString());
        Assert.AreEqual(0x1F522, s.LA(6));
    }

    [TestMethod]
    public void SeekWithLatin()
    {
        var s = CharStreams.FromString("0123456789");
        s.Seek(5);
        Assert.AreEqual('5', s.LA(1));
    }

    [TestMethod]
    public void SeekWithCJK()
    {
        var s = CharStreams.FromString("01234\u40946789");
        s.Seek(5);
        Assert.AreEqual(0x4094, s.LA(1));
    }

    [TestMethod]
    public void SeekWithEmoji()
    {
        var s = CharStreams.FromString(
                new StringBuilder("01234").Append(char.ConvertFromUtf32(0x1F522))
                    .Append("6789")
                    .ToString());
        s.Seek(5);
        Assert.AreEqual(0x1F522, s.LA(1));
    }

    [TestMethod]
    public void LookBehindWithLatin()
    {
        var s = CharStreams.FromString("0123456789");
        s.Seek(6);
        Assert.AreEqual('5', s.LA(-1));
    }

    [TestMethod]
    public void LookBehindWithCJK()
    {
        var s = CharStreams.FromString("01234\u40946789");
        s.Seek(6);
        Assert.AreEqual(0x4094, s.LA(-1));
    }

    [TestMethod]
    public void LookBehindWithEmoji()
    {
        var s = CharStreams.FromString(
                new StringBuilder("01234").Append(char.ConvertFromUtf32(0x1F522))
                    .Append("6789")
                    .ToString());
        s.Seek(6);
        Assert.AreEqual(0x1F522, s.LA(-1));
    }

    [TestMethod]
    public void AsciiContentsShouldUse8BitBuffer()
    {
        var s = CharStreams.FromString("hello");
        Assert.IsTrue(s.GetInternalStorage() is byte[]);
        Assert.AreEqual(5, s.Count);
    }

    [TestMethod]
    public void BmpContentsShouldUse16BitBuffer()
    {
        var s = CharStreams.FromString("hello \u4E16\u754C");
        Assert.IsTrue(s.GetInternalStorage() is char[]);
        Assert.AreEqual(8, s.Count);
    }

    [TestMethod]
    public void SmpContentsShouldUse32BitBuffer()
    {
        var s = CharStreams.FromString("hello \uD83C\uDF0D");
        Assert.IsTrue(s.GetInternalStorage() is int[]);
        Assert.AreEqual(7, s.Count);
    }
}
