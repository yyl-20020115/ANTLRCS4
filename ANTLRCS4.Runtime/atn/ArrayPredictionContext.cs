/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using System.Text;

namespace org.antlr.v4.runtime.atn;

public class ArrayPredictionContext : PredictionContext
{
    /** Parent can be null only if full ctx mode and we make an array
	 *  from {@link #EMPTY} and non-empty. We merge {@link #EMPTY} by using null parent and
	 *  returnState == {@link #EMPTY_RETURN_STATE}.
	 */
    public readonly PredictionContext[] parents;

    /** Sorted for merge, no duplicates; if present,
	 *  {@link #EMPTY_RETURN_STATE} is always last.
 	 */
    public readonly int[] returnStates;

    public ArrayPredictionContext(SingletonPredictionContext a) : this(new PredictionContext[] { a.parent }, new int[] { a.returnState })
    {
    }

    public ArrayPredictionContext(PredictionContext[] parents, int[] returnStates) : base(CalculateHashCode(parents, returnStates))
    {
        //assert parents!=null && parents.length>0;
        //assert returnStates!=null && returnStates.length>0;
        //		Console.Error.WriteLine("CREATE ARRAY: "+Arrays.toString(parents)+", "+Arrays.toString(returnStates));
        this.parents = parents;
        this.returnStates = returnStates;
    }

    public override bool IsEmpty =>
        // since EMPTY_RETURN_STATE can only appear in the last position, we
        // don't need to verify that size==1
        returnStates[0] == EMPTY_RETURN_STATE;

    public override int Count => returnStates.Length;

    public override PredictionContext GetParent(int index) => parents[index];

    public override int GetReturnState(int index) => returnStates[index];

    //	@Override
    //	public int findReturnState(int returnState) {
    //		return Arrays.binarySearch(returnStates, returnState);
    //	}

    public override bool Equals(object? o)
    {
        if (this == o)
        {
            return true;
        }
        else if (o is not ArrayPredictionContext)
        {
            return false;
        }

        if (this.GetHashCode() != o.GetHashCode())
        {
            return false; // can't be same if hash is different
        }

        ArrayPredictionContext a = (ArrayPredictionContext)o;
        return Enumerable.SequenceEqual(returnStates, a.returnStates) &&
               Enumerable.SequenceEqual(parents, a.parents);
    }

    public override string ToString()
    {
        if (IsEmpty) return "[]";
        var buffer = new StringBuilder();
        buffer.Append('[');
        for (int i = 0; i < returnStates.Length; i++)
        {
            if (i > 0) buffer.Append(", ");
            if (returnStates[i] == EMPTY_RETURN_STATE)
            {
                buffer.Append('$');
                continue;
            }
            buffer.Append(returnStates[i]);
            if (parents[i] != null)
            {
                buffer.Append(' ');
                buffer.Append(parents[i].ToString());
            }
            else
            {
                buffer.Append("null");
            }
        }
        buffer.Append(']');
        return buffer.ToString();
    }
}
