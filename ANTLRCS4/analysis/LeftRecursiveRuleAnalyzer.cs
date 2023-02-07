/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using Antlr4.StringTemplate;
using org.antlr.runtime.tree;
using org.antlr.v4.codegen;
using org.antlr.v4.parse;
using org.antlr.v4.runtime;
using org.antlr.v4.runtime.misc;
using org.antlr.v4.runtime.tree;
using org.antlr.v4.tool;
using org.antlr.v4.tool.ast;
using System.Text;

namespace org.antlr.v4.analysis;

/** Using a tree walker on the rules, determine if a rule is directly left-recursive and if it follows
 *  our pattern.
 */
public class LeftRecursiveRuleAnalyzer : LeftRecursiveRuleWalker
{
    public enum ASSOC :int { left, right }

    public Tool tool;
    public string ruleName;
    public Dictionary<int, LeftRecursiveRuleAltInfo> binaryAlts = new();
    public Dictionary<int, LeftRecursiveRuleAltInfo> ternaryAlts = new();
    public Dictionary<int, LeftRecursiveRuleAltInfo> suffixAlts = new();
    public List<LeftRecursiveRuleAltInfo> prefixAndOtherAlts = new();

    /** Pointer to ID node of ^(= ID element) */
    public List<Pair<GrammarAST, String>> leftRecursiveRuleRefLabels =
        new();

    /** Tokens from which rule AST comes from */
    public readonly TokenStream tokenStream;

    public GrammarAST retvals;

    public readonly static TemplateGroup recRuleTemplates;
    public readonly TemplateGroup codegenTemplates;
    public readonly String language;

    public Dictionary<int, ASSOC> altAssociativity = new();

    static LeftRecursiveRuleAnalyzer()
    {
        var templateGroupFile = "org/antlr/v4/tool/templates/LeftRecursiveRules.stg";
        recRuleTemplates = new TemplateGroupFile(templateGroupFile);
        if (!recRuleTemplates.IsDefined("recRule"))
        {
            try
            {
                throw new FileNotFoundException(
                    "can't find code generation templates: LeftRecursiveRules");
            }
            catch (FileNotFoundException e)
            {
                //e.printStackTrace();
            }
        }
    }

    public LeftRecursiveRuleAnalyzer(GrammarAST ruleAST,
                                     Tool tool, String ruleName, String language)
        : base(new CommonTreeNodeStream(new GrammarASTAdaptor(ruleAST.token.InputStream), ruleAST))
    {
        this.tool = tool;
        this.ruleName = ruleName;
        this.language = language;
        this.tokenStream = ruleAST.g.tokenStream;
        if (this.tokenStream == null)
            throw new NullReferenceException("grammar must have a token stream");

        // use codegen to get correct language templates; that's it though
        codegenTemplates = CodeGenerator.Create(tool, null, language).Templates;
    }

    //@Override
    public void setReturnValues(GrammarAST t)
    {
        retvals = t;
    }

    //@Override
    public void setAltAssoc(AltAST t, int alt)
    {
        ASSOC assoc = ASSOC.left;
        if (t.getOptions() != null)
        {
            String a = t.getOptionString("assoc");
            if (a != null)
            {
                if (a.Equals(ASSOC.right.ToString()))
                {
                    assoc = ASSOC.right;
                }
                else if (a.Equals(ASSOC.left.ToString()))
                {
                    assoc = ASSOC.left;
                }
                else
                {
                    tool.ErrMgr.GrammarError(ErrorType.ILLEGAL_OPTION_VALUE, t.g.fileName, t.getOptionAST("assoc").Token, "assoc", assoc);
                }
            }
        }

        if (altAssociativity.TryGetValue(alt, out var r) && r != assoc)
        {
            tool.ErrMgr.toolError(ErrorType.INTERNAL_ERROR, "all operators of alt " + alt + " of left-recursive rule must have same associativity");
        }
        altAssociativity[alt] = assoc;

        //		Console.Out.WriteLine("setAltAssoc: op " + alt + ": " + t.getText()+", assoc="+assoc);
    }

