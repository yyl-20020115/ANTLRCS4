/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime;

namespace org.antlr.v4.tool.ast;

public class BlockAST : GrammarASTWithOptions , RuleElementAST {
    // TODO: maybe I need a Subrule object like Rule so these options mov to that?
    /** What are the default options for a subrule? */
    public static readonly Dictionary<String, String> defaultBlockOptions =
            new ();

    public static readonly Dictionary<String, String> defaultLexerBlockOptions =
            new ();

	public BlockAST(BlockAST node) {
		super(node);
	}

	public BlockAST(Token t) { super(t); }
    public BlockAST(int type) { super(type); }
    public BlockAST(int type, Token t) { super(type, t); }
	public BlockAST(int type, Token t, String text) { super(type,t,text); }

	//@Override
	public BlockAST dupNode() { return new BlockAST(this); }

	//@Override
	public Object visit(GrammarASTVisitor v) { return v.visit(this); }
}
