/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.runtime.tree;
using org.antlr.v4.automata;
using org.antlr.v4.parse;
using org.antlr.v4.runtime;
using org.antlr.v4.runtime.tree;
using org.antlr.v4.tool;
using org.antlr.v4.tool.ast;
using System.Text;

namespace org.antlr.v4.semantics;

/** Check for symbol problems; no side-effects.  Inefficient to walk rules
 *  and such multiple times, but I like isolating all error checking outside
 *  of code that actually defines symbols etc...
 *
 *  Side-effect: strip away redef'd rules.
 */
public class SymbolChecks {
	Grammar g;
	SymbolCollector collector;
	Dictionary<String, Rule> nameToRuleMap = new();
	HashSet<String> tokenIDs = new HashSet<String>();
	Dictionary<String, HashSet<String>> actionScopeToActionNames = new ();

	public ErrorManager errMgr;

	protected HashSet<String> reservedNames = new HashSet<String>(LexerATNFactory.GetCommonConstants());

	public SymbolChecks(Grammar g, SymbolCollector collector) {
		this.g = g;
		this.collector = collector;
		this.errMgr = g.Tools.ErrMgr;

		foreach (GrammarAST tokenId in collector.tokenIDRefs) {
			tokenIDs.Add(tokenId.getText());
		}
	}

	public void process() {
		// methods affect fields, but no side-effects outside this object
		// So, call order sensitive
		// First collect all rules for later use in checkForLabelConflict()
		if (g.rules != null) {
			foreach (Rule r in g.rules.Values) nameToRuleMap[r.name]= r;
		}
		checkReservedNames(g.rules.Values);
		checkActionRedefinitions(collector.namedActions);
		checkForLabelConflicts(g.rules.Values);
	}

	public void checkActionRedefinitions(List<GrammarAST> actions) {
		if (actions == null) return;
		String scope = g.getDefaultActionScope();
		String name;
		GrammarAST nameNode;
		foreach (GrammarAST ampersandAST in actions) {
			nameNode = (GrammarAST) ampersandAST.getChild(0);
			if (ampersandAST.getChildCount() == 2) {
				name = nameNode.getText();
			}
			else {
				scope = nameNode.getText();
				name = ampersandAST.getChild(1).getText();
			}
			if (!actionScopeToActionNames.TryGetValue(scope,out var scopeActions) ) { // init scope
				scopeActions = new HashSet<String>();
				actionScopeToActionNames[scope]= scopeActions;
			}
			if (!scopeActions.Contains(name)) {
				scopeActions.Add(name);
			}
			else {
				errMgr.GrammarError(ErrorType.ACTION_REDEFINITION,
						g.fileName, nameNode.token, name);
			}
		}
	}

	/**
	 * Make sure a label doesn't conflict with another symbol.
	 * Labels must not conflict with: rules, tokens, scope names,
	 * return values, parameters, and rule-scope dynamic attributes
	 * defined in surrounding rule.  Also they must have same type
	 * for repeated defs.
	 */
	public void checkForLabelConflicts(ICollection<Rule> rules) {
		foreach(Rule r in rules) {
			checkForAttributeConflicts(r);

			Dictionary<String, LabelElementPair> labelNameSpace = new ();
			for (int i = 1; i <= r.numberOfAlts; i++) {
				Alternative a = r.alt[i];
				foreach (List<LabelElementPair> pairs in a.labelDefs.Values) {
					if (r.hasAltSpecificContexts()) {
						// Collect labelName-labeledRules map for rule with alternative labels.
						Dictionary<String, List<LabelElementPair>> labelPairs = new ();
						foreach (LabelElementPair p in pairs) {
							String labelName = findAltLabelName(p.label);
							if (labelName != null) {
								if (!labelPairs.TryGetValue(labelName,out var list)) {
									list = new ();
									labelPairs[labelName] = list;
								}
								list.Add(p);
							}
						}

						foreach (List<LabelElementPair> internalPairs in labelPairs.Values) {
							labelNameSpace.Clear();
							checkLabelPairs(r, labelNameSpace, internalPairs);
						}
					}
					else {
						checkLabelPairs(r, labelNameSpace, pairs);
					}
				}
			}
		}
	}

	private void checkLabelPairs(Rule r, Dictionary<String, LabelElementPair> labelNameSpace, List<LabelElementPair> pairs) {
        foreach (LabelElementPair p in pairs) {
			checkForLabelConflict(r, p.label);
			String name = p.label.getText();
			if (!labelNameSpace.TryGetValue(name,out var prev)) {
				labelNameSpace[name] = p;
			}
			else {
				checkForTypeMismatch(r, prev, p);
			}
		}
	}

