/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */
using org.antlr.v4.test.runtime.states;

namespace org.antlr.v4.test.runtime.csharp;

public class CSharpRunner : RuntimeRunner {
	////@Override
	public override String getLanguage() { return "CSharp"; }

	////@Override
	public String getTitleName() { return "C#"; }

	////@Override
	public String getExtension() { return "cs"; }

	////@Override
	public String getRuntimeToolName() { return "dotnet"; }

	////@Override
	public String getExecFileName() { return getTestFileName() + ".dll"; }

	private static readonly String testProjectFileName = "Antlr4.Test.csproj";
	private static readonly String cSharpAntlrRuntimeDllName =
			Path.Combine(getCachePath("CSharp"), "Antlr4.Runtime.Standard.dll").ToString();

	private static readonly String cSharpTestProjectContent;

	static CSharpRunner(){
		Template projectTemplate = new Template(RuntimeTestUtils.getTextFromResource("org/antlr/v4/test/runtime/helpers/Antlr4.Test.csproj.stg"));
		projectTemplate.add("runtimeLibraryPath", cSharpAntlrRuntimeDllName);
		cSharpTestProjectContent = projectTemplate.render();
	}

	////@Override
	protected void initRuntime()  {
		String cachePath = getCachePath();
		mkdir(cachePath);
		String projectPath = Path.Combine(getRuntimePath(), "src", "Antlr4.csproj").ToString();
		String[] args = new String[]{getRuntimeToolPath(), "build", projectPath, "-c", "Release", "-o", cachePath};
		runCommand(args, cachePath, "build " + getTitleName() + " ANTLR runtime");
	}

	////@Override
	public CompiledState compile(RunOptions runOptions, GeneratedState generatedState) {
		Exception exception = null;
		try {
			writeFile(getTempDirPath(), testProjectFileName, cSharpTestProjectContent);
			runCommand(new String[]{getRuntimeToolPath(), "build", testProjectFileName, "-c", "Release"}, getTempDirPath(),
					"build C# test binary");
		} catch (Exception e) {
			exception = e;
		}
		return new CompiledState(generatedState, exception);
	}
}
