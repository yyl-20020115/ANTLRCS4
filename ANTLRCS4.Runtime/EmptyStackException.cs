/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */
using System.Runtime.Serialization;

namespace org.antlr.v4.runtime;

public class EmptyStackException : Exception
{
    public EmptyStackException()
    {
    }

    public EmptyStackException(string? message) : base(message)
    {
    }

    public EmptyStackException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}