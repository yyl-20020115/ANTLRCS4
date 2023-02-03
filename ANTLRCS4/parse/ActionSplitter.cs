// $ANTLR 3.5.3 org\\antlr\\v4\\parse\\ActionSplitter.g 2023-01-27 22:27:34

using org.antlr.runtime;
using org.antlr.v4.parse;
using org.antlr.v4.runtime;
using org.antlr.v4.runtime.atn;
using System.Text;

namespace org.antlr.v4.parse;
public class ActionSplitter : antlr.runtime.Lexer
{

    public const int EOF = -1;
    public const int ATTR = 4;
    public const int ATTR_VALUE_EXPR = 5;
    public const int COMMENT = 6;
    public const int ID = 7;
    public const int LINE_COMMENT = 8;
    public const int NONLOCAL_ATTR = 9;
    public const int QUALIFIED_ATTR = 10;
    public const int SET_ATTR = 11;
    public const int SET_NONLOCAL_ATTR = 12;
    public const int TEXT = 13;
    public const int WS = 14;

    ActionSplitterListener @delegate;

    public ActionSplitter(CharStream input, ActionSplitterListener @delegate)
        : this(input, new RecognizerSharedState())
    {
        ;
        this.@delegate = @delegate;
    }

    /** force filtering (and return tokens). triggers all above actions. */
    public List<Token> getActionTokens()
    {
        List<Token> chunks = new();
        Token t = nextToken();
        while (t.getType() != Token.EOF)
        {
            chunks.Add(t);
            t = nextToken();
        }
        return chunks;
    }

    private bool isIDStartChar(int c)
    {
        return c == '_' || char.IsLetter((char)c);
    }


    // delegates
    // delegators
    public antlr.runtime.Lexer[] getDelegates()
    {
        return new antlr.runtime.Lexer[] { };
    }

    public ActionSplitter() { }
    public ActionSplitter(CharStream input)
        : this(input, new RecognizerSharedState())
    {
    }
    public ActionSplitter(CharStream input, RecognizerSharedState state)
        : base(input, state) 
    {
    }
    //@Override 
    public override String GetGrammarFileName() { return "org\\antlr\\v4\\parse\\ActionSplitter.g"; }

    //@Override
    public virtual Token nextToken()
    {
        while (true)
        {
            if (input.LA(1) == CharStream.EOF)
            {
                Token eof = new CommonToken(input, Token.EOF,
                                            Token.DEFAULT_CHANNEL,
                                            input.index(), input.index());
                eof.setLine(getLine());
                eof.setCharPositionInLine(getCharPositionInLine());
                return eof;
            }
            state.token = null;
            state.channel = Token.DEFAULT_CHANNEL;
            state.tokenStartCharIndex = input.index();
            state.tokenStartCharPositionInLine = input.getCharPositionInLine();
            state.tokenStartLine = input.getLine();
            state.text = null;
            try
            {
                int m = input.mark();
                state.backtracking = 1;
                state.failed = false;
                mTokens();
                state.backtracking = 0;
                if (state.failed)
                {
                    input.rewind(m);
                    input.consume();
                }
                else
                {
                    emit();
                    return state.token;
                }
            }
            catch (RecognitionException re)
            {
                // shouldn't happen in backtracking mode, but...
                reportError(re);
                recover(re);
            }
        }
    }

    //@Override
    public void memoize(IntStream input,
            int ruleIndex,
            int ruleStartIndex)
    {
        if (state.backtracking > 1) base.Memoize(input, ruleIndex, ruleStartIndex);
    }

    //@Override
    public bool alreadyParsedRule(IntStream input, int ruleIndex)
    {
        if (state.backtracking > 1) return base.AlreadyParsedRule(input, ruleIndex);
        return false;
    }
    // $ANTLR start "COMMENT"
    public void mCOMMENT()
    {
        try
        {
            int _type = COMMENT;
            int _channel = DEFAULT_TOKEN_CHANNEL;
            // org\\antlr\\v4\\parse\\ActionSplitter.g:68:5: ( '/*' ( options {greedy=false; } : . )* '*/' )
            // org\\antlr\\v4\\parse\\ActionSplitter.g:68:9: '/*' ( options {greedy=false; } : . )* '*/'
            {
                match("/*"); if (state.failed) return;

                // org\\antlr\\v4\\parse\\ActionSplitter.g:68:14: ( options {greedy=false; } : . )*
                loop1:
                while (true)
                {
                    int alt1 = 2;
                    int LA1_0 = input.LA(1);
                    if ((LA1_0 == '*'))
                    {
                        int LA1_1 = input.LA(2);
                        if ((LA1_1 == '/'))
                        {
                            alt1 = 2;
                        }
                        else if (((LA1_1 >= '\u0000' && LA1_1 <= '.') || (LA1_1 >= '0' && LA1_1 <= '\uFFFF')))
                        {
                            alt1 = 1;
                        }

                    }
                    else if (((LA1_0 >= '\u0000' && LA1_0 <= ')') || (LA1_0 >= '+' && LA1_0 <= '\uFFFF')))
                    {
                        alt1 = 1;
                    }

                    switch (alt1)
                    {
                        case 1:
                            // org\\antlr\\v4\\parse\\ActionSplitter.g:68:42: .
                            {
                                matchAny(); if (state.failed) return;
                            }
                            break;

                        default:
                            goto exit1;
                            //break loop1;
                    }
                }
                exit1:
                match("*/"); if (state.failed) return;

                if (state.backtracking == 1) { @delegate.Text(getText()); }
            }

            state.type = _type;
            state.channel = _channel;
        }
        finally
        {
            // do for sure before leaving
        }
    }
    // $ANTLR end "COMMENT"

