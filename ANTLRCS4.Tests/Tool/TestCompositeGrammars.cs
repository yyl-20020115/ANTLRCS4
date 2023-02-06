/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime.misc;
using org.antlr.v4.test.runtime;
using org.antlr.v4.test.runtime.java;
using org.antlr.v4.test.runtime.states;
using org.antlr.v4.tool;

namespace org.antlr.v4.test.tool;

[TestClass]
public class TestCompositeGrammars
{
    protected bool debug = false;

    [TestMethod]
    public void TestImportFileLocationInSubdir(string tempDir)
    {
        var tempDirPath = tempDir.ToString();
        var slave =
            "parser grammar S;\n" +
            "a : B {Console.Out.WriteLine(\"S.a\");} ;\n";
        FileUtils.MakeDirectory(tempDirPath);
        var subdir = Path.Combine(tempDirPath, "sub");
        FileUtils.MakeDirectory(subdir);
        FileUtils.WriteFile(subdir, "S.g4", slave);
        var master =
            "grammar M;\n" +
            "import S;\n" +
            "s : a ;\n" +
            "B : 'b' ;" + // defines B from inherited token space
            "WS : (' '|'\\n') -> skip ;\n";
        FileUtils.WriteFile(tempDirPath, "M.g4", master);
        var equeue = Generator.AntlrOnString(tempDirPath, "Java", "M.g4", false, "-lib", subdir);
        Assert.AreEqual(0, equeue.Count);
    }

    // Test for https://github.com/antlr/antlr4/issues/1317
    [TestMethod]
    public void TestImportSelfLoop(string tempDir)
    {
        var tempDirPath = tempDir.ToString();
        FileUtils.MakeDirectory(tempDirPath);
        var master =
            "grammar M;\n" +
            "import M;\n" +
            "s : 'a' ;\n";
        FileUtils.WriteFile(tempDirPath, "M.g4", master);
        var equeue = Generator.AntlrOnString(tempDirPath, "Java", "M.g4", false, "-lib", tempDirPath);
        Assert.AreEqual(0, equeue.Count);
    }

    [TestMethod]
    public void TestImportIntoLexerGrammar(string tempDir)
    {
        var tempDirPath = tempDir.ToString();
        FileUtils.MakeDirectory(tempDirPath);

        var master =
            "lexer grammar M;\n" +
            "import S;\n" +
            "A : 'a';\n" +
            "B : 'b';\n";
        FileUtils.WriteFile(tempDirPath, "M.g4", master);

        var slave =
            "lexer grammar S;\n" +
            "C : 'c';\n";
        FileUtils.WriteFile(tempDirPath, "S.g4", slave);

        var equeue = Generator.AntlrOnString(tempDirPath, "Java", "M.g4", false, "-lib", tempDirPath);
        Assert.AreEqual(0, equeue.errors.Count);
    }

    [TestMethod]
    public void TestImportModesIntoLexerGrammar(string tempDir)
    {
        var tempDirPath = tempDir.ToString();
        FileUtils.MakeDirectory(tempDirPath);

        var master =
            "lexer grammar M;\n" +
            "import S;\n" +
            "A : 'a' -> pushMode(X);\n" +
            "B : 'b';\n";
        FileUtils.WriteFile(tempDirPath, "M.g4", master);

        var slave =
            "lexer grammar S;\n" +
            "D : 'd';\n" +
            "mode X;\n" +
            "C : 'c' -> popMode;\n";
        FileUtils.WriteFile(tempDirPath, "S.g4", slave);

        var equeue = Generator.AntlrOnString(tempDirPath, "Java", "M.g4", false, "-lib", tempDirPath);
        Assert.AreEqual(0, equeue.errors.Count);
    }

