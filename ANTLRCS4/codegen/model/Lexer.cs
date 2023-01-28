/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.tool;

namespace org.antlr.v4.codegen.model;


public class Lexer : Recognizer {
	public ICollection<String> channelNames;
	public Dictionary<String, int> escapedChannels;
	public LexerFile file;
	public ICollection<String> modes;
	public ICollection<String> escapedModeNames;

	[ModelElement]
	public Dictionary<Rule, RuleActionFunction> actionFuncs =
		new ();

	public Lexer(OutputModelFactory factory, LexerFile file):base(factory)
    {
		this.file = file; // who contains us?

		Grammar g = factory.getGrammar();
		Target target = factory.getGenerator().getTarget();

		escapedChannels = new ();
		channelNames = new List<string>();
		foreach (String key in g.channelNameToValueMap.Keys) {
			int value = g.channelNameToValueMap[key];
			escapedChannels[target.escapeIfNeeded(key)]= value;
			channelNames.Add(key);
		}

		modes = (g as LexerGrammar).modes.Keys;
		escapedModeNames = new List<String>(modes.Count);
		foreach (String mode in modes) {
			escapedModeNames.Add(target.escapeIfNeeded(mode));
		}
	}
}
