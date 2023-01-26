/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using System.Runtime.Serialization;

namespace org.antlr.v4.runtime.misc;

public class Pair<A,B> : ISerializable {
	public readonly A a;
	public readonly B b;

	public Pair(A a, B b) {
		this.a = a;
		this.b = b;
	}

	public override bool Equals(Object obj) {
		if (obj == this) {
			return true;
		}
		else if (!(obj is Pair<A, B>)) {
			return false;
		}

		Pair<A, B> other = (Pair<A, B>)obj;
		return ObjectEqualityComparator.INSTANCE.Equals(a, other.a)
			&& ObjectEqualityComparator.INSTANCE.Equals(b, other.b);
	}


	public override int GetHashCode() {
		int hash = MurmurHash.initialize();
		hash = MurmurHash.update(hash, a);
		hash = MurmurHash.update(hash, b);
		return MurmurHash.finish(hash, 2);
	}

	public override String ToString() {
		return $"({a}, {b})";
	}
}
