/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.runtime;
using org.antlr.v4.codegen.model;
using org.antlr.v4.codegen.model.chunk;
using org.antlr.v4.codegen.model.decl;
using org.antlr.v4.parse;
using org.antlr.v4.runtime;
using org.antlr.v4.tool;
using org.antlr.v4.tool.ast;
using System.Text;

namespace org.antlr.v4.codegen;

/** */
public class ActionTranslator : ActionSplitterListener
{
    public static readonly Dictionary<string, Type> thisRulePropToModelMap = new();
    static ActionTranslator()
    {
        thisRulePropToModelMap.Add("start", typeof(ThisRulePropertyRef_start));
        thisRulePropToModelMap.Add("stop", typeof(ThisRulePropertyRef_stop));
        thisRulePropToModelMap.Add("text", typeof(ThisRulePropertyRef_text));
        thisRulePropToModelMap.Add("ctx", typeof(ThisRulePropertyRef_ctx));
        thisRulePropToModelMap.Add("parser", typeof(ThisRulePropertyRef_parser));
        rulePropToModelMap.Add("start", typeof(RulePropertyRef_start));
        rulePropToModelMap.Add("stop", typeof(RulePropertyRef_stop));
        rulePropToModelMap.Add("text", typeof(RulePropertyRef_text));
        rulePropToModelMap.Add("ctx", typeof(RulePropertyRef_ctx));
        rulePropToModelMap.Add("parser", typeof(RulePropertyRef_parser));
        tokenPropToModelMap.Add("text", typeof(TokenPropertyRef_text));
        tokenPropToModelMap.Add("type", typeof(TokenPropertyRef_type));
        tokenPropToModelMap.Add("line", typeof(TokenPropertyRef_line));
        tokenPropToModelMap.Add("index", typeof(TokenPropertyRef_index));
        tokenPropToModelMap.Add("pos", typeof(TokenPropertyRef_pos));
        tokenPropToModelMap.Add("channel", typeof(TokenPropertyRef_channel));
        tokenPropToModelMap.Add("int", typeof(TokenPropertyRef_int));
    }

    public static readonly Dictionary<string, Type> rulePropToModelMap =
        new();

    public static readonly Dictionary<string, Type> tokenPropToModelMap =
        new();

    readonly CodeGenerator gen;
    readonly Target target;
    readonly ActionAST node;
    RuleFunction rf;
    readonly List<ActionChunk> chunks = new();
    readonly OutputModelFactory factory;
    StructDecl nodeContext;

    public ActionTranslator(OutputModelFactory factory, ActionAST node)
    {
        this.factory = factory;
        this.node = node;
        this.gen = factory.GetGenerator();
        this.target = gen.Target;
    }

    public static string ToString(List<ActionChunk> chunks)
    {
        var buffer = new StringBuilder();
        foreach (var c in chunks) buffer.Append(c.ToString());
        return buffer.ToString();
    }

    public static List<ActionChunk> TranslateAction(OutputModelFactory factory,
                                                    RuleFunction rf,
                                                    Token tokenWithinAction,
                                                    ActionAST node)
    {
        var action = tokenWithinAction.getText();
        if (action != null && action.Length > 0 && action[(0)] == '{')
        {
            int firstCurly = action.IndexOf('{');
            int lastCurly = action.LastIndexOf('}');
            if (firstCurly >= 0 && lastCurly >= 0)
            {
                action = action[(firstCurly + 1)..lastCurly]; // trim {...}
            }
        }
        return TranslateActionChunk(factory, rf, action, node);
    }

    public static List<ActionChunk> TranslateActionChunk(OutputModelFactory factory,
                                                         RuleFunction rf,
                                                         String action,
                                                         ActionAST node)
    {
        var tokenWithinAction = node.token;
        var translator = new ActionTranslator(factory, node)
        {
            rf = rf
        };
        factory.GetGrammar().Tools.Log("action-translator", "translate " + action);
        var altLabel = node.getAltLabel();
        if (rf != null)
        {
            translator.nodeContext = rf.ruleCtx;
            if (altLabel != null)
            {
                translator.nodeContext = rf.altLabelCtxs.TryGetValue(altLabel, out var r) ? r : null;
            }
        }
        var @in = new ANTLRStringStream(action);
        @in.setLine(tokenWithinAction.getLine());
        @in.setCharPositionInLine(tokenWithinAction.getCharPositionInLine());
        var trigger = new ActionSplitter(@in, translator);
        // forces eval, triggers listener methods
        trigger.GetActionTokens();
        return translator.chunks;
    }

    //@Override
    public virtual void Attr(string expr, Token x)
    {
        gen.g.Tools.Log("action-translator", "attr " + x);
        var a = node.resolver.resolveToAttribute(x.getText(), node);
        var name = x.getText();
        var escapedName = target.EscapeIfNeeded(name);
        if (a != null)
        {
            switch (a.dict.type)
            {
                case AttributeDict.DictType.ARG:
                    chunks.Add(new ArgRef(nodeContext, name, escapedName));
                    break;
                case AttributeDict.DictType.RET:
                    chunks.Add(new RetValueRef(rf.ruleCtx, name, escapedName));
                    break;
                case AttributeDict.DictType.LOCAL:
                    chunks.Add(new LocalRef(nodeContext, name, escapedName));
                    break;
                case AttributeDict.DictType.PREDEFINED_RULE:
                    chunks.Add(GetRulePropertyRef(null, x));
                    break;
                default:
                    break;
            }
        }
        if (node.resolver.resolvesToToken(name, node))
        {
            var tokenLabel = GetTokenLabel(name);
            chunks.Add(new TokenRef(nodeContext, tokenLabel, target.EscapeIfNeeded(tokenLabel))); // $label
            return;
        }
        if (node.resolver.resolvesToLabel(name, node))
        {
            var tokenLabel = GetTokenLabel(name);
            chunks.Add(new LabelRef(nodeContext, tokenLabel, target.EscapeIfNeeded(tokenLabel))); // $x for x=ID etc...
            return;
        }
        if (node.resolver.resolvesToListLabel(name, node))
        {
            chunks.Add(new ListLabelRef(nodeContext, name, escapedName)); // $ids for ids+=ID etc...
            return;
        }
        var r = factory.GetGrammar().getRule(name);
        if (r != null)
        {
            var ruleLabel = GetRuleLabel(name);
            chunks.Add(new LabelRef(nodeContext, ruleLabel, target.EscapeIfNeeded(ruleLabel))); // $r for r rule ref
        }
    }