    // $ANTLR start "LINE_COMMENT"
    public void mLINE_COMMENT()
    {
        try
        {
            int _type = LINE_COMMENT;
            int _channel = DEFAULT_TOKEN_CHANNEL;
            // org\\antlr\\v4\\parse\\ActionSplitter.g:72:5: ( '//' (~ ( '\\n' | '\\r' ) )* ( '\\r' )? '\\n' )
            // org\\antlr\\v4\\parse\\ActionSplitter.g:72:7: '//' (~ ( '\\n' | '\\r' ) )* ( '\\r' )? '\\n'
            {
                match("//"); if (state.failed) return;

                // org\\antlr\\v4\\parse\\ActionSplitter.g:72:12: (~ ( '\\n' | '\\r' ) )*
                loop2:
                while (true)
                {
                    int alt2 = 2;
                    int LA2_0 = input.LA(1);
                    if (((LA2_0 >= '\u0000' && LA2_0 <= '\t') || (LA2_0 >= '\u000B' && LA2_0 <= '\f') || (LA2_0 >= '\u000E' && LA2_0 <= '\uFFFF')))
                    {
                        alt2 = 1;
                    }

                    switch (alt2)
                    {
                        case 1:
                            // org\\antlr\\v4\\parse\\ActionSplitter.g:
                            {
                                if ((input.LA(1) >= '\u0000' && input.LA(1) <= '\t') || (input.LA(1) >= '\u000B' && input.LA(1) <= '\f') || (input.LA(1) >= '\u000E' && input.LA(1) <= '\uFFFF'))
                                {
                                    input.consume();
                                    state.failed = false;
                                }
                                else
                                {
                                    if (state.backtracking > 0) { state.failed = true; return; }
                                    MismatchedSetException mse = new MismatchedSetException(null, input);
                                    recover(mse);
                                    throw mse;
                                }
                            }
                            break;

                        default:
                            goto exit2;
                            //break loop2;
                    }
                }
                exit2:
                // org\\antlr\\v4\\parse\\ActionSplitter.g:72:26: ( '\\r' )?
                int alt3 = 2;
                int LA3_0 = input.LA(1);
                if ((LA3_0 == '\r'))
                {
                    alt3 = 1;
                }
                switch (alt3)
                {
                    case 1:
                        // org\\antlr\\v4\\parse\\ActionSplitter.g:72:26: '\\r'
                        {
                            match('\r'); if (state.failed) return;
                        }
                        break;

                }

                match('\n'); if (state.failed) return;
                if (state.backtracking == 1) { @delegate.Text(getText()); }
            }

            state.type = _type;
            state.channel = _channel;
        }
        finally
        {
            // do for sure before leaving
        }
    }
    // $ANTLR end "LINE_COMMENT"

    // $ANTLR start "SET_NONLOCAL_ATTR"
    public void mSET_NONLOCAL_ATTR()
    {
        try
        {
            int _type = SET_NONLOCAL_ATTR;
            int _channel = DEFAULT_TOKEN_CHANNEL;
            CommonToken x = null;
            CommonToken y = null;
            CommonToken expr = null;

            // org\\antlr\\v4\\parse\\ActionSplitter.g:76:2: ( '$' x= ID '::' y= ID ( WS )? '=' expr= ATTR_VALUE_EXPR ';' )
            // org\\antlr\\v4\\parse\\ActionSplitter.g:76:4: '$' x= ID '::' y= ID ( WS )? '=' expr= ATTR_VALUE_EXPR ';'
            {
                match('$'); if (state.failed) return;
                int xStart115 = getCharIndex();
                int xStartLine115 = getLine();
                int xStartCharPos115 = getCharPositionInLine();
                mID(); if (state.failed) return;
                x = new CommonToken(input, Token.INVALID_TOKEN_TYPE, Token.DEFAULT_CHANNEL, xStart115, getCharIndex() - 1);
                x.setLine(xStartLine115);
                x.setCharPositionInLine(xStartCharPos115);

                match("::"); if (state.failed) return;

                int yStart121 = getCharIndex();
                int yStartLine121 = getLine();
                int yStartCharPos121 = getCharPositionInLine();
                mID(); if (state.failed) return;
                y = new CommonToken(input, Token.INVALID_TOKEN_TYPE, Token.DEFAULT_CHANNEL, yStart121, getCharIndex() - 1);
                y.setLine(yStartLine121);
                y.setCharPositionInLine(yStartCharPos121);

                // org\\antlr\\v4\\parse\\ActionSplitter.g:76:23: ( WS )?
                int alt4 = 2;
                int LA4_0 = input.LA(1);
                if (((LA4_0 >= '\t' && LA4_0 <= '\n') || LA4_0 == '\r' || LA4_0 == ' '))
                {
                    alt4 = 1;
                }
                switch (alt4)
                {
                    case 1:
                        // org\\antlr\\v4\\parse\\ActionSplitter.g:76:23: WS
                        {
                            mWS(); if (state.failed) return;

                        }
                        break;

                }

                match('='); if (state.failed) return;
                int exprStart130 = getCharIndex();
                int exprStartLine130 = getLine();
                int exprStartCharPos130 = getCharPositionInLine();
                mATTR_VALUE_EXPR(); if (state.failed) return;
                expr = new CommonToken(input, Token.INVALID_TOKEN_TYPE, Token.DEFAULT_CHANNEL, exprStart130, getCharIndex() - 1);
                expr.setLine(exprStartLine130);
                expr.setCharPositionInLine(exprStartCharPos130);

                match(';'); if (state.failed) return;
                if (state.backtracking == 1)
                {
                    @delegate.SetNonLocalAttr(getText(), x, y, expr);
                }
            }

            state.type = _type;
            state.channel = _channel;
        }
        finally
        {
            // do for sure before leaving
        }
    }
    // $ANTLR end "SET_NONLOCAL_ATTR"

