/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */
namespace org.antlr.v4.runtime.dfa;

public static class Arrays
{
    public static T[] copyOf<T>(T[] array, int length)
    {
        var ret = new T[length];
        Array.Copy(array, ret, Math.Min(length, array.Length));
        return ret;
    }
}