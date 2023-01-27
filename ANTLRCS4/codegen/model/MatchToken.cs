/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.codegen.model.decl;
using org.antlr.v4.tool.ast;

namespace org.antlr.v4.codegen.model;


/** */
public class MatchToken : RuleElement , LabeledOp {
	public readonly String name;
	public readonly String escapedName;
	public readonly int ttype;
	public readonly List<Decl> labels = new ();

	public MatchToken(OutputModelFactory factory, TerminalAST ast) {
		super(factory, ast);
		Grammar g = factory.getGrammar();
		CodeGenerator gen = factory.getGenerator();
		ttype = g.getTokenType(ast.getText());
		Target target = gen.getTarget();
		name = target.getTokenTypeAsTargetLabel(g, ttype);
		escapedName = target.escapeIfNeeded(name);
	}

	public MatchToken(OutputModelFactory factory, GrammarAST ast) {
		super(factory, ast);
		ttype = 0;
		name = null;
		escapedName = null;
	}

	//@Override
	public List<Decl> getLabels() { return labels; }
}
