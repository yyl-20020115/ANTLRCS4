/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime;
using System.Text;

namespace org.antlr.v4.test.runtime.java;

public class TestCharStreams
{
    [TestMethod]
    public void FromBMPStringHasExpectedSize()
    {
        CharStream s = CharStreams.fromString("hello");
        Assert.AreEqual(5, s.Count);
        Assert.AreEqual(0, s.index());
        Assert.AreEqual("hello", s.ToString());
    }

    [TestMethod]
    public void FromSMPStringHasExpectedSize()
    {
        CharStream s = CharStreams.fromString(
                "hello \uD83C\uDF0E");
        Assert.AreEqual(7, s.Count);
        Assert.AreEqual(0, s.index());
        Assert.AreEqual("hello \uD83C\uDF0E", s.ToString());
    }

    [TestMethod]
    public void FromBMPUTF8PathHasExpectedSize(string tempDir)
    {
        string test = Path.Combine(tempDir, "test");
        File.WriteAllBytes(test, Encoding.UTF8.GetBytes("hello"));
        CharStream s = CharStreams.fromPath(test);
        Assert.AreEqual(5, s.Count);
        Assert.AreEqual(0, s.index());
        Assert.AreEqual("hello", s.ToString());
        Assert.AreEqual(test.ToString(), s.getSourceName());
    }

    [TestMethod]
    public void FromSMPUTF8PathHasExpectedSize(string tempDir)
    {
        string p = GetTestFile(tempDir);
        File.WriteAllBytes(p, Encoding.UTF8.GetBytes("hello \uD83C\uDF0E"));
        CharStream s = CharStreams.fromPath(p);
        Assert.AreEqual(7, s.Count);
        Assert.AreEqual(0, s.index());
        Assert.AreEqual("hello \uD83C\uDF0E", s.ToString());
        Assert.AreEqual(p.ToString(), s.getSourceName());
    }

    [TestMethod]
    public void FromBMPUTF8InputStreamHasExpectedSize(string tempDir)
    {
        string p = GetTestFile(tempDir);
        File.WriteAllBytes(p, Encoding.UTF8.GetBytes("hello"));
        {
            CharStream s = CharStreams.fromReader(new StreamReader(p));
            Assert.AreEqual(5, s.Count);
            Assert.AreEqual(0, s.index());
            Assert.AreEqual("hello", s.ToString());
        }
    }

    [TestMethod]
    public void FromSMPUTF8InputStreamHasExpectedSize(string tempDir)
    {
        string p = GetTestFile(tempDir);
        File.WriteAllBytes(p, Encoding.UTF8.GetBytes("hello \uD83C\uDF0E"));
        {
            CharStream s = CharStreams.fromReader(new StreamReader(p));
            Assert.AreEqual(7, s.Count);
            Assert.AreEqual(0, s.index());
            Assert.AreEqual("hello \uD83C\uDF0E", s.ToString());
        }
    }

    [TestMethod]
    public void FromBMPUTF8ChannelHasExpectedSize(string tempDir)
    {
        string p = GetTestFile(tempDir);
        File.WriteAllBytes(p, Encoding.UTF8.GetBytes("hello"));
        {
            CharStream s = CharStreams.fromReader(new StreamReader(p));
            Assert.AreEqual(5, s.Count);
            Assert.AreEqual(0, s.index());
            Assert.AreEqual("hello", s.ToString());
            Assert.AreEqual("foo", s.getSourceName());
        }
    }

    [TestMethod]
    public void FromSMPUTF8ChannelHasExpectedSize(string tempDir)
    {
        string p = GetTestFile(tempDir);
        File.WriteAllBytes(p, Encoding.UTF8.GetBytes("hello \uD83C\uDF0E"));

        {
            CharStream s = CharStreams.fromReader(new StreamReader(p));
            Assert.AreEqual(7, s.Count);
            Assert.AreEqual(0, s.index());
            Assert.AreEqual("hello \uD83C\uDF0E", s.ToString());
            Assert.AreEqual("foo", s.getSourceName());
        }
    }

    [TestMethod]
    public void FromInvalidUTF8BytesChannelReplacesWithSubstCharInReplaceMode(string tempDir)
    {
        string p = GetTestFile(tempDir);
        byte[] toWrite = new byte[] { (byte)0xCA, (byte)0xFE, (byte)0xFE, (byte)0xED };
        File.WriteAllBytes(p, toWrite);
        {
            CharStream s = CharStreams.fromReader(new StreamReader(p));
            Assert.AreEqual(4, s.Count);
            Assert.AreEqual(0, s.index());
            Assert.AreEqual("\uFFFD\uFFFD\uFFFD\uFFFD", s.ToString());
        }
    }

