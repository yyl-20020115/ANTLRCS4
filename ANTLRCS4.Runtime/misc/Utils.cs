/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using System.Text;

namespace org.antlr.v4.runtime.misc;

public static class RuntimeUtils
{
    public static bool DoEquals(object? a, object? b)
    => (a == b) || (a != null && a.Equals(b));

    public static void RequireNonNull(object? o)
    {
        if (o == null) throw new ArgumentNullException(nameof(o));
    }

    public static int BitCount(long v)
    {
        v -= ((v >>> 1) & 0x5555555555555555L);
        v = (v & 0x3333333333333333L) + ((v >>> 2) & 0x3333333333333333L);
        v = (v + (v >>> 4)) & 0x0f0f0f0f0f0f0f0fL;
        v += (v >>> 8);
        v += (v >>> 16);
        v += (v >>> 32);
        return (int)v & 0x7f;
    }

    public static int NumberOfLeadingZeros(long v)
    {
        int x = (int)(v >>> 32);
        return x == 0 ? 32 + NumberOfLeadingZeros((int)v)
                : NumberOfLeadingZeros(x);
    }

    public static int NumberOfTrailingZeros(long v)
    {
        int x = (int)v;
        return x == 0 ? 32 + NumberOfTrailingZeros((int)(v >>> 32))
                : NumberOfTrailingZeros(x);
    }

    public static int BitCount(int v)
    {
        v -= ((v >>> 1) & 0x55555555);
        v = (v & 0x33333333) + ((v >>> 2) & 0x33333333);
        v = (v + (v >>> 4)) & 0x0f0f0f0f;
        v += (v >>> 8);
        v += (v >>> 16);
        return v & 0x3f;
    }

    public static int NumberOfLeadingZeros(int v)
    {
        if (v <= 0)
            return v == 0 ? 32 : 0;
        int n = 31;
        if (v >= 1 << 16) { n -= 16; v >>>= 16; }
        if (v >= 1 << 8) { n -= 8; v >>>= 8; }
        if (v >= 1 << 4) { n -= 4; v >>>= 4; }
        if (v >= 1 << 2) { n -= 2; v >>>= 2; }
        return n - (v >>> 1);
    }

    public static int NumberOfTrailingZeros(int v)
    {
        v = ~v & (v - 1);
        if (v <= 0) return v & 32;
        int n = 1;
        if (v > 1 << 16) { n += 16; v >>>= 16; }
        if (v > 1 << 8) { n += 8; v >>>= 8; }
        if (v > 1 << 4) { n += 4; v >>>= 4; }
        if (v > 1 << 2) { n += 2; v >>>= 2; }
        return n + (v >>> 1);
    }

    public static int ObjectsHash(params object[] array)
    {
        if (array == null)
            return 0;

        int result = 1;
        foreach (var element in array)
            result = 31 * result + (element == null ? 0 : element.GetHashCode());
        return result;
    }
    public static bool ObjectsEquals(object a, object b) => (a == b) || (a != null && a.Equals(b));
    public static short[] Convert(char[] chars)
    {
        var shorts = new short[chars.Length];
        for (int i = 0; i < shorts.Length; i++)
        {
            int c = chars[i];
            shorts[i] = (short)c;
        }
        return shorts;
    }


    // Seriously: why isn't this built in to java? ugh!
    public static string Join<T>(IEnumerator<T> iter, string separator)
    {
        var list = new List<T>();
        while (iter.MoveNext())
            list.Add(iter.Current);

        return string.Join(separator, list);
    }
    public static string Join<T>(IEnumerable<T> values, string separator) 
        => string.Join(separator, values.ToArray());

    public static string Join<T>(T[] array, String separator)
    {
        var builder = new StringBuilder();
        for (int i = 0; i < array.Length; i++)
        {
            builder.Append(array[i]);
            if (i < array.Length - 1)
                builder.Append(separator);
        }

        return builder.ToString();
    }

    public static int NumNonnull(object[] data)
    {
        int n = 0;
        if (data == null) return n;
        foreach (var o in data)
            if (o != null) n++;
        return n;
    }

    public static void RemoveAllElements<T>(ICollection<T> data, T value)
    {
        if (data == null) return;
        while (data.Contains(value)) data.Remove(value);
    }

    public static string EscapeWhitespace(string s, bool escapeSpaces)
    {
        var buffer = new StringBuilder();
        foreach (var c in s.ToCharArray())
        {
            if (c == ' ' && escapeSpaces) buffer.Append('\u00B7');
            else if (c == '\t') buffer.Append("\\t");
            else if (c == '\n') buffer.Append("\\n");
            else if (c == '\r') buffer.Append("\\r");
            else buffer.Append(c);
        }
        return buffer.ToString();
    }

    public static void WriteFile(string fileName, String content)
    {
        WriteFile(fileName, content, null);
    }

    public static void WriteFile(string fileName, String content, String encoding)
    {
        File.WriteAllText(fileName, content, Encoding.GetEncoding(encoding));
    }

    public static char[] ReadFile(string fileName)
    {
        return ReadFile(fileName, Encoding.Default);
    }


    public static char[] ReadFile(String fileName, Encoding encoding)
    {
        return File.ReadAllText(fileName, encoding).ToArray();
    }

    /** Convert array of strings to string&rarr;index map. Useful for
	 *  converting rulenames to name&rarr;ruleindex map.
	 */
    public static Dictionary<string, int> ToMap(string[] keys)
    {
        Dictionary<string, int> m = new();
        for (int i = 0; i < keys.Length; i++)
        {
            m[keys[i]] = i;
        }
        return m;
    }

    public static char[] ToCharArray(IntegerList data) => data?.ToCharArray();

    public static IntervalSet ToSet(BitSet bits)
    {
        var s = new IntervalSet();
        int i = bits.NextSetBit(0);
        while (i >= 0)
        {
            s.Add(i);
            i = bits.NextSetBit(i + 1);
        }
        return s;
    }

    /** @since 4.6 */
    public static string ExpandTabs(string s, int tabSize)
    {
        if (s == null) return null;
        var buffer = new StringBuilder();
        int col = 0;
        for (int i = 0; i < s.Length; i++)
        {
            char c = s[i];
            switch (c)
            {
                case '\n':
                    col = 0;
                    buffer.Append(c);
                    break;
                case '\t':
                    int n = tabSize - col % tabSize;
                    col += n;
                    buffer.Append(Spaces(n));
                    break;
                default:
                    col++;
                    buffer.Append(c);
                    break;
            }
        }
        return buffer.ToString();
    }

    /** @since 4.6 */
    public static string Spaces(int n)
    {
        return Sequence(n, " ");
    }

    /** @since 4.6 */
    public static string NewLines(int n)
    {
        return Sequence(n, "\n");
    }

    /** @since 4.6 */
    public static string Sequence(int n, String s)
    {
        var buffer = new StringBuilder();
        for (int sp = 1; sp <= n; sp++) buffer.Append(s);
        return buffer.ToString();
    }

    /** @since 4.6 */
    public static int Count(string s, char x)
    {
        int n = 0;
        for (int i = 0; i < s.Length; i++)
        {
            if (s[i] == x) n++;
        }
        return n;
    }
}
