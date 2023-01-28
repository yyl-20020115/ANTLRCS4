/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.runtime.tree;
using org.antlr.v4.codegen.model.decl;
using org.antlr.v4.misc;
using org.antlr.v4.parse;
using org.antlr.v4.runtime;
using org.antlr.v4.runtime.atn;
using org.antlr.v4.runtime.misc;
using org.antlr.v4.tool;
using org.antlr.v4.tool.ast;
using Attribute = org.antlr.v4.tool.Attribute;
using RuntimeUtils = org.antlr.v4.runtime.misc.RuntimeUtils;

namespace org.antlr.v4.codegen.model;


/** */
public class RuleFunction : OutputModelObject {
	public readonly String name;
	public readonly String escapedName;
	public readonly List<String> modifiers;
	public String ctxType;
	public readonly ICollection<String> ruleLabels;
	public readonly ICollection<String> tokenLabels;
	public readonly ATNState startState;
	public readonly int index;
	public readonly Rule rule;
	public readonly AltLabelStructDecl[] altToContext;
	public bool hasLookaheadBlock;

	//@ModelElement 
		public List<SrcOp> code;
    //@ModelElement 
    public OrderedHashSet<Decl> locals; // TODO: move into ctx?
	//@ModelElement 
		public ICollection<AttributeDecl> args = null;
    //@ModelElement 
    public StructDecl ruleCtx;
    //@ModelElement 
    public Dictionary<String,AltLabelStructDecl> altLabelCtxs;
    //@ModelElement 
    public Dictionary<String,Action> namedActions;
    //@ModelElement 
    public Action finallyAction;
    //@ModelElement 
    public List<ExceptionClause> exceptions;
    //@ModelElement 
    public List<SrcOp> postamble;

	public RuleFunction(OutputModelFactory factory, Rule r): base(factory)
    {
		;
		this.name = r.name;
		this.escapedName = factory.getGenerator().getTarget().escapeIfNeeded(r.name);
		this.rule = r;
		modifiers = RuntimeUtils.nodesToStrings(r.modifiers);

		index = r.index;

		ruleCtx = new StructDecl(factory, r);
		altToContext = new AltLabelStructDecl[r.getOriginalNumberOfAlts()+1];
		addContextGetters(factory, r);

		if ( r.args!=null ) {
			ICollection<Attribute> decls = r.args.attributes.Values;
			if ( decls.Count>0 ) {
				args = new List<AttributeDecl>();
				ruleCtx.addDecls(decls);
				foreach (var a in decls) {
					args.Add(new AttributeDecl(factory, a));
				}
				ruleCtx.ctorAttrs = args;
			}
		}
		if ( r.retvals!=null ) {
			ruleCtx.addDecls(r.retvals.attributes.Values);
		}
		if ( r.locals!=null ) {
			ruleCtx.addDecls(r.locals.attributes.Values);
		}

		ruleLabels = r.getElementLabelNames();
		tokenLabels = r.getTokenRefs();
		if ( r.exceptions!=null ) {
			exceptions = new ();
			foreach (GrammarAST e in r.exceptions) {
				ActionAST catchArg = (ActionAST)e.getChild(0);
				ActionAST catchAction = (ActionAST)e.getChild(1);
				exceptions.Add(new ExceptionClause(factory, catchArg, catchAction));
			}
		}

		startState = factory.getGrammar().atn.ruleToStartState[r.index];
	}

	public void addContextGetters(OutputModelFactory factory, Rule r) {
		// Add ctx labels for elements in alts with no -> label
		List<AltAST> altsNoLabels = r.getUnlabeledAltASTs();
		if ( altsNoLabels!=null ) {
			HashSet<Decl> decls = getDeclsForAllElements(altsNoLabels);
			// we know to put in rule ctx, so do it directly
			foreach (Decl d in decls) ruleCtx.addDecl(d);
		}

		// make structs for -> labeled alts, define ctx labels for elements
		altLabelCtxs = new ();
		var labels = r.getAltLabels();
		if ( labels!=null ) {
			foreach (var entry in labels) {
				String label = entry.Key;
				List<AltAST> alts = new ();
				foreach (var pair in entry.Value) {
					alts.Add(pair.b);
				}

				HashSet<Decl> decls = getDeclsForAllElements(alts);
				foreach (var pair in entry.Value) {
					int altNum = pair.a;
					altToContext[altNum] = new AltLabelStructDecl(factory, r, altNum, label);
					if (!altLabelCtxs.ContainsKey(label)) {
						altLabelCtxs[label]= altToContext[altNum];
					}

					// we know which ctx to put in, so do it directly
					foreach (Decl d in decls) {
						altToContext[altNum].addDecl(d);
					}
				}
			}
		}
	}

	public void fillNamedActions(OutputModelFactory factory, Rule r) {
		if ( r.finallyAction!=null ) {
			finallyAction = new Action(factory, r.finallyAction);
		}

		namedActions = new();
        foreach (String name in r.namedActions.Keys) {
			ActionAST ast = r.namedActions[(name)];
			namedActions[name]= new Action(factory, ast);
		}
	}

