/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime.atn;
using org.antlr.v4.runtime.tree;
using System.Text;

namespace org.antlr.v4.test.tool;

[TestClass]
public class TestGraphNodes
{
    public bool RootIsWildcard() => true;
    public bool FullCtx() => false;

    [TestMethod]
    public void TestDollarDollar()
    {
        var r = PredictionContext.Merge(
                EmptyPredictionContext.Instance, EmptyPredictionContext.Instance, RootIsWildcard(), null);
        //		Console.Out.WriteLine(toDOTString(r, rootIsWildcard()));
        var expecting =
            "digraph G {\n" +
            "rankdir=LR;\n" +
            "  s0[label=\"*\"];\n" +
            "}\n";
        Assert.AreEqual(expecting, ToDOTString(r, RootIsWildcard()));
    }

    [TestMethod]
    public void TestDollarDollarFullctx()
    {
        var r = PredictionContext.Merge(
                EmptyPredictionContext.Instance, EmptyPredictionContext.Instance, FullCtx(), null);
        //		Console.Out.WriteLine(toDOTString(r, fullCtx()));
        var expecting =
            "digraph G {\n" +
            "rankdir=LR;\n" +
            "  s0[label=\"$\"];\n" +
            "}\n";
        Assert.AreEqual(expecting, ToDOTString(r, FullCtx()));
    }

    [TestMethod]
    public void TestXDollar()
    {
        var r = PredictionContext.Merge(X(), EmptyPredictionContext.Instance, RootIsWildcard(), null);
        //		Console.Out.WriteLine(toDOTString(r, rootIsWildcard()));
        var expecting =
            "digraph G {\n" +
            "rankdir=LR;\n" +
            "  s0[label=\"*\"];\n" +
            "}\n";
        Assert.AreEqual(expecting, ToDOTString(r, RootIsWildcard()));
    }

    [TestMethod]
    public void TestXDollarFullctx()
    {
        var r = PredictionContext.Merge(X(), EmptyPredictionContext.Instance, FullCtx(), null);
        //		Console.Out.WriteLine(toDOTString(r, fullCtx()));
        var expecting =
            "digraph G {\n" +
            "rankdir=LR;\n" +
            "  s0[shape=record, label=\"<p0>|<p1>$\"];\n" +
            "  s1[label=\"$\"];\n" +
            "  s0:p0->s1[label=\"9\"];\n" +
            "}\n";
        Assert.AreEqual(expecting, ToDOTString(r, FullCtx()));
    }

    [TestMethod]
    public void TestDollarX()
    {
        var r = PredictionContext.Merge(EmptyPredictionContext.Instance, X(), RootIsWildcard(), null);
        //		Console.Out.WriteLine(toDOTString(r, rootIsWildcard()));
        var expecting =
            "digraph G {\n" +
            "rankdir=LR;\n" +
            "  s0[label=\"*\"];\n" +
            "}\n";
        Assert.AreEqual(expecting, ToDOTString(r, RootIsWildcard()));
    }

    [TestMethod]
    public void TestDollarXFullctx()
    {
        PredictionContext r = PredictionContext.Merge(EmptyPredictionContext.Instance, X(), FullCtx(), null);
        //		Console.Out.WriteLine(toDOTString(r, fullCtx()));
        String expecting =
            "digraph G {\n" +
            "rankdir=LR;\n" +
            "  s0[shape=record, label=\"<p0>|<p1>$\"];\n" +
            "  s1[label=\"$\"];\n" +
            "  s0:p0->s1[label=\"9\"];\n" +
            "}\n";
        Assert.AreEqual(expecting, ToDOTString(r, FullCtx()));
    }

    [TestMethod]
    public void TestAA()
    {
        var r = PredictionContext.Merge(A(), A(), RootIsWildcard(), null);
        //		Console.Out.WriteLine(toDOTString(r, rootIsWildcard()));
        var expecting =
            "digraph G {\n" +
            "rankdir=LR;\n" +
            "  s0[label=\"0\"];\n" +
            "  s1[label=\"*\"];\n" +
            "  s0->s1[label=\"1\"];\n" +
            "}\n";
        Assert.AreEqual(expecting, ToDOTString(r, RootIsWildcard()));
    }

    [TestMethod]
    public void TestAdollarAx()
    {
        var a1 = A();
        var _x = X();
        var a2 = CreateSingleton(_x, 1);
        var r = PredictionContext.Merge(a1, a2, RootIsWildcard(), null);
        //		Console.Out.WriteLine(toDOTString(r, rootIsWildcard()));
        var expecting =
            "digraph G {\n" +
            "rankdir=LR;\n" +
            "  s0[label=\"0\"];\n" +
            "  s1[label=\"*\"];\n" +
            "  s0->s1[label=\"1\"];\n" +
            "}\n";
        Assert.AreEqual(expecting, ToDOTString(r, RootIsWildcard()));
    }

