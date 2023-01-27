/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using System.Text;

namespace org.antlr.v4.runtime.atn;

public class BitSet
{
    private static readonly int ADDRESS_BITS_PER_WORD = 6;
    private static readonly int BITS_PER_WORD = 1 << ADDRESS_BITS_PER_WORD;
    private static readonly int BIT_INDEX_MASK = BITS_PER_WORD - 1;

    /* Used to shift left or right for a partial word mask */
    private static readonly long WORD_MASK = ~0L;


    /**
     * The internal field corresponding to the serialField "bits".
     */
    private long[] words;

    /**
     * The number of words in the logical size of this BitSet.
     */
    private volatile int wordsInUse = 0;

    /**
     * Whether the size of "words" is user-specified.  If so, we assume
     * the user knows what he's doing and try harder to preserve it.
     */
    private volatile bool sizeIsSticky = false;

    /**
     * Given a bit index, return word index containing it.
     */
    private static int wordIndex(int bitIndex)
    {
        return bitIndex >> ADDRESS_BITS_PER_WORD;
    }

    /**
     * Every public method must preserve these invariants.
     */
    private void checkInvariants()
    {
        System.Diagnostics.Debug.Assert(wordsInUse == 0 || words[wordsInUse - 1] != 0);
        System.Diagnostics.Debug.Assert(wordsInUse >= 0 && wordsInUse <= words.Length);
        System.Diagnostics.Debug.Assert(wordsInUse == words.Length || words[wordsInUse] == 0);
    }

    /**
     * Sets the field wordsInUse to the logical size in words of the bit set.
     * WARNING:This method assumes that the number of words actually in use is
     * less than or equal to the current value of wordsInUse!
     */
    private void recalculateWordsInUse()
    {
        // Traverse the bitset until a used word is found
        int i;
        for (i = wordsInUse - 1; i >= 0; i--)
            if (words[i] != 0)
                break;

        wordsInUse = i + 1; // The new logical size
    }

    /**
     * Creates a new bit set. All bits are initially {@code false}.
     */
    public BitSet()
    {
        initWords(BITS_PER_WORD);
        sizeIsSticky = false;
    }

    /**
     * Creates a bit set whose initial size is large enough to explicitly
     * represent bits with indices in the range {@code 0} through
     * {@code nbits-1}. All bits are initially {@code false}.
     *
     * @param  nbits the initial size of the bit set
     * @throws NegativeArraySizeException if the specified initial size
     *         is negative
     */
    public BitSet(int nbits)
    {
        // nbits can't be negative; size 0 is OK
        if (nbits < 0)
            throw new InvalidOperationException("nbits < 0: " + nbits);

        initWords(nbits);
        sizeIsSticky = true;
    }
    public BitSet(BitSet other)
    {
        this.words =(long[]) other.words.Clone();
        this.wordsInUse = other.wordsInUse;
        this.sizeIsSticky = other.sizeIsSticky;
    }
    private void initWords(int nbits)
    {
        words = new long[wordIndex(nbits - 1) + 1];
    }

    /**
     * Creates a bit set using words as the internal representation.
     * The last word (if there is one) must be non-zero.
     */
    private BitSet(long[] words)
    {
        this.words = words;
        this.wordsInUse = words.Length;
        checkInvariants();
    }

    /**
     * Returns a new byte array containing all the bits in this bit set.
     *
     * <p>More precisely, if
     * <br>{@code byte[] bytes = s.toByteArray();}
     * <br>then {@code bytes.length == (s.length()+7)/8} and
     * <br>{@code s.get(n) == ((bytes[n/8] & (1<<(n%8))) != 0)}
     * <br>for all {@code n < 8 * bytes.length}.
     *
     * @return a byte array containing a little-endian representation
     *         of all the bits in this bit set
     * @since 1.7
     */
    public byte[] ToByteArray()
    {
        int n = wordsInUse;
        if (n == 0)
            return Array.Empty<byte>();
        int len = 8 * (n - 1);
        for (long x = words[n - 1]; x != 0; x >>>= 8)
            len++;

        var buffer = new byte[len];
        var ms = new MemoryStream(buffer);

        for (int i = 0; i < n - 1; i++)
            ms.Write(BitConverter.GetBytes(words[i]));
        for (long x = words[n - 1]; x != 0; x >>>= 8)
            ms.WriteByte((byte)(x & 0xff));

        return buffer;
    }

