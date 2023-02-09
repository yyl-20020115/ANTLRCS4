/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.parse;
using org.antlr.v4.runtime.misc;
using org.antlr.v4.tool.ast;

namespace org.antlr.v4.tool;

public class LabelElementPair
{
    public static readonly BitSet tokenTypeForTokens = new ();
    static LabelElementPair()
    {
        tokenTypeForTokens.Set(ANTLRParser.TOKEN_REF);
        tokenTypeForTokens.Set(ANTLRParser.STRING_LITERAL);
        tokenTypeForTokens.Set(ANTLRParser.WILDCARD);
    }

    public GrammarAST label;
    public GrammarAST element;
    public LabelType type;

    public LabelElementPair(Grammar g, GrammarAST label, GrammarAST element, int labelOp)
    {
        this.label = label;
        this.element = element;
        // compute general case for label type
        if (element.getFirstDescendantWithType(tokenTypeForTokens) != null)
        {
            if (labelOp == ANTLRParser.ASSIGN) type = LabelType.TOKEN_LABEL;
            else type = LabelType.TOKEN_LIST_LABEL;
        }
        else if (element.getFirstDescendantWithType(ANTLRParser.RULE_REF) != null)
        {
            if (labelOp == ANTLRParser.ASSIGN) type = LabelType.RULE_LABEL;
            else type = LabelType.RULE_LIST_LABEL;
        }

        // now reset if lexer and string
        if (g.isLexer())
        {
            if (element.getFirstDescendantWithType(ANTLRParser.STRING_LITERAL) != null)
            {
                if (labelOp == ANTLRParser.ASSIGN) type = LabelType.LEXER_STRING_LABEL;
            }
        }
    }

    public override string ToString() => label.getText() + " " + type + " " + element.toString();
}
