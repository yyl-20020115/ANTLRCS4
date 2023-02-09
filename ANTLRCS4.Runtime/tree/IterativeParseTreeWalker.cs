/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime.misc;
using org.antlr.v4.runtime.tree;

namespace org.antlr.v4.runtime.tree;

/**
 * An iterative (read: non-recursive) pre-order and post-order tree walker that
 * doesn't use the thread stack but heap-based stacks. Makes it possible to
 * process deeply nested parse trees.
 */
public class IterativeParseTreeWalker : ParseTreeWalker
{

    public override void Walk(ParseTreeListener listener, ParseTree t)
    {

        ArrayDeque<ParseTree> nodeStack = new();
        IntegerStack indexStack = new();

        ParseTree currentNode = t;
        int currentIndex = 0;

        while (currentNode != null)
        {

            // pre-order visit
            if (currentNode is ErrorNode)
            {
                listener.VisitErrorNode((ErrorNode)currentNode);
            }
            else if (currentNode is TerminalNode)
            {
                listener.VisitTerminal((TerminalNode)currentNode);
            }
            else
            {
                RuleNode r = (RuleNode)currentNode;
                EnterRule(listener, r);
            }

            // Move down to first child, if exists
            if (currentNode.ChildCount > 0)
            {
                nodeStack.Push(currentNode);
                indexStack.Push(currentIndex);
                currentIndex = 0;
                currentNode = currentNode.GetChild(0);
                continue;
            }

            // No child nodes, so walk tree
            do
            {

                // post-order visit
                if (currentNode is RuleNode node)
                {
                    ExitRule(listener, node);
                }

                // No parent, so no siblings
                if (nodeStack.IsEmpty)
                {
                    currentNode = null;
                    currentIndex = 0;
                    break;
                }

                // Move to next sibling if possible
                currentNode = nodeStack.Peek().GetChild(++currentIndex);
                if (currentNode != null)
                {
                    break;
                }

                // No next, sibling, so move up
                currentNode = nodeStack.Pop();
                currentIndex = indexStack.Pop();

            } while (currentNode != null);
        }
    }
}
