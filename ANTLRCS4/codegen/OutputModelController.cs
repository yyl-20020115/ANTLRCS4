/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using Antlr4.StringTemplate;
using org.antlr.runtime.tree;
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
public class OutputModelController
{
    /** Who does the work? Doesn't have to be CoreOutputModelFactory. */
    public OutputModelFactory @delegate;

    /** Post-processing CodeGeneratorExtension objects; done in order given. */
    public List<CodeGeneratorExtension> extensions = new();

    /** While walking code in rules, this is set to the tree walker that
	 *  triggers actions.
	 */
    public SourceGenTriggers walker;

    /** Context set by the SourceGenTriggers.g */
    public int codeBlockLevel = -1;
    public int treeLevel = -1;
    public OutputModelObject root; // normally ParserFile, LexerFile, ...
    public Stack<RuleFunction> currentRule = new();
    public Alternative currentOuterMostAlt;
    public CodeBlock currentBlock;
    public CodeBlockForOuterMostAlt currentOuterMostAlternativeBlock;

    public OutputModelController(OutputModelFactory factory)
    {
        this.@delegate = factory;
    }

    public void AddExtension(CodeGeneratorExtension ext) { extensions.Add(ext); }

    /** Build a file with a parser containing rule functions. Use the
	 *  controller as factory in SourceGenTriggers so it triggers codegen
	 *  extensions too, not just the factory functions in this factory.
	 */
    public OutputModelObject BuildParserOutputModel(bool header)
    {
        var gen = @delegate.GetGenerator();
        var file = ParserFile(gen.GetRecognizerFileName(header));
        SetRoot(file);
        file.parser = Parser(file);

        var g = @delegate.GetGrammar();
        foreach (Rule r in g.rules.Values)
        {
            BuildRuleFunction(file.parser, r);
        }

        return file;
    }

    public OutputModelObject BuildLexerOutputModel(bool header)
    {
        var gen = @delegate.GetGenerator();
        var file = LexerFile(gen.GetRecognizerFileName(header));
        SetRoot(file);
        file.lexer = Lexer(file);

        var g = @delegate.GetGrammar();
        foreach (var r in g.rules.Values)
        {
            BuildLexerRuleActions(file.lexer, r);
        }

        return file;
    }

    public OutputModelObject BuildListenerOutputModel(bool header)
    {
        var gen = @delegate.GetGenerator();
        return new ListenerFile(@delegate, gen.GetListenerFileName(header));
    }

    public OutputModelObject BuildBaseListenerOutputModel(bool header)
    {
        var gen = @delegate.GetGenerator();
        return new BaseListenerFile(@delegate, gen.GetBaseListenerFileName(header));
    }

    public OutputModelObject BuildVisitorOutputModel(bool header)
    {
        var gen = @delegate.GetGenerator();
        return new VisitorFile(@delegate, gen.GetVisitorFileName(header));
    }

    public OutputModelObject BuildBaseVisitorOutputModel(bool header)
    {
        var gen = @delegate.GetGenerator();
        return new BaseVisitorFile(@delegate, gen.GetBaseVisitorFileName(header));
    }

    public ParserFile ParserFile(String fileName)
    {
        var f = @delegate.ParserFile(fileName);
        foreach (CodeGeneratorExtension ext in extensions) f = ext.ParserFile(f);
        return f;
    }

    public Parser Parser(ParserFile file)
    {
        Parser p = @delegate.Parser(file);
        foreach (CodeGeneratorExtension ext in extensions) p = ext.Parser(p);
        return p;
    }

    public LexerFile LexerFile(String fileName)
    {
        return new LexerFile(@delegate, fileName);
    }

    public Lexer Lexer(LexerFile file)
    {
        return new Lexer(@delegate, file);
    }

    /** Create RuleFunction per rule and update sempreds,actions of parser
	 *  output object with stuff found in r.
	 */
    public void BuildRuleFunction(model.Parser parser, Rule r)
    {
        var function = Rule(r);
        parser.funcs.Add(function);
        PushCurrentRule(function);
        function.FillNamedActions(@delegate, r);

        if (r is LeftRecursiveRule rule1)
        {
            BuildLeftRecursiveRuleFunction(rule1,
                                           (LeftRecursiveRuleFunction)function);
        }
        else
        {
            BuildNormalRuleFunction(r, function);
        }

        var g = GetGrammar();
        foreach (var a in r.actions)
        {
            if (a is PredAST)
            {
                var p = (PredAST)a;
                if (!parser.sempredFuncs.TryGetValue(r, out var rsf))
                {
                    rsf = new RuleSempredFunction(@delegate, r, function.ctxType);
                    parser.sempredFuncs[r] = rsf;
                }
                rsf.actions[g.sempreds[(p)]] = new Action(@delegate, p);
            }
        }

        PopCurrentRule();
    }

