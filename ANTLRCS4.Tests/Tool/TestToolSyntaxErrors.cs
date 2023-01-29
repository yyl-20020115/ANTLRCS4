/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.tool;

namespace org.antlr.v4.test.tool;

[TestClass]
public class TestToolSyntaxErrors {
	readonly static String[] A = new string[] {
	    // INPUT
		"grammar A;\n" +
		"",
		// YIELDS
		"error(" + ErrorType.NO_RULES + "): A.g4::: grammar A has no rules\n",

		"lexer grammar A;\n" +
		"",
		"error(" + ErrorType.NO_RULES + "): A.g4::: grammar A has no rules\n",

		"A;",
		"error(" + ErrorType.SYNTAX_ERROR + "): A.g4:1:0: syntax error: 'A' came as a complete surprise to me\n",

		"grammar ;",
		"error(" + ErrorType.SYNTAX_ERROR + "): A.g4:1:8: syntax error: ';' came as a complete surprise to me while looking for an identifier\n",

		"grammar A\n" +
		"a : ID ;\n",
		"error(" + ErrorType.SYNTAX_ERROR + "): A.g4:2:0: syntax error: missing SEMI at 'a'\n",

		"grammar A;\n" +
		"a : ID ;;\n"+
		"b : B ;",
		"error(" + ErrorType.SYNTAX_ERROR + "): A.g4:2:8: syntax error: ';' came as a complete surprise to me\n",

		"grammar A;;\n" +
		"a : ID ;\n",
		"error(" + ErrorType.SYNTAX_ERROR + "): A;.g4:1:10: syntax error: ';' came as a complete surprise to me\n",

		"grammar A;\n" +
		"a @init : ID ;\n",
		"error(" + ErrorType.SYNTAX_ERROR + "): A.g4:2:8: syntax error: mismatched input ':' expecting ACTION while matching rule preamble\n",

		"grammar A;\n" +
		"a  ( A | B ) D ;\n" +
		"b : B ;",
		"error(" + ErrorType.SYNTAX_ERROR + "): A.g4:2:3: syntax error: '(' came as a complete surprise to me while matching rule preamble\n" +
		"error(" + ErrorType.SYNTAX_ERROR + "): A.g4:2:11: syntax error: mismatched input ')' expecting SEMI while matching a rule\n" +
		"error(" + ErrorType.SYNTAX_ERROR + "): A.g4:2:15: syntax error: mismatched input ';' expecting COLON while matching a lexer rule\n",
    };

	[TestMethod]
	public void AllErrorCodesDistinct() {
		ErrorType[] errorTypes = ErrorType.ErrorTypes;
		for (int i = 0; i < errorTypes.Length; i++) {
			for (int j = i + 1; j < errorTypes.Length; j++) {
				Assert.AreNotEqual(errorTypes[i], errorTypes[j]);
			}
		}
	}

	[TestMethod] public void testA() { ToolTestUtils.testErrors(A, true); }

	[TestMethod] public void testExtraColon() {
		String[] pair = new String[] {
			"grammar A;\n" +
			"a : : A ;\n" +
			"b : B ;",
			"error(" + ErrorType.SYNTAX_ERROR + "): A.g4:2:4: syntax error: ':' came as a complete surprise to me while matching alternative\n",
		};
		ToolTestUtils.testErrors(pair, true);
	}

	[TestMethod] public void testMissingRuleSemi() {
		String[] pair = new String[] {
			"grammar A;\n" +
			"a : A \n" +
			"b : B ;",
			"error(" + ErrorType.SYNTAX_ERROR + "): A.g4:3:0: syntax error: unterminated rule (missing ';') detected at 'b :' while looking for rule element\n",
		};
		ToolTestUtils.testErrors(pair, true);
	}

	[TestMethod] public void testMissingRuleSemi2() {
		String[] pair = new String[] {
			"lexer grammar A;\n" +
			"A : 'a' \n" +
			"B : 'b' ;",
			"error(" + ErrorType.SYNTAX_ERROR + "): A.g4:3:2: syntax error: unterminated rule (missing ';') detected at ': 'b'' while looking for lexer rule element\n",
		};
		ToolTestUtils.testErrors(pair, true);
	}

	[TestMethod] public void testMissingRuleSemi3() {
		String[] pair = new String[] {
			"grammar A;\n" +
			"a : A \n" +
			"b[int i] returns [int y] : B ;",
			"error(" + ErrorType.SYNTAX_ERROR + "): A.g4:3:9: syntax error: unterminated rule (missing ';') detected at 'returns int y' while looking for rule element\n"
		};
		ToolTestUtils.testErrors(pair, true);
	}

	[TestMethod] public void testMissingRuleSemi4() {
		String[] pair = new String[] {
			"grammar A;\n" +
			"a : b \n" +
			"  catch [Exception e] {...}\n" +
			"b : B ;\n",

			"error(" + ErrorType.SYNTAX_ERROR + "): A.g4:2:4: syntax error: unterminated rule (missing ';') detected at 'b catch' while looking for rule element\n"
		};
		ToolTestUtils.testErrors(pair, true);
	}

