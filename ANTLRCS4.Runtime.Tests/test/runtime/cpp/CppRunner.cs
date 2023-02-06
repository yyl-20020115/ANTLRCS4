/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */
using Antlr4.StringTemplate;
using org.antlr.v4.test.runtime.states;

namespace org.antlr.v4.test.runtime.cpp;

/**
 * For my own information on I'm recording what I needed to do to get a unit test to compile and run in C++ on the Mac.
 * I got a segmentation violation and couldn't figure out how to get information about it, so I turned on debugging
 * and then figured out lldb enough to create this issue: https://github.com/antlr/antlr4/issues/3845 on a bug.
 *
 * cd ~/antlr/code/antlr4/runtime/Cpp
 * cmake . -D CMAKE_OSX_ARCHITECTURES="arm64; x86_64" -DCMAKE_BUILD_TYPE=Debug
 * make -j 8
 *
 * In test dir with generated test code:
 *
 * clang++ -g -std=c++17 -I /Users/parrt/antlr/code/antlr4/runtime/Cpp/runtime/src -L. -lantlr4-runtime *.cpp
 * ./a.out input
 *
 * $ lldb ./a.out input
 * (lldb) run
 * ... crash ...
 * (lldb) thread backtrace
 */
public class CppRunner : RuntimeRunner
{
    ////@Override
    public override string GetLanguage() => "Cpp";

    ////@Override
    public override string GetTitleName() => "C++";

    private static readonly string runtimeSourcePath;
    private static readonly string runtimeBinaryPath;
    private static readonly string runtimeLibraryFileName;
    private static string compilerName;
    private static readonly string visualStudioProjectContent;
    private static readonly Dictionary<string, string> environment;

    static CppRunner()
    {
        var runtimePath = GetRuntimePath("Cpp");
        runtimeSourcePath = Path.Combine(runtimePath, "runtime", "src").ToString();

        environment = new();
        if (RuntimeTestUtils.IsWindows())
        {
            runtimeBinaryPath = Path.Combine(runtimePath, "runtime", "bin", "vs-2022", "x64", "Release DLL").ToString();
            runtimeLibraryFileName = Path.Combine(runtimeBinaryPath, "antlr4-runtime.dll").ToString();
            var path = Environment.GetEnvironmentVariable("PATH");
            environment.Add("PATH", path == null ? runtimeBinaryPath : path + ";" + runtimeBinaryPath);
        }
        else
        {
            runtimeBinaryPath = Path.Combine(runtimePath, "dist").ToString();
            runtimeLibraryFileName = Path.Combine(runtimeBinaryPath,
                    "libantlr4-runtime." + (RuntimeTestUtils.GetOS() == OSType.Mac ? "dylib" : "so")).ToString();
            environment.Add("LD_PRELOAD", runtimeLibraryFileName);
        }

        visualStudioProjectContent = RuntimeTestUtils.IsWindows()
            ? RuntimeTestUtils.GetTextFromResource("org/antlr/v4/test/runtime/helpers/Test.vcxproj.stg")
            : null;
    }

    ////@Override
    public override string GetCompilerName()
    {
        if (compilerName == null)
        {
            if (RuntimeTestUtils.IsWindows())
            {
                compilerName = "MSBuild";
            }
            else
            {
                compilerName = "clang++";
            }
        }

        return compilerName;
    }

    ////@Override
    protected override void InitRuntime()
    {
        var runtimePath = GetRuntimePath();

        if (RuntimeTestUtils.IsWindows())
        {
            string[] command = {
                GetCompilerPath(), "antlr4cpp-vs2022.vcxproj", "/p:configuration=Release DLL", "/p:platform=x64"
            };

            RunCommand(command, runtimePath + "\\runtime", "build c++ ANTLR runtime using MSBuild");
        }
        else
        {
            string[] command = { "cmake", ".", "-DCMAKE_BUILD_TYPE=Release" };
            RunCommand(command, runtimePath, "run cmake on antlr c++ runtime");

            command = new string[] { "make", "-j", (Environment.ProcessorCount.ToString()) };
            RunCommand(command, runtimePath, "run make on antlr c++ runtime");
        }
    }

    ////@Override
    public override CompiledState Compile(RunOptions runOptions, GeneratedState generatedState)
    {
        if (RuntimeTestUtils.IsWindows())
        {
            WriteVisualStudioProjectFile(runOptions.grammarName, runOptions.lexerName, runOptions.parserName,
                    runOptions.useListener, runOptions.useVisitor);
        }

        Exception exception = null;
        try
        {
            if (!RuntimeTestUtils.IsWindows())
            {
                string[] linkCommand = new string[] { "ln", "-s", runtimeLibraryFileName };
                RunCommand(linkCommand, GetTempDirPath(), "sym link C++ runtime");
            }

            List<string> buildCommand = new();
            buildCommand.Add(GetCompilerPath());
            if (RuntimeTestUtils.IsWindows())
            {
                buildCommand.Add(GetTestFileName() + ".vcxproj");
                buildCommand.Add("/p:configuration=Release");
                buildCommand.Add("/p:platform=x64");
            }
            else
            {
                buildCommand.Add("-std=c++17");
                buildCommand.Add("-I");
                buildCommand.Add(runtimeSourcePath);
                buildCommand.Add("-L.");
                buildCommand.Add("-lantlr4-runtime");
                buildCommand.Add("-pthread");
                buildCommand.Add("-o");
                buildCommand.Add(GetTestFileName() + ".out");
                buildCommand.Add(GetTestFileWithExt());
                buildCommand.AddRange(
                    generatedState.generatedFiles.Select(f => f.name));
            }

            RunCommand(buildCommand.ToArray(), GetTempDirPath(), "build test c++ binary");
        }
        catch (Exception ex)
        {
            exception = ex;
        }
        return new CompiledState(generatedState, exception);
    }

    private void WriteVisualStudioProjectFile(string grammarName, string lexerName, string parserName,
                                              bool useListener, bool useVisitor)
    {
        var projectFileST = new Template(visualStudioProjectContent);
        projectFileST.Add("runtimeSourcePath", runtimeSourcePath);
        projectFileST.Add("runtimeBinaryPath", runtimeBinaryPath);
        projectFileST.Add("grammarName", grammarName);
        projectFileST.Add("lexerName", lexerName);
        projectFileST.Add("parserName", parserName);
        projectFileST.Add("useListener", useListener);
        projectFileST.Add("useVisitor", useVisitor);
        FileUtils.WriteFile(GetTempDirPath(), "Test.vcxproj", projectFileST.Render());
    }

    ////@Override
    public override string GetRuntimeToolName()
    {
        return null;
    }

    ////@Override
    public override string GetExecFileName()
    {
        return Path.Combine(GetTempDirPath(), GetTestFileName() + "." + (RuntimeTestUtils.IsWindows() ? "exe" : "out")).ToString();
    }

    ////@Override
    public override Dictionary<string, string> GetExecEnvironment()
    {
        return environment;
    }
}

