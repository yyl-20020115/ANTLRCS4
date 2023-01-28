/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.tool;

namespace org.antlr.v4.test.tool;

[TestClass]
public class TestCompositeGrammars {
	protected bool debug = false;

	[TestMethod] public void testImportFileLocationInSubdir(string tempDir) {
		String tempDirPath = tempDir.ToString();
		String slave =
			"parser grammar S;\n" +
			"a : B {Console.Out.WriteLine(\"S.a\");} ;\n";
		FileUtils.mkdir(tempDirPath);
		String subdir = tempDirPath + PathSeparator + "sub";
		FileUtils.mkdir(subdir);
		writeFile(subdir, "S.g4", slave);
		String master =
			"grammar M;\n" +
			"import S;\n" +
			"s : a ;\n" +
			"B : 'b' ;" + // defines B from inherited token space
			"WS : (' '|'\\n') -> skip ;\n" ;
		writeFile(tempDirPath, "M.g4", master);
		ErrorQueue equeue = Generator.antlrOnString(tempDirPath, "Java", "M.g4", false, "-lib", subdir);
		assertEquals(0, equeue.size());
	}

	// Test for https://github.com/antlr/antlr4/issues/1317
	[TestMethod] public void testImportSelfLoop( Path tempDir) {
		String tempDirPath = tempDir.ToString();
		FileUtils.mkdir(tempDirPath);
		String master =
			"grammar M;\n" +
			"import M;\n" +
			"s : 'a' ;\n";
		writeFile(tempDirPath, "M.g4", master);
		ErrorQueue equeue = Generator.antlrOnString(tempDirPath, "Java", "M.g4", false, "-lib", tempDirPath);
		assertEquals(0, equeue.size());
	}

	[TestMethod] public void testImportIntoLexerGrammar( Path tempDir) {
		String tempDirPath = tempDir.ToString();
		FileUtils.mkdir(tempDirPath);

		String master =
			"lexer grammar M;\n" +
			"import S;\n" +
			"A : 'a';\n" +
			"B : 'b';\n";
		writeFile(tempDirPath, "M.g4", master);

		String slave =
		    "lexer grammar S;\n" +
			"C : 'c';\n";
		writeFile(tempDirPath, "S.g4", slave);

		ErrorQueue equeue = Generator.antlrOnString(tempDirPath, "Java", "M.g4", false, "-lib", tempDirPath);
		assertEquals(0, equeue.errors.size());
	}

	[TestMethod] public void testImportModesIntoLexerGrammar( Path tempDir) {
		String tempDirPath = tempDir.ToString();
		FileUtils.mkdir(tempDirPath);

		String master =
			"lexer grammar M;\n" +
			"import S;\n" +
			"A : 'a' -> pushMode(X);\n" +
			"B : 'b';\n";
		writeFile(tempDirPath, "M.g4", master);

		String slave =
			"lexer grammar S;\n" +
			"D : 'd';\n" +
			"mode X;\n" +
			"C : 'c' -> popMode;\n";
		writeFile(tempDirPath, "S.g4", slave);

		ErrorQueue equeue = Generator.antlrOnString(tempDirPath, "Java", "M.g4", false, "-lib", tempDirPath);
		assertEquals(0, equeue.errors.size());
	}

	[TestMethod] public void testImportChannelsIntoLexerGrammar( Path tempDir) {
		String tempDirPath = tempDir.ToString();
		FileUtils.mkdir(tempDirPath);

		String master =
			"lexer grammar M;\n" +
			"import S;\n" +
			"channels {CH_A, CH_B}\n" +
			"A : 'a' -> channel(CH_A);\n" +
			"B : 'b' -> channel(CH_B);\n";
		writeFile(tempDirPath, "M.g4", master);

		String slave =
			"lexer grammar S;\n" +
			"C : 'c';\n";
		writeFile(tempDirPath, "S.g4", slave);

		ErrorQueue equeue = Generator.antlrOnString(tempDirPath, "Java", "M.g4", false, "-lib", tempDirPath);
		assertEquals(0, equeue.errors.size());
	}

