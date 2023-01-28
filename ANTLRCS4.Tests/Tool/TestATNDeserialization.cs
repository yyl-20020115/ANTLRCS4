/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.tool;

namespace org.antlr.v4.test.tool;

[TestClass]
public class TestATNDeserialization {
	[TestMethod] public void testSimpleNoBlock(){
		Grammar g = new Grammar(
			"parser grammar T;\n"+
			"a : A B ;");
		checkDeserializationIsStable(g);
	}

	[TestMethod] public void testEOF(){
		Grammar g = new Grammar(
			"parser grammar T;\n"+
			"a : EOF ;");
		checkDeserializationIsStable(g);
	}

	[TestMethod] public void testEOFInSet(){
		Grammar g = new Grammar(
			"parser grammar T;\n"+
			"a : (EOF|A) ;");
		checkDeserializationIsStable(g);
	}

	[TestMethod] public void testNot(){
		Grammar g = new Grammar(
			"parser grammar T;\n"+
			"tokens {A, B, C}\n" +
			"a : ~A ;");
		checkDeserializationIsStable(g);
	}

	[TestMethod] public void testWildcard(){
		Grammar g = new Grammar(
			"parser grammar T;\n"+
			"tokens {A, B, C}\n" +
			"a : . ;");
		checkDeserializationIsStable(g);
	}

	[TestMethod] public void testPEGAchillesHeel(){
		Grammar g = new Grammar(
			"parser grammar T;\n"+
			"a : A | A B ;");
		checkDeserializationIsStable(g);
	}

	[TestMethod] public void test3Alts(){
		Grammar g = new Grammar(
			"parser grammar T;\n"+
			"a : A | A B | A B C ;");
		checkDeserializationIsStable(g);
	}

	[TestMethod] public void testSimpleLoop(){
		Grammar g = new Grammar(
			"parser grammar T;\n"+
			"a : A+ B ;");
		checkDeserializationIsStable(g);
	}

	[TestMethod] public void testRuleRef(){
		Grammar g = new Grammar(
			"parser grammar T;\n"+
			"a : e ;\n" +
			"e : E ;\n");
		checkDeserializationIsStable(g);
	}

	[TestMethod] public void testLexerTwoRules(){
		LexerGrammar lg = new LexerGrammar(
			"lexer grammar L;\n"+
			"A : 'a' ;\n" +
			"B : 'b' ;\n");
		checkDeserializationIsStable(lg);
	}

	[TestMethod] public void testLexerEOF(){
		LexerGrammar lg = new LexerGrammar(
			"lexer grammar L;\n"+
			"A : 'a' EOF ;\n");
		checkDeserializationIsStable(lg);
	}

	[TestMethod] public void testLexerEOFInSet(){
		LexerGrammar lg = new LexerGrammar(
			"lexer grammar L;\n"+
			"A : 'a' (EOF|'\\n') ;\n");
		checkDeserializationIsStable(lg);
	}

	[TestMethod] public void testLexerRange(){
		LexerGrammar lg = new LexerGrammar(
			"lexer grammar L;\n"+
			"INT : '0'..'9' ;\n");
		checkDeserializationIsStable(lg);
	}

	[TestMethod] public void testLexerLoops(){
		LexerGrammar lg = new LexerGrammar(
			"lexer grammar L;\n"+
			"INT : '0'..'9'+ ;\n");
		checkDeserializationIsStable(lg);
	}

	[TestMethod] public void testLexerNotSet(){
		LexerGrammar lg = new LexerGrammar(
			"lexer grammar L;\n"+
			"ID : ~('a'|'b')\n ;");
		checkDeserializationIsStable(lg);
	}

	[TestMethod] public void testLexerNotSetWithRange(){
		LexerGrammar lg = new LexerGrammar(
			"lexer grammar L;\n"+
			"ID : ~('a'|'b'|'e'|'p'..'t')\n ;");
		checkDeserializationIsStable(lg);
	}

	[TestMethod] public void testLexerNotSetWithRange2(){
		LexerGrammar lg = new LexerGrammar(
			"lexer grammar L;\n"+
			"ID : ~('a'|'b') ~('e'|'p'..'t')\n ;");
		checkDeserializationIsStable(lg);
	}

	[TestMethod] public void test2ModesInLexer(){
		LexerGrammar lg = new LexerGrammar(
				"lexer grammar L;\n"+
						"A : 'a'\n ;\n" +
						"mode M;\n" +
						"B : 'b';\n" +
						"mode M2;\n" +
						"C : 'c';\n");
		checkDeserializationIsStable(lg);
	}

	[TestMethod] public void testLastValidBMPCharInSet(){
		LexerGrammar lg = new LexerGrammar(
				"lexer grammar L;\n" +
						"ID : 'Ā'..'\\uFFFC'; // FFFD+ are not valid char\n");
		checkDeserializationIsStable(lg);
	}

	protected void checkDeserializationIsStable(Grammar g) {
		ATN atn = createATN(g, false);
		IntegerList serialized = ATNSerializer.getSerialized(atn);
		String atnData = new ATNDescriber(atn, Arrays.asList(g.getTokenNames())).decode(serialized.toArray());

		IntegerList serialized16 = encodeIntsWith16BitWords(serialized);
		int[] ints16 = serialized16.toArray();
		char[] chars = new char[ints16.Length];
		for (int i = 0; i < ints16.Length; i++) {
			chars[i] = (char)ints16[i];
		}
		int[] serialized32 = decodeIntsEncodedAs16BitWords(chars, true);

		assertArrayEquals(serialized.toArray(), serialized32);

		ATN atn2 = new ATNDeserializer().deserialize(serialized.toArray());
		IntegerList serialized1 = ATNSerializer.getSerialized(atn2);
		String atn2Data = new ATNDescriber(atn2, Arrays.asList(g.getTokenNames())).decode(serialized1.toArray());

		Assert.AreEqual(atnData, atn2Data);
	}
}
