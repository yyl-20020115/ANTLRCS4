/* Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */
using System;
namespace org.antlr.v4.runtime.misc;

public static class Runtime
{
    public static string Substring(string str, int beginOffset, int endOffset)
    {
        if (str == null)
            throw new ArgumentNullException(nameof(str));

        return str.Substring(beginOffset, endOffset - beginOffset);
    }
}