    // $ANTLR start "NONLOCAL_ATTR"
    public void mNONLOCAL_ATTR()
    {
        try
        {
            int _type = NONLOCAL_ATTR;
            int _channel = DEFAULT_TOKEN_CHANNEL;
            CommonToken x = null;
            CommonToken y = null;

            // org\\antlr\\v4\\parse\\ActionSplitter.g:83:2: ( '$' x= ID '::' y= ID )
            // org\\antlr\\v4\\parse\\ActionSplitter.g:83:4: '$' x= ID '::' y= ID
            {
                match('$'); if (state.failed) return;
                int xStart151 = getCharIndex();
                int xStartLine151 = getLine();
                int xStartCharPos151 = getCharPositionInLine();
                mID(); if (state.failed) return;
                x = new CommonToken(input, Token.INVALID_TOKEN_TYPE, Token.DEFAULT_CHANNEL, xStart151, getCharIndex() - 1);
                x.setLine(xStartLine151);
                x.setCharPositionInLine(xStartCharPos151);

                match("::"); if (state.failed) return;

                int yStart157 = getCharIndex();
                int yStartLine157 = getLine();
                int yStartCharPos157 = getCharPositionInLine();
                mID(); if (state.failed) return;
                y = new CommonToken(input, Token.INVALID_TOKEN_TYPE, Token.DEFAULT_CHANNEL, yStart157, getCharIndex() - 1);
                y.setLine(yStartLine157);
                y.setCharPositionInLine(yStartCharPos157);

                if (state.backtracking == 1) { @delegate.NonLocalAttr(getText(), x, y); }
            }

            state.type = _type;
            state.channel = _channel;
        }
        finally
        {
            // do for sure before leaving
        }
    }
    // $ANTLR end "NONLOCAL_ATTR"

    // $ANTLR start "QUALIFIED_ATTR"
    public void mQUALIFIED_ATTR()
    {
        try
        {
            int _type = QUALIFIED_ATTR;
            int _channel = DEFAULT_TOKEN_CHANNEL;
            CommonToken x = null;
            CommonToken y = null;

            // org\\antlr\\v4\\parse\\ActionSplitter.g:87:2: ( '$' x= ID '.' y= ID {...}?)
            // org\\antlr\\v4\\parse\\ActionSplitter.g:87:4: '$' x= ID '.' y= ID {...}?
            {
                match('$'); if (state.failed) return;
                int xStart174 = getCharIndex();
                int xStartLine174 = getLine();
                int xStartCharPos174 = getCharPositionInLine();
                mID(); if (state.failed) return;
                x = new CommonToken(input, Token.INVALID_TOKEN_TYPE, Token.DEFAULT_CHANNEL, xStart174, getCharIndex() - 1);
                x.setLine(xStartLine174);
                x.setCharPositionInLine(xStartCharPos174);

                match('.'); if (state.failed) return;
                int yStart180 = getCharIndex();
                int yStartLine180 = getLine();
                int yStartCharPos180 = getCharPositionInLine();
                mID(); if (state.failed) return;
                y = new CommonToken(input, Token.INVALID_TOKEN_TYPE, Token.DEFAULT_CHANNEL, yStart180, getCharIndex() - 1);
                y.setLine(yStartLine180);
                y.setCharPositionInLine(yStartCharPos180);

                if (!((input.LA(1) != '(')))
                {
                    if (state.backtracking > 0) { state.failed = true; return; }
                    throw new FailedPredicateException(input, "QUALIFIED_ATTR", "input.LA(1)!='('");
                }
                if (state.backtracking == 1) { @delegate.QualifiedAttr(getText(), x, y); }
            }

            state.type = _type;
            state.channel = _channel;
        }
        finally
        {
            // do for sure before leaving
        }
    }
    // $ANTLR end "QUALIFIED_ATTR"

