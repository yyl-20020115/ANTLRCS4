/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

namespace org.antlr.v4.runtime.atn;

public class StarLoopbackState : ATNState
{
    public StarLoopEntryState LoopEntryState => (StarLoopEntryState)Transition(0).target;

    public override int StateType => STAR_LOOP_BACK;
}
