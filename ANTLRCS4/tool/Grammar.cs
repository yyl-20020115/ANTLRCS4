/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.runtime.tree;
using org.antlr.v4.analysis;
using org.antlr.v4.automata;
using org.antlr.v4.misc;
using org.antlr.v4.parse;
using org.antlr.v4.runtime;
using org.antlr.v4.runtime.atn;
using org.antlr.v4.runtime.dfa;
using org.antlr.v4.runtime.misc;
using org.antlr.v4.runtime.tree;
using org.antlr.v4.tool.ast;
using System.Text;

namespace org.antlr.v4.tool;


public class Grammar : AttributeResolver
{
    public static readonly String GRAMMAR_FROM_STRING_NAME = "<string>";
    /**
	 * This value is used in the following situations to indicate that a token
	 * type does not have an associated name which can be directly referenced in
	 * a grammar.
	 *
	 * <ul>
	 * <li>This value is the name and display name for the token with type
	 * {@link Token#INVALID_TYPE}.</li>
	 * <li>This value is the name for tokens with a type not represented by a
	 * named token. The display name for these tokens is simply the string
	 * representation of the token type as an integer.</li>
	 * </ul>
	 */
    public static readonly String INVALID_TOKEN_NAME = "<INVALID>";
    /**
	 * This value is used as the name for elements in the array returned by
	 * {@link #getRuleNames} for indexes not associated with a rule.
	 */
    public static readonly String INVALID_RULE_NAME = "<invalid>";

    public static readonly String caseInsensitiveOptionName = "caseInsensitive";

    public static readonly HashSet<String> parserOptions = new HashSet<String>();
    static Grammar()
    {
        parserOptions.Add("superClass");
        parserOptions.Add("contextSuperClass");
        parserOptions.Add("TokenLabelType");
        parserOptions.Add("tokenVocab");
        parserOptions.Add("language");
        parserOptions.Add("accessLevel");
        parserOptions.Add("exportMacro");
        parserOptions.Add(caseInsensitiveOptionName);
        lexerRuleOptions.Add(caseInsensitiveOptionName);
        ruleRefOptions.Add(LeftRecursiveRuleTransformer.PRECEDENCE_OPTION_NAME);
        ruleRefOptions.Add(LeftRecursiveRuleTransformer.TOKENINDEX_OPTION_NAME);
        tokenOptions.Add("assoc");
        tokenOptions.Add(LeftRecursiveRuleTransformer.TOKENINDEX_OPTION_NAME);
        semPredOptions.Add(LeftRecursiveRuleTransformer.PRECEDENCE_OPTION_NAME);
        semPredOptions.Add("fail");
        doNotCopyOptionsToLexer.Add("superClass");
        doNotCopyOptionsToLexer.Add("TokenLabelType");
        doNotCopyOptionsToLexer.Add("tokenVocab");

        grammarAndLabelRefTypeToScope.Add("parser:RULE_LABEL", Rule.predefinedRulePropertiesDict);
        grammarAndLabelRefTypeToScope.Add("parser:TOKEN_LABEL", AttributeDict.predefinedTokenDict);
        grammarAndLabelRefTypeToScope.Add("combined:RULE_LABEL", Rule.predefinedRulePropertiesDict);
        grammarAndLabelRefTypeToScope.Add("combined:TOKEN_LABEL", AttributeDict.predefinedTokenDict);
    }

    public static readonly HashSet<String> lexerOptions = parserOptions;

    public static readonly HashSet<String> lexerRuleOptions = new();

    public static readonly HashSet<String> parseRuleOptions = new();

    public static readonly HashSet<String> parserBlockOptions = new HashSet<String>();

    public static readonly HashSet<String> lexerBlockOptions = new HashSet<String>();

    /** Legal options for rule refs like id&lt;key=value&gt; */
    public static readonly HashSet<String> ruleRefOptions = new HashSet<String>();
    /** Legal options for terminal refs like ID&lt;assoc=right&gt; */
    public static readonly HashSet<String> tokenOptions = new HashSet<String>();

    public static readonly HashSet<String> actionOptions = new HashSet<String>();

    public static readonly HashSet<String> semPredOptions = new HashSet<String>();

    public static readonly HashSet<String> doNotCopyOptionsToLexer = new HashSet<String>();

    public static readonly Dictionary<String, AttributeDict> grammarAndLabelRefTypeToScope =
        new();

    public String name;
    public GrammarRootAST ast;

    /** Track token stream used to create this grammar */

    public readonly TokenStream tokenStream;

    /** If we transform grammar, track original unaltered token stream.
	 *  This is set to the same value as tokenStream when tokenStream is
	 *  initially set.
	 *
	 *  If this field differs from tokenStream, then we have transformed
	 *  the grammar.
	 */

    public TokenStream originalTokenStream;

    public String text; // testing only
    public String fileName;

    /** Was this parser grammar created from a COMBINED grammar?  If so,
	 *  this is what we extracted.
	 */
    public LexerGrammar implicitLexer;

    /** If this is an extracted/implicit lexer, we point at original grammar */
    public Grammar originalGrammar;

    /** If we're imported, who imported us? If null, implies grammar is root */
    public Grammar parent;
    public List<Grammar> importedGrammars;

    /** All rules defined in this specific grammar, not imported. Also does
	 *  not include lexical rules if combined.
	 */
    public OrderedHashMap<String, Rule> rules = new OrderedHashMap<String, Rule>();
    public List<Rule> indexToRule = new();

    int ruleNumber = 0; // used to get rule indexes (0..n-1)
    int stringLiteralRuleNumber = 0; // used to invent rule names for 'keyword', ';', ... (0..n-1)

    /** The ATN that represents the grammar with edges labelled with tokens
	 *  or epsilon.  It is more suitable to analysis than an AST representation.
	 */
    public ATN atn;

    public Dictionary<int, Interval> stateToGrammarRegionMap;

    public Dictionary<int, DFA> decisionDFAs = new();

    public List<IntervalSet[]> decisionLOOK;

    public readonly Tool Tools;

