/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime;
using org.antlr.v4.runtime.atn;
using org.antlr.v4.runtime.dfa;
using org.antlr.v4.runtime.misc;
using org.antlr.v4.tool;
using System.Text;

namespace org.antlr.v4.test.tool;

/**
 * Lexer rules are little quirky when it comes to wildcards. Problem
 * stems from the fact that we want the longest match to win among
 * several rules and even within a rule. However, that conflicts
 * with the notion of non-greedy, which by definition tries to match
 * the fewest possible. During ATN construction, non-greedy loops
 * have their entry and exit branches reversed so that the ATN
 * simulator will see the exit branch 1st, giving it a priority. The
 * 1st path to the stop state kills any other paths for that rule
 * that begin with the wildcard. In general, this does everything we
 * want, but occasionally there are some quirks as you'll see from
 * the tests below.
 */
[TestClass]
public class TestATNLexerInterpreter
{
    [TestMethod]
    public void TestLexerTwoRules()
    {
        var lg = new LexerGrammar(
            "lexer grammar L;\n" +
            "A : 'a' ;\n" +
            "B : 'b' ;\n");
        var expecting = "A, B, A, B, EOF";
        CheckLexerMatches(lg, "abab", expecting);
    }

    [TestMethod]
    public void TestShortLongRule()
    {
        var lg = new LexerGrammar(
            "lexer grammar L;\n" +
            "A : 'xy'\n" +
            "  | 'xyz'\n" + // this alt is preferred since there are no non-greedy configs
            "  ;\n" +
            "Z : 'z'\n" +
            "  ;\n");
        CheckLexerMatches(lg, "xy", "A, EOF");
        CheckLexerMatches(lg, "xyz", "A, EOF");
    }

    [TestMethod]
    public void TestShortLongRule2()
    {
        var lg = new LexerGrammar(
            "lexer grammar L;\n" +
            "A : 'xyz'\n" +  // make sure nongreedy mech cut off doesn't kill this alt
            "  | 'xy'\n" +
            "  ;\n");
        CheckLexerMatches(lg, "xy", "A, EOF");
        CheckLexerMatches(lg, "xyz", "A, EOF");
    }

    [TestMethod]
    public void TestWildOnEndFirstAlt()
    {
        var lg = new LexerGrammar(
            "lexer grammar L;\n" +
            "A : 'xy' .\n" + // should pursue '.' since xyz hits stop first, before 2nd alt
            "  | 'xy'\n" +
            "  ;\n" +
            "Z : 'z'\n" +
            "  ;\n");
        CheckLexerMatches(lg, "xy", "A, EOF");
        CheckLexerMatches(lg, "xyz", "A, EOF");
    }

    [TestMethod]
    public void TestWildOnEndLastAlt()
    {
        var lg = new LexerGrammar(
            "lexer grammar L;\n" +
            "A : 'xy'\n" +
            "  | 'xy' .\n" +  // this alt is preferred since there are no non-greedy configs
            "  ;\n" +
            "Z : 'z'\n" +
            "  ;\n");
        CheckLexerMatches(lg, "xy", "A, EOF");
        CheckLexerMatches(lg, "xyz", "A, EOF");
    }

    [TestMethod]
    public void TestWildcardNonQuirkWhenSplitBetweenTwoRules()
    {
        var lg = new LexerGrammar(
            "lexer grammar L;\n" +
            "A : 'xy' ;\n" +
            "B : 'xy' . 'z' ;\n");
        CheckLexerMatches(lg, "xy", "A, EOF");
        CheckLexerMatches(lg, "xyqz", "B, EOF");
    }

    [TestMethod]
    public void TestLexerLoops()
    {
        var lg = new LexerGrammar(
            "lexer grammar L;\n" +
            "INT : '0'..'9'+ ;\n" +
            "ID : 'a'..'z'+ ;\n");
        var expecting = "ID, INT, ID, INT, EOF";
        CheckLexerMatches(lg, "a34bde3", expecting);
    }

