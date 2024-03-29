﻿/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */
namespace org.antlr.v4.runtime;


public class MismatchedTokenException : RecognitionException
{
    public readonly int CharValue;
    internal int expecting;

    public MismatchedTokenException(int CharValue, IntStream input) : base(null, input, null) { this.CharValue = CharValue; }
}