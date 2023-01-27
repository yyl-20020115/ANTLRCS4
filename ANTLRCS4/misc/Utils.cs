/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using System.Text;

namespace org.antlr.v4.misc;

/** */
public static class Utils {


	public const int INTEGER_POOL_MAX_VALUE = 1000;


    public interface Filter<T> {
		bool select(T t);
	}

	public interface Func0<TResult> {
		TResult exec();
	}

	public interface Func1<T1, TResult> {
		TResult exec(T1 arg1);
	}

    public static String stripFileExtension(String name) {
        if ( name==null ) return null;
        int lastDot = name.LastIndexOf('.');
        if ( lastDot<0 ) return name;
        return name.Substring(0, lastDot);
    }

	public static String join(Object[] a, String separator) {
		StringBuilder buf = new StringBuilder();
		for (int i=0; i<a.Length; i++) {
			Object o = a[i];
			buf.Append(o.ToString());
			if ( (i+1)<a.Length ) {
				buf.Append(separator);
			}
		}
		return buf.ToString();
	}

	public static String sortLinesInString(String s) {
		String lines[] = s.split("\n");
		Arrays.sort(lines);
		List<String> linesL = Arrays.asList(lines);
		StringBuilder buf = new StringBuilder();
		for (String l : linesL) {
			buf.append(l);
			buf.append('\n');
		}
		return buf.toString();
	}

	public static List<String> nodesToStrings<T>(List<T> nodes) where T : GrammarAST
    {
		if ( nodes == null ) return null;
		List<String> a = new ();
		foreach (T t in nodes) a.Add(t.getText());
		return a;
	}

//	public static <T> List<T> list(T... values) {
//		List<T> x = new ArrayList<T>(values.length);
//		for (T v : values) {
//			if ( v!=null ) x.add(v);
//		}
//		return x;
//	}

	public static void writeSerializedATNIntegerHistogram(String filename, IntegerList serializedATN) {
		Dictionary<int, int> histo = new ();
		for (int i : serializedATN.toArray()) {
			if ( histo.containsKey(i) ) {
				histo.put(i, histo.get(i) + 1);
			}
			else {
				histo.put(i, 1);
			}
		}
		TreeMap<Integer,Integer> sorted = new TreeMap<>(histo);

		String output = "";
		output += "value,count\n";
		for (int key : sorted.keySet()) {
			output += key+","+sorted.get(key)+"\n";
		}
		try {
			Files.write(Paths.get(filename), output.getBytes(StandardCharsets.UTF_8));
		}
		catch (IOException ioe) {
			System.err.println(ioe);
		}
	}

	public static String capitalize(String s) {
		return Character.toUpperCase(s.charAt(0)) + s.substring(1);
	}

	public static String decapitalize(String s) {
		return Character.toLowerCase(s.charAt(0)) + s.substring(1);
	}

	/** apply methodName to list and return list of results. method has
	 *  no args.  This pulls data out of a list essentially.
	 */
	public static List<To> select<From, To>(List<From> list, Func1<From, To> selector) {
		if ( list==null ) return null;
		List<To> b = new ();
		foreach (From f in list) {
			b.Add(selector.exec(f));
		}
		return b;
	}

	/** Find exact object type or subclass of cl in list */
	public static T find<T>(List<T> ops, Type cl) where T:class {
		foreach (Object o in ops) {
			if ( cl.IsInstanceOfType(o) ) return (o as T);
//			if ( o.getClass() == cl ) return o;
		}
		return null;
	}

	public static int indexOf<T>(List<T> elems, Filter<T> filter) {
		for (int i=0; i<elems.Count; i++) {
			if ( filter.select(elems[(i)]) ) return i;
		}
		return -1;
	}

	public static int lastIndexOf<T>(List<T> elems, Filter<T> filter) {
		for (int i=elems.Count-1; i>=0; i--) {
			if ( filter.select(elems[(i)]) ) return i;
		}
		return -1;
	}

	public static void setSize(List<object> list, int size) {
		if (size < list.Count) {
			list.subList(size, list.Count).clear();
		}
		else {
			while (size > list.Count) {
				list.Add(null);
			}
		}
	}

}
