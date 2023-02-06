/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime.misc;
using org.antlr.v4.tool.ast;

namespace org.antlr.v4.codegen.model;

/** */
public class TestSetInline : SrcOp
{
    public readonly int bitsetWordSize;
    public readonly string varName;
    public readonly Bitset[] bitsets;

    public TestSetInline(OutputModelFactory factory, GrammarAST ast, IntervalSet set, int wordSize) : base(factory, ast)
    {
        bitsetWordSize = wordSize;
        var withZeroOffset = CreateBitsets(factory, set, wordSize, true);
        var withoutZeroOffset = CreateBitsets(factory, set, wordSize, false);
        this.bitsets = withZeroOffset.Length <= withoutZeroOffset.Length ? withZeroOffset : withoutZeroOffset;
        this.varName = "_la";
    }

    private static Bitset[] CreateBitsets(OutputModelFactory factory,
                                          IntervalSet set,
                                          int wordSize,
                                          bool useZeroOffset)
    {
        List<Bitset> bitsetList = new();
        var target = factory.GetGenerator().Target;
        Bitset current = null;
        foreach (int ttype in set.ToArray())
        {
            if (current == null || ttype > (current.shift + wordSize - 1))
            {
                int shift;
                if (useZeroOffset && ttype >= 0 && ttype < wordSize - 1)
                {
                    shift = 0;
                }
                else
                {
                    shift = ttype;
                }
                current = new Bitset(shift);
                bitsetList.Add(current);
            }

            current.AddToken(ttype, target.GetTokenTypeAsTargetLabel(factory.GetGrammar(), ttype));
        }

        return bitsetList.ToArray();
    }

    public class Bitset
    {
        public readonly int shift;
        private readonly List<TokenInfo> tokens = new();
        private long calculated;

        public Bitset(int shift)
        {
            this.shift = shift;
        }

        public void AddToken(int type, String name)
        {
            tokens.Add(new TokenInfo(type, name));
            calculated |= 1L << (type - shift);
        }

        public List<TokenInfo> GetTokens() => tokens;

        public long GetCalculated() => calculated;
    }
}
