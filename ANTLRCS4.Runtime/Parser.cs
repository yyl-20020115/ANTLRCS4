/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */
using org.antlr.runtime;
using org.antlr.v4.runtime.atn;
using org.antlr.v4.runtime.dfa;
using org.antlr.v4.runtime.misc;
using org.antlr.v4.runtime.tree;
using org.antlr.v4.runtime.tree.pattern;

namespace org.antlr.v4.runtime;

/** This is all the parsing support code essentially; most of it is error recovery stuff. */
public abstract class Parser : Recognizer<Token, ParserATNSimulator>
{
    public class TraceListener : ParseTreeListener
    {
        public readonly Parser parser;

        public TraceListener(Parser parser)
        {
            this.parser = parser;
        }
        //@Override
        public void enterEveryRule(ParserRuleContext ctx)
        {
            Console.Out.WriteLine("enter   " + this.parser.getRuleNames()[ctx.getRuleIndex()] +
                               ", LT(1)=" + this.parser.input.LT(1).getText());
        }

        //@Override
        public void visitTerminal(TerminalNode node)
        {
            Console.Out.WriteLine("consume " + node.getSymbol() + " rule " +
                               this.parser.getRuleNames()[this.parser._ctx.getRuleIndex()]);
        }

        //@Override
        public void visitErrorNode(ErrorNode node)
        {
        }

        //@Override
        public void exitEveryRule(ParserRuleContext ctx)
        {
            Console.Out.WriteLine("exit    " + this.parser.getRuleNames()[ctx.getRuleIndex()] +
                               ", LT(1)=" + this.parser.input.LT(1).getText());
        }
    }

    public class TrimToSizeListener : ParseTreeListener
    {
        public static readonly TrimToSizeListener INSTANCE = new TrimToSizeListener();

        //@Override
        public void enterEveryRule(ParserRuleContext ctx) { }

        //@Override
        public void visitTerminal(TerminalNode node) { }

        //@Override
        public void visitErrorNode(ErrorNode node) { }

        //@Override
        public void exitEveryRule(ParserRuleContext ctx)
        {
            if (ctx.children is List<ParseTree> ch)
            {
                ch.TrimExcess();
            }
        }
    }

    /**
	 * This field holds the deserialized {@link ATN} with bypass alternatives, created
	 * lazily upon first demand. In 4.10 I changed from map<serializedATNstring, ATN>
	 * since we only need one per parser object and also it complicates other targets
	 * that don't use ATN strings.
	 *
	 * @see ATNDeserializationOptions#isGenerateRuleBypassTransitions()
	 */
    private ATN bypassAltsAtnCache;

    /**
	 * The error handling strategy for the parser. The default value is a new
	 * instance of {@link DefaultErrorStrategy}.
	 *
	 * @see #getErrorHandler
	 * @see #setErrorHandler
	 */

    protected ANTLRErrorStrategy _errHandler = new DefaultErrorStrategy();

    /**
	 * The input stream.
	 *
	 * @see #getInputStream
	 * @see #setInputStream
	 */
    protected TokenStream input;

    protected readonly IntegerStack _precedenceStack = new IntegerStack();
    //{
    //	_precedenceStack = new IntegerStack();
    //	_precedenceStack.push(0);
    //}

    /**
	 * The {@link ParserRuleContext} object for the currently executing rule.
	 * This is always non-null during the parsing process.
	 */
    protected ParserRuleContext _ctx;
    public ParserRuleContext GetCtx() { return _ctx; }
    /**
	 * Specifies whether or not the parser should construct a parse tree during
	 * the parsing process. The default value is {@code true}.
	 *
	 * @see #getBuildParseTree
	 * @see #setBuildParseTree
	 */
    protected bool _buildParseTrees = true;


    /**
	 * When {@link #setTrace}{@code (true)} is called, a reference to the
	 * {@link TraceListener} is stored here so it can be easily removed in a
	 * later call to {@link #setTrace}{@code (false)}. The listener itself is
	 * implemented as a parser listener so this field is not directly used by
	 * other parser methods.
	 */
    private TraceListener _tracer;

