/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime.misc;

namespace org.antlr.v4.runtime;

/**
 * Provides an implementation of {@link TokenSource} as a wrapper around a list
 * of {@link Token} objects.
 *
 * <p>If the final token in the list is an {@link Token#EOF} token, it will be used
 * as the EOF token for every call to {@link #nextToken} after the end of the
 * list is reached. Otherwise, an EOF token will be created.</p>
 */
public class ListTokenSource : TokenSource
{
    /**
	 * The wrapped collection of {@link Token} objects to return.
	 */
    protected readonly List<Token> tokens;

    /**
	 * The name of the input source. If this value is {@code null}, a call to
	 * {@link #getSourceName} should return the source name used to create the
	 * the next token in {@link #tokens} (or the previous token if the end of
	 * the input has been reached).
	 */
    private readonly string sourceName;

    /**
	 * The index into {@link #tokens} of token to return by the next call to
	 * {@link #nextToken}. The end of the input is indicated by this value
	 * being greater than or equal to the number of items in {@link #tokens}.
	 */
    protected int i;

    /**
	 * This field caches the EOF token for the token source.
	 */
    protected Token eofToken;

    /**
	 * This is the backing field for {@link #getTokenFactory} and
	 * {@link setTokenFactory}.
	 */
    private TokenFactory _factory = CommonTokenFactory.DEFAULT;

    /**
	 * Constructs a new {@link ListTokenSource} instance from the specified
	 * collection of {@link Token} objects.
	 *
	 * @param tokens The collection of {@link Token} objects to provide as a
	 * {@link TokenSource}.
	 * @exception NullReferenceException if {@code tokens} is {@code null}
	 */
    public ListTokenSource(List<Token> tokens) : this(tokens, null)
    {
    }

    /**
	 * Constructs a new {@link ListTokenSource} instance from the specified
	 * collection of {@link Token} objects and source name.
	 *
	 * @param tokens The collection of {@link Token} objects to provide as a
	 * {@link TokenSource}.
	 * @param sourceName The name of the {@link TokenSource}. If this value is
	 * {@code null}, {@link #getSourceName} will attempt to infer the name from
	 * the next {@link Token} (or the previous token if the end of the input has
	 * been reached).
	 *
	 * @exception NullReferenceException if {@code tokens} is {@code null}
	 */
    public ListTokenSource(List<Token> tokens, string sourceName)
    {
        this.tokens = tokens ?? throw new NullReferenceException("tokens cannot be null");
        this.sourceName = sourceName;
    }

    /**
	 * {@inheritDoc}
	 */
    
    public virtual int CharPositionInLine
    {
        get
        {
            if (i < tokens.Count)
            {
                return tokens[(i)].CharPositionInLine;
            }
            else if (eofToken != null)
            {
                return eofToken.CharPositionInLine;
            }
            else if (tokens.Count > 0)
            {
                // have to calculate the result from the line/column of the previous
                // token, along with the text of the token.
                var lastToken = tokens[(tokens.Count - 1)];
                var tokenText = lastToken.Text;
                if (tokenText != null)
                {
                    int lastNewLine = tokenText.LastIndexOf('\n');
                    if (lastNewLine >= 0)
                    {
                        return tokenText.Length - lastNewLine - 1;
                    }
                }

                return lastToken.CharPositionInLine + lastToken.StopIndex - lastToken.StartIndex + 1;
            }

            // only reach this if tokens is empty, meaning EOF occurs at the first
            // position in the input
            return 0;
        }
    }

    /**
	 * {@inheritDoc}
	 */
    
    public virtual Token NextToken()
    {
        if (i >= tokens.Count)
        {
            if (eofToken == null)
            {
                int start = -1;
                if (tokens.Count > 0)
                {
                    int previousStop = tokens[(tokens.Count - 1)].StopIndex;
                    if (previousStop != -1)
                    {
                        start = previousStop + 1;
                    }
                }

                int stop = Math.Max(-1, start - 1);
                eofToken = (_factory as TokenFactory<Token>).Create(new Pair<TokenSource, CharStream>(this, CharInputStream), Token.EOF, "EOF", Token.DEFAULT_CHANNEL, start, stop, Line, CharPositionInLine);
            }

            return eofToken;
        }

        Token t = tokens[(i)];
        if (i == tokens.Count - 1 && t.Type == Token.EOF)
        {
            eofToken = t;
        }

        i++;
        return t;
    }

    /**
	 * {@inheritDoc}
	 */
    
    public virtual int Line
    {
        get
        {
            if (i < tokens.Count)
            {
                return tokens[(i)].Line;
            }
            else if (eofToken != null)
            {
                return eofToken.Line;
            }
            else if (tokens.Count > 0)
            {
                // have to calculate the result from the line/column of the previous
                // token, along with the text of the token.
                var lastToken = tokens[(tokens.Count - 1)];
                int line = lastToken.Line;

                var tokenText = lastToken.Text;
                if (tokenText != null)
                {
                    for (int i = 0; i < tokenText.Length; i++)
                    {
                        if (tokenText[(i)] == '\n')
                        {
                            line++;
                        }
                    }
                }

                // if no text is available, assume the token did not contain any newline characters.
                return line;
            }

            // only reach this if tokens is empty, meaning EOF occurs at the first
            // position in the input
            return 1;
        }
    }

    /**
	 * {@inheritDoc}
	 */
    
    public virtual CharStream CharInputStream
    {
        get
        {
            if (i < tokens.Count)
            {
                return tokens[(i)].InputStream;
            }
            else if (eofToken != null)
            {
                return eofToken.InputStream;
            }
            else if (tokens.Count > 0)
            {
                return tokens[(tokens.Count - 1)].InputStream;
            }

            // no input stream information is available
            return null;
        }
    }

    /**
	 * {@inheritDoc}
	 */
    
    public virtual string SourceName
    {
        get
        {
            if (sourceName != null)
            {
                return sourceName;
            }

            CharStream inputStream = CharInputStream;
            if (inputStream != null)
            {
                return inputStream.SourceName;
            }

            return "List";
        }
    }

    /**
	 * {@inheritDoc}
	 */
    
    /**
 * {@inheritDoc}
 */
    
    public virtual TokenFactory TokenFactory { get => _factory; set => this._factory = value; }
}
