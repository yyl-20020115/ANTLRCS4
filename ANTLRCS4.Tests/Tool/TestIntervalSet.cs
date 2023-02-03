/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime;
using org.antlr.v4.runtime.misc;

namespace org.antlr.v4.test.tool;

[TestClass]
public class TestIntervalSet {

    /** Public default constructor used by TestRig */
    public TestIntervalSet() {
	}

	[TestMethod] public void testSingleElement(){
		IntervalSet s = IntervalSet.of(99);
		String expecting = "99";
		Assert.AreEqual(s.ToString(), expecting);
	}

	[TestMethod] public void testMin(){
		Assert.AreEqual(0, IntervalSet.COMPLETE_CHAR_SET.getMinElement());
		Assert.AreEqual(Token.EPSILON, IntervalSet.COMPLETE_CHAR_SET.or(IntervalSet.of(Token.EPSILON)).getMinElement());
		Assert.AreEqual(Token.EOF, IntervalSet.COMPLETE_CHAR_SET.or(IntervalSet.of(Token.EOF)).getMinElement());
	}

	[TestMethod] public void testIsolatedElements(){
		IntervalSet s = new IntervalSet();
		s.add(1);
		s.add('z');
		s.add('\uFFF0');
		String expecting = "{1, 122, 65520}";
        Assert.AreEqual(s.ToString(), expecting);
    }

    [TestMethod] public void testMixedRangesAndElements(){
        IntervalSet s = new IntervalSet();
        s.add(1);
        s.Add('a','z');
        s.Add('0','9');
        String expecting = "{1, 48..57, 97..122}";
        Assert.AreEqual(s.ToString(), expecting);
    }

    [TestMethod] public void testSimpleAnd(){
        IntervalSet s = IntervalSet.of(10,20);
        IntervalSet s2 = IntervalSet.of(13,15);
        String expecting = "{13..15}";
        String result = (s.and(s2)).ToString();
        Assert.AreEqual(expecting, result);
    }

    [TestMethod] public void testRangeAndIsolatedElement(){
        IntervalSet s = IntervalSet.of('a','z');
        IntervalSet s2 = IntervalSet.of('d');
        String expecting = "100";
        String result = (s.and(s2)).ToString();
        Assert.AreEqual(expecting, result);
    }

	[TestMethod] public void testEmptyIntersection(){
		IntervalSet s = IntervalSet.of('a','z');
		IntervalSet s2 = IntervalSet.of('0','9');
		String expecting = "{}";
		String result = (s.and(s2)).ToString();
		Assert.AreEqual(expecting, result);
	}

	[TestMethod] public void testEmptyIntersectionSingleElements(){
		IntervalSet s = IntervalSet.of('a');
		IntervalSet s2 = IntervalSet.of('d');
		String expecting = "{}";
		String result = (s.and(s2)).ToString();
		Assert.AreEqual(expecting, result);
	}

    [TestMethod] public void testNotSingleElement(){
        IntervalSet vocabulary = IntervalSet.of(1,1000);
        vocabulary.Add(2000,3000);
        IntervalSet s = IntervalSet.of(50,50);
        String expecting = "{1..49, 51..1000, 2000..3000}";
        String result = (s.complement(vocabulary)).ToString();
        Assert.AreEqual(expecting, result);
    }

	[TestMethod] public void testNotSet(){
		IntervalSet vocabulary = IntervalSet.of(1,1000);
		IntervalSet s = IntervalSet.of(50,60);
		s.add(5);
		s.Add(250,300);
		String expecting = "{1..4, 6..49, 61..249, 301..1000}";
		String result = (s.complement(vocabulary)).ToString();
		Assert.AreEqual(expecting, result);
	}

	[TestMethod] public void testNotEqualSet(){
		IntervalSet vocabulary = IntervalSet.of(1,1000);
		IntervalSet s = IntervalSet.of(1,1000);
		String expecting = "{}";
		String result = (s.complement(vocabulary)).ToString();
		Assert.AreEqual(expecting, result);
	}

