/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.tool;

namespace org.antlr.v4.codegen.model;


public class Lexer : Recognizer
{
    public ICollection<string> channelNames;
    public Dictionary<string, int> escapedChannels;
    public LexerFile file;
    public ICollection<string> modes;
    public ICollection<string> escapedModeNames;

    [ModelElement]
    public Dictionary<Rule, RuleActionFunction> actionFuncs = new();

    public Lexer(OutputModelFactory factory, LexerFile file) : base(factory)
    {
        this.file = file; // who contains us?

        var g = factory.Grammar;
        var target = factory.Generator.Target;

        escapedChannels = new();
        channelNames = new List<string>();
        foreach (var key in g.channelNameToValueMap.Keys)
        {
            int value = g.channelNameToValueMap[key];
            escapedChannels[target.EscapeIfNeeded(key)] = value;
            channelNames.Add(key);
        }

        modes = (g as LexerGrammar).modes.Keys;
        escapedModeNames = new List<string>(modes.Count);
        foreach (var mode in modes)
        {
            escapedModeNames.Add(target.EscapeIfNeeded(mode));
        }
    }
}