    /**
	 * The list of {@link ParseTreeListener} listeners registered to receive
	 * events during the parse.
	 *
	 * @see #addParseListener
	 */
    protected List<ParseTreeListener> _parseListeners;

    /**
	 * The number of syntax errors reported during parsing. This value is
	 * incremented each time {@link #notifyErrorListeners} is called.
	 */
    protected int _syntaxErrors;

    /** Indicates parser has match()ed EOF token. See {@link #exitRule()}. */
    protected bool matchedEOF;

    public Parser(TokenStream input)
    {
        _precedenceStack.push(0);

        setInputStream(input);
    }
    protected readonly RecognizerSharedState state;
    public Parser(TokenStream input, RecognizerSharedState state)
        : this(input)
    {
        this.state = state;
    }
    /** reset the parser's state */
    public virtual void reset()
    {
        if (getInputStream() != null) getInputStream().seek(0);
        _errHandler.reset(this);
        _ctx = null;
        _syntaxErrors = 0;
        matchedEOF = false;
        setTrace(false);
        _precedenceStack.clear();
        _precedenceStack.push(0);
        ATNSimulator interpreter = getInterpreter();
        if (interpreter != null)
        {
            interpreter.reset();
        }
    }
    public Token match(int ttype)
    {
        Token t = getCurrentToken();
        if (t.getType() == ttype)
        {
            if (ttype == Token.EOF)
            {
                matchedEOF = true;
            }
            _errHandler.reportMatch(this);
            consume();
        }
        else
        {
            t = _errHandler.recoverInline(this);
            if (_buildParseTrees && t.getTokenIndex() == -1)
            {
                // we must have conjured up a new token during single token insertion
                // if it's not the current symbol
                _ctx.addErrorNode(createErrorNode(_ctx, t));
            }
        }
        return t;
    }
    /**
	 * Match current input symbol against {@code ttype}. If the symbol type
	 * matches, {@link ANTLRErrorStrategy#reportMatch} and {@link #consume} are
	 * called to complete the match process.
	 *
	 * <p>If the symbol type does not match,
	 * {@link ANTLRErrorStrategy#recoverInline} is called on the current error
	 * strategy to attempt recovery. If {@link #getBuildParseTree} is
	 * {@code true} and the token index of the symbol returned by
	 * {@link ANTLRErrorStrategy#recoverInline} is -1, the symbol is added to
	 * the parse tree by calling {@link #createErrorNode(ParserRuleContext, Token)} then
	 * {@link ParserRuleContext#addErrorNode(ErrorNode)}.</p>
	 *
	 * @param ttype the token type to match
	 * @return the matched symbol
	 * @ if the current input symbol did not match
	 * {@code ttype} and the error strategy could not recover from the
	 * mismatched symbol
	 */
    public Token match(TokenStream input, int ttype, BitSet fOLLOW_ACTION_in_optionValue890)
    {
        Token t = getCurrentToken();
        if (t.getType() == ttype)
        {
            if (ttype == Token.EOF)
            {
                matchedEOF = true;
            }
            _errHandler.reportMatch(this);
            consume();
        }
        else
        {
            t = _errHandler.recoverInline(this);
            if (_buildParseTrees && t.getTokenIndex() == -1)
            {
                // we must have conjured up a new token during single token insertion
                // if it's not the current symbol
                _ctx.addErrorNode(createErrorNode(_ctx, t));
            }
        }
        return t;
    }

