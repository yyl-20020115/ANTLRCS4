/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime;
using org.antlr.v4.runtime.misc;

namespace org.antlr.v4.test.tool;

[TestClass]
public class TestIntervalSet
{

    /** Public default constructor used by TestRig */
    public TestIntervalSet() { }

    [TestMethod]
    public void TestSingleElement()
    {
        var s = IntervalSet.Of(99);
        var expecting = "99";
        Assert.AreEqual(s.ToString(), expecting);
    }

    [TestMethod]
    public void TestMin()
    {
        Assert.AreEqual(0, IntervalSet.COMPLETE_CHAR_SET.GetMinElement());
        Assert.AreEqual(Token.EPSILON, IntervalSet.COMPLETE_CHAR_SET.Or(IntervalSet.Of(Token.EPSILON)).GetMinElement());
        Assert.AreEqual(Token.EOF, IntervalSet.COMPLETE_CHAR_SET.Or(IntervalSet.Of(Token.EOF)).GetMinElement());
    }

    [TestMethod]
    public void TestIsolatedElements()
    {
        var s = new IntervalSet();
        s.Add(1);
        s.Add('z');
        s.Add('\uFFF0');
        var expecting = "{1, 122, 65520}";
        Assert.AreEqual(s.ToString(), expecting);
    }

    [TestMethod]
    public void TestMixedRangesAndElements()
    {
        var s = new IntervalSet();
        s.Add(1);
        s.Add('a', 'z');
        s.Add('0', '9');
        var expecting = "{1, 48..57, 97..122}";
        Assert.AreEqual(s.ToString(), expecting);
    }

