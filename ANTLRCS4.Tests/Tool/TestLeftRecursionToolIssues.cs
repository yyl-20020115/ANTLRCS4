/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.tool;

namespace org.antlr.v4.test.tool;

[TestClass]
public class TestLeftRecursionToolIssues
{
    protected bool debug = false;

    [TestMethod]
    public void TestCheckForNonLeftRecursiveRule()
    {
        var grammar =
            "grammar T;\n" +
            "s @after {Console.Out.WriteLine($ctx.toStringTree(this));} : a ;\n" +
            "a : a ID\n" +
            "  ;\n" +
            "ID : 'a'..'z'+ ;\n" +
            "WS : (' '|'\\n') -> skip ;\n";
        var expected =
            "error(" + ErrorType.NO_NON_LR_ALTS + "): T.g4:3:0: left recursive rule a must contain an alternative which is not left recursive\n";
        ToolTestUtils.TestErrors(new String[] { grammar, expected }, false);
    }


    [TestMethod]
    public void TestCheckForLeftRecursiveEmptyFollow()
    {
        var grammar =
            "grammar T;\n" +
            "s @after {Console.Out.WriteLine($ctx.toStringTree(this));} : a ;\n" +
            "a : a ID?\n" +
            "  | ID\n" +
            "  ;\n" +
            "ID : 'a'..'z'+ ;\n" +
            "WS : (' '|'\\n') -> skip ;\n";
        var expected =
            "error(" + ErrorType.EPSILON_LR_FOLLOW + "): T.g4:3:0: left recursive rule a contains a left recursive alternative which can be followed by the empty string\n";
        ToolTestUtils.TestErrors(new String[] { grammar, expected }, false);
    }

    /** Reproduces https://github.com/antlr/antlr4/issues/855 */
    [TestMethod]
    public void TestLeftRecursiveRuleRefWithArg()
    {
        var grammar =
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
        var expected =
            "error(" + ErrorType.NONCONFORMING_LR_RULE + "): T.g4:6:0: rule expressionA is left recursive but doesn't conform to a pattern ANTLR can handle\n";
        ToolTestUtils.TestErrors(new string[] { grammar, expected }, false);
    }

    /** Reproduces https://github.com/antlr/antlr4/issues/855 */
    [TestMethod]
    public void TestLeftRecursiveRuleRefWithArg2()
    {
        var grammar =
            "grammar T;\n" +
            "a[int i] : 'x'\n" +
            "  | a[3] 'y'\n" +
            "  ;";
        var expected =
            "error(" + ErrorType.NONCONFORMING_LR_RULE + "): T.g4:2:0: rule a is left recursive but doesn't conform to a pattern ANTLR can handle\n";
        ToolTestUtils.TestErrors(new string[] { grammar, expected }, false);
    }

    /** Reproduces https://github.com/antlr/antlr4/issues/855 */
    [TestMethod]
    public void TestLeftRecursiveRuleRefWithArg3()
    {
        var grammar =
            "grammar T;\n" +
            "a : 'x'\n" +
            "  | a[3] 'y'\n" +
            "  ;";
        var expected =
            "error(" + ErrorType.NONCONFORMING_LR_RULE + "): T.g4:2:0: rule a is left recursive but doesn't conform to a pattern ANTLR can handle\n";
        ToolTestUtils.TestErrors(new String[] { grammar, expected }, false);
    }

    /** Reproduces https://github.com/antlr/antlr4/issues/822 */
    [TestMethod]
    public void TestIsolatedLeftRecursiveRuleRef()
    {
        var grammar =
            "grammar T;\n" +
            "a : a | b ;\n" +
            "b : 'B' ;\n";
        var expected =
            "error(" + ErrorType.NONCONFORMING_LR_RULE + "): T.g4:2:0: rule a is left recursive but doesn't conform to a pattern ANTLR can handle\n";
        ToolTestUtils.TestErrors(new string[] { grammar, expected }, false);
    }

    /** Reproduces https://github.com/antlr/antlr4/issues/773 */
    [TestMethod]
    public void TestArgOnPrimaryRuleInLeftRecursiveRule()
    {
        var grammar =
            "grammar T;\n" +
            "val: dval[1]\n" +
            "   | val '*' val\n" +
            "   ;\n" +
            "dval[int  x]: '.';\n";
        var expected = ""; // dval[1] should not be error
        ToolTestUtils.TestErrors(new string[] { grammar, expected }, false);
    }
}
