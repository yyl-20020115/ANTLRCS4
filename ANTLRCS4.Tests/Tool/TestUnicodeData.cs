/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.unicode;

namespace org.antlr.v4.test.tool;
[TestClass]
public class TestUnicodeData {
	[TestMethod]
	public void testUnicodeGeneralCategoriesLatin() {
		Assert.IsTrue(UnicodeData.getPropertyCodePoints("Lu").contains('X'));
		Assert.IsFalse(UnicodeData.getPropertyCodePoints("Lu").contains('x'));
		Assert.IsTrue(UnicodeData.getPropertyCodePoints("Ll").contains('x'));
		Assert.IsFalse(UnicodeData.getPropertyCodePoints("Ll").contains('X'));
		Assert.IsTrue(UnicodeData.getPropertyCodePoints("L").contains('X'));
		Assert.IsTrue(UnicodeData.getPropertyCodePoints("L").contains('x'));
		Assert.IsTrue(UnicodeData.getPropertyCodePoints("N").contains('0'));
		Assert.IsTrue(UnicodeData.getPropertyCodePoints("Z").contains(' '));
	}

	[TestMethod]
	public void testUnicodeGeneralCategoriesBMP() {
		Assert.IsTrue(UnicodeData.getPropertyCodePoints("Lu").contains('\u1E3A'));
		Assert.IsFalse(UnicodeData.getPropertyCodePoints("Lu").contains('\u1E3B'));
		Assert.IsTrue(UnicodeData.getPropertyCodePoints("Ll").contains('\u1E3B'));
		Assert.IsFalse(UnicodeData.getPropertyCodePoints("Ll").contains('\u1E3A'));
		Assert.IsTrue(UnicodeData.getPropertyCodePoints("L").contains('\u1E3A'));
		Assert.IsTrue(UnicodeData.getPropertyCodePoints("L").contains('\u1E3B'));
		Assert.IsTrue(UnicodeData.getPropertyCodePoints("N").contains('\u1BB0'));
		Assert.IsFalse(UnicodeData.getPropertyCodePoints("N").contains('\u1E3A'));
		Assert.IsTrue(UnicodeData.getPropertyCodePoints("Z").contains('\u2028'));
		Assert.IsFalse(UnicodeData.getPropertyCodePoints("Z").contains('\u1E3A'));
	}

	[TestMethod]
	public void testUnicodeGeneralCategoriesSMP() {
		Assert.IsTrue(UnicodeData.getPropertyCodePoints("Lu").contains(0x1D5D4));
		Assert.IsFalse(UnicodeData.getPropertyCodePoints("Lu").contains(0x1D770));
		Assert.IsTrue(UnicodeData.getPropertyCodePoints("Ll").contains(0x1D770));
		Assert.IsFalse(UnicodeData.getPropertyCodePoints("Ll").contains(0x1D5D4));
		Assert.IsTrue(UnicodeData.getPropertyCodePoints("L").contains(0x1D5D4));
		Assert.IsTrue(UnicodeData.getPropertyCodePoints("L").contains(0x1D770));
		Assert.IsTrue(UnicodeData.getPropertyCodePoints("N").contains(0x11C50));
		Assert.IsFalse(UnicodeData.getPropertyCodePoints("N").contains(0x1D5D4));
	}

	[TestMethod]
	public void testUnicodeCategoryAliases() {
		Assert.IsTrue(UnicodeData.getPropertyCodePoints("Lowercase_Letter").contains('x'));
		Assert.IsFalse(UnicodeData.getPropertyCodePoints("Lowercase_Letter").contains('X'));
		Assert.IsTrue(UnicodeData.getPropertyCodePoints("Letter").contains('x'));
		Assert.IsFalse(UnicodeData.getPropertyCodePoints("Letter").contains('0'));
		Assert.IsTrue(UnicodeData.getPropertyCodePoints("Enclosing_Mark").contains(0x20E2));
		Assert.IsFalse(UnicodeData.getPropertyCodePoints("Enclosing_Mark").contains('x'));
	}

