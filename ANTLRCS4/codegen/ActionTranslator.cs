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
using System.Reflection;
using System.Text;

namespace org.antlr.v4.codegen;



/** */
public class ActionTranslator : ActionSplitterListener {
	public static readonly Dictionary<String, Type> thisRulePropToModelMap =
		new ();
	static ActionTranslator(){
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

    public static readonly Dictionary<String, Type> rulePropToModelMap =
		new ();
	
    public static readonly Dictionary<String,Type> tokenPropToModelMap =
		new ();
	
	readonly CodeGenerator gen;
	readonly Target target;
	readonly ActionAST node;
	RuleFunction rf;
	readonly List<ActionChunk> chunks = new ();
	readonly OutputModelFactory factory;
	StructDecl nodeContext;

	public ActionTranslator(OutputModelFactory factory, ActionAST node) {
		this.factory = factory;
		this.node = node;
		this.gen = factory.getGenerator();
		this.target = gen.getTarget();
	}

	public static String ToString(List<ActionChunk> chunks) {
		StringBuilder buf = new StringBuilder();
		foreach (ActionChunk c in chunks) buf.Append(c.ToString());
		return buf.ToString();
	}

	public static List<ActionChunk> translateAction(OutputModelFactory factory,
													RuleFunction rf,
													Token tokenWithinAction,
													ActionAST node)
	{
		String action = tokenWithinAction.getText();
		if ( action!=null && action.Length>0 && action[(0)]=='{' ) {
			int firstCurly = action.IndexOf('{');
			int lastCurly = action.LastIndexOf('}');
			if ( firstCurly>=0 && lastCurly>=0 ) {
				action = action.Substring(firstCurly+1, lastCurly-(firstCurly + 1)); // trim {...}
			}
		}
		return translateActionChunk(factory, rf, action, node);
	}

	public static List<ActionChunk> translateActionChunk(OutputModelFactory factory,
														 RuleFunction rf,
														 String action,
														 ActionAST node)
	{
		Token tokenWithinAction = node.token;
		ActionTranslator translator = new ActionTranslator(factory, node);
		translator.rf = rf;
        factory.getGrammar().tool.log("action-translator", "translate " + action);
		String altLabel = node.getAltLabel();
		if ( rf!=null ) {
		    translator.nodeContext = rf.ruleCtx;
	        if ( altLabel!=null ) translator.nodeContext = rf.altLabelCtxs.get(altLabel);
		}
		ANTLRStringStream @in = new ANTLRStringStream(action);
		@in.setLine(tokenWithinAction.getLine());
		@in.setCharPositionInLine(tokenWithinAction.getCharPositionInLine());
		ActionSplitter trigger = new ActionSplitter(@in, translator);
		// forces eval, triggers listener methods
		trigger.getActionTokens();
		return translator.chunks;
	}

	//@Override
	public void attr(String expr, Token x) {
		gen.g.tool.log("action-translator", "attr "+x);
        tool.Attribute a = node.resolver.resolveToAttribute(x.getText(), node);
		String name = x.getText();
		String escapedName = target.escapeIfNeeded(name);
		if ( a!=null ) {
			switch ( a.dict.type ) {
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
					chunks.Add(getRulePropertyRef(null, x));
					break;
				default:
					break;
			}
		}
		if ( node.resolver.resolvesToToken(name, node) ) {
			String tokenLabel = getTokenLabel(name);
			chunks.Add(new TokenRef(nodeContext, tokenLabel, target.escapeIfNeeded(tokenLabel))); // $label
			return;
		}
		if ( node.resolver.resolvesToLabel(name, node) ) {
			String tokenLabel = getTokenLabel(name);
			chunks.Add(new LabelRef(nodeContext, tokenLabel, target.escapeIfNeeded(tokenLabel))); // $x for x=ID etc...
			return;
		}
		if ( node.resolver.resolvesToListLabel(name, node) ) {
			chunks.Add(new ListLabelRef(nodeContext, name, escapedName)); // $ids for ids+=ID etc...
			return;
		}
		Rule r = factory.getGrammar().getRule(name);
		if ( r!=null ) {
			String ruleLabel = getRuleLabel(name);
			chunks.Add(new LabelRef(nodeContext, ruleLabel, target.escapeIfNeeded(ruleLabel))); // $r for r rule ref
		}
	}

	//@Override
	public void qualifiedAttr(String expr, Token x, Token y) {
		gen.g.tool.log("action-translator", "qattr "+x+"."+y);
		if ( node.resolver.resolveToAttribute(x.getText(), node)!=null ) {
			// must be a member access to a predefined attribute like $ctx.foo
			attr(expr, x);
			chunks.Add(new ActionText(nodeContext, "."+y.getText()));
			return;
		}
		tool.Attribute a = node.resolver.resolveToAttribute(x.getText(), y.getText(), node);
		if ( a==null ) {
			// Added in response to https://github.com/antlr/antlr4/issues/1211
			gen.g.tool.errMgr.grammarError(ErrorType.UNKNOWN_SIMPLE_ATTRIBUTE,
			                               gen.g.fileName, x,
			                               x.getText(),
			                               "rule");
			return;
		}
		switch ( a.dict.type ) {
			case AttributeDict.DictType.ARG: chunks.Add(new ArgRef(nodeContext, y.getText(), target.escapeIfNeeded(y.getText()))); break; // has to be current rule
			case AttributeDict.DictType.RET:
				chunks.Add(new QRetValueRef(nodeContext, getRuleLabel(x.getText()), y.getText(), target.escapeIfNeeded(y.getText())));
				break;
			case AttributeDict.DictType.PREDEFINED_RULE:
				chunks.Add(getRulePropertyRef(x, y));
				break;
			case AttributeDict.DictType.TOKEN:
				chunks.Add(getTokenPropertyRef(x, y));
				break;
			default:
				break;
		}
	}

	//@Override
	public void setAttr(String expr, Token x, Token rhs) {
		gen.g.tool.log("action-translator", "setAttr "+x+" "+rhs);
		List<ActionChunk> rhsChunks = translateActionChunk(factory,rf,rhs.getText(),node);
		String name = x.getText();
		SetAttr s = new SetAttr(nodeContext, name, target.escapeIfNeeded(name), rhsChunks);
		chunks.Add(s);
	}

	//@Override
	public void nonLocalAttr(String expr, Token x, Token y) {
		gen.g.tool.log("action-translator", "nonLocalAttr "+x+"::"+y);
		Rule r = factory.getGrammar().getRule(x.getText());
		String name = y.getText();
		chunks.Add(new NonLocalAttrRef(nodeContext, x.getText(), name, target.escapeIfNeeded(name), r.index));
	}

	//@Override
	public void setNonLocalAttr(String expr, Token x, Token y, Token rhs) {
		gen.g.tool.log("action-translator", "setNonLocalAttr "+x+"::"+y+"="+rhs);
		Rule r = factory.getGrammar().getRule(x.getText());
		List<ActionChunk> rhsChunks = translateActionChunk(factory,rf,rhs.getText(),node);
		String name = y.getText();
		SetNonLocalAttr s = new SetNonLocalAttr(nodeContext, x.getText(), name, target.escapeIfNeeded(name), r.index, rhsChunks);
		chunks.Add(s);
	}

	//@Override
	public void text(String text) {
		chunks.Add(new ActionText(nodeContext,text));
	}

	TokenPropertyRef getTokenPropertyRef(Token x, Token y) {
		try {
			Type c = tokenPropToModelMap.get(y.getText());
			ConstructorInfo ctor = c.getConstructor(typeof(StructDecl), String);
			return ctor.newInstance(nodeContext, getTokenLabel(x.getText()));
		}
		catch (Exception e) {
			factory.getGrammar().tool.errMgr.toolError(ErrorType.INTERNAL_ERROR, e);
		}
		return null;
	}

	RulePropertyRef getRulePropertyRef(Token x, Token prop) {
		try {
			Type c = (x != null ? rulePropToModelMap : thisRulePropToModelMap).get(prop.getText());
			ConstructorInfo ctor = c.getConstructor(typeof(StructDecl), typeof(String));
			return ctor.newInstance(nodeContext, getRuleLabel((x != null ? x : prop).getText()));
		}
		catch (Exception e) {
			factory.getGrammar().tool.errMgr.toolError(ErrorType.INTERNAL_ERROR, e, prop.getText());
		}
		return null;
	}

	public String getTokenLabel(String x) {
		if ( node.resolver.resolvesToLabel(x, node) ) return x;
		return target.getImplicitTokenLabel(x);
	}

	public String getRuleLabel(String x) {
		if ( node.resolver.resolvesToLabel(x, node) ) return x;
		return target.getImplicitRuleLabel(x);
	}
}
