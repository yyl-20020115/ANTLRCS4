/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

namespace org.antlr.v4.runtime.tree;

/**
 * Associate a property with a parse tree node. Useful with parse tree listeners
 * that need to associate values with particular tree nodes, kind of like
 * specifying a return value for the listener event method that visited a
 * particular node. Example:
 *
 * <pre>
 * ParseTreeProperty&lt;Integer&gt; values = new ParseTreeProperty&lt;Integer&gt;();
 * values.put(tree, 36);
 * int x = values.get(tree);
 * values.removeFrom(tree);
 * </pre>
 *
 * You would make one decl (values here) in the listener and use lots of times
 * in your event methods.
 */
public class ParseTreeProperty<V> {
	protected Dictionary<ParseTree, V> annotations = new ();

	public V get(ParseTree node)
	{
        if (annotations.TryGetValue(node, out var v))
        {
            return v;
        }
		return default;
    }
    public void put(ParseTree node, V value) { annotations[node]= value; }
	public V removeFrom(ParseTree node)
	{
		if(annotations.TryGetValue(node, out var v))
		{
			annotations.Remove(node);
			return v;
		}
		return default;
	}
}
