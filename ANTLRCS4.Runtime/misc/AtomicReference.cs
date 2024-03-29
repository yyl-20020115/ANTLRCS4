﻿/* Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */
namespace org.antlr.v4.runtime.misc;

public class AtomicReference<T>
    where T : class
{
    private volatile T _value;

    public AtomicReference(T value = default) => _value = value;

    public T Get() => _value;

    public void Set(T value) => _value = value;

    public bool CompareAndSet(T expect, T update) 
        => Interlocked.CompareExchange(ref _value, update, expect) == expect;

    public T GetAndSet(T value) 
        => Interlocked.Exchange(ref _value, value);
}
