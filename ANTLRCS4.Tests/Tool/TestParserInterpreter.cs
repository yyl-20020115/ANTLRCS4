/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime.tree;
using org.antlr.v4.tool;

namespace org.antlr.v4.test.tool;

[TestClass]
public class TestParserInterpreter {
	[TestMethod] public void testEmptyStartRule(){
		LexerGrammar lg = new LexerGrammar(
			"lexer grammar L;\n" +
			"A : 'a' ;\n");
		Grammar g = new Grammar(
			"parser grammar T;\n" +
			"s :  ;",
			lg);

		testInterp(lg, g, "s", "", "s");
		testInterp(lg, g, "s", "a", "s");
	}

	[TestMethod] public void testA(){
		LexerGrammar lg = new LexerGrammar(
			"lexer grammar L;\n" +
			"A : 'a' ;\n");
		Grammar g = new Grammar(
			"parser grammar T;\n" +
			"s : A ;",
			lg);

		ParseTree t = testInterp(lg, g, "s", "a", "(s a)");
		Assert.AreEqual("0..0", t.getSourceInterval().ToString());
	}

	[TestMethod] public void testEOF(){
		LexerGrammar lg = new LexerGrammar(
			"lexer grammar L;\n" +
			"A : 'a' ;\n");
		Grammar g = new Grammar(
			"parser grammar T;\n" +
			"s : A EOF ;",
			lg);

		ParseTree t = testInterp(lg, g, "s", "a", "(s a <EOF>)");
		Assert.AreEqual("0..1", t.getSourceInterval().ToString());
	}

	[TestMethod] public void testEOFInChild(){
		LexerGrammar lg = new LexerGrammar(
			"lexer grammar L;\n" +
			"A : 'a' ;\n");
		Grammar g = new Grammar(
			"parser grammar T;\n" +
			"s : x ;\n" +
			"x : A EOF ;",
			lg);

		ParseTree t = testInterp(lg, g, "s", "a", "(s (x a <EOF>))");
		Assert.AreEqual("0..1", t.getSourceInterval().ToString());
		Assert.AreEqual("0..1", t.getChild(0).getSourceInterval().ToString());
	}

	[TestMethod] public void testEmptyRuleAfterEOFInChild(){
		LexerGrammar lg = new LexerGrammar(
			"lexer grammar L;\n" +
			"A : 'a' ;\n");
		Grammar g = new Grammar(
			"parser grammar T;\n" +
			"s : x y;\n" +
			"x : A EOF ;\n" +
			"y : ;",
			lg);

		ParseTree t = testInterp(lg, g, "s", "a", "(s (x a <EOF>) y)");
		Assert.AreEqual("0..1", t.getSourceInterval().ToString()); // s
		Assert.AreEqual("0..1", t.getChild(0).getSourceInterval().ToString()); // x
// unspecified		Assert.AreEqual("1..0", t.getChild(1).getSourceInterval().ToString()); // y
	}

	[TestMethod] public void testEmptyRuleAfterJustEOFInChild(){
		LexerGrammar lg = new LexerGrammar(
			"lexer grammar L;\n" +
			"A : 'a' ;\n");
		Grammar g = new Grammar(
			"parser grammar T;\n" +
			"s : x y;\n" +
			"x : EOF ;\n" +
			"y : ;",
			lg);

		ParseTree t = testInterp(lg, g, "s", "", "(s (x <EOF>) y)");
		Assert.AreEqual("0..0", t.getSourceInterval().ToString()); // s
		Assert.AreEqual("0..0", t.getChild(0).getSourceInterval().ToString()); // x
		// this next one is a weird special case where somebody tries to match beyond in the file
// unspecified		Assert.AreEqual("0..-1", t.getChild(1).getSourceInterval().ToString()); // y
	}

	[TestMethod] public void testEmptyInput(){
		LexerGrammar lg = new LexerGrammar(
			"lexer grammar L;\n" +
			"A : 'a' ;\n");
		Grammar g = new Grammar(
			"parser grammar T;\n" +
			"s : x EOF ;\n" +
			"x : ;\n",
			lg);

		ParseTree t = testInterp(lg, g, "s", "", "(s x <EOF>)");
		Assert.AreEqual("0..0", t.getSourceInterval().ToString()); // s
		Assert.AreEqual("0..-1", t.getChild(0).getSourceInterval().ToString()); // x
	}

