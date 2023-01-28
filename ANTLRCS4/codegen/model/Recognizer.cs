/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */
using org.antlr.v4.codegen.model.chunk;
using org.antlr.v4.runtime.dfa;
using org.antlr.v4.runtime.misc;
using org.antlr.v4.tool;

namespace org.antlr.v4.codegen.model;

public abstract class Recognizer : OutputModelObject {
	public String name;
	public String grammarName;
	public String grammarFileName;
	public String accessLevel;
	public Dictionary<String,int> tokens;

    /**
	 * @deprecated This field is provided only for compatibility with code
	 * generation targets which have not yet been updated to use
	 * {@link #literalNames} and {@link #symbolicNames}.
	 */
    //@Deprecated
    public List<String> tokenNames;

	public List<String> literalNames;
	public List<String> symbolicNames;
	public HashSet<String> ruleNames;
	public ICollection<Rule> rules;
	//@ModelElement 
		public ActionChunk superClass;

    //@ModelElement 
    public SerializedATN atn;
    //@ModelElement 
    public Dictionary<Rule, RuleSempredFunction> sempredFuncs =
		new ();

	public Recognizer(OutputModelFactory factory) : base(factory)
    {

		Grammar g = factory.getGrammar();
		CodeGenerator gen = factory.getGenerator();
		grammarFileName = new File(g.fileName).getName();
		grammarName = g.name;
		name = g.getRecognizerName();
		accessLevel = g.getOptionString("accessLevel");
		tokens = new ();
        foreach (var entry in g.tokenNameToTypeMap) {
			int ttype = entry.getValue();
			if ( ttype>0 ) {
				tokens.put(entry.getKey(), ttype);
			}
		}

		ruleNames = g.rules.keySet();
		rules = g.rules.Values;
		if ( gen.getTarget() is JavaTarget ) {
			atn = new SerializedJavaATN(factory, g.atn);
		}
		else {
			atn = new SerializedATN(factory, g.atn);
		}
		if (g.getOptionString("superClass") != null) {
			superClass = new ActionText(null, g.getOptionString("superClass"));
		}
		else {
			superClass = null;
		}

		tokenNames = translateTokenStringsToTarget(g.getTokenDisplayNames(), gen);
		literalNames = translateTokenStringsToTarget(g.getTokenLiteralNames(), gen);
		symbolicNames = translateTokenStringsToTarget(g.getTokenSymbolicNames(), gen);
	}

	protected static List<String> translateTokenStringsToTarget(String[] tokenStrings, CodeGenerator gen) {
		String[] result = tokenStrings.clone();
		for (int i = 0; i < tokenStrings.Length; i++) {
			result[i] = translateTokenStringToTarget(tokenStrings[i], gen);
		}

		int lastTrueEntry = result.Length - 1;
		while (lastTrueEntry >= 0 && result[lastTrueEntry] == null) {
			lastTrueEntry --;
		}

		if (lastTrueEntry < result.Length - 1) {
			result = Arrays.CopyOf(result, lastTrueEntry + 1);
		}

		return Arrays.AsList(result);
	}

	protected static String translateTokenStringToTarget(String tokenName, CodeGenerator gen) {
		if (tokenName == null) {
			return null;
		}

		if (tokenName[(0)] == '\'') {
			String targetString =
				gen.getTarget().getTargetStringLiteralFromANTLRStringLiteral(gen, tokenName, false, true);
			return "\"'" + targetString + "'\"";
		}
		else {
			return gen.getTarget().getTargetStringLiteralFromString(tokenName, true);
		}
	}

}
