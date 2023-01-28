/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */
using org.antlr.v4.test.runtime.states;

namespace org.antlr.v4.test.runtime.go;

public class GoRunner : RuntimeRunner
{
    public override String getLanguage()
    {
        return "Go";
    }

    ////@Override
    public override String getLexerSuffix()
    {
        return "_lexer";
    }

    ////@Override
    public String getParserSuffix()
    {
        return "_parser";
    }

    ////@Override
    public String getBaseListenerSuffix()
    {
        return "_base_listener";
    }

    ////@Override
    public String getListenerSuffix()
    {
        return "_listener";
    }

    ////@Override
    public String getBaseVisitorSuffix()
    {
        return "_base_visitor";
    }

    ////@Override
    public String getVisitorSuffix()
    {
        return "_visitor";
    }

    ////@Override
    protected String grammarNameToFileName(String grammarName)
    {
        return grammarName.ToLower();
    }

    ////@Override
    public String[] getExtraRunArgs()
    {
        return new String[] { "run" };
    }

    private static readonly String GoRuntimeImportPath = "github.com/antlr/antlr4/runtime/Go/antlr/v4";

    private static readonly Dictionary<String, String> environment;

    private static String cachedGoMod;

    static GoRunner()
    {
        environment = new();
        environment.Add("GOWORK", "off");
    }

    ////@Override
    protected void initRuntime()
    {
        String cachePath = getCachePath();
        mkdir(cachePath);
        Path runtimeFilesPath = Paths.get(getRuntimePath("Go"), "antlr");
        String runtimeToolPath = getRuntimeToolPath();
        File goModFile = new File(cachePath, "go.mod");
        if (goModFile.exists())
            if (!goModFile.delete())
                throw new IOException("Can't delete " + goModFile);
        Processor.run(new String[] { runtimeToolPath, "mod", "init", "test" }, cachePath, environment);
        Processor.run(new String[] {runtimeToolPath, "mod", "edit",
                "-replace=" + GoRuntimeImportPath + "=" + runtimeFilesPath}, cachePath, environment);
        cachedGoMod = readFile(cachePath + FileSeparator, "go.mod");
    }

    ////@Override
    protected String grammarParseRuleToRecognizerName(String startRuleName)
    {
        if (startRuleName == null || startRuleName.length() == 0)
        {
            return null;
        }

        return startRuleName[..1].ToUpper() + startRuleName.Substring(1);
    }

    ////@Override
    protected CompiledState compile(RunOptions runOptions, GeneratedState generatedState)
    {
        List<GeneratedFile> generatedFiles = generatedState.generatedFiles;
        String tempDirPath = getTempDirPath();
        File generatedParserDir = new File(tempDirPath, "parser");
        if (!generatedParserDir.mkdir())
        {
            return new CompiledState(generatedState, new Exception("can't make dir " + generatedParserDir));
        }

        // The generated files seem to need to be in the parser subdirectory.
        // We have no need to change the import of the runtime because of go mod replace so, we could just generate them
        // directly in to the parser subdir. But in case down the line, there is some reason to want to replace things in
        // the generated code, then I will leave this here, and we can use replaceInFile()
        //
        foreach (GeneratedFile generatedFile in generatedFiles)
        {
            try
            {
                Path originalFile = Paths.get(tempDirPath, generatedFile.name);
                Files.move(originalFile, Paths.get(tempDirPath, "parser", generatedFile.name));
            }
            catch (IOException e)
            {
                return new CompiledState(generatedState, e);
            }
        }

        writeFile(tempDirPath, "go.mod", cachedGoMod);
        Exception ex = null;
        try
        {
            Processor.run(new String[] { getRuntimeToolPath(), "mod", "tidy" }, tempDirPath, environment);
        }
        catch (Exception e)
        {
            ex = e;
        }

        return new CompiledState(generatedState, ex);
    }

    ////@Override
    public Dictionary<String, String> getExecEnvironment()
    {
        return environment;
    }
}