/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.test.runtime.states;

namespace org.antlr.v4.test.tool;

public class TestDollarParser {
	[TestMethod]
	public void testSimpleCall() {
		String grammar = "grammar T;\n" +
                      "a : ID  { outStream.println(new java.io.File($parser.getSourceName()).getAbsolutePath()); }\n" +
                      "  ;\n" +
                      "ID : 'a'..'z'+ ;\n";
		ExecutedState executedState = ToolTestUtils.execParser("T.g4", grammar, "TParser", "TLexer", "a", "x", true);
		Assert.IsTrue(executedState.output.contains("input"));
		Assert.AreEqual("", executedState.errors);
	}
}
