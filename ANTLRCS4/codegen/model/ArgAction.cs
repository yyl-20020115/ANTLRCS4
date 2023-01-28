/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

namespace org.antlr.v4.codegen.model;


public class ArgAction : Action {
	/** Context type of invoked rule */
	public String ctxType;
	public ArgAction(OutputModelFactory factory, ActionAST ast, String ctxType) {
		base(factory, ast);
		this.ctxType = ctxType;
	}
}
