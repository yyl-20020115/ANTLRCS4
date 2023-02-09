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
using org.antlr.v4.runtime.tree.pattern;
using org.antlr.v4.runtime.tree;
using org.antlr.v4.runtime;

namespace org.antlr.runtime.tree;
/** A TreeAdaptor that works with any Tree implementation. */
public abstract class BaseTreeAdaptor : TreeAdaptor
{
    /** System.identityHashCode() is not always unique; we have to
	 *  track ourselves.  That's ok, it's only for debugging, though it's
	 *  expensive: we have to create a hashtable with all tree nodes in it.
	 */
    protected Dictionary<object, int> treeToUniqueIDMap;
    protected int uniqueNodeID = 1;

    public object Nil()
    {
        return Create(null);
    }

    /** create tree node that holds the start and stop tokens associated
     *  with an error.
     *
     *  If you specify your own kind of tree nodes, you will likely have to
     *  override this method. CommonTree returns Token.INVALID_TOKEN_TYPE
     *  if no token payload but you might have to set token type for diff
     *  node type.
     *
     *  You don't have to subclass CommonErrorNode; you will likely need to
     *  subclass your own tree node class to avoid class cast exception.
     */
    object TreeAdaptor.ErrorNode(TokenStream input, Token start, Token stop,
                            RecognitionException e)
    {
        CommonErrorNode t = new CommonErrorNode(input, start, stop, e);
        //Console.Out.WriteLine("returning error node '"+t+"' @index="+input.index());
        return t;
    }

    public bool IsNil(object tree)
    {
        return ((Tree)tree).IsNil;
    }

    public object DupTree(object tree)
    {
        return DupTree(tree, null);
    }

    /** This is generic in the sense that it will work with any kind of
     *  tree (not just Tree interface).  It invokes the adaptor routines
     *  not the tree node routines to do the construction.  
     */
    public object DupTree(object t, object parent)
    {
        if (t == null)
        {
            return null;
        }
        var newTree = DupNode(t);
        // ensure new subtree root has parent/child index set
        SetChildIndex(newTree, GetChildIndex(t)); // same index in new tree
        SetParent(newTree, parent);
        int n = GetChildCount(t);
        for (int i = 0; i < n; i++)
        {
            var child = GetChild(t, i);
            var newSubTree = DupTree(child, t);
            AddChild(newTree, newSubTree);
        }
        return newTree;
    }

    /** Add a child to the tree t.  If child is a flat tree (a list), make all
     *  in list children of t.  Warning: if t has no children, but child does
     *  and child isNil then you can decide it is ok to move children to t via
     *  t.children = child.children; i.e., without copying the array.  Just
     *  make sure that this is consistent with have the user will build
     *  ASTs.
     */
    public void AddChild(object t,object child)
    {
        if (t != null && child != null)
        {
            ((Tree)t).AddChild((Tree)child);
        }
    }

    /** If oldRoot is a nil root, just copy or move the children to newRoot.
     *  If not a nil root, make oldRoot a child of newRoot.
     *
     *    old=^(nil a b c), new=r yields ^(r a b c)
     *    old=^(a b c), new=r yields ^(r ^(a b c))
     *
     *  If newRoot is a nil-rooted single child tree, use the single
     *  child as the new root node.
     *
     *    old=^(nil a b c), new=^(nil r) yields ^(r a b c)
     *    old=^(a b c), new=^(nil r) yields ^(r ^(a b c))
     *
     *  If oldRoot was null, it's ok, just return newRoot (even if isNil).
     *
     *    old=null, new=r yields r
     *    old=null, new=^(nil r) yields ^(nil r)
     *
     *  Return newRoot.  Throw an exception if newRoot is not a
     *  simple node or nil root with a single child node--it must be a root
     *  node.  If newRoot is ^(nil x) return x as newRoot.
     *
     *  Be advised that it's ok for newRoot to point at oldRoot's
     *  children; i.e., you don't have to copy the list.  We are
     *  constructing these nodes so we should have this control for
     *  efficiency.
     */
    public object BecomeRoot(object newRoot, object oldRoot)
    {
        //Console.Out.WriteLine("becomeroot new "+newRoot.toString()+" old "+oldRoot);
        Tree newRootTree = (Tree)newRoot;
        Tree oldRootTree = (Tree)oldRoot;
        if (oldRoot == null)
        {
            return newRoot;
        }
        // handle ^(nil real-node)
        if (newRootTree.IsNil)
        {
            int nc = newRootTree.ChildCount;
            if (nc == 1) newRootTree = newRootTree.GetChild(0);
            else if (nc > 1)
            {
                // TODO: make tree run time exceptions hierarchy
                throw new RuntimeException("more than one node as root (TODO: make exception hierarchy)");
            }
        }
        // add oldRoot to newRoot; addChild takes care of case where oldRoot
        // is a flat list (i.e., nil-rooted tree).  All children of oldRoot
        // are added to newRoot.
        newRootTree.AddChild(oldRootTree);
        return newRootTree;
    }