    public void BuildLeftRecursiveRuleFunction(LeftRecursiveRule r, LeftRecursiveRuleFunction function)
    {
        BuildNormalRuleFunction(r, function);

        // now inject code to start alts
        var gen = @delegate.GetGenerator();
        var codegenTemplates = gen.Templates;

        // pick out alt(s) for primaries
        var outerAlt = (CodeBlockForOuterMostAlt)function.code[(0)];
        List<CodeBlockForAlt> primaryAltsCode = new();
        var primaryStuff = outerAlt.ops[(0)];
        if (primaryStuff is Choice)
        {
            var primaryAltBlock = (Choice)primaryStuff;
            primaryAltsCode.AddRange(primaryAltBlock.alts);
        }
        else
        { // just a single alt I guess; no block
            primaryAltsCode.Add((CodeBlockForAlt)primaryStuff);
        }

        // pick out alt(s) for op alts
        var opAltStarBlock = (StarBlock)outerAlt.ops[(1)];
        var altForOpAltBlock = opAltStarBlock.alts[(0)];
        List<CodeBlockForAlt> opAltsCode = new();
        var opStuff = altForOpAltBlock.ops[(0)];
        if (opStuff is AltBlock)
        {
            var opAltBlock = (AltBlock)opStuff;
            opAltsCode.AddRange(opAltBlock.alts);
        }
        else
        { // just a single alt I guess; no block
            opAltsCode.Add((CodeBlockForAlt)opStuff);
        }

        // Insert code in front of each primary alt to create specialized ctx if there was a label
        for (int i = 0; i < primaryAltsCode.Count; i++)
        {
            var altInfo = r.recPrimaryAlts[(i)];
            if (altInfo.altLabel == null) continue;
            var altActionST = codegenTemplates.GetInstanceOf("recRuleReplaceContext");
            altActionST.Add("ctxName", Utils.Capitalize(altInfo.altLabel));
            var altAction =
                new Action(@delegate, function.altLabelCtxs[(altInfo.altLabel)], altActionST);
            var alt = primaryAltsCode[(i)];
            alt.InsertOp(0, altAction);
        }

        // Insert code to set ctx.stop after primary block and before op * loop
        var setStopTokenAST = codegenTemplates.GetInstanceOf("recRuleSetStopToken");
        var setStopTokenAction = new Action(@delegate, function.ruleCtx, setStopTokenAST);
        outerAlt.InsertOp(1, setStopTokenAction);

        // Insert code to set _prevctx at start of * loop
        var setPrevCtx = codegenTemplates.GetInstanceOf("recRuleSetPrevCtx");
        var setPrevCtxAction = new Action(@delegate, function.ruleCtx, setPrevCtx);
        opAltStarBlock.AddIterationOp(setPrevCtxAction);

        // Insert code in front of each op alt to create specialized ctx if there was an alt label
        for (int i = 0; i < opAltsCode.Count; i++)
        {
            Template altActionST;
            var altInfo = r.recOpAlts.GetElement(i);
            string templateName;
            if (altInfo.altLabel != null)
            {
                templateName = "recRuleLabeledAltStartAction";
                altActionST = codegenTemplates.GetInstanceOf(templateName);
                altActionST.Add("currentAltLabel", altInfo.altLabel);
            }
            else
            {
                templateName = "recRuleAltStartAction";
                altActionST = codegenTemplates.GetInstanceOf(templateName);
                altActionST.Add("ctxName", Utils.Capitalize(r.name));
            }
            altActionST.Add("ruleName", r.name);
            // add label of any lr ref we deleted
            altActionST.Add("label", altInfo.leftRecursiveRuleRefLabel);
            if (altActionST.impl.FormalArguments.Any(f => f.Name == "isListLabel"))
            {
                altActionST.Add("isListLabel", altInfo.isListLabel);
            }
            else if (altInfo.isListLabel)
            {
                @delegate.GetGenerator().tool.ErrMgr.toolError(ErrorType.CODE_TEMPLATE_ARG_ISSUE, templateName, "isListLabel");
            }
            Action altAction =
                new Action(@delegate, function.altLabelCtxs[(altInfo.altLabel)], altActionST);
            CodeBlockForAlt alt = opAltsCode[i];
            alt.InsertOp(0, altAction);
        }
    }

