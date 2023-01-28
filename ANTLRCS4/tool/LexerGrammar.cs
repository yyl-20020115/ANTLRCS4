/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime.misc;
using org.antlr.v4.tool.ast;

namespace org.antlr.v4.tool;

/** */
public class LexerGrammar : Grammar {
	public static readonly String DEFAULT_MODE_NAME = "DEFAULT_MODE";

	/** The grammar from which this lexer grammar was derived (if implicit) */
    public Grammar implicitLexerOwner;

	/** DEFAULT_MODE rules are added first due to grammar syntax order */
	public MultiMap<String, Rule> modes;

	public LexerGrammar(Tool tool, GrammarRootAST ast): base(tool, ast)
    {
	}

	public LexerGrammar(String grammarText): base(grammarText)
    {
	}

	public LexerGrammar(String grammarText, ANTLRToolListener listener):base(grammarText,listener) {
	}

	public LexerGrammar(String fileName, String grammarText, ANTLRToolListener listener): base(fileName, grammarText, listener)
    {
	}

	//@Override
	public bool defineRule(Rule r) {
		if (!base.defineRule(r)) {
			return false;
		}

		if ( modes==null ) modes = new MultiMap<String, Rule>();
		modes.map(r.mode, r);
		return true;
	}

	//@Override
	public bool undefineRule(Rule r) {
		if (!base.undefineRule(r)) {
			return false;
		}

		bool removed = modes.get(r.mode).remove(r);
		//assert removed;
		return true;
	}
}
