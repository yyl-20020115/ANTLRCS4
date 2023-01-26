/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

namespace org.antlr.v4.runtime.tree.xpath;

/**
 * Either {@code ID} at start of path or {@code ...//ID} in middle of path.
 */
public class XPathRuleAnywhereElement : XPathElement {
	protected int ruleIndex;
	public XPathRuleAnywhereElement(String ruleName, int ruleIndex):base(ruleName) {
		this.ruleIndex = ruleIndex;
	}

	
	public override ICollection<ParseTree> evaluate(ParseTree t) {
		return Trees.findAllRuleNodes(t, ruleIndex);
	}
}
