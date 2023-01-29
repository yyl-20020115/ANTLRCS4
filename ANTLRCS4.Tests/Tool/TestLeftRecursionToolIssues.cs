/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.tool;

namespace org.antlr.v4.test.tool;

/** */
public class TestLeftRecursionToolIssues {
	protected bool debug = false;

	[TestMethod] public void testCheckForNonLeftRecursiveRule(){
		String grammar =
			"grammar T;\n" +
			"s @after {Console.Out.WriteLine($ctx.toStringTree(this));} : a ;\n" +
			"a : a ID\n" +
			"  ;\n" +
			"ID : 'a'..'z'+ ;\n" +
			"WS : (' '|'\\n') -> skip ;\n";
		String expected =
			"error(" + ErrorType.NO_NON_LR_ALTS + "): T.g4:3:0: left recursive rule a must contain an alternative which is not left recursive\n";
		testErrors(new String[] { grammar, expected }, false);
	}

    private void testErrors(string[] strings, bool v)
    {
        throw new NotImplementedException();
    }

    [TestMethod] public void testCheckForLeftRecursiveEmptyFollow(){
		String grammar =
			"grammar T;\n" +
			"s @after {Console.Out.WriteLine($ctx.toStringTree(this));} : a ;\n" +
			"a : a ID?\n" +
			"  | ID\n" +
			"  ;\n" +
			"ID : 'a'..'z'+ ;\n" +
			"WS : (' '|'\\n') -> skip ;\n";
		String expected =
			"error(" + ErrorType.EPSILON_LR_FOLLOW + "): T.g4:3:0: left recursive rule a contains a left recursive alternative which can be followed by the empty string\n";
		testErrors(new String[] { grammar, expected }, false);
	}

	/** Reproduces https://github.com/antlr/antlr4/issues/855 */
	[TestMethod] public void testLeftRecursiveRuleRefWithArg(){
		String grammar =
			"grammar T;\n" +
			"statement\n" +
			"locals[Scope scope]\n" +
			"    : expressionA[$scope] ';'\n" +
			"    ;\n" +
			"expressionA[Scope scope]\n" +
			"    : atom[$scope]\n" +
			"    | expressionA[$scope] '[' expressionA[$scope] ']'\n" +
			"    ;\n" +
			"atom[Scope scope]\n" +
			"    : 'dummy'\n" +
			"    ;\n";
		String expected =
			"error(" + ErrorType.NONCONFORMING_LR_RULE + "): T.g4:6:0: rule expressionA is left recursive but doesn't conform to a pattern ANTLR can handle\n";
		testErrors(new String[]{grammar, expected}, false);
	}

	/** Reproduces https://github.com/antlr/antlr4/issues/855 */
	[TestMethod] public void testLeftRecursiveRuleRefWithArg2(){
		String grammar =
			"grammar T;\n" +
			"a[int i] : 'x'\n" +
			"  | a[3] 'y'\n" +
			"  ;";
		String expected =
			"error(" + ErrorType.NONCONFORMING_LR_RULE + "): T.g4:2:0: rule a is left recursive but doesn't conform to a pattern ANTLR can handle\n";
		testErrors(new String[]{grammar, expected}, false);
	}

	/** Reproduces https://github.com/antlr/antlr4/issues/855 */
	[TestMethod] public void testLeftRecursiveRuleRefWithArg3(){
		String grammar =
			"grammar T;\n" +
			"a : 'x'\n" +
			"  | a[3] 'y'\n" +
			"  ;";
		String expected =
			"error(" + ErrorType.NONCONFORMING_LR_RULE + "): T.g4:2:0: rule a is left recursive but doesn't conform to a pattern ANTLR can handle\n";
		testErrors(new String[]{grammar, expected}, false);
	}

	/** Reproduces https://github.com/antlr/antlr4/issues/822 */
	[TestMethod] public void testIsolatedLeftRecursiveRuleRef(){
		String grammar =
			"grammar T;\n" +
			"a : a | b ;\n" +
			"b : 'B' ;\n";
		String expected =
			"error(" + ErrorType.NONCONFORMING_LR_RULE + "): T.g4:2:0: rule a is left recursive but doesn't conform to a pattern ANTLR can handle\n";
		testErrors(new String[]{grammar, expected}, false);
	}

	/** Reproduces https://github.com/antlr/antlr4/issues/773 */
	[TestMethod] public void testArgOnPrimaryRuleInLeftRecursiveRule(){
		String grammar =
			"grammar T;\n" +
			"val: dval[1]\n" +
			"   | val '*' val\n" +
			"   ;\n" +
			"dval[int  x]: '.';\n";
		String expected = ""; // dval[1] should not be error
		testErrors(new String[]{grammar, expected}, false);
	}
}
