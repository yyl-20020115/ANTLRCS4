/*
 * Copyright (c) 2012-2022 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using Antlr4.StringTemplate;
using org.antlr.v4.runtime.misc;
using org.antlr.v4.test.runtime.java;
using org.antlr.v4.test.runtime.states;

namespace org.antlr.v4.test.runtime;
/** This class represents runtime tests for specified runtime.
 *  It pulls data from {@link RuntimeTestDescriptor} and uses junit to trigger tests.
 *  The only functionality needed to execute a test is defined in {@link RuntimeRunner}.
 *  All the various test rig classes derived from this one.
 *  E.g., see {@link JavaRuntimeTests}.
 */
public abstract class RuntimeTests {
	protected abstract RuntimeRunner createRuntimeRunner();

	private static readonly Dictionary<String, RuntimeTestDescriptor[]> testDescriptors = new ();
	private static readonly Dictionary<String, TemplateGroup> cachedTargetTemplates = new ();
	private static readonly StringRenderer rendered = new StringRenderer();

	static RuntimeTests(){
		string descriptorsDir = (Paths.get(RuntimeTestUtils.resourcePath.ToString(), "org/antlr/v4/test/runtime/descriptors").ToString());
        string[] directoryListing = descriptorsDir.listFiles();
		//assert directoryListing != null;
		for (string directory in directoryListing) {
			String groupName = directory.getName();
			if (groupName.startsWith(".")) {
				continue; // Ignore service directories (like .DS_Store in Mac)
			}

			List<RuntimeTestDescriptor> descriptors = new ArrayList<>();

            string[] descriptorFiles = directory.listFiles();
			//assert descriptorFiles != null;
			for (string descriptorFile : descriptorFiles) {
				String name = descriptorFile.getName().replace(".txt", "");
				if (name.startsWith(".")) {
					continue;
				}

				String text;
				try {
					text = new String(Files.readAllBytes(descriptorFile.toPath()));
				} catch (IOException e) {
					throw new RuntimeException(e);
				}
				descriptors.add(RuntimeTestDescriptorParser.parse(name, text, descriptorFile.toURI()));
			}

			testDescriptors.put(groupName, descriptors.ToArray());
		}

		foreach (String key in CustomDescriptors.descriptors.Keys) {
			RuntimeTestDescriptor[] descriptors = CustomDescriptors.descriptors.get(key);
			RuntimeTestDescriptor[] existedDescriptors = testDescriptors.putIfAbsent(key, descriptors);
			if (existedDescriptors != null) {
				testDescriptors.put(key, Stream.concat(Arrays.stream(existedDescriptors), Arrays.stream(descriptors))
						.toArray());
			}
		}
	}

	//@TestFactory
	//@Execution(ExecutionMode.CONCURRENT)
	public List<DynamicNode> runtimeTests() {
		List<DynamicNode> result = new ArrayList<>();

		foreach (String group in testDescriptors.Keys) {
			List<DynamicNode> descriptorTests = new ();
			RuntimeTestDescriptor[] descriptors = testDescriptors.get(group);
			foreach (RuntimeTestDescriptor descriptor in descriptors) {
				descriptorTests.add(dynamicTest(descriptor.name, descriptor.uri, () => {
					using (RuntimeRunner runner = createRuntimeRunner()) {
						String errorMessage = test(descriptor, runner);
						if (errorMessage != null) {
							runner.setSaveTestDir(true);
							fail(joinLines("Test: " + descriptor.name + "; " + errorMessage, "Test directory: " + runner.getTempDirPath()));
						}
					}
				}));
			}

			string descriptorGroupPath = Paths.get(RuntimeTestUtils.resourcePath.ToString(), "descriptors", group);
			result.add(dynamicContainer(group, descriptorGroupPath, Arrays.stream(descriptorTests.toArray(new DynamicNode[0]))));
		}

		return result;
	}

