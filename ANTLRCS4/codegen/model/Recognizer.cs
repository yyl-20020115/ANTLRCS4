/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */
using org.antlr.v4.codegen.model.chunk;
using org.antlr.v4.codegen.target;
using org.antlr.v4.runtime.misc;
using org.antlr.v4.tool;

namespace org.antlr.v4.codegen.model;

public abstract class Recognizer : OutputModelObject
{
    public string name;
    public string grammarName;
    public string grammarFileName;
    public string accessLevel;
    public Dictionary<string, int> tokens;

    /**
	 * @deprecated This field is provided only for compatibility with code
	 * generation targets which have not yet been updated to use
	 * {@link #literalNames} and {@link #symbolicNames}.
	 */
    //@Deprecated
    public List<string> tokenNames;

    public List<string> literalNames;
    public List<string> symbolicNames;
    public HashSet<string> ruleNames;
    public ICollection<Rule> rules;
    [ModelElement]
    public ActionChunk superClass;
    [ModelElement]
    public SerializedATN atn;
    [ModelElement]
    public Dictionary<Rule, RuleSempredFunction> sempredFuncs = new();

    public Recognizer(OutputModelFactory factory) : base(factory)
    {
        var g = factory.Grammar;
        var gen = factory.Generator;
        grammarFileName = g.fileName;
        grammarName = g.name;
        name = g.GetRecognizerName();
        accessLevel = g.GetOptionString("accessLevel");
        tokens = new();
        foreach (var entry in g.tokenNameToTypeMap)
        {
            int ttype = entry.Value;
            if (ttype > 0)
            {
                tokens[entry.Key] = ttype;
            }
        }

        ruleNames = g.rules.Keys.ToHashSet();
        rules = g.rules.Values;
        atn = gen.Target is JavaTarget ? new SerializedJavaATN(factory, g.atn) : new SerializedATN(factory, g.atn);
        superClass = g.GetOptionString("superClass") != null ? new ActionText(null, g.GetOptionString("superClass")) : (ActionChunk?)null;
        tokenNames = TranslateTokenStringsToTarget(g.GetTokenDisplayNames(), gen);
        literalNames = TranslateTokenStringsToTarget(g.GetTokenLiteralNames(), gen);
        symbolicNames = TranslateTokenStringsToTarget(g.GetTokenSymbolicNames(), gen);
    }

    protected static List<string> TranslateTokenStringsToTarget(string[] tokenStrings, CodeGenerator gen)
    {
        var result = (string[])tokenStrings.Clone();
        for (int i = 0; i < tokenStrings.Length; i++)
            result[i] = TranslateTokenStringToTarget(tokenStrings[i], gen);
        int lastTrueEntry = result.Length - 1;
        while (lastTrueEntry >= 0 && result[lastTrueEntry] == null)
            lastTrueEntry--;
        if (lastTrueEntry < result.Length - 1)
            result = Arrays.CopyOf(result, lastTrueEntry + 1);
        return Arrays.AsList(result);
    }

    protected static string TranslateTokenStringToTarget(string tokenName, CodeGenerator gen)
    {
        if (tokenName == null) return null;
        if (tokenName[(0)] == '\'')
        {
            var targetString =
                gen.                Target.GetTargetStringLiteralFromANTLRStringLiteral(gen, tokenName, false, true);
            return "\"'" + targetString + "'\"";
        }
        else
        {
            return gen.Target.GetTargetStringLiteralFromString(tokenName, true);
        }
    }
}
