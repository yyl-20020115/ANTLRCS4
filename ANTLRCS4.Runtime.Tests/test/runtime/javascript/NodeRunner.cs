/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */
using Antlr4.StringTemplate;
using org.antlr.v4.test.runtime.states;

namespace org.antlr.v4.test.runtime.javascript;

public class NodeRunner : RuntimeRunner {
	////@Override
	public override String getLanguage() {
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

	private static readonly String normalizedRuntimePath = getRuntimePath("JavaScript").Replace('\\', '/');
	private static readonly String newImportAntlrString =
			"import antlr4 from 'file://" + normalizedRuntimePath + "/src/antlr4/index.js'";

	////@Override
	protected CompiledState compile(RunOptions runOptions, GeneratedState generatedState) {
		List<GeneratedFile> generatedFiles = generatedState.generatedFiles;
		foreach (GeneratedFile generatedFile in generatedFiles) {
			try {
				FileUtils.replaceInFile(Path.Combine(getTempDirPath(), generatedFile.name),
						"import antlr4 from 'antlr4';",
						newImportAntlrString);
			} catch (IOException e) {
				return new CompiledState(generatedState, e);
			}
		}

		FileUtils.writeFile(getTempDirPath(), "package.json",
				RuntimeTestUtils.getTextFromResource("org/antlr/v4/test/runtime/helpers/package.json"));
		return new CompiledState(generatedState, null);
	}

	////@Override
	protected void addExtraRecognizerParameters(Template template) {
		template.Add("runtimePath", normalizedRuntimePath);
	}
}
