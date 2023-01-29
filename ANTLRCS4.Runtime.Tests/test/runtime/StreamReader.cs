/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using System.Text;

namespace org.antlr.v4.test.runtime;

public class RunnableStreamReader : Runnable {
	private readonly StringBuilder buffer = new StringBuilder();
	private readonly TextReader reader;
	private readonly Thread worker;

	public RunnableStreamReader(TextReader reader) {
		this.reader = reader;
		worker = new Thread(this);
	}

	public void start() {
		worker.start();
	}

	////@Override
	public void run() {
		try {
			while (true) {
				int c = in.read();
				if (c == -1) {
					break;
				}
				if (c == '\r') {
					continue;
				}
				buffer.Append((char) c);
			}
		}
		catch (IOException ioe) {
			Console.Error.WriteLine("can't read output from process");
		}
	}

	/** wait for the thread to finish */
	public void join()  {
		worker.join();
	}

	////@Override
	public override String ToString() {
		return buffer.ToString();
	}
}
