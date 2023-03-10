/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

namespace org.antlr.v4.runtime.tree;

public class ParseTreeWalker
{
    public static readonly ParseTreeWalker DEFAULT = new();


    /**
	 * Performs a walk on the given parse tree starting at the root and going down recursively
	 * with depth-first search. On each node, {@link ParseTreeWalker#enterRule} is called before
	 * recursively walking down into child nodes, then
	 * {@link ParseTreeWalker#exitRule} is called after the recursive call to wind up.
	 * @param listener The listener used by the walker to process grammar rules
	 * @param t The parse tree to be walked on
	 */
    public virtual void Walk(ParseTreeListener listener, ParseTree t)
    {
        if (t is ErrorNode node)
        {
            listener.VisitErrorNode(node);
            return;
        }
        else if (t is TerminalNode node1)
        {
            listener.VisitTerminal(node1);
            return;
        }
        var r = (RuleNode)t;
        EnterRule(listener, r);
        int n = r.ChildCount;
        for (int i = 0; i < n; i++)
        {
            Walk(listener, r.GetChild(i));
        }
        ExitRule(listener, r);
    }

    /**
	 * Enters a grammar rule by first triggering the generic event {@link ParseTreeListener#enterEveryRule}
	 * then by triggering the event specific to the given parse tree node
	 * @param listener The listener responding to the trigger events
	 * @param r The grammar rule containing the rule context
	 */
    protected void EnterRule(ParseTreeListener listener, RuleNode r)
    {
        var ctx = (ParserRuleContext)r.CurrentRuleContext;
        listener.EnterEveryRule(ctx);
        ctx.EnterRule(listener);
    }


    /**
	 * Exits a grammar rule by first triggering the event specific to the given parse tree node
	 * then by triggering the generic event {@link ParseTreeListener#exitEveryRule}
	 * @param listener The listener responding to the trigger events
	 * @param r The grammar rule containing the rule context
	 */
    protected void ExitRule(ParseTreeListener listener, RuleNode r)
    {
        var ctx = (ParserRuleContext)r.CurrentRuleContext;
        ctx.ExitRule(listener);
        listener.ExitEveryRule(ctx);
    }
}