	private String findAltLabelName(CommonTree label) {
		if (label == null) {
			return null;
		}
		else if (label is AltAST) {
			AltAST altAST = (AltAST) label;
			if (altAST.altLabel != null) {
				return altAST.altLabel.toString();
			}
			else if (altAST.leftRecursiveAltInfo != null) {
				return altAST.leftRecursiveAltInfo.altLabel.ToString();
			}
			else {
				return findAltLabelName(label.parent);
			}
		}
		else {
			return findAltLabelName(label.parent);
		}
	}

	private void checkForTypeMismatch(Rule r, LabelElementPair prevLabelPair, LabelElementPair labelPair) {
		// label already defined; if same type, no problem
		if (prevLabelPair.type != labelPair.type) {
			// Current behavior: take a token of rule declaration in case of left-recursive rule
			// Desired behavior: take a token of proper label declaration in case of left-recursive rule
			// See https://github.com/antlr/antlr4/pull/1585
			// Such behavior is referring to the fact that the warning is typically reported on the actual label redefinition,
			//   but for left-recursive rules the warning is reported on the enclosing rule.
			Token token = r is LeftRecursiveRule
					? ((GrammarAST) r.ast.getChild(0)).getToken()
					: labelPair.label.token;
			errMgr.GrammarError(
					ErrorType.LABEL_TYPE_CONFLICT,
					g.fileName,
					token,
					labelPair.label.getText(),
					labelPair.type + "!=" + prevLabelPair.type);
		}
		if (!prevLabelPair.element.getText().Equals(labelPair.element.getText()) &&
			(prevLabelPair.type.Equals(LabelType.RULE_LABEL) || prevLabelPair.type.Equals(LabelType.RULE_LIST_LABEL)) &&
			(labelPair.type.Equals(LabelType.RULE_LABEL) || labelPair.type.Equals(LabelType.RULE_LIST_LABEL))) {

			Token token = r is LeftRecursiveRule
					? ((GrammarAST) r.ast.getChild(0)).getToken()
					: labelPair.label.token;
			String prevLabelOp = prevLabelPair.type.Equals(LabelType.RULE_LIST_LABEL) ? "+=" : "=";
			String labelOp = labelPair.type.Equals(LabelType.RULE_LIST_LABEL) ? "+=" : "=";
			errMgr.GrammarError(
					ErrorType.LABEL_TYPE_CONFLICT,
					g.fileName,
					token,
					labelPair.label.getText() + labelOp + labelPair.element.getText(),
					prevLabelPair.label.getText() + prevLabelOp + prevLabelPair.element.getText());
		}
	}

	public void checkForLabelConflict(Rule r, GrammarAST labelID) {
		String name = labelID.getText();
		if (nameToRuleMap.ContainsKey(name)) {
			ErrorType etype = ErrorType.LABEL_CONFLICTS_WITH_RULE;
			errMgr.GrammarError(etype, g.fileName, labelID.token, name, r.name);
		}

		if (tokenIDs.Contains(name)) {
			ErrorType etype = ErrorType.LABEL_CONFLICTS_WITH_TOKEN;
			errMgr.GrammarError(etype, g.fileName, labelID.token, name, r.name);
		}

		if (r.args != null && r.args.get(name) != null) {
			ErrorType etype = ErrorType.LABEL_CONFLICTS_WITH_ARG;
			errMgr.GrammarError(etype, g.fileName, labelID.token, name, r.name);
		}

		if (r.retvals != null && r.retvals.get(name) != null) {
			ErrorType etype = ErrorType.LABEL_CONFLICTS_WITH_RETVAL;
			errMgr.GrammarError(etype, g.fileName, labelID.token, name, r.name);
		}

		if (r.locals != null && r.locals.get(name) != null) {
			ErrorType etype = ErrorType.LABEL_CONFLICTS_WITH_LOCAL;
			errMgr.GrammarError(etype, g.fileName, labelID.token, name, r.name);
		}
	}

	public void checkForAttributeConflicts(Rule r) {
		checkDeclarationRuleConflicts(r, r.args, nameToRuleMap.Keys.ToHashSet(), ErrorType.ARG_CONFLICTS_WITH_RULE);
		checkDeclarationRuleConflicts(r, r.args, tokenIDs, ErrorType.ARG_CONFLICTS_WITH_TOKEN);

		checkDeclarationRuleConflicts(r, r.retvals, nameToRuleMap.Keys.ToHashSet(), ErrorType.RETVAL_CONFLICTS_WITH_RULE);
		checkDeclarationRuleConflicts(r, r.retvals, tokenIDs, ErrorType.RETVAL_CONFLICTS_WITH_TOKEN);

		checkDeclarationRuleConflicts(r, r.locals, nameToRuleMap.Keys.ToHashSet(), ErrorType.LOCAL_CONFLICTS_WITH_RULE);
		checkDeclarationRuleConflicts(r, r.locals, tokenIDs, ErrorType.LOCAL_CONFLICTS_WITH_TOKEN);

		checkLocalConflictingDeclarations(r, r.retvals, r.args, ErrorType.RETVAL_CONFLICTS_WITH_ARG);
		checkLocalConflictingDeclarations(r, r.locals, r.args, ErrorType.LOCAL_CONFLICTS_WITH_ARG);
		checkLocalConflictingDeclarations(r, r.locals, r.retvals, ErrorType.LOCAL_CONFLICTS_WITH_RETVAL);
	}

