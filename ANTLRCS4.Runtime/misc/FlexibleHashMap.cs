/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime.dfa;
using System.Text;

namespace org.antlr.v4.runtime.misc;

/** A limited map (many unsupported operations) that lets me use
 *  varying hashCode/equals.
 */
public class FlexibleHashMap<K, V> : Dictionary<K, V>
{
    public static readonly int INITAL_CAPACITY = 16; // must be power of 2
    public static readonly int INITAL_BUCKET_CAPACITY = 8;
    public static readonly double LOAD_FACTOR = 0.75;

    public class Entry<K, V>
    {
        public readonly K key;
        public V value;

        public Entry(K key, V value) { this.key = key; this.value = value; }

        public override string ToString() => key.ToString() + ":" + value.ToString();
    }


    protected readonly AbstractEqualityComparator<K> comparator;

    protected LinkedList<Entry<K, V>>[] buckets;

    /** How many elements in set */
    protected int n = 0;

    protected int currentPrime = 1; // jump by 4 primes each expand or whatever

    /** when to expand */
    protected int threshold;
    protected readonly int initialCapacity;
    protected readonly int initialBucketCapacity;

    public FlexibleHashMap() : this(null, INITAL_CAPACITY, INITAL_BUCKET_CAPACITY)
    {
    }

    public FlexibleHashMap(AbstractEqualityComparator<K> comparator) : this(comparator, INITAL_CAPACITY, INITAL_BUCKET_CAPACITY)
    {
    }

    public FlexibleHashMap(AbstractEqualityComparator<K> comparator, int initialCapacity, int initialBucketCapacity)
    {
        this.comparator = comparator ?? TEqualityComparator<K>.INSTANCE;
        this.initialCapacity = initialCapacity;
        this.initialBucketCapacity = initialBucketCapacity;
        this.threshold = (int)Math.Floor(initialCapacity * LOAD_FACTOR);
        this.buckets = CreateEntryListArray<K, V>(initialBucketCapacity);
    }

    private static LinkedList<Entry<K, V>>[] CreateEntryListArray<K, V>(int length)
    {
        LinkedList<Entry<K, V>>[] result = new LinkedList<Entry<K, V>>[length];
        return result;
    }

    protected int GetBucket(K key)
    {
        int hash = comparator.GetHashCode(key);
        int b = hash & (buckets.Length - 1); // assumes len is power of 2
        return b;
    }

    public V Get(object key)
    {
        K typedKey = (K)key;
        if (key == null) return default;
        int b = GetBucket(typedKey);
        LinkedList<Entry<K, V>> bucket = buckets[b];
        if (bucket == null) return default; // no bucket
        foreach (Entry<K, V> e in bucket)
        {
            if (comparator.Equals(e.key, typedKey))
            {
                return e.value;
            }
        }
        return default;
    }

    public V Put(K key, V value)
    {
        if (key == null) return default;
        if (n > threshold) Expand();
        int b = GetBucket(key);
        LinkedList<Entry<K, V>> bucket = buckets[b];
        if (bucket == null)
        {
            bucket = buckets[b] = new LinkedList<Entry<K, V>>();
        }
        foreach (Entry<K, V> e in bucket)
        {
            if (comparator.Equals(e.key, key))
            {
                V prev = e.value;
                e.value = value;
                n++;
                return prev;
            }
        }
        // not there
        bucket.AddLast(new Entry<K, V>(key, value));
        n++;
        return default;
    }

    public V Remove(object key)
    {
        throw new UnsupportedOperationException();
    }

    public void PutAll(Dictionary<K, V> m)
    {
        throw new UnsupportedOperationException();
    }

    public HashSet<K> KeySet()
    {
        throw new UnsupportedOperationException();
    }

    public ICollection<V> Values()
    {
        List<V> a = new(Count);
        foreach (LinkedList<Entry<K, V>> bucket in buckets)
        {
            if (bucket == null) continue;
            foreach (Entry<K, V> e in bucket)
            {
                a.Add(e.value);
            }
        }
        return a;
    }

    //@Override
    //public HashSet<Dictionary<K,V>.Entry<K, V>> entrySet() {
    //	throw new UnsupportedOperationException();
    //}

    public bool ContainsKey(object key) => Get(key) != null;

    public bool ContainsValue(object value) => throw new UnsupportedOperationException();

    public override int GetHashCode()
    {
        int hash = MurmurHash.Initialize();
        foreach (LinkedList<Entry<K, V>> bucket in buckets)
        {
            if (bucket == null) continue;
            foreach (Entry<K, V> e in bucket)
            {
                if (e == null) break;
                hash = MurmurHash.Update(hash, comparator.GetHashCode(e.key));
            }
        }

        hash = MurmurHash.Finish(hash, Count);
        return hash;
    }

    public override bool Equals(object? o)
    {
        throw new UnsupportedOperationException();
    }

    protected void Expand()
    {
        LinkedList<Entry<K, V>>[] old = buckets;
        currentPrime += 4;
        int newCapacity = buckets.Length * 2;
        LinkedList<Entry<K, V>>[] newTable = CreateEntryListArray<K, V>(newCapacity);
        buckets = newTable;
        threshold = (int)(newCapacity * LOAD_FACTOR);
        //		Console.WriteLine("new size="+newCapacity+", thres="+threshold);
        // rehash all existing entries
        int oldSize = Count;
        foreach (LinkedList<Entry<K, V>> bucket in old)
        {
            if (bucket == null) continue;
            foreach (Entry<K, V> e in bucket)
            {
                if (e == null) break;
                Put(e.key, e.value);
            }
        }
        n = oldSize;
    }

    //@Override
    public new int Count => n;

    //@Override
    public bool IsEmpty => n == 0;

    public new void Clear()
    {
        buckets = CreateEntryListArray<K, V>(this.initialCapacity);
        n = 0;
        threshold = (int)Math.Floor(this.initialCapacity * LOAD_FACTOR);
    }

    public override string ToString()
    {
        if (Count == 0) return "{}";

        var buffer = new StringBuilder();
        buffer.Append('{');
        bool first = true;
        foreach (LinkedList<Entry<K, V>> bucket in buckets)
        {
            if (bucket == null) continue;
            foreach (Entry<K, V> e in bucket)
            {
                if (e == null) break;
                if (first) first = false;
                else buffer.Append(", ");
                buffer.Append(e.ToString());
            }
        }
        buffer.Append('}');
        return buffer.ToString();
    }

    public string ToTableString()
    {
        var buffer = new StringBuilder();
        foreach (LinkedList<Entry<K, V>> bucket in buckets)
        {
            if (bucket == null)
            {
                buffer.Append("null\n");
                continue;
            }
            buffer.Append('[');
            bool first = true;
            foreach (Entry<K, V> e in bucket)
            {
                if (first) first = false;
                else buffer.Append(" ");
                if (e == null) buffer.Append("_");
                else buffer.Append(e.ToString());
            }
            buffer.Append("]\n");
        }
        return buffer.ToString();
    }

    public static void TestMain(string[] args)
    {
        FlexibleHashMap<string, int> map = new();
        map.Put("hi", 1);
        map.Put("mom", 2);
        map.Put("foo", 3);
        map.Put("ach", 4);
        map.Put("cbba", 5);
        map.Put("d", 6);
        map.Put("edf", 7);
        map.Put("mom", 8);
        map.Put("hi", 9);
        Console.WriteLine(map);
        Console.WriteLine(map.ToTableString());
    }
}
