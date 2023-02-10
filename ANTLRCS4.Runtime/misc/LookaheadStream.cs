﻿/*
 [The "BSD license"]
 Copyright (c) 2005-2009 Terence Parr
 All rights reserved.

 Redistribution and use in source and binary forms, with or without
 modification, are permitted provided that the following conditions
 are met:
 1. Redistributions of source code must retain the above copyright
     notice, this list of conditions and the following disclaimer.
 2. Redistributions in binary form must reproduce the above copyright
     notice, this list of conditions and the following disclaimer in the
     documentation and/or other materials provided with the distribution.
 3. The name of the author may not be used to endorse or promote products
     derived from this software without specific prior written permission.

 THIS SOFTWARE IS PROVIDED BY THE AUTHOR ``AS IS'' AND ANY EXPRESS OR
 IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
 OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.
 IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY DIRECT, INDIRECT,
 INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT
 NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
 DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
 THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
 THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */
using org.antlr.v4.runtime.dfa;

namespace org.antlr.runtime.misc;

/**
 * A lookahead queue that knows how to mark/release locations in the buffer for
 * backtracking purposes. Any markers force the {@link FastQueue} superclass to
 * keep all elements until no more markers; then can reset to avoid growing a
 * huge buffer.
 */
public abstract class LookaheadStream<T> : FastQueue<T>
{
    public const int UNINITIALIZED_EOF_ELEMENT_INDEX = int.MaxValue;

    /** Absolute token index. It's the index of the symbol about to be
     *  read via {@code LT(1)}. Goes from 0 to numtokens.
     */
    protected int currentElementIndex = 0;

    /**
     * This is the {@code LT(-1)} element for the first element in {@link #data}.
     */
    protected T prevElement;

    /** Track object returned by nextElement upon end of stream;
     *  Return it later when they ask for LT passed end of input.
     */
    public T eof = default;

    /** Track the last mark() call result value for use in rewind(). */
    protected int lastMarker;

    /** tracks how deep mark() calls are nested */
    protected int markDepth = 0;

    public override void Reset()
    {
        base.Reset();
        currentElementIndex = 0;
        p = 0;
        prevElement = default;
    }

    /** Implement nextElement to supply a stream of elements to this
     *  lookahead buffer.  Return EOF upon end of the stream we're pulling from.
     *
     * @see #isEOF
     */
    public abstract T NextElement();

    public abstract bool IsEOF(T o);

    /**
     * Get and remove first element in queue; override
     * {@link FastQueue#remove()}; it's the same, just checks for backtracking.
     */
    
    public override T Remove()
    {
        T o = ElementAt(0);
        p++;
        // have we hit end of buffer and not backtracking?
        if (p == data.Count && markDepth == 0)
        {
            prevElement = o;
            // if so, it's an opportunity to start filling at index 0 again
            Clear(); // size goes to 0, but retains memory
        }
        return o;
    }

    /** Make sure we have at least one element to remove, even if EOF */
    public virtual void Consume()
    {
        SyncAhead(1);
        Remove();
        currentElementIndex++;
    }

    /** Make sure we have 'need' elements from current position p. Last valid
     *  p index is data.size()-1.  p+need-1 is the data index 'need' elements
     *  ahead.  If we need 1 element, (p+1-1)==p must be &lt; data.size().
     */
    protected void SyncAhead(int need)
    {
        int n = (p + need - 1) - data.Count + 1; // how many more elements we need?
        if (n > 0) Fill(n);                 // out of elements?
    }

    /** add n elements to buffer */
    public void Fill(int n)
    {
        for (int i = 1; i <= n; i++)
        {
            T o = NextElement();
            if (IsEOF(o)) eof = o;
            data.Add(o);
        }
    }

    /** Size of entire stream is unknown; we only know buffer size from FastQueue. */
    public override int Count 
        => throw new UnsupportedOperationException("streams are of unknown size");
    public T LT(int k)
    {
        if (k == 0) return default;
        if (k < 0) return LB(-k);
        //System.out.print("LT(p="+p+","+k+")=");
        SyncAhead(k);
        if ((p + k - 1) > data.Count) return eof;
        return ElementAt(k - 1);
    }

    public int Index => currentElementIndex;

    public int Mark()
    {
        markDepth++;
        lastMarker = p; // track where we are in buffer not absolute token index
        return lastMarker;
    }

    public void Release(int marker)
    {
        // no resources to release
    }

    public void Rewind(int marker)
    {
        markDepth--;
        int delta = p - marker;
        currentElementIndex -= delta;
        p = marker;
    }

    public void Rewind()
    {
        // rewind but do not release marker
        int delta = p - lastMarker;
        currentElementIndex -= delta;
        p = lastMarker;
    }

    /**
     * Seek to a 0-indexed absolute token index. Normally used to seek backwards
     * in the buffer. Does not force loading of nodes.
     * <p>
     * To preserve backward compatibility, this method allows seeking past the
     * end of the currently buffered data. In this case, the input pointer will
     * be moved but the data will only actually be loaded upon the next call to
     * {@link #consume} or {@link #LT} for {@code k>0}.</p>
     *
     * @throws IllegalArgumentException if {@code index} is less than 0
     * @throws UnsupportedOperationException if {@code index} lies before the
     * beginning of the moving window buffer
     * ({@code index < }{@link #currentElementIndex currentElementIndex}<code> - </code>{@link #p p}).
     */
    public void Seek(int index)
    {
        if (index < 0)
        {
            throw new ArgumentException("can't seek before the beginning of the input");
        }

        int delta = currentElementIndex - index;
        if (p - delta < 0)
        {
            throw new UnsupportedOperationException("can't seek before the beginning of this stream's buffer");
        }

        p -= delta;
        currentElementIndex = index;
    }

    protected T LB(int k)
    {
        //assert k > 0;

        int index = p - k;
        if (index == -1)
        {
            return prevElement;
        }

        // if k>0 then we know index < data.size(). avoid the double-check for
        // performance.
        if (index >= 0 /*&& index < data.size()*/)
        {
            return data[(index)];
        }

        if (index < -1)
        {
            throw new UnsupportedOperationException("can't look more than one token before the beginning of this stream's buffer");
        }
        
        throw new UnsupportedOperationException("can't look past the end of this stream's buffer using LB(int)");
    }
}
