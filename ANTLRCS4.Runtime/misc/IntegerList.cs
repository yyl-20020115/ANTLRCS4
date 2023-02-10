/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */
using System.Text;

namespace org.antlr.v4.runtime.misc;


/**
 *
 * @author Sam Harwell
 */
public class IntegerList
{

    private readonly static int[] EMPTY_DATA = Array.Empty<int>();
    private static readonly int INITIAL_SIZE = 4;
    private static readonly int MAX_ARRAY_SIZE = int.MaxValue - 8;


    private int[] _data = EMPTY_DATA;
    private int count = 0;

    public IntegerList() { }

    public IntegerList(int capacity)
    {
        if (capacity < 0)
        {
            throw new ArgumentException(null, nameof(capacity));
        }

        _data = capacity == 0 ? EMPTY_DATA : (new int[capacity]);
    }

    public IntegerList(IntegerList list)
    {
        _data = (int[])list._data.Clone();
        count = list.count;
    }

    public IntegerList(ICollection<int> list) : this(list.Count)
    {
        foreach (var value in list)
            Add(value);
    }

    public void Add(int value)
    {
        if (_data.Length == count)
            EnsureCapacity(count + 1);

        _data[count] = value;
        count++;
    }

    public void AddAll(int[] array)
    {
        EnsureCapacity(count + array.Length);
        Array.Copy(array, 0, _data, count, array.Length);
        count += array.Length;
    }

    public void AddAll(IntegerList list)
    {
        EnsureCapacity(count + list.count);
        Array.Copy(list._data, 0, _data, count, list.count);
        count += list.count;
    }

    public void AddAll(ICollection<int> list)
    {
        EnsureCapacity(count + list.Count);
        int current = 0;
        foreach (int x in list)
        {
            _data[count + current] = x;
            current++;
        }
        count += list.Count;
    }

    public int Get(int index)
    {
        if (index < 0 || index >= count)
            throw new IndexOutOfRangeException(nameof(index));

        return _data[index];
    }

    public bool Contains(int value)
    {
        for (int i = 0; i < count; i++)
            if (_data[i] == value)
                return true;

        return false;
    }

    public int Set(int index, int value)
    {
        if (index < 0 || index >= count)
            throw new IndexOutOfRangeException(nameof(index));

        int previous = _data[index];
        _data[index] = value;
        return previous;
    }

    public int RemoveAt(int index)
    {
        int value = Get(index);
        Array.Copy(_data, index + 1, _data, index, count - index - 1);
        _data[count - 1] = 0;
        count--;
        return value;
    }

    public void RemoveRange(int fromIndex, int toIndex)
    {
        if (fromIndex < 0 || toIndex < 0 || fromIndex > count || toIndex > count)
            throw new IndexOutOfRangeException(nameof(fromIndex));
        if (fromIndex > toIndex)
            throw new ArgumentException(nameof(fromIndex));

        Array.Copy(_data, toIndex, _data, fromIndex, count - toIndex);
        Array.Fill(_data, 0, count - (toIndex - fromIndex), count);
        //Arrays.fill(_data, _size - (toIndex - fromIndex), _size, 0);
        count -= (toIndex - fromIndex);
    }

    public bool IsEmpty => count == 0;

    public int Count => count;

    public void TrimToSize()
    {
        if (_data.Length == count) return;
        _data = Arrays.CopyOf(_data, count);
    }

    public void Clear()
    {
        Array.Fill(_data, count, 0, 0);
        count = 0;
    }

    public int[] ToArray()
    {
        if (count == 0) return EMPTY_DATA;
        return Arrays.CopyOf(_data, count);
    }

    public void Sort()
    {
        Array.Sort(_data, 0, count);
    }

    /**
	 * Compares the specified object with this list for equality.  Returns
	 * {@code true} if and only if the specified object is also an {@link IntegerList},
	 * both lists have the same size, and all corresponding pairs of elements in
	 * the two lists are equal.  In other words, two lists are defined to be
	 * equal if they contain the same elements in the same order.
	 * <p>
	 * This implementation first checks if the specified object is this
	 * list. If so, it returns {@code true}; if not, it checks if the
	 * specified object is an {@link IntegerList}. If not, it returns {@code false};
	 * if so, it checks the size of both lists. If the lists are not the same size,
	 * it returns {@code false}; otherwise it iterates over both lists, comparing
	 * corresponding pairs of elements.  If any comparison returns {@code false},
	 * this method returns {@code false}.
	 *
	 * @param o the object to be compared for equality with this list
	 * @return {@code true} if the specified object is equal to this list
	 */
    public override bool Equals(object? o)
    {
        if (o == this)
        {
            return true;
        }

        if (o is not IntegerList other)
        {
            return false;
        }

        if (count != other.count)
        {
            return false;
        }

        for (int i = 0; i < count; i++)
        {
            if (_data[i] != other._data[i])
            {
                return false;
            }
        }

        return true;
    }

