/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

namespace org.antlr.v4.codegen.model.decl;

/** {@code public Token X() { }} */
public class ContextTokenGetterDecl : ContextGetterDecl
{
    public readonly bool optional;

    public ContextTokenGetterDecl(OutputModelFactory factory, string name, bool optional) : base(factory, name) => this.optional = optional;
}
