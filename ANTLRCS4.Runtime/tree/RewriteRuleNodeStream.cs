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

namespace org.antlr.runtime.tree;

/** Queues up nodes matched on left side of -&gt; in a tree parser. This is
 *  the analog of RewriteRuleTokenStream for normal parsers. 
 */
public class RewriteRuleNodeStream : RewriteRuleElementStream
{

    public RewriteRuleNodeStream(TreeAdaptor adaptor, string elementDescription)
        : base(adaptor, elementDescription)
    {

    }

    /** Create a stream with one element */
    public RewriteRuleNodeStream(TreeAdaptor adaptor,
                                 string elementDescription,
                                 object oneElement)
        : base(adaptor, elementDescription, oneElement)
    {
    }

    /** Create a stream, but feed off an existing list */
    public RewriteRuleNodeStream(TreeAdaptor adaptor,
                                 string elementDescription,
                                 List<object> elements)
        : base(adaptor, elementDescription, elements)
    {
    }

    public object NextNode()
    {
        return Next();
    }

    protected object ToTree(object el)
    {
        return adaptor.DupNode(el);
    }

    protected override object Dup(object el)
    {
        // we dup every node, so don't have to worry about calling dup; short-
        // circuited next() so it doesn't call.
        throw new UnsupportedOperationException("dup can't be called for a node stream.");
    }
}
