/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */
using org.antlr.v4.automata;
using org.antlr.v4.parse;
using org.antlr.v4.runtime.atn;
using org.antlr.v4.runtime.misc;
using org.antlr.v4.tool;
using org.antlr.v4.tool.ast;

namespace org.antlr.v4.test.tool;

[TestClass]
public class TestATNConstruction {
	[TestMethod] public void testA(){
		Grammar g = new Grammar(
			"parser grammar P;\n"+
			"a : A;");
		String expecting =
			"RuleStart_a_0->s2\n" +
			"s2-A->s3\n" +
			"s3->RuleStop_a_1\n" +
			"RuleStop_a_1-EOF->s4\n";
		RuntimeTestUtils.checkRuleATN(g, "a", expecting);
	}
	[TestMethod] public void testAB(){
		Grammar g = new Grammar(
			"parser grammar P;\n"+
			"a : A B ;");
		String expecting =
			"RuleStart_a_0->s2\n" +
				"s2-A->s3\n" +
				"s3-B->s4\n" +
				"s4->RuleStop_a_1\n" +
				"RuleStop_a_1-EOF->s5\n";
		RuntimeTestUtils.checkRuleATN(g, "a", expecting);
	}
	[TestMethod] public void testAorB(){
		Grammar g = new Grammar(
			"parser grammar P;\n"+
			"a : A | B {;} ;");
		String expecting =
			"RuleStart_a_0->BlockStart_5\n" +
				"BlockStart_5->s2\n" +
				"BlockStart_5->s3\n" +
				"s2-A->BlockEnd_6\n" +
				"s3-B->s4\n" +
				"BlockEnd_6->RuleStop_a_1\n" +
				"s4-action_0:-1->BlockEnd_6\n" +
				"RuleStop_a_1-EOF->s7\n";
		RuntimeTestUtils.checkRuleATN(g, "a", expecting);
	}
	[TestMethod] public void testSetAorB(){
		Grammar g = new Grammar(
			"parser grammar P;\n"+
			"a : A | B ;");
		String expecting =
			"RuleStart_a_0->s2\n" +
				"s2-{A, B}->s3\n" +
				"s3->RuleStop_a_1\n" +
				"RuleStop_a_1-EOF->s4\n";
		RuntimeTestUtils.checkRuleATN(g, "a", expecting);
	}
	[TestMethod] public void testLexerIsntSetMultiCharString(){
		LexerGrammar g = new LexerGrammar(
			"lexer grammar P;\n"+
			"A : ('0x' | '0X') ;");
		String expecting =
			"s0->RuleStart_A_1\n" +
				"RuleStart_A_1->BlockStart_7\n" +
				"BlockStart_7->s3\n" +
				"BlockStart_7->s5\n" +
				"s3-'0'->s4\n" +
				"s5-'0'->s6\n" +
				"s4-'x'->BlockEnd_8\n" +
				"s6-'X'->BlockEnd_8\n" +
				"BlockEnd_8->RuleStop_A_2\n";
		checkTokensRule(g, null, expecting);
	}
	[TestMethod] public void testRange(){
		LexerGrammar g = new LexerGrammar(
			"lexer grammar P;\n"+
			"A : 'a'..'c' ;"
		);
		String expecting =
			"s0->RuleStart_A_1\n" +
				"RuleStart_A_1->s3\n" +
				"s3-'a'..'c'->s4\n" +
				"s4->RuleStop_A_2\n";
		checkTokensRule(g, null, expecting);
	}
	[TestMethod] public void testCharSet(){
		LexerGrammar g = new LexerGrammar(
			"lexer grammar P;\n"+
			"A : [abc] ;"
		);
		String expecting =
			"s0->RuleStart_A_1\n" +
				"RuleStart_A_1->s3\n" +
				"s3-{97..99}->s4\n" +
				"s4->RuleStop_A_2\n";
		checkTokensRule(g, null, expecting);
	}
	[TestMethod] public void testCharSetRange(){
		LexerGrammar g = new LexerGrammar(
			"lexer grammar P;\n"+
			"A : [a-c] ;"
		);
		String expecting =
			"s0->RuleStart_A_1\n" +
				"RuleStart_A_1->s3\n" +
				"s3-{97..99}->s4\n" +
				"s4->RuleStop_A_2\n";
		checkTokensRule(g, null, expecting);
	}
	[TestMethod] public void testCharSetUnicodeBMPEscape(){
		LexerGrammar g = new LexerGrammar(
			"lexer grammar P;\n"+
			"A : [\\uABCD] ;"
		);
		String expecting =
			"s0->RuleStart_A_1\n" +
				"RuleStart_A_1->s3\n" +
				"s3-43981->s4\n" +
				"s4->RuleStop_A_2\n";
		checkTokensRule(g, null, expecting);
	}
	[TestMethod] public void testCharSetUnicodeBMPEscapeRange(){
		LexerGrammar g = new LexerGrammar(
			"lexer grammar P;\n"+
			"A : [a-c\\uABCD-\\uABFF] ;"
		);
		String expecting =
			"s0->RuleStart_A_1\n" +
				"RuleStart_A_1->s3\n" +
				"s3-{97..99, 43981..44031}->s4\n" +
				"s4->RuleStop_A_2\n";
		checkTokensRule(g, null, expecting);
	}
	[TestMethod] public void testCharSetUnicodeSMPEscape(){
		LexerGrammar g = new LexerGrammar(
			"lexer grammar P;\n"+
			"A : [\\u{10ABCD}] ;"
		);
		String expecting =
			"s0->RuleStart_A_1\n" +
				"RuleStart_A_1->s3\n" +
				"s3-1092557->s4\n" +
				"s4->RuleStop_A_2\n";
		checkTokensRule(g, null, expecting);
	}
	[TestMethod] public void testCharSetUnicodeSMPEscapeRange(){
		LexerGrammar g = new LexerGrammar(
			"lexer grammar P;\n"+
			"A : [a-c\\u{10ABCD}-\\u{10ABFF}] ;"
		);
		String expecting =
			"s0->RuleStart_A_1\n" +
				"RuleStart_A_1->s3\n" +
				"s3-{97..99, 1092557..1092607}->s4\n" +
				"s4->RuleStop_A_2\n";
		checkTokensRule(g, null, expecting);
	}
	[TestMethod] public void testCharSetUnicodePropertyEscape(){
		// The Gothic script is long dead and unlikely to change (which would
		// cause this test to fail)
		LexerGrammar g = new LexerGrammar(
			"lexer grammar P;\n"+
			"A : [\\p{Gothic}] ;"
		);
		String expecting =
			"s0->RuleStart_A_1\n" +
				"RuleStart_A_1->s3\n" +
				"s3-{66352..66378}->s4\n" +
				"s4->RuleStop_A_2\n";
		checkTokensRule(g, null, expecting);
	}
	[TestMethod] public void testCharSetUnicodePropertyInvertEscape(){
		LexerGrammar g = new LexerGrammar(
			"lexer grammar P;\n"+
			"A : [\\P{Gothic}] ;"
		);
		String expecting =
			"s0->RuleStart_A_1\n" +
				"RuleStart_A_1->s3\n" +
				"s3-{0..66351, 66379..1114111}->s4\n" +
				"s4->RuleStop_A_2\n";
		checkTokensRule(g, null, expecting);
	}
	[TestMethod] public void testCharSetUnicodeMultiplePropertyEscape(){
		// Ditto the Mahajani script. Not going to change soon. I hope.
		LexerGrammar g = new LexerGrammar(
			"lexer grammar P;\n"+
			"A : [\\p{Gothic}\\p{Mahajani}] ;"
		);
		String expecting =
			"s0->RuleStart_A_1\n" +
				"RuleStart_A_1->s3\n" +
				"s3-{66352..66378, 69968..70006}->s4\n" +
				"s4->RuleStop_A_2\n";
		checkTokensRule(g, null, expecting);
	}
	[TestMethod] public void testCharSetUnicodePropertyOverlap(){
		LexerGrammar g = new LexerGrammar(
			"lexer grammar P;\n"+
			"A : [\\p{ASCII_Hex_Digit}\\p{Hex_Digit}] ;"
		);
		String expecting =
			"s0->RuleStart_A_1\n" +
				"RuleStart_A_1->s3\n" +
				"s3-{48..57, 65..70, 97..102, 65296..65305, 65313..65318, 65345..65350}->s4\n" +
				"s4->RuleStop_A_2\n";
		checkTokensRule(g, null, expecting);
	}
	[TestMethod] public void testRangeOrRange(){
		LexerGrammar g = new LexerGrammar(
			"lexer grammar P;\n"+
			"A : ('a'..'c' 'h' | 'q' 'j'..'l') ;"
		);
		String expecting =
			"s0->RuleStart_A_1\n" +
				"RuleStart_A_1->BlockStart_7\n" +
				"BlockStart_7->s3\n" +
				"BlockStart_7->s5\n" +
				"s3-'a'..'c'->s4\n" +
				"s5-'q'->s6\n" +
				"s4-'h'->BlockEnd_8\n" +
				"s6-'j'..'l'->BlockEnd_8\n" +
				"BlockEnd_8->RuleStop_A_2\n";
		checkTokensRule(g, null, expecting);
	}
	[TestMethod] public void testStringLiteralInParser(){
		Grammar g = new Grammar(
			"grammar P;\n"+
			"a : A|'b' ;"
		);
		String expecting =
			"RuleStart_a_0->s2\n" +
				"s2-{'b', A}->s3\n" +
				"s3->RuleStop_a_1\n" +
				"RuleStop_a_1-EOF->s4\n";
		RuntimeTestUtils.checkRuleATN(g, "a", expecting);
	}
	[TestMethod] public void testABorCD(){
		Grammar g = new Grammar(
			"parser grammar P;\n"+
			"a : A B | C D;");
		String expecting =
			"RuleStart_a_0->BlockStart_6\n" +
				"BlockStart_6->s2\n" +
				"BlockStart_6->s4\n" +
				"s2-A->s3\n" +
				"s4-C->s5\n" +
				"s3-B->BlockEnd_7\n" +
				"s5-D->BlockEnd_7\n" +
				"BlockEnd_7->RuleStop_a_1\n" +
				"RuleStop_a_1-EOF->s8\n";
		RuntimeTestUtils.checkRuleATN(g, "a", expecting);
	}
	[TestMethod] public void testbA(){
		Grammar g = new Grammar(
			"parser grammar P;\n"+
			"a : b A ;\n"+
			"b : B ;");
		String expecting =
			"RuleStart_a_0->s4\n" +
				"s4-b->RuleStart_b_2\n" +
				"s5-A->s6\n" +
				"s6->RuleStop_a_1\n" +
				"RuleStop_a_1-EOF->s9\n";
		RuntimeTestUtils.checkRuleATN(g, "a", expecting);
		expecting =
			"RuleStart_b_2->s7\n" +
				"s7-B->s8\n" +
				"s8->RuleStop_b_3\n" +
				"RuleStop_b_3->s5\n";
		RuntimeTestUtils.checkRuleATN(g, "b", expecting);
	}
	[TestMethod] public void testFollow(){
		Grammar g = new Grammar(
			"parser grammar P;\n"+
			"a : b A ;\n"+
			"b : B ;\n"+
			"c : b C;");
		String expecting =
			"RuleStart_b_2->s9\n" +
				"s9-B->s10\n" +
				"s10->RuleStop_b_3\n" +
				"RuleStop_b_3->s7\n" +
				"RuleStop_b_3->s12\n";
		RuntimeTestUtils.checkRuleATN(g, "b", expecting);
	}
	[TestMethod] public void testAorEpsilon(){
		Grammar g = new Grammar(
			"parser grammar P;\n"+
			"a : A | ;");
		String expecting =
			"RuleStart_a_0->BlockStart_4\n" +
				"BlockStart_4->s2\n" +
				"BlockStart_4->s3\n" +
				"s2-A->BlockEnd_5\n" +
				"s3->BlockEnd_5\n" +
				"BlockEnd_5->RuleStop_a_1\n" +
				"RuleStop_a_1-EOF->s6\n";
		RuntimeTestUtils.checkRuleATN(g, "a", expecting);
	}
	[TestMethod] public void testAOptional(){
		Grammar g = new Grammar(
			"parser grammar P;\n"+
			"a : A?;");
		String expecting =
			"RuleStart_a_0->BlockStart_3\n" +
				"BlockStart_3->s2\n" +
				"BlockStart_3->BlockEnd_4\n" +
				"s2-A->BlockEnd_4\n" +
				"BlockEnd_4->RuleStop_a_1\n" +
				"RuleStop_a_1-EOF->s5\n";
		RuntimeTestUtils.checkRuleATN(g, "a", expecting);
	}
	[TestMethod] public void testAorBoptional(){
		Grammar g = new Grammar(
			"parser grammar P;\n"+
			"a : (A{;}|B)?;");
		String expecting =
			"RuleStart_a_0->BlockStart_5\n" +
				"BlockStart_5->s2\n" +
				"BlockStart_5->s4\n" +
				"BlockStart_5->BlockEnd_6\n" +
				"s2-A->s3\n" +
				"s4-B->BlockEnd_6\n" +
				"BlockEnd_6->RuleStop_a_1\n" +
				"s3-action_0:-1->BlockEnd_6\n" +
				"RuleStop_a_1-EOF->s7\n";
		RuntimeTestUtils.checkRuleATN(g, "a", expecting);
	}
	[TestMethod] public void testSetAorBoptional(){
		Grammar g = new Grammar(
			"parser grammar P;\n"+
			"a : (A|B)?;");
		String expecting =
			"RuleStart_a_0->BlockStart_3\n" +
				"BlockStart_3->s2\n" +
				"BlockStart_3->BlockEnd_4\n" +
				"s2-{A, B}->BlockEnd_4\n" +
				"BlockEnd_4->RuleStop_a_1\n" +
				"RuleStop_a_1-EOF->s5\n";
		RuntimeTestUtils.checkRuleATN(g, "a", expecting);
	}
	[TestMethod] public void testAorBthenC(){
		Grammar g = new Grammar(
			"parser grammar P;\n"+
			"a : (A | B) C;");
		String expecting =
			"RuleStart_a_0->s2\n" +
				"s2-{A, B}->s3\n" +
				"s3-C->s4\n" +
				"s4->RuleStop_a_1\n" +
				"RuleStop_a_1-EOF->s5\n";
		RuntimeTestUtils.checkRuleATN(g, "a", expecting);
	}
	[TestMethod] public void testAplus(){
		Grammar g = new Grammar(
			"parser grammar P;\n"+
			"a : A+;");
		String expecting =
			"RuleStart_a_0->PlusBlockStart_3\n" +
				"PlusBlockStart_3->s2\n" +
				"s2-A->BlockEnd_4\n" +
				"BlockEnd_4->PlusLoopBack_5\n" +
				"PlusLoopBack_5->PlusBlockStart_3\n" +
				"PlusLoopBack_5->s6\n" +
				"s6->RuleStop_a_1\n" +
				"RuleStop_a_1-EOF->s7\n";
		RuntimeTestUtils.checkRuleATN(g, "a", expecting);
	}
	[TestMethod] public void testAplusSingleAltHasPlusASTPointingAtLoopBackState(){
		Grammar g = new Grammar(
			"parser grammar P;\n"+
			"s : a B ;\n" +			// (RULE a (BLOCK (ALT (+ (BLOCK (ALT A))))))
			"a : A+;");
		String expecting =
			"RuleStart_a_2->PlusBlockStart_8\n" +
			"PlusBlockStart_8->s7\n" +
			"s7-A->BlockEnd_9\n" +
			"BlockEnd_9->PlusLoopBack_10\n" +
			"PlusLoopBack_10->PlusBlockStart_8\n" +
			"PlusLoopBack_10->s11\n" +
			"s11->RuleStop_a_3\n" +
			"RuleStop_a_3->s5\n";
		RuntimeTestUtils.checkRuleATN(g, "a", expecting);
		// Get all AST -> ATNState relationships. Make sure loopback is covered when no loop entry decision
		List<GrammarAST> ruleNodes = g.ast.getNodesWithType(ANTLRParser.RULE);
		RuleAST a = (RuleAST)ruleNodes.get(1);
		List<GrammarAST> nodesInRule = a.getNodesWithType(null);
		Dictionary<GrammarAST, ATNState> covered = new ();
		for (GrammarAST node in nodesInRule) {
			if ( node.atnState != null ) {
				covered.put(node, node.atnState);
			}
		}
		Assert.AreEqual("{RULE=2, BLOCK=8, +=10, BLOCK=8, A=7}", covered.ToString());
	}
	[TestMethod] public void testAorBplus(){
		Grammar g = new Grammar(
			"parser grammar P;\n"+
			"a : (A|B{;})+;");
		String expecting =
			"RuleStart_a_0->PlusBlockStart_5\n" +
				"PlusBlockStart_5->s2\n" +
				"PlusBlockStart_5->s3\n" +
				"s2-A->BlockEnd_6\n" +
				"s3-B->s4\n" +
				"BlockEnd_6->PlusLoopBack_7\n" +
				"s4-action_0:-1->BlockEnd_6\n" +
				"PlusLoopBack_7->PlusBlockStart_5\n" +
				"PlusLoopBack_7->s8\n" +
				"s8->RuleStop_a_1\n" +
				"RuleStop_a_1-EOF->s9\n";
		RuntimeTestUtils.checkRuleATN(g, "a", expecting);
	}
	[TestMethod] public void testAorBorEmptyPlus(){
		Grammar g = new Grammar(
			"parser grammar P;\n"+
			"a : (A | B | )+ ;");
		String expecting =
			"RuleStart_a_0->PlusBlockStart_5\n" +
				"PlusBlockStart_5->s2\n" +
				"PlusBlockStart_5->s3\n" +
				"PlusBlockStart_5->s4\n" +
				"s2-A->BlockEnd_6\n" +
				"s3-B->BlockEnd_6\n" +
				"s4->BlockEnd_6\n" +
				"BlockEnd_6->PlusLoopBack_7\n" +
				"PlusLoopBack_7->PlusBlockStart_5\n" +
				"PlusLoopBack_7->s8\n" +
				"s8->RuleStop_a_1\n" +
				"RuleStop_a_1-EOF->s9\n";
		RuntimeTestUtils.checkRuleATN(g, "a", expecting);
	}
	[TestMethod] public void testEmptyOrEmpty(){
		Grammar g = new Grammar(
			"parser grammar P;\n"+
			"a : | ;");
		String expecting =
			"RuleStart_a_0->BlockStart_4\n"+
			"BlockStart_4->s2\n"+
			"BlockStart_4->s3\n"+
			"s2->BlockEnd_5\n"+
			"s3->BlockEnd_5\n"+
			"BlockEnd_5->RuleStop_a_1\n"+
			"RuleStop_a_1-EOF->s6\n";
		RuntimeTestUtils.checkRuleATN(g, "a", expecting);
	}
	[TestMethod] public void testAStar(){
		Grammar g = new Grammar(
			"parser grammar P;\n"+
			"a : A*;");
		String expecting =
			"RuleStart_a_0->StarLoopEntry_5\n" +
				"StarLoopEntry_5->StarBlockStart_3\n" +
				"StarLoopEntry_5->s6\n" +
				"StarBlockStart_3->s2\n" +
				"s6->RuleStop_a_1\n" +
				"s2-A->BlockEnd_4\n" +
				"RuleStop_a_1-EOF->s8\n" +
				"BlockEnd_4->StarLoopBack_7\n" +
				"StarLoopBack_7->StarLoopEntry_5\n";
		RuntimeTestUtils.checkRuleATN(g, "a", expecting);
	}
	[TestMethod] public void testNestedAstar(){
		Grammar g = new Grammar(
			"parser grammar P;\n"+
			"a : (COMMA ID*)*;");
		String expecting =
			"RuleStart_a_0->StarLoopEntry_11\n" +
				"StarLoopEntry_11->StarBlockStart_9\n" +
				"StarLoopEntry_11->s12\n" +
				"StarBlockStart_9->s2\n" +
				"s12->RuleStop_a_1\n" +
				"s2-COMMA->StarLoopEntry_6\n" +
				"RuleStop_a_1-EOF->s14\n" +
				"StarLoopEntry_6->StarBlockStart_4\n" +
				"StarLoopEntry_6->s7\n" +
				"StarBlockStart_4->s3\n" +
				"s7->BlockEnd_10\n" +
				"s3-ID->BlockEnd_5\n" +
				"BlockEnd_10->StarLoopBack_13\n" +
				"BlockEnd_5->StarLoopBack_8\n" +
				"StarLoopBack_13->StarLoopEntry_11\n" +
				"StarLoopBack_8->StarLoopEntry_6\n";
		RuntimeTestUtils.checkRuleATN(g, "a", expecting);
	}
	[TestMethod] public void testAorBstar(){
		Grammar g = new Grammar(
			"parser grammar P;\n"+
			"a : (A | B{;})* ;");
		String expecting =
			"RuleStart_a_0->StarLoopEntry_7\n" +
				"StarLoopEntry_7->StarBlockStart_5\n" +
				"StarLoopEntry_7->s8\n" +
				"StarBlockStart_5->s2\n" +
				"StarBlockStart_5->s3\n" +
				"s8->RuleStop_a_1\n" +
				"s2-A->BlockEnd_6\n" +
				"s3-B->s4\n" +
				"RuleStop_a_1-EOF->s10\n" +
				"BlockEnd_6->StarLoopBack_9\n" +
				"s4-action_0:-1->BlockEnd_6\n" +
				"StarLoopBack_9->StarLoopEntry_7\n";
		RuntimeTestUtils.checkRuleATN(g, "a", expecting);
	}
	[TestMethod] public void testPredicatedAorB(){
		Grammar g = new Grammar(
			"parser grammar P;\n"+
			"a : {p1}? A | {p2}? B ;");
		String expecting =
			"RuleStart_a_0->BlockStart_6\n" +
				"BlockStart_6->s2\n" +
				"BlockStart_6->s4\n" +
				"s2-pred_0:0->s3\n" +
				"s4-pred_0:1->s5\n" +
				"s3-A->BlockEnd_7\n" +
				"s5-B->BlockEnd_7\n" +
				"BlockEnd_7->RuleStop_a_1\n" +
				"RuleStop_a_1-EOF->s8\n";
		RuntimeTestUtils.checkRuleATN(g, "a", expecting);
	}

