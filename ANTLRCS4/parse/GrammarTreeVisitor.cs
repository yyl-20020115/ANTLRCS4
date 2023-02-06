// $ANTLR 3.5.3 org\\antlr\\v4\\parse\\g 2023-01-27 22:27:34

/*
 [The "BSD license"]
 Copyright (c) 2011 Terence Parr
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
using org.antlr.runtime.tree;
using org.antlr.v4.runtime;
using org.antlr.v4.runtime.dfa;
using org.antlr.v4.runtime.misc;
using org.antlr.v4.tool;
using org.antlr.v4.tool.ast;
using System.Reflection;

namespace org.antlr.v4.parse;
/** The definitive ANTLR v3 tree grammar to walk/visit ANTLR v4 grammars.
 *  Parses trees created by ANTLRParser.g.
 *
 *  Rather than have multiple tree grammars, one for each visit, I'm
 *  creating this generic visitor that knows about context. All of the
 *  boilerplate pattern recognition is done here. Then, subclasses can
 *  override the methods they care about. This prevents a lot of the same
 *  context tracking stuff like "set current alternative for current
 *  rule node" that is repeated in lots of tree filters.
 */
public class GrammarTreeVisitor : TreeParser
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
        "RULE", "RULEMODIFIERS", "RULES", "SET", "WILDCARD", "PRIVATE", "PROTECTED",
        "PUBLIC"
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
    public const int PRIVATE = 84;
    public const int PROTECTED = 85;
    public const int PUBLIC = 86;

    // delegates
    public TreeParser[] GetDelegates()
    {
        return Array.Empty<TreeParser>();
    }

    // delegators

    protected DFA38 dfa38;

    public GrammarTreeVisitor(TreeNodeStream input)
        : this(input, new RecognizerSharedState())
    {
        ;
    }
    public GrammarTreeVisitor(TreeNodeStream input, RecognizerSharedState state)
        : base(input, state)
    {
        dfa38 = new DFA38(this);
    }

    //@Override 
    public override string[] GetTokenNames() => tokenNames;
    //@Override
    public override string GetGrammarFileName() => "org\\antlr\\v4\\parse\\GrammarTreeVisitor.g";

    public string grammarName;
    public GrammarAST currentRuleAST;
    public string currentModeName = LexerGrammar.DEFAULT_MODE_NAME;
    public string currentRuleName;
    public GrammarAST currentOuterAltRoot;
    public int currentOuterAltNumber = 1; // 1..n
    public int rewriteEBNFLevel = 0;

    public GrammarTreeVisitor() : this(null) { }

    // Should be abstract but can't make gen'd parser abstract;
    // subclasses should implement else everything goes to stderr!
    public virtual ErrorManager GetErrorManager() { return null; }

    public void visitGrammar(GrammarAST t) { visit(t, "grammarSpec"); }
    public void visit(GrammarAST t, String ruleName)
    {
        var nodes = new CommonTreeNodeStream(new GrammarASTAdaptor(), t);
        setTreeNodeStream(nodes);
        try
        {
            var m = this.GetType().GetMethod(ruleName);
            m.Invoke(this, new object[] {  });
        }
        catch (Exception e)
        {
            var errMgr = GetErrorManager();
            if (e is TargetInvocationException)
            {
                e = e.InnerException;
            }
            //e.printStackTrace(System.err);
            if (errMgr == null)
            {
                Console.Error.WriteLine("can't find rule " + ruleName +
                                   " or tree structure error: " + t.toStringTree()
                                   );
                //e.printStackTrace(System.err);
            }
            else errMgr.toolError(ErrorType.INTERNAL_ERROR, e);
        }
    }

    public void discoverGrammar(GrammarRootAST root, GrammarAST ID) { }
    public void finishPrequels(GrammarAST firstPrequel) { }
    public void finishGrammar(GrammarRootAST root, GrammarAST ID) { }

    public void grammarOption(GrammarAST ID, GrammarAST valueAST) { }
    public void ruleOption(GrammarAST ID, GrammarAST valueAST) { }
    public void blockOption(GrammarAST ID, GrammarAST valueAST) { }
    public void defineToken(GrammarAST ID) { }
    public void defineChannel(GrammarAST ID) { }
    public void globalNamedAction(GrammarAST scope, GrammarAST ID, ActionAST action) { }
    public void importGrammar(GrammarAST label, GrammarAST ID) { }

    public void modeDef(GrammarAST m, GrammarAST ID) { }

    public void discoverRules(GrammarAST rules) { }
    public void finishRules(GrammarAST rule) { }
    public void discoverRule(RuleAST rule, GrammarAST ID, List<GrammarAST> modifiers,
                             ActionAST arg, ActionAST returns, GrammarAST thrws,
                             GrammarAST options, ActionAST locals,
                             List<GrammarAST> actions,
                             GrammarAST block)
    { }
    public void finishRule(RuleAST rule, GrammarAST ID, GrammarAST block) { }
    public void discoverLexerRule(RuleAST rule, GrammarAST ID, List<GrammarAST> modifiers, GrammarAST options,
                                  GrammarAST block)
    { }
    public void finishLexerRule(RuleAST rule, GrammarAST ID, GrammarAST block) { }
    public void ruleCatch(GrammarAST arg, ActionAST action) { }
    public void finallyAction(ActionAST action) { }
    public void discoverOuterAlt(AltAST alt) { }
    public void finishOuterAlt(AltAST alt) { }
    public void discoverAlt(AltAST alt) { }
    public void finishAlt(AltAST alt) { }

    public void ruleRef(GrammarAST @ref, ActionAST arg) { }
    public void tokenRef(TerminalAST @ref) { }
    public void elementOption(GrammarASTWithOptions t, GrammarAST ID, GrammarAST valueAST) { }
    public void stringRef(TerminalAST @ref) { }
    public void wildcardRef(GrammarAST @ref) { }
    public void actionInAlt(ActionAST action) { }
    public void sempredInAlt(PredAST pred) { }
    public void label(GrammarAST op, GrammarAST ID, GrammarAST element) { }
    public void lexerCallCommand(int outerAltNumber, GrammarAST ID, GrammarAST arg) { }
    public void lexerCommand(int outerAltNumber, GrammarAST ID) { }

    protected void enterGrammarSpec(GrammarAST tree) { }
    protected void exitGrammarSpec(GrammarAST tree) { }

    protected void enterPrequelConstructs(GrammarAST tree) { }
    protected void exitPrequelConstructs(GrammarAST tree) { }

    protected void enterPrequelConstruct(GrammarAST tree) { }
    protected void exitPrequelConstruct(GrammarAST tree) { }

    protected void enterOptionsSpec(GrammarAST tree) { }
    protected void exitOptionsSpec(GrammarAST tree) { }

    protected void enterOption(GrammarAST tree) { }
    protected void exitOption(GrammarAST tree) { }

    protected void enterOptionValue(GrammarAST tree) { }
    protected void exitOptionValue(GrammarAST tree) { }

    protected void enterDelegateGrammars(GrammarAST tree) { }
    protected void exitDelegateGrammars(GrammarAST tree) { }

    protected void enterDelegateGrammar(GrammarAST tree) { }
    protected void exitDelegateGrammar(GrammarAST tree) { }

    protected void enterTokensSpec(GrammarAST tree) { }
    protected void exitTokensSpec(GrammarAST tree) { }

    protected void enterTokenSpec(GrammarAST tree) { }
    protected void exitTokenSpec(GrammarAST tree) { }

    protected void enterChannelsSpec(GrammarAST tree) { }
    protected void exitChannelsSpec(GrammarAST tree) { }

    protected void enterChannelSpec(GrammarAST tree) { }
    protected void exitChannelSpec(GrammarAST tree) { }

    protected void enterAction(GrammarAST tree) { }
    protected void exitAction(GrammarAST tree) { }

    protected void enterRules(GrammarAST tree) { }
    protected void exitRules(GrammarAST tree) { }

    protected void enterMode(GrammarAST tree) { }
    protected void exitMode(GrammarAST tree) { }

    protected void enterLexerRule(GrammarAST tree) { }
    protected void exitLexerRule(GrammarAST tree) { }

    protected void enterRule(GrammarAST tree) { }
    protected void exitRule(GrammarAST tree) { }

    protected void enterExceptionGroup(GrammarAST tree) { }
    protected void exitExceptionGroup(GrammarAST tree) { }

    protected void enterExceptionHandler(GrammarAST tree) { }
    protected void exitExceptionHandler(GrammarAST tree) { }

    protected void enterFinallyClause(GrammarAST tree) { }
    protected void exitFinallyClause(GrammarAST tree) { }

    protected void enterLocals(GrammarAST tree) { }
    protected void exitLocals(GrammarAST tree) { }

    protected void enterRuleReturns(GrammarAST tree) { }
    protected void exitRuleReturns(GrammarAST tree) { }

    protected void enterThrowsSpec(GrammarAST tree) { }
    protected void exitThrowsSpec(GrammarAST tree) { }

    protected void enterRuleAction(GrammarAST tree) { }
    protected void exitRuleAction(GrammarAST tree) { }

    protected void enterRuleModifier(GrammarAST tree) { }
    protected void exitRuleModifier(GrammarAST tree) { }

    protected void enterLexerRuleBlock(GrammarAST tree) { }
    protected void exitLexerRuleBlock(GrammarAST tree) { }

    protected void enterRuleBlock(GrammarAST tree) { }
    protected void exitRuleBlock(GrammarAST tree) { }

    protected void enterLexerOuterAlternative(AltAST tree) { }
    protected void exitLexerOuterAlternative(AltAST tree) { }

    protected void enterOuterAlternative(AltAST tree) { }
    protected void exitOuterAlternative(AltAST tree) { }

    protected void enterLexerAlternative(GrammarAST tree) { }
    protected void exitLexerAlternative(GrammarAST tree) { }

    protected void enterLexerElements(GrammarAST tree) { }
    protected void exitLexerElements(GrammarAST tree) { }

    protected void enterLexerElement(GrammarAST tree) { }
    protected void exitLexerElement(GrammarAST tree) { }

    protected void enterLexerBlock(GrammarAST tree) { }
    protected void exitLexerBlock(GrammarAST tree) { }

    protected void enterLexerAtom(GrammarAST tree) { }
    protected void exitLexerAtom(GrammarAST tree) { }

    protected void enterActionElement(GrammarAST tree) { }
    protected void exitActionElement(GrammarAST tree) { }

    protected void enterAlternative(AltAST tree) { }
    protected void exitAlternative(AltAST tree) { }

    protected void enterLexerCommand(GrammarAST tree) { }
    protected void exitLexerCommand(GrammarAST tree) { }

    protected void enterLexerCommandExpr(GrammarAST tree) { }
    protected void exitLexerCommandExpr(GrammarAST tree) { }

    protected void enterElement(GrammarAST tree) { }
    protected void exitElement(GrammarAST tree) { }

    protected void enterAstOperand(GrammarAST tree) { }
    protected void exitAstOperand(GrammarAST tree) { }

    protected void enterLabeledElement(GrammarAST tree) { }
    protected void exitLabeledElement(GrammarAST tree) { }

    protected void enterSubrule(GrammarAST tree) { }
    protected void exitSubrule(GrammarAST tree) { }

    protected void enterLexerSubrule(GrammarAST tree) { }
    protected void exitLexerSubrule(GrammarAST tree) { }

    protected void enterBlockSuffix(GrammarAST tree) { }
    protected void exitBlockSuffix(GrammarAST tree) { }

    protected void enterEbnfSuffix(GrammarAST tree) { }
    protected void exitEbnfSuffix(GrammarAST tree) { }

    protected void enterAtom(GrammarAST tree) { }
    protected void exitAtom(GrammarAST tree) { }

    protected void enterBlockSet(GrammarAST tree) { }
    protected void exitBlockSet(GrammarAST tree) { }

    protected void enterSetElement(GrammarAST tree) { }
    protected void exitSetElement(GrammarAST tree) { }

    protected void enterBlock(GrammarAST tree) { }
    protected void exitBlock(GrammarAST tree) { }

    protected void enterRuleref(GrammarAST tree) { }
    protected void exitRuleref(GrammarAST tree) { }

    protected void enterRange(GrammarAST tree) { }
    protected void exitRange(GrammarAST tree) { }

    protected void enterTerminal(GrammarAST tree) { }
    protected void exitTerminal(GrammarAST tree) { }

    protected void enterElementOptions(GrammarAST tree) { }
    protected void exitElementOptions(GrammarAST tree) { }

    protected void enterElementOption(GrammarAST tree) { }
    protected void exitElementOption(GrammarAST tree) { }

    //@Override
    public void traceIn(String ruleName, int ruleIndex)
    {
        Console.Error.WriteLine("enter " + ruleName + ": " + input.LT(1));
    }

    //@Override
    public void traceOut(String ruleName, int ruleIndex)
    {
        Console.Error.WriteLine("exit " + ruleName + ": " + input.LT(1));
    }


    public class grammarSpec_return : TreeRuleReturnScope
    {
    };


    // $ANTLR start "grammarSpec"
    // org\\antlr\\v4\\parse\\GrammarTreeVisitor.g:341:1: grammarSpec : ^( GRAMMAR ID prequelConstructs rules ( mode )* ) ;
    public grammarSpec_return grammarSpec()
    {
        grammarSpec_return retval = new grammarSpec_return();
        retval.start = input.LT(1);

        GrammarAST ID1 = null;
        GrammarAST GRAMMAR2 = null;
        TreeRuleReturnScope prequelConstructs3 = null;


        enterGrammarSpec(((GrammarAST)retval.start));

        try
        {
            // org\\antlr\\v4\\parse\\GrammarTreeVisitor.g:348:5: ( ^( GRAMMAR ID prequelConstructs rules ( mode )* ) )
            // org\\antlr\\v4\\parse\\GrammarTreeVisitor.g:348:9: ^( GRAMMAR ID prequelConstructs rules ( mode )* )
            {
                GRAMMAR2 = (GrammarAST)Match(input, GRAMMAR, FOLLOW_GRAMMAR_in_grammarSpec85);
                Match(input, Token.DOWN, null);
                ID1 = (GrammarAST)Match(input, ID, FOLLOW_ID_in_grammarSpec87);
                grammarName = (ID1 != null ? ID1.getText() : null);
                discoverGrammar((GrammarRootAST)GRAMMAR2, ID1);
                PushFollow(FOLLOW_prequelConstructs_in_grammarSpec106);
                prequelConstructs3 = prequelConstructs();
                state._fsp--;

                finishPrequels((prequelConstructs3 != null ? ((GrammarTreeVisitor.prequelConstructs_return)prequelConstructs3).firstOne : null));
                PushFollow(FOLLOW_rules_in_grammarSpec123);
                rules();
                state._fsp--;

            // org\\antlr\\v4\\parse\\GrammarTreeVisitor.g:352:14: ( mode )*
            loop1:
                while (true)
                {
                    int alt1 = 2;
                    int LA1_0 = input.LA(1);
                    if ((LA1_0 == MODE))
                    {
                        alt1 = 1;
                    }

                    switch (alt1)
                    {
                        case 1:
                            // org\\antlr\\v4\\parse\\GrammarTreeVisitor.g:352:14: mode
                            {
                                PushFollow(FOLLOW_mode_in_grammarSpec125);
                                mode();
                                state._fsp--;

                            }
                            break;

                        default:
                            goto exit1;
                            //break loop1;
                    }
                }
            exit1:
                finishGrammar((GrammarRootAST)GRAMMAR2, ID1);
                Match(input, Token.UP, null);

            }


            exitGrammarSpec(((GrammarAST)retval.start));

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
    // $ANTLR end "grammarSpec"


    public class prequelConstructs_return : TreeRuleReturnScope
    {

        public GrammarAST firstOne = null;
    };


    // $ANTLR start "prequelConstructs"
    // org\\antlr\\v4\\parse\\GrammarTreeVisitor.g:357:1: prequelConstructs returns [GrammarAST firstOne=null] : ( ( prequelConstruct )+ |);
    public prequelConstructs_return prequelConstructs()
    {
        prequelConstructs_return retval = new prequelConstructs_return();
        retval.start = input.LT(1);


        enterPrequelConstructs(((GrammarAST)retval.start));

        try
        {
            // org\\antlr\\v4\\parse\\GrammarTreeVisitor.g:364:2: ( ( prequelConstruct )+ |)
            int alt3 = 2;
            int LA3_0 = input.LA(1);
            if ((LA3_0 == AT || LA3_0 == CHANNELS || LA3_0 == IMPORT || LA3_0 == OPTIONS || LA3_0 == TOKENS_SPEC))
            {
                alt3 = 1;
            }
            else if ((LA3_0 == RULES))
            {
                alt3 = 2;
            }

            else
            {
                NoViableAltException nvae =
                    new NoViableAltException("", 3, 0, input);
                throw nvae;
            }

            switch (alt3)
            {
                case 1:
                    // org\\antlr\\v4\\parse\\GrammarTreeVisitor.g:364:4: ( prequelConstruct )+
                    {
                        retval.firstOne = ((GrammarAST)retval.start);
                        // org\\antlr\\v4\\parse\\GrammarTreeVisitor.g:364:24: ( prequelConstruct )+
                        int cnt2 = 0;
                    loop2:
                        while (true)
                        {
                            int alt2 = 2;
                            int LA2_0 = input.LA(1);
                            if ((LA2_0 == AT || LA2_0 == CHANNELS || LA2_0 == IMPORT || LA2_0 == OPTIONS || LA2_0 == TOKENS_SPEC))
                            {
                                alt2 = 1;
                            }

                            switch (alt2)
                            {
                                case 1:
                                    // org\\antlr\\v4\\parse\\GrammarTreeVisitor.g:364:24: prequelConstruct
                                    {
                                        PushFollow(FOLLOW_prequelConstruct_in_prequelConstructs167);
                                        prequelConstruct();
                                        state._fsp--;

                                    }
                                    break;

                                default:
                                    if (cnt2 >= 1) goto exit2;// break loop2;
                                    EarlyExitException eee = new EarlyExitException(2, input);
                                    throw eee;
                            }
                            cnt2++;
                        }
                    exit2:
                        ;
                    }
                    break;
                case 2:
                    // org\\antlr\\v4\\parse\\GrammarTreeVisitor.g:366:2: 
                    {
                    }
                    break;

            }

            exitPrequelConstructs(((GrammarAST)retval.start));

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
    // $ANTLR end "prequelConstructs"


    public class prequelConstruct_return : TreeRuleReturnScope
    {
    };


    // $ANTLR start "prequelConstruct"
    // org\\antlr\\v4\\parse\\GrammarTreeVisitor.g:368:1: prequelConstruct : ( optionsSpec | delegateGrammars | tokensSpec | channelsSpec | action );
    public prequelConstruct_return prequelConstruct()
    {
        prequelConstruct_return retval = new prequelConstruct_return();
        retval.start = input.LT(1);


        enterPrequelConstructs(((GrammarAST)retval.start));

        try
        {
            // org\\antlr\\v4\\parse\\GrammarTreeVisitor.g:375:2: ( optionsSpec | delegateGrammars | tokensSpec | channelsSpec | action )
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
                    // org\\antlr\\v4\\parse\\GrammarTreeVisitor.g:375:6: optionsSpec
                    {
                        PushFollow(FOLLOW_optionsSpec_in_prequelConstruct194);
                        optionsSpec();
                        state._fsp--;

                    }
                    break;
                case 2:
                    // org\\antlr\\v4\\parse\\GrammarTreeVisitor.g:376:9: delegateGrammars
                    {
                        PushFollow(FOLLOW_delegateGrammars_in_prequelConstruct204);
                        delegateGrammars();
                        state._fsp--;

                    }
                    break;
                case 3:
                    // org\\antlr\\v4\\parse\\GrammarTreeVisitor.g:377:9: tokensSpec
                    {
                        PushFollow(FOLLOW_tokensSpec_in_prequelConstruct214);
                        tokensSpec();
                        state._fsp--;

                    }
                    break;
                case 4:
                    // org\\antlr\\v4\\parse\\GrammarTreeVisitor.g:378:9: channelsSpec
                    {
                        PushFollow(FOLLOW_channelsSpec_in_prequelConstruct224);
                        channelsSpec();
                        state._fsp--;

                    }
                    break;
                case 5:
                    // org\\antlr\\v4\\parse\\GrammarTreeVisitor.g:379:9: action
                    {
                        PushFollow(FOLLOW_action_in_prequelConstruct234);
                        action();
                        state._fsp--;

                    }
                    break;

            }

            exitPrequelConstructs(((GrammarAST)retval.start));

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
    // $ANTLR end "prequelConstruct"


    public class optionsSpec_return : TreeRuleReturnScope
    {
    };


    // $ANTLR start "optionsSpec"
    // org\\antlr\\v4\\parse\\GrammarTreeVisitor.g:382:1: optionsSpec : ^( OPTIONS ( option )* ) ;
    public optionsSpec_return optionsSpec()
    {
        optionsSpec_return retval = new optionsSpec_return();
        retval.start = input.LT(1);


        enterOptionsSpec(((GrammarAST)retval.start));

        try
        {
            // org\\antlr\\v4\\parse\\GrammarTreeVisitor.g:389:2: ( ^( OPTIONS ( option )* ) )
            // org\\antlr\\v4\\parse\\GrammarTreeVisitor.g:389:4: ^( OPTIONS ( option )* )
            {
                Match(input, OPTIONS, FOLLOW_OPTIONS_in_optionsSpec259);
                if (input.LA(1) == Token.DOWN)
                {
                    Match(input, Token.DOWN, null);
                // org\\antlr\\v4\\parse\\GrammarTreeVisitor.g:389:14: ( option )*
                loop5:
                    while (true)
                    {
                        int alt5 = 2;
                        int LA5_0 = input.LA(1);
                        if ((LA5_0 == ASSIGN))
                        {
                            alt5 = 1;
                        }

                        switch (alt5)
                        {
                            case 1:
                                // org\\antlr\\v4\\parse\\GrammarTreeVisitor.g:389:14: option
                                {
                                    PushFollow(FOLLOW_option_in_optionsSpec261);
                                    option();
                                    state._fsp--;

                                }
                                break;

                            default:
                                goto exit5;
                                //break loop5;
                        }
                    }
                exit5:
                    Match(input, Token.UP, null);
                }

            }


            exitOptionsSpec(((GrammarAST)retval.start));

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
    // $ANTLR end "optionsSpec"


    public class option_return : TreeRuleReturnScope
    {
    };


    // $ANTLR start "option"
    // org\\antlr\\v4\\parse\\GrammarTreeVisitor.g:392:1: option : ^(a= ASSIGN ID v= optionValue ) ;
    public option_return option()
    {
        option_return retval = new option_return();
        retval.start = input.LT(1);

        GrammarAST a = null;
        GrammarAST ID4 = null;
        TreeRuleReturnScope v = null;


        enterOption(((GrammarAST)retval.start));
        bool rule = inContext("RULE ...");
        bool block = inContext("BLOCK ...");

        try
        {
            // org\\antlr\\v4\\parse\\GrammarTreeVisitor.g:401:5: ( ^(a= ASSIGN ID v= optionValue ) )
            // org\\antlr\\v4\\parse\\GrammarTreeVisitor.g:401:9: ^(a= ASSIGN ID v= optionValue )
            {
                a = (GrammarAST)Match(input, ASSIGN, FOLLOW_ASSIGN_in_option295);
                Match(input, Token.DOWN, null);
                ID4 = (GrammarAST)Match(input, ID, FOLLOW_ID_in_option297);
                PushFollow(FOLLOW_optionValue_in_option301);
                v = optionValue();
                state._fsp--;

                Match(input, Token.UP, null);


                if (block) blockOption(ID4, (v != null ? ((GrammarAST)v.start) : null)); // most specific first
                else if (rule) ruleOption(ID4, (v != null ? ((GrammarAST)v.start) : null));
                else grammarOption(ID4, (v != null ? ((GrammarAST)v.start) : null));

            }


            exitOption(((GrammarAST)retval.start));

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
    // $ANTLR end "option"


    public class optionValue_return : TreeRuleReturnScope
    {
        public string v;
    };


    // $ANTLR start "optionValue"
    // org\\antlr\\v4\\parse\\GrammarTreeVisitor.g:409:1: optionValue returns [String v] : ( ID | STRING_LITERAL | INT );
    public optionValue_return optionValue()
    {
        optionValue_return retval = new optionValue_return();
        retval.start = input.LT(1);


        enterOptionValue(((GrammarAST)retval.start));
        retval.v = ((GrammarAST)retval.start).token.getText();

        try
        {
            // org\\antlr\\v4\\parse\\GrammarTreeVisitor.g:417:5: ( ID | STRING_LITERAL | INT )
            // org\\antlr\\v4\\parse\\GrammarTreeVisitor.g:
            {
                if (input.LA(1) == ID || input.LA(1) == INT || input.LA(1) == STRING_LITERAL)
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


            exitOptionValue(((GrammarAST)retval.start));

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
    // $ANTLR end "optionValue"


    public class delegateGrammars_return : TreeRuleReturnScope
    {
    };


    // $ANTLR start "delegateGrammars"
    // org\\antlr\\v4\\parse\\GrammarTreeVisitor.g:422:1: delegateGrammars : ^( IMPORT ( delegateGrammar )+ ) ;
    public delegateGrammars_return delegateGrammars()
    {
        delegateGrammars_return retval = new delegateGrammars_return();
        retval.start = input.LT(1);


        enterDelegateGrammars(((GrammarAST)retval.start));

        try
        {
            // org\\antlr\\v4\\parse\\GrammarTreeVisitor.g:429:2: ( ^( IMPORT ( delegateGrammar )+ ) )
            // org\\antlr\\v4\\parse\\GrammarTreeVisitor.g:429:6: ^( IMPORT ( delegateGrammar )+ )
            {
                Match(input, IMPORT, FOLLOW_IMPORT_in_delegateGrammars389);
                Match(input, Token.DOWN, null);
                // org\\antlr\\v4\\parse\\GrammarTreeVisitor.g:429:15: ( delegateGrammar )+
                int cnt6 = 0;
            loop6:
                while (true)
                {
                    int alt6 = 2;
                    int LA6_0 = input.LA(1);
                    if ((LA6_0 == ASSIGN || LA6_0 == ID))
                    {
                        alt6 = 1;
                    }

                    switch (alt6)
                    {
                        case 1:
                            // org\\antlr\\v4\\parse\\GrammarTreeVisitor.g:429:15: delegateGrammar
                            {
                                PushFollow(FOLLOW_delegateGrammar_in_delegateGrammars391);
                                delegateGrammar();
                                state._fsp--;

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

            }


            exitDelegateGrammars(((GrammarAST)retval.start));

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
    // $ANTLR end "delegateGrammars"


    public class delegateGrammar_return : TreeRuleReturnScope
    {
    };


    // $ANTLR start "delegateGrammar"
    // org\\antlr\\v4\\parse\\GrammarTreeVisitor.g:432:1: delegateGrammar : ( ^( ASSIGN label= ID id= ID ) |id= ID );
    public delegateGrammar_return delegateGrammar()
    {
        delegateGrammar_return retval = new delegateGrammar_return();
        retval.start = input.LT(1);

        GrammarAST label = null;
        GrammarAST id = null;


        enterDelegateGrammar(((GrammarAST)retval.start));

        try
        {
            // org\\antlr\\v4\\parse\\GrammarTreeVisitor.g:439:5: ( ^( ASSIGN label= ID id= ID ) |id= ID )
            int alt7 = 2;
            int LA7_0 = input.LA(1);
            if ((LA7_0 == ASSIGN))
            {
                alt7 = 1;
            }
            else if ((LA7_0 == ID))
            {
                alt7 = 2;
            }

            else
            {
                NoViableAltException nvae =
                    new NoViableAltException("", 7, 0, input);
                throw nvae;
            }

            switch (alt7)
            {
                case 1:
                    // org\\antlr\\v4\\parse\\GrammarTreeVisitor.g:439:9: ^( ASSIGN label= ID id= ID )
                    {
                        Match(input, ASSIGN, FOLLOW_ASSIGN_in_delegateGrammar420);
                        Match(input, Token.DOWN, null);
                        label = (GrammarAST)Match(input, ID, FOLLOW_ID_in_delegateGrammar424);
                        id = (GrammarAST)Match(input, ID, FOLLOW_ID_in_delegateGrammar428);
                        Match(input, Token.UP, null);

                        importGrammar(label, id);
                    }
                    break;
                case 2:
                    // org\\antlr\\v4\\parse\\GrammarTreeVisitor.g:440:9: id= ID
                    {
                        id = (GrammarAST)Match(input, ID, FOLLOW_ID_in_delegateGrammar443);
                        importGrammar(null, id);
                    }
                    break;

            }

            exitDelegateGrammar(((GrammarAST)retval.start));

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
    // $ANTLR end "delegateGrammar"


    public class tokensSpec_return : TreeRuleReturnScope
    {
    };


    // $ANTLR start "tokensSpec"
    // org\\antlr\\v4\\parse\\GrammarTreeVisitor.g:443:1: tokensSpec : ^( TOKENS_SPEC ( tokenSpec )+ ) ;
    public tokensSpec_return tokensSpec()
    {
        tokensSpec_return retval = new tokensSpec_return();
        retval.start = input.LT(1);


        enterTokensSpec(((GrammarAST)retval.start));

        try
        {
            // org\\antlr\\v4\\parse\\GrammarTreeVisitor.g:450:2: ( ^( TOKENS_SPEC ( tokenSpec )+ ) )
            // org\\antlr\\v4\\parse\\GrammarTreeVisitor.g:450:6: ^( TOKENS_SPEC ( tokenSpec )+ )
            {
                Match(input, TOKENS_SPEC, FOLLOW_TOKENS_SPEC_in_tokensSpec477);
                Match(input, Token.DOWN, null);
                // org\\antlr\\v4\\parse\\GrammarTreeVisitor.g:450:20: ( tokenSpec )+
                int cnt8 = 0;
            loop8:
                while (true)
                {
                    int alt8 = 2;
                    int LA8_0 = input.LA(1);
                    if ((LA8_0 == ID))
                    {
                        alt8 = 1;
                    }

                    switch (alt8)
                    {
                        case 1:
                            // org\\antlr\\v4\\parse\\GrammarTreeVisitor.g:450:20: tokenSpec
                            {
                                PushFollow(FOLLOW_tokenSpec_in_tokensSpec479);
                                tokenSpec();
                                state._fsp--;

                            }
                            break;

                        default:
                            if (cnt8 >= 1) goto exit8;// break loop8;
                            EarlyExitException eee = new EarlyExitException(8, input);
                            throw eee;
                    }
                    cnt8++;
                }
            exit8:
                Match(input, Token.UP, null);

            }


            exitTokensSpec(((GrammarAST)retval.start));

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
    // $ANTLR end "tokensSpec"


    public class tokenSpec_return : TreeRuleReturnScope
    {
    };


    // $ANTLR start "tokenSpec"
    // org\\antlr\\v4\\parse\\GrammarTreeVisitor.g:453:1: tokenSpec : ID ;
    public tokenSpec_return tokenSpec()
    {
        tokenSpec_return retval = new tokenSpec_return();
        retval.start = input.LT(1);

        GrammarAST ID5 = null;


        enterTokenSpec(((GrammarAST)retval.start));

        try
        {
            // org\\antlr\\v4\\parse\\GrammarTreeVisitor.g:460:2: ( ID )
            // org\\antlr\\v4\\parse\\GrammarTreeVisitor.g:460:4: ID
            {
                ID5 = (GrammarAST)Match(input, ID, FOLLOW_ID_in_tokenSpec502);
                defineToken(ID5);
            }


            exitTokenSpec(((GrammarAST)retval.start));

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
    // $ANTLR end "tokenSpec"


    public class channelsSpec_return : TreeRuleReturnScope
    {
    };


    // $ANTLR start "channelsSpec"
    // org\\antlr\\v4\\parse\\GrammarTreeVisitor.g:463:1: channelsSpec : ^( CHANNELS ( channelSpec )+ ) ;
    public channelsSpec_return channelsSpec()
    {
        channelsSpec_return retval = new channelsSpec_return();
        retval.start = input.LT(1);


        enterChannelsSpec(((GrammarAST)retval.start));

        try
        {
            // org\\antlr\\v4\\parse\\GrammarTreeVisitor.g:470:2: ( ^( CHANNELS ( channelSpec )+ ) )
            // org\\antlr\\v4\\parse\\GrammarTreeVisitor.g:470:6: ^( CHANNELS ( channelSpec )+ )
            {
                Match(input, CHANNELS, FOLLOW_CHANNELS_in_channelsSpec532);
                Match(input, Token.DOWN, null);
                // org\\antlr\\v4\\parse\\GrammarTreeVisitor.g:470:17: ( channelSpec )+
                int cnt9 = 0;
            loop9:
                while (true)
                {
                    int alt9 = 2;
                    int LA9_0 = input.LA(1);
                    if ((LA9_0 == ID))
                    {
                        alt9 = 1;
                    }

                    switch (alt9)
                    {
                        case 1:
                            // org\\antlr\\v4\\parse\\GrammarTreeVisitor.g:470:17: channelSpec
                            {
                                PushFollow(FOLLOW_channelSpec_in_channelsSpec534);
                                channelSpec();
                                state._fsp--;

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

            }


            exitChannelsSpec(((GrammarAST)retval.start));

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
    // $ANTLR end "channelsSpec"


    public class channelSpec_return : TreeRuleReturnScope
    {
    };


    // $ANTLR start "channelSpec"
    // org\\antlr\\v4\\parse\\GrammarTreeVisitor.g:473:1: channelSpec : ID ;
    public channelSpec_return channelSpec()
    {
        channelSpec_return retval = new channelSpec_return();
        retval.start = input.LT(1);

        GrammarAST ID6 = null;


        enterChannelSpec(((GrammarAST)retval.start));

        try
        {
            // org\\antlr\\v4\\parse\\g:480:2: ( ID )
            // org\\antlr\\v4\\parse\\g:480:4: ID
            {
                ID6 = (GrammarAST)Match(input, ID, FOLLOW_ID_in_channelSpec557);
                defineChannel(ID6);
            }


            exitChannelSpec(((GrammarAST)retval.start));

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
    // $ANTLR end "channelSpec"


    public class action_return : TreeRuleReturnScope
    {
    };


    // $ANTLR start "action"
    // org\\antlr\\v4\\parse\\g:483:1: action : ^( AT (sc= ID )? name= ID ACTION ) ;
    public action_return action()
    {
        action_return retval = new action_return();
        retval.start = input.LT(1);

        GrammarAST sc = null;
        GrammarAST name = null;
        GrammarAST ACTION7 = null;


        enterAction(((GrammarAST)retval.start));

        try
        {
            // org\\antlr\\v4\\parse\\g:490:2: ( ^( AT (sc= ID )? name= ID ACTION ) )
            // org\\antlr\\v4\\parse\\g:490:4: ^( AT (sc= ID )? name= ID ACTION )
            {
                Match(input, AT, FOLLOW_AT_in_action585);
                Match(input, Token.DOWN, null);
                // org\\antlr\\v4\\parse\\g:490:11: (sc= ID )?
                int alt10 = 2;
                int LA10_0 = input.LA(1);
                if ((LA10_0 == ID))
                {
                    int LA10_1 = input.LA(2);
                    if ((LA10_1 == ID))
                    {
                        alt10 = 1;
                    }
                }
                switch (alt10)
                {
                    case 1:
                        // org\\antlr\\v4\\parse\\g:490:11: sc= ID
                        {
                            sc = (GrammarAST)Match(input, ID, FOLLOW_ID_in_action589);
                        }
                        break;

                }

                name = (GrammarAST)Match(input, ID, FOLLOW_ID_in_action594);
                ACTION7 = (GrammarAST)Match(input, ACTION, FOLLOW_ACTION_in_action596);
                Match(input, Token.UP, null);

                globalNamedAction(sc, name, (ActionAST)ACTION7);
            }


            exitAction(((GrammarAST)retval.start));

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
    // $ANTLR end "action"


    public class rules_return : TreeRuleReturnScope
    {
    };


    // $ANTLR start "rules"
    // org\\antlr\\v4\\parse\\g:493:1: rules : ^( RULES ( rule | lexerRule )* ) ;
    public rules_return rules()
    {
        rules_return retval = new rules_return();
        retval.start = input.LT(1);

        GrammarAST RULES8 = null;


        enterRules(((GrammarAST)retval.start));

        try
        {
            // org\\antlr\\v4\\parse\\g:500:5: ( ^( RULES ( rule | lexerRule )* ) )
            // org\\antlr\\v4\\parse\\g:500:7: ^( RULES ( rule | lexerRule )* )
            {
                RULES8 = (GrammarAST)Match(input, RULES, FOLLOW_RULES_in_rules624);
                discoverRules(RULES8);
                if (input.LA(1) == Token.DOWN)
                {
                    Match(input, Token.DOWN, null);
                // org\\antlr\\v4\\parse\\g:500:40: ( rule | lexerRule )*
                loop11:
                    while (true)
                    {
                        int alt11 = 3;
                        int LA11_0 = input.LA(1);
                        if ((LA11_0 == RULE))
                        {
                            int LA11_2 = input.LA(2);
                            if ((LA11_2 == DOWN))
                            {
                                int LA11_3 = input.LA(3);
                                if ((LA11_3 == RULE_REF))
                                {
                                    alt11 = 1;
                                }
                                else if ((LA11_3 == TOKEN_REF))
                                {
                                    alt11 = 2;
                                }

                            }

                        }

                        switch (alt11)
                        {
                            case 1:
                                // org\\antlr\\v4\\parse\\g:500:41: rule
                                {
                                    PushFollow(FOLLOW_rule_in_rules629);
                                    rule();
                                    state._fsp--;

                                }
                                break;
                            case 2:
                                // org\\antlr\\v4\\parse\\g:500:46: lexerRule
                                {
                                    PushFollow(FOLLOW_lexerRule_in_rules631);
                                    lexerRule();
                                    state._fsp--;

                                }
                                break;

                            default:
                                goto exit11;
                                //break loop11;
                        }
                    }
                exit11:
                    finishRules(RULES8);
                    Match(input, Token.UP, null);
                }

            }


            exitRules(((GrammarAST)retval.start));

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
    // $ANTLR end "rules"


    public class mode_return : TreeRuleReturnScope
    {
    };


    // $ANTLR start "mode"
    // org\\antlr\\v4\\parse\\g:503:1: mode : ^( MODE ID ( lexerRule )* ) ;
    public mode_return mode()
    {
        mode_return retval = new mode_return();
        retval.start = input.LT(1);

        GrammarAST ID9 = null;
        GrammarAST MODE10 = null;


        enterMode(((GrammarAST)retval.start));

        try
        {
            // org\\antlr\\v4\\parse\\g:510:2: ( ^( MODE ID ( lexerRule )* ) )
            // org\\antlr\\v4\\parse\\g:510:4: ^( MODE ID ( lexerRule )* )
            {
                MODE10 = (GrammarAST)Match(input, MODE, FOLLOW_MODE_in_mode662);
                Match(input, Token.DOWN, null);
                ID9 = (GrammarAST)Match(input, ID, FOLLOW_ID_in_mode664);
                currentModeName = (ID9 != null ? ID9.getText() : null); modeDef(MODE10, ID9);
            // org\\antlr\\v4\\parse\\g:510:64: ( lexerRule )*
            loop12:
                while (true)
                {
                    int alt12 = 2;
                    int LA12_0 = input.LA(1);
                    if ((LA12_0 == RULE))
                    {
                        alt12 = 1;
                    }

                    switch (alt12)
                    {
                        case 1:
                            // org\\antlr\\v4\\parse\\g:510:64: lexerRule
                            {
                                PushFollow(FOLLOW_lexerRule_in_mode668);
                                lexerRule();
                                state._fsp--;

                            }
                            break;

                        default:
                            goto exit12;
                            //break loop12;
                    }
                }
            exit12:
                Match(input, Token.UP, null);

            }


            exitMode(((GrammarAST)retval.start));

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
    // $ANTLR end "mode"


    public class lexerRule_return : TreeRuleReturnScope
    {
    };


    // $ANTLR start "lexerRule"
    // org\\antlr\\v4\\parse\\g:513:1: lexerRule : ^( RULE TOKEN_REF ( ^( RULEMODIFIERS m= FRAGMENT ) )? (opts= optionsSpec )* lexerRuleBlock ) ;
    public lexerRule_return lexerRule()
    {
        lexerRule_return retval = new lexerRule_return();
        retval.start = input.LT(1);

        GrammarAST m = null;
        GrammarAST TOKEN_REF11 = null;
        GrammarAST RULE12 = null;
        TreeRuleReturnScope opts = null;
        TreeRuleReturnScope lexerRuleBlock13 = null;


        enterLexerRule(((GrammarAST)retval.start));
        List<GrammarAST> mods = new ();
        currentOuterAltNumber = 0;

        try
        {
            // org\\antlr\\v4\\parse\\g:522:2: ( ^( RULE TOKEN_REF ( ^( RULEMODIFIERS m= FRAGMENT ) )? (opts= optionsSpec )* lexerRuleBlock ) )
            // org\\antlr\\v4\\parse\\g:522:4: ^( RULE TOKEN_REF ( ^( RULEMODIFIERS m= FRAGMENT ) )? (opts= optionsSpec )* lexerRuleBlock )
            {
                RULE12 = (GrammarAST)Match(input, RULE, FOLLOW_RULE_in_lexerRule694);
                Match(input, Token.DOWN, null);
                TOKEN_REF11 = (GrammarAST)Match(input, TOKEN_REF, FOLLOW_TOKEN_REF_in_lexerRule696);
                currentRuleName = (TOKEN_REF11 != null ? TOKEN_REF11.getText() : null); currentRuleAST = RULE12;
                // org\\antlr\\v4\\parse\\g:524:4: ( ^( RULEMODIFIERS m= FRAGMENT ) )?
                int alt13 = 2;
                int LA13_0 = input.LA(1);
                if ((LA13_0 == RULEMODIFIERS))
                {
                    alt13 = 1;
                }
                switch (alt13)
                {
                    case 1:
                        // org\\antlr\\v4\\parse\\g:524:5: ^( RULEMODIFIERS m= FRAGMENT )
                        {
                            Match(input, RULEMODIFIERS, FOLLOW_RULEMODIFIERS_in_lexerRule708);
                            Match(input, Token.DOWN, null);
                            m = (GrammarAST)Match(input, FRAGMENT, FOLLOW_FRAGMENT_in_lexerRule712);
                            mods.Add(m);
                            Match(input, Token.UP, null);

                        }
                        break;

                }

            // org\\antlr\\v4\\parse\\g:525:8: (opts= optionsSpec )*
            loop14:
                while (true)
                {
                    int alt14 = 2;
                    int LA14_0 = input.LA(1);
                    if ((LA14_0 == OPTIONS))
                    {
                        alt14 = 1;
                    }

                    switch (alt14)
                    {
                        case 1:
                            // org\\antlr\\v4\\parse\\g:525:8: opts= optionsSpec
                            {
                                PushFollow(FOLLOW_optionsSpec_in_lexerRule724);
                                opts = optionsSpec();
                                state._fsp--;

                            }
                            break;

                        default:
                            goto exit14;
                            //break loop14;
                    }
                }
            exit14:
                discoverLexerRule((RuleAST)RULE12, TOKEN_REF11, mods, (opts != null ? ((GrammarAST)opts.start) : null), (GrammarAST)input.LT(1));
                PushFollow(FOLLOW_lexerRuleBlock_in_lexerRule745);
                lexerRuleBlock13 = lexerRuleBlock();
                state._fsp--;


                finishLexerRule((RuleAST)RULE12, TOKEN_REF11, (lexerRuleBlock13 != null ? ((GrammarAST)lexerRuleBlock13.start) : null));
                currentRuleName = null; currentRuleAST = null;

                Match(input, Token.UP, null);

            }


            exitLexerRule(((GrammarAST)retval.start));

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
    // $ANTLR end "lexerRule"


    public class rule_return : TreeRuleReturnScope
    {
    };


    // $ANTLR start "rule"
    // org\\antlr\\v4\\parse\\g:535:1: rule : ^( RULE RULE_REF ( ^( RULEMODIFIERS (m= ruleModifier )+ ) )? ( ARG_ACTION )? (ret= ruleReturns )? (thr= throwsSpec )? (loc= locals )? (opts= optionsSpec |a= ruleAction )* ruleBlock exceptionGroup ) ;
    public rule_return rule()
    {
        rule_return retval = new rule_return();
        retval.start = input.LT(1);

        GrammarAST RULE_REF14 = null;
        GrammarAST RULE15 = null;
        GrammarAST ARG_ACTION16 = null;
        TreeRuleReturnScope m = null;
        TreeRuleReturnScope ret = null;
        TreeRuleReturnScope thr = null;
        TreeRuleReturnScope loc = null;
        TreeRuleReturnScope opts = null;
        TreeRuleReturnScope a = null;
        TreeRuleReturnScope ruleBlock17 = null;


        enterRule(((GrammarAST)retval.start));
        List<GrammarAST> mods = new ();
        List<GrammarAST> actions = new (); // track roots
        currentOuterAltNumber = 0;

        try
        {
            // org\\antlr\\v4\\parse\\g:545:2: ( ^( RULE RULE_REF ( ^( RULEMODIFIERS (m= ruleModifier )+ ) )? ( ARG_ACTION )? (ret= ruleReturns )? (thr= throwsSpec )? (loc= locals )? (opts= optionsSpec |a= ruleAction )* ruleBlock exceptionGroup ) )
            // org\\antlr\\v4\\parse\\g:545:6: ^( RULE RULE_REF ( ^( RULEMODIFIERS (m= ruleModifier )+ ) )? ( ARG_ACTION )? (ret= ruleReturns )? (thr= throwsSpec )? (loc= locals )? (opts= optionsSpec |a= ruleAction )* ruleBlock exceptionGroup )
            {
                RULE15 = (GrammarAST)Match(input, RULE, FOLLOW_RULE_in_rule790);
                Match(input, Token.DOWN, null);
                RULE_REF14 = (GrammarAST)Match(input, RULE_REF, FOLLOW_RULE_REF_in_rule792);
                currentRuleName = (RULE_REF14 != null ? RULE_REF14.getText() : null); currentRuleAST = RULE15;
                // org\\antlr\\v4\\parse\\g:546:4: ( ^( RULEMODIFIERS (m= ruleModifier )+ ) )?
                int alt16 = 2;
                int LA16_0 = input.LA(1);
                if ((LA16_0 == RULEMODIFIERS))
                {
                    alt16 = 1;
                }
                switch (alt16)
                {
                    case 1:
                        // org\\antlr\\v4\\parse\\g:546:5: ^( RULEMODIFIERS (m= ruleModifier )+ )
                        {
                            Match(input, RULEMODIFIERS, FOLLOW_RULEMODIFIERS_in_rule801);
                            Match(input, Token.DOWN, null);
                            // org\\antlr\\v4\\parse\\g:546:21: (m= ruleModifier )+
                            int cnt15 = 0;
                        loop15:
                            while (true)
                            {
                                int alt15 = 2;
                                int LA15_0 = input.LA(1);
                                if ((LA15_0 == FRAGMENT || (LA15_0 >= PRIVATE && LA15_0 <= PUBLIC)))
                                {
                                    alt15 = 1;
                                }

                                switch (alt15)
                                {
                                    case 1:
                                        // org\\antlr\\v4\\parse\\g:546:22: m= ruleModifier
                                        {
                                            PushFollow(FOLLOW_ruleModifier_in_rule806);
                                            m = ruleModifier();
                                            state._fsp--;

                                            mods.Add((m != null ? ((GrammarAST)m.start) : null));
                                        }
                                        break;

                                    default:
                                        if (cnt15 >= 1) goto exit15;// break loop15;
                                        EarlyExitException eee = new EarlyExitException(15, input);
                                        throw eee;
                                }
                                cnt15++;
                            }
                        exit15:
                            Match(input, Token.UP, null);

                        }
                        break;

                }

                // org\\antlr\\v4\\parse\\g:547:4: ( ARG_ACTION )?
                int alt17 = 2;
                int LA17_0 = input.LA(1);
                if ((LA17_0 == ARG_ACTION))
                {
                    alt17 = 1;
                }
                switch (alt17)
                {
                    case 1:
                        // org\\antlr\\v4\\parse\\g:547:4: ARG_ACTION
                        {
                            ARG_ACTION16 = (GrammarAST)Match(input, ARG_ACTION, FOLLOW_ARG_ACTION_in_rule817);
                        }
                        break;

                }

                // org\\antlr\\v4\\parse\\g:548:12: (ret= ruleReturns )?
                int alt18 = 2;
                int LA18_0 = input.LA(1);
                if ((LA18_0 == RETURNS))
                {
                    alt18 = 1;
                }
                switch (alt18)
                {
                    case 1:
                        // org\\antlr\\v4\\parse\\g:548:12: ret= ruleReturns
                        {
                            PushFollow(FOLLOW_ruleReturns_in_rule830);
                            ret = ruleReturns();
                            state._fsp--;

                        }
                        break;

                }

                // org\\antlr\\v4\\parse\\g:549:12: (thr= throwsSpec )?
                int alt19 = 2;
                int LA19_0 = input.LA(1);
                if ((LA19_0 == THROWS))
                {
                    alt19 = 1;
                }
                switch (alt19)
                {
                    case 1:
                        // org\\antlr\\v4\\parse\\g:549:12: thr= throwsSpec
                        {
                            PushFollow(FOLLOW_throwsSpec_in_rule843);
                            thr = throwsSpec();
                            state._fsp--;

                        }
                        break;

                }

                // org\\antlr\\v4\\parse\\g:550:12: (loc= locals )?
                int alt20 = 2;
                int LA20_0 = input.LA(1);
                if ((LA20_0 == LOCALS))
                {
                    alt20 = 1;
                }
                switch (alt20)
                {
                    case 1:
                        // org\\antlr\\v4\\parse\\g:550:12: loc= locals
                        {
                            PushFollow(FOLLOW_locals_in_rule856);
                            loc = locals();
                            state._fsp--;

                        }
                        break;

                }

            // org\\antlr\\v4\\parse\\g:551:9: (opts= optionsSpec |a= ruleAction )*
            loop21:
                while (true)
                {
                    int alt21 = 3;
                    int LA21_0 = input.LA(1);
                    if ((LA21_0 == OPTIONS))
                    {
                        alt21 = 1;
                    }
                    else if ((LA21_0 == AT))
                    {
                        alt21 = 2;
                    }

                    switch (alt21)
                    {
                        case 1:
                            // org\\antlr\\v4\\parse\\g:551:11: opts= optionsSpec
                            {
                                PushFollow(FOLLOW_optionsSpec_in_rule871);
                                opts = optionsSpec();
                                state._fsp--;

                            }
                            break;
                        case 2:
                            // org\\antlr\\v4\\parse\\g:552:11: a= ruleAction
                            {
                                PushFollow(FOLLOW_ruleAction_in_rule885);
                                a = ruleAction();
                                state._fsp--;

                                actions.Add((a != null ? ((GrammarAST)a.start) : null));
                            }
                            break;

                        default:
                            goto exit21;
                            //break loop21;
                    }
                }
            exit21:
                discoverRule((RuleAST)RULE15, RULE_REF14, mods, (ActionAST)ARG_ACTION16,
                                            (ret != null ? ((GrammarAST)ret.start) : null) != null ? (ActionAST)(ret != null ? ((GrammarAST)ret.start) : null).getChild(0) : null,
                                            (thr != null ? ((GrammarAST)thr.start) : null), (opts != null ? ((GrammarAST)opts.start) : null),
                                            (loc != null ? ((GrammarAST)loc.start) : null) != null ? (ActionAST)(loc != null ? ((GrammarAST)loc.start) : null).getChild(0) : null,
                                            actions, (GrammarAST)input.LT(1));
                PushFollow(FOLLOW_ruleBlock_in_rule916);
                ruleBlock17 = ruleBlock();
                state._fsp--;

                PushFollow(FOLLOW_exceptionGroup_in_rule918);
                exceptionGroup();
                state._fsp--;

                finishRule((RuleAST)RULE15, RULE_REF14, (ruleBlock17 != null ? ((GrammarAST)ruleBlock17.start) : null)); currentRuleName = null; currentRuleAST = null;
                Match(input, Token.UP, null);

            }


            exitRule(((GrammarAST)retval.start));

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
    // $ANTLR end "rule"


    public class exceptionGroup_return : TreeRuleReturnScope
    {
    };


    // $ANTLR start "exceptionGroup"
    // org\\antlr\\v4\\parse\\g:564:1: exceptionGroup : ( exceptionHandler )* ( finallyClause )? ;
    public exceptionGroup_return exceptionGroup()
    {
        exceptionGroup_return retval = new exceptionGroup_return();
        retval.start = input.LT(1);


        enterExceptionGroup(((GrammarAST)retval.start));

        try
        {
            // org\\antlr\\v4\\parse\\g:571:5: ( ( exceptionHandler )* ( finallyClause )? )
            // org\\antlr\\v4\\parse\\g:571:7: ( exceptionHandler )* ( finallyClause )?
            {
            // org\\antlr\\v4\\parse\\g:571:7: ( exceptionHandler )*
            loop22:
                while (true)
                {
                    int alt22 = 2;
                    int LA22_0 = input.LA(1);
                    if ((LA22_0 == CATCH))
                    {
                        alt22 = 1;
                    }

                    switch (alt22)
                    {
                        case 1:
                            // org\\antlr\\v4\\parse\\g:571:7: exceptionHandler
                            {
                                PushFollow(FOLLOW_exceptionHandler_in_exceptionGroup965);
                                exceptionHandler();
                                state._fsp--;

                            }
                            break;

                        default:
                            goto exit22;
                            //break loop22;
                    }
                }
            exit22:
                // org\\antlr\\v4\\parse\\g:571:25: ( finallyClause )?
                int alt23 = 2;
                int LA23_0 = input.LA(1);
                if ((LA23_0 == FINALLY))
                {
                    alt23 = 1;
                }
                switch (alt23)
                {
                    case 1:
                        // org\\antlr\\v4\\parse\\g:571:25: finallyClause
                        {
                            PushFollow(FOLLOW_finallyClause_in_exceptionGroup968);
                            finallyClause();
                            state._fsp--;

                        }
                        break;

                }

            }


            exitExceptionGroup(((GrammarAST)retval.start));

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
    // $ANTLR end "exceptionGroup"


    public class exceptionHandler_return : TreeRuleReturnScope
    {
    };


    // $ANTLR start "exceptionHandler"
    // org\\antlr\\v4\\parse\\g:574:1: exceptionHandler : ^( CATCH ARG_ACTION ACTION ) ;
    public exceptionHandler_return exceptionHandler()
    {
        exceptionHandler_return retval = new exceptionHandler_return();
        retval.start = input.LT(1);

        GrammarAST ARG_ACTION18 = null;
        GrammarAST ACTION19 = null;


        enterExceptionHandler(((GrammarAST)retval.start));

        try
        {
            // org\\antlr\\v4\\parse\\g:581:2: ( ^( CATCH ARG_ACTION ACTION ) )
            // org\\antlr\\v4\\parse\\g:581:4: ^( CATCH ARG_ACTION ACTION )
            {
                Match(input, CATCH, FOLLOW_CATCH_in_exceptionHandler994);
                Match(input, Token.DOWN, null);
                ARG_ACTION18 = (GrammarAST)Match(input, ARG_ACTION, FOLLOW_ARG_ACTION_in_exceptionHandler996);
                ACTION19 = (GrammarAST)Match(input, ACTION, FOLLOW_ACTION_in_exceptionHandler998);
                Match(input, Token.UP, null);

                ruleCatch(ARG_ACTION18, (ActionAST)ACTION19);
            }


            exitExceptionHandler(((GrammarAST)retval.start));

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
    // $ANTLR end "exceptionHandler"


    public class finallyClause_return : TreeRuleReturnScope
    {
    };


    // $ANTLR start "finallyClause"
    // org\\antlr\\v4\\parse\\g:584:1: finallyClause : ^( FINALLY ACTION ) ;
    public finallyClause_return finallyClause()
    {
        finallyClause_return retval = new finallyClause_return();
        retval.start = input.LT(1);

        GrammarAST ACTION20 = null;


        enterFinallyClause(((GrammarAST)retval.start));

        try
        {
            // org\\antlr\\v4\\parse\\g:591:2: ( ^( FINALLY ACTION ) )
            // org\\antlr\\v4\\parse\\g:591:4: ^( FINALLY ACTION )
            {
                Match(input, FINALLY, FOLLOW_FINALLY_in_finallyClause1023);
                Match(input, Token.DOWN, null);
                ACTION20 = (GrammarAST)Match(input, ACTION, FOLLOW_ACTION_in_finallyClause1025);
                Match(input, Token.UP, null);

                finallyAction((ActionAST)ACTION20);
            }


            exitFinallyClause(((GrammarAST)retval.start));

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
    // $ANTLR end "finallyClause"


    public class locals_return : TreeRuleReturnScope
    {
    };


    // $ANTLR start "locals"
    // org\\antlr\\v4\\parse\\g:594:1: locals : ^( LOCALS ARG_ACTION ) ;
    public locals_return locals()
    {
        locals_return retval = new locals_return();
        retval.start = input.LT(1);


        enterLocals(((GrammarAST)retval.start));

        try
        {
            // org\\antlr\\v4\\parse\\g:601:2: ( ^( LOCALS ARG_ACTION ) )
            // org\\antlr\\v4\\parse\\g:601:4: ^( LOCALS ARG_ACTION )
            {
                Match(input, LOCALS, FOLLOW_LOCALS_in_locals1053);
                Match(input, Token.DOWN, null);
                Match(input, ARG_ACTION, FOLLOW_ARG_ACTION_in_locals1055);
                Match(input, Token.UP, null);

            }


            exitLocals(((GrammarAST)retval.start));

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
    // $ANTLR end "locals"


    public class ruleReturns_return : TreeRuleReturnScope
    {
    };


    // $ANTLR start "ruleReturns"
    // org\\antlr\\v4\\parse\\g:604:1: ruleReturns : ^( RETURNS ARG_ACTION ) ;
    public ruleReturns_return ruleReturns()
    {
        ruleReturns_return retval = new ruleReturns_return();
        retval.start = input.LT(1);


        enterRuleReturns(((GrammarAST)retval.start));

        try
        {
            // org\\antlr\\v4\\parse\\g:611:2: ( ^( RETURNS ARG_ACTION ) )
            // org\\antlr\\v4\\parse\\g:611:4: ^( RETURNS ARG_ACTION )
            {
                Match(input, RETURNS, FOLLOW_RETURNS_in_ruleReturns1078);
                Match(input, Token.DOWN, null);
                Match(input, ARG_ACTION, FOLLOW_ARG_ACTION_in_ruleReturns1080);
                Match(input, Token.UP, null);

            }


            exitRuleReturns(((GrammarAST)retval.start));

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
    // $ANTLR end "ruleReturns"


    public class throwsSpec_return : TreeRuleReturnScope
    {
    };


    // $ANTLR start "throwsSpec"
    // org\\antlr\\v4\\parse\\g:614:1: throwsSpec : ^( THROWS ( ID )+ ) ;
    public throwsSpec_return throwsSpec()
    {
        throwsSpec_return retval = new throwsSpec_return();
        retval.start = input.LT(1);


        enterThrowsSpec(((GrammarAST)retval.start));

        try
        {
            // org\\antlr\\v4\\parse\\g:621:5: ( ^( THROWS ( ID )+ ) )
            // org\\antlr\\v4\\parse\\g:621:7: ^( THROWS ( ID )+ )
            {
                Match(input, THROWS, FOLLOW_THROWS_in_throwsSpec1106);
                Match(input, Token.DOWN, null);
                // org\\antlr\\v4\\parse\\g:621:16: ( ID )+
                int cnt24 = 0;
            loop24:
                while (true)
                {
                    int alt24 = 2;
                    int LA24_0 = input.LA(1);
                    if ((LA24_0 == ID))
                    {
                        alt24 = 1;
                    }

                    switch (alt24)
                    {
                        case 1:
                            // org\\antlr\\v4\\parse\\g:621:16: ID
                            {
                                Match(input, ID, FOLLOW_ID_in_throwsSpec1108);
                            }
                            break;

                        default:
                            if (cnt24 >= 1) goto exit24;// break loop24;
                            EarlyExitException eee = new EarlyExitException(24, input);
                            throw eee;
                    }
                    cnt24++;
                }
            exit24:
                Match(input, Token.UP, null);

            }


            exitThrowsSpec(((GrammarAST)retval.start));

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
    // $ANTLR end "throwsSpec"


    public class ruleAction_return : TreeRuleReturnScope
    {
    };


    // $ANTLR start "ruleAction"
    // org\\antlr\\v4\\parse\\g:624:1: ruleAction : ^( AT ID ACTION ) ;
    public ruleAction_return ruleAction()
    {
        ruleAction_return retval = new ruleAction_return();
        retval.start = input.LT(1);


        enterRuleAction(((GrammarAST)retval.start));

        try
        {
            // org\\antlr\\v4\\parse\\g:631:2: ( ^( AT ID ACTION ) )
            // org\\antlr\\v4\\parse\\g:631:4: ^( AT ID ACTION )
            {
                Match(input, AT, FOLLOW_AT_in_ruleAction1135);
                Match(input, Token.DOWN, null);
                Match(input, ID, FOLLOW_ID_in_ruleAction1137);
                Match(input, ACTION, FOLLOW_ACTION_in_ruleAction1139);
                Match(input, Token.UP, null);

            }


            exitRuleAction(((GrammarAST)retval.start));

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
    // $ANTLR end "ruleAction"


    public class ruleModifier_return : TreeRuleReturnScope
    {
    };


    // $ANTLR start "ruleModifier"
    // org\\antlr\\v4\\parse\\g:634:1: ruleModifier : ( PUBLIC | PRIVATE | PROTECTED | FRAGMENT );
    public ruleModifier_return ruleModifier()
    {
        ruleModifier_return retval = new ruleModifier_return();
        retval.start = input.LT(1);


        enterRuleModifier(((GrammarAST)retval.start));

        try
        {
            // org\\antlr\\v4\\parse\\g:641:5: ( PUBLIC | PRIVATE | PROTECTED | FRAGMENT )
            // org\\antlr\\v4\\parse\\g:
            {
                if (input.LA(1) == FRAGMENT || (input.LA(1) >= PRIVATE && input.LA(1) <= PUBLIC))
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


            exitRuleModifier(((GrammarAST)retval.start));

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
    // $ANTLR end "ruleModifier"


    public class lexerRuleBlock_return : TreeRuleReturnScope
    {
    };


    // $ANTLR start "lexerRuleBlock"
    // org\\antlr\\v4\\parse\\g:647:1: lexerRuleBlock : ^( BLOCK ( lexerOuterAlternative )+ ) ;
    public lexerRuleBlock_return lexerRuleBlock()
    {
        lexerRuleBlock_return retval = new lexerRuleBlock_return();
        retval.start = input.LT(1);


        enterLexerRuleBlock(((GrammarAST)retval.start));

        try
        {
            // org\\antlr\\v4\\parse\\g:654:5: ( ^( BLOCK ( lexerOuterAlternative )+ ) )
            // org\\antlr\\v4\\parse\\g:654:7: ^( BLOCK ( lexerOuterAlternative )+ )
            {
                Match(input, BLOCK, FOLLOW_BLOCK_in_lexerRuleBlock1217);
                Match(input, Token.DOWN, null);
                // org\\antlr\\v4\\parse\\g:655:7: ( lexerOuterAlternative )+
                int cnt25 = 0;
            loop25:
                while (true)
                {
                    int alt25 = 2;
                    int LA25_0 = input.LA(1);
                    if ((LA25_0 == ALT || LA25_0 == LEXER_ALT_ACTION))
                    {
                        alt25 = 1;
                    }

                    switch (alt25)
                    {
                        case 1:
                            // org\\antlr\\v4\\parse\\g:655:9: lexerOuterAlternative
                            {

                                currentOuterAltRoot = (GrammarAST)input.LT(1);
                                currentOuterAltNumber++;

                                PushFollow(FOLLOW_lexerOuterAlternative_in_lexerRuleBlock1236);
                                lexerOuterAlternative();
                                state._fsp--;

                            }
                            break;

                        default:
                            if (cnt25 >= 1) goto exit25;// break loop25;
                            EarlyExitException eee = new EarlyExitException(25, input);
                            throw eee;
                    }
                    cnt25++;
                }
            exit25:
                Match(input, Token.UP, null);

            }


            exitLexerRuleBlock(((GrammarAST)retval.start));

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
    // $ANTLR end "lexerRuleBlock"


    public class ruleBlock_return : TreeRuleReturnScope
    {
    };


    // $ANTLR start "ruleBlock"
    // org\\antlr\\v4\\parse\\g:664:1: ruleBlock : ^( BLOCK ( outerAlternative )+ ) ;
    public ruleBlock_return ruleBlock()
    {
        ruleBlock_return retval = new ruleBlock_return();
        retval.start = input.LT(1);


        enterRuleBlock(((GrammarAST)retval.start));

        try
        {
            // org\\antlr\\v4\\parse\\g:671:5: ( ^( BLOCK ( outerAlternative )+ ) )
            // org\\antlr\\v4\\parse\\g:671:7: ^( BLOCK ( outerAlternative )+ )
            {
                Match(input, BLOCK, FOLLOW_BLOCK_in_ruleBlock1281);
                Match(input, Token.DOWN, null);
                // org\\antlr\\v4\\parse\\g:672:7: ( outerAlternative )+
                int cnt26 = 0;
            loop26:
                while (true)
                {
                    int alt26 = 2;
                    int LA26_0 = input.LA(1);
                    if ((LA26_0 == ALT))
                    {
                        alt26 = 1;
                    }

                    switch (alt26)
                    {
                        case 1:
                            // org\\antlr\\v4\\parse\\g:672:9: outerAlternative
                            {

                                currentOuterAltRoot = (GrammarAST)input.LT(1);
                                currentOuterAltNumber++;

                                PushFollow(FOLLOW_outerAlternative_in_ruleBlock1300);
                                outerAlternative();
                                state._fsp--;

                            }
                            break;

                        default:
                            if (cnt26 >= 1) goto exit26;// break loop26;
                            EarlyExitException eee = new EarlyExitException(26, input);
                            throw eee;
                    }
                    cnt26++;
                }
            exit26:
                Match(input, Token.UP, null);

            }


            exitRuleBlock(((GrammarAST)retval.start));

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
    // $ANTLR end "ruleBlock"


    public class lexerOuterAlternative_return : TreeRuleReturnScope
    {
    };


    // $ANTLR start "lexerOuterAlternative"
    // org\\antlr\\v4\\parse\\g:681:1: lexerOuterAlternative : lexerAlternative ;
    public lexerOuterAlternative_return lexerOuterAlternative()
    {
        lexerOuterAlternative_return retval = new lexerOuterAlternative_return();
        retval.start = input.LT(1);


        enterLexerOuterAlternative((AltAST)((GrammarAST)retval.start));
        discoverOuterAlt((AltAST)((GrammarAST)retval.start));

        try
        {
            // org\\antlr\\v4\\parse\\g:690:2: ( lexerAlternative )
            // org\\antlr\\v4\\parse\\g:690:4: lexerAlternative
            {
                PushFollow(FOLLOW_lexerAlternative_in_lexerOuterAlternative1340);
                lexerAlternative();
                state._fsp--;

            }


            finishOuterAlt((AltAST)((GrammarAST)retval.start));
            exitLexerOuterAlternative((AltAST)((GrammarAST)retval.start));

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
    // $ANTLR end "lexerOuterAlternative"


    public class outerAlternative_return : TreeRuleReturnScope
    {
    };


    // $ANTLR start "outerAlternative"
    // org\\antlr\\v4\\parse\\g:694:1: outerAlternative : alternative ;
    public outerAlternative_return outerAlternative()
    {
        outerAlternative_return retval = new outerAlternative_return();
        retval.start = input.LT(1);


        enterOuterAlternative((AltAST)((GrammarAST)retval.start));
        discoverOuterAlt((AltAST)((GrammarAST)retval.start));

        try
        {
            // org\\antlr\\v4\\parse\\g:703:2: ( alternative )
            // org\\antlr\\v4\\parse\\g:703:4: alternative
            {
                PushFollow(FOLLOW_alternative_in_outerAlternative1362);
                alternative();
                state._fsp--;

            }


            finishOuterAlt((AltAST)((GrammarAST)retval.start));
            exitOuterAlternative((AltAST)((GrammarAST)retval.start));

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
    // $ANTLR end "outerAlternative"


    public class lexerAlternative_return : TreeRuleReturnScope
    {
    };


    // $ANTLR start "lexerAlternative"
    // org\\antlr\\v4\\parse\\g:706:1: lexerAlternative : ( ^( LEXER_ALT_ACTION lexerElements ( lexerCommand )+ ) | lexerElements );
    public lexerAlternative_return lexerAlternative()
    {
        lexerAlternative_return retval = new lexerAlternative_return();
        retval.start = input.LT(1);


        enterLexerAlternative(((GrammarAST)retval.start));

        try
        {
            // org\\antlr\\v4\\parse\\g:713:2: ( ^( LEXER_ALT_ACTION lexerElements ( lexerCommand )+ ) | lexerElements )
            int alt28 = 2;
            int LA28_0 = input.LA(1);
            if ((LA28_0 == LEXER_ALT_ACTION))
            {
                alt28 = 1;
            }
            else if ((LA28_0 == ALT))
            {
                alt28 = 2;
            }

            else
            {
                NoViableAltException nvae =
                    new NoViableAltException("", 28, 0, input);
                throw nvae;
            }

            switch (alt28)
            {
                case 1:
                    // org\\antlr\\v4\\parse\\g:713:4: ^( LEXER_ALT_ACTION lexerElements ( lexerCommand )+ )
                    {
                        Match(input, LEXER_ALT_ACTION, FOLLOW_LEXER_ALT_ACTION_in_lexerAlternative1384);
                        Match(input, Token.DOWN, null);
                        PushFollow(FOLLOW_lexerElements_in_lexerAlternative1386);
                        lexerElements();
                        state._fsp--;

                        // org\\antlr\\v4\\parse\\g:713:37: ( lexerCommand )+
                        int cnt27 = 0;
                    loop27:
                        while (true)
                        {
                            int alt27 = 2;
                            int LA27_0 = input.LA(1);
                            if ((LA27_0 == ID || LA27_0 == LEXER_ACTION_CALL))
                            {
                                alt27 = 1;
                            }

                            switch (alt27)
                            {
                                case 1:
                                    // org\\antlr\\v4\\parse\\g:713:37: lexerCommand
                                    {
                                        PushFollow(FOLLOW_lexerCommand_in_lexerAlternative1388);
                                        lexerCommand();
                                        state._fsp--;

                                    }
                                    break;

                                default:
                                    if (cnt27 >= 1) goto exit27;// break loop27;
                                    EarlyExitException eee = new EarlyExitException(27, input);
                                    throw eee;
                            }
                            cnt27++;
                        }
                    exit27:
                        Match(input, Token.UP, null);

                    }
                    break;
                case 2:
                    // org\\antlr\\v4\\parse\\g:714:9: lexerElements
                    {
                        PushFollow(FOLLOW_lexerElements_in_lexerAlternative1400);
                        lexerElements();
                        state._fsp--;

                    }
                    break;

            }

            exitLexerAlternative(((GrammarAST)retval.start));

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
    // $ANTLR end "lexerAlternative"


    public class lexerElements_return : TreeRuleReturnScope
    {
    };


    // $ANTLR start "lexerElements"
    // org\\antlr\\v4\\parse\\g:717:1: lexerElements : ^( ALT ( lexerElement )+ ) ;
    public lexerElements_return lexerElements()
    {
        lexerElements_return retval = new lexerElements_return();
        retval.start = input.LT(1);


        enterLexerElements(((GrammarAST)retval.start));

        try
        {
            // org\\antlr\\v4\\parse\\g:724:5: ( ^( ALT ( lexerElement )+ ) )
            // org\\antlr\\v4\\parse\\g:724:7: ^( ALT ( lexerElement )+ )
            {
                Match(input, ALT, FOLLOW_ALT_in_lexerElements1428);
                Match(input, Token.DOWN, null);
                // org\\antlr\\v4\\parse\\g:724:13: ( lexerElement )+
                int cnt29 = 0;
            loop29:
                while (true)
                {
                    int alt29 = 2;
                    int LA29_0 = input.LA(1);
                    if ((LA29_0 == ACTION || LA29_0 == LEXER_CHAR_SET || LA29_0 == NOT || LA29_0 == RANGE || LA29_0 == RULE_REF || LA29_0 == SEMPRED || LA29_0 == STRING_LITERAL || LA29_0 == TOKEN_REF || (LA29_0 >= BLOCK && LA29_0 <= CLOSURE) || LA29_0 == EPSILON || (LA29_0 >= OPTIONAL && LA29_0 <= POSITIVE_CLOSURE) || (LA29_0 >= SET && LA29_0 <= WILDCARD)))
                    {
                        alt29 = 1;
                    }

                    switch (alt29)
                    {
                        case 1:
                            // org\\antlr\\v4\\parse\\g:724:13: lexerElement
                            {
                                PushFollow(FOLLOW_lexerElement_in_lexerElements1430);
                                lexerElement();
                                state._fsp--;

                            }
                            break;

                        default:
                            if (cnt29 >= 1) goto exit29; // break loop29;
                            EarlyExitException eee = new EarlyExitException(29, input);
                            throw eee;
                    }
                    cnt29++;
                }
            exit29:
                Match(input, Token.UP, null);

            }


            exitLexerElements(((GrammarAST)retval.start));

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
    // $ANTLR end "lexerElements"


    public class lexerElement_return : TreeRuleReturnScope
    {
    };


    // $ANTLR start "lexerElement"
    // org\\antlr\\v4\\parse\\g:727:1: lexerElement : ( lexerAtom | lexerSubrule | ACTION | SEMPRED | ^( ACTION elementOptions ) | ^( SEMPRED elementOptions ) | EPSILON );
    public lexerElement_return lexerElement()
    {
        lexerElement_return retval = new lexerElement_return();
        retval.start = input.LT(1);

        GrammarAST ACTION21 = null;
        GrammarAST SEMPRED22 = null;
        GrammarAST ACTION23 = null;
        GrammarAST SEMPRED24 = null;


        enterLexerElement(((GrammarAST)retval.start));

        try
        {
            // org\\antlr\\v4\\parse\\g:734:2: ( lexerAtom | lexerSubrule | ACTION | SEMPRED | ^( ACTION elementOptions ) | ^( SEMPRED elementOptions ) | EPSILON )
            int alt30 = 7;
            switch (input.LA(1))
            {
                case LEXER_CHAR_SET:
                case NOT:
                case RANGE:
                case RULE_REF:
                case STRING_LITERAL:
                case TOKEN_REF:
                case SET:
                case WILDCARD:
                    {
                        alt30 = 1;
                    }
                    break;
                case BLOCK:
                case CLOSURE:
                case OPTIONAL:
                case POSITIVE_CLOSURE:
                    {
                        alt30 = 2;
                    }
                    break;
                case ACTION:
                    {
                        int LA30_3 = input.LA(2);
                        if ((LA30_3 == DOWN))
                        {
                            alt30 = 5;
                        }
                        else if (((LA30_3 >= UP && LA30_3 <= ACTION) || LA30_3 == LEXER_CHAR_SET || LA30_3 == NOT || LA30_3 == RANGE || LA30_3 == RULE_REF || LA30_3 == SEMPRED || LA30_3 == STRING_LITERAL || LA30_3 == TOKEN_REF || (LA30_3 >= BLOCK && LA30_3 <= CLOSURE) || LA30_3 == EPSILON || (LA30_3 >= OPTIONAL && LA30_3 <= POSITIVE_CLOSURE) || (LA30_3 >= SET && LA30_3 <= WILDCARD)))
                        {
                            alt30 = 3;
                        }

                        else
                        {
                            int nvaeMark = input.Mark();
                            try
                            {
                                input.Consume();
                                NoViableAltException nvae2 =
                                    new NoViableAltException("", 30, 3, input);
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
                        int LA30_4 = input.LA(2);
                        if ((LA30_4 == DOWN))
                        {
                            alt30 = 6;
                        }
                        else if (((LA30_4 >= UP && LA30_4 <= ACTION) || LA30_4 == LEXER_CHAR_SET || LA30_4 == NOT || LA30_4 == RANGE || LA30_4 == RULE_REF || LA30_4 == SEMPRED || LA30_4 == STRING_LITERAL || LA30_4 == TOKEN_REF || (LA30_4 >= BLOCK && LA30_4 <= CLOSURE) || LA30_4 == EPSILON || (LA30_4 >= OPTIONAL && LA30_4 <= POSITIVE_CLOSURE) || (LA30_4 >= SET && LA30_4 <= WILDCARD)))
                        {
                            alt30 = 4;
                        }

                        else
                        {
                            int nvaeMark = input.Mark();
                            try
                            {
                                input.Consume();
                                NoViableAltException nvae2 =
                                    new NoViableAltException("", 30, 4, input);
                                throw nvae2;
                            }
                            finally
                            {
                                input.Rewind(nvaeMark);
                            }
                        }

                    }
                    break;
                case EPSILON:
                    {
                        alt30 = 7;
                    }
                    break;
                default:
                    NoViableAltException nvae =
                        new NoViableAltException("", 30, 0, input);
                    throw nvae;
            }
            switch (alt30)
            {
                case 1:
                    // org\\antlr\\v4\\parse\\g:734:4: lexerAtom
                    {
                        PushFollow(FOLLOW_lexerAtom_in_lexerElement1456);
                        lexerAtom();
                        state._fsp--;

                    }
                    break;
                case 2:
                    // org\\antlr\\v4\\parse\\g:735:4: lexerSubrule
                    {
                        PushFollow(FOLLOW_lexerSubrule_in_lexerElement1461);
                        lexerSubrule();
                        state._fsp--;

                    }
                    break;
                case 3:
                    // org\\antlr\\v4\\parse\\g:736:6: ACTION
                    {
                        ACTION21 = (GrammarAST)Match(input, ACTION, FOLLOW_ACTION_in_lexerElement1468);
                        actionInAlt((ActionAST)ACTION21);
                    }
                    break;
                case 4:
                    // org\\antlr\\v4\\parse\\g:737:6: SEMPRED
                    {
                        SEMPRED22 = (GrammarAST)Match(input, SEMPRED, FOLLOW_SEMPRED_in_lexerElement1482);
                        sempredInAlt((PredAST)SEMPRED22);
                    }
                    break;
                case 5:
                    // org\\antlr\\v4\\parse\\g:738:6: ^( ACTION elementOptions )
                    {
                        ACTION23 = (GrammarAST)Match(input, ACTION, FOLLOW_ACTION_in_lexerElement1497);
                        Match(input, Token.DOWN, null);
                        PushFollow(FOLLOW_elementOptions_in_lexerElement1499);
                        elementOptions();
                        state._fsp--;

                        Match(input, Token.UP, null);

                        actionInAlt((ActionAST)ACTION23);
                    }
                    break;
                case 6:
                    // org\\antlr\\v4\\parse\\g:739:6: ^( SEMPRED elementOptions )
                    {
                        SEMPRED24 = (GrammarAST)Match(input, SEMPRED, FOLLOW_SEMPRED_in_lexerElement1510);
                        Match(input, Token.DOWN, null);
                        PushFollow(FOLLOW_elementOptions_in_lexerElement1512);
                        elementOptions();
                        state._fsp--;

                        Match(input, Token.UP, null);

                        sempredInAlt((PredAST)SEMPRED24);
                    }
                    break;
                case 7:
                    // org\\antlr\\v4\\parse\\g:740:4: EPSILON
                    {
                        Match(input, EPSILON, FOLLOW_EPSILON_in_lexerElement1520);
                    }
                    break;

            }

            exitLexerElement(((GrammarAST)retval.start));

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
    // $ANTLR end "lexerElement"


    public class lexerBlock_return : TreeRuleReturnScope
    {
    };


    // $ANTLR start "lexerBlock"
    // org\\antlr\\v4\\parse\\g:743:1: lexerBlock : ^( BLOCK ( optionsSpec )? ( lexerAlternative )+ ) ;
    public lexerBlock_return lexerBlock()
    {
        lexerBlock_return retval = new lexerBlock_return();
        retval.start = input.LT(1);


        enterLexerBlock(((GrammarAST)retval.start));

        try
        {
            // org\\antlr\\v4\\parse\\g:750:3: ( ^( BLOCK ( optionsSpec )? ( lexerAlternative )+ ) )
            // org\\antlr\\v4\\parse\\g:750:5: ^( BLOCK ( optionsSpec )? ( lexerAlternative )+ )
            {
                Match(input, BLOCK, FOLLOW_BLOCK_in_lexerBlock1543);
                Match(input, Token.DOWN, null);
                // org\\antlr\\v4\\parse\\g:750:13: ( optionsSpec )?
                int alt31 = 2;
                int LA31_0 = input.LA(1);
                if ((LA31_0 == OPTIONS))
                {
                    alt31 = 1;
                }
                switch (alt31)
                {
                    case 1:
                        // org\\antlr\\v4\\parse\\g:750:13: optionsSpec
                        {
                            PushFollow(FOLLOW_optionsSpec_in_lexerBlock1545);
                            optionsSpec();
                            state._fsp--;

                        }
                        break;

                }

                // org\\antlr\\v4\\parse\\g:750:26: ( lexerAlternative )+
                int cnt32 = 0;
            loop32:
                while (true)
                {
                    int alt32 = 2;
                    int LA32_0 = input.LA(1);
                    if ((LA32_0 == ALT || LA32_0 == LEXER_ALT_ACTION))
                    {
                        alt32 = 1;
                    }

                    switch (alt32)
                    {
                        case 1:
                            // org\\antlr\\v4\\parse\\g:750:26: lexerAlternative
                            {
                                PushFollow(FOLLOW_lexerAlternative_in_lexerBlock1548);
                                lexerAlternative();
                                state._fsp--;

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
                Match(input, Token.UP, null);

            }


            exitLexerBlock(((GrammarAST)retval.start));

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
    // $ANTLR end "lexerBlock"


    public class lexerAtom_return : TreeRuleReturnScope
    {
    };


    // $ANTLR start "lexerAtom"
    // org\\antlr\\v4\\parse\\g:753:1: lexerAtom : ( terminal | ^( NOT blockSet ) | blockSet | ^( WILDCARD elementOptions ) | WILDCARD | LEXER_CHAR_SET | range | ruleref );
    public lexerAtom_return lexerAtom()
    {
        lexerAtom_return retval = new lexerAtom_return();
        retval.start = input.LT(1);


        enterLexerAtom(((GrammarAST)retval.start));

        try
        {
            // org\\antlr\\v4\\parse\\g:760:5: ( terminal | ^( NOT blockSet ) | blockSet | ^( WILDCARD elementOptions ) | WILDCARD | LEXER_CHAR_SET | range | ruleref )
            int alt33 = 8;
            switch (input.LA(1))
            {
                case STRING_LITERAL:
                case TOKEN_REF:
                    {
                        alt33 = 1;
                    }
                    break;
                case NOT:
                    {
                        alt33 = 2;
                    }
                    break;
                case SET:
                    {
                        alt33 = 3;
                    }
                    break;
                case WILDCARD:
                    {
                        int LA33_4 = input.LA(2);
                        if ((LA33_4 == DOWN))
                        {
                            alt33 = 4;
                        }
                        else if (((LA33_4 >= UP && LA33_4 <= ACTION) || LA33_4 == LEXER_CHAR_SET || LA33_4 == NOT || LA33_4 == RANGE || LA33_4 == RULE_REF || LA33_4 == SEMPRED || LA33_4 == STRING_LITERAL || LA33_4 == TOKEN_REF || (LA33_4 >= BLOCK && LA33_4 <= CLOSURE) || LA33_4 == EPSILON || (LA33_4 >= OPTIONAL && LA33_4 <= POSITIVE_CLOSURE) || (LA33_4 >= SET && LA33_4 <= WILDCARD)))
                        {
                            alt33 = 5;
                        }

                        else
                        {
                            int nvaeMark = input.Mark();
                            try
                            {
                                input.Consume();
                                NoViableAltException nvae2 =
                                    new NoViableAltException("", 33, 4, input);
                                throw nvae2;
                            }
                            finally
                            {
                                input.Rewind(nvaeMark);
                            }
                        }

                    }
                    break;
                case LEXER_CHAR_SET:
                    {
                        alt33 = 6;
                    }
                    break;
                case RANGE:
                    {
                        alt33 = 7;
                    }
                    break;
                case RULE_REF:
                    {
                        alt33 = 8;
                    }
                    break;
                default:
                    NoViableAltException nvae =
                        new NoViableAltException("", 33, 0, input);
                    throw nvae;
            }
            switch (alt33)
            {
                case 1:
                    // org\\antlr\\v4\\parse\\g:760:9: terminal
                    {
                        PushFollow(FOLLOW_terminal_in_lexerAtom1579);
                        terminal();
                        state._fsp--;

                    }
                    break;
                case 2:
                    // org\\antlr\\v4\\parse\\g:761:9: ^( NOT blockSet )
                    {
                        Match(input, NOT, FOLLOW_NOT_in_lexerAtom1590);
                        Match(input, Token.DOWN, null);
                        PushFollow(FOLLOW_blockSet_in_lexerAtom1592);
                        blockSet();
                        state._fsp--;

                        Match(input, Token.UP, null);

                    }
                    break;
                case 3:
                    // org\\antlr\\v4\\parse\\g:762:9: blockSet
                    {
                        PushFollow(FOLLOW_blockSet_in_lexerAtom1603);
                        blockSet();
                        state._fsp--;

                    }
                    break;
                case 4:
                    // org\\antlr\\v4\\parse\\g:763:9: ^( WILDCARD elementOptions )
                    {
                        Match(input, WILDCARD, FOLLOW_WILDCARD_in_lexerAtom1614);
                        Match(input, Token.DOWN, null);
                        PushFollow(FOLLOW_elementOptions_in_lexerAtom1616);
                        elementOptions();
                        state._fsp--;

                        Match(input, Token.UP, null);

                    }
                    break;
                case 5:
                    // org\\antlr\\v4\\parse\\g:764:9: WILDCARD
                    {
                        Match(input, WILDCARD, FOLLOW_WILDCARD_in_lexerAtom1627);
                    }
                    break;
                case 6:
                    // org\\antlr\\v4\\parse\\g:765:7: LEXER_CHAR_SET
                    {
                        Match(input, LEXER_CHAR_SET, FOLLOW_LEXER_CHAR_SET_in_lexerAtom1635);
                    }
                    break;
                case 7:
                    // org\\antlr\\v4\\parse\\g:766:9: range
                    {
                        PushFollow(FOLLOW_range_in_lexerAtom1645);
                        range();
                        state._fsp--;

                    }
                    break;
                case 8:
                    // org\\antlr\\v4\\parse\\g:767:9: ruleref
                    {
                        PushFollow(FOLLOW_ruleref_in_lexerAtom1655);
                        ruleref();
                        state._fsp--;

                    }
                    break;

            }

            exitLexerAtom(((GrammarAST)retval.start));

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
    // $ANTLR end "lexerAtom"


    public class actionElement_return : TreeRuleReturnScope
    {
    };


    // $ANTLR start "actionElement"
    // org\\antlr\\v4\\parse\\g:770:1: actionElement : ( ACTION | ^( ACTION elementOptions ) | SEMPRED | ^( SEMPRED elementOptions ) );
    public actionElement_return actionElement()
    {
        actionElement_return retval = new actionElement_return();
        retval.start = input.LT(1);


        enterActionElement(((GrammarAST)retval.start));

        try
        {
            // org\\antlr\\v4\\parse\\g:777:2: ( ACTION | ^( ACTION elementOptions ) | SEMPRED | ^( SEMPRED elementOptions ) )
            int alt34 = 4;
            int LA34_0 = input.LA(1);
            if ((LA34_0 == ACTION))
            {
                int LA34_1 = input.LA(2);
                if ((LA34_1 == DOWN))
                {
                    alt34 = 2;
                }
                else if ((LA34_1 == EOF))
                {
                    alt34 = 1;
                }

                else
                {
                    int nvaeMark = input.Mark();
                    try
                    {
                        input.Consume();
                        NoViableAltException nvae =
                            new NoViableAltException("", 34, 1, input);
                        throw nvae;
                    }
                    finally
                    {
                        input.Rewind(nvaeMark);
                    }
                }

            }
            else if ((LA34_0 == SEMPRED))
            {
                int LA34_2 = input.LA(2);
                if ((LA34_2 == DOWN))
                {
                    alt34 = 4;
                }
                else if ((LA34_2 == EOF))
                {
                    alt34 = 3;
                }

                else
                {
                    int nvaeMark = input.Mark();
                    try
                    {
                        input.Consume();
                        NoViableAltException nvae =
                            new NoViableAltException("", 34, 2, input);
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
                    new NoViableAltException("", 34, 0, input);
                throw nvae;
            }

            switch (alt34)
            {
                case 1:
                    // org\\antlr\\v4\\parse\\g:777:4: ACTION
                    {
                        Match(input, ACTION, FOLLOW_ACTION_in_actionElement1679);
                    }
                    break;
                case 2:
                    // org\\antlr\\v4\\parse\\g:778:6: ^( ACTION elementOptions )
                    {
                        Match(input, ACTION, FOLLOW_ACTION_in_actionElement1687);
                        Match(input, Token.DOWN, null);
                        PushFollow(FOLLOW_elementOptions_in_actionElement1689);
                        elementOptions();
                        state._fsp--;

                        Match(input, Token.UP, null);

                    }
                    break;
                case 3:
                    // org\\antlr\\v4\\parse\\g:779:6: SEMPRED
                    {
                        Match(input, SEMPRED, FOLLOW_SEMPRED_in_actionElement1697);
                    }
                    break;
                case 4:
                    // org\\antlr\\v4\\parse\\g:780:6: ^( SEMPRED elementOptions )
                    {
                        Match(input, SEMPRED, FOLLOW_SEMPRED_in_actionElement1705);
                        Match(input, Token.DOWN, null);
                        PushFollow(FOLLOW_elementOptions_in_actionElement1707);
                        elementOptions();
                        state._fsp--;

                        Match(input, Token.UP, null);

                    }
                    break;

            }

            exitActionElement(((GrammarAST)retval.start));

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
    // $ANTLR end "actionElement"


    public class alternative_return : TreeRuleReturnScope
    {
    };


    // $ANTLR start "alternative"
    // org\\antlr\\v4\\parse\\g:783:1: alternative : ( ^( ALT ( elementOptions )? ( element )+ ) | ^( ALT ( elementOptions )? EPSILON ) );
    public alternative_return alternative()
    {
        alternative_return retval = new alternative_return();
        retval.start = input.LT(1);


        enterAlternative((AltAST)((GrammarAST)retval.start));
        discoverAlt((AltAST)((GrammarAST)retval.start));

        try
        {
            // org\\antlr\\v4\\parse\\g:792:2: ( ^( ALT ( elementOptions )? ( element )+ ) | ^( ALT ( elementOptions )? EPSILON ) )
            int alt38 = 2;
            alt38 = dfa38.Predict(input);
            switch (alt38)
            {
                case 1:
                    // org\\antlr\\v4\\parse\\g:792:4: ^( ALT ( elementOptions )? ( element )+ )
                    {
                        Match(input, ALT, FOLLOW_ALT_in_alternative1730);
                        Match(input, Token.DOWN, null);
                        // org\\antlr\\v4\\parse\\g:792:10: ( elementOptions )?
                        int alt35 = 2;
                        int LA35_0 = input.LA(1);
                        if ((LA35_0 == ELEMENT_OPTIONS))
                        {
                            alt35 = 1;
                        }
                        switch (alt35)
                        {
                            case 1:
                                // org\\antlr\\v4\\parse\\g:792:10: elementOptions
                                {
                                    PushFollow(FOLLOW_elementOptions_in_alternative1732);
                                    elementOptions();
                                    state._fsp--;

                                }
                                break;

                        }

                        // org\\antlr\\v4\\parse\\g:792:26: ( element )+
                        int cnt36 = 0;
                    loop36:
                        while (true)
                        {
                            int alt36 = 2;
                            int LA36_0 = input.LA(1);
                            if ((LA36_0 == ACTION || LA36_0 == ASSIGN || LA36_0 == DOT || LA36_0 == NOT || LA36_0 == PLUS_ASSIGN || LA36_0 == RANGE || LA36_0 == RULE_REF || LA36_0 == SEMPRED || LA36_0 == STRING_LITERAL || LA36_0 == TOKEN_REF || (LA36_0 >= BLOCK && LA36_0 <= CLOSURE) || (LA36_0 >= OPTIONAL && LA36_0 <= POSITIVE_CLOSURE) || (LA36_0 >= SET && LA36_0 <= WILDCARD)))
                            {
                                alt36 = 1;
                            }

                            switch (alt36)
                            {
                                case 1:
                                    // org\\antlr\\v4\\parse\\g:792:26: element
                                    {
                                        PushFollow(FOLLOW_element_in_alternative1735);
                                        element();
                                        state._fsp--;

                                    }
                                    break;

                                default:
                                    if (cnt36 >= 1) goto exit36;// break loop36;
                                    EarlyExitException eee = new EarlyExitException(36, input);
                                    throw eee;
                            }
                            cnt36++;
                        }
                    exit36:
                        Match(input, Token.UP, null);

                    }
                    break;
                case 2:
                    // org\\antlr\\v4\\parse\\g:793:4: ^( ALT ( elementOptions )? EPSILON )
                    {
                        Match(input, ALT, FOLLOW_ALT_in_alternative1743);
                        Match(input, Token.DOWN, null);
                        // org\\antlr\\v4\\parse\\g:793:10: ( elementOptions )?
                        int alt37 = 2;
                        int LA37_0 = input.LA(1);
                        if ((LA37_0 == ELEMENT_OPTIONS))
                        {
                            alt37 = 1;
                        }
                        switch (alt37)
                        {
                            case 1:
                                // org\\antlr\\v4\\parse\\g:793:10: elementOptions
                                {
                                    PushFollow(FOLLOW_elementOptions_in_alternative1745);
                                    elementOptions();
                                    state._fsp--;

                                }
                                break;

                        }

                        Match(input, EPSILON, FOLLOW_EPSILON_in_alternative1748);
                        Match(input, Token.UP, null);

                    }
                    break;

            }

            finishAlt((AltAST)((GrammarAST)retval.start));
            exitAlternative((AltAST)((GrammarAST)retval.start));

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


    public class lexerCommand_return : TreeRuleReturnScope
    {
    };


    // $ANTLR start "lexerCommand"
    // org\\antlr\\v4\\parse\\g:796:1: lexerCommand : ( ^( LEXER_ACTION_CALL ID lexerCommandExpr ) | ID );
    public lexerCommand_return lexerCommand()
    {
        lexerCommand_return retval = new lexerCommand_return();
        retval.start = input.LT(1);

        GrammarAST ID25 = null;
        GrammarAST ID27 = null;
        TreeRuleReturnScope lexerCommandExpr26 = null;


        enterLexerCommand(((GrammarAST)retval.start));

        try
        {
            // org\\antlr\\v4\\parse\\g:803:2: ( ^( LEXER_ACTION_CALL ID lexerCommandExpr ) | ID )
            int alt39 = 2;
            int LA39_0 = input.LA(1);
            if ((LA39_0 == LEXER_ACTION_CALL))
            {
                alt39 = 1;
            }
            else if ((LA39_0 == ID))
            {
                alt39 = 2;
            }

            else
            {
                NoViableAltException nvae =
                    new NoViableAltException("", 39, 0, input);
                throw nvae;
            }

            switch (alt39)
            {
                case 1:
                    // org\\antlr\\v4\\parse\\g:803:4: ^( LEXER_ACTION_CALL ID lexerCommandExpr )
                    {
                        Match(input, LEXER_ACTION_CALL, FOLLOW_LEXER_ACTION_CALL_in_lexerCommand1774);
                        Match(input, Token.DOWN, null);
                        ID25 = (GrammarAST)Match(input, ID, FOLLOW_ID_in_lexerCommand1776);
                        PushFollow(FOLLOW_lexerCommandExpr_in_lexerCommand1778);
                        lexerCommandExpr26 = lexerCommandExpr();
                        state._fsp--;

                        Match(input, Token.UP, null);

                        lexerCallCommand(currentOuterAltNumber, ID25, (lexerCommandExpr26 != null ? ((GrammarAST)lexerCommandExpr26.start) : null));
                    }
                    break;
                case 2:
                    // org\\antlr\\v4\\parse\\g:805:4: ID
                    {
                        ID27 = (GrammarAST)Match(input, ID, FOLLOW_ID_in_lexerCommand1794);
                        lexerCommand(currentOuterAltNumber, ID27);
                    }
                    break;

            }

            exitLexerCommand(((GrammarAST)retval.start));

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
    // $ANTLR end "lexerCommand"


    public class lexerCommandExpr_return : TreeRuleReturnScope
    {
    };


    // $ANTLR start "lexerCommandExpr"
    // org\\antlr\\v4\\parse\\g:809:1: lexerCommandExpr : ( ID | INT );
    public lexerCommandExpr_return lexerCommandExpr()
    {
        lexerCommandExpr_return retval = new lexerCommandExpr_return();
        retval.start = input.LT(1);


        enterLexerCommandExpr(((GrammarAST)retval.start));

        try
        {
            // org\\antlr\\v4\\parse\\g:816:2: ( ID | INT )
            // org\\antlr\\v4\\parse\\g:
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


            exitLexerCommandExpr(((GrammarAST)retval.start));

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
    };


    // $ANTLR start "element"
    // org\\antlr\\v4\\parse\\g:820:1: element : ( labeledElement | atom | subrule | ACTION | SEMPRED | ^( ACTION elementOptions ) | ^( SEMPRED elementOptions ) | range | ^( NOT blockSet ) | ^( NOT block ) );
    public element_return element()
    {
        element_return retval = new element_return();
        retval.start = input.LT(1);

        GrammarAST ACTION28 = null;
        GrammarAST SEMPRED29 = null;
        GrammarAST ACTION30 = null;
        GrammarAST SEMPRED31 = null;


        enterElement(((GrammarAST)retval.start));

        try
        {
            // org\\antlr\\v4\\parse\\g:827:2: ( labeledElement | atom | subrule | ACTION | SEMPRED | ^( ACTION elementOptions ) | ^( SEMPRED elementOptions ) | range | ^( NOT blockSet ) | ^( NOT block ) )
            int alt40 = 10;
            switch (input.LA(1))
            {
                case ASSIGN:
                case PLUS_ASSIGN:
                    {
                        alt40 = 1;
                    }
                    break;
                case DOT:
                case RULE_REF:
                case STRING_LITERAL:
                case TOKEN_REF:
                case SET:
                case WILDCARD:
                    {
                        alt40 = 2;
                    }
                    break;
                case BLOCK:
                case CLOSURE:
                case OPTIONAL:
                case POSITIVE_CLOSURE:
                    {
                        alt40 = 3;
                    }
                    break;
                case ACTION:
                    {
                        int LA40_4 = input.LA(2);
                        if ((LA40_4 == DOWN))
                        {
                            alt40 = 6;
                        }
                        else if (((LA40_4 >= UP && LA40_4 <= ACTION) || LA40_4 == ASSIGN || LA40_4 == DOT || LA40_4 == NOT || LA40_4 == PLUS_ASSIGN || LA40_4 == RANGE || LA40_4 == RULE_REF || LA40_4 == SEMPRED || LA40_4 == STRING_LITERAL || LA40_4 == TOKEN_REF || (LA40_4 >= BLOCK && LA40_4 <= CLOSURE) || (LA40_4 >= OPTIONAL && LA40_4 <= POSITIVE_CLOSURE) || (LA40_4 >= SET && LA40_4 <= WILDCARD)))
                        {
                            alt40 = 4;
                        }

                        else
                        {
                            int nvaeMark = input.Mark();
                            try
                            {
                                input.Consume();
                                NoViableAltException nvae2 =
                                    new NoViableAltException("", 40, 4, input);
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
                        int LA40_5 = input.LA(2);
                        if ((LA40_5 == DOWN))
                        {
                            alt40 = 7;
                        }
                        else if (((LA40_5 >= UP && LA40_5 <= ACTION) || LA40_5 == ASSIGN || LA40_5 == DOT || LA40_5 == NOT || LA40_5 == PLUS_ASSIGN || LA40_5 == RANGE || LA40_5 == RULE_REF || LA40_5 == SEMPRED || LA40_5 == STRING_LITERAL || LA40_5 == TOKEN_REF || (LA40_5 >= BLOCK && LA40_5 <= CLOSURE) || (LA40_5 >= OPTIONAL && LA40_5 <= POSITIVE_CLOSURE) || (LA40_5 >= SET && LA40_5 <= WILDCARD)))
                        {
                            alt40 = 5;
                        }

                        else
                        {
                            int nvaeMark = input.Mark();
                            try
                            {
                                input.Consume();
                                NoViableAltException nvae2 =
                                    new NoViableAltException("", 40, 5, input);
                                throw nvae2;
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
                        alt40 = 8;
                    }
                    break;
                case NOT:
                    {
                        int LA40_7 = input.LA(2);
                        if ((LA40_7 == DOWN))
                        {
                            int LA40_12 = input.LA(3);
                            if ((LA40_12 == SET))
                            {
                                alt40 = 9;
                            }
                            else if ((LA40_12 == BLOCK))
                            {
                                alt40 = 10;
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
                                        new NoViableAltException("", 40, 12, input);
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
                                NoViableAltException nvae2 =
                                    new NoViableAltException("", 40, 7, input);
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
                    NoViableAltException nvae =
                        new NoViableAltException("", 40, 0, input);
                    throw nvae;
            }
            switch (alt40)
            {
                case 1:
                    // org\\antlr\\v4\\parse\\g:827:4: labeledElement
                    {
                        PushFollow(FOLLOW_labeledElement_in_element1851);
                        labeledElement();
                        state._fsp--;

                    }
                    break;
                case 2:
                    // org\\antlr\\v4\\parse\\g:828:4: atom
                    {
                        PushFollow(FOLLOW_atom_in_element1856);
                        atom();
                        state._fsp--;

                    }
                    break;
                case 3:
                    // org\\antlr\\v4\\parse\\g:829:4: subrule
                    {
                        PushFollow(FOLLOW_subrule_in_element1861);
                        subrule();
                        state._fsp--;

                    }
                    break;
                case 4:
                    // org\\antlr\\v4\\parse\\g:830:6: ACTION
                    {
                        ACTION28 = (GrammarAST)Match(input, ACTION, FOLLOW_ACTION_in_element1868);
                        actionInAlt((ActionAST)ACTION28);
                    }
                    break;
                case 5:
                    // org\\antlr\\v4\\parse\\g:831:6: SEMPRED
                    {
                        SEMPRED29 = (GrammarAST)Match(input, SEMPRED, FOLLOW_SEMPRED_in_element1882);
                        sempredInAlt((PredAST)SEMPRED29);
                    }
                    break;
                case 6:
                    // org\\antlr\\v4\\parse\\g:832:6: ^( ACTION elementOptions )
                    {
                        ACTION30 = (GrammarAST)Match(input, ACTION, FOLLOW_ACTION_in_element1897);
                        Match(input, Token.DOWN, null);
                        PushFollow(FOLLOW_elementOptions_in_element1899);
                        elementOptions();
                        state._fsp--;

                        Match(input, Token.UP, null);

                        actionInAlt((ActionAST)ACTION30);
                    }
                    break;
                case 7:
                    // org\\antlr\\v4\\parse\\g:833:6: ^( SEMPRED elementOptions )
                    {
                        SEMPRED31 = (GrammarAST)Match(input, SEMPRED, FOLLOW_SEMPRED_in_element1910);
                        Match(input, Token.DOWN, null);
                        PushFollow(FOLLOW_elementOptions_in_element1912);
                        elementOptions();
                        state._fsp--;

                        Match(input, Token.UP, null);

                        sempredInAlt((PredAST)SEMPRED31);
                    }
                    break;
                case 8:
                    // org\\antlr\\v4\\parse\\g:834:4: range
                    {
                        PushFollow(FOLLOW_range_in_element1920);
                        range();
                        state._fsp--;

                    }
                    break;
                case 9:
                    // org\\antlr\\v4\\parse\\g:835:4: ^( NOT blockSet )
                    {
                        Match(input, NOT, FOLLOW_NOT_in_element1926);
                        Match(input, Token.DOWN, null);
                        PushFollow(FOLLOW_blockSet_in_element1928);
                        blockSet();
                        state._fsp--;

                        Match(input, Token.UP, null);

                    }
                    break;
                case 10:
                    // org\\antlr\\v4\\parse\\g:836:4: ^( NOT block )
                    {
                        Match(input, NOT, FOLLOW_NOT_in_element1935);
                        Match(input, Token.DOWN, null);
                        PushFollow(FOLLOW_block_in_element1937);
                        block();
                        state._fsp--;

                        Match(input, Token.UP, null);

                    }
                    break;

            }

            exitElement(((GrammarAST)retval.start));

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


    public class astOperand_return : TreeRuleReturnScope
    {
    };


    // $ANTLR start "astOperand"
    // org\\antlr\\v4\\parse\\g:839:1: astOperand : ( atom | ^( NOT blockSet ) | ^( NOT block ) );
    public astOperand_return astOperand()
    {
        astOperand_return retval = new astOperand_return();
        retval.start = input.LT(1);


        enterAstOperand(((GrammarAST)retval.start));

        try
        {
            // org\\antlr\\v4\\parse\\g:846:2: ( atom | ^( NOT blockSet ) | ^( NOT block ) )
            int alt41 = 3;
            int LA41_0 = input.LA(1);
            if ((LA41_0 == DOT || LA41_0 == RULE_REF || LA41_0 == STRING_LITERAL || LA41_0 == TOKEN_REF || (LA41_0 >= SET && LA41_0 <= WILDCARD)))
            {
                alt41 = 1;
            }
            else if ((LA41_0 == NOT))
            {
                int LA41_2 = input.LA(2);
                if ((LA41_2 == DOWN))
                {
                    int LA41_3 = input.LA(3);
                    if ((LA41_3 == SET))
                    {
                        alt41 = 2;
                    }
                    else if ((LA41_3 == BLOCK))
                    {
                        alt41 = 3;
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
                                new NoViableAltException("", 41, 3, input);
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
                            new NoViableAltException("", 41, 2, input);
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
                    new NoViableAltException("", 41, 0, input);
                throw nvae;
            }

            switch (alt41)
            {
                case 1:
                    // org\\antlr\\v4\\parse\\g:846:4: atom
                    {
                        PushFollow(FOLLOW_atom_in_astOperand1959);
                        atom();
                        state._fsp--;

                    }
                    break;
                case 2:
                    // org\\antlr\\v4\\parse\\g:847:4: ^( NOT blockSet )
                    {
                        Match(input, NOT, FOLLOW_NOT_in_astOperand1965);
                        Match(input, Token.DOWN, null);
                        PushFollow(FOLLOW_blockSet_in_astOperand1967);
                        blockSet();
                        state._fsp--;

                        Match(input, Token.UP, null);

                    }
                    break;
                case 3:
                    // org\\antlr\\v4\\parse\\g:848:4: ^( NOT block )
                    {
                        Match(input, NOT, FOLLOW_NOT_in_astOperand1974);
                        Match(input, Token.DOWN, null);
                        PushFollow(FOLLOW_block_in_astOperand1976);
                        block();
                        state._fsp--;

                        Match(input, Token.UP, null);

                    }
                    break;

            }

            exitAstOperand(((GrammarAST)retval.start));

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
    // $ANTLR end "astOperand"


    public class labeledElement_return : TreeRuleReturnScope
    {
    };


    // $ANTLR start "labeledElement"
    // org\\antlr\\v4\\parse\\g:851:1: labeledElement : ^( ( ASSIGN | PLUS_ASSIGN ) ID element ) ;
    public labeledElement_return labeledElement()
    {
        labeledElement_return retval = new labeledElement_return();
        retval.start = input.LT(1);

        GrammarAST ID32 = null;
        TreeRuleReturnScope element33 = null;


        enterLabeledElement(((GrammarAST)retval.start));

        try
        {
            // org\\antlr\\v4\\parse\\g:858:2: ( ^( ( ASSIGN | PLUS_ASSIGN ) ID element ) )
            // org\\antlr\\v4\\parse\\g:858:4: ^( ( ASSIGN | PLUS_ASSIGN ) ID element )
            {
                if (input.LA(1) == ASSIGN || input.LA(1) == PLUS_ASSIGN)
                {
                    input.Consume();
                    state.errorRecovery = false;
                }
                else
                {
                    MismatchedSetException mse = new MismatchedSetException(null, input);
                    throw mse;
                }
                Match(input, Token.DOWN, null);
                ID32 = (GrammarAST)Match(input, ID, FOLLOW_ID_in_labeledElement2005);
                PushFollow(FOLLOW_element_in_labeledElement2007);
                element33 = element();
                state._fsp--;

                Match(input, Token.UP, null);

                label(((GrammarAST)retval.start), ID32, (element33 != null ? ((GrammarAST)element33.start) : null));
            }


            exitLabeledElement(((GrammarAST)retval.start));

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
    // $ANTLR end "labeledElement"


    public class subrule_return : TreeRuleReturnScope
    {
    };


    // $ANTLR start "subrule"
    // org\\antlr\\v4\\parse\\g:861:1: subrule : ( ^( blockSuffix block ) | block );
    public subrule_return subrule()
    {
        subrule_return retval = new subrule_return();
        retval.start = input.LT(1);


        enterSubrule(((GrammarAST)retval.start));

        try
        {
            // org\\antlr\\v4\\parse\\g:868:2: ( ^( blockSuffix block ) | block )
            int alt42 = 2;
            int LA42_0 = input.LA(1);
            if ((LA42_0 == CLOSURE || (LA42_0 >= OPTIONAL && LA42_0 <= POSITIVE_CLOSURE)))
            {
                alt42 = 1;
            }
            else if ((LA42_0 == BLOCK))
            {
                alt42 = 2;
            }

            else
            {
                NoViableAltException nvae =
                    new NoViableAltException("", 42, 0, input);
                throw nvae;
            }

            switch (alt42)
            {
                case 1:
                    // org\\antlr\\v4\\parse\\g:868:4: ^( blockSuffix block )
                    {
                        PushFollow(FOLLOW_blockSuffix_in_subrule2032);
                        blockSuffix();
                        state._fsp--;

                        Match(input, Token.DOWN, null);
                        PushFollow(FOLLOW_block_in_subrule2034);
                        block();
                        state._fsp--;

                        Match(input, Token.UP, null);

                    }
                    break;
                case 2:
                    // org\\antlr\\v4\\parse\\g:869:5: block
                    {
                        PushFollow(FOLLOW_block_in_subrule2041);
                        block();
                        state._fsp--;

                    }
                    break;

            }

            exitSubrule(((GrammarAST)retval.start));

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


    public class lexerSubrule_return : TreeRuleReturnScope
    {
    };


    // $ANTLR start "lexerSubrule"
    // org\\antlr\\v4\\parse\\g:872:1: lexerSubrule : ( ^( blockSuffix lexerBlock ) | lexerBlock );
    public lexerSubrule_return lexerSubrule()
    {
        lexerSubrule_return retval = new lexerSubrule_return();
        retval.start = input.LT(1);


        enterLexerSubrule(((GrammarAST)retval.start));

        try
        {
            // org\\antlr\\v4\\parse\\g:879:2: ( ^( blockSuffix lexerBlock ) | lexerBlock )
            int alt43 = 2;
            int LA43_0 = input.LA(1);
            if ((LA43_0 == CLOSURE || (LA43_0 >= OPTIONAL && LA43_0 <= POSITIVE_CLOSURE)))
            {
                alt43 = 1;
            }
            else if ((LA43_0 == BLOCK))
            {
                alt43 = 2;
            }

            else
            {
                NoViableAltException nvae =
                    new NoViableAltException("", 43, 0, input);
                throw nvae;
            }

            switch (alt43)
            {
                case 1:
                    // org\\antlr\\v4\\parse\\g:879:4: ^( blockSuffix lexerBlock )
                    {
                        PushFollow(FOLLOW_blockSuffix_in_lexerSubrule2066);
                        blockSuffix();
                        state._fsp--;

                        Match(input, Token.DOWN, null);
                        PushFollow(FOLLOW_lexerBlock_in_lexerSubrule2068);
                        lexerBlock();
                        state._fsp--;

                        Match(input, Token.UP, null);

                    }
                    break;
                case 2:
                    // org\\antlr\\v4\\parse\\g:880:5: lexerBlock
                    {
                        PushFollow(FOLLOW_lexerBlock_in_lexerSubrule2075);
                        lexerBlock();
                        state._fsp--;

                    }
                    break;

            }

            exitLexerSubrule(((GrammarAST)retval.start));

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
    // $ANTLR end "lexerSubrule"


    public class blockSuffix_return : TreeRuleReturnScope
    {
    };


    // $ANTLR start "blockSuffix"
    // org\\antlr\\v4\\parse\\g:883:1: blockSuffix : ebnfSuffix ;
    public blockSuffix_return blockSuffix()
    {
        blockSuffix_return retval = new blockSuffix_return();
        retval.start = input.LT(1);


        enterBlockSuffix(((GrammarAST)retval.start));

        try
        {
            // org\\antlr\\v4\\parse\\g:890:5: ( ebnfSuffix )
            // org\\antlr\\v4\\parse\\g:890:7: ebnfSuffix
            {
                PushFollow(FOLLOW_ebnfSuffix_in_blockSuffix2102);
                ebnfSuffix();
                state._fsp--;

            }


            exitBlockSuffix(((GrammarAST)retval.start));

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
    // $ANTLR end "blockSuffix"


    public class ebnfSuffix_return : TreeRuleReturnScope
    {
    };


    // $ANTLR start "ebnfSuffix"
    // org\\antlr\\v4\\parse\\g:893:1: ebnfSuffix : ( OPTIONAL | CLOSURE | POSITIVE_CLOSURE );
    public ebnfSuffix_return ebnfSuffix()
    {
        ebnfSuffix_return retval = new ebnfSuffix_return();
        retval.start = input.LT(1);


        enterEbnfSuffix(((GrammarAST)retval.start));

        try
        {
            // org\\antlr\\v4\\parse\\g:900:2: ( OPTIONAL | CLOSURE | POSITIVE_CLOSURE )
            // org\\antlr\\v4\\parse\\g:
            {
                if (input.LA(1) == CLOSURE || (input.LA(1) >= OPTIONAL && input.LA(1) <= POSITIVE_CLOSURE))
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


            exitEbnfSuffix(((GrammarAST)retval.start));

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


    public class atom_return : TreeRuleReturnScope
    {
    };


    // $ANTLR start "atom"
    // org\\antlr\\v4\\parse\\g:905:1: atom : ( ^( DOT ID terminal ) | ^( DOT ID ruleref ) | ^( WILDCARD elementOptions ) | WILDCARD | terminal | blockSet | ruleref );
    public atom_return atom()
    {
        atom_return retval = new atom_return();
        retval.start = input.LT(1);

        GrammarAST WILDCARD34 = null;
        GrammarAST WILDCARD35 = null;


        enterAtom(((GrammarAST)retval.start));

        try
        {
            // org\\antlr\\v4\\parse\\g:912:2: ( ^( DOT ID terminal ) | ^( DOT ID ruleref ) | ^( WILDCARD elementOptions ) | WILDCARD | terminal | blockSet | ruleref )
            int alt44 = 7;
            switch (input.LA(1))
            {
                case DOT:
                    {
                        int LA44_1 = input.LA(2);
                        if ((LA44_1 == DOWN))
                        {
                            int LA44_6 = input.LA(3);
                            if ((LA44_6 == ID))
                            {
                                int LA44_9 = input.LA(4);
                                if ((LA44_9 == STRING_LITERAL || LA44_9 == TOKEN_REF))
                                {
                                    alt44 = 1;
                                }
                                else if ((LA44_9 == RULE_REF))
                                {
                                    alt44 = 2;
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
                                        NoViableAltException nvae2 =
                                            new NoViableAltException("", 44, 9, input);
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
                                    for (int nvaeConsume = 0; nvaeConsume < 3 - 1; nvaeConsume++)
                                    {
                                        input.Consume();
                                    }
                                    NoViableAltException nvae2 =
                                        new NoViableAltException("", 44, 6, input);
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
                                NoViableAltException nvae2 =
                                    new NoViableAltException("", 44, 1, input);
                                throw nvae2;
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
                        int LA44_2 = input.LA(2);
                        if ((LA44_2 == DOWN))
                        {
                            alt44 = 3;
                        }
                        else if ((LA44_2 == EOF || (LA44_2 >= UP && LA44_2 <= ACTION) || LA44_2 == ASSIGN || LA44_2 == DOT || LA44_2 == NOT || LA44_2 == PLUS_ASSIGN || LA44_2 == RANGE || LA44_2 == RULE_REF || LA44_2 == SEMPRED || LA44_2 == STRING_LITERAL || LA44_2 == TOKEN_REF || (LA44_2 >= BLOCK && LA44_2 <= CLOSURE) || (LA44_2 >= OPTIONAL && LA44_2 <= POSITIVE_CLOSURE) || (LA44_2 >= SET && LA44_2 <= WILDCARD)))
                        {
                            alt44 = 4;
                        }

                        else
                        {
                            int nvaeMark = input.Mark();
                            try
                            {
                                input.Consume();
                                NoViableAltException nvae2 =
                                    new NoViableAltException("", 44, 2, input);
                                throw nvae2;
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
                        alt44 = 5;
                    }
                    break;
                case SET:
                    {
                        alt44 = 6;
                    }
                    break;
                case RULE_REF:
                    {
                        alt44 = 7;
                    }
                    break;
                default:
                    NoViableAltException nvae =
                        new NoViableAltException("", 44, 0, input);
                    throw nvae;
            }
            switch (alt44)
            {
                case 1:
                    // org\\antlr\\v4\\parse\\g:912:4: ^( DOT ID terminal )
                    {
                        Match(input, DOT, FOLLOW_DOT_in_atom2163);
                        Match(input, Token.DOWN, null);
                        Match(input, ID, FOLLOW_ID_in_atom2165);
                        PushFollow(FOLLOW_terminal_in_atom2167);
                        terminal();
                        state._fsp--;

                        Match(input, Token.UP, null);

                    }
                    break;
                case 2:
                    // org\\antlr\\v4\\parse\\g:913:4: ^( DOT ID ruleref )
                    {
                        Match(input, DOT, FOLLOW_DOT_in_atom2174);
                        Match(input, Token.DOWN, null);
                        Match(input, ID, FOLLOW_ID_in_atom2176);
                        PushFollow(FOLLOW_ruleref_in_atom2178);
                        ruleref();
                        state._fsp--;

                        Match(input, Token.UP, null);

                    }
                    break;
                case 3:
                    // org\\antlr\\v4\\parse\\g:914:7: ^( WILDCARD elementOptions )
                    {
                        WILDCARD34 = (GrammarAST)Match(input, WILDCARD, FOLLOW_WILDCARD_in_atom2188);
                        Match(input, Token.DOWN, null);
                        PushFollow(FOLLOW_elementOptions_in_atom2190);
                        elementOptions();
                        state._fsp--;

                        Match(input, Token.UP, null);

                        wildcardRef(WILDCARD34);
                    }
                    break;
                case 4:
                    // org\\antlr\\v4\\parse\\g:915:7: WILDCARD
                    {
                        WILDCARD35 = (GrammarAST)Match(input, WILDCARD, FOLLOW_WILDCARD_in_atom2201);
                        wildcardRef(WILDCARD35);
                    }
                    break;
                case 5:
                    // org\\antlr\\v4\\parse\\g:916:9: terminal
                    {
                        PushFollow(FOLLOW_terminal_in_atom2217);
                        terminal();
                        state._fsp--;

                    }
                    break;
                case 6:
                    // org\\antlr\\v4\\parse\\g:917:7: blockSet
                    {
                        PushFollow(FOLLOW_blockSet_in_atom2225);
                        blockSet();
                        state._fsp--;

                    }
                    break;
                case 7:
                    // org\\antlr\\v4\\parse\\g:918:9: ruleref
                    {
                        PushFollow(FOLLOW_ruleref_in_atom2235);
                        ruleref();
                        state._fsp--;

                    }
                    break;

            }

            exitAtom(((GrammarAST)retval.start));

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


    public class blockSet_return : TreeRuleReturnScope
    {
    };


    // $ANTLR start "blockSet"
    // org\\antlr\\v4\\parse\\g:921:1: blockSet : ^( SET ( setElement )+ ) ;
    public blockSet_return blockSet()
    {
        blockSet_return retval = new blockSet_return();
        retval.start = input.LT(1);


        enterBlockSet(((GrammarAST)retval.start));

        try
        {
            // org\\antlr\\v4\\parse\\g:928:2: ( ^( SET ( setElement )+ ) )
            // org\\antlr\\v4\\parse\\g:928:4: ^( SET ( setElement )+ )
            {
                Match(input, SET, FOLLOW_SET_in_blockSet2260);
                Match(input, Token.DOWN, null);
                // org\\antlr\\v4\\parse\\g:928:10: ( setElement )+
                int cnt45 = 0;
            loop45:
                while (true)
                {
                    int alt45 = 2;
                    int LA45_0 = input.LA(1);
                    if ((LA45_0 == LEXER_CHAR_SET || LA45_0 == RANGE || LA45_0 == STRING_LITERAL || LA45_0 == TOKEN_REF))
                    {
                        alt45 = 1;
                    }

                    switch (alt45)
                    {
                        case 1:
                            // org\\antlr\\v4\\parse\\g:928:10: setElement
                            {
                                PushFollow(FOLLOW_setElement_in_blockSet2262);
                                setElement();
                                state._fsp--;

                            }
                            break;

                        default:
                            if (cnt45 >= 1) goto exit45;// break loop45;
                            EarlyExitException eee = new EarlyExitException(45, input);
                            throw eee;
                    }
                    cnt45++;
                }
            exit45:
                Match(input, Token.UP, null);

            }


            exitBlockSet(((GrammarAST)retval.start));

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
    // org\\antlr\\v4\\parse\\g:931:1: setElement : ( ^( STRING_LITERAL elementOptions ) | ^( TOKEN_REF elementOptions ) | STRING_LITERAL | TOKEN_REF | ^( RANGE a= STRING_LITERAL b= STRING_LITERAL ) | LEXER_CHAR_SET );
    public setElement_return setElement()
    {
        setElement_return retval = new setElement_return();
        retval.start = input.LT(1);

        GrammarAST a = null;
        GrammarAST b = null;
        GrammarAST STRING_LITERAL36 = null;
        GrammarAST TOKEN_REF37 = null;
        GrammarAST STRING_LITERAL38 = null;
        GrammarAST TOKEN_REF39 = null;


        enterSetElement(((GrammarAST)retval.start));

        try
        {
            // org\\antlr\\v4\\parse\\g:938:2: ( ^( STRING_LITERAL elementOptions ) | ^( TOKEN_REF elementOptions ) | STRING_LITERAL | TOKEN_REF | ^( RANGE a= STRING_LITERAL b= STRING_LITERAL ) | LEXER_CHAR_SET )
            int alt46 = 6;
            switch (input.LA(1))
            {
                case STRING_LITERAL:
                    {
                        int LA46_1 = input.LA(2);
                        if ((LA46_1 == DOWN))
                        {
                            alt46 = 1;
                        }
                        else if ((LA46_1 == UP || LA46_1 == LEXER_CHAR_SET || LA46_1 == RANGE || LA46_1 == STRING_LITERAL || LA46_1 == TOKEN_REF))
                        {
                            alt46 = 3;
                        }

                        else
                        {
                            int nvaeMark = input.Mark();
                            try
                            {
                                input.Consume();
                                NoViableAltException nvae2 =
                                    new NoViableAltException("", 46, 1, input);
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
                        int LA46_2 = input.LA(2);
                        if ((LA46_2 == DOWN))
                        {
                            alt46 = 2;
                        }
                        else if ((LA46_2 == UP || LA46_2 == LEXER_CHAR_SET || LA46_2 == RANGE || LA46_2 == STRING_LITERAL || LA46_2 == TOKEN_REF))
                        {
                            alt46 = 4;
                        }

                        else
                        {
                            int nvaeMark = input.Mark();
                            try
                            {
                                input.Consume();
                                NoViableAltException nvae2 =
                                    new NoViableAltException("", 46, 2, input);
                                throw nvae2;
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
                        alt46 = 5;
                    }
                    break;
                case LEXER_CHAR_SET:
                    {
                        alt46 = 6;
                    }
                    break;
                default:
                    NoViableAltException nvae =
                        new NoViableAltException("", 46, 0, input);
                    throw nvae;
            }
            switch (alt46)
            {
                case 1:
                    // org\\antlr\\v4\\parse\\g:938:4: ^( STRING_LITERAL elementOptions )
                    {
                        STRING_LITERAL36 = (GrammarAST)Match(input, STRING_LITERAL, FOLLOW_STRING_LITERAL_in_setElement2286);
                        Match(input, Token.DOWN, null);
                        PushFollow(FOLLOW_elementOptions_in_setElement2288);
                        elementOptions();
                        state._fsp--;

                        Match(input, Token.UP, null);

                        stringRef((TerminalAST)STRING_LITERAL36);
                    }
                    break;
                case 2:
                    // org\\antlr\\v4\\parse\\g:939:4: ^( TOKEN_REF elementOptions )
                    {
                        TOKEN_REF37 = (GrammarAST)Match(input, TOKEN_REF, FOLLOW_TOKEN_REF_in_setElement2300);
                        Match(input, Token.DOWN, null);
                        PushFollow(FOLLOW_elementOptions_in_setElement2302);
                        elementOptions();
                        state._fsp--;

                        Match(input, Token.UP, null);

                        tokenRef((TerminalAST)TOKEN_REF37);
                    }
                    break;
                case 3:
                    // org\\antlr\\v4\\parse\\g:940:4: STRING_LITERAL
                    {
                        STRING_LITERAL38 = (GrammarAST)Match(input, STRING_LITERAL, FOLLOW_STRING_LITERAL_in_setElement2312);
                        stringRef((TerminalAST)STRING_LITERAL38);
                    }
                    break;
                case 4:
                    // org\\antlr\\v4\\parse\\g:941:4: TOKEN_REF
                    {
                        TOKEN_REF39 = (GrammarAST)Match(input, TOKEN_REF, FOLLOW_TOKEN_REF_in_setElement2337);
                        tokenRef((TerminalAST)TOKEN_REF39);
                    }
                    break;
                case 5:
                    // org\\antlr\\v4\\parse\\g:942:4: ^( RANGE a= STRING_LITERAL b= STRING_LITERAL )
                    {
                        Match(input, RANGE, FOLLOW_RANGE_in_setElement2366);
                        Match(input, Token.DOWN, null);
                        a = (GrammarAST)Match(input, STRING_LITERAL, FOLLOW_STRING_LITERAL_in_setElement2370);
                        b = (GrammarAST)Match(input, STRING_LITERAL, FOLLOW_STRING_LITERAL_in_setElement2374);
                        Match(input, Token.UP, null);


                        stringRef((TerminalAST)a);
                        stringRef((TerminalAST)b);

                    }
                    break;
                case 6:
                    // org\\antlr\\v4\\parse\\g:947:17: LEXER_CHAR_SET
                    {
                        Match(input, LEXER_CHAR_SET, FOLLOW_LEXER_CHAR_SET_in_setElement2397);
                    }
                    break;

            }

            exitSetElement(((GrammarAST)retval.start));

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


    public class block_return : TreeRuleReturnScope
    {
    };


    // $ANTLR start "block"
    // org\\antlr\\v4\\parse\\g:950:1: block : ^( BLOCK ( optionsSpec )? ( ruleAction )* ( ACTION )? ( alternative )+ ) ;
    public block_return block()
    {
        block_return retval = new block_return();
        retval.start = input.LT(1);


        enterBlock(((GrammarAST)retval.start));

        try
        {
            // org\\antlr\\v4\\parse\\g:957:5: ( ^( BLOCK ( optionsSpec )? ( ruleAction )* ( ACTION )? ( alternative )+ ) )
            // org\\antlr\\v4\\parse\\g:957:7: ^( BLOCK ( optionsSpec )? ( ruleAction )* ( ACTION )? ( alternative )+ )
            {
                Match(input, BLOCK, FOLLOW_BLOCK_in_block2422);
                Match(input, Token.DOWN, null);
                // org\\antlr\\v4\\parse\\g:957:15: ( optionsSpec )?
                int alt47 = 2;
                int LA47_0 = input.LA(1);
                if ((LA47_0 == OPTIONS))
                {
                    alt47 = 1;
                }
                switch (alt47)
                {
                    case 1:
                        // org\\antlr\\v4\\parse\\g:957:15: optionsSpec
                        {
                            PushFollow(FOLLOW_optionsSpec_in_block2424);
                            optionsSpec();
                            state._fsp--;

                        }
                        break;

                }

            // org\\antlr\\v4\\parse\\g:957:28: ( ruleAction )*
            loop48:
                while (true)
                {
                    int alt48 = 2;
                    int LA48_0 = input.LA(1);
                    if ((LA48_0 == AT))
                    {
                        alt48 = 1;
                    }

                    switch (alt48)
                    {
                        case 1:
                            // org\\antlr\\v4\\parse\\g:957:28: ruleAction
                            {
                                PushFollow(FOLLOW_ruleAction_in_block2427);
                                ruleAction();
                                state._fsp--;

                            }
                            break;

                        default:
                            goto exit48;
                            //break loop48;
                    }
                }
            exit48:
                // org\\antlr\\v4\\parse\\g:957:40: ( ACTION )?
                int alt49 = 2;
                int LA49_0 = input.LA(1);
                if ((LA49_0 == ACTION))
                {
                    alt49 = 1;
                }
                switch (alt49)
                {
                    case 1:
                        // org\\antlr\\v4\\parse\\g:957:40: ACTION
                        {
                            Match(input, ACTION, FOLLOW_ACTION_in_block2430);
                        }
                        break;

                }

                // org\\antlr\\v4\\parse\\g:957:48: ( alternative )+
                int cnt50 = 0;
            loop50:
                while (true)
                {
                    int alt50 = 2;
                    int LA50_0 = input.LA(1);
                    if ((LA50_0 == ALT))
                    {
                        alt50 = 1;
                    }

                    switch (alt50)
                    {
                        case 1:
                            // org\\antlr\\v4\\parse\\g:957:48: alternative
                            {
                                PushFollow(FOLLOW_alternative_in_block2433);
                                alternative();
                                state._fsp--;

                            }
                            break;

                        default:
                            if (cnt50 >= 1) goto exit50;//  break loop50;
                            EarlyExitException eee = new EarlyExitException(50, input);
                            throw eee;
                    }
                    cnt50++;
                }
            exit50:
                Match(input, Token.UP, null);

            }


            exitBlock(((GrammarAST)retval.start));

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
    // $ANTLR end "block"


    public class ruleref_return : TreeRuleReturnScope
    {
    };


    // $ANTLR start "ruleref"
    // org\\antlr\\v4\\parse\\g:960:1: ruleref : ^( RULE_REF (arg= ARG_ACTION )? ( elementOptions )? ) ;
    public ruleref_return ruleref()
    {
        ruleref_return retval = new ruleref_return();
        retval.start = input.LT(1);

        GrammarAST arg = null;
        GrammarAST RULE_REF40 = null;


        enterRuleref(((GrammarAST)retval.start));

        try
        {
            // org\\antlr\\v4\\parse\\g:967:5: ( ^( RULE_REF (arg= ARG_ACTION )? ( elementOptions )? ) )
            // org\\antlr\\v4\\parse\\g:967:7: ^( RULE_REF (arg= ARG_ACTION )? ( elementOptions )? )
            {
                RULE_REF40 = (GrammarAST)Match(input, RULE_REF, FOLLOW_RULE_REF_in_ruleref2463);
                if (input.LA(1) == Token.DOWN)
                {
                    Match(input, Token.DOWN, null);
                    // org\\antlr\\v4\\parse\\g:967:21: (arg= ARG_ACTION )?
                    int alt51 = 2;
                    int LA51_0 = input.LA(1);
                    if ((LA51_0 == ARG_ACTION))
                    {
                        alt51 = 1;
                    }
                    switch (alt51)
                    {
                        case 1:
                            // org\\antlr\\v4\\parse\\g:967:21: arg= ARG_ACTION
                            {
                                arg = (GrammarAST)Match(input, ARG_ACTION, FOLLOW_ARG_ACTION_in_ruleref2467);
                            }
                            break;

                    }

                    // org\\antlr\\v4\\parse\\g:967:34: ( elementOptions )?
                    int alt52 = 2;
                    int LA52_0 = input.LA(1);
                    if ((LA52_0 == ELEMENT_OPTIONS))
                    {
                        alt52 = 1;
                    }
                    switch (alt52)
                    {
                        case 1:
                            // org\\antlr\\v4\\parse\\g:967:34: elementOptions
                            {
                                PushFollow(FOLLOW_elementOptions_in_ruleref2470);
                                elementOptions();
                                state._fsp--;

                            }
                            break;

                    }

                    Match(input, Token.UP, null);
                }


                ruleRef(RULE_REF40, (ActionAST)arg);
                if (arg != null) actionInAlt((ActionAST)arg);

            }


            exitRuleref(((GrammarAST)retval.start));

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
    // $ANTLR end "ruleref"


    public class range_return : TreeRuleReturnScope
    {
    };


    // $ANTLR start "range"
    // org\\antlr\\v4\\parse\\g:974:1: range : ^( RANGE STRING_LITERAL STRING_LITERAL ) ;
    public range_return range()
    {
        range_return retval = new range_return();
        retval.start = input.LT(1);


        enterRange(((GrammarAST)retval.start));

        try
        {
            // org\\antlr\\v4\\parse\\g:981:5: ( ^( RANGE STRING_LITERAL STRING_LITERAL ) )
            // org\\antlr\\v4\\parse\\g:981:7: ^( RANGE STRING_LITERAL STRING_LITERAL )
            {
                Match(input, RANGE, FOLLOW_RANGE_in_range2507);
                Match(input, Token.DOWN, null);
                Match(input, STRING_LITERAL, FOLLOW_STRING_LITERAL_in_range2509);
                Match(input, STRING_LITERAL, FOLLOW_STRING_LITERAL_in_range2511);
                Match(input, Token.UP, null);

            }


            exitRange(((GrammarAST)retval.start));

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
    // $ANTLR end "range"


    public class terminal_return : TreeRuleReturnScope
    {
    };


    // $ANTLR start "terminal"
    // org\\antlr\\v4\\parse\\g:984:1: terminal : ( ^( STRING_LITERAL elementOptions ) | STRING_LITERAL | ^( TOKEN_REF elementOptions ) | TOKEN_REF );
    public terminal_return terminal()
    {
        terminal_return retval = new terminal_return();
        retval.start = input.LT(1);

        GrammarAST STRING_LITERAL41 = null;
        GrammarAST STRING_LITERAL42 = null;
        GrammarAST TOKEN_REF43 = null;
        GrammarAST TOKEN_REF44 = null;


        enterTerminal(((GrammarAST)retval.start));

        try
        {
            // org\\antlr\\v4\\parse\\g:991:5: ( ^( STRING_LITERAL elementOptions ) | STRING_LITERAL | ^( TOKEN_REF elementOptions ) | TOKEN_REF )
            int alt53 = 4;
            int LA53_0 = input.LA(1);
            if ((LA53_0 == STRING_LITERAL))
            {
                int LA53_1 = input.LA(2);
                if ((LA53_1 == DOWN))
                {
                    alt53 = 1;
                }
                else if ((LA53_1 == EOF || (LA53_1 >= UP && LA53_1 <= ACTION) || LA53_1 == ASSIGN || LA53_1 == DOT || LA53_1 == LEXER_CHAR_SET || LA53_1 == NOT || LA53_1 == PLUS_ASSIGN || LA53_1 == RANGE || LA53_1 == RULE_REF || LA53_1 == SEMPRED || LA53_1 == STRING_LITERAL || LA53_1 == TOKEN_REF || (LA53_1 >= BLOCK && LA53_1 <= CLOSURE) || LA53_1 == EPSILON || (LA53_1 >= OPTIONAL && LA53_1 <= POSITIVE_CLOSURE) || (LA53_1 >= SET && LA53_1 <= WILDCARD)))
                {
                    alt53 = 2;
                }

                else
                {
                    int nvaeMark = input.Mark();
                    try
                    {
                        input.Consume();
                        NoViableAltException nvae =
                            new NoViableAltException("", 53, 1, input);
                        throw nvae;
                    }
                    finally
                    {
                        input.Rewind(nvaeMark);
                    }
                }

            }
            else if ((LA53_0 == TOKEN_REF))
            {
                int LA53_2 = input.LA(2);
                if ((LA53_2 == DOWN))
                {
                    alt53 = 3;
                }
                else if ((LA53_2 == EOF || (LA53_2 >= UP && LA53_2 <= ACTION) || LA53_2 == ASSIGN || LA53_2 == DOT || LA53_2 == LEXER_CHAR_SET || LA53_2 == NOT || LA53_2 == PLUS_ASSIGN || LA53_2 == RANGE || LA53_2 == RULE_REF || LA53_2 == SEMPRED || LA53_2 == STRING_LITERAL || LA53_2 == TOKEN_REF || (LA53_2 >= BLOCK && LA53_2 <= CLOSURE) || LA53_2 == EPSILON || (LA53_2 >= OPTIONAL && LA53_2 <= POSITIVE_CLOSURE) || (LA53_2 >= SET && LA53_2 <= WILDCARD)))
                {
                    alt53 = 4;
                }

                else
                {
                    int nvaeMark = input.Mark();
                    try
                    {
                        input.Consume();
                        NoViableAltException nvae =
                            new NoViableAltException("", 53, 2, input);
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
                    new NoViableAltException("", 53, 0, input);
                throw nvae;
            }

            switch (alt53)
            {
                case 1:
                    // org\\antlr\\v4\\parse\\g:991:8: ^( STRING_LITERAL elementOptions )
                    {
                        STRING_LITERAL41 = (GrammarAST)Match(input, STRING_LITERAL, FOLLOW_STRING_LITERAL_in_terminal2541);
                        Match(input, Token.DOWN, null);
                        PushFollow(FOLLOW_elementOptions_in_terminal2543);
                        elementOptions();
                        state._fsp--;

                        Match(input, Token.UP, null);

                        stringRef((TerminalAST)STRING_LITERAL41);
                    }
                    break;
                case 2:
                    // org\\antlr\\v4\\parse\\g:993:7: STRING_LITERAL
                    {
                        STRING_LITERAL42 = (GrammarAST)Match(input, STRING_LITERAL, FOLLOW_STRING_LITERAL_in_terminal2566);
                        stringRef((TerminalAST)STRING_LITERAL42);
                    }
                    break;
                case 3:
                    // org\\antlr\\v4\\parse\\g:994:7: ^( TOKEN_REF elementOptions )
                    {
                        TOKEN_REF43 = (GrammarAST)Match(input, TOKEN_REF, FOLLOW_TOKEN_REF_in_terminal2580);
                        Match(input, Token.DOWN, null);
                        PushFollow(FOLLOW_elementOptions_in_terminal2582);
                        elementOptions();
                        state._fsp--;

                        Match(input, Token.UP, null);

                        tokenRef((TerminalAST)TOKEN_REF43);
                    }
                    break;
                case 4:
                    // org\\antlr\\v4\\parse\\g:995:7: TOKEN_REF
                    {
                        TOKEN_REF44 = (GrammarAST)Match(input, TOKEN_REF, FOLLOW_TOKEN_REF_in_terminal2593);
                        tokenRef((TerminalAST)TOKEN_REF44);
                    }
                    break;

            }

            exitTerminal(((GrammarAST)retval.start));

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


    public class elementOptions_return : TreeRuleReturnScope
    {
    };


    // $ANTLR start "elementOptions"
    // org\\antlr\\v4\\parse\\g:998:1: elementOptions : ^( ELEMENT_OPTIONS ( elementOption[(GrammarASTWithOptions)$start.getParent()] )* ) ;
    public elementOptions_return elementOptions()
    {
        elementOptions_return retval = new elementOptions_return();
        retval.start = input.LT(1);


        enterElementOptions(((GrammarAST)retval.start));

        try
        {
            // org\\antlr\\v4\\parse\\g:1005:5: ( ^( ELEMENT_OPTIONS ( elementOption[(GrammarASTWithOptions)$start.getParent()] )* ) )
            // org\\antlr\\v4\\parse\\g:1005:7: ^( ELEMENT_OPTIONS ( elementOption[(GrammarASTWithOptions)$start.getParent()] )* )
            {
                Match(input, ELEMENT_OPTIONS, FOLLOW_ELEMENT_OPTIONS_in_elementOptions2630);
                if (input.LA(1) == Token.DOWN)
                {
                    Match(input, Token.DOWN, null);
                // org\\antlr\\v4\\parse\\g:1005:25: ( elementOption[(GrammarASTWithOptions)$start.getParent()] )*
                loop54:
                    while (true)
                    {
                        int alt54 = 2;
                        int LA54_0 = input.LA(1);
                        if ((LA54_0 == ASSIGN || LA54_0 == ID))
                        {
                            alt54 = 1;
                        }

                        switch (alt54)
                        {
                            case 1:
                                // org\\antlr\\v4\\parse\\g:1005:25: elementOption[(GrammarASTWithOptions)$start.getParent()]
                                {
                                    PushFollow(FOLLOW_elementOption_in_elementOptions2632);
                                    elementOption((GrammarASTWithOptions)((GrammarAST)retval.start).getParent());
                                    state._fsp--;

                                }
                                break;

                            default:
                                goto exit54;
                                //break loop54;
                        }
                    }
                exit54:
                    Match(input, Token.UP, null);
                }

            }


            exitElementOptions(((GrammarAST)retval.start));

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
    };


    // $ANTLR start "elementOption"
    // org\\antlr\\v4\\parse\\g:1008:1: elementOption[GrammarASTWithOptions t] : ( ID | ^( ASSIGN id= ID v= ID ) | ^( ASSIGN ID v= STRING_LITERAL ) | ^( ASSIGN ID v= ACTION ) | ^( ASSIGN ID v= INT ) );
    public elementOption_return elementOption(GrammarASTWithOptions t)
    {
        elementOption_return retval = new elementOption_return();
        retval.start = input.LT(1);

        GrammarAST id = null;
        GrammarAST v = null;
        GrammarAST ID45 = null;
        GrammarAST ID46 = null;
        GrammarAST ID47 = null;
        GrammarAST ID48 = null;


        enterElementOption(((GrammarAST)retval.start));

        try
        {
            // org\\antlr\\v4\\parse\\g:1015:5: ( ID | ^( ASSIGN id= ID v= ID ) | ^( ASSIGN ID v= STRING_LITERAL ) | ^( ASSIGN ID v= ACTION ) | ^( ASSIGN ID v= INT ) )
            int alt55 = 5;
            int LA55_0 = input.LA(1);
            if ((LA55_0 == ID))
            {
                alt55 = 1;
            }
            else if ((LA55_0 == ASSIGN))
            {
                int LA55_2 = input.LA(2);
                if ((LA55_2 == DOWN))
                {
                    int LA55_3 = input.LA(3);
                    if ((LA55_3 == ID))
                    {
                        switch (input.LA(4))
                        {
                            case ID:
                                {
                                    alt55 = 2;
                                }
                                break;
                            case STRING_LITERAL:
                                {
                                    alt55 = 3;
                                }
                                break;
                            case ACTION:
                                {
                                    alt55 = 4;
                                }
                                break;
                            case INT:
                                {
                                    alt55 = 5;
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
                                        new NoViableAltException("", 55, 4, input);
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
                                new NoViableAltException("", 55, 3, input);
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
                            new NoViableAltException("", 55, 2, input);
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
                    new NoViableAltException("", 55, 0, input);
                throw nvae;
            }

            switch (alt55)
            {
                case 1:
                    // org\\antlr\\v4\\parse\\g:1015:7: ID
                    {
                        ID45 = (GrammarAST)Match(input, ID, FOLLOW_ID_in_elementOption2663);
                        elementOption(t, ID45, null);
                    }
                    break;
                case 2:
                    // org\\antlr\\v4\\parse\\g:1016:9: ^( ASSIGN id= ID v= ID )
                    {
                        Match(input, ASSIGN, FOLLOW_ASSIGN_in_elementOption2683);
                        Match(input, Token.DOWN, null);
                        id = (GrammarAST)Match(input, ID, FOLLOW_ID_in_elementOption2687);
                        v = (GrammarAST)Match(input, ID, FOLLOW_ID_in_elementOption2691);
                        Match(input, Token.UP, null);

                        elementOption(t, id, v);
                    }
                    break;
                case 3:
                    // org\\antlr\\v4\\parse\\g:1017:9: ^( ASSIGN ID v= STRING_LITERAL )
                    {
                        Match(input, ASSIGN, FOLLOW_ASSIGN_in_elementOption2707);
                        Match(input, Token.DOWN, null);
                        ID46 = (GrammarAST)Match(input, ID, FOLLOW_ID_in_elementOption2709);
                        v = (GrammarAST)Match(input, STRING_LITERAL, FOLLOW_STRING_LITERAL_in_elementOption2713);
                        Match(input, Token.UP, null);

                        elementOption(t, ID46, v);
                    }
                    break;
                case 4:
                    // org\\antlr\\v4\\parse\\g:1018:9: ^( ASSIGN ID v= ACTION )
                    {
                        Match(input, ASSIGN, FOLLOW_ASSIGN_in_elementOption2727);
                        Match(input, Token.DOWN, null);
                        ID47 = (GrammarAST)Match(input, ID, FOLLOW_ID_in_elementOption2729);
                        v = (GrammarAST)Match(input, ACTION, FOLLOW_ACTION_in_elementOption2733);
                        Match(input, Token.UP, null);

                        elementOption(t, ID47, v);
                    }
                    break;
                case 5:
                    // org\\antlr\\v4\\parse\\g:1019:9: ^( ASSIGN ID v= INT )
                    {
                        Match(input, ASSIGN, FOLLOW_ASSIGN_in_elementOption2749);
                        Match(input, Token.DOWN, null);
                        ID48 = (GrammarAST)Match(input, ID, FOLLOW_ID_in_elementOption2751);
                        v = (GrammarAST)Match(input, INT, FOLLOW_INT_in_elementOption2755);
                        Match(input, Token.UP, null);

                        elementOption(t, ID48, v);
                    }
                    break;

            }

            exitElementOption(((GrammarAST)retval.start));

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



    //static final short[] DFA38_eot = DFA.unpackEncodedString(DFA38_eotS);
    //static final short[] DFA38_eof = DFA.unpackEncodedString(DFA38_eofS);
    //static final char[] DFA38_min = DFA.unpackEncodedStringToUnsignedChars(DFA38_minS);
    //static final char[] DFA38_max = DFA.unpackEncodedStringToUnsignedChars(DFA38_maxS);
    //static final short[] DFA38_accept = DFA.unpackEncodedString(DFA38_acceptS);
    //static final short[] DFA38_special = DFA.unpackEncodedString(DFA38_specialS);
    //static final short[][] DFA38_transition;
    static readonly short[] DFA38_eot = RuntimeUtils.Convert(new char[] { '\u0014', '\uffff' });
    static readonly short[] DFA38_eof = RuntimeUtils.Convert(new char[] { '\u0014', '\uffff' });
    static readonly char[] DFA38_min = new char[] { '\u0001', '\u0045', '\u0001', '\u0002', '\u0001', '\u0004', '\u0001', '\u0002', '\u0002', '\uffff', '\u0002', '\u0003', '\u0001', '\u0002', '\u0001', '\u0004', '\u0001', '\u001c', '\u0001', '\u0004', '\u0008', '\u0003' };
    static readonly char[] DFA38_max = new char[] { '\u0001', '\u0045', '\u0001', '\u0002', '\u0001', '\u0053', '\u0001', '\u0002', '\u0002', '\uffff', '\u0002', '\u001c', '\u0001', '\u0002', '\u0001', '\u0053', '\u0001', '\u001c', '\u0001', '\u003b', '\u0004', '\u0003', '\u0004', '\u001c' };
    static readonly short[] DFA38_accept = RuntimeUtils.Convert(new char[] { '\u0004', '\uffff', '\u0001', '\u0001', '\u0001', '\u0002', '\u000e', '\uffff' });
    static readonly short[] DFA38_special = RuntimeUtils.Convert(new char[] { '\u0014', '\uffff', '\u007d', '\u003e' });
    static readonly short[][] DFA38_transition = new short[][]{
    RuntimeUtils.Convert(new char[] {'\u0001','\u0001'}),
    RuntimeUtils.Convert(new char[] {'\u0001','\u0002'}),
    RuntimeUtils.Convert(new char[] {'\u0001','\u0004','\u0005','\uffff','\u0001','\u0004','\u0009','\uffff','\u0001','\u0004','\u0012','\uffff','\u0001','\u0004','\u0006','\uffff','\u0001','\u0004','\u0002','\uffff','\u0001','\u0004','\u0004','\uffff','\u0001','\u0004','\u0001','\uffff','\u0001','\u0004','\u0002','\uffff','\u0001','\u0004','\u0002','\uffff','\u0001','\u0004','\u0007','\uffff','\u0002','\u0004','\u0001','\uffff','\u0001','\u0003','\u0001','\u0005','\u0002','\uffff','\u0002','\u0004','\u0003','\uffff','\u0002','\u0004'}),
    RuntimeUtils.Convert(new char[] {'\u0001','\u0006'}),
    RuntimeUtils.Convert(new char[0]),
    RuntimeUtils.Convert(new char[0]),
    RuntimeUtils.Convert(new char[] {'\u0001','\u0009','\u0006','\uffff','\u0001','\u0008','\u0011','\uffff','\u0001','\u0007'}),
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



    protected class DFA38 : antlr.runtime.DFA
    {


        public DFA38(BaseRecognizer recognizer)
        {
            this.recognizer = recognizer;
            this.decisionNumber = 38;
            this.eot = DFA38_eot;
            this.eof = DFA38_eof;
            this.min = DFA38_min;
            this.max = DFA38_max;
            this.accept = DFA38_accept;
            this.special = DFA38_special;
            this.transition = DFA38_transition;
        }
        //@Override
        public String getDescription()
        {
            return "783:1: alternative : ( ^( ALT ( elementOptions )? ( element )+ ) | ^( ALT ( elementOptions )? EPSILON ) );";
        }
    }

    public static readonly BitSet FOLLOW_GRAMMAR_in_grammarSpec85 = new BitSet(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_ID_in_grammarSpec87 = new BitSet(new long[] { 0x2000040020002800L, 0x0000000000020000L });
    public static readonly BitSet FOLLOW_prequelConstructs_in_grammarSpec106 = new BitSet(new long[] { 0x0000000000000000L, 0x0000000000020000L });
    public static readonly BitSet FOLLOW_rules_in_grammarSpec123 = new BitSet(new long[] { 0x0000001000000008L });
    public static readonly BitSet FOLLOW_mode_in_grammarSpec125 = new BitSet(new long[] { 0x0000001000000008L });
    public static readonly BitSet FOLLOW_prequelConstruct_in_prequelConstructs167 = new BitSet(new long[] { 0x2000040020002802L });
    public static readonly BitSet FOLLOW_optionsSpec_in_prequelConstruct194 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_delegateGrammars_in_prequelConstruct204 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_tokensSpec_in_prequelConstruct214 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_channelsSpec_in_prequelConstruct224 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_action_in_prequelConstruct234 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_OPTIONS_in_optionsSpec259 = new BitSet(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_option_in_optionsSpec261 = new BitSet(new long[] { 0x0000000000000408L });
    public static readonly BitSet FOLLOW_ASSIGN_in_option295 = new BitSet(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_ID_in_option297 = new BitSet(new long[] { 0x0800000050000000L });
    public static readonly BitSet FOLLOW_optionValue_in_option301 = new BitSet(new long[] { 0x0000000000000008L });
    public static readonly BitSet FOLLOW_IMPORT_in_delegateGrammars389 = new BitSet(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_delegateGrammar_in_delegateGrammars391 = new BitSet(new long[] { 0x0000000010000408L });
    public static readonly BitSet FOLLOW_ASSIGN_in_delegateGrammar420 = new BitSet(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_ID_in_delegateGrammar424 = new BitSet(new long[] { 0x0000000010000000L });
    public static readonly BitSet FOLLOW_ID_in_delegateGrammar428 = new BitSet(new long[] { 0x0000000000000008L });
    public static readonly BitSet FOLLOW_ID_in_delegateGrammar443 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_TOKENS_SPEC_in_tokensSpec477 = new BitSet(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_tokenSpec_in_tokensSpec479 = new BitSet(new long[] { 0x0000000010000008L });
    public static readonly BitSet FOLLOW_ID_in_tokenSpec502 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_CHANNELS_in_channelsSpec532 = new BitSet(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_channelSpec_in_channelsSpec534 = new BitSet(new long[] { 0x0000000010000008L });
    public static readonly BitSet FOLLOW_ID_in_channelSpec557 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_AT_in_action585 = new BitSet(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_ID_in_action589 = new BitSet(new long[] { 0x0000000010000000L });
    public static readonly BitSet FOLLOW_ID_in_action594 = new BitSet(new long[] { 0x0000000000000010L });
    public static readonly BitSet FOLLOW_ACTION_in_action596 = new BitSet(new long[] { 0x0000000000000008L });
    public static readonly BitSet FOLLOW_RULES_in_rules624 = new BitSet(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_rule_in_rules629 = new BitSet(new long[] { 0x0000000000000008L, 0x0000000000008000L });
    public static readonly BitSet FOLLOW_lexerRule_in_rules631 = new BitSet(new long[] { 0x0000000000000008L, 0x0000000000008000L });
    public static readonly BitSet FOLLOW_MODE_in_mode662 = new BitSet(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_ID_in_mode664 = new BitSet(new long[] { 0x0000000000000008L, 0x0000000000008000L });
    public static readonly BitSet FOLLOW_lexerRule_in_mode668 = new BitSet(new long[] { 0x0000000000000008L, 0x0000000000008000L });
    public static readonly BitSet FOLLOW_RULE_in_lexerRule694 = new BitSet(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_TOKEN_REF_in_lexerRule696 = new BitSet(new long[] { 0x0000040000000000L, 0x0000000000010040L });
    public static readonly BitSet FOLLOW_RULEMODIFIERS_in_lexerRule708 = new BitSet(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_FRAGMENT_in_lexerRule712 = new BitSet(new long[] { 0x0000000000000008L });
    public static readonly BitSet FOLLOW_optionsSpec_in_lexerRule724 = new BitSet(new long[] { 0x0000040000000000L, 0x0000000000000040L });
    public static readonly BitSet FOLLOW_lexerRuleBlock_in_lexerRule745 = new BitSet(new long[] { 0x0000000000000008L });
    public static readonly BitSet FOLLOW_RULE_in_rule790 = new BitSet(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_RULE_REF_in_rule792 = new BitSet(new long[] { 0x1010040200000900L, 0x0000000000010040L });
    public static readonly BitSet FOLLOW_RULEMODIFIERS_in_rule801 = new BitSet(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_ruleModifier_in_rule806 = new BitSet(new long[] { 0x0000000001000008L, 0x0000000000700000L });
    public static readonly BitSet FOLLOW_ARG_ACTION_in_rule817 = new BitSet(new long[] { 0x1010040200000800L, 0x0000000000000040L });
    public static readonly BitSet FOLLOW_ruleReturns_in_rule830 = new BitSet(new long[] { 0x1000040200000800L, 0x0000000000000040L });
    public static readonly BitSet FOLLOW_throwsSpec_in_rule843 = new BitSet(new long[] { 0x0000040200000800L, 0x0000000000000040L });
    public static readonly BitSet FOLLOW_locals_in_rule856 = new BitSet(new long[] { 0x0000040000000800L, 0x0000000000000040L });
    public static readonly BitSet FOLLOW_optionsSpec_in_rule871 = new BitSet(new long[] { 0x0000040000000800L, 0x0000000000000040L });
    public static readonly BitSet FOLLOW_ruleAction_in_rule885 = new BitSet(new long[] { 0x0000040000000800L, 0x0000000000000040L });
    public static readonly BitSet FOLLOW_ruleBlock_in_rule916 = new BitSet(new long[] { 0x0000000000801008L });
    public static readonly BitSet FOLLOW_exceptionGroup_in_rule918 = new BitSet(new long[] { 0x0000000000000008L });
    public static readonly BitSet FOLLOW_exceptionHandler_in_exceptionGroup965 = new BitSet(new long[] { 0x0000000000801002L });
    public static readonly BitSet FOLLOW_finallyClause_in_exceptionGroup968 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_CATCH_in_exceptionHandler994 = new BitSet(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_ARG_ACTION_in_exceptionHandler996 = new BitSet(new long[] { 0x0000000000000010L });
    public static readonly BitSet FOLLOW_ACTION_in_exceptionHandler998 = new BitSet(new long[] { 0x0000000000000008L });
    public static readonly BitSet FOLLOW_FINALLY_in_finallyClause1023 = new BitSet(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_ACTION_in_finallyClause1025 = new BitSet(new long[] { 0x0000000000000008L });
    public static readonly BitSet FOLLOW_LOCALS_in_locals1053 = new BitSet(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_ARG_ACTION_in_locals1055 = new BitSet(new long[] { 0x0000000000000008L });
    public static readonly BitSet FOLLOW_RETURNS_in_ruleReturns1078 = new BitSet(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_ARG_ACTION_in_ruleReturns1080 = new BitSet(new long[] { 0x0000000000000008L });
    public static readonly BitSet FOLLOW_THROWS_in_throwsSpec1106 = new BitSet(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_ID_in_throwsSpec1108 = new BitSet(new long[] { 0x0000000010000008L });
    public static readonly BitSet FOLLOW_AT_in_ruleAction1135 = new BitSet(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_ID_in_ruleAction1137 = new BitSet(new long[] { 0x0000000000000010L });
    public static readonly BitSet FOLLOW_ACTION_in_ruleAction1139 = new BitSet(new long[] { 0x0000000000000008L });
    public static readonly BitSet FOLLOW_BLOCK_in_lexerRuleBlock1217 = new BitSet(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_lexerOuterAlternative_in_lexerRuleBlock1236 = new BitSet(new long[] { 0x0000000000000008L, 0x0000000000001020L });
    public static readonly BitSet FOLLOW_BLOCK_in_ruleBlock1281 = new BitSet(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_outerAlternative_in_ruleBlock1300 = new BitSet(new long[] { 0x0000000000000008L, 0x0000000000000020L });
    public static readonly BitSet FOLLOW_lexerAlternative_in_lexerOuterAlternative1340 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_alternative_in_outerAlternative1362 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_LEXER_ALT_ACTION_in_lexerAlternative1384 = new BitSet(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_lexerElements_in_lexerAlternative1386 = new BitSet(new long[] { 0x0000000010000000L, 0x0000000000000800L });
    public static readonly BitSet FOLLOW_lexerCommand_in_lexerAlternative1388 = new BitSet(new long[] { 0x0000000010000008L, 0x0000000000000800L });
    public static readonly BitSet FOLLOW_lexerElements_in_lexerAlternative1400 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_ALT_in_lexerElements1428 = new BitSet(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_lexerElement_in_lexerElements1430 = new BitSet(new long[] { 0x4942008100000018L, 0x00000000000C64C0L });
    public static readonly BitSet FOLLOW_lexerAtom_in_lexerElement1456 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_lexerSubrule_in_lexerElement1461 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_ACTION_in_lexerElement1468 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_SEMPRED_in_lexerElement1482 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_ACTION_in_lexerElement1497 = new BitSet(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_elementOptions_in_lexerElement1499 = new BitSet(new long[] { 0x0000000000000008L });
    public static readonly BitSet FOLLOW_SEMPRED_in_lexerElement1510 = new BitSet(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_elementOptions_in_lexerElement1512 = new BitSet(new long[] { 0x0000000000000008L });
    public static readonly BitSet FOLLOW_EPSILON_in_lexerElement1520 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_BLOCK_in_lexerBlock1543 = new BitSet(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_optionsSpec_in_lexerBlock1545 = new BitSet(new long[] { 0x0000000000000000L, 0x0000000000001020L });
    public static readonly BitSet FOLLOW_lexerAlternative_in_lexerBlock1548 = new BitSet(new long[] { 0x0000000000000008L, 0x0000000000001020L });
    public static readonly BitSet FOLLOW_terminal_in_lexerAtom1579 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_NOT_in_lexerAtom1590 = new BitSet(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_blockSet_in_lexerAtom1592 = new BitSet(new long[] { 0x0000000000000008L });
    public static readonly BitSet FOLLOW_blockSet_in_lexerAtom1603 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_WILDCARD_in_lexerAtom1614 = new BitSet(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_elementOptions_in_lexerAtom1616 = new BitSet(new long[] { 0x0000000000000008L });
    public static readonly BitSet FOLLOW_WILDCARD_in_lexerAtom1627 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_LEXER_CHAR_SET_in_lexerAtom1635 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_range_in_lexerAtom1645 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_ruleref_in_lexerAtom1655 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_ACTION_in_actionElement1679 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_ACTION_in_actionElement1687 = new BitSet(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_elementOptions_in_actionElement1689 = new BitSet(new long[] { 0x0000000000000008L });
    public static readonly BitSet FOLLOW_SEMPRED_in_actionElement1697 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_SEMPRED_in_actionElement1705 = new BitSet(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_elementOptions_in_actionElement1707 = new BitSet(new long[] { 0x0000000000000008L });
    public static readonly BitSet FOLLOW_ALT_in_alternative1730 = new BitSet(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_elementOptions_in_alternative1732 = new BitSet(new long[] { 0x4942408000100410L, 0x00000000000C60C0L });
    public static readonly BitSet FOLLOW_element_in_alternative1735 = new BitSet(new long[] { 0x4942408000100418L, 0x00000000000C60C0L });
    public static readonly BitSet FOLLOW_ALT_in_alternative1743 = new BitSet(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_elementOptions_in_alternative1745 = new BitSet(new long[] { 0x0000000000000000L, 0x0000000000000400L });
    public static readonly BitSet FOLLOW_EPSILON_in_alternative1748 = new BitSet(new long[] { 0x0000000000000008L });
    public static readonly BitSet FOLLOW_LEXER_ACTION_CALL_in_lexerCommand1774 = new BitSet(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_ID_in_lexerCommand1776 = new BitSet(new long[] { 0x0000000050000000L });
    public static readonly BitSet FOLLOW_lexerCommandExpr_in_lexerCommand1778 = new BitSet(new long[] { 0x0000000000000008L });
    public static readonly BitSet FOLLOW_ID_in_lexerCommand1794 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_labeledElement_in_element1851 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_atom_in_element1856 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_subrule_in_element1861 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_ACTION_in_element1868 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_SEMPRED_in_element1882 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_ACTION_in_element1897 = new BitSet(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_elementOptions_in_element1899 = new BitSet(new long[] { 0x0000000000000008L });
    public static readonly BitSet FOLLOW_SEMPRED_in_element1910 = new BitSet(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_elementOptions_in_element1912 = new BitSet(new long[] { 0x0000000000000008L });
    public static readonly BitSet FOLLOW_range_in_element1920 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_NOT_in_element1926 = new BitSet(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_blockSet_in_element1928 = new BitSet(new long[] { 0x0000000000000008L });
    public static readonly BitSet FOLLOW_NOT_in_element1935 = new BitSet(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_block_in_element1937 = new BitSet(new long[] { 0x0000000000000008L });
    public static readonly BitSet FOLLOW_atom_in_astOperand1959 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_NOT_in_astOperand1965 = new BitSet(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_blockSet_in_astOperand1967 = new BitSet(new long[] { 0x0000000000000008L });
    public static readonly BitSet FOLLOW_NOT_in_astOperand1974 = new BitSet(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_block_in_astOperand1976 = new BitSet(new long[] { 0x0000000000000008L });
    public static readonly BitSet FOLLOW_set_in_labeledElement1999 = new BitSet(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_ID_in_labeledElement2005 = new BitSet(new long[] { 0x4942408000100410L, 0x00000000000C60C0L });
    public static readonly BitSet FOLLOW_element_in_labeledElement2007 = new BitSet(new long[] { 0x0000000000000008L });
    public static readonly BitSet FOLLOW_blockSuffix_in_subrule2032 = new BitSet(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_block_in_subrule2034 = new BitSet(new long[] { 0x0000000000000008L });
    public static readonly BitSet FOLLOW_block_in_subrule2041 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_blockSuffix_in_lexerSubrule2066 = new BitSet(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_lexerBlock_in_lexerSubrule2068 = new BitSet(new long[] { 0x0000000000000008L });
    public static readonly BitSet FOLLOW_lexerBlock_in_lexerSubrule2075 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_ebnfSuffix_in_blockSuffix2102 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_DOT_in_atom2163 = new BitSet(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_ID_in_atom2165 = new BitSet(new long[] { 0x4800000000000000L });
    public static readonly BitSet FOLLOW_terminal_in_atom2167 = new BitSet(new long[] { 0x0000000000000008L });
    public static readonly BitSet FOLLOW_DOT_in_atom2174 = new BitSet(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_ID_in_atom2176 = new BitSet(new long[] { 0x0040000000000000L });
    public static readonly BitSet FOLLOW_ruleref_in_atom2178 = new BitSet(new long[] { 0x0000000000000008L });
    public static readonly BitSet FOLLOW_WILDCARD_in_atom2188 = new BitSet(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_elementOptions_in_atom2190 = new BitSet(new long[] { 0x0000000000000008L });
    public static readonly BitSet FOLLOW_WILDCARD_in_atom2201 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_terminal_in_atom2217 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_blockSet_in_atom2225 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_ruleref_in_atom2235 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_SET_in_blockSet2260 = new BitSet(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_setElement_in_blockSet2262 = new BitSet(new long[] { 0x4802000100000008L });
    public static readonly BitSet FOLLOW_STRING_LITERAL_in_setElement2286 = new BitSet(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_elementOptions_in_setElement2288 = new BitSet(new long[] { 0x0000000000000008L });
    public static readonly BitSet FOLLOW_TOKEN_REF_in_setElement2300 = new BitSet(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_elementOptions_in_setElement2302 = new BitSet(new long[] { 0x0000000000000008L });
    public static readonly BitSet FOLLOW_STRING_LITERAL_in_setElement2312 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_TOKEN_REF_in_setElement2337 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_RANGE_in_setElement2366 = new BitSet(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_STRING_LITERAL_in_setElement2370 = new BitSet(new long[] { 0x0800000000000000L });
    public static readonly BitSet FOLLOW_STRING_LITERAL_in_setElement2374 = new BitSet(new long[] { 0x0000000000000008L });
    public static readonly BitSet FOLLOW_LEXER_CHAR_SET_in_setElement2397 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_BLOCK_in_block2422 = new BitSet(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_optionsSpec_in_block2424 = new BitSet(new long[] { 0x0000000000000810L, 0x0000000000000020L });
    public static readonly BitSet FOLLOW_ruleAction_in_block2427 = new BitSet(new long[] { 0x0000000000000810L, 0x0000000000000020L });
    public static readonly BitSet FOLLOW_ACTION_in_block2430 = new BitSet(new long[] { 0x0000000000000000L, 0x0000000000000020L });
    public static readonly BitSet FOLLOW_alternative_in_block2433 = new BitSet(new long[] { 0x0000000000000008L, 0x0000000000000020L });
    public static readonly BitSet FOLLOW_RULE_REF_in_ruleref2463 = new BitSet(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_ARG_ACTION_in_ruleref2467 = new BitSet(new long[] { 0x0000000000000008L, 0x0000000000000200L });
    public static readonly BitSet FOLLOW_elementOptions_in_ruleref2470 = new BitSet(new long[] { 0x0000000000000008L });
    public static readonly BitSet FOLLOW_RANGE_in_range2507 = new BitSet(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_STRING_LITERAL_in_range2509 = new BitSet(new long[] { 0x0800000000000000L });
    public static readonly BitSet FOLLOW_STRING_LITERAL_in_range2511 = new BitSet(new long[] { 0x0000000000000008L });
    public static readonly BitSet FOLLOW_STRING_LITERAL_in_terminal2541 = new BitSet(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_elementOptions_in_terminal2543 = new BitSet(new long[] { 0x0000000000000008L });
    public static readonly BitSet FOLLOW_STRING_LITERAL_in_terminal2566 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_TOKEN_REF_in_terminal2580 = new BitSet(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_elementOptions_in_terminal2582 = new BitSet(new long[] { 0x0000000000000008L });
    public static readonly BitSet FOLLOW_TOKEN_REF_in_terminal2593 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_ELEMENT_OPTIONS_in_elementOptions2630 = new BitSet(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_elementOption_in_elementOptions2632 = new BitSet(new long[] { 0x0000000010000408L });
    public static readonly BitSet FOLLOW_ID_in_elementOption2663 = new BitSet(new long[] { 0x0000000000000002L });
    public static readonly BitSet FOLLOW_ASSIGN_in_elementOption2683 = new BitSet(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_ID_in_elementOption2687 = new BitSet(new long[] { 0x0000000010000000L });
    public static readonly BitSet FOLLOW_ID_in_elementOption2691 = new BitSet(new long[] { 0x0000000000000008L });
    public static readonly BitSet FOLLOW_ASSIGN_in_elementOption2707 = new BitSet(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_ID_in_elementOption2709 = new BitSet(new long[] { 0x0800000000000000L });
    public static readonly BitSet FOLLOW_STRING_LITERAL_in_elementOption2713 = new BitSet(new long[] { 0x0000000000000008L });
    public static readonly BitSet FOLLOW_ASSIGN_in_elementOption2727 = new BitSet(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_ID_in_elementOption2729 = new BitSet(new long[] { 0x0000000000000010L });
    public static readonly BitSet FOLLOW_ACTION_in_elementOption2733 = new BitSet(new long[] { 0x0000000000000008L });
    public static readonly BitSet FOLLOW_ASSIGN_in_elementOption2749 = new BitSet(new long[] { 0x0000000000000004L });
    public static readonly BitSet FOLLOW_ID_in_elementOption2751 = new BitSet(new long[] { 0x0000000040000000L });
    public static readonly BitSet FOLLOW_INT_in_elementOption2755 = new BitSet(new long[] { 0x0000000000000008L });
}
