/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.codegen.model;
using org.antlr.v4.runtime;
using org.antlr.v4.runtime.atn;
using org.antlr.v4.runtime.dfa;
using org.antlr.v4.tool;

namespace org.antlr.v4.test.tool;

public class ParserInterpreterForTesting {
	public class DummyParser : codegen.model.Parser
    {
		public readonly ATN atn;
		public readonly DFA[] decisionToDFA; // not shared for interp
		public readonly PredictionContextCache sharedContextCache =
			new PredictionContextCache();

		public Grammar g;
		public DummyParser(Grammar g, ATN atn, TokenStream input): base(input)
        {
			this.g = g;
			this.atn = atn;
			this.decisionToDFA = new DFA[atn.getNumberOfDecisions()];
			for (int i = 0; i < decisionToDFA.Length; i++) {
				decisionToDFA[i] = new DFA(atn.getDecisionState(i), i);
			}
		}

		//@Override
		public String getGrammarFileName() {
			throw new UnsupportedOperationException("not implemented");
		}

		//@Override
		public String[] getRuleNames() {
			return g.rules.Keys.ToArray();//.toArray(new String[0]);
		}

		//@Override
		//@Deprecated
		public String[] getTokenNames() {
			return g.getTokenNames();
		}

		//@Override
		public ATN getATN() {
			return atn;
		}
	}

	protected Grammar g;
	public DummyParser parser;
	protected ParserATNSimulator atnSimulator;
	protected TokenStream input;

	public ParserInterpreterForTesting(Grammar g) {
		this.g = g;
	}

	public ParserInterpreterForTesting(Grammar g, TokenStream input) {
		Tool antlr = new Tool();
		antlr.process(g,false);
		parser = new DummyParser(g, g.atn, input);
		atnSimulator =
			new ParserATNSimulator(parser, g.atn, parser.decisionToDFA,
										  parser.sharedContextCache);
	}

	public int adaptivePredict(TokenStream input, int decision,
							   ParserRuleContext outerContext)
	{
		return atnSimulator.adaptivePredict(input, decision, outerContext);
	}

	public int matchATN(TokenStream input,
						ATNState startState)
	{
		if (startState.getNumberOfTransitions() == 1) {
			return 1;
		}
		else if (startState is DecisionState) {
			return atnSimulator.adaptivePredict(input, ((DecisionState)startState).decision, null);
		}
		else if (startState.getNumberOfTransitions() > 0) {
			return 1;
		}
		else {
			return -1;
		}
	}

	public ParserATNSimulator getATNSimulator() {
		return atnSimulator;
	}

}
