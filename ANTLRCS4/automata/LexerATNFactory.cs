/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using Antlr4.StringTemplate;
using ANTLRCS4.Runtime;
using org.antlr.runtime.tree;
using org.antlr.v4.automata;
using org.antlr.v4.codegen;
using org.antlr.v4.misc;
using org.antlr.v4.parse;
using org.antlr.v4.runtime;
using org.antlr.v4.runtime.atn;
using org.antlr.v4.runtime.misc;
using org.antlr.v4.tool;
using org.antlr.v4.tool.ast;
using System.Text;
using static org.antlr.v4.automata.ATNFactory;

namespace org.antlr.v4.automata;


public class LexerATNFactory : ParserATNFactory
{
    public TemplateGroup codegenTemplates;

    /**
	 * Provides a map of names of predefined constants which are likely to
	 * appear as the argument for lexer commands. These names would be resolved
	 * by the Java compiler for lexer commands that are translated to embedded
	 * actions, but are required during code generation for creating
	 * {@link LexerAction} instances that are usable by a lexer interpreter.
	 */
    public static readonly Dictionary<String, int> COMMON_CONSTANTS = new();
    static LexerATNFactory()
    {
        COMMON_CONSTANTS.Add("HIDDEN", Lexer.HIDDEN);
        COMMON_CONSTANTS.Add("DEFAULT_TOKEN_CHANNEL", Lexer.DEFAULT_TOKEN_CHANNEL);
        COMMON_CONSTANTS.Add("DEFAULT_MODE", Lexer.DEFAULT_MODE);
        COMMON_CONSTANTS.Add("SKIP", Lexer.SKIP);
        COMMON_CONSTANTS.Add("MORE", Lexer.MORE);
        COMMON_CONSTANTS.Add("EOF", Lexer.EOF);
        COMMON_CONSTANTS.Add("MAX_CHAR_VALUE", Lexer.MAX_CHAR_VALUE);
        COMMON_CONSTANTS.Add("MIN_CHAR_VALUE", Lexer.MIN_CHAR_VALUE);
    }

    private readonly List<string> ruleCommands = new();

    /**
	 * Maps from an action index to a {@link LexerAction} object.
	 */
    protected Dictionary<int, LexerAction> indexToActionMap = new ();
    /**
	 * Maps from a {@link LexerAction} object to the action index.
	 */
    protected Dictionary<LexerAction, int> actionToIndexMap = new ();

    public LexerATNFactory(LexerGrammar g, CodeGenerator codeGenerator = null) : base(g)
    {

        // use codegen to get correct language templates for lexer commands
        codegenTemplates = (codeGenerator == null ? CodeGenerator.Create(g) : codeGenerator).Templates;
    }

    public static HashSet<String> GetCommonConstants() => COMMON_CONSTANTS.Keys.ToHashSet();

    ////@Override
    public ATN CreateATN()
    {
        // BUILD ALL START STATES (ONE PER MODE)
        var modes = ((LexerGrammar)g).modes.Keys.ToHashSet();
        foreach (var modeName in modes)
        {
            // create s0, start state; implied Tokens rule node
            var startState =
                newState<TokensStartState>(typeof(TokensStartState), null);
            atn.modeNameToStartState[modeName] = startState;
            atn.modeToStartState.Add(startState);
            atn.defineDecisionState(startState);
        }

        // INIT ACTION, RULE->TOKEN_TYPE MAP
        atn.ruleToTokenType = new int[g.rules.Count];
        foreach (var r in g.rules.Values)
        {
            atn.ruleToTokenType[r.index] = g.getTokenType(r.name);
        }

        // CREATE ATN FOR EACH RULE
        CreateATN(g.rules.Values);

        atn.lexerActions = new LexerAction[indexToActionMap.Count];
        foreach (var entry in indexToActionMap)
        {
            atn.lexerActions[entry.Key] = entry.Value;
        }

        // LINK MODE START STATE TO EACH TOKEN RULE
        foreach (var modeName in modes)
        {
            var rules = ((LexerGrammar)g).modes[(modeName)];
            var startState = atn.modeNameToStartState[(modeName)];
            foreach (var r in rules)
            {
                if (!r.isFragment())
                {
                    var s = atn.ruleToStartState[r.index];
                    Epsilon(startState, s);
                }
            }
        }

        ATNOptimizer.Optimize(g, atn);
        CheckEpsilonClosure();
        return atn;
    }