    [TestMethod]
    public void TestLexerNotSet()
    {
        var lg = new LexerGrammar(
            "lexer grammar L;\n" +
            "ID : ~('a'|'b')\n ;");
        var expecting = "ID, EOF";
        CheckLexerMatches(lg, "c", expecting);
    }

    [TestMethod]
    public void TestLexerSetUnicodeBMP()
    {
        var lg = new LexerGrammar(
            "lexer grammar L;\n" +
            "ID : ('\u611B'|'\u611C')\n ;");
        var expecting = "ID, EOF";
        CheckLexerMatches(lg, "\u611B", expecting);
    }

    [TestMethod]
    public void TestLexerNotSetUnicodeBMP()
    {
        var lg = new LexerGrammar(
            "lexer grammar L;\n" +
            "ID : ~('\u611B'|'\u611C')\n ;");
        var expecting = "ID, EOF";
        CheckLexerMatches(lg, "\u611D", expecting);
    }

    [TestMethod]
    public void TestLexerNotSetUnicodeBMPMatchesSMP()
    {
        var lg = new LexerGrammar(
            "lexer grammar L;\n" +
            "ID : ~('\u611B'|'\u611C')\n ;");
        var expecting = "ID, EOF";
        CheckLexerMatches(lg, new StringBuilder().Append(char.ConvertFromUtf32(0x1F4A9)).ToString(), expecting);
    }

    [TestMethod]
    public void TestLexerSetUnicodeSMP()
    {
        var lg = new LexerGrammar(
            "lexer grammar L;\n" +
            "ID : ('\\u{1F4A9}'|'\\u{1F4AA}')\n ;");
        var expecting = "ID, EOF";
        CheckLexerMatches(lg, new StringBuilder().Append(char.ConvertFromUtf32(0x1F4A9)).ToString(), expecting);
    }

    [TestMethod]
    public void TestLexerNotBMPSetMatchesUnicodeSMP()
    {
        var lg = new LexerGrammar(
            "lexer grammar L;\n" +
            "ID : ~('a'|'b')\n ;");
        var expecting = "ID, EOF";
        CheckLexerMatches(lg, new StringBuilder().Append(char.ConvertFromUtf32(0x1F4A9)).ToString(), expecting);
    }

    [TestMethod]
    public void TestLexerNotBMPSetMatchesBMP()
    {
        var lg = new LexerGrammar(
            "lexer grammar L;\n" +
            "ID : ~('a'|'b')\n ;");
        var expecting = "ID, EOF";
        CheckLexerMatches(lg, "\u611B", expecting);
    }

    [TestMethod]
    public void TestLexerNotBMPSetMatchesSMP()
    {
        var lg = new LexerGrammar(
            "lexer grammar L;\n" +
            "ID : ~('a'|'b')\n ;");
        var expecting = "ID, EOF";
        CheckLexerMatches(lg, new StringBuilder().Append(char.ConvertFromUtf32(0x1F4A9)).ToString(), expecting);
    }

    [TestMethod]
    public void TestLexerNotSMPSetMatchesBMP()
    {
        var lg = new LexerGrammar(
            "lexer grammar L;\n" +
            "ID : ~('\\u{1F4A9}'|'\\u{1F4AA}')\n ;");
        var expecting = "ID, EOF";
        CheckLexerMatches(lg, "\u611B", expecting);
    }

    [TestMethod]
    public void TestLexerNotSMPSetMatchesSMP()
    {
        var lg = new LexerGrammar(
            "lexer grammar L;\n" +
            "ID : ~('\\u{1F4A9}'|'\\u{1F4AA}')\n ;");
        var expecting = "ID, EOF";
        CheckLexerMatches(lg, new StringBuilder().Append(char.ConvertFromUtf32(0x1D7C0)).ToString(), expecting);
    }

