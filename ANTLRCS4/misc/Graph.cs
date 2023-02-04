/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */
using org.antlr.v4.runtime.misc;

namespace org.antlr.v4.misc;


/** A generic graph with edges; Each node as a single Object payload.
 *  This is only used to topologically sort a list of file dependencies
 *  at the moment.
 */
public class Graph<T>
{
    public class Node<T>
    {
        public T payload;

        public List<Node<T>> edges = new(); // points at which nodes?

        public Node(T payload) => this.payload = payload;

        public void AddEdge(Node<T> n)
        {
            if (!edges.Contains(n)) edges.Add(n);
        }

        public override string ToString() => payload.ToString();
    }

    /** Map from node payload to node containing it */
    protected Dictionary<T, Node<T>> nodes = new();

    public void AddEdge(T a, T b)
    {
        //Console.Out.WriteLine("add edge "+a+" to "+b);
        var a_node = GetNode(a);
        var b_node = GetNode(b);
        a_node.AddEdge(b_node);
    }

    public Node<T> GetNode(T a)
    {
        if (nodes.TryGetValue(a, out var existing)) return existing;
        var n = new Node<T>(a);
        nodes[a] = n;
        return n;
    }

    /** DFS-based topological sort.  A valid sort is the reverse of
	 *  the post-order DFA traversal.  Amazingly simple but true.
	 *  For sorting, I'm not following convention here since ANTLR
	 *  needs the opposite.  Here's what I assume for sorting:
	 *
	 *    If there exists an edge u -&gt; v then u depends on v and v
	 *    must happen before u.
	 *
	 *  So if this gives nonreversed postorder traversal, I get the order
	 *  I want.
	 */
    public List<T> Sort()
    {
        var visited = new OrderedHashSet<Node<T>>();
        List<T> sorted = new();
        while (visited.Count < nodes.Count)
        {
            // pick any unvisited node, n
            Node<T> n = null;
            foreach (var tNode in nodes.Values)
            {
                n = tNode;
                if (!visited.Contains(n)) break;
            }
            if (n != null)
            { // if at least one unvisited
                DFS(n, visited, sorted);
            }
        }
        return sorted;
    }

    public void DFS(Node<T> n, HashSet<Node<T>> visited, List<T> sorted)
    {
        if (visited.Contains(n)) return;
        visited.Add(n);
        if (n.edges != null)
        {
            foreach (Node<T> target in n.edges)
            {
                DFS(target, visited, sorted);
            }
        }
        sorted.Add(n.payload);
    }
}