	[TestMethod] public void testMissingRuleSemi5() {
		String[] pair = new String[] {
			"grammar A;\n" +
			"a : A \n" +
			"  catch [Exception e] {...}\n",

			"error(" + ErrorType.SYNTAX_ERROR + "): A.g4:2:4: syntax error: unterminated rule (missing ';') detected at 'A catch' while looking for rule element\n"
		};
		ToolTestUtils.testErrors(pair, true);
	}

	[TestMethod] public void testBadRulePrequelStart() {
		String[] pair = new String[] {
			"grammar A;\n" +
			"a @ options {k=1;} : A ;\n" +
			"b : B ;",

			"error(" + ErrorType.SYNTAX_ERROR + "): A.g4:2:4: syntax error: 'options {' came as a complete surprise to me while looking for an identifier\n"
		};
		ToolTestUtils.testErrors(pair, true);
	}

	[TestMethod] public void testBadRulePrequelStart2() {
		String[] pair = new String[] {
			"grammar A;\n" +
			"a } : A ;\n" +
			"b : B ;",

			"error(" + ErrorType.SYNTAX_ERROR + "): A.g4:2:2: syntax error: '}' came as a complete surprise to me while matching rule preamble\n"
		};
		ToolTestUtils.testErrors(pair, true);
	}

	[TestMethod] public void testModeInParser() {
		String[] pair = new String[] {
			"grammar A;\n" +
			"a : A ;\n" +
			"mode foo;\n" +
			"b : B ;",

			"error(" + ErrorType.SYNTAX_ERROR + "): A.g4:4:0: syntax error: 'b' came as a complete surprise to me\n" +
			"error(" + ErrorType.SYNTAX_ERROR + "): A.g4:4:6: syntax error: mismatched input ';' expecting COLON while matching a lexer rule\n"
		};
		ToolTestUtils.testErrors(pair, true);
	}

	/**
	 * This is a regression test for antlr/antlr4#243
	 * "Generate a good message for unterminated strings"
	 * https://github.com/antlr/antlr4/issues/243
	 */
	[TestMethod] public void testUnterminatedStringLiteral() {
		String[] pair = new String[] {
			"grammar A;\n" +
			"a : 'x\n" +
			"  ;\n",

			"error(" + ErrorType.UNTERMINATED_STRING_LITERAL + "): A.g4:2:4: unterminated string literal\n"
		};
		ToolTestUtils.testErrors(pair, true);
	}

	/**
	 * This is a regression test for antlr/antlr4#262
	 * "Parser Rule Name Starting With an Underscore"
	 * https://github.com/antlr/antlr4/issues/262
	 */
	[TestMethod] public void testParserRuleNameStartingWithUnderscore() {
		String[] pair = new String[] {
			"grammar A;\n" +
			"_a : 'x' ;\n",

			"error(" + ErrorType.SYNTAX_ERROR + "): A.g4:2:0: syntax error: '_' came as a complete surprise to me\n"
		};
		ToolTestUtils.testErrors(pair, true);
	}

	/**
	 * This is a regression test for antlr/antlr4#194
	 * "NullReferenceException on 'options{}' in grammar file"
	 * https://github.com/antlr/antlr4/issues/194
	 */
	[TestMethod] public void testEmptyGrammarOptions() {
		String[] pair = new String[] {
			"grammar A;\n" +
			"options {}\n" +
			"a : 'x' ;\n",

			""
		};
		ToolTestUtils.testErrors(pair, true);
	}

	/**
	 * This is a "related" regression test for antlr/antlr4#194
	 * "NullReferenceException on 'options{}' in grammar file"
	 * https://github.com/antlr/antlr4/issues/194
	 */
	[TestMethod] public void testEmptyRuleOptions() {
		String[] pair = new String[] {
			"grammar A;\n" +
			"a options{} : 'x' ;\n",

			""
		};
		ToolTestUtils.testErrors(pair, true);
	}

	/**
	 * This is a "related" regression test for antlr/antlr4#194
	 * "NullReferenceException on 'options{}' in grammar file"
	 * https://github.com/antlr/antlr4/issues/194
	 */
	[TestMethod] public void testEmptyBlockOptions() {
		String[] pair = new String[] {
			"grammar A;\n" +
			"a : (options{} : 'x') ;\n",

			""
		};
		ToolTestUtils.testErrors(pair, true);
	}

	[TestMethod] public void testEmptyTokensBlock() {
		String[] pair = new String[] {
			"grammar A;\n" +
			"tokens {}\n" +
			"a : 'x' ;\n",

			""
		};
		ToolTestUtils.testErrors(pair, true);
	}

