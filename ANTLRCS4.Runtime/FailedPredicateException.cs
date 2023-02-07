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
public class FailedPredicateException : RecognitionException
{
    private readonly int ruleIndex;
    private readonly int predicateIndex;
    private readonly string predicate;
    internal string predicateText;
    internal string ruleName;
    public FailedPredicateException(IntStream input,
                                    string predicate = null,
                                    string message = null)
        : base(FormatMessage(predicate, message), null, null, null)
    {

    }
    public FailedPredicateException(Parser recognizer,
                                    string predicate = null,
                                    string message = null)
        : base(FormatMessage(predicate, message), recognizer, recognizer.InputStream, recognizer.GetCtx())
    {
        ATNState s = recognizer.GetInterpreter().atn.states[(recognizer.GetState())];

        AbstractPredicateTransition trans = (AbstractPredicateTransition)s.Transition(0);
        if (trans is PredicateTransition transition)
        {
            this.ruleIndex = transition.ruleIndex;
            this.predicateIndex = transition.predIndex;
        }
        else
        {
            this.ruleIndex = 0;
            this.predicateIndex = 0;
        }

        this.predicate = predicate;
        this.setOffendingToken(recognizer.getCurrentToken());
    }

    public int GetRuleIndex()
    {
        return ruleIndex;
    }

    public int GetPredIndex()
    {
        return predicateIndex;
    }


    public string GetPredicate()
    {
        return predicate;
    }


    private static string FormatMessage(string predicate, string message)
    {
        if (message != null)
        {
            return message;
        }

        return $"failed predicate: {{{predicate}}}?";
    }
}
