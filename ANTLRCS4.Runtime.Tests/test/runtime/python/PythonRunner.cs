/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */
namespace org.antlr.v4.test.runtime.python;

public abstract class PythonRunner : RuntimeRunner {
	//////@Override
	public String getExtension() { return "py"; }

	////@Override
	protected void addExtraRecognizerParameters(ST template) {
		template.Add("python3", getLanguage().Equals("Python3"));
	}
}
