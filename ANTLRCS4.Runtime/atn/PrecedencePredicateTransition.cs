/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

namespace org.antlr.v4.runtime.atn;

/**
 *
 * @author Sam Harwell
 */
public class PrecedencePredicateTransition : AbstractPredicateTransition
{
    public readonly int precedence;

    public PrecedencePredicateTransition(ATNState target, int precedence) : base(target) => this.precedence = precedence;

    public override int SerializationType => PRECEDENCE;

    public override bool IsEpsilon => true;

    public override bool Matches(int symbol, int minVocabSymbol, int maxVocabSymbol) => false;

    public SemanticContext.PrecedencePredicate GetPredicate() => new(precedence);

    public override string ToString() => $"{precedence} >= _p";

}
