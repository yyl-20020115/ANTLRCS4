/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime;
using org.antlr.v4.runtime.atn;
using org.antlr.v4.runtime.misc;
using org.antlr.v4.runtime.tree.pattern;
using org.antlr.v4.test.runtime;
using org.antlr.v4.test.runtime.java;
using org.antlr.v4.test.runtime.states;
using org.antlr.v4.tool;

namespace org.antlr.v4.test.tool;

//@SuppressWarnings("unused")
[TestClass]
public class TestParserProfiler
{
    readonly static LexerGrammar lg;

    static TestParserProfiler()
    {
        try
        {
            lg = new LexerGrammar(
                    "lexer grammar L;\n" +
                            "WS : [ \\r\\t\\n]+ -> channel(HIDDEN) ;\n" +
                            "SEMI : ';' ;\n" +
                            "DOT : '.' ;\n" +
                            "ID : [a-zA-Z]+ ;\n" +
                            "INT : [0-9]+ ;\n" +
                            "PLUS : '+' ;\n" +
                            "MULT : '*' ;\n");
        }
        catch (RecognitionException e)
        {
            throw new RuntimeException(e.Message, e);
        }
    }

    [TestMethod]
    public void TestLL1()
    {
        var g = new Grammar(
                "parser grammar T;\n" +
                "s : ';'{}\n" +
                "  | '.'\n" +
                "  ;\n",
                lg);

        var info = InterpAndGetDecisionInfo(lg, g, "s", ";");
        Assert.AreEqual(1, info.Length);
        var expecting =
                "{decision=0, contextSensitivities=0, errors=0, ambiguities=0, SLL_lookahead=1, " +
                "SLL_ATNTransitions=1, SLL_DFATransitions=0, LL_Fallback=0, LL_lookahead=0, LL_ATNTransitions=0}";
        Assert.AreEqual(expecting, info[0].ToString());
    }

    [TestMethod]
    public void TestLL2()
    {
        var g = new Grammar(
                "parser grammar T;\n" +
                "s : ID ';'{}\n" +
                "  | ID '.'\n" +
                "  ;\n",
                lg);

        var info = InterpAndGetDecisionInfo(lg, g, "s", "xyz;");
        Assert.AreEqual(1, info.Length);
        var expecting =
                "{decision=0, contextSensitivities=0, errors=0, ambiguities=0, SLL_lookahead=2, " +
                "SLL_ATNTransitions=2, SLL_DFATransitions=0, LL_Fallback=0, LL_lookahead=0, LL_ATNTransitions=0}";
        Assert.AreEqual(expecting, info[0].ToString());
    }

    [TestMethod]
    public void TestRepeatedLL2()
    {
        var g = new Grammar(
                "parser grammar T;\n" +
                "s : ID ';'{}\n" +
                "  | ID '.'\n" +
                "  ;\n",
                lg);

        var info = InterpAndGetDecisionInfo(lg, g, "s", "xyz;", "abc;");
        Assert.AreEqual(1, info.Length);
        var expecting =
                "{decision=0, contextSensitivities=0, errors=0, ambiguities=0, SLL_lookahead=4, " +
                "SLL_ATNTransitions=2, SLL_DFATransitions=2, LL_Fallback=0, LL_lookahead=0, LL_ATNTransitions=0}";
        Assert.AreEqual(expecting, info[0].ToString());
    }

    [TestMethod]
    public void Test3xLL2()
    {
        var g = new Grammar(
                "parser grammar T;\n" +
                "s : ID ';'{}\n" +
                "  | ID '.'\n" +
                "  ;\n",
                lg);

        // The '.' vs ';' causes another ATN transition
        var info = InterpAndGetDecisionInfo(lg, g, "s", "xyz;", "abc;", "z.");
        Assert.AreEqual(1, info.Length);
        var expecting =
                "{decision=0, contextSensitivities=0, errors=0, ambiguities=0, SLL_lookahead=6, " +
                "SLL_ATNTransitions=3, SLL_DFATransitions=3, LL_Fallback=0, LL_lookahead=0, LL_ATNTransitions=0}";
        Assert.AreEqual(expecting, info[0].ToString());
    }

