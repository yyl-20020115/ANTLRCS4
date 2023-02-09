/*
 [The "BSD license"]
 Copyright (c) 2005-2009 Terence Parr
 All rights reserved.

 Redistribution and use in source and binary forms, with or without
 modification, are permitted provided that the following conditions
 are met:
 1. Redistributions of source code must retain the above copyright
     notice, this list of conditions and the following disclaimer.
 2. Redistributions in binary form must reproduce the above copyright
     notice, this list of conditions and the following disclaimer in the
     documentation and/or other materials provided with the distribution.
 3. The name of the author may not be used to endorse or promote products
     derived from this software without specific prior written permission.

 THIS SOFTWARE IS PROVIDED BY THE AUTHOR ``AS IS'' AND ANY EXPRESS OR
 IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
 OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.
 IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY DIRECT, INDIRECT,
 INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT
 NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
 DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
 THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
 THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */
using org.antlr.runtime.misc;
using org.antlr.runtime.tree;
using org.antlr.v4.runtime.dfa;
using org.antlr.v4.runtime.tree;
using org.antlr.v4.runtime;
using System.Xml.Linq;

namespace org.antlr.runtime.tree;

/** Return a node stream from a doubly-linked tree whose nodes
 *  know what child index they are.  No remove() is supported.
 *
 *  Emit navigation nodes (DOWN, UP, and EOF) to let show tree structure.
 */
public class TreeIterator
{
    protected TreeAdaptor adaptor;
    protected Object root;
    protected Object tree;
    protected bool firstTime = true;

    // navigation nodes to return during walk and at end
    public Object up;
    public Object down;
    public Object eof;

    /** If we emit UP/DOWN nodes, we need to spit out multiple nodes per
     *  next() call.
     */
    protected FastQueue<Object> nodes;

    public TreeIterator(Object tree)
        : this(new CommonTreeAdaptor(), tree)
    {
    }

    public TreeIterator(TreeAdaptor adaptor, Object tree)
    {
        this.adaptor = adaptor;
        this.tree = tree;
        this.root = tree;
        nodes = new FastQueue<Object>();
        down = adaptor.Create(Token.DOWN, "DOWN");
        up = adaptor.Create(Token.UP, "UP");
        eof = adaptor.Create(Token.EOF, "EOF");
    }
    public void reset()
    {
        firstTime = true;
        tree = root;
        nodes.Clear();
    }
    public bool hasNext()
    {
        if (firstTime) return root != null;
        if (nodes != null && nodes.Count > 0) return true;
        if (tree == null) return false;
        if (adaptor.GetChildCount(tree) > 0) return true;
        return adaptor.GetParent(tree) != null; // back at root?
    }

    public Object next()
    {
        if (firstTime)
        { // initial condition
            firstTime = false;
            if (adaptor.GetChildCount(tree) == 0)
            { // single node tree (special)
                nodes.Add(eof);
                return tree;
            }
            return tree;
        }
        // if any queued up, use those first
        if (nodes != null && nodes.Count > 0) return nodes.Remove();

        // no nodes left?
        if (tree == null) return eof;

        // next node will be child 0 if any children
        if (adaptor.GetChildCount(tree) > 0)
        {
            tree = adaptor.GetChild(tree, 0);
            nodes.Add(tree); // real node is next after DOWN
            return down;
        }
        // if no children, look for next sibling of tree or ancestor
        Object parent = adaptor.GetParent(tree);
        // while we're out of siblings, keep popping back up towards root
        while (parent != null &&
                adaptor.GetChildIndex(tree) + 1 >= adaptor.GetChildCount(parent))
        {
            nodes.Add(up); // we're moving back up
            tree = parent;
            parent = adaptor.GetParent(tree);
        }
        // no nodes left?
        if (parent == null)
        {
            tree = null; // back at root? nothing left then
            nodes.Add(eof); // add to queue, might have UP nodes in there
            return nodes.Remove();
        }

        // must have found a node with an unvisited sibling
        // move to it and return it
        int nextSiblingIndex = adaptor.GetChildIndex(tree) + 1;
        tree = adaptor.GetChild(parent, nextSiblingIndex);
        nodes.Add(tree); // add to queue, might have UP nodes in there
        return nodes.Remove();
    }

    public void remove() { throw new UnsupportedOperationException(); }
}
