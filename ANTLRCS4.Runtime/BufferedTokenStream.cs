/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime.dfa;
using org.antlr.v4.runtime.misc;
using System.Text;

namespace org.antlr.v4.runtime;


/**
 * This implementation of {@link TokenStream} loads tokens from a
 * {@link TokenSource} on-demand, and places the tokens in a buffer to provide
 * access to any previous token by index.
 *
 * <p>
 * This token stream ignores the value of {@link Token#getChannel}. If your
 * parser requires the token stream filter tokens to only those on a particular
 * channel, such as {@link Token#DEFAULT_CHANNEL} or
 * {@link Token#HIDDEN_CHANNEL}, use a filtering token stream such a
 * {@link CommonTokenStream}.</p>
 */
public class BufferedTokenStream : TokenStream
{
    /**
	 * The {@link TokenSource} from which tokens for this stream are fetched.
	 */
    protected TokenSource tokenSource;

    /**
	 * A collection of all tokens fetched from the token source. The list is
	 * considered a complete view of the input once {@link #fetchedEOF} is set
	 * to {@code true}.
	 */
    protected List<Token> tokens = new(100);

    /**
	 * The index into {@link #tokens} of the current token (next token to
	 * {@link #consume}). {@link #tokens}{@code [}{@link #p}{@code ]} should be
	 * {@link #LT LT(1)}.
	 *
	 * <p>This field is set to -1 when the stream is first constructed or when
	 * {@link #setTokenSource} is called, indicating that the first token has
	 * not yet been fetched from the token source. For additional information,
	 * see the documentation of {@link IntStream} for a description of
	 * Initializing Methods.</p>
	 */
    protected int p = -1;

    /**
	 * Indicates whether the {@link Token#EOF} token has been fetched from
	 * {@link #tokenSource} and added to {@link #tokens}. This field improves
	 * performance for the following cases:
	 *
	 * <ul>
	 * <li>{@link #consume}: The lookahead check in {@link #consume} to prevent
	 * consuming the EOF symbol is optimized by checking the values of
	 * {@link #fetchedEOF} and {@link #p} instead of calling {@link #LA}.</li>
	 * <li>{@link #fetch}: The check to prevent adding multiple EOF symbols into
	 * {@link #tokens} is trivial with this field.</li>
	 * <ul>
	 */
    protected bool fetchedEOF;

    public BufferedTokenStream(TokenSource tokenSource)
    {
        this.tokenSource = tokenSource ?? throw new NullReferenceException("tokenSource cannot be null");
    }

    
    /** Reset this token stream by setting its token source. */
    public TokenSource TokenSource
    {
        get => tokenSource;
        set
        {
            this.tokenSource = value;
            tokens.Clear();
            p = -1;
            fetchedEOF = false;
        }
    }

    
    public int Index => p;
    
    public int Mark() => 0;

    
    public void Release(int marker)
    {
        // no resources to release
    }

    /**
	 * This method resets the token stream back to the first token in the
	 * buffer. It is equivalent to calling {@link #seek}{@code (0)}.
	 *
	 * @see #setTokenSource(TokenSource)
	 * @deprecated Use {@code seek(0)} instead.
	 */
    //@Deprecated
    public void Reset()
    {
        Seek(0);
    }

    
    public void Seek(int index)
    {
        LazyInit();
        p = AdjustSeekIndex(index);
    }

    
    public int Count => tokens.Count;
    
    public void Consume()
    {
        bool skipEofCheck;
        if (p >= 0)
        {
            if (fetchedEOF)
            {
                // the last token in tokens is EOF. skip check if p indexes any
                // fetched token except the last.
                skipEofCheck = p < tokens.Count - 1;
            }
            else
            {
                // no EOF token in tokens. skip check if p indexes a fetched token.
                skipEofCheck = p < tokens.Count;
            }
        }
        else
        {
            // not yet initialized
            skipEofCheck = false;
        }

        if (!skipEofCheck && LA(1) == Token.EOF)
        {
            throw new IllegalStateException("cannot consume EOF");
        }

        if (Sync(p + 1))
        {
            p = AdjustSeekIndex(p + 1);
        }
    }

    /** Make sure index {@code i} in tokens has a token.
	 *
	 * @return {@code true} if a token is located at index {@code i}, otherwise
	 *    {@code false}.
	 * @see #get(int i)
	 */
    protected bool Sync(int i)
    {
        //assert i >= 0;
        int n = i - tokens.Count + 1; // how many more elements we need?
        //Console.Out.WriteLine("sync("+i+") needs "+n);
        if (n > 0)
        {
            int fetched = Fetch(n);
            return fetched >= n;
        }

        return true;
    }

