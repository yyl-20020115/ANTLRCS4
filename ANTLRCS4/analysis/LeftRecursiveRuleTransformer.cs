/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.runtime;
using org.antlr.v4.misc;
using org.antlr.v4.parse;
using org.antlr.v4.runtime;
using org.antlr.v4.runtime.misc;
using org.antlr.v4.semantics;
using org.antlr.v4.tool;
using org.antlr.v4.tool.ast;

namespace org.antlr.v4.analysis;

/** Remove left-recursive rule refs, add precedence args to recursive rule refs.
 *  Rewrite rule so we can create ATN.
 *
 *  MODIFIES grammar AST in place.
 */
public class LeftRecursiveRuleTransformer
{
    public static readonly string PRECEDENCE_OPTION_NAME = "p";
    public static readonly string TOKENINDEX_OPTION_NAME = "tokenIndex";

    public GrammarRootAST ast;
    public ICollection<Rule> rules;
    public Grammar g;
    public Tool tool;

    public LeftRecursiveRuleTransformer(GrammarRootAST ast, ICollection<Rule> rules, Grammar g)
    {
        this.ast = ast;
        this.rules = rules;
        this.g = g;
        this.tool = g.Tools;
    }

    public void TranslateLeftRecursiveRules()
    {
        var language = g.getLanguage();
        // translate all recursive rules
        List<string> leftRecursiveRuleNames = new();
        foreach (var r in rules)
        {
            if (!Grammar.isTokenName(r.name))
            {
                if (LeftRecursiveRuleAnalyzer.hasImmediateRecursiveRuleRefs(r.ast, r.name))
                {
                    bool fitsPattern = TranslateLeftRecursiveRule(ast, (LeftRecursiveRule)r, language);
                    if (fitsPattern)
                    {
                        leftRecursiveRuleNames.Add(r.name);
                    }
                    else
                    { // better given an error that non-conforming left-recursion exists
                        tool.ErrMgr.GrammarError(ErrorType.NONCONFORMING_LR_RULE, g.fileName, ((GrammarAST)r.ast.getChild(0)).token, r.name);
                    }
                }
            }
        }

        // update all refs to recursive rules to have [0] argument
        foreach (var r in ast.getNodesWithType(ANTLRParser.RULE_REF))
        {
            if (r.getParent().getType() == ANTLRParser.RULE) continue; // must be rule def
            if (((GrammarASTWithOptions)r).getOptionString(PRECEDENCE_OPTION_NAME) != null) continue; // already has arg; must be in rewritten rule
            if (leftRecursiveRuleNames.Contains(r.getText()))
            {
                // found ref to recursive rule not already rewritten with arg
                ((GrammarASTWithOptions)r).setOption(PRECEDENCE_OPTION_NAME, (GrammarAST)new GrammarASTAdaptor().create(ANTLRParser.INT, "0"));
            }
        }
    }

    /** Return true if successful */
    public bool TranslateLeftRecursiveRule(GrammarRootAST ast,
                                              LeftRecursiveRule r,
                                              String language)
    {
        //tool.log("grammar", ruleAST.toStringTree());
        var prevRuleAST = r.ast;
        var ruleName = prevRuleAST.getChild(0).getText();
        var leftRecursiveRuleWalker =
            new LeftRecursiveRuleAnalyzer(prevRuleAST, tool, ruleName, language);
        bool isLeftRec;
        try
        {
            //			Console.Out.WriteLine("TESTING ---------------\n"+
            //							   leftRecursiveRuleWalker.text(ruleAST));
            isLeftRec = leftRecursiveRuleWalker.rec_rule();
        }
        catch (RecognitionException re)
        {
            isLeftRec = false; // didn't match; oh well
        }
        if (!isLeftRec) return false;

        // replace old rule's AST; first create text of altered rule
        var RULES = (GrammarAST)ast.getFirstChildWithType(ANTLRParser.RULES);
        var newRuleText = leftRecursiveRuleWalker.getArtificialOpPrecRule();
        //		Console.Out.WriteLine("created: "+newRuleText);
        // now parse within the context of the grammar that originally created
        // the AST we are transforming. This could be an imported grammar so
        // we cannot just reference this.g because the role might come from
        // the imported grammar and not the root grammar (this.g)
        var t = ParseArtificialRule(prevRuleAST.g, newRuleText);

        // reuse the name token from the original AST since it refers to the proper source location in the original grammar
        ((GrammarAST)t.getChild(0)).token = ((GrammarAST)prevRuleAST.getChild(0)).getToken();

        // update grammar AST and set rule's AST.
        RULES.setChild(prevRuleAST.getChildIndex(), t);
        r.ast = t;

        // Reduce sets in newly created rule tree
        var transform = new GrammarTransformPipeline(g, g.Tools);
        transform.reduceBlocksToSets(r.ast);
        transform.expandParameterizedLoops(r.ast);

        // Rerun semantic checks on the new rule
        var ruleCollector = new RuleCollector(g);
        ruleCollector.visit(t, "rule");
        var basics = new BasicSemanticChecks(g, ruleCollector);
        // disable the assoc element option checks because they are already
        // handled for the pre-transformed rule.
        basics.checkAssocElementOption = false;
        basics.visit(t, "rule");

        // track recursive alt info for codegen
        r.recPrimaryAlts = new();
        r.recPrimaryAlts.AddRange(leftRecursiveRuleWalker.prefixAndOtherAlts);
        if (r.recPrimaryAlts.Count == 0)
        {
            tool.ErrMgr.GrammarError(ErrorType.NO_NON_LR_ALTS, g.fileName, ((GrammarAST)r.ast.getChild(0)).getToken(), r.name);
        }

        r.recOpAlts = new OrderedHashMap<int, LeftRecursiveRuleAltInfo>();
        r.recOpAlts.PutAll(leftRecursiveRuleWalker.binaryAlts);
        r.recOpAlts.PutAll(leftRecursiveRuleWalker.ternaryAlts);
        r.recOpAlts.PutAll(leftRecursiveRuleWalker.suffixAlts);

        // walk alt info records and set their altAST to point to appropriate ALT subtree
        // from freshly created AST
        SetAltASTPointers(r, t);

        // update Rule to just one alt and add prec alt
        var arg = (ActionAST)r.ast.getFirstChildWithType(ANTLRParser.ARG_ACTION);
        if (arg != null)
        {
            r.args = ScopeParser.parseTypedArgList(arg, arg.getText(), g);
            r.args.type = AttributeDict.DictType.ARG;
            r.args.ast = arg;
            arg.resolver = r.alt[1]; // todo: isn't this Rule or something?
        }

        // define labels on recursive rule refs we delete; they don't point to nodes of course
        // these are so $label in action translation works
        foreach (var pair in leftRecursiveRuleWalker.leftRecursiveRuleRefLabels)
        {
            var labelNode = pair.a;
            var labelOpNode = (GrammarAST)labelNode.getParent();
            var elementNode = (GrammarAST)labelOpNode.getChild(1);
            var lp = new LabelElementPair(g, labelNode, elementNode, labelOpNode.getType());
            r.alt[1].labelDefs.map(labelNode.getText(), lp);
        }
        // copy to rule from walker
        r.leftRecursiveRuleRefLabels = leftRecursiveRuleWalker.leftRecursiveRuleRefLabels;

        tool.Log("grammar", "added: " + t.toStringTree());
        return true;
    }

