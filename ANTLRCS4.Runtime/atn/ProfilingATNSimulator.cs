/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime.dfa;
using org.antlr.v4.runtime.misc;

namespace org.antlr.v4.runtime.atn;

/**
 * @since 4.3
 */
public class ProfilingATNSimulator : ParserATNSimulator
{
    protected readonly DecisionInfo[] decisions;
    protected int numDecisions;

    protected int _sllStopIndex;
    protected int _llStopIndex;

    protected int currentDecision;
    protected DFAState currentState;

    /** At the point of LL failover, we record how SLL would resolve the conflict so that
	 *  we can determine whether or not a decision / input pair is context-sensitive.
	 *  If LL gives a different result than SLL's predicted alternative, we have a
	 *  context sensitivity for sure. The converse is not necessarily true, however.
	 *  It's possible that after conflict resolution chooses minimum alternatives,
	 *  SLL could get the same answer as LL. Regardless of whether or not the result indicates
	 *  an ambiguity, it is not treated as a context sensitivity because LL prediction
	 *  was not required in order to produce a correct prediction for this decision and input sequence.
	 *  It may in fact still be a context sensitivity but we don't know by looking at the
	 *  minimum alternatives for the current input.
 	 */
    protected int conflictingAltResolvedBySLL;

    public ProfilingATNSimulator(Parser parser) : base(parser,
                parser.GetInterpreter().atn,
                parser.GetInterpreter().decisionToDFA,
                parser.GetInterpreter().sharedContextCache)
    {
        numDecisions = atn.decisionToState.Count;
        decisions = new DecisionInfo[numDecisions];
        for (int i = 0; i < numDecisions; i++)
            decisions[i] = new (i);
    }

