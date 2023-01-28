/*
 * Copyright (c) 2012-2022 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using System.Text;

namespace org.antlr.v4.test.runtime;

public abstract class RuntimeTestUtils {
	public static readonly String NewLine = System.getProperty("line.separator");
	public static readonly String PathSeparator = System.getProperty("path.separator");
	public static readonly String FileSeparator = System.getProperty("file.separator");
	public static readonly String TempDirectory = System.getProperty("java.io.tmpdir");

	public static readonly Path runtimePath;
	public static readonly Path runtimeTestsuitePath;
	public static readonly Path resourcePath;

	private static readonly Map<String, String> resourceCache = new HashMap<>();
	private static OSType detectedOS;
	private static Boolean isWindows;

	static RuntimeTestUtils() {
		String locationPath = RuntimeTestUtils.getProtectionDomain().getCodeSource().getLocation().getPath();
		if (isWindows()) {
			locationPath = locationPath.replaceFirst("/", "");
		}
		Path potentialRuntimeTestsuitePath = Paths.get(locationPath, "..", "..").normalize();
		Path potentialResourcePath = Paths.get(potentialRuntimeTestsuitePath.ToString(), "resources");

		if (Files.exists(potentialResourcePath)) {
			runtimeTestsuitePath = potentialRuntimeTestsuitePath;
		}
		else {
			runtimeTestsuitePath = Paths.get("..", "runtime-testsuite").normalize();
		}

		runtimePath = Paths.get(runtimeTestsuitePath.ToString(), "..", "runtime").normalize();
		resourcePath = Paths.get(runtimeTestsuitePath.ToString(), "resources");
	}

	public static bool isWindows() {
		if (isWindows == null) {
			isWindows = getOS() == OSType.Windows;
		}

		return isWindows;
	}

	public static OSType getOS() {
		if (detectedOS == null) {
			String os = Environment.GetEnvironmentVariable("os.name", "generic").ToLower();
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

	public static synchronized String getTextFromResource(String name) {
		try {
			String text = resourceCache.get(name);
			if (text == null) {
				Path path = Paths.get(resourcePath.ToString(), name);
				text = new String(Files.readAllBytes(path));
				resourceCache.put(name, text);
			}
			return text;
		}
		catch (Exception ex) {
			throw new RuntimeException(ex);
		}
	}

	public static void checkRuleATN(Grammar g, String ruleName, String expecting) {
		Rule r = g.getRule(ruleName);
		ATNState startState = g.getATN().ruleToStartState[r.index];
		ATNPrinter serializer = new ATNPrinter(g, startState);
		String result = serializer.asString();

		assertEquals(expecting, result);
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
