/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

namespace org.antlr.v4.misc;

/** I need the get-element-i functionality so I'm subclassing
 *  LinkedHashMap.
 */
public class OrderedHashMap<K, V> : Dictionary<K, V> where K : notnull
{
    /** Track the elements as they are added to the set */
    protected List<K> elements = new();

    public K GetKey(int i) => elements[(i)];

    public V GetElement(int i) => this.TryGetValue((elements[(i)]), out var r) ? r : default;

    public V Put(K key, V value)
    {
        elements.Add(key);
        return base[key] = value;
    }

    public void PutAll(Dictionary<K, V> m)
    {
        foreach (var entry in m)
        {
            Put(entry.Key, entry.Value);
        }
    }

    public new V Remove(K key)
    {
        if (this.TryGetValue(key, out var v))
        {
            elements.Remove(key);
            base.Remove(key);
            return v;
        }
        return default;
    }

    public new void Clear()
    {
        elements.Clear();
        base.Clear();
    }
}
