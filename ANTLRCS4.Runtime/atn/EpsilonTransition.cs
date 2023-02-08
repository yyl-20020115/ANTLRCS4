/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

namespace org.antlr.v4.runtime.atn;

public class EpsilonTransition : Transition
{

    private readonly int _outermostPrecedenceReturn;

    public EpsilonTransition(ATNState target, int outermostPrecedenceReturn = -1) : base(target)
        => this._outermostPrecedenceReturn = outermostPrecedenceReturn;

    /**
	 * @return the rule index of a precedence rule for which this transition is
	 * returning from, where the precedence value is 0; otherwise, -1.
	 *
	 * @see ATNConfig#isPrecedenceFilterSuppressed()
	 * @see ParserATNSimulator#applyPrecedenceFilter(ATNConfigSet)
	 * @since 4.4.1
	 */
    public int OutermostPrecedenceReturn => _outermostPrecedenceReturn;

    //@Override
    public override int SerializationType => EPSILON;

    //@Override
    public override bool IsEpsilon => true;
    //@Override
    public override bool Matches(int symbol, int minVocabSymbol, int maxVocabSymbol) => false;

    //@Override
    public override string ToString() => "epsilon";
}
