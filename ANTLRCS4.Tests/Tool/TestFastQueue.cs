/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */
using org.antlr.runtime.misc;
using org.antlr.v4.runtime.misc;
using System.Text;

namespace org.antlr.v4.test.tool;

[TestClass]
public class TestFastQueue
{
    [TestMethod]
    public void TestQueueNoRemove()
    {
        var q = new FastQueue<string>();
        q.Add("a");
        q.Add("b");
        q.Add("c");
        q.Add("d");
        q.Add("e");
        var expecting = "a b c d e";
        var found = q.ToString();
        Assert.AreEqual(expecting, found);
    }

    [TestMethod]
    public void TestQueueThenRemoveAll()
    {
        var q = new FastQueue<string>();
        q.Add("a");
        q.Add("b");
        q.Add("c");
        q.Add("d");
        q.Add("e");
        var buffer = new StringBuilder();
        while (q.Count > 0)
        {
            var o = q.Remove();
            buffer.Append(o);
            if (q.Count > 0) buffer.Append(" ");
        }
        Assert.AreEqual(0, q.Count, "queue should be empty");
        var expecting = "a b c d e";
        var found = buffer.ToString();
        Assert.AreEqual(expecting, found);
    }

    [TestMethod]
    public void TestQueueThenRemoveOneByOne()
    {
        var buffer = new StringBuilder();
        var q = new FastQueue<string>();
        q.Add("a");
        buffer.Append(q.Remove());
        q.Add("b");
        buffer.Append(q.Remove());
        q.Add("c");
        buffer.Append(q.Remove());
        q.Add("d");
        buffer.Append(q.Remove());
        q.Add("e");
        buffer.Append(q.Remove());
        Assert.AreEqual(0, q.Count, "queue should be empty");
        var expecting = "abcde";
        var found = buffer.ToString();
        Assert.AreEqual(expecting, found);
    }

    // E r r o r s

    [TestMethod]
    public void TestGetFromEmptyQueue()
    {
        var q = new FastQueue<string>();
        string msg = null;
        try { q.Remove(); }
        catch (NoSuchElementException nsee)
        {
            msg = nsee.Message;
        }
        var expecting = "queue index 0 > last index -1";
        var found = msg;
        Assert.AreEqual(expecting, found);
    }

    [TestMethod]
    public void TestGetFromEmptyQueueAfterSomeAdds()
    {
        var q = new FastQueue<string>();
        q.Add("a");
        q.Add("b");
        q.Remove();
        q.Remove();
        string msg = null;
        try { q.Remove(); }
        catch (NoSuchElementException nsee)
        {
            msg = nsee.Message;
        }
        var expecting = "queue index 0 > last index -1";
        var found = msg;
        Assert.AreEqual(expecting, found);
    }

    [TestMethod]
    public void TestGetFromEmptyQueueAfterClear()
    {
        var q = new FastQueue<string>();
        q.Add("a");
        q.Add("b");
        q.Clear();
        string msg = null;
        try { q.Remove(); }
        catch (NoSuchElementException nsee)
        {
            msg = nsee.Message;
        }
        var expecting = "queue index 0 > last index -1";
        var found = msg;
        Assert.AreEqual(expecting, found);
    }
}
