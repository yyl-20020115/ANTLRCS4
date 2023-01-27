﻿// $ANTLR 3.5.3 org\\antlr\\v4\\parse\\ActionSplitter.g 2023-01-27 22:27:34

using org.antlr.v4.runtime;

namespace org.antlr.v4.parse;

public class EarlyExitException :RecognitionException
{
    private int v;
    private IntStream input;

    public EarlyExitException(int v, IntStream input) : base(null, input, null)
    {
        this.v = v;
        this.input = input;
    }
}