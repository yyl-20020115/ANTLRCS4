/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

namespace org.antlr.v4.misc;


/** Count how many of each key we have; not thread safe */
public class FrequencySet<T> : Dictionary<T, MutableInt> {
	public int count(T key) {
		if (!this.TryGetValue(key,out var value)) return 0;
		return value.v;
	}
	public void add(T key) {
		if (!this.TryGetValue(key,out var value)) {
			value = new MutableInt(1);
			Add(key, value);
		}
		else {
			value.v++;
		}
	}
}
