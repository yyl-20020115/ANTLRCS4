/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using System.Xml.Linq;

namespace org.antlr.v4.codegen.model.decl;

/** */
public class RuleContextDecl : Decl
{
    public readonly string ctxName;
    public bool isImplicit;

    public RuleContextDecl(OutputModelFactory factory, string name, string ctxName) : base(factory, name)
    {
        this.ctxName = ctxName;
    }
}
