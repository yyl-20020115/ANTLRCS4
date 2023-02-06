/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

namespace org.antlr.v4.runtime.atn;

public class WildcardTransition : Transition {
	public WildcardTransition(ATNState target):base(target) {  }

    //@Override
    public override int SerializationType => WILDCARD;

    //@Override
    public override bool Matches(int symbol, int minVocabSymbol, int maxVocabSymbol) {
		return symbol >= minVocabSymbol && symbol <= maxVocabSymbol;
	}

	//@Override
	public override String ToString() {
		return ".";
	}
}
