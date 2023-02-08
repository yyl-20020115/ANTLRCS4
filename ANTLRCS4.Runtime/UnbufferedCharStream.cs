/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime.dfa;
using org.antlr.v4.runtime.misc;
using org.antlr.v4.runtime.tree.pattern;
using System.Text;

namespace org.antlr.v4.runtime;


/** Do not buffer up the entire char stream. It does keep a small buffer
 *  for efficiency and also buffers while a mark exists (set by the
 *  lookahead prediction in parser). "Unbuffered" here refers to fact
 *  that it doesn't buffer all data, not that's it's on demand loading of char.
 *
 *  Before 4.7, this class used the default environment encoding to convert
 *  bytes to UTF-16, and held the UTF-16 bytes in the buffer as chars.
 *
 *  As of 4.7, the class uses UTF-8 by default, and the buffer holds Unicode
 *  code points in the buffer as ints.
 */
public class UnbufferedCharStream : CharStream
{
    /**
	 * A moving window buffer of the data being scanned. While there's a marker,
	 * we keep adding to buffer. Otherwise, {@link #consume consume()} resets so
	 * we start filling at index 0 again.
	 */
    protected int[] data;

    /**
	 * The number of characters currently in {@link #data data}.
	 *
	 * <p>This is not the buffer capacity, that's {@code data.length}.</p>
	 */
    protected int n;

    /**
	 * 0..n-1 index into {@link #data data} of next character.
	 *
	 * <p>The {@code LA(1)} character is {@code data[p]}. If {@code p == n}, we are
	 * out of buffered characters.</p>
	 */
    protected int p = 0;

    /**
	 * Count up with {@link #mark mark()} and down with
	 * {@link #release release()}. When we {@code release()} the last mark,
	 * {@code numMarkers} reaches 0 and we reset the buffer. Copy
	 * {@code data[p]..data[n-1]} to {@code data[0]..data[(n-1)-p]}.
	 */
    protected int numMarkers = 0;

    /**
	 * This is the {@code LA(-1)} character for the current position.
	 */
    protected int lastChar = -1;

    /**
	 * When {@code numMarkers > 0}, this is the {@code LA(-1)} character for the
	 * first character in {@link #data data}. Otherwise, this is unspecified.
	 */
    protected int lastCharBufferStart;

    /**
	 * Absolute character index. It's the index of the character about to be
	 * read via {@code LA(1)}. Goes from 0 to the number of characters in the
	 * entire stream, although the stream size is unknown before the end is
	 * reached.
	 */
    protected int currentCharIndex = 0;

    protected TextReader input;

    /** The name or source of this char stream. */
    public String name;

    /** Useful for subclasses that pull char from other than this.input. */
    public UnbufferedCharStream() : this(256)
    {
    }

    /** Useful for subclasses that pull char from other than this.input. */
    public UnbufferedCharStream(int bufferSize)
    {
        n = 0;
        data = new int[bufferSize];
    }

    public UnbufferedCharStream(Stream input) : this(input, 256)
    {
    }

    public UnbufferedCharStream(TextReader input) : this(input, 256) {; }

    public UnbufferedCharStream(Stream input, int bufferSize) : this(input, bufferSize, Encoding.UTF8) {; }

    public UnbufferedCharStream(Stream input, int bufferSize, Encoding charset) : this(bufferSize)
    {
        this.input = new StreamReader(input, charset);
        Fill(1); // prime
    }

    public UnbufferedCharStream(TextReader input, int bufferSize) : this(bufferSize)
    {
        this.input = input;
        Fill(1); // prime
    }

    //@Override
    public void Consume()
    {
        if (LA(1) == IntStream.EOF)
        {
            throw new IllegalStateException("cannot consume EOF");
        }

        // buf always has at least data[p==0] in this method due to ctor
        lastChar = data[p];   // track last char for LA(-1)

        if (p == n - 1 && numMarkers == 0)
        {
            n = 0;
            p = -1; // p++ will leave this at 0
            lastCharBufferStart = lastChar;
        }

        p++;
        currentCharIndex++;
        Sync(1);
    }

    /**
	 * Make sure we have 'need' elements from current position {@link #p p}.
	 * Last valid {@code p} index is {@code data.length-1}. {@code p+need-1} is
	 * the char index 'need' elements ahead. If we need 1 element,
	 * {@code (p+1-1)==p} must be less than {@code data.length}.
	 */
    protected virtual void Sync(int want)
    {
        int need = (p + want - 1) - n + 1; // how many more elements we need?
        if (need > 0) Fill(need);
    }

    /**
	 * Add {@code n} characters to the buffer. Returns the number of characters
	 * actually added to the buffer. If the return value is less than {@code n},
	 * then EOF was reached before {@code n} characters could be added.
	 */
    protected virtual int Fill(int n)
    {
        for (int i = 0; i < n; i++)
        {
            if (this.n > 0 && data[this.n - 1] == IntStream.EOF)
            {
                return i;
            }

            try
            {
                int c = nextChar();
                if (c > char.MaxValue || c == IntStream.EOF)
                {
                    add(c);
                }
                else
                {
                    char ch = (char)c;
                    if (char.IsLowSurrogate(ch))
                    {
                        throw new RuntimeException("Invalid UTF-16 (low surrogate with no preceding high surrogate)");
                    }
                    else if (char.IsHighSurrogate(ch))
                    {
                        int lowSurrogate = nextChar();
                        if (lowSurrogate > char.MaxValue)
                        {
                            throw new RuntimeException("Invalid UTF-16 (high surrogate followed by code point > U+FFFF");
                        }
                        else if (lowSurrogate == IntStream.EOF)
                        {
                            throw new RuntimeException("Invalid UTF-16 (dangling high surrogate at end of file)");
                        }
                        else
                        {
                            char lowSurrogateChar = (char)lowSurrogate;
                            if (char.IsLowSurrogate(lowSurrogateChar))
                            {
                                add(char.ConvertToUtf32(ch, lowSurrogateChar));
                            }
                            else
                            {
                                throw new RuntimeException("Invalid UTF-16 (dangling high surrogate");
                            }
                        }
                    }
                    else
                    {
                        add(c);
                    }
                }
            }
            catch (IOException ioe)
            {
                throw new RuntimeException(ioe.Message, ioe);
            }
        }

        return n;
    }

