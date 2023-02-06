/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */
using org.antlr.v4.runtime.atn;
using org.antlr.v4.runtime.misc;

namespace org.antlr.v4.runtime.tree.xpath;


/** Mimic the old XPathLexer from .g4 file */
public class XPathLexer : Lexer {
	public const int
		TOKEN_REF=1, 
		RULE_REF=2, 
		ANYWHERE=3, 
		ROOT=4, 
		WILDCARD=5, 
		BANG=6, 
		ID=7,
		STRING=8;
	public readonly static String[] modeNames = {
		"DEFAULT_MODE"
	};

	public static readonly String[] ruleNames = {
		"ANYWHERE", "ROOT", "WILDCARD", "BANG", "ID", "NameChar", "NameStartChar",
		"STRING"
	};

	private static readonly String[] _LITERAL_NAMES = {
		null, null, null, "'//'", "'/'", "'*'", "'!'"
	};
	private static readonly String[] _SYMBOLIC_NAMES = {
		null, "TOKEN_REF", "RULE_REF", "ANYWHERE", "ROOT", "WILDCARD", "BANG",
		"ID", "STRING"
	};
	public static readonly Vocabulary VOCABULARY = new VocabularyImpl(_LITERAL_NAMES, _SYMBOLIC_NAMES);

	/**
	 * @deprecated Use {@link #VOCABULARY} instead.
	 */
	//@Deprecated
	public static String[] tokenNames = new String[_SYMBOLIC_NAMES.Length];
    static XPathLexer(){
		for (int i = 0; i < tokenNames.Length; i++) {
			tokenNames[i] = VOCABULARY.getLiteralName(i);
			if (tokenNames[i] == null) {
				tokenNames[i] = VOCABULARY.getSymbolicName(i);
			}

			if (tokenNames[i] == null) {
				tokenNames[i] = "<INVALID>";
			}
		}
	}

	//@Override
	public override String getGrammarFileName() { return "XPathLexer.g4"; }

    //@Override
    public override String[] getRuleNames() { return ruleNames; }

    //@Override
    public override String[] getModeNames() { return modeNames; }

    //@Override
    //@Deprecated
    public override String[] getTokenNames() {
		return tokenNames;
	}

    //@Override
    public virtual Vocabulary getVocabulary() {
		return VOCABULARY;
	}

    //@Override
    public override ATN getATN() {
		return null;
	}

	protected int line = 1;
	protected int charPositionInLine = 0;

	public XPathLexer(CharStream input):base(input)
    {
	}

	//@Override
	public override Token NextToken() {
		_tokenStartCharIndex = input.Index();
		CommonToken t = null;
		while ( t==null ) {
			switch ( input.LA(1) ) {
				case '/':
					consume();
					if ( input.LA(1)=='/' ) {
						consume();
						t = new CommonToken(ANYWHERE, "//");
					}
					else {
						t = new CommonToken(ROOT, "/");
					}
					break;
				case '*':
					consume();
					t = new CommonToken(WILDCARD, "*");
					break;
				case '!':
					consume();
					t = new CommonToken(BANG, "!");
					break;
				case '\'':
					String s = matchString();
					t = new CommonToken(STRING, s);
					break;
				case CharStream.EOF :
					return new CommonToken(EOF, "<EOF>");
				default:
					if ( isNameStartChar(input.LA(1)) ) {
						String id = matchID();
						if ( char.IsUpper(id[0]) ) t = new CommonToken(TOKEN_REF, id);
						else t = new CommonToken(RULE_REF, id);
					}
					else {
						throw new LexerNoViableAltException(this, input, _tokenStartCharIndex, null);
					}
					break;
			}
		}
		t.setStartIndex(_tokenStartCharIndex);
		t.setCharPositionInLine(_tokenStartCharIndex);
		t.setLine(line);
		return t;
	}

	public void consume() {
		int curChar = input.LA(1);
		if ( curChar=='\n' ) {
			line++;
			charPositionInLine=0;
		}
		else {
			charPositionInLine++;
		}
		input.Consume();
	}

	//@Override
	public int getCharPositionInLine() {
		return charPositionInLine;
	}

	public String matchID() {
		int start = input.Index();
		consume(); // drop start char
		while ( isNameChar(input.LA(1)) ) {
			consume();
		}
		return input.GetText(Interval.Of(start,input.Index()-1));
	}

	public String matchString() {
		int start = input.Index();
		consume(); // drop first quote
		while ( input.LA(1)!='\'' ) {
			consume();
		}
		consume(); // drop last quote
		return input.GetText(Interval.Of(start,input.Index()-1));
	}

	public bool isNameChar(int c) { return char.IsLetterOrDigit((char)c); }

	public bool isNameStartChar(int c) { return c =='_'|| char.IsLetter((char)c); }
}
