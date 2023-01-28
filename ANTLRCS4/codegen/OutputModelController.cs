/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using Antlr4.StringTemplate;
using org.antlr.runtime.tree;
using org.antlr.v4.analysis;
using org.antlr.v4.codegen.model;
using org.antlr.v4.codegen.model.decl;
using org.antlr.v4.misc;
using org.antlr.v4.parse;
using org.antlr.v4.runtime;
using org.antlr.v4.tool;
using org.antlr.v4.tool.ast;
using Action = org.antlr.v4.codegen.model.Action;
using Lexer = org.antlr.v4.codegen.model.Lexer;
using Parser = org.antlr.v4.codegen.model.Parser;

namespace org.antlr.v4.codegen;


/** This receives events from SourceGenTriggers.g and asks factory to do work.
 *  Then runs extensions in order on resulting SrcOps to get final list.
 **/
public class OutputModelController {
	/** Who does the work? Doesn't have to be CoreOutputModelFactory. */
	public OutputModelFactory @delegate;

	/** Post-processing CodeGeneratorExtension objects; done in order given. */
	public List<CodeGeneratorExtension> extensions = new ();

	/** While walking code in rules, this is set to the tree walker that
	 *  triggers actions.
	 */
	public SourceGenTriggers walker;

	/** Context set by the SourceGenTriggers.g */
	public int codeBlockLevel = -1;
	public int treeLevel = -1;
	public OutputModelObject root; // normally ParserFile, LexerFile, ...
	public Stack<RuleFunction> currentRule = new Stack<RuleFunction>();
	public Alternative currentOuterMostAlt;
	public CodeBlock currentBlock;
	public CodeBlockForOuterMostAlt currentOuterMostAlternativeBlock;

	public OutputModelController(OutputModelFactory factory) {
		this.@delegate = factory;
	}

	public void addExtension(CodeGeneratorExtension ext) { extensions.Add(ext); }

	/** Build a file with a parser containing rule functions. Use the
	 *  controller as factory in SourceGenTriggers so it triggers codegen
	 *  extensions too, not just the factory functions in this factory.
	 */
	public OutputModelObject buildParserOutputModel(bool header) {
		CodeGenerator gen = @delegate.getGenerator();
		ParserFile file = parserFile(gen.getRecognizerFileName(header));
		setRoot(file);
		file.parser = parser(file);

		Grammar g = @delegate.getGrammar();
		foreach (Rule r in g.rules.Values) {
			buildRuleFunction(file.parser, r);
		}

		return file;
	}

	public OutputModelObject buildLexerOutputModel(bool header) {
		CodeGenerator gen = @delegate.getGenerator();
		LexerFile file = lexerFile(gen.getRecognizerFileName(header));
		setRoot(file);
		file.lexer = lexer(file);

		Grammar g = @delegate.getGrammar();
        foreach (Rule r in g.rules.Values) {
			buildLexerRuleActions(file.lexer, r);
		}

		return file;
	}

	public OutputModelObject buildListenerOutputModel(bool header) {
		CodeGenerator gen = @delegate.getGenerator();
		return new ListenerFile(@delegate, gen.getListenerFileName(header));
	}

	public OutputModelObject buildBaseListenerOutputModel(bool header) {
		CodeGenerator gen = @delegate.getGenerator();
		return new BaseListenerFile(@delegate, gen.getBaseListenerFileName(header));
	}

	public OutputModelObject buildVisitorOutputModel(bool header) {
		CodeGenerator gen = @delegate.getGenerator();
		return new VisitorFile(@delegate, gen.getVisitorFileName(header));
	}

	public OutputModelObject buildBaseVisitorOutputModel(bool header) {
		CodeGenerator gen = @delegate.getGenerator();
		return new BaseVisitorFile(@delegate, gen.getBaseVisitorFileName(header));
	}

	public ParserFile parserFile(String fileName) {
		ParserFile f = @delegate.parserFile(fileName);
        foreach (CodeGeneratorExtension ext in extensions) f = ext.parserFile(f);
		return f;
	}

	public Parser parser(ParserFile file) {
		Parser p = @delegate.parser(file);
        foreach (CodeGeneratorExtension ext in extensions) p = ext.parser(p);
		return p;
	}

	public LexerFile lexerFile(String fileName) {
		return new LexerFile(@delegate, fileName);
	}

	public Lexer lexer(LexerFile file) {
		return new Lexer(@delegate, file);
	}

