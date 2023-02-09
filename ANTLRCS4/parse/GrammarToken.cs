/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime;
using org.antlr.v4.tool;

namespace org.antlr.v4.parse;

/** A CommonToken that can also track it's original location,
 *  derived from options on the element ref like BEGIN&lt;line=34,...&gt;.
 */
public class GrammarToken : CommonToken
{
    public Grammar g;
    public int originalTokenIndex = -1;

    public GrammarToken(Grammar g, Token oldToken) : base(oldToken)
    {
        this.g = g;
    }


    public override int CharPositionInLine
    {
        get
        {
            if (originalTokenIndex >= 0) return g.originalTokenStream.Get(originalTokenIndex).CharPositionInLine;
            return base.CharPositionInLine;
        }
        set => base.CharPositionInLine = value;
    }

    public override int Line
    {
        get
        {
            if (originalTokenIndex >= 0) return g.originalTokenStream.Get(originalTokenIndex).Line;
            return base.Line;
        }
        set => base.Line = value;
    }

    public override int TokenIndex => originalTokenIndex;

    public override int StartIndex
    {
        get
        {
            if (originalTokenIndex >= 0)
            {
                return ((CommonToken)g.originalTokenStream.Get(originalTokenIndex)).StartIndex;
            }
            return base.StartIndex;
        }
    }

    public override int StopIndex
    {
        get
        {
            int n = base.StopIndex - base.StartIndex + 1;
            return StartIndex + n - 1;
        }
    }

    public override string ToString()
    {
        var channelStr = "";
        if (channel > 0)
        {
            channelStr = ",channel=" + channel;
        }
        var txt = Text;
        if (txt != null)
        {
            txt = txt.Replace("\n", "\\\\n");
            txt = txt.Replace("\r", "\\\\r");
            txt = txt.Replace("\t", "\\\\t");
        }
        else
        {
            txt = "<no text>";
        }
        return "[@" + TokenIndex + "," + StartIndex + ":" + StopIndex +
               "='" + txt + "',<" + Type + ">" + channelStr + "," + Line + ":" + CharPositionInLine + "]";
    }
}
