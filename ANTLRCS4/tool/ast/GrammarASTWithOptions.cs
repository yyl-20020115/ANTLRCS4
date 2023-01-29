/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.misc;
using org.antlr.v4.runtime;

namespace org.antlr.v4.tool.ast;

public abstract class GrammarASTWithOptions : GrammarAST {
    protected Dictionary<String, GrammarAST> options;

	public GrammarASTWithOptions(GrammarASTWithOptions node):base(node) {
		this.options = node.options;
	}

	public GrammarASTWithOptions(Token t):base(t) {  }
    public GrammarASTWithOptions(int type):base(type) {  }
    public GrammarASTWithOptions(int type, Token t):base(type,t) {  }
    public GrammarASTWithOptions(int type, Token t, String text):base(type,t,text) {  }

    public void setOption(String key, GrammarAST node) {
        if ( options==null ) options = new ();
        options[key] = node;
    }

	public String getOptionString(String key) {
		GrammarAST value = getOptionAST(key);
		if ( value == null ) return null;
		if ( value is ActionAST ) {
			return value.getText();
		}
		else {
			String v = value.getText();
			if ( v.StartsWith("'") || v.StartsWith("\"") ) {
				v = CharSupport.getStringFromGrammarStringLiteral(v);
				if (v == null) {
					g.tool.errMgr.grammarError(ErrorType.INVALID_ESCAPE_SEQUENCE, g.fileName, value.getToken(), value.getText());
					v = "";
				}
			}
			return v;
		}
	}

	/** Gets AST node holding value for option key; ignores default options
	 *  and command-line forced options.
	 */
    public GrammarAST getOptionAST(String key) {
        if ( options==null ) return null;
        return options.TryGetValue(key,out var r)?r:null;
    }

	public int getNumberOfOptions() {
		return options==null ? 0 : options.Count;
	}

	//@Override
	public abstract GrammarASTWithOptions dupNode();


	public Dictionary<String, GrammarAST> getOptions() {
		if (options == null) {
			return new();// Collections.emptyMap();
		}

		return options;
	}
}
