/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using System.Runtime.Serialization;

namespace org.antlr.v4.runtime.misc;

public class Pair<A, B>
{
    public readonly A a;
    public readonly B b;

    public Pair(A a, B b)
    {
        this.a = a;
        this.b = b;
    }

    public override bool Equals(Object? obj)
    {
        if (obj == this)
        {
            return true;
        }
        else if (obj is Pair<A, B> other)
        {
            return ObjectEqualityComparator.INSTANCE.Equals(a, other.a)
                && ObjectEqualityComparator.INSTANCE.Equals(b, other.b);
        }
        return false;
    }


    public override int GetHashCode()
    {
        int hash = MurmurHash.Initialize();
        hash = MurmurHash.Update(hash, a);
        hash = MurmurHash.Update(hash, b);
        return MurmurHash.Finish(hash, 2);
    }

    public override string ToString() => $"({a}, {b})";
}
