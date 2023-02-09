/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime;

namespace org.antlr.v4.tool.ast;


public class TerminalAST : GrammarASTWithOptions, RuleElementAST
{

    public TerminalAST(TerminalAST node) : base(node) { }

    public TerminalAST(Token t) : base(t) { }
    public TerminalAST(int type) : base(type) { }
    public TerminalAST(int type, Token t) : base(type, t) { }

    public override TerminalAST DupNode() => new (this);

    public override object Visit(GrammarASTVisitor v) => v.Visit(this);
}
