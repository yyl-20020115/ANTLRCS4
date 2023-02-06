/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime.dfa;
using org.antlr.v4.unicode;

namespace org.antlr.v4.test.tool;
[TestClass]
public class TestUnicodeData {
	[TestMethod]
	public void testUnicodeGeneralCategoriesLatin() {
		Assert.IsTrue(UnicodeData.getPropertyCodePoints("Lu").Contains('X'));
		Assert.IsFalse(UnicodeData.getPropertyCodePoints("Lu").Contains('x'));
		Assert.IsTrue(UnicodeData.getPropertyCodePoints("Ll").Contains('x'));
		Assert.IsFalse(UnicodeData.getPropertyCodePoints("Ll").Contains('X'));
		Assert.IsTrue(UnicodeData.getPropertyCodePoints("L").Contains('X'));
		Assert.IsTrue(UnicodeData.getPropertyCodePoints("L").Contains('x'));
		Assert.IsTrue(UnicodeData.getPropertyCodePoints("N").Contains('0'));
		Assert.IsTrue(UnicodeData.getPropertyCodePoints("Z").Contains(' '));
	}

	[TestMethod]
	public void testUnicodeGeneralCategoriesBMP() {
		Assert.IsTrue(UnicodeData.getPropertyCodePoints("Lu").Contains('\u1E3A'));
		Assert.IsFalse(UnicodeData.getPropertyCodePoints("Lu").Contains('\u1E3B'));
		Assert.IsTrue(UnicodeData.getPropertyCodePoints("Ll").Contains('\u1E3B'));
		Assert.IsFalse(UnicodeData.getPropertyCodePoints("Ll").Contains('\u1E3A'));
		Assert.IsTrue(UnicodeData.getPropertyCodePoints("L").Contains('\u1E3A'));
		Assert.IsTrue(UnicodeData.getPropertyCodePoints("L").Contains('\u1E3B'));
		Assert.IsTrue(UnicodeData.getPropertyCodePoints("N").Contains('\u1BB0'));
		Assert.IsFalse(UnicodeData.getPropertyCodePoints("N").Contains('\u1E3A'));
		Assert.IsTrue(UnicodeData.getPropertyCodePoints("Z").Contains('\u2028'));
		Assert.IsFalse(UnicodeData.getPropertyCodePoints("Z").Contains('\u1E3A'));
	}

	[TestMethod]
	public void testUnicodeGeneralCategoriesSMP() {
		Assert.IsTrue(UnicodeData.getPropertyCodePoints("Lu").Contains(0x1D5D4));
		Assert.IsFalse(UnicodeData.getPropertyCodePoints("Lu").Contains(0x1D770));
		Assert.IsTrue(UnicodeData.getPropertyCodePoints("Ll").Contains(0x1D770));
		Assert.IsFalse(UnicodeData.getPropertyCodePoints("Ll").Contains(0x1D5D4));
		Assert.IsTrue(UnicodeData.getPropertyCodePoints("L").Contains(0x1D5D4));
		Assert.IsTrue(UnicodeData.getPropertyCodePoints("L").Contains(0x1D770));
		Assert.IsTrue(UnicodeData.getPropertyCodePoints("N").Contains(0x11C50));
		Assert.IsFalse(UnicodeData.getPropertyCodePoints("N").Contains(0x1D5D4));
	}

	[TestMethod]
	public void testUnicodeCategoryAliases() {
		Assert.IsTrue(UnicodeData.getPropertyCodePoints("Lowercase_Letter").Contains('x'));
		Assert.IsFalse(UnicodeData.getPropertyCodePoints("Lowercase_Letter").Contains('X'));
		Assert.IsTrue(UnicodeData.getPropertyCodePoints("Letter").Contains('x'));
		Assert.IsFalse(UnicodeData.getPropertyCodePoints("Letter").Contains('0'));
		Assert.IsTrue(UnicodeData.getPropertyCodePoints("Enclosing_Mark").Contains(0x20E2));
		Assert.IsFalse(UnicodeData.getPropertyCodePoints("Enclosing_Mark").Contains('x'));
	}

