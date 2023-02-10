/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime.dfa;

namespace org.antlr.v4.runtime.misc;

/** A HashMap that remembers the order that the elements were added.
 *  You can alter the ith element with set(i,value) too :)  Unique list.
 *  I need the replace/set-element-i functionality so I'm subclassing
 *  LinkedHashSet.
 */
public class OrderedHashSet<T> : HashSet<T>
{
    /** Track the elements as they are added to the set */
    protected List<T> elements = new();

    public OrderedHashSet() { }
    public OrderedHashSet(OrderedHashSet<T> other)
        : base(other) => this.elements.AddRange(other.elements);
    public T Get(int i) => elements[i];

    /** Replace an existing value with a new value; updates the element
     *  list and the hash table, but not the key as that has not changed.
     */
    public T Set(int i, T value)
    {
        var oldElement = elements[i];
        elements[i] = value;//.set(i,value); // update list
        base.Remove(oldElement); // now update the set: remove/add
        base.Add(value);
        return oldElement;
    }

    public bool Remove(int i)
    {
        var o = elements[i];
        elements.RemoveAt(i);
        return base.Remove(o);
    }

    /** Add a value to list; keep in hashtable for consistency also;
     *  Key is object itself.  Good for say asking if a certain string is in
     *  a list of strings.
     */
    public new bool Add(T value)
    {
        var result = base.Add(value);
        if (result)
        {  // only track if new element not in set
            elements.Add(value);
        }
        return result;
    }

    public bool Remove(object o)
    {
        throw new UnsupportedOperationException();
    }

    public new void Clear()
    {
        elements.Clear();
        base.Clear();
    }

    public override int GetHashCode() => elements.GetHashCode();

    public override bool Equals(object? o)
    {
        if (o is OrderedHashSet<T> t)
        {
            //		System.out.print("equals " + this + ", " + o+" = ");
            bool same = elements != null && elements.Equals(t.Elements);
            //		Console.Out.WriteLine(same);
            return same;
        }
        return false;

    }

    public new IEnumerator<T> GetEnumerator() => elements.GetEnumerator();

    /** Return the List holding list of table elements.  Note that you are
     *  NOT getting a copy so don't write to the list.
     */
    public List<T> Elements() => elements;

    public object Clone() => new OrderedHashSet<T>(this);

    public T[] ToArray() => elements.ToArray();

    public override string ToString() => Arrays.ToString(elements.ToArray());
}
