/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.runtime.tree;
using org.antlr.v4.misc;
using org.antlr.v4.parse;
using org.antlr.v4.runtime;
using org.antlr.v4.runtime.misc;
using org.antlr.v4.runtime.tree;
using org.antlr.v4.tool;
using org.antlr.v4.tool.ast;

namespace org.antlr.v4.semantics;



/** No side-effects except for setting options into the appropriate node.
 *  TODO:  make the side effects into a separate pass this
 *
 * Invokes check rules for these:
 *
 * FILE_AND_GRAMMAR_NAME_DIFFER
 * LEXER_RULES_NOT_ALLOWED
 * PARSER_RULES_NOT_ALLOWED
 * CANNOT_ALIAS_TOKENS
 * ARGS_ON_TOKEN_REF
 * ILLEGAL_OPTION
 * REWRITE_OR_OP_WITH_NO_OUTPUT_OPTION
 * NO_RULES
 * REWRITE_FOR_MULTI_ELEMENT_ALT
 * HETERO_ILLEGAL_IN_REWRITE_ALT
 * AST_OP_WITH_NON_AST_OUTPUT_OPTION
 * AST_OP_IN_ALT_WITH_REWRITE
 * CONFLICTING_OPTION_IN_TREE_FILTER
 * WILDCARD_AS_ROOT
 * INVALID_IMPORT
 * TOKEN_VOCAB_IN_DELEGATE
 * IMPORT_NAME_CLASH
 * REPEATED_PREQUEL
 * TOKEN_NAMES_MUST_START_UPPER
 */
public class BasicSemanticChecks : GrammarTreeVisitor
{
    /** Set of valid imports.  Maps delegate to set of delegator grammar types.
	 *  validDelegations.get(LEXER) gives list of the kinds of delegators
	 *  that can import lexers.
	 */
    public static MultiMap<int, int> validImportTypes = null;
    //TODO:
    //new MultiMap<int, int>() {
    //	{
    //		map(ANTLRParser.LEXER, ANTLRParser.LEXER);
    //		map(ANTLRParser.LEXER, ANTLRParser.COMBINED);

    //		map(ANTLRParser.PARSER, ANTLRParser.PARSER);
    //		map(ANTLRParser.PARSER, ANTLRParser.COMBINED);

    //		map(ANTLRParser.COMBINED, ANTLRParser.COMBINED);
    //	}
    //};

    public Grammar g;
    public RuleCollector ruleCollector;
    public ErrorManager errMgr;

    /**
	 * When this is {@code true}, the semantic checks will report
	 * {@link ErrorType#UNRECOGNIZED_ASSOC_OPTION} where appropriate. This may
	 * be set to {@code false} to disable this specific check.
	 *
	 * <p>The default value is {@code true}.</p>
	 */
    public bool checkAssocElementOption = true;

    /**
	 * This field is used for reporting the {@link ErrorType#MODE_WITHOUT_RULES}
	 * error when necessary.
	 */
    protected int nonFragmentRuleCount;

    /**
	 * This is {@code true} from the time {@link #discoverLexerRule} is called
	 * for a lexer rule with the {@code fragment} modifier until
	 * {@link #exitLexerRule} is called.
	 */
    private bool inFragmentRule;

    /**
	 * Value of caseInsensitive option (false if not defined)
	 */
    private bool grammarCaseInsensitive = false;

    public BasicSemanticChecks(Grammar g, RuleCollector ruleCollector)
    {
        this.g = g;
        this.ruleCollector = ruleCollector;
        this.errMgr = g.Tools.ErrMgr;
    }

    public override ErrorManager ErrorManager => errMgr;
    public void Process() => VisitGrammar(g.ast);

    // Routines to route visitor traffic to the checking routines

    public override void DiscoverGrammar(GrammarRootAST root, GrammarAST ID)
    {
        CheckGrammarName(ID.token);
    }

    public override void FinishPrequels(GrammarAST firstPrequel)
    {
        if (firstPrequel == null) return;
        var parent = (GrammarAST)firstPrequel.parent;
        var options = parent.getAllChildrenWithType(OPTIONS);
        var imports = parent.getAllChildrenWithType(IMPORT);
        var tokens = parent.getAllChildrenWithType(TOKENS_SPEC);
        CheckNumPrequels(options, imports, tokens);
    }

    public override void ImportGrammar(GrammarAST label, GrammarAST ID)
    {
        CheckImport(ID.token);
    }

    public override void DiscoverRules(GrammarAST rules)
    {
        CheckNumRules(rules);
    }

