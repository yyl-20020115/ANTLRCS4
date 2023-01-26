/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

namespace org.antlr.v4.runtime.atn;

/** The block that begins a closure loop. */
public class StarBlockStartState : BlockStartState {

	
	public override int getStateType() {
		return STAR_BLOCK_START;
	}
}
