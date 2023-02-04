/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

namespace org.antlr.v4.misc;


public class MutableInt : IComparable<MutableInt>
{
    public int v;

    public MutableInt(int v) { this.v = v; }

    public override bool Equals(object? o)
        => o is MutableInt m && v == m.IntValue();

    public override int GetHashCode() => v;

    public virtual int CompareTo(MutableInt? o) => v - o.IntValue();
    public virtual int IntValue() => v;
    public virtual long LongValue() => v;
    public virtual float FloatValue() => v;
    public virtual double DoubleValue() => v;
    public override string ToString() => v.ToString();
}
