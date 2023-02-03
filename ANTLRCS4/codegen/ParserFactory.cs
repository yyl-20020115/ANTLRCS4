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
public class ParserFactory : DefaultOutputModelFactory {
	public ParserFactory(CodeGenerator gen): base(gen) {  }

	//@Override
	public ParserFile parserFile(String fileName) {
		return new ParserFile(this, fileName);
	}

	//@Override
	public Parser parser(ParserFile file) {
		return new Parser(this, file);
	}

	//@Override
	public RuleFunction rule(Rule r) {
		if ( r is LeftRecursiveRule ) {
			return new LeftRecursiveRuleFunction(this, (LeftRecursiveRule)r);
		}
		else {
			RuleFunction rf = new RuleFunction(this, r);
			return rf;
		}
	}

	//@Override
	public CodeBlockForAlt epsilon(Alternative alt, bool outerMost) {
		return alternative(alt, outerMost);
	}

	//@Override
	public CodeBlockForAlt alternative(Alternative alt, bool outerMost) {
		if ( outerMost ) return new CodeBlockForOuterMostAlt(this, alt);
		return new CodeBlockForAlt(this);
	}

	//@Override
	public CodeBlockForAlt finishAlternative(CodeBlockForAlt blk, List<SrcOp> ops) {
		blk.ops = ops;
		return blk;
	}

	//@Override
	public List<SrcOp> action(ActionAST ast) { return List(new model.Action(this, ast)); }

	//@Override
	public List<SrcOp> sempred(ActionAST ast) { return List(new SemPred(this, ast)); }

	//@Override
	public List<SrcOp> ruleRef(GrammarAST ID, GrammarAST label, GrammarAST args) {
		InvokeRule invokeOp = new InvokeRule(this, ID, label);
		// If no manual label and action refs as token/rule not label, we need to define implicit label
		if ( controller.NeedsImplicitLabel(ID, invokeOp) ) defineImplicitLabel(ID, invokeOp);
		AddToLabelList listLabelOp = getAddToListOpIfListLabelPresent(invokeOp, label);
		return List(invokeOp, listLabelOp);
	}

