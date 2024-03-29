/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

namespace org.antlr.v4.codegen.target;

public class DartTarget : Target
{
    protected static readonly Dictionary<char, string> targetCharValueEscape;
    static DartTarget()
    {
        var map = new Dictionary<char, string>(defaultCharValueEscape);
        AddEscapedChar(map, '$');
        targetCharValueEscape = map;
    }

    protected static readonly HashSet<String> reservedWords = new() {
        "abstract", "dynamic", "implements", "show",
        "as", "else", "import", "static",
        "assert", "enum", "in", "super",
        "async", "export", "interface", "switch",
        "await", "extends", "is", "sync",
        "break", "external", "library", "this",
        "case", "factory", "mixin", "throw",
        "catch", "false", "new", "true",
        "class", "final", "null", "try",
        "const", "finally", "on", "typedef",
        "continue", "for", "operator", "var",
        "covariant", "Function", "part", "void",
        "default", "get", "rethrow", "while",
        "deferred", "hide", "return", "with",
        "do", "if", "set", "yield",

        "rule", "parserRule"
    };

    public DartTarget(CodeGenerator gen) : base(gen)
    {
    }

    public override Dictionary<char, string> TargetCharValueEscape => targetCharValueEscape;

    public override string GetTargetStringLiteralFromANTLRStringLiteral(CodeGenerator generator, string literal, bool addQuotes,
                                                               bool escapeSpecial) => base.GetTargetStringLiteralFromANTLRStringLiteral(generator, literal, addQuotes, escapeSpecial).Replace("$", "\\$");

    public override HashSet<string> ReservedWords => reservedWords;

    public override bool IsATNSerializedAsInts => true;

    protected override string EscapeChar(int v)
    => $"\\u{v:X}";
}
