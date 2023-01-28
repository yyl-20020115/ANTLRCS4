/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime;
using org.antlr.v4.runtime.misc;
using org.antlr.v4.tool;
using org.antlr.v4.tool.ast;
using Attribute = org.antlr.v4.tool.Attribute;

namespace org.antlr.v4.parse;

/**
 * Parse args, return values, locals
 * <p>
 * rule[arg1, arg2, ..., argN] returns [ret1, ..., retN]
 * <p>
 * text is target language dependent.  Java/C#/C/C++ would
 * use "int i" but ruby/python would use "i". Languages with
 * postfix types like Go, Swift use "x : T" notation or "T x".
 */
public class ScopeParser {
	/**
	 * Given an arg or retval scope definition list like
	 * <p>
	 * <code>
	 * Map&lt;String, String&gt;, int[] j3, char *foo32[3]
	 * </code>
	 * <p>
	 * or
	 * <p>
	 * <code>
	 * int i=3, j=a[34]+20
	 * </code>
	 * <p>
	 * convert to an attribute scope.
	 */
	public static AttributeDict parseTypedArgList(ActionAST action, String s, Grammar g) {
		return parse(action, s, ',', g);
	}

	public static AttributeDict parse(ActionAST action, String s, char separator, Grammar g) {
		AttributeDict dict = new AttributeDict();
		List<Pair<String, int>> decls = splitDecls(s, separator);
		foreach (Pair<String, int> decl in decls) {
			if (decl.a.Trim().Length > 0) {
				tool.Attribute a = parseAttributeDef(action, decl, g);
				dict.add(a);
			}
		}
		return dict;
	}

	/**
	 * For decls like "String foo" or "char *foo32[]" compute the ID
	 * and type declarations.  Also handle "int x=3" and 'T t = new T("foo")'
	 * but if the separator is ',' you cannot use ',' in the initvalue
	 * unless you escape use "\," escape.
	 */
	public static Attribute parseAttributeDef(ActionAST action, Pair<String, int> decl, Grammar g) {
		if (decl.a == null) return null;

		Attribute attr = new Attribute();
		int rightEdgeOfDeclarator = decl.a.Length - 1;
		int equalsIndex = decl.a.indexOf('=');
		if (equalsIndex > 0) {
			// everything after the '=' is the init value
			attr.initValue = decl.a.substring(equalsIndex + 1, decl.a.Length).Trim();
			rightEdgeOfDeclarator = equalsIndex - 1;
		}

		String declarator = decl.a.substring(0, rightEdgeOfDeclarator + 1);
		Pair<int, int> p;
		String text = decl.a;
		text = text.Replace("::","");
		if ( text.Contains(':') ) {
			// declarator has type appearing after the name like "x:T"
			p = _parsePostfixDecl(attr, declarator, action, g);
		}
		else {
			// declarator has type appearing before the name like "T x"
			p = _parsePrefixDecl(attr, declarator, action, g);
		}
		int idStart = p.a;
		int idStop = p.b;

		attr.decl = decl.a;

		if (action != null) {
			String actionText = action.getText();
			int[] lines = new int[actionText.Length];
			int[] charPositionInLines = new int[actionText.Length];
			for (int i = 0, _line = 0, col = 0; i < actionText.Length; i++, col++) {
				lines[i] = _line;
				charPositionInLines[i] = col;
				if (actionText[(i)] == '\n') {
					_line++;
					col = -1;
				}
			}

			int[] charIndexes = new int[actionText.Length];
			for (int i = 0, j = 0; i < actionText.Length; i++, j++) {
				charIndexes[j] = i;
				// skip comments
				if (i < actionText.Length - 1 && actionText[i] == '/' && actionText[(i + 1)] == '/') {
					while (i < actionText.Length && actionText[i] != '\n') {
						i++;
					}
				}
			}

			int declOffset = charIndexes[decl.b];
			int declLine = lines[declOffset + idStart];

			int line = action.getToken().getLine() + declLine;
			int charPositionInLine = charPositionInLines[declOffset + idStart];
			if (declLine == 0) {
				/* offset for the start position of the ARG_ACTION token, plus 1
				 * since the ARG_ACTION text had the leading '[' stripped before
				 * reaching the scope parser.
				 */
				charPositionInLine += action.getToken().getCharPositionInLine() + 1;
			}

			int offset = ((CommonToken) action.getToken()).getStartIndex();
			attr.token = new CommonToken(action.getToken().getInputStream(), ANTLRParser.ID, BaseRecognizer.DEFAULT_TOKEN_CHANNEL, offset + declOffset + idStart + 1, offset + declOffset + idStop);
			attr.token.setLine(line);
			attr.token.setCharPositionInLine(charPositionInLine);
			//assert attr.name.Equals(attr.token.getText()) : "Attribute text should match the pseudo-token text at this point.";
		}

		return attr;
	}

