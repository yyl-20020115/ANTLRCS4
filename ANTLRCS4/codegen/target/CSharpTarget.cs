/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */
namespace org.antlr.v4.codegen.target;


public class CSharpTarget : Target
{
    protected static readonly HashSet<string> reservedWords = new() {
        "abstract",
        "as",
        "base",
        "bool",
        "break",
        "byte",
        "case",
        "catch",
        "char",
        "checked",
        "class",
        "const",
        "continue",
        "decimal",
        "default",
        "delegate",
        "do",
        "double",
        "else",
        "enum",
        "event",
        "explicit",
        "extern",
        "false",
        "finally",
        "fixed",
        "float",
        "for",
        "foreach",
        "goto",
        "if",
        "implicit",
        "in",
        "int",
        "interface",
        "internal",
        "is",
        "lock",
        "long",
        "namespace",
        "new",
        "null",
        "object",
        "operator",
        "out",
        "override",
        "params",
        "private",
        "protected",
        "public",
        "readonly",
        "ref",
        "return",
        "sbyte",
        "sealed",
        "short",
        "sizeof",
        "stackalloc",
        "static",
        "string",
        "struct",
        "switch",
        "this",
        "throw",
        "true",
        "try",
        "typeof",
        "uint",
        "ulong",
        "unchecked",
        "unsafe",
        "ushort",
        "using",
        "virtual",
        "values",
        "void",
        "volatile",
        "while"
    };

    protected static readonly Dictionary<char, string> targetCharValueEscape;
    static CSharpTarget()
    {
        // https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/strings/#string-escape-sequences
        var map = new Dictionary<char, string>();
        AddEscapedChar(map, '\'');
        AddEscapedChar(map, '\"');
        AddEscapedChar(map, '\\');
        AddEscapedChar(map, '\0', '0');
        AddEscapedChar(map, (char)0x0007, 'a');
        AddEscapedChar(map, (char)0x0008, 'b');
        AddEscapedChar(map, '\f', 'f');
        AddEscapedChar(map, '\n', 'n');
        AddEscapedChar(map, '\r', 'r');
        AddEscapedChar(map, '\t', 't');
        AddEscapedChar(map, (char)0x000B, 'v');
        targetCharValueEscape = map;
    }

    public CSharpTarget(CodeGenerator gen) : base(gen)
    {
    }

    public override Dictionary<char, string> TargetCharValueEscape
        => targetCharValueEscape;


    public override HashSet<string> ReservedWords => reservedWords;

    protected override string EscapeWord(string word) => "@" + word;

    public override bool IsATNSerializedAsInts => true;

    protected override string EscapeChar(int v) => $"\\x{v:X}";// String.format("\\x%X", v);
}
