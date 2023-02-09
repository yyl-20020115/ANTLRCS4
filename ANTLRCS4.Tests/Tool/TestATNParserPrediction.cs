/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.automata;
using org.antlr.v4.runtime;
using org.antlr.v4.runtime.atn;
using org.antlr.v4.runtime.dfa;
using org.antlr.v4.runtime.misc;
using org.antlr.v4.tool;

namespace org.antlr.v4.test.tool;


// NOTICE: TOKENS IN LEXER, PARSER MUST BE SAME OR TOKEN TYPE MISMATCH
// NOTICE: TOKENS IN LEXER, PARSER MUST BE SAME OR TOKEN TYPE MISMATCH
// NOTICE: TOKENS IN LEXER, PARSER MUST BE SAME OR TOKEN TYPE MISMATCH
[TestClass]
public class TestATNParserPrediction
{
    [TestMethod]
    public void TestAorB()
    {
        var lg = new LexerGrammar(
        "lexer grammar L;\n" +
        "A : 'a' ;\n" +
        "B : 'b' ;\n" +
        "C : 'c' ;\n");
        var g = new Grammar(
        "parser grammar T;\n" +
        "a : A{;} | B ;");
        int decision = 0;
        CheckPredictedAlt(lg, g, decision, "a", 1);
        CheckPredictedAlt(lg, g, decision, "b", 2);

        // After matching these inputs for decision, what is DFA after each prediction?
        string[] inputs = {
        "a",
        "b",
        "a"
        };
        string[] dfa = {
        "s0-'a'->:s1=>1\n",

        "s0-'a'->:s1=>1\n" +
        "s0-'b'->:s2=>2\n",

        "s0-'a'->:s1=>1\n" + // don't change after it works
		"s0-'b'->:s2=>2\n",
        };
        CheckDFAConstruction(lg, g, decision, inputs, dfa);
    }

    [TestMethod]
    public void TestEmptyInput()
    {
        var lg = new LexerGrammar(
        "lexer grammar L;\n" +
        "A : 'a' ;\n" +
        "B : 'b' ;\n" +
        "C : 'c' ;\n");
        var g = new Grammar(
        "parser grammar T;\n" +
        "a : A | ;");
        int decision = 0;
        CheckPredictedAlt(lg, g, decision, "a", 1);
        CheckPredictedAlt(lg, g, decision, "", 2);

        // After matching these inputs for decision, what is DFA after each prediction?
        string[] inputs = {
        "a",
        "",
        };
        string[] dfa = {
        "s0-'a'->:s1=>1\n",

        "s0-EOF->:s2=>2\n" +
        "s0-'a'->:s1=>1\n",
        };
        CheckDFAConstruction(lg, g, decision, inputs, dfa);
    }

    [TestMethod]
    public void TestPEGAchillesHeel()
    {
        var lg = new LexerGrammar(
        "lexer grammar L;\n" +
        "A : 'a' ;\n" +
        "B : 'b' ;\n" +
        "C : 'c' ;\n");
        var g = new Grammar(
        "parser grammar T;\n" +
        "a : A | A B ;");
        CheckPredictedAlt(lg, g, 0, "a", 1);
        CheckPredictedAlt(lg, g, 0, "ab", 2);
        CheckPredictedAlt(lg, g, 0, "abc", 2);

        string[] inputs = {
        "a",
        "ab",
        "abc"
        };
        string[] dfa = {
        "s0-'a'->s1\n" +
        "s1-EOF->:s2=>1\n",

        "s0-'a'->s1\n" +
        "s1-EOF->:s2=>1\n" +
        "s1-'b'->:s3=>2\n",

        "s0-'a'->s1\n" +
        "s1-EOF->:s2=>1\n" +
        "s1-'b'->:s3=>2\n"
        };
        CheckDFAConstruction(lg, g, 0, inputs, dfa);
    }