	[TestMethod]
	public void testUnicodeBinaryProperties() {
		Assert.IsTrue(UnicodeData.getPropertyCodePoints("Emoji").Contains(0x1F4A9));
		Assert.IsFalse(UnicodeData.getPropertyCodePoints("Emoji").Contains('X'));
		Assert.IsTrue(UnicodeData.getPropertyCodePoints("alnum").Contains('9'));
		Assert.IsFalse(UnicodeData.getPropertyCodePoints("alnum").Contains(0x1F4A9));
		Assert.IsTrue(UnicodeData.getPropertyCodePoints("Dash").Contains('-'));
		Assert.IsTrue(UnicodeData.getPropertyCodePoints("Hex").Contains('D'));
		Assert.IsFalse(UnicodeData.getPropertyCodePoints("Hex").Contains('Q'));
	}

	[TestMethod]
	public void testUnicodeBinaryPropertyAliases() {
		Assert.IsTrue(UnicodeData.getPropertyCodePoints("Ideo").Contains('\u611B'));
		Assert.IsFalse(UnicodeData.getPropertyCodePoints("Ideo").Contains('X'));
		Assert.IsTrue(UnicodeData.getPropertyCodePoints("Soft_Dotted").Contains('\u0456'));
		Assert.IsFalse(UnicodeData.getPropertyCodePoints("Soft_Dotted").Contains('X'));
		Assert.IsTrue(UnicodeData.getPropertyCodePoints("Noncharacter_Code_Point").Contains('\uFFFF'));
		Assert.IsFalse(UnicodeData.getPropertyCodePoints("Noncharacter_Code_Point").Contains('X'));
	}

	[TestMethod]
	public void testUnicodeScripts() {
		Assert.IsTrue(UnicodeData.getPropertyCodePoints("Zyyy").Contains('0'));
		Assert.IsTrue(UnicodeData.getPropertyCodePoints("Latn").Contains('X'));
		Assert.IsTrue(UnicodeData.getPropertyCodePoints("Hani").Contains(0x4E04));
		Assert.IsTrue(UnicodeData.getPropertyCodePoints("Cyrl").Contains(0x0404));
	}

	[TestMethod]
	public void testUnicodeScriptEquals() {
		Assert.IsTrue(UnicodeData.getPropertyCodePoints("Script=Zyyy").Contains('0'));
		Assert.IsTrue(UnicodeData.getPropertyCodePoints("Script=Latn").Contains('X'));
		Assert.IsTrue(UnicodeData.getPropertyCodePoints("Script=Hani").Contains(0x4E04));
		Assert.IsTrue(UnicodeData.getPropertyCodePoints("Script=Cyrl").Contains(0x0404));
	}

	[TestMethod]
	public void testUnicodeScriptAliases() {
		Assert.IsTrue(UnicodeData.getPropertyCodePoints("Common").Contains('0'));
		Assert.IsTrue(UnicodeData.getPropertyCodePoints("Latin").Contains('X'));
		Assert.IsTrue(UnicodeData.getPropertyCodePoints("Han").Contains(0x4E04));
		Assert.IsTrue(UnicodeData.getPropertyCodePoints("Cyrillic").Contains(0x0404));
	}

	[TestMethod]
	public void testUnicodeBlocks() {
		Assert.IsTrue(UnicodeData.getPropertyCodePoints("InASCII").Contains('0'));
		Assert.IsTrue(UnicodeData.getPropertyCodePoints("InCJK").Contains(0x4E04));
		Assert.IsTrue(UnicodeData.getPropertyCodePoints("InCyrillic").Contains(0x0404));
		Assert.IsTrue(UnicodeData.getPropertyCodePoints("InMisc_Pictographs").Contains(0x1F4A9));
	}

	[TestMethod]
	public void testUnicodeBlockEquals() {
		Assert.IsTrue(UnicodeData.getPropertyCodePoints("Block=ASCII").Contains('0'));
		Assert.IsTrue(UnicodeData.getPropertyCodePoints("Block=CJK").Contains(0x4E04));
		Assert.IsTrue(UnicodeData.getPropertyCodePoints("Block=Cyrillic").Contains(0x0404));
		Assert.IsTrue(UnicodeData.getPropertyCodePoints("Block=Misc_Pictographs").Contains(0x1F4A9));
	}