    /** Token names and literal tokens like "void" are uniquely indexed.
	 *  with -1 implying EOF.  Characters are different; they go from
	 *  -1 (EOF) to \uFFFE.  For example, 0 could be a binary byte you
	 *  want to lexer.  Labels of DFA/ATN transitions can be both tokens
	 *  and characters.  I use negative numbers for bookkeeping labels
	 *  like EPSILON. Char/String literals and token types overlap in the same
	 *  space, however.
	 */
    int maxTokenType = Token.MIN_USER_TOKEN_TYPE - 1;

    /**
	 * Map token like {@code ID} (but not literals like {@code 'while'}) to its
	 * token type.
	 */
    public readonly Dictionary<String, int> tokenNameToTypeMap = new();

    /**
	 * Map token literals like {@code 'while'} to its token type. It may be that
	 * {@code WHILE="while"=35}, in which case both {@link #tokenNameToTypeMap}
	 * and this field will have entries both mapped to 35.
	 */
    public readonly Dictionary<String, int> stringLiteralToTypeMap = new();

    /**
	 * Reverse index for {@link #stringLiteralToTypeMap}. Indexed with raw token
	 * type. 0 is invalid.
	 */
    public readonly List<String> typeToStringLiteralList = new();

    /**
	 * Map a token type to its token name. Indexed with raw token type. 0 is
	 * invalid.
	 */
    public readonly List<String> typeToTokenList = new();

    /**
	 * The maximum channel value which is assigned by this grammar. Values below
	 * {@link Token#MIN_USER_CHANNEL_VALUE} are assumed to be predefined.
	 */
    int maxChannelType = Token.MIN_USER_CHANNEL_VALUE - 1;

    /**
	 * Map channel like {@code COMMENTS_CHANNEL} to its constant channel value.
	 * Only user-defined channels are defined in this map.
	 */
    public readonly Dictionary<String, int> channelNameToValueMap = new();

    /**
	 * Map a constant channel value to its name. Indexed with raw channel value.
	 * The predefined channels {@link Token#DEFAULT_CHANNEL} and
	 * {@link Token#HIDDEN_CHANNEL} are not stored in this list, so the values
	 * at the corresponding indexes is {@code null}.
	 */
    public readonly List<String> channelValueToNameList = new();

    /** Map a name to an action.
     *  The code generator will use this to fill holes in the output files.
     *  I track the AST node for the action in case I need the line number
     *  for errors.
     */
    public Dictionary<String, ActionAST> namedActions = new();

    /** Tracks all user lexer actions in all alternatives of all rules.
	 *  Doesn't track sempreds.  maps tree node to action index (alt number 1..n).
 	 */
    public Dictionary<ActionAST, int> lexerActions = new();

    /** All sempreds found in grammar; maps tree node to sempred index;
	 *  sempred index is 0..n-1
	 */
    public Dictionary<PredAST, int> sempreds = new();
    /** Map the other direction upon demand */
    public Dictionary<int, PredAST> indexToPredMap;

    public static readonly String AUTO_GENERATED_TOKEN_NAME_PREFIX = "T__";

    public Grammar(Tool tool, GrammarRootAST ast)
    {
        if (ast == null)
        {
            throw new NullReferenceException("ast");
        }

        if (ast.tokenStream == null)
        {
            throw new ArgumentException("ast must have a token stream");
        }

        this.Tools = tool;
        this.ast = ast;
        this.name = (ast.getChild(0)).getText();
        this.tokenStream = ast.tokenStream;
        this.originalTokenStream = this.tokenStream;

        initTokenSymbolTables();
    }

    /** For testing */
    public Grammar(String grammarText) : this(GRAMMAR_FROM_STRING_NAME, grammarText, null)
    {
    }

    public Grammar(String grammarText, LexerGrammar tokenVocabSource)
       : this(GRAMMAR_FROM_STRING_NAME, grammarText, tokenVocabSource, null)
    {
    }

    /** For testing */
    public Grammar(String grammarText, ANTLRToolListener listener)
        : this(GRAMMAR_FROM_STRING_NAME, grammarText, listener)
    {
    }

    /** For testing; builds trees, does sem anal */
    public Grammar(String fileName, String grammarText)
        : this(fileName, grammarText, null)
    {

    }

    /** For testing; builds trees, does sem anal */
    public Grammar(String fileName, String grammarText, ANTLRToolListener listener)
        : this(fileName, grammarText, null, listener)
    {

    }
    public class ATL : ANTLRToolListener
    {
        ////@Override

        public void info(String msg) { }
        ////@Override

        public void error(ANTLRMessage msg) { }
        ////@Override

        public void warning(ANTLRMessage msg) { }

    }

    public class TVA : TreeVisitorAction
    {
        readonly Grammar thiz;
        public TVA(Grammar thiz)
        {
            this.thiz = thiz;
        }

        ////@Override
        public Object pre(Object t) { ((GrammarAST)t).g = thiz; return t; }
        ////@Override
        public Object post(Object t) { return t; }
    }
    /** For testing; builds trees, does sem anal */
    public Grammar(String fileName, String grammarText, Grammar tokenVocabSource, ANTLRToolListener listener)

    {
        this.text = grammarText;
        this.fileName = fileName;
        this.Tools = new Tool();
        ANTLRToolListener hush = new ATL();
        Tools.addListener(hush); // we want to hush errors/warnings
        this.Tools.addListener(listener);
        org.antlr.runtime.ANTLRStringStream @in = new org.antlr.runtime.ANTLRStringStream(grammarText);
        @in.name = fileName;

        this.ast = Tools.parse(fileName, @in);
        if (ast == null)
        {
            throw new UnsupportedOperationException();
        }

        if (ast.tokenStream == null)
        {
            throw new IllegalStateException("expected ast to have a token stream");
        }

        this.tokenStream = ast.tokenStream;
        this.originalTokenStream = this.tokenStream;

        // ensure each node has pointer to surrounding grammar
        Grammar thiz = this;
        TreeVisitor v = new TreeVisitor(new GrammarASTAdaptor());
        v.visit(ast, new TVA(thiz));
        initTokenSymbolTables();

        if (tokenVocabSource != null)
        {
            importVocab(tokenVocabSource);
        }

        Tools.process(this, false);
    }

