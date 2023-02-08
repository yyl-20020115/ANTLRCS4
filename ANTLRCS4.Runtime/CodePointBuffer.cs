/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found @In the LICENSE.txt file @In the project root.
 */

using org.antlr.v4.runtime.atn;
using org.antlr.v4.runtime.dfa;
using org.antlr.v4.runtime.misc;

namespace org.antlr.v4.runtime;

/**
 * Wrapper for {@link ByteBuffer} / {@link CharBuffer} / {@link IntBuffer}.
 *
 * Because Java lacks generics on primitive types, these three types
 * do not share an interface, so we have to write one manually.
 */
public class CodePointBuffer
{
    public enum Type : uint
    {
        BYTE,
        CHAR,
        INT
    }
    private readonly Type type;
    private readonly ByteBuffer byteBuffer;
    private readonly CharBuffer charBuffer;
    private readonly IntBuffer intBuffer;

    private CodePointBuffer(Type type, ByteBuffer byteBuffer, CharBuffer charBuffer, IntBuffer intBuffer)
    {
        this.type = type;
        this.byteBuffer = byteBuffer;
        this.charBuffer = charBuffer;
        this.intBuffer = intBuffer;
    }

    public static CodePointBuffer WithBytes(ByteBuffer byteBuffer)
    {
        return new CodePointBuffer(Type.BYTE, byteBuffer, null, null);
    }

    public static CodePointBuffer WithChars(CharBuffer charBuffer)
    {
        return new CodePointBuffer(Type.CHAR, null, charBuffer, null);
    }

    public static CodePointBuffer WithInts(IntBuffer intBuffer)
    {
        return new CodePointBuffer(Type.INT, null, null, intBuffer);
    }

    public int Position() => type switch
    {
        Type.BYTE => byteBuffer.Position(),
        Type.CHAR => charBuffer.Position(),
        Type.INT => intBuffer.Position(),
        _ => throw new UnsupportedOperationException("Not reached"),
    };

    public void Position(int newPosition)
    {
        switch (type)
        {
            case Type.BYTE:
                byteBuffer.Position(newPosition);
                break;
            case Type.CHAR:
                charBuffer.Position(newPosition);
                break;
            case Type.INT:
                intBuffer.Position(newPosition);
                break;
        }
    }

    public int Remaining() => type switch
    {
        Type.BYTE => byteBuffer.Remaining(),
        Type.CHAR => charBuffer.Remaining(),
        Type.INT => intBuffer.Remaining(),
        _ => throw new UnsupportedOperationException("Not reached"),
    };

    public int Get(int offset) => type switch
    {
        Type.BYTE => byteBuffer.Get(offset),
        Type.CHAR => charBuffer.Get(offset),
        Type.INT => intBuffer.Get(offset),
        _ => throw new UnsupportedOperationException("Not reached"),
    };

    public Type Type1 => type;

    public int ArrayOffset() => type switch
    {
        Type.BYTE => byteBuffer.ArrayOffset(),
        Type.CHAR => charBuffer.ArrayOffset(),
        Type.INT => intBuffer.ArrayOffset(),
        _ => throw new UnsupportedOperationException("Not reached"),
    };

    public byte[] ByteArray()
    {
        //assert type == Type.BYTE;
        return byteBuffer.Array();
    }

    public char[] CharArray()
    {
        //assert type == Type.CHAR;
        return charBuffer.Array();
    }

    public int[] IntArray()
    {
        //assert type == Type.INT;
        return intBuffer.Array();
    }

    public static Builder GetBuilder(int initialBufferSize)
    {
        return new Builder(initialBufferSize);
    }

    public class Builder
    {
        private Type type;
        private ByteBuffer byteBuffer;
        private CharBuffer charBuffer;
        private IntBuffer intBuffer;
        private int prevHighSurrogate;

        public Builder(int initialBufferSize)
        {
            type = Type.BYTE;
            byteBuffer = ByteBuffer.Allocate(initialBufferSize);
            charBuffer = null;
            intBuffer = null;
            prevHighSurrogate = -1;
        }

        Type Type => type;

        ByteBuffer ByteBuffer => byteBuffer;

        CharBuffer CharBuffer => charBuffer;

        IntBuffer IntBuffer => intBuffer;

