/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using Antlr4.StringTemplate;
using org.antlr.v4.codegen.model.decl;

namespace org.antlr.v4.codegen.model.chunk;


public class ActionTemplate : ActionChunk
{
    public readonly Template st;

    public ActionTemplate(StructDecl ctx, Template st) : base(ctx) => this.st = st;
}
