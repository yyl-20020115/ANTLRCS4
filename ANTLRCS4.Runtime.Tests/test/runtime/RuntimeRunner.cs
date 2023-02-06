/*
 * Copyright (c) 2012-2022 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using Antlr4.StringTemplate;
using org.antlr.v4.test.runtime.states;

namespace org.antlr.v4.test.runtime;

public abstract class RuntimeRunner
{
    public abstract string GetLanguage();

    public virtual string GetExtension() => GetLanguage().ToLower();

    public virtual string GetTitleName() => GetLanguage();

    public virtual string GetTestFileName() => "Test";

    public virtual string GetLexerSuffix() => "Lexer";

    public virtual string GetParserSuffix() => "Parser";

    public virtual string GetBaseListenerSuffix() => "BaseListener";

    public virtual string GetListenerSuffix() => "Listener";

    public virtual string GetBaseVisitorSuffix() => "BaseVisitor";

    public virtual string GetVisitorSuffix() => "Visitor";

    public virtual string GrammarNameToFileName(string grammarName) => grammarName;

    private static string runtimeToolPath;
    private static string compilerPath;

    public virtual string GetCompilerPath()
    {
        if (compilerPath == null)
        {
            compilerPath = GetCompilerName();
            if (compilerPath != null)
            {
                var compilerPathFromProperty = Environment.GetEnvironmentVariable(GetPropertyPrefix() + "-compiler");
                if (compilerPathFromProperty != null && compilerPathFromProperty.Length > 0)
                {
                    compilerPath = compilerPathFromProperty;
                }
            }
        }

        return compilerPath;
    }

    public virtual string GetRuntimeToolPath()
    {
        if (runtimeToolPath == null)
        {
            runtimeToolPath = GetRuntimeToolName();
            if (runtimeToolPath != null)
            {
                var runtimeToolPathFromProperty = Environment.GetEnvironmentVariable(GetPropertyPrefix() + "-exec");
                if (runtimeToolPathFromProperty != null && runtimeToolPathFromProperty.Length > 0)
                {
                    runtimeToolPath = runtimeToolPathFromProperty;
                }
            }
        }

        return runtimeToolPath;
    }

    public virtual string GetCompilerName() => null;

    public virtual string GetRuntimeToolName() => GetLanguage().ToLower();

    public virtual string GetTestFileWithExt() => GetTestFileName() + "." + GetExtension();

    public virtual string GetExecFileName() => GetTestFileWithExt();

    public virtual string[] GetExtraRunArgs() => null;

    public virtual Dictionary<string, string> GetExecEnvironment() => null;

    public virtual string GetPropertyPrefix() => "antlr-" + GetLanguage().ToLower();

    public virtual string GetTempDirPath() => tempTestDir.ToString();

    protected bool saveTestDir;

    protected readonly string tempTestDir;

    protected RuntimeRunner()
        : this(null, false)
    {
    }

    protected RuntimeRunner(string tempDir, bool saveTestDir)
    {
        if (tempDir == null)
        {
            var dirName = GetType().Name + "-" + Thread.CurrentThread.Name + "-" + DateTime.Now.Millisecond;
            tempTestDir = Path.Combine(RuntimeTestUtils.TempDirectory, dirName);
        }
        else
        {
            tempTestDir = tempDir;
        }
        this.saveTestDir = saveTestDir;
    }

    public virtual void SetSaveTestDir(bool saveTestDir)
    {
        this.saveTestDir = saveTestDir;
    }

    public virtual void Close()
    {
        RemoveTempTestDirIfRequired();
    }

    public static readonly string cacheDirectory;

    private class InitializationStatus
    {
        public object lockObject = new();
        public volatile bool isInitialized;
        public Exception exception;
    }

    private static readonly Dictionary<string, InitializationStatus> runtimeInitializationStatuses = new();

    static RuntimeRunner()
    {
        cacheDirectory = Environment.CurrentDirectory;// new File(System.getProperty("java.io.tmpdir"), "ANTLR-runtime-testsuite-cache").getAbsolutePath();
    }

    public virtual string GetCachePath() => GetCachePath(GetLanguage());

    public static string GetCachePath(string language) => Path.Combine(cacheDirectory, language);

    protected virtual string GetRuntimePath() => GetRuntimePath(GetLanguage());

    public static string GetRuntimePath(string language) => Path.Combine(RuntimeTestUtils.runtimePath, language);

    public virtual State Run(RunOptions runOptions)
    {
        List<string> options = new();
        if (runOptions.useVisitor)
        {
            options.Add("-visitor");
        }
        if (runOptions.superClass != null && runOptions.superClass.Length > 0)
        {
            options.Add("-DsuperClass=" + runOptions.superClass);
        }
        var errorQueue = Generator.AntlrOnString(GetTempDirPath(), GetLanguage(),
                runOptions.grammarFileName, runOptions.grammarStr, false, options.ToArray());

        var generatedFiles = GetGeneratedFiles(runOptions);
        var generatedState = new GeneratedState(errorQueue, generatedFiles, null);

        if (generatedState.ContainsErrors() || runOptions.endStage == Stage.Generate)
        {
            return generatedState;
        }

        if (!InitAntlrRuntimeIfRequired())
        {
            // Do not repeat ANTLR runtime initialization error
            return new CompiledState(generatedState, new Exception(GetTitleName() + " ANTLR runtime is not initialized"));
        }

        WriteRecognizerFile(runOptions);

        var compiledState = Compile(runOptions, generatedState);

        if (compiledState.ContainsErrors() || runOptions.endStage == Stage.Compile)
        {
            return compiledState;
        }

        FileUtils.WriteFile(GetTempDirPath(), "input", runOptions.input);

        return Execute(runOptions, compiledState);
    }

    protected virtual List<GeneratedFile> GetGeneratedFiles(RunOptions runOptions)
    {
        List<GeneratedFile> files = new();
        var extensionWithDot = "." + GetExtension();
        var fileGrammarName = GrammarNameToFileName(runOptions.grammarName);
        bool isCombinedGrammarOrGo = runOptions.lexerName != null && runOptions.parserName != null || GetLanguage().Equals("Go");
        if (runOptions.lexerName != null)
        {
            files.Add(new(fileGrammarName + (isCombinedGrammarOrGo ? GetLexerSuffix() : "") + extensionWithDot, false));
        }
        if (runOptions.parserName != null)
        {
            files.Add(new(fileGrammarName + (isCombinedGrammarOrGo ? GetParserSuffix() : "") + extensionWithDot, true));
            if (runOptions.useListener)
            {
                files.Add(new(fileGrammarName + GetListenerSuffix() + extensionWithDot, true));
                var baseListenerSuffix = GetBaseListenerSuffix();
                if (baseListenerSuffix != null)
                {
                    files.Add(new(fileGrammarName + baseListenerSuffix + extensionWithDot, true));
                }
            }
            if (runOptions.useVisitor)
            {
                files.Add(new(fileGrammarName + GetVisitorSuffix() + extensionWithDot, true));
                var baseVisitorSuffix = GetBaseVisitorSuffix();
                if (baseVisitorSuffix != null)
                {
                    files.Add(new(fileGrammarName + baseVisitorSuffix + extensionWithDot, true));
                }
            }
        }
        return files;
    }

    protected virtual void WriteRecognizerFile(RunOptions runOptions)
    {
        var text = RuntimeTestUtils.GetTextFromResource("org/antlr/v4/test/runtime/helpers/" + GetTestFileWithExt() + ".stg");
        var outputFileST = new Template(text);
        outputFileST.Add("grammarName", runOptions.grammarName);
        outputFileST.Add("lexerName", runOptions.lexerName);
        outputFileST.Add("parserName", runOptions.parserName);
        outputFileST.Add("parserStartRuleName", GrammarParseRuleToRecognizerName(runOptions.startRuleName));
        outputFileST.Add("debug", runOptions.showDiagnosticErrors);
        outputFileST.Add("profile", runOptions.profile);
        outputFileST.Add("showDFA", runOptions.showDFA);
        outputFileST.Add("useListener", runOptions.useListener);
        outputFileST.Add("useVisitor", runOptions.useVisitor);
        AddExtraRecognizerParameters(outputFileST);
        FileUtils.WriteFile(GetTempDirPath(), GetTestFileWithExt(), outputFileST.Render());
    }

    protected virtual string GrammarParseRuleToRecognizerName(string startRuleName)
    {
        return startRuleName;
    }

    protected virtual void AddExtraRecognizerParameters(Template template)
    {
    }

    protected virtual bool InitAntlrRuntimeIfRequired()
    {
        var language = GetLanguage();
        InitializationStatus status = default;
        // Create initialization status for every runtime with lock object
        lock (runtimeInitializationStatuses)
        {
            if (!runtimeInitializationStatuses.TryGetValue(language, out status))
            {
                runtimeInitializationStatuses[language] =
                    status = new InitializationStatus();
            }
        }

        if (status.isInitialized)
            return status.isInitialized;

        // Locking per runtime, several runtimes can be being initialized simultaneously
        lock (status.lockObject)
        {
            if (!status.isInitialized)
            {
                Exception exception = null;
                try
                {
                    InitRuntime();
                }
                catch (Exception e)
                {
                    exception = e;
                    //e.printStackTrace();
                }
                status.isInitialized = exception == null;
                status.exception = exception;
            }
        }
        return status.isInitialized;
    }

    protected virtual void InitRuntime()
    {
    }

    public virtual CompiledState Compile(RunOptions runOptions, GeneratedState generatedState)
    {
        return new CompiledState(generatedState, null);
    }

    public virtual ExecutedState Execute(RunOptions runOptions, CompiledState compiledState)
    {
        string output = null;
        string errors = null;
        Exception exception = null;
        try
        {
            List<string> args = new();
            var runtimeToolPath = GetRuntimeToolPath();
            if (runtimeToolPath != null)
            {
                args.Add(runtimeToolPath);
            }
            var extraRunArgs = GetExtraRunArgs();
            if (extraRunArgs != null)
            {
                args.AddRange((extraRunArgs));
            }
            args.Add(GetExecFileName());
            args.Add("input");
            var result = Processor.Run(args.ToArray(), GetTempDirPath(), GetExecEnvironment());
            output = result.output;
            errors = result.errors;
        }
        catch (Exception e)
        {
            exception = e;
        }
        return new ExecutedState(compiledState, output, errors, exception);
    }

    public virtual ProcessorResult RunCommand(string[] command, string workPath, string description = null)
    {
        try
        {
            return Processor.Run(command, workPath);
        }
        catch (Exception e)
        {
            throw description != null ? new Exception("can't " + description, e) : e;
        }
    }

    public virtual void RemoveTempTestDirIfRequired()
    {
        if (!saveTestDir)
        {
            var dirFile = tempTestDir;
            if (File.Exists(dirFile))
            {
                try
                {
                    FileUtils.DeleteDirectory(dirFile);
                }
                catch (IOException e)
                {
                    //e.printStackTrace();
                }
            }
        }
    }
}
