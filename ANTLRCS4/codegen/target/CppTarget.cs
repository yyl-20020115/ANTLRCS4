/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

namespace org.antlr.v4.codegen.target;

public class CppTarget : Target
{
	protected static readonly Dictionary<char, string> targetCharValueEscape;
	static CppTarget(){
        // https://stackoverflow.com/a/10220539/1046374
        Dictionary<char, string> map = new ();
		AddEscapedChar(map, (char)0x0007, 'a');
		AddEscapedChar(map, (char)0x0008, 'b');
		AddEscapedChar(map, '\t', 't');
		AddEscapedChar(map, '\n', 'n');
		AddEscapedChar(map, (char)0x000B, 'v');
		AddEscapedChar(map, '\f', 'f');
		AddEscapedChar(map, '\r', 'r');
		AddEscapedChar(map, (char)0x001B, 'e');
		AddEscapedChar(map, '\"');
		AddEscapedChar(map, '\'');
		AddEscapedChar(map, '?');
		AddEscapedChar(map, '\\');
		targetCharValueEscape = map;
	}

	protected static readonly HashSet<string> reservedWords =  new() {
		"alignas", "alignof", "and", "and_eq", "asm", "auto", "bitand",
		"bitor", "bool", "break", "case", "catch", "char", "char16_t",
		"char32_t", "class", "compl", "concept", "const", "constexpr",
		"const_cast", "continue", "decltype", "default", "delete", "do",
		"double", "dynamic_cast", "else", "enum", "explicit", "export",
		"extern", "false", "float", "for", "friend", "goto", "if",
		"inline", "int", "long", "mutable", "namespace", "new",
		"noexcept", "not", "not_eq", "nullptr", "NULL", "operator", "or",
		"or_eq", "private", "protected", "public", "register",
		"reinterpret_cast", "requires", "return", "short", "signed",
		"sizeof", "static", "static_assert", "static_cast", "struct",
		"switch", "template", "this", "thread_local", "throw", "true",
		"try", "typedef", "typeid", "typename", "union", "unsigned",
		"using", "virtual", "void", "volatile", "wchar_t", "while",
		"xor", "xor_eq",

		"rule", "parserRule"
	};

	public CppTarget(CodeGenerator gen) :base(gen){
		
	}

	
	public override Dictionary<char, string> GetTargetCharValueEscape() {
		return targetCharValueEscape;
	}

	
	public override HashSet<String> GetReservedWords() {
		return reservedWords;
	}

	public override bool NeedsHeader() { return true; }

    
	protected override bool ShouldUseUnicodeEscapeForCodePointInDoubleQuotedString(int codePoint) {
		if (codePoint == '?') {
			// in addition to the default escaped code points, also escape ? to prevent trigraphs
			// ideally, we would escape ? with \?, but escaping as unicode \u003F works as well
			return true;
		}
		else {
			return base.ShouldUseUnicodeEscapeForCodePointInDoubleQuotedString(codePoint);
		}
	}

	public override String GetRecognizerFileName(bool header) {
		var extST = GetTemplates().GetInstanceOf(header ? "headerFileExtension" : "codeFileExtension");
		String recognizerName = gen.g.GetRecognizerName();
		return recognizerName+extST.Render();
	}

	public override String GetListenerFileName(bool header) {
		//assert gen.g.name != null;
		var extST = GetTemplates().GetInstanceOf(header ? "headerFileExtension" : "codeFileExtension");
		String listenerName = gen.g.name + "Listener";
		return listenerName+extST.Render();
	}

	public override String GetVisitorFileName(bool header) {
		//assert gen.g.name != null;
		var extST = GetTemplates().GetInstanceOf(header ? "headerFileExtension" : "codeFileExtension");
		String listenerName = gen.g.name + "Visitor";
		return listenerName+extST.Render();
	}

	public override String GetBaseListenerFileName(bool header) {
		//assert gen.g.name != null;
		var extST = GetTemplates().GetInstanceOf(header ? "headerFileExtension" : "codeFileExtension");
		String listenerName = gen.g.name + "BaseListener";
		return listenerName+extST.Render();
	}

	public override String GetBaseVisitorFileName(bool header) {
		//assert gen.g.name != null;
		var extST = GetTemplates().GetInstanceOf(header ? "headerFileExtension" : "codeFileExtension");
		String listenerName = gen.g.name + "BaseVisitor";
		return listenerName+extST.Render();
	}
}
