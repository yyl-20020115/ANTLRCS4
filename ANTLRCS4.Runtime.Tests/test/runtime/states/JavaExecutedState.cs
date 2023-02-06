/*
 * Copyright (c) 2012-2022 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime.tree;

namespace org.antlr.v4.test.runtime.states;

public class JavaExecutedState : ExecutedState
{
    public readonly ParseTree parseTree;

    public JavaExecutedState(JavaCompiledState previousState, String output, String errors, ParseTree parseTree,
                             Exception exception) : base(previousState, output, errors, exception)
    {
        ;
        this.parseTree = parseTree;
    }
}