    //@Override
    public Handle Rule(GrammarAST ruleAST, String name, Handle blk)
    {
        ruleCommands.Clear();
        return base.Rule(ruleAST, name, blk);
    }

    //@Override
    public Handle Action(ActionAST _action)
    {
        int ruleIndex = currentRule.index;
        int actionIndex = g.lexerActions[(_action)];
        var lexerAction = new LexerCustomAction(ruleIndex, actionIndex);
        return Action(_action, lexerAction);
    }

    protected int getLexerActionIndex(LexerAction lexerAction)
    {
        if (!actionToIndexMap.TryGetValue(lexerAction,out var lexerActionIndex))
        {
            lexerActionIndex = actionToIndexMap.Count;
            actionToIndexMap[lexerAction] = lexerActionIndex;
            indexToActionMap[lexerActionIndex] = lexerAction;
        }

        return lexerActionIndex;
    }

    ////@Override
    public Handle Action(String _action)
    {
        if (_action.Trim().Length == 0)
        {
            var left = NewState(null);
            var right = NewState(null);
            Epsilon(left, right);
            return new (left, right);
        }

        // define action AST for this rule as if we had found in grammar
        var ast = new ActionAST(new CommonToken(ANTLRParser.ACTION, _action));
        currentRule.defineActionInAlt(currentOuterAlt, ast);
        return Action(ast);
    }

    protected Handle Action(GrammarAST node, LexerAction lexerAction)
    {
        var left = NewState(node);
        var right = NewState(node);
        bool isCtxDependent = false;
        int lexerActionIndex = getLexerActionIndex(lexerAction);
        var a =
            new ActionTransition(right, currentRule.index, lexerActionIndex, isCtxDependent);
        left.AddTransition(a);
        node.atnState = left;
        return new Handle(left, right);
    }

    ////@Override
    public Handle LexerAltCommands(Handle alt, Handle cmds)
    {
        var h = new Handle(alt.left, cmds.right);
        Epsilon(alt.right, cmds.left);
        return h;
    }

    ////@Override
    public Handle LexerCallCommand(GrammarAST ID, GrammarAST arg)
    {
        return LexerCallCommandOrCommand(ID, arg);
    }

    ////@Override
    public Handle LexerCommand(GrammarAST ID)
    {
        return LexerCallCommandOrCommand(ID, null);
    }

    private Handle LexerCallCommandOrCommand(GrammarAST ID, GrammarAST arg)
    {
        var lexerAction = CreateLexerAction(ID, arg);
        if (lexerAction != null)
        {
            return Action(ID, lexerAction);
        }

        // fall back to standard action generation for the command
        var cmdST = codegenTemplates.GetInstanceOf("Lexer" +
                CharSupport.Capitalize(ID.getText()) +
                "Command");
        if (cmdST == null)
        {
            g.Tools.ErrMgr.GrammarError(ErrorType.INVALID_LEXER_COMMAND, g.fileName, ID.token, ID.getText());
            return Epsilon(ID);
        }

        bool callCommand = arg != null;
        bool containsArg = cmdST.impl.FormalArguments != null &&
            cmdST.impl.FormalArguments.Any(f => f.Name == "arg");
        if (callCommand != containsArg)
        {
            var errorType = callCommand ? ErrorType.UNWANTED_LEXER_COMMAND_ARGUMENT : ErrorType.MISSING_LEXER_COMMAND_ARGUMENT;
            g.Tools.ErrMgr.GrammarError(errorType, g.fileName, ID.token, ID.getText());
            return Epsilon(ID);
        }

        if (callCommand)
        {
            cmdST.Add("arg", arg.getText());
            cmdST.Add("grammar", arg.g);
        }

        return Action(cmdST.Render());
    }

