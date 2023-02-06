/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */
using Microsoft.VisualBasic;
using org.antlr.v4.tool;

namespace org.antlr.v4.test.tool;


/** Test errors with the set stuff in lexer and parser */
[TestClass]
public class TestErrorSets
{
    protected bool debug = false;

    [TestMethod]
    public void TestNotCharSetWithRuleRef()
    {
        // might be a useful feature to add someday
        var pair = new String[] {
            "grammar T;\n" +
            "a : A {Console.Out.WriteLine($A.text);} ;\n" +
            "A : ~('a'|B) ;\n" +
            "B : 'b' ;\n",
            "error(" + ErrorType.UNSUPPORTED_REFERENCE_IN_LEXER_SET + "): T.g4:3:10: rule reference B is not currently supported in a set\n"
        };
        TestErrors(pair, true);
    }

    private void TestErrors(string[] pair, bool v)
    {
        ToolTestUtils.TestErrors(strings, v);
    }

    [TestMethod]
    public void TestNotCharSetWithString()
    {
        // might be a useful feature to add someday
        var pair = new String[] {
            "grammar T;\n" +
            "a : A {Console.Out.WriteLine($A.text);} ;\n" +
            "A : ~('a'|'aa') ;\n" +
            "B : 'b' ;\n",
            "error(" + ErrorType.INVALID_LITERAL_IN_LEXER_SET + "): T.g4:3:10: multi-character literals are not allowed in lexer sets: 'aa'\n"
        };
        TestErrors(pair, true);
    }
}
