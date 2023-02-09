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
using System.IO;
using System.Text;

namespace org.antlr.v4.test.runtime;
/** This class represents runtime tests for specified runtime.
 *  It pulls data from {@link RuntimeTestDescriptor} and uses junit to trigger tests.
 *  The only functionality needed to execute a test is defined in {@link RuntimeRunner}.
 *  All the various test rig classes derived from this one.
 *  E.g., see {@link JavaRuntimeTests}.
 */
public abstract class RuntimeTests
{
    protected abstract RuntimeRunner CreateRuntimeRunner();

    //private static readonly Dictionary<string, RuntimeTestDescriptor[]> testDescriptors = new ();
    private static readonly Dictionary<string, TemplateGroup> cachedTargetTemplates = new();
    private static readonly StringRenderer rendered = new();

    static RuntimeTests()
    {
        var descriptorsDir = (Path.Combine(RuntimeTestUtils.resourcePath.ToString(), "org/antlr/v4/test/runtime/descriptors").ToString());
        var directoryListing = new DirectoryInfo(descriptorsDir).GetFiles().Select(f => f.FullName);
        //assert directoryListing != null;
        foreach (var directory in directoryListing)
        {
            var groupName = directory;
            if (groupName.StartsWith("."))
            {
                continue; // Ignore service directories (like .DS_Store in Mac)
            }

            List<RuntimeTestDescriptor> descriptors = new();

            var descriptorFiles = new DirectoryInfo(directory).GetFiles().Select(f => f.FullName);
            //assert descriptorFiles != null;
            foreach (var descriptorFile in descriptorFiles)
            {
                var name = descriptorFile.Replace(".txt", "");
                if (name.StartsWith("."))
                {
                    continue;
                }

                string text;
                try
                {
                    text = File.ReadAllText(descriptorFile);
                }
                catch (IOException e)
                {
                    throw new RuntimeException(e.Message, e);
                }
                descriptors.Add(RuntimeTestDescriptorParser.Parse(name, text, descriptorFile));
            }

            //testDescriptors.put(groupName, descriptors.ToArray());
        }

        //foreach (var key in CustomDescriptors.descriptors.Keys) {
        //	RuntimeTestDescriptor[] descriptors = CustomDescriptors.descriptors.get(key);
        //	RuntimeTestDescriptor[] existedDescriptors = testDescriptors.putIfAbsent(key, descriptors);
        //	if (existedDescriptors != null) {
        //		testDescriptors.put(key, Stream.concat(Arrays.stream(existedDescriptors), Arrays.stream(descriptors))
        //				.toArray());
        //	}
        //}
    }
#if false
	//@TestFactory
	//@Execution(ExecutionMode.CONCURRENT)
	public List<DynamicNode> runtimeTests() {
		List<DynamicNode> result = new ();

		foreach (var group in testDescriptors.Keys) {
			List<DynamicNode> descriptorTests = new ();
			RuntimeTestDescriptor[] descriptors = testDescriptors.get(group);
			foreach (RuntimeTestDescriptor descriptor in descriptors) {
				descriptorTests.Add(dynamicTest(descriptor.name, descriptor.uri, () => {
					using (RuntimeRunner runner = createRuntimeRunner()) {
						var errorMessage = test(descriptor, runner);
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
    private static string Test(RuntimeTestDescriptor descriptor, RuntimeRunner runner)
    {
        var targetName = runner.GetLanguage();
        if (descriptor.Ignore(targetName))
        {
            Console.Out.WriteLine("Ignore " + descriptor);
            return null;
        }

        FileUtils.MakeDirectory(runner.GetTempDirPath());

        var grammarName = descriptor.grammarName;
        var grammar = PrepareGrammars(descriptor, runner);

        string lexerName, parserName;
        bool useListenerOrVisitor;
        string superClass;
        if (descriptor.testType == GrammarType.Parser || descriptor.testType == GrammarType.CompositeParser)
        {
            lexerName = grammarName + "Lexer";
            parserName = grammarName + "Parser";
            useListenerOrVisitor = true;
            if (targetName.Equals("Java"))
            {
                superClass = JavaRunner.runtimeTestParserName;
            }
            else
            {
                superClass = null;
            }
        }
        else
        {
            lexerName = grammarName;
            parserName = null;
            useListenerOrVisitor = false;
            if (targetName.Equals("Java"))
            {
                superClass = JavaRunner.runtimeTestLexerName;
            }
            else
            {
                superClass = null;
            }
        }

        var runOptions = new RunOptions(grammarName + ".g4",
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

        var result = runner.Run(runOptions);

        return AssertCorrectOutput(descriptor, targetName, result);
    }

    private static string PrepareGrammars(RuntimeTestDescriptor descriptor, RuntimeRunner runner)
    {
        var targetName = runner.GetLanguage();
        TemplateGroup targetTemplates;
        lock (cachedTargetTemplates)
        {
            if (!cachedTargetTemplates.TryGetValue(targetName, out targetTemplates))
            {
                var templates = File.ReadAllText("org/antlr/v4/test/runtime/templates/" + targetName + ".test.stg");
                //assert templates != null;
                targetTemplates = new TemplateGroupFile(templates, Encoding.UTF8, '<', '>');
                targetTemplates.RegisterRenderer(typeof(string), rendered);
                cachedTargetTemplates.Add(targetName, targetTemplates);
            }
        }

        // write out any slave grammars
        var slaveGrammars = descriptor.slaveGrammars;
        if (slaveGrammars != null)
        {
            foreach (var spair in slaveGrammars)
            {
                var gx = new TemplateGroup('<', '>');
                gx.RegisterRenderer(typeof(string), rendered);
                gx.ImportTemplates(targetTemplates);
                var _grammarST = new Template(gx, spair.b);
                FileUtils.WriteFile(runner.GetTempDirPath(), spair.a + ".g4", _grammarST.Render());
            }
        }

        var g = new TemplateGroup('<', '>');
        g.ImportTemplates(targetTemplates);
        g.RegisterRenderer(typeof(string), rendered);
        var grammarST = new Template(g, descriptor.grammar);
        return grammarST.Render();
    }

    private static string AssertCorrectOutput(RuntimeTestDescriptor descriptor, string targetName, State state)
    {
        ExecutedState executedState;
        if (state is ExecutedState state1)
        {
            executedState = state1;
            if (executedState.exception != null)
            {
                return state.GetErrorMessage();
            }
        }
        else
        {
            return state.GetErrorMessage();
        }

        var expectedOutput = descriptor.output;
        var expectedParseErrors = descriptor.errors;

        bool doesOutputEqualToExpected = executedState.output.Equals(expectedOutput);
        if (!doesOutputEqualToExpected || !executedState.errors.Equals(expectedParseErrors))
        {
            var message = doesOutputEqualToExpected
                ? "Parse output is as expected, but errors are not: "
                : "Parse output is incorrect: " +
                        "expectedOutput:<" + expectedOutput + ">; actualOutput:<" + executedState.output + ">; ";
            return "[" + targetName + ":" + descriptor.name + "] " +
                    message +
                    "expectedParseErrors:<" + expectedParseErrors + ">;" +
                    "actualParseErrors:<" + executedState.errors + ">.";
        }

        return null;
    }
}