    protected override void EnterMode(GrammarAST tree)
    {
        nonFragmentRuleCount = 0;
    }

    protected override void ExitMode(GrammarAST tree)
    {
        if (nonFragmentRuleCount == 0)
        {
            var token = tree.Token;
            var name = "?";
            if (tree.ChildCount > 0)
            {
                name = tree.GetChild(0).Text;
                if (name == null || name.Length == 0)
                {
                    name = "?";
                }

                token = ((GrammarAST)tree.GetChild(0)).Token;
            }

            g.Tools.ErrMgr.GrammarError(ErrorType.MODE_WITHOUT_RULES, g.fileName, token, name, g);
        }
    }

    public override void ModeDef(GrammarAST m, GrammarAST ID)
    {
        if (!g.isLexer())
        {
            g.Tools.ErrMgr.GrammarError(ErrorType.MODE_NOT_IN_LEXER, g.fileName,
                                       ID.token, ID.token.Text, g);
        }
    }

    public override void DiscoverRule(RuleAST rule, GrammarAST ID,
                             List<GrammarAST> modifiers,
                             ActionAST arg, ActionAST returns,
                             GrammarAST thrws, GrammarAST options,
                             ActionAST locals,
                             List<GrammarAST> actions, GrammarAST block)
    {
        // TODO: chk that all or no alts have "# label"
        CheckInvalidRuleDef(ID.token);
    }

    public override void DiscoverLexerRule(RuleAST rule, GrammarAST ID, List<GrammarAST> modifiers, GrammarAST options,
                                  GrammarAST block)
    {
        CheckInvalidRuleDef(ID.token);

        if (modifiers != null)
        {
            foreach (GrammarAST tree in modifiers)
            {
                if (tree.getType() == ANTLRParser.FRAGMENT)
                {
                    inFragmentRule = true;
                }
            }
        }

        if (!inFragmentRule)
        {
            nonFragmentRuleCount++;
        }
    }

    protected override void ExitLexerRule(GrammarAST tree)
    {
        inFragmentRule = false;
    }

    public override void RuleRef(GrammarAST @ref, ActionAST arg)
    {
        CheckInvalidRuleRef(@ref.token);
    }

    public override void GrammarOption(GrammarAST ID, GrammarAST valueAST)
    {
        CheckOptions(g.ast, ID.token, valueAST);
    }

    public override void RuleOption(GrammarAST ID, GrammarAST valueAST)
    {
        CheckOptions((GrammarAST)ID.GetAncestor(RULE), ID.token, valueAST);
    }

    public override void BlockOption(GrammarAST ID, GrammarAST valueAST)
    {
        CheckOptions((GrammarAST)ID.GetAncestor(BLOCK), ID.token, valueAST);
    }

    public override void DefineToken(GrammarAST ID)
    {
        CheckTokenDefinition(ID.token);
    }

    protected override void EnterChannelsSpec(GrammarAST tree)
    {
        ErrorType errorType = g.isParser()
                ? ErrorType.CHANNELS_BLOCK_IN_PARSER_GRAMMAR
                : g.isCombined()
                ? ErrorType.CHANNELS_BLOCK_IN_COMBINED_GRAMMAR
                : null;
        if (errorType != null)
        {
            g.Tools.ErrMgr.GrammarError(errorType, g.fileName, tree.token);
        }
    }

    public override void DefineChannel(GrammarAST ID)
    {
        CheckChannelDefinition(ID.token);
    }

    public override void ElementOption(GrammarASTWithOptions elem, GrammarAST ID, GrammarAST valueAST)
    {
        CheckElementOptions(elem, ID, valueAST);
    }

