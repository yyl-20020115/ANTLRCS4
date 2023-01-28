using org.antlr.v4.misc;

namespace org.antlr.v4.test.tool;

[TestClass]
public class TestUtils {
	[TestMethod]
	public void testStripFileExtension() {
		assertNull(Utils.stripFileExtension(null));
		Assert.AreEqual("foo", Utils.stripFileExtension("foo"));
		Assert.AreEqual("foo", Utils.stripFileExtension("foo.txt"));
	}

	[TestMethod]
	public void testJoin() {
		Assert.AreEqual("foobbar",
			Utils.join(new String[]{"foo", "bar"}, "b"));
		Assert.AreEqual("foo,bar",
			Utils.join(new String[]{"foo", "bar"}, ","));
	}

	[TestMethod]
	public void testSortLinesInString() {
		Assert.AreEqual("bar\nbaz\nfoo\n",
			Utils.sortLinesInString("foo\nbar\nbaz"));
	}

	[TestMethod]
	public void testNodesToStrings() {
		ArrayList<GrammarAST> values = new ArrayList<>();
		values.add(new GrammarAST(Token.EOR_TOKEN_TYPE));
		values.add(new GrammarAST(Token.DOWN));
		values.add(new GrammarAST(Token.UP));

		assertNull(Utils.nodesToStrings(null));
		assertNotNull(Utils.nodesToStrings(values));
	}

	[TestMethod]
	public void testCapitalize() {
		Assert.AreEqual("Foo", Utils.capitalize("foo"));
	}

	[TestMethod]
	public void testDecapitalize() {
		Assert.AreEqual("fOO", Utils.decapitalize("FOO"));
	}

	[TestMethod]
	public void testSelect() {
		List<String> strings = new ();
		strings.add("foo");
		strings.add("bar");

		Utils.Func1<String, String> func1 = new Utils.Func1() {
			//@Override
			public Object exec(Object arg1) {
				return "baz";
			}
		};

		ArrayList<String> retval = new ArrayList<>();
		retval.add("baz");
		retval.add("baz");

		Assert.AreEqual(retval, Utils.select(strings, func1));
		assertNull(Utils.select(null, null));
	}

	[TestMethod]
	public void testFind() {
		ArrayList<String> strings = new ArrayList<>();
		strings.add("foo");
		strings.add("bar");
		Assert.AreEqual("foo", Utils.find(strings, String));

		assertNull(Utils.find(new ArrayList<>(), String));
	}

	[TestMethod]
	public void testIndexOf() {
		ArrayList<String> strings = new ArrayList<>();
		strings.add("foo");
		strings.add("bar");
		Utils.Filter filter = new Utils.Filter() {
			//@Override
			public bool select(Object o) {
				return true;
			}
		};
		Assert.AreEqual(0, Utils.indexOf(strings, filter));
		Assert.AreEqual(-1, Utils.indexOf(new ArrayList<>(), null));
	}

	[TestMethod]
	public void testLastIndexOf() {
		ArrayList<String> strings = new ArrayList<>();
		strings.add("foo");
		strings.add("bar");
		Utils.Filter filter = new Utils.Filter() {
			//@Override
			public bool select(Object o) {
				return true;
			}
		};
		Assert.AreEqual(1, Utils.lastIndexOf(strings, filter));
		Assert.AreEqual(-1, Utils.lastIndexOf(new ArrayList<>(), null));
	}

	[TestMethod]
	public void testSetSize() {
		ArrayList<String> strings = new ArrayList<>();
		strings.add("foo");
		strings.add("bar");
		strings.add("baz");
		Assert.AreEqual(3, strings.size());

		Utils.setSize(strings, 2);
		Assert.AreEqual(2, strings.size());

		Utils.setSize(strings, 4);
		Assert.AreEqual(4, strings.size());
	}
}
