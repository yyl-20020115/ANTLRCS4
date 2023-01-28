/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */
using org.antlr.v4.test.runtime.python;

namespace org.antlr.v4.test.runtime.python3;

public class Python3Runner : PythonRunner {
	public static readonly Dictionary<String, String> environment;

	static Python3Runner(){
		environment = new();
		environment.Add("PYTHONPATH", Path.Combine(getRuntimePath("Python3"), "src"));
		environment.Add("PYTHONIOENCODING", "utf-8");
	}

	////@Override
	public override String getLanguage() {
		return "Python3";
	}

	////@Override
	public Dictionary<String, String> getExecEnvironment() {
		return environment;
	}
}
