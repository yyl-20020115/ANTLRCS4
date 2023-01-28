/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using ANTLRCS4.Runtime;
using org.antlr.v4.runtime.dfa;
using System.Text;

namespace org.antlr.v4.runtime.misc;

/** {@link Set} implementation with closed hashing (open addressing). */
public class Array2DHashSet<T> : HashSet<T> {
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

	public Array2DHashSet(): this(null, INITAL_CAPACITY, INITAL_BUCKET_CAPACITY)
    {
	}

	public Array2DHashSet(AbstractEqualityComparator<T> comparator) : this(comparator, INITAL_CAPACITY, INITAL_BUCKET_CAPACITY)
    {
	}

	public Array2DHashSet(AbstractEqualityComparator<T> comparator, int initialCapacity, int initialBucketCapacity) {
		if (comparator == null) {
			comparator = TEqualityComparator<T>.INSTANCE;
		}

		this.comparator = comparator;
		this.initialCapacity = initialCapacity;
		this.initialBucketCapacity = initialBucketCapacity;
		this.buckets = createBuckets(initialCapacity);
		this.threshold = (int)Math.Floor(initialCapacity * LOAD_FACTOR);
	}

	/**
	 * Add {@code o} to set if not there; return existing value if already
	 * there. This method performs the same operation as {@link #add} aside from
	 * the return value.
	 */
	public T getOrAdd(T o) {
		if ( n > threshold ) expand();
		return getOrAddImpl(o);
	}

	protected T getOrAddImpl(T o) {
		int b = getBucket(o);
		T[] bucket = buckets[b];

		// NEW BUCKET
		if ( bucket==null ) {
			bucket = createBucket(initialBucketCapacity);
			bucket[0] = o;
			buckets[b] = bucket;
			n++;
			return o;
		}

		// LOOK FOR IT IN BUCKET
		for (int i=0; i<bucket.Length; i++) {
			T existing = bucket[i];
			if ( existing==null ) { // empty slot; not there, add.
				bucket[i] = o;
				n++;
				return o;
			}
			if ( comparator.Equals(existing, o) ) return existing; // found existing, quit
		}

		// FULL BUCKET, expand and add to end
		int oldLength = bucket.Length;
		bucket = Arrays.CopyOf(bucket, bucket.Length * 2);
		buckets[b] = bucket;
		bucket[oldLength] = o; // add to end
		n++;
		return o;
	}

	public T get(T o) {
		if ( o==null ) return o;
		int b = getBucket(o);
		T[] bucket = buckets[b];
		if ( bucket==null ) return default; // no bucket
		foreach (T e in bucket) {
			if ( e==null ) return default; // empty slot; not there
			if ( comparator.Equals(e, o) ) return e;
		}
		return default;
	}

	protected  int getBucket(T o) {
		int hash = comparator.GetHashCode(o);
		int b = hash & (buckets.Length-1); // assumes len is power of 2
		return b;
	}

	//@Override
	public override int GetHashCode() {
		int hash = MurmurHash.initialize();
		foreach (T[] bucket in buckets) {
			if ( bucket==null ) continue;
			foreach (T o in bucket) {
				if ( o==null ) break;
				hash = MurmurHash.update(hash, comparator.GetHashCode(o));
			}
		}

		hash = MurmurHash.finish(hash, size());
		return hash;
	}

	//@Override
	public bool Equals(Object o) {
		if (o == this) return true;
		if ( !(o is Array2DHashSet<T>) ) return false;
		Array2DHashSet<T> other = (Array2DHashSet<T>)o;
		if ( other.size() != size() ) return false;
		bool same = this.containsAll(other);
		return same;
	}

	protected void expand() {
		T[][] old = buckets;
		currentPrime += 4;
		int newCapacity = buckets.Length * 2;
		T[][] newTable = createBuckets(newCapacity);
		int[] newBucketLengths = new int[newTable.Length];
		buckets = newTable;
		threshold = (int)(newCapacity * LOAD_FACTOR);
//		Console.Out.WriteLine("new size="+newCapacity+", thres="+threshold);
		// rehash all existing entries
		int oldSize = size();
		foreach (T[] bucket in old) {
			if ( bucket==null ) {
				continue;
			}

			foreach (T o in bucket) {
				if ( o==null ) {
					break;
				}

				int b = getBucket(o);
				int bucketLength = newBucketLengths[b];
				T[] newBucket;
				if (bucketLength == 0) {
					// new bucket
					newBucket = createBucket(initialBucketCapacity);
					newTable[b] = newBucket;
				}
				else {
					newBucket = newTable[b];
					if (bucketLength == newBucket.Length) {
						// expand
						newBucket = Arrays.CopyOf(newBucket, newBucket.Length * 2);
						newTable[b] = newBucket;
					}
				}

				newBucket[bucketLength] = o;
				newBucketLengths[b]++;
			}
		}

		//assert n == oldSize;
	}

	//@Override
	public bool add(T t) {
		T existing = getOrAdd(t);
		return existing.Equals(t);
	}

	//@Override
	public int size() {
		return n;
	}

	//@Override
	public bool isEmpty() {
		return n==0;
	}

	//@Override
	public bool contains(Object o) {
		return containsFast(asElementType(o));
	}

	public bool containsFast(T obj) {
		if (obj == null) {
			return false;
		}

		return get(obj) != null;
	}

	//@Override
	public Iterator<T> iterator() {
		return new SetIterator(this,this.toArray());
	}

	//@Override
	public T[] toArray() {
		T[] a = createBucket(size());
		int i = 0;
		foreach (T[] bucket in buckets) {
			if ( bucket==null ) {
				continue;
			}

			foreach (T o in bucket) {
				if ( o==null ) {
					break;
				}

				a[i++] = o;
			}
		}

		return a;
	}

