/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

namespace org.antlr.v4.codegen;


/** Create output objects for elements *within* rule functions except
 *  buildOutputModel() which builds outer/root model object and any
 *  objects such as RuleFunction that surround elements in rule
 *  functions.
 */
public abstract class DefaultOutputModelFactory : BlankOutputModelFactory {
	// Interface to outside world

	public readonly Grammar g;

	public readonly CodeGenerator gen;

	public OutputModelController controller;

	protected DefaultOutputModelFactory(CodeGenerator gen) {
		this.gen = gen;
		this.g = gen.g;
	}

	@Override
	public void setController(OutputModelController controller) {
		this.controller = controller;
	}

	@Override
	public OutputModelController getController() {
		return controller;
	}

	@Override
	public List<SrcOp> rulePostamble(RuleFunction function, Rule r) {
		if ( r.namedActions.containsKey("after") || r.namedActions.containsKey("finally") ) {
			// See OutputModelController.buildLeftRecursiveRuleFunction
			// and Parser.exitRule for other places which set stop.
			CodeGenerator gen = getGenerator();
			STGroup codegenTemplates = gen.getTemplates();
			ST setStopTokenAST = codegenTemplates.getInstanceOf("recRuleSetStopToken");
			Action setStopTokenAction = new Action(this, function.ruleCtx, setStopTokenAST);
			List<SrcOp> ops = new ArrayList<SrcOp>(1);
			ops.add(setStopTokenAction);
			return ops;
		}
		return super.rulePostamble(function, r);
	}

	// Convenience methods


	@Override
	public Grammar getGrammar() { return g; }

	@Override
	public CodeGenerator getGenerator() { return gen; }

	@Override
	public OutputModelObject getRoot() { return controller.getRoot(); }

	@Override
	public RuleFunction getCurrentRuleFunction() { return controller.getCurrentRuleFunction(); }

	@Override
	public Alternative getCurrentOuterMostAlt() { return controller.getCurrentOuterMostAlt(); }

	@Override
	public CodeBlock getCurrentBlock() { return controller.getCurrentBlock(); }

	@Override
	public CodeBlockForOuterMostAlt getCurrentOuterMostAlternativeBlock() { return controller.getCurrentOuterMostAlternativeBlock(); }

	@Override
	public int getCodeBlockLevel() { return controller.codeBlockLevel; }

	@Override
	public int getTreeLevel() { return controller.treeLevel; }

	// MISC


	public static List<SrcOp> list(SrcOp... values) {
		return new ArrayList<SrcOp>(Arrays.asList(values));
	}


	public static List<SrcOp> list(Collection<? : SrcOp> values) {
		return new ArrayList<SrcOp>(values);
	}
}