    [TestMethod]
    public void TestAdollarAxFullctx()
    {
        var a1 = A();
        var _x = X();
        var a2 = CreateSingleton(_x, 1);
        var r = PredictionContext.Merge(a1, a2, FullCtx(), null);
        //		Console.Out.WriteLine(toDOTString(r, fullCtx()));
        var expecting =
            "digraph G {\n" +
            "rankdir=LR;\n" +
            "  s0[label=\"0\"];\n" +
            "  s1[shape=record, label=\"<p0>|<p1>$\"];\n" +
            "  s2[label=\"$\"];\n" +
            "  s0->s1[label=\"1\"];\n" +
            "  s1:p0->s2[label=\"9\"];\n" +
            "}\n";
        Assert.AreEqual(expecting, ToDOTString(r, FullCtx()));
    }

    [TestMethod]
    public void TestAxdollarAdollar()
    {
        var _x = X();
        var a1 = CreateSingleton(_x, 1);
        var a2 = A();
        var r = PredictionContext.Merge(a1, a2, RootIsWildcard(), null);
        //		Console.Out.WriteLine(toDOTString(r, rootIsWildcard()));
        var expecting =
            "digraph G {\n" +
            "rankdir=LR;\n" +
            "  s0[label=\"0\"];\n" +
            "  s1[label=\"*\"];\n" +
            "  s0->s1[label=\"1\"];\n" +
            "}\n";
        Assert.AreEqual(expecting, ToDOTString(r, RootIsWildcard()));
    }

    [TestMethod]
    public void TestAadollarAdollarDollarFullCtx()
    {
        var empty = EmptyPredictionContext.Instance;
        var child1 = CreateSingleton(empty, 8);
        var right = PredictionContext.Merge(empty, child1, false, null);
        var left = CreateSingleton(right, 8);
        var merged = PredictionContext.Merge(left, right, false, null);
        var actual = ToDOTString(merged, false);
        //		Console.Out.WriteLine(actual);
        var expecting =
            "digraph G {\n" +
            "rankdir=LR;\n" +
            "  s0[shape=record, label=\"<p0>|<p1>$\"];\n" +
            "  s1[shape=record, label=\"<p0>|<p1>$\"];\n" +
            "  s2[label=\"$\"];\n" +
            "  s0:p0->s1[label=\"8\"];\n" +
            "  s1:p0->s2[label=\"8\"];\n" +
            "}\n";
        Assert.AreEqual(expecting, actual);
    }

    [TestMethod]
    public void TestAxdollarAdollarFullctx()
    {
        var _x = X();
        var a1 = CreateSingleton(_x, 1);
        var a2 = A();
        var r = PredictionContext.Merge(a1, a2, FullCtx(), null);
        //		Console.Out.WriteLine(toDOTString(r, fullCtx()));
        var expecting =
            "digraph G {\n" +
            "rankdir=LR;\n" +
            "  s0[label=\"0\"];\n" +
            "  s1[shape=record, label=\"<p0>|<p1>$\"];\n" +
            "  s2[label=\"$\"];\n" +
            "  s0->s1[label=\"1\"];\n" +
            "  s1:p0->s2[label=\"9\"];\n" +
            "}\n";
        Assert.AreEqual(expecting, ToDOTString(r, FullCtx()));
    }

    [TestMethod]
    public void TestAB()
    {
        var r = PredictionContext.Merge(A(), B(), RootIsWildcard(), null);
        //		Console.Out.WriteLine(toDOTString(r, rootIsWildcard()));
        var expecting =
            "digraph G {\n" +
            "rankdir=LR;\n" +
            "  s0[shape=record, label=\"<p0>|<p1>\"];\n" +
            "  s1[label=\"*\"];\n" +
            "  s0:p0->s1[label=\"1\"];\n" +
            "  s0:p1->s1[label=\"2\"];\n" +
            "}\n";
        Assert.AreEqual(expecting, ToDOTString(r, RootIsWildcard()));
    }

    [TestMethod]
    public void TestAxAxSame()
    {
        var _x = X();
        var a1 = CreateSingleton(_x, 1);
        var a2 = CreateSingleton(_x, 1);
        var r = PredictionContext.Merge(a1, a2, RootIsWildcard(), null);
        //		Console.Out.WriteLine(toDOTString(r, rootIsWildcard()));
        var expecting =
            "digraph G {\n" +
            "rankdir=LR;\n" +
            "  s0[label=\"0\"];\n" +
            "  s1[label=\"1\"];\n" +
            "  s2[label=\"*\"];\n" +
            "  s0->s1[label=\"1\"];\n" +
            "  s1->s2[label=\"9\"];\n" +
            "}\n";
        Assert.AreEqual(expecting, ToDOTString(r, RootIsWildcard()));
    }

