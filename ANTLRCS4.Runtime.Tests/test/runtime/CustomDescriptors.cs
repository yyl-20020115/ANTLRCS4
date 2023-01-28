/*
 * Copyright (c) 2012-2022 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using System.Text;

namespace org.antlr.v4.test.runtime;

public class CustomDescriptors {
	public static readonly Dictionary<String, RuntimeTestDescriptor[]> descriptors;
	private static readonly string uri;

	static CustomDescriptors(){
		uri = Path.Combine(RuntimeTestUtils.runtimeTestsuitePath.ToString(),
						"test", "org", "antlr", "v4", "test", "runtime", "CustomDescriptors.java").toUri();

		descriptors = new ();
		descriptors.Add("LexerExec",
				new RuntimeTestDescriptor[]{
						getLineSeparatorLfDescriptor(),
						getLineSeparatorCrLfDescriptor(),
						getLargeLexerDescriptor(),
						getAtnStatesSizeMoreThan65535Descriptor()
				});
		descriptors.Add("ParserExec",
				new RuntimeTestDescriptor[] {
						getMultiTokenAlternativeDescriptor()
				});
	}

	private static RuntimeTestDescriptor getLineSeparatorLfDescriptor() {
		return new RuntimeTestDescriptor(
				GrammarType.Lexer,
				"LineSeparatorLf",
				"",
				"1\n2\n3",
				"[@0,0:0='1',<1>,1:0]\n" +
						"[@1,1:1='\\n',<2>,1:1]\n" +
						"[@2,2:2='2',<1>,2:0]\n" +
						"[@3,3:3='\\n',<2>,2:1]\n" +
						"[@4,4:4='3',<1>,3:0]\n" +
						"[@5,5:4='<EOF>',<-1>,3:1]\n",
				"",
				null,
				"L",
				"lexer grammar L;\n" +
						"T: ~'\\n'+;\n" +
						"SEPARATOR: '\\n';",
				null, false, false, null, uri);
	}

	private static RuntimeTestDescriptor getLineSeparatorCrLfDescriptor() {
		return new RuntimeTestDescriptor(
				GrammarType.Lexer,
				"LineSeparatorCrLf",
				"",
				"1\r\n2\r\n3",
				"[@0,0:0='1',<1>,1:0]\n" +
						"[@1,1:2='\\r\\n',<2>,1:1]\n" +
						"[@2,3:3='2',<1>,2:0]\n" +
						"[@3,4:5='\\r\\n',<2>,2:1]\n" +
						"[@4,6:6='3',<1>,3:0]\n" +
						"[@5,7:6='<EOF>',<-1>,3:1]\n",
				"",
				"",
				"L",
				"lexer grammar L;\n" +
						"T: ~'\\r'+;\n" +
						"SEPARATOR: '\\r\\n';",
				null, false, false, null, uri);
	}

	private static RuntimeTestDescriptor getLargeLexerDescriptor() {
		 int tokensCount = 4000;
		 String grammarName = "L";

		StringBuilder grammar = new StringBuilder();
		grammar.Append("lexer grammar ").Append(grammarName).Append(";\n");
		grammar.Append("WS: [ \\t\\r\\n]+ -> skip;\n");
		for (int i = 0; i < tokensCount; i++) {
			grammar.Append("KW").Append(i).Append(" : 'KW' '").Append(i).Append("';\n");
		}

		return new RuntimeTestDescriptor(
				GrammarType.Lexer,
				"LargeLexer",
				"This is a regression test for antlr/antlr4#76 \"Serialized ATN strings\n" +
						"should be split when longer than 2^16 bytes (class file limitation)\"\n" +
						"https://github.com/antlr/antlr4/issues/76",
				"KW400",
				"[@0,0:4='KW400',<402>,1:0]\n" +
						"[@1,5:4='<EOF>',<-1>,1:5]\n",
				"",
				"",
				grammarName,
				grammar.ToString(),
				null, false, false, null, uri);
	}

	private static RuntimeTestDescriptor getAtnStatesSizeMoreThan65535Descriptor() {
		// I tried playing around with different sizes, and I think 1002 works for Go but 1003 does not;
		// the executing lexer gets a token syntax error for T208 or something like that
		int tokensCount = 1024;
		String suffix = String.Join("", Collections.nCopies(70, "_"));

		String grammarName = "L";
		StringBuilder grammar = new StringBuilder();
		grammar.Append("lexer grammar ").Append(grammarName).Append(";\n");
		grammar.Append('\n');
		StringBuilder input = new StringBuilder();
		StringBuilder output = new StringBuilder();
		int startOffset;
		int stopOffset = -2;
		for (int i = 0; i < tokensCount; i++) {
			String ruleName = String.format("T_%06d", i);
			String value = ruleName+suffix;
			grammar.Append(ruleName).Append(": '").Append(value).Append("';\n");
			input.Append(value).Append('\n');

			startOffset = stopOffset + 2;
			stopOffset += value.Length + 1;

			output.Append("[@").Append(i).Append(',').Append(startOffset).Append(':').Append(stopOffset)
					.Append("='").Append(value).Append("',<").Append(i + 1).Append(">,").Append(i + 1)
					.Append(":0]\n");
		}

		grammar.Append("\n");
		grammar.Append("WS: [ \\t\\r\\n]+ -> skip;\n");

		startOffset = stopOffset + 2;
		stopOffset = startOffset - 1;
		output.Append("[@").Append(tokensCount).Append(',').Append(startOffset).Append(':').Append(stopOffset)
				.Append("='<EOF>',<-1>,").Append(tokensCount + 1).Append(":0]\n");

		return new RuntimeTestDescriptor(
				GrammarType.Lexer,
				"AtnStatesSizeMoreThan65535",
				"Regression for https://github.com/antlr/antlr4/issues/1863",
				input.ToString(),
				output.ToString(),
				"",
				"",
				grammarName,
				grammar.ToString(),
				null, false, false,
				new String[] {"CSharp", "Python2", "Python3", "Go", "PHP", "Swift", "JavaScript", "Dart"}, uri);
	}

	private static RuntimeTestDescriptor getMultiTokenAlternativeDescriptor() {
		 int tokensCount = 64;

		StringBuilder rule = new StringBuilder("r1: ");
		StringBuilder tokens = new StringBuilder();
		StringBuilder input = new StringBuilder();
		StringBuilder output = new StringBuilder();

		for (int i = 0; i < tokensCount; i++) {
			String _currentToken = "T" + i;
			rule.Append(_currentToken);
			if (i < tokensCount - 1) {
				rule.Append(" | ");
			} else {
				rule.Append(";");
			}
			tokens.Append(_currentToken).Append(": '").Append(_currentToken).Append("';\n");
			input.Append(_currentToken).Append(" ");
			output.Append(_currentToken);
		}
		String currentToken = "T" + tokensCount;
		tokens.Append(currentToken).Append(": '").Append(currentToken).Append("';\n");
		input.Append(currentToken).Append(" ");
		output.Append(currentToken);

		String grammar = "grammar P;\n" +
				"r: (r1 | T" + tokensCount + ")+ EOF {<writeln(\"$text\")>};\n" +
				rule + "\n" +
				tokens + "\n" +
				"WS: [ ]+ -> skip;";

		return new RuntimeTestDescriptor(
				GrammarType.Parser,
				"MultiTokenAlternative",
				"https://github.com/antlr/antlr4/issues/3698, https://github.com/antlr/antlr4/issues/3703",
				input.ToString(),
				output + "\n",
				"",
				"r",
				"P",
				grammar,
				null, false, false, null, uri);
	}
}
