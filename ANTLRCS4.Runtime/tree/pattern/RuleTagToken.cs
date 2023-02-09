/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

namespace org.antlr.v4.runtime.tree.pattern;

/**
 * A {@link Token} object representing an entire subtree matched by a parser
 * rule; e.g., {@code <expr>}. These tokens are created for {@link TagChunk}
 * chunks where the tag corresponds to a parser rule.
 */
public class RuleTagToken : Token
{
    /**
	 * This is the backing field for {@link #getRuleName}.
	 */
    private readonly string ruleName;
    /**
	 * The token type for the current token. This is the token type assigned to
	 * the bypass alternative for the rule during ATN deserialization.
	 */
    private readonly int bypassTokenType;
    /**
	 * This is the backing field for {@link #getLabel}.
	 */
    private readonly string label;

    /**
	 * Constructs a new instance of {@link RuleTagToken} with the specified rule
	 * name and bypass token type and no label.
	 *
	 * @param ruleName The name of the parser rule this rule tag matches.
	 * @param bypassTokenType The bypass token type assigned to the parser rule.
	 *
	 * @exception IllegalArgumentException if {@code ruleName} is {@code null}
	 * or empty.
	 */
    public RuleTagToken(string ruleName, int bypassTokenType) : this(ruleName, bypassTokenType, null)
    {
    }

    /**
	 * Constructs a new instance of {@link RuleTagToken} with the specified rule
	 * name, bypass token type, and label.
	 *
	 * @param ruleName The name of the parser rule this rule tag matches.
	 * @param bypassTokenType The bypass token type assigned to the parser rule.
	 * @param label The label associated with the rule tag, or {@code null} if
	 * the rule tag is unlabeled.
	 *
	 * @exception IllegalArgumentException if {@code ruleName} is {@code null}
	 * or empty.
	 */
    public RuleTagToken(string ruleName, int bypassTokenType, string label)
    {
        if (string.IsNullOrEmpty(ruleName))
        {
            throw new ArgumentException("ruleName cannot be null or empty.", nameof(ruleName));
        }

        this.ruleName = ruleName;
        this.bypassTokenType = bypassTokenType;
        this.label = label;
    }

    /**
	 * Gets the name of the rule associated with this rule tag.
	 *
	 * @return The name of the parser rule associated with this rule tag.
	 */

    public string RuleName => ruleName;

    /**
	 * Gets the label associated with the rule tag.
	 *
	 * @return The name of the label associated with the rule tag, or
	 * {@code null} if this is an unlabeled rule tag.
	 */

    public string Label => label;

    /**
	 * {@inheritDoc}
	 *
	 * <p>Rule tag tokens are always placed on the {@link #DEFAULT_CHANNEL}.</p>
	 */
    
    public int Channel { get => Token.DEFAULT_CHANNEL; set { } }

    /**
	 * {@inheritDoc}
	 *
	 * <p>This method returns the rule tag formatted with {@code <} and {@code >}
	 * delimiters.</p>
	 */
    
    public string Text
    {
        get => label != null ? "<" + label + ":" + ruleName + ">" : "<" + ruleName + ">";

		set { }
    }

    /**
	 * {@inheritDoc}
	 *
	 * <p>Rule tag tokens have types assigned according to the rule bypass
	 * transitions created during ATN deserialization.</p>
	 */
    
    public int Type { get => bypassTokenType; set { } }

    /**
	 * {@inheritDoc}
	 *
	 * <p>The implementation for {@link RuleTagToken} always returns 0.</p>
	 */
    
    public int Line { get => 0; set => throw new NotImplementedException(); }

    /**
	 * {@inheritDoc}
	 *
	 * <p>The implementation for {@link RuleTagToken} always returns -1.</p>
	 */
    
    public int CharPositionInLine { get => -1; set { } }

    /**
	 * {@inheritDoc}
	 *
	 * <p>The implementation for {@link RuleTagToken} always returns -1.</p>
	 */
    
    public int TokenIndex { get => -1; set { } }

    /**
	 * {@inheritDoc}
	 *
	 * <p>The implementation for {@link RuleTagToken} always returns -1.</p>
	 */
    
    public int StartIndex { get => -1; set { } }

    /**
	 * {@inheritDoc}
	 *
	 * <p>The implementation for {@link RuleTagToken} always returns -1.</p>
	 */
    
    public int StopIndex { get => -1; set { } }

    /**
	 * {@inheritDoc}
	 *
	 * <p>The implementation for {@link RuleTagToken} always returns {@code null}.</p>
	 */
    
    public TokenSource TokenSource { get => null; set { } }

    /**
	 * {@inheritDoc}
	 *
	 * <p>The implementation for {@link RuleTagToken} always returns {@code null}.</p>
	 */
    
    public CharStream InputStream { get => null; set { } }

    /**
	 * {@inheritDoc}
	 *
	 * <p>The implementation for {@link RuleTagToken} returns a string of the form
	 * {@code ruleName:bypassTokenType}.</p>
	 */
    public override string ToString() => ruleName + ":" + bypassTokenType;
}
