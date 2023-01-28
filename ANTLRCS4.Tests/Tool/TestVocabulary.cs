/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */
using org.antlr.v4.runtime;

namespace org.antlr.v4.test.tool;


/**
 *
 * @author Sam Harwell
 */
[TestClass]
public class TestVocabulary {
	[TestMethod]
	public void testEmptyVocabulary() {
		Assert.IsNotNull(VocabularyImpl.EMPTY_VOCABULARY);
		Assert.AreEqual("EOF", VocabularyImpl.EMPTY_VOCABULARY.getSymbolicName(Token.EOF));
		Assert.AreEqual("0", VocabularyImpl.EMPTY_VOCABULARY.getDisplayName(Token.INVALID_TYPE));
	}

	[TestMethod]
	public void testVocabularyFromTokenNames() {
		String[] tokenNames = {
			"<INVALID>",
			"TOKEN_REF", "RULE_REF", "'//'", "'/'", "'*'", "'!'", "ID", "STRING"
		};

		Vocabulary vocabulary = VocabularyImpl.fromTokenNames(tokenNames);
		Assert.IsNotNull(vocabulary);
		Assert.AreEqual("EOF", vocabulary.getSymbolicName(Token.EOF));
		for (int i = 0; i < tokenNames.Length; i++) {
			Assert.AreEqual(tokenNames[i], vocabulary.getDisplayName(i));

			if (tokenNames[i].StartsWith("'")) {
				Assert.AreEqual(tokenNames[i], vocabulary.getLiteralName(i));
				Assert.IsNull(vocabulary.getSymbolicName(i));
			}
			else if (char.IsUpper(tokenNames[i][(0)])) {
				Assert.IsNull(vocabulary.getLiteralName(i));
				Assert.AreEqual(tokenNames[i], vocabulary.getSymbolicName(i));
			}
			else {
				Assert.IsNull(vocabulary.getLiteralName(i));
				Assert.IsNull(vocabulary.getSymbolicName(i));
			}
		}
	}
}
