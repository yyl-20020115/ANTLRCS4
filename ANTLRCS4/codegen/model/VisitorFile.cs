/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */
using org.antlr.v4.tool;
using org.antlr.v4.tool.ast;

namespace org.antlr.v4.codegen.model;

public class VisitorFile : OutputFile
{
    public string genPackage; // from -package cmd-line
    public string accessLevel; // from -DaccessLevel cmd-line
    public string exportMacro; // from -DexportMacro cmd-line
    public string grammarName;
    public string parserName;
    /**
	 * The names of all rule contexts which may need to be visited.
	 */
    public HashSet<string> visitorNames = new();
    /**
	 * For rule contexts created for a labeled outer alternative, maps from
	 * a listener context name to the name of the rule which defines the
	 * context.
	 */
    public Dictionary<string, string> visitorLabelRuleNames = new();

    [ModelElement]
    public Action header;
    [ModelElement]
    public Dictionary<string, Action> namedActions;

    public VisitorFile(OutputModelFactory factory, string fileName) : base(factory, fileName)
    {
        var g = factory.Grammar;
        namedActions = BuildNamedActions(g);
        parserName = g.GetRecognizerName();
        grammarName = g.name;
        foreach (var r in g.rules.Values)
        {
            var labels = r.GetAltLabels();
            if (labels != null)
            {
                foreach (var pair in labels)
                {
                    visitorNames.Add(pair.Key);
                    visitorLabelRuleNames[pair.Key] = r.name;
                }
            }
            else
            {
                // if labels, must label all. no need for generic rule visitor then
                visitorNames.Add(r.name);
            }
        }
        if (g.namedActions.TryGetValue("header", out var ast) && ast.Scope == null)
            header = new Action(factory, ast);
        genPackage = g.Tools.genPackage;
        accessLevel = g.GetOptionString("accessLevel");
        exportMacro = g.GetOptionString("exportMacro");
    }
}
