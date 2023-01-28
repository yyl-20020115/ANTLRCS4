/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime;
using org.antlr.v4.runtime.misc;
using org.antlr.v4.runtime.tree.pattern;

namespace org.antlr.v4.test.tool;

[TestClass]
public class TestParseTreeMatcher {
	[TestMethod] public void testChunking() {
		ParseTreePatternMatcher m = new ParseTreePatternMatcher(null, null);
		Assert.AreEqual("[ID, ' = ', expr, ' ;']", m.split("<ID> = <expr> ;").ToString());
		Assert.AreEqual("[' ', ID, ' = ', expr]", m.split(" <ID> = <expr>").ToString());
		Assert.AreEqual("[ID, ' = ', expr]", m.split("<ID> = <expr>").ToString());
		Assert.AreEqual("[expr]", m.split("<expr>").ToString());
		Assert.AreEqual("['<x> foo']", m.split("\\<x\\> foo").ToString());
		Assert.AreEqual("['foo <x> bar ', tag]", m.split("foo \\<x\\> bar <tag>").ToString());
	}

	[TestMethod] public void testDelimiters() {
		ParseTreePatternMatcher m = new ParseTreePatternMatcher(null, null);
		m.setDelimiters("<<", ">>", "$");
		String result = m.split("<<ID>> = <<expr>> ;$<< ick $>>").ToString();
		Assert.AreEqual("[ID, ' = ', expr, ' ;<< ick >>']", result);
	}

	[TestMethod] public void testInvertedTags(){
		ParseTreePatternMatcher m= new ParseTreePatternMatcher(null, null);
		String result = null;
		try {
			m.split(">expr<");
		}
		catch (ArgumentException iae) {
			result = iae.Message;
		}
		String expected = "tag delimiters out of order in pattern: >expr<";
		Assert.AreEqual(expected, result);
	}

	[TestMethod] public void testUnclosedTag(){
		ParseTreePatternMatcher m = new ParseTreePatternMatcher(null, null);
		String result = null;
		try {
			m.split("<expr hi mom");
		}
		catch (ArgumentException iae) {
			result = iae.Message;
		}
		String expected = "unterminated tag in pattern: <expr hi mom";
		Assert.AreEqual(expected, result);
	}

	[TestMethod] public void testExtraClose(){
		ParseTreePatternMatcher m = new ParseTreePatternMatcher(null, null);
		String result = null;
		try {
			m.split("<expr> >");
		}
		catch (ArgumentException iae) {
			result = iae.Message;
		}
		String expected = "missing start tag in pattern: <expr> >";
		Assert.AreEqual(expected, result);
	}

	[TestMethod] public void testTokenizingPattern(){
		String grammar =
			"grammar X1;\n" +
			"s : ID '=' expr ';' ;\n" +
			"expr : ID | INT ;\n" +
			"ID : [a-z]+ ;\n" +
			"INT : [0-9]+ ;\n" +
			"WS : [ \\r\\n\\t]+ -> skip ;\n";
		ParseTreePatternMatcher m = getPatternMatcher("X1.g4", grammar, "X1Parser", "X1Lexer", "s");

		List<? : Token> tokens = m.tokenize("<ID> = <expr> ;");
		Assert.AreEqual("[ID:3, [@-1,1:1='=',<1>,1:1], expr:7, [@-1,1:1=';',<2>,1:1]]", tokens.ToString());
	}

	[TestMethod]
	public void testCompilingPattern(){
		String grammar =
			"grammar X2;\n" +
			"s : ID '=' expr ';' ;\n" +
			"expr : ID | INT ;\n" +
			"ID : [a-z]+ ;\n" +
			"INT : [0-9]+ ;\n" +
			"WS : [ \\r\\n\\t]+ -> skip ;\n";
		ParseTreePatternMatcher m = getPatternMatcher("X2.g4", grammar, "X2Parser", "X2Lexer", "s");

		ParseTreePattern t = m.compile("<ID> = <expr> ;", m.getParser().getRuleIndex("s"));
		Assert.AreEqual("(s <ID> = (expr <expr>) ;)", t.getPatternTree().toStringTree(m.getParser()));
	}