    [TestMethod]
    public void TestLexerRangeUnicodeSMP()
    {
        var lg = new LexerGrammar(
            "lexer grammar L;\n" +
            "ID : ('\\u{1F4A9}'..'\\u{1F4B0}')\n ;");
        var expecting = "ID, EOF";
        CheckLexerMatches(lg, new StringBuilder().Append(char.ConvertFromUtf32(0x1F4AF)).ToString(), expecting);
    }

    [TestMethod]
    public void TestLexerRangeUnicodeBMPToSMP()
    {
        var lg = new LexerGrammar(
            "lexer grammar L;\n" +
            "ID : ('\\u611B'..'\\u{1F4B0}')\n ;");
        var expecting = "ID, EOF";
        CheckLexerMatches(lg, new StringBuilder().Append(char.ConvertFromUtf32(0x12001)).ToString(), expecting);
    }

    [TestMethod]
    public void TestLexerKeywordIDAmbiguity()
    {
        var lg = new LexerGrammar(
            "lexer grammar L;\n" +
            "KEND : 'end' ;\n" +
            "ID : 'a'..'z'+ ;\n" +
            "WS : (' '|'\\n')+ ;");
        string expecting;// = "ID, EOF";
        //checkLexerMatches(lg, "e", expecting);
        expecting = "KEND, EOF";
        CheckLexerMatches(lg, "end", expecting);
        expecting = "ID, EOF";
        CheckLexerMatches(lg, "ending", expecting);
        expecting = "ID, WS, KEND, WS, ID, EOF";
        CheckLexerMatches(lg, "a end bcd", expecting);
    }

    [TestMethod]
    public void TestLexerRuleRef()
    {
        var lg = new LexerGrammar(
            "lexer grammar L;\n" +
            "INT : DIGIT+ ;\n" +
            "fragment DIGIT : '0'..'9' ;\n" +
            "WS : (' '|'\\n')+ ;");
        var expecting = "INT, WS, INT, EOF";
        CheckLexerMatches(lg, "32 99", expecting);
    }

    [TestMethod]
    public void TestRecursiveLexerRuleRef()
    {
        var lg = new LexerGrammar(
            "lexer grammar L;\n" +
            "CMT : '/*' (CMT | ~'*')+ '*/' ;\n" +
            "WS : (' '|'\\n')+ ;");
        var expecting = "CMT, WS, CMT, EOF";
        CheckLexerMatches(lg, "/* ick */\n/* /*nested*/ */", expecting);
    }

    [TestMethod]
    public void TestRecursiveLexerRuleRefWithWildcard()
    {
        var lg = new LexerGrammar(
            "lexer grammar L;\n" +
            "CMT : '/*' (CMT | .)*? '*/' ;\n" +
            "WS : (' '|'\\n')+ ;");

        var expecting = "CMT, WS, CMT, WS, EOF";
        CheckLexerMatches(lg,
                          "/* ick */\n" +
                          "/* /* */\n" +
                          "/* /*nested*/ */\n",
                          expecting);
    }

    [TestMethod]
    public void TestLexerWildcardGreedyLoopByDefault()
    {
        var lg = new LexerGrammar(
            "lexer grammar L;\n" +
            "CMT : '//' .* '\\n' ;\n");
        var expecting = "CMT, EOF";
        CheckLexerMatches(lg, "//x\n//y\n", expecting);
    }

    [TestMethod]
    public void TestLexerWildcardLoopExplicitNonGreedy()
    {
        var lg = new LexerGrammar(
            "lexer grammar L;\n" +
            "CMT : '//' .*? '\\n' ;\n");
        var expecting = "CMT, CMT, EOF";
        CheckLexerMatches(lg, "//x\n//y\n", expecting);
    }

    [TestMethod]
    public void TestLexerEscapeInString()
    {
        var lg = new LexerGrammar(
            "lexer grammar L;\n" +
            "STR : '[' ('~' ']' | .)* ']' ;\n");
        CheckLexerMatches(lg, "[a~]b]", "STR, EOF");
        CheckLexerMatches(lg, "[a]", "STR, EOF");
    }

