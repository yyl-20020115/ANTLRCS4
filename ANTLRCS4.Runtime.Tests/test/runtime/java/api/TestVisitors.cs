/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */
using org.antlr.v4.runtime;
using org.antlr.v4.runtime.atn;
using org.antlr.v4.runtime.tree;
using org.antlr.v4.runtime.tree.pattern;

namespace org.antlr.v4.test.runtime.java.api;

[TestClass]
public class TestVisitors {

	public class VBV: VisitorBasicBaseVisitor<String>()
    {
        //@Override
        public String visitTerminal(TerminalNode node)
        {
            return node.getSymbol().ToString() + "\n";
        }

        //@Override
        protected String defaultResult()
        {
            return "";
        }

        //@Override
        protected String aggregateResult(String aggregate, String nextResult)
        {
            return aggregate + nextResult;
        }
    }
    /**
	 * This test verifies the basic behavior of visitors, with an emphasis on
	 * {@link AbstractParseTreeVisitor#visitTerminal}.
	 */
    [TestMethod]
	public void testVisitTerminalNode() {
		String input = "A";
		VisitorBasicLexer lexer = new VisitorBasicLexer(new ANTLRInputStream(input));
		VisitorBasicParser parser = new VisitorBasicParser(new CommonTokenStream(lexer));

		VisitorBasicParser.SContext context = parser.s();
		Assert.AreEqual("(s A <EOF>)", context.toStringTree(parser));

		VisitorBasicVisitor<String> listener = new VBV();

		String result = listener.visit(context);
		String expected =
			"[@0,0:0='A',<1>,1:0]\n" +
			"[@1,1:0='<EOF>',<-1>,1:1]\n";
		Assert.AreEqual(expected, result);
	}
	class BEL: BaseErrorListener
    {
        //@Override
        public void syntaxError(Recognizer<Token, ATNSimulator> recognizer, Object offendingSymbol, int line, int charPositionInLine, String msg, RecognitionException e)
        {
            errors.add("line " + line + ":" + charPositionInLine + " " + msg);
        }
    }
	class VBN : VisitorBasicBaseVisitor<String>
    {
        //@Override
        public String visitErrorNode(ErrorNode node)
        {
            return "Error encountered: " + node.getSymbol();
        }

        //@Override
        protected String defaultResult()
        {
            return "";
        }

        //@Override
        protected String aggregateResult(String aggregate, String nextResult)
        {
            return aggregate + nextResult;
        }
    }
    /**
	 * This test verifies the basic behavior of visitors, with an emphasis on
	 * {@link AbstractParseTreeVisitor#visitErrorNode}.
	 */
    [TestMethod]
	public void testVisitErrorNode() {
		String input = "";
		VisitorBasicLexer lexer = new VisitorBasicLexer(new ANTLRInputStream(input));
		VisitorBasicParser parser = new VisitorBasicParser(new CommonTokenStream(lexer));

		 List<String> errors = new ();
		parser.removeErrorListeners();
		parser.addErrorListener(new );

		VisitorBasicParser.SContext context = parser.s();
		Assert.AreEqual("(s <missing 'A'> <EOF>)", context.toStringTree(parser));
		Assert.AreEqual(1, errors.Count);
		Assert.AreEqual("line 1:0 missing 'A' at '<EOF>'", errors.get(0));

		VisitorBasicVisitor<String> listener = new;

		String result = listener.visit(context);
		String expected = "Error encountered: [@-1,-1:-1='<missing 'A'>',<1>,1:0]";
		Assert.AreEqual(expected, result);
	}