    [TestMethod]
    public void TestSimpleAnd()
    {
        var s = IntervalSet.Of(10, 20);
        var s2 = IntervalSet.Of(13, 15);
        var expecting = "{13..15}";
        var result = (s.And(s2)).ToString();
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    public void TestRangeAndIsolatedElement()
    {
        var s = IntervalSet.Of('a', 'z');
        var s2 = IntervalSet.Of('d');
        var expecting = "100";
        var result = (s.And(s2)).ToString();
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    public void TestEmptyIntersection()
    {
        var s = IntervalSet.Of('a', 'z');
        var s2 = IntervalSet.Of('0', '9');
        var expecting = "{}";
        var result = (s.And(s2)).ToString();
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    public void TestEmptyIntersectionSingleElements()
    {
        var s = IntervalSet.Of('a');
        var s2 = IntervalSet.Of('d');
        var expecting = "{}";
        var result = (s.And(s2)).ToString();
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    public void TestNotSingleElement()
    {
        var vocabulary = IntervalSet.Of(1, 1000);
        vocabulary.Add(2000, 3000);
        var s = IntervalSet.Of(50, 50);
        var expecting = "{1..49, 51..1000, 2000..3000}";
        var result = (s.Complement(vocabulary)).ToString();
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    public void TestNotSet()
    {
        var vocabulary = IntervalSet.Of(1, 1000);
        var s = IntervalSet.Of(50, 60);
        s.Add(5);
        s.Add(250, 300);
        var expecting = "{1..4, 6..49, 61..249, 301..1000}";
        var result = (s.Complement(vocabulary)).ToString();
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    public void TestNotEqualSet()
    {
        var vocabulary = IntervalSet.Of(1, 1000);
        var s = IntervalSet.Of(1, 1000);
        var expecting = "{}";
        var result = (s.Complement(vocabulary)).ToString();
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    public void TestNotSetEdgeElement()
    {
        var vocabulary = IntervalSet.Of(1, 2);
        var s = IntervalSet.Of(1);
        var expecting = "2";
        var result = (s.Complement(vocabulary)).ToString();
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    public void TestNotSetFragmentedVocabulary()
    {
        var vocabulary = IntervalSet.Of(1, 255);
        vocabulary.Add(1000, 2000);
        vocabulary.Add(9999);
        var s = IntervalSet.Of(50, 60);
        s.Add(3);
        s.Add(250, 300);
        s.Add(10000); // this is outside range of vocab and should be ignored
        var expecting = "{1..2, 4..49, 61..249, 1000..2000, 9999}";
        var result = (s.Complement(vocabulary)).ToString();
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    public void TestSubtractOfCompletelyContainedRange()
    {
        var s = IntervalSet.Of(10, 20);
        var s2 = IntervalSet.Of(12, 15);
        var expecting = "{10..11, 16..20}";
        var result = (s.Subtract(s2)).ToString();
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    public void TestSubtractFromSetWithEOF()
    {
        var s = IntervalSet.Of(10, 20);
        s.Add(Token.EOF);
        var s2 = IntervalSet.Of(12, 15);
        var expecting = "{<EOF>, 10..11, 16..20}";
        var result = (s.Subtract(s2)).ToString();
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    public void TestSubtractOfOverlappingRangeFromLeft()
    {
        var s = IntervalSet.Of(10, 20);
        var s2 = IntervalSet.Of(5, 11);
        var expecting = "{12..20}";
        var result = (s.Subtract(s2)).ToString();
        Assert.AreEqual(expecting, result);

        var s3 = IntervalSet.Of(5, 10);
        expecting = "{11..20}";
        result = (s.Subtract(s3)).ToString();
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    public void TestSubtractOfOverlappingRangeFromRight()
    {
        var s = IntervalSet.Of(10, 20);
        var s2 = IntervalSet.Of(15, 25);
        var expecting = "{10..14}";
        var result = (s.Subtract(s2)).ToString();
        Assert.AreEqual(expecting, result);

        var s3 = IntervalSet.Of(20, 25);
        expecting = "{10..19}";
        result = (s.Subtract(s3)).ToString();
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    public void TestSubtractOfCompletelyCoveredRange()
    {
        var s = IntervalSet.Of(10, 20);
        var s2 = IntervalSet.Of(1, 25);
        var expecting = "{}";
        var result = (s.Subtract(s2)).ToString();
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    public void TestSubtractOfRangeSpanningMultipleRanges()
    {
        var s = IntervalSet.Of(10, 20);
        s.Add(30, 40);
        s.Add(50, 60); // s has 3 ranges now: 10..20, 30..40, 50..60
        var s2 = IntervalSet.Of(5, 55); // covers one and touches 2nd range
        var expecting = "{56..60}";
        var result = (s.Subtract(s2)).ToString();
        Assert.AreEqual(expecting, result);

        var s3 = IntervalSet.Of(15, 55); // touches both
        expecting = "{10..14, 56..60}";
        result = (s.Subtract(s3)).ToString();
        Assert.AreEqual(expecting, result);
    }

    /** The following was broken:
	 	{0..113, 115..65534}-{0..115, 117..65534}=116..65534
	 */
    [TestMethod]
    public void TestSubtractOfWackyRange()
    {
        var s = IntervalSet.Of(0, 113);
        s.Add(115, 200);
        var s2 = IntervalSet.Of(0, 115);
        s2.Add(117, 200);
        var expecting = "116";
        var result = (s.Subtract(s2)).ToString();
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    public void TestSimpleEquals()
    {
        var s = IntervalSet.Of(10, 20);
        var s2 = IntervalSet.Of(10, 20);
        Assert.AreEqual(s, s2);

        var s3 = IntervalSet.Of(15, 55);
        Assert.IsFalse(s.Equals(s3));
    }

    [TestMethod]
    public void TestEquals()
    {
        var s = IntervalSet.Of(10, 20);
        s.Add(2);
        s.Add(499, 501);
        var s2 = IntervalSet.Of(10, 20);
        s2.Add(2);
        s2.Add(499, 501);
        Assert.AreEqual(s, s2);

        var s3 = IntervalSet.Of(10, 20);
        s3.Add(2);
        Assert.IsFalse(s.Equals(s3));
    }

    [TestMethod]
    public void TestSingleElementMinusDisjointSet()
    {
        var s = IntervalSet.Of(15, 15);
        var s2 = IntervalSet.Of(1, 5);
        s2.Add(10, 20);
        var expecting = "{}"; // 15 - {1..5, 10..20} = {}
        var result = s.Subtract(s2).ToString();
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    public void TestMembership()
    {
        var s = IntervalSet.Of(15, 15);
        s.Add(50, 60);
        Assert.IsTrue(!s.Contains(0));
        Assert.IsTrue(!s.Contains(20));
        Assert.IsTrue(!s.Contains(100));
        Assert.IsTrue(s.Contains(15));
        Assert.IsTrue(s.Contains(55));
        Assert.IsTrue(s.Contains(50));
        Assert.IsTrue(s.Contains(60));
    }

    // {2,15,18} & 10..20
    [TestMethod]
    public void TestIntersectionWithTwoContainedElements()
    {
        var s = IntervalSet.Of(10, 20);
        var s2 = IntervalSet.Of(2, 2);
        s2.Add(15);
        s2.Add(18);
        var expecting = "{15, 18}";
        var result = (s.And(s2)).ToString();
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    public void TestIntersectionWithTwoContainedElementsReversed()
    {
        var s = IntervalSet.Of(10, 20);
        var s2 = IntervalSet.Of(2, 2);
        s2.Add(15);
        s2.Add(18);
        var expecting = "{15, 18}";
        var result = (s2.And(s)).ToString();
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    public void TestComplement()
    {
        var s = IntervalSet.Of(100, 100);
        s.Add(101, 101);
        var s2 = IntervalSet.Of(100, 102);
        var expecting = "102";
        var result = (s.Complement(s2)).ToString();
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    public void TestComplement2()
    {
        var s = IntervalSet.Of(100, 101);
        var s2 = IntervalSet.Of(100, 102);
        var expecting = "102";
        var result = (s.Complement(s2)).ToString();
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    public void TestComplement3()
    {
        var s = IntervalSet.Of(1, 96);
        s.Add(99, Lexer.MAX_CHAR_VALUE);
        var expecting = "{97..98}";
        var result = (s.Complement(1, Lexer.MAX_CHAR_VALUE)).ToString();
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    public void TestMergeOfRangesAndSingleValues()
    {
        // {0..41, 42, 43..65534}
        var s = IntervalSet.Of(0, 41);
        s.Add(42);
        s.Add(43, 65534);
        var expecting = "{0..65534}";
        var result = s.ToString();
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    public void TestMergeOfRangesAndSingleValuesReverse()
    {
        var s = IntervalSet.Of(43, 65534);
        s.Add(42);
        s.Add(0, 41);
        var expecting = "{0..65534}";
        var result = s.ToString();
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    public void TestMergeWhereAdditionMergesTwoExistingIntervals()
    {
        // 42, 10, {0..9, 11..41, 43..65534}
        var s = IntervalSet.Of(42);
        s.Add(10);
        s.Add(0, 9);
        s.Add(43, 65534);
        s.Add(11, 41);
        var expecting = "{0..65534}";
        var result = s.ToString();
        Assert.AreEqual(expecting, result);
    }

    /**
	 * This case is responsible for antlr/antlr4#153.
	 * https://github.com/antlr/antlr4/issues/153
	 */
    [TestMethod]
    public void TestMergeWhereAdditionMergesThreeExistingIntervals()
    {
        var s = new IntervalSet();
        s.Add(0);
        s.Add(3);
        s.Add(5);
        s.Add(0, 7);
        var expecting = "{0..7}";
        var result = s.ToString();
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    public void TestMergeWithDoubleOverlap()
    {
        var s = IntervalSet.Of(1, 10);
        s.Add(20, 30);
        s.Add(5, 25); // overlaps two!
        var expecting = "{1..30}";
        var result = s.ToString();
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    public void TestSize()
    {
        var s = IntervalSet.Of(20, 30);
        s.Add(50, 55);
        s.Add(5, 19);
        var expecting = "32";
        var result = (s.Size.ToString());
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    public void TestToList()
    {
        var s = IntervalSet.Of(20, 25);
        s.Add(50, 55);
        s.Add(5, 5);
        var expecting = "[5, 20, 21, 22, 23, 24, 25, 50, 51, 52, 53, 54, 55]";
        var result = "[" + string.Join(", ", (s.ToList())) + "]";
        Assert.AreEqual(expecting, result);
    }

    /** The following was broken:
	    {'\u0000'..'s', 'u'..'\uFFFE'} & {'\u0000'..'q', 's'..'\uFFFE'}=
	    {'\u0000'..'q', 's'}!!!! broken...
	 	'q' is 113 ascii
	 	'u' is 117
	*/
    [TestMethod]
    public void TestNotRIntersectionNotT()
    {
        var s = IntervalSet.Of(0, 's');
        s.Add('u', 200);
        var s2 = IntervalSet.Of(0, 'q');
        s2.Add('s', 200);
        var expecting = "{0..113, 115, 117..200}";
        var result = (s.And(s2)).ToString();
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    public void TestRmSingleElement()
    {
        var s = IntervalSet.Of(1, 10);
        s.Add(-3, -3);
        s.Remove(-3);
        var expecting = "{1..10}";
        var result = s.ToString();
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    public void TestRmLeftSide()
    {
        var s = IntervalSet.Of(1, 10);
        s.Add(-3, -3);
        s.Remove(1);
        var expecting = "{-3, 2..10}";
        var result = s.ToString();
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    public void TestRmRightSide()
    {
        var s = IntervalSet.Of(1, 10);
        s.Add(-3, -3);
        s.Remove(10);
        var expecting = "{-3, 1..9}";
        var result = s.ToString();
        Assert.AreEqual(expecting, result);
    }

    [TestMethod]
    public void TestRmMiddleRange()
    {
        var s = IntervalSet.Of(1, 10);
        s.Add(-3, -3);
        s.Remove(5);
        var expecting = "{-3, 1..4, 6..10}";
        var result = s.ToString();
        Assert.AreEqual(expecting, result);
    }
}
