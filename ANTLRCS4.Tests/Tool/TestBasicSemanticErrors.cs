/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using Antlr4.StringTemplate;
using org.antlr.v4.tool;

namespace org.antlr.v4.test.tool;

[TestClass]
public class TestBasicSemanticErrors
{
    readonly static string[] U = {
        // INPUT
        "parser grammar U;\n" +
        "options { foo=bar; k=3;}\n" +
        "tokens {\n" +
        "        ID,\n" +
        "        f,\n" +
        "        S\n" +
        "}\n" +
        "tokens { A }\n" +
        "options { x=y; }\n" +
        "\n" +
        "a\n" +
        "options { blech=bar; greedy=true; }\n" +
        "        :       ID\n" +
        "        ;\n" +
        "b : ( options { ick=bar; greedy=true; } : ID )+ ;\n" +
        "c : ID<blue> ID<x=y> ;",
        // YIELDS
		"warning(" + ErrorType.ILLEGAL_OPTION + "): U.g4:2:10: unsupported option foo\n" +
        "warning(" + ErrorType.ILLEGAL_OPTION + "): U.g4:2:19: unsupported option k\n" +
        "error(" + ErrorType.TOKEN_NAMES_MUST_START_UPPER + "): U.g4:5:8: token names must start with an uppercase letter: f\n" +
        "warning(" + ErrorType.ILLEGAL_OPTION + "): U.g4:9:10: unsupported option x\n" +
        "error(" + ErrorType.REPEATED_PREQUEL + "): U.g4:9:0: repeated grammar prequel spec (options, tokens, or import); please merge\n" +
        "error(" + ErrorType.REPEATED_PREQUEL + "): U.g4:8:0: repeated grammar prequel spec (options, tokens, or import); please merge\n" +
        "warning(" + ErrorType.ILLEGAL_OPTION + "): U.g4:12:10: unsupported option blech\n" +
        "warning(" + ErrorType.ILLEGAL_OPTION + "): U.g4:12:21: unsupported option greedy\n" +
        "warning(" + ErrorType.ILLEGAL_OPTION + "): U.g4:15:16: unsupported option ick\n" +
        "warning(" + ErrorType.ILLEGAL_OPTION + "): U.g4:15:25: unsupported option greedy\n" +
        "warning(" + ErrorType.ILLEGAL_OPTION + "): U.g4:16:16: unsupported option x\n",
    };

    [TestMethod]
    public void TestU()
    {
        TestErrors(U, false);
    }

    /**
	 * Regression test for #25 "Don't allow labels on not token set subrules".
	 * https://github.com/antlr/antlr4/issues/25
	 */
    [TestMethod]
    public void TestIllegalNonSetLabel()
    {
        var grammar =
            "grammar T;\n" +
            "ss : op=('=' | '+=' | expr) EOF;\n" +
            "expr : '=' '=';\n" +
            "";

        var expected =
            "error(" + ErrorType.LABEL_BLOCK_NOT_A_SET + "): T.g4:2:5: label op assigned to a block which is not a set\n";

        TestErrors(new string[] { grammar, expected }, false);
    }

    [TestMethod]
    public void TestArgumentRetvalLocalConflicts()
    {
        var grammarTemplate =
            "grammar T;\n" +
            "ss<if(args)>[<args>]<endif> <if(retvals)>returns [<retvals>]<endif>\n" +
            "<if(locals)>locals [<locals>]<endif>\n" +
            "  : <body> EOF;\n" +
            "expr : '=';\n";

        var expected =
            "error(" + ErrorType.ARG_CONFLICTS_WITH_RULE + "): T.g4:2:7: parameter expr conflicts with rule with same name\n" +
            "error(" + ErrorType.RETVAL_CONFLICTS_WITH_RULE + "): T.g4:2:26: return value expr conflicts with rule with same name\n" +
            "error(" + ErrorType.LOCAL_CONFLICTS_WITH_RULE + "): T.g4:3:12: local expr conflicts with rule with same name\n" +
            "error(" + ErrorType.RETVAL_CONFLICTS_WITH_ARG + "): T.g4:2:26: return value expr conflicts with parameter with same name\n" +
            "error(" + ErrorType.LOCAL_CONFLICTS_WITH_ARG + "): T.g4:3:12: local expr conflicts with parameter with same name\n" +
            "error(" + ErrorType.LOCAL_CONFLICTS_WITH_RETVAL + "): T.g4:3:12: local expr conflicts with return value with same name\n" +
            "error(" + ErrorType.LABEL_CONFLICTS_WITH_RULE + "): T.g4:4:4: label expr conflicts with rule with same name\n" +
            "error(" + ErrorType.LABEL_CONFLICTS_WITH_ARG + "): T.g4:4:4: label expr conflicts with parameter with same name\n" +
            "error(" + ErrorType.LABEL_CONFLICTS_WITH_RETVAL + "): T.g4:4:4: label expr conflicts with return value with same name\n" +
            "error(" + ErrorType.LABEL_CONFLICTS_WITH_LOCAL + "): T.g4:4:4: label expr conflicts with local with same name\n";
        var grammarST = new Template(grammarTemplate);
        grammarST.Add("args", "int expr");
        grammarST.Add("retvals", "int expr");
        grammarST.Add("locals", "int expr");
        grammarST.Add("body", "expr=expr");
        TestErrors(new string[] { grammarST.Render(), expected }, false);
    }

    private static void TestErrors(string[] strings, bool v)
    {
        ToolTestUtils.TestErrors(strings, v);
    }
}
