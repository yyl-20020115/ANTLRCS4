/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.tool;

namespace org.antlr.v4.codegen.model;

public class RuleActionFunction : OutputModelObject
{
    public readonly String escapedName;
    public readonly String ctxType;
    public readonly int ruleIndex;
    public readonly string name;
    /** Map actionIndex to Action */
    [ModelElement]
    public Dictionary<int, Action> actions = new();

    public RuleActionFunction(OutputModelFactory factory, Rule r, string ctxType) : base(factory)
    {
        name = r.name;
        escapedName = factory.Generator.Target.EscapeIfNeeded(name);
        ruleIndex = r.index;
        this.ctxType = ctxType;
    }
}
