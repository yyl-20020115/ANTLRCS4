/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */
using org.antlr.v4.runtime.atn;

namespace org.antlr.v4.runtime;


/** A semantic predicate failed during validation.  Validation of predicates
 *  occurs when normally parsing the alternative just like matching a token.
 *  Disambiguating predicate evaluation occurs when we test a predicate during
 *  prediction.
 */
public class FailedPredicateException : RecognitionException {
	private readonly int ruleIndex;
	private readonly int predicateIndex;
	private readonly String predicate;
    internal string predicateText;
    internal string ruleName;
    public FailedPredicateException(IntStream input,
                                    String predicate = null,
                                    String message = null)
		: base(formatMessage(predicate, message), null, null,null)
    {

    }
    public FailedPredicateException(Parser recognizer,
									String predicate = null,
									String message = null)
		: base(formatMessage(predicate, message), recognizer, recognizer.getInputStream(), recognizer.GetCtx())
    {
		ATNState s = recognizer.getInterpreter().atn.states[(recognizer.getState())];

		AbstractPredicateTransition trans = (AbstractPredicateTransition)s.Transition(0);
		if (trans is PredicateTransition) {
			this.ruleIndex = ((PredicateTransition)trans).ruleIndex;
			this.predicateIndex = ((PredicateTransition)trans).predIndex;
		}
		else {
			this.ruleIndex = 0;
			this.predicateIndex = 0;
		}

		this.predicate = predicate;
		this.setOffendingToken(recognizer.getCurrentToken());
	}

	public int getRuleIndex() {
		return ruleIndex;
	}

	public int getPredIndex() {
		return predicateIndex;
	}


	public String getPredicate() {
		return predicate;
	}


	private static String formatMessage(String predicate, String message) {
		if (message != null) {
			return message;
		}

		return $"failed predicate: {{{predicate}}}?";
	}
}
