/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */
using org.antlr.v4.runtime.atn;
using org.antlr.v4.runtime.misc;

namespace org.antlr.v4.runtime.tree.xpath;


/** Mimic the old XPathLexer from .g4 file */
public class XPathLexer : Lexer
{
    public const int
        TOKEN_REF = 1,
        RULE_REF = 2,
        ANYWHERE = 3,
        ROOT = 4,
        WILDCARD = 5,
        BANG = 6,
        ID = 7,
        STRING = 8;
    public readonly static string[] modeNames = {
        "DEFAULT_MODE"
    };

    public static readonly string[] ruleNames = {
        "ANYWHERE", "ROOT", "WILDCARD", "BANG", "ID", "NameChar", "NameStartChar",
        "STRING"
    };

    private static readonly string[] _LITERAL_NAMES = {
        null, null, null, "'//'", "'/'", "'*'", "'!'"
    };
    private static readonly string[] _SYMBOLIC_NAMES = {
        null, "TOKEN_REF", "RULE_REF", "ANYWHERE", "ROOT", "WILDCARD", "BANG",
        "ID", "STRING"
    };
    public static readonly Vocabulary VOCABULARY = new VocabularyImpl(_LITERAL_NAMES, _SYMBOLIC_NAMES);

    /**
	 * @deprecated Use {@link #VOCABULARY} instead.
	 */
    //@Deprecated
    public static readonly string[] tokenNames = new string[_SYMBOLIC_NAMES.Length];
    static XPathLexer()
    {
        for (int i = 0; i < tokenNames.Length; i++)
        {
            tokenNames[i] = VOCABULARY.GetLiteralName(i);
            if (tokenNames[i] == null)
            {
                tokenNames[i] = VOCABULARY.GetSymbolicName(i);
            }

            if (tokenNames[i] == null)
            {
                tokenNames[i] = "<INVALID>";
            }
        }
    }

    //@Override
    public override string GetGrammarFileName() { return "XPathLexer.g4"; }

    //@Override
    public override string[] GetRuleNames() { return ruleNames; }

    //@Override
    public override string[] GetModeNames() { return modeNames; }

    //@Override
    //@Deprecated
    public override string[] GetTokenNames()
    {
        return tokenNames;
    }

    //@Override
    public virtual Vocabulary getVocabulary()
    {
        return VOCABULARY;
    }

    //@Override
    public override ATN GetATN()
    {
        return null;
    }

    protected int line = 1;
    protected int charPositionInLine = 0;

    public XPathLexer(CharStream input) : base(input) { }

    //@Override
    public override Token NextToken()
    {
        _tokenStartCharIndex = input.Index;
        CommonToken t = null;
        while (t == null)
        {
            switch (input.LA(1))
            {
                case '/':
                    Consume();
                    if (input.LA(1) == '/')
                    {
                        Consume();
                        t = new CommonToken(ANYWHERE, "//");
                    }
                    else
                    {
                        t = new CommonToken(ROOT, "/");
                    }
                    break;
                case '*':
                    Consume();
                    t = new CommonToken(WILDCARD, "*");
                    break;
                case '!':
                    Consume();
                    t = new CommonToken(BANG, "!");
                    break;
                case '\'':
                    String s = matchString();
                    t = new CommonToken(STRING, s);
                    break;
                case CharStream.EOF:
                    return new CommonToken(EOF, "<EOF>");
                default:
                    if (isNameStartChar(input.LA(1)))
                    {
                        String id = matchID();
                        if (char.IsUpper(id[0])) t = new CommonToken(TOKEN_REF, id);
                        else t = new CommonToken(RULE_REF, id);
                    }
                    else
                    {
                        throw new LexerNoViableAltException(this, input, _tokenStartCharIndex, null);
                    }
                    break;
            }
        }
        t.StartIndex = _tokenStartCharIndex;
        t.CharPositionInLine = _tokenStartCharIndex;
        t.Line = line;
        return t;
    }

    public void Consume()
    {
        int curChar = input.LA(1);
        if (curChar == '\n')
        {
            line++;
            charPositionInLine = 0;
        }
        else
        {
            charPositionInLine++;
        }
        input.Consume();
    }

    //@Override
    public int getCharPositionInLine()
    {
        return charPositionInLine;
    }

    public String matchID()
    {
        int start = input.Index;
        Consume(); // drop start char
        while (isNameChar(input.LA(1)))
        {
            Consume();
        }
        return input.GetText(Interval.Of(start, input.Index - 1));
    }

    public String matchString()
    {
        int start = input.Index;
        Consume(); // drop first quote
        while (input.LA(1) != '\'')
        {
            Consume();
        }
        Consume(); // drop last quote
        return input.GetText(Interval.Of(start, input.Index - 1));
    }

    public bool isNameChar(int c) { return char.IsLetterOrDigit((char)c); }

    public bool isNameStartChar(int c) { return c == '_' || char.IsLetter((char)c); }
}
