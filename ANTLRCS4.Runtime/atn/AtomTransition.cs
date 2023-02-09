/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime.misc;

namespace org.antlr.v4.runtime.atn;

/** TODO: make all transitions sets? no, should remove set edges */
public class AtomTransition : Transition
{
    /** The token type or character value; or, signifies special label. */
    public readonly int _label;

    public AtomTransition(ATNState target, int label) : base(target) => this._label = label;

    public override int SerializationType => ATOM;

    
    public override IntervalSet Label => IntervalSet.Of(_label);
    
    public override bool Matches(int symbol, int minVocabSymbol, int maxVocabSymbol) 
        => _label == symbol;

    public override string ToString() => this._label.ToString();
}
