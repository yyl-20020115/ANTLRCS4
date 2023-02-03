// $ANTLR 3.5.3 org\\antlr\\v4\\parse\\ANTLRParser.g 2023-01-27 22:27:33

/*
 [The "BSD licence"]
 Copyright (c) 2005-20012 Terence Parr
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
using org.antlr.runtime;
using org.antlr.v4.runtime;
using org.antlr.v4.runtime.misc;
using org.antlr.v4.runtime.tree;
using org.antlr.v4.tool;
using org.antlr.v4.tool.ast;
using org.antlr.runtime.tree;
using org.antlr.v4.runtime.atn;

namespace org.antlr.v4.parse;


/** The definitive ANTLR v3 grammar to parse ANTLR v4 grammars.
 *  The grammar builds ASTs that are sniffed by subsequent stages.
 */
public class ANTLRParser : antlr.runtime.Parser
{
    public static readonly String[] tokenNames = new String[] {
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
    public antlr.runtime.Parser[] getDelegates()
    {
        return new antlr.runtime.Parser[] { };
    }

    // delegators


    public ANTLRParser(TokenStream input)
        : this(input, new RecognizerSharedState())
    {
    }
    public ANTLRParser(TokenStream input, RecognizerSharedState state)
        : base(input, state)
    {
    }

    protected TreeAdaptor adaptor = new CommonTreeAdaptor();

    public void setTreeAdaptor(TreeAdaptor adaptor)
    {
        this.adaptor = adaptor;
    }
    public TreeAdaptor getTreeAdaptor()
    {
        return adaptor;
    }
    ////@Override 
    public virtual String[] getTokenNames() { return ANTLRParser.tokenNames; }
    ////@Override
    public override String GetGrammarFileName() { return "org\\antlr\\v4\\parse\\ANTLRParser.g"; }


    public Deque<String> paraphrases = new();
    public void grammarError(ErrorType etype, Token token, params Object[] args) { }


    public class grammarSpec_return : ParserRuleReturnScope
    {
        public GrammarAST tree;
        //@Override

        public GrammarAST getTree() { return tree; }
    };


    // $ANTLR start "grammarSpec"
    // org\\antlr\\v4\\parse\\ANTLRParser.g:130:1: grammarSpec : grammarType id SEMI sync ( prequelConstruct sync )* rules ( modeSpec )* EOF -> ^( grammarType id ( prequelConstruct )* rules ( modeSpec )* ) ;
    public ANTLRParser.grammarSpec_return grammarSpec()  {
        ANTLRParser.grammarSpec_return retval = new ANTLRParser.grammarSpec_return();
        retval.start = input.LT(1);

        GrammarAST root_0 = null;

        Token SEMI3 = null;
        Token EOF9 = null;
        ParserRuleReturnScope grammarType1 = null;
        ParserRuleReturnScope id2 = null;
        ParserRuleReturnScope sync4 = null;
        ParserRuleReturnScope prequelConstruct5 = null;
        ParserRuleReturnScope sync6 = null;
        ParserRuleReturnScope rules7 = null;
        ParserRuleReturnScope modeSpec8 = null;

        GrammarAST SEMI3_tree = null;
        GrammarAST EOF9_tree = null;
        RewriteRuleTokenStream stream_SEMI = new RewriteRuleTokenStream(adaptor, "token SEMI");
        RewriteRuleTokenStream stream_EOF = new RewriteRuleTokenStream(adaptor, "token EOF");
        RewriteRuleSubtreeStream stream_modeSpec = new RewriteRuleSubtreeStream(adaptor, "rule modeSpec");
        RewriteRuleSubtreeStream stream_grammarType = new RewriteRuleSubtreeStream(adaptor, "rule grammarType");
        RewriteRuleSubtreeStream stream_rules = new RewriteRuleSubtreeStream(adaptor, "rule rules");
        RewriteRuleSubtreeStream stream_id = new RewriteRuleSubtreeStream(adaptor, "rule id");
        RewriteRuleSubtreeStream stream_prequelConstruct = new RewriteRuleSubtreeStream(adaptor, "rule prequelConstruct");
        RewriteRuleSubtreeStream stream_sync = new RewriteRuleSubtreeStream(adaptor, "rule sync");

        try
        {
            // org\\antlr\\v4\\parse\\ANTLRParser.g:137:5: ( grammarType id SEMI sync ( prequelConstruct sync )* rules ( modeSpec )* EOF -> ^( grammarType id ( prequelConstruct )* rules ( modeSpec )* ) )
            // org\\antlr\\v4\\parse\\ANTLRParser.g:140:7: grammarType id SEMI sync ( prequelConstruct sync )* rules ( modeSpec )* EOF
            {
                pushFollow(FOLLOW_grammarType_in_grammarSpec281);
                grammarType1 = grammarType();
                state._fsp--;

                stream_grammarType.add(grammarType1.getTree());
                pushFollow(FOLLOW_id_in_grammarSpec283);
                id2 = id();
                state._fsp--;

                stream_id.add(id2.getTree());
                SEMI3 = (Token)Match(input, SEMI, FOLLOW_SEMI_in_grammarSpec285);
                stream_SEMI.add(SEMI3);

                pushFollow(FOLLOW_sync_in_grammarSpec323);
                sync4 = sync();
                state._fsp--;

                stream_sync.add(sync4.getTree());
            // org\\antlr\\v4\\parse\\ANTLRParser.g:153:12: ( prequelConstruct sync )*
            loop1:
                while (true)
                {
                    int alt1 = 2;
                    int LA1_0 = input.LA(1);
                    if ((LA1_0 == AT || LA1_0 == CHANNELS || LA1_0 == IMPORT || LA1_0 == OPTIONS || LA1_0 == TOKENS_SPEC))
                    {
                        alt1 = 1;
                    }

                    switch (alt1)
                    {
                        case 1:
                            // org\\antlr\\v4\\parse\\ANTLRParser.g:153:14: prequelConstruct sync
                            {
                                pushFollow(FOLLOW_prequelConstruct_in_grammarSpec327);
                                prequelConstruct5 = prequelConstruct();
                                state._fsp--;

                                stream_prequelConstruct.add(prequelConstruct5.getTree());
                                pushFollow(FOLLOW_sync_in_grammarSpec329);
                                sync6 = sync();
                                state._fsp--;

                                stream_sync.add(sync6.getTree());
                            }
                            break;

                        default:
                            goto exit1;
                            //break loop1;
                    }
                }
            exit1:
                pushFollow(FOLLOW_rules_in_grammarSpec354);
                rules7 = rules();
                state._fsp--;

                stream_rules.add(rules7.getTree());
            // org\\antlr\\v4\\parse\\ANTLRParser.g:161:4: ( modeSpec )*
            loop2:
                while (true)
                {
                    int alt2 = 2;
                    int LA2_0 = input.LA(1);
                    if ((LA2_0 == MODE))
                    {
                        alt2 = 1;
                    }

                    switch (alt2)
                    {
                        case 1:
                            // org\\antlr\\v4\\parse\\ANTLRParser.g:161:4: modeSpec
                            {
                                pushFollow(FOLLOW_modeSpec_in_grammarSpec360);
                                modeSpec8 = modeSpec();
                                state._fsp--;

                                stream_modeSpec.add(modeSpec8.getTree());
                            }
                            break;

                        default:
                            goto exit2;
                            //break loop2;
                    }
                }
            exit2:
                EOF9 = (Token)Match(input, EOF, FOLLOW_EOF_in_grammarSpec398);
                stream_EOF.add(EOF9);


                // AST REWRITE
                // elements: grammarType, modeSpec, rules, id, prequelConstruct
                // token labels: 
                // rule labels: retval
                // token list labels: 
                // rule list labels: 
                // wildcard labels: 
                retval.tree = root_0;
                RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval != null ? retval.getTree() : null);

                root_0 = (GrammarAST)adaptor.nil();
                // 174:7: -> ^( grammarType id ( prequelConstruct )* rules ( modeSpec )* )
                {
                    // org\\antlr\\v4\\parse\\ANTLRParser.g:174:10: ^( grammarType id ( prequelConstruct )* rules ( modeSpec )* )
                    {
                        GrammarAST root_1 = (GrammarAST)adaptor.nil();
                        root_1 = (GrammarAST)adaptor.becomeRoot(stream_grammarType.nextNode(), root_1);
                        adaptor.addChild(root_1, stream_id.nextTree());
                        // org\\antlr\\v4\\parse\\ANTLRParser.g:176:14: ( prequelConstruct )*
                        while (stream_prequelConstruct.hasNext())
                        {
                            adaptor.addChild(root_1, stream_prequelConstruct.nextTree());
                        }
                        stream_prequelConstruct.reset();

                        adaptor.addChild(root_1, stream_rules.nextTree());
                        // org\\antlr\\v4\\parse\\ANTLRParser.g:178:14: ( modeSpec )*
                        while (stream_modeSpec.hasNext())
                        {
                            adaptor.addChild(root_1, stream_modeSpec.nextTree());
                        }
                        stream_modeSpec.reset();

                        adaptor.addChild(root_0, root_1);
                    }

                }


                retval.tree = root_0;

            }

            retval.stop = input.LT(-1);

            retval.tree = (GrammarAST)adaptor.rulePostProcessing(root_0);
            adaptor.setTokenBoundaries(retval.tree, retval.start, retval.stop);


            GrammarAST options = (GrammarAST)retval.tree.getFirstChildWithType(ANTLRParser.OPTIONS);
            if (options != null)
            {
                Grammar.setNodeOptions(retval.tree, options);
            }

        }
        catch (RecognitionException re)
        {
            reportError(re);
            recover(input, re);
            retval.tree = (GrammarAST)adaptor.errorNode(input, retval.start, input.LT(-1), re);
        }
        finally
        {
            // do for sure before leaving
        }
        return retval;
    }
    // $ANTLR end "grammarSpec"


    public class grammarType_return : ParserRuleReturnScope
    {
        public GrammarAST tree;
        //@Override

        public GrammarAST getTree() { return tree; }
    };


    // $ANTLR start "grammarType"
    // org\\antlr\\v4\\parse\\ANTLRParser.g:182:1: grammarType : (t= LEXER g= GRAMMAR -> GRAMMAR[$g, \"LEXER_GRAMMAR\", getTokenStream()] |t= PARSER g= GRAMMAR -> GRAMMAR[$g, \"PARSER_GRAMMAR\", getTokenStream()] |g= GRAMMAR -> GRAMMAR[$g, \"COMBINED_GRAMMAR\", getTokenStream()] ) ;
    public ANTLRParser.grammarType_return grammarType()  {
        ANTLRParser.grammarType_return retval = new ANTLRParser.grammarType_return();
        retval.start = input.LT(1);

        GrammarAST root_0 = null;

        Token t = null;
        Token g = null;

        GrammarAST t_tree = null;
        GrammarAST g_tree = null;
        RewriteRuleTokenStream stream_PARSER = new RewriteRuleTokenStream(adaptor, "token PARSER");
        RewriteRuleTokenStream stream_LEXER = new RewriteRuleTokenStream(adaptor, "token LEXER");
        RewriteRuleTokenStream stream_GRAMMAR = new RewriteRuleTokenStream(adaptor, "token GRAMMAR");

        try
        {
            // org\\antlr\\v4\\parse\\ANTLRParser.g:187:5: ( (t= LEXER g= GRAMMAR -> GRAMMAR[$g, \"LEXER_GRAMMAR\", getTokenStream()] |t= PARSER g= GRAMMAR -> GRAMMAR[$g, \"PARSER_GRAMMAR\", getTokenStream()] |g= GRAMMAR -> GRAMMAR[$g, \"COMBINED_GRAMMAR\", getTokenStream()] ) )
            // org\\antlr\\v4\\parse\\ANTLRParser.g:187:7: (t= LEXER g= GRAMMAR -> GRAMMAR[$g, \"LEXER_GRAMMAR\", getTokenStream()] |t= PARSER g= GRAMMAR -> GRAMMAR[$g, \"PARSER_GRAMMAR\", getTokenStream()] |g= GRAMMAR -> GRAMMAR[$g, \"COMBINED_GRAMMAR\", getTokenStream()] )
            {
                // org\\antlr\\v4\\parse\\ANTLRParser.g:187:7: (t= LEXER g= GRAMMAR -> GRAMMAR[$g, \"LEXER_GRAMMAR\", getTokenStream()] |t= PARSER g= GRAMMAR -> GRAMMAR[$g, \"PARSER_GRAMMAR\", getTokenStream()] |g= GRAMMAR -> GRAMMAR[$g, \"COMBINED_GRAMMAR\", getTokenStream()] )
                int alt3 = 3;
                switch (input.LA(1))
                {
                    case LEXER:
                        {
                            alt3 = 1;
                        }
                        break;
                    case PARSER:
                        {
                            alt3 = 2;
                        }
                        break;
                    case GRAMMAR:
                        {
                            alt3 = 3;
                        }
                        break;
                    default:
                        NoViableAltException nvae =
                            new NoViableAltException("", 3, 0, input);
                        throw nvae;
                }
                switch (alt3)
                {
                    case 1:
                        // org\\antlr\\v4\\parse\\ANTLRParser.g:187:9: t= LEXER g= GRAMMAR
                        {
                            t = (Token)Match(input, LEXER, FOLLOW_LEXER_in_grammarType568);
                            stream_LEXER.add(t);

                            g = (Token)Match(input, GRAMMAR, FOLLOW_GRAMMAR_in_grammarType572);
                            stream_GRAMMAR.add(g);


                            // AST REWRITE
                            // elements: GRAMMAR
                            // token labels: 
                            // rule labels: retval
                            // token list labels: 
                            // rule list labels: 
                            // wildcard labels: 
                            retval.tree = root_0;
                            RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval != null ? retval.getTree() : null);

                            root_0 = (GrammarAST)adaptor.nil();
                            // 187:28: -> GRAMMAR[$g, \"LEXER_GRAMMAR\", getTokenStream()]
                            {
                                adaptor.addChild(root_0, new GrammarRootAST(GRAMMAR, g, "LEXER_GRAMMAR", getTokenStream()));
                            }


                            retval.tree = root_0;

                        }
                        break;
                    case 2:
                        // org\\antlr\\v4\\parse\\ANTLRParser.g:189:6: t= PARSER g= GRAMMAR
                        {
                            t = (Token)Match(input, PARSER, FOLLOW_PARSER_in_grammarType595);
                            stream_PARSER.add(t);

                            g = (Token)Match(input, GRAMMAR, FOLLOW_GRAMMAR_in_grammarType599);
                            stream_GRAMMAR.add(g);


                            // AST REWRITE
                            // elements: GRAMMAR
                            // token labels: 
                            // rule labels: retval
                            // token list labels: 
                            // rule list labels: 
                            // wildcard labels: 
                            retval.tree = root_0;
                            RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval != null ? retval.getTree() : null);

                            root_0 = (GrammarAST)adaptor.nil();
                            // 189:25: -> GRAMMAR[$g, \"PARSER_GRAMMAR\", getTokenStream()]
                            {
                                adaptor.addChild(root_0, new GrammarRootAST(GRAMMAR, g, "PARSER_GRAMMAR", getTokenStream()));
                            }


                            retval.tree = root_0;

                        }
                        break;
                    case 3:
                        // org\\antlr\\v4\\parse\\ANTLRParser.g:192:6: g= GRAMMAR
                        {
                            g = (Token)Match(input, GRAMMAR, FOLLOW_GRAMMAR_in_grammarType620);
                            stream_GRAMMAR.add(g);


                            // AST REWRITE
                            // elements: GRAMMAR
                            // token labels: 
                            // rule labels: retval
                            // token list labels: 
                            // rule list labels: 
                            // wildcard labels: 
                            retval.tree = root_0;
                            RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval != null ? retval.getTree() : null);

                            root_0 = (GrammarAST)adaptor.nil();
                            // 192:25: -> GRAMMAR[$g, \"COMBINED_GRAMMAR\", getTokenStream()]
                            {
                                adaptor.addChild(root_0, new GrammarRootAST(GRAMMAR, g, "COMBINED_GRAMMAR", getTokenStream()));
                            }


                            retval.tree = root_0;

                        }
                        break;

                }

            }

            retval.stop = input.LT(-1);

            retval.tree = (GrammarAST)adaptor.rulePostProcessing(root_0);
            adaptor.setTokenBoundaries(retval.tree, retval.start, retval.stop);


            if (t != null) ((GrammarRootAST)retval.tree).grammarType = (t != null ? t.getType() : 0);
            else ((GrammarRootAST)retval.tree).grammarType = COMBINED;

        }
        catch (RecognitionException re)
        {
            reportError(re);
            recover(input, re);
            retval.tree = (GrammarAST)adaptor.errorNode(input, retval.start, input.LT(-1), re);
        }
        finally
        {
            // do for sure before leaving
        }
        return retval;
    }
    // $ANTLR end "grammarType"


    public class prequelConstruct_return : ParserRuleReturnScope
    {
        public GrammarAST tree;
        //@Override

        public GrammarAST getTree() { return tree; }
    };


    // $ANTLR start "prequelConstruct"
    // org\\antlr\\v4\\parse\\ANTLRParser.g:199:1: prequelConstruct : ( optionsSpec | delegateGrammars | tokensSpec | channelsSpec | action );
    public ANTLRParser.prequelConstruct_return prequelConstruct()  {
        ANTLRParser.prequelConstruct_return retval = new ANTLRParser.prequelConstruct_return();
        retval.start = input.LT(1);

        GrammarAST root_0 = null;

        ParserRuleReturnScope optionsSpec10 = null;
        ParserRuleReturnScope delegateGrammars11 = null;
        ParserRuleReturnScope tokensSpec12 = null;
        ParserRuleReturnScope channelsSpec13 = null;
        ParserRuleReturnScope action14 = null;


        try
        {
            // org\\antlr\\v4\\parse\\ANTLRParser.g:200:2: ( optionsSpec | delegateGrammars | tokensSpec | channelsSpec | action )
            int alt4 = 5;
            switch (input.LA(1))
            {
                case OPTIONS:
                    {
                        alt4 = 1;
                    }
                    break;
                case IMPORT:
                    {
                        alt4 = 2;
                    }
                    break;
                case TOKENS_SPEC:
                    {
                        alt4 = 3;
                    }
                    break;
                case CHANNELS:
                    {
                        alt4 = 4;
                    }
                    break;
                case AT:
                    {
                        alt4 = 5;
                    }
                    break;
                default:
                    NoViableAltException nvae =
                        new NoViableAltException("", 4, 0, input);
                    throw nvae;
            }
            switch (alt4)
            {
                case 1:
                    // org\\antlr\\v4\\parse\\ANTLRParser.g:201:4: optionsSpec
                    {
                        root_0 = (GrammarAST)adaptor.nil();


                        pushFollow(FOLLOW_optionsSpec_in_prequelConstruct662);
                        optionsSpec10 = optionsSpec();
                        state._fsp--;

                        adaptor.addChild(root_0, optionsSpec10.getTree());

                    }
                    break;
                case 2:
                    // org\\antlr\\v4\\parse\\ANTLRParser.g:205:7: delegateGrammars
                    {
                        root_0 = (GrammarAST)adaptor.nil();


                        pushFollow(FOLLOW_delegateGrammars_in_prequelConstruct685);
                        delegateGrammars11 = delegateGrammars();
                        state._fsp--;

                        adaptor.addChild(root_0, delegateGrammars11.getTree());

                    }
                    break;
                case 3:
                    // org\\antlr\\v4\\parse\\ANTLRParser.g:212:7: tokensSpec
                    {
                        root_0 = (GrammarAST)adaptor.nil();


                        pushFollow(FOLLOW_tokensSpec_in_prequelConstruct729);
                        tokensSpec12 = tokensSpec();
                        state._fsp--;

                        adaptor.addChild(root_0, tokensSpec12.getTree());

                    }
                    break;
                case 4:
                    // org\\antlr\\v4\\parse\\ANTLRParser.g:215:4: channelsSpec
                    {
                        root_0 = (GrammarAST)adaptor.nil();


                        pushFollow(FOLLOW_channelsSpec_in_prequelConstruct739);
                        channelsSpec13 = channelsSpec();
                        state._fsp--;

                        adaptor.addChild(root_0, channelsSpec13.getTree());

                    }
                    break;
                case 5:
                    // org\\antlr\\v4\\parse\\ANTLRParser.g:221:7: action
                    {
                        root_0 = (GrammarAST)adaptor.nil();


                        pushFollow(FOLLOW_action_in_prequelConstruct776);
                        action14 = action();
                        state._fsp--;

                        adaptor.addChild(root_0, action14.getTree());

                    }
                    break;

            }
            retval.stop = input.LT(-1);

            retval.tree = (GrammarAST)adaptor.rulePostProcessing(root_0);
            adaptor.setTokenBoundaries(retval.tree, retval.start, retval.stop);

        }
        catch (RecognitionException re)
        {
            reportError(re);
            recover(input, re);
            retval.tree = (GrammarAST)adaptor.errorNode(input, retval.start, input.LT(-1), re);
        }
        finally
        {
            // do for sure before leaving
        }
        return retval;
    }
    // $ANTLR end "prequelConstruct"


    public class optionsSpec_return : ParserRuleReturnScope
    {
        public GrammarAST tree;
        //@Override

        public GrammarAST getTree() { return tree; }
    };


    // $ANTLR start "optionsSpec"
    // org\\antlr\\v4\\parse\\ANTLRParser.g:225:1: optionsSpec : OPTIONS ( option SEMI )* RBRACE -> ^( OPTIONS[$OPTIONS, \"OPTIONS\"] ( option )* ) ;
    public ANTLRParser.optionsSpec_return optionsSpec()  {
        ANTLRParser.optionsSpec_return retval = new ANTLRParser.optionsSpec_return();
        retval.start = input.LT(1);

        GrammarAST root_0 = null;

        Token OPTIONS15 = null;
        Token SEMI17 = null;
        Token RBRACE18 = null;
        ParserRuleReturnScope option16 = null;

        GrammarAST OPTIONS15_tree = null;
        GrammarAST SEMI17_tree = null;
        GrammarAST RBRACE18_tree = null;
        RewriteRuleTokenStream stream_RBRACE = new RewriteRuleTokenStream(adaptor, "token RBRACE");
        RewriteRuleTokenStream stream_SEMI = new RewriteRuleTokenStream(adaptor, "token SEMI");
        RewriteRuleTokenStream stream_OPTIONS = new RewriteRuleTokenStream(adaptor, "token OPTIONS");
        RewriteRuleSubtreeStream stream_option = new RewriteRuleSubtreeStream(adaptor, "rule option");

        try
        {
            // org\\antlr\\v4\\parse\\ANTLRParser.g:226:2: ( OPTIONS ( option SEMI )* RBRACE -> ^( OPTIONS[$OPTIONS, \"OPTIONS\"] ( option )* ) )
            // org\\antlr\\v4\\parse\\ANTLRParser.g:226:4: OPTIONS ( option SEMI )* RBRACE
            {
                OPTIONS15 = (Token)Match(input, OPTIONS, FOLLOW_OPTIONS_in_optionsSpec791);
                stream_OPTIONS.add(OPTIONS15);

            // org\\antlr\\v4\\parse\\ANTLRParser.g:226:12: ( option SEMI )*
            loop5:
                while (true)
                {
                    int alt5 = 2;
                    int LA5_0 = input.LA(1);
                    if ((LA5_0 == RULE_REF || LA5_0 == TOKEN_REF))
                    {
                        alt5 = 1;
                    }

                    switch (alt5)
                    {
                        case 1:
                            // org\\antlr\\v4\\parse\\ANTLRParser.g:226:13: option SEMI
                            {
                                pushFollow(FOLLOW_option_in_optionsSpec794);
                                option16 = option();
                                state._fsp--;

                                stream_option.add(option16.getTree());
                                SEMI17 = (Token)Match(input, SEMI, FOLLOW_SEMI_in_optionsSpec796);
                                stream_SEMI.add(SEMI17);

                            }
                            break;

                        default:
                            goto exit5;
                            //break loop5;
                    }
                }
            exit5:
                RBRACE18 = (Token)Match(input, RBRACE, FOLLOW_RBRACE_in_optionsSpec800);
                stream_RBRACE.add(RBRACE18);


                // AST REWRITE
                // elements: OPTIONS, option
                // token labels: 
                // rule labels: retval
                // token list labels: 
                // rule list labels: 
                // wildcard labels: 
                retval.tree = root_0;
                RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval != null ? retval.getTree() : null);

                root_0 = (GrammarAST)adaptor.nil();
                // 226:34: -> ^( OPTIONS[$OPTIONS, \"OPTIONS\"] ( option )* )
                {
                    // org\\antlr\\v4\\parse\\ANTLRParser.g:226:37: ^( OPTIONS[$OPTIONS, \"OPTIONS\"] ( option )* )
                    {
                        GrammarAST root_1 = (GrammarAST)adaptor.nil();
                        root_1 = (GrammarAST)adaptor.becomeRoot((GrammarAST)adaptor.create(OPTIONS, OPTIONS15, "OPTIONS"), root_1);
                        // org\\antlr\\v4\\parse\\ANTLRParser.g:226:68: ( option )*
                        while (stream_option.hasNext())
                        {
                            adaptor.addChild(root_1, stream_option.nextTree());
                        }
                        stream_option.reset();

                        adaptor.addChild(root_0, root_1);
                    }

                }


                retval.tree = root_0;

            }

            retval.stop = input.LT(-1);

            retval.tree = (GrammarAST)adaptor.rulePostProcessing(root_0);
            adaptor.setTokenBoundaries(retval.tree, retval.start, retval.stop);

        }
        catch (RecognitionException re)
        {
            reportError(re);
            recover(input, re);
            retval.tree = (GrammarAST)adaptor.errorNode(input, retval.start, input.LT(-1), re);
        }
        finally
        {
            // do for sure before leaving
        }
        return retval;
    }
    // $ANTLR end "optionsSpec"


    public class option_return : ParserRuleReturnScope
    {
        public GrammarAST tree;
        //@Override

        public GrammarAST getTree() { return tree; }
    };


    // $ANTLR start "option"
    // org\\antlr\\v4\\parse\\ANTLRParser.g:229:1: option : id ASSIGN ^ optionValue ;
    public ANTLRParser.option_return option()  {
        ANTLRParser.option_return retval = new ANTLRParser.option_return();
        retval.start = input.LT(1);

        GrammarAST root_0 = null;

        Token ASSIGN20 = null;
        ParserRuleReturnScope id19 = null;
        ParserRuleReturnScope optionValue21 = null;

        GrammarAST ASSIGN20_tree = null;

        try
        {
            // org\\antlr\\v4\\parse\\ANTLRParser.g:230:5: ( id ASSIGN ^ optionValue )
            // org\\antlr\\v4\\parse\\ANTLRParser.g:230:9: id ASSIGN ^ optionValue
            {
                root_0 = (GrammarAST)adaptor.nil();


                pushFollow(FOLLOW_id_in_option829);
                id19 = id();
                state._fsp--;

                adaptor.addChild(root_0, id19.getTree());

                ASSIGN20 = (Token)Match(input, ASSIGN, FOLLOW_ASSIGN_in_option831);
                ASSIGN20_tree = (GrammarAST)adaptor.create(ASSIGN20);
                root_0 = (GrammarAST)adaptor.becomeRoot(ASSIGN20_tree, root_0);

                pushFollow(FOLLOW_optionValue_in_option834);
                optionValue21 = optionValue();
                state._fsp--;

                adaptor.addChild(root_0, optionValue21.getTree());

            }

            retval.stop = input.LT(-1);

            retval.tree = (GrammarAST)adaptor.rulePostProcessing(root_0);
            adaptor.setTokenBoundaries(retval.tree, retval.start, retval.stop);

        }
        catch (RecognitionException re)
        {
            reportError(re);
            recover(input, re);
            retval.tree = (GrammarAST)adaptor.errorNode(input, retval.start, input.LT(-1), re);
        }
        finally
        {
            // do for sure before leaving
        }
        return retval;
    }
    // $ANTLR end "option"


    public class optionValue_return : ParserRuleReturnScope
    {
        public GrammarAST tree;
        //@Override

        public GrammarAST getTree() { return tree; }
    };


    // $ANTLR start "optionValue"
    // org\\antlr\\v4\\parse\\ANTLRParser.g:238:1: optionValue : ( qid | STRING_LITERAL | ACTION | INT );
    public ANTLRParser.optionValue_return optionValue()  {
        ANTLRParser.optionValue_return retval = new ANTLRParser.optionValue_return();
        retval.start = input.LT(1);

        GrammarAST root_0 = null;

        Token STRING_LITERAL23 = null;
        Token ACTION24 = null;
        Token INT25 = null;
        ParserRuleReturnScope qid22 = null;

        GrammarAST STRING_LITERAL23_tree = null;
        GrammarAST ACTION24_tree = null;
        GrammarAST INT25_tree = null;

        try
        {
            // org\\antlr\\v4\\parse\\ANTLRParser.g:239:5: ( qid | STRING_LITERAL | ACTION | INT )
            int alt6 = 4;
            switch (input.LA(1))
            {
                case RULE_REF:
                case TOKEN_REF:
                    {
                        alt6 = 1;
                    }
                    break;
                case STRING_LITERAL:
                    {
                        alt6 = 2;
                    }
                    break;
                case ACTION:
                    {
                        alt6 = 3;
                    }
                    break;
                case INT:
                    {
                        alt6 = 4;
                    }
                    break;
                default:
                    NoViableAltException nvae =
                        new NoViableAltException("", 6, 0, input);
                    throw nvae;
            }
            switch (alt6)
            {
                case 1:
                    // org\\antlr\\v4\\parse\\ANTLRParser.g:242:7: qid
                    {
                        root_0 = (GrammarAST)adaptor.nil();


                        pushFollow(FOLLOW_qid_in_optionValue877);
                        qid22 = qid();
                        state._fsp--;

                        adaptor.addChild(root_0, qid22.getTree());

                    }
                    break;
                case 2:
                    // org\\antlr\\v4\\parse\\ANTLRParser.g:243:7: STRING_LITERAL
                    {
                        root_0 = (GrammarAST)adaptor.nil();


                        STRING_LITERAL23 = (Token)Match(input, STRING_LITERAL, FOLLOW_STRING_LITERAL_in_optionValue885);
                        STRING_LITERAL23_tree = (GrammarAST)adaptor.create(STRING_LITERAL23);
                        adaptor.addChild(root_0, STRING_LITERAL23_tree);

                    }
                    break;
                case 3:
                    // org\\antlr\\v4\\parse\\ANTLRParser.g:244:4: ACTION
                    {
                        root_0 = (GrammarAST)adaptor.nil();


                        ACTION24 = (Token)Match(input, ACTION, FOLLOW_ACTION_in_optionValue890);
                        ACTION24_tree = new ActionAST(ACTION24);
                        adaptor.addChild(root_0, ACTION24_tree);

                    }
                    break;
                case 4:
                    // org\\antlr\\v4\\parse\\ANTLRParser.g:245:7: INT
                    {
                        root_0 = (GrammarAST)adaptor.nil();


                        INT25 = (Token)Match(input, INT, FOLLOW_INT_in_optionValue901);
                        INT25_tree = (GrammarAST)adaptor.create(INT25);
                        adaptor.addChild(root_0, INT25_tree);

                    }
                    break;

            }
            retval.stop = input.LT(-1);

            retval.tree = (GrammarAST)adaptor.rulePostProcessing(root_0);
            adaptor.setTokenBoundaries(retval.tree, retval.start, retval.stop);

        }
        catch (RecognitionException re)
        {
            reportError(re);
            recover(input, re);
            retval.tree = (GrammarAST)adaptor.errorNode(input, retval.start, input.LT(-1), re);
        }
        finally
        {
            // do for sure before leaving
        }
        return retval;
    }
    // $ANTLR end "optionValue"


    public class delegateGrammars_return : ParserRuleReturnScope
    {
        public GrammarAST tree;
        //@Override

        public GrammarAST getTree() { return tree; }
    };


    // $ANTLR start "delegateGrammars"
    // org\\antlr\\v4\\parse\\ANTLRParser.g:250:1: delegateGrammars : IMPORT delegateGrammar ( COMMA delegateGrammar )* SEMI -> ^( IMPORT ( delegateGrammar )+ ) ;
    public ANTLRParser.delegateGrammars_return delegateGrammars()  {
        ANTLRParser.delegateGrammars_return retval = new ANTLRParser.delegateGrammars_return();
        retval.start = input.LT(1);

        GrammarAST root_0 = null;

        Token IMPORT26 = null;
        Token COMMA28 = null;
        Token SEMI30 = null;
        ParserRuleReturnScope delegateGrammar27 = null;
        ParserRuleReturnScope delegateGrammar29 = null;

        GrammarAST IMPORT26_tree = null;
        GrammarAST COMMA28_tree = null;
        GrammarAST SEMI30_tree = null;
        RewriteRuleTokenStream stream_COMMA = new RewriteRuleTokenStream(adaptor, "token COMMA");
        RewriteRuleTokenStream stream_IMPORT = new RewriteRuleTokenStream(adaptor, "token IMPORT");
        RewriteRuleTokenStream stream_SEMI = new RewriteRuleTokenStream(adaptor, "token SEMI");
        RewriteRuleSubtreeStream stream_delegateGrammar = new RewriteRuleSubtreeStream(adaptor, "rule delegateGrammar");

        try
        {
            // org\\antlr\\v4\\parse\\ANTLRParser.g:251:2: ( IMPORT delegateGrammar ( COMMA delegateGrammar )* SEMI -> ^( IMPORT ( delegateGrammar )+ ) )
            // org\\antlr\\v4\\parse\\ANTLRParser.g:251:4: IMPORT delegateGrammar ( COMMA delegateGrammar )* SEMI
            {
                IMPORT26 = (Token)Match(input, IMPORT, FOLLOW_IMPORT_in_delegateGrammars917);
                stream_IMPORT.add(IMPORT26);

                pushFollow(FOLLOW_delegateGrammar_in_delegateGrammars919);
                delegateGrammar27 = delegateGrammar();
                state._fsp--;

                stream_delegateGrammar.add(delegateGrammar27.getTree());
            // org\\antlr\\v4\\parse\\ANTLRParser.g:251:27: ( COMMA delegateGrammar )*
            loop7:
                while (true)
                {
                    int alt7 = 2;
                    int LA7_0 = input.LA(1);
                    if ((LA7_0 == COMMA))
                    {
                        alt7 = 1;
                    }

                    switch (alt7)
                    {
                        case 1:
                            // org\\antlr\\v4\\parse\\ANTLRParser.g:251:28: COMMA delegateGrammar
                            {
                                COMMA28 = (Token)Match(input, COMMA, FOLLOW_COMMA_in_delegateGrammars922);
                                stream_COMMA.add(COMMA28);

                                pushFollow(FOLLOW_delegateGrammar_in_delegateGrammars924);
                                delegateGrammar29 = delegateGrammar();
                                state._fsp--;

                                stream_delegateGrammar.add(delegateGrammar29.getTree());
                            }
                            break;

                        default:
                            goto exit7;
                            //break loop7;
                    }
                }
            exit7:
                SEMI30 = (Token)Match(input, SEMI, FOLLOW_SEMI_in_delegateGrammars928);
                stream_SEMI.add(SEMI30);


                // AST REWRITE
                // elements: delegateGrammar, IMPORT
                // token labels: 
                // rule labels: retval
                // token list labels: 
                // rule list labels: 
                // wildcard labels: 
                retval.tree = root_0;
                RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval != null ? retval.getTree() : null);

                root_0 = (GrammarAST)adaptor.nil();
                // 251:57: -> ^( IMPORT ( delegateGrammar )+ )
                {
                    // org\\antlr\\v4\\parse\\ANTLRParser.g:251:60: ^( IMPORT ( delegateGrammar )+ )
                    {
                        GrammarAST root_1 = (GrammarAST)adaptor.nil();
                        root_1 = (GrammarAST)adaptor.becomeRoot(stream_IMPORT.nextNode(), root_1);
                        if (!(stream_delegateGrammar.hasNext()))
                        {
                            throw new RewriteEarlyExitException();
                        }
                        while (stream_delegateGrammar.hasNext())
                        {
                            adaptor.addChild(root_1, stream_delegateGrammar.nextTree());
                        }
                        stream_delegateGrammar.reset();

                        adaptor.addChild(root_0, root_1);
                    }

                }


                retval.tree = root_0;

            }

            retval.stop = input.LT(-1);

            retval.tree = (GrammarAST)adaptor.rulePostProcessing(root_0);
            adaptor.setTokenBoundaries(retval.tree, retval.start, retval.stop);

        }
        catch (RecognitionException re)
        {
            reportError(re);
            recover(input, re);
            retval.tree = (GrammarAST)adaptor.errorNode(input, retval.start, input.LT(-1), re);
        }
        finally
        {
            // do for sure before leaving
        }
        return retval;
    }
    // $ANTLR end "delegateGrammars"


    public class delegateGrammar_return : ParserRuleReturnScope
    {
        public GrammarAST tree;
        //@Override

        public GrammarAST getTree() { return tree; }
    };


    // $ANTLR start "delegateGrammar"
    // org\\antlr\\v4\\parse\\ANTLRParser.g:256:1: delegateGrammar : ( id ASSIGN ^ id | id );
    public ANTLRParser.delegateGrammar_return delegateGrammar()  {
        ANTLRParser.delegateGrammar_return retval = new ANTLRParser.delegateGrammar_return();
        retval.start = input.LT(1);

        GrammarAST root_0 = null;

        Token ASSIGN32 = null;
        ParserRuleReturnScope id31 = null;
        ParserRuleReturnScope id33 = null;
        ParserRuleReturnScope id34 = null;

        GrammarAST ASSIGN32_tree = null;

        try
        {
            // org\\antlr\\v4\\parse\\ANTLRParser.g:257:5: ( id ASSIGN ^ id | id )
            int alt8 = 2;
            int LA8_0 = input.LA(1);
            if ((LA8_0 == RULE_REF))
            {
                int LA8_1 = input.LA(2);
                if ((LA8_1 == ASSIGN))
                {
                    alt8 = 1;
                }
                else if ((LA8_1 == COMMA || LA8_1 == SEMI))
                {
                    alt8 = 2;
                }

                else
                {
                    int nvaeMark = input.mark();
                    try
                    {
                        input.consume();
                        NoViableAltException nvae =
                            new NoViableAltException("", 8, 1, input);
                        throw nvae;
                    }
                    finally
                    {
                        input.rewind(nvaeMark);
                    }
                }

            }
            else if ((LA8_0 == TOKEN_REF))
            {
                int LA8_2 = input.LA(2);
                if ((LA8_2 == ASSIGN))
                {
                    alt8 = 1;
                }
                else if ((LA8_2 == COMMA || LA8_2 == SEMI))
                {
                    alt8 = 2;
                }

                else
                {
                    int nvaeMark = input.mark();
                    try
                    {
                        input.consume();
                        NoViableAltException nvae =
                            new NoViableAltException("", 8, 2, input);
                        throw nvae;
                    }
                    finally
                    {
                        input.rewind(nvaeMark);
                    }
                }

            }

            else
            {
                NoViableAltException nvae =
                    new NoViableAltException("", 8, 0, input);
                throw nvae;
            }

            switch (alt8)
            {
                case 1:
                    // org\\antlr\\v4\\parse\\ANTLRParser.g:257:9: id ASSIGN ^ id
                    {
                        root_0 = (GrammarAST)adaptor.nil();


                        pushFollow(FOLLOW_id_in_delegateGrammar955);
                        id31 = id();
                        state._fsp--;

                        adaptor.addChild(root_0, id31.getTree());

                        ASSIGN32 = (Token)Match(input, ASSIGN, FOLLOW_ASSIGN_in_delegateGrammar957);
                        ASSIGN32_tree = (GrammarAST)adaptor.create(ASSIGN32);
                        root_0 = (GrammarAST)adaptor.becomeRoot(ASSIGN32_tree, root_0);

                        pushFollow(FOLLOW_id_in_delegateGrammar960);
                        id33 = id();
                        state._fsp--;

                        adaptor.addChild(root_0, id33.getTree());

                    }
                    break;
                case 2:
                    // org\\antlr\\v4\\parse\\ANTLRParser.g:258:9: id
                    {
                        root_0 = (GrammarAST)adaptor.nil();


                        pushFollow(FOLLOW_id_in_delegateGrammar970);
                        id34 = id();
                        state._fsp--;

                        adaptor.addChild(root_0, id34.getTree());

                    }
                    break;

            }
            retval.stop = input.LT(-1);

            retval.tree = (GrammarAST)adaptor.rulePostProcessing(root_0);
            adaptor.setTokenBoundaries(retval.tree, retval.start, retval.stop);

        }
        catch (RecognitionException re)
        {
            reportError(re);
            recover(input, re);
            retval.tree = (GrammarAST)adaptor.errorNode(input, retval.start, input.LT(-1), re);
        }
        finally
        {
            // do for sure before leaving
        }
        return retval;
    }
    // $ANTLR end "delegateGrammar"


    public class tokensSpec_return : ParserRuleReturnScope
    {
        public GrammarAST tree;
        //@Override

        public GrammarAST getTree() { return tree; }
    };


    // $ANTLR start "tokensSpec"
    // org\\antlr\\v4\\parse\\ANTLRParser.g:261:1: tokensSpec : ( TOKENS_SPEC id ( COMMA id )* RBRACE -> ^( TOKENS_SPEC ( id )+ ) | TOKENS_SPEC RBRACE ->);
    public ANTLRParser.tokensSpec_return tokensSpec()  {
        ANTLRParser.tokensSpec_return retval = new ANTLRParser.tokensSpec_return();
        retval.start = input.LT(1);

        GrammarAST root_0 = null;

        Token TOKENS_SPEC35 = null;
        Token COMMA37 = null;
        Token RBRACE39 = null;
        Token TOKENS_SPEC40 = null;
        Token RBRACE41 = null;
        ParserRuleReturnScope id36 = null;
        ParserRuleReturnScope id38 = null;

        GrammarAST TOKENS_SPEC35_tree = null;
        GrammarAST COMMA37_tree = null;
        GrammarAST RBRACE39_tree = null;
        GrammarAST TOKENS_SPEC40_tree = null;
        GrammarAST RBRACE41_tree = null;
        RewriteRuleTokenStream stream_COMMA = new RewriteRuleTokenStream(adaptor, "token COMMA");
        RewriteRuleTokenStream stream_RBRACE = new RewriteRuleTokenStream(adaptor, "token RBRACE");
        RewriteRuleTokenStream stream_TOKENS_SPEC = new RewriteRuleTokenStream(adaptor, "token TOKENS_SPEC");
        RewriteRuleSubtreeStream stream_id = new RewriteRuleSubtreeStream(adaptor, "rule id");

        try
        {
            // org\\antlr\\v4\\parse\\ANTLRParser.g:262:2: ( TOKENS_SPEC id ( COMMA id )* RBRACE -> ^( TOKENS_SPEC ( id )+ ) | TOKENS_SPEC RBRACE ->)
            int alt10 = 2;
            int LA10_0 = input.LA(1);
            if ((LA10_0 == TOKENS_SPEC))
            {
                int LA10_1 = input.LA(2);
                if ((LA10_1 == RBRACE))
                {
                    alt10 = 2;
                }
                else if ((LA10_1 == RULE_REF || LA10_1 == TOKEN_REF))
                {
                    alt10 = 1;
                }

                else
                {
                    int nvaeMark = input.mark();
                    try
                    {
                        input.consume();
                        NoViableAltException nvae =
                            new NoViableAltException("", 10, 1, input);
                        throw nvae;
                    }
                    finally
                    {
                        input.rewind(nvaeMark);
                    }
                }

            }

            else
            {
                NoViableAltException nvae =
                    new NoViableAltException("", 10, 0, input);
                throw nvae;
            }

            switch (alt10)
            {
                case 1:
                    // org\\antlr\\v4\\parse\\ANTLRParser.g:262:4: TOKENS_SPEC id ( COMMA id )* RBRACE
                    {
                        TOKENS_SPEC35 = (Token)Match(input, TOKENS_SPEC, FOLLOW_TOKENS_SPEC_in_tokensSpec984);
                        stream_TOKENS_SPEC.add(TOKENS_SPEC35);

                        pushFollow(FOLLOW_id_in_tokensSpec986);
                        id36 = id();
                        state._fsp--;

                        stream_id.add(id36.getTree());
                    // org\\antlr\\v4\\parse\\ANTLRParser.g:262:19: ( COMMA id )*
                    loop9:
                        while (true)
                        {
                            int alt9 = 2;
                            int LA9_0 = input.LA(1);
                            if ((LA9_0 == COMMA))
                            {
                                alt9 = 1;
                            }

                            switch (alt9)
                            {
                                case 1:
                                    // org\\antlr\\v4\\parse\\ANTLRParser.g:262:20: COMMA id
                                    {
                                        COMMA37 = (Token)Match(input, COMMA, FOLLOW_COMMA_in_tokensSpec989);
                                        stream_COMMA.add(COMMA37);

                                        pushFollow(FOLLOW_id_in_tokensSpec991);
                                        id38 = id();
                                        state._fsp--;

                                        stream_id.add(id38.getTree());
                                    }
                                    break;

                                default:
                                    goto exit9;
                                    //break loop9;
                            }
                        }
                    exit9:
                        RBRACE39 = (Token)Match(input, RBRACE, FOLLOW_RBRACE_in_tokensSpec995);
                        stream_RBRACE.add(RBRACE39);


                        // AST REWRITE
                        // elements: id, TOKENS_SPEC
                        // token labels: 
                        // rule labels: retval
                        // token list labels: 
                        // rule list labels: 
                        // wildcard labels: 
                        retval.tree = root_0;
                        RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval != null ? retval.getTree() : null);

                        root_0 = (GrammarAST)adaptor.nil();
                        // 262:38: -> ^( TOKENS_SPEC ( id )+ )
                        {
                            // org\\antlr\\v4\\parse\\ANTLRParser.g:262:41: ^( TOKENS_SPEC ( id )+ )
                            {
                                GrammarAST root_1 = (GrammarAST)adaptor.nil();
                                root_1 = (GrammarAST)adaptor.becomeRoot(stream_TOKENS_SPEC.nextNode(), root_1);
                                if (!(stream_id.hasNext()))
                                {
                                    throw new RewriteEarlyExitException();
                                }
                                while (stream_id.hasNext())
                                {
                                    adaptor.addChild(root_1, stream_id.nextTree());
                                }
                                stream_id.reset();

                                adaptor.addChild(root_0, root_1);
                            }

                        }


                        retval.tree = root_0;

                    }
                    break;
                case 2:
                    // org\\antlr\\v4\\parse\\ANTLRParser.g:263:7: TOKENS_SPEC RBRACE
                    {
                        TOKENS_SPEC40 = (Token)Match(input, TOKENS_SPEC, FOLLOW_TOKENS_SPEC_in_tokensSpec1012);
                        stream_TOKENS_SPEC.add(TOKENS_SPEC40);

                        RBRACE41 = (Token)Match(input, RBRACE, FOLLOW_RBRACE_in_tokensSpec1014);
                        stream_RBRACE.add(RBRACE41);


                        // AST REWRITE
                        // elements: 
                        // token labels: 
                        // rule labels: retval
                        // token list labels: 
                        // rule list labels: 
                        // wildcard labels: 
                        retval.tree = root_0;
                        RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval != null ? retval.getTree() : null);

                        root_0 = (GrammarAST)adaptor.nil();
                        // 263:26: ->
                        {
                            root_0 = null;
                        }


                        retval.tree = root_0;

                    }
                    break;

            }
            retval.stop = input.LT(-1);

            retval.tree = (GrammarAST)adaptor.rulePostProcessing(root_0);
            adaptor.setTokenBoundaries(retval.tree, retval.start, retval.stop);

        }
        catch (RecognitionException re)
        {
            reportError(re);
            recover(input, re);
            retval.tree = (GrammarAST)adaptor.errorNode(input, retval.start, input.LT(-1), re);
        }
        finally
        {
            // do for sure before leaving
        }
        return retval;
    }
    // $ANTLR end "tokensSpec"


    public class channelsSpec_return : ParserRuleReturnScope
    {
        public GrammarAST tree;
        //@Override

        public GrammarAST getTree() { return tree; }
    };


    // $ANTLR start "channelsSpec"
    // org\\antlr\\v4\\parse\\ANTLRParser.g:266:1: channelsSpec : CHANNELS ^ id ( COMMA ! id )* RBRACE !;
    public ANTLRParser.channelsSpec_return channelsSpec()  {
        ANTLRParser.channelsSpec_return retval = new ANTLRParser.channelsSpec_return();
        retval.start = input.LT(1);

        GrammarAST root_0 = null;

        Token CHANNELS42 = null;
        Token COMMA44 = null;
        Token RBRACE46 = null;
        ParserRuleReturnScope id43 = null;
        ParserRuleReturnScope id45 = null;

        GrammarAST CHANNELS42_tree = null;
        GrammarAST COMMA44_tree = null;
        GrammarAST RBRACE46_tree = null;

        try
        {
            // org\\antlr\\v4\\parse\\ANTLRParser.g:267:2: ( CHANNELS ^ id ( COMMA ! id )* RBRACE !)
            // org\\antlr\\v4\\parse\\ANTLRParser.g:267:4: CHANNELS ^ id ( COMMA ! id )* RBRACE !
            {
                root_0 = (GrammarAST)adaptor.nil();


                CHANNELS42 = (Token)Match(input, CHANNELS, FOLLOW_CHANNELS_in_channelsSpec1027);
                CHANNELS42_tree = (GrammarAST)adaptor.create(CHANNELS42);
                root_0 = (GrammarAST)adaptor.becomeRoot(CHANNELS42_tree, root_0);

                pushFollow(FOLLOW_id_in_channelsSpec1030);
                id43 = id();
                state._fsp--;

                adaptor.addChild(root_0, id43.getTree());

            // org\\antlr\\v4\\parse\\ANTLRParser.g:267:17: ( COMMA ! id )*
            loop11:
                while (true)
                {
                    int alt11 = 2;
                    int LA11_0 = input.LA(1);
                    if ((LA11_0 == COMMA))
                    {
                        alt11 = 1;
                    }

                    switch (alt11)
                    {
                        case 1:
                            // org\\antlr\\v4\\parse\\ANTLRParser.g:267:18: COMMA ! id
                            {
                                COMMA44 = (Token)Match(input, COMMA, FOLLOW_COMMA_in_channelsSpec1033);
                                pushFollow(FOLLOW_id_in_channelsSpec1036);
                                id45 = id();
                                state._fsp--;

                                adaptor.addChild(root_0, id45.getTree());

                            }
                            break;

                        default:
                            goto exit11;
                            //break loop11;
                    }
                }
            exit11:
                RBRACE46 = (Token)Match(input, RBRACE, FOLLOW_RBRACE_in_channelsSpec1040);
            }

            retval.stop = input.LT(-1);

            retval.tree = (GrammarAST)adaptor.rulePostProcessing(root_0);
            adaptor.setTokenBoundaries(retval.tree, retval.start, retval.stop);

        }
        catch (RecognitionException re)
        {
            reportError(re);
            recover(input, re);
            retval.tree = (GrammarAST)adaptor.errorNode(input, retval.start, input.LT(-1), re);
        }
        finally
        {
            // do for sure before leaving
        }
        return retval;
    }
    // $ANTLR end "channelsSpec"


    public class action_return : ParserRuleReturnScope
    {
        public GrammarAST tree;
        //@Override

        public GrammarAST getTree() { return tree; }
    };


    // $ANTLR start "action"
    // org\\antlr\\v4\\parse\\ANTLRParser.g:274:1: action : AT ( actionScopeName COLONCOLON )? id ACTION -> ^( AT ( actionScopeName )? id ACTION ) ;
    public ANTLRParser.action_return action()  {
        ANTLRParser.action_return retval = new ANTLRParser.action_return();
        retval.start = input.LT(1);

        GrammarAST root_0 = null;

        Token AT47 = null;
        Token COLONCOLON49 = null;
        Token ACTION51 = null;
        ParserRuleReturnScope actionScopeName48 = null;
        ParserRuleReturnScope id50 = null;

        GrammarAST AT47_tree = null;
        GrammarAST COLONCOLON49_tree = null;
        GrammarAST ACTION51_tree = null;
        RewriteRuleTokenStream stream_AT = new RewriteRuleTokenStream(adaptor, "token AT");
        RewriteRuleTokenStream stream_ACTION = new RewriteRuleTokenStream(adaptor, "token ACTION");
        RewriteRuleTokenStream stream_COLONCOLON = new RewriteRuleTokenStream(adaptor, "token COLONCOLON");
        RewriteRuleSubtreeStream stream_id = new RewriteRuleSubtreeStream(adaptor, "rule id");
        RewriteRuleSubtreeStream stream_actionScopeName = new RewriteRuleSubtreeStream(adaptor, "rule actionScopeName");

        try
        {
            // org\\antlr\\v4\\parse\\ANTLRParser.g:275:2: ( AT ( actionScopeName COLONCOLON )? id ACTION -> ^( AT ( actionScopeName )? id ACTION ) )
            // org\\antlr\\v4\\parse\\ANTLRParser.g:275:4: AT ( actionScopeName COLONCOLON )? id ACTION
            {
                AT47 = (Token)Match(input, AT, FOLLOW_AT_in_action1057);
                stream_AT.add(AT47);

                // org\\antlr\\v4\\parse\\ANTLRParser.g:275:7: ( actionScopeName COLONCOLON )?
                int alt12 = 2;
                switch (input.LA(1))
                {
                    case RULE_REF:
                        {
                            int LA12_1 = input.LA(2);
                            if ((LA12_1 == COLONCOLON))
                            {
                                alt12 = 1;
                            }
                        }
                        break;
                    case TOKEN_REF:
                        {
                            int LA12_2 = input.LA(2);
                            if ((LA12_2 == COLONCOLON))
                            {
                                alt12 = 1;
                            }
                        }
                        break;
                    case LEXER:
                    case PARSER:
                        {
                            alt12 = 1;
                        }
                        break;
                }
                switch (alt12)
                {
                    case 1:
                        // org\\antlr\\v4\\parse\\ANTLRParser.g:275:8: actionScopeName COLONCOLON
                        {
                            pushFollow(FOLLOW_actionScopeName_in_action1060);
                            actionScopeName48 = actionScopeName();
                            state._fsp--;

                            stream_actionScopeName.add(actionScopeName48.getTree());
                            COLONCOLON49 = (Token)Match(input, COLONCOLON, FOLLOW_COLONCOLON_in_action1062);
                            stream_COLONCOLON.add(COLONCOLON49);

                        }
                        break;

                }

                pushFollow(FOLLOW_id_in_action1066);
                id50 = id();
                state._fsp--;

                stream_id.add(id50.getTree());
                ACTION51 = (Token)Match(input, ACTION, FOLLOW_ACTION_in_action1068);
                stream_ACTION.add(ACTION51);


                // AST REWRITE
                // elements: id, AT, ACTION, actionScopeName
                // token labels: 
                // rule labels: retval
                // token list labels: 
                // rule list labels: 
                // wildcard labels: 
                retval.tree = root_0;
                RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval != null ? retval.getTree() : null);

                root_0 = (GrammarAST)adaptor.nil();
                // 275:47: -> ^( AT ( actionScopeName )? id ACTION )
                {
                    // org\\antlr\\v4\\parse\\ANTLRParser.g:275:50: ^( AT ( actionScopeName )? id ACTION )
                    {
                        GrammarAST root_1 = (GrammarAST)adaptor.nil();
                        root_1 = (GrammarAST)adaptor.becomeRoot(stream_AT.nextNode(), root_1);
                        // org\\antlr\\v4\\parse\\ANTLRParser.g:275:55: ( actionScopeName )?
                        if (stream_actionScopeName.hasNext())
                        {
                            adaptor.addChild(root_1, stream_actionScopeName.nextTree());
                        }
                        stream_actionScopeName.reset();

                        adaptor.addChild(root_1, stream_id.nextTree());
                        adaptor.addChild(root_1, new ActionAST(stream_ACTION.nextToken()));
                        adaptor.addChild(root_0, root_1);
                    }

                }


                retval.tree = root_0;

            }

            retval.stop = input.LT(-1);

            retval.tree = (GrammarAST)adaptor.rulePostProcessing(root_0);
            adaptor.setTokenBoundaries(retval.tree, retval.start, retval.stop);

        }
        catch (RecognitionException re)
        {
            reportError(re);
            recover(input, re);
            retval.tree = (GrammarAST)adaptor.errorNode(input, retval.start, input.LT(-1), re);
        }
        finally
        {
            // do for sure before leaving
        }
        return retval;
    }
    // $ANTLR end "action"


    public class actionScopeName_return : ParserRuleReturnScope
    {
        public GrammarAST tree;
        //@Override

        public GrammarAST getTree() { return tree; }
    };


    // $ANTLR start "actionScopeName"
    // org\\antlr\\v4\\parse\\ANTLRParser.g:281:1: actionScopeName : ( id | LEXER -> ID[$LEXER] | PARSER -> ID[$PARSER] );
    public ANTLRParser.actionScopeName_return actionScopeName()  {
        ANTLRParser.actionScopeName_return retval = new ANTLRParser.actionScopeName_return();
        retval.start = input.LT(1);

        GrammarAST root_0 = null;

        Token LEXER53 = null;
        Token PARSER54 = null;
        ParserRuleReturnScope id52 = null;

        GrammarAST LEXER53_tree = null;
        GrammarAST PARSER54_tree = null;
        RewriteRuleTokenStream stream_PARSER = new RewriteRuleTokenStream(adaptor, "token PARSER");
        RewriteRuleTokenStream stream_LEXER = new RewriteRuleTokenStream(adaptor, "token LEXER");

        try
        {
            // org\\antlr\\v4\\parse\\ANTLRParser.g:282:2: ( id | LEXER -> ID[$LEXER] | PARSER -> ID[$PARSER] )
            int alt13 = 3;
            switch (input.LA(1))
            {
                case RULE_REF:
                case TOKEN_REF:
                    {
                        alt13 = 1;
                    }
                    break;
                case LEXER:
                    {
                        alt13 = 2;
                    }
                    break;
                case PARSER:
                    {
                        alt13 = 3;
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
                    // org\\antlr\\v4\\parse\\ANTLRParser.g:282:4: id
                    {
                        root_0 = (GrammarAST)adaptor.nil();


                        pushFollow(FOLLOW_id_in_actionScopeName1097);
                        id52 = id();
                        state._fsp--;

                        adaptor.addChild(root_0, id52.getTree());

                    }
                    break;
                case 2:
                    // org\\antlr\\v4\\parse\\ANTLRParser.g:283:4: LEXER
                    {
                        LEXER53 = (Token)Match(input, LEXER, FOLLOW_LEXER_in_actionScopeName1102);
                        stream_LEXER.add(LEXER53);


                        // AST REWRITE
                        // elements: 
                        // token labels: 
                        // rule labels: retval
                        // token list labels: 
                        // rule list labels: 
                        // wildcard labels: 
                        retval.tree = root_0;
                        RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval != null ? retval.getTree() : null);

                        root_0 = (GrammarAST)adaptor.nil();
                        // 283:10: -> ID[$LEXER]
                        {
                            adaptor.addChild(root_0, (GrammarAST)adaptor.create(ID, LEXER53));
                        }


                        retval.tree = root_0;

                    }
                    break;
                case 3:
                    // org\\antlr\\v4\\parse\\ANTLRParser.g:284:9: PARSER
                    {
                        PARSER54 = (Token)Match(input, PARSER, FOLLOW_PARSER_in_actionScopeName1117);
                        stream_PARSER.add(PARSER54);


                        // AST REWRITE
                        // elements: 
                        // token labels: 
                        // rule labels: retval
                        // token list labels: 
                        // rule list labels: 
                        // wildcard labels: 
                        retval.tree = root_0;
                        RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval != null ? retval.getTree() : null);

                        root_0 = (GrammarAST)adaptor.nil();
                        // 284:16: -> ID[$PARSER]
                        {
                            adaptor.addChild(root_0, (GrammarAST)adaptor.create(ID, PARSER54));
                        }


                        retval.tree = root_0;

                    }
                    break;

            }
            retval.stop = input.LT(-1);

            retval.tree = (GrammarAST)adaptor.rulePostProcessing(root_0);
            adaptor.setTokenBoundaries(retval.tree, retval.start, retval.stop);

        }
        catch (RecognitionException re)
        {
            reportError(re);
            recover(input, re);
            retval.tree = (GrammarAST)adaptor.errorNode(input, retval.start, input.LT(-1), re);
        }
        finally
        {
            // do for sure before leaving
        }
        return retval;
    }
    // $ANTLR end "actionScopeName"


    public class modeSpec_return : ParserRuleReturnScope
    {
        public GrammarAST tree;
        //@Override

        public GrammarAST getTree() { return tree; }
    };


    // $ANTLR start "modeSpec"
    // org\\antlr\\v4\\parse\\ANTLRParser.g:287:1: modeSpec : MODE id SEMI sync ( lexerRule sync )* -> ^( MODE id ( lexerRule )* ) ;
    public ANTLRParser.modeSpec_return modeSpec()  {
        ANTLRParser.modeSpec_return retval = new ANTLRParser.modeSpec_return();
        retval.start = input.LT(1);

        GrammarAST root_0 = null;

        Token MODE55 = null;
        Token SEMI57 = null;
        ParserRuleReturnScope id56 = null;
        ParserRuleReturnScope sync58 = null;
        ParserRuleReturnScope lexerRule59 = null;
        ParserRuleReturnScope sync60 = null;

        GrammarAST MODE55_tree = null;
        GrammarAST SEMI57_tree = null;
        RewriteRuleTokenStream stream_MODE = new RewriteRuleTokenStream(adaptor, "token MODE");
        RewriteRuleTokenStream stream_SEMI = new RewriteRuleTokenStream(adaptor, "token SEMI");
        RewriteRuleSubtreeStream stream_lexerRule = new RewriteRuleSubtreeStream(adaptor, "rule lexerRule");
        RewriteRuleSubtreeStream stream_id = new RewriteRuleSubtreeStream(adaptor, "rule id");
        RewriteRuleSubtreeStream stream_sync = new RewriteRuleSubtreeStream(adaptor, "rule sync");

        try
        {
            // org\\antlr\\v4\\parse\\ANTLRParser.g:288:5: ( MODE id SEMI sync ( lexerRule sync )* -> ^( MODE id ( lexerRule )* ) )
            // org\\antlr\\v4\\parse\\ANTLRParser.g:288:7: MODE id SEMI sync ( lexerRule sync )*
            {
                MODE55 = (Token)Match(input, MODE, FOLLOW_MODE_in_modeSpec1136);
                stream_MODE.add(MODE55);

                pushFollow(FOLLOW_id_in_modeSpec1138);
                id56 = id();
                state._fsp--;

                stream_id.add(id56.getTree());
                SEMI57 = (Token)Match(input, SEMI, FOLLOW_SEMI_in_modeSpec1140);
                stream_SEMI.add(SEMI57);

                pushFollow(FOLLOW_sync_in_modeSpec1142);
                sync58 = sync();
                state._fsp--;

                stream_sync.add(sync58.getTree());
            // org\\antlr\\v4\\parse\\ANTLRParser.g:288:25: ( lexerRule sync )*
            loop14:
                while (true)
                {
                    int alt14 = 2;
                    int LA14_0 = input.LA(1);
                    if ((LA14_0 == FRAGMENT || LA14_0 == TOKEN_REF))
                    {
                        alt14 = 1;
                    }

                    switch (alt14)
                    {
                        case 1:
                            // org\\antlr\\v4\\parse\\ANTLRParser.g:288:26: lexerRule sync
                            {
                                pushFollow(FOLLOW_lexerRule_in_modeSpec1145);
                                lexerRule59 = lexerRule();
                                state._fsp--;

                                stream_lexerRule.add(lexerRule59.getTree());
                                pushFollow(FOLLOW_sync_in_modeSpec1147);
                                sync60 = sync();
                                state._fsp--;

                                stream_sync.add(sync60.getTree());
                            }
                            break;

                        default:
                            goto exit14;
                            //break loop14;
                    }
                }
            exit14:

                // AST REWRITE
                // elements: MODE, lexerRule, id
                // token labels: 
                // rule labels: retval
                // token list labels: 
                // rule list labels: 
                // wildcard labels: 
                retval.tree = root_0;
                RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval != null ? retval.getTree() : null);

                root_0 = (GrammarAST)adaptor.nil();
                // 288:44: -> ^( MODE id ( lexerRule )* )
                {
                    // org\\antlr\\v4\\parse\\ANTLRParser.g:288:47: ^( MODE id ( lexerRule )* )
                    {
                        GrammarAST root_1 = (GrammarAST)adaptor.nil();
                        root_1 = (GrammarAST)adaptor.becomeRoot(stream_MODE.nextNode(), root_1);
                        adaptor.addChild(root_1, stream_id.nextTree());
                        // org\\antlr\\v4\\parse\\ANTLRParser.g:288:57: ( lexerRule )*
                        while (stream_lexerRule.hasNext())
                        {
                            adaptor.addChild(root_1, stream_lexerRule.nextTree());
                        }
                        stream_lexerRule.reset();

                        adaptor.addChild(root_0, root_1);
                    }

                }


                retval.tree = root_0;

            }

            retval.stop = input.LT(-1);

            retval.tree = (GrammarAST)adaptor.rulePostProcessing(root_0);
            adaptor.setTokenBoundaries(retval.tree, retval.start, retval.stop);

        }
        catch (RecognitionException re)
        {
            reportError(re);
            recover(input, re);
            retval.tree = (GrammarAST)adaptor.errorNode(input, retval.start, input.LT(-1), re);
        }
        finally
        {
            // do for sure before leaving
        }
        return retval;
    }
    // $ANTLR end "modeSpec"


    public class rules_return : ParserRuleReturnScope
    {
        public GrammarAST tree;
        //@Override

        public GrammarAST getTree() { return tree; }
    };


    // $ANTLR start "rules"
    // org\\antlr\\v4\\parse\\ANTLRParser.g:291:1: rules : sync ( rule sync )* -> ^( RULES ( rule )* ) ;
    public ANTLRParser.rules_return rules()  {
        ANTLRParser.rules_return retval = new ANTLRParser.rules_return();
        retval.start = input.LT(1);

        GrammarAST root_0 = null;

        ParserRuleReturnScope sync61 = null;
        ParserRuleReturnScope rule62 = null;
        ParserRuleReturnScope sync63 = null;

        RewriteRuleSubtreeStream stream_rule = new RewriteRuleSubtreeStream(adaptor, "rule rule");
        RewriteRuleSubtreeStream stream_sync = new RewriteRuleSubtreeStream(adaptor, "rule sync");

        try
        {
            // org\\antlr\\v4\\parse\\ANTLRParser.g:292:5: ( sync ( rule sync )* -> ^( RULES ( rule )* ) )
            // org\\antlr\\v4\\parse\\ANTLRParser.g:292:7: sync ( rule sync )*
            {
                pushFollow(FOLLOW_sync_in_rules1178);
                sync61 = sync();
                state._fsp--;

                stream_sync.add(sync61.getTree());
            // org\\antlr\\v4\\parse\\ANTLRParser.g:292:12: ( rule sync )*
            loop15:
                while (true)
                {
                    int alt15 = 2;
                    int LA15_0 = input.LA(1);
                    if ((LA15_0 == FRAGMENT || LA15_0 == RULE_REF || LA15_0 == TOKEN_REF))
                    {
                        alt15 = 1;
                    }

                    switch (alt15)
                    {
                        case 1:
                            // org\\antlr\\v4\\parse\\ANTLRParser.g:292:13: rule sync
                            {
                                pushFollow(FOLLOW_rule_in_rules1181);
                                rule62 = rule();
                                state._fsp--;

                                stream_rule.add(rule62.getTree());
                                pushFollow(FOLLOW_sync_in_rules1183);
                                sync63 = sync();
                                state._fsp--;

                                stream_sync.add(sync63.getTree());
                            }
                            break;

                        default:
                            goto exit15;
                            //break loop15;
                    }
                }
            exit15:

                // AST REWRITE
                // elements: rule
                // token labels: 
                // rule labels: retval
                // token list labels: 
                // rule list labels: 
                // wildcard labels: 
                retval.tree = root_0;
                RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval != null ? retval.getTree() : null);

                root_0 = (GrammarAST)adaptor.nil();
                // 296:7: -> ^( RULES ( rule )* )
                {
                    // org\\antlr\\v4\\parse\\ANTLRParser.g:296:9: ^( RULES ( rule )* )
                    {
                        GrammarAST root_1 = (GrammarAST)adaptor.nil();
                        root_1 = (GrammarAST)adaptor.becomeRoot((GrammarAST)adaptor.create(RULES, "RULES"), root_1);
                        // org\\antlr\\v4\\parse\\ANTLRParser.g:296:17: ( rule )*
                        while (stream_rule.hasNext())
                        {
                            adaptor.addChild(root_1, stream_rule.nextTree());
                        }
                        stream_rule.reset();

                        adaptor.addChild(root_0, root_1);
                    }

                }


                retval.tree = root_0;

            }

            retval.stop = input.LT(-1);

            retval.tree = (GrammarAST)adaptor.rulePostProcessing(root_0);
            adaptor.setTokenBoundaries(retval.tree, retval.start, retval.stop);

        }
        catch (RecognitionException re)
        {
            reportError(re);
            recover(input, re);
            retval.tree = (GrammarAST)adaptor.errorNode(input, retval.start, input.LT(-1), re);
        }
        finally
        {
            // do for sure before leaving
        }
        return retval;
    }
    // $ANTLR end "rules"


    public class sync_return : ParserRuleReturnScope
    {
        public GrammarAST tree;
        //@Override

        public GrammarAST getTree() { return tree; }
    };


    // $ANTLR start "sync"
    // org\\antlr\\v4\\parse\\ANTLRParser.g:299:1: sync :;
    public ANTLRParser.sync_return sync()  {
        ANTLRParser.sync_return retval = new ANTLRParser.sync_return();
        retval.start = input.LT(1);

        GrammarAST root_0 = null;


        BitSet followSet = computeErrorRecoverySet();
        if (input.LA(1) != Token.EOF && !followSet.member(input.LA(1)))
        {
            reportError(new NoViableAltException("", 0, 0, input));
            beginResync();
            consumeUntil(input, followSet);
            endResync();
        }

        try
        {
            // org\\antlr\\v4\\parse\\ANTLRParser.g:308:3: ()
            // org\\antlr\\v4\\parse\\ANTLRParser.g:309:2: 
            {
                root_0 = (GrammarAST)adaptor.nil();


            }

            retval.stop = input.LT(-1);

            retval.tree = (GrammarAST)adaptor.rulePostProcessing(root_0);
            adaptor.setTokenBoundaries(retval.tree, retval.start, retval.stop);

        }
        finally
        {
            // do for sure before leaving
        }
        return retval;
    }

    private void endResync()
    {
        throw new NotImplementedException();
    }

    private void consumeUntil(TokenStream input, BitSet followSet)
    {
        throw new NotImplementedException();
    }

    private void beginResync()
    {
        throw new NotImplementedException();
    }

    private BitSet computeErrorRecoverySet()
    {
        throw new NotImplementedException();
    }

    // $ANTLR end "sync"


    public class rule_return : ParserRuleReturnScope
    {
        public GrammarAST tree;
        //@Override

        public GrammarAST getTree() { return tree; }
    };


    // $ANTLR start "rule"
    // org\\antlr\\v4\\parse\\ANTLRParser.g:311:1: rule : ( parserRule | lexerRule );
    public ANTLRParser.rule_return rule()  {
        ANTLRParser.rule_return retval = new ANTLRParser.rule_return();
        retval.start = input.LT(1);

        GrammarAST root_0 = null;

        ParserRuleReturnScope parserRule64 = null;
        ParserRuleReturnScope lexerRule65 = null;


        try
        {
            // org\\antlr\\v4\\parse\\ANTLRParser.g:311:5: ( parserRule | lexerRule )
            int alt16 = 2;
            int LA16_0 = input.LA(1);
            if ((LA16_0 == RULE_REF))
            {
                alt16 = 1;
            }
            else if ((LA16_0 == FRAGMENT || LA16_0 == TOKEN_REF))
            {
                alt16 = 2;
            }

            else
            {
                NoViableAltException nvae =
                    new NoViableAltException("", 16, 0, input);
                throw nvae;
            }

            switch (alt16)
            {
                case 1:
                    // org\\antlr\\v4\\parse\\ANTLRParser.g:311:7: parserRule
                    {
                        root_0 = (GrammarAST)adaptor.nil();


                        pushFollow(FOLLOW_parserRule_in_rule1245);
                        parserRule64 = parserRule();
                        state._fsp--;

                        adaptor.addChild(root_0, parserRule64.getTree());

                    }
                    break;
                case 2:
                    // org\\antlr\\v4\\parse\\ANTLRParser.g:312:4: lexerRule
                    {
                        root_0 = (GrammarAST)adaptor.nil();


                        pushFollow(FOLLOW_lexerRule_in_rule1250);
                        lexerRule65 = lexerRule();
                        state._fsp--;

                        adaptor.addChild(root_0, lexerRule65.getTree());

                    }
                    break;

            }
            retval.stop = input.LT(-1);

            retval.tree = (GrammarAST)adaptor.rulePostProcessing(root_0);
            adaptor.setTokenBoundaries(retval.tree, retval.start, retval.stop);

        }
        catch (RecognitionException re)
        {
            reportError(re);
            recover(input, re);
            retval.tree = (GrammarAST)adaptor.errorNode(input, retval.start, input.LT(-1), re);
        }
        finally
        {
            // do for sure before leaving
        }
        return retval;
    }
    // $ANTLR end "rule"


    public class parserRule_return : ParserRuleReturnScope
    {
        public GrammarAST tree;
        //@Override

        public GrammarAST getTree() { return tree; }
    };


    // $ANTLR start "parserRule"
    // org\\antlr\\v4\\parse\\ANTLRParser.g:324:1: parserRule : RULE_REF ( ARG_ACTION )? ( ruleReturns )? ( throwsSpec )? ( localsSpec )? rulePrequels COLON ruleBlock SEMI exceptionGroup -> ^( RULE RULE_REF ( ARG_ACTION )? ( ruleReturns )? ( throwsSpec )? ( localsSpec )? ( rulePrequels )? ruleBlock ( exceptionGroup )* ) ;
    public ANTLRParser.parserRule_return parserRule()  {
        ANTLRParser.parserRule_return retval = new ANTLRParser.parserRule_return();
        retval.start = input.LT(1);

        GrammarAST root_0 = null;

        Token RULE_REF66 = null;
        Token ARG_ACTION67 = null;
        Token COLON72 = null;
        Token SEMI74 = null;
        ParserRuleReturnScope ruleReturns68 = null;
        ParserRuleReturnScope throwsSpec69 = null;
        ParserRuleReturnScope localsSpec70 = null;
        ParserRuleReturnScope rulePrequels71 = null;
        ParserRuleReturnScope ruleBlock73 = null;
        ParserRuleReturnScope exceptionGroup75 = null;

        GrammarAST RULE_REF66_tree = null;
        GrammarAST ARG_ACTION67_tree = null;
        GrammarAST COLON72_tree = null;
        GrammarAST SEMI74_tree = null;
        RewriteRuleTokenStream stream_ARG_ACTION = new RewriteRuleTokenStream(adaptor, "token ARG_ACTION");
        RewriteRuleTokenStream stream_SEMI = new RewriteRuleTokenStream(adaptor, "token SEMI");
        RewriteRuleTokenStream stream_COLON = new RewriteRuleTokenStream(adaptor, "token COLON");
        RewriteRuleTokenStream stream_RULE_REF = new RewriteRuleTokenStream(adaptor, "token RULE_REF");
        RewriteRuleSubtreeStream stream_throwsSpec = new RewriteRuleSubtreeStream(adaptor, "rule throwsSpec");
        RewriteRuleSubtreeStream stream_localsSpec = new RewriteRuleSubtreeStream(adaptor, "rule localsSpec");
        RewriteRuleSubtreeStream stream_ruleBlock = new RewriteRuleSubtreeStream(adaptor, "rule ruleBlock");
        RewriteRuleSubtreeStream stream_ruleReturns = new RewriteRuleSubtreeStream(adaptor, "rule ruleReturns");
        RewriteRuleSubtreeStream stream_rulePrequels = new RewriteRuleSubtreeStream(adaptor, "rule rulePrequels");
        RewriteRuleSubtreeStream stream_exceptionGroup = new RewriteRuleSubtreeStream(adaptor, "rule exceptionGroup");

        paraphrases.push("matching a rule");
        try
        {
            // org\\antlr\\v4\\parse\\ANTLRParser.g:333:5: ( RULE_REF ( ARG_ACTION )? ( ruleReturns )? ( throwsSpec )? ( localsSpec )? rulePrequels COLON ruleBlock SEMI exceptionGroup -> ^( RULE RULE_REF ( ARG_ACTION )? ( ruleReturns )? ( throwsSpec )? ( localsSpec )? ( rulePrequels )? ruleBlock ( exceptionGroup )* ) )
            // org\\antlr\\v4\\parse\\ANTLRParser.g:337:4: RULE_REF ( ARG_ACTION )? ( ruleReturns )? ( throwsSpec )? ( localsSpec )? rulePrequels COLON ruleBlock SEMI exceptionGroup
            {
                RULE_REF66 = (Token)Match(input, RULE_REF, FOLLOW_RULE_REF_in_parserRule1299);
                stream_RULE_REF.add(RULE_REF66);

                // org\\antlr\\v4\\parse\\ANTLRParser.g:345:4: ( ARG_ACTION )?
                int alt17 = 2;
                int LA17_0 = input.LA(1);
                if ((LA17_0 == ARG_ACTION))
                {
                    alt17 = 1;
                }
                switch (alt17)
                {
                    case 1:
                        // org\\antlr\\v4\\parse\\ANTLRParser.g:345:4: ARG_ACTION
                        {
                            ARG_ACTION67 = (Token)Match(input, ARG_ACTION, FOLLOW_ARG_ACTION_in_parserRule1329);
                            stream_ARG_ACTION.add(ARG_ACTION67);

                        }
                        break;

                }

                // org\\antlr\\v4\\parse\\ANTLRParser.g:347:4: ( ruleReturns )?
                int alt18 = 2;
                int LA18_0 = input.LA(1);
                if ((LA18_0 == RETURNS))
                {
                    alt18 = 1;
                }
                switch (alt18)
                {
                    case 1:
                        // org\\antlr\\v4\\parse\\ANTLRParser.g:347:4: ruleReturns
                        {
                            pushFollow(FOLLOW_ruleReturns_in_parserRule1336);
                            ruleReturns68 = ruleReturns();
                            state._fsp--;

                            stream_ruleReturns.add(ruleReturns68.getTree());
                        }
                        break;

                }

                // org\\antlr\\v4\\parse\\ANTLRParser.g:349:4: ( throwsSpec )?
                int alt19 = 2;
                int LA19_0 = input.LA(1);
                if ((LA19_0 == THROWS))
                {
                    alt19 = 1;
                }
                switch (alt19)
                {
                    case 1:
                        // org\\antlr\\v4\\parse\\ANTLRParser.g:349:4: throwsSpec
                        {
                            pushFollow(FOLLOW_throwsSpec_in_parserRule1343);
                            throwsSpec69 = throwsSpec();
                            state._fsp--;

                            stream_throwsSpec.add(throwsSpec69.getTree());
                        }
                        break;

                }

                // org\\antlr\\v4\\parse\\ANTLRParser.g:351:4: ( localsSpec )?
                int alt20 = 2;
                int LA20_0 = input.LA(1);
                if ((LA20_0 == LOCALS))
                {
                    alt20 = 1;
                }
                switch (alt20)
                {
                    case 1:
                        // org\\antlr\\v4\\parse\\ANTLRParser.g:351:4: localsSpec
                        {
                            pushFollow(FOLLOW_localsSpec_in_parserRule1350);
                            localsSpec70 = localsSpec();
                            state._fsp--;

                            stream_localsSpec.add(localsSpec70.getTree());
                        }
                        break;

                }

                pushFollow(FOLLOW_rulePrequels_in_parserRule1388);
                rulePrequels71 = rulePrequels();
                state._fsp--;

                stream_rulePrequels.add(rulePrequels71.getTree());
                COLON72 = (Token)Match(input, COLON, FOLLOW_COLON_in_parserRule1397);
                stream_COLON.add(COLON72);

                pushFollow(FOLLOW_ruleBlock_in_parserRule1420);
                ruleBlock73 = ruleBlock();
                state._fsp--;

                stream_ruleBlock.add(ruleBlock73.getTree());
                SEMI74 = (Token)Match(input, SEMI, FOLLOW_SEMI_in_parserRule1429);
                stream_SEMI.add(SEMI74);

                pushFollow(FOLLOW_exceptionGroup_in_parserRule1438);
                exceptionGroup75 = exceptionGroup();
                state._fsp--;

                stream_exceptionGroup.add(exceptionGroup75.getTree());

                // AST REWRITE
                // elements: ruleBlock, ruleReturns, throwsSpec, rulePrequels, RULE_REF, ARG_ACTION, exceptionGroup, localsSpec
                // token labels: 
                // rule labels: retval
                // token list labels: 
                // rule list labels: 
                // wildcard labels: 
                retval.tree = root_0;
                RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval != null ? retval.getTree() : null);

                root_0 = (GrammarAST)adaptor.nil();
                // 378:7: -> ^( RULE RULE_REF ( ARG_ACTION )? ( ruleReturns )? ( throwsSpec )? ( localsSpec )? ( rulePrequels )? ruleBlock ( exceptionGroup )* )
                {
                    // org\\antlr\\v4\\parse\\ANTLRParser.g:378:10: ^( RULE RULE_REF ( ARG_ACTION )? ( ruleReturns )? ( throwsSpec )? ( localsSpec )? ( rulePrequels )? ruleBlock ( exceptionGroup )* )
                    {
                        GrammarAST root_1 = (GrammarAST)adaptor.nil();
                        root_1 = (GrammarAST)adaptor.becomeRoot(new RuleAST(RULE), root_1);
                        adaptor.addChild(root_1, stream_RULE_REF.nextNode());
                        // org\\antlr\\v4\\parse\\ANTLRParser.g:378:36: ( ARG_ACTION )?
                        if (stream_ARG_ACTION.hasNext())
                        {
                            adaptor.addChild(root_1, new ActionAST(stream_ARG_ACTION.nextToken()));
                        }
                        stream_ARG_ACTION.reset();

                        // org\\antlr\\v4\\parse\\ANTLRParser.g:379:9: ( ruleReturns )?
                        if (stream_ruleReturns.hasNext())
                        {
                            adaptor.addChild(root_1, stream_ruleReturns.nextTree());
                        }
                        stream_ruleReturns.reset();

                        // org\\antlr\\v4\\parse\\ANTLRParser.g:379:22: ( throwsSpec )?
                        if (stream_throwsSpec.hasNext())
                        {
                            adaptor.addChild(root_1, stream_throwsSpec.nextTree());
                        }
                        stream_throwsSpec.reset();

                        // org\\antlr\\v4\\parse\\ANTLRParser.g:379:34: ( localsSpec )?
                        if (stream_localsSpec.hasNext())
                        {
                            adaptor.addChild(root_1, stream_localsSpec.nextTree());
                        }
                        stream_localsSpec.reset();

                        // org\\antlr\\v4\\parse\\ANTLRParser.g:379:46: ( rulePrequels )?
                        if (stream_rulePrequels.hasNext())
                        {
                            adaptor.addChild(root_1, stream_rulePrequels.nextTree());
                        }
                        stream_rulePrequels.reset();

                        adaptor.addChild(root_1, stream_ruleBlock.nextTree());
                        // org\\antlr\\v4\\parse\\ANTLRParser.g:379:70: ( exceptionGroup )*
                        while (stream_exceptionGroup.hasNext())
                        {
                            adaptor.addChild(root_1, stream_exceptionGroup.nextTree());
                        }
                        stream_exceptionGroup.reset();

                        adaptor.addChild(root_0, root_1);
                    }

                }


                retval.tree = root_0;

            }

            retval.stop = input.LT(-1);

            retval.tree = (GrammarAST)adaptor.rulePostProcessing(root_0);
            adaptor.setTokenBoundaries(retval.tree, retval.start, retval.stop);


            paraphrases.pop();
            GrammarAST options = (GrammarAST)retval.tree.getFirstChildWithType(ANTLRParser.OPTIONS);
            if (options != null)
            {
                Grammar.setNodeOptions(retval.tree, options);
            }

        }
        catch (RecognitionException re)
        {
            reportError(re);
            recover(input, re);
            retval.tree = (GrammarAST)adaptor.errorNode(input, retval.start, input.LT(-1), re);
        }
        finally
        {
            // do for sure before leaving
        }
        return retval;
    }

    private void recover(TokenStream input, RecognitionException re)
    {
        throw new NotImplementedException();
    }

    private void reportError(RecognitionException re)
    {
        throw new NotImplementedException();
    }

    // $ANTLR end "parserRule"


    public class exceptionGroup_return : ParserRuleReturnScope
    {
        public GrammarAST tree;
        //@Override

        public GrammarAST getTree() { return tree; }
    };


    // $ANTLR start "exceptionGroup"
    // org\\antlr\\v4\\parse\\ANTLRParser.g:389:1: exceptionGroup : ( exceptionHandler )* ( finallyClause )? ;
    public ANTLRParser.exceptionGroup_return exceptionGroup()  {
        ANTLRParser.exceptionGroup_return retval = new ANTLRParser.exceptionGroup_return();
        retval.start = input.LT(1);

        GrammarAST root_0 = null;

        ParserRuleReturnScope exceptionHandler76 = null;
        ParserRuleReturnScope finallyClause77 = null;


        try
        {
            // org\\antlr\\v4\\parse\\ANTLRParser.g:390:5: ( ( exceptionHandler )* ( finallyClause )? )
            // org\\antlr\\v4\\parse\\ANTLRParser.g:390:7: ( exceptionHandler )* ( finallyClause )?
            {
                root_0 = (GrammarAST)adaptor.nil();


            // org\\antlr\\v4\\parse\\ANTLRParser.g:390:7: ( exceptionHandler )*
            loop21:
                while (true)
                {
                    int alt21 = 2;
                    int LA21_0 = input.LA(1);
                    if ((LA21_0 == CATCH))
                    {
                        alt21 = 1;
                    }

                    switch (alt21)
                    {
                        case 1:
                            // org\\antlr\\v4\\parse\\ANTLRParser.g:390:7: exceptionHandler
                            {
                                pushFollow(FOLLOW_exceptionHandler_in_exceptionGroup1521);
                                exceptionHandler76 = exceptionHandler();
                                state._fsp--;

                                adaptor.addChild(root_0, exceptionHandler76.getTree());

                            }
                            break;

                        default:
                            goto exit21;
                            //break loop21;
                    }
                }
            exit21:
                // org\\antlr\\v4\\parse\\ANTLRParser.g:390:25: ( finallyClause )?
                int alt22 = 2;
                int LA22_0 = input.LA(1);
                if ((LA22_0 == FINALLY))
                {
                    alt22 = 1;
                }
                switch (alt22)
                {
                    case 1:
                        // org\\antlr\\v4\\parse\\ANTLRParser.g:390:25: finallyClause
                        {
                            pushFollow(FOLLOW_finallyClause_in_exceptionGroup1524);
                            finallyClause77 = finallyClause();
                            state._fsp--;

                            adaptor.addChild(root_0, finallyClause77.getTree());

                        }
                        break;

                }

            }

            retval.stop = input.LT(-1);

            retval.tree = (GrammarAST)adaptor.rulePostProcessing(root_0);
            adaptor.setTokenBoundaries(retval.tree, retval.start, retval.stop);

        }
        catch (RecognitionException re)
        {
            reportError(re);
            recover(input, re);
            retval.tree = (GrammarAST)adaptor.errorNode(input, retval.start, input.LT(-1), re);
        }
        finally
        {
            // do for sure before leaving
        }
        return retval;
    }
    // $ANTLR end "exceptionGroup"


    public class exceptionHandler_return : ParserRuleReturnScope
    {
        public GrammarAST tree;
        //@Override

        public GrammarAST getTree() { return tree; }
    };


    // $ANTLR start "exceptionHandler"
    // org\\antlr\\v4\\parse\\ANTLRParser.g:395:1: exceptionHandler : CATCH ARG_ACTION ACTION -> ^( CATCH ARG_ACTION ACTION ) ;
    public ANTLRParser.exceptionHandler_return exceptionHandler()  {
        ANTLRParser.exceptionHandler_return retval = new ANTLRParser.exceptionHandler_return();
        retval.start = input.LT(1);

        GrammarAST root_0 = null;

        Token CATCH78 = null;
        Token ARG_ACTION79 = null;
        Token ACTION80 = null;

        GrammarAST CATCH78_tree = null;
        GrammarAST ARG_ACTION79_tree = null;
        GrammarAST ACTION80_tree = null;
        RewriteRuleTokenStream stream_ACTION = new RewriteRuleTokenStream(adaptor, "token ACTION");
        RewriteRuleTokenStream stream_CATCH = new RewriteRuleTokenStream(adaptor, "token CATCH");
        RewriteRuleTokenStream stream_ARG_ACTION = new RewriteRuleTokenStream(adaptor, "token ARG_ACTION");

        try
        {
            // org\\antlr\\v4\\parse\\ANTLRParser.g:396:2: ( CATCH ARG_ACTION ACTION -> ^( CATCH ARG_ACTION ACTION ) )
            // org\\antlr\\v4\\parse\\ANTLRParser.g:396:4: CATCH ARG_ACTION ACTION
            {
                CATCH78 = (Token)Match(input, CATCH, FOLLOW_CATCH_in_exceptionHandler1541);
                stream_CATCH.add(CATCH78);

                ARG_ACTION79 = (Token)Match(input, ARG_ACTION, FOLLOW_ARG_ACTION_in_exceptionHandler1543);
                stream_ARG_ACTION.add(ARG_ACTION79);

                ACTION80 = (Token)Match(input, ACTION, FOLLOW_ACTION_in_exceptionHandler1545);
                stream_ACTION.add(ACTION80);


                // AST REWRITE
                // elements: CATCH, ARG_ACTION, ACTION
                // token labels: 
                // rule labels: retval
                // token list labels: 
                // rule list labels: 
                // wildcard labels: 
                retval.tree = root_0;
                RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval != null ? retval.getTree() : null);

                root_0 = (GrammarAST)adaptor.nil();
                // 396:28: -> ^( CATCH ARG_ACTION ACTION )
                {
                    // org\\antlr\\v4\\parse\\ANTLRParser.g:396:31: ^( CATCH ARG_ACTION ACTION )
                    {
                        GrammarAST root_1 = (GrammarAST)adaptor.nil();
                        root_1 = (GrammarAST)adaptor.becomeRoot(stream_CATCH.nextNode(), root_1);
                        adaptor.addChild(root_1, new ActionAST(stream_ARG_ACTION.nextToken()));
                        adaptor.addChild(root_1, new ActionAST(stream_ACTION.nextToken()));
                        adaptor.addChild(root_0, root_1);
                    }

                }


                retval.tree = root_0;

            }

            retval.stop = input.LT(-1);

            retval.tree = (GrammarAST)adaptor.rulePostProcessing(root_0);
            adaptor.setTokenBoundaries(retval.tree, retval.start, retval.stop);

        }
        catch (RecognitionException re)
        {
            reportError(re);
            recover(input, re);
            retval.tree = (GrammarAST)adaptor.errorNode(input, retval.start, input.LT(-1), re);
        }
        finally
        {
            // do for sure before leaving
        }
        return retval;
    }
    // $ANTLR end "exceptionHandler"


    public class finallyClause_return : ParserRuleReturnScope
    {
        public GrammarAST tree;
        //@Override

        public GrammarAST getTree() { return tree; }
    };


    // $ANTLR start "finallyClause"
    // org\\antlr\\v4\\parse\\ANTLRParser.g:399:1: finallyClause : FINALLY ACTION -> ^( FINALLY ACTION ) ;
    public ANTLRParser.finallyClause_return finallyClause()  {
        ANTLRParser.finallyClause_return retval = new ANTLRParser.finallyClause_return();
        retval.start = input.LT(1);

        GrammarAST root_0 = null;

        Token FINALLY81 = null;
        Token ACTION82 = null;

        GrammarAST FINALLY81_tree = null;
        GrammarAST ACTION82_tree = null;
        RewriteRuleTokenStream stream_ACTION = new RewriteRuleTokenStream(adaptor, "token ACTION");
        RewriteRuleTokenStream stream_FINALLY = new RewriteRuleTokenStream(adaptor, "token FINALLY");

        try
        {
            // org\\antlr\\v4\\parse\\ANTLRParser.g:400:2: ( FINALLY ACTION -> ^( FINALLY ACTION ) )
            // org\\antlr\\v4\\parse\\ANTLRParser.g:400:4: FINALLY ACTION
            {
                FINALLY81 = (Token)Match(input, FINALLY, FOLLOW_FINALLY_in_finallyClause1572);
                stream_FINALLY.add(FINALLY81);

                ACTION82 = (Token)Match(input, ACTION, FOLLOW_ACTION_in_finallyClause1574);
                stream_ACTION.add(ACTION82);


                // AST REWRITE
                // elements: FINALLY, ACTION
                // token labels: 
                // rule labels: retval
                // token list labels: 
                // rule list labels: 
                // wildcard labels: 
                retval.tree = root_0;
                RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval != null ? retval.getTree() : null);

                root_0 = (GrammarAST)adaptor.nil();
                // 400:19: -> ^( FINALLY ACTION )
                {
                    // org\\antlr\\v4\\parse\\ANTLRParser.g:400:22: ^( FINALLY ACTION )
                    {
                        GrammarAST root_1 = (GrammarAST)adaptor.nil();
                        root_1 = (GrammarAST)adaptor.becomeRoot(stream_FINALLY.nextNode(), root_1);
                        adaptor.addChild(root_1, new ActionAST(stream_ACTION.nextToken()));
                        adaptor.addChild(root_0, root_1);
                    }

                }


                retval.tree = root_0;

            }

            retval.stop = input.LT(-1);

            retval.tree = (GrammarAST)adaptor.rulePostProcessing(root_0);
            adaptor.setTokenBoundaries(retval.tree, retval.start, retval.stop);

        }
        catch (RecognitionException re)
        {
            reportError(re);
            recover(input, re);
            retval.tree = (GrammarAST)adaptor.errorNode(input, retval.start, input.LT(-1), re);
        }
        finally
        {
            // do for sure before leaving
        }
        return retval;
    }
    // $ANTLR end "finallyClause"


    public class rulePrequels_return : ParserRuleReturnScope
    {
        public GrammarAST tree;
        //@Override

        public GrammarAST getTree() { return tree; }
    };


    // $ANTLR start "rulePrequels"
    // org\\antlr\\v4\\parse\\ANTLRParser.g:403:1: rulePrequels : sync ( rulePrequel sync )* -> ( rulePrequel )* ;
    public ANTLRParser.rulePrequels_return rulePrequels()  {
        ANTLRParser.rulePrequels_return retval = new ANTLRParser.rulePrequels_return();
        retval.start = input.LT(1);

        GrammarAST root_0 = null;

        ParserRuleReturnScope sync83 = null;
        ParserRuleReturnScope rulePrequel84 = null;
        ParserRuleReturnScope sync85 = null;

        RewriteRuleSubtreeStream stream_sync = new RewriteRuleSubtreeStream(adaptor, "rule sync");
        RewriteRuleSubtreeStream stream_rulePrequel = new RewriteRuleSubtreeStream(adaptor, "rule rulePrequel");

        paraphrases.push("matching rule preamble");
        try
        {
            // org\\antlr\\v4\\parse\\ANTLRParser.g:406:2: ( sync ( rulePrequel sync )* -> ( rulePrequel )* )
            // org\\antlr\\v4\\parse\\ANTLRParser.g:406:4: sync ( rulePrequel sync )*
            {
                pushFollow(FOLLOW_sync_in_rulePrequels1606);
                sync83 = sync();
                state._fsp--;

                stream_sync.add(sync83.getTree());
            // org\\antlr\\v4\\parse\\ANTLRParser.g:406:9: ( rulePrequel sync )*
            loop23:
                while (true)
                {
                    int alt23 = 2;
                    int LA23_0 = input.LA(1);
                    if ((LA23_0 == AT || LA23_0 == OPTIONS))
                    {
                        alt23 = 1;
                    }

                    switch (alt23)
                    {
                        case 1:
                            // org\\antlr\\v4\\parse\\ANTLRParser.g:406:10: rulePrequel sync
                            {
                                pushFollow(FOLLOW_rulePrequel_in_rulePrequels1609);
                                rulePrequel84 = rulePrequel();
                                state._fsp--;

                                stream_rulePrequel.add(rulePrequel84.getTree());
                                pushFollow(FOLLOW_sync_in_rulePrequels1611);
                                sync85 = sync();
                                state._fsp--;

                                stream_sync.add(sync85.getTree());
                            }
                            break;

                        default:
                            goto exit23;
                            //break loop23;
                    }
                }
            exit23:

                // AST REWRITE
                // elements: rulePrequel
                // token labels: 
                // rule labels: retval
                // token list labels: 
                // rule list labels: 
                // wildcard labels: 
                retval.tree = root_0;
                RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval != null ? retval.getTree() : null);

                root_0 = (GrammarAST)adaptor.nil();
                // 406:29: -> ( rulePrequel )*
                {
                    // org\\antlr\\v4\\parse\\ANTLRParser.g:406:32: ( rulePrequel )*
                    while (stream_rulePrequel.hasNext())
                    {
                        adaptor.addChild(root_0, stream_rulePrequel.nextTree());
                    }
                    stream_rulePrequel.reset();

                }


                retval.tree = root_0;

            }

            retval.stop = input.LT(-1);

            retval.tree = (GrammarAST)adaptor.rulePostProcessing(root_0);
            adaptor.setTokenBoundaries(retval.tree, retval.start, retval.stop);

            paraphrases.pop();
        }
        catch (RecognitionException re)
        {
            reportError(re);
            recover(input, re);
            retval.tree = (GrammarAST)adaptor.errorNode(input, retval.start, input.LT(-1), re);
        }
        finally
        {
            // do for sure before leaving
        }
        return retval;
    }
    // $ANTLR end "rulePrequels"


    public class rulePrequel_return : ParserRuleReturnScope
    {
        public GrammarAST tree;
        //@Override

        public GrammarAST getTree() { return tree; }
    };


    // $ANTLR start "rulePrequel"
    // org\\antlr\\v4\\parse\\ANTLRParser.g:412:1: rulePrequel : ( optionsSpec | ruleAction );
    public ANTLRParser.rulePrequel_return rulePrequel()  {
        ANTLRParser.rulePrequel_return retval = new ANTLRParser.rulePrequel_return();
        retval.start = input.LT(1);

        GrammarAST root_0 = null;

        ParserRuleReturnScope optionsSpec86 = null;
        ParserRuleReturnScope ruleAction87 = null;


        try
        {
            // org\\antlr\\v4\\parse\\ANTLRParser.g:413:5: ( optionsSpec | ruleAction )
            int alt24 = 2;
            int LA24_0 = input.LA(1);
            if ((LA24_0 == OPTIONS))
            {
                alt24 = 1;
            }
            else if ((LA24_0 == AT))
            {
                alt24 = 2;
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
                    // org\\antlr\\v4\\parse\\ANTLRParser.g:413:7: optionsSpec
                    {
                        root_0 = (GrammarAST)adaptor.nil();


                        pushFollow(FOLLOW_optionsSpec_in_rulePrequel1635);
                        optionsSpec86 = optionsSpec();
                        state._fsp--;

                        adaptor.addChild(root_0, optionsSpec86.getTree());

                    }
                    break;
                case 2:
                    // org\\antlr\\v4\\parse\\ANTLRParser.g:414:7: ruleAction
                    {
                        root_0 = (GrammarAST)adaptor.nil();


                        pushFollow(FOLLOW_ruleAction_in_rulePrequel1643);
                        ruleAction87 = ruleAction();
                        state._fsp--;

                        adaptor.addChild(root_0, ruleAction87.getTree());

                    }
                    break;

            }
            retval.stop = input.LT(-1);

            retval.tree = (GrammarAST)adaptor.rulePostProcessing(root_0);
            adaptor.setTokenBoundaries(retval.tree, retval.start, retval.stop);

        }
        catch (RecognitionException re)
        {
            reportError(re);
            recover(input, re);
            retval.tree = (GrammarAST)adaptor.errorNode(input, retval.start, input.LT(-1), re);
        }
        finally
        {
            // do for sure before leaving
        }
        return retval;
    }
    // $ANTLR end "rulePrequel"


    public class ruleReturns_return : ParserRuleReturnScope
    {
        public GrammarAST tree;
        //@Override

        public GrammarAST getTree() { return tree; }
    };


    // $ANTLR start "ruleReturns"
    // org\\antlr\\v4\\parse\\ANTLRParser.g:423:1: ruleReturns : RETURNS ^ ARG_ACTION ;
    public ANTLRParser.ruleReturns_return ruleReturns()  {
        ANTLRParser.ruleReturns_return retval = new ANTLRParser.ruleReturns_return();
        retval.start = input.LT(1);

        GrammarAST root_0 = null;

        Token RETURNS88 = null;
        Token ARG_ACTION89 = null;

        GrammarAST RETURNS88_tree = null;
        GrammarAST ARG_ACTION89_tree = null;

        try
        {
            // org\\antlr\\v4\\parse\\ANTLRParser.g:424:2: ( RETURNS ^ ARG_ACTION )
            // org\\antlr\\v4\\parse\\ANTLRParser.g:424:4: RETURNS ^ ARG_ACTION
            {
                root_0 = (GrammarAST)adaptor.nil();


                RETURNS88 = (Token)Match(input, RETURNS, FOLLOW_RETURNS_in_ruleReturns1663);
                RETURNS88_tree = (GrammarAST)adaptor.create(RETURNS88);
                root_0 = (GrammarAST)adaptor.becomeRoot(RETURNS88_tree, root_0);

                ARG_ACTION89 = (Token)Match(input, ARG_ACTION, FOLLOW_ARG_ACTION_in_ruleReturns1666);
                ARG_ACTION89_tree = new ActionAST(ARG_ACTION89);
                adaptor.addChild(root_0, ARG_ACTION89_tree);

            }

            retval.stop = input.LT(-1);

            retval.tree = (GrammarAST)adaptor.rulePostProcessing(root_0);
            adaptor.setTokenBoundaries(retval.tree, retval.start, retval.stop);

        }
        catch (RecognitionException re)
        {
            reportError(re);
            recover(input, re);
            retval.tree = (GrammarAST)adaptor.errorNode(input, retval.start, input.LT(-1), re);
        }
        finally
        {
            // do for sure before leaving
        }
        return retval;
    }
    // $ANTLR end "ruleReturns"


    public class throwsSpec_return : ParserRuleReturnScope
    {
        public GrammarAST tree;
        //@Override

        public GrammarAST getTree() { return tree; }
    };


    // $ANTLR start "throwsSpec"
    // org\\antlr\\v4\\parse\\ANTLRParser.g:438:1: throwsSpec : THROWS qid ( COMMA qid )* -> ^( THROWS ( qid )+ ) ;
    public ANTLRParser.throwsSpec_return throwsSpec()  {
        ANTLRParser.throwsSpec_return retval = new ANTLRParser.throwsSpec_return();
        retval.start = input.LT(1);

        GrammarAST root_0 = null;

        Token THROWS90 = null;
        Token COMMA92 = null;
        ParserRuleReturnScope qid91 = null;
        ParserRuleReturnScope qid93 = null;

        GrammarAST THROWS90_tree = null;
        GrammarAST COMMA92_tree = null;
        RewriteRuleTokenStream stream_COMMA = new RewriteRuleTokenStream(adaptor, "token COMMA");
        RewriteRuleTokenStream stream_THROWS = new RewriteRuleTokenStream(adaptor, "token THROWS");
        RewriteRuleSubtreeStream stream_qid = new RewriteRuleSubtreeStream(adaptor, "rule qid");

        try
        {
            // org\\antlr\\v4\\parse\\ANTLRParser.g:439:5: ( THROWS qid ( COMMA qid )* -> ^( THROWS ( qid )+ ) )
            // org\\antlr\\v4\\parse\\ANTLRParser.g:439:7: THROWS qid ( COMMA qid )*
            {
                THROWS90 = (Token)Match(input, THROWS, FOLLOW_THROWS_in_throwsSpec1694);
                stream_THROWS.add(THROWS90);

                pushFollow(FOLLOW_qid_in_throwsSpec1696);
                qid91 = qid();
                state._fsp--;

                stream_qid.add(qid91.getTree());
            // org\\antlr\\v4\\parse\\ANTLRParser.g:439:18: ( COMMA qid )*
            loop25:
                while (true)
                {
                    int alt25 = 2;
                    int LA25_0 = input.LA(1);
                    if ((LA25_0 == COMMA))
                    {
                        alt25 = 1;
                    }

                    switch (alt25)
                    {
                        case 1:
                            // org\\antlr\\v4\\parse\\ANTLRParser.g:439:19: COMMA qid
                            {
                                COMMA92 = (Token)Match(input, COMMA, FOLLOW_COMMA_in_throwsSpec1699);
                                stream_COMMA.add(COMMA92);

                                pushFollow(FOLLOW_qid_in_throwsSpec1701);
                                qid93 = qid();
                                state._fsp--;

                                stream_qid.add(qid93.getTree());
                            }
                            break;

                        default:
                            goto exit25;
                            //break loop25;
                    }
                }
            exit25:

                // AST REWRITE
                // elements: qid, THROWS
                // token labels: 
                // rule labels: retval
                // token list labels: 
                // rule list labels: 
                // wildcard labels: 
                retval.tree = root_0;
                RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval != null ? retval.getTree() : null);

                root_0 = (GrammarAST)adaptor.nil();
                // 439:31: -> ^( THROWS ( qid )+ )
                {
                    // org\\antlr\\v4\\parse\\ANTLRParser.g:439:34: ^( THROWS ( qid )+ )
                    {
                        GrammarAST root_1 = (GrammarAST)adaptor.nil();
                        root_1 = (GrammarAST)adaptor.becomeRoot(stream_THROWS.nextNode(), root_1);
                        if (!(stream_qid.hasNext()))
                        {
                            throw new RewriteEarlyExitException();
                        }
                        while (stream_qid.hasNext())
                        {
                            adaptor.addChild(root_1, stream_qid.nextTree());
                        }
                        stream_qid.reset();

                        adaptor.addChild(root_0, root_1);
                    }

                }


                retval.tree = root_0;

            }

            retval.stop = input.LT(-1);

            retval.tree = (GrammarAST)adaptor.rulePostProcessing(root_0);
            adaptor.setTokenBoundaries(retval.tree, retval.start, retval.stop);

        }
        catch (RecognitionException re)
        {
            reportError(re);
            recover(input, re);
            retval.tree = (GrammarAST)adaptor.errorNode(input, retval.start, input.LT(-1), re);
        }
        finally
        {
            // do for sure before leaving
        }
        return retval;
    }
    // $ANTLR end "throwsSpec"


    public class localsSpec_return : ParserRuleReturnScope
    {
        public GrammarAST tree;
        //@Override

        public GrammarAST getTree() { return tree; }
    };


    // $ANTLR start "localsSpec"
    // org\\antlr\\v4\\parse\\ANTLRParser.g:443:1: localsSpec : LOCALS ^ ARG_ACTION ;
    public ANTLRParser.localsSpec_return localsSpec()  {
        ANTLRParser.localsSpec_return retval = new ANTLRParser.localsSpec_return();
        retval.start = input.LT(1);

        GrammarAST root_0 = null;

        Token LOCALS94 = null;
        Token ARG_ACTION95 = null;

        GrammarAST LOCALS94_tree = null;
        GrammarAST ARG_ACTION95_tree = null;

        try
        {
            // org\\antlr\\v4\\parse\\ANTLRParser.g:443:12: ( LOCALS ^ ARG_ACTION )
            // org\\antlr\\v4\\parse\\ANTLRParser.g:443:14: LOCALS ^ ARG_ACTION
            {
                root_0 = (GrammarAST)adaptor.nil();


                LOCALS94 = (Token)Match(input, LOCALS, FOLLOW_LOCALS_in_localsSpec1726);
                LOCALS94_tree = (GrammarAST)adaptor.create(LOCALS94);
                root_0 = (GrammarAST)adaptor.becomeRoot(LOCALS94_tree, root_0);

                ARG_ACTION95 = (Token)Match(input, ARG_ACTION, FOLLOW_ARG_ACTION_in_localsSpec1729);
                ARG_ACTION95_tree = new ActionAST(ARG_ACTION95);
                adaptor.addChild(root_0, ARG_ACTION95_tree);

            }

            retval.stop = input.LT(-1);

            retval.tree = (GrammarAST)adaptor.rulePostProcessing(root_0);
            adaptor.setTokenBoundaries(retval.tree, retval.start, retval.stop);

        }
        catch (RecognitionException re)
        {
            reportError(re);
            recover(input, re);
            retval.tree = (GrammarAST)adaptor.errorNode(input, retval.start, input.LT(-1), re);
        }
        finally
        {
            // do for sure before leaving
        }
        return retval;
    }
    // $ANTLR end "localsSpec"


    public class ruleAction_return : ParserRuleReturnScope
    {
        public GrammarAST tree;
        //@Override

        public GrammarAST getTree() { return tree; }
    };


    // $ANTLR start "ruleAction"
    // org\\antlr\\v4\\parse\\ANTLRParser.g:454:1: ruleAction : AT id ACTION -> ^( AT id ACTION ) ;
    public ANTLRParser.ruleAction_return ruleAction()  {
        ANTLRParser.ruleAction_return retval = new ANTLRParser.ruleAction_return();
        retval.start = input.LT(1);

        GrammarAST root_0 = null;

        Token AT96 = null;
        Token ACTION98 = null;
        ParserRuleReturnScope id97 = null;

        GrammarAST AT96_tree = null;
        GrammarAST ACTION98_tree = null;
        RewriteRuleTokenStream stream_AT = new RewriteRuleTokenStream(adaptor, "token AT");
        RewriteRuleTokenStream stream_ACTION = new RewriteRuleTokenStream(adaptor, "token ACTION");
        RewriteRuleSubtreeStream stream_id = new RewriteRuleSubtreeStream(adaptor, "rule id");

        try
        {
            // org\\antlr\\v4\\parse\\ANTLRParser.g:455:2: ( AT id ACTION -> ^( AT id ACTION ) )
            // org\\antlr\\v4\\parse\\ANTLRParser.g:455:4: AT id ACTION
            {
                AT96 = (Token)Match(input, AT, FOLLOW_AT_in_ruleAction1752);
                stream_AT.add(AT96);

                pushFollow(FOLLOW_id_in_ruleAction1754);
                id97 = id();
                state._fsp--;

                stream_id.add(id97.getTree());
                ACTION98 = (Token)Match(input, ACTION, FOLLOW_ACTION_in_ruleAction1756);
                stream_ACTION.add(ACTION98);


                // AST REWRITE
                // elements: AT, id, ACTION
                // token labels: 
                // rule labels: retval
                // token list labels: 
                // rule list labels: 
                // wildcard labels: 
                retval.tree = root_0;
                RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval != null ? retval.getTree() : null);

                root_0 = (GrammarAST)adaptor.nil();
                // 455:17: -> ^( AT id ACTION )
                {
                    // org\\antlr\\v4\\parse\\ANTLRParser.g:455:20: ^( AT id ACTION )
                    {
                        GrammarAST root_1 = (GrammarAST)adaptor.nil();
                        root_1 = (GrammarAST)adaptor.becomeRoot(stream_AT.nextNode(), root_1);
                        adaptor.addChild(root_1, stream_id.nextTree());
                        adaptor.addChild(root_1, new ActionAST(stream_ACTION.nextToken()));
                        adaptor.addChild(root_0, root_1);
                    }

                }


                retval.tree = root_0;

            }

            retval.stop = input.LT(-1);

            retval.tree = (GrammarAST)adaptor.rulePostProcessing(root_0);
            adaptor.setTokenBoundaries(retval.tree, retval.start, retval.stop);

        }
        catch (RecognitionException re)
        {
            reportError(re);
            recover(input, re);
            retval.tree = (GrammarAST)adaptor.errorNode(input, retval.start, input.LT(-1), re);
        }
        finally
        {
            // do for sure before leaving
        }
        return retval;
    }
    // $ANTLR end "ruleAction"


    public class ruleBlock_return : ParserRuleReturnScope
    {
        public GrammarAST tree;
        //@Override

        public GrammarAST getTree() { return tree; }
    };


    // $ANTLR start "ruleBlock"
    // org\\antlr\\v4\\parse\\ANTLRParser.g:464:1: ruleBlock : ruleAltList -> ^( BLOCK[colon,\"BLOCK\"] ruleAltList ) ;
    public ANTLRParser.ruleBlock_return ruleBlock()  {
        ANTLRParser.ruleBlock_return retval = new ANTLRParser.ruleBlock_return();
        retval.start = input.LT(1);

        GrammarAST root_0 = null;

        ParserRuleReturnScope ruleAltList99 = null;

        RewriteRuleSubtreeStream stream_ruleAltList = new RewriteRuleSubtreeStream(adaptor, "rule ruleAltList");

        Token colon = input.LT(-1);
        try
        {
            // org\\antlr\\v4\\parse\\ANTLRParser.g:466:5: ( ruleAltList -> ^( BLOCK[colon,\"BLOCK\"] ruleAltList ) )
            // org\\antlr\\v4\\parse\\ANTLRParser.g:466:7: ruleAltList
            {
                pushFollow(FOLLOW_ruleAltList_in_ruleBlock1794);
                ruleAltList99 = ruleAltList();
                state._fsp--;

                stream_ruleAltList.add(ruleAltList99.getTree());

                // AST REWRITE
                // elements: ruleAltList
                // token labels: 
                // rule labels: retval
                // token list labels: 
                // rule list labels: 
                // wildcard labels: 
                retval.tree = root_0;
                RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval != null ? retval.getTree() : null);

                root_0 = (GrammarAST)adaptor.nil();
                // 466:19: -> ^( BLOCK[colon,\"BLOCK\"] ruleAltList )
                {
                    // org\\antlr\\v4\\parse\\ANTLRParser.g:466:22: ^( BLOCK[colon,\"BLOCK\"] ruleAltList )
                    {
                        GrammarAST root_1 = (GrammarAST)adaptor.nil();
                        root_1 = (GrammarAST)adaptor.becomeRoot(new BlockAST(BLOCK, colon, "BLOCK"), root_1);
                        adaptor.addChild(root_1, stream_ruleAltList.nextTree());
                        adaptor.addChild(root_0, root_1);
                    }

                }


                retval.tree = root_0;

            }

            retval.stop = input.LT(-1);

            retval.tree = (GrammarAST)adaptor.rulePostProcessing(root_0);
            adaptor.setTokenBoundaries(retval.tree, retval.start, retval.stop);

        }
        catch (ResyncToEndOfRuleBlock e)
        {

            // just resyncing; ignore error
            retval.tree = (GrammarAST)adaptor.errorNode(input, retval.start, input.LT(-1), null);

        }

        finally
        {
            // do for sure before leaving
        }
        return retval;
    }
    // $ANTLR end "ruleBlock"


    public class ruleAltList_return : ParserRuleReturnScope
    {
        public GrammarAST tree;
        //@Override

        public GrammarAST getTree() { return tree; }
    };


    // $ANTLR start "ruleAltList"
    // org\\antlr\\v4\\parse\\ANTLRParser.g:473:1: ruleAltList : labeledAlt ( OR labeledAlt )* -> ( labeledAlt )+ ;
    public ANTLRParser.ruleAltList_return ruleAltList()  {
        ANTLRParser.ruleAltList_return retval = new ANTLRParser.ruleAltList_return();
        retval.start = input.LT(1);

        GrammarAST root_0 = null;

        Token OR101 = null;
        ParserRuleReturnScope labeledAlt100 = null;
        ParserRuleReturnScope labeledAlt102 = null;

        GrammarAST OR101_tree = null;
        RewriteRuleTokenStream stream_OR = new RewriteRuleTokenStream(adaptor, "token OR");
        RewriteRuleSubtreeStream stream_labeledAlt = new RewriteRuleSubtreeStream(adaptor, "rule labeledAlt");

        try
        {
            // org\\antlr\\v4\\parse\\ANTLRParser.g:474:2: ( labeledAlt ( OR labeledAlt )* -> ( labeledAlt )+ )
            // org\\antlr\\v4\\parse\\ANTLRParser.g:474:4: labeledAlt ( OR labeledAlt )*
            {
                pushFollow(FOLLOW_labeledAlt_in_ruleAltList1830);
                labeledAlt100 = labeledAlt();
                state._fsp--;

                stream_labeledAlt.add(labeledAlt100.getTree());
            // org\\antlr\\v4\\parse\\ANTLRParser.g:474:15: ( OR labeledAlt )*
            loop26:
                while (true)
                {
                    int alt26 = 2;
                    int LA26_0 = input.LA(1);
                    if ((LA26_0 == OR))
                    {
                        alt26 = 1;
                    }

                    switch (alt26)
                    {
                        case 1:
                            // org\\antlr\\v4\\parse\\ANTLRParser.g:474:16: OR labeledAlt
                            {
                                OR101 = (Token)Match(input, OR, FOLLOW_OR_in_ruleAltList1833);
                                stream_OR.add(OR101);

                                pushFollow(FOLLOW_labeledAlt_in_ruleAltList1835);
                                labeledAlt102 = labeledAlt();
                                state._fsp--;

                                stream_labeledAlt.add(labeledAlt102.getTree());
                            }
                            break;

                        default:
                            //break loop26;
                            goto exit26;
                    }
                }
            exit26:

                // AST REWRITE
                // elements: labeledAlt
                // token labels: 
                // rule labels: retval
                // token list labels: 
                // rule list labels: 
                // wildcard labels: 
                retval.tree = root_0;
                RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval != null ? retval.getTree() : null);

                root_0 = (GrammarAST)adaptor.nil();
                // 474:32: -> ( labeledAlt )+
                {
                    if (!(stream_labeledAlt.hasNext()))
                    {
                        throw new RewriteEarlyExitException();
                    }
                    while (stream_labeledAlt.hasNext())
                    {
                        adaptor.addChild(root_0, stream_labeledAlt.nextTree());
                    }
                    stream_labeledAlt.reset();

                }


                retval.tree = root_0;

            }

            retval.stop = input.LT(-1);

            retval.tree = (GrammarAST)adaptor.rulePostProcessing(root_0);
            adaptor.setTokenBoundaries(retval.tree, retval.start, retval.stop);

        }
        catch (RecognitionException re)
        {
            reportError(re);
            recover(input, re);
            retval.tree = (GrammarAST)adaptor.errorNode(input, retval.start, input.LT(-1), re);
        }
        finally
        {
            // do for sure before leaving
        }
        return retval;
    }

    private void pushFollow(BitSet fOLLOW_labeledAlt_in_ruleAltList1830)
    {
        throw new NotImplementedException();
    }

    // $ANTLR end "ruleAltList"


    public class labeledAlt_return : ParserRuleReturnScope
    {
        public GrammarAST tree;
        //@Override

        public GrammarAST getTree() { return tree; }
    };


    // $ANTLR start "labeledAlt"
    // org\\antlr\\v4\\parse\\ANTLRParser.g:477:1: labeledAlt : alternative ( POUND ! id !)? ;
    public ANTLRParser.labeledAlt_return labeledAlt()  {
        ANTLRParser.labeledAlt_return retval = new ANTLRParser.labeledAlt_return();
        retval.start = input.LT(1);

        GrammarAST root_0 = null;

        Token POUND104 = null;
        ParserRuleReturnScope alternative103 = null;
        ParserRuleReturnScope id105 = null;

        GrammarAST POUND104_tree = null;

        try
        {
            // org\\antlr\\v4\\parse\\ANTLRParser.g:478:2: ( alternative ( POUND ! id !)? )
            // org\\antlr\\v4\\parse\\ANTLRParser.g:478:4: alternative ( POUND ! id !)?
            {
                root_0 = (GrammarAST)adaptor.nil();


                pushFollow(FOLLOW_alternative_in_labeledAlt1853);
                alternative103 = alternative();
                state._fsp--;

                adaptor.addChild(root_0, alternative103.getTree());

                // org\\antlr\\v4\\parse\\ANTLRParser.g:479:3: ( POUND ! id !)?
                int alt27 = 2;
                int LA27_0 = input.LA(1);
                if ((LA27_0 == POUND))
                {
                    alt27 = 1;
                }
                switch (alt27)
                {
                    case 1:
                        // org\\antlr\\v4\\parse\\ANTLRParser.g:479:5: POUND ! id !
                        {
                            POUND104 = (Token)Match(input, POUND, FOLLOW_POUND_in_labeledAlt1859);
                            pushFollow(FOLLOW_id_in_labeledAlt1862);
                            id105 = id();
                            state._fsp--;

                            ((AltAST)(alternative103 != null ? ((GrammarAST)alternative103.getTree()) : null)).altLabel = (id105 != null ? ((GrammarAST)id105.getTree()) : null);
                        }
                        break;

                }

            }

            retval.stop = input.LT(-1);

            retval.tree = (GrammarAST)adaptor.rulePostProcessing(root_0);
            adaptor.setTokenBoundaries(retval.tree, retval.start, retval.stop);

        }
        catch (RecognitionException re)
        {
            reportError(re);
            recover(input, re);
            retval.tree = (GrammarAST)adaptor.errorNode(input, retval.start, input.LT(-1), re);
        }
        finally
        {
            // do for sure before leaving
        }
        return retval;
    }
    // $ANTLR end "labeledAlt"


    public class lexerRule_return : ParserRuleReturnScope
    {
        public GrammarAST tree;
        //@Override

        public GrammarAST getTree() { return tree; }
    };


    // $ANTLR start "lexerRule"
    // org\\antlr\\v4\\parse\\ANTLRParser.g:483:1: lexerRule : ( FRAGMENT )? TOKEN_REF ( optionsSpec )? COLON lexerRuleBlock SEMI -> ^( RULE TOKEN_REF ( ^( RULEMODIFIERS FRAGMENT ) )? ( optionsSpec )? lexerRuleBlock ) ;
    public ANTLRParser.lexerRule_return lexerRule()  {
        ANTLRParser.lexerRule_return retval = new ANTLRParser.lexerRule_return();
        retval.start = input.LT(1);

        GrammarAST root_0 = null;

        Token FRAGMENT106 = null;
        Token TOKEN_REF107 = null;
        Token COLON109 = null;
        Token SEMI111 = null;
        ParserRuleReturnScope optionsSpec108 = null;
        ParserRuleReturnScope lexerRuleBlock110 = null;

        GrammarAST FRAGMENT106_tree = null;
        GrammarAST TOKEN_REF107_tree = null;
        GrammarAST COLON109_tree = null;
        GrammarAST SEMI111_tree = null;
        RewriteRuleTokenStream stream_TOKEN_REF = new RewriteRuleTokenStream(adaptor, "token TOKEN_REF");
        RewriteRuleTokenStream stream_FRAGMENT = new RewriteRuleTokenStream(adaptor, "token FRAGMENT");
        RewriteRuleTokenStream stream_SEMI = new RewriteRuleTokenStream(adaptor, "token SEMI");
        RewriteRuleTokenStream stream_COLON = new RewriteRuleTokenStream(adaptor, "token COLON");
        RewriteRuleSubtreeStream stream_optionsSpec = new RewriteRuleSubtreeStream(adaptor, "rule optionsSpec");
        RewriteRuleSubtreeStream stream_lexerRuleBlock = new RewriteRuleSubtreeStream(adaptor, "rule lexerRuleBlock");

        paraphrases.push("matching a lexer rule");
        try
        {
            // org\\antlr\\v4\\parse\\ANTLRParser.g:488:5: ( ( FRAGMENT )? TOKEN_REF ( optionsSpec )? COLON lexerRuleBlock SEMI -> ^( RULE TOKEN_REF ( ^( RULEMODIFIERS FRAGMENT ) )? ( optionsSpec )? lexerRuleBlock ) )
            // org\\antlr\\v4\\parse\\ANTLRParser.g:488:7: ( FRAGMENT )? TOKEN_REF ( optionsSpec )? COLON lexerRuleBlock SEMI
            {
                // org\\antlr\\v4\\parse\\ANTLRParser.g:488:7: ( FRAGMENT )?
                int alt28 = 2;
                int LA28_0 = input.LA(1);
                if ((LA28_0 == FRAGMENT))
                {
                    alt28 = 1;
                }
                switch (alt28)
                {
                    case 1:
                        // org\\antlr\\v4\\parse\\ANTLRParser.g:488:7: FRAGMENT
                        {
                            FRAGMENT106 = (Token)Match(input, FRAGMENT, FOLLOW_FRAGMENT_in_lexerRule1894);
                            stream_FRAGMENT.add(FRAGMENT106);

                        }
                        break;

                }

                TOKEN_REF107 = (Token)Match(input, TOKEN_REF, FOLLOW_TOKEN_REF_in_lexerRule1900);
                stream_TOKEN_REF.add(TOKEN_REF107);

                // org\\antlr\\v4\\parse\\ANTLRParser.g:491:4: ( optionsSpec )?
                int alt29 = 2;
                int LA29_0 = input.LA(1);
                if ((LA29_0 == OPTIONS))
                {
                    alt29 = 1;
                }
                switch (alt29)
                {
                    case 1:
                        // org\\antlr\\v4\\parse\\ANTLRParser.g:491:4: optionsSpec
                        {
                            pushFollow(FOLLOW_optionsSpec_in_lexerRule1906);
                            optionsSpec108 = optionsSpec();
                            state._fsp--;

                            stream_optionsSpec.add(optionsSpec108.getTree());
                        }
                        break;

                }

                COLON109 = (Token)Match(input, COLON, FOLLOW_COLON_in_lexerRule1913);
                stream_COLON.add(COLON109);

                pushFollow(FOLLOW_lexerRuleBlock_in_lexerRule1915);
                lexerRuleBlock110 = lexerRuleBlock();
                state._fsp--;

                stream_lexerRuleBlock.add(lexerRuleBlock110.getTree());
                SEMI111 = (Token)Match(input, SEMI, FOLLOW_SEMI_in_lexerRule1917);
                stream_SEMI.add(SEMI111);


                // AST REWRITE
                // elements: TOKEN_REF, optionsSpec, lexerRuleBlock, FRAGMENT
                // token labels: 
                // rule labels: retval
                // token list labels: 
                // rule list labels: 
                // wildcard labels: 
                retval.tree = root_0;
                RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval != null ? retval.getTree() : null);

                root_0 = (GrammarAST)adaptor.nil();
                // 494:7: -> ^( RULE TOKEN_REF ( ^( RULEMODIFIERS FRAGMENT ) )? ( optionsSpec )? lexerRuleBlock )
                {
                    // org\\antlr\\v4\\parse\\ANTLRParser.g:494:10: ^( RULE TOKEN_REF ( ^( RULEMODIFIERS FRAGMENT ) )? ( optionsSpec )? lexerRuleBlock )
                    {
                        GrammarAST root_1 = (GrammarAST)adaptor.nil();
                        root_1 = (GrammarAST)adaptor.becomeRoot(new RuleAST(RULE), root_1);
                        adaptor.addChild(root_1, stream_TOKEN_REF.nextNode());
                        // org\\antlr\\v4\\parse\\ANTLRParser.g:495:9: ( ^( RULEMODIFIERS FRAGMENT ) )?
                        if (stream_FRAGMENT.hasNext())
                        {
                            // org\\antlr\\v4\\parse\\ANTLRParser.g:495:9: ^( RULEMODIFIERS FRAGMENT )
                            {
                                GrammarAST root_2 = (GrammarAST)adaptor.nil();
                                root_2 = (GrammarAST)adaptor.becomeRoot((GrammarAST)adaptor.create(RULEMODIFIERS, "RULEMODIFIERS"), root_2);
                                adaptor.addChild(root_2, stream_FRAGMENT.nextNode());
                                adaptor.addChild(root_1, root_2);
                            }

                        }
                        stream_FRAGMENT.reset();

                        // org\\antlr\\v4\\parse\\ANTLRParser.g:495:36: ( optionsSpec )?
                        if (stream_optionsSpec.hasNext())
                        {
                            adaptor.addChild(root_1, stream_optionsSpec.nextTree());
                        }
                        stream_optionsSpec.reset();

                        adaptor.addChild(root_1, stream_lexerRuleBlock.nextTree());
                        adaptor.addChild(root_0, root_1);
                    }

                }


                retval.tree = root_0;

            }

            retval.stop = input.LT(-1);

            retval.tree = (GrammarAST)adaptor.rulePostProcessing(root_0);
            adaptor.setTokenBoundaries(retval.tree, retval.start, retval.stop);


            paraphrases.pop();

        }
        catch (RecognitionException re)
        {
            reportError(re);
            recover(input, re);
            retval.tree = (GrammarAST)adaptor.errorNode(input, retval.start, input.LT(-1), re);
        }
        finally
        {
            // do for sure before leaving
        }
        return retval;
    }
    // $ANTLR end "lexerRule"


    public class lexerRuleBlock_return : ParserRuleReturnScope
    {
        public GrammarAST tree;
        //@Override

        public GrammarAST getTree() { return tree; }
    };


    // $ANTLR start "lexerRuleBlock"
    // org\\antlr\\v4\\parse\\ANTLRParser.g:499:1: lexerRuleBlock : lexerAltList -> ^( BLOCK[colon,\"BLOCK\"] lexerAltList ) ;
    public ANTLRParser.lexerRuleBlock_return lexerRuleBlock()  {
        ANTLRParser.lexerRuleBlock_return retval = new ANTLRParser.lexerRuleBlock_return();
        retval.start = input.LT(1);

        GrammarAST root_0 = null;

        ParserRuleReturnScope lexerAltList112 = null;

        RewriteRuleSubtreeStream stream_lexerAltList = new RewriteRuleSubtreeStream(adaptor, "rule lexerAltList");

        Token colon = input.LT(-1);
        try
        {
            // org\\antlr\\v4\\parse\\ANTLRParser.g:501:5: ( lexerAltList -> ^( BLOCK[colon,\"BLOCK\"] lexerAltList ) )
            // org\\antlr\\v4\\parse\\ANTLRParser.g:501:7: lexerAltList
            {
                pushFollow(FOLLOW_lexerAltList_in_lexerRuleBlock1984);
                lexerAltList112 = lexerAltList();
                state._fsp--;

                stream_lexerAltList.add(lexerAltList112.getTree());

                // AST REWRITE
                // elements: lexerAltList
                // token labels: 
                // rule labels: retval
                // token list labels: 
                // rule list labels: 
                // wildcard labels: 
                retval.tree = root_0;
                RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval != null ? retval.getTree() : null);

                root_0 = (GrammarAST)adaptor.nil();
                // 501:20: -> ^( BLOCK[colon,\"BLOCK\"] lexerAltList )
                {
                    // org\\antlr\\v4\\parse\\ANTLRParser.g:501:23: ^( BLOCK[colon,\"BLOCK\"] lexerAltList )
                    {
                        GrammarAST root_1 = (GrammarAST)adaptor.nil();
                        root_1 = (GrammarAST)adaptor.becomeRoot(new BlockAST(BLOCK, colon, "BLOCK"), root_1);
                        adaptor.addChild(root_1, stream_lexerAltList.nextTree());
                        adaptor.addChild(root_0, root_1);
                    }

                }


                retval.tree = root_0;

            }

            retval.stop = input.LT(-1);

            retval.tree = (GrammarAST)adaptor.rulePostProcessing(root_0);
            adaptor.setTokenBoundaries(retval.tree, retval.start, retval.stop);

        }
        catch (ResyncToEndOfRuleBlock e)
        {

            // just resyncing; ignore error
            retval.tree = (GrammarAST)adaptor.errorNode(input, retval.start, input.LT(-1), null);

        }

        finally
        {
            // do for sure before leaving
        }
        return retval;
    }
    // $ANTLR end "lexerRuleBlock"


    public class lexerAltList_return : ParserRuleReturnScope
    {
        public GrammarAST tree;
        //@Override

        public GrammarAST getTree() { return tree; }
    };


    // $ANTLR start "lexerAltList"
    // org\\antlr\\v4\\parse\\ANTLRParser.g:508:1: lexerAltList : lexerAlt ( OR lexerAlt )* -> ( lexerAlt )+ ;
    public ANTLRParser.lexerAltList_return lexerAltList()  {
        ANTLRParser.lexerAltList_return retval = new ANTLRParser.lexerAltList_return();
        retval.start = input.LT(1);

        GrammarAST root_0 = null;

        Token OR114 = null;
        ParserRuleReturnScope lexerAlt113 = null;
        ParserRuleReturnScope lexerAlt115 = null;

        GrammarAST OR114_tree = null;
        RewriteRuleTokenStream stream_OR = new RewriteRuleTokenStream(adaptor, "token OR");
        RewriteRuleSubtreeStream stream_lexerAlt = new RewriteRuleSubtreeStream(adaptor, "rule lexerAlt");

        try
        {
            // org\\antlr\\v4\\parse\\ANTLRParser.g:509:2: ( lexerAlt ( OR lexerAlt )* -> ( lexerAlt )+ )
            // org\\antlr\\v4\\parse\\ANTLRParser.g:509:4: lexerAlt ( OR lexerAlt )*
            {
                pushFollow(FOLLOW_lexerAlt_in_lexerAltList2020);
                lexerAlt113 = lexerAlt();
                state._fsp--;

                stream_lexerAlt.add(lexerAlt113.getTree());
            // org\\antlr\\v4\\parse\\ANTLRParser.g:509:13: ( OR lexerAlt )*
            loop30:
                while (true)
                {
                    int alt30 = 2;
                    int LA30_0 = input.LA(1);
                    if ((LA30_0 == OR))
                    {
                        alt30 = 1;
                    }

                    switch (alt30)
                    {
                        case 1:
                            // org\\antlr\\v4\\parse\\ANTLRParser.g:509:14: OR lexerAlt
                            {
                                OR114 = (Token)Match(input, OR, FOLLOW_OR_in_lexerAltList2023);
                                stream_OR.add(OR114);

                                pushFollow(FOLLOW_lexerAlt_in_lexerAltList2025);
                                lexerAlt115 = lexerAlt();
                                state._fsp--;

                                stream_lexerAlt.add(lexerAlt115.getTree());
                            }
                            break;

                        default:
                            goto exit30;
                            //break loop30;
                    }
                }

            exit30:
                // AST REWRITE
                // elements: lexerAlt
                // token labels: 
                // rule labels: retval
                // token list labels: 
                // rule list labels: 
                // wildcard labels: 
                retval.tree = root_0;
                RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval != null ? retval.getTree() : null);

                root_0 = (GrammarAST)adaptor.nil();
                // 509:28: -> ( lexerAlt )+
                {
                    if (!(stream_lexerAlt.hasNext()))
                    {
                        throw new RewriteEarlyExitException();
                    }
                    while (stream_lexerAlt.hasNext())
                    {
                        adaptor.addChild(root_0, stream_lexerAlt.nextTree());
                    }
                    stream_lexerAlt.reset();

                }


                retval.tree = root_0;

            }

            retval.stop = input.LT(-1);

            retval.tree = (GrammarAST)adaptor.rulePostProcessing(root_0);
            adaptor.setTokenBoundaries(retval.tree, retval.start, retval.stop);

        }
        catch (RecognitionException re)
        {
            reportError(re);
            recover(input, re);
            retval.tree = (GrammarAST)adaptor.errorNode(input, retval.start, input.LT(-1), re);
        }
        finally
        {
            // do for sure before leaving
        }
        return retval;
    }
    // $ANTLR end "lexerAltList"


    public class lexerAlt_return : ParserRuleReturnScope
    {
        public GrammarAST tree;
        //@Override

        public GrammarAST getTree() { return tree; }
    };


    // $ANTLR start "lexerAlt"
    // org\\antlr\\v4\\parse\\ANTLRParser.g:512:1: lexerAlt : lexerElements ( lexerCommands -> ^( LEXER_ALT_ACTION lexerElements lexerCommands ) | -> lexerElements ) ;
    public ANTLRParser.lexerAlt_return lexerAlt()  {
        ANTLRParser.lexerAlt_return retval = new ANTLRParser.lexerAlt_return();
        retval.start = input.LT(1);

        GrammarAST root_0 = null;

        ParserRuleReturnScope lexerElements116 = null;
        ParserRuleReturnScope lexerCommands117 = null;

        RewriteRuleSubtreeStream stream_lexerCommands = new RewriteRuleSubtreeStream(adaptor, "rule lexerCommands");
        RewriteRuleSubtreeStream stream_lexerElements = new RewriteRuleSubtreeStream(adaptor, "rule lexerElements");

        try
        {
            // org\\antlr\\v4\\parse\\ANTLRParser.g:513:2: ( lexerElements ( lexerCommands -> ^( LEXER_ALT_ACTION lexerElements lexerCommands ) | -> lexerElements ) )
            // org\\antlr\\v4\\parse\\ANTLRParser.g:513:4: lexerElements ( lexerCommands -> ^( LEXER_ALT_ACTION lexerElements lexerCommands ) | -> lexerElements )
            {
                pushFollow(FOLLOW_lexerElements_in_lexerAlt2043);
                lexerElements116 = lexerElements();
                state._fsp--;

                stream_lexerElements.add(lexerElements116.getTree());
                // org\\antlr\\v4\\parse\\ANTLRParser.g:514:3: ( lexerCommands -> ^( LEXER_ALT_ACTION lexerElements lexerCommands ) | -> lexerElements )
                int alt31 = 2;
                int LA31_0 = input.LA(1);
                if ((LA31_0 == RARROW))
                {
                    alt31 = 1;
                }
                else if ((LA31_0 == OR || LA31_0 == RPAREN || LA31_0 == SEMI))
                {
                    alt31 = 2;
                }

                else
                {
                    NoViableAltException nvae =
                        new NoViableAltException("", 31, 0, input);
                    throw nvae;
                }

                switch (alt31)
                {
                    case 1:
                        // org\\antlr\\v4\\parse\\ANTLRParser.g:514:5: lexerCommands
                        {
                            pushFollow(FOLLOW_lexerCommands_in_lexerAlt2049);
                            lexerCommands117 = lexerCommands();
                            state._fsp--;

                            stream_lexerCommands.add(lexerCommands117.getTree());

                            // AST REWRITE
                            // elements: lexerCommands, lexerElements
                            // token labels: 
                            // rule labels: retval
                            // token list labels: 
                            // rule list labels: 
                            // wildcard labels: 
                            retval.tree = root_0;
                            RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval != null ? retval.getTree() : null);

                            root_0 = (GrammarAST)adaptor.nil();
                            // 514:19: -> ^( LEXER_ALT_ACTION lexerElements lexerCommands )
                            {
                                // org\\antlr\\v4\\parse\\ANTLRParser.g:514:22: ^( LEXER_ALT_ACTION lexerElements lexerCommands )
                                {
                                    GrammarAST root_1 = (GrammarAST)adaptor.nil();
                                    root_1 = (GrammarAST)adaptor.becomeRoot(new AltAST(LEXER_ALT_ACTION), root_1);
                                    adaptor.addChild(root_1, stream_lexerElements.nextTree());
                                    adaptor.addChild(root_1, stream_lexerCommands.nextTree());
                                    adaptor.addChild(root_0, root_1);
                                }

                            }


                            retval.tree = root_0;

                        }
                        break;
                    case 2:
                        // org\\antlr\\v4\\parse\\ANTLRParser.g:515:9: 
                        {

                            // AST REWRITE
                            // elements: lexerElements
                            // token labels: 
                            // rule labels: retval
                            // token list labels: 
                            // rule list labels: 
                            // wildcard labels: 
                            retval.tree = root_0;
                            RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval != null ? retval.getTree() : null);

                            root_0 = (GrammarAST)adaptor.nil();
                            // 515:9: -> lexerElements
                            {
                                adaptor.addChild(root_0, stream_lexerElements.nextTree());
                            }


                            retval.tree = root_0;

                        }
                        break;

                }

            }

            retval.stop = input.LT(-1);

            retval.tree = (GrammarAST)adaptor.rulePostProcessing(root_0);
            adaptor.setTokenBoundaries(retval.tree, retval.start, retval.stop);

        }
        catch (RecognitionException re)
        {
            reportError(re);
            recover(input, re);
            retval.tree = (GrammarAST)adaptor.errorNode(input, retval.start, input.LT(-1), re);
        }
        finally
        {
            // do for sure before leaving
        }
        return retval;
    }
    // $ANTLR end "lexerAlt"


    public class lexerElements_return : ParserRuleReturnScope
    {
        public GrammarAST tree;
        //@Override

        public GrammarAST getTree() { return tree; }
    };


    // $ANTLR start "lexerElements"
    // org\\antlr\\v4\\parse\\ANTLRParser.g:519:1: lexerElements : ( ( lexerElement )+ -> ^( ALT ( lexerElement )+ ) | -> ^( ALT EPSILON ) );
    public ANTLRParser.lexerElements_return lexerElements()  {
        ANTLRParser.lexerElements_return retval = new ANTLRParser.lexerElements_return();
        retval.start = input.LT(1);

        GrammarAST root_0 = null;

        ParserRuleReturnScope lexerElement118 = null;

        RewriteRuleSubtreeStream stream_lexerElement = new RewriteRuleSubtreeStream(adaptor, "rule lexerElement");

        try
        {
            // org\\antlr\\v4\\parse\\ANTLRParser.g:520:5: ( ( lexerElement )+ -> ^( ALT ( lexerElement )+ ) | -> ^( ALT EPSILON ) )
            int alt33 = 2;
            int LA33_0 = input.LA(1);
            if ((LA33_0 == ACTION || LA33_0 == DOT || LA33_0 == LEXER_CHAR_SET || LA33_0 == LPAREN || LA33_0 == NOT || LA33_0 == RULE_REF || LA33_0 == SEMPRED || LA33_0 == STRING_LITERAL || LA33_0 == TOKEN_REF))
            {
                alt33 = 1;
            }
            else if ((LA33_0 == OR || LA33_0 == RARROW || LA33_0 == RPAREN || LA33_0 == SEMI))
            {
                alt33 = 2;
            }

            else
            {
                NoViableAltException nvae =
                    new NoViableAltException("", 33, 0, input);
                throw nvae;
            }

            switch (alt33)
            {
                case 1:
                    // org\\antlr\\v4\\parse\\ANTLRParser.g:520:7: ( lexerElement )+
                    {
                        // org\\antlr\\v4\\parse\\ANTLRParser.g:520:7: ( lexerElement )+
                        int cnt32 = 0;
                    loop32:
                        while (true)
                        {
                            int alt32 = 2;
                            int LA32_0 = input.LA(1);
                            if ((LA32_0 == ACTION || LA32_0 == DOT || LA32_0 == LEXER_CHAR_SET || LA32_0 == LPAREN || LA32_0 == NOT || LA32_0 == RULE_REF || LA32_0 == SEMPRED || LA32_0 == STRING_LITERAL || LA32_0 == TOKEN_REF))
                            {
                                alt32 = 1;
                            }

                            switch (alt32)
                            {
                                case 1:
                                    // org\\antlr\\v4\\parse\\ANTLRParser.g:520:7: lexerElement
                                    {
                                        pushFollow(FOLLOW_lexerElement_in_lexerElements2092);
                                        lexerElement118 = lexerElement();
                                        state._fsp--;

                                        stream_lexerElement.add(lexerElement118.getTree());
                                    }
                                    break;

                                default:
                                    if (cnt32 >= 1) goto exit32;// break loop32;
                                    EarlyExitException eee = new EarlyExitException(32, input);
                                    throw eee;
                            }
                            cnt32++;
                        }
                    exit32:

                        // AST REWRITE
                        // elements: lexerElement
                        // token labels: 
                        // rule labels: retval
                        // token list labels: 
                        // rule list labels: 
                        // wildcard labels: 
                        retval.tree = root_0;
                        RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval != null ? retval.getTree() : null);

                        root_0 = (GrammarAST)adaptor.nil();
                        // 520:21: -> ^( ALT ( lexerElement )+ )
                        {
                            // org\\antlr\\v4\\parse\\ANTLRParser.g:520:24: ^( ALT ( lexerElement )+ )
                            {
                                GrammarAST root_1 = (GrammarAST)adaptor.nil();
                                root_1 = (GrammarAST)adaptor.becomeRoot(new AltAST(ALT), root_1);
                                if (!(stream_lexerElement.hasNext()))
                                {
                                    throw new RewriteEarlyExitException();
                                }
                                while (stream_lexerElement.hasNext())
                                {
                                    adaptor.addChild(root_1, stream_lexerElement.nextTree());
                                }
                                stream_lexerElement.reset();

                                adaptor.addChild(root_0, root_1);
                            }

                        }


                        retval.tree = root_0;

                    }
                    break;
                case 2:
                    // org\\antlr\\v4\\parse\\ANTLRParser.g:521:8: 
                    {

                        // AST REWRITE
                        // elements: 
                        // token labels: 
                        // rule labels: retval
                        // token list labels: 
                        // rule list labels: 
                        // wildcard labels: 
                        retval.tree = root_0;
                        RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval != null ? retval.getTree() : null);

                        root_0 = (GrammarAST)adaptor.nil();
                        // 521:8: -> ^( ALT EPSILON )
                        {
                            // org\\antlr\\v4\\parse\\ANTLRParser.g:521:11: ^( ALT EPSILON )
                            {
                                GrammarAST root_1 = (GrammarAST)adaptor.nil();
                                root_1 = (GrammarAST)adaptor.becomeRoot(new AltAST(ALT), root_1);
                                adaptor.addChild(root_1, (GrammarAST)adaptor.create(EPSILON, "EPSILON"));
                                adaptor.addChild(root_0, root_1);
                            }

                        }


                        retval.tree = root_0;

                    }
                    break;

            }
            retval.stop = input.LT(-1);

            retval.tree = (GrammarAST)adaptor.rulePostProcessing(root_0);
            adaptor.setTokenBoundaries(retval.tree, retval.start, retval.stop);

        }
        catch (RecognitionException re)
        {
            reportError(re);
            recover(input, re);
            retval.tree = (GrammarAST)adaptor.errorNode(input, retval.start, input.LT(-1), re);
        }
        finally
        {
            // do for sure before leaving
        }
        return retval;
    }
    // $ANTLR end "lexerElements"


    public class lexerElement_return : ParserRuleReturnScope
    {
        public GrammarAST tree;
        //@Override

        public GrammarAST getTree() { return tree; }
    };


    // $ANTLR start "lexerElement"
    // org\\antlr\\v4\\parse\\ANTLRParser.g:524:1: lexerElement : ( lexerAtom ( ebnfSuffix -> ^( ebnfSuffix ^( BLOCK[$lexerAtom.start,\"BLOCK\"] ^( ALT lexerAtom ) ) ) | -> lexerAtom ) | lexerBlock ( ebnfSuffix -> ^( ebnfSuffix lexerBlock ) | -> lexerBlock ) | actionElement );
    public ANTLRParser.lexerElement_return lexerElement()  {
        ANTLRParser.lexerElement_return retval = new ANTLRParser.lexerElement_return();
        retval.start = input.LT(1);

        GrammarAST root_0 = null;

        ParserRuleReturnScope lexerAtom119 = null;
        ParserRuleReturnScope ebnfSuffix120 = null;
        ParserRuleReturnScope lexerBlock121 = null;
        ParserRuleReturnScope ebnfSuffix122 = null;
        ParserRuleReturnScope actionElement123 = null;

        RewriteRuleSubtreeStream stream_lexerBlock = new RewriteRuleSubtreeStream(adaptor, "rule lexerBlock");
        RewriteRuleSubtreeStream stream_lexerAtom = new RewriteRuleSubtreeStream(adaptor, "rule lexerAtom");
        RewriteRuleSubtreeStream stream_ebnfSuffix = new RewriteRuleSubtreeStream(adaptor, "rule ebnfSuffix");


        paraphrases.push("looking for lexer rule element");
        int m = input.mark();

        try
        {
            // org\\antlr\\v4\\parse\\ANTLRParser.g:530:2: ( lexerAtom ( ebnfSuffix -> ^( ebnfSuffix ^( BLOCK[$lexerAtom.start,\"BLOCK\"] ^( ALT lexerAtom ) ) ) | -> lexerAtom ) | lexerBlock ( ebnfSuffix -> ^( ebnfSuffix lexerBlock ) | -> lexerBlock ) | actionElement )
            int alt36 = 3;
            switch (input.LA(1))
            {
                case DOT:
                case LEXER_CHAR_SET:
                case NOT:
                case RULE_REF:
                case STRING_LITERAL:
                case TOKEN_REF:
                    {
                        alt36 = 1;
                    }
                    break;
                case LPAREN:
                    {
                        alt36 = 2;
                    }
                    break;
                case ACTION:
                case SEMPRED:
                    {
                        alt36 = 3;
                    }
                    break;
                default:
                    NoViableAltException nvae =
                        new NoViableAltException("", 36, 0, input);
                    throw nvae;
            }
            switch (alt36)
            {
                case 1:
                    // org\\antlr\\v4\\parse\\ANTLRParser.g:530:4: lexerAtom ( ebnfSuffix -> ^( ebnfSuffix ^( BLOCK[$lexerAtom.start,\"BLOCK\"] ^( ALT lexerAtom ) ) ) | -> lexerAtom )
                    {
                        pushFollow(FOLLOW_lexerAtom_in_lexerElement2148);
                        lexerAtom119 = lexerAtom();
                        state._fsp--;

                        stream_lexerAtom.add(lexerAtom119.getTree());
                        // org\\antlr\\v4\\parse\\ANTLRParser.g:531:3: ( ebnfSuffix -> ^( ebnfSuffix ^( BLOCK[$lexerAtom.start,\"BLOCK\"] ^( ALT lexerAtom ) ) ) | -> lexerAtom )
                        int alt34 = 2;
                        int LA34_0 = input.LA(1);
                        if ((LA34_0 == PLUS || LA34_0 == QUESTION || LA34_0 == STAR))
                        {
                            alt34 = 1;
                        }
                        else if ((LA34_0 == ACTION || LA34_0 == DOT || LA34_0 == LEXER_CHAR_SET || LA34_0 == LPAREN || LA34_0 == NOT || LA34_0 == OR || LA34_0 == RARROW || (LA34_0 >= RPAREN && LA34_0 <= SEMPRED) || LA34_0 == STRING_LITERAL || LA34_0 == TOKEN_REF))
                        {
                            alt34 = 2;
                        }

                        else
                        {
                            NoViableAltException nvae =
                                new NoViableAltException("", 34, 0, input);
                            throw nvae;
                        }

                        switch (alt34)
                        {
                            case 1:
                                // org\\antlr\\v4\\parse\\ANTLRParser.g:531:5: ebnfSuffix
                                {
                                    pushFollow(FOLLOW_ebnfSuffix_in_lexerElement2154);
                                    ebnfSuffix120 = ebnfSuffix();
                                    state._fsp--;

                                    stream_ebnfSuffix.add(ebnfSuffix120.getTree());

                                    // AST REWRITE
                                    // elements: lexerAtom, ebnfSuffix
                                    // token labels: 
                                    // rule labels: retval
                                    // token list labels: 
                                    // rule list labels: 
                                    // wildcard labels: 
                                    retval.tree = root_0;
                                    RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval != null ? retval.getTree() : null);

                                    root_0 = (GrammarAST)adaptor.nil();
                                    // 531:16: -> ^( ebnfSuffix ^( BLOCK[$lexerAtom.start,\"BLOCK\"] ^( ALT lexerAtom ) ) )
                                    {
                                        // org\\antlr\\v4\\parse\\ANTLRParser.g:531:19: ^( ebnfSuffix ^( BLOCK[$lexerAtom.start,\"BLOCK\"] ^( ALT lexerAtom ) ) )
                                        {
                                            GrammarAST root_1 = (GrammarAST)adaptor.nil();
                                            root_1 = (GrammarAST)adaptor.becomeRoot(stream_ebnfSuffix.nextNode(), root_1);
                                            // org\\antlr\\v4\\parse\\ANTLRParser.g:531:33: ^( BLOCK[$lexerAtom.start,\"BLOCK\"] ^( ALT lexerAtom ) )
                                            {
                                                GrammarAST root_2 = (GrammarAST)adaptor.nil();
                                                root_2 = (GrammarAST)adaptor.becomeRoot(new BlockAST(BLOCK, (lexerAtom119 != null ? (lexerAtom119.start) : null), "BLOCK"), root_2);
                                                // org\\antlr\\v4\\parse\\ANTLRParser.g:531:77: ^( ALT lexerAtom )
                                                {
                                                    GrammarAST root_3 = (GrammarAST)adaptor.nil();
                                                    root_3 = (GrammarAST)adaptor.becomeRoot(new AltAST(ALT), root_3);
                                                    adaptor.addChild(root_3, stream_lexerAtom.nextTree());
                                                    adaptor.addChild(root_2, root_3);
                                                }

                                                adaptor.addChild(root_1, root_2);
                                            }

                                            adaptor.addChild(root_0, root_1);
                                        }

                                    }


                                    retval.tree = root_0;

                                }
                                break;
                            case 2:
                                // org\\antlr\\v4\\parse\\ANTLRParser.g:532:8: 
                                {

                                    // AST REWRITE
                                    // elements: lexerAtom
                                    // token labels: 
                                    // rule labels: retval
                                    // token list labels: 
                                    // rule list labels: 
                                    // wildcard labels: 
                                    retval.tree = root_0;
                                    RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval != null ? retval.getTree() : null);

                                    root_0 = (GrammarAST)adaptor.nil();
                                    // 532:8: -> lexerAtom
                                    {
                                        adaptor.addChild(root_0, stream_lexerAtom.nextTree());
                                    }


                                    retval.tree = root_0;

                                }
                                break;

                        }

                    }
                    break;
                case 2:
                    // org\\antlr\\v4\\parse\\ANTLRParser.g:534:4: lexerBlock ( ebnfSuffix -> ^( ebnfSuffix lexerBlock ) | -> lexerBlock )
                    {
                        pushFollow(FOLLOW_lexerBlock_in_lexerElement2200);
                        lexerBlock121 = lexerBlock();
                        state._fsp--;

                        stream_lexerBlock.add(lexerBlock121.getTree());
                        // org\\antlr\\v4\\parse\\ANTLRParser.g:535:3: ( ebnfSuffix -> ^( ebnfSuffix lexerBlock ) | -> lexerBlock )
                        int alt35 = 2;
                        int LA35_0 = input.LA(1);
                        if ((LA35_0 == PLUS || LA35_0 == QUESTION || LA35_0 == STAR))
                        {
                            alt35 = 1;
                        }
                        else if ((LA35_0 == ACTION || LA35_0 == DOT || LA35_0 == LEXER_CHAR_SET || LA35_0 == LPAREN || LA35_0 == NOT || LA35_0 == OR || LA35_0 == RARROW || (LA35_0 >= RPAREN && LA35_0 <= SEMPRED) || LA35_0 == STRING_LITERAL || LA35_0 == TOKEN_REF))
                        {
                            alt35 = 2;
                        }

                        else
                        {
                            NoViableAltException nvae =
                                new NoViableAltException("", 35, 0, input);
                            throw nvae;
                        }

                        switch (alt35)
                        {
                            case 1:
                                // org\\antlr\\v4\\parse\\ANTLRParser.g:535:5: ebnfSuffix
                                {
                                    pushFollow(FOLLOW_ebnfSuffix_in_lexerElement2206);
                                    ebnfSuffix122 = ebnfSuffix();
                                    state._fsp--;

                                    stream_ebnfSuffix.add(ebnfSuffix122.getTree());

                                    // AST REWRITE
                                    // elements: ebnfSuffix, lexerBlock
                                    // token labels: 
                                    // rule labels: retval
                                    // token list labels: 
                                    // rule list labels: 
                                    // wildcard labels: 
                                    retval.tree = root_0;
                                    RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval != null ? retval.getTree() : null);

                                    root_0 = (GrammarAST)adaptor.nil();
                                    // 535:16: -> ^( ebnfSuffix lexerBlock )
                                    {
                                        // org\\antlr\\v4\\parse\\ANTLRParser.g:535:19: ^( ebnfSuffix lexerBlock )
                                        {
                                            GrammarAST root_1 = (GrammarAST)adaptor.nil();
                                            root_1 = (GrammarAST)adaptor.becomeRoot(stream_ebnfSuffix.nextNode(), root_1);
                                            adaptor.addChild(root_1, stream_lexerBlock.nextTree());
                                            adaptor.addChild(root_0, root_1);
                                        }

                                    }


                                    retval.tree = root_0;

                                }
                                break;
                            case 2:
                                // org\\antlr\\v4\\parse\\ANTLRParser.g:536:8: 
                                {

                                    // AST REWRITE
                                    // elements: lexerBlock
                                    // token labels: 
                                    // rule labels: retval
                                    // token list labels: 
                                    // rule list labels: 
                                    // wildcard labels: 
                                    retval.tree = root_0;
                                    RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval != null ? retval.getTree() : null);

                                    root_0 = (GrammarAST)adaptor.nil();
                                    // 536:8: -> lexerBlock
                                    {
                                        adaptor.addChild(root_0, stream_lexerBlock.nextTree());
                                    }


                                    retval.tree = root_0;

                                }
                                break;

                        }

                    }
                    break;
                case 3:
                    // org\\antlr\\v4\\parse\\ANTLRParser.g:538:4: actionElement
                    {
                        root_0 = (GrammarAST)adaptor.nil();


                        pushFollow(FOLLOW_actionElement_in_lexerElement2234);
                        actionElement123 = actionElement();
                        state._fsp--;

                        adaptor.addChild(root_0, actionElement123.getTree());

                    }
                    break;

            }
            retval.stop = input.LT(-1);

            retval.tree = (GrammarAST)adaptor.rulePostProcessing(root_0);
            adaptor.setTokenBoundaries(retval.tree, retval.start, retval.stop);

            paraphrases.pop();
        }
        catch (RecognitionException re)
        {

            retval.tree = (GrammarAST)adaptor.errorNode(input, retval.start, input.LT(-1), re);
            int ttype = input.get(input.range()).getType(); // seems to be next token
                                                            // look for anything that really belongs at the start of the rule minus the initial ID
            if (ttype == COLON || ttype == RETURNS || ttype == CATCH || ttype == FINALLY || ttype == AT || ttype == EOF)
            {
                RecognitionException missingSemi =
                    new v4ParserException("unterminated rule (missing ';') detected at '" +
                                          input.LT(1).getText() + " " + input.LT(2).getText() + "'", input);
                reportError(missingSemi);
                if (ttype == EOF)
                {
                    input.seek(input.index() + 1);
                }
                else if (ttype == CATCH || ttype == FINALLY)
                {
                    input.seek(input.range()); // ignore what's before rule trailer stuff
                }
                else if (ttype == RETURNS || ttype == AT)
                { // scan back looking for ID of rule header
                    int p = input.index();
                    Token t = input.get(p);
                    while (t.getType() != RULE_REF && t.getType() != TOKEN_REF)
                    {
                        p--;
                        t = input.get(p);
                    }
                    input.seek(p);
                }
                throw new ResyncToEndOfRuleBlock(); // make sure it goes back to rule block level to recover
            }
            reportError(re);
            recover(input, re);

        }

        finally
        {
            // do for sure before leaving
        }
        return retval;
    }
    // $ANTLR end "lexerElement"


    public class lexerBlock_return : ParserRuleReturnScope
    {
        public GrammarAST tree;
        //@Override

        public GrammarAST getTree() { return tree; }
    };


    // $ANTLR start "lexerBlock"
    // org\\antlr\\v4\\parse\\ANTLRParser.g:571:1: lexerBlock : LPAREN ( optionsSpec COLON )? lexerAltList RPAREN -> ^( BLOCK[$LPAREN,\"BLOCK\"] ( optionsSpec )? lexerAltList ) ;
    public ANTLRParser.lexerBlock_return lexerBlock()  {
        ANTLRParser.lexerBlock_return retval = new ANTLRParser.lexerBlock_return();
        retval.start = input.LT(1);

        GrammarAST root_0 = null;

        Token LPAREN124 = null;
        Token COLON126 = null;
        Token RPAREN128 = null;
        ParserRuleReturnScope optionsSpec125 = null;
        ParserRuleReturnScope lexerAltList127 = null;

        GrammarAST LPAREN124_tree = null;
        GrammarAST COLON126_tree = null;
        GrammarAST RPAREN128_tree = null;
        RewriteRuleTokenStream stream_LPAREN = new RewriteRuleTokenStream(adaptor, "token LPAREN");
        RewriteRuleTokenStream stream_COLON = new RewriteRuleTokenStream(adaptor, "token COLON");
        RewriteRuleTokenStream stream_RPAREN = new RewriteRuleTokenStream(adaptor, "token RPAREN");
        RewriteRuleSubtreeStream stream_optionsSpec = new RewriteRuleSubtreeStream(adaptor, "rule optionsSpec");
        RewriteRuleSubtreeStream stream_lexerAltList = new RewriteRuleSubtreeStream(adaptor, "rule lexerAltList");

        try
        {
            // org\\antlr\\v4\\parse\\ANTLRParser.g:578:3: ( LPAREN ( optionsSpec COLON )? lexerAltList RPAREN -> ^( BLOCK[$LPAREN,\"BLOCK\"] ( optionsSpec )? lexerAltList ) )
            // org\\antlr\\v4\\parse\\ANTLRParser.g:578:5: LPAREN ( optionsSpec COLON )? lexerAltList RPAREN
            {
                LPAREN124 = (Token)Match(input, LPAREN, FOLLOW_LPAREN_in_lexerBlock2270);
                stream_LPAREN.add(LPAREN124);

                // org\\antlr\\v4\\parse\\ANTLRParser.g:579:9: ( optionsSpec COLON )?
                int alt37 = 2;
                int LA37_0 = input.LA(1);
                if ((LA37_0 == OPTIONS))
                {
                    alt37 = 1;
                }
                switch (alt37)
                {
                    case 1:
                        // org\\antlr\\v4\\parse\\ANTLRParser.g:579:11: optionsSpec COLON
                        {
                            pushFollow(FOLLOW_optionsSpec_in_lexerBlock2282);
                            optionsSpec125 = optionsSpec();
                            state._fsp--;

                            stream_optionsSpec.add(optionsSpec125.getTree());
                            COLON126 = (Token)Match(input, COLON, FOLLOW_COLON_in_lexerBlock2284);
                            stream_COLON.add(COLON126);

                        }
                        break;

                }

                pushFollow(FOLLOW_lexerAltList_in_lexerBlock2297);
                lexerAltList127 = lexerAltList();
                state._fsp--;

                stream_lexerAltList.add(lexerAltList127.getTree());
                RPAREN128 = (Token)Match(input, RPAREN, FOLLOW_RPAREN_in_lexerBlock2307);
                stream_RPAREN.add(RPAREN128);


                // AST REWRITE
                // elements: lexerAltList, optionsSpec
                // token labels: 
                // rule labels: retval
                // token list labels: 
                // rule list labels: 
                // wildcard labels: 
                retval.tree = root_0;
                RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval != null ? retval.getTree() : null);

                root_0 = (GrammarAST)adaptor.nil();
                // 582:7: -> ^( BLOCK[$LPAREN,\"BLOCK\"] ( optionsSpec )? lexerAltList )
                {
                    // org\\antlr\\v4\\parse\\ANTLRParser.g:582:10: ^( BLOCK[$LPAREN,\"BLOCK\"] ( optionsSpec )? lexerAltList )
                    {
                        GrammarAST root_1 = (GrammarAST)adaptor.nil();
                        root_1 = (GrammarAST)adaptor.becomeRoot(new BlockAST(BLOCK, LPAREN124, "BLOCK"), root_1);
                        // org\\antlr\\v4\\parse\\ANTLRParser.g:582:45: ( optionsSpec )?
                        if (stream_optionsSpec.hasNext())
                        {
                            adaptor.addChild(root_1, stream_optionsSpec.nextTree());
                        }
                        stream_optionsSpec.reset();

                        adaptor.addChild(root_1, stream_lexerAltList.nextTree());
                        adaptor.addChild(root_0, root_1);
                    }

                }


                retval.tree = root_0;

            }

            retval.stop = input.LT(-1);

            retval.tree = (GrammarAST)adaptor.rulePostProcessing(root_0);
            adaptor.setTokenBoundaries(retval.tree, retval.start, retval.stop);


            GrammarAST options = (GrammarAST)retval.tree.getFirstChildWithType(ANTLRParser.OPTIONS);
            if (options != null)
            {
                Grammar.setNodeOptions(retval.tree, options);
            }

        }
        catch (RecognitionException re)
        {
            reportError(re);
            recover(input, re);
            retval.tree = (GrammarAST)adaptor.errorNode(input, retval.start, input.LT(-1), re);
        }
        finally
        {
            // do for sure before leaving
        }
        return retval;
    }
    // $ANTLR end "lexerBlock"


    public class lexerCommands_return : ParserRuleReturnScope
    {
        public GrammarAST tree;
        //@Override

        public GrammarAST getTree() { return tree; }
    };


    // $ANTLR start "lexerCommands"
    // org\\antlr\\v4\\parse\\ANTLRParser.g:586:1: lexerCommands : RARROW lexerCommand ( COMMA lexerCommand )* -> ( lexerCommand )+ ;
    public ANTLRParser.lexerCommands_return lexerCommands()  {
        ANTLRParser.lexerCommands_return retval = new ANTLRParser.lexerCommands_return();
        retval.start = input.LT(1);

        GrammarAST root_0 = null;

        Token RARROW129 = null;
        Token COMMA131 = null;
        ParserRuleReturnScope lexerCommand130 = null;
        ParserRuleReturnScope lexerCommand132 = null;

        GrammarAST RARROW129_tree = null;
        GrammarAST COMMA131_tree = null;
        RewriteRuleTokenStream stream_RARROW = new RewriteRuleTokenStream(adaptor, "token RARROW");
        RewriteRuleTokenStream stream_COMMA = new RewriteRuleTokenStream(adaptor, "token COMMA");
        RewriteRuleSubtreeStream stream_lexerCommand = new RewriteRuleSubtreeStream(adaptor, "rule lexerCommand");

        try
        {
            // org\\antlr\\v4\\parse\\ANTLRParser.g:587:2: ( RARROW lexerCommand ( COMMA lexerCommand )* -> ( lexerCommand )+ )
            // org\\antlr\\v4\\parse\\ANTLRParser.g:587:4: RARROW lexerCommand ( COMMA lexerCommand )*
            {
                RARROW129 = (Token)Match(input, RARROW, FOLLOW_RARROW_in_lexerCommands2344);
                stream_RARROW.add(RARROW129);

                pushFollow(FOLLOW_lexerCommand_in_lexerCommands2346);
                lexerCommand130 = lexerCommand();
                state._fsp--;

                stream_lexerCommand.add(lexerCommand130.getTree());
            // org\\antlr\\v4\\parse\\ANTLRParser.g:587:24: ( COMMA lexerCommand )*
            loop38:
                while (true)
                {
                    int alt38 = 2;
                    int LA38_0 = input.LA(1);
                    if ((LA38_0 == COMMA))
                    {
                        alt38 = 1;
                    }

                    switch (alt38)
                    {
                        case 1:
                            // org\\antlr\\v4\\parse\\ANTLRParser.g:587:25: COMMA lexerCommand
                            {
                                COMMA131 = (Token)Match(input, COMMA, FOLLOW_COMMA_in_lexerCommands2349);
                                stream_COMMA.add(COMMA131);

                                pushFollow(FOLLOW_lexerCommand_in_lexerCommands2351);
                                lexerCommand132 = lexerCommand();
                                state._fsp--;

                                stream_lexerCommand.add(lexerCommand132.getTree());
                            }
                            break;

                        default:
                            goto exit38;
                            //break loop38;
                    }
                }
            exit38:

                // AST REWRITE
                // elements: lexerCommand
                // token labels: 
                // rule labels: retval
                // token list labels: 
                // rule list labels: 
                // wildcard labels: 
                retval.tree = root_0;
                RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval != null ? retval.getTree() : null);

                root_0 = (GrammarAST)adaptor.nil();
                // 587:46: -> ( lexerCommand )+
                {
                    if (!(stream_lexerCommand.hasNext()))
                    {
                        throw new RewriteEarlyExitException();
                    }
                    while (stream_lexerCommand.hasNext())
                    {
                        adaptor.addChild(root_0, stream_lexerCommand.nextTree());
                    }
                    stream_lexerCommand.reset();

                }


                retval.tree = root_0;

            }

            retval.stop = input.LT(-1);

            retval.tree = (GrammarAST)adaptor.rulePostProcessing(root_0);
            adaptor.setTokenBoundaries(retval.tree, retval.start, retval.stop);

        }
        catch (RecognitionException re)
        {
            reportError(re);
            recover(input, re);
            retval.tree = (GrammarAST)adaptor.errorNode(input, retval.start, input.LT(-1), re);
        }
        finally
        {
            // do for sure before leaving
        }
        return retval;
    }
    // $ANTLR end "lexerCommands"


    public class lexerCommand_return : ParserRuleReturnScope
    {
        public GrammarAST tree;
        //@Override

        public GrammarAST getTree() { return tree; }
    };


    // $ANTLR start "lexerCommand"
    // org\\antlr\\v4\\parse\\ANTLRParser.g:590:1: lexerCommand : ( lexerCommandName LPAREN lexerCommandExpr RPAREN -> ^( LEXER_ACTION_CALL lexerCommandName lexerCommandExpr ) | lexerCommandName );
    public ANTLRParser.lexerCommand_return lexerCommand()  {
        ANTLRParser.lexerCommand_return retval = new ANTLRParser.lexerCommand_return();
        retval.start = input.LT(1);

        GrammarAST root_0 = null;

        Token LPAREN134 = null;
        Token RPAREN136 = null;
        ParserRuleReturnScope lexerCommandName133 = null;
        ParserRuleReturnScope lexerCommandExpr135 = null;
        ParserRuleReturnScope lexerCommandName137 = null;

        GrammarAST LPAREN134_tree = null;
        GrammarAST RPAREN136_tree = null;
        RewriteRuleTokenStream stream_LPAREN = new RewriteRuleTokenStream(adaptor, "token LPAREN");
        RewriteRuleTokenStream stream_RPAREN = new RewriteRuleTokenStream(adaptor, "token RPAREN");
        RewriteRuleSubtreeStream stream_lexerCommandName = new RewriteRuleSubtreeStream(adaptor, "rule lexerCommandName");
        RewriteRuleSubtreeStream stream_lexerCommandExpr = new RewriteRuleSubtreeStream(adaptor, "rule lexerCommandExpr");

        try
        {
            // org\\antlr\\v4\\parse\\ANTLRParser.g:591:2: ( lexerCommandName LPAREN lexerCommandExpr RPAREN -> ^( LEXER_ACTION_CALL lexerCommandName lexerCommandExpr ) | lexerCommandName )
            int alt39 = 2;
            switch (input.LA(1))
            {
                case RULE_REF:
                    {
                        int LA39_1 = input.LA(2);
                        if ((LA39_1 == LPAREN))
                        {
                            alt39 = 1;
                        }
                        else if ((LA39_1 == COMMA || LA39_1 == OR || LA39_1 == RPAREN || LA39_1 == SEMI))
                        {
                            alt39 = 2;
                        }

                        else
                        {
                            int nvaeMark = input.mark();
                            try
                            {
                                input.consume();
                                NoViableAltException nvae3 =
                                    new NoViableAltException("", 39, 1, input);
                                throw nvae3;
                            }
                            finally
                            {
                                input.rewind(nvaeMark);
                            }
                        }

                    }
                    break;
                case TOKEN_REF:
                    {
                        int LA39_2 = input.LA(2);
                        if ((LA39_2 == LPAREN))
                        {
                            alt39 = 1;
                        }
                        else if ((LA39_2 == COMMA || LA39_2 == OR || LA39_2 == RPAREN || LA39_2 == SEMI))
                        {
                            alt39 = 2;
                        }

                        else
                        {
                            int nvaeMark = input.mark();
                            try
                            {
                                input.consume();
                                NoViableAltException nvae5 =
                                    new NoViableAltException("", 39, 2, input);
                                throw nvae5;
                            }
                            finally
                            {
                                input.rewind(nvaeMark);
                            }
                        }

                    }
                    break;
                case MODE:
                    {
                        int LA39_3 = input.LA(2);
                        if ((LA39_3 == LPAREN))
                        {
                            alt39 = 1;
                        }
                        else if ((LA39_3 == COMMA || LA39_3 == OR || LA39_3 == RPAREN || LA39_3 == SEMI))
                        {
                            alt39 = 2;
                        }

                        else
                        {
                            int nvaeMark = input.mark();
                            try
                            {
                                input.consume();
                                NoViableAltException nvae4 =
                                    new NoViableAltException("", 39, 3, input);
                                throw nvae4;
                            }
                            finally
                            {
                                input.rewind(nvaeMark);
                            }
                        }

                    }
                    break;
                default:
                    NoViableAltException nvae =
                        new NoViableAltException("", 39, 0, input);
                    throw nvae;
            }
            switch (alt39)
            {
                case 1:
                    // org\\antlr\\v4\\parse\\ANTLRParser.g:591:4: lexerCommandName LPAREN lexerCommandExpr RPAREN
                    {
                        pushFollow(FOLLOW_lexerCommandName_in_lexerCommand2369);
                        lexerCommandName133 = lexerCommandName();
                        state._fsp--;

                        stream_lexerCommandName.add(lexerCommandName133.getTree());
                        LPAREN134 = (Token)Match(input, LPAREN, FOLLOW_LPAREN_in_lexerCommand2371);
                        stream_LPAREN.add(LPAREN134);

                        pushFollow(FOLLOW_lexerCommandExpr_in_lexerCommand2373);
                        lexerCommandExpr135 = lexerCommandExpr();
                        state._fsp--;

                        stream_lexerCommandExpr.add(lexerCommandExpr135.getTree());
                        RPAREN136 = (Token)Match(input, RPAREN, FOLLOW_RPAREN_in_lexerCommand2375);
                        stream_RPAREN.add(RPAREN136);


                        // AST REWRITE
                        // elements: lexerCommandName, lexerCommandExpr
                        // token labels: 
                        // rule labels: retval
                        // token list labels: 
                        // rule list labels: 
                        // wildcard labels: 
                        retval.tree = root_0;
                        RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval != null ? retval.getTree() : null);

                        root_0 = (GrammarAST)adaptor.nil();
                        // 591:52: -> ^( LEXER_ACTION_CALL lexerCommandName lexerCommandExpr )
                        {
                            // org\\antlr\\v4\\parse\\ANTLRParser.g:591:55: ^( LEXER_ACTION_CALL lexerCommandName lexerCommandExpr )
                            {
                                GrammarAST root_1 = (GrammarAST)adaptor.nil();
                                root_1 = (GrammarAST)adaptor.becomeRoot((GrammarAST)adaptor.create(LEXER_ACTION_CALL, "LEXER_ACTION_CALL"), root_1);
                                adaptor.addChild(root_1, stream_lexerCommandName.nextTree());
                                adaptor.addChild(root_1, stream_lexerCommandExpr.nextTree());
                                adaptor.addChild(root_0, root_1);
                            }

                        }


                        retval.tree = root_0;

                    }
                    break;
                case 2:
                    // org\\antlr\\v4\\parse\\ANTLRParser.g:592:4: lexerCommandName
                    {
                        root_0 = (GrammarAST)adaptor.nil();


                        pushFollow(FOLLOW_lexerCommandName_in_lexerCommand2390);
                        lexerCommandName137 = lexerCommandName();
                        state._fsp--;

                        adaptor.addChild(root_0, lexerCommandName137.getTree());

                    }
                    break;

            }
            retval.stop = input.LT(-1);

            retval.tree = (GrammarAST)adaptor.rulePostProcessing(root_0);
            adaptor.setTokenBoundaries(retval.tree, retval.start, retval.stop);

        }
        catch (RecognitionException re)
        {
            reportError(re);
            recover(input, re);
            retval.tree = (GrammarAST)adaptor.errorNode(input, retval.start, input.LT(-1), re);
        }
        finally
        {
            // do for sure before leaving
        }
        return retval;
    }
    // $ANTLR end "lexerCommand"


    public class lexerCommandExpr_return : ParserRuleReturnScope
    {
        public GrammarAST tree;
        //@Override

        public GrammarAST getTree() { return tree; }
    };


    // $ANTLR start "lexerCommandExpr"
    // org\\antlr\\v4\\parse\\ANTLRParser.g:595:1: lexerCommandExpr : ( id | INT );
    public ANTLRParser.lexerCommandExpr_return lexerCommandExpr()  {
        ANTLRParser.lexerCommandExpr_return retval = new ANTLRParser.lexerCommandExpr_return();
        retval.start = input.LT(1);

        GrammarAST root_0 = null;

        Token INT139 = null;
        ParserRuleReturnScope id138 = null;

        GrammarAST INT139_tree = null;

        try
        {
            // org\\antlr\\v4\\parse\\ANTLRParser.g:596:2: ( id | INT )
            int alt40 = 2;
            int LA40_0 = input.LA(1);
            if ((LA40_0 == RULE_REF || LA40_0 == TOKEN_REF))
            {
                alt40 = 1;
            }
            else if ((LA40_0 == INT))
            {
                alt40 = 2;
            }

            else
            {
                NoViableAltException nvae =
                    new NoViableAltException("", 40, 0, input);
                throw nvae;
            }

            switch (alt40)
            {
                case 1:
                    // org\\antlr\\v4\\parse\\ANTLRParser.g:596:4: id
                    {
                        root_0 = (GrammarAST)adaptor.nil();


                        pushFollow(FOLLOW_id_in_lexerCommandExpr2401);
                        id138 = id();
                        state._fsp--;

                        adaptor.addChild(root_0, id138.getTree());

                    }
                    break;
                case 2:
                    // org\\antlr\\v4\\parse\\ANTLRParser.g:597:4: INT
                    {
                        root_0 = (GrammarAST)adaptor.nil();


                        INT139 = (Token)Match(input, INT, FOLLOW_INT_in_lexerCommandExpr2406);
                        INT139_tree = (GrammarAST)adaptor.create(INT139);
                        adaptor.addChild(root_0, INT139_tree);

                    }
                    break;

            }
            retval.stop = input.LT(-1);

            retval.tree = (GrammarAST)adaptor.rulePostProcessing(root_0);
            adaptor.setTokenBoundaries(retval.tree, retval.start, retval.stop);

        }
        catch (RecognitionException re)
        {
            reportError(re);
            recover(input, re);
            retval.tree = (GrammarAST)adaptor.errorNode(input, retval.start, input.LT(-1), re);
        }
        finally
        {
            // do for sure before leaving
        }
        return retval;
    }
    // $ANTLR end "lexerCommandExpr"


    public class lexerCommandName_return : ParserRuleReturnScope
    {
        public GrammarAST tree;
        //@Override

        public GrammarAST getTree() { return tree; }
    };


    // $ANTLR start "lexerCommandName"
    // org\\antlr\\v4\\parse\\ANTLRParser.g:600:1: lexerCommandName : ( id | MODE -> ID[$MODE] );
    public ANTLRParser.lexerCommandName_return lexerCommandName()  {
        ANTLRParser.lexerCommandName_return retval = new ANTLRParser.lexerCommandName_return();
        retval.start = input.LT(1);

        GrammarAST root_0 = null;

        Token MODE141 = null;
        ParserRuleReturnScope id140 = null;

        GrammarAST MODE141_tree = null;
        RewriteRuleTokenStream stream_MODE = new RewriteRuleTokenStream(adaptor, "token MODE");

        try
        {
            // org\\antlr\\v4\\parse\\ANTLRParser.g:601:9: ( id | MODE -> ID[$MODE] )
            int alt41 = 2;
            int LA41_0 = input.LA(1);
            if ((LA41_0 == RULE_REF || LA41_0 == TOKEN_REF))
            {
                alt41 = 1;
            }
            else if ((LA41_0 == MODE))
            {
                alt41 = 2;
            }

            else
            {
                NoViableAltException nvae =
                    new NoViableAltException("", 41, 0, input);
                throw nvae;
            }

            switch (alt41)
            {
                case 1:
                    // org\\antlr\\v4\\parse\\ANTLRParser.g:601:17: id
                    {
                        root_0 = (GrammarAST)adaptor.nil();


                        pushFollow(FOLLOW_id_in_lexerCommandName2430);
                        id140 = id();
                        state._fsp--;

                        adaptor.addChild(root_0, id140.getTree());

                    }
                    break;
                case 2:
                    // org\\antlr\\v4\\parse\\ANTLRParser.g:602:17: MODE
                    {
                        MODE141 = (Token)Match(input, MODE, FOLLOW_MODE_in_lexerCommandName2448);
                        stream_MODE.add(MODE141);


                        // AST REWRITE
                        // elements: 
                        // token labels: 
                        // rule labels: retval
                        // token list labels: 
                        // rule list labels: 
                        // wildcard labels: 
                        retval.tree = root_0;
                        RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval != null ? retval.getTree() : null);

                        root_0 = (GrammarAST)adaptor.nil();
                        // 602:25: -> ID[$MODE]
                        {
                            adaptor.addChild(root_0, (GrammarAST)adaptor.create(ID, MODE141));
                        }


                        retval.tree = root_0;

                    }
                    break;

            }
            retval.stop = input.LT(-1);

            retval.tree = (GrammarAST)adaptor.rulePostProcessing(root_0);
            adaptor.setTokenBoundaries(retval.tree, retval.start, retval.stop);

        }
        catch (RecognitionException re)
        {
            reportError(re);
            recover(input, re);
            retval.tree = (GrammarAST)adaptor.errorNode(input, retval.start, input.LT(-1), re);
        }
        finally
        {
            // do for sure before leaving
        }
        return retval;
    }
    // $ANTLR end "lexerCommandName"


    public class altList_return : ParserRuleReturnScope
    {
        public GrammarAST tree;
        //@Override

        public GrammarAST getTree() { return tree; }
    };


    // $ANTLR start "altList"
    // org\\antlr\\v4\\parse\\ANTLRParser.g:605:1: altList : alternative ( OR alternative )* -> ( alternative )+ ;
    public ANTLRParser.altList_return altList()  {
        ANTLRParser.altList_return retval = new ANTLRParser.altList_return();
        retval.start = input.LT(1);

        GrammarAST root_0 = null;

        Token OR143 = null;
        ParserRuleReturnScope alternative142 = null;
        ParserRuleReturnScope alternative144 = null;

        GrammarAST OR143_tree = null;
        RewriteRuleTokenStream stream_OR = new RewriteRuleTokenStream(adaptor, "token OR");
        RewriteRuleSubtreeStream stream_alternative = new RewriteRuleSubtreeStream(adaptor, "rule alternative");

        try
        {
            // org\\antlr\\v4\\parse\\ANTLRParser.g:606:5: ( alternative ( OR alternative )* -> ( alternative )+ )
            // org\\antlr\\v4\\parse\\ANTLRParser.g:606:7: alternative ( OR alternative )*
            {
                pushFollow(FOLLOW_alternative_in_altList2476);
                alternative142 = alternative();
                state._fsp--;

                stream_alternative.add(alternative142.getTree());
            // org\\antlr\\v4\\parse\\ANTLRParser.g:606:19: ( OR alternative )*
            loop42:
                while (true)
                {
                    int alt42 = 2;
                    int LA42_0 = input.LA(1);
                    if ((LA42_0 == OR))
                    {
                        alt42 = 1;
                    }

                    switch (alt42)
                    {
                        case 1:
                            // org\\antlr\\v4\\parse\\ANTLRParser.g:606:20: OR alternative
                            {
                                OR143 = (Token)Match(input, OR, FOLLOW_OR_in_altList2479);
                                stream_OR.add(OR143);

                                pushFollow(FOLLOW_alternative_in_altList2481);
                                alternative144 = alternative();
                                state._fsp--;

                                stream_alternative.add(alternative144.getTree());
                            }
                            break;

                        default:
                            goto exit42;
                            //break loop42;
                    }
                }
            exit42:

                // AST REWRITE
                // elements: alternative
                // token labels: 
                // rule labels: retval
                // token list labels: 
                // rule list labels: 
                // wildcard labels: 
                retval.tree = root_0;
                RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval != null ? retval.getTree() : null);

                root_0 = (GrammarAST)adaptor.nil();
                // 606:37: -> ( alternative )+
                {
                    if (!(stream_alternative.hasNext()))
                    {
                        throw new RewriteEarlyExitException();
                    }
                    while (stream_alternative.hasNext())
                    {
                        adaptor.addChild(root_0, stream_alternative.nextTree());
                    }
                    stream_alternative.reset();

                }


                retval.tree = root_0;

            }

            retval.stop = input.LT(-1);

            retval.tree = (GrammarAST)adaptor.rulePostProcessing(root_0);
            adaptor.setTokenBoundaries(retval.tree, retval.start, retval.stop);

        }
        catch (RecognitionException re)
        {
            reportError(re);
            recover(input, re);
            retval.tree = (GrammarAST)adaptor.errorNode(input, retval.start, input.LT(-1), re);
        }
        finally
        {
            // do for sure before leaving
        }
        return retval;
    }
    // $ANTLR end "altList"


    public class alternative_return : ParserRuleReturnScope
    {
        public GrammarAST tree;
        //@Override

        public GrammarAST getTree() { return tree; }
    };


    // $ANTLR start "alternative"
    // org\\antlr\\v4\\parse\\ANTLRParser.g:610:1: alternative : (o= elementOptions )? ( (e+= element )+ -> ^( ALT ( elementOptions )? ( $e)+ ) | -> ^( ALT ( elementOptions )? EPSILON ) ) ;
    public ANTLRParser.alternative_return alternative()  {
        ANTLRParser.alternative_return retval = new ANTLRParser.alternative_return();
        retval.start = input.LT(1);

        GrammarAST root_0 = null;

        List<Object> list_e = null;
        ParserRuleReturnScope o = null;
        RuleReturnScope e = null;
        RewriteRuleSubtreeStream stream_elementOptions = new RewriteRuleSubtreeStream(adaptor, "rule elementOptions");
        RewriteRuleSubtreeStream stream_element = new RewriteRuleSubtreeStream(adaptor, "rule element");

        paraphrases.push("matching alternative");
        try
        {
            // org\\antlr\\v4\\parse\\ANTLRParser.g:616:2: ( (o= elementOptions )? ( (e+= element )+ -> ^( ALT ( elementOptions )? ( $e)+ ) | -> ^( ALT ( elementOptions )? EPSILON ) ) )
            // org\\antlr\\v4\\parse\\ANTLRParser.g:616:4: (o= elementOptions )? ( (e+= element )+ -> ^( ALT ( elementOptions )? ( $e)+ ) | -> ^( ALT ( elementOptions )? EPSILON ) )
            {
                // org\\antlr\\v4\\parse\\ANTLRParser.g:616:5: (o= elementOptions )?
                int alt43 = 2;
                int LA43_0 = input.LA(1);
                if ((LA43_0 == LT))
                {
                    alt43 = 1;
                }
                switch (alt43)
                {
                    case 1:
                        // org\\antlr\\v4\\parse\\ANTLRParser.g:616:5: o= elementOptions
                        {
                            pushFollow(FOLLOW_elementOptions_in_alternative2515);
                            o = elementOptions();
                            state._fsp--;

                            stream_elementOptions.add(o.getTree());
                        }
                        break;

                }

                // org\\antlr\\v4\\parse\\ANTLRParser.g:617:3: ( (e+= element )+ -> ^( ALT ( elementOptions )? ( $e)+ ) | -> ^( ALT ( elementOptions )? EPSILON ) )
                int alt45 = 2;
                int LA45_0 = input.LA(1);
                if ((LA45_0 == ACTION || LA45_0 == DOT || LA45_0 == LPAREN || LA45_0 == NOT || LA45_0 == RULE_REF || LA45_0 == SEMPRED || LA45_0 == STRING_LITERAL || LA45_0 == TOKEN_REF))
                {
                    alt45 = 1;
                }
                else if ((LA45_0 == EOF || LA45_0 == OR || LA45_0 == POUND || LA45_0 == RPAREN || LA45_0 == SEMI))
                {
                    alt45 = 2;
                }

                else
                {
                    NoViableAltException nvae =
                        new NoViableAltException("", 45, 0, input);
                    throw nvae;
                }

                switch (alt45)
                {
                    case 1:
                        // org\\antlr\\v4\\parse\\ANTLRParser.g:617:5: (e+= element )+
                        {
                            // org\\antlr\\v4\\parse\\ANTLRParser.g:617:6: (e+= element )+
                            int cnt44 = 0;
                        loop44:
                            while (true)
                            {
                                int alt44 = 2;
                                int LA44_0 = input.LA(1);
                                if ((LA44_0 == ACTION || LA44_0 == DOT || LA44_0 == LPAREN || LA44_0 == NOT || LA44_0 == RULE_REF || LA44_0 == SEMPRED || LA44_0 == STRING_LITERAL || LA44_0 == TOKEN_REF))
                                {
                                    alt44 = 1;
                                }

                                switch (alt44)
                                {
                                    case 1:
                                        // org\\antlr\\v4\\parse\\ANTLRParser.g:617:6: e+= element
                                        {
                                            pushFollow(FOLLOW_element_in_alternative2524);
                                            e = element();
                                            state._fsp--;

                                            stream_element.add(e.getTree());
                                            if (list_e == null) list_e = new ();
                                            list_e.Add(e.getTree());
                                        }
                                        break;

                                    default:
                                        if (cnt44 >= 1) goto exit44;// break loop44;
                                        EarlyExitException eee = new EarlyExitException(44, input);
                                        throw eee;
                                }
                                cnt44++;
                            }
                        exit44:

                            // AST REWRITE
                            // elements: elementOptions, e
                            // token labels: 
                            // rule labels: retval
                            // token list labels: 
                            // rule list labels: e
                            // wildcard labels: 
                            retval.tree = root_0;
                            RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval != null ? retval.getTree() : null);
                            RewriteRuleSubtreeStream stream_e = new RewriteRuleSubtreeStream(adaptor, "token e", list_e);
                            root_0 = (GrammarAST)adaptor.nil();
                            // 617:37: -> ^( ALT ( elementOptions )? ( $e)+ )
                            {
                                // org\\antlr\\v4\\parse\\ANTLRParser.g:617:40: ^( ALT ( elementOptions )? ( $e)+ )
                                {
                                    GrammarAST root_1 = (GrammarAST)adaptor.nil();
                                    root_1 = (GrammarAST)adaptor.becomeRoot(new AltAST(ALT), root_1);
                                    // org\\antlr\\v4\\parse\\ANTLRParser.g:617:54: ( elementOptions )?
                                    if (stream_elementOptions.hasNext())
                                    {
                                        adaptor.addChild(root_1, stream_elementOptions.nextTree());
                                    }
                                    stream_elementOptions.reset();

                                    if (!(stream_e.hasNext()))
                                    {
                                        throw new RewriteEarlyExitException();
                                    }
                                    while (stream_e.hasNext())
                                    {
                                        adaptor.addChild(root_1, stream_e.nextTree());
                                    }
                                    stream_e.reset();

                                    adaptor.addChild(root_0, root_1);
                                }

                            }


                            retval.tree = root_0;

                        }
                        break;
                    case 2:
                        // org\\antlr\\v4\\parse\\ANTLRParser.g:618:39: 
                        {

                            // AST REWRITE
                            // elements: elementOptions
                            // token labels: 
                            // rule labels: retval
                            // token list labels: 
                            // rule list labels: 
                            // wildcard labels: 
                            retval.tree = root_0;
                            RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval != null ? retval.getTree() : null);

                            root_0 = (GrammarAST)adaptor.nil();
                            // 618:39: -> ^( ALT ( elementOptions )? EPSILON )
                            {
                                // org\\antlr\\v4\\parse\\ANTLRParser.g:618:42: ^( ALT ( elementOptions )? EPSILON )
                                {
                                    GrammarAST root_1 = (GrammarAST)adaptor.nil();
                                    root_1 = (GrammarAST)adaptor.becomeRoot(new AltAST(ALT), root_1);
                                    // org\\antlr\\v4\\parse\\ANTLRParser.g:618:56: ( elementOptions )?
                                    if (stream_elementOptions.hasNext())
                                    {
                                        adaptor.addChild(root_1, stream_elementOptions.nextTree());
                                    }
                                    stream_elementOptions.reset();

                                    adaptor.addChild(root_1, (GrammarAST)adaptor.create(EPSILON, "EPSILON"));
                                    adaptor.addChild(root_0, root_1);
                                }

                            }


                            retval.tree = root_0;

                        }
                        break;

                }

            }

            retval.stop = input.LT(-1);

            retval.tree = (GrammarAST)adaptor.rulePostProcessing(root_0);
            adaptor.setTokenBoundaries(retval.tree, retval.start, retval.stop);


            paraphrases.pop();
            Grammar.setNodeOptions(retval.tree, (o != null ? ((GrammarAST)o.getTree()) : null));

        }
        catch (RecognitionException re)
        {
            reportError(re);
            recover(input, re);
            retval.tree = (GrammarAST)adaptor.errorNode(input, retval.start, input.LT(-1), re);
        }
        finally
        {
            // do for sure before leaving
        }
        return retval;
    }
    // $ANTLR end "alternative"


    public class element_return : ParserRuleReturnScope
    {
        public GrammarAST tree;
        //@Override

        public GrammarAST getTree() { return tree; }
    };


    // $ANTLR start "element"
    // org\\antlr\\v4\\parse\\ANTLRParser.g:622:1: element : ( labeledElement ( ebnfSuffix -> ^( ebnfSuffix ^( BLOCK[$labeledElement.start,\"BLOCK\"] ^( ALT labeledElement ) ) ) | -> labeledElement ) | atom ( ebnfSuffix -> ^( ebnfSuffix ^( BLOCK[$atom.start,\"BLOCK\"] ^( ALT atom ) ) ) | -> atom ) | ebnf | actionElement );
    public ANTLRParser.element_return element()  {
        ANTLRParser.element_return retval = new ANTLRParser.element_return();
        retval.start = input.LT(1);

        GrammarAST root_0 = null;

        ParserRuleReturnScope labeledElement145 = null;
        ParserRuleReturnScope ebnfSuffix146 = null;
        ParserRuleReturnScope atom147 = null;
        ParserRuleReturnScope ebnfSuffix148 = null;
        ParserRuleReturnScope ebnf149 = null;
        ParserRuleReturnScope actionElement150 = null;

        RewriteRuleSubtreeStream stream_labeledElement = new RewriteRuleSubtreeStream(adaptor, "rule labeledElement");
        RewriteRuleSubtreeStream stream_atom = new RewriteRuleSubtreeStream(adaptor, "rule atom");
        RewriteRuleSubtreeStream stream_ebnfSuffix = new RewriteRuleSubtreeStream(adaptor, "rule ebnfSuffix");


        paraphrases.push("looking for rule element");
        int m = input.mark();

        try
        {
            // org\\antlr\\v4\\parse\\ANTLRParser.g:628:2: ( labeledElement ( ebnfSuffix -> ^( ebnfSuffix ^( BLOCK[$labeledElement.start,\"BLOCK\"] ^( ALT labeledElement ) ) ) | -> labeledElement ) | atom ( ebnfSuffix -> ^( ebnfSuffix ^( BLOCK[$atom.start,\"BLOCK\"] ^( ALT atom ) ) ) | -> atom ) | ebnf | actionElement )
            int alt48 = 4;
            switch (input.LA(1))
            {
                case RULE_REF:
                    {
                        int LA48_1 = input.LA(2);
                        if ((LA48_1 == ASSIGN || LA48_1 == PLUS_ASSIGN))
                        {
                            alt48 = 1;
                        }
                        else if ((LA48_1 == EOF || LA48_1 == ACTION || LA48_1 == ARG_ACTION || LA48_1 == DOT || (LA48_1 >= LPAREN && LA48_1 <= LT) || LA48_1 == NOT || LA48_1 == OR || LA48_1 == PLUS || (LA48_1 >= POUND && LA48_1 <= QUESTION) || (LA48_1 >= RPAREN && LA48_1 <= SEMPRED) || (LA48_1 >= STAR && LA48_1 <= STRING_LITERAL) || LA48_1 == TOKEN_REF))
                        {
                            alt48 = 2;
                        }

                        else
                        {
                            int nvaeMark = input.mark();
                            try
                            {
                                input.consume();
                                NoViableAltException nvae2 =
                                    new NoViableAltException("", 48, 1, input);
                                throw nvae2;
                            }
                            finally
                            {
                                input.rewind(nvaeMark);
                            }
                        }

                    }
                    break;
                case TOKEN_REF:
                    {
                        int LA48_2 = input.LA(2);
                        if ((LA48_2 == ASSIGN || LA48_2 == PLUS_ASSIGN))
                        {
                            alt48 = 1;
                        }
                        else if ((LA48_2 == EOF || LA48_2 == ACTION || LA48_2 == DOT || (LA48_2 >= LPAREN && LA48_2 <= LT) || LA48_2 == NOT || LA48_2 == OR || LA48_2 == PLUS || (LA48_2 >= POUND && LA48_2 <= QUESTION) || (LA48_2 >= RPAREN && LA48_2 <= SEMPRED) || (LA48_2 >= STAR && LA48_2 <= STRING_LITERAL) || LA48_2 == TOKEN_REF))
                        {
                            alt48 = 2;
                        }

                        else
                        {
                            int nvaeMark = input.mark();
                            try
                            {
                                input.consume();
                                NoViableAltException nvae4 =
                                    new NoViableAltException("", 48, 2, input);
                                throw nvae4;
                            }
                            finally
                            {
                                input.rewind(nvaeMark);
                            }
                        }

                    }
                    break;
                case DOT:
                case NOT:
                case STRING_LITERAL:
                    {
                        alt48 = 2;
                    }
                    break;
                case LPAREN:
                    {
                        alt48 = 3;
                    }
                    break;
                case ACTION:
                case SEMPRED:
                    {
                        alt48 = 4;
                    }
                    break;
                default:
                    NoViableAltException nvae =
                        new NoViableAltException("", 48, 0, input);
                    throw nvae;
            }
            switch (alt48)
            {
                case 1:
                    // org\\antlr\\v4\\parse\\ANTLRParser.g:628:4: labeledElement ( ebnfSuffix -> ^( ebnfSuffix ^( BLOCK[$labeledElement.start,\"BLOCK\"] ^( ALT labeledElement ) ) ) | -> labeledElement )
                    {
                        pushFollow(FOLLOW_labeledElement_in_element2639);
                        labeledElement145 = labeledElement();
                        state._fsp--;

                        stream_labeledElement.add(labeledElement145.getTree());
                        // org\\antlr\\v4\\parse\\ANTLRParser.g:629:3: ( ebnfSuffix -> ^( ebnfSuffix ^( BLOCK[$labeledElement.start,\"BLOCK\"] ^( ALT labeledElement ) ) ) | -> labeledElement )
                        int alt46 = 2;
                        int LA46_0 = input.LA(1);
                        if ((LA46_0 == PLUS || LA46_0 == QUESTION || LA46_0 == STAR))
                        {
                            alt46 = 1;
                        }
                        else if ((LA46_0 == EOF || LA46_0 == ACTION || LA46_0 == DOT || LA46_0 == LPAREN || LA46_0 == NOT || LA46_0 == OR || LA46_0 == POUND || (LA46_0 >= RPAREN && LA46_0 <= SEMPRED) || LA46_0 == STRING_LITERAL || LA46_0 == TOKEN_REF))
                        {
                            alt46 = 2;
                        }

                        else
                        {
                            NoViableAltException nvae =
                                new NoViableAltException("", 46, 0, input);
                            throw nvae;
                        }

                        switch (alt46)
                        {
                            case 1:
                                // org\\antlr\\v4\\parse\\ANTLRParser.g:629:5: ebnfSuffix
                                {
                                    pushFollow(FOLLOW_ebnfSuffix_in_element2645);
                                    ebnfSuffix146 = ebnfSuffix();
                                    state._fsp--;

                                    stream_ebnfSuffix.add(ebnfSuffix146.getTree());

                                    // AST REWRITE
                                    // elements: labeledElement, ebnfSuffix
                                    // token labels: 
                                    // rule labels: retval
                                    // token list labels: 
                                    // rule list labels: 
                                    // wildcard labels: 
                                    retval.tree = root_0;
                                    RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval != null ? retval.getTree() : null);

                                    root_0 = (GrammarAST)adaptor.nil();
                                    // 629:16: -> ^( ebnfSuffix ^( BLOCK[$labeledElement.start,\"BLOCK\"] ^( ALT labeledElement ) ) )
                                    {
                                        // org\\antlr\\v4\\parse\\ANTLRParser.g:629:19: ^( ebnfSuffix ^( BLOCK[$labeledElement.start,\"BLOCK\"] ^( ALT labeledElement ) ) )
                                        {
                                            GrammarAST root_1 = (GrammarAST)adaptor.nil();
                                            root_1 = (GrammarAST)adaptor.becomeRoot(stream_ebnfSuffix.nextNode(), root_1);
                                            // org\\antlr\\v4\\parse\\ANTLRParser.g:629:33: ^( BLOCK[$labeledElement.start,\"BLOCK\"] ^( ALT labeledElement ) )
                                            {
                                                GrammarAST root_2 = (GrammarAST)adaptor.nil();
                                                root_2 = (GrammarAST)adaptor.becomeRoot(new BlockAST(BLOCK, (labeledElement145 != null ? (labeledElement145.start) : null), "BLOCK"), root_2);
                                                // org\\antlr\\v4\\parse\\ANTLRParser.g:629:82: ^( ALT labeledElement )
                                                {
                                                    GrammarAST root_3 = (GrammarAST)adaptor.nil();
                                                    root_3 = (GrammarAST)adaptor.becomeRoot(new AltAST(ALT), root_3);
                                                    adaptor.addChild(root_3, stream_labeledElement.nextTree());
                                                    adaptor.addChild(root_2, root_3);
                                                }

                                                adaptor.addChild(root_1, root_2);
                                            }

                                            adaptor.addChild(root_0, root_1);
                                        }

                                    }


                                    retval.tree = root_0;

                                }
                                break;
                            case 2:
                                // org\\antlr\\v4\\parse\\ANTLRParser.g:630:8: 
                                {

                                    // AST REWRITE
                                    // elements: labeledElement
                                    // token labels: 
                                    // rule labels: retval
                                    // token list labels: 
                                    // rule list labels: 
                                    // wildcard labels: 
                                    retval.tree = root_0;
                                    RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval != null ? retval.getTree() : null);

                                    root_0 = (GrammarAST)adaptor.nil();
                                    // 630:8: -> labeledElement
                                    {
                                        adaptor.addChild(root_0, stream_labeledElement.nextTree());
                                    }


                                    retval.tree = root_0;

                                }
                                break;

                        }

                    }
                    break;
                case 2:
                    // org\\antlr\\v4\\parse\\ANTLRParser.g:632:4: atom ( ebnfSuffix -> ^( ebnfSuffix ^( BLOCK[$atom.start,\"BLOCK\"] ^( ALT atom ) ) ) | -> atom )
                    {
                        pushFollow(FOLLOW_atom_in_element2691);
                        atom147 = atom();
                        state._fsp--;

                        stream_atom.add(atom147.getTree());
                        // org\\antlr\\v4\\parse\\ANTLRParser.g:633:3: ( ebnfSuffix -> ^( ebnfSuffix ^( BLOCK[$atom.start,\"BLOCK\"] ^( ALT atom ) ) ) | -> atom )
                        int alt47 = 2;
                        int LA47_0 = input.LA(1);
                        if ((LA47_0 == PLUS || LA47_0 == QUESTION || LA47_0 == STAR))
                        {
                            alt47 = 1;
                        }
                        else if ((LA47_0 == EOF || LA47_0 == ACTION || LA47_0 == DOT || LA47_0 == LPAREN || LA47_0 == NOT || LA47_0 == OR || LA47_0 == POUND || (LA47_0 >= RPAREN && LA47_0 <= SEMPRED) || LA47_0 == STRING_LITERAL || LA47_0 == TOKEN_REF))
                        {
                            alt47 = 2;
                        }

                        else
                        {
                            NoViableAltException nvae =
                                new NoViableAltException("", 47, 0, input);
                            throw nvae;
                        }

                        switch (alt47)
                        {
                            case 1:
                                // org\\antlr\\v4\\parse\\ANTLRParser.g:633:5: ebnfSuffix
                                {
                                    pushFollow(FOLLOW_ebnfSuffix_in_element2697);
                                    ebnfSuffix148 = ebnfSuffix();
                                    state._fsp--;

                                    stream_ebnfSuffix.add(ebnfSuffix148.getTree());

                                    // AST REWRITE
                                    // elements: atom, ebnfSuffix
                                    // token labels: 
                                    // rule labels: retval
                                    // token list labels: 
                                    // rule list labels: 
                                    // wildcard labels: 
                                    retval.tree = root_0;
                                    RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval != null ? retval.getTree() : null);

                                    root_0 = (GrammarAST)adaptor.nil();
                                    // 633:16: -> ^( ebnfSuffix ^( BLOCK[$atom.start,\"BLOCK\"] ^( ALT atom ) ) )
                                    {
                                        // org\\antlr\\v4\\parse\\ANTLRParser.g:633:19: ^( ebnfSuffix ^( BLOCK[$atom.start,\"BLOCK\"] ^( ALT atom ) ) )
                                        {
                                            GrammarAST root_1 = (GrammarAST)adaptor.nil();
                                            root_1 = (GrammarAST)adaptor.becomeRoot(stream_ebnfSuffix.nextNode(), root_1);
                                            // org\\antlr\\v4\\parse\\ANTLRParser.g:633:33: ^( BLOCK[$atom.start,\"BLOCK\"] ^( ALT atom ) )
                                            {
                                                GrammarAST root_2 = (GrammarAST)adaptor.nil();
                                                root_2 = (GrammarAST)adaptor.becomeRoot(new BlockAST(BLOCK, (atom147 != null ? (atom147.start) : null), "BLOCK"), root_2);
                                                // org\\antlr\\v4\\parse\\ANTLRParser.g:633:72: ^( ALT atom )
                                                {
                                                    GrammarAST root_3 = (GrammarAST)adaptor.nil();
                                                    root_3 = (GrammarAST)adaptor.becomeRoot(new AltAST(ALT), root_3);
                                                    adaptor.addChild(root_3, stream_atom.nextTree());
                                                    adaptor.addChild(root_2, root_3);
                                                }

                                                adaptor.addChild(root_1, root_2);
                                            }

                                            adaptor.addChild(root_0, root_1);
                                        }

                                    }


                                    retval.tree = root_0;

                                }
                                break;
                            case 2:
                                // org\\antlr\\v4\\parse\\ANTLRParser.g:634:8: 
                                {

                                    // AST REWRITE
                                    // elements: atom
                                    // token labels: 
                                    // rule labels: retval
                                    // token list labels: 
                                    // rule list labels: 
                                    // wildcard labels: 
                                    retval.tree = root_0;
                                    RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval != null ? retval.getTree() : null);

                                    root_0 = (GrammarAST)adaptor.nil();
                                    // 634:8: -> atom
                                    {
                                        adaptor.addChild(root_0, stream_atom.nextTree());
                                    }


                                    retval.tree = root_0;

                                }
                                break;

                        }

                    }
                    break;
                case 3:
                    // org\\antlr\\v4\\parse\\ANTLRParser.g:636:4: ebnf
                    {
                        root_0 = (GrammarAST)adaptor.nil();


                        pushFollow(FOLLOW_ebnf_in_element2743);
                        ebnf149 = ebnf();
                        state._fsp--;

                        adaptor.addChild(root_0, ebnf149.getTree());

                    }
                    break;
                case 4:
                    // org\\antlr\\v4\\parse\\ANTLRParser.g:637:4: actionElement
                    {
                        root_0 = (GrammarAST)adaptor.nil();


                        pushFollow(FOLLOW_actionElement_in_element2748);
                        actionElement150 = actionElement();
                        state._fsp--;

                        adaptor.addChild(root_0, actionElement150.getTree());

                    }
                    break;

            }
            retval.stop = input.LT(-1);

            retval.tree = (GrammarAST)adaptor.rulePostProcessing(root_0);
            adaptor.setTokenBoundaries(retval.tree, retval.start, retval.stop);

            paraphrases.pop();
        }
        catch (RecognitionException re)
        {

            retval.tree = (GrammarAST)adaptor.errorNode(input, retval.start, input.LT(-1), re);
            int ttype = input.get(input.range()).getType();
            // look for anything that really belongs at the start of the rule minus the initial ID
            if (ttype == COLON || ttype == RETURNS || ttype == CATCH || ttype == FINALLY || ttype == AT)
            {
                RecognitionException missingSemi =
                    new v4ParserException("unterminated rule (missing ';') detected at '" +
                                          input.LT(1).getText() + " " + input.LT(2).getText() + "'", input);
                reportError(missingSemi);
                if (ttype == CATCH || ttype == FINALLY)
                {
                    input.seek(input.range()); // ignore what's before rule trailer stuff
                }
                if (ttype == RETURNS || ttype == AT)
                { // scan back looking for ID of rule header
                    int p = input.index();
                    Token t = input.get(p);
                    while (t.getType() != RULE_REF && t.getType() != TOKEN_REF)
                    {
                        p--;
                        t = input.get(p);
                    }
                    input.seek(p);
                }
                throw new ResyncToEndOfRuleBlock(); // make sure it goes back to rule block level to recover
            }
            reportError(re);
            recover(input, re);

        }

        finally
        {
            // do for sure before leaving
        }
        return retval;
    }
    // $ANTLR end "element"


    public class actionElement_return : ParserRuleReturnScope
    {
        public GrammarAST tree;
        //@Override

        public GrammarAST getTree() { return tree; }
    };


    // $ANTLR start "actionElement"
    // org\\antlr\\v4\\parse\\ANTLRParser.g:666:1: actionElement : ( ACTION | ACTION elementOptions -> ^( ACTION elementOptions ) | SEMPRED | SEMPRED elementOptions -> ^( SEMPRED elementOptions ) );
    public ANTLRParser.actionElement_return actionElement()  {
        ANTLRParser.actionElement_return retval = new ANTLRParser.actionElement_return();
        retval.start = input.LT(1);

        GrammarAST root_0 = null;

        Token ACTION151 = null;
        Token ACTION152 = null;
        Token SEMPRED154 = null;
        Token SEMPRED155 = null;
        ParserRuleReturnScope elementOptions153 = null;
        ParserRuleReturnScope elementOptions156 = null;

        GrammarAST ACTION151_tree = null;
        GrammarAST ACTION152_tree = null;
        GrammarAST SEMPRED154_tree = null;
        GrammarAST SEMPRED155_tree = null;
        RewriteRuleTokenStream stream_ACTION = new RewriteRuleTokenStream(adaptor, "token ACTION");
        RewriteRuleTokenStream stream_SEMPRED = new RewriteRuleTokenStream(adaptor, "token SEMPRED");
        RewriteRuleSubtreeStream stream_elementOptions = new RewriteRuleSubtreeStream(adaptor, "rule elementOptions");

        try
        {
            // org\\antlr\\v4\\parse\\ANTLRParser.g:673:2: ( ACTION | ACTION elementOptions -> ^( ACTION elementOptions ) | SEMPRED | SEMPRED elementOptions -> ^( SEMPRED elementOptions ) )
            int alt49 = 4;
            int LA49_0 = input.LA(1);
            if ((LA49_0 == ACTION))
            {
                int LA49_1 = input.LA(2);
                if ((LA49_1 == EOF || LA49_1 == ACTION || LA49_1 == DOT || LA49_1 == LEXER_CHAR_SET || LA49_1 == LPAREN || LA49_1 == NOT || LA49_1 == OR || LA49_1 == POUND || LA49_1 == RARROW || (LA49_1 >= RPAREN && LA49_1 <= SEMPRED) || LA49_1 == STRING_LITERAL || LA49_1 == TOKEN_REF))
                {
                    alt49 = 1;
                }
                else if ((LA49_1 == LT))
                {
                    alt49 = 2;
                }

                else
                {
                    int nvaeMark = input.mark();
                    try
                    {
                        input.consume();
                        NoViableAltException nvae =
                            new NoViableAltException("", 49, 1, input);
                        throw nvae;
                    }
                    finally
                    {
                        input.rewind(nvaeMark);
                    }
                }

            }
            else if ((LA49_0 == SEMPRED))
            {
                int LA49_2 = input.LA(2);
                if ((LA49_2 == EOF || LA49_2 == ACTION || LA49_2 == DOT || LA49_2 == LEXER_CHAR_SET || LA49_2 == LPAREN || LA49_2 == NOT || LA49_2 == OR || LA49_2 == POUND || LA49_2 == RARROW || (LA49_2 >= RPAREN && LA49_2 <= SEMPRED) || LA49_2 == STRING_LITERAL || LA49_2 == TOKEN_REF))
                {
                    alt49 = 3;
                }
                else if ((LA49_2 == LT))
                {
                    alt49 = 4;
                }

                else
                {
                    int nvaeMark = input.mark();
                    try
                    {
                        input.consume();
                        NoViableAltException nvae =
                            new NoViableAltException("", 49, 2, input);
                        throw nvae;
                    }
                    finally
                    {
                        input.rewind(nvaeMark);
                    }
                }

            }

            else
            {
                NoViableAltException nvae =
                    new NoViableAltException("", 49, 0, input);
                throw nvae;
            }

            switch (alt49)
            {
                case 1:
                    // org\\antlr\\v4\\parse\\ANTLRParser.g:673:4: ACTION
                    {
                        root_0 = (GrammarAST)adaptor.nil();


                        ACTION151 = (Token)Match(input, ACTION, FOLLOW_ACTION_in_actionElement2774);
                        ACTION151_tree = new ActionAST(ACTION151);
                        adaptor.addChild(root_0, ACTION151_tree);

                    }
                    break;
                case 2:
                    // org\\antlr\\v4\\parse\\ANTLRParser.g:674:6: ACTION elementOptions
                    {
                        ACTION152 = (Token)Match(input, ACTION, FOLLOW_ACTION_in_actionElement2784);
                        stream_ACTION.add(ACTION152);

                        pushFollow(FOLLOW_elementOptions_in_actionElement2786);
                        elementOptions153 = elementOptions();
                        state._fsp--;

                        stream_elementOptions.add(elementOptions153.getTree());

                        // AST REWRITE
                        // elements: ACTION, elementOptions
                        // token labels: 
                        // rule labels: retval
                        // token list labels: 
                        // rule list labels: 
                        // wildcard labels: 
                        retval.tree = root_0;
                        RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval != null ? retval.getTree() : null);

                        root_0 = (GrammarAST)adaptor.nil();
                        // 674:28: -> ^( ACTION elementOptions )
                        {
                            // org\\antlr\\v4\\parse\\ANTLRParser.g:674:31: ^( ACTION elementOptions )
                            {
                                GrammarAST root_1 = (GrammarAST)adaptor.nil();
                                root_1 = (GrammarAST)adaptor.becomeRoot(new ActionAST(stream_ACTION.nextToken()), root_1);
                                adaptor.addChild(root_1, stream_elementOptions.nextTree());
                                adaptor.addChild(root_0, root_1);
                            }

                        }


                        retval.tree = root_0;

                    }
                    break;
                case 3:
                    // org\\antlr\\v4\\parse\\ANTLRParser.g:675:6: SEMPRED
                    {
                        root_0 = (GrammarAST)adaptor.nil();


                        SEMPRED154 = (Token)Match(input, SEMPRED, FOLLOW_SEMPRED_in_actionElement2804);
                        SEMPRED154_tree = new PredAST(SEMPRED154);
                        adaptor.addChild(root_0, SEMPRED154_tree);

                    }
                    break;
                case 4:
                    // org\\antlr\\v4\\parse\\ANTLRParser.g:676:6: SEMPRED elementOptions
                    {
                        SEMPRED155 = (Token)Match(input, SEMPRED, FOLLOW_SEMPRED_in_actionElement2814);
                        stream_SEMPRED.add(SEMPRED155);

                        pushFollow(FOLLOW_elementOptions_in_actionElement2816);
                        elementOptions156 = elementOptions();
                        state._fsp--;

                        stream_elementOptions.add(elementOptions156.getTree());

                        // AST REWRITE
                        // elements: elementOptions, SEMPRED
                        // token labels: 
                        // rule labels: retval
                        // token list labels: 
                        // rule list labels: 
                        // wildcard labels: 
                        retval.tree = root_0;
                        RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval != null ? retval.getTree() : null);

                        root_0 = (GrammarAST)adaptor.nil();
                        // 676:29: -> ^( SEMPRED elementOptions )
                        {
                            // org\\antlr\\v4\\parse\\ANTLRParser.g:676:32: ^( SEMPRED elementOptions )
                            {
                                GrammarAST root_1 = (GrammarAST)adaptor.nil();
                                root_1 = (GrammarAST)adaptor.becomeRoot(new PredAST(stream_SEMPRED.nextToken()), root_1);
                                adaptor.addChild(root_1, stream_elementOptions.nextTree());
                                adaptor.addChild(root_0, root_1);
                            }

                        }


                        retval.tree = root_0;

                    }
                    break;

            }
            retval.stop = input.LT(-1);

            retval.tree = (GrammarAST)adaptor.rulePostProcessing(root_0);
            adaptor.setTokenBoundaries(retval.tree, retval.start, retval.stop);


            GrammarAST options = (GrammarAST)retval.tree.getFirstChildWithType(ANTLRParser.ELEMENT_OPTIONS);
            if (options != null)
            {
                Grammar.setNodeOptions(retval.tree, options);
            }

        }
        catch (RecognitionException re)
        {
            reportError(re);
            recover(input, re);
            retval.tree = (GrammarAST)adaptor.errorNode(input, retval.start, input.LT(-1), re);
        }
        finally
        {
            // do for sure before leaving
        }
        return retval;
    }
    // $ANTLR end "actionElement"


    public class labeledElement_return : ParserRuleReturnScope
    {
        public GrammarAST tree;
        //@Override

        public GrammarAST getTree() { return tree; }
    };


    // $ANTLR start "labeledElement"
    // org\\antlr\\v4\\parse\\ANTLRParser.g:679:1: labeledElement : id (ass= ASSIGN |ass= PLUS_ASSIGN ) ( atom -> ^( $ass id atom ) | block -> ^( $ass id block ) ) ;
    public ANTLRParser.labeledElement_return labeledElement()  {
        ANTLRParser.labeledElement_return retval = new ANTLRParser.labeledElement_return();
        retval.start = input.LT(1);

        GrammarAST root_0 = null;

        Token ass = null;
        ParserRuleReturnScope id157 = null;
        ParserRuleReturnScope atom158 = null;
        ParserRuleReturnScope block159 = null;

        GrammarAST ass_tree = null;
        RewriteRuleTokenStream stream_PLUS_ASSIGN = new RewriteRuleTokenStream(adaptor, "token PLUS_ASSIGN");
        RewriteRuleTokenStream stream_ASSIGN = new RewriteRuleTokenStream(adaptor, "token ASSIGN");
        RewriteRuleSubtreeStream stream_block = new RewriteRuleSubtreeStream(adaptor, "rule block");
        RewriteRuleSubtreeStream stream_id = new RewriteRuleSubtreeStream(adaptor, "rule id");
        RewriteRuleSubtreeStream stream_atom = new RewriteRuleSubtreeStream(adaptor, "rule atom");

        try
        {
            // org\\antlr\\v4\\parse\\ANTLRParser.g:680:2: ( id (ass= ASSIGN |ass= PLUS_ASSIGN ) ( atom -> ^( $ass id atom ) | block -> ^( $ass id block ) ) )
            // org\\antlr\\v4\\parse\\ANTLRParser.g:680:4: id (ass= ASSIGN |ass= PLUS_ASSIGN ) ( atom -> ^( $ass id atom ) | block -> ^( $ass id block ) )
            {
                pushFollow(FOLLOW_id_in_labeledElement2838);
                id157 = id();
                state._fsp--;

                stream_id.add(id157.getTree());
                // org\\antlr\\v4\\parse\\ANTLRParser.g:680:7: (ass= ASSIGN |ass= PLUS_ASSIGN )
                int alt50 = 2;
                int LA50_0 = input.LA(1);
                if ((LA50_0 == ASSIGN))
                {
                    alt50 = 1;
                }
                else if ((LA50_0 == PLUS_ASSIGN))
                {
                    alt50 = 2;
                }

                else
                {
                    NoViableAltException nvae =
                        new NoViableAltException("", 50, 0, input);
                    throw nvae;
                }

                switch (alt50)
                {
                    case 1:
                        // org\\antlr\\v4\\parse\\ANTLRParser.g:680:8: ass= ASSIGN
                        {
                            ass = (Token)Match(input, ASSIGN, FOLLOW_ASSIGN_in_labeledElement2843);
                            stream_ASSIGN.add(ass);

                        }
                        break;
                    case 2:
                        // org\\antlr\\v4\\parse\\ANTLRParser.g:680:19: ass= PLUS_ASSIGN
                        {
                            ass = (Token)Match(input, PLUS_ASSIGN, FOLLOW_PLUS_ASSIGN_in_labeledElement2847);
                            stream_PLUS_ASSIGN.add(ass);

                        }
                        break;

                }

                // org\\antlr\\v4\\parse\\ANTLRParser.g:681:3: ( atom -> ^( $ass id atom ) | block -> ^( $ass id block ) )
                int alt51 = 2;
                int LA51_0 = input.LA(1);
                if ((LA51_0 == DOT || LA51_0 == NOT || LA51_0 == RULE_REF || LA51_0 == STRING_LITERAL || LA51_0 == TOKEN_REF))
                {
                    alt51 = 1;
                }
                else if ((LA51_0 == LPAREN))
                {
                    alt51 = 2;
                }

                else
                {
                    NoViableAltException nvae =
                        new NoViableAltException("", 51, 0, input);
                    throw nvae;
                }

                switch (alt51)
                {
                    case 1:
                        // org\\antlr\\v4\\parse\\ANTLRParser.g:681:5: atom
                        {
                            pushFollow(FOLLOW_atom_in_labeledElement2854);
                            atom158 = atom();
                            state._fsp--;

                            stream_atom.add(atom158.getTree());

                            // AST REWRITE
                            // elements: ass, id, atom
                            // token labels: ass
                            // rule labels: retval
                            // token list labels: 
                            // rule list labels: 
                            // wildcard labels: 
                            retval.tree = root_0;
                            RewriteRuleTokenStream stream_ass = new RewriteRuleTokenStream(adaptor, "token ass", ass);
                            RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval != null ? retval.getTree() : null);

                            root_0 = (GrammarAST)adaptor.nil();
                            // 681:15: -> ^( $ass id atom )
                            {
                                // org\\antlr\\v4\\parse\\ANTLRParser.g:681:18: ^( $ass id atom )
                                {
                                    GrammarAST root_1 = (GrammarAST)adaptor.nil();
                                    root_1 = (GrammarAST)adaptor.becomeRoot(stream_ass.nextNode(), root_1);
                                    adaptor.addChild(root_1, stream_id.nextTree());
                                    adaptor.addChild(root_1, stream_atom.nextTree());
                                    adaptor.addChild(root_0, root_1);
                                }

                            }


                            retval.tree = root_0;

                        }
                        break;
                    case 2:
                        // org\\antlr\\v4\\parse\\ANTLRParser.g:682:5: block
                        {
                            pushFollow(FOLLOW_block_in_labeledElement2876);
                            block159 = block();
                            state._fsp--;

                            stream_block.add(block159.getTree());

                            // AST REWRITE
                            // elements: ass, block, id
                            // token labels: ass
                            // rule labels: retval
                            // token list labels: 
                            // rule list labels: 
                            // wildcard labels: 
                            retval.tree = root_0;
                            RewriteRuleTokenStream stream_ass = new RewriteRuleTokenStream(adaptor, "token ass", ass);
                            RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval != null ? retval.getTree() : null);

                            root_0 = (GrammarAST)adaptor.nil();
                            // 682:16: -> ^( $ass id block )
                            {
                                // org\\antlr\\v4\\parse\\ANTLRParser.g:682:19: ^( $ass id block )
                                {
                                    GrammarAST root_1 = (GrammarAST)adaptor.nil();
                                    root_1 = (GrammarAST)adaptor.becomeRoot(stream_ass.nextNode(), root_1);
                                    adaptor.addChild(root_1, stream_id.nextTree());
                                    adaptor.addChild(root_1, stream_block.nextTree());
                                    adaptor.addChild(root_0, root_1);
                                }

                            }


                            retval.tree = root_0;

                        }
                        break;

                }

            }

            retval.stop = input.LT(-1);

            retval.tree = (GrammarAST)adaptor.rulePostProcessing(root_0);
            adaptor.setTokenBoundaries(retval.tree, retval.start, retval.stop);

        }
        catch (RecognitionException re)
        {
            reportError(re);
            recover(input, re);
            retval.tree = (GrammarAST)adaptor.errorNode(input, retval.start, input.LT(-1), re);
        }
        finally
        {
            // do for sure before leaving
        }
        return retval;
    }
    // $ANTLR end "labeledElement"


    public class ebnf_return : ParserRuleReturnScope
    {
        public GrammarAST tree;
        //@Override

        public GrammarAST getTree() { return tree; }
    };


    // $ANTLR start "ebnf"
    // org\\antlr\\v4\\parse\\ANTLRParser.g:688:1: ebnf : block ( blockSuffix -> ^( blockSuffix block ) | -> block ) ;
    public ANTLRParser.ebnf_return ebnf()  {
        ANTLRParser.ebnf_return retval = new ANTLRParser.ebnf_return();
        retval.start = input.LT(1);

        GrammarAST root_0 = null;

        ParserRuleReturnScope block160 = null;
        ParserRuleReturnScope blockSuffix161 = null;

        RewriteRuleSubtreeStream stream_blockSuffix = new RewriteRuleSubtreeStream(adaptor, "rule blockSuffix");
        RewriteRuleSubtreeStream stream_block = new RewriteRuleSubtreeStream(adaptor, "rule block");

        try
        {
            // org\\antlr\\v4\\parse\\ANTLRParser.g:689:5: ( block ( blockSuffix -> ^( blockSuffix block ) | -> block ) )
            // org\\antlr\\v4\\parse\\ANTLRParser.g:689:7: block ( blockSuffix -> ^( blockSuffix block ) | -> block )
            {
                pushFollow(FOLLOW_block_in_ebnf2912);
                block160 = block();
                state._fsp--;

                stream_block.add(block160.getTree());
                // org\\antlr\\v4\\parse\\ANTLRParser.g:692:7: ( blockSuffix -> ^( blockSuffix block ) | -> block )
                int alt52 = 2;
                int LA52_0 = input.LA(1);
                if ((LA52_0 == PLUS || LA52_0 == QUESTION || LA52_0 == STAR))
                {
                    alt52 = 1;
                }
                else if ((LA52_0 == EOF || LA52_0 == ACTION || LA52_0 == DOT || LA52_0 == LPAREN || LA52_0 == NOT || LA52_0 == OR || LA52_0 == POUND || (LA52_0 >= RPAREN && LA52_0 <= SEMPRED) || LA52_0 == STRING_LITERAL || LA52_0 == TOKEN_REF))
                {
                    alt52 = 2;
                }

                else
                {
                    NoViableAltException nvae =
                        new NoViableAltException("", 52, 0, input);
                    throw nvae;
                }

                switch (alt52)
                {
                    case 1:
                        // org\\antlr\\v4\\parse\\ANTLRParser.g:692:9: blockSuffix
                        {
                            pushFollow(FOLLOW_blockSuffix_in_ebnf2936);
                            blockSuffix161 = blockSuffix();
                            state._fsp--;

                            stream_blockSuffix.add(blockSuffix161.getTree());

                            // AST REWRITE
                            // elements: blockSuffix, block
                            // token labels: 
                            // rule labels: retval
                            // token list labels: 
                            // rule list labels: 
                            // wildcard labels: 
                            retval.tree = root_0;
                            RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval != null ? retval.getTree() : null);

                            root_0 = (GrammarAST)adaptor.nil();
                            // 692:21: -> ^( blockSuffix block )
                            {
                                // org\\antlr\\v4\\parse\\ANTLRParser.g:692:24: ^( blockSuffix block )
                                {
                                    GrammarAST root_1 = (GrammarAST)adaptor.nil();
                                    root_1 = (GrammarAST)adaptor.becomeRoot(stream_blockSuffix.nextNode(), root_1);
                                    adaptor.addChild(root_1, stream_block.nextTree());
                                    adaptor.addChild(root_0, root_1);
                                }

                            }


                            retval.tree = root_0;

                        }
                        break;
                    case 2:
                        // org\\antlr\\v4\\parse\\ANTLRParser.g:693:12: 
                        {

                            // AST REWRITE
                            // elements: block
                            // token labels: 
                            // rule labels: retval
                            // token list labels: 
                            // rule list labels: 
                            // wildcard labels: 
                            retval.tree = root_0;
                            RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval != null ? retval.getTree() : null);

                            root_0 = (GrammarAST)adaptor.nil();
                            // 693:12: -> block
                            {
                                adaptor.addChild(root_0, stream_block.nextTree());
                            }


                            retval.tree = root_0;

                        }
                        break;

                }

            }

            retval.stop = input.LT(-1);

            retval.tree = (GrammarAST)adaptor.rulePostProcessing(root_0);
            adaptor.setTokenBoundaries(retval.tree, retval.start, retval.stop);

        }
        catch (RecognitionException re)
        {
            reportError(re);
            recover(input, re);
            retval.tree = (GrammarAST)adaptor.errorNode(input, retval.start, input.LT(-1), re);
        }
        finally
        {
            // do for sure before leaving
        }
        return retval;
    }
    // $ANTLR end "ebnf"


    public class blockSuffix_return : ParserRuleReturnScope
    {
        public GrammarAST tree;
        //@Override

        public GrammarAST getTree() { return tree; }
    };


    // $ANTLR start "blockSuffix"
    // org\\antlr\\v4\\parse\\ANTLRParser.g:699:1: blockSuffix : ebnfSuffix ;
    public ANTLRParser.blockSuffix_return blockSuffix()  {
        ANTLRParser.blockSuffix_return retval = new ANTLRParser.blockSuffix_return();
        retval.start = input.LT(1);

        GrammarAST root_0 = null;

        ParserRuleReturnScope ebnfSuffix162 = null;


        try
        {
            // org\\antlr\\v4\\parse\\ANTLRParser.g:700:5: ( ebnfSuffix )
            // org\\antlr\\v4\\parse\\ANTLRParser.g:700:7: ebnfSuffix
            {
                root_0 = (GrammarAST)adaptor.nil();


                pushFollow(FOLLOW_ebnfSuffix_in_blockSuffix2986);
                ebnfSuffix162 = ebnfSuffix();
                state._fsp--;

                adaptor.addChild(root_0, ebnfSuffix162.getTree());

            }

            retval.stop = input.LT(-1);

            retval.tree = (GrammarAST)adaptor.rulePostProcessing(root_0);
            adaptor.setTokenBoundaries(retval.tree, retval.start, retval.stop);

        }
        catch (RecognitionException re)
        {
            reportError(re);
            recover(input, re);
            retval.tree = (GrammarAST)adaptor.errorNode(input, retval.start, input.LT(-1), re);
        }
        finally
        {
            // do for sure before leaving
        }
        return retval;
    }
    // $ANTLR end "blockSuffix"


    public class ebnfSuffix_return : ParserRuleReturnScope
    {
        public GrammarAST tree;
        //@Override

        public GrammarAST getTree() { return tree; }
    };


    // $ANTLR start "ebnfSuffix"
    // org\\antlr\\v4\\parse\\ANTLRParser.g:703:1: ebnfSuffix : ( QUESTION (nongreedy= QUESTION )? -> OPTIONAL[$start, $nongreedy] | STAR (nongreedy= QUESTION )? -> CLOSURE[$start, $nongreedy] | PLUS (nongreedy= QUESTION )? -> POSITIVE_CLOSURE[$start, $nongreedy] );
    public ANTLRParser.ebnfSuffix_return ebnfSuffix()  {
        ANTLRParser.ebnfSuffix_return retval = new ANTLRParser.ebnfSuffix_return();
        retval.start = input.LT(1);

        GrammarAST root_0 = null;

        Token nongreedy = null;
        Token QUESTION163 = null;
        Token STAR164 = null;
        Token PLUS165 = null;

        GrammarAST nongreedy_tree = null;
        GrammarAST QUESTION163_tree = null;
        GrammarAST STAR164_tree = null;
        GrammarAST PLUS165_tree = null;
        RewriteRuleTokenStream stream_STAR = new RewriteRuleTokenStream(adaptor, "token STAR");
        RewriteRuleTokenStream stream_QUESTION = new RewriteRuleTokenStream(adaptor, "token QUESTION");
        RewriteRuleTokenStream stream_PLUS = new RewriteRuleTokenStream(adaptor, "token PLUS");

        try
        {
            // org\\antlr\\v4\\parse\\ANTLRParser.g:704:2: ( QUESTION (nongreedy= QUESTION )? -> OPTIONAL[$start, $nongreedy] | STAR (nongreedy= QUESTION )? -> CLOSURE[$start, $nongreedy] | PLUS (nongreedy= QUESTION )? -> POSITIVE_CLOSURE[$start, $nongreedy] )
            int alt56 = 3;
            switch (input.LA(1))
            {
                case QUESTION:
                    {
                        alt56 = 1;
                    }
                    break;
                case STAR:
                    {
                        alt56 = 2;
                    }
                    break;
                case PLUS:
                    {
                        alt56 = 3;
                    }
                    break;
                default:
                    NoViableAltException nvae =
                        new NoViableAltException("", 56, 0, input);
                    throw nvae;
            }
            switch (alt56)
            {
                case 1:
                    // org\\antlr\\v4\\parse\\ANTLRParser.g:704:4: QUESTION (nongreedy= QUESTION )?
                    {
                        QUESTION163 = (Token)Match(input, QUESTION, FOLLOW_QUESTION_in_ebnfSuffix3001);
                        stream_QUESTION.add(QUESTION163);

                        // org\\antlr\\v4\\parse\\ANTLRParser.g:704:22: (nongreedy= QUESTION )?
                        int alt53 = 2;
                        int LA53_0 = input.LA(1);
                        if ((LA53_0 == QUESTION))
                        {
                            alt53 = 1;
                        }
                        switch (alt53)
                        {
                            case 1:
                                // org\\antlr\\v4\\parse\\ANTLRParser.g:704:22: nongreedy= QUESTION
                                {
                                    nongreedy = (Token)Match(input, QUESTION, FOLLOW_QUESTION_in_ebnfSuffix3005);
                                    stream_QUESTION.add(nongreedy);

                                }
                                break;

                        }


                        // AST REWRITE
                        // elements: 
                        // token labels: 
                        // rule labels: retval
                        // token list labels: 
                        // rule list labels: 
                        // wildcard labels: 
                        retval.tree = root_0;
                        RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval != null ? retval.getTree() : null);

                        root_0 = (GrammarAST)adaptor.nil();
                        // 704:33: -> OPTIONAL[$start, $nongreedy]
                        {
                            adaptor.addChild(root_0, new OptionalBlockAST(OPTIONAL, (retval.start), nongreedy));
                        }


                        retval.tree = root_0;

                    }
                    break;
                case 2:
                    // org\\antlr\\v4\\parse\\ANTLRParser.g:705:6: STAR (nongreedy= QUESTION )?
                    {
                        STAR164 = (Token)Match(input, STAR, FOLLOW_STAR_in_ebnfSuffix3021);
                        stream_STAR.add(STAR164);

                        // org\\antlr\\v4\\parse\\ANTLRParser.g:705:20: (nongreedy= QUESTION )?
                        int alt54 = 2;
                        int LA54_0 = input.LA(1);
                        if ((LA54_0 == QUESTION))
                        {
                            alt54 = 1;
                        }
                        switch (alt54)
                        {
                            case 1:
                                // org\\antlr\\v4\\parse\\ANTLRParser.g:705:20: nongreedy= QUESTION
                                {
                                    nongreedy = (Token)Match(input, QUESTION, FOLLOW_QUESTION_in_ebnfSuffix3025);
                                    stream_QUESTION.add(nongreedy);

                                }
                                break;

                        }


                        // AST REWRITE
                        // elements: 
                        // token labels: 
                        // rule labels: retval
                        // token list labels: 
                        // rule list labels: 
                        // wildcard labels: 
                        retval.tree = root_0;
                        RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval != null ? retval.getTree() : null);

                        root_0 = (GrammarAST)adaptor.nil();
                        // 705:32: -> CLOSURE[$start, $nongreedy]
                        {
                            adaptor.addChild(root_0, new StarBlockAST(CLOSURE, (retval.start), nongreedy));
                        }


                        retval.tree = root_0;

                    }
                    break;
                case 3:
                    // org\\antlr\\v4\\parse\\ANTLRParser.g:706:7: PLUS (nongreedy= QUESTION )?
                    {
                        PLUS165 = (Token)Match(input, PLUS, FOLLOW_PLUS_in_ebnfSuffix3043);
                        stream_PLUS.add(PLUS165);

                        // org\\antlr\\v4\\parse\\ANTLRParser.g:706:21: (nongreedy= QUESTION )?
                        int alt55 = 2;
                        int LA55_0 = input.LA(1);
                        if ((LA55_0 == QUESTION))
                        {
                            alt55 = 1;
                        }
                        switch (alt55)
                        {
                            case 1:
                                // org\\antlr\\v4\\parse\\ANTLRParser.g:706:21: nongreedy= QUESTION
                                {
                                    nongreedy = (Token)Match(input, QUESTION, FOLLOW_QUESTION_in_ebnfSuffix3047);
                                    stream_QUESTION.add(nongreedy);

                                }
                                break;

                        }


                        // AST REWRITE
                        // elements: 
                        // token labels: 
                        // rule labels: retval
                        // token list labels: 
                        // rule list labels: 
                        // wildcard labels: 
                        retval.tree = root_0;
                        RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval != null ? retval.getTree() : null);

                        root_0 = (GrammarAST)adaptor.nil();
                        // 706:33: -> POSITIVE_CLOSURE[$start, $nongreedy]
                        {
                            adaptor.addChild(root_0, new PlusBlockAST(POSITIVE_CLOSURE, (retval.start), nongreedy));
                        }


                        retval.tree = root_0;

                    }
                    break;

            }
            retval.stop = input.LT(-1);

            retval.tree = (GrammarAST)adaptor.rulePostProcessing(root_0);
            adaptor.setTokenBoundaries(retval.tree, retval.start, retval.stop);

        }
        catch (RecognitionException re)
        {
            reportError(re);
            recover(input, re);
            retval.tree = (GrammarAST)adaptor.errorNode(input, retval.start, input.LT(-1), re);
        }
        finally
        {
            // do for sure before leaving
        }
        return retval;
    }
    // $ANTLR end "ebnfSuffix"


    public class lexerAtom_return : ParserRuleReturnScope
    {
        public GrammarAST tree;
        //@Override

        public GrammarAST getTree() { return tree; }
    };


    // $ANTLR start "lexerAtom"
    // org\\antlr\\v4\\parse\\ANTLRParser.g:709:1: lexerAtom : ( range | terminal | RULE_REF | notSet | wildcard | LEXER_CHAR_SET );
    public ANTLRParser.lexerAtom_return lexerAtom()  {
        ANTLRParser.lexerAtom_return retval = new ANTLRParser.lexerAtom_return();
        retval.start = input.LT(1);

        GrammarAST root_0 = null;

        Token RULE_REF168 = null;
        Token LEXER_CHAR_SET171 = null;
        ParserRuleReturnScope range166 = null;
        ParserRuleReturnScope terminal167 = null;
        ParserRuleReturnScope notSet169 = null;
        ParserRuleReturnScope wildcard170 = null;

        GrammarAST RULE_REF168_tree = null;
        GrammarAST LEXER_CHAR_SET171_tree = null;

        try
        {
            // org\\antlr\\v4\\parse\\ANTLRParser.g:710:2: ( range | terminal | RULE_REF | notSet | wildcard | LEXER_CHAR_SET )
            int alt57 = 6;
            switch (input.LA(1))
            {
                case STRING_LITERAL:
                    {
                        int LA57_1 = input.LA(2);
                        if ((LA57_1 == RANGE))
                        {
                            alt57 = 1;
                        }
                        else if ((LA57_1 == ACTION || LA57_1 == DOT || LA57_1 == LEXER_CHAR_SET || (LA57_1 >= LPAREN && LA57_1 <= LT) || LA57_1 == NOT || LA57_1 == OR || LA57_1 == PLUS || LA57_1 == QUESTION || LA57_1 == RARROW || (LA57_1 >= RPAREN && LA57_1 <= SEMPRED) || (LA57_1 >= STAR && LA57_1 <= STRING_LITERAL) || LA57_1 == TOKEN_REF))
                        {
                            alt57 = 2;
                        }

                        else
                        {
                            int nvaeMark = input.mark();
                            try
                            {
                                input.consume();
                                NoViableAltException nvae3 =
                                    new NoViableAltException("", 57, 1, input);
                                throw nvae3;
                            }
                            finally
                            {
                                input.rewind(nvaeMark);
                            }
                        }

                    }
                    break;
                case TOKEN_REF:
                    {
                        alt57 = 2;
                    }
                    break;
                case RULE_REF:
                    {
                        alt57 = 3;
                    }
                    break;
                case NOT:
                    {
                        alt57 = 4;
                    }
                    break;
                case DOT:
                    {
                        alt57 = 5;
                    }
                    break;
                case LEXER_CHAR_SET:
                    {
                        alt57 = 6;
                    }
                    break;
                default:
                    NoViableAltException nvae =
                        new NoViableAltException("", 57, 0, input);
                    throw nvae;
            }
            switch (alt57)
            {
                case 1:
                    // org\\antlr\\v4\\parse\\ANTLRParser.g:710:4: range
                    {
                        root_0 = (GrammarAST)adaptor.nil();


                        pushFollow(FOLLOW_range_in_lexerAtom3068);
                        range166 = range();
                        state._fsp--;

                        adaptor.addChild(root_0, range166.getTree());

                    }
                    break;
                case 2:
                    // org\\antlr\\v4\\parse\\ANTLRParser.g:711:4: terminal
                    {
                        root_0 = (GrammarAST)adaptor.nil();


                        pushFollow(FOLLOW_terminal_in_lexerAtom3073);
                        terminal167 = terminal();
                        state._fsp--;

                        adaptor.addChild(root_0, terminal167.getTree());

                    }
                    break;
                case 3:
                    // org\\antlr\\v4\\parse\\ANTLRParser.g:712:9: RULE_REF
                    {
                        root_0 = (GrammarAST)adaptor.nil();


                        RULE_REF168 = (Token)Match(input, RULE_REF, FOLLOW_RULE_REF_in_lexerAtom3083);
                        RULE_REF168_tree = new RuleRefAST(RULE_REF168);
                        adaptor.addChild(root_0, RULE_REF168_tree);

                    }
                    break;
                case 4:
                    // org\\antlr\\v4\\parse\\ANTLRParser.g:713:7: notSet
                    {
                        root_0 = (GrammarAST)adaptor.nil();


                        pushFollow(FOLLOW_notSet_in_lexerAtom3094);
                        notSet169 = notSet();
                        state._fsp--;

                        adaptor.addChild(root_0, notSet169.getTree());

                    }
                    break;
                case 5:
                    // org\\antlr\\v4\\parse\\ANTLRParser.g:714:7: wildcard
                    {
                        root_0 = (GrammarAST)adaptor.nil();


                        pushFollow(FOLLOW_wildcard_in_lexerAtom3102);
                        wildcard170 = wildcard();
                        state._fsp--;

                        adaptor.addChild(root_0, wildcard170.getTree());

                    }
                    break;
                case 6:
                    // org\\antlr\\v4\\parse\\ANTLRParser.g:715:7: LEXER_CHAR_SET
                    {
                        root_0 = (GrammarAST)adaptor.nil();


                        LEXER_CHAR_SET171 = (Token)Match(input, LEXER_CHAR_SET, FOLLOW_LEXER_CHAR_SET_in_lexerAtom3110);
                        LEXER_CHAR_SET171_tree = (GrammarAST)adaptor.create(LEXER_CHAR_SET171);
                        adaptor.addChild(root_0, LEXER_CHAR_SET171_tree);

                    }
                    break;

            }
            retval.stop = input.LT(-1);

            retval.tree = (GrammarAST)adaptor.rulePostProcessing(root_0);
            adaptor.setTokenBoundaries(retval.tree, retval.start, retval.stop);

        }
        catch (RecognitionException re)
        {
            reportError(re);
            recover(input, re);
            retval.tree = (GrammarAST)adaptor.errorNode(input, retval.start, input.LT(-1), re);
        }
        finally
        {
            // do for sure before leaving
        }
        return retval;
    }
    // $ANTLR end "lexerAtom"


    public class atom_return : ParserRuleReturnScope
    {
        public GrammarAST tree;
        //@Override

        public GrammarAST getTree() { return tree; }
    };


    // $ANTLR start "atom"
    // org\\antlr\\v4\\parse\\ANTLRParser.g:718:1: atom : ( range | terminal | ruleref | notSet | wildcard );
    public ANTLRParser.atom_return atom()  {
        ANTLRParser.atom_return retval = new ANTLRParser.atom_return();
        retval.start = input.LT(1);

        GrammarAST root_0 = null;

        ParserRuleReturnScope range172 = null;
        ParserRuleReturnScope terminal173 = null;
        ParserRuleReturnScope ruleref174 = null;
        ParserRuleReturnScope notSet175 = null;
        ParserRuleReturnScope wildcard176 = null;


        try
        {
            // org\\antlr\\v4\\parse\\ANTLRParser.g:719:2: ( range | terminal | ruleref | notSet | wildcard )
            int alt58 = 5;
            switch (input.LA(1))
            {
                case STRING_LITERAL:
                    {
                        int LA58_1 = input.LA(2);
                        if ((LA58_1 == RANGE))
                        {
                            alt58 = 1;
                        }
                        else if ((LA58_1 == EOF || LA58_1 == ACTION || LA58_1 == DOT || (LA58_1 >= LPAREN && LA58_1 <= LT) || LA58_1 == NOT || LA58_1 == OR || LA58_1 == PLUS || (LA58_1 >= POUND && LA58_1 <= QUESTION) || (LA58_1 >= RPAREN && LA58_1 <= SEMPRED) || (LA58_1 >= STAR && LA58_1 <= STRING_LITERAL) || LA58_1 == TOKEN_REF))
                        {
                            alt58 = 2;
                        }

                        else
                        {
                            int nvaeMark = input.mark();
                            try
                            {
                                input.consume();
                                NoViableAltException nvae2 =
                                    new NoViableAltException("", 58, 1, input);
                                throw nvae2;
                            }
                            finally
                            {
                                input.rewind(nvaeMark);
                            }
                        }

                    }
                    break;
                case TOKEN_REF:
                    {
                        alt58 = 2;
                    }
                    break;
                case RULE_REF:
                    {
                        alt58 = 3;
                    }
                    break;
                case NOT:
                    {
                        alt58 = 4;
                    }
                    break;
                case DOT:
                    {
                        alt58 = 5;
                    }
                    break;
                default:
                    NoViableAltException nvae =
                        new NoViableAltException("", 58, 0, input);
                    throw nvae;
            }
            switch (alt58)
            {
                case 1:
                    // org\\antlr\\v4\\parse\\ANTLRParser.g:733:9: range
                    {
                        root_0 = (GrammarAST)adaptor.nil();


                        pushFollow(FOLLOW_range_in_atom3155);
                        range172 = range();
                        state._fsp--;

                        adaptor.addChild(root_0, range172.getTree());

                    }
                    break;
                case 2:
                    // org\\antlr\\v4\\parse\\ANTLRParser.g:734:4: terminal
                    {
                        root_0 = (GrammarAST)adaptor.nil();


                        pushFollow(FOLLOW_terminal_in_atom3162);
                        terminal173 = terminal();
                        state._fsp--;

                        adaptor.addChild(root_0, terminal173.getTree());

                    }
                    break;
                case 3:
                    // org\\antlr\\v4\\parse\\ANTLRParser.g:735:9: ruleref
                    {
                        root_0 = (GrammarAST)adaptor.nil();


                        pushFollow(FOLLOW_ruleref_in_atom3172);
                        ruleref174 = ruleref();
                        state._fsp--;

                        adaptor.addChild(root_0, ruleref174.getTree());

                    }
                    break;
                case 4:
                    // org\\antlr\\v4\\parse\\ANTLRParser.g:736:7: notSet
                    {
                        root_0 = (GrammarAST)adaptor.nil();


                        pushFollow(FOLLOW_notSet_in_atom3180);
                        notSet175 = notSet();
                        state._fsp--;

                        adaptor.addChild(root_0, notSet175.getTree());

                    }
                    break;
                case 5:
                    // org\\antlr\\v4\\parse\\ANTLRParser.g:737:7: wildcard
                    {
                        root_0 = (GrammarAST)adaptor.nil();


                        pushFollow(FOLLOW_wildcard_in_atom3188);
                        wildcard176 = wildcard();
                        state._fsp--;

                        adaptor.addChild(root_0, wildcard176.getTree());

                    }
                    break;

            }
            retval.stop = input.LT(-1);

            retval.tree = (GrammarAST)adaptor.rulePostProcessing(root_0);
            adaptor.setTokenBoundaries(retval.tree, retval.start, retval.stop);

        }
        catch (RecognitionException re)
        {
            throw re;
        }

        finally
        {
            // do for sure before leaving
        }
        return retval;
    }
    // $ANTLR end "atom"


    public class wildcard_return : ParserRuleReturnScope
    {
        public GrammarAST tree;
        //@Override

        public GrammarAST getTree() { return tree; }
    };


    // $ANTLR start "wildcard"
    // org\\antlr\\v4\\parse\\ANTLRParser.g:741:1: wildcard : DOT ( elementOptions )? -> ^( WILDCARD[$DOT] ( elementOptions )? ) ;
    public ANTLRParser.wildcard_return wildcard()  {
        ANTLRParser.wildcard_return retval = new ANTLRParser.wildcard_return();
        retval.start = input.LT(1);

        GrammarAST root_0 = null;

        Token DOT177 = null;
        ParserRuleReturnScope elementOptions178 = null;

        GrammarAST DOT177_tree = null;
        RewriteRuleTokenStream stream_DOT = new RewriteRuleTokenStream(adaptor, "token DOT");
        RewriteRuleSubtreeStream stream_elementOptions = new RewriteRuleSubtreeStream(adaptor, "rule elementOptions");

        try
        {
            // org\\antlr\\v4\\parse\\ANTLRParser.g:748:2: ( DOT ( elementOptions )? -> ^( WILDCARD[$DOT] ( elementOptions )? ) )
            // org\\antlr\\v4\\parse\\ANTLRParser.g:753:6: DOT ( elementOptions )?
            {
                DOT177 = (Token)Match(input, DOT, FOLLOW_DOT_in_wildcard3236);
                stream_DOT.add(DOT177);

                // org\\antlr\\v4\\parse\\ANTLRParser.g:753:10: ( elementOptions )?
                int alt59 = 2;
                int LA59_0 = input.LA(1);
                if ((LA59_0 == LT))
                {
                    alt59 = 1;
                }
                switch (alt59)
                {
                    case 1:
                        // org\\antlr\\v4\\parse\\ANTLRParser.g:753:10: elementOptions
                        {
                            pushFollow(FOLLOW_elementOptions_in_wildcard3238);
                            elementOptions178 = elementOptions();
                            state._fsp--;

                            stream_elementOptions.add(elementOptions178.getTree());
                        }
                        break;

                }


                // AST REWRITE
                // elements: elementOptions
                // token labels: 
                // rule labels: retval
                // token list labels: 
                // rule list labels: 
                // wildcard labels: 
                retval.tree = root_0;
                RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval != null ? retval.getTree() : null);

                root_0 = (GrammarAST)adaptor.nil();
                // 754:6: -> ^( WILDCARD[$DOT] ( elementOptions )? )
                {
                    // org\\antlr\\v4\\parse\\ANTLRParser.g:754:9: ^( WILDCARD[$DOT] ( elementOptions )? )
                    {
                        GrammarAST root_1 = (GrammarAST)adaptor.nil();
                        root_1 = (GrammarAST)adaptor.becomeRoot(new TerminalAST(WILDCARD, DOT177), root_1);
                        // org\\antlr\\v4\\parse\\ANTLRParser.g:754:39: ( elementOptions )?
                        if (stream_elementOptions.hasNext())
                        {
                            adaptor.addChild(root_1, stream_elementOptions.nextTree());
                        }
                        stream_elementOptions.reset();

                        adaptor.addChild(root_0, root_1);
                    }

                }


                retval.tree = root_0;

            }

            retval.stop = input.LT(-1);

            retval.tree = (GrammarAST)adaptor.rulePostProcessing(root_0);
            adaptor.setTokenBoundaries(retval.tree, retval.start, retval.stop);


            GrammarAST options = (GrammarAST)retval.tree.getFirstChildWithType(ANTLRParser.ELEMENT_OPTIONS);
            if (options != null)
            {
                Grammar.setNodeOptions(retval.tree, options);
            }

        }
        catch (RecognitionException re)
        {
            reportError(re);
            recover(input, re);
            retval.tree = (GrammarAST)adaptor.errorNode(input, retval.start, input.LT(-1), re);
        }
        finally
        {
            // do for sure before leaving
        }
        return retval;
    }
    // $ANTLR end "wildcard"


    public class notSet_return : ParserRuleReturnScope
    {
        public GrammarAST tree;
        //@Override

        public GrammarAST getTree() { return tree; }
    };


    // $ANTLR start "notSet"
    // org\\antlr\\v4\\parse\\ANTLRParser.g:762:1: notSet : ( NOT setElement -> ^( NOT[$NOT] ^( SET[$setElement.start,\"SET\"] setElement ) ) | NOT blockSet -> ^( NOT[$NOT] blockSet ) );
    public ANTLRParser.notSet_return notSet()  {
        ANTLRParser.notSet_return retval = new ANTLRParser.notSet_return();
        retval.start = input.LT(1);

        GrammarAST root_0 = null;

        Token NOT179 = null;
        Token NOT181 = null;
        ParserRuleReturnScope setElement180 = null;
        ParserRuleReturnScope blockSet182 = null;

        GrammarAST NOT179_tree = null;
        GrammarAST NOT181_tree = null;
        RewriteRuleTokenStream stream_NOT = new RewriteRuleTokenStream(adaptor, "token NOT");
        RewriteRuleSubtreeStream stream_blockSet = new RewriteRuleSubtreeStream(adaptor, "rule blockSet");
        RewriteRuleSubtreeStream stream_setElement = new RewriteRuleSubtreeStream(adaptor, "rule setElement");

        try
        {
            // org\\antlr\\v4\\parse\\ANTLRParser.g:763:5: ( NOT setElement -> ^( NOT[$NOT] ^( SET[$setElement.start,\"SET\"] setElement ) ) | NOT blockSet -> ^( NOT[$NOT] blockSet ) )
            int alt60 = 2;
            int LA60_0 = input.LA(1);
            if ((LA60_0 == NOT))
            {
                int LA60_1 = input.LA(2);
                if ((LA60_1 == LEXER_CHAR_SET || LA60_1 == STRING_LITERAL || LA60_1 == TOKEN_REF))
                {
                    alt60 = 1;
                }
                else if ((LA60_1 == LPAREN))
                {
                    alt60 = 2;
                }

                else
                {
                    int nvaeMark = input.mark();
                    try
                    {
                        input.consume();
                        NoViableAltException nvae =
                            new NoViableAltException("", 60, 1, input);
                        throw nvae;
                    }
                    finally
                    {
                        input.rewind(nvaeMark);
                    }
                }

            }

            else
            {
                NoViableAltException nvae =
                    new NoViableAltException("", 60, 0, input);
                throw nvae;
            }

            switch (alt60)
            {
                case 1:
                    // org\\antlr\\v4\\parse\\ANTLRParser.g:763:7: NOT setElement
                    {
                        NOT179 = (Token)Match(input, NOT, FOLLOW_NOT_in_notSet3276);
                        stream_NOT.add(NOT179);

                        pushFollow(FOLLOW_setElement_in_notSet3278);
                        setElement180 = setElement();
                        state._fsp--;

                        stream_setElement.add(setElement180.getTree());

                        // AST REWRITE
                        // elements: setElement, NOT
                        // token labels: 
                        // rule labels: retval
                        // token list labels: 
                        // rule list labels: 
                        // wildcard labels: 
                        retval.tree = root_0;
                        RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval != null ? retval.getTree() : null);

                        root_0 = (GrammarAST)adaptor.nil();
                        // 763:22: -> ^( NOT[$NOT] ^( SET[$setElement.start,\"SET\"] setElement ) )
                        {
                            // org\\antlr\\v4\\parse\\ANTLRParser.g:763:25: ^( NOT[$NOT] ^( SET[$setElement.start,\"SET\"] setElement ) )
                            {
                                GrammarAST root_1 = (GrammarAST)adaptor.nil();
                                root_1 = (GrammarAST)adaptor.becomeRoot(new NotAST(NOT, NOT179), root_1);
                                // org\\antlr\\v4\\parse\\ANTLRParser.g:763:45: ^( SET[$setElement.start,\"SET\"] setElement )
                                {
                                    GrammarAST root_2 = (GrammarAST)adaptor.nil();
                                    root_2 = (GrammarAST)adaptor.becomeRoot(new SetAST(SET, (setElement180 != null ? (setElement180.start) : null), "SET"), root_2);
                                    adaptor.addChild(root_2, stream_setElement.nextTree());
                                    adaptor.addChild(root_1, root_2);
                                }

                                adaptor.addChild(root_0, root_1);
                            }

                        }


                        retval.tree = root_0;

                    }
                    break;
                case 2:
                    // org\\antlr\\v4\\parse\\ANTLRParser.g:764:7: NOT blockSet
                    {
                        NOT181 = (Token)Match(input, NOT, FOLLOW_NOT_in_notSet3306);
                        stream_NOT.add(NOT181);

                        pushFollow(FOLLOW_blockSet_in_notSet3308);
                        blockSet182 = blockSet();
                        state._fsp--;

                        stream_blockSet.add(blockSet182.getTree());

                        // AST REWRITE
                        // elements: blockSet, NOT
                        // token labels: 
                        // rule labels: retval
                        // token list labels: 
                        // rule list labels: 
                        // wildcard labels: 
                        retval.tree = root_0;
                        RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval != null ? retval.getTree() : null);

                        root_0 = (GrammarAST)adaptor.nil();
                        // 764:21: -> ^( NOT[$NOT] blockSet )
                        {
                            // org\\antlr\\v4\\parse\\ANTLRParser.g:764:24: ^( NOT[$NOT] blockSet )
                            {
                                GrammarAST root_1 = (GrammarAST)adaptor.nil();
                                root_1 = (GrammarAST)adaptor.becomeRoot(new NotAST(NOT, NOT181), root_1);
                                adaptor.addChild(root_1, stream_blockSet.nextTree());
                                adaptor.addChild(root_0, root_1);
                            }

                        }


                        retval.tree = root_0;

                    }
                    break;

            }
            retval.stop = input.LT(-1);

            retval.tree = (GrammarAST)adaptor.rulePostProcessing(root_0);
            adaptor.setTokenBoundaries(retval.tree, retval.start, retval.stop);

        }
        catch (RecognitionException re)
        {
            reportError(re);
            recover(input, re);
            retval.tree = (GrammarAST)adaptor.errorNode(input, retval.start, input.LT(-1), re);
        }
        finally
        {
            // do for sure before leaving
        }
        return retval;
    }
    // $ANTLR end "notSet"


    public class blockSet_return : ParserRuleReturnScope
    {
        public GrammarAST tree;
        //@Override

        public GrammarAST getTree() { return tree; }
    };


    // $ANTLR start "blockSet"
    // org\\antlr\\v4\\parse\\ANTLRParser.g:767:1: blockSet : LPAREN setElement ( OR setElement )* RPAREN -> ^( SET[$LPAREN,\"SET\"] ( setElement )+ ) ;
    public ANTLRParser.blockSet_return blockSet()  {
        ANTLRParser.blockSet_return retval = new ANTLRParser.blockSet_return();
        retval.start = input.LT(1);

        GrammarAST root_0 = null;

        Token LPAREN183 = null;
        Token OR185 = null;
        Token RPAREN187 = null;
        ParserRuleReturnScope setElement184 = null;
        ParserRuleReturnScope setElement186 = null;

        GrammarAST LPAREN183_tree = null;
        GrammarAST OR185_tree = null;
        GrammarAST RPAREN187_tree = null;
        RewriteRuleTokenStream stream_OR = new RewriteRuleTokenStream(adaptor, "token OR");
        RewriteRuleTokenStream stream_LPAREN = new RewriteRuleTokenStream(adaptor, "token LPAREN");
        RewriteRuleTokenStream stream_RPAREN = new RewriteRuleTokenStream(adaptor, "token RPAREN");
        RewriteRuleSubtreeStream stream_setElement = new RewriteRuleSubtreeStream(adaptor, "rule setElement");


        Token t;
        bool ebnf = false;

        try
        {
            // org\\antlr\\v4\\parse\\ANTLRParser.g:772:5: ( LPAREN setElement ( OR setElement )* RPAREN -> ^( SET[$LPAREN,\"SET\"] ( setElement )+ ) )
            // org\\antlr\\v4\\parse\\ANTLRParser.g:772:7: LPAREN setElement ( OR setElement )* RPAREN
            {
                LPAREN183 = (Token)Match(input, LPAREN, FOLLOW_LPAREN_in_blockSet3343);
                stream_LPAREN.add(LPAREN183);

                pushFollow(FOLLOW_setElement_in_blockSet3345);
                setElement184 = setElement();
                state._fsp--;

                stream_setElement.add(setElement184.getTree());
            // org\\antlr\\v4\\parse\\ANTLRParser.g:772:25: ( OR setElement )*
            loop61:
                while (true)
                {
                    int alt61 = 2;
                    int LA61_0 = input.LA(1);
                    if ((LA61_0 == OR))
                    {
                        alt61 = 1;
                    }

                    switch (alt61)
                    {
                        case 1:
                            // org\\antlr\\v4\\parse\\ANTLRParser.g:772:26: OR setElement
                            {
                                OR185 = (Token)Match(input, OR, FOLLOW_OR_in_blockSet3348);
                                stream_OR.add(OR185);

                                pushFollow(FOLLOW_setElement_in_blockSet3350);
                                setElement186 = setElement();
                                state._fsp--;

                                stream_setElement.add(setElement186.getTree());
                            }
                            break;

                        default:
                            //break loop61;
                            goto exit61;
                    }
                }
            exit61:
                RPAREN187 = (Token)Match(input, RPAREN, FOLLOW_RPAREN_in_blockSet3354);
                stream_RPAREN.add(RPAREN187);


                // AST REWRITE
                // elements: setElement
                // token labels: 
                // rule labels: retval
                // token list labels: 
                // rule list labels: 
                // wildcard labels: 
                retval.tree = root_0;
                RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval != null ? retval.getTree() : null);

                root_0 = (GrammarAST)adaptor.nil();
                // 773:3: -> ^( SET[$LPAREN,\"SET\"] ( setElement )+ )
                {
                    // org\\antlr\\v4\\parse\\ANTLRParser.g:773:6: ^( SET[$LPAREN,\"SET\"] ( setElement )+ )
                    {
                        GrammarAST root_1 = (GrammarAST)adaptor.nil();
                        root_1 = (GrammarAST)adaptor.becomeRoot(new SetAST(SET, LPAREN183, "SET"), root_1);
                        if (!(stream_setElement.hasNext()))
                        {
                            throw new RewriteEarlyExitException();
                        }
                        while (stream_setElement.hasNext())
                        {
                            adaptor.addChild(root_1, stream_setElement.nextTree());
                        }
                        stream_setElement.reset();

                        adaptor.addChild(root_0, root_1);
                    }

                }


                retval.tree = root_0;

            }

            retval.stop = input.LT(-1);

            retval.tree = (GrammarAST)adaptor.rulePostProcessing(root_0);
            adaptor.setTokenBoundaries(retval.tree, retval.start, retval.stop);

        }
        catch (RecognitionException re)
        {
            reportError(re);
            recover(input, re);
            retval.tree = (GrammarAST)adaptor.errorNode(input, retval.start, input.LT(-1), re);
        }
        finally
        {
            // do for sure before leaving
        }
        return retval;
    }
    // $ANTLR end "blockSet"


    public class setElement_return : ParserRuleReturnScope
    {
        public GrammarAST tree;
        //@Override

        public GrammarAST getTree() { return tree; }
    };


    // $ANTLR start "setElement"
    // org\\antlr\\v4\\parse\\ANTLRParser.g:776:1: setElement : ( TOKEN_REF ^ ( elementOptions )? | STRING_LITERAL ^ ( elementOptions )? | range | LEXER_CHAR_SET );
    public ANTLRParser.setElement_return setElement()  {
        ANTLRParser.setElement_return retval = new ANTLRParser.setElement_return();
        retval.start = input.LT(1);

        GrammarAST root_0 = null;

        Token TOKEN_REF188 = null;
        Token STRING_LITERAL190 = null;
        Token LEXER_CHAR_SET193 = null;
        ParserRuleReturnScope elementOptions189 = null;
        ParserRuleReturnScope elementOptions191 = null;
        ParserRuleReturnScope range192 = null;

        GrammarAST TOKEN_REF188_tree = null;
        GrammarAST STRING_LITERAL190_tree = null;
        GrammarAST LEXER_CHAR_SET193_tree = null;

        try
        {
            // org\\antlr\\v4\\parse\\ANTLRParser.g:777:2: ( TOKEN_REF ^ ( elementOptions )? | STRING_LITERAL ^ ( elementOptions )? | range | LEXER_CHAR_SET )
            int alt64 = 4;
            switch (input.LA(1))
            {
                case TOKEN_REF:
                    {
                        alt64 = 1;
                    }
                    break;
                case STRING_LITERAL:
                    {
                        int LA64_2 = input.LA(2);
                        if ((LA64_2 == RANGE))
                        {
                            alt64 = 3;
                        }
                        else if ((LA64_2 == EOF || LA64_2 == ACTION || LA64_2 == DOT || LA64_2 == LEXER_CHAR_SET || (LA64_2 >= LPAREN && LA64_2 <= LT) || LA64_2 == NOT || LA64_2 == OR || LA64_2 == PLUS || (LA64_2 >= POUND && LA64_2 <= QUESTION) || LA64_2 == RARROW || (LA64_2 >= RPAREN && LA64_2 <= SEMPRED) || (LA64_2 >= STAR && LA64_2 <= STRING_LITERAL) || LA64_2 == TOKEN_REF))
                        {
                            alt64 = 2;
                        }

                        else
                        {
                            int nvaeMark = input.mark();
                            try
                            {
                                input.consume();
                                NoViableAltException nvae6 =
                                    new NoViableAltException("", 64, 2, input);
                                throw nvae6;
                            }
                            finally
                            {
                                input.rewind(nvaeMark);
                            }
                        }

                    }
                    break;
                case LEXER_CHAR_SET:
                    {
                        alt64 = 4;
                    }
                    break;
                default:
                    NoViableAltException nvae =
                        new NoViableAltException("", 64, 0, input);
                    throw nvae;
            }
            switch (alt64)
            {
                case 1:
                    // org\\antlr\\v4\\parse\\ANTLRParser.g:777:4: TOKEN_REF ^ ( elementOptions )?
                    {
                        root_0 = (GrammarAST)adaptor.nil();


                        TOKEN_REF188 = (Token)Match(input, TOKEN_REF, FOLLOW_TOKEN_REF_in_setElement3384);
                        TOKEN_REF188_tree = new TerminalAST(TOKEN_REF188);
                        root_0 = (GrammarAST)adaptor.becomeRoot(TOKEN_REF188_tree, root_0);

                        // org\\antlr\\v4\\parse\\ANTLRParser.g:777:28: ( elementOptions )?
                        int alt62 = 2;
                        int LA62_0 = input.LA(1);
                        if ((LA62_0 == LT))
                        {
                            alt62 = 1;
                        }
                        switch (alt62)
                        {
                            case 1:
                                // org\\antlr\\v4\\parse\\ANTLRParser.g:777:28: elementOptions
                                {
                                    pushFollow(FOLLOW_elementOptions_in_setElement3390);
                                    elementOptions189 = elementOptions();
                                    state._fsp--;

                                    adaptor.addChild(root_0, elementOptions189.getTree());

                                }
                                break;

                        }

                    }
                    break;
                case 2:
                    // org\\antlr\\v4\\parse\\ANTLRParser.g:778:4: STRING_LITERAL ^ ( elementOptions )?
                    {
                        root_0 = (GrammarAST)adaptor.nil();


                        STRING_LITERAL190 = (Token)Match(input, STRING_LITERAL, FOLLOW_STRING_LITERAL_in_setElement3396);
                        STRING_LITERAL190_tree = new TerminalAST(STRING_LITERAL190);
                        root_0 = (GrammarAST)adaptor.becomeRoot(STRING_LITERAL190_tree, root_0);

                        // org\\antlr\\v4\\parse\\ANTLRParser.g:778:33: ( elementOptions )?
                        int alt63 = 2;
                        int LA63_0 = input.LA(1);
                        if ((LA63_0 == LT))
                        {
                            alt63 = 1;
                        }
                        switch (alt63)
                        {
                            case 1:
                                // org\\antlr\\v4\\parse\\ANTLRParser.g:778:33: elementOptions
                                {
                                    pushFollow(FOLLOW_elementOptions_in_setElement3402);
                                    elementOptions191 = elementOptions();
                                    state._fsp--;

                                    adaptor.addChild(root_0, elementOptions191.getTree());

                                }
                                break;

                        }

                    }
                    break;
                case 3:
                    // org\\antlr\\v4\\parse\\ANTLRParser.g:779:4: range
                    {
                        root_0 = (GrammarAST)adaptor.nil();


                        pushFollow(FOLLOW_range_in_setElement3408);
                        range192 = range();
                        state._fsp--;

                        adaptor.addChild(root_0, range192.getTree());

                    }
                    break;
                case 4:
                    // org\\antlr\\v4\\parse\\ANTLRParser.g:780:9: LEXER_CHAR_SET
                    {
                        root_0 = (GrammarAST)adaptor.nil();


                        LEXER_CHAR_SET193 = (Token)Match(input, LEXER_CHAR_SET, FOLLOW_LEXER_CHAR_SET_in_setElement3418);
                        LEXER_CHAR_SET193_tree = (GrammarAST)adaptor.create(LEXER_CHAR_SET193);
                        adaptor.addChild(root_0, LEXER_CHAR_SET193_tree);

                    }
                    break;

            }
            retval.stop = input.LT(-1);

            retval.tree = (GrammarAST)adaptor.rulePostProcessing(root_0);
            adaptor.setTokenBoundaries(retval.tree, retval.start, retval.stop);

        }
        catch (RecognitionException re)
        {
            reportError(re);
            recover(input, re);
            retval.tree = (GrammarAST)adaptor.errorNode(input, retval.start, input.LT(-1), re);
        }
        finally
        {
            // do for sure before leaving
        }
        return retval;
    }
    // $ANTLR end "setElement"


    public class block_return : ParserRuleReturnScope
    {
        public GrammarAST tree;
        //@Override

        public GrammarAST getTree() { return tree; }
    };


    // $ANTLR start "block"
    // org\\antlr\\v4\\parse\\ANTLRParser.g:790:1: block : LPAREN ( ( optionsSpec )? (ra+= ruleAction )* COLON )? altList RPAREN -> ^( BLOCK[$LPAREN,\"BLOCK\"] ( optionsSpec )? ( $ra)* altList ) ;
    public ANTLRParser.block_return block()  {
        ANTLRParser.block_return retval = new ANTLRParser.block_return();
        retval.start = input.LT(1);

        GrammarAST root_0 = null;

        Token LPAREN194 = null;
        Token COLON196 = null;
        Token RPAREN198 = null;
        List<Object> list_ra = null;
        ParserRuleReturnScope optionsSpec195 = null;
        ParserRuleReturnScope altList197 = null;
        RuleReturnScope ra = null;
        GrammarAST LPAREN194_tree = null;
        GrammarAST COLON196_tree = null;
        GrammarAST RPAREN198_tree = null;
        RewriteRuleTokenStream stream_LPAREN = new RewriteRuleTokenStream(adaptor, "token LPAREN");
        RewriteRuleTokenStream stream_COLON = new RewriteRuleTokenStream(adaptor, "token COLON");
        RewriteRuleTokenStream stream_RPAREN = new RewriteRuleTokenStream(adaptor, "token RPAREN");
        RewriteRuleSubtreeStream stream_ruleAction = new RewriteRuleSubtreeStream(adaptor, "rule ruleAction");
        RewriteRuleSubtreeStream stream_optionsSpec = new RewriteRuleSubtreeStream(adaptor, "rule optionsSpec");
        RewriteRuleSubtreeStream stream_altList = new RewriteRuleSubtreeStream(adaptor, "rule altList");

        try
        {
            // org\\antlr\\v4\\parse\\ANTLRParser.g:797:3: ( LPAREN ( ( optionsSpec )? (ra+= ruleAction )* COLON )? altList RPAREN -> ^( BLOCK[$LPAREN,\"BLOCK\"] ( optionsSpec )? ( $ra)* altList ) )
            // org\\antlr\\v4\\parse\\ANTLRParser.g:797:5: LPAREN ( ( optionsSpec )? (ra+= ruleAction )* COLON )? altList RPAREN
            {
                LPAREN194 = (Token)Match(input, LPAREN, FOLLOW_LPAREN_in_block3442);
                stream_LPAREN.add(LPAREN194);

                // org\\antlr\\v4\\parse\\ANTLRParser.g:798:9: ( ( optionsSpec )? (ra+= ruleAction )* COLON )?
                int alt67 = 2;
                int LA67_0 = input.LA(1);
                if ((LA67_0 == AT || LA67_0 == COLON || LA67_0 == OPTIONS))
                {
                    alt67 = 1;
                }
                switch (alt67)
                {
                    case 1:
                        // org\\antlr\\v4\\parse\\ANTLRParser.g:798:11: ( optionsSpec )? (ra+= ruleAction )* COLON
                        {
                            // org\\antlr\\v4\\parse\\ANTLRParser.g:798:11: ( optionsSpec )?
                            int alt65 = 2;
                            int LA65_0 = input.LA(1);
                            if ((LA65_0 == OPTIONS))
                            {
                                alt65 = 1;
                            }
                            switch (alt65)
                            {
                                case 1:
                                    // org\\antlr\\v4\\parse\\ANTLRParser.g:798:11: optionsSpec
                                    {
                                        pushFollow(FOLLOW_optionsSpec_in_block3454);
                                        optionsSpec195 = optionsSpec();
                                        state._fsp--;

                                        stream_optionsSpec.add(optionsSpec195.getTree());
                                    }
                                    break;

                            }

                        // org\\antlr\\v4\\parse\\ANTLRParser.g:798:26: (ra+= ruleAction )*
                        loop66:
                            while (true)
                            {
                                int alt66 = 2;
                                int LA66_0 = input.LA(1);
                                if ((LA66_0 == AT))
                                {
                                    alt66 = 1;
                                }

                                switch (alt66)
                                {
                                    case 1:
                                        // org\\antlr\\v4\\parse\\ANTLRParser.g:798:26: ra+= ruleAction
                                        {
                                            pushFollow(FOLLOW_ruleAction_in_block3459);
                                            ra = ruleAction();
                                            state._fsp--;

                                            stream_ruleAction.add(ra.getTree());
                                            if (list_ra == null) list_ra = new ();
                                            list_ra.Add(ra.getTree());
                                        }
                                        break;

                                    default:
                                        goto exit66;
                                        //break loop66;
                                }
                            }
                        exit66:
                            COLON196 = (Token)Match(input, COLON, FOLLOW_COLON_in_block3462);
                            stream_COLON.add(COLON196);

                        }
                        break;

                }

                pushFollow(FOLLOW_altList_in_block3475);
                altList197 = altList();
                state._fsp--;

                stream_altList.add(altList197.getTree());
                RPAREN198 = (Token)Match(input, RPAREN, FOLLOW_RPAREN_in_block3479);
                stream_RPAREN.add(RPAREN198);


                // AST REWRITE
                // elements: altList, optionsSpec, ra
                // token labels: 
                // rule labels: retval
                // token list labels: 
                // rule list labels: ra
                // wildcard labels: 
                retval.tree = root_0;
                RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval != null ? retval.getTree() : null);
                RewriteRuleSubtreeStream stream_ra = new RewriteRuleSubtreeStream(adaptor, "token ra", list_ra);
                root_0 = (GrammarAST)adaptor.nil();
                // 801:7: -> ^( BLOCK[$LPAREN,\"BLOCK\"] ( optionsSpec )? ( $ra)* altList )
                {
                    // org\\antlr\\v4\\parse\\ANTLRParser.g:801:10: ^( BLOCK[$LPAREN,\"BLOCK\"] ( optionsSpec )? ( $ra)* altList )
                    {
                        GrammarAST root_1 = (GrammarAST)adaptor.nil();
                        root_1 = (GrammarAST)adaptor.becomeRoot(new BlockAST(BLOCK, LPAREN194, "BLOCK"), root_1);
                        // org\\antlr\\v4\\parse\\ANTLRParser.g:801:45: ( optionsSpec )?
                        if (stream_optionsSpec.hasNext())
                        {
                            adaptor.addChild(root_1, stream_optionsSpec.nextTree());
                        }
                        stream_optionsSpec.reset();

                        // org\\antlr\\v4\\parse\\ANTLRParser.g:801:59: ( $ra)*
                        while (stream_ra.hasNext())
                        {
                            adaptor.addChild(root_1, stream_ra.nextTree());
                        }
                        stream_ra.reset();

                        adaptor.addChild(root_1, stream_altList.nextTree());
                        adaptor.addChild(root_0, root_1);
                    }

                }


                retval.tree = root_0;

            }

            retval.stop = input.LT(-1);

            retval.tree = (GrammarAST)adaptor.rulePostProcessing(root_0);
            adaptor.setTokenBoundaries(retval.tree, retval.start, retval.stop);


            GrammarAST options = (GrammarAST)retval.tree.getFirstChildWithType(ANTLRParser.OPTIONS);
            if (options != null)
            {
                Grammar.setNodeOptions(retval.tree, options);
            }

        }
        catch (RecognitionException re)
        {
            reportError(re);
            recover(input, re);
            retval.tree = (GrammarAST)adaptor.errorNode(input, retval.start, input.LT(-1), re);
        }
        finally
        {
            // do for sure before leaving
        }
        return retval;
    }
    // $ANTLR end "block"


    public class ruleref_return : ParserRuleReturnScope
    {
        public GrammarAST tree;
        //@Override

        public GrammarAST getTree() { return tree; }
    };


    // $ANTLR start "ruleref"
    // org\\antlr\\v4\\parse\\ANTLRParser.g:810:1: ruleref : RULE_REF ( ARG_ACTION )? ( elementOptions )? -> ^( RULE_REF ( ARG_ACTION )? ( elementOptions )? ) ;
    public ANTLRParser.ruleref_return ruleref()  {
        ANTLRParser.ruleref_return retval = new ANTLRParser.ruleref_return();
        retval.start = input.LT(1);

        GrammarAST root_0 = null;

        Token RULE_REF199 = null;
        Token ARG_ACTION200 = null;
        ParserRuleReturnScope elementOptions201 = null;

        GrammarAST RULE_REF199_tree = null;
        GrammarAST ARG_ACTION200_tree = null;
        RewriteRuleTokenStream stream_ARG_ACTION = new RewriteRuleTokenStream(adaptor, "token ARG_ACTION");
        RewriteRuleTokenStream stream_RULE_REF = new RewriteRuleTokenStream(adaptor, "token RULE_REF");
        RewriteRuleSubtreeStream stream_elementOptions = new RewriteRuleSubtreeStream(adaptor, "rule elementOptions");

        try
        {
            // org\\antlr\\v4\\parse\\ANTLRParser.g:817:5: ( RULE_REF ( ARG_ACTION )? ( elementOptions )? -> ^( RULE_REF ( ARG_ACTION )? ( elementOptions )? ) )
            // org\\antlr\\v4\\parse\\ANTLRParser.g:817:7: RULE_REF ( ARG_ACTION )? ( elementOptions )?
            {
                RULE_REF199 = (Token)Match(input, RULE_REF, FOLLOW_RULE_REF_in_ruleref3533);
                stream_RULE_REF.add(RULE_REF199);

                // org\\antlr\\v4\\parse\\ANTLRParser.g:817:16: ( ARG_ACTION )?
                int alt68 = 2;
                int LA68_0 = input.LA(1);
                if ((LA68_0 == ARG_ACTION))
                {
                    alt68 = 1;
                }
                switch (alt68)
                {
                    case 1:
                        // org\\antlr\\v4\\parse\\ANTLRParser.g:817:16: ARG_ACTION
                        {
                            ARG_ACTION200 = (Token)Match(input, ARG_ACTION, FOLLOW_ARG_ACTION_in_ruleref3535);
                            stream_ARG_ACTION.add(ARG_ACTION200);

                        }
                        break;

                }

                // org\\antlr\\v4\\parse\\ANTLRParser.g:817:28: ( elementOptions )?
                int alt69 = 2;
                int LA69_0 = input.LA(1);
                if ((LA69_0 == LT))
                {
                    alt69 = 1;
                }
                switch (alt69)
                {
                    case 1:
                        // org\\antlr\\v4\\parse\\ANTLRParser.g:817:28: elementOptions
                        {
                            pushFollow(FOLLOW_elementOptions_in_ruleref3538);
                            elementOptions201 = elementOptions();
                            state._fsp--;

                            stream_elementOptions.add(elementOptions201.getTree());
                        }
                        break;

                }


                // AST REWRITE
                // elements: RULE_REF, ARG_ACTION, elementOptions
                // token labels: 
                // rule labels: retval
                // token list labels: 
                // rule list labels: 
                // wildcard labels: 
                retval.tree = root_0;
                RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval != null ? retval.getTree() : null);

                root_0 = (GrammarAST)adaptor.nil();
                // 817:44: -> ^( RULE_REF ( ARG_ACTION )? ( elementOptions )? )
                {
                    // org\\antlr\\v4\\parse\\ANTLRParser.g:817:47: ^( RULE_REF ( ARG_ACTION )? ( elementOptions )? )
                    {
                        GrammarAST root_1 = (GrammarAST)adaptor.nil();
                        root_1 = (GrammarAST)adaptor.becomeRoot(new RuleRefAST(stream_RULE_REF.nextToken()), root_1);
                        // org\\antlr\\v4\\parse\\ANTLRParser.g:817:70: ( ARG_ACTION )?
                        if (stream_ARG_ACTION.hasNext())
                        {
                            adaptor.addChild(root_1, new ActionAST(stream_ARG_ACTION.nextToken()));
                        }
                        stream_ARG_ACTION.reset();

                        // org\\antlr\\v4\\parse\\ANTLRParser.g:817:93: ( elementOptions )?
                        if (stream_elementOptions.hasNext())
                        {
                            adaptor.addChild(root_1, stream_elementOptions.nextTree());
                        }
                        stream_elementOptions.reset();

                        adaptor.addChild(root_0, root_1);
                    }

                }


                retval.tree = root_0;

            }

            retval.stop = input.LT(-1);

            retval.tree = (GrammarAST)adaptor.rulePostProcessing(root_0);
            adaptor.setTokenBoundaries(retval.tree, retval.start, retval.stop);


            GrammarAST options = (GrammarAST)retval.tree.getFirstChildWithType(ANTLRParser.ELEMENT_OPTIONS);
            if (options != null)
            {
                Grammar.setNodeOptions(retval.tree, options);
            }

        }
        catch (RecognitionException re)
        {
            throw re;
        }

        finally
        {
            // do for sure before leaving
        }
        return retval;
    }
    // $ANTLR end "ruleref"


    public class range_return : ParserRuleReturnScope
    {
        public GrammarAST tree;
        //@Override

        public GrammarAST getTree() { return tree; }
    };


    // $ANTLR start "range"
    // org\\antlr\\v4\\parse\\ANTLRParser.g:830:1: range : STRING_LITERAL RANGE ^ STRING_LITERAL ;
    public ANTLRParser.range_return range()  {
        ANTLRParser.range_return retval = new ANTLRParser.range_return();
        retval.start = input.LT(1);

        GrammarAST root_0 = null;

        Token STRING_LITERAL202 = null;
        Token RANGE203 = null;
        Token STRING_LITERAL204 = null;

        GrammarAST STRING_LITERAL202_tree = null;
        GrammarAST RANGE203_tree = null;
        GrammarAST STRING_LITERAL204_tree = null;

        try
        {
            // org\\antlr\\v4\\parse\\ANTLRParser.g:831:5: ( STRING_LITERAL RANGE ^ STRING_LITERAL )
            // org\\antlr\\v4\\parse\\ANTLRParser.g:831:7: STRING_LITERAL RANGE ^ STRING_LITERAL
            {
                root_0 = (GrammarAST)adaptor.nil();


                STRING_LITERAL202 = (Token)Match(input, STRING_LITERAL, FOLLOW_STRING_LITERAL_in_range3594);
                STRING_LITERAL202_tree = new TerminalAST(STRING_LITERAL202);
                adaptor.addChild(root_0, STRING_LITERAL202_tree);

                RANGE203 = (Token)Match(input, RANGE, FOLLOW_RANGE_in_range3599);
                RANGE203_tree = new RangeAST(RANGE203);
                root_0 = (GrammarAST)adaptor.becomeRoot(RANGE203_tree, root_0);

                STRING_LITERAL204 = (Token)Match(input, STRING_LITERAL, FOLLOW_STRING_LITERAL_in_range3605);
                STRING_LITERAL204_tree = new TerminalAST(STRING_LITERAL204);
                adaptor.addChild(root_0, STRING_LITERAL204_tree);

            }

            retval.stop = input.LT(-1);

            retval.tree = (GrammarAST)adaptor.rulePostProcessing(root_0);
            adaptor.setTokenBoundaries(retval.tree, retval.start, retval.stop);

        }
        catch (RecognitionException re)
        {
            reportError(re);
            recover(input, re);
            retval.tree = (GrammarAST)adaptor.errorNode(input, retval.start, input.LT(-1), re);
        }
        finally
        {
            // do for sure before leaving
        }
        return retval;
    }
    // $ANTLR end "range"


    public class terminal_return : ParserRuleReturnScope
    {
        public GrammarAST tree;
        //@Override

        public GrammarAST getTree() { return tree; }
    };


    // $ANTLR start "terminal"
    // org\\antlr\\v4\\parse\\ANTLRParser.g:834:1: terminal : ( TOKEN_REF ( elementOptions )? -> ^( TOKEN_REF ( elementOptions )? ) | STRING_LITERAL ( elementOptions )? -> ^( STRING_LITERAL ( elementOptions )? ) );
    public ANTLRParser.terminal_return terminal()  {
        ANTLRParser.terminal_return retval = new ANTLRParser.terminal_return();
        retval.start = input.LT(1);

        GrammarAST root_0 = null;

        Token TOKEN_REF205 = null;
        Token STRING_LITERAL207 = null;
        ParserRuleReturnScope elementOptions206 = null;
        ParserRuleReturnScope elementOptions208 = null;

        GrammarAST TOKEN_REF205_tree = null;
        GrammarAST STRING_LITERAL207_tree = null;
        RewriteRuleTokenStream stream_TOKEN_REF = new RewriteRuleTokenStream(adaptor, "token TOKEN_REF");
        RewriteRuleTokenStream stream_STRING_LITERAL = new RewriteRuleTokenStream(adaptor, "token STRING_LITERAL");
        RewriteRuleSubtreeStream stream_elementOptions = new RewriteRuleSubtreeStream(adaptor, "rule elementOptions");

        try
        {
            // org\\antlr\\v4\\parse\\ANTLRParser.g:841:2: ( TOKEN_REF ( elementOptions )? -> ^( TOKEN_REF ( elementOptions )? ) | STRING_LITERAL ( elementOptions )? -> ^( STRING_LITERAL ( elementOptions )? ) )
            int alt72 = 2;
            int LA72_0 = input.LA(1);
            if ((LA72_0 == TOKEN_REF))
            {
                alt72 = 1;
            }
            else if ((LA72_0 == STRING_LITERAL))
            {
                alt72 = 2;
            }

            else
            {
                NoViableAltException nvae =
                    new NoViableAltException("", 72, 0, input);
                throw nvae;
            }

            switch (alt72)
            {
                case 1:
                    // org\\antlr\\v4\\parse\\ANTLRParser.g:841:6: TOKEN_REF ( elementOptions )?
                    {
                        TOKEN_REF205 = (Token)Match(input, TOKEN_REF, FOLLOW_TOKEN_REF_in_terminal3629);
                        stream_TOKEN_REF.add(TOKEN_REF205);

                        // org\\antlr\\v4\\parse\\ANTLRParser.g:841:16: ( elementOptions )?
                        int alt70 = 2;
                        int LA70_0 = input.LA(1);
                        if ((LA70_0 == LT))
                        {
                            alt70 = 1;
                        }
                        switch (alt70)
                        {
                            case 1:
                                // org\\antlr\\v4\\parse\\ANTLRParser.g:841:16: elementOptions
                                {
                                    pushFollow(FOLLOW_elementOptions_in_terminal3631);
                                    elementOptions206 = elementOptions();
                                    state._fsp--;

                                    stream_elementOptions.add(elementOptions206.getTree());
                                }
                                break;

                        }


                        // AST REWRITE
                        // elements: TOKEN_REF, elementOptions
                        // token labels: 
                        // rule labels: retval
                        // token list labels: 
                        // rule list labels: 
                        // wildcard labels: 
                        retval.tree = root_0;
                        RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval != null ? retval.getTree() : null);

                        root_0 = (GrammarAST)adaptor.nil();
                        // 841:33: -> ^( TOKEN_REF ( elementOptions )? )
                        {
                            // org\\antlr\\v4\\parse\\ANTLRParser.g:841:36: ^( TOKEN_REF ( elementOptions )? )
                            {
                                GrammarAST root_1 = (GrammarAST)adaptor.nil();
                                root_1 = (GrammarAST)adaptor.becomeRoot(new TerminalAST(stream_TOKEN_REF.nextToken()), root_1);
                                // org\\antlr\\v4\\parse\\ANTLRParser.g:841:61: ( elementOptions )?
                                if (stream_elementOptions.hasNext())
                                {
                                    adaptor.addChild(root_1, stream_elementOptions.nextTree());
                                }
                                stream_elementOptions.reset();

                                adaptor.addChild(root_0, root_1);
                            }

                        }


                        retval.tree = root_0;

                    }
                    break;
                case 2:
                    // org\\antlr\\v4\\parse\\ANTLRParser.g:842:6: STRING_LITERAL ( elementOptions )?
                    {
                        STRING_LITERAL207 = (Token)Match(input, STRING_LITERAL, FOLLOW_STRING_LITERAL_in_terminal3652);
                        stream_STRING_LITERAL.add(STRING_LITERAL207);

                        // org\\antlr\\v4\\parse\\ANTLRParser.g:842:21: ( elementOptions )?
                        int alt71 = 2;
                        int LA71_0 = input.LA(1);
                        if ((LA71_0 == LT))
                        {
                            alt71 = 1;
                        }
                        switch (alt71)
                        {
                            case 1:
                                // org\\antlr\\v4\\parse\\ANTLRParser.g:842:21: elementOptions
                                {
                                    pushFollow(FOLLOW_elementOptions_in_terminal3654);
                                    elementOptions208 = elementOptions();
                                    state._fsp--;

                                    stream_elementOptions.add(elementOptions208.getTree());
                                }
                                break;

                        }


                        // AST REWRITE
                        // elements: STRING_LITERAL, elementOptions
                        // token labels: 
                        // rule labels: retval
                        // token list labels: 
                        // rule list labels: 
                        // wildcard labels: 
                        retval.tree = root_0;
                        RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval != null ? retval.getTree() : null);

                        root_0 = (GrammarAST)adaptor.nil();
                        // 842:37: -> ^( STRING_LITERAL ( elementOptions )? )
                        {
                            // org\\antlr\\v4\\parse\\ANTLRParser.g:842:40: ^( STRING_LITERAL ( elementOptions )? )
                            {
                                GrammarAST root_1 = (GrammarAST)adaptor.nil();
                                root_1 = (GrammarAST)adaptor.becomeRoot(new TerminalAST(stream_STRING_LITERAL.nextToken()), root_1);
                                // org\\antlr\\v4\\parse\\ANTLRParser.g:842:70: ( elementOptions )?
                                if (stream_elementOptions.hasNext())
                                {
                                    adaptor.addChild(root_1, stream_elementOptions.nextTree());
                                }
                                stream_elementOptions.reset();

                                adaptor.addChild(root_0, root_1);
                            }

                        }


                        retval.tree = root_0;

                    }
                    break;

            }
            retval.stop = input.LT(-1);

            retval.tree = (GrammarAST)adaptor.rulePostProcessing(root_0);
            adaptor.setTokenBoundaries(retval.tree, retval.start, retval.stop);


            GrammarAST options = (GrammarAST)retval.tree.getFirstChildWithType(ANTLRParser.ELEMENT_OPTIONS);
            if (options != null)
            {
                Grammar.setNodeOptions(retval.tree, options);
            }

        }
        catch (RecognitionException re)
        {
            reportError(re);
            recover(input, re);
            retval.tree = (GrammarAST)adaptor.errorNode(input, retval.start, input.LT(-1), re);
        }
        finally
        {
            // do for sure before leaving
        }
        return retval;
    }
    // $ANTLR end "terminal"


    public class elementOptions_return : ParserRuleReturnScope
    {
        public GrammarAST tree;
        //@Override

        public GrammarAST getTree() { return tree; }
    };


    // $ANTLR start "elementOptions"
    // org\\antlr\\v4\\parse\\ANTLRParser.g:847:1: elementOptions : LT ( elementOption ( COMMA elementOption )* )? GT -> ^( ELEMENT_OPTIONS[$LT,\"ELEMENT_OPTIONS\"] ( elementOption )* ) ;
    public ANTLRParser.elementOptions_return elementOptions()  {
        ANTLRParser.elementOptions_return retval = new ANTLRParser.elementOptions_return();
        retval.start = input.LT(1);

        GrammarAST root_0 = null;

        Token LT209 = null;
        Token COMMA211 = null;
        Token GT213 = null;
        ParserRuleReturnScope elementOption210 = null;
        ParserRuleReturnScope elementOption212 = null;

        GrammarAST LT209_tree = null;
        GrammarAST COMMA211_tree = null;
        GrammarAST GT213_tree = null;
        RewriteRuleTokenStream stream_COMMA = new RewriteRuleTokenStream(adaptor, "token COMMA");
        RewriteRuleTokenStream stream_LT = new RewriteRuleTokenStream(adaptor, "token LT");
        RewriteRuleTokenStream stream_GT = new RewriteRuleTokenStream(adaptor, "token GT");
        RewriteRuleSubtreeStream stream_elementOption = new RewriteRuleSubtreeStream(adaptor, "rule elementOption");

        try
        {
            // org\\antlr\\v4\\parse\\ANTLRParser.g:848:5: ( LT ( elementOption ( COMMA elementOption )* )? GT -> ^( ELEMENT_OPTIONS[$LT,\"ELEMENT_OPTIONS\"] ( elementOption )* ) )
            // org\\antlr\\v4\\parse\\ANTLRParser.g:848:9: LT ( elementOption ( COMMA elementOption )* )? GT
            {
                LT209 = (Token)Match(input, LT, FOLLOW_LT_in_elementOptions3685);
                stream_LT.add(LT209);

                // org\\antlr\\v4\\parse\\ANTLRParser.g:848:12: ( elementOption ( COMMA elementOption )* )?
                int alt74 = 2;
                int LA74_0 = input.LA(1);
                if ((LA74_0 == RULE_REF || LA74_0 == TOKEN_REF))
                {
                    alt74 = 1;
                }
                switch (alt74)
                {
                    case 1:
                        // org\\antlr\\v4\\parse\\ANTLRParser.g:848:13: elementOption ( COMMA elementOption )*
                        {
                            pushFollow(FOLLOW_elementOption_in_elementOptions3688);
                            elementOption210 = elementOption();
                            state._fsp--;

                            stream_elementOption.add(elementOption210.getTree());
                        // org\\antlr\\v4\\parse\\ANTLRParser.g:848:27: ( COMMA elementOption )*
                        loop73:
                            while (true)
                            {
                                int alt73 = 2;
                                int LA73_0 = input.LA(1);
                                if ((LA73_0 == COMMA))
                                {
                                    alt73 = 1;
                                }

                                switch (alt73)
                                {
                                    case 1:
                                        // org\\antlr\\v4\\parse\\ANTLRParser.g:848:28: COMMA elementOption
                                        {
                                            COMMA211 = (Token)Match(input, COMMA, FOLLOW_COMMA_in_elementOptions3691);
                                            stream_COMMA.add(COMMA211);

                                            pushFollow(FOLLOW_elementOption_in_elementOptions3693);
                                            elementOption212 = elementOption();
                                            state._fsp--;

                                            stream_elementOption.add(elementOption212.getTree());
                                        }
                                        break;

                                    default:
                                        goto exit73;
                                        //break loop73;
                                }
                            }
                        exit73:
                            ;
                        }
                        break;

                }

                GT213 = (Token)Match(input, GT, FOLLOW_GT_in_elementOptions3699);
                stream_GT.add(GT213);


                // AST REWRITE
                // elements: elementOption
                // token labels: 
                // rule labels: retval
                // token list labels: 
                // rule list labels: 
                // wildcard labels: 
                retval.tree = root_0;
                RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval != null ? retval.getTree() : null);

                root_0 = (GrammarAST)adaptor.nil();
                // 849:13: -> ^( ELEMENT_OPTIONS[$LT,\"ELEMENT_OPTIONS\"] ( elementOption )* )
                {
                    // org\\antlr\\v4\\parse\\ANTLRParser.g:849:16: ^( ELEMENT_OPTIONS[$LT,\"ELEMENT_OPTIONS\"] ( elementOption )* )
                    {
                        GrammarAST root_1 = (GrammarAST)adaptor.nil();
                        root_1 = (GrammarAST)adaptor.becomeRoot((GrammarAST)adaptor.create(ELEMENT_OPTIONS, LT209, "ELEMENT_OPTIONS"), root_1);
                        // org\\antlr\\v4\\parse\\ANTLRParser.g:849:57: ( elementOption )*
                        while (stream_elementOption.hasNext())
                        {
                            adaptor.addChild(root_1, stream_elementOption.nextTree());
                        }
                        stream_elementOption.reset();

                        adaptor.addChild(root_0, root_1);
                    }

                }


                retval.tree = root_0;

            }

            retval.stop = input.LT(-1);

            retval.tree = (GrammarAST)adaptor.rulePostProcessing(root_0);
            adaptor.setTokenBoundaries(retval.tree, retval.start, retval.stop);

        }
        catch (RecognitionException re)
        {
            reportError(re);
            recover(input, re);
            retval.tree = (GrammarAST)adaptor.errorNode(input, retval.start, input.LT(-1), re);
        }
        finally
        {
            // do for sure before leaving
        }
        return retval;
    }
    // $ANTLR end "elementOptions"


    public class elementOption_return : ParserRuleReturnScope
    {
        public GrammarAST tree;
        //@Override

        public GrammarAST getTree() { return tree; }
    };


    // $ANTLR start "elementOption"
    // org\\antlr\\v4\\parse\\ANTLRParser.g:854:1: elementOption : ( qid | id ASSIGN ^ optionValue );
    public ANTLRParser.elementOption_return elementOption()  {
        ANTLRParser.elementOption_return retval = new ANTLRParser.elementOption_return();
        retval.start = input.LT(1);

        GrammarAST root_0 = null;

        Token ASSIGN216 = null;
        ParserRuleReturnScope qid214 = null;
        ParserRuleReturnScope id215 = null;
        ParserRuleReturnScope optionValue217 = null;

        GrammarAST ASSIGN216_tree = null;

        try
        {
            // org\\antlr\\v4\\parse\\ANTLRParser.g:855:5: ( qid | id ASSIGN ^ optionValue )
            int alt75 = 2;
            int LA75_0 = input.LA(1);
            if ((LA75_0 == RULE_REF))
            {
                int LA75_1 = input.LA(2);
                if ((LA75_1 == COMMA || LA75_1 == DOT || LA75_1 == GT))
                {
                    alt75 = 1;
                }
                else if ((LA75_1 == ASSIGN))
                {
                    alt75 = 2;
                }

                else
                {
                    int nvaeMark = input.mark();
                    try
                    {
                        input.consume();
                        NoViableAltException nvae =
                            new NoViableAltException("", 75, 1, input);
                        throw nvae;
                    }
                    finally
                    {
                        input.rewind(nvaeMark);
                    }
                }

            }
            else if ((LA75_0 == TOKEN_REF))
            {
                int LA75_2 = input.LA(2);
                if ((LA75_2 == COMMA || LA75_2 == DOT || LA75_2 == GT))
                {
                    alt75 = 1;
                }
                else if ((LA75_2 == ASSIGN))
                {
                    alt75 = 2;
                }

                else
                {
                    int nvaeMark = input.mark();
                    try
                    {
                        input.consume();
                        NoViableAltException nvae =
                            new NoViableAltException("", 75, 2, input);
                        throw nvae;
                    }
                    finally
                    {
                        input.rewind(nvaeMark);
                    }
                }

            }

            else
            {
                NoViableAltException nvae =
                    new NoViableAltException("", 75, 0, input);
                throw nvae;
            }

            switch (alt75)
            {
                case 1:
                    // org\\antlr\\v4\\parse\\ANTLRParser.g:856:7: qid
                    {
                        root_0 = (GrammarAST)adaptor.nil();


                        pushFollow(FOLLOW_qid_in_elementOption3747);
                        qid214 = qid();
                        state._fsp--;

                        adaptor.addChild(root_0, qid214.getTree());

                    }
                    break;
                case 2:
                    // org\\antlr\\v4\\parse\\ANTLRParser.g:857:7: id ASSIGN ^ optionValue
                    {
                        root_0 = (GrammarAST)adaptor.nil();


                        pushFollow(FOLLOW_id_in_elementOption3755);
                        id215 = id();
                        state._fsp--;

                        adaptor.addChild(root_0, id215.getTree());

                        ASSIGN216 = (Token)Match(input, ASSIGN, FOLLOW_ASSIGN_in_elementOption3757);
                        ASSIGN216_tree = (GrammarAST)adaptor.create(ASSIGN216);
                        root_0 = (GrammarAST)adaptor.becomeRoot(ASSIGN216_tree, root_0);

                        pushFollow(FOLLOW_optionValue_in_elementOption3760);
                        optionValue217 = optionValue();
                        state._fsp--;

                        adaptor.addChild(root_0, optionValue217.getTree());

                    }
                    break;

            }
            retval.stop = input.LT(-1);

            retval.tree = (GrammarAST)adaptor.rulePostProcessing(root_0);
            adaptor.setTokenBoundaries(retval.tree, retval.start, retval.stop);

        }
        catch (RecognitionException re)
        {
            reportError(re);
            recover(input, re);
            retval.tree = (GrammarAST)adaptor.errorNode(input, retval.start, input.LT(-1), re);
        }
        finally
        {
            // do for sure before leaving
        }
        return retval;
    }
    // $ANTLR end "elementOption"


    public class id_return : ParserRuleReturnScope
    {
        public GrammarAST tree;
        //@Override

        public GrammarAST getTree() { return tree; }
    };


    // $ANTLR start "id"
    // org\\antlr\\v4\\parse\\ANTLRParser.g:864:1: id : ( RULE_REF -> ID[$RULE_REF] | TOKEN_REF -> ID[$TOKEN_REF] );
    public ANTLRParser.id_return id()  {
        ANTLRParser.id_return retval = new ANTLRParser.id_return();
        retval.start = input.LT(1);

        GrammarAST root_0 = null;

        Token RULE_REF218 = null;
        Token TOKEN_REF219 = null;

        GrammarAST RULE_REF218_tree = null;
        GrammarAST TOKEN_REF219_tree = null;
        RewriteRuleTokenStream stream_TOKEN_REF = new RewriteRuleTokenStream(adaptor, "token TOKEN_REF");
        RewriteRuleTokenStream stream_RULE_REF = new RewriteRuleTokenStream(adaptor, "token RULE_REF");

        paraphrases.push("looking for an identifier");
        try
        {
            // org\\antlr\\v4\\parse\\ANTLRParser.g:867:5: ( RULE_REF -> ID[$RULE_REF] | TOKEN_REF -> ID[$TOKEN_REF] )
            int alt76 = 2;
            int LA76_0 = input.LA(1);
            if ((LA76_0 == RULE_REF))
            {
                alt76 = 1;
            }
            else if ((LA76_0 == TOKEN_REF))
            {
                alt76 = 2;
            }

            else
            {
                NoViableAltException nvae =
                    new NoViableAltException("", 76, 0, input);
                throw nvae;
            }

            switch (alt76)
            {
                case 1:
                    // org\\antlr\\v4\\parse\\ANTLRParser.g:867:7: RULE_REF
                    {
                        RULE_REF218 = (Token)Match(input, RULE_REF, FOLLOW_RULE_REF_in_id3791);
                        stream_RULE_REF.add(RULE_REF218);


                        // AST REWRITE
                        // elements: 
                        // token labels: 
                        // rule labels: retval
                        // token list labels: 
                        // rule list labels: 
                        // wildcard labels: 
                        retval.tree = root_0;
                        RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval != null ? retval.getTree() : null);

                        root_0 = (GrammarAST)adaptor.nil();
                        // 867:17: -> ID[$RULE_REF]
                        {
                            adaptor.addChild(root_0, (GrammarAST)adaptor.create(ID, RULE_REF218));
                        }


                        retval.tree = root_0;

                    }
                    break;
                case 2:
                    // org\\antlr\\v4\\parse\\ANTLRParser.g:868:7: TOKEN_REF
                    {
                        TOKEN_REF219 = (Token)Match(input, TOKEN_REF, FOLLOW_TOKEN_REF_in_id3804);
                        stream_TOKEN_REF.add(TOKEN_REF219);


                        // AST REWRITE
                        // elements: 
                        // token labels: 
                        // rule labels: retval
                        // token list labels: 
                        // rule list labels: 
                        // wildcard labels: 
                        retval.tree = root_0;
                        RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval != null ? retval.getTree() : null);

                        root_0 = (GrammarAST)adaptor.nil();
                        // 868:17: -> ID[$TOKEN_REF]
                        {
                            adaptor.addChild(root_0, (GrammarAST)adaptor.create(ID, TOKEN_REF219));
                        }


                        retval.tree = root_0;

                    }
                    break;

            }
            retval.stop = input.LT(-1);

            retval.tree = (GrammarAST)adaptor.rulePostProcessing(root_0);
            adaptor.setTokenBoundaries(retval.tree, retval.start, retval.stop);

            paraphrases.pop();
        }
        catch (RecognitionException re)
        {
            reportError(re);
            recover(input, re);
            retval.tree = (GrammarAST)adaptor.errorNode(input, retval.start, input.LT(-1), re);
        }
        finally
        {
            // do for sure before leaving
        }
        return retval;
    }
    // $ANTLR end "id"


    public class qid_return : ParserRuleReturnScope
    {
        public GrammarAST tree;
        //@Override

        public GrammarAST getTree() { return tree; }
    };


    // $ANTLR start "qid"
    // org\\antlr\\v4\\parse\\ANTLRParser.g:871:1: qid : id ( DOT id )* -> ID[$qid.start, $text] ;
    public ANTLRParser.qid_return qid()  {
        ANTLRParser.qid_return retval = new ANTLRParser.qid_return();
        retval.start = input.LT(1);

        GrammarAST root_0 = null;

        Token DOT221 = null;
        ParserRuleReturnScope id220 = null;
        ParserRuleReturnScope id222 = null;

        GrammarAST DOT221_tree = null;
        RewriteRuleTokenStream stream_DOT = new RewriteRuleTokenStream(adaptor, "token DOT");
        RewriteRuleSubtreeStream stream_id = new RewriteRuleSubtreeStream(adaptor, "rule id");

        paraphrases.push("looking for a qualified identifier");
        try
        {
            // org\\antlr\\v4\\parse\\ANTLRParser.g:874:2: ( id ( DOT id )* -> ID[$qid.start, $text] )
            // org\\antlr\\v4\\parse\\ANTLRParser.g:874:4: id ( DOT id )*
            {
                pushFollow(FOLLOW_id_in_qid3832);
                id220 = id();
                state._fsp--;

                stream_id.add(id220.getTree());
            // org\\antlr\\v4\\parse\\ANTLRParser.g:874:7: ( DOT id )*
            loop77:
                while (true)
                {
                    int alt77 = 2;
                    int LA77_0 = input.LA(1);
                    if ((LA77_0 == DOT))
                    {
                        alt77 = 1;
                    }

                    switch (alt77)
                    {
                        case 1:
                            // org\\antlr\\v4\\parse\\ANTLRParser.g:874:8: DOT id
                            {
                                DOT221 = (Token)Match(input, DOT, FOLLOW_DOT_in_qid3835);
                                stream_DOT.add(DOT221);

                                pushFollow(FOLLOW_id_in_qid3837);
                                id222 = id();
                                state._fsp--;

                                stream_id.add(id222.getTree());
                            }
                            break;

                        default:
                            goto exit77;
                            //break loop77;
                    }
                }
            exit77:

                // AST REWRITE
                // elements: 
                // token labels: 
                // rule labels: retval
                // token list labels: 
                // rule list labels: 
                // wildcard labels: 
                retval.tree = root_0;
                RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval != null ? retval.getTree() : null);

                root_0 = (GrammarAST)adaptor.nil();
                // 874:17: -> ID[$qid.start, $text]
                {
                    adaptor.addChild(root_0, (GrammarAST)adaptor.create(ID, (retval.start), input.toString(retval.start, input.LT(-1))));
                }


                retval.tree = root_0;

            }

            retval.stop = input.LT(-1);

            retval.tree = (GrammarAST)adaptor.rulePostProcessing(root_0);
            adaptor.setTokenBoundaries(retval.tree, retval.start, retval.stop);

            paraphrases.pop();
        }
        catch (RecognitionException re)
        {
            reportError(re);
            recover(input, re);
            retval.tree = (GrammarAST)adaptor.errorNode(input, retval.start, input.LT(-1), re);
        }
        finally
        {
            // do for sure before leaving
        }
        return retval;
    }
    // $ANTLR end "qid"


    public class alternativeEntry_return : ParserRuleReturnScope
    {
        public GrammarAST tree;
        //@Override

        public GrammarAST getTree() { return tree; }
    };


    // $ANTLR start "alternativeEntry"
    // org\\antlr\\v4\\parse\\ANTLRParser.g:877:1: alternativeEntry : alternative EOF ;
    public ANTLRParser.alternativeEntry_return alternativeEntry()  {
        ANTLRParser.alternativeEntry_return retval = new ANTLRParser.alternativeEntry_return();
        retval.start = input.LT(1);

        GrammarAST root_0 = null;

        Token EOF224 = null;
        ParserRuleReturnScope alternative223 = null;

        GrammarAST EOF224_tree = null;

        try
        {
            // org\\antlr\\v4\\parse\\ANTLRParser.g:877:18: ( alternative EOF )
            // org\\antlr\\v4\\parse\\ANTLRParser.g:877:20: alternative EOF
            {
                root_0 = (GrammarAST)adaptor.nil();


                pushFollow(FOLLOW_alternative_in_alternativeEntry3854);
                alternative223 = alternative();
                state._fsp--;

                adaptor.addChild(root_0, alternative223.getTree());

                EOF224 = (Token)Match(input, EOF, FOLLOW_EOF_in_alternativeEntry3856);
                EOF224_tree = (GrammarAST)adaptor.create(EOF224);
                adaptor.addChild(root_0, EOF224_tree);

            }

            retval.stop = input.LT(-1);

            retval.tree = (GrammarAST)adaptor.rulePostProcessing(root_0);
            adaptor.setTokenBoundaries(retval.tree, retval.start, retval.stop);

        }
        catch (RecognitionException re)
        {
            reportError(re);
            recover(input, re);
            retval.tree = (GrammarAST)adaptor.errorNode(input, retval.start, input.LT(-1), re);
        }
        finally
        {
            // do for sure before leaving
        }
        return retval;
    }
    // $ANTLR end "alternativeEntry"


    public class elementEntry_return : ParserRuleReturnScope
    {
        public GrammarAST tree;
        //@Override

        public GrammarAST getTree() { return tree; }
    };


    // $ANTLR start "elementEntry"
    // org\\antlr\\v4\\parse\\ANTLRParser.g:878:1: elementEntry : element EOF ;
    public ANTLRParser.elementEntry_return elementEntry()  {
        ANTLRParser.elementEntry_return retval = new ANTLRParser.elementEntry_return();
        retval.start = input.LT(1);

        GrammarAST root_0 = null;

        Token EOF226 = null;
        ParserRuleReturnScope element225 = null;

        GrammarAST EOF226_tree = null;

        try
        {
            // org\\antlr\\v4\\parse\\ANTLRParser.g:878:14: ( element EOF )
            // org\\antlr\\v4\\parse\\ANTLRParser.g:878:16: element EOF
            {
                root_0 = (GrammarAST)adaptor.nil();


                pushFollow(FOLLOW_element_in_elementEntry3865);
                element225 = element();
                state._fsp--;

                adaptor.addChild(root_0, element225.getTree());

                EOF226 = (Token)Match(input, EOF, FOLLOW_EOF_in_elementEntry3867);
                EOF226_tree = (GrammarAST)adaptor.create(EOF226);
                adaptor.addChild(root_0, EOF226_tree);

            }

            retval.stop = input.LT(-1);

            retval.tree = (GrammarAST)adaptor.rulePostProcessing(root_0);
            adaptor.setTokenBoundaries(retval.tree, retval.start, retval.stop);

        }
        catch (RecognitionException re)
        {
            reportError(re);
            recover(input, re);
            retval.tree = (GrammarAST)adaptor.errorNode(input, retval.start, input.LT(-1), re);
        }
        finally
        {
            // do for sure before leaving
        }
        return retval;
    }
    // $ANTLR end "elementEntry"


    public class ruleEntry_return : ParserRuleReturnScope
    {
        public GrammarAST tree;
        //@Override

        public GrammarAST getTree() { return tree; }
    };


    // $ANTLR start "ruleEntry"
    // org\\antlr\\v4\\parse\\ANTLRParser.g:879:1: ruleEntry : rule EOF ;
    public ANTLRParser.ruleEntry_return ruleEntry()  {
        ANTLRParser.ruleEntry_return retval = new ANTLRParser.ruleEntry_return();
        retval.start = input.LT(1);

        GrammarAST root_0 = null;

        Token EOF228 = null;
        ParserRuleReturnScope rule227 = null;

        GrammarAST EOF228_tree = null;

        try
        {
            // org\\antlr\\v4\\parse\\ANTLRParser.g:879:11: ( rule EOF )
            // org\\antlr\\v4\\parse\\ANTLRParser.g:879:13: rule EOF
            {
                root_0 = (GrammarAST)adaptor.nil();


                pushFollow(FOLLOW_rule_in_ruleEntry3875);
                rule227 = rule();
                state._fsp--;

                adaptor.addChild(root_0, rule227.getTree());

                EOF228 = (Token)Match(input, EOF, FOLLOW_EOF_in_ruleEntry3877);
                EOF228_tree = (GrammarAST)adaptor.create(EOF228);
                adaptor.addChild(root_0, EOF228_tree);

            }

            retval.stop = input.LT(-1);

            retval.tree = (GrammarAST)adaptor.rulePostProcessing(root_0);
            adaptor.setTokenBoundaries(retval.tree, retval.start, retval.stop);

        }
        catch (RecognitionException re)
        {
            reportError(re);
            recover(input, re);
            retval.tree = (GrammarAST)adaptor.errorNode(input, retval.start, input.LT(-1), re);
        }
        finally
        {
            // do for sure before leaving
        }
        return retval;
    }
    // $ANTLR end "ruleEntry"


    public class blockEntry_return : ParserRuleReturnScope
    {
        public GrammarAST tree;
        //@Override

        public GrammarAST getTree() { return tree; }
    };


    // $ANTLR start "blockEntry"
    // org\\antlr\\v4\\parse\\ANTLRParser.g:880:1: blockEntry : block EOF ;
    public ANTLRParser.blockEntry_return blockEntry()  {
        ANTLRParser.blockEntry_return retval = new ANTLRParser.blockEntry_return();
        retval.start = input.LT(1);

        GrammarAST root_0 = null;

        Token EOF230 = null;
        ParserRuleReturnScope block229 = null;

        GrammarAST EOF230_tree = null;

        try
        {
            // org\\antlr\\v4\\parse\\ANTLRParser.g:880:12: ( block EOF )
            // org\\antlr\\v4\\parse\\ANTLRParser.g:880:14: block EOF
            {
                root_0 = (GrammarAST)adaptor.nil();


                pushFollow(FOLLOW_block_in_blockEntry3885);
                block229 = block();
                state._fsp--;

                adaptor.addChild(root_0, block229.getTree());

                EOF230 = (Token)Match(input, EOF, FOLLOW_EOF_in_blockEntry3887);
                EOF230_tree = (GrammarAST)adaptor.create(EOF230);
                adaptor.addChild(root_0, EOF230_tree);

            }

            retval.stop = input.LT(-1);

            retval.tree = (GrammarAST)adaptor.rulePostProcessing(root_0);
            adaptor.setTokenBoundaries(retval.tree, retval.start, retval.stop);

        }
        catch (RecognitionException re)
        {
            reportError(re);
            recover(input, re);
            retval.tree = (GrammarAST)adaptor.errorNode(input, retval.start, input.LT(-1), re);
        }
        finally
        {
            // do for sure before leaving
        }
        return retval;
    }

    // $ANTLR end "blockEntry"

    // Delegated rules



    public static readonly BitSet FOLLOW_grammarType_in_grammarSpec281 = new BitSet(new long[] { 0x4040000000000000L });
    public static readonly BitSet FOLLOW_id_in_grammarSpec283 = new BitSet(new long[] { 0x0080000000000000L });
    public static readonly BitSet FOLLOW_SEMI_in_grammarSpec285 = new BitSet(new long[] { 0x6040040021002800L });
    public static readonly BitSet FOLLOW_sync_in_grammarSpec323 = new BitSet(new long[] { 0x6040040021002800L });
    public static readonly BitSet FOLLOW_prequelConstruct_in_grammarSpec327 = new BitSet(new long[] { 0x6040040021002800L });
    public static readonly BitSet FOLLOW_sync_in_grammarSpec329 = new BitSet(new long[] { 0x6040040021002800L });
    public static readonly BitSet FOLLOW_rules_in_grammarSpec354 = new BitSet(new long[] { 0x0000001000000000L });
    public static readonly BitSet FOLLOW_modeSpec_in_grammarSpec360 = new BitSet(new long[] { 0x0000001000000000L });
    public static readonly BitSet FOLLOW_EOF_in_grammarSpec398 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_LEXER_in_grammarType568 = new BitSet(new long[] { 0x0000000002000000L });
    public static readonly BitSet FOLLOW_GRAMMAR_in_grammarType572 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_PARSER_in_grammarType595 = new BitSet(new long[] { 0x0000000002000000L });
    public static readonly BitSet FOLLOW_GRAMMAR_in_grammarType599 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_GRAMMAR_in_grammarType620 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_optionsSpec_in_prequelConstruct662 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_delegateGrammars_in_prequelConstruct685 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_tokensSpec_in_prequelConstruct729 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_channelsSpec_in_prequelConstruct739 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_action_in_prequelConstruct776 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_OPTIONS_in_optionsSpec791 = new BitSet(new long[] { 0x4048000000000000L });
    public static readonly BitSet FOLLOW_option_in_optionsSpec794 = new BitSet(new long[] { 0x0080000000000000L });
    public static readonly BitSet FOLLOW_SEMI_in_optionsSpec796 = new BitSet(new long[] { 0x4048000000000000L });
    public static readonly BitSet FOLLOW_RBRACE_in_optionsSpec800 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_id_in_option829 = new BitSet(new long[] { 0x0000000000000400L });
    public static readonly BitSet FOLLOW_ASSIGN_in_option831 = new BitSet(new long[] { 0x4840000040000010L });
    public static readonly BitSet FOLLOW_optionValue_in_option834 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_qid_in_optionValue877 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_STRING_LITERAL_in_optionValue885 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_ACTION_in_optionValue890 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_INT_in_optionValue901 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_IMPORT_in_delegateGrammars917 = new BitSet(new long[] { 0x4040000000000000L });
    public static readonly BitSet FOLLOW_delegateGrammar_in_delegateGrammars919 = new BitSet(new long[] { 0x0080000000010000L });
    public static readonly BitSet FOLLOW_COMMA_in_delegateGrammars922 = new BitSet(new long[] { 0x4040000000000000L });
    public static readonly BitSet FOLLOW_delegateGrammar_in_delegateGrammars924 = new BitSet(new long[] { 0x0080000000010000L });
    public static readonly BitSet FOLLOW_SEMI_in_delegateGrammars928 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_id_in_delegateGrammar955 = new BitSet(new long[] { 0x0000000000000400L });
    public static readonly BitSet FOLLOW_ASSIGN_in_delegateGrammar957 = new BitSet(new long[] { 0x4040000000000000L });
    public static readonly BitSet FOLLOW_id_in_delegateGrammar960 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_id_in_delegateGrammar970 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_TOKENS_SPEC_in_tokensSpec984 = new BitSet(new long[] { 0x4040000000000000L });
    public static readonly BitSet FOLLOW_id_in_tokensSpec986 = new BitSet(new long[] { 0x0008000000010000L });
    public static readonly BitSet FOLLOW_COMMA_in_tokensSpec989 = new BitSet(new long[] { 0x4040000000000000L });
    public static readonly BitSet FOLLOW_id_in_tokensSpec991 = new BitSet(new long[] { 0x0008000000010000L });
    public static readonly BitSet FOLLOW_RBRACE_in_tokensSpec995 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_TOKENS_SPEC_in_tokensSpec1012 = new BitSet(new long[] { 0x0008000000000000L });
    public static readonly BitSet FOLLOW_RBRACE_in_tokensSpec1014 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_CHANNELS_in_channelsSpec1027 = new BitSet(new long[] { 0x4040000000000000L });
    public static readonly BitSet FOLLOW_id_in_channelsSpec1030 = new BitSet(new long[] { 0x0008000000010000L });
    public static readonly BitSet FOLLOW_COMMA_in_channelsSpec1033 = new BitSet(new long[] { 0x4040000000000000L });
    public static readonly BitSet FOLLOW_id_in_channelsSpec1036 = new BitSet(new long[] { 0x0008000000010000L });
    public static readonly BitSet FOLLOW_RBRACE_in_channelsSpec1040 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_AT_in_action1057 = new BitSet(new long[] { 0x4040100080000000L });
    public static readonly BitSet FOLLOW_actionScopeName_in_action1060 = new BitSet(new long[] { 0x0000000000008000L });
    public static readonly BitSet FOLLOW_COLONCOLON_in_action1062 = new BitSet(new long[] { 0x4040000000000000L });
    public static readonly BitSet FOLLOW_id_in_action1066 = new BitSet(new long[] { 0x0000000000000010L });
    public static readonly BitSet FOLLOW_ACTION_in_action1068 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_id_in_actionScopeName1097 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_LEXER_in_actionScopeName1102 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_PARSER_in_actionScopeName1117 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_MODE_in_modeSpec1136 = new BitSet(new long[] { 0x4040000000000000L });
    public static readonly BitSet FOLLOW_id_in_modeSpec1138 = new BitSet(new long[] { 0x0080000000000000L });
    public static readonly BitSet FOLLOW_SEMI_in_modeSpec1140 = new BitSet(new long[] { 0x4000000001000000L });
    public static readonly BitSet FOLLOW_sync_in_modeSpec1142 = new BitSet(new long[] { 0x4000000001000002L });
    public static readonly BitSet FOLLOW_lexerRule_in_modeSpec1145 = new BitSet(new long[] { 0x4000000001000000L });
    public static readonly BitSet FOLLOW_sync_in_modeSpec1147 = new BitSet(new long[] { 0x4000000001000002L });
    public static readonly BitSet FOLLOW_sync_in_rules1178 = new BitSet(new long[] { 0x4040000001000002L });
    public static readonly BitSet FOLLOW_rule_in_rules1181 = new BitSet(new long[] { 0x4040000001000000L });
    public static readonly BitSet FOLLOW_sync_in_rules1183 = new BitSet(new long[] { 0x4040000001000002L });
    public static readonly BitSet FOLLOW_parserRule_in_rule1245 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_lexerRule_in_rule1250 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_RULE_REF_in_parserRule1299 = new BitSet(new long[] { 0x1010040200000900L });
    public static readonly BitSet FOLLOW_ARG_ACTION_in_parserRule1329 = new BitSet(new long[] { 0x1010040200000800L });
    public static readonly BitSet FOLLOW_ruleReturns_in_parserRule1336 = new BitSet(new long[] { 0x1000040200000800L });
    public static readonly BitSet FOLLOW_throwsSpec_in_parserRule1343 = new BitSet(new long[] { 0x0000040200000800L });
    public static readonly BitSet FOLLOW_localsSpec_in_parserRule1350 = new BitSet(new long[] { 0x0000040000000800L });
    public static readonly BitSet FOLLOW_rulePrequels_in_parserRule1388 = new BitSet(new long[] { 0x0000000000004000L });
    public static readonly BitSet FOLLOW_COLON_in_parserRule1397 = new BitSet(new long[] { 0x4940808C00100010L });
    public static readonly BitSet FOLLOW_ruleBlock_in_parserRule1420 = new BitSet(new long[] { 0x0080000000000000L });
    public static readonly BitSet FOLLOW_SEMI_in_parserRule1429 = new BitSet(new long[] { 0x0000000000801000L });
    public static readonly BitSet FOLLOW_exceptionGroup_in_parserRule1438 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_exceptionHandler_in_exceptionGroup1521 = new BitSet(new long[] { 0x0000000000801002L });
    public static readonly BitSet FOLLOW_finallyClause_in_exceptionGroup1524 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_CATCH_in_exceptionHandler1541 = new BitSet(new long[] { 0x0000000000000100L });
    public static readonly BitSet FOLLOW_ARG_ACTION_in_exceptionHandler1543 = new BitSet(new long[] { 0x0000000000000010L });
    public static readonly BitSet FOLLOW_ACTION_in_exceptionHandler1545 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_FINALLY_in_finallyClause1572 = new BitSet(new long[] { 0x0000000000000010L });
    public static readonly BitSet FOLLOW_ACTION_in_finallyClause1574 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_sync_in_rulePrequels1606 = new BitSet(new long[] { 0x0000040000000802L });
    public static readonly BitSet FOLLOW_rulePrequel_in_rulePrequels1609 = new BitSet(new long[] { 0x0000040000000800L });
    public static readonly BitSet FOLLOW_sync_in_rulePrequels1611 = new BitSet(new long[] { 0x0000040000000802L });
    public static readonly BitSet FOLLOW_optionsSpec_in_rulePrequel1635 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_ruleAction_in_rulePrequel1643 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_RETURNS_in_ruleReturns1663 = new BitSet(new long[] { 0x0000000000000100L });
    public static readonly BitSet FOLLOW_ARG_ACTION_in_ruleReturns1666 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_THROWS_in_throwsSpec1694 = new BitSet(new long[] { 0x4040000000000000L });
    public static readonly BitSet FOLLOW_qid_in_throwsSpec1696 = new BitSet(new long[] { 0x0000000000010002L });
    public static readonly BitSet FOLLOW_COMMA_in_throwsSpec1699 = new BitSet(new long[] { 0x4040000000000000L });
    public static readonly BitSet FOLLOW_qid_in_throwsSpec1701 = new BitSet(new long[] { 0x0000000000010002L });
    public static readonly BitSet FOLLOW_LOCALS_in_localsSpec1726 = new BitSet(new long[] { 0x0000000000000100L });
    public static readonly BitSet FOLLOW_ARG_ACTION_in_localsSpec1729 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_AT_in_ruleAction1752 = new BitSet(new long[] { 0x4040000000000000L });
    public static readonly BitSet FOLLOW_id_in_ruleAction1754 = new BitSet(new long[] { 0x0000000000000010L });
    public static readonly BitSet FOLLOW_ACTION_in_ruleAction1756 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_ruleAltList_in_ruleBlock1794 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_labeledAlt_in_ruleAltList1830 = new BitSet(new long[] { 0x0000080000000002L });
    public static readonly BitSet FOLLOW_OR_in_ruleAltList1833 = new BitSet(new long[] { 0x4940808C00100010L });
    public static readonly BitSet FOLLOW_labeledAlt_in_ruleAltList1835 = new BitSet(new long[] { 0x0000080000000002L });
    public static readonly BitSet FOLLOW_alternative_in_labeledAlt1853 = new BitSet(new long[] { 0x0000800000000002L });
    public static readonly BitSet FOLLOW_POUND_in_labeledAlt1859 = new BitSet(new long[] { 0x4040000000000000L });
    public static readonly BitSet FOLLOW_id_in_labeledAlt1862 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_FRAGMENT_in_lexerRule1894 = new BitSet(new long[] { 0x4000000000000000L });
    public static readonly BitSet FOLLOW_TOKEN_REF_in_lexerRule1900 = new BitSet(new long[] { 0x0000040000004000L });
    public static readonly BitSet FOLLOW_optionsSpec_in_lexerRule1906 = new BitSet(new long[] { 0x0000000000004000L });
    public static readonly BitSet FOLLOW_COLON_in_lexerRule1913 = new BitSet(new long[] { 0x4944008500100010L });
    public static readonly BitSet FOLLOW_lexerRuleBlock_in_lexerRule1915 = new BitSet(new long[] { 0x0080000000000000L });
    public static readonly BitSet FOLLOW_SEMI_in_lexerRule1917 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_lexerAltList_in_lexerRuleBlock1984 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_lexerAlt_in_lexerAltList2020 = new BitSet(new long[] { 0x0000080000000002L });
    public static readonly BitSet FOLLOW_OR_in_lexerAltList2023 = new BitSet(new long[] { 0x4944008500100010L });
    public static readonly BitSet FOLLOW_lexerAlt_in_lexerAltList2025 = new BitSet(new long[] { 0x0000080000000002L });
    public static readonly BitSet FOLLOW_lexerElements_in_lexerAlt2043 = new BitSet(new long[] { 0x0004000000000002L });
    public static readonly BitSet FOLLOW_lexerCommands_in_lexerAlt2049 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_lexerElement_in_lexerElements2092 = new BitSet(new long[] { 0x4940008500100012L });
    public static readonly BitSet FOLLOW_lexerAtom_in_lexerElement2148 = new BitSet(new long[] { 0x0401200000000002L });
    public static readonly BitSet FOLLOW_ebnfSuffix_in_lexerElement2154 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_lexerBlock_in_lexerElement2200 = new BitSet(new long[] { 0x0401200000000002L });
    public static readonly BitSet FOLLOW_ebnfSuffix_in_lexerElement2206 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_actionElement_in_lexerElement2234 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_LPAREN_in_lexerBlock2270 = new BitSet(new long[] { 0x4944048500100010L });
    public static readonly BitSet FOLLOW_optionsSpec_in_lexerBlock2282 = new BitSet(new long[] { 0x0000000000004000L });
    public static readonly BitSet FOLLOW_COLON_in_lexerBlock2284 = new BitSet(new long[] { 0x4944008500100010L });
    public static readonly BitSet FOLLOW_lexerAltList_in_lexerBlock2297 = new BitSet(new long[] { 0x0020000000000000L });
    public static readonly BitSet FOLLOW_RPAREN_in_lexerBlock2307 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_RARROW_in_lexerCommands2344 = new BitSet(new long[] { 0x4040001000000000L });
    public static readonly BitSet FOLLOW_lexerCommand_in_lexerCommands2346 = new BitSet(new long[] { 0x0000000000010002L });
    public static readonly BitSet FOLLOW_COMMA_in_lexerCommands2349 = new BitSet(new long[] { 0x4040001000000000L });
    public static readonly BitSet FOLLOW_lexerCommand_in_lexerCommands2351 = new BitSet(new long[] { 0x0000000000010002L });
    public static readonly BitSet FOLLOW_lexerCommandName_in_lexerCommand2369 = new BitSet(new long[] { 0x0000000400000000L });
    public static readonly BitSet FOLLOW_LPAREN_in_lexerCommand2371 = new BitSet(new long[] { 0x4040000040000000L });
    public static readonly BitSet FOLLOW_lexerCommandExpr_in_lexerCommand2373 = new BitSet(new long[] { 0x0020000000000000L });
    public static readonly BitSet FOLLOW_RPAREN_in_lexerCommand2375 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_lexerCommandName_in_lexerCommand2390 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_id_in_lexerCommandExpr2401 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_INT_in_lexerCommandExpr2406 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_id_in_lexerCommandName2430 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_MODE_in_lexerCommandName2448 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_alternative_in_altList2476 = new BitSet(new long[] { 0x0000080000000002L });
    public static readonly BitSet FOLLOW_OR_in_altList2479 = new BitSet(new long[] { 0x4940088C00100010L });
    public static readonly BitSet FOLLOW_alternative_in_altList2481 = new BitSet(new long[] { 0x0000080000000002L });
    public static readonly BitSet FOLLOW_elementOptions_in_alternative2515 = new BitSet(new long[] { 0x4940008400100012L });
    public static readonly BitSet FOLLOW_element_in_alternative2524 = new BitSet(new long[] { 0x4940008400100012L });
    public static readonly BitSet FOLLOW_labeledElement_in_element2639 = new BitSet(new long[] { 0x0401200000000002L });
    public static readonly BitSet FOLLOW_ebnfSuffix_in_element2645 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_atom_in_element2691 = new BitSet(new long[] { 0x0401200000000002L });
    public static readonly BitSet FOLLOW_ebnfSuffix_in_element2697 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_ebnf_in_element2743 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_actionElement_in_element2748 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_ACTION_in_actionElement2774 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_ACTION_in_actionElement2784 = new BitSet(new long[] { 0x0000000800000000L });
    public static readonly BitSet FOLLOW_elementOptions_in_actionElement2786 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_SEMPRED_in_actionElement2804 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_SEMPRED_in_actionElement2814 = new BitSet(new long[] { 0x0000000800000000L });
    public static readonly BitSet FOLLOW_elementOptions_in_actionElement2816 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_id_in_labeledElement2838 = new BitSet(new long[] { 0x0000400000000400L });
    public static readonly BitSet FOLLOW_ASSIGN_in_labeledElement2843 = new BitSet(new long[] { 0x4840008400100000L });
    public static readonly BitSet FOLLOW_PLUS_ASSIGN_in_labeledElement2847 = new BitSet(new long[] { 0x4840008400100000L });
    public static readonly BitSet FOLLOW_atom_in_labeledElement2854 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_block_in_labeledElement2876 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_block_in_ebnf2912 = new BitSet(new long[] { 0x0401200000000002L });
    public static readonly BitSet FOLLOW_blockSuffix_in_ebnf2936 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_ebnfSuffix_in_blockSuffix2986 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_QUESTION_in_ebnfSuffix3001 = new BitSet(new long[] { 0x0001000000000002L });
    public static readonly BitSet FOLLOW_QUESTION_in_ebnfSuffix3005 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_STAR_in_ebnfSuffix3021 = new BitSet(new long[] { 0x0001000000000002L });
    public static readonly BitSet FOLLOW_QUESTION_in_ebnfSuffix3025 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_PLUS_in_ebnfSuffix3043 = new BitSet(new long[] { 0x0001000000000002L });
    public static readonly BitSet FOLLOW_QUESTION_in_ebnfSuffix3047 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_range_in_lexerAtom3068 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_terminal_in_lexerAtom3073 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_RULE_REF_in_lexerAtom3083 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_notSet_in_lexerAtom3094 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_wildcard_in_lexerAtom3102 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_LEXER_CHAR_SET_in_lexerAtom3110 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_range_in_atom3155 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_terminal_in_atom3162 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_ruleref_in_atom3172 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_notSet_in_atom3180 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_wildcard_in_atom3188 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_DOT_in_wildcard3236 = new BitSet(new long[] { 0x0000000800000002L });
    public static readonly BitSet FOLLOW_elementOptions_in_wildcard3238 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_NOT_in_notSet3276 = new BitSet(new long[] { 0x4800000100000000L });
    public static readonly BitSet FOLLOW_setElement_in_notSet3278 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_NOT_in_notSet3306 = new BitSet(new long[] { 0x0000000400000000L });
    public static readonly BitSet FOLLOW_blockSet_in_notSet3308 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_LPAREN_in_blockSet3343 = new BitSet(new long[] { 0x4800000100000000L });
    public static readonly BitSet FOLLOW_setElement_in_blockSet3345 = new BitSet(new long[] { 0x0020080000000000L });
    public static readonly BitSet FOLLOW_OR_in_blockSet3348 = new BitSet(new long[] { 0x4800000100000000L });
    public static readonly BitSet FOLLOW_setElement_in_blockSet3350 = new BitSet(new long[] { 0x0020080000000000L });
    public static readonly BitSet FOLLOW_RPAREN_in_blockSet3354 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_TOKEN_REF_in_setElement3384 = new BitSet(new long[] { 0x0000000800000002L });
    public static readonly BitSet FOLLOW_elementOptions_in_setElement3390 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_STRING_LITERAL_in_setElement3396 = new BitSet(new long[] { 0x0000000800000002L });
    public static readonly BitSet FOLLOW_elementOptions_in_setElement3402 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_range_in_setElement3408 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_LEXER_CHAR_SET_in_setElement3418 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_LPAREN_in_block3442 = new BitSet(new long[] { 0x49400C8C00104810L });
    public static readonly BitSet FOLLOW_optionsSpec_in_block3454 = new BitSet(new long[] { 0x0000000000004800L });
    public static readonly BitSet FOLLOW_ruleAction_in_block3459 = new BitSet(new long[] { 0x0000000000004800L });
    public static readonly BitSet FOLLOW_COLON_in_block3462 = new BitSet(new long[] { 0x4940088C00100010L });
    public static readonly BitSet FOLLOW_altList_in_block3475 = new BitSet(new long[] { 0x0020000000000000L });
    public static readonly BitSet FOLLOW_RPAREN_in_block3479 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_RULE_REF_in_ruleref3533 = new BitSet(new long[] { 0x0000000800000102L });
    public static readonly BitSet FOLLOW_ARG_ACTION_in_ruleref3535 = new BitSet(new long[] { 0x0000000800000002L });
    public static readonly BitSet FOLLOW_elementOptions_in_ruleref3538 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_STRING_LITERAL_in_range3594 = new BitSet(new long[] { 0x0002000000000000L });
    public static readonly BitSet FOLLOW_RANGE_in_range3599 = new BitSet(new long[] { 0x0800000000000000L });
    public static readonly BitSet FOLLOW_STRING_LITERAL_in_range3605 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_TOKEN_REF_in_terminal3629 = new BitSet(new long[] { 0x0000000800000002L });
    public static readonly BitSet FOLLOW_elementOptions_in_terminal3631 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_STRING_LITERAL_in_terminal3652 = new BitSet(new long[] { 0x0000000800000002L });
    public static readonly BitSet FOLLOW_elementOptions_in_terminal3654 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_LT_in_elementOptions3685 = new BitSet(new long[] { 0x4040000004000000L });
    public static readonly BitSet FOLLOW_elementOption_in_elementOptions3688 = new BitSet(new long[] { 0x0000000004010000L });
    public static readonly BitSet FOLLOW_COMMA_in_elementOptions3691 = new BitSet(new long[] { 0x4040000000000000L });
    public static readonly BitSet FOLLOW_elementOption_in_elementOptions3693 = new BitSet(new long[] { 0x0000000004010000L });
    public static readonly BitSet FOLLOW_GT_in_elementOptions3699 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_qid_in_elementOption3747 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_id_in_elementOption3755 = new BitSet(new long[] { 0x0000000000000400L });
    public static readonly BitSet FOLLOW_ASSIGN_in_elementOption3757 = new BitSet(new long[] { 0x4840000040000010L });
    public static readonly BitSet FOLLOW_optionValue_in_elementOption3760 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_RULE_REF_in_id3791 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_TOKEN_REF_in_id3804 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_id_in_qid3832 = new BitSet(new long[] { 0x0000000000100002L });
    public static readonly BitSet FOLLOW_DOT_in_qid3835 = new BitSet(new long[] { 0x4040000000000000L });
    public static readonly BitSet FOLLOW_id_in_qid3837 = new BitSet(new long[] { 0x0000000000100002L });
    public static readonly BitSet FOLLOW_alternative_in_alternativeEntry3854 = new BitSet(new long[] { 0x0000000000000000L });
    public static readonly BitSet FOLLOW_EOF_in_alternativeEntry3856 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_element_in_elementEntry3865 = new BitSet(new long[] { 0x0000000000000000L });
    public static readonly BitSet FOLLOW_EOF_in_elementEntry3867 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_rule_in_ruleEntry3875 = new BitSet(new long[] { 0x0000000000000000L });
    public static readonly BitSet FOLLOW_EOF_in_ruleEntry3877 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_block_in_blockEntry3885 = new BitSet(new long[] { 0x0000000000000000L });
    public static readonly BitSet FOLLOW_EOF_in_blockEntry3887 = new BitSet(new long[] { 0x0000000000000002L });
}
