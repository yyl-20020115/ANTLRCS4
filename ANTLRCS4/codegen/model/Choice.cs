/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.codegen.model.decl;
using org.antlr.v4.runtime.misc;
using org.antlr.v4.tool;
using org.antlr.v4.tool.ast;

namespace org.antlr.v4.codegen.model;


/** The class hierarchy underneath SrcOp is pretty deep but makes sense that,
 *  for example LL1StarBlock is a kind of LL1Loop which is a kind of Choice.
 *  The problem is it's impossible to figure
 *  out how to construct one of these deeply nested objects because of the
 *  long super constructor call chain. Instead, I decided to in-line all of
 *  this and then look for opportunities to re-factor code into functions.
 *  It makes sense to use a class hierarchy to share data fields, but I don't
 *  think it makes sense to factor code using super constructors because
 *  it has too much work to do.
 */
public abstract class Choice : RuleElement {
	public int decision = -1;
	public Decl label;

	//@ModelElement 
	public List<CodeBlockForAlt> alts;
	//@ModelElement
	public List<SrcOp> preamble = new ();

	public Choice(OutputModelFactory factory,
				  GrammarAST blkOrEbnfRootAST,
				  List<CodeBlockForAlt> alts)
		: base(factory, blkOrEbnfRootAST)
    {
		this.alts = alts;
	}

	public void addPreambleOp(SrcOp op) {
		preamble.Add(op);
	}

	public List<TokenInfo[]> getAltLookaheadAsStringLists(IntervalSet[] altLookSets) {
		List<TokenInfo[]> altLook = new ();
		Target target = factory.getGenerator().getTarget();
		Grammar grammar = factory.getGrammar();
		foreach (IntervalSet s in altLookSets) {
			IntegerList list = s.toIntegerList();
			TokenInfo[] info = new TokenInfo[list.size()];
			for (int i = 0; i < info.Length; i++) {
				info[i] = new TokenInfo(list.get(i), target.getTokenTypeAsTargetLabel(grammar, list.get(i)));
			}
			altLook.Add(info);
		}
		return altLook;
	}

	public TestSetInline addCodeForLookaheadTempVar(IntervalSet look) {
		List<SrcOp> testOps = factory.getLL1Test(look, ast);
		TestSetInline expr = RuntimeUtils.find(testOps, typeof(TestSetInline));
		if (expr != null) {
			Decl d = new TokenTypeDecl(factory, expr.varName);
			factory.getCurrentRuleFunction().addLocalDecl(d);
			CaptureNextTokenType nextType = new CaptureNextTokenType(factory,expr.varName);
			addPreambleOp(nextType);
		}
		return expr;
	}

	public ThrowNoViableAlt getThrowNoViableAlt(OutputModelFactory factory,
												GrammarAST blkAST,
												IntervalSet expecting) {
		return new ThrowNoViableAlt(factory, blkAST, expecting);
	}
}