    /**
	 * Match current input symbol as a wildcard. If the symbol type matches
	 * (i.e. has a value greater than 0), {@link ANTLRErrorStrategy#reportMatch}
	 * and {@link #consume} are called to complete the match process.
	 *
	 * <p>If the symbol type does not match,
	 * {@link ANTLRErrorStrategy#recoverInline} is called on the current error
	 * strategy to attempt recovery. If {@link #getBuildParseTree} is
	 * {@code true} and the token index of the symbol returned by
	 * {@link ANTLRErrorStrategy#recoverInline} is -1, the symbol is added to
	 * the parse tree by calling {@link Parser#createErrorNode(ParserRuleContext, Token)}. then
     * {@link ParserRuleContext#addErrorNode(ErrorNode)}</p>
	 *
	 * @return the matched symbol
	 * @ if the current input symbol did not match
	 * a wildcard and the error strategy could not recover from the mismatched
	 * symbol
	 */
    public Token matchWildcard()
    {
        Token t = getCurrentToken();
        if (t.getType() > 0)
        {
            _errHandler.reportMatch(this);
            consume();
        }
        else
        {
            t = _errHandler.recoverInline(this);
            if (_buildParseTrees && t.getTokenIndex() == -1)
            {
                // we must have conjured up a new token during single token insertion
                // if it's not the current symbol
                _ctx.addErrorNode(createErrorNode(_ctx, t));
            }
        }

        return t;
    }

    /**
	 * Track the {@link ParserRuleContext} objects during the parse and hook
	 * them up using the {@link ParserRuleContext#children} list so that it
	 * forms a parse tree. The {@link ParserRuleContext} returned from the start
	 * rule represents the root of the parse tree.
	 *
	 * <p>Note that if we are not building parse trees, rule contexts only point
	 * upwards. When a rule exits, it returns the context but that gets garbage
	 * collected if nobody holds a reference. It points upwards but nobody
	 * points at it.</p>
	 *
	 * <p>When we build parse trees, we are adding all of these contexts to
	 * {@link ParserRuleContext#children} list. Contexts are then not candidates
	 * for garbage collection.</p>
	 */
    public void setBuildParseTree(bool buildParseTrees)
    {
        this._buildParseTrees = buildParseTrees;
    }

    /**
	 * Gets whether or not a complete parse tree will be constructed while
	 * parsing. This property is {@code true} for a newly constructed parser.
	 *
	 * @return {@code true} if a complete parse tree will be constructed while
	 * parsing, otherwise {@code false}
	 */
    public bool getBuildParseTree()
    {
        return _buildParseTrees;
    }

    /**
	 * Trim the internal lists of the parse tree during parsing to conserve memory.
	 * This property is set to {@code false} by default for a newly constructed parser.
	 *
	 * @param trimParseTrees {@code true} to trim the capacity of the {@link ParserRuleContext#children}
	 * list to its size after a rule is parsed.
	 */
    public void setTrimParseTree(bool trimParseTrees)
    {
        if (trimParseTrees)
        {
            if (getTrimParseTree()) return;
            addParseListener(TrimToSizeListener.INSTANCE);
        }
        else
        {
            removeParseListener(TrimToSizeListener.INSTANCE);
        }
    }

    /**
	 * @return {@code true} if the {@link ParserRuleContext#children} list is trimmed
	 * using the default {@link Parser.TrimToSizeListener} during the parse process.
	 */
    public bool getTrimParseTree()
    {
        return getParseListeners().Contains(TrimToSizeListener.INSTANCE);
    }


    public List<ParseTreeListener> getParseListeners()
    {
        List<ParseTreeListener> listeners = _parseListeners;
        if (listeners == null)
        {
            return new();// Collections.emptyList();
        }

        return listeners;
    }

