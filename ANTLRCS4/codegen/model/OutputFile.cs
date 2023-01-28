/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.tool;
using org.antlr.v4.tool.ast;

namespace org.antlr.v4.codegen.model;

public abstract class OutputFile : OutputModelObject {
	public readonly String fileName;
	public readonly String grammarFileName;
	public readonly String ANTLRVersion;
    public readonly String TokenLabelType;
    public readonly String InputSymbolType;

    public OutputFile(OutputModelFactory factory, String fileName): base(factory)
    {
        this.fileName = fileName;
        Grammar g = factory.getGrammar();
		grammarFileName = g.fileName;
		ANTLRVersion = Tool.VERSION;
        TokenLabelType = g.getOptionString("TokenLabelType");
        InputSymbolType = TokenLabelType;
    }

	public Dictionary<String, Action> buildNamedActions(Grammar g) {
		return buildNamedActions(g, null);
	}

	public Dictionary<String, Action> buildNamedActions(Grammar g, Predicate<ActionAST> filter) {
		Dictionary<String, Action> namedActions = new ();
        foreach (String name in g.namedActions.Keys) {
			ActionAST ast = g.namedActions[(name)];
			if(filter==null || filter(ast))
				namedActions[name]=new Action(factory, ast);
		}
		return namedActions;
	}
}
