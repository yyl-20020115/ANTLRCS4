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
public class BasicSemanticChecks : GrammarTreeVisitor {
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

	public BasicSemanticChecks(Grammar g, RuleCollector ruleCollector) {
		this.g = g;
		this.ruleCollector = ruleCollector;
		this.errMgr = g.Tools.ErrMgr;
	}

	//@Override
	public ErrorManager getErrorManager() { return errMgr; }

	public void process() {	visitGrammar(g.ast); }

	// Routines to route visitor traffic to the checking routines

	//@Override
	public void discoverGrammar(GrammarRootAST root, GrammarAST ID) {
		checkGrammarName(ID.token);
	}

	//@Override
	public void finishPrequels(GrammarAST firstPrequel) {
		if ( firstPrequel==null ) return;
		GrammarAST parent = (GrammarAST)firstPrequel.parent;
		List<GrammarAST> options = parent.getAllChildrenWithType(OPTIONS);
		List<GrammarAST> imports = parent.getAllChildrenWithType(IMPORT);
		List<GrammarAST> tokens = parent.getAllChildrenWithType(TOKENS_SPEC);
		checkNumPrequels(options, imports, tokens);
	}

	//@Override
	public void importGrammar(GrammarAST label, GrammarAST ID) {
		checkImport(ID.token);
	}

	//@Override
	public void discoverRules(GrammarAST rules) {
		checkNumRules(rules);
	}

	//@Override
	protected void enterMode(GrammarAST tree) {
		nonFragmentRuleCount = 0;
	}

	//@Override
	protected void exitMode(GrammarAST tree) {
		if (nonFragmentRuleCount == 0) {
			Token token = tree.getToken();
			String name = "?";
			if (tree.getChildCount() > 0) {
				name = tree.getChild(0).getText();
				if (name == null || name.Length==0) {
					name = "?";
				}

				token = ((GrammarAST)tree.getChild(0)).getToken();
			}

			g.Tools.ErrMgr.GrammarError(ErrorType.MODE_WITHOUT_RULES, g.fileName, token, name, g);
		}
	}

	//@Override
	public void modeDef(GrammarAST m, GrammarAST ID) {
		if ( !g.isLexer() ) {
			g.Tools.ErrMgr.GrammarError(ErrorType.MODE_NOT_IN_LEXER, g.fileName,
									   ID.token, ID.token.getText(), g);
		}
	}

	//@Override
	public void discoverRule(RuleAST rule, GrammarAST ID,
							 List<GrammarAST> modifiers,
							 ActionAST arg, ActionAST returns,
							 GrammarAST thrws, GrammarAST options,
							 ActionAST locals,
							 List<GrammarAST> actions, GrammarAST block)
	{
		// TODO: chk that all or no alts have "# label"
		checkInvalidRuleDef(ID.token);
	}

	//@Override
	public void discoverLexerRule(RuleAST rule, GrammarAST ID, List<GrammarAST> modifiers, GrammarAST options,
								  GrammarAST block)
	{
		checkInvalidRuleDef(ID.token);

		if (modifiers != null) {
			foreach (GrammarAST tree in modifiers) {
				if (tree.getType() == ANTLRParser.FRAGMENT) {
					inFragmentRule = true;
				}
			}
		}

		if (!inFragmentRule) {
			nonFragmentRuleCount++;
		}
	}

	//@Override
	protected void exitLexerRule(GrammarAST tree) {
		inFragmentRule = false;
	}

	//@Override
	public void ruleRef(GrammarAST @ref, ActionAST arg) {
		checkInvalidRuleRef(@ref.token);
	}

	//@Override
	public void grammarOption(GrammarAST ID, GrammarAST valueAST) {
		checkOptions(g.ast, ID.token, valueAST);
	}

	//@Override
	public void ruleOption(GrammarAST ID, GrammarAST valueAST) {
		checkOptions((GrammarAST)ID.getAncestor(RULE), ID.token, valueAST);
	}

	//@Override
	public void blockOption(GrammarAST ID, GrammarAST valueAST) {
		checkOptions((GrammarAST)ID.getAncestor(BLOCK), ID.token, valueAST);
	}

	//@Override
	public void defineToken(GrammarAST ID) {
		checkTokenDefinition(ID.token);
	}

	//@Override
	protected void enterChannelsSpec(GrammarAST tree) {
		ErrorType errorType = g.isParser()
				? ErrorType.CHANNELS_BLOCK_IN_PARSER_GRAMMAR
				: g.isCombined()
				? ErrorType.CHANNELS_BLOCK_IN_COMBINED_GRAMMAR
				: null;
		if (errorType != null) {
			g.Tools.ErrMgr.GrammarError(errorType, g.fileName, tree.token);
		}
	}

