/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

namespace org.antlr.v4.codegen.model.decl;


/** */
public class AttributeDecl : Decl
{
    public readonly string type;
    public readonly string initValue;
    public AttributeDecl(OutputModelFactory factory, tool.Attribute a) 
        : base(factory, a.name, a.decl)
    {
        this.type = a.type;
        this.initValue = a.initValue;
    }
}
