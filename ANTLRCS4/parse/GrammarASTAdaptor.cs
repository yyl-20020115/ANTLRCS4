/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.runtime.tree;
using org.antlr.v4.runtime;
using org.antlr.v4.tool.ast;

namespace org.antlr.v4.parse;


public class GrammarASTAdaptor : CommonTreeAdaptor
{
    CharStream input; // where we can find chars ref'd by tokens in tree
    public GrammarASTAdaptor() { }
    public GrammarASTAdaptor(CharStream input) { this.input = input; }

    //@Override
    public object Create(Token token)
    {
        return new GrammarAST(token);
    }

    //@Override
    /** Make sure even imaginary nodes know the input stream */
    public object create(int tokenType, string text)
    {
        GrammarAST t;
        if (tokenType == ANTLRParser.RULE)
        {
            // needed by TreeWizard to make RULE tree
            t = new RuleAST(new CommonToken(tokenType, text));
        }
        else if (tokenType == ANTLRParser.STRING_LITERAL)
        {
            // implicit lexer construction done with wizard; needs this node type
            // whereas grammar ANTLRParser.g can use token option to spec node type
            t = new TerminalAST(new CommonToken(tokenType, text));
        }
        else
        {
            t = (GrammarAST)base.create(tokenType, text);
        }
        t.token.        InputStream = input;
        return t;
    }

    //@Override
    public object dupNode(Object t)
    {
        if (t == null) return null;
        return ((GrammarAST)t).dupNode(); //create(((GrammarAST)t).token);
    }

    //@Override
    public object errorNode(TokenStream input, Token start, Token stop,
                            RecognitionException e)
    {
        return new GrammarASTErrorNode(input, start, stop, e);
    }
}
