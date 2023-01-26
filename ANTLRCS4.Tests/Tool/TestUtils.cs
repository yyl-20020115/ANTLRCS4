namespace org.antlr.v4.test.tool;

[TestClass]
public class TestUtils {
	[TestMethod]
	public void testStripFileExtension() {
		assertNull(Utils.stripFileExtension(null));
		assertEquals("foo", Utils.stripFileExtension("foo"));
		assertEquals("foo", Utils.stripFileExtension("foo.txt"));
	}

	[TestMethod]
	public void testJoin() {
		assertEquals("foobbar",
			Utils.join(new String[]{"foo", "bar"}, "b"));
		assertEquals("foo,bar",
			Utils.join(new String[]{"foo", "bar"}, ","));
	}

	[TestMethod]
	public void testSortLinesInString() {
		assertEquals("bar\nbaz\nfoo\n",
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
		assertEquals("Foo", Utils.capitalize("foo"));
	}

	[TestMethod]
	public void testDecapitalize() {
		assertEquals("fOO", Utils.decapitalize("FOO"));
	}

	[TestMethod]
	public void testSelect() {
		ArrayList<String> strings = new ArrayList<>();
		strings.add("foo");
		strings.add("bar");

		Utils.Func1<String, String> func1 = new Utils.Func1() {
			@Override
			public Object exec(Object arg1) {
				return "baz";
			}
		};

		ArrayList<String> retval = new ArrayList<>();
		retval.add("baz");
		retval.add("baz");

		assertEquals(retval, Utils.select(strings, func1));
		assertNull(Utils.select(null, null));
	}

	[TestMethod]
	public void testFind() {
		ArrayList<String> strings = new ArrayList<>();
		strings.add("foo");
		strings.add("bar");
		assertEquals("foo", Utils.find(strings, String));

		assertNull(Utils.find(new ArrayList<>(), String));
	}

	[TestMethod]
	public void testIndexOf() {
		ArrayList<String> strings = new ArrayList<>();
		strings.add("foo");
		strings.add("bar");
		Utils.Filter filter = new Utils.Filter() {
			@Override
			public boolean select(Object o) {
				return true;
			}
		};
		assertEquals(0, Utils.indexOf(strings, filter));
		assertEquals(-1, Utils.indexOf(new ArrayList<>(), null));
	}

	[TestMethod]
	public void testLastIndexOf() {
		ArrayList<String> strings = new ArrayList<>();
		strings.add("foo");
		strings.add("bar");
		Utils.Filter filter = new Utils.Filter() {
			@Override
			public boolean select(Object o) {
				return true;
			}
		};
		assertEquals(1, Utils.lastIndexOf(strings, filter));
		assertEquals(-1, Utils.lastIndexOf(new ArrayList<>(), null));
	}

	[TestMethod]
	public void testSetSize() {
		ArrayList<String> strings = new ArrayList<>();
		strings.add("foo");
		strings.add("bar");
		strings.add("baz");
		assertEquals(3, strings.size());

		Utils.setSize(strings, 2);
		assertEquals(2, strings.size());

		Utils.setSize(strings, 4);
		assertEquals(4, strings.size());
	}
}
