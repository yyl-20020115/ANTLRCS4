/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using Antlr4.StringTemplate;
using org.antlr.v4.test.runtime.states;

namespace org.antlr.v4.test.runtime.dart;

public class DartRunner : RuntimeRunner
{
    
    public override string GetLanguage() => "Dart";

    private static string cacheDartPackageConfig;

    
    protected override void InitRuntime()
    {
        string cachePath = GetCachePath();
        FileUtils.MakeDirectory(cachePath);

        var projectTemplate = new Template(RuntimeTestUtils.GetTextFromResource("org/antlr/v4/test/runtime/helpers/pubspec.yaml.stg"));
        projectTemplate.Add("runtimePath", GetRuntimePath());

        FileUtils.WriteFile(cachePath, "pubspec.yaml", projectTemplate.Render());

        RunCommand(new string[] { GetRuntimeToolPath(), "pub", "get" }, cachePath);

        cacheDartPackageConfig = FileUtils.ReadFile(
            Path.Combine(cachePath, ".dart_tool"), "package_config.json");
    }

    
    public override CompiledState Compile(RunOptions runOptions, GeneratedState generatedState)
    {
        string dartToolDirPath = Path.Combine(GetTempDirPath(), ".dart_tool");
        FileUtils.MakeDirectory(dartToolDirPath);
        FileUtils.WriteFile(dartToolDirPath, "package_config.json", cacheDartPackageConfig);

        return new CompiledState(generatedState, null);
    }
}
