/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using System.Text;
using ANTLRCS4.Runtime;
using org.antlr.v4.runtime.dfa;

namespace org.antlr.v4.runtime.misc;

/** {@link Set} implementation with closed hashing (open addressing). */
public class Array2DHashSet<T> : HashSet<T>
{
    public static readonly int INITAL_CAPACITY = 16; // must be power of 2
    public static readonly int INITAL_BUCKET_CAPACITY = 8;
    public static readonly double LOAD_FACTOR = 0.75;
    protected readonly AbstractEqualityComparator<T> comparator;

    protected T[][] buckets;

    /** How many elements in set */
    protected int n = 0;
    protected int currentPrime = 1; // jump by 4 primes each expand or whatever

    /** when to expand */
    protected int threshold;
    protected readonly int initialCapacity;
    protected readonly int initialBucketCapacity;

    public Array2DHashSet() 
        : this(null, INITAL_CAPACITY, INITAL_BUCKET_CAPACITY) { }

    public Array2DHashSet(AbstractEqualityComparator<T> comparator) 
        : this(comparator, INITAL_CAPACITY, INITAL_BUCKET_CAPACITY) { }

    public Array2DHashSet(AbstractEqualityComparator<T> comparator, int initialCapacity, int initialBucketCapacity)
    {
        this.comparator = comparator ?? TEqualityComparator<T>.INSTANCE;
        this.initialCapacity = initialCapacity;
        this.initialBucketCapacity = initialBucketCapacity;
        this.buckets = this.CreateBuckets(initialCapacity);
        this.threshold = (int)Math.Floor(initialCapacity * LOAD_FACTOR);
    }

    /**
	 * Add {@code o} to set if not there; return existing value if already
	 * there. This method performs the same operation as {@link #add} aside from
	 * the return value.
	 */
    public virtual T GetOrAdd(T o)
    {
        if (n > threshold) Expand();
        return GetOrAddImpl(o);
    }

    protected virtual T GetOrAddImpl(T o)
    {
        var b = GetBucket(o);
        var bucket = buckets[b];

        // NEW BUCKET
        if (bucket == null)
        {
            bucket = CreateBucket(initialBucketCapacity);
            bucket[0] = o;
            buckets[b] = bucket;
            n++;
            return o;
        }

        // LOOK FOR IT IN BUCKET
        for (int i = 0; i < bucket.Length; i++)
        {
            var existing = bucket[i];
            if (existing == null)
            { // empty slot; not there, add.
                bucket[i] = o;
                n++;
                return o;
            }
            if (comparator.Equals(existing, o)) return existing; // found existing, quit
        }

        // FULL BUCKET, expand and add to end
        int oldLength = bucket.Length;
        bucket = Arrays.CopyOf(bucket, bucket.Length * 2);
        buckets[b] = bucket;
        bucket[oldLength] = o; // add to end
        n++;
        return o;
    }

    public virtual T Get(T o)
    {
        if (o == null) return o;
        int b = GetBucket(o);
        var bucket = buckets[b];
        if (bucket == null) return default; // no bucket
        foreach (var e in bucket)
        {
            if (e == null) return default; // empty slot; not there
            if (comparator.Equals(e, o)) return e;
        }
        return default;
    }

    protected virtual int GetBucket(T o)
    {
        int hash = comparator.GetHashCode(o);
        int b = hash & (buckets.Length - 1); // assumes len is power of 2
        return b;
    }

    
    public override int GetHashCode()
    {
        int hash = MurmurHash.Initialize();
        foreach (var bucket in buckets)
        {
            if (bucket == null) continue;
            foreach (var o in bucket)
            {
                if (o == null) break;
                hash = MurmurHash.Update(hash, comparator.GetHashCode(o));
            }
        }

        hash = MurmurHash.Finish(hash, Count);
        return hash;
    }

    
    public override bool Equals(object? o)
    {
        if (o == this) return true;
        if (o is not Array2DHashSet<T> other) return false;
        if (other.Count != Count) return false;
        bool same = this.ContainsAll(other);
        return same;
    }

