/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime.misc;
using System.Text;

namespace org.antlr.v4.runtime.tree.pattern;

/**
 * A tree pattern matching mechanism for ANTLR {@link ParseTree}s.
 *
 * <p>Patterns are strings of source input text with special tags representing
 * token or rule references such as:</p>
 *
 * <p>{@code <ID> = <expr>;}</p>
 *
 * <p>Given a pattern start rule such as {@code statement}, this object constructs
 * a {@link ParseTree} with placeholders for the {@code ID} and {@code expr}
 * subtree. Then the {@link #match} routines can compare an actual
 * {@link ParseTree} from a parse with this pattern. Tag {@code <ID>} matches
 * any {@code ID} token and tag {@code <expr>} references the result of the
 * {@code expr} rule (generally an instance of {@code ExprContext}.</p>
 *
 * <p>Pattern {@code x = 0;} is a similar pattern that matches the same pattern
 * except that it requires the identifier to be {@code x} and the expression to
 * be {@code 0}.</p>
 *
 * <p>The {@link #matches} routines return {@code true} or {@code false} based
 * upon a match for the tree rooted at the parameter sent in. The
 * {@link #match} routines return a {@link ParseTreeMatch} object that
 * contains the parse tree, the parse tree pattern, and a map from tag name to
 * matched nodes (more below). A subtree that fails to match, returns with
 * {@link ParseTreeMatch#mismatchedNode} set to the first tree node that did not
 * match.</p>
 *
 * <p>For efficiency, you can compile a tree pattern in string form to a
 * {@link ParseTreePattern} object.</p>
 *
 * <p>See {@code TestParseTreeMatcher} for lots of examples.
 * {@link ParseTreePattern} has two static helper methods:
 * {@link ParseTreePattern#findAll} and {@link ParseTreePattern#match} that
 * are easy to use but not super efficient because they create new
 * {@link ParseTreePatternMatcher} objects each time and have to compile the
 * pattern in string form before using it.</p>
 *
 * <p>The lexer and parser that you pass into the {@link ParseTreePatternMatcher}
 * constructor are used to parse the pattern in string form. The lexer converts
 * the {@code <ID> = <expr>;} into a sequence of four tokens (assuming lexer
 * throws out whitespace or puts it on a hidden channel). Be aware that the
 * input stream is reset for the lexer (but not the parser; a
 * {@link ParserInterpreter} is created to parse the input.). Any user-defined
 * fields you have put into the lexer might get changed when this mechanism asks
 * it to scan the pattern string.</p>
 *
 * <p>Normally a parser does not accept token {@code <expr>} as a valid
 * {@code expr} but, from the parser passed in, we create a special version of
 * the underlying grammar representation (an {@link ATN}) that allows imaginary
 * tokens representing rules ({@code <expr>}) to match entire rules. We call
 * these <em>bypass alternatives</em>.</p>
 *
 * <p>Delimiters are {@code <} and {@code >}, with {@code \} as the escape string
 * by default, but you can set them to whatever you want using
 * {@link #setDelimiters}. You must escape both start and stop strings
 * {@code \<} and {@code \>}.</p>
 */
public class ParseTreePatternMatcher
{
    public class CannotInvokeStartRule : RuntimeException
    {
        public CannotInvokeStartRule(Exception e) : base(e.Message, e) { }
    }

    // Fixes https://github.com/antlr/antlr4/issues/413
    // "Tree pattern compilation doesn't check for a complete parse"
    public class StartRuleDoesNotConsumeFullPattern : RuntimeException { }

    /**
	 * This is the backing field for {@link #getLexer()}.
	 */
    private readonly Lexer lexer;

    /**
	 * This is the backing field for {@link #getParser()}.
	 */
    private readonly Parser parser;
    protected string start = "<";
    protected string stop = ">";
    protected string escape = "\\"; // e.g., \< and \> must escape BOTH!

    /**
	 * Constructs a {@link ParseTreePatternMatcher} or from a {@link Lexer} and
	 * {@link Parser} object. The lexer input stream is altered for tokenizing
	 * the tree patterns. The parser is used as a convenient mechanism to get
	 * the grammar name, plus token, rule names.
	 */
    public ParseTreePatternMatcher(Lexer lexer, Parser parser)
    {
        this.lexer = lexer;
        this.parser = parser;
    }

