/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime.atn;
using System.Text;

namespace org.antlr.v4.runtime.misc;

public static class RuntimeUtils
{
    public static int ObjectsHash(params object[] array)
    {
        if (array == null)
            return 0;

        int result = 1;

        foreach (Object element in array)
            result = 31 * result + (element == null ? 0 : element.GetHashCode());

        return result;
    }
    public static bool ObjectsEquals(object a, object b)
    {
        return (a == b) || (a != null && a.Equals(b));
    }
    public static short[] Convert(char[] chars)
    {
        short[] shorts = new short[chars.Length];
        for (int i = 0; i < shorts.Length; i++)
        {
            int c = chars[i];
            shorts[i] = (short)c;
        }
        return shorts;
    }


    // Seriously: why isn't this built in to java? ugh!
    public static String join<T>(IEnumerator<T> iter, String separator)
    {
        var list = new List<T>();
        while (iter.MoveNext())
        {
            list.Add(iter.Current);
        }

        return string.Join(separator, list);
    }
    public static String join<T>(IEnumerable<T> values, String separator)
    {
        return string.Join(separator, values.ToArray());
    }

    public static String join<T>(T[] array, String separator)
    {
        StringBuilder builder = new StringBuilder();
        for (int i = 0; i < array.Length; i++)
        {
            builder.Append(array[i]);
            if (i < array.Length - 1)
            {
                builder.Append(separator);
            }
        }

        return builder.ToString();
    }

    public static int numNonnull(Object[] data)
    {
        int n = 0;
        if (data == null) return n;
        foreach (Object o in data)
        {
            if (o != null) n++;
        }
        return n;
    }

    public static void removeAllElements<T>(ICollection<T> data, T value)
    {
        if (data == null) return;
        while (data.Contains(value)) data.Remove(value);
    }

    public static String escapeWhitespace(String s, bool escapeSpaces)
    {
        StringBuilder buf = new StringBuilder();
        foreach (char c in s.ToCharArray())
        {
            if (c == ' ' && escapeSpaces) buf.Append('\u00B7');
            else if (c == '\t') buf.Append("\\t");
            else if (c == '\n') buf.Append("\\n");
            else if (c == '\r') buf.Append("\\r");
            else buf.Append(c);
        }
        return buf.ToString();
    }

    public static void writeFile(String fileName, String content)
    {
        writeFile(fileName, content, null);
    }

    public static void writeFile(String fileName, String content, String encoding)
    {
        File.WriteAllText(fileName, content, Encoding.GetEncoding(encoding));
    }


    public static char[] readFile(String fileName)
    {
        return readFile(fileName, Encoding.Default);
    }


    public static char[] readFile(String fileName, Encoding encoding)
    {
        return File.ReadAllText(fileName, encoding).ToArray();
    }

    /** Convert array of strings to string&rarr;index map. Useful for
	 *  converting rulenames to name&rarr;ruleindex map.
	 */
    public static Dictionary<String, int> toMap(String[] keys)
    {
        Dictionary<String, int> m = new();
        for (int i = 0; i < keys.Length; i++)
        {
            m[keys[i]] = i;
        }
        return m;
    }

    public static char[] toCharArray(IntegerList data)
    {
        if (data == null) return null;
        return data.toCharArray();
    }

    public static IntervalSet toSet(BitSet bits)
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
    public static String expandTabs(String s, int tabSize)
    {
        if (s == null) return null;
        StringBuilder buf = new StringBuilder();
        int col = 0;
        for (int i = 0; i < s.Length; i++)
        {
            char c = s[i];
            switch (c)
            {
                case '\n':
                    col = 0;
                    buf.Append(c);
                    break;
                case '\t':
                    int n = tabSize - col % tabSize;
                    col += n;
                    buf.Append(spaces(n));
                    break;
                default:
                    col++;
                    buf.Append(c);
                    break;
            }
        }
        return buf.ToString();
    }

    /** @since 4.6 */
    public static String spaces(int n)
    {
        return sequence(n, " ");
    }

    /** @since 4.6 */
    public static String newlines(int n)
    {
        return sequence(n, "\n");
    }

    /** @since 4.6 */
    public static String sequence(int n, String s)
    {
        StringBuilder buf = new StringBuilder();
        for (int sp = 1; sp <= n; sp++) buf.Append(s);
        return buf.ToString();
    }

    /** @since 4.6 */
    public static int count(String s, char x)
    {
        int n = 0;
        for (int i = 0; i < s.Length; i++)
        {
            if (s[i] == x)
            {
                n++;
            }
        }
        return n;
    }
}
