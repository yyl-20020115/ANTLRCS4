/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

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


public class LexerATNFactory : ParserATNFactory {
	public STGroup codegenTemplates;

	/**
	 * Provides a map of names of predefined constants which are likely to
	 * appear as the argument for lexer commands. These names would be resolved
	 * by the Java compiler for lexer commands that are translated to embedded
	 * actions, but are required during code generation for creating
	 * {@link LexerAction} instances that are usable by a lexer interpreter.
	 */
	public static readonly Dictionary<String, int> COMMON_CONSTANTS = new ();
	static LexerATNFactory(){
		COMMON_CONSTANTS.Add("HIDDEN", Lexer.HIDDEN);
		COMMON_CONSTANTS.Add("DEFAULT_TOKEN_CHANNEL", Lexer.DEFAULT_TOKEN_CHANNEL);
		COMMON_CONSTANTS.Add("DEFAULT_MODE", Lexer.DEFAULT_MODE);
		COMMON_CONSTANTS.Add("SKIP", Lexer.SKIP);
		COMMON_CONSTANTS.Add("MORE", Lexer.MORE);
		COMMON_CONSTANTS.Add("EOF", Lexer.EOF);
		COMMON_CONSTANTS.Add("MAX_CHAR_VALUE", Lexer.MAX_CHAR_VALUE);
		COMMON_CONSTANTS.Add("MIN_CHAR_VALUE", Lexer.MIN_CHAR_VALUE);
	}

	private readonly List<String> ruleCommands = new();

	/**
	 * Maps from an action index to a {@link LexerAction} object.
	 */
	protected Dictionary<int, LexerAction> indexToActionMap = new Dictionary<int, LexerAction>();
	/**
	 * Maps from a {@link LexerAction} object to the action index.
	 */
	protected Dictionary<LexerAction, int> actionToIndexMap = new Dictionary<LexerAction, int>();

	public LexerATNFactory(LexerGrammar g, CodeGenerator codeGenerator = null) :base(g){
		
		// use codegen to get correct language templates for lexer commands
		codegenTemplates = (codeGenerator == null ? CodeGenerator.create(g) : codeGenerator).getTemplates();
	}

	public static HashSet<String> getCommonConstants() {
		return COMMON_CONSTANTS.Keys.ToHashSet();
	}

	////@Override
	public ATN createATN() {
		// BUILD ALL START STATES (ONE PER MODE)
		HashSet<String> modes = ((LexerGrammar) g).modes.Keys.ToHashSet();
		foreach (String modeName in modes) {
			// create s0, start state; implied Tokens rule node
			TokensStartState startState =
				newState<TokensStartState>(typeof(TokensStartState), null);
			atn.modeNameToStartState[modeName]= startState;
			atn.modeToStartState.Add(startState);
			atn.defineDecisionState(startState);
		}

		// INIT ACTION, RULE->TOKEN_TYPE MAP
		atn.ruleToTokenType = new int[g.rules.Count];
		foreach (Rule r in g.rules.Values) {
			atn.ruleToTokenType[r.index] = g.getTokenType(r.name);
		}

		// CREATE ATN FOR EACH RULE
		_createATN(g.rules.Values);

		atn.lexerActions = new LexerAction[indexToActionMap.Count];
		foreach (var entry in indexToActionMap) {
			atn.lexerActions[entry.Key] = entry.Value;
		}

		// LINK MODE START STATE TO EACH TOKEN RULE
		foreach (String modeName in modes) {
			List<Rule> rules = ((LexerGrammar)g).modes[(modeName)];
			TokensStartState startState = atn.modeNameToStartState[(modeName)];
            foreach (Rule r in rules) {
				if ( !r.isFragment() ) {
					RuleStartState s = atn.ruleToStartState[r.index];
					epsilon(startState, s);
				}
			}
		}

		ATNOptimizer.optimize(g, atn);
		checkEpsilonClosure();
		return atn;
	}

	//@Override
	public Handle rule(GrammarAST ruleAST, String name, Handle blk) {
		ruleCommands.Clear();
		return base.rule(ruleAST, name, blk);
	}