    /**
	 * Set the delimiters used for marking rule and token tags within concrete
	 * syntax used by the tree pattern parser.
	 *
	 * @param start The start delimiter.
	 * @param stop The stop delimiter.
	 * @param escapeLeft The escape sequence to use for escaping a start or stop delimiter.
	 *
	 * @exception IllegalArgumentException if {@code start} is {@code null} or empty.
	 * @exception IllegalArgumentException if {@code stop} is {@code null} or empty.
	 */
    public void SetDelimiters(string start, string stop, string escapeLeft)
    {
        if (string.IsNullOrEmpty(start))
        {
            throw new ArgumentException("start cannot be null or empty");
        }

        if (string.IsNullOrEmpty(stop))
        {
            throw new ArgumentException("stop cannot be null or empty");
        }

        this.start = start;
        this.stop = stop;
        this.escape = escapeLeft;
    }

    /** Does {@code pattern} matched as rule {@code patternRuleIndex} match {@code tree}? */
    public bool Matches(ParseTree tree, string pattern, int patternRuleIndex)
    {
        var p = Compile(pattern, patternRuleIndex);
        return Matches(tree, p);
    }

    /** Does {@code pattern} matched as rule patternRuleIndex match tree? Pass in a
	 *  compiled pattern instead of a string representation of a tree pattern.
	 */
    public bool Matches(ParseTree tree, ParseTreePattern pattern)
    {
        var labels = new MultiMap<string, ParseTree>();
        var mismatchedNode = MatchImpl(tree, pattern.PatternTree, labels);
        return mismatchedNode == null;
    }

    /**
	 * Compare {@code pattern} matched as rule {@code patternRuleIndex} against
	 * {@code tree} and return a {@link ParseTreeMatch} object that contains the
	 * matched elements, or the node at which the match failed.
	 */
    public ParseTreeMatch Match(ParseTree tree, string pattern, int patternRuleIndex)
    {
        var p = Compile(pattern, patternRuleIndex);
        return Match(tree, p);
    }

    /**
	 * Compare {@code pattern} matched against {@code tree} and return a
	 * {@link ParseTreeMatch} object that contains the matched elements, or the
	 * node at which the match failed. Pass in a compiled pattern instead of a
	 * string representation of a tree pattern.
	 */

    public ParseTreeMatch Match(ParseTree tree, ParseTreePattern pattern)
    {
        var labels = new MultiMap<string, ParseTree>();
        var mismatchedNode = MatchImpl(tree, pattern.PatternTree, labels);
        return new ParseTreeMatch(tree, pattern, labels, mismatchedNode);
    }

    /**
	 * For repeated use of a tree pattern, compile it to a
	 * {@link ParseTreePattern} using this method.
	 */
    public ParseTreePattern Compile(string pattern, int patternRuleIndex)
    {
        var tokenList = Tokenize(pattern);
        var tokenSrc = new ListTokenSource(tokenList);
        var tokens = new CommonTokenStream(tokenSrc);

        var parserInterp = new ParserInterpreter(parser.GrammarFileName,
                                                               parser.Vocabulary,
                                                               parser.RuleNames,
                                                               parser.GetATNWithBypassAlts(),
                                                               tokens);

        ParseTree tree = null;
        try
        {
            parserInterp.ErrorHandler = new BailErrorStrategy();
            tree = parserInterp.Parse(patternRuleIndex);
            //			Console.Out.WriteLine("pattern tree = "+tree.toStringTree(parserInterp));
        }
        catch (ParseCancellationException e)
        {
            throw (RecognitionException)e.GetBaseException();
        }
        catch (RecognitionException re)
        {
            throw re;
        }
        catch (Exception e)
        {
            throw new CannotInvokeStartRule(e);
        }

        // Make sure tree pattern compilation checks for a complete parse
        if (tokens.LA(1) != Token.EOF)
        {
            throw new StartRuleDoesNotConsumeFullPattern();
        }

        return new ParseTreePattern(this, pattern, patternRuleIndex, tree);
    }

    /**
	 * Used to convert the tree pattern string into a series of tokens. The
	 * input stream is reset.
	 */

    public Lexer Lexer => lexer;

    /**
	 * Used to collect to the grammar file name, token names, rule names for
	 * used to parse the pattern into a parse tree.
	 */

    public Parser Parser => parser;

    // ---- SUPPORT CODE ----

    /**
	 * Recursively walk {@code tree} against {@code patternTree}, filling
	 * {@code match.}{@link ParseTreeMatch#labels labels}.
	 *
	 * @return the first node encountered in {@code tree} which does not match
	 * a corresponding node in {@code patternTree}, or {@code null} if the match
	 * was successful. The specific node returned depends on the matching
	 * algorithm used by the implementation, and may be overridden.
	 */

