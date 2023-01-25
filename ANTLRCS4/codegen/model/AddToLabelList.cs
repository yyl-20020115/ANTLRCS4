/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

namespace org.antlr.v4.codegen.model;


/** */
public class AddToLabelList extends SrcOp {
	public final Decl label;
	public final String listName;

	public AddToLabelList(OutputModelFactory factory, String listName, Decl label) {
		super(factory);
		this.label = label;
		this.listName = listName;
	}
}