    [TestMethod]
    public void TestAxAx()
    {
        var x1 = X();
        var x2 = X();
        var a1 = CreateSingleton(x1, 1);
        var a2 = CreateSingleton(x2, 1);
        var r = PredictionContext.Merge(a1, a2, RootIsWildcard(), null);
        //		Console.Out.WriteLine(toDOTString(r, rootIsWildcard()));
        var expecting =
            "digraph G {\n" +
            "rankdir=LR;\n" +
            "  s0[label=\"0\"];\n" +
            "  s1[label=\"1\"];\n" +
            "  s2[label=\"*\"];\n" +
            "  s0->s1[label=\"1\"];\n" +
            "  s1->s2[label=\"9\"];\n" +
            "}\n";
        Assert.AreEqual(expecting, ToDOTString(r, RootIsWildcard()));
    }

    [TestMethod]
    public void TestAbxAbx()
    {
        var x1 = X();
        var x2 = X();
        var b1 = CreateSingleton(x1, 2);
        var b2 = CreateSingleton(x2, 2);
        var a1 = CreateSingleton(b1, 1);
        var a2 = CreateSingleton(b2, 1);
        var r = PredictionContext.Merge(a1, a2, RootIsWildcard(), null);
        //		Console.Out.WriteLine(toDOTString(r, rootIsWildcard()));
        var expecting =
            "digraph G {\n" +
            "rankdir=LR;\n" +
            "  s0[label=\"0\"];\n" +
            "  s1[label=\"1\"];\n" +
            "  s2[label=\"2\"];\n" +
            "  s3[label=\"*\"];\n" +
            "  s0->s1[label=\"1\"];\n" +
            "  s1->s2[label=\"2\"];\n" +
            "  s2->s3[label=\"9\"];\n" +
            "}\n";
        Assert.AreEqual(expecting, ToDOTString(r, RootIsWildcard()));
    }

    [TestMethod]
    public void TestAbxAcx()
    {
        var x1 = X();
        var x2 = X();
        var b = CreateSingleton(x1, 2);
        var c = CreateSingleton(x2, 3);
        var a1 = CreateSingleton(b, 1);
        var a2 = CreateSingleton(c, 1);
        var r = PredictionContext.Merge(a1, a2, RootIsWildcard(), null);
        //		Console.Out.WriteLine(toDOTString(r, rootIsWildcard()));
        var expecting =
            "digraph G {\n" +
            "rankdir=LR;\n" +
            "  s0[label=\"0\"];\n" +
            "  s1[shape=record, label=\"<p0>|<p1>\"];\n" +
            "  s2[label=\"2\"];\n" +
            "  s3[label=\"*\"];\n" +
            "  s0->s1[label=\"1\"];\n" +
            "  s1:p0->s2[label=\"2\"];\n" +
            "  s1:p1->s2[label=\"3\"];\n" +
            "  s2->s3[label=\"9\"];\n" +
            "}\n";
        Assert.AreEqual(expecting, ToDOTString(r, RootIsWildcard()));
    }

    [TestMethod]
    public void TestAxBxSame()
    {
        var _x = X();
        var a = CreateSingleton(_x, 1);
        var b = CreateSingleton(_x, 2);
        var r = PredictionContext.Merge(a, b, RootIsWildcard(), null);
        //		Console.Out.WriteLine(toDOTString(r, rootIsWildcard()));
        var expecting =
            "digraph G {\n" +
            "rankdir=LR;\n" +
            "  s0[shape=record, label=\"<p0>|<p1>\"];\n" +
            "  s1[label=\"1\"];\n" +
            "  s2[label=\"*\"];\n" +
            "  s0:p0->s1[label=\"1\"];\n" +
            "  s0:p1->s1[label=\"2\"];\n" +
            "  s1->s2[label=\"9\"];\n" +
            "}\n";
        Assert.AreEqual(expecting, ToDOTString(r, RootIsWildcard()));
    }

    [TestMethod]
    public void TestAxBx()
    {
        var x1 = X();
        var x2 = X();
        var a = CreateSingleton(x1, 1);
        var b = CreateSingleton(x2, 2);
        var r = PredictionContext.Merge(a, b, RootIsWildcard(), null);
        //		Console.Out.WriteLine(toDOTString(r, rootIsWildcard()));
        var expecting =
            "digraph G {\n" +
            "rankdir=LR;\n" +
            "  s0[shape=record, label=\"<p0>|<p1>\"];\n" +
            "  s1[label=\"1\"];\n" +
            "  s2[label=\"*\"];\n" +
            "  s0:p0->s1[label=\"1\"];\n" +
            "  s0:p1->s1[label=\"2\"];\n" +
            "  s1->s2[label=\"9\"];\n" +
            "}\n";
        Assert.AreEqual(expecting, ToDOTString(r, RootIsWildcard()));
    }

