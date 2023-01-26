/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.tool;

namespace org.antlr.v4.test.tool;

[TestClass]
public class TestAmbigParseTrees {
	[TestMethod] public void testParseDecisionWithinAmbiguousStartRule(){
		LexerGrammar lg = new LexerGrammar(
			"lexer grammar L;\n" +
			"A : 'a' ;\n" +
			"B : 'b' ;\n" +
			"C : 'c' ;\n");
		Grammar g = new Grammar(
			"parser grammar T;\n" +
			"s : A x C" +
			"  | A B C" +
			"  ;" +
			"x : B ; \n",
			lg);

		testInterpAtSpecificAlt(lg, g, "s", 1, "abc", "(s:1 a (x:1 b) c)");
		testInterpAtSpecificAlt(lg, g, "s", 2, "abc", "(s:2 a b c)");
	}

	[TestMethod] public void testAmbigAltsAtRoot(){
		LexerGrammar lg = new LexerGrammar(
			"lexer grammar L;\n" +
			"A : 'a' ;\n" +
			"B : 'b' ;\n" +
			"C : 'c' ;\n");
		Grammar g = new Grammar(
			"parser grammar T;\n" +
			"s : A x C" +
			"  | A B C" +
			"  ;" +
			"x : B ; \n",
			lg);

		String startRule = "s";
		String input = "abc";
		String expectedAmbigAlts = "{1, 2}";
		int decision = 0;
		String expectedOverallTree = "(s:1 a (x:1 b) c)";
		String[] expectedParseTrees = {"(s:1 a (x:1 b) c)",
									   "(s:2 a b c)"};

		testAmbiguousTrees(lg, g, startRule, input, decision,
						   expectedAmbigAlts,
						   expectedOverallTree, expectedParseTrees);
	}

	[TestMethod] public void testAmbigAltsNotAtRoot(){
		LexerGrammar lg = new LexerGrammar(
			"lexer grammar L;\n" +
			"A : 'a' ;\n" +
			"B : 'b' ;\n" +
			"C : 'c' ;\n");
		Grammar g = new Grammar(
			"parser grammar T;\n" +
			"s : x ;" +
			"x : y ;" +
			"y : A z C" +
			"  | A B C" +
			"  ;" +
			"z : B ; \n",
			lg);

		String startRule = "s";
		String input = "abc";
		String expectedAmbigAlts = "{1, 2}";
		int decision = 0;
		String expectedOverallTree = "(s:1 (x:1 (y:1 a (z:1 b) c)))";
		String[] expectedParseTrees = {"(y:1 a (z:1 b) c)",
									   "(y:2 a b c)"};

		testAmbiguousTrees(lg, g, startRule, input, decision,
						   expectedAmbigAlts,
						   expectedOverallTree, expectedParseTrees);
	}

	[TestMethod] public void testAmbigAltDipsIntoOuterContextToRoot(){
		LexerGrammar lg = new LexerGrammar(
			"lexer grammar L;\n" +
			"SELF : 'self' ;\n" +
			"ID : [a-z]+ ;\n" +
			"DOT : '.' ;\n");
		Grammar g = new Grammar(
			"parser grammar T;\n" +
			"e : p (DOT ID)* ;\n"+
			"p : SELF" +
			"  | SELF DOT ID" +
			"  ;",
			lg);

		String startRule = "e";
		String input = "self.x";
		String expectedAmbigAlts = "{1, 2}";
		int decision = 1; // decision in p
		String expectedOverallTree = "(e:1 (p:1 self) . x)";
		String[] expectedParseTrees = {"(e:1 (p:1 self) . x)",
									   "(p:2 self . x)"};

		testAmbiguousTrees(lg, g, startRule, input, decision,
						   expectedAmbigAlts,
						   expectedOverallTree, expectedParseTrees);
	}

	[TestMethod] public void testAmbigAltDipsIntoOuterContextBelowRoot(){
		LexerGrammar lg = new LexerGrammar(
			"lexer grammar L;\n" +
			"SELF : 'self' ;\n" +
			"ID : [a-z]+ ;\n" +
			"DOT : '.' ;\n");
		Grammar g = new Grammar(
			"parser grammar T;\n" +
			"s : e ;\n"+
			"e : p (DOT ID)* ;\n"+
			"p : SELF" +
			"  | SELF DOT ID" +
			"  ;",
			lg);

		String startRule = "s";
		String input = "self.x";
		String expectedAmbigAlts = "{1, 2}";
		int decision = 1; // decision in p
		String expectedOverallTree = "(s:1 (e:1 (p:1 self) . x))";
		String[] expectedParseTrees = {"(e:1 (p:1 self) . x)", // shouldn't include s
									   "(p:2 self . x)"};      // shouldn't include e

		testAmbiguousTrees(lg, g, startRule, input, decision,
						   expectedAmbigAlts,
						   expectedOverallTree, expectedParseTrees);
	}

