/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */
using org.antlr.v4.runtime.misc;
using org.antlr.v4.runtime.tree;

namespace org.antlr.v4.runtime;


/** A rule invocation record for parsing.
 *
 *  Contains all of the information about the current rule not stored in the
 *  RuleContext. It handles parse tree children list, Any ATN state
 *  tracing, and the default values available for rule invocations:
 *  start, stop, rule index, current alt number.
 *
 *  Subclasses made for each rule and grammar track the parameters,
 *  return values, locals, and labels specific to that rule. These
 *  are the objects that are returned from rules.
 *
 *  Note text is not an actual field of a rule return value; it is computed
 *  from start and stop using the input stream's toString() method.  I
 *  could add a ctor to this so that we can pass in and store the input
 *  stream, but I'm not sure we want to do that.  It would seem to be undefined
 *  to get the .text property anyway if the rule matches tokens from multiple
 *  input streams.
 *
 *  I do not use getters for fields of objects that are used simply to
 *  group values such as this aggregate.  The getters/setters are there to
 *  satisfy the superclass interface.
 */
public class ParserRuleContext : RuleContext
{
    public static readonly ParserRuleContext EMPTY = new ();

    /** If we are debugging or building a parse tree for a visitor,
	 *  we need to track all of the tokens and rule invocations associated
	 *  with this rule's context. This is empty for parsing w/o tree constr.
	 *  operation because we don't the need to track the details about
	 *  how we parse this rule.
	 */
    public List<ParseTree> children;

    /** For debugging/tracing purposes, we want to track all of the nodes in
	 *  the ATN traversed by the parser for a particular rule.
	 *  This list indicates the sequence of ATN nodes used to match
	 *  the elements of the children list. This list does not include
	 *  ATN nodes and other rules used to match rule invocations. It
	 *  traces the rule invocation node itself but nothing inside that
	 *  other rule's ATN submachine.
	 *
	 *  There is NOT a one-to-one correspondence between the children and
	 *  states list. There are typically many nodes in the ATN traversed
	 *  for each element in the children list. For example, for a rule
	 *  invocation there is the invoking state and the following state.
	 *
	 *  The parser setState() method updates field s and adds it to this list
	 *  if we are debugging/tracing.
	 *
	 *  This does not trace states visited during prediction.
	 */
    //	public List<Integer> states;

    public Token start, stop;

    /**
	 * The exception that forced this rule to return. If the rule successfully
	 * completed, this is {@code null}.
	 */
    public RecognitionException exception;

    public ParserRuleContext() { }

    /** COPY a ctx (I'm deliberately not using copy constructor) to avoid
	 *  confusion with creating node with parent. Does not copy children
	 *  (except error leaves).
	 *
	 *  This is used in the generated parser code to flip a generic XContext
	 *  node for rule X to a YContext for alt label Y. In that sense, it is
	 *  not really a generic copy function.
	 *
	 *  If we do an error sync() at start of a rule, we might add error nodes
	 *  to the generic XContext so this function must copy those nodes to
	 *  the YContext as well else they are lost!
	 */
    public void CopyFrom(ParserRuleContext ctx)
    {
        this.parent = ctx.parent;
        this.invokingState = ctx.invokingState;

        this.start = ctx.start;
        this.stop = ctx.stop;

        // copy any error nodes to alt label node
        if (ctx.children != null)
        {
            this.children = new();
            // reset parent pointer for any error nodes
            foreach (ParseTree child in ctx.children)
            {
                if (child is ErrorNode node)
                {
                    AddChild(node);
                }
            }
        }
    }

    public ParserRuleContext(ParserRuleContext parent, int invokingStateNumber) 
        : base(parent, invokingStateNumber)
    {
    }

    // Double dispatch methods for listeners

    public void EnterRule(ParseTreeListener listener) { }
    public void ExitRule(ParseTreeListener listener) { }

    /** Add a parse tree node to this as a child.  Works for
	 *  internal and leaf nodes. Does not set parent link;
	 *  other add methods must do that. Other addChild methods
	 *  call this.
	 *
	 *  We cannot set the parent pointer of the incoming node
	 *  because the existing interfaces do not have a setParent()
	 *  method and I don't want to break backward compatibility for this.
	 *
	 *  @since 4.7
	 */
    public T AddAnyChild<T>(T t) where T : ParseTree
    {
        children ??= new();
        children.Add(t);
        return t;
    }

    public RuleContext AddChild(RuleContext ruleInvocation)
    {
        return AddAnyChild(ruleInvocation);
    }

    /** Add a token leaf node child and force its parent to be this node. */
    public TerminalNode AddChild(TerminalNode t)
    {
        t.SetParent(this);
        return AddAnyChild(t);
    }

    /** Add an error node child and force its parent to be this node.
	 *
	 * @since 4.7
	 */
    public ErrorNode addErrorNode(ErrorNode errorNode)
    {
        errorNode.SetParent(this);
        return AddAnyChild(errorNode);
    }