	[TestMethod] public void testImportMixedChannelsIntoLexerGrammar( Path tempDir) {
		String tempDirPath = tempDir.ToString();
		FileUtils.mkdir(tempDirPath);

		String master =
			"lexer grammar M;\n" +
			"import S;\n" +
			"channels {CH_A, CH_B}\n" +
			"A : 'a' -> channel(CH_A);\n" +
			"B : 'b' -> channel(CH_B);\n";
		writeFile(tempDirPath, "M.g4", master);

		String slave =
			"lexer grammar S;\n" +
			"channels {CH_C}\n" +
			"C : 'c' -> channel(CH_C);\n";
		writeFile(tempDirPath, "S.g4", slave);

		ErrorQueue equeue = Generator.antlrOnString(tempDirPath, "Java", "M.g4", false, "-lib", tempDirPath);
		assertEquals(0, equeue.errors.size());
	}

	[TestMethod] public void testImportClashingChannelsIntoLexerGrammar( Path tempDir) {
		String tempDirPath = tempDir.ToString();
		FileUtils.mkdir(tempDirPath);

		String master =
			"lexer grammar M;\n" +
			"import S;\n" +
			"channels {CH_A, CH_B, CH_C}\n" +
			"A : 'a' -> channel(CH_A);\n" +
			"B : 'b' -> channel(CH_B);\n" +
			"C : 'C' -> channel(CH_C);\n";
		writeFile(tempDirPath, "M.g4", master);

		String slave =
			"lexer grammar S;\n" +
			"channels {CH_C}\n" +
			"C : 'c' -> channel(CH_C);\n";
		writeFile(tempDirPath, "S.g4", slave);

		ErrorQueue equeue = Generator.antlrOnString(tempDirPath, "Java", "M.g4", false, "-lib", tempDirPath);
		assertEquals(0, equeue.errors.size());
	}

	[TestMethod] public void testMergeModesIntoLexerGrammar( Path tempDir) {
		String tempDirPath = tempDir.ToString();
		FileUtils.mkdir(tempDirPath);

		String master =
			"lexer grammar M;\n" +
			"import S;\n" +
			"A : 'a' -> pushMode(X);\n" +
			"mode X;\n" +
			"B : 'b';\n";
		writeFile(tempDirPath, "M.g4", master);

		String slave =
			"lexer grammar S;\n" +
			"D : 'd';\n" +
			"mode X;\n" +
			"C : 'c' -> popMode;\n";
		writeFile(tempDirPath, "S.g4", slave);

		ErrorQueue equeue = Generator.antlrOnString(tempDirPath, "Java", "M.g4", false, "-lib", tempDirPath);
		assertEquals(0, equeue.errors.size());
	}

	[TestMethod] public void testEmptyModesInLexerGrammar( Path tempDir) {
		String tempDirPath = tempDir.ToString();
		FileUtils.mkdir(tempDirPath);

		String master =
			"lexer grammar M;\n" +
			"import S;\n" +
			"A : 'a';\n" +
			"C : 'e';\n" +
			"B : 'b';\n";
		writeFile(tempDirPath, "M.g4", master);

		String slave =
			"lexer grammar S;\n" +
			"D : 'd';\n" +
			"mode X;\n" +
			"C : 'c' -> popMode;\n";
		writeFile(tempDirPath, "S.g4", slave);

		ErrorQueue equeue = Generator.antlrOnString(tempDirPath, "Java", "M.g4", false, "-lib", tempDirPath);
		assertEquals(0, equeue.errors.size());
	}

	[TestMethod] public void testCombinedGrammarImportsModalLexerGrammar( Path tempDir) {
		String tempDirPath = tempDir.ToString();
		FileUtils.mkdir(tempDirPath);

		String master =
			"grammar M;\n" +
			"import S;\n" +
			"A : 'a';\n" +
			"B : 'b';\n" +
			"r : A B;\n";
		writeFile(tempDirPath, "M.g4", master);

		String slave =
			"lexer grammar S;\n" +
			"D : 'd';\n" +
			"mode X;\n" +
			"C : 'c' -> popMode;\n";
		writeFile(tempDirPath, "S.g4", slave);

		ErrorQueue equeue = Generator.antlrOnString(tempDirPath, "Java", "M.g4", false, "-lib", tempDirPath);
		assertEquals(1, equeue.errors.size());
		ANTLRMessage msg = equeue.errors.get(0);
		assertEquals(ErrorType.MODE_NOT_IN_LEXER, msg.getErrorType());
		assertEquals("X", msg.getArgs()[0]);
		assertEquals(3, msg.line);
		assertEquals(5, msg.charPosition);
		assertEquals("M.g4", new File(msg.fileName).getName());
	}

