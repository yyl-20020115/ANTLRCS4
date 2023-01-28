/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

namespace org.antlr.v4.codegen.model;

public class CaptureNextToken : SrcOp {
	public String varName;
	public CaptureNextToken(OutputModelFactory factory, String varName):base(factory) {
		this.varName = varName;
	}
}