	//@Override
	public List<SrcOp> tokenRef(GrammarAST ID, GrammarAST labelAST, GrammarAST args) {
		MatchToken matchOp = new MatchToken(this, (TerminalAST) ID);
		if ( labelAST!=null ) {
			String label = labelAST.getText();
			RuleFunction rf = getCurrentRuleFunction();
			if ( labelAST.parent.getType() == ANTLRParser.PLUS_ASSIGN ) {
				// add Token _X and List<Token> X decls
				defineImplicitLabel(ID, matchOp); // adds _X
				TokenListDecl l = getTokenListLabelDecl(label);
				rf.AddContextDecl(ID.getAltLabel(), l);
			}
			else {
				Decl d = getTokenLabelDecl(label);
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
		if ( controller.NeedsImplicitLabel(ID, matchOp) ) defineImplicitLabel(ID, matchOp);
		AddToLabelList listLabelOp = getAddToListOpIfListLabelPresent(matchOp, labelAST);
		return List(matchOp, listLabelOp);
	}

	public Decl getTokenLabelDecl(String label) {
		return new TokenDecl(this, label);
	}

	public TokenListDecl getTokenListLabelDecl(String label) {
		return new TokenListDecl(this, gen.Target.GetListLabel(label));
	}

	//@Override
	public List<SrcOp> set(GrammarAST setAST, GrammarAST labelAST, bool invert) {
		MatchSet matchOp;
		if ( invert ) matchOp = new MatchNotSet(this, setAST);
		else matchOp = new MatchSet(this, setAST);
		if ( labelAST!=null ) {
			String label = labelAST.getText();
			RuleFunction rf = getCurrentRuleFunction();
			if ( labelAST.parent.getType() == ANTLRParser.PLUS_ASSIGN ) {
				defineImplicitLabel(setAST, matchOp);
				TokenListDecl l = getTokenListLabelDecl(label);
				rf.AddContextDecl(setAST.getAltLabel(), l);
			}
			else {
				Decl d = getTokenLabelDecl(label);
				matchOp.labels.Add(d);
				rf.AddContextDecl(setAST.getAltLabel(), d);
			}
		}
		if ( controller.NeedsImplicitLabel(setAST, matchOp) ) defineImplicitLabel(setAST, matchOp);
		AddToLabelList listLabelOp = getAddToListOpIfListLabelPresent(matchOp, labelAST);
		return List(matchOp, listLabelOp);
	}

	//@Override
	public List<SrcOp> wildcard(GrammarAST ast, GrammarAST labelAST) {
		Wildcard wild = new Wildcard(this, ast);
		// TODO: dup with tokenRef
		if ( labelAST!=null ) {
			String label = labelAST.getText();
			Decl d = getTokenLabelDecl(label);
			wild.labels.Add(d);
			getCurrentRuleFunction().AddContextDecl(ast.getAltLabel(), d);
			if ( labelAST.parent.getType() == ANTLRParser.PLUS_ASSIGN ) {
				TokenListDecl l = getTokenListLabelDecl(label);
				getCurrentRuleFunction().AddContextDecl(ast.getAltLabel(), l);
			}
		}
		if ( controller.NeedsImplicitLabel(ast, wild) ) defineImplicitLabel(ast, wild);
		AddToLabelList listLabelOp = getAddToListOpIfListLabelPresent(wild, labelAST);
		return List(wild, listLabelOp);
	}

	//@Override
	public Choice getChoiceBlock(BlockAST blkAST, List<CodeBlockForAlt> alts, GrammarAST labelAST) {
		int decision = ((DecisionState)blkAST.atnState).decision;
		Choice c;
		if ( !g.Tools.force_atn && AnalysisPipeline.Disjoint(g.decisionLOOK[decision]) ) {
			c = getLL1ChoiceBlock(blkAST, alts);
		}
		else {
			c = getComplexChoiceBlock(blkAST, alts);
		}

		if ( labelAST!=null ) { // for x=(...), define x or x_list
			String label = labelAST.getText();
			Decl d = getTokenLabelDecl(label);
			c.label = d;
			getCurrentRuleFunction().AddContextDecl(labelAST.getAltLabel(), d);
			if ( labelAST.parent.getType() == ANTLRParser.PLUS_ASSIGN  ) {
				String listLabel = gen.Target.GetListLabel(label);
				TokenListDecl l = new TokenListDecl(this, listLabel);
				getCurrentRuleFunction().AddContextDecl(labelAST.getAltLabel(), l);
			}
		}

		return c;
	}

	//@Override
	public Choice getEBNFBlock(GrammarAST ebnfRoot, List<CodeBlockForAlt> alts) {
		if (!g.Tools.force_atn) {
			int decision;
			if ( ebnfRoot.getType()==ANTLRParser.POSITIVE_CLOSURE ) {
				decision = ((PlusLoopbackState)ebnfRoot.atnState).decision;
			}
			else if ( ebnfRoot.getType()==ANTLRParser.CLOSURE ) {
				decision = ((StarLoopEntryState)ebnfRoot.atnState).decision;
			}
			else {
				decision = ((DecisionState)ebnfRoot.atnState).decision;
			}

			if ( AnalysisPipeline.Disjoint(g.decisionLOOK[decision]) ) {
				return getLL1EBNFBlock(ebnfRoot, alts);
			}
		}

		return getComplexEBNFBlock(ebnfRoot, alts);
	}

	//@Override
	public Choice getLL1ChoiceBlock(BlockAST blkAST, List<CodeBlockForAlt> alts) {
		return new LL1AltBlock(this, blkAST, alts);
	}

	//@Override
	public Choice getComplexChoiceBlock(BlockAST blkAST, List<CodeBlockForAlt> alts) {
		return new AltBlock(this, blkAST, alts);
	}

	//@Override
	public Choice getLL1EBNFBlock(GrammarAST ebnfRoot, List<CodeBlockForAlt> alts) {
		int ebnf = 0;
		if ( ebnfRoot!=null ) ebnf = ebnfRoot.getType();
		Choice c = null;
		switch ( ebnf ) {
			case ANTLRParser.OPTIONAL :
				if ( alts.Count==1 ) c = new LL1OptionalBlockSingleAlt(this, ebnfRoot, alts);
				else c = new LL1OptionalBlock(this, ebnfRoot, alts);
				break;
			case ANTLRParser.CLOSURE :
				if ( alts.Count==1 ) c = new LL1StarBlockSingleAlt(this, ebnfRoot, alts);
				else c = getComplexEBNFBlock(ebnfRoot, alts);
				break;
			case ANTLRParser.POSITIVE_CLOSURE :
				if ( alts.Count ==1 ) c = new LL1PlusBlockSingleAlt(this, ebnfRoot, alts);
				else c = getComplexEBNFBlock(ebnfRoot, alts);
				break;
		}
		return c;
	}

	//@Override
	public Choice getComplexEBNFBlock(GrammarAST ebnfRoot, List<CodeBlockForAlt> alts) {
		int ebnf = 0;
		if ( ebnfRoot!=null ) ebnf = ebnfRoot.getType();
		Choice c = null;
		switch ( ebnf ) {
			case ANTLRParser.OPTIONAL :
				c = new OptionalBlock(this, ebnfRoot, alts);
				break;
			case ANTLRParser.CLOSURE :
				c = new StarBlock(this, ebnfRoot, alts);
				break;
			case ANTLRParser.POSITIVE_CLOSURE :
				c = new PlusBlock(this, ebnfRoot, alts);
				break;
		}
		return c;
	}

	//@Override
	public List<SrcOp> getLL1Test(IntervalSet look, GrammarAST blkAST) {
		return List(new TestSetInline(this, blkAST, look, gen.Target.GetInlineTestSetWordSize()));
	}

	//@Override
	public bool needsImplicitLabel(GrammarAST ID, LabeledOp op) {
		Alternative currentOuterMostAlt = getCurrentOuterMostAlt();
		bool actionRefsAsToken = currentOuterMostAlt.tokenRefsInActions.ContainsKey(ID.getText());
		bool actionRefsAsRule = currentOuterMostAlt.ruleRefsInActions.ContainsKey(ID.getText());
		return	op.GetLabels().Count==0 &&	(actionRefsAsToken || actionRefsAsRule);
	}

	// support

	public void defineImplicitLabel(GrammarAST ast, LabeledOp op) {
		Decl d;
		if ( ast.getType()==ANTLRParser.SET || ast.getType()==ANTLRParser.WILDCARD ) {
			String implLabel =
				gen.				Target.GetImplicitSetLabel((ast.token.getTokenIndex().ToString()));
			d = getTokenLabelDecl(implLabel);
			((TokenDecl)d).isImplicit = true;
		}
		else if ( ast.getType()==ANTLRParser.RULE_REF ) { // a rule reference?
			Rule r = g.getRule(ast.getText());
			String implLabel = gen.Target.GetImplicitRuleLabel(ast.getText());
			String ctxName =
				gen.				Target.GetRuleFunctionContextStructName(r);
			d = new RuleContextDecl(this, implLabel, ctxName);
			((RuleContextDecl)d).isImplicit = true;
		}
		else {
			String implLabel = gen.Target.GetImplicitTokenLabel(ast.getText());
			d = getTokenLabelDecl(implLabel);
			((TokenDecl)d).isImplicit = true;
		}
		op.GetLabels().Add(d);
		// all labels must be in scope struct in case we exec action out of context
		getCurrentRuleFunction().AddContextDecl(ast.getAltLabel(), d);
	}

	public AddToLabelList getAddToListOpIfListLabelPresent(LabeledOp op, GrammarAST label) {
		AddToLabelList labelOp = null;
		if ( label!=null && label.parent.getType()==ANTLRParser.PLUS_ASSIGN ) {
			Target target = gen.Target;
			String listLabel = target.GetListLabel(label.getText());
			String listRuntimeName = target.EscapeIfNeeded(listLabel);
			labelOp = new AddToLabelList(this, listRuntimeName, op.GetLabels()[0]);
		}
		return labelOp;
	}

}
