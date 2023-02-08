/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.runtime;
using org.antlr.runtime.tree;
using org.antlr.v4.runtime;
using org.antlr.v4.runtime.tree;

namespace org.antlr.v4.test.tool;

public class TestASTStructure
{
    readonly string lexerClassName = "org.antlr.v4.parse.ANTLRLexer";
    readonly string parserClassName = "org.antlr.v4.parse.ANTLRParser";
    readonly string adaptorClassName = "org.antlr.v4.parse.GrammarASTAdaptor";

    public object ExecParser(
        string ruleName,
        string input,
        int scriptLine)
    {
        var @is = new ANTLRStringStream(input);
        var lexerClass = Type.GetType(lexerClassName);
        var lexConstructor = lexerClass.GetConstructor(new Type[] { typeof(CharStream) });
        var lexer = lexConstructor.Invoke(new object[] { @is }) as TokenSource;
        @is.SetLine(scriptLine);

        var tokens = new CommonTokenStream(lexer);

        var parserClass = Type.GetType(parserClassName);
        var parConstructor = parserClass.GetConstructor(new Type[] { typeof(TokenStream) });
        antlr.runtime.Parser parser = parConstructor.Invoke(new object[] { tokens }) as antlr.runtime.Parser;

        // set up customized tree adaptor if necessary
        if (adaptorClassName != null)
        {
            var m = parserClass.GetMethod("setTreeAdaptor", new Type[] { typeof(TreeAdaptor) });
            var adaptorClass = Type.GetType(adaptorClassName);
            m.Invoke(parser,
                new object[] { adaptorClass.GetConstructor(new Type[0]).Invoke(new object[0]) });
        }

        var ruleMethod = parserClass.GetMethod(ruleName);

        // INVOKE RULE
        return ruleMethod.Invoke(parser, new object[0]);
    }

    [TestMethod]
    public void TestGrammarSpec1()
    {
        // gunit test on line 15
        var rstruct = (RuleReturnScope)ExecParser("grammarSpec", "parser grammar P; a : A;", 15);
        var actual = ((Tree)rstruct.Tree).ToStringTree();
        var expecting = "(PARSER_GRAMMAR P (RULES (RULE a (BLOCK (ALT A)))))";
        Assert.AreEqual(expecting, actual, "testing rule grammarSpec");
    }

    [TestMethod]
    public void TestGrammarSpec2()
    {
        // gunit test on line 18
        var rstruct = (RuleReturnScope)ExecParser("grammarSpec", "\n    parser grammar P;\n    tokens { A, B }\n    @header {foo}\n    a : A;\n    ", 18);
        var actual = ((Tree)rstruct.Tree).ToStringTree();
        var expecting = "(PARSER_GRAMMAR P (tokens { A B) (@ header {foo}) (RULES (RULE a (BLOCK (ALT A)))))";
        Assert.AreEqual(expecting, actual, "testing rule grammarSpec");
    }

    [TestMethod]
    public void TestGrammarSpec3()
    {
        // gunit test on line 30
        var rstruct = (RuleReturnScope)ExecParser("grammarSpec", "\n    parser grammar P;\n    @header {foo}\n    tokens { A,B }\n    a : A;\n    ", 30);
        var actual = ((Tree)rstruct.Tree).ToStringTree();
        var expecting = "(PARSER_GRAMMAR P (@ header {foo}) (tokens { A B) (RULES (RULE a (BLOCK (ALT A)))))";
        Assert.AreEqual(expecting, actual, "testing rule grammarSpec");
    }

