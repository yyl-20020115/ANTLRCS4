/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.test.runtime.states;

namespace org.antlr.v4.test.tool;


/** Test parser execution.
 *
 *  For the non-greedy stuff, the rule is that .* or any other non-greedy loop
 *  (any + or * loop that has an alternative with '.' in it is automatically
 *  non-greedy) never sees past the end of the rule containing that loop.
 *  There is no automatic way to detect when the exit branch of a non-greedy
 *  loop has seen enough input to determine how much the loop should consume
 *  yet still allow matching the entire input. Of course, this is extremely
 *  inefficient, particularly for things like
 *
 *     block : '{' (block|.)* '}' ;
 *
 *  that need only see one symbol to know when it hits a '}'. So, I
 *  came up with a practical solution.  During prediction, the ATN
 *  simulator never fall off the end of a rule to compute the global
 *  FOLLOW. Instead, we terminate the loop, choosing the exit branch.
 *  Otherwise, we predict to reenter the loop.  For example, input
 *  "{ foo }" will allow the loop to match foo, but that's it. During
 *  prediction, the ATN simulator will see that '}' reaches the end of a
 *  rule that contains a non-greedy loop and stop prediction. It will choose
 *  the exit branch of the inner loop. So, the way in which you construct
 *  the rule containing a non-greedy loop dictates how far it will scan ahead.
 *  Include everything after the non-greedy loop that you know it must scan
 *  in order to properly make a prediction decision. these beasts are tricky,
 *  so be careful. don't liberally sprinkle them around your code.
 *
 *  To simulate filter mode, use ( .* (pattern1|pattern2|...) )*
 *
 *  Nongreedy loops match as much input as possible while still allowing
 *  the remaining input to match.
 */
[TestClass]
public class TestParserExec {
	/**
	 * This is a regression test for antlr/antlr4#118.
	 * https://github.com/antlr/antlr4/issues/118
	 */
	//@Disabled("Performance impact of passing this test may not be worthwhile")
	// TODO: port to test framework (not ported because test currently fails)
	[TestMethod] public void testStartRuleWithoutEOF() {
		String grammar =
			"grammar T;\n"+
			"s @after {dumpDFA();}\n" +
			"  : ID | ID INT ID ;\n" +
			"ID : 'a'..'z'+ ;\n"+
			"INT : '0'..'9'+ ;\n"+
			"WS : (' '|'\\t'|'\\n')+ -> skip ;\n";
		ExecutedState executedState = ToolTestUtils.execParser("T.g4", grammar, "TParser", "TLexer",
				"s", "abc 34", true);
		String expecting =
			"Decision 0:\n" +
			"s0-ID->s1\n" +
			"s1-INT->s2\n" +
			"s2-EOF->:s3=>1\n"; // Must point at accept state
		Assert.AreEqual(expecting, executedState.output);
		Assert.AreEqual("", executedState.errors);
	}

	/**
	 * This is a regression test for antlr/antlr4#588 "ClassCastException during
	 * semantic predicate handling".
	 * https://github.com/antlr/antlr4/issues/588
	 */
	// TODO: port to test framework (can we simplify the Psl grammar?)
	[TestMethod] public void testFailedPredicateExceptionState(){
		String grammar = load("Psl.g4");
		ExecutedState executedState = ToolTestUtils.execParser("Psl.g4", grammar,
				"PslParser", "PslLexer", "floating_constant", " . 234", false);
		Assert.AreEqual("", executedState.output);
		Assert.AreEqual("line 1:6 rule floating_constant DEC:A floating-point constant cannot have internal white space\n", executedState.errors);
	}

	/**
	 * This is a regression test for antlr/antlr4#563 "Inconsistent token
	 * handling in ANTLR4".
	 * https://github.com/antlr/antlr4/issues/563
	 */
	// TODO: port to test framework (missing templates)
	[TestMethod] public void testAlternateQuotes( Path tempDir) {
		String lexerGrammar =
			"lexer grammar ModeTagsLexer;\n" +
			"\n" +
			"// Default mode rules (the SEA)\n" +
			"OPEN  : '«'     -> mode(ISLAND) ;       // switch to ISLAND mode\n" +
			"TEXT  : ~'«'+ ;                         // clump all text together\n" +
			"\n" +
			"mode ISLAND;\n" +
			"CLOSE : '»'     -> mode(DEFAULT_MODE) ; // back to SEA mode \n" +
			"SLASH : '/' ;\n" +
			"ID    : [a-zA-Z]+ ;                     // match/send ID in tag to parser\n";
		String parserGrammar =
			"parser grammar ModeTagsParser;\n" +
			"\n" +
			"options { tokenVocab=ModeTagsLexer; } // use tokens from ModeTagsLexer.g4\n" +
			"\n" +
			"file: (tag | TEXT)* ;\n" +
			"\n" +
			"tag : '«' ID '»'\n" +
			"    | '«' '/' ID '»'\n" +
			"    ;";

        ToolTestUtils.execLexer("ModeTagsLexer.g4", lexerGrammar, "ModeTagsLexer", "",
				tempDir, true);
		ExecutedState executedState = ToolTestUtils.execParser("ModeTagsParser.g4", parserGrammar,
				"ModeTagsParser", "ModeTagsLexer",
				"file", "", false,
				tempDir);
		Assert.AreEqual("", executedState.output);
		Assert.AreEqual("", executedState.errors);
	}

	/**
	 * This is a regression test for antlr/antlr4#672 "Initialization failed in
	 * locals".
	 * https://github.com/antlr/antlr4/issues/672
	 */
	// TODO: port to test framework (missing templates)
	[TestMethod] public void testAttributeValueInitialization() {
		String grammar =
			"grammar Data; \n" +
			"\n" +
			"file : group+ EOF; \n" +
			"\n" +
			"group: INT sequence {outStream.println($sequence.values.size());} ; \n" +
			"\n" +
			"sequence returns [List<Integer> values = new ArrayList<Integer>()] \n" +
			"  locals[List<Integer> localValues = new ArrayList<Integer>()]\n" +
			"         : (INT {$localValues.add($INT.int);})* {$values.addAll($localValues);}\n" +
			"; \n" +
			"\n" +
			"INT : [0-9]+ ; // match integers \n" +
			"WS : [ \\t\\n\\r]+ -> skip ; // toss out all whitespace\n";

		String input = "2 9 10 3 1 2 3";
		ExecutedState executedState = ToolTestUtils.execParser("Data.g4", grammar,
				"DataParser", "DataLexer", "file", input, false);
		Assert.AreEqual("6\n", executedState.output);
		Assert.AreEqual("", executedState.errors);
	}

	[TestMethod] public void testCaseInsensitiveInCombinedGrammar(){
		String grammar =
				"grammar CaseInsensitiveGrammar;\n" +
				"options { caseInsensitive = true; }\n" +
				"e\n" +
				"    : ID\n" +
				"    | 'not' e\n" +
				"    | e 'and' e\n" +
				"    | 'new' ID '(' e ')'\n" +
				"    ;\n" +
				"ID: [a-z_][a-z_0-9]*;\n" +
				"WS: [ \\t\\n\\r]+ -> skip;";

		String input = "NEW Abc (Not a AND not B)";
		ExecutedState executedState = ToolTestUtils.execParser(
				"CaseInsensitiveGrammar.g4", grammar,
				"CaseInsensitiveGrammarParser", "CaseInsensitiveGrammarLexer",
				"e", input, false);
		Assert.AreEqual("", executedState.output);
		Assert.AreEqual("", executedState.errors);
	}
}
