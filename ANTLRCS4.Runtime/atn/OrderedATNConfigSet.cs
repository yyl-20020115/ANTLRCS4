/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime.misc;

namespace org.antlr.v4.runtime.atn;

/**
 *
 * @author Sam Harwell
 */
public class OrderedATNConfigSet : ATNConfigSet
{

    public OrderedATNConfigSet()
    {
        this.configLookup = new LexerConfigHashSet();
    }

    public class LexerConfigHashSet : AbstractConfigHashSet
    {
        public LexerConfigHashSet()
            : base(TEqualityComparator<ATNConfig>.INSTANCE) { }
    }
}