    [TestMethod]
    public void TestRuleRefxory()
    {
        var lg = new LexerGrammar(
        "lexer grammar L;\n" +
        "A : 'a' ;\n" +
        "B : 'b' ;\n" +
        "C : 'c' ;\n");
        var g = new Grammar(
        "parser grammar T;\n" +
        "a : x | y ;\n" +
        "x : A ;\n" +
        "y : B ;\n");
        int decision = 0;
        CheckPredictedAlt(lg, g, decision, "a", 1);
        CheckPredictedAlt(lg, g, decision, "b", 2);

        // After matching these inputs for decision, what is DFA after each prediction?
        string[] inputs = {
        "a",
        "b",
        "a"
        };
        string[] dfa = {
        "s0-'a'->:s1=>1\n",

        "s0-'a'->:s1=>1\n" +
        "s0-'b'->:s2=>2\n",

        "s0-'a'->:s1=>1\n" + // don't change after it works
		"s0-'b'->:s2=>2\n",
        };
        CheckDFAConstruction(lg, g, decision, inputs, dfa);
    }

    [TestMethod]
    public void TestOptionalRuleChasesGlobalFollow()
    {
        var lg = new LexerGrammar(
        "lexer grammar L;\n" +
        "A : 'a' ;\n" +
        "B : 'b' ;\n" +
        "C : 'c' ;\n");
        var g = new Grammar(
        "parser grammar T;\n" +
        "tokens {A,B,C}\n" +
        "a : x B ;\n" +
        "b : x C ;\n" +
        "x : A | ;\n");
        int decision = 0;
        CheckPredictedAlt(lg, g, decision, "a", 1);
        CheckPredictedAlt(lg, g, decision, "b", 2);
        CheckPredictedAlt(lg, g, decision, "c", 2);

        // After matching these inputs for decision, what is DFA after each prediction?
        string[] inputs = {
        "a",
        "b",
        "c",
        "c",
        };
        string[] dfa = {
        "s0-'a'->:s1=>1\n",

        "s0-'a'->:s1=>1\n" +
        "s0-'b'->:s2=>2\n",

        "s0-'a'->:s1=>1\n" +
        "s0-'b'->:s2=>2\n" +
        "s0-'c'->:s3=>2\n",

        "s0-'a'->:s1=>1\n" +
        "s0-'b'->:s2=>2\n" +
        "s0-'c'->:s3=>2\n",
        };
        CheckDFAConstruction(lg, g, decision, inputs, dfa);
    }

    [TestMethod]
    public void TestLL1Ambig()
    {
        var lg = new LexerGrammar(
        "lexer grammar L;\n" +
        "A : 'a' ;\n" +
        "B : 'b' ;\n" +
        "C : 'c' ;\n");
        var g = new Grammar(
        "parser grammar T;\n" +
        "a : A | A | A B ;");
        int decision = 0;
        CheckPredictedAlt(lg, g, decision, "a", 1);
        CheckPredictedAlt(lg, g, decision, "ab", 3);

        // After matching these inputs for decision, what is DFA after each prediction?
        string[] inputs = {
        "a",
        "ab",
        "ab"
        };
        string[] dfa = {
        "s0-'a'->s1\n" +
        "s1-EOF->:s2^=>1\n",

        "s0-'a'->s1\n" +
        "s1-EOF->:s2^=>1\n" +
        "s1-'b'->:s3=>3\n",

        "s0-'a'->s1\n" +
        "s1-EOF->:s2^=>1\n" +
        "s1-'b'->:s3=>3\n",
        };
        CheckDFAConstruction(lg, g, decision, inputs, dfa);
    }

    [TestMethod]
    public void TestLL2Ambig()
    {
        var lg = new LexerGrammar(
        "lexer grammar L;\n" +
        "A : 'a' ;\n" +
        "B : 'b' ;\n" +
        "C : 'c' ;\n");
        var g = new Grammar(
        "parser grammar T;\n" +
        "a : A B | A B | A B C ;");
        int decision = 0;
        CheckPredictedAlt(lg, g, decision, "ab", 1);
        CheckPredictedAlt(lg, g, decision, "abc", 3);

        // After matching these inputs for decision, what is DFA after each prediction?
        string[] inputs = {
        "ab",
        "abc",
        "ab"
        };
        string[] dfa = {
        "s0-'a'->s1\n" +
        "s1-'b'->s2\n" +
        "s2-EOF->:s3^=>1\n",

        "s0-'a'->s1\n" +
        "s1-'b'->s2\n" +
        "s2-EOF->:s3^=>1\n" +
        "s2-'c'->:s4=>3\n",

        "s0-'a'->s1\n" +
        "s1-'b'->s2\n" +
        "s2-EOF->:s3^=>1\n" +
        "s2-'c'->:s4=>3\n",
        };
        CheckDFAConstruction(lg, g, decision, inputs, dfa);
    }