	protected void checkDeclarationRuleConflicts(Rule r, AttributeDict attributes, HashSet<String> ruleNames, ErrorType errorType) {
		if (attributes == null) {
			return;
		}

		foreach (tool.Attribute attribute in attributes.attributes.Values) {
			if (ruleNames.Contains(attribute.name)) {
				errMgr.GrammarError(
						errorType,
						g.fileName,
						attribute.token != null ? attribute.token : ((GrammarAST) r.ast.getChild(0)).token,
						attribute.name,
						r.name);
			}
		}
	}

	protected void checkLocalConflictingDeclarations(Rule r, AttributeDict attributes, AttributeDict referenceAttributes, ErrorType errorType) {
		if (attributes == null || referenceAttributes == null) {
			return;
		}

		HashSet<String> conflictingKeys = attributes.intersection(referenceAttributes);
        foreach (String key in conflictingKeys) {
			errMgr.GrammarError(
					errorType,
					g.fileName,
					attributes.get(key).token != null ? attributes.get(key).token : ((GrammarAST)r.ast.getChild(0)).token,
					key,
					r.name);
		}
	}

	protected void checkReservedNames(ICollection<Rule> rules) {
		foreach (Rule rule in rules) {
			if (reservedNames.Contains(rule.name)) {
				errMgr.GrammarError(ErrorType.RESERVED_RULE_NAME, g.fileName, ((GrammarAST)rule.ast.getChild(0)).getToken(), rule.name);
			}
		}
	}

	public void checkForModeConflicts(Grammar g) {
		if (g.isLexer()) {
			LexerGrammar lexerGrammar = (LexerGrammar)g;
			foreach (String modeName in lexerGrammar.modes.Keys) {
				var rx = (lexerGrammar.modes.TryGetValue(modeName, out var ret) ? ret : new());
                Rule rule = rx.FirstOrDefault();

                if (!modeName.Equals("DEFAULT_MODE") && reservedNames.Contains(modeName)) {
					g.Tools.ErrMgr.GrammarError(ErrorType.MODE_CONFLICTS_WITH_COMMON_CONSTANTS, g.fileName, rule.ast.parent.getToken(), modeName);
				}

				if (g.getTokenType(modeName) != Token.INVALID_TYPE) {
					g.Tools.ErrMgr.GrammarError(ErrorType.MODE_CONFLICTS_WITH_TOKEN, g.fileName, rule.ast.parent.getToken(), modeName);
				}
			}
		}
	}

	/**
	 * Algorithm steps:
	 * 1. Collect all simple string literals (i.e. 'asdf', 'as' 'df', but not [a-z]+, 'a'..'z')
	 *    for all lexer rules in each mode except of autogenerated tokens ({@link #getSingleTokenValues(Rule) getSingleTokenValues})
	 * 2. Compare every string literal with each other ({@link #checkForOverlap(Grammar, Rule, Rule, List<String>, List<String>) checkForOverlap})
	 *    and throw TOKEN_UNREACHABLE warning if the same string found.
	 * Complexity: O(m * n^2 / 2), approximately equals to O(n^2)
	 * where m - number of modes, n - average number of lexer rules per mode.
	 * See also testUnreachableTokens unit test for details.
	 */
	public void checkForUnreachableTokens(Grammar g) {
		if (g.isLexer()) {
			LexerGrammar lexerGrammar = (LexerGrammar)g;
			foreach (List<Rule> rules in lexerGrammar.modes.Values) {
				// Collect string literal lexer rules for each mode
				List<Rule> stringLiteralRules = new ();
				List<List<String>> stringLiteralValues = new ();
				for (int i = 0; i < rules.Count; i++) {
					Rule rule = rules[(i)];

					List<String> ruleStringAlts = getSingleTokenValues(rule);
					if (ruleStringAlts != null && ruleStringAlts.Count > 0) {
						stringLiteralRules.Add(rule);
						stringLiteralValues.Add(ruleStringAlts);
					}
				}

				// Check string sets intersection
				for (int i = 0; i < stringLiteralRules.Count; i++) {
					List<String> firstTokenStringValues = stringLiteralValues[(i)];
					Rule rule1 =  stringLiteralRules[(i)];
					checkForOverlap(g, rule1, rule1, firstTokenStringValues, stringLiteralValues[(i)]);

					// Check fragment rules only with themself
					if (!rule1.isFragment()) {
						for (int j = i + 1; j < stringLiteralRules.Count; j++) {
							Rule rule2 = stringLiteralRules[(j)];
							if (!rule2.isFragment()) {
								checkForOverlap(g, rule1, stringLiteralRules[(j)], firstTokenStringValues, stringLiteralValues[(j)]);
							}
						}
					}
				}
			}
		}
	}

