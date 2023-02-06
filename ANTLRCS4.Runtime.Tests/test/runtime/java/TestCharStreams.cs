/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime;
using System.Text;

namespace org.antlr.v4.test.runtime.java;

[TestClass]
public class TestCharStreams
{
    [TestMethod]
    public void FromBMPStringHasExpectedSize()
    {
        var s = CharStreams.fromString("hello");
        Assert.AreEqual(5, s.Count);
        Assert.AreEqual(0, s.Index());
        Assert.AreEqual("hello", s.ToString());
    }

    [TestMethod]
    public void FromSMPStringHasExpectedSize()
    {
        var s = CharStreams.fromString(
                "hello \uD83C\uDF0E");
        Assert.AreEqual(7, s.Count);
        Assert.AreEqual(0, s.Index());
        Assert.AreEqual("hello \uD83C\uDF0E", s.ToString());
    }

    [TestMethod]
    public void FromBMPUTF8PathHasExpectedSize(string tempDir)
    {
        var test = Path.Combine(tempDir, "test");
        File.WriteAllBytes(test, Encoding.UTF8.GetBytes("hello"));
        var s = CharStreams.fromPath(test);
        Assert.AreEqual(5, s.Count);
        Assert.AreEqual(0, s.Index());
        Assert.AreEqual("hello", s.ToString());
        Assert.AreEqual(test.ToString(), s.getSourceName());
    }

    [TestMethod]
    public void FromSMPUTF8PathHasExpectedSize(string tempDir)
    {
        var p = GetTestFile(tempDir);
        File.WriteAllBytes(p, Encoding.UTF8.GetBytes("hello \uD83C\uDF0E"));
        var s = CharStreams.fromPath(p);
        Assert.AreEqual(7, s.Count);
        Assert.AreEqual(0, s.Index());
        Assert.AreEqual("hello \uD83C\uDF0E", s.ToString());
        Assert.AreEqual(p.ToString(), s.getSourceName());
    }

    [TestMethod]
    public void FromBMPUTF8InputStreamHasExpectedSize(string tempDir)
    {
        var p = GetTestFile(tempDir);
        File.WriteAllBytes(p, Encoding.UTF8.GetBytes("hello"));
        {
            var s = CharStreams.fromReader(new StreamReader(p));
            Assert.AreEqual(5, s.Count);
            Assert.AreEqual(0, s.Index());
            Assert.AreEqual("hello", s.ToString());
        }
    }

    [TestMethod]
    public void FromSMPUTF8InputStreamHasExpectedSize(string tempDir)
    {
        var p = GetTestFile(tempDir);
        File.WriteAllBytes(p, Encoding.UTF8.GetBytes("hello \uD83C\uDF0E"));
        {
            var s = CharStreams.fromReader(new StreamReader(p));
            Assert.AreEqual(7, s.Count);
            Assert.AreEqual(0, s.Index());
            Assert.AreEqual("hello \uD83C\uDF0E", s.ToString());
        }
    }

    [TestMethod]
    public void FromBMPUTF8ChannelHasExpectedSize(string tempDir)
    {
        var p = GetTestFile(tempDir);
        File.WriteAllBytes(p, Encoding.UTF8.GetBytes("hello"));
        {
            var s = CharStreams.fromReader(new StreamReader(p));
            Assert.AreEqual(5, s.Count);
            Assert.AreEqual(0, s.Index());
            Assert.AreEqual("hello", s.ToString());
            Assert.AreEqual("foo", s.getSourceName());
        }
    }

    [TestMethod]
    public void FromSMPUTF8ChannelHasExpectedSize(string tempDir)
    {
        var p = GetTestFile(tempDir);
        File.WriteAllBytes(p, Encoding.UTF8.GetBytes("hello \uD83C\uDF0E"));
        {
            var s = CharStreams.fromReader(new StreamReader(p));
            Assert.AreEqual(7, s.Count);
            Assert.AreEqual(0, s.Index());
            Assert.AreEqual("hello \uD83C\uDF0E", s.ToString());
            Assert.AreEqual("foo", s.getSourceName());
        }
    }