    [TestMethod]
    public void TestAxBy()
    {
        var a = CreateSingleton(X(), 1);
        var b = CreateSingleton(Y(), 2);
        var r = PredictionContext.Merge(a, b, RootIsWildcard(), null);
        //		Console.Out.WriteLine(toDOTString(r, rootIsWildcard()));
        var expecting =
            "digraph G {\n" +
            "rankdir=LR;\n" +
            "  s0[shape=record, label=\"<p0>|<p1>\"];\n" +
            "  s2[label=\"2\"];\n" +
            "  s3[label=\"*\"];\n" +
            "  s1[label=\"1\"];\n" +
            "  s0:p0->s1[label=\"1\"];\n" +
            "  s0:p1->s2[label=\"2\"];\n" +
            "  s2->s3[label=\"10\"];\n" +
            "  s1->s3[label=\"9\"];\n" +
            "}\n";
        Assert.AreEqual(expecting, ToDOTString(r, RootIsWildcard()));
    }

    [TestMethod]
    public void TestAdollarBx()
    {
        var x2 = X();
        var _a = A();
        var _b = CreateSingleton(x2, 2);
        var r = PredictionContext.Merge(_a, _b, RootIsWildcard(), null);
        //		Console.Out.WriteLine(toDOTString(r, rootIsWildcard()));
        var expecting =
            "digraph G {\n" +
            "rankdir=LR;\n" +
            "  s0[shape=record, label=\"<p0>|<p1>\"];\n" +
            "  s2[label=\"2\"];\n" +
            "  s1[label=\"*\"];\n" +
            "  s0:p0->s1[label=\"1\"];\n" +
            "  s0:p1->s2[label=\"2\"];\n" +
            "  s2->s1[label=\"9\"];\n" +
            "}\n";
        Assert.AreEqual(expecting, ToDOTString(r, RootIsWildcard()));
    }

    [TestMethod]
    public void TestAdollarBxFullctx()
    {
        var x2 = X();
        var _a = A();
        var b = CreateSingleton(x2, 2);
        var r = PredictionContext.Merge(_a, b, FullCtx(), null);
        //		Console.Out.WriteLine(toDOTString(r, fullCtx()));
        var expecting =
            "digraph G {\n" +
            "rankdir=LR;\n" +
            "  s0[shape=record, label=\"<p0>|<p1>\"];\n" +
            "  s2[label=\"2\"];\n" +
            "  s1[label=\"$\"];\n" +
            "  s0:p0->s1[label=\"1\"];\n" +
            "  s0:p1->s2[label=\"2\"];\n" +
            "  s2->s1[label=\"9\"];\n" +
            "}\n";
        Assert.AreEqual(expecting, ToDOTString(r, FullCtx()));
    }

    //@Disabled("Known inefficiency but deferring resolving the issue for now")
    [TestMethod]
    public void TestAexBfx()
    {
        // TJP: this is inefficient as it leaves the top x nodes unmerged.
        var x1 = X();
        var x2 = X();
        var e = CreateSingleton(x1, 5);
        var f = CreateSingleton(x2, 6);
        var a = CreateSingleton(e, 1);
        var b = CreateSingleton(f, 2);
        var r = PredictionContext.Merge(a, b, RootIsWildcard(), null);
        //		Console.Out.WriteLine(toDOTString(r, rootIsWildcard()));
        var expecting =
            "digraph G {\n" +
            "rankdir=LR;\n" +
            "  s0[shape=record, label=\"<p0>|<p1>\"];\n" +
            "  s2[label=\"2\"];\n" +
            "  s3[label=\"3\"];\n" +
            "  s4[label=\"*\"];\n" +
            "  s1[label=\"1\"];\n" +
            "  s0:p0->s1[label=\"1\"];\n" +
            "  s0:p1->s2[label=\"2\"];\n" +
            "  s2->s3[label=\"6\"];\n" +
            "  s3->s4[label=\"9\"];\n" +
            "  s1->s3[label=\"5\"];\n" +
            "}\n";
        Assert.AreEqual(expecting, ToDOTString(r, RootIsWildcard()));
    }

    // Array merges

    [TestMethod]
    public void TestAdollarAdollarFullctx()
    {
        var A1 = Array(EmptyPredictionContext.Instance);
        var A2 = Array(EmptyPredictionContext.Instance);
        var r = PredictionContext.Merge(A1, A2, FullCtx(), null);
        //		Console.Out.WriteLine(toDOTString(r, fullCtx()));
        var expecting =
            "digraph G {\n" +
            "rankdir=LR;\n" +
            "  s0[label=\"$\"];\n" +
            "}\n";
        Assert.AreEqual(expecting, ToDOTString(r, FullCtx()));
    }

