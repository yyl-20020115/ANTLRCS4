/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

namespace org.antlr.v4.runtime.tree.xpath;

public class XPathTokenAnywhereElement : XPathElement
{
    protected int tokenType;
    public XPathTokenAnywhereElement(string tokenName, int tokenType) : base(tokenName)
    {
        this.tokenType = tokenType;
    }

    public override ICollection<ParseTree> Evaluate(ParseTree t) 
        => Trees.findAllTokenNodes(t, tokenType);
}
