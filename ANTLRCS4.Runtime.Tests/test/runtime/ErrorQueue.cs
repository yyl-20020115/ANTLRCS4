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

public class ErrorQueue : ANTLRToolListener {
	public readonly Tool tool;
	public readonly List<String> infos = new ();
	public readonly List<ANTLRMessage> errors = new ();
	public readonly List<ANTLRMessage> warnings = new ();
	public readonly List<ANTLRMessage> all = new ();

	
	public ErrorQueue(Tool tool = null) {
		this.tool = tool;
	}

	////@Override
	public void info(String msg) {
		infos.Add(msg);
	}

	////@Override
	public void error(ANTLRMessage msg) {
		errors.Add(msg);
        all.Add(msg);
	}

	////@Override
	public void warning(ANTLRMessage msg) {
		warnings.Add(msg);
        all.Add(msg);
	}

	public void error(ToolMessage msg) {
		errors.Add(msg);
		all.Add(msg);
	}

	public int size() {
		return all.Count + infos.Count;
	}

	////@Override
	public override String ToString() {
		return toString(false);
	}

	public String toString(bool rendered) {
		if (!rendered) {
			return RuntimeUtils.join(all, "\n");
		}

		if (tool == null) {
			throw new IllegalStateException(String.format("No %s instance is available.", typeof(Tool).Name));
		}

		StringBuilder buf = new StringBuilder();
		foreach (ANTLRMessage m in all) {
			ST st = tool.errMgr.getMessageTemplate(m);
			buf.Append(st.render());
			buf.Append("\n");
		}

		return buf.ToString();
	}

}