    /**
     * Returns a new long array containing all the bits in this bit set.
     *
     * <p>More precisely, if
     * <br>{@code long[] longs = s.toLongArray();}
     * <br>then {@code longs.length == (s.length()+63)/64} and
     * <br>{@code s.get(n) == ((longs[n/64] & (1L<<(n%64))) != 0)}
     * <br>for all {@code n < 64 * longs.length}.
     *
     * @return a long array containing a little-endian representation
     *         of all the bits in this bit set
     * @since 1.7
     */
    public long[] ToLongArray()
    {
        var newwords = new long[wordsInUse];
        Array.Copy(this.words,newwords, wordsInUse);
        return newwords;
    }

    /**
     * Ensures that the BitSet can hold enough words.
     * @param wordsRequired the minimum acceptable number of words.
     */
    private void ensureCapacity(int wordsRequired)
    {
        if (words.Length < wordsRequired)
        {
            // Allocate larger of doubled size or required size
            int request = Math.Max(2 * words.Length, wordsRequired);
            Array.Resize(ref words, request);
            //words = Arrays.copyOf(words, request);
            sizeIsSticky = false;
        }
    }

    /**
     * Ensures that the BitSet can accommodate a given wordIndex,
     * temporarily violating the invariants.  The caller must
     * restore the invariants before returning to the user,
     * possibly using recalculateWordsInUse().
     * @param wordIndex the index to be accommodated.
     */
    private void expandTo(int wordIndex)
    {
        int wordsRequired = wordIndex + 1;
        if (wordsInUse < wordsRequired)
        {
            ensureCapacity(wordsRequired);
            wordsInUse = wordsRequired;
        }
    }

    /**
     * Checks that fromIndex ... toIndex is a valid range of bit indices.
     */
    private static void checkRange(int fromIndex, int toIndex)
    {
        if (fromIndex < 0)
            throw new IndexOutOfRangeException("fromIndex < 0: " + fromIndex);
        if (toIndex < 0)
            throw new IndexOutOfRangeException("toIndex < 0: " + toIndex);
        if (fromIndex > toIndex)
            throw new IndexOutOfRangeException("fromIndex: " + fromIndex +
                                                " > toIndex: " + toIndex);
    }

    /**
     * Sets the bit at the specified index to the complement of its
     * current value.
     *
     * @param  bitIndex the index of the bit to flip
     * @throws IndexOutOfBoundsException if the specified index is negative
     * @since  1.4
     */
    public void flip(int bitIndex)
    {
        if (bitIndex < 0)
            throw new IndexOutOfRangeException("bitIndex < 0: " + bitIndex);

        int _wordIndex = wordIndex(bitIndex);
        expandTo(_wordIndex);

        words[_wordIndex] ^= (1L << bitIndex);

        recalculateWordsInUse();
        checkInvariants();
    }

    /**
     * Sets each bit from the specified {@code fromIndex} (inclusive) to the
     * specified {@code toIndex} (exclusive) to the complement of its current
     * value.
     *
     * @param  fromIndex index of the first bit to flip
     * @param  toIndex index after the last bit to flip
     * @throws IndexOutOfBoundsException if {@code fromIndex} is negative,
     *         or {@code toIndex} is negative, or {@code fromIndex} is
     *         larger than {@code toIndex}
     * @since  1.4
     */
    public void flip(int fromIndex, int toIndex)
    {
        checkRange(fromIndex, toIndex);

        if (fromIndex == toIndex)
            return;

        int startWordIndex = wordIndex(fromIndex);
        int endWordIndex = wordIndex(toIndex - 1);
        expandTo(endWordIndex);

        long firstWordMask = WORD_MASK << fromIndex;
        long lastWordMask = WORD_MASK >>> -toIndex;
        if (startWordIndex == endWordIndex)
        {
            // Case 1: One word
            words[startWordIndex] ^= (firstWordMask & lastWordMask);
        }
        else
        {
            // Case 2: Multiple words
            // Handle first word
            words[startWordIndex] ^= firstWordMask;

            // Handle intermediate words, if any
            for (int i = startWordIndex + 1; i < endWordIndex; i++)
                words[i] ^= WORD_MASK;

            // Handle last word
            words[endWordIndex] ^= lastWordMask;
        }

        recalculateWordsInUse();
        checkInvariants();
    }

