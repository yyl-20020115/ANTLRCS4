/*
 [The "BSD license"]
 Copyright (c) 2005-2009 Terence Parr
 All rights reserved.

 Redistribution and use in source and binary forms, with or without
 modification, are permitted provided that the following conditions
 are met:
 1. Redistributions of source code must retain the above copyright
     notice, this list of conditions and the following disclaimer.
 2. Redistributions in binary form must reproduce the above copyright
     notice, this list of conditions and the following disclaimer in the
     documentation and/or other materials provided with the distribution.
 3. The name of the author may not be used to endorse or promote products
     derived from this software without specific prior written permission.

 THIS SOFTWARE IS PROVIDED BY THE AUTHOR ``AS IS'' AND ANY EXPRESS OR
 IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
 OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.
 IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY DIRECT, INDIRECT,
 INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT
 NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
 DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
 THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
 THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */
using org.antlr.runtime;
using org.antlr.v4.runtime.misc;
using org.antlr.v4.runtime;
using System.Text.RegularExpressions;

namespace org.antlr.runtime.tree;


/** A parser for a stream of tree nodes.  "tree grammars" result in a subclass
 *  of this.  All the error reporting and recovery is shared with Parser via
 *  the BaseRecognizer superclass.
*/
public class TreeParser : BaseRecognizer
{

    public const int DOWN = Token.DOWN;
    public const int UP = Token.UP;

    // precompiled regex used by inContext
    static String dotdot = ".*[^.]\\.\\.[^.].*";
    static String doubleEtc = ".*\\.\\.\\.\\s+\\.\\.\\..*";
    static Regex dotdotPattern = new(dotdot);
    static Regex doubleEtcPattern = new(doubleEtc);

    protected TreeNodeStream input;

    public TreeParser(TreeNodeStream input) : base()
    {
        // highlight that we go to base to set state object
        TreeNodeStream = input;
    }

    public TreeParser(TreeNodeStream input, RecognizerSharedState state)
        : base(state)
    {
        // share the state object with another parser
        TreeNodeStream = input;
    }

    public override void Reset()
    {
        base.Reset(); // reset all recognizer state variables
        input?.Seek(0); // rewind the input
    }

    /** Set the input stream */
    public TreeNodeStream TreeNodeStream { get => input; set => this.input = value; }

    public override string[] TokenNames => base.TokenNames;
    
    public override string SourceName => input.SourceName;
    public override string GrammarFileName => base.GrammarFileName;
    
    protected override object GetCurrentInputSymbol(IntStream input)
    {
        return ((TreeNodeStream)input).LT(1);
    }

    
    protected override object GetMissingSymbol(IntStream input,
                                      RecognitionException e,
                                      int expectedTokenType,
                                      BitSet follow)
    {
        var tokenText =
            "<missing " + TokenNames[expectedTokenType] + ">";
        var adaptor = ((TreeNodeStream)e.InputStream).getTreeAdaptor();
        return adaptor.create(new CommonToken(expectedTokenType, tokenText));
    }

    /** Match '.' in tree parser has special meaning.  Skip node or
     *  entire tree if node has children.  If children, scan until
     *  corresponding UP node.
     */
    
    public override void MatchAny(IntStream ignore)
    { // ignore stream, copy of input
        state.errorRecovery = false;
        state.failed = false;
        var look = input.LT(1);
        if (input.getTreeAdaptor().getChildCount(look) == 0)
        {
            input.Consume(); // not subtree, consume 1 node and return
            return;
        }
        // current node is a subtree, skip to corresponding UP.
        // must count nesting level to get right UP
        int level = 0;
        int tokenType = input.getTreeAdaptor().getType(look);
        while (tokenType != Token.EOF && !(tokenType == UP && level == 0))
        {
            input.Consume();
            look = input.LT(1);
            tokenType = input.getTreeAdaptor().getType(look);
            if (tokenType == DOWN)
            {
                level++;
            }
            else if (tokenType == UP)
            {
                level--;
            }
        }
        input.Consume(); // consume UP
    }

    /** We have DOWN/UP nodes in the stream that have no line info; override.
     *  plus we want to alter the exception type.  Don't try to recover
     *  from tree parser errors inline...
     */
    
