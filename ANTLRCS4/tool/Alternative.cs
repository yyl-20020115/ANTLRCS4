/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime.misc;
using org.antlr.v4.tool.ast;

namespace org.antlr.v4.tool;




/** An outermost alternative for a rule.  We don't track inner alternatives. */
public class Alternative : AttributeResolver
{
    public Rule rule;

    public AltAST ast;

    /** What alternative number is this outermost alt? 1..n */
    public int altNum;

    // token IDs, string literals in this alt
    public MultiMap<String, TerminalAST> tokenRefs = new();

    // does not include labels
    public MultiMap<String, GrammarAST> tokenRefsInActions = new();

    // all rule refs in this alt
    public MultiMap<String, GrammarAST> ruleRefs = new();

    // does not include labels
    public MultiMap<String, GrammarAST> ruleRefsInActions = new();

    /** A list of all LabelElementPair attached to tokens like id=ID, ids+=ID */
    public MultiMap<String, LabelElementPair> labelDefs = new();

    // track all token, rule, label refs in rewrite (right of ->)
    //public List<GrammarAST> rewriteElements = new ArrayList<GrammarAST>();

    /** Track all executable actions other than named actions like @init
     *  and catch/finally (not in an alt). Also tracks predicates, rewrite actions.
     *  We need to examine these actions before code generation so
     *  that we can detect refs to $rule.attr etc...
	 *
	 *  This tracks per alt
     */
    public List<ActionAST> actions = new();

    public Alternative(Rule r, int altNum) { this.rule = r; this.altNum = altNum; }

    //@Override
    public bool ResolvesToToken(String x, ActionAST node)
    {
        if (tokenRefs.ContainsKey(x)) return true;
        var anyLabelDef = getAnyLabelDef(x);
        if (anyLabelDef != null && anyLabelDef.type == LabelType.TOKEN_LABEL) return true;
        return false;
    }

    //@Override
    public bool ResolvesToAttributeDict(String x, ActionAST node)
    {
        if (ResolvesToToken(x, node)) return true;
        if (ruleRefs.ContainsKey(x)) return true; // rule ref in this alt?
        var anyLabelDef = getAnyLabelDef(x);
        if (anyLabelDef != null && anyLabelDef.type == LabelType.RULE_LABEL) return true;
        return false;
    }

    /**  $x		Attribute: rule arguments, return values, predefined rule prop.
	 */
    //@Override
    public Attribute ResolveToAttribute(String x, ActionAST node)
    {
        return rule.ResolveToAttribute(x, node); // reuse that code
    }

    /** $x.y, x can be surrounding rule, token/rule/label ref. y is visible
	 *  attr in that dictionary.  Can't see args on rule refs.
	 */
    //@Override
    public Attribute ResolveToAttribute(String x, String y, ActionAST node)
    {
        if (tokenRefs.ContainsKey(x))
        { // token ref in this alt?
            return rule.GetPredefinedScope(LabelType.TOKEN_LABEL).Get(y);
        }
        if (ruleRefs.ContainsKey(x))
        {  // rule ref in this alt?
           // look up rule, ask it to resolve y (must be retval or predefined)
            return rule.g.GetRule(x).ResolveRetvalOrProperty(y);
        }
        LabelElementPair anyLabelDef = getAnyLabelDef(x);
        if (anyLabelDef != null && anyLabelDef.type == LabelType.RULE_LABEL)
        {
            return rule.g.GetRule(anyLabelDef.element.getText()).ResolveRetvalOrProperty(y);
        }
        else if (anyLabelDef != null)
        {
            AttributeDict scope = rule.GetPredefinedScope(anyLabelDef.type);
            if (scope == null)
            {
                return null;
            }

            return scope.Get(y);
        }
        return null;
    }

    //@Override
    public bool ResolvesToLabel(String x, ActionAST node)
    {
        LabelElementPair anyLabelDef = getAnyLabelDef(x);
        return anyLabelDef != null &&
               (anyLabelDef.type == LabelType.TOKEN_LABEL ||
                anyLabelDef.type == LabelType.RULE_LABEL);
    }

    //@Override
    public bool ResolvesToListLabel(String x, ActionAST node)
    {
        LabelElementPair anyLabelDef = getAnyLabelDef(x);
        return anyLabelDef != null &&
               (anyLabelDef.type == LabelType.RULE_LIST_LABEL ||
                anyLabelDef.type == LabelType.TOKEN_LIST_LABEL);
    }

    public LabelElementPair getAnyLabelDef(String x)
    {
        if (labelDefs.TryGetValue(x, out var labels)) return labels[0];
        return null;
    }

    /** x can be ruleref or rule label. */
    public Rule resolveToRule(String x)
    {
        if (ruleRefs.ContainsKey(x)) return rule.g.GetRule(x);
        LabelElementPair anyLabelDef = getAnyLabelDef(x);
        if (anyLabelDef != null && anyLabelDef.type == LabelType.RULE_LABEL)
        {
            return rule.g.GetRule(anyLabelDef.element.getText());
        }
        return null;
    }
}
