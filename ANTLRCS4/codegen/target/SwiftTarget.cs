/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using Antlr4.StringTemplate;
using org.antlr.v4.tool;

namespace org.antlr.v4.codegen.target;

public class SwiftTarget : Target
{
    protected static readonly Dictionary<char, string> targetCharValueEscape;
    static SwiftTarget()
    {
        // https://docs.swift.org/swift-book/LanguageGuide/StringsAndCharacters.html
        var map = new Dictionary<char, string>();
        AddEscapedChar(map, '\0', '0');
        AddEscapedChar(map, '\\');
        AddEscapedChar(map, '\t', 't');
        AddEscapedChar(map, '\n', 'n');
        AddEscapedChar(map, '\r', 'r');
        AddEscapedChar(map, '\"');
        AddEscapedChar(map, '\'');
        targetCharValueEscape = map;
    }

    protected static readonly HashSet<string> reservedWords = new() {
            "associatedtype", "class", "deinit", "enum", "extension", "func", "import", "init", "inout", "internal",
            "let", "operator", "private", "protocol", "public", "static", "struct", "subscript", "typealias", "var",
            "break", "case", "continue", "default", "defer", "do", "else", "fallthrough", "for", "guard", "if",
            "in", "repeat", "return", "switch", "where", "while",
            "as", "catch", "dynamicType", "false", "is", "nil", "rethrows", "super", "self", "Self", "throw", "throws",
            "true", "try", "__COLUMN__", "__FILE__", "__FUNCTION__","__LINE__", "#column", "#file", "#function", "#line", "_" , "#available", "#else", "#elseif", "#endif", "#if", "#selector",
            "associativity", "convenience", "dynamic", "didSet", "final", "get", "infix", "indirect", "lazy",
            "left", "mutating", "none", "nonmutating", "optional", "override", "postfix", "precedence",
            "prefix", "Protocol", "required", "right", "set", "Type", "unowned", "weak", "willSet",

             "rule", "parserRule"
    };

    public SwiftTarget(CodeGenerator gen) : base(gen)
    {
    }

    public override Dictionary<char, String> TargetCharValueEscape => targetCharValueEscape;

    public override HashSet<string> ReservedWords => reservedWords;

    protected override string EscapeWord(string word) => "`" + word + "`";

    public override void GenFile(Grammar g, Template outputFileST, string fileName) => base.GenFile(g, outputFileST, fileName);


    public override bool IsATNSerializedAsInts => true;


    protected override string EscapeChar(int v) => $"\\u{v:X}";// String.format("\\u{%X}", v);
}
