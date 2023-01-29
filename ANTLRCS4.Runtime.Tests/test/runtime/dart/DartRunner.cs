/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using Antlr4.StringTemplate;
using org.antlr.v4.test.runtime.states;

namespace org.antlr.v4.test.runtime.dart;

public class DartRunner : RuntimeRunner {
	////@Override
	public override String getLanguage() {
		return "Dart";
	}

	private static String cacheDartPackageConfig;

	////@Override
	protected void initRuntime()  {
		String cachePath = getCachePath();
        FileUtils.mkdir(cachePath);

		Template projectTemplate = new Template(RuntimeTestUtils.getTextFromResource("org/antlr/v4/test/runtime/helpers/pubspec.yaml.stg"));
		projectTemplate.Add("runtimePath", getRuntimePath());

		FileUtils.writeFile(cachePath, "pubspec.yaml", projectTemplate.Render());

		runCommand(new String[]{getRuntimeToolPath(), "pub", "get"}, cachePath);

		cacheDartPackageConfig = FileUtils.readFile(
			Path.Combine(cachePath , ".dart_tool"), "package_config.json");
	}

	////@Override
	protected CompiledState compile(RunOptions runOptions, GeneratedState generatedState) {
		String dartToolDirPath = Path.Combine(getTempDirPath(), ".dart_tool");
		FileUtils.mkdir(dartToolDirPath);
		FileUtils.writeFile(dartToolDirPath, "package_config.json", cacheDartPackageConfig);

		return new CompiledState(generatedState, null);
	}
}
