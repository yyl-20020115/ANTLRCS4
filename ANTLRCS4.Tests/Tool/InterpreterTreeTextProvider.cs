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
	public List<String> ruleNames;
	public InterpreterTreeTextProvider(String[] ruleNames) {this.ruleNames = Arrays.AsList(ruleNames);}

	////@Override
	public String getText(Tree node) {
		if ( node==null ) return "null";
		String nodeText = Trees.getNodeText(node, ruleNames);
		if ( node is ErrorNode) {
			return "<error "+nodeText+">";
		}
		return nodeText;
	}
}
