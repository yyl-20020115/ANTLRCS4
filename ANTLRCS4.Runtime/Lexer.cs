/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */
using org.antlr.runtime;
using org.antlr.v4.runtime.atn;
using org.antlr.v4.runtime.dfa;
using org.antlr.v4.runtime.misc;
using System.Text;

namespace org.antlr.v4.runtime;
/** A lexer is recognizer that draws input symbols from a character stream.
 *  lexer grammars result in a subclass of this object. A Lexer object
 *  uses simplified match() and error recovery mechanisms in the interest
 *  of speed.
 */
public abstract class Lexer : Recognizer<int, LexerATNSimulator>, TokenSource
{
    public static readonly int DEFAULT_MODE = 0;
    public static readonly int MORE = -2;
    public static readonly int SKIP = -3;

    public static readonly int DEFAULT_TOKEN_CHANNEL = Token.DEFAULT_CHANNEL;
    public static readonly int HIDDEN = Token.HIDDEN_CHANNEL;
    public static readonly int MIN_CHAR_VALUE = 0x0000;
    public static readonly int MAX_CHAR_VALUE = 0x10FFFF;

    public CharStream input;
    protected Pair<TokenSource, CharStream> _tokenFactorySourcePair;

    /** How to create token objects */
    protected TokenFactory _factory = CommonTokenFactory.DEFAULT;

    /** The goal of all lexer rules/methods is to create a token object.
	 *  This is an instance variable as multiple rules may collaborate to
	 *  create a single token.  nextToken will return this object after
	 *  matching lexer rule(s).  If you subclass to allow multiple token
	 *  emissions, then set this to the last token to be matched or
	 *  something nonnull so that the auto token emit mechanism will not
	 *  emit another token.
	 */
    public Token token;

    /** What character index in the stream did the current token start at?
	 *  Needed, for example, to get the text for current token.  Set at
	 *  the start of nextToken.
	 */
    public int _tokenStartCharIndex = -1;

    /** The line on which the first character of the token resides */
    public int _tokenStartLine;

    /** The character position of first character within the line */
    public int _tokenStartCharPositionInLine;

    /** Once we see EOF on char stream, next token will be EOF.
	 *  If you have DONE : EOF ; then you see DONE EOF.
	 */
    public bool _hitEOF;

    /** The channel number for the current token */
    public int _channel;

    /** The token type for the current token */
    public int _type;

    public readonly IntegerStack _modeStack = new ();
    public int _mode = DEFAULT_MODE;

    /** You can set the text for the current token to override what is in
	 *  the input char buffer.  Use setText() or can set this instance var.
	 */
    public string _text;
    public RecognizerSharedState state;
    public Lexer() { }

    public Lexer(CharStream input)
    {
        this.input = input;
        this._tokenFactorySourcePair = new Pair<TokenSource, CharStream>(this, input);
    }
    public Lexer(CharStream input, RecognizerSharedState state)
    {
        this.state = state;
        this.input = input;
        this._tokenFactorySourcePair = new Pair<TokenSource, CharStream>(this, input);

    }
    public void Match(string s)
    {
        int i = 0;
        while (i < s.Length)
        {
            if (input.LA(1) != s[i])
            {
                if (state.backtracking > 0)
                {
                    state.failed = true;
                    return;
                }
                var mte =
                    new MismatchedTokenException(s[i], input);
                Recover(mte);
                throw mte;
            }
            i++;
            input.Consume();
            state.failed = false;
        }
    }

    public void MatchAny()
    {
        input.Consume();
    }

    public void Match(int c)
    {
        if (input.LA(1) != c)
        {
            if (state.backtracking > 0)
            {
                state.failed = true;
                return;
            }
            var mte =
                new MismatchedTokenException(c, input);
            Recover(mte);  // don't really recover; just consume in lexer
            throw mte;
        }
        input.Consume();
        state.failed = false;
    }

    public void MatchRange(int a, int b)
    {
        if (input.LA(1) < a || input.LA(1) > b)
        {
            if (state.backtracking > 0)
            {
                state.failed = true;
                return;
            }
            var mre =
                new MismatchedRangeException(a, b, input);
            Recover(mre);
            throw mre;
        }
        input.Consume();
        state.failed = false;
    }
    public void Reset()
    {
        // wack Lexer state variables
        input?.Seek(0); // rewind the input
        token = null;
        _type = Token.INVALID_TYPE;
        _channel = Token.DEFAULT_CHANNEL;
        _tokenStartCharIndex = -1;
        _tokenStartCharPositionInLine = -1;
        _tokenStartLine = -1;
        _text = null;

        _hitEOF = false;
        _mode = DEFAULT_MODE;
        _modeStack.Clear();

        GetInterpreter().Reset();
    }

