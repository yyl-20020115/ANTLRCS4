﻿/*
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
using System.Text;

namespace org.antlr.runtime.tree;

public class TreePatternLexer
{
    public const int EOF = -1;
    public const int BEGIN = 1;
    public const int END = 2;
    public const int ID = 3;
    public const int ARG = 4;
    public const int PERCENT = 5;
    public const int COLON = 6;
    public const int DOT = 7;

    /** The tree pattern to lex like "(A B C)" */
    protected string pattern;

    /** Index into input string */
    protected int p = -1;

    /** Current char */
    protected int c;

    /** How long is the pattern in char? */
    protected int n;

    /** Set when token type is ID or ARG (name mimics Java's StreamTokenizer) */
    public StringBuilder sval = new StringBuilder();

    public bool error = false;

    public TreePatternLexer(string pattern)
    {
        this.pattern = pattern;
        this.n = pattern.Length;
        consume();
    }

    public int nextToken()
    {
        sval.Length=0;
        // reset, but reuse buffer
        while (c != EOF)
        {
            if (c == ' ' || c == '\n' || c == '\r' || c == '\t')
            {
                consume();
                continue;
            }
            if ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || c == '_')
            {
                sval.Append((char)c);
                consume();
                while ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') ||
                        (c >= '0' && c <= '9') || c == '_')
                {
                    sval.Append((char)c);
                    consume();
                }
                return ID;
            }
            if (c == '(')
            {
                consume();
                return BEGIN;
            }
            if (c == ')')
            {
                consume();
                return END;
            }
            if (c == '%')
            {
                consume();
                return PERCENT;
            }
            if (c == ':')
            {
                consume();
                return COLON;
            }
            if (c == '.')
            {
                consume();
                return DOT;
            }
            if (c == '[')
            { // grab [x] as a string, returning x
                consume();
                while (c != ']')
                {
                    if (c == '\\')
                    {
                        consume();
                        if (c != ']')
                        {
                            sval.Append('\\');
                        }
                        sval.Append((char)c);
                    }
                    else
                    {
                        sval.Append((char)c);
                    }
                    consume();
                }
                consume();
                return ARG;
            }
            consume();
            error = true;
            return EOF;
        }
        return EOF;
    }

    protected void consume()
    {
        p++;
        if (p >= n)
        {
            c = EOF;
        }
        else
        {
            c = pattern[(p)];
        }
    }
}