    // $ANTLR start "SET_ATTR"
    public void mSET_ATTR()
    {
        try
        {
            int _type = SET_ATTR;
            int _channel = DEFAULT_TOKEN_CHANNEL;
            CommonToken x = null;
            CommonToken expr = null;

            // org\\antlr\\v4\\parse\\ActionSplitter.g:91:2: ( '$' x= ID ( WS )? '=' expr= ATTR_VALUE_EXPR ';' )
            // org\\antlr\\v4\\parse\\ActionSplitter.g:91:4: '$' x= ID ( WS )? '=' expr= ATTR_VALUE_EXPR ';'
            {
                match('$'); if (state.failed) return;
                int xStart199 = getCharIndex();
                int xStartLine199 = getLine();
                int xStartCharPos199 = getCharPositionInLine();
                mID(); if (state.failed) return;
                x = new CommonToken( input, Token.INVALID_TOKEN_TYPE, Token.DEFAULT_CHANNEL, xStart199, getCharIndex() - 1);
                x.setLine(xStartLine199);
                x.setCharPositionInLine(xStartCharPos199);

                // org\\antlr\\v4\\parse\\ActionSplitter.g:91:13: ( WS )?
                int alt5 = 2;
                int LA5_0 = input.LA(1);
                if (((LA5_0 >= '\t' && LA5_0 <= '\n') || LA5_0 == '\r' || LA5_0 == ' '))
                {
                    alt5 = 1;
                }
                switch (alt5)
                {
                    case 1:
                        // org\\antlr\\v4\\parse\\ActionSplitter.g:91:13: WS
                        {
                            mWS(); if (state.failed) return;

                        }
                        break;

                }

                match('='); if (state.failed) return;
                int exprStart208 = getCharIndex();
                int exprStartLine208 = getLine();
                int exprStartCharPos208 = getCharPositionInLine();
                mATTR_VALUE_EXPR(); if (state.failed) return;
                expr = new CommonToken(input, Token.INVALID_TOKEN_TYPE, Token.DEFAULT_CHANNEL, exprStart208, getCharIndex() - 1);
                expr.setLine(exprStartLine208);
                expr.setCharPositionInLine(exprStartCharPos208);

                match(';'); if (state.failed) return;
                if (state.backtracking == 1)
                {
                    @delegate.SetAttr(getText(), x, expr);
                }
            }

            state.type = _type;
            state.channel = _channel;
        }
        finally
        {
            // do for sure before leaving
        }
    }
    // $ANTLR end "SET_ATTR"

    // $ANTLR start "ATTR"
    public void mATTR()
    {
        try
        {
            int _type = ATTR;
            int _channel = DEFAULT_TOKEN_CHANNEL;
            CommonToken x = null;

            // org\\antlr\\v4\\parse\\ActionSplitter.g:98:2: ( '$' x= ID )
            // org\\antlr\\v4\\parse\\ActionSplitter.g:98:4: '$' x= ID
            {
                match('$'); if (state.failed) return;
                int xStart229 = getCharIndex();
                int xStartLine229 = getLine();
                int xStartCharPos229 = getCharPositionInLine();
                mID(); if (state.failed) return;
                x = new CommonToken(input, Token.INVALID_TOKEN_TYPE, Token.DEFAULT_CHANNEL, xStart229, getCharIndex() - 1);
                x.setLine(xStartLine229);
                x.setCharPositionInLine(xStartCharPos229);

                if (state.backtracking == 1) { @delegate.Attr(getText(), x); }
            }

            state.type = _type;
            state.channel = _channel;
        }
        finally
        {
            // do for sure before leaving
        }
    }
    // $ANTLR end "ATTR"