    [TestMethod]
    public void TestOptional()
    {
        var g = new Grammar(
                "parser grammar T;\n" +
                "s : ID ('.' ID)? ';'\n" +
                "  | ID INT \n" +
                "  ;\n",
                lg);

        var info = InterpAndGetDecisionInfo(lg, g, "s", "a.b;");
        Assert.AreEqual(2, info.Length);
        var expecting =
            "[{decision=0, contextSensitivities=0, errors=0, ambiguities=0, SLL_lookahead=1, " +
              "SLL_ATNTransitions=1, SLL_DFATransitions=0, LL_Fallback=0, LL_lookahead=0, LL_ATNTransitions=0}, " +
             "{decision=1, contextSensitivities=0, errors=0, ambiguities=0, SLL_lookahead=2, " +
              "SLL_ATNTransitions=2, SLL_DFATransitions=0, LL_Fallback=0, LL_lookahead=0, LL_ATNTransitions=0}]";
        Assert.AreEqual(expecting, Arrays.ToString(info));
    }

    [TestMethod]
    public void Test2xOptional()
    {
        var g = new Grammar(
                "parser grammar T;\n" +
                "s : ID ('.' ID)? ';'\n" +
                "  | ID INT \n" +
                "  ;\n",
                lg);

        var info = InterpAndGetDecisionInfo(lg, g, "s", "a.b;", "a.b;");
        Assert.AreEqual(2, info.Length);
        var expecting =
            "[{decision=0, contextSensitivities=0, errors=0, ambiguities=0, SLL_lookahead=2, " +
              "SLL_ATNTransitions=1, SLL_DFATransitions=1, LL_Fallback=0, LL_lookahead=0, LL_ATNTransitions=0}, " +
             "{decision=1, contextSensitivities=0, errors=0, ambiguities=0, SLL_lookahead=4, " +
              "SLL_ATNTransitions=2, SLL_DFATransitions=2, LL_Fallback=0, LL_lookahead=0, LL_ATNTransitions=0}]";
        Assert.AreEqual(expecting, Arrays.ToString(info));
    }

    [TestMethod]
    public void TestContextSensitivity()
    {
        var g = new Grammar(
            "parser grammar T;\n" +
            "a : '.' e ID \n" +
            "  | ';' e INT ID ;\n" +
            "e : INT | ;\n",
            lg);
        var info = InterpAndGetDecisionInfo(lg, g, "a", "; 1 x");
        Assert.AreEqual(2, info.Length);
        var expecting =
            "{decision=1, contextSensitivities=1, errors=0, ambiguities=0, SLL_lookahead=3, SLL_ATNTransitions=2, SLL_DFATransitions=0, LL_Fallback=1, LL_lookahead=3, LL_ATNTransitions=2}";
        Assert.AreEqual(expecting, info[1].ToString());
    }


    [TestMethod]
    public void TestSimpleLanguage()
    {
        var g = new Grammar(TestXPath.grammar);
        var input =
            "def f(x,y) { x = 3+4*1*1/5*1*1+1*1+1; y; ; }\n" +
            "def g(x,a,b,c,d,e) { return 1+2*x; }\n" +
            "def h(x) { a=3; x=0+1; return a*x; }\n";
        var info = InterpAndGetDecisionInfo(g.getImplicitLexer(), g, "prog", input);
        var expecting =
            "[{decision=0, contextSensitivities=1, errors=0, ambiguities=0, SLL_lookahead=3, " +
            "SLL_ATNTransitions=2, SLL_DFATransitions=0, LL_Fallback=1, LL_ATNTransitions=1}]";


        Assert.AreEqual(expecting, Arrays.ToString(info));
        Assert.AreEqual(1, info.Length);
    }

