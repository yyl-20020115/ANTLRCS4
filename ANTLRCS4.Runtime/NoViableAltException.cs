/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */
using org.antlr.runtime.tree;
using org.antlr.v4.runtime.atn;

namespace org.antlr.v4.runtime;


/** Indicates that the parser could not decide which of two or more paths
 *  to take based upon the remaining input. It tracks the starting token
 *  of the offending input and also knows where the parser was
 *  in the various paths when the error. Reported by reportNoViableAlternative()
 */
public class NoViableAltException : RecognitionException
{
    /** Which configurations did we try at input.index() that couldn't match input.LT(1)? */

    private readonly ATNConfigSet deadEndConfigs;

    /** The token object at the start index; the input stream might
	 * 	not be buffering tokens so get a reference to it. (At the
	 *  time the error occurred, of course the stream needs to keep a
	 *  buffer all of the tokens but later we might not have access to those.)
	 */

    private readonly Token startToken;

    public string V1 { get; }
    public int V2 { get; }
    public int V3 { get; }
    public IntStream Input { get; }

    public NoViableAltException(Parser recognizer)
        : this(recognizer,
             recognizer.InputStream as TokenStream,
             recognizer.GetCurrentToken(),
             recognizer.GetCurrentToken(),
             null,
             recognizer.             Ctx)
    { // LL(1) error
        ;
    }

    public NoViableAltException(Parser recognizer,
                                TokenStream input,
                                Token startToken,
                                Token offendingToken,
                                ATNConfigSet deadEndConfigs,
                                ParserRuleContext ctx)
        : base(recognizer, input, ctx)
    {
        ;
        this.deadEndConfigs = deadEndConfigs;
        this.startToken = startToken;
        this.OffendingToken = offendingToken;
    }

    public NoViableAltException(string v1, int v2, int v3, IntStream input)
        : base(null, input, null)
    {
        V1 = v1;
        V2 = v2;
        V3 = v3;
        Input = input;
    }

    public Token StartToken => startToken;


    public ATNConfigSet DeadEndConfigs => deadEndConfigs;

}
