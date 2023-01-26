/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

namespace org.antlr.v4.runtime.tree.pattern;

/**
 * A {@link Token} object representing a token of a particular type; e.g.,
 * {@code <ID>}. These tokens are created for {@link TagChunk} chunks where the
 * tag corresponds to a lexer rule or token type.
 */
public class TokenTagToken : CommonToken {
	/**
	 * This is the backing field for {@link #getTokenName}.
	 */

	private readonly String tokenName;
	/**
	 * This is the backing field for {@link #getLabel}.
	 */

	private readonly String label;

	/**
	 * Constructs a new instance of {@link TokenTagToken} for an unlabeled tag
	 * with the specified token name and type.
	 *
	 * @param tokenName The token name.
	 * @param type The token type.
	 */
	public TokenTagToken(String tokenName, int type):this(tokenName,type,null) {
	}

	/**
	 * Constructs a new instance of {@link TokenTagToken} with the specified
	 * token name, type, and label.
	 *
	 * @param tokenName The token name.
	 * @param type The token type.
	 * @param label The label associated with the token tag, or {@code null} if
	 * the token tag is unlabeled.
	 */
	public TokenTagToken(String tokenName, int type, String label):base(type) {
		this.tokenName = tokenName;
		this.label = label;
	}

	/**
	 * Gets the token name.
	 * @return The token name.
	 */

	public  String getTokenName() {
		return tokenName;
	}

	/**
	 * Gets the label associated with the rule tag.
	 *
	 * @return The name of the label associated with the rule tag, or
	 * {@code null} if this is an unlabeled rule tag.
	 */

	public String getLabel() {
		return label;
	}

	/**
	 * {@inheritDoc}
	 *
	 * <p>The implementation for {@link TokenTagToken} returns the token tag
	 * formatted with {@code <} and {@code >} delimiters.</p>
	 */
	//@Override
	public String getText() {
		if (label != null) {
			return "<" + label + ":" + tokenName + ">";
		}

		return "<" + tokenName + ">";
	}

	/**
	 * {@inheritDoc}
	 *
	 * <p>The implementation for {@link TokenTagToken} returns a string of the form
	 * {@code tokenName:type}.</p>
	 */
	//@Override
	public override String ToString() {
		return tokenName + ":" + type;
	}
}
