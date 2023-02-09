/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

namespace org.antlr.v4.tool;

/** */
public class DefaultToolListener : ANTLRToolListener
{
    public Tool tool;

    public DefaultToolListener(Tool tool) => this.tool = tool;

    public void Info(string msg)
    {
        if (tool.ErrMgr.FormatWantsSingleLineMessage())
        {
            msg = msg.Replace('\n', ' ');
        }
        Console.Out.WriteLine(msg);
    }

    public void Error(ANTLRMessage msg)
    {
        var msgST = tool.ErrMgr.GetMessageTemplate(msg);
        var outputMsg = msgST.Render();
        if (tool.ErrMgr.FormatWantsSingleLineMessage())
        {
            outputMsg = outputMsg.Replace('\n', ' ');
        }
        Console.Error.WriteLine(outputMsg);
    }

    public void Warning(ANTLRMessage msg)
    {
        var msgST = tool.ErrMgr.GetMessageTemplate(msg);
        var outputMsg = msgST.Render();
        if (tool.ErrMgr.FormatWantsSingleLineMessage())
        {
            outputMsg = outputMsg.Replace('\n', ' ');
        }
        Console.Error.WriteLine(outputMsg);
    }
}
