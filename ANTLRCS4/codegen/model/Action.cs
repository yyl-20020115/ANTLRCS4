/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using Antlr4.StringTemplate;
using org.antlr.v4.codegen.model.chunk;
using org.antlr.v4.codegen.model.decl;
using org.antlr.v4.parse;
using org.antlr.v4.runtime;
using org.antlr.v4.tool.ast;

namespace org.antlr.v4.codegen.model;


/** */
public class Action : RuleElement
{
    [ModelElement]
    public List<ActionChunk> chunks;

    public Action(OutputModelFactory factory, ActionAST ast) : base(factory, ast)
    {
        var rf = factory.CurrentRuleFunction;
        chunks = ast != null ? ActionTranslator.TranslateAction(factory, rf, ast.token, ast) : (new());
        //Console.Out.WriteLine("actions="+chunks);
    }

    public Action(OutputModelFactory factory, StructDecl ctx, string action) : base(factory, null)
    {
        var ast = new ActionAST(new CommonToken(ANTLRParser.ACTION, action));
        var rf = factory.CurrentRuleFunction;
        if (rf != null)
        { // we can translate
            ast.resolver = rf.rule;
            this.chunks = ActionTranslator.TranslateActionChunk(factory, rf, action, ast);
        }
        else
        {
            this.chunks = new()
            {
                new ActionText(ctx, action)
            };
        }
    }

    public Action(OutputModelFactory factory, StructDecl ctx, Template actionST) : base(factory, null)
    {
        this.chunks = new()
        {
            new ActionTemplate(ctx, actionST)
        };
    }

}
