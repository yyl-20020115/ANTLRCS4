/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime.atn;
using org.antlr.v4.runtime.dfa;
using org.antlr.v4.runtime.misc;
using org.antlr.v4.tool;

namespace org.antlr.v4.test.tool;

// NOTICE: TOKENS IN LEXER, PARSER MUST BE SAME OR TOKEN TYPE MISMATCH
// NOTICE: TOKENS IN LEXER, PARSER MUST BE SAME OR TOKEN TYPE MISMATCH
// NOTICE: TOKENS IN LEXER, PARSER MUST BE SAME OR TOKEN TYPE MISMATCH
[TestClass]
public class TestATNInterpreter {
	[TestMethod] public void testSimpleNoBlock(){
		LexerGrammar lg = new LexerGrammar(
			"lexer grammar L;\n" +
			"A : 'a' ;\n" +
			"B : 'b' ;\n" +
			"C : 'c' ;\n");
		Grammar g = new Grammar(
			"parser grammar T;\n"+
			"a : A B ;");
		checkMatchedAlt(lg, g, "ab", 1);
	}

	[TestMethod] public void testSet(){
		LexerGrammar lg = new LexerGrammar(
			"lexer grammar L;\n" +
			"A : 'a' ;\n" +
			"B : 'b' ;\n" +
			"C : 'c' ;\n");
		Grammar g = new Grammar(
			"parser grammar T;\n"+
			"tokens {A,B,C}\n" +
			"a : ~A ;");
		checkMatchedAlt(lg, g, "b", 1);
	}

	[TestMethod] public void testPEGAchillesHeel(){
		LexerGrammar lg = new LexerGrammar(
			"lexer grammar L;\n" +
			"A : 'a' ;\n" +
			"B : 'b' ;\n" +
			"C : 'c' ;\n");
		Grammar g = new Grammar(
			"parser grammar T;\n"+
			"a : A | A B ;");
		checkMatchedAlt(lg, g, "a", 1);
		checkMatchedAlt(lg, g, "ab", 2);
		checkMatchedAlt(lg, g, "abc", 2);
	}

	[TestMethod] public void testMustTrackPreviousGoodAlt(){
		LexerGrammar lg = new LexerGrammar(
			"lexer grammar L;\n" +
			"A : 'a' ;\n" +
			"B : 'b' ;\n" +
			"C : 'c' ;\n");
		Grammar g = new Grammar(
			"parser grammar T;\n"+
			"a : A | A B ;");

		checkMatchedAlt(lg, g, "a", 1);
		checkMatchedAlt(lg, g, "ab", 2);

		checkMatchedAlt(lg, g, "ac", 1);
		checkMatchedAlt(lg, g, "abc", 2);
	}

	[TestMethod]
	public void testMustTrackPreviousGoodAltWithEOF(){
		LexerGrammar lg = new LexerGrammar(
			"lexer grammar L;\n" +
			"A : 'a' ;\n" +
			"B : 'b' ;\n" +
			"C : 'c' ;\n");
		Grammar g = new Grammar(
			"parser grammar T;\n"+
			"a : (A | A B) EOF;");

		checkMatchedAlt(lg, g, "a", 1);
		checkMatchedAlt(lg, g, "ab", 2);

		try {
			checkMatchedAlt(lg, g, "ac", 1);
			fail();
		}
		catch (NoViableAltException re) {
			Assert.AreEqual(1, re.getOffendingToken().getTokenIndex());
			Assert.AreEqual(3, re.getOffendingToken().getType());
		}
	}

	[TestMethod] public void testMustTrackPreviousGoodAlt2(){
		LexerGrammar lg = new LexerGrammar(
			"lexer grammar L;\n" +
			"A : 'a' ;\n" +
			"B : 'b' ;\n" +
			"C : 'c' ;\n" +
			"D : 'd' ;\n");
		Grammar g = new Grammar(
			"parser grammar T;\n"+
			"a : A | A B | A B C ;");

		checkMatchedAlt(lg, g, "a", 1);
		checkMatchedAlt(lg, g, "ab", 2);
		checkMatchedAlt(lg, g, "abc", 3);

		checkMatchedAlt(lg, g, "ad", 1);
		checkMatchedAlt(lg, g, "abd", 2);
		checkMatchedAlt(lg, g, "abcd", 3);
	}

