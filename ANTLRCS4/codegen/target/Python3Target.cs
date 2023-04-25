/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

namespace org.antlr.v4.codegen.target;

public class Python3Target : Target
{
    protected static readonly HashSet<string> reservedWords = new() {
        "abs", "all", "and", "any", "apply", "as", "assert",
        "bin", "bool", "break", "buffer", "bytearray",
        "callable", "chr", "class", "classmethod", "coerce", "compile", "complex", "continue",
        "def", "del", "delattr", "dict", "dir", "divmod",
        "elif", "else", "enumerate", "eval", "execfile", "except",
        "file", "filter", "finally", "float", "for", "format", "from", "frozenset",
        "getattr", "global", "globals",
        "hasattr", "hash", "help", "hex",
        "id", "if", "import", "in", "input", "int", "intern", "is", "isinstance", "issubclass", "iter",
        "lambda", "len", "list", "locals",
        "map", "max", "min", "memoryview",
        "next", "nonlocal", "not",
        "object", "oct", "open", "or", "ord",
        "pass", "pow", "print", "property",
        "raise", "range", "raw_input", "reduce", "reload", "repr", "return", "reversed", "round",
        "set", "setattr", "slice", "sorted", "staticmethod", "str", "sum", "super",
        "try", "tuple", "type",
        "unichr", "unicode",
        "vars",
        "with", "while",
        "yield",
        "zip",
        "__import__",
        "True", "False", "None",

		// misc
		"rule", "parserRule"
    };

    protected static readonly Dictionary<char, string> targetCharValueEscape;
    static Python3Target()
    {
        // https://docs.python.org/3/reference/lexical_analysis.html#string-and-bytes-literals
        var map = new Dictionary<char, string>();
        AddEscapedChar(map, '\\');
        AddEscapedChar(map, '\'');
        AddEscapedChar(map, '\"');
        AddEscapedChar(map, (char)0x0007, 'a');
        AddEscapedChar(map, (char)0x0008, 'b');
        AddEscapedChar(map, '\f', 'f');
        AddEscapedChar(map, '\n', 'n');
        AddEscapedChar(map, '\r', 'r');
        AddEscapedChar(map, '\t', 't');
        AddEscapedChar(map, (char)0x000B, 'v');
        targetCharValueEscape = map;
    }

    public Python3Target(CodeGenerator gen) : base(gen)
    {
    }

    public override Dictionary<char, String> TargetCharValueEscape => targetCharValueEscape;

    public override HashSet<String> ReservedWords => reservedWords;

    public override bool WantsBaseListener => false;

    public override bool WantsBaseVisitor => false;

    public override bool SupportsOverloadedMethods => false;
}
