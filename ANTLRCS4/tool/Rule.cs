/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime.misc;
using org.antlr.v4.tool.ast;
using System.Text;

namespace org.antlr.v4.tool;


public class Rule : AttributeResolver
{
    /** Rule refs have a predefined set of attributes as well as
     *  the return values and args.
     *
     *  These must be consistent with ActionTranslator.rulePropToModelMap, ...
     */
    public static readonly AttributeDict predefinedRulePropertiesDict =
        new (AttributeDict.DictType.PREDEFINED_RULE);
    static Rule()
    {
        predefinedRulePropertiesDict.Add(new ("parser"));
        predefinedRulePropertiesDict.Add(new ("text"));
        predefinedRulePropertiesDict.Add(new ("start"));
        predefinedRulePropertiesDict.Add(new ("stop"));
        predefinedRulePropertiesDict.Add(new ("ctx"));
        // CALLS
        validLexerCommands.Add("mode");
        validLexerCommands.Add("pushMode");
        validLexerCommands.Add("type");
        validLexerCommands.Add("channel");

        // ACTIONS
        validLexerCommands.Add("popMode");
        validLexerCommands.Add("skip");
        validLexerCommands.Add("more");

    }

    public static readonly HashSet<string> validLexerCommands = new ();

    public readonly string name;
    public List<GrammarAST> modifiers;

    public RuleAST ast;
    public AttributeDict args;
    public AttributeDict retvals;
    public AttributeDict locals;

    /** In which grammar does this rule live? */
    public readonly Grammar g;

    /** If we're in a lexer grammar, we might be in a mode */
    public readonly string mode;

    /** If null then use value from global option that is false by default */
    public readonly bool caseInsensitive;

    /** Map a name to an action for this rule like @init {...}.
     *  The code generator will use this to fill holes in the rule template.
     *  I track the AST node for the action in case I need the line number
     *  for errors.
     */
    public Dictionary<string, ActionAST> namedActions = new();

    /** Track exception handlers; points at "catch" node of (catch exception action)
	 *  don't track finally action
	 */
    public List<GrammarAST> exceptions = new();

    /** Track all executable actions other than named actions like @init
	 *  and catch/finally (not in an alt). Also tracks predicates, rewrite actions.
	 *  We need to examine these actions before code generation so
	 *  that we can detect refs to $rule.attr etc...
	 *
	 *  This tracks per rule; Alternative objs also track per alt.
	 */
    public List<ActionAST> actions = new();

    public ActionAST finallyAction;

    public readonly int numberOfAlts;

    public bool isStartRule = true; // nobody calls us

    /** 1..n alts */
    public Alternative[] alt;

    /** All rules have unique index 0..n-1 */
    public int index;

    public int actionIndex = -1; // if lexer; 0..n-1 for n actions in a rule

    public Rule(Grammar g, string name, RuleAST ast, int numberOfAlts)
    : this(g, name, ast, numberOfAlts, null, false) { }

    public Rule(Grammar g, string name, RuleAST ast, int numberOfAlts, string lexerMode, bool caseInsensitive)
    {
        this.g = g;
        this.name = name;
        this.ast = ast;
        this.numberOfAlts = numberOfAlts;
        alt = new Alternative[numberOfAlts + 1]; // 1..n
        for (int i = 1; i <= numberOfAlts; i++) alt[i] = new Alternative(this, i);
        this.mode = lexerMode;
        this.caseInsensitive = caseInsensitive;
    }

    public void DefineActionInAlt(int currentAlt, ActionAST actionAST)
    {
        actions.Add(actionAST);
        alt[currentAlt].actions.Add(actionAST);
        if (g.IsLexer)
        {
            DefineLexerAction(actionAST);
        }
    }

    /** Lexer actions are numbered across rules 0..n-1 */
    public void DefineLexerAction(ActionAST actionAST)
    {
        actionIndex = g.lexerActions.Count;
        if (!g.lexerActions.ContainsKey(actionAST))
        {
            g.lexerActions.Add(actionAST, actionIndex);
        }
    }

    public void DefinePredicateInAlt(int currentAlt, PredAST predAST)
    {
        actions.Add(predAST);
        alt[currentAlt].actions.Add(predAST);
        if (!g.sempreds.ContainsKey(predAST))
        {
            g.sempreds[predAST] = g.sempreds.Count;
        }
    }

    public Attribute ResolveRetvalOrProperty(string y)
    {
        if (retvals != null)
        {
            var a = retvals.Get(y);
            if (a != null) return a;
        }
        var d = GetPredefinedScope(LabelType.RULE_LABEL);
        return d.Get(y);
    }

    public HashSet<string> GetTokenRefs()
    {
        HashSet<string> refs = new ();
        for (int i = 1; i <= numberOfAlts; i++)
        {
            refs.UnionWith(alt[i].tokenRefs.Keys);
        }
        return refs;
    }

    public HashSet<string> GetElementLabelNames()
    {
        HashSet<string> refs = new();
        for (int i = 1; i <= numberOfAlts; i++)
        {
            refs.UnionWith(alt[i].labelDefs.Keys);
        }
        if (refs.Count == 0) return null;
        return refs;
    }

    public MultiMap<string, LabelElementPair> GetElementLabelDefs()
    {
        MultiMap<string, LabelElementPair> defs =
            new();
        for (int i = 1; i <= numberOfAlts; i++)
        {
            foreach (var pairs in alt[i].labelDefs.Values)
            {
                foreach (var p in pairs)
                {
                    defs.Map(p.label.Text, p);
                }
            }
        }
        return defs;
    }