	//@Override
	public void defineChannel(GrammarAST ID) {
		checkChannelDefinition(ID.token);
	}

	//@Override
	public void elementOption(GrammarASTWithOptions elem, GrammarAST ID, GrammarAST valueAST) {
		checkElementOptions(elem, ID, valueAST);
	}

	//@Override
	public void finishRule(RuleAST rule, GrammarAST ID, GrammarAST block) {
		if ( rule.isLexerRule() ) return;
		BlockAST blk = (BlockAST)rule.getFirstChildWithType(BLOCK);
		int nalts = blk.getChildCount();
		GrammarAST idAST = (GrammarAST)rule.getChild(0);
		for (int i=0; i< nalts; i++) {
			AltAST altAST = (AltAST)blk.getChild(i);
			if ( altAST.altLabel!=null ) {
				String altLabel = altAST.altLabel.getText();
				// first check that label doesn't conflict with a rule
				// label X or x can't be rule x.
				if (ruleCollector.rules.TryGetValue(Utils.Decapitalize(altLabel),out var r)) {
					g.Tools.ErrMgr.GrammarError(ErrorType.ALT_LABEL_CONFLICTS_WITH_RULE,
											   g.fileName, altAST.altLabel.token,
											   altLabel,
											   r.name);
				}
				// Now verify that label X or x doesn't conflict with label
				// in another rule. altLabelToRuleName has both X and x mapped.
				if ( ruleCollector.altLabelToRuleName.TryGetValue(altLabel,out var prevRuleForLabel) && !prevRuleForLabel.Equals(rule.getRuleName()) ) {
					g.Tools.ErrMgr.GrammarError(ErrorType.ALT_LABEL_REDEF,
											   g.fileName, altAST.altLabel.token,
											   altLabel,
											   rule.getRuleName(),
											   prevRuleForLabel);
				}
			}
		}
        int numAltLabels = 0;
		if (ruleCollector.ruleToAltLabels.TryGetValue(rule.getRuleName(),out var altLabels)) numAltLabels = altLabels.Count;
		if ( numAltLabels>0 && nalts != numAltLabels ) {
			g.Tools.ErrMgr.GrammarError(ErrorType.RULE_WITH_TOO_FEW_ALT_LABELS,
									   g.fileName, idAST.token, rule.getRuleName());
		}
	}

	// Routines to do the actual work of checking issues with a grammar.
	// They are triggered by the visitor methods above.

	void checkGrammarName(Token nameToken) {
		String fullyQualifiedName = nameToken.getInputStream().GetSourceName();
		if (fullyQualifiedName == null) {
			// This wasn't read from a file.
			return;
		}

		string f = (fullyQualifiedName);
		String fileName = f;
		if ( g.originalGrammar!=null ) return; // don't warn about diff if this is implicit lexer
		if ( !Utils.StripFileExtension(fileName).Equals(nameToken.getText()) &&
		     !fileName.Equals(Grammar.GRAMMAR_FROM_STRING_NAME)) {
			g.Tools.ErrMgr.GrammarError(ErrorType.FILE_AND_GRAMMAR_NAME_DIFFER,
									   fileName, nameToken, nameToken.getText(), fileName);
		}
	}

	void checkNumRules(GrammarAST rulesNode) {
		if ( rulesNode.getChildCount()==0 ) {
			GrammarAST root = (GrammarAST)rulesNode.getParent();
			GrammarAST IDNode = (GrammarAST)root.getChild(0);
			g.Tools.ErrMgr.GrammarError(ErrorType.NO_RULES, g.fileName,
					null, IDNode.getText(), g);
		}
	}

	void checkNumPrequels(List<GrammarAST> options,
						  List<GrammarAST> imports,
						  List<GrammarAST> tokens)
	{
		List<Token> secondOptionTokens = new ();
		if ( options!=null && options.Count >1 ) {
			secondOptionTokens.Add(options[(1)].token);
		}
		if ( imports!=null && imports.Count >1 ) {
			secondOptionTokens.Add(imports[(1)].token);
		}
		if ( tokens!=null && tokens.Count>1 ) {
			secondOptionTokens.Add(tokens[(1)].token);
		}
		foreach (Token t in secondOptionTokens) {
			String fileName = t.getInputStream().GetSourceName();
			g.Tools.ErrMgr.GrammarError(ErrorType.REPEATED_PREQUEL,
									   fileName, t);
		}
	}