	[TestMethod]
	public void testUnicodeBinaryProperties() {
		Assert.IsTrue(UnicodeData.getPropertyCodePoints("Emoji").contains(0x1F4A9));
		Assert.IsFalse(UnicodeData.getPropertyCodePoints("Emoji").contains('X'));
		Assert.IsTrue(UnicodeData.getPropertyCodePoints("alnum").contains('9'));
		Assert.IsFalse(UnicodeData.getPropertyCodePoints("alnum").contains(0x1F4A9));
		Assert.IsTrue(UnicodeData.getPropertyCodePoints("Dash").contains('-'));
		Assert.IsTrue(UnicodeData.getPropertyCodePoints("Hex").contains('D'));
		Assert.IsFalse(UnicodeData.getPropertyCodePoints("Hex").contains('Q'));
	}

	[TestMethod]
	public void testUnicodeBinaryPropertyAliases() {
		Assert.IsTrue(UnicodeData.getPropertyCodePoints("Ideo").contains('\u611B'));
		Assert.IsFalse(UnicodeData.getPropertyCodePoints("Ideo").contains('X'));
		Assert.IsTrue(UnicodeData.getPropertyCodePoints("Soft_Dotted").contains('\u0456'));
		Assert.IsFalse(UnicodeData.getPropertyCodePoints("Soft_Dotted").contains('X'));
		Assert.IsTrue(UnicodeData.getPropertyCodePoints("Noncharacter_Code_Point").contains('\uFFFF'));
		Assert.IsFalse(UnicodeData.getPropertyCodePoints("Noncharacter_Code_Point").contains('X'));
	}

	[TestMethod]
	public void testUnicodeScripts() {
		Assert.IsTrue(UnicodeData.getPropertyCodePoints("Zyyy").contains('0'));
		Assert.IsTrue(UnicodeData.getPropertyCodePoints("Latn").contains('X'));
		Assert.IsTrue(UnicodeData.getPropertyCodePoints("Hani").contains(0x4E04));
		Assert.IsTrue(UnicodeData.getPropertyCodePoints("Cyrl").contains(0x0404));
	}

	[TestMethod]
	public void testUnicodeScriptEquals() {
		Assert.IsTrue(UnicodeData.getPropertyCodePoints("Script=Zyyy").contains('0'));
		Assert.IsTrue(UnicodeData.getPropertyCodePoints("Script=Latn").contains('X'));
		Assert.IsTrue(UnicodeData.getPropertyCodePoints("Script=Hani").contains(0x4E04));
		Assert.IsTrue(UnicodeData.getPropertyCodePoints("Script=Cyrl").contains(0x0404));
	}

	[TestMethod]
	public void testUnicodeScriptAliases() {
		Assert.IsTrue(UnicodeData.getPropertyCodePoints("Common").contains('0'));
		Assert.IsTrue(UnicodeData.getPropertyCodePoints("Latin").contains('X'));
		Assert.IsTrue(UnicodeData.getPropertyCodePoints("Han").contains(0x4E04));
		Assert.IsTrue(UnicodeData.getPropertyCodePoints("Cyrillic").contains(0x0404));
	}

	[TestMethod]
	public void testUnicodeBlocks() {
		Assert.IsTrue(UnicodeData.getPropertyCodePoints("InASCII").contains('0'));
		Assert.IsTrue(UnicodeData.getPropertyCodePoints("InCJK").contains(0x4E04));
		Assert.IsTrue(UnicodeData.getPropertyCodePoints("InCyrillic").contains(0x0404));
		Assert.IsTrue(UnicodeData.getPropertyCodePoints("InMisc_Pictographs").contains(0x1F4A9));
	}

	[TestMethod]
	public void testUnicodeBlockEquals() {
		Assert.IsTrue(UnicodeData.getPropertyCodePoints("Block=ASCII").contains('0'));
		Assert.IsTrue(UnicodeData.getPropertyCodePoints("Block=CJK").contains(0x4E04));
		Assert.IsTrue(UnicodeData.getPropertyCodePoints("Block=Cyrillic").contains(0x0404));
		Assert.IsTrue(UnicodeData.getPropertyCodePoints("Block=Misc_Pictographs").contains(0x1F4A9));
	}

