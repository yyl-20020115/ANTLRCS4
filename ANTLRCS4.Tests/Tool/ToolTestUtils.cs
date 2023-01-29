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
using org.antlr.v4.tool;

namespace org.antlr.v4.test.tool;


public class ToolTestUtils {
	public static ExecutedState execLexer(String grammarFileName, String grammarStr, String lexerName, String input) {
		return execLexer(grammarFileName, grammarStr, lexerName, input, null, false);
	}

	public static ExecutedState execLexer(String grammarFileName, String grammarStr, String lexerName, String input,
									  string tempDir, bool saveTestDir) {
		return execRecognizer(grammarFileName, grammarStr, null, lexerName,
				null, input, false, tempDir, saveTestDir);
	}

	public static ExecutedState execParser(String grammarFileName, String grammarStr,
									   String parserName, String lexerName, String startRuleName,
									   String input, bool showDiagnosticErrors
	) {
		return execParser(grammarFileName, grammarStr, parserName, lexerName, startRuleName,
				input, showDiagnosticErrors, null);
	}

	public static ExecutedState execParser(String grammarFileName, String grammarStr,
									String parserName, String lexerName, String startRuleName,
									String input, bool showDiagnosticErrors, string workingDir
	) {
		return execRecognizer(grammarFileName, grammarStr, parserName, lexerName,
				startRuleName, input, showDiagnosticErrors, workingDir, false);
	}

	private static ExecutedState execRecognizer(String grammarFileName, String grammarStr,
										 String parserName, String lexerName, String startRuleName,
										 String input, bool showDiagnosticErrors,
										 string workingDir, bool saveTestDir) {
		RunOptions runOptions = createOptionsForJavaToolTests(grammarFileName, grammarStr, parserName, lexerName,
				false, true, startRuleName, input,
				false, showDiagnosticErrors, Stage.Execute, false);
		using (JavaRunner runner = new JavaRunner(workingDir, saveTestDir)) {
			State result = runner.run(runOptions);
			if (!(result is ExecutedState)) {
				Assert.Fail(result.getErrorMessage());
			}
			return  (ExecutedState) result;
		}
	}

	public static RunOptions createOptionsForJavaToolTests(
			String grammarFileName, String grammarStr, String parserName, String lexerName,
			bool useListener, bool useVisitor, String startRuleName,
			String input, bool profile, bool showDiagnosticErrors,
			Stage endStage, bool returnObject
	) {
		return new RunOptions(grammarFileName, grammarStr, parserName, lexerName, useListener, useVisitor, startRuleName,
				input, profile, showDiagnosticErrors, false, endStage, returnObject, "Java",
				JavaRunner.runtimeTestParserName);
	}

	public static void testErrors(String[] pairs, bool printTree) {
		for (int i = 0; i < pairs.Length; i += 2) {
			String grammarStr = pairs[i];
			String expect = pairs[i + 1];

			String[] lines = grammarStr.Split('\n');
			String fileName = getFilenameFromFirstLineOfGrammar(lines[0]);

			String tempDirName = "AntlrTestErrors-" + Thread.currentThread().getName() + "-" + System.currentTimeMillis();
			String tempTestDir = Paths.get(TempDirectory, tempDirName).ToString();

			try {
				ErrorQueue equeue = antlrOnString(tempTestDir, null, fileName, grammarStr, false);

				String actual = equeue.ToString(true);
				actual = actual.Replace(tempTestDir + Path.DirectorySeparatorChar, "");
				String msg = grammarStr;
				msg = msg.Replace("\n", "\\n");
				msg = msg.Replace("\r", "\\r");
				msg = msg.Replace("\t", "\\t");

				Assert.AreEqual(expect, actual, "error in: " + msg);
			}
			finally {
				try {
					deleteDirectory(new File(tempTestDir));
				} catch (IOException ignored) {
				}
			}
		}
	}

	public static String getFilenameFromFirstLineOfGrammar(String line) {
		String fileName = "A" + Tool.GRAMMAR_EXTENSION;
		int grIndex = line.LastIndexOf("grammar");
		int semi = line.LastIndexOf(';');
		if ( grIndex>=0 && semi>=0 ) {
			int space = line.IndexOf(' ', grIndex);
			fileName = line.Substring(space+1, semi-(space+1))+Tool.GRAMMAR_EXTENSION;
		}
		if ( fileName.Length ==Tool.GRAMMAR_EXTENSION.Length ) fileName = "A" + Tool.GRAMMAR_EXTENSION;
		return fileName;
	}

	public static List<String> realElements(List<String> elements) {
		return elements.subList(Token.MIN_USER_TOKEN_TYPE, elements.Count);
	}

	public static String load(String fileName)
			 {
		if ( fileName==null ) {
			return null;
		}

		String fullFileName = ToolTestUtils.getPackage().getName().replace('.', '/')+'/'+fileName;
		int size = 65000;
		InputStream fis = ToolTestUtils.getClassLoader().getResourceAsStream(fullFileName);
		using (InputStreamReader isr = new InputStreamReader(fis)) {
			char[] data = new char[size];
			int n = isr.read(data);
			return new String(data, 0, n);
		}
	}

	public static ATN createATN(Grammar g, bool useSerializer) {
		if ( g.atn==null ) {
			semanticProcess(g);
			Assert.AreEqual(0, g.tool.getNumErrors());

			ParserATNFactory f = g.isLexer() ? new LexerATNFactory((LexerGrammar) g) : new ParserATNFactory(g);

			g.atn = f.createATN();
			Assert.AreEqual(0, g.tool.getNumErrors());
		}

		ATN atn = g.atn;
		if ( useSerializer ) {
			// sets some flags in ATN
			IntegerList serialized = ATNSerializer.getSerialized(atn);
			return new ATNDeserializer().deserialize(serialized.toArray());
		}

		return atn;
	}

	public static void semanticProcess(Grammar g) {
		if ( g.ast!=null && !g.ast.hasErrors ) {
//			Console.Out.WriteLine(g.ast.toStringTree());
			Tool antlr = new Tool();
			SemanticPipeline sem = new SemanticPipeline(g);
			sem.process();
			if ( g.getImportedGrammars()!=null ) { // process imported grammars (if any)
				foreach (Grammar imp in g.getImportedGrammars()) {
					antlr.processNonCombinedGrammar(imp, false);
				}
			}
		}
	}

	public static IntegerList getTokenTypesViaATN(String input, LexerATNSimulator lexerATN) {
		ANTLRInputStream @in = new ANTLRInputStream(input);
		IntegerList tokenTypes = new IntegerList();
		int ttype;
		do {
			ttype = lexerATN.match(@in, Lexer.DEFAULT_MODE);
			tokenTypes.add(ttype);
		} while ( ttype!= Token.EOF );
		return tokenTypes;
	}
}