    //@Override
    public override void FinishRule(RuleAST rule, GrammarAST ID, GrammarAST block)
    {
        if (rule.isLexerRule()) return;
        var blk = (BlockAST)rule.GetFirstChildWithType(BLOCK);
        int nalts = blk.ChildCount;
        var idAST = (GrammarAST)rule.GetChild(0);
        for (int i = 0; i < nalts; i++)
        {
            AltAST altAST = (AltAST)blk.GetChild(i);
            if (altAST.altLabel != null)
            {
                String altLabel = altAST.altLabel.getText();
                // first check that label doesn't conflict with a rule
                // label X or x can't be rule x.
                if (ruleCollector.rules.TryGetValue(Utils.Decapitalize(altLabel), out var r))
                {
                    g.Tools.ErrMgr.GrammarError(ErrorType.ALT_LABEL_CONFLICTS_WITH_RULE,
                                               g.fileName, altAST.altLabel.token,
                                               altLabel,
                                               r.name);
                }
                // Now verify that label X or x doesn't conflict with label
                // in another rule. altLabelToRuleName has both X and x mapped.
                if (ruleCollector.altLabelToRuleName.TryGetValue(altLabel, out var prevRuleForLabel) && !prevRuleForLabel.Equals(rule.getRuleName()))
                {
                    g.Tools.ErrMgr.GrammarError(ErrorType.ALT_LABEL_REDEF,
                                               g.fileName, altAST.altLabel.token,
                                               altLabel,
                                               rule.getRuleName(),
                                               prevRuleForLabel);
                }
            }
        }
        int numAltLabels = 0;
        if (ruleCollector.ruleToAltLabels.TryGetValue(rule.getRuleName(), out var altLabels)) numAltLabels = altLabels.Count;
        if (numAltLabels > 0 && nalts != numAltLabels)
        {
            g.Tools.ErrMgr.GrammarError(ErrorType.RULE_WITH_TOO_FEW_ALT_LABELS,
                                       g.fileName, idAST.token, rule.getRuleName());
        }
    }

    // Routines to do the actual work of checking issues with a grammar.
    // They are triggered by the visitor methods above.

    void CheckGrammarName(Token nameToken)
    {
        var fullyQualifiedName = nameToken.InputStream.SourceName;
        if (fullyQualifiedName == null)
        {
            // This wasn't read from a file.
            return;
        }

        var f = (fullyQualifiedName);
        var fileName = f;
        if (g.originalGrammar != null) return; // don't warn about diff if this is implicit lexer
        if (!Utils.StripFileExtension(fileName).Equals(nameToken.Text) &&
             !fileName.Equals(Grammar.GRAMMAR_FROM_STRING_NAME))
        {
            g.Tools.ErrMgr.GrammarError(ErrorType.FILE_AND_GRAMMAR_NAME_DIFFER,
                                       fileName, nameToken, nameToken.Text, fileName);
        }
    }

    void CheckNumRules(GrammarAST rulesNode)
    {
        if (rulesNode.ChildCount == 0)
        {
            var root = (GrammarAST)rulesNode.getParent();
            var IDNode = (GrammarAST)root.GetChild(0);
            g.Tools.ErrMgr.GrammarError(ErrorType.NO_RULES, g.fileName,
                    null, IDNode.getText(), g);
        }
    }

    void CheckNumPrequels(List<GrammarAST> options,
                          List<GrammarAST> imports,
                          List<GrammarAST> tokens)
    {
        List<Token> secondOptionTokens = new();
        if (options != null && options.Count > 1)
        {
            secondOptionTokens.Add(options[(1)].token);
        }
        if (imports != null && imports.Count > 1)
        {
            secondOptionTokens.Add(imports[(1)].token);
        }
        if (tokens != null && tokens.Count > 1)
        {
            secondOptionTokens.Add(tokens[(1)].token);
        }
        foreach (Token t in secondOptionTokens)
        {
            var fileName = t.InputStream.SourceName;
            g.Tools.ErrMgr.GrammarError(ErrorType.REPEATED_PREQUEL,
                                       fileName, t);
        }
    }

    void CheckInvalidRuleDef(Token ruleID)
    {
        string fileName = null;
        if (ruleID.InputStream != null)
        {
            fileName = ruleID.InputStream.SourceName;
        }
        if (g.isLexer() && char.IsLower(ruleID.Text[(0)]))
        {
            g.Tools.ErrMgr.GrammarError(ErrorType.PARSER_RULES_NOT_ALLOWED,
                                       fileName, ruleID, ruleID.Text);
        }
        if (g.isParser() &&
            Grammar.isTokenName(ruleID.Text))
        {
            g.Tools.ErrMgr.GrammarError(ErrorType.LEXER_RULES_NOT_ALLOWED,
                                       fileName, ruleID, ruleID.Text);
        }
    }

    void CheckInvalidRuleRef(Token ruleID)
    {
        var fileName = ruleID.InputStream.SourceName;
        if (g.isLexer() && char.IsLower(ruleID.Text[(0)]))
        {
            g.Tools.ErrMgr.GrammarError(ErrorType.PARSER_RULE_REF_IN_LEXER_RULE,
                                       fileName, ruleID, ruleID.Text, currentRuleName);
        }
    }