	[TestMethod] public void testAmbigAltInLeftRecursiveBelowStartRule(){
		LexerGrammar lg = new LexerGrammar(
			"lexer grammar L;\n" +
			"SELF : 'self' ;\n" +
			"ID : [a-z]+ ;\n" +
			"DOT : '.' ;\n");
		Grammar g = new Grammar(
			"parser grammar T;\n" +
			"s : e ;\n" +
			"e : p | e DOT ID ;\n"+
			"p : SELF" +
			"  | SELF DOT ID" +
			"  ;",
			lg);

		String startRule = "s";
		String input = "self.x";
		String expectedAmbigAlts = "{1, 2}";
		int decision = 1; // decision in p
		String expectedOverallTree = "(s:1 (e:2 (e:1 (p:1 self)) . x))";
		String[] expectedParseTrees = {"(e:2 (e:1 (p:1 self)) . x)",
									   "(p:2 self . x)"};

		testAmbiguousTrees(lg, g, startRule, input, decision,
						   expectedAmbigAlts,
						   expectedOverallTree, expectedParseTrees);
	}

	[TestMethod] public void testAmbigAltInLeftRecursiveStartRule(){
		LexerGrammar lg = new LexerGrammar(
			"lexer grammar L;\n" +
			"SELF : 'self' ;\n" +
			"ID : [a-z]+ ;\n" +
			"DOT : '.' ;\n");
		Grammar g = new Grammar(
			"parser grammar T;\n" +
			"e : p | e DOT ID ;\n"+
			"p : SELF" +
			"  | SELF DOT ID" +
			"  ;",
			lg);

		String startRule = "e";
		String input = "self.x";
		String expectedAmbigAlts = "{1, 2}";
		int decision = 1; // decision in p
		String expectedOverallTree = "(e:2 (e:1 (p:1 self)) . x)";
		String[] expectedParseTrees = {"(e:2 (e:1 (p:1 self)) . x)",
									   "(p:2 self . x)"}; // shows just enough for self.x

		testAmbiguousTrees(lg, g, startRule, input, decision,
						   expectedAmbigAlts,
						   expectedOverallTree, expectedParseTrees);
	}

	public void testAmbiguousTrees(LexerGrammar lg, Grammar g,
								   String startRule, String input, int decision,
								   String expectedAmbigAlts,
								   String overallTree,
								   String[] expectedParseTrees)
	{
		InterpreterTreeTextProvider nodeTextProvider = new InterpreterTreeTextProvider(g.getRuleNames());

		LexerInterpreter lexEngine = lg.createLexerInterpreter(new ANTLRInputStream(input));
		CommonTokenStream tokens = new CommonTokenStream(lexEngine);
		 GrammarParserInterpreter parser = g.createGrammarParserInterpreter(tokens);
		parser.setProfile(true);
		parser.getInterpreter().setPredictionMode(PredictionMode.LL_EXACT_AMBIG_DETECTION);

		// PARSE
		int ruleIndex = g.rules.get(startRule).index;
		ParserRuleContext parseTree = parser.parse(ruleIndex);
		assertEquals(overallTree, Trees.toStringTree(parseTree, nodeTextProvider));
		System.out.println();

		DecisionInfo[] decisionInfo = parser.getParseInfo().getDecisionInfo();
		List<AmbiguityInfo> ambiguities = decisionInfo[decision].ambiguities;
		assertEquals(1, ambiguities.size());
		AmbiguityInfo ambiguityInfo = ambiguities.get(0);

		List<ParserRuleContext> ambiguousParseTrees =
			GrammarParserInterpreter.getAllPossibleParseTrees(g,
															  parser,
															  tokens,
															  decision,
															  ambiguityInfo.ambigAlts,
															  ambiguityInfo.startIndex,
															  ambiguityInfo.stopIndex,
															  ruleIndex);
		assertEquals(expectedAmbigAlts, ambiguityInfo.ambigAlts.ToString());
		assertEquals(ambiguityInfo.ambigAlts.cardinality(), ambiguousParseTrees.size());

		for (int i = 0; i<ambiguousParseTrees.size(); i++) {
			ParserRuleContext t = ambiguousParseTrees.get(i);
			assertEquals(expectedParseTrees[i], Trees.toStringTree(t, nodeTextProvider));
		}
	}

	void testInterpAtSpecificAlt(LexerGrammar lg, Grammar g,
								 String startRule, int startAlt,
								 String input,
								 String expectedParseTree)
	{
		LexerInterpreter lexEngine = lg.createLexerInterpreter(new ANTLRInputStream(input));
		CommonTokenStream tokens = new CommonTokenStream(lexEngine);
		ParserInterpreter parser = g.createGrammarParserInterpreter(tokens);
		RuleStartState ruleStartState = g.atn.ruleToStartState[g.getRule(startRule).index];
		Transition tr = ruleStartState.transition(0);
		ATNState t2 = tr.target;
		if ( !(t2 is BasicBlockStartState) ) {
			throw new IllegalArgumentException("rule has no decision: "+startRule);
		}
		parser.addDecisionOverride(((DecisionState)t2).decision, 0, startAlt);
		ParseTree t = parser.parse(g.rules.get(startRule).index);
		InterpreterTreeTextProvider nodeTextProvider = new InterpreterTreeTextProvider(g.getRuleNames());
		assertEquals(expectedParseTree, Trees.toStringTree(t, nodeTextProvider));
	}
}
