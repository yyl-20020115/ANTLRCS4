/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime;
using org.antlr.v4.runtime.misc;
using System.Text;

namespace org.antlr.v4.misc;

/** */
public class CharSupport
{
    /** When converting ANTLR char and string literals, here is the
	 *  value set of escape chars.
	 */
    public readonly static int[] ANTLRLiteralEscapedCharValue = new int[255];

    /** Given a char, we need to be able to show as an ANTLR literal.
	 */
    public readonly static string[] ANTLRLiteralCharValueEscape = new string[255];

    static CharSupport()
    {
        ANTLRLiteralEscapedCharValue['n'] = '\n';
        ANTLRLiteralEscapedCharValue['r'] = '\r';
        ANTLRLiteralEscapedCharValue['t'] = '\t';
        ANTLRLiteralEscapedCharValue['b'] = '\b';
        ANTLRLiteralEscapedCharValue['f'] = '\f';
        ANTLRLiteralEscapedCharValue['\\'] = '\\';
        ANTLRLiteralCharValueEscape['\n'] = "\\n";
        ANTLRLiteralCharValueEscape['\r'] = "\\r";
        ANTLRLiteralCharValueEscape['\t'] = "\\t";
        ANTLRLiteralCharValueEscape['\b'] = "\\b";
        ANTLRLiteralCharValueEscape['\f'] = "\\f";
        ANTLRLiteralCharValueEscape['\\'] = "\\\\";
    }

    /** Return a string representing the escaped char for code c.  E.g., If c
	 *  has value 0x100, you will get "\\u0100".  ASCII gets the usual
	 *  char (non-hex) representation.  Non-ASCII characters are spit out
	 *  as \\uXXXX or \\u{XXXXXX} escapes.
	 */
    public static string GetANTLRCharLiteralForChar(int c)
    {
        string result;
        if (c < Lexer.MIN_CHAR_VALUE)
        {
            result = "<INVALID>";
        }
        else
        {
            var charValueEscape = c < ANTLRLiteralCharValueEscape.Length ? ANTLRLiteralCharValueEscape[c] : null;
            if (charValueEscape != null)
            {
                result = charValueEscape;
            }
            else if (/*char.UnicodeBlock.of((char) c) == char.UnicodeBlock.BASIC_LATIN*/
                (c >= 0x20 && c <= 0x7f) &&
                    !char.IsControl((char)c))
            {
                if (c == '\\')
                {
                    result = "\\\\";
                }
                else if (c == '\'')
                {
                    result = "\\'";
                }
                else
                {
                    result = c.ToString();// char.toString((char) c);
                }
            }
            else if (c <= 0xFFFF)
            {
                result = $"\\u{c:X4}";// string.format("\\u%04X", c);
            }
            else
            {
                result = $"\\u{c:X6}";// string.format("\\u{%06X}", c);
            }
        }
        return '\'' + result + '\'';
    }

    /** Given a literal like (the 3 char sequence with single quotes) 'a',
	 *  return the int value of 'a'. Convert escape sequences here also.
	 *  Return -1 if not single char.
	 */
    public static int GetCharValueFromGrammarCharLiteral(string literal)
    {
        return literal == null || literal.Length < 3
            ? -1
            : GetCharValueFromCharInGrammarLiteral(literal[1..^1]);
    }

    public static string GetStringFromGrammarStringLiteral(string literal)
    {
        var builder = new StringBuilder();
        int i = 1; // skip first quote
        int n = literal.Length - 1; // skip last quote
        while (i < n)
        { // scan all but last quote
            int end = i + 1;
            if (literal[(i)] == '\\')
            {
                end = i + 2;
                if (i + 1 < n && literal[(i + 1)] == 'u')
                {
                    if (i + 2 < n && literal[(i + 2)] == '{')
                    { // extended escape sequence
                        end = i + 3;
                        while (true)
                        {
                            if (end + 1 > n) return null; // invalid escape sequence.
                            char charAt = literal[(end++)];
                            if (charAt == '}')
                            {
                                break;
                            }
                            if (!char.IsDigit(charAt) && !(charAt >= 'a' && charAt <= 'f') && !(charAt >= 'A' && charAt <= 'F'))
                            {
                                return null; // invalid escape sequence.
                            }
                        }
                    }
                    else
                    {
                        for (end = i + 2; end < i + 6; end++)
                        {
                            if (end > n) return null; // invalid escape sequence.
                            char charAt = literal[(end)];
                            if (!char.IsDigit(charAt) && !(charAt >= 'a' && charAt <= 'f') && !(charAt >= 'A' && charAt <= 'F'))
                            {
                                return null; // invalid escape sequence.
                            }
                        }
                    }
                }
            }
            if (end > n) return null; // invalid escape sequence.
            var esc = literal[i..end];
            int c = GetCharValueFromCharInGrammarLiteral(esc);
            if (c == -1)
            {
                return null; // invalid escape sequence.
            }
            else builder.Append(char.ConvertFromUtf32(c));//.appendCodePoint(c);
            i = end;
        }
        return builder.ToString();
    }

    /** Given char x or \\t or \\u1234 return the char value;
	 *  Unnecessary escapes like '\{' yield -1.
	 */
    public static int GetCharValueFromCharInGrammarLiteral(string cstr)
    {
        switch (cstr.Length)
        {
            case 1:
                // 'x'
                return cstr[(0)]; // no escape char
            case 2:
                if (cstr[(0)] != '\\') return -1;
                // '\x'  (antlr lexer will catch invalid char)
                char escChar = cstr[(1)];
                if (escChar == '\'') return escChar; // escape quote only in string literals.
                int charVal = ANTLRLiteralEscapedCharValue[escChar];
                if (charVal == 0) return -1;
                return charVal;
            case 6:
                // '\\u1234' or '\\u{12}'
                if (!cstr.StartsWith("\\u")) return -1;
                int startOff;
                int endOff;
                if (cstr[(2)] == '{')
                {
                    startOff = 3;
                    endOff = cstr.IndexOf('}');
                }
                else
                {
                    startOff = 2;
                    endOff = cstr.Length;
                }
                return ParseHexValue(cstr, startOff, endOff);
            default:
                if (cstr.StartsWith("\\u{"))
                {
                    return ParseHexValue(cstr, 3, cstr.IndexOf('}'));
                }
                return -1;
        }
    }

    public static int ParseHexValue(string cstr, int startOff, int endOff)
    {
        if (startOff < 0 || endOff < 0)
        {
            return -1;
        }
        var unicodeChars = cstr[startOff..endOff];
        int result = -1;
        try
        {
            result = int.TryParse(unicodeChars, System.Globalization.NumberStyles.HexNumber, null, out var r)
                ? r : -1; //Integer.parseInt(unicodeChars, 16);
        }
        catch (Exception e)
        {
        }
        return result;
    }

    public static string Capitalize(string s) => s.Length>0 ? char.ToUpper(s[(0)]) + s[1..] : s;

    public static string GetIntervalSetEscapedString(IntervalSet intervalSet)
    {
        var buffer = new StringBuilder();
        bool first = true;
        foreach (var interval in intervalSet.GetIntervals())
        {
            if (!first) buffer.Append(" | ");
            first = false;
            buffer.Append(GetRangeEscapedString(interval.a, interval.b));
        }
        return buffer.ToString();
    }

    public static string GetRangeEscapedString(int codePointStart, int codePointEnd) 
        => codePointStart != codePointEnd
                ? GetANTLRCharLiteralForChar(codePointStart) + ".." + GetANTLRCharLiteralForChar(codePointEnd)
                : GetANTLRCharLiteralForChar(codePointStart);
}
