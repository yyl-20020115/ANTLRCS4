/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime;
using org.antlr.v4.tool;

namespace org.antlr.v4.parse;

public class ToolANTLRLexer : ANTLRLexer
{
    public Tool tool;

    public ToolANTLRLexer(CharStream input, Tool tool) : base(input)
    {
        this.tool = tool;
    }

    public override void DisplayRecognitionError(string[] tokenNames, RecognitionException e)
    {
        var msg = GetErrorMessage(e, tokenNames);
        tool.ErrMgr.SyntaxError(ErrorType.SYNTAX_ERROR, SourceName, e.token, e, msg);
    }
    public override void GrammarError(ErrorType etype, Token token, params object[] args)
    {
        tool.ErrMgr.GrammarError(etype, SourceName, token, args);
    }
}
