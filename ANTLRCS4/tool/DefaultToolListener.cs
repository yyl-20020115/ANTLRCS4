/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

namespace org.antlr.v4.tool;

/** */
public class DefaultToolListener : ANTLRToolListener {
	public Tool tool;

	public DefaultToolListener(Tool tool) { this.tool = tool; }

	//@Override
	public void info(String msg) {
		if (tool.errMgr.formatWantsSingleLineMessage()) {
			msg = msg.Replace('\n', ' ');
		}
		Console.Out.WriteLine(msg);
	}

	//@Override
	public void error(ANTLRMessage msg) {
		ST msgST = tool.errMgr.getMessageTemplate(msg);
		String outputMsg = msgST.render();
		if (tool.errMgr.formatWantsSingleLineMessage()) {
			outputMsg = outputMsg.Replace('\n', ' ');
		}
		Console.Error.WriteLine(outputMsg);
	}

	//@Override
	public void warning(ANTLRMessage msg) {
		ST msgST = tool.errMgr.getMessageTemplate(msg);
		String outputMsg = msgST.render();
		if (tool.errMgr.formatWantsSingleLineMessage()) {
			outputMsg = outputMsg.Replace('\n', ' ');
		}
		Console.Error.WriteLine(outputMsg);
	}
}