    protected void initTokenSymbolTables()
    {
        tokenNameToTypeMap.Add("EOF", Token.EOF);

        // reserve a spot for the INVALID token
        typeToTokenList.Add(null);
    }


    public void loadImportedGrammars()
    {
        this.loadImportedGrammars(new());
    }

    private void loadImportedGrammars(HashSet<String> visited)
    {
        if (ast == null) return;
        GrammarAST i = (GrammarAST)ast.getFirstChildWithType(ANTLRParser.IMPORT);
        if (i == null) return;
        visited.Add(this.name);
        importedGrammars = new();
        foreach (Object c in i.getChildren())
        {
            GrammarAST t = (GrammarAST)c;
            String importedGrammarName = null;
            if (t.getType() == ANTLRParser.ASSIGN)
            {
                t = (GrammarAST)t.getChild(1);
                importedGrammarName = t.getText();
            }
            else if (t.getType() == ANTLRParser.ID)
            {
                importedGrammarName = t.getText();
            }
            if (visited.Contains(importedGrammarName))
            { // ignore circular refs
                continue;
            }
            Grammar g;
            try
            {
                g = Tools.loadImportedGrammar(this, t);
            }
            catch (IOException ioe)
            {
                Tools.ErrMgr.GrammarError(ErrorType.ERROR_READING_IMPORTED_GRAMMAR,
                                         importedGrammarName,
                                         t.getToken(),
                                         importedGrammarName,
                                         name);
                continue;
            }
            // did it come back as error node or missing?
            if (g == null) continue;
            g.parent = this;
            importedGrammars.Add(g);
            g.loadImportedGrammars(visited); // recursively pursue any imports in this import
        }
    }

    public void defineAction(GrammarAST atAST)
    {
        if (atAST.getChildCount() == 2)
        {
            String name = atAST.getChild(0).getText();
            namedActions[name] = (ActionAST)atAST.getChild(1);
        }
        else
        {
            String scope = atAST.getChild(0).getText();
            String gtype = getTypeString();
            if (scope.Equals(gtype) || (scope.Equals("parser") && gtype.Equals("combined")))
            {
                String name = atAST.getChild(1).getText();
                namedActions[name] = (ActionAST)atAST.getChild(2);
            }
        }
    }

    /**
	 * Define the specified rule in the grammar. This method assigns the rule's
	 * {@link Rule#index} according to the {@link #ruleNumber} field, and adds
	 * the {@link Rule} instance to {@link #rules} and {@link #indexToRule}.
	 *
	 * @param r The rule to define in the grammar.
	 * @return {@code true} if the rule was added to the {@link Grammar}
	 * instance; otherwise, {@code false} if a rule with this name already
	 * existed in the grammar instance.
	 */
    public bool defineRule(Rule r)
    {
        if (rules.ContainsKey(r.name))
        {
            return false;
        }
        rules.Put(r.name, r);
        r.index = ruleNumber++;
        indexToRule.Add(r);
        return true;
    }

    /**
	 * Undefine the specified rule from this {@link Grammar} instance. The
	 * instance {@code r} is removed from {@link #rules} and
	 * {@link #indexToRule}. This method updates the {@link Rule#index} field
	 * for all rules defined after {@code r}, and decrements {@link #ruleNumber}
	 * in preparation for adding new rules.
	 * <p>
	 * This method does nothing if the current {@link Grammar} does not contain
	 * the instance {@code r} at index {@code r.index} in {@link #indexToRule}.
	 * </p>
	 *
	 * @param r
	 * @return {@code true} if the rule was removed from the {@link Grammar}
	 * instance; otherwise, {@code false} if the specified rule was not defined
	 * in the grammar.
	 */
    public bool undefineRule(Rule r)
    {
        if (r.index < 0 || r.index >= indexToRule.Count || indexToRule[(r.index)] != r)
        {
            return false;
        }

        //assert rules.get(r.name) == r;

        rules.Remove(r.name);
        indexToRule.RemoveAt(r.index);
        for (int i = r.index; i < indexToRule.Count; i++)
        {
            //assert indexToRule.get(i).index == i + 1;
            indexToRule[(i)].index--;
        }

        ruleNumber--;
        return true;
    }

    //	public int getNumRules() {
    //		int n = rules.size();
    //		List<Grammar> imports = getAllImportedGrammars();
    //		if ( imports!=null ) {
    //			for (Grammar g : imports) n += g.getNumRules();
    //		}
    //		return n;
    //	}

    public Rule getRule(String name)
    {
        if (rules.TryGetValue(name,out var r)) return r;
        return null;
        /*
		List<Grammar> imports = getAllImportedGrammars();
		if ( imports==null ) return null;
		for (Grammar g : imports) {
			r = g.getRule(name); // recursively walk up hierarchy
			if ( r!=null ) return r;
		}
		return null;
		*/
    }

    public ATN getATN()
    {
        if (atn == null)
        {
            ParserATNFactory factory = new ParserATNFactory(this);
            atn = factory.CreateATN();
        }
        return atn;
    }

    public Rule getRule(int index) { return indexToRule[(index)]; }

    public Rule getRule(String grammarName, String ruleName)
    {
        if (grammarName != null)
        { // scope override
            Grammar g = getImportedGrammar(grammarName);
            if (g == null)
            {
                return null;
            }
            return g.rules.TryGetValue(ruleName,out var r)?r:null;
        }
        return getRule(ruleName);
    }

    /** Get list of all imports from all grammars in the delegate subtree of g.
     *  The grammars are in import tree preorder.  Don't include ourselves
     *  in list as we're not a delegate of ourselves.
     */
    public List<Grammar> getAllImportedGrammars()
    {
        if (importedGrammars == null)
        {
            return null;
        }

        Dictionary<String, Grammar> delegates = new();
        foreach (Grammar d in importedGrammars)
        {
            delegates[d.fileName] = d;
            List<Grammar> ds = d.getAllImportedGrammars();
            if (ds != null)
            {
                foreach (Grammar imported in ds)
                {
                    delegates[imported.fileName]= imported;
                }
            }
        }

        return new(delegates.Values);
    }

