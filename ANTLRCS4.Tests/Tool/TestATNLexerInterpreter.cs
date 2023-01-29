/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime;
using org.antlr.v4.runtime.atn;
using org.antlr.v4.runtime.dfa;
using org.antlr.v4.runtime.misc;
using org.antlr.v4.tool;
using System.Text;

namespace org.antlr.v4.test.tool;

/**
 * Lexer rules are little quirky when it comes to wildcards. Problem
 * stems from the fact that we want the longest match to win among
 * several rules and even within a rule. However, that conflicts
 * with the notion of non-greedy, which by definition tries to match
 * the fewest possible. During ATN construction, non-greedy loops
 * have their entry and exit branches reversed so that the ATN
 * simulator will see the exit branch 1st, giving it a priority. The
 * 1st path to the stop state kills any other paths for that rule
 * that begin with the wildcard. In general, this does everything we
 * want, but occasionally there are some quirks as you'll see from
 * the tests below.
 */
[TestClass]
public class TestATNLexerInterpreter {
	[TestMethod] public void testLexerTwoRules(){
		LexerGrammar lg = new LexerGrammar(
			"lexer grammar L;\n"+
			"A : 'a' ;\n" +
			"B : 'b' ;\n");
		String expecting = "A, B, A, B, EOF";
		checkLexerMatches(lg, "abab", expecting);
	}

	[TestMethod] public void testShortLongRule(){
		LexerGrammar lg = new LexerGrammar(
			"lexer grammar L;\n"+
			"A : 'xy'\n" +
			"  | 'xyz'\n" + // this alt is preferred since there are no non-greedy configs
			"  ;\n" +
			"Z : 'z'\n" +
			"  ;\n");
		checkLexerMatches(lg, "xy", "A, EOF");
		checkLexerMatches(lg, "xyz", "A, EOF");
	}

	[TestMethod] public void testShortLongRule2(){
		LexerGrammar lg = new LexerGrammar(
			"lexer grammar L;\n"+
			"A : 'xyz'\n" +  // make sure nongreedy mech cut off doesn't kill this alt
			"  | 'xy'\n" +
			"  ;\n");
		checkLexerMatches(lg, "xy", "A, EOF");
		checkLexerMatches(lg, "xyz", "A, EOF");
	}

	[TestMethod] public void testWildOnEndFirstAlt(){
		LexerGrammar lg = new LexerGrammar(
			"lexer grammar L;\n"+
			"A : 'xy' .\n" + // should pursue '.' since xyz hits stop first, before 2nd alt
			"  | 'xy'\n" +
			"  ;\n" +
			"Z : 'z'\n" +
			"  ;\n");
		checkLexerMatches(lg, "xy", "A, EOF");
		checkLexerMatches(lg, "xyz", "A, EOF");
	}

	[TestMethod] public void testWildOnEndLastAlt(){
		LexerGrammar lg = new LexerGrammar(
			"lexer grammar L;\n"+
			"A : 'xy'\n" +
			"  | 'xy' .\n" +  // this alt is preferred since there are no non-greedy configs
			"  ;\n" +
			"Z : 'z'\n" +
			"  ;\n");
		checkLexerMatches(lg, "xy", "A, EOF");
		checkLexerMatches(lg, "xyz", "A, EOF");
	}

	[TestMethod] public void testWildcardNonQuirkWhenSplitBetweenTwoRules(){
		LexerGrammar lg = new LexerGrammar(
			"lexer grammar L;\n"+
			"A : 'xy' ;\n" +
			"B : 'xy' . 'z' ;\n");
		checkLexerMatches(lg, "xy", "A, EOF");
		checkLexerMatches(lg, "xyqz", "B, EOF");
	}

	[TestMethod] public void testLexerLoops(){
		LexerGrammar lg = new LexerGrammar(
			"lexer grammar L;\n"+
			"INT : '0'..'9'+ ;\n" +
			"ID : 'a'..'z'+ ;\n");
		String expecting = "ID, INT, ID, INT, EOF";
		checkLexerMatches(lg, "a34bde3", expecting);
	}