	/**
	 * This is a regression test for antlr/antlr4#190
	 * "NullReferenceException building lexer grammar using bogus 'token' action"
	 * https://github.com/antlr/antlr4/issues/190
	 */
	[TestMethod] public void testInvalidLexerCommand() {
		String[] pair = new String[] {
			"grammar A;\n" +
			"tokens{Foo}\n" +
			"b : Foo ;\n" +
			"X : 'foo1' -> popmode;\n" + // "meant" to use -> popMode
			"Y : 'foo2' -> token(Foo);", // "meant" to use -> type(Foo)

			"error(" + ErrorType.INVALID_LEXER_COMMAND + "): A.g4:4:14: lexer command popmode does not exist or is not supported by the current target\n" +
			"error(" + ErrorType.INVALID_LEXER_COMMAND + "): A.g4:5:14: lexer command token does not exist or is not supported by the current target\n"
		};
		ToolTestUtils.testErrors(pair, true);
	}

	[TestMethod] public void testLexerCommandArgumentValidation() {
		String[] pair = new String[] {
			"grammar A;\n" +
			"tokens{Foo}\n" +
			"b : Foo ;\n" +
			"X : 'foo1' -> popMode(Foo);\n" + // "meant" to use -> popMode
			"Y : 'foo2' -> type;", // "meant" to use -> type(Foo)

			"error(" + ErrorType.UNWANTED_LEXER_COMMAND_ARGUMENT + "): A.g4:4:14: lexer command popMode does not take any arguments\n" +
			"error(" + ErrorType.MISSING_LEXER_COMMAND_ARGUMENT + "): A.g4:5:14: missing argument for lexer command type\n"
		};
		ToolTestUtils.testErrors(pair, true);
	}

	[TestMethod] public void testRuleRedefinition() {
		String[] pair = new String[] {
			"grammar Oops;\n" +
			"\n" +
			"ret_ty : A ;\n" +
			"ret_ty : B ;\n" +
			"\n" +
			"A : 'a' ;\n" +
			"B : 'b' ;\n",

			"error(" + ErrorType.RULE_REDEFINITION + "): Oops.g4:4:0: rule ret_ty redefinition; previous at line 3\n"
		};
		ToolTestUtils.testErrors(pair, true);
	}

	[TestMethod] public void testEpsilonClosureAnalysis() {
		String grammar =
			"grammar A;\n"
			+ "x : ;\n"
			+ "y1 : x+;\n"
			+ "y2 : x*;\n"
			+ "z1 : ('foo' | 'bar'? 'bar2'?)*;\n"
			+ "z2 : ('foo' | 'bar' 'bar2'? | 'bar2')*;\n";
		String expected =
			"error(" + ErrorType.EPSILON_CLOSURE + "): A.g4:3:0: rule y1 contains a closure with at least one alternative that can match an empty string\n" +
			"error(" + ErrorType.EPSILON_CLOSURE + "): A.g4:4:0: rule y2 contains a closure with at least one alternative that can match an empty string\n" +
			"error(" + ErrorType.EPSILON_CLOSURE + "): A.g4:5:0: rule z1 contains a closure with at least one alternative that can match an empty string\n";

		String[] pair = new String[] {
			grammar,
			expected
		};

		ToolTestUtils.testErrors(pair, true);
	}

	// Test for https://github.com/antlr/antlr4/issues/1203
	[TestMethod] public void testEpsilonNestedClosureAnalysis() {
		String grammar =
			"grammar T;\n"+
			"s : (a a)* ;\n"+
			"a : 'foo'* ;\n";
		String expected =
			"error(" + ErrorType.EPSILON_CLOSURE + "): T.g4:2:0: rule s contains a closure with at least one alternative that can match an empty string\n";

		String[] pair = new String[] {
			grammar,
			expected
		};

		ToolTestUtils.testErrors(pair, true);
	}

	// Test for https://github.com/antlr/antlr4/issues/2860, https://github.com/antlr/antlr4/issues/1105
	[TestMethod] public void testEpsilonClosureInLexer() {
		String grammar =
				"lexer grammar T;\n" +
				"TOKEN: '\\'' FRAGMENT '\\'';\n" +
				"fragment FRAGMENT: ('x'|)+;";

		String expected =
			"error(" + ErrorType.EPSILON_CLOSURE + "): T.g4:3:9: rule FRAGMENT contains a closure with at least one alternative that can match an empty string\n";

		String[] pair = new String[] {
				grammar,
				expected
		};

		ToolTestUtils.testErrors(pair, true);
	}

	// Test for https://github.com/antlr/antlr4/issues/3359
	[TestMethod] public void testEofClosure() {
		String grammar =
				"lexer grammar EofClosure;\n" +
				"EofClosure: 'x' EOF*;\n" +
				"EofInAlternative: 'y' ('z' | EOF);";

		String expected =
			"error(" + ErrorType.EOF_CLOSURE + "): EofClosure.g4:2:0: rule EofClosure contains a closure with at least one alternative that can match EOF\n";

		String[] pair = new String[] {
				grammar,
				expected
		};

		ToolTestUtils.testErrors(pair, true);
	}

