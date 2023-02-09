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
using org.antlr.v4.runtime.misc;

namespace org.antlr.runtime;

/** A parser for TokenStreams.  "parser grammars" result in a subclass
 *  of this.
 */
public class Parser : BaseRecognizer
{

    public TokenStream input;

    public Parser(TokenStream input)
        : base()
    {
        // highlight that we go to super to set state object
        SetTokenStream(input);
    }

    public Parser(TokenStream input, RecognizerSharedState state)
        : base(state)
    {
        ; // share the state object with another parser
        this.input = input;
    }

    //@Override
    public override void Reset()
    {
        base.Reset(); // reset all recognizer state variables
        input?.Seek(0); // rewind the input
    }

    //@Override
    protected object GetCurrentInputSymbol(IntStream input)
    {
        return ((TokenStream)input).LT(1);
    }

    //@Override
    protected object GetMissingSymbol(IntStream input,
                                      RecognitionException e,
                                      int expectedTokenType,
                                      BitSet follow)
    {
        string tokenText;
        if (expectedTokenType == Token.EOF) tokenText = "<missing EOF>";
        else tokenText = "<missing " + TokenNames[expectedTokenType] + ">";
        var t = new CommonToken(expectedTokenType, tokenText);
        var current = ((TokenStream)input).LT(1);
        if (current.Type == Token.EOF)
        {
            current = ((TokenStream)input).LT(-1);
        }
        t.line = current.Line;
        t.charPositionInLine = current.CharPositionInLine;
        t.channel = DEFAULT_TOKEN_CHANNEL;
        t.        InputStream = current.InputStream;
        return t;
    }

    /** Set the token stream and reset the parser */
    public void SetTokenStream(TokenStream input)
    {
        this.input = null;
        Reset();
        this.input = input;
    }

    public TokenStream GetTokenStream()
    {
        return input;
    }

    //@Override
    public override string SourceName => input.SourceName;

    public void TraceIn(string ruleName, int ruleIndex)
    {
        base.TraceIn(ruleName, ruleIndex, input.LT(1));
    }

    public void TraceOut(string ruleName, int ruleIndex)
    {
        base.TraceOut(ruleName, ruleIndex, input.LT(1));
    }
}
