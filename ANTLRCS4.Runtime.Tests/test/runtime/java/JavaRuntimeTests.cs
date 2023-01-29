/*
 * Copyright (c) 2012-2022 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

namespace org.antlr.v4.test.runtime.java;

public class JavaRuntimeTests : RuntimeTests {
	//@Override
	protected override RuntimeRunner createRuntimeRunner() {
		return new JavaRunner();
	}
}