    /**
     * Sets the bit at the specified index to {@code true}.
     *
     * @param  bitIndex a bit index
     * @throws IndexOutOfBoundsException if the specified index is negative
     * @since  1.0
     */
    public void set(int bitIndex)
    {
        if (bitIndex < 0)
            throw new IndexOutOfRangeException("bitIndex < 0: " + bitIndex);

        int _wordIndex = wordIndex(bitIndex);
        expandTo(_wordIndex);

        words[_wordIndex] |= (1L << bitIndex); // Restores invariants

        checkInvariants();
    }

    /**
     * Sets the bit at the specified index to the specified value.
     *
     * @param  bitIndex a bit index
     * @param  value a boolean value to set
     * @throws IndexOutOfBoundsException if the specified index is negative
     * @since  1.4
     */
    public void set(int bitIndex, bool value)
    {
        if (value)
            set(bitIndex);
        else
            clear(bitIndex);
    }

    /**
     * Sets the bits from the specified {@code fromIndex} (inclusive) to the
     * specified {@code toIndex} (exclusive) to {@code true}.
     *
     * @param  fromIndex index of the first bit to be set
     * @param  toIndex index after the last bit to be set
     * @throws IndexOutOfBoundsException if {@code fromIndex} is negative,
     *         or {@code toIndex} is negative, or {@code fromIndex} is
     *         larger than {@code toIndex}
     * @since  1.4
     */
    public void set(int fromIndex, int toIndex)
    {
        checkRange(fromIndex, toIndex);

        if (fromIndex == toIndex)
            return;

        // Increase capacity if necessary
        int startWordIndex = wordIndex(fromIndex);
        int endWordIndex = wordIndex(toIndex - 1);
        expandTo(endWordIndex);

        long firstWordMask = WORD_MASK << fromIndex;
        long lastWordMask = WORD_MASK >>> -toIndex;
        if (startWordIndex == endWordIndex)
        {
            // Case 1: One word
            words[startWordIndex] |= (firstWordMask & lastWordMask);
        }
        else
        {
            // Case 2: Multiple words
            // Handle first word
            words[startWordIndex] |= firstWordMask;

            // Handle intermediate words, if any
            for (int i = startWordIndex + 1; i < endWordIndex; i++)
                words[i] = WORD_MASK;

            // Handle last word (restores invariants)
            words[endWordIndex] |= lastWordMask;
        }

        checkInvariants();
    }

    /**
     * Sets the bits from the specified {@code fromIndex} (inclusive) to the
     * specified {@code toIndex} (exclusive) to the specified value.
     *
     * @param  fromIndex index of the first bit to be set
     * @param  toIndex index after the last bit to be set
     * @param  value value to set the selected bits to
     * @throws IndexOutOfBoundsException if {@code fromIndex} is negative,
     *         or {@code toIndex} is negative, or {@code fromIndex} is
     *         larger than {@code toIndex}
     * @since  1.4
     */
    public void set(int fromIndex, int toIndex, bool value)
    {
        if (value)
            set(fromIndex, toIndex);
        else
            clear(fromIndex, toIndex);
    }

    /**
     * Sets the bit specified by the index to {@code false}.
     *
     * @param  bitIndex the index of the bit to be cleared
     * @throws IndexOutOfBoundsException if the specified index is negative
     * @since  1.0
     */
    public void clear(int bitIndex)
    {
        if (bitIndex < 0)
            throw new IndexOutOfRangeException("bitIndex < 0: " + bitIndex);

        int _wordIndex = wordIndex(bitIndex);
        if (_wordIndex >= wordsInUse)
            return;

        words[_wordIndex] &= ~(1L << bitIndex);

        recalculateWordsInUse();
        checkInvariants();
    }

