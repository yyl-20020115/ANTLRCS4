/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */
using ANTLRCS4.Runtime;
using org.antlr.v4.runtime.dfa;
using org.antlr.v4.runtime.tree.pattern;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System.Text;

namespace org.antlr.v4.runtime.misc;

/**
 * This class implements the {@link IntSet} backed by a sorted array of
 * non-overlapping intervals. It is particularly efficient for representing
 * large collections of numbers, where the majority of elements appear as part
 * of a sequential range of numbers that are all part of the set. For example,
 * the set { 1, 2, 3, 4, 7, 8 } may be represented as { [1, 4], [7, 8] }.
 *
 * <p>
 * This class is able to represent sets containing any combination of values in
 * the range {@link Integer#MIN_VALUE} to {@link Integer#MAX_VALUE}
 * (inclusive).</p>
 */
public class IntervalSet : IntSet {
	public static readonly IntervalSet COMPLETE_CHAR_SET = IntervalSet.of(Lexer.MIN_CHAR_VALUE, Lexer.MAX_CHAR_VALUE);
	static IntervalSet() {
		COMPLETE_CHAR_SET.setReadonly(true);
        EMPTY_SET.setReadonly(true);
    }

    public static readonly IntervalSet EMPTY_SET = new ();
	
	/** The list of sorted, disjoint intervals. */
    protected List<Interval> intervals;

    protected bool @readonly;

	public IntervalSet(List<Interval> intervals) {
		this.intervals = intervals;
	}

	public IntervalSet(IntervalSet set):this() {
		addAll(set);
	}

	public IntervalSet(params int[] els) {
		if ( els==null ) {
			intervals = new (2); // most sets are 1 or 2 elements
		}
		else {
			intervals = new (els.Length);
			foreach(int e in els) add(e);
		}
	}

	/** Create a set with a single element, el. */

    public static IntervalSet of(int a) {
		IntervalSet s = new IntervalSet();
        s.add(a);
        return s;
    }

    /** Create a set with all ints within range [a..b] (inclusive) */
	public static IntervalSet of(int a, int b) {
		IntervalSet s = new IntervalSet();
		s.add(a,b);
		return s;
	}

	public void clear() {
        if ( @readonly ) throw new IllegalStateException("can't alter readonly IntervalSet");
		intervals.Clear();
	}

    /** Add a single element to the set.  An isolated element is stored
     *  as a range el..el.
     */
    //@Override
    public void add(int el) {
        if (@readonly) throw new IllegalStateException("can't alter readonly IntervalSet");
        add(el,el);
    }

    /** Add interval; i.e., add all integers from a to b to set.
     *  If b&lt;a, do nothing.
     *  Keep list in sorted order (by left range value).
     *  If overlap, combine ranges.  For example,
     *  If this is {1..5, 10..20}, adding 6..7 yields
     *  {1..5, 6..7, 10..20}.  Adding 4..8 yields {1..8, 10..20}.
     */
    public void add(int a, int b) {
        add(Interval.of(a, b));
    }

	// copy on write so we can cache a..a intervals and sets of that
	protected void add(Interval addition) {
        if (@readonly) throw new IllegalStateException("can't alter readonly IntervalSet");
		//System.out.println("add "+addition+" to "+intervals.toString());
		if ( addition.b<addition.a ) {
			return;
		}
		// find position in list
		// Use iterators as we modify list in place
		
        for(int i = 0;i< intervals.Count;i++) {
			Interval r =intervals[i];
			if ( addition.Equals(r) ) {
				return;
			}
			if ( addition.adjacent(r) || !addition.disjoint(r) ) {
				// next to each other, make a single larger interval
				Interval bigger = addition.union(r);
				intervals[i] = bigger;
                // make sure we didn't just create an interval that
                // should be merged with next interval in list
                while ( ++i<intervals.Count ) {
					Interval next = intervals[i];
					if ( !bigger.adjacent(next) && bigger.disjoint(next) ) {
						break;
					}
					intervals.RemoveAt(i);
					i--;
					intervals[i] = bigger.union(next);
					i++;
					// if we bump up against or overlap next, merge
					//iter.remove();   // remove this one
					//iter.previous(); // move backwards to what we just set
					//iter.set(bigger.union(next)); // set to 3 merged ones
					//iter.next(); // first call to next after previous duplicates the result
				}
				return;
			}
			if ( addition.startsBeforeDisjoint(r) ) {
				// insert before r
				//i++;
				intervals.Insert(i, addition);
				//iter.previous();
				//iter.add(addition);
				return;
			}
			// if disjoint and after r, a future iteration will handle it
		}
		// ok, must be after last interval (and disjoint from last interval)
		// just add it
		intervals.Add(addition);
	}

