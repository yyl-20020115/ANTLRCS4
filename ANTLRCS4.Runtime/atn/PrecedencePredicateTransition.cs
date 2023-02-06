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
public class PrecedencePredicateTransition : AbstractPredicateTransition {
	public readonly int precedence;

	public PrecedencePredicateTransition(ATNState target, int precedence):base(target)
    {
		;
		this.precedence = precedence;
	}

    //@Override
    public override int SerializationType => PRECEDENCE;

    //@Override
    public override bool IsEpsilon => true;

    //@Override
    public override bool Matches(int symbol, int minVocabSymbol, int maxVocabSymbol) {
		return false;
	}

	public SemanticContext.PrecedencePredicate getPredicate() {
		return new SemanticContext.PrecedencePredicate(precedence);
	}

	//@Override
	public override String ToString() {
		return precedence + " >= _p";
	}

}