    [TestMethod]
    public void TestImportChannelsIntoLexerGrammar(string tempDir)
    {
        var tempDirPath = tempDir.ToString();
        FileUtils.MakeDirectory(tempDirPath);

        var master =
            "lexer grammar M;\n" +
            "import S;\n" +
            "channels {CH_A, CH_B}\n" +
            "A : 'a' -> channel(CH_A);\n" +
            "B : 'b' -> channel(CH_B);\n";
        FileUtils.WriteFile(tempDirPath, "M.g4", master);

        var slave =
            "lexer grammar S;\n" +
            "C : 'c';\n";
        FileUtils.WriteFile(tempDirPath, "S.g4", slave);

        var equeue = Generator.AntlrOnString(tempDirPath, "Java", "M.g4", false, "-lib", tempDirPath);
        Assert.AreEqual(0, equeue.errors.Count);
    }

    [TestMethod]
    public void TestImportMixedChannelsIntoLexerGrammar(string tempDir)
    {
        var tempDirPath = tempDir.ToString();
        FileUtils.MakeDirectory(tempDirPath);

        var master =
            "lexer grammar M;\n" +
            "import S;\n" +
            "channels {CH_A, CH_B}\n" +
            "A : 'a' -> channel(CH_A);\n" +
            "B : 'b' -> channel(CH_B);\n";
        FileUtils.WriteFile(tempDirPath, "M.g4", master);

        var slave =
            "lexer grammar S;\n" +
            "channels {CH_C}\n" +
            "C : 'c' -> channel(CH_C);\n";
        FileUtils.WriteFile(tempDirPath, "S.g4", slave);

        var equeue = Generator.AntlrOnString(tempDirPath, "Java", "M.g4", false, "-lib", tempDirPath);
        Assert.AreEqual(0, equeue.errors.Count);
    }

    [TestMethod]
    public void TestImportClashingChannelsIntoLexerGrammar(string tempDir)
    {
        var tempDirPath = tempDir.ToString();
        FileUtils.MakeDirectory(tempDirPath);

        var master =
            "lexer grammar M;\n" +
            "import S;\n" +
            "channels {CH_A, CH_B, CH_C}\n" +
            "A : 'a' -> channel(CH_A);\n" +
            "B : 'b' -> channel(CH_B);\n" +
            "C : 'C' -> channel(CH_C);\n";
        FileUtils.WriteFile(tempDirPath, "M.g4", master);

        var slave =
            "lexer grammar S;\n" +
            "channels {CH_C}\n" +
            "C : 'c' -> channel(CH_C);\n";
        FileUtils.WriteFile(tempDirPath, "S.g4", slave);

        var equeue = Generator.AntlrOnString(tempDirPath, "Java", "M.g4", false, "-lib", tempDirPath);
        Assert.AreEqual(0, equeue.errors.Count);
    }

    [TestMethod]
    public void TestMergeModesIntoLexerGrammar(string tempDir)
    {
        var tempDirPath = tempDir.ToString();
        FileUtils.MakeDirectory(tempDirPath);

        var master =
            "lexer grammar M;\n" +
            "import S;\n" +
            "A : 'a' -> pushMode(X);\n" +
            "mode X;\n" +
            "B : 'b';\n";
        FileUtils.WriteFile(tempDirPath, "M.g4", master);

        var slave =
            "lexer grammar S;\n" +
            "D : 'd';\n" +
            "mode X;\n" +
            "C : 'c' -> popMode;\n";
        FileUtils.WriteFile(tempDirPath, "S.g4", slave);

        var equeue = Generator.AntlrOnString(tempDirPath, "Java", "M.g4", false, "-lib", tempDirPath);
        Assert.AreEqual(0, equeue.errors.Count);
    }

    [TestMethod]
    public void TestEmptyModesInLexerGrammar(string tempDir)
    {
        var tempDirPath = tempDir.ToString();
        FileUtils.MakeDirectory(tempDirPath);

        var master =
            "lexer grammar M;\n" +
            "import S;\n" +
            "A : 'a';\n" +
            "C : 'e';\n" +
            "B : 'b';\n";
        FileUtils.WriteFile(tempDirPath, "M.g4", master);

        var slave =
            "lexer grammar S;\n" +
            "D : 'd';\n" +
            "mode X;\n" +
            "C : 'c' -> popMode;\n";
        FileUtils.WriteFile(tempDirPath, "S.g4", slave);

        var equeue = Generator.AntlrOnString(tempDirPath, "Java", "M.g4", false, "-lib", tempDirPath);
        Assert.AreEqual(0, equeue.errors.Count);
    }