    [TestMethod]
    public void TestAabAc()
    { // a,b + c
        var _a = A();
        var _b = B();
        var _c = C();
        var A1 = Array(_a, _b);
        var A2 = Array(_c);
        var r = PredictionContext.Merge(A1, A2, RootIsWildcard(), null);
        //		Console.Out.WriteLine(toDOTString(r, rootIsWildcard()));
        var expecting =
            "digraph G {\n" +
            "rankdir=LR;\n" +
            "  s0[shape=record, label=\"<p0>|<p1>|<p2>\"];\n" +
            "  s1[label=\"*\"];\n" +
            "  s0:p0->s1[label=\"1\"];\n" +
            "  s0:p1->s1[label=\"2\"];\n" +
            "  s0:p2->s1[label=\"3\"];\n" +
            "}\n";
        Assert.AreEqual(expecting, ToDOTString(r, RootIsWildcard()));
    }

    [TestMethod]
    public void TestAaAa()
    {
        var a1 = A();
        var a2 = A();
        var A1 = Array(a1);
        var A2 = Array(a2);
        var r = PredictionContext.Merge(A1, A2, RootIsWildcard(), null);
        //		Console.Out.WriteLine(toDOTString(r, rootIsWildcard()));
        var expecting =
            "digraph G {\n" +
            "rankdir=LR;\n" +
            "  s0[label=\"0\"];\n" +
            "  s1[label=\"*\"];\n" +
            "  s0->s1[label=\"1\"];\n" +
            "}\n";
        Assert.AreEqual(expecting, ToDOTString(r, RootIsWildcard()));
    }

    [TestMethod]
    public void TestAaAbc()
    { // a + b,c
        var _a = A();
        var _b = B();
        var _c = C();
        var A1 = Array(_a);
        var A2 = Array(_b, _c);
        var r = PredictionContext.Merge(A1, A2, RootIsWildcard(), null);
        //		Console.Out.WriteLine(toDOTString(r, rootIsWildcard()));
        var expecting =
            "digraph G {\n" +
            "rankdir=LR;\n" +
            "  s0[shape=record, label=\"<p0>|<p1>|<p2>\"];\n" +
            "  s1[label=\"*\"];\n" +
            "  s0:p0->s1[label=\"1\"];\n" +
            "  s0:p1->s1[label=\"2\"];\n" +
            "  s0:p2->s1[label=\"3\"];\n" +
            "}\n";
        Assert.AreEqual(expecting, ToDOTString(r, RootIsWildcard()));
    }

    [TestMethod]
    public void TestAacAb()
    { // a,c + b
        var _a = A();
        var _b = B();
        var _c = C();
        var A1 = Array(_a, _c);
        var A2 = Array(_b);
        var r = PredictionContext.Merge(A1, A2, RootIsWildcard(), null);
        //		Console.Out.WriteLine(toDOTString(r, rootIsWildcard()));
        var expecting =
            "digraph G {\n" +
            "rankdir=LR;\n" +
            "  s0[shape=record, label=\"<p0>|<p1>|<p2>\"];\n" +
            "  s1[label=\"*\"];\n" +
            "  s0:p0->s1[label=\"1\"];\n" +
            "  s0:p1->s1[label=\"2\"];\n" +
            "  s0:p2->s1[label=\"3\"];\n" +
            "}\n";
        Assert.AreEqual(expecting, ToDOTString(r, RootIsWildcard()));
    }

    [TestMethod]
    public void TestAabAa()
    { // a,b + a
        var A1 = Array(A(), B());
        var A2 = Array(A());
        var r = PredictionContext.Merge(A1, A2, RootIsWildcard(), null);
        //		Console.Out.WriteLine(toDOTString(r, rootIsWildcard()));
        var expecting =
            "digraph G {\n" +
            "rankdir=LR;\n" +
            "  s0[shape=record, label=\"<p0>|<p1>\"];\n" +
            "  s1[label=\"*\"];\n" +
            "  s0:p0->s1[label=\"1\"];\n" +
            "  s0:p1->s1[label=\"2\"];\n" +
            "}\n";
        Assert.AreEqual(expecting, ToDOTString(r, RootIsWildcard()));
    }

    [TestMethod]
    public void TestAabAb()
    { // a,b + b
        var A1 = Array(A(), B());
        var A2 = Array(B());
        var r = PredictionContext.Merge(A1, A2, RootIsWildcard(), null);
        //		Console.Out.WriteLine(toDOTString(r, rootIsWildcard()));
        var expecting =
            "digraph G {\n" +
            "rankdir=LR;\n" +
            "  s0[shape=record, label=\"<p0>|<p1>\"];\n" +
            "  s1[label=\"*\"];\n" +
            "  s0:p0->s1[label=\"1\"];\n" +
            "  s0:p1->s1[label=\"2\"];\n" +
            "}\n";
        Assert.AreEqual(expecting, ToDOTString(r, RootIsWildcard()));
    }

