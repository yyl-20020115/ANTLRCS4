/*
 * Copyright (c) 2012-2022 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

namespace org.antlr.v4.test.runtime;

public class ProcessorResult
{
    public readonly int exitCode;
    public readonly string output;
    public readonly string errors;

    public ProcessorResult(int exitCode, string output, string errors)
    {
        this.exitCode = exitCode;
        this.output = output;
        this.errors = errors;
    }

    public bool IsSuccessful => exitCode == 0;
}
