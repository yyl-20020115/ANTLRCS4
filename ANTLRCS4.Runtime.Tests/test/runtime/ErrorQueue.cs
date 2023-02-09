/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */
using org.antlr.v4.runtime.dfa;
using org.antlr.v4.runtime.misc;
using org.antlr.v4.tool;
using System.Text;

namespace org.antlr.v4.test.runtime;

public class ErrorQueue : ANTLRToolListener
{
    public readonly Tool tool;
    public readonly List<string> infos = new();
    public readonly List<ANTLRMessage> errors = new();
    public readonly List<ANTLRMessage> warnings = new();
    public readonly List<ANTLRMessage> all = new();


    public ErrorQueue(Tool tool = null)
    {
        this.tool = tool;
    }

    public virtual void Info(string msg)
    {
        infos.Add(msg);
    }

    ////@Override
    public virtual void Error(ANTLRMessage msg)
    {
        errors.Add(msg);
        all.Add(msg);
    }

    ////@Override
    public virtual void Warning(ANTLRMessage msg)
    {
        warnings.Add(msg);
        all.Add(msg);
    }

    public virtual void Error(ToolMessage msg)
    {
        errors.Add(msg);
        all.Add(msg);
    }

    public int Count => all.Count + infos.Count;

    ////@Override
    public override string ToString() => ToString(false);

    public string ToString(bool rendered)
    {
        if (!rendered)
        {
            return RuntimeUtils.Join(all, "\n");
        }

        if (tool == null)
        {
            throw new IllegalStateException(
                $"No {nameof(Tool)} instance is available.");
        }

        var buffer = new StringBuilder();
        foreach (var m in all)
        {
            var st = tool.ErrMgr.GetMessageTemplate(m);
            buffer.Append(st.Render());
            buffer.Append('\n');
        }

        return buffer.ToString();
    }

}