    public List<Grammar> getImportedGrammars() { return importedGrammars; }

    public LexerGrammar getImplicitLexer()
    {
        return implicitLexer;
    }

    /** convenience method for Tool.loadGrammar() */
    public static Grammar load(String fileName)
    {
        Tool antlr = new Tool();
        return antlr.loadGrammar(fileName);
    }

    /** Return list of imported grammars from root down to our parent.
     *  Order is [root, ..., this.parent].  (us not included).
     */
    public List<Grammar> getGrammarAncestors()
    {
        Grammar root = getOutermostGrammar();
        if (this == root) return null;
        List<Grammar> grammars = new();
        // walk backwards to root, collecting grammars
        Grammar p = this.parent;
        while (p != null)
        {
            grammars.Insert(0, p); // add to head so in order later
            p = p.parent;
        }
        return grammars;
    }

    /** Return the grammar that imported us and our parents. Return this
     *  if we're root.
     */
    public Grammar getOutermostGrammar()
    {
        if (parent == null) return this;
        return parent.getOutermostGrammar();
    }

    /** Get the name of the generated recognizer; may or may not be same
     *  as grammar name.
     *  Recognizer is TParser and TLexer from T if combined, else
     *  just use T regardless of grammar type.
     */
    public String getRecognizerName()
    {
        String suffix = "";
        List<Grammar> grammarsFromRootToMe = getOutermostGrammar().getGrammarAncestors();
        String qualifiedName = name;
        if (grammarsFromRootToMe != null)
        {
            StringBuilder buf = new StringBuilder();
            foreach (Grammar g in grammarsFromRootToMe)
            {
                buf.Append(g.name);
                buf.Append('_');
            }
            buf.Append(name);
            qualifiedName = buf.ToString();
        }

        if (isCombined() || (isLexer() && implicitLexer != null))
        {
            suffix = Grammar.getGrammarTypeToFileNameSuffix(getType());
        }
        return qualifiedName + suffix;
    }

    public String getStringLiteralLexerRuleName(String lit)
    {
        return AUTO_GENERATED_TOKEN_NAME_PREFIX + stringLiteralRuleNumber++;
    }

    /** Return grammar directly imported by this grammar */
    public Grammar getImportedGrammar(String name)
    {
        foreach (Grammar g in importedGrammars)
        {
            if (g.name.Equals(name)) return g;
        }
        return null;
    }

    public int getTokenType(String token)
    {
        if (token[(0)] == '\'')
        {
            return stringLiteralToTypeMap.TryGetValue(token,out var r)?r: Token.INVALID_TYPE;
        }
        else
        { // must be a label like ID
            return tokenNameToTypeMap.TryGetValue(token, out var r) ? r : Token.INVALID_TYPE;
        }
        //tool.log("grammar", "grammar type "+type+" "+tokenName+"->"+i);
    }

    public String getTokenName(String literal)
    {
        Grammar grammar = this;
        while (grammar != null)
        {
            if (grammar.stringLiteralToTypeMap.TryGetValue(literal,out var v))
                return grammar.getTokenName(v);
            grammar = grammar.parent;
        }
        return null;
    }

    /** Given a token type, get a meaningful name for it such as the ID
	 *  or string literal.  If this is a lexer and the ttype is in the
	 *  char vocabulary, compute an ANTLR-valid (possibly escaped) char literal.
	 */
    public String getTokenDisplayName(int ttype)
    {
        // inside any target's char range and is lexer grammar?
        if (isLexer() &&
             ttype >= Lexer.MIN_CHAR_VALUE && ttype <= Lexer.MAX_CHAR_VALUE)
        {
            return CharSupport.GetANTLRCharLiteralForChar(ttype);
        }

        if (ttype == Token.EOF)
        {
            return "EOF";
        }

        if (ttype == Token.INVALID_TYPE)
        {
            return INVALID_TOKEN_NAME;
        }

        if (ttype >= 0 && ttype < typeToStringLiteralList.Count && typeToStringLiteralList[ttype] != null)
        {
            return typeToStringLiteralList[ttype];
        }

        if (ttype >= 0 && ttype < typeToTokenList.Count && typeToTokenList[ttype] != null)
        {
            return typeToTokenList[ttype];
        }

        return ttype.ToString();// String.valueOf(ttype);
    }

    /**
	 * Gets the name by which a token can be referenced in the generated code.
	 * For tokens defined in a {@code tokens{}} block or via a lexer rule, this
	 * is the declared name of the token. For token types generated by the use
	 * of a string literal within a parser rule of a combined grammar, this is
	 * the automatically generated token type which includes the
	 * {@link #AUTO_GENERATED_TOKEN_NAME_PREFIX} prefix. For types which are not
	 * associated with a defined token, this method returns
	 * {@link #INVALID_TOKEN_NAME}.
	 *
	 * @param ttype The token type.
	 * @return The name of the token with the specified type.
	 */

    public String getTokenName(int ttype)
    {
        // inside any target's char range and is lexer grammar?
        if (isLexer() &&
             ttype >= Lexer.MIN_CHAR_VALUE && ttype <= Lexer.MAX_CHAR_VALUE)
        {
            return CharSupport.GetANTLRCharLiteralForChar(ttype);
        }

        if (ttype == Token.EOF)
        {
            return "EOF";
        }

        if (ttype >= 0 && ttype < typeToTokenList.Count && typeToTokenList[ttype] != null)
        {
            return typeToTokenList[ttype];
        }

        return INVALID_TOKEN_NAME;
    }

    /**
	 * Gets the constant channel value for a user-defined channel.
	 *
	 * <p>
	 * This method only returns channel values for user-defined channels. All
	 * other channels, including the predefined channels
	 * {@link Token#DEFAULT_CHANNEL} and {@link Token#HIDDEN_CHANNEL} along with
	 * any channel defined in code (e.g. in a {@code @members{}} block), are
	 * ignored.</p>
	 *
	 * @param channel The channel name.
	 * @return The channel value, if {@code channel} is the name of a known
	 * user-defined token channel; otherwise, -1.
	 */
    public int getChannelValue(String channel)
    {
        return channelNameToValueMap.TryGetValue(channel,out var r)?r:-1;
    }

