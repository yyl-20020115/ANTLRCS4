/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.runtime.tree;
using org.antlr.v4.analysis;
using org.antlr.v4.parse;
using org.antlr.v4.runtime;
using org.antlr.v4.runtime.misc;
using org.antlr.v4.runtime.tree;
using org.antlr.v4.tool.ast;

namespace org.antlr.v4.tool;


/** Handle left-recursion and block-set transforms */
public class GrammarTransformPipeline
{
    public Grammar g;
    public Tool tool;

    public GrammarTransformPipeline(Grammar g, Tool tool)
    {
        this.g = g;
        this.tool = tool;
    }

    public void Process()
    {
        var root = g.ast;
        if (root == null) return;
        tool.Log("grammar", "before: " + root.ToStringTree());

        IntegrateImportedGrammars(g);
        ReduceBlocksToSets(root);
        ExpandParameterizedLoops(root);

        tool.Log("grammar", "after: " + root.ToStringTree());
    }

    public void ReduceBlocksToSets(GrammarAST root)
    {
        var nodes = new CommonTreeNodeStream(new GrammarASTAdaptor(), root);
        var adaptor = new GrammarASTAdaptor();
        var transformer = new BlockSetTransformer(nodes, g);
        transformer.        TreeAdaptor = adaptor;
        transformer.Downup(root);
    }
    public class TVA : TreeVisitorAction
    {
        public readonly GrammarTransformPipeline pipeline;
        public TVA(GrammarTransformPipeline pipline) => this.pipeline = pipline;
        public object Pre(object t) => ((GrammarAST)t).Type == 3 ? expandParameterizedLoop((GrammarAST)t) : t;
        public object Post(object t) => t;
    }
    /** Find and replace
     *      ID*[','] with ID (',' ID)*
     *      ID+[','] with ID (',' ID)+
     *      (x {action} y)+[','] with x {action} y (',' x {action} y)+
     *
     *  Parameter must be a token.
     *  todo: do we want?
     */
    public void ExpandParameterizedLoops(GrammarAST root)
    {
        var v = new TreeVisitor(new GrammarASTAdaptor());
        v.visit(root, new TVA(this));
    }

    public static GrammarAST expandParameterizedLoop(GrammarAST t)
    {
        // todo: update grammar, alter AST
        return t;
    }
    public class TVA2 : TreeVisitorAction
    {
        public object Pre(object t) { (t as GrammarAST).g = gx; return t; }
        public object Post(object t) => t;
    }
    /** Utility visitor that sets grammar ptr in each node */
    static Grammar gx;
    public static void SetGrammarPtr(Grammar g, GrammarAST tree)
    {
        if (tree == null) return;
        gx = g;
        // ensure each node has pointer to surrounding grammar
        TreeVisitor v = new TreeVisitor(new GrammarASTAdaptor());
        v.visit(tree, new TVA2());
    }

    public static void AugmentTokensWithOriginalPosition(Grammar g, GrammarAST tree)
    {
        if (tree == null) return;

        var optionsSubTrees = tree.GetNodesWithType(ANTLRParser.ELEMENT_OPTIONS);
        for (int i = 0; i < optionsSubTrees.Count; i++)
        {
            var t = optionsSubTrees[i];
            var elWithOpt = t.parent;
            if (elWithOpt is GrammarASTWithOptions options1)
            {
                var options = options1.Options;
                if (options.TryGetValue(LeftRecursiveRuleTransformer.TOKENINDEX_OPTION_NAME, out var on))
                {
                    var newTok = new GrammarToken(g, elWithOpt.Token);

                    newTok.originalTokenIndex = int.TryParse(on.Text, out var ox)
                        ? ox : throw new InvalidOperationException("ox");

                    elWithOpt.token = newTok;

                    var originalNode = g.ast.GetNodeWithTokenIndex(newTok.TokenIndex);
                    if (originalNode != null)
                    {
                        // update the AST node start/stop index to match the values
                        // of the corresponding node in the original parse tree.
                        elWithOpt.                        // update the AST node start/stop index to match the values
                        // of the corresponding node in the original parse tree.
                        TokenStartIndex = originalNode.TokenStartIndex;
                        elWithOpt.                        TokenStopIndex = originalNode.TokenStopIndex;
                    }
                    else
                    {
                        // the original AST node could not be located by index;
                        // make sure to assign valid values for the start/stop
                        // index so toTokenString will not throw exceptions.
                        elWithOpt.                        // the original AST node could not be located by index;
                        // make sure to assign valid values for the start/stop
                        // index so toTokenString will not throw exceptions.
                        TokenStartIndex = newTok.TokenIndex;
                        elWithOpt.                        TokenStopIndex = newTok.TokenIndex;
                    }
                }
            }
        }
    }

