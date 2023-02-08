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
using org.antlr.v4.runtime.tree;

namespace org.antlr.runtime.tree;


/** A node representing erroneous token range in token stream */
public class CommonErrorNode : CommonTree
{

    public IntStream input;
    public Token start;
    public Token stop;
    public RecognitionException trappedException;

    public CommonErrorNode(TokenStream input, Token start, Token stop,
                           RecognitionException e)
    {
        //System.out.println("start: "+start+", stop: "+stop);
        if (stop == null ||
             (stop.TokenIndex < start.TokenIndex &&
              stop.              Type != Token.EOF))
        {
            // sometimes resync does not consume a token (when LT(1) is
            // in follow set.  So, stop will be 1 to left to start. adjust.
            // Also handle case where start is the first token and no token
            // is consumed during recovery; LT(-1) will return null.
            stop = start;
        }
        this.input = input;
        this.start = start;
        this.stop = stop;
        this.trappedException = e;
    }

    //@Override
    public bool IsNil => false;

    //@Override
    public int Type => Token.INVALID_TOKEN_TYPE;

    //@Override
    public string Text
    {
        get
        {
            String badText;
            if (start is Token)
            {
                int i = start.TokenIndex;
                int j = stop.TokenIndex;
                if (stop.Type == Token.EOF)
                {
                    j = ((TokenStream)input).Count;
                }
                badText = ((TokenStream)input).ToString(i, j);
            }

            else if (start is Tree)
            {
                badText = ((TreeNodeStream)input).toString(start, stop);
            }
            else
            {
                // people should subclass if they alter the tree type so this
                // next one is for sure correct.
                badText = "<unknown>";
            }
            return badText;
        }
    }

    //@Override
    public override string ToString()
    {
        if (trappedException is MissingTokenException ) {
            return "<missing type: " +
                   ((MissingTokenException)trappedException).                   MissingType +
                   ">";
        }

        else if (trappedException is UnwantedTokenException ) {
            return "<extraneous: " +
                   ((UnwantedTokenException)trappedException).                   UnexpectedToken +
                   ", resync=" + Text + ">";
        }

        else if (trappedException is MismatchedTokenException ) {
            return "<mismatched token: " + trappedException.token + ", resync=" + Text + ">";
        }

        else if (trappedException is NoViableAltException ) {
            return "<unexpected: " + trappedException.token +
                   ", resync=" + Text + ">";
        }
        return "<error: " + Text + ">";
    }
}
