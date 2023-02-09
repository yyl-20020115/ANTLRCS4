/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime.dfa;
using org.antlr.v4.runtime.misc;
using System.Text;

namespace org.antlr.v4.runtime;


public class UnbufferedTokenStream : TokenStream
{
    protected TokenSource tokenSource;

    /**
	 * A moving window buffer of the data being scanned. While there's a marker,
	 * we keep adding to buffer. Otherwise, {@link #consume consume()} resets so
	 * we start filling at index 0 again.
	 */
    protected Token[] tokens;

    /**
	 * The number of tokens currently in {@link #tokens tokens}.
	 *
	 * <p>This is not the buffer capacity, that's {@code tokens.length}.</p>
	 */
    protected int n;

    /**
	 * 0..n-1 index into {@link #tokens tokens} of next token.
	 *
	 * <p>The {@code LT(1)} token is {@code tokens[p]}. If {@code p == n}, we are
	 * out of buffered tokens.</p>
	 */
    protected int p = 0;

    /**
	 * Count up with {@link #mark mark()} and down with
	 * {@link #release release()}. When we {@code release()} the last mark,
	 * {@code numMarkers} reaches 0 and we reset the buffer. Copy
	 * {@code tokens[p]..tokens[n-1]} to {@code tokens[0]..tokens[(n-1)-p]}.
	 */
    protected int numMarkers = 0;

    /**
	 * This is the {@code LT(-1)} token for the current position.
	 */
    protected Token lastToken;

    /**
	 * When {@code numMarkers > 0}, this is the {@code LT(-1)} token for the
	 * first token in {@link #tokens}. Otherwise, this is {@code null}.
	 */
    protected Token lastTokenBufferStart;

    /**
	 * Absolute token index. It's the index of the token about to be read via
	 * {@code LT(1)}. Goes from 0 to the number of tokens in the entire stream,
	 * although the stream size is unknown before the end is reached.
	 *
	 * <p>This value is used to set the token indexes if the stream provides tokens
	 * that implement {@link WritableToken}.</p>
	 */
    protected int currentTokenIndex = 0;

    public UnbufferedTokenStream(TokenSource tokenSource) : this(tokenSource, 256)
    {
    }

    public UnbufferedTokenStream(TokenSource tokenSource, int bufferSize)
    {
        this.tokenSource = tokenSource;
        tokens = new Token[bufferSize];
        n = 0;
        Fill(1); // prime the pump
    }

    //@Override
    public virtual Token Get(int i)
    { // get absolute index
        int bufferStartIndex = GetBufferStartIndex();
        if (i < bufferStartIndex || i >= bufferStartIndex + n)
        {
            throw new IndexOutOfRangeException("get(" + i + ") outside buffer: " +
                                bufferStartIndex + ".." + (bufferStartIndex + n));
        }
        return tokens[i - bufferStartIndex];
    }

    //@Override
    public virtual Token LT(int i)
    {
        if (i == -1)
        {
            return lastToken;
        }

        Sync(i);
        int index = p + i - 1;
        if (index < 0)
        {
            throw new IndexOutOfRangeException("LT(" + i + ") gives negative index");
        }

        if (index >= n)
        {
            //assert n > 0 && tokens[n-1].getType() == Token.EOF;
            return tokens[n - 1];
        }

        return tokens[index];
    }

    public virtual int LA(int i)
    {
        return LT(i).Type;
    }

    public virtual TokenSource TokenSource => tokenSource;


    public virtual string Text => "";


    public virtual string GetText(RuleContext ctx) => GetText(ctx.SourceInterval);


    public string GetText(Token start, Token stop) => GetText(Interval.Of(start.TokenIndex, stop.TokenIndex));

    public virtual void Consume()
    {
        if (LA(1) == Token.EOF)
        {
            throw new IllegalStateException("cannot consume EOF");
        }

        // buf always has at least tokens[p==0] in this method due to ctor
        lastToken = tokens[p];   // track last token for LT(-1)

        // if we're at last token and no markers, opportunity to flush buffer
        if (p == n - 1 && numMarkers == 0)
        {
            n = 0;
            p = -1; // p++ will leave this at 0
            lastTokenBufferStart = lastToken;
        }

        p++;
        currentTokenIndex++;
        Sync(1);
    }

    /** Make sure we have 'need' elements from current position {@link #p p}. Last valid
	 *  {@code p} index is {@code tokens.length-1}.  {@code p+need-1} is the tokens index 'need' elements
	 *  ahead.  If we need 1 element, {@code (p+1-1)==p} must be less than {@code tokens.length}.
	 */
    public virtual void Sync(int want)
    {
        int need = (p + want - 1) - n + 1; // how many more elements we need?
        if (need > 0)
        {
            Fill(need);
        }
    }

