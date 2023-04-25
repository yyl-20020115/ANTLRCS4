/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using Antlr4.StringTemplate;

namespace org.antlr.v4.codegen.target;

public class JavaTarget : Target
{
    /**
	 * The Java target can cache the code generation templates.
	 */
    private static readonly ThreadLocal<TemplateGroup> targetTemplates = new();

    protected static readonly HashSet<string> reservedWords = new() {
        "abstract", "assert", "boolean", "break", "byte", "case", "catch",
        "char", "class", "const", "continue", "default", "do", "double", "else",
        "enum", "extends", "false", "final", "finally", "float", "for", "goto",
        "if", "implements", "import", "instanceof", "int", "interface",
        "long", "native", "new", "null", "package", "private", "protected",
        "public", "return", "short", "static", "strictfp", "super", "switch",
        "synchronized", "this", "throw", "throws", "transient", "true", "try",
        "void", "volatile", "while",

		// misc
		"rule", "parserRule"
    };

    public JavaTarget(CodeGenerator gen) : base(gen)
    {

    }

    public override HashSet<String> ReservedWords => reservedWords;

    public override int SerializedATNSegmentLimit =>
        // 65535 is the class file format byte limit for a UTF-8 encoded string literal
        // 3 is the maximum number of bytes it takes to encode a value in the range 0-0xFFFF
        65535 / 3;


    public override bool IsATNSerializedAsInts => false;
}