	void checkInvalidRuleDef(Token ruleID) {
		String fileName = null;
		if ( ruleID.getInputStream()!=null ) {
			fileName = ruleID.getInputStream().GetSourceName();
		}
		if ( g.isLexer() && char.IsLower(ruleID.getText()[(0)]) ) {
			g.Tools.ErrMgr.GrammarError(ErrorType.PARSER_RULES_NOT_ALLOWED,
									   fileName, ruleID, ruleID.getText());
		}
		if ( g.isParser() &&
			Grammar.isTokenName(ruleID.getText()) )
		{
			g.Tools.ErrMgr.GrammarError(ErrorType.LEXER_RULES_NOT_ALLOWED,
									   fileName, ruleID, ruleID.getText());
		}
	}

	void checkInvalidRuleRef(Token ruleID) {
		String fileName = ruleID.getInputStream().GetSourceName();
		if ( g.isLexer() && char.IsLower(ruleID.getText()[(0)]) ) {
			g.Tools.ErrMgr.GrammarError(ErrorType.PARSER_RULE_REF_IN_LEXER_RULE,
									   fileName, ruleID, ruleID.getText(), currentRuleName);
		}
	}

	void checkTokenDefinition(Token tokenID) {
		String fileName = tokenID.getInputStream().GetSourceName();
		if ( !Grammar.isTokenName(tokenID.getText()) ) {
			g.Tools.ErrMgr.GrammarError(ErrorType.TOKEN_NAMES_MUST_START_UPPER,
									   fileName,
									   tokenID,
									   tokenID.getText());
		}
	}

	void checkChannelDefinition(Token tokenID) {
	}

	//@Override
	protected void enterLexerElement(GrammarAST tree) {
	}

	//@Override
	protected void enterLexerCommand(GrammarAST tree) {
		checkElementIsOuterMostInSingleAlt(tree);

		if (inFragmentRule) {
			String fileName = tree.token.getInputStream().GetSourceName();
			String ruleName = currentRuleName;
			g.Tools.ErrMgr.GrammarError(ErrorType.FRAGMENT_ACTION_IGNORED, fileName, tree.token, ruleName);
		}
	}