    /**
	 * Gets an array of rule names for rules defined or imported by the
	 * grammar. The array index is the rule index, and the value is the name of
	 * the rule with the corresponding {@link Rule#index}.
	 *
	 * <p>If no rule is defined with an index for an element of the resulting
	 * array, the value of that element is {@link #INVALID_RULE_NAME}.</p>
	 *
	 * @return The names of all rules defined in the grammar.
	 */
    public String[] getRuleNames()
    {
        String[] result = new String[rules.Count];
        Array.Fill(result, INVALID_RULE_NAME);
        foreach (Rule rule in rules.Values)
        {
            result[rule.index] = rule.name;
        }

        return result;
    }

    /**
	 * Gets an array of token names for tokens defined or imported by the
	 * grammar. The array index is the token type, and the value is the result
	 * of {@link #getTokenName} for the corresponding token type.
	 *
	 * @see #getTokenName
	 * @return The token names of all tokens defined in the grammar.
	 */
    public String[] getTokenNames()
    {
        int numTokens = getMaxTokenType();
        String[] tokenNames = new String[numTokens + 1];
        for (int i = 0; i < tokenNames.Length; i++)
        {
            tokenNames[i] = getTokenName(i);
        }

        return tokenNames;
    }

    /**
	 * Gets an array of display names for tokens defined or imported by the
	 * grammar. The array index is the token type, and the value is the result
	 * of {@link #getTokenDisplayName} for the corresponding token type.
	 *
	 * @see #getTokenDisplayName
	 * @return The display names of all tokens defined in the grammar.
	 */
    public String[] getTokenDisplayNames()
    {
        int numTokens = getMaxTokenType();
        String[] tokenNames = new String[numTokens + 1];
        for (int i = 0; i < tokenNames.Length; i++)
        {
            tokenNames[i] = getTokenDisplayName(i);
        }

        return tokenNames;
    }

    /**
	 * Gets the literal names assigned to tokens in the grammar.
	 */

    public String[] getTokenLiteralNames()
    {
        int numTokens = getMaxTokenType();
        String[] literalNames = new String[numTokens + 1];
        for (int i = 0; i < Math.Min(literalNames.Length, typeToStringLiteralList.Count); i++)
        {
            literalNames[i] = typeToStringLiteralList[i];
        }

        foreach (var entry in stringLiteralToTypeMap)
        {
            int value = entry.Value;
            if (value >= 0 && value < literalNames.Length && literalNames[value] == null)
            {
                literalNames[value] = entry.Key;
            }
        }

        return literalNames;
    }

    /**
	 * Gets the symbolic names assigned to tokens in the grammar.
	 */

    public String[] getTokenSymbolicNames()
    {
        int numTokens = getMaxTokenType();
        String[] symbolicNames = new String[numTokens + 1];
        for (int i = 0; i < Math.Min(symbolicNames.Length, typeToTokenList.Count); i++)
        {
            if (typeToTokenList[i] == null || typeToTokenList[(i)].StartsWith(AUTO_GENERATED_TOKEN_NAME_PREFIX))
            {
                continue;
            }

            symbolicNames[i] = typeToTokenList[i];
        }

        return symbolicNames;
    }

    /**
	 * Gets a {@link Vocabulary} instance describing the vocabulary used by the
	 * grammar.
	 */

    public Vocabulary getVocabulary()
    {
        return new VocabularyImpl(getTokenLiteralNames(), getTokenSymbolicNames());
    }

    /** Given an arbitrarily complex SemanticContext, walk the "tree" and get display string.
	 *  Pull predicates from grammar text.
	 */
    public String getSemanticContextDisplayString(SemanticContext semctx)
    {
        if (semctx is SemanticContext.Predicate)
        {
            return getPredicateDisplayString((SemanticContext.Predicate)semctx);
        }
        if (semctx is SemanticContext.AND)
        {
            SemanticContext.AND and = (SemanticContext.AND)semctx;
            return joinPredicateOperands(and, " and ");
        }
        if (semctx is SemanticContext.OR)
        {
            SemanticContext.OR or = (SemanticContext.OR)semctx;
            return joinPredicateOperands(or, " or ");
        }
        return semctx.ToString();
    }

    public String joinPredicateOperands(SemanticContext.Operator op, String separator)
    {
        StringBuilder buf = new StringBuilder();
        foreach (SemanticContext operand in op.getOperands())
        {
            if (buf.Length > 0)
            {
                buf.Append(separator);
            }

            buf.Append(getSemanticContextDisplayString(operand));
        }

        return buf.ToString();
    }

    public Dictionary<int, PredAST> getIndexToPredicateMap()
    {
        Dictionary<int, PredAST> indexToPredMap = new Dictionary<int, PredAST>();
        foreach (Rule r in rules.Values)
        {
            foreach (ActionAST a in r.actions)
            {
                if (a is PredAST)
                {
                    PredAST p = (PredAST)a;
                    if(sempreds.TryGetValue(p, out var r2))
                    {
                        indexToPredMap[r2] = p;
                    }
                }
            }
        }
        return indexToPredMap;
    }

    public String getPredicateDisplayString(SemanticContext.Predicate pred)
    {
        if (indexToPredMap == null)
        {
            indexToPredMap = getIndexToPredicateMap();
        }
        ActionAST? actionAST = indexToPredMap.TryGetValue(pred.predIndex,out var r)?r:null;
        return actionAST?.getText();
    }

    /** What is the max char value possible for this grammar's target?  Use
	 *  unicode max if no target defined.
	 */
    public int getMaxCharValue()
    {
        return org.antlr.v4.runtime.Lexer.MAX_CHAR_VALUE;
        //		if ( generator!=null ) {
        //			return generator.getTarget().getMaxCharValue(generator);
        //		}
        //		else {
        //			return Label.MAX_CHAR_VALUE;
        //		}
    }

