/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

namespace org.antlr.v4.runtime.tree.xpath;


public class XPathTokenElement : XPathElement
{
    protected int tokenType;
    public XPathTokenElement(string tokenName, int tokenType) : base(tokenName) => this.tokenType = tokenType;

    //@Override
    public override ICollection<ParseTree> Evaluate(ParseTree t)
    {
        // return all children of t that match nodeName
        List<ParseTree> nodes = new();
        foreach (Tree c in Trees.getChildren(t))
            if (c is TerminalNode tnode && ((tnode.getSymbol().Type == tokenType && !invert) ||
                     (tnode.getSymbol().Type != tokenType && invert)))
                nodes.Add(tnode);
        return nodes;
    }
}
