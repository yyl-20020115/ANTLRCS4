/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.codegen.model.chunk;
using org.antlr.v4.tool;

namespace org.antlr.v4.codegen.model;

/** */
public class ParserFile : OutputFile {
	public String genPackage; // from -package cmd-line
	public String exportMacro; // from -DexportMacro cmd-line
	public bool genListener; // from -listener cmd-line
	public bool genVisitor; // from -visitor cmd-line
	[ModelElement]
		public Parser parser;
    [ModelElement] 
    public Dictionary<String, Action> namedActions;
    [ModelElement]
    public ActionChunk contextSuperClass;
	public String grammarName;

	public ParserFile(OutputModelFactory factory, String fileName): base(factory, fileName)
    {
		Grammar g = factory.getGrammar();
		namedActions = buildNamedActions(factory.getGrammar());
		genPackage = g.tool.genPackage;
		exportMacro = factory.getGrammar().getOptionString("exportMacro");
		// need the below members in the ST for Python, C++
		genListener = g.tool.gen_listener;
		genVisitor = g.tool.gen_visitor;
		grammarName = g.name;

		if (g.getOptionString("contextSuperClass") != null) {
			contextSuperClass = new ActionText(null, g.getOptionString("contextSuperClass"));
		}
	}
}