    protected ParseTree MatchImpl(ParseTree tree,
                                  ParseTree patternTree,
                                  MultiMap<string, ParseTree> labels)
    {
        if (tree == null)
        {
            throw new ArgumentException("tree cannot be null", nameof(tree));
        }

        if (patternTree == null)
        {
            throw new ArgumentException("patternTree cannot be null", nameof(patternTree));
        }

        // x and <ID>, x and y, or x and x; or could be mismatched types
        if (tree is TerminalNode && patternTree is TerminalNode)
        {
            var t1 = (TerminalNode)tree;
            var t2 = (TerminalNode)patternTree;
            ParseTree mismatchedNode = null;
            // both are tokens and they have same type
            if (t1.GetSymbol().Type == t2.GetSymbol().Type)
            {
                if (t2.GetSymbol() is TokenTagToken)
                { // x and <ID>
                    var tokenTagToken = (TokenTagToken)t2.GetSymbol();
                    // track label->list-of-nodes for both token name and label (if any)
                    labels.Map(tokenTagToken.TokenName, tree);
                    if (tokenTagToken.Label != null)
                    {
                        labels.Map(tokenTagToken.Label, tree);
                    }
                }
                else if (t1.Text.Equals(t2.Text))
                {
                    // x and x
                }
                else
                {
                    // x and y
                    mismatchedNode ??= t1;
                }
            }
            else
            {
                if (mismatchedNode == null)
                {
                    mismatchedNode = t1;
                }
            }

            return mismatchedNode;
        }

        if (tree is ParserRuleContext r1 && patternTree is ParserRuleContext r2)
        {
            ParseTree mismatchedNode = null;
            // (expr ...) and <expr>
            var ruleTagToken = GetRuleTagToken(r2);
            if (ruleTagToken != null)
            {
                ParseTreeMatch m = null;
                if (r1.CurrentRuleContext.RuleIndex == r2.CurrentRuleContext.RuleIndex)
                {
                    // track label->list-of-nodes for both rule name and label (if any)
                    labels.Map(ruleTagToken.RuleName, tree);
                    if (ruleTagToken.Label != null)
                    {
                        labels.Map(ruleTagToken.Label, tree);
                    }
                }
                else
                {
                    if (mismatchedNode == null)
                    {
                        mismatchedNode = r1;
                    }
                }

                return mismatchedNode;
            }

            // (expr ...) and (expr ...)
            if (r1.ChildCount != r2.ChildCount)
            {
                if (mismatchedNode == null)
                {
                    mismatchedNode = r1;
                }

                return mismatchedNode;
            }

            int n = r1.ChildCount;
            for (int i = 0; i < n; i++)
            {
                var childMatch = MatchImpl(r1.GetChild(i), patternTree.GetChild(i), labels);
                if (childMatch != null)
                {
                    return childMatch;
                }
            }

            return mismatchedNode;
        }

        // if nodes aren't both tokens or both rule nodes, can't match
        return tree;
    }

    /** Is {@code t} {@code (expr <expr>)} subtree? */
    protected RuleTagToken GetRuleTagToken(ParseTree t)
    {
        if (t is RuleNode r)
        {
            if (r.ChildCount == 1 && r.GetChild(0) is TerminalNode c)
            {
                if (c.GetSymbol() is RuleTagToken)
                {
                    //					Console.Out.WriteLine("rule tag subtree "+t.toStringTree(parser));
                    return (RuleTagToken)c.GetSymbol();
                }
            }
        }
        return null;
    }

