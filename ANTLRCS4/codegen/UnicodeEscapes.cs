/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using System.Text;

namespace org.antlr.v4.codegen;

/**
 * Utility class to escape Unicode code points using various
 * languages' syntax.
 */
public class UnicodeEscapes
{
    public static string EscapeCodePoint(int codePoint, string language)
    {
        var result = new StringBuilder();
        AppendEscapedCodePoint(result, codePoint, language);
        return result.ToString();
    }

    public static void AppendEscapedCodePoint(StringBuilder builder, int codePoint, string language)
    {
        switch (language)
        {
            case "CSharp":
            case "Python2":
            case "Python3":
            case "Cpp":
            case "Go":
            case "PHP":
                {
                    //string format = char.isSupplementaryCodePoint(codePoint) ? "\\U%08X" : "\\u%04X";
                    int n = new Rune(codePoint).Utf16SequenceLength;

                    builder.Append(n == 1 ? $"\\u{codePoint & 0xffff:X4}" : $"\\U{codePoint:X8}");
                    break;
                }
            case "Swift":
                builder.Append($"\\u{codePoint & 0xffff:X4}");
                break;
            case "Java":
            case "JavaScript":
            case "Dart":
            default:
                var s = char.ConvertFromUtf32(codePoint);
                if (s.Length > 1)
                {
                    // char is not an 'integral' type, so we have to explicitly convert
                    // to int before passing to the %X formatter or else it throws.
                    //sb.Append(string.format("\\u%04X", (int)char.highSurrogate(codePoint)));
                    //sb.Append(string.format("\\u%04X", (int)char.lowSurrogate(codePoint)));
                    builder.Append($"\\u{(int)s[0] & 0xffff:X4}");//highSurrogate
                    builder.Append($"\\u{(int)s[1] & 0xffff:X4}");//lowSurrogate
                }
                else
                {
                    builder.Append($"\\u{codePoint & 0xffff:X4}");
                }
                break;
        }
    }
}
