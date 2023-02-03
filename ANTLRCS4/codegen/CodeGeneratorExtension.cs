/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.codegen.model;
using org.antlr.v4.tool.ast;

namespace org.antlr.v4.codegen;


/** Filter list of SrcOps and return; default is pass-through filter */
public class CodeGeneratorExtension
{
    public OutputModelFactory factory;

    public CodeGeneratorExtension(OutputModelFactory factory)
    {
        this.factory = factory;
    }

    public ParserFile ParserFile(ParserFile f) { return f; }

    public Parser Parser(Parser p) { return p; }

    public LexerFile LexerFile(LexerFile f) { return f; }

    public Lexer Lexer(Lexer l) { return l; }

    public RuleFunction Rule(RuleFunction rf) { return rf; }

    public List<SrcOp> RulePostamble(List<SrcOp> ops) { return ops; }

    public CodeBlockForAlt Alternative(CodeBlockForAlt blk, bool outerMost) { return blk; }

    public CodeBlockForAlt FinishAlternative(CodeBlockForAlt blk, bool outerMost) { return blk; }

    public CodeBlockForAlt Epsilon(CodeBlockForAlt blk) { return blk; }

    public List<SrcOp> RuleRef(List<SrcOp> ops) { return ops; }

    public List<SrcOp> TokenRef(List<SrcOp> ops) { return ops; }

    public List<SrcOp> Set(List<SrcOp> ops) { return ops; }

    public List<SrcOp> StringRef(List<SrcOp> ops) { return ops; }

    public List<SrcOp> Wildcard(List<SrcOp> ops) { return ops; }

    // ACTIONS

    public List<SrcOp> Action(List<SrcOp> ops) { return ops; }

    public List<SrcOp> Sempred(List<SrcOp> ops) { return ops; }

    // BLOCKS

    public Choice GetChoiceBlock(Choice c) { return c; }

    public Choice GetEBNFBlock(Choice c) { return c; }

    public bool NeedsImplicitLabel(GrammarAST ID, LabeledOp op) { return false; }
}