	// Test for https://github.com/antlr/antlr4/issues/1203
	[TestMethod] public void testEpsilonOptionalAndClosureAnalysis() {
		String grammar =
			"grammar T;\n"+
			"s : (a a)? ;\n"+
			"a : 'foo'* ;\n";
		String expected =
			"warning(" + ErrorType.EPSILON_OPTIONAL + "): T.g4:2:0: rule s contains an optional block with at least one alternative that can match an empty string\n";

		String[] pair = new String[] {
			grammar,
			expected
		};

		ToolTestUtils.testErrors(pair, true);
	}

	[TestMethod] public void testEpsilonOptionalAnalysis() {
		String grammar =
			"grammar A;\n"
			+ "x : ;\n"
			+ "y  : x?;\n"
			+ "z1 : ('foo' | 'bar'? 'bar2'?)?;\n"
			+ "z2 : ('foo' | 'bar' 'bar2'? | 'bar2')?;\n";
		String expected =
			"warning(" + ErrorType.EPSILON_OPTIONAL + "): A.g4:3:0: rule y contains an optional block with at least one alternative that can match an empty string\n" +
			"warning(" + ErrorType.EPSILON_OPTIONAL + "): A.g4:4:0: rule z1 contains an optional block with at least one alternative that can match an empty string\n";

		String[] pair = new String[] {
			grammar,
			expected
		};

		ToolTestUtils.testErrors(pair, true);
	}

	/**
	 * This is a regression test for antlr/antlr4#315
	 * "Inconsistent lexer error msg for actions"
	 * https://github.com/antlr/antlr4/issues/315
	 */
	[TestMethod] public void testActionAtEndOfOneLexerAlternative() {
		String grammar =
			"grammar A;\n" +
			"stat : 'start' CharacterLiteral 'end' EOF;\n" +
			"\n" +
			"// Lexer\n" +
			"\n" +
			"CharacterLiteral\n" +
			"    :   '\\'' SingleCharacter '\\''\n" +
			"    |   '\\'' ~[\\r\\n] {notifyErrorListeners(\"unclosed character literal\");}\n" +
			"    ;\n" +
			"\n" +
			"fragment\n" +
			"SingleCharacter\n" +
			"    :   ~['\\\\\\r\\n]\n" +
			"    ;\n" +
			"\n" +
			"WS   : [ \\r\\t\\n]+ -> skip ;\n";
		String expected =
			"";

		String[] pair = new String[] { grammar, expected };
		ToolTestUtils.testErrors(pair, true);
	}

	/**
	 * This is a regression test for antlr/antlr4#308 "NullPointer exception"
	 * https://github.com/antlr/antlr4/issues/308
	 */
	[TestMethod] public void testDoubleQuotedStringLiteral() {
		String grammar =
			"lexer grammar A;\n"
			+ "WHITESPACE : (\" \" | \"\\t\" | \"\\n\" | \"\\r\" | \"\\f\");\n";
		String expected =
			"error(" + ErrorType.SYNTAX_ERROR + "): A.g4:2:14: syntax error: '\"' came as a complete surprise to me\n" +
			"error(" + ErrorType.SYNTAX_ERROR + "): A.g4:2:16: syntax error: '\"' came as a complete surprise to me\n" +
			"error(" + ErrorType.SYNTAX_ERROR + "): A.g4:2:20: syntax error: '\"' came as a complete surprise to me\n" +
			"error(" + ErrorType.SYNTAX_ERROR + "): A.g4:2:21: syntax error: '\\' came as a complete surprise to me\n" +
			"error(" + ErrorType.SYNTAX_ERROR + "): A.g4:2:23: syntax error: '\"' came as a complete surprise to me\n" +
			"error(" + ErrorType.SYNTAX_ERROR + "): A.g4:2:27: syntax error: '\"' came as a complete surprise to me\n" +
			"error(" + ErrorType.SYNTAX_ERROR + "): A.g4:2:28: syntax error: '\\' came as a complete surprise to me\n" +
			"error(" + ErrorType.SYNTAX_ERROR + "): A.g4:2:30: syntax error: '\"' came as a complete surprise to me\n" +
			"error(" + ErrorType.SYNTAX_ERROR + "): A.g4:2:34: syntax error: '\"' came as a complete surprise to me\n" +
			"error(" + ErrorType.SYNTAX_ERROR + "): A.g4:2:35: syntax error: '\\' came as a complete surprise to me\n" +
			"error(" + ErrorType.SYNTAX_ERROR + "): A.g4:2:37: syntax error: '\"' came as a complete surprise to me\n" +
			"error(" + ErrorType.SYNTAX_ERROR + "): A.g4:2:41: syntax error: '\"' came as a complete surprise to me\n" +
			"error(" + ErrorType.SYNTAX_ERROR + "): A.g4:2:42: syntax error: '\\' came as a complete surprise to me\n" +
			"error(" + ErrorType.SYNTAX_ERROR + "): A.g4:2:44: syntax error: '\"' came as a complete surprise to me\n";

		String[] pair = new String[] {
			grammar,
			expected
		};

			ToolTestUtils.testErrors(pair, true);
	}

