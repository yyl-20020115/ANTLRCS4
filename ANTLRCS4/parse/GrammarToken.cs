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

    //@Override
    public override int GetCharPositionInLine()
    {
        if (originalTokenIndex >= 0) return g.originalTokenStream.get(originalTokenIndex).getCharPositionInLine();
        return base.getCharPositionInLine();
    }

    //@Override
    public int getLine()
    {
        if (originalTokenIndex >= 0) return g.originalTokenStream.get(originalTokenIndex).getLine();
        return base.getLine();
    }

    //@Override
    public int getTokenIndex()
    {
        return originalTokenIndex;
    }

    //@Override
    public int getStartIndex()
    {
        if (originalTokenIndex >= 0)
        {
            return ((CommonToken)g.originalTokenStream.get(originalTokenIndex)).getStartIndex();
        }
        return base.getStartIndex();
    }

    //@Override
    public int getStopIndex()
    {
        int n = base.getStopIndex() - base.getStartIndex() + 1;
        return getStartIndex() + n - 1;
    }

    //@Override
    public String toString()
    {
        String channelStr = "";
        if (channel > 0)
        {
            channelStr = ",channel=" + channel;
        }
        String txt = getText();
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
        return "[@" + getTokenIndex() + "," + getStartIndex() + ":" + getStopIndex() +
               "='" + txt + "',<" + getType() + ">" + channelStr + "," + getLine() + ":" + getCharPositionInLine() + "]";
    }
}
