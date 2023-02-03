/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime.misc;

namespace org.antlr.v4.codegen.model.decl;


public class CodeBlock : SrcOp
{
    public readonly int codeBlockLevel;
    public readonly int treeLevel;

    [ModelElement]
    public OrderedHashSet<Decl> locals;
    [ModelElement]
    public List<SrcOp> preamble;
    [ModelElement]
    public List<SrcOp> ops;

    public CodeBlock(OutputModelFactory factory) : base(factory)
    {
    }

    public CodeBlock(OutputModelFactory factory, int treeLevel, int codeBlockLevel) : base(factory)
    {
        this.treeLevel = treeLevel;
        this.codeBlockLevel = codeBlockLevel;
    }

    /** Add local var decl */
    public void AddLocalDecl(Decl d)
    {
        locals ??= new ();
        locals.add(d);
        d.isLocal = true;
    }

    public void AddPreambleOp(SrcOp op)
    {
        preamble ??= new();
        preamble.Add(op);
    }

    public void AddOp(SrcOp op)
    {
        ops ??= new();
        ops.Add(op);
    }

    public void InsertOp(int i, SrcOp op)
    {
        ops ??= new();
        ops.Insert(i, op);
    }

    public void AddOps(List<SrcOp> ops)
    {
        this.ops ??= new();
        this.ops.AddRange(ops);
    }
}