	[TestMethod]
	public void testUnicodeBlockAliases() {
		Assert.IsTrue(UnicodeData.getPropertyCodePoints("InBasic_Latin").contains('0'));
		Assert.IsTrue(UnicodeData.getPropertyCodePoints("InMiscellaneous_Mathematical_Symbols_B").contains(0x29BE));
	}

	[TestMethod]
	public void testEnumeratedPropertyEquals() {
		Assert.IsFalse(
				UnicodeData.getPropertyCodePoints("Grapheme_Cluster_Break=E_Base").contains(0x1F47E),
				"U+1F47E ALIEN MONSTER is not an emoji modifier");

		Assert.IsFalse(
				UnicodeData.getPropertyCodePoints("Grapheme_Cluster_Break=E_Base").contains(0x1038),
				"U+1038 MYANMAR SIGN VISARGA is not a spacing mark");

		Assert.IsTrue(
				UnicodeData.getPropertyCodePoints("East_Asian_Width=Ambiguous").contains(0x00A1),
				"U+00A1 INVERTED EXCLAMATION MARK has ambiguous East Asian Width");

		Assert.IsFalse(
				UnicodeData.getPropertyCodePoints("East_Asian_Width=Ambiguous").contains(0x00A2),
				"U+00A2 CENT SIGN does not have ambiguous East Asian Width");
	}

        [TestMethod]
        public void extendedPictographic() {
		Assert.IsTrue(
				UnicodeData.getPropertyCodePoints("Extended_Pictographic").contains(0x1F588),
				"U+1F588 BLACK PUSHPIN is in Extended Pictographic");
		Assert.IsFalse(
				UnicodeData.getPropertyCodePoints("Extended_Pictographic").contains('0'),
				"0 is not in Extended Pictographic");
        }

        [TestMethod]
        public void emojiPresentation() {
		Assert.IsTrue(
				UnicodeData.getPropertyCodePoints("EmojiPresentation=EmojiDefault").contains(0x1F4A9),
				"U+1F4A9 PILE OF POO is in EmojiPresentation=EmojiDefault");
		Assert.IsFalse(
				UnicodeData.getPropertyCodePoints("EmojiPresentation=EmojiDefault").contains('0'),
				"0 is not in EmojiPresentation=EmojiDefault");
		Assert.IsFalse(
				UnicodeData.getPropertyCodePoints("EmojiPresentation=EmojiDefault").contains('A'),
				"A is not in EmojiPresentation=EmojiDefault");
		Assert.IsFalse(
				UnicodeData.getPropertyCodePoints("EmojiPresentation=TextDefault").contains(0x1F4A9),
				"U+1F4A9 PILE OF POO is not in EmojiPresentation=TextDefault");
		Assert.IsTrue(
				UnicodeData.getPropertyCodePoints("EmojiPresentation=TextDefault").contains('0'),
				"0 is in EmojiPresentation=TextDefault");
		Assert.IsFalse(
				UnicodeData.getPropertyCodePoints("EmojiPresentation=TextDefault").contains('A'),
				"A is not in EmojiPresentation=TextDefault");
        }

	[TestMethod]
	public void testPropertyCaseInsensitivity() {
		Assert.IsTrue(UnicodeData.getPropertyCodePoints("l").contains('x'));
		Assert.IsFalse(UnicodeData.getPropertyCodePoints("l").contains('0'));
		Assert.IsTrue(UnicodeData.getPropertyCodePoints("common").contains('0'));
		Assert.IsTrue(UnicodeData.getPropertyCodePoints("Alnum").contains('0'));
	}

	[TestMethod]
	public void testPropertyDashSameAsUnderscore() {
		Assert.IsTrue(UnicodeData.getPropertyCodePoints("InLatin-1").contains('\u00F0'));
	}

	[TestMethod]
	public void modifyingUnicodeDataShouldThrow() {
		IllegalStateException exception = assertThrows(IllegalStateException, () => UnicodeData.getPropertyCodePoints("L").add(0x12345));
		Assert.AreEqual("can't alter readonly IntervalSet", exception.Message);
	}
}
