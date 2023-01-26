/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.codegen.model.chunk;
using org.antlr.v4.codegen.model.decl;
using org.antlr.v4.runtime;
using org.antlr.v4.tool.ast;

namespace org.antlr.v4.codegen.model;



/** */
public class Action : RuleElement {
	//@ModelElement
	public List<ActionChunk> chunks;

	public Action(OutputModelFactory factory, ActionAST ast) :base(factory, ast)
    {
		;
		RuleFunction rf = factory.getCurrentRuleFunction();
		if (ast != null) {
			chunks = ActionTranslator.translateAction(factory, rf, ast.token, ast);
		}
		else {
			chunks = new ();
		}
		//System.out.println("actions="+chunks);
	}

	public Action(OutputModelFactory factory, StructDecl ctx, String action):base(factory,null)
    {
		ActionAST ast = new ActionAST(new CommonToken(ANTLRParser.ACTION, action));
		RuleFunction rf = factory.getCurrentRuleFunction();
		if ( rf!=null ) { // we can translate
			ast.resolver = rf.rule;
			chunks = ActionTranslator.translateActionChunk(factory, rf, action, ast);
		}
		else {
			chunks = new ();
			chunks.Add(new ActionText(ctx, action));
		}
	}

	public Action(OutputModelFactory factory, StructDecl ctx, ST actionST) :base(factory, null)
    {
		;
		chunks = new ();
		chunks.Add(new ActionTemplate(ctx, actionST));
	}

}
