/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

namespace org.antlr.v4.misc;


public class MutableInt : IComparable<MutableInt> {
	public int v;

	public MutableInt(int v) { this.v = v; }

	public override bool Equals(Object o) {
		if ( o is MutableInt m ) return v == m.intValue();
		return false;
	}

	public override int GetHashCode() { return v; }

	public virtual int CompareTo(MutableInt? o) { return v-o.intValue(); }
	public virtual int intValue() { return v; }
	public virtual long longValue() { return v; }
	public virtual float floatValue() { return v; }
	public virtual double doubleValue() { return v; }

	public override String ToString() {
		return v.ToString();
	}
}