    /**
	 * Add {@code n} elements to the buffer. Returns the number of tokens
	 * actually added to the buffer. If the return value is less than {@code n},
	 * then EOF was reached before {@code n} tokens could be added.
	 */
    public virtual int Fill(int n)
    {
        for (int i = 0; i < n; i++)
        {
            if (this.n > 0 && tokens[this.n - 1].Type == Token.EOF)
            {
                return i;
            }

            Token t = tokenSource.NextToken();
            Add(t);
        }

        return n;
    }

    public virtual void Add(Token t)
    {
        if (n >= tokens.Length)
        {
            tokens = Arrays.CopyOf(tokens, tokens.Length * 2);
        }

        if (t is WritableToken token)
        {
            token.TokenIndex = GetBufferStartIndex() + n;
        }

        tokens[n++] = t;
    }

    /**
	 * Return a marker that we can release later.
	 *
	 * <p>The specific marker value used for this class allows for some level of
	 * protection against misuse where {@code seek()} is called on a mark or
	 * {@code release()} is called in the wrong order.</p>
	 */
    //@Override
    public virtual int Mark()
    {
        if (numMarkers == 0)
        {
            lastTokenBufferStart = lastToken;
        }

        int mark = -numMarkers - 1;
        numMarkers++;
        return mark;
    }

    //@Override
    public virtual void Release(int marker)
    {
        int expectedMark = -numMarkers;
        if (marker != expectedMark)
        {
            throw new IllegalStateException("release() called with an invalid marker.");
        }

        numMarkers--;
        if (numMarkers == 0)
        { // can we release buffer?
            if (p > 0)
            {
                // Copy tokens[p]..tokens[n-1] to tokens[0]..tokens[(n-1)-p], reset ptrs
                // p is last valid token; move nothing if p==n as we have no valid char
                Array.Copy(tokens, p, tokens, 0, n - p); // shift n-p tokens from p to 0
                n = n - p;
                p = 0;
            }

            lastTokenBufferStart = lastToken;
        }
    }

    //@Override
    public virtual int Index => currentTokenIndex;

    //@Override
    public virtual void Seek(int index)
    { // seek to absolute index
        if (index == currentTokenIndex)
        {
            return;
        }

        if (index > currentTokenIndex)
        {
            Sync(index - currentTokenIndex);
            index = Math.Min(index, GetBufferStartIndex() + n - 1);
        }

        int bufferStartIndex = GetBufferStartIndex();
        int i = index - bufferStartIndex;
        if (i < 0)
        {
            throw new ArgumentException("cannot seek to negative index " + index);
        }
        else if (i >= n)
        {
            throw new UnsupportedOperationException("seek to index outside buffer: " +
                                                    index + " not in " + bufferStartIndex + ".." + (bufferStartIndex + n));
        }

        p = i;
        currentTokenIndex = index;
        if (p == 0)
        {
            lastToken = lastTokenBufferStart;
        }
        else
        {
            lastToken = tokens[p - 1];
        }
    }

    //@Override
    public virtual int Count => throw new UnsupportedOperationException("Unbuffered stream cannot know its size");

    //@Override
    public virtual string SourceName => tokenSource.SourceName;


    //@Override
    public virtual string GetText(Interval interval)
    {
        int bufferStartIndex = GetBufferStartIndex();
        int bufferStopIndex = bufferStartIndex + tokens.Length - 1;

        int start = interval.a;
        int stop = interval.b;
        if (start < bufferStartIndex || stop > bufferStopIndex)
        {
            throw new UnsupportedOperationException("interval " + interval + " not in token buffer window: " +
                                                    bufferStartIndex + ".." + bufferStopIndex);
        }

        int a = start - bufferStartIndex;
        int b = stop - bufferStartIndex;

        var buffer = new StringBuilder();
        for (int i = a; i <= b; i++)
        {
            var t = tokens[i];
            buffer.Append(t.Text);
        }

        return buffer.ToString();
    }

    protected virtual int GetBufferStartIndex()
    {
        return currentTokenIndex - p;
    }

    public virtual int Range()
    {
        throw new NotImplementedException();
    }

    public virtual string ToString(int start, int stop)
    {
        throw new NotImplementedException();
    }

    public virtual string ToString(Token start, Token stop)
    {
        throw new NotImplementedException();
    }

    public void Rewind(int nvaeMark)
    {
        throw new NotImplementedException();
    }

    public void Rewind()
    {
        throw new NotImplementedException();
    }
}
