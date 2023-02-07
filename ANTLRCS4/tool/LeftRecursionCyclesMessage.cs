/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime;

namespace org.antlr.v4.tool;


public class LeftRecursionCyclesMessage : ANTLRMessage {
	public LeftRecursionCyclesMessage(String fileName, ICollection<HashSet<Rule>> cycles) 
	: base(ErrorType.LEFT_RECURSION_CYCLES, getStartTokenOfFirstRule(cycles), cycles)
    {
		this.fileName = fileName;
	}

	protected static Token getStartTokenOfFirstRule(ICollection<HashSet<Rule>> cycles) {
	    if (cycles == null) {
	        return null;
	    }

	    foreach (ICollection<Rule> collection in cycles) {
	        if (collection == null) {
	            return null;
	        }

	        foreach (Rule rule in collection) {
	            if (rule.ast != null) {
	                return rule.ast.Token;
	            }
	        }
	    }
		return null;
	}
}
