/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime.misc;
using org.antlr.v4.tool.ast;
using System.Text;

namespace org.antlr.v4.misc;

/** */
public static class Utils
{
    public const int INTEGER_POOL_MAX_VALUE = 1000;


    public interface Filter<T>
    {
        bool Select(T t);
    }

    public interface Func0<TResult>
    {
        TResult Exec();
    }

    public interface Func1<T1, TResult>
    {
        TResult Exec(T1 arg1);
    }

    public static string StripFileExtension(string name)
    {
        if (name == null) return null;
        int lastDot = name.LastIndexOf('.');
        if (lastDot < 0) return name;
        return name.Substring(0, lastDot);
    }

    public static string Join(object[] a, string separator)
    {
        var buffer = new StringBuilder();
        for (int i = 0; i < a.Length; i++)
        {
            var o = a[i];
            buffer.Append(o.ToString());
            if ((i + 1) < a.Length)
            {
                buffer.Append(separator);
            }
        }
        return buffer.ToString();
    }

    public static string SortLinesInString(string s)
    {
        var lines = s.Split("\n");
        Array.Sort(lines);
        var linesL = Arrays.AsList(lines);
        var buffer = new StringBuilder();
        foreach (var l in linesL)
        {
            buffer.Append(l);
            buffer.Append('\n');
        }
        return buffer.ToString();
    }

    public static List<string> NodesToStrings<T>(List<T> nodes) where T : GrammarAST
    {
        if (nodes == null) return null;
        List<string> a = new();
        foreach (var t in nodes) a.Add(t.getText());
        return a;
    }

    //	public static <T> List<T> list(T... values) {
    //		List<T> x = new ArrayList<T>(values.length);
    //		for (T v : values) {
    //			if ( v!=null ) x.add(v);
    //		}
    //		return x;
    //	}

    public static void WriteSerializedATNIntegerHistogram(string filename, IntegerList serializedATN)
    {
        Dictionary<int, int> histo = new();
        foreach (int i in serializedATN.ToArray())
        {
            if (histo.TryGetValue(i, out var v))
            {
                histo[i] = v + 1;
            }
            else
            {
                histo.Add(i, 1);
            }
        }
        Dictionary<int, int> sorted = new(histo);

        string output = "";
        output += "value,count\n";
        foreach (int key in sorted.Keys)
        {
            output += key + "," + sorted[(key)] + "\n";
        }
        try
        {
            File.WriteAllText((filename), output, Encoding.UTF8);
        }
        catch (IOException ioe)
        {
            Console.Error.WriteLine(ioe);
        }
    }

    public static string Capitalize(string s) => s.Length > 0 ? char.ToUpper(s[0]) + s[1..] : s;

    public static string Decapitalize(string s) => s.Length > 0 ? char.ToLower(s[0]) + s[1..] : s;

    /** apply methodName to list and return list of results. method has
	 *  no args.  This pulls data out of a list essentially.
	 */
    public static List<To> Select<From, To>(List<From> list, Func1<From, To> selector)
    {
        if (list == null) return null;
        List<To> b = new();
        foreach (From f in list)
        {
            b.Add(selector.Exec(f));
        }
        return b;
    }

    /** Find exact object type or subclass of cl in list */
    public static T Find<T>(List<T> ops, Type cl) where T : class
    {
        foreach (var o in ops)
        {
            if (cl.IsInstanceOfType(o)) return (o as T);
            //			if ( o.getClass() == cl ) return o;
        }
        return null;
    }

    public static int IndexOf<T>(List<T> elems, Filter<T> filter)
    {
        for (int i = 0; i < elems.Count; i++)
        {
            if (filter.Select(elems[(i)])) return i;
        }
        return -1;
    }

    public static int LastIndexOf<T>(List<T> elems, Filter<T> filter)
    {
        for (int i = elems.Count - 1; i >= 0; i--)
        {
            if (filter.Select(elems[(i)])) return i;
        }
        return -1;
    }

    public static void SetSize<T>(List<T> list, int size)
    {
        if (size < list.Count)
        {
            list.Capacity = size;
            //list.subList(size, list.Count).clear();
        }
        else
        {
            while (size > list.Count)
            {
                list.Add(default);
            }
        }
    }

}