	[TestMethod] public void testParserRuleRefInLexerRule(){
		bool threwException = false;
		ErrorQueue errorQueue = new ErrorQueue();
		try {
			String gstr =
				"grammar U;\n"+
				"a : A;\n"+
				"A : a;\n";

			Tool tool = new Tool();
			tool.removeListeners();
			tool.addListener(errorQueue);
			Assert.AreEqual(0, errorQueue.size());
			GrammarRootAST grammarRootAST = tool.parseGrammarFromString(gstr);
			Assert.AreEqual(0, errorQueue.size());
			Grammar g = tool.createGrammar(grammarRootAST);
			Assert.AreEqual(0, errorQueue.size());
			g.fileName = "<string>";
			tool.process(g, false);
		}
		catch (Exception e) {
			threwException = true;
			e.printStackTrace();
		}
		Console.Out.WriteLine(errorQueue);
		Assert.AreEqual(1, errorQueue.errors.size());
		Assert.AreEqual(ErrorType.PARSER_RULE_REF_IN_LEXER_RULE, errorQueue.errors.get(0).getErrorType());
		Assert.AreEqual("[a, A]", Arrays.ToString(errorQueue.errors.get(0).getArgs()));
		Assert.IsTrue(!threwException);
	}

	/** Test for https://github.com/antlr/antlr4/issues/1369
	 *  Repeated edges:

	 RuleStop_e_3->BlockEnd_26
	 RuleStop_e_3->BlockEnd_26
	 RuleStop_e_3->BlockEnd_26

	 * @
	 */
	[TestMethod] public void testForRepeatedTransitionsToStopState(){
		String gstr =
			"grammar T;\n"+
			"\t s : e EOF;\n"+
			"\t e :<assoc=right> e '*' e\n"+
			"\t   |<assoc=right> e '+' e\n"+
			"\t   |<assoc=right> e '?' e ':' e\n"+
			"\t   |<assoc=right> e '=' e\n"+
			"\t   | ID\n"+
			"\t   ;\n"+
			"\t ID : 'a'..'z'+ ;\n"+
			"\t WS : (' '|'\\n') -> skip ;";
		Grammar g = new Grammar(gstr);
		String expecting =
			"RuleStart_e_2->s7\n"+
			"s7-action_1:-1->s8\n"+
			"s8-ID->s9\n"+
			"s9->StarLoopEntry_27\n"+
			"StarLoopEntry_27->StarBlockStart_25\n"+
			"StarLoopEntry_27->s28\n"+
			"StarBlockStart_25->s10\n"+
			"StarBlockStart_25->s13\n"+
			"StarBlockStart_25->s16\n"+
			"StarBlockStart_25->s22\n"+
			"s28->RuleStop_e_3\n"+
			"s10-5 >= _p->s11\n"+
			"s13-4 >= _p->s14\n"+
			"s16-3 >= _p->s17\n"+
			"s22-2 >= _p->s23\n"+
			"RuleStop_e_3->s5\n"+
			"RuleStop_e_3->BlockEnd_26\n"+
			"RuleStop_e_3->s19\n"+
			"RuleStop_e_3->s21\n"+
			"s11-'*'->s12\n"+
			"s14-'+'->s15\n"+
			"s17-'?'->s18\n"+
			"s23-'='->s24\n"+
			"s12-e->RuleStart_e_2\n"+
			"s15-e->RuleStart_e_2\n"+
			"s18-e->RuleStart_e_2\n"+
			"s24-e->RuleStart_e_2\n"+
			"BlockEnd_26->StarLoopBack_29\n"+
			"s19-':'->s20\n"+
			"StarLoopBack_29->StarLoopEntry_27\n"+
			"s20-e->RuleStart_e_2\n"+
			"s21->BlockEnd_26\n";
		RuntimeTestUtils.checkRuleATN(g, "e", expecting);
	}


/*
	[TestMethod] public void testMultiplePredicates(){
		Grammar g = new Grammar(
			"parser grammar P;\n"+
			"a : {p1}? {p1a}? A | {p2}? B | {p3} b;\n" +
			"b : {p4}? B ;");
		String expecting =
			"\n";
		checkRule(g, "a", expecting);
	}
	[TestMethod] public void testSets(){
		Grammar g = new Grammar(
			"parser grammar P;\n"+
			"a : ( A | B )+ ;\n" +
			"b : ( A | B{;} )+ ;\n" +
			"c : (A|B) (A|B) ;\n" +
			"d : ( A | B )* ;\n" +
			"e : ( A | B )? ;");
		String expecting =
			"\n";
		checkRule(g, "a", expecting);
		expecting =
			"\n";
		checkRule(g, "b", expecting);
		expecting =
			"\n";
		checkRule(g, "c", expecting);
		expecting =
			"\n";
		checkRule(g, "d", expecting);
		expecting =
			"\n";
		checkRule(g, "e", expecting);
	}
	[TestMethod] public void testNotSet(){
		Grammar g = new Grammar(
			"parser grammar P;\n"+
			"tokens { A; B; C; }\n"+
			"a : ~A ;\n");
		String expecting =
			"\n";
		checkRule(g, "a", expecting);
	}
	[TestMethod] public void testNotSingletonBlockSet(){
		Grammar g = new Grammar(
			"parser grammar P;\n"+
			"tokens { A; B; C; }\n"+
			"a : ~(A) ;\n");
		String expecting =
			"\n";
		checkRule(g, "a", expecting);
	}
	[TestMethod] public void testNotCharSet(){
		Grammar g = new Grammar(
			"lexer grammar P;\n"+
			"A : ~'3' ;\n");
		String expecting =
			"RuleStart_A_1->s5\n" +
			"s5-{'\\u0000'..'2', '4'..'\\uFFFE'}->s6\n" +
			"s6->RuleStop_A_2\n";
		checkRule(g, "A", expecting);
	}
	[TestMethod] public void testNotBlockSet(){
		Grammar g = new Grammar(
			"lexer grammar P;\n"+
			"A : ~('3'|'b') ;\n");
		String expecting =
			"\n";
		checkRule(g, "A", expecting);
	}
	[TestMethod] public void testNotSetLoop(){
		Grammar g = new Grammar(
			"lexer grammar P;\n"+
			"A : ~('3')* ;\n");
		String expecting =
			"\n";
		checkRule(g, "A", expecting);
	}
	[TestMethod] public void testNotBlockSetLoop(){
		Grammar g = new Grammar(
			"lexer grammar P;\n"+
			"A : ~('3'|'b')* ;\n");
		String expecting =
			"\n";
		checkRule(g, "A", expecting);
	}
	[TestMethod] public void testLabeledNotSet(){
		Grammar g = new Grammar(
			"parser grammar P;\n"+
			"tokens { A; B; C; }\n"+
			"a : t=~A ;\n");
		String expecting =
			".s0->.s1\n" +
			".s1->.s2\n" +
			".s2-B..C->.s3\n" +
			".s3->:s4\n" +
			":s4-EOF->.s5\n";
		checkRule(g, "a", expecting);
	}
	[TestMethod] public void testLabeledNotCharSet(){
		Grammar g = new Grammar(
			"lexer grammar P;\n"+
			"A : t=~'3' ;\n");
		String expecting =
			".s0->.s1\n" +
			".s1->.s2\n" +
			".s2-{'\\u0000'..'2', '4'..'\\uFFFF'}->.s3\n" +
			".s3->:s4\n" +
			":s4-<EOT>->.s5\n";
		checkRule(g, "A", expecting);
	}
	[TestMethod] public void testLabeledNotBlockSet(){
		Grammar g = new Grammar(
			"lexer grammar P;\n"+
			"A : t=~('3'|'b') ;\n");
		String expecting =
			".s0->.s1\n" +
			".s1->.s2\n" +
			".s2-{'\\u0000'..'2', '4'..'a', 'c'..'\\uFFFF'}->.s3\n" +
			".s3->:s4\n" +
			":s4-<EOT>->.s5\n";
		checkRule(g, "A", expecting);
	}
	[TestMethod] public void testEscapedCharLiteral(){
		Grammar g = new Grammar(
			"grammar P;\n"+
			"a : '\\n';");
		String expecting =
			".s0->.s1\n" +
			".s1->.s2\n" +
			".s2-'\\n'->.s3\n" +
			".s3->:s4\n" +
			":s4-EOF->.s5\n";
		checkRule(g, "a", expecting);
	}
	[TestMethod] public void testEscapedStringLiteral(){
		Grammar g = new Grammar(
			"grammar P;\n"+
			"a : 'a\\nb\\u0030c\\'';");
		String expecting =
			"RuleStart_a_0->s2\n" +
			"s2-'a\\nb\\u0030c\\''->s3\n" +
			"s3->RuleStop_a_1\n" +
			"RuleStop_a_1-EOF->s4\n";
		checkRule(g, "a", expecting);
	}
	*/