    protected virtual void Expand()
    {
        var old = buckets;
        currentPrime += 4;
        int newCapacity = buckets.Length * 2;
        var newTable = CreateBuckets(newCapacity);
        var newBucketLengths = new int[newTable.Length];
        buckets = newTable;
        threshold = (int)(newCapacity * LOAD_FACTOR);
        //		Console.Out.WriteLine("new size="+newCapacity+", thres="+threshold);
        // rehash all existing entries
        int oldSize = Count;
        foreach (var bucket in old)
        {
            if (bucket == null) continue;
            foreach (T o in bucket)
            {
                if (o == null) break;
                int b = GetBucket(o);
                int bucketLength = newBucketLengths[b];
                T[] newBucket;
                if (bucketLength == 0)
                {
                    // new bucket
                    newTable[b] = newBucket = CreateBucket(initialBucketCapacity);
                }
                else
                {
                    newBucket = newTable[b];
                    if (bucketLength == newBucket.Length)
                        // expand
                        newTable[b] = newBucket = Arrays.CopyOf(newBucket, newBucket.Length * 2);
                }

                newBucket[bucketLength] = o;
                newBucketLengths[b]++;
            }
        }

        //assert n == oldSize;
    }

    
    public new virtual bool Add(T t)
    {
        var existing = GetOrAdd(t);
        return existing.Equals(t);
    }

    
    public new virtual int Count => n;

    
    public virtual bool IsEmpty => n == 0;

    
    public virtual bool Contains(object o) => ContainsFast(AsElementType(o));

    public virtual bool ContainsFast(T o) => o != null && Get(o) != null;

    
    public virtual Iterator<T> Iterator() 
        => new SetIterator(this, this.ToArray());

    
    public virtual T[] ToArray()
    {
        int i = 0;
        var a = CreateBucket(Count);
        foreach (var bucket in buckets)
        {
            if (bucket == null) continue;
            foreach (T o in bucket)
            {
                if (o == null) break;
                a[i++] = o;
            }
        }

        return a;
    }

    
    public virtual T[] ToArray(T[] a = null)
    {
        a ??= new T[Count];
        if (a.Length < Count)
            a = Arrays.CopyOf(a, Count);

        int i = 0;
        foreach (var bucket in buckets)
        {
            if (bucket == null) continue;
            foreach (var targetElement in bucket)
            {
                if (targetElement == null) break;
                //@SuppressWarnings("unchecked") // array store will check this
                a[i++] = targetElement;
            }
        }
        return a;
    }

    
    public virtual bool Remove(object o) 
        => RemoveFast(AsElementType(o));