	[TestMethod] public void testNotSetEdgeElement(){
		IntervalSet vocabulary = IntervalSet.of(1,2);
		IntervalSet s = IntervalSet.of(1);
		String expecting = "2";
		String result = (s.complement(vocabulary)).ToString();
		Assert.AreEqual(expecting, result);
	}

    [TestMethod] public void testNotSetFragmentedVocabulary(){
        IntervalSet vocabulary = IntervalSet.of(1,255);
        vocabulary.Add(1000,2000);
        vocabulary.add(9999);
        IntervalSet s = IntervalSet.of(50, 60);
        s.add(3);
        s.Add(250,300);
        s.add(10000); // this is outside range of vocab and should be ignored
        String expecting = "{1..2, 4..49, 61..249, 1000..2000, 9999}";
        String result = (s.complement(vocabulary)).ToString();
        Assert.AreEqual(expecting, result);
    }

    [TestMethod] public void testSubtractOfCompletelyContainedRange(){
        IntervalSet s = IntervalSet.of(10,20);
        IntervalSet s2 = IntervalSet.of(12,15);
        String expecting = "{10..11, 16..20}";
        String result = (s.subtract(s2)).ToString();
        Assert.AreEqual(expecting, result);
    }

	[TestMethod] public void testSubtractFromSetWithEOF(){
		IntervalSet s = IntervalSet.of(10,20);
		s.add(Token.EOF);
		IntervalSet s2 = IntervalSet.of(12,15);
		String expecting = "{<EOF>, 10..11, 16..20}";
		String result = (s.subtract(s2)).ToString();
		Assert.AreEqual(expecting, result);
	}

	[TestMethod] public void testSubtractOfOverlappingRangeFromLeft(){
		IntervalSet s = IntervalSet.of(10,20);
		IntervalSet s2 = IntervalSet.of(5,11);
		String expecting = "{12..20}";
        String result = (s.subtract(s2)).ToString();
        Assert.AreEqual(expecting, result);

        IntervalSet s3 = IntervalSet.of(5,10);
        expecting = "{11..20}";
        result = (s.subtract(s3)).ToString();
        Assert.AreEqual(expecting, result);
    }

    [TestMethod] public void testSubtractOfOverlappingRangeFromRight(){
        IntervalSet s = IntervalSet.of(10,20);
        IntervalSet s2 = IntervalSet.of(15,25);
        String expecting = "{10..14}";
        String result = (s.subtract(s2)).ToString();
        Assert.AreEqual(expecting, result);

        IntervalSet s3 = IntervalSet.of(20,25);
        expecting = "{10..19}";
        result = (s.subtract(s3)).ToString();
        Assert.AreEqual(expecting, result);
    }

    [TestMethod] public void testSubtractOfCompletelyCoveredRange(){
        IntervalSet s = IntervalSet.of(10,20);
        IntervalSet s2 = IntervalSet.of(1,25);
        String expecting = "{}";
        String result = (s.subtract(s2)).ToString();
        Assert.AreEqual(expecting, result);
    }

    [TestMethod] public void testSubtractOfRangeSpanningMultipleRanges(){
        IntervalSet s = IntervalSet.of(10,20);
        s.Add(30,40);
        s.Add(50,60); // s has 3 ranges now: 10..20, 30..40, 50..60
        IntervalSet s2 = IntervalSet.of(5,55); // covers one and touches 2nd range
        String expecting = "{56..60}";
        String result = (s.subtract(s2)).ToString();
        Assert.AreEqual(expecting, result);

        IntervalSet s3 = IntervalSet.of(15,55); // touches both
        expecting = "{10..14, 56..60}";
        result = (s.subtract(s3)).ToString();
        Assert.AreEqual(expecting, result);
    }

	/** The following was broken:
	 	{0..113, 115..65534}-{0..115, 117..65534}=116..65534
	 */
	[TestMethod] public void testSubtractOfWackyRange(){
		IntervalSet s = IntervalSet.of(0,113);
		s.Add(115,200);
		IntervalSet s2 = IntervalSet.of(0,115);
		s2.Add(117,200);
		String expecting = "116";
		String result = (s.subtract(s2)).ToString();
		Assert.AreEqual(expecting, result);
	}

