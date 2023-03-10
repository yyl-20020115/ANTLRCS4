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
        
        public void EnterEveryRule(ParserRuleContext ctx)
        {
            Console.Out.WriteLine("enter   " + this.parser.RuleNames[ctx.RuleIndex] +
                               ", LT(1)=" + this.parser.input.LT(1).Text);
        }

        
        public void VisitTerminal(TerminalNode node)
        {
            Console.Out.WriteLine("consume " + node.GetSymbol() + " rule " +
                               this.parser.                               RuleNames[this.parser._ctx.RuleIndex]);
        }

        
        public void VisitErrorNode(ErrorNode node)
        {
        }

        
        public void ExitEveryRule(ParserRuleContext ctx)
        {
            Console.Out.WriteLine("exit    " + this.parser.RuleNames[ctx.RuleIndex] +
                               ", LT(1)=" + this.parser.input.LT(1).Text);
        }
    }

    public class TrimToSizeListener : ParseTreeListener
    {
        public static readonly TrimToSizeListener INSTANCE = new ();

        
        public void EnterEveryRule(ParserRuleContext ctx) { }

        
        public void VisitTerminal(TerminalNode node) { }

        
        public void VisitErrorNode(ErrorNode node) { }

        
        public void ExitEveryRule(ParserRuleContext ctx)
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

    protected readonly IntegerStack _precedenceStack = new ();
    //{
    //	_precedenceStack = new IntegerStack();
    //	_precedenceStack.push(0);
    //}

    /**
	 * The {@link ParserRuleContext} object for the currently executing rule.
	 * This is always non-null during the parsing process.
	 */
    protected ParserRuleContext _ctx;
    public ParserRuleContext Ctx => _ctx;     /**
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
        _precedenceStack.Push(0);

        InputStream = input;
    }
    protected readonly RecognizerSharedState state;
    public Parser(TokenStream input, RecognizerSharedState state)
        : this(input)
    {
        this.state = state;
    }
    /** reset the parser's state */
    public virtual void Reset()
    {
        InputStream?.Seek(0);
        _errHandler.Reset(this);
        _ctx = null;
        _syntaxErrors = 0;
        matchedEOF = false;
        Trace = false;
        _precedenceStack.Clear();
        _precedenceStack.Push(0);
        var interpreter = GetInterpreter();
        if (interpreter != null)
        {
            interpreter.Reset();
        }
    }
    public Token Match(int ttype)
    {
        var t = GetCurrentToken();
        if (t.Type == ttype)
        {
            if (ttype == Token.EOF)
            {
                matchedEOF = true;
            }
            _errHandler.ReportMatch(this);
            Consume();
        }
        else
        {
            t = _errHandler.RecoverInline(this);
            if (_buildParseTrees && t.TokenIndex == -1)
            {
                // we must have conjured up a new token during single token insertion
                // if it's not the current symbol
                _ctx.AddErrorNode(CreateErrorNode(_ctx, t));
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
    public Token Match(TokenStream input, int ttype, BitSet fOLLOW_ACTION_in_optionValue890)
    {
        var t = GetCurrentToken();
        if (t.Type == ttype)
        {
            if (ttype == Token.EOF)
            {
                matchedEOF = true;
            }
            _errHandler.ReportMatch(this);
            Consume();
        }
        else
        {
            t = _errHandler.RecoverInline(this);
            if (_buildParseTrees && t.TokenIndex == -1)
            {
                // we must have conjured up a new token during single token insertion
                // if it's not the current symbol
                _ctx.AddErrorNode(CreateErrorNode(_ctx, t));
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
    public Token MatchWildcard()
    {
        var t = GetCurrentToken();
        if (t.Type > 0)
        {
            _errHandler.ReportMatch(this);
            Consume();
        }
        else
        {
            t = _errHandler.RecoverInline(this);
            if (_buildParseTrees && t.TokenIndex == -1)
            {
                // we must have conjured up a new token during single token insertion
                // if it's not the current symbol
                _ctx.AddErrorNode(CreateErrorNode(_ctx, t));
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
    public void SetBuildParseTree(bool buildParseTrees)
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
    public bool GetBuildParseTree() => _buildParseTrees;

    /**
	 * Trim the internal lists of the parse tree during parsing to conserve memory.
	 * This property is set to {@code false} by default for a newly constructed parser.
	 *
	 * @param trimParseTrees {@code true} to trim the capacity of the {@link ParserRuleContext#children}
	 * list to its size after a rule is parsed.
	 */
    public void SetTrimParseTree(bool trimParseTrees)
    {
        if (trimParseTrees)
        {
            if (GetTrimParseTree()) return;
            AddParseListener(TrimToSizeListener.INSTANCE);
        }
        else
        {
            RemoveParseListener(TrimToSizeListener.INSTANCE);
        }
    }

    /**
	 * @return {@code true} if the {@link ParserRuleContext#children} list is trimmed
	 * using the default {@link Parser.TrimToSizeListener} during the parse process.
	 */
    public bool GetTrimParseTree() => GetParseListeners().Contains(TrimToSizeListener.INSTANCE);


    public List<ParseTreeListener> GetParseListeners()
    {
        var listeners = _parseListeners;
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
    public void AddParseListener(ParseTreeListener listener)
    {
        if (listener == null)
        {
            throw new NullReferenceException(nameof(listener));
        }

        _parseListeners ??= new();

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
    public void RemoveParseListener(ParseTreeListener listener)
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
    public void RemoveParseListeners()
    {
        _parseListeners = null;
    }

    /**
	 * Notify any parse listeners of an enter rule event.
	 *
	 * @see #addParseListener
	 */
    protected void TriggerEnterRuleEvent()
    {
        foreach (var listener in _parseListeners)
        {
            listener.EnterEveryRule(_ctx);
            _ctx.EnterRule(listener);
        }
    }

    /**
	 * Notify any parse listeners of an exit rule event.
	 *
	 * @see #addParseListener
	 */
    protected void TriggerExitRuleEvent()
    {
        // reverse order walk of listeners
        for (int i = _parseListeners.Count - 1; i >= 0; i--)
        {
            var listener = _parseListeners[i];
            _ctx.ExitRule(listener);
            listener.ExitEveryRule(_ctx);
        }
    }

    /**
	 * Gets the number of syntax errors reported during parsing. This value is
	 * incremented each time {@link #notifyErrorListeners} is called.
	 *
	 * @see #notifyErrorListeners
	 */
    public int NumberOfSyntaxErrors => _syntaxErrors;

    
    /** Tell our token source and error strategy about a new way to create tokens. */
    
    public override TokenFactory TokenFactory { get => input.TokenSource.TokenFactory; set => input.TokenSource.TokenFactory = value; }

    /**
	 * The ATN with bypass alternatives is expensive to create so we create it
	 * lazily.
	 *
	 * @throws UnsupportedOperationException if the current parser does not
	 * implement the {@link #getSerializedATN()} method.
	 */

    public ATN GetATNWithBypassAlts()
    {
        var serializedAtn = GetSerializedATN();
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
            var deserializationOptions = new ATNDeserializationOptions();
            deserializationOptions.            GenerateRuleBypassTransitions = true;
            bypassAltsAtnCache = new ATNDeserializer(deserializationOptions).Deserialize(serializedAtn.ToCharArray());
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
	 * string id = m.get("ID");
	 * </pre>
	 */
    public ParseTreePattern CompileParseTreePattern(string pattern, int patternRuleIndex)
    {
        if (TokenStream != null)
        {
            var tokenSource = TokenStream.TokenSource;
            if (tokenSource is Lexer lexer)
            {
                return CompileParseTreePattern(pattern, patternRuleIndex, lexer);
            }
        }
        throw new UnsupportedOperationException("Parser can't discover a lexer to use");
    }

    /**
	 * The same as {@link #compileParseTreePattern(string, int)} but specify a
	 * {@link Lexer} rather than trying to deduce it from this parser.
	 */
    public ParseTreePattern CompileParseTreePattern(string pattern, int patternRuleIndex,
                                                    Lexer lexer)
    {
        var m = new ParseTreePatternMatcher(lexer, this);
        return m.Compile(pattern, patternRuleIndex);
    }


    public virtual ANTLRErrorStrategy ErrorHandler { get => _errHandler; set => this._errHandler = value; }

    public override IntStream InputStream { get => TokenStream; set => TokenStream = value as TokenStream; }

    /** Set the token stream and reset the parser. */
    public TokenStream TokenStream
    {
        get => input;
        set
        {
            this.input = null;
            Reset();
            this.input = value;
        }
    }

    /** Match needs to return the current input symbol, which gets put
     *  into the label for the associated token ref; e.g., x=ID.
     */

    public Token GetCurrentToken()
    {
        return input.LT(1);
    }

    public void NotifyErrorListeners(string msg)
    {
        NotifyErrorListeners(GetCurrentToken(), msg, null);
    }

    public void NotifyErrorListeners(Token offendingToken, string msg,
                                     RecognitionException e)
    {
        _syntaxErrors++;
        int line = -1;
        int charPositionInLine = -1;
        line = offendingToken.Line;
        charPositionInLine = offendingToken.CharPositionInLine;

        var listener = GetErrorListenerDispatch();
        listener.SyntaxError(this, offendingToken, line, charPositionInLine, msg, e);
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
    public Token Consume()
    {
        var o = GetCurrentToken();
        if (o.Type != EOF)
        {
            InputStream.Consume();
        }
        bool hasListener = _parseListeners != null && _parseListeners.Count > 0;
        if (_buildParseTrees || hasListener)
        {
            if (_errHandler.InErrorRecoveryMode(this))
            {
                var node = _ctx.AddErrorNode(CreateErrorNode(_ctx, o));
                if (_parseListeners != null)
                {
                    foreach (var listener in _parseListeners)
                    {
                        listener.VisitErrorNode(node);
                    }
                }
            }
            else
            {
                var node = _ctx.AddChild(CreateTerminalNode(_ctx, o));
                if (_parseListeners != null)
                {
                    foreach (var listener in _parseListeners)
                    {
                        listener.VisitTerminal(node);
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
    public TerminalNode CreateTerminalNode(ParserRuleContext parent, Token t)
    {
        return new TerminalNodeImpl(t);
    }

    /** How to create an error node, given a token, associated with a parent.
	 *  Typically, the error node to create is not a function of the parent.
	 *
	 * @since 4.7
	 */
    public static ErrorNode CreateErrorNode(ParserRuleContext parent, Token t)
    {
        return new ErrorNodeImpl(t);
    }

    protected void AddContextToParseTree()
    {
        var parent = (ParserRuleContext)_ctx.parent;
        // add current context to parent if we have a parent
        parent?.AddChild(_ctx);
    }

    /**
	 * Always called by generated parsers upon entry to a rule. Access field
	 * {@link #_ctx} get the current context.
	 */
    public void EnterRule(ParserRuleContext localctx, int state, int ruleIndex)
    {
        State = state;
        _ctx = localctx;
        _ctx.start = input.LT(1);
        if (_buildParseTrees) AddContextToParseTree();
        if (_parseListeners != null) TriggerEnterRuleEvent();
    }

    public void ExitRule()
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
        if (_parseListeners != null) TriggerExitRuleEvent();
        State = _ctx.invokingState;
        _ctx = (ParserRuleContext)_ctx.parent;
    }

    public void EnterOuterAlt(ParserRuleContext localctx, int altNum)
    {
        localctx.        AltNumber = altNum;
        // if we have new localctx, make sure we replace existing ctx
        // that is previous child of parse tree
        if (_buildParseTrees && _ctx != localctx)
        {
            var parent = (ParserRuleContext)_ctx.parent;
            if (parent != null)
            {
                parent.RemoveLastChild();
                parent.AddChild(localctx);
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
    public int GetPrecedence() => _precedenceStack.IsEmpty ? -1 : _precedenceStack.Peek();

    /**
	 * @deprecated Use
	 * {@link #enterRecursionRule(ParserRuleContext, int, int, int)} instead.
	 */
    //@Deprecated
    public virtual void EnterRecursionRule(ParserRuleContext localctx, int ruleIndex)
    {
        EnterRecursionRule(localctx, ATN.ruleToStartState[ruleIndex].stateNumber, ruleIndex, 0);
    }

    public virtual void EnterRecursionRule(ParserRuleContext localctx, int state, int ruleIndex, int precedence)
    {
        State = state;
        _precedenceStack.Push(precedence);
        _ctx = localctx;
        _ctx.start = input.LT(1);
        if (_parseListeners != null)
        {
            TriggerEnterRuleEvent(); // simulates rule entry for left-recursive rules
        }
    }

    /** Like {@link #enterRule} but for recursive rules.
	 *  Make the current context the child of the incoming localctx.
	 */
    public void PushNewRecursionContext(ParserRuleContext localctx, int state, int ruleIndex)
    {
        var previous = _ctx;
        previous.parent = localctx;
        previous.invokingState = state;
        previous.stop = input.LT(-1);

        _ctx = localctx;
        _ctx.start = previous.start;
        if (_buildParseTrees)
        {
            _ctx.AddChild(previous);
        }

        if (_parseListeners != null)
        {
            TriggerEnterRuleEvent(); // simulates rule entry for left-recursive rules
        }
    }

    public void UnrollRecursionContexts(ParserRuleContext _parentctx)
    {
        _precedenceStack.Pop();
        _ctx.stop = input.LT(-1);
        var retctx = _ctx; // save current ctx (return value)

        // unroll so _ctx is as it was before call to recursive method
        if (_parseListeners != null)
        {
            while (_ctx != _parentctx)
            {
                TriggerExitRuleEvent();
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
            _parentctx.AddChild(retctx);
        }
    }

    public ParserRuleContext GetInvokingContext(int ruleIndex)
    {
        var p = _ctx;
        while (p != null)
        {
            if (p.RuleIndex == ruleIndex) return p;
            p = (ParserRuleContext)p.parent;
        }
        return null;
    }

    public ParserRuleContext Context { get => _ctx; set => _ctx = value; }

    
    public bool Precpred(RuleContext localctx, int precedence)
    {
        return precedence >= _precedenceStack.Peek();
    }

    public bool InContext(string context)
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
    public bool IsExpectedToken(int symbol)
    {
        //   		return getInterpreter().atn.nextTokens(_ctx);
        var atn = GetInterpreter().atn;
        var ctx = _ctx;
        var s = atn.states[State];
        var following = atn.NextTokens(s);
        if (following.Contains(symbol))
        {
            return true;
        }
        //        Console.Out.println("following "+s+"="+following);
        if (!following.Contains(Token.EPSILON)) return false;

        while (ctx != null && ctx.invokingState >= 0 && following.Contains(Token.EPSILON))
        {
            var invokingState = atn.states[ctx.invokingState];
            var rt = (RuleTransition)invokingState.Transition(0);
            following = atn.NextTokens(rt.followState);
            if (following.Contains(symbol))
            {
                return true;
            }

            ctx = (ParserRuleContext)ctx.parent;
        }

        if (following.Contains(Token.EPSILON) && symbol == Token.EOF)
        {
            return true;
        }

        return false;
    }

    public bool IsMatchedEOF => matchedEOF;

    /**
	 * Computes the set of input symbols which could follow the current parser
	 * state and context, as given by {@link #getState} and {@link #getContext},
	 * respectively.
	 *
	 * @see ATN#getExpectedTokens(int, RuleContext)
	 */
    public IntervalSet GetExpectedTokens() => ATN.GetExpectedTokens(State, Context);


    public IntervalSet GetExpectedTokensWithinCurrentRule()
    {
        var atn = GetInterpreter().atn;
        var s = atn.states[(State)];
        return atn.NextTokens(s);
    }

    /** Get a rule's index (i.e., {@code RULE_ruleName} field) or -1 if not found. */
    public int GetRuleIndex(string ruleName) => GetRuleIndexMap().TryGetValue(ruleName, out var ruleIndex) ? ruleIndex : -1;

    public ParserRuleContext RuleContext => _ctx;
    /** Return List&lt;string&gt; of the rule names in your parser instance
	 *  leading up to a call to the current rule.  You could override if
	 *  you want more details such as the file/line info of where
	 *  in the ATN a rule is invoked.
	 *
	 *  This is very useful for error messages.
	 */
    public List<string> GetRuleInvocationStack()
    {
        return getRuleInvocationStack(_ctx);
    }

    public List<string> getRuleInvocationStack(RuleContext p)
    {
        var ruleNames = RuleNames;
        List<string> stack = new();
        while (p != null)
        {
            // compute what follows who invoked us
            int ruleIndex = p.RuleIndex;
            if (ruleIndex < 0) stack.Add("n/a");
            else stack.Add(ruleNames[ruleIndex]);
            p = p.parent;
        }
        return stack;
    }

    /** For debugging and other purposes. */
    public List<string> GetDFAStrings()
    {
        lock (_interp.decisionToDFA)
        {
            List<string> s = new();
            for (int d = 0; d < _interp.decisionToDFA.Length; d++)
            {
                var dfa = _interp.decisionToDFA[d];
                s.Add(dfa.ToString(Vocabulary));
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
                var dfa = _interp.decisionToDFA[d];
                if (dfa.states.Count > 0)
                {
                    if (seenOne) dumpStream.WriteLine();
                    dumpStream.WriteLine("Decision " + dfa.decision + ":");
                    dumpStream.Write(dfa.ToString(Vocabulary));
                    seenOne = true;
                }
            }
        }
    }

    public string SourceName => input.SourceName;

    
    public  ParseInfo GetParseInfo()
    {
        ParserATNSimulator interp = GetInterpreter();
        if (interp is ProfilingATNSimulator)
        {
            return new ParseInfo((ProfilingATNSimulator)interp);
        }
        return null;
    }

    /**
	 * @since 4.3
	 */
    public void SetProfile(bool profile)
    {
        var interp = GetInterpreter();
        var saveMode = interp.PredictionMode;
        if (profile)
        {
            if (interp is not ProfilingATNSimulator)
            {
                SetInterpreter(new ProfilingATNSimulator(this));
            }
        }
        else if (interp is ProfilingATNSimulator)
        {
            var sim =
                new ParserATNSimulator(this, ATN, interp.decisionToDFA, interp.GetSharedContextCache());
            SetInterpreter(sim);
        }
        GetInterpreter().PredictionMode = saveMode;
    }

    /**
	 * Gets whether a {@link TraceListener} is registered as a parse listener
	 * for the parser.
	 *
	 * @see #setTrace(bool)
	 */
    /** During a parse is sometimes useful to listen in on the rule entry and exit
 *  events as well as token matches. This is for quick and dirty debugging.
 */
    public bool Trace
    {
        get => _tracer != null;
        set
        {
            if (!value)
            {
                RemoveParseListener(_tracer);
                _tracer = null;
            }
            else
            {
                if (_tracer != null) RemoveParseListener(_tracer);
                else _tracer = new TraceListener(this);
                AddParseListener(_tracer);
            }
        }
    }
}