    //@Override
    public Handle Range(GrammarAST a, GrammarAST b)
    {
        var left = NewState(a);
        var right = NewState(b);
        int t1 = CharSupport.GetCharValueFromGrammarCharLiteral(a.getText());
        int t2 = CharSupport.GetCharValueFromGrammarCharLiteral(b.getText());
        if (CheckRange(a, b, t1, t2))
        {
            left.AddTransition(CreateTransition(right, t1, t2, a));
        }
        a.atnState = left;
        b.atnState = left;
        return new Handle(left, right);
    }

    //@Override
    public Handle Set(GrammarAST associatedAST, List<GrammarAST> alts, bool invert)
    {
        var left = NewState(associatedAST);
        var right = NewState(associatedAST);
        var set = new IntervalSet();
        foreach (var t in alts)
        {
            if (t.getType() == ANTLRParser.RANGE)
            {
                int a = CharSupport.GetCharValueFromGrammarCharLiteral(t.getChild(0).getText());
                int b = CharSupport.GetCharValueFromGrammarCharLiteral(t.getChild(1).getText());
                if (CheckRange((GrammarAST)t.getChild(0), (GrammarAST)t.getChild(1), a, b))
                {
                    CheckRangeAndAddToSet(associatedAST, t, set, a, b, currentRule.caseInsensitive, null);
                }
            }
            else if (t.getType() == ANTLRParser.LEXER_CHAR_SET)
            {
                set.addAll(GetSetFromCharSetLiteral(t));
            }
            else if (t.getType() == ANTLRParser.STRING_LITERAL)
            {
                int c = CharSupport.GetCharValueFromGrammarCharLiteral(t.getText());
                if (c != -1)
                {
                    CheckCharAndAddToSet(associatedAST, set, c);
                }
                else
                {
                    g.Tools.ErrMgr.GrammarError(ErrorType.INVALID_LITERAL_IN_LEXER_SET,
                                               g.fileName, t.getToken(), t.getText());
                }
            }
            else if (t.getType() == ANTLRParser.TOKEN_REF)
            {
                g.Tools.ErrMgr.GrammarError(ErrorType.UNSUPPORTED_REFERENCE_IN_LEXER_SET,
                                           g.fileName, t.getToken(), t.getText());
            }
        }
        if (invert)
        {
            left.AddTransition(new NotSetTransition(right, set));
        }
        else
        {
            Transition transition;
            if (set.getIntervals().Count == 1)
            {
                var interval = set.getIntervals()[(0)];
                transition = CodePointTransitions.createWithCodePointRange(right, interval.a, interval.b);
            }
            else
            {
                transition = new SetTransition(right, set);
            }

            left.AddTransition(transition);
        }
        associatedAST.atnState = left;
        return new Handle(left, right);
    }

    protected bool CheckRange(GrammarAST leftNode, GrammarAST rightNode, int leftValue, int rightValue)
    {
        bool result = true;
        if (leftValue == -1)
        {
            result = false;
            g.Tools.ErrMgr.GrammarError(ErrorType.INVALID_LITERAL_IN_LEXER_SET,
                    g.fileName, leftNode.getToken(), leftNode.getText());
        }
        if (rightValue == -1)
        {
            result = false;
            g.Tools.ErrMgr.GrammarError(ErrorType.INVALID_LITERAL_IN_LEXER_SET,
                    g.fileName, rightNode.getToken(), rightNode.getText());
        }
        if (!result) return false;

        if (rightValue < leftValue)
        {
            g.Tools.ErrMgr.GrammarError(ErrorType.EMPTY_STRINGS_AND_SETS_NOT_ALLOWED,
                    g.fileName, leftNode.parent.getToken(), leftNode.getText() + ".." + rightNode.getText());
            return false;
        }
        return true;
    }