    /**
     * Sets the bits from the specified {@code fromIndex} (inclusive) to the
     * specified {@code toIndex} (exclusive) to {@code false}.
     *
     * @param  fromIndex index of the first bit to be cleared
     * @param  toIndex index after the last bit to be cleared
     * @throws IndexOutOfBoundsException if {@code fromIndex} is negative,
     *         or {@code toIndex} is negative, or {@code fromIndex} is
     *         larger than {@code toIndex}
     * @since  1.4
     */
    public void clear(int fromIndex, int toIndex)
    {
        checkRange(fromIndex, toIndex);

        if (fromIndex == toIndex)
            return;

        int startWordIndex = wordIndex(fromIndex);
        if (startWordIndex >= wordsInUse)
            return;

        int endWordIndex = wordIndex(toIndex - 1);
        if (endWordIndex >= wordsInUse)
        {
            toIndex = length();
            endWordIndex = wordsInUse - 1;
        }

        long firstWordMask = WORD_MASK << fromIndex;
        long lastWordMask = WORD_MASK >>> -toIndex;
        if (startWordIndex == endWordIndex)
        {
            // Case 1: One word
            words[startWordIndex] &= ~(firstWordMask & lastWordMask);
        }
        else
        {
            // Case 2: Multiple words
            // Handle first word
            words[startWordIndex] &= ~firstWordMask;

            // Handle intermediate words, if any
            for (int i = startWordIndex + 1; i < endWordIndex; i++)
                words[i] = 0;

            // Handle last word
            words[endWordIndex] &= ~lastWordMask;
        }

        recalculateWordsInUse();
        checkInvariants();
    }

    /**
     * Sets all of the bits in this BitSet to {@code false}.
     *
     * @since 1.4
     */
    public void clear()
    {
        while (wordsInUse > 0)
            words[--wordsInUse] = 0;
    }

    /**
     * Returns the value of the bit with the specified index. The value
     * is {@code true} if the bit with the index {@code bitIndex}
     * is currently set in this {@code BitSet}; otherwise, the result
     * is {@code false}.
     *
     * @param  bitIndex   the bit index
     * @return the value of the bit with the specified index
     * @throws IndexOutOfBoundsException if the specified index is negative
     */
    public bool get(int bitIndex)
    {
        if (bitIndex < 0)
            throw new IndexOutOfRangeException("bitIndex < 0: " + bitIndex);

        checkInvariants();

        int _wordIndex = wordIndex(bitIndex);
        return (_wordIndex < wordsInUse)
            && ((words[_wordIndex] & (1L << bitIndex)) != 0);
    }

    /**
     * Returns a new {@code BitSet} composed of bits from this {@code BitSet}
     * from {@code fromIndex} (inclusive) to {@code toIndex} (exclusive).
     *
     * @param  fromIndex index of the first bit to include
     * @param  toIndex index after the last bit to include
     * @return a new {@code BitSet} from a range of this {@code BitSet}
     * @throws IndexOutOfBoundsException if {@code fromIndex} is negative,
     *         or {@code toIndex} is negative, or {@code fromIndex} is
     *         larger than {@code toIndex}
     * @since  1.4
     */
    public BitSet get(int fromIndex, int toIndex)
    {
        checkRange(fromIndex, toIndex);

        checkInvariants();

        int len = length();

        // If no set bits in range return empty bitset
        if (len <= fromIndex || fromIndex == toIndex)
            return new BitSet(0);

        // An optimization
        if (toIndex > len)
            toIndex = len;

        BitSet result = new BitSet(toIndex - fromIndex);
        int targetWords = wordIndex(toIndex - fromIndex - 1) + 1;
        int sourceIndex = wordIndex(fromIndex);
        bool wordAligned = ((fromIndex & BIT_INDEX_MASK) == 0);

        // Process all words but the last word
        for (int i = 0; i < targetWords - 1; i++, sourceIndex++)
            result.words[i] = wordAligned ? words[sourceIndex] :
                (words[sourceIndex] >>> fromIndex) |
                (words[sourceIndex + 1] << -fromIndex);

        // Process the last word
        long lastWordMask = WORD_MASK >>> -toIndex;
        result.words[targetWords - 1] =
            ((toIndex - 1) & BIT_INDEX_MASK) < (fromIndex & BIT_INDEX_MASK)
            ? /* straddles source words */
            ((words[sourceIndex] >>> fromIndex) |
             (words[sourceIndex + 1] & lastWordMask) << -fromIndex)
            :
            ((words[sourceIndex] & lastWordMask) >>> fromIndex);

        // Set wordsInUse correctly
        result.wordsInUse = targetWords;
        result.recalculateWordsInUse();
        result.checkInvariants();

        return result;
    }

