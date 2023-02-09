/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime.atn;
using org.antlr.v4.runtime.dfa;
using org.antlr.v4.runtime.misc;

namespace org.antlr.v4.runtime;

public interface Recognizer
{
    string[] TokenNames { get; }

    string[] RuleNames { get; }

    bool Precpred(RuleContext localctx, int precedence);
    bool Sempred(RuleContext _localctx, int ruleIndex, int actionIndex);
    int State { get; }
    ATN ATN { get; }
}
public abstract class Recognizer<Symbol, ATNInterpreter> : Recognizer where ATNInterpreter : ATNSimulator
{
    public static readonly int EOF = -1;

    private static readonly Dictionary<Vocabulary, Dictionary<string, int>> tokenTypeMapCache =
        new();
    private static readonly Dictionary<string[], Dictionary<string, int>> ruleIndexMapCache =
        new();


    private List<ANTLRErrorListener> _listeners = new() { ConsoleErrorListener.INSTANCE };
    //new CopyOnWriteArrayList<ANTLRErrorListener>() {{
    //	add(ConsoleErrorListener.INSTANCE);
    //}};

    protected ATNInterpreter _interp;

    private int _stateNumber = -1;

    /** Used to print out token names like ID during debugging and
	 *  error reporting.  The generated parsers implement a method
	 *  that overrides this to point to their String[] tokenNames.
	 *
	 * @deprecated Use {@link #getVocabulary()} instead.
	 */
    //@Deprecated
    public abstract string[] TokenNames { get; }

    public abstract string[] RuleNames { get; }

    /**
	 * Get the vocabulary used by the recognizer.
	 *
	 * @return A {@link Vocabulary} instance providing information about the
	 * vocabulary used by the grammar.
	 */
    //@SuppressWarnings("deprecation")
    public virtual Vocabulary Vocabulary => VocabularyImpl.FromTokenNames(TokenNames);

    /**
	 * Get a map from token names to token types.
	 *
	 * <p>Used for XPath and tree pattern compilation.</p>
	 */
    public Dictionary<string, int> GetTokenTypeMap()
    {
        var vocabulary = Vocabulary;
        lock (tokenTypeMapCache)
        {
            if (!tokenTypeMapCache.TryGetValue(vocabulary, out var result))
            {
                result = new();
                for (int i = 0; i <= ATN.maxTokenType; i++)
                {
                    var literalName = vocabulary.GetLiteralName(i);
                    if (literalName != null)
                    {
                        result[literalName] = i;
                    }

                    var symbolicName = vocabulary.GetSymbolicName(i);
                    if (symbolicName != null)
                    {
                        result[symbolicName] = i;
                    }
                }

                result["EOF"] = Token.EOF;
                result = new(result);
                tokenTypeMapCache[vocabulary] = result;
            }

            return result;
        }
    }

    /**
	 * Get a map from rule names to rule indexes.
	 *
	 * <p>Used for XPath and tree pattern compilation.</p>
	 */
    public Dictionary<string, int> GetRuleIndexMap()
    {
        var ruleNames = RuleNames;
        if (ruleNames == null)
        {
            throw new UnsupportedOperationException("The current recognizer does not provide a list of rule names.");
        }

        lock (ruleIndexMapCache)
        {
            if (!ruleIndexMapCache.TryGetValue(ruleNames, out var result))
            {
                result = RuntimeUtils.ToMap(ruleNames);

                ruleIndexMapCache.Add(ruleNames, result);
            }

            return result;
        }
    }

    public virtual int GetTokenType(string tokenName)
    {
        if (GetTokenTypeMap().TryGetValue(tokenName, out var ttype)) return ttype;
        return Token.INVALID_TYPE;
    }

    /**
	 * If this recognizer was generated, it will have a serialized ATN
	 * representation of the grammar.
	 *
	 * <p>For interpreters, we don't know their serialized ATN despite having
	 * created the interpreter from it.</p>
	 */
    public string GetSerializedATN()
    {
        throw new UnsupportedOperationException("there is no serialized ATN");
    }

