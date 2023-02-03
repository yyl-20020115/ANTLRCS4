/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.codegen.model.decl;

namespace org.antlr.v4.codegen.model.chunk;

public class NonLocalAttrRef : SymbolRefChunk
{
    public readonly string ruleName;
    public readonly int ruleIndex;

    public NonLocalAttrRef(StructDecl ctx, string ruleName, string name, string escapedName, int ruleIndex) : base(ctx, name, escapedName)
    {
        this.ruleName = ruleName;
        this.ruleIndex = ruleIndex;
    }
}
