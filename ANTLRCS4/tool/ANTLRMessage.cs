/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using Antlr4.StringTemplate;
using org.antlr.v4.codegen.model.chunk;
using org.antlr.v4.runtime;

namespace org.antlr.v4.tool;



public class ANTLRMessage {
	private static readonly Object[] EMPTY_ARGS = new Object[0];


    private readonly ErrorType errorType;

    private readonly Object[] args;

    private readonly Exception e;

    // used for location template
    public String fileName;
    public int line = -1;
    public int charPosition = -1;

	public Grammar g;
	/** Most of the time, we'll have a token such as an undefined rule ref
     *  and so this will be set.
     */
    public Token offendingToken;

	public ANTLRMessage(ErrorType errorType): this(errorType, (Exception)null, Token.INVALID_TOKEN)
    {
    }

    public ANTLRMessage(ErrorType errorType, Token offendingToken,params Object[] args) : this(errorType, null, offendingToken, args)
    {
	}

    public ANTLRMessage(ErrorType errorType, Exception e, Token offendingToken, params Object[] args) {
        this.errorType = errorType;
        this.e = e;
        this.args = args;
		this.offendingToken = offendingToken;
    }


    public ErrorType getErrorType() {
        return errorType;
    }


    public Object[] getArgs() {
		if (args == null) {
			return EMPTY_ARGS;
		}

		return args;
    }

	public Template getMessageTemplate(bool verbose) {
        Template messageST = new Template(getErrorType().msg);
		messageST.impl.Name = errorType.name;

		messageST.Add("verbose", verbose);
		Object[] args = getArgs();
		for (int i=0; i<args.Length; i++) {
			String attr = "arg";
			if ( i>0 ) attr += i + 1;
			messageST.Add(attr, args[i]);
		}
		if ( args.Length <2 ) messageST.Add("arg2", null); // some messages ref arg2

		Exception cause = getCause();
		if ( cause!=null ) {
			messageST.Add("exception", cause);
			messageST.Add("stackTrace", cause.StackTrace);
		}
		else {
			messageST.Add("exception", null); // avoid ST error msg
			messageST.Add("stackTrace", null);
		}

		return messageST;
	}


    public Exception getCause() {
        return e;
    }

	//@Override
	public String toString() {
		return "Message{" +
			   "errorType=" + getErrorType() +
			   ", args=" + string.Join(',',(getArgs())) +
			   ", e=" + getCause() +
			   ", fileName='" + fileName + '\'' +
			   ", line=" + line +
			   ", charPosition=" + charPosition +
			   '}';
	}
}
