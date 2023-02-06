/*
 * Copyright (c) 2012-2022 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using System.Diagnostics;

namespace org.antlr.v4.test.runtime;

public class Processor
{
    public readonly string[] arguments;
    public readonly string workingDirectory;
    public readonly Dictionary<string, string> environmentVariables;
    public readonly bool throwOnNonZeroErrorCode;

    public static ProcessorResult Run(string[] arguments, string workingDirectory, Dictionary<string, string> environmentVariables)

    {
        return new Processor(arguments, workingDirectory, environmentVariables, true).Start();
    }

    public static ProcessorResult Run(string[] arguments, string workingDirectory)
    {
        return new Processor(arguments, workingDirectory, new(), true).Start();
    }

    public Processor(string[] arguments, string workingDirectory, Dictionary<string, string> environmentVariables,
                     bool throwOnNonZeroErrorCode)
    {
        this.arguments = arguments;
        this.workingDirectory = workingDirectory;
        this.environmentVariables = environmentVariables;
        this.throwOnNonZeroErrorCode = throwOnNonZeroErrorCode;
    }

    public ProcessorResult Start()
    {
        var processStartInfo
            = new ProcessStartInfo()
            {
                WorkingDirectory = workingDirectory
            };

        if (environmentVariables != null && environmentVariables.Count > 0)
        {
            var environment = processStartInfo.EnvironmentVariables;
            foreach (String key in environmentVariables.Keys)
            {
                environment.Add(key, environmentVariables[(key)]);
            }
        }

        var process = Process.Start(processStartInfo);

        RunnableStreamReader stdoutReader = new(process.StandardOutput);
        RunnableStreamReader stderrReader = new(process.StandardError);

        stdoutReader.Start();
        stderrReader.Start();

        process.WaitForExit();

        stdoutReader.Join();
        stderrReader.Join();

        String output = stdoutReader.ToString();
        String errors = stderrReader.ToString();
        if (throwOnNonZeroErrorCode && process.ExitCode != 0)
        {
            throw new Exception(RuntimeTestUtils.JoinLines(output, errors));
        }
        return new ProcessorResult(process.ExitCode, output, errors);
    }
}
