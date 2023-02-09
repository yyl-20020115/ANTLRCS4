/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime;
using org.antlr.v4.runtime.tree;

namespace org.antlr.v4.tool.ast;

public class GrammarRootAST : GrammarASTWithOptions
{
    public static readonly Dictionary<string, string> defaultOptions = new();
    static GrammarRootAST()
    {
        defaultOptions.Add("language", "Java");
    }

    public int grammarType; // LEXER, PARSER, GRAMMAR (combined)
    public bool hasErrors;
    /** Track stream used to create this tree */

    public readonly TokenStream tokenStream;
    public Dictionary<string, string> cmdLineOptions; // -DsuperClass=T on command line
    public string fileName;

    public GrammarRootAST(GrammarRootAST node) : base(node)
    {
        this.grammarType = node.grammarType;
        this.hasErrors = node.hasErrors;
        this.tokenStream = node.tokenStream;
    }

    public GrammarRootAST(Token t, TokenStream tokenStream) : base(t)
    {
        this.tokenStream = tokenStream ?? throw new NullReferenceException("tokenStream");
    }

    public GrammarRootAST(int type, Token t, TokenStream tokenStream) : base(type, t)
    {
        this.tokenStream = tokenStream ?? throw new NullReferenceException("tokenStream");
    }

    public GrammarRootAST(int type, Token t, string text, TokenStream tokenStream) : base(type, t, text)
    {
        this.tokenStream = tokenStream ?? throw new NullReferenceException("tokenStream");
    }

    public string GetGrammarName()
    {
        var t = GetChild(0);
        if (t != null) return t.Text;
        return null;
    }

    public override string GetOptionString(string key)
    {
        if (cmdLineOptions != null && cmdLineOptions.TryGetValue(key, out var v))
        {
            return v;
        }
        var value = base.GetOptionString(key);
        if (value == null && defaultOptions.TryGetValue(key, out var v2))
        {
            value = v2;
        }
        return value;
    }

    public override object Visit(GrammarASTVisitor v) => v.Visit(this);

    public override GrammarRootAST DupNode() => new GrammarRootAST(this);
}
