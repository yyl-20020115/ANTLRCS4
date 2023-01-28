/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

namespace org.antlr.v4.codegen.model;

public class LexerFile : OutputFile {
	public String genPackage; // from -package cmd-line
	public String exportMacro; // from -DexportMacro cmd-line
	public bool genListener; // from -listener cmd-line
	public bool genVisitor; // from -visitor cmd-line
	//@ModelElement 
		public Lexer lexer;
    //@ModelElement 
    public Dictionary<String, Action> namedActions;

	public LexerFile(OutputModelFactory factory, String fileName) {
		base(factory, fileName);
		namedActions = buildNamedActions(factory.getGrammar());
		genPackage = factory.getGrammar().tool.genPackage;
		exportMacro = factory.getGrammar().getOptionString("exportMacro");
		genListener = factory.getGrammar().tool.gen_listener;
		genVisitor = factory.getGrammar().tool.gen_visitor;
	}
}
