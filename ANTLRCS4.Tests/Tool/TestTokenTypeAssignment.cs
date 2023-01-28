/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime;
using org.antlr.v4.tool;

namespace org.antlr.v4.test.tool;

[TestClass]
public class TestTokenTypeAssignment {
	[TestMethod] public void testParserSimpleTokens(){
		Grammar g = new Grammar(
				"parser grammar t;\n"+
				"a : A | B;\n" +
				"b : C ;");
		String rules = "a, b";
		String tokenNames = "A, B, C";
		checkSymbols(g, rules, tokenNames);
	}

	[TestMethod] public void testParserTokensSection(){
		Grammar g = new Grammar(
				"parser grammar t;\n" +
				"tokens {\n" +
				"  C,\n" +
				"  D" +
				"}\n"+
				"a : A | B;\n" +
				"b : C ;");
		String rules = "a, b";
		String tokenNames = "A, B, C, D";
		checkSymbols(g, rules, tokenNames);
	}

	[TestMethod] public void testLexerTokensSection(){
		LexerGrammar g = new LexerGrammar(
				"lexer grammar t;\n" +
				"tokens {\n" +
				"  C,\n" +
				"  D" +
				"}\n"+
				"A : 'a';\n" +
				"C : 'c' ;");
		String rules = "A, C";
		String tokenNames = "A, C, D";
		checkSymbols(g, rules, tokenNames);
	}

	[TestMethod] public void testCombinedGrammarLiterals(){
		Grammar g = new Grammar(
				"grammar t;\n"+
				"a : 'begin' b 'end';\n" +
				"b : C ';' ;\n" +
				"ID : 'a' ;\n" +
				"FOO : 'foo' ;\n" +  // "foo" is not a token name
				"C : 'c' ;\n");        // nor is 'c'
		String rules = "a, b";
		String tokenNames = "C, FOO, ID, 'begin', 'end', ';'";
		checkSymbols(g, rules, tokenNames);
	}

	[TestMethod] public void testLiteralInParserAndLexer(){
		// 'x' is token and char in lexer rule
		Grammar g = new Grammar(
				"grammar t;\n" +
				"a : 'x' E ; \n" +
				"E: 'x' '0' ;\n");

		String literals = "['x']";
		String foundLiterals = g.stringLiteralToTypeMap.Keys.ToString();
		Assert.AreEqual(literals, foundLiterals);

		foundLiterals = g.implicitLexer.stringLiteralToTypeMap.Keys.ToString();
		Assert.AreEqual("['x']", foundLiterals); // pushed in lexer from parser

		String[] typeToTokenName = g.getTokenDisplayNames();
		Set<String> tokens = new LinkedHashSet<String>();
		for (String t : typeToTokenName) if ( t!=null ) tokens.add(t);
		Assert.AreEqual("[<INVALID>, 'x', E]", tokens.ToString());
	}

	[TestMethod] public void testPredDoesNotHideNameToLiteralMapInLexer(){
		// 'x' is token and char in lexer rule
		Grammar g = new Grammar(
				"grammar t;\n" +
				"a : 'x' X ; \n" +
				"X: 'x' {true}?;\n"); // must match as alias even with pred

		Assert.AreEqual("{'x'=1}", g.stringLiteralToTypeMap.ToString());
		Assert.AreEqual("{EOF=-1, X=1}", g.tokenNameToTypeMap.ToString());

		// pushed in lexer from parser
		Assert.AreEqual("{'x'=1}", g.implicitLexer.stringLiteralToTypeMap.ToString());
		Assert.AreEqual("{EOF=-1, X=1}", g.implicitLexer.tokenNameToTypeMap.ToString());
	}

	[TestMethod] public void testCombinedGrammarWithRefToLiteralButNoTokenIDRef(){
		Grammar g = new Grammar(
				"grammar t;\n"+
				"a : 'a' ;\n" +
				"A : 'a' ;\n");
		String rules = "a";
		String tokenNames = "A, 'a'";
		checkSymbols(g, rules, tokenNames);
	}

	[TestMethod] public void testSetDoesNotMissTokenAliases(){
		Grammar g = new Grammar(
				"grammar t;\n"+
				"a : 'a'|'b' ;\n" +
				"A : 'a' ;\n" +
				"B : 'b' ;\n");
		String rules = "a";
		String tokenNames = "A, 'a', B, 'b'";
		checkSymbols(g, rules, tokenNames);
	}

	// T E S T  L I T E R A L  E S C A P E S

	[TestMethod] public void testParserCharLiteralWithEscape(){
		Grammar g = new Grammar(
				"grammar t;\n"+
				"a : '\\n';\n");
		var literals = g.stringLiteralToTypeMap.Keys;
		// must store literals how they appear in the antlr grammar
		Assert.AreEqual("'\\n'", literals.toArray()[0]);
	}

	[TestMethod] public void testParserCharLiteralWithBasicUnicodeEscape(){
		Grammar g = new Grammar(
				"grammar t;\n"+
				"a : '\\uABCD';\n");
		var literals = g.stringLiteralToTypeMap.Keys;
		// must store literals how they appear in the antlr grammar
		Assert.AreEqual("'\\uABCD'", literals.toArray()[0]);
	}

	[TestMethod] public void testParserCharLiteralWithExtendedUnicodeEscape(){
		Grammar g = new Grammar(
				"grammar t;\n"+
				"a : '\\u{1ABCD}';\n");
		var literals = g.stringLiteralToTypeMap.Keys;
		// must store literals how they appear in the antlr grammar
		Assert.AreEqual("'\\u{1ABCD}'", literals.ToArray()[0]);
	}

	protected void checkSymbols(Grammar g,
								String rulesStr,
								String allValidTokensStr)
		
	{
		String[] typeToTokenName = g.getTokenNames();
        HashSet<String> tokens = new HashSet<String>();
		for (int i = 0; i < typeToTokenName.Length; i++) {
			String t = typeToTokenName[i];
			if ( t!=null ) {
				if (t.StartsWith(Grammar.AUTO_GENERATED_TOKEN_NAME_PREFIX)) {
					tokens.Add(g.getTokenDisplayName(i));
				}
				else {
					tokens.Add(t);
				}
			}
		}

		// make sure expected tokens are there
		StringTokenizer st = new StringTokenizer(allValidTokensStr, ", ");
		while ( st.hasMoreTokens() ) {
			String tokenName = st.nextToken();
			Assert.IsTrue(g.getTokenType(tokenName) != Token.INVALID_TYPE, "token "+tokenName+" expected, but was undefined");
			tokens.remove(tokenName);
		}
		// make sure there are not any others (other than <EOF> etc...)
		for (String tokenName : tokens) {
			Assert.IsTrue(g.getTokenType(tokenName) < Token.MIN_USER_TOKEN_TYPE, "unexpected token name "+tokenName);
		}

		// make sure all expected rules are there
		st = new StringTokenizer(rulesStr, ", ");
		int n = 0;
		while ( st.hasMoreTokens() ) {
			String ruleName = st.nextToken();
			assertNotNull(g.getRule(ruleName), "rule "+ruleName+" expected");
			n++;
		}
		//Console.Out.WriteLine("rules="+rules);
		// make sure there are no extra rules
		Assert.AreEqual(n, g.rules.size(), "number of rules mismatch; expecting "+n+"; found "+g.rules.size());
	}
}
