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
    //@Override
    public virtual ParserFile ParserFile(String fileName) => null;

    //@Override
    public virtual Parser Parser(ParserFile file) => null;

    //@Override
    public virtual RuleFunction Rule(Rule r) => null;

    //@Override
    public virtual List<SrcOp> RulePostamble(RuleFunction function, Rule r) => null;

    //@Override
    public virtual LexerFile LexerFile(String fileName) => null;

    //@Override
    public virtual Lexer Lexer(LexerFile file) => null;

    // ALTERNATIVES / ELEMENTS

    //@Override
    public virtual CodeBlockForAlt Alternative(Alternative alt, bool outerMost) => null;

    //@Override
    public virtual CodeBlockForAlt FinishAlternative(CodeBlockForAlt blk, List<SrcOp> ops) { return blk; }

    //@Override
    public virtual CodeBlockForAlt Epsilon(Alternative alt, bool outerMost) => null;

    //@Override
    public virtual List<SrcOp> RuleRef(GrammarAST ID, GrammarAST label, GrammarAST args) => null;

    //@Override
    public virtual List<SrcOp> TokenRef(GrammarAST ID, GrammarAST label, GrammarAST args) => null;

    //@Override
    public virtual List<SrcOp> StringRef(GrammarAST ID, GrammarAST label) { return TokenRef(ID, label, null); }

    //@Override
    public virtual List<SrcOp> Set(GrammarAST setAST, GrammarAST label, bool invert) => null;

    //@Override
    public virtual List<SrcOp> Wildcard(GrammarAST ast, GrammarAST labelAST) => null;

    // ACTIONS

    //@Override
    public virtual List<SrcOp> Action(ActionAST ast) => null;

    //@Override
    public virtual List<SrcOp> Sempred(ActionAST ast) => null;

    // BLOCKS

    //@Override
    public virtual Choice GetChoiceBlock(BlockAST blkAST, List<CodeBlockForAlt> alts, GrammarAST label) => null;

    //@Override
    public virtual Choice GetEBNFBlock(GrammarAST ebnfRoot, List<CodeBlockForAlt> alts) => null;

    //@Override
    public virtual Choice GetLL1ChoiceBlock(BlockAST blkAST, List<CodeBlockForAlt> alts) => null;

    //@Override
    public virtual Choice GetComplexChoiceBlock(BlockAST blkAST, List<CodeBlockForAlt> alts) => null;

    //@Override
    public virtual Choice GetLL1EBNFBlock(GrammarAST ebnfRoot, List<CodeBlockForAlt> alts) => null;

    //@Override
    public virtual Choice GetComplexEBNFBlock(GrammarAST ebnfRoot, List<CodeBlockForAlt> alts) => null;

    //@Override
    public virtual List<SrcOp> GetLL1Test(IntervalSet look, GrammarAST blkAST) => null;

    //@Override
    public virtual bool NeedsImplicitLabel(GrammarAST ID, LabeledOp op) { return false; }

    public virtual Grammar GetGrammar() => null;

    public virtual CodeGenerator GetGenerator() => null;

    public virtual OutputModelController Controller
    {
        get;set;
    }

    public OutputModelObject GetRoot() => null;

    public RuleFunction GetCurrentRuleFunction() => null;

    public Alternative GetCurrentOuterMostAlt() => null;

    public CodeBlock GetCurrentBlock() => null;

    public CodeBlockForOuterMostAlt GetCurrentOuterMostAlternativeBlock() => null;

    public int GetCodeBlockLevel() => 0;

    public int GetTreeLevel() => 0;
}

