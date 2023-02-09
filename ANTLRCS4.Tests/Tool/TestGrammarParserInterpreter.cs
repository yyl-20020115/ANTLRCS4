/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */
using org.antlr.v4.runtime;
using org.antlr.v4.runtime.tree;
using org.antlr.v4.tool;

namespace org.antlr.v4.test.tool;


/** Tests to ensure GrammarParserInterpreter subclass of ParserInterpreter
 *  hasn't messed anything up.
 */
[TestClass]
public class TestGrammarParserInterpreter
{
    public static readonly string lexerText = "lexer grammar L;\n" +
                                           "PLUS : '+' ;\n" +
                                           "MULT : '*' ;\n" +
                                           "ID : [a-z]+ ;\n" +
                                           "INT : [0-9]+ ;\n" +
                                           "WS : [ \\r\\t\\n]+ ;\n";

    [TestMethod]
    public void TestAlts()
    {
        var lg = new LexerGrammar(lexerText);
        var g = new Grammar(
            "parser grammar T;\n" +
            "s : ID\n" +
            "  | INT{;}\n" +
            "  ;\n",
            lg);
        TestInterp(lg, g, "s", "a", "(s:1 a)");
        TestInterp(lg, g, "s", "3", "(s:2 3)");
    }

    [TestMethod]
    public void TestAltsAsSet()
    {
        var lg = new LexerGrammar(lexerText);
        var g = new Grammar(
            "parser grammar T;\n" +
            "s : ID\n" +
            "  | INT\n" +
            "  ;\n",
            lg);
        TestInterp(lg, g, "s", "a", "(s:1 a)");
        TestInterp(lg, g, "s", "3", "(s:1 3)");
    }

    [TestMethod]
    public void TestAltsWithLabels()
    {
        var lg = new LexerGrammar(lexerText);
        var g = new Grammar(
            "parser grammar T;\n" +
            "s : ID  # foo\n" +
            "  | INT # bar\n" +
            "  ;\n",
            lg);
        // it won't show the labels here because my simple node text provider above just shows the alternative
        TestInterp(lg, g, "s", "a", "(s:1 a)");
        TestInterp(lg, g, "s", "3", "(s:2 3)");
    }

    [TestMethod]
    public void TestOneAlt()
    {
        var lg = new LexerGrammar(lexerText);
        var g = new Grammar(
            "parser grammar T;\n" +
            "s : ID\n" +
            "  ;\n",
            lg);
        TestInterp(lg, g, "s", "a", "(s:1 a)");
    }


    [TestMethod]
    public void TestLeftRecursionWithMultiplePrimaryAndRecursiveOps()
    {
        var lg = new LexerGrammar(lexerText);
        var g = new Grammar(
            "parser grammar T;\n" +
            "s : e EOF ;\n" +
            "e : e MULT e\n" +
            "  | e PLUS e\n" +
            "  | INT\n" +
            "  | ID\n" +
            "  ;\n",
            lg);

        TestInterp(lg, g, "s", "a", "(s:1 (e:4 a) <EOF>)");
        TestInterp(lg, g, "e", "a", "(e:4 a)");
        TestInterp(lg, g, "e", "34", "(e:3 34)");
        TestInterp(lg, g, "e", "a+1", "(e:2 (e:4 a) + (e:3 1))");
        TestInterp(lg, g, "e", "1+2*a", "(e:2 (e:3 1) + (e:1 (e:3 2) * (e:4 a)))");
    }

    static InterpreterRuleContext TestInterp(LexerGrammar lg, Grammar g,
                                      string startRule, string input,
                                      string expectedParseTree)
    {
        var lexEngine = lg.createLexerInterpreter(new ANTLRInputStream(input));
        var tokens = new CommonTokenStream(lexEngine);
        var parser = g.createGrammarParserInterpreter(tokens);
        var t = parser.Parse(g.rules[(startRule)].index);
        var nodeTextProvider = new InterpreterTreeTextProvider(g.getRuleNames());
        var treeStr = Trees.ToStringTree(t, nodeTextProvider);
        //		Console.Out.WriteLine("parse tree: "+treeStr);
        Assert.AreEqual(expectedParseTree, treeStr);
        return (InterpreterRuleContext)t;
    }
}
