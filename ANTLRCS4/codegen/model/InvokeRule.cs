/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.codegen.model.chunk;
using org.antlr.v4.codegen.model.decl;
using org.antlr.v4.parse;
using org.antlr.v4.runtime.misc;
using org.antlr.v4.tool.ast;

namespace org.antlr.v4.codegen.model;

/** */
public class InvokeRule : RuleElement, LabeledOp
{
    public readonly string name;
    public readonly string escapedName;
    public readonly OrderedHashSet<Decl> labels = new (); // TODO: should need just 1
    public readonly string ctxName;

    [ModelElement]
    public List<ActionChunk> argExprsChunks;

    public InvokeRule(ParserFactory factory, GrammarAST ast, GrammarAST labelAST)
        : base(factory, ast)
    {
        if (ast.atnState != null)
            stateNumber = ast.atnState.stateNumber;

        var gen = factory.Generator;
        var target = gen.Target;
        var identifier = ast.Text;
        var r = factory.Grammar.GetRule(identifier);
        this.name = r.name;
        this.escapedName = gen.Target.EscapeIfNeeded(name);
        ctxName = target.GetRuleFunctionContextStructName(r);

        // TODO: move to factory
        var rf = factory.CurrentRuleFunction;
        if (labelAST != null)
        {
            RuleContextDecl decl;
            // for x=r, define <rule-context-type> x and list_x
            var label = labelAST.Text;
            if (labelAST.parent.Type == ANTLRParser.PLUS_ASSIGN)
            {
                factory.DefineImplicitLabel(ast, this);
                var listLabel = gen.Target.GetListLabel(label);
                decl = new RuleContextListDecl(factory, listLabel, ctxName);
            }
            else
            {
                decl = new RuleContextDecl(factory, label, ctxName);
                labels.Add(decl);
            }
            rf.AddContextDecl(ast.GetAltLabel(), decl);
        }

        var arg = (ActionAST)ast.GetFirstChildWithType(ANTLRParser.ARG_ACTION);
        if (arg != null)
        {
            argExprsChunks = ActionTranslator.TranslateAction(factory, rf, arg.token, arg);
        }

        // If action refs rule as rulename not label, we need to define implicit label
        if (factory.CurrentOuterMostAlt.ruleRefsInActions.ContainsKey(identifier))
        {
            var label = gen.Target.GetImplicitRuleLabel(identifier);
            var d = new RuleContextDecl(factory, label, ctxName);
            labels.Add(d);
            rf.AddContextDecl(ast.GetAltLabel(), d);
        }
    }

    //@Override
    public virtual List<Decl> Labels => labels.Elements();
}
