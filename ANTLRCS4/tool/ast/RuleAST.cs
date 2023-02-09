/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.parse;
using org.antlr.v4.runtime;

namespace org.antlr.v4.tool.ast;

public class RuleAST : GrammarASTWithOptions
{
    public RuleAST(RuleAST node) : base(node) { }

    public RuleAST(Token t) : base(t) { }
    public RuleAST(int type) : base(type) { }

    public bool IsLexerRule
    {
        get
        {
            var name = RuleName;
            return name != null && Grammar.IsTokenName(name);
        }
    }

    public string RuleName
    {
        get
        {
            var nameNode = (GrammarAST)GetChild(0);
            if (nameNode != null) return nameNode.Text;
            return null;
        }
    }

    public override RuleAST DupNode() => new (this);

    public ActionAST LexerAction
    {
        get
        {
            var blk = GetFirstChildWithType(ANTLRParser.BLOCK);
            if (blk.ChildCount == 1)
            {
                var onlyAlt = blk.GetChild(0);
                var lastChild = onlyAlt.GetChild(onlyAlt.ChildCount - 1);
                if (lastChild.Type == ANTLRParser.ACTION)
                {
                    return (ActionAST)lastChild;
                }
            }
            return null;
        }
    }

    public override object Visit(GrammarASTVisitor v) => v.Visit(this);
}
