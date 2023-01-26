/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

namespace org.antlr.v4.runtime.atn;

public class SingletonPredictionContext : PredictionContext {
	public readonly PredictionContext parent;
	public readonly int returnState;

	public SingletonPredictionContext(PredictionContext parent, int returnState)
	: base(parent != null ? calculateHashCode(parent, returnState) : calculateEmptyHashCode())
    {
		//assert returnState!=ATNState.INVALID_STATE_NUMBER;
		this.parent = parent;
		this.returnState = returnState;
	}

	public static SingletonPredictionContext create(PredictionContext parent, int returnState) {
		if ( returnState == EMPTY_RETURN_STATE && parent == null ) {
			// someone can pass in the bits of an array ctx that mean $
			return EmptyPredictionContext.Instance;
		}
		return new SingletonPredictionContext(parent, returnState);
	}

	//@Override
	public override int size() {
		return 1;
	}

	//@Override
	public override PredictionContext getParent(int index) {
		//assert index == 0;
		return parent;
	}

	//@Override
	public override int getReturnState(int index) {
        //assert index == 0;
        return returnState;
	}

	//@Override
	public override bool Equals(Object? o) {
		if (this == o) {
			return true;
		}
		else if ( !(o is SingletonPredictionContext) ) {
			return false;
		}

		if ( this.GetHashCode() != o.GetHashCode() ) {
			return false; // can't be same if hash is different
		}

		SingletonPredictionContext s = (SingletonPredictionContext)o;
		return returnState == s.returnState &&
			(parent!=null && parent.Equals(s.parent));
	}

	//@Override
	public override String ToString() {
		String up = parent!=null ? parent.ToString() : "";
		if ( up.Length==0 ) {
			if ( returnState == EMPTY_RETURN_STATE ) {
				return "$";
			}
			return returnState.ToString();

        }
		return returnState.ToString() + " "+up;
	}
}
