/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.tool.ast;

namespace org.antlr.v4.codegen.model;

/** An optional block is just an alternative block where the last alternative
 *  is epsilon. The analysis takes care of adding to the empty alternative.
 *
 *  (A | B | C)?
 */
public class LL1OptionalBlock : LL1AltBlock
{
    public LL1OptionalBlock(OutputModelFactory factory, GrammarAST blkAST, List<CodeBlockForAlt> alts)
    : base(factory, blkAST, alts) { }
}
