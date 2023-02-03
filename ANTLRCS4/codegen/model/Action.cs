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
        var rf = factory.GetCurrentRuleFunction();
        if (ast != null)
        {
            chunks = ActionTranslator.TranslateAction(factory, rf, ast.token, ast);
        }
        else
        {
            chunks = new();
        }
        //Console.Out.WriteLine("actions="+chunks);
    }

    public Action(OutputModelFactory factory, StructDecl ctx, String action) : base(factory, null)
    {
        var ast = new ActionAST(new CommonToken(ANTLRParser.ACTION, action));
        var rf = factory.GetCurrentRuleFunction();
        if (rf != null)
        { // we can translate
            ast.resolver = rf.rule;
            chunks = ActionTranslator.TranslateActionChunk(factory, rf, action, ast);
        }
        else
        {
            chunks = new();
            chunks.Add(new ActionText(ctx, action));
        }
    }

    public Action(OutputModelFactory factory, StructDecl ctx, Template actionST) : base(factory, null)
    {
        chunks = new();
        chunks.Add(new ActionTemplate(ctx, actionST));
    }

}