	private static String test(RuntimeTestDescriptor descriptor, RuntimeRunner runner) {
		String targetName = runner.getLanguage();
		if (descriptor.ignore(targetName)) {
			Console.Out.WriteLine("Ignore " + descriptor);
			return null;
		}

		FileUtils.mkdir(runner.getTempDirPath());

		String grammarName = descriptor.grammarName;
		String grammar = prepareGrammars(descriptor, runner);

		String lexerName, parserName;
		bool useListenerOrVisitor;
		String superClass;
		if (descriptor.testType == GrammarType.Parser || descriptor.testType == GrammarType.CompositeParser) {
			lexerName = grammarName + "Lexer";
			parserName = grammarName + "Parser";
			useListenerOrVisitor = true;
			if (targetName.Equals("Java")) {
				superClass = JavaRunner.runtimeTestParserName;
			}
			else {
				superClass = null;
			}
		}
		else {
			lexerName = grammarName;
			parserName = null;
			useListenerOrVisitor = false;
			if (targetName.Equals("Java")) {
				superClass = JavaRunner.runtimeTestLexerName;
			}
			else {
				superClass = null;
			}
		}

		RunOptions runOptions = new RunOptions(grammarName + ".g4",
				grammar,
				parserName,
				lexerName,
				useListenerOrVisitor,
				useListenerOrVisitor,
				descriptor.startRule,
				descriptor.input,
				false,
				descriptor.showDiagnosticErrors,
				descriptor.showDFA,
				Stage.Execute,
				false,
				targetName,
				superClass
		);

		State result = runner.run(runOptions);

		return assertCorrectOutput(descriptor, targetName, result);
	}

	private static String prepareGrammars(RuntimeTestDescriptor descriptor, RuntimeRunner runner) {
		String targetName = runner.getLanguage();

		TemplateGroup targetTemplates;
		lock (cachedTargetTemplates) {
			targetTemplates = cachedTargetTemplates.get(targetName);
			if (targetTemplates == null) {
				ClassLoader classLoader = RuntimeTests.getClassLoader();
				URL templates = classLoader.getResource("org/antlr/v4/test/runtime/templates/" + targetName + ".test.stg");
				//assert templates != null;
				targetTemplates = new TemplateGroupFile(templates, "UTF-8", '<', '>');
				targetTemplates.RegisterRenderer(String, rendered);
				cachedTargetTemplates.put(targetName, targetTemplates);
			}
		}

		// write out any slave grammars
		List<Pair<String, String>> slaveGrammars = descriptor.slaveGrammars;
		if (slaveGrammars != null) {
			for (Pair<String, String> spair : slaveGrammars) {
				TemplateGroup g = new TemplateGroup('<', '>');
				g.registerRenderer(String, rendered);
				g.importTemplates(targetTemplates);
				Template grammarST = new Template(g, spair.b);
				writeFile(runner.getTempDirPath(), spair.a + ".g4", grammarST.Render());
			}
		}

        TemplateGroup g = new TemplateGroup('<', '>');
		g.importTemplates(targetTemplates);
		g.registerRenderer(String, rendered);
		Template grammarST = new Template(g, descriptor.grammar);
		return grammarST.Render();
	}

	private static String assertCorrectOutput(RuntimeTestDescriptor descriptor, String targetName, State state) {
		ExecutedState executedState;
		if (state is ExecutedState) {
			executedState = (ExecutedState)state;
			if (executedState.exception != null) {
				return state.getErrorMessage();
			}
		}
		else {
			return state.getErrorMessage();
		}

		String expectedOutput = descriptor.output;
		String expectedParseErrors = descriptor.errors;

		bool doesOutputEqualToExpected = executedState.output.Equals(expectedOutput);
		if (!doesOutputEqualToExpected || !executedState.errors.Equals(expectedParseErrors)) {
			String message;
			if (doesOutputEqualToExpected) {
				message = "Parse output is as expected, but errors are not: ";
			}
			else {
				message = "Parse output is incorrect: " +
						"expectedOutput:<" + expectedOutput + ">; actualOutput:<" + executedState.output + ">; ";
			}

			return "[" + targetName + ":" + descriptor.name + "] " +
					message +
					"expectedParseErrors:<" + expectedParseErrors + ">;" +
					"actualParseErrors:<" + executedState.errors + ">.";
		}

		return null;
	}
}
