﻿/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

namespace org.antlr.v4.runtime.misc;

public class NoSuchElementException : Exception
{
    public NoSuchElementException()
    {
    }

    public NoSuchElementException(string? message) : base(message)
    {
    }

    public NoSuchElementException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

}