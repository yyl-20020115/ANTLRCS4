/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

namespace org.antlr.v4.runtime.atn;

/** Mark the end of a * or + loop. */
public class LoopEndState : ATNState
{
    public ATNState loopBackState;
    public override int StateType => LOOP_END;
}