	/** combine all sets in the array returned the or'd value */
	public static IntervalSet or(IntervalSet[] sets) {
		IntervalSet r = new IntervalSet();
		foreach (IntervalSet s in sets) r.addAll(s);
		return r;
	}

	//@Override
	public IntervalSet addAll(IntSet set) {
		if ( set==null ) {
			return this;
		}

		if (set is IntervalSet) {
			IntervalSet other = (IntervalSet)set;
			// walk set and add each interval
			int n = other.intervals.Count;
			for (int i = 0; i < n; i++) {
				Interval I = other.intervals[i];
				this.add(I.a,I.b);
			}
		}
		else {
			foreach (int value in set.toList()) {
				add(value);
			}
		}

		return this;
    }

    public IntervalSet complement(int minElement, int maxElement) {
        return this.complement(IntervalSet.of(minElement,maxElement));
    }

    /** {@inheritDoc} */
    //@Override
    public IntervalSet complement(IntSet vocabulary) {
		if ( vocabulary==null || vocabulary.isNil() ) {
			return null; // nothing in common with null set
		}

		IntervalSet vocabularyIS;
		if (vocabulary is IntervalSet) {
			vocabularyIS = (IntervalSet)vocabulary;
		}
		else {
			vocabularyIS = new IntervalSet();
			vocabularyIS.addAll(vocabulary);
		}

		return vocabularyIS.subtract(this);
    }

	//@Override
	public IntervalSet subtract(IntSet a) {
		if (a == null || a.isNil()) {
			return new IntervalSet(this);
		}

		if (a is IntervalSet) {
			return subtract(this, (IntervalSet)a);
		}

		IntervalSet other = new IntervalSet();
		other.addAll(a);
		return subtract(this, other);
	}

	/**
	 * Compute the set difference between two interval sets. The specific
	 * operation is {@code left - right}. If either of the input sets is
	 * {@code null}, it is treated as though it was an empty set.
	 */

	public static IntervalSet subtract(IntervalSet left, IntervalSet right) {
		if (left == null || left.isNil()) {
			return new IntervalSet();
		}

		IntervalSet result = new IntervalSet(left);
		if (right == null || right.isNil()) {
			// right set has no elements; just return the copy of the current set
			return result;
		}

		int resultI = 0;
		int rightI = 0;
		while (resultI < result.intervals.Count && rightI < right.intervals.Count) {
			Interval resultInterval = result.intervals[(resultI)];
			Interval rightInterval = right.intervals[(rightI)];

			// operation: (resultInterval - rightInterval) and update indexes

			if (rightInterval.b < resultInterval.a) {
				rightI++;
				continue;
			}

			if (rightInterval.a > resultInterval.b) {
				resultI++;
				continue;
			}

			Interval beforeCurrent = null;
			Interval afterCurrent = null;
			if (rightInterval.a > resultInterval.a) {
				beforeCurrent = new Interval(resultInterval.a, rightInterval.a - 1);
			}

			if (rightInterval.b < resultInterval.b) {
				afterCurrent = new Interval(rightInterval.b + 1, resultInterval.b);
			}

			if (beforeCurrent != null) {
				if (afterCurrent != null) {
					// split the current interval into two
					result.intervals[resultI]= beforeCurrent;
					result.intervals.Insert(resultI + 1, afterCurrent);
					resultI++;
					rightI++;
					continue;
				}
				else {
					// replace the current interval
					result.intervals[resultI]= beforeCurrent;
					resultI++;
					continue;
				}
			}
			else {
				if (afterCurrent != null) {
					// replace the current interval
					result.intervals[resultI]= afterCurrent;
					rightI++;
					continue;
				}
				else {
					// remove the current interval (thus no need to increment resultI)
					result.intervals.RemoveAt(resultI);
					continue;
				}
			}
		}

		// If rightI reached right.intervals.Count, no more intervals to subtract from result.
		// If resultI reached result.intervals.Count, we would be subtracting from an empty set.
		// Either way, we are done.
		return result;
	}

