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
    private int _size = 0;

    public IntegerList()
    {
    }

    public IntegerList(int capacity)
    {
        if (capacity < 0)
        {
            throw new ArgumentException(null, nameof(capacity));
        }

        if (capacity == 0)
        {
            _data = EMPTY_DATA;
        }
        else
        {
            _data = new int[capacity];
        }
    }

    public IntegerList(IntegerList list)
    {
        _data = (int[])list._data.Clone();
        _size = list._size;
    }

    public IntegerList(ICollection<int> list) : this(list.Count)
    {
        foreach (var value in list)
        {
            add(value);
        }
    }

    public void add(int value)
    {
        if (_data.Length == _size)
        {
            ensureCapacity(_size + 1);
        }

        _data[_size] = value;
        _size++;
    }

    public void addAll(int[] array)
    {
        ensureCapacity(_size + array.Length);
        Array.Copy(array, 0, _data, _size, array.Length);
        _size += array.Length;
    }

    public void addAll(IntegerList list)
    {
        ensureCapacity(_size + list._size);
        Array.Copy(list._data, 0, _data, _size, list._size);
        _size += list._size;
    }

    public void addAll(ICollection<int> list)
    {
        ensureCapacity(_size + list.Count);
        int current = 0;
        foreach (int x in list)
        {
            _data[_size + current] = x;
            current++;
        }
        _size += list.Count;
    }

    public int get(int index)
    {
        if (index < 0 || index >= _size)
        {
            throw new IndexOutOfRangeException(nameof(index));
        }

        return _data[index];
    }

    public bool contains(int value)
    {
        for (int i = 0; i < _size; i++)
        {
            if (_data[i] == value)
            {
                return true;
            }
        }

        return false;
    }

    public int set(int index, int value)
    {
        if (index < 0 || index >= _size)
        {
            throw new IndexOutOfRangeException(nameof(index));
        }

        int previous = _data[index];
        _data[index] = value;
        return previous;
    }

    public int removeAt(int index)
    {
        int value = get(index);
        Array.Copy(_data, index + 1, _data, index, _size - index - 1);
        _data[_size - 1] = 0;
        _size--;
        return value;
    }

    public void removeRange(int fromIndex, int toIndex)
    {
        if (fromIndex < 0 || toIndex < 0 || fromIndex > _size || toIndex > _size)
        {
            throw new IndexOutOfRangeException();
        }
        if (fromIndex > toIndex)
        {
            throw new ArgumentException();
        }

        Array.Copy(_data, toIndex, _data, fromIndex, _size - toIndex);
        Array.Fill(_data, 0, _size - (toIndex - fromIndex), _size);
        //Arrays.fill(_data, _size - (toIndex - fromIndex), _size, 0);
        _size -= (toIndex - fromIndex);
    }

    public bool isEmpty()
    {
        return _size == 0;
    }

    public int size()
    {
        return _size;
    }

    public void trimToSize()
    {
        if (_data.Length == _size)
        {
            return;
        }

        _data = Arrays.CopyOf(_data, _size);
    }

    public void clear()
    {
        Array.Fill(_data, _size, 0, 0);
        _size = 0;
    }

    public int[] toArray()
    {
        if (_size == 0)
        {
            return EMPTY_DATA;
        }

        return Arrays.CopyOf(_data, _size);
    }

    public void sort()
    {
        Array.Sort(_data, 0, _size);
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
    //@Override
    public override bool Equals(Object? o)
    {
        if (o == this)
        {
            return true;
        }

        if (!(o is IntegerList))
        {
            return false;
        }

        IntegerList other = (IntegerList)o;
        if (_size != other._size)
        {
            return false;
        }

        for (int i = 0; i < _size; i++)
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
    //@Override
    public override int GetHashCode()
    {
        int hashCode = 1;
        for (int i = 0; i < _size; i++)
        {
            hashCode = 31 * hashCode + _data[i];
        }

        return hashCode;
    }

    /**
	 * Returns a string representation of this list.
	 */
    //@Override
    public String toString()
    {
        return string.Join(',', toArray());
    }

    public int binarySearch(int key)
    {
        return Array.BinarySearch(_data, 0, _size, key);
    }

    public int binarySearch(int fromIndex, int toIndex, int key)
    {
        if (fromIndex < 0 || toIndex < 0 || fromIndex > _size || toIndex > _size)
        {
            throw new IndexOutOfRangeException();
        }
        if (fromIndex > toIndex)
        {
            throw new ArgumentException();
        }

        return Array.BinarySearch(_data, fromIndex, toIndex, key);
    }

    private void ensureCapacity(int capacity)
    {
        if (capacity < 0 || capacity > MAX_ARRAY_SIZE)
        {
            throw new OutOfMemoryException();
        }

        int newLength;
        if (_data.Length == 0)
        {
            newLength = INITIAL_SIZE;
        }
        else
        {
            newLength = _data.Length;
        }

        while (newLength < capacity)
        {
            newLength = newLength * 2;
            if (newLength < 0 || newLength > MAX_ARRAY_SIZE)
            {
                newLength = MAX_ARRAY_SIZE;
            }
        }

        _data = Arrays.CopyOf(_data, newLength);
    }
    public const int MIN_SUPPLEMENTARY_CODE_POINT = 0x010000;
    public const int MAX_CODE_POINT = 0X10FFFF;
    public static bool isSupplementaryCodePoint(int codePoint)
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
    public char[] toCharArray()
    {
        // Optimize for the common case (all data values are
        // < 0xFFFF) to avoid an extra scan
        char[] resultArray = new char[_size];
        int resultIdx = 0;
        bool calculatedPreciseResultSize = false;
        for (int i = 0; i < _size; i++)
        {
            int codePoint = _data[i];
            // Calculate the precise result size if we encounter
            // a code point > 0xFFFF
            if (!calculatedPreciseResultSize &&
                isSupplementaryCodePoint(codePoint))
            {
                resultArray = Arrays.CopyOf(resultArray, charArraySize());
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

    private int charArraySize()
    {
        int result = 0;
        for (int i = 0; i < _size; i++)
        {
            result += new Rune(_data[i]).Utf16SequenceLength;
        }
        return result;
    }
}
