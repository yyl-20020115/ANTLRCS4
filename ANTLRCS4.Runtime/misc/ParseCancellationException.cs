/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

namespace org.antlr.v4.runtime.misc;

/**
 * This exception is thrown to cancel a parsing operation. This exception does
 * not extend {@link RecognitionException}, allowing it to bypass the standard
 * error recovery mechanisms. {@link BailErrorStrategy} throws this exception in
 * response to a parse error.
 *
 * @author Sam Harwell
 */
public class ParseCancellationException : CancellationException
{
    public ParseCancellationException()
    {
    }

    public ParseCancellationException(string message) : base(message)
    {
    }

    public ParseCancellationException(Exception cause) : base(cause.Message, cause)
    {
        //initCause(cause);
    }

    public ParseCancellationException(string message, Exception cause) : base(message, cause)
    {
        //initCause(cause);
    }

}
