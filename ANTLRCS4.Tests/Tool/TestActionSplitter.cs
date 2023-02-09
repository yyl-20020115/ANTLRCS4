/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.runtime;
using org.antlr.v4.parse;
using org.antlr.v4.runtime;
using org.antlr.v4.semantics;

namespace org.antlr.v4.test.tool;

[TestClass]
public class TestActionSplitter
{
    readonly static string[] exprs = {
        "foo",      "['foo'<" + ActionSplitter.TEXT + ">]",
        "$x",       "['$x'<" + ActionSplitter.ATTR + ">]",
        "\\$x",     "['\\$x'<" + ActionSplitter.TEXT + ">]",
        "$x.y",     "['$x.y'<" + ActionSplitter.QUALIFIED_ATTR + ">]",
        "$ID.text",     "['$ID.text'<" + ActionSplitter.QUALIFIED_ATTR + ">]",
        "$ID",      "['$ID'<" + ActionSplitter.ATTR + ">]",
        "$ID.getText()",        "['$ID'<" + ActionSplitter.ATTR + ">, '.getText()'<" + ActionSplitter.TEXT + ">]",
        "$ID.text = \"test\";",     "['$ID.text'<" + ActionSplitter.QUALIFIED_ATTR + ">, ' = \"test\";'<" + ActionSplitter.TEXT + ">]",
        "$a.line == $b.line",       "['$a.line'<" + ActionSplitter.QUALIFIED_ATTR + ">, ' == '<" + ActionSplitter.TEXT + ">, '$b.line'<" + ActionSplitter.QUALIFIED_ATTR + ">]",
        "$r.tree",      "['$r.tree'<" + ActionSplitter.QUALIFIED_ATTR + ">]",
        "foo $a::n bar",        "['foo '<" + ActionSplitter.TEXT + ">, '$a::n'<" + ActionSplitter.NONLOCAL_ATTR + ">, ' bar'<" + ActionSplitter.TEXT + ">]",
        "$rule::x;",        "['$rule::x'<" + ActionSplitter.NONLOCAL_ATTR + ">, ';'<" + ActionSplitter.TEXT + ">]",
        "$field::x = $field.st;",       "['$field::x = $field.st;'<" + ActionSplitter.SET_NONLOCAL_ATTR + ">]",
        "$foo.get(\"ick\");",       "['$foo'<" + ActionSplitter.ATTR + ">, '.get(\"ick\");'<" + ActionSplitter.TEXT + ">]",
    };

    [TestMethod]
    public void TestExprs()
    {
        for (int i = 0; i < exprs.Length; i += 2)
        {
            var input = exprs[i];
            var expect = exprs[i + 1];
            var chunks = GetActionChunks(input);
            Assert.AreEqual(expect, chunks.ToString(), "input: " + input);
        }
    }

    private static List<string> GetActionChunks(string a)
    {
        List<string> chunks = new();
        var splitter = new ActionSplitter(new ANTLRStringStream(a), new BlankActionSplitterListener());
        var t = splitter.NextToken();
        while (t.Type != Token.EOF)
        {
            chunks.Add("'" + t.Text + "'<" + t.Type + ">");
            t = splitter.NextToken();
        }
        return chunks;
    }
}
