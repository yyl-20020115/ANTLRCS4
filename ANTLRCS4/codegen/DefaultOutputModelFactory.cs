/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using Antlr4.StringTemplate;
using org.antlr.v4.codegen.model;
using org.antlr.v4.codegen.model.decl;
using org.antlr.v4.tool;
using Action = org.antlr.v4.codegen.model.Action;

namespace org.antlr.v4.codegen;


/** Create output objects for elements *within* rule functions except
 *  buildOutputModel() which builds outer/root model object and any
 *  objects such as RuleFunction that surround elements in rule
 *  functions.
 */
public abstract class DefaultOutputModelFactory : BlankOutputModelFactory
{
    // Interface to outside world

    public readonly Grammar g;

    public readonly CodeGenerator gen;

    public OutputModelController controller;

    protected DefaultOutputModelFactory(CodeGenerator gen)
    {
        this.gen = gen;
        this.g = gen.g;
    }

    //@Override
    //@Override
    public OutputModelController Controller 
    { 
        get => controller; 
        set => this.controller = value; 
    }

    //@Override
    public override List<SrcOp> RulePostamble(RuleFunction function, Rule r)
    {
        if (r.namedActions.ContainsKey("after") || r.namedActions.ContainsKey("finally"))
        {
            // See OutputModelController.buildLeftRecursiveRuleFunction
            // and Parser.exitRule for other places which set stop.
            var gen = this.GetGenerator();
            var codegenTemplates = gen.Templates;
            var setStopTokenAST = codegenTemplates.GetInstanceOf("recRuleSetStopToken");
            var setStopTokenAction = new Action(this, function.ruleCtx, setStopTokenAST);
            List<SrcOp> ops = new(1)
            {
                setStopTokenAction
            };
            return ops;
        }
        return base.RulePostamble(function, r);
    }

    // Convenience methods


    //@Override
    public override Grammar GetGrammar() => g;

    //@Override
    public override CodeGenerator GetGenerator() => gen;

    //@Override
    public override OutputModelObject GetRoot() => controller.GetRoot();

    //@Override
    public override RuleFunction GetCurrentRuleFunction() => controller.GetCurrentRuleFunction();

    //@Override
    public override Alternative GetCurrentOuterMostAlt() => controller.GetCurrentOuterMostAlt();

    //@Override
    public override CodeBlock GetCurrentBlock() => controller.CurrentBlock;

    //@Override
    public override CodeBlockForOuterMostAlt GetCurrentOuterMostAlternativeBlock() => controller.CurrentOuterMostAlternativeBlock;

    //@Override
    public override int GetCodeBlockLevel() => controller.codeBlockLevel;

    //@Override
    public override int GetTreeLevel() => controller.treeLevel;

    // MISC
    public static List<SrcOp> List(params SrcOp[] values) => new (values);


    public static List<SrcOp> List(ICollection<SrcOp> values) => new (values);
}
