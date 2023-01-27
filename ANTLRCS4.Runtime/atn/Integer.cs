/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

namespace org.antlr.v4.runtime.atn;

public class Integer
{
    public static int bitCount(long v)
    {
        v = v - ((v >>> 1) & 0x5555555555555555L);
        v = (v & 0x3333333333333333L) + ((v >>> 2) & 0x3333333333333333L);
        v = (v + (v >>> 4)) & 0x0f0f0f0f0f0f0f0fL;
        v = v + (v >>> 8);
        v = v + (v >>> 16);
        v = v + (v >>> 32);
        return (int)v & 0x7f;
    }

    public static int numberOfLeadingZeros(long v)
    {
        int x = (int)(v >>> 32);
        return x == 0 ? 32 + Integer.numberOfLeadingZeros((int)v)
                : Integer.numberOfLeadingZeros(x);
    }

    public static int numberOfTrailingZeros(long v)
    {
        int x = (int)v;
        return x == 0 ? 32 + Integer.numberOfTrailingZeros((int)(v >>> 32))
                : Integer.numberOfTrailingZeros(x);
    }

    public static int bitCount(int v)
    {
        v = v - ((v >>> 1) & 0x55555555);
        v = (v & 0x33333333) + ((v >>> 2) & 0x33333333);
        v = (v + (v >>> 4)) & 0x0f0f0f0f;
        v = v + (v >>> 8);
        v = v + (v >>> 16);
        return v & 0x3f;
    }

    public static int numberOfLeadingZeros(int v)
    {
        if (v <= 0)
            return v == 0 ? 32 : 0;
        int n = 31;
        if (v >= 1 << 16) { n -= 16; v >>>= 16; }
        if (v >= 1 << 8) { n -= 8; v >>>= 8; }
        if (v >= 1 << 4) { n -= 4; v >>>= 4; }
        if (v >= 1 << 2) { n -= 2; v >>>= 2; }
        return n - (v >>> 1);
    }

    public static int numberOfTrailingZeros(int v)
    {
        v = ~v & (v - 1);
        if (v <= 0) return v & 32;
        int n = 1;
        if (v > 1 << 16) { n += 16; v >>>= 16; }
        if (v > 1 << 8) { n += 8; v >>>= 8; }
        if (v > 1 << 4) { n += 4; v >>>= 4; }
        if (v > 1 << 2) { n += 2; v >>>= 2; }
        return n + (v >>> 1);
    }

}