    /** Return a token from this source; i.e., match a token on the char
	 *  stream.
	 */
    //@Override
    public virtual Token NextToken()
    {
        if (input == null)
            throw new IllegalStateException("nextToken requires a non-null input stream.");

        // Mark start location in char stream so unbuffered streams are
        // guaranteed at least have text of current token
        int tokenStartMarker = input.Mark();
        try
        {
        outer:
            while (true)
            {
                if (_hitEOF)
                {
                    EmitEOF();
                    return token;
                }

                token = null;
                _channel = Token.DEFAULT_CHANNEL;
                _tokenStartCharIndex = input.Index;
                _tokenStartCharPositionInLine = GetInterpreter().GetCharPositionInLine();
                _tokenStartLine = GetInterpreter().GetLine();
                _text = null;
                do
                {
                    _type = Token.INVALID_TYPE;
                    //				Console.WriteLine("nextToken line "+tokenStartLine+" at "+((char)input.LA(1))+
                    //								   " in mode "+mode+
                    //								   " at index "+input.index());
                    int ttype;
                    try
                    {
                        ttype = GetInterpreter().Match(input, _mode);
                    }
                    catch (LexerNoViableAltException e)
                    {
                        NotifyListeners(e);     // report error
                        Recover(e);
                        ttype = SKIP;
                    }
                    if (input.LA(1) == IntStream.EOF)
                    {
                        _hitEOF = true;
                    }
                    if (_type == Token.INVALID_TYPE) _type = ttype;
                    if (_type == SKIP)
                    {
                        goto outer;
                    }
                } while (_type == MORE);
                if (token == null) Emit();
                return token;
            }
        }
        finally
        {
            // make sure we release marker after match or
            // unbuffered char stream will keep buffering
            input.Release(tokenStartMarker);
        }
    }

    /** Instruct the lexer to skip creating a token for current lexer rule
	 *  and look for another token.  nextToken() knows to keep looking when
	 *  a lexer rule finishes with token set to SKIP_TOKEN.  Recall that
	 *  if token==null at end of any token rule, it creates one for you
	 *  and emits it.
	 */
    public void Skip()
    {
        _type = SKIP;
    }

    public void More()
    {
        _type = MORE;
    }

    public void Mode(int m)
    {
        _mode = m;
    }

    public void PushMode(int m)
    {
        if (LexerATNSimulator.debug) Console.WriteLine("pushMode " + m);
        _modeStack.Push(_mode);
        Mode(m);
    }

    public int PopMode()
    {
        if (_modeStack.IsEmpty) throw new EmptyStackException();
        if (LexerATNSimulator.debug) Console.WriteLine("popMode back to " + _modeStack.Peek());
        Mode(_modeStack.Pop());
        return _mode;
    }

    //@Override
    //@Override
    public override TokenFactory TokenFactory { get => _factory; set => this._factory = value; }

    /** Set the char stream and reset the lexer */
    //@Override
    public override void SetInputStream(IntStream input)
    {
        this.input = null;
        this._tokenFactorySourcePair = new Pair<TokenSource, CharStream>(this, this.input);
        Reset();
        this.input = (CharStream)input;
        this._tokenFactorySourcePair = new Pair<TokenSource, CharStream>(this, this.input);
    }

    //@Override
    public virtual string SourceName => input.SourceName;

    //@Override
    public override CharStream InputStream => input;

    /** By default does not support multiple emits per nextToken invocation
	 *  for efficiency reasons.  Subclass and override this method, nextToken,
	 *  and getToken (to push tokens into a list and pull from that list
	 *  rather than a single variable as this implementation does).
	 */
    public void Emit(Token token)
    {
        //Console.Error.WriteLine("emit "+token);
        this.token = token;
    }

