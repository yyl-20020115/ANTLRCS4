/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime.misc;

namespace org.antlr.v4.codegen.model.decl;

public abstract class ContextGetterDecl : Decl
{
    public ContextGetterDecl(OutputModelFactory factory, string name) : base(factory, name) { }

    /** Not used for output; just used to distinguish between decl types
	 *  to avoid dups.
	 */
    public virtual string ArgType => ""; // assume no args

    public override int GetHashCode()
    {
        var hash = MurmurHash.Initialize();
        hash = MurmurHash.Update(hash, name);
        hash = MurmurHash.Update(hash, ArgType);
        hash = MurmurHash.Finish(hash, 2);
        return hash;
    }

    /** Make sure that a getter does not equal a label. X() and X are ok.
	 *  OTOH, treat X() with two diff return values as the same.  Treat
	 *  two X() with diff args as different.
	 */
    //@Override
    public override bool Equals(object? o) =>
        // A() and label A are different
        this == o || o is ContextGetterDecl decl1 && name.Equals(((Decl)o).name) &&
                ArgType.Equals(decl1.ArgType);
}