	/**
	 * This is a regression test for https://github.com/antlr/antlr4/issues/1815
	 * "Null ptr exception in SqlBase.g4"
	 */
	[TestMethod] public void testDoubleQuoteInTwoStringLiterals() {
		String grammar =
			"lexer grammar A;\n" +
			"STRING : '\\\"' '\\\"' 'x' ;";
		String expected =
			"error(" + ErrorType.INVALID_ESCAPE_SEQUENCE + "): A.g4:2:10: invalid escape sequence \\\"\n"+
			"error(" + ErrorType.INVALID_ESCAPE_SEQUENCE + "): A.g4:2:15: invalid escape sequence \\\"\n";

		String[] pair = new String[] {
			grammar,
			expected
		};

		ToolTestUtils.testErrors(pair, true);
	}

	/**
	 * This test ensures that the {@link ErrorType#INVALID_ESCAPE_SEQUENCE}
	 * error is not reported for escape sequences that are known to be valid.
	 */
	[TestMethod] public void testValidEscapeSequences() {
		String grammar =
			"lexer grammar A;\n" +
			"NORMAL_ESCAPE : '\\b \\t \\n \\f \\r \\' \\\\';\n" +
			"UNICODE_ESCAPE : '\\u0001 \\u00A1 \\u00a1 \\uaaaa \\uAAAA';\n";
		String expected =
			"";

		String[] pair = new String[] {
			grammar,
			expected
		};

		ToolTestUtils.testErrors(pair, true);
	}

	/**
	 * This is a regression test for antlr/antlr4#507 "NullReferenceException When
	 * Generating Code from Grammar".
	 * https://github.com/antlr/antlr4/issues/507
	 */
	[TestMethod] public void testInvalidEscapeSequences() {
		String grammar =
			"lexer grammar A;\n" +
			"RULE : 'Foo \\uAABG \\x \\u';\n";
		String expected =
			"error(" + ErrorType.INVALID_ESCAPE_SEQUENCE + "): A.g4:2:12: invalid escape sequence \\uAABG\n" +
			"error(" + ErrorType.INVALID_ESCAPE_SEQUENCE + "): A.g4:2:19: invalid escape sequence \\x\n" +
			"error(" + ErrorType.INVALID_ESCAPE_SEQUENCE + "): A.g4:2:22: invalid escape sequence \\u\n";

		String[] pair = new String[] {
			grammar,
			expected
		};

		ToolTestUtils.testErrors(pair, true);
	}

	/**
	 * This is a regression test for antlr/antlr4#959 "NullReferenceException".
	 * https://github.com/antlr/antlr4/issues/959
	 */
	[TestMethod] public void testNotAllowedEmptyStrings() {
		String grammar =
			"lexer grammar T;\n" +
			"Error0: '''test''';\n" +
			"Error1: '' 'test';\n" +
			"Error2: 'test' '';\n" +
			"Error3: '';\n" +
			"NotError: ' ';";
		String expected =
			"error(" + ErrorType.EMPTY_STRINGS_AND_SETS_NOT_ALLOWED + "): T.g4:2:8: string literals and sets cannot be empty: ''\n" +
			"error(" + ErrorType.EMPTY_STRINGS_AND_SETS_NOT_ALLOWED + "): T.g4:2:16: string literals and sets cannot be empty: ''\n" +
			"error(" + ErrorType.EMPTY_STRINGS_AND_SETS_NOT_ALLOWED + "): T.g4:3:8: string literals and sets cannot be empty: ''\n" +
			"error(" + ErrorType.EMPTY_STRINGS_AND_SETS_NOT_ALLOWED + "): T.g4:4:15: string literals and sets cannot be empty: ''\n" +
			"error(" + ErrorType.EMPTY_STRINGS_AND_SETS_NOT_ALLOWED + "): T.g4:5:8: string literals and sets cannot be empty: ''\n";

		String[] pair = new String[] {
				grammar,
				expected
		};

		ToolTestUtils.testErrors(pair, true);
	}


