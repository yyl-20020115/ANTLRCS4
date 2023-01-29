/*
 * Copyright (c) 2012-2022 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime.misc;
using org.antlr.v4.runtime.tree.pattern;
using org.antlr.v4.test.runtime;
using System.Text;

namespace org.antlr.v4.test.runtime;


public class RuntimeTestDescriptorParser {
	private static readonly HashSet<String> sections = new (Arrays.AsList(
			"notes", "type", "grammar", "slaveGrammar", "start", "input", "output", "errors", "flags", "skip"
	));

	/**  Read stuff like:
	 [grammar]
	 grammar T;
	 s @after {<DumpDFA()>}
	 : ID | ID {} ;
	 ID : 'a'..'z'+;
	 WS : (' '|'\t'|'\n')+ -> skip ;

	 [grammarName]
	 T

	 [start]
	 s

	 [input]
	 abc

	 [output]
	 Decision 0:
	 s0-ID->:s1^=>1

	 [errors]
	 """line 1:0 reportAttemptingFullContext d=0 (s), input='abc'
	 """

	 Some can be missing like [errors].

	 Get gr names automatically "lexer grammar Unicode;" "grammar T;" "parser grammar S;"

	 Also handle slave grammars:

	 [grammar]
	 grammar M;
	 import S,T;
	 s : a ;
	 B : 'b' ; // defines B from inherited token space
	 WS : (' '|'\n') -> skip ;

	 [slaveGrammar]
	 parser grammar T;
	 a : B {<writeln("\"T.a\"")>};<! hidden by S.a !>

	 [slaveGrammar]
	 parser grammar S;
	 a : b {<writeln("\"S.a\"")>};
	 b : B;
	 */
	public static RuntimeTestDescriptor parse(String name, String text, string uri) {
		String currentField = null;
		StringBuilder currentValue = new StringBuilder();

		List<Pair<String, String>> pairs = new ();
		String[] lines = text.Split("\r?\n");

		foreach (String line in lines) {
			bool newSection = false;
			String sectionName = null;
			if (line.StartsWith("[") && line.Length > 2) {
				sectionName = line.Substring(1, line.Length - 1 -1);
				newSection = sections.Contains(sectionName);
			}

			if (newSection) {
				if (currentField != null) {
					pairs.Add(new (currentField, currentValue.ToString()));
				}
				currentField = sectionName;
				currentValue.Length=0;
			}
			else {
				currentValue.Append(line);
				currentValue.Append('\n');
			}
		}
		pairs.Add(new (currentField, currentValue.ToString()));

		String notes = "";
		GrammarType testType = GrammarType.Lexer;
		String grammar = "";
		String grammarName = "";
		List<Pair<String, String>> slaveGrammars = new ();
		String startRule = "";
		String input = "";
		String output = "";
		String errors = "";
		bool showDFA = false;
		bool showDiagnosticErrors = false;
		String[] skipTargets = new String[0];
		foreach (Pair<String,String> p in pairs) {
			String section = p.a;
			String value = "";
			if ( p.b!=null ) {
				value = p.b.Trim();
			}
			if ( value.StartsWith("\"\"\"") ) {
				value = value.Replace("\"\"\"", "");
			}
			else if ( value.IndexOf('\n')>=0 ) {
				value = value + "\n"; // if multi line and not quoted, leave \n on end.
			}
			switch (section) {
				case "notes":
					notes = value;
					break;
				case "type":
					testType = Enum.TryParse<GrammarType>(value, out var ret)?ret:GrammarType.Lexer;
					break;
				case "grammar":
					grammarName = getGrammarName(value.Split("\n")[0]);
					grammar = value;
					break;
				case "slaveGrammar":
					String gname = getGrammarName(value.Split("\n")[0]);
					slaveGrammars.Add(new (gname, value));
                    startRule = value;
					break;
                case "start":
					startRule = value;
					break;
				case "input":
					input = value;
					break;
				case "output":
					output = value;
					break;
				case "errors":
					errors = value;
					break;
				case "flags":
					String[] flags = value.Split('\n');
					foreach (String f in flags) {
						switch (f) {
							case "showDFA":
								showDFA = true;
								break;
							case "showDiagnosticErrors":
								showDiagnosticErrors = true;
								break;
						}
					}
					break;
				case "skip":
					skipTargets = value.Split('\n');
					break;
				default:
					throw new RuntimeException("Unknown descriptor section ignored: "+section);
			}
		}
		return new RuntimeTestDescriptor(testType, name, notes, input, output, errors, startRule, grammarName, grammar,
				slaveGrammars, showDFA, showDiagnosticErrors, skipTargets, uri);
	}

	/** Get A, B, or C from:
	 * "lexer grammar A;" "grammar B;" "parser grammar C;"
	 */
	private static String getGrammarName(String grammarDeclLine) {
		int gi = grammarDeclLine.IndexOf("grammar ");
		if ( gi<0 ) {
			return "<unknown grammar name>";
		}
		gi += "grammar ".Length;
		int gsemi = grammarDeclLine.IndexOf(';');
		return grammarDeclLine.Substring(gi, gsemi-gi);
	}
}
