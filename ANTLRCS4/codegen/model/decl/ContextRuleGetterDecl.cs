/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

namespace org.antlr.v4.codegen.model.decl;

/** {@code public XContext X() { }} */
public class ContextRuleGetterDecl : ContextGetterDecl {
	public String ctxName;
	public bool optional;

	public ContextRuleGetterDecl(OutputModelFactory factory, String name, String ctxName, bool optional): base(factory, name)
    {
		this.ctxName = ctxName;
		this.optional = optional;
	}
}
