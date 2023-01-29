using org.antlr.v4.misc;
using org.antlr.v4.runtime;
using org.antlr.v4.tool.ast;

namespace org.antlr.v4.test.tool;

[TestClass]
public class TestUtils
{
    [TestMethod]
    public void testStripFileExtension()
    {
        Assert.IsNull(Utils.stripFileExtension(null));
        Assert.AreEqual("foo", Utils.stripFileExtension("foo"));
        Assert.AreEqual("foo", Utils.stripFileExtension("foo.txt"));
    }

    [TestMethod]
    public void testJoin()
    {
        Assert.AreEqual("foobbar",
            Utils.join(new String[] { "foo", "bar" }, "b"));
        Assert.AreEqual("foo,bar",
            Utils.join(new String[] { "foo", "bar" }, ","));
    }

    [TestMethod]
    public void testSortLinesInString()
    {
        Assert.AreEqual("bar\nbaz\nfoo\n",
            Utils.sortLinesInString("foo\nbar\nbaz"));
    }

    [TestMethod]
    public void testNodesToStrings()
    {
        List<GrammarAST> values = new();
        values.Add(new GrammarAST(Token.EOR_TOKEN_TYPE));
        values.Add(new GrammarAST(Token.DOWN));
        values.Add(new GrammarAST(Token.UP));

        Assert.IsNull(Utils.nodesToStrings(null));
        Assert.IsNotNull(Utils.nodesToStrings(values));
    }

    [TestMethod]
    public void testCapitalize()
    {
        Assert.AreEqual("Foo", Utils.capitalize("foo"));
    }

    [TestMethod]
    public void testDecapitalize()
    {
        Assert.AreEqual("fOO", Utils.decapitalize("FOO"));
    }
    public class UFA<T> : Utils.Func1<T,string>
    {
        //@Override
        public string exec(T arg1)
        {
            return "baz";
        }
    }

    [TestMethod]
    public void testSelect()
    {
        List<String> strings = new();
        strings.Add("foo");
        strings.Add("bar");

        Utils.Func1<String, String> func1 = new UFA();

        List<String> retval = new();
        retval.Add("baz");
        retval.Add("baz");

        Assert.AreEqual(retval, Utils.select(strings, func1));
        Assert.IsNull(Utils.select(null, null));
    }

    [TestMethod]
    public void testFind()
    {
        List<String> strings = new();
        strings.Add("foo");
        strings.Add("bar");
        Assert.AreEqual("foo", Utils.find(strings, typeof(String)));

        Assert.IsNull(Utils.find(new List<string>(), typeof(String)));
    }

    public class UFB<T> : Utils.Filter<T>
    {
        //@Override
        public bool select(T o)
        {
            return true;
        }
    }
    [TestMethod]
    public void testIndexOf()
    {
        List<String> strings = new();
        strings.Add("foo");
        strings.Add("bar");
        Utils.Filter filter = new UFB();
        Assert.AreEqual(0, Utils.indexOf(strings, filter));
        Assert.AreEqual(-1, Utils.indexOf(new(), null));
    }

    public class UFC<T> : Utils.Filter<T>
    {
        //@Override
        public bool select(T o)
        {
            return true;
        }
    }
    [TestMethod]
    public void testLastIndexOf()
    {
        List<String> strings = new();
        strings.Add("foo");
        strings.Add("bar");
        Utils.Filter<string> filter = new UFC();
        Assert.AreEqual(1, Utils.lastIndexOf(strings, filter));
        Assert.AreEqual(-1, Utils.lastIndexOf(new(), null));
    }

    [TestMethod]
    public void testSetSize()
    {
        List<String> strings = new();
        strings.Add("foo");
        strings.Add("bar");
        strings.Add("baz");
        Assert.AreEqual(3, strings.Count);

        Utils.setSize(strings, 2);
        Assert.AreEqual(2, strings.Count);

        Utils.setSize(strings, 4);
        Assert.AreEqual(4, strings.Count);
    }
}
