/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.runtime.tree;
using org.antlr.v4.parse;
using org.antlr.v4.runtime;
using org.antlr.v4.runtime.atn;
using org.antlr.v4.runtime.misc;
using org.antlr.v4.runtime.tree;
using System.Text;

namespace org.antlr.v4.tool.ast;

public class GrammarAST : CommonTree
{
    /** For error msgs, nice to know which grammar this AST lives in */
    // TODO: try to remove
    public Grammar g;

    /** If we build an ATN, we make AST node point at left edge of ATN construct */
    public ATNState atnState;

    public string textOverride;

    public GrammarAST() { }
    public GrammarAST(Token t) : base(t) {; }
    public GrammarAST(GrammarAST node) : base(node)
    {
        this.g = node.g;
        this.atnState = node.atnState;
        this.textOverride = node.textOverride;
    }
    public GrammarAST(int type) : base(new CommonToken(type, ANTLRParser.tokenNames[type])) { }
    public GrammarAST(int type, Token t) : this(new CommonToken(t))
    {
        token.Type = type;
    }
    public GrammarAST(int type, Token t, string text) : this(new CommonToken(t))
    {
        token.Type = type;
        token.Text = text;
    }

    public GrammarAST[] GetChildrenAsArray() => children.Cast<GrammarAST>().ToArray();

    public List<GrammarAST> GetNodesWithType(int ttype) => GetNodesWithType(IntervalSet.Of(ttype));

    public List<GrammarAST> GetAllChildrenWithType(int type)
    {
        List<GrammarAST> nodes = new();
        for (int i = 0; children != null && i < children.Count; i++)
        {
            Tree t = (Tree)children[(i)];
            if (t.Type == type)
            {
                nodes.Add((GrammarAST)t);
            }
        }
        return nodes;
    }

    public List<GrammarAST> GetNodesWithType(IntervalSet types)
    {
        List<GrammarAST> nodes = new();
        List<GrammarAST> work = new()
        {
            this
        };
        GrammarAST t;
        while (work.Count > 0)
        {
            t = work[0];
            work.RemoveAt(0);
            if (types == null || types.Contains(t.Type)) nodes.Add(t);
            if (t.children != null)
            {
                work.AddRange(Arrays.AsList(t.GetChildrenAsArray()));
            }
        }
        return nodes;
    }

    public List<GrammarAST> GetNodesWithTypePreorderDFS(IntervalSet types) => GetNodesWithTypePreorderDFS(new(), types);

    public List<GrammarAST> GetNodesWithTypePreorderDFS(List<GrammarAST> nodes, IntervalSet types)
    {
        if (types.Contains(this.Type)) nodes.Add(this);
        // walk all children of root.
        for (int i = 0; i < ChildCount; i++)
        {
            var child = (GrammarAST)GetChild(i);
            child.GetNodesWithTypePreorderDFS(nodes, types);
        }
        return nodes;
    }

    public GrammarAST GetNodeWithTokenIndex(int index)
    {
        if (this.Token != null && this.Token.TokenIndex == index)
        {
            return this;
        }
        // walk all children of root.
        for (int i = 0; i < ChildCount; i++)
        {
            var child = (GrammarAST)GetChild(i);
            var result = child.GetNodeWithTokenIndex(index);
            if (result != null)
            {
                return result;
            }
        }
        return null;
    }

    public AltAST GetOutermostAltNode()
    {
        if (this is AltAST aST && parent.parent is RuleAST)
        {
            return aST;
        }
        if (parent != null) return ((GrammarAST)parent).GetOutermostAltNode();
        return null;
    }

    /** Walk ancestors of this node until we find ALT with
	 *  alt!=null or leftRecursiveAltInfo!=null. Then grab label if any.
	 *  If not a rule element, just returns null.
	 */
    public string GetAltLabel()
    {
        var ancestors = this.GetAncestors();
        if (ancestors == null) return null;
        for (int i = ancestors.Count - 1; i >= 0; i--)
        {
            var p = (GrammarAST)ancestors[i];
            if (p.Type == ANTLRParser.ALT)
            {
                var a = (AltAST)p;
                if (a.altLabel != null) return a.altLabel.Text;
                if (a.leftRecursiveAltInfo != null)
                {
                    return a.leftRecursiveAltInfo.altLabel;
                }
            }
        }
        return null;
    }

    public bool DeleteChild(Tree t)
    {
        for (int i = 0; i < children.Count; i++)
        {
            var c = children[(i)];
            if (c == t)
            {
                DeleteChild(t.ChildIndex);
                return true;
            }
        }
        return false;
    }

    // TODO: move to basetree when i settle on how runtime works
    // TODO: don't include this node!!
    // TODO: reuse other method
    public CommonTree GetFirstDescendantWithType(int type)
    {
        if (Type == type) return this;
        if (children == null) return null;
        foreach (var c in children)
        {
            var t = (GrammarAST)c;
            if (t.Type == type) return t;
            var d = t.GetFirstDescendantWithType(type);
            if (d != null) return d;
        }
        return null;
    }

    // TODO: don't include this node!!
    public CommonTree GetFirstDescendantWithType(BitSet types)
    {
        if (types.Member(Type)) return this;
        if (children == null) return null;
        foreach (var c in children)
        {
            var t = (GrammarAST)c;
            if (types.Member(t.Type)) return t;
            var d = t.GetFirstDescendantWithType(types);
            if (d != null) return d;
        }
        return null;
    }

    public void SetType(int type)
    {
        token.Type = type;
    }
    //
    //	//@Override
    //	public String getText() {
    //		if ( textOverride!=null ) return textOverride;
    //        if ( token!=null ) {
    //            return token.getText();
    //        }
    //        return "";
    //	}

    public void SetText(string text)
    {
        //		textOverride = text; // don't alt tokens as others might see
        token.//		textOverride = text; // don't alt tokens as others might see
        Text = text; // we delete surrounding tree, so ok to alter
    }

    //	//@Override
    //	public bool equals(object obj) {
    //		return base.Equals(obj);
    //	}

    public override GrammarAST DupNode() => new(this);

    public GrammarAST DupTree()
    {
        var t = this;
        var input = this.token.InputStream;
        var adaptor = new GrammarASTAdaptor(input);
        return (GrammarAST)adaptor.DupTree(t);
    }

    public string ToTokenString()
    {
        var input = this.token.InputStream;
        var adaptor = new GrammarASTAdaptor(input);
        var nodes =
            new CommonTreeNodeStream(adaptor, this);
        var buffer = new StringBuilder();
        var o = (GrammarAST)nodes.LT(1);
        int type = adaptor.getType(o);
        while (type != Token.EOF)
        {
            buffer.Append(' ');
            buffer.Append(o.Text);
            nodes.Consume();
            o = (GrammarAST)nodes.LT(1);
            type = adaptor.getType(o);
        }
        return buffer.ToString();
    }

    public virtual object Visit(GrammarASTVisitor v) => v.Visit(this);
}
