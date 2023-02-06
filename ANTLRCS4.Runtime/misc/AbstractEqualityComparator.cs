/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */
namespace org.antlr.v4.runtime.misc;

/**
 * This abstract base class is provided so performance-critical applications can
 * use virtual- instead of interface-dispatch when calling comparator methods.
 *
 * @author Sam Harwell
 */
public abstract class AbstractEqualityComparator<T> : EqualityComparator<T>
{
    public virtual bool Equals(T a, T b) => throw new NotImplementedException();

    public virtual int GetHashCode(T obj) => throw new NotImplementedException();
}
