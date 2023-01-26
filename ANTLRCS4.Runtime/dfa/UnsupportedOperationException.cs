/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */
using System.Runtime.Serialization;

namespace org.antlr.v4.runtime.dfa;

public class UnsupportedOperationException : InvalidOperationException
{
    public UnsupportedOperationException()
    {
    }

    public UnsupportedOperationException(string? message) : base(message)
    {
    }

    public UnsupportedOperationException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

}