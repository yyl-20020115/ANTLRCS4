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

    private EmptyPredictionContext() : base(null, EMPTY_RETURN_STATE) { }

    public override bool IsEmpty => true;

    public override int Count => 1;

    public override PredictionContext GetParent(int index) => null;

    public override int GetReturnState(int index) => returnState;

    public override bool Equals(object? o) => this == o;

    public override string ToString() => "$";
}
