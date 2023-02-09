/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

namespace org.antlr.v4.test.runtime.php;

public class PHPRunner : RuntimeRunner
{
    private static readonly Dictionary<string, string> environment = new();

    static PHPRunner()
    {
        environment.Add("RUNTIME", GetRuntimePath("PHP"));
    }

    
    public override string GetLanguage() => "PHP";

    
    public override Dictionary<string, string> GetExecEnvironment() => environment;
}
