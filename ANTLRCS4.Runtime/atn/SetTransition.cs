/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime.misc;

namespace org.antlr.v4.runtime.atn;


/** A transition containing a set of values. */
public class SetTransition : Transition
{
    public readonly IntervalSet label;

    // TODO (sam): should we really allow null here?
    public SetTransition(ATNState target, IntervalSet set) : base(target)
    {
        this.label = set ??= IntervalSet.Of(Token.INVALID_TYPE);
    }

    //@Override
    public override int SerializationType => SET;

    //@Override

    public override IntervalSet Label => label;
    //@Override
    public override bool Matches(int symbol, int minVocabSymbol, int maxVocabSymbol) => label.Contains(symbol);

    //@Override

    public override string ToString() => label.ToString();
}