	/** Create RuleFunction per rule and update sempreds,actions of parser
	 *  output object with stuff found in r.
	 */
	public void buildRuleFunction(model.Parser parser, Rule r) {
		RuleFunction function = rule(r);
		parser.funcs.Add(function);
		pushCurrentRule(function);
		function.fillNamedActions(@delegate, r);

		if ( r is LeftRecursiveRule ) {
			buildLeftRecursiveRuleFunction((LeftRecursiveRule)r,
										   (LeftRecursiveRuleFunction)function);
		}
		else {
			buildNormalRuleFunction(r, function);
		}

		Grammar g = getGrammar();
        foreach (ActionAST a in r.actions) {
			if ( a is PredAST ) {
				PredAST p = (PredAST)a;
				if (!parser.sempredFuncs.TryGetValue(r,out var rsf)) {
					rsf = new RuleSempredFunction(@delegate, r, function.ctxType);
					parser.sempredFuncs[r]=rsf;
				}
				rsf.actions[g.sempreds[(p)]]= new Action(@delegate, p);
			}
		}

		popCurrentRule();
	}

	public void buildLeftRecursiveRuleFunction(LeftRecursiveRule r, LeftRecursiveRuleFunction function) {
		buildNormalRuleFunction(r, function);

		// now inject code to start alts
		CodeGenerator gen = @delegate.getGenerator();
		var codegenTemplates = gen.getTemplates();

		// pick out alt(s) for primaries
		CodeBlockForOuterMostAlt outerAlt = (CodeBlockForOuterMostAlt)function.code[(0)];
		List<CodeBlockForAlt> primaryAltsCode = new ();
		SrcOp primaryStuff = outerAlt.ops[(0)];
		if ( primaryStuff is Choice ) {
			Choice primaryAltBlock = (Choice) primaryStuff;
			primaryAltsCode.AddRange(primaryAltBlock.alts);
		}
		else { // just a single alt I guess; no block
			primaryAltsCode.Add((CodeBlockForAlt)primaryStuff);
		}

		// pick out alt(s) for op alts
		StarBlock opAltStarBlock = (StarBlock)outerAlt.ops[(1)];
		CodeBlockForAlt altForOpAltBlock = opAltStarBlock.alts[(0)];
		List<CodeBlockForAlt> opAltsCode = new ();
		SrcOp opStuff = altForOpAltBlock.ops[(0)];
		if ( opStuff is AltBlock ) {
			AltBlock opAltBlock = (AltBlock)opStuff;
			opAltsCode.AddRange(opAltBlock.alts);
		}
		else { // just a single alt I guess; no block
			opAltsCode.Add((CodeBlockForAlt)opStuff);
		}

		// Insert code in front of each primary alt to create specialized ctx if there was a label
		for (int i = 0; i < primaryAltsCode.Count; i++) {
			LeftRecursiveRuleAltInfo altInfo = r.recPrimaryAlts[(i)];
			if ( altInfo.altLabel==null ) continue;
			var altActionST = codegenTemplates.GetInstanceOf("recRuleReplaceContext");
			altActionST.Add("ctxName", Utils.capitalize(altInfo.altLabel));
			Action altAction =
				new Action(@delegate, function.altLabelCtxs[(altInfo.altLabel)], altActionST);
			CodeBlockForAlt alt = primaryAltsCode[(i)];
			alt.insertOp(0, altAction);
		}

		// Insert code to set ctx.stop after primary block and before op * loop
		Template setStopTokenAST = codegenTemplates.GetInstanceOf("recRuleSetStopToken");
		Action setStopTokenAction = new Action(@delegate, function.ruleCtx, setStopTokenAST);
		outerAlt.insertOp(1, setStopTokenAction);

		// Insert code to set _prevctx at start of * loop
		var setPrevCtx = codegenTemplates.GetInstanceOf("recRuleSetPrevCtx");
		Action setPrevCtxAction = new Action(@delegate, function.ruleCtx, setPrevCtx);
		opAltStarBlock.addIterationOp(setPrevCtxAction);

		// Insert code in front of each op alt to create specialized ctx if there was an alt label
		for (int i = 0; i < opAltsCode.Count; i++) {
			Template altActionST;
			LeftRecursiveRuleAltInfo altInfo = r.recOpAlts.getElement(i);
			String templateName;
			if ( altInfo.altLabel!=null ) {
				templateName = "recRuleLabeledAltStartAction";
				altActionST = codegenTemplates.GetInstanceOf(templateName);
				altActionST.Add("currentAltLabel", altInfo.altLabel);
			}
			else {
				templateName = "recRuleAltStartAction";
				altActionST = codegenTemplates.GetInstanceOf(templateName);
				altActionST.Add("ctxName", Utils.capitalize(r.name));
			}
			altActionST.Add("ruleName", r.name);
			// add label of any lr ref we deleted
			altActionST.Add("label", altInfo.leftRecursiveRuleRefLabel);
			if (altActionST.impl.FormalArguments.Any(f=>f.Name== "isListLabel")) {
				altActionST.Add("isListLabel", altInfo.isListLabel);
			}
			else if (altInfo.isListLabel) {
				@delegate.getGenerator().tool.errMgr.toolError(ErrorType.CODE_TEMPLATE_ARG_ISSUE, templateName, "isListLabel");
			}
			Action altAction =
				new Action(@delegate, function.altLabelCtxs[(altInfo.altLabel)], altActionST);
			CodeBlockForAlt alt = opAltsCode[i];
			alt.insertOp(0, altAction);
		}
	}