    /**
	 * Registers {@code listener} to receive events during the parsing process.
	 *
	 * <p>To support output-preserving grammar transformations (including but not
	 * limited to left-recursion removal, automated left-factoring, and
	 * optimized code generation), calls to listener methods during the parse
	 * may differ substantially from calls made by
	 * {@link ParseTreeWalker#DEFAULT} used after the parse is complete. In
	 * particular, rule entry and exit events may occur in a different order
	 * during the parse than after the parser. In addition, calls to certain
	 * rule entry methods may be omitted.</p>
	 *
	 * <p>With the following specific exceptions, calls to listener events are
	 * <em>deterministic</em>, i.e. for identical input the calls to listener
	 * methods will be the same.</p>
	 *
	 * <ul>
	 * <li>Alterations to the grammar used to generate code may change the
	 * behavior of the listener calls.</li>
	 * <li>Alterations to the command line options passed to ANTLR 4 when
	 * generating the parser may change the behavior of the listener calls.</li>
	 * <li>Changing the version of the ANTLR Tool used to generate the parser
	 * may change the behavior of the listener calls.</li>
	 * </ul>
	 *
	 * @param listener the listener to add
	 *
	 * @throws NullReferenceException if {@code} listener is {@code null}
	 */
    public void addParseListener(ParseTreeListener listener)
    {
        if (listener == null)
        {
            throw new NullReferenceException(nameof(listener));
        }

        if (_parseListeners == null)
        {
            _parseListeners = new();
        }

        this._parseListeners.Add(listener);
    }

    /**
	 * Remove {@code listener} from the list of parse listeners.
	 *
	 * <p>If {@code listener} is {@code null} or has not been added as a parse
	 * listener, this method does nothing.</p>
	 *
	 * @see #addParseListener
	 *
	 * @param listener the listener to remove
	 */
    public void removeParseListener(ParseTreeListener listener)
    {
        if (_parseListeners != null)
        {
            if (_parseListeners.Remove(listener))
            {
                if (_parseListeners.Count == 0)
                {
                    _parseListeners = null;
                }
            }
        }
    }

    /**
	 * Remove all parse listeners.
	 *
	 * @see #addParseListener
	 */
    public void removeParseListeners()
    {
        _parseListeners = null;
    }

    /**
	 * Notify any parse listeners of an enter rule event.
	 *
	 * @see #addParseListener
	 */
    protected void triggerEnterRuleEvent()
    {
        foreach (ParseTreeListener listener in _parseListeners)
        {
            listener.enterEveryRule(_ctx);
            _ctx.enterRule(listener);
        }
    }

    /**
	 * Notify any parse listeners of an exit rule event.
	 *
	 * @see #addParseListener
	 */
    protected void triggerExitRuleEvent()
    {
        // reverse order walk of listeners
        for (int i = _parseListeners.Count - 1; i >= 0; i--)
        {
            ParseTreeListener listener = _parseListeners[i];
            _ctx.exitRule(listener);
            listener.exitEveryRule(_ctx);
        }
    }

    /**
	 * Gets the number of syntax errors reported during parsing. This value is
	 * incremented each time {@link #notifyErrorListeners} is called.
	 *
	 * @see #notifyErrorListeners
	 */
    public int getNumberOfSyntaxErrors()
    {
        return _syntaxErrors;
    }

    //@Override
    public override TokenFactory getTokenFactory()
    {
        return input.getTokenSource().getTokenFactory();
    }

    /** Tell our token source and error strategy about a new way to create tokens. */
    //@Override
    public override void setTokenFactory(TokenFactory factory)
    {
        input.getTokenSource().setTokenFactory(factory);
    }

    /**
	 * The ATN with bypass alternatives is expensive to create so we create it
	 * lazily.
	 *
	 * @throws UnsupportedOperationException if the current parser does not
	 * implement the {@link #getSerializedATN()} method.
	 */

    public ATN getATNWithBypassAlts()
    {
        String serializedAtn = getSerializedATN();
        if (serializedAtn == null)
        {
            throw new UnsupportedOperationException("The current parser does not support an ATN with bypass alternatives.");
        }

        lock (this)
        {
            if (bypassAltsAtnCache != null)
            {
                return bypassAltsAtnCache;
            }
            ATNDeserializationOptions deserializationOptions = new ATNDeserializationOptions();
            deserializationOptions.setGenerateRuleBypassTransitions(true);
            bypassAltsAtnCache = new ATNDeserializer(deserializationOptions).deserialize(serializedAtn.ToCharArray());
            return bypassAltsAtnCache;
        }
    }

