/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */
namespace org.antlr.v4.runtime;

public class MismatchedRangeException: RecognitionException
{
    private int a;
    private int b;
    private CharStream input;

    public MismatchedRangeException(int a, int b, CharStream input)
        :base(null,input,null)
    {
        this.a = a;
        this.b = b;
        this.input = input;
    }
}