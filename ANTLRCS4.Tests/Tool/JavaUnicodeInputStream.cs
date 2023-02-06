/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime;
using org.antlr.v4.runtime.dfa;
using org.antlr.v4.runtime.misc;

namespace org.antlr.v4.test.tool;
/**
 *
 * @author Sam Harwell
 */
public class JavaUnicodeInputStream : CharStream
{
    private readonly CharStream source;
    private readonly IntegerList escapeIndexes = new ();
    private readonly IntegerList escapeCharacters = new ();
    private readonly IntegerList escapeIndirectionLevels = new ();

    private int escapeListIndex;
    private int range;
    private int slashCount;

    private int la1;

    public JavaUnicodeInputStream(CharStream source)
    {
        this.source = source ?? throw new NullReferenceException("source");
        this.la1 = source.LA(1);
    }

    ////@Override
    public int Count => source.Count;

    ////@Override
    public int Index()
    {
        return source.Index();
    }

    ////@Override
    public String GetSourceName()
    {
        return source.GetSourceName();
    }

    ////@Override
    public string GetText(Interval interval)
    {
        return source.GetText(interval);
    }

    ////@Override
    public void Consume()
    {
        if (la1 != '\\')
        {
            source.Consume();
            la1 = source.LA(1);
            range = Math.Max(range, source.Index());
            slashCount = 0;
            return;
        }

        // make sure the next character has been processed
        this.LA(1);

        if (escapeListIndex >= escapeIndexes.Size || escapeIndexes.Get(escapeListIndex) != Index())
        {
            source.Consume();
            slashCount++;
        }
        else
        {
            int indirectionLevel = escapeIndirectionLevels.Get(escapeListIndex);
            for (int i = 0; i < 6 + indirectionLevel; i++)
            {
                source.Consume();
            }

            escapeListIndex++;
            slashCount = 0;
        }

        la1 = source.LA(1);
        //assert range >= index();
    }

    ////@Override
    public int LA(int i)
    {
        if (i == 1 && la1 != '\\')
        {
            return la1;
        }

        if (i <= 0)
        {
            int desiredIndex = Index() + i;
            for (int j = escapeListIndex - 1; j >= 0; j--)
            {
                if (escapeIndexes.Get(j) + 6 + escapeIndirectionLevels.Get(j) > desiredIndex)
                {
                    desiredIndex -= 5 + escapeIndirectionLevels.Get(j);
                }

                if (escapeIndexes.Get(j) == desiredIndex)
                {
                    return escapeCharacters.Get(j);
                }
            }

            return source.LA(desiredIndex - Index());
        }
        else
        {
            int desiredIndex = Index() + i - 1;
            for (int j = escapeListIndex; j < escapeIndexes.Size; j++)
            {
                if (escapeIndexes.Get(j) == desiredIndex)
                {
                    return escapeCharacters.Get(j);
                }
                else if (escapeIndexes.Get(j) < desiredIndex)
                {
                    desiredIndex += 5 + escapeIndirectionLevels.Get(j);
                }
                else
                {
                    return source.LA(desiredIndex - Index() + 1);
                }
            }

            int[] currentIndex = { Index() };
            int[] slashCountPtr = { slashCount };
            int[] indirectionLevelPtr = { 0 };
            for (int j = 0; j < i; j++)
            {
                int previousIndex = currentIndex[0];
                int c = readCharAt(currentIndex, slashCountPtr, indirectionLevelPtr);
                if (currentIndex[0] > range)
                {
                    if (currentIndex[0] - previousIndex > 1)
                    {
                        escapeIndexes.Add(previousIndex);
                        escapeCharacters.Add(c);
                        escapeIndirectionLevels.Add(indirectionLevelPtr[0]);
                    }

                    range = currentIndex[0];
                }

                if (j == i - 1)
                {
                    return c;
                }
            }

            throw new IllegalStateException("shouldn't be reachable");
        }
    }

    ////@Override
    public int Mark()
    {
        return source.Mark();
    }

    ////@Override
    public void Release(int marker)
    {
        source.Release(marker);
    }

    ////@Override
    public void Seek(int index)
    {
        if (index > range)
        {
            throw new UnsupportedOperationException();
        }

        source.Seek(index);
        la1 = source.LA(1);

        slashCount = 0;
        while (source.LA(-slashCount - 1) == '\\')
        {
            slashCount++;
        }

        escapeListIndex = escapeIndexes.BinarySearch(source.Index());
        if (escapeListIndex < 0)
        {
            escapeListIndex = -escapeListIndex - 1;
        }
    }

    private static bool isHexDigit(int c)
    {
        return c >= '0' && c <= '9'
            || c >= 'a' && c <= 'f'
            || c >= 'A' && c <= 'F';
    }

    private static int hexValue(int c)
    {
        if (c >= '0' && c <= '9')
        {
            return c - '0';
        }

        if (c >= 'a' && c <= 'f')
        {
            return c - 'a' + 10;
        }

        if (c >= 'A' && c <= 'F')
        {
            return c - 'A' + 10;
        }

        throw new ArgumentException("c");
    }

    private int readCharAt(int[] nextIndexPtr, int[] slashCountPtr, int[] indirectionLevelPtr)
    {
        //assert nextIndexPtr != null && nextIndexPtr.Length == 1;
        //assert slashCountPtr != null && slashCountPtr.Length == 1;
        //assert indirectionLevelPtr != null && indirectionLevelPtr.Length == 1;

        bool blockUnicodeEscape = (slashCountPtr[0] % 2) != 0;

        int c0 = source.LA(nextIndexPtr[0] - Index() + 1);
        if (c0 == '\\')
        {
            slashCountPtr[0]++;

            if (!blockUnicodeEscape)
            {
                int c1 = source.LA(nextIndexPtr[0] - Index() + 2);
                if (c1 == 'u')
                {
                    int c2 = source.LA(nextIndexPtr[0] - Index() + 3);
                    indirectionLevelPtr[0] = 0;
                    while (c2 == 'u')
                    {
                        indirectionLevelPtr[0]++;
                        c2 = source.LA(nextIndexPtr[0] - Index() + 3 + indirectionLevelPtr[0]);
                    }

                    int c3 = source.LA(nextIndexPtr[0] - Index() + 4 + indirectionLevelPtr[0]);
                    int c4 = source.LA(nextIndexPtr[0] - Index() + 5 + indirectionLevelPtr[0]);
                    int c5 = source.LA(nextIndexPtr[0] - Index() + 6 + indirectionLevelPtr[0]);
                    if (isHexDigit(c2) && isHexDigit(c3) && isHexDigit(c4) && isHexDigit(c5))
                    {
                        int value = hexValue(c2);
                        value = (value << 4) + hexValue(c3);
                        value = (value << 4) + hexValue(c4);
                        value = (value << 4) + hexValue(c5);

                        nextIndexPtr[0] += 6 + indirectionLevelPtr[0];
                        slashCountPtr[0] = 0;
                        return value;
                    }
                }
            }
        }

        nextIndexPtr[0]++;
        return c0;
    }

    public int CharPositionInLine => throw new NotImplementedException();

    public int Line => throw new NotImplementedException();

    public int LT(int v)
    {
        throw new NotImplementedException();
    }

    public string Substring(int tokenStartCharIndex, int v)
    {
        throw new NotImplementedException();
    }

    public void Rewind(int nvaeMark)
    {
        throw new NotImplementedException();
    }

    public void Rewind()
    {
        throw new NotImplementedException();
    }
}