    /**
	 * The preferred method of getting a tree pattern. For example, here's a
	 * sample use:
	 *
	 * <pre>
	 * ParseTree t = parser.expr();
	 * ParseTreePattern p = parser.compileParseTreePattern("&lt;ID&gt;+0", MyParser.RULE_expr);
	 * ParseTreeMatch m = p.match(t);
	 * String id = m.get("ID");
	 * </pre>
	 */
    public ParseTreePattern compileParseTreePattern(String pattern, int patternRuleIndex)
    {
        if (getTokenStream() != null)
        {
            TokenSource tokenSource = getTokenStream().getTokenSource();
            if (tokenSource is Lexer)
            {
                Lexer lexer = (Lexer)tokenSource;
                return compileParseTreePattern(pattern, patternRuleIndex, lexer);
            }
        }
        throw new UnsupportedOperationException("Parser can't discover a lexer to use");
    }

    /**
	 * The same as {@link #compileParseTreePattern(String, int)} but specify a
	 * {@link Lexer} rather than trying to deduce it from this parser.
	 */
    public ParseTreePattern compileParseTreePattern(String pattern, int patternRuleIndex,
                                                    Lexer lexer)
    {
        ParseTreePatternMatcher m = new ParseTreePatternMatcher(lexer, this);
        return m.compile(pattern, patternRuleIndex);
    }


    public ANTLRErrorStrategy getErrorHandler()
    {
        return _errHandler;
    }

    public void setErrorHandler(ANTLRErrorStrategy handler)
    {
        this._errHandler = handler;
    }

    //@Override
    public override TokenStream getInputStream() { return getTokenStream(); }

    //@Override
    public override void setInputStream(IntStream input)
    {
        setTokenStream((TokenStream)input);
    }

    public TokenStream getTokenStream()
    {
        return input;
    }

    /** Set the token stream and reset the parser. */
    public void setTokenStream(TokenStream input)
    {
        this.input = null;
        reset();
        this.input = input;
    }

    /** Match needs to return the current input symbol, which gets put
     *  into the label for the associated token ref; e.g., x=ID.
     */

    public Token getCurrentToken()
    {
        return input.LT(1);
    }

    public void notifyErrorListeners(String msg)
    {
        notifyErrorListeners(getCurrentToken(), msg, null);
    }

    public void notifyErrorListeners(Token offendingToken, String msg,
                                     RecognitionException e)
    {
        _syntaxErrors++;
        int line = -1;
        int charPositionInLine = -1;
        line = offendingToken.getLine();
        charPositionInLine = offendingToken.getCharPositionInLine();

        ANTLRErrorListener listener = getErrorListenerDispatch();
        listener.syntaxError(this, offendingToken, line, charPositionInLine, msg, e);
    }

    /**
	 * Consume and return the {@linkplain #getCurrentToken current symbol}.
	 *
	 * <p>E.g., given the following input with {@code A} being the current
	 * lookahead symbol, this function moves the cursor to {@code B} and returns
	 * {@code A}.</p>
	 *
	 * <pre>
	 *  A B
	 *  ^
	 * </pre>
	 *
	 * If the parser is not in error recovery mode, the consumed symbol is added
	 * to the parse tree using {@link ParserRuleContext#addChild(TerminalNode)}, and
	 * {@link ParseTreeListener#visitTerminal} is called on any parse listeners.
	 * If the parser <em>is</em> in error recovery mode, the consumed symbol is
	 * added to the parse tree using {@link #createErrorNode(ParserRuleContext, Token)} then
     * {@link ParserRuleContext#addErrorNode(ErrorNode)} and
	 * {@link ParseTreeListener#visitErrorNode} is called on any parse
	 * listeners.
	 */
    public Token consume()
    {
        Token o = getCurrentToken();
        if (o.getType() != EOF)
        {
            getInputStream().consume();
        }
        bool hasListener = _parseListeners != null && _parseListeners.Count > 0;
        if (_buildParseTrees || hasListener)
        {
            if (_errHandler.inErrorRecoveryMode(this))
            {
                ErrorNode node = _ctx.addErrorNode(createErrorNode(_ctx, o));
                if (_parseListeners != null)
                {
                    foreach (ParseTreeListener listener in _parseListeners)
                    {
                        listener.visitErrorNode(node);
                    }
                }
            }
            else
            {
                TerminalNode node = _ctx.addChild(createTerminalNode(_ctx, o));
                if (_parseListeners != null)
                {
                    foreach (ParseTreeListener listener in _parseListeners)
                    {
                        listener.visitTerminal(node);
                    }
                }
            }
        }
        return o;
    }