    /** Merge all the rules, token definitions, and named actions from
		imported grammars into the root grammar tree.  Perform:

	 	(tokens { X (= Y 'y')) + (tokens { Z )	-&gt;	(tokens { X (= Y 'y') Z)

	 	(@ members {foo}) + (@ members {bar})	-&gt;	(@ members {foobar})

	 	(RULES (RULE x y)) + (RULES (RULE z))	-&gt;	(RULES (RULE x y z))

	 	Rules in root prevent same rule from being appended to RULES node.

	 	The goal is a complete combined grammar so we can ignore subordinate
	 	grammars.
	 */
    public void IntegrateImportedGrammars(Grammar rootGrammar)
    {
        var imports = rootGrammar.GetAllImportedGrammars();
        if (imports == null) return;

        var root = rootGrammar.ast;
        var id = (GrammarAST)root.GetChild(0);
        var adaptor = new GrammarASTAdaptor(id.token.InputStream);

        var channelsRoot = (GrammarAST)root.GetFirstChildWithType(ANTLRParser.CHANNELS);
        var tokensRoot = (GrammarAST)root.GetFirstChildWithType(ANTLRParser.TOKENS_SPEC);

        var actionRoots = root.GetNodesWithType(ANTLRParser.AT);

        // Compute list of rules in root grammar and ensure we have a RULES node
        var RULES = (GrammarAST)root.GetFirstChildWithType(ANTLRParser.RULES);
        var rootRuleNames = new HashSet<string>();
        // make list of rules we have in root grammar
        var rootRules = RULES.GetNodesWithType(ANTLRParser.RULE);
        foreach (var r in rootRules) rootRuleNames.Add(r.GetChild(0).Text);

        // make list of modes we have in root grammar
        var rootModes = root.GetNodesWithType(ANTLRParser.MODE);
        var rootModeNames = new HashSet<string>();
        foreach (var m in rootModes) rootModeNames.Add(m.GetChild(0).Text);
        List<GrammarAST> addedModes = new();

        foreach (var imp in imports)
        {
            // COPY CHANNELS
            var imp_channelRoot = (GrammarAST)imp.ast.GetFirstChildWithType(ANTLRParser.CHANNELS);
            if (imp_channelRoot != null)
            {
                rootGrammar.Tools.Log("grammar", "imported channels: " + imp_channelRoot.GetChildren());
                if (channelsRoot == null)
                {
                    channelsRoot = imp_channelRoot.DupTree();
                    channelsRoot.g = rootGrammar;
                    root.InsertChild(1, channelsRoot); // ^(GRAMMAR ID TOKENS...)
                }
                else
                {
                    for (int c = 0; c < imp_channelRoot.ChildCount; ++c)
                    {
                        var channel = imp_channelRoot.GetChild(c).Text;
                        bool channelIsInRootGrammar = false;
                        for (int rc = 0; rc < channelsRoot.ChildCount; ++rc)
                        {
                            var rootChannel = channelsRoot.GetChild(rc).Text;
                            if (rootChannel.Equals(channel))
                            {
                                channelIsInRootGrammar = true;
                                break;
                            }
                        }
                        if (!channelIsInRootGrammar)
                        {
                            channelsRoot.AddChild(imp_channelRoot.GetChild(c).DupNode() as Tree);
                        }
                    }
                }
            }

            // COPY TOKENS
            var imp_tokensRoot = (GrammarAST)imp.ast.GetFirstChildWithType(ANTLRParser.TOKENS_SPEC);
            if (imp_tokensRoot != null)
            {
                rootGrammar.Tools.Log("grammar", "imported tokens: " + imp_tokensRoot.GetChildren());
                if (tokensRoot == null)
                {
                    tokensRoot = (GrammarAST)adaptor.Create(ANTLRParser.TOKENS_SPEC, "TOKENS");
                    tokensRoot.g = rootGrammar;
                    root.InsertChild(1, tokensRoot); // ^(GRAMMAR ID TOKENS...)
                }
                tokensRoot.AddChildren(Arrays.AsList(imp_tokensRoot.GetChildren().ToArray()));
            }

            List<GrammarAST> all_actionRoots = new();
            var imp_actionRoots = imp.ast.GetAllChildrenWithType(ANTLRParser.AT);
            if (actionRoots != null) all_actionRoots.AddRange(actionRoots);
            all_actionRoots.AddRange(imp_actionRoots);

            // COPY ACTIONS
            if (imp_actionRoots != null)
            {
                DoubleKeyMap<string, string, GrammarAST> namedActions =
                    new();

                rootGrammar.Tools.Log("grammar", "imported actions: " + imp_actionRoots);
                foreach (GrammarAST at in all_actionRoots)
                {
                    var scopeName = rootGrammar.GetDefaultActionScope();
                    GrammarAST scope, name, action;
                    if (at.ChildCount > 2)
                    { // must have a scope
                        scope = (GrammarAST)at.GetChild(0);
                        scopeName = scope.Text;
                        name = (GrammarAST)at.GetChild(1);
                        action = (GrammarAST)at.GetChild(2);
                    }
                    else
                    {
                        name = (GrammarAST)at.GetChild(0);
                        action = (GrammarAST)at.GetChild(1);
                    }
                    var prevAction = namedActions.Get(scopeName, name.Text);
                    if (prevAction == null)
                    {
                        namedActions.Put(scopeName, name.Text, action);
                    }
                    else
                    {
                        if (prevAction.g == at.g)
                        {
                            rootGrammar.Tools.ErrMgr.GrammarError(ErrorType.ACTION_REDEFINITION,
                                                at.g.fileName, name.token, name.Text);
                        }
                        else
                        {
                            var s1 = prevAction.Text;
                            s1 = s1[1..^1];
                            var s2 = action.Text;
                            s2 = s2[1..^1];
                            var combinedAction = "{" + s1 + '\n' + s2 + "}";
                            prevAction.token.Text = combinedAction;
                        }
                    }
                }
                // at this point, we have complete list of combined actions,
                // some of which are already living in root grammar.
                // Merge in any actions not in root grammar into root's tree.
                foreach (var scopeName in namedActions.KeySet())
                {
                    foreach (var name in namedActions.KeySet(scopeName))
                    {
                        var action = namedActions.Get(scopeName, name);
                        rootGrammar.Tools.Log("grammar", action.g.name + " " + scopeName + ":" + name + "=" + action.Text);
                        if (action.g != rootGrammar)
                        {
                            root.InsertChild(1, action.Parent);
                        }
                    }
                }
            }

            // COPY MODES
            // The strategy is to copy all the mode sections rules across to any
            // mode section in the new grammar with the same name or a new
            // mode section if no matching mode is resolved. Rules which are
            // already in the new grammar are ignored for copy. If the mode
            // section being added ends up empty it is not added to the merged
            // grammar.
            var modes = imp.ast.GetNodesWithType(ANTLRParser.MODE);
            if (modes != null)
            {
                foreach (var m in modes)
                {
                    rootGrammar.Tools.Log("grammar", "imported mode: " + m.ToStringTree());
                    var name = m.GetChild(0).Text;
                    bool rootAlreadyHasMode = rootModeNames.Contains(name);
                    GrammarAST destinationAST = null;
                    if (rootAlreadyHasMode)
                    {
                        foreach (var m2 in rootModes)
                        {
                            if (m2.GetChild(0).Text.Equals(name))
                            {
                                destinationAST = m2;
                                break;
                            }
                        }
                    }
                    else
                    {
                        destinationAST = m.DupNode();
                        destinationAST.AddChild(m.GetChild(0).DupNode() as Tree);
                    }

                    int addedRules = 0;
                    var modeRules = m.GetAllChildrenWithType(ANTLRParser.RULE);
                    foreach (var r in modeRules)
                    {
                        rootGrammar.Tools.Log("grammar", "imported rule: " + r.ToStringTree());
                        var ruleName = r.GetChild(0).Text;
                        bool rootAlreadyHasRule = rootRuleNames.Contains(ruleName);
                        if (!rootAlreadyHasRule)
                        {
                            destinationAST.AddChild(r);
                            addedRules++;
                            rootRuleNames.Add(ruleName);
                        }
                    }
                    if (!rootAlreadyHasMode && addedRules > 0)
                    {
                        rootGrammar.ast.AddChild(destinationAST);
                        rootModeNames.Add(name);
                        rootModes.Add(destinationAST);
                    }
                }
            }

            // COPY RULES
            // Rules copied in the mode copy phase are not copied again.
            var rules = imp.ast.GetNodesWithType(ANTLRParser.RULE);
            if (rules != null)
            {
                foreach (var r in rules)
                {
                    rootGrammar.Tools.Log("grammar", "imported rule: " + r.ToStringTree());
                    var name = r.GetChild(0).Text;
                    bool rootAlreadyHasRule = rootRuleNames.Contains(name);
                    if (!rootAlreadyHasRule)
                    {
                        RULES.AddChild(r); // merge in if not overridden
                        rootRuleNames.Add(name);
                    }
                }
            }

            var optionsRoot = (GrammarAST)imp.ast.GetFirstChildWithType(ANTLRParser.OPTIONS);
            if (optionsRoot != null)
            {
                // suppress the warning if the options match the options specified
                // in the root grammar
                // https://github.com/antlr/antlr4/issues/707

                bool hasNewOption = false;
                foreach (var option in imp.ast.Options)
                {
                    var importOption = imp.ast.GetOptionString(option.Key);
                    if (importOption == null)
                    {
                        continue;
                    }

                    var rootOption = rootGrammar.ast.GetOptionString(option.Key);
                    if (!importOption.Equals(rootOption))
                    {
                        hasNewOption = true;
                        break;
                    }
                }

                if (hasNewOption)
                {
                    rootGrammar.Tools.ErrMgr.GrammarError(ErrorType.OPTIONS_IN_DELEGATE,
                                        optionsRoot.g.fileName, optionsRoot.token, imp.name);
                }
            }
        }
        rootGrammar.Tools.Log("grammar", "Grammar: " + rootGrammar.ast.ToStringTree());
    }

