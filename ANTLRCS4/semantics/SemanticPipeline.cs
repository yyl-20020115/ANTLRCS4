/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.analysis;
using org.antlr.v4.automata;
using org.antlr.v4.parse;
using org.antlr.v4.runtime;
using org.antlr.v4.runtime.misc;
using org.antlr.v4.tool;
using org.antlr.v4.tool.ast;

namespace org.antlr.v4.semantics;

/** Do as much semantic checking as we can and fill in grammar
 *  with rules, actions, and token definitions.
 *  The only side effects are in the grammar passed to process().
 *  We consume a bunch of memory here while we build up data structures
 *  to perform checking, but all of it goes away after this pipeline object
 *  gets garbage collected.
 *
 *  After this pipeline finishes, we can be sure that the grammar
 *  is syntactically correct and that it's semantically correct enough for us
 *  to attempt grammar analysis. We have assigned all token types.
 *  Note that imported grammars bring in token and rule definitions
 *  but only the root grammar and any implicitly created lexer grammar
 *  get their token definitions filled up. We are treating the
 *  imported grammars like includes.
 *
 *  The semantic pipeline works on root grammars (those that do the importing,
 *  if any). Upon entry to the semantic pipeline, all imported grammars
 *  should have been loaded into delegate grammar objects with their
 *  ASTs created.  The pipeline does the BasicSemanticChecks on the
 *  imported grammar before collecting symbols. We cannot perform the
 *  simple checks such as undefined rule until we have collected all
 *  tokens and rules from the imported grammars into a single collection.
 */
public class SemanticPipeline
{
    public readonly Grammar g;

    public SemanticPipeline(Grammar g)
    {
        this.g = g;
    }

    public void Process()
    {
        if (g.ast == null) return;

        // COLLECT RULE OBJECTS
        var ruleCollector = new RuleCollector(g);
        ruleCollector.Process(g.ast);

        // DO BASIC / EASY SEMANTIC CHECKS
        int prevErrors = g.Tools.ErrMgr.NumErrors;
        var basics = new BasicSemanticChecks(g, ruleCollector);
        basics.Process();
        if (g.Tools.ErrMgr.NumErrors > prevErrors) return;

        // TRANSFORM LEFT-RECURSIVE RULES
        prevErrors = g.Tools.ErrMgr.NumErrors;
        var lrtrans =
            new LeftRecursiveRuleTransformer(g.ast, ruleCollector.rules.Values, g);
        lrtrans.TranslateLeftRecursiveRules();

        // don't continue if we got errors during left-recursion elimination
        if (g.Tools.ErrMgr.NumErrors > prevErrors) return;

        // STORE RULES IN GRAMMAR
        foreach (var r in ruleCollector.rules.Values)
        {
            g.DefineRule(r);
        }

        // COLLECT SYMBOLS: RULES, ACTIONS, TERMINALS, ...
        var collector = new SymbolCollector(g);
        collector.Process(g.ast);

        // CHECK FOR SYMBOL COLLISIONS
        var symcheck = new SymbolChecks(g, collector);
        symcheck.Process(); // side-effect: strip away redef'd rules.

        foreach (var a in collector.namedActions)
        {
            g.DefineAction(a);
        }

        // LINK (outermost) ALT NODES WITH Alternatives
        foreach (var r in g.rules.Values)
        {
            for (int i = 1; i <= r.numberOfAlts; i++)
            {
                r.alt[i].ast.alt = r.alt[i];
            }
        }

        // ASSIGN TOKEN TYPES
        g.importTokensFromTokensFile();
        if (g.isLexer())
        {
            AssignLexerTokenTypes(g, collector.tokensDefs);
        }
        else
        {
            AssignTokenTypes(g, collector.tokensDefs,
                             collector.tokenIDRefs, collector.terminals);
        }

        symcheck.CheckForModeConflicts(g);
        symcheck.CheckForUnreachableTokens(g);

        AssignChannelTypes(g, collector.channelDefs);

        // CHECK RULE REFS NOW (that we've defined rules in grammar)
        symcheck.CheckRuleArgs(g, collector.rulerefs);
        IdentifyStartRules(collector);
        symcheck.CheckForQualifiedRuleIssues(g, collector.qualifiedRulerefs);

        // don't continue if we got symbol errors
        if (g.Tools.getNumErrors() > 0) return;

        // CHECK ATTRIBUTE EXPRESSIONS FOR SEMANTIC VALIDITY
        AttributeChecks.CheckAllAttributeExpressions(g);

        UseDefAnalyzer.trackTokenRuleRefsInActions(g);
    }

    void IdentifyStartRules(SymbolCollector collector)
    {
        foreach (var @ref in collector.rulerefs)
        {
            var ruleName = @ref.getText();
            var r = g.GetRule(ruleName);
            if (r != null) r.isStartRule = false;
        }
    }

