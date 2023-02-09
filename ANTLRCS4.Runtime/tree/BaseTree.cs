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
using org.antlr.v4.runtime.dfa;
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

    protected List<Tree> children = new();

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

    public Tree GetChild(int i) => children == null || i >= children.Count ? null : children[i];

    /** Get the children internal List; note that if you directly mess with
     *  the list, do so at your own risk.
     */
    public List<Tree> GetChildren() => children;

    public Tree GetFirstChildWithType(int type)
    {
        for (int i = 0; children != null && i < children.Count; i++)
        {
            var t = children[i];
            if (t.Type == type) return t;
        }
        return null;
    }

    public int ChildCount => children == null ? 0 : children.Count;

    /** Add t as child of this node.
     *
     *  Warning: if t has no children, but child does
     *  and child isNil then this routine moves children to t via
     *  t.children = child.children; i.e., without copying the array.
     */
    public virtual void AddChild(Tree t)
    {
        //Console.Out.WriteLine("add child "+t.toStringTree()+" "+this.toStringTree());
        //Console.Out.WriteLine("existing children: "+children);
        if (t == null)
        {
            return; // do nothing upon addChild(null)
        }
        var childTree = (BaseTree)t;
        if (childTree.IsNil)
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
                        var c = childTree.children[(i)];
                        this.children.Add(c);
                        // handle double-link stuff for each child of nil root
                        c.                        // handle double-link stuff for each child of nil root
                        Parent = this;
                        c.ChildIndex = children.Count - 1;
                    }
                }
                else
                {
                    // no children for this but t has children; just set pointer
                    // call general freshener routine
                    this.children = childTree.children;
                    this.FreshenParentAndChildIndexes();
                }
            }
        }
        else
        { // child is not nil (don't care about children)
            children ??= CreateChildrenList().Cast<Tree>().ToList(); // create children list on demand
            children.Add(t);
            childTree.SetParent(this);
            childTree.ChildIndex = children.Count - 1;
        }
        // Console.Out.WriteLine("now children are: "+children);
    }

    /** Add all elements of kids list as children of this node */
    public virtual void AddChildren(List<Tree> kids)
    {
        for (int i = 0; i < kids.Count; i++)
        {
            var t = kids[(i)];
            AddChild(t);
        }
    }

    public virtual void SetChild(int i, Tree t)
    {
        if (t == null)
        {
            return;
        }
        if (t.IsNil)
        {
            throw new ArgumentException("Can't set single child to a list");
        }
        children ??= CreateChildrenList().Cast<Tree>().ToList();
        children[i] = t;
        t.Parent = this;
        t.ChildIndex = i;
    }

    /** Insert child t at child position i (0..n-1) by shifting children
        i+1..n-1 to the right one position. Set parent / indexes properly
        but does NOT collapse nil-rooted t's that come in here like addChild.
     */
    public virtual void InsertChild(int i, Tree t)
    {
        if (i < 0 || i > ChildCount)
        {
            throw new IndexOutOfRangeException(i + " out or range");
        }

        children ??= CreateChildrenList().Cast<Tree>().ToList();

        children.Insert(i, t);
        // walk others to increment their child indexes
        // set index, parent of this one too
        this.FreshenParentAndChildIndexes(i);
    }

    public virtual object DeleteChild(int i)
    {
        if (children == null)
        {
            return null;
        }
        Tree killed = children[i];
        children.RemoveAt(i);
        // walk rest and decrement their child indexes
        this.FreshenParentAndChildIndexes(i);
        return killed;
    }

    /** Delete children from start to stop and replace with t even if t is
     *  a list (nil-root tree).  num of children can increase or decrease.
     *  For huge child lists, inserting children can force walking rest of
     *  children to set their childindex; could be slow.
     */
    public virtual void ReplaceChildren(int startChildIndex, int stopChildIndex, object t)
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
        var newTree = (BaseTree)t;
        List<Tree> newChildren;
        // normalize to a list of children to add: newChildren
        if (newTree.IsNil)
        {
            newChildren = newTree.children;
        }
        else
        {
            newChildren = new(1);
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
                child.SetParent(this);
                child.ChildIndex = i;
                j++;
            }
        }
        else if (delta > 0)
        { // fewer new nodes than there were
          // set children and then delete extra
            for (int j = 0; j < numNewChildren; j++)
            {
                children[startChildIndex + j] = newChildren[(j)];
            }
            int indexToDelete = startChildIndex + numNewChildren;
            for (int c = indexToDelete; c <= stopChildIndex; c++)
            {
                // delete same index, shifting everybody down each time
                children.RemoveAt(indexToDelete);
            }
            FreshenParentAndChildIndexes(startChildIndex);
        }
        else
        { // more new nodes than were there before
          // fill in as many children as we can (replacingHowMany) w/o moving data
            for (int j = 0; j < replacingHowMany; j++)
            {
                children[(startChildIndex + j)] = newChildren[(j)];
            }
            int numToInsert = replacingWithHowMany - replacingHowMany;
            for (int j = replacingHowMany; j < replacingWithHowMany; j++)
            {
                children.Insert(startChildIndex + j, newChildren[(j)]);
            }
            FreshenParentAndChildIndexes(startChildIndex);
        }
        //Console.Out.WriteLine("out="+toStringTree());
    }

    /** Override in a subclass to change the impl of children list */
    protected virtual List<object> CreateChildrenList()
    {
        return new();
    }

    public virtual bool IsNil => false;

    /** Set the parent and child index values for all child of t */
    public virtual void FreshenParentAndChildIndexes()
    {
        FreshenParentAndChildIndexes(0);
    }

    public virtual void FreshenParentAndChildIndexes(int offset)
    {
        int n = ChildCount;
        for (int c = offset; c < n; c++)
        {
            Tree child = GetChild(c);
            child.ChildIndex = c;
            child.Parent = this;
        }
    }

    public void FreshenParentAndChildIndexesDeeply()
    {
        FreshenParentAndChildIndexesDeeply(0);
    }

    public void FreshenParentAndChildIndexesDeeply(int offset)
    {
        int n = ChildCount;
        for (int c = offset; c < n; c++)
        {
            BaseTree child = (BaseTree)GetChild(c);
            child.ChildIndex = c;
            child.SetParent(this);
            child.FreshenParentAndChildIndexesDeeply();
        }
    }

    public void SanityCheckParentAndChildIndexes()
    {
        SanityCheckParentAndChildIndexes(null, -1);
    }

    public void SanityCheckParentAndChildIndexes(Tree parent, int i)
    {
        if (parent != this.Parent)
        {
            throw new IllegalStateException("parents don't match; expected " + parent + " found " + this.Parent);
        }
        if (i != this.ChildIndex)
        {
            throw new IllegalStateException("child indexes don't match; expected " + i + " found " + this.ChildIndex);
        }
        int n = this.ChildCount;
        for (int c = 0; c < n; c++)
        {
            CommonTree child = (CommonTree)this.GetChild(c);
            child.SanityCheckParentAndChildIndexes(this, c);
        }
    }


    /** BaseTree doesn't track parent pointers. */
    public virtual Tree Parent
    {
        get => null;
        set
        {
        }
    }

    /** Walk upwards looking for ancestor with this token type. */
    //@Override
    public bool HasAncestor(int ttype) => GetAncestor(ttype) != null;

    /** Walk upwards and get first ancestor with this token type. */
    //@Override
    public Tree GetAncestor(int ttype)
    {
        Tree t = this;
        t = t.Parent;
        while (t != null)
        {
            if (t.Type == ttype) return t;
            t = t.Parent;
        }
        return null;
    }

    /** Return a list of all ancestors of this node.  The first node of
     *  list is the root and the last is the parent of this node.
     */
    public virtual List<Tree> GetAncestors()
    {
        if (Parent == null) return null;
        List<Tree> ancestors = new();
        Tree t = this;
        t = t.Parent;
        while (t != null)
        {
            ancestors.Insert(0, t); // insert at start
            t = t.Parent;
        }
        return ancestors;
    }

    /** Print out a whole tree not just a node */
    public virtual string ToStringTree()
    {
        if (children == null || children.Count == 0)
        {
            return this.ToString();
        }
        var buffer = new StringBuilder();
        if (!IsNil)
        {
            buffer.Append('(');
            buffer.Append(this.ToString());
            buffer.Append(' ');
        }
        for (int i = 0; children != null && i < children.Count; i++)
        {
            var t = children[(i)];
            if (i > 0)
            {
                buffer.Append(' ');
            }
            buffer.Append(t.ToStringTree());
        }
        if (!IsNil)
        {
            buffer.Append(")");
        }
        return buffer.ToString();
    }

    //@Override
    public virtual int Line => 0;

    //@Override
    public virtual int CharPositionInLine => 0;

    public virtual object Payload => throw new NotImplementedException();

    public virtual int Type => throw new NotImplementedException();

    public virtual string Text => throw new NotImplementedException();

    public virtual void SetParent(BaseTree baseTree)
    {
        throw new NotImplementedException();
    }

    public virtual int TokenStartIndex { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public virtual int TokenStopIndex { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    /** BaseTree doesn't track child indexes. */
    public virtual int ChildIndex
    {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }

    public virtual object DupNode()
    {
        throw new NotImplementedException();
    }
    /** Override to say how a node (not a tree) should look as text */
    //@Override
    //public abstract string toString();
}