    //@Override
    public void binaryAlt(AltAST originalAltTree, int alt)
    {
        AltAST altTree = (AltAST)originalAltTree.dupTree();
        String altLabel = altTree.altLabel != null ? altTree.altLabel.getText() : null;

        String label = null;
        bool isListLabel = false;
        GrammarAST lrlabel = stripLeftRecursion(altTree);
        if (lrlabel != null)
        {
            label = lrlabel.getText();
            isListLabel = lrlabel.getParent().Type == PLUS_ASSIGN;
            leftRecursiveRuleRefLabels.Add(new Pair<GrammarAST, String>(lrlabel, altLabel));
        }

        stripAltLabel(altTree);

        // rewrite e to be e_[rec_arg]
        int nextPrec = nextPrecedence(alt);
        altTree = addPrecedenceArgToRules(altTree, nextPrec);

        stripAltLabel(altTree);
        String altText = text(altTree);
        altText = altText.Trim();
        LeftRecursiveRuleAltInfo a =
            new LeftRecursiveRuleAltInfo(alt, altText, label, altLabel, isListLabel, originalAltTree);
        a.nextPrec = nextPrec;
        binaryAlts[alt] = a;
        //Console.Out.WriteLine("binaryAlt " + alt + ": " + altText + ", rewrite=" + rewriteText);
    }

    //@Override
    public void prefixAlt(AltAST originalAltTree, int alt)
    {
        AltAST altTree = (AltAST)originalAltTree.dupTree();
        stripAltLabel(altTree);

        int nextPrec = precedence(alt);
        // rewrite e to be e_[prec]
        altTree = addPrecedenceArgToRules(altTree, nextPrec);
        String altText = text(altTree);
        altText = altText.Trim();
        String altLabel = altTree.altLabel != null ? altTree.altLabel.getText() : null;
        LeftRecursiveRuleAltInfo a =
            new LeftRecursiveRuleAltInfo(alt, altText, null, altLabel, false, originalAltTree);
        a.nextPrec = nextPrec;
        prefixAndOtherAlts.Add(a);
        //Console.Out.WriteLine("prefixAlt " + alt + ": " + altText + ", rewrite=" + rewriteText);
    }

    //@Override
    public void suffixAlt(AltAST originalAltTree, int alt)
    {
        AltAST altTree = (AltAST)originalAltTree.dupTree();
        String altLabel = altTree.altLabel != null ? altTree.altLabel.getText() : null;

        String label = null;
        bool isListLabel = false;
        GrammarAST lrlabel = stripLeftRecursion(altTree);
        if (lrlabel != null)
        {
            label = lrlabel.getText();
            isListLabel = lrlabel.getParent().Type == PLUS_ASSIGN;
            leftRecursiveRuleRefLabels.Add(new Pair<GrammarAST, String>(lrlabel, altLabel));
        }
        stripAltLabel(altTree);
        String altText = text(altTree);
        altText = altText.Trim();
        LeftRecursiveRuleAltInfo a =
            new LeftRecursiveRuleAltInfo(alt, altText, label, altLabel, isListLabel, originalAltTree);
        suffixAlts[alt] = a;
        //		Console.Out.WriteLine("suffixAlt " + alt + ": " + altText + ", rewrite=" + rewriteText);
    }

    //@Override
    public void otherAlt(AltAST originalAltTree, int alt)
    {
        AltAST altTree = (AltAST)originalAltTree.dupTree();
        stripAltLabel(altTree);
        String altText = text(altTree);
        String altLabel = altTree.altLabel != null ? altTree.altLabel.getText() : null;
        LeftRecursiveRuleAltInfo a =
            new LeftRecursiveRuleAltInfo(alt, altText, null, altLabel, false, originalAltTree);
        // We keep other alts with prefix alts since they are all added to the start of the generated rule, and
        // we want to retain any prior ordering between them
        prefixAndOtherAlts.Add(a);
        //		Console.Out.WriteLine("otherAlt " + alt + ": " + altText);
    }

    // --------- get transformed rules ----------------

