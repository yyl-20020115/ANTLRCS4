﻿/*
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
using org.antlr.v4.runtime.dfa;
using org.antlr.v4.runtime.misc;
using org.antlr.v4.runtime.tree.pattern;
using org.antlr.v4.runtime.tree;
using System.Text;

namespace org.antlr.runtime.tree;

/** A generic tree implementation with no payload.  You must subclass to
 *  actually have any user data.  ANTLR v3 uses a list of children approach
 *  instead of the child-sibling approach in v2.  A flat tree (a list) is
 *  an empty node whose children represent the list.  An empty, but
 *  non-null node is called "nil".
 */
public abstract class BaseTree : Tree
{

    protected List<Tree> children;

    public BaseTree()
    {
    }

    /** Create a new node from an existing node does nothing for BaseTree
     *  as there are no fields other than the children list, which cannot
     *  be copied as the children are not considered part of this node.
     */
    public BaseTree(Tree node)
    {
    }

    //@Override
    public Tree getChild(int i)
    {
        if (children == null || i >= children.Count)
        {
            return null;
        }
        return (Tree)children[(i)];
    }

    /** Get the children internal List; note that if you directly mess with
     *  the list, do so at your own risk.
     */
    public List<Tree> getChildren()
    {
        return children;
    }

    public Tree getFirstChildWithType(int type)
    {
        for (int i = 0; children != null && i < children.Count; i++)
        {
            Tree t = (Tree)children[i];
            if (t.getType() == type)
            {
                return t;
            }
        }
        return null;
    }

    //@Override
    public int getChildCount()
    {
        if (children == null)
        {
            return 0;
        }
        return children.Count;
    }

    /** Add t as child of this node.
     *
     *  Warning: if t has no children, but child does
     *  and child isNil then this routine moves children to t via
     *  t.children = child.children; i.e., without copying the array.
     */
    //@Override
    public void addChild(Tree t)
    {
        //Console.Out.WriteLine("add child "+t.toStringTree()+" "+this.toStringTree());
        //Console.Out.WriteLine("existing children: "+children);
        if (t == null)
        {
            return; // do nothing upon addChild(null)
        }
        BaseTree childTree = (BaseTree)t;
        if (childTree.isNil())
        { // t is an empty node possibly with children
            if (this.children != null && this.children == childTree.children)
            {
                throw new RuntimeException("attempt to add child list to itself");
            }
            // just add all of childTree's children to this
            if (childTree.children != null)
            {
                if (this.children != null)
                { // must copy, this has children already
                    int n = childTree.children.Count;
                    for (int i = 0; i < n; i++)
                    {
                        Tree c = (Tree)childTree.children[(i)];
                        this.children.Add(c);
                        // handle double-link stuff for each child of nil root
                        c.setParent(this);
                        c.setChildIndex(children.Count - 1);
                    }
                }
                else
                {
                    // no children for this but t has children; just set pointer
                    // call general freshener routine
                    this.children = childTree.children;
                    this.freshenParentAndChildIndexes();
                }
            }
        }
        else
        { // child is not nil (don't care about children)
            if (children == null)
            {
                children = createChildrenList(); // create children list on demand
            }
            children.Add(t);
            childTree.setParent(this);
            childTree.setChildIndex(children.Count - 1);
        }
        // Console.Out.WriteLine("now children are: "+children);
    }

    /** Add all elements of kids list as children of this node */
    public void addChildren(List<Tree> kids)
    {
        for (int i = 0; i < kids.Count; i++)
        {
            Tree t = kids[(i)];
            addChild(t);
        }
    }

    //@Override
    public void setChild(int i, Tree t)
    {
        if (t == null)
        {
            return;
        }
        if (t.isNil())
        {
            throw new ArgumentException("Can't set single child to a list");
        }
        if (children == null)
        {
            children = createChildrenList();
        }
        children[i]=t;
        t.setParent(this);
        t.setChildIndex(i);
    }

    /** Insert child t at child position i (0..n-1) by shifting children
        i+1..n-1 to the right one position. Set parent / indexes properly
        but does NOT collapse nil-rooted t's that come in here like addChild.
     */
    public void insertChild(int i, Tree t)
    {
        if (i < 0 || i > getChildCount())
        {
            throw new IndexOutOfRangeException(i + " out or range");
        }

        if (children == null)
        {
            children = createChildrenList();
        }

        children.Insert(i, t);
        // walk others to increment their child indexes
        // set index, parent of this one too
        this.freshenParentAndChildIndexes(i);
    }

    //@Override
    public Object deleteChild(int i)
    {
        if (children == null)
        {
            return null;
        }
        Tree killed = children[i];
        children.RemoveAt(i);
        // walk rest and decrement their child indexes
        this.freshenParentAndChildIndexes(i);
        return killed;
    }

