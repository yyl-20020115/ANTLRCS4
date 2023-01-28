/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

namespace org.antlr.v4.test.runtime.swift;

public class SwiftRunner : RuntimeRunner {
	////@Override
	public String getLanguage() {
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
		buildSuffix = isWindows() ? "x86_64-unknown-windows-msvc" : "";
		includePath = Paths.get(swiftRuntimePath, ".build", buildSuffix, "release").ToString();
		environment = new HashMap<>();
		if (isWindows()) {
			libraryPath = Paths.get(includePath, "Antlr4.lib").ToString();
			String path = System.getenv("PATH");
			environment.put("PATH", path == null ? includePath : path + ";" + includePath);
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
			File tempDirFile = new File(tempDirPath);

			File[] ignoredFiles = tempDirFile.listFiles(NoSwiftFileFilter.Instance);
			assert ignoredFiles != null;
			List<String> excludedFiles = Arrays.stream(ignoredFiles).map(File::getName).collect(Collectors.toList());

			String text = getTextFromResource("org/antlr/v4/test/runtime/helpers/Package.swift.stg");
			ST outputFileST = new ST(text);
			outputFileST.add("excludedFiles", excludedFiles);
			writeFile(tempDirPath, "Package.swift", outputFileST.render());

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

		public bool accept(File dir, String name) {
			return !name.endsWith(".swift");
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
