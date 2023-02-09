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


public abstract class BlankOutputModelFactory : OutputModelFactory
{
    public virtual ParserFile ParserFile(string fileName) => null;

    public virtual Parser Parser(ParserFile file) => null;

    public virtual RuleFunction Rule(Rule r) => null;

    public virtual List<SrcOp> RulePostamble(RuleFunction function, Rule r) => null;

    public virtual LexerFile LexerFile(string fileName) => null;

    public virtual Lexer Lexer(LexerFile file) => null;

    // ALTERNATIVES / ELEMENTS

    public virtual CodeBlockForAlt Alternative(Alternative alt, bool outerMost) => null;

    public virtual CodeBlockForAlt FinishAlternative(CodeBlockForAlt blk, List<SrcOp> ops) { return blk; }

    public virtual CodeBlockForAlt Epsilon(Alternative alt, bool outerMost) => null;

    public virtual List<SrcOp> RuleRef(GrammarAST ID, GrammarAST label, GrammarAST args) => null;

    public virtual List<SrcOp> TokenRef(GrammarAST ID, GrammarAST label, GrammarAST args) => null;

    public virtual List<SrcOp> StringRef(GrammarAST ID, GrammarAST label) { return TokenRef(ID, label, null); }

    public virtual List<SrcOp> Set(GrammarAST setAST, GrammarAST label, bool invert) => null;

    public virtual List<SrcOp> Wildcard(GrammarAST ast, GrammarAST labelAST) => null;

    // ACTIONS

    public virtual List<SrcOp> Action(ActionAST ast) => null;

    public virtual List<SrcOp> Sempred(ActionAST ast) => null;

    // BLOCKS

    public virtual Choice GetChoiceBlock(BlockAST blkAST, List<CodeBlockForAlt> alts, GrammarAST label) => null;

    public virtual Choice GetEBNFBlock(GrammarAST ebnfRoot, List<CodeBlockForAlt> alts) => null;

    public virtual Choice GetLL1ChoiceBlock(BlockAST blkAST, List<CodeBlockForAlt> alts) => null;

    public virtual Choice GetComplexChoiceBlock(BlockAST blkAST, List<CodeBlockForAlt> alts) => null;

    public virtual Choice GetLL1EBNFBlock(GrammarAST ebnfRoot, List<CodeBlockForAlt> alts) => null;

    public virtual Choice GetComplexEBNFBlock(GrammarAST ebnfRoot, List<CodeBlockForAlt> alts) => null;

    public virtual List<SrcOp> GetLL1Test(IntervalSet look, GrammarAST blkAST) => null;

    public virtual bool NeedsImplicitLabel(GrammarAST ID, LabeledOp op) { return false; }

    public virtual Grammar Grammar => null;

    public virtual CodeGenerator Generator => null;

    public virtual OutputModelController Controller
    {
        get;set;
    }

    public virtual OutputModelObject Root => null;

    public virtual RuleFunction CurrentRuleFunction => null;

    public virtual Alternative CurrentOuterMostAlt => null;

    public virtual CodeBlock CurrentBlock => null;

    public virtual CodeBlockForOuterMostAlt CurrentOuterMostAlternativeBlock => null;

    public virtual int CodeBlockLevel => 0;

    public virtual int TreeLevel => 0;
}

