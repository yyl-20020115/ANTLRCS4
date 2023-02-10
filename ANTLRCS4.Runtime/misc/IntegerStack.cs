/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */
namespace org.antlr.v4.runtime.misc;

/**
 *
 * @author Sam Harwell
 */
public class IntegerStack : IntegerList
{

    public IntegerStack() { }

    public IntegerStack(int capacity) : base(capacity) { }

    public IntegerStack(IntegerStack list) : base(list) { }

    public void Push(int value) => Add(value);

    public int Pop() => RemoveAt(Count - 1);

    public int Peek() => Get(Count - 1);

}
