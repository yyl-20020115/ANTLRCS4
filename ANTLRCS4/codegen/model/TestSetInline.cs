/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime.misc;
using org.antlr.v4.tool.ast;

namespace org.antlr.v4.codegen.model;

/** */
public class TestSetInline : SrcOp {
	public readonly int bitsetWordSize;
	public readonly String varName;
	public readonly Bitset[] bitsets;

	public TestSetInline(OutputModelFactory factory, GrammarAST ast, IntervalSet set, int wordSize) {
		super(factory, ast);
		bitsetWordSize = wordSize;
		Bitset[] withZeroOffset = createBitsets(factory, set, wordSize, true);
		Bitset[] withoutZeroOffset = createBitsets(factory, set, wordSize, false);
		this.bitsets = withZeroOffset.Length <= withoutZeroOffset.Length ? withZeroOffset : withoutZeroOffset;
		this.varName = "_la";
	}

	private static Bitset[] createBitsets(OutputModelFactory factory,
										  IntervalSet set,
										  int wordSize,
										  bool useZeroOffset) {
		List<Bitset> bitsetList = new ();
		Target target = factory.getGenerator().getTarget();
		Bitset current = null;
		for (int ttype : set.toArray()) {
			if (current == null || ttype > (current.shift + wordSize-1)) {
				int shift;
				if (useZeroOffset && ttype >= 0 && ttype < wordSize-1) {
					shift = 0;
				}
				else {
					shift = ttype;
				}
				current = new Bitset(shift);
				bitsetList.add(current);
			}

			current.addToken(ttype, target.getTokenTypeAsTargetLabel(factory.getGrammar(), ttype));
		}

		return bitsetList.toArray(new Bitset[0]);
	}

	public class Bitset {
		public readonly int shift;
		private readonly List<TokenInfo> tokens = new ();
		private long calculated;

		public Bitset(int shift) {
			this.shift = shift;
		}

		public void addToken(int type, String name) {
			tokens.add(new TokenInfo(type, name));
			calculated |= 1L << (type - shift);
		}

		public List<TokenInfo> getTokens() {
			return tokens;
		}

		public long getCalculated() {
			return calculated;
		}
	}
}
