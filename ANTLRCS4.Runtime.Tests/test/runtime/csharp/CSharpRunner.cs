/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */
using Antlr4.StringTemplate;
using org.antlr.v4.test.runtime.states;

namespace org.antlr.v4.test.runtime.csharp;

public class CSharpRunner : RuntimeRunner
{
    ////@Override
    public override string GetLanguage() { return "CSharp"; }

    ////@Override
    public override string GetTitleName() { return "C#"; }

    ////@Override
    public override string GetExtension() { return "cs"; }

    ////@Override
    public override string GetRuntimeToolName() { return "dotnet"; }

    ////@Override
    public override string GetExecFileName() { return GetTestFileName() + ".dll"; }

    private static readonly string testProjectFileName = "Antlr4.Test.csproj";
    private static readonly string cSharpAntlrRuntimeDllName =
            Path.Combine(GetCachePath("CSharp"), "Antlr4.Runtime.Standard.dll").ToString();

    private static readonly string cSharpTestProjectContent;

    static CSharpRunner()
    {
        var projectTemplate = new Template(RuntimeTestUtils.GetTextFromResource("org/antlr/v4/test/runtime/helpers/Antlr4.Test.csproj.stg"));
        projectTemplate.Add("runtimeLibraryPath", cSharpAntlrRuntimeDllName);
        cSharpTestProjectContent = projectTemplate.Render();
    }

    ////@Override
    protected override void InitRuntime()
    {
        var cachePath = GetCachePath();
        FileUtils.MakeDirectory(cachePath);
        var projectPath = Path.Combine(GetRuntimePath(), "src", "Antlr4.csproj").ToString();
        var args = new string[] { GetRuntimeToolPath(), "build", projectPath, "-c", "Release", "-o", cachePath };
        RunCommand(args, cachePath, "build " + GetTitleName() + " ANTLR runtime");
    }

    ////@Override
    public override CompiledState Compile(RunOptions runOptions, GeneratedState generatedState)
    {
        Exception exception = null;
        try
        {
            FileUtils.WriteFile(GetTempDirPath(), testProjectFileName, cSharpTestProjectContent);
            RunCommand(new string[] { GetRuntimeToolPath(), "build", testProjectFileName, "-c", "Release" }, GetTempDirPath(),
                    "build C# test binary");
        }
        catch (Exception e)
        {
            exception = e;
        }
        return new CompiledState(generatedState, exception);
    }
}
