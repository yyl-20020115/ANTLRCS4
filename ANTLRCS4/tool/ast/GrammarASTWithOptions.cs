/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.misc;
using org.antlr.v4.runtime;

namespace org.antlr.v4.tool.ast;

public abstract class GrammarASTWithOptions : GrammarAST
{
    protected Dictionary<string, GrammarAST> options;

    public GrammarASTWithOptions(GrammarASTWithOptions node) : base(node)
    {
        this.options = node.options;
    }

    public GrammarASTWithOptions(Token t) : base(t) { }
    public GrammarASTWithOptions(int type) : base(type) { }
    public GrammarASTWithOptions(int type, Token t) : base(type, t) { }
    public GrammarASTWithOptions(int type, Token t, string text) : base(type, t, text) { }

    public void SetOption(string key, GrammarAST node)
    {
        options ??= new();
        options[key] = node;
    }

    public virtual string GetOptionString(string key)
    {
        var value = GetOptionAST(key);
        if (value == null) return null;
        if (value is ActionAST)
        {
            return value.Text;
        }
        else
        {
            var v = value.Text;
            if (v.StartsWith("'") || v.StartsWith("\""))
            {
                v = CharSupport.GetStringFromGrammarStringLiteral(v);
                if (v == null)
                {
                    g.Tools.ErrMgr.GrammarError(ErrorType.INVALID_ESCAPE_SEQUENCE, g.fileName, value.Token, value.Text);
                    v = "";
                }
            }
            return v;
        }
    }

    /** Gets AST node holding value for option key; ignores default options
	 *  and command-line forced options.
	 */
    public GrammarAST GetOptionAST(string key) => options == null ? null : options.TryGetValue(key, out var r) ? r : null;

    public int NumberOfOptions => options == null ? 0 : options.Count;

    //@Override
    public abstract GrammarASTWithOptions DupNode();


    public Dictionary<string, GrammarAST> Options
    {
        get
        {
            if (options == null)
            {
                return new();// Collections.emptyMap();
            }

            return options;
        }
    }
}
