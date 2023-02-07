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
using org.antlr.runtime.tree;
using org.antlr.runtime;
using org.antlr.v4.runtime.tree;
using org.antlr.v4.runtime;
using System;

namespace org.antlr.runtime.tree;

public class TreeRewriter : TreeParser
{
    public interface fptr
    {
        public Object rule();
    }

    protected bool showTransformations = false;

    protected TokenStream originalTokenStream;
    protected TreeAdaptor originalAdaptor;

    public TreeRewriter(TreeNodeStream input)
        : this(input, new RecognizerSharedState())
    {
        ;
    }
    public TreeRewriter(TreeNodeStream input, RecognizerSharedState state)
        : base(input, state)
    {
        originalAdaptor = input.getTreeAdaptor();
        originalTokenStream = input.getTokenStream();
    }

    public Object applyOnce(Object t, fptr whichRule)
    {
        if (t == null) return null;
        try
        {
            // share TreeParser object but not parsing-related state
            state = new RecognizerSharedState();
            input = new CommonTreeNodeStream(originalAdaptor, t);
            ((CommonTreeNodeStream)input).setTokenStream(originalTokenStream);
            BacktrackingLevel = 1;
            TreeRuleReturnScope r = (TreeRuleReturnScope)whichRule.rule();
            BacktrackingLevel = 0;
            if (Failed) return t;
            if (showTransformations &&
                 r != null && !t.Equals(r.getTree()) && r.getTree() != null)
            {
                reportTransformation(t, r.getTree());
            }
            if (r != null && r.getTree() != null) return r.getTree();
            else return t;
        }
        catch (RecognitionException e) {; }
        return t;
    }

    public Object applyRepeatedly(Object t, fptr whichRule)
    {
        bool treeChanged = true;
        while (treeChanged)
        {
            Object u = applyOnce(t, whichRule);
            treeChanged = !t.Equals(u);
            t = u;
        }
        return t;
    }

    public Object downup(Object t) { return downup(t, false); }

    public class TVA : TreeVisitorAction
    {
        public readonly TreeRewriter treeRewriter;
        public TVA(TreeRewriter treeRewriter)
        {
            this.treeRewriter = treeRewriter;
        }

        //@Override
        public Object pre(Object t) { return treeRewriter.applyOnce(t, treeRewriter.topdown_fptr); }
        //@Override
        public Object post(Object t) { return treeRewriter.applyRepeatedly(t, treeRewriter.bottomup_ftpr); }
    }
    public class CFPTR1 : fptr
    {
        public Object rule() { return topdown(); }
        public Object topdown() { return null; }

    }
    public class CFPTR2 : fptr
    {
        public Object rule() { return bottomup(); }
        public Object bottomup() { return null; }

    }
    public Object downup(Object t, bool showTransformations)
    {
        this.showTransformations = showTransformations;
        TreeVisitor v = new TreeVisitor(new CommonTreeAdaptor());
        TreeVisitorAction actions = new TVA(this);
        t = v.visit(t, actions);
        return t;
    }

    /** Override this if you need transformation tracing to go somewhere
     *  other than stdout or if you're not using Tree-derived trees.
     */
    public void reportTransformation(Object oldTree, Object newTree)
    {
        Console.WriteLine(((Tree)oldTree).ToStringTree() + " -> " +
                           ((Tree)newTree).ToStringTree());
    }

    fptr topdown_fptr = new CFPTR1();

    fptr bottomup_ftpr = new CFPTR2();

    // methods the downup strategy uses to do the up and down rules.
    // to override, just define tree grammar rule topdown and turn on
    // filter=true.
}