	/**
	 * {@return} list of simple string literals for rule {@param rule}
	 */
	private List<String> getSingleTokenValues(Rule rule)
	{
		List<String> values = new ();
		foreach (Alternative alt in rule.alt) {
			if (alt != null) {
				// select first alt if token has a command
				Tree rootNode = alt.ast.getChildCount() == 2 &&
						alt.ast.getChild(0) is AltAST && alt.ast.getChild(1) is GrammarAST
						? alt.ast.getChild(0)
						: alt.ast;

				if (rootNode.getTokenStartIndex() == -1) {
					continue; // ignore autogenerated tokens from combined grammars that start with T__
				}

				// Ignore alt if contains not only string literals (repetition, optional)
				bool ignore = false;
				StringBuilder currentValue = new StringBuilder();
				for (int i = 0; i < rootNode.getChildCount(); i++) {
					Tree child = rootNode.getChild(i);
					if (!(child is TerminalAST)) {
						ignore = true;
						break;
					}

					TerminalAST terminalAST = (TerminalAST)child;
					if (terminalAST.token.getType() != ANTLRLexer.STRING_LITERAL) {
						ignore = true;
						break;
					}
					else {
						String text = terminalAST.token.getText();
						currentValue.Append(text.Substring(1, text.Length - 1-1));
					}
				}

				if (!ignore) {
					values.Add(currentValue.ToString());
				}
			}
		}
		return values;
	}

	/**
	 * For same rule compare values from next index:
	 * TOKEN_WITH_SAME_VALUES: 'asdf' | 'asdf';
	 * For different rules compare from start value:
	 * TOKEN1: 'asdf';
	 * TOKEN2: 'asdf';
	 */
	private void checkForOverlap(Grammar g, Rule rule1, Rule rule2, List<String> firstTokenStringValues, List<String> secondTokenStringValues) {
		for (int i = 0; i < firstTokenStringValues.Count; i++) {
			int secondTokenInd = rule1 == rule2 ? i + 1 : 0;
			String str1 = firstTokenStringValues[i];
			for (int j = secondTokenInd; j < secondTokenStringValues.Count; j++) {
				String str2 = secondTokenStringValues[j];
				if (str1.Equals(str2)) {
					errMgr.GrammarError(ErrorType.TOKEN_UNREACHABLE, g.fileName,
							((GrammarAST) rule2.ast.getChild(0)).token, rule2.name, str2, rule1.name);
				}
			}
		}
	}

	// CAN ONLY CALL THE TWO NEXT METHODS AFTER GRAMMAR HAS RULE DEFS (see semanticpipeline)
	public void checkRuleArgs(Grammar g, List<GrammarAST> rulerefs) {
		if ( rulerefs==null ) return;
        foreach (GrammarAST @ref in rulerefs) {
			String ruleName = @ref.getText();
			Rule r = g.getRule(ruleName);
			GrammarAST arg = (GrammarAST)@ref.getFirstChildWithType(ANTLRParser.ARG_ACTION);
			if ( arg!=null && (r==null || r.args==null) ) {
				errMgr.GrammarError(ErrorType.RULE_HAS_NO_ARGS,
						g.fileName, @ref.token, ruleName);

			}
			else if ( arg==null && (r!=null && r.args!=null) ) {
				errMgr.GrammarError(ErrorType.MISSING_RULE_ARGS,
						g.fileName, @ref.token, ruleName);
			}
		}
	}

	public void checkForQualifiedRuleIssues(Grammar g, List<GrammarAST> qualifiedRuleRefs) {
        foreach (GrammarAST dot in qualifiedRuleRefs) {
			GrammarAST grammar = (GrammarAST)dot.getChild(0);
			GrammarAST rule = (GrammarAST)dot.getChild(1);
			g.Tools.Log("semantics", grammar.getText()+"."+rule.getText());
			Grammar @delegate = g.getImportedGrammar(grammar.getText());
			if ( @delegate==null ) {
				errMgr.GrammarError(ErrorType.NO_SUCH_GRAMMAR_SCOPE,
						g.fileName, grammar.token, grammar.getText(),
						rule.getText());
			}
			else {
				if ( g.getRule(grammar.getText(), rule.getText())==null ) {
					errMgr.GrammarError(ErrorType.NO_SUCH_RULE_IN_SCOPE,
							g.fileName, rule.token, grammar.getText(),
							rule.getText());
				}
			}
		}
	}
}
