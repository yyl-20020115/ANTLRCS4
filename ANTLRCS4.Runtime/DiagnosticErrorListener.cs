/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime.atn;
using org.antlr.v4.runtime.dfa;
using org.antlr.v4.runtime.misc;

namespace org.antlr.v4.runtime;

/**
 * This implementation of {@link ANTLRErrorListener} can be used to identify
 * certain potential correctness and performance problems in grammars. "Reports"
 * are made by calling {@link Parser#notifyErrorListeners} with the appropriate
 * message.
 *
 * <ul>
 * <li><b>Ambiguities</b>: These are cases where more than one path through the
 * grammar can match the input.</li>
 * <li><b>Weak context sensitivity</b>: These are cases where full-context
 * prediction resolved an SLL conflict to a unique alternative which equaled the
 * minimum alternative of the SLL conflict.</li>
 * <li><b>Strong (forced) context sensitivity</b>: These are cases where the
 * full-context prediction resolved an SLL conflict to a unique alternative,
 * <em>and</em> the minimum alternative of the SLL conflict was found to not be
 * a truly viable alternative. Two-stage parsing cannot be used for inputs where
 * this situation occurs.</li>
 * </ul>
 *
 * @author Sam Harwell
 */
public class DiagnosticErrorListener : BaseErrorListener
{
    /**
	 * When {@code true}, only exactly known ambiguities are reported.
	 */
    protected readonly bool exactOnly;

    /**
	 * Initializes a new instance of {@link DiagnosticErrorListener} which only
	 * reports exact ambiguities.
	 */

    /**
	 * Initializes a new instance of {@link DiagnosticErrorListener}, specifying
	 * whether all ambiguities or only exact ambiguities are reported.
	 *
	 * @param exactOnly {@code true} to report only exact ambiguities, otherwise
	 * {@code false} to report all ambiguities.
	 */
    public DiagnosticErrorListener(bool exactOnly = true)
    {
        this.exactOnly = exactOnly;
    }

    ////@Override
    public override void ReportAmbiguity(Parser recognizer,
                                DFA dfa,
                                int startIndex,
                                int stopIndex,
                                bool exact,
                                BitSet ambigAlts,
                                ATNConfigSet configs)
    {
        if (exactOnly && !exact)
        {
            return;
        }

        var decision = GetDecisionDescription(recognizer, dfa);
        var conflictingAlts = GetConflictingAlts(ambigAlts, configs);
        var text = recognizer.TokenStream.GetText(Interval.Of(startIndex, stopIndex));
        var message = $"reportAmbiguity d={decision}: ambigAlts={conflictingAlts}, input='{text}'";
        recognizer.NotifyErrorListeners(message);
    }

    //@Override
    public override void ReportAttemptingFullContext(Parser recognizer,
                                            DFA dfa,
                                            int startIndex,
                                            int stopIndex,
                                            BitSet conflictingAlts,
                                            ATNConfigSet configs)
    {
        var decision = GetDecisionDescription(recognizer, dfa);
        var text = recognizer.TokenStream.GetText(Interval.Of(startIndex, stopIndex));
        var message = $"reportAttemptingFullContext d={decision}, input='{text}'";

        recognizer.NotifyErrorListeners(message);
    }

    //@Override
    public override void ReportContextSensitivity(Parser recognizer,
                                         DFA dfa,
                                         int startIndex,
                                         int stopIndex,
                                         int prediction,
                                         ATNConfigSet configs)
    {
        var decision = GetDecisionDescription(recognizer, dfa);
        var text = recognizer.TokenStream.GetText(Interval.Of(startIndex, stopIndex));
        var message = $"reportContextSensitivity d={decision}, input='{text}'";

        recognizer.NotifyErrorListeners(message);
    }

    protected string GetDecisionDescription(Parser recognizer, DFA dfa)
    {
        int decision = dfa.decision;
        int ruleIndex = dfa.atnStartState.ruleIndex;

        var ruleNames = recognizer.GetRuleNames();
        if (ruleIndex < 0 || ruleIndex >= ruleNames.Length)
        {
            return decision.ToString();
        }

        var ruleName = ruleNames[ruleIndex];
        if (ruleName == null || ruleName.Length == 0)
        {
            return decision.ToString();
        }

        return $"{decision} ({ruleName})";
    }

    /**
	 * Computes the set of conflicting or ambiguous alternatives from a
	 * configuration set, if that information was not already provided by the
	 * parser.
	 *
	 * @param reportedAlts The set of conflicting or ambiguous alternatives, as
	 * reported by the parser.
	 * @param configs The conflicting or ambiguous configuration set.
	 * @return Returns {@code reportedAlts} if it is not {@code null}, otherwise
	 * returns the set of alternatives represented in {@code configs}.
	 */
    protected BitSet GetConflictingAlts(BitSet reportedAlts, ATNConfigSet configs)
    {
        if (reportedAlts != null)
        {
            return reportedAlts;
        }

        var result = new BitSet();
        foreach (var config in configs)
        {
            result.Set(config.alt);
        }

        return result;
    }
}
