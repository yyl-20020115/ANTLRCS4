/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

namespace org.antlr.v4.codegen.model.decl;

/** */
public class Decl : SrcOp {
	public readonly string name;
	public readonly string escapedName;
	public readonly string decl; 	// whole thing if copied from action
	public bool isLocal; // if local var (not in RuleContext struct)
	public StructDecl ctx;  // which context contains us? set by addDecl

	public Decl(OutputModelFactory factory, string name, string decl = null):base(factory)
    {
		this.name = name;
		this.escapedName = factory.getGenerator().getTarget().escapeIfNeeded(name);
		this.decl = decl;
	}

	public override int GetHashCode() {
		return name.GetHashCode();
	}

	/** If same name, can't redefine, unless it's a getter */
	public override bool Equals(Object obj) {
		if ( this==obj ) return true;
		if ( obj is not Decl ) return false;
		// A() and label A are different
		if ( obj is ContextGetterDecl ) return false;
		return name.Equals(((Decl) obj).name);
	}
}
