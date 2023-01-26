/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */
namespace org.antlr.v4.test.tool;


/** Test errors with the set stuff in lexer and parser */
public class TestErrorSets {
	protected bool debug = false;

	[TestMethod] public void testNotCharSetWithRuleRef(){
		// might be a useful feature to add someday
		String[] pair = new String[] {
			"grammar T;\n" +
			"a : A {System.out.println($A.text);} ;\n" +
			"A : ~('a'|B) ;\n" +
			"B : 'b' ;\n",
			"error(" + ErrorType.UNSUPPORTED_REFERENCE_IN_LEXER_SET + "): T.g4:3:10: rule reference B is not currently supported in a set\n"
		};
		testErrors(pair, true);
	}

	[TestMethod] public void testNotCharSetWithString(){
		// might be a useful feature to add someday
		String[] pair = new String[] {
			"grammar T;\n" +
			"a : A {System.out.println($A.text);} ;\n" +
			"A : ~('a'|'aa') ;\n" +
			"B : 'b' ;\n",
			"error(" + ErrorType.INVALID_LITERAL_IN_LEXER_SET + "): T.g4:3:10: multi-character literals are not allowed in lexer sets: 'aa'\n"
		};
		testErrors(pair, true);
	}
}