    /** Transform ^(nil x) to x and nil to null */
    public object RulePostProcessing(object root)
    {
        //Console.Out.WriteLine("rulePostProcessing: "+((Tree)root).toStringTree());
        Tree r = (Tree)root;
        if (r != null && r.IsNil)
        {
            if (r.ChildCount == 0)
            {
                r = null;
            }
            else if (r.ChildCount == 1)
            {
                r = r.GetChild(0);
                // whoever invokes rule will set parent and child index
                r.                // whoever invokes rule will set parent and child index
                Parent = null;
                r.                ChildIndex = -1;
            }
        }
        return r;
    }

    public object BecomeRoot(Token newRoot, object oldRoot)
    {
        return BecomeRoot(Create(newRoot), oldRoot);
    }

    public object Create(int tokenType, Token fromToken)
    {
        fromToken = CreateToken(fromToken);
        //((ClassicToken)fromToken).setType(tokenType);
        fromToken.        //((ClassicToken)fromToken).setType(tokenType);
        Type = tokenType;
        Tree t = (Tree)Create(fromToken);
        return t;
    }

    public object Create(int tokenType, Token fromToken, string text)
    {
        if (fromToken == null) return Create(tokenType, text);
        fromToken = CreateToken(fromToken);
        fromToken.        Type = tokenType;
        fromToken.        Text = text;
        Tree t = (Tree)Create(fromToken);
        return t;
    }

    public object Create(int tokenType, string text)
    {
        Token fromToken = CreateToken(tokenType, text);
        Tree t = (Tree)Create(fromToken);
        return t;
    }

    public int GetType(object t)
    {
        return ((Tree)t).Type;
    }

    public void SetType(object t, int type)
    {
        throw new NoSuchMethodError("don't know enough about Tree node");
    }

    public string GetText(object t)
    {
        return ((Tree)t).Text;
    }

    public void SetText(object t, string text)
    {
        throw new NoSuchMethodError("don't know enough about Tree node");
    }

    public object GetChild(object t, int i)
    {
        return ((Tree)t).GetChild(i);
    }

    public void SetChild(object t, int i, object child)
    {
        ((Tree)t).SetChild(i, (Tree)child);
    }

    public object DeleteChild(object t, int i)
    {
        return ((Tree)t).DeleteChild(i);
    }

    public int GetChildCount(object t)
    {
        return ((Tree)t).ChildCount;
    }

    public int GetUniqueID(object node)
    {
        treeToUniqueIDMap ??= new Dictionary<object, int>();
        if (treeToUniqueIDMap.TryGetValue(node, out var prevID))
        {
            return prevID;
        }
        int ID = uniqueNodeID;
        treeToUniqueIDMap[node]= ID;
        uniqueNodeID++;
        return ID;
        // GC makes these nonunique:
        // return System.identityHashCode(node);
    }

    /** Tell me how to create a token for use with imaginary token nodes.
     *  For example, there is probably no input symbol associated with imaginary
     *  token DECL, but you need to create it as a payload or whatever for
     *  the DECL node as in ^(DECL type ID).
     *
     *  If you care what the token payload objects' type is, you should
     *  override this method and any other createToken variant.
     */
    public abstract Token CreateToken(int tokenType, string text);

    /** Tell me how to create a token for use with imaginary token nodes.
     *  For example, there is probably no input symbol associated with imaginary
     *  token DECL, but you need to create it as a payload or whatever for
     *  the DECL node as in ^(DECL type ID).
     *
     *  This is a variant of createToken where the new token is derived from
     *  an actual real input token.  Typically this is for converting '{'
     *  tokens to BLOCK etc...  You'll see
     *
     *    r : lc='{' ID+ '}' -&gt; ^(BLOCK[$lc] ID+) ;
     *
     *  If you care what the token payload objects' type is, you should
     *  override this method and any other createToken variant.
     */
    public abstract Token CreateToken(Token fromToken);

    public virtual object Create(Token payload)
    {
        throw new NotImplementedException();
    }

    public virtual object DupNode(object treeNode)
    {
        throw new NotImplementedException();
    }

    public virtual Token GetToken(object t)
    {
        throw new NotImplementedException();
    }

    public virtual void SetTokenBoundaries(object t, Token startToken, Token stopToken)
    {
        throw new NotImplementedException();
    }

    public virtual int GetTokenStartIndex(object t)
    {
        throw new NotImplementedException();
    }

    public virtual int GetTokenStopIndex(object t)
    {
        throw new NotImplementedException();
    }

    public virtual object GetParent(object t)
    {
        throw new NotImplementedException();
    }

    public virtual void SetParent(object t, object parent)
    {
        throw new NotImplementedException();
    }

    public virtual int GetChildIndex(object t)
    {
        throw new NotImplementedException();
    }

    public virtual void SetChildIndex(object t, int index)
    {
        throw new NotImplementedException();
    }

    public virtual void ReplaceChildren(object parent, int startChildIndex, int stopChildIndex, object t)
    {
        throw new NotImplementedException();
    }
}

