/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.codegen.model;
using org.antlr.v4.runtime;
using org.antlr.v4.runtime.misc;
using org.antlr.v4.runtime.tree;
using org.antlr.v4.runtime.tree.xpath;
using org.antlr.v4.test.runtime;
using org.antlr.v4.test.runtime.java;
using org.antlr.v4.test.runtime.states;

namespace org.antlr.v4.test.tool;

[TestClass]
public class TestXPath {
	public static readonly String grammar =
		"grammar Expr;\n" +
		"prog:   func+ ;\n" +
		"func:  'def' ID '(' arg (',' arg)* ')' body ;\n" +
		"body:  '{' stat+ '}' ;\n" +
		"arg :  ID ;\n" +
		"stat:   expr ';'                 # printExpr\n" +
		"    |   ID '=' expr ';'          # assign\n" +
		"    |   'return' expr ';'        # ret\n" +
		"    |   ';'                      # blank\n" +
		"    ;\n" +
		"expr:   expr ('*'|'/') expr      # MulDiv\n" +
		"    |   expr ('+'|'-') expr      # AddSub\n" +
		"    |   primary                  # prim\n" +
		"    ;\n" +
		"primary" +
		"    :   INT                      # int\n" +
		"    |   ID                       # id\n" +
		"    |   '(' expr ')'             # parens\n" +
		"	 ;" +
		"\n" +
		"MUL :   '*' ; // assigns token name to '*' used above in grammar\n" +
		"DIV :   '/' ;\n" +
		"ADD :   '+' ;\n" +
		"SUB :   '-' ;\n" +
		"RETURN : 'return' ;\n" +
		"ID  :   [a-zA-Z]+ ;      // match identifiers\n" +
		"INT :   [0-9]+ ;         // match integers\n" +
		"NEWLINE:'\\r'? '\\n' -> skip;     // return newlines to parser (is end-statement signal)\n" +
		"WS  :   [ \\t]+ -> skip ; // toss out whitespace\n";
	public static readonly String SAMPLE_PROGRAM =
			"def f(x,y) { x = 3+4; y; ; }\n" +
			"def g(x) { return 1+2*x; }\n";

	[TestMethod] public void testValidPaths(){
		String[] xpath = {
			"/prog/func",		// all funcs under prog at root
			"/prog/*",			// all children of prog at root
			"/*/func",			// all func kids of any root node
			"prog",				// prog must be root node
			"/prog",			// prog must be root node
			"/*",				// any root
			"*",				// any root
			"//ID",				// any ID in tree
			"//expr/primary/ID",// any ID child of a primary under any expr
			"//body//ID",		// any ID under a body
			"//'return'",		// any 'return' literal in tree, matched by literal name
			"//RETURN",			// any 'return' literal in tree, matched by symbolic name
			"//primary/*",		// all kids of any primary
			"//func/*/stat",	// all stat nodes grandkids of any func node
			"/prog/func/'def'",	// all def literal kids of func kid of prog
			"//stat/';'",		// all ';' under any stat node
			"//expr/primary/!ID",	// anything but ID under primary under any expr node
			"//expr/!primary",	// anything but primary under any expr node
			"//!*",				// nothing anywhere
			"/!*",				// nothing at root
			"//expr//ID",		// any ID under any expression (tests antlr/antlr4#370)
		};
		String[] expected = {
			"[func, func]",
			"[func, func]",
			"[func, func]",
			"[prog]",
			"[prog]",
			"[prog]",
			"[prog]",
			"[f, x, y, x, y, g, x, x]",
			"[y, x]",
			"[x, y, x]",
			"[return]",
			"[return]",
			"[3, 4, y, 1, 2, x]",
			"[stat, stat, stat, stat]",
			"[def, def]",
			"[;, ;, ;, ;]",
			"[3, 4, 1, 2]",
			"[expr, expr, expr, expr, expr, expr]",
			"[]",
			"[]",
			"[y, x]",
		};

		for (int i=0; i<xpath.Length; i++) {
			List<String> nodes = getNodeStrings("Expr.g4", grammar, SAMPLE_PROGRAM, xpath[i], "prog", "ExprParser", "ExprLexer");
			String result = nodes.ToString();
			Assert.AreEqual(expected[i], result, "path "+xpath[i]+" failed");
		}
	}

