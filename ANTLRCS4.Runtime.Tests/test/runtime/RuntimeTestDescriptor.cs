/*
 * Copyright (c) 2012-2022 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime.misc;

namespace org.antlr.v4.test.runtime;

/** This object represents all the information we need about a single test and is the
 * in-memory representation of a descriptor file
 */
public class RuntimeTestDescriptor
{
    /** A type in {"Lexer", "Parser", "CompositeLexer", "CompositeParser"} */
    public readonly GrammarType testType;

    /** Return a string representing the name of the target currently testing
	 *  this descriptor.
	 *  Multiple instances of the same descriptor class
	 *  can be created to test different targets.
	 */
    public readonly string name;

    public readonly string notes;

    /** Parser input. Return "" if not input should be provided to the parser or lexer. */
    public readonly string input;

    /** Output from executing the parser. Return null if no output is expected. */
    public readonly string output;

    /** Parse errors Return null if no errors are expected. */
    public readonly string errors;

    /** The rule at which parsing should start */
    public readonly string startRule;
    public readonly string grammarName;

    public readonly string grammar;
    /** List of grammars imported into the grammar */
    public readonly List<Pair<string, string>> slaveGrammars;

    /** For lexical tests, dump the DFA of the default lexer mode to stdout */
    public readonly bool showDFA;

    /** For parsing, engage the DiagnosticErrorListener, dumping results to stderr */
    public readonly bool showDiagnosticErrors;

    public readonly string[] skipTargets;

    public readonly string uri;

    public RuntimeTestDescriptor(GrammarType testType, string name, string notes,
                                 string input, string output, string errors,
                                 string startRule,
                                 string grammarName, string grammar, List<Pair<string, string>> slaveGrammars,
                                 bool showDFA, bool showDiagnosticErrors, string[] skipTargets,
                                 string uri)
    {
        this.testType = testType;
        this.name = name;
        this.notes = notes;
        this.input = input;
        this.output = output;
        this.errors = errors;
        this.startRule = startRule;
        this.grammarName = grammarName;
        this.grammar = grammar;
        this.slaveGrammars = slaveGrammars;
        this.showDFA = showDFA;
        this.showDiagnosticErrors = showDiagnosticErrors;
        this.skipTargets = skipTargets != null ? skipTargets : new string[0];
        this.uri = uri;
    }

    /** Return true if this test should be ignored for the indicated target */
    public bool Ignore(string targetName) => Arrays.AsList(skipTargets).Contains(targetName);

    public override string ToString() => name;
}
