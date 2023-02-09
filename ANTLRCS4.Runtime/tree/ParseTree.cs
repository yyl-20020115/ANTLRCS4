/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

namespace org.antlr.v4.runtime.tree;

/** An interface to access the tree of {@link RuleContext} objects created
 *  during a parse that makes the data structure look like a simple parse tree.
 *  This node represents both internal nodes, rule invocations,
 *  and leaf nodes, token matches.
 *
 *  <p>The payload is either a {@link Token} or a {@link RuleContext} object.</p>
 */
public interface ParseTree : SyntaxTree
{
    // the following methods narrow the return type; they are not additional methods
    new ParseTree Parent { get; }

    new ParseTree GetChild(int i);

    /** The {@link ParseTreeVisitor} needs a double dispatch method. */
    T Accept<T>(ParseTreeVisitor<T> visitor);

    /** Return the combined text of all leaf nodes. Does not get any
	 *  off-channel tokens (if any) so won't return whitespace and
	 *  comments if they are sent to parser on hidden channel.
	 */
    new string Text { get; }

    /** Specialize toStringTree so that it can print out more information
	 * 	based upon the parser.
	 */
    string ToStringTree(Parser parser);
}
