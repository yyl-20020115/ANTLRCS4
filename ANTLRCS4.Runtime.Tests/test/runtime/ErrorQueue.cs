/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */
using org.antlr.v4.runtime.misc;
using System.Text;

namespace org.antlr.v4.test.runtime;

public class ErrorQueue : ANTLRToolListener {
	public readonly Tool tool;
	public readonly List<String> infos = new ArrayList<String>();
	public readonly List<ANTLRMessage> errors = new ArrayList<ANTLRMessage>();
	public readonly List<ANTLRMessage> warnings = new ArrayList<ANTLRMessage>();
	public readonly List<ANTLRMessage> all = new ArrayList<ANTLRMessage>();

	public ErrorQueue() {
		this(null);
	}

	public ErrorQueue(Tool tool) {
		this.tool = tool;
	}

	////@Override
	public void info(String msg) {
		infos.Add(msg);
	}

	////@Override
	public void error(ANTLRMessage msg) {
		errors.add(msg);
        all.add(msg);
	}

	////@Override
	public void warning(ANTLRMessage msg) {
		warnings.add(msg);
        all.add(msg);
	}

	public void error(ToolMessage msg) {
		errors.add(msg);
		all.add(msg);
	}

	public int size() {
		return all.size() + infos.size();
	}

	////@Override
	public String ToString() {
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
		for (ANTLRMessage in all) {
			ST st = tool.errMgr.getMessageTemplate(m);
			buf.Append(st.render());
			buf.Append("\n");
		}

		return buf.ToString();
	}

}

