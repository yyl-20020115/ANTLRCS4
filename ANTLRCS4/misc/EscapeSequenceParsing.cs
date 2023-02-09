/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime.misc;
using org.antlr.v4.unicode;
using System.Text;

namespace org.antlr.v4.misc;

/**
 * Utility class to parse escapes like:
 *   \\n
 *   \\uABCD
 *   \\u{10ABCD}
 *   \\p{Foo}
 *   \\P{Bar}
 *   \\p{Baz=Blech}
 *   \\P{Baz=Blech}
 */
public abstract class EscapeSequenceParsing
{
    public class Result
    {
        public enum Type :int
        {
            INVALID,
            CODE_POINT,
            PROPERTY
        };

        public readonly Type type;
        public readonly int codePoint;
        public readonly IntervalSet propertyIntervalSet;
        public readonly int startOffset;
        public readonly int parseLength;

        public Result(Type type, int codePoint, IntervalSet propertyIntervalSet, int startOffset, int parseLength)
        {
            this.type = type;
            this.codePoint = codePoint;
            this.propertyIntervalSet = propertyIntervalSet;
            this.startOffset = startOffset;
            this.parseLength = parseLength;
        }

        public override string ToString() 
            => $"{base.ToString()} type={type} codePoint={codePoint} propertyIntervalSet={propertyIntervalSet} parseLength={parseLength}";

        public override bool Equals(object? other)
        {
            if (other is not Result that)
            {
                return false;
            }
            if (this == that)
            {
                return true;
            }
            return RuntimeUtils.ObjectsEquals(this.type, that.type) &&
                RuntimeUtils.ObjectsEquals(this.codePoint, that.codePoint) &&
                RuntimeUtils.ObjectsEquals(this.propertyIntervalSet, that.propertyIntervalSet) &&
                RuntimeUtils.ObjectsEquals(this.parseLength, that.parseLength);
        }
        public override int GetHashCode() 
            => RuntimeUtils.ObjectsHash(type, codePoint, propertyIntervalSet, parseLength);
    }

    /**
	 * Parses a single escape sequence starting at {@code startOff}.
	 *
	 * Returns a type of INVALID if no valid escape sequence was found, a Result otherwise.
	 */
    public static Result ParseEscape(string s, int startOff)
    {
        int offset = startOff;
        if (offset + 2 > s.Length || char.ConvertToUtf32(s, offset) != '\\')
        {
            return Invalid(startOff, s.Length - 1);
        }
        // Move past backslash
        offset++;
        int escaped = char.ConvertToUtf32(s, offset);// s.codePointAt(offset);
                                                     // Move past escaped code point
        offset += new Rune(escaped).Utf16SequenceLength;
        if (escaped == 'u')
        {
            // \\u{1} is the shortest we support
            if (offset + 3 > s.Length)
            {
                return Invalid(startOff, s.Length - 1);
            }
            int hexStartOffset;
            int hexEndOffset; // appears to be exclusive
            if (char.ConvertToUtf32(s, offset) == '{')
            {
                hexStartOffset = offset + 1;
                hexEndOffset = s.IndexOf('}', hexStartOffset);
                if (hexEndOffset == -1)
                {
                    return Invalid(startOff, s.Length - 1);
                }
                offset = hexEndOffset + 1;
            }
            else
            {
                if (offset + 4 > s.Length)
                {
                    return Invalid(startOff, s.Length - 1);
                }
                hexStartOffset = offset;
                hexEndOffset = offset + 4;
                offset = hexEndOffset;
            }
            int codePointValue = CharSupport.ParseHexValue(s, hexStartOffset, hexEndOffset);
            if (codePointValue == -1 || codePointValue > char.MaxValue)
            {
                return Invalid(startOff, startOff + 6 - 1);
            }
            return new Result(
                Result.Type.CODE_POINT,
                codePointValue,
                IntervalSet.EMPTY_SET,
                startOff,
                offset - startOff);
        }
        else if (escaped == 'p' || escaped == 'P')
        {
            // \p{L} is the shortest we support
            if (offset + 3 > s.Length)
            {
                return Invalid(startOff, s.Length - 1);
            }
            if (char.ConvertToUtf32(s, offset) != '{')
            {
                return Invalid(startOff, offset);
            }
            int openBraceOffset = offset;
            int closeBraceOffset = s.IndexOf('}', openBraceOffset);
            if (closeBraceOffset == -1)
            {
                return Invalid(startOff, s.Length - 1);
            }
            var propertyName = s[(openBraceOffset + 1)..closeBraceOffset];
            var propertyIntervalSet = UnicodeData.getPropertyCodePoints(propertyName);
            if (propertyIntervalSet == null || propertyIntervalSet.IsNil)
            {
                return Invalid(startOff, closeBraceOffset);
            }
            offset = closeBraceOffset + 1;
            if (escaped == 'P')
            {
                propertyIntervalSet = propertyIntervalSet.Complement(IntervalSet.COMPLETE_CHAR_SET);
            }
            return new Result(
                Result.Type.PROPERTY,
                -1,
                propertyIntervalSet,
                startOff,
                offset - startOff);
        }
        else if (escaped < CharSupport.ANTLRLiteralEscapedCharValue.Length)
        {
            int codePoint = CharSupport.ANTLRLiteralEscapedCharValue[escaped];
            if (codePoint == 0)
            {
                if (escaped != ']' && escaped != '-')
                { // escape ']' and '-' only in char sets.
                    return Invalid(startOff, startOff + 1);
                }
                else
                {
                    codePoint = escaped;
                }
            }
            return new Result(
                Result.Type.CODE_POINT,
                codePoint,
                IntervalSet.EMPTY_SET,
                startOff,
                offset - startOff);
        }
        else
        {
            return Invalid(startOff, s.Length - 1);
        }
    }

    private static Result Invalid(int start, int stop) => new(
            Result.Type.INVALID,
            0,
            IntervalSet.EMPTY_SET,
            start,
            stop - start + 1);
}
