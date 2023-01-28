/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */
using org.antlr.v4.test.runtime.states;

namespace org.antlr.v4.test.runtime.javascript;

public class NodeRunner : RuntimeRunner {
	////@Override
	public String getLanguage() {
		return "JavaScript";
	}

	////@Override
	public String getExtension() { return "js"; }

	////@Override
	public String getBaseListenerSuffix() { return null; }

	////@Override
	public String getBaseVisitorSuffix() { return null; }

	////@Override
	public String getRuntimeToolName() { return "node"; }

	private static readonly String normalizedRuntimePath = getRuntimePath("JavaScript").replace('\\', '/');
	private static readonly String newImportAntlrString =
			"import antlr4 from 'file://" + normalizedRuntimePath + "/src/antlr4/index.js'";

	////@Override
	protected CompiledState compile(RunOptions runOptions, GeneratedState generatedState) {
		List<GeneratedFile> generatedFiles = generatedState.generatedFiles;
		for (GeneratedFile generatedFile : generatedFiles) {
			try {
				FileUtils.replaceInFile(Paths.get(getTempDirPath(), generatedFile.name),
						"import antlr4 from 'antlr4';",
						newImportAntlrString);
			} catch (IOException e) {
				return new CompiledState(generatedState, e);
			}
		}

		writeFile(getTempDirPath(), "package.json",
				RuntimeTestUtils.getTextFromResource("org/antlr/v4/test/runtime/helpers/package.json"));
		return new CompiledState(generatedState, null);
	}

	////@Override
	protected void addExtraRecognizerParameters(ST template) {
		template.add("runtimePath", normalizedRuntimePath);
	}
}
