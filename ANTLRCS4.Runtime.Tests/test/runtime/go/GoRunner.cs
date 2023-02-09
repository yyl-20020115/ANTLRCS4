/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */
using org.antlr.v4.test.runtime.states;

namespace org.antlr.v4.test.runtime.go;

public class GoRunner : RuntimeRunner
{
    public override string GetLanguage() => "Go";

    
    public override string GetLexerSuffix() => "_lexer";

    
    public override string GetParserSuffix() => "_parser";

    
    public override string GetBaseListenerSuffix() => "_base_listener";

    
    public override string GetListenerSuffix() => "_listener";

    
    public override string GetBaseVisitorSuffix() => "_base_visitor";

    
    public override string GetVisitorSuffix() => "_visitor";


    public override string GrammarNameToFileName(string grammarName) => grammarName.ToLower();

    
    public override string[] GetExtraRunArgs() => new string[] { "run" };

    private static readonly string GoRuntimeImportPath = "github.com/antlr/antlr4/runtime/Go/antlr/v4";

    private static readonly Dictionary<string, string> environment;

    private static string cachedGoMod;

    static GoRunner()
    {
        environment = new();
        environment.Add("GOWORK", "off");
    }

    
    protected override void InitRuntime()
    {
        var cachePath = GetCachePath();
        FileUtils.MakeDirectory(cachePath);
        var runtimeFilesPath = Path.Combine(GetRuntimePath("Go"), "antlr");
        var runtimeToolPath = GetRuntimeToolPath();
        var goModFile = Path.Combine(cachePath, "go.mod");
        if (File.Exists(goModFile))
            File.Delete(goModFile);
        if (File.Exists(goModFile))
            throw new IOException("Can't delete " + goModFile);
        Processor.Run(new string[] { runtimeToolPath, "mod", "init", "test" }, cachePath, environment);
        Processor.Run(new string[] {runtimeToolPath, "mod", "edit",
                "-replace=" + GoRuntimeImportPath + "=" + runtimeFilesPath}, cachePath, environment);
        cachedGoMod = FileUtils.ReadFile(cachePath, "go.mod");
    }

    
    protected override string GrammarParseRuleToRecognizerName(string startRuleName)
    {
        if (startRuleName == null || startRuleName.Length == 0)
        {
            return null;
        }

        return startRuleName[..1].ToUpper() + startRuleName[1..];
    }

    
    public override CompiledState Compile(RunOptions runOptions, GeneratedState generatedState)
    {
        var generatedFiles = generatedState.generatedFiles;
        var tempDirPath = GetTempDirPath();
        var generatedParserDir = Path.Combine(tempDirPath, "parser");
        if (!Directory.CreateDirectory(generatedParserDir).Exists)
        {
            return new CompiledState(generatedState, new Exception("can't make dir " + generatedParserDir));
        }

        // The generated files seem to need to be in the parser subdirectory.
        // We have no need to change the import of the runtime because of go mod replace so, we could just generate them
        // directly in to the parser subdir. But in case down the line, there is some reason to want to replace things in
        // the generated code, then I will leave this here, and we can use replaceInFile()
        //
        foreach (var generatedFile in generatedFiles)
        {
            try
            {
                var originalFile = Path.Combine(tempDirPath, generatedFile.name);
                File.Move(originalFile,
                    Path.Combine(tempDirPath, "parser", generatedFile.name));
            }
            catch (IOException e)
            {
                return new CompiledState(generatedState, e);
            }
        }

        FileUtils.WriteFile(tempDirPath, "go.mod", cachedGoMod);
        Exception ex = null;
        try
        {
            Processor.Run(new string[] { GetRuntimeToolPath(), "mod", "tidy" }, tempDirPath, environment);
        }
        catch (Exception e)
        {
            ex = e;
        }

        return new CompiledState(generatedState, ex);
    }

    
    public override Dictionary<string, string> GetExecEnvironment() => environment;
}