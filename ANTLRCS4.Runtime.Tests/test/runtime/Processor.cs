/*
 * Copyright (c) 2012-2022 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using System.Diagnostics;

namespace org.antlr.v4.test.runtime;

public class Processor {
	public readonly String[] arguments;
	public readonly String workingDirectory;
	public readonly Dictionary<String, String> environmentVariables;
	public readonly bool throwOnNonZeroErrorCode;

	public static ProcessorResult run(String[] arguments, String workingDirectory, Dictionary<String, String> environmentVariables)
			
	{
		return new Processor(arguments, workingDirectory, environmentVariables, true).start();
	}

	public static ProcessorResult run(String[] arguments, String workingDirectory) {
		return new Processor(arguments, workingDirectory, new (), true).start();
	}

	public Processor(String[] arguments, String workingDirectory, Dictionary<String, String> environmentVariables,
					 bool throwOnNonZeroErrorCode) {
		this.arguments = arguments;
		this.workingDirectory = workingDirectory;
		this.environmentVariables = environmentVariables;
		this.throwOnNonZeroErrorCode = throwOnNonZeroErrorCode;
	}

	public ProcessorResult start() {
        ProcessStartInfo processStartInfo
            = new()
            {
                WorkingDirectory = workingDirectory
            };

        if (environmentVariables != null && environmentVariables.Count > 0) {
			var environment = processStartInfo.EnvironmentVariables;
            foreach (String key in environmentVariables.Keys) {
				environment.Add(key, environmentVariables[(key)]);
			}
		}
        
        Process process = Process.Start(processStartInfo);

        RunnableStreamReader stdoutReader = new (process.StandardOutput);
        RunnableStreamReader stderrReader = new (process.StandardError);

        stdoutReader.start();
		stderrReader.start();

		process.WaitForExit();

		stdoutReader.join();
		stderrReader.join();

		String output = stdoutReader.ToString();
		String errors = stderrReader.ToString();
		if (throwOnNonZeroErrorCode && process.ExitCode != 0) {
			throw new Exception(RuntimeTestUtils.joinLines(output, errors));
		}
		return new ProcessorResult(process.ExitCode, output, errors);
	}
}