    // $ANTLR start "TEXT"
    public void mTEXT()
    {
        try
        {
            int _type = TEXT;
            int _channel = DEFAULT_TOKEN_CHANNEL;
            int c;

            StringBuilder buf = new StringBuilder();
            // org\\antlr\\v4\\parse\\ActionSplitter.g:105:2: ( (c=~ ( '\\\\' | '$' ) | '\\\\$' | '\\\\' c=~ ( '$' ) |{...}? => '$' )+ )
            // org\\antlr\\v4\\parse\\ActionSplitter.g:105:4: (c=~ ( '\\\\' | '$' ) | '\\\\$' | '\\\\' c=~ ( '$' ) |{...}? => '$' )+
            {
                // org\\antlr\\v4\\parse\\ActionSplitter.g:105:4: (c=~ ( '\\\\' | '$' ) | '\\\\$' | '\\\\' c=~ ( '$' ) |{...}? => '$' )+
                int cnt6 = 0;
            loop6:
                while (true)
                {
                    int alt6 = 5;
                    int LA6_0 = input.LA(1);
                    if (((LA6_0 >= '\u0000' && LA6_0 <= '#') || (LA6_0 >= '%' && LA6_0 <= '[') || (LA6_0 >= ']' && LA6_0 <= '\uFFFF')))
                    {
                        alt6 = 1;
                    }
                    else if ((LA6_0 == '\\'))
                    {
                        int LA6_3 = input.LA(2);
                        if ((LA6_3 == '$'))
                        {
                            alt6 = 2;
                        }
                        else if (((LA6_3 >= '\u0000' && LA6_3 <= '#') || (LA6_3 >= '%' && LA6_3 <= '\uFFFF')))
                        {
                            alt6 = 3;
                        }

                    }
                    else if ((LA6_0 == '$') && ((!isIDStartChar(input.LA(2)))))
                    {
                        alt6 = 4;
                    }

                    switch (alt6)
                    {
                        case 1:
                            // org\\antlr\\v4\\parse\\ActionSplitter.g:105:6: c=~ ( '\\\\' | '$' )
                            {
                                c = input.LA(1);
                                if ((input.LA(1) >= '\u0000' && input.LA(1) <= '#') || (input.LA(1) >= '%' && input.LA(1) <= '[') || (input.LA(1) >= ']' && input.LA(1) <= '\uFFFF'))
                                {
                                    input.consume();
                                    state.failed = false;
                                }
                                else
                                {
                                    if (state.backtracking > 0) { state.failed = true; return; }
                                    MismatchedSetException mse = new MismatchedSetException(null, input);
                                    recover(mse);
                                    throw mse;
                                }
                                if (state.backtracking == 1) { buf.Append((char)c); }
                            }
                            break;
                        case 2:
                            // org\\antlr\\v4\\parse\\ActionSplitter.g:106:5: '\\\\$'
                            {
                                match("\\$"); if (state.failed) return;

                                if (state.backtracking == 1) { buf.Append('$'); }
                            }
                            break;
                        case 3:
                            // org\\antlr\\v4\\parse\\ActionSplitter.g:107:5: '\\\\' c=~ ( '$' )
                            {
                                match('\\'); if (state.failed) return;
                                c = input.LA(1);
                                if ((input.LA(1) >= '\u0000' && input.LA(1) <= '#') || (input.LA(1) >= '%' && input.LA(1) <= '\uFFFF'))
                                {
                                    input.consume();
                                    state.failed = false;
                                }
                                else
                                {
                                    if (state.backtracking > 0) { state.failed = true; return; }
                                    MismatchedSetException mse = new MismatchedSetException(null, input);
                                    recover(mse);
                                    throw mse;
                                }
                                if (state.backtracking == 1) { buf.Append('\\').Append((char)c); }
                            }
                            break;
                        case 4:
                            // org\\antlr\\v4\\parse\\ActionSplitter.g:108:5: {...}? => '$'
                            {
                                if (!((!isIDStartChar(input.LA(2)))))
                                {
                                    if (state.backtracking > 0) { state.failed = true; return; }
                                    throw new FailedPredicateException(input, "TEXT", "!isIDStartChar(input.LA(2))");
                                }
                                match('$'); if (state.failed) return;
                                if (state.backtracking == 1) { buf.Append('$'); }
                            }
                            break;

                        default:
                            if (cnt6 >= 1) goto exit6;// break loop6;
                            if (state.backtracking > 0) { state.failed = true; return; }
                            EarlyExitException eee = new EarlyExitException(6, input);
                            throw eee;
                    }
                    cnt6++;
                }
            exit6:
                ;
            }

            state.type = _type;
            state.channel = _channel;
            if (state.backtracking == 1) { @delegate.Text(buf.ToString()); }
        }
        finally
        {
            // do for sure before leaving
        }
    }
    // $ANTLR end "TEXT"

    // $ANTLR start "ID"
    public void mID()
    {
        try
        {
            // org\\antlr\\v4\\parse\\ActionSplitter.g:113:5: ( ( 'a' .. 'z' | 'A' .. 'Z' | '_' ) ( 'a' .. 'z' | 'A' .. 'Z' | '0' .. '9' | '_' )* )
            // org\\antlr\\v4\\parse\\ActionSplitter.g:113:7: ( 'a' .. 'z' | 'A' .. 'Z' | '_' ) ( 'a' .. 'z' | 'A' .. 'Z' | '0' .. '9' | '_' )*
            {
                if ((input.LA(1) >= 'A' && input.LA(1) <= 'Z') || input.LA(1) == '_' || (input.LA(1) >= 'a' && input.LA(1) <= 'z'))
                {
                    input.consume();
                    state.failed = false;
                }
                else
                {
                    if (state.backtracking > 0) { state.failed = true; return; }
                    MismatchedSetException mse = new MismatchedSetException(null, input);
                    recover(mse);
                    throw mse;
                }
            // org\\antlr\\v4\\parse\\ActionSplitter.g:113:31: ( 'a' .. 'z' | 'A' .. 'Z' | '0' .. '9' | '_' )*
            loop7:
                while (true)
                {
                    int alt7 = 2;
                    int LA7_0 = input.LA(1);
                    if (((LA7_0 >= '0' && LA7_0 <= '9') || (LA7_0 >= 'A' && LA7_0 <= 'Z') || LA7_0 == '_' || (LA7_0 >= 'a' && LA7_0 <= 'z')))
                    {
                        alt7 = 1;
                    }

                    switch (alt7)
                    {
                        case 1:
                            // org\\antlr\\v4\\parse\\ActionSplitter.g:
                            {
                                if ((input.LA(1) >= '0' && input.LA(1) <= '9') || (input.LA(1) >= 'A' && input.LA(1) <= 'Z') || input.LA(1) == '_' || (input.LA(1) >= 'a' && input.LA(1) <= 'z'))
                                {
                                    input.consume();
                                    state.failed = false;
                                }
                                else
                                {
                                    if (state.backtracking > 0) { state.failed = true; return; }
                                    MismatchedSetException mse = new MismatchedSetException(null, input);
                                    recover(mse);
                                    throw mse;
                                }
                            }
                            break;

                        default:
                            goto exit7;
                            //break loop7;
                    }
                }
            exit7:
                ;
            }

        }
        finally
        {
            // do for sure before leaving
        }
    }
    // $ANTLR end "ID"

