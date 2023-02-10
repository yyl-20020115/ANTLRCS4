/*
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
using org.antlr.v4.runtime;

namespace org.antlr.runtime;

/** A lexer is recognizer that draws input symbols from a character stream.
 *  lexer grammars result in a subclass of this object. A Lexer object
 *  uses simplified match() and error recovery mechanisms in the interest
 *  of speed.
 */
public abstract class Lexer : BaseRecognizer, TokenSource
{
    /** Where is the lexer drawing characters from? */
    protected CharStream input;

    public Lexer() { }

    public Lexer(CharStream input) => this.input = input;

    public Lexer(CharStream input, RecognizerSharedState state)
        : base(state) => this.input = input;

    public override void Reset()
    {
        base.Reset(); // reset all recognizer state variables
                      // wack Lexer state variables
        input?.Seek(0); // rewind the input
        if (state == null) return; // no shared state work to do
        state.token = null;
        state.type = Token.INVALID_TOKEN_TYPE;
        state.channel = Token.DEFAULT_CHANNEL;
        state.tokenStartCharIndex = -1;
        state.tokenStartCharPositionInLine = -1;
        state.tokenStartLine = -1;
        state.text = null;
    }

    /** Return a token from this source; i.e., match a token on the char
     *  stream.
     */
    public virtual Token NextToken()
    {
        while (true)
        {
            state.token = null;
            state.channel = Token.DEFAULT_CHANNEL;
            state.tokenStartCharIndex = input.Index;
            state.tokenStartCharPositionInLine = input.CharPositionInLine;
            state.tokenStartLine = input.Line;
            state.text = null;
            if (input.LA(1) == CharStream.EOF)
            {
                return GetEOFToken();
            }
            try
            {
                MTokens();
                if (state.token == null)
                {
                    Emit();
                }
                else if (state.token == Token.SKIP_TOKEN)
                {
                    continue;
                }
                return state.token;
            }
            catch (MismatchedRangeException re)
            {
                ReportError(re);
                // matchRange() routine has already called recover()
            }
            catch (MismatchedTokenException re)
            {
                ReportError(re);
                // match() routine has already called recover()
            }
            catch (RecognitionException re)
            {
                ReportError(re);
                Recover(re); // throw out current char and try again
            }
        }
    }

    /** Returns the EOF token (default), if you need
     *  to return a custom token instead override this method.
     */
    public Token GetEOFToken()
    {
        var eof = new CommonToken(input, Token.EOF,
                                    Token.DEFAULT_CHANNEL,
                                    input.Index, input.Index)
        {
            Line = Line,
            CharPositionInLine = CharPositionInLine
        };
        return eof;
    }

    /** Instruct the lexer to skip creating a token for current lexer rule
     *  and look for another token.  nextToken() knows to keep looking when
     *  a lexer rule finishes with token set to SKIP_TOKEN.  Recall that
     *  if token==null at end of any token rule, it creates one for you
     *  and emits it.
     */
    public void Skip()
    {
        state.token = Token.SKIP_TOKEN;
    }

    /** This is the lexer entry point that sets instance var 'token' */
    public abstract void MTokens();

    /** Set the char stream and reset the lexer */
    public void SetCharStream(CharStream input)
    {
        this.input = null;
        Reset();
        this.input = input;
    }

    public CharStream GetCharStream() => this.input;
    public override string SourceName => input.SourceName;

    /** Currently does not support multiple emits per nextToken invocation
     *  for efficiency reasons.  Subclass and override this method and
     *  nextToken (to push tokens into a list and pull from that list rather
     *  than a single variable as this implementation does).
     */
    public void Emit(Token token) => state.token = token;

