/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

namespace org.antlr.v4.runtime;

/** A token has properties: text, type, line, character position in the line
 *  (so we can ignore tabs), token channel, index, and source from which
 *  we obtained this token.
 */
public interface Token
{
	public const int INVALID_TYPE = 0;

    /** During lookahead operations, this "token" signifies we hit rule end ATN state
     *  and did not follow it despite needing to.
     */
    public const int EPSILON = -2;

	public const int MIN_USER_TOKEN_TYPE = 1;

    public const int EOF = IntStream.EOF;

	/** All tokens go to the parser (unless skip() is called in that rule)
	 *  on a particular "channel".  The parser tunes to a particular channel
	 *  so that whitespace etc... can go to the parser on a "hidden" channel.
	 */
	public const int DEFAULT_CHANNEL = 0;

	/** Anything on different channel than DEFAULT_CHANNEL is not parsed
	 *  by parser.
	 */
	public const int HIDDEN_CHANNEL = 1;

	/**
	 * This is the minimum constant value which can be assigned to a
	 * user-defined token channel.
	 *
	 * <p>
	 * The non-negative numbers less than {@link #MIN_USER_CHANNEL_VALUE} are
	 * assigned to the predefined channels {@link #DEFAULT_CHANNEL} and
	 * {@link #HIDDEN_CHANNEL}.</p>
	 *
	 * @see Token#getChannel()
	 */
	public const int MIN_USER_CHANNEL_VALUE = 2;

    public const int EOR_TOKEN_TYPE = 1;

    /** imaginary tree navigation type; traverse "get child" link */
    public const int DOWN = 2;
    /** imaginary tree navigation type; finish with a child list */
    public const int UP = 3;

    public const int MIN_TOKEN_TYPE = UP + 1;

    public const int INVALID_TOKEN_TYPE = 0;
    public static readonly Token INVALID_TOKEN = new CommonToken(INVALID_TOKEN_TYPE);

    /** In an action, a lexer rule can set token to this SKIP_TOKEN and ANTLR
	 *  will avoid creating a token for this symbol and try to fetch another.
	 */
    public static readonly Token SKIP_TOKEN = new CommonToken(INVALID_TOKEN_TYPE);



    /**
	 * Get the text of the token.
	 */
    String Text { get; set; }

    /** Get the token type of the token */
    int Type { get; set; }

    /** The line number on which the 1st character of this token was matched,
	 *  line=1..n
	 */
    int Line { get; set; }

    /** The index of the first character of this token relative to the
	 *  beginning of the line at which it occurs, 0..n-1
	 */
    int CharPositionInLine { get; set; }

    /** Return the channel this token. Each token can arrive at the parser
	 *  on a different channel, but the parser only "tunes" to a single channel.
	 *  The parser ignores everything not on DEFAULT_CHANNEL.
	 */
    int Channel { get; set; }

    /** An index from 0..n-1 of the token object in the input stream.
	 *  This must be valid in order to print token streams and
	 *  use TokenRewriteStream.
	 *
	 *  Return -1 to indicate that this token was conjured up since
	 *  it doesn't have a valid index.
	 */
    int TokenIndex { get; set; }

    /** The starting character index of the token
*  This method is optional; return -1 if not implemented.
*/
    int StartIndex { get; set; }

    /** The last character index of the token.
*  This method is optional; return -1 if not implemented.
*/
    int StopIndex { get; set; }

    /** Gets the {@link TokenSource} which created this token.
*/
    TokenSource TokenSource { get; set; }

    /**
* Gets the {@link CharStream} from which this token was derived.
*/
    CharStream InputStream { get; set; }
}
