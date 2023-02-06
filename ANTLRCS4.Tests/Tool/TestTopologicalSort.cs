/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */
using org.antlr.v4.misc;

namespace org.antlr.v4.test.tool;

[TestClass]
/** Test topo sort in GraphNode. */
public class TestTopologicalSort
{
    [TestMethod]
    public void TestFairlyLargeGraph()
    {
        var g = new Graph<string>();
        g.AddEdge("C", "F");
        g.AddEdge("C", "G");
        g.AddEdge("C", "A");
        g.AddEdge("C", "B");
        g.AddEdge("A", "D");
        g.AddEdge("A", "E");
        g.AddEdge("B", "E");
        g.AddEdge("D", "E");
        g.AddEdge("D", "F");
        g.AddEdge("F", "H");
        g.AddEdge("E", "F");

        var expecting = "[H, F, G, E, D, A, B, C]";
        var nodes = g.Sort();
        var result = nodes.ToString();
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    public void TestCyclicGraph()
    {
        var g = new Graph<string>();
        g.AddEdge("A", "B");
        g.AddEdge("B", "C");
        g.AddEdge("C", "A");
        g.AddEdge("C", "D");

        var expecting = "[D, C, B, A]";
        var nodes = g.Sort();
        var result = nodes.ToString();
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    public void TestRepeatedEdges()
    {
        var g = new Graph<string>();
        g.AddEdge("A", "B");
        g.AddEdge("B", "C");
        g.AddEdge("A", "B"); // dup
        g.AddEdge("C", "D");

        var expecting = "[D, C, B, A]";
        var nodes = g.Sort();
        var result = nodes.ToString();
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    public void TestSimpleTokenDependence()
    {
        var g = new Graph<string>();
        g.AddEdge("Java.g4", "MyJava.tokens"); // Java feeds off manual token file
        g.AddEdge("Java.tokens", "Java.g4");
        g.AddEdge("Def.g4", "Java.tokens");    // walkers feed off generated tokens
        g.AddEdge("Ref.g4", "Java.tokens");

        var expecting = "[MyJava.tokens, Java.g4, Java.tokens, Def.g4, Ref.g4]";
        var nodes = g.Sort();
        var result = nodes.ToString();
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    public void TestParserLexerCombo()
    {
        var g = new Graph<string>();
        g.AddEdge("JavaLexer.tokens", "JavaLexer.g4");
        g.AddEdge("JavaParser.g4", "JavaLexer.tokens");
        g.AddEdge("Def.g4", "JavaLexer.tokens");
        g.AddEdge("Ref.g4", "JavaLexer.tokens");

        var expecting = "[JavaLexer.g4, JavaLexer.tokens, JavaParser.g4, Def.g4, Ref.g4]";
        var nodes = g.Sort();
        var result = nodes.ToString();
        Assert.AreEqual(expecting, result);
    }
}