	//@Override
	public Handle action(ActionAST _action) {
		int ruleIndex = currentRule.index;
		int actionIndex = g.lexerActions[(_action)];
		LexerCustomAction lexerAction = new LexerCustomAction(ruleIndex, actionIndex);
		return action(_action, lexerAction);
	}

	protected int getLexerActionIndex(LexerAction lexerAction) {
		int lexerActionIndex = actionToIndexMap[lexerAction];
		if (lexerActionIndex == null) {
			lexerActionIndex = actionToIndexMap.Count;
			actionToIndexMap[lexerAction] = lexerActionIndex;
			indexToActionMap[lexerActionIndex] = lexerAction;
		}

		return lexerActionIndex;
	}

	////@Override
	public Handle action(String _action) {
		if (_action.Trim().Length == 0) {
			ATNState left = newState(null);
			ATNState right = newState(null);
			epsilon(left, right);
			return new Handle(left, right);
		}

		// define action AST for this rule as if we had found in grammar
        ActionAST ast =	new ActionAST(new CommonToken(ANTLRParser.ACTION, _action));
		currentRule.defineActionInAlt(currentOuterAlt, ast);
		return action(ast);
	}

	protected Handle action(GrammarAST node, LexerAction lexerAction) {
		ATNState left = newState(node);
		ATNState right = newState(node);
		bool isCtxDependent = false;
		int lexerActionIndex = getLexerActionIndex(lexerAction);
		ActionTransition a =
			new ActionTransition(right, currentRule.index, lexerActionIndex, isCtxDependent);
		left.addTransition(a);
		node.atnState = left;
		Handle h = new Handle(left, right);
		return h;
	}

	////@Override
	public Handle lexerAltCommands(Handle alt, Handle cmds) {
		Handle h = new Handle(alt.left, cmds.right);
		epsilon(alt.right, cmds.left);
		return h;
	}

	////@Override
	public Handle lexerCallCommand(GrammarAST ID, GrammarAST arg) {
		return lexerCallCommandOrCommand(ID, arg);
	}

    ////@Override
    public Handle lexerCommand(GrammarAST ID) {
		return lexerCallCommandOrCommand(ID, null);
	}

	private Handle lexerCallCommandOrCommand(GrammarAST ID, GrammarAST arg) {
		LexerAction lexerAction = createLexerAction(ID, arg);
		if (lexerAction != null) {
			return action(ID, lexerAction);
		}

		// fall back to standard action generation for the command
		ST cmdST = codegenTemplates.getInstanceOf("Lexer" +
				CharSupport.capitalize(ID.getText())+
				"Command");
		if (cmdST == null) {
			g.tool.errMgr.grammarError(ErrorType.INVALID_LEXER_COMMAND, g.fileName, ID.token, ID.getText());
			return epsilon(ID);
		}

		bool callCommand = arg != null;
		bool containsArg = cmdST.impl.formalArguments != null && cmdST.impl.formalArguments.containsKey("arg");
		if (callCommand != containsArg) {
			ErrorType errorType = callCommand ? ErrorType.UNWANTED_LEXER_COMMAND_ARGUMENT : ErrorType.MISSING_LEXER_COMMAND_ARGUMENT;
			g.tool.errMgr.grammarError(errorType, g.fileName, ID.token, ID.getText());
			return epsilon(ID);
		}

		if (callCommand) {
			cmdST.add("arg", arg.getText());
			cmdST.add("grammar", arg.g);
		}

		return action(cmdST.render());
	}

	//@Override
	public Handle range(GrammarAST a, GrammarAST b) {
		ATNState left = newState(a);
		ATNState right = newState(b);
		int t1 = CharSupport.getCharValueFromGrammarCharLiteral(a.getText());
		int t2 = CharSupport.getCharValueFromGrammarCharLiteral(b.getText());
		if (checkRange(a, b, t1, t2)) {
			left.addTransition(createTransition(right, t1, t2, a));
		}
		a.atnState = left;
		b.atnState = left;
		return new Handle(left, right);
	}

