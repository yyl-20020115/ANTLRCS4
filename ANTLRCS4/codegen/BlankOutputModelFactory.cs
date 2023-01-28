/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.codegen.model;
using org.antlr.v4.codegen.model.decl;
using org.antlr.v4.runtime.misc;
using org.antlr.v4.tool;
using org.antlr.v4.tool.ast;

namespace org.antlr.v4.codegen;


public abstract class BlankOutputModelFactory : OutputModelFactory {
	//@Override
	public ParserFile parserFile(String fileName) { return null; }

	//@Override
	public Parser parser(ParserFile file) { return null; }

	//@Override
	public RuleFunction rule(Rule r) { return null; }

	//@Override
	public List<SrcOp> rulePostamble(RuleFunction function, Rule r) { return null; }

	//@Override
	public LexerFile lexerFile(String fileName) { return null; }

	//@Override
	public Lexer lexer(LexerFile file) { return null; }

	// ALTERNATIVES / ELEMENTS

	//@Override
	public CodeBlockForAlt alternative(Alternative alt, bool outerMost) { return null; }

	//@Override
	public CodeBlockForAlt finishAlternative(CodeBlockForAlt blk, List<SrcOp> ops) { return blk; }

	//@Override
	public CodeBlockForAlt epsilon(Alternative alt, bool outerMost) { return null; }

	//@Override
	public List<SrcOp> ruleRef(GrammarAST ID, GrammarAST label, GrammarAST args) { return null; }

	//@Override
	public List<SrcOp> tokenRef(GrammarAST ID, GrammarAST label, GrammarAST args) { return null; }

	//@Override
	public List<SrcOp> stringRef(GrammarAST ID, GrammarAST label) { return tokenRef(ID, label, null); }

	//@Override
	public List<SrcOp> set(GrammarAST setAST, GrammarAST label, bool invert) {	return null; }

	//@Override
	public List<SrcOp> wildcard(GrammarAST ast, GrammarAST labelAST) { return null; }

	// ACTIONS

	//@Override
	public List<SrcOp> action(ActionAST ast) { return null; }

	//@Override
	public List<SrcOp> sempred(ActionAST ast) { return null; }

	// BLOCKS

	//@Override
	public Choice getChoiceBlock(BlockAST blkAST, List<CodeBlockForAlt> alts, GrammarAST label) { return null; }

	//@Override
	public Choice getEBNFBlock(GrammarAST ebnfRoot, List<CodeBlockForAlt> alts) { return null; }

	//@Override
	public Choice getLL1ChoiceBlock(BlockAST blkAST, List<CodeBlockForAlt> alts) { return null; }

	//@Override
	public Choice getComplexChoiceBlock(BlockAST blkAST, List<CodeBlockForAlt> alts) { return null; }

	//@Override
	public Choice getLL1EBNFBlock(GrammarAST ebnfRoot, List<CodeBlockForAlt> alts) { return null; }

	//@Override
	public Choice getComplexEBNFBlock(GrammarAST ebnfRoot, List<CodeBlockForAlt> alts) { return null; }

	//@Override
	public List<SrcOp> getLL1Test(IntervalSet look, GrammarAST blkAST) { return null; }

	//@Override
	public bool needsImplicitLabel(GrammarAST ID, LabeledOp op) { return false; }

    public Grammar getGrammar()
    {
        throw new NotImplementedException();
    }

    public CodeGenerator getGenerator()
    {
        throw new NotImplementedException();
    }

    public void setController(OutputModelController controller)
    {
        throw new NotImplementedException();
    }

    public OutputModelController getController()
    {
        throw new NotImplementedException();
    }

    

    public OutputModelObject getRoot()
    {
        throw new NotImplementedException();
    }

    public RuleFunction getCurrentRuleFunction()
    {
        throw new NotImplementedException();
    }

    public Alternative getCurrentOuterMostAlt()
    {
        throw new NotImplementedException();
    }

    public CodeBlock getCurrentBlock()
    {
        throw new NotImplementedException();
    }

    public CodeBlockForOuterMostAlt getCurrentOuterMostAlternativeBlock()
    {
        throw new NotImplementedException();
    }

    public int getCodeBlockLevel()
    {
        throw new NotImplementedException();
    }

    public int getTreeLevel()
    {
        throw new NotImplementedException();
    }
}