	[TestMethod] public void testDelegatesSeeSameTokenType( Path tempDir)  {
		String tempDirPath = tempDir.ToString();
		String slaveS =
			"parser grammar S;\n"+
			"tokens { A, B, C }\n"+
			"x : A ;\n";
		String slaveT =
			"parser grammar T;\n"+
			"tokens { C, B, A } // reverse order\n"+
			"y : A ;\n";

		FileUtils.mkdir(tempDirPath);
		writeFile(tempDirPath, "S.g4", slaveS);
		writeFile(tempDirPath, "T.g4", slaveT);

		String master =
			"// The lexer will create rules to match letters a, b, c.\n"+
			"// The associated token types A, B, C must have the same value\n"+
			"// and all import'd parsers.  Since ANTLR regenerates all imports\n"+
			"// for use with the delegator M, it can generate the same token type\n"+
			"// mapping in each parser:\n"+
			"// public static final int C=6;\n"+
			"// public static final int EOF=-1;\n"+
			"// public static final int B=5;\n"+
			"// public static final int WS=7;\n"+
			"// public static final int A=4;\n"+
			"grammar M;\n"+
			"import S,T;\n"+
			"s : x y ; // matches AA, which should be 'aa'\n"+
			"B : 'b' ; // another order: B, A, C\n"+
			"A : 'a' ;\n"+
			"C : 'c' ;\n"+
			"WS : (' '|'\\n') -> skip ;\n";
		writeFile(tempDirPath, "M.g4", master);
		ErrorQueue equeue = new ErrorQueue();
		Grammar g = new Grammar(tempDirPath+"/M.g4", master, equeue);
		String expectedTokenIDToTypeMap = "{EOF=-1, B=1, A=2, C=3, WS=4}";
		String expectedStringLiteralToTypeMap = "{'a'=2, 'b'=1, 'c'=3}";
		String expectedTypeToTokenList = "[B, A, C, WS]";
		assertEquals(expectedTokenIDToTypeMap, g.tokenNameToTypeMap.ToString());
		assertEquals(expectedStringLiteralToTypeMap, sort(g.stringLiteralToTypeMap).ToString());
		assertEquals(expectedTypeToTokenList, realElements(g.typeToTokenList).ToString());
		assertEquals(0, equeue.errors.size(), "unexpected errors: "+equeue);
	}

	[TestMethod] public void testErrorInImportedGetsRightFilename( Path tempDir) {
		String tempDirPath = tempDir.ToString();
		String slave =
			"parser grammar S;\n" +
			"a : 'a' | c;\n";
		FileUtils.mkdir(tempDirPath);
		writeFile(tempDirPath, "S.g4", slave);
		String master =
			"grammar M;\n" +
			"import S;\n";
		writeFile(tempDirPath, "M.g4", master);
		ErrorQueue equeue = Generator.antlrOnString(tempDirPath, "Java", "M.g4", false, "-lib", tempDirPath);
		ANTLRMessage msg = equeue.errors.get(0);
		assertEquals(ErrorType.UNDEFINED_RULE_REF, msg.getErrorType());
		assertEquals("c", msg.getArgs()[0]);
		assertEquals(2, msg.line);
		assertEquals(10, msg.charPosition);
		assertEquals("S.g4", new File(msg.fileName).getName());
	}

	[TestMethod] public void testImportFileNotSearchedForInOutputDir( Path tempDir) {
		String tempDirPath = tempDir.ToString();
		String slave =
			"parser grammar S;\n" +
			"a : B {Console.Out.WriteLine(\"S.a\");} ;\n";
		FileUtils.mkdir(tempDirPath);
		String outdir = tempDirPath + "/out";
		FileUtils.mkdir(outdir);
		writeFile(outdir, "S.g4", slave);
		String master =
			"grammar M;\n" +
			"import S;\n" +
			"s : a ;\n" +
			"B : 'b' ;" + // defines B from inherited token space
			"WS : (' '|'\\n') -> skip ;\n" ;
		writeFile(tempDirPath, "M.g4", master);
		ErrorQueue equeue = Generator.antlrOnString(tempDirPath, "Java", "M.g4", false, "-o", outdir);
		assertEquals(ErrorType.CANNOT_FIND_IMPORTED_GRAMMAR, equeue.errors.get(0).getErrorType());
	}