	//@Override
	public Handle set(GrammarAST associatedAST, List<GrammarAST> alts, bool invert) {
		ATNState left = newState(associatedAST);
		ATNState right = newState(associatedAST);
		IntervalSet set = new IntervalSet();
		foreach (GrammarAST t in alts) {
			if ( t.getType()==ANTLRParser.RANGE ) {
				int a = CharSupport.getCharValueFromGrammarCharLiteral(t.getChild(0).getText());
				int b = CharSupport.getCharValueFromGrammarCharLiteral(t.getChild(1).getText());
				if (checkRange((GrammarAST)t.getChild(0), (GrammarAST)t.getChild(1), a, b)) {
					checkRangeAndAddToSet(associatedAST, t, set, a, b, currentRule.caseInsensitive, null);
				}
			}
			else if ( t.getType()==ANTLRParser.LEXER_CHAR_SET ) {
				set.addAll(getSetFromCharSetLiteral(t));
			}
			else if ( t.getType()==ANTLRParser.STRING_LITERAL ) {
				int c = CharSupport.getCharValueFromGrammarCharLiteral(t.getText());
				if ( c != -1 ) {
					checkCharAndAddToSet(associatedAST, set, c);
				}
				else {
					g.tool.errMgr.grammarError(ErrorType.INVALID_LITERAL_IN_LEXER_SET,
											   g.fileName, t.getToken(), t.getText());
				}
			}
			else if ( t.getType()==ANTLRParser.TOKEN_REF ) {
				g.tool.errMgr.grammarError(ErrorType.UNSUPPORTED_REFERENCE_IN_LEXER_SET,
										   g.fileName, t.getToken(), t.getText());
			}
		}
		if ( invert ) {
			left.addTransition(new NotSetTransition(right, set));
		}
		else {
			Transition transition;
			if (set.getIntervals().Count == 1) {
				Interval interval = set.getIntervals()[(0)];
				transition = CodePointTransitions.createWithCodePointRange(right, interval.a, interval.b);
			}
			else {
				transition = new SetTransition(right, set);
			}

			left.addTransition(transition);
		}
		associatedAST.atnState = left;
		return new Handle(left, right);
	}

