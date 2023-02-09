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
public class SymbolChecks
{
    Grammar g;
    SymbolCollector collector;
    Dictionary<string, Rule> nameToRuleMap = new();
    HashSet<string> tokenIDs = new();
    Dictionary<string, HashSet<string>> actionScopeToActionNames = new();

    public ErrorManager errMgr;

    protected HashSet<string> reservedNames = new(LexerATNFactory.GetCommonConstants());

    public SymbolChecks(Grammar g, SymbolCollector collector)
    {
        this.g = g;
        this.collector = collector;
        this.errMgr = g.Tools.ErrMgr;

        foreach (var tokenId in collector.tokenIDRefs)
        {
            tokenIDs.Add(tokenId.Text);
        }
    }

    public void Process()
    {
        // methods affect fields, but no side-effects outside this object
        // So, call order sensitive
        // First collect all rules for later use in checkForLabelConflict()
        if (g.rules != null)
        {
            foreach (var r in g.rules.Values) nameToRuleMap[r.name] = r;
        }
        CheckReservedNames(g.rules.Values);
        CheckActionRedefinitions(collector.namedActions);
        CheckForLabelConflicts(g.rules.Values);
    }

    public void CheckActionRedefinitions(List<GrammarAST> actions)
    {
        if (actions == null) return;
        var scope = g.GetDefaultActionScope();
        string name;
        GrammarAST nameNode;
        foreach (var ampersandAST in actions)
        {
            nameNode = (GrammarAST)ampersandAST.GetChild(0);
            if (ampersandAST.ChildCount == 2)
            {
                name = nameNode.Text;
            }
            else
            {
                scope = nameNode.Text;
                name = ampersandAST.GetChild(1).Text;
            }
            if (!actionScopeToActionNames.TryGetValue(scope, out var scopeActions))
            { // init scope
                scopeActions = new ();
                actionScopeToActionNames[scope] = scopeActions;
            }
            if (!scopeActions.Contains(name))
            {
                scopeActions.Add(name);
            }
            else
            {
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
    public void CheckForLabelConflicts(ICollection<Rule> rules)
    {
        foreach (var r in rules)
        {
            CheckForAttributeConflicts(r);

            Dictionary<string, LabelElementPair> labelNameSpace = new();
            for (int i = 1; i <= r.numberOfAlts; i++)
            {
                var a = r.alt[i];
                foreach (var pairs in a.labelDefs.Values)
                {
                    if (r.HasAltSpecificContexts())
                    {
                        // Collect labelName-labeledRules map for rule with alternative labels.
                        Dictionary<string, List<LabelElementPair>> labelPairs = new();
                        foreach (var p in pairs)
                        {
                            var labelName = FindAltLabelName(p.label);
                            if (labelName != null)
                            {
                                if (!labelPairs.TryGetValue(labelName, out var list))
                                {
                                    list = new();
                                    labelPairs[labelName] = list;
                                }
                                list.Add(p);
                            }
                        }

                        foreach (var internalPairs in labelPairs.Values)
                        {
                            labelNameSpace.Clear();
                            CheckLabelPairs(r, labelNameSpace, internalPairs);
                        }
                    }
                    else
                    {
                        CheckLabelPairs(r, labelNameSpace, pairs);
                    }
                }
            }
        }
    }

    private void CheckLabelPairs(Rule r, Dictionary<string, LabelElementPair> labelNameSpace, List<LabelElementPair> pairs)
    {
        foreach (var p in pairs)
        {
            CheckForLabelConflict(r, p.label);
            var name = p.label.Text;
            if (!labelNameSpace.TryGetValue(name, out var prev))
            {
                labelNameSpace[name] = p;
            }
            else
            {
                CheckForTypeMismatch(r, prev, p);
            }
        }
    }

    private string FindAltLabelName(CommonTree label)
    {
        if (label == null)
        {
            return null;
        }
        else if (label is AltAST altAST)
        {
            if (altAST.altLabel != null)
            {
                return altAST.altLabel.ToString();
            }
            else if (altAST.leftRecursiveAltInfo != null)
            {
                return altAST.leftRecursiveAltInfo.altLabel.ToString();
            }
            else
            {
                return FindAltLabelName(label.parent);
            }
        }
        else
        {
            return FindAltLabelName(label.parent);
        }
    }

    private void CheckForTypeMismatch(Rule r, LabelElementPair prevLabelPair, LabelElementPair labelPair)
    {
        // label already defined; if same type, no problem
        if (prevLabelPair.type != labelPair.type)
        {
            // Current behavior: take a token of rule declaration in case of left-recursive rule
            // Desired behavior: take a token of proper label declaration in case of left-recursive rule
            // See https://github.com/antlr/antlr4/pull/1585
            // Such behavior is referring to the fact that the warning is typically reported on the actual label redefinition,
            //   but for left-recursive rules the warning is reported on the enclosing rule.
            var token = r is LeftRecursiveRule
                    ? ((GrammarAST)r.ast.GetChild(0)).Token
                    : labelPair.label.token;
            errMgr.GrammarError(
                    ErrorType.LABEL_TYPE_CONFLICT,
                    g.fileName,
                    token,
                    labelPair.label.                    Text,
                    labelPair.type + "!=" + prevLabelPair.type);
        }
        if (!prevLabelPair.element.Text.Equals(labelPair.element.Text) &&
            (prevLabelPair.type.Equals(LabelType.RULE_LABEL) || prevLabelPair.type.Equals(LabelType.RULE_LIST_LABEL)) &&
            (labelPair.type.Equals(LabelType.RULE_LABEL) || labelPair.type.Equals(LabelType.RULE_LIST_LABEL)))
        {

            var token = r is LeftRecursiveRule
                    ? ((GrammarAST)r.ast.GetChild(0)).Token
                    : labelPair.label.token;
            var prevLabelOp = prevLabelPair.type.Equals(LabelType.RULE_LIST_LABEL) ? "+=" : "=";
            var labelOp = labelPair.type.Equals(LabelType.RULE_LIST_LABEL) ? "+=" : "=";
            errMgr.GrammarError(
                    ErrorType.LABEL_TYPE_CONFLICT,
                    g.fileName,
                    token,
                    labelPair.label.                    Text + labelOp + labelPair.element.Text,
                    prevLabelPair.label.                    Text + prevLabelOp + prevLabelPair.element.Text);
        }
    }

    public void CheckForLabelConflict(Rule r, GrammarAST labelID)
    {
        var name = labelID.Text;
        if (nameToRuleMap.ContainsKey(name))
        {
            ErrorType etype = ErrorType.LABEL_CONFLICTS_WITH_RULE;
            errMgr.GrammarError(etype, g.fileName, labelID.token, name, r.name);
        }

        if (tokenIDs.Contains(name))
        {
            ErrorType etype = ErrorType.LABEL_CONFLICTS_WITH_TOKEN;
            errMgr.GrammarError(etype, g.fileName, labelID.token, name, r.name);
        }

        if (r.args != null && r.args.Get(name) != null)
        {
            ErrorType etype = ErrorType.LABEL_CONFLICTS_WITH_ARG;
            errMgr.GrammarError(etype, g.fileName, labelID.token, name, r.name);
        }

        if (r.retvals != null && r.retvals.Get(name) != null)
        {
            ErrorType etype = ErrorType.LABEL_CONFLICTS_WITH_RETVAL;
            errMgr.GrammarError(etype, g.fileName, labelID.token, name, r.name);
        }

        if (r.locals != null && r.locals.Get(name) != null)
        {
            ErrorType etype = ErrorType.LABEL_CONFLICTS_WITH_LOCAL;
            errMgr.GrammarError(etype, g.fileName, labelID.token, name, r.name);
        }
    }

    public void CheckForAttributeConflicts(Rule r)
    {
        CheckDeclarationRuleConflicts(r, r.args, nameToRuleMap.Keys.ToHashSet(), ErrorType.ARG_CONFLICTS_WITH_RULE);
        CheckDeclarationRuleConflicts(r, r.args, tokenIDs, ErrorType.ARG_CONFLICTS_WITH_TOKEN);

        CheckDeclarationRuleConflicts(r, r.retvals, nameToRuleMap.Keys.ToHashSet(), ErrorType.RETVAL_CONFLICTS_WITH_RULE);
        CheckDeclarationRuleConflicts(r, r.retvals, tokenIDs, ErrorType.RETVAL_CONFLICTS_WITH_TOKEN);

        CheckDeclarationRuleConflicts(r, r.locals, nameToRuleMap.Keys.ToHashSet(), ErrorType.LOCAL_CONFLICTS_WITH_RULE);
        CheckDeclarationRuleConflicts(r, r.locals, tokenIDs, ErrorType.LOCAL_CONFLICTS_WITH_TOKEN);

        CheckLocalConflictingDeclarations(r, r.retvals, r.args, ErrorType.RETVAL_CONFLICTS_WITH_ARG);
        CheckLocalConflictingDeclarations(r, r.locals, r.args, ErrorType.LOCAL_CONFLICTS_WITH_ARG);
        CheckLocalConflictingDeclarations(r, r.locals, r.retvals, ErrorType.LOCAL_CONFLICTS_WITH_RETVAL);
    }

    protected void CheckDeclarationRuleConflicts(Rule r, AttributeDict attributes, HashSet<string> ruleNames, ErrorType errorType)
    {
        if (attributes == null)
        {
            return;
        }

        foreach (tool.Attribute attribute in attributes.attributes.Values)
        {
            if (ruleNames.Contains(attribute.name))
            {
                errMgr.GrammarError(
                        errorType,
                        g.fileName,
                        attribute.token != null ? attribute.token : ((GrammarAST)r.ast.GetChild(0)).token,
                        attribute.name,
                        r.name);
            }
        }
    }

    protected void CheckLocalConflictingDeclarations(Rule r, AttributeDict attributes, AttributeDict referenceAttributes, ErrorType errorType)
    {
        if (attributes == null || referenceAttributes == null)
        {
            return;
        }

        var conflictingKeys = attributes.Intersection(referenceAttributes);
        foreach (var key in conflictingKeys)
        {
            errMgr.GrammarError(
                    errorType,
                    g.fileName,
                    attributes.Get(key).token != null ? attributes.Get(key).token : ((GrammarAST)r.ast.GetChild(0)).token,
                    key,
                    r.name);
        }
    }

    protected void CheckReservedNames(ICollection<Rule> rules)
    {
        foreach (var rule in rules)
        {
            if (reservedNames.Contains(rule.name))
            {
                errMgr.GrammarError(ErrorType.RESERVED_RULE_NAME, g.fileName, ((GrammarAST)rule.ast.GetChild(0)).Token, rule.name);
            }
        }
    }

    public void CheckForModeConflicts(Grammar g)
    {
        if (g.IsLexer)
        {
            var lexerGrammar = (LexerGrammar)g;
            foreach (var modeName in lexerGrammar.modes.Keys)
            {
                var rx = (lexerGrammar.modes.TryGetValue(modeName, out var ret) ? ret : new());
                var rule = rx.FirstOrDefault();

                if (!modeName.Equals("DEFAULT_MODE") && reservedNames.Contains(modeName))
                {
                    g.Tools.ErrMgr.GrammarError(ErrorType.MODE_CONFLICTS_WITH_COMMON_CONSTANTS, g.fileName, rule.ast.parent.Token, modeName);
                }

                if (g.GetTokenType(modeName) != Token.INVALID_TYPE)
                {
                    g.Tools.ErrMgr.GrammarError(ErrorType.MODE_CONFLICTS_WITH_TOKEN, g.fileName, rule.ast.parent.Token, modeName);
                }
            }
        }
    }

    /**
	 * Algorithm steps:
	 * 1. Collect all simple string literals (i.e. 'asdf', 'as' 'df', but not [a-z]+, 'a'..'z')
	 *    for all lexer rules in each mode except of autogenerated tokens ({@link #getSingleTokenValues(Rule) getSingleTokenValues})
	 * 2. Compare every string literal with each other ({@link #checkForOverlap(Grammar, Rule, Rule, List<string>, List<string>) checkForOverlap})
	 *    and throw TOKEN_UNREACHABLE warning if the same string found.
	 * Complexity: O(m * n^2 / 2), approximately equals to O(n^2)
	 * where m - number of modes, n - average number of lexer rules per mode.
	 * See also testUnreachableTokens unit test for details.
	 */
    public void CheckForUnreachableTokens(Grammar g)
    {
        if (g.IsLexer)
        {
            var lexerGrammar = (LexerGrammar)g;
            foreach (var rules in lexerGrammar.modes.Values)
            {
                // Collect string literal lexer rules for each mode
                List<Rule> stringLiteralRules = new();
                List<List<string>> stringLiteralValues = new();
                for (int i = 0; i < rules.Count; i++)
                {
                    var rule = rules[(i)];

                    var ruleStringAlts = GetSingleTokenValues(rule);
                    if (ruleStringAlts != null && ruleStringAlts.Count > 0)
                    {
                        stringLiteralRules.Add(rule);
                        stringLiteralValues.Add(ruleStringAlts);
                    }
                }

                // Check string sets intersection
                for (int i = 0; i < stringLiteralRules.Count; i++)
                {
                    var firstTokenStringValues = stringLiteralValues[(i)];
                    var rule1 = stringLiteralRules[(i)];
                    CheckForOverlap(g, rule1, rule1, firstTokenStringValues, stringLiteralValues[(i)]);

                    // Check fragment rules only with themself
                    if (!rule1.IsFragment)
                    {
                        for (int j = i + 1; j < stringLiteralRules.Count; j++)
                        {
                            var rule2 = stringLiteralRules[(j)];
                            if (!rule2.IsFragment)
                            {
                                CheckForOverlap(g, rule1, stringLiteralRules[(j)], firstTokenStringValues, stringLiteralValues[(j)]);
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
    private List<string> GetSingleTokenValues(Rule rule)
    {
        List<string> values = new();
        foreach (var alt in rule.alt)
        {
            if (alt != null)
            {
                // select first alt if token has a command
                var rootNode = alt.ast.ChildCount == 2 &&
                        alt.ast.GetChild(0) is AltAST && alt.ast.GetChild(1) is GrammarAST
                        ? alt.ast.GetChild(0)
                        : alt.ast;

                if (rootNode.TokenStartIndex == -1)
                {
                    continue; // ignore autogenerated tokens from combined grammars that start with T__
                }

                // Ignore alt if contains not only string literals (repetition, optional)
                bool ignore = false;
                var currentValue = new StringBuilder();
                for (int i = 0; i < rootNode.ChildCount; i++)
                {
                    var child = rootNode.GetChild(i);
                    if (child is not TerminalAST)
                    {
                        ignore = true;
                        break;
                    }

                    var terminalAST = (TerminalAST)child;
                    if (terminalAST.token.Type != ANTLRLexer.STRING_LITERAL)
                    {
                        ignore = true;
                        break;
                    }
                    else
                    {
                        var text = terminalAST.token.Text;
                        currentValue.Append(text[1..^1]);
                    }
                }

                if (!ignore)
                {
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
    private void CheckForOverlap(Grammar g, Rule rule1, Rule rule2, List<string> firstTokenStringValues, List<string> secondTokenStringValues)
    {
        for (int i = 0; i < firstTokenStringValues.Count; i++)
        {
            int secondTokenInd = rule1 == rule2 ? i + 1 : 0;
            var str1 = firstTokenStringValues[i];
            for (int j = secondTokenInd; j < secondTokenStringValues.Count; j++)
            {
                var str2 = secondTokenStringValues[j];
                if (str1.Equals(str2))
                {
                    errMgr.GrammarError(ErrorType.TOKEN_UNREACHABLE, g.fileName,
                            ((GrammarAST)rule2.ast.GetChild(0)).token, rule2.name, str2, rule1.name);
                }
            }
        }
    }

    // CAN ONLY CALL THE TWO NEXT METHODS AFTER GRAMMAR HAS RULE DEFS (see semanticpipeline)
    public void CheckRuleArgs(Grammar g, List<GrammarAST> rulerefs)
    {
        if (rulerefs == null) return;
        foreach (var @ref in rulerefs)
        {
            var ruleName = @ref.Text;
            var r = g.GetRule(ruleName);
            var arg = (GrammarAST)@ref.GetFirstChildWithType(ANTLRParser.ARG_ACTION);
            if (arg != null && (r == null || r.args == null))
            {
                errMgr.GrammarError(ErrorType.RULE_HAS_NO_ARGS,
                        g.fileName, @ref.token, ruleName);

            }
            else if (arg == null && (r != null && r.args != null))
            {
                errMgr.GrammarError(ErrorType.MISSING_RULE_ARGS,
                        g.fileName, @ref.token, ruleName);
            }
        }
    }

    public void CheckForQualifiedRuleIssues(Grammar g, List<GrammarAST> qualifiedRuleRefs)
    {
        foreach (var dot in qualifiedRuleRefs)
        {
            var grammar = (GrammarAST)dot.GetChild(0);
            var rule = (GrammarAST)dot.GetChild(1);
            g.Tools.Log("semantics", grammar.Text + "." + rule.Text);
            var @delegate = g.GetImportedGrammar(grammar.Text);
            if (@delegate == null)
            {
                errMgr.GrammarError(ErrorType.NO_SUCH_GRAMMAR_SCOPE,
                        g.fileName, grammar.token, grammar.Text,
                        rule.                        Text);
            }
            else
            {
                if (g.GetRule(grammar.Text, rule.Text) == null)
                {
                    errMgr.GrammarError(ErrorType.NO_SUCH_RULE_IN_SCOPE,
                            g.fileName, rule.token, grammar.Text,
                            rule.                            Text);
                }
            }
        }
    }
}
