/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */
using Antlr4.StringTemplate;

namespace org.antlr.v4.test.runtime.python;

public abstract class PythonRunner : RuntimeRunner
{
    //
    public override string GetExtension() => "py";

    
    protected override void AddExtraRecognizerParameters(Template template)
    {
        template.Add("python3", GetLanguage().Equals("Python3"));
    }
}
