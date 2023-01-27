/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime.atn;
using org.antlr.v4.runtime.tree.pattern;

namespace org.antlr.v4.runtime.misc;

// A class to read plain text interpreter data produced by ANTLR.
public class InterpreterDataReader {

	public class InterpreterData {
	  public ATN atn;
        public Vocabulary vocabulary;
        public List<String> ruleNames;
        public List<String> channels; // Only valid for lexer grammars.
        public List<String> modes; // ditto
	};

	/**
	 * The structure of the data file is very simple. Everything is line based with empty lines
	 * separating the different parts. For lexers the layout is:
	 * token literal names:
	 * ...
	 *
	 * token symbolic names:
	 * ...
	 *
	 * rule names:
	 * ...
	 *
	 * channel names:
	 * ...
	 *
	 * mode names:
	 * ...
	 *
	 * atn:
	 * <a single line with comma separated int values> enclosed in a pair of squared brackets.
	 *
	 * Data for a parser does not contain channel and mode names.
	 */
	public static InterpreterData parseFile(String fileName) {
		InterpreterData result = new InterpreterData();
		result.ruleNames = new ();

		using (var br = new StreamReader(fileName)) {
		    String line;
		  	List<String> literalNames = new();
		  	List<String> symbolicNames = new();

			line = br.ReadLine();
			if ( !line.Equals("token literal names:") )
				throw new RuntimeException("Unexpected data entry");
		    while ((line = br.ReadLine()) != null) {
		       if ( line.Length == 0 )
					break;
				literalNames.Add(line.Equals("null") ? "" : line);
		    }

			line = br.ReadLine();
			if ( !line.Equals("token symbolic names:") )
				throw new RuntimeException("Unexpected data entry");
		    while ((line = br.ReadLine()) != null) {
		       if ( line.Length == 0 )
					break;
				symbolicNames.Add(line.Equals("null") ? "" : line);
		    }

		  	result.vocabulary = new VocabularyImpl(literalNames.ToArray(), symbolicNames.ToArray());

			line = br.ReadLine();
			if ( !line.Equals("rule names:") )
				throw new RuntimeException("Unexpected data entry");
		    while ((line = br.ReadLine()) != null) {
		       if ( line.Length==0 )
					break;
				result.ruleNames.Add(line);
		    }

			line = br.ReadLine();
			if ( line.Equals("channel names:") ) { // Additional lexer data.
				result.channels = new ();
			    while ((line = br.ReadLine()) != null) {
			       if ( line.Length==0 )
						break;
					result.channels.Add(line);
			    }

				line = br.ReadLine();
				if ( !line.Equals("mode names:") )
					throw new RuntimeException("Unexpected data entry");
				result.modes = new ();
			    while ((line = br.ReadLine()) != null) {
			       if ( line.Length == 0 )
						break;
					result.modes.Add(line);
			    }
			}

		  	line = br.ReadLine();
		  	if ( !line.Equals("atn:") )
		  		throw new RuntimeException("Unexpected data entry");
			line = br.ReadLine();
			String[] elements = line[1..^1].Split(",");
	  		int[] serializedATN = new int[elements.Length];

			for (int i = 0; i < elements.Length; ++i) { // ignore [...] on ends
				serializedATN[i] = Integer.parseInt(elements[i].Trim());
			}

		  	ATNDeserializer deserializer = new ATNDeserializer();
		  	result.atn = deserializer.deserialize(serializedATN);
		}
		//catch (IOException e) {
		//	// We just swallow the error and return empty objects instead.
		//}

		return result;
	}

}
