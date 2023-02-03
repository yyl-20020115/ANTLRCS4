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
public class RuleFunction : OutputModelObject
{
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

    [ModelElement]
    public List<SrcOp> code;
    [ModelElement]
    public OrderedHashSet<Decl> locals; // TODO: move into ctx?
    [ModelElement]
    public ICollection<AttributeDecl> args = null;
    [ModelElement]
    public StructDecl ruleCtx;
    [ModelElement]
    public Dictionary<String, AltLabelStructDecl> altLabelCtxs;
    [ModelElement]
    public Dictionary<String, Action> namedActions;
    [ModelElement]
    public Action finallyAction;
    [ModelElement]
    public List<ExceptionClause> exceptions;
    [ModelElement]
    public List<SrcOp> postamble;

    public RuleFunction(OutputModelFactory factory, Rule r) : base(factory)
    {
        this.name = r.name;
        this.escapedName = factory.GetGenerator().Target.EscapeIfNeeded(r.name);
        this.rule = r;
        modifiers = Utils.nodesToStrings(r.modifiers);

        index = r.index;

        ruleCtx = new StructDecl(factory, r);
        altToContext = new AltLabelStructDecl[r.getOriginalNumberOfAlts() + 1];
        AddContextGetters(factory, r);

        if (r.args != null)
        {
            var decls = r.args.attributes.Values;
            if (decls.Count > 0)
            {
                args = new List<AttributeDecl>();
                ruleCtx.AddDecls(decls);
                foreach (var a in decls)
                {
                    args.Add(new AttributeDecl(factory, a));
                }
                ruleCtx.ctorAttrs = args;
            }
        }
        if (r.retvals != null)
        {
            ruleCtx.AddDecls(r.retvals.attributes.Values);
        }
        if (r.locals != null)
        {
            ruleCtx.AddDecls(r.locals.attributes.Values);
        }

        ruleLabels = r.getElementLabelNames();
        tokenLabels = r.getTokenRefs();
        if (r.exceptions != null)
        {
            exceptions = new();
            foreach (var e in r.exceptions)
            {
                var catchArg = (ActionAST)e.getChild(0);
                var catchAction = (ActionAST)e.getChild(1);
                exceptions.Add(new ExceptionClause(factory, catchArg, catchAction));
            }
        }

        startState = factory.GetGrammar().atn.ruleToStartState[r.index];
    }

    public void AddContextGetters(OutputModelFactory factory, Rule r)
    {
        // Add ctx labels for elements in alts with no -> label
        var altsNoLabels = r.getUnlabeledAltASTs();
        if (altsNoLabels != null)
        {
            var decls = GetDeclsForAllElements(altsNoLabels);
            // we know to put in rule ctx, so do it directly
            foreach (var d in decls) ruleCtx.AddDecl(d);
        }

        // make structs for -> labeled alts, define ctx labels for elements
        altLabelCtxs = new();
        var labels = r.getAltLabels();
        if (labels != null)
        {
            foreach (var entry in labels)
            {
                var label = entry.Key;
                List<AltAST> alts = new();
                foreach (var pair in entry.Value)
                {
                    alts.Add(pair.b);
                }

                var decls = GetDeclsForAllElements(alts);
                foreach (var pair in entry.Value)
                {
                    int altNum = pair.a;
                    altToContext[altNum] = new (factory, r, altNum, label);
                    if (!altLabelCtxs.ContainsKey(label))
                    {
                        altLabelCtxs[label] = altToContext[altNum];
                    }

                    // we know which ctx to put in, so do it directly
                    foreach (Decl d in decls)
                    {
                        altToContext[altNum].AddDecl(d);
                    }
                }
            }
        }
    }

    public void FillNamedActions(OutputModelFactory factory, Rule r)
    {
        if (r.finallyAction != null)
            finallyAction = new Action(factory, r.finallyAction);

        namedActions = new();
        foreach (var name in r.namedActions.Keys)
        {
            var ast = r.namedActions[(name)];
            namedActions[name] = new Action(factory, ast);
        }
    }

