/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.runtime.tree;
using org.antlr.v4.runtime.misc;

namespace org.antlr.v4.runtime.tree;

public class TerminalNodeImpl : TerminalNode {
	public Token symbol;
	public ParseTree parent;

	public TerminalNodeImpl(Token symbol) {	this.symbol = symbol;	}

	//@Override
	public ParseTree getChild(int i) {return null;}

	//@Override
	public Token getSymbol() {return symbol;}

	//@Override
	public ParseTree getParent() { return parent; }

	//@Override
	public void setParent(RuleContext parent) {
		this.parent = parent;
	}

	//@Override
	public Token getPayload() { return symbol; }

    //@Override
    public Interval getSourceInterval() {
		if ( symbol ==null ) return Interval.INVALID;

		int tokenIndex = symbol.getTokenIndex();
		return new Interval(tokenIndex, tokenIndex);
	}

    //@Override
    public int getChildCount() { return 0; }

    //@Override
    public  T accept<T>(ParseTreeVisitor<T> visitor) {
		return visitor.visitTerminal(this);
    }

	//@Override
    public String getText() { return symbol.getText(); }

    //@Override
    public String toStringTree(Parser parser) {
		return ToString();
	}

    //@Override
    public override String ToString() {
			if ( symbol.getType() == Token.EOF ) return "<EOF>";
			return symbol.getText();
	}

    //@Override
    public String toStringTree() {
		return ToString();
	}

    Tree Tree.getParent()
    {
        return this.getParent();
    }

    object Tree.getPayload()
    {
		return this.getPayload();
    }

    Tree Tree.getChild(int i)
    {
		return this.getChild(i);	
    }

    public int getType()
    {
        throw new NotImplementedException();
    }

    public void setParent(Tree baseTree)
    {
        throw new NotImplementedException();
    }

    public void setChildIndex(int i)
    {
        throw new NotImplementedException();
    }

    public void setTokenStartIndex(int start)
    {
        throw new NotImplementedException();
    }

    public void setTokenStopIndex(int stop)
    {
        throw new NotImplementedException();
    }

    public int getTokenStartIndex()
    {
        throw new NotImplementedException();
    }

    public int getTokenStopIndex()
    {
        throw new NotImplementedException();
    }

    public void replaceChildren(int startChildIndex, int stopChildIndex, object t)
    {
        throw new NotImplementedException();
    }

    public object dupNode()
    {
        throw new NotImplementedException();
    }

    public object deleteChild(int i)
    {
        throw new NotImplementedException();
    }

    public int getChildIndex()
    {
        throw new NotImplementedException();
    }

    public bool isNil()
    {
        throw new NotImplementedException();
    }

    public void addChild(Tree child)
    {
        throw new NotImplementedException();
    }

    public void setChild(int i, Tree child)
    {
        throw new NotImplementedException();
    }

    public int getLine()
    {
        throw new NotImplementedException();
    }

    public int getCharPositionInLine()
    {
        throw new NotImplementedException();
    }
}
