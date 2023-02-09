using org.antlr.v4.misc;
using org.antlr.v4.runtime;
using org.antlr.v4.tool.ast;

namespace org.antlr.v4.test.tool;

[TestClass]
public class TestUtils
{
    [TestMethod]
    public void TestStripFileExtension()
    {
        Assert.IsNull(Utils.StripFileExtension(null));
        Assert.AreEqual("foo", Utils.StripFileExtension("foo"));
        Assert.AreEqual("foo", Utils.StripFileExtension("foo.txt"));
    }

    [TestMethod]
    public void TestJoin()
    {
        Assert.AreEqual("foobbar",
            Utils.Join(new string[] { "foo", "bar" }, "b"));
        Assert.AreEqual("foo,bar",
            Utils.Join(new string[] { "foo", "bar" }, ","));
    }

    [TestMethod]
    public void TestSortLinesInString()
    {
        Assert.AreEqual("bar\nbaz\nfoo\n",
            Utils.SortLinesInString("foo\nbar\nbaz"));
    }

    [TestMethod]
    public void TestNodesToStrings()
    {
        List<GrammarAST> values = new()
        {
            new GrammarAST(Token.EOR_TOKEN_TYPE),
            new GrammarAST(Token.DOWN),
            new GrammarAST(Token.UP)
        };

        Assert.IsNull(Utils.NodesToStrings(new List<GrammarAST>()));
        Assert.IsNotNull(Utils.NodesToStrings(values));
    }

    [TestMethod]
    public void TestCapitalize()
    {
        Assert.AreEqual("Foo", Utils.Capitalize("foo"));
    }

    [TestMethod]
    public void TestDecapitalize()
    {
        Assert.AreEqual("fOO", Utils.Decapitalize("FOO"));
    }
    public class UFA<T> : Utils.Func1<T,string>
    {
        //@Override
        public string Exec(T arg1) => "baz";
    }

    [TestMethod]
    public void TestSelect()
    {
        List<string> strings = new()
        {
            "foo",
            "bar"
        };

        var func1 = new UFA<string>();

        List<string> retval = new()
        {
            "baz",
            "baz"
        };

        Assert.AreEqual(retval, Utils.Select(strings, func1));
        Assert.IsNull(Utils.Select<string,string>(null, null));
    }

    [TestMethod]
    public void TestFind()
    {
        List<string> strings = new()
        {
            "foo",
            "bar"
        };
        Assert.AreEqual("foo", Utils.Find(strings, typeof(string)));

        Assert.IsNull(Utils.Find(new List<string>(), typeof(string)));
    }

    public class UFB<T> : Utils.Filter<T>
    {
        //@Override
        public bool Select(T o) => true;
    }
    [TestMethod]
    public void TestIndexOf()
    {
        List<string> strings = new()
        {
            "foo",
            "bar"
        };
        var filter = new UFB<string>();
        Assert.AreEqual(0, Utils.IndexOf(strings, filter));
        Assert.AreEqual(-1, Utils.IndexOf<string>(new(), null));
    }

    public class UFC<T> : Utils.Filter<T>
    {
        //@Override
        public bool Select(T o) => true;
    }
    [TestMethod]
    public void TestLastIndexOf()
    {
        List<string> strings = new()
        {
            "foo",
            "bar"
        };
        var filter = new UFC<string>();
        Assert.AreEqual(+1, Utils.LastIndexOf(strings, filter));
        Assert.AreEqual(-1, Utils.LastIndexOf<string>(new(), null));
    }

    [TestMethod]
    public void TestSetSize()
    {
        List<string> strings = new()
        {
            "foo",
            "bar",
            "baz"
        };
        Assert.AreEqual(3, strings.Count);

        Utils.SetSize(strings, 2);
        Assert.AreEqual(2, strings.Count);

        Utils.SetSize(strings, 4);
        Assert.AreEqual(4, strings.Count);
    }
}