	//@Override
	public IntervalSet or(IntSet a) {
		IntervalSet o = new IntervalSet();
		o.addAll(this);
		o.addAll(a);
		return o;
	}

    /** {@inheritDoc} */
	//@Override
	public IntervalSet and(IntSet other) {
		if ( other==null ) { //|| !(other is IntervalSet) ) {
			return null; // nothing in common with null set
		}

		List<Interval> myIntervals = this.intervals;
		List<Interval> theirIntervals = ((IntervalSet)other).intervals;
		IntervalSet intersection = null;
		int mySize = myIntervals.Count;
		int theirSize = theirIntervals.Count;
		int i = 0;
		int j = 0;
		// iterate down both interval lists looking for nondisjoint intervals
		while ( i<mySize && j<theirSize ) {
			Interval mine = myIntervals[(i)];
			Interval theirs = theirIntervals[(j)];
			//System.out.println("mine="+mine+" and theirs="+theirs);
			if ( mine.startsBeforeDisjoint(theirs) ) {
				// move this iterator looking for interval that might overlap
				i++;
			}
			else if ( theirs.startsBeforeDisjoint(mine) ) {
				// move other iterator looking for interval that might overlap
				j++;
			}
			else if ( mine.properlyContains(theirs) ) {
				// overlap, add intersection, get next theirs
				if ( intersection==null ) {
					intersection = new IntervalSet();
				}
				intersection.add(mine.intersection(theirs));
				j++;
			}
			else if ( theirs.properlyContains(mine) ) {
				// overlap, add intersection, get next mine
				if ( intersection==null ) {
					intersection = new IntervalSet();
				}
				intersection.add(mine.intersection(theirs));
				i++;
			}
			else if ( !mine.disjoint(theirs) ) {
				// overlap, add intersection
				if ( intersection==null ) {
					intersection = new IntervalSet();
				}
				intersection.add(mine.intersection(theirs));
				// Move the iterator of lower range [a..b], but not
				// the upper range as it may contain elements that will collide
				// with the next iterator. So, if mine=[0..115] and
				// theirs=[115..200], then intersection is 115 and move mine
				// but not theirs as theirs may collide with the next range
				// in thisIter.
				// move both iterators to next ranges
				if ( mine.startsAfterNonDisjoint(theirs) ) {
					j++;
				}
				else if ( theirs.startsAfterNonDisjoint(mine) ) {
					i++;
				}
			}
		}
		if ( intersection==null ) {
			return new IntervalSet();
		}
		return intersection;
	}

    /** {@inheritDoc} */
    ////@Override
    public bool contains(int el) {
		int n = intervals.Count;
		int l = 0;
		int r = n - 1;
		// Binary search for the element in the (sorted,
		// disjoint) array of intervals.
		while (l <= r) {
			int m = (l + r) / 2;
			Interval I = intervals[(m)];
			int a = I.a;
			int b = I.b;
			if ( b<el ) {
				l = m + 1;
			} else if ( a>el ) {
				r = m - 1;
			} else { // el >= a && el <= b
				return true;
			}
		}
		return false;
    }

    /** {@inheritDoc} */
    //@Override
    public bool isNil() {
        return intervals==null || intervals.Count==0;
    }