	[TestMethod] public void testLexerNotSet(){
		LexerGrammar lg = new LexerGrammar(
			"lexer grammar L;\n"+
			"ID : ~('a'|'b')\n ;");
		String expecting = "ID, EOF";
		checkLexerMatches(lg, "c", expecting);
	}

	[TestMethod] public void testLexerSetUnicodeBMP(){
		LexerGrammar lg = new LexerGrammar(
			"lexer grammar L;\n"+
			"ID : ('\u611B'|'\u611C')\n ;");
		String expecting = "ID, EOF";
		checkLexerMatches(lg, "\u611B", expecting);
	}

	[TestMethod] public void testLexerNotSetUnicodeBMP(){
		LexerGrammar lg = new LexerGrammar(
			"lexer grammar L;\n"+
			"ID : ~('\u611B'|'\u611C')\n ;");
		String expecting = "ID, EOF";
		checkLexerMatches(lg, "\u611D", expecting);
	}

		[TestMethod] public void testLexerNotSetUnicodeBMPMatchesSMP(){
		LexerGrammar lg = new LexerGrammar(
			"lexer grammar L;\n"+
			"ID : ~('\u611B'|'\u611C')\n ;");
		String expecting = "ID, EOF";
		checkLexerMatches(lg, new StringBuilder().appendCodePoint(0x1F4A9).ToString(), expecting);
	}

	[TestMethod] public void testLexerSetUnicodeSMP(){
		LexerGrammar lg = new LexerGrammar(
			"lexer grammar L;\n"+
			"ID : ('\\u{1F4A9}'|'\\u{1F4AA}')\n ;");
		String expecting = "ID, EOF";
		checkLexerMatches(lg, new StringBuilder().appendCodePoint(0x1F4A9).ToString(), expecting);
	}

	[TestMethod] public void testLexerNotBMPSetMatchesUnicodeSMP(){
		LexerGrammar lg = new LexerGrammar(
			"lexer grammar L;\n"+
			"ID : ~('a'|'b')\n ;");
		String expecting = "ID, EOF";
		checkLexerMatches(lg, new StringBuilder().appendCodePoint(0x1F4A9).ToString(), expecting);
	}

	[TestMethod] public void testLexerNotBMPSetMatchesBMP(){
		LexerGrammar lg = new LexerGrammar(
			"lexer grammar L;\n"+
			"ID : ~('a'|'b')\n ;");
		String expecting = "ID, EOF";
		checkLexerMatches(lg, "\u611B", expecting);
	}

	[TestMethod] public void testLexerNotBMPSetMatchesSMP(){
		LexerGrammar lg = new LexerGrammar(
			"lexer grammar L;\n"+
			"ID : ~('a'|'b')\n ;");
		String expecting = "ID, EOF";
		checkLexerMatches(lg, new StringBuilder().appendCodePoint(0x1F4A9).ToString(), expecting);
	}

	[TestMethod] public void testLexerNotSMPSetMatchesBMP(){
		LexerGrammar lg = new LexerGrammar(
			"lexer grammar L;\n"+
			"ID : ~('\\u{1F4A9}'|'\\u{1F4AA}')\n ;");
		String expecting = "ID, EOF";
		checkLexerMatches(lg, "\u611B", expecting);
	}

	[TestMethod] public void testLexerNotSMPSetMatchesSMP(){
		LexerGrammar lg = new LexerGrammar(
			"lexer grammar L;\n"+
			"ID : ~('\\u{1F4A9}'|'\\u{1F4AA}')\n ;");
		String expecting = "ID, EOF";
		checkLexerMatches(lg, new StringBuilder().appendCodePoint(0x1D7C0).ToString(), expecting);
	}

