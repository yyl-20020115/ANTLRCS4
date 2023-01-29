/*
 * Copyright (c) 2012-2022 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using Antlr4.StringTemplate;
using org.antlr.v4.tool;

namespace org.antlr.v4.test.runtime;

public class Generator {
	/** Write a grammar to tmpdir and run antlr */
	public static ErrorQueue antlrOnString(String workdir,
										   String targetName,
										   String grammarFileName,
										   String grammarStr,
										   bool defaultListener,
										   params String[] extraOptions)
	{
		FileUtils.mkdir(workdir);
		FileUtils.writeFile(workdir, grammarFileName, grammarStr);
		return antlrOnString(workdir, targetName, grammarFileName, defaultListener, extraOptions);
	}

	/** Run ANTLR on stuff in workdir and error queue back */
	public static ErrorQueue antlrOnString(String workdir,
										   String targetName,
										   String grammarFileName,
										   bool defaultListener,
										   params String[] extraOptions)
	{
		List<String> options = new (extraOptions);
		if ( targetName!=null ) {
			options.Add("-Dlanguage="+targetName);
		}
		if ( !options.Contains("-o") ) {
			options.Add("-o");
			options.Add(workdir);
		}
		if ( !options.Contains("-lib") ) {
			options.Add("-lib");
			options.Add(workdir);
		}
		if ( !options.Contains("-encoding") ) {
			options.Add("-encoding");
			options.Add("UTF-8");
		}
		options.Add(Path.Combine(workdir,grammarFileName));

		 String[] optionsA = new String[options.Count];
		options.ToArray();
		Tool antlr = new Tool(optionsA);
		ErrorQueue equeue = new ErrorQueue(antlr);
		antlr.addListener(equeue);
		if (defaultListener) {
			antlr.addListener(new DefaultToolListener(antlr));
		}
		antlr.processGrammarsOnCommandLine();

		List<String> errors = new ();

		if ( !defaultListener && equeue.errors.Count>0 ) {
			for (int i = 0; i < equeue.errors.Count; i++) {
				ANTLRMessage msg = equeue.errors[(i)];
				Template msgST = antlr.errMgr.getMessageTemplate(msg);
				errors.Add(msgST.Render());
			}
		}
		if ( !defaultListener && equeue.warnings.Count>0 ) {
			for (int i = 0; i < equeue.warnings.Count; i++) {
				ANTLRMessage msg = equeue.warnings[(i)];
				// antlrToolErrors.Append(msg); warnings are hushed
			}
		}

		return equeue;
	}
}