	//@Override
	public T[] toArray(T[] a) {
		if (a.Length < size()) {
			a = Arrays.CopyOf(a, size());
		}

		int i = 0;
		foreach (T[] bucket in buckets) {
			if ( bucket==null ) {
				continue;
			}

			foreach (T o in bucket) {
				if ( o==null ) {
					break;
				}

				//@SuppressWarnings("unchecked") // array store will check this
				T targetElement = (T)o;
				a[i++] = targetElement;
			}
		}
		return a;
	}

	//@Override
	public bool remove(Object o) {
		return removeFast(asElementType(o));
	}

	public bool removeFast(T obj) {
		if (obj == null) {
			return false;
		}

		int b = getBucket(obj);
		T[] bucket = buckets[b];
		if ( bucket==null ) {
			// no bucket
			return false;
		}

		for (int i=0; i<bucket.Length; i++) {
			T e = bucket[i];
			if ( e==null ) {
				// empty slot; not there
				return false;
			}

			if ( comparator.Equals(e, obj) ) {          // found it
				// shift all elements to the right down one
				Array.Copy(bucket, i+1, bucket, i, bucket.Length-i-1);
				bucket[bucket.Length - 1] = default;
				n--;
				return true;
			}
		}

		return false;
	}

	//@Override
	public bool containsAll(ICollection<T> collection) {
		if ( collection is Array2DHashSet<T> ) {
			Array2DHashSet<T> s = (Array2DHashSet<T>)collection;
			foreach (T[] bucket in s.buckets) {
				if ( bucket==null ) continue;
				foreach (Object o in bucket) {
					if ( o==null ) break;
					if ( !this.containsFast(asElementType(o)) ) return false;
				}
			}
		}
		else {
			foreach (Object o in collection) {
				if ( !this.containsFast(asElementType(o)) ) return false;
			}
		}
		return true;
	}

	//@Override
	public bool addAll(ICollection<T> c) {
		bool changed = false;
		foreach (T o in c) {
			T existing = getOrAdd(o);
			if ( !existing.Equals(o) ) changed=true;
		}
		return changed;
	}

	//@Override
	public bool retainAll(ICollection<T> c) {
		int newsize = 0;
		foreach (T[] bucket in buckets) {
			if (bucket == null) {
				continue;
			}

			int i;
			int j;
			for (i = 0, j = 0; i < bucket.Length; i++) {
				if (bucket[i] == null) {
					break;
				}

				if (!c.Contains(bucket[i])) {
					// removed
					continue;
				}

				// keep
				if (i != j) {
					bucket[j] = bucket[i];
				}

				j++;
				newsize++;
			}

			newsize += j;

			while (j < i) {
				bucket[j] = default;
				j++;
			}
		}

		bool changed = newsize != n;
		n = newsize;
		return changed;
	}

	//@Override
	public bool removeAll(ICollection<T> c) {
		bool changed = false;
		foreach (var o in c) {
			changed |= removeFast(asElementType(o));
		}

		return changed;
	}

	//@Override
	public void clear() {
		n = 0;
		buckets = createBuckets(this.initialCapacity);
		threshold = (int)Math.Floor(this.initialCapacity * LOAD_FACTOR);
	}

	//@Override
	public String toString() {
		if ( size()==0 ) return "{}";

		StringBuilder buf = new StringBuilder();
		buf.Append('{');
		bool first = true;
		foreach (T[] bucket in buckets) {
			if ( bucket==null ) continue;
			foreach (T o in bucket) {
				if ( o==null ) break;
				if ( first ) first=false;
				else buf.Append(", ");
				buf.Append(o.ToString());
			}
		}
		buf.Append('}');
		return buf.ToString();
	}

	public String toTableString() {
		StringBuilder buf = new StringBuilder();
		foreach (T[] bucket in buckets) {
			if ( bucket==null ) {
				buf.Append("null\n");
				continue;
			}
			buf.Append('[');
			bool first = true;
			foreach (T o in bucket) {
				if ( first ) first=false;
				else buf.Append(" ");
				if ( o==null ) buf.Append("_");
				else buf.Append(o.ToString());
			}
			buf.Append("]\n");
		}
		return buf.ToString();
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
	protected T asElementType(Object o) {
		return (T)o;
	}

	/**
	 * Return an array of {@code T[]} with Length {@code capacity}.
	 *
	 * @param capacity the Length of the array to return
	 * @return the newly constructed array
	 */
	//@SuppressWarnings("unchecked")
	protected T[][] createBuckets(int capacity) {
		return (T[][])new T[capacity][];
	}

	/**
	 * Return an array of {@code T} with Length {@code capacity}.
	 *
	 * @param capacity the Length of the array to return
	 * @return the newly constructed array
	 */
	//@SuppressWarnings("unchecked")
	protected T[] createBucket(int capacity) {
		return (T[])new T[capacity];
	}

	protected class SetIterator : Iterator<T> {
		readonly Array2DHashSet<T> values;
        readonly T[] data;
		int nextIndex = 0;
		bool removed = true;

		public SetIterator(Array2DHashSet<T> values,T[] data) {
			this.data = (T[])data.Clone();
			this.values = values;
		}

		//@Override
		public bool hasNext() {
			return nextIndex < data.Length;
		}

		//@Override
		public T next() {
			if (!hasNext()) {
				throw new NoSuchElementException();
			}

			removed = false;
			return data[nextIndex++];
		}

		//@Override
		public void remove() {
			if (removed) {
				throw new IllegalStateException();
			}

			this.values.Remove(data[nextIndex - 1]);
			removed = true;
		}
	}
}