    /** For a lexer, a string is a sequence of char to match.  That is,
	 *  "fog" is treated as 'f' 'o' 'g' not as a single transition in
	 *  the DFA.  Machine== o-'f'-&gt;o-'o'-&gt;o-'g'-&gt;o and has n+1 states
	 *  for n characters.
	 *  if "caseInsensitive" option is enabled, "fog" will be treated as
	 *  o-('f'|'F') -> o-('o'|'O') -> o-('g'|'G')
	 */
    //@Override
    public Handle StringLiteral(TerminalAST stringLiteralAST)
    {
        var chars = stringLiteralAST.getText();
        var left = NewState(stringLiteralAST);
        ATNState right;
        var s = CharSupport.GetStringFromGrammarStringLiteral(chars);
        if (s == null)
        {
            // the lexer will already have given an error
            return new Handle(left, left);
        }

        int n = s.Length;
        ATNState prev = left;
        right = null;
        for (int i = 0; i < n;)
        {
            right = NewState(stringLiteralAST);
            int codePoint = char.ConvertToUtf32(s, i);// s.codePointAt(i);
            prev.AddTransition(CreateTransition(right, codePoint, codePoint, stringLiteralAST));
            prev = right;
            i += new Rune(codePoint).Utf16SequenceLength;// char.charCount(codePoint);
        }
        stringLiteralAST.atnState = left;
        return new Handle(left, right);
    }

    /** [Aa\t \u1234a-z\]\p{Letter}\-] char sets */
    //@Override
    public Handle CharSetLiteral(GrammarAST charSetAST)
    {
        var left = NewState(charSetAST);
        var right = NewState(charSetAST);
        var set = GetSetFromCharSetLiteral(charSetAST);

        left.AddTransition(new SetTransition(right, set));
        charSetAST.atnState = left;
        return new Handle(left, right);
    }

    public class CharSetParseState
    {
        public enum Mode :int
        {
            NONE,
            ERROR,
            PREV_CODE_POINT,
            PREV_PROPERTY
        }

        public static readonly CharSetParseState NONE = new (Mode.NONE, false, -1, IntervalSet.EMPTY_SET);
        public static readonly CharSetParseState ERROR = new (Mode.ERROR, false, -1, IntervalSet.EMPTY_SET);

        public readonly Mode mode;
        public readonly bool inRange;
        public readonly int prevCodePoint;
        public readonly IntervalSet prevProperty;

        public CharSetParseState(
                Mode mode,
                bool inRange,
                int prevCodePoint,
                IntervalSet prevProperty)
        {
            this.mode = mode;
            this.inRange = inRange;
            this.prevCodePoint = prevCodePoint;
            this.prevProperty = prevProperty;
        }

        //@Override
        public override String ToString() => $"{base.ToString()} mode={mode} inRange={inRange} prevCodePoint={prevCodePoint} prevProperty={prevProperty}";

        //@Override
        public override bool Equals(Object other)
        {
            if (other is not CharSetParseState that)
            {
                return false;
            }
            if (this == that)
            {
                return true;
            }
            return Objects.Equals(this.mode, that.mode) &&
                Objects.Equals(this.inRange, that.inRange) &&
                Objects.Equals(this.prevCodePoint, that.prevCodePoint) &&
                Objects.Equals(this.prevProperty, that.prevProperty);
        }

        //@Override
        public override int GetHashCode()
            => RuntimeUtils.ObjectsHash(mode, inRange, prevCodePoint, prevProperty);
    }

