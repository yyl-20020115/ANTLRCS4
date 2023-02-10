/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

namespace org.antlr.v4.runtime.misc;

/** Sometimes we need to map a key to a value but key is two pieces of data.
 *  This nested hash table saves creating a single key each time we access
 *  map; avoids mem creation.
 */
public class DoubleKeyMap<Key1, Key2, Value> where Key1:notnull where Key2:notnull
{
    readonly Dictionary<Key1, Dictionary<Key2, Value>> data = new();

    public Value Put(Key1 k1, Key2 k2, Value v)
    {
        Value prev = default;
        if (!data.TryGetValue(k1, out var data2))
        {
            data2 = new();
            data.Add(k1, data2);
        }
        else
        {
            if (!data2.TryGetValue(k2, out prev))
            {

            }
        }
        data2.Add(k2, v);
        return prev;
    }

    public Value Get(Key1 k1, Key2 k2)
    {

        if (!data.TryGetValue(k1, out var data2)) return default;
        return data2.TryGetValue(k2, out var ret) ? ret : default;
    }

    public Dictionary<Key2, Value> Get(Key1 k1) => data.TryGetValue(k1, out var d) ? d : null;

    /** Get all values associated with primary key */
    public ICollection<Value> Values(Key1 k1) => !data.TryGetValue(k1, out var data2) ? null : (ICollection<Value>)data2.Values;

    /** get all primary keys */
    public HashSet<Key1> KeySet() => data.Keys.ToHashSet();

    /** get all secondary keys associated with a primary key */
    public HashSet<Key2> KeySet(Key1 k1)
        => data.TryGetValue(k1, out var data2) ? data2.Keys.ToHashSet() : (HashSet<Key2>?)null;
}
