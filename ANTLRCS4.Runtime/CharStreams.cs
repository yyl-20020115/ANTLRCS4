/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file @is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using System.Text;

namespace org.antlr.v4.runtime;

/** This class represents the primary interface for creating {@link CharStream}s
 *  from a variety of sources as of 4.7.  The motivation was to support
 *  Unicode code points > U+FFFF.  {@link ANTLRInputStream} and
 *  {@link ANTLRFileStream} are now deprecated in favor of the streams created
 *  by this interface.
 *
 *  DEPRECATED: {@code new ANTLRFileStream("myinputfile")}
 *  NEW:        {@code CharStreams.fromFileName("myinputfile")}
 *
 *  WARNING: If you use both the deprecated and the new streams, you will see
 *  a nontrivial performance degradation. This speed hit @is because the
 *  {@link Lexer}'s internal code goes from a monomorphic to megamorphic
 *  dynamic dispatch to get characters from the input stream. Java's
 *  on-the-fly compiler (JIT) @is unable to perform the same optimizations
 *  so stick with either the old or the new streams, if performance @is
 *  a primary concern. See the extreme debugging and spelunking
 *  needed to identify this issue in our timing rig:
 *
 *      https://github.com/antlr/antlr4/pull/1781
 *
 *  The ANTLR character streams still buffer all the input when you create
 *  the stream, as they have done for ~20 years. If you need unbuffered
 *  access, please note that it becomes challenging to create
 *  parse trees. The parse tree has to point to tokens which will either
 *  point into a stale location in an unbuffered stream or you have to copy
 *  the characters out of the buffer into the token. That defeats the purpose
 *  of unbuffered input. Per the ANTLR book, unbuffered streams are primarily
 *  useful for processing infinite streams *during the parse.*
 *
 *  The new streams also use 8-bit buffers when possible so this new
 *  interface supports character streams that use half as much memory
 *  as the old {@link ANTLRFileStream}, which assumed 16-bit characters.
 *
 *  A big shout out to Ben Hamilton (github bhamiltoncx) for his superhuman
 *  efforts across all targets to get true Unicode 3.1 support for U+10FFFF.
 *
 *  @since 4.7
 */
public class CharStreams
{
    private static int DEFAULT_BUFFER_SIZE = 4096;

    // Utility class; do not construct.
    private CharStreams() { }

    /**
	 * Creates a {@link CharStream} given a path to a UTF-8
	 * encoded file on disk.
	 *
	 * Reads the entire contents of the file into the result before returning.
	 */
    public static CharStream FromPath(string path)
    {
        return FromReader(new StreamReader(path, Encoding.UTF8));
    }
    public static CharStream FromPath(string path, Encoding encoding)
    {
        return FromReader(new StreamReader(path, encoding));
    }

    /**
	 * Creates a {@link CharStream} given a path to a file on disk and the
	 * charset of the bytes contained in the file.
	 *
	 * Reads the entire contents of the file into the result before returning.
	 */
    //public static CharStream fromPath(string path, Encoding charset){
    //	long size = Files.size(path);
    //	using (ReadableByteChannel channel = Files.newByteChannel(path)) {
    //		return fromChannel(
    //			channel,
    //			charset,
    //			DEFAULT_BUFFER_SIZE,
    //			CodingErrorAction.REPLACE,
    //			path.toString(),
    //			size);
    //	}
    //}

    /**
	 * Creates a {@link CharStream} given a string containing a
	 * path to a UTF-8 file on disk.
	 *
	 * Reads the entire contents of the file into the result before returning.
	 */
    public static CharStream FromFileName(string fileName)
    {
        return FromReader(new StreamReader(fileName, Encoding.UTF8));
    }

    /**
	 * Creates a {@link CharStream} given a string containing a
	 * path to a file on disk and the charset of the bytes
	 * contained in the file.
	 *
	 * Reads the entire contents of the file into the result before returning.
	 */
    public static CharStream FromFileName(string fileName, Encoding charset)
    {
        return FromReader(new StreamReader(fileName, charset));
    }


    /**
	 * Creates a {@link CharStream} given an opened {@link InputStream}
	 * containing UTF-8 bytes.
	 *
	 * Reads the entire contents of the {@code InputStream} into
	 * the result before returning, then closes the {@code InputStream}.
	 */
    //public static CharStream fromStream(Stream @is){
    //	return fromStream(@is, Encoding.UTF8);
    //}

    /**
	 * Creates a {@link CharStream} given an opened {@link InputStream} and the
	 * charset of the bytes contained in the stream.
	 *
	 * Reads the entire contents of the {@code InputStream} into
	 * the result before returning, then closes the {@code InputStream}.
	 */
    //public static CharStream fromStream(Stream @is, Encoding charset){
    //	return fromStream(@is, charset, -1);
    //}

    //public static CharStream fromStream(Stream @is, Encoding charset, long inputSize){
    //	using (ReadableByteChannel channel = Channels.newChannel(@is)) {
    //		return fromChannel(
    //			channel,
    //			charset,
    //			DEFAULT_BUFFER_SIZE,
    //			CodingErrorAction.REPLACE,
    //			IntStream.UNKNOWN_SOURCE_NAME,
    //			inputSize);
    //	}
    //}

    /**
	 * Creates a {@link CharStream} given an opened {@link ReadableByteChannel}
	 * containing UTF-8 bytes.
	 *
	 * Reads the entire contents of the {@code channel} into
	 * the result before returning, then closes the {@code channel}.
	 */
    //public static CharStream fromChannel(ReadableByteChannel channel){
    //	return fromChannel(channel, StandardCharsets.UTF_8);
    //}

