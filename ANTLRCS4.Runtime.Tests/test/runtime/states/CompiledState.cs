/*
 * Copyright (c) 2012-2022 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

namespace org.antlr.v4.test.runtime.states;

public class CompiledState : State
{
    
    public override Stage Stage => Stage.Compile;

    public CompiledState(GeneratedState previousState, Exception exception)
        : base(previousState, exception)
    {
    }
}
