/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime.misc;
using org.antlr.v4.tool.ast;
using System.Text;

namespace org.antlr.v4.tool;


public class Rule : AttributeResolver {
	/** Rule refs have a predefined set of attributes as well as
     *  the return values and args.
     *
     *  These must be consistent with ActionTranslator.rulePropToModelMap, ...
     */
	public static readonly AttributeDict predefinedRulePropertiesDict =
		new AttributeDict(AttributeDict.DictType.PREDEFINED_RULE);
	static Rule() {
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

    public static readonly HashSet<String> validLexerCommands = new HashSet<String>();

	public readonly String name;
	public List<GrammarAST> modifiers;

	public RuleAST ast;
	public AttributeDict args;
	public AttributeDict retvals;
	public AttributeDict locals;

	/** In which grammar does this rule live? */
	public readonly Grammar g;

	/** If we're in a lexer grammar, we might be in a mode */
	public readonly String mode;

	/** If null then use value from global option that is false by default */
	public readonly bool caseInsensitive;

    /** Map a name to an action for this rule like @init {...}.
     *  The code generator will use this to fill holes in the rule template.
     *  I track the AST node for the action in case I need the line number
     *  for errors.
     */
    public Dictionary<String, ActionAST> namedActions =
        new ();

    /** Track exception handlers; points at "catch" node of (catch exception action)
	 *  don't track finally action
	 */
    public List<GrammarAST> exceptions = new ();

	/** Track all executable actions other than named actions like @init
	 *  and catch/finally (not in an alt). Also tracks predicates, rewrite actions.
	 *  We need to examine these actions before code generation so
	 *  that we can detect refs to $rule.attr etc...
	 *
	 *  This tracks per rule; Alternative objs also track per alt.
	 */
	public List<ActionAST> actions = new ();

	public ActionAST finallyAction;

	public readonly int numberOfAlts;

	public bool isStartRule = true; // nobody calls us

	/** 1..n alts */
	public Alternative[] alt;

	/** All rules have unique index 0..n-1 */
	public int index;

	public int actionIndex = -1; // if lexer; 0..n-1 for n actions in a rule

	public Rule(Grammar g, String name, RuleAST ast, int numberOfAlts)
	: this(g, name, ast, numberOfAlts, null, false)
    {
	}

	public Rule(Grammar g, String name, RuleAST ast, int numberOfAlts, String lexerMode, bool caseInsensitive) {
		this.g = g;
		this.name = name;
		this.ast = ast;
		this.numberOfAlts = numberOfAlts;
		alt = new Alternative[numberOfAlts+1]; // 1..n
		for (int i=1; i<=numberOfAlts; i++) alt[i] = new Alternative(this, i);
		this.mode = lexerMode;
		this.caseInsensitive = caseInsensitive;
	}

	public void defineActionInAlt(int currentAlt, ActionAST actionAST) {
		actions.Add(actionAST);
		alt[currentAlt].actions.Add(actionAST);
		if ( g.isLexer() ) {
			defineLexerAction(actionAST);
		}
	}

	/** Lexer actions are numbered across rules 0..n-1 */
	public void defineLexerAction(ActionAST actionAST) {
		actionIndex = g.lexerActions.Count;
		if ( g.lexerActions.get(actionAST)==null ) {
			g.lexerActions.put(actionAST, actionIndex);
		}
	}

	public void definePredicateInAlt(int currentAlt, PredAST predAST) {
		actions.Add(predAST);
		alt[currentAlt].actions.Add(predAST);
		if ( g.sempreds.get(predAST)==null ) {
			g.sempreds.put(predAST, g.sempreds.size());
		}
	}

	public Attribute resolveRetvalOrProperty(String y) {
		if ( retvals!=null ) {
			Attribute a = retvals.get(y);
			if ( a!=null ) return a;
		}
		AttributeDict d = getPredefinedScope(LabelType.RULE_LABEL);
		return d.get(y);
	}

	public HashSet<String> getTokenRefs() {
        HashSet<String> refs = new HashSet<String>();
		for (int i=1; i<=numberOfAlts; i++) {
			refs.addAll(alt[i].tokenRefs.keySet());
		}
		return refs;
    }

    public HashSet<String> getElementLabelNames() {
        HashSet<String> refs = new HashSet<String>();
        for (int i=1; i<=numberOfAlts; i++) {
            refs.addAll(alt[i].labelDefs.keySet());
        }
		if ( refs.Count == 0 ) return null;
        return refs;
    }

    public MultiMap<String, LabelElementPair> getElementLabelDefs() {
        MultiMap<String, LabelElementPair> defs =
            new MultiMap<String, LabelElementPair>();
        for (int i=1; i<=numberOfAlts; i++) {
            foreach (List<LabelElementPair> pairs in alt[i].labelDefs.Values) {
                foreach (LabelElementPair p in pairs) {
                    defs.map(p.label.getText(), p);
                }
            }
        }
        return defs;
    }

	public bool hasAltSpecificContexts() {
		return getAltLabels()!=null;
	}

	/** Used for recursive rules (subclass), which have 1 alt, but many original alts */
	public int getOriginalNumberOfAlts() {
		return numberOfAlts;
	}

	/**
	 * Get {@code #} labels. The keys of the map are the labels applied to outer
	 * alternatives of a lexer rule, and the values are collections of pairs
	 * (alternative number and {@link AltAST}) identifying the alternatives with
	 * this label. Unlabeled alternatives are not included in the result.
	 */
	public Dictionary<String, List<Pair<int, AltAST>>> getAltLabels() {
        Dictionary<String, List<Pair<int, AltAST>>> labels = new ();
		for (int i=1; i<=numberOfAlts; i++) {
			GrammarAST altLabel = alt[i].ast.altLabel;
			if ( altLabel!=null ) {
				List<Pair<int, AltAST>> list = labels.get(altLabel.getText());
				if (list == null) {
					list = new ();
					labels.put(altLabel.getText(), list);
				}

				list.Add(new Pair<int, AltAST>(i, alt[i].ast));
			}
		}
		if ( labels.Count == 0 ) return null;
		return labels;
	}

	public List<AltAST> getUnlabeledAltASTs() {
		List<AltAST> alts = new();
		for (int i=1; i<=numberOfAlts; i++) {
			GrammarAST altLabel = alt[i].ast.altLabel;
			if ( altLabel==null ) alts.add(alt[i].ast);
		}
		if ( alts.Count == 0 ) return null;
		return alts;
	}

	/**  $x		Attribute: rule arguments, return values, predefined rule prop.
	 */
	//@Override
	public Attribute resolveToAttribute(String x, ActionAST node) {
		if ( args!=null ) {
			Attribute a = args.get(x);   	if ( a!=null ) return a;
		}
		if ( retvals!=null ) {
			Attribute a = retvals.get(x);	if ( a!=null ) return a;
		}
		if ( locals!=null ) {
			Attribute a = locals.get(x);	if ( a!=null ) return a;
		}
		AttributeDict properties = getPredefinedScope(LabelType.RULE_LABEL);
		return properties.get(x);
	}

    /** $x.y	Attribute: x is surrounding rule, label ref (in any alts) */
    //@Override
    public Attribute resolveToAttribute(String x, String y, ActionAST node) {
		LabelElementPair anyLabelDef = getAnyLabelDef(x);
		if ( anyLabelDef!=null ) {
			if ( anyLabelDef.type==LabelType.RULE_LABEL ) {
				return g.getRule(anyLabelDef.element.getText()).resolveRetvalOrProperty(y);
			}
			else {
				AttributeDict scope = getPredefinedScope(anyLabelDef.type);
				if (scope == null) {
					return null;
				}

				return scope.get(y);
			}
		}
		return null;

	}

    //@Override
    public bool resolvesToLabel(String x, ActionAST node) {
		LabelElementPair anyLabelDef = getAnyLabelDef(x);
		return anyLabelDef!=null &&
			   (anyLabelDef.type==LabelType.RULE_LABEL ||
				anyLabelDef.type==LabelType.TOKEN_LABEL);
	}

    //@Override
    public bool resolvesToListLabel(String x, ActionAST node) {
		LabelElementPair anyLabelDef = getAnyLabelDef(x);
		return anyLabelDef!=null &&
			   (anyLabelDef.type==LabelType.RULE_LIST_LABEL ||
				anyLabelDef.type==LabelType.TOKEN_LIST_LABEL);
	}

    //@Override
    public bool resolvesToToken(String x, ActionAST node) {
		LabelElementPair anyLabelDef = getAnyLabelDef(x);
		if ( anyLabelDef!=null && anyLabelDef.type==LabelType.TOKEN_LABEL ) return true;
		return false;
	}

    //@Override
    public bool resolvesToAttributeDict(String x, ActionAST node) {
		if ( resolvesToToken(x, node) ) return true;
		return false;
	}

	public Rule resolveToRule(String x) {
		if ( x.Equals(this.name) ) return this;
		LabelElementPair anyLabelDef = getAnyLabelDef(x);
		if ( anyLabelDef!=null && anyLabelDef.type==LabelType.RULE_LABEL ) {
			return g.getRule(anyLabelDef.element.getText());
		}
		return g.getRule(x);
	}

	public LabelElementPair getAnyLabelDef(String x) {
		List<LabelElementPair> labels = getElementLabelDefs().get(x);
		if ( labels!=null ) return labels.get(0);
		return null;
	}

    public AttributeDict getPredefinedScope(LabelType ltype) {
        String grammarLabelKey = g.getTypeString() + ":" + ltype;
        return Grammar.grammarAndLabelRefTypeToScope.get(grammarLabelKey);
    }

	public bool isFragment() {
		if ( modifiers==null ) return false;
		foreach (GrammarAST a in modifiers) {
			if ( a.getText().Equals("fragment") ) return true;
		}
		return false;
	}

    //@Override
    public override int GetHashCode() { return name.GetHashCode(); }

    //@Override
    public override bool Equals(Object? obj) {
		if (this == obj) {
			return true;
		}

		if (!(obj is Rule)) {
			return false;
		}

		return name.Equals(((Rule)obj).name);
	}

    //@Override
    public String ToString() {
		StringBuilder buf = new StringBuilder();
		buf.Append("Rule{name=").Append(name);
		if ( args!=null ) buf.Append(", args=").Append(args);
		if ( retvals!=null ) buf.Append(", retvals=").Append(retvals);
		buf.Append('}');
		return buf.ToString();
    }
}