	protected bool checkRange(GrammarAST leftNode, GrammarAST rightNode, int leftValue, int rightValue) {
		bool result = true;
		if (leftValue == -1) {
			result = false;
			g.tool.errMgr.grammarError(ErrorType.INVALID_LITERAL_IN_LEXER_SET,
					g.fileName, leftNode.getToken(), leftNode.getText());
		}
		if (rightValue == -1) {
			result = false;
			g.tool.errMgr.grammarError(ErrorType.INVALID_LITERAL_IN_LEXER_SET,
					g.fileName, rightNode.getToken(), rightNode.getText());
		}
		if (!result) return false;

		if (rightValue < leftValue) {
			g.tool.errMgr.grammarError(ErrorType.EMPTY_STRINGS_AND_SETS_NOT_ALLOWED,
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
	public Handle stringLiteral(TerminalAST stringLiteralAST) {
		String chars = stringLiteralAST.getText();
		ATNState left = newState(stringLiteralAST);
		ATNState right;
		String s = CharSupport.getStringFromGrammarStringLiteral(chars);
		if (s == null) {
			// the lexer will already have given an error
			return new Handle(left, left);
		}

		int n = s.Length;
		ATNState prev = left;
		right = null;
		for (int i = 0; i < n; ) {
			right = newState(stringLiteralAST);
			int codePoint = char.ConvertToUtf32(s, i);// s.codePointAt(i);
			prev.addTransition(createTransition(right, codePoint, codePoint, stringLiteralAST));
			prev = right;
			i += new Rune(codePoint).Utf16SequenceLength;// char.charCount(codePoint);
		}
		stringLiteralAST.atnState = left;
		return new Handle(left, right);
	}

	/** [Aa\t \u1234a-z\]\p{Letter}\-] char sets */
	//@Override
	public Handle charSetLiteral(GrammarAST charSetAST) {
		ATNState left = newState(charSetAST);
		ATNState right = newState(charSetAST);
		IntervalSet set = getSetFromCharSetLiteral(charSetAST);

		left.addTransition(new SetTransition(right, set));
		charSetAST.atnState = left;
		return new Handle(left, right);
	}

	public class CharSetParseState {
		public enum Mode {
			NONE,
			ERROR,
			PREV_CODE_POINT,
			PREV_PROPERTY
		}

		public static readonly CharSetParseState NONE = new CharSetParseState(Mode.NONE, false, -1, IntervalSet.EMPTY_SET);
		public static readonly CharSetParseState ERROR = new CharSetParseState(Mode.ERROR, false, -1, IntervalSet.EMPTY_SET);

		public readonly Mode mode;
		public readonly bool inRange;
		public readonly int prevCodePoint;
		public readonly IntervalSet prevProperty;

		public CharSetParseState(
				Mode mode,
				bool inRange,
				int prevCodePoint,
				IntervalSet prevProperty) {
			this.mode = mode;
			this.inRange = inRange;
			this.prevCodePoint = prevCodePoint;
			this.prevProperty = prevProperty;
		}

		//@Override
		public String ToString() {
			return String.format(
					"%s mode=%s inRange=%s prevCodePoint=%d prevProperty=%s",
					base.ToString(),
					mode,
					inRange,
					prevCodePoint,
					prevProperty);
		}

		//@Override
		public bool Equals(Object other) {
			if (!(other is CharSetParseState)) {
				return false;
			}
			CharSetParseState that = (CharSetParseState) other;
			if (this == that) {
				return true;
			}
			return Objects.Equals(this.mode, that.mode) &&
				Objects.Equals(this.inRange, that.inRange) &&
				Objects.Equals(this.prevCodePoint, that.prevCodePoint) &&
				Objects.Equals(this.prevProperty, that.prevProperty);
		}

		//@Override
		public int hashCode() {
			return Objects.GetHashCode(mode, inRange, prevCodePoint, prevProperty);
		}
	}

	public IntervalSet getSetFromCharSetLiteral(GrammarAST charSetAST) {
		String chars = charSetAST.getText();
		chars = chars.Substring(1, chars.Length - 1 - 1);
		IntervalSet set = new IntervalSet();
		CharSetParseState state = CharSetParseState.NONE;

		int n = chars.Length;
		for (int i = 0; i < n; ) {
			if (state.mode == CharSetParseState.Mode.ERROR) {
				return new IntervalSet();
			}
			int c = char.ConvertToUtf32(chars,i);
			int offset = new Rune(c).Utf16SequenceLength;
			if (c == '\\') {
				EscapeSequenceParsing.Result escapeParseResult =
					EscapeSequenceParsing.parseEscape(chars, i);
				switch (escapeParseResult.type) {
					case INVALID:
						String invalid = chars.Substring(escapeParseResult.startOffset,
						                                 escapeParseResult.parseLength);
						g.tool.errMgr.grammarError(ErrorType.INVALID_ESCAPE_SEQUENCE,
						                           g.fileName, charSetAST.getToken(), invalid);
						state = CharSetParseState.ERROR;
						break;
					case CODE_POINT:
						state = applyPrevStateAndMoveToCodePoint(charSetAST, set, state, escapeParseResult.codePoint);
						break;
					case PROPERTY:
						state = applyPrevStateAndMoveToProperty(charSetAST, set, state, escapeParseResult.propertyIntervalSet);
						break;
				}
				offset = escapeParseResult.parseLength;
			}
			else if (c == '-' && !state.inRange && i != 0 && i != n - 1 && state.mode != CharSetParseState.Mode.NONE) {
				if (state.mode == CharSetParseState.Mode.PREV_PROPERTY) {
					g.tool.errMgr.grammarError(ErrorType.UNICODE_PROPERTY_NOT_ALLOWED_IN_RANGE,
							g.fileName, charSetAST.getToken(), charSetAST.getText());
					state = CharSetParseState.ERROR;
				}
				else {
					state = new CharSetParseState(state.mode, true, state.prevCodePoint, state.prevProperty);
				}
			}
			else {
				state = applyPrevStateAndMoveToCodePoint(charSetAST, set, state, c);
			}
			i += offset;
		}
		if (state.mode == CharSetParseState.Mode.ERROR) {
			return new IntervalSet();
		}
		// Whether or not we were in a range, we'll add the last code point found to the set.
		applyPrevState(charSetAST, set, state);

		if (set.isNil()) {
			g.tool.errMgr.grammarError(ErrorType.EMPTY_STRINGS_AND_SETS_NOT_ALLOWED, g.fileName, charSetAST.getToken(), "[]");
		}

		return set;
	}

	private CharSetParseState applyPrevStateAndMoveToCodePoint(
			GrammarAST charSetAST,
			IntervalSet set,
			CharSetParseState state,
			int codePoint) {
		if (state.inRange) {
			if (state.prevCodePoint > codePoint) {
				g.tool.errMgr.grammarError(
						ErrorType.EMPTY_STRINGS_AND_SETS_NOT_ALLOWED,
						g.fileName,
						charSetAST.getToken(),
						CharSupport.getRangeEscapedString(state.prevCodePoint, codePoint));
			}
			checkRangeAndAddToSet(charSetAST, set, state.prevCodePoint, codePoint);
			state = CharSetParseState.NONE;
		}
		else {
			applyPrevState(charSetAST, set, state);
			state = new CharSetParseState(
					CharSetParseState.Mode.PREV_CODE_POINT,
					false,
					codePoint,
					IntervalSet.EMPTY_SET);
		}
		return state;
	}

	private CharSetParseState applyPrevStateAndMoveToProperty(
			GrammarAST charSetAST,
			IntervalSet set,
			CharSetParseState state,
			IntervalSet property) {
		if (state.inRange) {
			g.tool.errMgr.grammarError(ErrorType.UNICODE_PROPERTY_NOT_ALLOWED_IN_RANGE,
						   g.fileName, charSetAST.getToken(), charSetAST.getText());
			return CharSetParseState.ERROR;
		}
		else {
			applyPrevState(charSetAST, set, state);
			state = new CharSetParseState(
					CharSetParseState.Mode.PREV_PROPERTY,
					false,
					-1,
					property);
		}
		return state;
	}

	private void applyPrevState(GrammarAST charSetAST, IntervalSet set, CharSetParseState state) {
		switch (state.mode) {
			case CharSetParseState.Mode.NONE:
			case CharSetParseState.Mode.ERROR:
				break;
			case CharSetParseState.Mode.PREV_CODE_POINT:
				checkCharAndAddToSet(charSetAST, set, state.prevCodePoint);
				break;
			case CharSetParseState.Mode.PREV_PROPERTY:
				set.addAll(state.prevProperty);
				break;
		}
	}

	private void checkCharAndAddToSet(GrammarAST ast, IntervalSet set, int c) {
		checkRangeAndAddToSet(ast, ast, set, c, c, currentRule.caseInsensitive, null);
	}

	private void checkRangeAndAddToSet(GrammarAST mainAst, IntervalSet set, int a, int b) {
		checkRangeAndAddToSet(mainAst, mainAst, set, a, b, currentRule.caseInsensitive, null);
	}

	private CharactersDataCheckStatus checkRangeAndAddToSet(GrammarAST rootAst, GrammarAST ast, IntervalSet set, int a, int b, bool caseInsensitive, CharactersDataCheckStatus previousStatus) {
		CharactersDataCheckStatus status;
		RangeBorderCharactersData charactersData = RangeBorderCharactersData.getAndCheckCharactersData(a, b, g, ast,
				previousStatus == null || !previousStatus.notImpliedCharacters);
		if (caseInsensitive) {
			status = new CharactersDataCheckStatus(false, charactersData.mixOfLowerAndUpperCharCase);
			if (charactersData.isSingleRange()) {
				status = checkRangeAndAddToSet(rootAst, ast, set, a, b, false, status);
			}
			else {
				status = checkRangeAndAddToSet(rootAst, ast, set, charactersData.lowerFrom, charactersData.lowerTo, false, status);
				// Don't report similar warning twice
				status = checkRangeAndAddToSet(rootAst, ast, set, charactersData.upperFrom, charactersData.upperTo, false, status);
			}
		}
		else {
			bool charactersCollision = previousStatus != null && previousStatus.collision;
			if (!charactersCollision) {
				for (int i = a; i <= b; i++) {
					if (set.contains(i)) {
						String setText;
						if (rootAst.getChildren() == null) {
							setText = rootAst.getText();
						}
						else {
							StringBuilder sb = new StringBuilder();
							foreach (Object child in rootAst.getChildren()) {
								if (child is RangeAST) {
									sb.Append(((RangeAST) child).getChild(0).getText());
									sb.Append("..");
									sb.Append(((RangeAST) child).getChild(1).getText());
								}
								else {
									sb.Append(((GrammarAST) child).getText());
								}
								sb.Append(" | ");
							}
							sb.replace(sb.Length - 3, sb.Length, "");
							setText = sb.ToString();
						}
						String charsString = a == b ? String.valueOf((char)a) : (char) a + "-" + (char) b;
						g.tool.errMgr.grammarError(ErrorType.CHARACTERS_COLLISION_IN_SET, g.fileName, ast.getToken(),
								charsString, setText);
						charactersCollision = true;
						break;
					}
				}
			}
			status = new CharactersDataCheckStatus(charactersCollision, charactersData.mixOfLowerAndUpperCharCase);
			set.add(a, b);
		}
		return status;
	}

	private Transition createTransition(ATNState target, int from, int to, CommonTree tree) {
		RangeBorderCharactersData charactersData = RangeBorderCharactersData.getAndCheckCharactersData(from, to, g, tree, true);
		if (currentRule.caseInsensitive) {
			if (charactersData.isSingleRange()) {
				return CodePointTransitions.createWithCodePointRange(target, from, to);
			}
			else {
				IntervalSet intervalSet = new IntervalSet();
				intervalSet.add(charactersData.lowerFrom, charactersData.lowerTo);
				intervalSet.add(charactersData.upperFrom, charactersData.upperTo);
				return new SetTransition(target, intervalSet);
			}
		}
		else {
			return CodePointTransitions.createWithCodePointRange(target, from, to);
		}
	}

	////@Override
	public Handle tokenRef(TerminalAST node) {
		// Ref to EOF in lexer yields char transition on -1
		if (node.getText().Equals("EOF") ) {
			ATNState left = newState(node);
			ATNState right = newState(node);
			left.addTransition(new AtomTransition(right, IntStream.EOF));
			return new Handle(left, right);
		}
		return _ruleRef(node);
	}

	private LexerAction createLexerAction(GrammarAST ID, GrammarAST arg) {
		String command = ID.getText();
		checkCommands(command, ID.getToken());

		if ("skip".Equals(command) && arg == null) {
			return LexerSkipAction.INSTANCE;
		}
		else if ("more".Equals(command) && arg == null) {
			return LexerMoreAction.INSTANCE;
		}
		else if ("popMode".Equals(command) && arg == null) {
			return LexerPopModeAction.INSTANCE;
		}
		else if ("mode".Equals(command) && arg != null) {
			String modeName = arg.getText();
			int? mode = getModeConstantValue(modeName, arg.getToken());
			if (mode == null) {
				return null;
			}

			return new LexerModeAction(mode.GetValueOrDefault());
		}
		else if ("pushMode".Equals(command) && arg != null) {
			String modeName = arg.getText();
			int? mode = getModeConstantValue(modeName, arg.getToken());
			if (mode == null) {
				return null;
			}

			return new LexerPushModeAction(mode.GetValueOrDefault());
		}
		else if ("type".Equals(command) && arg != null) {
			String typeName = arg.getText();
			int? type = getTokenConstantValue(typeName, arg.getToken());
			if (type == null) {
				return null;
			}

			return new LexerTypeAction(type.GetValueOrDefault());
		}
		else if ("channel".Equals(command) && arg != null) {
			String channelName = arg.getText();
			int? channel = getChannelConstantValue(channelName, arg.getToken());
			if (channel == null) {
				return null;
			}

			return new LexerChannelAction(channel.GetValueOrDefault());
		}
		else {
			return null;
		}
	}

	private void checkCommands(String command, Token commandToken) {
		// Command combinations list: https://github.com/antlr/antlr4/issues/1388#issuecomment-263344701
		if (!command.Equals("pushMode") && !command.Equals("popMode")) {
			if (ruleCommands.Contains(command)) {
				g.tool.errMgr.grammarError(ErrorType.DUPLICATED_COMMAND, g.fileName, commandToken, command);
			}

			String firstCommand = null;

			if (command.Equals("skip")) {
				if (ruleCommands.Contains("more")) {
					firstCommand = "more";
				}
				else if (ruleCommands.Contains("type")) {
					firstCommand = "type";
				}
				else if (ruleCommands.Contains("channel")) {
					firstCommand = "channel";
				}
			}
			else if (command.Equals("more")) {
				if (ruleCommands.Contains("skip")) {
					firstCommand = "skip";
				}
				else if (ruleCommands.Contains("type")) {
					firstCommand = "type";
				}
				else if (ruleCommands.Contains("channel")) {
					firstCommand = "channel";
				}
			}
			else if (command.Equals("type") || command.Equals("channel")) {
				if (ruleCommands.Contains("more")) {
					firstCommand = "more";
				}
				else if (ruleCommands.Contains("skip")) {
					firstCommand = "skip";
				}
			}

			if (firstCommand != null) {
				g.tool.errMgr.grammarError(ErrorType.INCOMPATIBLE_COMMANDS, g.fileName, commandToken, firstCommand, command);
			}
		}

		ruleCommands.Add(command);
	}

	private int? getModeConstantValue(String modeName, Token token)
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
			g.tool.errMgr.grammarError(ErrorType.MODE_CONFLICTS_WITH_COMMON_CONSTANTS, g.fileName, token, token.getText());
			return null;
		}

		List<String> modeNames = new(((LexerGrammar)g).modes.Keys);
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
			g.tool.errMgr.grammarError(ErrorType.CONSTANT_VALUE_IS_NOT_A_RECOGNIZED_MODE_NAME, g.fileName, token, token.getText());
			return null;
		}
	}
	private int? getTokenConstantValue(String tokenName, Token token) {
		if (tokenName == null) {
			return null;
		}

		if (tokenName.Equals("EOF")) {
			return Lexer.EOF;
		}
		if (COMMON_CONSTANTS.ContainsKey(tokenName)) {
			g.tool.errMgr.grammarError(ErrorType.TOKEN_CONFLICTS_WITH_COMMON_CONSTANTS, g.fileName, token, token.getText());
			return null;
		}

		int tokenType = g.getTokenType(tokenName);
		if (tokenType != org.antlr.v4.runtime.Token.INVALID_TYPE) {
			return tokenType;
		}

		if (int.TryParse(tokenName, out var r))
		{
			return r;
		}
		else
		{
			g.tool.errMgr.grammarError(ErrorType.CONSTANT_VALUE_IS_NOT_A_RECOGNIZED_TOKEN_NAME, g.fileName, token, token.getText());
			return null;
		}
	}

	private int? getChannelConstantValue(String channelName, Token token) {
		if (channelName == null) {
			return null;
		}

		if (channelName.Equals("HIDDEN")) {
			return Lexer.HIDDEN;
		}
		if (channelName.Equals("DEFAULT_TOKEN_CHANNEL")) {
			return Lexer.DEFAULT_TOKEN_CHANNEL;
		}
		if (COMMON_CONSTANTS.ContainsKey(channelName)) {
			g.tool.errMgr.grammarError(ErrorType.CHANNEL_CONFLICTS_WITH_COMMON_CONSTANTS, g.fileName, token, token.getText());
			return null;
		}

		int channelValue = g.getChannelValue(channelName);
		if (channelValue >= org.antlr.v4.runtime.Token.MIN_USER_CHANNEL_VALUE) {
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