    [TestMethod]
    public void TestRecursiveLeftPrefix()
    {
        var lg = new LexerGrammar(
        "lexer grammar L;\n" +
        "A : 'a' ;\n" +
        "B : 'b' ;\n" +
        "C : 'c' ;\n" +
        "LP : '(' ;\n" +
        "RP : ')' ;\n" +
        "INT : '0'..'9'+ ;\n"
        );
        var g = new Grammar(
        "parser grammar T;\n" +
        "tokens {A,B,C,LP,RP,INT}\n" +
        "a : e B | e C ;\n" +
        "e : LP e RP\n" +
        "  | INT\n" +
        "  ;");
        int decision = 0;
        CheckPredictedAlt(lg, g, decision, "34b", 1);
        CheckPredictedAlt(lg, g, decision, "34c", 2);
        CheckPredictedAlt(lg, g, decision, "((34))b", 1);
        CheckPredictedAlt(lg, g, decision, "((34))c", 2);

        // After matching these inputs for decision, what is DFA after each prediction?
        string[] inputs = {
        "34b",
        "34c",
        "((34))b",
        "((34))c"
        };
        string[] dfa = {
        "s0-INT->s1\n" +
        "s1-'b'->:s2=>1\n",

        "s0-INT->s1\n" +
        "s1-'b'->:s2=>1\n" +
        "s1-'c'->:s3=>2\n",

        "s0-'('->s4\n" +
        "s0-INT->s1\n" +
        "s1-'b'->:s2=>1\n" +
        "s1-'c'->:s3=>2\n" +
        "s4-'('->s5\n" +
        "s5-INT->s6\n" +
        "s6-')'->s7\n" +
        "s7-')'->s1\n",

        "s0-'('->s4\n" +
        "s0-INT->s1\n" +
        "s1-'b'->:s2=>1\n" +
        "s1-'c'->:s3=>2\n" +
        "s4-'('->s5\n" +
        "s5-INT->s6\n" +
        "s6-')'->s7\n" +
        "s7-')'->s1\n",
        };
        CheckDFAConstruction(lg, g, decision, inputs, dfa);
    }

    [TestMethod]
    public void TestRecursiveLeftPrefixWithAorABIssue()
    {
        var lg = new LexerGrammar(
        "lexer grammar L;\n" +
        "A : 'a' ;\n" +
        "B : 'b' ;\n" +
        "C : 'c' ;\n" +
        "LP : '(' ;\n" +
        "RP : ')' ;\n" +
        "INT : '0'..'9'+ ;\n"
        );
        var g = new Grammar(
        "parser grammar T;\n" +
        "tokens {A,B,C,LP,RP,INT}\n" +
        "a : e A | e A B ;\n" +
        "e : LP e RP\n" +
        "  | INT\n" +
        "  ;");
        int decision = 0;
        CheckPredictedAlt(lg, g, decision, "34a", 1);
        CheckPredictedAlt(lg, g, decision, "34ab", 2); // PEG would miss this one!
        CheckPredictedAlt(lg, g, decision, "((34))a", 1);
        CheckPredictedAlt(lg, g, decision, "((34))ab", 2);

        // After matching these inputs for decision, what is DFA after each prediction?
        string[] inputs = {
        "34a",
        "34ab",
        "((34))a",
        "((34))ab",
        };
        string[] dfa = {
        "s0-INT->s1\n" +
        "s1-'a'->s2\n" +
        "s2-EOF->:s3=>1\n",

        "s0-INT->s1\n" +
        "s1-'a'->s2\n" +
        "s2-EOF->:s3=>1\n" +
        "s2-'b'->:s4=>2\n",

        "s0-'('->s5\n" +
        "s0-INT->s1\n" +
        "s1-'a'->s2\n" +
        "s2-EOF->:s3=>1\n" +
        "s2-'b'->:s4=>2\n" +
        "s5-'('->s6\n" +
        "s6-INT->s7\n" +
        "s7-')'->s8\n" +
        "s8-')'->s1\n",

        "s0-'('->s5\n" +
        "s0-INT->s1\n" +
        "s1-'a'->s2\n" +
        "s2-EOF->:s3=>1\n" +
        "s2-'b'->:s4=>2\n" +
        "s5-'('->s6\n" +
        "s6-INT->s7\n" +
        "s7-')'->s8\n" +
        "s8-')'->s1\n",
        };
        CheckDFAConstruction(lg, g, decision, inputs, dfa);
    }

