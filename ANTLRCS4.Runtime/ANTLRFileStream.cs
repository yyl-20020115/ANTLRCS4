/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */
namespace org.antlr.v4.runtime;

/**
 * This is an {@link ANTLRInputStream} that is loaded from a file all at once
 * when you construct the object.
 *
 * @deprecated as of 4.7 Please use {@link CharStreams} interface.
 */
//@Deprecated
public class ANTLRFileStream : ANTLRInputStream {
	protected String fileName;

	public ANTLRFileStream(String fileName){
		this(fileName, null);
	}

	public ANTLRFileStream(String fileName, String encoding){
		this.fileName = fileName;
		load(fileName, encoding);
	}

	public void load(String fileName, String encoding)
		
	{
		data = Utils.readFile(fileName, encoding);
		this.n = data.Length;
	}

	//@Override
	public String getSourceName() {
		return fileName;
	}
}