	[TestMethod] public void testEmptyInputWithCallsAfter(){
		LexerGrammar lg = new LexerGrammar(
			"lexer grammar L;\n" +
			"A : 'a' ;\n");
		Grammar g = new Grammar(
			"parser grammar T;\n" +
			"s : x y ;\n" +
			"x : EOF ;\n" +
			"y : z ;\n" +
			"z : ;",
			lg);

		ParseTree t = testInterp(lg, g, "s", "", "(s (x <EOF>) (y z))");
		Assert.AreEqual("0..0", t.getSourceInterval().ToString()); // s
		Assert.AreEqual("0..0", t.getChild(0).getSourceInterval().ToString()); // x
// unspecified		Assert.AreEqual("0..-1", t.getChild(1).getSourceInterval().ToString()); // x
	}

	[TestMethod] public void testEmptyFirstRule(){
		LexerGrammar lg = new LexerGrammar(
			"lexer grammar L;\n" +
			"A : 'a' ;\n");
		Grammar g = new Grammar(
			"parser grammar T;\n" +
			"s : x A ;\n" +
			"x : ;\n",
			lg);

		ParseTree t = testInterp(lg, g, "s", "a", "(s x a)");
		Assert.AreEqual("0..0", t.getSourceInterval().ToString()); // s
		// This gets an empty interval because the stop token is null for x
		Assert.AreEqual("0..-1", t.getChild(0).getSourceInterval().ToString()); // x
	}

	[TestMethod] public void testAorB(){
		LexerGrammar lg = new LexerGrammar(
			"lexer grammar L;\n" +
			"A : 'a' ;\n" +
			"B : 'b' ;\n" +
			"C : 'c' ;\n");
		Grammar g = new Grammar(
			"parser grammar T;\n"+
			"s : A{;} | B ;",
			lg);
		testInterp(lg, g, "s", "a", "(s a)");
		testInterp(lg, g, "s", "b", "(s b)");
	}

	[TestMethod] public void testCall(){
		LexerGrammar lg = new LexerGrammar(
			"lexer grammar L;\n" +
			"A : 'a' ;\n" +
			"B : 'b' ;\n" +
			"C : 'c' ;\n");
		Grammar g = new Grammar(
			"parser grammar T;\n"+
			"s : t C ;\n" +
			"t : A{;} | B ;\n",
			lg);

		testInterp(lg, g, "s", "ac", "(s (t a) c)");
		testInterp(lg, g, "s", "bc", "(s (t b) c)");
	}

	[TestMethod] public void testCall2(){
		LexerGrammar lg = new LexerGrammar(
			"lexer grammar L;\n" +
			"A : 'a' ;\n" +
			"B : 'b' ;\n" +
			"C : 'c' ;\n");
		Grammar g = new Grammar(
			"parser grammar T;\n"+
			"s : t C ;\n" +
			"t : u ;\n" +
			"u : A{;} | B ;\n",
			lg);

		testInterp(lg, g, "s", "ac", "(s (t (u a)) c)");
		testInterp(lg, g, "s", "bc", "(s (t (u b)) c)");
	}

	[TestMethod] public void testOptionalA(){
		LexerGrammar lg = new LexerGrammar(
			"lexer grammar L;\n" +
			"A : 'a' ;\n" +
			"B : 'b' ;\n" +
			"C : 'c' ;\n");
		Grammar g = new Grammar(
			"parser grammar T;\n" +
			"s : A? B ;\n",
			lg);

		testInterp(lg, g, "s", "b", "(s b)");
		testInterp(lg, g, "s", "ab", "(s a b)");
	}

	[TestMethod] public void testOptionalAorB(){
		LexerGrammar lg = new LexerGrammar(
			"lexer grammar L;\n" +
			"A : 'a' ;\n" +
			"B : 'b' ;\n" +
			"C : 'c' ;\n");
		Grammar g = new Grammar(
			"parser grammar T;\n" +
			"s : (A{;}|B)? C ;\n",
			lg);

		testInterp(lg, g, "s", "c", "(s c)");
		testInterp(lg, g, "s", "ac", "(s a c)");
		testInterp(lg, g, "s", "bc", "(s b c)");
	}

	[TestMethod] public void testStarA(){
		LexerGrammar lg = new LexerGrammar(
			"lexer grammar L;\n" +
			"A : 'a' ;\n" +
			"B : 'b' ;\n" +
			"C : 'c' ;\n");
		Grammar g = new Grammar(
			"parser grammar T;\n" +
			"s : A* B ;\n",
			lg);

		testInterp(lg, g, "s", "b", "(s b)");
		testInterp(lg, g, "s", "ab", "(s a b)");
		testInterp(lg, g, "s", "aaaaaab", "(s a a a a a a b)");
	}