    public List<Token> Tokenize(string pattern)
    {
        // split pattern into chunks: sea (raw input) and islands (<ID>, <expr>)
        var chunks = Split(pattern);

        // create token stream from text and tags
        List<Token> tokens = new();
        foreach (var chunk in chunks)
        {
            if (chunk is TagChunk tagChunk)
            {
                // add special rule token or conjure up new token from name
                if (char.IsUpper(tagChunk.Tag[(0)]))
                {
                    int ttype = parser.GetTokenType(tagChunk.Tag);
                    if (ttype == Token.INVALID_TYPE)
                    {
                        throw new ArgumentException("Unknown token " + tagChunk.Tag + " in pattern: " + pattern);
                    }
                    var t = new TokenTagToken(tagChunk.Tag, ttype, tagChunk.Label);
                    tokens.Add(t);
                }
                else if (char.IsLower(tagChunk.Tag[(0)]))
                {
                    int ruleIndex = parser.GetRuleIndex(tagChunk.Tag);
                    if (ruleIndex == -1)
                    {
                        throw new ArgumentException("Unknown rule " + tagChunk.Tag + " in pattern: " + pattern);
                    }
                    int ruleImaginaryTokenType = parser.GetATNWithBypassAlts().ruleToTokenType[ruleIndex];
                    tokens.Add(new RuleTagToken(tagChunk.Tag, ruleImaginaryTokenType, tagChunk.Label));
                }
                else
                {
                    throw new ArgumentException("invalid tag: " + tagChunk.Tag + " in pattern: " + pattern);
                }
            }
            else
            {
                var textChunk = (TextChunk)chunk;
                var @in = new ANTLRInputStream(textChunk.Text);
                lexer.InputStream = @in;
                Token t = lexer.NextToken();
                while (t.Type != Token.EOF)
                {
                    tokens.Add(t);
                    t = lexer.NextToken();
                }
            }
        }

        //		Console.Out.WriteLine("tokens="+tokens);
        return tokens;
    }

    /** Split {@code <ID> = <e:expr> ;} into 4 chunks for tokenizing by {@link #tokenize}. */
    public List<Chunk> Split(string pattern)
    {
        int p = 0;
        int n = pattern.Length;
        List<Chunk> chunks = new();
        var buffer = new StringBuilder();
        // find all start and stop indexes first, then collect
        List<int> starts = new();
        List<int> stops = new();
        while (p < n)
        {
            if (p == pattern.IndexOf(escape + start, p))
            {
                p += escape.Length + start.Length;
            }
            else if (p == pattern.IndexOf(escape + stop, p))
            {
                p += escape.Length + stop.Length;
            }
            else if (p == pattern.IndexOf(start, p))
            {
                starts.Add(p);
                p += start.Length;
            }
            else if (p == pattern.IndexOf(stop, p))
            {
                stops.Add(p);
                p += stop.Length;
            }
            else
            {
                p++;
            }
        }

        //		Console.Out.WriteLine("");
        //		Console.Out.WriteLine(starts);
        //		Console.Out.WriteLine(stops);
        if (starts.Count > stops.Count)
        {
            throw new ArgumentException("unterminated tag in pattern: " + pattern);
        }

        if (starts.Count < stops.Count)
        {
            throw new ArgumentException("missing start tag in pattern: " + pattern);
        }

        int ntags = starts.Count;
        for (int i = 0; i < ntags; i++)
        {
            if (starts[(i)] >= stops[(i)])
            {
                throw new ArgumentException("tag delimiters out of order in pattern: " + pattern);
            }
        }

        // collect into chunks now
        if (ntags == 0)
        {
            var text = pattern.Substring(0, n - 0);
            chunks.Add(new TextChunk(text));
        }

        if (ntags > 0 && starts[(0)] > 0)
        { // copy text up to first tag into chunks
            var text = pattern.Substring(0, starts[(0)] - 0);
            chunks.Add(new TextChunk(text));
        }
        for (int i = 0; i < ntags; i++)
        {
            // copy inside of <tag>
            var tag = pattern.Substring(starts[(i)] + start.Length, stops[(i)] - (starts[(i)] + start.Length));
            var ruleOrToken = tag;
            string label = null;
            int colon = tag.IndexOf(':');
            if (colon >= 0)
            {
                label = tag.Substring(0, colon - 0);
                ruleOrToken = tag.Substring(colon + 1, tag.Length - (colon + 1));
            }
            chunks.Add(new TagChunk(label, ruleOrToken));
            if (i + 1 < ntags)
            {
                // copy from end of <tag> to start of next
                var text = pattern.Substring(stops[(i)] + stop.Length, starts[(i + 1)] - (stops[(i)] + stop.Length));
                chunks.Add(new TextChunk(text));
            }
        }
        if (ntags > 0)
        {
            int afterLastTag = stops[(ntags - 1)] + stop.Length;
            if (afterLastTag < n)
            { // copy text from end of last tag to end
                var text = pattern.Substring(afterLastTag, n - afterLastTag);
                chunks.Add(new TextChunk(text));
            }
        }

        // strip out the escape sequences from text chunks but not tags
        for (int i = 0; i < chunks.Count; i++)
        {
            var c = chunks[(i)];
            if (c is TextChunk tc)
            {
                var unescaped = tc.Text.Replace(escape, "");
                if (unescaped.Length < tc.Text.Length)
                {
                    chunks[i] = new TextChunk(unescaped);
                }
            }
        }

        return chunks;
    }
}
