/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime;
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
        Assert.AreEqual("[ID, ' = ', expr, ' ;']", m.Split("<ID> = <expr> ;").ToString());
        Assert.AreEqual("[' ', ID, ' = ', expr]", m.Split(" <ID> = <expr>").ToString());
        Assert.AreEqual("[ID, ' = ', expr]", m.Split("<ID> = <expr>").ToString());
        Assert.AreEqual("[expr]", m.Split("<expr>").ToString());
        Assert.AreEqual("['<x> foo']", m.Split("\\<x\\> foo").ToString());
        Assert.AreEqual("['foo <x> bar ', tag]", m.Split("foo \\<x\\> bar <tag>").ToString());
    }

    [TestMethod]
    public void TestDelimiters()
    {
        var m = new ParseTreePatternMatcher(null, null);
        m.SetDelimiters("<<", ">>", "$");
        var result = m.Split("<<ID>> = <<expr>> ;$<< ick $>>").ToString();
        Assert.AreEqual("[ID, ' = ', expr, ' ;<< ick >>']", result);
    }

    [TestMethod]
    public void TestInvertedTags()
    {
        var m = new ParseTreePatternMatcher(null, null);
        string result = null;
        try
        {
            m.Split(">expr<");
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
        string result = null;
        try
        {
            m.Split("<expr hi mom");
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
            m.Split("<expr> >");
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

        var tokens = m.Tokenize("<ID> = <expr> ;");
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

        var t = m.Compile("<ID> = <expr> ;", m.Parser.GetRuleIndex("s"));
        Assert.AreEqual("(s <ID> = (expr <expr>) ;)", t.PatternTree.ToStringTree(m.Parser));
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
            m.Compile("<ID> = <expr> ; extra", m.Parser.GetRuleIndex("s"));
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
            m.Compile("<ID> ;", m.Parser.GetRuleIndex("s"));
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
            m.Compile("<ID> <ID> ;", m.Parser.GetRuleIndex("s"));
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
        var grammar =
            "grammar X2;\n" +
            "s : ID '=' expr ';' ;\n" +
            "expr : ID | INT ;\n" +
            "ID : [a-z]+ ;\n" +
            "INT : [0-9]+ ;\n" +
            "WS : [ \\r\\n\\t]+ -> channel(HIDDEN) ;\n";
        var m = GetPatternMatcher("X2.g4", grammar, "X2Parser", "X2Lexer", "s");

        var t = m.Compile("<ID> = <expr> ;", m.Parser.GetRuleIndex("s"));
        Assert.AreEqual("(s <ID> = (expr <expr>) ;)", t.PatternTree.ToStringTree(m.Parser));
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

        var t = m.Compile("<ID> = <ID> ;", m.Parser.GetRuleIndex("s"));
        var results = t.PatternTree.ToStringTree(m.Parser);
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
        Assert.AreEqual("{ID=[x], id=[x]}", m.GetLabels().ToString());
        Assert.IsNotNull(m.Get("id"));
        Assert.IsNotNull(m.Get("ID"));
        Assert.AreEqual("x", m.Get("id").Text);
        Assert.AreEqual("x", m.Get("ID").Text);
        Assert.AreEqual("[x]", m.GetAll("id").ToString());
        Assert.AreEqual("[x]", m.GetAll("ID").ToString());

        Assert.IsNull(m.Get("undefined"));
        Assert.AreEqual("[]", m.GetAll("undefined").ToString());
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
        Assert.AreEqual("{ID=[x, y], id=[x, y]}", m.GetLabels().ToString());
        Assert.IsNotNull(m.Get("id"));
        Assert.IsNotNull(m.Get("ID"));
        Assert.AreEqual("y", m.Get("id").Text);
        Assert.AreEqual("y", m.Get("ID").Text);
        Assert.AreEqual("[x, y]", m.GetAll("id").ToString());
        Assert.AreEqual("[x, y]", m.GetAll("ID").ToString());

        Assert.IsNull(m.Get("undefined"));
        Assert.AreEqual("[]", m.GetAll("undefined").ToString());
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
        Assert.AreEqual("{ID=[x, y, z], a=[x, z], b=[y]}", m.GetLabels().ToString());
        Assert.IsNotNull(m.Get("a")); // get first
        Assert.IsNotNull(m.Get("b"));
        Assert.IsNotNull(m.Get("ID"));
        Assert.AreEqual("z", m.Get("a").Text);
        Assert.AreEqual("y", m.Get("b").Text);
        Assert.AreEqual("z", m.Get("ID").Text); // get last
        Assert.AreEqual("[x, z]", m.GetAll("a").ToString());
        Assert.AreEqual("[y]", m.GetAll("b").ToString());
        Assert.AreEqual("[x, y, z]", m.GetAll("ID").ToString()); // ordered

        Assert.AreEqual("xyz;", m.GetTree().Text); // whitespace stripped by lexer

        Assert.IsNull(m.Get("undefined"));
        Assert.AreEqual("[]", m.GetAll("undefined").ToString());
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

            var p = parser.CompileParseTreePattern(pattern, parser.GetRuleIndex(startRule));

            var match = p.Match(executedState.parseTree);
            bool matched = match.Succeeded();
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