    /**
     * Returns the index of the first bit that is set to {@code true}
     * that occurs on or after the specified starting index. If no such
     * bit exists then {@code -1} is returned.
     *
     * <p>To iterate over the {@code true} bits in a {@code BitSet},
     * use the following loop:
     *
     *  <pre> {@code
     * for (int i = bs.nextSetBit(0); i >= 0; i = bs.nextSetBit(i+1)) {
     *     // operate on index i here
     *     if (i == Integer.MAX_VALUE) {
     *         break; // or (i+1) would overflow
     *     }
     * }}</pre>
     *
     * @param  fromIndex the index to start checking from (inclusive)
     * @return the index of the next set bit, or {@code -1} if there
     *         is no such bit
     * @throws IndexOutOfBoundsException if the specified index is negative
     * @since  1.4
     */
    public int nextSetBit(int fromIndex)
    {
        if (fromIndex < 0)
            throw new IndexOutOfRangeException("fromIndex < 0: " + fromIndex);

        checkInvariants();

        int u = wordIndex(fromIndex);
        if (u >= wordsInUse)
            return -1;

        long word = words[u] & (WORD_MASK << fromIndex);

        while (true)
        {
            if (word != 0)
                return (u * BITS_PER_WORD) + Integer.numberOfTrailingZeros(word);
            if (++u == wordsInUse)
                return -1;
            word = words[u];
        }
    }

    /**
     * Returns the index of the first bit that is set to {@code false}
     * that occurs on or after the specified starting index.
     *
     * @param  fromIndex the index to start checking from (inclusive)
     * @return the index of the next clear bit
     * @throws IndexOutOfBoundsException if the specified index is negative
     * @since  1.4
     */
    public int nextClearBit(int fromIndex)
    {
        // Neither spec nor implementation handle bitsets of maximal length.
        // See 4816253.
        if (fromIndex < 0)
            throw new IndexOutOfRangeException("fromIndex < 0: " + fromIndex);

        checkInvariants();

        int u = wordIndex(fromIndex);
        if (u >= wordsInUse)
            return fromIndex;

        long word = ~words[u] & (WORD_MASK << fromIndex);

        while (true)
        {
            if (word != 0)
                return (u * BITS_PER_WORD) + Integer.numberOfTrailingZeros(word);
            if (++u == wordsInUse)
                return wordsInUse * BITS_PER_WORD;
            word = ~words[u];
        }
    }

    /**
     * Returns the index of the nearest bit that is set to {@code true}
     * that occurs on or before the specified starting index.
     * If no such bit exists, or if {@code -1} is given as the
     * starting index, then {@code -1} is returned.
     *
     * <p>To iterate over the {@code true} bits in a {@code BitSet},
     * use the following loop:
     *
     *  <pre> {@code
     * for (int i = bs.length(); (i = bs.previousSetBit(i-1)) >= 0; ) {
     *     // operate on index i here
     * }}</pre>
     *
     * @param  fromIndex the index to start checking from (inclusive)
     * @return the index of the previous set bit, or {@code -1} if there
     *         is no such bit
     * @throws IndexOutOfBoundsException if the specified index is less
     *         than {@code -1}
     * @since  1.7
     */
    public int previousSetBit(int fromIndex)
    {
        if (fromIndex < 0)
        {
            if (fromIndex == -1)
                return -1;
            throw new IndexOutOfRangeException(
                "fromIndex < -1: " + fromIndex);
        }

        checkInvariants();

        int u = wordIndex(fromIndex);
        if (u >= wordsInUse)
            return length() - 1;

        long word = words[u] & (WORD_MASK >>> -(fromIndex + 1));

        while (true)
        {
            if (word != 0)
                return (u + 1) * BITS_PER_WORD - 1 - Integer.numberOfLeadingZeros(word);
            if (u-- == 0)
                return -1;
            word = words[u];
        }
    }

