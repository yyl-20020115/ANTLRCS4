/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.codegen.model.decl;
using org.antlr.v4.runtime.misc;
using org.antlr.v4.tool;
using org.antlr.v4.tool.ast;

namespace org.antlr.v4.codegen.model;

public class LeftRecursiveRuleFunction : RuleFunction {
	public LeftRecursiveRuleFunction(OutputModelFactory factory, LeftRecursiveRule r): base(factory, r)
    {
		CodeGenerator gen = factory.getGenerator();
		// Since we delete x=lr, we have to manually add decls for all labels
		// on left-recur refs to proper structs
		foreach (Pair<GrammarAST,String> pair in r.leftRecursiveRuleRefLabels) {
			GrammarAST idAST = pair.a;
			String altLabel = pair.b;
			String label = idAST.getText();
			GrammarAST rrefAST = (GrammarAST)idAST.getParent().getChild(1);
			if ( rrefAST.getType() == ANTLRParser.RULE_REF ) {
				Rule targetRule = factory.getGrammar().getRule(rrefAST.getText());
				String ctxName = gen.getTarget().getRuleFunctionContextStructName(targetRule);
				RuleContextDecl d;
				if (idAST.getParent().getType() == ANTLRParser.ASSIGN) {
					d = new RuleContextDecl(factory, label, ctxName);
				}
				else {
					d = new RuleContextListDecl(factory, label, ctxName);
				}

				StructDecl @struct = ruleCtx;
				if ( altLabelCtxs!=null ) {
					StructDecl s = altLabelCtxs.get(altLabel);
					if ( s!=null ) @struct = s; // if alt label, use subctx
				}
                @struct.addDecl(d); // stick in overall rule's ctx
			}
		}
	}
}
