/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

namespace org.antlr.v4.runtime.tree.pattern;

/**
 * Represents a span of raw text (concrete syntax) between tags in a tree
 * pattern string.
 */
class TextChunk : Chunk
{
    /**
	 * This is the backing field for {@link #getText}.
	 */

    private readonly string text;

    /**
	 * Constructs a new instance of {@link TextChunk} with the specified text.
	 *
	 * @param text The text of this chunk.
	 * @exception IllegalArgumentException if {@code text} is {@code null}.
	 */
    public TextChunk(string text)
    {
        this.text = text ?? throw new ArgumentException("text cannot be null", nameof(text));
    }

    /**
	 * Gets the raw text of this chunk.
	 *
	 * @return The text of the chunk.
	 */

    public string Text => text;

    /**
	 * {@inheritDoc}
	 *
	 * <p>The implementation for {@link TextChunk} returns the result of
	 * {@link #getText()} in single quotes.</p>
	 */
    //@Override
    public override string ToString() => "'" + text + "'";
}