    /** Return a set of all possible token or char types for this grammar */
    public IntSet getTokenTypes()
    {
        if (isLexer())
        {
            return getAllCharValues();
        }
        return IntervalSet.of(Token.MIN_USER_TOKEN_TYPE, getMaxTokenType());
    }

    /** Return min to max char as defined by the target.
	 *  If no target, use max unicode char value.
	 */
    public IntSet getAllCharValues()
    {
        return IntervalSet.of(Lexer.MIN_CHAR_VALUE, getMaxCharValue());
    }

    /** How many token types have been allocated so far? */
    public int getMaxTokenType()
    {
        return typeToTokenList.Count - 1; // don't count 0 (invalid)
    }

    /** Return a new unique integer in the token type space */
    public int getNewTokenType()
    {
        maxTokenType++;
        return maxTokenType;
    }

    /** Return a new unique integer in the channel value space. */
    public int getNewChannelNumber()
    {
        maxChannelType++;
        return maxChannelType;
    }

    public void importTokensFromTokensFile()
    {
        String vocab = getOptionString("tokenVocab");
        if (vocab != null)
        {
            TokenVocabParser vparser = new TokenVocabParser(this);
            Dictionary<String, int> tokens = vparser.load();
            Tools.Log("grammar", "tokens=" + tokens);
            foreach (String t in tokens.Keys)
            {
                if(tokens.TryGetValue(t, out var ret))
                {
                    if (t[0] == '\'') defineStringLiteral(t, ret);
                    else defineTokenName(t, ret);
                }
            }
        }
    }

    public void importVocab(Grammar importG)
    {
        foreach (String tokenName in importG.tokenNameToTypeMap.Keys)
        {
            defineTokenName(tokenName, importG.tokenNameToTypeMap[(tokenName)]);
        }
        foreach (String tokenName in importG.stringLiteralToTypeMap.Keys)
        {
            defineStringLiteral(tokenName, importG.stringLiteralToTypeMap[(tokenName)]);
        }
        foreach (var channel in importG.channelNameToValueMap)
        {
            defineChannelName(channel.Key, channel.Value);
        }
        //		this.tokenNameToTypeMap.putAll( importG.tokenNameToTypeMap );
        //		this.stringLiteralToTypeMap.putAll( importG.stringLiteralToTypeMap );
        int max = Math.Max(this.typeToTokenList.Count, importG.typeToTokenList.Count);
        Utils.SetSize(typeToTokenList, max);
        for (int ttype = 0; ttype < importG.typeToTokenList.Count; ttype++)
        {
            maxTokenType = Math.Max(maxTokenType, ttype);
            this.typeToTokenList[ttype]= importG.typeToTokenList[(ttype)];
        }

        max = Math.Max(this.channelValueToNameList.Count, importG.channelValueToNameList.Count);
        Utils.SetSize(channelValueToNameList, max);
        for (int channelValue = 0; channelValue < importG.channelValueToNameList.Count; channelValue++)
        {
            maxChannelType = Math.Max(maxChannelType, channelValue);
            this.channelValueToNameList[channelValue]= importG.channelValueToNameList[channelValue];
        }
    }

    public int defineTokenName(String name)
    {
        if (!tokenNameToTypeMap.TryGetValue(name,out var prev))
            return defineTokenName(name, getNewTokenType());
        return prev;
    }

    public int defineTokenName(String name, int ttype)
    {
        if (tokenNameToTypeMap.TryGetValue(name,out var prev)) return prev;
        tokenNameToTypeMap[name]= ttype;
        setTokenForType(ttype, name);
        maxTokenType = Math.Max(maxTokenType, ttype);
        return ttype;
    }

    public int defineStringLiteral(String lit)
    {
        if (stringLiteralToTypeMap.ContainsKey(lit))
        {
            return stringLiteralToTypeMap[(lit)];
        }
        return defineStringLiteral(lit, getNewTokenType());

    }

    public int defineStringLiteral(String lit, int ttype)
    {
        if (!stringLiteralToTypeMap.ContainsKey(lit))
        {
            stringLiteralToTypeMap[lit]= ttype;
            // track in reverse index too
            if (ttype >= typeToStringLiteralList.Count)
            {
                Utils.SetSize(typeToStringLiteralList, ttype + 1);
            }
            typeToStringLiteralList[ttype]= lit;

            setTokenForType(ttype, lit);
            return ttype;
        }
        return Token.INVALID_TYPE;
    }

    public int defineTokenAlias(String name, String lit)
    {
        int ttype = defineTokenName(name);
        stringLiteralToTypeMap[lit] = ttype;
        setTokenForType(ttype, name);
        return ttype;
    }

    public void setTokenForType(int ttype, String text)
    {
        if (ttype == Token.EOF)
        {
            // ignore EOF, it will be reported as an error separately
            return;
        }

        if (ttype >= typeToTokenList.Count)
        {
            Utils.SetSize(typeToTokenList, ttype + 1);
        }
        String prevToken = typeToTokenList[ttype];
        if (prevToken == null || prevToken[(0)] == '\'')
        {
            // only record if nothing there before or if thing before was a literal
            typeToTokenList[ttype] = text;
        }
    }

    /**
	 * Define a token channel with a specified name.
	 *
	 * <p>
	 * If a channel with the specified name already exists, the previously
	 * assigned channel value is returned.</p>
	 *
	 * @param name The channel name.
	 * @return The constant channel value assigned to the channel.
	 */
    public int defineChannelName(String name)
    {
        if (!channelNameToValueMap.TryGetValue(name,out var prev))
        {
            return defineChannelName(name, getNewChannelNumber());
        }

        return prev;
    }

    /**
	 * Define a token channel with a specified name.
	 *
	 * <p>
	 * If a channel with the specified name already exists, the previously
	 * assigned channel value is not altered.</p>
	 *
	 * @param name The channel name.
	 * @return The constant channel value assigned to the channel.
	 */
    public int defineChannelName(String name, int value)
    {
        if (channelNameToValueMap.TryGetValue(name,out var prev))
        {
            return prev;
        }

        channelNameToValueMap[name] = value;
        setChannelNameForValue(value, name);
        maxChannelType = Math.Max(maxChannelType, value);
        return value;
    }