	/**
	 * Returns the maximum value contained in the set if not isNil().
	 *
	 * @return the maximum value contained in the set.
	 * @throws RuntimeException if set is empty
	 */
	public int getMaxElement() {
		if ( isNil() ) {
			throw new RuntimeException("set is empty");
		}
		Interval last = intervals[(intervals.Count-1)];
		return last.b;
	}

	/**
	 * Returns the minimum value contained in the set if not isNil().
	 *
	 * @return the minimum value contained in the set.
	 * @throws RuntimeException if set is empty
	 */
	public int getMinElement() {
		if ( isNil() ) {
			throw new RuntimeException("set is empty");
		}

		return intervals[(0)].a;
	}

    /** Return a list of Interval objects. */
    public List<Interval> getIntervals() {
        return intervals;
    }

	////@Override
	public int hashCode() {
		int hash = MurmurHash.initialize();
        foreach (Interval I in intervals) {
			hash = MurmurHash.update(hash, I.a);
			hash = MurmurHash.update(hash, I.b);
		}

		hash = MurmurHash.finish(hash, intervals.Count * 2);
		return hash;
	}

	/** Are two IntervalSets equal?  Because all intervals are sorted
     *  and disjoint, equals is a simple linear walk over both lists
     *  to make sure they are the same.  Interval.equals() is used
     *  by the List.equals() method to check the ranges.
     */
    ////@Override
    public override bool Equals(Object? obj) {
        if ( obj==null || !(obj is IntervalSet) ) {
            return false;
        }
        IntervalSet other = (IntervalSet)obj;
		return Enumerable.SequenceEqual(this.intervals,other.intervals);
	}

	////@Override
	public override String ToString() { return toString(false); }

	public String toString(bool elemAreChar) {
		StringBuilder buf = new StringBuilder();
		if ( this.intervals==null || this.intervals.Count==0 ) {
			return "{}";
		}
		if ( this.size()>1 ) {
			buf.Append("{");
		}
		//Iterator<Interval> iter = this.intervals.iterator();
		var first = true;
		foreach (var I in this.intervals) {
            if (!first)
            {
                buf.Append(", ");
            }
			first = true;
            int a = I.a;
			int b = I.b;
			if ( a==b ) {
				if ( a==Token.EOF ) buf.Append("<EOF>");
				else if ( elemAreChar ) buf.Append('\'').Append(char.ConvertFromUtf32(a)).Append('\'');
				else buf.Append(a);
			}
			else {
				if ( elemAreChar ) buf.Append('\'').Append(char.ConvertFromUtf32(a)).Append("'..'").Append(char.ConvertFromUtf32(b)).Append('\'');
				else buf.Append(a).Append("..").Append(b);
			}
			
		}
		if ( this.size()>1 ) {
			buf.Append('}');
		}
		return buf.ToString();
	}

	/**
	 * @deprecated Use {@link #toString(Vocabulary)} instead.
	 */
	//@Deprecated
	public String toString(String[] tokenNames) {
		return toString(VocabularyImpl.fromTokenNames(tokenNames));
	}

	public String toString(Vocabulary vocabulary) {
		StringBuilder buf = new StringBuilder();
		if ( this.intervals==null || this.intervals.Count==0 ) {
			return "{}";
		}
		if ( this.size()>1 ) {
			buf.Append("{");
		}
		bool first = true;
		foreach (var I in this.intervals) {
			if (!first)
			{
                buf.Append(", ");
            }
			first= false;
            int a = I.a;
			int b = I.b;
			if ( a==b ) {
				buf.Append(elementName(vocabulary, a));
			}
			else {
				for (int i=a; i<=b; i++) {
					if ( i>a ) buf.Append(", ");
                    buf.Append(elementName(vocabulary, i));
				}
			}
		}
		if ( this.size()>1 ) {
			buf.Append("}");
		}
        return buf.ToString();
    }

	/**
	 * @deprecated Use {@link #elementName(Vocabulary, int)} instead.
	 */
	//@Deprecated
	protected String elementName(String[] tokenNames, int a) {
		return elementName(VocabularyImpl.fromTokenNames(tokenNames), a);
	}


