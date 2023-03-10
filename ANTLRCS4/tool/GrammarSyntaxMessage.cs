/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime;

namespace org.antlr.v4.tool;


/** A problem with the syntax of your antlr grammar such as
 *  "The '{' came as a complete surprise to me at this point in your program"
 */
public class GrammarSyntaxMessage : ANTLRMessage
{
    public GrammarSyntaxMessage(ErrorType etype,
                                string fileName,
                                Token offendingToken,
                                RecognitionException antlrException,
                                params object[] args)
        : base(etype, antlrException, offendingToken, args)
    {
        this.fileName = fileName;
        this.offendingToken = offendingToken;
        if (offendingToken != null)
        {
            line = offendingToken.Line;
            charPosition = offendingToken.CharPositionInLine;
        }
    }

    public new RecognitionException Cause => (RecognitionException)base.Cause;
}
