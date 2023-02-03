/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

namespace org.antlr.v4.codegen.model.decl;

/** {@code public XContext X() { }} */
public class ContextRuleGetterDecl : ContextGetterDecl
{
    public readonly string ctxName;
    public readonly bool optional;

    public ContextRuleGetterDecl(OutputModelFactory factory, string name, string ctxName, bool optional) : base(factory, name)
    {
        this.ctxName = ctxName;
        this.optional = optional;
    }
}