        public CodePointBuffer Build()
        {
            switch (type)
            {
                case Type.BYTE:
                    byteBuffer.Flip();
                    break;
                case Type.CHAR:
                    charBuffer.Flip();
                    break;
                case Type.INT:
                    intBuffer.Flip();
                    break;
            }
            return new CodePointBuffer(type, byteBuffer, charBuffer, intBuffer);
        }

        private static int RoundUpToNextPowerOfTwo(int i)
        {
            int nextPowerOfTwo = 32 - RuntimeUtils.NumberOfLeadingZeros(i - 1);
            return (int)Math.Pow(2, nextPowerOfTwo);
        }

        public void EnsureRemaining(int remainingNeeded)
        {
            switch (type)
            {
                case Type.BYTE:
                    if (byteBuffer.Remaining() < remainingNeeded)
                    {
                        int newCapacity = RoundUpToNextPowerOfTwo(byteBuffer.Capacity() + remainingNeeded);
                        var newBuffer = ByteBuffer.Allocate(newCapacity);
                        byteBuffer.Flip();
                        newBuffer.Put(byteBuffer);
                        byteBuffer = newBuffer;
                    }
                    break;
                case Type.CHAR:
                    if (charBuffer.Remaining() < remainingNeeded)
                    {
                        int newCapacity = RoundUpToNextPowerOfTwo(charBuffer.Capacity() + remainingNeeded);
                        var newBuffer = CharBuffer.Allocate(newCapacity);
                        charBuffer.Flip();
                        newBuffer.Put(charBuffer);
                        charBuffer = newBuffer;
                    }
                    break;
                case Type.INT:
                    if (intBuffer.Remaining() < remainingNeeded)
                    {
                        int newCapacity = RoundUpToNextPowerOfTwo(intBuffer.Capacity() + remainingNeeded);
                        var newBuffer = IntBuffer.Allocate(newCapacity);
                        intBuffer.Flip();
                        newBuffer.Put(intBuffer);
                        intBuffer = newBuffer;
                    }
                    break;
            }
        }

        public void Append(CharBuffer utf16In)
        {
            EnsureRemaining(utf16In.Remaining());
            if (utf16In.HasArray())
            {
                AppendArray(utf16In);
            }
            else
            {
                // TODO
                throw new UnsupportedOperationException("TODO");
            }
        }

        private void AppendArray(CharBuffer utf16In)
        {
            //assert utf16In.hasArray();

            switch (type)
            {
                case Type.BYTE:
                    AppendArrayByte(utf16In);
                    break;
                case Type.CHAR:
                    AppendArrayChar(utf16In);
                    break;
                case Type.INT:
                    AppendArrayInt(utf16In);
                    break;
            }
        }

        private void AppendArrayByte(CharBuffer utf16In)
        {
            //assert prevHighSurrogate == -1;

            char[] @In = utf16In.Array();
            int inOffset = utf16In.ArrayOffset() + utf16In.Position();
            int inLimit = utf16In.ArrayOffset() + utf16In.Limit();

            byte[] outByte = byteBuffer.Array();
            int outOffset = byteBuffer.ArrayOffset() + byteBuffer.Position();

            while (inOffset < inLimit)
            {
                char c = @In[inOffset];
                if (c <= 0xFF)
                {
                    outByte[outOffset] = (byte)(c & 0xFF);
                }
                else
                {
                    utf16In.Position(inOffset - utf16In.ArrayOffset());
                    byteBuffer.Position(outOffset - byteBuffer.ArrayOffset());
                    if (!char.IsHighSurrogate(c))
                    {
                        ByteToCharBuffer(utf16In.Remaining());
                        AppendArrayChar(utf16In);
                        return;
                    }
                    else
                    {
                        ByteToIntBuffer(utf16In.Remaining());
                        AppendArrayInt(utf16In);
                        return;
                    }
                }
                inOffset++;
                outOffset++;
            }

            utf16In.Position(inOffset - utf16In.ArrayOffset());
            byteBuffer.Position(outOffset - byteBuffer.ArrayOffset());
        }

