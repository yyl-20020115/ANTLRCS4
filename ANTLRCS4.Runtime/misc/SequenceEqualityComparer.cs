/* Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

namespace Antlr4.Runtime.Sharpen;

using System.Collections.Generic;
using System.Linq;

public class SequenceEqualityComparer<T> : EqualityComparer<IEnumerable<T>>
{
    private static readonly SequenceEqualityComparer<T> comparer = new ();

    private readonly IEqualityComparer<T> elementEqualityComparer = EqualityComparer<T>.Default;

    public SequenceEqualityComparer()
        : this(null) { }

    public SequenceEqualityComparer(IEqualityComparer<T> elementComparer) 
        => elementEqualityComparer = elementComparer ?? EqualityComparer<T>.Default;

    public new static SequenceEqualityComparer<T> Default => comparer;

    public override bool Equals(IEnumerable<T> x, IEnumerable<T> y) 
        => x == y || (x != null && y != null && x.SequenceEqual(y, elementEqualityComparer));

    public override int GetHashCode(IEnumerable<T> obj)
    {
        if (obj == null)
            return 0;

        int hashCode = 1;
        foreach (T element in obj)
            hashCode = 31 * hashCode + elementEqualityComparer.GetHashCode(element);

        return hashCode;
    }
}
