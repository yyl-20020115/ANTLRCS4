/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using ANTLRCS4.Runtime;
using org.antlr.v4.runtime.dfa;

namespace org.antlr.v4.runtime.misc;

/** A HashMap that remembers the order that the elements were added.
 *  You can alter the ith element with set(i,value) too :)  Unique list.
 *  I need the replace/set-element-i functionality so I'm subclassing
 *  LinkedHashSet.
 */
public class OrderedHashSet<T> : HashSet<T> {
    /** Track the elements as they are added to the set */
    protected List<T> _elements = new ();

    public OrderedHashSet()
    {

    }
    public OrderedHashSet(OrderedHashSet<T> other)
        :base(other)
    {
        this._elements.AddRange(other._elements);
    }
    public T get(int i) {
        return _elements[i];
    }

    /** Replace an existing value with a new value; updates the element
     *  list and the hash table, but not the key as that has not changed.
     */
    public T set(int i, T value) {
        T oldElement = _elements[i];
        _elements[i] = value;//.set(i,value); // update list
        base.Remove(oldElement); // now update the set: remove/add
        base.Add(value);
        return oldElement;
    }

	public bool remove(int i) {
        T o = _elements[i];
        _elements.RemoveAt(i);
        return base.Remove(o);
	}

    /** Add a value to list; keep in hashtable for consistency also;
     *  Key is object itself.  Good for say asking if a certain string is in
     *  a list of strings.
     */
    public bool add(T value) {
        bool result = base.Add(value);
		if ( result ) {  // only track if new element not in set
            _elements.Add(value);
		}
		return result;
    }

	public bool remove(Object o) {
		throw new UnsupportedOperationException();
    }

	public void clear() {
        _elements.Clear();
        base.Clear();
    }

	public override int GetHashCode() {
		return _elements.GetHashCode();
	}

	public override bool Equals(Object o) {
		if (!(o is OrderedHashSet<T>)) {
			return false;
		}

//		System.out.print("equals " + this + ", " + o+" = ");
		bool same = _elements != null && _elements.Equals(((OrderedHashSet<T>)o).elements);
//		System.out.println(same);
		return same;
	}

	public new IEnumerator<T> GetEnumerator() {
		return _elements.GetEnumerator();
	}

	/** Return the List holding list of table elements.  Note that you are
     *  NOT getting a copy so don't write to the list.
     */
    public List<T> elements() {
        return _elements;
    }

    public Object clone() {
        return new OrderedHashSet<T>(this);
    }

	public T[] ToArray() {
		return _elements.ToArray();
	}

	public override String ToString() {
        return _elements.ToString();
    }
}
