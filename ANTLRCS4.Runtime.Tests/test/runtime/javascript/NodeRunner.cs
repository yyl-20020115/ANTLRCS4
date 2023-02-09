/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */
using Antlr4.StringTemplate;
using org.antlr.v4.test.runtime.states;

namespace org.antlr.v4.test.runtime.javascript;

public class NodeRunner : RuntimeRunner
{
    
    public override string GetLanguage() => "JavaScript";

    
    public override string GetExtension() => "js";

    
    public override string GetBaseListenerSuffix() => null;

    
    public override string GetBaseVisitorSuffix() => null;

    
    public override string GetRuntimeToolName() => "node";

    private static readonly string normalizedRuntimePath = GetRuntimePath("JavaScript").Replace('\\', '/');
    private static readonly string newImportAntlrString = "import antlr4 from 'file://" + normalizedRuntimePath + "/src/antlr4/index.js'";

    
    public override CompiledState Compile(RunOptions runOptions, GeneratedState generatedState)
    {
        var generatedFiles = generatedState.generatedFiles;
        foreach (var generatedFile in generatedFiles)
        {
            try
            {
                FileUtils.ReplaceInFile(Path.Combine(GetTempDirPath(), generatedFile.name),
                        "import antlr4 from 'antlr4';",
                        newImportAntlrString);
            }
            catch (IOException e)
            {
                return new CompiledState(generatedState, e);
            }
        }

        FileUtils.WriteFile(GetTempDirPath(), "package.json",
                RuntimeTestUtils.GetTextFromResource("org/antlr/v4/test/runtime/helpers/package.json"));
        return new CompiledState(generatedState, null);
    }

    
    protected override void AddExtraRecognizerParameters(Template template)
    {
        template.Add("runtimePath", normalizedRuntimePath);
    }
}
