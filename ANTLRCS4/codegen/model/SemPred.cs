/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.codegen.model.chunk;
using org.antlr.v4.tool.ast;

namespace org.antlr.v4.codegen.model;
public class SemPred : Action {
	/**
	 * The user-specified terminal option {@code fail}, if it was used and the
	 * value is a string literal. For example:
	 *
	 * <p>
	 * {@code {pred}?<fail='message'>}</p>
	 */
	public String msg;
	/**
	 * The predicate string with <code>{</code> and <code>}?</code> stripped from the ends.
	 */
	public String predicate;

	/**
	 * The translated chunks of the user-specified terminal option {@code fail},
	 * if it was used and the value is an action. For example:
	 *
	 * <p>
	 * {@code {pred}?<fail={"Java literal"}>}</p>
	 */
	//@ModelElement 
		public List<ActionChunk> failChunks;

	public SemPred(OutputModelFactory factory, ActionAST ast) {
		base(factory,ast);

		//assert ast.atnState != null
		//	&& ast.atnState.getNumberOfTransitions() == 1
		//	&& ast.atnState.transition(0) is AbstractPredicateTransition;

		GrammarAST failNode = ast.getOptionAST("fail");
		CodeGenerator gen = factory.getGenerator();
		predicate = ast.getText();
		if (predicate.startsWith("{") && predicate.endsWith("}?")) {
			predicate = predicate.substring(1, predicate.length() - 2);
		}
		predicate = gen.getTarget().getTargetStringLiteralFromString(predicate);

		if ( failNode==null ) return;

		if ( failNode is ActionAST ) {
			ActionAST failActionNode = (ActionAST)failNode;
			RuleFunction rf = factory.getCurrentRuleFunction();
			failChunks = ActionTranslator.translateAction(factory, rf,
														  failActionNode.token,
														  failActionNode);
		}
		else {
			msg = gen.getTarget().getTargetStringLiteralFromANTLRStringLiteral(gen,
																		  failNode.getText(),
																		  true,
																		  true);
		}
	}
}
