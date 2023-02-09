/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime.misc;
using org.antlr.v4.tool.ast;

namespace org.antlr.v4.tool;

/** */
public class LexerGrammar : Grammar
{
    public static readonly string DEFAULT_MODE_NAME = "DEFAULT_MODE";

    /** The grammar from which this lexer grammar was derived (if implicit) */
    public Grammar implicitLexerOwner;

    /** DEFAULT_MODE rules are added first due to grammar syntax order */
    public MultiMap<string, Rule> modes;

    public LexerGrammar(Tool tool, GrammarRootAST ast) : base(tool, ast)
    {
    }

    public LexerGrammar(string grammarText) : base(grammarText)
    {
    }

    public LexerGrammar(string grammarText, ANTLRToolListener listener) : base(grammarText, listener)
    {
    }

    public LexerGrammar(string fileName, string grammarText, ANTLRToolListener listener) : base(fileName, grammarText, listener)
    {
    }

    public override bool DefineRule(Rule r)
    {
        if (!base.DefineRule(r))
        {
            return false;
        }

        modes ??= new MultiMap<String, Rule>();
        modes.Map(r.mode, r);
        return true;
    }

    public override bool UndefineRule(Rule r)
    {
        if (!base.UndefineRule(r))
        {
            return false;
        }
        if (modes.TryGetValue(r.name, out var rule))
        {
            rule.Remove(r);
            return true;
        }
        return false;
        //bool removed = modes.get(r.mode).remove(r);
        ////assert removed;
        //return true;
    }
}
