/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */
using org.antlr.v4.runtime;
using org.antlr.v4.runtime.dfa;
using org.antlr.v4.runtime.misc;

/**
 * Vacuum all input from a {@link Reader}/{@link InputStream} and then treat it
 * like a {@code char[]} buffer. Can also pass in a {@link string} or
 * {@code char[]} to use.
 *
 * <p>If you need encoding, pass in stream/reader with correct encoding.</p>
 *
 * @deprecated as of 4.7 Please use {@link CharStreams} interface.
 */
//@Deprecated
public class ANTLRInputStream : CharStream
{
    public static readonly int READ_BUFFER_SIZE = 1024;
    public static readonly int INITIAL_BUFFER_SIZE = 1024;

    /** The data being scanned */
    protected char[] data;

    /** How many characters are actually in the buffer */
    protected int n;

    /** 0..n-1 index into string of next char */
    protected int p = 0;

    /** What is name or source of this char stream? */
    public string name;

    public ANTLRInputStream() { }

    /** Copy data in string to a local char array */
    public ANTLRInputStream(string input)
    {
        this.data = input.ToCharArray();
        this.n = input.Length;
    }

    /** This is the preferred constructor for strings as no data is copied */
    public ANTLRInputStream(char[] data, int numberOfActualCharsInArray)
    {
        this.data = data;
        this.n = numberOfActualCharsInArray;
    }

    public ANTLRInputStream(TextReader r) : this(r, INITIAL_BUFFER_SIZE, READ_BUFFER_SIZE)
    {
    }

    public ANTLRInputStream(TextReader r, int initialSize) : this(r, initialSize, READ_BUFFER_SIZE)
    {
    }

    public ANTLRInputStream(TextReader r, int initialSize, int readChunkSize)
    {
        Load(r, initialSize, readChunkSize);
    }

    public ANTLRInputStream(Stream input) : this(new StreamReader(input), INITIAL_BUFFER_SIZE)
    {
    }

    public ANTLRInputStream(Stream input, int initialSize) : this(new StreamReader(input), initialSize)
    {
    }

    public ANTLRInputStream(Stream input, int initialSize, int readChunkSize) : this(new StreamReader(input), initialSize, readChunkSize)
    {
    }

    public void Load(TextReader r, int size, int readChunkSize)
    {
        if (r == null)
        {
            return;
        }
        if (size <= 0)
        {
            size = INITIAL_BUFFER_SIZE;
        }
        if (readChunkSize <= 0)
        {
            readChunkSize = READ_BUFFER_SIZE;
        }
        // Console.Out.WriteLine("load "+size+" in chunks of "+readChunkSize);
        try
        {
            // alloc initial buffer size.
            data = new char[size];
            // read all the data in chunks of readChunkSize
            int numRead = 0;
            int p = 0;
            do
            {
                if (p + readChunkSize > data.Length)
                { // overflow?
                  // Console.Out.WriteLine("### overflow p="+p+", data.length="+data.length);
                    data = Arrays.CopyOf(data, data.Length * 2);
                }
                numRead = r.Read(data, p, readChunkSize);
                // Console.Out.WriteLine("read "+numRead+" chars; p was "+p+" is now "+(p+numRead));
                p += numRead;
            } while (numRead != -1); // while not EOF
                                     // set the actual size of the data available;
                                     // EOF subtracted one above in p+=numRead; add one back
            n = p + 1;
            //Console.Out.WriteLine("n="+n);
        }
        finally
        {
            r.Close();
        }
    }

    /** Reset the stream so that it's in the same state it was
	 *  when the object was created *except* the data array is not
	 *  touched.
	 */
    public void Reset()
    {
        p = 0;
    }

    
    public virtual void Consume()
    {
        if (p >= n)
        {
            if (LA(1) == IntStream.EOF)

                throw new IllegalStateException("cannot consume EOF");
        }

        //Console.Out.WriteLine("prev p="+p+", c="+(char)data[p]);
        if (p < n)
        {
            p++;
            //Console.Out.WriteLine("p moves to "+p+" (c='"+(char)data[p]+"')");
        }
    }

    
    public virtual int LA(int i)
    {
        if (i == 0)
        {
            return 0; // undefined
        }
        if (i < 0)
        {
            i++; // e.g., translate LA(-1) to use offset i=0; then data[p+0-1]
            if ((p + i - 1) < 0)
            {
                return IntStream.EOF; // invalid; no char before first char
            }
        }

        if ((p + i - 1) >= n)
        {
            //Console.Out.WriteLine("char LA("+i+")=EOF; p="+p);
            return IntStream.EOF;
        }
        //Console.Out.WriteLine("char LA("+i+")="+(char)data[p+i-1]+"; p="+p);
        //Console.Out.WriteLine("LA("+i+"); p="+p+" n="+n+" data.length="+data.length);
        return data[p + i - 1];
    }

    public int LT(int i)
    {
        return LA(i);
    }

    /** Return the current input symbol index 0..n where n indicates the
     *  last symbol has been read.  The index is the index of char to
	 *  be returned from LA(1).
     */
    
    public int Index => p;

    
    public int Count => n;

    /** mark/release do nothing; we have entire buffer */
    
    public int Mark()
    {
        return -1;
    }

    
    public void Release(int marker)
    {
    }

    /** consume() ahead until p==index; can't just set p=index as we must
	 *  update line and charPositionInLine. If we seek backwards, just set p
	 */
    
    public void Seek(int index)
    {
        if (index <= p)
        {
            p = index; // just jump; don't update stream state (line, ...)
            return;
        }
        // seek forward, consume until p hits index or n (whichever comes first)
        index = Math.Min(index, n);
        while (p < index)
        {
            Consume();
        }
    }

    
    public string GetText(Interval interval)
    {
        int start = interval.a;
        int stop = interval.b;
        if (stop >= n) stop = n - 1;
        int count = stop - start + 1;
        if (start >= n) return "";
        //		Console.Error.WriteLine("data: "+Arrays.toString(data)+", n="+n+
        //						   ", start="+start+
        //						   ", stop="+stop);
        return new string(data, start, count);
    }

    public virtual string SourceName => string.IsNullOrEmpty(name) ? IntStream.UNKNOWN_SOURCE_NAME : name;

    public override string ToString() => new string(data);

    public int CharPositionInLine => throw new NotImplementedException();

    public int Line => throw new NotImplementedException();

    public void Rewind(int start)
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