    /** Delete children from start to stop and replace with t even if t is
     *  a list (nil-root tree).  num of children can increase or decrease.
     *  For huge child lists, inserting children can force walking rest of
     *  children to set their childindex; could be slow.
     */
    //@Override
    public void replaceChildren(int startChildIndex, int stopChildIndex, Object t)
    {
        /*
        Console.Out.WriteLine("replaceChildren "+startChildIndex+", "+stopChildIndex+
                           " with "+((BaseTree)t).toStringTree());
        Console.Out.WriteLine("in="+toStringTree());
        */
        if (children == null)
        {
            throw new ArgumentException("indexes invalid; no children in list");
        }
        int replacingHowMany = stopChildIndex - startChildIndex + 1;
        int replacingWithHowMany;
        BaseTree newTree = (BaseTree)t;
        List<Tree> newChildren;
        // normalize to a list of children to add: newChildren
        if (newTree.isNil())
        {
            newChildren = newTree.children;
        }
        else
        {
            newChildren = new (1);
            newChildren.Add(newTree);
        }
        replacingWithHowMany = newChildren.Count;
        int numNewChildren = newChildren.Count;
        int delta = replacingHowMany - replacingWithHowMany;
        // if same number of nodes, do direct replace
        if (delta == 0)
        {
            int j = 0; // index into new children
            for (int i = startChildIndex; i <= stopChildIndex; i++)
            {
                BaseTree child = (BaseTree)newChildren[j];
                children[i] = child;
                child.setParent(this);
                child.setChildIndex(i);
                j++;
            }
        }
        else if (delta > 0)
        { // fewer new nodes than there were
          // set children and then delete extra
            for (int j = 0; j < numNewChildren; j++)
            {
                children.set(startChildIndex + j, newChildren[(j)]);
            }
            int indexToDelete = startChildIndex + numNewChildren;
            for (int c = indexToDelete; c <= stopChildIndex; c++)
            {
                // delete same index, shifting everybody down each time
                children.RemoveAt(indexToDelete);
            }
            freshenParentAndChildIndexes(startChildIndex);
        }
        else
        { // more new nodes than were there before
          // fill in as many children as we can (replacingHowMany) w/o moving data
            for (int j = 0; j < replacingHowMany; j++)
            {
                children[(startChildIndex + j)]=newChildren[(j)];
            }
            int numToInsert = replacingWithHowMany - replacingHowMany;
            for (int j = replacingHowMany; j < replacingWithHowMany; j++)
            {
                children.Insert(startChildIndex + j, newChildren[(j)]);
            }
            freshenParentAndChildIndexes(startChildIndex);
        }
        //Console.Out.WriteLine("out="+toStringTree());
    }

    /** Override in a subclass to change the impl of children list */
    protected List<Object> createChildrenList()
    {
        return new ();
    }

    //@Override
    public bool isNil()
    {
        return false;
    }

    /** Set the parent and child index values for all child of t */
    //@Override
    public void freshenParentAndChildIndexes()
    {
        freshenParentAndChildIndexes(0);
    }

    public void freshenParentAndChildIndexes(int offset)
    {
        int n = getChildCount();
        for (int c = offset; c < n; c++)
        {
            Tree child = getChild(c);
            child.setChildIndex(c);
            child.setParent(this);
        }
    }

    public void freshenParentAndChildIndexesDeeply()
    {
        freshenParentAndChildIndexesDeeply(0);
    }

    public void freshenParentAndChildIndexesDeeply(int offset)
    {
        int n = getChildCount();
        for (int c = offset; c < n; c++)
        {
            BaseTree child = (BaseTree)getChild(c);
            child.setChildIndex(c);
            child.setParent(this);
            child.freshenParentAndChildIndexesDeeply();
        }
    }

    public void sanityCheckParentAndChildIndexes()
    {
        sanityCheckParentAndChildIndexes(null, -1);
    }

    public void sanityCheckParentAndChildIndexes(Tree parent, int i)
    {
        if (parent != this.getParent())
        {
            throw new IllegalStateException("parents don't match; expected " + parent + " found " + this.getParent());
        }
        if (i != this.getChildIndex())
        {
            throw new IllegalStateException("child indexes don't match; expected " + i + " found " + this.getChildIndex());
        }
        int n = this.getChildCount();
        for (int c = 0; c < n; c++)
        {
            CommonTree child = (CommonTree)this.getChild(c);
            child.sanityCheckParentAndChildIndexes(this, c);
        }
    }

    /** BaseTree doesn't track child indexes. */
    //@Override
    public int getChildIndex()
    {
        return 0;
    }
    //@Override
    public void setChildIndex(int index)
    {
    }

    /** BaseTree doesn't track parent pointers. */
    //@Override
    public Tree getParent()
    {
        return null;
    }

    //@Override
    public void setParent(Tree t)
    {
    }

    /** Walk upwards looking for ancestor with this token type. */
    //@Override
    public bool hasAncestor(int ttype) { return getAncestor(ttype) != null; }

    /** Walk upwards and get first ancestor with this token type. */
    //@Override
    public Tree getAncestor(int ttype)
    {
        Tree t = this;
        t = t.getParent();
        while (t != null)
        {
            if (t.getType() == ttype) return t;
            t = t.getParent();
        }
        return null;
    }

    /** Return a list of all ancestors of this node.  The first node of
     *  list is the root and the last is the parent of this node.
     */
    //@Override
    public List<Tree> getAncestors()
    {
        if (getParent() == null) return null;
        List<Tree> ancestors = new ();
        Tree t = this;
        t = t.getParent();
        while (t != null)
        {
            ancestors.Insert(0, t); // insert at start
            t = t.getParent();
        }
        return ancestors;
    }

    /** Print out a whole tree not just a node */
    //@Override
    public String toStringTree()
    {
        if (children == null || children.Count==0)
        {
            return this.toString();
        }
        StringBuilder buf = new StringBuilder();
        if (!isNil())
        {
            buf.Append("(");
            buf.Append(this.toString());
            buf.Append(' ');
        }
        for (int i = 0; children != null && i < children.Count; i++)
        {
            Tree t = (Tree)children[(i)];
            if (i > 0)
            {
                buf.Append(' ');
            }
            buf.Append(t.toStringTree());
        }
        if (!isNil())
        {
            buf.Append(")");
        }
        return buf.ToString();
    }

    //@Override
    public virtual int getLine()
    {
        return 0;
    }

    //@Override
    public virtual int getCharPositionInLine()
    {
        return 0;
    }

    public object getPayload()
    {
        throw new NotImplementedException();
    }

    /** Override to say how a node (not a tree) should look as text */
    //@Override
    //public abstract String toString();
}
