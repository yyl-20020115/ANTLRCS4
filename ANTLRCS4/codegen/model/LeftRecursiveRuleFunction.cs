/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.codegen.model.decl;
using org.antlr.v4.parse;
using org.antlr.v4.tool;
using org.antlr.v4.tool.ast;

namespace org.antlr.v4.codegen.model;

public class LeftRecursiveRuleFunction : RuleFunction
{
    public LeftRecursiveRuleFunction(OutputModelFactory factory, LeftRecursiveRule r) : base(factory, r)
    {
        var gen = factory.GetGenerator();
        // Since we delete x=lr, we have to manually add decls for all labels
        // on left-recur refs to proper structs
        foreach (var pair in r.leftRecursiveRuleRefLabels)
        {
            var idAST = pair.a;
            var altLabel = pair.b;
            var label = idAST.getText();
            var rrefAST = (GrammarAST)idAST.getParent().GetChild(1);
            if (rrefAST.getType() == ANTLRParser.RULE_REF)
            {
                var targetRule = factory.GetGrammar().getRule(rrefAST.getText());
                var ctxName = gen.Target.GetRuleFunctionContextStructName(targetRule);
                var d = idAST.getParent().Type == ANTLRParser.ASSIGN
                    ? new RuleContextDecl(factory, label, ctxName)
                    : new RuleContextListDecl(factory, label, ctxName);
                var @struct = ruleCtx;
                if (altLabelCtxs != null)
                {
                    if (altLabelCtxs.TryGetValue(altLabel, out var s))
                        @struct = s; // if alt label, use subctx
                }
                @struct.AddDecl(d); // stick in overall rule's ctx
            }
        }
    }
}
