/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.parse;
using org.antlr.v4.runtime;

namespace org.antlr.v4.semantics;


public class BlankActionSplitterListener : ActionSplitterListener
{
    public virtual void QualifiedAttr(string expr, Token x, Token y)
    {
    }

    public virtual void SetAttr(string expr, Token x, Token rhs)
    {
    }

    public virtual void Attr(string expr, Token x)
    {
    }

    public virtual void TemplateInstance(string expr)
    {
    }

    public virtual void NonLocalAttr(string expr, Token x, Token y)
    {
    }

    public virtual void SetNonLocalAttr(string expr, Token x, Token y, Token rhs)
    {
    }

    public virtual void IndirectTemplateInstance(string expr)
    {
    }

    public virtual void SetExprAttribute(string expr)
    {
    }

    public virtual void SetSTAttribute(string expr)
    {
    }

    public virtual void TemplateExpr(string expr)
    {
    }

    public virtual void Text(string text)
    {
    }
}