    [TestMethod]
    public void TestCombinedGrammarImportsModalLexerGrammar(string tempDir)
    {
        var tempDirPath = tempDir.ToString();
        FileUtils.MakeDirectory(tempDirPath);

        var master =
            "grammar M;\n" +
            "import S;\n" +
            "A : 'a';\n" +
            "B : 'b';\n" +
            "r : A B;\n";
        FileUtils.WriteFile(tempDirPath, "M.g4", master);

        var slave =
            "lexer grammar S;\n" +
            "D : 'd';\n" +
            "mode X;\n" +
            "C : 'c' -> popMode;\n";
        FileUtils.WriteFile(tempDirPath, "S.g4", slave);

        var equeue = Generator.AntlrOnString(tempDirPath, "Java", "M.g4", false, "-lib", tempDirPath);
        Assert.AreEqual(1, equeue.errors.Count);
        var msg = equeue.errors[0];
        Assert.AreEqual(ErrorType.MODE_NOT_IN_LEXER, msg.getErrorType());
        Assert.AreEqual("X", msg.getArgs()[0]);
        Assert.AreEqual(3, msg.line);
        Assert.AreEqual(5, msg.charPosition);
        Assert.AreEqual("M.g4", msg.fileName);
    }

    [TestMethod]
    public void TestDelegatesSeeSameTokenType(string tempDir)
    {
        var tempDirPath = tempDir.ToString();
        var slaveS =
            "parser grammar S;\n" +
            "tokens { A, B, C }\n" +
            "x : A ;\n";
        var slaveT =
            "parser grammar T;\n" +
            "tokens { C, B, A } // reverse order\n" +
            "y : A ;\n";

        FileUtils.MakeDirectory(tempDirPath);
        FileUtils.WriteFile(tempDirPath, "S.g4", slaveS);
        FileUtils.WriteFile(tempDirPath, "T.g4", slaveT);

        var master =
            "// The lexer will create rules to match letters a, b, c.\n" +
            "// The associated token types A, B, C must have the same value\n" +
            "// and all import'd parsers.  Since ANTLR regenerates all imports\n" +
            "// for use with the delegator M, it can generate the same token type\n" +
            "// mapping in each parser:\n" +
            "// public static final int C=6;\n" +
            "// public static final int EOF=-1;\n" +
            "// public static final int B=5;\n" +
            "// public static final int WS=7;\n" +
            "// public static final int A=4;\n" +
            "grammar M;\n" +
            "import S,T;\n" +
            "s : x y ; // matches AA, which should be 'aa'\n" +
            "B : 'b' ; // another order: B, A, C\n" +
            "A : 'a' ;\n" +
            "C : 'c' ;\n" +
            "WS : (' '|'\\n') -> skip ;\n";
        FileUtils.WriteFile(tempDirPath, "M.g4", master);
        var equeue = new ErrorQueue();
        var g = new Grammar(tempDirPath + "/M.g4", master, equeue);
        var expectedTokenIDToTypeMap = "{EOF=-1, B=1, A=2, C=3, WS=4}";
        var expectedStringLiteralToTypeMap = "{'a'=2, 'b'=1, 'c'=3}";
        var expectedTypeToTokenList = "[B, A, C, WS]";
        Assert.AreEqual(expectedTokenIDToTypeMap, g.tokenNameToTypeMap.ToString());
        Assert.AreEqual(expectedStringLiteralToTypeMap, Sort(g.stringLiteralToTypeMap).ToString());
        Assert.AreEqual(expectedTypeToTokenList, ToolTestUtils.RealElements(g.typeToTokenList).ToString());
        Assert.AreEqual(0, equeue.errors.Count, "unexpected errors: " + equeue);
    }

