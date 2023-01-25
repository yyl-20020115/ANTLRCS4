/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

namespace org.antlr.v4.codegen.model.chunk;

/** */
public class QRetValueRef : RetValueRef {
	public readonly String dict;

	public QRetValueRef(StructDecl ctx, String dict, String name, String escapedName) {
		super(ctx, name, escapedName);
		this.dict = dict;
	}
}