    // $ANTLR start "ATTR_VALUE_EXPR"
    public void mATTR_VALUE_EXPR()
    {
        try
        {
            // org\\antlr\\v4\\parse\\ActionSplitter.g:119:2: (~ '=' (~ ';' )* )
            // org\\antlr\\v4\\parse\\ActionSplitter.g:119:4: ~ '=' (~ ';' )*
            {
                if ((input.LA(1) >= '\u0000' && input.LA(1) <= '<') || (input.LA(1) >= '>' && input.LA(1) <= '\uFFFF'))
                {
                    input.consume();
                    state.failed = false;
                }
                else
                {
                    if (state.backtracking > 0) { state.failed = true; return; }
                    MismatchedSetException mse = new MismatchedSetException(null, input);
                    recover(mse);
                    throw mse;
                }
            // org\\antlr\\v4\\parse\\ActionSplitter.g:119:9: (~ ';' )*
            loop8:
                while (true)
                {
                    int alt8 = 2;
                    int LA8_0 = input.LA(1);
                    if (((LA8_0 >= '\u0000' && LA8_0 <= ':') || (LA8_0 >= '<' && LA8_0 <= '\uFFFF')))
                    {
                        alt8 = 1;
                    }

                    switch (alt8)
                    {
                        case 1:
                            // org\\antlr\\v4\\parse\\ActionSplitter.g:
                            {
                                if ((input.LA(1) >= '\u0000' && input.LA(1) <= ':') || (input.LA(1) >= '<' && input.LA(1) <= '\uFFFF'))
                                {
                                    input.consume();
                                    state.failed = false;
                                }
                                else
                                {
                                    if (state.backtracking > 0) { state.failed = true; return; }
                                    MismatchedSetException mse = new MismatchedSetException(null, input);
                                    recover(mse);
                                    throw mse;
                                }
                            }
                            break;

                        default:
                            goto exit8;
                            //break loop8;
                    }
                }
            exit8:
                ;
            }

        }
        finally
        {
            // do for sure before leaving
        }
    }
    // $ANTLR end "ATTR_VALUE_EXPR"

    // $ANTLR start "WS"
    public void mWS()
    {
        try
        {
            // org\\antlr\\v4\\parse\\ActionSplitter.g:123:4: ( ( ' ' | '\\t' | '\\n' | '\\r' )+ )
            // org\\antlr\\v4\\parse\\ActionSplitter.g:123:6: ( ' ' | '\\t' | '\\n' | '\\r' )+
            {
                // org\\antlr\\v4\\parse\\ActionSplitter.g:123:6: ( ' ' | '\\t' | '\\n' | '\\r' )+
                int cnt9 = 0;
            loop9:
                while (true)
                {
                    int alt9 = 2;
                    int LA9_0 = input.LA(1);
                    if (((LA9_0 >= '\t' && LA9_0 <= '\n') || LA9_0 == '\r' || LA9_0 == ' '))
                    {
                        alt9 = 1;
                    }

                    switch (alt9)
                    {
                        case 1:
                            // org\\antlr\\v4\\parse\\ActionSplitter.g:
                            {
                                if ((input.LA(1) >= '\t' && input.LA(1) <= '\n') || input.LA(1) == '\r' || input.LA(1) == ' ')
                                {
                                    input.consume();
                                    state.failed = false;
                                }
                                else
                                {
                                    if (state.backtracking > 0) { state.failed = true; return; }
                                    MismatchedSetException mse = new MismatchedSetException(null, input);
                                    recover(mse);
                                    throw mse;
                                }
                            }
                            break;

                        default:
                            if (cnt9 >= 1) goto exit9;// break loop9;
                            if (state.backtracking > 0) { state.failed = true; return; }
                            EarlyExitException eee = new EarlyExitException(9, input);
                            throw eee;
                    }
                    cnt9++;
                }
            exit9:
                ;
            }

        }
        finally
        {
            // do for sure before leaving
        }
    }
    // $ANTLR end "WS"