	//@Override
	public void actionInAlt(ActionAST action) {
		if (inFragmentRule) {
			String fileName = action.token.getInputStream().GetSourceName();
			String ruleName = currentRuleName;
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
	protected void checkElementIsOuterMostInSingleAlt(GrammarAST tree) {
		CommonTree alt = tree.parent;
		CommonTree blk = alt.parent;
		bool outerMostAlt = blk.parent.getType() == RULE;
		Tree rule = tree.getAncestor(RULE);
		String fileName = tree.getToken().getInputStream().GetSourceName();
		if ( !outerMostAlt || blk.getChildCount()>1 )
		{
			ErrorType e = ErrorType.LEXER_COMMAND_PLACEMENT_ISSUE;
			g.Tools.ErrMgr.GrammarError(e,
									   fileName,
									   tree.getToken(),
									   rule.getChild(0).getText());

		}
	}

	//@Override
	public void label(GrammarAST op, GrammarAST ID, GrammarAST element) {
		switch (element.getType()) {
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
			String fileName = ID.token.getInputStream().GetSourceName();
			g.Tools.ErrMgr.GrammarError(ErrorType.LABEL_BLOCK_NOT_A_SET, fileName, ID.token, ID.getText());
			break;
		}
	}

	//@Override
	protected void enterTerminal(GrammarAST tree) {
		String text = tree.getText();
		if (text.Equals("''")) {
			g.Tools.ErrMgr.GrammarError(ErrorType.EMPTY_STRINGS_AND_SETS_NOT_ALLOWED, g.fileName, tree.token, "''");
		}
	}

	/** Check option is appropriate for grammar, rule, subrule */
	void checkOptions(GrammarAST parent, Token optionID, GrammarAST valueAST) {
		HashSet<String> optionsToCheck = null;
		int parentType = parent.getType();
		switch (parentType) {
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
		String optionName = optionID.getText();
		if (optionsToCheck != null && !optionsToCheck.Contains(optionName)) {
			g.Tools.ErrMgr.GrammarError(ErrorType.ILLEGAL_OPTION, g.fileName, optionID, optionName);
		}
		else {
			checkCaseInsensitiveOption(optionID, valueAST, parentType);
		}
	}

	private void checkCaseInsensitiveOption(Token optionID, GrammarAST valueAST, int parentType) {
		String optionName = optionID.getText();
		if (optionName.Equals(Grammar.caseInsensitiveOptionName)) {
			String valueText = valueAST.getText();
			if (valueText.Equals("true") || valueText.Equals("false")) {
				bool currentValue = bool.TryParse(valueText,out var ret1)&&ret1;
				if (parentType == ANTLRParser.GRAMMAR) {
					grammarCaseInsensitive = currentValue;
				}
				else {
					if (grammarCaseInsensitive == currentValue) {
						g.Tools.ErrMgr.GrammarError(ErrorType.REDUNDANT_CASE_INSENSITIVE_LEXER_RULE_OPTION,
								g.fileName, optionID, currentValue);
					}
				}
			}
			else {
				g.Tools.ErrMgr.GrammarError(ErrorType.ILLEGAL_OPTION_VALUE, g.fileName, valueAST.getToken(),
						optionName, valueText);
			}
		}
	}

	/** Check option is appropriate for elem; parent of ID is ELEMENT_OPTIONS */
	bool checkElementOptions(GrammarASTWithOptions elem,
								GrammarAST ID,
								GrammarAST valueAST)
	{
		if (checkAssocElementOption && ID != null && "assoc".Equals(ID.getText())) {
			if (elem.getType() != ANTLRParser.ALT) {
				Token optionID = ID.token;
				String fileName = optionID.getInputStream().GetSourceName();
				g.Tools.ErrMgr.GrammarError(ErrorType.UNRECOGNIZED_ASSOC_OPTION,
										   fileName,
										   optionID,
										   currentRuleName);
			}
		}

		if ( elem is RuleRefAST ) {
			return checkRuleRefOptions((RuleRefAST)elem, ID, valueAST);
		}
		if ( elem is TerminalAST ) {
			return checkTokenOptions((TerminalAST)elem, ID, valueAST);
		}
		if ( elem.getType()==ANTLRParser.ACTION ) {
			return false;
		}
		if ( elem.getType()==ANTLRParser.SEMPRED ) {
			Token optionID = ID.token;
			String fileName = optionID.getInputStream().GetSourceName();
			if ( valueAST!=null && !Grammar.semPredOptions.Contains(optionID.getText()) ) {
				g.Tools.ErrMgr.GrammarError(ErrorType.ILLEGAL_OPTION,
										   fileName,
										   optionID,
										   optionID.getText());
				return false;
			}
		}
		return false;
	}

	bool checkRuleRefOptions(RuleRefAST elem, GrammarAST ID, GrammarAST valueAST) {
		Token optionID = ID.token;
		String fileName = optionID.getInputStream().GetSourceName();
		// don't care about id<SimpleValue> options
		if ( valueAST!=null && !Grammar.ruleRefOptions.Contains(optionID.getText()) ) {
			g.Tools.ErrMgr.GrammarError(ErrorType.ILLEGAL_OPTION,
									   fileName,
									   optionID,
									   optionID.getText());
			return false;
		}
		// TODO: extra checks depending on rule kind?
		return true;
	}

	bool checkTokenOptions(TerminalAST elem, GrammarAST ID, GrammarAST valueAST) {
		Token optionID = ID.token;
		String fileName = optionID.getInputStream().GetSourceName();
		// don't care about ID<ASTNodeName> options
		if ( valueAST!=null && !Grammar.tokenOptions.Contains(optionID.getText()) ) {
			g.Tools.ErrMgr.GrammarError(ErrorType.ILLEGAL_OPTION,
									   fileName,
									   optionID,
									   optionID.getText());
			return false;
		}
		// TODO: extra checks depending on terminal kind?
		return true;
	}

	void checkImport(Token importID) {
		Grammar @delegate = g.getImportedGrammar(importID.getText());
		if ( @delegate==null ) return;
		if (validImportTypes .TryGetValue(@delegate.getType() ,out var validDelegators) && !validDelegators.Contains(g.getType()) ) {
			g.Tools.ErrMgr.GrammarError(ErrorType.INVALID_IMPORT,
									   g.fileName,
									   importID,
									   g, @delegate);
		}
		if ( g.isCombined() &&
			 (@delegate.name.Equals(g.name+Grammar.getGrammarTypeToFileNameSuffix(ANTLRParser.LEXER))||
			  @delegate.name.Equals(g.name+Grammar.getGrammarTypeToFileNameSuffix(ANTLRParser.PARSER))) )
		{
			g.Tools.ErrMgr.GrammarError(ErrorType.IMPORT_NAME_CLASH,
									   g.fileName,
									   importID,
									   g, @delegate);
		}
	}
}
