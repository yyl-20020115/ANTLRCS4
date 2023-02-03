﻿/*
 * [The "BSD license"]
 * Copyright (c) 2011 Terence Parr
 * All rights reserved.
 *
 * Conversion to C#:
 * Copyright (c) 2011 Sam Harwell, Tunnel Vision Laboratories, LLC
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions
 * are met:
 * 1. Redistributions of source code must retain the above copyright
 *    notice, this list of conditions and the following disclaimer.
 * 2. Redistributions in binary form must reproduce the above copyright
 *    notice, this list of conditions and the following disclaimer in the
 *    documentation and/or other materials provided with the distribution.
 * 3. The name of the author may not be used to endorse or promote products
 *    derived from this software without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE AUTHOR ``AS IS'' AND ANY EXPRESS OR
 * IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
 * OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.
 * IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY DIRECT, INDIRECT,
 * INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT
 * NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
 * DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
 * THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
 * THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */

namespace Antlr4.StringTemplate.Misc;

using StringBuilder = System.Text.StringBuilder;

public static class Utility
{
    public static string Strip(string s, int n) => s.Substring(n, s.Length - (2 * n));

    // strip newline from front but just one
    public static string TrimOneStartingNewline(string s)
    {
        if (s.StartsWith("\r\n"))
            s = s[2..];
        else if (s.StartsWith("\n"))
            s = s[1..];
        return s;
    }

    // strip newline from end but just one
    public static string TrimOneTrailingNewline(string s)
    {
        if (s.EndsWith("\r\n"))
            s = s[..^2];
        else if (s.EndsWith("\n"))
            s = s[..^1];
        return s;
    }

    public static string GetParent(string name)
    {
        if (name == null)
            return null;

        int lastSlash = name.LastIndexOf('/');
        return lastSlash == 0 ? "/" : lastSlash > 0 ? name[..lastSlash] : string.Empty;
    }

    public static string GetPrefix(string name)
    {
        if (name == null)
            return "/";

        var parent = GetParent(name);
        var prefix = parent;
        if (!parent.EndsWith("/"))
            prefix += '/';

        return prefix;
    }

    public static string ReplaceEscapes(string s)
    {
        s = s.Replace("\n", "\\\\n");
        s = s.Replace("\r", "\\\\r");
        s = s.Replace("\t", "\\\\t");
        return s;
    }

    /** Replace >\> with >> in s. Replace \>> unless prefix of \>>> with >>.
     *  Do NOT replace if it's &lt;\\&gt;
     */
    public static string ReplaceEscapedRightAngle(string s)
    {
        var buffer = new StringBuilder();
        int i = 0;
        while (i < s.Length)
        {
            char c = s[i];
            if (c == '<' && s[i..].StartsWith("<\\\\>"))
            {
                buffer.Append("<\\\\>");
                i += "<\\\\>".Length;
                continue;
            }

            if (c == '>' && s.Substring(i).StartsWith(">\\>"))
            {
                buffer.Append(">>");
                i += ">\\>".Length;
                continue;
            }

            if (c == '\\' && s.Substring(i).StartsWith("\\>>") &&
                !s.Substring(i).StartsWith("\\>>>"))
            {
                buffer.Append(">>");
                i += "\\>>".Length;
                continue;
            }

            buffer.Append(c);
            i++;
        }

        return buffer.ToString();
    }

    /** Given index into string, compute the line and char position in line */
    public static Coordinate GetLineCharPosition(string s, int index)
    {
        int line = 1;
        int charPos = 0;
        int p = 0;
        while (p < index)
        {
            // don't care about s[index] itself; count before
            if (s[p] == '\n')
            {
                line++;
                charPos = 0;
            }
            else
            {
                charPos++;
            }

            p++;
        }

        return new Coordinate(line, charPos);
    }
}
