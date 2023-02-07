/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.analysis;
using org.antlr.v4.codegen.model;
using org.antlr.v4.codegen.model.decl;
using org.antlr.v4.parse;
using org.antlr.v4.runtime.atn;
using org.antlr.v4.runtime.misc;
using org.antlr.v4.tool;
using org.antlr.v4.tool.ast;

namespace org.antlr.v4.codegen;


/** */
public class ParserFactory : DefaultOutputModelFactory
{
    public ParserFactory(CodeGenerator gen) : base(gen) { }

    //@Override
    public override ParserFile ParserFile(String fileName) => new ParserFile(this, fileName);

    //@Override
    public override Parser Parser(ParserFile file) => new Parser(this, file);

    //@Override
    public override RuleFunction Rule(Rule r) => r is LeftRecursiveRule rule ? new LeftRecursiveRuleFunction(this, rule) : new RuleFunction(this, r);

    //@Override
    public override CodeBlockForAlt Epsilon(Alternative alt, bool outerMost) => Alternative(alt, outerMost);

    //@Override
    public override CodeBlockForAlt Alternative(Alternative alt, bool outerMost) 
        => outerMost ? new CodeBlockForOuterMostAlt(this, alt) : new CodeBlockForAlt(this);

    //@Override
    public override CodeBlockForAlt FinishAlternative(CodeBlockForAlt blk, List<SrcOp> ops)
    {
        blk.ops = ops;
        return blk;
    }

    //@Override
    public override List<SrcOp> Action(ActionAST ast) => List(new model.Action(this, ast));

    //@Override
    public override List<SrcOp> Sempred(ActionAST ast) => List(new SemPred(this, ast));

    //@Override
    public override List<SrcOp> RuleRef(GrammarAST ID, GrammarAST label, GrammarAST args)
    {
        var invokeOp = new InvokeRule(this, ID, label);
        // If no manual label and action refs as token/rule not label, we need to define implicit label
        if (controller.NeedsImplicitLabel(ID, invokeOp)) DefineImplicitLabel(ID, invokeOp);
        var listLabelOp = GetAddToListOpIfListLabelPresent(invokeOp, label);
        return List(invokeOp, listLabelOp);
    }

    //@Override
    public override List<SrcOp> TokenRef(GrammarAST ID, GrammarAST labelAST, GrammarAST args)
    {
        var matchOp = new MatchToken(this, (TerminalAST)ID);
        if (labelAST != null)
        {
            var label = labelAST.getText();
            var rf = GetCurrentRuleFunction();
            if (labelAST.parent.getType() == ANTLRParser.PLUS_ASSIGN)
            {
                // add Token _X and List<Token> X decls
                DefineImplicitLabel(ID, matchOp); // adds _X
                var l = GetTokenListLabelDecl(label);
                rf.AddContextDecl(ID.getAltLabel(), l);
            }
            else
            {
                var d = GetTokenLabelDecl(label);
                matchOp.labels.Add(d);
                rf.AddContextDecl(ID.getAltLabel(), d);
            }

            //			Decl d = getTokenLabelDecl(label);
            //			((MatchToken)matchOp).labels.add(d);
            //			getCurrentRuleFunction().addContextDecl(ID.getAltLabel(), d);
            //			if ( labelAST.parent.getType() == ANTLRParser.PLUS_ASSIGN ) {
            //				TokenListDecl l = getTokenListLabelDecl(label);
            //				getCurrentRuleFunction().addContextDecl(ID.getAltLabel(), l);
            //			}
        }
        if (controller.NeedsImplicitLabel(ID, matchOp)) DefineImplicitLabel(ID, matchOp);
        var listLabelOp = GetAddToListOpIfListLabelPresent(matchOp, labelAST);
        return List(matchOp, listLabelOp);
    }

    public Decl GetTokenLabelDecl(string label) => new TokenDecl(this, label);

    public TokenListDecl GetTokenListLabelDecl(string label) => new (this, gen.Target.GetListLabel(label));