    [TestMethod]
    public void TestAaxAby()
    { // ax + by but in arrays
        var a = CreateSingleton(X(), 1);
        var b = CreateSingleton(Y(), 2);
        var A1 = Array(a);
        var A2 = Array(b);
        var r = PredictionContext.Merge(A1, A2, RootIsWildcard(), null);
        //		Console.Out.WriteLine(toDOTString(r, rootIsWildcard()));
        var expecting =
            "digraph G {\n" +
            "rankdir=LR;\n" +
            "  s0[shape=record, label=\"<p0>|<p1>\"];\n" +
            "  s2[label=\"2\"];\n" +
            "  s3[label=\"*\"];\n" +
            "  s1[label=\"1\"];\n" +
            "  s0:p0->s1[label=\"1\"];\n" +
            "  s0:p1->s2[label=\"2\"];\n" +
            "  s2->s3[label=\"10\"];\n" +
            "  s1->s3[label=\"9\"];\n" +
            "}\n";
        Assert.AreEqual(expecting, ToDOTString(r, RootIsWildcard()));
    }

    [TestMethod]
    public void TestAaxAay()
    { // ax + ay -> merged singleton a, array parent
        var a1 = CreateSingleton(X(), 1);
        var a2 = CreateSingleton(Y(), 1);
        var A1 = Array(a1);
        var A2 = Array(a2);
        var r = PredictionContext.Merge(A1, A2, RootIsWildcard(), null);
        //		Console.Out.WriteLine(toDOTString(r, rootIsWildcard()));
        var expecting =
            "digraph G {\n" +
            "rankdir=LR;\n" +
            "  s0[label=\"0\"];\n" +
            "  s1[shape=record, label=\"<p0>|<p1>\"];\n" +
            "  s2[label=\"*\"];\n" +
            "  s0->s1[label=\"1\"];\n" +
            "  s1:p0->s2[label=\"9\"];\n" +
            "  s1:p1->s2[label=\"10\"];\n" +
            "}\n";
        Assert.AreEqual(expecting, ToDOTString(r, RootIsWildcard()));
    }

    [TestMethod]
    public void TestAaxcAayd()
    { // ax,c + ay,d -> merged a, array parent
        var a1 = CreateSingleton(X(), 1);
        var a2 = CreateSingleton(Y(), 1);
        var A1 = Array(a1, C());
        var A2 = Array(a2, D());
        var r = PredictionContext.Merge(A1, A2, RootIsWildcard(), null);
        //		Console.Out.WriteLine(toDOTString(r, rootIsWildcard()));
        var expecting =
            "digraph G {\n" +
            "rankdir=LR;\n" +
            "  s0[shape=record, label=\"<p0>|<p1>|<p2>\"];\n" +
            "  s2[label=\"*\"];\n" +
            "  s1[shape=record, label=\"<p0>|<p1>\"];\n" +
            "  s0:p0->s1[label=\"1\"];\n" +
            "  s0:p1->s2[label=\"3\"];\n" +
            "  s0:p2->s2[label=\"4\"];\n" +
            "  s1:p0->s2[label=\"9\"];\n" +
            "  s1:p1->s2[label=\"10\"];\n" +
            "}\n";
        Assert.AreEqual(expecting, ToDOTString(r, RootIsWildcard()));
    }

    [TestMethod]
    public void TestAaubvAcwdx()
    { // au,bv + cw,dx -> [a,b,c,d]->[u,v,w,x]
        var a = CreateSingleton(U(), 1);
        var b = CreateSingleton(V(), 2);
        var c = CreateSingleton(W(), 3);
        var d = CreateSingleton(X(), 4);
        var A1 = Array(a, b);
        var A2 = Array(c, d);
        var r = PredictionContext.Merge(A1, A2, RootIsWildcard(), null);
        //		Console.Out.WriteLine(toDOTString(r, rootIsWildcard()));
        var expecting =
            "digraph G {\n" +
            "rankdir=LR;\n" +
            "  s0[shape=record, label=\"<p0>|<p1>|<p2>|<p3>\"];\n" +
            "  s4[label=\"4\"];\n" +
            "  s5[label=\"*\"];\n" +
            "  s3[label=\"3\"];\n" +
            "  s2[label=\"2\"];\n" +
            "  s1[label=\"1\"];\n" +
            "  s0:p0->s1[label=\"1\"];\n" +
            "  s0:p1->s2[label=\"2\"];\n" +
            "  s0:p2->s3[label=\"3\"];\n" +
            "  s0:p3->s4[label=\"4\"];\n" +
            "  s4->s5[label=\"9\"];\n" +
            "  s3->s5[label=\"8\"];\n" +
            "  s2->s5[label=\"7\"];\n" +
            "  s1->s5[label=\"6\"];\n" +
            "}\n";
        Assert.AreEqual(expecting, ToDOTString(r, RootIsWildcard()));
    }