	[TestMethod]
	public void testCompilingPatternConsumesAllTokens(){
		String grammar =
			"grammar X2;\n" +
			"s : ID '=' expr ';' ;\n" +
			"expr : ID | INT ;\n" +
			"ID : [a-z]+ ;\n" +
			"INT : [0-9]+ ;\n" +
			"WS : [ \\r\\n\\t]+ -> skip ;\n";
		ParseTreePatternMatcher m = getPatternMatcher("X2.g4", grammar, "X2Parser", "X2Lexer", "s");

		bool failed = false;
		try {
			m.compile("<ID> = <expr> ; extra", m.getParser().getRuleIndex("s"));
		}
		catch (ParseTreePatternMatcher.StartRuleDoesNotConsumeFullPattern e) {
			failed = true;
		}
		Assert.IsTrue(failed);
	}

	[TestMethod]
	public void testPatternMatchesStartRule(){
		String grammar =
			"grammar X2;\n" +
			"s : ID '=' expr ';' ;\n" +
			"expr : ID | INT ;\n" +
			"ID : [a-z]+ ;\n" +
			"INT : [0-9]+ ;\n" +
			"WS : [ \\r\\n\\t]+ -> skip ;\n";
		ParseTreePatternMatcher m = getPatternMatcher("X2.g4", grammar, "X2Parser", "X2Lexer", "s");

		bool failed = false;
		try {
			m.compile("<ID> ;", m.getParser().getRuleIndex("s"));
		}
		catch (InputMismatchException e) {
			failed = true;
		}
		Assert.IsTrue(failed);
	}

	[TestMethod]
	public void testPatternMatchesStartRule2(){
		String grammar =
			"grammar X2;\n" +
			"s : ID '=' expr ';' | expr ';' ;\n" +
			"expr : ID | INT ;\n" +
			"ID : [a-z]+ ;\n" +
			"INT : [0-9]+ ;\n" +
			"WS : [ \\r\\n\\t]+ -> skip ;\n";
		ParseTreePatternMatcher m = getPatternMatcher("X2.g4", grammar, "X2Parser", "X2Lexer", "s");

		bool failed = false;
		try {
			m.compile("<ID> <ID> ;", m.getParser().getRuleIndex("s"));
		}
		catch (NoViableAltException e) {
			failed = true;
		}
		Assert.IsTrue(failed);
	}

	[TestMethod]
	public void testHiddenTokensNotSeenByTreePatternParser(){
		String grammar =
			"grammar X2;\n" +
			"s : ID '=' expr ';' ;\n" +
			"expr : ID | INT ;\n" +
			"ID : [a-z]+ ;\n" +
			"INT : [0-9]+ ;\n" +
			"WS : [ \\r\\n\\t]+ -> channel(HIDDEN) ;\n";
		ParseTreePatternMatcher m = getPatternMatcher("X2.g4", grammar, "X2Parser", "X2Lexer", "s");

		ParseTreePattern t = m.compile("<ID> = <expr> ;", m.getParser().getRuleIndex("s"));
		Assert.AreEqual("(s <ID> = (expr <expr>) ;)", t.getPatternTree().toStringTree(m.getParser()));
	}

	[TestMethod]
	public void testCompilingMultipleTokens(){
		String grammar =
			"grammar X2;\n" +
			"s : ID '=' ID ';' ;\n" +
			"ID : [a-z]+ ;\n" +
			"WS : [ \\r\\n\\t]+ -> skip ;\n";
		ParseTreePatternMatcher m =	getPatternMatcher("X2.g4", grammar, "X2Parser", "X2Lexer", "s");

		ParseTreePattern t = m.compile("<ID> = <ID> ;", m.getParser().getRuleIndex("s"));
		String results = t.getPatternTree().toStringTree(m.getParser());
		String expected = "(s <ID> = <ID> ;)";
		Assert.AreEqual(expected, results);
	}

	[TestMethod] public void testIDNodeMatches(){
		String grammar =
			"grammar X3;\n" +
			"s : ID ';' ;\n" +
			"ID : [a-z]+ ;\n" +
			"WS : [ \\r\\n\\t]+ -> skip ;\n";

		String input = "x ;";
		String pattern = "<ID>;";
		checkPatternMatch(grammar, "s", input, pattern, "X3");
	}