    //@Disabled
    [TestMethod]
    public void TestDeepLookahead()
    {
        var g = new Grammar(
                "parser grammar T;\n" +
                "s : e ';'\n" +
                "  | e '.' \n" +
                "  ;\n" +
                "e : (ID|INT) ({true}? '+' e)*\n" +       // d=1 entry, d=2 bypass
                "  ;\n",
                lg);

        // pred forces to
        // ambig and ('+' e)* tail recursion forces lookahead to fall out of e
        // any non-precedence predicates are always evaluated as true by the interpreter
        var info = InterpAndGetDecisionInfo(lg, g, "s", "a+b+c;");
        // at "+b" it uses k=1 and enters loop then calls e for b...
        // e matches and d=2 uses "+c;" for k=3
        Assert.AreEqual(2, info.Length);
        var expecting =
            "[{decision=0, contextSensitivities=0, errors=0, ambiguities=0, SLL_lookahead=6, " +
              "SLL_ATNTransitions=6, SLL_DFATransitions=0, LL_Fallback=0, LL_lookahead=0, LL_ATNTransitions=0}, " +
             "{decision=1, contextSensitivities=0, errors=0, ambiguities=0, SLL_lookahead=4, " +
              "SLL_ATNTransitions=2, SLL_DFATransitions=2, LL_Fallback=0, LL_lookahead=0, LL_ATNTransitions=0}]";
        Assert.AreEqual(expecting, Arrays.ToString(info));
    }

    [TestMethod]
    public void TestProfilerGeneratedCode()
    {
        var grammar =
            "grammar T;\n" +
            "s : a+ ID EOF ;\n" +
            "a : ID ';'{}\n" +
            "  | ID '.'\n" +
            "  ;\n" +
            "WS : [ \\r\\t\\n]+ -> channel(HIDDEN) ;\n" +
            "SEMI : ';' ;\n" +
            "DOT : '.' ;\n" +
            "ID : [a-zA-Z]+ ;\n" +
            "INT : [0-9]+ ;\n" +
            "PLUS : '+' ;\n" +
            "MULT : '*' ;\n";

        var runOptions = ToolTestUtils.CreateOptionsForJavaToolTests("T.g4", grammar, "TParser", "TLexer",
                false, false, "s", "xyz;abc;z.q",
                true, false, Stage.Execute, false);
        var runner = new JavaRunner();
        {
            var state = (ExecutedState)runner.Run(runOptions);
            var expecting =
                    "[{decision=0, contextSensitivities=0, errors=0, ambiguities=0, SLL_lookahead=6, SLL_ATNTransitions=4, " +
                            "SLL_DFATransitions=2, LL_Fallback=0, LL_lookahead=0, LL_ATNTransitions=0}," +
                            " {decision=1, contextSensitivities=0, errors=0, ambiguities=0, SLL_lookahead=6, " +
                            "SLL_ATNTransitions=3, SLL_DFATransitions=3, LL_Fallback=0, LL_lookahead=0, LL_ATNTransitions=0}]\n";
            Assert.AreEqual(expecting, state.output);
            Assert.AreEqual("", state.errors);
        }
    }

    public DecisionInfo[] InterpAndGetDecisionInfo(
            LexerGrammar lg, Grammar g,
            string startRule, params string[] input)
    {

        var lexEngine = lg.createLexerInterpreter(null);
        var parser = g.createParserInterpreter(null);
        parser.setProfile(true);
        foreach (var s in input)
        {
            lexEngine.Reset();
            parser.reset();
            lexEngine.SetInputStream(new ANTLRInputStream(s));
            var tokens = new CommonTokenStream(lexEngine);
            parser.SetInputStream(tokens);
            if (!g.rules.TryGetValue(startRule, out var r))
            {
                return parser.getParseInfo().GetDecisionInfo();
            }
            var t = parser.parse(r.index);
            //			try {
            //				Utils.waitForClose(t.inspect(parser).get());
            //			}
            //			catch (Exception e) {
            //				e.printStackTrace();
            //			}
            //
            //			Console.Out.WriteLine(t.toStringTree(parser));
        }
        return parser.getParseInfo().GetDecisionInfo();
    }
}