    public IntervalSet GetSetFromCharSetLiteral(GrammarAST charSetAST)
    {
        var chars = charSetAST.getText()[1..^1];
        var set = new IntervalSet();
        var state = CharSetParseState.NONE;

        int n = chars.Length;
        for (int i = 0; i < n;)
        {
            if (state.mode == CharSetParseState.Mode.ERROR)
            {
                return new ();
            }
            int c = char.ConvertToUtf32(chars, i);
            int offset = new Rune(c).Utf16SequenceLength;
            if (c == '\\')
            {
                var escapeParseResult =
                    EscapeSequenceParsing.ParseEscape(chars, i);
                switch (escapeParseResult.type)
                {
                    case EscapeSequenceParsing.Result.Type.INVALID:
                        String invalid = chars.Substring(escapeParseResult.startOffset,
                                                         escapeParseResult.parseLength);
                        g.Tools.ErrMgr.GrammarError(ErrorType.INVALID_ESCAPE_SEQUENCE,
                                                   g.fileName, charSetAST.getToken(), invalid);
                        state = CharSetParseState.ERROR;
                        break;
                    case EscapeSequenceParsing.Result.Type.CODE_POINT:
                        state = ApplyPrevStateAndMoveToCodePoint(charSetAST, set, state, escapeParseResult.codePoint);
                        break;
                    case EscapeSequenceParsing.Result.Type.PROPERTY:
                        state = ApplyPrevStateAndMoveToProperty(charSetAST, set, state, escapeParseResult.propertyIntervalSet);
                        break;
                }
                offset = escapeParseResult.parseLength;
            }
            else if (c == '-' && !state.inRange && i != 0 && i != n - 1 && state.mode != CharSetParseState.Mode.NONE)
            {
                if (state.mode == CharSetParseState.Mode.PREV_PROPERTY)
                {
                    g.Tools.ErrMgr.GrammarError(ErrorType.UNICODE_PROPERTY_NOT_ALLOWED_IN_RANGE,
                            g.fileName, charSetAST.getToken(), charSetAST.getText());
                    state = CharSetParseState.ERROR;
                }
                else
                {
                    state = new CharSetParseState(state.mode, true, state.prevCodePoint, state.prevProperty);
                }
            }
            else
            {
                state = ApplyPrevStateAndMoveToCodePoint(charSetAST, set, state, c);
            }
            i += offset;
        }
        if (state.mode == CharSetParseState.Mode.ERROR)
        {
            return new IntervalSet();
        }
        // Whether or not we were in a range, we'll add the last code point found to the set.
        ApplyPrevState(charSetAST, set, state);

        if (set.isNil())
        {
            g.Tools.ErrMgr.GrammarError(ErrorType.EMPTY_STRINGS_AND_SETS_NOT_ALLOWED, g.fileName, charSetAST.getToken(), "[]");
        }

        return set;
    }

    private CharSetParseState ApplyPrevStateAndMoveToCodePoint(
            GrammarAST charSetAST,
            IntervalSet set,
            CharSetParseState state,
            int codePoint)
    {
        if (state.inRange)
        {
            if (state.prevCodePoint > codePoint)
            {
                g.Tools.ErrMgr.GrammarError(
                        ErrorType.EMPTY_STRINGS_AND_SETS_NOT_ALLOWED,
                        g.fileName,
                        charSetAST.getToken(),
                        CharSupport.GetRangeEscapedString(state.prevCodePoint, codePoint));
            }
            CheckRangeAndAddToSet(charSetAST, set, state.prevCodePoint, codePoint);
            state = CharSetParseState.NONE;
        }
        else
        {
            ApplyPrevState(charSetAST, set, state);
            state = new CharSetParseState(
                    CharSetParseState.Mode.PREV_CODE_POINT,
                    false,
                    codePoint,
                    IntervalSet.EMPTY_SET);
        }
        return state;
    }

