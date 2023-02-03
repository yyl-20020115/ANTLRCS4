/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */
namespace org.antlr.v4.codegen.model;

/** A model object representing a parse tree listener file.
 *  These are the rules specific events triggered by a parse tree visitor.
 */
public class ListenerFile : OutputFile
{
    public string genPackage; // from -package cmd-line
    public string accessLevel; // from -DaccessLevel cmd-line
    public string exportMacro; // from -DexportMacro cmd-line
    public string grammarName;
    public string parserName;
    /**
	 * The names of all listener contexts.
	 */
    public HashSet<string> listenerNames = new();
    /**
	 * For listener contexts created for a labeled outer alternative, maps from
	 * a listener context name to the name of the rule which defines the
	 * context.
	 */
    public Dictionary<string, string> listenerLabelRuleNames = new();

    [ModelElement]
    public Action header;
    [ModelElement]
    public Dictionary<string, Action> namedActions;

    public ListenerFile(OutputModelFactory factory, string fileName) : base(factory, fileName)
    {
        var g = factory.GetGrammar();
        parserName = g.getRecognizerName();
        grammarName = g.name;
        namedActions = BuildNamedActions(factory.GetGrammar(), ast => ast.getScope() == null);
        foreach (var r in g.rules.Values)
        {
            var labels = r.getAltLabels();
            if (labels != null)
            {
                foreach (var pair in labels)
                {
                    listenerNames.Add(pair.Key);
                    listenerLabelRuleNames[pair.Key] = r.name;
                }
            }
            else
            {
                // only add rule context if no labels
                listenerNames.Add(r.name);
            }
        }
        if (g.namedActions.TryGetValue("header",out var ast)) 
            header = new (factory, ast);
        genPackage = g.Tools.genPackage;
        accessLevel = g.getOptionString("accessLevel");
        exportMacro = g.getOptionString("exportMacro");
    }
}
