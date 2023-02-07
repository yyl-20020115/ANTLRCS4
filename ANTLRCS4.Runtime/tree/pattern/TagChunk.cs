/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

namespace org.antlr.v4.runtime.tree.pattern;

/**
 * Represents a placeholder tag in a tree pattern. A tag can have any of the
 * following forms.
 *
 * <ul>
 * <li>{@code expr}: An unlabeled placeholder for a parser rule {@code expr}.</li>
 * <li>{@code ID}: An unlabeled placeholder for a token of type {@code ID}.</li>
 * <li>{@code e:expr}: A labeled placeholder for a parser rule {@code expr}.</li>
 * <li>{@code id:ID}: A labeled placeholder for a token of type {@code ID}.</li>
 * </ul>
 *
 * This class does not perform any validation on the tag or label names aside
 * from ensuring that the tag is a non-null, non-empty string.
 */
class TagChunk : Chunk
{
    /**
	 * This is the backing field for {@link #getTag}.
	 */
    private readonly string tag;
    /**
	 * This is the backing field for {@link #getLabel}.
	 */
    private readonly string label;

    /**
	 * Construct a new instance of {@link TagChunk} using the specified tag and
	 * no label.
	 *
	 * @param tag The tag, which should be the name of a parser rule or token
	 * type.
	 *
	 * @exception IllegalArgumentException if {@code tag} is {@code null} or
	 * empty.
	 */
    public TagChunk(string tag) : this(null, tag)
    {
    }

    /**
	 * Construct a new instance of {@link TagChunk} using the specified label
	 * and tag.
	 *
	 * @param label The label for the tag. If this is {@code null}, the
	 * {@link TagChunk} represents an unlabeled tag.
	 * @param tag The tag, which should be the name of a parser rule or token
	 * type.
	 *
	 * @exception IllegalArgumentException if {@code tag} is {@code null} or
	 * empty.
	 */
    public TagChunk(string label, string tag)
    {
        if (string.IsNullOrEmpty(tag))
        {
            throw new ArgumentException("tag cannot be null or empty", nameof(tag));
        }

        this.label = label;
        this.tag = tag;
    }

    /**
	 * Get the tag for this chunk.
	 *
	 * @return The tag for the chunk.
	 */

    public string Tag => tag;

    /**
	 * Get the label, if any, assigned to this chunk.
	 *
	 * @return The label assigned to this chunk, or {@code null} if no label is
	 * assigned to the chunk.
	 */

    public string Label => label;

    /**
	 * This method returns a text representation of the tag chunk. Labeled tags
	 * are returned in the form {@code label:tag}, and unlabeled tags are
	 * returned as just the tag name.
	 */
    //@Override
    public override string ToString() => label != null ? label + ":" + tag : tag;
}