    [TestMethod]
    public void TestContinuePrediction()
    {
        // Sam found prev def of ambiguity was too restrictive.
        // E.g., (13, 1, []), (13, 2, []), (12, 2, []) should not
        // be declared ambig since (12, 2, []) can take us to
        // unambig state maybe. keep going.
        var lg = new LexerGrammar(
        "lexer grammar L;\n" +
        "ID : 'a'..'z' ;\n" + // one char
        "SEMI : ';' ;\n" +
        "INT : '0'..'9'+ ;\n"
        );
        var g = new Grammar(
        "parser grammar T;\n" +
        "tokens {ID,SEMI,INT}\n" +
        "a : (ID | ID ID?) SEMI ;");
        int decision = 1;
        CheckPredictedAlt(lg, g, decision, "a;", 1);
        CheckPredictedAlt(lg, g, decision, "ab;", 2);
    }

    [TestMethod]
    public void TestContinuePrediction2()
    {
        // ID is ambig for first two alts, but ID SEMI lets us move forward with alt 3
        var lg = new LexerGrammar(
        "lexer grammar L;\n" +
        "ID : 'a'..'z' ;\n" + // one char
        "SEMI : ';' ;\n" +
        "INT : '0'..'9'+ ;\n"
        );
        var g = new Grammar(
        "parser grammar T;\n" +
        "tokens {ID,SEMI,INT}\n" +
        "a : ID | ID | ID SEMI ;\n");
        int decision = 0;
        CheckPredictedAlt(lg, g, decision, "a", 1);
        CheckPredictedAlt(lg, g, decision, "a;", 3);
    }

    [TestMethod]
    public void TestAltsForLRRuleComputation()
    {
        var g = new Grammar(
        "grammar T;\n" +
        "e : e '*' e\n" +
        "  | INT\n" +
        "  | e '+' e\n" +
        "  | ID\n" +
        "  ;\n" +
        "ID : [a-z]+ ;\n" +
        "INT : [0-9]+ ;\n" +
        "WS : [ \\r\\t\\n]+ ;");
        var e = g.GetRule("e");
        Assert.IsTrue(e is LeftRecursiveRule);
        var lr = (LeftRecursiveRule)e;
        Assert.AreEqual("[0, 2, 4]", Arrays.ToString(lr.GetPrimaryAlts()));
        Assert.AreEqual("[0, 1, 3]", Arrays.ToString(lr.GetRecursiveOpAlts()));
    }

    [TestMethod]
    public void TestAltsForLRRuleComputation2()
    {
        var g = new Grammar(
        "grammar T;\n" +
        "e : INT\n" +
        "  | e '*' e\n" +
        "  | ID\n" +
        "  ;\n" +
        "ID : [a-z]+ ;\n" +
        "INT : [0-9]+ ;\n" +
        "WS : [ \\r\\t\\n]+ ;");
        var e = g.GetRule("e");
        Assert.IsTrue(e is LeftRecursiveRule);
        var lr = (LeftRecursiveRule)e;
        Assert.AreEqual("[0, 1, 3]", Arrays.ToString(lr.GetPrimaryAlts()));
        Assert.AreEqual("[0, 2]", Arrays.ToString(lr.GetRecursiveOpAlts()));
    }

    [TestMethod]
    public void TestAltsForLRRuleComputation3()
    {
        var g = new Grammar(
        "grammar T;\n" +
        "random : 'blort';\n" + // should have no effect
        "e : '--' e\n" +
        "  | e '*' e\n" +
        "  | e '+' e\n" +
        "  | e '--'\n" +
        "  | ID\n" +
        "  ;\n" +
        "ID : [a-z]+ ;\n" +
        "INT : [0-9]+ ;\n" +
        "WS : [ \\r\\t\\n]+ ;");
        var e = g.GetRule("e");
        Assert.IsTrue(e is LeftRecursiveRule);
        var lr = (LeftRecursiveRule)e;
        Assert.AreEqual("[0, 1, 5]", Arrays.ToString(lr.GetPrimaryAlts()));
        Assert.AreEqual("[0, 2, 3, 4]", Arrays.ToString(lr.GetRecursiveOpAlts()));
    }