    /** Add {@code n} elements to buffer.
	 *
	 * @return The actual number of elements added to the buffer.
	 */
    protected int Fetch(int n)
    {
        if (fetchedEOF)
        {
            return 0;
        }

        for (int i = 0; i < n; i++)
        {
            var t = tokenSource.NextToken();
            if (t is WritableToken token)
            {
                token.TokenIndex = tokens.Count;
            }
            tokens.Add(t);
            if (t.Type == Token.EOF)
            {
                fetchedEOF = true;
                return i + 1;
            }
        }

        return n;
    }

    
    public Token Get(int index)
    {
        if (index < 0 || index >= tokens.Count)
        {
            throw new IndexOutOfRangeException("token index " + index + " out of range 0.." + (tokens.Count - 1));
        }
        return tokens[index];
    }

    /** Get all tokens from start..stop inclusively */
    public List<Token> Get(int start, int stop)
    {
        if (start < 0 || stop < 0) return null;
        LazyInit();
        List<Token> subset = new();
        if (stop >= tokens.Count) stop = tokens.Count - 1;
        for (int i = start; i <= stop; i++)
        {
            Token t = tokens[i];
            if (t.Type == Token.EOF) break;
            subset.Add(t);
        }
        return subset;
    }

    
    public virtual int LA(int i) { return LT(i).Type; }

    protected virtual Token LB(int k)
    {
        if ((p - k) < 0) return null;
        return tokens[(p - k)];
    }


    public virtual Token LT(int k)
    {
        LazyInit();
        if (k == 0) return null;
        if (k < 0) return LB(-k);

        int i = p + k - 1;
        Sync(i);
        if (i >= tokens.Count)
        { // return EOF token
            // EOF must be last token
            return tokens[(tokens.Count - 1)];
        }
        //		if ( i>range ) range = i;
        return tokens[i];
    }

    /**
	 * Allowed derived classes to modify the behavior of operations which change
	 * the current stream position by adjusting the target token index of a seek
	 * operation. The default implementation simply returns {@code i}. If an
	 * exception is thrown in this method, the current stream index should not be
	 * changed.
	 *
	 * <p>For example, {@link CommonTokenStream} overrides this method to ensure that
	 * the seek target is always an on-channel token.</p>
	 *
	 * @param i The target token index.
	 * @return The adjusted target token index.
	 */
    protected virtual int AdjustSeekIndex(int i)
    {
        return i;
    }

    protected void LazyInit()
    {
        if (p == -1) Setup();
    }

    protected void Setup()
    {
        Sync(0);
        p = AdjustSeekIndex(0);
    }

    public List<Token> GetTokens() { return tokens; }

    public List<Token> GetTokens(int start, int stop)
    {
        return GetTokens(start, stop, null);
    }

    /** Given a start and stop index, return a List of all tokens in
     *  the token type BitSet.  Return null if no tokens were found.  This
     *  method looks at both on and off channel tokens.
     */
    public List<Token> GetTokens(int start, int stop, HashSet<int> types)
    {
        LazyInit();
        if (start < 0 || stop >= tokens.Count ||
             stop < 0 || start >= tokens.Count)
        {
            throw new IndexOutOfRangeException("start " + start + " or stop " + stop +
                                                " not in 0.." + (tokens.Count - 1));
        }
        if (start > stop) return null;

        // list = tokens[start:stop]:{T t, t.getType() in types}
        List<Token> filteredTokens = new();
        for (int i = start; i <= stop; i++)
        {
            var t = tokens[i];
            if (types == null || types.Contains(t.Type))
            {
                filteredTokens.Add(t);
            }
        }
        if (filteredTokens.Count == 0)
        {
            filteredTokens = null;
        }
        return filteredTokens;
    }

    public List<Token> GetTokens(int start, int stop, int ttype)
    {
        var s = new HashSet<int>(ttype);
        s.Add(ttype);
        return GetTokens(start, stop, s);
    }

    /**
	 * Given a starting index, return the index of the next token on channel.
	 * Return {@code i} if {@code tokens[i]} is on channel. Return the index of
	 * the EOF token if there are no tokens on channel between {@code i} and
	 * EOF.
	 */
    protected int NextTokenOnChannel(int i, int channel)
    {
        Sync(i);
        if (i >= Count)
        {
            return Count - 1;
        }

        var token = tokens[i];
        while (token.Channel != channel)
        {
            if (token.Type == Token.EOF)
            {
                return i;
            }

            i++;
            Sync(i);
            token = tokens[i];
        }

        return i;
    }

    /**
	 * Given a starting index, return the index of the previous token on
	 * channel. Return {@code i} if {@code tokens[i]} is on channel. Return -1
	 * if there are no tokens on channel between {@code i} and 0.
	 *
	 * <p>
	 * If {@code i} specifies an index at or after the EOF token, the EOF token
	 * index is returned. This is due to the fact that the EOF token is treated
	 * as though it were on every channel.</p>
	 */
    protected int PreviousTokenOnChannel(int i, int channel)
    {
        Sync(i);
        if (i >= Count)
        {
            // the EOF token is on every channel
            return Count - 1;
        }

        while (i >= 0)
        {
            var token = tokens[i];
            if (token.Type == Token.EOF || token.Channel == channel)
            {
                return i;
            }

            i--;
        }

        return i;
    }

