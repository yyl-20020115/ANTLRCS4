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
public static class MurmurHash
{

	private static readonly int DEFAULT_SEED = 0;

	/**
	 * Initialize the hash using the default seed value.
	 *
	 * @return the intermediate hash value
	 */
	public static int initialize() {
		return initialize(DEFAULT_SEED);
	}

	/**
	 * Initialize the hash using the specified {@code seed}.
	 *
	 * @param seed the seed
	 * @return the intermediate hash value
	 */
	public static int initialize(int seed) {
		return seed;
	}

	/**
	 * Update the intermediate hash value for the next input {@code value}.
	 *
	 * @param hash the intermediate hash value
	 * @param value the value to add to the current hash
	 * @return the updated intermediate hash value
	 */
	public static int update(int hash, int value) {
		int c1 = unchecked((int)0xCC9E2D51);
		int c2 = unchecked((int)0x1B873593);
		int r1 = unchecked((int)15);
		int r2 = unchecked((int)13);
		int m = unchecked((int)5);
		int n = unchecked((int)0xE6546B64);

		int k = value;
		k = k * c1;
		k = (k << r1) | (k >>> (32 - r1));
		k = k * c2;

		hash = hash ^ k;
		hash = (hash << r2) | (hash >>> (32 - r2));
		hash = hash * m + n;

		return hash;
	}

	/**
	 * Update the intermediate hash value for the next input {@code value}.
	 *
	 * @param hash the intermediate hash value
	 * @param value the value to add to the current hash
	 * @return the updated intermediate hash value
	 */
	public static int update(int hash, Object value) {
		return update(hash, value != null ? value.GetHashCode() : 0);
	}

	/**
	 * Apply the final computation steps to the intermediate value {@code hash}
	 * to form the final result of the MurmurHash 3 hash function.
	 *
	 * @param hash the intermediate hash value
	 * @param numberOfWords the number of integer values added to the hash
	 * @return the final hash result
	 */
	public static int finish(int hash, int numberOfWords) {
		hash ^= (numberOfWords * 4);
		hash ^= (hash >>> 16);
		hash *= unchecked((int)0x85EBCA6B);
		hash ^= (hash >>> 13);
		hash *= unchecked((int)0xC2B2AE35);
		hash ^= (hash >>> 16);
		return hash;
	}

	/**
	 * Utility function to compute the hash code of an array using the
	 * MurmurHash algorithm.
	 *
	 * @param <T> the array element type
	 * @param data the array data
	 * @param seed the seed for the MurmurHash algorithm
	 * @return the hash code of the data
	 */
	public static int GetHashCode<T>(T[] data, int seed) {
		int hash = initialize(seed);
		foreach (T value in data) {
			hash = update(hash, value);
		}

		hash = finish(hash, data.Length);
		return hash;
	}

}