    void CheckTokenDefinition(Token tokenID)
    {
        var fileName = tokenID.InputStream.SourceName;
        if (!Grammar.isTokenName(tokenID.Text))
        {
            g.Tools.ErrMgr.GrammarError(ErrorType.TOKEN_NAMES_MUST_START_UPPER,
                                       fileName,
                                       tokenID,
                                       tokenID.Text);
        }
    }

    void CheckChannelDefinition(Token tokenID)
    {
    }

    protected override void EnterLexerElement(GrammarAST tree)
    {
    }

    protected override void EnterLexerCommand(GrammarAST tree)
    {
        CheckElementIsOuterMostInSingleAlt(tree);

        if (inFragmentRule)
        {
            var fileName = tree.token.InputStream.SourceName;
            var ruleName = currentRuleName;
            g.Tools.ErrMgr.GrammarError(ErrorType.FRAGMENT_ACTION_IGNORED, fileName, tree.token, ruleName);
        }
    }

    //@Override
    public override void ActionInAlt(ActionAST action)
    {
        if (inFragmentRule)
        {
            var fileName = action.token.InputStream.SourceName;
            var ruleName = currentRuleName;
            g.Tools.ErrMgr.GrammarError(ErrorType.FRAGMENT_ACTION_IGNORED, fileName, action.token, ruleName);
        }
    }

    /**
	 Make sure that action is last element in outer alt; here action,
	 a2, z, and zz are bad, but a3 is ok:
	 (RULE A (BLOCK (ALT {action} 'a')))
	 (RULE B (BLOCK (ALT (BLOCK (ALT {a2} 'x') (ALT 'y')) {a3})))
	 (RULE C (BLOCK (ALT 'd' {z}) (ALT 'e' {zz})))
	 */
    protected void CheckElementIsOuterMostInSingleAlt(GrammarAST tree)
    {
        var alt = tree.parent;
        var blk = alt.parent;
        var outerMostAlt = blk.parent.getType() == RULE;
        var rule = tree.GetAncestor(RULE);
        var fileName = tree.Token.InputStream.SourceName;
        if (!outerMostAlt || blk.ChildCount > 1)
        {
            ErrorType e = ErrorType.LEXER_COMMAND_PLACEMENT_ISSUE;
            g.Tools.ErrMgr.GrammarError(e,
                                       fileName,
                                       tree.Token,
                                       rule.GetChild(0).Text);

        }
    }

    public override void Label(GrammarAST op, GrammarAST ID, GrammarAST element)
    {
        switch (element.getType())
        {
            // token atoms
            case TOKEN_REF:
            case STRING_LITERAL:
            case RANGE:
            // token sets
            case SET:
            case NOT:
            // rule atoms
            case RULE_REF:
            case WILDCARD:
                return;

            default:
                String fileName = ID.token.InputStream.SourceName;
                g.Tools.ErrMgr.GrammarError(ErrorType.LABEL_BLOCK_NOT_A_SET, fileName, ID.token, ID.getText());
                break;
        }
    }

    protected override void EnterTerminal(GrammarAST tree)
    {
        var text = tree.getText();
        if (text.Equals("''"))
        {
            g.Tools.ErrMgr.GrammarError(ErrorType.EMPTY_STRINGS_AND_SETS_NOT_ALLOWED, g.fileName, tree.token, "''");
        }
    }

    /** Check option is appropriate for grammar, rule, subrule */
    void CheckOptions(GrammarAST parent, Token optionID, GrammarAST valueAST)
    {
        HashSet<string> optionsToCheck = null;
        int parentType = parent.getType();
        switch (parentType)
        {
            case ANTLRParser.BLOCK:
                optionsToCheck = g.isLexer() ? Grammar.lexerBlockOptions : Grammar.parserBlockOptions;
                break;
            case ANTLRParser.RULE:
                optionsToCheck = g.isLexer() ? Grammar.lexerRuleOptions : Grammar.parseRuleOptions;
                break;
            case ANTLRParser.GRAMMAR:
                optionsToCheck = g.getType() == ANTLRParser.LEXER
                        ? Grammar.lexerOptions
                        : Grammar.parserOptions;
                break;
        }
        var optionName = optionID.Text;
        if (optionsToCheck != null && !optionsToCheck.Contains(optionName))
        {
            g.Tools.ErrMgr.GrammarError(ErrorType.ILLEGAL_OPTION, g.fileName, optionID, optionName);
        }
        else
        {
            CheckCaseInsensitiveOption(optionID, valueAST, parentType);
        }
    }

