/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using Antlr4.StringTemplate;
using org.antlr.v4.parse;
using org.antlr.v4.tool;
using System.Diagnostics;

namespace org.antlr.v4.codegen.target;

public class GoTarget : Target {
	protected static readonly HashSet<String> reservedWords = new() { 
		// keywords
		"break", "default", "func", "interface", "select",
		"case", "defer", "go", "map", "struct",
		"chan", "else", "goto", "package", "switch",
		"const", "fallthrough", "if", "range", "type",
		"continue", "for", "import", "return", "var",

		// predeclared identifiers https://golang.org/ref/spec#Predeclared_identifiers
		"bool", "byte", "complex64", "complex128", "error", "float32", "float64",
		"int", "int8", "int16", "int32", "int64", "rune", "string",
		"uint", "uint8", "uint16", "uint32", "uint64", "uintptr",
		"true", "false", "iota", "nil",
		"append", "cap", "close", "complex", "copy", "delete", "imag", "len",
		"make", "new", "panic", "print", "println", "real", "recover",
		"string",

		// interface definition of RuleContext from runtime/Go/antlr/rule_context.go
		"Accept", "GetAltNumber", "GetBaseRuleContext", "GetChild", "GetChildCount",
		"GetChildren", "GetInvokingState", "GetParent", "GetPayload", "GetRuleContext",
		"GetRuleIndex", "GetSourceInterval", "GetText", "IsEmpty", "SetAltNumber",
		"SetInvokingState", "SetParent", "String",

		// misc
		"rule", "parserRule", "action"
	};

	private static readonly bool DO_GOFMT = !(bool.TryParse(
		Environment.GetEnvironmentVariable("ANTLR_GO_DISABLE_GOFMT"), out var do_gofmt)
		|| do_gofmt) && !(bool.TryParse(Environment.GetEnvironmentVariable("antlr.go.disable-gofmt\""), out var disable_gofmt) ||
		!disable_gofmt);

	public GoTarget(CodeGenerator gen):base(gen) {
	}

    public override HashSet<String> ReservedWords => reservedWords;

    public override void GenFile(Grammar g, Template outputFileST, String fileName) {
		base.GenFile(g, outputFileST, fileName);
		if (DO_GOFMT && !fileName.StartsWith(".") /* criterion taken from gofmt */ && fileName.EndsWith(".go")) {
			gofmt(Path.Combine(CodeGenerator.tool.GetOutputDirectory(g.fileName), fileName));
		}
	}

	private void gofmt(string fileName) {
		// Optimistically run gofmt. If this fails, it doesn't matter at this point. Wait for termination though,
		// because "gofmt -w" uses ioutil.WriteFile internally, which means it literally writes in-place with O_TRUNC.
		// That could result in a race. (Why oh why doesn't it do tmpfile + rename?)
		try {
			// TODO: need something like: String goExecutable = locateGo();
			Process p = Process.Start("gofmt", "-w -s " + fileName);
			p.ErrorDataReceived += (sender, e) =>
			{

			};
			if (p.Start())
			{
				p.WaitForExit();
			}
			//// TODO(wjkohnen): simplify to `while (stdout.Read() > 1) {}`
			//byte[] buf = new byte[1 << 10];
			//for (int l = 0; l > -1; l = stdout.read(buf)) {
			//	// There should not be any output that exceeds the implicit output buffer. In normal ops there should be
			//	// zero output. In case there is output, blocking and therefore killing the process is acceptable. This
			//	// drains the buffer anyway to play it safe.

			//	// dirty debug (change -w above to -d):
			//	// System.err.write(buf, 0, l);
			//}
		} catch (IOException e) {
			// Probably gofmt not in $PATH, in any case ignore.
		}
		//catch (InterruptedException forward) {
		//	Thread.currentThread().interrupt();
		//}
	}

	public override String GetRecognizerFileName(bool header) {
		CodeGenerator gen = CodeGenerator;
		Grammar g = gen.g;
		//assert g!=null;
		String name;
		switch ( g.Type) {
			case ANTLRParser.PARSER:
                name = g.name.EndsWith("Parser") ? g.name[0..(g.name.Length - 6)] : g.name;
				return name.ToLower()+"_parser.go";
			case ANTLRParser.LEXER:
				name = g.name.EndsWith("Lexer") ? g.name[0..(g.name.Length - 5)] : g.name; // trim off "lexer"
				return name.ToLower()+"_lexer.go";
			case ANTLRParser.COMBINED:
				return g.name.ToLower()+"_parser.go";
			default :
				return "INVALID_FILE_NAME";
		}
	}

	/** A given grammar T, return the listener name such as
	 *  TListener.java, if we're using the Java target.
 	 */
	public override String GetListenerFileName(bool header) {
		CodeGenerator gen = CodeGenerator;
		Grammar g = gen.g;
		//assert g.name != null;
		return g.name.ToLower()+"_listener.go";
	}

	/** A given grammar T, return the visitor name such as
	 *  TVisitor.java, if we're using the Java target.
 	 */
	public override String GetVisitorFileName(bool header) {
		CodeGenerator gen = CodeGenerator;
		Grammar g = gen.g;
		//assert g.name != null;
		return g.name.ToLower()+"_visitor.go";
	}

	/** A given grammar T, return a blank listener implementation
	 *  such as TBaseListener.java, if we're using the Java target.
 	 */
	public override String GetBaseListenerFileName(bool header) {
		CodeGenerator gen = CodeGenerator;
		Grammar g = gen.g;
		//assert g.name != null;
		return g.name.ToLower()+"_base_listener.go";
	}

	/** A given grammar T, return a blank listener implementation
	 *  such as TBaseListener.java, if we're using the Java target.
 	 */
	public override String GetBaseVisitorFileName(bool header) {
		CodeGenerator gen = CodeGenerator;
		Grammar g = gen.g;
		//assert g.name != null;
		return g.name.ToLower()+"_base_visitor.go";
	}
}