	[TestMethod]
	public void testUnicodeBlockAliases() {
		Assert.IsTrue(UnicodeData.getPropertyCodePoints("InBasic_Latin").Contains('0'));
		Assert.IsTrue(UnicodeData.getPropertyCodePoints("InMiscellaneous_Mathematical_Symbols_B").Contains(0x29BE));
	}

	[TestMethod]
	public void testEnumeratedPropertyEquals() {
		Assert.IsFalse(
				UnicodeData.getPropertyCodePoints("Grapheme_Cluster_Break=E_Base").Contains(0x1F47E),
				"U+1F47E ALIEN MONSTER is not an emoji modifier");

		Assert.IsFalse(
				UnicodeData.getPropertyCodePoints("Grapheme_Cluster_Break=E_Base").Contains(0x1038),
				"U+1038 MYANMAR SIGN VISARGA is not a spacing mark");

		Assert.IsTrue(
				UnicodeData.getPropertyCodePoints("East_Asian_Width=Ambiguous").Contains(0x00A1),
				"U+00A1 INVERTED EXCLAMATION MARK has ambiguous East Asian Width");

		Assert.IsFalse(
				UnicodeData.getPropertyCodePoints("East_Asian_Width=Ambiguous").Contains(0x00A2),
				"U+00A2 CENT SIGN does not have ambiguous East Asian Width");
	}

        [TestMethod]
        public void extendedPictographic() {
		Assert.IsTrue(
				UnicodeData.getPropertyCodePoints("Extended_Pictographic").Contains(0x1F588),
				"U+1F588 BLACK PUSHPIN is in Extended Pictographic");
		Assert.IsFalse(
				UnicodeData.getPropertyCodePoints("Extended_Pictographic").Contains('0'),
				"0 is not in Extended Pictographic");
        }

        [TestMethod]
        public void emojiPresentation() {
		Assert.IsTrue(
				UnicodeData.getPropertyCodePoints("EmojiPresentation=EmojiDefault").Contains(0x1F4A9),
				"U+1F4A9 PILE OF POO is in EmojiPresentation=EmojiDefault");
		Assert.IsFalse(
				UnicodeData.getPropertyCodePoints("EmojiPresentation=EmojiDefault").Contains('0'),
				"0 is not in EmojiPresentation=EmojiDefault");
		Assert.IsFalse(
				UnicodeData.getPropertyCodePoints("EmojiPresentation=EmojiDefault").Contains('A'),
				"A is not in EmojiPresentation=EmojiDefault");
		Assert.IsFalse(
				UnicodeData.getPropertyCodePoints("EmojiPresentation=TextDefault").Contains(0x1F4A9),
				"U+1F4A9 PILE OF POO is not in EmojiPresentation=TextDefault");
		Assert.IsTrue(
				UnicodeData.getPropertyCodePoints("EmojiPresentation=TextDefault").Contains('0'),
				"0 is in EmojiPresentation=TextDefault");
		Assert.IsFalse(
				UnicodeData.getPropertyCodePoints("EmojiPresentation=TextDefault").Contains('A'),
				"A is not in EmojiPresentation=TextDefault");
        }

	[TestMethod]
	public void testPropertyCaseInsensitivity() {
		Assert.IsTrue(UnicodeData.getPropertyCodePoints("l").Contains('x'));
		Assert.IsFalse(UnicodeData.getPropertyCodePoints("l").Contains('0'));
		Assert.IsTrue(UnicodeData.getPropertyCodePoints("common").Contains('0'));
		Assert.IsTrue(UnicodeData.getPropertyCodePoints("Alnum").Contains('0'));
	}

	[TestMethod]
	public void testPropertyDashSameAsUnderscore() {
		Assert.IsTrue(UnicodeData.getPropertyCodePoints("InLatin-1").Contains('\u00F0'));
	}

	[TestMethod]
	public void modifyingUnicodeDataShouldThrow() {
		//IllegalStateException exception = assertThrows(IllegalStateException, () => UnicodeData.getPropertyCodePoints("L").add(0x12345));
		IllegalStateException exception = Assert.ThrowsException<IllegalStateException>(() => UnicodeData.getPropertyCodePoints("L").Add(0x12345));

        Assert.AreEqual("can't alter readonly IntervalSet", exception.Message);
	}
}