	[TestMethod]
	public void testMustTrackPreviousGoodAlt2WithEOF(){
		LexerGrammar lg = new LexerGrammar(
			"lexer grammar L;\n" +
			"A : 'a' ;\n" +
			"B : 'b' ;\n" +
			"C : 'c' ;\n" +
			"D : 'd' ;\n");
		Grammar g = new Grammar(
			"parser grammar T;\n"+
			"a : (A | A B | A B C) EOF;");

		checkMatchedAlt(lg, g, "a", 1);
		checkMatchedAlt(lg, g, "ab", 2);
		checkMatchedAlt(lg, g, "abc", 3);

		try {
			checkMatchedAlt(lg, g, "abd", 1);
			fail();
		}
		catch (NoViableAltException re) {
			Assert.AreEqual(2, re.getOffendingToken().getTokenIndex());
			Assert.AreEqual(4, re.getOffendingToken().getType());
		}
	}

	[TestMethod] public void testMustTrackPreviousGoodAlt3(){
		LexerGrammar lg = new LexerGrammar(
			"lexer grammar L;\n" +
			"A : 'a' ;\n" +
			"B : 'b' ;\n" +
			"C : 'c' ;\n" +
			"D : 'd' ;\n");
		Grammar g = new Grammar(
			"parser grammar T;\n"+
			"a : A B | A | A B C ;");

		checkMatchedAlt(lg, g, "a", 2);
		checkMatchedAlt(lg, g, "ab", 1);
		checkMatchedAlt(lg, g, "abc", 3);

		checkMatchedAlt(lg, g, "ad", 2);
		checkMatchedAlt(lg, g, "abd", 1);
		checkMatchedAlt(lg, g, "abcd", 3);
	}

	[TestMethod]
	public void testMustTrackPreviousGoodAlt3WithEOF(){
		LexerGrammar lg = new LexerGrammar(
			"lexer grammar L;\n" +
			"A : 'a' ;\n" +
			"B : 'b' ;\n" +
			"C : 'c' ;\n" +
			"D : 'd' ;\n");
		Grammar g = new Grammar(
			"parser grammar T;\n"+
			"a : (A B | A | A B C) EOF;");

		checkMatchedAlt(lg, g, "a", 2);
		checkMatchedAlt(lg, g, "ab", 1);
		checkMatchedAlt(lg, g, "abc", 3);

		try {
			checkMatchedAlt(lg, g, "abd", 1);
			fail();
		}
		catch (NoViableAltException re) {
			Assert.AreEqual(2, re.getOffendingToken().getTokenIndex());
			Assert.AreEqual(4, re.getOffendingToken().getType());
		}
	}

	[TestMethod] public void testAmbigAltChooseFirst(){
		LexerGrammar lg = new LexerGrammar(
			"lexer grammar L;\n" +
			"A : 'a' ;\n" +
			"B : 'b' ;\n" +
			"C : 'c' ;\n" +
			"D : 'd' ;\n");
		Grammar g = new Grammar(
			"parser grammar T;\n"+
			"a : A B | A B ;"); // first alt
		checkMatchedAlt(lg, g, "ab", 1);
		checkMatchedAlt(lg, g, "abc", 1);
	}

	[TestMethod] public void testAmbigAltChooseFirstWithFollowingToken(){
		LexerGrammar lg = new LexerGrammar(
			"lexer grammar L;\n" +
			"A : 'a' ;\n" +
			"B : 'b' ;\n" +
			"C : 'c' ;\n" +
			"D : 'd' ;\n");
		Grammar g = new Grammar(
			"parser grammar T;\n"+
			"a : (A B | A B) C ;"); // first alt
		checkMatchedAlt(lg, g, "abc", 1);
		checkMatchedAlt(lg, g, "abcd", 1);
	}

	[TestMethod] public void testAmbigAltChooseFirstWithFollowingToken2(){
		LexerGrammar lg = new LexerGrammar(
			"lexer grammar L;\n" +
			"A : 'a' ;\n" +
			"B : 'b' ;\n" +
			"C : 'c' ;\n" +
			"D : 'd' ;\n");
		Grammar g = new Grammar(
			"parser grammar T;\n"+
			"a : (A B | A B | C) D ;");
		checkMatchedAlt(lg, g, "abd", 1);
		checkMatchedAlt(lg, g, "abdc", 1);
		checkMatchedAlt(lg, g, "cd", 3);
	}

	[TestMethod] public void testAmbigAltChooseFirst2(){
		LexerGrammar lg = new LexerGrammar(
			"lexer grammar L;\n" +
			"A : 'a' ;\n" +
			"B : 'b' ;\n" +
			"C : 'c' ;\n" +
			"D : 'd' ;\n");
		Grammar g = new Grammar(
			"parser grammar T;\n"+
			"a : A B | A B | A B C ;");

		checkMatchedAlt(lg, g, "ab", 1);
		checkMatchedAlt(lg, g, "abc", 3);

		checkMatchedAlt(lg, g, "abd", 1);
		checkMatchedAlt(lg, g, "abcd", 3);
	}

