/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */
using org.antlr.v4.misc;

namespace org.antlr.v4.test.tool;

[TestClass]
/** Test topo sort in GraphNode. */
public class TestTopologicalSort {
    [TestMethod]
    public void testFairlyLargeGraph(){
        Graph<String> g = new Graph<String>();
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

        String expecting = "[H, F, G, E, D, A, B, C]";
        List<String> nodes = g.Sort();
        String result = nodes.ToString();
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    public void testCyclicGraph(){
        Graph<String> g = new Graph<String>();
        g.AddEdge("A", "B");
        g.AddEdge("B", "C");
        g.AddEdge("C", "A");
        g.AddEdge("C", "D");

        String expecting = "[D, C, B, A]";
        List<String> nodes = g.Sort();
        String result = nodes.ToString();
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    public void testRepeatedEdges(){
        Graph<String> g = new Graph<String>();
        g.AddEdge("A", "B");
        g.AddEdge("B", "C");
        g.AddEdge("A", "B"); // dup
        g.AddEdge("C", "D");

        String expecting = "[D, C, B, A]";
        List<String> nodes = g.Sort();
        String result = nodes.ToString();
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    public void testSimpleTokenDependence(){
        Graph<String> g = new Graph<String>();
        g.AddEdge("Java.g4", "MyJava.tokens"); // Java feeds off manual token file
        g.AddEdge("Java.tokens", "Java.g4");
        g.AddEdge("Def.g4", "Java.tokens");    // walkers feed off generated tokens
        g.AddEdge("Ref.g4", "Java.tokens");

        String expecting = "[MyJava.tokens, Java.g4, Java.tokens, Def.g4, Ref.g4]";
        List<String> nodes = g.Sort();
        String result = nodes.ToString();
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    public void testParserLexerCombo(){
        Graph<String> g = new Graph<String>();
        g.AddEdge("JavaLexer.tokens", "JavaLexer.g4");
        g.AddEdge("JavaParser.g4", "JavaLexer.tokens");
        g.AddEdge("Def.g4", "JavaLexer.tokens");
        g.AddEdge("Ref.g4", "JavaLexer.tokens");

        String expecting = "[JavaLexer.g4, JavaLexer.tokens, JavaParser.g4, Def.g4, Ref.g4]";
        List<String> nodes = g.Sort();
        String result = nodes.ToString();
        Assert.AreEqual(expecting, result);
    }
}