	protected String elementName(Vocabulary vocabulary, int a) {
		if (a == Token.EOF) {
			return "<EOF>";
		}
		else if (a == Token.EPSILON) {
			return "<EPSILON>";
		}
		else {
			return vocabulary.getDisplayName(a);
		}
	}

    ////@Override
    public int size() {
		int n = 0;
		int numIntervals = intervals.Count;
		if ( numIntervals==1 ) {
			Interval firstInterval = this.intervals[(0)];
			return firstInterval.b-firstInterval.a+1;
		}
		for (int i = 0; i < numIntervals; i++) {
			Interval I = intervals[(i)];
			n += (I.b-I.a+1);
		}
		return n;
    }

	public IntegerList toIntegerList() {
		IntegerList values = new IntegerList(size());
		int n = intervals.Count;
		for (int i = 0; i < n; i++) {
			Interval I = intervals[(i)];
			int a = I.a;
			int b = I.b;
			for (int v=a; v<=b; v++) {
				values.add(v);
			}
		}
		return values;
	}

    ////@Override
    public List<int> toList() {
		List<int> values = new ();
		int n = intervals.Count;
		for (int i = 0; i < n; i++) {
			Interval I = intervals[(i)];
			int a = I.a;
			int b = I.b;
			for (int v=a; v<=b; v++) {
				values.Add(v);
			}
		}
		return values;
	}

	public HashSet<int> toSet() {
        HashSet<int> s = new HashSet<int> ();
        foreach (Interval I in intervals) {
			int a = I.a;
			int b = I.b;
			for (int v=a; v<=b; v++) {
				s.Add(v);
			}
		}
		return s;
	}

	/** Get the ith element of ordered set.  Used only by RandomPhrase so
	 *  don't bother to implement if you're not doing that for a new
	 *  ANTLR code gen target.
	 */
	public int get(int i) {
		int n = intervals.Count;
		int index = 0;
		for (int j = 0; j < n; j++) {
			Interval I = intervals[(j)];
			int a = I.a;
			int b = I.b;
			for (int v=a; v<=b; v++) {
				if ( index==i ) {
					return v;
				}
				index++;
			}
		}
		return -1;
	}

	public int[] toArray() {
		return toIntegerList().toArray();
	}

	//@Override
	public void remove(int el) {
        if ( @readonly ) throw new IllegalStateException("can't alter readonly IntervalSet");
        int n = intervals.Count;
        for (int i = 0; i < n; i++) {
            Interval I = intervals[(i)];
            int a = I.a;
            int b = I.b;
            if ( el<a ) {
                break; // list is sorted and el is before this interval; not here
            }
            // if whole interval x..x, rm
            if ( el==a && el==b ) {
                intervals.RemoveAt(i);
                break;
            }
            // if on left edge x..b, adjust left
            if ( el==a ) {
                I.a++;
                break;
            }
            // if on right edge a..x, adjust right
            if ( el==b ) {
                I.b--;
                break;
            }
            // if in middle a..x..b, split interval
            if ( el>a && el<b ) { // found in this interval
                int oldb = I.b;
                I.b = el-1;      // [a..x-1]
                add(el+1, oldb); // add [x+1..b]
            }
        }
    }

    public bool isReadonly() {
        return @readonly;
    }

    public void setReadonly(bool @readonly) {
        if ( this.@readonly && !@readonly) throw new IllegalStateException("can't alter readonly IntervalSet");
        this.@readonly = @readonly;
    }

    IntSet IntSet.addAll(IntSet set)
    {
        throw new NotImplementedException();
    }

    IntSet IntSet.and(IntSet a)
    {
        throw new NotImplementedException();
    }

    IntSet IntSet.complement(IntSet elements)
    {
        throw new NotImplementedException();
    }

    IntSet IntSet.or(IntSet a)
    {
        throw new NotImplementedException();
    }

    IntSet IntSet.subtract(IntSet a)
    {
        throw new NotImplementedException();
    }
}