    [TestMethod] public void testInvalidCharSetsAndStringLiterals() {
		String grammar =
				"lexer grammar Test;\n" +
				"INVALID_STRING_LITERAL_RANGE: 'GH'..'LM';\n" +
				"INVALID_CHAR_SET:             [\\u24\\uA2][\\{];\n" +  //https://github.com/antlr/antlr4/issues/1077
				"EMPTY_STRING_LITERAL_RANGE:   'F'..'A' | 'Z';\n" +
				"EMPTY_CHAR_SET:               [f-az][];\n" +
				"START_HYPHEN_IN_CHAR_SET:     [-z];\n" +
				"END_HYPHEN_IN_CHAR_SET:       [a-];\n" +
				"SINGLE_HYPHEN_IN_CHAR_SET:    [-];\n" +
				"VALID_STRING_LITERALS:        '\\u1234' | '\\t' | '\\'';\n" +
				"VALID_CHAR_SET:               [`\\-=\\]];" +
				"EMPTY_CHAR_SET_WITH_INVALID_ESCAPE_SEQUENCE: [\\'];";  // https://github.com/antlr/antlr4/issues/1556

		String expected =
				"error(" + ErrorType.INVALID_LITERAL_IN_LEXER_SET + "): Test.g4:2:30: multi-character literals are not allowed in lexer sets: 'GH'\n" +
				"error(" + ErrorType.INVALID_LITERAL_IN_LEXER_SET + "): Test.g4:2:36: multi-character literals are not allowed in lexer sets: 'LM'\n" +
				"error(" + ErrorType.INVALID_ESCAPE_SEQUENCE + "): Test.g4:3:30: invalid escape sequence \\u24\\u\n" +
				"error(" + ErrorType.INVALID_ESCAPE_SEQUENCE + "): Test.g4:3:40: invalid escape sequence \\{\n" +
				"error(" + ErrorType.EMPTY_STRINGS_AND_SETS_NOT_ALLOWED + "): Test.g4:4:33: string literals and sets cannot be empty: 'F'..'A'\n" +
				"error(" + ErrorType.EMPTY_STRINGS_AND_SETS_NOT_ALLOWED + "): Test.g4:5:30: string literals and sets cannot be empty: 'f'..'a'\n" +
				"error(" + ErrorType.EMPTY_STRINGS_AND_SETS_NOT_ALLOWED + "): Test.g4:5:36: string literals and sets cannot be empty: []\n" +
				"error(" + ErrorType.INVALID_ESCAPE_SEQUENCE + "): Test.g4:10:84: invalid escape sequence \\'\n";

		String[] pair = new String[] {
				grammar,
				expected
		};

		ToolTestUtils.testErrors(pair, true);
	}

	[TestMethod] public void testInvalidUnicodeEscapesInCharSet() {
		String grammar =
				"lexer grammar Test;\n" +
				"INVALID_EXTENDED_UNICODE_EMPTY: [\\u{}];\n" +
				"INVALID_EXTENDED_UNICODE_NOT_TERMINATED: [\\u{];\n" +
				"INVALID_EXTENDED_UNICODE_TOO_LONG: [\\u{110000}];\n" +
				"INVALID_UNICODE_PROPERTY_EMPTY: [\\p{}];\n" +
				"INVALID_UNICODE_PROPERTY_NOT_TERMINATED: [\\p{];\n" +
				"INVALID_INVERTED_UNICODE_PROPERTY_EMPTY: [\\P{}];\n" +
				"INVALID_UNICODE_PROPERTY_UNKNOWN: [\\p{NotAProperty}];\n" +
				"INVALID_INVERTED_UNICODE_PROPERTY_UNKNOWN: [\\P{NotAProperty}];\n" +
				"UNICODE_PROPERTY_NOT_ALLOWED_IN_RANGE: [\\p{Uppercase_Letter}-\\p{Lowercase_Letter}];\n" +
				"UNICODE_PROPERTY_NOT_ALLOWED_IN_RANGE_2: [\\p{Letter}-Z];\n" +
				"UNICODE_PROPERTY_NOT_ALLOWED_IN_RANGE_3: [A-\\p{Number}];\n" +
				"INVERTED_UNICODE_PROPERTY_NOT_ALLOWED_IN_RANGE: [\\P{Uppercase_Letter}-\\P{Number}];\n" +
				"EMOJI_MODIFIER: [\\p{Grapheme_Cluster_Break=E_Base}];\n";

		String expected =
				"error(" + ErrorType.INVALID_ESCAPE_SEQUENCE + "): Test.g4:2:32: invalid escape sequence \\u{}\n" +
				"error(" + ErrorType.INVALID_ESCAPE_SEQUENCE + "): Test.g4:3:41: invalid escape sequence \\u{\n" +
				"error(" + ErrorType.INVALID_ESCAPE_SEQUENCE + "): Test.g4:4:35: invalid escape sequence \\u{110\n" +
				"error(" + ErrorType.INVALID_ESCAPE_SEQUENCE + "): Test.g4:5:32: invalid escape sequence \\p{}\n" +
				"error(" + ErrorType.INVALID_ESCAPE_SEQUENCE + "): Test.g4:6:41: invalid escape sequence \\p{\n" +
				"error(" + ErrorType.INVALID_ESCAPE_SEQUENCE + "): Test.g4:7:41: invalid escape sequence \\P{}\n" +
				"error(" + ErrorType.INVALID_ESCAPE_SEQUENCE + "): Test.g4:8:34: invalid escape sequence \\p{NotAProperty}\n" +
				"error(" + ErrorType.INVALID_ESCAPE_SEQUENCE + "): Test.g4:9:43: invalid escape sequence \\P{NotAProperty}\n" +
				"error(" + ErrorType.UNICODE_PROPERTY_NOT_ALLOWED_IN_RANGE + "): Test.g4:10:39: unicode property escapes not allowed in lexer charset range: [\\p{Uppercase_Letter}-\\p{Lowercase_Letter}]\n" +
				"error(" + ErrorType.UNICODE_PROPERTY_NOT_ALLOWED_IN_RANGE + "): Test.g4:11:41: unicode property escapes not allowed in lexer charset range: [\\p{Letter}-Z]\n" +
				"error(" + ErrorType.UNICODE_PROPERTY_NOT_ALLOWED_IN_RANGE + "): Test.g4:12:41: unicode property escapes not allowed in lexer charset range: [A-\\p{Number}]\n" +
				"error(" + ErrorType.UNICODE_PROPERTY_NOT_ALLOWED_IN_RANGE + "): Test.g4:13:48: unicode property escapes not allowed in lexer charset range: [\\P{Uppercase_Letter}-\\P{Number}]\n" +
				"error(" + ErrorType.INVALID_ESCAPE_SEQUENCE + "): Test.g4:14:16: invalid escape sequence \\p{Grapheme_Cluster_Break=E_Base}\n";

		String[] pair = new String[] {
				grammar,
				expected
		};

		ToolTestUtils.testErrors(pair, true);
	}

