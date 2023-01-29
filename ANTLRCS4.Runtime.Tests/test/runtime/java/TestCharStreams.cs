/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime;
using System.Text;

namespace org.antlr.v4.test.runtime.java;

public class TestCharStreams {
	[TestMethod]
	public void fromBMPStringHasExpectedSize() {
		CharStream s = CharStreams.fromString("hello");
		Assert.AreEqual(5, s.size());
		Assert.AreEqual(0, s.index());
		Assert.AreEqual("hello", s.ToString());
	}

	[TestMethod]
	public void fromSMPStringHasExpectedSize() {
		CharStream s = CharStreams.fromString(
				"hello \uD83C\uDF0E");
		Assert.AreEqual(7, s.size());
		Assert.AreEqual(0, s.index());
		Assert.AreEqual("hello \uD83C\uDF0E", s.ToString());
	}

	[TestMethod]
	public void fromBMPUTF8PathHasExpectedSize( string tempDir)  {
		string test = Path.Combine(tempDir, "test");
		File.WriteAllBytes(test, Encoding.UTF8.GetBytes("hello"));
		CharStream s = CharStreams.fromPath(test);
		Assert.AreEqual(5, s.size());
		Assert.AreEqual(0, s.index());
		Assert.AreEqual("hello", s.ToString());
		Assert.AreEqual(test.ToString(), s.getSourceName());
	}

	[TestMethod]
	public void fromSMPUTF8PathHasExpectedSize(string tempDir)  {
		string p = getTestFile(tempDir);
		File.WriteAllBytes(p,Encoding.UTF8.GetBytes("hello \uD83C\uDF0E"));
		CharStream s = CharStreams.fromPath(p);
		Assert.AreEqual(7, s.size());
		Assert.AreEqual(0, s.index());
		Assert.AreEqual("hello \uD83C\uDF0E", s.ToString());
		Assert.AreEqual(p.ToString(), s.getSourceName());
	}

	[TestMethod]
	public void fromBMPUTF8InputStreamHasExpectedSize(string tempDir)  {
		string p = getTestFile(tempDir);
		File.WriteAllBytes(p, Encoding.UTF8.GetBytes( "hello"));
		InputStream @is = Files.newInputStream(p);
		{
			CharStream s = CharStreams.fromStream(@is);
			Assert.AreEqual(5, s.size());
			Assert.AreEqual(0, s.index());
			Assert.AreEqual("hello", s.ToString());
		}
	}

	[TestMethod]
	public void fromSMPUTF8InputStreamHasExpectedSize( string tempDir)  {
		string p = getTestFile(tempDir);
		File.WriteAllBytes(p, Encoding.UTF8.GetBytes("hello \uD83C\uDF0E"));
		InputStream @is = Files.newInputStream(p);
			{
			CharStream s = CharStreams.fromStream(@is);
			Assert.AreEqual(7, s.size());
			Assert.AreEqual(0, s.index());
			Assert.AreEqual("hello \uD83C\uDF0E", s.ToString());
		}
	}

	[TestMethod]
	public void fromBMPUTF8ChannelHasExpectedSize(string tempDir)  {
		string p = getTestFile(tempDir);
		File.WriteAllBytes(p,Encoding.UTF8.GetBytes("hello"));
		SeekableByteChannel c = Files.newByteChannel(p);
		{
			CharStream s = CharStreams.fromChannel(
					c, 4096, CodingErrorAction.REPLACE, "foo");
			Assert.AreEqual(5, s.size());
			Assert.AreEqual(0, s.index());
			Assert.AreEqual("hello", s.ToString());
			Assert.AreEqual("foo", s.getSourceName());
		}
	}