    public void BuildNormalRuleFunction(Rule r, RuleFunction function)
    {
        var gen = @delegate.GetGenerator();
        // TRIGGER factory functions for rule alts, elements
        var adaptor = new GrammarASTAdaptor(r.ast.token.getInputStream());
        var blk = (GrammarAST)r.ast.getFirstChildWithType(ANTLRParser.BLOCK);
        var nodes = new CommonTreeNodeStream(adaptor, blk);
        walker = new SourceGenTriggers(nodes, this);
        try
        {
            // walk AST of rule alts/elements
            function.code = DefaultOutputModelFactory.List(walker.Block(null, null));
            function.hasLookaheadBlock = walker.hasLookaheadBlock;
        }
        catch (RecognitionException e)
        {
            //e.printStackTrace(System.err);
        }

        function.ctxType = gen.Target.GetRuleFunctionContextStructName(function);

        function.postamble = RulePostamble(function, r);
    }

    public void BuildLexerRuleActions(Lexer lexer, Rule r)
    {
        if (r.actions.Count == 0)
        {
            return;
        }

        var gen = @delegate.GetGenerator();
        var g = @delegate.GetGrammar();
        var ctxType = gen.Target.GetRuleFunctionContextStructName(r);
        if (!lexer.actionFuncs.TryGetValue(r, out var raf))
        {
            raf = new RuleActionFunction(@delegate, r, ctxType);
        }

        foreach (var a in r.actions)
        {
            if (a is PredAST)
            {
                PredAST p = (PredAST)a;
                if (lexer.sempredFuncs.TryGetValue(r, out var rsf))
                {
                    rsf = new RuleSempredFunction(@delegate, r, ctxType);
                    lexer.sempredFuncs.Add(r, rsf);
                }
                rsf.actions[g.sempreds[(p)]] = new Action(@delegate, p);
            }
            else if (a.getType() == ANTLRParser.ACTION)
            {
                raf.actions[g.lexerActions[(a)]] = new Action(@delegate, a);
            }
        }

        if (raf.actions.Count > 0 && !lexer.actionFuncs.ContainsKey(r))
        {
            // only add to lexer if the function actually contains actions
            lexer.actionFuncs[r] = raf;
        }
    }

    public RuleFunction Rule(Rule r)
    {
        RuleFunction rf = @delegate.Rule(r);
        foreach (CodeGeneratorExtension ext in extensions) rf = ext.Rule(rf);
        return rf;
    }

    public List<SrcOp> RulePostamble(RuleFunction function, Rule r)
    {
        List<SrcOp> ops = @delegate.RulePostamble(function, r);
        foreach (CodeGeneratorExtension ext in extensions) ops = ext.RulePostamble(ops);
        return ops;
    }

    public Grammar GetGrammar() { return @delegate.GetGrammar(); }

    public CodeGenerator GetGenerator() { return @delegate.GetGenerator(); }

    public CodeBlockForAlt Alternative(Alternative alt, bool outerMost)
    {
        CodeBlockForAlt blk = @delegate.Alternative(alt, outerMost);
        if (outerMost)
        {
            currentOuterMostAlternativeBlock = (CodeBlockForOuterMostAlt)blk;
        }
        foreach (CodeGeneratorExtension ext in extensions) blk = ext.Alternative(blk, outerMost);
        return blk;
    }

    public CodeBlockForAlt FinishAlternative(CodeBlockForAlt blk, List<SrcOp> ops,
                                             bool outerMost)
    {
        blk = @delegate.FinishAlternative(blk, ops);
        foreach (CodeGeneratorExtension ext in extensions) blk = ext.FinishAlternative(blk, outerMost);
        return blk;
    }

    public List<SrcOp> RuleRef(GrammarAST ID, GrammarAST label, GrammarAST args)
    {
        List<SrcOp> ops = @delegate.RuleRef(ID, label, args);
        foreach (CodeGeneratorExtension ext in extensions)
        {
            ops = ext.RuleRef(ops);
        }
        return ops;
    }

