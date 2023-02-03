/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.codegen.model.decl;
using org.antlr.v4.tool.ast;

namespace org.antlr.v4.codegen.model;

/** */
public abstract class SrcOp : OutputModelObject
{
    /** Used to create unique var names etc... */
    public int uniqueID; // TODO: do we need?

    /** All operations know in which block they live:
	 *
	 *  	CodeBlock, CodeBlockForAlt
	 *
	 *  Templates might need to know block nesting level or find
	 *  a specific declaration, etc...
	 */
    public CodeBlock enclosingBlock;

    public RuleFunction enclosingRuleRunction;

    public SrcOp(OutputModelFactory factory) : this(factory, null) {; }
    public SrcOp(OutputModelFactory factory, GrammarAST ast) : base(factory, ast)
    {
        if (ast != null) uniqueID = ast.token.getTokenIndex();
        enclosingBlock = factory.GetCurrentBlock();
        enclosingRuleRunction = factory.GetCurrentRuleFunction();
    }

    /** Walk upwards in model tree, looking for outer alt's code block */
    public CodeBlockForOuterMostAlt GetOuterMostAltCodeBlock()
    {
        if (this is CodeBlockForOuterMostAlt alt)
            return alt;
        var p = enclosingBlock;
        while (p != null)
        {
            if (p is CodeBlockForOuterMostAlt alt1)
                return alt1;
            p = p.enclosingBlock;
        }
        return null;
    }

    /** Return label alt or return name of rule */
    public string GetContextName()
    {
        var alt = GetOuterMostAltCodeBlock();
        if (alt != null && alt.altLabel != null)
            return alt.altLabel;
        return enclosingRuleRunction.name;
    }
}
