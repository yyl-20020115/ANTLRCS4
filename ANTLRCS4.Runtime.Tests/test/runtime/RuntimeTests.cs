/*
 * Copyright (c) 2012-2022 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using Antlr4.StringTemplate;
using org.antlr.v4.runtime.misc;
using org.antlr.v4.runtime.tree.pattern;
using org.antlr.v4.test.runtime.java;
using org.antlr.v4.test.runtime.states;
using System.Text;

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
		string descriptorsDir = (Path.Combine(RuntimeTestUtils.resourcePath.ToString(), "org/antlr/v4/test/runtime/descriptors").ToString());
        string[] directoryListing = descriptorsDir.listFiles();
		//assert directoryListing != null;
		foreach (string directory in directoryListing) {
			String groupName = directory;
			if (groupName.StartsWith(".")) {
				continue; // Ignore service directories (like .DS_Store in Mac)
			}

			List<RuntimeTestDescriptor> descriptors = new ();

            string[] descriptorFiles = directory.listFiles();
			//assert descriptorFiles != null;
			foreach (string descriptorFile in descriptorFiles) {
				String name = descriptorFile.Replace(".txt", "");
				if (name.StartsWith(".")) {
					continue;
				}

				String text;
				try {
					text = File.ReadAllText(descriptorFile);
				} catch (IOException e) {
					throw new RuntimeException(e.Message, e);
				}
				descriptors.Add(RuntimeTestDescriptorParser.parse(name, text, descriptorFile));
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
#if false
	//@TestFactory
	//@Execution(ExecutionMode.CONCURRENT)
	public List<DynamicNode> runtimeTests() {
		List<DynamicNode> result = new ();

		foreach (String group in testDescriptors.Keys) {
			List<DynamicNode> descriptorTests = new ();
			RuntimeTestDescriptor[] descriptors = testDescriptors.get(group);
			foreach (RuntimeTestDescriptor descriptor in descriptors) {
				descriptorTests.Add(dynamicTest(descriptor.name, descriptor.uri, () => {
					using (RuntimeRunner runner = createRuntimeRunner()) {
						String errorMessage = test(descriptor, runner);
						if (errorMessage != null) {
							runner.setSaveTestDir(true);
							fail(joinLines("Test: " + descriptor.name + "; " + errorMessage, "Test directory: " + runner.getTempDirPath()));
						}
					}
				}));
			}

			string descriptorGroupPath = Path.Combine(RuntimeTestUtils.resourcePath, "descriptors", group);
			result.Add(dynamicContainer(group, descriptorGroupPath, Arrays.stream(descriptorTests.ToArray())));
		}

		return result;
	}
#endif
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
			if (!cachedTargetTemplates.TryGetValue(targetName,out targetTemplates)) {
				string templates = .getResource("org/antlr/v4/test/runtime/templates/" + targetName + ".test.stg");
				//assert templates != null;
				targetTemplates = new TemplateGroupFile(templates, Encoding.UTF8, '<', '>');
				targetTemplates.RegisterRenderer(typeof(String), rendered);
				cachedTargetTemplates.Add(targetName, targetTemplates);
			}
		}

		// write out any slave grammars
		List<Pair<String, String>> slaveGrammars = descriptor.slaveGrammars;
		if (slaveGrammars != null) {
			foreach (Pair<String, String> spair in slaveGrammars) {
				TemplateGroup gx = new TemplateGroup('<', '>');
				gx.RegisterRenderer(typeof(String), rendered);
				gx.ImportTemplates(targetTemplates);
				Template _grammarST = new Template(gx, spair.b);
				FileUtils.writeFile(runner.getTempDirPath(), spair.a + ".g4", _grammarST.Render());
			}
		}

        TemplateGroup g = new TemplateGroup('<', '>');
		g.ImportTemplates(targetTemplates);
		g.RegisterRenderer(typeof(String), rendered);
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
