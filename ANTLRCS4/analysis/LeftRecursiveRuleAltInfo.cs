/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.tool.ast;

namespace org.antlr.v4.analysis;

public class LeftRecursiveRuleAltInfo
{
    public int altNum; // original alt index (from 1)
    public string leftRecursiveRuleRefLabel;
    public string altLabel;
    public readonly bool isListLabel;
    public string altText;
    public AltAST altAST; // transformed ALT
    public AltAST originalAltAST;
    public int nextPrec;

    public LeftRecursiveRuleAltInfo(int altNum, string altText)
        : this(altNum, altText, null, null, false, null)
    {
    }

    public LeftRecursiveRuleAltInfo(int altNum, string altText,
                                    string leftRecursiveRuleRefLabel,
                                    string altLabel,
                                    bool isListLabel,
                                    AltAST originalAltAST)
    {
        this.altNum = altNum;
        this.altText = altText;
        this.leftRecursiveRuleRefLabel = leftRecursiveRuleRefLabel;
        this.altLabel = altLabel;
        this.isListLabel = isListLabel;
        this.originalAltAST = originalAltAST;
    }
}
