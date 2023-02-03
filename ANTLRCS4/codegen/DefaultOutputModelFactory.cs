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
    public override Grammar GetGrammar() { return g; }

    //@Override
    public override CodeGenerator GetGenerator() { return gen; }

    //@Override
    public OutputModelObject getRoot() { return controller.GetRoot(); }

    //@Override
    public RuleFunction getCurrentRuleFunction() { return controller.GetCurrentRuleFunction(); }

    //@Override
    public Alternative getCurrentOuterMostAlt() { return controller.GetCurrentOuterMostAlt(); }

    //@Override
    public CodeBlock getCurrentBlock() { return controller.CurrentBlock; }

    //@Override
    public CodeBlockForOuterMostAlt getCurrentOuterMostAlternativeBlock() { return controller.getCurrentOuterMostAlternativeBlock(); }

    //@Override
    public int GetCodeBlockLevel() { return controller.codeBlockLevel; }

    //@Override
    public int GetTreeLevel() { return controller.treeLevel; }

    // MISC


    public static List<SrcOp> List(params SrcOp[] values) => new List<SrcOp>(values);


    public static List<SrcOp> List(ICollection<SrcOp> values) => new List<SrcOp>(values);
}