    /** How to create a token leaf node associated with a parent.
	 *  Typically, the terminal node to create is not a function of the parent.
	 *
	 * @since 4.7
	 */
    public TerminalNode createTerminalNode(ParserRuleContext parent, Token t)
    {
        return new TerminalNodeImpl(t);
    }

    /** How to create an error node, given a token, associated with a parent.
	 *  Typically, the error node to create is not a function of the parent.
	 *
	 * @since 4.7
	 */
    public ErrorNode createErrorNode(ParserRuleContext parent, Token t)
    {
        return new ErrorNodeImpl(t);
    }

    protected void addContextToParseTree()
    {
        ParserRuleContext parent = (ParserRuleContext)_ctx.parent;
        // add current context to parent if we have a parent
        if (parent != null)
        {
            parent.addChild(_ctx);
        }
    }

    /**
	 * Always called by generated parsers upon entry to a rule. Access field
	 * {@link #_ctx} get the current context.
	 */
    public void enterRule(ParserRuleContext localctx, int state, int ruleIndex)
    {
        setState(state);
        _ctx = localctx;
        _ctx.start = input.LT(1);
        if (_buildParseTrees) addContextToParseTree();
        if (_parseListeners != null) triggerEnterRuleEvent();
    }

    public void exitRule()
    {
        if (matchedEOF)
        {
            // if we have matched EOF, it cannot consume past EOF so we use LT(1) here
            _ctx.stop = input.LT(1); // LT(1) will be end of file
        }
        else
        {
            _ctx.stop = input.LT(-1); // stop node is what we just matched
        }
        // trigger event on _ctx, before it reverts to parent
        if (_parseListeners != null) triggerExitRuleEvent();
        setState(_ctx.invokingState);
        _ctx = (ParserRuleContext)_ctx.parent;
    }

    public void enterOuterAlt(ParserRuleContext localctx, int altNum)
    {
        localctx.setAltNumber(altNum);
        // if we have new localctx, make sure we replace existing ctx
        // that is previous child of parse tree
        if (_buildParseTrees && _ctx != localctx)
        {
            ParserRuleContext parent = (ParserRuleContext)_ctx.parent;
            if (parent != null)
            {
                parent.removeLastChild();
                parent.addChild(localctx);
            }
        }
        _ctx = localctx;
    }

    /**
	 * Get the precedence level for the top-most precedence rule.
	 *
	 * @return The precedence level for the top-most precedence rule, or -1 if
	 * the parser context is not nested within a precedence rule.
	 */
    public int getPrecedence()
    {
        if (_precedenceStack.isEmpty())
        {
            return -1;
        }

        return _precedenceStack.peek();
    }

    /**
	 * @deprecated Use
	 * {@link #enterRecursionRule(ParserRuleContext, int, int, int)} instead.
	 */
    //@Deprecated
    public void enterRecursionRule(ParserRuleContext localctx, int ruleIndex)
    {
        enterRecursionRule(localctx, getATN().ruleToStartState[ruleIndex].stateNumber, ruleIndex, 0);
    }

    public void enterRecursionRule(ParserRuleContext localctx, int state, int ruleIndex, int precedence)
    {
        setState(state);
        _precedenceStack.push(precedence);
        _ctx = localctx;
        _ctx.start = input.LT(1);
        if (_parseListeners != null)
        {
            triggerEnterRuleEvent(); // simulates rule entry for left-recursive rules
        }
    }