	/**
	 * This test ensures the {@link ErrorType#UNRECOGNIZED_ASSOC_OPTION} warning
	 * is produced as described in the documentation.
	 */
	[TestMethod] public void testUnrecognizedAssocOption() {
		String grammar =
			"grammar A;\n" +
			"x : 'x'\n" +
			"  | x '+'<assoc=right> x   // warning 157\n" +
			"  |<assoc=right> x '*' x   // ok\n" +
			"  ;\n";
		String expected =
			"warning(" + ErrorType.UNRECOGNIZED_ASSOC_OPTION + "): A.g4:3:10: rule x contains an assoc terminal option in an unrecognized location\n";

		String[] pair = new String[] {
			grammar,
			expected
		};

		ToolTestUtils.testErrors(pair, true);
	}

	/**
	 * This test ensures the {@link ErrorType#FRAGMENT_ACTION_IGNORED} warning
	 * is produced as described in the documentation.
	 */
	[TestMethod] public void testFragmentActionIgnored() {
		String grammar =
			"lexer grammar A;\n" +
			"X1 : 'x' -> more    // ok\n" +
			"   ;\n" +
			"Y1 : 'x' {more();}  // ok\n" +
			"   ;\n" +
			"fragment\n" +
			"X2 : 'x' -> more    // warning 158\n" +
			"   ;\n" +
			"fragment\n" +
			"Y2 : 'x' {more();}  // warning 158\n" +
			"   ;\n";
		String expected =
			"warning(" + ErrorType.FRAGMENT_ACTION_IGNORED + "): A.g4:7:12: fragment rule X2 contains an action or command which can never be executed\n" +
			"warning(" + ErrorType.FRAGMENT_ACTION_IGNORED + "): A.g4:10:9: fragment rule Y2 contains an action or command which can never be executed\n";

		String[] pair = new String[] {
			grammar,
			expected
		};

		ToolTestUtils.testErrors(pair, true);
	}

	/**
	 * This is a regression test for antlr/antlr4#500 "Array Index Out Of
	 * Bounds".
	 * https://github.com/antlr/antlr4/issues/500
	 */
	[TestMethod] public void testTokenNamedEOF() {
		String grammar =
			"lexer grammar A;\n" +
			"WS : ' ';\n" +
			" EOF : 'a';\n";
		String expected =
			"error(" + ErrorType.RESERVED_RULE_NAME + "): A.g4:3:1: cannot declare a rule with reserved name EOF\n";

		String[] pair = new String[] {
			grammar,
			expected
		};

		ToolTestUtils.testErrors(pair, true);
	}

	/**
	 * This is a regression test for antlr/antlr4#649 "unknown target causes
	 * null ptr exception.".
	 * https://github.com/antlr/antlr4/issues/649
	 * Stops before processing the lexer
	 */
	[TestMethod] public void testInvalidLanguageInGrammarWithLexerCommand() {
		String grammar =
			"grammar T;\n" +
			"options { language=Foo; }\n" +
			"start : 'T' EOF;\n" +
			"Something : 'something' -> channel(CUSTOM);";
		String expected =
			"error(" + ErrorType.CANNOT_CREATE_TARGET_GENERATOR + "):  ANTLR cannot generate Foo code as of version " + Tool.VERSION + "\n";
		String[] pair = new String[] {
			grammar,
			expected
		};

		ToolTestUtils.testErrors(pair, true);
	}

	/**
	 * This is a regression test for antlr/antlr4#649 "unknown target causes
	 * null ptr exception.".
	 * https://github.com/antlr/antlr4/issues/649
	 */
	[TestMethod] public void testInvalidLanguageInGrammar() {
		String grammar =
			"grammar T;\n" +
			"options { language=Foo; }\n" +
			"start : 'T' EOF;\n";
		String expected =
			"error(" + ErrorType.CANNOT_CREATE_TARGET_GENERATOR + "):  ANTLR cannot generate Foo code as of version " + Tool.VERSION + "\n";

		String[] pair = new String[] {
			grammar,
			expected
		};

		ToolTestUtils.testErrors(pair, true);
	}

