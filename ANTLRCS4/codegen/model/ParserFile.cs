/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.codegen.model.chunk;
using org.antlr.v4.tool;

namespace org.antlr.v4.codegen.model;

/** */
public class ParserFile : OutputFile
{
    public string genPackage; // from -package cmd-line
    public string exportMacro; // from -DexportMacro cmd-line
    public bool genListener; // from -listener cmd-line
    public bool genVisitor; // from -visitor cmd-line
    [ModelElement]
    public Parser parser;
    [ModelElement]
    public Dictionary<string, Action> namedActions;
    [ModelElement]
    public ActionChunk contextSuperClass;
    public string grammarName;

    public ParserFile(OutputModelFactory factory, string fileName) : base(factory, fileName)
    {
        var g = factory.Grammar;
        namedActions = BuildNamedActions(factory.Grammar);
        genPackage = g.Tools.genPackage;
        exportMacro = factory.Grammar.getOptionString("exportMacro");
        // need the below members in the ST for Python, C++
        genListener = g.Tools.gen_listener;
        genVisitor = g.Tools.gen_visitor;
        grammarName = g.name;

        if (g.getOptionString("contextSuperClass") != null)
        {
            contextSuperClass = new ActionText(null, g.getOptionString("contextSuperClass"));
        }
    }
}