    /** The standard method called to automatically emit a token at the
	 *  outermost lexical rule.  The token object should point into the
	 *  char buffer start..stop.  If there is a text override in 'text',
	 *  use that to set the token's text.  Override this method to emit
	 *  custom Token objects or provide a new factory.
	 */
    public Token Emit()
    {
        var t = (_factory as TokenFactory<Token>).Create(_tokenFactorySourcePair, _type, _text, _channel, _tokenStartCharIndex, GetCharIndex() - 1,
                                  _tokenStartLine, _tokenStartCharPositionInLine);
        Emit(t);
        return t;
    }

    public Token EmitEOF()
    {
        int cpos = CharPositionInLine;
        int line = Line;
        var eof = (_factory as TokenFactory<Token>).Create(_tokenFactorySourcePair, Token.EOF, null, Token.DEFAULT_CHANNEL, input.Index, input.Index - 1,
                                    line, cpos);
        Emit(eof);
        return eof;
    }

    //@Override
    public int Line => GetInterpreter().GetLine();

    //@Override
    public int CharPositionInLine => GetInterpreter().GetCharPositionInLine();

    public void SetLine(int line) => GetInterpreter().SetLine(line);

    public void SetCharPositionInLine(int charPositionInLine)
    {
        GetInterpreter().SetCharPositionInLine(charPositionInLine);
    }

    /** What is the index of the current character of lookahead? */
    public int GetCharIndex()
    {
        return input.Index;
    }

    /** Return the text matched so far for the current token or any
	 *  text override.
	 */
    /** Set the complete text of this token; it wipes any previous
 *  changes to the text.
 */
    public string Text
    {
        get => _text ?? GetInterpreter().GetText(input);
        set => this._text = value;
    }

    /** Override if emitting multiple tokens. */
    public Token Token { get => token; set => this.token = value; }
    public int Type { get => _type; set => _type = value; }
    public int Channel { get => _channel; set => _channel = value; }

    public virtual string[] GetChannelNames() => null;

    public virtual string[] GetModeNames() => null;

    /** Used to print out token names like ID during debugging and
	 *  error reporting.  The generated parsers implement a method
	 *  that overrides this to point to their String[] tokenNames.
	 */
    //@Override
    //@Deprecated
    public override string[] GetTokenNames() => null;

    /** Return a list of all Token objects in input char stream.
	 *  Forces load of all tokens. Does not include EOF token.
	 */
    public List<Token> GetAllTokens()
    {
        List<Token> tokens = new();
        Token t = NextToken();
        while (t.Type != Token.EOF)
        {
            tokens.Add(t);
            t = NextToken();
        }
        return tokens;
    }

    public void Recover(LexerNoViableAltException e)
    {
        if (input.LA(1) != IntStream.EOF)
        {
            // skip a char and try again
            GetInterpreter().Consume(input);
        }
    }

    public void NotifyListeners(LexerNoViableAltException e)
    {
        var text = input.GetText(Interval.Of(_tokenStartCharIndex, input.Index));
        var msg = "token recognition error at: '" + GetErrorDisplay(text) + "'";

        var listener = GetErrorListenerDispatch();
        listener.SyntaxError(this, null, _tokenStartLine, _tokenStartCharPositionInLine, msg, e);
    }

    public string GetErrorDisplay(String s)
    {
        var buffer = new StringBuilder();
        foreach (char c in s.ToCharArray())
        {
            buffer.Append(GetErrorDisplay(c));
        }
        return buffer.ToString();
    }

    public string GetErrorDisplay(int c)
    {
        var s = c.ToString();// String.valueOf((char)c);
        switch (c)
        {
            case Token.EOF:
                s = "<EOF>";
                break;
            case '\n':
                s = "\\n";
                break;
            case '\t':
                s = "\\t";
                break;
            case '\r':
                s = "\\r";
                break;
        }
        return s;
    }

    public string GetCharErrorDisplay(int c)
    {
        var s = GetErrorDisplay(c);
        return "'" + s + "'";
    }

    /** Lexers can normally match any char in it's vocabulary after matching
	 *  a token, so do the easy thing and just kill a character and hope
	 *  it all works out.  You can instead use the rule invocation stack
	 *  to do sophisticated error recovery if you are in a fragment rule.
	 */
    public virtual void Recover(RecognitionException re)
    {
        //Console.WriteLine("consuming char "+(char)input.LA(1)+" during recovery");
        //re.printStackTrace();
        // TODO: Do we lose character or line position information?
        input.Consume();
    }
}