	[TestMethod] public void testChannelDefinitionInLexer(){
		String grammar =
			"lexer grammar T;\n" +
			"\n" +
			"channels {\n" +
			"	WHITESPACE_CHANNEL,\n" +
			"	COMMENT_CHANNEL\n" +
			"}\n" +
			"\n" +
			"COMMENT:    '//' ~[\\n]+ -> channel(COMMENT_CHANNEL);\n" +
			"WHITESPACE: [ \\t]+      -> channel(WHITESPACE_CHANNEL);\n";

		String expected = "";

		String[] pair = { grammar, expected };
		ToolTestUtils.testErrors(pair, true);
	}

	[TestMethod] public void testChannelDefinitionInParser(){
		String grammar =
			"parser grammar T;\n" +
			"\n" +
			"channels {\n" +
			"	WHITESPACE_CHANNEL,\n" +
			"	COMMENT_CHANNEL\n" +
			"}\n" +
			"\n" +
			"start : EOF;\n";

		String expected =
			"error(" + ErrorType.CHANNELS_BLOCK_IN_PARSER_GRAMMAR + "): T.g4:3:0: custom channels are not supported in parser grammars\n";

		String[] pair = { grammar, expected };
		ToolTestUtils.testErrors(pair, true);
	}

	[TestMethod] public void testChannelDefinitionInCombined(){
		String grammar =
			"grammar T;\n" +
			"\n" +
			"channels {\n" +
			"	WHITESPACE_CHANNEL,\n" +
			"	COMMENT_CHANNEL\n" +
			"}\n" +
			"\n" +
			"start : EOF;\n" +
			"\n" +
			"COMMENT:    '//' ~[\\n]+ -> channel(COMMENT_CHANNEL);\n" +
			"WHITESPACE: [ \\t]+      -> channel(WHITESPACE_CHANNEL);\n";

		String expected =
			"error(" + ErrorType.CONSTANT_VALUE_IS_NOT_A_RECOGNIZED_CHANNEL_NAME + "): T.g4:10:35: COMMENT_CHANNEL is not a recognized channel name\n" +
			"error(" + ErrorType.CONSTANT_VALUE_IS_NOT_A_RECOGNIZED_CHANNEL_NAME + "): T.g4:11:35: WHITESPACE_CHANNEL is not a recognized channel name\n" +
			"error(" + ErrorType.CHANNELS_BLOCK_IN_COMBINED_GRAMMAR + "): T.g4:3:0: custom channels are not supported in combined grammars\n";

		String[] pair = { grammar, expected };
		ToolTestUtils.testErrors(pair, true);
	}

	/**
	 * This is a regression test for antlr/antlr4#497 now that antlr/antlr4#309
	 * is resolved.
	 * https://github.com/antlr/antlr4/issues/497
	 * https://github.com/antlr/antlr4/issues/309
	 */
	[TestMethod] public void testChannelDefinitions(){
		String grammar =
			"lexer grammar T;\n" +
			"\n" +
			"channels {\n" +
			"	WHITESPACE_CHANNEL,\n" +
			"	COMMENT_CHANNEL\n" +
			"}\n" +
			"\n" +
			"COMMENT:    '//' ~[\\n]+ -> channel(COMMENT_CHANNEL);\n" +
			"WHITESPACE: [ \\t]+      -> channel(WHITESPACE_CHANNEL);\n" +
			"NEWLINE:    '\\r'? '\\n' -> channel(NEWLINE_CHANNEL);";

		// WHITESPACE_CHANNEL and COMMENT_CHANNEL are defined, but NEWLINE_CHANNEL is not
		String expected =
			"error(" + ErrorType.CONSTANT_VALUE_IS_NOT_A_RECOGNIZED_CHANNEL_NAME + "): T.g4:10:34: NEWLINE_CHANNEL is not a recognized channel name\n";

		String[] pair = { grammar, expected };
		ToolTestUtils.testErrors(pair, true);
	}

	// Test for https://github.com/antlr/antlr4/issues/1556
	[TestMethod] public void testRangeInParserGrammar() {
		String grammar =
			"grammar T;\n"+
			"a:  'A'..'Z' ;\n";
		String expected =
			"error(" + ErrorType.TOKEN_RANGE_IN_PARSER + "): T.g4:2:4: token ranges not allowed in parser: 'A'..'Z'\n";

		String[] pair = new String[] {
			grammar,
			expected
		};

		ToolTestUtils.testErrors(pair, true);
	}

	[TestMethod] public void testRuleNamesAsTree() {
		String grammar =
				"grammar T;\n" +
				"tree : 'X';";
		ToolTestUtils.testErrors(new String[] { grammar, "" }, true);
	}

	[TestMethod] public void testLexerRuleLabel() {
		String grammar =
				"grammar T;\n" +
				"a : A;\n" +
				"A : h=~('b'|'c') ;";
			ToolTestUtils.testErrors(new String[] {
				grammar,
				"error(" + ErrorType.SYNTAX_ERROR + "): T.g4:3:5: syntax error: '=' came as a complete surprise to me while looking for lexer rule element\n" }, false);
	}
}