    [TestMethod]
    public void TestAaubvAbvdx()
    { // au,bv + bv,dx -> [a,b,d]->[u,v,x]
        var a = CreateSingleton(U(), 1);
        var b1 = CreateSingleton(V(), 2);
        var b2 = CreateSingleton(V(), 2);
        var d = CreateSingleton(X(), 4);
        var A1 = Array(a, b1);
        var A2 = Array(b2, d);
        var r = PredictionContext.Merge(A1, A2, RootIsWildcard(), null);
        //		Console.Out.WriteLine(toDOTString(r, rootIsWildcard()));
        var expecting =
            "digraph G {\n" +
            "rankdir=LR;\n" +
            "  s0[shape=record, label=\"<p0>|<p1>|<p2>\"];\n" +
            "  s3[label=\"3\"];\n" +
            "  s4[label=\"*\"];\n" +
            "  s2[label=\"2\"];\n" +
            "  s1[label=\"1\"];\n" +
            "  s0:p0->s1[label=\"1\"];\n" +
            "  s0:p1->s2[label=\"2\"];\n" +
            "  s0:p2->s3[label=\"4\"];\n" +
            "  s3->s4[label=\"9\"];\n" +
            "  s2->s4[label=\"7\"];\n" +
            "  s1->s4[label=\"6\"];\n" +
            "}\n";
        Assert.AreEqual(expecting, ToDOTString(r, RootIsWildcard()));
    }

    [TestMethod]
    public void TestAaubvAbwdx()
    { // au,bv + bw,dx -> [a,b,d]->[u,[v,w],x]
        var a = CreateSingleton(U(), 1);
        var b1 = CreateSingleton(V(), 2);
        var b2 = CreateSingleton(W(), 2);
        var d = CreateSingleton(X(), 4);
        var A1 = Array(a, b1);
        var A2 = Array(b2, d);
        var r = PredictionContext.Merge(A1, A2, RootIsWildcard(), null);
        //		Console.Out.WriteLine(toDOTString(r, rootIsWildcard()));
        var expecting =
            "digraph G {\n" +
            "rankdir=LR;\n" +
            "  s0[shape=record, label=\"<p0>|<p1>|<p2>\"];\n" +
            "  s3[label=\"3\"];\n" +
            "  s4[label=\"*\"];\n" +
            "  s2[shape=record, label=\"<p0>|<p1>\"];\n" +
            "  s1[label=\"1\"];\n" +
            "  s0:p0->s1[label=\"1\"];\n" +
            "  s0:p1->s2[label=\"2\"];\n" +
            "  s0:p2->s3[label=\"4\"];\n" +
            "  s3->s4[label=\"9\"];\n" +
            "  s2:p0->s4[label=\"7\"];\n" +
            "  s2:p1->s4[label=\"8\"];\n" +
            "  s1->s4[label=\"6\"];\n" +
            "}\n";
        Assert.AreEqual(expecting, ToDOTString(r, RootIsWildcard()));
    }

    [TestMethod]
    public void TestAaubvAbvdu()
    { // au,bv + bv,du -> [a,b,d]->[u,v,u]; u,v shared
        var a = CreateSingleton(U(), 1);
        var b1 = CreateSingleton(V(), 2);
        var b2 = CreateSingleton(V(), 2);
        var d = CreateSingleton(U(), 4);
        var A1 = Array(a, b1);
        var A2 = Array(b2, d);
        var r = PredictionContext.Merge(A1, A2, RootIsWildcard(), null);
        //		Console.Out.WriteLine(toDOTString(r, rootIsWildcard()));
        var expecting =
            "digraph G {\n" +
            "rankdir=LR;\n" +
            "  s0[shape=record, label=\"<p0>|<p1>|<p2>\"];\n" +
            "  s2[label=\"2\"];\n" +
            "  s3[label=\"*\"];\n" +
            "  s1[label=\"1\"];\n" +
            "  s0:p0->s1[label=\"1\"];\n" +
            "  s0:p1->s2[label=\"2\"];\n" +
            "  s0:p2->s1[label=\"4\"];\n" +
            "  s2->s3[label=\"7\"];\n" +
            "  s1->s3[label=\"6\"];\n" +
            "}\n";
        Assert.AreEqual(expecting, ToDOTString(r, RootIsWildcard()));
    }

    [TestMethod]
    public void TestAaubuAcudu()
    { // au,bu + cu,du -> [a,b,c,d]->[u,u,u,u]
        var a = CreateSingleton(U(), 1);
        var b = CreateSingleton(U(), 2);
        var c = CreateSingleton(U(), 3);
        var d = CreateSingleton(U(), 4);
        var A1 = Array(a, b);
        var A2 = Array(c, d);
        var r = PredictionContext.Merge(A1, A2, RootIsWildcard(), null);
        //		Console.Out.WriteLine(toDOTString(r, rootIsWildcard()));
        var expecting =
            "digraph G {\n" +
            "rankdir=LR;\n" +
            "  s0[shape=record, label=\"<p0>|<p1>|<p2>|<p3>\"];\n" +
            "  s1[label=\"1\"];\n" +
            "  s2[label=\"*\"];\n" +
            "  s0:p0->s1[label=\"1\"];\n" +
            "  s0:p1->s1[label=\"2\"];\n" +
            "  s0:p2->s1[label=\"3\"];\n" +
            "  s0:p3->s1[label=\"4\"];\n" +
            "  s1->s2[label=\"6\"];\n" +
            "}\n";
        Assert.AreEqual(expecting, ToDOTString(r, RootIsWildcard()));
    }