    [TestMethod] public void testSimpleEquals(){
        IntervalSet s = IntervalSet.of(10,20);
        IntervalSet s2 = IntervalSet.of(10,20);
        Assert.AreEqual(s, s2);

        IntervalSet s3 = IntervalSet.of(15,55);
        Assert.IsFalse(s.Equals(s3));
    }

    [TestMethod] public void testEquals(){
        IntervalSet s = IntervalSet.of(10,20);
        s.add(2);
        s.Add(499,501);
        IntervalSet s2 = IntervalSet.of(10,20);
        s2.add(2);
        s2.Add(499,501);
        Assert.AreEqual(s, s2);

        IntervalSet s3 = IntervalSet.of(10,20);
        s3.add(2);
		Assert.IsFalse(s.Equals(s3));
    }

    [TestMethod] public void testSingleElementMinusDisjointSet(){
        IntervalSet s = IntervalSet.of(15,15);
        IntervalSet s2 = IntervalSet.of(1,5);
        s2.Add(10,20);
        String expecting = "{}"; // 15 - {1..5, 10..20} = {}
        String result = s.subtract(s2).ToString();
        Assert.AreEqual(expecting, result);
    }

    [TestMethod] public void testMembership(){
        IntervalSet s = IntervalSet.of(15,15);
        s.Add(50,60);
        Assert.IsTrue(!s.contains(0));
        Assert.IsTrue(!s.contains(20));
        Assert.IsTrue(!s.contains(100));
        Assert.IsTrue(s.contains(15));
        Assert.IsTrue(s.contains(55));
        Assert.IsTrue(s.contains(50));
        Assert.IsTrue(s.contains(60));
    }

    // {2,15,18} & 10..20
    [TestMethod] public void testIntersectionWithTwoContainedElements(){
        IntervalSet s = IntervalSet.of(10,20);
        IntervalSet s2 = IntervalSet.of(2,2);
        s2.add(15);
        s2.add(18);
        String expecting = "{15, 18}";
        String result = (s.and(s2)).ToString();
        Assert.AreEqual(expecting, result);
    }

    [TestMethod] public void testIntersectionWithTwoContainedElementsReversed(){
        IntervalSet s = IntervalSet.of(10,20);
        IntervalSet s2 = IntervalSet.of(2,2);
        s2.add(15);
        s2.add(18);
        String expecting = "{15, 18}";
        String result = (s2.and(s)).ToString();
        Assert.AreEqual(expecting, result);
    }

    [TestMethod] public void testComplement(){
        IntervalSet s = IntervalSet.of(100,100);
        s.Add(101,101);
        IntervalSet s2 = IntervalSet.of(100,102);
        String expecting = "102";
        String result = (s.complement(s2)).ToString();
        Assert.AreEqual(expecting, result);
    }

	[TestMethod] public void testComplement2(){
		IntervalSet s = IntervalSet.of(100,101);
		IntervalSet s2 = IntervalSet.of(100,102);
		String expecting = "102";
		String result = (s.complement(s2)).ToString();
		Assert.AreEqual(expecting, result);
	}

	[TestMethod] public void testComplement3(){
		IntervalSet s = IntervalSet.of(1,96);
		s.Add(99, Lexer.MAX_CHAR_VALUE);
		String expecting = "{97..98}";
		String result = (s.complement(1, Lexer.MAX_CHAR_VALUE)).ToString();
		Assert.AreEqual(expecting, result);
	}

    [TestMethod] public void testMergeOfRangesAndSingleValues(){
        // {0..41, 42, 43..65534}
        IntervalSet s = IntervalSet.of(0,41);
        s.add(42);
        s.Add(43,65534);
        String expecting = "{0..65534}";
        String result = s.ToString();
        Assert.AreEqual(expecting, result);
    }

