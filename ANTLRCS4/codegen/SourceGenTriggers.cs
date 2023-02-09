﻿// $ANTLR 3.5.3 org\\antlr\\v4\\codegen\\SourceGenTriggers.g 2023-01-27 22:27:34

using org.antlr.runtime.tree;
using org.antlr.runtime;
using org.antlr.v4.codegen.model;
using org.antlr.v4.runtime;
using org.antlr.v4.tool.ast;
using org.antlr.v4.runtime.misc;

namespace org.antlr.v4.codegen;
public class SourceGenTriggers : TreeParser
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
    public TreeParser[] GetDelegates()
    {
        return new TreeParser[] { };
    }

    // delegators

    protected DFA7 dfa7;

    public SourceGenTriggers(TreeNodeStream input, OutputModelController controller)
        : this(input)
    {
        this.controller = controller;
    }
    public SourceGenTriggers(TreeNodeStream input)
        : this(input, new RecognizerSharedState())
    {
    }

    public SourceGenTriggers(TreeNodeStream input, RecognizerSharedState state)
        : base(input, state)
    {
        dfa7 = new DFA7(this);
    }

    //@Override 
    public override string[] TokenNames => tokenNames;
    //@Override 
    public override string GrammarFileName => "org\\antlr\\v4\\codegen\\SourceGenTriggers.g";


    public OutputModelController controller;
    public bool hasLookaheadBlock;



    // $ANTLR start "dummy"
    // org\\antlr\\v4\\codegen\\SourceGenTriggers.g:59:1: dummy : block[null, null] ;
    public void Dummy()
    {
        try
        {
            // org\\antlr\\v4\\codegen\\SourceGenTriggers.g:59:7: ( block[null, null] )
            // org\\antlr\\v4\\codegen\\SourceGenTriggers.g:59:9: block[null, null]
            {
                PushFollow(FOLLOW_block_in_dummy61);
                Block(null, null);
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



    // $ANTLR start "block"
    // org\\antlr\\v4\\codegen\\SourceGenTriggers.g:61:1: block[GrammarAST label, GrammarAST ebnfRoot] returns [List<SrcOp> omos] : ^(blk= BLOCK ( ^( OPTIONS ( . )+ ) )? ( alternative )+ ) ;
    public List<SrcOp> Block(GrammarAST label, GrammarAST ebnfRoot)
    {

        List<SrcOp> omos = null;
        GrammarAST blk = null;
        TreeRuleReturnScope alternative1 = null;

        try
        {
            // org\\antlr\\v4\\codegen\\SourceGenTriggers.g:62:5: ( ^(blk= BLOCK ( ^( OPTIONS ( . )+ ) )? ( alternative )+ ) )
            // org\\antlr\\v4\\codegen\\SourceGenTriggers.g:62:7: ^(blk= BLOCK ( ^( OPTIONS ( . )+ ) )? ( alternative )+ )
            {
                blk = (GrammarAST)Match(input, BLOCK, FOLLOW_BLOCK_in_block84);
                Match(input, Token.DOWN, null);
                // org\\antlr\\v4\\codegen\\SourceGenTriggers.g:62:20: ( ^( OPTIONS ( . )+ ) )?
                int alt2 = 2;
                int LA2_0 = input.LA(1);
                if ((LA2_0 == OPTIONS))
                {
                    alt2 = 1;
                }
                switch (alt2)
                {
                    case 1:
                        // org\\antlr\\v4\\codegen\\SourceGenTriggers.g:62:21: ^( OPTIONS ( . )+ )
                        {
                            Match(input, OPTIONS, FOLLOW_OPTIONS_in_block88);
                            Match(input, Token.DOWN, null);
                            // org\\antlr\\v4\\codegen\\SourceGenTriggers.g:62:31: ( . )+
                            int cnt1 = 0;
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
                                        // org\\antlr\\v4\\codegen\\SourceGenTriggers.g:62:31: .
                                        {
                                            MatchAny(input);
                                        }
                                        break;

                                    default:
                                        if (cnt1 >= 1) goto exit1;// break loop1;
                                        var eee = new EarlyExitException(1, input);
                                        throw eee;
                                }
                                cnt1++;
                            }
                        exit1:
                            Match(input, Token.UP, null);

                        }
                        break;

                }

                List<CodeBlockForAlt> alts = new();
                // org\\antlr\\v4\\codegen\\SourceGenTriggers.g:64:7: ( alternative )+
                int cnt3 = 0;
            loop3:
                while (true)
                {
                    int alt3 = 2;
                    int LA3_0 = input.LA(1);
                    if ((LA3_0 == ALT))
                    {
                        alt3 = 1;
                    }

                    switch (alt3)
                    {
                        case 1:
                            // org\\antlr\\v4\\codegen\\SourceGenTriggers.g:64:9: alternative
                            {
                                PushFollow(FOLLOW_alternative_in_block109);
                                alternative1 = Alternative();
                                state._fsp--;

                                alts.Add((alternative1 != null ? ((SourceGenTriggers.AlternativeReturn)alternative1).altCodeBlock : null));
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


                if (alts.Count == 1 && ebnfRoot == null) return alts.Cast<SrcOp>().ToList();
                if (ebnfRoot == null)
                {
                    omos = DefaultOutputModelFactory.List(controller.GetChoiceBlock((BlockAST)blk, alts, label));
                }
                else
                {
                    Choice choice = controller.GetEBNFBlock(ebnfRoot, alts);
                    hasLookaheadBlock |= choice is PlusBlock || choice is StarBlock;
                    omos = DefaultOutputModelFactory.List(choice);
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
        return omos;
    }
    // $ANTLR end "block"


    public class AlternativeReturn : TreeRuleReturnScope
    {

        public CodeBlockForAlt altCodeBlock;
        public List<SrcOp> ops;
    };


    // $ANTLR start "alternative"
    // org\\antlr\\v4\\codegen\\SourceGenTriggers.g:79:1: alternative returns [CodeBlockForAlt altCodeBlock, List<SrcOp> ops] : a= alt[outerMost] ;
    public AlternativeReturn Alternative()
    {
        AlternativeReturn retval = new();
        retval.start = input.LT(1);
        bool outerMost = InContext("RULE BLOCK");

        try
        {
            // org\\antlr\\v4\\codegen\\SourceGenTriggers.g:86:5: (a= alt[outerMost] )
            // org\\antlr\\v4\\codegen\\SourceGenTriggers.g:86:7: a= alt[outerMost]
            {
                PushFollow(FOLLOW_alt_in_alternative161);
                state._fsp--;

                TreeRuleReturnScope a = Alt(outerMost);
                retval.altCodeBlock = (a != null ? ((SourceGenTriggers.AltReturn)a).altCodeBlock : null); retval.ops = (a != null ? ((SourceGenTriggers.AltReturn)a).ops : null);
            }


            controller.FinishAlternative(retval.altCodeBlock, retval.ops, outerMost);

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
    // $ANTLR end "alternative"


    public class AltReturn : TreeRuleReturnScope
    {

        public CodeBlockForAlt altCodeBlock;
        public List<SrcOp> ops;
    };


    // $ANTLR start "alt"
    // org\\antlr\\v4\\codegen\\SourceGenTriggers.g:89:1: alt[bool outerMost] returns [CodeBlockForAlt altCodeBlock, List<SrcOp> ops] : ( ^( ALT ( elementOptions )? ( element )+ ) | ^( ALT ( elementOptions )? EPSILON ) );
    public AltReturn Alt(bool outerMost)
    {
        var retval = new AltReturn();
        retval.start = input.LT(1);

        List<SrcOp> element2 = null;


        // set alt if outer ALT only (the only ones with alt field set to Alternative object)
        AltAST altAST = retval.start as AltAST;
        if (outerMost) controller.CurrentOuterMostAlt = altAST.alt;

        try
        {
            // org\\antlr\\v4\\codegen\\SourceGenTriggers.g:95:2: ( ^( ALT ( elementOptions )? ( element )+ ) | ^( ALT ( elementOptions )? EPSILON ) )
            int alt7 = 2;
            alt7 = dfa7.Predict(input);
            switch (alt7)
            {
                case 1:
                    // org\\antlr\\v4\\codegen\\SourceGenTriggers.g:95:4: ^( ALT ( elementOptions )? ( element )+ )
                    {

                        List<SrcOp> elems = new();
                        // TODO: shouldn't we pass ((GrammarAST)retval.start) to controller.alternative()?
                        retval.altCodeBlock = controller.Alternative(controller.CurrentOuterMostAlt, outerMost);
                        retval.altCodeBlock.ops = retval.ops = elems;
                        controller.CurrentBlock = retval.altCodeBlock;

                        Match(input, ALT, FOLLOW_ALT_in_alt191);
                        Match(input, Token.DOWN, null);
                        // org\\antlr\\v4\\codegen\\SourceGenTriggers.g:102:10: ( elementOptions )?
                        int alt4 = 2;
                        int LA4_0 = input.LA(1);
                        if ((LA4_0 == ELEMENT_OPTIONS))
                        {
                            alt4 = 1;
                        }
                        switch (alt4)
                        {
                            case 1:
                                // org\\antlr\\v4\\codegen\\SourceGenTriggers.g:102:10: elementOptions
                                {
                                    PushFollow(FOLLOW_elementOptions_in_alt193);
                                    ElementOptions();
                                    state._fsp--;

                                }
                                break;

                        }

                        // org\\antlr\\v4\\codegen\\SourceGenTriggers.g:102:26: ( element )+
                        int cnt5 = 0;
                    loop5:
                        while (true)
                        {
                            int alt5 = 2;
                            int LA5_0 = input.LA(1);
                            if ((LA5_0 == ACTION || LA5_0 == ASSIGN || LA5_0 == DOT || LA5_0 == NOT || LA5_0 == PLUS_ASSIGN || LA5_0 == RANGE || LA5_0 == RULE_REF || LA5_0 == SEMPRED || LA5_0 == STRING_LITERAL || LA5_0 == TOKEN_REF || (LA5_0 >= BLOCK && LA5_0 <= CLOSURE) || (LA5_0 >= OPTIONAL && LA5_0 <= POSITIVE_CLOSURE) || (LA5_0 >= SET && LA5_0 <= WILDCARD)))
                            {
                                alt5 = 1;
                            }

                            switch (alt5)
                            {
                                case 1:
                                    // org\\antlr\\v4\\codegen\\SourceGenTriggers.g:102:28: element
                                    {
                                        PushFollow(FOLLOW_element_in_alt198);
                                        element2 = Element();
                                        state._fsp--;

                                        if (element2 != null) elems.AddRange(element2);
                                    }
                                    break;

                                default:
                                    if (cnt5 >= 1) goto exit5;// break loop5;
                                    var eee = new EarlyExitException(5, input);
                                    throw eee;
                            }
                            cnt5++;
                        }
                    exit5:
                        Match(input, Token.UP, null);

                    }
                    break;
                case 2:
                    // org\\antlr\\v4\\codegen\\SourceGenTriggers.g:104:4: ^( ALT ( elementOptions )? EPSILON )
                    {
                        Match(input, ALT, FOLLOW_ALT_in_alt212);
                        Match(input, Token.DOWN, null);
                        // org\\antlr\\v4\\codegen\\SourceGenTriggers.g:104:10: ( elementOptions )?
                        int alt6 = 2;
                        int LA6_0 = input.LA(1);
                        if ((LA6_0 == ELEMENT_OPTIONS))
                        {
                            alt6 = 1;
                        }
                        switch (alt6)
                        {
                            case 1:
                                // org\\antlr\\v4\\codegen\\SourceGenTriggers.g:104:10: elementOptions
                                {
                                    PushFollow(FOLLOW_elementOptions_in_alt214);
                                    ElementOptions();
                                    state._fsp--;

                                }
                                break;

                        }

                        Match(input, EPSILON, FOLLOW_EPSILON_in_alt217);
                        Match(input, Token.UP, null);

                        retval.altCodeBlock = controller.Epsilon(controller.CurrentOuterMostAlt, outerMost);
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
    // $ANTLR end "alt"



    // $ANTLR start "element"
    // org\\antlr\\v4\\codegen\\SourceGenTriggers.g:108:1: element returns [List<SrcOp> omos] : ( labeledElement | atom[null,false] | subrule | ACTION | SEMPRED | ^( ACTION elementOptions ) | ^( SEMPRED elementOptions ) );
    public List<SrcOp> Element()
    {

        List<SrcOp> omos = null;

        GrammarAST ACTION6 = null;
        GrammarAST SEMPRED7 = null;
        GrammarAST ACTION8 = null;
        GrammarAST SEMPRED9 = null;
        List<SrcOp> labeledElement3 = null;
        List<SrcOp> atom4 = null;
        List<SrcOp> subrule5 = null;

        try
        {
            // org\\antlr\\v4\\codegen\\SourceGenTriggers.g:109:2: ( labeledElement | atom[null,false] | subrule | ACTION | SEMPRED | ^( ACTION elementOptions ) | ^( SEMPRED elementOptions ) )
            int alt8 = 7;
            switch (input.LA(1))
            {
                case ASSIGN:
                case PLUS_ASSIGN:
                    {
                        alt8 = 1;
                    }
                    break;
                case DOT:
                case NOT:
                case RANGE:
                case RULE_REF:
                case STRING_LITERAL:
                case TOKEN_REF:
                case SET:
                case WILDCARD:
                    {
                        alt8 = 2;
                    }
                    break;
                case BLOCK:
                case CLOSURE:
                case OPTIONAL:
                case POSITIVE_CLOSURE:
                    {
                        alt8 = 3;
                    }
                    break;
                case ACTION:
                    {
                        int LA8_4 = input.LA(2);
                        if ((LA8_4 == DOWN))
                        {
                            alt8 = 6;
                        }
                        else if (((LA8_4 >= UP && LA8_4 <= ACTION) || LA8_4 == ASSIGN || LA8_4 == DOT || LA8_4 == NOT || LA8_4 == PLUS_ASSIGN || LA8_4 == RANGE || LA8_4 == RULE_REF || LA8_4 == SEMPRED || LA8_4 == STRING_LITERAL || LA8_4 == TOKEN_REF || (LA8_4 >= BLOCK && LA8_4 <= CLOSURE) || (LA8_4 >= OPTIONAL && LA8_4 <= POSITIVE_CLOSURE) || (LA8_4 >= SET && LA8_4 <= WILDCARD)))
                        {
                            alt8 = 4;
                        }

                        else
                        {
                            int nvaeMark = input.Mark();
                            try
                            {
                                input.Consume();
                                NoViableAltException nvae2 =
                                    new NoViableAltException("", 8, 4, input);
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
                        int LA8_5 = input.LA(2);
                        if ((LA8_5 == DOWN))
                        {
                            alt8 = 7;
                        }
                        else if (((LA8_5 >= UP && LA8_5 <= ACTION) || LA8_5 == ASSIGN || LA8_5 == DOT || LA8_5 == NOT || LA8_5 == PLUS_ASSIGN || LA8_5 == RANGE || LA8_5 == RULE_REF || LA8_5 == SEMPRED || LA8_5 == STRING_LITERAL || LA8_5 == TOKEN_REF || (LA8_5 >= BLOCK && LA8_5 <= CLOSURE) || (LA8_5 >= OPTIONAL && LA8_5 <= POSITIVE_CLOSURE) || (LA8_5 >= SET && LA8_5 <= WILDCARD)))
                        {
                            alt8 = 5;
                        }

                        else
                        {
                            int nvaeMark = input.Mark();
                            try
                            {
                                input.Consume();
                                var nvae2 =
                                    new NoViableAltException("", 8, 5, input);
                                throw nvae2;
                            }
                            finally
                            {
                                input.Rewind(nvaeMark);
                            }
                        }

                    }
                    break;
                default:
                    var nvae =
                        new NoViableAltException("", 8, 0, input);
                    throw nvae;
            }
            switch (alt8)
            {
                case 1:
                    // org\\antlr\\v4\\codegen\\SourceGenTriggers.g:109:4: labeledElement
                    {
                        PushFollow(FOLLOW_labeledElement_in_element246);
                        labeledElement3 = LabeledElement();
                        state._fsp--;

                        omos = labeledElement3;
                    }
                    break;
                case 2:
                    // org\\antlr\\v4\\codegen\\SourceGenTriggers.g:110:4: atom[null,false]
                    {
                        PushFollow(FOLLOW_atom_in_element257);
                        atom4 = Atom(null, false);
                        state._fsp--;

                        omos = atom4;
                    }
                    break;
                case 3:
                    // org\\antlr\\v4\\codegen\\SourceGenTriggers.g:111:4: subrule
                    {
                        PushFollow(FOLLOW_subrule_in_element267);
                        subrule5 = Subrule();
                        state._fsp--;

                        omos = subrule5;
                    }
                    break;
                case 4:
                    // org\\antlr\\v4\\codegen\\SourceGenTriggers.g:112:6: ACTION
                    {
                        ACTION6 = (GrammarAST)Match(input, ACTION, FOLLOW_ACTION_in_element282);
                        omos = controller.Action((ActionAST)ACTION6);
                    }
                    break;
                case 5:
                    // org\\antlr\\v4\\codegen\\SourceGenTriggers.g:113:6: SEMPRED
                    {
                        SEMPRED7 = (GrammarAST)Match(input, SEMPRED, FOLLOW_SEMPRED_in_element297);
                        omos = controller.Sempred((ActionAST)SEMPRED7);
                    }
                    break;
                case 6:
                    // org\\antlr\\v4\\codegen\\SourceGenTriggers.g:114:4: ^( ACTION elementOptions )
                    {
                        ACTION8 = (GrammarAST)Match(input, ACTION, FOLLOW_ACTION_in_element311);
                        Match(input, Token.DOWN, null);
                        PushFollow(FOLLOW_elementOptions_in_element313);
                        ElementOptions();
                        state._fsp--;

                        Match(input, Token.UP, null);

                        omos = controller.Action((ActionAST)ACTION8);
                    }
                    break;
                case 7:
                    // org\\antlr\\v4\\codegen\\SourceGenTriggers.g:115:6: ^( SEMPRED elementOptions )
                    {
                        SEMPRED9 = (GrammarAST)Match(input, SEMPRED, FOLLOW_SEMPRED_in_element325);
                        Match(input, Token.DOWN, null);
                        PushFollow(FOLLOW_elementOptions_in_element327);
                        ElementOptions();
                        state._fsp--;

                        Match(input, Token.UP, null);

                        omos = controller.Sempred((ActionAST)SEMPRED9);
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
        return omos;
    }
    // $ANTLR end "element"



    // $ANTLR start "labeledElement"
    // org\\antlr\\v4\\codegen\\SourceGenTriggers.g:118:1: labeledElement returns [List<SrcOp> omos] : ( ^( ASSIGN ID atom[$ID,false] ) | ^( PLUS_ASSIGN ID atom[$ID,false] ) | ^( ASSIGN ID block[$ID,null] ) | ^( PLUS_ASSIGN ID block[$ID,null] ) );
    public List<SrcOp> LabeledElement()
    {

        List<SrcOp> omos = null;
        GrammarAST ID10 = null;
        GrammarAST ID12 = null;
        GrammarAST ID14 = null;
        GrammarAST ID16 = null;
        List<SrcOp> atom11 = null;
        List<SrcOp> atom13 = null;
        List<SrcOp> block15 = null;
        List<SrcOp> block17 = null;

        try
        {
            // org\\antlr\\v4\\codegen\\SourceGenTriggers.g:119:2: ( ^( ASSIGN ID atom[$ID,false] ) | ^( PLUS_ASSIGN ID atom[$ID,false] ) | ^( ASSIGN ID block[$ID,null] ) | ^( PLUS_ASSIGN ID block[$ID,null] ) )
            int alt9 = 4;
            int LA9_0 = input.LA(1);
            if ((LA9_0 == ASSIGN))
            {
                int LA9_1 = input.LA(2);
                if ((LA9_1 == DOWN))
                {
                    int LA9_3 = input.LA(3);
                    if ((LA9_3 == ID))
                    {
                        int LA9_5 = input.LA(4);
                        if ((LA9_5 == DOT || LA9_5 == NOT || LA9_5 == RANGE || LA9_5 == RULE_REF || LA9_5 == STRING_LITERAL || LA9_5 == TOKEN_REF || (LA9_5 >= SET && LA9_5 <= WILDCARD)))
                        {
                            alt9 = 1;
                        }
                        else if ((LA9_5 == BLOCK))
                        {
                            alt9 = 3;
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
                                var nvae =
                                    new NoViableAltException("", 9, 5, input);
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
                            var nvae =
                                new NoViableAltException("", 9, 3, input);
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
                        var nvae =
                            new NoViableAltException("", 9, 1, input);
                        throw nvae;
                    }
                    finally
                    {
                        input.Rewind(nvaeMark);
                    }
                }

            }
            else if ((LA9_0 == PLUS_ASSIGN))
            {
                int LA9_2 = input.LA(2);
                if ((LA9_2 == DOWN))
                {
                    int LA9_4 = input.LA(3);
                    if ((LA9_4 == ID))
                    {
                        int LA9_6 = input.LA(4);
                        if ((LA9_6 == DOT || LA9_6 == NOT || LA9_6 == RANGE || LA9_6 == RULE_REF || LA9_6 == STRING_LITERAL || LA9_6 == TOKEN_REF || (LA9_6 >= SET && LA9_6 <= WILDCARD)))
                        {
                            alt9 = 2;
                        }
                        else if ((LA9_6 == BLOCK))
                        {
                            alt9 = 4;
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
                                    new NoViableAltException("", 9, 6, input);
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
                            var nvae =
                                new NoViableAltException("", 9, 4, input);
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
                        var nvae =
                            new NoViableAltException("", 9, 2, input);
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
                var nvae =
                    new NoViableAltException("", 9, 0, input);
                throw nvae;
            }

            switch (alt9)
            {
                case 1:
                    // org\\antlr\\v4\\codegen\\SourceGenTriggers.g:119:4: ^( ASSIGN ID atom[$ID,false] )
                    {
                        Match(input, ASSIGN, FOLLOW_ASSIGN_in_labeledElement347);
                        Match(input, Token.DOWN, null);
                        ID10 = (GrammarAST)Match(input, ID, FOLLOW_ID_in_labeledElement349);
                        PushFollow(FOLLOW_atom_in_labeledElement351);
                        atom11 = Atom(ID10, false);
                        state._fsp--;

                        Match(input, Token.UP, null);

                        omos = atom11;
                    }
                    break;
                case 2:
                    // org\\antlr\\v4\\codegen\\SourceGenTriggers.g:120:4: ^( PLUS_ASSIGN ID atom[$ID,false] )
                    {
                        Match(input, PLUS_ASSIGN, FOLLOW_PLUS_ASSIGN_in_labeledElement364);
                        Match(input, Token.DOWN, null);
                        ID12 = (GrammarAST)Match(input, ID, FOLLOW_ID_in_labeledElement366);
                        PushFollow(FOLLOW_atom_in_labeledElement368);
                        atom13 = Atom(ID12, false);
                        state._fsp--;

                        Match(input, Token.UP, null);

                        omos = atom13;
                    }
                    break;
                case 3:
                    // org\\antlr\\v4\\codegen\\SourceGenTriggers.g:121:4: ^( ASSIGN ID block[$ID,null] )
                    {
                        Match(input, ASSIGN, FOLLOW_ASSIGN_in_labeledElement379);
                        Match(input, Token.DOWN, null);
                        ID14 = (GrammarAST)Match(input, ID, FOLLOW_ID_in_labeledElement381);
                        PushFollow(FOLLOW_block_in_labeledElement383);
                        block15 = Block(ID14, null);
                        state._fsp--;

                        Match(input, Token.UP, null);

                        omos = block15;
                    }
                    break;
                case 4:
                    // org\\antlr\\v4\\codegen\\SourceGenTriggers.g:122:4: ^( PLUS_ASSIGN ID block[$ID,null] )
                    {
                        Match(input, PLUS_ASSIGN, FOLLOW_PLUS_ASSIGN_in_labeledElement396);
                        Match(input, Token.DOWN, null);
                        ID16 = (GrammarAST)Match(input, ID, FOLLOW_ID_in_labeledElement398);
                        PushFollow(FOLLOW_block_in_labeledElement400);
                        block17 = Block(ID16, null);
                        state._fsp--;

                        Match(input, Token.UP, null);

                        omos = block17;
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
        return omos;
    }
    // $ANTLR end "labeledElement"



    // $ANTLR start "subrule"
    // org\\antlr\\v4\\codegen\\SourceGenTriggers.g:125:1: subrule returns [List<SrcOp> omos] : ( ^( OPTIONAL b= block[null,$OPTIONAL] ) | ( ^(op= CLOSURE b= block[null,null] ) | ^(op= POSITIVE_CLOSURE b= block[null,null] ) ) | block[null, null] );
    public List<SrcOp> Subrule()
    {

        List<SrcOp> omos = null;
        GrammarAST op = null;
        GrammarAST OPTIONAL18 = null;
        List<SrcOp> b = null;
        List<SrcOp> block19 = null;

        try
        {
            // org\\antlr\\v4\\codegen\\SourceGenTriggers.g:126:2: ( ^( OPTIONAL b= block[null,$OPTIONAL] ) | ( ^(op= CLOSURE b= block[null,null] ) | ^(op= POSITIVE_CLOSURE b= block[null,null] ) ) | block[null, null] )
            int alt11 = 3;
            switch (input.LA(1))
            {
                case OPTIONAL:
                    {
                        alt11 = 1;
                    }
                    break;
                case CLOSURE:
                case POSITIVE_CLOSURE:
                    {
                        alt11 = 2;
                    }
                    break;
                case BLOCK:
                    {
                        alt11 = 3;
                    }
                    break;
                default:
                    var nvae =
                        new NoViableAltException("", 11, 0, input);
                    throw nvae;
            }
            switch (alt11)
            {
                case 1:
                    // org\\antlr\\v4\\codegen\\SourceGenTriggers.g:126:4: ^( OPTIONAL b= block[null,$OPTIONAL] )
                    {
                        OPTIONAL18 = (GrammarAST)Match(input, OPTIONAL, FOLLOW_OPTIONAL_in_subrule421);
                        Match(input, Token.DOWN, null);
                        PushFollow(FOLLOW_block_in_subrule425);
                        b = Block(null, OPTIONAL18);
                        state._fsp--;

                        Match(input, Token.UP, null);


                        omos = b;

                    }
                    break;
                case 2:
                    // org\\antlr\\v4\\codegen\\SourceGenTriggers.g:130:4: ( ^(op= CLOSURE b= block[null,null] ) | ^(op= POSITIVE_CLOSURE b= block[null,null] ) )
                    {
                        // org\\antlr\\v4\\codegen\\SourceGenTriggers.g:130:4: ( ^(op= CLOSURE b= block[null,null] ) | ^(op= POSITIVE_CLOSURE b= block[null,null] ) )
                        int alt10 = 2;
                        int LA10_0 = input.LA(1);
                        if ((LA10_0 == CLOSURE))
                        {
                            alt10 = 1;
                        }
                        else if ((LA10_0 == POSITIVE_CLOSURE))
                        {
                            alt10 = 2;
                        }

                        else
                        {
                            var nvae =
                                new NoViableAltException("", 10, 0, input);
                            throw nvae;
                        }

                        switch (alt10)
                        {
                            case 1:
                                // org\\antlr\\v4\\codegen\\SourceGenTriggers.g:130:6: ^(op= CLOSURE b= block[null,null] )
                                {
                                    op = (GrammarAST)Match(input, CLOSURE, FOLLOW_CLOSURE_in_subrule441);
                                    Match(input, Token.DOWN, null);
                                    PushFollow(FOLLOW_block_in_subrule445);
                                    b = Block(null, null);
                                    state._fsp--;

                                    Match(input, Token.UP, null);

                                }
                                break;
                            case 2:
                                // org\\antlr\\v4\\codegen\\SourceGenTriggers.g:131:5: ^(op= POSITIVE_CLOSURE b= block[null,null] )
                                {
                                    op = (GrammarAST)Match(input, POSITIVE_CLOSURE, FOLLOW_POSITIVE_CLOSURE_in_subrule456);
                                    Match(input, Token.DOWN, null);
                                    PushFollow(FOLLOW_block_in_subrule460);
                                    b = Block(null, null);
                                    state._fsp--;

                                    Match(input, Token.UP, null);

                                }
                                break;

                        }


                        List<CodeBlockForAlt> alts = new();
                        var blk = b[0];
                        CodeBlockForAlt alt = new(controller.@delegate);
                        alt.AddOp(blk);
                        alts.Add(alt);
                        var loop = controller.GetEBNFBlock(op, alts); // "star it"
                        hasLookaheadBlock |= loop is PlusBlock || loop is StarBlock;
                        omos = DefaultOutputModelFactory.List(loop);

                    }
                    break;
                case 3:
                    // org\\antlr\\v4\\codegen\\SourceGenTriggers.g:143:5: block[null, null]
                    {
                        PushFollow(FOLLOW_block_in_subrule476);
                        block19 = Block(null, null);
                        state._fsp--;

                        omos = block19;
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
        return omos;
    }
    // $ANTLR end "subrule"



    // $ANTLR start "blockSet"
    // org\\antlr\\v4\\codegen\\SourceGenTriggers.g:146:1: blockSet[GrammarAST label, bool invert] returns [List<SrcOp> omos] : ^( SET ( atom[label,invert] )+ ) ;
    public List<SrcOp> BlockSet(GrammarAST label, bool invert)
    {

        List<SrcOp> omos = null;


        GrammarAST SET20 = null;

        try
        {
            // org\\antlr\\v4\\codegen\\SourceGenTriggers.g:147:5: ( ^( SET ( atom[label,invert] )+ ) )
            // org\\antlr\\v4\\codegen\\SourceGenTriggers.g:147:7: ^( SET ( atom[label,invert] )+ )
            {
                SET20 = (GrammarAST)Match(input, SET, FOLLOW_SET_in_blockSet506);
                Match(input, Token.DOWN, null);
                // org\\antlr\\v4\\codegen\\SourceGenTriggers.g:147:13: ( atom[label,invert] )+
                int cnt12 = 0;
            loop12:
                while (true)
                {
                    int alt12 = 2;
                    int LA12_0 = input.LA(1);
                    if ((LA12_0 == DOT || LA12_0 == NOT || LA12_0 == RANGE || LA12_0 == RULE_REF || LA12_0 == STRING_LITERAL || LA12_0 == TOKEN_REF || (LA12_0 >= SET && LA12_0 <= WILDCARD)))
                    {
                        alt12 = 1;
                    }

                    switch (alt12)
                    {
                        case 1:
                            // org\\antlr\\v4\\codegen\\SourceGenTriggers.g:147:13: atom[label,invert]
                            {
                                PushFollow(FOLLOW_atom_in_blockSet508);
                                Atom(label, invert);
                                state._fsp--;
                            }
                            break;

                        default:
                            if (cnt12 >= 1) goto exit12;// break loop12;
                            EarlyExitException eee = new EarlyExitException(12, input);
                            throw eee;
                    }
                    cnt12++;
                }
            exit12:
                Match(input, Token.UP, null);

                omos = controller.Set(SET20, label, invert);
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
        return omos;
    }
    // $ANTLR end "blockSet"



    // $ANTLR start "atom"
    // org\\antlr\\v4\\codegen\\SourceGenTriggers.g:160:1: atom[GrammarAST label, bool invert] returns [List<SrcOp> omos] : ( ^( NOT a= atom[$label, true] ) | range[label] | ^( DOT ID terminal[$label] ) | ^( DOT ID ruleref[$label] ) | ^( WILDCARD . ) | WILDCARD | terminal[label] | ruleref[label] | blockSet[$label, invert] );
    public List<SrcOp> Atom(GrammarAST label, bool invert)
    {

        List<SrcOp> omos = null;
        GrammarAST WILDCARD22 = null;
        GrammarAST WILDCARD23 = null;
        List<SrcOp> a = null;
        List<SrcOp> range21 = null;
        List<SrcOp> terminal24 = null;
        List<SrcOp> ruleref25 = null;
        List<SrcOp> blockSet26 = null;

        try
        {
            // org\\antlr\\v4\\codegen\\SourceGenTriggers.g:161:2: ( ^( NOT a= atom[$label, true] ) | range[label] | ^( DOT ID terminal[$label] ) | ^( DOT ID ruleref[$label] ) | ^( WILDCARD . ) | WILDCARD | terminal[label] | ruleref[label] | blockSet[$label, invert] )
            int alt13 = 9;
            switch (input.LA(1))
            {
                case NOT:
                    {
                        alt13 = 1;
                    }
                    break;
                case RANGE:
                    {
                        alt13 = 2;
                    }
                    break;
                case DOT:
                    {
                        int LA13_3 = input.LA(2);
                        if ((LA13_3 == DOWN))
                        {
                            int LA13_8 = input.LA(3);
                            if ((LA13_8 == ID))
                            {
                                int LA13_11 = input.LA(4);
                                if ((LA13_11 == STRING_LITERAL || LA13_11 == TOKEN_REF))
                                {
                                    alt13 = 3;
                                }
                                else if ((LA13_11 == RULE_REF))
                                {
                                    alt13 = 4;
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
                                        var nvae3 =
                                            new NoViableAltException("", 13, 11, input);
                                        throw nvae3;
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
                                    var nvae2 =
                                        new NoViableAltException("", 13, 8, input);
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
                                var nvae4 =
                                    new NoViableAltException("", 13, 3, input);
                                throw nvae4;
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
                        int LA13_4 = input.LA(2);
                        if ((LA13_4 == DOWN))
                        {
                            alt13 = 5;
                        }
                        else if (((LA13_4 >= UP && LA13_4 <= ACTION) || LA13_4 == ASSIGN || LA13_4 == DOT || LA13_4 == NOT || LA13_4 == PLUS_ASSIGN || LA13_4 == RANGE || LA13_4 == RULE_REF || LA13_4 == SEMPRED || LA13_4 == STRING_LITERAL || LA13_4 == TOKEN_REF || (LA13_4 >= BLOCK && LA13_4 <= CLOSURE) || (LA13_4 >= OPTIONAL && LA13_4 <= POSITIVE_CLOSURE) || (LA13_4 >= SET && LA13_4 <= WILDCARD)))
                        {
                            alt13 = 6;
                        }

                        else
                        {
                            int nvaeMark = input.Mark();
                            try
                            {
                                input.Consume();
                                var nvae5 =
                                    new NoViableAltException("", 13, 4, input);
                                throw nvae5;
                            }
                            finally
                            {
                                input.Rewind(nvaeMark);
                            }
                        }

                    }
                    break;
                case STRING_LITERAL:
                case TOKEN_REF:
                    {
                        alt13 = 7;
                    }
                    break;
                case RULE_REF:
                    {
                        alt13 = 8;
                    }
                    break;
                case SET:
                    {
                        alt13 = 9;
                    }
                    break;
                default:
                    var nvae =
                        new NoViableAltException("", 13, 0, input);
                    throw nvae;
            }
            switch (alt13)
            {
                case 1:
                    // org\\antlr\\v4\\codegen\\SourceGenTriggers.g:161:4: ^( NOT a= atom[$label, true] )
                    {
                        Match(input, NOT, FOLLOW_NOT_in_atom538);
                        Match(input, Token.DOWN, null);
                        PushFollow(FOLLOW_atom_in_atom542);
                        a = Atom(label, true);
                        state._fsp--;

                        Match(input, Token.UP, null);

                        omos = a;
                    }
                    break;
                case 2:
                    // org\\antlr\\v4\\codegen\\SourceGenTriggers.g:162:4: range[label]
                    {
                        PushFollow(FOLLOW_range_in_atom552);
                        range21 = Range(label);
                        state._fsp--;

                        omos = range21;
                    }
                    break;
                case 3:
                    // org\\antlr\\v4\\codegen\\SourceGenTriggers.g:163:4: ^( DOT ID terminal[$label] )
                    {
                        Match(input, DOT, FOLLOW_DOT_in_atom567);
                        Match(input, Token.DOWN, null);
                        Match(input, ID, FOLLOW_ID_in_atom569);
                        PushFollow(FOLLOW_terminal_in_atom571);
                        Terminal(label);
                        state._fsp--;

                        Match(input, Token.UP, null);

                    }
                    break;
                case 4:
                    // org\\antlr\\v4\\codegen\\SourceGenTriggers.g:164:4: ^( DOT ID ruleref[$label] )
                    {
                        Match(input, DOT, FOLLOW_DOT_in_atom579);
                        Match(input, Token.DOWN, null);
                        Match(input, ID, FOLLOW_ID_in_atom581);
                        PushFollow(FOLLOW_ruleref_in_atom583);
                        Ruleref(label);
                        state._fsp--;

                        Match(input, Token.UP, null);

                    }
                    break;
                case 5:
                    // org\\antlr\\v4\\codegen\\SourceGenTriggers.g:165:7: ^( WILDCARD . )
                    {
                        WILDCARD22 = (GrammarAST)Match(input, WILDCARD, FOLLOW_WILDCARD_in_atom594);
                        Match(input, Token.DOWN, null);
                        MatchAny(input);
                        Match(input, Token.UP, null);

                        omos = controller.Wildcard(WILDCARD22, label);
                    }
                    break;
                case 6:
                    // org\\antlr\\v4\\codegen\\SourceGenTriggers.g:166:7: WILDCARD
                    {
                        WILDCARD23 = (GrammarAST)Match(input, WILDCARD, FOLLOW_WILDCARD_in_atom613);
                        omos = controller.Wildcard(WILDCARD23, label);
                    }
                    break;
                case 7:
                    // org\\antlr\\v4\\codegen\\SourceGenTriggers.g:167:9: terminal[label]
                    {
                        PushFollow(FOLLOW_terminal_in_atom632);
                        terminal24 = Terminal(label);
                        state._fsp--;

                        omos = terminal24;
                    }
                    break;
                case 8:
                    // org\\antlr\\v4\\codegen\\SourceGenTriggers.g:168:9: ruleref[label]
                    {
                        PushFollow(FOLLOW_ruleref_in_atom649);
                        ruleref25 = Ruleref(label);
                        state._fsp--;

                        omos = ruleref25;
                    }
                    break;
                case 9:
                    // org\\antlr\\v4\\codegen\\SourceGenTriggers.g:169:4: blockSet[$label, invert]
                    {
                        PushFollow(FOLLOW_blockSet_in_atom661);
                        blockSet26 = BlockSet(label, invert);
                        state._fsp--;

                        omos = blockSet26;
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
        return omos;
    }
    // $ANTLR end "atom"



    // $ANTLR start "ruleref"
    // org\\antlr\\v4\\codegen\\SourceGenTriggers.g:172:1: ruleref[GrammarAST label] returns [List<SrcOp> omos] : ^( RULE_REF ( ARG_ACTION )? ( elementOptions )? ) ;
    public List<SrcOp> Ruleref(GrammarAST label)
    {

        List<SrcOp> omos = null;
        GrammarAST RULE_REF27 = null;
        GrammarAST ARG_ACTION28 = null;

        try
        {
            // org\\antlr\\v4\\codegen\\SourceGenTriggers.g:173:5: ( ^( RULE_REF ( ARG_ACTION )? ( elementOptions )? ) )
            // org\\antlr\\v4\\codegen\\SourceGenTriggers.g:173:7: ^( RULE_REF ( ARG_ACTION )? ( elementOptions )? )
            {
                RULE_REF27 = (GrammarAST)Match(input, RULE_REF, FOLLOW_RULE_REF_in_ruleref685);
                if (input.LA(1) == Token.DOWN)
                {
                    Match(input, Token.DOWN, null);
                    // org\\antlr\\v4\\codegen\\SourceGenTriggers.g:173:18: ( ARG_ACTION )?
                    int alt14 = 2;
                    int LA14_0 = input.LA(1);
                    if ((LA14_0 == ARG_ACTION))
                    {
                        alt14 = 1;
                    }
                    switch (alt14)
                    {
                        case 1:
                            // org\\antlr\\v4\\codegen\\SourceGenTriggers.g:173:18: ARG_ACTION
                            {
                                ARG_ACTION28 = (GrammarAST)Match(input, ARG_ACTION, FOLLOW_ARG_ACTION_in_ruleref687);
                            }
                            break;

                    }

                    // org\\antlr\\v4\\codegen\\SourceGenTriggers.g:173:30: ( elementOptions )?
                    int alt15 = 2;
                    int LA15_0 = input.LA(1);
                    if ((LA15_0 == ELEMENT_OPTIONS))
                    {
                        alt15 = 1;
                    }
                    switch (alt15)
                    {
                        case 1:
                            // org\\antlr\\v4\\codegen\\SourceGenTriggers.g:173:30: elementOptions
                            {
                                PushFollow(FOLLOW_elementOptions_in_ruleref690);
                                ElementOptions();
                                state._fsp--;

                            }
                            break;

                    }

                    Match(input, Token.UP, null);
                }

                omos = controller.RuleRef(RULE_REF27, label, ARG_ACTION28);
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
        return omos;
    }
    // $ANTLR end "ruleref"



    // $ANTLR start "range"
    // org\\antlr\\v4\\codegen\\SourceGenTriggers.g:176:1: range[GrammarAST label] returns [List<SrcOp> omos] : ^( RANGE a= STRING_LITERAL b= STRING_LITERAL ) ;
    public List<SrcOp> Range(GrammarAST label)
    {

        List<SrcOp> omos = null;
        GrammarAST a = null;
        GrammarAST b = null;

        try
        {
            // org\\antlr\\v4\\codegen\\SourceGenTriggers.g:177:5: ( ^( RANGE a= STRING_LITERAL b= STRING_LITERAL ) )
            // org\\antlr\\v4\\codegen\\SourceGenTriggers.g:177:7: ^( RANGE a= STRING_LITERAL b= STRING_LITERAL )
            {
                Match(input, RANGE, FOLLOW_RANGE_in_range718);
                Match(input, Token.DOWN, null);
                a = (GrammarAST)Match(input, STRING_LITERAL, FOLLOW_STRING_LITERAL_in_range722);
                b = (GrammarAST)Match(input, STRING_LITERAL, FOLLOW_STRING_LITERAL_in_range726);
                Match(input, Token.UP, null);

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
        return omos;
    }
    // $ANTLR end "range"



    // $ANTLR start "terminal"
    // org\\antlr\\v4\\codegen\\SourceGenTriggers.g:180:1: terminal[GrammarAST label] returns [List<SrcOp> omos] : ( ^( STRING_LITERAL . ) | STRING_LITERAL | ^( TOKEN_REF ARG_ACTION . ) | ^( TOKEN_REF . ) | TOKEN_REF );
    public List<SrcOp> Terminal(GrammarAST label)
    {
        List<SrcOp> omos = null;
        GrammarAST STRING_LITERAL29 = null;
        GrammarAST STRING_LITERAL30 = null;
        GrammarAST TOKEN_REF31 = null;
        GrammarAST ARG_ACTION32 = null;
        GrammarAST TOKEN_REF33 = null;
        GrammarAST TOKEN_REF34 = null;

        try
        {
            // org\\antlr\\v4\\codegen\\SourceGenTriggers.g:181:5: ( ^( STRING_LITERAL . ) | STRING_LITERAL | ^( TOKEN_REF ARG_ACTION . ) | ^( TOKEN_REF . ) | TOKEN_REF )
            int alt16 = 5;
            int LA16_0 = input.LA(1);
            if ((LA16_0 == STRING_LITERAL))
            {
                int LA16_1 = input.LA(2);
                if ((LA16_1 == DOWN))
                {
                    alt16 = 1;
                }
                else if (((LA16_1 >= UP && LA16_1 <= ACTION) || LA16_1 == ASSIGN || LA16_1 == DOT || LA16_1 == NOT || LA16_1 == PLUS_ASSIGN || LA16_1 == RANGE || LA16_1 == RULE_REF || LA16_1 == SEMPRED || LA16_1 == STRING_LITERAL || LA16_1 == TOKEN_REF || (LA16_1 >= BLOCK && LA16_1 <= CLOSURE) || (LA16_1 >= OPTIONAL && LA16_1 <= POSITIVE_CLOSURE) || (LA16_1 >= SET && LA16_1 <= WILDCARD)))
                {
                    alt16 = 2;
                }

                else
                {
                    int nvaeMark = input.Mark();
                    try
                    {
                        input.Consume();
                        var nvae =
                            new NoViableAltException("", 16, 1, input);
                        throw nvae;
                    }
                    finally
                    {
                        input.Rewind(nvaeMark);
                    }
                }

            }
            else if ((LA16_0 == TOKEN_REF))
            {
                int LA16_2 = input.LA(2);
                if ((LA16_2 == DOWN))
                {
                    int LA16_5 = input.LA(3);
                    if ((LA16_5 == ARG_ACTION))
                    {
                        int LA16_7 = input.LA(4);
                        if (((LA16_7 >= ACTION && LA16_7 <= WILDCARD)))
                        {
                            alt16 = 3;
                        }
                        else if (((LA16_7 >= DOWN && LA16_7 <= UP)))
                        {
                            alt16 = 4;
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
                                var nvae =
                                    new NoViableAltException("", 16, 7, input);
                                throw nvae;
                            }
                            finally
                            {
                                input.Rewind(nvaeMark);
                            }
                        }

                    }
                    else if (((LA16_5 >= ACTION && LA16_5 <= ACTION_STRING_LITERAL) || (LA16_5 >= ARG_OR_CHARSET && LA16_5 <= WILDCARD)))
                    {
                        alt16 = 4;
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
                            var nvae =
                                new NoViableAltException("", 16, 5, input);
                            throw nvae;
                        }
                        finally
                        {
                            input.Rewind(nvaeMark);
                        }
                    }

                }
                else if (((LA16_2 >= UP && LA16_2 <= ACTION) || LA16_2 == ASSIGN || LA16_2 == DOT || LA16_2 == NOT || LA16_2 == PLUS_ASSIGN || LA16_2 == RANGE || LA16_2 == RULE_REF || LA16_2 == SEMPRED || LA16_2 == STRING_LITERAL || LA16_2 == TOKEN_REF || (LA16_2 >= BLOCK && LA16_2 <= CLOSURE) || (LA16_2 >= OPTIONAL && LA16_2 <= POSITIVE_CLOSURE) || (LA16_2 >= SET && LA16_2 <= WILDCARD)))
                {
                    alt16 = 5;
                }

                else
                {
                    int nvaeMark = input.Mark();
                    try
                    {
                        input.Consume();
                        var nvae =
                            new NoViableAltException("", 16, 2, input);
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
                var nvae =
                    new NoViableAltException("", 16, 0, input);
                throw nvae;
            }

            switch (alt16)
            {
                case 1:
                    // org\\antlr\\v4\\codegen\\SourceGenTriggers.g:181:8: ^( STRING_LITERAL . )
                    {
                        STRING_LITERAL29 = (GrammarAST)Match(input, STRING_LITERAL, FOLLOW_STRING_LITERAL_in_terminal751);
                        Match(input, Token.DOWN, null);
                        MatchAny(input);
                        Match(input, Token.UP, null);

                        omos = controller.StringRef(STRING_LITERAL29, label);
                    }
                    break;
                case 2:
                    // org\\antlr\\v4\\codegen\\SourceGenTriggers.g:182:7: STRING_LITERAL
                    {
                        STRING_LITERAL30 = (GrammarAST)Match(input, STRING_LITERAL, FOLLOW_STRING_LITERAL_in_terminal766);
                        omos = controller.StringRef(STRING_LITERAL30, label);
                    }
                    break;
                case 3:
                    // org\\antlr\\v4\\codegen\\SourceGenTriggers.g:183:7: ^( TOKEN_REF ARG_ACTION . )
                    {
                        TOKEN_REF31 = (GrammarAST)Match(input, TOKEN_REF, FOLLOW_TOKEN_REF_in_terminal780);
                        Match(input, Token.DOWN, null);
                        ARG_ACTION32 = (GrammarAST)Match(input, ARG_ACTION, FOLLOW_ARG_ACTION_in_terminal782);
                        MatchAny(input);
                        Match(input, Token.UP, null);

                        omos = controller.TokenRef(TOKEN_REF31, label, ARG_ACTION32);
                    }
                    break;
                case 4:
                    // org\\antlr\\v4\\codegen\\SourceGenTriggers.g:184:7: ^( TOKEN_REF . )
                    {
                        TOKEN_REF33 = (GrammarAST)Match(input, TOKEN_REF, FOLLOW_TOKEN_REF_in_terminal796);
                        Match(input, Token.DOWN, null);
                        MatchAny(input);
                        Match(input, Token.UP, null);

                        omos = controller.TokenRef(TOKEN_REF33, label, null);
                    }
                    break;
                case 5:
                    // org\\antlr\\v4\\codegen\\SourceGenTriggers.g:185:7: TOKEN_REF
                    {
                        TOKEN_REF34 = (GrammarAST)Match(input, TOKEN_REF, FOLLOW_TOKEN_REF_in_terminal812);
                        omos = controller.TokenRef(TOKEN_REF34, label, null);
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
        return omos;
    }
    // $ANTLR end "terminal"



    // $ANTLR start "elementOptions"
    // org\\antlr\\v4\\codegen\\SourceGenTriggers.g:188:1: elementOptions : ^( ELEMENT_OPTIONS ( elementOption )+ ) ;
    public void ElementOptions()
    {
        try
        {
            // org\\antlr\\v4\\codegen\\SourceGenTriggers.g:189:5: ( ^( ELEMENT_OPTIONS ( elementOption )+ ) )
            // org\\antlr\\v4\\codegen\\SourceGenTriggers.g:189:7: ^( ELEMENT_OPTIONS ( elementOption )+ )
            {
                Match(input, ELEMENT_OPTIONS, FOLLOW_ELEMENT_OPTIONS_in_elementOptions836);
                Match(input, Token.DOWN, null);
                // org\\antlr\\v4\\codegen\\SourceGenTriggers.g:189:25: ( elementOption )+
                int cnt17 = 0;
            loop17:
                while (true)
                {
                    int alt17 = 2;
                    int LA17_0 = input.LA(1);
                    if ((LA17_0 == ASSIGN || LA17_0 == ID))
                    {
                        alt17 = 1;
                    }

                    switch (alt17)
                    {
                        case 1:
                            // org\\antlr\\v4\\codegen\\SourceGenTriggers.g:189:25: elementOption
                            {
                                PushFollow(FOLLOW_elementOption_in_elementOptions838);
                                ElementOption();
                                state._fsp--;

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
    // org\\antlr\\v4\\codegen\\SourceGenTriggers.g:192:1: elementOption : ( ID | ^( ASSIGN ID ID ) | ^( ASSIGN ID STRING_LITERAL ) | ^( ASSIGN ID ACTION ) | ^( ASSIGN ID INT ) );
    public void ElementOption()
    {
        try
        {
            // org\\antlr\\v4\\codegen\\SourceGenTriggers.g:193:5: ( ID | ^( ASSIGN ID ID ) | ^( ASSIGN ID STRING_LITERAL ) | ^( ASSIGN ID ACTION ) | ^( ASSIGN ID INT ) )
            int alt18 = 5;
            int LA18_0 = input.LA(1);
            if ((LA18_0 == ID))
            {
                alt18 = 1;
            }
            else if ((LA18_0 == ASSIGN))
            {
                int LA18_2 = input.LA(2);
                if ((LA18_2 == DOWN))
                {
                    int LA18_3 = input.LA(3);
                    if ((LA18_3 == ID))
                    {
                        switch (input.LA(4))
                        {
                            case ID:
                                {
                                    alt18 = 2;
                                }
                                break;
                            case STRING_LITERAL:
                                {
                                    alt18 = 3;
                                }
                                break;
                            case ACTION:
                                {
                                    alt18 = 4;
                                }
                                break;
                            case INT:
                                {
                                    alt18 = 5;
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
                                    var nvae =
                                        new NoViableAltException("", 18, 4, input);
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
                            var nvae =
                                new NoViableAltException("", 18, 3, input);
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
                        var nvae =
                            new NoViableAltException("", 18, 2, input);
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
                var nvae =
                    new NoViableAltException("", 18, 0, input);
                throw nvae;
            }

            switch (alt18)
            {
                case 1:
                    // org\\antlr\\v4\\codegen\\SourceGenTriggers.g:193:7: ID
                    {
                        Match(input, ID, FOLLOW_ID_in_elementOption857);
                    }
                    break;
                case 2:
                    // org\\antlr\\v4\\codegen\\SourceGenTriggers.g:194:9: ^( ASSIGN ID ID )
                    {
                        Match(input, ASSIGN, FOLLOW_ASSIGN_in_elementOption868);
                        Match(input, Token.DOWN, null);
                        Match(input, ID, FOLLOW_ID_in_elementOption870);
                        Match(input, ID, FOLLOW_ID_in_elementOption872);
                        Match(input, Token.UP, null);

                    }
                    break;
                case 3:
                    // org\\antlr\\v4\\codegen\\SourceGenTriggers.g:195:9: ^( ASSIGN ID STRING_LITERAL )
                    {
                        Match(input, ASSIGN, FOLLOW_ASSIGN_in_elementOption884);
                        Match(input, Token.DOWN, null);
                        Match(input, ID, FOLLOW_ID_in_elementOption886);
                        Match(input, STRING_LITERAL, FOLLOW_STRING_LITERAL_in_elementOption888);
                        Match(input, Token.UP, null);

                    }
                    break;
                case 4:
                    // org\\antlr\\v4\\codegen\\SourceGenTriggers.g:196:9: ^( ASSIGN ID ACTION )
                    {
                        Match(input, ASSIGN, FOLLOW_ASSIGN_in_elementOption900);
                        Match(input, Token.DOWN, null);
                        Match(input, ID, FOLLOW_ID_in_elementOption902);
                        Match(input, ACTION, FOLLOW_ACTION_in_elementOption904);
                        Match(input, Token.UP, null);

                    }
                    break;
                case 5:
                    // org\\antlr\\v4\\codegen\\SourceGenTriggers.g:197:9: ^( ASSIGN ID INT )
                    {
                        Match(input, ASSIGN, FOLLOW_ASSIGN_in_elementOption916);
                        Match(input, Token.DOWN, null);
                        Match(input, ID, FOLLOW_ID_in_elementOption918);
                        Match(input, INT, FOLLOW_INT_in_elementOption920);
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

    static readonly short[] DFA7_eot = RuntimeUtils.Convert(new char[] { '\u0014', '\uffff' });
    static readonly short[] DFA7_eof = RuntimeUtils.Convert(new char[] { '\u0014', '\uffff' });
    static readonly char[] DFA7_min = new char[] { '\u0001', '\u0045', '\u0001', '\u0002', '\u0001', '\u0004', '\u0001', '\u0002', '\u0002', '\uffff', '\u0001', '\u000a', '\u0001', '\u0003', '\u0001', '\u0002', '\u0001', '\u0004', '\u0001', '\u001c', '\u0001', '\u0004', '\u0008', '\u0003' };
    static readonly char[] DFA7_max = new char[] { '\u0001', '\u0045', '\u0001', '\u0002', '\u0001', '\u0053', '\u0001', '\u0002', '\u0002', '\uffff', '\u0002', '\u001c', '\u0001', '\u0002', '\u0001', '\u0053', '\u0001', '\u001c', '\u0001', '\u003b', '\u0004', '\u0003', '\u0004', '\u001c' };
    static readonly short[] DFA7_accept = RuntimeUtils.Convert(new char[] { '\u0004', '\uffff', '\u0001', '\u0001', '\u0001', '\u0002', '\u000e', '\uffff' });
    static readonly short[] DFA7_special = RuntimeUtils.Convert(new char[] { '\u0014', '\uffff', '\u007d', '\u003e' });
    static readonly short[][] DFA7_transition = new short[][]{
    RuntimeUtils.Convert(new char[] {'\u0001','\u0001'}),
    RuntimeUtils.Convert(new char[] {'\u0001','\u0002'}),
    RuntimeUtils.Convert(new char[] {'\u0001','\u0004','\u0005','\uffff','\u0001','\u0004','\u0009','\uffff','\u0001','\u0004','\u0012','\uffff','\u0001','\u0004','\u0006','\uffff','\u0001','\u0004','\u0002','\uffff','\u0001','\u0004','\u0004','\uffff','\u0001','\u0004','\u0001','\uffff','\u0001','\u0004','\u0002','\uffff','\u0001','\u0004','\u0002','\uffff','\u0001','\u0004','\u0007','\uffff','\u0002','\u0004','\u0001','\uffff','\u0001','\u0003','\u0001','\u0005','\u0002','\uffff','\u0002','\u0004','\u0003','\uffff','\u0002','\u0004'}),
    RuntimeUtils.Convert(new char[] {'\u0001','\u0006'}),
    RuntimeUtils.Convert(new char[0]),
    RuntimeUtils.Convert(new char[0]),
    RuntimeUtils.Convert(new char[] {'\u0001','\u0008','\u0011','\uffff','\u0001','\u0007'}),
    RuntimeUtils.Convert(new char[] {'\u0001','\u0009','\u0006','\uffff','\u0001','\u0008','\u0011','\uffff','\u0001','\u0007'}),
    RuntimeUtils.Convert(new char[] {'\u0001','\u000a'}),
    RuntimeUtils.Convert(new char[] {'\u0001','\u0004','\u0005','\uffff','\u0001','\u0004','\u0009','\uffff','\u0001','\u0004','\u0012','\uffff','\u0001','\u0004','\u0006','\uffff','\u0001','\u0004','\u0002','\uffff','\u0001','\u0004','\u0004','\uffff','\u0001','\u0004','\u0001','\uffff','\u0001','\u0004','\u0002','\uffff','\u0001','\u0004','\u0002','\uffff','\u0001','\u0004','\u0007','\uffff','\u0002','\u0004','\u0002','\uffff','\u0001','\u0005','\u0002','\uffff','\u0002','\u0004','\u0003','\uffff','\u0002','\u0004'}),
    RuntimeUtils.Convert(new char[] {'\u0001','\u000b'}),
    RuntimeUtils.Convert(new char[] {'\u0001','\u000e','\u0017','\uffff','\u0001','\u000c','\u0001','\uffff','\u0001','\u000f','\u001c','\uffff','\u0001','\u000d'}),
    RuntimeUtils.Convert(new char[] {'\u0001','\u0010'}),
    RuntimeUtils.Convert(new char[] {'\u0001','\u0011'}),
    RuntimeUtils.Convert(new char[] {'\u0001','\u0012'}),
    RuntimeUtils.Convert(new char[] {'\u0001','\u0013'}),
    RuntimeUtils.Convert(new char[] {'\u0001','\u0009','\u0006','\uffff','\u0001','\u0008','\u0011','\uffff','\u0001','\u0007'}),
    RuntimeUtils.Convert(new char[] {'\u0001','\u0009','\u0006','\uffff','\u0001','\u0008','\u0011','\uffff','\u0001','\u0007'}),
    RuntimeUtils.Convert(new char[] {'\u0001','\u0009','\u0006','\uffff','\u0001','\u0008','\u0011','\uffff','\u0001','\u0007'}),
    RuntimeUtils.Convert(new char[] {'\u0001','\u0009','\u0006','\uffff','\u0001','\u0008','\u0011','\uffff','\u0001','\u0007'})
    };


    protected class DFA7 : antlr.runtime.DFA
    {

        public DFA7(BaseRecognizer recognizer)
        {
            this.recognizer = recognizer;
            this.decisionNumber = 7;
            this.eot = DFA7_eot;
            this.eof = DFA7_eof;
            this.min = DFA7_min;
            this.max = DFA7_max;
            this.accept = DFA7_accept;
            this.special = DFA7_special;
            this.transition = DFA7_transition;
        }
        //@Override
        public override string Description => "89:1: alt[bool outerMost] returns [CodeBlockForAlt altCodeBlock, List<SrcOp> ops] : ( ^( ALT ( elementOptions )? ( element )+ ) | ^( ALT ( elementOptions )? EPSILON ) );";
    }

    public static readonly BitSet FOLLOW_block_in_dummy61 = new (new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_BLOCK_in_block84 = new (new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_OPTIONS_in_block88 = new(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_alternative_in_block109 = new(new long[] { 0x0000000000000008L, 0x0000000000000020L });
    public static readonly BitSet FOLLOW_alt_in_alternative161 = new(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_ALT_in_alt191 = new(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_elementOptions_in_alt193 = new(new long[] { 0x4942408000100410L, 0x00000000000C60C0L });
    public static readonly BitSet FOLLOW_element_in_alt198 = new(new long[] { 0x4942408000100418L, 0x00000000000C60C0L });
    public static readonly BitSet FOLLOW_ALT_in_alt212 = new(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_elementOptions_in_alt214 = new(new long[] { 0x0000000000000000L, 0x0000000000000400L });
    public static readonly BitSet FOLLOW_EPSILON_in_alt217 = new(new long[] { 0x0000000000000008L });
    public static readonly BitSet FOLLOW_labeledElement_in_element246 = new(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_atom_in_element257 = new(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_subrule_in_element267 = new(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_ACTION_in_element282 = new(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_SEMPRED_in_element297 = new(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_ACTION_in_element311 = new(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_elementOptions_in_element313 = new(new long[] { 0x0000000000000008L });
    public static readonly BitSet FOLLOW_SEMPRED_in_element325 = new(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_elementOptions_in_element327 = new(new long[] { 0x0000000000000008L });
    public static readonly BitSet FOLLOW_ASSIGN_in_labeledElement347 = new(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_ID_in_labeledElement349 = new(new long[] { 0x4842008000100000L, 0x00000000000C0000L });
    public static readonly BitSet FOLLOW_atom_in_labeledElement351 = new(new long[] { 0x0000000000000008L });
    public static readonly BitSet FOLLOW_PLUS_ASSIGN_in_labeledElement364 = new(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_ID_in_labeledElement366 = new(new long[] { 0x4842008000100000L, 0x00000000000C0000L });
    public static readonly BitSet FOLLOW_atom_in_labeledElement368 = new(new long[] { 0x0000000000000008L });
    public static readonly BitSet FOLLOW_ASSIGN_in_labeledElement379 = new(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_ID_in_labeledElement381 = new(new long[] { 0x0000000000000000L, 0x0000000000000040L });
    public static readonly BitSet FOLLOW_block_in_labeledElement383 = new(new long[] { 0x0000000000000008L });
    public static readonly BitSet FOLLOW_PLUS_ASSIGN_in_labeledElement396 = new(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_ID_in_labeledElement398 = new(new long[] { 0x0000000000000000L, 0x0000000000000040L });
    public static readonly BitSet FOLLOW_block_in_labeledElement400 = new(new long[] { 0x0000000000000008L });
    public static readonly BitSet FOLLOW_OPTIONAL_in_subrule421 = new(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_block_in_subrule425 = new(new long[] { 0x0000000000000008L });
    public static readonly BitSet FOLLOW_CLOSURE_in_subrule441 = new(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_block_in_subrule445 = new(new long[] { 0x0000000000000008L });
    public static readonly BitSet FOLLOW_POSITIVE_CLOSURE_in_subrule456 = new(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_block_in_subrule460 = new(new long[] { 0x0000000000000008L });
    public static readonly BitSet FOLLOW_block_in_subrule476 = new(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_SET_in_blockSet506 = new(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_atom_in_blockSet508 = new(new long[] { 0x4842008000100008L, 0x00000000000C0000L });
    public static readonly BitSet FOLLOW_NOT_in_atom538 = new(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_atom_in_atom542 = new(new long[] { 0x0000000000000008L });
    public static readonly BitSet FOLLOW_range_in_atom552 = new(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_DOT_in_atom567 = new(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_ID_in_atom569 = new(new long[] { 0x4800000000000000L });
    public static readonly BitSet FOLLOW_terminal_in_atom571 = new(new long[] { 0x0000000000000008L });
    public static readonly BitSet FOLLOW_DOT_in_atom579 = new(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_ID_in_atom581 = new(new long[] { 0x0040000000000000L });
    public static readonly BitSet FOLLOW_ruleref_in_atom583 = new(new long[] { 0x0000000000000008L });
    public static readonly BitSet FOLLOW_WILDCARD_in_atom594 = new(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_WILDCARD_in_atom613 = new(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_terminal_in_atom632 = new(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_ruleref_in_atom649 = new(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_blockSet_in_atom661 = new(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_RULE_REF_in_ruleref685 = new(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_ARG_ACTION_in_ruleref687 = new(new long[] { 0x0000000000000008L, 0x0000000000000200L });
    public static readonly BitSet FOLLOW_elementOptions_in_ruleref690 = new(new long[] { 0x0000000000000008L });
    public static readonly BitSet FOLLOW_RANGE_in_range718 = new(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_STRING_LITERAL_in_range722 = new(new long[] { 0x0800000000000000L });
    public static readonly BitSet FOLLOW_STRING_LITERAL_in_range726 = new(new long[] { 0x0000000000000008L });
    public static readonly BitSet FOLLOW_STRING_LITERAL_in_terminal751 = new(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_STRING_LITERAL_in_terminal766 = new(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_TOKEN_REF_in_terminal780 = new(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_ARG_ACTION_in_terminal782 = new(new ulong[] { 0xFFFFFFFFFFFFFFF0L, 0x00000000000FFFFFL });
    public static readonly BitSet FOLLOW_TOKEN_REF_in_terminal796 = new(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_TOKEN_REF_in_terminal812 = new(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_ELEMENT_OPTIONS_in_elementOptions836 = new(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_elementOption_in_elementOptions838 = new(new long[] { 0x0000000010000408L });
    public static readonly BitSet FOLLOW_ID_in_elementOption857 = new(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_ASSIGN_in_elementOption868 = new(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_ID_in_elementOption870 = new(new long[] { 0x0000000010000000L });
    public static readonly BitSet FOLLOW_ID_in_elementOption872 = new(new long[] { 0x0000000000000008L });
    public static readonly BitSet FOLLOW_ASSIGN_in_elementOption884 = new(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_ID_in_elementOption886 = new(new long[] { 0x0800000000000000L });
    public static readonly BitSet FOLLOW_STRING_LITERAL_in_elementOption888 = new(new long[] { 0x0000000000000008L });
    public static readonly BitSet FOLLOW_ASSIGN_in_elementOption900 = new(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_ID_in_elementOption902 = new(new long[] { 0x0000000000000010L });
    public static readonly BitSet FOLLOW_ACTION_in_elementOption904 = new(new long[] { 0x0000000000000008L });
    public static readonly BitSet FOLLOW_ASSIGN_in_elementOption916 = new(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_ID_in_elementOption918 = new(new long[] { 0x0000000040000000L });
    public static readonly BitSet FOLLOW_INT_in_elementOption920 = new(new long[] { 0x0000000000000008L });
}
