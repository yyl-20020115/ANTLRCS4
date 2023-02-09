/*
 * Copyright (c) 2012-2022 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using Antlr4.StringTemplate;
using org.antlr.v4.tool;

namespace org.antlr.v4.test.runtime;

public static class Generator
{
    /** Write a grammar to tmpdir and run antlr */
    public static ErrorQueue AntlrOnString(string workdir,
                                           string targetName,
                                           string grammarFileName,
                                           string grammarStr,
                                           bool defaultListener,
                                           params string[] extraOptions)
    {
        FileUtils.MakeDirectory(workdir);
        FileUtils.WriteFile(workdir, grammarFileName, grammarStr);
        return AntlrOnString(workdir, targetName, grammarFileName, defaultListener, extraOptions);
    }

    /** Run ANTLR on stuff in workdir and error queue back */
    public static ErrorQueue AntlrOnString(string workdir,
                                           string targetName,
                                           string grammarFileName,
                                           bool defaultListener,
                                           params string[] extraOptions)
    {
        List<string> options = new(extraOptions);
        if (targetName != null)
        {
            options.Add("-Dlanguage=" + targetName);
        }
        if (!options.Contains("-o"))
        {
            options.Add("-o");
            options.Add(workdir);
        }
        if (!options.Contains("-lib"))
        {
            options.Add("-lib");
            options.Add(workdir);
        }
        if (!options.Contains("-encoding"))
        {
            options.Add("-encoding");
            options.Add("UTF-8");
        }
        options.Add(Path.Combine(workdir, grammarFileName));

        var optionsA = new string[options.Count];
        options.ToArray();
        var antlr = new Tool(optionsA);
        var equeue = new ErrorQueue(antlr);
        antlr.addListener(equeue);
        if (defaultListener)
        {
            antlr.addListener(new DefaultToolListener(antlr));
        }
        antlr.ProcessGrammarsOnCommandLine();

        List<string> errors = new();

        if (!defaultListener && equeue.errors.Count > 0)
        {
            for (int i = 0; i < equeue.errors.Count; i++)
            {
                var msg = equeue.errors[(i)];
                var msgST = antlr.ErrMgr.GetMessageTemplate(msg);
                errors.Add(msgST.Render());
            }
        }
        if (!defaultListener && equeue.warnings.Count > 0)
        {
            for (int i = 0; i < equeue.warnings.Count; i++)
            {
                var msg = equeue.warnings[(i)];
                // antlrToolErrors.Append(msg); warnings are hushed
            }
        }

        return equeue;
    }
}
