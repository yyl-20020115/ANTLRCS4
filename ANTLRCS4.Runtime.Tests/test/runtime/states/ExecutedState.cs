/*
 * Copyright (c) 2012-2022 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

namespace org.antlr.v4.test.runtime.states;

public class ExecutedState : State {
	////@Override
	public Stage getStage() {
		return Stage.Execute;
	}

	public readonly String output;

	public readonly String errors;

	public ExecutedState(CompiledState previousState, String output, String errors, Exception exception) {
		base(previousState, exception);
		this.output = output != null ? output : "";
		this.errors = errors != null ? errors : "";
	}
}
