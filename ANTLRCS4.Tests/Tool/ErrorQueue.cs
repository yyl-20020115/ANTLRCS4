/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.tool;

namespace org.antlr.v4.test.tool;

public class ErrorQueue
{
    public List<ANTLRMessage> errors = new();

    public int size()
    {
        throw new NotImplementedException();
    }
}