    [TestMethod]
    public void TestErrorInImportedGetsRightFilename(string tempDir)
    {
        var tempDirPath = tempDir.ToString();
        var slave =
            "parser grammar S;\n" +
            "a : 'a' | c;\n";
        FileUtils.MakeDirectory(tempDirPath);
        FileUtils.WriteFile(tempDirPath, "S.g4", slave);
        var master =
            "grammar M;\n" +
            "import S;\n";
        FileUtils.WriteFile(tempDirPath, "M.g4", master);
        var equeue = Generator.AntlrOnString(tempDirPath, "Java", "M.g4", false, "-lib", tempDirPath);
        var msg = equeue.errors[(0)];
        Assert.AreEqual(ErrorType.UNDEFINED_RULE_REF, msg.getErrorType());
        Assert.AreEqual("c", msg.getArgs()[0]);
        Assert.AreEqual(2, msg.line);
        Assert.AreEqual(10, msg.charPosition);
        Assert.AreEqual("S.g4", msg.fileName);
    }

    [TestMethod]
    public void TestImportFileNotSearchedForInOutputDir(string tempDir)
    {
        var tempDirPath = tempDir.ToString();
        var slave =
            "parser grammar S;\n" +
            "a : B {Console.Out.WriteLine(\"S.a\");} ;\n";
        FileUtils.MakeDirectory(tempDirPath);
        var outdir = tempDirPath + "/out";
        FileUtils.MakeDirectory(outdir);
        FileUtils.WriteFile(outdir, "S.g4", slave);
        var master =
            "grammar M;\n" +
            "import S;\n" +
            "s : a ;\n" +
            "B : 'b' ;" + // defines B from inherited token space
            "WS : (' '|'\\n') -> skip ;\n";
        FileUtils.WriteFile(tempDirPath, "M.g4", master);
        var equeue = Generator.AntlrOnString(tempDirPath, "Java", "M.g4", false, "-o", outdir);
        Assert.AreEqual(ErrorType.CANNOT_FIND_IMPORTED_GRAMMAR, equeue.errors[0].getErrorType());
    }

    [TestMethod]
    public void TestOutputDirShouldNotEffectImports(string tempDir)
    {
        var tempDirPath = tempDir.ToString();
        var slave =
            "parser grammar S;\n" +
            "a : B {Console.Out.WriteLine(\"S.a\");} ;\n";
        FileUtils.MakeDirectory(tempDirPath);
        var subdir = tempDirPath + "/sub";
        FileUtils.MakeDirectory(subdir);
        FileUtils.WriteFile(subdir, "S.g4", slave);
        var master =
            "grammar M;\n" +
            "import S;\n" +
            "s : a ;\n" +
            "B : 'b' ;" + // defines B from inherited token space
            "WS : (' '|'\\n') -> skip ;\n";
        FileUtils.WriteFile(tempDirPath, "M.g4", master);
        var outdir = tempDirPath + "/out";
        FileUtils.MakeDirectory(outdir);
        var equeue = Generator.AntlrOnString(tempDirPath, "Java", "M.g4", false, "-o", outdir, "-lib", subdir);
        Assert.AreEqual(0, equeue.Count);
    }

    [TestMethod]
    public void TestTokensFileInOutputDirAndImportFileInSubdir(string tempDir)
    {
        var tempDirPath = tempDir.ToString();
        var slave =
            "parser grammar S;\n" +
            "a : B {Console.Out.WriteLine(\"S.a\");} ;\n";
        FileUtils.MakeDirectory(tempDirPath);
        var subdir = tempDirPath + "/sub";
        FileUtils.MakeDirectory(subdir);
        FileUtils.WriteFile(subdir, "S.g4", slave);
        var parser =
            "parser grammar MParser;\n" +
            "import S;\n" +
            "options {tokenVocab=MLexer;}\n" +
            "s : a ;\n";
        FileUtils.WriteFile(tempDirPath, "MParser.g4", parser);
        var lexer =
            "lexer grammar MLexer;\n" +
            "B : 'b' ;" + // defines B from inherited token space
            "WS : (' '|'\\n') -> skip ;\n";
        FileUtils.WriteFile(tempDirPath, "MLexer.g4", lexer);
        var outdir = tempDirPath + "/out";
        FileUtils.MakeDirectory(outdir);
        var equeue = Generator.AntlrOnString(tempDirPath, "Java", "MLexer.g4", false, "-o", outdir);
        Assert.AreEqual(0, equeue.Count);
        equeue = Generator.AntlrOnString(tempDirPath, "Java", "MParser.g4", false, "-o", outdir, "-lib", subdir);
        Assert.AreEqual(0, equeue.Count);
    }