	/**
	 * This test verifies that {@link AbstractParseTreeVisitor#visitChildren} does not call
	 * {@link org.antlr.v4.runtime.tree.ParseTreeVisitor#visit} after
	 * {@link org.antlr.v4.runtime.tree.AbstractParseTreeVisitor#shouldVisitNextChild} returns
	 * {@code false}.
	 */
	[TestMethod]
	public void testShouldNotVisitEOF() {
		String input = "A";
		VisitorBasicLexer lexer = new VisitorBasicLexer(new ANTLRInputStream(input));
		VisitorBasicParser parser = new VisitorBasicParser(new CommonTokenStream(lexer));

		VisitorBasicParser.SContext context = parser.s();
		Assert.AreEqual("(s A <EOF>)", context.toStringTree(parser));

		VisitorBasicVisitor<String> listener = new VisitorBasicBaseVisitor<String>() {
			//@Override
			public String visitTerminal(TerminalNode node) {
				return node.getSymbol().ToString() + "\n";
			}

			//@Override
			protected bool shouldVisitNextChild(RuleNode node, String currentResult) {
				return currentResult == null || currentResult.isEmpty();
			}
		};

		String result = listener.visit(context);
		String expected = "[@0,0:0='A',<1>,1:0]\n";
		Assert.AreEqual(expected, result);
	}

	/**
	 * This test verifies that {@link AbstractParseTreeVisitor#shouldVisitNextChild} is called before visiting the first
	 * child. It also verifies that {@link AbstractParseTreeVisitor#defaultResult} provides the default return value for
	 * visiting a tree.
	 */
	[TestMethod]
	public void testShouldNotVisitTerminal() {
		String input = "A";
		VisitorBasicLexer lexer = new VisitorBasicLexer(new ANTLRInputStream(input));
		VisitorBasicParser parser = new VisitorBasicParser(new CommonTokenStream(lexer));

		VisitorBasicParser.SContext context = parser.s();
		Assert.AreEqual("(s A <EOF>)", context.toStringTree(parser));

		VisitorBasicVisitor<String> listener = new VisitorBasicBaseVisitor<String>() {
			//@Override
			public String visitTerminal(TerminalNode node) {
				throw new RuntimeException("Should not be reachable");
			}

			//@Override
			protected String defaultResult() {
				return "default result";
			}

			//@Override
			protected bool shouldVisitNextChild(RuleNode node, String currentResult) {
				return false;
			}
		};

		String result = listener.visit(context);
		String expected = "default result";
		Assert.AreEqual(expected, result);
	}

	/**
	 * This test verifies that the visitor correctly dispatches calls for labeled outer alternatives.
	 */
	[TestMethod]
	public void testCalculatorVisitor() {
		String input = "2 + 8 / 2";
		VisitorCalcLexer lexer = new VisitorCalcLexer(new ANTLRInputStream(input));
		VisitorCalcParser parser = new VisitorCalcParser(new CommonTokenStream(lexer));

		VisitorCalcParser.SContext context = parser.s();
		Assert.AreEqual("(s (expr (expr 2) + (expr (expr 8) / (expr 2))) <EOF>)", context.toStringTree(parser));

		VisitorCalcVisitor<int> listener = new VisitorCalcBaseVisitor<int>() {
			//@Override
			public int visitS(VisitorCalcParser.SContext ctx) {
				return visit(ctx.expr());
			}

			//@Override
			public int visitNumber(VisitorCalcParser.NumberContext ctx) {
				return int.valueOf(ctx.INT().getText());
			}

			//@Override
			public int visitMultiply(VisitorCalcParser.MultiplyContext ctx) {
				int left = visit(ctx.expr(0));
				int right = visit(ctx.expr(1));
				if (ctx.MUL() != null) {
					return left * right;
				}
				else {
					return left / right;
				}
			}

			//@Override
			public int visitAdd(VisitorCalcParser.AddContext ctx) {
				int left = visit(ctx.expr(0));
				int right = visit(ctx.expr(1));
				if (ctx.ADD() != null) {
					return left + right;
				}
				else {
					return left - right;
				}
			}

			//@Override
			protected int defaultResult() {
				throw new RuntimeException("Should not be reachable");
			}

			//@Override
			protected int aggregateResult(int aggregate, int nextResult) {
				throw new RuntimeException("Should not be reachable");
			}
		};

		int result = listener.visit(context);
		int expected = 6;
		Assert.AreEqual(expected, result);
	}

}