    [TestMethod]
    public void TestGrammarSpec4()
    {
        // gunit test on line 42
        var rstruct = (RuleReturnScope)ExecParser("grammarSpec", "\n    parser grammar P;\n    import A=B, C;\n    a : A;\n    ", 42);
        var actual = ((Tree)rstruct.Tree).ToStringTree();
        var expecting = "(PARSER_GRAMMAR P (import (= A B) C) (RULES (RULE a (BLOCK (ALT A)))))";
        Assert.AreEqual(expecting, actual, "testing rule grammarSpec");
    }
    [TestMethod]
    public void TestDelegateGrammars1()
    {
        // gunit test on line 53
        var rstruct = (RuleReturnScope)ExecParser("delegateGrammars", "import A;", 53);
        var actual = ((Tree)rstruct.Tree).ToStringTree();
        var expecting = "(import A)";
        Assert.AreEqual(expecting, actual, "testing rule delegateGrammars");
    }
    [TestMethod]
    public void TestRule1()
    {
        // gunit test on line 56
        var rstruct = (RuleReturnScope)ExecParser("rule", "a : A<X,Y=a.b.c>;", 56);
        var actual = ((Tree)rstruct.Tree).ToStringTree();
        var expecting = "(RULE a (BLOCK (ALT (A (ELEMENT_OPTIONS X (= Y a.b.c))))))";
        Assert.AreEqual(expecting, actual, "testing rule rule");
    }

    [TestMethod]
    public void TestRule2()
    {
        // gunit test on line 58
        var rstruct = (RuleReturnScope)ExecParser("rule", "A : B+;", 58);
        var actual = ((Tree)rstruct.Tree).ToStringTree();
        var expecting = "(RULE A (BLOCK (ALT (+ (BLOCK (ALT B))))))";
        Assert.AreEqual(expecting, actual, "testing rule rule");
    }

    [TestMethod]
    public void TestRule3()
    {
        // gunit test on line 60
        var rstruct = (RuleReturnScope)ExecParser("rule", "\n    a[int i] returns [int y]\n    @init {blort}\n      : ID ;\n    ", 60);
        var actual = ((Tree)rstruct.Tree).ToStringTree();
        var expecting = "(RULE a int i (returns int y) (@ init {blort}) (BLOCK (ALT ID)))";
        Assert.AreEqual(expecting, actual, "testing rule rule");
    }

    [TestMethod]
    public void TestRule4()
    {
        // gunit test on line 75
        var rstruct = (RuleReturnScope)ExecParser("rule", "\n    a[int i] returns [int y]\n    @init {blort}\n    options {backtrack=true;}\n      : ID;\n    ", 75);
        var actual = ((Tree)rstruct.Tree).ToStringTree();
        var expecting = "(RULE a int i (returns int y) (@ init {blort}) (OPTIONS (= backtrack true)) (BLOCK (ALT ID)))";
        Assert.AreEqual(expecting, actual, "testing rule rule");
    }

    [TestMethod]
    public void TestRule5()
    {
        // gunit test on line 88
        var rstruct = (RuleReturnScope)ExecParser("rule", "\n    a : ID ;\n      catch[A b] {foo}\n      finally {bar}\n    ", 88);
        var actual = ((Tree)rstruct.Tree).ToStringTree();
        var expecting = "(RULE a (BLOCK (ALT ID)) (catch A b {foo}) (finally {bar}))";
        Assert.AreEqual(expecting, actual, "testing rule rule");
    }

    [TestMethod]
    public void TestRule6()
    {
        // gunit test on line 97
        var rstruct = (RuleReturnScope)ExecParser("rule", "\n    a : ID ;\n      catch[A a] {foo}\n      catch[B b] {fu}\n      finally {bar}\n    ", 97);
        var actual = ((Tree)rstruct.Tree).ToStringTree();
        var expecting = "(RULE a (BLOCK (ALT ID)) (catch A a {foo}) (catch B b {fu}) (finally {bar}))";
        Assert.AreEqual(expecting, actual, "testing rule rule");
    }