    [TestMethod]
    public void TestImportedTokenVocabIgnoredWithWarning(string tempDir)
    {
        var tempDirPath = tempDir.ToString();
        var equeue = new ErrorQueue();
        var slave =
            "parser grammar S;\n" +
            "options {tokenVocab=whatever;}\n" +
            "tokens { A }\n" +
            "x : A {Console.Out.WriteLine(\"S.x\");} ;\n";
        FileUtils.MakeDirectory(tempDirPath);
        FileUtils.WriteFile(tempDirPath, "S.g4", slave);

        var master =
            "grammar M;\n" +
            "import S;\n" +
            "s : x ;\n" +
            "WS : (' '|'\\n') -> skip ;\n";
        FileUtils.WriteFile(tempDirPath, "M.g4", master);
        var g = new Grammar(tempDirPath + "/M.g4", master, equeue);

        var expectedArg = "S";
        var expectedMsgID = ErrorType.OPTIONS_IN_DELEGATE;
        var expectedMessage =
            new GrammarSemanticsMessage(expectedMsgID, g.fileName, null, expectedArg);
        CheckGrammarSemanticsWarning(equeue, expectedMessage);

        Assert.AreEqual(0, equeue.errors.Count, "unexpected errors: " + equeue);
        Assert.AreEqual(1, equeue.warnings.Count, "unexpected warnings: " + equeue);
    }

    [TestMethod]
    public void TestSyntaxErrorsInImportsNotThrownOut(string tempDir)
    {
        var tempDirPath = tempDir.ToString();
        var equeue = new ErrorQueue();
        var slave =
            "parser grammar S;\n" +
            "options {toke\n";
        FileUtils.MakeDirectory(tempDirPath);
        FileUtils.WriteFile(tempDirPath, "S.g4", slave);

        var master =
            "grammar M;\n" +
            "import S;\n" +
            "s : x ;\n" +
            "WS : (' '|'\\n') -> skip ;\n";
        FileUtils.WriteFile(tempDirPath, "M.g4", master);
        /*Grammar g =*/
        var gx = new Grammar(tempDirPath + "/M.g4", master, equeue);

        Assert.AreEqual(ErrorType.SYNTAX_ERROR, equeue.errors[0].getErrorType());
    }

    // Make sure that M can import S that imports T.
    [TestMethod]
    public void Test3LevelImport(string tempDir)
    {
        var tempDirPath = tempDir.ToString();
        var equeue = new ErrorQueue();
        var slave =
            "parser grammar T;\n" +
            "a : T ;\n";
        FileUtils.MakeDirectory(tempDirPath);
        FileUtils.WriteFile(tempDirPath, "T.g4", slave);
        var slave2 =
            "parser grammar S;\n" +
            "import T;\n" +
            "a : S ;\n";
        FileUtils.MakeDirectory(tempDirPath);
        FileUtils.WriteFile(tempDirPath, "S.g4", slave2);

        var master =
            "grammar M;\n" +
            "import S;\n" +
            "a : M ;\n";
        FileUtils.WriteFile(tempDirPath, "M.g4", master);
        var g = new Grammar(tempDirPath + "/M.g4", master, equeue);

        var expectedTokenIDToTypeMap = "{EOF=-1, M=1}"; // S and T aren't imported; overridden
        var expectedStringLiteralToTypeMap = "{}";
        var expectedTypeToTokenList = "[M]";

        Assert.AreEqual(expectedTokenIDToTypeMap,
                     g.tokenNameToTypeMap.ToString());
        Assert.AreEqual(expectedStringLiteralToTypeMap, g.stringLiteralToTypeMap.ToString());
        Assert.AreEqual(expectedTypeToTokenList,
                     ToolTestUtils.RealElements(g.typeToTokenList).ToString());

        Assert.AreEqual(0, equeue.errors.Count, "unexpected errors: " + equeue);
        Assert.IsTrue(Compile("M.g4", master, "MParser", "a", tempDir));
    }

