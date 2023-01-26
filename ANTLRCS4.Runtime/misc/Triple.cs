/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

namespace org.antlr.v4.runtime.misc;

public class Triple<A,B,C> {
	public readonly A a;
	public readonly B b;
	public readonly C c;

	public Triple(A a, B b, C c) {
		this.a = a;
		this.b = b;
		this.c = c;
	}

	public override bool Equals(Object obj) {
		if (obj == this) {
			return true;
		}
		else if (!(obj is Triple<?, ?, ?>)) {
			return false;
		}

		Triple<?, ?, ?> other = (Triple<?, ?, ?>)obj;
		return ObjectEqualityComparator.INSTANCE.equals(a, other.a)
			&& ObjectEqualityComparator.INSTANCE.equals(b, other.b)
			&& ObjectEqualityComparator.INSTANCE.equals(c, other.c);
	}

	
	public override int GetHashCode() {
		int hash = MurmurHash.initialize();
		hash = MurmurHash.update(hash, a);
		hash = MurmurHash.update(hash, b);
		hash = MurmurHash.update(hash, c);
		return MurmurHash.finish(hash, 3);
	}

	public override String ToString() {
		return $"({a}, {b}, {c})";
	}
}