    [TestMethod]
    public void TestLexerWildcardGreedyPlusLoopByDefault()
    {
        var lg = new LexerGrammar(
            "lexer grammar L;\n" +
            "CMT : '//' .+ '\\n' ;\n");
        var expecting = "CMT, EOF";
        CheckLexerMatches(lg, "//x\n//y\n", expecting);
    }

    [TestMethod]
    public void TestLexerWildcardExplicitNonGreedyPlusLoop()
    {
        var lg = new LexerGrammar(
            "lexer grammar L;\n" +
            "CMT : '//' .+? '\\n' ;\n");
        var expecting = "CMT, CMT, EOF";
        CheckLexerMatches(lg, "//x\n//y\n", expecting);
    }

    // does not fail since ('*/')? can't match and have rule succeed
    [TestMethod]
    public void TestLexerGreedyOptionalShouldWorkAsWeExpect()
    {
        var lg = new LexerGrammar(
            "lexer grammar L;\n" +
            "CMT : '/*' ('*/')? '*/' ;\n");
        var expecting = "CMT, EOF";
        CheckLexerMatches(lg, "/**/", expecting);
    }

    [TestMethod]
    public void TestGreedyBetweenRules()
    {
        var lg = new LexerGrammar(
            "lexer grammar L;\n" +
            "A : '<a>' ;\n" +
            "B : '<' .+ '>' ;\n");
        var expecting = "B, EOF";
        CheckLexerMatches(lg, "<a><x>", expecting);
    }

    [TestMethod]
    public void TestNonGreedyBetweenRules()
    {
        var lg = new LexerGrammar(
            "lexer grammar L;\n" +
            "A : '<a>' ;\n" +
            "B : '<' .+? '>' ;\n");
        var expecting = "A, B, EOF";
        CheckLexerMatches(lg, "<a><x>", expecting);
    }

    [TestMethod]
    public void TestEOFAtEndOfLineComment()
    {
        var lg = new LexerGrammar(
            "lexer grammar L;\n" +
            "CMT : '//' ~('\\n')* ;\n");
        var expecting = "CMT, EOF";
        CheckLexerMatches(lg, "//x", expecting);
    }

    [TestMethod]
    public void TestEOFAtEndOfLineComment2()
    {
        var lg = new LexerGrammar(
            "lexer grammar L;\n" +
            "CMT : '//' ~('\\n'|'\\r')* ;\n");
        var expecting = "CMT, EOF";
        CheckLexerMatches(lg, "//x", expecting);
    }

    /** only positive sets like (EOF|'\n') can match EOF and not in wildcard or ~foo sets
	 *  EOF matches but does not advance cursor.
	 */
    [TestMethod]
    public void TestEOFInSetAtEndOfLineComment()
    {
        var lg = new LexerGrammar(
            "lexer grammar L;\n" +
            "CMT : '//' .* (EOF|'\\n') ;\n");
        var expecting = "CMT, EOF";
        CheckLexerMatches(lg, "//", expecting);
    }

    [TestMethod]
    public void TestEOFSuffixInSecondRule()
    {
        var lg = new LexerGrammar(
            "lexer grammar L;\n" +
            "A : 'a' ;\n" + // shorter than 'a' EOF, despite EOF being 0 width
            "B : 'a' EOF ;\n");
        var expecting = "B, EOF";
        CheckLexerMatches(lg, "a", expecting);
    }

    [TestMethod]
    public void TestEOFSuffixInFirstRule()
    {
        var lg = new LexerGrammar(
            "lexer grammar L;\n" +
            "A : 'a' EOF ;\n" +
            "B : 'a';\n");
        var expecting = "A, EOF";
        CheckLexerMatches(lg, "a", expecting);
    }

    [TestMethod]
    public void TestEOFByItself()
    {
        var lg = new LexerGrammar(
            "lexer grammar L;\n" +
            "DONE : EOF ;\n" +
            "A : 'a';\n");
        var expecting = "A, DONE, EOF";
        CheckLexerMatches(lg, "a", expecting);
    }