    public virtual bool HasAltSpecificContexts() => GetAltLabels() != null;

    /** Used for recursive rules (subclass), which have 1 alt, but many original alts */
    public virtual int OriginalNumberOfAlts => numberOfAlts;

    /**
	 * Get {@code #} labels. The keys of the map are the labels applied to outer
	 * alternatives of a lexer rule, and the values are collections of pairs
	 * (alternative number and {@link AltAST}) identifying the alternatives with
	 * this label. Unlabeled alternatives are not included in the result.
	 */
    public virtual Dictionary<string, List<Pair<int, AltAST>>> GetAltLabels()
    {
        Dictionary<string, List<Pair<int, AltAST>>> labels = new();
        for (int i = 1; i <= numberOfAlts; i++)
        {
            var altLabel = alt[i].ast.altLabel;
            if (altLabel != null)
            {
                if (!labels.TryGetValue(altLabel.Text, out var list))
                {
                    list = new();
                    labels.Add(altLabel.Text, list);
                }

                list.Add(new Pair<int, AltAST>(i, alt[i].ast));
            }
        }
        if (labels.Count == 0) return null;
        return labels;
    }

    public virtual List<AltAST> GetUnlabeledAltASTs()
    {
        List<AltAST> alts = new();
        for (int i = 1; i <= numberOfAlts; i++)
        {
            var altLabel = alt[i].ast.altLabel;
            if (altLabel == null) alts.Add(alt[i].ast);
        }
        if (alts.Count == 0) return null;
        return alts;
    }

    /**  $x		Attribute: rule arguments, return values, predefined rule prop.
	 */
    //@Override
    public Attribute ResolveToAttribute(string x, ActionAST node)
    {
        if (args != null)
        {
            Attribute a = args.Get(x); if (a != null) return a;
        }
        if (retvals != null)
        {
            Attribute a = retvals.Get(x); if (a != null) return a;
        }
        if (locals != null)
        {
            Attribute a = locals.Get(x); if (a != null) return a;
        }
        var properties = GetPredefinedScope(LabelType.RULE_LABEL);
        return properties.Get(x);
    }

    /** $x.y	Attribute: x is surrounding rule, label ref (in any alts) */
    //@Override
    public Attribute ResolveToAttribute(string x, string y, ActionAST node)
    {
        var anyLabelDef = GetAnyLabelDef(x);
        if (anyLabelDef != null)
        {
            if (anyLabelDef.type == LabelType.RULE_LABEL)
            {
                return g.GetRule(anyLabelDef.element.Text).ResolveRetvalOrProperty(y);
            }
            else
            {
                var scope = GetPredefinedScope(anyLabelDef.type);
                if (scope == null)
                {
                    return null;
                }

                return scope.Get(y);
            }
        }
        return null;

    }

    //@Override
    public bool ResolvesToLabel(string x, ActionAST node)
    {
        var anyLabelDef = GetAnyLabelDef(x);
        return anyLabelDef != null &&
               (anyLabelDef.type == LabelType.RULE_LABEL ||
                anyLabelDef.type == LabelType.TOKEN_LABEL);
    }

    //@Override
    public bool ResolvesToListLabel(string x, ActionAST node)
    {
        var anyLabelDef = GetAnyLabelDef(x);
        return anyLabelDef != null &&
               (anyLabelDef.type == LabelType.RULE_LIST_LABEL ||
                anyLabelDef.type == LabelType.TOKEN_LIST_LABEL);
    }

    //@Override
    public bool ResolvesToToken(string x, ActionAST node)
    {
        var anyLabelDef = GetAnyLabelDef(x);
        if (anyLabelDef != null && anyLabelDef.type == LabelType.TOKEN_LABEL) return true;
        return false;
    }

    //@Override
    public bool ResolvesToAttributeDict(string x, ActionAST node) => ResolvesToToken(x, node);

    public Rule ResolveToRule(string x)
    {
        if (x.Equals(this.name)) return this;
        var anyLabelDef = GetAnyLabelDef(x);
        return anyLabelDef != null && anyLabelDef.type == LabelType.RULE_LABEL ? g.GetRule(anyLabelDef.element.Text) : g.GetRule(x);
    }

    public LabelElementPair GetAnyLabelDef(string x)
    {
        return GetElementLabelDefs().TryGetValue(x, out var labels) ? labels[(0)] : null;
    }

    public AttributeDict GetPredefinedScope(LabelType ltype)
    {
        var grammarLabelKey = g.GetTypeString() + ":" + ltype;
        return Grammar.grammarAndLabelRefTypeToScope.TryGetValue(grammarLabelKey, out var v) ? v : null;
    }

    public bool IsFragment
    {
        get
        {
            if (modifiers == null) return false;
            foreach (var a in modifiers)
            {
                if (a.Text.Equals("fragment")) return true;
            }
            return false;
        }
    }

    public override int GetHashCode() => name.GetHashCode();

    public override bool Equals(object? o) => this == o || o is Rule rule && name.Equals(rule.name);

    public override string ToString()
    {
        var buffer = new StringBuilder();
        buffer.Append("Rule{name=").Append(name);
        if (args != null) buffer.Append(", args=").Append(args);
        if (retvals != null) buffer.Append(", retvals=").Append(retvals);
        buffer.Append('}');
        return buffer.ToString();
    }
}
