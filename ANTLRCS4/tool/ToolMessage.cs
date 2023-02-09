/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime;

namespace org.antlr.v4.tool;

/** A generic message from the tool such as "file not found" type errors; there
 *  is no reason to create a special object for each error unlike the grammar
 *  errors, which may be rather complex.
 *
 *  Sometimes you need to pass in a filename or something to say it is "bad".
 *  Allow a generic object to be passed in and the string template can deal
 *  with just printing it or pulling a property out of it.
 */
public class ToolMessage : ANTLRMessage
{
    public ToolMessage(ErrorType errorType) : base(errorType)
    {
    }
    public ToolMessage(ErrorType errorType, params Object[] args)
        : base(errorType, null, Token.INVALID_TOKEN, args)
    {
    }
    public ToolMessage(ErrorType errorType, Exception e, params Object[] args)
    : base(errorType, e, Token.INVALID_TOKEN, args)
    {

    }
}
