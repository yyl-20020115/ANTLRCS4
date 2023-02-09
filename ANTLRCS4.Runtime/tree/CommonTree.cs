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
using org.antlr.v4.runtime.tree;
using org.antlr.v4.runtime;

namespace org.antlr.runtime.tree;

/** A tree node that is wrapper for a Token object.  After 3.0 release
 *  while building tree rewrite stuff, it became clear that computing
 *  parent and child index is very difficult and cumbersome.  Better to
 *  spend the space in every tree node.  If you don't want these extra
 *  fields, it's easy to cut them out in your own BaseTree subclass.
 */
public class CommonTree : BaseTree
{
    /** A single token is the payload */
    public Token token;

    /** What token indexes bracket all tokens associated with this node
     *  and below?
     */
    protected int startIndex = -1, stopIndex = -1;

    /** Who is the parent node of this node; if null, implies node is root */
    public CommonTree parent;

    /** What index is this node in the child list? Range: 0..n-1 */
    public int childIndex = -1;

    public CommonTree() { }

    public CommonTree(CommonTree node):base(node)
    {
        this.token = node.token;
        this.startIndex = node.startIndex;
        this.stopIndex = node.stopIndex;
    }

    public CommonTree(Token t)
    {
        this.token = t;
    }

    public Token Token => token;

    public override Tree DupNode() => new CommonTree(this);

    public override bool IsNil => token == null;

    public override int Type => token == null ? Token.INVALID_TYPE : token.Type;

    public override string Text => token?.Text;

    public override int Line => token == null || token.Line == 0 ? ChildCount > 0 ? GetChild(0).Line : 0 : token.Line;

    public override int CharPositionInLine => token == null || token.CharPositionInLine == -1
                ? ChildCount > 0 ? GetChild(0).CharPositionInLine : 0
                : token.CharPositionInLine;
    public override int TokenStartIndex
    {
        get => startIndex == -1 && token != null ? token.TokenIndex : startIndex;

        set => startIndex = value;
    }

    public override int TokenStopIndex
    {
        get => stopIndex == -1 && token != null ? token.TokenIndex : stopIndex;

        set => stopIndex = value;
    }

    /** For every node in this subtree, make sure it's start/stop token's
     *  are set.  Walk depth first, visit bottom up.  Only updates nodes
     *  with at least one token index &lt; 0.
     */
    public void SetUnknownTokenBoundaries()
    {
        if (children == null)
        {
            if (startIndex < 0 || stopIndex < 0)
            {
                startIndex = stopIndex = token.TokenIndex;
            }
            return;
        }
        for (int i = 0; i < children.Count; i++)
        {
            ((CommonTree)children[(i)]).SetUnknownTokenBoundaries();
        }
        if (startIndex >= 0 && stopIndex >= 0) return; // already set
        if (children.Count > 0)
        {
            var firstChild = (CommonTree)children[0];
            var lastChild = (CommonTree)children[^1];
            startIndex = firstChild.TokenStartIndex;
            stopIndex = lastChild.TokenStopIndex;
        }
    }

    public override int ChildIndex { get => childIndex; set => this.childIndex = value; }

    public override Tree Parent { get => parent; set => this.parent = (CommonTree)value; }

    public override string ToString() => IsNil ? "nil" : Type == Token.INVALID_TYPE ? "<errornode>" : token?.Text;
}
