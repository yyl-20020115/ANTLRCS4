/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime;

namespace org.antlr.v4.test.runtime.java;

public class TestCharStreams {
	[TestMethod]
	public void fromBMPStringHasExpectedSize() {
		CharStream s = CharStreams.fromString("hello");
		assertEquals(5, s.size());
		assertEquals(0, s.index());
		assertEquals("hello", s.ToString());
	}

	[TestMethod]
	public void fromSMPStringHasExpectedSize() {
		CharStream s = CharStreams.fromString(
				"hello \uD83C\uDF0E");
		assertEquals(7, s.size());
		assertEquals(0, s.index());
		assertEquals("hello \uD83C\uDF0E", s.ToString());
	}

	[TestMethod]
	public void fromBMPUTF8PathHasExpectedSize( string tempDir)  {
		Path test = new File(tempDir.ToString(), "test").toPath();
		Files.write(test, "hello".getBytes(StandardCharsets.UTF_8));
		CharStream s = CharStreams.fromPath(test);
		assertEquals(5, s.size());
		assertEquals(0, s.index());
		assertEquals("hello", s.ToString());
		assertEquals(test.ToString(), s.getSourceName());
	}

	[TestMethod]
	public void fromSMPUTF8PathHasExpectedSize(string tempDir)  {
		Path p = getTestFile(tempDir);
		Files.write(p, "hello \uD83C\uDF0E".getBytes(StandardCharsets.UTF_8));
		CharStream s = CharStreams.fromPath(p);
		assertEquals(7, s.size());
		assertEquals(0, s.index());
		assertEquals("hello \uD83C\uDF0E", s.ToString());
		assertEquals(p.ToString(), s.getSourceName());
	}

	[TestMethod]
	public void fromBMPUTF8InputStreamHasExpectedSize(string tempDir)  {
		Path p = getTestFile(tempDir);
		Files.write(p, "hello".getBytes(StandardCharsets.UTF_8));
		using (InputStream @is = Files.newInputStream(p)) {
			CharStream s = CharStreams.fromStream(@is);
			assertEquals(5, s.size());
			assertEquals(0, s.index());
			assertEquals("hello", s.ToString());
		}
	}

	[TestMethod]
	public void fromSMPUTF8InputStreamHasExpectedSize( Path tempDir)  {
		Path p = getTestFile(tempDir);
		Files.write(p, "hello \uD83C\uDF0E".getBytes(StandardCharsets.UTF_8));
		try (InputStream is = Files.newInputStream(p)) {
			CharStream s = CharStreams.fromStream(is);
			assertEquals(7, s.size());
			assertEquals(0, s.index());
			assertEquals("hello \uD83C\uDF0E", s.ToString());
		}
	}

	[TestMethod]
	public void fromBMPUTF8ChannelHasExpectedSize(string tempDir)  {
		Path p = getTestFile(tempDir);
		Files.write(p, "hello".getBytes(StandardCharsets.UTF_8));
		try (SeekableByteChannel c = Files.newByteChannel(p)) {
			CharStream s = CharStreams.fromChannel(
					c, 4096, CodingErrorAction.REPLACE, "foo");
			assertEquals(5, s.size());
			assertEquals(0, s.index());
			assertEquals("hello", s.ToString());
			assertEquals("foo", s.getSourceName());
		}
	}

	[TestMethod]
	public void fromSMPUTF8ChannelHasExpectedSize(string tempDir)  {
		Path p = getTestFile(tempDir);
		Files.write(p, "hello \uD83C\uDF0E".getBytes(StandardCharsets.UTF_8));
		try (SeekableByteChannel c = Files.newByteChannel(p)) {
			CharStream s = CharStreams.fromChannel(
					c, 4096, CodingErrorAction.REPLACE, "foo");
			assertEquals(7, s.size());
			assertEquals(0, s.index());
			assertEquals("hello \uD83C\uDF0E", s.ToString());
			assertEquals("foo", s.getSourceName());
		}
	}

	[TestMethod]
	public void fromInvalidUTF8BytesChannelReplacesWithSubstCharInReplaceMode(string tempDir)
		 {
		Path p = getTestFile(tempDir);
		byte[] toWrite = new byte[] { (byte)0xCA, (byte)0xFE, (byte)0xFE, (byte)0xED };
		Files.write(p, toWrite);
		try (SeekableByteChannel c = Files.newByteChannel(p)) {
			CharStream s = CharStreams.fromChannel(
					c, 4096, CodingErrorAction.REPLACE, "foo");
			assertEquals(4, s.size());
			assertEquals(0, s.index());
			assertEquals("\uFFFD\uFFFD\uFFFD\uFFFD", s.ToString());
		}
	}