    private CharSetParseState ApplyPrevStateAndMoveToProperty(
            GrammarAST charSetAST,
            IntervalSet set,
            CharSetParseState state,
            IntervalSet property)
    {
        if (state.inRange)
        {
            g.Tools.ErrMgr.GrammarError(ErrorType.UNICODE_PROPERTY_NOT_ALLOWED_IN_RANGE,
                           g.fileName, charSetAST.getToken(), charSetAST.getText());
            return CharSetParseState.ERROR;
        }
        else
        {
            ApplyPrevState(charSetAST, set, state);
            state = new CharSetParseState(
                    CharSetParseState.Mode.PREV_PROPERTY,
                    false,
                    -1,
                    property);
        }
        return state;
    }

    private void ApplyPrevState(GrammarAST charSetAST, IntervalSet set, CharSetParseState state)
    {
        switch (state.mode)
        {
            case CharSetParseState.Mode.NONE:
            case CharSetParseState.Mode.ERROR:
                break;
            case CharSetParseState.Mode.PREV_CODE_POINT:
                CheckCharAndAddToSet(charSetAST, set, state.prevCodePoint);
                break;
            case CharSetParseState.Mode.PREV_PROPERTY:
                set.addAll(state.prevProperty);
                break;
        }
    }

    private void CheckCharAndAddToSet(GrammarAST ast, IntervalSet set, int c)
    {
        CheckRangeAndAddToSet(ast, ast, set, c, c, currentRule.caseInsensitive, null);
    }

    private void CheckRangeAndAddToSet(GrammarAST mainAst, IntervalSet set, int a, int b)
    {
        CheckRangeAndAddToSet(mainAst, mainAst, set, a, b, currentRule.caseInsensitive, null);
    }

    private CharactersDataCheckStatus CheckRangeAndAddToSet(GrammarAST rootAst, GrammarAST ast, IntervalSet set, int a, int b, bool caseInsensitive, CharactersDataCheckStatus previousStatus)
    {
        CharactersDataCheckStatus status;
        var charactersData = RangeBorderCharactersData.GetAndCheckCharactersData(a, b, g, ast,
                previousStatus == null || !previousStatus.notImpliedCharacters);
        if (caseInsensitive)
        {
            status = new CharactersDataCheckStatus(false, charactersData.mixOfLowerAndUpperCharCase);
            if (charactersData.IsSingleRange())
            {
                status = CheckRangeAndAddToSet(rootAst, ast, set, a, b, false, status);
            }
            else
            {
                status = CheckRangeAndAddToSet(rootAst, ast, set, charactersData.lowerFrom, charactersData.lowerTo, false, status);
                // Don't report similar warning twice
                status = CheckRangeAndAddToSet(rootAst, ast, set, charactersData.upperFrom, charactersData.upperTo, false, status);
            }
        }
        else
        {
            bool charactersCollision = previousStatus != null && previousStatus.collision;
            if (!charactersCollision)
            {
                for (int i = a; i <= b; i++)
                {
                    if (set.contains(i))
                    {
                        string setText;
                        if (rootAst.getChildren() == null)
                        {
                            setText = rootAst.getText();
                        }
                        else
                        {
                            var sb = new StringBuilder();
                            foreach (Object child in rootAst.getChildren())
                            {
                                if (child is RangeAST AST)
                                {
                                    sb.Append(AST.getChild(0).getText());
                                    sb.Append("..");
                                    sb.Append(AST.getChild(1).getText());
                                }
                                else
                                {
                                    sb.Append((child as GrammarAST).getText());
                                }
                                sb.Append(" | ");
                            }
                            sb.Length -= 3;
                            //sb.replace(sb.Length - 3, sb.Length, "");
                            setText = sb.ToString();
                        }
                        var charsString = a == b ? ((char)a).ToString() : (char)a + "-" + (char)b;
                        g.Tools.ErrMgr.GrammarError(ErrorType.CHARACTERS_COLLISION_IN_SET, g.fileName, ast.getToken(),
                                charsString, setText);
                        charactersCollision = true;
                        break;
                    }
                }
            }
            status = new CharactersDataCheckStatus(charactersCollision, charactersData.mixOfLowerAndUpperCharCase);
            set.Add(a, b);
        }
        return status;
    }

