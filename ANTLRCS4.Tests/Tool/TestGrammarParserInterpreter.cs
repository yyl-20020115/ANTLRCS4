/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */
using org.antlr.v4.runtime;
using org.antlr.v4.runtime.tree;
using org.antlr.v4.tool;

namespace org.antlr.v4.test.tool;


/** Tests to ensure GrammarParserInterpreter subclass of ParserInterpreter
 *  hasn't messed anything up.
 */
public class TestGrammarParserInterpreter {
	public static readonly String lexerText = "lexer grammar L;\n" +
										   "PLUS : '+' ;\n" +
										   "MULT : '*' ;\n" +
										   "ID : [a-z]+ ;\n" +
										   "INT : [0-9]+ ;\n" +
										   "WS : [ \\r\\t\\n]+ ;\n";

	[TestMethod]
	public void testAlts(){
		LexerGrammar lg = new LexerGrammar(lexerText);
		Grammar g = new Grammar(
			"parser grammar T;\n" +
			"s : ID\n"+
			"  | INT{;}\n"+
			"  ;\n",
			lg);
		testInterp(lg, g, "s", "a",		"(s:1 a)");
		testInterp(lg, g, "s", "3", 	"(s:2 3)");
	}

	[TestMethod]
	public void testAltsAsSet(){
		LexerGrammar lg = new LexerGrammar(lexerText);
		Grammar g = new Grammar(
			"parser grammar T;\n" +
			"s : ID\n"+
			"  | INT\n"+
			"  ;\n",
			lg);
		testInterp(lg, g, "s", "a",		"(s:1 a)");
		testInterp(lg, g, "s", "3", 	"(s:1 3)");
	}

	[TestMethod]
	public void testAltsWithLabels(){
		LexerGrammar lg = new LexerGrammar(lexerText);
		Grammar g = new Grammar(
			"parser grammar T;\n" +
			"s : ID  # foo\n" +
			"  | INT # bar\n" +
			"  ;\n",
			lg);
		// it won't show the labels here because my simple node text provider above just shows the alternative
		testInterp(lg, g, "s", "a",		"(s:1 a)");
		testInterp(lg, g, "s", "3", 	"(s:2 3)");
	}

	[TestMethod]
	public void testOneAlt(){
		LexerGrammar lg = new LexerGrammar(lexerText);
		Grammar g = new Grammar(
			"parser grammar T;\n" +
			"s : ID\n"+
			"  ;\n",
			lg);
		testInterp(lg, g, "s", "a",		"(s:1 a)");
	}


	[TestMethod]
	public void testLeftRecursionWithMultiplePrimaryAndRecursiveOps(){
		LexerGrammar lg = new LexerGrammar(lexerText);
		Grammar g = new Grammar(
			"parser grammar T;\n" +
			"s : e EOF ;\n" +
			"e : e MULT e\n" +
			"  | e PLUS e\n" +
			"  | INT\n" +
			"  | ID\n" +
			"  ;\n",
			lg);

		testInterp(lg, g, "s", "a",		"(s:1 (e:4 a) <EOF>)");
		testInterp(lg, g, "e", "a",		"(e:4 a)");
		testInterp(lg, g, "e", "34",	"(e:3 34)");
		testInterp(lg, g, "e", "a+1",	"(e:2 (e:4 a) + (e:3 1))");
		testInterp(lg, g, "e", "1+2*a",	"(e:2 (e:3 1) + (e:1 (e:3 2) * (e:4 a)))");
	}

	InterpreterRuleContext testInterp(LexerGrammar lg, Grammar g,
	                                  String startRule, String input,
	                                  String expectedParseTree)
	{
		LexerInterpreter lexEngine = lg.createLexerInterpreter(new ANTLRInputStream(input));
		CommonTokenStream tokens = new CommonTokenStream(lexEngine);
		GrammarParserInterpreter parser = g.createGrammarParserInterpreter(tokens);
		ParseTree t = parser.parse(g.rules.get(startRule).index);
		InterpreterTreeTextProvider nodeTextProvider = new InterpreterTreeTextProvider(g.getRuleNames());
		String treeStr = Trees.toStringTree(t, nodeTextProvider);
//		Console.Out.WriteLine("parse tree: "+treeStr);
		Assert.AreEqual(expectedParseTree, treeStr);
		return (InterpreterRuleContext)t;
	}
}
