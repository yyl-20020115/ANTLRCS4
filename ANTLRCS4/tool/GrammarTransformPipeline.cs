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
using org.antlr.v4.tool;
using org.antlr.v4.tool.ast;

namespace org.antlr.v4.tool;


/** Handle left-recursion and block-set transforms */
public class GrammarTransformPipeline {
	public Grammar g;
	public Tool tool;

	public GrammarTransformPipeline(Grammar g, Tool tool) {
		this.g = g;
		this.tool = tool;
	}

	public void process() {
		GrammarRootAST root = g.ast;
		if ( root==null ) return;
        tool.Log("grammar", "before: "+root.ToStringTree());

        integrateImportedGrammars(g);
		reduceBlocksToSets(root);
        expandParameterizedLoops(root);

        tool.Log("grammar", "after: "+root.ToStringTree());
	}

	public void reduceBlocksToSets(GrammarAST root) {
		CommonTreeNodeStream nodes = new CommonTreeNodeStream(new GrammarASTAdaptor(), root);
		GrammarASTAdaptor adaptor = new GrammarASTAdaptor();
		BlockSetTransformer transformer = new BlockSetTransformer(nodes, g);
		transformer.setTreeAdaptor(adaptor);
		transformer.downup(root);
	}
	public class TVA: TreeVisitorAction
    {
		public readonly GrammarTransformPipeline pipeline;
		public TVA(GrammarTransformPipeline pipline)
		{
			this.pipeline = pipline;
		}
        //@Override
        public Object pre(Object t)
        {
            if (((GrammarAST)t).getType() == 3)
            {
                return pipeline.expandParameterizedLoop((GrammarAST)t);
            }
            return t;
        }
        //@Override
        public Object post(Object t) { return t; }
    }
    /** Find and replace
     *      ID*[','] with ID (',' ID)*
     *      ID+[','] with ID (',' ID)+
     *      (x {action} y)+[','] with x {action} y (',' x {action} y)+
     *
     *  Parameter must be a token.
     *  todo: do we want?
     */
    public void expandParameterizedLoops(GrammarAST root) {
        TreeVisitor v = new TreeVisitor(new GrammarASTAdaptor());
        v.visit(root, new TVA(this));
    }

    public GrammarAST expandParameterizedLoop(GrammarAST t) {
        // todo: update grammar, alter AST
        return t;
    }
	public class TVA2 : TreeVisitorAction
    {
        //@Override
        public Object pre(Object t) { ((GrammarAST)t).g = gx; return t; }
        //@Override
        public Object post(Object t) { return t; }
    }
	/** Utility visitor that sets grammar ptr in each node */
	static Grammar gx;
    public static void setGrammarPtr(Grammar g, GrammarAST tree) {
		if ( tree==null ) return;
		gx = g;
		// ensure each node has pointer to surrounding grammar
		TreeVisitor v = new TreeVisitor(new GrammarASTAdaptor());
		v.visit(tree,new TVA2() );
	}