    /** Collect all tokens on specified channel to the right of
	 *  the current token up until we see a token on DEFAULT_TOKEN_CHANNEL or
	 *  EOF. If channel is -1, find any non default channel token.
	 */
    public List<Token> GetHiddenTokensToRight(int tokenIndex, int channel)
    {
        LazyInit();
        if (tokenIndex < 0 || tokenIndex >= tokens.Count)
        {
            throw new IndexOutOfRangeException(tokenIndex + " not in 0.." + (tokens.Count - 1));
        }

        int nextOnChannel =
            NextTokenOnChannel(tokenIndex + 1, Lexer.DEFAULT_TOKEN_CHANNEL);
        int to;
        int from = tokenIndex + 1;
        // if none onchannel to right, nextOnChannel=-1 so set to = last token
        if (nextOnChannel == -1) to = Count - 1;
        else to = nextOnChannel;

        return FilterForChannel(from, to, channel);
    }

    /** Collect all hidden tokens (any off-default channel) to the right of
	 *  the current token up until we see a token on DEFAULT_TOKEN_CHANNEL
	 *  or EOF.
	 */
    public List<Token> GetHiddenTokensToRight(int tokenIndex)
    {
        return GetHiddenTokensToRight(tokenIndex, -1);
    }

    /** Collect all tokens on specified channel to the left of
	 *  the current token up until we see a token on DEFAULT_TOKEN_CHANNEL.
	 *  If channel is -1, find any non default channel token.
	 */
    public List<Token> GetHiddenTokensToLeft(int tokenIndex, int channel)
    {
        LazyInit();
        if (tokenIndex < 0 || tokenIndex >= tokens.Count)
        {
            throw new IndexOutOfRangeException(tokenIndex + " not in 0.." + (tokens.Count - 1));
        }

        if (tokenIndex == 0)
        {
            // obviously no tokens can appear before the first token
            return null;
        }

        int prevOnChannel =
            PreviousTokenOnChannel(tokenIndex - 1, Lexer.DEFAULT_TOKEN_CHANNEL);
        if (prevOnChannel == tokenIndex - 1) return null;
        // if none onchannel to left, prevOnChannel=-1 then from=0
        int from = prevOnChannel + 1;
        int to = tokenIndex - 1;

        return FilterForChannel(from, to, channel);
    }

    /** Collect all hidden tokens (any off-default channel) to the left of
	 *  the current token up until we see a token on DEFAULT_TOKEN_CHANNEL.
	 */
    public List<Token> GetHiddenTokensToLeft(int tokenIndex)
    {
        return GetHiddenTokensToLeft(tokenIndex, -1);
    }

    protected List<Token> FilterForChannel(int from, int to, int channel)
    {
        List<Token> hidden = new();
        for (int i = from; i <= to; i++)
        {
            Token t = tokens[i];
            if (channel == -1)
            {
                if (t.Channel != Lexer.DEFAULT_TOKEN_CHANNEL) hidden.Add(t);
            }
            else
            {
                if (t.Channel == channel) hidden.Add(t);
            }
        }
        if (hidden.Count == 0) return null;
        return hidden;
    }

    
    public string SourceName => tokenSource.SourceName;
    /** Get the text of all tokens in this buffer. */

    
    public string Text => GetText(Interval.Of(0, Count - 1));

    
    public string GetText(Interval interval)
    {
        int start = interval.a;
        int stop = interval.b;
        if (start < 0 || stop < 0) return "";
        Sync(stop);
        if (stop >= tokens.Count) stop = tokens.Count - 1;

        var buffer = new StringBuilder();
        for (int i = start; i <= stop; i++)
        {
            var t = tokens[i];
            if (t.Type == Token.EOF) break;
            buffer.Append(t.Text);
        }
        return buffer.ToString();
    }


    
    public string GetText(RuleContext ctx)
    {
        return GetText(ctx.SourceInterval);
    }


    
    public string GetText(Token start, Token stop)
    {
        return start != null && stop != null ? GetText(Interval.Of(start.TokenIndex, stop.TokenIndex)) : "";
    }

    /** Get all tokens from lexer until EOF */
    public void Fill()
    {
        LazyInit();
        int blockSize = 1000;
        while (true)
        {
            int fetched = Fetch(blockSize);
            if (fetched < blockSize)
            {
                return;
            }
        }
    }

    public int Range()
    {
        throw new NotImplementedException();
    }

    public string ToString(int start, int stop)
    {
        throw new NotImplementedException();
    }

    public string ToString(Token start, Token stop)
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