	public void buildNormalRuleFunction(Rule r, RuleFunction function) {
		CodeGenerator gen = @delegate.getGenerator();
		// TRIGGER factory functions for rule alts, elements
		GrammarASTAdaptor adaptor = new GrammarASTAdaptor(r.ast.token.getInputStream());
		GrammarAST blk = (GrammarAST)r.ast.getFirstChildWithType(ANTLRParser.BLOCK);
		CommonTreeNodeStream nodes = new CommonTreeNodeStream(adaptor,blk);
		walker = new SourceGenTriggers(nodes, this);
		try {
			// walk AST of rule alts/elements
			function.code = DefaultOutputModelFactory.list(walker.block(null, null));
			function.hasLookaheadBlock = walker.hasLookaheadBlock;
		}
		catch (RecognitionException e){
			//e.printStackTrace(System.err);
		}

		function.ctxType = gen.getTarget().getRuleFunctionContextStructName(function);

		function.postamble = rulePostamble(function, r);
	}

	public void buildLexerRuleActions(Lexer lexer, Rule r) {
		if (r.actions.Count==0) {
			return;
		}

		CodeGenerator gen = @delegate.getGenerator();
		Grammar g = @delegate.getGrammar();
		String ctxType = gen.getTarget().getRuleFunctionContextStructName(r);
		if ( !lexer.actionFuncs.TryGetValue(r,out var raf) ) {
			raf = new RuleActionFunction(@delegate, r, ctxType);
		}

		foreach (ActionAST a in r.actions) {
			if ( a is PredAST ) {
				PredAST p = (PredAST)a;
				if ( lexer.sempredFuncs.TryGetValue(r,out var rsf)) {
					rsf = new RuleSempredFunction(@delegate, r, ctxType);
					lexer.sempredFuncs.Add(r, rsf);
				}
				rsf.actions[g.sempreds[(p)]]= new Action(@delegate, p);
			}
			else if ( a.getType()== ANTLRParser.ACTION ) {
				raf.actions[g.lexerActions[(a)]]= new Action(@delegate, a);
			}
		}

		if (raf.actions.Count>0 && !lexer.actionFuncs.ContainsKey(r)) {
			// only add to lexer if the function actually contains actions
			lexer.actionFuncs[r]= raf;
		}
	}

	public RuleFunction rule(Rule r) {
		RuleFunction rf = @delegate.rule(r);
        foreach (CodeGeneratorExtension ext in extensions) rf = ext.rule(rf);
		return rf;
	}

	public List<SrcOp> rulePostamble(RuleFunction function, Rule r) {
		List<SrcOp> ops = @delegate.rulePostamble(function, r);
        foreach (CodeGeneratorExtension ext in extensions) ops = ext.rulePostamble(ops);
		return ops;
	}

	public Grammar getGrammar() { return @delegate.getGrammar(); }

	public CodeGenerator getGenerator() { return @delegate.getGenerator(); }

	public CodeBlockForAlt alternative(Alternative alt, bool outerMost) {
		CodeBlockForAlt blk = @delegate.alternative(alt, outerMost);
		if ( outerMost ) {
			currentOuterMostAlternativeBlock = (CodeBlockForOuterMostAlt)blk;
		}
		foreach (CodeGeneratorExtension ext in extensions) blk = ext.alternative(blk, outerMost);
		return blk;
	}

	public CodeBlockForAlt finishAlternative(CodeBlockForAlt blk, List<SrcOp> ops,
											 bool outerMost)
	{
		blk = @delegate.finishAlternative(blk, ops);
        foreach (CodeGeneratorExtension ext in extensions) blk = ext.finishAlternative(blk, outerMost);
		return blk;
	}