	[TestMethod] public void testOutputDirShouldNotEffectImports( Path tempDir) {
		String tempDirPath = tempDir.ToString();
		String slave =
			"parser grammar S;\n" +
			"a : B {Console.Out.WriteLine(\"S.a\");} ;\n";
		FileUtils.mkdir(tempDirPath);
		String subdir = tempDirPath + "/sub";
		FileUtils.mkdir(subdir);
		writeFile(subdir, "S.g4", slave);
		String master =
			"grammar M;\n" +
			"import S;\n" +
			"s : a ;\n" +
			"B : 'b' ;" + // defines B from inherited token space
			"WS : (' '|'\\n') -> skip ;\n" ;
		writeFile(tempDirPath, "M.g4", master);
		String outdir = tempDirPath + "/out";
		FileUtils.mkdir(outdir);
		ErrorQueue equeue = Generator.antlrOnString(tempDirPath, "Java", "M.g4", false, "-o", outdir, "-lib", subdir);
		assertEquals(0, equeue.size());
	}

	[TestMethod] public void testTokensFileInOutputDirAndImportFileInSubdir( Path tempDir) {
		String tempDirPath = tempDir.ToString();
		String slave =
			"parser grammar S;\n" +
			"a : B {Console.Out.WriteLine(\"S.a\");} ;\n";
		FileUtils.mkdir(tempDirPath);
		String subdir = tempDirPath + "/sub";
		FileUtils.mkdir(subdir);
		writeFile(subdir, "S.g4", slave);
		String parser =
			"parser grammar MParser;\n" +
			"import S;\n" +
			"options {tokenVocab=MLexer;}\n" +
			"s : a ;\n";
		writeFile(tempDirPath, "MParser.g4", parser);
		String lexer =
			"lexer grammar MLexer;\n" +
			"B : 'b' ;" + // defines B from inherited token space
			"WS : (' '|'\\n') -> skip ;\n" ;
		writeFile(tempDirPath, "MLexer.g4", lexer);
		String outdir = tempDirPath + "/out";
		FileUtils.mkdir(outdir);
		ErrorQueue equeue = Generator.antlrOnString(tempDirPath, "Java", "MLexer.g4", false, "-o", outdir);
		assertEquals(0, equeue.size());
		equeue = Generator.antlrOnString(tempDirPath, "Java", "MParser.g4", false, "-o", outdir, "-lib", subdir);
		assertEquals(0, equeue.size());
	}

	[TestMethod] public void testImportedTokenVocabIgnoredWithWarning( Path tempDir)  {
		String tempDirPath = tempDir.ToString();
		ErrorQueue equeue = new ErrorQueue();
		String slave =
			"parser grammar S;\n" +
			"options {tokenVocab=whatever;}\n" +
			"tokens { A }\n" +
			"x : A {Console.Out.WriteLine(\"S.x\");} ;\n";
		FileUtils.mkdir(tempDirPath);
		writeFile(tempDirPath, "S.g4", slave);

		String master =
			"grammar M;\n" +
			"import S;\n" +
			"s : x ;\n" +
			"WS : (' '|'\\n') -> skip ;\n" ;
		writeFile(tempDirPath, "M.g4", master);
		Grammar g = new Grammar(tempDirPath+"/M.g4", master, equeue);

		Object expectedArg = "S";
		ErrorType expectedMsgID = ErrorType.OPTIONS_IN_DELEGATE;
		GrammarSemanticsMessage expectedMessage =
			new GrammarSemanticsMessage(expectedMsgID, g.fileName, null, expectedArg);
		checkGrammarSemanticsWarning(equeue, expectedMessage);

		assertEquals(0, equeue.errors.size(), "unexpected errors: "+equeue);
		assertEquals(1, equeue.warnings.size(), "unexpected warnings: "+equeue);
	}

	[TestMethod] public void testSyntaxErrorsInImportsNotThrownOut( Path tempDir)  {
		String tempDirPath = tempDir.ToString();
		ErrorQueue equeue = new ErrorQueue();
		String slave =
			"parser grammar S;\n" +
			"options {toke\n";
		FileUtils.mkdir(tempDirPath);
		writeFile(tempDirPath, "S.g4", slave);

		String master =
			"grammar M;\n" +
			"import S;\n" +
			"s : x ;\n" +
			"WS : (' '|'\\n') -> skip ;\n" ;
		writeFile(tempDirPath, "M.g4", master);
		/*Grammar g =*/ new Grammar(tempDirPath+"/M.g4", master, equeue);

		assertEquals(ErrorType.SYNTAX_ERROR, equeue.errors.get(0).getErrorType());
	}

