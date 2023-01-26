/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

namespace org.antlr.v4.runtime.atn;

public class ActionTransition : Transition {
	public readonly int ruleIndex;
	public readonly int actionIndex;
	public readonly bool isCtxDependent; // e.g., $i ref in action

	
	public ActionTransition(ATNState target, int ruleIndex, int actionIndex = -1, bool isCtxDependent = false) :base(target)
    {
		;
		this.ruleIndex = ruleIndex;
		this.actionIndex = actionIndex;
		this.isCtxDependent = isCtxDependent;
	}

	public override int getSerializationType() {
		return ACTION;
	}

	public override bool isEpsilon() {
		return true; // we are to be ignored by analysis 'cept for predicates
	}

	public override bool matches(int symbol, int minVocabSymbol, int maxVocabSymbol) {
		return false;
	}

	public override String ToString() {
		return "action_"+ruleIndex+":"+actionIndex;
	}
}