    /** Build lexer grammar from combined grammar that looks like:
	 *
	 *  (COMBINED_GRAMMAR A
	 *      (tokens { X (= Y 'y'))
	 *      (OPTIONS (= x 'y'))
	 *      (@ members {foo})
	 *      (@ lexer header {package jj;})
	 *      (RULES (RULE .+)))
	 *
	 *  Move rules and actions to new tree, don't dup. Split AST apart.
	 *  We'll have this Grammar share token symbols later; don't generate
	 *  tokenVocab or tokens{} section.  Copy over named actions.
	 *
	 *  Side-effects: it removes children from GRAMMAR &amp; RULES nodes
	 *                in combined AST.  Anything cut out is dup'd before
	 *                adding to lexer to avoid "who's ur daddy" issues
	 */
    public GrammarRootAST ExtractImplicitLexer(Grammar combinedGrammar)
    {
        var combinedAST = combinedGrammar.ast;
        //tool.log("grammar", "before="+combinedAST.toStringTree());
        var adaptor = new GrammarASTAdaptor(combinedAST.token.InputStream);
        var elements = combinedAST.GetChildren().Cast<GrammarAST>().ToArray();

        // MAKE A GRAMMAR ROOT and ID
        var lexerName = combinedAST.GetChild(0).Text + "Lexer";
        var lexerAST =
            new GrammarRootAST(new CommonToken(ANTLRParser.GRAMMAR, "LEXER_GRAMMAR"), combinedGrammar.ast.tokenStream);
        lexerAST.grammarType = ANTLRParser.LEXER;
        lexerAST.token.InputStream = combinedAST.token.InputStream;
        lexerAST.AddChild((GrammarAST)adaptor.Create(ANTLRParser.ID, lexerName));

        // COPY OPTIONS
        var optionsRoot =
            (GrammarAST)combinedAST.GetFirstChildWithType(ANTLRParser.OPTIONS);
        if (optionsRoot != null && optionsRoot.ChildCount != 0)
        {
            var lexerOptionsRoot = (GrammarAST)adaptor.DupNode(optionsRoot);
            lexerAST.AddChild(lexerOptionsRoot);
            var options = optionsRoot.GetChildren().Cast<GrammarAST>().ToArray();
            foreach (var o in options)
            {
                var optionName = o.GetChild(0).Text;
                if (Grammar.lexerOptions.Contains(optionName) &&
                     !Grammar.doNotCopyOptionsToLexer.Contains(optionName))
                {
                    var optionTree = (GrammarAST)adaptor.DupTree(o);
                    lexerOptionsRoot.AddChild(optionTree);
                    lexerAST.SetOption(optionName, (GrammarAST)optionTree.GetChild(1));
                }
            }
        }

        // COPY all named actions, but only move those with lexer:: scope
        List<GrammarAST> actionsWeMoved = new();
        foreach (var e in elements)
        {
            if (e.Type == ANTLRParser.AT)
            {
                lexerAST.AddChild((Tree)adaptor.DupTree(e));
                if (e.GetChild(0).Text.Equals("lexer"))
                {
                    actionsWeMoved.Add(e);
                }
            }
        }

        foreach (var r in actionsWeMoved)
        {
            combinedAST.DeleteChild(r);
        }

        var combinedRulesRoot =
            (GrammarAST)combinedAST.GetFirstChildWithType(ANTLRParser.RULES);
        if (combinedRulesRoot == null) return lexerAST;

        // MOVE lexer rules

        var lexerRulesRoot =
            (GrammarAST)adaptor.Create(ANTLRParser.RULES, "RULES");
        lexerAST.AddChild(lexerRulesRoot);
        List<GrammarAST> rulesWeMoved = new();
        GrammarASTWithOptions[] rules;
        if (combinedRulesRoot.ChildCount > 0)
        {
            rules = combinedRulesRoot.GetChildren().Cast<GrammarASTWithOptions>().ToArray();
        }
        else
        {
            rules = new GrammarASTWithOptions[0];
        }

        foreach (var r in rules)
        {
            var ruleName = r.GetChild(0).Text;
            if (Grammar.IsTokenName(ruleName))
            {
                lexerRulesRoot.AddChild((Tree)adaptor.DupTree(r));
                rulesWeMoved.Add(r);
            }
        }
        foreach (GrammarAST r in rulesWeMoved)
        {
            combinedRulesRoot.DeleteChild(r);
        }

        // Will track 'if' from IF : 'if' ; rules to avoid defining new token for 'if'
        var litAliases =
            Grammar.GetStringLiteralAliasesFromLexerRules(lexerAST);

        var stringLiterals = combinedGrammar.GetStringLiterals();
        // add strings from combined grammar (and imported grammars) into lexer
        // put them first as they are keywords; must resolve ambigs to these rules
        //		tool.log("grammar", "strings from parser: "+stringLiterals);
        int insertIndex = 0;
    //nextLit:
        foreach (var lit in stringLiterals)
        {
            // if lexer already has a rule for literal, continue
            if (litAliases != null)
            {
                foreach (var pair in litAliases)
                {
                    var litAST = pair.b;
                    if (lit.Equals(litAST.Text)) continue;// nextLit;
                }
            }
            // create for each literal: (RULE <uniquename> (BLOCK (ALT <lit>))
            var rname = combinedGrammar.GetStringLiteralLexerRuleName(lit);
            // can't use wizard; need special node types
            var litRule = new RuleAST(ANTLRParser.RULE);
            var blk = new BlockAST(ANTLRParser.BLOCK);
            var alt = new AltAST(ANTLRParser.ALT);
            var slit = new TerminalAST(new CommonToken(ANTLRParser.STRING_LITERAL, lit));
            alt.AddChild(slit);
            blk.AddChild(alt);
            var idToken = new CommonToken(ANTLRParser.TOKEN_REF, rname);
            litRule.AddChild(new TerminalAST(idToken));
            litRule.AddChild(blk);
            lexerRulesRoot.InsertChild(insertIndex, litRule);
            //			lexerRulesRoot.getChildren().add(0, litRule);
            lexerRulesRoot.FreshenParentAndChildIndexes(); // reset indexes and set litRule parent

            // next literal will be added after the one just added
            insertIndex++;
        }

        // TODO: take out after stable if slow
        lexerAST.SanityCheckParentAndChildIndexes();
        combinedAST.SanityCheckParentAndChildIndexes();
        //		tool.log("grammar", combinedAST.toTokenString());

        combinedGrammar.Tools.Log("grammar", "after extract implicit lexer =" + combinedAST.ToStringTree());
        combinedGrammar.Tools.Log("grammar", "lexer =" + lexerAST.ToStringTree());

        if (lexerRulesRoot.ChildCount == 0) return null;
        return lexerAST;
    }

}
