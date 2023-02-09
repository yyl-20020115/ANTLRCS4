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
        predefinedRulePropertiesDict.add(new Attribute("parser"));
        predefinedRulePropertiesDict.add(new Attribute("text"));
        predefinedRulePropertiesDict.add(new Attribute("start"));
        predefinedRulePropertiesDict.add(new Attribute("stop"));
        predefinedRulePropertiesDict.add(new Attribute("ctx"));
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
    : this(g, name, ast, numberOfAlts, null, false)
    {
    }

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
        if (g.isLexer())
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

    public Attribute ResolveRetvalOrProperty(String y)
    {
        if (retvals != null)
        {
            var a = retvals.get(y);
            if (a != null) return a;
        }
        var d = GetPredefinedScope(LabelType.RULE_LABEL);
        return d.get(y);
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
                    defs.Map(p.label.getText(), p);
                }
            }
        }
        return defs;
    }

    public bool HasAltSpecificContexts()
    {
        return GetAltLabels() != null;
    }

    /** Used for recursive rules (subclass), which have 1 alt, but many original alts */
    public int GetOriginalNumberOfAlts()
    {
        return numberOfAlts;
    }

    /**
	 * Get {@code #} labels. The keys of the map are the labels applied to outer
	 * alternatives of a lexer rule, and the values are collections of pairs
	 * (alternative number and {@link AltAST}) identifying the alternatives with
	 * this label. Unlabeled alternatives are not included in the result.
	 */
    public Dictionary<string, List<Pair<int, AltAST>>> GetAltLabels()
    {
        Dictionary<string, List<Pair<int, AltAST>>> labels = new();
        for (int i = 1; i <= numberOfAlts; i++)
        {
            var altLabel = alt[i].ast.altLabel;
            if (altLabel != null)
            {
                if (!labels.TryGetValue(altLabel.getText(), out var list))
                {
                    list = new();
                    labels.Add(altLabel.getText(), list);
                }

                list.Add(new Pair<int, AltAST>(i, alt[i].ast));
            }
        }
        if (labels.Count == 0) return null;
        return labels;
    }

    public List<AltAST> GetUnlabeledAltASTs()
    {
        List<AltAST> alts = new();
        for (int i = 1; i <= numberOfAlts; i++)
        {
            GrammarAST altLabel = alt[i].ast.altLabel;
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
            Attribute a = args.get(x); if (a != null) return a;
        }
        if (retvals != null)
        {
            Attribute a = retvals.get(x); if (a != null) return a;
        }
        if (locals != null)
        {
            Attribute a = locals.get(x); if (a != null) return a;
        }
        var properties = GetPredefinedScope(LabelType.RULE_LABEL);
        return properties.get(x);
    }

    /** $x.y	Attribute: x is surrounding rule, label ref (in any alts) */
    //@Override
    public Attribute ResolveToAttribute(String x, String y, ActionAST node)
    {
        var anyLabelDef = GetAnyLabelDef(x);
        if (anyLabelDef != null)
        {
            if (anyLabelDef.type == LabelType.RULE_LABEL)
            {
                return g.getRule(anyLabelDef.element.getText()).ResolveRetvalOrProperty(y);
            }
            else
            {
                var scope = GetPredefinedScope(anyLabelDef.type);
                if (scope == null)
                {
                    return null;
                }

                return scope.get(y);
            }
        }
        return null;

    }

    //@Override
    public bool ResolvesToLabel(string x, ActionAST node)
    {
        LabelElementPair anyLabelDef = GetAnyLabelDef(x);
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
        LabelElementPair anyLabelDef = GetAnyLabelDef(x);
        if (anyLabelDef != null && anyLabelDef.type == LabelType.TOKEN_LABEL) return true;
        return false;
    }

    //@Override
    public bool ResolvesToAttributeDict(string x, ActionAST node)
    {
        if (ResolvesToToken(x, node)) return true;
        return false;
    }

    public Rule ResolveToRule(String x)
    {
        if (x.Equals(this.name)) return this;
        var anyLabelDef = GetAnyLabelDef(x);
        if (anyLabelDef != null && anyLabelDef.type == LabelType.RULE_LABEL)
        {
            return g.getRule(anyLabelDef.element.getText());
        }
        return g.getRule(x);
    }

    public LabelElementPair GetAnyLabelDef(String x)
    {
        if (GetElementLabelDefs().TryGetValue(x, out var labels)) return labels[(0)];
        return null;
    }

    public AttributeDict GetPredefinedScope(LabelType ltype)
    {
        String grammarLabelKey = g.getTypeString() + ":" + ltype;
        return Grammar.grammarAndLabelRefTypeToScope.TryGetValue(grammarLabelKey, out var v) ? v : null;
    }

    public bool IsFragment
    {
        get
        {
            if (modifiers == null) return false;
            foreach (var a in modifiers)
            {
                if (a.getText().Equals("fragment")) return true;
            }
            return false;
        }
    }

    public override int GetHashCode() => name.GetHashCode();

    public override bool Equals(object? obj)
    {
        if (this == obj)
        {
            return true;
        }

        if (!(obj is Rule))
        {
            return false;
        }

        return name.Equals(((Rule)obj).name);
    }

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
