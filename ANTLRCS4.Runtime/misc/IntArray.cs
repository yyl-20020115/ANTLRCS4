﻿/*
 [The "BSD license"]
 Copyright (c) 2005-2009 Terence Parr
 All rights reserved.

 Redistribution and use in source and binary forms, with or without
 modification, are permitted provided that the following conditions
 are met:
 1. Redistributions of source code must retain the above copyright
     notice, this list of conditions and the following disclaimer.
 2. Redistributions in binary form must reproduce the above copyright
     notice, this list of conditions and the following disclaimer in the
     documentation and/or other materials provided with the distribution.
 3. The name of the author may not be used to endorse or promote products
     derived from this software without specific prior written permission.

 THIS SOFTWARE IS PROVIDED BY THE AUTHOR ``AS IS'' AND ANY EXPRESS OR
 IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
 OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.
 IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY DIRECT, INDIRECT,
 INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT
 NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
 DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
 THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
 THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */
namespace org.antlr.runtime.misc;

/** A dynamic array that uses int not Integer objects. In principle this
 *  is more efficient in time, but certainly in space.
 *
 *  This is simple enough that you can access the data array directly,
 *  but make sure that you append elements only with add() so that you
 *  get dynamic sizing.  Make sure to call ensureCapacity() when you are
 *  manually adding new elements.
 *
 *  Doesn't impl List because it doesn't return objects and I mean this
 *  really as just an array not a List per se.  Manipulate the elements
 *  at will.  This has stack methods too.
 *
 *  When runtime can be 1.5, I'll make this generic.
 */
public class IntArray
{
    public const int INITIAL_SIZE = 10;
    public int[] data = new int[INITIAL_SIZE];
    protected int p = -1;

    public IntArray() { }
    public void Add(int v)
    {
        EnsureCapacity(p + 1);
        data[++p] = v;
    }

    public void Push(int v)
    {
        Add(v);
    }

    public int Pop()
    {
        int v = data[p];
        p--;
        return v;
    }

    /** This only tracks elements added via push/add. */
    public int Count => p;

    public void Clear()
    {
        p = -1;
    }

    public void EnsureCapacity(int index)
    {
        if (data == null)
        {
            data = new int[INITIAL_SIZE];
        }
        else if ((index + 1) >= data.Length)
        {
            var newSize = data.Length << 1;
            if (index > newSize)
            {
                newSize = index + 1;
            }
            var newData = new int[newSize];
            Array.Copy(data, 0, newData, 0, data.Length);
            data = newData;
        }
    }
}
