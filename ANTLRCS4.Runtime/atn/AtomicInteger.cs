/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

namespace org.antlr.v4.runtime.atn;

public class AtomicInteger
{
    private int value;

    /// <summary>
    /// Creates a new <c>Atomic</c> instance with an initial value of <c>null</c>.
    /// </summary>

    /// <summary>
    /// Creates a new <c>Atomic</c> instance with the initial value provided.
    /// </summary>
    public AtomicInteger(int value = 0) 
        => this.value = value;

    /// <summary>
    /// This method returns the current value.
    /// </summary>
    /// <returns>
    /// The <c>T</c> instance.
    /// </returns>
    /// <summary>
    /// This method sets the current value atomically.
    /// </summary>
    /// <param name="value">
    /// The new value to set.
    /// </param>
    public int Value
    {
        get => value;
        set => Interlocked.Exchange(ref this.value, value);
    }

    public int GetAndIncrement()
        => this.GetAndSet(this.value + 1);

    /// <summary>
    /// This method atomically sets the value and returns the original value.
    /// </summary>
    /// <param name="value">
    /// The new value.
    /// </param>
    /// <returns>
    /// The value before setting to the new value.
    /// </returns>
    public int GetAndSet(int value) 
        => Interlocked.Exchange(ref this.value, value);

    /// <summary>
    /// Atomically sets the value to the given updated value if the current value <c>==</c> the expected value.
    /// </summary>
    /// <param name="expected">
    /// The value to compare against.
    /// </param>
    /// <param name="result">
    /// The value to set if the value is equal to the <c>expected</c> value.
    /// </param>
    /// <returns>
    /// <c>true</c> if the comparison and set was successful. A <c>false</c> indicates the comparison failed.
    /// </returns>
    public bool CompareAndSet(int expected, int result) 
        => Interlocked.CompareExchange(ref value, result, expected) == expected;

    /// <summary>
    /// This operator allows an implicit cast from <c>Atomic&lt;T&gt;</c> to <c>T</c>.
    /// </summary>
    public static implicit operator int(AtomicInteger value) 
        => value.Value;
}
