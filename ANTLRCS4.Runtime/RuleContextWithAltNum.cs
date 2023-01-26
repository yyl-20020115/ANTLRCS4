/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime.atn;

namespace org.antlr.v4.runtime;


/** A handy class for use with
 *
 *  options {contextSuperClass=org.antlr.v4.runtime.RuleContextWithAltNum;}
 *
 *  that provides a backing field / impl for the outer alternative number
 *  matched for an internal parse tree node.
 *
 *  I'm only putting into Java runtime as I'm certain I'm the only one that
 *  will really every use this.
 */
public class RuleContextWithAltNum : ParserRuleContext {
	public int altNum;
	public RuleContextWithAltNum() { altNum = ATN.INVALID_ALT_NUMBER; }

	public RuleContextWithAltNum(ParserRuleContext parent, int invokingStateNumber):base(parent, invokingStateNumber)
    {
		;
	}
		public override int getAltNumber() { return altNum; }
		public override void setAltNumber(int altNum) { this.altNum = altNum; }
}
