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
using org.antlr.runtime.tree;
using org.antlr.v4.runtime.tree.pattern;
using org.antlr.v4.runtime;
using System.Xml.Linq;

namespace org.antlr.runtime.tree;

public class TreePatternParser
{
    protected TreePatternLexer tokenizer;
    protected int ttype;
    protected TreeWizard wizard;
    protected TreeAdaptor adaptor;

    public TreePatternParser(TreePatternLexer tokenizer, TreeWizard wizard, TreeAdaptor adaptor)
    {
        this.tokenizer = tokenizer;
        this.wizard = wizard;
        this.adaptor = adaptor;
        ttype = tokenizer.nextToken(); // kickstart
    }

    public object Pattern()
    {
        if (ttype == TreePatternLexer.BEGIN)
        {
            return ParseTree();
        }
        else if (ttype == TreePatternLexer.ID)
        {
            var node = ParseNode();
            if (ttype == TreePatternLexer.EOF)
            {
                return node;
            }
            return null; // extra junk on end
        }
        return null;
    }

    public object ParseTree()
    {
        if (ttype != TreePatternLexer.BEGIN)
        {
            throw new RuntimeException("no BEGIN");
        }
        ttype = tokenizer.nextToken();
        var root = ParseNode();
        if (root == null)
        {
            return null;
        }
        while (ttype == TreePatternLexer.BEGIN ||
                ttype == TreePatternLexer.ID ||
                ttype == TreePatternLexer.PERCENT ||
                ttype == TreePatternLexer.DOT)
        {
            if (ttype == TreePatternLexer.BEGIN)
            {
                var subtree = ParseTree();
                adaptor.AddChild(root, subtree);
            }
            else
            {
                var child = ParseNode();
                if (child == null)
                {
                    return null;
                }
                adaptor.AddChild(root, child);
            }
        }
        if (ttype != TreePatternLexer.END)
        {
            throw new RuntimeException("no END");
        }
        ttype = tokenizer.nextToken();
        return root;
    }

    public object ParseNode()
    {
        // "%label:" prefix
        string label = null;
        if (ttype == TreePatternLexer.PERCENT)
        {
            ttype = tokenizer.nextToken();
            if (ttype != TreePatternLexer.ID)
            {
                return null;
            }
            label = tokenizer.sval.ToString();
            ttype = tokenizer.nextToken();
            if (ttype != TreePatternLexer.COLON)
            {
                return null;
            }
            ttype = tokenizer.nextToken(); // move to ID following colon
        }

        // Wildcard?
        if (ttype == TreePatternLexer.DOT)
        {
            ttype = tokenizer.nextToken();
            Token wildcardPayload = new CommonToken(0, ".");
            TreeWizard.TreePattern node2 =
                new TreeWizard.WildcardTreePattern(wildcardPayload);
            if (label != null)
            {
                node2.label = label;
            }
            return node2;
        }

        // "ID" or "ID[arg]"
        if (ttype != TreePatternLexer.ID)
        {
            return null;
        }
        string tokenName = tokenizer.sval.ToString();
        ttype = tokenizer.nextToken();
        if (tokenName.Equals("nil"))
        {
            return adaptor.Nil();
        }
        string text = tokenName;
        // check for arg
        string arg = null;
        if (ttype == TreePatternLexer.ARG)
        {
            arg = tokenizer.sval.ToString();
            text = arg;
            ttype = tokenizer.nextToken();
        }

        // create node
        int treeNodeType = wizard.GetTokenType(tokenName);
        if (treeNodeType == Token.INVALID_TOKEN_TYPE)
        {
            return null;
        }
        var node = adaptor.Create(treeNodeType, text);
        if (label != null && node is TreeWizard.TreePattern pattern2)
        {
            pattern2.label = label;
        }
        if (arg != null && node is TreeWizard.TreePattern pattern1)
        {
            pattern1.hasTextArg = true;
        }
        return node;
    }
}
