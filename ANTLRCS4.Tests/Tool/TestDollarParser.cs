/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

namespace org.antlr.v4.test.tool;

[TestClass]
public class TestDollarParser
{
    [TestMethod]
    public void TestSimpleCall()
    {
        var grammar = "grammar T;\n" +
                      "a : ID  { outStream.println(new java.io.File($parser.getSourceName()).getAbsolutePath()); }\n" +
                      "  ;\n" +
                      "ID : 'a'..'z'+ ;\n";
        var executedState = ToolTestUtils.ExecParser("T.g4", grammar, "TParser", "TLexer", "a", "x", true);
        Assert.IsTrue(executedState.output.Contains("input"));
        Assert.AreEqual("", executedState.errors);
    }
}
