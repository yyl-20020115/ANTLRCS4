/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime;
using org.antlr.v4.runtime.misc;
using org.antlr.v4.runtime.tree.pattern;
using org.antlr.v4.test.runtime;
using org.antlr.v4.test.runtime.java;
using org.antlr.v4.test.runtime.states;

namespace org.antlr.v4.test.tool;

[TestClass]
public class TestParseTreeMatcher
{
    [TestMethod]
    public void TestChunking()
    {
        var m = new ParseTreePatternMatcher(null, null);
        Assert.AreEqual("[ID, ' = ', expr, ' ;']", m.split("<ID> = <expr> ;").ToString());
        Assert.AreEqual("[' ', ID, ' = ', expr]", m.split(" <ID> = <expr>").ToString());
        Assert.AreEqual("[ID, ' = ', expr]", m.split("<ID> = <expr>").ToString());
        Assert.AreEqual("[expr]", m.split("<expr>").ToString());
        Assert.AreEqual("['<x> foo']", m.split("\\<x\\> foo").ToString());
        Assert.AreEqual("['foo <x> bar ', tag]", m.split("foo \\<x\\> bar <tag>").ToString());
    }

    [TestMethod]
    public void TestDelimiters()
    {
        var m = new ParseTreePatternMatcher(null, null);
        m.setDelimiters("<<", ">>", "$");
        String result = m.split("<<ID>> = <<expr>> ;$<< ick $>>").ToString();
        Assert.AreEqual("[ID, ' = ', expr, ' ;<< ick >>']", result);
    }

