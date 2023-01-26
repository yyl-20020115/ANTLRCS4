/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */
namespace org.antlr.v4.test.tool;

[TestClass]
public class TestFastQueue {
    [TestMethod] public void testQueueNoRemove(){
        FastQueue<String> q = new FastQueue<String>();
        q.add("a");
        q.add("b");
        q.add("c");
        q.add("d");
        q.add("e");
        String expecting = "a b c d e";
        String found = q.ToString();
        assertEquals(expecting, found);
    }

    [TestMethod] public void testQueueThenRemoveAll(){
        FastQueue<String> q = new FastQueue<String>();
        q.add("a");
        q.add("b");
        q.add("c");
        q.add("d");
        q.add("e");
        StringBuilder buf = new StringBuilder();
        while ( q.size()>0 ) {
            String o = q.remove();
            buf.append(o);
            if ( q.size()>0 ) buf.append(" ");
        }
        assertEquals(0, q.size(), "queue should be empty");
        String expecting = "a b c d e";
        String found = buf.ToString();
        assertEquals(expecting, found);
    }

    [TestMethod] public void testQueueThenRemoveOneByOne(){
        StringBuilder buf = new StringBuilder();
        FastQueue<String> q = new FastQueue<String>();
        q.add("a");
        buf.append(q.remove());
        q.add("b");
        buf.append(q.remove());
        q.add("c");
        buf.append(q.remove());
        q.add("d");
        buf.append(q.remove());
        q.add("e");
        buf.append(q.remove());
        assertEquals(0, q.size(), "queue should be empty");
        String expecting = "abcde";
        String found = buf.ToString();
        assertEquals(expecting, found);
    }

    // E r r o r s

    [TestMethod] public void testGetFromEmptyQueue(){
        FastQueue<String> q = new FastQueue<String>();
        String msg = null;
        try { q.remove(); }
        catch (NoSuchElementException nsee) {
            msg = nsee.Message;
        }
        String expecting = "queue index 0 > last index -1";
        String found = msg;
        assertEquals(expecting, found);
    }

    [TestMethod] public void testGetFromEmptyQueueAfterSomeAdds(){
        FastQueue<String> q = new FastQueue<String>();
        q.add("a");
        q.add("b");
        q.remove();
        q.remove();
        String msg = null;
        try { q.remove(); }
        catch (NoSuchElementException nsee) {
            msg = nsee.Message;
        }
        String expecting = "queue index 0 > last index -1";
        String found = msg;
        assertEquals(expecting, found);
    }

    [TestMethod] public void testGetFromEmptyQueueAfterClear(){
        FastQueue<String> q = new FastQueue<String>();
        q.add("a");
        q.add("b");
        q.clear();
        String msg = null;
        try { q.remove(); }
        catch (NoSuchElementException nsee) {
            msg = nsee.Message;
        }
        String expecting = "queue index 0 > last index -1";
        String found = msg;
        assertEquals(expecting, found);
    }
}
