/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

namespace org.antlr.v4.codegen.model;



/** */
public class Action extends RuleElement {
	@ModelElement public List<ActionChunk> chunks;

	public Action(OutputModelFactory factory, ActionAST ast) {
		super(factory,ast);
		RuleFunction rf = factory.getCurrentRuleFunction();
		if (ast != null) {
			chunks = ActionTranslator.translateAction(factory, rf, ast.token, ast);
		}
		else {
			chunks = new ArrayList<ActionChunk>();
		}
		//System.out.println("actions="+chunks);
	}

	public Action(OutputModelFactory factory, StructDecl ctx, String action) {
		super(factory,null);
		ActionAST ast = new ActionAST(new CommonToken(ANTLRParser.ACTION, action));
		RuleFunction rf = factory.getCurrentRuleFunction();
		if ( rf!=null ) { // we can translate
			ast.resolver = rf.rule;
			chunks = ActionTranslator.translateActionChunk(factory, rf, action, ast);
		}
		else {
			chunks = new ArrayList<ActionChunk>();
			chunks.add(new ActionText(ctx, action));
		}
	}

	public Action(OutputModelFactory factory, StructDecl ctx, ST actionST) {
		super(factory, null);
		chunks = new ArrayList<ActionChunk>();
		chunks.add(new ActionTemplate(ctx, actionST));
	}

}