	[TestMethod] public void testWeirdChar(){
		String path = "&";
		String expected = "Invalid tokens or characters at index 0 in path '&'";

		testError("Expr.g4", grammar, SAMPLE_PROGRAM, path, expected, "prog", "ExprParser", "ExprLexer");
	}

	[TestMethod] public void testWeirdChar2(){
		String path = "//w&e/";
		String expected = "Invalid tokens or characters at index 3 in path '//w&e/'";

		testError("Expr.g4", grammar, SAMPLE_PROGRAM, path, expected, "prog", "ExprParser", "ExprLexer");
	}

	[TestMethod] public void testBadSyntax(){
		String path = "///";
		String expected = "/ at index 2 isn't a valid rule name";

		testError("Expr.g4", grammar, SAMPLE_PROGRAM, path, expected, "prog", "ExprParser", "ExprLexer");
	}

	[TestMethod] public void testMissingWordAtEnd(){
		String path = "//";
		String expected = "Missing path element at end of path";

		testError("Expr.g4", grammar, SAMPLE_PROGRAM, path, expected, "prog", "ExprParser", "ExprLexer");
	}

	[TestMethod] public void testBadTokenName(){
		String path = "//Ick";
		String expected = "Ick at index 2 isn't a valid token name";

		testError("Expr.g4", grammar, SAMPLE_PROGRAM, path, expected, "prog", "ExprParser", "ExprLexer");
	}

	[TestMethod] public void testBadRuleName(){
		String path = "/prog/ick";
		String expected = "ick at index 6 isn't a valid rule name";

		testError("Expr.g4", grammar, SAMPLE_PROGRAM, path, expected, "prog", "ExprParser", "ExprLexer");
	}

	private void testError(String grammarFileName, String grammar, String input, String xpath, String expected,
							 String startRuleName, String parserName, String lexerName)
		
	{
		ArgumentException e = null;
		try {
			compileAndExtract(grammarFileName, grammar, input, xpath, startRuleName, parserName, lexerName);
		}
		catch (ArgumentException iae) {
			e = iae;
		}
		Assert.IsNotNull(e);
		Assert.AreEqual(expected, e.Message);
	}

	private List<String> getNodeStrings(String grammarFileName, String grammar, String input, String xpath,
									   String startRuleName, String parserName, String lexerName)
		
	{
		Pair<String[], ICollection<ParseTree>> result = compileAndExtract(
				grammarFileName, grammar, input, xpath, startRuleName, parserName, lexerName);

		List<String> nodes = new ();
		foreach (ParseTree t in result.b) {
			if ( t is RuleContext) {
				RuleContext r = (RuleContext)t;
				nodes.Add(result.a[r.getRuleIndex()]);
			}
			else {
				TerminalNode token = (TerminalNode)t;
				nodes.Add(token.getText());
			}
		}
		return nodes;
	}

	private Pair<String[], ICollection<ParseTree>> compileAndExtract(String grammarFileName, String grammar,
																	String input, String xpath, String startRuleName,
																	String parserName, String lexerName
	){
		RunOptions runOptions = ToolTestUtils.createOptionsForJavaToolTests(grammarFileName, grammar, parserName, lexerName,
				false, false, startRuleName, input,
				false, false, Stage.Execute, true);
		JavaRunner runner = new JavaRunner();
		{
			JavaExecutedState executedState = (JavaExecutedState)runner.run(runOptions);
			JavaCompiledState compiledState = (JavaCompiledState)executedState.previousState;
            var parser = compiledState.initializeLexerAndParser(input).b;
			ICollection<ParseTree> found = XPath.findAll(executedState.parseTree, xpath, parser);

			return new (parser.getRuleNames(), found);
		}
	}
}
