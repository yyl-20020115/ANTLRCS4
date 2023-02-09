/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime.atn;
using org.antlr.v4.runtime.misc;
using System.Text;

namespace org.antlr.v4.runtime.tree;
public interface TreeTextProvider
{
    string GetText(Tree node);
}
/** A set of utility routines useful for all kinds of ANTLR trees. */
public static class Trees
{
    /** Print out a whole tree in LISP form. {@link #getNodeText} is used on the
	 *  node payloads to get the text for the nodes.  Detect
	 *  parse trees and extract data appropriately.
	 */
    public static string ToStringTree(Tree t)
    {
        return ToStringTree(t, (List<string>)null);
    }
    public static string ToStringTree(Tree t, TreeTextProvider nodeTextProvider)
    {
        if (t == null) return "null";
        var s = RuntimeUtils.EscapeWhitespace(nodeTextProvider.GetText(t), false);
        if (t.ChildCount == 0) return s;
        var buffer = new StringBuilder();
        buffer.Append('(');
        s = RuntimeUtils.EscapeWhitespace(nodeTextProvider.GetText(t), false);
        buffer.Append(s);
        buffer.Append(' ');
        for (int i = 0; i < t.ChildCount; i++)
        {
            if (i > 0) buffer.Append(' ');
            buffer.Append(ToStringTree(t.GetChild(i), nodeTextProvider));
        }
        buffer.Append(')');
        return buffer.ToString();
    }

    /** Print out a whole tree in LISP form. {@link #getNodeText} is used on the
	 *  node payloads to get the text for the nodes.  Detect
	 *  parse trees and extract data appropriately.
	 */
    public static string ToStringTree(Tree t, Parser recog)
    {
        var ruleNames = recog != null ? recog.RuleNames : null;
        var ruleNamesList = ruleNames?.ToList();
        return ToStringTree(t, ruleNamesList);
    }

    /** Print out a whole tree in LISP form. {@link #getNodeText} is used on the
	 *  node payloads to get the text for the nodes.
	 */
    public static string ToStringTree(Tree t, List<string> ruleNames)
    {
        var s = RuntimeUtils.EscapeWhitespace(GetNodeText(t, ruleNames), false);
        if (t.ChildCount == 0) return s;
        var buffer = new StringBuilder();
        buffer.Append('(');
        s = RuntimeUtils.EscapeWhitespace(GetNodeText(t, ruleNames), false);
        buffer.Append(s);
        buffer.Append(' ');
        for (int i = 0; i < t.ChildCount; i++)
        {
            if (i > 0) buffer.Append(' ');
            buffer.Append(ToStringTree(t.GetChild(i), ruleNames));
        }
        buffer.Append(')');
        return buffer.ToString();
    }

    public static string GetNodeText(Tree t, Parser recog)
    {
        var ruleNames = recog != null ? recog.RuleNames : null;
        List<string> ruleNamesList = ruleNames?.ToList();
        return GetNodeText(t, ruleNamesList);
    }

    public static string GetNodeText(Tree t, List<string> ruleNames)
    {
        if (ruleNames != null)
        {
            if (t is RuleContext context)
            {
                int ruleIndex = context.CurrentRuleContext.RuleIndex;
                var ruleName = ruleNames[(ruleIndex)];
                int altNumber = context.AltNumber;
                if (altNumber != ATN.INVALID_ALT_NUMBER)
                {
                    return ruleName + ":" + altNumber;
                }
                return ruleName;
            }
            else if (t is ErrorNode)
            {
                return t.ToString();
            }
            else if (t is TerminalNode)
            {
                Token symbol = ((TerminalNode)t).getSymbol();
                if (symbol != null)
                {
                    var s = symbol.Text;
                    return s;
                }
            }
        }
        // no recog for rule names
        Object payload = t.Payload;
        if (payload is Token)
        {
            return ((Token)payload).Text;
        }
        return t.Payload.ToString();
    }

    /** Return ordered list of all children of this node */
    public static List<Tree> getChildren(Tree t)
    {
        List<Tree> kids = new();
        for (int i = 0; i < t.ChildCount; i++)
        {
            kids.Add(t.GetChild(i));
        }
        return kids;
    }

    /** Return a list of all ancestors of this node.  The first node of
	 *  list is the root and the last is the parent of this node.
	 *
	 *  @since 4.5.1
	 */
    public static List<Tree> getAncestors<T>(Tree t) where T : Tree
    {
        if (t.Parent == null) return new();
        List<Tree> ancestors = new();
        t = t.Parent;
        while (t != null)
        {
            ancestors.Insert(0, t); // insert at start
            t = t.Parent;
        }
        return ancestors;
    }

