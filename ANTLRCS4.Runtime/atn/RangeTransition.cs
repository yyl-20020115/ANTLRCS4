/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime.misc;
using System.Text;

namespace org.antlr.v4.runtime.atn;

public class RangeTransition : Transition {
	public readonly int from;
	public readonly int to;

	public RangeTransition(ATNState target, int from, int to):base(target) {
		this.from = from;
		this.to = to;
	}

	//@Override
	public override int getSerializationType() {
		return RANGE;
	}

    //@Override

    public override IntervalSet label() { return IntervalSet.of(from, to); }

	//@Override
	public override bool matches(int symbol, int minVocabSymbol, int maxVocabSymbol) {
		return symbol >= from && symbol <= to;
	}

	//@Override
	public String ToString() {
		return new StringBuilder("'")
				.Append(char.ConvertFromUtf32(from))
				.Append("'..'")
				.Append(char.ConvertFromUtf32(to))
				.Append("'")
				.toString();
	}
}