    /**
	 * Sets the channel name associated with a particular channel value.
	 *
	 * <p>
	 * If a name has already been assigned to the channel with constant value
	 * {@code channelValue}, this method does nothing.</p>
	 *
	 * @param channelValue The constant value for the channel.
	 * @param name The channel name.
	 */
    public void setChannelNameForValue(int channelValue, String name)
    {
        if (channelValue >= channelValueToNameList.Count)
        {
            Utils.SetSize(channelValueToNameList, channelValue + 1);
        }

        String prevChannel = channelValueToNameList[channelValue];
        if (prevChannel == null)
        {
            channelValueToNameList[channelValue]= name;
        }
    }

    // no isolated attr at grammar action level
    //@Override
    public Attribute resolveToAttribute(String x, ActionAST node)
    {
        return null;
    }

    // no $x.y makes sense here
    //@Override
    public Attribute resolveToAttribute(String x, String y, ActionAST node)
    {
        return null;
    }

    //@Override
    public bool resolvesToLabel(String x, ActionAST node) { return false; }

    //@Override
    public bool resolvesToListLabel(String x, ActionAST node) { return false; }

    //@Override
    public bool resolvesToToken(String x, ActionAST node) { return false; }

    //@Override
    public bool resolvesToAttributeDict(String x, ActionAST node)
    {
        return false;
    }

    /** Given a grammar type, what should be the default action scope?
     *  If I say @members in a COMBINED grammar, for example, the
     *  default scope should be "parser".
     */
    public String getDefaultActionScope()
    {
        switch (getType())
        {
            case ANTLRParser.LEXER:
                return "lexer";
            case ANTLRParser.PARSER:
            case ANTLRParser.COMBINED:
                return "parser";
        }
        return null;
    }

    public int getType()
    {
        if (ast != null) return ast.grammarType;
        return 0;
    }

    public TokenStream getTokenStream()
    {
        if (ast != null) return ast.tokenStream;
        return null;
    }

    public bool isLexer() { return getType() == ANTLRParser.LEXER; }
    public bool isParser() { return getType() == ANTLRParser.PARSER; }
    public bool isCombined() { return getType() == ANTLRParser.COMBINED; }

    /** Is id a valid token name? Does id start with an uppercase letter? */
    public static bool isTokenName(String id)
    {
        return char.IsUpper(id[(0)]);
    }

    public String getTypeString()
    {
        if (ast == null) return null;
        return ANTLRParser.tokenNames[getType()].ToLower();
    }

    public static String getGrammarTypeToFileNameSuffix(int type)
    {
        switch (type)
        {
            case ANTLRParser.LEXER: return "Lexer";
            case ANTLRParser.PARSER: return "Parser";
            // if combined grammar, gen Parser and Lexer will be done later
            // TODO: we are separate now right?
            case ANTLRParser.COMBINED: return "Parser";
            default:
                return "<invalid>";
        }
    }

    public String getLanguage()
    {
        return getOptionString("language");
    }

    public String getOptionString(String key) { return ast.getOptionString(key); }

    /** Given ^(TOKEN_REF ^(OPTIONS ^(ELEMENT_OPTIONS (= assoc right))))
	 *  set option assoc=right in TOKEN_REF.
	 */
    public static void setNodeOptions(GrammarAST node, GrammarAST options)
    {
        if (options == null) return;
        GrammarASTWithOptions t = (GrammarASTWithOptions)node;
        if (t.getChildCount() == 0 || options.getChildCount() == 0) return;
        foreach (Object o in options.getChildren())
        {
            GrammarAST c = (GrammarAST)o;
            if (c.getType() == ANTLRParser.ASSIGN)
            {
                t.setOption(c.getChild(0).getText(), (GrammarAST)c.getChild(1));
            }
            else
            {
                t.setOption(c.getText(), null); // no arg such as ID<VarNodeType>
            }
        }
    }

    /** Return list of (TOKEN_NAME node, 'literal' node) pairs */
    public static List<Pair<GrammarAST, GrammarAST>> getStringLiteralAliasesFromLexerRules(GrammarRootAST ast)
    {
        String[] patterns = {
            "(RULE %name:TOKEN_REF (BLOCK (ALT %lit:STRING_LITERAL)))",
            "(RULE %name:TOKEN_REF (BLOCK (ALT %lit:STRING_LITERAL ACTION)))",
            "(RULE %name:TOKEN_REF (BLOCK (ALT %lit:STRING_LITERAL SEMPRED)))",
            "(RULE %name:TOKEN_REF (BLOCK (LEXER_ALT_ACTION (ALT %lit:STRING_LITERAL) .)))",
            "(RULE %name:TOKEN_REF (BLOCK (LEXER_ALT_ACTION (ALT %lit:STRING_LITERAL) . .)))",
            "(RULE %name:TOKEN_REF (BLOCK (LEXER_ALT_ACTION (ALT %lit:STRING_LITERAL) (LEXER_ACTION_CALL . .))))",
            "(RULE %name:TOKEN_REF (BLOCK (LEXER_ALT_ACTION (ALT %lit:STRING_LITERAL) . (LEXER_ACTION_CALL . .))))",
            "(RULE %name:TOKEN_REF (BLOCK (LEXER_ALT_ACTION (ALT %lit:STRING_LITERAL) (LEXER_ACTION_CALL . .) .)))",
			// TODO: allow doc comment in there
		};
        GrammarASTAdaptor adaptor = new GrammarASTAdaptor(ast.token.getInputStream());
        TreeWizard wiz = new TreeWizard(adaptor, ANTLRParser.tokenNames);
        List<Pair<GrammarAST, GrammarAST>> lexerRuleToStringLiteral =
            new();

        List<GrammarAST> ruleNodes = ast.getNodesWithType(ANTLRParser.RULE);
        if (ruleNodes == null || ruleNodes.Count == 0) return null;

        foreach (GrammarAST r in ruleNodes)
        {
            //tool.log("grammar", r.toStringTree());
            //			Console.Out.WriteLine("chk: "+r.toStringTree());
            Tree name = r.getChild(0);
            if (name.getType() == ANTLRParser.TOKEN_REF)
            {
                // check rule against patterns
                bool isLitRule;
                foreach (String pattern in patterns)
                {
                    isLitRule =
                        defAlias(r, pattern, wiz, lexerRuleToStringLiteral);
                    if (isLitRule) break;
                }
                //				if ( !isLitRule ) Console.Out.WriteLine("no pattern matched");
            }
        }
        return lexerRuleToStringLiteral;
    }