    /** Like {@link #enterRule} but for recursive rules.
	 *  Make the current context the child of the incoming localctx.
	 */
    public void pushNewRecursionContext(ParserRuleContext localctx, int state, int ruleIndex)
    {
        ParserRuleContext previous = _ctx;
        previous.parent = localctx;
        previous.invokingState = state;
        previous.stop = input.LT(-1);

        _ctx = localctx;
        _ctx.start = previous.start;
        if (_buildParseTrees)
        {
            _ctx.addChild(previous);
        }

        if (_parseListeners != null)
        {
            triggerEnterRuleEvent(); // simulates rule entry for left-recursive rules
        }
    }

    public void unrollRecursionContexts(ParserRuleContext _parentctx)
    {
        _precedenceStack.pop();
        _ctx.stop = input.LT(-1);
        ParserRuleContext retctx = _ctx; // save current ctx (return value)

        // unroll so _ctx is as it was before call to recursive method
        if (_parseListeners != null)
        {
            while (_ctx != _parentctx)
            {
                triggerExitRuleEvent();
                _ctx = (ParserRuleContext)_ctx.parent;
            }
        }
        else
        {
            _ctx = _parentctx;
        }

        // hook into tree
        retctx.parent = _parentctx;

        if (_buildParseTrees && _parentctx != null)
        {
            // add return ctx into invoking rule's tree
            _parentctx.addChild(retctx);
        }
    }

    public ParserRuleContext getInvokingContext(int ruleIndex)
    {
        ParserRuleContext p = _ctx;
        while (p != null)
        {
            if (p.getRuleIndex() == ruleIndex) return p;
            p = (ParserRuleContext)p.parent;
        }
        return null;
    }

    public ParserRuleContext getContext()
    {
        return _ctx;
    }

    public void setContext(ParserRuleContext ctx)
    {
        _ctx = ctx;
    }

    //@Override
    public bool precpred(RuleContext localctx, int precedence)
    {
        return precedence >= _precedenceStack.peek();
    }

    public bool inContext(String context)
    {
        // TODO: useful in parser?
        return false;
    }

    /**
	 * Checks whether or not {@code symbol} can follow the current state in the
	 * ATN. The behavior of this method is equivalent to the following, but is
	 * implemented such that the complete context-sensitive follow set does not
	 * need to be explicitly constructed.
	 *
	 * <pre>
	 * return getExpectedTokens().contains(symbol);
	 * </pre>
	 *
	 * @param symbol the symbol type to check
	 * @return {@code true} if {@code symbol} can follow the current state in
	 * the ATN, otherwise {@code false}.
	 */
    public bool isExpectedToken(int symbol)
    {
        //   		return getInterpreter().atn.nextTokens(_ctx);
        ATN atn = getInterpreter().atn;
        ParserRuleContext ctx = _ctx;
        ATNState s = atn.states[getState()];
        IntervalSet following = atn.nextTokens(s);
        if (following.contains(symbol))
        {
            return true;
        }
        //        Console.Out.println("following "+s+"="+following);
        if (!following.contains(Token.EPSILON)) return false;

        while (ctx != null && ctx.invokingState >= 0 && following.contains(Token.EPSILON))
        {
            ATNState invokingState = atn.states[ctx.invokingState];
            RuleTransition rt = (RuleTransition)invokingState.transition(0);
            following = atn.nextTokens(rt.followState);
            if (following.contains(symbol))
            {
                return true;
            }

            ctx = (ParserRuleContext)ctx.parent;
        }

        if (following.contains(Token.EPSILON) && symbol == Token.EOF)
        {
            return true;
        }

        return false;
    }

    public bool isMatchedEOF()
    {
        return matchedEOF;
    }

    /**
	 * Computes the set of input symbols which could follow the current parser
	 * state and context, as given by {@link #getState} and {@link #getContext},
	 * respectively.
	 *
	 * @see ATN#getExpectedTokens(int, RuleContext)
	 */
    public IntervalSet getExpectedTokens()
    {
        return getATN().getExpectedTokens(getState(), getContext());
    }


    public IntervalSet getExpectedTokensWithinCurrentRule()
    {
        ATN atn = getInterpreter().atn;
        ATNState s = atn.states[(getState())];
        return atn.nextTokens(s);
    }

