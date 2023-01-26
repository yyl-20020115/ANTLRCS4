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
public class OrderedHashSet<T> : HashSet<T> {
    /** Track the elements as they are added to the set */
    protected List<T> elements = new ();

    public T get(int i) {
        return elements[i];
    }

    /** Replace an existing value with a new value; updates the element
     *  list and the hash table, but not the key as that has not changed.
     */
    public T set(int i, T value) {
        T oldElement = elements[i];
        elements.set(i,value); // update list
        super.remove(oldElement); // now update the set: remove/add
        super.add(value);
        return oldElement;
    }

	public bool remove(int i) {
		T o = elements.remove(i);
        return super.remove(o);
	}

    /** Add a value to list; keep in hashtable for consistency also;
     *  Key is object itself.  Good for say asking if a certain string is in
     *  a list of strings.
     */
    public bool add(T value) {
        bool result = super.add(value);
		if ( result ) {  // only track if new element not in set
			elements.add(value);
		}
		return result;
    }

	public bool remove(Object o) {
		throw new UnsupportedOperationException();
    }

	public void clear() {
        elements.clear();
        super.clear();
    }

	public override int GetHashCode() {
		return elements.hashCode();
	}

	public bool Equals(Object o) {
		if (!(o is OrderedHashSet<T>)) {
			return false;
		}

//		System.out.print("equals " + this + ", " + o+" = ");
		bool same = elements!=null && elements.equals(((OrderedHashSet<T>)o).elements);
//		System.out.println(same);
		return same;
	}

	public Iterator<T> iterator() {
		return elements.iterator();
	}

	/** Return the List holding list of table elements.  Note that you are
     *  NOT getting a copy so don't write to the list.
     */
    public List<T> elements() {
        return elements;
    }

    public Object clone() {
        OrderedHashSet<T> dup = (OrderedHashSet<T>)super.clone();
        dup.elements = new (this.elements);
        return dup;
    }

	public Object[] ToArray() {
		return elements.ToArray();
	}

	public override String ToString() {
        return elements.toString();
    }
}