    [TestMethod]
    public void TestRule7()
    {
        // gunit test on line 107
        var rstruct = (RuleReturnScope)ExecParser("rule", "\n\ta[int i]\n\tlocals [int a, float b]\n\t\t:\tA\n\t\t;\n\t", 107);
        var actual = ((Tree)rstruct.Tree).ToStringTree();
        var expecting = "(RULE a int i (locals int a, float b) (BLOCK (ALT A)))";
        Assert.AreEqual(expecting, actual, "testing rule rule");
    }

    [TestMethod]
    public void TestRule8()
    {
        // gunit test on line 115
        var rstruct = (RuleReturnScope)ExecParser("rule", "\n\ta[int i] throws a.b.c\n\t\t:\tA\n\t\t;\n\t", 115);
        var actual = ((Tree)rstruct.Tree).ToStringTree();
        var expecting = "(RULE a int i (throws a.b.c) (BLOCK (ALT A)))";
        Assert.AreEqual(expecting, actual, "testing rule rule");
    }
    [TestMethod]
    public void TestEbnf1()
    {
        // gunit test on line 123
        var rstruct = (RuleReturnScope)ExecParser("ebnf", "(A|B)", 123);
        var actual = ((Tree)rstruct.Tree).ToStringTree();
        var expecting = "(BLOCK (ALT A) (ALT B))";
        Assert.AreEqual(expecting, actual, "testing rule ebnf");
    }

    [TestMethod]
    public void TestEbnf2()
    {
        // gunit test on line 124
        var rstruct = (RuleReturnScope)ExecParser("ebnf", "(A|B)?", 124);
        var actual = ((Tree)rstruct.Tree).ToStringTree();
        var expecting = "(? (BLOCK (ALT A) (ALT B)))";
        Assert.AreEqual(expecting, actual, "testing rule ebnf");
    }

    [TestMethod]
    public void TestEbnf3()
    {
        // gunit test on line 125
        var rstruct = (RuleReturnScope)ExecParser("ebnf", "(A|B)*", 125);
        var actual = ((Tree)rstruct.Tree).ToStringTree();
        var expecting = "(* (BLOCK (ALT A) (ALT B)))";
        Assert.AreEqual(expecting, actual, "testing rule ebnf");
    }

    [TestMethod]
    public void TestEbnf4()
    {
        // gunit test on line 126
        var rstruct = (RuleReturnScope)ExecParser("ebnf", "(A|B)+", 126);
        var actual = ((Tree)rstruct.Tree).ToStringTree();
        var expecting = "(+ (BLOCK (ALT A) (ALT B)))";
        Assert.AreEqual(expecting, actual, "testing rule ebnf");
    }
    [TestMethod]
    public void TestElement1()
    {
        // gunit test on line 129
        var rstruct = (RuleReturnScope)ExecParser("element", "~A", 129);
        var actual = ((Tree)rstruct.Tree).ToStringTree();
        var expecting = "(~ (SET A))";
        Assert.AreEqual(expecting, actual, "testing rule element");
    }

    [TestMethod]
    public void TestElement2()
    {
        // gunit test on line 130
        var rstruct = (RuleReturnScope)ExecParser("element", "b+", 130);
        var actual = ((Tree)rstruct.Tree).ToStringTree();
        var expecting = "(+ (BLOCK (ALT b)))";
        Assert.AreEqual(expecting, actual, "testing rule element");
    }

    [TestMethod]
    public void TestElement3()
    {
        // gunit test on line 131
        var rstruct = (RuleReturnScope)ExecParser("element", "(b)+", 131);
        var actual = ((Tree)rstruct.Tree).ToStringTree();
        var expecting = "(+ (BLOCK (ALT b)))";
        Assert.AreEqual(expecting, actual, "testing rule element");
    }

    [TestMethod]
    public void TestElement4()
    {
        // gunit test on line 132
        var rstruct = (RuleReturnScope)ExecParser("element", "b?", 132);
        var actual = ((Tree)rstruct.Tree).ToStringTree();
        var expecting = "(? (BLOCK (ALT b)))";
        Assert.AreEqual(expecting, actual, "testing rule element");
    }