	public static Pair<int, int> _parsePrefixDecl(tool.Attribute attr, String decl, ActionAST a, Grammar g) {
		// walk backwards looking for start of an ID
		bool inID = false;
		int start = -1;
		for (int i = decl.Length - 1; i >= 0; i--) {
			char ch = decl[(i)];
			// if we haven't found the end yet, keep going
			if (!inID && char.IsLetterOrDigit(ch)) {
				inID = true;
			}
			else if (inID && !(char.IsLetterOrDigit(ch) || ch == '_')) {
				start = i + 1;
				break;
			}
		}
		if (start < 0 && inID) {
			start = 0;
		}
		if (start < 0) {
			g.tool.errMgr.grammarError(ErrorType.CANNOT_FIND_ATTRIBUTE_NAME_IN_DECL, g.fileName, a.token, decl);
		}

		// walk forward looking for end of an ID
		int stop = -1;
		for (int i = start; i < decl.Length; i++) {
			char ch = decl[(i)];
			// if we haven't found the end yet, keep going
			if (!(char.IsLetterOrDigit(ch) || ch == '_')) {
				stop = i;
				break;
			}
			if (i == decl.Length - 1) {
				stop = i + 1;
			}
		}

		// the name is the last ID
		attr.name = decl.substring(start, stop);

		// the type is the decl minus the ID (could be empty)
		attr.type = decl.substring(0, start);
		if (stop <= decl.Length	 - 1) {
			attr.type += decl.substring(stop, decl.Length);
		}

		attr.type = attr.type.Trim();
		if (attr.type.Length == 0) {
			attr.type = null;
		}
		return new Pair<int, int>(start, stop);
	}

	public static Pair<int, int> _parsePostfixDecl(tool.Attribute attr, String decl, ActionAST a, Grammar g) {
		int start = -1;
		int stop = -1;
		int colon = decl.indexOf(':');
		int namePartEnd = colon == -1 ? decl.Length : colon;

		// look for start of name
		for (int i = 0; i < namePartEnd; ++i) {
			char ch = decl[(i)];
			if (char.IsLetterOrDigit(ch) || ch == '_') {
				start = i;
				break;
			}
		}

		if (start == -1) {
			start = 0;
			g.tool.errMgr.grammarError(ErrorType.CANNOT_FIND_ATTRIBUTE_NAME_IN_DECL, g.fileName, a.token, decl);
		}

		// look for stop of name
		for (int i = start; i < namePartEnd; ++i) {
			char ch = decl[(i)];
			if (!(char.IsLetterOrDigit(ch) || ch == '_')) {
				stop = i;
				break;
			}
			if (i == namePartEnd - 1) {
				stop = namePartEnd;
			}
		}

		if (stop == -1) {
			stop = start;
		}

		// extract name from decl
		attr.name =  decl.substring(start, stop);

		// extract type from decl (could be empty)
		if (colon == -1) {
			attr.type = "";
		}
		else {
			attr.type = decl.substring(colon + 1, decl.Length);
		}
		attr.type = attr.type.Trim();

		if (attr.type.Length == 0) {
			attr.type = null;
		}
		return new Pair<int, int>(start, stop);
	}

	/**
	 * Given an argument list like
	 * <p>
	 * x, (*a).foo(21,33), 3.2+1, '\n',
	 * "a,oo\nick", {bl, "fdkj"eck}, ["cat\n,", x, 43]
	 * <p>
	 * convert to a list of attributes.  Allow nested square brackets etc...
	 * Set separatorChar to ';' or ',' or whatever you want.
	 */
	public static List<Pair<String, int>> splitDecls(String s, int separatorChar) {
		List<Pair<String, int>> args = new ();
		_splitArgumentList(s, 0, -1, separatorChar, args);
		return args;
	}

	public static int _splitArgumentList(String actionText,
	                                     int start,
	                                     int targetChar,
	                                     int separatorChar,
	                                     List<Pair<String, int>> args) {
		if (actionText == null) {
			return -1;
		}

		actionText = actionText.replaceAll("//[^\\n]*", "");
		int n = actionText.Length;
		//Console.Out.WriteLine("actionText@"+start+"->"+(char)targetChar+"="+actionText.substring(start,n));
		int p = start;
		int last = p;
		while (p < n && actionText[p] != targetChar) {
			int c = actionText[p];
			switch (c) {
				case '\'':
					p++;
					while (p < n && actionText[p] != '\'') {
						if (actionText[p] == '\\' && (p + 1) < n &&
								actionText[(p + 1)] == '\'') {
							p++; // skip escaped quote
						}
						p++;
					}
					p++;
					break;
				case '"':
					p++;
					while (p < n && actionText[p] != '\"') {
						if (actionText[p] == '\\' && (p + 1) < n &&
								actionText[(p + 1)] == '\"') {
							p++; // skip escaped quote
						}
						p++;
					}
					p++;
					break;
				case '(':
					p = _splitArgumentList(actionText, p + 1, ')', separatorChar, args);
					break;
				case '{':
					p = _splitArgumentList(actionText, p + 1, '}', separatorChar, args);
					break;
				case '<':
					if (actionText.indexOf('>', p + 1) >= p) {
						// do we see a matching '>' ahead?  if so, hope it's a generic
						// and not less followed by expr with greater than
						p = _splitArgumentList(actionText, p + 1, '>', separatorChar, args);
					}
					else {
						p++; // treat as normal char
					}
					break;
				case '[':
					p = _splitArgumentList(actionText, p + 1, ']', separatorChar, args);
					break;
				default:
					if (c == separatorChar && targetChar == -1) {
						String arg = actionText.substring(last, p);
						int index = last;
						while (index < p && char.isWhitespace(actionText[(index)])) {
							index++;
						}
						//Console.Out.WriteLine("arg="+arg);
						args.Add(new Pair<String, int>(arg.Trim(), index));
						last = p + 1;
					}
					p++;
					break;
			}
		}
		if (targetChar == -1 && p <= n) {
			String arg = actionText.substring(last, p).Trim();
			int index = last;
			while (index < p && char.IsWhiteSpace(actionText[index])) {
				index++;
			}
			//Console.Out.WriteLine("arg="+arg);
			if (arg.Length > 0) {
				args.Add(new Pair<String, int>(arg.Trim(), index));
			}
		}
		p++;
		return p;
	}

}