    [TestMethod]
    public void TestBigTreeOfImports(string tempDir)
    {
        var tempDirPath = tempDir.ToString();
        var equeue = new ErrorQueue();
        var slave =
            "parser grammar T;\n" +
            "tokens{T}\n" +
            "x : T ;\n";
        FileUtils.MakeDirectory(tempDirPath);
        FileUtils.WriteFile(tempDirPath, "T.g4", slave);
        slave =
            "parser grammar S;\n" +
            "import T;\n" +
            "tokens{S}\n" +
            "y : S ;\n";
        FileUtils.MakeDirectory(tempDirPath);
        FileUtils.WriteFile(tempDirPath, "S.g4", slave);

        slave =
            "parser grammar C;\n" +
            "tokens{C}\n" +
            "i : C ;\n";
        FileUtils.MakeDirectory(tempDirPath);
        FileUtils.WriteFile(tempDirPath, "C.g4", slave);
        slave =
            "parser grammar B;\n" +
            "tokens{B}\n" +
            "j : B ;\n";
        FileUtils.MakeDirectory(tempDirPath);
        FileUtils.WriteFile(tempDirPath, "B.g4", slave);
        slave =
            "parser grammar A;\n" +
            "import B,C;\n" +
            "tokens{A}\n" +
            "k : A ;\n";
        FileUtils.MakeDirectory(tempDirPath);
        FileUtils.WriteFile(tempDirPath, "A.g4", slave);

        var master =
            "grammar M;\n" +
            "import S,A;\n" +
            "tokens{M}\n" +
            "a : M ;\n";
        FileUtils.WriteFile(tempDirPath, "M.g4", master);
        var g = new Grammar(tempDirPath + "/M.g4", master, equeue);

        Assert.AreEqual("[]", equeue.errors.ToString());
        Assert.AreEqual("[]", equeue.warnings.ToString());
        var expectedTokenIDToTypeMap = "{EOF=-1, M=1, S=2, T=3, A=4, B=5, C=6}";
        var expectedStringLiteralToTypeMap = "{}";
        var expectedTypeToTokenList = "[M, S, T, A, B, C]";

        Assert.AreEqual(expectedTokenIDToTypeMap,
                     g.tokenNameToTypeMap.ToString());
        Assert.AreEqual(expectedStringLiteralToTypeMap, g.stringLiteralToTypeMap.ToString());
        Assert.AreEqual(expectedTypeToTokenList,
                     ToolTestUtils.RealElements(g.typeToTokenList).ToString());
        Assert.IsTrue(Compile("M.g4", master, "MParser", "a", tempDir));
    }

    [TestMethod]
    public void TestRulesVisibleThroughMultilevelImport(string tempDir)
    {
        var tempDirPath = tempDir.ToString();
        var equeue = new ErrorQueue();
        var slave =
            "parser grammar T;\n" +
            "x : T ;\n";
        FileUtils.MakeDirectory(tempDirPath);
        FileUtils.WriteFile(tempDirPath, "T.g4", slave);
        var slave2 =
            "parser grammar S;\n" + // A, B, C token type order
            "import T;\n" +
            "a : S ;\n";
        FileUtils.MakeDirectory(tempDirPath);
        FileUtils.WriteFile(tempDirPath, "S.g4", slave2);

        var master =
            "grammar M;\n" +
            "import S;\n" +
            "a : M x ;\n"; // x MUST BE VISIBLE TO M
        FileUtils.WriteFile(tempDirPath, "M.g4", master);
        var g = new Grammar(tempDirPath + "/M.g4", master, equeue);

        var expectedTokenIDToTypeMap = "{EOF=-1, M=1, T=2}";
        var expectedStringLiteralToTypeMap = "{}";
        var expectedTypeToTokenList = "[M, T]";

        Assert.AreEqual(expectedTokenIDToTypeMap,
                     g.tokenNameToTypeMap.ToString());
        Assert.AreEqual(expectedStringLiteralToTypeMap, g.stringLiteralToTypeMap.ToString());
        Assert.AreEqual(expectedTypeToTokenList,
                     ToolTestUtils.RealElements(g.typeToTokenList).ToString());

        Assert.AreEqual(0, equeue.errors.Count, "unexpected errors: " + equeue);
    }

