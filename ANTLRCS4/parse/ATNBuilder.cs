﻿// $ANTLR 3.5.3 org\\antlr\\v4\\parse\\ATNBuilder.g 2023-01-27 22:27:34

/*
 [The "BSD license"]
 Copyright (c) 2010 Terence Parr
 All rights reserved.
 Redistribution and use in source and binary forms, with or without
 modification, are permitted provided that the following conditions
 are met:
 1. Redistributions of source code must retain the above copyright
    notice, this list of conditions and the following disclaimer.
 2. Redistributions in binary form must reproduce the above copyright
    notice, this list of conditions and the following disclaimer in the
    documentation and/or other materials provided with the distribution.
 3. The name of the author may not be used to endorse or promote products
    derived from this software without specific prior written permission.
 THIS SOFTWARE IS PROVIDED BY THE AUTHOR ``AS IS'' AND ANY EXPRESS OR
 IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
 OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.
 IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY DIRECT, INDIRECT,
 INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT
 NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
 DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
 THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
 THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/
using org.antlr.runtime.tree;
using org.antlr.runtime;
using org.antlr.v4.automata;
using org.antlr.v4.runtime;
using org.antlr.v4.tool.ast;
using org.antlr.v4.runtime.misc;

namespace org.antlr.v4.parse;
public class ATNBuilder : TreeParser
{

    public static readonly string[] tokenNames = new string[] {
        "<invalid>", "<EOR>", "<DOWN>", "<UP>", "ACTION", "ACTION_CHAR_LITERAL",
        "ACTION_ESC", "ACTION_STRING_LITERAL", "ARG_ACTION", "ARG_OR_CHARSET",
        "ASSIGN", "AT", "CATCH", "CHANNELS", "COLON", "COLONCOLON", "COMMA", "COMMENT",
        "DOC_COMMENT", "DOLLAR", "DOT", "ERRCHAR", "ESC_SEQ", "FINALLY", "FRAGMENT",
        "GRAMMAR", "GT", "HEX_DIGIT", "ID", "IMPORT", "INT", "LEXER", "LEXER_CHAR_SET",
        "LOCALS", "LPAREN", "LT", "MODE", "NESTED_ACTION", "NLCHARS", "NOT", "NameChar",
        "NameStartChar", "OPTIONS", "OR", "PARSER", "PLUS", "PLUS_ASSIGN", "POUND",
        "QUESTION", "RANGE", "RARROW", "RBRACE", "RETURNS", "RPAREN", "RULE_REF",
        "SEMI", "SEMPRED", "SRC", "STAR", "STRING_LITERAL", "THROWS", "TOKENS_SPEC",
        "TOKEN_REF", "UNICODE_ESC", "UNICODE_EXTENDED_ESC", "UnicodeBOM", "WS",
        "WSCHARS", "WSNLCHARS", "ALT", "BLOCK", "CLOSURE", "COMBINED", "ELEMENT_OPTIONS",
        "EPSILON", "LEXER_ACTION_CALL", "LEXER_ALT_ACTION", "OPTIONAL", "POSITIVE_CLOSURE",
        "RULE", "RULEMODIFIERS", "RULES", "SET", "WILDCARD"
    };
    public const int EOF = -1;
    public const int ACTION = 4;
    public const int ACTION_CHAR_LITERAL = 5;
    public const int ACTION_ESC = 6;
    public const int ACTION_STRING_LITERAL = 7;
    public const int ARG_ACTION = 8;
    public const int ARG_OR_CHARSET = 9;
    public const int ASSIGN = 10;
    public const int AT = 11;
    public const int CATCH = 12;
    public const int CHANNELS = 13;
    public const int COLON = 14;
    public const int COLONCOLON = 15;
    public const int COMMA = 16;
    public const int COMMENT = 17;
    public const int DOC_COMMENT = 18;
    public const int DOLLAR = 19;
    public const int DOT = 20;
    public const int ERRCHAR = 21;
    public const int ESC_SEQ = 22;
    public const int FINALLY = 23;
    public const int FRAGMENT = 24;
    public const int GRAMMAR = 25;
    public const int GT = 26;
    public const int HEX_DIGIT = 27;
    public const int ID = 28;
    public const int IMPORT = 29;
    public const int INT = 30;
    public const int LEXER = 31;
    public const int LEXER_CHAR_SET = 32;
    public const int LOCALS = 33;
    public const int LPAREN = 34;
    public const int LT = 35;
    public const int MODE = 36;
    public const int NESTED_ACTION = 37;
    public const int NLCHARS = 38;
    public const int NOT = 39;
    public const int NameChar = 40;
    public const int NameStartChar = 41;
    public const int OPTIONS = 42;
    public const int OR = 43;
    public const int PARSER = 44;
    public const int PLUS = 45;
    public const int PLUS_ASSIGN = 46;
    public const int POUND = 47;
    public const int QUESTION = 48;
    public const int RANGE = 49;
    public const int RARROW = 50;
    public const int RBRACE = 51;
    public const int RETURNS = 52;
    public const int RPAREN = 53;
    public const int RULE_REF = 54;
    public const int SEMI = 55;
    public const int SEMPRED = 56;
    public const int SRC = 57;
    public const int STAR = 58;
    public const int STRING_LITERAL = 59;
    public const int THROWS = 60;
    public const int TOKENS_SPEC = 61;
    public const int TOKEN_REF = 62;
    public const int UNICODE_ESC = 63;
    public const int UNICODE_EXTENDED_ESC = 64;
    public const int UnicodeBOM = 65;
    public const int WS = 66;
    public const int WSCHARS = 67;
    public const int WSNLCHARS = 68;
    public const int ALT = 69;
    public const int BLOCK = 70;
    public const int CLOSURE = 71;
    public const int COMBINED = 72;
    public const int ELEMENT_OPTIONS = 73;
    public const int EPSILON = 74;
    public const int LEXER_ACTION_CALL = 75;
    public const int LEXER_ALT_ACTION = 76;
    public const int OPTIONAL = 77;
    public const int POSITIVE_CLOSURE = 78;
    public const int RULE = 79;
    public const int RULEMODIFIERS = 80;
    public const int RULES = 81;
    public const int SET = 82;
    public const int WILDCARD = 83;

    // delegates
    public TreeParser[] getDelegates()
    {
        return new TreeParser[] { };
    }

    // delegators

    protected DFA10 dfa10;

    public ATNBuilder(TreeNodeStream input)
        : this(input, new RecognizerSharedState())
    {
    }
    public ATNBuilder(TreeNodeStream input, RecognizerSharedState state)
        : base(input, state)
    {
        dfa10 = new DFA10(this);
    }

    public override string[] TokenNames => ATNBuilder.tokenNames; 
    public override string GrammarFileName => "org\\antlr\\v4\\parse\\ATNBuilder.g";

    readonly ATNFactory factory;
    public ATNBuilder(TreeNodeStream input, ATNFactory factory)
        : this(input) => this.factory = factory;



    // $ANTLR start "dummy"
    // org\\antlr\\v4\\parse\\ATNBuilder.g:80:1: dummy : block[null] ;
    public void Dummy()
    {
        try
        {
            // org\\antlr\\v4\\parse\\ATNBuilder.g:80:7: ( block[null] )
            // org\\antlr\\v4\\parse\\ATNBuilder.g:80:9: block[null]
            {
                PushFollow(FOLLOW_block_in_dummy63);
                Block(null);
                state._fsp--;

            }

        }
        catch (RecognitionException re)
        {
            ReportError(re);
            Recover(input, re);
        }
        finally
        {
            // do for sure before leaving
        }
    }
    // $ANTLR end "dummy"



    // $ANTLR start "ruleBlock"
    // org\\antlr\\v4\\parse\\ATNBuilder.g:82:1: ruleBlock[GrammarAST ebnfRoot] returns [ATNFactory.Handle p] : ^( BLOCK ( ^( OPTIONS ( . )* ) )? (a= alternative )+ ) ;
    public ATNFactory.Handle RuleBlock(GrammarAST ebnfRoot)
    {
        ATNFactory.Handle p = null;


        GrammarAST BLOCK1 = null;
        ATNFactory.Handle a = null;


        List<ATNFactory.Handle> alts = new();
        int alt = 1;
        factory.SetCurrentOuterAlt(alt);

        try
        {
            // org\\antlr\\v4\\parse\\ATNBuilder.g:88:5: ( ^( BLOCK ( ^( OPTIONS ( . )* ) )? (a= alternative )+ ) )
            // org\\antlr\\v4\\parse\\ATNBuilder.g:88:7: ^( BLOCK ( ^( OPTIONS ( . )* ) )? (a= alternative )+ )
            {
                BLOCK1 = (GrammarAST)Match(input, BLOCK, FOLLOW_BLOCK_in_ruleBlock89);
                Match(input, Token.DOWN, null);
                // org\\antlr\\v4\\parse\\ATNBuilder.g:89:13: ( ^( OPTIONS ( . )* ) )?
                int alt2 = 2;
                int LA2_0 = input.LA(1);
                if ((LA2_0 == OPTIONS))
                {
                    alt2 = 1;
                }
                switch (alt2)
                {
                    case 1:
                        // org\\antlr\\v4\\parse\\ATNBuilder.g:89:14: ^( OPTIONS ( . )* )
                        {
                            Match(input, OPTIONS, FOLLOW_OPTIONS_in_ruleBlock105);
                            if (input.LA(1) == Token.DOWN)
                            {
                                Match(input, Token.DOWN, null);
                            // org\\antlr\\v4\\parse\\ATNBuilder.g:89:24: ( . )*
                            loop1:
                                while (true)
                                {
                                    int alt1 = 2;
                                    int LA1_0 = input.LA(1);
                                    if (((LA1_0 >= ACTION && LA1_0 <= WILDCARD)))
                                    {
                                        alt1 = 1;
                                    }
                                    else if ((LA1_0 == UP))
                                    {
                                        alt1 = 2;
                                    }

                                    switch (alt1)
                                    {
                                        case 1:
                                            // org\\antlr\\v4\\parse\\ATNBuilder.g:89:24: .
                                            {
                                                MatchAny(input);
                                            }
                                            break;

                                        default:
                                            goto exit1;
                                            //break loop1;
                                    }
                                }
                            exit1:
                                Match(input, Token.UP, null);
                            }

                        }
                        break;

                }

                // org\\antlr\\v4\\parse\\ATNBuilder.g:90:13: (a= alternative )+
                int cnt3 = 0;
            loop3:
                while (true)
                {
                    int alt3 = 2;
                    int LA3_0 = input.LA(1);
                    if ((LA3_0 == ALT || LA3_0 == LEXER_ALT_ACTION))
                    {
                        alt3 = 1;
                    }

                    switch (alt3)
                    {
                        case 1:
                            // org\\antlr\\v4\\parse\\ATNBuilder.g:90:17: a= alternative
                            {
                                PushFollow(FOLLOW_alternative_in_ruleBlock131);
                                a = Alternative();
                                state._fsp--;

                                alts.Add(a); factory.SetCurrentOuterAlt(++alt);
                            }
                            break;

                        default:
                            if (cnt3 >= 1) goto exit3;// break loop3;
                            EarlyExitException eee = new EarlyExitException(3, input);
                            throw eee;
                    }
                    cnt3++;
                }
            exit3:
                Match(input, Token.UP, null);

                p = factory.Block((BlockAST)BLOCK1, ebnfRoot, alts);
            }

        }
        catch (RecognitionException re)
        {
            ReportError(re);
            Recover(input, re);
        }
        finally
        {
            // do for sure before leaving
        }
        return p;
    }
    // $ANTLR end "ruleBlock"



    // $ANTLR start "block"
    // org\\antlr\\v4\\parse\\ATNBuilder.g:97:1: block[GrammarAST ebnfRoot] returns [ATNFactory.Handle p] : ^( BLOCK ( ^( OPTIONS ( . )* ) )? (a= alternative )+ ) ;
    public ATNFactory.Handle Block(GrammarAST ebnfRoot)
    {
        ATNFactory.Handle p = null;


        GrammarAST BLOCK2 = null;
        ATNFactory.Handle a = null;

        List<ATNFactory.Handle> alts = new();
        try
        {
            // org\\antlr\\v4\\parse\\ATNBuilder.g:99:5: ( ^( BLOCK ( ^( OPTIONS ( . )* ) )? (a= alternative )+ ) )
            // org\\antlr\\v4\\parse\\ATNBuilder.g:99:7: ^( BLOCK ( ^( OPTIONS ( . )* ) )? (a= alternative )+ )
            {
                BLOCK2 = (GrammarAST)Match(input, BLOCK, FOLLOW_BLOCK_in_block209);
                Match(input, Token.DOWN, null);
                // org\\antlr\\v4\\parse\\ATNBuilder.g:99:15: ( ^( OPTIONS ( . )* ) )?
                int alt5 = 2;
                int LA5_0 = input.LA(1);
                if ((LA5_0 == OPTIONS))
                {
                    alt5 = 1;
                }
                switch (alt5)
                {
                    case 1:
                        // org\\antlr\\v4\\parse\\ATNBuilder.g:99:16: ^( OPTIONS ( . )* )
                        {
                            Match(input, OPTIONS, FOLLOW_OPTIONS_in_block213);
                            if (input.LA(1) == Token.DOWN)
                            {
                                Match(input, Token.DOWN, null);
                            // org\\antlr\\v4\\parse\\ATNBuilder.g:99:26: ( . )*
                            loop4:
                                while (true)
                                {
                                    int alt4 = 2;
                                    int LA4_0 = input.LA(1);
                                    if (((LA4_0 >= ACTION && LA4_0 <= WILDCARD)))
                                    {
                                        alt4 = 1;
                                    }
                                    else if ((LA4_0 == UP))
                                    {
                                        alt4 = 2;
                                    }

                                    switch (alt4)
                                    {
                                        case 1:
                                            // org\\antlr\\v4\\parse\\ATNBuilder.g:99:26: .
                                            {
                                                MatchAny(input);
                                            }
                                            break;

                                        default:
                                            goto exit4;
                                            //break loop4;
                                    }
                                }
                            exit4:
                                Match(input, Token.UP, null);
                            }

                        }
                        break;

                }

                // org\\antlr\\v4\\parse\\ATNBuilder.g:99:32: (a= alternative )+
                int cnt6 = 0;
            loop6:
                while (true)
                {
                    int alt6 = 2;
                    int LA6_0 = input.LA(1);
                    if ((LA6_0 == ALT || LA6_0 == LEXER_ALT_ACTION))
                    {
                        alt6 = 1;
                    }

                    switch (alt6)
                    {
                        case 1:
                            // org\\antlr\\v4\\parse\\ATNBuilder.g:99:33: a= alternative
                            {
                                PushFollow(FOLLOW_alternative_in_block224);
                                a = Alternative();
                                state._fsp--;

                                alts.Add(a);
                            }
                            break;

                        default:
                            if (cnt6 >= 1) goto exit6;// break loop6;
                            EarlyExitException eee = new EarlyExitException(6, input);
                            throw eee;
                    }
                    cnt6++;
                }
            exit6:
                Match(input, Token.UP, null);

                p = factory.Block((BlockAST)BLOCK2, ebnfRoot, alts);
            }

        }
        catch (RecognitionException re)
        {
            ReportError(re);
            Recover(input, re);
        }
        finally
        {
            // do for sure before leaving
        }
        return p;
    }
    // $ANTLR end "block"



    // $ANTLR start "alternative"
    // org\\antlr\\v4\\parse\\ATNBuilder.g:103:1: alternative returns [ATNFactory.Handle p] : ( ^( LEXER_ALT_ACTION a= alternative lexerCommands ) | ^( ALT ( elementOptions )? EPSILON ) | ^( ALT ( elementOptions )? (e= element )+ ) );
    public ATNFactory.Handle Alternative()
    {
        ATNFactory.Handle p = null;


        GrammarAST EPSILON4 = null;
        ATNFactory.Handle a = null;
        TreeRuleReturnScope e = null;
        ATNFactory.Handle lexerCommands3 = null;

        List<ATNFactory.Handle> els = new();
        try
        {
            // org\\antlr\\v4\\parse\\ATNBuilder.g:105:5: ( ^( LEXER_ALT_ACTION a= alternative lexerCommands ) | ^( ALT ( elementOptions )? EPSILON ) | ^( ALT ( elementOptions )? (e= element )+ ) )
            int alt10 = 3;
            alt10 = dfa10.Predict(input);
            switch (alt10)
            {
                case 1:
                    // org\\antlr\\v4\\parse\\ATNBuilder.g:105:7: ^( LEXER_ALT_ACTION a= alternative lexerCommands )
                    {
                        Match(input, LEXER_ALT_ACTION, FOLLOW_LEXER_ALT_ACTION_in_alternative263);
                        Match(input, Token.DOWN, null);
                        PushFollow(FOLLOW_alternative_in_alternative267);
                        a = Alternative();
                        state._fsp--;

                        PushFollow(FOLLOW_lexerCommands_in_alternative269);
                        lexerCommands3 = lexerCommands();
                        state._fsp--;

                        Match(input, Token.UP, null);

                        p = factory.LexerAltCommands(a, lexerCommands3);
                    }
                    break;
                case 2:
                    // org\\antlr\\v4\\parse\\ATNBuilder.g:107:7: ^( ALT ( elementOptions )? EPSILON )
                    {
                        Match(input, ALT, FOLLOW_ALT_in_alternative289);
                        Match(input, Token.DOWN, null);
                        // org\\antlr\\v4\\parse\\ATNBuilder.g:107:13: ( elementOptions )?
                        int alt7 = 2;
                        int LA7_0 = input.LA(1);
                        if ((LA7_0 == ELEMENT_OPTIONS))
                        {
                            alt7 = 1;
                        }
                        switch (alt7)
                        {
                            case 1:
                                // org\\antlr\\v4\\parse\\ATNBuilder.g:107:13: elementOptions
                                {
                                    PushFollow(FOLLOW_elementOptions_in_alternative291);
                                    elementOptions();
                                    state._fsp--;

                                }
                                break;

                        }

                        EPSILON4 = (GrammarAST)Match(input, EPSILON, FOLLOW_EPSILON_in_alternative294);
                        Match(input, Token.UP, null);

                        p = factory.Epsilon(EPSILON4);
                    }
                    break;
                case 3:
                    // org\\antlr\\v4\\parse\\ATNBuilder.g:108:9: ^( ALT ( elementOptions )? (e= element )+ )
                    {
                        Match(input, ALT, FOLLOW_ALT_in_alternative314);
                        Match(input, Token.DOWN, null);
                        // org\\antlr\\v4\\parse\\ATNBuilder.g:108:15: ( elementOptions )?
                        int alt8 = 2;
                        int LA8_0 = input.LA(1);
                        if ((LA8_0 == ELEMENT_OPTIONS))
                        {
                            alt8 = 1;
                        }
                        switch (alt8)
                        {
                            case 1:
                                // org\\antlr\\v4\\parse\\ATNBuilder.g:108:15: elementOptions
                                {
                                    PushFollow(FOLLOW_elementOptions_in_alternative316);
                                    elementOptions();
                                    state._fsp--;

                                }
                                break;

                        }

                        // org\\antlr\\v4\\parse\\ATNBuilder.g:108:31: (e= element )+
                        int cnt9 = 0;
                    loop9:
                        while (true)
                        {
                            int alt9 = 2;
                            int LA9_0 = input.LA(1);
                            if ((LA9_0 == ACTION || LA9_0 == ASSIGN || LA9_0 == DOT || LA9_0 == LEXER_CHAR_SET || LA9_0 == NOT || LA9_0 == PLUS_ASSIGN || LA9_0 == RANGE || LA9_0 == RULE_REF || LA9_0 == SEMPRED || LA9_0 == STRING_LITERAL || LA9_0 == TOKEN_REF || (LA9_0 >= BLOCK && LA9_0 <= CLOSURE) || (LA9_0 >= OPTIONAL && LA9_0 <= POSITIVE_CLOSURE) || (LA9_0 >= SET && LA9_0 <= WILDCARD)))
                            {
                                alt9 = 1;
                            }

                            switch (alt9)
                            {
                                case 1:
                                    // org\\antlr\\v4\\parse\\ATNBuilder.g:108:32: e= element
                                    {
                                        PushFollow(FOLLOW_element_in_alternative322);
                                        e = element();
                                        state._fsp--;

                                        els.Add((e != null ? ((ATNBuilder.element_return)e).p : null));
                                    }
                                    break;

                                default:
                                    if (cnt9 >= 1) goto exit9;// break loop9;
                                    EarlyExitException eee = new EarlyExitException(9, input);
                                    throw eee;
                            }
                            cnt9++;
                        }
                    exit9:
                        Match(input, Token.UP, null);

                        p = factory.Alt(els);
                    }
                    break;

            }
        }
        catch (RecognitionException re)
        {
            ReportError(re);
            Recover(input, re);
        }
        finally
        {
            // do for sure before leaving
        }
        return p;
    }
    // $ANTLR end "alternative"



    // $ANTLR start "lexerCommands"
    // org\\antlr\\v4\\parse\\ATNBuilder.g:111:1: lexerCommands returns [ATNFactory.Handle p] : (c= lexerCommand )+ ;
    public ATNFactory.Handle lexerCommands()
    {
        ATNFactory.Handle p = null;


        ATNFactory.Handle c = null;

        List<ATNFactory.Handle> cmds = new();
        try
        {
            // org\\antlr\\v4\\parse\\ATNBuilder.g:113:5: ( (c= lexerCommand )+ )
            // org\\antlr\\v4\\parse\\ATNBuilder.g:113:9: (c= lexerCommand )+
            {
                // org\\antlr\\v4\\parse\\ATNBuilder.g:113:9: (c= lexerCommand )+
                int cnt11 = 0;
            loop11:
                while (true)
                {
                    int alt11 = 2;
                    int LA11_0 = input.LA(1);
                    if ((LA11_0 == ID || LA11_0 == LEXER_ACTION_CALL))
                    {
                        alt11 = 1;
                    }

                    switch (alt11)
                    {
                        case 1:
                            // org\\antlr\\v4\\parse\\ATNBuilder.g:113:10: c= lexerCommand
                            {
                                PushFollow(FOLLOW_lexerCommand_in_lexerCommands360);
                                c = lexerCommand();
                                state._fsp--;

                                if (c != null) cmds.Add(c);
                            }
                            break;

                        default:
                            if (cnt11 >= 1) goto exit11;// break loop11;
                            EarlyExitException eee = new EarlyExitException(11, input);
                            throw eee;
                    }
                    cnt11++;
                }
            exit11:

                p = factory.Alt(cmds);

            }

        }
        catch (RecognitionException re)
        {
            ReportError(re);
            Recover(input, re);
        }
        finally
        {
            // do for sure before leaving
        }
        return p;
    }
    // $ANTLR end "lexerCommands"



    // $ANTLR start "lexerCommand"
    // org\\antlr\\v4\\parse\\ATNBuilder.g:119:1: lexerCommand returns [ATNFactory.Handle cmd] : ( ^( LEXER_ACTION_CALL ID lexerCommandExpr ) | ID );
    public ATNFactory.Handle lexerCommand()
    {
        ATNFactory.Handle cmd = null;


        GrammarAST ID5 = null;
        GrammarAST ID7 = null;
        TreeRuleReturnScope lexerCommandExpr6 = null;

        try
        {
            // org\\antlr\\v4\\parse\\ATNBuilder.g:120:2: ( ^( LEXER_ACTION_CALL ID lexerCommandExpr ) | ID )
            int alt12 = 2;
            int LA12_0 = input.LA(1);
            if ((LA12_0 == LEXER_ACTION_CALL))
            {
                alt12 = 1;
            }
            else if ((LA12_0 == ID))
            {
                alt12 = 2;
            }

            else
            {
                NoViableAltException nvae =
                    new NoViableAltException("", 12, 0, input);
                throw nvae;
            }

            switch (alt12)
            {
                case 1:
                    // org\\antlr\\v4\\parse\\ATNBuilder.g:120:4: ^( LEXER_ACTION_CALL ID lexerCommandExpr )
                    {
                        Match(input, LEXER_ACTION_CALL, FOLLOW_LEXER_ACTION_CALL_in_lexerCommand393);
                        Match(input, Token.DOWN, null);
                        ID5 = (GrammarAST)Match(input, ID, FOLLOW_ID_in_lexerCommand395);
                        PushFollow(FOLLOW_lexerCommandExpr_in_lexerCommand397);
                        lexerCommandExpr6 = lexerCommandExpr();
                        state._fsp--;

                        Match(input, Token.UP, null);

                        cmd = factory.LexerCallCommand(ID5, (lexerCommandExpr6 != null ? ((GrammarAST)lexerCommandExpr6.start) : null));
                    }
                    break;
                case 2:
                    // org\\antlr\\v4\\parse\\ATNBuilder.g:122:4: ID
                    {
                        ID7 = (GrammarAST)Match(input, ID, FOLLOW_ID_in_lexerCommand413);
                        cmd = factory.LexerCommand(ID7);
                    }
                    break;

            }
        }
        catch (RecognitionException re)
        {
            ReportError(re);
            Recover(input, re);
        }
        finally
        {
            // do for sure before leaving
        }
        return cmd;
    }
    // $ANTLR end "lexerCommand"


    public class lexerCommandExpr_return : TreeRuleReturnScope
    {
    };


    // $ANTLR start "lexerCommandExpr"
    // org\\antlr\\v4\\parse\\ATNBuilder.g:126:1: lexerCommandExpr : ( ID | INT );
    public ATNBuilder.lexerCommandExpr_return lexerCommandExpr()
    {
        ATNBuilder.lexerCommandExpr_return retval = new ATNBuilder.lexerCommandExpr_return();
        retval.start = input.LT(1);

        try
        {
            // org\\antlr\\v4\\parse\\ATNBuilder.g:127:2: ( ID | INT )
            // org\\antlr\\v4\\parse\\ATNBuilder.g:
            {
                if (input.LA(1) == ID || input.LA(1) == INT)
                {
                    input.Consume();
                    state.errorRecovery = false;
                }
                else
                {
                    MismatchedSetException mse = new MismatchedSetException(null, input);
                    throw mse;
                }
            }

        }
        catch (RecognitionException re)
        {
            ReportError(re);
            Recover(input, re);
        }
        finally
        {
            // do for sure before leaving
        }
        return retval;
    }
    // $ANTLR end "lexerCommandExpr"


    public class element_return : TreeRuleReturnScope
    {

        public ATNFactory.Handle p;
    };


    // $ANTLR start "element"
    // org\\antlr\\v4\\parse\\ATNBuilder.g:131:1: element returns [ATNFactory.Handle p] : ( labeledElement | atom | subrule | ACTION | SEMPRED | ^( ACTION . ) | ^( SEMPRED . ) | ^( NOT b= blockSet[true] ) | LEXER_CHAR_SET );
    public ATNBuilder.element_return element()
    {
        ATNBuilder.element_return retval = new ATNBuilder.element_return();
        retval.start = input.LT(1);

        GrammarAST ACTION11 = null;
        GrammarAST SEMPRED12 = null;
        GrammarAST ACTION13 = null;
        GrammarAST SEMPRED14 = null;
        TreeRuleReturnScope b = null;
        ATNFactory.Handle labeledElement8 = null;
        TreeRuleReturnScope atom9 = null;
        TreeRuleReturnScope subrule10 = null;

        try
        {
            // org\\antlr\\v4\\parse\\ATNBuilder.g:132:2: ( labeledElement | atom | subrule | ACTION | SEMPRED | ^( ACTION . ) | ^( SEMPRED . ) | ^( NOT b= blockSet[true] ) | LEXER_CHAR_SET )
            int alt13 = 9;
            switch (input.LA(1))
            {
                case ASSIGN:
                case PLUS_ASSIGN:
                    {
                        alt13 = 1;
                    }
                    break;
                case DOT:
                case RANGE:
                case RULE_REF:
                case STRING_LITERAL:
                case TOKEN_REF:
                case SET:
                case WILDCARD:
                    {
                        alt13 = 2;
                    }
                    break;
                case BLOCK:
                case CLOSURE:
                case OPTIONAL:
                case POSITIVE_CLOSURE:
                    {
                        alt13 = 3;
                    }
                    break;
                case ACTION:
                    {
                        int LA13_4 = input.LA(2);
                        if ((LA13_4 == DOWN))
                        {
                            alt13 = 6;
                        }
                        else if (((LA13_4 >= UP && LA13_4 <= ACTION) || LA13_4 == ASSIGN || LA13_4 == DOT || LA13_4 == LEXER_CHAR_SET || LA13_4 == NOT || LA13_4 == PLUS_ASSIGN || LA13_4 == RANGE || LA13_4 == RULE_REF || LA13_4 == SEMPRED || LA13_4 == STRING_LITERAL || LA13_4 == TOKEN_REF || (LA13_4 >= BLOCK && LA13_4 <= CLOSURE) || (LA13_4 >= OPTIONAL && LA13_4 <= POSITIVE_CLOSURE) || (LA13_4 >= SET && LA13_4 <= WILDCARD)))
                        {
                            alt13 = 4;
                        }

                        else
                        {
                            int nvaeMark = input.Mark();
                            try
                            {
                                input.Consume();
                                NoViableAltException nvae2 =
                                    new NoViableAltException("", 13, 4, input);
                                throw nvae2;
                            }
                            finally
                            {
                                input.Rewind(nvaeMark);
                            }
                        }

                    }
                    break;
                case SEMPRED:
                    {
                        int LA13_5 = input.LA(2);
                        if ((LA13_5 == DOWN))
                        {
                            alt13 = 7;
                        }
                        else if (((LA13_5 >= UP && LA13_5 <= ACTION) || LA13_5 == ASSIGN || LA13_5 == DOT || LA13_5 == LEXER_CHAR_SET || LA13_5 == NOT || LA13_5 == PLUS_ASSIGN || LA13_5 == RANGE || LA13_5 == RULE_REF || LA13_5 == SEMPRED || LA13_5 == STRING_LITERAL || LA13_5 == TOKEN_REF || (LA13_5 >= BLOCK && LA13_5 <= CLOSURE) || (LA13_5 >= OPTIONAL && LA13_5 <= POSITIVE_CLOSURE) || (LA13_5 >= SET && LA13_5 <= WILDCARD)))
                        {
                            alt13 = 5;
                        }

                        else
                        {
                            int nvaeMark = input.Mark();
                            try
                            {
                                input.Consume();
                                NoViableAltException nvae3 =
                                    new NoViableAltException("", 13, 5, input);
                                throw nvae3;
                            }
                            finally
                            {
                                input.Rewind(nvaeMark);
                            }
                        }

                    }
                    break;
                case NOT:
                    {
                        alt13 = 8;
                    }
                    break;
                case LEXER_CHAR_SET:
                    {
                        alt13 = 9;
                    }
                    break;
                default:
                    NoViableAltException nvae =
                        new NoViableAltException("", 13, 0, input);
                    throw nvae;
            }
            switch (alt13)
            {
                case 1:
                    // org\\antlr\\v4\\parse\\ATNBuilder.g:132:4: labeledElement
                    {
                        PushFollow(FOLLOW_labeledElement_in_element454);
                        labeledElement8 = labeledElement();
                        state._fsp--;

                        retval.p = labeledElement8;
                    }
                    break;
                case 2:
                    // org\\antlr\\v4\\parse\\ATNBuilder.g:133:4: atom
                    {
                        PushFollow(FOLLOW_atom_in_element464);
                        atom9 = atom();
                        state._fsp--;

                        retval.p = (atom9 != null ? ((ATNBuilder.atom_return)atom9).p : null);
                    }
                    break;
                case 3:
                    // org\\antlr\\v4\\parse\\ATNBuilder.g:134:4: subrule
                    {
                        PushFollow(FOLLOW_subrule_in_element476);
                        subrule10 = subrule();
                        state._fsp--;

                        retval.p = (subrule10 != null ? ((ATNBuilder.subrule_return)subrule10).p : null);
                    }
                    break;
                case 4:
                    // org\\antlr\\v4\\parse\\ATNBuilder.g:135:6: ACTION
                    {
                        ACTION11 = (GrammarAST)Match(input, ACTION, FOLLOW_ACTION_in_element490);
                        retval.p = factory.Action((ActionAST)ACTION11);
                    }
                    break;
                case 5:
                    // org\\antlr\\v4\\parse\\ATNBuilder.g:136:6: SEMPRED
                    {
                        SEMPRED12 = (GrammarAST)Match(input, SEMPRED, FOLLOW_SEMPRED_in_element504);
                        retval.p = factory.Sempred((PredAST)SEMPRED12);
                    }
                    break;
                case 6:
                    // org\\antlr\\v4\\parse\\ATNBuilder.g:137:6: ^( ACTION . )
                    {
                        ACTION13 = (GrammarAST)Match(input, ACTION, FOLLOW_ACTION_in_element519);
                        Match(input, Token.DOWN, null);
                        MatchAny(input);
                        Match(input, Token.UP, null);

                        retval.p = factory.Action((ActionAST)ACTION13);
                    }
                    break;
                case 7:
                    // org\\antlr\\v4\\parse\\ATNBuilder.g:138:6: ^( SEMPRED . )
                    {
                        SEMPRED14 = (GrammarAST)Match(input, SEMPRED, FOLLOW_SEMPRED_in_element536);
                        Match(input, Token.DOWN, null);
                        MatchAny(input);
                        Match(input, Token.UP, null);

                        retval.p = factory.Sempred((PredAST)SEMPRED14);
                    }
                    break;
                case 8:
                    // org\\antlr\\v4\\parse\\ATNBuilder.g:139:7: ^( NOT b= blockSet[true] )
                    {
                        Match(input, NOT, FOLLOW_NOT_in_element553);
                        Match(input, Token.DOWN, null);
                        PushFollow(FOLLOW_blockSet_in_element557);
                        b = blockSet(true);
                        state._fsp--;

                        Match(input, Token.UP, null);

                        retval.p = (b != null ? ((ATNBuilder.blockSet_return)b).p : null);
                    }
                    break;
                case 9:
                    // org\\antlr\\v4\\parse\\ATNBuilder.g:140:7: LEXER_CHAR_SET
                    {
                        Match(input, LEXER_CHAR_SET, FOLLOW_LEXER_CHAR_SET_in_element570);
                        retval.p = factory.CharSetLiteral(((GrammarAST)retval.start));
                    }
                    break;

            }
        }
        catch (RecognitionException re)
        {
            ReportError(re);
            Recover(input, re);
        }
        finally
        {
            // do for sure before leaving
        }
        return retval;
    }
    // $ANTLR end "element"



    // $ANTLR start "astOperand"
    // org\\antlr\\v4\\parse\\ATNBuilder.g:143:1: astOperand returns [ATNFactory.Handle p] : ( atom | ^( NOT blockSet[true] ) );
    public ATNFactory.Handle astOperand()
    {
        ATNFactory.Handle p = null;


        TreeRuleReturnScope atom15 = null;
        TreeRuleReturnScope blockSet16 = null;

        try
        {
            // org\\antlr\\v4\\parse\\ATNBuilder.g:144:2: ( atom | ^( NOT blockSet[true] ) )
            int alt14 = 2;
            int LA14_0 = input.LA(1);
            if ((LA14_0 == DOT || LA14_0 == RANGE || LA14_0 == RULE_REF || LA14_0 == STRING_LITERAL || LA14_0 == TOKEN_REF || (LA14_0 >= SET && LA14_0 <= WILDCARD)))
            {
                alt14 = 1;
            }
            else if ((LA14_0 == NOT))
            {
                alt14 = 2;
            }

            else
            {
                NoViableAltException nvae =
                    new NoViableAltException("", 14, 0, input);
                throw nvae;
            }

            switch (alt14)
            {
                case 1:
                    // org\\antlr\\v4\\parse\\ATNBuilder.g:144:4: atom
                    {
                        PushFollow(FOLLOW_atom_in_astOperand590);
                        atom15 = atom();
                        state._fsp--;

                        p = (atom15 != null ? ((ATNBuilder.atom_return)atom15).p : null);
                    }
                    break;
                case 2:
                    // org\\antlr\\v4\\parse\\ATNBuilder.g:145:4: ^( NOT blockSet[true] )
                    {
                        Match(input, NOT, FOLLOW_NOT_in_astOperand603);
                        Match(input, Token.DOWN, null);
                        PushFollow(FOLLOW_blockSet_in_astOperand605);
                        blockSet16 = blockSet(true);
                        state._fsp--;

                        Match(input, Token.UP, null);

                        p = (blockSet16 != null ? ((ATNBuilder.blockSet_return)blockSet16).p : null);
                    }
                    break;

            }
        }
        catch (RecognitionException re)
        {
            ReportError(re);
            Recover(input, re);
        }
        finally
        {
            // do for sure before leaving
        }
        return p;
    }
    // $ANTLR end "astOperand"



    // $ANTLR start "labeledElement"
    // org\\antlr\\v4\\parse\\ATNBuilder.g:148:1: labeledElement returns [ATNFactory.Handle p] : ( ^( ASSIGN ID element ) | ^( PLUS_ASSIGN ID element ) );
    public ATNFactory.Handle labeledElement()
    {
        ATNFactory.Handle p = null;


        TreeRuleReturnScope element17 = null;
        TreeRuleReturnScope element18 = null;

        try
        {
            // org\\antlr\\v4\\parse\\ATNBuilder.g:149:2: ( ^( ASSIGN ID element ) | ^( PLUS_ASSIGN ID element ) )
            int alt15 = 2;
            int LA15_0 = input.LA(1);
            if ((LA15_0 == ASSIGN))
            {
                alt15 = 1;
            }
            else if ((LA15_0 == PLUS_ASSIGN))
            {
                alt15 = 2;
            }

            else
            {
                NoViableAltException nvae =
                    new NoViableAltException("", 15, 0, input);
                throw nvae;
            }

            switch (alt15)
            {
                case 1:
                    // org\\antlr\\v4\\parse\\ATNBuilder.g:149:4: ^( ASSIGN ID element )
                    {
                        Match(input, ASSIGN, FOLLOW_ASSIGN_in_labeledElement626);
                        Match(input, Token.DOWN, null);
                        Match(input, ID, FOLLOW_ID_in_labeledElement628);
                        PushFollow(FOLLOW_element_in_labeledElement630);
                        element17 = element();
                        state._fsp--;

                        Match(input, Token.UP, null);

                        p = factory.Label((element17 != null ? ((ATNBuilder.element_return)element17).p : null));
                    }
                    break;
                case 2:
                    // org\\antlr\\v4\\parse\\ATNBuilder.g:150:4: ^( PLUS_ASSIGN ID element )
                    {
                        Match(input, PLUS_ASSIGN, FOLLOW_PLUS_ASSIGN_in_labeledElement643);
                        Match(input, Token.DOWN, null);
                        Match(input, ID, FOLLOW_ID_in_labeledElement645);
                        PushFollow(FOLLOW_element_in_labeledElement647);
                        element18 = element();
                        state._fsp--;

                        Match(input, Token.UP, null);

                        p = factory.ListLabel((element18 != null ? ((ATNBuilder.element_return)element18).p : null));
                    }
                    break;

            }
        }
        catch (RecognitionException re)
        {
            ReportError(re);
            Recover(input, re);
        }
        finally
        {
            // do for sure before leaving
        }
        return p;
    }
    // $ANTLR end "labeledElement"


    public class subrule_return : TreeRuleReturnScope
    {

        public ATNFactory.Handle p;
    };


    // $ANTLR start "subrule"
    // org\\antlr\\v4\\parse\\ATNBuilder.g:153:1: subrule returns [ATNFactory.Handle p] : ( ^( OPTIONAL block[$start] ) | ^( CLOSURE block[$start] ) | ^( POSITIVE_CLOSURE block[$start] ) | block[null] );
    public ATNBuilder.subrule_return subrule()
    {
        ATNBuilder.subrule_return retval = new ATNBuilder.subrule_return();
        retval.start = input.LT(1);

        ATNFactory.Handle block19 = null;
        ATNFactory.Handle block20 = null;
        ATNFactory.Handle block21 = null;
        ATNFactory.Handle block22 = null;

        try
        {
            // org\\antlr\\v4\\parse\\ATNBuilder.g:154:2: ( ^( OPTIONAL block[$start] ) | ^( CLOSURE block[$start] ) | ^( POSITIVE_CLOSURE block[$start] ) | block[null] )
            int alt16 = 4;
            switch (input.LA(1))
            {
                case OPTIONAL:
                    {
                        alt16 = 1;
                    }
                    break;
                case CLOSURE:
                    {
                        alt16 = 2;
                    }
                    break;
                case POSITIVE_CLOSURE:
                    {
                        alt16 = 3;
                    }
                    break;
                case BLOCK:
                    {
                        alt16 = 4;
                    }
                    break;
                default:
                    NoViableAltException nvae =
                        new NoViableAltException("", 16, 0, input);
                    throw nvae;
            }
            switch (alt16)
            {
                case 1:
                    // org\\antlr\\v4\\parse\\ATNBuilder.g:154:4: ^( OPTIONAL block[$start] )
                    {
                        Match(input, OPTIONAL, FOLLOW_OPTIONAL_in_subrule668);
                        Match(input, Token.DOWN, null);
                        PushFollow(FOLLOW_block_in_subrule670);
                        block19 = Block(((GrammarAST)retval.start));
                        state._fsp--;

                        Match(input, Token.UP, null);

                        retval.p = block19;
                    }
                    break;
                case 2:
                    // org\\antlr\\v4\\parse\\ATNBuilder.g:155:4: ^( CLOSURE block[$start] )
                    {
                        Match(input, CLOSURE, FOLLOW_CLOSURE_in_subrule682);
                        Match(input, Token.DOWN, null);
                        PushFollow(FOLLOW_block_in_subrule684);
                        block20 = Block(((GrammarAST)retval.start));
                        state._fsp--;

                        Match(input, Token.UP, null);

                        retval.p = block20;
                    }
                    break;
                case 3:
                    // org\\antlr\\v4\\parse\\ATNBuilder.g:156:4: ^( POSITIVE_CLOSURE block[$start] )
                    {
                        Match(input, POSITIVE_CLOSURE, FOLLOW_POSITIVE_CLOSURE_in_subrule696);
                        Match(input, Token.DOWN, null);
                        PushFollow(FOLLOW_block_in_subrule698);
                        block21 = Block(((GrammarAST)retval.start));
                        state._fsp--;

                        Match(input, Token.UP, null);

                        retval.p = block21;
                    }
                    break;
                case 4:
                    // org\\antlr\\v4\\parse\\ATNBuilder.g:157:5: block[null]
                    {
                        PushFollow(FOLLOW_block_in_subrule708);
                        block22 = Block(null);
                        state._fsp--;

                        retval.p = block22;
                    }
                    break;

            }
        }
        catch (RecognitionException re)
        {
            ReportError(re);
            Recover(input, re);
        }
        finally
        {
            // do for sure before leaving
        }
        return retval;
    }
    // $ANTLR end "subrule"


    public class blockSet_return : TreeRuleReturnScope
    {

        public ATNFactory.Handle p;
    };


    // $ANTLR start "blockSet"
    // org\\antlr\\v4\\parse\\ATNBuilder.g:160:1: blockSet[bool invert] returns [ATNFactory.Handle p] : ^( SET ( setElement )+ ) ;
    public ATNBuilder.blockSet_return blockSet(bool invert)
    {
        ATNBuilder.blockSet_return retval = new ATNBuilder.blockSet_return();
        retval.start = input.LT(1);

        TreeRuleReturnScope setElement23 = null;

        List<GrammarAST> alts = new();
        try
        {
            // org\\antlr\\v4\\parse\\ATNBuilder.g:162:2: ( ^( SET ( setElement )+ ) )
            // org\\antlr\\v4\\parse\\ATNBuilder.g:162:4: ^( SET ( setElement )+ )
            {
                Match(input, SET, FOLLOW_SET_in_blockSet742);
                Match(input, Token.DOWN, null);
                // org\\antlr\\v4\\parse\\ATNBuilder.g:162:10: ( setElement )+
                int cnt17 = 0;
            loop17:
                while (true)
                {
                    int alt17 = 2;
                    int LA17_0 = input.LA(1);
                    if ((LA17_0 == LEXER_CHAR_SET || LA17_0 == RANGE || LA17_0 == STRING_LITERAL || LA17_0 == TOKEN_REF))
                    {
                        alt17 = 1;
                    }

                    switch (alt17)
                    {
                        case 1:
                            // org\\antlr\\v4\\parse\\ATNBuilder.g:162:11: setElement
                            {
                                PushFollow(FOLLOW_setElement_in_blockSet745);
                                setElement23 = setElement();
                                state._fsp--;

                                alts.Add((setElement23 != null ? ((GrammarAST)setElement23.start) : null));
                            }
                            break;

                        default:
                            if (cnt17 >= 1) goto exit17;// break loop17;
                            EarlyExitException eee = new EarlyExitException(17, input);
                            throw eee;
                    }
                    cnt17++;
                }
            exit17:
                Match(input, Token.UP, null);

                retval.p = factory.Set(((GrammarAST)retval.start), alts, invert);
            }

        }
        catch (RecognitionException re)
        {
            ReportError(re);
            Recover(input, re);
        }
        finally
        {
            // do for sure before leaving
        }
        return retval;
    }
    // $ANTLR end "blockSet"


    public class setElement_return : TreeRuleReturnScope
    {
    };


    // $ANTLR start "setElement"
    // org\\antlr\\v4\\parse\\ATNBuilder.g:166:1: setElement : ( ^( STRING_LITERAL . ) | ^( TOKEN_REF . ) | STRING_LITERAL | TOKEN_REF | ^( RANGE a= STRING_LITERAL b= STRING_LITERAL ) | LEXER_CHAR_SET );
    public ATNBuilder.setElement_return setElement()
    {
        ATNBuilder.setElement_return retval = new ATNBuilder.setElement_return();
        retval.start = input.LT(1);

        GrammarAST a = null;
        GrammarAST b = null;

        try
        {
            // org\\antlr\\v4\\parse\\ATNBuilder.g:167:2: ( ^( STRING_LITERAL . ) | ^( TOKEN_REF . ) | STRING_LITERAL | TOKEN_REF | ^( RANGE a= STRING_LITERAL b= STRING_LITERAL ) | LEXER_CHAR_SET )
            int alt18 = 6;
            switch (input.LA(1))
            {
                case STRING_LITERAL:
                    {
                        int LA18_1 = input.LA(2);
                        if ((LA18_1 == DOWN))
                        {
                            alt18 = 1;
                        }
                        else if ((LA18_1 == UP || LA18_1 == LEXER_CHAR_SET || LA18_1 == RANGE || LA18_1 == STRING_LITERAL || LA18_1 == TOKEN_REF))
                        {
                            alt18 = 3;
                        }

                        else
                        {
                            int nvaeMark = input.Mark();
                            try
                            {
                                input.Consume();
                                NoViableAltException nvae2 =
                                    new NoViableAltException("", 18, 1, input);
                                throw nvae2;
                            }
                            finally
                            {
                                input.Rewind(nvaeMark);
                            }
                        }

                    }
                    break;
                case TOKEN_REF:
                    {
                        int LA18_2 = input.LA(2);
                        if ((LA18_2 == DOWN))
                        {
                            alt18 = 2;
                        }
                        else if ((LA18_2 == UP || LA18_2 == LEXER_CHAR_SET || LA18_2 == RANGE || LA18_2 == STRING_LITERAL || LA18_2 == TOKEN_REF))
                        {
                            alt18 = 4;
                        }

                        else
                        {
                            int nvaeMark = input.Mark();
                            try
                            {
                                input.Consume();
                                NoViableAltException nvae3 =
                                    new NoViableAltException("", 18, 2, input);
                                throw nvae3;
                            }
                            finally
                            {
                                input.Rewind(nvaeMark);
                            }
                        }

                    }
                    break;
                case RANGE:
                    {
                        alt18 = 5;
                    }
                    break;
                case LEXER_CHAR_SET:
                    {
                        alt18 = 6;
                    }
                    break;
                default:
                    NoViableAltException nvae =
                        new NoViableAltException("", 18, 0, input);
                    throw nvae;
            }
            switch (alt18)
            {
                case 1:
                    // org\\antlr\\v4\\parse\\ATNBuilder.g:167:4: ^( STRING_LITERAL . )
                    {
                        Match(input, STRING_LITERAL, FOLLOW_STRING_LITERAL_in_setElement766);
                        Match(input, Token.DOWN, null);
                        MatchAny(input);
                        Match(input, Token.UP, null);

                    }
                    break;
                case 2:
                    // org\\antlr\\v4\\parse\\ATNBuilder.g:168:4: ^( TOKEN_REF . )
                    {
                        Match(input, TOKEN_REF, FOLLOW_TOKEN_REF_in_setElement775);
                        Match(input, Token.DOWN, null);
                        MatchAny(input);
                        Match(input, Token.UP, null);

                    }
                    break;
                case 3:
                    // org\\antlr\\v4\\parse\\ATNBuilder.g:169:4: STRING_LITERAL
                    {
                        Match(input, STRING_LITERAL, FOLLOW_STRING_LITERAL_in_setElement783);
                    }
                    break;
                case 4:
                    // org\\antlr\\v4\\parse\\ATNBuilder.g:170:4: TOKEN_REF
                    {
                        Match(input, TOKEN_REF, FOLLOW_TOKEN_REF_in_setElement788);
                    }
                    break;
                case 5:
                    // org\\antlr\\v4\\parse\\ATNBuilder.g:171:4: ^( RANGE a= STRING_LITERAL b= STRING_LITERAL )
                    {
                        Match(input, RANGE, FOLLOW_RANGE_in_setElement794);
                        Match(input, Token.DOWN, null);
                        a = (GrammarAST)Match(input, STRING_LITERAL, FOLLOW_STRING_LITERAL_in_setElement798);
                        b = (GrammarAST)Match(input, STRING_LITERAL, FOLLOW_STRING_LITERAL_in_setElement802);
                        Match(input, Token.UP, null);

                    }
                    break;
                case 6:
                    // org\\antlr\\v4\\parse\\ATNBuilder.g:172:9: LEXER_CHAR_SET
                    {
                        Match(input, LEXER_CHAR_SET, FOLLOW_LEXER_CHAR_SET_in_setElement813);
                    }
                    break;

            }
        }
        catch (RecognitionException re)
        {
            ReportError(re);
            Recover(input, re);
        }
        finally
        {
            // do for sure before leaving
        }
        return retval;
    }
    // $ANTLR end "setElement"


    public class atom_return : TreeRuleReturnScope
    {

        public ATNFactory.Handle p;
    };


    // $ANTLR start "atom"
    // org\\antlr\\v4\\parse\\ATNBuilder.g:175:1: atom returns [ATNFactory.Handle p] : ( range | ^( DOT ID terminal ) | ^( DOT ID ruleref ) | ^( WILDCARD . ) | WILDCARD | blockSet[false] | terminal | ruleref );
    public ATNBuilder.atom_return atom()
    {
        ATNBuilder.atom_return retval = new ATNBuilder.atom_return();
        retval.start = input.LT(1);

        ATNFactory.Handle range24 = null;
        TreeRuleReturnScope terminal25 = null;
        ATNFactory.Handle ruleref26 = null;
        TreeRuleReturnScope blockSet27 = null;
        TreeRuleReturnScope terminal28 = null;
        ATNFactory.Handle ruleref29 = null;

        try
        {
            // org\\antlr\\v4\\parse\\ATNBuilder.g:176:2: ( range | ^( DOT ID terminal ) | ^( DOT ID ruleref ) | ^( WILDCARD . ) | WILDCARD | blockSet[false] | terminal | ruleref )
            int alt19 = 8;
            switch (input.LA(1))
            {
                case RANGE:
                    {
                        alt19 = 1;
                    }
                    break;
                case DOT:
                    {
                        int LA19_2 = input.LA(2);
                        if ((LA19_2 == DOWN))
                        {
                            int LA19_7 = input.LA(3);
                            if ((LA19_7 == ID))
                            {
                                int LA19_10 = input.LA(4);
                                if ((LA19_10 == STRING_LITERAL || LA19_10 == TOKEN_REF))
                                {
                                    alt19 = 2;
                                }
                                else if ((LA19_10 == RULE_REF))
                                {
                                    alt19 = 3;
                                }

                                else
                                {
                                    int nvaeMark = input.Mark();
                                    try
                                    {
                                        for (int nvaeConsume = 0; nvaeConsume < 4 - 1; nvaeConsume++)
                                        {
                                            input.Consume();
                                        }
                                        NoViableAltException nvae4 =
                                            new NoViableAltException("", 19, 10, input);
                                        throw nvae4;
                                    }
                                    finally
                                    {
                                        input.Rewind(nvaeMark);
                                    }
                                }

                            }

                            else
                            {
                                int nvaeMark = input.Mark();
                                try
                                {
                                    for (int nvaeConsume = 0; nvaeConsume < 3 - 1; nvaeConsume++)
                                    {
                                        input.Consume();
                                    }
                                    NoViableAltException nvae2 =
                                        new NoViableAltException("", 19, 7, input);
                                    throw nvae2;
                                }
                                finally
                                {
                                    input.Rewind(nvaeMark);
                                }
                            }

                        }

                        else
                        {
                            int nvaeMark = input.Mark();
                            try
                            {
                                input.Consume();
                                NoViableAltException nvae3 =
                                    new NoViableAltException("", 19, 2, input);
                                throw nvae3;
                            }
                            finally
                            {
                                input.Rewind(nvaeMark);
                            }
                        }

                    }
                    break;
                case WILDCARD:
                    {
                        int LA19_3 = input.LA(2);
                        if ((LA19_3 == DOWN))
                        {
                            alt19 = 4;
                        }
                        else if ((LA19_3 == EOF || (LA19_3 >= UP && LA19_3 <= ACTION) || LA19_3 == ASSIGN || LA19_3 == DOT || LA19_3 == LEXER_CHAR_SET || LA19_3 == NOT || LA19_3 == PLUS_ASSIGN || LA19_3 == RANGE || LA19_3 == RULE_REF || LA19_3 == SEMPRED || LA19_3 == STRING_LITERAL || LA19_3 == TOKEN_REF || (LA19_3 >= BLOCK && LA19_3 <= CLOSURE) || (LA19_3 >= OPTIONAL && LA19_3 <= POSITIVE_CLOSURE) || (LA19_3 >= SET && LA19_3 <= WILDCARD)))
                        {
                            alt19 = 5;
                        }

                        else
                        {
                            int nvaeMark = input.Mark();
                            try
                            {
                                input.Consume();
                                NoViableAltException nvae4 =
                                    new NoViableAltException("", 19, 3, input);
                                throw nvae4;
                            }
                            finally
                            {
                                input.Rewind(nvaeMark);
                            }
                        }

                    }
                    break;
                case SET:
                    {
                        alt19 = 6;
                    }
                    break;
                case STRING_LITERAL:
                case TOKEN_REF:
                    {
                        alt19 = 7;
                    }
                    break;
                case RULE_REF:
                    {
                        alt19 = 8;
                    }
                    break;
                default:
                    NoViableAltException nvae =
                        new NoViableAltException("", 19, 0, input);
                    throw nvae;
            }
            switch (alt19)
            {
                case 1:
                    // org\\antlr\\v4\\parse\\ATNBuilder.g:176:4: range
                    {
                        PushFollow(FOLLOW_range_in_atom828);
                        range24 = range();
                        state._fsp--;

                        retval.p = range24;
                    }
                    break;
                case 2:
                    // org\\antlr\\v4\\parse\\ATNBuilder.g:177:4: ^( DOT ID terminal )
                    {
                        Match(input, DOT, FOLLOW_DOT_in_atom840);
                        Match(input, Token.DOWN, null);
                        Match(input, ID, FOLLOW_ID_in_atom842);
                        PushFollow(FOLLOW_terminal_in_atom844);
                        terminal25 = terminal();
                        state._fsp--;

                        Match(input, Token.UP, null);

                        retval.p = (terminal25 != null ? ((ATNBuilder.terminal_return)terminal25).p : null);
                    }
                    break;
                case 3:
                    // org\\antlr\\v4\\parse\\ATNBuilder.g:178:4: ^( DOT ID ruleref )
                    {
                        Match(input, DOT, FOLLOW_DOT_in_atom854);
                        Match(input, Token.DOWN, null);
                        Match(input, ID, FOLLOW_ID_in_atom856);
                        PushFollow(FOLLOW_ruleref_in_atom858);
                        ruleref26 = ruleref();
                        state._fsp--;

                        Match(input, Token.UP, null);

                        retval.p = ruleref26;
                    }
                    break;
                case 4:
                    // org\\antlr\\v4\\parse\\ATNBuilder.g:179:7: ^( WILDCARD . )
                    {
                        Match(input, WILDCARD, FOLLOW_WILDCARD_in_atom871);
                        Match(input, Token.DOWN, null);
                        MatchAny(input);
                        Match(input, Token.UP, null);

                        retval.p = factory.Wildcard(((GrammarAST)retval.start));
                    }
                    break;
                case 5:
                    // org\\antlr\\v4\\parse\\ATNBuilder.g:180:7: WILDCARD
                    {
                        Match(input, WILDCARD, FOLLOW_WILDCARD_in_atom886);
                        retval.p = factory.Wildcard(((GrammarAST)retval.start));
                    }
                    break;
                case 6:
                    // org\\antlr\\v4\\parse\\ATNBuilder.g:181:7: blockSet[false]
                    {
                        PushFollow(FOLLOW_blockSet_in_atom899);
                        blockSet27 = blockSet(false);
                        state._fsp--;

                        retval.p = (blockSet27 != null ? ((ATNBuilder.blockSet_return)blockSet27).p : null);
                    }
                    break;
                case 7:
                    // org\\antlr\\v4\\parse\\ATNBuilder.g:182:9: terminal
                    {
                        PushFollow(FOLLOW_terminal_in_atom914);
                        terminal28 = terminal();
                        state._fsp--;

                        retval.p = (terminal28 != null ? ((ATNBuilder.terminal_return)terminal28).p : null);
                    }
                    break;
                case 8:
                    // org\\antlr\\v4\\parse\\ATNBuilder.g:183:9: ruleref
                    {
                        PushFollow(FOLLOW_ruleref_in_atom929);
                        ruleref29 = ruleref();
                        state._fsp--;

                        retval.p = ruleref29;
                    }
                    break;

            }
        }
        catch (RecognitionException re)
        {
            ReportError(re);
            Recover(input, re);
        }
        finally
        {
            // do for sure before leaving
        }
        return retval;
    }
    // $ANTLR end "atom"



    // $ANTLR start "ruleref"
    // org\\antlr\\v4\\parse\\ATNBuilder.g:186:1: ruleref returns [ATNFactory.Handle p] : ( ^( RULE_REF ( ARG_ACTION )? ^( ELEMENT_OPTIONS ( . )* ) ) | ^( RULE_REF ( ARG_ACTION )? ) | RULE_REF );
    public ATNFactory.Handle ruleref()
    {
        ATNFactory.Handle p = null;


        GrammarAST RULE_REF30 = null;
        GrammarAST RULE_REF31 = null;
        GrammarAST RULE_REF32 = null;

        try
        {
            // org\\antlr\\v4\\parse\\ATNBuilder.g:187:5: ( ^( RULE_REF ( ARG_ACTION )? ^( ELEMENT_OPTIONS ( . )* ) ) | ^( RULE_REF ( ARG_ACTION )? ) | RULE_REF )
            int alt23 = 3;
            int LA23_0 = input.LA(1);
            if ((LA23_0 == RULE_REF))
            {
                int LA23_1 = input.LA(2);
                if ((LA23_1 == DOWN))
                {
                    switch (input.LA(3))
                    {
                        case ARG_ACTION:
                            {
                                int LA23_4 = input.LA(4);
                                if ((LA23_4 == ELEMENT_OPTIONS))
                                {
                                    alt23 = 1;
                                }
                                else if ((LA23_4 == UP))
                                {
                                    alt23 = 2;
                                }

                                else
                                {
                                    int nvaeMark2 = input.Mark();
                                    try
                                    {
                                        for (int nvaeConsume = 0; nvaeConsume < 4 - 1; nvaeConsume++)
                                        {
                                            input.Consume();
                                        }
                                        NoViableAltException nvae =
                                            new NoViableAltException("", 23, 4, input);
                                        throw nvae;
                                    }
                                    finally
                                    {
                                        input.Rewind(nvaeMark2);
                                    }
                                }

                            }
                            break;
                        case ELEMENT_OPTIONS:
                            {
                                alt23 = 1;
                            }
                            break;
                        case UP:
                            {
                                alt23 = 2;
                            }
                            break;
                        default:
                            int nvaeMark = input.Mark();
                            try
                            {
                                for (int nvaeConsume = 0; nvaeConsume < 3 - 1; nvaeConsume++)
                                {
                                    input.Consume();
                                }
                                NoViableAltException nvae =
                                    new NoViableAltException("", 23, 2, input);
                                throw nvae;
                            }
                            finally
                            {
                                input.Rewind(nvaeMark);
                            }
                    }
                }
                else if ((LA23_1 == EOF || (LA23_1 >= UP && LA23_1 <= ACTION) || LA23_1 == ASSIGN || LA23_1 == DOT || LA23_1 == LEXER_CHAR_SET || LA23_1 == NOT || LA23_1 == PLUS_ASSIGN || LA23_1 == RANGE || LA23_1 == RULE_REF || LA23_1 == SEMPRED || LA23_1 == STRING_LITERAL || LA23_1 == TOKEN_REF || (LA23_1 >= BLOCK && LA23_1 <= CLOSURE) || (LA23_1 >= OPTIONAL && LA23_1 <= POSITIVE_CLOSURE) || (LA23_1 >= SET && LA23_1 <= WILDCARD)))
                {
                    alt23 = 3;
                }

                else
                {
                    int nvaeMark = input.Mark();
                    try
                    {
                        input.Consume();
                        NoViableAltException nvae =
                            new NoViableAltException("", 23, 1, input);
                        throw nvae;
                    }
                    finally
                    {
                        input.Rewind(nvaeMark);
                    }
                }

            }

            else
            {
                NoViableAltException nvae =
                    new NoViableAltException("", 23, 0, input);
                throw nvae;
            }

            switch (alt23)
            {
                case 1:
                    // org\\antlr\\v4\\parse\\ATNBuilder.g:187:7: ^( RULE_REF ( ARG_ACTION )? ^( ELEMENT_OPTIONS ( . )* ) )
                    {
                        RULE_REF30 = (GrammarAST)Match(input, RULE_REF, FOLLOW_RULE_REF_in_ruleref957);
                        Match(input, Token.DOWN, null);
                        // org\\antlr\\v4\\parse\\ATNBuilder.g:187:18: ( ARG_ACTION )?
                        int alt20 = 2;
                        int LA20_0 = input.LA(1);
                        if ((LA20_0 == ARG_ACTION))
                        {
                            alt20 = 1;
                        }
                        switch (alt20)
                        {
                            case 1:
                                // org\\antlr\\v4\\parse\\ATNBuilder.g:187:18: ARG_ACTION
                                {
                                    Match(input, ARG_ACTION, FOLLOW_ARG_ACTION_in_ruleref959);
                                }
                                break;

                        }

                        Match(input, ELEMENT_OPTIONS, FOLLOW_ELEMENT_OPTIONS_in_ruleref963);
                        if (input.LA(1) == Token.DOWN)
                        {
                            Match(input, Token.DOWN, null);
                        // org\\antlr\\v4\\parse\\ATNBuilder.g:187:48: ( . )*
                        loop21:
                            while (true)
                            {
                                int alt21 = 2;
                                int LA21_0 = input.LA(1);
                                if (((LA21_0 >= ACTION && LA21_0 <= WILDCARD)))
                                {
                                    alt21 = 1;
                                }
                                else if ((LA21_0 == UP))
                                {
                                    alt21 = 2;
                                }

                                switch (alt21)
                                {
                                    case 1:
                                        // org\\antlr\\v4\\parse\\ATNBuilder.g:187:48: .
                                        {
                                            MatchAny(input);
                                        }
                                        break;

                                    default:
                                        goto exit12;
                                        //break loop21;
                                }
                            }
                        exit12:
                            Match(input, Token.UP, null);
                        }

                        Match(input, Token.UP, null);

                        p = factory.RuleRef(RULE_REF30);
                    }
                    break;
                case 2:
                    // org\\antlr\\v4\\parse\\ATNBuilder.g:188:7: ^( RULE_REF ( ARG_ACTION )? )
                    {
                        RULE_REF31 = (GrammarAST)Match(input, RULE_REF, FOLLOW_RULE_REF_in_ruleref980);
                        if (input.LA(1) == Token.DOWN)
                        {
                            Match(input, Token.DOWN, null);
                            // org\\antlr\\v4\\parse\\ATNBuilder.g:188:18: ( ARG_ACTION )?
                            int alt22 = 2;
                            int LA22_0 = input.LA(1);
                            if ((LA22_0 == ARG_ACTION))
                            {
                                alt22 = 1;
                            }
                            switch (alt22)
                            {
                                case 1:
                                    // org\\antlr\\v4\\parse\\ATNBuilder.g:188:18: ARG_ACTION
                                    {
                                        Match(input, ARG_ACTION, FOLLOW_ARG_ACTION_in_ruleref982);
                                    }
                                    break;

                            }

                            Match(input, Token.UP, null);
                        }

                        p = factory.RuleRef(RULE_REF31);
                    }
                    break;
                case 3:
                    // org\\antlr\\v4\\parse\\ATNBuilder.g:189:7: RULE_REF
                    {
                        RULE_REF32 = (GrammarAST)Match(input, RULE_REF, FOLLOW_RULE_REF_in_ruleref1001);
                        p = factory.RuleRef(RULE_REF32);
                    }
                    break;

            }
        }
        catch (RecognitionException re)
        {
            ReportError(re);
            Recover(input, re);
        }
        finally
        {
            // do for sure before leaving
        }
        return p;
    }
    // $ANTLR end "ruleref"



    // $ANTLR start "range"
    // org\\antlr\\v4\\parse\\ATNBuilder.g:192:1: range returns [ATNFactory.Handle p] : ^( RANGE a= STRING_LITERAL b= STRING_LITERAL ) ;
    public ATNFactory.Handle range()
    {
        ATNFactory.Handle p = null;


        GrammarAST a = null;
        GrammarAST b = null;

        try
        {
            // org\\antlr\\v4\\parse\\ATNBuilder.g:193:5: ( ^( RANGE a= STRING_LITERAL b= STRING_LITERAL ) )
            // org\\antlr\\v4\\parse\\ATNBuilder.g:193:7: ^( RANGE a= STRING_LITERAL b= STRING_LITERAL )
            {
                Match(input, RANGE, FOLLOW_RANGE_in_range1035);
                Match(input, Token.DOWN, null);
                a = (GrammarAST)Match(input, STRING_LITERAL, FOLLOW_STRING_LITERAL_in_range1039);
                b = (GrammarAST)Match(input, STRING_LITERAL, FOLLOW_STRING_LITERAL_in_range1043);
                Match(input, Token.UP, null);

                p = factory.Range(a, b);
            }

        }
        catch (RecognitionException re)
        {
            ReportError(re);
            Recover(input, re);
        }
        finally
        {
            // do for sure before leaving
        }
        return p;
    }
    // $ANTLR end "range"


    public class terminal_return : TreeRuleReturnScope
    {

        public ATNFactory.Handle p;
    };


    // $ANTLR start "terminal"
    // org\\antlr\\v4\\parse\\ATNBuilder.g:196:1: terminal returns [ATNFactory.Handle p] : ( ^( STRING_LITERAL . ) | STRING_LITERAL | ^( TOKEN_REF ARG_ACTION . ) | ^( TOKEN_REF . ) | TOKEN_REF );
    public ATNBuilder.terminal_return terminal()
    {
        ATNBuilder.terminal_return retval = new ATNBuilder.terminal_return();
        retval.start = input.LT(1);

        try
        {
            // org\\antlr\\v4\\parse\\ATNBuilder.g:197:5: ( ^( STRING_LITERAL . ) | STRING_LITERAL | ^( TOKEN_REF ARG_ACTION . ) | ^( TOKEN_REF . ) | TOKEN_REF )
            int alt24 = 5;
            int LA24_0 = input.LA(1);
            if ((LA24_0 == STRING_LITERAL))
            {
                int LA24_1 = input.LA(2);
                if ((LA24_1 == DOWN))
                {
                    alt24 = 1;
                }
                else if ((LA24_1 == EOF || (LA24_1 >= UP && LA24_1 <= ACTION) || LA24_1 == ASSIGN || LA24_1 == DOT || LA24_1 == LEXER_CHAR_SET || LA24_1 == NOT || LA24_1 == PLUS_ASSIGN || LA24_1 == RANGE || LA24_1 == RULE_REF || LA24_1 == SEMPRED || LA24_1 == STRING_LITERAL || LA24_1 == TOKEN_REF || (LA24_1 >= BLOCK && LA24_1 <= CLOSURE) || (LA24_1 >= OPTIONAL && LA24_1 <= POSITIVE_CLOSURE) || (LA24_1 >= SET && LA24_1 <= WILDCARD)))
                {
                    alt24 = 2;
                }

                else
                {
                    int nvaeMark = input.Mark();
                    try
                    {
                        input.Consume();
                        NoViableAltException nvae =
                            new NoViableAltException("", 24, 1, input);
                        throw nvae;
                    }
                    finally
                    {
                        input.Rewind(nvaeMark);
                    }
                }

            }
            else if ((LA24_0 == TOKEN_REF))
            {
                int LA24_2 = input.LA(2);
                if ((LA24_2 == DOWN))
                {
                    int LA24_5 = input.LA(3);
                    if ((LA24_5 == ARG_ACTION))
                    {
                        int LA24_7 = input.LA(4);
                        if (((LA24_7 >= ACTION && LA24_7 <= WILDCARD)))
                        {
                            alt24 = 3;
                        }
                        else if (((LA24_7 >= DOWN && LA24_7 <= UP)))
                        {
                            alt24 = 4;
                        }

                        else
                        {
                            int nvaeMark = input.Mark();
                            try
                            {
                                for (int nvaeConsume = 0; nvaeConsume < 4 - 1; nvaeConsume++)
                                {
                                    input.Consume();
                                }
                                NoViableAltException nvae =
                                    new NoViableAltException("", 24, 7, input);
                                throw nvae;
                            }
                            finally
                            {
                                input.Rewind(nvaeMark);
                            }
                        }

                    }
                    else if (((LA24_5 >= ACTION && LA24_5 <= ACTION_STRING_LITERAL) || (LA24_5 >= ARG_OR_CHARSET && LA24_5 <= WILDCARD)))
                    {
                        alt24 = 4;
                    }

                    else
                    {
                        int nvaeMark = input.Mark();
                        try
                        {
                            for (int nvaeConsume = 0; nvaeConsume < 3 - 1; nvaeConsume++)
                            {
                                input.Consume();
                            }
                            NoViableAltException nvae =
                                new NoViableAltException("", 24, 5, input);
                            throw nvae;
                        }
                        finally
                        {
                            input.Rewind(nvaeMark);
                        }
                    }

                }
                else if ((LA24_2 == EOF || (LA24_2 >= UP && LA24_2 <= ACTION) || LA24_2 == ASSIGN || LA24_2 == DOT || LA24_2 == LEXER_CHAR_SET || LA24_2 == NOT || LA24_2 == PLUS_ASSIGN || LA24_2 == RANGE || LA24_2 == RULE_REF || LA24_2 == SEMPRED || LA24_2 == STRING_LITERAL || LA24_2 == TOKEN_REF || (LA24_2 >= BLOCK && LA24_2 <= CLOSURE) || (LA24_2 >= OPTIONAL && LA24_2 <= POSITIVE_CLOSURE) || (LA24_2 >= SET && LA24_2 <= WILDCARD)))
                {
                    alt24 = 5;
                }

                else
                {
                    int nvaeMark = input.Mark();
                    try
                    {
                        input.Consume();
                        NoViableAltException nvae =
                            new NoViableAltException("", 24, 2, input);
                        throw nvae;
                    }
                    finally
                    {
                        input.Rewind(nvaeMark);
                    }
                }

            }

            else
            {
                NoViableAltException nvae =
                    new NoViableAltException("", 24, 0, input);
                throw nvae;
            }

            switch (alt24)
            {
                case 1:
                    // org\\antlr\\v4\\parse\\ATNBuilder.g:197:8: ^( STRING_LITERAL . )
                    {
                        Match(input, STRING_LITERAL, FOLLOW_STRING_LITERAL_in_terminal1069);
                        Match(input, Token.DOWN, null);
                        MatchAny(input);
                        Match(input, Token.UP, null);

                        retval.p = factory.StringLiteral((TerminalAST)((GrammarAST)retval.start));
                    }
                    break;
                case 2:
                    // org\\antlr\\v4\\parse\\ATNBuilder.g:198:7: STRING_LITERAL
                    {
                        Match(input, STRING_LITERAL, FOLLOW_STRING_LITERAL_in_terminal1084);
                        retval.p = factory.StringLiteral((TerminalAST)((GrammarAST)retval.start));
                    }
                    break;
                case 3:
                    // org\\antlr\\v4\\parse\\ATNBuilder.g:199:7: ^( TOKEN_REF ARG_ACTION . )
                    {
                        Match(input, TOKEN_REF, FOLLOW_TOKEN_REF_in_terminal1098);
                        Match(input, Token.DOWN, null);
                        Match(input, ARG_ACTION, FOLLOW_ARG_ACTION_in_terminal1100);
                        MatchAny(input);
                        Match(input, Token.UP, null);

                        retval.p = factory.TokenRef((TerminalAST)((GrammarAST)retval.start));
                    }
                    break;
                case 4:
                    // org\\antlr\\v4\\parse\\ATNBuilder.g:200:7: ^( TOKEN_REF . )
                    {
                        Match(input, TOKEN_REF, FOLLOW_TOKEN_REF_in_terminal1114);
                        Match(input, Token.DOWN, null);
                        MatchAny(input);
                        Match(input, Token.UP, null);

                        retval.p = factory.TokenRef((TerminalAST)((GrammarAST)retval.start));
                    }
                    break;
                case 5:
                    // org\\antlr\\v4\\parse\\ATNBuilder.g:201:7: TOKEN_REF
                    {
                        Match(input, TOKEN_REF, FOLLOW_TOKEN_REF_in_terminal1130);
                        retval.p = factory.TokenRef((TerminalAST)((GrammarAST)retval.start));
                    }
                    break;

            }
        }
        catch (RecognitionException re)
        {
            ReportError(re);
            Recover(input, re);
        }
        finally
        {
            // do for sure before leaving
        }
        return retval;
    }
    // $ANTLR end "terminal"



    // $ANTLR start "elementOptions"
    // org\\antlr\\v4\\parse\\ATNBuilder.g:204:1: elementOptions : ^( ELEMENT_OPTIONS ( elementOption )* ) ;
    public void elementOptions()
    {
        try
        {
            // org\\antlr\\v4\\parse\\ATNBuilder.g:205:2: ( ^( ELEMENT_OPTIONS ( elementOption )* ) )
            // org\\antlr\\v4\\parse\\ATNBuilder.g:205:4: ^( ELEMENT_OPTIONS ( elementOption )* )
            {
                Match(input, ELEMENT_OPTIONS, FOLLOW_ELEMENT_OPTIONS_in_elementOptions1151);
                if (input.LA(1) == Token.DOWN)
                {
                    Match(input, Token.DOWN, null);
                // org\\antlr\\v4\\parse\\ATNBuilder.g:205:22: ( elementOption )*
                loop25:
                    while (true)
                    {
                        int alt25 = 2;
                        int LA25_0 = input.LA(1);
                        if ((LA25_0 == ASSIGN || LA25_0 == ID))
                        {
                            alt25 = 1;
                        }

                        switch (alt25)
                        {
                            case 1:
                                // org\\antlr\\v4\\parse\\ATNBuilder.g:205:22: elementOption
                                {
                                    PushFollow(FOLLOW_elementOption_in_elementOptions1153);
                                    elementOption();
                                    state._fsp--;

                                }
                                break;

                            default:
                                goto exit25;
                                //break loop25;
                        }
                    }
                exit25:
                    Match(input, Token.UP, null);
                }

            }

        }
        catch (RecognitionException re)
        {
            ReportError(re);
            Recover(input, re);
        }
        finally
        {
            // do for sure before leaving
        }
    }
    // $ANTLR end "elementOptions"



    // $ANTLR start "elementOption"
    // org\\antlr\\v4\\parse\\ATNBuilder.g:208:1: elementOption : ( ID | ^( ASSIGN ID ID ) | ^( ASSIGN ID STRING_LITERAL ) | ^( ASSIGN ID ACTION ) | ^( ASSIGN ID INT ) );
    public void elementOption()
    {
        try
        {
            // org\\antlr\\v4\\parse\\ATNBuilder.g:209:2: ( ID | ^( ASSIGN ID ID ) | ^( ASSIGN ID STRING_LITERAL ) | ^( ASSIGN ID ACTION ) | ^( ASSIGN ID INT ) )
            int alt26 = 5;
            int LA26_0 = input.LA(1);
            if ((LA26_0 == ID))
            {
                alt26 = 1;
            }
            else if ((LA26_0 == ASSIGN))
            {
                int LA26_2 = input.LA(2);
                if ((LA26_2 == DOWN))
                {
                    int LA26_3 = input.LA(3);
                    if ((LA26_3 == ID))
                    {
                        switch (input.LA(4))
                        {
                            case ID:
                                {
                                    alt26 = 2;
                                }
                                break;
                            case STRING_LITERAL:
                                {
                                    alt26 = 3;
                                }
                                break;
                            case ACTION:
                                {
                                    alt26 = 4;
                                }
                                break;
                            case INT:
                                {
                                    alt26 = 5;
                                }
                                break;
                            default:
                                int nvaeMark = input.Mark();
                                try
                                {
                                    for (int nvaeConsume = 0; nvaeConsume < 4 - 1; nvaeConsume++)
                                    {
                                        input.Consume();
                                    }
                                    NoViableAltException nvae =
                                        new NoViableAltException("", 26, 4, input);
                                    throw nvae;
                                }
                                finally
                                {
                                    input.Rewind(nvaeMark);
                                }
                        }
                    }

                    else
                    {
                        int nvaeMark = input.Mark();
                        try
                        {
                            for (int nvaeConsume = 0; nvaeConsume < 3 - 1; nvaeConsume++)
                            {
                                input.Consume();
                            }
                            NoViableAltException nvae =
                                new NoViableAltException("", 26, 3, input);
                            throw nvae;
                        }
                        finally
                        {
                            input.Rewind(nvaeMark);
                        }
                    }

                }

                else
                {
                    int nvaeMark = input.Mark();
                    try
                    {
                        input.Consume();
                        NoViableAltException nvae =
                            new NoViableAltException("", 26, 2, input);
                        throw nvae;
                    }
                    finally
                    {
                        input.Rewind(nvaeMark);
                    }
                }

            }

            else
            {
                NoViableAltException nvae =
                    new NoViableAltException("", 26, 0, input);
                throw nvae;
            }

            switch (alt26)
            {
                case 1:
                    // org\\antlr\\v4\\parse\\ATNBuilder.g:209:4: ID
                    {
                        Match(input, ID, FOLLOW_ID_in_elementOption1166);
                    }
                    break;
                case 2:
                    // org\\antlr\\v4\\parse\\ATNBuilder.g:210:4: ^( ASSIGN ID ID )
                    {
                        Match(input, ASSIGN, FOLLOW_ASSIGN_in_elementOption1172);
                        Match(input, Token.DOWN, null);
                        Match(input, ID, FOLLOW_ID_in_elementOption1174);
                        Match(input, ID, FOLLOW_ID_in_elementOption1176);
                        Match(input, Token.UP, null);

                    }
                    break;
                case 3:
                    // org\\antlr\\v4\\parse\\ATNBuilder.g:211:4: ^( ASSIGN ID STRING_LITERAL )
                    {
                        Match(input, ASSIGN, FOLLOW_ASSIGN_in_elementOption1183);
                        Match(input, Token.DOWN, null);
                        Match(input, ID, FOLLOW_ID_in_elementOption1185);
                        Match(input, STRING_LITERAL, FOLLOW_STRING_LITERAL_in_elementOption1187);
                        Match(input, Token.UP, null);

                    }
                    break;
                case 4:
                    // org\\antlr\\v4\\parse\\ATNBuilder.g:212:4: ^( ASSIGN ID ACTION )
                    {
                        Match(input, ASSIGN, FOLLOW_ASSIGN_in_elementOption1194);
                        Match(input, Token.DOWN, null);
                        Match(input, ID, FOLLOW_ID_in_elementOption1196);
                        Match(input, ACTION, FOLLOW_ACTION_in_elementOption1198);
                        Match(input, Token.UP, null);

                    }
                    break;
                case 5:
                    // org\\antlr\\v4\\parse\\ATNBuilder.g:213:4: ^( ASSIGN ID INT )
                    {
                        Match(input, ASSIGN, FOLLOW_ASSIGN_in_elementOption1205);
                        Match(input, Token.DOWN, null);
                        Match(input, ID, FOLLOW_ID_in_elementOption1207);
                        Match(input, INT, FOLLOW_INT_in_elementOption1209);
                        Match(input, Token.UP, null);
                    }
                    break;

            }
        }
        catch (RecognitionException re)
        {
            ReportError(re);
            Recover(input, re);
        }
        finally
        {
            // do for sure before leaving
        }
    }
    // $ANTLR end "elementOption"

    // Delegated rules



    //static final short[] DFA10_eot = DFA.unpackEncodedString(DFA10_eotS);
    //static final short[] DFA10_eof = DFA.unpackEncodedString(DFA10_eofS);
    //static final char[] DFA10_min = DFA.unpackEncodedStringToUnsignedChars(DFA10_minS);
    //static final char[] DFA10_max = DFA.unpackEncodedStringToUnsignedChars(DFA10_maxS);
    //static final short[] DFA10_accept = DFA.unpackEncodedString(DFA10_acceptS);
    //static final short[] DFA10_special = DFA.unpackEncodedString(DFA10_specialS);
    //static final short[][] DFA10_transition;
    static readonly short[] DFA10_eot = RuntimeUtils.Convert(new char[] { '\u0015', '\uffff' });
    static readonly short[] DFA10_eof = RuntimeUtils.Convert(new char[] { '\u0015', '\uffff' });
    static readonly char[] DFA10_min = new char[] { '\u0001', '\u0045', '\u0001', '\uffff', '\u0001', '\u0002', '\u0001', '\u0004', '\u0001', '\u0002', '\u0002', '\uffff', '\u0002', '\u0003', '\u0001', '\u0002', '\u0001', '\u0004', '\u0001', '\u001c', '\u0001', '\u0004', '\u0008', '\u0003' };
    static readonly char[] DFA10_max = new char[] { '\u0001', '\u004c', '\u0001', '\uffff', '\u0001', '\u0002', '\u0001', '\u0053', '\u0001', '\u0002', '\u0002', '\uffff', '\u0002', '\u001c', '\u0001', '\u0002', '\u0001', '\u0053', '\u0001', '\u001c', '\u0001', '\u003b', '\u0004', '\u0003', '\u0004', '\u001c' };
    static readonly short[] DFA10_accept = RuntimeUtils.Convert(new char[] { '\u0001', '\uffff', '\u0001', '\u0001', '\u0003', '\uffff', '\u0001', '\u0002', '\u0001', '\u0003', '\u000e', '\uffff' });
    static readonly short[] DFA10_special = RuntimeUtils.Convert(new char[] { '\u0015', '\uffff', '\u007d', '\u003e' });
    static readonly short[][] DFA10_transition = new short[][]{
        RuntimeUtils.Convert(new char[] {'\u0001','\u0002','\u0006','\uffff','\u0001','\u0001'}),
        RuntimeUtils.Convert(new char[0]),
        RuntimeUtils.Convert(new char[] {'\u0001','\u0003'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u0006','\u0005','\uffff','\u0001','\u0006','\u0009','\uffff','\u0001','\u0006','\u000b','\uffff','\u0001','\u0006','\u0006','\uffff','\u0001','\u0006','\u0006','\uffff','\u0001','\u0006','\u0002','\uffff','\u0001','\u0006','\u0004','\uffff','\u0001','\u0006','\u0001','\uffff','\u0001','\u0006','\u0002','\uffff','\u0001','\u0006','\u0002','\uffff','\u0001','\u0006','\u0007','\uffff','\u0002','\u0006','\u0001','\uffff','\u0001','\u0004','\u0001','\u0005','\u0002','\uffff','\u0002','\u0006','\u0003','\uffff','\u0002','\u0006'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u0007'}),
        RuntimeUtils.Convert(new char[0]),
        RuntimeUtils.Convert(new char[0]),
        RuntimeUtils.Convert(new char[] {'\u0001','\u000a','\u0006','\uffff','\u0001','\u0009','\u0011','\uffff','\u0001','\u0008'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u000a','\u0006','\uffff','\u0001','\u0009','\u0011','\uffff','\u0001','\u0008'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u000b'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u0006','\u0005','\uffff','\u0001','\u0006','\u0009','\uffff','\u0001','\u0006','\u000b','\uffff','\u0001','\u0006','\u0006','\uffff','\u0001','\u0006','\u0006','\uffff','\u0001','\u0006','\u0002','\uffff','\u0001','\u0006','\u0004','\uffff','\u0001','\u0006','\u0001','\uffff','\u0001','\u0006','\u0002','\uffff','\u0001','\u0006','\u0002','\uffff','\u0001','\u0006','\u0007','\uffff','\u0002','\u0006','\u0002','\uffff','\u0001','\u0005','\u0002','\uffff','\u0002','\u0006','\u0003','\uffff','\u0002','\u0006'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u000c'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u000f','\u0017','\uffff','\u0001','\u000d','\u0001','\uffff','\u0001','\u0010','\u001c','\uffff','\u0001','\u000e'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u0011'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u0012'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u0013'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u0014'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u000a','\u0006','\uffff','\u0001','\u0009','\u0011','\uffff','\u0001','\u0008'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u000a','\u0006','\uffff','\u0001','\u0009','\u0011','\uffff','\u0001','\u0008'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u000a','\u0006','\uffff','\u0001','\u0009','\u0011','\uffff','\u0001','\u0008'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u000a','\u0006','\uffff','\u0001','\u0009','\u0011','\uffff','\u0001','\u0008'})
    };



    protected class DFA10 : antlr.runtime.DFA
    {


        public DFA10(BaseRecognizer recognizer)
        {
            this.recognizer = recognizer;
            this.decisionNumber = 10;
            this.eot = DFA10_eot;
            this.eof = DFA10_eof;
            this.min = DFA10_min;
            this.max = DFA10_max;
            this.accept = DFA10_accept;
            this.special = DFA10_special;
            this.transition = DFA10_transition;
        }
        public override string Description => "103:1: alternative returns [ATNFactory.Handle p] : ( ^( LEXER_ALT_ACTION a= alternative lexerCommands ) | ^( ALT ( elementOptions )? EPSILON ) | ^( ALT ( elementOptions )? (e= element )+ ) );";
    }

    public static readonly BitSet FOLLOW_block_in_dummy63 = new(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_BLOCK_in_ruleBlock89 = new(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_OPTIONS_in_ruleBlock105 = new(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_alternative_in_ruleBlock131 = new(new long[] { 0x0000000000000008L, 0x0000000000001020L });
    public static readonly BitSet FOLLOW_BLOCK_in_block209 = new(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_OPTIONS_in_block213 = new(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_alternative_in_block224 = new(new long[] { 0x0000000000000008L, 0x0000000000001020L });
    public static readonly BitSet FOLLOW_LEXER_ALT_ACTION_in_alternative263 = new(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_alternative_in_alternative267 = new(new long[] { 0x0000000010000000L, 0x0000000000000800L });
    public static readonly BitSet FOLLOW_lexerCommands_in_alternative269 = new(new long[] { 0x0000000000000008L });
    public static readonly BitSet FOLLOW_ALT_in_alternative289 = new(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_elementOptions_in_alternative291 = new(new long[] { 0x0000000000000000L, 0x0000000000000400L });
    public static readonly BitSet FOLLOW_EPSILON_in_alternative294 = new(new long[] { 0x0000000000000008L });
    public static readonly BitSet FOLLOW_ALT_in_alternative314 = new(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_elementOptions_in_alternative316 = new(new long[] { 0x4942408100100410L, 0x00000000000C60C0L });
    public static readonly BitSet FOLLOW_element_in_alternative322 = new(new long[] { 0x4942408100100418L, 0x00000000000C60C0L });
    public static readonly BitSet FOLLOW_lexerCommand_in_lexerCommands360 = new(new long[] { 0x0000000010000002L, 0x0000000000000800L });
    public static readonly BitSet FOLLOW_LEXER_ACTION_CALL_in_lexerCommand393 = new(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_ID_in_lexerCommand395 = new(new long[] { 0x0000000050000000L });
    public static readonly BitSet FOLLOW_lexerCommandExpr_in_lexerCommand397 = new(new long[] { 0x0000000000000008L });
    public static readonly BitSet FOLLOW_ID_in_lexerCommand413 = new(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_labeledElement_in_element454 = new(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_atom_in_element464 = new(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_subrule_in_element476 = new(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_ACTION_in_element490 = new(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_SEMPRED_in_element504 = new(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_ACTION_in_element519 = new(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_SEMPRED_in_element536 = new(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_NOT_in_element553 = new(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_blockSet_in_element557 = new(new long[] { 0x0000000000000008L });
    public static readonly BitSet FOLLOW_LEXER_CHAR_SET_in_element570 = new(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_atom_in_astOperand590 = new(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_NOT_in_astOperand603 = new(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_blockSet_in_astOperand605 = new(new long[] { 0x0000000000000008L });
    public static readonly BitSet FOLLOW_ASSIGN_in_labeledElement626 = new(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_ID_in_labeledElement628 = new(new long[] { 0x4942408100100410L, 0x00000000000C60C0L });
    public static readonly BitSet FOLLOW_element_in_labeledElement630 = new(new long[] { 0x0000000000000008L });
    public static readonly BitSet FOLLOW_PLUS_ASSIGN_in_labeledElement643 = new(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_ID_in_labeledElement645 = new(new long[] { 0x4942408100100410L, 0x00000000000C60C0L });
    public static readonly BitSet FOLLOW_element_in_labeledElement647 = new(new long[] { 0x0000000000000008L });
    public static readonly BitSet FOLLOW_OPTIONAL_in_subrule668 = new(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_block_in_subrule670 = new(new long[] { 0x0000000000000008L });
    public static readonly BitSet FOLLOW_CLOSURE_in_subrule682 = new(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_block_in_subrule684 = new(new long[] { 0x0000000000000008L });
    public static readonly BitSet FOLLOW_POSITIVE_CLOSURE_in_subrule696 = new(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_block_in_subrule698 = new(new long[] { 0x0000000000000008L });
    public static readonly BitSet FOLLOW_block_in_subrule708 = new(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_SET_in_blockSet742 = new(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_setElement_in_blockSet745 = new(new long[] { 0x4802000100000008L });
    public static readonly BitSet FOLLOW_STRING_LITERAL_in_setElement766 = new(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_TOKEN_REF_in_setElement775 = new(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_STRING_LITERAL_in_setElement783 = new(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_TOKEN_REF_in_setElement788 = new(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_RANGE_in_setElement794 = new(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_STRING_LITERAL_in_setElement798 = new(new long[] { 0x0800000000000000L });
    public static readonly BitSet FOLLOW_STRING_LITERAL_in_setElement802 = new(new long[] { 0x0000000000000008L });
    public static readonly BitSet FOLLOW_LEXER_CHAR_SET_in_setElement813 = new(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_range_in_atom828 = new(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_DOT_in_atom840 = new(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_ID_in_atom842 = new(new long[] { 0x4800000000000000L });
    public static readonly BitSet FOLLOW_terminal_in_atom844 = new(new long[] { 0x0000000000000008L });
    public static readonly BitSet FOLLOW_DOT_in_atom854 = new(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_ID_in_atom856 = new(new long[] { 0x0040000000000000L });
    public static readonly BitSet FOLLOW_ruleref_in_atom858 = new(new long[] { 0x0000000000000008L });
    public static readonly BitSet FOLLOW_WILDCARD_in_atom871 = new(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_WILDCARD_in_atom886 = new(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_blockSet_in_atom899 = new(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_terminal_in_atom914 = new(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_ruleref_in_atom929 = new(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_RULE_REF_in_ruleref957 = new(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_ARG_ACTION_in_ruleref959 = new(new long[] { 0x0000000000000000L, 0x0000000000000200L });
    public static readonly BitSet FOLLOW_ELEMENT_OPTIONS_in_ruleref963 = new(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_RULE_REF_in_ruleref980 = new(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_ARG_ACTION_in_ruleref982 = new(new long[] { 0x0000000000000008L });
    public static readonly BitSet FOLLOW_RULE_REF_in_ruleref1001 = new(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_RANGE_in_range1035 = new(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_STRING_LITERAL_in_range1039 = new(new long[] { 0x0800000000000000L });
    public static readonly BitSet FOLLOW_STRING_LITERAL_in_range1043 = new(new long[] { 0x0000000000000008L });
    public static readonly BitSet FOLLOW_STRING_LITERAL_in_terminal1069 = new(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_STRING_LITERAL_in_terminal1084 = new(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_TOKEN_REF_in_terminal1098 = new(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_ARG_ACTION_in_terminal1100 = new(new ulong[] { 0xFFFFFFFFFFFFFFF0L, 0x00000000000FFFFFL });
    public static readonly BitSet FOLLOW_TOKEN_REF_in_terminal1114 = new(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_TOKEN_REF_in_terminal1130 = new(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_ELEMENT_OPTIONS_in_elementOptions1151 = new(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_elementOption_in_elementOptions1153 = new(new long[] { 0x0000000010000408L });
    public static readonly BitSet FOLLOW_ID_in_elementOption1166 = new(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_ASSIGN_in_elementOption1172 = new(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_ID_in_elementOption1174 = new(new long[] { 0x0000000010000000L });
    public static readonly BitSet FOLLOW_ID_in_elementOption1176 = new(new long[] { 0x0000000000000008L });
    public static readonly BitSet FOLLOW_ASSIGN_in_elementOption1183 = new(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_ID_in_elementOption1185 = new(new long[] { 0x0800000000000000L });
    public static readonly BitSet FOLLOW_STRING_LITERAL_in_elementOption1187 = new(new long[] { 0x0000000000000008L });
    public static readonly BitSet FOLLOW_ASSIGN_in_elementOption1194 = new(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_ID_in_elementOption1196 = new(new long[] { 0x0000000000000010L });
    public static readonly BitSet FOLLOW_ACTION_in_elementOption1198 = new(new long[] { 0x0000000000000008L });
    public static readonly BitSet FOLLOW_ASSIGN_in_elementOption1205 = new(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_ID_in_elementOption1207 = new(new long[] { 0x0000000040000000L });
    public static readonly BitSet FOLLOW_INT_in_elementOption1209 = new(new long[] { 0x0000000000000008L });
}
