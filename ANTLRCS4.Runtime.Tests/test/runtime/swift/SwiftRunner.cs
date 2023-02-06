/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using Antlr4.StringTemplate;
using org.antlr.v4.test.runtime.states;

namespace org.antlr.v4.test.runtime.swift;

public class SwiftRunner : RuntimeRunner
{
    ////@Override
    public override string GetLanguage() => "Swift";

    ////@Override
    public override string GetTestFileName() => "main";

    private static readonly string swiftRuntimePath;
    private static readonly string buildSuffix;
    private static readonly Dictionary<string, string> environment;

    private static readonly string includePath;
    private static readonly string libraryPath;

    static SwiftRunner()
    {
        swiftRuntimePath = GetRuntimePath("Swift");
        buildSuffix = RuntimeTestUtils.IsWindows() ? "x86_64-unknown-windows-msvc" : "";
        includePath = Path.Combine(swiftRuntimePath, ".build", buildSuffix, "release").ToString();
        environment = new();
        if (RuntimeTestUtils.IsWindows())
        {
            libraryPath = Path.Combine(includePath, "Antlr4.lib");
            String path = Environment.GetEnvironmentVariable("PATH");
            environment.Add("PATH", path == null ? includePath : path + ";" + includePath);
        }
        else
        {
            libraryPath = includePath;
        }
    }

    ////@Override
    public override string GetCompilerName()
    {
        return "swift";
    }

    ////@Override
    protected override void InitRuntime()
    {
        RunCommand(new String[] { GetCompilerPath(), "build", "-c", "release" }, swiftRuntimePath, "build Swift runtime");
    }

    ////@Override
    public override CompiledState Compile(RunOptions runOptions, GeneratedState generatedState)
    {
        Exception exception = null;
        try
        {
            var tempDirPath = GetTempDirPath();
            var tempDirFile = (tempDirPath);

            var ignoredFiles =
                new DirectoryInfo(tempDirFile).GetFiles("*.*").Where(f => f.Name.EndsWith(".swift")).Select(f => f.FullName).ToArray()
                ;// ;.listFiles(NoSwiftFileFilter.Instance);
                 //assert ignoredFiles != null;
            var excludedFiles = ignoredFiles.ToList();// Arrays.stream().map(File::getName).collect(Collectors.toList());

            var text = RuntimeTestUtils.GetTextFromResource("org/antlr/v4/test/runtime/helpers/Package.swift.stg");
            var outputFileST = new Template(text);
            outputFileST.Add("excludedFiles", excludedFiles);
            FileUtils.WriteFile(tempDirPath, "Package.swift", outputFileST.Render());

            var buildProjectArgs = new String[]{
                    GetCompilerPath(),
                    "build",
                    "-c",
                    "release",
                    "-Xswiftc",
                    "-I" + includePath,
                    "-Xlinker",
                    "-L" + includePath,
                    "-Xlinker",
                    "-lAntlr4",
                    "-Xlinker",
                    "-rpath",
                    "-Xlinker",
                    libraryPath
            };
            RunCommand(buildProjectArgs, tempDirPath);
        }
        catch (Exception e)
        {
            exception = e;
        }

        return new CompiledState(generatedState, exception);
    }

    //class NoSwiftFileFilter : FilenameFilter {
    //	public static readonly NoSwiftFileFilter Instance = new NoSwiftFileFilter();

    //	public bool accept(string dir, String name) {
    //		return !name.EndsWith(".swift");
    //	}
    //}

    ////@Override
    public override string GetRuntimeToolName()
    {
        return null;
    }

    ////@Override
    public override string GetExecFileName()
    {
        return Path.Combine(GetTempDirPath(),
                ".build",
                buildSuffix,
                "release",
                "Test" + (RuntimeTestUtils.IsWindows() ? ".exe" : "")).ToString();
    }

    ////@Override
    public override Dictionary<string, string> GetExecEnvironment()
    {
        return environment;
    }
}