    protected static bool defAlias(GrammarAST r, String pattern,
                                      TreeWizard wiz,
                                      List<Pair<GrammarAST, GrammarAST>> lexerRuleToStringLiteral)
    {
        Dictionary<string, object> nodes = new();
        if (wiz.parse(r, pattern, nodes))
        {
            if(nodes.TryGetValue("lit",out var litNode) && litNode is GrammarAST ln 
                && nodes.TryGetValue("name",out var nameNode) && nameNode is GrammarAST nn)
            {
                Pair<GrammarAST, GrammarAST> pair =
                    new Pair<GrammarAST, GrammarAST>(nn, ln);
                lexerRuleToStringLiteral.Add(pair);
                return true;
            }
        }
        return false;
    }
    public class GTV : GrammarTreeVisitor
    {
        public readonly Grammar g;
        public GTV(Grammar g)
        {
            this.g = g;
        }

        //@Override

        public void stringRef(TerminalAST @ref)
        {
            g.strings.Add(@ref.getText());
        }
        //@Override

        public ErrorManager getErrorManager() { return g.Tools.ErrMgr; }
    }
    HashSet<String> strings = new HashSet<String>();
    public HashSet<String> getStringLiterals()
    {
        strings.Clear();
        GrammarTreeVisitor collector = new GTV(this);
        collector.visitGrammar(ast);
        return strings;
    }

    public void setLookaheadDFA(int decision, DFA lookaheadDFA)
    {
        decisionDFAs[decision]= lookaheadDFA;
    }

    public static Dictionary<int, Interval> getStateToGrammarRegionMap(GrammarRootAST ast, IntervalSet grammarTokenTypes)
    {
        Dictionary<int, Interval> stateToGrammarRegionMap = new();
        if (ast == null) return stateToGrammarRegionMap;

        List<GrammarAST> nodes = ast.getNodesWithType(grammarTokenTypes);
        foreach (GrammarAST n in nodes)
        {
            if (n.atnState != null)
            {
                Interval tokenRegion = Interval.of(n.getTokenStartIndex(), n.getTokenStopIndex());
                Tree ruleNode = null;
                // RULEs, BLOCKs of transformed recursive rules point to original token interval
                switch (n.getType())
                {
                    case ANTLRParser.RULE:
                        ruleNode = n;
                        break;
                    case ANTLRParser.BLOCK:
                    case ANTLRParser.CLOSURE:
                        ruleNode = n.getAncestor(ANTLRParser.RULE);
                        break;
                }
                if (ruleNode is RuleAST)
                {
                    String ruleName = ((RuleAST)ruleNode).getRuleName();
                    Rule r = ast.g.getRule(ruleName);
                    if (r is LeftRecursiveRule)
                    {
                        RuleAST originalAST = ((LeftRecursiveRule)r).getOriginalAST();
                        tokenRegion = Interval.of(originalAST.getTokenStartIndex(), originalAST.getTokenStopIndex());
                    }
                }
                stateToGrammarRegionMap[n.atnState.stateNumber]= tokenRegion;
            }
        }
        return stateToGrammarRegionMap;
    }

    /** Given an ATN state number, return the token index range within the grammar from which that ATN state was derived. */
    public Interval getStateToGrammarRegion(int atnStateNumber)
    {
        if (stateToGrammarRegionMap == null)
        {
            stateToGrammarRegionMap = getStateToGrammarRegionMap(ast, null); // map all nodes with non-null atn state ptr
        }
        if (stateToGrammarRegionMap == null) return Interval.INVALID;

        return stateToGrammarRegionMap.TryGetValue(atnStateNumber,out var r)?r:null;
    }

    public LexerInterpreter createLexerInterpreter(CharStream input)
    {
        if (this.isParser())
        {
            throw new IllegalStateException("A lexer interpreter can only be created for a lexer or combined grammar.");
        }

        if (this.isCombined())
        {
            return implicitLexer.createLexerInterpreter(input);
        }

        List<String> allChannels = new();
        allChannels.Add("DEFAULT_TOKEN_CHANNEL");
        allChannels.Add("HIDDEN");
        allChannels.AddRange(channelValueToNameList);

        // must run ATN through serializer to set some state flags
        IntegerList serialized = ATNSerializer.getSerialized(atn);
        ATN deserializedATN = new ATNDeserializer().deserialize(serialized.toArray());
        return new LexerInterpreter(
                fileName,
                getVocabulary(),
                Arrays.AsList(getRuleNames()),
                allChannels,
                ((LexerGrammar)this).modes.Keys,
                deserializedATN,
                input);
    }

    /** @since 4.5.1 */
    public GrammarParserInterpreter createGrammarParserInterpreter(TokenStream tokenStream)
    {
        if (this.isLexer())
        {
            throw new IllegalStateException("A parser interpreter can only be created for a parser or combined grammar.");
        }
        // must run ATN through serializer to set some state flags
        IntegerList serialized = ATNSerializer.getSerialized(atn);
        ATN deserializedATN = new ATNDeserializer().deserialize(serialized.toArray());

        return new GrammarParserInterpreter(this, deserializedATN, tokenStream);
    }

    public ParserInterpreter createParserInterpreter(TokenStream tokenStream)
    {
        if (this.isLexer())
        {
            throw new IllegalStateException("A parser interpreter can only be created for a parser or combined grammar.");
        }

        // must run ATN through serializer to set some state flags
        IntegerList serialized = ATNSerializer.getSerialized(atn);
        ATN deserializedATN = new ATNDeserializer().deserialize(serialized.toArray());

        return new ParserInterpreter(fileName, getVocabulary(), Arrays.AsList(getRuleNames()), deserializedATN, tokenStream);
    }
}