    [TestMethod]
    public void TestElement5()
    {
        // gunit test on line 133
        var rstruct = (RuleReturnScope)ExecParser("element", "(b)?", 133);
        var actual = ((Tree)rstruct.Tree).ToStringTree();
        var expecting = "(? (BLOCK (ALT b)))";
        Assert.AreEqual(expecting, actual, "testing rule element");
    }

    [TestMethod]
    public void TestElement6()
    {
        // gunit test on line 134
        var rstruct = (RuleReturnScope)ExecParser("element", "(b)*", 134);
        var actual = ((Tree)rstruct.Tree).ToStringTree();
        var expecting = "(* (BLOCK (ALT b)))";
        Assert.AreEqual(expecting, actual, "testing rule element");
    }

    [TestMethod]
    public void TestElement7()
    {
        // gunit test on line 135
        var rstruct = (RuleReturnScope)ExecParser("element", "b*", 135);
        var actual = ((Tree)rstruct.Tree).ToStringTree();
        var expecting = "(* (BLOCK (ALT b)))";
        Assert.AreEqual(expecting, actual, "testing rule element");
    }

    [TestMethod]
    public void TestElement8()
    {
        // gunit test on line 136
        var rstruct = (RuleReturnScope)ExecParser("element", "'while'*", 136);
        var actual = ((Tree)rstruct.Tree).ToStringTree();
        var expecting = "(* (BLOCK (ALT 'while')))";
        Assert.AreEqual(expecting, actual, "testing rule element");
    }

    [TestMethod]
    public void TestElement9()
    {
        // gunit test on line 137
        var rstruct = (RuleReturnScope)ExecParser("element", "'a'+", 137);
        var actual = ((Tree)rstruct.Tree).ToStringTree();
        var expecting = "(+ (BLOCK (ALT 'a')))";
        Assert.AreEqual(expecting, actual, "testing rule element");
    }

    [TestMethod]
    public void TestElement10()
    {
        // gunit test on line 138
        var rstruct = (RuleReturnScope)ExecParser("element", "a[3]", 138);
        var actual = ((Tree)rstruct.Tree).ToStringTree();
        var expecting = "(a 3)";
        Assert.AreEqual(expecting, actual, "testing rule element");
    }

    [TestMethod]
    public void TestElement11()
    {
        // gunit test on line 139
        var rstruct = (RuleReturnScope)ExecParser("element", "'a'..'z'+", 139);
        var actual = ((Tree)rstruct.Tree).ToStringTree();
        var expecting = "(+ (BLOCK (ALT (.. 'a' 'z'))))";
        Assert.AreEqual(expecting, actual, "testing rule element");
    }

    [TestMethod]
    public void TestElement12()
    {
        // gunit test on line 140
        var rstruct = (RuleReturnScope)ExecParser("element", "x=ID", 140);
        var actual = ((Tree)rstruct.Tree).ToStringTree();
        var expecting = "(= x ID)";
        Assert.AreEqual(expecting, actual, "testing rule element");
    }

    [TestMethod]
    public void TestElement13()
    {
        // gunit test on line 141
        var rstruct = (RuleReturnScope)ExecParser("element", "x=ID?", 141);
        var actual = ((Tree)rstruct.Tree).ToStringTree();
        var expecting = "(? (BLOCK (ALT (= x ID))))";
        Assert.AreEqual(expecting, actual, "testing rule element");
    }

    [TestMethod]
    public void TestElement14()
    {
        // gunit test on line 142
        var rstruct = (RuleReturnScope)ExecParser("element", "x=ID*", 142);
        var actual = ((Tree)rstruct.Tree).ToStringTree();
        var expecting = "(* (BLOCK (ALT (= x ID))))";
        Assert.AreEqual(expecting, actual, "testing rule element");
    }