	// Make sure that M can import S that imports T.
	[TestMethod] public void test3LevelImport( Path tempDir)  {
		String tempDirPath = tempDir.ToString();
		ErrorQueue equeue = new ErrorQueue();
		String slave =
			"parser grammar T;\n" +
			"a : T ;\n" ;
		FileUtils.mkdir(tempDirPath);
		writeFile(tempDirPath, "T.g4", slave);
		String slave2 =
			"parser grammar S;\n" +
			"import T;\n" +
			"a : S ;\n" ;
		FileUtils.mkdir(tempDirPath);
		writeFile(tempDirPath, "S.g4", slave2);

		String master =
			"grammar M;\n" +
			"import S;\n" +
			"a : M ;\n" ;
		writeFile(tempDirPath, "M.g4", master);
		Grammar g = new Grammar(tempDirPath+"/M.g4", master, equeue);

		String expectedTokenIDToTypeMap = "{EOF=-1, M=1}"; // S and T aren't imported; overridden
		String expectedStringLiteralToTypeMap = "{}";
		String expectedTypeToTokenList = "[M]";

		assertEquals(expectedTokenIDToTypeMap,
					 g.tokenNameToTypeMap.ToString());
		assertEquals(expectedStringLiteralToTypeMap, g.stringLiteralToTypeMap.ToString());
		assertEquals(expectedTypeToTokenList,
					 realElements(g.typeToTokenList).ToString());

		assertEquals(0, equeue.errors.size(), "unexpected errors: "+equeue);
		assertTrue(compile("M.g4", master, "MParser", "a", tempDir));
	}

	[TestMethod] public void testBigTreeOfImports( Path tempDir)  {
		String tempDirPath = tempDir.ToString();
		ErrorQueue equeue = new ErrorQueue();
		String slave =
			"parser grammar T;\n" +
			"tokens{T}\n" +
			"x : T ;\n" ;
		FileUtils.mkdir(tempDirPath);
		writeFile(tempDirPath, "T.g4", slave);
		slave =
			"parser grammar S;\n" +
			"import T;\n" +
			"tokens{S}\n" +
			"y : S ;\n" ;
		FileUtils.mkdir(tempDirPath);
		writeFile(tempDirPath, "S.g4", slave);

		slave =
			"parser grammar C;\n" +
			"tokens{C}\n" +
			"i : C ;\n" ;
		FileUtils.mkdir(tempDirPath);
		writeFile(tempDirPath, "C.g4", slave);
		slave =
			"parser grammar B;\n" +
			"tokens{B}\n" +
			"j : B ;\n" ;
		FileUtils.mkdir(tempDirPath);
		writeFile(tempDirPath, "B.g4", slave);
		slave =
			"parser grammar A;\n" +
			"import B,C;\n" +
			"tokens{A}\n" +
			"k : A ;\n" ;
		FileUtils.mkdir(tempDirPath);
		writeFile(tempDirPath, "A.g4", slave);

		String master =
			"grammar M;\n" +
			"import S,A;\n" +
			"tokens{M}\n" +
			"a : M ;\n" ;
		writeFile(tempDirPath, "M.g4", master);
		Grammar g = new Grammar(tempDirPath+"/M.g4", master, equeue);

		assertEquals("[]", equeue.errors.ToString());
		assertEquals("[]", equeue.warnings.ToString());
		String expectedTokenIDToTypeMap = "{EOF=-1, M=1, S=2, T=3, A=4, B=5, C=6}";
		String expectedStringLiteralToTypeMap = "{}";
		String expectedTypeToTokenList = "[M, S, T, A, B, C]";

		assertEquals(expectedTokenIDToTypeMap,
					 g.tokenNameToTypeMap.ToString());
		assertEquals(expectedStringLiteralToTypeMap, g.stringLiteralToTypeMap.ToString());
		assertEquals(expectedTypeToTokenList,
					 realElements(g.typeToTokenList).ToString());
		assertTrue(compile("M.g4", master, "MParser", "a", tempDir));
	}

