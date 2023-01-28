/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.test.runtime.states;

namespace org.antlr.v4.test.runtime.dart;

public class DartRunner : RuntimeRunner {
	////@Override
	public String getLanguage() {
		return "Dart";
	}

	private static String cacheDartPackageConfig;

	////@Override
	protected void initRuntime()  {
		String cachePath = getCachePath();
		mkdir(cachePath);

		ST projectTemplate = new ST(RuntimeTestUtils.getTextFromResource("org/antlr/v4/test/runtime/helpers/pubspec.yaml.stg"));
		projectTemplate.add("runtimePath", getRuntimePath());

		writeFile(cachePath, "pubspec.yaml", projectTemplate.render());

		runCommand(new String[]{getRuntimeToolPath(), "pub", "get"}, cachePath);

		cacheDartPackageConfig = readFile(cachePath + FileSeparator + ".dart_tool", "package_config.json");
	}

	////@Override
	protected CompiledState compile(RunOptions runOptions, GeneratedState generatedState) {
		String dartToolDirPath = new File(getTempDirPath(), ".dart_tool").getAbsolutePath();
		mkdir(dartToolDirPath);
		writeFile(dartToolDirPath, "package_config.json", cacheDartPackageConfig);

		return new CompiledState(generatedState, null);
	}
}
