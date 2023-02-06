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
public class TestParserProfiler {
	readonly static LexerGrammar lg;

	static TestParserProfiler(){
		try {
			lg = new LexerGrammar(
					"lexer grammar L;\n" +
							"WS : [ \\r\\t\\n]+ -> channel(HIDDEN) ;\n" +
							"SEMI : ';' ;\n" +
							"DOT : '.' ;\n" +
							"ID : [a-zA-Z]+ ;\n" +
							"INT : [0-9]+ ;\n" +
							"PLUS : '+' ;\n" +
							"MULT : '*' ;\n");
		} catch (RecognitionException e) {
			throw new RuntimeException(e.Message,e);
		}
	}

	[TestMethod] public void testLL1(){
		Grammar g = new Grammar(
				"parser grammar T;\n" +
				"s : ';'{}\n" +
				"  | '.'\n" +
				"  ;\n",
				lg);

		DecisionInfo[] info = interpAndGetDecisionInfo(lg, g, "s", ";");
		Assert.AreEqual(1, info.Length);
		String expecting =
				"{decision=0, contextSensitivities=0, errors=0, ambiguities=0, SLL_lookahead=1, " +
				"SLL_ATNTransitions=1, SLL_DFATransitions=0, LL_Fallback=0, LL_lookahead=0, LL_ATNTransitions=0}";
		Assert.AreEqual(expecting, info[0].ToString());
	}

	[TestMethod] public void testLL2(){
		Grammar g = new Grammar(
				"parser grammar T;\n" +
				"s : ID ';'{}\n" +
				"  | ID '.'\n" +
				"  ;\n",
				lg);

		DecisionInfo[] info = interpAndGetDecisionInfo(lg, g, "s", "xyz;");
		Assert.AreEqual(1, info.Length);
		String expecting =
				"{decision=0, contextSensitivities=0, errors=0, ambiguities=0, SLL_lookahead=2, " +
				"SLL_ATNTransitions=2, SLL_DFATransitions=0, LL_Fallback=0, LL_lookahead=0, LL_ATNTransitions=0}";
		Assert.AreEqual(expecting, info[0].ToString());
	}

	[TestMethod] public void testRepeatedLL2(){
		Grammar g = new Grammar(
				"parser grammar T;\n" +
				"s : ID ';'{}\n" +
				"  | ID '.'\n" +
				"  ;\n",
				lg);

		DecisionInfo[] info = interpAndGetDecisionInfo(lg, g, "s", "xyz;", "abc;");
		Assert.AreEqual(1, info.Length);
		String expecting =
				"{decision=0, contextSensitivities=0, errors=0, ambiguities=0, SLL_lookahead=4, " +
				"SLL_ATNTransitions=2, SLL_DFATransitions=2, LL_Fallback=0, LL_lookahead=0, LL_ATNTransitions=0}";
		Assert.AreEqual(expecting, info[0].ToString());
	}

	[TestMethod] public void test3xLL2(){
		Grammar g = new Grammar(
				"parser grammar T;\n" +
				"s : ID ';'{}\n" +
				"  | ID '.'\n" +
				"  ;\n",
				lg);

		// The '.' vs ';' causes another ATN transition
		DecisionInfo[] info = interpAndGetDecisionInfo(lg, g, "s", "xyz;", "abc;", "z.");
		Assert.AreEqual(1, info.Length);
		String expecting =
				"{decision=0, contextSensitivities=0, errors=0, ambiguities=0, SLL_lookahead=6, " +
				"SLL_ATNTransitions=3, SLL_DFATransitions=3, LL_Fallback=0, LL_lookahead=0, LL_ATNTransitions=0}";
		Assert.AreEqual(expecting, info[0].ToString());
	}

	[TestMethod] public void testOptional(){
		Grammar g = new Grammar(
				"parser grammar T;\n" +
				"s : ID ('.' ID)? ';'\n" +
				"  | ID INT \n" +
				"  ;\n",
				lg);

		DecisionInfo[] info = interpAndGetDecisionInfo(lg, g, "s", "a.b;");
		Assert.AreEqual(2, info.Length);
		String expecting =
			"[{decision=0, contextSensitivities=0, errors=0, ambiguities=0, SLL_lookahead=1, " +
			  "SLL_ATNTransitions=1, SLL_DFATransitions=0, LL_Fallback=0, LL_lookahead=0, LL_ATNTransitions=0}, " +
			 "{decision=1, contextSensitivities=0, errors=0, ambiguities=0, SLL_lookahead=2, " +
			  "SLL_ATNTransitions=2, SLL_DFATransitions=0, LL_Fallback=0, LL_lookahead=0, LL_ATNTransitions=0}]";
		Assert.AreEqual(expecting, Arrays.ToString(info));
	}

	[TestMethod] public void test2xOptional(){
		Grammar g = new Grammar(
				"parser grammar T;\n" +
				"s : ID ('.' ID)? ';'\n" +
				"  | ID INT \n" +
				"  ;\n",
				lg);

		DecisionInfo[] info = interpAndGetDecisionInfo(lg, g, "s", "a.b;", "a.b;");
		Assert.AreEqual(2, info.Length);
		String expecting =
			"[{decision=0, contextSensitivities=0, errors=0, ambiguities=0, SLL_lookahead=2, " +
			  "SLL_ATNTransitions=1, SLL_DFATransitions=1, LL_Fallback=0, LL_lookahead=0, LL_ATNTransitions=0}, " +
			 "{decision=1, contextSensitivities=0, errors=0, ambiguities=0, SLL_lookahead=4, " +
			  "SLL_ATNTransitions=2, SLL_DFATransitions=2, LL_Fallback=0, LL_lookahead=0, LL_ATNTransitions=0}]";
		Assert.AreEqual(expecting, Arrays.ToString(info));
	}

