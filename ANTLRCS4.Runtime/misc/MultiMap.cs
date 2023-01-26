/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

namespace org.antlr.v4.runtime.misc;

public class MultiMap<K, V> : Dictionary<K, List<V>> where K :notnull
{
	public void map(K key, V value) {
		if (!this.TryGetValue(key, out var elementsForKey)) {
			elementsForKey = new ();
			base.Add(key, elementsForKey);
		}
		elementsForKey.Add(value);
	}

	public List<Pair<K,V>> getPairs() {
		List<Pair<K,V>> pairs = new ();
		foreach (K key in this.Keys) {
			foreach (V value in this[key]) {
				pairs.Add(new Pair<K,V>(key, value));
			}
		}
		return pairs;
	}
}
