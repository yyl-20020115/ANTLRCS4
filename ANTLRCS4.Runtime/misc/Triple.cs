/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

namespace org.antlr.v4.runtime.misc;

public class Triple<A, B, C>
{
    public readonly A a;
    public readonly B b;
    public readonly C c;

    public Triple(A a, B b, C c)
    {
        this.a = a;
        this.b = b;
        this.c = c;
    }

    public override bool Equals(object? obj) 
        => obj == this || (obj is Triple<A, B, C> other && ObjectEqualityComparator.INSTANCE.Equals(a, other.a)
                            && ObjectEqualityComparator.INSTANCE.Equals(b, other.b)
                            && ObjectEqualityComparator.INSTANCE.Equals(c, other.c));


    public override int GetHashCode()
    {
        int hash = MurmurHash.Initialize();
        hash = MurmurHash.Update(hash, a);
        hash = MurmurHash.Update(hash, b);
        hash = MurmurHash.Update(hash, c);
        return MurmurHash.Finish(hash, 3);
    }

    public override string ToString() => $"({a}, {b}, {c})";
}