    /** first check that the ATN predicts right alt.
	 *  Then check adaptive prediction.
	 */
    public static void CheckPredictedAlt(LexerGrammar lg, Grammar g, int decision,
                                  string inputString, int expectedAlt)
    {
        var lexatn = ToolTestUtils.CreateATN(lg, true);
        var lexInterp =
        new LexerATNSimulator(lexatn, new DFA[] { new DFA(lexatn.modeToStartState[(Lexer.DEFAULT_MODE)]) }, new PredictionContextCache());
        var types = ToolTestUtils.GetTokenTypesViaATN(inputString, lexInterp);
        //		Console.Out.WriteLine(types);

        ToolTestUtils.SemanticProcess(lg);
        g.ImportVocab(lg);
        ToolTestUtils.SemanticProcess(g);

        var f = new ParserATNFactory(g);
        var atn = f.CreateATN();

        var dot = new DOTGenerator(g);

        var r = g.GetRule("a");
        //		if ( r!=null) Console.Out.WriteLine(dot.getDOT(atn.ruleToStartState[r.index]));
        r = g.GetRule("b");
        //		if ( r!=null) Console.Out.WriteLine(dot.getDOT(atn.ruleToStartState[r.index]));
        r = g.GetRule("e");
        //		if ( r!=null) Console.Out.WriteLine(dot.getDOT(atn.ruleToStartState[r.index]));
        r = g.GetRule("ifstat");
        //		if ( r!=null) Console.Out.WriteLine(dot.getDOT(atn.ruleToStartState[r.index]));
        r = g.GetRule("block");
        //		if ( r!=null) Console.Out.WriteLine(dot.getDOT(atn.ruleToStartState[r.index]));

        // Check ATN prediction
        //		ParserATNSimulator interp = new ParserATNSimulator(atn);
        var input = new MockIntTokenStream(types);
        var interp = new ParserInterpreterForTesting(g, input);
        int alt = interp.AdaptivePredict(input, decision, ParserRuleContext.EMPTY);

        Assert.AreEqual(expectedAlt, alt);

        // Check adaptive prediction
        input.Seek(0);
        alt = interp.AdaptivePredict(input, decision, null);
        Assert.AreEqual(expectedAlt, alt);
        // run 2x; first time creates DFA in atn
        input.Seek(0);
        alt = interp.AdaptivePredict(input, decision, null);
        Assert.AreEqual(expectedAlt, alt);
    }

    public static void CheckDFAConstruction(LexerGrammar lg, Grammar g, int decision,
                                     string[] inputString, string[] dfaString)
    {
        //		Tool.internalOption_ShowATNConfigsInDFA = true;
        var lexatn = ToolTestUtils.CreateATN(lg, true);
        var lexInterp =
        new LexerATNSimulator(lexatn, new DFA[] { new DFA(lexatn.GetDecisionState(Lexer.DEFAULT_MODE)) }, new PredictionContextCache());

        ToolTestUtils.SemanticProcess(lg);
        g.ImportVocab(lg);
        ToolTestUtils.SemanticProcess(g);

        var interp = new ParserInterpreterForTesting(g, null);
        for (int i = 0; i < inputString.Length; i++)
        {
            // Check DFA
            var types = ToolTestUtils.GetTokenTypesViaATN(inputString[i], lexInterp);
            //			Console.Out.WriteLine(types);
            var input = new MockIntTokenStream(types);
            try
            {
                interp.AdaptivePredict(input, decision, ParserRuleContext.EMPTY);
            }
            catch (NoViableAltException nvae)
            {
                //nvae.printStackTrace(System.err);
            }
            var dfa = interp.parser.decisionToDFA[decision];
            Assert.AreEqual(dfaString[i], dfa.ToString(g.Vocabulary));
        }
    }
}
