/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.codegen.model;
using org.antlr.v4.codegen.model.decl;
using org.antlr.v4.runtime;
using org.antlr.v4.runtime.misc;
using org.antlr.v4.tool;
using org.antlr.v4.tool.ast;
using Lexer = org.antlr.v4.codegen.model.Lexer;
using Parser = org.antlr.v4.codegen.model.Parser;

namespace org.antlr.v4.codegen;

public interface OutputModelFactory {
	Grammar getGrammar();

	CodeGenerator getGenerator();

	void setController(OutputModelController controller);

	OutputModelController getController();

	ParserFile parserFile(String fileName);

	Parser parser(ParserFile file);

	LexerFile lexerFile(String fileName);

	Lexer lexer(LexerFile file);

	RuleFunction rule(Rule r);

	List<SrcOp> rulePostamble(RuleFunction function, Rule r);

	// ELEMENT TRIGGERS

	CodeBlockForAlt alternative(Alternative alt, bool outerMost);

	CodeBlockForAlt finishAlternative(CodeBlockForAlt blk, List<SrcOp> ops);

	CodeBlockForAlt epsilon(Alternative alt, bool outerMost);

	List<SrcOp> ruleRef(GrammarAST ID, GrammarAST label, GrammarAST args);

	List<SrcOp> tokenRef(GrammarAST ID, GrammarAST label, GrammarAST args);

	List<SrcOp> stringRef(GrammarAST ID, GrammarAST label);

	List<SrcOp> set(GrammarAST setAST, GrammarAST label, bool invert);

	List<SrcOp> wildcard(GrammarAST ast, GrammarAST labelAST);

	List<SrcOp> action(ActionAST ast);

	List<SrcOp> sempred(ActionAST ast);

	Choice getChoiceBlock(BlockAST blkAST, List<CodeBlockForAlt> alts, GrammarAST label);

	Choice getEBNFBlock(GrammarAST ebnfRoot, List<CodeBlockForAlt> alts);

	Choice getLL1ChoiceBlock(BlockAST blkAST, List<CodeBlockForAlt> alts);

	Choice getComplexChoiceBlock(BlockAST blkAST, List<CodeBlockForAlt> alts);

	Choice getLL1EBNFBlock(GrammarAST ebnfRoot, List<CodeBlockForAlt> alts);

	Choice getComplexEBNFBlock(GrammarAST ebnfRoot, List<CodeBlockForAlt> alts);

	List<SrcOp> getLL1Test(IntervalSet look, GrammarAST blkAST);

	bool needsImplicitLabel(GrammarAST ID, LabeledOp op);

	// CONTEXT INFO

	OutputModelObject getRoot();

	RuleFunction getCurrentRuleFunction();

	Alternative getCurrentOuterMostAlt();

	CodeBlock getCurrentBlock();

	CodeBlockForOuterMostAlt getCurrentOuterMostAlternativeBlock();

	int getCodeBlockLevel();

	int getTreeLevel();

}
