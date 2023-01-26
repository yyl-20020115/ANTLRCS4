/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

namespace org.antlr.v4.runtime.tree.xpath;


public class XPathRuleElement : XPathElement {
	protected int ruleIndex;
	public XPathRuleElement(String ruleName, int ruleIndex) :base(ruleName){
		this.ruleIndex = ruleIndex;
	}

	//@Override
	public override ICollection<ParseTree> evaluate(ParseTree t) {
				// return all children of t that match nodeName
		List<ParseTree> nodes = new ();
		foreach (Tree c in Trees.getChildren(t)) {
            if (c is ParserRuleContext ctx)
            {
                if ((ctx.getRuleIndex() == ruleIndex && !invert) ||
                     (ctx.getRuleIndex() != ruleIndex && invert))
                {
                    nodes.Add(ctx);
                }
            }
        }
		return nodes;
	}
}