	[TestMethod]
	public void testAmbigAltChooseFirst2WithEOF(){
		LexerGrammar lg = new LexerGrammar(
			"lexer grammar L;\n" +
			"A : 'a' ;\n" +
			"B : 'b' ;\n" +
			"C : 'c' ;\n" +
			"D : 'd' ;\n");
		Grammar g = new Grammar(
			"parser grammar T;\n"+
			"a : (A B | A B | A B C) EOF;");

		checkMatchedAlt(lg, g, "ab", 1);
		checkMatchedAlt(lg, g, "abc", 3);

		try {
			checkMatchedAlt(lg, g, "abd", 1);
			fail();
		}
		catch (NoViableAltException re) {
			Assert.AreEqual(2, re.getOffendingToken().getTokenIndex());
			Assert.AreEqual(4, re.getOffendingToken().getType());
		}
	}

	[TestMethod] public void testSimpleLoop(){
		LexerGrammar lg = new LexerGrammar(
			"lexer grammar L;\n" +
			"A : 'a' ;\n" +
			"B : 'b' ;\n" +
			"C : 'c' ;\n" +
			"D : 'd' ;\n");
		Grammar g = new Grammar(
			"parser grammar T;\n"+
			"a : A+ B ;");
		checkMatchedAlt(lg, g, "ab", 1);
		checkMatchedAlt(lg, g, "aab", 1);
		checkMatchedAlt(lg, g, "aaaaaab", 1);
		checkMatchedAlt(lg, g, "aabd", 1);
	}

	[TestMethod] public void testCommonLeftPrefix(){
		LexerGrammar lg = new LexerGrammar(
			"lexer grammar L;\n" +
			"A : 'a' ;\n" +
			"B : 'b' ;\n" +
			"C : 'c' ;\n");
		Grammar g = new Grammar(
			"parser grammar T;\n"+
			"a : A B | A C ;");
		checkMatchedAlt(lg, g, "ab", 1);
		checkMatchedAlt(lg, g, "ac", 2);
	}

	[TestMethod] public void testArbitraryLeftPrefix(){
		LexerGrammar lg = new LexerGrammar(
			"lexer grammar L;\n" +
			"A : 'a' ;\n" +
			"B : 'b' ;\n" +
			"C : 'c' ;\n");
		Grammar g = new Grammar(
			"parser grammar T;\n"+
			"a : A+ B | A+ C ;");
		checkMatchedAlt(lg, g, "aac", 2);
	}

	[TestMethod] public void testRecursiveLeftPrefix(){
		LexerGrammar lg = new LexerGrammar(
			"lexer grammar L;\n" +
			"A : 'a' ;\n" +
			"B : 'b' ;\n" +
			"C : 'c' ;\n" +
			"LP : '(' ;\n" +
			"RP : ')' ;\n" +
			"INT : '0'..'9'+ ;\n"
		);
		Grammar g = new Grammar(
			"parser grammar T;\n"+
			"tokens {A,B,C,LP,RP,INT}\n" +
			"a : e B | e C ;\n" +
			"e : LP e RP\n" +
			"  | INT\n" +
			"  ;");
		checkMatchedAlt(lg, g, "34b", 1);
		checkMatchedAlt(lg, g, "34c", 2);
		checkMatchedAlt(lg, g, "(34)b", 1);
		checkMatchedAlt(lg, g, "(34)c", 2);
		checkMatchedAlt(lg, g, "((34))b", 1);
		checkMatchedAlt(lg, g, "((34))c", 2);
	}

	public void checkMatchedAlt(LexerGrammar lg, Grammar g,
								String inputString,
								int expected)
	{
		ATN lexatn = createATN(lg, true);
		LexerATNSimulator lexInterp = new LexerATNSimulator(lexatn,new DFA[] { new DFA(lexatn.modeToStartState.get(Lexer.DEFAULT_MODE)) },null);
		IntegerList types = getTokenTypesViaATN(inputString, lexInterp);
//		Console.Out.WriteLine(types);

		g.importVocab(lg);

		ParserATNFactory f = new ParserATNFactory(g);
		ATN atn = f.createATN();

		TokenStream input = new MockIntTokenStream(types);
//		Console.Out.WriteLine("input="+input.types);
		ParserInterpreterForTesting interp = new ParserInterpreterForTesting(g, input);
		ATNState startState = atn.ruleToStartState[g.getRule("a").index];
		if ( startState.transition(0).target is BlockStartState ) {
			startState = startState.transition(0).target;
		}

		DOTGenerator dot = new DOTGenerator(g);
//		Console.Out.WriteLine(dot.getDOT(atn.ruleToStartState[g.getRule("a").index]));
		Rule r = g.getRule("e");
//		if ( r!=null ) Console.Out.WriteLine(dot.getDOT(atn.ruleToStartState[r.index]));

		int result = interp.matchATN(input, startState);
		Assert.AreEqual(expected, result);
	}
}
