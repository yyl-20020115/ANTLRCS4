/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime.misc;

namespace org.antlr.v4.runtime;

/**
 * This default implementation of {@link TokenFactory} creates
 * {@link CommonToken} objects.
 */
public class CommonTokenFactory : TokenFactory<CommonToken>
{
    /**
	 * The default {@link CommonTokenFactory} instance.
	 *
	 * <p>
	 * This token factory does not explicitly copy token text when constructing
	 * tokens.</p>
	 */
    public static readonly TokenFactory<CommonToken> DEFAULT = new CommonTokenFactory();

    /**
	 * Indicates whether {@link CommonToken#setText} should be called after
	 * constructing tokens to explicitly set the text. This is useful for cases
	 * where the input stream might not be able to provide arbitrary substrings
	 * of text from the input after the lexer creates a token (e.g. the
	 * implementation of {@link CharStream#getText} in
	 * {@link UnbufferedCharStream} throws an
	 * {@link UnsupportedOperationException}). Explicitly setting the token text
	 * allows {@link Token#getText} to be called at any time regardless of the
	 * input stream implementation.
	 *
	 * <p>
	 * The default value is {@code false} to avoid the performance and memory
	 * overhead of copying text for every token unless explicitly requested.</p>
	 */
    protected readonly bool copyText;

    /**
	 * Constructs a {@link CommonTokenFactory} with the specified value for
	 * {@link #copyText}.
	 *
	 * <p>
	 * When {@code copyText} is {@code false}, the {@link #DEFAULT} instance
	 * should be used instead of constructing a new instance.</p>
	 *
	 * @param copyText The value for {@link #copyText}.
	 */
    public CommonTokenFactory(bool copyText) { this.copyText = copyText; }

    /**
	 * Constructs a {@link CommonTokenFactory} with {@link #copyText} set to
	 * {@code false}.
	 *
	 * <p>
	 * The {@link #DEFAULT} instance should be used instead of calling this
	 * directly.</p>
	 */
    public CommonTokenFactory() : this(false) { }

    //@Override
    public CommonToken Create(Pair<TokenSource, CharStream> source, int type, string text,
                              int channel, int start, int stop,
                              int line, int charPositionInLine)
    {
        var t = new CommonToken(source, type, channel, start, stop)
        {
            Line = line,
            CharPositionInLine = charPositionInLine
        };
        if (text != null)
        {
            t.Text = text;
        }
        else if (copyText && source.b != null)
        {
            t.Text = source.b.GetText(Interval.Of(start, stop));
        }

        return t;
    }

    public virtual CommonToken Create(int type, string text)
    {
        return new CommonToken(type, text);
    }
}