    [TestMethod]
    public void TestLexerCaseInsensitive()
    {
        var lg = new LexerGrammar(
            "lexer grammar L;\n" +
            "\n" +
            "options { caseInsensitive = true; }\n" +
            "\n" +
            "WS:             [ \\t\\r\\n] -> skip;\n" +
            "\n" +
            "SIMPLE_TOKEN:           'and';\n" +
            "TOKEN_WITH_SPACES:      'as' 'd' 'f';\n" +
            "TOKEN_WITH_DIGITS:      'INT64';\n" +
            "TOKEN_WITH_UNDERSCORE:  'TOKEN_WITH_UNDERSCORE';\n" +
            "BOOL:                   'true' | 'FALSE';\n" +
            "SPECIAL:                '==';\n" +
            "SET:                    [a-z0-9]+;\n" +   // [a-zA-Z0-9]
            "RANGE:                  ('а'..'я')+;"
            );

        var inputString =
            "and AND aND\n" +
            "asdf ASDF\n" +
            "int64\n" +
            "token_WITH_underscore\n" +
            "TRUE FALSE\n" +
            "==\n" +
            "A0bcDE93\n" +
            "АБВабв\n";

        var expecting = RuntimeUtils.Join(new string[] {
            "SIMPLE_TOKEN", "SIMPLE_TOKEN", "SIMPLE_TOKEN",
            "TOKEN_WITH_SPACES", "TOKEN_WITH_SPACES",
            "TOKEN_WITH_DIGITS",
            "TOKEN_WITH_UNDERSCORE",
            "BOOL", "BOOL",
            "SPECIAL",
            "SET",
            "RANGE",
            "EOF"
        },
        ", WS, ");

        CheckLexerMatches(lg, inputString, expecting);
    }

    [TestMethod]
    public void TestLexerCaseInsensitiveLiteralWithNegation()
    {
        var grammar =
                "lexer grammar L;\n" +
                "options { caseInsensitive = true; }\n" +
                "LITERAL_WITH_NOT:   ~'f';\n";     // ~('f' | 'F)
        var executedState = ToolTestUtils.ExecLexer("L.g4", grammar, "L", "F");

        Assert.AreEqual("line 1:0 token recognition error at: 'F'\n", executedState.errors);
    }

    [TestMethod]
    public void TestLexerCaseInsensitiveSetWithNegation()
    {
        var grammar =
                "lexer grammar L;\n" +
                "options { caseInsensitive = true; }\n" +
                "SET_WITH_NOT: ~[a-c];\n";        // ~[a-cA-C]
        var executedState = ToolTestUtils.ExecLexer("L.g4", grammar, "L", "B");

        Assert.AreEqual("line 1:0 token recognition error at: 'B'\n", executedState.errors);
    }

    [TestMethod]
    public void TestLexerCaseInsensitiveFragments()
    {
        var lg = new LexerGrammar(
                "lexer grammar L;\n" +
                "options { caseInsensitive = true; }\n" +
                "TOKEN_0:         FRAGMENT 'd'+;\n" +
                "TOKEN_1:         FRAGMENT 'e'+;\n" +
                "FRAGMENT:        'abc';\n");

        var inputString =
                "ABCDDD";

        var expecting = "TOKEN_0, EOF";

        CheckLexerMatches(lg, inputString, expecting);
    }

