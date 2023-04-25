/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

namespace org.antlr.v4.codegen.target;

public class PHPTarget : Target {
	protected static readonly HashSet<String> reservedWords = new() {
		"abstract", "and", "array", "as",
		"break",
		"callable", "case", "catch", "class", "clone", "const", "continue",
		"declare", "default", "die", "do",
		"echo", "else", "elseif", "empty", "enddeclare", "endfor", "endforeach",
		"endif", "endswitch", "endwhile", "eval", "exit", "extends",
		"final", "finally", "for", "foreach", "function",
		"global", "goto",
		"if", "implements", "include", "include_once", "instanceof", "insteadof", "interface", "isset",
		"list",
		"namespace", "new",
		"or",
		"print", "private", "protected", "public",
		"require", "require_once", "return",
		"static", "switch",
		"throw", "trait", "try",
		"unset", "use",
		"var",
		"while",
		"xor",
		"yield",
		"__halt_compiler", "__CLASS__", "__DIR__", "__FILE__", "__FUNCTION__",
		"__LINE__", "__METHOD__", "__NAMESPACE__", "__TRAIT__",

		// misc
		"rule", "parserRule"
	};

	protected static readonly Dictionary<char, String> targetCharValueEscape;
	static PHPTarget(){
		// https://www.php.net/manual/en/language.types.string.php
		var map = new Dictionary<char, String>();
		AddEscapedChar(map, '\n', 'n');
		AddEscapedChar(map, '\r', 'r');
		AddEscapedChar(map, '\t', 't');
		AddEscapedChar(map, (char)0x000B, 'v');
		AddEscapedChar(map, (char)0x001B, 'e');
		AddEscapedChar(map, '\f', 'f');
		AddEscapedChar(map, '\\');
		AddEscapedChar(map, '$');
		AddEscapedChar(map, '\"');
		targetCharValueEscape = map;
	}

	public PHPTarget(CodeGenerator gen):base(gen) {
	}


    public override Dictionary<char, String> TargetCharValueEscape => targetCharValueEscape;

    public override HashSet<String> ReservedWords => reservedWords;

    public override bool SupportsOverloadedMethods => false;

    public override String GetTargetStringLiteralFromANTLRStringLiteral(CodeGenerator generator, String literal, bool addQuotes,
															   bool escapeSpecial) {
		String targetStringLiteral = base.GetTargetStringLiteralFromANTLRStringLiteral(generator, literal, addQuotes, escapeSpecial);
		targetStringLiteral = targetStringLiteral.Replace("$", "\\$");
		return targetStringLiteral;
	}

    public override bool IsATNSerializedAsInts => true;

    protected override String EscapeChar(int v) {
		return $"\\u{v:X}";
	}
}