    private Transition CreateTransition(ATNState target, int from, int to, CommonTree tree)
    {
        var charactersData = RangeBorderCharactersData.GetAndCheckCharactersData(from, to, g, tree, true);
        if (currentRule.caseInsensitive)
        {
            if (charactersData.IsSingleRange())
            {
                return CodePointTransitions.createWithCodePointRange(target, from, to);
            }
            else
            {
                var intervalSet = new IntervalSet();
                intervalSet.Add(charactersData.lowerFrom, charactersData.lowerTo);
                intervalSet.Add(charactersData.upperFrom, charactersData.upperTo);
                return new SetTransition(target, intervalSet);
            }
        }
        else
        {
            return CodePointTransitions.createWithCodePointRange(target, from, to);
        }
    }

    ////@Override
    public Handle TokenRef(TerminalAST node)
    {
        // Ref to EOF in lexer yields char transition on -1
        if (node.getText().Equals("EOF"))
        {
            var left = NewState(node);
            var right = NewState(node);
            left.AddTransition(new AtomTransition(right, IntStream.EOF));
            return new Handle(left, right);
        }
        return GetRuleRef(node);
    }

    private LexerAction CreateLexerAction(GrammarAST ID, GrammarAST arg)
    {
        var command = ID.getText();
        CheckCommands(command, ID.getToken());

        if ("skip".Equals(command) && arg == null)
        {
            return LexerSkipAction.INSTANCE;
        }
        else if ("more".Equals(command) && arg == null)
        {
            return LexerMoreAction.INSTANCE;
        }
        else if ("popMode".Equals(command) && arg == null)
        {
            return LexerPopModeAction.INSTANCE;
        }
        else if ("mode".Equals(command) && arg != null)
        {
            var modeName = arg.getText();
            int? mode = GetModeConstantValue(modeName, arg.getToken());
            if (mode == null)
            {
                return null;
            }

            return new LexerModeAction(mode.GetValueOrDefault());
        }
        else if ("pushMode".Equals(command) && arg != null)
        {
            var modeName = arg.getText();
            int? mode = GetModeConstantValue(modeName, arg.getToken());
            if (mode == null)
            {
                return null;
            }

            return new LexerPushModeAction(mode.GetValueOrDefault());
        }
        else if ("type".Equals(command) && arg != null)
        {
            var typeName = arg.getText();
            int? type = GetTokenConstantValue(typeName, arg.getToken());
            if (type == null)
            {
                return null;
            }

            return new LexerTypeAction(type.GetValueOrDefault());
        }
        else if ("channel".Equals(command) && arg != null)
        {
            var channelName = arg.getText();
            int? channel = GetChannelConstantValue(channelName, arg.getToken());
            if (channel == null)
            {
                return null;
            }

            return new LexerChannelAction(channel.GetValueOrDefault());
        }
        else
        {
            return null;
        }
    }

    private void CheckCommands(String command, Token commandToken)
    {
        // Command combinations list: https://github.com/antlr/antlr4/issues/1388#issuecomment-263344701
        if (!command.Equals("pushMode") && !command.Equals("popMode"))
        {
            if (ruleCommands.Contains(command))
            {
                g.Tools.ErrMgr.GrammarError(ErrorType.DUPLICATED_COMMAND, g.fileName, commandToken, command);
            }

            string firstCommand = null;

            if (command.Equals("skip"))
            {
                if (ruleCommands.Contains("more"))
                {
                    firstCommand = "more";
                }
                else if (ruleCommands.Contains("type"))
                {
                    firstCommand = "type";
                }
                else if (ruleCommands.Contains("channel"))
                {
                    firstCommand = "channel";
                }
            }
            else if (command.Equals("more"))
            {
                if (ruleCommands.Contains("skip"))
                {
                    firstCommand = "skip";
                }
                else if (ruleCommands.Contains("type"))
                {
                    firstCommand = "type";
                }
                else if (ruleCommands.Contains("channel"))
                {
                    firstCommand = "channel";
                }
            }
            else if (command.Equals("type") || command.Equals("channel"))
            {
                if (ruleCommands.Contains("more"))
                {
                    firstCommand = "more";
                }
                else if (ruleCommands.Contains("skip"))
                {
                    firstCommand = "skip";
                }
            }

            if (firstCommand != null)
            {
                g.Tools.ErrMgr.GrammarError(ErrorType.INCOMPATIBLE_COMMANDS, g.fileName, commandToken, firstCommand, command);
            }
        }

        ruleCommands.Add(command);
    }

