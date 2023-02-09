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
using Lexer = org.antlr.v4.codegen.model.Lexer;
using Parser = org.antlr.v4.codegen.model.Parser;

namespace org.antlr.v4.codegen;

public interface OutputModelFactory
{
    Grammar Grammar { get; }

    CodeGenerator Generator { get; }
    OutputModelController Controller { get; set; }

    ParserFile ParserFile(string fileName);

    Parser Parser(ParserFile file);

    LexerFile LexerFile(string fileName);

    Lexer Lexer(LexerFile file);

    RuleFunction Rule(Rule r);

    List<SrcOp> RulePostamble(RuleFunction function, Rule r);

    // ELEMENT TRIGGERS

    CodeBlockForAlt Alternative(Alternative alt, bool outerMost);

    CodeBlockForAlt FinishAlternative(CodeBlockForAlt blk, List<SrcOp> ops);

    CodeBlockForAlt Epsilon(Alternative alt, bool outerMost);

    List<SrcOp> RuleRef(GrammarAST ID, GrammarAST label, GrammarAST args);

    List<SrcOp> TokenRef(GrammarAST ID, GrammarAST label, GrammarAST args);

    List<SrcOp> StringRef(GrammarAST ID, GrammarAST label);

    List<SrcOp> Set(GrammarAST setAST, GrammarAST label, bool invert);

    List<SrcOp> Wildcard(GrammarAST ast, GrammarAST labelAST);

    List<SrcOp> Action(ActionAST ast);

    List<SrcOp> Sempred(ActionAST ast);

    Choice GetChoiceBlock(BlockAST blkAST, List<CodeBlockForAlt> alts, GrammarAST label);

    Choice GetEBNFBlock(GrammarAST ebnfRoot, List<CodeBlockForAlt> alts);

    Choice GetLL1ChoiceBlock(BlockAST blkAST, List<CodeBlockForAlt> alts);

    Choice GetComplexChoiceBlock(BlockAST blkAST, List<CodeBlockForAlt> alts);

    Choice GetLL1EBNFBlock(GrammarAST ebnfRoot, List<CodeBlockForAlt> alts);

    Choice GetComplexEBNFBlock(GrammarAST ebnfRoot, List<CodeBlockForAlt> alts);

    List<SrcOp> GetLL1Test(IntervalSet look, GrammarAST blkAST);

    bool NeedsImplicitLabel(GrammarAST ID, LabeledOp op);

    // CONTEXT INFO

    OutputModelObject Root { get; }
    RuleFunction CurrentRuleFunction { get; }
    Alternative CurrentOuterMostAlt { get; }
    CodeBlock CurrentBlock { get; }
    CodeBlockForOuterMostAlt CurrentOuterMostAlternativeBlock { get; }
    int CodeBlockLevel { get; }

    int TreeLevel { get; }
}