    [TestMethod]
    public void TestLexerCaseInsensitiveWithDifferentCultures()
    {
        // From http://www.periodni.com/unicode_utf-8_encoding.html
        var lg = new LexerGrammar(
                "lexer grammar L;\n" +
                "options { caseInsensitive = true; }\n" +
                "ENGLISH_TOKEN:   [a-z]+;\n" +
                "GERMAN_TOKEN:    [äéöüß]+;\n" +
                "FRENCH_TOKEN:    [àâæ-ëîïôœùûüÿ]+;\n" +
                "CROATIAN_TOKEN:  [ćčđšž]+;\n" +
                "ITALIAN_TOKEN:   [àèéìòù]+;\n" +
                "SPANISH_TOKEN:   [áéíñóúü¡¿]+;\n" +
                "GREEK_TOKEN:     [α-ω]+;\n" +
                "RUSSIAN_TOKEN:   [а-я]+;\n" +
                "WS:              [ ]+ -> skip;"
                );

        var inputString = "abcXYZ äéöüßÄÉÖÜß àâæçÙÛÜŸ ćčđĐŠŽ àèéÌÒÙ áéÚÜ¡¿ αβγΧΨΩ абвЭЮЯ ";

        var expecting = RuntimeUtils.Join(new string[] {
                "ENGLISH_TOKEN",
                "GERMAN_TOKEN",
                "FRENCH_TOKEN",
                "CROATIAN_TOKEN",
                "ITALIAN_TOKEN",
                "SPANISH_TOKEN",
                "GREEK_TOKEN",
                "RUSSIAN_TOKEN",
                "EOF" },
                ", WS, ");

        CheckLexerMatches(lg, inputString, expecting);
    }

    [TestMethod]
    public void TestNotImpliedCharactersWithEnabledCaseInsensitiveOption()
    {
        var lg = new LexerGrammar(
                "lexer grammar L;\n" +
                "options { caseInsensitive = true; }\n" +
                "TOKEN: ('A'..'z')+;\n"
        );

        // No range transformation because of mixed character case in range borders
        var inputString = "ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz";
        CheckLexerMatches(lg, inputString, "TOKEN, EOF");
    }

    [TestMethod]
    public void TestCaseInsensitiveInLexerRule()
    {
        var lg = new LexerGrammar(
                "lexer grammar L;\n" +
                "TOKEN1 options { caseInsensitive=true; } : [a-f]+;\n" +
                "WS: [ ]+ -> skip;"
        );

        CheckLexerMatches(lg, "ABCDEF", "TOKEN1, EOF");
    }

    [TestMethod]
    public void TestCaseInsensitiveInLexerRuleOverridesGlobalValue()
    {
        var grammar =
                "lexer grammar L;\n" +
                "options { caseInsensitive=true; }\n" +
                "STRING options { caseInsensitive=false; } : 'N'? '\\'' (~'\\'' | '\\'\\'')* '\\'';\n";

        var executedState = ToolTestUtils.ExecLexer("L.g4", grammar, "L", "n'sample'");
        Assert.AreEqual("line 1:0 token recognition error at: 'n'\n", executedState.errors);
    }

    private static void CheckLexerMatches(LexerGrammar lg, string inputString, string expecting)
    {
        var atn = ToolTestUtils.CreateATN(lg, true);
        var input = CharStreams.FromString(inputString);
        var startState = atn.modeNameToStartState[("DEFAULT_MODE")];
        var dot = new DOTGenerator(lg);
        //		Console.Out.WriteLine(dot.getDOT(startState, true));

        var tokenTypes = GetTokenTypes(lg, atn, input);

        var result = RuntimeUtils.Join(tokenTypes, ", ");
        //		Console.Out.WriteLine(tokenTypes);
        Assert.AreEqual(expecting, result);
    }

    private static List<string> GetTokenTypes(LexerGrammar lg, ATN atn, CharStream input)
    {
        var interp = new LexerATNSimulator(atn, new DFA[] { new DFA(atn.modeToStartState[(Lexer.DEFAULT_MODE)]) }, null);
        List<string> tokenTypes = new();
        int ttype;
        bool hitEOF = false;
        do
        {
            if (hitEOF)
            {
                tokenTypes.Add("EOF");
                break;
            }
            int t = input.LA(1);
            ttype = interp.Match(input, Lexer.DEFAULT_MODE);
            if (ttype == Token.EOF)
            {
                tokenTypes.Add("EOF");
            }
            else
            {
                tokenTypes.Add(lg.typeToTokenList[(ttype)]);
            }

            if (t == IntStream.EOF)
            {
                hitEOF = true;
            }
        } while (ttype != Token.EOF);
        return tokenTypes;
    }
}
