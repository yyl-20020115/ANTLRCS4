/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime;
using org.antlr.v4.runtime.tree;

namespace org.antlr.v4.tool.ast;

public class GrammarRootAST : GrammarASTWithOptions {
	public static readonly Dictionary<String, String> defaultOptions = new ();
	static GrammarRootAST(){
		defaultOptions.Add("language","Java");
	}

    public int grammarType; // LEXER, PARSER, GRAMMAR (combined)
	public bool hasErrors;
	/** Track stream used to create this tree */

	public readonly TokenStream tokenStream;
	public Dictionary<String, String> cmdLineOptions; // -DsuperClass=T on command line
	public String fileName;

	public GrammarRootAST(GrammarRootAST node): base(node)
    {
		this.grammarType = node.grammarType;
		this.hasErrors = node.hasErrors;
		this.tokenStream = node.tokenStream;
	}

	public GrammarRootAST(Token t, TokenStream tokenStream): base(t)
    {
		if (tokenStream == null) {
			throw new NullReferenceException("tokenStream");
		}

		this.tokenStream = tokenStream;
	}

	public GrammarRootAST(int type, Token t, TokenStream tokenStream): base(type, t)
    {
		;
		if (tokenStream == null) {
			throw new NullReferenceException("tokenStream");
		}

		this.tokenStream = tokenStream;
	}

	public GrammarRootAST(int type, Token t, String text, TokenStream tokenStream) : base(type, t, text)
    {
		if (tokenStream == null) {
			throw new NullReferenceException("tokenStream");
		}

		this.tokenStream = tokenStream;
    }

	public String getGrammarName() {
		Tree t = getChild(0);
		if ( t!=null ) return t.getText();
		return null;
	}

	//@Override
	public String getOptionString(String key) {
		if ( cmdLineOptions!=null && cmdLineOptions.TryGetValue(key,out var v) ) {
			return v;
		}
		String value = base.getOptionString(key);
		if ( value==null && defaultOptions.TryGetValue(key, out var v2)) {
			value = v2;
		}
		return value;
	}

	//@Override
	public Object visit(GrammarASTVisitor v) { return v.visit(this); }

    //@Override
    public override GrammarRootAST dupNode() { return new GrammarRootAST(this); }
}
