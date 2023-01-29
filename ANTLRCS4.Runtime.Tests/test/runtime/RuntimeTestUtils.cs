/*
 * Copyright (c) 2012-2022 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.automata;
using org.antlr.v4.runtime.atn;
using org.antlr.v4.runtime.tree.pattern;
using org.antlr.v4.test.runtime.java.api;
using org.antlr.v4.tool;
using System.Text;

namespace org.antlr.v4.test.runtime;

public abstract class RuntimeTestUtils {
	public static readonly String NewLine = Environment.NewLine;
	public static readonly String PathSeparator = Path.PathSeparator.ToString();// System.getProperty("path.separator");
	public static readonly String FileSeparator = Path.PathSeparator.ToString();// System.getProperty("file.separator");
	public static readonly String TempDirectory = Environment.CurrentDirectory;// System.getProperty("java.io.tmpdir");

	public static readonly string runtimePath;
	public static readonly string runtimeTestsuitePath;
	public static readonly string resourcePath;

	private static readonly Dictionary<String, String> resourceCache = new ();
	private static OSType detectedOS;
	private static Boolean isWindows;

	static RuntimeTestUtils() {
		String locationPath = Environment.CurrentDirectory;
		//if (IsWindows()) {
		//	locationPath = locationPath.ReplaceFirst("/", "");
		//}
		string potentialRuntimeTestsuitePath = Path.Combine(locationPath, "..", "..");
		string potentialResourcePath = Path.Combine(potentialRuntimeTestsuitePath.ToString(), "resources");

		if (File.Exists(potentialResourcePath)) {
			runtimeTestsuitePath = potentialRuntimeTestsuitePath;
		}
		else {
			runtimeTestsuitePath = Path.Combine("..", "runtime-testsuite");
		}

		runtimePath = Path.Combine(runtimeTestsuitePath.ToString(), "..", "runtime");
		resourcePath = Path.Combine(runtimeTestsuitePath.ToString(), "resources");
	}

	public static bool IsWindows() {
		if (isWindows == null) {
			isWindows = getOS() == OSType.Windows;
		}

		return isWindows;
	}

	public static OSType getOS() {
		if (detectedOS == null) {
			String os = Environment.GetEnvironmentVariable("os.name").ToLower();
			os = os.Length == 0 ? "generic" : os;

            if (os.Contains("mac") || os.Contains("darwin")) {
				detectedOS = OSType.Mac;
			}
			else if (os.Contains("win")) {
				detectedOS = OSType.Windows;
			}
			else if (os.Contains("nux")) {
				detectedOS = OSType.Linux;
			}
			else {
				detectedOS = OSType.Unknown;
			}
		}
		return detectedOS;
	}

	public static String getTextFromResource(String name) {
		try {
			if (!resourceCache.TryGetValue(name,out var text)) {
				string path = Path.Combine(resourcePath.ToString(), name);
				text = File.ReadAllText(path);
				resourceCache[name]= text;
			}
			return text;
		}
		catch (Exception ex) {
			throw new RuntimeException(ex.Message, ex);
		}
	}

	public static void checkRuleATN(Grammar g, String ruleName, String expecting) {
		Rule r = g.getRule(ruleName);
		ATNState startState = g.getATN().ruleToStartState[r.index];
		ATNPrinter serializer = new ATNPrinter(g, startState);
		String result = serializer.asString();

		Assert.AreEqual(expecting, result);
	}

	public static String joinLines(params Object[] args) {
		StringBuilder result = new StringBuilder();
		foreach (Object arg in args) {
			String str = arg.ToString();
			result.Append(str);
			if (!str.EndsWith("\n"))
				result.Append("\n");
		}
		return result.ToString();
	}
}