    /**
	 * Override to provide different source of characters than
	 * {@link #input input}.
	 */
    protected int nextChar()
    {
        return input.Read();
    }

    protected void add(int c)
    {
        if (n >= data.Length)
        {
            data = Arrays.CopyOf(data, data.Length * 2);
        }
        data[n++] = c;
    }

    //@Override
    public int LA(int i)
    {
        if (i == -1) return lastChar; // special case
        Sync(i);
        int index = p + i - 1;
        if (index < 0) throw new IndexOutOfRangeException();
        if (index >= n) return IntStream.EOF;
        return data[index];
    }

    /**
	 * Return a marker that we can release later.
	 *
	 * <p>The specific marker value used for this class allows for some level of
	 * protection against misuse where {@code seek()} is called on a mark or
	 * {@code release()} is called in the wrong order.</p>
	 */
    //@Override
    public int Mark()
    {
        if (numMarkers == 0)
        {
            lastCharBufferStart = lastChar;
        }

        int mark = -numMarkers - 1;
        numMarkers++;
        return mark;
    }

    /** Decrement number of markers, resetting buffer if we hit 0.
	 * @param marker
	 */
    //@Override
    public void Release(int marker)
    {
        int expectedMark = -numMarkers;
        if (marker != expectedMark)
        {
            throw new IllegalStateException("release() called with an invalid marker.");
        }

        numMarkers--;
        if (numMarkers == 0 && p > 0)
        { // release buffer when we can, but don't do unnecessary work
          // Copy data[p]..data[n-1] to data[0]..data[(n-1)-p], reset ptrs
          // p is last valid char; move nothing if p==n as we have no valid char
            Array.Copy(data, p, data, 0, n - p); // shift n-p char from p to 0
            n -= p;
            p = 0;
            lastCharBufferStart = lastChar;
        }
    }

    //@Override
    public int Index => currentCharIndex;

    /** Seek to absolute character index, which might not be in the current
	 *  sliding window.  Move {@code p} to {@code index-bufferStartIndex}.
	 */
    //@Override
    public void Seek(int index)
    {
        if (index == currentCharIndex)
        {
            return;
        }

        if (index > currentCharIndex)
        {
            Sync(index - currentCharIndex);
            index = Math.Min(index, GetBufferStartIndex() + n - 1);
        }

        // index == to bufferStartIndex should set p to 0
        int i = index - GetBufferStartIndex();
        if (i < 0)
        {
            throw new ArgumentException("cannot seek to negative index " + index);
        }
        else if (i >= n)
        {
            throw new UnsupportedOperationException("seek to index outside buffer: " +
                    index + " not in " + GetBufferStartIndex() + ".." + (GetBufferStartIndex() + n));
        }

        p = i;
        currentCharIndex = index;
        if (p == 0)
        {
            lastChar = lastCharBufferStart;
        }
        else
        {
            lastChar = data[p - 1];
        }
    }

    //@Override
    public int Count => throw new UnsupportedOperationException("Unbuffered stream cannot know its size");

    //@Override
    public String SourceName
    {
        get
        {
            if (string.IsNullOrEmpty(name))
            {
                return IntStream.UNKNOWN_SOURCE_NAME;
            }

            return name;
        }
    }

    //@Override
    public string GetText(Interval interval)
    {
        if (interval.a < 0 || interval.b < interval.a - 1)
        {
            throw new ArgumentException("invalid interval");
        }

        int bufferStartIndex = GetBufferStartIndex();
        if (n > 0 && data[n - 1] == char.MaxValue)
        {
            if (interval.a + interval.Length > bufferStartIndex + n)
            {
                throw new ArgumentException("the interval extends past the end of the stream");
            }
        }

        if (interval.a < bufferStartIndex || interval.b >= bufferStartIndex + n)
        {
            throw new UnsupportedOperationException("interval " + interval + " outside buffer: " +
                                bufferStartIndex + ".." + (bufferStartIndex + n - 1));
        }
        // convert from absolute to local index
        int i = interval.a - bufferStartIndex;
        var runs = data.Select(d => new Rune(d)).ToArray()[i..interval.Length].ToArray();
        var builder = new StringBuilder();
        foreach (var r in runs)
        {
            builder.Append(r.ToString());
        }
        return builder.ToString();
    }

    protected int GetBufferStartIndex() => currentCharIndex - p;

    public int CharPositionInLine => throw new NotImplementedException();

    public int Line => throw new NotImplementedException();

    public void Rewind(int start)
    {
        throw new NotImplementedException();
    }

    public int LT(int v)
    {
        throw new NotImplementedException();
    }

    public string Substring(int tokenStartCharIndex, int v)
    {
        throw new NotImplementedException();
    }

    public void Rewind()
    {
        throw new NotImplementedException();
    }
}
