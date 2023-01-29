/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime;

namespace org.antlr.v4.parse;

/** */
public class v4ParserException : RecognitionException {
	public String msg;
	/** Used for remote debugger deserialization */
	public v4ParserException(String msg, IntStream input)
		:base(null,input,null) {
		this.msg = msg;
	}

}