    /** Add a child to this node based upon matchedToken. It
	 *  creates a TerminalNodeImpl rather than using
	 *  {@link Parser#createTerminalNode(ParserRuleContext, Token)}. I'm leaving this
     *  in for compatibility but the parser doesn't use this anymore.
	 */
    //@Deprecated
    public TerminalNode AddChild(Token matchedToken)
    {
        var t = new TerminalNodeImpl(matchedToken);
        AddAnyChild(t);
        t.SetParent(this);
        return t;
    }

    /** Add a child to this node based upon badToken.  It
	 *  creates a ErrorNodeImpl rather than using
	 *  {@link Parser#createErrorNode(ParserRuleContext, Token)}. I'm leaving this
	 *  in for compatibility but the parser doesn't use this anymore.
	 */
    //@Deprecated
    public ErrorNode AddErrorNode(Token badToken)
    {
        var t = new ErrorNodeImpl(badToken);
        AddAnyChild(t);
        t.SetParent(this);
        return t;
    }

    //	public void trace(int s) {
    //		if ( states==null ) states = new ArrayList<Integer>();
    //		states.add(s);
    //	}

    /** Used by enterOuterAlt to toss out a RuleContext previously added as
	 *  we entered a rule. If we have # label, we will need to remove
	 *  generic ruleContext object.
	 */
    public void RemoveLastChild()
    {
        children?.RemoveAt(children.Count - 1);
    }

    ////@Override
    /** Override to make type more specific */
    public ParserRuleContext GetParent()
    {
        return (ParserRuleContext)base.getParent();
    }

    ////@Override
    public ParseTree GetChild(int i)
    {
        return children != null && i >= 0 && i < children.Count ? children[(i)] : null;
    }

    public T GetChild<T>(Type ctxType, int i) where T : class
    {
        if (children == null || i < 0 || i >= children.Count)
        {
            return default;
        }

        int j = -1; // what element have we found with ctxType?
        foreach (var o in children)
        {
            if (ctxType.IsInstanceOfType(o))
            {
                j++;
                if (j == i)
                {
                    return o as T;
                }
            }
        }
        return default;
    }

    public TerminalNode GetToken(int ttype, int i)
    {
        if (children == null || i < 0 || i >= children.Count)
        {
            return null;
        }

        int j = -1; // what token with ttype have we found?
        foreach (var o in children)
        {
            if (o is TerminalNode tnode)
            {
                var symbol = tnode.getSymbol();
                if (symbol.Type == ttype)
                {
                    j++;
                    if (j == i)
                    {
                        return tnode;
                    }
                }
            }
        }

        return null;
    }

    public List<TerminalNode> GetTokens(int ttype)
    {
        if (children == null)
        {
            return new List<TerminalNode>();
        }

        List<TerminalNode> tokens = null;
        foreach (ParseTree o in children)
        {
            if (o is TerminalNode)
            {
                TerminalNode tnode = (TerminalNode)o;
                Token symbol = tnode.getSymbol();
                if (symbol.Type == ttype)
                {
                    if (tokens == null)
                    {
                        tokens = new();
                    }
                    tokens.Add(tnode);
                }
            }
        }

        if (tokens == null)
        {
            return new List<TerminalNode>();
        }

        return tokens;
    }

    public T GetRuleContext<T>(Type ctxType, int i) where T : ParserRuleContext
    {
        return GetChild<T>(ctxType, i);
    }

    public List<T> GetRuleContexts<T>(Type ctxType) where T : class
    {
        if (children == null)
        {
            return new List<T>();
        }

        List<T> contexts = null;
        foreach (ParseTree o in children)
        {
            if (ctxType.IsInstanceOfType(o))
            {
                if (contexts == null)
                {
                    contexts = new();
                }

                contexts.Add(o as T);
            }
        }

        if (contexts == null)
        {
            return new();
        }

        return contexts;
    }

    //@Override
    public int GetChildCount() { return children != null ? children.Count : 0; }

    //@Override
    public Interval GetSourceInterval()
    {
        if (start == null)
        {
            return Interval.INVALID;
        }
        if (stop == null || stop.TokenIndex < start.TokenIndex)
        {
            return Interval.Of(start.TokenIndex, start.TokenIndex - 1); // empty
        }
        return Interval.Of(start.TokenIndex, stop.TokenIndex);
    }

    /**
	 * Get the initial token in this context.
	 * Note that the range from start to stop is inclusive, so for rules that do not consume anything
	 * (for example, zero length or error productions) this token may exceed stop.
	 */
    public Token GetStart() { return start; }
    /**
	 * Get the final token in this context.
	 * Note that the range from start to stop is inclusive, so for rules that do not consume anything
	 * (for example, zero length or error productions) this token may precede start.
	 */
    public Token GetStop() { return stop; }

    /** Used for rule context info debugging during parse-time, not so much for ATN debugging */
    public string ToInfoString(Parser recognizer)
    {
        var rules = recognizer.getRuleInvocationStack(this);
        rules.Reverse();
        return "ParserRuleContext" + rules + "{" +
            "start=" + start +
            ", stop=" + stop +
            '}';
    }
}

