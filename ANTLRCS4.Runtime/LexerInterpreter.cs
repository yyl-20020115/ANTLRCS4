/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime.atn;
using org.antlr.v4.runtime.dfa;

namespace org.antlr.v4.runtime;


public class LexerInterpreter : Lexer
{
    protected string grammarFileName;
    protected ATN atn;

    //@Deprecated
    protected string[] tokenNames;
    protected string[] ruleNames;
    protected string[] channelNames;
    protected string[] modeNames;


    private Vocabulary vocabulary;

    protected DFA[] _decisionToDFA;
    protected PredictionContextCache _sharedContextCache =
        new PredictionContextCache();

    //@Deprecated
    public LexerInterpreter(string grammarFileName, ICollection<string> tokenNames, ICollection<string> ruleNames, ICollection<String> modeNames, ATN atn, CharStream input)
    : this(grammarFileName, VocabularyImpl.fromTokenNames(tokenNames.ToArray()), ruleNames, new List<String>(), modeNames, atn, input)
    {
    }

    //@Deprecated
    public LexerInterpreter(string grammarFileName, Vocabulary vocabulary, ICollection<string> ruleNames, ICollection<String> modeNames, ATN atn, CharStream input)
    : this(grammarFileName, vocabulary, ruleNames, new List<String>(), modeNames, atn, input)
    {

    }

    public LexerInterpreter(string grammarFileName, Vocabulary vocabulary, ICollection<string> ruleNames, ICollection<String> channelNames, ICollection<String> modeNames, ATN atn, CharStream input)
        : base(input)
    {

        if (atn.grammarType != ATNType.LEXER)
        {
            throw new ArgumentException("The ATN must be a lexer ATN.");
        }

        this.grammarFileName = grammarFileName;
        this.atn = atn;
        this.tokenNames = new String[atn.maxTokenType];
        for (int i = 0; i < tokenNames.Length; i++)
        {
            tokenNames[i] = vocabulary.GetDisplayName(i);
        }

        this.ruleNames = ruleNames.ToArray();
        this.channelNames = channelNames.ToArray();
        this.modeNames = modeNames.ToArray();
        this.vocabulary = vocabulary;

        this._decisionToDFA = new DFA[atn.NumberOfDecisions()];
        for (int i = 0; i < _decisionToDFA.Length; i++)
        {
            _decisionToDFA[i] = new DFA(atn.GetDecisionState(i), i);
        }
        this._interp = new LexerATNSimulator(this, atn, _decisionToDFA, _sharedContextCache);
    }

    //@Override
    public override ATN GetATN()
    {
        return atn;
    }

    //@Override
    public override String GetGrammarFileName()
    {
        return grammarFileName;
    }

    //@Override
    //@Deprecated
    public String[] GetTokenNames()
    {
        return tokenNames;
    }

    //@Override
    public override String[] GetRuleNames()
    {
        return ruleNames;
    }

    //@Override
    public String[] GetChannelNames()
    {
        return channelNames;
    }

    //@Override
    public override String[] GetModeNames()
    {
        return modeNames;
    }

    //@Override
    public override Vocabulary GetVocabulary()
    {
        if (vocabulary != null)
        {
            return vocabulary;
        }

        return base.GetVocabulary();
    }

    public override TokenFactory TokenFactory { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
}
