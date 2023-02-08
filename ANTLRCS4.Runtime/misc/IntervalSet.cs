/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */
using org.antlr.v4.runtime.dfa;
using org.antlr.v4.runtime.tree.pattern;
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
public class IntervalSet : IntSet
{
    public static readonly IntervalSet COMPLETE_CHAR_SET 
        = Of(Lexer.MIN_CHAR_VALUE, Lexer.MAX_CHAR_VALUE);
    static IntervalSet()
    {
        COMPLETE_CHAR_SET.SetReadonly(true);
        EMPTY_SET.SetReadonly(true);
    }

    public static readonly IntervalSet EMPTY_SET = new();

    /** The list of sorted, disjoint intervals. */
    protected List<Interval> intervals;

    protected bool @readonly;

    public IntervalSet(List<Interval> intervals)
    {
        this.intervals = intervals;
    }

    public IntervalSet(IntervalSet set) : this()
    {
        AddAll(set);
    }

    public IntervalSet(params int[] els)
    {
        if (els == null)
        {
            intervals = new(2); // most sets are 1 or 2 elements
        }
        else
        {
            intervals = new(els.Length);
            foreach (int e in els) Add(e);
        }
    }

    /** Create a set with a single element, el. */

    public static IntervalSet Of(int a)
    {
        var s = new IntervalSet();
        s.Add(a);
        return s;
    }

    /** Create a set with all ints within range [a..b] (inclusive) */
    public static IntervalSet Of(int a, int b)
    {
        var s = new IntervalSet();
        s.Add(a, b);
        return s;
    }

    public void Clear()
    {
        if (@readonly) throw new IllegalStateException("can't alter readonly IntervalSet");
        intervals.Clear();
    }

    /** Add a single element to the set.  An isolated element is stored
     *  as a range el..el.
     */
    //@Override
    public void Add(int el)
    {
        if (@readonly) throw new IllegalStateException("can't alter readonly IntervalSet");
        Add(el, el);
    }

    /** Add interval; i.e., add all integers from a to b to set.
     *  If b&lt;a, do nothing.
     *  Keep list in sorted order (by left range value).
     *  If overlap, combine ranges.  For example,
     *  If this is {1..5, 10..20}, adding 6..7 yields
     *  {1..5, 6..7, 10..20}.  Adding 4..8 yields {1..8, 10..20}.
     */
    public void Add(int a, int b)
    {
        Add(Interval.Of(a, b));
    }