    [TestMethod]
    public void FromInvalidUTF8BytesChannelReplacesWithSubstCharInReplaceMode(string tempDir)
    {
        var p = GetTestFile(tempDir);
        var toWrite = new byte[] { (byte)0xCA, (byte)0xFE, (byte)0xFE, (byte)0xED };
        File.WriteAllBytes(p, toWrite);
        {
            var s = CharStreams.fromReader(new StreamReader(p));
            Assert.AreEqual(4, s.Count);
            Assert.AreEqual(0, s.Index());
            Assert.AreEqual("\uFFFD\uFFFD\uFFFD\uFFFD", s.ToString());
        }
    }

    [TestMethod]
    public void FromInvalidUTF8BytesThrowsInReportMode(string tempDir)
    {
        var p = GetTestFile(tempDir);
        var toWrite = new byte[] { (byte)0xCA, (byte)0xFE };
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
        var p = GetTestFile(tempDir);
        File.WriteAllBytes(p, Encoding.UTF8.GetBytes("hello \uD83C\uDF0E"));
        {
            var s = CharStreams.fromReader(new StreamReader(p));
            Assert.AreEqual(7, s.Count);
            Assert.AreEqual(0, s.Index());
            Assert.AreEqual("hello \uD83C\uDF0E", s.ToString());
        }
    }

    [TestMethod]
    public void FromFileName(string tempDir)
    {
        var p = GetTestFile(tempDir);
        File.WriteAllBytes(p, Encoding.UTF8.GetBytes("hello \uD83C\uDF0E"));
        var s = CharStreams.fromFileName(p.ToString());
        Assert.AreEqual(7, s.Count);
        Assert.AreEqual(0, s.Index());
        Assert.AreEqual("hello \uD83C\uDF0E", s.ToString());
        Assert.AreEqual(p.ToString(), s.getSourceName());

    }

    [TestMethod]
    public void FromFileNameWithLatin1(string tempDir)
    {
        var p = GetTestFile(tempDir);
        File.WriteAllBytes(p, Encoding.Latin1.GetBytes("hello \u00CA\u00FE"));
        var s = CharStreams.fromPath(p.ToString(), Encoding.Latin1);
        Assert.AreEqual(8, s.Count);
        Assert.AreEqual(0, s.Index());
        Assert.AreEqual("hello \u00CA\u00FE", s.ToString());
        Assert.AreEqual(p.ToString(), s.getSourceName());
    }

    [TestMethod]
    public void FromReader(string tempDir)
    {
        var p = GetTestFile(tempDir);
        File.WriteAllBytes(p, Encoding.UTF8.GetBytes("hello \uD83C\uDF0E"));
        var s = CharStreams.fromPath(p, Encoding.UTF8);
        Assert.AreEqual(7, s.Count);
        Assert.AreEqual(0, s.Index());
        Assert.AreEqual("hello \uD83C\uDF0E", s.ToString());
    }

    [TestMethod]
    public void FromSMPUTF16LEPathSMPHasExpectedSize(string tempDir)
    {
        var p = GetTestFile(tempDir);
        File.WriteAllBytes(p, Encoding.Unicode.GetBytes("hello \uD83C\uDF0E"));
        var s = CharStreams.fromPath(p, Encoding.Unicode);
        Assert.AreEqual(7, s.Count);
        Assert.AreEqual(0, s.Index());
        Assert.AreEqual("hello \uD83C\uDF0E", s.ToString());
        Assert.AreEqual(p.ToString(), s.getSourceName());
    }

    [TestMethod]
    public void FromSMPUTF32LEPathSMPHasExpectedSize(string tempDir)
    {
        var p = GetTestFile(tempDir);
        // UTF-32 isn't popular enough to have an entry in StandardCharsets.
        File.WriteAllBytes(p, Encoding.UTF32.GetBytes("hello \uD83C\uDF0E"));
        var s = CharStreams.fromPath(p, Encoding.UTF32);
        Assert.AreEqual(7, s.Count);
        Assert.AreEqual(0, s.Index());
        Assert.AreEqual("hello \uD83C\uDF0E", s.ToString());
        Assert.AreEqual(p.ToString(), s.getSourceName());
    }

    protected static string GetTestFile(string dir)
    {
        return Path.Combine(dir, "test");// new File(dir.ToString(), "test").toPath();
    }
}