    /**
	 * Creates a {@link CharStream} given an opened {@link ReadableByteChannel} and the
	 * charset of the bytes contained in the channel.
	 *
	 * Reads the entire contents of the {@code channel} into
	 * the result before returning, then closes the {@code channel}.
	 */
    //public static CharStream fromChannel(ReadableByteChannel channel, Encoding charset){
    //	return fromChannel(
    //		channel,
    //		DEFAULT_BUFFER_SIZE,
    //		CodingErrorAction.REPLACE,
    //		IntStream.UNKNOWN_SOURCE_NAME);
    //}

    /**
	 * Creates a {@link CharStream} given a {@link Reader}. Closes
	 * the reader before returning.
	 */
    public static CodePointCharStream FromReader(TextReader r)
    {
        return FromReader(r, IntStream.UNKNOWN_SOURCE_NAME);
    }

    /**
	 * Creates a {@link CharStream} given a {@link Reader} and its
	 * source name. Closes the reader before returning.
	 */
    public static CodePointCharStream FromReader(TextReader r, string sourceName)
    {
        try
        {
            var codePointBufferBuilder = CodePointBuffer.GetBuilder(DEFAULT_BUFFER_SIZE);
            var charBuffer = CharBuffer.Allocate(DEFAULT_BUFFER_SIZE);
            while ((r.Read(/*charBuffer*/)) != -1)
            {
                //charBuffer.flip();
                codePointBufferBuilder.Append(charBuffer);
                //charBuffer.compact();
            }
            return CodePointCharStream.FromBuffer(codePointBufferBuilder.Build(), sourceName);
        }
        finally
        {
            r.Close();
        }
    }

    /**
	 * Creates a {@link CharStream} given a {@link string}.
	 */
    public static CodePointCharStream FromString(string s)
    {
        return FromString(s, IntStream.UNKNOWN_SOURCE_NAME);
    }

    /**
	 * Creates a {@link CharStream} given a {@link string} and the {@code sourceName}
	 * from which it came.
	 */
    public static CodePointCharStream FromString(string s, string sourceName)
    {
        // Initial guess assumes no code points > U+FFFF: one code
        // point for each code unit in the string
        var codePointBufferBuilder = CodePointBuffer.GetBuilder(s.Length);
        // TODO: CharBuffer.wrap(string) rightfully returns a read-only buffer
        // which doesn't expose its array, so we make a copy.
        var cb = CharBuffer.Allocate(s.Length);
        cb.Put(s);
        cb.Flip();
        codePointBufferBuilder.Append(cb);
        return CodePointCharStream.FromBuffer(codePointBufferBuilder.Build(), sourceName);
    }

    /**
	 * Creates a {@link CharStream} given an opened {@link ReadableByteChannel}
	 * containing UTF-8 bytes.
	 *
	 * Reads the entire contents of the {@code channel} into
	 * the result before returning, then closes the {@code channel}.
	 */
    //public static CodePointCharStream fromChannel(
    //	ReadableByteChannel channel,
    //	int bufferSize,
    //	CodingErrorAction decodingErrorAction,
    //	string sourceName)

    //{
    //	return fromChannel(channel, StandardCharsets.UTF_8, bufferSize, decodingErrorAction, sourceName, -1);
    //}

    //public static CodePointCharStream fromChannel(
    //	ReadableByteChannel channel,
    //	Encoding charset,
    //	int bufferSize,
    //	CodingErrorAction decodingErrorAction,
    //	string sourceName,
    //	long inputSize)

    //{
    //	try {
    //		ByteBuffer utf8BytesIn = ByteBuffer.allocate(bufferSize);
    //		CharBuffer utf16CodeUnitsOut = CharBuffer.allocate(bufferSize);
    //		if (inputSize == -1) {
    //			inputSize = bufferSize;
    //		} else if (inputSize > int.MaxValue) {
    //			// ByteBuffer et al don't support long sizes
    //			throw new IOException(string.format("inputSize %d larger than max %d", inputSize, int.MaxValue));
    //		}
    //		CodePointBuffer.Builder codePointBufferBuilder = CodePointBuffer.builder((int) inputSize);
    //		CharsetDecoder decoder = charset
    //				.newDecoder()
    //				.onMalformedInput(decodingErrorAction)
    //				.onUnmappableCharacter(decodingErrorAction);

    //		bool endOfInput = false;
    //		while (!endOfInput) {
    //			int bytesRead = channel.read(utf8BytesIn);
    //			endOfInput = (bytesRead == -1);
    //			utf8BytesIn.flip();
    //			CoderResult result = decoder.decode(
    //				utf8BytesIn,
    //				utf16CodeUnitsOut,
    //				endOfInput);
    //			if (result.isError() && decodingErrorAction.Equals(CodingErrorAction.REPORT)) {
    //				result.throwException();
    //			}
    //			utf16CodeUnitsOut.flip();
    //			codePointBufferBuilder.append(utf16CodeUnitsOut);
    //			utf8BytesIn.compact();
    //			utf16CodeUnitsOut.compact();
    //		}
    //		// Handle any bytes at the end of the file which need to
    //		// be represented as errors or substitution characters.
    //		CoderResult flushResult = decoder.flush(utf16CodeUnitsOut);
    //		if (flushResult.isError() && decodingErrorAction.Equals(CodingErrorAction.REPORT)) {
    //			flushResult.throwException();
    //		}
    //		utf16CodeUnitsOut.flip();
    //		codePointBufferBuilder.append(utf16CodeUnitsOut);

    //		CodePointBuffer codePointBuffer = codePointBufferBuilder.build();
    //		return CodePointCharStream.fromBuffer(codePointBuffer, sourceName);
    //	}
    //	finally {
    //		channel.close();
    //	}
    //}
}
