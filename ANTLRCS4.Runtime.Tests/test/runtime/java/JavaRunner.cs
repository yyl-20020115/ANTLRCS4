/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */
using Antlr3.Runtime;
using org.antlr.v4.runtime;
using org.antlr.v4.runtime.misc;
using org.antlr.v4.runtime.tree;
using org.antlr.v4.test.runtime.states;
using System.IO.Pipes;
using System.Reflection;

namespace org.antlr.v4.test.runtime.java;

public class JavaRunner : RuntimeRunner {
	//@Override
	public override String getLanguage() {
		return "Java";
	}

	public static readonly String classPath = Environment.GetEnvironmentVariable("java.class.path");

	public static readonly String runtimeTestLexerName = "RuntimeTestLexer";
	public static readonly String runtimeTestParserName = "RuntimeTestParser";

	private static readonly String testLexerContent;
	private static readonly String testParserContent;
	//private static JavaCompiler compiler;

	static JavaRunner(){
		testLexerContent = RuntimeTestUtils.getTextFromResource("org/antlr/v4/test/runtime/helpers/" + runtimeTestLexerName + ".java");
		testParserContent = RuntimeTestUtils.getTextFromResource("org/antlr/v4/test/runtime/helpers/" + runtimeTestParserName + ".java");
	}

	public JavaRunner(string tempDir, bool saveTestDir): base(tempDir, saveTestDir)
    {
		;
	}

	public JavaRunner(): base()
    {
	}

	//@Override
	protected void initRuntime() {
		//compiler = ToolProvider.getSystemJavaCompiler();
	}

	//@Override
	protected String getCompilerName() {
		return "javac";
	}
#if false
	//@Override
	protected JavaCompiledState compile(RunOptions runOptions, GeneratedState generatedState) {
		String tempTestDir = getTempDirPath();

		List<GeneratedFile> generatedFiles = generatedState.generatedFiles;
		GeneratedFile firstFile = generatedFiles[(0)];

		if (!firstFile.isParser) {
			FileUtils.writeFile(tempTestDir, runtimeTestLexerName + ".java", testLexerContent);
			try {
				// superClass for combined grammar generates the same : base class for Lexer and Parser
				// So, for lexer it should be replaced on correct base lexer class
				replaceInFile(Paths.get(getTempDirPath(), firstFile.name),
						": " + runtimeTestParserName + " {",
						": " + runtimeTestLexerName + " {");
			} catch (IOException e) {
				return new JavaCompiledState(generatedState, null, null, null, e);
			}
		}
		if (generatedFiles.Any(f=>f.isParser)) {
			FileUtils.writeFile(tempTestDir,  runtimeTestParserName + ".java", testParserContent);
		}

		Type lexer = null;
        Type parser = null;
		Exception exception = null;

		try {
			
			List<string> files = new ();
            string f = Path.Combine(tempTestDir, getTestFileWithExt());
			files.Add(f);

			Iterable<JavaFileObject> compilationUnits = fileManager.getJavaFileObjectsFromFiles(files);

			var compileOptions =
					Arrays.AsList("-g", "-source", "1.8", "-target", "1.8", "-implicit:class", "-Xlint:-options", "-d",
							tempTestDir, "-cp", tempTestDir + PathSeparator + classPath);

			JavaCompiler.CompilationTask task =
					compiler.getTask(null, fileManager, null, compileOptions, null,
							compilationUnits);
			task.call();

			//loader = new URLClassLoader(new URL[]{new File(tempTestDir).toURI().toURL()}, systemClassLoader);
			if (runOptions.lexerName != null) {
				lexer = loader.loadClass(runOptions.lexerName).asSubclass(Lexer);
			}
			if (runOptions.parserName != null) {
				parser = loader.loadClass(runOptions.parserName).asSubclass(Parser);
			}
		} catch (Exception ex) {
			exception = ex;
		}

		return new JavaCompiledState(generatedState, loader, lexer, parser, exception);
	}
#endif
	//@Override
	protected ExecutedState execute(RunOptions runOptions, CompiledState compiledState) {
		JavaCompiledState javaCompiledState = (JavaCompiledState) compiledState;

		ExecutedState result;
		if (runOptions.returnObject) {
			result = execWithObject(runOptions, javaCompiledState);
		} else {
			result = execCommon(javaCompiledState);
		}
		return result;
	}

	private JavaExecutedState execWithObject(RunOptions runOptions, JavaCompiledState javaCompiledState) {
		ParseTree parseTree = null;
		Exception exception = null;
		try {
			var lexerParser = javaCompiledState.initializeLexerAndParser(runOptions.input);

			if (runOptions.parserName != null) {
				MethodInfo startRule;
				Object[] args = null;
				try {
					startRule = javaCompiledState.parserType.GetMethod(runOptions.startRuleName);
				} catch (Exception noSuchMethodException) {
					// try with int _p arg for recursive func
					startRule = javaCompiledState.parserType.GetMethod(runOptions.startRuleName,new Type[] { typeof(int) });
					args = new object[]{0};
				}
				parseTree = startRule.Invoke(lexerParser.b, args) as ParseTree;
			}
		} catch (Exception ex) {
			exception = ex;
		}
		return new JavaExecutedState(javaCompiledState, null, null, parseTree, exception);
	}

	private ExecutedState execCommon(JavaCompiledState compiledState) {
		Exception exception = null;
		String output = null;
		String errors = null;
		try {
			 Type mainClass = this.GetType().Assembly.GetType(getTestFileName());
			 MethodInfo recognizeMethod = mainClass.GetMethod("recognize",
					new Type[] { typeof(String),typeof(TextWriter),typeof(TextWriter) });

			//PipedInputStream stdoutIn = new PipedInputStream();
			//PipedInputStream stderrIn = new PipedInputStream();
			//PipedOutputStream stdoutOut = new PipedOutputStream(stdoutIn);
			//PipedOutputStream stderrOut = new PipedOutputStream(stderrIn);

			//TODO: how about pipes			
			var stdoutIn = new StringReader("");
			var stderrIn = new StringReader("");

            var stdoutOut = new StringWriter();
            var stderrOut = new StringWriter();
			//TODO: pipe
            RunnableStreamReader stdoutReader = new RunnableStreamReader(stdoutIn);
            RunnableStreamReader stderrReader = new RunnableStreamReader(stderrIn);
			stdoutReader.start();
			stderrReader.start();

			recognizeMethod.Invoke(null,new object[]{ Path.Combine(getTempDirPath(), "input"),
					stdoutOut, stderrOut });

			stdoutOut.Close();
			stderrOut.Close();
			stdoutReader.join();
			stderrReader.join();
			output = stdoutReader.ToString();
			errors = stderrReader.ToString();
		} catch (Exception ex) {
			exception = ex;
		}
		return new JavaExecutedState(compiledState, output, errors, null, exception);
	}
}