    /** Get a rule's index (i.e., {@code RULE_ruleName} field) or -1 if not found. */
    public int getRuleIndex(String ruleName)
    {
        if (getRuleIndexMap().TryGetValue(ruleName, out var ruleIndex)) return ruleIndex;
        return -1;
    }

    public ParserRuleContext getRuleContext() { return _ctx; }

    /** Return List&lt;String&gt; of the rule names in your parser instance
	 *  leading up to a call to the current rule.  You could override if
	 *  you want more details such as the file/line info of where
	 *  in the ATN a rule is invoked.
	 *
	 *  This is very useful for error messages.
	 */
    public List<String> getRuleInvocationStack()
    {
        return getRuleInvocationStack(_ctx);
    }

    public List<String> getRuleInvocationStack(RuleContext p)
    {
        String[] ruleNames = getRuleNames();
        List<String> stack = new();
        while (p != null)
        {
            // compute what follows who invoked us
            int ruleIndex = p.getRuleIndex();
            if (ruleIndex < 0) stack.Add("n/a");
            else stack.Add(ruleNames[ruleIndex]);
            p = p.parent;
        }
        return stack;
    }

    /** For debugging and other purposes. */
    public List<String> getDFAStrings()
    {
        lock (_interp.decisionToDFA)
        {
            List<String> s = new();
            for (int d = 0; d < _interp.decisionToDFA.Length; d++)
            {
                DFA dfa = _interp.decisionToDFA[d];
                s.Add(dfa.toString(getVocabulary()));
            }
            return s;
        }
    }

    public void dumpDFA()
    {
        dumpDFA(Console.Out);
    }

    /** For debugging and other purposes. */
    public void dumpDFA(TextWriter dumpStream)
    {
        lock (_interp.decisionToDFA)
        {
            bool seenOne = false;
            for (int d = 0; d < _interp.decisionToDFA.Length; d++)
            {
                DFA dfa = _interp.decisionToDFA[d];
                if (dfa.states.Count > 0)
                {
                    if (seenOne) dumpStream.WriteLine();
                    dumpStream.WriteLine("Decision " + dfa.decision + ":");
                    dumpStream.Write(dfa.toString(getVocabulary()));
                    seenOne = true;
                }
            }
        }
    }

    public String getSourceName()
    {
        return input.getSourceName();
    }

    //@Override
    public ParseInfo getParseInfo()
    {
        ParserATNSimulator interp = getInterpreter();
        if (interp is ProfilingATNSimulator)
        {
            return new ParseInfo((ProfilingATNSimulator)interp);
        }
        return null;
    }

    /**
	 * @since 4.3
	 */
    public void setProfile(bool profile)
    {
        ParserATNSimulator interp = getInterpreter();
        PredictionMode saveMode = interp.getPredictionMode();
        if (profile)
        {
            if (!(interp is ProfilingATNSimulator))
            {
                setInterpreter(new ProfilingATNSimulator(this));
            }
        }
        else if (interp is ProfilingATNSimulator)
        {
            ParserATNSimulator sim =
                new ParserATNSimulator(this, getATN(), interp.decisionToDFA, interp.getSharedContextCache());
            setInterpreter(sim);
        }
        getInterpreter().setPredictionMode(saveMode);
    }

    /** During a parse is sometimes useful to listen in on the rule entry and exit
	 *  events as well as token matches. This is for quick and dirty debugging.
	 */
    public void setTrace(bool trace)
    {
        if (!trace)
        {
            removeParseListener(_tracer);
            _tracer = null;
        }
        else
        {
            if (_tracer != null) removeParseListener(_tracer);
            else _tracer = new TraceListener(this);
            addParseListener(_tracer);
        }
    }

    /**
	 * Gets whether a {@link TraceListener} is registered as a parse listener
	 * for the parser.
	 *
	 * @see #setTrace(bool)
	 */
    public bool isTrace()
    {
        return _tracer != null;
    }
}

