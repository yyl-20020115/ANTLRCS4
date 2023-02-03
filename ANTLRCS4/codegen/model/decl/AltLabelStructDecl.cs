/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */


using org.antlr.v4.tool;

namespace org.antlr.v4.codegen.model.decl;


/** A StructDecl to handle a -&gt; label on alt */
public class AltLabelStructDecl : StructDecl
{
    public readonly int altNum;
    public readonly string parentRule;
    public AltLabelStructDecl(OutputModelFactory factory, Rule r,
                              int altNum, String label)
        : base(factory, r, factory.GetGenerator().Target.GetAltLabelContextStructName(label))
    {
        // override name set in base to the label ctx
        this.altNum = altNum;
        this.parentRule = r.name;
        derivedFromName = label;
    }

    //@Override
    public new void AddDispatchMethods(Rule r)
    {
        dispatchMethods = new();
        if (factory.GetGrammar().Tools.gen_listener)
        {
            dispatchMethods.Add(new ListenerDispatchMethod(factory, true));
            dispatchMethods.Add(new ListenerDispatchMethod(factory, false));
        }
        if (factory.GetGrammar().Tools.gen_visitor)
        {
            dispatchMethods.Add(new VisitorDispatchMethod(factory));
        }
    }

    //@Override
    public override int GetHashCode() => name.GetHashCode();

    //@Override
    public override bool Equals(Object? obj)
    {
        if (obj == this) return true;
        if (obj is not AltLabelStructDecl) return false;

        return name.Equals(((AltLabelStructDecl)obj).name);
    }
}
