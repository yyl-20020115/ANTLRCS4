/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */
using org.antlr.v4.runtime.misc;
using System.Text;

namespace org.antlr.v4.runtime;

/**
 * This is an {@link ANTLRInputStream} that is loaded from a file all at once
 * when you construct the object.
 *
 * @deprecated as of 4.7 Please use {@link CharStreams} interface.
 */
//@Deprecated
public class ANTLRFileStream : ANTLRInputStream
{
    protected string fileName;

    public ANTLRFileStream(string fileName) : this(fileName, null)
    { }

    public ANTLRFileStream(string fileName, Encoding encoding)
    {
        this.fileName = fileName;
        Load(fileName, encoding);
    }

    public void Load(string fileName, Encoding encoding)
    {
        data = RuntimeUtils.ReadFile(fileName, encoding);
        this.n = data.Length;
    }

    //@Override
    public string GetSourceName()
    {
        return fileName;
    }
}
