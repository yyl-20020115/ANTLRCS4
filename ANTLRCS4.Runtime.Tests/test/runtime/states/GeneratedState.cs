/*
 * Copyright (c) 2012-2022 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

namespace org.antlr.v4.test.runtime.states;


public class GeneratedState : State {
	////@Override
	public override Stage getStage() {
		return Stage.Generate;
	}

	public readonly ErrorQueue errorQueue;
	public readonly List<GeneratedFile> generatedFiles;

	////@Override
	public bool containsErrors() {
		return errorQueue.errors.Count > 0 || base.containsErrors();
	}

	public String getErrorMessage() {
		String result = base.getErrorMessage();

		if (errorQueue.errors.Count > 0) {
			result = RuntimeTestUtils.joinLines(result, errorQueue.toString(true));
		}

		return result;
	}

	public GeneratedState(ErrorQueue errorQueue, List<GeneratedFile>  generatedFiles, Exception exception) 
	: base(null, exception)
    {
		this.errorQueue = errorQueue;
		this.generatedFiles = generatedFiles;
	}
}