    /** Return true if t is u's parent or a node on path to root from u.
	 *  Use == not equals().
	 *
	 *  @since 4.5.1
	 */
    public static bool isAncestorOf(Tree t, Tree u)
    {
        if (t == null || u == null || t.Parent == null) return false;
        Tree p = u.Parent;
        while (p != null)
        {
            if (t == p) return true;
            p = p.Parent;
        }
        return false;
    }

    public static ICollection<ParseTree> findAllTokenNodes(ParseTree t, int ttype)
    {
        return findAllNodes(t, ttype, true);
    }

    public static ICollection<ParseTree> findAllRuleNodes(ParseTree t, int ruleIndex)
    {
        return findAllNodes(t, ruleIndex, false);
    }

    public static List<ParseTree> findAllNodes(ParseTree t, int index, bool findTokens)
    {
        List<ParseTree> nodes = new();
        _findAllNodes<ParseTree>(t, index, findTokens, nodes);
        return nodes;
    }

    public static void _findAllNodes<T>(ParseTree t, int index, bool findTokens,
                                     List<ParseTree> nodes) where T : ParseTree
    {
        // check this node (the root) first
        if (findTokens && t is TerminalNode)
        {
            TerminalNode tnode = (TerminalNode)t;
            if (tnode.getSymbol().Type == index) nodes.Add(t);
        }
        else if (!findTokens && t is ParserRuleContext)
        {
            ParserRuleContext ctx = (ParserRuleContext)t;
            if (ctx.RuleIndex == index) nodes.Add(t);
        }
        // check children
        for (int i = 0; i < t.ChildCount; i++)
        {
            _findAllNodes<ParseTree>(t.GetChild(i), index, findTokens, nodes);
        }
    }

    /** Get all descendents; includes t itself.
	 *
	 * @since 4.5.1
 	 */
    public static List<ParseTree> getDescendants(ParseTree t)
    {
        List<ParseTree> nodes = new();
        nodes.Add(t);

        int n = t.ChildCount;
        for (int i = 0; i < n; i++)
        {
            nodes.AddRange(getDescendants(t.GetChild(i)));
        }
        return nodes;
    }

    /** @deprecated */
    //@Deprecated
    public static List<ParseTree> descendants(ParseTree t)
    {
        return getDescendants(t);
    }

    /** Find smallest subtree of t enclosing range startTokenIndex..stopTokenIndex
	 *  inclusively using postorder traversal.  Recursive depth-first-search.
	 *
	 *  @since 4.5.1
	 */
    public static ParserRuleContext getRootOfSubtreeEnclosingRegion(ParseTree t,
                                                                    int startTokenIndex, // inclusive
                                                                    int stopTokenIndex)  // inclusive
    {
        int n = t.ChildCount;
        for (int i = 0; i < n; i++)
        {
            ParseTree child = t.GetChild(i);
            ParserRuleContext r = getRootOfSubtreeEnclosingRegion(child, startTokenIndex, stopTokenIndex);
            if (r != null) return r;
        }
        if (t is ParserRuleContext)
        {
            ParserRuleContext r = (ParserRuleContext)t;
            if (startTokenIndex >= r.GetStart().TokenIndex && // is range fully contained in t?
                 (r.GetStop() == null || stopTokenIndex <= r.GetStop().TokenIndex))
            {
                // note: r.getStop()==null likely implies that we bailed out of parser and there's nothing to the right
                return r;
            }
        }
        return null;
    }

    /** Replace any subtree siblings of root that are completely to left
	 *  or right of lookahead range with a CommonToken(Token.INVALID_TYPE,"...")
	 *  node. The source interval for t is not altered to suit smaller range!
	 *
	 *  WARNING: destructive to t.
	 *
	 *  @since 4.5.1
	 */
    public static void stripChildrenOutOfRange(ParserRuleContext t,
                                               ParserRuleContext root,
                                               int startIndex,
                                               int stopIndex)
    {
        if (t == null) return;
        for (int i = 0; i < t.ChildCount; i++)
        {
            ParseTree child = t.GetChild(i);
            Interval range = child.SourceInterval;
            if (child is ParserRuleContext && (range.b < startIndex || range.a > stopIndex))
            {
                if (isAncestorOf(child, root))
                { // replace only if subtree doesn't have displayed root
                    CommonToken abbrev = new CommonToken(Token.INVALID_TYPE, "...");
                    t.children[i] = new TerminalNodeImpl(abbrev);
                }
            }
        }
    }

    /** Return first node satisfying the pred
	 *
 	 *  @since 4.5.1
	 */
    public static Tree findNodeSuchThat(Tree t, misc.Predicate<Tree> pred)
    {
        if (pred.Test(t)) return t;

        if (t == null) return null;

        int n = t.ChildCount;
        for (int i = 0; i < n; i++)
        {
            Tree u = findNodeSuchThat(t.GetChild(i), pred);
            if (u != null) return u;
        }
        return null;
    }

}
