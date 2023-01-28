/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using System.Text;

namespace org.antlr.v4.runtime.atn;

public class ArrayPredictionContext : PredictionContext {
	/** Parent can be null only if full ctx mode and we make an array
	 *  from {@link #EMPTY} and non-empty. We merge {@link #EMPTY} by using null parent and
	 *  returnState == {@link #EMPTY_RETURN_STATE}.
	 */
	public readonly PredictionContext[] parents;

	/** Sorted for merge, no duplicates; if present,
	 *  {@link #EMPTY_RETURN_STATE} is always last.
 	 */
	public readonly int[] returnStates;

	public ArrayPredictionContext(SingletonPredictionContext a): this(new PredictionContext[] { a.parent }, new int[] { a.returnState })
    {
	}

	public ArrayPredictionContext(PredictionContext[] parents, int[] returnStates) : base(calculateHashCode(parents, returnStates))
    {
        //assert parents!=null && parents.length>0;
        //assert returnStates!=null && returnStates.length>0;
        //		Console.Error.WriteLine("CREATE ARRAY: "+Arrays.toString(parents)+", "+Arrays.toString(returnStates));
        this.parents = parents;
		this.returnStates = returnStates;
	}

	public override bool isEmpty() {
		// since EMPTY_RETURN_STATE can only appear in the last position, we
		// don't need to verify that size==1
		return returnStates[0]==EMPTY_RETURN_STATE;
	}

	public override int size() {
		return returnStates.Length;
	}

	public override PredictionContext getParent(int index) {
		return parents[index];
	}

	public override int getReturnState(int index) {
		return returnStates[index];
	}

//	@Override
//	public int findReturnState(int returnState) {
//		return Arrays.binarySearch(returnStates, returnState);
//	}

	public override bool Equals(Object o) {
		if (this == o) {
			return true;
		}
		else if ( o is not ArrayPredictionContext ) {
			return false;
		}

		if ( this.GetHashCode() != o.GetHashCode() ) {
			return false; // can't be same if hash is different
		}

		ArrayPredictionContext a = (ArrayPredictionContext)o;
		return Enumerable.SequenceEqual(returnStates, a.returnStates) &&
               Enumerable.SequenceEqual(parents, a.parents);
	}

	public override String ToString() {
		if ( isEmpty() ) return "[]";
		StringBuilder buf = new StringBuilder();
		buf.Append('[');
		for (int i=0; i<returnStates.Length; i++) {
			if ( i>0 ) buf.Append(", ");
			if ( returnStates[i]==EMPTY_RETURN_STATE ) {
				buf.Append("$");
				continue;
			}
			buf.Append(returnStates[i]);
			if ( parents[i]!=null ) {
				buf.Append(' ');
				buf.Append(parents[i].ToString());
			}
			else {
				buf.Append("null");
			}
		}
		buf.Append(']');
		return buf.ToString();
	}
}
