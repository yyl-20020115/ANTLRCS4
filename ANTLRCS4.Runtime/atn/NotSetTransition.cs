/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime.misc;

namespace org.antlr.v4.runtime.atn;


public class NotSetTransition : SetTransition
{
    public NotSetTransition(ATNState target, IntervalSet set)
        : base(target, set) { }

    public override int SerializationType => NOT_SET;

    public override bool Matches(int symbol, int minVocabSymbol, int maxVocabSymbol) 
        => symbol >= minVocabSymbol
            && symbol <= maxVocabSymbol
            && !base.Matches(symbol, minVocabSymbol, maxVocabSymbol);

    public override string ToString() => '~' + base.ToString();
}
