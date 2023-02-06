/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

namespace org.antlr.v4.runtime.atn;

/** Terminal node of a simple {@code (a|b|c)} block. */
public class BlockEndState : ATNState
{
    public BlockStartState startState;

    public override int StateType => BLOCK_END;
}