    //@Override
    public List<SrcOp> Set(GrammarAST setAST, GrammarAST labelAST, bool invert)
    {
        MatchSet matchOp;
        if (invert) matchOp = new MatchNotSet(this, setAST);
        else matchOp = new MatchSet(this, setAST);
        if (labelAST != null)
        {
            var label = labelAST.getText();
            var rf = GetCurrentRuleFunction();
            if (labelAST.parent.getType() == ANTLRParser.PLUS_ASSIGN)
            {
                DefineImplicitLabel(setAST, matchOp);
                var l = GetTokenListLabelDecl(label);
                rf.AddContextDecl(setAST.getAltLabel(), l);
            }
            else
            {
                var d = GetTokenLabelDecl(label);
                matchOp.labels.Add(d);
                rf.AddContextDecl(setAST.getAltLabel(), d);
            }
        }
        if (controller.NeedsImplicitLabel(setAST, matchOp)) DefineImplicitLabel(setAST, matchOp);
        var listLabelOp = GetAddToListOpIfListLabelPresent(matchOp, labelAST);
        return List(matchOp, listLabelOp);
    }

    //@Override
    public override List<SrcOp> Wildcard(GrammarAST ast, GrammarAST labelAST)
    {
        var wild = new Wildcard(this, ast);
        // TODO: dup with tokenRef
        if (labelAST != null)
        {
            var label = labelAST.getText();
            var d = GetTokenLabelDecl(label);
            wild.labels.Add(d);
            GetCurrentRuleFunction().AddContextDecl(ast.getAltLabel(), d);
            if (labelAST.parent.getType() == ANTLRParser.PLUS_ASSIGN)
            {
                var l = GetTokenListLabelDecl(label);
                GetCurrentRuleFunction().AddContextDecl(ast.getAltLabel(), l);
            }
        }
        if (controller.NeedsImplicitLabel(ast, wild)) DefineImplicitLabel(ast, wild);
        var listLabelOp = GetAddToListOpIfListLabelPresent(wild, labelAST);
        return List(wild, listLabelOp);
    }

    //@Override
    public override Choice GetChoiceBlock(BlockAST blkAST, List<CodeBlockForAlt> alts, GrammarAST labelAST)
    {
        int decision = ((DecisionState)blkAST.atnState).decision;
        Choice c;
        if (!g.Tools.force_atn && AnalysisPipeline.Disjoint(g.decisionLOOK[decision]))
        {
            c = GetLL1ChoiceBlock(blkAST, alts);
        }
        else
        {
            c = GetComplexChoiceBlock(blkAST, alts);
        }

        if (labelAST != null)
        { // for x=(...), define x or x_list
            var label = labelAST.getText();
            var d = GetTokenLabelDecl(label);
            c.label = d;
            GetCurrentRuleFunction().AddContextDecl(labelAST.getAltLabel(), d);
            if (labelAST.parent.getType() == ANTLRParser.PLUS_ASSIGN)
            {
                var listLabel = gen.Target.GetListLabel(label);
                var l = new TokenListDecl(this, listLabel);
                GetCurrentRuleFunction().AddContextDecl(labelAST.getAltLabel(), l);
            }
        }

        return c;
    }

    //@Override
    public override Choice GetEBNFBlock(GrammarAST ebnfRoot, List<CodeBlockForAlt> alts)
    {
        if (!g.Tools.force_atn)
        {
            int decision;
            if (ebnfRoot.getType() == ANTLRParser.POSITIVE_CLOSURE)
            {
                decision = ((PlusLoopbackState)ebnfRoot.atnState).decision;
            }
            else if (ebnfRoot.getType() == ANTLRParser.CLOSURE)
            {
                decision = ((StarLoopEntryState)ebnfRoot.atnState).decision;
            }
            else
            {
                decision = ((DecisionState)ebnfRoot.atnState).decision;
            }

            if (AnalysisPipeline.Disjoint(g.decisionLOOK[decision]))
            {
                return GetLL1EBNFBlock(ebnfRoot, alts);
            }
        }

        return GetComplexEBNFBlock(ebnfRoot, alts);
    }

    //@Override
    public virtual Choice SetLL1ChoiceBlock(BlockAST blkAST, List<CodeBlockForAlt> alts) => new LL1AltBlock(this, blkAST, alts);

    //@Override
    public override Choice GetComplexChoiceBlock(BlockAST blkAST, List<CodeBlockForAlt> alts) => new AltBlock(this, blkAST, alts);

