/*
 * Copyright (c) 2012-2022 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.test.runtime.states;

namespace org.antlr.v4.test.runtime;

public abstract class RuntimeRunner {
	public abstract String getLanguage();

	protected String getExtension() { return getLanguage().ToLower(); }

	protected String getTitleName() { return getLanguage(); }

	protected String getTestFileName() { return "Test"; }

	protected String getLexerSuffix() { return "Lexer"; }

	protected String getParserSuffix() { return "Parser"; }

	protected String getBaseListenerSuffix() { return "BaseListener"; }

	protected String getListenerSuffix() { return "Listener"; }

	protected String getBaseVisitorSuffix() { return "BaseVisitor"; }

	protected String getVisitorSuffix() { return "Visitor"; }

	protected String grammarNameToFileName(String grammarName) { return grammarName; }

	private static String runtimeToolPath;
	private static String compilerPath;

	protected String getCompilerPath() {
		if (compilerPath == null) {
			compilerPath = getCompilerName();
			if (compilerPath != null) {
				String compilerPathFromProperty = System.getProperty(getPropertyPrefix() + "-compiler");
				if (compilerPathFromProperty != null && compilerPathFromProperty.length() > 0) {
					compilerPath = compilerPathFromProperty;
				}
			}
		}

		return compilerPath;
	}

	protected String getRuntimeToolPath() {
		if (runtimeToolPath == null) {
			runtimeToolPath = getRuntimeToolName();
			if (runtimeToolPath != null) {
				String runtimeToolPathFromProperty = System.getProperty(getPropertyPrefix() + "-exec");
				if (runtimeToolPathFromProperty != null && runtimeToolPathFromProperty.length() > 0) {
					runtimeToolPath = runtimeToolPathFromProperty;
				}
			}
		}

		return runtimeToolPath;
	}

	protected String getCompilerName() { return null; }

	protected String getRuntimeToolName() { return getLanguage().ToLower(); }

	protected String getTestFileWithExt() { return getTestFileName() + "." + getExtension(); }

	protected String getExecFileName() { return getTestFileWithExt(); }

	protected String[] getExtraRunArgs() { return null; }

	protected Dictionary<String, String> getExecEnvironment() { return null; }

	protected String getPropertyPrefix() {
		return "antlr-" + getLanguage().ToLower();
	}

	public  String getTempDirPath() {
		return tempTestDir.ToString();
	}

	private bool saveTestDir;

	protected readonly Path tempTestDir;

	protected RuntimeRunner() {
		this(null, false);
	}

	protected RuntimeRunner(string tempDir, bool saveTestDir) {
		if (tempDir == null) {
			String dirName = getClass().getSimpleName() + "-" + Thread.currentThread().getName() + "-" + System.currentTimeMillis();
			tempTestDir = Paths.get(TempDirectory, dirName);
		}
		else {
			tempTestDir = tempDir;
		}
		this.saveTestDir = saveTestDir;
	}

	public void setSaveTestDir(bool saveTestDir) {
		this.saveTestDir = saveTestDir;
	}

	public void close() {
		removeTempTestDirIfRequired();
	}

	public static readonly String cacheDirectory;

	private class InitializationStatus {
		public  Object lockObject = new Object();
		public volatile Boolean isInitialized;
		public Exception exception;
	}

	private static readonly HashMap<String, InitializationStatus> runtimeInitializationStatuses = new HashMap<>();

	static RuntimeRunner() {
		cacheDirectory = new File(System.getProperty("java.io.tmpdir"), "ANTLR-runtime-testsuite-cache").getAbsolutePath();
	}

	protected String getCachePath() {
		return getCachePath(getLanguage());
	}

	public static String getCachePath(String language) {
		return cacheDirectory + FileSeparator + language;
	}

	protected String getRuntimePath() {
		return getRuntimePath(getLanguage());
	}

	public static String getRuntimePath(String language) {
		return runtimePath.ToString() + FileSeparator + language;
	}

	public State run(RunOptions runOptions) {
		List<String> options = new ;
		if (runOptions.useVisitor) {
			options.add("-visitor");
		}
		if (runOptions.superClass != null && runOptions.superClass.length() > 0) {
			options.add("-DsuperClass=" + runOptions.superClass);
		}
		ErrorQueue errorQueue = Generator.antlrOnString(getTempDirPath(), getLanguage(),
				runOptions.grammarFileName, runOptions.grammarStr, false, options.toArray(new String[0]));

		List<GeneratedFile> generatedFiles = getGeneratedFiles(runOptions);
		GeneratedState generatedState = new GeneratedState(errorQueue, generatedFiles, null);

		if (generatedState.containsErrors() || runOptions.endStage == Stage.Generate) {
			return generatedState;
		}

		if (!initAntlrRuntimeIfRequired()) {
			// Do not repeat ANTLR runtime initialization error
			return new CompiledState(generatedState, new Exception(getTitleName() + " ANTLR runtime is not initialized"));
		}

		writeRecognizerFile(runOptions);

		CompiledState compiledState = compile(runOptions, generatedState);

		if (compiledState.containsErrors() || runOptions.endStage == Stage.Compile) {
			return compiledState;
		}

		writeFile(getTempDirPath(), "input", runOptions.input);

		return execute(runOptions, compiledState);
	}

	protected List<GeneratedFile> getGeneratedFiles(RunOptions runOptions) {
		List<GeneratedFile> files = new ();
		String extensionWithDot = "." + getExtension();
		String fileGrammarName = grammarNameToFileName(runOptions.grammarName);
		bool isCombinedGrammarOrGo = runOptions.lexerName != null && runOptions.parserName != null || getLanguage().Equals("Go");
		if (runOptions.lexerName != null) {
			files.Add(new GeneratedFile(fileGrammarName + (isCombinedGrammarOrGo ? getLexerSuffix() : "") + extensionWithDot, false));
		}
		if (runOptions.parserName != null) {
			files.Add(new GeneratedFile(fileGrammarName + (isCombinedGrammarOrGo ? getParserSuffix() : "") + extensionWithDot, true));
			if (runOptions.useListener) {
				files.Add(new GeneratedFile(fileGrammarName + getListenerSuffix() + extensionWithDot, true));
				String baseListenerSuffix = getBaseListenerSuffix();
				if (baseListenerSuffix != null) {
					files.Add(new GeneratedFile(fileGrammarName + baseListenerSuffix + extensionWithDot, true));
				}
			}
			if (runOptions.useVisitor) {
				files.Add(new GeneratedFile(fileGrammarName + getVisitorSuffix() + extensionWithDot, true));
				String baseVisitorSuffix = getBaseVisitorSuffix();
				if (baseVisitorSuffix != null) {
					files.Add(new GeneratedFile(fileGrammarName + baseVisitorSuffix + extensionWithDot, true));
				}
			}
		}
		return files;
	}

	protected void writeRecognizerFile(RunOptions runOptions) {
		String text = RuntimeTestUtils.getTextFromResource("org/antlr/v4/test/runtime/helpers/" + getTestFileWithExt() + ".stg");
		ST outputFileST = new ST(text);
		outputFileST.add("grammarName", runOptions.grammarName);
		outputFileST.add("lexerName", runOptions.lexerName);
		outputFileST.add("parserName", runOptions.parserName);
		outputFileST.add("parserStartRuleName", grammarParseRuleToRecognizerName(runOptions.startRuleName));
		outputFileST.add("debug", runOptions.showDiagnosticErrors);
		outputFileST.add("profile", runOptions.profile);
		outputFileST.add("showDFA", runOptions.showDFA);
		outputFileST.add("useListener", runOptions.useListener);
		outputFileST.add("useVisitor", runOptions.useVisitor);
		addExtraRecognizerParameters(outputFileST);
		writeFile(getTempDirPath(), getTestFileWithExt(), outputFileST.render());
	}

	protected String grammarParseRuleToRecognizerName(String startRuleName) {
		return startRuleName;
	}

	protected void addExtraRecognizerParameters(ST template) {}

	private bool initAntlrRuntimeIfRequired() {
		String language = getLanguage();
		InitializationStatus status;

		// Create initialization status for every runtime with lock object
		lock (runtimeInitializationStatuses) {
			status = runtimeInitializationStatuses.get(language);
			if (status == null) {
				status = new InitializationStatus();
				runtimeInitializationStatuses.put(language, status);
			}
		}

		if (status.isInitialized != null) {
			return status.isInitialized;
		}

		// Locking per runtime, several runtimes can be being initialized simultaneously
		lock (status.lockObject) {
			if (status.isInitialized == null) {
				Exception exception = null;
				try {
					initRuntime();
				} catch (Exception e) {
					exception = e;
					e.printStackTrace();
				}
				status.isInitialized = exception == null;
				status.exception = exception;
			}
		}
		return status.isInitialized;
	}

	protected void initRuntime()  {
	}

	protected CompiledState compile(RunOptions runOptions, GeneratedState generatedState) {
		return new CompiledState(generatedState, null);
	}

	protected ExecutedState execute(RunOptions runOptions, CompiledState compiledState) {
		String output = null;
		String errors = null;
		Exception exception = null;
		try {
			List<String> args = new ();
			String runtimeToolPath = getRuntimeToolPath();
			if (runtimeToolPath != null) {
				args.add(runtimeToolPath);
			}
			String[] extraRunArgs = getExtraRunArgs();
			if (extraRunArgs != null) {
				args.addAll(Arrays.asList(extraRunArgs));
			}
			args.add(getExecFileName());
			args.add("input");
			ProcessorResult result = Processor.run(args.toArray(new String[0]), getTempDirPath(), getExecEnvironment());
			output = result.output;
			errors = result.errors;
		} catch (Exception e) {
			exception = e;
		}
		return new ExecutedState(compiledState, output, errors, exception);
	}

	protected ProcessorResult runCommand(String[] command, String workPath)  {
		return runCommand(command, workPath, null);
	}

	protected ProcessorResult runCommand(String[] command, String workPath, String description)  {
		try {
			return Processor.run(command, workPath);
		} catch (Exception e) {
			throw description != null ? new Exception("can't " + description, e) : e;
		}
	}

	private void removeTempTestDirIfRequired() {
		if (!saveTestDir) {
			File dirFile = tempTestDir.toFile();
			if (dirFile.exists()) {
				try {
					deleteDirectory(dirFile);
				} catch (IOException e) {
					e.printStackTrace();
				}
			}
		}
	}
}