    // copy on write so we can cache a..a intervals and sets of that
    protected void Add(Interval addition)
    {
        if (@readonly) throw new IllegalStateException("can't alter readonly IntervalSet");
        //Console.Out.WriteLine("add "+addition+" to "+intervals.toString());
        if (addition.b < addition.a)
        {
            return;
        }
        // find position in list
        // Use iterators as we modify list in place

        for (int i = 0; i < intervals.Count; i++)
        {
            var r = intervals[i];
            if (addition.Equals(r))
            {
                return;
            }
            if (addition.Adjacent(r) || !addition.Disjoint(r))
            {
                // next to each other, make a single larger interval
                var bigger = addition.Union(r);
                intervals[i] = bigger;
                // make sure we didn't just create an interval that
                // should be merged with next interval in list
                while (++i < intervals.Count)
                {
                    var next = intervals[i];
                    if (!bigger.Adjacent(next) && bigger.Disjoint(next))
                    {
                        break;
                    }
                    intervals.RemoveAt(i);
                    i--;
                    intervals[i] = bigger.Union(next);
                    i++;
                    // if we bump up against or overlap next, merge
                    //iter.remove();   // remove this one
                    //iter.previous(); // move backwards to what we just set
                    //iter.set(bigger.union(next)); // set to 3 merged ones
                    //iter.next(); // first call to next after previous duplicates the result
                }
                return;
            }
            if (addition.StartsBeforeDisjoint(r))
            {
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
    public static IntervalSet Or(IntervalSet[] sets)
    {
        var r = new IntervalSet();
        foreach (IntervalSet s in sets) r.AddAll(s);
        return r;
    }

    //@Override
    public IntervalSet AddAll(IntSet set)
    {
        if (set == null)
        {
            return this;
        }

        if (set is IntervalSet other)
        {
            // walk set and add each interval
            int n = other.intervals.Count;
            for (int i = 0; i < n; i++)
            {
                var I = other.intervals[i];
                this.Add(I.a, I.b);
            }
        }
        else
        {
            foreach (int value in set.ToList())
            {
                Add(value);
            }
        }

        return this;
    }

    public IntervalSet Complement(int minElement, int maxElement)
    {
        return this.Complement(IntervalSet.Of(minElement, maxElement));
    }

    /** {@inheritDoc} */
    //@Override
    public IntervalSet Complement(IntSet vocabulary)
    {
        if (vocabulary == null || vocabulary.IsNil)
        {
            return null; // nothing in common with null set
        }

        IntervalSet vocabularyIS;
        if (vocabulary is IntervalSet set)
        {
            vocabularyIS = set;
        }
        else
        {
            vocabularyIS = new IntervalSet();
            vocabularyIS.AddAll(vocabulary);
        }

        return vocabularyIS.Subtract(this);
    }

    //@Override
    public IntervalSet Subtract(IntSet a)
    {
        if (a == null || a.IsNil)
        {
            return new IntervalSet(this);
        }

        if (a is IntervalSet set)
        {
            return Subtract(this, set);
        }

        var other = new IntervalSet();
        other.AddAll(a);
        return Subtract(this, other);
    }

    /**
	 * Compute the set difference between two interval sets. The specific
	 * operation is {@code left - right}. If either of the input sets is
	 * {@code null}, it is treated as though it was an empty set.
	 */

    public static IntervalSet Subtract(IntervalSet left, IntervalSet right)
    {
        if (left == null || left.IsNil)
        {
            return new IntervalSet();
        }

        var result = new IntervalSet(left);
        if (right == null || right.IsNil)
        {
            // right set has no elements; just return the copy of the current set
            return result;
        }

        int resultI = 0;
        int rightI = 0;
        while (resultI < result.intervals.Count && rightI < right.intervals.Count)
        {
            var resultInterval = result.intervals[(resultI)];
            var rightInterval = right.intervals[(rightI)];

            // operation: (resultInterval - rightInterval) and update indexes

            if (rightInterval.b < resultInterval.a)
            {
                rightI++;
                continue;
            }

            if (rightInterval.a > resultInterval.b)
            {
                resultI++;
                continue;
            }

            Interval beforeCurrent = null;
            Interval afterCurrent = null;
            if (rightInterval.a > resultInterval.a)
            {
                beforeCurrent = new Interval(resultInterval.a, rightInterval.a - 1);
            }

            if (rightInterval.b < resultInterval.b)
            {
                afterCurrent = new Interval(rightInterval.b + 1, resultInterval.b);
            }

            if (beforeCurrent != null)
            {
                if (afterCurrent != null)
                {
                    // split the current interval into two
                    result.intervals[resultI] = beforeCurrent;
                    result.intervals.Insert(resultI + 1, afterCurrent);
                    resultI++;
                    rightI++;
                    continue;
                }
                else
                {
                    // replace the current interval
                    result.intervals[resultI] = beforeCurrent;
                    resultI++;
                    continue;
                }
            }
            else
            {
                if (afterCurrent != null)
                {
                    // replace the current interval
                    result.intervals[resultI] = afterCurrent;
                    rightI++;
                    continue;
                }
                else
                {
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
    public IntervalSet Or(IntSet a)
    {
        var o = new IntervalSet();
        o.AddAll(this);
        o.AddAll(a);
        return o;
    }

    /** {@inheritDoc} */
    //@Override
    public IntervalSet And(IntSet other)
    {
        if (other == null)
        { //|| !(other is IntervalSet) ) {
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
        while (i < mySize && j < theirSize)
        {
            var mine = myIntervals[(i)];
            var theirs = theirIntervals[(j)];
            //Console.Out.WriteLine("mine="+mine+" and theirs="+theirs);
            if (mine.StartsBeforeDisjoint(theirs))
            {
                // move this iterator looking for interval that might overlap
                i++;
            }
            else if (theirs.StartsBeforeDisjoint(mine))
            {
                // move other iterator looking for interval that might overlap
                j++;
            }
            else if (mine.ProperlyContains(theirs))
            {
                // overlap, add intersection, get next theirs
                if (intersection == null)
                {
                    intersection = new IntervalSet();
                }
                intersection.Add(mine.Intersection(theirs));
                j++;
            }
            else if (theirs.ProperlyContains(mine))
            {
                // overlap, add intersection, get next mine
                if (intersection == null)
                {
                    intersection = new IntervalSet();
                }
                intersection.Add(mine.Intersection(theirs));
                i++;
            }
            else if (!mine.Disjoint(theirs))
            {
                // overlap, add intersection
                if (intersection == null)
                {
                    intersection = new IntervalSet();
                }
                intersection.Add(mine.Intersection(theirs));
                // Move the iterator of lower range [a..b], but not
                // the upper range as it may contain elements that will collide
                // with the next iterator. So, if mine=[0..115] and
                // theirs=[115..200], then intersection is 115 and move mine
                // but not theirs as theirs may collide with the next range
                // in thisIter.
                // move both iterators to next ranges
                if (mine.StartsAfterNonDisjoint(theirs))
                {
                    j++;
                }
                else if (theirs.StartsAfterNonDisjoint(mine))
                {
                    i++;
                }
            }
        }
        if (intersection == null)
        {
            return new IntervalSet();
        }
        return intersection;
    }

    /** {@inheritDoc} */
    ////@Override
    public bool Contains(int el)
    {
        int n = intervals.Count;
        int l = 0;
        int r = n - 1;
        // Binary search for the element in the (sorted,
        // disjoint) array of intervals.
        while (l <= r)
        {
            int m = (l + r) / 2;
            var I = intervals[(m)];
            int a = I.a;
            int b = I.b;
            if (b < el)
            {
                l = m + 1;
            }
            else if (a > el)
            {
                r = m - 1;
            }
            else
            { // el >= a && el <= b
                return true;
            }
        }
        return false;
    }

    /** {@inheritDoc} */
    //@Override
    public bool IsNil => intervals == null || intervals.Count == 0;

    /**
	 * Returns the maximum value contained in the set if not isNil().
	 *
	 * @return the maximum value contained in the set.
	 * @throws RuntimeException if set is empty
	 */
    public int GetMaxElement()
    {
        if (IsNil)
        {
            throw new RuntimeException("set is empty");
        }
        var last = intervals[^1];
        return last.b;
    }

    /**
	 * Returns the minimum value contained in the set if not isNil().
	 *
	 * @return the minimum value contained in the set.
	 * @throws RuntimeException if set is empty
	 */
    public int GetMinElement()
    {
        if (IsNil)
        {
            throw new RuntimeException("set is empty");
        }

        return intervals[(0)].a;
    }

    /** Return a list of Interval objects. */
    public List<Interval> GetIntervals()
    {
        return intervals;
    }

    ////@Override
    public override int GetHashCode()
    {
        int hash = MurmurHash.Initialize();
        foreach (Interval I in intervals)
        {
            hash = MurmurHash.Update(hash, I.a);
            hash = MurmurHash.Update(hash, I.b);
        }

        hash = MurmurHash.Finish(hash, intervals.Count * 2);
        return hash;
    }

    /** Are two IntervalSets equal?  Because all intervals are sorted
     *  and disjoint, equals is a simple linear walk over both lists
     *  to make sure they are the same.  Interval.equals() is used
     *  by the List.equals() method to check the ranges.
     */
    ////@Override
    public override bool Equals(object? obj)
    {
        if (obj == null || obj is not IntervalSet other)
        {
            return false;
        }
        return Enumerable.SequenceEqual(this.intervals, other.intervals);
    }

    ////@Override
    public override string ToString() => ToString(false);

    public string ToString(bool elemAreChar)
    {
        var buffer = new StringBuilder();
        if (this.intervals == null || this.intervals.Count == 0)
        {
            return "{}";
        }
        if (this.Size > 1)
        {
            buffer.Append("{");
        }
        //Iterator<Interval> iter = this.intervals.iterator();
        var first = true;
        foreach (var I in this.intervals)
        {
            if (!first)
            {
                buffer.Append(", ");
            }
            first = true;
            int a = I.a;
            int b = I.b;
            if (a == b)
            {
                if (a == Token.EOF) buffer.Append("<EOF>");
                else if (elemAreChar) buffer.Append('\'').Append(char.ConvertFromUtf32(a)).Append('\'');
                else buffer.Append(a);
            }
            else
            {
                if (elemAreChar) buffer.Append('\'').Append(char.ConvertFromUtf32(a)).Append("'..'").Append(char.ConvertFromUtf32(b)).Append('\'');
                else buffer.Append(a).Append("..").Append(b);
            }

        }
        if (this.Size > 1)
        {
            buffer.Append('}');
        }
        return buffer.ToString();
    }

    /**
	 * @deprecated Use {@link #toString(Vocabulary)} instead.
	 */
    //@Deprecated
    public string ToString(string[] tokenNames)
    {
        return ToString(VocabularyImpl.FromTokenNames(tokenNames));
    }

    public string ToString(Vocabulary vocabulary)
    {
        var buffer = new StringBuilder();
        if (this.intervals == null || this.intervals.Count == 0)
        {
            return "{}";
        }
        if (this.Size > 1)
        {
            buffer.Append('{');
        }
        bool first = true;
        foreach (var I in this.intervals)
        {
            if (!first)
            {
                buffer.Append(", ");
            }
            first = false;
            int a = I.a;
            int b = I.b;
            if (a == b)
            {
                buffer.Append(ElementName(vocabulary, a));
            }
            else
            {
                for (int i = a; i <= b; i++)
                {
                    if (i > a) buffer.Append(", ");
                    buffer.Append(ElementName(vocabulary, i));
                }
            }
        }
        if (this.Size > 1)
        {
            buffer.Append('}');
        }
        return buffer.ToString();
    }

    /**
	 * @deprecated Use {@link #elementName(Vocabulary, int)} instead.
	 */
    //@Deprecated
    protected string ElementName(string[] tokenNames, int a)
    {
        return ElementName(VocabularyImpl.FromTokenNames(tokenNames), a);
    }


    protected string ElementName(Vocabulary vocabulary, int a)
    {
        if (a == Token.EOF)
        {
            return "<EOF>";
        }
        else if (a == Token.EPSILON)
        {
            return "<EPSILON>";
        }
        else
        {
            return vocabulary.GetDisplayName(a);
        }
    }

    ////@Override
    public int Size
    {
        get
        {
            int n = 0;
            int numIntervals = intervals.Count;
            if (numIntervals == 1)
            {
                var firstInterval = this.intervals[(0)];
                return firstInterval.b - firstInterval.a + 1;
            }
            for (int i = 0; i < numIntervals; i++)
            {
                var I = intervals[(i)];
                n += (I.b - I.a + 1);
            }
            return n;
        }
    }

    public IntegerList ToIntegerList()
    {
        var values = new IntegerList(Size);
        int n = intervals.Count;
        for (int i = 0; i < n; i++)
        {
            var I = intervals[(i)];
            int a = I.a;
            int b = I.b;
            for (int v = a; v <= b; v++)
            {
                values.Add(v);
            }
        }
        return values;
    }

    ////@Override
    public List<int> ToList()
    {
        List<int> values = new();
        int n = intervals.Count;
        for (int i = 0; i < n; i++)
        {
            var I = intervals[(i)];
            int a = I.a;
            int b = I.b;
            for (int v = a; v <= b; v++)
            {
                values.Add(v);
            }
        }
        return values;
    }

    public HashSet<int> ToSet()
    {
        var s = new HashSet<int>();
        foreach (var I in intervals)
        {
            int a = I.a;
            int b = I.b;
            for (int v = a; v <= b; v++)
                s.Add(v);
        }
        return s;
    }

    /** Get the ith element of ordered set.  Used only by RandomPhrase so
	 *  don't bother to implement if you're not doing that for a new
	 *  ANTLR code gen target.
	 */
    public int Get(int i)
    {
        int n = intervals.Count;
        int index = 0;
        for (int j = 0; j < n; j++)
        {
            var I = intervals[(j)];
            int a = I.a;
            int b = I.b;
            for (int v = a; v <= b; v++)
            {
                if (index == i) return v;
                index++;
            }
        }
        return -1;
    }

    public int[] ToArray()
    {
        return ToIntegerList().ToArray();
    }

    //@Override
    public void Remove(int el)
    {
        if (@readonly) throw new IllegalStateException("can't alter readonly IntervalSet");
        int n = intervals.Count;
        for (int i = 0; i < n; i++)
        {
            var I = intervals[(i)];
            int a = I.a;
            int b = I.b;
            if (el < a)
            {
                break; // list is sorted and el is before this interval; not here
            }
            // if whole interval x..x, rm
            if (el == a && el == b)
            {
                intervals.RemoveAt(i);
                break;
            }
            // if on left edge x..b, adjust left
            if (el == a)
            {
                I.a++;
                break;
            }
            // if on right edge a..x, adjust right
            if (el == b)
            {
                I.b--;
                break;
            }
            // if in middle a..x..b, split interval
            if (el > a && el < b)
            { // found in this interval
                int oldb = I.b;
                I.b = el - 1;      // [a..x-1]
                Add(el + 1, oldb); // add [x+1..b]
            }
        }
    }

    public bool isReadonly()
    {
        return @readonly;
    }

    public void SetReadonly(bool @readonly)
    {
        if (this.@readonly && !@readonly) throw new IllegalStateException("can't alter readonly IntervalSet");
        this.@readonly = @readonly;
    }

    IntSet IntSet.AddAll(IntSet set) => this.AddAll(set);

    IntSet IntSet.And(IntSet a) => this.And(a);

    IntSet IntSet.Complement(IntSet elements)=>this.Complement(elements);

    IntSet IntSet.Or(IntSet a) => this.Or(a);

    IntSet IntSet.Subtract(IntSet a) => this.Subtract(a);
}