	[TestMethod] public void testStarAorB(){
		LexerGrammar lg = new LexerGrammar(
			"lexer grammar L;\n" +
			"A : 'a' ;\n" +
			"B : 'b' ;\n" +
			"C : 'c' ;\n");
		Grammar g = new Grammar(
			"parser grammar T;\n" +
			"s : (A{;}|B)* C ;\n",
			lg);

		testInterp(lg, g, "s", "c", "(s c)");
		testInterp(lg, g, "s", "ac", "(s a c)");
		testInterp(lg, g, "s", "bc", "(s b c)");
		testInterp(lg, g, "s", "abaaabc", "(s a b a a a b c)");
		testInterp(lg, g, "s", "babac", "(s b a b a c)");
	}

	[TestMethod] public void testLeftRecursion(){
		LexerGrammar lg = new LexerGrammar(
			"lexer grammar L;\n" +
			"A : 'a' ;\n" +
			"B : 'b' ;\n" +
			"C : 'c' ;\n" +
			"PLUS : '+' ;\n" +
			"MULT : '*' ;\n");
		Grammar g = new Grammar(
			"parser grammar T;\n" +
			"s : e ;\n" +
			"e : e MULT e\n" +
			"  | e PLUS e\n" +
			"  | A\n" +
			"  ;\n",
			lg);

		testInterp(lg, g, "s", "a", 	"(s (e a))");
		testInterp(lg, g, "s", "a+a", 	"(s (e (e a) + (e a)))");
		testInterp(lg, g, "s", "a*a", 	"(s (e (e a) * (e a)))");
		testInterp(lg, g, "s", "a+a+a", "(s (e (e (e a) + (e a)) + (e a)))");
		testInterp(lg, g, "s", "a*a+a", "(s (e (e (e a) * (e a)) + (e a)))");
		testInterp(lg, g, "s", "a+a*a", "(s (e (e a) + (e (e a) * (e a))))");
	}

	/**
	 * This is a regression test for antlr/antlr4#461.
	 * https://github.com/antlr/antlr4/issues/461
	 */
	[TestMethod] public void testLeftRecursiveStartRule(){
		LexerGrammar lg = new LexerGrammar(
			"lexer grammar L;\n" +
			"A : 'a' ;\n" +
			"B : 'b' ;\n" +
			"C : 'c' ;\n" +
			"PLUS : '+' ;\n" +
			"MULT : '*' ;\n");
		Grammar g = new Grammar(
			"parser grammar T;\n" +
			"s : e ;\n" +
			"e : e MULT e\n" +
			"  | e PLUS e\n" +
			"  | A\n" +
			"  ;\n",
			lg);

		testInterp(lg, g, "e", "a", 	"(e a)");
		testInterp(lg, g, "e", "a+a", 	"(e (e a) + (e a))");
		testInterp(lg, g, "e", "a*a", 	"(e (e a) * (e a))");
		testInterp(lg, g, "e", "a+a+a", "(e (e (e a) + (e a)) + (e a))");
		testInterp(lg, g, "e", "a*a+a", "(e (e (e a) * (e a)) + (e a))");
		testInterp(lg, g, "e", "a+a*a", "(e (e a) + (e (e a) * (e a)))");
	}

	[TestMethod] public void testCaseInsensitiveTokensInParser(){
		LexerGrammar lg = new LexerGrammar(
				"lexer grammar L;\n" +
				"options { caseInsensitive = true; }\n" +
				"NOT: 'not';\n" +
				"AND: 'and';\n" +
				"NEW: 'new';\n" +
				"LB:  '(';\n" +
				"RB:  ')';\n" +
				"ID: [a-z_][a-z_0-9]*;\n" +
				"WS: [ \\t\\n\\r]+ -> skip;");
		Grammar g = new Grammar(
				"parser grammar T;\n" +
				"options { caseInsensitive = true; }\n" +
				"e\n" +
				"    : ID\n" +
				"    | 'not' e\n" +
				"    | e 'and' e\n" +
				"    | 'new' ID '(' e ')'\n" +
				"    ;", lg);

		testInterp(lg, g, "e", "NEW Abc (Not a AND not B)", "(e NEW Abc ( (e (e Not (e a)) AND (e not (e B))) ))");
	}

	ParseTree testInterp(LexerGrammar lg, Grammar g,
					String startRule, String input,
					String expectedParseTree)
	{
		LexerInterpreter lexEngine = lg.createLexerInterpreter(new ANTLRInputStream(input));
		CommonTokenStream tokens = new CommonTokenStream(lexEngine);
		ParserInterpreter parser = g.createParserInterpreter(tokens);
		ParseTree t = parser.parse(g.rules.get(startRule).index);
		Assert.AreEqual(expectedParseTree, t.toStringTree(parser));
		return t;
	}
}
