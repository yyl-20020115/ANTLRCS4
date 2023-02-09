/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.automata;
using org.antlr.v4.runtime;
using org.antlr.v4.runtime.atn;
using org.antlr.v4.runtime.misc;
using org.antlr.v4.semantics;
using org.antlr.v4.test.runtime;
using org.antlr.v4.test.runtime.java;
using org.antlr.v4.test.runtime.states;
using org.antlr.v4.tool;
using System.Text;

namespace org.antlr.v4.test.tool;

public class ToolTestUtils
{
    public static ExecutedState ExecLexer(string grammarFileName, string grammarStr,
        string lexerName, string input)
    {
        return ExecLexer(grammarFileName, grammarStr, lexerName, input, null, false);
    }

    public static ExecutedState ExecLexer(string grammarFileName, string grammarStr, 
        string lexerName, string input, string tempDir, bool saveTestDir)
    {
        return ExecRecognizer(grammarFileName, grammarStr, null, lexerName,
                null, input, false, tempDir, saveTestDir);
    }

    public static ExecutedState ExecParser(string grammarFileName, string grammarStr,
                                       string parserName, string lexerName, string startRuleName,
                                       string input, bool showDiagnosticErrors
    )
    {
        return ExecParser(grammarFileName, grammarStr, parserName, lexerName, startRuleName,
                input, showDiagnosticErrors, null);
    }

    public static ExecutedState ExecParser(string grammarFileName, string grammarStr,
                                    string parserName, string lexerName, string startRuleName,
                                    string input, bool showDiagnosticErrors, string workingDir
    )
    {
        return ExecRecognizer(grammarFileName, grammarStr, parserName, lexerName,
                startRuleName, input, showDiagnosticErrors, workingDir, false);
    }

    private static ExecutedState ExecRecognizer(string grammarFileName, string grammarStr,
                                         string parserName, string lexerName, string startRuleName,
                                         string input, bool showDiagnosticErrors,
                                         string workingDir, bool saveTestDir)
    {
        var runOptions = CreateOptionsForJavaToolTests(grammarFileName, grammarStr, parserName, lexerName,
                false, true, startRuleName, input,
                false, showDiagnosticErrors, Stage.Execute, false);
        var runner = new JavaRunner(workingDir, saveTestDir);
        {
            var result = runner.Run(runOptions);
            if (result is not ExecutedState)
            {
                Assert.Fail(result.GetErrorMessage());
            }
            return (ExecutedState)result;
        }
    }

    public static RunOptions CreateOptionsForJavaToolTests(
            string grammarFileName, string grammarStr, string parserName, string lexerName,
            bool useListener, bool useVisitor, string startRuleName,
            string input, bool profile, bool showDiagnosticErrors,
            Stage endStage, bool returnObject
    )
    {
        return new RunOptions(grammarFileName, grammarStr, parserName, lexerName, useListener, useVisitor, startRuleName,
                input, profile, showDiagnosticErrors, false, endStage, returnObject, "Java",
                JavaRunner.runtimeTestParserName);
    }

    public static void TestErrors(string[] pairs, bool printTree)
    {
        for (int i = 0; i < pairs.Length; i += 2)
        {
            var grammarStr = pairs[i];
            var expect = pairs[i + 1];

            string[] lines = grammarStr.Split('\n');
            var fileName = GetFilenameFromFirstLineOfGrammar(lines[0]);

            var tempDirName = "AntlrTestErrors-" + Thread.CurrentThread.Name + "-"
                + DateTime.Now.Millisecond;
            var tempTestDir = Path.Combine(RuntimeTestUtils.TempDirectory, tempDirName);

            try
            {
                var equeue = Generator.AntlrOnString(tempTestDir, null, fileName, grammarStr, false);

                var actual = equeue.ToString(true);
                actual = actual.Replace(tempTestDir + Path.DirectorySeparatorChar, "");
                var msg = grammarStr;
                msg = msg.Replace("\n", "\\n");
                msg = msg.Replace("\r", "\\r");
                msg = msg.Replace("\t", "\\t");

                Assert.AreEqual(expect, actual, "error in: " + msg);
            }
            finally
            {
                try
                {
                    FileUtils.DeleteDirectory(tempTestDir);
                }
                catch (IOException ignored)
                {
                }
            }
        }
    }

    public static string GetFilenameFromFirstLineOfGrammar(string line)
    {
        var fileName = "A" + Tool.GRAMMAR_EXTENSION;
        int grIndex = line.LastIndexOf("grammar");
        int semi = line.LastIndexOf(';');
        if (grIndex >= 0 && semi >= 0)
        {
            int space = line.IndexOf(' ', grIndex);
            fileName = line[(space + 1)..semi] + Tool.GRAMMAR_EXTENSION;
        }
        if (fileName.Length == Tool.GRAMMAR_EXTENSION.Length) fileName = "A" + Tool.GRAMMAR_EXTENSION;
        return fileName;
    }

    public static List<string> RealElements(List<string> elements)
    {
        return elements.ToArray()[Token.MIN_USER_TOKEN_TYPE..elements.Count].ToList();

        //return elements.subList(Token.MIN_USER_TOKEN_TYPE, elements.Count);
    }

    public static string Load(string fileName)
    {
        if (fileName == null)
        {
            return null;
        }

        var fullFileName = Path.Combine(Environment.CurrentDirectory, nameof(ToolTestUtils), fileName);
        int size = 65000;
        {
            var data = File.ReadAllBytes(fullFileName);// isr.read(data);
            return Encoding.UTF8.GetString(data);
        }
    }

    public static ATN CreateATN(Grammar g, bool useSerializer)
    {
        if (g.atn == null)
        {
            SemanticProcess(g);
            Assert.AreEqual(0, g.Tools.getNumErrors());

            var f = g.IsLexer ? new LexerATNFactory((LexerGrammar)g) : new ParserATNFactory(g);

            g.atn = f.CreateATN();
            Assert.AreEqual(0, g.Tools.getNumErrors());
        }

        var atn = g.atn;
        if (useSerializer)
        {
            // sets some flags in ATN
            var serialized = ATNSerializer.GetSerialized(atn);
            return new ATNDeserializer().Deserialize(serialized.ToArray());
        }

        return atn;
    }

    public static void SemanticProcess(Grammar gammar)
    {
        if (gammar.ast != null && !gammar.ast.hasErrors)
        {
            //			Console.Out.WriteLine(g.ast.toStringTree());
            var antlr = new Tool();
            var sem = new SemanticPipeline(gammar);
            sem.Process();
            if (gammar.GetImportedGrammars() != null)
            { // process imported grammars (if any)
                foreach (var imp in gammar.GetImportedGrammars())
                {
                    antlr.ProcessNonCombinedGrammar(imp, false);
                }
            }
        }
    }

    public static IntegerList GetTokenTypesViaATN(string input, LexerATNSimulator lexerATN)
    {
        var @in = new ANTLRInputStream(input);
        var tokenTypes = new IntegerList();
        int ttype;
        do
        {
            ttype = lexerATN.Match(@in, Lexer.DEFAULT_MODE);
            tokenTypes.Add(ttype);
        } while (ttype != Token.EOF);
        return tokenTypes;
    }
}