    public override int AdaptivePredict(TokenStream input, int decision, ParserRuleContext outerContext)
    {
        try
        {
            this._sllStopIndex = -1;
            this._llStopIndex = -1;
            this.currentDecision = decision;
            long start = DateTime.Now.Nanosecond;// System.nanoTime(); // expensive but useful info
            int alt = base.AdaptivePredict(input, decision, outerContext);
            long stop = DateTime.Now.Nanosecond;
            decisions[decision].timeInPrediction += (stop - start);
            decisions[decision].invocations++;

            int SLL_k = _sllStopIndex - _startIndex + 1;
            decisions[decision].SLL_TotalLook += SLL_k;
            decisions[decision].SLL_MinLook = decisions[decision].SLL_MinLook == 0 ? SLL_k : Math.Min(decisions[decision].SLL_MinLook, SLL_k);
            if (SLL_k > decisions[decision].SLL_MaxLook)
            {
                decisions[decision].SLL_MaxLook = SLL_k;
                decisions[decision].SLL_MaxLookEvent =
                        new (decision, null, alt, input, _startIndex, _sllStopIndex, false);
            }

            if (_llStopIndex >= 0)
            {
                int LL_k = _llStopIndex - _startIndex + 1;
                decisions[decision].LL_TotalLook += LL_k;
                decisions[decision].LL_MinLook = decisions[decision].LL_MinLook == 0 ? LL_k : Math.Min(decisions[decision].LL_MinLook, LL_k);
                if (LL_k > decisions[decision].LL_MaxLook)
                {
                    decisions[decision].LL_MaxLook = LL_k;
                    decisions[decision].LL_MaxLookEvent =
                            new (decision, null, alt, input, _startIndex, _llStopIndex, true);
                }
            }

            return alt;
        }
        finally
        {
            this.currentDecision = -1;
        }
    }

    
    protected override DFAState GetExistingTargetState(DFAState previousD, int t)
    {
        // this method is called after each time the input position advances
        // during SLL prediction
        _sllStopIndex = _input.Index;

        var existingTargetState = base.GetExistingTargetState(previousD, t);
        if (existingTargetState != null)
        {
            decisions[currentDecision].SLL_DFATransitions++; // count only if we transition over a DFA state
            if (existingTargetState == ERROR)
            {
                decisions[currentDecision].errors.Add(
                        new (currentDecision, previousD.configs, _input, _startIndex, _sllStopIndex, false)
                );
            }
        }

        currentState = existingTargetState;
        return existingTargetState;
    }

    
    protected override DFAState ComputeTargetState(DFA dfa, DFAState previousD, int t)
    {
        var state = base.ComputeTargetState(dfa, previousD, t);
        currentState = state;
        return state;
    }

    
    protected override ATNConfigSet ComputeReachSet(ATNConfigSet closure, int t, bool fullCtx)
    {
        if (fullCtx)
        {
            // this method is called after each time the input position advances
            // during full context prediction
            _llStopIndex = _input.Index;
        }

        var reachConfigs = base.ComputeReachSet(closure, t, fullCtx);
        if (fullCtx)
        {
            decisions[currentDecision].LL_ATNTransitions++; // count computation even if error
            if (reachConfigs != null)
            {
            }
            else
            { // no reach on current lookahead symbol. ERROR.
              // TODO: does not handle delayed errors per getSynValidOrSemInvalidAltThatFinishedDecisionEntryRule()
                decisions[currentDecision].errors.Add(
                    new (currentDecision, closure, _input, _startIndex, _llStopIndex, true)
                );
            }
        }
        else
        {
            decisions[currentDecision].SLL_ATNTransitions++;
            if (reachConfigs != null)
            {
            }
            else
            { // no reach on current lookahead symbol. ERROR.
                decisions[currentDecision].errors.Add(
                    new (currentDecision, closure, _input, _startIndex, _sllStopIndex, false)
                );
            }
        }
        return reachConfigs;
    }

    
    protected override bool EvalSemanticContext(SemanticContext pred, ParserRuleContext parserCallStack, int alt, bool fullCtx)
    {
        bool result = base.EvalSemanticContext(pred, parserCallStack, alt, fullCtx);
        if (pred is not SemanticContext.PrecedencePredicate)
        {
            bool fullContext = _llStopIndex >= 0;
            int stopIndex = fullContext ? _llStopIndex : _sllStopIndex;
            decisions[currentDecision].predicateEvals.Add(
                new (currentDecision, _input, _startIndex, stopIndex, pred, result, alt, fullCtx)
            );
        }

        return result;
    }

    
    protected override void ReportAttemptingFullContext(DFA dfa, BitSet conflictingAlts, ATNConfigSet configs, int startIndex, int stopIndex)
    {
        conflictingAltResolvedBySLL = conflictingAlts != null ? conflictingAlts.NextSetBit(0) : configs.GetAlts().NextSetBit(0);
        decisions[currentDecision].LL_Fallback++;
        base.ReportAttemptingFullContext(dfa, conflictingAlts, configs, startIndex, stopIndex);
    }

    
    protected override void ReportContextSensitivity(DFA dfa, int prediction, ATNConfigSet configs, int startIndex, int stopIndex)
    {
        if (prediction != conflictingAltResolvedBySLL)
        {
            decisions[currentDecision].contextSensitivities.Add(
                    new (currentDecision, configs, _input, startIndex, stopIndex)
            );
        }
        base.ReportContextSensitivity(dfa, prediction, configs, startIndex, stopIndex);
    }

    
    protected override void ReportAmbiguity(DFA dfa, DFAState D, int startIndex, int stopIndex, bool exact,
                                   BitSet ambigAlts, ATNConfigSet configs)
    {
        int prediction;
        if (ambigAlts != null)
        {
            prediction = ambigAlts.NextSetBit(0);
        }
        else
        {
            prediction = configs.GetAlts().NextSetBit(0);
        }
        if (configs.fullCtx && prediction != conflictingAltResolvedBySLL)
        {
            // Even though this is an ambiguity we are reporting, we can
            // still detect some context sensitivities.  Both SLL and LL
            // are showing a conflict, hence an ambiguity, but if they resolve
            // to different minimum alternatives we have also identified a
            // context sensitivity.
            decisions[currentDecision].contextSensitivities.Add(
                    new (currentDecision, configs, _input, startIndex, stopIndex)
            );
        }
        decisions[currentDecision].ambiguities.Add(
            new (currentDecision, configs, ambigAlts,
                              _input, startIndex, stopIndex, configs.fullCtx)
        );
        base.ReportAmbiguity(dfa, D, startIndex, stopIndex, exact, ambigAlts, configs);
    }

    // ---------------------------------------------------------------------

    public DecisionInfo[] GetDecisionInfo() => decisions;

    public DFAState GetCurrentState() => currentState;
}
