/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime.misc;
using org.antlr.v4.runtime.tree;

namespace org.antlr.v4.test.tool;

public class InterpreterTreeTextProvider : TreeTextProvider
{
	public readonly List<string> ruleNames;
    public InterpreterTreeTextProvider(string[] ruleNames)
		=> this.ruleNames = Arrays.AsList(ruleNames);

    ////@Override
    public string GetText(Tree node) {
		if ( node==null ) return "null";
		var nodeText = Trees.getNodeText(node, ruleNames);
		if ( node is ErrorNode) {
			return "<error "+nodeText+">";
		}
		return nodeText;
	}
}