    private int? GetModeConstantValue(string modeName, Token token)
    {
        if (modeName == null)
        {
            return null;
        }

        if (modeName.Equals("DEFAULT_MODE"))
        {
            return Lexer.DEFAULT_MODE;
        }
        if (COMMON_CONSTANTS.ContainsKey(modeName))
        {
            g.Tools.ErrMgr.GrammarError(ErrorType.MODE_CONFLICTS_WITH_COMMON_CONSTANTS, g.fileName, token, token.getText());
            return null;
        }

        List<string> modeNames = new(((LexerGrammar)g).modes.Keys);
        int mode = modeNames.IndexOf(modeName);
        if (mode >= 0)
        {
            return mode;
        }

        if (int.TryParse(modeName, out var r))
        {
            return r;
        }
        else
        {
            g.Tools.ErrMgr.GrammarError(ErrorType.CONSTANT_VALUE_IS_NOT_A_RECOGNIZED_MODE_NAME, g.fileName, token, token.getText());
            return null;
        }
    }
    private int? GetTokenConstantValue(String tokenName, Token token)
    {
        if (tokenName == null)
        {
            return null;
        }

        if (tokenName.Equals("EOF"))
        {
            return Lexer.EOF;
        }
        if (COMMON_CONSTANTS.ContainsKey(tokenName))
        {
            g.Tools.ErrMgr.GrammarError(ErrorType.TOKEN_CONFLICTS_WITH_COMMON_CONSTANTS, g.fileName, token, token.getText());
            return null;
        }

        int tokenType = g.getTokenType(tokenName);
        if (tokenType != org.antlr.v4.runtime.Token.INVALID_TYPE)
        {
            return tokenType;
        }

        if (int.TryParse(tokenName, out var r))
        {
            return r;
        }
        else
        {
            g.Tools.ErrMgr.GrammarError(ErrorType.CONSTANT_VALUE_IS_NOT_A_RECOGNIZED_TOKEN_NAME, g.fileName, token, token.getText());
            return null;
        }
    }

    private int? GetChannelConstantValue(String channelName, Token token)
    {
        if (channelName == null)
        {
            return null;
        }

        if (channelName.Equals("HIDDEN"))
        {
            return Lexer.HIDDEN;
        }
        if (channelName.Equals("DEFAULT_TOKEN_CHANNEL"))
        {
            return Lexer.DEFAULT_TOKEN_CHANNEL;
        }
        if (COMMON_CONSTANTS.ContainsKey(channelName))
        {
            g.Tools.ErrMgr.GrammarError(ErrorType.CHANNEL_CONFLICTS_WITH_COMMON_CONSTANTS, g.fileName, token, token.getText());
            return null;
        }

        int channelValue = g.getChannelValue(channelName);
        if (channelValue >= org.antlr.v4.runtime.Token.MIN_USER_CHANNEL_VALUE)
        {
            return channelValue;
        }

        return int.TryParse(channelName, out var r) ? r : null;
        //      try
        //      {
        //} catch (NumberFormatException ex) {
        //	g.tool.errMgr.grammarError(ErrorType.CONSTANT_VALUE_IS_NOT_A_RECOGNIZED_CHANNEL_NAME, g.fileName, token, token.getText());
        //	return null;
        //}
    }
}
