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
    public static readonly string GRAMMAR_FROM_STRING_NAME = "<string>";
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
    public static readonly string INVALID_TOKEN_NAME = "<INVALID>";
    /**
	 * This value is used as the name for elements in the array returned by
	 * {@link #getRuleNames} for indexes not associated with a rule.
	 */
    public static readonly string INVALID_RULE_NAME = "<invalid>";

    public static readonly string caseInsensitiveOptionName = "caseInsensitive";

    public static readonly HashSet<string> parserOptions = new();
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

    public static readonly HashSet<string> lexerOptions = parserOptions;

    public static readonly HashSet<string> lexerRuleOptions = new();

    public static readonly HashSet<string> parseRuleOptions = new();

    public static readonly HashSet<string> parserBlockOptions = new();

    public static readonly HashSet<string> lexerBlockOptions = new();

    /** Legal options for rule refs like id&lt;key=value&gt; */
    public static readonly HashSet<string> ruleRefOptions = new();
    /** Legal options for terminal refs like ID&lt;assoc=right&gt; */
    public static readonly HashSet<string> tokenOptions = new();

    public static readonly HashSet<string> actionOptions = new();

    public static readonly HashSet<string> semPredOptions = new();

    public static readonly HashSet<string> doNotCopyOptionsToLexer = new();

    public static readonly Dictionary<string, AttributeDict> grammarAndLabelRefTypeToScope = new();

    public string name;
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

    public string text; // testing only
    public string fileName;

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
    public OrderedHashMap<string, Rule> rules = new();
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
	 *  like EPSILON. Char/string literals and token types overlap in the same
	 *  space, however.
	 */
    int maxTokenType = Token.MIN_USER_TOKEN_TYPE - 1;

    /**
	 * Map token like {@code ID} (but not literals like {@code 'while'}) to its
	 * token type.
	 */
    public readonly Dictionary<string, int> tokenNameToTypeMap = new();

    /**
	 * Map token literals like {@code 'while'} to its token type. It may be that
	 * {@code WHILE="while"=35}, in which case both {@link #tokenNameToTypeMap}
	 * and this field will have entries both mapped to 35.
	 */
    public readonly Dictionary<string, int> stringLiteralToTypeMap = new();

    /**
	 * Reverse index for {@link #stringLiteralToTypeMap}. Indexed with raw token
	 * type. 0 is invalid.
	 */
    public readonly List<string> typeToStringLiteralList = new();

    /**
	 * Map a token type to its token name. Indexed with raw token type. 0 is
	 * invalid.
	 */
    public readonly List<string> typeToTokenList = new();

    /**
	 * The maximum channel value which is assigned by this grammar. Values below
	 * {@link Token#MIN_USER_CHANNEL_VALUE} are assumed to be predefined.
	 */
    int maxChannelType = Token.MIN_USER_CHANNEL_VALUE - 1;

    /**
	 * Map channel like {@code COMMENTS_CHANNEL} to its constant channel value.
	 * Only user-defined channels are defined in this map.
	 */
    public readonly Dictionary<string, int> channelNameToValueMap = new();

    /**
	 * Map a constant channel value to its name. Indexed with raw channel value.
	 * The predefined channels {@link Token#DEFAULT_CHANNEL} and
	 * {@link Token#HIDDEN_CHANNEL} are not stored in this list, so the values
	 * at the corresponding indexes is {@code null}.
	 */
    public readonly List<string> channelValueToNameList = new();

    /** Map a name to an action.
     *  The code generator will use this to fill holes in the output files.
     *  I track the AST node for the action in case I need the line number
     *  for errors.
     */
    public Dictionary<string, ActionAST> namedActions = new();

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

    public static readonly string AUTO_GENERATED_TOKEN_NAME_PREFIX = "T__";

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
        this.name = (ast.GetChild(0)).Text;
        this.tokenStream = ast.tokenStream;
        this.originalTokenStream = this.tokenStream;

        InitTokenSymbolTables();
    }

    /** For testing */
    public Grammar(string grammarText) : this(GRAMMAR_FROM_STRING_NAME, grammarText, null)
    {
    }

    public Grammar(string grammarText, LexerGrammar tokenVocabSource)
       : this(GRAMMAR_FROM_STRING_NAME, grammarText, tokenVocabSource, null)
    {
    }

    /** For testing */
    public Grammar(string grammarText, ANTLRToolListener listener)
        : this(GRAMMAR_FROM_STRING_NAME, grammarText, listener)
    {
    }

    /** For testing; builds trees, does sem anal */
    public Grammar(string fileName, string grammarText)
        : this(fileName, grammarText, null)
    {

    }

    /** For testing; builds trees, does sem anal */
    public Grammar(string fileName, string grammarText, ANTLRToolListener listener)
        : this(fileName, grammarText, null, listener)
    {

    }
    public class ATL : ANTLRToolListener
    {
        public void Info(string msg) { }

        public void Error(ANTLRMessage msg) { }

        public void Warning(ANTLRMessage msg) { }

    }

    public class TVA : TreeVisitorAction
    {
        readonly Grammar thiz;
        public TVA(Grammar thiz) => this.thiz = thiz;

        public object Pre(object t) { ((GrammarAST)t).g = thiz; return t; }
        public object Post(object t) { return t; }
    }
    /** For testing; builds trees, does sem anal */
    public Grammar(string fileName, string grammarText, Grammar tokenVocabSource, ANTLRToolListener listener)

    {
        this.text = grammarText;
        this.fileName = fileName;
        this.Tools = new Tool();
        var hush = new ATL();
        Tools.addListener(hush); // we want to hush errors/warnings
        this.Tools.addListener(listener);
        org.antlr.runtime.ANTLRStringStream @in = new(grammarText)
        {
            name = fileName
        };

        this.ast = Tools.Parse(fileName, @in);
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
        var thiz = this;
        var v = new TreeVisitor(new GrammarASTAdaptor());
        v.visit(ast, new TVA(thiz));
        InitTokenSymbolTables();

        if (tokenVocabSource != null)
        {
            ImportVocab(tokenVocabSource);
        }

        Tools.Process(this, false);
    }

    protected void InitTokenSymbolTables()
    {
        tokenNameToTypeMap.Add("EOF", Token.EOF);

        // reserve a spot for the INVALID token
        typeToTokenList.Add(null);
    }


    public void LoadImportedGrammars()
    {
        this.LoadImportedGrammars(new());
    }

    private void LoadImportedGrammars(HashSet<string> visited)
    {
        if (ast == null) return;
        var i = (GrammarAST)ast.GetFirstChildWithType(ANTLRParser.IMPORT);
        if (i == null) return;
        visited.Add(this.name);
        importedGrammars = new();
        foreach (var c in i.GetChildren())
        {
            var t = (GrammarAST)c;
            string importedGrammarName = null;
            if (t.Type == ANTLRParser.ASSIGN)
            {
                t = (GrammarAST)t.GetChild(1);
                importedGrammarName = t.Text;
            }
            else if (t.Type == ANTLRParser.ID)
            {
                importedGrammarName = t.Text;
            }
            if (visited.Contains(importedGrammarName))
            { // ignore circular refs
                continue;
            }
            Grammar g;
            try
            {
                g = Tools.LoadImportedGrammar(this, t);
            }
            catch (IOException ioe)
            {
                Tools.ErrMgr.GrammarError(ErrorType.ERROR_READING_IMPORTED_GRAMMAR,
                                         importedGrammarName,
                                         t.                                         Token,
                                         importedGrammarName,
                                         name);
                continue;
            }
            // did it come back as error node or missing?
            if (g == null) continue;
            g.parent = this;
            importedGrammars.Add(g);
            g.LoadImportedGrammars(visited); // recursively pursue any imports in this import
        }
    }

    public void DefineAction(GrammarAST atAST)
    {
        if (atAST.ChildCount == 2)
        {
            var name = atAST.GetChild(0).Text;
            namedActions[name] = (ActionAST)atAST.GetChild(1);
        }
        else
        {
            var scope = atAST.GetChild(0).Text;
            var gtype = GetTypeString();
            if (scope.Equals(gtype) || (scope.Equals("parser") && gtype.Equals("combined")))
            {
                var name = atAST.GetChild(1).Text;
                namedActions[name] = (ActionAST)atAST.GetChild(2);
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
    public virtual bool DefineRule(Rule r)
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
    public virtual bool UndefineRule(Rule r)
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

    public Rule GetRule(string name)
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

    public ATN GetATN()
    {
        if (atn == null)
        {
            ParserATNFactory factory = new ParserATNFactory(this);
            atn = factory.CreateATN();
        }
        return atn;
    }

    public Rule GetRule(int index) => indexToRule[(index)];

    public Rule GetRule(string grammarName, string ruleName)
    {
        if (grammarName != null)
        { // scope override
            var g = GetImportedGrammar(grammarName);
            if (g == null)
            {
                return null;
            }
            return g.rules.TryGetValue(ruleName,out var r)?r:null;
        }
        return GetRule(ruleName);
    }

    /** Get list of all imports from all grammars in the delegate subtree of g.
     *  The grammars are in import tree preorder.  Don't include ourselves
     *  in list as we're not a delegate of ourselves.
     */
    public List<Grammar> GetAllImportedGrammars()
    {
        if (importedGrammars == null)
        {
            return null;
        }

        Dictionary<string, Grammar> delegates = new();
        foreach (var d in importedGrammars)
        {
            delegates[d.fileName] = d;
            var ds = d.GetAllImportedGrammars();
            if (ds != null)
            {
                foreach (var imported in ds)
                {
                    delegates[imported.fileName]= imported;
                }
            }
        }

        return new(delegates.Values);
    }

    public List<Grammar> GetImportedGrammars() => importedGrammars;

    public LexerGrammar GetImplicitLexer() => implicitLexer;

    /** convenience method for Tool.loadGrammar() */
    public static Grammar Load(string fileName)
    {
        var antlr = new Tool();
        return antlr.LoadGrammar(fileName);
    }

    /** Return list of imported grammars from root down to our parent.
     *  Order is [root, ..., this.parent].  (us not included).
     */
    public List<Grammar> GetGrammarAncestors()
    {
        var root = GetOutermostGrammar();
        if (this == root) return null;
        List<Grammar> grammars = new();
        // walk backwards to root, collecting grammars
        var p = this.parent;
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
    public Grammar GetOutermostGrammar() => parent == null ? this : parent.GetOutermostGrammar();

    /** Get the name of the generated recognizer; may or may not be same
     *  as grammar name.
     *  Recognizer is TParser and TLexer from T if combined, else
     *  just use T regardless of grammar type.
     */
    public string GetRecognizerName()
    {
        var suffix = "";
        var grammarsFromRootToMe = GetOutermostGrammar().GetGrammarAncestors();
        var qualifiedName = name;
        if (grammarsFromRootToMe != null)
        {
            var buffer = new StringBuilder();
            foreach (var g in grammarsFromRootToMe)
            {
                buffer.Append(g.name);
                buffer.Append('_');
            }
            buffer.Append(name);
            qualifiedName = buffer.ToString();
        }

        if (IsCombined || (IsLexer && implicitLexer != null))
        {
            suffix = Grammar.GetGrammarTypeToFileNameSuffix(Type);
        }
        return qualifiedName + suffix;
    }

    public string GetStringLiteralLexerRuleName(string lit)
    {
        return AUTO_GENERATED_TOKEN_NAME_PREFIX + stringLiteralRuleNumber++;
    }

    /** Return grammar directly imported by this grammar */
    public Grammar GetImportedGrammar(string name)
    {
        foreach (Grammar g in importedGrammars)
        {
            if (g.name.Equals(name)) return g;
        }
        return null;
    }

    public int GetTokenType(string token)
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

    public string GetTokenName(string literal)
    {
        var grammar = this;
        while (grammar != null)
        {
            if (grammar.stringLiteralToTypeMap.TryGetValue(literal,out var v))
                return grammar.GetTokenName(v);
            grammar = grammar.parent;
        }
        return null;
    }

    /** Given a token type, get a meaningful name for it such as the ID
	 *  or string literal.  If this is a lexer and the ttype is in the
	 *  char vocabulary, compute an ANTLR-valid (possibly escaped) char literal.
	 */
    public string GetTokenDisplayName(int ttype)
    {
        // inside any target's char range and is lexer grammar?
        if (IsLexer &&
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

        return ttype.ToString();// string.valueOf(ttype);
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

    public string GetTokenName(int ttype)
    {
        // inside any target's char range and is lexer grammar?
        if (IsLexer &&
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
    public int GetChannelValue(string channel)
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
    public string[] GetRuleNames()
    {
        var result = new string[rules.Count];
        Array.Fill(result, INVALID_RULE_NAME);
        foreach (var rule in rules.Values)
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
    public string[] GetTokenNames()
    {
        int numTokens = GetMaxTokenType();
        var tokenNames = new string[numTokens + 1];
        for (int i = 0; i < tokenNames.Length; i++)
        {
            tokenNames[i] = GetTokenName(i);
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
    public string[] GetTokenDisplayNames()
    {
        int numTokens = GetMaxTokenType();
        var tokenNames = new string[numTokens + 1];
        for (int i = 0; i < tokenNames.Length; i++)
        {
            tokenNames[i] = GetTokenDisplayName(i);
        }

        return tokenNames;
    }

    /**
	 * Gets the literal names assigned to tokens in the grammar.
	 */

    public string[] GetTokenLiteralNames()
    {
        int numTokens = GetMaxTokenType();
        var literalNames = new string[numTokens + 1];
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

    public string[] GetTokenSymbolicNames()
    {
        int numTokens = GetMaxTokenType();
        var symbolicNames = new string[numTokens + 1];
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

    public Vocabulary Vocabulary => new VocabularyImpl(GetTokenLiteralNames(), GetTokenSymbolicNames());

    /** Given an arbitrarily complex SemanticContext, walk the "tree" and get display string.
	 *  Pull predicates from grammar text.
	 */
    public string GetSemanticContextDisplayString(SemanticContext semctx)
    {
        if (semctx is SemanticContext.Predicate predicate)
        {
            return GetPredicateDisplayString(predicate);
        }
        if (semctx is SemanticContext.AND and)
        {
            return JoinPredicateOperands(and, " and ");
        }
        if (semctx is SemanticContext.OR or)
        {
            return JoinPredicateOperands(or, " or ");
        }
        return semctx.ToString();
    }

    public string JoinPredicateOperands(SemanticContext.Operator op, string separator)
    {
        var buffer = new StringBuilder();
        foreach (SemanticContext operand in op.GetOperands())
        {
            if (buffer.Length > 0)
            {
                buffer.Append(separator);
            }

            buffer.Append(GetSemanticContextDisplayString(operand));
        }

        return buffer.ToString();
    }

    public Dictionary<int, PredAST> GetIndexToPredicateMap()
    {
        Dictionary<int, PredAST> indexToPredMap = new();
        foreach (var r in rules.Values)
        {
            foreach (var a in r.actions)
            {
                if (a is PredAST p)
                {
                    if (sempreds.TryGetValue(p, out var r2))
                    {
                        indexToPredMap[r2] = p;
                    }
                }
            }
        }
        return indexToPredMap;
    }

    public string GetPredicateDisplayString(SemanticContext.Predicate pred)
    {
        indexToPredMap ??= GetIndexToPredicateMap();
        var actionAST = indexToPredMap.TryGetValue(pred.predIndex,out var r)?r:null;
        return actionAST?.Text;
    }

    /** What is the max char value possible for this grammar's target?  Use
	 *  unicode max if no target defined.
	 */
    public static int MaxCharValue => org.antlr.v4.runtime.Lexer.MAX_CHAR_VALUE;
    //		if ( generator!=null ) {//			return generator.getTarget().getMaxCharValue(generator);//		}//		else {//			return Label.MAX_CHAR_VALUE;//		}
    /** Return a set of all possible token or char types for this grammar */
    public IntSet GetTokenTypes()
    {
        if (IsLexer)
        {
            return GetAllCharValues();
        }
        return IntervalSet.Of(Token.MIN_USER_TOKEN_TYPE, GetMaxTokenType());
    }

    /** Return min to max char as defined by the target.
	 *  If no target, use max unicode char value.
	 */
    public static IntSet GetAllCharValues()
    {
        return IntervalSet.Of(Lexer.MIN_CHAR_VALUE, MaxCharValue);
    }

    /** How many token types have been allocated so far? */
    public int GetMaxTokenType()
    {
        return typeToTokenList.Count - 1; // don't count 0 (invalid)
    }

    /** Return a new unique integer in the token type space */
    public int GetNewTokenType()
    {
        maxTokenType++;
        return maxTokenType;
    }

    /** Return a new unique integer in the channel value space. */
    public int GetNewChannelNumber()
    {
        maxChannelType++;
        return maxChannelType;
    }

    public void ImportTokensFromTokensFile()
    {
        var vocab = GetOptionString("tokenVocab");
        if (vocab != null)
        {
            var vparser = new TokenVocabParser(this);
            var tokens = vparser.Load();
            Tools.Log("grammar", "tokens=" + tokens);
            foreach (var t in tokens.Keys)
            {
                if(tokens.TryGetValue(t, out var ret))
                {
                    if (t[0] == '\'') DefineStringLiteral(t, ret);
                    else DefineTokenName(t, ret);
                }
            }
        }
    }

    public void ImportVocab(Grammar importG)
    {
        foreach (var tokenName in importG.tokenNameToTypeMap.Keys)
        {
            DefineTokenName(tokenName, importG.tokenNameToTypeMap[(tokenName)]);
        }
        foreach (var tokenName in importG.stringLiteralToTypeMap.Keys)
        {
            DefineStringLiteral(tokenName, importG.stringLiteralToTypeMap[(tokenName)]);
        }
        foreach (var channel in importG.channelNameToValueMap)
        {
            DefineChannelName(channel.Key, channel.Value);
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

    public int DefineTokenName(string name)
    {
        if (!tokenNameToTypeMap.TryGetValue(name,out var prev))
            return DefineTokenName(name, GetNewTokenType());
        return prev;
    }

    public int DefineTokenName(string name, int ttype)
    {
        if (tokenNameToTypeMap.TryGetValue(name,out var prev)) return prev;
        tokenNameToTypeMap[name]= ttype;
        SetTokenForType(ttype, name);
        maxTokenType = Math.Max(maxTokenType, ttype);
        return ttype;
    }

    public int DefineStringLiteral(string lit)
    {
        return stringLiteralToTypeMap.TryGetValue(lit, out int value) ? value : DefineStringLiteral(lit, GetNewTokenType());
    }

    public int DefineStringLiteral(string lit, int ttype)
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

            SetTokenForType(ttype, lit);
            return ttype;
        }
        return Token.INVALID_TYPE;
    }

    public int DefineTokenAlias(string name, string lit)
    {
        int ttype = DefineTokenName(name);
        stringLiteralToTypeMap[lit] = ttype;
        SetTokenForType(ttype, name);
        return ttype;
    }

    public void SetTokenForType(int ttype, string text)
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
        var prevToken = typeToTokenList[ttype];
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
    public int DefineChannelName(string name)
    {
        return !channelNameToValueMap.TryGetValue(name,out var prev) ? DefineChannelName(name, GetNewChannelNumber()) : prev;
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
    public int DefineChannelName(string name, int value)
    {
        if (channelNameToValueMap.TryGetValue(name,out var prev))
        {
            return prev;
        }

        channelNameToValueMap[name] = value;
        SetChannelNameForValue(value, name);
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
    public void SetChannelNameForValue(int channelValue, string name)
    {
        if (channelValue >= channelValueToNameList.Count)
        {
            Utils.SetSize(channelValueToNameList, channelValue + 1);
        }

        var prevChannel = channelValueToNameList[channelValue];
        if (prevChannel == null)
        {
            channelValueToNameList[channelValue]= name;
        }
    }

    // no isolated attr at grammar action level
    //@Override
    public Attribute ResolveToAttribute(string x, ActionAST node)
    {
        return null;
    }

    // no $x.y makes sense here
    //@Override
    public Attribute ResolveToAttribute(string x, string y, ActionAST node)
    {
        return null;
    }

    //@Override
    public bool ResolvesToLabel(string x, ActionAST node) { return false; }

    //@Override
    public bool ResolvesToListLabel(string x, ActionAST node) { return false; }

    //@Override
    public bool ResolvesToToken(string x, ActionAST node) { return false; }

    //@Override
    public bool ResolvesToAttributeDict(string x, ActionAST node)
    {
        return false;
    }

    /** Given a grammar type, what should be the default action scope?
     *  If I say @members in a COMBINED grammar, for example, the
     *  default scope should be "parser".
     */
    public string GetDefaultActionScope() => this.Type switch
    {
        ANTLRParser.LEXER => "lexer",
        ANTLRParser.PARSER or ANTLRParser.COMBINED => "parser",
        _ => null,
    };

    public int Type
    {
        get
        {
            if (ast != null) return ast.grammarType;
            return 0;
        }
    }

    public TokenStream TokenStream
    {
        get
        {
            if (ast != null) return ast.tokenStream;
            return null;
        }
    }

    public bool IsLexer => Type == ANTLRParser.LEXER;
    public bool IsParser => Type == ANTLRParser.PARSER;
    public bool IsCombined => Type == ANTLRParser.COMBINED;
    /** Is id a valid token name? Does id start with an uppercase letter? */
    public static bool IsTokenName(string id) => id.Length > 0 && char.IsUpper(id[(0)]);

    public string GetTypeString()
    {
        if (ast == null) return null;
        return ANTLRParser.tokenNames[Type].ToLower();
    }

    public static string GetGrammarTypeToFileNameSuffix(int type) => type switch
    {
        ANTLRParser.LEXER => "Lexer",
        ANTLRParser.PARSER => "Parser",
        // if combined grammar, gen Parser and Lexer will be done later
        // TODO: we are separate now right?
        ANTLRParser.COMBINED => "Parser",
        _ => "<invalid>",
    };

    public string Language => GetOptionString("language");

    public string GetOptionString(string key) { return ast.GetOptionString(key); }

    /** Given ^(TOKEN_REF ^(OPTIONS ^(ELEMENT_OPTIONS (= assoc right))))
	 *  set option assoc=right in TOKEN_REF.
	 */
    public static void SetNodeOptions(GrammarAST node, GrammarAST options)
    {
        if (options == null) return;
        var t = (GrammarASTWithOptions)node;
        if (t.ChildCount == 0 || options.ChildCount == 0) return;
        foreach (var o in options.GetChildren())
        {
            var c = (GrammarAST)o;
            if (c.Type == ANTLRParser.ASSIGN)
            {
                t.SetOption(c.GetChild(0).Text, (GrammarAST)c.GetChild(1));
            }
            else
            {
                t.SetOption(c.Text, null); // no arg such as ID<VarNodeType>
            }
        }
    }

    /** Return list of (TOKEN_NAME node, 'literal' node) pairs */
    public static List<Pair<GrammarAST, GrammarAST>> GetStringLiteralAliasesFromLexerRules(GrammarRootAST ast)
    {
        string[] patterns = {
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
        GrammarASTAdaptor adaptor = new GrammarASTAdaptor(ast.token.InputStream);
        TreeWizard wiz = new TreeWizard(adaptor, ANTLRParser.tokenNames);
        List<Pair<GrammarAST, GrammarAST>> lexerRuleToStringLiteral =
            new();

        List<GrammarAST> ruleNodes = ast.GetNodesWithType(ANTLRParser.RULE);
        if (ruleNodes == null || ruleNodes.Count == 0) return null;

        foreach (var r in ruleNodes)
        {
            //tool.log("grammar", r.toStringTree());
            //			Console.Out.WriteLine("chk: "+r.toStringTree());
            var name = r.GetChild(0);
            if (name.Type == ANTLRParser.TOKEN_REF)
            {
                // check rule against patterns
                bool isLitRule;
                foreach (string pattern in patterns)
                {
                    isLitRule =
                        DefAlias(r, pattern, wiz, lexerRuleToStringLiteral);
                    if (isLitRule) break;
                }
                //				if ( !isLitRule ) Console.Out.WriteLine("no pattern matched");
            }
        }
        return lexerRuleToStringLiteral;
    }

    protected static bool DefAlias(GrammarAST r, string pattern,
                                      TreeWizard wiz,
                                      List<Pair<GrammarAST, GrammarAST>> lexerRuleToStringLiteral)
    {
        Dictionary<string, object> nodes = new();
        if (wiz.Parse(r, pattern, nodes))
        {
            if(nodes.TryGetValue("lit",out var litNode) && litNode is GrammarAST ln 
                && nodes.TryGetValue("name",out var nameNode) && nameNode is GrammarAST nn)
            {
                var pair =
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
        public GTV(Grammar g) => this.g = g;

        
        public override void StringRef(TerminalAST @ref)
        {
            g.strings.Add(@ref.Text);
        }
        public override ErrorManager ErrorManager => g.Tools.ErrMgr;
    }
    readonly HashSet<string> strings = new();
    public HashSet<string> GetStringLiterals()
    {
        strings.Clear();
        GrammarTreeVisitor collector = new GTV(this);
        collector.VisitGrammar(ast);
        return strings;
    }

    public void SetLookaheadDFA(int decision, DFA lookaheadDFA)
    {
        decisionDFAs[decision]= lookaheadDFA;
    }

    public static Dictionary<int, Interval> GetStateToGrammarRegionMap(GrammarRootAST ast, IntervalSet grammarTokenTypes)
    {
        Dictionary<int, Interval> stateToGrammarRegionMap = new();
        if (ast == null) return stateToGrammarRegionMap;

        var nodes = ast.GetNodesWithType(grammarTokenTypes);
        foreach (var n in nodes)
        {
            if (n.atnState != null)
            {
                var tokenRegion = Interval.Of(n.TokenStartIndex, n.TokenStopIndex);
                Tree ruleNode = null;
                // RULEs, BLOCKs of transformed recursive rules point to original token interval
                switch (n.Type)
                {
                    case ANTLRParser.RULE:
                        ruleNode = n;
                        break;
                    case ANTLRParser.BLOCK:
                    case ANTLRParser.CLOSURE:
                        ruleNode = n.GetAncestor(ANTLRParser.RULE);
                        break;
                }
                if (ruleNode is RuleAST)
                {
                    string ruleName = ((RuleAST)ruleNode).RuleName;
                    Rule r = ast.g.GetRule(ruleName);
                    if (r is LeftRecursiveRule)
                    {
                        RuleAST originalAST = ((LeftRecursiveRule)r).OriginalAST;
                        tokenRegion = Interval.Of(originalAST.TokenStartIndex, originalAST.TokenStopIndex);
                    }
                }
                stateToGrammarRegionMap[n.atnState.stateNumber]= tokenRegion;
            }
        }
        return stateToGrammarRegionMap;
    }

    /** Given an ATN state number, return the token index range within the grammar from which that ATN state was derived. */
    public Interval GetStateToGrammarRegion(int atnStateNumber)
    {
        if (stateToGrammarRegionMap == null)
        {
            stateToGrammarRegionMap = GetStateToGrammarRegionMap(ast, null); // map all nodes with non-null atn state ptr
        }
        if (stateToGrammarRegionMap == null) return Interval.INVALID;

        return stateToGrammarRegionMap.TryGetValue(atnStateNumber,out var r)?r:null;
    }

    public LexerInterpreter CreateLexerInterpreter(CharStream input)
    {
        if (this.IsParser)
        {
            throw new IllegalStateException("A lexer interpreter can only be created for a lexer or combined grammar.");
        }

        if (this.IsCombined)
        {
            return implicitLexer.CreateLexerInterpreter(input);
        }

        List<string> allChannels = new()
        {
            "DEFAULT_TOKEN_CHANNEL",
            "HIDDEN"
        };
        allChannels.AddRange(channelValueToNameList);

        // must run ATN through serializer to set some state flags
        var serialized = ATNSerializer.GetSerialized(atn);
        var deserializedATN = new ATNDeserializer().Deserialize(serialized.ToArray());
        return new LexerInterpreter(
                fileName,
                Vocabulary,
                Arrays.AsList(GetRuleNames()),
                allChannels,
                ((LexerGrammar)this).modes.Keys,
                deserializedATN,
                input);
    }

    /** @since 4.5.1 */
    public GrammarParserInterpreter CreateGrammarParserInterpreter(TokenStream tokenStream)
    {
        if (this.IsLexer)
        {
            throw new IllegalStateException("A parser interpreter can only be created for a parser or combined grammar.");
        }
        // must run ATN through serializer to set some state flags
        var serialized = ATNSerializer.GetSerialized(atn);
        var deserializedATN = new ATNDeserializer().Deserialize(serialized.ToArray());

        return new GrammarParserInterpreter(this, deserializedATN, tokenStream);
    }

    public ParserInterpreter CreateParserInterpreter(TokenStream tokenStream)
    {
        if (this.IsLexer)
        {
            throw new IllegalStateException("A parser interpreter can only be created for a parser or combined grammar.");
        }

        // must run ATN through serializer to set some state flags
        var serialized = ATNSerializer.GetSerialized(atn);
        var deserializedATN = new ATNDeserializer().Deserialize(serialized.ToArray());

        return new ParserInterpreter(fileName, Vocabulary, Arrays.AsList(GetRuleNames()), deserializedATN, tokenStream);
    }
}