	[TestMethod] public void testIDNodeWithLabelMatches(){
		String grammar =
			"grammar X8;\n" +
			"s : ID ';' ;\n" +
			"ID : [a-z]+ ;\n" +
			"WS : [ \\r\\n\\t]+ -> skip ;\n";

		String input = "x ;";
		String pattern = "<id:ID>;";
		ParseTreeMatch m = checkPatternMatch(grammar, "s", input, pattern, "X8");
		Assert.AreEqual("{ID=[x], id=[x]}", m.getLabels().ToString());
		assertNotNull(m.get("id"));
		assertNotNull(m.get("ID"));
		Assert.AreEqual("x", m.get("id").getText());
		Assert.AreEqual("x", m.get("ID").getText());
		Assert.AreEqual("[x]", m.getAll("id").ToString());
		Assert.AreEqual("[x]", m.getAll("ID").ToString());

		assertNull(m.get("undefined"));
		Assert.AreEqual("[]", m.getAll("undefined").ToString());
	}

	[TestMethod] public void testLabelGetsLastIDNode(){
		String grammar =
			"grammar X9;\n" +
			"s : ID ID ';' ;\n" +
			"ID : [a-z]+ ;\n" +
			"WS : [ \\r\\n\\t]+ -> skip ;\n";

		String input = "x y;";
		String pattern = "<id:ID> <id:ID>;";
		ParseTreeMatch m = checkPatternMatch(grammar, "s", input, pattern, "X9");
		Assert.AreEqual("{ID=[x, y], id=[x, y]}", m.getLabels().ToString());
		assertNotNull(m.get("id"));
		assertNotNull(m.get("ID"));
		Assert.AreEqual("y", m.get("id").getText());
		Assert.AreEqual("y", m.get("ID").getText());
		Assert.AreEqual("[x, y]", m.getAll("id").ToString());
		Assert.AreEqual("[x, y]", m.getAll("ID").ToString());

		assertNull(m.get("undefined"));
		Assert.AreEqual("[]", m.getAll("undefined").ToString());
	}

	[TestMethod] public void testIDNodeWithMultipleLabelMatches(){
		String grammar =
			"grammar X7;\n" +
			"s : ID ID ID ';' ;\n" +
			"ID : [a-z]+ ;\n" +
			"WS : [ \\r\\n\\t]+ -> skip ;\n";

		String input = "x y z;";
		String pattern = "<a:ID> <b:ID> <a:ID>;";
		ParseTreeMatch m = checkPatternMatch(grammar, "s", input, pattern, "X7");
		Assert.AreEqual("{ID=[x, y, z], a=[x, z], b=[y]}", m.getLabels().ToString());
		assertNotNull(m.get("a")); // get first
		assertNotNull(m.get("b"));
		assertNotNull(m.get("ID"));
		Assert.AreEqual("z", m.get("a").getText());
		Assert.AreEqual("y", m.get("b").getText());
		Assert.AreEqual("z", m.get("ID").getText()); // get last
		Assert.AreEqual("[x, z]", m.getAll("a").ToString());
		Assert.AreEqual("[y]", m.getAll("b").ToString());
		Assert.AreEqual("[x, y, z]", m.getAll("ID").ToString()); // ordered

		Assert.AreEqual("xyz;", m.getTree().getText()); // whitespace stripped by lexer

		assertNull(m.get("undefined"));
		Assert.AreEqual("[]", m.getAll("undefined").ToString());
	}

	[TestMethod] public void testTokenAndRuleMatch(){
		String grammar =
			"grammar X4;\n" +
			"s : ID '=' expr ';' ;\n" +
			"expr : ID | INT ;\n" +
			"ID : [a-z]+ ;\n" +
			"INT : [0-9]+ ;\n" +
			"WS : [ \\r\\n\\t]+ -> skip ;\n";

		String input = "x = 99;";
		String pattern = "<ID> = <expr> ;";
		checkPatternMatch(grammar, "s", input, pattern, "X4");
	}

