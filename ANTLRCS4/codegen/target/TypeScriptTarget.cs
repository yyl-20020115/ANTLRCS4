/*
 * Copyright 20162022 The ANTLR Project. All rights reserved.
 * Licensed under the BSD-3-Clause license. See LICENSE file in the project root for license information.
 */
namespace org.antlr.v4.codegen.target;

public class TypeScriptTarget : Target
{

    /* source: https://github.com/microsoft/TypeScript/blob/fad889283e710ee947e8412e173d2c050107a3c1/src/compiler/scanner.ts */
    protected static readonly HashSet<string> reservedWords = new() {
            "any",
            "as",
            "boolean",
            "break",
            "case",
            "catch",
            "class",
            "continue",
            "const",
            "constructor",
            "debugger",
            "declare",
            "default",
            "delete",
            "do",
            "else",
            "enum",
            "export",
            "extends",
            "false",
            "finally",
            "for",
            "from",
            "function",
            "get",
            "if",
            "implements",
            "import",
            "in",
            "instanceof",
            "interface",
            "let",
            "module",
            "new",
            "null",
            "number",
            "package",
            "private",
            "protected",
            "public",
            "require",
            "return",
            "set",
            "static",
            "string",
            "super",
            "switch",
            "symbol",
            "this",
            "throw",
            "true",
            "try",
            "type",
            "typeof",
            "var",
            "void",
            "while",
            "with",
            "yield",
            "of"
    };

    public TypeScriptTarget(CodeGenerator gen) : base(gen)
    {
    }

    public override HashSet<string> ReservedWords => reservedWords;

    public override int InlineTestSetWordSize => 32;

    public override bool WantsBaseListener => false;

    public override bool WantsBaseVisitor => false;

    public override bool SupportsOverloadedMethods => true;

    public override bool IsATNSerializedAsInts => true;

}
