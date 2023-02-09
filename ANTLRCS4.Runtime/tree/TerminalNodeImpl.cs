/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.runtime.tree;
using org.antlr.v4.runtime.misc;

namespace org.antlr.v4.runtime.tree;

public class TerminalNodeImpl : TerminalNode
{
    public Token symbol;
    public ParseTree parent;

    public TerminalNodeImpl(Token symbol) { this.symbol = symbol; }

    //@Override
    public ParseTree GetChild(int i) { return null; }

    //@Override
    public Token getSymbol() { return symbol; }

    //@Override
    public ParseTree Parent => parent;

    //@Override
    public Token getPayload() { return symbol; }

    //@Override
    public Interval SourceInterval
    {
        get
        {
            if (symbol == null) return Interval.INVALID;

            int tokenIndex = symbol.TokenIndex;
            return new Interval(tokenIndex, tokenIndex);
        }
    }

    //@Override
    public int ChildCount => 0;
    public T Accept<T>(ParseTreeVisitor<T> visitor)
    {
        return visitor.VisitTerminal(this);
    }

    public string Text => symbol.Text;
    public string ToStringTree(Parser parser)
    {
        return ToString();
    }

    public override string ToString()
    {
        if (symbol.Type == Token.EOF) return "<EOF>";
        return symbol.Text;
    }

    //@Override
    public string ToStringTree()
    {
        return ToString();
    }

    Tree Tree.Parent { get => this.Parent; set { } }

    object Tree.Payload => this.getPayload();

    Tree Tree.GetChild(int i)
    {
        return this.GetChild(i);
    }

    public int Type => throw new NotImplementedException();

    public int TokenStartIndex { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public int TokenStopIndex { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public void ReplaceChildren(int startChildIndex, int stopChildIndex, object t)
    {
        throw new NotImplementedException();
    }

    public object DupNode()
    {
        throw new NotImplementedException();
    }

    public object DeleteChild(int i)
    {
        throw new NotImplementedException();
    }

    public int ChildIndex { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public bool IsNil => throw new NotImplementedException();

    public void AddChild(Tree child)
    {
        throw new NotImplementedException();
    }

    public void SetChild(int i, Tree child)
    {
        throw new NotImplementedException();
    }

    public int Line => throw new NotImplementedException();

    public int CharPositionInLine => throw new NotImplementedException();
}
