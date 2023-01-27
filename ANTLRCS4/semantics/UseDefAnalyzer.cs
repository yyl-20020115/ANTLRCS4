/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.tool;

namespace org.antlr.v4.semantics;
/** Look for errors and deadcode stuff */
public class UseDefAnalyzer {
	// side-effect: updates Alternative with refs in actions
	public static void trackTokenRuleRefsInActions(Grammar g) {
		for (Rule r : g.rules.values()) {
			for (int i=1; i<=r.numberOfAlts; i++) {
				Alternative alt = r.alt[i];
				for (ActionAST a : alt.actions) {
					ActionSniffer sniffer =	new ActionSniffer(g, r, alt, a, a.token);
					sniffer.examineAction();
				}
			}
		}
	}

	public static boolean actionIsContextDependent(ActionAST actionAST) {
		ANTLRStringStream @in = new ANTLRStringStream(actionAST.token.getText());
		@in.setLine(actionAST.token.getLine());
		@in.setCharPositionInLine(actionAST.token.getCharPositionInLine());
		final boolean[] dependent = new boolean[] {false}; // can't be simple bool with anon class
		ActionSplitterListener listener = new BlankActionSplitterListener() {
			@Override
			public void nonLocalAttr(String expr, Token x, Token y) { dependent[0] = true; }
			@Override
			public void qualifiedAttr(String expr, Token x, Token y) { dependent[0] = true; }
			@Override
			public void setAttr(String expr, Token x, Token rhs) { dependent[0] = true; }
			@Override
			public void setExprAttribute(String expr) { dependent[0] = true; }
			@Override
			public void setNonLocalAttr(String expr, Token x, Token y, Token rhs) { dependent[0] = true; }
			@Override
			public void attr(String expr, Token x) {  dependent[0] = true; }
		};
		ActionSplitter splitter = new ActionSplitter(in, listener);
		// forces eval, triggers listener methods
		splitter.getActionTokens();
		return dependent[0];
	}

	/** Find all rules reachable from r directly or indirectly for all r in g */
	public static Map<Rule, Set<Rule>> getRuleDependencies(Grammar g) {
		return getRuleDependencies(g, g.rules.values());
	}

	public static Map<Rule, Set<Rule>> getRuleDependencies(LexerGrammar g, String modeName) {
		return getRuleDependencies(g, g.modes.get(modeName));
	}

	public static Map<Rule, Set<Rule>> getRuleDependencies(Grammar g, Collection<Rule> rules) {
		Map<Rule, Set<Rule>> dependencies = new HashMap<Rule, Set<Rule>>();

		for (Rule r : rules) {
			List<GrammarAST> tokenRefs = r.ast.getNodesWithType(ANTLRParser.TOKEN_REF);
			for (GrammarAST tref : tokenRefs) {
				Set<Rule> calls = dependencies.get(r);
				if ( calls==null ) {
					calls = new HashSet<Rule>();
					dependencies.put(r, calls);
				}
				calls.add(g.getRule(tref.getText()));
			}
		}

		return dependencies;
	}

}
