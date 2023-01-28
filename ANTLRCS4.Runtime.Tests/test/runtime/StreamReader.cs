/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using System.Text;

namespace org.antlr.v4.test.runtime;

public class StreamReader : Runnable {
	private readonly StringBuilder buffer = new StringBuilder();
	private readonly BufferedReader @in;
	private readonly Thread worker;

	public StreamReader(InputStream @in) {
		this.in = new BufferedReader(new InputStreamReader(@in, StandardCharsets.UTF_8) );
		worker = new Thread(this);
	}

	public void start() {
		worker.start();
	}

	//@Override
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
				buffer.append((char) c);
			}
		}
		catch (IOException ioe) {
			System.err.println("can't read output from process");
		}
	}

	/** wait for the thread to finish */
	public void join()  {
		worker.join();
	}

	//@Override
	public String toString() {
		return buffer.toString();
	}
}