	[TestMethod] public void testContextSensitivity(){
		Grammar g = new Grammar(
			"parser grammar T;\n"+
			"a : '.' e ID \n" +
			"  | ';' e INT ID ;\n" +
			"e : INT | ;\n",
			lg);
		DecisionInfo[] info = interpAndGetDecisionInfo(lg, g, "a", "; 1 x");
		Assert.AreEqual(2, info.Length);
		String expecting =
			"{decision=1, contextSensitivities=1, errors=0, ambiguities=0, SLL_lookahead=3, SLL_ATNTransitions=2, SLL_DFATransitions=0, LL_Fallback=1, LL_lookahead=3, LL_ATNTransitions=2}";
		Assert.AreEqual(expecting, info[1].ToString());
	}

	
	[TestMethod] public void testSimpleLanguage(){
		Grammar g = new Grammar(TestXPath.grammar);
		String input =
			"def f(x,y) { x = 3+4*1*1/5*1*1+1*1+1; y; ; }\n" +
			"def g(x,a,b,c,d,e) { return 1+2*x; }\n"+
			"def h(x) { a=3; x=0+1; return a*x; }\n";
		DecisionInfo[] info = interpAndGetDecisionInfo(g.getImplicitLexer(), g, "prog", input);
		String expecting =
			"[{decision=0, contextSensitivities=1, errors=0, ambiguities=0, SLL_lookahead=3, " +
			"SLL_ATNTransitions=2, SLL_DFATransitions=0, LL_Fallback=1, LL_ATNTransitions=1}]";


		Assert.AreEqual(expecting, Arrays.ToString(info));
		Assert.AreEqual(1, info.Length);
	}

	//@Disabled
	[TestMethod] public void testDeepLookahead(){
		Grammar g = new Grammar(
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
		DecisionInfo[] info = interpAndGetDecisionInfo(lg, g, "s", "a+b+c;");
		// at "+b" it uses k=1 and enters loop then calls e for b...
		// e matches and d=2 uses "+c;" for k=3
		Assert.AreEqual(2, info.Length);
		String expecting =
			"[{decision=0, contextSensitivities=0, errors=0, ambiguities=0, SLL_lookahead=6, " +
			  "SLL_ATNTransitions=6, SLL_DFATransitions=0, LL_Fallback=0, LL_lookahead=0, LL_ATNTransitions=0}, " +
			 "{decision=1, contextSensitivities=0, errors=0, ambiguities=0, SLL_lookahead=4, " +
			  "SLL_ATNTransitions=2, SLL_DFATransitions=2, LL_Fallback=0, LL_lookahead=0, LL_ATNTransitions=0}]";
		Assert.AreEqual(expecting, Arrays.ToString(info));
	}

	[TestMethod] public void testProfilerGeneratedCode() {
		String grammar =
			"grammar T;\n" +
			"s : a+ ID EOF ;\n" +
			"a : ID ';'{}\n" +
			"  | ID '.'\n" +
			"  ;\n"+
			"WS : [ \\r\\t\\n]+ -> channel(HIDDEN) ;\n" +
			"SEMI : ';' ;\n" +
			"DOT : '.' ;\n" +
			"ID : [a-zA-Z]+ ;\n" +
			"INT : [0-9]+ ;\n" +
			"PLUS : '+' ;\n" +
			"MULT : '*' ;\n";

		RunOptions runOptions =ToolTestUtils.createOptionsForJavaToolTests("T.g4", grammar, "TParser", "TLexer",
				false, false, "s", "xyz;abc;z.q",
				true, false, Stage.Execute, false);
		JavaRunner runner = new JavaRunner();
		{
			ExecutedState state = (ExecutedState) runner.Run(runOptions);
			String expecting =
					"[{decision=0, contextSensitivities=0, errors=0, ambiguities=0, SLL_lookahead=6, SLL_ATNTransitions=4, " +
							"SLL_DFATransitions=2, LL_Fallback=0, LL_lookahead=0, LL_ATNTransitions=0}," +
							" {decision=1, contextSensitivities=0, errors=0, ambiguities=0, SLL_lookahead=6, " +
							"SLL_ATNTransitions=3, SLL_DFATransitions=3, LL_Fallback=0, LL_lookahead=0, LL_ATNTransitions=0}]\n";
			Assert.AreEqual(expecting, state.output);
			Assert.AreEqual("", state.errors);
		}
	}

	public DecisionInfo[] interpAndGetDecisionInfo(
			LexerGrammar lg, Grammar g,
			String startRule, params String[] input)
	{

		LexerInterpreter lexEngine = lg.createLexerInterpreter(null);
		ParserInterpreter parser = g.createParserInterpreter(null);
		parser.setProfile(true);
		foreach (String s in input) {
			lexEngine.reset();
			parser.reset();
			lexEngine.setInputStream(new ANTLRInputStream(s));
			CommonTokenStream tokens = new CommonTokenStream(lexEngine);
			parser.setInputStream(tokens);
			if ( !g.rules.TryGetValue(startRule,out var r)) {
				return parser.getParseInfo().getDecisionInfo();
			}
			ParserRuleContext t = parser.parse(r.index);
//			try {
//				Utils.waitForClose(t.inspect(parser).get());
//			}
//			catch (Exception e) {
//				e.printStackTrace();
//			}
//
//			Console.Out.WriteLine(t.toStringTree(parser));
		}
		return parser.getParseInfo().getDecisionInfo();
	}
}
