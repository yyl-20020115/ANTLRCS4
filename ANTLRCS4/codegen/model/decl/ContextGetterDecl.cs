/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

namespace org.antlr.v4.codegen.model.decl;

public abstract class ContextGetterDecl : Decl {
	public ContextGetterDecl(OutputModelFactory factory, String name): base(factory, name) { }
    
	/** Not used for output; just used to distinguish between decl types
	 *  to avoid dups.
	 */
	public virtual String getArgType() => ""; // assume no args

	public override int GetHashCode() {
		int hash = MurmurHash.initialize();
		hash = MurmurHash.update(hash, name);
		hash = MurmurHash.update(hash, getArgType());
		hash = MurmurHash.finish(hash, 2);
		return hash;
	}

	/** Make sure that a getter does not equal a label. X() and X are ok.
	 *  OTOH, treat X() with two diff return values as the same.  Treat
	 *  two X() with diff args as different.
	 */
	@Override
	public bool equals(Object obj) {
		if ( this==obj ) return true;
		// A() and label A are different
		if ( !(obj instanceof ContextGetterDecl) ) return false;
		return name.equals(((Decl) obj).name) &&
				getArgType().equals(((ContextGetterDecl) obj).getArgType());
	}
}