    /** for all alts, find which ref X or r needs List
	   Must see across alts. If any alt needs X or r as list, then
	   define as list.
	 */
    public HashSet<Decl> GetDeclsForAllElements(List<AltAST> altASTs)
    {
        HashSet<String> needsList = new ();
        HashSet<String> nonOptional = new ();
        List<GrammarAST> allRefs = new();
        bool firstAlt = true;
        var reftypes = new IntervalSet(ANTLRParser.RULE_REF, ANTLRParser.TOKEN_REF, ANTLRParser.STRING_LITERAL);
        foreach (var ast in altASTs)
        {
            var refs = GetRuleTokens(ast.getNodesWithType(reftypes));
            allRefs.AddRange(refs);
            var minAndAltFreq = getElementFrequenciesForAlt(ast);
            var minFreq = minAndAltFreq.a;
            var altFreq = minAndAltFreq.b;
            foreach (var t in refs)
            {
                var refLabelName = GetName(t);

                if (refLabelName != null)
                {
                    if (altFreq.count(refLabelName) > 1)
                    {
                        needsList.Add(refLabelName);
                    }

                    if (firstAlt && minFreq.count(refLabelName) != 0)
                    {
                        nonOptional.Add(refLabelName);
                    }
                }
            }

            foreach (var @ref in nonOptional.ToArray())
            {
                if (minFreq.count(@ref) == 0)
                {
                    nonOptional.Remove(@ref);
                }
            }

            firstAlt = false;
        }
        HashSet<Decl> decls = new();
        foreach (var t in allRefs)
        {
            var refLabelName = GetName(t);

            if (refLabelName == null)
                continue;

            List<Decl> d = GetDeclForAltElement(t,
                                                refLabelName,
                                                needsList.Contains(refLabelName),
                                                !nonOptional.Contains(refLabelName));
            decls.UnionWith(d);
        }
        return decls;
    }

    private List<GrammarAST> GetRuleTokens(List<GrammarAST> refs)
    {
        List<GrammarAST> result = new(refs.Count);
        foreach (var @ref in refs)
        {
            CommonTree r = @ref;

            bool ignore = false;
            while (r != null)
            {
                // Ignore string literals in predicates
                if (r is PredAST)
                {
                    ignore = true;
                    break;
                }
                r = r.parent;
            }

            if (!ignore)
            {
                result.Add(@ref);
            }
        }

        return result;
    }

    private string GetName(GrammarAST token)
    {
        var tokenText = token.getText();
        var tokenName = token.getType() != ANTLRParser.STRING_LITERAL ? tokenText : token.g.getTokenName(tokenText);
        return tokenName == null || tokenName.StartsWith("T__") ? null : tokenName; // Do not include tokens with auto generated names
    }

    /** Given list of X and r refs in alt, compute how many of each there are */
    protected Pair<FrequencySet<string>, FrequencySet<string>> getElementFrequenciesForAlt(AltAST ast)
    {
        try
        {
            var visitor = new ElementFrequenciesVisitor(new CommonTreeNodeStream(new GrammarASTAdaptor(), ast));
            visitor.outerAlternative();
            if (visitor.frequencies.size() != 1)
            {
                factory.GetGrammar().Tools.ErrMgr.toolError(ErrorType.INTERNAL_ERROR);
                return new(new FrequencySet<string>(), new FrequencySet<string>());
            }

            return new(visitor.GetMinFrequencies(), visitor.frequencies.peek());
        }
        catch (RecognitionException ex)
        {
            factory.GetGrammar().Tools.ErrMgr.toolError(ErrorType.INTERNAL_ERROR, ex);
            return new(new (), new ());
        }
    }

    public List<Decl> GetDeclForAltElement(GrammarAST t, String refLabelName, bool needList, bool optional)
    {
        List<Decl> decls = new();
        if (t.getType() == ANTLRParser.RULE_REF)
        {
            var rref = factory.GetGrammar().getRule(t.getText());
            var ctxName = factory.GetGenerator().Target
                             .GetRuleFunctionContextStructName(rref);
            if (needList)
            {
                if (factory.GetGenerator().Target.SupportsOverloadedMethods())
                    decls.Add(new (factory, refLabelName, ctxName));
                decls.Add(new (factory, refLabelName, ctxName));
            }
            else
            {
                decls.Add(new ContextRuleGetterDecl(factory, refLabelName, ctxName, optional));
            }
        }
        else
        {
            if (needList)
            {
                if (factory.GetGenerator().Target.SupportsOverloadedMethods())
                    decls.Add(new (factory, refLabelName));
                decls.Add(new ContextTokenListIndexedGetterDecl(factory, refLabelName));
            }
            else
            {
                decls.Add(new ContextTokenGetterDecl(factory, refLabelName, optional));
            }
        }
        return decls;
    }

    /** Add local var decl */
    public void AddLocalDecl(Decl d)
    {
        locals ??= new ();
        locals.Add(d);
        d.isLocal = true;
    }

    /** Add decl to struct ctx for rule or alt if labeled */
    public void AddContextDecl(string altLabel, Decl d)
    {
        var alt = d.GetOuterMostAltCodeBlock();
        // if we found code blk and might be alt label, try to Add to that label ctx
        if (alt != null && altLabelCtxs != null)
        {
            //			Console.Out.WriteLine(d.name+" lives in alt "+alt.alt.altNum);
            if (altLabelCtxs.TryGetValue(altLabel, out var altCtx))
            { // we have an alt ctx
              //				Console.Out.WriteLine("ctx is "+ altCtx.name);
                altCtx.AddDecl(d);
                return;
            }
        }
        ruleCtx.AddDecl(d); // stick in overall rule's ctx
    }
}
