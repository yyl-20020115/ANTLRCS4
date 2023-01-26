/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime.atn;

namespace ANTLRCS4.Runtime;

public static class Objects
{
    public static bool equals(object a, object b)
    {
        return (a == b) || (a != null && a.Equals(b));
    }

    internal static void requireNonNull(object o)
    {
        if(o == null) throw new ArgumentNullException(nameof(o));
    }
}