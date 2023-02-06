/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.parse;
using org.antlr.v4.runtime.misc;
using org.antlr.v4.tool;
using Attribute = org.antlr.v4.tool.Attribute;

namespace org.antlr.v4.test.tool;

[TestClass]
public class TestScopeParsing {
    readonly static String[] argPairs = {
			"", "",
			" ", "",
			"int i", "i:int",
			"int[] i, int j[]", "i:int[], j:int []",
			"Map<A,B>[] i, int j[]", "i:Map<A,B>[], j:int []",
			"Map<A,List<B>>[] i", "i:Map<A,List<B>>[]",
			"int i = 34+a[3], int j[] = new int[34]",
			"i:int=34+a[3], j:int []=new int[34]",
			"char *[3] foo = {1,2,3}", "foo:char *[3]={1,2,3}", // not valid C really, C is "type name" however so this is cool (this was broken in 4.5 anyway)
			"String[] headers", "headers:String[]",

			// C++
			"std::vector<std::string> x", "x:std::vector<std::string>", // yuck. Don't choose :: as the : of a declaration

			// python/ruby style
			"i", "i",
			"i,j", "i, j",
			"i\t,j, k", "i, j, k",

			// swift style
			"x: int", "x:int",
			"x :int", "x:int",
			"x:int", "x:int",
			"x:int=3", "x:int=3",
			"r:Rectangle=Rectangle(fromLength: 6, fromBreadth: 12)", "r:Rectangle=Rectangle(fromLength: 6, fromBreadth: 12)",
			"p:pointer to int", "p:pointer to int",
			"a: array[3] of int", "a:array[3] of int",
			"a \t:\tfunc(array[3] of int)", "a:func(array[3] of int)",
			"x:int, y:float", "x:int, y:float",
			"x:T?, f:func(array[3] of int), y:int", "x:T?, f:func(array[3] of int), y:int",

			// go is postfix type notation like "x int" but must use either "int x" or "x:int" in [...] actions
			"float64 x = 3", "x:float64=3",
			"map[string]int x", "x:map[string]int",
	};

    //@ParameterizedTest
    //@MethodSource("getAllTestDescriptors")
    public void testArgs(Parameter parameter){
		Grammar dummy = new Grammar("grammar T; a:'a';");

		Dictionary<String, Attribute> attributes = ScopeParser.ParseTypedArgList(null, parameter.input, dummy).attributes;
		List<String> @out = new ();
		foreach (String arg in attributes.Keys) {
			Attribute attr = attributes[(arg)];
            @out.Add(attr.ToString());
		}
		String actual = RuntimeUtils.join(@out.ToArray(), ", ");
		Assert.AreEqual(parameter.output, actual);
	}

	private static ICollection<Parameter> getAllTestDescriptors() {
		List<Parameter> tests = new ();
		for (int i = 0; i < argPairs.Length; i+=2) {
			String arg = argPairs[i];
			String output = argPairs[i+1];
			tests.Add(new Parameter(arg, output));
		}
		return tests;
	}

	public class Parameter {
		public readonly String input;
		public readonly String output;

		public Parameter(String input, String output) {
			this.input = input;
			this.output = output;
		}

		////@Override
		public override String ToString() {
			return input;
		}
	}
}
