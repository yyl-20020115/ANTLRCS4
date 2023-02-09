/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

namespace org.antlr.v4.codegen.model;

public class LexerFile : OutputFile
{
    public string genPackage; // from -package cmd-line
    public string exportMacro; // from -DexportMacro cmd-line
    public bool genListener; // from -listener cmd-line
    public bool genVisitor; // from -visitor cmd-line
    [ModelElement]
    public Lexer lexer;
    [ModelElement]
    public Dictionary<string, Action> namedActions;

    public LexerFile(OutputModelFactory factory, string fileName) 
        : base(factory, fileName)
    {
        namedActions = BuildNamedActions(factory.Grammar);
        genPackage = factory.Grammar.Tools.genPackage;
        exportMacro = factory.Grammar.getOptionString("exportMacro");
        genListener = factory.Grammar.Tools.gen_listener;
        genVisitor = factory.Grammar.Tools.gen_visitor;
    }
}