    [TestMethod] public void testMergeOfRangesAndSingleValuesReverse(){
        IntervalSet s = IntervalSet.of(43,65534);
        s.add(42);
        s.Add(0,41);
        String expecting = "{0..65534}";
        String result = s.ToString();
        Assert.AreEqual(expecting, result);
    }

    [TestMethod] public void testMergeWhereAdditionMergesTwoExistingIntervals(){
        // 42, 10, {0..9, 11..41, 43..65534}
        IntervalSet s = IntervalSet.of(42);
        s.add(10);
        s.Add(0,9);
        s.Add(43,65534);
        s.Add(11,41);
        String expecting = "{0..65534}";
        String result = s.ToString();
        Assert.AreEqual(expecting, result);
    }

	/**
	 * This case is responsible for antlr/antlr4#153.
	 * https://github.com/antlr/antlr4/issues/153
	 */
	[TestMethod] public void testMergeWhereAdditionMergesThreeExistingIntervals(){
		IntervalSet s = new IntervalSet();
		s.add(0);
		s.add(3);
		s.add(5);
		s.Add(0, 7);
		String expecting = "{0..7}";
		String result = s.ToString();
		Assert.AreEqual(expecting, result);
	}

	[TestMethod] public void testMergeWithDoubleOverlap(){
		IntervalSet s = IntervalSet.of(1,10);
		s.Add(20,30);
		s.Add(5,25); // overlaps two!
		String expecting = "{1..30}";
		String result = s.ToString();
		Assert.AreEqual(expecting, result);
	}

	[TestMethod] public void testSize(){
		IntervalSet s = IntervalSet.of(20,30);
		s.Add(50,55);
		s.Add(5,19);
		String expecting = "32";
		String result = (s.size().ToString());
		Assert.AreEqual(expecting, result);
	}

	[TestMethod] public void testToList(){
		IntervalSet s = IntervalSet.of(20,25);
		s.Add(50,55);
		s.Add(5,5);
		String expecting = "[5, 20, 21, 22, 23, 24, 25, 50, 51, 52, 53, 54, 55]";
		String result ="[" +string.Join(", ",(s.toList()))+"]";
		Assert.AreEqual(expecting, result);
	}

	/** The following was broken:
	    {'\u0000'..'s', 'u'..'\uFFFE'} & {'\u0000'..'q', 's'..'\uFFFE'}=
	    {'\u0000'..'q', 's'}!!!! broken...
	 	'q' is 113 ascii
	 	'u' is 117
	*/
	[TestMethod] public void testNotRIntersectionNotT(){
		IntervalSet s = IntervalSet.of(0,'s');
		s.Add('u',200);
		IntervalSet s2 = IntervalSet.of(0,'q');
		s2.Add('s',200);
		String expecting = "{0..113, 115, 117..200}";
		String result = (s.and(s2)).ToString();
		Assert.AreEqual(expecting, result);
	}

    [TestMethod] public void testRmSingleElement(){
        IntervalSet s = IntervalSet.of(1,10);
        s.Add(-3,-3);
        s.remove(-3);
        String expecting = "{1..10}";
        String result = s.ToString();
        Assert.AreEqual(expecting, result);
    }

    [TestMethod] public void testRmLeftSide(){
        IntervalSet s = IntervalSet.of(1,10);
        s.Add(-3,-3);
        s.remove(1);
        String expecting = "{-3, 2..10}";
        String result = s.ToString();
        Assert.AreEqual(expecting, result);
    }

    [TestMethod] public void testRmRightSide(){
        IntervalSet s = IntervalSet.of(1,10);
        s.Add(-3,-3);
        s.remove(10);
        String expecting = "{-3, 1..9}";
        String result = s.ToString();
        Assert.AreEqual(expecting, result);
    }

    [TestMethod] public void testRmMiddleRange(){
        IntervalSet s = IntervalSet.of(1,10);
        s.Add(-3,-3);
        s.remove(5);
        String expecting = "{-3, 1..4, 6..10}";
        String result = s.ToString();
        Assert.AreEqual(expecting, result);
    }
}