	/** for all alts, find which ref X or r needs List
	   Must see across alts. If any alt needs X or r as list, then
	   define as list.
	 */
	public HashSet<Decl> getDeclsForAllElements(List<AltAST> altASTs) {
        HashSet<String> needsList = new HashSet<String>();
        HashSet<String> nonOptional = new HashSet<String>();
		List<GrammarAST> allRefs = new ();
		bool firstAlt = true;
		IntervalSet reftypes = new IntervalSet(RULE_REF, TOKEN_REF, STRING_LITERAL);
		foreach(AltAST ast in altASTs) {
			List<GrammarAST> refs = getRuleTokens(ast.getNodesWithType(reftypes));
			allRefs.AddRange(refs);
			Pair<FrequencySet<String>, FrequencySet<String>> minAndAltFreq = getElementFrequenciesForAlt(ast);
			FrequencySet<String> minFreq = minAndAltFreq.a;
			FrequencySet<String> altFreq = minAndAltFreq.b;
			foreach (GrammarAST t in refs) {
				String refLabelName = getName(t);

				if (refLabelName != null) {
					if (altFreq.count(refLabelName) > 1) {
						needsList.Add(refLabelName);
					}

					if (firstAlt && minFreq.count(refLabelName) != 0) {
						nonOptional.Add(refLabelName);
					}
				}
			}

			foreach (String @ref in nonOptional.ToArray()) {
				if (minFreq.count(@ref) == 0) {
					nonOptional.Remove(@ref);
				}
			}

			firstAlt = false;
		}
		HashSet<Decl> decls = new ();
		foreach (GrammarAST t in allRefs) {
			String refLabelName = getName(t);

			if (refLabelName == null) {
				continue;
			}

			List<Decl> d = getDeclForAltElement(t,
												refLabelName,
												needsList.Contains(refLabelName),
												!nonOptional.Contains(refLabelName));
			decls.UnionWith(d);
		}
		return decls;
	}

	private List<GrammarAST> getRuleTokens(List<GrammarAST> refs) {
		List<GrammarAST> result = new (refs.Count);
		foreach (GrammarAST @ref in refs) {
			CommonTree r = @ref;

			bool ignore = false;
			while (r != null) {
				// Ignore string literals in predicates
				if (r is PredAST) {
					ignore = true;
					break;
				}
				r = r.parent;
			}

			if (!ignore) {
				result.Add(@ref);
			}
		}

		return result;
	}

	private String getName(GrammarAST token) {
		String tokenText = token.getText();
		String tokenName = token.getType() != STRING_LITERAL ? tokenText : token.g.getTokenName(tokenText);
		return tokenName == null || tokenName.StartsWith("T__") ? null : tokenName; // Do not include tokens with auto generated names
	}

	/** Given list of X and r refs in alt, compute how many of each there are */
	protected Pair<FrequencySet<String>, FrequencySet<String>> getElementFrequenciesForAlt(AltAST ast) {
		try {
			ElementFrequenciesVisitor visitor = new ElementFrequenciesVisitor(new CommonTreeNodeStream(new GrammarASTAdaptor(), ast));
			visitor.outerAlternative();
			if (visitor.frequencies.size() != 1) {
				factory.getGrammar().tool.errMgr.toolError(ErrorType.INTERNAL_ERROR);
				return new (new FrequencySet<String>(), new FrequencySet<String>());
			}

			return new (visitor.getMinFrequencies(), visitor.frequencies.peek());
		}
		catch (RecognitionException ex) {
			factory.getGrammar().tool.errMgr.toolError(ErrorType.INTERNAL_ERROR, ex);
			return new (new FrequencySet<String>(), new FrequencySet<String>());
		}
	}

	public List<Decl> getDeclForAltElement(GrammarAST t, String refLabelName, bool needList, bool optional) {
		List<Decl> decls = new ();
		if ( t.getType()==RULE_REF ) {
			Rule rref = factory.getGrammar().getRule(t.getText());
			String ctxName = factory.getGenerator().getTarget()
							 .getRuleFunctionContextStructName(rref);
			if ( needList) {
				if(factory.getGenerator().getTarget().supportsOverloadedMethods())
					decls.Add( new ContextRuleListGetterDecl(factory, refLabelName, ctxName) );
				decls.Add( new ContextRuleListIndexedGetterDecl(factory, refLabelName, ctxName) );
			}
			else {
				decls.Add( new ContextRuleGetterDecl(factory, refLabelName, ctxName, optional) );
			}
		}
		else {
			if ( needList ) {
				if(factory.getGenerator().getTarget().supportsOverloadedMethods())
					decls.Add( new ContextTokenListGetterDecl(factory, refLabelName) );
				decls.Add( new ContextTokenListIndexedGetterDecl(factory, refLabelName) );
			}
			else {
				decls.Add( new ContextTokenGetterDecl(factory, refLabelName, optional) );
			}
		}
		return decls;
	}

	/** Add local var decl */
	public void addLocalDecl(Decl d) {
		if ( locals ==null ) locals = new OrderedHashSet<Decl>();
		locals.Add(d);
		d.isLocal = true;
	}

	/** Add decl to struct ctx for rule or alt if labeled */
	public void addContextDecl(String altLabel, Decl d) {
		CodeBlockForOuterMostAlt alt = d.getOuterMostAltCodeBlock();
		// if we found code blk and might be alt label, try to Add to that label ctx
		if ( alt!=null && altLabelCtxs!=null ) {
//			Console.Out.WriteLine(d.name+" lives in alt "+alt.alt.altNum);
			AltLabelStructDecl altCtx = altLabelCtxs.get(altLabel);
			if ( altCtx!=null ) { // we have an alt ctx
//				Console.Out.WriteLine("ctx is "+ altCtx.name);
				altCtx.addDecl(d);
				return;
			}
		}
		ruleCtx.addDecl(d); // stick in overall rule's ctx
	}
}
