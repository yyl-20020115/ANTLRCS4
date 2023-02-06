/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */
using org.antlr.v4.runtime.atn;
using org.antlr.v4.runtime.misc;

namespace org.antlr.v4.runtime;


public class CommonToken : WritableToken /*Serializable*/
{
    /**
	 * An empty {@link Pair} which is used as the default value of
	 * {@link #source} for tokens that do not have a source.
	 */
    protected static readonly Pair<TokenSource, CharStream> EMPTY_SOURCE =
        new (default, default);

    /**
	 * This is the backing field for {@link #getType} and {@link #setType}.
	 */
    protected int type;

    /**
	 * This is the backing field for {@link #getLine} and {@link #setLine}.
	 */
    public int line;

    /**
	 * This is the backing field for {@link #getCharPositionInLine} and
	 * {@link #setCharPositionInLine}.
	 */
    public int charPositionInLine = -1; // set to invalid position

    /**
	 * This is the backing field for {@link #getChannel} and
	 * {@link #setChannel}.
	 */
    public int channel = Token.DEFAULT_CHANNEL;

    /**
	 * This is the backing field for {@link #getTokenSource} and
	 * {@link #getInputStream}.
	 *
	 * <p>
	 * These properties share a field to reduce the memory footprint of
	 * {@link CommonToken}. Tokens created by a {@link CommonTokenFactory} from
	 * the same source and input stream share a reference to the same
	 * {@link Pair} containing these values.</p>
	 */

    protected Pair<TokenSource, CharStream> source;

    /**
	 * This is the backing field for {@link #getText} when the token text is
	 * explicitly set in the constructor or via {@link #setText}.
	 *
	 * @see #getText()
	 */
    protected string text;

    /**
	 * This is the backing field for {@link #getTokenIndex} and
	 * {@link #setTokenIndex}.
	 */
    protected int index = -1;

    /**
	 * This is the backing field for {@link #getStartIndex} and
	 * {@link #setStartIndex}.
	 */
    protected int start;

    /**
	 * This is the backing field for {@link #getStopIndex} and
	 * {@link #setStopIndex}.
	 */
    protected int stop;

    /**
	 * Constructs a new {@link CommonToken} with the specified token type.
	 *
	 * @param type The token type.
	 */
    public CommonToken(int type)
    {
        this.type = type;
        this.source = EMPTY_SOURCE;
    }
    public CommonToken(CharStream input, int type, int channel, int start, int stop)
        :this(new Pair<TokenSource, CharStream>(null,input), type, channel, start, stop)
    {
    }

    public CommonToken(Pair<TokenSource, CharStream> source, int type, int channel, int start, int stop)
    {
        this.source = source;
        this.type = type;
        this.channel = channel;
        this.start = start;
        this.stop = stop;
        if (source.a != null)
        {
            this.line = source.a.Line;
            this.charPositionInLine = source.a.CharPositionInLine;
        }
    }

    /**
	 * Constructs a new {@link CommonToken} with the specified token type and
	 * text.
	 *
	 * @param type The token type.
	 * @param text The text of the token.
	 */
    public CommonToken(int type, string text)
    {
        this.type = type;
        this.channel = Token.DEFAULT_CHANNEL;
        this.text = text;
        this.source = EMPTY_SOURCE;
    }

    /**
	 * Constructs a new {@link CommonToken} as a copy of another {@link Token}.
	 *
	 * <p>
	 * If {@code oldToken} is also a {@link CommonToken} instance, the newly
	 * constructed token will share a reference to the {@link #text} field and
	 * the {@link Pair} stored in {@link #source}. Otherwise, {@link #text} will
	 * be assigned the result of calling {@link #getText}, and {@link #source}
	 * will be constructed from the result of {@link Token#getTokenSource} and
	 * {@link Token#getInputStream}.</p>
	 *
	 * @param oldToken The token to copy.
	 */
    public CommonToken(Token oldToken)
    {
        type = oldToken.getType();
        line = oldToken.getLine();
        index = oldToken.getTokenIndex();
        charPositionInLine = oldToken.getCharPositionInLine();
        channel = oldToken.getChannel();
        start = oldToken.getStartIndex();
        stop = oldToken.getStopIndex();

        if (oldToken is CommonToken token)
        {
            text = token.text;
            source = token.source;
        }
        else
        {
            text = oldToken.getText();
            source = new (oldToken.getTokenSource(), oldToken.getInputStream());
        }
    }

    //@Override
    public virtual int getType()
    {
        return type;
    }

    //@Override
    public virtual void setLine(int line)
    {
        this.line = line;
    }

    //@Override
    public virtual string getText()
    {
        if (text != null)
        {
            return text;
        }

        var input = getInputStream();
        if (input == null) return null;
        int n = input.Count;
        if (start < n && stop < n)
        {
            return input.GetText(Interval.Of(start, stop));
        }
        else
        {
            return "<EOF>";
        }
    }

    /**
	 * Explicitly set the text for this token. If {code text} is not
	 * {@code null}, then {@link #getText} will return this value rather than
	 * extracting the text from the input.
	 *
	 * @param text The explicit text of the token, or {@code null} if the text
	 * should be obtained from the input along with the start and stop indexes
	 * of the token.
	 */
    //@Override
    public virtual void setText(String text)
    {
        this.text = text;
    }

    //@Override
    public virtual int getLine()
    {
        return line;
    }

    //@Override
    public virtual int getCharPositionInLine()
    {
        return charPositionInLine;
    }

    //@Override
    public virtual void setCharPositionInLine(int charPositionInLine)
    {
        this.charPositionInLine = charPositionInLine;
    }

    //@Override
    public virtual int getChannel()
    {
        return channel;
    }

    //@Override
    public virtual void setChannel(int channel)
    {
        this.channel = channel;
    }

    //@Override
    public virtual void setType(int type)
    {
        this.type = type;
    }

    //@Override
    public virtual int getStartIndex()
    {
        return start;
    }

    public virtual void setStartIndex(int start)
    {
        this.start = start;
    }

    //@Override
    public virtual int getStopIndex()
    {
        return stop;
    }

    public virtual void setStopIndex(int stop)
    {
        this.stop = stop;
    }

    //@Override
    public virtual int getTokenIndex()
    {
        return index;
    }

    //@Override
    public virtual void setTokenIndex(int index)
    {
        this.index = index;
    }

    //@Override
    public virtual TokenSource getTokenSource()
    {
        return source.a;
    }

    //@Override
    public virtual CharStream getInputStream()
    {
        return source.b;
    }

    //@Override
    public override string ToString()
    {
        return ToString(null);
    }

    public string ToString(Recognizer<Token, ATNSimulator> r)
    {
        string channelStr = "";
        if (channel > 0)
        {
            channelStr = ",channel=" + channel;
        }
        var txt = getText();
        if (txt != null)
        {
            txt = txt.Replace("\n", "\\n");
            txt = txt.Replace("\r", "\\r");
            txt = txt.Replace("\t", "\\t");
        }
        else
        {
            txt = "<no text>";
        }
        var typeString = type.ToString();// String.valueOf(type);
        if (r != null)
        {
            typeString = r.getVocabulary().getDisplayName(type);
        }
        return "[@" + getTokenIndex() + "," + start + ":" + stop + "='" + txt + "',<" + typeString + ">" + channelStr + "," + line + ":" + getCharPositionInLine() + "]";
    }

    public virtual void setTokenSource(TokenSource tokenSource)
    {
        throw new NotImplementedException();
    }

    public virtual void setInputStream(CharStream input)
    {
        throw new NotImplementedException();
    }
}
