/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.codegen.model.chunk;
using org.antlr.v4.codegen.model.decl;
using org.antlr.v4.runtime.misc;
using org.antlr.v4.tool;
using org.antlr.v4.tool.ast;

namespace org.antlr.v4.codegen.model;

/** */
public class InvokeRule : RuleElement , LabeledOp {
	public readonly String name;
	public readonly String escapedName;
	public readonly OrderedHashSet<Decl> labels = new OrderedHashSet<Decl>(); // TODO: should need just 1
	public readonly String ctxName;

	//@ModelElement
	public List<ActionChunk> argExprsChunks;

	public InvokeRule(ParserFactory factory, GrammarAST ast, GrammarAST labelAST) {
		super(factory, ast);
		if ( ast.atnState!=null ) {
			stateNumber = ast.atnState.stateNumber;
		}

		CodeGenerator gen = factory.getGenerator();
		Target target = gen.getTarget();
		String identifier = ast.getText();
		Rule r = factory.getGrammar().getRule(identifier);
		this.name = r.name;
		this.escapedName = gen.getTarget().escapeIfNeeded(name);
		ctxName = target.getRuleFunctionContextStructName(r);

		// TODO: move to factory
		RuleFunction rf = factory.getCurrentRuleFunction();
		if ( labelAST!=null ) {
			RuleContextDecl decl;
			// for x=r, define <rule-context-type> x and list_x
			String label = labelAST.getText();
			if ( labelAST.parent.getType() == ANTLRParser.PLUS_ASSIGN  ) {
				factory.defineImplicitLabel(ast, this);
				String listLabel = gen.getTarget().getListLabel(label);
				decl = new RuleContextListDecl(factory, listLabel, ctxName);
			}
			else {
				decl = new RuleContextDecl(factory,label,ctxName);
				labels.add(decl);
			}
			rf.addContextDecl(ast.getAltLabel(), decl);
		}

		ActionAST arg = (ActionAST)ast.getFirstChildWithType(ANTLRParser.ARG_ACTION);
		if ( arg != null ) {
			argExprsChunks = ActionTranslator.translateAction(factory, rf, arg.token, arg);
		}

		// If action refs rule as rulename not label, we need to define implicit label
		if ( factory.getCurrentOuterMostAlt().ruleRefsInActions.containsKey(identifier) ) {
			String label = gen.getTarget().getImplicitRuleLabel(identifier);
			RuleContextDecl d = new RuleContextDecl(factory,label,ctxName);
			labels.add(d);
			rf.addContextDecl(ast.getAltLabel(), d);
		}
	}

	@Override
	public List<Decl> getLabels() {
		return labels.elements();
	}
}