	[TestMethod] public void testDefaultMode(){
		LexerGrammar g = new LexerGrammar(
			"lexer grammar L;\n"+
			"A : 'a' ;\n" +
			"X : 'x' ;\n" +
			"mode FOO;\n" +
			"B : 'b' ;\n" +
			"C : 'c' ;\n");
		String expecting =
			"s0->RuleStart_A_2\n" +
				"s0->RuleStart_X_4\n" +
				"RuleStart_A_2->s10\n" +
				"RuleStart_X_4->s12\n" +
				"s10-'a'->s11\n" +
				"s12-'x'->s13\n" +
				"s11->RuleStop_A_3\n" +
				"s13->RuleStop_X_5\n";
		checkTokensRule(g, "DEFAULT_MODE", expecting);
	}

	[TestMethod] public void testMode(){
		LexerGrammar g = new LexerGrammar(
			"lexer grammar L;\n"+
			"A : 'a' ;\n" +
			"X : 'x' ;\n" +
			"mode FOO;\n" +
			"B : 'b' ;\n" +
			"C : 'c' ;\n");
		String expecting =
			"s1->RuleStart_B_6\n" +
				"s1->RuleStart_C_8\n" +
				"RuleStart_B_6->s14\n" +
				"RuleStart_C_8->s16\n" +
				"s14-'b'->s15\n" +
				"s16-'c'->s17\n" +
				"s15->RuleStop_B_7\n" +
				"s17->RuleStop_C_9\n";
		checkTokensRule(g, "FOO", expecting);
	}
	void checkTokensRule(LexerGrammar g, String modeName, String expecting) {
//		if ( g.ast!=null && !g.ast.hasErrors ) {
//			Console.Out.WriteLine(g.ast.toStringTree());
//			Tool antlr = new Tool();
//			SemanticPipeline sem = new SemanticPipeline(g);
//			sem.process();
//			if ( g.getImportedGrammars()!=null ) { // process imported grammars (if any)
//				for (Grammar imp : g.getImportedGrammars()) {
//					antlr.processNonCombinedGrammar(imp);
//				}
//			}
//		}
		if ( modeName==null ) modeName = "DEFAULT_MODE";
		if ( g.modes.get(modeName)==null ) {
			Console.Error.WriteLine("no such mode "+modeName);
			return;
		}
		ParserATNFactory f = new LexerATNFactory(g);
		ATN nfa = f.createATN();
		ATNState startState = nfa.modeNameToStartState.get(modeName);
		ATNPrinter serializer = new ATNPrinter(g, startState);
		String result = serializer.asString();
		//System.out.print(result);
		Assert.AreEqual(expecting, result);
	}
}
