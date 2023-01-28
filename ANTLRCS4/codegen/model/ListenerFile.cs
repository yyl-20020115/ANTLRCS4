/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */
using org.antlr.v4.runtime.misc;
using org.antlr.v4.tool;
using org.antlr.v4.tool.ast;

namespace org.antlr.v4.codegen.model;

/** A model object representing a parse tree listener file.
 *  These are the rules specific events triggered by a parse tree visitor.
 */
public class ListenerFile : OutputFile {
	public String genPackage; // from -package cmd-line
	public String accessLevel; // from -DaccessLevel cmd-line
	public String exportMacro; // from -DexportMacro cmd-line
	public String grammarName;
	public String parserName;
	/**
	 * The names of all listener contexts.
	 */
	public HashSet<String> listenerNames = new();
	/**
	 * For listener contexts created for a labeled outer alternative, maps from
	 * a listener context name to the name of the rule which defines the
	 * context.
	 */
	public Dictionary<String, String> listenerLabelRuleNames = new ();

	[ModelElement] 
		public Action header;
	[ModelElement]
		public Dictionary<String, Action> namedActions;

	public ListenerFile(OutputModelFactory factory, String fileName): base(factory, fileName)
    {
		;
		Grammar g = factory.getGrammar();
		parserName = g.getRecognizerName();
		grammarName = g.name;
		namedActions = buildNamedActions(factory.getGrammar(), ast => ast.getScope() == null);
        foreach (Rule r in g.rules.Values) {
			var labels = r.getAltLabels();
			if ( labels!=null ) {
                foreach (var pair in labels) {
					listenerNames.Add(pair.Key);
					listenerLabelRuleNames[pair.Key] = r.name;
				}
			}
			else {
				// only add rule context if no labels
				listenerNames.Add(r.name);
			}
		}
		ActionAST ast = g.namedActions["header"];
		if ( ast!=null ) header = new Action(factory, ast);
		genPackage = g.tool.genPackage;
		accessLevel = g.getOptionString("accessLevel");
		exportMacro = g.getOptionString("exportMacro");
	}
}
