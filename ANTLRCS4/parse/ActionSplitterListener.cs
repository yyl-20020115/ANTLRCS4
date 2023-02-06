/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime;
namespace org.antlr.v4.parse;


/** */
public interface ActionSplitterListener
{
    void QualifiedAttr(string expr, Token x, Token y);
    void SetAttr(string expr, Token x, Token rhs);
    void Attr(string expr, Token x);
    void SetNonLocalAttr(string expr, Token x, Token y, Token rhs);
    void NonLocalAttr(string expr, Token x, Token y);
    void Text(string text);
}
