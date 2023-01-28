/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

namespace org.antlr.v4.misc;

/** I need the get-element-i functionality so I'm subclassing
 *  LinkedHashMap.
 */
public class OrderedHashMap<K,V> : Dictionary<K,V> {
	/** Track the elements as they are added to the set */
	protected List<K> elements = new ();

	public K getKey(int i) { return elements.get(i); }

	public V getElement(int i) { return get(elements.get(i)); }

	//@Override
	public V put(K key, V value) {
		elements.Add(key);
		return base.put(key, value);
	}

    //@Override
    public void putAll(Dictionary<K, V> m) {
		foreach (Map.Entry<K,V> entry in m.entrySet()) {
			put(entry.getKey(), entry.getValue());
		}
	}

    //@Override
    public V remove(Object key) {
		elements.Remove(key);
		return base.Remove(key);
	}

    //@Override
    public void clear() {
		elements.Clear();
		base.Clear();
	}
}
