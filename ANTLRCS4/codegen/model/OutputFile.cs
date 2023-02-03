/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.tool;
using org.antlr.v4.tool.ast;

namespace org.antlr.v4.codegen.model;

public abstract class OutputFile : OutputModelObject
{
    public readonly string fileName;
    public readonly string grammarFileName;
    public readonly string ANTLRVersion;
    public readonly string TokenLabelType;
    public readonly string InputSymbolType;

    public OutputFile(OutputModelFactory factory, string fileName) : base(factory)
    {
        this.fileName = fileName;
        var g = factory.GetGrammar();
        grammarFileName = g.fileName;
        ANTLRVersion = Tool.VERSION;
        TokenLabelType = g.getOptionString("TokenLabelType");
        InputSymbolType = TokenLabelType;
    }

    public Dictionary<string, Action> BuildNamedActions(Grammar g) => BuildNamedActions(g, null);

    public Dictionary<string, Action> BuildNamedActions(Grammar g, Predicate<ActionAST> filter)
    {
        Dictionary<string, Action> namedActions = new();
        foreach (var name in g.namedActions.Keys)
        {
            var ast = g.namedActions[(name)];
            if (filter == null || filter(ast))
                namedActions[name] = new (factory, ast);
        }
        return namedActions;
    }
}