    [TestMethod]
    public void TestInvertedTags()
    {
        var m = new ParseTreePatternMatcher(null, null);
        string result = null;
        try
        {
            m.split(">expr<");
        }
        catch (ArgumentException iae)
        {
            result = iae.Message;
        }
        var expected = "tag delimiters out of order in pattern: >expr<";
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public void TestUnclosedTag()
    {
        var m = new ParseTreePatternMatcher(null, null);
        String result = null;
        try
        {
            m.split("<expr hi mom");
        }
        catch (ArgumentException iae)
        {
            result = iae.Message;
        }
        var expected = "unterminated tag in pattern: <expr hi mom";
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public void TestExtraClose()
    {
        var m = new ParseTreePatternMatcher(null, null);
        string result = null;
        try
        {
            m.split("<expr> >");
        }
        catch (ArgumentException iae)
        {
            result = iae.Message;
        }
        var expected = "missing start tag in pattern: <expr> >";
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public void TestTokenizingPattern()
    {
        var grammar =
            "grammar X1;\n" +
            "s : ID '=' expr ';' ;\n" +
            "expr : ID | INT ;\n" +
            "ID : [a-z]+ ;\n" +
            "INT : [0-9]+ ;\n" +
            "WS : [ \\r\\n\\t]+ -> skip ;\n";
        var m = GetPatternMatcher("X1.g4", grammar, "X1Parser", "X1Lexer", "s");

        var tokens = m.tokenize("<ID> = <expr> ;");
        Assert.AreEqual("[ID:3, [@-1,1:1='=',<1>,1:1], expr:7, [@-1,1:1=';',<2>,1:1]]", tokens.ToString());
    }

    [TestMethod]
    public void TestCompilingPattern()
    {
        var grammar =
            "grammar X2;\n" +
            "s : ID '=' expr ';' ;\n" +
            "expr : ID | INT ;\n" +
            "ID : [a-z]+ ;\n" +
            "INT : [0-9]+ ;\n" +
            "WS : [ \\r\\n\\t]+ -> skip ;\n";
        var m = GetPatternMatcher("X2.g4", grammar, "X2Parser", "X2Lexer", "s");

        var t = m.compile("<ID> = <expr> ;", m.getParser().getRuleIndex("s"));
        Assert.AreEqual("(s <ID> = (expr <expr>) ;)", t.getPatternTree().toStringTree(m.getParser()));
    }

    [TestMethod]
    public void TestCompilingPatternConsumesAllTokens()
    {
        var grammar =
            "grammar X2;\n" +
            "s : ID '=' expr ';' ;\n" +
            "expr : ID | INT ;\n" +
            "ID : [a-z]+ ;\n" +
            "INT : [0-9]+ ;\n" +
            "WS : [ \\r\\n\\t]+ -> skip ;\n";
        var m = GetPatternMatcher("X2.g4", grammar, "X2Parser", "X2Lexer", "s");

        bool failed = false;
        try
        {
            m.compile("<ID> = <expr> ; extra", m.getParser().getRuleIndex("s"));
        }
        catch (ParseTreePatternMatcher.StartRuleDoesNotConsumeFullPattern e)
        {
            failed = true;
        }
        Assert.IsTrue(failed);
    }

    [TestMethod]
    public void TestPatternMatchesStartRule()
    {
        var grammar =
            "grammar X2;\n" +
            "s : ID '=' expr ';' ;\n" +
            "expr : ID | INT ;\n" +
            "ID : [a-z]+ ;\n" +
            "INT : [0-9]+ ;\n" +
            "WS : [ \\r\\n\\t]+ -> skip ;\n";
        var m = GetPatternMatcher("X2.g4", grammar, "X2Parser", "X2Lexer", "s");

        bool failed = false;
        try
        {
            m.compile("<ID> ;", m.getParser().getRuleIndex("s"));
        }
        catch (InputMismatchException e)
        {
            failed = true;
        }
        Assert.IsTrue(failed);
    }

    [TestMethod]
    public void TestPatternMatchesStartRule2()
    {
        var grammar =
            "grammar X2;\n" +
            "s : ID '=' expr ';' | expr ';' ;\n" +
            "expr : ID | INT ;\n" +
            "ID : [a-z]+ ;\n" +
            "INT : [0-9]+ ;\n" +
            "WS : [ \\r\\n\\t]+ -> skip ;\n";
        var m = GetPatternMatcher("X2.g4", grammar, "X2Parser", "X2Lexer", "s");

        bool failed = false;
        try
        {
            m.compile("<ID> <ID> ;", m.getParser().getRuleIndex("s"));
        }
        catch (NoViableAltException e)
        {
            failed = true;
        }
        Assert.IsTrue(failed);
    }

    [TestMethod]
    public void TestHiddenTokensNotSeenByTreePatternParser()
    {
        vars grammar =
            "grammar X2;\n" +
            "s : ID '=' expr ';' ;\n" +
            "expr : ID | INT ;\n" +
            "ID : [a-z]+ ;\n" +
            "INT : [0-9]+ ;\n" +
            "WS : [ \\r\\n\\t]+ -> channel(HIDDEN) ;\n";
        var m = getPatternMatcher("X2.g4", grammar, "X2Parser", "X2Lexer", "s");

        var t = m.compile("<ID> = <expr> ;", m.getParser().getRuleIndex("s"));
        Assert.AreEqual("(s <ID> = (expr <expr>) ;)", t.getPatternTree().toStringTree(m.getParser()));
    }

    [TestMethod]
    public void TestCompilingMultipleTokens()
    {
        var grammar =
            "grammar X2;\n" +
            "s : ID '=' ID ';' ;\n" +
            "ID : [a-z]+ ;\n" +
            "WS : [ \\r\\n\\t]+ -> skip ;\n";
        var m = GetPatternMatcher("X2.g4", grammar, "X2Parser", "X2Lexer", "s");

        var t = m.compile("<ID> = <ID> ;", m.getParser().getRuleIndex("s"));
        var results = t.getPatternTree().toStringTree(m.getParser());
        var expected = "(s <ID> = <ID> ;)";
        Assert.AreEqual(expected, results);
    }

    [TestMethod]
    public void TestIDNodeMatches()
    {
        var grammar =
            "grammar X3;\n" +
            "s : ID ';' ;\n" +
            "ID : [a-z]+ ;\n" +
            "WS : [ \\r\\n\\t]+ -> skip ;\n";

        var input = "x ;";
        var pattern = "<ID>;";
        CheckPatternMatch(grammar, "s", input, pattern, "X3");
    }

    [TestMethod]
    public void TestIDNodeWithLabelMatches()
    {
        var grammar =
            "grammar X8;\n" +
            "s : ID ';' ;\n" +
            "ID : [a-z]+ ;\n" +
            "WS : [ \\r\\n\\t]+ -> skip ;\n";

        var input = "x ;";
        var pattern = "<id:ID>;";
        var m = CheckPatternMatch(grammar, "s", input, pattern, "X8");
        Assert.AreEqual("{ID=[x], id=[x]}", m.getLabels().ToString());
        Assert.IsNotNull(m.get("id"));
        Assert.IsNotNull(m.get("ID"));
        Assert.AreEqual("x", m.get("id").getText());
        Assert.AreEqual("x", m.get("ID").getText());
        Assert.AreEqual("[x]", m.getAll("id").ToString());
        Assert.AreEqual("[x]", m.getAll("ID").ToString());

        Assert.IsNull(m.get("undefined"));
        Assert.AreEqual("[]", m.getAll("undefined").ToString());
    }

    [TestMethod]
    public void TestLabelGetsLastIDNode()
    {
        var grammar =
            "grammar X9;\n" +
            "s : ID ID ';' ;\n" +
            "ID : [a-z]+ ;\n" +
            "WS : [ \\r\\n\\t]+ -> skip ;\n";

        var input = "x y;";
        var pattern = "<id:ID> <id:ID>;";
        var m = CheckPatternMatch(grammar, "s", input, pattern, "X9");
        Assert.AreEqual("{ID=[x, y], id=[x, y]}", m.getLabels().ToString());
        Assert.IsNotNull(m.get("id"));
        Assert.IsNotNull(m.get("ID"));
        Assert.AreEqual("y", m.get("id").getText());
        Assert.AreEqual("y", m.get("ID").getText());
        Assert.AreEqual("[x, y]", m.getAll("id").ToString());
        Assert.AreEqual("[x, y]", m.getAll("ID").ToString());

        Assert.IsNull(m.get("undefined"));
        Assert.AreEqual("[]", m.getAll("undefined").ToString());
    }

    [TestMethod]
    public void TestIDNodeWithMultipleLabelMatches()
    {
        var grammar =
            "grammar X7;\n" +
            "s : ID ID ID ';' ;\n" +
            "ID : [a-z]+ ;\n" +
            "WS : [ \\r\\n\\t]+ -> skip ;\n";

        var input = "x y z;";
        var pattern = "<a:ID> <b:ID> <a:ID>;";
        ParseTreeMatch m = CheckPatternMatch(grammar, "s", input, pattern, "X7");
        Assert.AreEqual("{ID=[x, y, z], a=[x, z], b=[y]}", m.getLabels().ToString());
        Assert.IsNotNull(m.get("a")); // get first
        Assert.IsNotNull(m.get("b"));
        Assert.IsNotNull(m.get("ID"));
        Assert.AreEqual("z", m.get("a").getText());
        Assert.AreEqual("y", m.get("b").getText());
        Assert.AreEqual("z", m.get("ID").getText()); // get last
        Assert.AreEqual("[x, z]", m.getAll("a").ToString());
        Assert.AreEqual("[y]", m.getAll("b").ToString());
        Assert.AreEqual("[x, y, z]", m.getAll("ID").ToString()); // ordered

        Assert.AreEqual("xyz;", m.getTree().getText()); // whitespace stripped by lexer

        Assert.IsNull(m.get("undefined"));
        Assert.AreEqual("[]", m.getAll("undefined").ToString());
    }

    [TestMethod]
    public void TestTokenAndRuleMatch()
    {
        var grammar =
            "grammar X4;\n" +
            "s : ID '=' expr ';' ;\n" +
            "expr : ID | INT ;\n" +
            "ID : [a-z]+ ;\n" +
            "INT : [0-9]+ ;\n" +
            "WS : [ \\r\\n\\t]+ -> skip ;\n";

        var input = "x = 99;";
        var pattern = "<ID> = <expr> ;";
        CheckPatternMatch(grammar, "s", input, pattern, "X4");
    }

    [TestMethod]
    public void TestTokenTextMatch()
    {
        var grammar =
            "grammar X4;\n" +
            "s : ID '=' expr ';' ;\n" +
            "expr : ID | INT ;\n" +
            "ID : [a-z]+ ;\n" +
            "INT : [0-9]+ ;\n" +
            "WS : [ \\r\\n\\t]+ -> skip ;\n";

        var input = "x = 0;";
        var pattern = "<ID> = 1;";
        bool invertMatch = true; // 0!=1
        CheckPatternMatch(grammar, "s", input, pattern, "X4", invertMatch);

        input = "x = 0;";
        pattern = "<ID> = 0;";
        invertMatch = false;
        CheckPatternMatch(grammar, "s", input, pattern, "X4", invertMatch);

        input = "x = 0;";
        pattern = "x = 0;";
        invertMatch = false;
        CheckPatternMatch(grammar, "s", input, pattern, "X4", invertMatch);

        input = "x = 0;";
        pattern = "y = 0;";
        invertMatch = true;
        CheckPatternMatch(grammar, "s", input, pattern, "X4", invertMatch);
    }

    [TestMethod]
    public void TestAssign()
    {
        var grammar =
            "grammar X5;\n" +
            "s   : expr ';'\n" +
            //"    | 'return' expr ';'\n" +
            "    ;\n" +
            "expr: expr '.' ID\n" +
            "    | expr '*' expr\n" +
            "    | expr '=' expr\n" +
            "    | ID\n" +
            "    | INT\n" +
            "    ;\n" +
            "ID : [a-z]+ ;\n" +
            "INT : [0-9]+ ;\n" +
            "WS : [ \\r\\n\\t]+ -> skip ;\n";

        var input = "x = 99;";
        var pattern = "<ID> = <expr>;";
        CheckPatternMatch(grammar, "s", input, pattern, "X5");
    }

    [TestMethod]
    public void TestLRecursiveExpr()
    {
        var grammar =
            "grammar X6;\n" +
            "s   : expr ';'\n" +
            "    ;\n" +
            "expr: expr '.' ID\n" +
            "    | expr '*' expr\n" +
            "    | expr '=' expr\n" +
            "    | ID\n" +
            "    | INT\n" +
            "    ;\n" +
            "ID : [a-z]+ ;\n" +
            "INT : [0-9]+ ;\n" +
            "WS : [ \\r\\n\\t]+ -> skip ;\n";

        var input = "3*4*5";
        var pattern = "<expr> * <expr> * <expr>";
        CheckPatternMatch(grammar, "expr", input, pattern, "X6");
    }

    private static ParseTreeMatch CheckPatternMatch(string grammar, string startRule,
                                            string input, string pattern,
                                            string grammarName)

    {
        return CheckPatternMatch(grammar, startRule, input, pattern, grammarName, false);
    }

    private static ParseTreeMatch CheckPatternMatch(string grammar, string startRule,
                                            string input, string pattern,
                                            string grammarName, bool invertMatch)

    {
        string grammarFileName = grammarName + ".g4";
        string parserName = grammarName + "Parser";
        string lexerName = grammarName + "Lexer";
        var runOptions = ToolTestUtils.CreateOptionsForJavaToolTests(grammarFileName, grammar, parserName, lexerName,
                false, false, startRule, input,
                false, false, Stage.Execute, true);
        var runner = new JavaRunner();
        {
            var executedState = (JavaExecutedState)runner.Run(runOptions);
            var compiledState = (JavaCompiledState)executedState.previousState;
            var parser = compiledState.InitializeLexerAndParser("").b;

            var p = parser.compileParseTreePattern(pattern, parser.getRuleIndex(startRule));

            var match = p.match(executedState.parseTree);
            bool matched = match.succeeded();
            if (invertMatch) Assert.IsFalse(matched);
            else Assert.IsTrue(matched);
            return match;
        }
    }

    private static ParseTreePatternMatcher GetPatternMatcher(
            string grammarFileName, string grammar, string parserName, string lexerName, string startRule
    )
    {
        var runOptions = ToolTestUtils.CreateOptionsForJavaToolTests(grammarFileName, grammar, parserName, lexerName,
                false, false, startRule, null,
                false, false, Stage.Compile, false);
        var runner = new JavaRunner();
        {
            var compiledState = (JavaCompiledState)runner.Run(runOptions);

            var lexerParserPair = compiledState.InitializeLexerAndParser("");

            return new ParseTreePatternMatcher(lexerParserPair.a, lexerParserPair.b);
        }
    }
}
