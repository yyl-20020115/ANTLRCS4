/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime.dfa;
using org.antlr.v4.runtime.misc;

namespace org.antlr.v4.runtime;


/**
 * Alternative to {@link ANTLRInputStream} which treats the input
 * as a series of Unicode code points, instead of a series of UTF-16
 * code units.
 *
 * Use this if you need to parse input which potentially contains
 * Unicode values > U+FFFF.
 */
public abstract class CodePointCharStream : CharStream {
	protected readonly int _size;
	protected readonly String name;

	// To avoid lots of virtual method calls, we directly access
	// the state of the underlying code points in the
	// CodePointBuffer.
	protected int position;

	// Use the factory method {@link #fromBuffer(CodePointBuffer)} to
	// construct instances of this type.
	private CodePointCharStream(int position, int remaining, String name) {
		// TODO
		//assert position == 0;
		this._size = remaining;
		this.name = name;
		this.position = 0;
	}

	// Visible for testing.
	public abstract Object getInternalStorage();

	/**
	 * Constructs a {@link CodePointCharStream} which provides access
	 * to the Unicode code points stored in {@code codePointBuffer}.
	 */
	public static CodePointCharStream fromBuffer(CodePointBuffer codePointBuffer) {
		return fromBuffer(codePointBuffer, IntStream.UNKNOWN_SOURCE_NAME);
	}

	/**
	 * Constructs a named {@link CodePointCharStream} which provides access
	 * to the Unicode code points stored in {@code codePointBuffer}.
	 */
	public static CodePointCharStream fromBuffer(CodePointBuffer codePointBuffer, String name) {
		// Java lacks generics on primitive types.
		//
		// To avoid lots of calls to virtual methods in the
		// very hot codepath of LA() below, we construct one
		// of three concrete subclasses.
		//
		// The concrete subclasses directly access the code
		// points stored in the underlying array (byte[],
		// char[], or int[]), so we can avoid lots of virtual
		// method calls to ByteBuffer.get(offset).
		switch (codePointBuffer.getType()) {
			case CodePointBuffer.Type.BYTE:
				return new CodePoint8BitCharStream(
						codePointBuffer.position(),
						codePointBuffer.remaining(),
						name,
						codePointBuffer.byteArray(),
						codePointBuffer.arrayOffset());
			case CodePointBuffer.Type.CHAR:
				return new CodePoint16BitCharStream(
						codePointBuffer.position(),
						codePointBuffer.remaining(),
						name,
						codePointBuffer.charArray(),
						codePointBuffer.arrayOffset());
			case CodePointBuffer.Type.INT:
				return new CodePoint32BitCharStream(
						codePointBuffer.position(),
						codePointBuffer.remaining(),
						name,
						codePointBuffer.intArray(),
						codePointBuffer.arrayOffset());
		}
		throw new UnsupportedOperationException("Not reached");
	}

	//@Override
	public void consume() {
		if (_size - position == 0) {
			//assert LA(1) == IntStream.EOF;
			throw new IllegalStateException("cannot consume EOF");
		}
		position = position + 1;
	}

	//@Override
	public int index() {
		return position;
	}

	//@Override
	public int size() {
		return _size;
	}

	/** mark/release do nothing; we have entire buffer */
	//@Override
	public int mark() {
		return -1;
	}

	//@Override
	public void release(int marker) {
	}

	//@Override
	public void seek(int index) {
		position = index;
	}

	//@Override
	public virtual String getSourceName() {
		if (string.IsNullOrEmpty(name)) {
			return IntStream.UNKNOWN_SOURCE_NAME;
		}

		return name;
	}

	//@Override
	public override String ToString() {
		return getText(Interval.of(0, _size - 1));
	}

    public string getText(Interval interval)
    {
        throw new NotImplementedException();
    }

    public int LA(int i)
    {
        throw new NotImplementedException();
    }

    // 8-bit storage for code points <= U+00FF.
    public class CodePoint8BitCharStream : CodePointCharStream {
		private readonly byte[] byteArray;

        public CodePoint8BitCharStream(int position, int remaining, String name, byte[] byteArray, int arrayOffset): base(position, remaining, name)
        {
			;
			// TODO
			//assert arrayOffset == 0;
			this.byteArray = byteArray;
		}