	public static void augmentTokensWithOriginalPosition( Grammar g, GrammarAST tree) {
		if ( tree==null ) return;

		List<GrammarAST> optionsSubTrees = tree.getNodesWithType(ANTLRParser.ELEMENT_OPTIONS);
		for (int i = 0; i < optionsSubTrees.Count; i++) {
			GrammarAST t = optionsSubTrees[i];
			CommonTree elWithOpt = t.parent;
			if ( elWithOpt is GrammarASTWithOptions ) {
				Dictionary<String, GrammarAST> options = ((GrammarASTWithOptions) elWithOpt).getOptions();
				if ( options.TryGetValue(LeftRecursiveRuleTransformer.TOKENINDEX_OPTION_NAME,out var on) ) {
					GrammarToken newTok = new GrammarToken(g, elWithOpt.Token);

                    newTok.originalTokenIndex = int.TryParse(on.getText(), out var ox) 
						? ox : throw new InvalidOperationException("ox");

					elWithOpt.token = newTok;

					GrammarAST originalNode = g.ast.getNodeWithTokenIndex(newTok.getTokenIndex());
					if (originalNode != null) {
						// update the AST node start/stop index to match the values
						// of the corresponding node in the original parse tree.
						elWithOpt.setTokenStartIndex(originalNode.getTokenStartIndex());
						elWithOpt.setTokenStopIndex(originalNode.getTokenStopIndex());
					}
					else {
						// the original AST node could not be located by index;
						// make sure to assign valid values for the start/stop
						// index so toTokenString will not throw exceptions.
						elWithOpt.setTokenStartIndex(newTok.getTokenIndex());
						elWithOpt.setTokenStopIndex(newTok.getTokenIndex());
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
	public void integrateImportedGrammars(Grammar rootGrammar) {
		List<Grammar> imports = rootGrammar.getAllImportedGrammars();
		if ( imports==null ) return;

		GrammarAST root = rootGrammar.ast;
		GrammarAST id = (GrammarAST) root.GetChild(0);
		GrammarASTAdaptor adaptor = new GrammarASTAdaptor(id.token.InputStream);

		GrammarAST channelsRoot = (GrammarAST)root.GetFirstChildWithType(ANTLRParser.CHANNELS);
	 	GrammarAST tokensRoot = (GrammarAST)root.GetFirstChildWithType(ANTLRParser.TOKENS_SPEC);

		List<GrammarAST> actionRoots = root.getNodesWithType(ANTLRParser.AT);

		// Compute list of rules in root grammar and ensure we have a RULES node
		GrammarAST RULES = (GrammarAST)root.GetFirstChildWithType(ANTLRParser.RULES);
		HashSet<String> rootRuleNames = new HashSet<String>();
		// make list of rules we have in root grammar
		List<GrammarAST> rootRules = RULES.getNodesWithType(ANTLRParser.RULE);
		foreach (GrammarAST r in rootRules) rootRuleNames.Add(r.GetChild(0).Text);

		// make list of modes we have in root grammar
		List<GrammarAST> rootModes = root.getNodesWithType(ANTLRParser.MODE);
		HashSet<String> rootModeNames = new HashSet<String>();
        foreach (GrammarAST m in rootModes) rootModeNames.Add(m.GetChild(0).Text);
		List<GrammarAST> addedModes = new ();

        foreach (Grammar imp in imports) {
			// COPY CHANNELS
			GrammarAST imp_channelRoot = (GrammarAST)imp.ast.GetFirstChildWithType(ANTLRParser.CHANNELS);
			if ( imp_channelRoot != null) {
				rootGrammar.Tools.Log("grammar", "imported channels: "+imp_channelRoot.GetChildren());
				if (channelsRoot==null) {
					channelsRoot = imp_channelRoot.dupTree();
					channelsRoot.g = rootGrammar;
					root.InsertChild(1, channelsRoot); // ^(GRAMMAR ID TOKENS...)
				} else {
					for (int c = 0; c < imp_channelRoot.ChildCount; ++c) {
						String channel = imp_channelRoot.GetChild(c).Text;
						bool channelIsInRootGrammar = false;
						for (int rc = 0; rc < channelsRoot.ChildCount; ++rc) {
							String rootChannel = channelsRoot.GetChild(rc).Text;
							if (rootChannel.Equals(channel)) {
								channelIsInRootGrammar = true;
								break;
							}
						}
						if (!channelIsInRootGrammar) {
                            channelsRoot.AddChild(imp_channelRoot.GetChild(c).DupNode() as Tree);
						}
					}
				}
			}

			// COPY TOKENS
			GrammarAST imp_tokensRoot = (GrammarAST)imp.ast.GetFirstChildWithType(ANTLRParser.TOKENS_SPEC);
			if ( imp_tokensRoot!=null ) {
				rootGrammar.Tools.Log("grammar", "imported tokens: "+imp_tokensRoot.GetChildren());
				if ( tokensRoot==null ) {
					tokensRoot = (GrammarAST)adaptor.create(ANTLRParser.TOKENS_SPEC, "TOKENS");
					tokensRoot.g = rootGrammar;
					root.InsertChild(1, tokensRoot); // ^(GRAMMAR ID TOKENS...)
				}
				tokensRoot.AddChildren(Arrays.AsList(imp_tokensRoot.GetChildren().ToArray()));
			}

			List<GrammarAST> all_actionRoots = new ();
			List<GrammarAST> imp_actionRoots = imp.ast.getAllChildrenWithType(ANTLRParser.AT);
			if ( actionRoots!=null ) all_actionRoots.AddRange(actionRoots);
			all_actionRoots.AddRange(imp_actionRoots);

			// COPY ACTIONS
			if ( imp_actionRoots!=null ) {
				DoubleKeyMap<String, String, GrammarAST> namedActions =
					new DoubleKeyMap<String, String, GrammarAST>();

				rootGrammar.Tools.Log("grammar", "imported actions: "+imp_actionRoots);
				foreach (GrammarAST at in all_actionRoots) {
					String scopeName = rootGrammar.getDefaultActionScope();
					GrammarAST scope, name, action;
					if ( at.ChildCount>2 ) { // must have a scope
						scope = (GrammarAST)at.GetChild(0);
						scopeName = scope.getText();
						name = (GrammarAST)at.GetChild(1);
						action = (GrammarAST)at.GetChild(2);
					}
					else {
						name = (GrammarAST)at.GetChild(0);
						action = (GrammarAST)at.GetChild(1);
					}
					GrammarAST prevAction = namedActions.Get(scopeName, name.getText());
					if ( prevAction==null ) {
						namedActions.Put(scopeName, name.getText(), action);
					}
					else {
						if ( prevAction.g == at.g ) {
							rootGrammar.Tools.ErrMgr.GrammarError(ErrorType.ACTION_REDEFINITION,
												at.g.fileName, name.token, name.getText());
						}
						else {
							String s1 = prevAction.getText();
							s1 = s1.Substring(1, s1.Length-1-1);
							String s2 = action.getText();
							s2 = s2.Substring(1, s2.Length-1-1);
							String combinedAction = "{"+s1 + '\n'+ s2+"}";
							prevAction.token.							Text = combinedAction;
						}
					}
				}
                // at this point, we have complete list of combined actions,
                // some of which are already living in root grammar.
                // Merge in any actions not in root grammar into root's tree.
                foreach (String scopeName in namedActions.KeySet()) {
                    foreach (String name in namedActions.KeySet(scopeName)) {
						GrammarAST action = namedActions.Get(scopeName, name);
						rootGrammar.Tools.Log("grammar", action.g.name+" "+scopeName+":"+name+"="+action.getText());
						if ( action.g != rootGrammar ) {
							root.InsertChild(1, action.getParent());
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
            List<GrammarAST> modes = imp.ast.getNodesWithType(ANTLRParser.MODE);
			if (modes != null) {
				foreach (GrammarAST m in modes) {
					rootGrammar.Tools.Log("grammar", "imported mode: " + m.ToStringTree());
					String name = m.GetChild(0).Text;
					bool rootAlreadyHasMode = rootModeNames.Contains(name);
					GrammarAST destinationAST = null;
					if (rootAlreadyHasMode) {
		                foreach (GrammarAST m2 in rootModes) {
							if (m2.GetChild(0).Text.Equals(name)) {
                                destinationAST = m2;
								break;
							}
						}
					} else {
						destinationAST = m.dupNode();
						destinationAST.AddChild(m.GetChild(0).DupNode() as Tree);
					}

					int addedRules = 0;
					List<GrammarAST> modeRules = m.getAllChildrenWithType(ANTLRParser.RULE);
					foreach (GrammarAST r in modeRules) {
					    rootGrammar.Tools.Log("grammar", "imported rule: "+r.ToStringTree());
						String ruleName = r.GetChild(0).Text;
					    bool rootAlreadyHasRule = rootRuleNames.Contains(ruleName);
					    if (!rootAlreadyHasRule) {
						    destinationAST.AddChild(r);
							addedRules++;
						    rootRuleNames.Add(ruleName);
					    }
					}
					if (!rootAlreadyHasMode && addedRules > 0) {
						rootGrammar.ast.AddChild(destinationAST);
						rootModeNames.Add(name);
						rootModes.Add(destinationAST);
					}
				}
			}

			// COPY RULES
			// Rules copied in the mode copy phase are not copied again.
			List<GrammarAST> rules = imp.ast.getNodesWithType(ANTLRParser.RULE);
			if ( rules!=null ) {
				foreach (GrammarAST r in rules) {
					rootGrammar.Tools.Log("grammar", "imported rule: "+r.ToStringTree());
					String name = r.GetChild(0).Text;
					bool rootAlreadyHasRule = rootRuleNames.Contains(name);
					if ( !rootAlreadyHasRule ) {
						RULES.AddChild(r); // merge in if not overridden
						rootRuleNames.Add(name);
					}
				}
			}

			GrammarAST optionsRoot = (GrammarAST)imp.ast.GetFirstChildWithType(ANTLRParser.OPTIONS);
			if ( optionsRoot!=null ) {
				// suppress the warning if the options match the options specified
				// in the root grammar
				// https://github.com/antlr/antlr4/issues/707

				bool hasNewOption = false;
				foreach (var option in imp.ast.getOptions()) {
					String importOption = imp.ast.getOptionString(option.Key);
					if (importOption == null) {
						continue;
					}

					String rootOption = rootGrammar.ast.getOptionString(option.Key);
					if (!importOption.Equals(rootOption)) {
						hasNewOption = true;
						break;
					}
				}

				if (hasNewOption) {
					rootGrammar.Tools.ErrMgr.GrammarError(ErrorType.OPTIONS_IN_DELEGATE,
										optionsRoot.g.fileName, optionsRoot.token, imp.name);
				}
			}
		}
		rootGrammar.Tools.Log("grammar", "Grammar: "+rootGrammar.ast.ToStringTree());
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
	public GrammarRootAST extractImplicitLexer(Grammar combinedGrammar) {
		GrammarRootAST combinedAST = combinedGrammar.ast;
		//tool.log("grammar", "before="+combinedAST.toStringTree());
		GrammarASTAdaptor adaptor = new GrammarASTAdaptor(combinedAST.token.InputStream);
		GrammarAST[] elements = combinedAST.GetChildren().Cast<GrammarAST>().ToArray();

		// MAKE A GRAMMAR ROOT and ID
		String lexerName = combinedAST.GetChild(0).Text+"Lexer";
		GrammarRootAST lexerAST =
		    new GrammarRootAST(new CommonToken(ANTLRParser.GRAMMAR, "LEXER_GRAMMAR"), combinedGrammar.ast.tokenStream);
		lexerAST.grammarType = ANTLRParser.LEXER;
		lexerAST.token.		InputStream = combinedAST.token.InputStream;
		lexerAST.AddChild((GrammarAST)adaptor.create(ANTLRParser.ID, lexerName));

		// COPY OPTIONS
		GrammarAST optionsRoot =
			(GrammarAST)combinedAST.GetFirstChildWithType(ANTLRParser.OPTIONS);
		if ( optionsRoot!=null && optionsRoot.ChildCount!=0 ) {
			GrammarAST lexerOptionsRoot = (GrammarAST)adaptor.dupNode(optionsRoot);
			lexerAST.AddChild(lexerOptionsRoot);
			GrammarAST[] options = optionsRoot.GetChildren().Cast<GrammarAST>().ToArray();
			foreach (GrammarAST o in options) {
				String optionName = o.GetChild(0).Text;
				if ( Grammar.lexerOptions.Contains(optionName) &&
					 !Grammar.doNotCopyOptionsToLexer.Contains(optionName) )
				{
					GrammarAST optionTree = (GrammarAST)adaptor.dupTree(o);
					lexerOptionsRoot.AddChild(optionTree);
					lexerAST.setOption(optionName, (GrammarAST)optionTree.GetChild(1));
				}
			}
		}

		// COPY all named actions, but only move those with lexer:: scope
		List<GrammarAST> actionsWeMoved = new ();
		foreach (GrammarAST e in elements) {
			if ( e.getType()==ANTLRParser.AT ) {
				lexerAST.AddChild((Tree)adaptor.dupTree(e));
				if ( e.GetChild(0).Text.Equals("lexer") ) {
					actionsWeMoved.Add(e);
				}
			}
		}

        foreach (GrammarAST r in actionsWeMoved) {
			combinedAST.deleteChild( r );
		}

		GrammarAST combinedRulesRoot =
			(GrammarAST)combinedAST.GetFirstChildWithType(ANTLRParser.RULES);
		if ( combinedRulesRoot==null ) return lexerAST;

		// MOVE lexer rules

		GrammarAST lexerRulesRoot =
			(GrammarAST)adaptor.create(ANTLRParser.RULES, "RULES");
		lexerAST.AddChild(lexerRulesRoot);
		List<GrammarAST> rulesWeMoved = new ();
		GrammarASTWithOptions[] rules;
		if (combinedRulesRoot.ChildCount > 0) {
			rules = combinedRulesRoot.GetChildren().Cast<GrammarASTWithOptions>().ToArray();
		}
		else {
			rules = new GrammarASTWithOptions[0];
		}

        foreach (GrammarASTWithOptions r in rules) {
			String ruleName = r.GetChild(0).Text;
			if (Grammar.isTokenName(ruleName)) {
				lexerRulesRoot.AddChild((Tree)adaptor.dupTree(r));
				rulesWeMoved.Add(r);
			}
		}
        foreach (GrammarAST r in rulesWeMoved) {
			combinedRulesRoot.deleteChild( r );
		}

		// Will track 'if' from IF : 'if' ; rules to avoid defining new token for 'if'
		List<Pair<GrammarAST,GrammarAST>> litAliases =
			Grammar.getStringLiteralAliasesFromLexerRules(lexerAST);

		HashSet<String> stringLiterals = combinedGrammar.getStringLiterals();
		// add strings from combined grammar (and imported grammars) into lexer
		// put them first as they are keywords; must resolve ambigs to these rules
//		tool.log("grammar", "strings from parser: "+stringLiterals);
		int insertIndex = 0;
		nextLit:
        foreach (String lit in stringLiterals) {
			// if lexer already has a rule for literal, continue
			if ( litAliases!=null ) {
                foreach (Pair<GrammarAST,GrammarAST> pair in litAliases) {
					GrammarAST litAST = pair.b;
					if (lit.Equals(litAST.getText())) continue;// nextLit;
				}
			}
			// create for each literal: (RULE <uniquename> (BLOCK (ALT <lit>))
			String rname = combinedGrammar.getStringLiteralLexerRuleName(lit);
			// can't use wizard; need special node types
			GrammarAST litRule = new RuleAST(ANTLRParser.RULE);
			BlockAST blk = new BlockAST(ANTLRParser.BLOCK);
			AltAST alt = new AltAST(ANTLRParser.ALT);
			TerminalAST slit = new TerminalAST(new CommonToken(ANTLRParser.STRING_LITERAL, lit));
			alt.AddChild(slit);
			blk.AddChild(alt);
			CommonToken idToken = new CommonToken(ANTLRParser.TOKEN_REF, rname);
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

        combinedGrammar.Tools.Log("grammar", "after extract implicit lexer ="+combinedAST.ToStringTree());
        combinedGrammar.Tools.Log("grammar", "lexer ="+lexerAST.ToStringTree());

		if ( lexerRulesRoot.ChildCount==0 )	return null;
		return lexerAST;
	}

}