    public virtual bool RemoveFast(T obj)
    {
        if (obj == null) return false;
        int b = GetBucket(obj);
        var bucket = buckets[b];
        if (bucket == null) return false;
        for (int i = 0; i < bucket.Length; i++)
        {
            var e = bucket[i];
            // empty slot; not there
            if (e == null) return false;
            if (comparator.Equals(e, obj))
            {          // found it
                       // shift all elements to the right down one
                Array.Copy(bucket, i + 1, bucket, i, bucket.Length - i - 1);
                bucket[^1] = default;
                n--;
                return true;
            }
        }

        return false;
    }

    
    public virtual bool ContainsAll(ICollection<T> collection)
    {
        if (collection is Array2DHashSet<T> s)
        {
            foreach (var bucket in s.buckets)
            {
                if (bucket == null) continue;
                foreach (var o in bucket)
                {
                    if (o == null) break;
                    if (!this.ContainsFast(AsElementType(o))) return false;
                }
            }
        }
        else
        {
            foreach (var o in collection)
                if (!this.ContainsFast(AsElementType(o))) return false;
        }
        return true;
    }

    
    public virtual bool AddAll(ICollection<T> c)
    {
        var changed = false;
        foreach (var o in c)
        {
            var existing = GetOrAdd(o);
            if (!existing.Equals(o)) changed = true;
        }
        return changed;
    }

    
    public virtual bool RetainAll(ICollection<T> c)
    {
        int newsize = 0;
        foreach (var bucket in buckets)
        {
            if (bucket == null) continue;
            int i;
            int j;
            for (i = 0, j = 0; i < bucket.Length; i++)
            {
                if (bucket[i] == null) break;
                if (!c.Contains(bucket[i])) continue;
                // removed

                // keep
                if (i != j) bucket[j] = bucket[i];

                j++;
                newsize++;
            }

            newsize += j;
            while (j < i)
            {
                bucket[j] = default;
                j++;
            }
        }

        var changed = newsize != n;
        n = newsize;
        return changed;
    }

    
    public virtual bool RemoveAll(ICollection<T> c)
    {
        var changed = false;
        foreach (var o in c)
            changed |= RemoveFast(AsElementType(o));
        return changed;
    }

    
    public new virtual void Clear()
    {
        n = 0;
        buckets = CreateBuckets(this.initialCapacity);
        threshold = (int)Math.Floor(this.initialCapacity * LOAD_FACTOR);
    }

    
    public override string ToString()
    {
        if (Count == 0) return "{}";
        var buffer = new StringBuilder();
        buffer.Append('{');
        bool first = true;
        foreach (T[] bucket in buckets)
        {
            if (bucket == null) continue;
            foreach (T o in bucket)
            {
                if (o == null) break;
                if (first) first = false;
                else buffer.Append(", ");
                buffer.Append(o.ToString());
            }
        }
        buffer.Append('}');
        return buffer.ToString();
    }

    public virtual string ToTableString()
    {
        var buffer = new StringBuilder();
        foreach (var bucket in buckets)
        {
            if (bucket == null)
            {
                buffer.Append("null\n");
                continue;
            }
            buffer.Append('[');
            var first = true;
            foreach (var o in bucket)
            {
                if (first) first = false;
                else buffer.Append(' ');
                if (o == null) buffer.Append('_');
                else buffer.Append(o.ToString());
            }
            buffer.Append("]\n");
        }
        return buffer.ToString();
    }

    /**
	 * Return {@code o} as an instance of the element type {@code T}. If
	 * {@code o} is non-null but known to not be an instance of {@code T}, this
	 * method returns {@code null}. The base implementation does not perform any
	 * type checks; override this method to provide strong type checks for the
	 * {@link #contains} and {@link #remove} methods to ensure the arguments to
	 * the {@link EqualityComparator} for the set always have the expected
	 * types.
	 *
	 * @param o the object to try and cast to the element type of the set
	 * @return {@code o} if it could be an instance of {@code T}, otherwise
	 * {@code null}.
	 */
    //@SuppressWarnings("unchecked")
    protected virtual T AsElementType(object o) => (T)o;

    /**
	 * Return an array of {@code T[]} with Length {@code capacity}.
	 *
	 * @param capacity the Length of the array to return
	 * @return the newly constructed array
	 */
    //@SuppressWarnings("unchecked")
    protected virtual T[][] CreateBuckets(int capacity) => new T[capacity][];

    /**
	 * Return an array of {@code T} with Length {@code capacity}.
	 *
	 * @param capacity the Length of the array to return
	 * @return the newly constructed array
	 */
    //@SuppressWarnings("unchecked")
    protected virtual T[] CreateBucket(int capacity) => new T[capacity];

    protected class SetIterator : Iterator<T>
    {
        readonly Array2DHashSet<T> values;
        readonly T[] data;
        int nextIndex = 0;
        bool removed = true;

        public SetIterator(Array2DHashSet<T> values, T[] data)
        {
            this.data = (T[])data.Clone();
            this.values = values;
        }

        
        public virtual bool HasNext() 
            => nextIndex < data.Length;

        
        public virtual T Next()
        {
            if (!HasNext())
                throw new NoSuchElementException();

            removed = false;
            return data[nextIndex++];
        }

        
        public virtual void Remove()
        {
            if (removed)
                throw new IllegalStateException();

            this.values.Remove(data[nextIndex - 1]);
            removed = true;
        }
    }
}
