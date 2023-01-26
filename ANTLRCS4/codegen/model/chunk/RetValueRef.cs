/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.codegen.model.decl;
using System.Xml.Linq;

namespace org.antlr.v4.codegen.model.chunk;

/** */
public class RetValueRef : SymbolRefChunk {
	public RetValueRef(StructDecl ctx, String name, String escapedName) :base(ctx, name, escapedName)
    {
		
	}
}
