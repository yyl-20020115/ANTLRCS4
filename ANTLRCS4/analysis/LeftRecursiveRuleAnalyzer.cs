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
    public enum ASSOC : uint
    {
        Left, 
        Right
    }

    public Tool tool;
    public string ruleName;
    public Dictionary<int, LeftRecursiveRuleAltInfo> binaryAlts = new();
    public Dictionary<int, LeftRecursiveRuleAltInfo> ternaryAlts = new();
    public Dictionary<int, LeftRecursiveRuleAltInfo> suffixAlts = new();
    public List<LeftRecursiveRuleAltInfo> prefixAndOtherAlts = new();

    /** Pointer to ID node of ^(= ID element) */
    public List<Pair<GrammarAST, string>> leftRecursiveRuleRefLabels =
        new();

    /** Tokens from which rule AST comes from */
    public readonly TokenStream tokenStream;

    public GrammarAST retvals;

    public readonly static TemplateGroup recRuleTemplates;
    public readonly TemplateGroup codegenTemplates;
    public readonly string language;

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
                                     Tool tool, string ruleName, string language)
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

    public override void SetReturnValues(GrammarAST t) => retvals = t;

    public override void SetAltAssoc(AltAST t, int alt)
    {
        ASSOC assoc = ASSOC.Left;
        if (t.Options != null)
        {
            var a = t.GetOptionString("assoc");
            if (a != null)
            {
                if (a.Equals(ASSOC.Right.ToString()))
                {
                    assoc = ASSOC.Right;
                }
                else if (a.Equals(ASSOC.Left.ToString()))
                {
                    assoc = ASSOC.Left;
                }
                else
                {
                    tool.ErrMgr.GrammarError(ErrorType.ILLEGAL_OPTION_VALUE, t.g.fileName, t.GetOptionAST("assoc").Token, "assoc", assoc);
                }
            }
        }

        if (altAssociativity.TryGetValue(alt, out var r) && r != assoc)
        {
            tool.ErrMgr.ToolError(ErrorType.INTERNAL_ERROR, "all operators of alt " + alt + " of left-recursive rule must have same associativity");
        }
        altAssociativity[alt] = assoc;

        //		Console.Out.WriteLine("setAltAssoc: op " + alt + ": " + t.getText()+", assoc="+assoc);
    }

    public override void BinaryAlt(AltAST originalAltTree, int alt)
    {
        var altTree = (AltAST)originalAltTree.DupTree();
        var altLabel = altTree.altLabel != null ? altTree.altLabel.Text : null;

        string label = null;
        bool isListLabel = false;
        var lrlabel = StripLeftRecursion(altTree);
        if (lrlabel != null)
        {
            label = lrlabel.Text;
            isListLabel = lrlabel.Parent.Type == PLUS_ASSIGN;
            leftRecursiveRuleRefLabels.Add(new Pair<GrammarAST, string>(lrlabel, altLabel));
        }

        StripAltLabel(altTree);

        // rewrite e to be e_[rec_arg]
        int nextPrec = NextPrecedence(alt);
        altTree = AddPrecedenceArgToRules(altTree, nextPrec);

        StripAltLabel(altTree);
        var altText = Text(altTree);
        altText = altText.Trim();
        var a =
            new LeftRecursiveRuleAltInfo(alt, altText, label, altLabel, isListLabel, originalAltTree);
        a.nextPrec = nextPrec;
        binaryAlts[alt] = a;
        //Console.Out.WriteLine("binaryAlt " + alt + ": " + altText + ", rewrite=" + rewriteText);
    }

    public virtual void PrefixAlt(AltAST originalAltTree, int alt)
    {
        var altTree = (AltAST)originalAltTree.DupTree();
        StripAltLabel(altTree);

        int nextPrec = Precedence(alt);
        // rewrite e to be e_[prec]
        altTree = AddPrecedenceArgToRules(altTree, nextPrec);
        var altText = Text(altTree);
        altText = altText.Trim();
        var altLabel = altTree.altLabel != null ? altTree.altLabel.Text : null;
        var a =
            new LeftRecursiveRuleAltInfo(alt, altText, null, altLabel, false, originalAltTree);
        a.nextPrec = nextPrec;
        prefixAndOtherAlts.Add(a);
        //Console.Out.WriteLine("prefixAlt " + alt + ": " + altText + ", rewrite=" + rewriteText);
    }

    public override void SuffixAlt(AltAST originalAltTree, int alt)
    {
        var altTree = (AltAST)originalAltTree.DupTree();
        var altLabel = altTree.altLabel != null ? altTree.altLabel.Text : null;

        string label = null;
        var isListLabel = false;
        var lrlabel = StripLeftRecursion(altTree);
        if (lrlabel != null)
        {
            label = lrlabel.Text;
            isListLabel = lrlabel.Parent.Type == PLUS_ASSIGN;
            leftRecursiveRuleRefLabels.Add(new Pair<GrammarAST, string>(lrlabel, altLabel));
        }
        StripAltLabel(altTree);
        var altText = Text(altTree);
        altText = altText.Trim();
        var a =
            new LeftRecursiveRuleAltInfo(alt, altText, label, altLabel, isListLabel, originalAltTree);
        suffixAlts[alt] = a;
        //		Console.Out.WriteLine("suffixAlt " + alt + ": " + altText + ", rewrite=" + rewriteText);
    }

    public override void OtherAlt(AltAST originalAltTree, int alt)
    {
        var altTree = (AltAST)originalAltTree.DupTree();
        StripAltLabel(altTree);
        var altText = Text(altTree);
        var altLabel = altTree.altLabel != null ? altTree.altLabel.Text : null;
        var a =
            new LeftRecursiveRuleAltInfo(alt, altText, null, altLabel, false, originalAltTree);
        // We keep other alts with prefix alts since they are all added to the start of the generated rule, and
        // we want to retain any prior ordering between them
        prefixAndOtherAlts.Add(a);
        //		Console.Out.WriteLine("otherAlt " + alt + ": " + altText);
    }

    // --------- get transformed rules ----------------

    public string GetArtificialOpPrecRule()
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
            var altInfo = opPrecRuleAlts[alt];
            var altST = recRuleTemplates.GetInstanceOf("recRuleAlt");
            var predST = codegenTemplates.GetInstanceOf("recRuleAltPredicate");
            predST.Add("opPrec", Precedence(alt));
            predST.Add("ruleName", ruleName);
            altST.Add("pred", predST);
            altST.Add("alt", altInfo);
            altST.Add("precOption", LeftRecursiveRuleTransformer.PRECEDENCE_OPTION_NAME);
            altST.Add("opPrec", Precedence(alt));
            ruleST.Add("opAlts", altST);
        }

        ruleST.Add("primaryAlts", prefixAndOtherAlts);

        tool.Log("left-recursion", ruleST.Render());

        return ruleST.Render();
    }

    public AltAST AddPrecedenceArgToRules(AltAST t, int prec)
    {
        if (t == null) return null;
        // get all top-level rule refs from ALT
        var outerAltRuleRefs = t.GetNodesWithTypePreorderDFS(IntervalSet.Of(RULE_REF));
        foreach (var rref in outerAltRuleRefs.Cast<RuleRefAST>())
        {
            bool recursive = rref.Text.Equals(ruleName);
            bool rightmost = rref == outerAltRuleRefs[(outerAltRuleRefs.Count - 1)];
            if (recursive && rightmost)
            {
                var dummyValueNode = new GrammarAST(new CommonToken(ANTLRParser.INT, "" + prec));
                rref.SetOption(LeftRecursiveRuleTransformer.PRECEDENCE_OPTION_NAME, dummyValueNode);
            }
        }
        return t;
    }

    /**
	 * Match (RULE RULE_REF (BLOCK (ALT .*) (ALT RULE_REF[self] .*) (ALT .*)))
	 * Match (RULE RULE_REF (BLOCK (ALT .*) (ALT (ASSIGN ID RULE_REF[self]) .*) (ALT .*)))
	 */
    public static bool HasImmediateRecursiveRuleRefs(GrammarAST t, string ruleName)
    {
        if (t == null) return false;
        var blk = (GrammarAST)t.GetFirstChildWithType(BLOCK);
        if (blk == null) return false;
        int n = blk.GetChildren().Count;
        for (int i = 0; i < n; i++)
        {
            var alt = (GrammarAST)blk.GetChildren()[(i)];
            var first = alt.GetChild(0);
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
    public GrammarAST StripLeftRecursion(GrammarAST altAST)
    {
        GrammarAST lrlabel = null;
        var first = (GrammarAST)altAST.GetChild(0);
        int leftRecurRuleIndex = 0;
        if (first.Type == ELEMENT_OPTIONS)
        {
            first = (GrammarAST)altAST.GetChild(1);
            leftRecurRuleIndex = 1;
        }
        var rref = first.GetChild(1); // if label=rule
        if ((first.Type == RULE_REF && first.Text.Equals(ruleName)) ||
             (rref != null && rref.Type == RULE_REF && rref.Text.Equals(ruleName)))
        {
            if (first.Type == ASSIGN || first.Type == PLUS_ASSIGN) lrlabel = (GrammarAST)first.GetChild(0);
            // remove rule ref (first child unless options present)
            altAST.DeleteChild(leftRecurRuleIndex);
            // reset index so it prints properly (sets token range of
            // ALT to start to right of left recur rule we deleted)
            var newFirstChild = (GrammarAST)altAST.GetChild(leftRecurRuleIndex);
            altAST.            TokenStartIndex = newFirstChild.TokenStartIndex;
        }
        return lrlabel;
    }

    /** Strip last 2 tokens if → label; alter indexes in altAST */
    public void StripAltLabel(GrammarAST altAST)
    {
        int start = altAST.TokenStartIndex;
        int stop = altAST.TokenStopIndex;
        // find =>
        for (int i = stop; i >= start; i--)
        {
            if (tokenStream.Get(i).Type == POUND)
            {
                altAST.                TokenStopIndex = i - 1;
                return;
            }
        }
    }

    public string Text(GrammarAST t)
    {
        if (t == null) return "";

        int tokenStartIndex = t.TokenStartIndex;
        int tokenStopIndex = t.TokenStopIndex;

        // ignore tokens from existing option subtrees like:
        //    (ELEMENT_OPTIONS (= assoc right))
        //
        // element options are added back according to the values in the map
        // returned by getOptions().
        var ignore = new IntervalSet();
        var optionsSubTrees = t.GetNodesWithType(ELEMENT_OPTIONS);
        foreach (var sub in optionsSubTrees)
        {
            ignore.Add(sub.TokenStartIndex, sub.TokenStopIndex);
        }

        // Individual labels appear as RULE_REF or TOKEN_REF tokens in the tree,
        // but do not support the ELEMENT_OPTIONS syntax. Make sure to not try
        // and add the tokenIndex option when writing these tokens.
        var noOptions = new IntervalSet();
        var labeledSubTrees = t.GetNodesWithType(new IntervalSet(ASSIGN, PLUS_ASSIGN));
        foreach (var sub in labeledSubTrees)
        {
            noOptions.Add(sub.GetChild(0).TokenStartIndex);
        }

        var buffer = new StringBuilder();
        int i = tokenStartIndex;
        while (i <= tokenStopIndex)
        {
            if (ignore.Contains(i))
            {
                i++;
                continue;
            }

            var tok = tokenStream.Get(i);

            // Compute/hold any element options
            var elementOptions = new StringBuilder();
            if (!noOptions.Contains(i))
            {
                var node = t.GetNodeWithTokenIndex(tok.TokenIndex);
                if (node != null &&
                     (tok.Type == TOKEN_REF ||
                      tok.                      Type == STRING_LITERAL ||
                      tok.                      Type == RULE_REF))
                {
                    elementOptions.Append("tokenIndex=").Append(tok.TokenIndex);
                }

                if (node is GrammarASTWithOptions o)
                {
                    foreach (var entry in o.Options)
                    {
                        if (elementOptions.Length > 0)
                        {
                            elementOptions.Append(',');
                        }

                        elementOptions.Append(entry.Key);
                        elementOptions.Append('=');
                        elementOptions.Append(entry.Value.Text);
                    }
                }
            }

            buffer.Append(tok.Text); // add actual text of the current token to the rewritten alternative
            i++;                       // move to the next token

            // Are there args on a rule?
            if (tok.Type == RULE_REF && i <= tokenStopIndex && tokenStream.Get(i).Type == ARG_ACTION)
            {
                buffer.Append('[' + tokenStream.Get(i).Text + ']');
                i++;
            }

            // now that we have the actual element, we can add the options.
            if (elementOptions.Length > 0)
            {
                buffer.Append('<').Append(elementOptions).Append('>');
            }
        }
        return buffer.ToString();
    }

    public int Precedence(int alt) => numAlts - alt + 1;

    // Assumes left assoc
    public int NextPrecedence(int alt)
    {
        int p = Precedence(alt);
        if (altAssociativity.TryGetValue(alt, out var r) && r == ASSOC.Right) return p;
        return p + 1;
    }

    public override string ToString() 
        => "PrecRuleOperatorCollector{" +
               "binaryAlts=" + binaryAlts +
               ", ternaryAlts=" + ternaryAlts +
               ", suffixAlts=" + suffixAlts +
               ", prefixAndOtherAlts=" + prefixAndOtherAlts +
               '}';
}
