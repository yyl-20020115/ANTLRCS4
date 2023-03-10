/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

namespace org.antlr.v4.codegen.model;

public class Parser : Recognizer
{
    public readonly ParserFile file;

    [ModelElement]
    public List<RuleFunction> funcs = new();

    public Parser(OutputModelFactory factory, ParserFile file) : base(factory) => this.file = file; // who contains us?
}
