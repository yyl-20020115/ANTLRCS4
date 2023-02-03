/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.parse;
using org.antlr.v4.runtime;

namespace org.antlr.v4.semantics;


public class BlankActionSplitterListener : ActionSplitterListener {
	//@Override
	public void QualifiedAttr(String expr, Token x, Token y) {
	}

	//@Override
	public void SetAttr(String expr, Token x, Token rhs) {
	}

	//@Override
	public void Attr(String expr, Token x) {
	}

	public void templateInstance(String expr) {
	}

	//@Override
	public void NonLocalAttr(String expr, Token x, Token y) {
	}

	//@Override
	public void SetNonLocalAttr(String expr, Token x, Token y, Token rhs) {
	}

	public void indirectTemplateInstance(String expr) {
	}

	public void setExprAttribute(String expr) {
	}

	public void setSTAttribute(String expr) {
	}

	public void templateExpr(String expr) {
	}

	//@Override
	public void Text(String text) {
	}
}