    //@Override
    public virtual void QualifiedAttr(string expr, Token x, Token y)
    {
        gen.g.Tools.Log("action-translator", "qattr " + x + "." + y);
        if (node.resolver.resolveToAttribute(x.getText(), node) != null)
        {
            // must be a member access to a predefined attribute like $ctx.foo
            Attr(expr, x);
            chunks.Add(new ActionText(nodeContext, "." + y.getText()));
            return;
        }
        var a = node.resolver.resolveToAttribute(x.getText(), y.getText(), node);
        if (a == null)
        {
            // Added in response to https://github.com/antlr/antlr4/issues/1211
            gen.g.Tools.ErrMgr.GrammarError(ErrorType.UNKNOWN_SIMPLE_ATTRIBUTE,
                                           gen.g.fileName, x,
                                           x.getText(),
                                           "rule");
            return;
        }
        switch (a.dict.type)
        {
            case AttributeDict.DictType.ARG: 
                chunks.Add(new ArgRef(nodeContext, y.getText(), target.EscapeIfNeeded(y.getText()))); 
                break; // has to be current rule
            case AttributeDict.DictType.RET:
                chunks.Add(new QRetValueRef(nodeContext, GetRuleLabel(x.getText()), y.getText(), target.EscapeIfNeeded(y.getText())));
                break;
            case AttributeDict.DictType.PREDEFINED_RULE:
                chunks.Add(GetRulePropertyRef(x, y));
                break;
            case AttributeDict.DictType.TOKEN:
                chunks.Add(GetTokenPropertyRef(x, y));
                break;
            default:
                break;
        }
    }

    //@Override
    public virtual void SetAttr(string expr, Token x, Token rhs)
    {
        gen.g.Tools.Log("action-translator", "setAttr " + x + " " + rhs);
        var rhsChunks = TranslateActionChunk(factory, rf, rhs.getText(), node);
        var name = x.getText();
        var s = new SetAttr(nodeContext, name, target.EscapeIfNeeded(name), rhsChunks);
        chunks.Add(s);
    }

    //@Override
    public virtual void NonLocalAttr(string expr, Token x, Token y)
    {
        gen.g.Tools.Log("action-translator", "nonLocalAttr " + x + "::" + y);
        var r = factory.GetGrammar().getRule(x.getText());
        var name = y.getText();
        chunks.Add(new NonLocalAttrRef(nodeContext, x.getText(), name, target.EscapeIfNeeded(name), r.index));
    }

    //@Override
    public virtual void SetNonLocalAttr(string expr, Token x, Token y, Token rhs)
    {
        gen.g.Tools.Log("action-translator", "setNonLocalAttr " + x + "::" + y + "=" + rhs);
        var r = factory.GetGrammar().getRule(x.getText());
        var rhsChunks = TranslateActionChunk(factory, rf, rhs.getText(), node);
        var name = y.getText();
        var s = new SetNonLocalAttr(nodeContext, x.getText(), name, target.EscapeIfNeeded(name), r.index, rhsChunks);
        chunks.Add(s);
    }

    //@Override
    public virtual void Text(string text)
    {
        chunks.Add(new ActionText(nodeContext, text));
    }

    TokenPropertyRef GetTokenPropertyRef(Token x, Token y)
    {
        try
        {
            var c = tokenPropToModelMap[y.getText()];
            var ctor = c.GetConstructor(new Type[] { typeof(StructDecl), typeof(string) });
            return ctor.Invoke(new object[] { nodeContext, GetTokenLabel(x.getText()) }) as TokenPropertyRef;
        }
        catch (Exception e)
        {
            factory.GetGrammar().Tools.ErrMgr.toolError(ErrorType.INTERNAL_ERROR, e);
        }
        return null;
    }

    RulePropertyRef GetRulePropertyRef(Token x, Token prop)
    {
        try
        {
            var c = (x != null ? rulePropToModelMap : thisRulePropToModelMap)[prop.getText()];
            var ctor = c.GetConstructor(new Type[] { typeof(StructDecl), typeof(string) });
            return ctor.Invoke(new object[] { nodeContext, GetRuleLabel((x != null ? x : prop).getText()) }) as RulePropertyRef;
        }
        catch (Exception e)
        {
            factory.GetGrammar().Tools.ErrMgr.toolError(ErrorType.INTERNAL_ERROR, e, prop.getText());
        }
        return null;
    }

    public virtual string GetTokenLabel(string x)
        => node.resolver.resolvesToLabel(x, node) ? x : target.GetImplicitTokenLabel(x);

    public virtual string GetRuleLabel(string x) 
        => node.resolver.resolvesToLabel(x, node) ? x : target.GetImplicitRuleLabel(x);
}
