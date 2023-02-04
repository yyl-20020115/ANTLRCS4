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
        Assert.IsNull(Utils.StripFileExtension(null));
        Assert.AreEqual("foo", Utils.StripFileExtension("foo"));
        Assert.AreEqual("foo", Utils.StripFileExtension("foo.txt"));
    }

    [TestMethod]
    public void testJoin()
    {
        Assert.AreEqual("foobbar",
            Utils.Join(new String[] { "foo", "bar" }, "b"));
        Assert.AreEqual("foo,bar",
            Utils.Join(new String[] { "foo", "bar" }, ","));
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

        Assert.IsNull(Utils.NodesToStrings(new List<GrammarAST>()));
        Assert.IsNotNull(Utils.NodesToStrings(values));
    }

    [TestMethod]
    public void testCapitalize()
    {
        Assert.AreEqual("Foo", Utils.Capitalize("foo"));
    }

    [TestMethod]
    public void testDecapitalize()
    {
        Assert.AreEqual("fOO", Utils.Decapitalize("FOO"));
    }
    public class UFA<T> : Utils.Func1<T,string>
    {
        //@Override
        public string Exec(T arg1)
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

        Utils.Func1<String, String> func1 = new UFA<string>();

        List<String> retval = new();
        retval.Add("baz");
        retval.Add("baz");

        Assert.AreEqual(retval, Utils.Select(strings, func1));
        Assert.IsNull(Utils.Select<string,string>(null, null));
    }

    [TestMethod]
    public void testFind()
    {
        List<String> strings = new();
        strings.Add("foo");
        strings.Add("bar");
        Assert.AreEqual("foo", Utils.Find(strings, typeof(String)));

        Assert.IsNull(Utils.Find(new List<string>(), typeof(String)));
    }

    public class UFB<T> : Utils.Filter<T>
    {
        //@Override
        public bool Select(T o)
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
        Utils.Filter<string> filter = new UFB<string>();
        Assert.AreEqual(0, Utils.IndexOf(strings, filter));
        Assert.AreEqual(-1, Utils.IndexOf<string>(new(), null));
    }

    public class UFC<T> : Utils.Filter<T>
    {
        //@Override
        public bool Select(T o)
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
        Utils.Filter<string> filter = new UFC<string>();
        Assert.AreEqual(1, Utils.LastIndexOf(strings, filter));
        Assert.AreEqual(-1, Utils.LastIndexOf<string>(new(), null));
    }

    [TestMethod]
    public void testSetSize()
    {
        List<String> strings = new();
        strings.Add("foo");
        strings.Add("bar");
        strings.Add("baz");
        Assert.AreEqual(3, strings.Count);

        Utils.SetSize(strings, 2);
        Assert.AreEqual(2, strings.Count);

        Utils.SetSize(strings, 4);
        Assert.AreEqual(4, strings.Count);
    }
}
