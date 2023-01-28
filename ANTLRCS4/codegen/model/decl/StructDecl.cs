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
public class StructDecl : Decl {
	public String derivedFromName; // rule name or label name
	public bool provideCopyFrom;
    //@ModelElement 
    public OrderedHashSet<Decl> attrs = new OrderedHashSet<Decl>();
    //@ModelElement 
    public OrderedHashSet<Decl> getters = new OrderedHashSet<Decl>();
    //@ModelElement 
    public ICollection<AttributeDecl> ctorAttrs;
    //@ModelElement 
    public List<DispatchMethod> dispatchMethods;
    //@ModelElement 
    public List<OutputModelObject> interfaces;
    //@ModelElement 
    public List<OutputModelObject> extensionMembers;

	// Track these separately; Go target needs to generate getters/setters
	// Do not make them templates; we only need the Decl object not the ST
	// built from it. Avoids adding args to StructDecl template
	public OrderedHashSet<Decl> tokenDecls = new OrderedHashSet<Decl>();
	public OrderedHashSet<Decl> tokenTypeDecls = new OrderedHashSet<Decl>();
	public OrderedHashSet<Decl> tokenListDecls = new OrderedHashSet<Decl>();
	public OrderedHashSet<Decl> ruleContextDecls = new OrderedHashSet<Decl>();
	public OrderedHashSet<Decl> ruleContextListDecls = new OrderedHashSet<Decl>();
	public OrderedHashSet<Decl> attributeDecls = new OrderedHashSet<Decl>();

	public StructDecl(OutputModelFactory factory, Rule r): this(factory, r, null)
    {
	}

	protected StructDecl(OutputModelFactory factory, Rule r, String name) : base(factory, name == null ? factory.getGenerator().getTarget().getRuleFunctionContextStructName(r) : name)
	{
		addDispatchMethods(r);
		derivedFromName = r.name;
		provideCopyFrom = r.hasAltSpecificContexts();
	}

	public void addDispatchMethods(Rule r) {
		dispatchMethods = new ();
		if ( !r.hasAltSpecificContexts() ) {
			// no enter/exit for this ruleContext if rule has labels
			if ( factory.getGrammar().tool.gen_listener ) {
				dispatchMethods.add(new ListenerDispatchMethod(factory, true));
				dispatchMethods.add(new ListenerDispatchMethod(factory, false));
			}
			if ( factory.getGrammar().tool.gen_visitor ) {
				dispatchMethods.add(new VisitorDispatchMethod(factory));
			}
		}
	}

	public void addDecl(Decl d) {
		d.ctx = this;

		if ( d is ContextGetterDecl ) getters.add(d);
		else attrs.add(d);

		// add to specific "lists"
		if ( d is TokenTypeDecl ) {
			tokenTypeDecls.add(d);
		}
		else if ( d is TokenListDecl ) {
			tokenListDecls.add(d);
		}
		else if ( d is TokenDecl ) {
			tokenDecls.add(d);
		}
		else if ( d is RuleContextListDecl ) {
			ruleContextListDecls.add(d);
		}
		else if ( d is RuleContextDecl ) {
			ruleContextDecls.add(d);
		}
		else if ( d is AttributeDecl ) {
			attributeDecls.add(d);
		}
	}

	public void addDecl(Attribute a) {
		addDecl(new AttributeDecl(factory, a));
	}

	public void addDecls(ICollection<Attribute> attrList) {
		foreach (Attribute a in attrList) addDecl(a);
	}

	public void implementInterface(OutputModelObject value) {
		if (interfaces == null) {
			interfaces = new ();
		}

		interfaces.Add(value);
	}

	public void addExtensionMember(OutputModelObject member) {
		if (extensionMembers == null) {
			extensionMembers = new ();
		}

		extensionMembers.Add(member);
	}

	public bool isEmpty() { return attrs.isEmpty(); }
}
