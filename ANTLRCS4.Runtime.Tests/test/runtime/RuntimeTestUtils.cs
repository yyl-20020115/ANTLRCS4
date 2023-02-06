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

public abstract class RuntimeTestUtils
{
    public static readonly string NewLine = Environment.NewLine;
    public static readonly string PathSeparator = Path.PathSeparator.ToString();// System.getProperty("path.separator");
    public static readonly string FileSeparator = Path.PathSeparator.ToString();// System.getProperty("file.separator");
    public static readonly string TempDirectory = Environment.CurrentDirectory;// System.getProperty("java.io.tmpdir");

    public static readonly string runtimePath;
    public static readonly string runtimeTestsuitePath;
    public static readonly string resourcePath;

    private static readonly Dictionary<string, string> resourceCache = new();
    private static OSType detectedOS = OSType.Unknown;
    private static bool isWindows = true;

    static RuntimeTestUtils()
    {
        var locationPath = Environment.CurrentDirectory;
        //if (IsWindows()) {
        //	locationPath = locationPath.ReplaceFirst("/", "");
        //}
        var potentialRuntimeTestsuitePath = Path.Combine(locationPath, "..", "..");
        var potentialResourcePath = Path.Combine(potentialRuntimeTestsuitePath.ToString(), "resources");

        if (File.Exists(potentialResourcePath))
        {
            runtimeTestsuitePath = potentialRuntimeTestsuitePath;
        }
        else
        {
            runtimeTestsuitePath = Path.Combine("..", "runtime-testsuite");
        }

        runtimePath = Path.Combine(runtimeTestsuitePath.ToString(), "..", "runtime");
        resourcePath = Path.Combine(runtimeTestsuitePath.ToString(), "resources");
    }

    public static bool IsWindows()
    {
        if (isWindows)
        {
            isWindows = GetOS() == OSType.Windows;
        }

        return isWindows;
    }

    public static OSType GetOS()
    {
        if (detectedOS == OSType.Unknown)
        {
            var os = Environment.GetEnvironmentVariable("os.name").ToLower();
            os = os.Length == 0 ? "generic" : os;

            if (os.Contains("mac") || os.Contains("darwin"))
            {
                detectedOS = OSType.Mac;
            }
            else if (os.Contains("win"))
            {
                detectedOS = OSType.Windows;
            }
            else if (os.Contains("nux"))
            {
                detectedOS = OSType.Linux;
            }
            else
            {
                detectedOS = OSType.Unknown;
            }
        }
        return detectedOS;
    }

    public static string GetTextFromResource(string name)
    {
        try
        {
            if (!resourceCache.TryGetValue(name, out var text))
            {
                var path = Path.Combine(resourcePath.ToString(), name);
                text = File.ReadAllText(path);
                resourceCache[name] = text;
            }
            return text;
        }
        catch (Exception ex)
        {
            throw new RuntimeException(ex.Message, ex);
        }
    }

    public static void CheckRuleATN(Grammar g, string ruleName, string expecting)
    {
        var r = g.getRule(ruleName);
        var startState = g.getATN().ruleToStartState[r.index];
        var serializer = new ATNPrinter(g, startState);
        var result = serializer.AsString();

        Assert.AreEqual(expecting, result);
    }

    public static string JoinLines(params object[] args)
    {
        var result = new StringBuilder();
        foreach (var arg in args)
        {
            var str = arg.ToString();
            result.Append(str);
            if (!str.EndsWith("\n"))
                result.Append('\n');
        }
        return result.ToString();
    }
}