    [TestMethod]
    public void TestNestedComposite(string tempDir)
    {
        var tempDirPath = tempDir.ToString();
        // Wasn't compiling. http://www.antlr.org/jira/browse/ANTLR-438
        var equeue = new ErrorQueue();
        var gstr =
            "lexer grammar L;\n" +
            "T1: '1';\n" +
            "T2: '2';\n" +
            "T3: '3';\n" +
            "T4: '4';\n";
        FileUtils.MakeDirectory(tempDirPath);
        FileUtils.WriteFile(tempDirPath, "L.g4", gstr);
        gstr =
            "parser grammar G1;\n" +
            "s: a | b;\n" +
            "a: T1;\n" +
            "b: T2;\n";
        FileUtils.MakeDirectory(tempDirPath);
        FileUtils.WriteFile(tempDirPath, "G1.g4", gstr);

        gstr =
            "parser grammar G2;\n" +
            "import G1;\n" +
            "a: T3;\n";
        FileUtils.MakeDirectory(tempDirPath);
        FileUtils.WriteFile(tempDirPath, "G2.g4", gstr);
        var G3str =
            "grammar G3;\n" +
            "import G2;\n" +
            "b: T4;\n";
        FileUtils.MakeDirectory(tempDirPath);
        FileUtils.WriteFile(tempDirPath, "G3.g4", G3str);

        var g = new Grammar(tempDirPath + "/G3.g4", G3str, equeue);

        var expectedTokenIDToTypeMap = "{EOF=-1, T4=1, T3=2}";
        var expectedStringLiteralToTypeMap = "{}";
        var expectedTypeToTokenList = "[T4, T3]";

        Assert.AreEqual(expectedTokenIDToTypeMap,
                     g.tokenNameToTypeMap.ToString());
        Assert.AreEqual(expectedStringLiteralToTypeMap, g.stringLiteralToTypeMap.ToString());
        Assert.AreEqual(expectedTypeToTokenList,
                     ToolTestUtils.RealElements(g.typeToTokenList).ToString());

        Assert.AreEqual(0, equeue.errors.Count, "unexpected errors: " + equeue);

        Assert.IsTrue(Compile("G3.g4", G3str, "G3Parser", "b", tempDir));
    }

    [TestMethod]
    public void TestHeadersPropogatedCorrectlyToImportedGrammars(string tempDir)
    {
        var tempDirPath = tempDir.ToString();
        var slave =
            "parser grammar S;\n" +
            "a : B {System.out.print(\"S.a\");} ;\n";
        FileUtils.MakeDirectory(tempDirPath);
        FileUtils.WriteFile(tempDirPath, "S.g4", slave);
        var master =
            "grammar M;\n" +
            "import S;\n" +
            "@header{package mypackage;}\n" +
            "s : a ;\n" +
            "B : 'b' ;" + // defines B from inherited token space
            "WS : (' '|'\\n') -> skip ;\n";
        var equeue = Generator.AntlrOnString(tempDirPath, "Java", "M.g4", master, false);
        int expecting = 0; // should be ok
        Assert.AreEqual(expecting, equeue.errors.Count);
    }