    /**
	 * Returns the hash code value for this list.
	 *
	 * <p>This implementation uses exactly the code that is used to define the
	 * list hash function in the documentation for the {@link List#hashCode}
	 * method.</p>
	 *
	 * @return the hash code value for this list
	 */
    public override int GetHashCode()
    {
        int hashCode = 1;
        for (int i = 0; i < count; i++)
        {
            hashCode = 31 * hashCode + _data[i];
        }

        return hashCode;
    }

    /**
	 * Returns a string representation of this list.
	 */
    public override string ToString() => string.Join(',', ToArray());

    public int BinarySearch(int key) => Array.BinarySearch(_data, 0, count, key);

    public int BinarySearch(int fromIndex, int toIndex, int key)
    {
        if (fromIndex < 0 || toIndex < 0 || fromIndex > count || toIndex > count)
        {
            throw new IndexOutOfRangeException();
        }
        if (fromIndex > toIndex)
        {
            throw new ArgumentException(nameof(fromIndex));
        }

        return Array.BinarySearch(_data, fromIndex, toIndex, key);
    }

    private void EnsureCapacity(int capacity)
    {
        if (capacity < 0 || capacity > MAX_ARRAY_SIZE)
        {
            throw new OutOfMemoryException();
        }

        int newLength = _data.Length == 0 ? INITIAL_SIZE : _data.Length;

        while (newLength < capacity)
        {
            newLength <<= 1;
            if (newLength < 0 || newLength > MAX_ARRAY_SIZE)
            {
                newLength = MAX_ARRAY_SIZE;
            }
        }

        _data = Arrays.CopyOf(_data, newLength);
    }
    public const int MIN_SUPPLEMENTARY_CODE_POINT = 0x010000;
    public const int MAX_CODE_POINT = 0X10FFFF;
    public static bool IsSupplementaryCodePoint(int codePoint)
    {
        return codePoint >= MIN_SUPPLEMENTARY_CODE_POINT
            && codePoint < MAX_CODE_POINT + 1;
    }
    /** Convert the int list to a char array where values > 0x7FFFF take 2 bytes. TODO?????
	 *  If all values are less
	 *  than the 0x7FFF 16-bit code point limit (1 bit taken to indicatethen this is just a char array
	 *  of 16-bit char as usual. For values in the supplementary range, encode
	 * them as two UTF-16 code units.
	 */
    public char[] ToCharArray()
    {
        // Optimize for the common case (all data values are
        // < 0xFFFF) to avoid an extra scan
        char[] resultArray = new char[count];
        int resultIdx = 0;
        bool calculatedPreciseResultSize = false;
        for (int i = 0; i < count; i++)
        {
            int codePoint = _data[i];
            // Calculate the precise result size if we encounter
            // a code point > 0xFFFF
            if (!calculatedPreciseResultSize &&
                IsSupplementaryCodePoint(codePoint))
            {
                resultArray = Arrays.CopyOf(resultArray, CharArraySize());
                calculatedPreciseResultSize = true;
            }
            // This will throw IllegalArgumentException if
            // the code point is not a valid Unicode code point
            var r = new Rune(codePoint);
            var a = r.ToString().ToCharArray();

            int charsWritten = a.Length;
            if (charsWritten == 1)
            {
                resultArray[resultIdx] = a[0];
            }
            else if (charsWritten == 2)
            {
                resultArray[resultIdx] = a[0];
                resultArray[resultIdx + 1] = a[1];
            }

            resultIdx += charsWritten;
        }
        return resultArray;
    }

    private int CharArraySize()
    {
        int result = 0;
        for (int i = 0; i < count; i++)
            result += new Rune(_data[i]).Utf16SequenceLength;
        return result;
    }
}