        private void AppendArrayChar(CharBuffer utf16In)
        {
            //assert prevHighSurrogate == -1;

            char[] @In = utf16In.Array();
            int inOffset = utf16In.ArrayOffset() + utf16In.Position();
            int inLimit = utf16In.ArrayOffset() + utf16In.Limit();

            char[] outChar = charBuffer.Array();
            int outOffset = charBuffer.ArrayOffset() + charBuffer.Position();

            while (inOffset < inLimit)
            {
                char c = @In[inOffset];
                if (!char.IsHighSurrogate(c))
                {
                    outChar[outOffset] = c;
                }
                else
                {
                    utf16In.Position(inOffset - utf16In.ArrayOffset());
                    charBuffer.Position(outOffset - charBuffer.ArrayOffset());
                    CharToIntBuffer(utf16In.Remaining());
                    AppendArrayInt(utf16In);
                    return;
                }
                inOffset++;
                outOffset++;
            }

            utf16In.Position(inOffset - utf16In.ArrayOffset());
            charBuffer.Position(outOffset - charBuffer.ArrayOffset());
        }

        private void AppendArrayInt(CharBuffer utf16In)
        {
            char[] @In = utf16In.Array();
            int inOffset = utf16In.ArrayOffset() + utf16In.Position();
            int inLimit = utf16In.ArrayOffset() + utf16In.Limit();

            int[] outInt = intBuffer.Array();
            int outOffset = intBuffer.ArrayOffset() + intBuffer.Position();

            while (inOffset < inLimit)
            {
                char c = @In[inOffset];
                inOffset++;
                if (prevHighSurrogate != -1)
                {
                    if (char.IsLowSurrogate(c))
                    {
                        outInt[outOffset] = char.ConvertToUtf32((char)prevHighSurrogate, c);
                        outOffset++;
                        prevHighSurrogate = -1;
                    }
                    else
                    {
                        // Dangling high surrogate
                        outInt[outOffset] = prevHighSurrogate;
                        outOffset++;
                        if (char.IsHighSurrogate(c))
                        {
                            prevHighSurrogate = c & 0xFFFF;
                        }
                        else
                        {
                            outInt[outOffset] = c & 0xFFFF;
                            outOffset++;
                            prevHighSurrogate = -1;
                        }
                    }
                }
                else if (char.IsHighSurrogate(c))
                {
                    prevHighSurrogate = c & 0xFFFF;
                }
                else
                {
                    outInt[outOffset] = c & 0xFFFF;
                    outOffset++;
                }
            }

            if (prevHighSurrogate != -1)
            {
                // Dangling high surrogate
                outInt[outOffset] = prevHighSurrogate & 0xFFFF;
                outOffset++;
            }

            utf16In.Position(inOffset - utf16In.ArrayOffset());
            intBuffer.Position(outOffset - intBuffer.ArrayOffset());
        }

        private void ByteToCharBuffer(int toAppend)
        {
            byteBuffer.Flip();
            // CharBuffers hold twice as much per unit as ByteBuffers, so start with half the capacity.
            var newBuffer = CharBuffer.Allocate(Math.Max(byteBuffer.Remaining() + toAppend, byteBuffer.Capacity() / 2));
            while (byteBuffer.HasRemaining())
            {
                newBuffer.Put((char)(byteBuffer.Get() & 0xFF));
            }
            type = Type.CHAR;
            byteBuffer = null;
            charBuffer = newBuffer;
        }

        private void ByteToIntBuffer(int toAppend)
        {
            byteBuffer.Flip();
            // IntBuffers hold four times as much per unit as ByteBuffers, so start with one quarter the capacity.
            var newBuffer = IntBuffer.Allocate(Math.Max(byteBuffer.Remaining() + toAppend, byteBuffer.Capacity() / 4));
            while (byteBuffer.HasRemaining())
            {
                newBuffer.Put(byteBuffer.Get() & 0xFF);
            }
            type = Type.INT;
            byteBuffer = null;
            intBuffer = newBuffer;
        }

        private void CharToIntBuffer(int toAppend)
        {
            charBuffer.Flip();
            // IntBuffers hold two times as much per unit as ByteBuffers, so start with one half the capacity.
            var newBuffer = IntBuffer.Allocate(Math.Max(charBuffer.Remaining() + toAppend, charBuffer.Capacity() / 2));
            while (charBuffer.HasRemaining())
            {
                newBuffer.Put(charBuffer.Get() & 0xFFFF);
            }
            type = Type.INT;
            charBuffer = null;
            intBuffer = newBuffer;
        }
    }
}
