/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.codegen.model.decl;

namespace org.antlr.v4.codegen.model.chunk;


/** */
public class ActionChunk : OutputModelObject
{
    /** Where is the ctx that defines attrs,labels etc... for this action? */
    public readonly StructDecl ctx;

    public ActionChunk(StructDecl ctx) => this.ctx = ctx;
}
