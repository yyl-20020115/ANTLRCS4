/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

namespace org.antlr.v4.test.tool;

public class JavaRunner:IDisposable
{
    public JavaRunner()
    {
    }

    public JavaRunner(string workingDir, bool saveTestDir)
    {
        WorkingDir = workingDir;
        SaveTestDir = saveTestDir;
    }

    public string WorkingDir { get; }
    public bool SaveTestDir { get; }

    public void Dispose()
    {
        throw new NotImplementedException();
    }

    internal State run(RunOptions runOptions)
    {
        throw new NotImplementedException();
    }
}