    [TestMethod]
    public void TestElement15()
    {
        // gunit test on line 143
        var rstruct = (RuleReturnScope)ExecParser("element", "x=b", 143);
        var actual = ((Tree)rstruct.Tree).ToStringTree();
        var expecting = "(= x b)";
        Assert.AreEqual(expecting, actual, "testing rule element");
    }

    [TestMethod]
    public void TestElement16()
    {
        // gunit test on line 144
        var rstruct = (RuleReturnScope)ExecParser("element", "x=(A|B)", 144);
        var actual = ((Tree)rstruct.Tree).ToStringTree();
        var expecting = "(= x (BLOCK (ALT A) (ALT B)))";
        Assert.AreEqual(expecting, actual, "testing rule element");
    }

    [TestMethod]
    public void TestElement17()
    {
        // gunit test on line 145
        var rstruct = (RuleReturnScope)ExecParser("element", "x=~(A|B)", 145);
        var actual = ((Tree)rstruct.Tree).ToStringTree();
        var expecting = "(= x (~ (SET A B)))";
        Assert.AreEqual(expecting, actual, "testing rule element");
    }

    [TestMethod]
    public void TestElement18()
    {
        // gunit test on line 146
        var rstruct = (RuleReturnScope)ExecParser("element", "x+=~(A|B)", 146);
        var actual = ((Tree)rstruct.Tree).ToStringTree();
        var expecting = "(+= x (~ (SET A B)))";
        Assert.AreEqual(expecting, actual, "testing rule element");
    }

    [TestMethod]
    public void TestElement19()
    {
        // gunit test on line 147
        var rstruct = (RuleReturnScope)ExecParser("element", "x+=~(A|B)+", 147);
        var actual = ((Tree)rstruct.Tree).ToStringTree();
        var expecting = "(+ (BLOCK (ALT (+= x (~ (SET A B))))))";
        Assert.AreEqual(expecting, actual, "testing rule element");
    }

    [TestMethod]
    public virtual void TestElement20()
    {
        // gunit test on line 148
        var rstruct = (RuleReturnScope)ExecParser("element", "x=b+", 148);
        var actual = ((Tree)rstruct.Tree).ToStringTree();
        var expecting = "(+ (BLOCK (ALT (= x b))))";
        Assert.AreEqual(expecting, actual, "testing rule element");
    }

    [TestMethod]
    public void TestElement21()
    {
        // gunit test on line 149
        var rstruct = (RuleReturnScope)ExecParser("element", "x+=ID*", 149);
        var actual = ((Tree)rstruct.Tree).ToStringTree();
        var expecting = "(* (BLOCK (ALT (+= x ID))))";
        Assert.AreEqual(expecting, actual, "testing rule element");
    }

    [TestMethod]
    public void TestElement22()
    {
        // gunit test on line 150
        var rstruct = (RuleReturnScope)ExecParser("element", "x+='int'*", 150);
        var actual = ((Tree)rstruct.Tree).ToStringTree();
        var expecting = "(* (BLOCK (ALT (+= x 'int'))))";
        Assert.AreEqual(expecting, actual, "testing rule element");
    }

    [TestMethod]
    public void TestElement23()
    {
        // gunit test on line 151
        var rstruct = (RuleReturnScope)ExecParser("element", "x+=b+", 151);
        var actual = ((Tree)rstruct.Tree).ToStringTree();
        var expecting = "(+ (BLOCK (ALT (+= x b))))";
        Assert.AreEqual(expecting, actual, "testing rule element");
    }

    [TestMethod]
    public void TestElement24()
    {
        // gunit test on line 152
        var rstruct = (RuleReturnScope)ExecParser("element", "({blort} 'x')*", 152);
        var actual = ((Tree)rstruct.Tree).ToStringTree();
        var expecting = "(* (BLOCK (ALT {blort} 'x')))";
        Assert.AreEqual(expecting, actual, "testing rule element");
    }
}