    /**
	 * This is a regression test for antlr/antlr4#670 "exception when importing
	 * grammar".  I think this one always worked but I found that a different
	 * Java grammar caused an error and so I made the testImportLeftRecursiveGrammar() test below.
	 * https://github.com/antlr/antlr4/issues/670
	 */
    // TODO: migrate to test framework
    [TestMethod]
    public void TestImportLargeGrammar(string tempDir)
    {
        var tempDirPath = tempDir.ToString();
        var slave = ToolTestUtils.Load("Java.g4");
        var master =
            "grammar NewJava;\n" +
            "import Java;\n";

        FileUtils.MakeDirectory(tempDirPath);
        FileUtils.WriteFile(tempDirPath, "Java.g4", slave);
        ExecutedState executedState = ToolTestUtils.ExecParser("NewJava.g4", master,
                "NewJavaParser", "NewJavaLexer", "compilationUnit", "package Foo;",
                debug, tempDir);
        Assert.AreEqual("", executedState.output);
        Assert.AreEqual("", executedState.errors);
    }

    /**
	 * This is a regression test for antlr/antlr4#670 "exception when importing
	 * grammar".
	 * https://github.com/antlr/antlr4/issues/670
	 */
    // TODO: migrate to test framework
    [TestMethod]
    public void TestImportLeftRecursiveGrammar(string tempDir)
    {
        var tempDirPath = tempDir.ToString();
        var slave =
            "grammar Java;\n" +
            "e : '(' e ')'\n" +
            "  | e '=' e\n" +
            "  | ID\n" +
            "  ;\n" +
            "ID : [a-z]+ ;\n";
        var master =
            "grammar T;\n" +
            "import Java;\n" +
            "s : e ;\n";

        FileUtils.MakeDirectory(tempDirPath);
        FileUtils.WriteFile(tempDirPath, "Java.g4", slave);
        var executedState = ToolTestUtils.ExecParser(
                "T.g4", master, "TParser", "TLexer", "s", "a=b", debug,
                tempDir);
        Assert.AreEqual("", executedState.output);
        Assert.AreEqual("", executedState.errors);
    }

    // ISSUE: https://github.com/antlr/antlr4/issues/2296
    [TestMethod]
    public void TestCircularGrammarInclusion(string tempDir)
    {
        var tempDirPath = tempDir.ToString();
        var g1 =
                "grammar G1;\n" +
                "import  G2;\n" +
                "r : 'R1';";

        var g2 =
                "grammar G2;\n" +
                "import  G1;\n" +
                "r : 'R2';";

        FileUtils.MakeDirectory(tempDirPath);
        FileUtils.WriteFile(tempDirPath, "G1.g4", g1);
        var executedState = ToolTestUtils.ExecParser("G2.g4", g2, "G2Parser", "G2Lexer", "r", "R2", debug, tempDir);
        Assert.AreEqual("", executedState.errors);
    }

    private static void CheckGrammarSemanticsWarning(ErrorQueue equeue, GrammarSemanticsMessage expectedMessage)
    {
        ANTLRMessage foundMsg = null;
        for (int i = 0; i < equeue.warnings.Count; i++)
        {
            var m = equeue.warnings[(i)];
            if (m.getErrorType() == expectedMessage.getErrorType())
            {
                foundMsg = m;
            }
        }
        Assert.IsNotNull(foundMsg, "no error; " + expectedMessage.getErrorType() + " expected");
        Assert.IsTrue(foundMsg is GrammarSemanticsMessage, "error is not a GrammarSemanticsMessage");
        Assert.AreEqual(Arrays.ToString(expectedMessage.getArgs()), Arrays.ToString(foundMsg.getArgs()));
        if (equeue.Count != 1)
        {
            Console.Error.WriteLine(equeue);
        }
    }

    private static bool Compile(string grammarFileName, string grammarStr, string parserName, string startRuleName, string tempDirPath)
    {
        var runOptions = ToolTestUtils.CreateOptionsForJavaToolTests(grammarFileName, grammarStr, parserName, null,
                false, false, startRuleName, null,
                false, false, Stage.Compile, false);
        var runner = new JavaRunner(tempDirPath, false);
        {
            JavaCompiledState compiledState = (JavaCompiledState)runner.Run(runOptions);
            return !compiledState.ContainsErrors();
        }
    }

    public static SortedDictionary<K, V> Sort<K, V>(IDictionary<K, V> data) where K : notnull
    {
        return new SortedDictionary<K,V>(data);
    }
}