    protected override object RecoverFromMismatchedToken(IntStream input,
                                                int ttype,
                                                BitSet follow)

    {
        throw new MismatchedTreeNodeException(ttype, (TreeNodeStream)input);
    }

    /** Prefix error message with the grammar name because message is
     *  always intended for the programmer because the parser built
     *  the input tree not the user.
     */
    public string GetErrorHeader(RecognitionException e)
    {
        return GrammarFileName + ": node from " +
               (e.approximateLineInfo ? "after " : "") + "line " + e.line + ":" + e.charPositionInLine;
    }

    /** Tree parsers parse nodes they usually have a token object as
     *  payload. Set the exception token and do the default behavior.
     */
    public string GetErrorMessage(RecognitionException e, string[] tokenNames)
    {
        if (this is TreeParser)
        {
            TreeAdaptor adaptor = ((TreeNodeStream)e.InputStream).getTreeAdaptor();
            e.token = adaptor.getToken(e.node);
            if (e.token == null)
            { // could be an UP/DOWN node
                e.token = new CommonToken(adaptor.getType(e.node),
                                          adaptor.getText(e.node));
            }
        }
        return base.GetErrorMessage(e, tokenNames);
    }

    /** Check if current node in input has a context.  Context means sequence
     *  of nodes towards root of tree.  For example, you might say context
     *  is "MULT" which means my parent must be MULT.  "CLASS VARDEF" says
     *  current node must be child of a VARDEF and whose parent is a CLASS node.
     *  You can use "..." to mean zero-or-more nodes.  "METHOD ... VARDEF"
     *  means my parent is VARDEF and somewhere above that is a METHOD node.
     *  The first node in the context is not necessarily the root.  The context
     *  matcher stops matching and returns true when it runs out of context.
     *  There is no way to force the first node to be the root.
     */
    public bool InContext(String context)
    {
        return InContext(input.getTreeAdaptor(), TokenNames, input.LT(1), context);
    }

    /** The worker for inContext.  It's static and full of parameters for
     *  testing purposes.
     */
    public static bool InContext(TreeAdaptor adaptor,
                                    string[] tokenNames,
                                    object t,
                                    string context)
    {
        var dotdotMatcher = dotdotPattern.Match(context);
        var doubleEtcMatcher = doubleEtcPattern.Match(context);
        if (dotdotMatcher.Success)
        { // don't allow "..", must be "..."
            throw new ArgumentException("invalid syntax: ..");
        }
        if (doubleEtcMatcher.Success)
        { // don't allow double "..."
            throw new ArgumentException("invalid syntax: ... ...");
        }
        context = context.Replace("\\.\\.\\.", " ... "); // ensure spaces around ...
        context = context.Trim();
        var nodes = context.Split("\\s+");
        int ni = nodes.Length - 1;
        t = adaptor.getParent(t);
        while (ni >= 0 && t != null)
        {
            if (nodes[ni].Equals("..."))
            {
                // walk upwards until we see nodes[ni-1] then continue walking
                if (ni == 0) return true; // ... at start is no-op
                var goal = nodes[ni - 1];
                var ancestor = GetAncestor(adaptor, tokenNames, t, goal);
                if (ancestor == null) return false;
                t = ancestor;
                ni--;
            }
            var name = tokenNames[adaptor.getType(t)];
            if (!name.Equals(nodes[ni]))
            {
                //Console.Error.WriteLine("not matched: "+nodes[ni]+" at "+t);
                return false;
            }
            // advance to parent and to previous element in context node list
            ni--;
            t = adaptor.getParent(t);
        }

        if (t == null && ni >= 0) return false; // at root but more nodes to match
        return true;
    }

    /** Helper for static inContext */
    protected static object GetAncestor(TreeAdaptor adaptor, string[] tokenNames, object t, string goal)
    {
        while (t != null)
        {
            var name = tokenNames[adaptor.getType(t)];
            if (name.Equals(goal)) return t;
            t = adaptor.getParent(t);
        }
        return null;
    }

    public void TraceIn(String ruleName, int ruleIndex)
    {
        base.TraceIn(ruleName, ruleIndex, input.LT(1));
    }

    public void TraceOut(String ruleName, int ruleIndex)
    {
        base.TraceOut(ruleName, ruleIndex, input.LT(1));
    }
}