    //@Override
    public override Choice GetLL1EBNFBlock(GrammarAST ebnfRoot, List<CodeBlockForAlt> alts)
    {
        int ebnf = 0;
        if (ebnfRoot != null) ebnf = ebnfRoot.getType();
        Choice c = null;
        switch (ebnf)
        {
            case ANTLRParser.OPTIONAL:
                if (alts.Count == 1) c = new LL1OptionalBlockSingleAlt(this, ebnfRoot, alts);
                else c = new LL1OptionalBlock(this, ebnfRoot, alts);
                break;
            case ANTLRParser.CLOSURE:
                if (alts.Count == 1) c = new LL1StarBlockSingleAlt(this, ebnfRoot, alts);
                else c = GetComplexEBNFBlock(ebnfRoot, alts);
                break;
            case ANTLRParser.POSITIVE_CLOSURE:
                if (alts.Count == 1) c = new LL1PlusBlockSingleAlt(this, ebnfRoot, alts);
                else c = GetComplexEBNFBlock(ebnfRoot, alts);
                break;
        }
        return c;
    }

    //@Override
    public override Choice GetComplexEBNFBlock(GrammarAST ebnfRoot, List<CodeBlockForAlt> alts)
    {
        int ebnf = 0;
        if (ebnfRoot != null) ebnf = ebnfRoot.getType();
        Choice c = null;
        switch (ebnf)
        {
            case ANTLRParser.OPTIONAL:
                c = new OptionalBlock(this, ebnfRoot, alts);
                break;
            case ANTLRParser.CLOSURE:
                c = new StarBlock(this, ebnfRoot, alts);
                break;
            case ANTLRParser.POSITIVE_CLOSURE:
                c = new PlusBlock(this, ebnfRoot, alts);
                break;
        }
        return c;
    }

    //@Override
    public override List<SrcOp> GetLL1Test(IntervalSet look, GrammarAST blkAST) => List(new TestSetInline(this, blkAST, look, gen.Target.GetInlineTestSetWordSize()));

    //@Override
    public override bool NeedsImplicitLabel(GrammarAST ID, LabeledOp op)
    {
        var currentOuterMostAlt = GetCurrentOuterMostAlt();
        var actionRefsAsToken = currentOuterMostAlt.tokenRefsInActions.ContainsKey(ID.getText());
        var actionRefsAsRule = currentOuterMostAlt.ruleRefsInActions.ContainsKey(ID.getText());
        return op.GetLabels().Count == 0 && (actionRefsAsToken || actionRefsAsRule);
    }

    // support

    public void DefineImplicitLabel(GrammarAST ast, LabeledOp op)
    {
        Decl d;
        if (ast.getType() == ANTLRParser.SET || ast.getType() == ANTLRParser.WILDCARD)
        {
            var implLabel =
                gen.Target.GetImplicitSetLabel((ast.token.TokenIndex.ToString()));
            d = GetTokenLabelDecl(implLabel);
            ((TokenDecl)d).isImplicit = true;
        }
        else if (ast.getType() == ANTLRParser.RULE_REF)
        { // a rule reference?
            var r = g.getRule(ast.getText());
            var implLabel = gen.Target.GetImplicitRuleLabel(ast.getText());
            var ctxName =
                gen.Target.GetRuleFunctionContextStructName(r);
            d = new RuleContextDecl(this, implLabel, ctxName);
            ((RuleContextDecl)d).isImplicit = true;
        }
        else
        {
            var implLabel = gen.Target.GetImplicitTokenLabel(ast.getText());
            d = GetTokenLabelDecl(implLabel);
            ((TokenDecl)d).isImplicit = true;
        }
        op.GetLabels().Add(d);
        // all labels must be in scope struct in case we exec action out of context
        GetCurrentRuleFunction().AddContextDecl(ast.getAltLabel(), d);
    }

    public AddToLabelList GetAddToListOpIfListLabelPresent(LabeledOp op, GrammarAST label)
    {
        AddToLabelList labelOp = null;
        if (label != null && label.parent.getType() == ANTLRParser.PLUS_ASSIGN)
        {
            var target = gen.Target;
            var listLabel = target.GetListLabel(label.getText());
            var listRuntimeName = target.EscapeIfNeeded(listLabel);
            labelOp = new AddToLabelList(this, listRuntimeName, op.GetLabels()[0]);
        }
        return labelOp;
    }

}
