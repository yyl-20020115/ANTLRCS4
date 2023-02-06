/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using Antlr4.StringTemplate;
using org.antlr.v4.analysis;
using org.antlr.v4.automata;
using org.antlr.v4.codegen;
using org.antlr.v4.semantics;
using org.antlr.v4.test.runtime;
using org.antlr.v4.tool;

namespace org.antlr.v4.test.tool;

//@SuppressWarnings("unused")
[TestClass]
public class TestActionTranslation
{
    readonly string attributeTemplate =
        "attributeTemplate(members,init,inline,finally,inline2) ::= <<\n" +
        "parser grammar A;\n" +
        "@members {#members#<members>#end-members#}\n" +
        "a[int x, int x1] returns [int y]\n" +
        "@init {#init#<init>#end-init#}\n" +
        "    :   id=ID ids+=ID lab=b[34] c d {\n" +
        "		 #inline#<inline>#end-inline#\n" +
        "		 }\n" +
        "		 c\n" +
        "    ;\n" +
        "    finally {#finally#<finally>#end-finally#}\n" +
        "b[int d] returns [int e]\n" +
        "    :   {#inline2#<inline2>#end-inline2#}\n" +
        "    ;\n" +
        "c returns [int x, int y] : ;\n" +
        "d	 :   ;\n" +
        ">>";

    [TestMethod]
    public void TestEscapedLessThanInAction()
    {
        var action = "i<3; '<xmltag>'";
        var expected = "i<3; '<xmltag>'";
        TestActions(attributeTemplate, "members", action, expected);
        TestActions(attributeTemplate, "init", action, expected);
        TestActions(attributeTemplate, "inline", action, expected);
        TestActions(attributeTemplate, "finally", action, expected);
        TestActions(attributeTemplate, "inline2", action, expected);
    }

    [TestMethod]
    public void TestEscapedDollarInAction()
    {
        var action = "int \\$n; \"\\$in string\\$\"";
        var expected = "int $n; \"$in string$\"";
        TestActions(attributeTemplate, "members", action, expected);
        TestActions(attributeTemplate, "init", action, expected);
        TestActions(attributeTemplate, "inline", action, expected);
        TestActions(attributeTemplate, "finally", action, expected);
        TestActions(attributeTemplate, "inline2", action, expected);
    }

    /**
	 * Regression test for "in antlr v4 lexer, $ translation issue in action".
	 * https://github.com/antlr/antlr4/issues/176
	 */
    [TestMethod]
    public void TestUnescapedDollarInAction()
    {
        var action = "\\$string$";
        var expected = "$string$";
        TestActions(attributeTemplate, "members", action, expected);
        TestActions(attributeTemplate, "init", action, expected);
        TestActions(attributeTemplate, "inline", action, expected);
        TestActions(attributeTemplate, "finally", action, expected);
        TestActions(attributeTemplate, "inline2", action, expected);
    }

    [TestMethod]
    public void TestEscapedSlash()
    {
        var action = "x = '\\n';";  // x = '\n'; -> x = '\n';
        var expected = "x = '\\n';";
        TestActions(attributeTemplate, "members", action, expected);
        TestActions(attributeTemplate, "init", action, expected);
        TestActions(attributeTemplate, "inline", action, expected);
        TestActions(attributeTemplate, "finally", action, expected);
        TestActions(attributeTemplate, "inline2", action, expected);
    }

    [TestMethod]
    public void TestComplicatedArgParsing()
    {
        var action = "x, (*a).foo(21,33), 3.2+1, '\\n', " +
                        "\"a,oo\\nick\", {bl, \"fdkj\"eck}";
        var expected = "x, (*a).foo(21,33), 3.2+1, '\\n', " +
                        "\"a,oo\\nick\", {bl, \"fdkj\"eck}";
        TestActions(attributeTemplate, "members", action, expected);
        TestActions(attributeTemplate, "init", action, expected);
        TestActions(attributeTemplate, "inline", action, expected);
        TestActions(attributeTemplate, "finally", action, expected);
        TestActions(attributeTemplate, "inline2", action, expected);
    }

    [TestMethod]
    public void TestComplicatedArgParsingWithTranslation()
    {
        var action = "x, $ID.text+\"3242\", (*$ID).foo(21,33), 3.2+1, '\\n', " +
                        "\"a,oo\\nick\", {bl, \"fdkj\"eck}";
        var expected =
            "x, (((AContext)_localctx).ID!=null?((AContext)_localctx).ID.getText():null)+\"3242\", " +
            "(*((AContext)_localctx).ID).foo(21,33), 3.2+1, '\\n', \"a,oo\\nick\", {bl, \"fdkj\"eck}";
        TestActions(attributeTemplate, "inline", action, expected);
    }

    [TestMethod]
    public void TestArguments()
    {
        var action = "$x; $ctx.x";
        var expected = "_localctx.x; _localctx.x";
        TestActions(attributeTemplate, "inline", action, expected);
    }

    [TestMethod]
    public void TestReturnValue()
    {
        var action = "$y; $ctx.y";
        var expected = "_localctx.y; _localctx.y";
        TestActions(attributeTemplate, "inline", action, expected);
    }

    [TestMethod]
    public void TestReturnValueWithNumber()
    {
        var action = "$ctx.x1";
        var expected = "_localctx.x1";
        TestActions(attributeTemplate, "inline", action, expected);
    }

    [TestMethod]
    public void TestReturnValuesCurrentRule()
    {
        var action = "$y; $ctx.y;";
        var expected = "_localctx.y; _localctx.y;";
        TestActions(attributeTemplate, "inline", action, expected);
    }

