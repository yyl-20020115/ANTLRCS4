/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

namespace org.antlr.v4.codegen.target;

public class JavaScriptTarget : Target
{
    /** Source: https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Lexical_grammar */
    protected static readonly HashSet<String> reservedWords = new() {
        "break", "case", "class", "catch", "const", "continue", "debugger",
        "default", "delete", "do", "else", "export", "extends", "finally", "for",
        "function", "if", "import", "in", "instanceof", "let", "new", "return",
        "super", "switch", "this", "throw", "try", "typeof", "var", "void",
        "while", "with", "yield",

		//future reserved
		"enum", "await", "implements", "package", "protected", "static",
        "interface", "private", "public",

		//future reserved in older standards
		"abstract", "boolean", "byte", "char", "double", "final", "float",
        "goto", "int", "long", "native", "short", "synchronized", "transient",
        "volatile",

		//literals
		"null", "true", "false",

		// misc
		"rule", "parserRule"
    };

    public JavaScriptTarget(CodeGenerator gen) : base(gen)
    {
    }

    public override HashSet<String> ReservedWords => reservedWords;

    public override int InlineTestSetWordSize => 32;

    public override bool WantsBaseListener => false;

    public override bool WantsBaseVisitor => false;

    public override bool SupportsOverloadedMethods => false;

    public override bool IsATNSerializedAsInts => true;
}
