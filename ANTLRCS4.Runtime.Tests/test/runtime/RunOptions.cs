/*
 * Copyright (c) 2012-2022 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

namespace org.antlr.v4.test.runtime;

public class RunOptions {
	public readonly String grammarFileName;
	public readonly String grammarStr;
	public readonly String parserName;
	public readonly String lexerName;
	public readonly String grammarName;
	public readonly bool useListener;
	public readonly bool useVisitor;
	public readonly String startRuleName;
	public readonly String input;
	public readonly bool profile;
	public readonly bool showDiagnosticErrors;
	public readonly bool showDFA;
	public readonly Stage endStage;
	public readonly bool returnObject;
	public readonly String superClass;

	public RunOptions(String grammarFileName, String grammarStr, String parserName, String lexerName,
					  bool useListener, bool useVisitor, String startRuleName,
					  String input, bool profile, bool showDiagnosticErrors,
					  bool showDFA, Stage endStage, bool returnObject,
					  String language, String superClass) {
		this.grammarFileName = grammarFileName;
		this.grammarStr = grammarStr;
		this.parserName = parserName;
		this.lexerName = lexerName;
		String grammarName = null;
		bool isCombinedGrammar = lexerName != null && parserName != null || language.Equals("Go");
		if (isCombinedGrammar) {
			if (parserName != null) {
				grammarName = parserName.EndsWith("Parser")
					? parserName[..^"Parser".Length]
					: parserName;
			}
			else if (lexerName != null) {
				grammarName = lexerName.EndsWith("Lexer")
					? lexerName[..^"Lexer".Length]
					: lexerName;
			}
		}
		else {
			if (parserName != null) {
				grammarName = parserName;
			}
			else {
				grammarName = lexerName;
			}
		}
		this.grammarName = grammarName;
		this.useListener = useListener;
		this.useVisitor = useVisitor;
		this.startRuleName = startRuleName;
		this.input = input;
		this.profile = profile;
		this.showDiagnosticErrors = showDiagnosticErrors;
		this.showDFA = showDFA;
		this.endStage = endStage;
		this.returnObject = returnObject;
		this.superClass = superClass;
	}
}
