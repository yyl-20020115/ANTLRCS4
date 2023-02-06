/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime.atn;
using org.antlr.v4.runtime.dfa;

namespace org.antlr.v4.runtime;


public class LexerInterpreter : Lexer {
	protected String grammarFileName;
	protected ATN atn;

	//@Deprecated
	protected String[] tokenNames;
	protected String[] ruleNames;
	protected String[] channelNames;
	protected String[] modeNames;


	private Vocabulary vocabulary;

	protected DFA[] _decisionToDFA;
	protected PredictionContextCache _sharedContextCache =
		new PredictionContextCache();

	//@Deprecated
	public LexerInterpreter(String grammarFileName, ICollection<String> tokenNames, ICollection<String> ruleNames, ICollection<String> modeNames, ATN atn, CharStream input) 
	: this(grammarFileName, VocabularyImpl.fromTokenNames(tokenNames.ToArray()), ruleNames, new List<String>(), modeNames, atn, input)
    {
	}

	//@Deprecated
	public LexerInterpreter(String grammarFileName, Vocabulary vocabulary, ICollection<String> ruleNames, ICollection<String> modeNames, ATN atn, CharStream input) 
	: this(grammarFileName, vocabulary, ruleNames, new List<String>(), modeNames, atn, input)
	{
		
	}

	public LexerInterpreter(String grammarFileName, Vocabulary vocabulary, ICollection<String> ruleNames, ICollection<String> channelNames, ICollection<String> modeNames, ATN atn, CharStream input)
        : base(input)
	{

		if (atn.grammarType != ATNType.LEXER) {
			throw new ArgumentException("The ATN must be a lexer ATN.");
		}

		this.grammarFileName = grammarFileName;
		this.atn = atn;
		this.tokenNames = new String[atn.maxTokenType];
		for (int i = 0; i < tokenNames.Length; i++) {
			tokenNames[i] = vocabulary.getDisplayName(i);
		}

		this.ruleNames = ruleNames.ToArray();
		this.channelNames = channelNames.ToArray();
		this.modeNames = modeNames.ToArray();
		this.vocabulary = vocabulary;

		this._decisionToDFA = new DFA[atn.NumberOfDecisions()];
		for (int i = 0; i < _decisionToDFA.Length; i++) {
			_decisionToDFA[i] = new DFA(atn.GetDecisionState(i), i);
		}
		this._interp = new LexerATNSimulator(this,atn,_decisionToDFA,_sharedContextCache);
	}

	//@Override
	public override ATN getATN() {
		return atn;
	}

    //@Override
    public override String getGrammarFileName() {
		return grammarFileName;
	}

    //@Override
    //@Deprecated
    public String[] getTokenNames() {
		return tokenNames;
	}

    //@Override
    public override String[] getRuleNames() {
		return ruleNames;
	}

    //@Override
    public String[] getChannelNames() {
		return channelNames;
	}

    //@Override
    public override String[] getModeNames() {
		return modeNames;
	}

    //@Override
    public Vocabulary getVocabulary() {
		if (vocabulary != null) {
			return vocabulary;
		}

		return base.getVocabulary();
	}

    public override TokenFactory TokenFactory { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
}
