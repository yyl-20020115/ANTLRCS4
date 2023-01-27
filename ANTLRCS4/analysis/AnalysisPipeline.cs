/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime;
using org.antlr.v4.runtime.atn;
using org.antlr.v4.runtime.misc;
using org.antlr.v4.tool;
using org.antlr.v4.tool.ast;

namespace org.antlr.v4.analysis;



public class AnalysisPipeline {
	public Grammar g;

	public AnalysisPipeline(Grammar g) {
		this.g = g;
	}

	public void process() {
		// LEFT-RECURSION CHECK
		LeftRecursionDetector lr = new LeftRecursionDetector(g, g.atn);
		lr.check();
		if ( lr.listOfRecursiveCycles.Count>0 ) return; // bail out

		if (g.isLexer()) {
			processLexer();
		}
		else {
			// BUILD DFA FOR EACH DECISION
			processParser();
		}
	}

	protected void processLexer() {
		// make sure all non-fragment lexer rules must match at least one symbol
		foreach (Rule rule in g.rules.Values) {
			if (rule.isFragment()) {
				continue;
			}

			LL1Analyzer analyzer = new LL1Analyzer(g.atn);
			IntervalSet look = analyzer.LOOK(g.atn.ruleToStartState[rule.index], null);
			if (look.contains(Token.EPSILON)) {
				g.tool.errMgr.grammarError(ErrorType.EPSILON_TOKEN, g.fileName, ((GrammarAST)rule.ast.getChild(0)).getToken(), rule.name);
			}
		}
	}

	protected void processParser() {
		g.decisionLOOK = new (g.atn.getNumberOfDecisions()+1);
		foreach (DecisionState s in g.atn.decisionToState) {
            g.tool.log("LL1", "\nDECISION "+s.decision+" in rule "+g.getRule(s.ruleIndex).name);
			IntervalSet[] look;
			if ( s.nonGreedy ) { // nongreedy decisions can't be LL(1)
				look = new IntervalSet[s.getNumberOfTransitions()+1];
			}
			else {
				LL1Analyzer anal = new LL1Analyzer(g.atn);
				look = anal.getDecisionLookahead(s);
				g.tool.log("LL1", "look=" + Arrays.toString(look));
			}

			//assert s.decision + 1 >= g.decisionLOOK.size();
			Utils.setSize(g.decisionLOOK, s.decision+1);
			g.decisionLOOK[s.decision]= look;
			g.tool.log("LL1", "LL(1)? " + disjoint(look));
		}
	}

	/** Return whether lookahead sets are disjoint; no lookahead ⇒ not disjoint */
	public static bool disjoint(IntervalSet[] altLook) {
        bool collision = false;
		IntervalSet combined = new IntervalSet();
		if ( altLook==null ) return false;
		foreach (IntervalSet look in altLook) {
			if ( look==null ) return false; // lookahead must've computation failed
			if ( !look.and(combined).isNil() ) {
				collision = true;
				break;
			}
			combined.addAll(look);
		}
		return !collision;
	}
}