    //@Override
    public override void mTokens()
    {
        // org\\antlr\\v4\\parse\\ActionSplitter.g:1:39: ( COMMENT | LINE_COMMENT | SET_NONLOCAL_ATTR | NONLOCAL_ATTR | QUALIFIED_ATTR | SET_ATTR | ATTR | TEXT )
        int alt10 = 8;
        int LA10_0 = input.LA(1);
        if ((LA10_0 == '/'))
        {
            int LA10_1 = input.LA(2);
            if ((synpred1_ActionSplitter()))
            {
                alt10 = 1;
            }
            else if ((synpred2_ActionSplitter()))
            {
                alt10 = 2;
            }
            else if ((true))
            {
                alt10 = 8;
            }

        }

        else if ((LA10_0 == '$'))
        {
            int LA10_2 = input.LA(2);
            if ((synpred3_ActionSplitter()))
            {
                alt10 = 3;
            }
            else if ((synpred4_ActionSplitter()))
            {
                alt10 = 4;
            }
            else if ((synpred5_ActionSplitter()))
            {
                alt10 = 5;
            }
            else if ((synpred6_ActionSplitter()))
            {
                alt10 = 6;
            }
            else if ((synpred7_ActionSplitter()))
            {
                alt10 = 7;
            }
            else if (((!isIDStartChar(input.LA(2)))))
            {
                alt10 = 8;
            }

            else
            {
                if (state.backtracking > 0) { state.failed = true; return; }
                int nvaeMark = input.mark();
                try
                {
                    input.consume();
                    NoViableAltException nvae =
                        new NoViableAltException("", 10, 2, input);
                    throw nvae;
                }
                finally
                {
                    input.rewind(nvaeMark);
                }
            }

        }
        else if (((LA10_0 >= '\u0000' && LA10_0 <= '#') || (LA10_0 >= '%' && LA10_0 <= '.') || (LA10_0 >= '0' && LA10_0 <= '\uFFFF')))
        {
            alt10 = 8;
        }

        else
        {
            if (state.backtracking > 0) { state.failed = true; return; }
            NoViableAltException nvae =
                new NoViableAltException("", 10, 0, input);
            throw nvae;
        }

        switch (alt10)
        {
            case 1:
                // org\\antlr\\v4\\parse\\ActionSplitter.g:1:41: COMMENT
                {
                    mCOMMENT(); if (state.failed) return;

                }
                break;
            case 2:
                // org\\antlr\\v4\\parse\\ActionSplitter.g:1:49: LINE_COMMENT
                {
                    mLINE_COMMENT(); if (state.failed) return;

                }
                break;
            case 3:
                // org\\antlr\\v4\\parse\\ActionSplitter.g:1:62: SET_NONLOCAL_ATTR
                {
                    mSET_NONLOCAL_ATTR(); if (state.failed) return;

                }
                break;
            case 4:
                // org\\antlr\\v4\\parse\\ActionSplitter.g:1:80: NONLOCAL_ATTR
                {
                    mNONLOCAL_ATTR(); if (state.failed) return;

                }
                break;
            case 5:
                // org\\antlr\\v4\\parse\\ActionSplitter.g:1:94: QUALIFIED_ATTR
                {
                    mQUALIFIED_ATTR(); if (state.failed) return;

                }
                break;
            case 6:
                // org\\antlr\\v4\\parse\\ActionSplitter.g:1:109: SET_ATTR
                {
                    mSET_ATTR(); if (state.failed) return;

                }
                break;
            case 7:
                // org\\antlr\\v4\\parse\\ActionSplitter.g:1:118: ATTR
                {
                    mATTR(); if (state.failed) return;

                }
                break;
            case 8:
                // org\\antlr\\v4\\parse\\ActionSplitter.g:1:123: TEXT
                {
                    mTEXT(); if (state.failed) return;

                }
                break;

        }
    }

    // $ANTLR start synpred1_ActionSplitter
    public void synpred1_ActionSplitter_fragment()
    {
        // org\\antlr\\v4\\parse\\ActionSplitter.g:1:41: ( COMMENT )
        // org\\antlr\\v4\\parse\\ActionSplitter.g:1:41: COMMENT
        {
            mCOMMENT(); if (state.failed) return;

        }

    }
    // $ANTLR end synpred1_ActionSplitter

    // $ANTLR start synpred2_ActionSplitter
    public void synpred2_ActionSplitter_fragment()
    {
        // org\\antlr\\v4\\parse\\ActionSplitter.g:1:49: ( LINE_COMMENT )
        // org\\antlr\\v4\\parse\\ActionSplitter.g:1:49: LINE_COMMENT
        {
            mLINE_COMMENT(); if (state.failed) return;

        }

    }
    // $ANTLR end synpred2_ActionSplitter

    // $ANTLR start synpred3_ActionSplitter
    public void synpred3_ActionSplitter_fragment()
    {
        // org\\antlr\\v4\\parse\\ActionSplitter.g:1:62: ( SET_NONLOCAL_ATTR )
        // org\\antlr\\v4\\parse\\ActionSplitter.g:1:62: SET_NONLOCAL_ATTR
        {
            mSET_NONLOCAL_ATTR(); if (state.failed) return;

        }

    }
    // $ANTLR end synpred3_ActionSplitter