    /**
     * Returns the index of the nearest bit that is set to {@code false}
     * that occurs on or before the specified starting index.
     * If no such bit exists, or if {@code -1} is given as the
     * starting index, then {@code -1} is returned.
     *
     * @param  fromIndex the index to start checking from (inclusive)
     * @return the index of the previous clear bit, or {@code -1} if there
     *         is no such bit
     * @throws IndexOutOfBoundsException if the specified index is less
     *         than {@code -1}
     * @since  1.7
     */
    public int previousClearBit(int fromIndex)
    {
        if (fromIndex < 0)
        {
            if (fromIndex == -1)
                return -1;
            throw new IndexOutOfRangeException(
                "fromIndex < -1: " + fromIndex);
        }

        checkInvariants();

        int u = wordIndex(fromIndex);
        if (u >= wordsInUse)
            return fromIndex;

        long word = ~words[u] & (WORD_MASK >>> -(fromIndex + 1));

        while (true)
        {
            if (word != 0)
                return (u + 1) * BITS_PER_WORD - 1 - Integer.numberOfLeadingZeros(word);
            if (u-- == 0)
                return -1;
            word = ~words[u];
        }
    }

    /**
     * Returns the "logical size" of this {@code BitSet}: the index of
     * the highest set bit in the {@code BitSet} plus one. Returns zero
     * if the {@code BitSet} contains no set bits.
     *
     * @return the logical size of this {@code BitSet}
     * @since  1.2
     */
    public int length()
    {
        if (wordsInUse == 0)
            return 0;

        return BITS_PER_WORD * (wordsInUse - 1) +
            (BITS_PER_WORD - Integer.numberOfLeadingZeros(words[wordsInUse - 1]));
    }

    /**
     * Returns true if this {@code BitSet} contains no bits that are set
     * to {@code true}.
     *
     * @return boolean indicating whether this {@code BitSet} is empty
     * @since  1.4
     */
    public bool isEmpty()
    {
        return wordsInUse == 0;
    }

    /**
     * Returns true if the specified {@code BitSet} has any bits set to
     * {@code true} that are also set to {@code true} in this {@code BitSet}.
     *
     * @param  set {@code BitSet} to intersect with
     * @return boolean indicating whether this {@code BitSet} intersects
     *         the specified {@code BitSet}
     * @since  1.4
     */
    public bool intersects(BitSet set)
    {
        for (int i = Math.Min(wordsInUse, set.wordsInUse) - 1; i >= 0; i--)
            if ((words[i] & set.words[i]) != 0)
                return true;
        return false;
    }

    /**
     * Returns the number of bits set to {@code true} in this {@code BitSet}.
     *
     * @return the number of bits set to {@code true} in this {@code BitSet}
     * @since  1.4
     */
    public int cardinality()
    {
        int sum = 0;
        for (int i = 0; i < wordsInUse; i++)
            sum += Integer.bitCount(words[i]);
        return sum;
    }

    /**
     * Performs a logical <b>AND</b> of this target bit set with the
     * argument bit set. This bit set is modified so that each bit in it
     * has the value {@code true} if and only if it both initially
     * had the value {@code true} and the corresponding bit in the
     * bit set argument also had the value {@code true}.
     *
     * @param set a bit set
     */
    public void and(BitSet set)
    {
        if (this == set)
            return;

        while (wordsInUse > set.wordsInUse)
            words[--wordsInUse] = 0;

        // Perform logical AND on words in common
        for (int i = 0; i < wordsInUse; i++)
            words[i] &= set.words[i];

        recalculateWordsInUse();
        checkInvariants();
    }

