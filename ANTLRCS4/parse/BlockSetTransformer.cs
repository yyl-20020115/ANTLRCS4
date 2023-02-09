// $ANTLR 3.5.3 org\\antlr\\v4\\parse\\BlockSetTransformer.g 2023-01-27 22:27:33

using org.antlr.runtime;
using org.antlr.runtime.tree;
using org.antlr.v4.misc;
using org.antlr.v4.runtime;
using org.antlr.v4.runtime.misc;
using org.antlr.v4.tool;
using org.antlr.v4.tool.ast;

namespace org.antlr.v4.parse;
public class BlockSetTransformer : TreeRewriter
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
    public TreeRewriter[] getDelegates()
    {
        return new TreeRewriter[] { };
    }

    // delegators

    protected DFA10 dfa10;

    public BlockSetTransformer(TreeNodeStream input)
        : this(input, new RecognizerSharedState())
    {
    }
    public BlockSetTransformer(TreeNodeStream input, RecognizerSharedState state)
        : base(input, state)
    {
        dfa10 = new DFA10(this);
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
    //@Override 
    public String[] getTokenNames() { return BlockSetTransformer.tokenNames; }
    //@Override 
    public override String GrammarFileName => "org\\antlr\\v4\\parse\\BlockSetTransformer.g";

    public String currentRuleName;
    public GrammarAST currentAlt;
    public Grammar g;
    public BlockSetTransformer(TreeNodeStream input, Grammar g)
        : this(input, new RecognizerSharedState())
    {
        ;
        this.g = g;
    }


    public class topdown_return : TreeRuleReturnScope
    {
        public GrammarAST tree;
        //@Override

        public GrammarAST getTree() { return tree; }
    };


    // $ANTLR start "topdown"
    // org\\antlr\\v4\\parse\\BlockSetTransformer.g:63:1: topdown : ( ^( RULE (id= TOKEN_REF |id= RULE_REF ) ( . )+ ) | setAlt | ebnfBlockSet | blockSet );
    //@Override
    public BlockSetTransformer.topdown_return topdown()
    {
        BlockSetTransformer.topdown_return retval = new BlockSetTransformer.topdown_return();
        retval.start = input.LT(1);

        GrammarAST root_0 = null;

        GrammarAST _first_0 = null;
        GrammarAST _last = null;


        GrammarAST id = null;
        GrammarAST RULE1 = null;
        GrammarAST wildcard2 = null;
        TreeRuleReturnScope setAlt3 = null;
        TreeRuleReturnScope ebnfBlockSet4 = null;
        TreeRuleReturnScope blockSet5 = null;

        GrammarAST id_tree = null;
        GrammarAST RULE1_tree = null;
        GrammarAST wildcard2_tree = null;

        try
        {
            // org\\antlr\\v4\\parse\\BlockSetTransformer.g:64:5: ( ^( RULE (id= TOKEN_REF |id= RULE_REF ) ( . )+ ) | setAlt | ebnfBlockSet | blockSet )
            int alt3 = 4;
            switch (input.LA(1))
            {
                case RULE:
                    {
                        alt3 = 1;
                    }
                    break;
                case ALT:
                    {
                        alt3 = 2;
                    }
                    break;
                case CLOSURE:
                case OPTIONAL:
                case POSITIVE_CLOSURE:
                    {
                        alt3 = 3;
                    }
                    break;
                case BLOCK:
                    {
                        alt3 = 4;
                    }
                    break;
                default:
                    if (state.backtracking > 0) { state.failed = true; return retval; }
                    NoViableAltException nvae =
                        new NoViableAltException("", 3, 0, input);
                    throw nvae;
            }
            switch (alt3)
            {
                case 1:
                    // org\\antlr\\v4\\parse\\BlockSetTransformer.g:64:7: ^( RULE (id= TOKEN_REF |id= RULE_REF ) ( . )+ )
                    {
                        _last = (GrammarAST)input.LT(1);
                        {
                            GrammarAST _save_last_1 = _last;
                            GrammarAST _first_1 = null;
                            _last = (GrammarAST)input.LT(1);
                            RULE1 = (GrammarAST)Match(input, RULE, FOLLOW_RULE_in_topdown86); if (state.failed) return retval;

                            if (state.backtracking == 1)
                                if (_first_0 == null) _first_0 = RULE1;
                            Match(input, Token.DOWN, null); if (state.failed) return retval;
                            // org\\antlr\\v4\\parse\\BlockSetTransformer.g:64:14: (id= TOKEN_REF |id= RULE_REF )
                            int alt1 = 2;
                            int LA1_0 = input.LA(1);
                            if ((LA1_0 == TOKEN_REF))
                            {
                                alt1 = 1;
                            }
                            else if ((LA1_0 == RULE_REF))
                            {
                                alt1 = 2;
                            }

                            else
                            {
                                if (state.backtracking > 0) { state.failed = true; return retval; }
                                NoViableAltException nvae =
                                    new NoViableAltException("", 1, 0, input);
                                throw nvae;
                            }

                            switch (alt1)
                            {
                                case 1:
                                    // org\\antlr\\v4\\parse\\BlockSetTransformer.g:64:15: id= TOKEN_REF
                                    {
                                        _last = (GrammarAST)input.LT(1);
                                        id = (GrammarAST)Match(input, TOKEN_REF, FOLLOW_TOKEN_REF_in_topdown91); if (state.failed) return retval;

                                        if (state.backtracking == 1)
                                            if (_first_1 == null) _first_1 = id;

                                        if (state.backtracking == 1)
                                        {
                                            retval.tree = _first_0;
                                            if (adaptor.getParent(retval.tree) != null && adaptor.isNil(adaptor.getParent(retval.tree)))
                                                retval.tree = (GrammarAST)adaptor.getParent(retval.tree);
                                        }

                                    }
                                    break;
                                case 2:
                                    // org\\antlr\\v4\\parse\\BlockSetTransformer.g:64:28: id= RULE_REF
                                    {
                                        _last = (GrammarAST)input.LT(1);
                                        id = (GrammarAST)Match(input, RULE_REF, FOLLOW_RULE_REF_in_topdown95); if (state.failed) return retval;

                                        if (state.backtracking == 1)
                                            if (_first_1 == null) _first_1 = id;

                                        if (state.backtracking == 1)
                                        {
                                            retval.tree = _first_0;
                                            if (adaptor.getParent(retval.tree) != null && adaptor.isNil(adaptor.getParent(retval.tree)))
                                                retval.tree = (GrammarAST)adaptor.getParent(retval.tree);
                                        }

                                    }
                                    break;

                            }

                            if (state.backtracking == 1) { currentRuleName = (id != null ? id.getText() : null); }
                            // org\\antlr\\v4\\parse\\BlockSetTransformer.g:64:69: ( . )+
                            int cnt2 = 0;
                        loop2:
                            while (true)
                            {
                                int alt2 = 2;
                                int LA2_0 = input.LA(1);
                                if (((LA2_0 >= ACTION && LA2_0 <= WILDCARD)))
                                {
                                    alt2 = 1;
                                }
                                else if ((LA2_0 == UP))
                                {
                                    alt2 = 2;
                                }

                                switch (alt2)
                                {
                                    case 1:
                                        // org\\antlr\\v4\\parse\\BlockSetTransformer.g:64:69: .
                                        {
                                            _last = (GrammarAST)input.LT(1);
                                            wildcard2 = (GrammarAST)input.LT(1);
                                            MatchAny(input); if (state.failed) return retval;

                                            if (state.backtracking == 1)
                                                if (_first_1 == null) _first_1 = wildcard2;

                                            if (state.backtracking == 1)
                                            {
                                                retval.tree = _first_0;
                                                if (adaptor.getParent(retval.tree) != null && adaptor.isNil(adaptor.getParent(retval.tree)))
                                                    retval.tree = (GrammarAST)adaptor.getParent(retval.tree);
                                            }

                                        }
                                        break;

                                    default:
                                        if (cnt2 >= 1) goto exit2;// break loop2;
                                        if (state.backtracking > 0) { state.failed = true; return retval; }
                                        EarlyExitException eee = new EarlyExitException(2, input);
                                        throw eee;
                                }
                                cnt2++;
                            }
                        exit2:
                            Match(input, Token.UP, null); if (state.failed) return retval;
                            _last = _save_last_1;
                        }


                        if (state.backtracking == 1)
                        {
                            retval.tree = _first_0;
                            if (adaptor.getParent(retval.tree) != null && adaptor.isNil(adaptor.getParent(retval.tree)))
                                retval.tree = (GrammarAST)adaptor.getParent(retval.tree);
                        }

                    }
                    break;
                case 2:
                    // org\\antlr\\v4\\parse\\BlockSetTransformer.g:65:7: setAlt
                    {
                        _last = (GrammarAST)input.LT(1);
                        PushFollow(FOLLOW_setAlt_in_topdown110);
                        setAlt3 = setAlt();
                        state._fsp--;
                        if (state.failed) return retval;
                        if (state.backtracking == 1)

                            if (_first_0 == null) _first_0 = (GrammarAST)setAlt3.Tree;

                        if (state.backtracking == 1)
                        {
                            retval.tree = _first_0;
                            if (adaptor.getParent(retval.tree) != null && adaptor.isNil(adaptor.getParent(retval.tree)))
                                retval.tree = (GrammarAST)adaptor.getParent(retval.tree);
                        }

                    }
                    break;
                case 3:
                    // org\\antlr\\v4\\parse\\BlockSetTransformer.g:66:7: ebnfBlockSet
                    {
                        _last = (GrammarAST)input.LT(1);
                        PushFollow(FOLLOW_ebnfBlockSet_in_topdown118);
                        ebnfBlockSet4 = ebnfBlockSet();
                        state._fsp--;
                        if (state.failed) return retval;
                        if (state.backtracking == 1)

                            if (_first_0 == null) _first_0 = (GrammarAST)ebnfBlockSet4.Tree;

                        if (state.backtracking == 1)
                        {
                            retval.tree = _first_0;
                            if (adaptor.getParent(retval.tree) != null && adaptor.isNil(adaptor.getParent(retval.tree)))
                                retval.tree = (GrammarAST)adaptor.getParent(retval.tree);
                        }

                    }
                    break;
                case 4:
                    // org\\antlr\\v4\\parse\\BlockSetTransformer.g:67:7: blockSet
                    {
                        _last = (GrammarAST)input.LT(1);
                        PushFollow(FOLLOW_blockSet_in_topdown126);
                        blockSet5 = blockSet();
                        state._fsp--;
                        if (state.failed) return retval;
                        if (state.backtracking == 1)

                            if (_first_0 == null) _first_0 = (GrammarAST)blockSet5.Tree;

                        if (state.backtracking == 1)
                        {
                            retval.tree = _first_0;
                            if (adaptor.getParent(retval.tree) != null && adaptor.isNil(adaptor.getParent(retval.tree)))
                                retval.tree = (GrammarAST)adaptor.getParent(retval.tree);
                        }

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
    // $ANTLR end "topdown"


    public class setAlt_return : TreeRuleReturnScope
    {
        public GrammarAST tree;
        //@Override

        public GrammarAST getTree() { return tree; }
    };


    // $ANTLR start "setAlt"
    // org\\antlr\\v4\\parse\\BlockSetTransformer.g:70:1: setAlt :{...}? ALT ;
    public BlockSetTransformer.setAlt_return setAlt()
    {
        BlockSetTransformer.setAlt_return retval = new BlockSetTransformer.setAlt_return();
        retval.start = input.LT(1);

        GrammarAST root_0 = null;

        GrammarAST _first_0 = null;
        GrammarAST _last = null;


        GrammarAST ALT6 = null;

        GrammarAST ALT6_tree = null;

        try
        {
            // org\\antlr\\v4\\parse\\BlockSetTransformer.g:71:2: ({...}? ALT )
            // org\\antlr\\v4\\parse\\BlockSetTransformer.g:71:4: {...}? ALT
            {
                if (!((InContext("RULE BLOCK"))))
                {
                    if (state.backtracking > 0) { state.failed = true; return retval; }
                    throw new FailedPredicateException(input, "setAlt", "inContext(\"RULE BLOCK\")");
                }
                _last = (GrammarAST)input.LT(1);
                ALT6 = (GrammarAST)Match(input, ALT, FOLLOW_ALT_in_setAlt141); if (state.failed) return retval;

                if (state.backtracking == 1)
                    if (_first_0 == null) _first_0 = ALT6;

                if (state.backtracking == 1) { currentAlt = ((GrammarAST)retval.start); }
                if (state.backtracking == 1)
                {
                    retval.tree = _first_0;
                    if (adaptor.getParent(retval.tree) != null && adaptor.isNil(adaptor.getParent(retval.tree)))
                        retval.tree = (GrammarAST)adaptor.getParent(retval.tree);
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
    // $ANTLR end "setAlt"


    public class ebnfBlockSet_return : TreeRuleReturnScope
    {
        public GrammarAST tree;
        //@Override

        public GrammarAST getTree() { return tree; }
    };


    // $ANTLR start "ebnfBlockSet"
    // org\\antlr\\v4\\parse\\BlockSetTransformer.g:76:1: ebnfBlockSet : ^( ebnfSuffix blockSet ) -> ^( ebnfSuffix ^( BLOCK ^( ALT blockSet ) ) ) ;
    public BlockSetTransformer.ebnfBlockSet_return ebnfBlockSet()
    {
        BlockSetTransformer.ebnfBlockSet_return retval = new BlockSetTransformer.ebnfBlockSet_return();
        retval.start = input.LT(1);

        GrammarAST root_0 = null;

        GrammarAST _first_0 = null;
        GrammarAST _last = null;


        TreeRuleReturnScope ebnfSuffix7 = null;
        TreeRuleReturnScope blockSet8 = null;

        RewriteRuleSubtreeStream stream_blockSet = new RewriteRuleSubtreeStream(adaptor, "rule blockSet");
        RewriteRuleSubtreeStream stream_ebnfSuffix = new RewriteRuleSubtreeStream(adaptor, "rule ebnfSuffix");

        try
        {
            // org\\antlr\\v4\\parse\\BlockSetTransformer.g:80:2: ( ^( ebnfSuffix blockSet ) -> ^( ebnfSuffix ^( BLOCK ^( ALT blockSet ) ) ) )
            // org\\antlr\\v4\\parse\\BlockSetTransformer.g:80:4: ^( ebnfSuffix blockSet )
            {
                _last = (GrammarAST)input.LT(1);
                {
                    GrammarAST _save_last_1 = _last;
                    GrammarAST _first_1 = null;
                    _last = (GrammarAST)input.LT(1);
                    PushFollow(FOLLOW_ebnfSuffix_in_ebnfBlockSet161);
                    ebnfSuffix7 = ebnfSuffix();
                    state._fsp--;
                    if (state.failed) return retval;
                    if (state.backtracking == 1) stream_ebnfSuffix.add(ebnfSuffix7.Tree);
                    if (state.backtracking == 1)
                        if (_first_0 == null) _first_0 = (GrammarAST)ebnfSuffix7.Tree;
                    Match(input, Token.DOWN, null); if (state.failed) return retval;
                    _last = (GrammarAST)input.LT(1);
                    PushFollow(FOLLOW_blockSet_in_ebnfBlockSet163);
                    blockSet8 = blockSet();
                    state._fsp--;
                    if (state.failed) return retval;
                    if (state.backtracking == 1) stream_blockSet.add(blockSet8.Tree);
                    Match(input, Token.UP, null); if (state.failed) return retval;
                    _last = _save_last_1;
                }



                // AST REWRITE
                // elements: blockSet, ebnfSuffix
                // token labels: 
                // rule labels: retval
                // token list labels: 
                // rule list labels: 
                // wildcard labels: 
                if (state.backtracking == 1)
                {
                    retval.tree = root_0;
                    RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval != null ? retval.getTree() : null);

                    root_0 = (GrammarAST)adaptor.nil();
                    // 80:27: -> ^( ebnfSuffix ^( BLOCK ^( ALT blockSet ) ) )
                    {
                        // org\\antlr\\v4\\parse\\BlockSetTransformer.g:80:30: ^( ebnfSuffix ^( BLOCK ^( ALT blockSet ) ) )
                        {
                            GrammarAST root_1 = (GrammarAST)adaptor.nil();
                            root_1 = (GrammarAST)adaptor.becomeRoot(stream_ebnfSuffix.nextNode(), root_1);
                            // org\\antlr\\v4\\parse\\BlockSetTransformer.g:80:43: ^( BLOCK ^( ALT blockSet ) )
                            {
                                GrammarAST root_2 = (GrammarAST)adaptor.nil();
                                root_2 = (GrammarAST)adaptor.becomeRoot(new BlockAST(BLOCK), root_2);
                                // org\\antlr\\v4\\parse\\BlockSetTransformer.g:80:61: ^( ALT blockSet )
                                {
                                    GrammarAST root_3 = (GrammarAST)adaptor.nil();
                                    root_3 = (GrammarAST)adaptor.becomeRoot(new AltAST(ALT), root_3);
                                    adaptor.addChild(root_3, stream_blockSet.nextTree());
                                    adaptor.addChild(root_2, root_3);
                                }

                                adaptor.addChild(root_1, root_2);
                            }

                            adaptor.addChild(root_0, root_1);
                        }

                    }


                    retval.tree = (GrammarAST)adaptor.rulePostProcessing(root_0);
                    input.replaceChildren(adaptor.getParent(retval.start),
                                          adaptor.getChildIndex(retval.start),
                                          adaptor.getChildIndex(_last),
                                          retval.tree);
                }

            }

            if (state.backtracking == 1)
            {
                GrammarTransformPipeline.setGrammarPtr(g, retval.tree);
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
    // $ANTLR end "ebnfBlockSet"


    public class ebnfSuffix_return : TreeRuleReturnScope
    {
        public GrammarAST tree;
        //@Override

        public GrammarAST getTree() { return tree; }
    };


    // $ANTLR start "ebnfSuffix"
    // org\\antlr\\v4\\parse\\BlockSetTransformer.g:83:1: ebnfSuffix : ( OPTIONAL | CLOSURE | POSITIVE_CLOSURE );
    public BlockSetTransformer.ebnfSuffix_return ebnfSuffix()
    {
        BlockSetTransformer.ebnfSuffix_return retval = new BlockSetTransformer.ebnfSuffix_return();
        retval.start = input.LT(1);

        GrammarAST root_0 = null;

        GrammarAST _first_0 = null;
        GrammarAST _last = null;


        GrammarAST set9 = null;

        GrammarAST set9_tree = null;

        try
        {
            // org\\antlr\\v4\\parse\\BlockSetTransformer.g:85:2: ( OPTIONAL | CLOSURE | POSITIVE_CLOSURE )
            // org\\antlr\\v4\\parse\\BlockSetTransformer.g:
            {
                _last = (GrammarAST)input.LT(1);
                set9 = (GrammarAST)input.LT(1);
                if (input.LA(1) == CLOSURE || (input.LA(1) >= OPTIONAL && input.LA(1) <= POSITIVE_CLOSURE))
                {
                    input.Consume();
                    state.errorRecovery = false;
                    state.failed = false;
                }
                else
                {
                    if (state.backtracking > 0) { state.failed = true; return retval; }
                    MismatchedSetException mse = new MismatchedSetException(null, input);
                    throw mse;
                }

                if (state.backtracking == 1)
                {
                    retval.tree = _first_0;
                    if (adaptor.getParent(retval.tree) != null && adaptor.isNil(adaptor.getParent(retval.tree)))
                        retval.tree = (GrammarAST)adaptor.getParent(retval.tree);
                }


            }

            if (state.backtracking == 1) { retval.tree = (GrammarAST)adaptor.dupNode(((GrammarAST)retval.start)); }
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
    // $ANTLR end "ebnfSuffix"


    public class blockSet_return : TreeRuleReturnScope
    {
        public GrammarAST tree;
        //@Override

        public GrammarAST getTree() { return tree; }
    };


    // $ANTLR start "blockSet"
    // org\\antlr\\v4\\parse\\BlockSetTransformer.g:90:1: blockSet : ({...}? ^( BLOCK ^(alt= ALT ( elementOptions )? {...}? setElement[inLexer] ) ( ^( ALT ( elementOptions )? setElement[inLexer] ) )+ ) -> ^( BLOCK[$BLOCK.token] ^( ALT[$BLOCK.token,\"ALT\"] ^( SET[$BLOCK.token, \"SET\"] ( setElement )+ ) ) ) |{...}? ^( BLOCK ^( ALT ( elementOptions )? setElement[inLexer] ) ( ^( ALT ( elementOptions )? setElement[inLexer] ) )+ ) -> ^( SET[$BLOCK.token, \"SET\"] ( setElement )+ ) );
    public BlockSetTransformer.blockSet_return blockSet()
    {
        BlockSetTransformer.blockSet_return retval = new BlockSetTransformer.blockSet_return();
        retval.start = input.LT(1);

        GrammarAST root_0 = null;

        GrammarAST _first_0 = null;
        GrammarAST _last = null;


        GrammarAST alt = null;
        GrammarAST BLOCK10 = null;
        GrammarAST ALT13 = null;
        GrammarAST BLOCK16 = null;
        GrammarAST ALT17 = null;
        GrammarAST ALT20 = null;
        TreeRuleReturnScope elementOptions11 = null;
        TreeRuleReturnScope setElement12 = null;
        TreeRuleReturnScope elementOptions14 = null;
        TreeRuleReturnScope setElement15 = null;
        TreeRuleReturnScope elementOptions18 = null;
        TreeRuleReturnScope setElement19 = null;
        TreeRuleReturnScope elementOptions21 = null;
        TreeRuleReturnScope setElement22 = null;

        GrammarAST alt_tree = null;
        GrammarAST BLOCK10_tree = null;
        GrammarAST ALT13_tree = null;
        GrammarAST BLOCK16_tree = null;
        GrammarAST ALT17_tree = null;
        GrammarAST ALT20_tree = null;
        RewriteRuleNodeStream stream_BLOCK = new RewriteRuleNodeStream(adaptor, "token BLOCK");
        RewriteRuleNodeStream stream_ALT = new RewriteRuleNodeStream(adaptor, "token ALT");
        RewriteRuleSubtreeStream stream_elementOptions = new RewriteRuleSubtreeStream(adaptor, "rule elementOptions");
        RewriteRuleSubtreeStream stream_setElement = new RewriteRuleSubtreeStream(adaptor, "rule setElement");


        bool inLexer = Grammar.isTokenName(currentRuleName);

        try
        {
            // org\\antlr\\v4\\parse\\BlockSetTransformer.g:97:2: ({...}? ^( BLOCK ^(alt= ALT ( elementOptions )? {...}? setElement[inLexer] ) ( ^( ALT ( elementOptions )? setElement[inLexer] ) )+ ) -> ^( BLOCK[$BLOCK.token] ^( ALT[$BLOCK.token,\"ALT\"] ^( SET[$BLOCK.token, \"SET\"] ( setElement )+ ) ) ) |{...}? ^( BLOCK ^( ALT ( elementOptions )? setElement[inLexer] ) ( ^( ALT ( elementOptions )? setElement[inLexer] ) )+ ) -> ^( SET[$BLOCK.token, \"SET\"] ( setElement )+ ) )
            int alt10 = 2;
            alt10 = dfa10.Predict(input);
            switch (alt10)
            {
                case 1:
                    // org\\antlr\\v4\\parse\\BlockSetTransformer.g:97:4: {...}? ^( BLOCK ^(alt= ALT ( elementOptions )? {...}? setElement[inLexer] ) ( ^( ALT ( elementOptions )? setElement[inLexer] ) )+ )
                    {
                        if (!((InContext("RULE"))))
                        {
                            if (state.backtracking > 0) { state.failed = true; return retval; }
                            throw new FailedPredicateException(input, "blockSet", "inContext(\"RULE\")");
                        }
                        _last = (GrammarAST)input.LT(1);
                        {
                            GrammarAST _save_last_1 = _last;
                            GrammarAST _first_1 = null;
                            _last = (GrammarAST)input.LT(1);
                            BLOCK10 = (GrammarAST)Match(input, BLOCK, FOLLOW_BLOCK_in_blockSet244); if (state.failed) return retval;

                            if (state.backtracking == 1) stream_BLOCK.add(BLOCK10);

                            if (state.backtracking == 1)
                                if (_first_0 == null) _first_0 = BLOCK10;
                            Match(input, Token.DOWN, null); if (state.failed) return retval;
                            _last = (GrammarAST)input.LT(1);
                            {
                                GrammarAST _save_last_2 = _last;
                                GrammarAST _first_2 = null;
                                _last = (GrammarAST)input.LT(1);
                                alt = (GrammarAST)Match(input, ALT, FOLLOW_ALT_in_blockSet249); if (state.failed) return retval;

                                if (state.backtracking == 1) stream_ALT.add(alt);

                                if (state.backtracking == 1)
                                    if (_first_1 == null) _first_1 = alt;
                                Match(input, Token.DOWN, null); if (state.failed) return retval;
                                // org\\antlr\\v4\\parse\\BlockSetTransformer.g:98:21: ( elementOptions )?
                                int alt4 = 2;
                                int LA4_0 = input.LA(1);
                                if ((LA4_0 == ELEMENT_OPTIONS))
                                {
                                    alt4 = 1;
                                }
                                switch (alt4)
                                {
                                    case 1:
                                        // org\\antlr\\v4\\parse\\BlockSetTransformer.g:98:21: elementOptions
                                        {
                                            _last = (GrammarAST)input.LT(1);
                                            PushFollow(FOLLOW_elementOptions_in_blockSet251);
                                            elementOptions11 = elementOptions();
                                            state._fsp--;
                                            if (state.failed) return retval;
                                            if (state.backtracking == 1) stream_elementOptions.add(elementOptions11.Tree);
                                            if (state.backtracking == 1)
                                            {
                                                retval.tree = _first_0;
                                                if (adaptor.getParent(retval.tree) != null && adaptor.isNil(adaptor.getParent(retval.tree)))
                                                    retval.tree = (GrammarAST)adaptor.getParent(retval.tree);
                                            }

                                        }
                                        break;

                                }

                                if (!((((AltAST)alt).altLabel == null)))
                                {
                                    if (state.backtracking > 0) { state.failed = true; return retval; }
                                    throw new FailedPredicateException(input, "blockSet", "((AltAST)$alt).altLabel==null");
                                }
                                _last = (GrammarAST)input.LT(1);
                                PushFollow(FOLLOW_setElement_in_blockSet256);
                                setElement12 = setElement(inLexer);
                                state._fsp--;
                                if (state.failed) return retval;
                                if (state.backtracking == 1) stream_setElement.add(setElement12.Tree);
                                Match(input, Token.UP, null); if (state.failed) return retval;
                                _last = _save_last_2;
                            }


                            // org\\antlr\\v4\\parse\\BlockSetTransformer.g:98:91: ( ^( ALT ( elementOptions )? setElement[inLexer] ) )+
                            int cnt6 = 0;
                        loop6:
                            while (true)
                            {
                                int alt6 = 2;
                                int LA6_0 = input.LA(1);
                                if ((LA6_0 == ALT))
                                {
                                    alt6 = 1;
                                }

                                switch (alt6)
                                {
                                    case 1:
                                        // org\\antlr\\v4\\parse\\BlockSetTransformer.g:98:93: ^( ALT ( elementOptions )? setElement[inLexer] )
                                        {
                                            _last = (GrammarAST)input.LT(1);
                                            {
                                                GrammarAST _save_last_2 = _last;
                                                GrammarAST _first_2 = null;
                                                _last = (GrammarAST)input.LT(1);
                                                ALT13 = (GrammarAST)Match(input, ALT, FOLLOW_ALT_in_blockSet263); if (state.failed) return retval;

                                                if (state.backtracking == 1) stream_ALT.add(ALT13);

                                                if (state.backtracking == 1)
                                                    if (_first_1 == null) _first_1 = ALT13;
                                                Match(input, Token.DOWN, null); if (state.failed) return retval;
                                                // org\\antlr\\v4\\parse\\BlockSetTransformer.g:98:99: ( elementOptions )?
                                                int alt5 = 2;
                                                int LA5_0 = input.LA(1);
                                                if ((LA5_0 == ELEMENT_OPTIONS))
                                                {
                                                    alt5 = 1;
                                                }
                                                switch (alt5)
                                                {
                                                    case 1:
                                                        // org\\antlr\\v4\\parse\\BlockSetTransformer.g:98:99: elementOptions
                                                        {
                                                            _last = (GrammarAST)input.LT(1);
                                                            PushFollow(FOLLOW_elementOptions_in_blockSet265);
                                                            elementOptions14 = elementOptions();
                                                            state._fsp--;
                                                            if (state.failed) return retval;
                                                            if (state.backtracking == 1) stream_elementOptions.add(elementOptions14.Tree);
                                                            if (state.backtracking == 1)
                                                            {
                                                                retval.tree = _first_0;
                                                                if (adaptor.getParent(retval.tree) != null && adaptor.isNil(adaptor.getParent(retval.tree)))
                                                                    retval.tree = (GrammarAST)adaptor.getParent(retval.tree);
                                                            }

                                                        }
                                                        break;

                                                }

                                                _last = (GrammarAST)input.LT(1);
                                                PushFollow(FOLLOW_setElement_in_blockSet268);
                                                setElement15 = setElement(inLexer);
                                                state._fsp--;
                                                if (state.failed) return retval;
                                                if (state.backtracking == 1) stream_setElement.add(setElement15.Tree);
                                                Match(input, Token.UP, null); if (state.failed) return retval;
                                                _last = _save_last_2;
                                            }


                                            if (state.backtracking == 1)
                                            {
                                                retval.tree = _first_0;
                                                if (adaptor.getParent(retval.tree) != null && adaptor.isNil(adaptor.getParent(retval.tree)))
                                                    retval.tree = (GrammarAST)adaptor.getParent(retval.tree);
                                            }

                                        }
                                        break;

                                    default:
                                        if (cnt6 >= 1) goto exit6;// break loop6;
                                        if (state.backtracking > 0) { state.failed = true; return retval; }
                                        EarlyExitException eee = new EarlyExitException(6, input);
                                        throw eee;
                                }
                                cnt6++;
                            }
                        exit6:
                            Match(input, Token.UP, null); if (state.failed) return retval;
                            _last = _save_last_1;
                        }



                        // AST REWRITE
                        // elements: setElement, BLOCK, ALT
                        // token labels: 
                        // rule labels: retval
                        // token list labels: 
                        // rule list labels: 
                        // wildcard labels: 
                        if (state.backtracking == 1)
                        {
                            retval.tree = root_0;
                            RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval != null ? retval.getTree() : null);

                            root_0 = (GrammarAST)adaptor.nil();
                            // 99:3: -> ^( BLOCK[$BLOCK.token] ^( ALT[$BLOCK.token,\"ALT\"] ^( SET[$BLOCK.token, \"SET\"] ( setElement )+ ) ) )
                            {
                                // org\\antlr\\v4\\parse\\BlockSetTransformer.g:99:6: ^( BLOCK[$BLOCK.token] ^( ALT[$BLOCK.token,\"ALT\"] ^( SET[$BLOCK.token, \"SET\"] ( setElement )+ ) ) )
                                {
                                    GrammarAST root_1 = (GrammarAST)adaptor.nil();
                                    root_1 = (GrammarAST)adaptor.becomeRoot(new BlockAST(BLOCK, BLOCK10.token), root_1);
                                    // org\\antlr\\v4\\parse\\BlockSetTransformer.g:99:38: ^( ALT[$BLOCK.token,\"ALT\"] ^( SET[$BLOCK.token, \"SET\"] ( setElement )+ ) )
                                    {
                                        GrammarAST root_2 = (GrammarAST)adaptor.nil();
                                        root_2 = (GrammarAST)adaptor.becomeRoot(new AltAST(ALT, BLOCK10.token, "ALT"), root_2);
                                        // org\\antlr\\v4\\parse\\BlockSetTransformer.g:99:72: ^( SET[$BLOCK.token, \"SET\"] ( setElement )+ )
                                        {
                                            GrammarAST root_3 = (GrammarAST)adaptor.nil();
                                            root_3 = (GrammarAST)adaptor.becomeRoot((GrammarAST)adaptor.create(SET, BLOCK10.token, "SET"), root_3);
                                            if (!(stream_setElement.hasNext()))
                                            {
                                                throw new RewriteEarlyExitException();
                                            }
                                            while (stream_setElement.hasNext())
                                            {
                                                adaptor.addChild(root_3, stream_setElement.nextTree());
                                            }
                                            stream_setElement.reset();

                                            adaptor.addChild(root_2, root_3);
                                        }

                                        adaptor.addChild(root_1, root_2);
                                    }

                                    adaptor.addChild(root_0, root_1);
                                }

                            }


                            retval.tree = (GrammarAST)adaptor.rulePostProcessing(root_0);
                            input.replaceChildren(adaptor.getParent(retval.start),
                                                  adaptor.getChildIndex(retval.start),
                                                  adaptor.getChildIndex(_last),
                                                  retval.tree);
                        }

                    }
                    break;
                case 2:
                    // org\\antlr\\v4\\parse\\BlockSetTransformer.g:100:4: {...}? ^( BLOCK ^( ALT ( elementOptions )? setElement[inLexer] ) ( ^( ALT ( elementOptions )? setElement[inLexer] ) )+ )
                    {
                        if (!((!InContext("RULE"))))
                        {
                            if (state.backtracking > 0) { state.failed = true; return retval; }
                            throw new FailedPredicateException(input, "blockSet", "!inContext(\"RULE\")");
                        }
                        _last = (GrammarAST)input.LT(1);
                        {
                            GrammarAST _save_last_1 = _last;
                            GrammarAST _first_1 = null;
                            _last = (GrammarAST)input.LT(1);
                            BLOCK16 = (GrammarAST)Match(input, BLOCK, FOLLOW_BLOCK_in_blockSet313); if (state.failed) return retval;

                            if (state.backtracking == 1) stream_BLOCK.add(BLOCK16);

                            if (state.backtracking == 1)
                                if (_first_0 == null) _first_0 = BLOCK16;
                            Match(input, Token.DOWN, null); if (state.failed) return retval;
                            _last = (GrammarAST)input.LT(1);
                            {
                                GrammarAST _save_last_2 = _last;
                                GrammarAST _first_2 = null;
                                _last = (GrammarAST)input.LT(1);
                                ALT17 = (GrammarAST)Match(input, ALT, FOLLOW_ALT_in_blockSet316); if (state.failed) return retval;

                                if (state.backtracking == 1) stream_ALT.add(ALT17);

                                if (state.backtracking == 1)
                                    if (_first_1 == null) _first_1 = ALT17;
                                Match(input, Token.DOWN, null); if (state.failed) return retval;
                                // org\\antlr\\v4\\parse\\BlockSetTransformer.g:101:17: ( elementOptions )?
                                int alt7 = 2;
                                int LA7_0 = input.LA(1);
                                if ((LA7_0 == ELEMENT_OPTIONS))
                                {
                                    alt7 = 1;
                                }
                                switch (alt7)
                                {
                                    case 1:
                                        // org\\antlr\\v4\\parse\\BlockSetTransformer.g:101:17: elementOptions
                                        {
                                            _last = (GrammarAST)input.LT(1);
                                            PushFollow(FOLLOW_elementOptions_in_blockSet318);
                                            elementOptions18 = elementOptions();
                                            state._fsp--;
                                            if (state.failed) return retval;
                                            if (state.backtracking == 1) stream_elementOptions.add(elementOptions18.Tree);
                                            if (state.backtracking == 1)
                                            {
                                                retval.tree = _first_0;
                                                if (adaptor.getParent(retval.tree) != null && adaptor.isNil(adaptor.getParent(retval.tree)))
                                                    retval.tree = (GrammarAST)adaptor.getParent(retval.tree);
                                            }

                                        }
                                        break;

                                }

                                _last = (GrammarAST)input.LT(1);
                                PushFollow(FOLLOW_setElement_in_blockSet321);
                                setElement19 = setElement(inLexer);
                                state._fsp--;
                                if (state.failed) return retval;
                                if (state.backtracking == 1) stream_setElement.add(setElement19.Tree);
                                Match(input, Token.UP, null); if (state.failed) return retval;
                                _last = _save_last_2;
                            }


                            // org\\antlr\\v4\\parse\\BlockSetTransformer.g:101:54: ( ^( ALT ( elementOptions )? setElement[inLexer] ) )+
                            int cnt9 = 0;
                        loop9:
                            while (true)
                            {
                                int alt9 = 2;
                                int LA9_0 = input.LA(1);
                                if ((LA9_0 == ALT))
                                {
                                    alt9 = 1;
                                }

                                switch (alt9)
                                {
                                    case 1:
                                        // org\\antlr\\v4\\parse\\BlockSetTransformer.g:101:56: ^( ALT ( elementOptions )? setElement[inLexer] )
                                        {
                                            _last = (GrammarAST)input.LT(1);
                                            {
                                                GrammarAST _save_last_2 = _last;
                                                GrammarAST _first_2 = null;
                                                _last = (GrammarAST)input.LT(1);
                                                ALT20 = (GrammarAST)Match(input, ALT, FOLLOW_ALT_in_blockSet328); if (state.failed) return retval;

                                                if (state.backtracking == 1) stream_ALT.add(ALT20);

                                                if (state.backtracking == 1)
                                                    if (_first_1 == null) _first_1 = ALT20;
                                                Match(input, Token.DOWN, null); if (state.failed) return retval;
                                                // org\\antlr\\v4\\parse\\BlockSetTransformer.g:101:62: ( elementOptions )?
                                                int alt8 = 2;
                                                int LA8_0 = input.LA(1);
                                                if ((LA8_0 == ELEMENT_OPTIONS))
                                                {
                                                    alt8 = 1;
                                                }
                                                switch (alt8)
                                                {
                                                    case 1:
                                                        // org\\antlr\\v4\\parse\\BlockSetTransformer.g:101:62: elementOptions
                                                        {
                                                            _last = (GrammarAST)input.LT(1);
                                                            PushFollow(FOLLOW_elementOptions_in_blockSet330);
                                                            elementOptions21 = elementOptions();
                                                            state._fsp--;
                                                            if (state.failed) return retval;
                                                            if (state.backtracking == 1) stream_elementOptions.add(elementOptions21.Tree);
                                                            if (state.backtracking == 1)
                                                            {
                                                                retval.tree = _first_0;
                                                                if (adaptor.getParent(retval.tree) != null && adaptor.isNil(adaptor.getParent(retval.tree)))
                                                                    retval.tree = (GrammarAST)adaptor.getParent(retval.tree);
                                                            }

                                                        }
                                                        break;

                                                }

                                                _last = (GrammarAST)input.LT(1);
                                                PushFollow(FOLLOW_setElement_in_blockSet333);
                                                setElement22 = setElement(inLexer);
                                                state._fsp--;
                                                if (state.failed) return retval;
                                                if (state.backtracking == 1) stream_setElement.add(setElement22.Tree);
                                                Match(input, Token.UP, null); if (state.failed) return retval;
                                                _last = _save_last_2;
                                            }


                                            if (state.backtracking == 1)
                                            {
                                                retval.tree = _first_0;
                                                if (adaptor.getParent(retval.tree) != null && adaptor.isNil(adaptor.getParent(retval.tree)))
                                                    retval.tree = (GrammarAST)adaptor.getParent(retval.tree);
                                            }

                                        }
                                        break;

                                    default:
                                        if (cnt9 >= 1) goto exit9;// break loop9;
                                        if (state.backtracking > 0) { state.failed = true; return retval; }
                                        EarlyExitException eee = new EarlyExitException(9, input);
                                        throw eee;
                                }
                                cnt9++;
                            }
                        exit9:
                            Match(input, Token.UP, null); if (state.failed) return retval;
                            _last = _save_last_1;
                        }



                        // AST REWRITE
                        // elements: setElement
                        // token labels: 
                        // rule labels: retval
                        // token list labels: 
                        // rule list labels: 
                        // wildcard labels: 
                        if (state.backtracking == 1)
                        {
                            retval.tree = root_0;
                            RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval != null ? retval.getTree() : null);

                            root_0 = (GrammarAST)adaptor.nil();
                            // 102:3: -> ^( SET[$BLOCK.token, \"SET\"] ( setElement )+ )
                            {
                                // org\\antlr\\v4\\parse\\BlockSetTransformer.g:102:6: ^( SET[$BLOCK.token, \"SET\"] ( setElement )+ )
                                {
                                    GrammarAST root_1 = (GrammarAST)adaptor.nil();
                                    root_1 = (GrammarAST)adaptor.becomeRoot((GrammarAST)adaptor.create(SET, BLOCK16.token, "SET"), root_1);
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


                            retval.tree = (GrammarAST)adaptor.rulePostProcessing(root_0);
                            input.replaceChildren(adaptor.getParent(retval.start),
                                                  adaptor.getChildIndex(retval.start),
                                                  adaptor.getChildIndex(_last),
                                                  retval.tree);
                        }

                    }
                    break;

            }
            if (state.backtracking == 1)
            {
                GrammarTransformPipeline.setGrammarPtr(g, retval.tree);
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
        public GrammarAST tree;
        //@Override

        public GrammarAST getTree() { return tree; }
    };


    // $ANTLR start "setElement"
    // org\\antlr\\v4\\parse\\BlockSetTransformer.g:105:1: setElement[boolean inLexer] : ( ^(a= STRING_LITERAL elementOptions ) {...}?|a= STRING_LITERAL {...}?|{...}? => ^( TOKEN_REF elementOptions ) |{...}? => TOKEN_REF |{...}? => ^( RANGE a= STRING_LITERAL b= STRING_LITERAL ) {...}?) ;
    public BlockSetTransformer.setElement_return setElement(bool inLexer)
    {
        BlockSetTransformer.setElement_return retval = new BlockSetTransformer.setElement_return();
        retval.start = input.LT(1);

        GrammarAST root_0 = null;

        GrammarAST _first_0 = null;
        GrammarAST _last = null;


        GrammarAST a = null;
        GrammarAST b = null;
        GrammarAST TOKEN_REF24 = null;
        GrammarAST TOKEN_REF26 = null;
        GrammarAST RANGE27 = null;
        TreeRuleReturnScope elementOptions23 = null;
        TreeRuleReturnScope elementOptions25 = null;

        GrammarAST a_tree = null;
        GrammarAST b_tree = null;
        GrammarAST TOKEN_REF24_tree = null;
        GrammarAST TOKEN_REF26_tree = null;
        GrammarAST RANGE27_tree = null;

        try
        {
            // org\\antlr\\v4\\parse\\BlockSetTransformer.g:109:2: ( ( ^(a= STRING_LITERAL elementOptions ) {...}?|a= STRING_LITERAL {...}?|{...}? => ^( TOKEN_REF elementOptions ) |{...}? => TOKEN_REF |{...}? => ^( RANGE a= STRING_LITERAL b= STRING_LITERAL ) {...}?) )
            // org\\antlr\\v4\\parse\\BlockSetTransformer.g:109:4: ( ^(a= STRING_LITERAL elementOptions ) {...}?|a= STRING_LITERAL {...}?|{...}? => ^( TOKEN_REF elementOptions ) |{...}? => TOKEN_REF |{...}? => ^( RANGE a= STRING_LITERAL b= STRING_LITERAL ) {...}?)
            {
                // org\\antlr\\v4\\parse\\BlockSetTransformer.g:109:4: ( ^(a= STRING_LITERAL elementOptions ) {...}?|a= STRING_LITERAL {...}?|{...}? => ^( TOKEN_REF elementOptions ) |{...}? => TOKEN_REF |{...}? => ^( RANGE a= STRING_LITERAL b= STRING_LITERAL ) {...}?)
                int alt11 = 5;
                int LA11_0 = input.LA(1);
                if ((LA11_0 == STRING_LITERAL))
                {
                    int LA11_1 = input.LA(2);
                    if ((LA11_1 == DOWN))
                    {
                        alt11 = 1;
                    }
                    else if ((LA11_1 == UP))
                    {
                        alt11 = 2;
                    }

                    else
                    {
                        if (state.backtracking > 0) { state.failed = true; return retval; }
                        int nvaeMark = input.Mark();
                        try
                        {
                            input.Consume();
                            NoViableAltException nvae =
                                new NoViableAltException("", 11, 1, input);
                            throw nvae;
                        }
                        finally
                        {
                            input.Rewind(nvaeMark);
                        }
                    }

                }
                else if ((LA11_0 == TOKEN_REF) && ((!inLexer)))
                {
                    int LA11_2 = input.LA(2);
                    if ((LA11_2 == DOWN) && ((!inLexer)))
                    {
                        alt11 = 3;
                    }
                    else if ((LA11_2 == UP) && ((!inLexer)))
                    {
                        alt11 = 4;
                    }

                }
                else if ((LA11_0 == RANGE) && ((inLexer)))
                {
                    alt11 = 5;
                }

                switch (alt11)
                {
                    case 1:
                        // org\\antlr\\v4\\parse\\BlockSetTransformer.g:109:6: ^(a= STRING_LITERAL elementOptions ) {...}?
                        {
                            _last = (GrammarAST)input.LT(1);
                            {
                                GrammarAST _save_last_1 = _last;
                                GrammarAST _first_1 = null;
                                _last = (GrammarAST)input.LT(1);
                                a = (GrammarAST)Match(input, STRING_LITERAL, FOLLOW_STRING_LITERAL_in_setElement373); if (state.failed) return retval;

                                if (state.backtracking == 1)
                                    if (_first_0 == null) _first_0 = a;
                                Match(input, Token.DOWN, null); if (state.failed) return retval;
                                _last = (GrammarAST)input.LT(1);
                                PushFollow(FOLLOW_elementOptions_in_setElement375);
                                elementOptions23 = elementOptions();
                                state._fsp--;
                                if (state.failed) return retval;
                                if (state.backtracking == 1)

                                    if (_first_1 == null) _first_1 = (GrammarAST)elementOptions23.Tree;

                                Match(input, Token.UP, null); if (state.failed) return retval;
                                _last = _save_last_1;
                            }


                            if (!((!inLexer || CharSupport.GetCharValueFromGrammarCharLiteral(a.getText()) != -1)))
                            {
                                if (state.backtracking > 0) { state.failed = true; return retval; }
                                throw new FailedPredicateException(input, "setElement", "!inLexer || CharSupport.getCharValueFromGrammarCharLiteral($a.getText())!=-1");
                            }
                            if (state.backtracking == 1)
                            {
                                retval.tree = _first_0;
                                if (adaptor.getParent(retval.tree) != null && adaptor.isNil(adaptor.getParent(retval.tree)))
                                    retval.tree = (GrammarAST)adaptor.getParent(retval.tree);
                            }

                        }
                        break;
                    case 2:
                        // org\\antlr\\v4\\parse\\BlockSetTransformer.g:110:7: a= STRING_LITERAL {...}?
                        {
                            _last = (GrammarAST)input.LT(1);
                            a = (GrammarAST)Match(input, STRING_LITERAL, FOLLOW_STRING_LITERAL_in_setElement388); if (state.failed) return retval;

                            if (state.backtracking == 1)
                                if (_first_0 == null) _first_0 = a;

                            if (!((!inLexer || CharSupport.GetCharValueFromGrammarCharLiteral(a.getText()) != -1)))
                            {
                                if (state.backtracking > 0) { state.failed = true; return retval; }
                                throw new FailedPredicateException(input, "setElement", "!inLexer || CharSupport.getCharValueFromGrammarCharLiteral($a.getText())!=-1");
                            }
                            if (state.backtracking == 1)
                            {
                                retval.tree = _first_0;
                                if (adaptor.getParent(retval.tree) != null && adaptor.isNil(adaptor.getParent(retval.tree)))
                                    retval.tree = (GrammarAST)adaptor.getParent(retval.tree);
                            }

                        }
                        break;
                    case 3:
                        // org\\antlr\\v4\\parse\\BlockSetTransformer.g:111:5: {...}? => ^( TOKEN_REF elementOptions )
                        {
                            if (!((!inLexer)))
                            {
                                if (state.backtracking > 0) { state.failed = true; return retval; }
                                throw new FailedPredicateException(input, "setElement", "!inLexer");
                            }
                            _last = (GrammarAST)input.LT(1);
                            {
                                GrammarAST _save_last_1 = _last;
                                GrammarAST _first_1 = null;
                                _last = (GrammarAST)input.LT(1);
                                TOKEN_REF24 = (GrammarAST)Match(input, TOKEN_REF, FOLLOW_TOKEN_REF_in_setElement400); if (state.failed) return retval;

                                if (state.backtracking == 1)
                                    if (_first_0 == null) _first_0 = TOKEN_REF24;
                                Match(input, Token.DOWN, null); if (state.failed) return retval;
                                _last = (GrammarAST)input.LT(1);
                                PushFollow(FOLLOW_elementOptions_in_setElement402);
                                elementOptions25 = elementOptions();
                                state._fsp--;
                                if (state.failed) return retval;
                                if (state.backtracking == 1)

                                    if (_first_1 == null) _first_1 = (GrammarAST)elementOptions25.Tree;

                                Match(input, Token.UP, null); if (state.failed) return retval;
                                _last = _save_last_1;
                            }


                            if (state.backtracking == 1)
                            {
                                retval.tree = _first_0;
                                if (adaptor.getParent(retval.tree) != null && adaptor.isNil(adaptor.getParent(retval.tree)))
                                    retval.tree = (GrammarAST)adaptor.getParent(retval.tree);
                            }

                        }
                        break;
                    case 4:
                        // org\\antlr\\v4\\parse\\BlockSetTransformer.g:112:5: {...}? => TOKEN_REF
                        {
                            if (!((!inLexer)))
                            {
                                if (state.backtracking > 0) { state.failed = true; return retval; }
                                throw new FailedPredicateException(input, "setElement", "!inLexer");
                            }
                            _last = (GrammarAST)input.LT(1);
                            TOKEN_REF26 = (GrammarAST)Match(input, TOKEN_REF, FOLLOW_TOKEN_REF_in_setElement414); if (state.failed) return retval;

                            if (state.backtracking == 1)
                                if (_first_0 == null) _first_0 = TOKEN_REF26;

                            if (state.backtracking == 1)
                            {
                                retval.tree = _first_0;
                                if (adaptor.getParent(retval.tree) != null && adaptor.isNil(adaptor.getParent(retval.tree)))
                                    retval.tree = (GrammarAST)adaptor.getParent(retval.tree);
                            }

                        }
                        break;
                    case 5:
                        // org\\antlr\\v4\\parse\\BlockSetTransformer.g:113:5: {...}? => ^( RANGE a= STRING_LITERAL b= STRING_LITERAL ) {...}?
                        {
                            if (!((inLexer)))
                            {
                                if (state.backtracking > 0) { state.failed = true; return retval; }
                                throw new FailedPredicateException(input, "setElement", "inLexer");
                            }
                            _last = (GrammarAST)input.LT(1);
                            {
                                GrammarAST _save_last_1 = _last;
                                GrammarAST _first_1 = null;
                                _last = (GrammarAST)input.LT(1);
                                RANGE27 = (GrammarAST)Match(input, RANGE, FOLLOW_RANGE_in_setElement425); if (state.failed) return retval;

                                if (state.backtracking == 1)
                                    if (_first_0 == null) _first_0 = RANGE27;
                                Match(input, Token.DOWN, null); if (state.failed) return retval;
                                _last = (GrammarAST)input.LT(1);
                                a = (GrammarAST)Match(input, STRING_LITERAL, FOLLOW_STRING_LITERAL_in_setElement429); if (state.failed) return retval;

                                if (state.backtracking == 1)
                                    if (_first_1 == null) _first_1 = a;

                                _last = (GrammarAST)input.LT(1);
                                b = (GrammarAST)Match(input, STRING_LITERAL, FOLLOW_STRING_LITERAL_in_setElement433); if (state.failed) return retval;

                                if (state.backtracking == 1)
                                    if (_first_1 == null) _first_1 = b;

                                Match(input, Token.UP, null); if (state.failed) return retval;
                                _last = _save_last_1;
                            }


                            if (!((CharSupport.GetCharValueFromGrammarCharLiteral(a.getText()) != -1 &&
                                         CharSupport.GetCharValueFromGrammarCharLiteral(b.getText()) != -1)))
                            {
                                if (state.backtracking > 0) { state.failed = true; return retval; }
                                throw new FailedPredicateException(input, "setElement", "CharSupport.getCharValueFromGrammarCharLiteral($a.getText())!=-1 &&\r\n\t\t\t CharSupport.getCharValueFromGrammarCharLiteral($b.getText())!=-1");
                            }
                            if (state.backtracking == 1)
                            {
                                retval.tree = _first_0;
                                if (adaptor.getParent(retval.tree) != null && adaptor.isNil(adaptor.getParent(retval.tree)))
                                    retval.tree = (GrammarAST)adaptor.getParent(retval.tree);
                            }

                        }
                        break;

                }

                if (state.backtracking == 1)
                {
                    retval.tree = _first_0;
                    if (adaptor.getParent(retval.tree) != null && adaptor.isNil(adaptor.getParent(retval.tree)))
                        retval.tree = (GrammarAST)adaptor.getParent(retval.tree);
                }

            }

            if (state.backtracking == 1)
            {
                GrammarTransformPipeline.setGrammarPtr(g, retval.tree);
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


    public class elementOptions_return : TreeRuleReturnScope
    {
        public GrammarAST tree;
        //@Override

        public GrammarAST getTree() { return tree; }
    };


    // $ANTLR start "elementOptions"
    // org\\antlr\\v4\\parse\\BlockSetTransformer.g:119:1: elementOptions : ^( ELEMENT_OPTIONS ( elementOption )* ) ;
    public BlockSetTransformer.elementOptions_return elementOptions()
    {
        BlockSetTransformer.elementOptions_return retval = new BlockSetTransformer.elementOptions_return();
        retval.start = input.LT(1);

        GrammarAST root_0 = null;

        GrammarAST _first_0 = null;
        GrammarAST _last = null;


        GrammarAST ELEMENT_OPTIONS28 = null;
        TreeRuleReturnScope elementOption29 = null;

        GrammarAST ELEMENT_OPTIONS28_tree = null;

        try
        {
            // org\\antlr\\v4\\parse\\BlockSetTransformer.g:120:2: ( ^( ELEMENT_OPTIONS ( elementOption )* ) )
            // org\\antlr\\v4\\parse\\BlockSetTransformer.g:120:4: ^( ELEMENT_OPTIONS ( elementOption )* )
            {
                _last = (GrammarAST)input.LT(1);
                {
                    GrammarAST _save_last_1 = _last;
                    GrammarAST _first_1 = null;
                    _last = (GrammarAST)input.LT(1);
                    ELEMENT_OPTIONS28 = (GrammarAST)Match(input, ELEMENT_OPTIONS, FOLLOW_ELEMENT_OPTIONS_in_elementOptions455); if (state.failed) return retval;

                    if (state.backtracking == 1)
                        if (_first_0 == null) _first_0 = ELEMENT_OPTIONS28;
                    if (input.LA(1) == Token.DOWN)
                    {
                        Match(input, Token.DOWN, null); if (state.failed) return retval;
                        // org\\antlr\\v4\\parse\\BlockSetTransformer.g:120:22: ( elementOption )*
                        loop12:
                        while (true)
                        {
                            int alt12 = 2;
                            int LA12_0 = input.LA(1);
                            if ((LA12_0 == ASSIGN || LA12_0 == ID))
                            {
                                alt12 = 1;
                            }

                            switch (alt12)
                            {
                                case 1:
                                    // org\\antlr\\v4\\parse\\BlockSetTransformer.g:120:22: elementOption
                                    {
                                        _last = (GrammarAST)input.LT(1);
                                        PushFollow(FOLLOW_elementOption_in_elementOptions457);
                                        elementOption29 = elementOption();
                                        state._fsp--;
                                        if (state.failed) return retval;
                                        if (state.backtracking == 1)

                                            if (_first_1 == null) _first_1 = (GrammarAST)elementOption29.Tree;

                                        if (state.backtracking == 1)
                                        {
                                            retval.tree = _first_0;
                                            if (adaptor.getParent(retval.tree) != null && adaptor.isNil(adaptor.getParent(retval.tree)))
                                                retval.tree = (GrammarAST)adaptor.getParent(retval.tree);
                                        }

                                    }
                                    break;

                                default:
                                    goto exit12;
                                    //break loop12;
                            }
                        }
                        exit12:
                        Match(input, Token.UP, null); if (state.failed) return retval;
                    }
                    _last = _save_last_1;
                }


                if (state.backtracking == 1)
                {
                    retval.tree = _first_0;
                    if (adaptor.getParent(retval.tree) != null && adaptor.isNil(adaptor.getParent(retval.tree)))
                        retval.tree = (GrammarAST)adaptor.getParent(retval.tree);
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
    // $ANTLR end "elementOptions"


    public class elementOption_return : TreeRuleReturnScope
    {
        public GrammarAST tree;
        //@Override

        public GrammarAST getTree() { return tree; }
    };


    // $ANTLR start "elementOption"
    // org\\antlr\\v4\\parse\\BlockSetTransformer.g:123:1: elementOption : ( ID | ^( ASSIGN id= ID v= ID ) | ^( ASSIGN ID v= STRING_LITERAL ) | ^( ASSIGN ID v= ACTION ) | ^( ASSIGN ID v= INT ) );
    public BlockSetTransformer.elementOption_return elementOption()
    {
        BlockSetTransformer.elementOption_return retval = new BlockSetTransformer.elementOption_return();
        retval.start = input.LT(1);

        GrammarAST root_0 = null;

        GrammarAST _first_0 = null;
        GrammarAST _last = null;


        GrammarAST id = null;
        GrammarAST v = null;
        GrammarAST ID30 = null;
        GrammarAST ASSIGN31 = null;
        GrammarAST ASSIGN32 = null;
        GrammarAST ID33 = null;
        GrammarAST ASSIGN34 = null;
        GrammarAST ID35 = null;
        GrammarAST ASSIGN36 = null;
        GrammarAST ID37 = null;

        GrammarAST id_tree = null;
        GrammarAST v_tree = null;
        GrammarAST ID30_tree = null;
        GrammarAST ASSIGN31_tree = null;
        GrammarAST ASSIGN32_tree = null;
        GrammarAST ID33_tree = null;
        GrammarAST ASSIGN34_tree = null;
        GrammarAST ID35_tree = null;
        GrammarAST ASSIGN36_tree = null;
        GrammarAST ID37_tree = null;

        try
        {
            // org\\antlr\\v4\\parse\\BlockSetTransformer.g:124:2: ( ID | ^( ASSIGN id= ID v= ID ) | ^( ASSIGN ID v= STRING_LITERAL ) | ^( ASSIGN ID v= ACTION ) | ^( ASSIGN ID v= INT ) )
            int alt13 = 5;
            int LA13_0 = input.LA(1);
            if ((LA13_0 == ID))
            {
                alt13 = 1;
            }
            else if ((LA13_0 == ASSIGN))
            {
                int LA13_2 = input.LA(2);
                if ((LA13_2 == DOWN))
                {
                    int LA13_3 = input.LA(3);
                    if ((LA13_3 == ID))
                    {
                        switch (input.LA(4))
                        {
                            case ID:
                                {
                                    alt13 = 2;
                                }
                                break;
                            case STRING_LITERAL:
                                {
                                    alt13 = 3;
                                }
                                break;
                            case ACTION:
                                {
                                    alt13 = 4;
                                }
                                break;
                            case INT:
                                {
                                    alt13 = 5;
                                }
                                break;
                            default:
                                if (state.backtracking > 0) { state.failed = true; return retval; }
                                int nvaeMark = input.Mark();
                                try
                                {
                                    for (int nvaeConsume = 0; nvaeConsume < 4 - 1; nvaeConsume++)
                                    {
                                        input.Consume();
                                    }
                                    NoViableAltException nvae =
                                        new NoViableAltException("", 13, 4, input);
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
                        if (state.backtracking > 0) { state.failed = true; return retval; }
                        int nvaeMark = input.Mark();
                        try
                        {
                            for (int nvaeConsume = 0; nvaeConsume < 3 - 1; nvaeConsume++)
                            {
                                input.Consume();
                            }
                            NoViableAltException nvae =
                                new NoViableAltException("", 13, 3, input);
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
                    if (state.backtracking > 0) { state.failed = true; return retval; }
                    int nvaeMark = input.Mark();
                    try
                    {
                        input.Consume();
                        NoViableAltException nvae =
                            new NoViableAltException("", 13, 2, input);
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
                if (state.backtracking > 0) { state.failed = true; return retval; }
                NoViableAltException nvae =
                    new NoViableAltException("", 13, 0, input);
                throw nvae;
            }

            switch (alt13)
            {
                case 1:
                    // org\\antlr\\v4\\parse\\BlockSetTransformer.g:124:4: ID
                    {
                        _last = (GrammarAST)input.LT(1);
                        ID30 = (GrammarAST)Match(input, ID, FOLLOW_ID_in_elementOption470); if (state.failed) return retval;

                        if (state.backtracking == 1)
                            if (_first_0 == null) _first_0 = ID30;

                        if (state.backtracking == 1)
                        {
                            retval.tree = _first_0;
                            if (adaptor.getParent(retval.tree) != null && adaptor.isNil(adaptor.getParent(retval.tree)))
                                retval.tree = (GrammarAST)adaptor.getParent(retval.tree);
                        }

                    }
                    break;
                case 2:
                    // org\\antlr\\v4\\parse\\BlockSetTransformer.g:125:4: ^( ASSIGN id= ID v= ID )
                    {
                        _last = (GrammarAST)input.LT(1);
                        {
                            GrammarAST _save_last_1 = _last;
                            GrammarAST _first_1 = null;
                            _last = (GrammarAST)input.LT(1);
                            ASSIGN31 = (GrammarAST)Match(input, ASSIGN, FOLLOW_ASSIGN_in_elementOption476); if (state.failed) return retval;

                            if (state.backtracking == 1)
                                if (_first_0 == null) _first_0 = ASSIGN31;
                            Match(input, Token.DOWN, null); if (state.failed) return retval;
                            _last = (GrammarAST)input.LT(1);
                            id = (GrammarAST)Match(input, ID, FOLLOW_ID_in_elementOption480); if (state.failed) return retval;

                            if (state.backtracking == 1)
                                if (_first_1 == null) _first_1 = id;

                            _last = (GrammarAST)input.LT(1);
                            v = (GrammarAST)Match(input, ID, FOLLOW_ID_in_elementOption484); if (state.failed) return retval;

                            if (state.backtracking == 1)
                                if (_first_1 == null) _first_1 = v;

                            Match(input, Token.UP, null); if (state.failed) return retval;
                            _last = _save_last_1;
                        }


                        if (state.backtracking == 1)
                        {
                            retval.tree = _first_0;
                            if (adaptor.getParent(retval.tree) != null && adaptor.isNil(adaptor.getParent(retval.tree)))
                                retval.tree = (GrammarAST)adaptor.getParent(retval.tree);
                        }

                    }
                    break;
                case 3:
                    // org\\antlr\\v4\\parse\\BlockSetTransformer.g:126:4: ^( ASSIGN ID v= STRING_LITERAL )
                    {
                        _last = (GrammarAST)input.LT(1);
                        {
                            GrammarAST _save_last_1 = _last;
                            GrammarAST _first_1 = null;
                            _last = (GrammarAST)input.LT(1);
                            ASSIGN32 = (GrammarAST)Match(input, ASSIGN, FOLLOW_ASSIGN_in_elementOption491); if (state.failed) return retval;

                            if (state.backtracking == 1)
                                if (_first_0 == null) _first_0 = ASSIGN32;
                            Match(input, Token.DOWN, null); if (state.failed) return retval;
                            _last = (GrammarAST)input.LT(1);
                            ID33 = (GrammarAST)Match(input, ID, FOLLOW_ID_in_elementOption493); if (state.failed) return retval;

                            if (state.backtracking == 1)
                                if (_first_1 == null) _first_1 = ID33;

                            _last = (GrammarAST)input.LT(1);
                            v = (GrammarAST)Match(input, STRING_LITERAL, FOLLOW_STRING_LITERAL_in_elementOption497); if (state.failed) return retval;

                            if (state.backtracking == 1)
                                if (_first_1 == null) _first_1 = v;

                            Match(input, Token.UP, null); if (state.failed) return retval;
                            _last = _save_last_1;
                        }


                        if (state.backtracking == 1)
                        {
                            retval.tree = _first_0;
                            if (adaptor.getParent(retval.tree) != null && adaptor.isNil(adaptor.getParent(retval.tree)))
                                retval.tree = (GrammarAST)adaptor.getParent(retval.tree);
                        }

                    }
                    break;
                case 4:
                    // org\\antlr\\v4\\parse\\BlockSetTransformer.g:127:4: ^( ASSIGN ID v= ACTION )
                    {
                        _last = (GrammarAST)input.LT(1);
                        {
                            GrammarAST _save_last_1 = _last;
                            GrammarAST _first_1 = null;
                            _last = (GrammarAST)input.LT(1);
                            ASSIGN34 = (GrammarAST)Match(input, ASSIGN, FOLLOW_ASSIGN_in_elementOption504); if (state.failed) return retval;

                            if (state.backtracking == 1)
                                if (_first_0 == null) _first_0 = ASSIGN34;
                            Match(input, Token.DOWN, null); if (state.failed) return retval;
                            _last = (GrammarAST)input.LT(1);
                            ID35 = (GrammarAST)Match(input, ID, FOLLOW_ID_in_elementOption506); if (state.failed) return retval;

                            if (state.backtracking == 1)
                                if (_first_1 == null) _first_1 = ID35;

                            _last = (GrammarAST)input.LT(1);
                            v = (GrammarAST)Match(input, ACTION, FOLLOW_ACTION_in_elementOption510); if (state.failed) return retval;

                            if (state.backtracking == 1)
                                if (_first_1 == null) _first_1 = v;

                            Match(input, Token.UP, null); if (state.failed) return retval;
                            _last = _save_last_1;
                        }


                        if (state.backtracking == 1)
                        {
                            retval.tree = _first_0;
                            if (adaptor.getParent(retval.tree) != null && adaptor.isNil(adaptor.getParent(retval.tree)))
                                retval.tree = (GrammarAST)adaptor.getParent(retval.tree);
                        }

                    }
                    break;
                case 5:
                    // org\\antlr\\v4\\parse\\BlockSetTransformer.g:128:4: ^( ASSIGN ID v= INT )
                    {
                        _last = (GrammarAST)input.LT(1);
                        {
                            GrammarAST _save_last_1 = _last;
                            GrammarAST _first_1 = null;
                            _last = (GrammarAST)input.LT(1);
                            ASSIGN36 = (GrammarAST)Match(input, ASSIGN, FOLLOW_ASSIGN_in_elementOption517); if (state.failed) return retval;

                            if (state.backtracking == 1)
                                if (_first_0 == null) _first_0 = ASSIGN36;
                            Match(input, Token.DOWN, null); if (state.failed) return retval;
                            _last = (GrammarAST)input.LT(1);
                            ID37 = (GrammarAST)Match(input, ID, FOLLOW_ID_in_elementOption519); if (state.failed) return retval;

                            if (state.backtracking == 1)
                                if (_first_1 == null) _first_1 = ID37;

                            _last = (GrammarAST)input.LT(1);
                            v = (GrammarAST)Match(input, INT, FOLLOW_INT_in_elementOption523); if (state.failed) return retval;

                            if (state.backtracking == 1)
                                if (_first_1 == null) _first_1 = v;

                            Match(input, Token.UP, null); if (state.failed) return retval;
                            _last = _save_last_1;
                        }


                        if (state.backtracking == 1)
                        {
                            retval.tree = _first_0;
                            if (adaptor.getParent(retval.tree) != null && adaptor.isNil(adaptor.getParent(retval.tree)))
                                retval.tree = (GrammarAST)adaptor.getParent(retval.tree);
                        }

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
    // $ANTLR end "elementOption"

    // Delegated rules




    //static final short[] DFA10_eot = DFA.unpackEncodedString(DFA10_eotS);
    //static final short[] DFA10_eof = DFA.unpackEncodedString(DFA10_eofS);
    //static final char[] DFA10_min = DFA.unpackEncodedStringToUnsignedChars(DFA10_minS);
    //static final char[] DFA10_max = DFA.unpackEncodedStringToUnsignedChars(DFA10_maxS);
    //static final short[] DFA10_accept = DFA.unpackEncodedString(DFA10_acceptS);
    //static final short[] DFA10_special = DFA.unpackEncodedString(DFA10_specialS);
    //static final short[][] DFA10_transition;
    static readonly short[] DFA10_eot = RuntimeUtils.Convert(new char[] { '\u007c', '\uffff' });
    static readonly short[] DFA10_eof = RuntimeUtils.Convert(new char[] { '\u007c', '\uffff' });
    static readonly char[] DFA10_min = new char[] { '\u0001', '\u0046', '\u0001', '\u0002', '\u0001', '\u0045', '\u0001', '\u0002', '\u0001', '\u0031', '\u0004', '\u0002', '\u0001', '\u0003', '\u0001', '\u0049', '\u0001', '\u0045', '\u0001', '\u0049', '\u0001', '\u003b', '\u0001', '\u0003', '\u0001', '\u0002', '\u0001', '\u0031', '\u0003', '\u0002', '\u0001', '\u003b', '\u0001', '\u001c', '\u0001', '\u0003', '\u0001', '\u0031', '\u0002', '\u0003', '\u0001', '\u0004', '\u0001', '\u0003', '\u0001', '\u0002', '\u0001', '\u0003', '\u0004', '\u0002', '\u0001', '\u0003', '\u0001', '\u0002', '\u0006', '\u0003', '\u0001', '\u001c', '\u0002', '\u0003', '\u0001', '\u0049', '\u0001', '\u0003', '\u0001', '\u0049', '\u0001', '\u003b', '\u0001', '\u001c', '\u0005', '\u0003', '\u0001', '\u0004', '\u0001', '\u0003', '\u0001', '\u0002', '\u0001', '\u0031', '\u0001', '\u0002', '\u0001', '\u0000', '\u0001', '\u0002', '\u0001', '\u003b', '\u0001', '\u0004', '\u0004', '\u0003', '\u0001', '\u001c', '\u0001', '\u0003', '\u0002', '\uffff', '\u000a', '\u0003', '\u0001', '\u0004', '\u0001', '\u0003', '\u0001', '\u0002', '\u0002', '\u0003', '\u0001', '\u0002', '\u000a', '\u0003', '\u0001', '\u001c', '\u0001', '\u0003', '\u0001', '\u001c', '\u0005', '\u0003', '\u0002', '\u0004', '\u0010', '\u0003' };
    static readonly char[] DFA10_max = new char[] { '\u0001', '\u0046', '\u0001', '\u0002', '\u0001', '\u0045', '\u0001', '\u0002', '\u0001', '\u0049', '\u0001', '\u0002', '\u0002', '\u0003', '\u0001', '\u0002', '\u0001', '\u001c', '\u0001', '\u0049', '\u0001', '\u0045', '\u0001', '\u0049', '\u0001', '\u003b', '\u0001', '\u001c', '\u0001', '\u0002', '\u0001', '\u003e', '\u0003', '\u0002', '\u0001', '\u003b', '\u0002', '\u001c', '\u0001', '\u0049', '\u0001', '\u001c', '\u0001', '\u0003', '\u0001', '\u003b', '\u0001', '\u001c', '\u0001', '\u0002', '\u0001', '\u0003', '\u0001', '\u0002', '\u0002', '\u0003', '\u0001', '\u0002', '\u0001', '\u001c', '\u0001', '\u0002', '\u0006', '\u0003', '\u0001', '\u001c', '\u0001', '\u0003', '\u0001', '\u001c', '\u0001', '\u0049', '\u0001', '\u0045', '\u0001', '\u0049', '\u0001', '\u003b', '\u0001', '\u001c', '\u0001', '\u0003', '\u0004', '\u001c', '\u0001', '\u003b', '\u0001', '\u001c', '\u0001', '\u0002', '\u0001', '\u003e', '\u0001', '\u0002', '\u0001', '\u0000', '\u0001', '\u0002', '\u0002', '\u003b', '\u0004', '\u0003', '\u0002', '\u001c', '\u0002', '\uffff', '\u0001', '\u001c', '\u0005', '\u0003', '\u0004', '\u001c', '\u0001', '\u003b', '\u0001', '\u001c', '\u0001', '\u0002', '\u0001', '\u0003', '\u0001', '\u001c', '\u0001', '\u0002', '\u0002', '\u0003', '\u0004', '\u001c', '\u0004', '\u0003', '\u0001', '\u001c', '\u0001', '\u0003', '\u0001', '\u001c', '\u0001', '\u0003', '\u0004', '\u001c', '\u0002', '\u003b', '\u0008', '\u0003', '\u0008', '\u001c' };
    static readonly short[] DFA10_accept = RuntimeUtils.Convert(new char[] { '\u0046', '\uffff', '\u0001', '\u0001', '\u0001', '\u0002', '\u0034', '\uffff' });
    static readonly short[] DFA10_special = RuntimeUtils.Convert(new char[] { '\u003c', '\uffff', '\u0001', '\u0000', '\u003f', '\uffff', '\u007d', '\u003e' });
    static readonly short[][] DFA10_transition = new short[][]{
        RuntimeUtils.Convert(new char[] {'\u0001','\u0001'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u0002'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u0003'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u0004'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u0008','\u0009','\uffff','\u0001','\u0006','\u0002','\uffff','\u0001','\u0007','\u000a','\uffff','\u0001','\u0005'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u0009'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u000a','\u0001','\u000b'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u000c','\u0001','\u000b'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u000d'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u0010','\u0006','\uffff','\u0001','\u000f','\u0011','\uffff','\u0001','\u000e'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u0011'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u0012'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u0013'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u0014'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u0010','\u0006','\uffff','\u0001','\u000f','\u0011','\uffff','\u0001','\u000e'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u0015'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u0008','\u0009','\uffff','\u0001','\u0006','\u0002','\uffff','\u0001','\u0007'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u0016'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u0017'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u0018'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u0019'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u001a'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u001d','\u0006','\uffff','\u0001','\u001c','\u0011','\uffff','\u0001','\u001b'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u0021','\u0009','\uffff','\u0001','\u001f','\u0002','\uffff','\u0001','\u0020','\u000a','\uffff','\u0001','\u001e'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u0024','\u0006','\uffff','\u0001','\u0023','\u0011','\uffff','\u0001','\u0022'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u0025'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u0028','\u0017','\uffff','\u0001','\u0026','\u0001','\uffff','\u0001','\u0029','\u001c','\uffff','\u0001','\u0027'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u001d','\u0006','\uffff','\u0001','\u001c','\u0011','\uffff','\u0001','\u001b'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u002a'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u002b'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u002c'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u002d','\u0001','\u002e'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u002f','\u0001','\u002e'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u0030'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u0024','\u0006','\uffff','\u0001','\u0023','\u0011','\uffff','\u0001','\u0022'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u0031'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u0032'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u000b'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u0033'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u0034'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u0035'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u0036'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u0037'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u000b'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u003a','\u0006','\uffff','\u0001','\u0039','\u0011','\uffff','\u0001','\u0038'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u003b'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u003c','\u0041','\uffff','\u0001','\u0012'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u003d'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u003e'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u003f'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u000b'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u0010','\u0006','\uffff','\u0001','\u000f','\u0011','\uffff','\u0001','\u000e'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u0010','\u0006','\uffff','\u0001','\u000f','\u0011','\uffff','\u0001','\u000e'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u0010','\u0006','\uffff','\u0001','\u000f','\u0011','\uffff','\u0001','\u000e'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u0010','\u0006','\uffff','\u0001','\u000f','\u0011','\uffff','\u0001','\u000e'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u0042','\u0017','\uffff','\u0001','\u0040','\u0001','\uffff','\u0001','\u0043','\u001c','\uffff','\u0001','\u0041'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u003a','\u0006','\uffff','\u0001','\u0039','\u0011','\uffff','\u0001','\u0038'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u0044'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u0021','\u0009','\uffff','\u0001','\u001f','\u0002','\uffff','\u0001','\u0020'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u0045'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\uffff'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u0048'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u0049'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u004c','\u0017','\uffff','\u0001','\u004a','\u0001','\uffff','\u0001','\u004d','\u001c','\uffff','\u0001','\u004b'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u004e'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u004f'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u0050'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u0051'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u0052'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u0055','\u0006','\uffff','\u0001','\u0054','\u0011','\uffff','\u0001','\u0053'}),
        RuntimeUtils.Convert(new char[0]),
        RuntimeUtils.Convert(new char[0]),
        RuntimeUtils.Convert(new char[] {'\u0001','\u0058','\u0006','\uffff','\u0001','\u0057','\u0011','\uffff','\u0001','\u0056'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u0059'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u005a'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u005b'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u005c'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u005d'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u001d','\u0006','\uffff','\u0001','\u001c','\u0011','\uffff','\u0001','\u001b'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u001d','\u0006','\uffff','\u0001','\u001c','\u0011','\uffff','\u0001','\u001b'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u001d','\u0006','\uffff','\u0001','\u001c','\u0011','\uffff','\u0001','\u001b'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u001d','\u0006','\uffff','\u0001','\u001c','\u0011','\uffff','\u0001','\u001b'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u0060','\u0017','\uffff','\u0001','\u005e','\u0001','\uffff','\u0001','\u0061','\u001c','\uffff','\u0001','\u005f'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u0055','\u0006','\uffff','\u0001','\u0054','\u0011','\uffff','\u0001','\u0053'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u0062'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u0063'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u0058','\u0006','\uffff','\u0001','\u0057','\u0011','\uffff','\u0001','\u0056'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u0064'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u0065'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u002e'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u0024','\u0006','\uffff','\u0001','\u0023','\u0011','\uffff','\u0001','\u0022'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u0024','\u0006','\uffff','\u0001','\u0023','\u0011','\uffff','\u0001','\u0022'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u0024','\u0006','\uffff','\u0001','\u0023','\u0011','\uffff','\u0001','\u0022'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u0024','\u0006','\uffff','\u0001','\u0023','\u0011','\uffff','\u0001','\u0022'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u0066'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u0067'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u0068'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u0069'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u006a'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u002e'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u006b'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u002e'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u003a','\u0006','\uffff','\u0001','\u0039','\u0011','\uffff','\u0001','\u0038'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u003a','\u0006','\uffff','\u0001','\u0039','\u0011','\uffff','\u0001','\u0038'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u003a','\u0006','\uffff','\u0001','\u0039','\u0011','\uffff','\u0001','\u0038'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u003a','\u0006','\uffff','\u0001','\u0039','\u0011','\uffff','\u0001','\u0038'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u006e','\u0017','\uffff','\u0001','\u006c','\u0001','\uffff','\u0001','\u006f','\u001c','\uffff','\u0001','\u006d'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u0072','\u0017','\uffff','\u0001','\u0070','\u0001','\uffff','\u0001','\u0073','\u001c','\uffff','\u0001','\u0071'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u0074'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u0075'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u0076'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u0077'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u0078'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u0079'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u007a'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u007b'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u0055','\u0006','\uffff','\u0001','\u0054','\u0011','\uffff','\u0001','\u0053'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u0055','\u0006','\uffff','\u0001','\u0054','\u0011','\uffff','\u0001','\u0053'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u0055','\u0006','\uffff','\u0001','\u0054','\u0011','\uffff','\u0001','\u0053'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u0055','\u0006','\uffff','\u0001','\u0054','\u0011','\uffff','\u0001','\u0053'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u0058','\u0006','\uffff','\u0001','\u0057','\u0011','\uffff','\u0001','\u0056'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u0058','\u0006','\uffff','\u0001','\u0057','\u0011','\uffff','\u0001','\u0056'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u0058','\u0006','\uffff','\u0001','\u0057','\u0011','\uffff','\u0001','\u0056'}),
        RuntimeUtils.Convert(new char[] {'\u0001','\u0058','\u0006','\uffff','\u0001','\u0057','\u0011','\uffff','\u0001','\u0056'})
    };



    protected class DFA10 : antlr.runtime.DFA
    {
        protected readonly BlockSetTransformer transformer;

        public DFA10(BlockSetTransformer transformer)
        {
            this.recognizer = this.transformer = transformer;
            this.decisionNumber = 10;
            this.eot = DFA10_eot;
            this.eof = DFA10_eof;
            this.min = DFA10_min;
            this.max = DFA10_max;
            this.accept = DFA10_accept;
            this.special = DFA10_special;
            this.transition = DFA10_transition;
        }
        //@Override
        public String getDescription()
        {
            return "90:1: blockSet : ({...}? ^( BLOCK ^(alt= ALT ( elementOptions )? {...}? setElement[inLexer] ) ( ^( ALT ( elementOptions )? setElement[inLexer] ) )+ ) -> ^( BLOCK[$BLOCK.token] ^( ALT[$BLOCK.token,\"ALT\"] ^( SET[$BLOCK.token, \"SET\"] ( setElement )+ ) ) ) |{...}? ^( BLOCK ^( ALT ( elementOptions )? setElement[inLexer] ) ( ^( ALT ( elementOptions )? setElement[inLexer] ) )+ ) -> ^( SET[$BLOCK.token, \"SET\"] ( setElement )+ ) );";
        }
        //@Override
        public int specialStateTransition(int s, IntStream _input)
        {
            TreeNodeStream input = (TreeNodeStream)_input;
            int _s = s;
            switch (s)
            {
                case 0:
                    int LA10_60 = input.LA(1);

                    int index10_60 = input.Index;
                    input.Rewind();
                    s = -1;
                    if (((this.transformer.InContext("RULE")))) { s = 70; }
                    else if (((!this.transformer.InContext("RULE")))) { s = 71; }

                    input.Seek(index10_60);
                    if (s >= 0) return s;
                    break;
            }
            if (this.transformer.state.backtracking > 0) { this.transformer.state.failed = true; return -1; }
            NoViableAltException nvae =

                        new NoViableAltException(getDescription(), 10, _s, input);
            Error(nvae);
            throw nvae;
        }
    }

    public static readonly BitSet FOLLOW_RULE_in_topdown86 = new BitSet(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_TOKEN_REF_in_topdown91 = new BitSet(new ulong[] { 0xFFFFFFFFFFFFFFF0L, 0x00000000000FFFFFL });
    public static readonly BitSet FOLLOW_RULE_REF_in_topdown95 = new BitSet(new ulong[] { 0xFFFFFFFFFFFFFFF0L, 0x00000000000FFFFFL });
    public static readonly BitSet FOLLOW_setAlt_in_topdown110 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_ebnfBlockSet_in_topdown118 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_blockSet_in_topdown126 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_ALT_in_setAlt141 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_ebnfSuffix_in_ebnfBlockSet161 = new BitSet(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_blockSet_in_ebnfBlockSet163 = new BitSet(new long[] { 0x0000000000000008L });
    public static readonly BitSet FOLLOW_BLOCK_in_blockSet244 = new BitSet(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_ALT_in_blockSet249 = new BitSet(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_elementOptions_in_blockSet251 = new BitSet(new long[] { 0x4802000000000000L });
    public static readonly BitSet FOLLOW_setElement_in_blockSet256 = new BitSet(new long[] { 0x0000000000000008L });
    public static readonly BitSet FOLLOW_ALT_in_blockSet263 = new BitSet(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_elementOptions_in_blockSet265 = new BitSet(new long[] { 0x4802000000000000L });
    public static readonly BitSet FOLLOW_setElement_in_blockSet268 = new BitSet(new long[] { 0x0000000000000008L });
    public static readonly BitSet FOLLOW_BLOCK_in_blockSet313 = new BitSet(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_ALT_in_blockSet316 = new BitSet(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_elementOptions_in_blockSet318 = new BitSet(new long[] { 0x4802000000000000L });
    public static readonly BitSet FOLLOW_setElement_in_blockSet321 = new BitSet(new long[] { 0x0000000000000008L });
    public static readonly BitSet FOLLOW_ALT_in_blockSet328 = new BitSet(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_elementOptions_in_blockSet330 = new BitSet(new long[] { 0x4802000000000000L });
    public static readonly BitSet FOLLOW_setElement_in_blockSet333 = new BitSet(new long[] { 0x0000000000000008L });
    public static readonly BitSet FOLLOW_STRING_LITERAL_in_setElement373 = new BitSet(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_elementOptions_in_setElement375 = new BitSet(new long[] { 0x0000000000000008L });
    public static readonly BitSet FOLLOW_STRING_LITERAL_in_setElement388 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_TOKEN_REF_in_setElement400 = new BitSet(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_elementOptions_in_setElement402 = new BitSet(new long[] { 0x0000000000000008L });
    public static readonly BitSet FOLLOW_TOKEN_REF_in_setElement414 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_RANGE_in_setElement425 = new BitSet(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_STRING_LITERAL_in_setElement429 = new BitSet(new long[] { 0x0800000000000000L });
    public static readonly BitSet FOLLOW_STRING_LITERAL_in_setElement433 = new BitSet(new long[] { 0x0000000000000008L });
    public static readonly BitSet FOLLOW_ELEMENT_OPTIONS_in_elementOptions455 = new BitSet(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_elementOption_in_elementOptions457 = new BitSet(new long[] { 0x0000000010000408L });
    public static readonly BitSet FOLLOW_ID_in_elementOption470 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_ASSIGN_in_elementOption476 = new BitSet(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_ID_in_elementOption480 = new BitSet(new long[] { 0x0000000010000000L });
    public static readonly BitSet FOLLOW_ID_in_elementOption484 = new BitSet(new long[] { 0x0000000000000008L });
    public static readonly BitSet FOLLOW_ASSIGN_in_elementOption491 = new BitSet(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_ID_in_elementOption493 = new BitSet(new long[] { 0x0800000000000000L });
    public static readonly BitSet FOLLOW_STRING_LITERAL_in_elementOption497 = new BitSet(new long[] { 0x0000000000000008L });
    public static readonly BitSet FOLLOW_ASSIGN_in_elementOption504 = new BitSet(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_ID_in_elementOption506 = new BitSet(new long[] { 0x0000000000000010L });
    public static readonly BitSet FOLLOW_ACTION_in_elementOption510 = new BitSet(new long[] { 0x0000000000000008L });
    public static readonly BitSet FOLLOW_ASSIGN_in_elementOption517 = new BitSet(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_ID_in_elementOption519 = new BitSet(new long[] { 0x0000000040000000L });
    public static readonly BitSet FOLLOW_INT_in_elementOption523 = new BitSet(new long[] { 0x0000000000000008L });
}