    // ------------ SUPPORT -------------------------

    protected SingletonPredictionContext A()
    {
        return CreateSingleton(EmptyPredictionContext.Instance, 1);
    }

    private SingletonPredictionContext B()
    {
        return CreateSingleton(EmptyPredictionContext.Instance, 2);
    }

    private SingletonPredictionContext C()
    {
        return CreateSingleton(EmptyPredictionContext.Instance, 3);
    }

    private SingletonPredictionContext D()
    {
        return CreateSingleton(EmptyPredictionContext.Instance, 4);
    }

    private SingletonPredictionContext U()
    {
        return CreateSingleton(EmptyPredictionContext.Instance, 6);
    }

    private SingletonPredictionContext V()
    {
        return CreateSingleton(EmptyPredictionContext.Instance, 7);
    }

    private SingletonPredictionContext W()
    {
        return CreateSingleton(EmptyPredictionContext.Instance, 8);
    }

    private SingletonPredictionContext X()
    {
        return CreateSingleton(EmptyPredictionContext.Instance, 9);
    }

    private SingletonPredictionContext Y()
    {
        return CreateSingleton(EmptyPredictionContext.Instance, 10);
    }

    public static SingletonPredictionContext CreateSingleton(PredictionContext parent, int payload)
    {
        var a = SingletonPredictionContext.Create(parent, payload);
        return a;
    }

    public static ArrayPredictionContext Array(params SingletonPredictionContext[] nodes)
    {
        var parents = new PredictionContext[nodes.Length];
        var invokingStates = new int[nodes.Length];
        for (int i = 0; i < nodes.Length; i++)
        {
            parents[i] = nodes[i].parent;
            invokingStates[i] = nodes[i].returnState;
        }
        return new ArrayPredictionContext(parents, invokingStates);
    }

    private static string ToDOTString(PredictionContext context, bool rootIsWildcard)
    {
        var nodes = new StringBuilder();
        var edges = new StringBuilder();
        Dictionary<PredictionContext, PredictionContext> visited = new();
        Dictionary<PredictionContext, int> contextIds = new();
        var workList = new ArrayDeque<PredictionContext>();
        visited[context] = context;
        contextIds[context] = contextIds.Count;
        workList.Add(context);
        while (workList.Count > 0)
        {
            var current = workList.Pop();
            nodes.Append("  s").Append(contextIds[(current)]).Append('[');

            if (current.Count > 1)
            {
                nodes.Append("shape=record, ");
            }

            nodes.Append("label=\"");

            if (current.Count == 0)
            {
                nodes.Append(rootIsWildcard ? '*' : '$');
            }
            else if (current.Count > 1)
            {
                for (int i = 0; i < current.Count; i++)
                {
                    if (i > 0)
                    {
                        nodes.Append('|');
                    }

                    nodes.Append("<p").Append(i).Append('>');
                    if (current.GetReturnState(i) == PredictionContext.EMPTY_RETURN_STATE)
                    {
                        nodes.Append(rootIsWildcard ? '*' : '$');
                    }
                }
            }
            else
            {
                nodes.Append(contextIds[(current)]);
            }

            nodes.Append("\"];\n");

            if (current.Count == 0)
            {
                continue;
            }

            for (int i = 0; i < current.Count; i++)
            {
                if (current.GetReturnState(i) == PredictionContext.EMPTY_RETURN_STATE)
                {
                    continue;
                }
                var parent = current.GetParent(i);

                if (!visited.ContainsKey(parent))
                {
                    visited.Add(parent, parent);
                    contextIds.Add(parent, contextIds.Count);
                    workList.Push(parent);
                }
                //if (visited.put(current.getParent(i), current.getParent(i)) == null) {
                //	contextIds.put(current.getParent(i), contextIds.Cast);
                //	workList.push(current.getParent(i));
                //}

                edges.Append("  s").Append(contextIds[(current)]);
                if (current.Count > 1)
                {
                    edges.Append(":p").Append(i);
                }

                edges.Append("->");
                edges.Append('s').Append(contextIds[current.GetParent(i)]);
                edges.Append("[label=\"").Append(current.GetReturnState(i)).Append("\"]");
                edges.Append(";\n");
            }
        }

        var builder = new StringBuilder();
        builder.Append("digraph G {\n");
        builder.Append("rankdir=LR;\n");
        builder.Append(nodes);
        builder.Append(edges);
        builder.Append("}\n");
        return builder.ToString();
    }
}