	[TestMethod] public void testLexerRangeUnicodeSMP(){
		LexerGrammar lg = new LexerGrammar(
			"lexer grammar L;\n"+
			"ID : ('\\u{1F4A9}'..'\\u{1F4B0}')\n ;");
		String expecting = "ID, EOF";
		checkLexerMatches(lg, new StringBuilder().appendCodePoint(0x1F4AF).ToString(), expecting);
	}

	[TestMethod] public void testLexerRangeUnicodeBMPToSMP(){
		LexerGrammar lg = new LexerGrammar(
			"lexer grammar L;\n"+
			"ID : ('\\u611B'..'\\u{1F4B0}')\n ;");
		String expecting = "ID, EOF";
		checkLexerMatches(lg, new StringBuilder().appendCodePoint(0x12001).ToString(), expecting);
	}

	[TestMethod] public void testLexerKeywordIDAmbiguity(){
		LexerGrammar lg = new LexerGrammar(
			"lexer grammar L;\n"+
			"KEND : 'end' ;\n" +
			"ID : 'a'..'z'+ ;\n" +
			"WS : (' '|'\\n')+ ;");
		String expecting = "ID, EOF";
		//checkLexerMatches(lg, "e", expecting);
		expecting = "KEND, EOF";
		checkLexerMatches(lg, "end", expecting);
		expecting = "ID, EOF";
		checkLexerMatches(lg, "ending", expecting);
		expecting = "ID, WS, KEND, WS, ID, EOF";
		checkLexerMatches(lg, "a end bcd", expecting);
	}

	[TestMethod] public void testLexerRuleRef(){
		LexerGrammar lg = new LexerGrammar(
			"lexer grammar L;\n"+
			"INT : DIGIT+ ;\n" +
			"fragment DIGIT : '0'..'9' ;\n" +
			"WS : (' '|'\\n')+ ;");
		String expecting = "INT, WS, INT, EOF";
		checkLexerMatches(lg, "32 99", expecting);
	}

	[TestMethod] public void testRecursiveLexerRuleRef(){
		LexerGrammar lg = new LexerGrammar(
			"lexer grammar L;\n"+
			"CMT : '/*' (CMT | ~'*')+ '*/' ;\n" +
			"WS : (' '|'\\n')+ ;");
		String expecting = "CMT, WS, CMT, EOF";
		checkLexerMatches(lg, "/* ick */\n/* /*nested*/ */", expecting);
	}

	[TestMethod] public void testRecursiveLexerRuleRefWithWildcard(){
		LexerGrammar lg = new LexerGrammar(
			"lexer grammar L;\n"+
			"CMT : '/*' (CMT | .)*? '*/' ;\n" +
			"WS : (' '|'\\n')+ ;");

		String expecting = "CMT, WS, CMT, WS, EOF";
		checkLexerMatches(lg,
						  "/* ick */\n" +
						  "/* /* */\n" +
						  "/* /*nested*/ */\n",
						  expecting);
	}

	[TestMethod] public void testLexerWildcardGreedyLoopByDefault(){
		LexerGrammar lg = new LexerGrammar(
			"lexer grammar L;\n"+
			"CMT : '//' .* '\\n' ;\n");
		String expecting = "CMT, EOF";
		checkLexerMatches(lg, "//x\n//y\n", expecting);
	}

	[TestMethod] public void testLexerWildcardLoopExplicitNonGreedy(){
		LexerGrammar lg = new LexerGrammar(
			"lexer grammar L;\n"+
			"CMT : '//' .*? '\\n' ;\n");
		String expecting = "CMT, CMT, EOF";
		checkLexerMatches(lg, "//x\n//y\n", expecting);
	}

	[TestMethod] public void testLexerEscapeInString(){
		LexerGrammar lg = new LexerGrammar(
			"lexer grammar L;\n"+
			"STR : '[' ('~' ']' | .)* ']' ;\n");
		checkLexerMatches(lg, "[a~]b]", "STR, EOF");
		checkLexerMatches(lg, "[a]", "STR, EOF");
	}