	public List<SrcOp> ruleRef(GrammarAST ID, GrammarAST label, GrammarAST args) {
		List<SrcOp> ops = @delegate.ruleRef(ID, label, args);
        foreach (CodeGeneratorExtension ext in extensions) {
			ops = ext.ruleRef(ops);
		}
		return ops;
	}

	public List<SrcOp> tokenRef(GrammarAST ID, GrammarAST label, GrammarAST args)
	{
		List<SrcOp> ops = @delegate.tokenRef(ID, label, args);
        foreach (CodeGeneratorExtension ext in extensions) {
			ops = ext.tokenRef(ops);
		}
		return ops;
	}

	public List<SrcOp> stringRef(GrammarAST ID, GrammarAST label) {
		List<SrcOp> ops = @delegate.stringRef(ID, label);
        foreach (CodeGeneratorExtension ext in extensions) {
			ops = ext.stringRef(ops);
		}
		return ops;
	}

	/** (A|B|C) possibly with ebnfRoot and label */
	public List<SrcOp> set(GrammarAST setAST, GrammarAST labelAST, bool invert) {
		List<SrcOp> ops = @delegate.set(setAST, labelAST, invert);
        foreach (CodeGeneratorExtension ext in extensions) {
			ops = ext.set(ops);
		}
		return ops;
	}

	public CodeBlockForAlt epsilon(Alternative alt, bool outerMost) {
		CodeBlockForAlt blk = @delegate.epsilon(alt, outerMost);
        foreach (CodeGeneratorExtension ext in extensions) blk = ext.epsilon(blk);
		return blk;
	}

	public List<SrcOp> wildcard(GrammarAST ast, GrammarAST labelAST) {
		List<SrcOp> ops = @delegate.wildcard(ast, labelAST);
		foreach (CodeGeneratorExtension ext in extensions) {
			ops = ext.wildcard(ops);
		}
		return ops;
	}

	public List<SrcOp> action(ActionAST ast) {
		List<SrcOp> ops = @delegate.action(ast);
        foreach (CodeGeneratorExtension ext in extensions) ops = ext.action(ops);
		return ops;
	}

	public List<SrcOp> sempred(ActionAST ast) {
		List<SrcOp> ops = @delegate.sempred(ast);
        foreach (CodeGeneratorExtension ext in extensions) ops = ext.sempred(ops);
		return ops;
	}

	public Choice getChoiceBlock(BlockAST blkAST, List<CodeBlockForAlt> alts, GrammarAST label) {
		Choice c = @delegate.getChoiceBlock(blkAST, alts, label);
        foreach (CodeGeneratorExtension ext in extensions) c = ext.getChoiceBlock(c);
		return c;
	}

	public Choice getEBNFBlock(GrammarAST ebnfRoot, List<CodeBlockForAlt> alts) {
		Choice c = @delegate.getEBNFBlock(ebnfRoot, alts);
        foreach (CodeGeneratorExtension ext in extensions) c = ext.getEBNFBlock(c);
		return c;
	}

	public bool needsImplicitLabel(GrammarAST ID, LabeledOp op) {
		bool needs = @delegate.needsImplicitLabel(ID, op);
        foreach (CodeGeneratorExtension ext in extensions) needs |= ext.needsImplicitLabel(ID, op);
		return needs;
	}

	public OutputModelObject getRoot() { return root; }

	public void setRoot(OutputModelObject root) { this.root = root; }

	public RuleFunction getCurrentRuleFunction() {
		if (currentRule.Count > 0)	return currentRule.Peek();
		return null;
	}

	public void pushCurrentRule(RuleFunction r) { currentRule.Push(r); }

	public RuleFunction popCurrentRule() {
		if ( currentRule.Count>0 ) return currentRule.Pop();
		return null;
	}

	public Alternative getCurrentOuterMostAlt() { return currentOuterMostAlt; }

	public void setCurrentOuterMostAlt(Alternative currentOuterMostAlt) { this.currentOuterMostAlt = currentOuterMostAlt; }

	public void setCurrentBlock(CodeBlock blk) {
		currentBlock = blk;
	}

	public CodeBlock getCurrentBlock() {
		return currentBlock;
	}

	public void setCurrentOuterMostAlternativeBlock(CodeBlockForOuterMostAlt currentOuterMostAlternativeBlock) {
		this.currentOuterMostAlternativeBlock = currentOuterMostAlternativeBlock;
	}

	public CodeBlockForOuterMostAlt getCurrentOuterMostAlternativeBlock() {
		return currentOuterMostAlternativeBlock;
	}

	public int getCodeBlockLevel() { return codeBlockLevel; }
}
