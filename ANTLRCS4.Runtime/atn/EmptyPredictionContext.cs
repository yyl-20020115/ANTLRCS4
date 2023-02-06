/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

namespace org.antlr.v4.runtime.atn;

public class EmptyPredictionContext : SingletonPredictionContext
{
    /**
	 * Represents {@code $} in local context prediction, which means wildcard.
	 * {@code *+x = *}.
	 */
    public static readonly EmptyPredictionContext Instance = new();

    private EmptyPredictionContext() : base(null, EMPTY_RETURN_STATE)
    {
        ;
    }

    //@Override
    public bool IsEmpty() => true;

    //@Override
    public int Size => 1;

    //@Override
    public PredictionContext GetParent(int index)
    {
        return null;
    }

    //@Override
    public int GetReturnState(int index) => returnState;

    //@Override
    public override bool Equals(object? o)
    {
        return this == o;
    }

    //@Override
    public override string ToString() => "$";
}