	[TestMethod] public void testRulesVisibleThroughMultilevelImport( Path tempDir)  {
		String tempDirPath = tempDir.ToString();
		ErrorQueue equeue = new ErrorQueue();
		String slave =
			"parser grammar T;\n" +
			"x : T ;\n" ;
		FileUtils.mkdir(tempDirPath);
		writeFile(tempDirPath, "T.g4", slave);
		String slave2 =
			"parser grammar S;\n" + // A, B, C token type order
			"import T;\n" +
			"a : S ;\n" ;
		FileUtils.mkdir(tempDirPath);
		writeFile(tempDirPath, "S.g4", slave2);

		String master =
			"grammar M;\n" +
			"import S;\n" +
			"a : M x ;\n" ; // x MUST BE VISIBLE TO M
		writeFile(tempDirPath, "M.g4", master);
		Grammar g = new Grammar(tempDirPath+"/M.g4", master, equeue);

		String expectedTokenIDToTypeMap = "{EOF=-1, M=1, T=2}";
		String expectedStringLiteralToTypeMap = "{}";
		String expectedTypeToTokenList = "[M, T]";

		assertEquals(expectedTokenIDToTypeMap,
					 g.tokenNameToTypeMap.ToString());
		assertEquals(expectedStringLiteralToTypeMap, g.stringLiteralToTypeMap.ToString());
		assertEquals(expectedTypeToTokenList,
					 realElements(g.typeToTokenList).ToString());

		assertEquals(0, equeue.errors.size(), "unexpected errors: "+equeue);
	}

	[TestMethod] public void testNestedComposite( Path tempDir)  {
		String tempDirPath = tempDir.ToString();
		// Wasn't compiling. http://www.antlr.org/jira/browse/ANTLR-438
		ErrorQueue equeue = new ErrorQueue();
		String gstr =
			"lexer grammar L;\n" +
			"T1: '1';\n" +
			"T2: '2';\n" +
			"T3: '3';\n" +
			"T4: '4';\n" ;
		FileUtils.mkdir(tempDirPath);
		writeFile(tempDirPath, "L.g4", gstr);
		gstr =
			"parser grammar G1;\n" +
			"s: a | b;\n" +
			"a: T1;\n" +
			"b: T2;\n" ;
		FileUtils.mkdir(tempDirPath);
		writeFile(tempDirPath, "G1.g4", gstr);

		gstr =
			"parser grammar G2;\n" +
			"import G1;\n" +
			"a: T3;\n" ;
		FileUtils.mkdir(tempDirPath);
		writeFile(tempDirPath, "G2.g4", gstr);
		String G3str =
			"grammar G3;\n" +
			"import G2;\n" +
			"b: T4;\n" ;
		FileUtils.mkdir(tempDirPath);
		writeFile(tempDirPath, "G3.g4", G3str);

		Grammar g = new Grammar(tempDirPath+"/G3.g4", G3str, equeue);

		String expectedTokenIDToTypeMap = "{EOF=-1, T4=1, T3=2}";
		String expectedStringLiteralToTypeMap = "{}";
		String expectedTypeToTokenList = "[T4, T3]";

		assertEquals(expectedTokenIDToTypeMap,
					 g.tokenNameToTypeMap.ToString());
		assertEquals(expectedStringLiteralToTypeMap, g.stringLiteralToTypeMap.ToString());
		assertEquals(expectedTypeToTokenList,
					 realElements(g.typeToTokenList).ToString());

		assertEquals(0, equeue.errors.size(), "unexpected errors: "+equeue);

		assertTrue(compile("G3.g4", G3str, "G3Parser", "b", tempDir));
	}

	[TestMethod] public void testHeadersPropogatedCorrectlyToImportedGrammars( Path tempDir) {
		String tempDirPath = tempDir.ToString();
		String slave =
			"parser grammar S;\n" +
			"a : B {System.out.print(\"S.a\");} ;\n";
		FileUtils.mkdir(tempDirPath);
		writeFile(tempDirPath, "S.g4", slave);
		String master =
			"grammar M;\n" +
			"import S;\n" +
			"@header{package mypackage;}\n" +
			"s : a ;\n" +
			"B : 'b' ;" + // defines B from inherited token space
			"WS : (' '|'\\n') -> skip ;\n" ;
		ErrorQueue equeue = Generator.antlrOnString(tempDirPath, "Java", "M.g4", master, false);
		int expecting = 0; // should be ok
		assertEquals(expecting, equeue.errors.size());
	}