    void AssignLexerTokenTypes(Grammar g, List<GrammarAST> tokensDefs)
    {
        var G = g.GetOutermostGrammar(); // put in root, even if imported
        foreach (var def in tokensDefs)
        {
            // tokens { id (',' id)* } so must check IDs not TOKEN_REF
            if (Grammar.isTokenName(def.getText()))
            {
                G.defineTokenName(def.getText());
            }
        }

        /* Define token types for nonfragment rules which do not include a 'type(...)'
		 * or 'more' lexer command.
		 */
        foreach (var r in g.rules.Values)
        {
            if (!r.IsFragment && !HasTypeOrMoreCommand(r))
            {
                G.defineTokenName(r.name);
            }
        }

        // FOR ALL X in 'xxx'; RULES, DEFINE 'xxx' AS TYPE X
        var litAliases =
            Grammar.getStringLiteralAliasesFromLexerRules(g.ast);
        var conflictingLiterals = new HashSet<string>();
        if (litAliases != null)
        {
            foreach (var pair in litAliases)
            {
                var nameAST = pair.a;
                var litAST = pair.b;
                if (!G.stringLiteralToTypeMap.ContainsKey(litAST.getText()))
                {
                    G.defineTokenAlias(nameAST.getText(), litAST.getText());
                }
                else
                {
                    // oops two literal defs in two rules (within or across modes).
                    conflictingLiterals.Add(litAST.getText());
                }
            }
            foreach (var lit in conflictingLiterals)
            {
                // Remove literal if repeated across rules so it's not
                // found by parser grammar.
                if (G.stringLiteralToTypeMap.TryGetValue(lit, out var value))
                {
                    G.stringLiteralToTypeMap.Remove(lit);
                    if (value > 0 && value < G.typeToStringLiteralList.Count && lit.Equals(G.typeToStringLiteralList[(value)]))
                    {
                        G.typeToStringLiteralList.Set(value, null);
                    }
                }
            }
        }
    }

    bool HasTypeOrMoreCommand(Rule r)
    {
        var ast = r.ast;
        if (ast == null)
        {
            return false;
        }

        var altActionAst = (GrammarAST)ast.getFirstDescendantWithType(ANTLRParser.LEXER_ALT_ACTION);
        if (altActionAst == null)
        {
            // the rule isn't followed by any commands
            return false;
        }

        // first child is the alt itself, subsequent are the actions
        for (int i = 1; i < altActionAst.ChildCount; i++)
        {
            var node = (GrammarAST)altActionAst.GetChild(i);
            if (node.getType() == ANTLRParser.LEXER_ACTION_CALL)
            {
                if ("type".Equals(node.GetChild(0).Text))
                {
                    return true;
                }
            }
            else if ("more".Equals(node.getText()))
            {
                return true;
            }
        }

        return false;
    }

    void AssignTokenTypes(Grammar g, List<GrammarAST> tokensDefs,
                          List<GrammarAST> tokenIDs, List<GrammarAST> terminals)
    {
        //Grammar G = g.getOutermostGrammar(); // put in root, even if imported

        // create token types for tokens { A, B, C } ALIASES
        foreach (var alias in tokensDefs)
        {
            if (g.getTokenType(alias.getText()) != Token.INVALID_TYPE)
            {
                g.Tools.ErrMgr.GrammarError(ErrorType.TOKEN_NAME_REASSIGNMENT, g.fileName, alias.token, alias.getText());
            }

            g.defineTokenName(alias.getText());
        }

        // DEFINE TOKEN TYPES FOR TOKEN REFS LIKE ID, INT
        foreach (var idAST in tokenIDs)
        {
            if (g.getTokenType(idAST.getText()) == Token.INVALID_TYPE)
            {
                g.Tools.ErrMgr.GrammarError(ErrorType.IMPLICIT_TOKEN_DEFINITION, g.fileName, idAST.token, idAST.getText());
            }

            g.defineTokenName(idAST.getText());
        }

        // VERIFY TOKEN TYPES FOR STRING LITERAL REFS LIKE 'while', ';'
        foreach (var termAST in terminals)
        {
            if (termAST.getType() != ANTLRParser.STRING_LITERAL)
            {
                continue;
            }

            if (g.getTokenType(termAST.getText()) == Token.INVALID_TYPE)
            {
                g.Tools.ErrMgr.GrammarError(ErrorType.IMPLICIT_STRING_DEFINITION, g.fileName, termAST.token, termAST.getText());
            }
        }

        g.Tools.Log("semantics", "tokens=" + g.tokenNameToTypeMap);
        g.Tools.Log("semantics", "strings=" + g.stringLiteralToTypeMap);
    }

    /**
	 * Assign constant values to custom channels defined in a grammar.
	 *
	 * @param g The grammar.
	 * @param channelDefs A collection of AST nodes defining individual channels
	 * within a {@code channels{}} block in the grammar.
	 */
    void AssignChannelTypes(Grammar g, List<GrammarAST> channelDefs)
    {
        var outermost = g.GetOutermostGrammar();
        foreach (var channel in channelDefs)
        {
            String channelName = channel.getText();

            // Channel names can't alias tokens or modes, because constant
            // values are also assigned to them and the ->channel(NAME) lexer
            // command does not distinguish between the various ways a constant
            // can be declared. This method does not verify that channels do not
            // alias rules, because rule names are not associated with constant
            // values in ANTLR grammar semantics.

            if (g.getTokenType(channelName) != Token.INVALID_TYPE)
            {
                g.Tools.ErrMgr.GrammarError(ErrorType.CHANNEL_CONFLICTS_WITH_TOKEN, g.fileName, channel.token, channelName);
            }

            if (LexerATNFactory.COMMON_CONSTANTS.ContainsKey(channelName))
            {
                g.Tools.ErrMgr.GrammarError(ErrorType.CHANNEL_CONFLICTS_WITH_COMMON_CONSTANTS, g.fileName, channel.token, channelName);
            }

            if (outermost is LexerGrammar lexerGrammar)
            {
                if (lexerGrammar.modes.ContainsKey(channelName))
                {
                    g.Tools.ErrMgr.GrammarError(ErrorType.CHANNEL_CONFLICTS_WITH_MODE, g.fileName, channel.token, channelName);
                }
            }

            outermost.defineChannelName(channel.getText());
        }
    }
}