    public RuleAST ParseArtificialRule(Grammar g, String ruleText)
    {
        var lexer = new ANTLRLexer(new ANTLRStringStream(ruleText));
        var adaptor = new GrammarASTAdaptor(lexer.getCharStream());
        var tokens = new CommonTokenStream(lexer);
        lexer.tokens = tokens;
        var p = new ToolANTLRParser(tokens, tool);
        p.setTreeAdaptor(adaptor);
        Token ruleStart = null;
        try
        {
            var r = p.rule();
            var tree = (RuleAST)r.getTree();
            ruleStart = (Token)r.getStart();
            GrammarTransformPipeline.setGrammarPtr(g, tree);
            GrammarTransformPipeline.augmentTokensWithOriginalPosition(g, tree);
            return tree;
        }
        catch (Exception e)
        {
            tool.ErrMgr.toolError(ErrorType.INTERNAL_ERROR,
                                  e,
                                  ruleStart,
                                  "error parsing rule created during left-recursion detection: " + ruleText);
        }
        return null;
    }

    /**
	 * <pre>
	 * (RULE e int _p (returns int v)
	 * 	(BLOCK
	 * 	  (ALT
	 * 		(BLOCK
	 * 			(ALT INT {$v = $INT.int;})
	 * 			(ALT '(' (= x e) ')' {$v = $x.v;})
	 * 			(ALT ID))
	 * 		(* (BLOCK
	 *			(OPTIONS ...)
	 * 			(ALT {7 &gt;= $_p}? '*' (= b e) {$v = $a.v * $b.v;})
	 * 			(ALT {6 &gt;= $_p}? '+' (= b e) {$v = $a.v + $b.v;})
	 * 			(ALT {3 &gt;= $_p}? '++') (ALT {2 &gt;= $_p}? '--'))))))
	 * </pre>
	 */
    public static void SetAltASTPointers(LeftRecursiveRule r, RuleAST t)
    {
        //		Console.Out.WriteLine("RULE: "+t.toStringTree());
        var ruleBlk = (BlockAST)t.getFirstChildWithType(ANTLRParser.BLOCK);
        var mainAlt = (AltAST)ruleBlk.getChild(0);
        var primaryBlk = (BlockAST)mainAlt.getChild(0);
        var opsBlk = (BlockAST)mainAlt.getChild(1).getChild(0); // (* BLOCK ...)
        for (int i = 0; i < r.recPrimaryAlts.Count; i++)
        {
            var altInfo = r.recPrimaryAlts[(i)];
            altInfo.altAST = (AltAST)primaryBlk.getChild(i);
            altInfo.altAST.leftRecursiveAltInfo = altInfo;
            altInfo.originalAltAST.leftRecursiveAltInfo = altInfo;
            //			altInfo.originalAltAST.parent = altInfo.altAST.parent;
            //			Console.Out.WriteLine(altInfo.altAST.toStringTree());
        }
        for (int i = 0; i < r.recOpAlts.Count; i++)
        {
            var altInfo = r.recOpAlts.GetElement(i);
            altInfo.altAST = (AltAST)opsBlk.getChild(i);
            altInfo.altAST.leftRecursiveAltInfo = altInfo;
            altInfo.originalAltAST.leftRecursiveAltInfo = altInfo;
            //			altInfo.originalAltAST.parent = altInfo.altAST.parent;
            //			Console.Out.WriteLine(altInfo.altAST.toStringTree());
        }
    }

}
