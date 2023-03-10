/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */
using org.antlr.v4.tool;

namespace org.antlr.v4.codegen.model;

/** The code associated with the outermost alternative of a rule.
 *  Sometimes we might want to treat them differently in the
 *  code generation.
 */
public class CodeBlockForOuterMostAlt : CodeBlockForAlt
{
    /**
	 * The label for the alternative; or null if the alternative is not labeled.
	 */
    public string altLabel;
    /**
	 * The alternative.
	 */
    public Alternative alt;

    public CodeBlockForOuterMostAlt(OutputModelFactory factory, Alternative alt) : base(factory)
    {
        this.alt = alt;
        altLabel = alt.ast.altLabel != null ? alt.ast.altLabel.Text : null;
    }
}