    [TestMethod]
    public void FromInvalidUTF8BytesThrowsInReportMode(string tempDir)
    {
        string p = GetTestFile(tempDir);
        byte[] toWrite = new byte[] { (byte)0xCA, (byte)0xFE };
        File.WriteAllBytes(p, toWrite);
        {
            Assert.ThrowsException<InvalidOperationException>(
                    () => { CharStream s = CharStreams.fromReader(new StreamReader(p)); }

            );
        }
    }

    [TestMethod]
    public void FromSMPUTF8SequenceStraddlingBufferBoundary(string tempDir)
    {
        string p = GetTestFile(tempDir);
        File.WriteAllBytes(p, Encoding.UTF8.GetBytes("hello \uD83C\uDF0E"));
        {
            CharStream s = CharStreams.fromReader(new StreamReader(p));
            Assert.AreEqual(7, s.Count);
            Assert.AreEqual(0, s.index());
            Assert.AreEqual("hello \uD83C\uDF0E", s.ToString());
        }
    }

    [TestMethod]
    public void FromFileName(string tempDir)
    {
        string p = GetTestFile(tempDir);
        File.WriteAllBytes(p, Encoding.UTF8.GetBytes("hello \uD83C\uDF0E"));
        CharStream s = CharStreams.fromFileName(p.ToString());
        Assert.AreEqual(7, s.Count);
        Assert.AreEqual(0, s.index());
        Assert.AreEqual("hello \uD83C\uDF0E", s.ToString());
        Assert.AreEqual(p.ToString(), s.getSourceName());

    }

    [TestMethod]
    public void FromFileNameWithLatin1(string tempDir)
    {
        string p = GetTestFile(tempDir);
        File.WriteAllBytes(p, Encoding.Latin1.GetBytes("hello \u00CA\u00FE"));
        CharStream s = CharStreams.fromPath(p.ToString(), Encoding.Latin1);
        Assert.AreEqual(8, s.Count);
        Assert.AreEqual(0, s.index());
        Assert.AreEqual("hello \u00CA\u00FE", s.ToString());
        Assert.AreEqual(p.ToString(), s.getSourceName());
    }

    [TestMethod]
    public void FromReader(string tempDir)
    {
        string p = GetTestFile(tempDir);
        File.WriteAllBytes(p, Encoding.UTF8.GetBytes("hello \uD83C\uDF0E"));
        CharStream s = CharStreams.fromPath(p, Encoding.UTF8);
        Assert.AreEqual(7, s.Count);
        Assert.AreEqual(0, s.index());
        Assert.AreEqual("hello \uD83C\uDF0E", s.ToString());
    }

    [TestMethod]
    public void FromSMPUTF16LEPathSMPHasExpectedSize(string tempDir)
    {
        string p = GetTestFile(tempDir);
        File.WriteAllBytes(p, Encoding.Unicode.GetBytes("hello \uD83C\uDF0E"));
        CharStream s = CharStreams.fromPath(p, Encoding.Unicode);
        Assert.AreEqual(7, s.Count);
        Assert.AreEqual(0, s.index());
        Assert.AreEqual("hello \uD83C\uDF0E", s.ToString());
        Assert.AreEqual(p.ToString(), s.getSourceName());
    }

    [TestMethod]
    public void FromSMPUTF32LEPathSMPHasExpectedSize(string tempDir)
    {
        string p = GetTestFile(tempDir);
        // UTF-32 isn't popular enough to have an entry in StandardCharsets.
        File.WriteAllBytes(p, Encoding.UTF32.GetBytes("hello \uD83C\uDF0E"));
        CharStream s = CharStreams.fromPath(p, Encoding.UTF32);
        Assert.AreEqual(7, s.Count);
        Assert.AreEqual(0, s.index());
        Assert.AreEqual("hello \uD83C\uDF0E", s.ToString());
        Assert.AreEqual(p.ToString(), s.getSourceName());
    }

    protected string GetTestFile(string dir)
    {
        return Path.Combine(dir, "test");// new File(dir.ToString(), "test").toPath();
    }
}
