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

public class GrammarAST : CommonTree {
	/** For error msgs, nice to know which grammar this AST lives in */
	// TODO: try to remove
	public Grammar g;

	/** If we build an ATN, we make AST node point at left edge of ATN construct */
	public ATNState atnState;

	public String textOverride;

    public GrammarAST() {}
    public GrammarAST(Token t):base(t) { ; }
    public GrammarAST(GrammarAST node):base(node) {
		this.g = node.g;
		this.atnState = node.atnState;
		this.textOverride = node.textOverride;
	}
    public GrammarAST(int type) :base(new CommonToken(type, ANTLRParser.tokenNames[type])){}
    public GrammarAST(int type, Token t): this(new CommonToken(t))
    {
		;
		token.setType(type);
	}
    public GrammarAST(int type, Token t, String text) : this(new CommonToken(t))
    {
		;
		token.setType(type);
		token.setText(text);
    }

	public GrammarAST[] getChildrenAsArray() {
		return children.ToArray();
	}

	public List<GrammarAST> getNodesWithType(int ttype) {
		return getNodesWithType(IntervalSet.of(ttype));
	}

	public List<GrammarAST> getAllChildrenWithType(int type) {
		List<GrammarAST> nodes = new ();
		for (int i = 0; children!=null && i < children.Count; i++) {
			Tree t = (Tree) children[(i)];
			if ( t.getType()==type ) {
				nodes.Add((GrammarAST)t);
			}
		}
		return nodes;
	}

	public List<GrammarAST> getNodesWithType(IntervalSet types) {
		List<GrammarAST> nodes = new();
		List<GrammarAST> work = new LinkedList<GrammarAST>();
		work.Add(this);
		GrammarAST t;
		while ( !work.isEmpty() ) {
			t = work.remove(0);
			if ( types==null || types.contains(t.getType()) ) nodes.add(t);
			if ( t.children!=null ) {
				work.AddRange(Arrays.AsList(t.getChildrenAsArray()));
			}
		}
		return nodes;
	}

	public List<GrammarAST> getNodesWithTypePreorderDFS(IntervalSet types) {
		List<GrammarAST> nodes = new ();
		getNodesWithTypePreorderDFS_(nodes, types);
		return nodes;
	}

	public void getNodesWithTypePreorderDFS_(List<GrammarAST> nodes, IntervalSet types) {
		if ( types.contains(this.getType()) ) nodes.Add(this);
		// walk all children of root.
		for (int i= 0; i < getChildCount(); i++) {
			GrammarAST child = (GrammarAST)getChild(i);
			child.getNodesWithTypePreorderDFS_(nodes, types);
		}
	}

	public GrammarAST getNodeWithTokenIndex(int index) {
		if ( this.getToken()!=null && this.getToken().getTokenIndex()==index ) {
			return this;
		}
		// walk all children of root.
		for (int i= 0; i < getChildCount(); i++) {
			GrammarAST child = (GrammarAST)getChild(i);
			GrammarAST result = child.getNodeWithTokenIndex(index);
			if ( result!=null ) {
				return result;
			}
		}
		return null;
	}

	public AltAST getOutermostAltNode() {
		if ( this is AltAST && parent.parent is RuleAST ) {
			return (AltAST)this;
		}
		if ( parent!=null ) return ((GrammarAST)parent).getOutermostAltNode();
		return null;
	}

	/** Walk ancestors of this node until we find ALT with
	 *  alt!=null or leftRecursiveAltInfo!=null. Then grab label if any.
	 *  If not a rule element, just returns null.
	 */
	public String getAltLabel() {
		List<Tree> ancestors = this.getAncestors();
		if ( ancestors==null ) return null;
		for (int i=ancestors.Count-1; i>=0; i--) {
			GrammarAST p = (GrammarAST)ancestors[i];
			if ( p.getType()== ANTLRParser.ALT ) {
				AltAST a = (AltAST)p;
				if ( a.altLabel!=null ) return a.altLabel.getText();
				if ( a.leftRecursiveAltInfo!=null ) {
					return a.leftRecursiveAltInfo.altLabel;
				}
			}
		}
		return null;
	}

	public bool deleteChild(Tree t) {
		for (int i=0; i<children.Count; i++) {
			Object c = children[(i)];
			if ( c == t ) {
				deleteChild(t.getChildIndex());
				return true;
			}
		}
		return false;
	}

    // TODO: move to basetree when i settle on how runtime works
    // TODO: don't include this node!!
	// TODO: reuse other method
    public CommonTree getFirstDescendantWithType(int type) {
        if ( getType()==type ) return this;
        if ( children==null ) return null;
        foreach (Object c in children) {
            GrammarAST t = (GrammarAST)c;
            if ( t.getType()==type ) return t;
            CommonTree d = t.getFirstDescendantWithType(type);
            if ( d!=null ) return d;
        }
        return null;
    }

	// TODO: don't include this node!!
	public CommonTree getFirstDescendantWithType(BitSet types) {
		if ( types.member(getType()) ) return this;
		if ( children==null ) return null;
		foreach (Object c in children) {
			GrammarAST t = (GrammarAST)c;
			if ( types.member(t.getType()) ) return t;
			CommonTree d = t.getFirstDescendantWithType(types);
			if ( d!=null ) return d;
		}
		return null;
	}

	public void setType(int type) {
		token.setType(type);
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

	public void setText(String text) {
//		textOverride = text; // don't alt tokens as others might see
		token.setText(text); // we delete surrounding tree, so ok to alter
	}

//	//@Override
//	public bool equals(Object obj) {
//		return base.Equals(obj);
//	}

	//@Override
    public GrammarAST dupNode() {
        return new GrammarAST(this);
    }

	public GrammarAST dupTree() {
		GrammarAST t = this;
		CharStream input = this.token.getInputStream();
		GrammarASTAdaptor adaptor = new GrammarASTAdaptor(input);
		return (GrammarAST)adaptor.dupTree(t);
	}

	public String toTokenString() {
		CharStream input = this.token.getInputStream();
		GrammarASTAdaptor adaptor = new GrammarASTAdaptor(input);
		CommonTreeNodeStream nodes =
			new CommonTreeNodeStream(adaptor, this);
		StringBuilder buf = new StringBuilder();
		GrammarAST o = (GrammarAST)nodes.LT(1);
		int type = adaptor.getType(o);
		while ( type!=Token.EOF ) {
			buf.Append(" ");
			buf.Append(o.getText());
			nodes.consume();
			o = (GrammarAST)nodes.LT(1);
			type = adaptor.getType(o);
		}
		return buf.ToString();
	}

	public Object visit(GrammarASTVisitor v) { return v.visit(this); }
}