    /**
     * Performs a logical <b>OR</b> of this bit set with the bit set
     * argument. This bit set is modified so that a bit in it has the
     * value {@code true} if and only if it either already had the
     * value {@code true} or the corresponding bit in the bit set
     * argument has the value {@code true}.
     *
     * @param set a bit set
     */
    public void or(BitSet set)
    {
        if (this == set)
            return;

        int wordsInCommon = Math.Min(wordsInUse, set.wordsInUse);

        if (wordsInUse < set.wordsInUse)
        {
            ensureCapacity(set.wordsInUse);
            wordsInUse = set.wordsInUse;
        }

        // Perform logical OR on words in common
        for (int i = 0; i < wordsInCommon; i++)
            words[i] |= set.words[i];

        // Copy any remaining words
        if (wordsInCommon < set.wordsInUse)
        {
            Array.Copy(set.words, wordsInCommon,
                             words, wordsInCommon,
                             wordsInUse - wordsInCommon);
        }
        // recalculateWordsInUse() is unnecessary
        checkInvariants();
    }

    /**
     * Performs a logical <b>XOR</b> of this bit set with the bit set
     * argument. This bit set is modified so that a bit in it has the
     * value {@code true} if and only if one of the following
     * statements holds:
     * <ul>
     * <li>The bit initially has the value {@code true}, and the
     *     corresponding bit in the argument has the value {@code false}.
     * <li>The bit initially has the value {@code false}, and the
     *     corresponding bit in the argument has the value {@code true}.
     * </ul>
     *
     * @param  set a bit set
     */
    public void xor(BitSet set)
    {
        int wordsInCommon = Math.Min(wordsInUse, set.wordsInUse);

        if (wordsInUse < set.wordsInUse)
        {
            ensureCapacity(set.wordsInUse);
            wordsInUse = set.wordsInUse;
        }

        // Perform logical XOR on words in common
        for (int i = 0; i < wordsInCommon; i++)
            words[i] ^= set.words[i];

        // Copy any remaining words
        if (wordsInCommon < set.wordsInUse)
        {
            Array.Copy(set.words, wordsInCommon,
                             words, wordsInCommon,
                             set.wordsInUse - wordsInCommon);
        }
        recalculateWordsInUse();
        checkInvariants();
    }

    /**
     * Clears all of the bits in this {@code BitSet} whose corresponding
     * bit is set in the specified {@code BitSet}.
     *
     * @param  set the {@code BitSet} with which to mask this
     *         {@code BitSet}
     * @since  1.2
     */
    public void andNot(BitSet set)
    {
        // Perform logical (a & !b) on words in common
        for (int i = Math.Min(wordsInUse, set.wordsInUse) - 1; i >= 0; i--)
            words[i] &= ~set.words[i];

        recalculateWordsInUse();
        checkInvariants();
    }

    /**
     * Returns the hash code value for this bit set. The hash code depends
     * only on which bits are set within this {@code BitSet}.
     *
     * <p>The hash code is defined to be the result of the following
     * calculation:
     *  <pre> {@code
     * public int hashCode() {
     *     long h = 1234;
     *     long[] words = toLongArray();
     *     for (int i = words.length; --i >= 0; )
     *         h ^= words[i] * (i + 1);
     *     return (int)((h >> 32) ^ h);
     * }}</pre>
     * Note that the hash code changes if the set of bits is altered.
     *
     * @return the hash code value for this bit set
     */
    public override int GetHashCode()
    {
        long h = 1234;
        for (int i = wordsInUse; --i >= 0;)
            h ^= words[i] * (i + 1);

        return (int)((h >> 32) ^ h);
    }

    /**
     * Returns the number of bits of space actually in use by this
     * {@code BitSet} to represent bit values.
     * The maximum element in the set is the size - 1st element.
     *
     * @return the number of bits currently in this bit set
     */
    public int size()
    {
        return words.Length * BITS_PER_WORD;
    }

    /**
     * Compares this object against the specified object.
     * The result is {@code true} if and only if the argument is
     * not {@code null} and is a {@code BitSet} object that has
     * exactly the same set of bits set to {@code true} as this bit
     * set. That is, for every nonnegative {@code int} index {@code k},
     * <pre>((BitSet)obj).get(k) == this.get(k)</pre>
     * must be true. The current sizes of the two bit sets are not compared.
     *
     * @param  obj the object to compare with
     * @return {@code true} if the objects are the same;
     *         {@code false} otherwise
     * @see    #size()
     */
    public override bool Equals(Object? obj)
    {
        if (!(obj is BitSet set))
            return false;
        if (this == obj)
            return true;

        checkInvariants();
        set.checkInvariants();

        if (wordsInUse != set.wordsInUse)
            return false;

        // Check words in use by both BitSets
        for (int i = 0; i < wordsInUse; i++)
            if (words[i] != set.words[i])
                return false;

        return true;
    }

