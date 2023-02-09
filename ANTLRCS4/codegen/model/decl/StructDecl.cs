/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime.misc;
using org.antlr.v4.tool;
using Attribute = org.antlr.v4.tool.Attribute;

namespace org.antlr.v4.codegen.model.decl;

/** This object models the structure holding all of the parameters,
 *  return values, local variables, and labels associated with a rule.
 */
public class StructDecl : Decl
{
    public string derivedFromName; // rule name or label name
    public bool provideCopyFrom;
    [ModelElement]
    public OrderedHashSet<Decl> attrs = new ();
    [ModelElement]
    public OrderedHashSet<Decl> getters = new ();
    [ModelElement]
    public ICollection<AttributeDecl> ctorAttrs;
    [ModelElement]
    public List<DispatchMethod> dispatchMethods;
    [ModelElement]
    public List<OutputModelObject> interfaces;
    [ModelElement]
    public List<OutputModelObject> extensionMembers;

    // Track these separately; Go target needs to generate getters/setters
    // Do not make them templates; we only need the Decl object not the ST
    // built from it. Avoids adding args to StructDecl template
    public readonly OrderedHashSet<Decl> tokenDecls = new ();
    public readonly OrderedHashSet<Decl> tokenTypeDecls = new ();
    public readonly OrderedHashSet<Decl> tokenListDecls = new ();
    public readonly OrderedHashSet<Decl> ruleContextDecls = new ();
    public readonly OrderedHashSet<Decl> ruleContextListDecls = new ();
    public readonly OrderedHashSet<Decl> attributeDecls = new ();

    public StructDecl(OutputModelFactory factory, Rule r) : this(factory, r, null)
    {
    }

    protected StructDecl(OutputModelFactory factory, Rule r, String name) : base(factory, name == null ? factory.Generator.Target.GetRuleFunctionContextStructName(r) : name)
    {
        AddDispatchMethods(r);
        derivedFromName = r.name;
        provideCopyFrom = r.HasAltSpecificContexts();
    }

    public void AddDispatchMethods(Rule r)
    {
        dispatchMethods = new();
        if (!r.HasAltSpecificContexts())
        {
            // no enter/exit for this ruleContext if rule has labels
            if (factory.Grammar.Tools.gen_listener)
            {
                dispatchMethods.Add(new ListenerDispatchMethod(factory, true));
                dispatchMethods.Add(new ListenerDispatchMethod(factory, false));
            }
            if (factory.Grammar.Tools.gen_visitor)
            {
                dispatchMethods.Add(new VisitorDispatchMethod(factory));
            }
        }
    }

    public void AddDecl(Decl d)
    {
        d.ctx = this;

        if (d is ContextGetterDecl) 
            getters.Add(d);
        else 
            attrs.Add(d);

        // add to specific "lists"
        if (d is TokenTypeDecl)
        {
            tokenTypeDecls.Add(d);
        }
        else if (d is TokenListDecl)
        {
            tokenListDecls.Add(d);
        }
        else if (d is TokenDecl)
        {
            tokenDecls.Add(d);
        }
        else if (d is RuleContextListDecl)
        {
            ruleContextListDecls.Add(d);
        }
        else if (d is RuleContextDecl)
        {
            ruleContextDecls.Add(d);
        }
        else if (d is AttributeDecl)
        {
            attributeDecls.Add(d);
        }
    }

    public void AddDecl(Attribute a)
    {
        AddDecl(new AttributeDecl(factory, a));
    }

    public void AddDecls(ICollection<Attribute> attrList)
    {
        foreach (Attribute a in attrList) AddDecl(a);
    }

    public void ImplementInterface(OutputModelObject value)
    {
        interfaces ??= new();
        interfaces.Add(value);
    }

    public void AddExtensionMember(OutputModelObject member)
    {
        extensionMembers ??= new();
        extensionMembers.Add(member);
    }

    public bool IsEmpty => attrs.Count == 0;
}
