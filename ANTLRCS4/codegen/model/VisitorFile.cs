/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */
using org.antlr.v4.tool;
using org.antlr.v4.tool.ast;

namespace org.antlr.v4.codegen.model;

public class VisitorFile : OutputFile {
	public String genPackage; // from -package cmd-line
	public String accessLevel; // from -DaccessLevel cmd-line
	public String exportMacro; // from -DexportMacro cmd-line
	public String grammarName;
	public String parserName;
	/**
	 * The names of all rule contexts which may need to be visited.
	 */
	public HashSet<String> visitorNames = new ();
	/**
	 * For rule contexts created for a labeled outer alternative, maps from
	 * a listener context name to the name of the rule which defines the
	 * context.
	 */
	public Dictionary<String, String> visitorLabelRuleNames = new ();

	//@ModelElement 
		public Action header;
    //@ModelElement 
    public Dictionary<String, Action> namedActions;

	public VisitorFile(OutputModelFactory factory, String fileName): base(factory, fileName)
    {
		;
		Grammar g = factory.getGrammar();
		namedActions = buildNamedActions(g);
		parserName = g.getRecognizerName();
		grammarName = g.name;
		foreach (Rule r in g.rules.Values) {
			var labels = r.getAltLabels();
			if ( labels!=null ) {
				foreach (var pair in labels) {
					visitorNames.Add(pair.Key);
					visitorLabelRuleNames[pair.Key]= r.name;
				}
			}
			else {
				// if labels, must label all. no need for generic rule visitor then
				visitorNames.Add(r.name);
			}
		}
		if ( g.namedActions.TryGetValue("header",out var ast) && ast.getScope()==null)
			header = new Action(factory, ast);
		genPackage = g.tool.genPackage;
		accessLevel = g.getOptionString("accessLevel");
		exportMacro = g.getOptionString("exportMacro");
	}
}
