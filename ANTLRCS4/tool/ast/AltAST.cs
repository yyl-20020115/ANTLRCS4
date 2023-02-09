/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.analysis;
using org.antlr.v4.runtime;

namespace org.antlr.v4.tool.ast;


/** Any ALT (which can be child of ALT_REWRITE node) */
public class AltAST : GrammarASTWithOptions
{
    public Alternative alt;

    /** If we transformed this alt from a left-recursive one, need info on it */
    public LeftRecursiveRuleAltInfo leftRecursiveAltInfo;

    /** If someone specified an outermost alternative label with #foo.
	 *  Token type will be ID.
	 */
    public GrammarAST altLabel;

    public AltAST(AltAST node) : base(node)
    {
        this.alt = node.alt;
        this.altLabel = node.altLabel;
        this.leftRecursiveAltInfo = node.leftRecursiveAltInfo;
    }

    public AltAST(Token t) : base(t) { }
    public AltAST(int type) : base(type) { }
    public AltAST(int type, Token t) : base(type, t) { }
    public AltAST(int type, Token t, string text) : base(type, t, text) { }

    public override AltAST DupNode() => new (this);

    public override object Visit(GrammarASTVisitor v) => v.Visit(this);
}
