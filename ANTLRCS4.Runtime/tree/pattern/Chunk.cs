/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

namespace org.antlr.v4.runtime.tree.pattern;

/**
 * A chunk is either a token tag, a rule tag, or a span of literal text within a
 * tree pattern.
 *
 * <p>The method {@link ParseTreePatternMatcher#split(string)} returns a list of
 * chunks in preparation for creating a token stream by
 * {@link ParseTreePatternMatcher#tokenize(string)}. From there, we get a parse
 * tree from with {@link ParseTreePatternMatcher#compile(string, int)}. These
 * chunks are converted to {@link RuleTagToken}, {@link TokenTagToken}, or the
 * regular tokens of the text surrounding the tags.</p>
 */
public abstract class Chunk
{
}
