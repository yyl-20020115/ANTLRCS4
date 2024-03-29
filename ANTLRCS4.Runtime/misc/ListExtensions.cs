﻿/* Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */
namespace org.antlr.v4.runtime.misc;

public static class ListExtensions
{
    public static T Set<T>(this IList<T> list, int index, T value)
        where T : class
    {
        T previous = list[index];
        list[index] = value;
        return previous;
    }
}
