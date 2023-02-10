/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime.tree.xpath;

namespace org.antlr.v4.runtime.tree.pattern;

/**
 * A pattern like {@code <ID> = <expr>;} converted to a {@link ParseTree} by
 * {@link ParseTreePatternMatcher#compile(string, int)}.
 */
public class ParseTreePattern
{
    /**
	 * This is the backing field for {@link #getPatternRuleIndex()}.
	 */
    private readonly int patternRuleIndex;

    /**
	 * This is the backing field for {@link #getPattern()}.
	 */

    private readonly string pattern;

    /**
	 * This is the backing field for {@link #getPatternTree()}.
	 */

    private readonly ParseTree patternTree;

    /**
	 * This is the backing field for {@link #getMatcher()}.
	 */

    private readonly ParseTreePatternMatcher matcher;

    /**
	 * Construct a new instance of the {@link ParseTreePattern} class.
	 *
	 * @param matcher The {@link ParseTreePatternMatcher} which created this
	 * tree pattern.
	 * @param pattern The tree pattern in concrete syntax form.
	 * @param patternRuleIndex The parser rule which serves as the root of the
	 * tree pattern.
	 * @param patternTree The tree pattern in {@link ParseTree} form.
	 */
    public ParseTreePattern(ParseTreePatternMatcher matcher,
                            string pattern, int patternRuleIndex, ParseTree patternTree)
    {
        this.matcher = matcher;
        this.patternRuleIndex = patternRuleIndex;
        this.pattern = pattern;
        this.patternTree = patternTree;
    }

    /**
	 * Match a specific parse tree against this tree pattern.
	 *
	 * @param tree The parse tree to match against this tree pattern.
	 * @return A {@link ParseTreeMatch} object describing the result of the
	 * match operation. The {@link ParseTreeMatch#succeeded()} method can be
	 * used to determine whether or not the match was successful.
	 */

    public ParseTreeMatch Match(ParseTree tree) => matcher.Match(tree, this);

    /**
	 * Determine whether or not a parse tree matches this tree pattern.
	 *
	 * @param tree The parse tree to match against this tree pattern.
	 * @return {@code true} if {@code tree} is a match for the current tree
	 * pattern; otherwise, {@code false}.
	 */
    public bool Matches(ParseTree tree) => matcher.Match(tree, this).Succeeded();

    /**
	 * Find all nodes using XPath and then try to match those subtrees against
	 * this tree pattern.
	 *
	 * @param tree The {@link ParseTree} to match against this pattern.
	 * @param xpath An expression matching the nodes
	 *
	 * @return A collection of {@link ParseTreeMatch} objects describing the
	 * successful matches. Unsuccessful matches are omitted from the result,
	 * regardless of the reason for the failure.
	 */

    public List<ParseTreeMatch> FindAll(ParseTree tree, string xpath)
    {
        var subtrees = XPath.FindAll(tree, xpath, matcher.Parser);
        List<ParseTreeMatch> matches = new();
        foreach (ParseTree t in subtrees)
        {
            var matched = Match(t);
            if (matched.Succeeded())
            {
                matches.Add(matched);
            }
        }
        return matches;
    }

    /**
	 * Get the {@link ParseTreePatternMatcher} which created this tree pattern.
	 *
	 * @return The {@link ParseTreePatternMatcher} which created this tree
	 * pattern.
	 */

    public ParseTreePatternMatcher Matcher => matcher;

    /**
	 * Get the tree pattern in concrete syntax form.
	 *
	 * @return The tree pattern in concrete syntax form.
	 */

    public string Pattern => pattern;

    /**
	 * Get the parser rule which serves as the outermost rule for the tree
	 * pattern.
	 *
	 * @return The parser rule which serves as the outermost rule for the tree
	 * pattern.
	 */
    public int PatternRuleIndex => patternRuleIndex;

    /**
	 * Get the tree pattern as a {@link ParseTree}. The rule and token tags from
	 * the pattern are present in the parse tree as terminal nodes with a symbol
	 * of type {@link RuleTagToken} or {@link TokenTagToken}.
	 *
	 * @return The tree pattern as a {@link ParseTree}.
	 */

    public ParseTree PatternTree => patternTree;
}