    public List<SrcOp> TokenRef(GrammarAST ID, GrammarAST label, GrammarAST args)
    {
        List<SrcOp> ops = @delegate.TokenRef(ID, label, args);
        foreach (CodeGeneratorExtension ext in extensions)
        {
            ops = ext.TokenRef(ops);
        }
        return ops;
    }

    public List<SrcOp> StringRef(GrammarAST ID, GrammarAST label)
    {
        List<SrcOp> ops = @delegate.StringRef(ID, label);
        foreach (CodeGeneratorExtension ext in extensions)
        {
            ops = ext.StringRef(ops);
        }
        return ops;
    }

    /** (A|B|C) possibly with ebnfRoot and label */
    public List<SrcOp> Set(GrammarAST setAST, GrammarAST labelAST, bool invert)
    {
        List<SrcOp> ops = @delegate.Set(setAST, labelAST, invert);
        foreach (CodeGeneratorExtension ext in extensions)
        {
            ops = ext.Set(ops);
        }
        return ops;
    }

    public CodeBlockForAlt Epsilon(Alternative alt, bool outerMost)
    {
        CodeBlockForAlt blk = @delegate.Epsilon(alt, outerMost);
        foreach (CodeGeneratorExtension ext in extensions) blk = ext.Epsilon(blk);
        return blk;
    }

    public List<SrcOp> Wildcard(GrammarAST ast, GrammarAST labelAST)
    {
        List<SrcOp> ops = @delegate.Wildcard(ast, labelAST);
        foreach (CodeGeneratorExtension ext in extensions)
        {
            ops = ext.Wildcard(ops);
        }
        return ops;
    }

    public List<SrcOp> Action(ActionAST ast)
    {
        List<SrcOp> ops = @delegate.Action(ast);
        foreach (CodeGeneratorExtension ext in extensions) ops = ext.Action(ops);
        return ops;
    }

    public List<SrcOp> Sempred(ActionAST ast)
    {
        List<SrcOp> ops = @delegate.Sempred(ast);
        foreach (CodeGeneratorExtension ext in extensions) ops = ext.Sempred(ops);
        return ops;
    }

    public Choice GetChoiceBlock(BlockAST blkAST, List<CodeBlockForAlt> alts, GrammarAST label)
    {
        Choice c = @delegate.GetChoiceBlock(blkAST, alts, label);
        foreach (CodeGeneratorExtension ext in extensions) c = ext.GetChoiceBlock(c);
        return c;
    }

    public Choice GetEBNFBlock(GrammarAST ebnfRoot, List<CodeBlockForAlt> alts)
    {
        Choice c = @delegate.GetEBNFBlock(ebnfRoot, alts);
        foreach (CodeGeneratorExtension ext in extensions) c = ext.GetEBNFBlock(c);
        return c;
    }

    public bool NeedsImplicitLabel(GrammarAST ID, LabeledOp op)
    {
        bool needs = @delegate.NeedsImplicitLabel(ID, op);
        foreach (CodeGeneratorExtension ext in extensions) needs |= ext.NeedsImplicitLabel(ID, op);
        return needs;
    }

    public OutputModelObject GetRoot() { return root; }

    public void SetRoot(OutputModelObject root) { this.root = root; }

    public RuleFunction GetCurrentRuleFunction()
    {
        if (currentRule.Count > 0) return currentRule.Peek();
        return null;
    }

    public void PushCurrentRule(RuleFunction r) { currentRule.Push(r); }

    public RuleFunction PopCurrentRule()
    {
        if (currentRule.Count > 0) return currentRule.Pop();
        return null;
    }

    public Alternative GetCurrentOuterMostAlt() { return currentOuterMostAlt; }

    public void SetCurrentOuterMostAlt(Alternative currentOuterMostAlt) { this.currentOuterMostAlt = currentOuterMostAlt; }

    public CodeBlock CurrentBlock { get => currentBlock; set => currentBlock = value; }

    public CodeBlockForOuterMostAlt CurrentOuterMostAlternativeBlock { get => currentOuterMostAlternativeBlock; set => this.currentOuterMostAlternativeBlock = value; }

    public int CodeBlockLevel => codeBlockLevel;
}
