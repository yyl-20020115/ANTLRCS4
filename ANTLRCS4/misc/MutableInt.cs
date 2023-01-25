/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

namespace org.antlr.v4.misc;

public class MutableInt : Number , Comparable<MutableInt> {
	public int v;

	public MutableInt(int v) { this.v = v; }

	public override bool Equals(Object o) {
		if ( o is Number ) return v == ((Number)o).intValue();
		return false;
	}

	public override int GetHashCode() { return v; }

	public override int compareTo(MutableInt o) { return v-o.intValue(); }
	public override int intValue() { return v; }
	public override long longValue() { return v; }
	public override float floatValue() { return v; }
	public override double doubleValue() { return v; }

	public override String ToString() {
		return String.valueOf(v);
	}
}