	[TestMethod]
	public void fromInvalidUTF8BytesThrowsInReportMode(string tempDir)  {
		Path p = getTestFile(tempDir);
		byte[] toWrite = new byte[] { (byte)0xCA, (byte)0xFE };
		Files.write(p, toWrite);
		try (SeekableByteChannel c = Files.newByteChannel(p)) {
			assertThrows(
					CharacterCodingException,
					() -> CharStreams.fromChannel(c, 4096, CodingErrorAction.REPORT, "foo")
			);
		}
	}

	[TestMethod]
	public void fromSMPUTF8SequenceStraddlingBufferBoundary(string tempDir)  {
		Path p = getTestFile(tempDir);
		Files.write(p, "hello \uD83C\uDF0E".getBytes(StandardCharsets.UTF_8));
		try (SeekableByteChannel c = Files.newByteChannel(p)) {
			CharStream s = CharStreams.fromChannel(
					c,
					// Note this buffer size ensures the SMP code point
					// straddles the boundary of two buffers
					8,
					CodingErrorAction.REPLACE,
					"foo");
			assertEquals(7, s.size());
			assertEquals(0, s.index());
			assertEquals("hello \uD83C\uDF0E", s.ToString());
		}
	}

	[TestMethod]
	public void fromFileName(string tempDir)  {
		Path p = getTestFile(tempDir);
		Files.write(p, "hello \uD83C\uDF0E".getBytes(StandardCharsets.UTF_8));
		CharStream s = CharStreams.fromFileName(p.ToString());
		assertEquals(7, s.size());
		assertEquals(0, s.index());
		assertEquals("hello \uD83C\uDF0E", s.ToString());
		assertEquals(p.ToString(), s.getSourceName());

	}

	[TestMethod]
	public void fromFileNameWithLatin1(string tempDir)  {
		Path p = getTestFile(tempDir);
		Files.write(p, "hello \u00CA\u00FE".getBytes(StandardCharsets.ISO_8859_1));
		CharStream s = CharStreams.fromFileName(p.ToString(), StandardCharsets.ISO_8859_1);
		assertEquals(8, s.size());
		assertEquals(0, s.index());
		assertEquals("hello \u00CA\u00FE", s.ToString());
		assertEquals(p.ToString(), s.getSourceName());
	}

	[TestMethod]
	public void fromReader(string tempDir)  {
		Path p = getTestFile(tempDir);
		Files.write(p, "hello \uD83C\uDF0E".getBytes(StandardCharsets.UTF_8));
		using (Reader r = Files.newBufferedReader(p, StandardCharsets.UTF_8)) {
			CharStream s = CharStreams.fromReader(r);
			assertEquals(7, s.size());
			assertEquals(0, s.index());
			assertEquals("hello \uD83C\uDF0E", s.ToString());
		}
	}

	[TestMethod]
	public void fromSMPUTF16LEPathSMPHasExpectedSize(string tempDir)  {
		Path p = getTestFile(tempDir);
		Files.write(p, "hello \uD83C\uDF0E".getBytes(StandardCharsets.UTF_16LE));
		CharStream s = CharStreams.fromPath(p, StandardCharsets.UTF_16LE);
		assertEquals(7, s.size());
		assertEquals(0, s.index());
		assertEquals("hello \uD83C\uDF0E", s.ToString());
		assertEquals(p.ToString(), s.getSourceName());
	}

	[TestMethod]
	public void fromSMPUTF32LEPathSMPHasExpectedSize(string tempDir)  {
		Path p = getTestFile(tempDir);
		// UTF-32 isn't popular enough to have an entry in StandardCharsets.
		Charset c = Charset.forName("UTF-32LE");
		Files.write(p, "hello \uD83C\uDF0E".getBytes(c));
		CharStream s = CharStreams.fromPath(p, c);
		assertEquals(7, s.size());
		assertEquals(0, s.index());
		assertEquals("hello \uD83C\uDF0E", s.ToString());
		assertEquals(p.ToString(), s.getSourceName());
	}

	private Path getTestFile(Path dir) {
		return new File(dir.ToString(), "test").toPath();
	}
}
