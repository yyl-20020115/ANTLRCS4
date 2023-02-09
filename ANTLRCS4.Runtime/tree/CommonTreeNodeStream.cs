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
using org.antlr.v4.runtime;
using System.Text;
using org.antlr.runtime.misc;

namespace org.antlr.runtime.tree;


public class CommonTreeNodeStream : LookaheadStream<Object>, TreeNodeStream, PositionTrackingStream<Object>
{

    public static readonly int DEFAULT_INITIAL_BUFFER_SIZE = 100;
    public static readonly int INITIAL_CALL_STACK_SIZE = 10;

    /** Pull nodes from which tree? */
    protected Object root;

    /** If this tree (root) was created from a {@link TokenStream}, track it. */
    protected TokenStream tokens;

    /** What {@link TreeAdaptor} was used to build these trees */
    TreeAdaptor adaptor;

    /** The {@link TreeIterator} we using. */
    protected TreeIterator it;

    /** Stack of indexes used for push/pop calls. */
    protected IntArray calls;

    /** Tree {@code (nil A B C)} trees like flat {@code A B C} streams */
    protected bool hasNilRoot = false;

    /** Tracks tree depth.  Level=0 means we're at root node level. */
    protected int level = 0;

    /**
     * Tracks the last node before the start of {@link #data} which contains
     * position information to provide information for error reporting. This is
     * tracked in addition to {@link #prevElement} which may or may not contain
     * position information.
     *
     * @see #hasPositionInformation
     * @see RecognitionException#extractInformationFromTreeNodeStream
     */
    protected Object previousLocationElement;

    public CommonTreeNodeStream(Object tree)
        : this(new CommonTreeAdaptor(), tree)
    {
    }

    public CommonTreeNodeStream(TreeAdaptor adaptor, Object tree)
    {
        this.root = tree;
        this.adaptor = adaptor;
        it = new TreeIterator(adaptor, root);
    }

    public override void Reset()
    {
        base.Reset();
        it.reset();
        hasNilRoot = false;
        level = 0;
        previousLocationElement = null;
        if (calls != null) calls.Clear();
    }

    /** Pull elements from tree iterator.  Track tree level 0..max_level.
     *  If nil rooted tree, don't give initial nil and DOWN nor final UP.
     */
    //@Override
    public override Object nextElement()
    {
        Object t = it.next();
        //Console.Out.WriteLine("pulled "+adaptor.getType(t));
        if (t == it.up)
        {
            level--;
            if (level == 0 && hasNilRoot) return it.next(); // don't give last UP; get EOF
        }
        else if (t == it.down) level++;
        if (level == 0 && adaptor.IsNil(t))
        { // if nil root, scarf nil, DOWN
            hasNilRoot = true;
            t = it.next(); // t is now DOWN, so get first real node next
            level++;
            t = it.next();
        }
        return t;
    }

    //@Override
    public override Object Remove()
    {
        Object result = base.Remove();
        if (p == 0 && hasPositionInformation(prevElement))
        {
            previousLocationElement = prevElement;
        }

        return result;
    }

    //@Override
    public override bool isEOF(Object o) { return adaptor.GetType(o) == Token.EOF; }

    //@Override
    public void setUniqueNavigationNodes(bool uniqueNavigationNodes) { }
    //@Override
    public Object getTreeSource() { return root; }

    //@Override
    public string SourceName => getTokenStream().SourceName;
    //@Override
    public TokenStream getTokenStream() { return tokens; }

    public void setTokenStream(TokenStream tokens) { this.tokens = tokens; }

    //@Override
    public TreeAdaptor getTreeAdaptor() { return adaptor; }

    public void setTreeAdaptor(TreeAdaptor adaptor) { this.adaptor = adaptor; }

    //@Override
    public Object get(int i)
    {
        throw new UnsupportedOperationException("Absolute node indexes are meaningless in an unbuffered stream");
    }

    //@Override
    public int LA(int i) { return adaptor.GetType(LT(i)); }

    /** Make stream jump to a new location, saving old location.
     *  Switch back with pop().
     */
    public void push(int index)
    {
        if (calls == null)
        {
            calls = new IntArray();
        }
        calls.Push(p); // save current index
        Seek(index);
    }

    /** Seek back to previous index saved during last {@link #push} call.
     *  Return top of stack (return index).
     */
    public int pop()
    {
        int ret = calls.Pop();
        Seek(ret);
        return ret;
    }

    /**
     * Returns an element containing position information. If {@code allowApproximateLocation} is {@code false}, then
     * this method will return the {@code LT(1)} element if it contains position information, and otherwise return {@code null}.
     * If {@code allowApproximateLocation} is {@code true}, then this method will return the last known element containing position information.
     *
     * @see #hasPositionInformation
     */
    //@Override
    public Object getKnownPositionElement(bool allowApproximateLocation)
    {
        Object node = data[p];
        if (hasPositionInformation(node))
        {
            return node;
        }

        if (!allowApproximateLocation)
        {
            return null;
        }

        for (int index = p - 1; index >= 0; index--)
        {
            node = data[(index)];
            if (hasPositionInformation(node))
            {
                return node;
            }
        }

        return previousLocationElement;
    }

    //@Override
    public bool hasPositionInformation(Object node)
    {
        Token token = adaptor.GetToken(node);
        if (token == null)
        {
            return false;
        }

        if (token.Line <= 0)
        {
            return false;
        }

        return true;
    }

    // TREE REWRITE INTERFACE

    //@Override
    public void replaceChildren(Object parent, int startChildIndex, int stopChildIndex, Object t)
    {
        if (parent != null)
        {
            adaptor.ReplaceChildren(parent, startChildIndex, stopChildIndex, t);
        }
    }

    //@Override
    public string toString(Object start, Object stop)
    {
        // we'll have to walk from start to stop in tree; we're not keeping
        // a complete node stream buffer
        return "n/a";
    }

    /** For debugging; destructive: moves tree iterator to end. */
    public string toTokenTypeString()
    {
        Reset();
        StringBuilder buf = new StringBuilder();
        Object o = LT(1);
        int type = adaptor.GetType(o);
        while (type != Token.EOF)
        {
            buf.Append(" ");
            buf.Append(type);
            Consume();
            o = LT(1);
            type = adaptor.GetType(o);
        }
        return buf.ToString();
    }
}