    private void CheckCaseInsensitiveOption(Token optionID, GrammarAST valueAST, int parentType)
    {
        var optionName = optionID.Text;
        if (optionName.Equals(Grammar.caseInsensitiveOptionName))
        {
            var valueText = valueAST.getText();
            if (valueText.Equals("true") || valueText.Equals("false"))
            {
                var currentValue = bool.TryParse(valueText, out var ret1) && ret1;
                if (parentType == ANTLRParser.GRAMMAR)
                {
                    grammarCaseInsensitive = currentValue;
                }
                else
                {
                    if (grammarCaseInsensitive == currentValue)
                    {
                        g.Tools.ErrMgr.GrammarError(ErrorType.REDUNDANT_CASE_INSENSITIVE_LEXER_RULE_OPTION,
                                g.fileName, optionID, currentValue);
                    }
                }
            }
            else
            {
                g.Tools.ErrMgr.GrammarError(ErrorType.ILLEGAL_OPTION_VALUE, g.fileName, valueAST.Token,
                        optionName, valueText);
            }
        }
    }

    /** Check option is appropriate for elem; parent of ID is ELEMENT_OPTIONS */
    bool CheckElementOptions(GrammarASTWithOptions elem,
                                GrammarAST ID,
                                GrammarAST valueAST)
    {
        if (checkAssocElementOption && ID != null && "assoc".Equals(ID.getText()))
        {
            if (elem.getType() != ANTLRParser.ALT)
            {
                var optionID = ID.token;
                var fileName = optionID.InputStream.SourceName;
                g.Tools.ErrMgr.GrammarError(ErrorType.UNRECOGNIZED_ASSOC_OPTION,
                                           fileName,
                                           optionID,
                                           currentRuleName);
            }
        }

        if (elem is RuleRefAST aST1)
        {
            return CheckRuleRefOptions(aST1, ID, valueAST);
        }
        if (elem is TerminalAST aST)
        {
            return CheckTokenOptions(aST, ID, valueAST);
        }
        if (elem.getType() == ANTLRParser.ACTION)
        {
            return false;
        }
        if (elem.getType() == ANTLRParser.SEMPRED)
        {
            var optionID = ID.token;
            var fileName = optionID.InputStream.SourceName;
            if (valueAST != null && !Grammar.semPredOptions.Contains(optionID.Text))
            {
                g.Tools.ErrMgr.GrammarError(ErrorType.ILLEGAL_OPTION,
                                           fileName,
                                           optionID,
                                           optionID.Text);
                return false;
            }
        }
        return false;
    }

    bool CheckRuleRefOptions(RuleRefAST elem, GrammarAST ID, GrammarAST valueAST)
    {
        var optionID = ID.token;
        var fileName = optionID.InputStream.SourceName;
        // don't care about id<SimpleValue> options
        if (valueAST != null && !Grammar.ruleRefOptions.Contains(optionID.Text))
        {
            g.Tools.ErrMgr.GrammarError(ErrorType.ILLEGAL_OPTION,
                                       fileName,
                                       optionID,
                                       optionID.Text);
            return false;
        }
        // TODO: extra checks depending on rule kind?
        return true;
    }

    bool CheckTokenOptions(TerminalAST elem, GrammarAST ID, GrammarAST valueAST)
    {
        var optionID = ID.token;
        var fileName = optionID.InputStream.SourceName;
        // don't care about ID<ASTNodeName> options
        if (valueAST != null && !Grammar.tokenOptions.Contains(optionID.Text))
        {
            g.Tools.ErrMgr.GrammarError(ErrorType.ILLEGAL_OPTION,
                                       fileName,
                                       optionID,
                                       optionID.Text);
            return false;
        }
        // TODO: extra checks depending on terminal kind?
        return true;
    }

    void CheckImport(Token importID)
    {
        var @delegate = g.getImportedGrammar(importID.Text);
        if (@delegate == null) return;
        if (validImportTypes.TryGetValue(@delegate.getType(), out var validDelegators) && !validDelegators.Contains(g.getType()))
        {
            g.Tools.ErrMgr.GrammarError(ErrorType.INVALID_IMPORT,
                                       g.fileName,
                                       importID,
                                       g, @delegate);
        }
        if (g.isCombined() &&
             (@delegate.name.Equals(g.name + Grammar.getGrammarTypeToFileNameSuffix(ANTLRParser.LEXER)) ||
              @delegate.name.Equals(g.name + Grammar.getGrammarTypeToFileNameSuffix(ANTLRParser.PARSER))))
        {
            g.Tools.ErrMgr.GrammarError(ErrorType.IMPORT_NAME_CLASH,
                                       g.fileName,
                                       importID,
                                       g, @delegate);
        }
    }
}
