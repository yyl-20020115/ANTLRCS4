/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */
using org.antlr.v4.runtime.atn;
using org.antlr.v4.runtime.misc;
using org.antlr.v4.runtime.tree;
using System.Text;

namespace org.antlr.v4.runtime;


/** A rule context is a record of a single rule invocation.
 *
 *  We form a stack of these context objects using the parent
 *  pointer. A parent pointer of null indicates that the current
 *  context is the bottom of the stack. The ParserRuleContext subclass
 *  as a children list so that we can turn this data structure into a
 *  tree.
 *
 *  The root node always has a null pointer and invokingState of -1.
 *
 *  Upon entry to parsing, the first invoked rule function creates a
 *  context object (a subclass specialized for that rule such as
 *  SContext) and makes it the root of a parse tree, recorded by field
 *  Parser._ctx.
 *
 *  public final SContext s() throws RecognitionException {
 *      SContext _localctx = new SContext(_ctx, getState()); <-- create new node
 *      enterRule(_localctx, 0, RULE_s);                     <-- push it
 *      ...
 *      exitRule();                                          <-- pop back to _localctx
 *      return _localctx;
 *  }
 *
 *  A subsequent rule invocation of r from the start rule s pushes a
 *  new context object for r whose parent points at s and use invoking
 *  state is the state with r emanating as edge label.
 *
 *  The invokingState fields from a context object to the root
 *  together form a stack of rule indication states where the root
 *  (bottom of the stack) has a -1 sentinel value. If we invoke start
 *  symbol s then call r1, which calls r2, the  would look like
 *  this:
 *
 *     SContext[-1]   <- root node (bottom of the stack)
 *     R1Context[p]   <- p in rule s called r1
 *     R2Context[q]   <- q in rule r1 called r2
 *
 *  So the top of the stack, _ctx, represents a call to the current
 *  rule and it holds the return address from another rule that invoke
 *  to this rule. To invoke a rule, we must always have a current context.
 *
 *  The parent contexts are useful for computing lookahead sets and
 *  getting error information.
 *
 *  These objects are used during parsing and prediction.
 *  For the special case of parsers, we use the subclass
 *  ParserRuleContext.
 *
 *  @see ParserRuleContext
 */
public class RuleContext : RuleNode
{
    /** What context invoked this rule? */
    public RuleContext parent;

    /** What state invoked the rule associated with this context?
	 *  The "return address" is the followState of invokingState
	 *  If parent is null, this should be -1 this context object represents
	 *  the start rule.
	 */
    public int invokingState = -1;

    public RuleContext() { }

    public RuleContext(RuleContext parent, int invokingState)
    {
        this.parent = parent;
        //if ( parent!=null ) Console.Out.WriteLine("invoke "+stateNumber+" from "+parent);
        this.invokingState = invokingState;
    }

    public int Depth()
    {
        int n = 0;
        RuleContext p = this;
        while (p != null)
        {
            p = p.parent;
            n++;
        }
        return n;
    }

    /** A context is empty if there is no invoking state; meaning nobody called
	 *  current context.
	 */
    public bool IsEmpty => invokingState == -1;

    // satisfy the ParseTree / SyntaxTree interface

    //@Override
    public virtual Interval SourceInterval => Interval.INVALID;

    //@Override
    public virtual RuleContext CurrentRuleContext => this;
    //@Override
    /** @since 4.7. {@see ParseTree#setParent} comment */
    //@Override
    public virtual RuleContext Parent { get => parent; set => this.parent = value; }

    //@Override
    public virtual RuleContext Payload => this;
    /** Return the combined text of all child nodes. This method only considers
	 *  tokens which have been added to the parse tree.
	 *  <p>
	 *  Since tokens on hidden channels (e.g. whitespace or comments) are not
	 *  added to the parse trees, they will not appear in the output of this
	 *  method.
	 */
    //@Override
    public string Text
    {
        get
        {
            if (ChildCount == 0)
            {
                return "";
            }

            var builder = new StringBuilder();
            for (int i = 0; i < ChildCount; i++)
            {
                builder.Append(GetChild(i).Text);
            }

            return builder.ToString();
        }
    }

    public virtual int RuleIndex => -1;
    /** For rule associated with this parse tree internal node, return
	 *  the outer alternative number used to match the input. Default
	 *  implementation does not compute nor store this alt num. Create
	 *  a subclass of ParserRuleContext with backing field and set
	 *  option contextSuperClass.
	 *  to set it.
	 *
	 *  @since 4.5.3
	 */
    /** Set the outer alternative number for this context node. Default
 *  implementation does nothing to avoid backing field overhead for
 *  trees that don't need it.  Create
 *  a subclass of ParserRuleContext with backing field and set
 *  option contextSuperClass.
 *
 *  @since 4.5.3
 */
    public virtual int AltNumber
    {
        get => ATN.INVALID_ALT_NUMBER;
        set
        { }
    }

    //@Override
    public virtual ParseTree GetChild(int i)
    {
        return null;
    }

    //@Override
    public int ChildCount => 0;

    //@Override
    public virtual T Accept<T>(ParseTreeVisitor<T> visitor) { return visitor.VisitChildren(this); }

    /** Print out a whole tree, not just a node, in LISP format
	 *  (root child1 .. childN). Print just a node if this is a leaf.
	 *  We have to know the recognizer so we can get rule names.
	 */
    //@Override
    public virtual string ToStringTree(Parser recog)
    {
        return Trees.ToStringTree(this, recog);
    }

    /** Print out a whole tree, not just a node, in LISP format
	 *  (root child1 .. childN). Print just a node if this is a leaf.
	 */
    public virtual string ToStringTree(List<String> ruleNames)
    {
        return Trees.ToStringTree(this, ruleNames);
    }

    //@Override
    public virtual string ToStringTree()
    {
        return ToStringTree((List<String>)null);
    }

    //@Override
    public override string ToString()
    {
        return ToString((List<String>)null, (RuleContext)null);
    }

    public virtual string ToString(Recognizer recog)
    {
        return ToString(recog, ParserRuleContext.EMPTY);
    }

    public virtual string ToString(List<string> ruleNames)
    {
        return ToString(ruleNames, null);
    }

    // recog null unless ParserRuleContext, in which case we use subclass toString(...)
    public virtual string ToString(Recognizer recog, RuleContext stop)
    {
        var ruleNames = recog != null ? recog.RuleNames : null;
        List<string> ruleNamesList = ruleNames != null ? new List<string>(ruleNames) : null;
        return ToString(ruleNamesList, stop);
    }

    public virtual string ToString(List<String> ruleNames, RuleContext stop)
    {
        var buffer = new StringBuilder();
        RuleContext p = this;
        buffer.Append('[');
        while (p != null && p != stop)
        {
            if (ruleNames == null)
            {
                if (!p.IsEmpty)
                {
                    buffer.Append(p.invokingState);
                }
            }
            else
            {
                int ruleIndex = p.RuleIndex;
                String ruleName = ruleIndex >= 0 && ruleIndex < ruleNames.Count ? ruleNames[ruleIndex] :
                    (ruleIndex.ToString());
                buffer.Append(ruleName);
            }

            if (p.parent != null && (ruleNames != null || !p.parent.IsEmpty))
            {
                buffer.Append(' ');
            }

            p = p.parent;
        }

        buffer.Append(']');
        return buffer.ToString();
    }

    ParseTree ParseTree.Parent => throw new NotImplementedException();

    object Tree.Payload => throw new NotImplementedException();

    Tree Tree.GetChild(int i)
    {
        throw new NotImplementedException();
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

    Tree Tree.Parent { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
}
