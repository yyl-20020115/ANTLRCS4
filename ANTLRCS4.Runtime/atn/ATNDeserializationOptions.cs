/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime.dfa;

namespace org.antlr.v4.runtime.atn;

/**
 *
 * @author Sam Harwell
 */
public class ATNDeserializationOptions {
	private static readonly ATNDeserializationOptions defaultOptions;
	static ATNDeserializationOptions(){
		defaultOptions = new ATNDeserializationOptions();
		defaultOptions.makeReadOnly();
	}

	private bool readOnly;
	private bool verifyATN;
	private bool generateRuleBypassTransitions;

	public ATNDeserializationOptions() {
		this.verifyATN = true;
		this.generateRuleBypassTransitions = false;
	}

	public ATNDeserializationOptions(ATNDeserializationOptions options) {
		this.verifyATN = options.verifyATN;
		this.generateRuleBypassTransitions = options.generateRuleBypassTransitions;
	}


	public static ATNDeserializationOptions getDefaultOptions() {
		return defaultOptions;
	}

	public bool isReadOnly() {
		return readOnly;
	}

	public void makeReadOnly() {
		readOnly = true;
	}

	public bool isVerifyATN() {
		return verifyATN;
	}

	public void setVerifyATN(bool verifyATN) {
		throwIfReadOnly();
		this.verifyATN = verifyATN;
	}

	public bool isGenerateRuleBypassTransitions() {
		return generateRuleBypassTransitions;
	}

	public  void setGenerateRuleBypassTransitions(bool generateRuleBypassTransitions) {
		throwIfReadOnly();
		this.generateRuleBypassTransitions = generateRuleBypassTransitions;
	}

	protected void throwIfReadOnly() {
		if (isReadOnly()) {
			throw new IllegalStateException("The object is read only.");
		}
	}
}