    /**
     * Cloning this {@code BitSet} produces a new {@code BitSet}
     * that is equal to it.
     * The clone of the bit set is another bit set that has exactly the
     * same bits set to {@code true} as this bit set.
     *
     * @return a clone of this bit set
     * @see    #size()
     */
    public Object clone()
    {
        if (!sizeIsSticky)
            trimToSize();

        try
        {
            return new BitSet(this);
        }
        catch (InvalidOperationException e)
        {
            throw new InternalError(e);
        }
    }

    /**
     * Attempts to reduce internal storage used for the bits in this bit set.
     * Calling this method may, but is not required to, affect the value
     * returned by a subsequent call to the {@link #size()} method.
     */
    private void trimToSize()
    {
        if (wordsInUse != words.Length)
        {
            //words = Arrays.copyOf(words, wordsInUse);
            var newwords= new long[wordsInUse];
            Array.Copy(this.words,newwords, wordsInUse);
            this.words=newwords;
            checkInvariants();
        }
    }


    /**
     * Reconstitute the {@code BitSet} instance from a stream (i.e.,
     * deserialize it).
     */
    /**
     * Returns a string representation of this bit set. For every index
     * for which this {@code BitSet} contains a bit in the set
     * state, the decimal representation of that index is included in
     * the result. Such indices are listed in order from lowest to
     * highest, separated by ",&nbsp;" (a comma and a space) and
     * surrounded by braces, resulting in the usual mathematical
     * notation for a set of integers.
     *
     * <p>Example:
     * <pre>
     * BitSet drPepper = new BitSet();</pre>
     * Now {@code drPepper.toString()} returns "{@code {}}".
     * <pre>
     * drPepper.set(2);</pre>
     * Now {@code drPepper.toString()} returns "{@code {2}}".
     * <pre>
     * drPepper.set(4);
     * drPepper.set(10);</pre>
     * Now {@code drPepper.toString()} returns "{@code {2, 4, 10}}".
     *
     * @return a string representation of this bit set
     */
    public override String ToString()
    {
        checkInvariants();

        int MAX_INITIAL_CAPACITY = int.MaxValue - 8;
        int numBits = (wordsInUse > 128) ?
            cardinality() : wordsInUse * BITS_PER_WORD;
        // Avoid overflow in the case of a humongous numBits
        int initialCapacity = (numBits <= (MAX_INITIAL_CAPACITY - 2) / 6) ?
            6 * numBits + 2 : MAX_INITIAL_CAPACITY;
        StringBuilder b = new StringBuilder(initialCapacity);
        b.Append('{');

        int i = nextSetBit(0);
        if (i != -1)
        {
            b.Append(i);
            while (true)
            {
                if (++i < 0) break;
                if ((i = nextSetBit(i)) < 0) break;
                int endOfRun = nextClearBit(i);
                do { b.Append(", ").Append(i); }
                while (++i != endOfRun);
            }
        }

        b.Append('}');
        return b.ToString();
    }

    /**
     * Returns the index of the first bit that is set to {@code true}
     * that occurs on or after the specified starting index and up to and
     * including the specified word index
     * If no such bit exists then {@code -1} is returned.
     *
     * @param  fromIndex the index to start checking from (inclusive)
     * @param  toWordIndex the last word index to check (inclusive)
     * @return the index of the next set bit, or {@code -1} if there
     *         is no such bit
     */
    private int nextSetBit(int fromIndex, int toWordIndex)
    {
        int u = wordIndex(fromIndex);
        // Check if out of bounds
        if (u > toWordIndex)
            return -1;

        long word = words[u] & (WORD_MASK << fromIndex);

        while (true)
        {
            if (word != 0)
                return (u * BITS_PER_WORD) + Integer.numberOfTrailingZeros(word);
            // Check if out of bounds
            if (++u > toWordIndex)
                return -1;
            word = words[u];
        }
    }
}