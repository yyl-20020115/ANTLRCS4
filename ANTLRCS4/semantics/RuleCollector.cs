/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.analysis;
using org.antlr.v4.misc;
using org.antlr.v4.parse;
using org.antlr.v4.runtime.misc;
using org.antlr.v4.tool;
using org.antlr.v4.tool.ast;

namespace org.antlr.v4.semantics;

public class RuleCollector : GrammarTreeVisitor {
	private bool grammarCaseInsensitive = false;

	/** which grammar are we checking */
	public Grammar g;
	public ErrorManager errMgr;

	// stuff to collect. this is the output
	public OrderedHashMap<String, Rule> rules = new OrderedHashMap<String, Rule>();
	public MultiMap<String,GrammarAST> ruleToAltLabels = new MultiMap<String, GrammarAST>();
	public Dictionary<String,String> altLabelToRuleName = new ();

	public RuleCollector(Grammar g) {
		this.g = g;
		this.errMgr = g.Tools.ErrMgr;
	}

	//@Override
	public ErrorManager getErrorManager() { return errMgr; }

	public void process(GrammarAST ast) { visitGrammar(ast); }

	//@Override
	public void discoverRule(RuleAST rule, GrammarAST ID,
							 List<GrammarAST> modifiers, ActionAST arg,
							 ActionAST returns, GrammarAST thrws,
							 GrammarAST options, ActionAST locals,
							 List<GrammarAST> actions,
							 GrammarAST block)
	{
		int numAlts = block.ChildCount;
		Rule r;
		if ( LeftRecursiveRuleAnalyzer.hasImmediateRecursiveRuleRefs(rule, ID.getText()) ) {
			r = new LeftRecursiveRule(g, ID.getText(), rule);
		}
		else {
			r = new Rule(g, ID.getText(), rule, numAlts);
		}
		rules.Put(r.name, r);

		if ( arg!=null ) {
			r.args = ScopeParser.ParseTypedArgList(arg, arg.getText(), g);
			r.args.type = AttributeDict.DictType.ARG;
			r.args.ast = arg;
			arg.resolver = r.alt[currentOuterAltNumber];
		}

		if ( returns!=null ) {
			r.retvals = ScopeParser.ParseTypedArgList(returns, returns.getText(), g);
			r.retvals.type = AttributeDict.DictType.RET;
			r.retvals.ast = returns;
		}

		if ( locals!=null ) {
			r.locals = ScopeParser.ParseTypedArgList(locals, locals.getText(), g);
			r.locals.type = AttributeDict.DictType.LOCAL;
			r.locals.ast = locals;
		}

        foreach (GrammarAST a in actions) {
			// a = ^(AT ID ACTION)
			ActionAST action = (ActionAST) a.GetChild(1);
			r.namedActions[a.GetChild(0).Text]= action;
			action.resolver = r;
		}
	}

	//@Override
	public void discoverOuterAlt(AltAST alt) {
		if ( alt.altLabel!=null ) {
			ruleToAltLabels.Map(currentRuleName, alt.altLabel);
			String altLabel = alt.altLabel.getText();
			altLabelToRuleName[Utils.Capitalize(altLabel)]= currentRuleName;
			altLabelToRuleName[misc.Utils.Decapitalize(altLabel)] = currentRuleName;
		}
	}

	//@Override
	public void grammarOption(GrammarAST ID, GrammarAST valueAST) {
		var caseInsensitive = getCaseInsensitiveValue(ID, valueAST);
		if (caseInsensitive != null) {
			grammarCaseInsensitive = caseInsensitive.GetValueOrDefault();
		}
	}

	//@Override
	public void discoverLexerRule(RuleAST rule, GrammarAST ID, List<GrammarAST> modifiers,
								  GrammarAST options, GrammarAST block)
	{
		bool currentCaseInsensitive = grammarCaseInsensitive;
		if (options != null) {
			foreach (Object child in options.GetChildren()) {
				GrammarAST childAST = (GrammarAST) child;
				var caseInsensitive = getCaseInsensitiveValue((GrammarAST)childAST.GetChild(0), (GrammarAST)childAST.GetChild(1));
				if (caseInsensitive != null) {
					currentCaseInsensitive = caseInsensitive.GetValueOrDefault();
				}
			}
		}

		int numAlts = block.ChildCount;
		Rule r = new Rule(g, ID.getText(), rule, numAlts, currentModeName, currentCaseInsensitive);
		if ( modifiers.Count>0 ) r.modifiers = modifiers;
		rules.Put(r.name, r);
	}

	private bool? getCaseInsensitiveValue(GrammarAST optionID, GrammarAST valueAST) {
		String optionName = optionID.getText();
		if (optionName.Equals(Grammar.caseInsensitiveOptionName)) {
			String valueText = valueAST.getText();
			if (valueText.Equals("true") || valueText.Equals("false")) {
				return bool.TryParse(valueText, out var ret1) && ret1;
			}
		}
		return null;
	}
}