	[TestMethod] public void testTokenTextMatch(){
		String grammar =
			"grammar X4;\n" +
			"s : ID '=' expr ';' ;\n" +
			"expr : ID | INT ;\n" +
			"ID : [a-z]+ ;\n" +
			"INT : [0-9]+ ;\n" +
			"WS : [ \\r\\n\\t]+ -> skip ;\n";

		String input = "x = 0;";
		String pattern = "<ID> = 1;";
		bool invertMatch = true; // 0!=1
		checkPatternMatch(grammar, "s", input, pattern, "X4", invertMatch);

		input = "x = 0;";
		pattern = "<ID> = 0;";
		invertMatch = false;
		checkPatternMatch(grammar, "s", input, pattern, "X4", invertMatch);

		input = "x = 0;";
		pattern = "x = 0;";
		invertMatch = false;
		checkPatternMatch(grammar, "s", input, pattern, "X4", invertMatch);

		input = "x = 0;";
		pattern = "y = 0;";
		invertMatch = true;
		checkPatternMatch(grammar, "s", input, pattern, "X4", invertMatch);
	}

	[TestMethod] public void testAssign(){
		String grammar =
			"grammar X5;\n" +
			"s   : expr ';'\n" +
			//"    | 'return' expr ';'\n" +
			"    ;\n" +
			"expr: expr '.' ID\n" +
			"    | expr '*' expr\n" +
			"    | expr '=' expr\n" +
			"    | ID\n" +
			"    | INT\n" +
			"    ;\n" +
			"ID : [a-z]+ ;\n" +
			"INT : [0-9]+ ;\n" +
			"WS : [ \\r\\n\\t]+ -> skip ;\n";

		String input = "x = 99;";
		String pattern = "<ID> = <expr>;";
		checkPatternMatch(grammar, "s", input, pattern, "X5");
	}

	[TestMethod] public void testLRecursiveExpr(){
		String grammar =
			"grammar X6;\n" +
			"s   : expr ';'\n" +
			"    ;\n" +
			"expr: expr '.' ID\n" +
			"    | expr '*' expr\n" +
			"    | expr '=' expr\n" +
			"    | ID\n" +
			"    | INT\n" +
			"    ;\n" +
			"ID : [a-z]+ ;\n" +
			"INT : [0-9]+ ;\n" +
			"WS : [ \\r\\n\\t]+ -> skip ;\n";

		String input = "3*4*5";
		String pattern = "<expr> * <expr> * <expr>";
		checkPatternMatch(grammar, "expr", input, pattern, "X6");
	}

	private static ParseTreeMatch checkPatternMatch(String grammar, String startRule,
											String input, String pattern,
											String grammarName)
		
	{
		return checkPatternMatch(grammar, startRule, input, pattern, grammarName, false);
	}

	private static ParseTreeMatch checkPatternMatch(String grammar, String startRule,
											String input, String pattern,
											String grammarName, bool invertMatch)
		
	{
		String grammarFileName = grammarName+".g4";
		String parserName = grammarName+"Parser";
		String lexerName = grammarName+"Lexer";
		RunOptions runOptions = createOptionsForJavaToolTests(grammarFileName, grammar, parserName, lexerName,
				false, false, startRule, input,
				false, false, Stage.Execute, true);
		using(JavaRunner runner = new JavaRunner()) {
			JavaExecutedState executedState = (JavaExecutedState)runner.run(runOptions);
			JavaCompiledState compiledState = (JavaCompiledState)executedState.previousState;
			Parser parser = compiledState.initializeLexerAndParser("").b;

			ParseTreePattern p = parser.compileParseTreePattern(pattern, parser.getRuleIndex(startRule));

			ParseTreeMatch match = p.match(executedState.parseTree);
			bool matched = match.succeeded();
			if ( invertMatch ) assertFalse(matched);
			else Assert.IsTrue(matched);
			return match;
		}
	}

	private static ParseTreePatternMatcher getPatternMatcher(
			String grammarFileName, String grammar, String parserName, String lexerName, String startRule
	){
		RunOptions runOptions = createOptionsForJavaToolTests(grammarFileName, grammar, parserName, lexerName,
				false, false, startRule, null,
				false, false, Stage.Compile, false);
		using (JavaRunner runner = new JavaRunner()) {
			JavaCompiledState compiledState = (JavaCompiledState) runner.run(runOptions);

			Pair<Lexer, Parser> lexerParserPair = compiledState.initializeLexerAndParser("");

			return new ParseTreePatternMatcher(lexerParserPair.a, lexerParserPair.b);
		}
	}
}
