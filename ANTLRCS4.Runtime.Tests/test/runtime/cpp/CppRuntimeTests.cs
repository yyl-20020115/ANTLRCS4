/*
 * Copyright (c) 2012-2022 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

namespace org.antlr.v4.test.runtime.cpp;

public class CppRuntimeTests : RuntimeTests
{
    
    protected override RuntimeRunner CreateRuntimeRunner() => new CppRunner();
}
