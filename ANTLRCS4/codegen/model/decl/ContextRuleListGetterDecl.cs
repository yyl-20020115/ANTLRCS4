/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

namespace org.antlr.v4.codegen.model.decl;

/** {@code public List<XContext> X() { }
 *  public XContext X(int i) { }}
 */
public class ContextRuleListGetterDecl : ContextGetterDecl {
	public String ctxName;
	public ContextRuleListGetterDecl(OutputModelFactory factory, String name, String ctxName) : base(factory, name)
    {
		this.ctxName = ctxName;
	}
}
