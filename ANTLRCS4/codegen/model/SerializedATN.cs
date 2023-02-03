/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime.atn;

namespace org.antlr.v4.codegen.model;


/** Represents a serialized ATN that is just a list of signed integers; works for all targets
 *  except for java, which requires a 16-bit char encoding. See {@link SerializedJavaATN}.
 */
public class SerializedATN : OutputModelObject
{
    public int[] serialized;

    public SerializedATN(OutputModelFactory factory) : base(factory)
    {
    }

    public SerializedATN(OutputModelFactory factory, ATN atn) : base(factory)
    {
        var data = ATNSerializer.getSerialized(atn);
        serialized = data.toArray();
    }

    public virtual object GetSerialized() => serialized;
}
