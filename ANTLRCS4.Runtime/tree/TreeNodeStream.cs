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
using org.antlr.v4.runtime;

namespace org.antlr.runtime.tree;


/** A stream of tree nodes, accessing nodes from a tree of some kind */
public interface TreeNodeStream : IntStream
{
    /** Get a tree node at an absolute index {@code i}; 0..n-1.
	 *  If you don't want to buffer up nodes, then this method makes no
	 *  sense for you.
	 */
    object Get(int i);

    /**
     * Get tree node at current input pointer + {@code k} ahead where
     * {@code k==1} is next node. {@code k<0} indicates nodes in the past. So
     * {@code LT(-1)} is previous node, but implementations are not required to
     * provide results for {@code k < -1}. {@code LT(0)} is undefined. For
     * {@code k<=n}, return {@code null}. Return {@code null} for {@code LT(0)}
     * and any index that results in an absolute address that is negative.
     * <p>
     * This is analogous to {@link TokenStream#LT}, but this returns a tree node
     * instead of a {@link Token}. Makes code generation identical for both
     * parser and tree grammars.</p>
     */
    object LT(int k);

    /** Where is this stream pulling nodes from?  This is not the name, but
     *  the object that provides node objects.
     */
    object GetTreeSource();

    /**
     * If the tree associated with this stream was created from a
     * {@link TokenStream}, you can specify it here. Used to do rule
     * {@code $text} attribute in tree parser. Optional unless you use tree
     * parser rule {@code $text} attribute or {@code output=template} and
     * {@code rewrite=true} options.
     */
    TokenStream TokenStream { get; }

    /** What adaptor can tell me how to interpret/navigate nodes and
     *  trees.  E.g., get text of a node.
     */
    TreeAdaptor TreeAdaptor { get; }

    /**
     * As we flatten the tree, we use {@link Token#UP}, {@link Token#DOWN} nodes
     * to represent the tree structure. When debugging we need unique nodes so
     * we have to instantiate new ones. When doing normal tree parsing, it's
     * slow and a waste of memory to create unique navigation nodes. Default
     * should be {@code false}.
     */
    void SetUniqueNavigationNodes(bool uniqueNavigationNodes);

    /** Reset the tree node stream in such a way that it acts like
     *  a freshly constructed stream.
     */
    void Reset();

    /**
     * Return the text of all nodes from {@code start} to {@code stop},
     * inclusive. If the stream does not buffer all the nodes then it can still
     * walk recursively from start until stop. You can always return
     * {@code null} or {@code ""} too, but users should not access
     * {@code $ruleLabel.text} in an action of course in that case.
     */
    string ToString(object start, object stop);

    // REWRITING TREES (used by tree parser)

    /**
     * Replace children of {@code parent} from index {@code startChildIndex} to
     * {@code stopChildIndex} with {@code t}, which might be a list. Number of
     * children may be different after this call. The stream is notified because
     * it is walking the tree and might need to know you are monkeying with the
     * underlying tree. Also, it might be able to modify the node stream to
     * avoid restreaming for future phases.
     * <p>
     * If {@code parent} is {@code null}, don't do anything; must be at root of
     * overall tree. Can't replace whatever points to the parent externally. Do
     * nothing.</p>
     */
    void ReplaceChildren(object parent, int startChildIndex, int stopChildIndex, object t);
    void Rewind(int nvaeMark);
}