	[TestMethod] public void testLexerWildcardGreedyPlusLoopByDefault(){
		LexerGrammar lg = new LexerGrammar(
			"lexer grammar L;\n"+
			"CMT : '//' .+ '\\n' ;\n");
		String expecting = "CMT, EOF";
		checkLexerMatches(lg, "//x\n//y\n", expecting);
	}

	[TestMethod] public void testLexerWildcardExplicitNonGreedyPlusLoop(){
		LexerGrammar lg = new LexerGrammar(
			"lexer grammar L;\n"+
			"CMT : '//' .+? '\\n' ;\n");
		String expecting = "CMT, CMT, EOF";
		checkLexerMatches(lg, "//x\n//y\n", expecting);
	}

	// does not fail since ('*/')? can't match and have rule succeed
	[TestMethod] public void testLexerGreedyOptionalShouldWorkAsWeExpect(){
		LexerGrammar lg = new LexerGrammar(
			"lexer grammar L;\n"+
			"CMT : '/*' ('*/')? '*/' ;\n");
		String expecting = "CMT, EOF";
		checkLexerMatches(lg, "/**/", expecting);
	}

	[TestMethod] public void testGreedyBetweenRules(){
		LexerGrammar lg = new LexerGrammar(
			"lexer grammar L;\n"+
			"A : '<a>' ;\n" +
			"B : '<' .+ '>' ;\n");
		String expecting = "B, EOF";
		checkLexerMatches(lg, "<a><x>", expecting);
	}

	[TestMethod] public void testNonGreedyBetweenRules(){
		LexerGrammar lg = new LexerGrammar(
			"lexer grammar L;\n"+
			"A : '<a>' ;\n" +
			"B : '<' .+? '>' ;\n");
		String expecting = "A, B, EOF";
		checkLexerMatches(lg, "<a><x>", expecting);
	}

	[TestMethod] public void testEOFAtEndOfLineComment(){
		LexerGrammar lg = new LexerGrammar(
			"lexer grammar L;\n"+
			"CMT : '//' ~('\\n')* ;\n");
		String expecting = "CMT, EOF";
		checkLexerMatches(lg, "//x", expecting);
	}

	[TestMethod] public void testEOFAtEndOfLineComment2(){
		LexerGrammar lg = new LexerGrammar(
			"lexer grammar L;\n"+
			"CMT : '//' ~('\\n'|'\\r')* ;\n");
		String expecting = "CMT, EOF";
		checkLexerMatches(lg, "//x", expecting);
	}

	/** only positive sets like (EOF|'\n') can match EOF and not in wildcard or ~foo sets
	 *  EOF matches but does not advance cursor.
	 */
	[TestMethod] public void testEOFInSetAtEndOfLineComment(){
		LexerGrammar lg = new LexerGrammar(
			"lexer grammar L;\n"+
			"CMT : '//' .* (EOF|'\\n') ;\n");
		String expecting = "CMT, EOF";
		checkLexerMatches(lg, "//", expecting);
	}

	[TestMethod] public void testEOFSuffixInSecondRule(){
		LexerGrammar lg = new LexerGrammar(
			"lexer grammar L;\n"+
			"A : 'a' ;\n"+ // shorter than 'a' EOF, despite EOF being 0 width
			"B : 'a' EOF ;\n");
		String expecting = "B, EOF";
		checkLexerMatches(lg, "a", expecting);
	}

	[TestMethod] public void testEOFSuffixInFirstRule(){
		LexerGrammar lg = new LexerGrammar(
			"lexer grammar L;\n"+
			"A : 'a' EOF ;\n"+
			"B : 'a';\n");
		String expecting = "A, EOF";
		checkLexerMatches(lg, "a", expecting);
	}

	[TestMethod] public void testEOFByItself(){
		LexerGrammar lg = new LexerGrammar(
			"lexer grammar L;\n"+
			"DONE : EOF ;\n"+
			"A : 'a';\n");
		String expecting = "A, DONE, EOF";
		checkLexerMatches(lg, "a", expecting);
	}

