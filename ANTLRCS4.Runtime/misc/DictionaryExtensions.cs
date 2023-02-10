﻿/* Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */
namespace org.antlr.v4.runtime.misc;

using System.Collections.Generic;

public static class DictionaryExtensions
{
    public static TValue Get<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
        where TValue : class => dictionary.TryGetValue(key, out var value) ? value : null;

    public static TValue Put<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        where TValue : class
    {
        if (!dictionary.TryGetValue(key, out var previous))
            previous = null;

        dictionary[key] = value;
        return previous;
    }
    public static IDictionary<TKey, TValue> AddRange<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, IDictionary<TKey, TValue> other)
        where TValue : class
    {
        foreach (var p in other)
            dictionary[p.Key] = p.Value;
        return dictionary;
    }
}