	/**
	 * This is a regression test for antlr/antlr4#670 "exception when importing
	 * grammar".  I think this one always worked but I found that a different
	 * Java grammar caused an error and so I made the testImportLeftRecursiveGrammar() test below.
	 * https://github.com/antlr/antlr4/issues/670
	 */
	// TODO: migrate to test framework
	[TestMethod]
	public void testImportLargeGrammar( Path tempDir){
		String tempDirPath = tempDir.ToString();
		String slave = load("Java.g4");
		String master =
			"grammar NewJava;\n" +
			"import Java;\n";

		FileUtils.mkdir(tempDirPath);
		writeFile(tempDirPath, "Java.g4", slave);
		ExecutedState executedState = execParser("NewJava.g4", master,
				"NewJavaParser", "NewJavaLexer", "compilationUnit", "package Foo;",
				debug, tempDir);
		assertEquals("", executedState.output);
		assertEquals("", executedState.errors);
	}

	/**
	 * This is a regression test for antlr/antlr4#670 "exception when importing
	 * grammar".
	 * https://github.com/antlr/antlr4/issues/670
	 */
	// TODO: migrate to test framework
	[TestMethod]
	public void testImportLeftRecursiveGrammar( Path tempDir) {
		String tempDirPath = tempDir.ToString();
		String slave =
			"grammar Java;\n" +
			"e : '(' e ')'\n" +
			"  | e '=' e\n" +
			"  | ID\n" +
			"  ;\n" +
			"ID : [a-z]+ ;\n";
		String master =
			"grammar T;\n" +
			"import Java;\n" +
			"s : e ;\n";

		FileUtils.mkdir(tempDirPath);
		writeFile(tempDirPath, "Java.g4", slave);
		ExecutedState executedState = execParser(
				"T.g4", master, "TParser", "TLexer", "s", "a=b", debug,
				tempDir);
		assertEquals("", executedState.output);
		assertEquals("", executedState.errors);
	}

	// ISSUE: https://github.com/antlr/antlr4/issues/2296
	[TestMethod]
	public void testCircularGrammarInclusion( Path tempDir) {
		String tempDirPath = tempDir.ToString();
		String g1 =
				"grammar G1;\n" +
				"import  G2;\n" +
				"r : 'R1';";

		String g2 =
				"grammar G2;\n" +
				"import  G1;\n" +
				"r : 'R2';";

		FileUtils.mkdir(tempDirPath);
		writeFile(tempDirPath, "G1.g4", g1);
		ExecutedState executedState = execParser("G2.g4", g2, "G2Parser", "G2Lexer", "r", "R2", debug, tempDir);
		assertEquals("", executedState.errors);
	}

	private static void checkGrammarSemanticsWarning(ErrorQueue equeue, GrammarSemanticsMessage expectedMessage) {
		ANTLRMessage foundMsg = null;
		for (int i = 0; i < equeue.warnings.size(); i++) {
			ANTLRMessage m = equeue.warnings.get(i);
			if (m.getErrorType()==expectedMessage.getErrorType() ) {
				foundMsg = m;
			}
		}
		assertNotNull(foundMsg, "no error; "+expectedMessage.getErrorType()+" expected");
		assertTrue(foundMsg is GrammarSemanticsMessage, "error is not a GrammarSemanticsMessage");
		assertEquals(Arrays.ToString(expectedMessage.getArgs()), Arrays.ToString(foundMsg.getArgs()));
		if ( equeue.size()!=1 ) {
			Console.Error.WriteLine(equeue);
		}
	}

	private static bool compile(String grammarFileName, String grammarStr, String parserName, String startRuleName,
							Path tempDirPath
	) {
		RunOptions runOptions = createOptionsForJavaToolTests(grammarFileName, grammarStr, parserName, null,
				false, false, startRuleName, null,
				false, false, Stage.Compile, false);
		try (JavaRunner runner = new JavaRunner(tempDirPath, false)) {
			JavaCompiledState compiledState = (JavaCompiledState) runner.run(runOptions);
			return !compiledState.containsErrors();
		}
	}

	public static <K : Comparable<? super K>,V> LinkedHashMap<K,V> sort(Map<K,V> data) {
		LinkedHashMap<K,V> dup = new LinkedHashMap<K, V>();
		List<K> keys = new ArrayList<K>(data.keySet());
		Collections.sort(keys);
		for (K k : keys) {
			dup.put(k, data.get(k));
		}
		return dup;
	}
}
