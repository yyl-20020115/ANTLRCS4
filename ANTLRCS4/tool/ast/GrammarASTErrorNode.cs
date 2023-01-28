/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.runtime.tree;
using org.antlr.v4.runtime;

namespace org.antlr.v4.tool.ast;

/** A node representing erroneous token range in token stream */
public class GrammarASTErrorNode : GrammarAST {
    CommonErrorNode @delegate;
    public GrammarASTErrorNode(TokenStream input, Token start, Token stop,
                               RecognitionException e)
    {
        @delegate = new CommonErrorNode(input,start,stop,e);
    }

    //@Override
    public bool isNil() { return @delegate.isNil(); }

    //@Override
    public int getType() { return @delegate.getType(); }

    //@Override
    public String getText() { return @delegate.getText(); }
    //@Override
    public String toString() { return @delegate.toString(); }
}
