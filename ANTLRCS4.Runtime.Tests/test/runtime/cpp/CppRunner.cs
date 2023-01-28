/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */
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
public class CppRunner : RuntimeRunner {
	////@Override
	public override String getLanguage() {
		return "Cpp";
	}

	////@Override
	protected override String getTitleName() { return "C++"; }

	private static readonly String runtimeSourcePath;
	private static readonly String runtimeBinaryPath;
	private static readonly String runtimeLibraryFileName;
	private static String compilerName;
	private static readonly String visualStudioProjectContent;
	private static readonly Dictionary<String, String> environment;

	static CppRunner() {
		String runtimePath = getRuntimePath("Cpp");
		runtimeSourcePath = Path.Combine(runtimePath, "runtime", "src").ToString();

		environment = new ();
		if (IsWindows()) {
			runtimeBinaryPath = Paths.get(runtimePath, "runtime", "bin", "vs-2022", "x64", "Release DLL").ToString();
			runtimeLibraryFileName = Paths.get(runtimeBinaryPath, "antlr4-runtime.dll").ToString();
			String path = System.getenv("PATH");
			environment.put("PATH", path == null ? runtimeBinaryPath : path + ";" + runtimeBinaryPath);
		}
		else {
			runtimeBinaryPath = Paths.get(runtimePath, "dist").ToString();
			runtimeLibraryFileName = Paths.get(runtimeBinaryPath,
					"libantlr4-runtime." + (getOS() == OSType.Mac ? "dylib" : "so")).ToString();
			environment.put("LD_PRELOAD", runtimeLibraryFileName);
		}

		if (isWindows()) {
			visualStudioProjectContent = RuntimeTestUtils.getTextFromResource("org/antlr/v4/test/runtime/helpers/Test.vcxproj.stg");
		} else {
			visualStudioProjectContent = null;
		}
	}

	////@Override
	protected String getCompilerName() {
		if (compilerName == null) {
			if (isWindows()) {
				compilerName = "MSBuild";
			}
			else {
				compilerName = "clang++";
			}
		}

		return compilerName;
	}

	////@Override
	protected void initRuntime()  {
		String runtimePath = getRuntimePath();

		if (isWindows()) {
			String[] command = {
				getCompilerPath(), "antlr4cpp-vs2022.vcxproj", "/p:configuration=Release DLL", "/p:platform=x64"
			};

			runCommand(command, runtimePath + "\\runtime","build c++ ANTLR runtime using MSBuild");
		}
		else {
			String[] command = {"cmake", ".", "-DCMAKE_BUILD_TYPE=Release"};
			runCommand(command, runtimePath, "run cmake on antlr c++ runtime");

			command = new String[] {"make", "-j", Integer.toString(Runtime.getRuntime().availableProcessors())};
			runCommand(command, runtimePath, "run make on antlr c++ runtime");
		}
	}

	////@Override
	protected CompiledState compile(RunOptions runOptions, GeneratedState generatedState) {
		if (isWindows()) {
			writeVisualStudioProjectFile(runOptions.grammarName, runOptions.lexerName, runOptions.parserName,
					runOptions.useListener, runOptions.useVisitor);
		}

		Exception exception = null;
		try {
			if (!isWindows()) {
				String[] linkCommand = new String[]{"ln", "-s", runtimeLibraryFileName};
				runCommand(linkCommand, getTempDirPath(), "sym link C++ runtime");
			}

			List<String> buildCommand = new ();
			buildCommand.Add(getCompilerPath());
			if (isWindows()) {
				buildCommand.Add(getTestFileName() + ".vcxproj");
				buildCommand.Add("/p:configuration=Release");
				buildCommand.Add("/p:platform=x64");
			}
			else {
				buildCommand.Add("-std=c++17");
				buildCommand.Add("-I");
				buildCommand.Add(runtimeSourcePath);
				buildCommand.Add("-L.");
				buildCommand.Add("-lantlr4-runtime");
				buildCommand.Add("-pthread");
				buildCommand.Add("-o");
				buildCommand.Add(getTestFileName() + ".out");
				buildCommand.Add(getTestFileWithExt());
				buildCommand.AddRange(generatedState.generatedFiles.stream().map(file => file.name).collect(Collectors.toList()));
			}

			runCommand(buildCommand.ToArray(), getTempDirPath(), "build test c++ binary");
		}
		catch (Exception ex) {
			exception = ex;
		}
		return new CompiledState(generatedState, exception);
	}

	private void writeVisualStudioProjectFile(String grammarName, String lexerName, String parserName,
											  bool useListener, bool useVisitor) {
		ST projectFileST = new ST(visualStudioProjectContent);
		projectFileST.add("runtimeSourcePath", runtimeSourcePath);
		projectFileST.add("runtimeBinaryPath", runtimeBinaryPath);
		projectFileST.add("grammarName", grammarName);
		projectFileST.add("lexerName", lexerName);
		projectFileST.add("parserName", parserName);
		projectFileST.add("useListener", useListener);
		projectFileST.add("useVisitor", useVisitor);
		writeFile(getTempDirPath(), "Test.vcxproj", projectFileST.render());
	}

	////@Override
	protected override String getRuntimeToolName() {
		return null;
	}

	////@Override
	protected override String getExecFileName() {
		return Paths.get(getTempDirPath(), getTestFileName() + "." + (isWindows() ? "exe" : "out")).ToString();
	}

	////@Override
	protected object Dictionary<String, String> getExecEnvironment() {
		return environment;
	}
}

