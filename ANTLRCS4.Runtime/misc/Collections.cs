/* Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using System.Collections.ObjectModel;
namespace org.antlr.v4.runtime.misc;

public static class Collections
{
    public static T[] EmptyList<T>() => EmptyListImpl<T>.Instance;

    public static ReadOnlyDictionary<TKey, TValue> EmptyMap<TKey, TValue>() => EmptyMapImpl<TKey, TValue>.Instance;

    public static ReadOnlyCollection<T> SingletonList<T>(T item) => new ReadOnlyCollection<T>(new T[] { item });

    public static ReadOnlyDictionary<TKey, TValue> SingletonMap<TKey, TValue>(TKey key, TValue value) => new ReadOnlyDictionary<TKey, TValue>(new Dictionary<TKey, TValue> { { key, value } });

    private static class EmptyListImpl<T>
    {
        public static readonly T[] Instance = Array.Empty<T>();
    }

    private static class EmptyMapImpl<TKey, TValue>
    {
        public static readonly ReadOnlyDictionary<TKey, TValue> Instance =
            new (new Dictionary<TKey, TValue>());
    }
}