		/** Return the UTF-16 encoded string for the given interval */
		//@Override
		public String getText(Interval interval) {
			int startIdx = Math.Min(interval.a, size());
			int len = Math.Min(interval.b - interval.a + 1, _size - startIdx);

			// We know the maximum code point in byteArray is U+00FF,
			// so we can treat this as if it were ISO-8859-1, aka Latin-1,
			// which shares the same code points up to 0xFF.
			return new String(byteArray, startIdx, len, StandardCharsets.ISO_8859_1);
		}

		//@Override
		public int LA(int i) {
			int offset;
			switch (Math.Sign(i)) {
				case -1:
					offset = position + i;
					if (offset < 0) {
						return IntStream.EOF;
					}
					return byteArray[offset] & 0xFF;
				case 0:
					// Undefined
					return 0;
				case 1:
					offset = position + i - 1;
					if (offset >= _size) {
						return IntStream.EOF;
					}
					return byteArray[offset] & 0xFF;
			}
			throw new UnsupportedOperationException("Not reached");
		}

		//@Override
		 public override Object getInternalStorage() {
			return byteArray;
		}
	}

    // 16-bit internal storage for code points between U+0100 and U+FFFF.
    public class CodePoint16BitCharStream : CodePointCharStream {
		private readonly char[] charArray;

        public CodePoint16BitCharStream(int position, int remaining, String name, char[] charArray, int arrayOffset): base(position, remaining, name)
        {
			;
			this.charArray = charArray;
			// TODO
			//assert arrayOffset == 0;
		}

		/** Return the UTF-16 encoded string for the given interval */
		//@Override
		public String getText(Interval interval) {
			int startIdx = Math.Min(interval.a, size());
			int len = Math.Min(interval.b - interval.a + 1, _size - startIdx);

			// We know there are no surrogates in this
			// array, since otherwise we would be given a
			// 32-bit int[] array.
			//
			// So, it's safe to treat this as if it were
			// UTF-16.
			return new String(charArray, startIdx, len);
		}

		//@Override
		public int LA(int i) {
			int offset;
			switch (Math.Sign(i)) {
				case -1:
					offset = position + i;
					if (offset < 0) {
						return IntStream.EOF;
					}
					return charArray[offset] & 0xFFFF;
				case 0:
					// Undefined
					return 0;
				case 1:
					offset = position + i - 1;
					if (offset >= _size) {
						return IntStream.EOF;
					}
					return charArray[offset] & 0xFFFF;
			}
			throw new UnsupportedOperationException("Not reached");
		}

		//@Override
		public override Object getInternalStorage() {
			return charArray;
		}
	}

    // 32-bit internal storage for code points between U+10000 and U+10FFFF.
    public class CodePoint32BitCharStream : CodePointCharStream {
		private readonly int[] intArray;

        public CodePoint32BitCharStream(int position, int remaining, String name, int[] intArray, int arrayOffset): base(position, remaining, name)
        {
			;
			this.intArray = intArray;
			// TODO
			//assert arrayOffset == 0;
		}

		/** Return the UTF-16 encoded string for the given interval */
		//@Override
		public String getText(Interval interval) {
			int startIdx = Math.Min(interval.a, _size);
			int len = Math.Min(interval.b - interval.a + 1, _size - startIdx);

			// Note that we pass the int[] code points to the String constructor --
			// this is supported, and the constructor will convert to UTF-16 internally.
			return new String(intArray, startIdx, len);
		}

		//@Override
		public int LA(int i) {
			int offset;
			switch (Math.Sign(i)) {
				case -1:
					offset = position + i;
					if (offset < 0) {
						return IntStream.EOF;
					}
					return intArray[offset];
				case 0:
					// Undefined
					return 0;
				case 1:
					offset = position + i - 1;
					if (offset >= _size) {
						return IntStream.EOF;
					}
					return intArray[offset];
			}
			throw new UnsupportedOperationException("Not reached");
		}

        //@Override
        public override Object getInternalStorage() {
			return intArray;
		}
	}
}
