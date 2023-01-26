/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */
namespace org.antlr.v4.runtime.misc;
public class TEqualityComparator<T> : AbstractEqualityComparator<T>
{
    public static readonly TEqualityComparator<T> INSTANCE = new();

    /**
	 * {@inheritDoc}
	 *
	 * <p>This implementation returns
	 * {@code obj.}{@link Object#hashCode hashCode()}.</p>
	 */
    public override int GetHashCode(T obj)
    {
        if (obj == null)
        {
            return 0;
        }

        return obj.GetHashCode();
    }

    /**
	 * {@inheritDoc}
	 *
	 * <p>This implementation relies on object equality. If both objects are
	 * {@code null}, this method returns {@code true}. Otherwise if only
	 * {@code a} is {@code null}, this method returns {@code false}. Otherwise,
	 * this method returns the result of
	 * {@code a.}{@link Object#equals equals}{@code (b)}.</p>
	 */
    public override bool Equals(T a, T b)
    {
        if (a == null)
        {
            return b == null;
        }

        return a.Equals(b);
    }

}

/**
 * This default implementation of {@link EqualityComparator} uses object equality
 * for comparisons by calling {@link Object#hashCode} and {@link Object#equals}.
 *
 * @author Sam Harwell
 */
public class ObjectEqualityComparator : AbstractEqualityComparator<object> {
	public static readonly ObjectEqualityComparator INSTANCE = new ();
}