	[TestMethod]
	public void fromSMPUTF8ChannelHasExpectedSize(string tempDir)  {
		string p = getTestFile(tempDir);
        File.WriteAllBytes(p, Encoding.UTF8.GetBytes("hello \uD83C\uDF0E"));

		(SeekableByteChannel c = Files.newByteChannel(p);
		{
			CharStream s = CharStreams.fromChannel(
					c, 4096, CodingErrorAction.REPLACE, "foo");
			Assert.AreEqual(7, s.size());
			Assert.AreEqual(0, s.index());
			Assert.AreEqual("hello \uD83C\uDF0E", s.ToString());
			Assert.AreEqual("foo", s.getSourceName());
		}
	}

	[TestMethod]
	public void fromInvalidUTF8BytesChannelReplacesWithSubstCharInReplaceMode(string tempDir)
		 {
		string p = getTestFile(tempDir);
		byte[] toWrite = new byte[] { (byte)0xCA, (byte)0xFE, (byte)0xFE, (byte)0xED };
		File.WriteAllBytes(p, toWrite);
		SeekableByteChannel c = Files.newByteChannel(p);
		{
			CharStream s = CharStreams.fromChannel(
					c, 4096, CodingErrorAction.REPLACE, "foo");
			Assert.AreEqual(4, s.size());
			Assert.AreEqual(0, s.index());
			Assert.AreEqual("\uFFFD\uFFFD\uFFFD\uFFFD", s.ToString());
		}
	}

	[TestMethod]
	public void fromInvalidUTF8BytesThrowsInReportMode(string tempDir)  {
		string p = getTestFile(tempDir);
		byte[] toWrite = new byte[] { (byte)0xCA, (byte)0xFE };
        File.WriteAllBytes(p, toWrite);
        SeekableByteChannel c = Files.newByteChannel(p);
		{
			assertThrows(
					CharacterCodingException,
					() => CharStreams.fromChannel(c, 4096, CodingErrorAction.REPORT, "foo")
			);
		}
	}

	[TestMethod]
	public void fromSMPUTF8SequenceStraddlingBufferBoundary(string tempDir)  {
		string p = getTestFile(tempDir);
		Files.write(p, "hello \uD83C\uDF0E".getBytes(Encoding.UTF8));
		SeekableByteChannel c = Files.newByteChannel(p);
		{
			CharStream s = CharStreams.fromChannel(
					c,
					// Note this buffer size ensures the SMP code point
					// straddles the boundary of two buffers
					8,
					CodingErrorAction.REPLACE,
					"foo");
			Assert.AreEqual(7, s.size());
			Assert.AreEqual(0, s.index());
			Assert.AreEqual("hello \uD83C\uDF0E", s.ToString());
		}
	}

	[TestMethod]
	public void fromFileName(string tempDir)  {
		string p = getTestFile(tempDir);
		Files.write(p, "hello \uD83C\uDF0E".getBytes(Encoding.UTF8));
		CharStream s = CharStreams.fromFileName(p.ToString());
		Assert.AreEqual(7, s.size());
		Assert.AreEqual(0, s.index());
		Assert.AreEqual("hello \uD83C\uDF0E", s.ToString());
		Assert.AreEqual(p.ToString(), s.getSourceName());

	}

	[TestMethod]
	public void fromFileNameWithLatin1(string tempDir)  {
		string p = getTestFile(tempDir);
		Files.write(p, "hello \u00CA\u00FE".getBytes(StandardCharsets.ISO_8859_1));
		CharStream s = CharStreams.fromFileName(p.ToString(), StandardCharsets.ISO_8859_1);
		Assert.AreEqual(8, s.size());
		Assert.AreEqual(0, s.index());
		Assert.AreEqual("hello \u00CA\u00FE", s.ToString());
		Assert.AreEqual(p.ToString(), s.getSourceName());
	}

	[TestMethod]
	public void fromReader(string tempDir)  {
		string p = getTestFile(tempDir);
		File.WriteAllBytes(p,Encoding.UTF8.GetBytes("hello \uD83C\uDF0E"));
		using (TextReader r = Files.newBufferedReader(p, Encoding.UTF8)) {
			CharStream s = CharStreams.fromReader(r);
			Assert.AreEqual(7, s.size());
			Assert.AreEqual(0, s.index());
			Assert.AreEqual("hello \uD83C\uDF0E", s.ToString());
		}
	}

	[TestMethod]
	public void fromSMPUTF16LEPathSMPHasExpectedSize(string tempDir)  {
		string p = getTestFile(tempDir);
		File.WriteAllBytes(p, Encoding.Unicode.GetBytes("hello \uD83C\uDF0E"));
		CharStream s = CharStreams.fromPath(p, Encoding.Unicode);
		Assert.AreEqual(7, s.size());
		Assert.AreEqual(0, s.index());
		Assert.AreEqual("hello \uD83C\uDF0E", s.ToString());
		Assert.AreEqual(p.ToString(), s.getSourceName());
	}

	[TestMethod]
	public void fromSMPUTF32LEPathSMPHasExpectedSize(string tempDir)  {
		string p = getTestFile(tempDir);
		// UTF-32 isn't popular enough to have an entry in StandardCharsets.
		File.WriteAllBytes(p,Encoding.UTF32.GetBytes( "hello \uD83C\uDF0E"));
		CharStream s = CharStreams.fromPath(p, Encoding.UTF32);
		Assert.AreEqual(7, s.size());
		Assert.AreEqual(0, s.index());
		Assert.AreEqual("hello \uD83C\uDF0E", s.ToString());
		Assert.AreEqual(p.ToString(), s.getSourceName());
	}

	private string getTestFile(string dir) {
		return Path.Combine(dir, "test");// new File(dir.ToString(), "test").toPath();
	}
}
