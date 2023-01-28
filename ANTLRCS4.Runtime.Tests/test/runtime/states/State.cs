/*
 * Copyright (c) 2012-2022 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

namespace org.antlr.v4.test.runtime.states;

public abstract class State {
	public readonly State previousState;

	public readonly Exception exception;

	public abstract Stage getStage();

	public bool containsErrors() {
		return exception != null;
	}

	public String getErrorMessage() {
		String result = "State: " + getStage() + "; ";
		if (exception != null) {
			result += exception.ToString();
			if ( exception.getCause()!=null ) {
				result += "\nCause:\n";
				result += exception.getCause().ToString();
			}
		}
		return result;
	}

	public State(State previousState, Exception exception) {
		this.previousState = previousState;
		this.exception = exception;
	}
}