    [TestMethod]
    public void TestReturnValues()
    {
        var action = "$lab.e; $b.e; $y.e = \"\";";
        var expected = "((AContext)_localctx).lab.e; ((AContext)_localctx).b.e; _localctx.y.e = \"\";";
        TestActions(attributeTemplate, "inline", action, expected);
    }

    [TestMethod]
    public void TestReturnWithMultipleRuleRefs()
    {
        var action = "$c.x; $c.y;";
        var expected = "((AContext)_localctx).c.x; ((AContext)_localctx).c.y;";
        TestActions(attributeTemplate, "inline", action, expected);
    }

    [TestMethod]
    public void TestTokenRefs()
    {
        var action = "$id; $ID; $id.text; $id.getText(); $id.line;";
        var expected = "((AContext)_localctx).id; ((AContext)_localctx).ID; (((AContext)_localctx).id!=null?((AContext)_localctx).id.getText():null); ((AContext)_localctx).id.getText(); (((AContext)_localctx).id!=null?((AContext)_localctx).id.getLine():0);";
        TestActions(attributeTemplate, "inline", action, expected);
    }

    [TestMethod]
    public void TestRuleRefs()
    {
        var action = "$lab.start; $c.text;";
        var expected = "(((AContext)_localctx).lab!=null?(((AContext)_localctx).lab.start):null); (((AContext)_localctx).c!=null?_input.getText(((AContext)_localctx).c.start,((AContext)_localctx).c.stop):null);";
        TestActions(attributeTemplate, "inline", action, expected);
    }

    /** Added in response to https://github.com/antlr/antlr4/issues/1211 */
    [TestMethod]
    public void TestUnknownAttr()
    {
        var action = "$qqq.text";
        var expected = ""; // was causing an exception
        TestActions(attributeTemplate, "inline", action, expected);
    }

    /**
	 * Regression test for issue #1295
     * $e.v yields incorrect value 0 in "e returns [int v] : '1' {$v = 1;} | '(' e ')' {$v = $e.v;} ;"
	 * https://github.com/antlr/antlr4/issues/1295
	 */
    [TestMethod]
    public void TestRuleRefsRecursive()
    {
        var recursiveTemplate =
            "recursiveTemplate(inline) ::= <<\n" +
            "parser grammar A;\n" +
            "e returns [int v]\n" +
            "    :   INT {$v = $INT.int;}\n" +
            "    |   '(' e ')' {\n" +
            "		 #inline#<inline>#end-inline#\n" +
            "		 }\n" +
            "    ;\n" +
            ">>";
        var leftRecursiveTemplate =
            "recursiveTemplate(inline) ::= <<\n" +
            "parser grammar A;\n" +
            "e returns [int v]\n" +
            "    :   a=e op=('*'|'/') b=e  {$v = eval($a.v, $op.type, $b.v);}\n" +
            "    |   INT {$v = $INT.int;}\n" +
            "    |   '(' e ')' {\n" +
            "		 #inline#<inline>#end-inline#\n" +
            "		 }\n" +
            "    ;\n" +
            ">>";
        // ref to value returned from recursive call to rule
        var action = "$v = $e.v;";
        var expected = "((EContext)_localctx).v =  ((EContext)_localctx).e.v;";
        TestActions(recursiveTemplate, "inline", action, expected);
        TestActions(leftRecursiveTemplate, "inline", action, expected);
        // ref to predefined attribute obtained from recursive call to rule
        action = "$v = $e.text.Length();";
        expected = "((EContext)_localctx).v =  (((EContext)_localctx).e!=null?_input.getText(((EContext)_localctx).e.start,((EContext)_localctx).e.stop):null).Length();";
        TestActions(recursiveTemplate, "inline", action, expected);
        TestActions(leftRecursiveTemplate, "inline", action, expected);
    }

    [TestMethod]
    public void TestRefToTextAttributeForCurrentRule()
    {
        var action = "$ctx.text; $text";

        // this is the expected translation for all cases
        var expected =
            "_localctx.text; _input.getText(_localctx.start, _input.LT(-1))";

        TestActions(attributeTemplate, "init", action, expected);
        TestActions(attributeTemplate, "inline", action, expected);
        TestActions(attributeTemplate, "finally", action, expected);
    }

    [TestMethod]
    public void TestEmptyActions()
    {
        var gS =
               "grammar A;\n" +
               "a[] : 'a' ;\n" +
               "c : a[] c[] ;\n";
        var g = new Grammar(gS);
    }

    private static void TestActions(string templates, string actionName, string action, string expected)
    {
        int lp = templates.IndexOf('(');
        var name = templates.Substring(0, lp);
        var group = new TemplateGroupString(templates);
        var st = group.GetInstanceOf(name);
        st.Add(actionName, action);
        var grammar = st.Render();
        var equeue = new ErrorQueue();
        var g = new Grammar(grammar, equeue);
        if (g.ast != null && !g.ast.hasErrors)
        {
            var sem = new SemanticPipeline(g);
            sem.process();

            var factory = new ParserATNFactory(g);
            if (g.isLexer()) factory = new LexerATNFactory((LexerGrammar)g);
            g.atn = factory.CreateATN();

            var anal = new AnalysisPipeline(g);
            anal.Process();

            var gen = CodeGenerator.Create(g);
            var outputFileST = gen.GenerateParser(false);
            var output = outputFileST.Render();
            //Console.Out.WriteLine(output);
            var b = "#" + actionName + "#";
            int start = output.IndexOf(b);
            var e = "#end-" + actionName + "#";
            int end = output.IndexOf(e);
            var snippet = output.Substring(start + b.Length, end - (start + b.Length));
            Assert.AreEqual(expected, snippet);
        }
        if (equeue.Count > 0)
        {
            //			Console.Error.WriteLine(equeue.ToString());
        }
    }
}