	[TestMethod] public void testLexerCaseInsensitive(){
		LexerGrammar lg = new LexerGrammar(
			"lexer grammar L;\n" +
			"\n" +
			"options { caseInsensitive = true; }\n" +
			"\n" +
			"WS:             [ \\t\\r\\n] -> skip;\n" +
			"\n" +
			"SIMPLE_TOKEN:           'and';\n" +
			"TOKEN_WITH_SPACES:      'as' 'd' 'f';\n" +
			"TOKEN_WITH_DIGITS:      'INT64';\n" +
			"TOKEN_WITH_UNDERSCORE:  'TOKEN_WITH_UNDERSCORE';\n" +
			"BOOL:                   'true' | 'FALSE';\n" +
			"SPECIAL:                '==';\n" +
			"SET:                    [a-z0-9]+;\n" +   // [a-zA-Z0-9]
			"RANGE:                  ('а'..'я')+;"
			);

		String inputString =
			"and AND aND\n" +
			"asdf ASDF\n" +
			"int64\n" +
			"token_WITH_underscore\n" +
			"TRUE FALSE\n" +
			"==\n" +
			"A0bcDE93\n" +
			"АБВабв\n";

		String expecting = RuntimeUtils.join(new String[] {
			"SIMPLE_TOKEN", "SIMPLE_TOKEN", "SIMPLE_TOKEN",
			"TOKEN_WITH_SPACES", "TOKEN_WITH_SPACES",
			"TOKEN_WITH_DIGITS",
			"TOKEN_WITH_UNDERSCORE",
			"BOOL", "BOOL",
			"SPECIAL",
			"SET",
			"RANGE",
			"EOF"
		},
		", WS, ");

		checkLexerMatches(lg, inputString, expecting);
	}

	[TestMethod] public void testLexerCaseInsensitiveLiteralWithNegation() {
		String grammar =
				"lexer grammar L;\n" +
				"options { caseInsensitive = true; }\n" +
				"LITERAL_WITH_NOT:   ~'f';\n";     // ~('f' | 'F)
		ExecutedState executedState = execLexer("L.g4", grammar, "L", "F");

		Assert.AreEqual("line 1:0 token recognition error at: 'F'\n", executedState.errors);
	}

	[TestMethod] public void testLexerCaseInsensitiveSetWithNegation() {
		String grammar =
				"lexer grammar L;\n" +
				"options { caseInsensitive = true; }\n" +
				"SET_WITH_NOT: ~[a-c];\n";        // ~[a-cA-C]
		ExecutedState executedState = execLexer("L.g4", grammar, "L", "B");

		Assert.AreEqual("line 1:0 token recognition error at: 'B'\n", executedState.errors);
	}

	[TestMethod] public void testLexerCaseInsensitiveFragments(){
		LexerGrammar lg = new LexerGrammar(
				"lexer grammar L;\n" +
				"options { caseInsensitive = true; }\n" +
				"TOKEN_0:         FRAGMENT 'd'+;\n" +
				"TOKEN_1:         FRAGMENT 'e'+;\n" +
				"FRAGMENT:        'abc';\n");

		String inputString =
				"ABCDDD";

		String expecting = "TOKEN_0, EOF";

		checkLexerMatches(lg, inputString, expecting);
	}