    public String getArtificialOpPrecRule()
    {
        var ruleST = recRuleTemplates.GetInstanceOf("recRule");
        ruleST.Add("ruleName", ruleName);
        var ruleArgST = codegenTemplates.GetInstanceOf("recRuleArg");
        ruleST.Add("argName", ruleArgST);
        var setResultST = codegenTemplates.GetInstanceOf("recRuleSetResultAction");
        ruleST.Add("setResultAction", setResultST);
        ruleST.Add("userRetvals", retvals);

        var opPrecRuleAlts =
            new Dictionary<int, LeftRecursiveRuleAltInfo>()
                .AddRange(binaryAlts)
                .AddRange(ternaryAlts)
                .AddRange(suffixAlts);

        foreach (int alt in opPrecRuleAlts.Keys)
        {
            LeftRecursiveRuleAltInfo altInfo = opPrecRuleAlts[alt];
            var altST = recRuleTemplates.GetInstanceOf("recRuleAlt");
            var predST = codegenTemplates.GetInstanceOf("recRuleAltPredicate");
            predST.Add("opPrec", precedence(alt));
            predST.Add("ruleName", ruleName);
            altST.Add("pred", predST);
            altST.Add("alt", altInfo);
            altST.Add("precOption", LeftRecursiveRuleTransformer.PRECEDENCE_OPTION_NAME);
            altST.Add("opPrec", precedence(alt));
            ruleST.Add("opAlts", altST);
        }

        ruleST.Add("primaryAlts", prefixAndOtherAlts);

        tool.Log("left-recursion", ruleST.Render());

        return ruleST.Render();
    }

    public AltAST addPrecedenceArgToRules(AltAST t, int prec)
    {
        if (t == null) return null;
        // get all top-level rule refs from ALT
        List<GrammarAST> outerAltRuleRefs = t.getNodesWithTypePreorderDFS(IntervalSet.Of(RULE_REF));
        foreach (GrammarAST x in outerAltRuleRefs)
        {
            RuleRefAST rref = (RuleRefAST)x;
            bool recursive = rref.getText().Equals(ruleName);
            bool rightmost = rref == outerAltRuleRefs[(outerAltRuleRefs.Count - 1)];
            if (recursive && rightmost)
            {
                GrammarAST dummyValueNode = new GrammarAST(new CommonToken(ANTLRParser.INT, "" + prec));
                rref.setOption(LeftRecursiveRuleTransformer.PRECEDENCE_OPTION_NAME, dummyValueNode);
            }
        }
        return t;
    }

    /**
	 * Match (RULE RULE_REF (BLOCK (ALT .*) (ALT RULE_REF[self] .*) (ALT .*)))
	 * Match (RULE RULE_REF (BLOCK (ALT .*) (ALT (ASSIGN ID RULE_REF[self]) .*) (ALT .*)))
	 */
    public static bool hasImmediateRecursiveRuleRefs(GrammarAST t, String ruleName)
    {
        if (t == null) return false;
        GrammarAST blk = (GrammarAST)t.GetFirstChildWithType(BLOCK);
        if (blk == null) return false;
        int n = blk.GetChildren().Count;
        for (int i = 0; i < n; i++)
        {
            GrammarAST alt = (GrammarAST)blk.GetChildren()[(i)];
            Tree first = alt.GetChild(0);
            if (first == null) continue;
            if (first.Type == ELEMENT_OPTIONS)
            {
                first = alt.GetChild(1);
                if (first == null)
                {
                    continue;
                }
            }
            if (first.Type == RULE_REF && first.Text.Equals(ruleName)) return true;
            Tree rref = first.GetChild(1);
            if (rref != null && rref.Type == RULE_REF && rref.Text.Equals(ruleName)) return true;
        }
        return false;
    }

    // TODO: this strips the tree properly, but since text()
    // uses the start of stop token index and gets text from that
    // ineffectively ignores this routine.
    public GrammarAST stripLeftRecursion(GrammarAST altAST)
    {
        GrammarAST lrlabel = null;
        GrammarAST first = (GrammarAST)altAST.GetChild(0);
        int leftRecurRuleIndex = 0;
        if (first.getType() == ELEMENT_OPTIONS)
        {
            first = (GrammarAST)altAST.GetChild(1);
            leftRecurRuleIndex = 1;
        }
        Tree rref = first.GetChild(1); // if label=rule
        if ((first.getType() == RULE_REF && first.getText().Equals(ruleName)) ||
             (rref != null && rref.Type == RULE_REF && rref.Text.Equals(ruleName)))
        {
            if (first.getType() == ASSIGN || first.getType() == PLUS_ASSIGN) lrlabel = (GrammarAST)first.GetChild(0);
            // remove rule ref (first child unless options present)
            altAST.DeleteChild(leftRecurRuleIndex);
            // reset index so it prints properly (sets token range of
            // ALT to start to right of left recur rule we deleted)
            GrammarAST newFirstChild = (GrammarAST)altAST.GetChild(leftRecurRuleIndex);
            altAST.setTokenStartIndex(newFirstChild.getTokenStartIndex());
        }
        return lrlabel;
    }