    // $ANTLR start synpred4_ActionSplitter
    public void synpred4_ActionSplitter_fragment()
    {
        // org\\antlr\\v4\\parse\\ActionSplitter.g:1:80: ( NONLOCAL_ATTR )
        // org\\antlr\\v4\\parse\\ActionSplitter.g:1:80: NONLOCAL_ATTR
        {
            mNONLOCAL_ATTR(); if (state.failed) return;

        }

    }
    // $ANTLR end synpred4_ActionSplitter

    // $ANTLR start synpred5_ActionSplitter
    public void synpred5_ActionSplitter_fragment()
    {
        // org\\antlr\\v4\\parse\\ActionSplitter.g:1:94: ( QUALIFIED_ATTR )
        // org\\antlr\\v4\\parse\\ActionSplitter.g:1:94: QUALIFIED_ATTR
        {
            mQUALIFIED_ATTR(); if (state.failed) return;

        }

    }
    // $ANTLR end synpred5_ActionSplitter

    // $ANTLR start synpred6_ActionSplitter
    public void synpred6_ActionSplitter_fragment()
    {
        // org\\antlr\\v4\\parse\\ActionSplitter.g:1:109: ( SET_ATTR )
        // org\\antlr\\v4\\parse\\ActionSplitter.g:1:109: SET_ATTR
        {
            mSET_ATTR(); if (state.failed) return;

        }

    }
    // $ANTLR end synpred6_ActionSplitter

    // $ANTLR start synpred7_ActionSplitter
    public void synpred7_ActionSplitter_fragment()
    {
        // org\\antlr\\v4\\parse\\ActionSplitter.g:1:118: ( ATTR )
        // org\\antlr\\v4\\parse\\ActionSplitter.g:1:118: ATTR
        {
            mATTR(); if (state.failed) return;

        }

    }
    // $ANTLR end synpred7_ActionSplitter

    public bool synpred2_ActionSplitter()
    {
        state.backtracking++;
        int start = input.mark();
        try
        {
            synpred2_ActionSplitter_fragment(); // can never throw exception
        }
        catch (RecognitionException re)
        {
            Console.Error.WriteLine("impossible: " + re);
        }
        bool success = !state.failed;
        input.rewind(start);
        state.backtracking--;
        state.failed = false;
        return success;
    }
    public bool synpred7_ActionSplitter()
    {
        state.backtracking++;
        int start = input.mark();
        try
        {
            synpred7_ActionSplitter_fragment(); // can never throw exception
        }
        catch (RecognitionException re)
        {
            Console.Error.WriteLine("impossible: " + re);
        }
        bool success = !state.failed;
        input.rewind(start);
        state.backtracking--;
        state.failed = false;
        return success;
    }
    public bool synpred1_ActionSplitter()
    {
        state.backtracking++;
        int start = input.mark();
        try
        {
            synpred1_ActionSplitter_fragment(); // can never throw exception
        }
        catch (RecognitionException re)
        {
            Console.Error.WriteLine("impossible: " + re);
        }
        bool success = !state.failed;
        input.rewind(start);
        state.backtracking--;
        state.failed = false;
        return success;
    }
    public bool synpred6_ActionSplitter()
    {
        state.backtracking++;
        int start = input.mark();
        try
        {
            synpred6_ActionSplitter_fragment(); // can never throw exception
        }
        catch (RecognitionException re)
        {
            Console.Error.WriteLine("impossible: " + re);
        }
        bool success = !state.failed;
        input.rewind(start);
        state.backtracking--;
        state.failed = false;
        return success;
    }
    public bool synpred3_ActionSplitter()
    {
        state.backtracking++;
        int start = input.mark();
        try
        {
            synpred3_ActionSplitter_fragment(); // can never throw exception
        }
        catch (RecognitionException re)
        {
            Console.Error.WriteLine("impossible: " + re);
        }
        bool success = !state.failed;
        input.rewind(start);
        state.backtracking--;
        state.failed = false;
        return success;
    }
    public bool synpred4_ActionSplitter()
    {
        state.backtracking++;
        int start = input.mark();
        try
        {
            synpred4_ActionSplitter_fragment(); // can never throw exception
        }
        catch (RecognitionException re)
        {
            Console.Error.WriteLine("impossible: " + re);
        }
        bool success = !state.failed;
        input.rewind(start);
        state.backtracking--;
        state.failed = false;
        return success;
    }
    public bool synpred5_ActionSplitter()
    {
        state.backtracking++;
        int start = input.mark();
        try
        {
            synpred5_ActionSplitter_fragment(); // can never throw exception
        }
        catch (RecognitionException re)
        {
            Console.Error.WriteLine("impossible: " + re);
        }
        bool success = !state.failed;
        input.rewind(start);
        state.backtracking--;
        state.failed = false;
        return success;
    }

}
