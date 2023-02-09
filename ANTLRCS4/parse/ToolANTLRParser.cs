/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime;
using org.antlr.v4.tool;

namespace org.antlr.v4.parse;


/** Override error handling for use with ANTLR tool itself; leaves
 *  nothing in grammar associated with Tool so others can use in IDEs, ...
 */
public class ToolANTLRParser : ANTLRParser
{
    public Tool tool;

    public ToolANTLRParser(TokenStream input, Tool tool) : base(input)
    {
        this.tool = tool;
    }

    //@Override
    public void DisplayRecognitionError(string[] tokenNames,
                                        RecognitionException e)
    {
        var msg = GetParserErrorMessage(this, e);
        if (paraphrases.Count > 0)
        {
            var paraphrase = paraphrases.Peek();
            msg = msg + " while " + paraphrase;
        }
        //	List stack = getRuleInvocationStack(e, this.getClass().getName());
        //	msg += ", rule stack = "+stack;
        tool.ErrMgr.SyntaxError(ErrorType.SYNTAX_ERROR, SourceName, e.token, e, msg);
    }

    public string GetParserErrorMessage(antlr.runtime.Parser parser, RecognitionException e)
    {
        string msg;
        if (e is NoViableAltException)
        {
            string name = GetTokenErrorDisplay(e.token);
            msg = name + " came as a complete surprise to me";
        }
        else if (e is V4ParserException exception)
        {
            msg = exception.msg;
        }
        else
        {
            msg = parser.GetErrorMessage(e, parser.TokenNames);
        }
        return msg;
    }

    //@Override
    public void GrammarError(ErrorType etype, Token token, params object[] args)
    {
        tool.ErrMgr.GrammarError(etype, SourceName, token, args);
    }
}