    /** The standard method called to automatically emit a token at the
     *  outermost lexical rule.  The token object should point into the
     *  char buffer start..stop.  If there is a text override in 'text',
     *  use that to set the token's text.  Override this method to emit
     *  custom Token objects.
     *
     *  If you are building trees, then you should also override
     *  Parser or TreeParser.getMissingSymbol().
     */
    public Token Emit()
    {
        var t = new CommonToken(input, state.type, state.channel, state.tokenStartCharIndex, CharIndex - 1)
        {
            Line = state.tokenStartLine,
            Text = state.text,
            CharPositionInLine = state.tokenStartCharPositionInLine
        };
        Emit(t);
        return t;
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
    public void MatchAny() => input.Consume();

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

    public int Line => input.Line;

    public int CharPositionInLine => input.CharPositionInLine;

    /** What is the index of the current character of lookahead? */
    public int CharIndex => input.Index;

    /** Return the text matched so far for the current token or any
     *  text override.
     */
    public string GetText() => state.text ?? input.Substring(state.tokenStartCharIndex, CharIndex - 1);

    /** Set the complete text of this token; it wipes any previous
     *  changes to the text.
     */
    public void SetText(string text) => state.text = text;

    public override void ReportError(RecognitionException e) =>
        /** TODO: not thought about recovery in lexer yet.
            *
            // if we've already reported an error and have not matched a token
            // yet successfully, don't report any errors.
            if ( errorRecovery ) {
            //System.err.print("[SPURIOUS] ");
            return;
            }
            errorRecovery = true;
            */

        DisplayRecognitionError(this.TokenNames, e);

    public override string GetErrorMessage(RecognitionException e, string[] tokenNames)
    {
        string msg;
        if (e is MismatchedTokenException mte) {
            msg = "mismatched character " + GetCharErrorDisplay(e.c) + " expecting " + GetCharErrorDisplay(mte.expecting);
        }

        else if (e is NoViableAltException nvae) {
            // for development, can add "decision=<<"+nvae.grammarDecisionDescription+">>"
            // and "(decision="+nvae.decisionNumber+") and
            // "state "+nvae.stateNumber
            msg = "no viable alternative at character " + GetCharErrorDisplay(e.c);
        }

        else if (e is EarlyExitException eee) {
            // for development, can add "(decision="+eee.decisionNumber+")"
            msg = "required (...)+ loop did not match anything at character " + GetCharErrorDisplay(e.c);
        }

        else if (e is MismatchedNotSetException mse) {
            msg = "mismatched character " + GetCharErrorDisplay(e.c) + " expecting set " + mse.expecting;
        }

        else if (e is MismatchedSetException mse1) {
            msg = "mismatched character " + GetCharErrorDisplay(e.c) + " expecting set " + mse1.expecting;
        }

        else if (e is MismatchedRangeException mre) {
            msg = "mismatched character " + GetCharErrorDisplay(e.c) + " expecting set " +
                  GetCharErrorDisplay(mre.a) + ".." + GetCharErrorDisplay(mre.b);
        }

        else
        {
            msg = base.GetErrorMessage(e, tokenNames);
        }
        return msg;
    }

    public string GetCharErrorDisplay(int c)
    {
        var s = c switch
        {
            Token.EOF => "<EOF>",
            '\n' => "\\n",
            '\t' => "\\t",
            '\r' => "\\r",
            _ => ((char)c).ToString(),
        };
        return "'" + s + "'";
    }

    /** Lexers can normally match any char in it's vocabulary after matching
     *  a token, so do the easy thing and just kill a character and hope
     *  it all works out.  You can instead use the rule invocation stack
     *  to do sophisticated error recovery if you are in a fragment rule.
     */
    public void Recover(RecognitionException re)
    {
        //System.out.println("consuming char "+(char)input.LA(1)+" during recovery");
        //re.printStackTrace();
        input.Consume();
    }

    public void TraceIn(string ruleName, int ruleIndex)
    {
        var inputSymbol = ((char)input.LT(1)) + " line=" + Line + ":" + CharPositionInLine;
        base.TraceIn(ruleName, ruleIndex, inputSymbol);
    }

    public void TraceOut(string ruleName, int ruleIndex)
    {
        var inputSymbol = ((char)input.LT(1)) + " line=" + Line + ":" + CharPositionInLine;
        base.TraceOut(ruleName, ruleIndex, inputSymbol);
    }

    public CharStream CharInputStream => throw new NotImplementedException();

    public TokenFactory TokenFactory 
    { 
        get; set; 
    }
}
