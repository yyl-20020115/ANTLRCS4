/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime.dfa;

namespace org.antlr.v4.runtime.atn;

/**
 *
 * @author Sam Harwell
 */
public class ATNDeserializationOptions
{
    private static readonly ATNDeserializationOptions defaultOptions = new();
    static ATNDeserializationOptions()
    {
        defaultOptions.MakeReadOnly();
    }

    private bool readOnly;
    private bool verifyATN;
    private bool generateRuleBypassTransitions;

    public ATNDeserializationOptions()
    {
        this.verifyATN = true;
        this.generateRuleBypassTransitions = false;
    }

    public ATNDeserializationOptions(ATNDeserializationOptions options)
    {
        this.verifyATN = options.verifyATN;
        this.generateRuleBypassTransitions = options.generateRuleBypassTransitions;
    }


    public static ATNDeserializationOptions GetDefaultOptions() => defaultOptions;

    public bool IsReadOnly => this.readOnly;

    public void MakeReadOnly() => this.readOnly = true;

    public bool VerifyATN
    {
        get => verifyATN;
        set
        {
            ThrowIfReadOnly();
            this.verifyATN = value;
        }
    }

    public bool GenerateRuleBypassTransitions
    {
        get => generateRuleBypassTransitions;
        set
        {
            ThrowIfReadOnly();
            this.generateRuleBypassTransitions = value;
        }
    }

    protected void ThrowIfReadOnly()
    {
        if (IsReadOnly)
            throw new IllegalStateException("The object is read only.");
    }
}
