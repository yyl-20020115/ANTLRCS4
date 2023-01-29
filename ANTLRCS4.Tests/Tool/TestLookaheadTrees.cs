/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime;
using org.antlr.v4.runtime.atn;
using org.antlr.v4.runtime.tree;
using org.antlr.v4.tool;

namespace org.antlr.v4.test.tool;

[TestClass]
public class TestLookaheadTrees {
	public static readonly String lexerText =
		"lexer grammar L;\n" +
		"DOT  : '.' ;\n" +
		"SEMI : ';' ;\n" +
		"BANG : '!' ;\n" +
		"PLUS : '+' ;\n" +
		"LPAREN : '(' ;\n" +
		"RPAREN : ')' ;\n" +
		"MULT : '*' ;\n" +
		"ID : [a-z]+ ;\n" +
		"INT : [0-9]+ ;\n" +
		"WS : [ \\r\\t\\n]+ ;\n";

	[TestMethod]
	public void testAlts(){
		LexerGrammar lg = new LexerGrammar(lexerText);
		Grammar g = new Grammar(
			"parser grammar T;\n" +
			"s : e SEMI EOF ;\n" +
			"e : ID DOT ID\n"+
			"  | ID LPAREN RPAREN\n"+
			"  ;\n",
			lg);

		String startRuleName = "s";
		int decision = 0;

		testLookaheadTrees(lg, g, "a.b;", startRuleName, decision,
						   new String[] {"(e:1 a . b)", "(e:2 a <error .>)"});
	}

	[TestMethod]
	public void testAlts2(){
		LexerGrammar lg = new LexerGrammar(lexerText);
		Grammar g = new Grammar(
			"parser grammar T;\n" +
			"s : e? SEMI EOF ;\n" +
			"e : ID\n" +
			"  | e BANG" +
			"  ;\n",
			lg);

		String startRuleName = "s";
		int decision = 1; // (...)* in e.

		testLookaheadTrees(lg, g, "a;", startRuleName, decision,
						   new String[] {"(e:2 (e:1 a) <error ;>)", // Decision for alt 1 is error as no ! char, but alt 2 (exit) is good.
										 "(s:1 (e:1 a) ; <EOF>)"}); // root s:1 is included to show ';' node
	}

	[TestMethod]
	public void testIncludeEOF(){
		LexerGrammar lg = new LexerGrammar(lexerText);
		Grammar g = new Grammar(
			"parser grammar T;\n" +
			"s : e ;\n" +
			"e : ID DOT ID EOF\n"+
			"  | ID DOT ID EOF\n"+
			"  ;\n",
			lg);

		int decision = 0;
		testLookaheadTrees(lg, g, "a.b", "s", decision,
						   new String[] {"(e:1 a . b <EOF>)", "(e:2 a . b <EOF>)"});
	}

	[TestMethod]
	public void testCallLeftRecursiveRule(){
		LexerGrammar lg = new LexerGrammar(lexerText);
		Grammar g = new Grammar(
			"parser grammar T;\n" +
			"s : a BANG EOF;\n" +
			"a : e SEMI \n" +
			"  | ID SEMI \n" +
			"  ;" +
			"e : e MULT e\n" +
			"  | e PLUS e\n" +
			"  | e DOT e\n" +
			"  | ID\n" +
			"  | INT\n" +
			"  ;\n",
			lg);

		int decision = 0;
		testLookaheadTrees(lg, g, "x;!", "s", decision,
						   new String[] {"(a:1 (e:4 x) ;)",
										 "(a:2 x ;)"}); // shouldn't include BANG, EOF
		decision = 2; // (...)* in e
		testLookaheadTrees(lg, g, "x+1;!", "s", decision,
						   new String[] {"(e:1 (e:4 x) <error +>)",
										 "(e:2 (e:4 x) + (e:5 1))",
										 "(e:3 (e:4 x) <error +>)"});
	}

	public void testLookaheadTrees(LexerGrammar lg, Grammar g,
								   String input,
								   String startRuleName,
								   int decision,
								   String[] expectedTrees)
	{
		int startRuleIndex = g.getRule(startRuleName).index;
		InterpreterTreeTextProvider nodeTextProvider =
					new InterpreterTreeTextProvider(g.getRuleNames());

		LexerInterpreter lexEngine = lg.createLexerInterpreter(new ANTLRInputStream(input));
		CommonTokenStream tokens = new CommonTokenStream(lexEngine);
		GrammarParserInterpreter parser = g.createGrammarParserInterpreter(tokens);
		parser.setProfile(true);
		ParseTree t = parser.parse(startRuleIndex);

		DecisionInfo decisionInfo = parser.getParseInfo().getDecisionInfo()[decision];
		LookaheadEventInfo lookaheadEventInfo = decisionInfo.SLL_MaxLookEvent;

		List<ParserRuleContext> lookaheadParseTrees =
			GrammarParserInterpreter.getLookaheadParseTrees(g, parser, tokens, startRuleIndex, lookaheadEventInfo.decision,
															lookaheadEventInfo.startIndex, lookaheadEventInfo.stopIndex);

		Assert.AreEqual(expectedTrees.Length, lookaheadParseTrees.Count);
		for (int i = 0; i < lookaheadParseTrees.Count; i++) {
			ParserRuleContext lt = lookaheadParseTrees[(i)];
			Assert.AreEqual(expectedTrees[i], Trees.toStringTree(lt, nodeTextProvider));
		}
	}
}
