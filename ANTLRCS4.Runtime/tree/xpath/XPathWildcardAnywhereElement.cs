/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

namespace org.antlr.v4.runtime.tree.xpath;

public class XPathWildcardAnywhereElement : XPathElement
{
    public XPathWildcardAnywhereElement() : base(XPath.WILDCARD) { }

    public override ICollection<ParseTree> Evaluate(ParseTree t)
    {
        if (invert) return new List<ParseTree>(); // !* is weird but valid (empty)
        return Trees.getDescendants(t);
    }
}