    /** For debugging and other purposes, might want the grammar name.
	 *  Have ANTLR generate an implementation for this method.
	 */
    public abstract string GrammarFileName { get; }

    /**
	 * Get the {@link ATN} used by the recognizer for prediction.
	 *
	 * @return The {@link ATN} used by the recognizer for prediction.
	 */
    public abstract ATN ATN { get; }

    /**
	 * Get the ATN interpreter used by the recognizer for prediction.
	 *
	 * @return The ATN interpreter used by the recognizer for prediction.
	 */
    public ATNInterpreter GetInterpreter() => _interp;

    /** If profiling during the parse/lex, this will return DecisionInfo records
	 *  for each decision in recognizer in a ParseInfo object.
	 *
	 * @since 4.3
	 */
    public ParseInfo GetParseInfo() => null;

    /**
	 * Set the ATN interpreter used by the recognizer for prediction.
	 *
	 * @param interpreter The ATN interpreter used by the recognizer for
	 * prediction.
	 */
    public void SetInterpreter(ATNInterpreter interpreter) => _interp = interpreter;

    /** What is the error header, normally line/character position information? */
    public string GetErrorHeader(RecognitionException e)
    {
        int line = e.OffendingToken.Line;
        int charPositionInLine = e.OffendingToken.CharPositionInLine;
        return "line " + line + ":" + charPositionInLine;
    }

    /** How should a token be displayed in an error message? The default
	 *  is to display just the text, but during development you might
	 *  want to have a lot of information spit out.  Override in that case
	 *  to use t.toString() (which, for CommonToken, dumps everything about
	 *  the token). This is better than forcing you to override a method in
	 *  your token objects because you don't have to go modify your lexer
	 *  so that it creates a new Java type.
	 *
	 * @deprecated This method is not called by the ANTLR 4 Runtime. Specific
	 * implementations of {@link ANTLRErrorStrategy} may provide a similar
	 * feature when necessary. For example, see
	 * {@link DefaultErrorStrategy#getTokenErrorDisplay}.
	 */
    //@Deprecated
    public string GetTokenErrorDisplay(Token t)
    {
        if (t == null) return "<no token>";
        var s = t.Text;
        s ??= t.Type == Token.EOF ? "<EOF>" : "<" + t.Type + ">";
        s = s.Replace("\n", "\\n");
        s = s.Replace("\r", "\\r");
        s = s.Replace("\t", "\\t");
        return "'" + s + "'";
    }

    /**
	 * @exception NullReferenceException if {@code listener} is {@code null}.
	 */
    public void AddErrorListener(ANTLRErrorListener listener)
    {
        if (listener == null)
        {
            throw new NullReferenceException("listener cannot be null.");
        }

        _listeners.Add(listener);
    }

    public void RemoveErrorListener(ANTLRErrorListener listener)
    {
        _listeners.Remove(listener);
    }

    public void RemoveErrorListeners()
    {
        _listeners.Clear();
    }


    public List<ANTLRErrorListener> GetErrorListeners()
    {
        return _listeners;
    }

    public ANTLRErrorListener GetErrorListenerDispatch() => new ProxyErrorListener(GetErrorListeners());

    // subclass needs to override these if there are sempreds or actions
    // that the ATN interp needs to execute
    public bool Sempred(RuleContext _localctx, int ruleIndex, int actionIndex) => true;

    public bool Precpred(RuleContext localctx, int precedence) => true;

    public void Action(RuleContext _localctx, int ruleIndex, int actionIndex) { }

    /** Indicate that the recognizer has changed internal state that is
 *  consistent with the ATN state passed in.  This way we always know
 *  where we are in the ATN as the parser goes along. The rule
 *  context objects form a stack that lets us see the stack of
 *  invoking rules. Combine this and we have complete ATN
 *  configuration information.
 */
    public int State { get => _stateNumber; set =>
                                                    //		Console.Error.WriteLine("setState "+atnState);
                                                    _stateNumber = value;
        //		if ( traceATNStates ) _ctx.trace(atnState);
                                                   }
    public abstract IntStream InputStream { get; set; }

    public abstract TokenFactory TokenFactory { get; set; }
}
