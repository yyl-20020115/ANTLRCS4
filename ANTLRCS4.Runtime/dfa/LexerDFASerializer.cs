/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using System.Text;

namespace org.antlr.v4.runtime.dfa;

public class LexerDFASerializer : DFASerializer {
	public LexerDFASerializer(DFA dfa) : base(dfa, VocabularyImpl.EMPTY_VOCABULARY)
    {
	}

	protected override String getEdgeLabel(int i) {
		return new StringBuilder("'")
				.Append(char.ConvertFromUtf32(i))//.appendCodePoint(i)
				.Append("'")
				.ToString();
	}
}