	[TestMethod] public void testLexerCaseInsensitiveWithDifferentCultures(){
		// From http://www.periodni.com/unicode_utf-8_encoding.html
		LexerGrammar lg = new LexerGrammar(
				"lexer grammar L;\n" +
				"options { caseInsensitive = true; }\n" +
				"ENGLISH_TOKEN:   [a-z]+;\n" +
				"GERMAN_TOKEN:    [äéöüß]+;\n" +
				"FRENCH_TOKEN:    [àâæ-ëîïôœùûüÿ]+;\n" +
				"CROATIAN_TOKEN:  [ćčđšž]+;\n" +
				"ITALIAN_TOKEN:   [àèéìòù]+;\n" +
				"SPANISH_TOKEN:   [áéíñóúü¡¿]+;\n" +
				"GREEK_TOKEN:     [α-ω]+;\n" +
				"RUSSIAN_TOKEN:   [а-я]+;\n" +
				"WS:              [ ]+ -> skip;"
				);

		String inputString = "abcXYZ äéöüßÄÉÖÜß àâæçÙÛÜŸ ćčđĐŠŽ àèéÌÒÙ áéÚÜ¡¿ αβγΧΨΩ абвЭЮЯ ";

		String expecting = RuntimeUtils.join(new String[] {
				"ENGLISH_TOKEN",
				"GERMAN_TOKEN",
				"FRENCH_TOKEN",
				"CROATIAN_TOKEN",
				"ITALIAN_TOKEN",
				"SPANISH_TOKEN",
				"GREEK_TOKEN",
				"RUSSIAN_TOKEN",
				"EOF" },
				", WS, ");

		checkLexerMatches(lg, inputString, expecting);
	}

	[TestMethod] public void testNotImpliedCharactersWithEnabledCaseInsensitiveOption(){
		LexerGrammar lg = new LexerGrammar(
				"lexer grammar L;\n" +
				"options { caseInsensitive = true; }\n" +
				"TOKEN: ('A'..'z')+;\n"
		);

		// No range transformation because of mixed character case in range borders
		String inputString = "ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz";
		checkLexerMatches(lg, inputString, "TOKEN, EOF");
	}

	[TestMethod] public void testCaseInsensitiveInLexerRule(){
		LexerGrammar lg = new LexerGrammar(
				"lexer grammar L;\n" +
				"TOKEN1 options { caseInsensitive=true; } : [a-f]+;\n" +
				"WS: [ ]+ -> skip;"
		);

		checkLexerMatches(lg, "ABCDEF", "TOKEN1, EOF");
	}

	[TestMethod] public void testCaseInsensitiveInLexerRuleOverridesGlobalValue() {
		String grammar =
				"lexer grammar L;\n" +
				"options { caseInsensitive=true; }\n" +
				"STRING options { caseInsensitive=false; } : 'N'? '\\'' (~'\\'' | '\\'\\'')* '\\'';\n";

		ExecutedState executedState = execLexer("L.g4", grammar, "L", "n'sample'");
		Assert.AreEqual("line 1:0 token recognition error at: 'n'\n", executedState.errors);
	}

	private void checkLexerMatches(LexerGrammar lg, String inputString, String expecting) {
		ATN atn = createATN(lg, true);
		CharStream input = CharStreams.fromString(inputString);
		ATNState startState = atn.modeNameToStartState.get("DEFAULT_MODE");
		DOTGenerator dot = new DOTGenerator(lg);
//		Console.Out.WriteLine(dot.getDOT(startState, true));

		List<String> tokenTypes = getTokenTypes(lg, atn, input);

		String result = RuntimeUtils.join(tokenTypes, ", ");
//		Console.Out.WriteLine(tokenTypes);
		Assert.AreEqual(expecting, result);
	}

	private static List<String> getTokenTypes(LexerGrammar lg, ATN atn, CharStream input) {
		LexerATNSimulator interp = new LexerATNSimulator(atn, new DFA[]{new DFA(atn.modeToStartState[(Lexer.DEFAULT_MODE)])}, null);
		List<String> tokenTypes = new ();
		int ttype;
		bool hitEOF = false;
		do {
			if ( hitEOF ) {
				tokenTypes.Add("EOF");
				break;
			}
			int t = input.LA(1);
			ttype = interp.match(input, Lexer.DEFAULT_MODE);
			if ( ttype== Token.EOF ) {
				tokenTypes.Add("EOF");
			}
			else {
				tokenTypes.Add(lg.typeToTokenList[(ttype)]);
			}

			if ( t== IntStream.EOF ) {
				hitEOF = true;
			}
		} while ( ttype!=Token.EOF );
		return tokenTypes;
	}
}
