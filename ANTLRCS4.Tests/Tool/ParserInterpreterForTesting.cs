/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime;
using org.antlr.v4.runtime.atn;
using org.antlr.v4.runtime.dfa;
using org.antlr.v4.tool;

namespace org.antlr.v4.test.tool;

public class ParserInterpreterForTesting
{
    public class DummyParser : antlr.runtime.Parser
    {
        public readonly ATN atn;
        public readonly DFA[] decisionToDFA; // not shared for interp
        public readonly PredictionContextCache sharedContextCache =
            new ();

        public Grammar g;
        public DummyParser(Grammar g, ATN atn, TokenStream input) : base(input)
        {
            this.g = g;
            this.atn = atn;
            this.decisionToDFA = new DFA[atn.NumberOfDecisions()];
            for (int i = 0; i < decisionToDFA.Length; i++)
            {
                decisionToDFA[i] = new DFA(atn.GetDecisionState(i), i);
            }
        }

        public override string GrammarFileName => throw new UnsupportedOperationException("not implemented");

        //@Override
        public string[] RuleNames => g.rules.Keys.ToArray();//.toArray(new String[0]);

        public override string[] TokenNames => g.GetTokenNames();

        public virtual ATN ATN => atn;
    }

    protected Grammar g;
    public DummyParser parser;
    protected ParserATNSimulator atnSimulator;
    protected TokenStream input;

    public ParserInterpreterForTesting(Grammar g)
    {
        this.g = g;
    }

    public ParserInterpreterForTesting(Grammar g, TokenStream input)
    {
        var antlr = new Tool();
        antlr.Process(g, false);
        parser = new DummyParser(g, g.atn, input);
        atnSimulator =
            new (null/*parser*/, g.atn, parser.decisionToDFA,
                                          parser.sharedContextCache);
    }

    public int AdaptivePredict(TokenStream input, int decision,
                               ParserRuleContext outerContext)
    {
        return atnSimulator.AdaptivePredict(input, decision, outerContext);
    }

    public int MatchATN(TokenStream input,
                        ATNState startState)
    {
        if (startState.NumberOfTransitions == 1)
        {
            return 1;
        }
        else if (startState is DecisionState)
        {
            return atnSimulator.AdaptivePredict(input, ((DecisionState)startState).decision, null);
        }
        else if (startState.NumberOfTransitions > 0)
        {
            return 1;
        }
        else
        {
            return -1;
        }
    }

    public ParserATNSimulator GetATNSimulator()
    {
        return atnSimulator;
    }

}
