/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using Antlr4.StringTemplate;
using org.antlr.v4.test.runtime.states;

namespace org.antlr.v4.test.runtime.swift;

public class SwiftRunner : RuntimeRunner {
	////@Override
	public override String getLanguage() {
		return "Swift";
	}

	////@Override
	public String getTestFileName() {
		return "main";
	}

	private static readonly String swiftRuntimePath;
	private static readonly String buildSuffix;
	private static readonly Dictionary<String, String> environment;

	private static readonly String includePath;
	private static readonly String libraryPath;

	static SwiftRunner(){
		swiftRuntimePath = getRuntimePath("Swift");
		buildSuffix = RuntimeTestUtils.IsWindows() ? "x86_64-unknown-windows-msvc" : "";
		includePath = Path.Combine(swiftRuntimePath, ".build", buildSuffix, "release").ToString();
		environment = new ();
		if (RuntimeTestUtils.IsWindows()) {
			libraryPath = Path.Combine(includePath, "Antlr4.lib");
			String path = Environment.GetEnvironmentVariable("PATH");
			environment.Add("PATH", path == null ? includePath : path + ";" + includePath);
		}
		else {
			libraryPath = includePath;
		}
	}

	////@Override
	protected String getCompilerName() {
		return "swift";
	}

	////@Override
	protected void initRuntime()  {
		runCommand(new String[] {getCompilerPath(), "build", "-c", "release"}, swiftRuntimePath, "build Swift runtime");
	}

	////@Override
	protected CompiledState compile(RunOptions runOptions, GeneratedState generatedState) {
		Exception exception = null;
		try {
			String tempDirPath = getTempDirPath();
			string tempDirFile = (tempDirPath);

			string[] ignoredFiles =
				new DirectoryInfo(tempDirFile).GetFiles("*.*").Where(f => f.Name.EndsWith(".swift")).Select(f => f.FullName).ToArray()
				;// ;.listFiles(NoSwiftFileFilter.Instance);
			//assert ignoredFiles != null;
			List<String> excludedFiles = ignoredFiles.ToList();// Arrays.stream().map(File::getName).collect(Collectors.toList());

			String text = getTextFromResource("org/antlr/v4/test/runtime/helpers/Package.swift.stg");
			Template outputFileST = new Template(text);
			outputFileST.Add("excludedFiles", excludedFiles);
			FileUtils.writeFile(tempDirPath, "Package.swift", outputFileST.Render());

			String[] buildProjectArgs = new String[]{
					getCompilerPath(),
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
			runCommand(buildProjectArgs, tempDirPath);
		} catch (Exception e) {
			exception = e;
		}

		return new CompiledState(generatedState, exception);
	}

	class NoSwiftFileFilter : FilenameFilter {
		public static readonly NoSwiftFileFilter Instance = new NoSwiftFileFilter();

		public bool accept(string dir, String name) {
			return !name.EndsWith(".swift");
		}
	}

	////@Override
	public String getRuntimeToolName() {
		return null;
	}

	////@Override
	public String getExecFileName() {
		return Paths.get(getTempDirPath(),
				".build",
				buildSuffix,
				"release",
				"Test" + (isWindows() ? ".exe" : "")).ToString();
	}

	////@Override
	public Dictionary<String, String> getExecEnvironment() {
		return environment;
	}
}

internal class FilenameFilter
{
}