    /** Strip last 2 tokens if → label; alter indexes in altAST */
    public void stripAltLabel(GrammarAST altAST)
    {
        int start = altAST.getTokenStartIndex();
        int stop = altAST.getTokenStopIndex();
        // find =>
        for (int i = stop; i >= start; i--)
        {
            if (tokenStream.Get(i).Type == POUND)
            {
                altAST.setTokenStopIndex(i - 1);
                return;
            }
        }
    }

    public String text(GrammarAST t)
    {
        if (t == null) return "";

        int tokenStartIndex = t.getTokenStartIndex();
        int tokenStopIndex = t.getTokenStopIndex();

        // ignore tokens from existing option subtrees like:
        //    (ELEMENT_OPTIONS (= assoc right))
        //
        // element options are added back according to the values in the map
        // returned by getOptions().
        IntervalSet ignore = new IntervalSet();
        List<GrammarAST> optionsSubTrees = t.getNodesWithType(ELEMENT_OPTIONS);
        foreach (GrammarAST sub in optionsSubTrees)
        {
            ignore.Add(sub.getTokenStartIndex(), sub.getTokenStopIndex());
        }

        // Individual labels appear as RULE_REF or TOKEN_REF tokens in the tree,
        // but do not support the ELEMENT_OPTIONS syntax. Make sure to not try
        // and add the tokenIndex option when writing these tokens.
        IntervalSet noOptions = new IntervalSet();
        List<GrammarAST> labeledSubTrees = t.getNodesWithType(new IntervalSet(ASSIGN, PLUS_ASSIGN));
        foreach (GrammarAST sub in labeledSubTrees)
        {
            noOptions.Add(sub.GetChild(0).TokenStartIndex);
        }

        StringBuilder buf = new StringBuilder();
        int i = tokenStartIndex;
        while (i <= tokenStopIndex)
        {
            if (ignore.Contains(i))
            {
                i++;
                continue;
            }

            Token tok = tokenStream.Get(i);

            // Compute/hold any element options
            StringBuilder elementOptions = new StringBuilder();
            if (!noOptions.Contains(i))
            {
                GrammarAST node = t.getNodeWithTokenIndex(tok.TokenIndex);
                if (node != null &&
                     (tok.Type == TOKEN_REF ||
                      tok.                      Type == STRING_LITERAL ||
                      tok.                      Type == RULE_REF))
                {
                    elementOptions.Append("tokenIndex=").Append(tok.TokenIndex);
                }

                if (node is GrammarASTWithOptions)
                {
                    GrammarASTWithOptions o = (GrammarASTWithOptions)node;
                    foreach (var entry in o.getOptions())
                    {
                        if (elementOptions.Length > 0)
                        {
                            elementOptions.Append(',');
                        }

                        elementOptions.Append(entry.Key);
                        elementOptions.Append('=');
                        elementOptions.Append(entry.Value.getText());
                    }
                }
            }

            buf.Append(tok.Text); // add actual text of the current token to the rewritten alternative
            i++;                       // move to the next token

            // Are there args on a rule?
            if (tok.Type == RULE_REF && i <= tokenStopIndex && tokenStream.Get(i).Type == ARG_ACTION)
            {
                buf.Append('[' + tokenStream.Get(i).Text + ']');
                i++;
            }

            // now that we have the actual element, we can add the options.
            if (elementOptions.Length > 0)
            {
                buf.Append('<').Append(elementOptions).Append('>');
            }
        }
        return buf.ToString();
    }

    public int precedence(int alt)
    {
        return numAlts - alt + 1;
    }

    // Assumes left assoc
    public int nextPrecedence(int alt)
    {
        int p = precedence(alt);
        if (altAssociativity.TryGetValue(alt, out var r) && r == ASSOC.right) return p;
        return p + 1;
    }

    //@Override
    public String ToString()
    {
        return "PrecRuleOperatorCollector{" +
               "binaryAlts=" + binaryAlts +
               ", ternaryAlts=" + ternaryAlts +
               ", suffixAlts=" + suffixAlts +
               ", prefixAndOtherAlts=" + prefixAndOtherAlts +
               '}';
    }
}
