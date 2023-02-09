/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.analysis;
using org.antlr.v4.misc;
using org.antlr.v4.parse;
using org.antlr.v4.runtime.misc;
using org.antlr.v4.tool;
using org.antlr.v4.tool.ast;

namespace org.antlr.v4.semantics;

public class RuleCollector : GrammarTreeVisitor
{
    private bool grammarCaseInsensitive = false;

    /** which grammar are we checking */
    public Grammar g;
    public ErrorManager errMgr;

    // stuff to collect. this is the output
    public OrderedHashMap<string, Rule> rules = new ();
    public MultiMap<string, GrammarAST> ruleToAltLabels = new ();
    public Dictionary<string, string> altLabelToRuleName = new();

    public RuleCollector(Grammar g)
    {
        this.g = g;
        this.errMgr = g.Tools.ErrMgr;
    }

    public override ErrorManager ErrorManager => errMgr;
    public void Process(GrammarAST ast) => VisitGrammar(ast);

    public override void DiscoverRule(RuleAST rule, GrammarAST ID,
                             List<GrammarAST> modifiers, ActionAST arg,
                             ActionAST returns, GrammarAST thrws,
                             GrammarAST options, ActionAST locals,
                             List<GrammarAST> actions,
                             GrammarAST block)
    {
        int numAlts = block.ChildCount;
        Rule r;
        if (LeftRecursiveRuleAnalyzer.HasImmediateRecursiveRuleRefs(rule, ID.Text))
        {
            r = new LeftRecursiveRule(g, ID.Text, rule);
        }
        else
        {
            r = new Rule(g, ID.Text, rule, numAlts);
        }
        rules.Put(r.name, r);

        if (arg != null)
        {
            r.args = ScopeParser.ParseTypedArgList(arg, arg.Text, g);
            r.args.type = AttributeDict.DictType.ARG;
            r.args.ast = arg;
            arg.resolver = r.alt[currentOuterAltNumber];
        }

        if (returns != null)
        {
            r.retvals = ScopeParser.ParseTypedArgList(returns, returns.Text, g);
            r.retvals.type = AttributeDict.DictType.RET;
            r.retvals.ast = returns;
        }

        if (locals != null)
        {
            r.locals = ScopeParser.ParseTypedArgList(locals, locals.Text, g);
            r.locals.type = AttributeDict.DictType.LOCAL;
            r.locals.ast = locals;
        }

        foreach (var a in actions)
        {
            // a = ^(AT ID ACTION)
            var action = (ActionAST)a.GetChild(1);
            r.namedActions[a.GetChild(0).Text] = action;
            action.resolver = r;
        }
    }

    public override void DiscoverOuterAlt(AltAST alt)
    {
        if (alt.altLabel != null)
        {
            ruleToAltLabels.Map(currentRuleName, alt.altLabel);
            var altLabel = alt.altLabel.Text;
            altLabelToRuleName[Utils.Capitalize(altLabel)] = currentRuleName;
            altLabelToRuleName[misc.Utils.Decapitalize(altLabel)] = currentRuleName;
        }
    }

    public override void GrammarOption(GrammarAST ID, GrammarAST valueAST)
    {
        var caseInsensitive = GetCaseInsensitiveValue(ID, valueAST);
        if (caseInsensitive != null)
        {
            grammarCaseInsensitive = caseInsensitive.GetValueOrDefault();
        }
    }

    public override void DiscoverLexerRule(RuleAST rule, GrammarAST ID, List<GrammarAST> modifiers,
                                  GrammarAST options, GrammarAST block)
    {
        bool currentCaseInsensitive = grammarCaseInsensitive;
        if (options != null)
        {
            foreach (var child in options.GetChildren())
            {
                var childAST = (GrammarAST)child;
                var caseInsensitive = GetCaseInsensitiveValue((GrammarAST)childAST.GetChild(0), (GrammarAST)childAST.GetChild(1));
                if (caseInsensitive != null)
                {
                    currentCaseInsensitive = caseInsensitive.GetValueOrDefault();
                }
            }
        }

        int numAlts = block.ChildCount;
        var r = new Rule(g, ID.Text, rule, numAlts, currentModeName, currentCaseInsensitive);
        if (modifiers.Count > 0) r.modifiers = modifiers;
        rules.Put(r.name, r);
    }

    private static bool? GetCaseInsensitiveValue(GrammarAST optionID, GrammarAST valueAST)
    {
        var optionName = optionID.Text;
        if (optionName.Equals(Grammar.caseInsensitiveOptionName))
        {
            var valueText = valueAST.Text;
            if (valueText.Equals("true") || valueText.Equals("false"))
            {
                return bool.TryParse(valueText, out var ret1) && ret1;
            }
        }
        return null;
    }
}
