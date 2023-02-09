/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.runtime;
using org.antlr.v4.analysis;
using org.antlr.v4.automata;
using org.antlr.v4.codegen;
using org.antlr.v4.misc;
using org.antlr.v4.parse;
using org.antlr.v4.runtime;
using org.antlr.v4.runtime.atn;
using org.antlr.v4.runtime.misc;
using org.antlr.v4.semantics;
using org.antlr.v4.tool;
using org.antlr.v4.tool.ast;
using System.Text;

namespace org.antlr.v4;

public class Tool
{
    public static readonly string VERSION = RuntimeMetaData.VERSION;
    static Tool()
    {
        // Assigned in a static{} block to prevent the field from becoming a
        // compile-time constant
    }

    public static readonly string GRAMMAR_EXTENSION = ".g4";
    public static readonly string LEGACY_GRAMMAR_EXTENSION = ".g";

    public static readonly List<string> ALL_GRAMMAR_EXTENSIONS =
        new () { GRAMMAR_EXTENSION, LEGACY_GRAMMAR_EXTENSION };

    public enum OptionArgType :uint { NONE, STRING } // NONE implies boolean
    public class Option
    {
        public readonly string fieldName;
        public readonly string name;
        public readonly OptionArgType argType;
        public readonly string description;

        public Option(string fieldName, string name, string description)
        : this(fieldName, name, OptionArgType.NONE, description) { }

        public Option(string fieldName, string name, OptionArgType argType, string description)
        {
            this.fieldName = fieldName;
            this.name = name;
            this.argType = argType;
            this.description = description;
        }
    }

    // fields set by option manager

    public string inputDirectory; // used by mvn plugin but not set by tool itself.
    public string outputDirectory;
    public string libDirectory;
    public bool generate_ATN_dot = false;
    public Encoding grammarEncoding = Encoding.Default; // use default locale's encoding
    public string msgFormat = "antlr";
    public bool Launch_ST_inspector = false;
    public bool ST_inspector_wait_for_close = false;
    public bool force_atn = false;
    public bool dolog = false;
    public bool gen_listener = true;
    public bool gen_visitor = false;
    public bool gen_dependencies = false;
    public string genPackage = null;
    public Dictionary<string, string> grammarOptions = null;
    public bool warnings_are_errors = false;
    public bool longMessages = false;
    public bool exact_output_dir = false;

    public readonly static Option[] optionDefs = {
        new Option("outputDirectory",             "-o", OptionArgType.STRING, "specify output directory where all output is generated"),
        new Option("libDirectory",                "-lib", OptionArgType.STRING, "specify location of grammars, tokens files"),
        new Option("generate_ATN_dot",            "-atn", "generate rule augmented transition network diagrams"),
        new Option("grammarEncoding",             "-encoding", OptionArgType.STRING, "specify grammar file encoding; e.g., euc-jp"),
        new Option("msgFormat",                   "-message-format", OptionArgType.STRING, "specify output style for messages in antlr, gnu, vs2005"),
        new Option("longMessages",                "-long-messages", "show exception details when available for errors and warnings"),
        new Option("gen_listener",                "-listener", "generate parse tree listener (default)"),
        new Option("gen_listener",                "-no-listener", "don't generate parse tree listener"),
        new Option("gen_visitor",                 "-visitor", "generate parse tree visitor"),
        new Option("gen_visitor",                 "-no-visitor", "don't generate parse tree visitor (default)"),
        new Option("genPackage",                  "-package", OptionArgType.STRING, "specify a package/namespace for the generated code"),
        new Option("gen_dependencies",            "-depend", "generate file dependencies"),
        new Option("",                            "-D<option>=value", "set/override a grammar-level option"),
        new Option("warnings_are_errors",         "-Werror", "treat warnings as errors"),
        new Option("launch_ST_inspector",         "-XdbgST", "launch StringTemplate visualizer on generated code"),
        new Option("ST_inspector_wait_for_close", "-XdbgSTWait", "wait for STViz to close before continuing"),
        new Option("force_atn",                   "-Xforce-atn", "use the ATN simulator for all predictions"),
        new Option("log",                         "-Xlog", "dump lots of logging info to antlr-timestamp.log"),
        new Option("exact_output_dir",            "-Xexact-output-dir", "all output goes into -o dir regardless of paths/package"),
    };

    // helper vars for option management
    protected bool haveOutputDir = false;
    protected bool return_dont_exit = false;


    public readonly string[] args;

    protected List<string> grammarFiles = new();

    public ErrorManager ErrMgr;
    public LogManager logMgr = new();
    readonly List<ANTLRToolListener> listeners = new ();// CopyOnWriteArrayList<ANTLRToolListener>();

    /** Track separately so if someone adds a listener, it's the only one
	 *  instead of it and the default stderr listener.
	 */
    readonly DefaultToolListener defaultListener;

    public Tool()
    {
        defaultListener = new DefaultToolListener(this);
    }

    public static void Main(string[] args)
    {
        var antlr = new Tool(args);
        if (args.Length == 0) { antlr.Help(); Exit(0); }

        try
        {
            antlr.ProcessGrammarsOnCommandLine();
        }
        finally
        {
            if (antlr.dolog)
            {
                try
                {
                    var logname = antlr.logMgr.Save();
                    Console.WriteLine("wrote " + logname);
                }
                catch (IOException ioe)
                {
                    antlr.ErrMgr.ToolError(ErrorType.INTERNAL_ERROR, ioe);
                }
            }
        }
        if (antlr.return_dont_exit) return;

        if (antlr.ErrMgr.NumErrors > 0)
        {
            Exit(1);
        }
        Exit(0);
    }

    public Tool(params string[] args)
    {
        this.args = args;
        ErrMgr = new ErrorManager(this);
        // We have to use the default message format until we have
        // parsed the -message-format command line option.
        ErrMgr.SetFormat("antlr");
        HandleArgs();
        ErrMgr.SetFormat(msgFormat);
    }

    protected void HandleArgs()
    {
        int i = 0;
        while (args != null && i < args.Length)
        {
            var arg = args[i];
            i++;
            if (arg.StartsWith("-D"))
            { // -Dlanguage=Java syntax
                HandleOptionSetArg(arg);
                continue;
            }
            if (arg[(0)] != '-')
            { // file name
                if (!grammarFiles.Contains(arg)) grammarFiles.Add(arg);
                continue;
            }
            var found = false;
            foreach (var o in optionDefs)
            {
                if (arg.Equals(o.name))
                {
                    found = true;
                    string argValue = null;
                    if (o.argType == OptionArgType.STRING)
                    {
                        argValue = args[i];
                        i++;
                    }
                    // use reflection to set field
                    var c = this.GetType();
                    try
                    {
                        var f = c.GetField(o.fieldName);
                        if (argValue == null)
                        {
                            if (arg.StartsWith("-no-")) f.SetValue(this, false);
                            else f.SetValue(this, true);
                        }
                        else f.SetValue(this, argValue);
                    }
                    catch (Exception e)
                    {
                        ErrMgr.ToolError(ErrorType.INTERNAL_ERROR, "can't access field " + o.fieldName);
                    }
                }
            }
            if (!found)
            {
                ErrMgr.ToolError(ErrorType.INVALID_CMDLINE_ARG, arg);
            }
        }
        if (outputDirectory != null)
        {
            if (outputDirectory.EndsWith("/") ||
                outputDirectory.EndsWith("\\"))
            {
                outputDirectory =
                    outputDirectory.Substring(0, outputDirectory.Length - 1);
            }
            var outDir = outputDirectory;
            haveOutputDir = true;
            if (File.Exists(outDir) && !Directory.Exists(outDir))
            {
                ErrMgr.ToolError(ErrorType.OUTPUT_DIR_IS_FILE, outputDirectory);
                outputDirectory = ".";
            }
        }
        else
        {
            outputDirectory = ".";
        }
        if (libDirectory != null)
        {
            if (libDirectory.EndsWith("/") ||
                libDirectory.EndsWith("\\"))
            {
                libDirectory = libDirectory.Substring(0, libDirectory.Length - 1);
            }
            var outDir = (libDirectory);
            if (!File.Exists(outDir))
            {
                ErrMgr.ToolError(ErrorType.DIR_NOT_FOUND, libDirectory);
                libDirectory = ".";
            }
        }
        else
        {
            libDirectory = ".";
        }
        if (Launch_ST_inspector)
        {

            //TemplateGroup.trackCreationEvents = true;
            return_dont_exit = true;
        }
    }

    protected void HandleOptionSetArg(string arg)
    {
        int eq = arg.IndexOf('=');
        if (eq > 0 && arg.Length > 3)
        {
            var option = arg[2/*"-D".Length*/..eq];
            var value = arg[(eq + 1)..];
            if (value.Length == 0)
            {
                ErrMgr.ToolError(ErrorType.BAD_OPTION_SET_SYNTAX, arg);
                return;
            }
            if (Grammar.parserOptions.Contains(option) ||
                 Grammar.lexerOptions.Contains(option))
            {
                if (grammarOptions == null) grammarOptions = new();
                grammarOptions[option] = value;
            }
            else
            {
                ErrMgr.GrammarError(ErrorType.ILLEGAL_OPTION,
                                    null,
                                    null,
                                    option);
            }
        }
        else
        {
            ErrMgr.ToolError(ErrorType.BAD_OPTION_SET_SYNTAX, arg);
        }
    }

    public void ProcessGrammarsOnCommandLine()
    {
        var sortedGrammars = SortGrammarByTokenVocab(grammarFiles);

        foreach (var t in sortedGrammars)
        {
            var g = CreateGrammar(t);
            g.fileName = t.fileName;
            if (gen_dependencies)
            {
                var dep =
                    new BuildDependencyGenerator(this, g);
                /*
					List outputFiles = dep.getGeneratedFileList();
					List dependents = dep.getDependenciesFileList();
					Console.WriteLine("output: "+outputFiles);
					Console.WriteLine("dependents: "+dependents);
					 */
                Console.WriteLine(dep.GetDependencies().Render());

            }
            else if (ErrMgr.NumErrors == 0)
            {
                Process(g, true);
            }
        }
    }

    /** To process a grammar, we load all of its imported grammars into
		subordinate grammar objects. Then we merge the imported rules
		into the root grammar. If a root grammar is a combined grammar,
		we have to extract the implicit lexer. Once all this is done, we
		process the lexer first, if present, and then the parser grammar
	 */
    public void Process(Grammar g, bool gencode)
    {
        g.LoadImportedGrammars();

        var transform = new GrammarTransformPipeline(g, this);
        transform.Process();

        LexerGrammar lexerg;
        GrammarRootAST lexerAST;
        if (g.ast != null && g.ast.grammarType == ANTLRParser.COMBINED &&
             !g.ast.hasErrors)
        {
            lexerAST = transform.ExtractImplicitLexer(g); // alters g.ast
            if (lexerAST != null)
            {
                if (grammarOptions != null)
                {
                    lexerAST.cmdLineOptions = grammarOptions;
                }

                lexerg = new LexerGrammar(this, lexerAST)
                {
                    fileName = g.fileName,
                    originalGrammar = g
                };
                g.implicitLexer = lexerg;
                lexerg.implicitLexerOwner = g;
                ProcessNonCombinedGrammar(lexerg, gencode);
                //				Console.WriteLine("lexer tokens="+lexerg.tokenNameToTypeMap);
                //				Console.WriteLine("lexer strings="+lexerg.stringLiteralToTypeMap);
            }
        }
        if (g.implicitLexer != null) g.ImportVocab(g.implicitLexer);
        //		Console.WriteLine("tokens="+g.tokenNameToTypeMap);
        //		Console.WriteLine("strings="+g.stringLiteralToTypeMap);
        ProcessNonCombinedGrammar(g, gencode);
    }

    public void ProcessNonCombinedGrammar(Grammar g, bool gencode)
    {
        if (g.ast == null || g.ast.hasErrors) return;

        bool ruleFail = CheckForRuleIssues(g);
        if (ruleFail) return;

        int prevErrors = ErrMgr.NumErrors;
        // MAKE SURE GRAMMAR IS SEMANTICALLY CORRECT (FILL IN GRAMMAR OBJECT)
        var sem = new SemanticPipeline(g);
        sem.Process();

        if (ErrMgr.NumErrors > prevErrors) return;

        var codeGenerator = CodeGenerator.Create(g);
        if (codeGenerator == null)
        {
            return;
        }

        // BUILD ATN FROM AST
        ATNFactory factory;
        if (g.IsLexer) factory = new LexerATNFactory((LexerGrammar)g, codeGenerator);
        else factory = new ParserATNFactory(g);
        g.atn = factory.CreateATN();

        if (generate_ATN_dot) GenerateATNs(g);

        if (gencode && g.Tools.getNumErrors() == 0)
        {
            var interpFile = GenerateInterpreterData(g);
            using var fw = GetOutputFileWriter(g, g.name + ".interp");
            fw.Write(interpFile);
            //catch (IOException ioe) {
            //	errMgr.toolError(ErrorType.CANNOT_WRITE_FILE, ioe);
            //}
        }

        // PERFORM GRAMMAR ANALYSIS ON ATN: BUILD DECISION DFAs
        var anal = new AnalysisPipeline(g);
        anal.Process();

        //if ( generate_DFA_dot ) generateDFAs(g);

        if (g.Tools.getNumErrors() > prevErrors) return;

        // GENERATE CODE
        if (gencode)
        {
            var gen = new CodeGenPipeline(g, codeGenerator);
            gen.Process();
        }
    }
    // check for undefined rules
    class UndefChecker : GrammarTreeVisitor
    {
        public Tool tool;

        public bool badref = false;

        public Dictionary<string, RuleAST> ruleToAST;

        public UndefChecker(Tool tool, Dictionary<string, RuleAST> ruleToAST)
        {
            this.tool = tool;
            this.ruleToAST = ruleToAST;
        }
        public override void TokenRef(TerminalAST @ref)
        {
            if ("EOF".Equals(@ref.Text))
            {
                // this is a special predefined reference
                return;
            }

            if (tool.gx.IsLexer) RuleRef(@ref, null);
        }

        public override void RuleRef(GrammarAST @ref, ActionAST arg)
        {
            var ruleAST = ruleToAST.TryGetValue(@ref.Text, out var ret) ? ret : null;
            var fileName = @ref.Token.InputStream.SourceName;
            if (char.IsUpper(currentRuleName[(0)]) &&
                char.IsLower(@ref.Text[(0)]))
            {
                badref = true;
                this.tool.ErrMgr.GrammarError(ErrorType.PARSER_RULE_REF_IN_LEXER_RULE,
                                    fileName, @ref.Token, @ref.Text, currentRuleName);
            }
            else if (ruleAST == null)
            {
                badref = true;
                this.tool.ErrMgr.GrammarError(ErrorType.UNDEFINED_RULE_REF,
                                    fileName, @ref.token, @ref.Text);
            }
        }
        public override ErrorManager ErrorManager => this.tool.ErrMgr;
    }


    /**
	 * Important enough to avoid multiple definitions that we do very early,
	 * right after AST construction. Also check for undefined rules in
	 * parser/lexer to avoid exceptions later. Return true if we find multiple
	 * definitions of the same rule or a reference to an undefined rule or
	 * parser rule ref in lexer rule.
	 */
    Grammar gx;
    public bool CheckForRuleIssues(Grammar g)
    {
        // check for redefined rules
        var RULES = (GrammarAST)g.ast.GetFirstChildWithType(ANTLRParser.RULES);
        List<GrammarAST> rules = new(RULES.GetAllChildrenWithType(ANTLRParser.RULE));
        foreach (var mode in g.ast.GetAllChildrenWithType(ANTLRParser.MODE))
        {
            rules.AddRange(mode.GetAllChildrenWithType(ANTLRParser.RULE));
        }

        bool redefinition = false;
        Dictionary<string, RuleAST> ruleToAST = new();
        foreach (var r in rules)
        {
            RuleAST ruleAST = (RuleAST)r;
            var ID = (GrammarAST)ruleAST.GetChild(0);
            var ruleName = ID.Text;
            if (ruleToAST.TryGetValue(ruleName, out var prev))
            {
                var prevChild = (GrammarAST)prev.GetChild(0);
                g.Tools.ErrMgr.GrammarError(ErrorType.RULE_REDEFINITION,
                                           g.fileName,
                                           ID.Token,
                                           ruleName,
                                           prevChild.Token.Line);
                redefinition = true;
                continue;
            }
            ruleToAST[ruleName] = ruleAST;
        }
        gx = g;
        var chk = new UndefChecker(this, ruleToAST);
        chk.VisitGrammar(g.ast);

        return redefinition || chk.badref;
    }

    public List<GrammarRootAST> SortGrammarByTokenVocab(List<string> fileNames)
    {
        //		Console.WriteLine(fileNames);
        Graph<string> g = new ();
        List<GrammarRootAST> roots = new();
        foreach (var fileName in fileNames)
        {
            var t = ParseGrammar(fileName);
            if (t == null || t is GrammarASTErrorNode) continue; // came back as error node
            if (((GrammarRootAST)t).hasErrors) continue;
            GrammarRootAST root = (GrammarRootAST)t;
            roots.Add(root);
            root.fileName = fileName;
            var grammarName = root.GetChild(0).Text;

            var tokenVocabNode = FindOptionValueAST(root, "tokenVocab");
            // Make grammars depend on any tokenVocab options
            if (tokenVocabNode != null)
            {
                var vocabName = tokenVocabNode.Text;
                // Strip quote characters if any
                int len = vocabName.Length;
                int firstChar = vocabName[(0)];
                int lastChar = vocabName[len - 1];
                if (len >= 2 && firstChar == '\'' && lastChar == '\'')
                {
                    vocabName = vocabName.Substring(1, len - 1 - 1);
                }
                // If the name Contains a path delimited by forward slashes,
                // use only the part after the last slash as the name
                int lastSlash = vocabName.LastIndexOf('/');
                if (lastSlash >= 0)
                {
                    vocabName = vocabName.Substring(lastSlash + 1);
                }
                g.AddEdge(grammarName, vocabName);
            }
            // add cycle to graph so we always process a grammar if no error
            // even if no dependency
            g.AddEdge(grammarName, grammarName);
        }

        var sortedGrammarNames = g.Sort();
        //		Console.WriteLine("sortedGrammarNames="+sortedGrammarNames);

        List<GrammarRootAST> sortedRoots = new();
        foreach (var grammarName in sortedGrammarNames)
        {
            foreach (var root in roots)
            {
                if (root.GetGrammarName().Equals(grammarName))
                {
                    sortedRoots.Add(root);
                    break;
                }
            }
        }

        return sortedRoots;
    }

    /** Manually get option node from tree; return null if no defined. */
    public static GrammarAST FindOptionValueAST(GrammarRootAST root, string option)
    {
        var options = (GrammarAST)root.GetFirstChildWithType(ANTLRParser.OPTIONS);
        if (options != null && options.ChildCount > 0)
        {
            foreach (var o in options.GetChildren())
            {
                var c = (GrammarAST)o;
                if (c.Type == ANTLRParser.ASSIGN &&
                     c.GetChild(0).Text.Equals(option))
                {
                    return (GrammarAST)c.GetChild(1);
                }
            }
        }
        return null;
    }


    /** Given the raw AST of a grammar, create a grammar object
		associated with the AST. Once we have the grammar object, ensure
		that all nodes in tree referred to this grammar. Later, we will
		use it for error handling and generally knowing from where a rule
		comes from.
	 */
    public Grammar CreateGrammar(GrammarRootAST ast)
    {
        var g = ast.grammarType == ANTLRParser.LEXER ? new LexerGrammar(this, ast) : new Grammar(this, ast);

        // ensure each node has pointer to surrounding grammar
        GrammarTransformPipeline.SetGrammarPtr(g, ast);
        return g;
    }

    public GrammarRootAST ParseGrammar(string fileName)
    {
        try
        {
            var file = fileName;
            if (!Path.IsPathFullyQualified(fileName))
            {
                file = Path.Combine(inputDirectory, fileName);
            }

            var @in = new ANTLRFileStream(file, grammarEncoding);
            var t = Parse(fileName, @in);
            return t;
        }
        catch (IOException ioe)
        {
            ErrMgr.ToolError(ErrorType.CANNOT_OPEN_FILE, ioe, fileName);
        }
        return null;
    }

    /** Convenience method to load and process an ANTLR grammar. Useful
	 *  when creating interpreters.  If you need to access to the lexer
	 *  grammar created while processing a combined grammar, use
	 *  getImplicitLexer() on returned grammar.
	 */
    public Grammar LoadGrammar(string fileName)
    {
        var grammarRootAST = ParseGrammar(fileName);
        var g = CreateGrammar(grammarRootAST);
        g.fileName = fileName;
        Process(g, false);
        return g;
    }

    private readonly Dictionary<string, Grammar> importedGrammars = new();

    /**
	 * Try current dir then dir of g then lib dir
	 * @param g
	 * @param nameNode The node associated with the imported grammar name.
	 */
    public Grammar LoadImportedGrammar(Grammar g, GrammarAST nameNode)
    {
        var name = nameNode.Text;
        if (!importedGrammars.TryGetValue(name, out var imported))
        {
            g.Tools.Log("grammar", "load " + name + " from " + g.fileName);
            string importedFile = null;
            foreach (var extension in ALL_GRAMMAR_EXTENSIONS)
            {
                importedFile = GetImportedGrammarFile(g, name + extension);
                if (importedFile != null)
                {
                    break;
                }
            }

            if (importedFile == null)
            {
                ErrMgr.GrammarError(ErrorType.CANNOT_FIND_IMPORTED_GRAMMAR, g.fileName, nameNode.Token, name);
                return null;
            }

            var absolutePath = importedFile;
            var @in = new ANTLRFileStream(absolutePath, grammarEncoding);
            var root = Parse(g.fileName, @in);
            if (root == null)
            {
                return null;
            }

            imported = CreateGrammar(root);
            imported.fileName = absolutePath;
            importedGrammars[root.GetGrammarName()] = imported;
        }

        return imported;
    }

    public GrammarRootAST ParseGrammarFromString(string grammar)
    {
        return Parse("<string>", new ANTLRStringStream(grammar));
    }

    public GrammarRootAST Parse(string fileName, CharStream @in)
    {
        try
        {
            var adaptor = new GrammarASTAdaptor(@in);
            var lexer = new ToolANTLRLexer(@in, this);
            var tokens = new CommonTokenStream(lexer);
            lexer.tokens = tokens;
            var p = new ToolANTLRParser(tokens, this);
            p.TreeAdaptor = adaptor;
            var r = p.GrammarSpec();
            GrammarAST root = (GrammarAST)r.Tree;
            if (root is GrammarRootAST aST)
            {
                aST.hasErrors = lexer.GetNumberOfSyntaxErrors() > 0 || p.GetNumberOfSyntaxErrors() > 0;
                //assert ((GrammarRootAST) root).tokenStream == tokens;
                if (grammarOptions != null)
                {
                    aST.cmdLineOptions = grammarOptions;
                }
                return aST;
            }
            return null;
        }
        catch (RecognitionException re)
        {
            // TODO: do we gen errors now?
            ErrorManager.InternalError("can't generate this message at moment; antlr recovers");
        }
        return null;
    }

    public void GenerateATNs(Grammar g)
    {
        var dotGenerator = new DOTGenerator(g);
        List<Grammar> grammars = new();
        grammars.Add(g);
        var imported = g.GetAllImportedGrammars();
        if (imported != null) grammars.AddRange(imported);
        foreach (var ig in grammars)
        {
            foreach (var r in ig.rules.Values)
            {
                try
                {
                    var dot = dotGenerator.GetDOT(g.atn.ruleToStartState[r.index], g.IsLexer);
                    if (dot != null)
                    {
                        WriteDOTFile(g, r, dot);
                    }
                }
                catch (IOException ioe)
                {
                    ErrMgr.ToolError(ErrorType.CANNOT_WRITE_FILE, ioe);
                }
            }
        }
    }

    public static string GenerateInterpreterData(Grammar g)
    {
        var content = new StringBuilder();

        content.Append("token literal names:\n");
        var names = g.GetTokenLiteralNames();
        foreach (var name in names)
        {
            content.Append(name + "\n");
        }
        content.Append('\n');

        content.Append("token symbolic names:\n");
        names = g.GetTokenSymbolicNames();
        foreach (var name in names)
        {
            content.Append(name + "\n");
        }
        content.Append('\n');

        content.Append("rule names:\n");
        names = g.GetRuleNames();
        foreach (var name in names)
        {
            content.Append(name + "\n");
        }
        content.Append('\n');

        if (g.IsLexer)
        {
            content.Append("channel names:\n");
            content.Append("DEFAULT_TOKEN_CHANNEL\n");
            content.Append("HIDDEN\n");
            foreach (var channel in g.channelValueToNameList)
            {
                content.Append(channel + "\n");
            }
            content.Append('\n');

            content.Append("mode names:\n");
            foreach (var mode in ((LexerGrammar)g).modes.Keys)
            {
                content.Append(mode + "\n");
            }
        }
        content.Append('\n');

        var serializedATN = ATNSerializer.GetSerialized(g.atn);
        // Uncomment if you'd like to write out histogram info on the numbers of
        // each integer value:
        //Utils.writeSerializedATNIntegerHistogram(g.name+"-histo.csv", serializedATN);

        content.Append("atn:\n");
        content.Append(serializedATN.ToString());

        return content.ToString();
    }

    /** This method is used by all code generators to create new output
	 *  files. If the outputDir set by -o is not present it will be created.
	 *  The final filename is sensitive to the output directory and
	 *  the directory where the grammar file was found.  If -o is /tmp
	 *  and the original grammar file was foo/t.g4 then output files
	 *  go in /tmp/foo.
	 *
	 *  The output dir -o spec takes precedence if it's absolute.
	 *  E.g., if the grammar file dir is absolute the output dir is given
	 *  precedence. "-o /tmp /usr/lib/t.g4" results in "/tmp/T.java" as
	 *  output (assuming t.g4 holds T.java).
	 *
	 *  If no -o is specified, then just write to the directory where the
	 *  grammar file was found.
	 *
	 *  If outputDirectory==null then write a string.
	 */
    public TextWriter GetOutputFileWriter(Grammar g, string fileName)
    {
        if (outputDirectory == null)
        {
            return new StringWriter();
        }
        // output directory is a function of where the grammar file lives
        // for subdir/T.g4, you get subdir here.  Well, depends on -o etc...
        var outputDir = GetOutputDirectory(g.fileName);
        var outputFile = Path.Combine(outputDir, fileName);

        if (!File.Exists(outputDir))
        {
            Directory.CreateDirectory(outputDir);
        }
        TextWriter osw = null;
        if (grammarEncoding != null)
        {
            osw = new StreamWriter(outputFile, false, grammarEncoding);
        }
        else
        {
            osw = new StreamWriter(outputFile);
        }
        return osw;
    }

    public string GetImportedGrammarFile(Grammar g, string fileName)
    {
        var importedFile = Path.Combine(inputDirectory, fileName);
        if (!File.Exists(importedFile))
        {
            var gfile = (g.fileName);
            var parentDir = gfile;
            importedFile = Path.Combine(parentDir, fileName);
            if (!File.Exists(importedFile))
            { // try in lib dir
                importedFile = Path.Combine(libDirectory, fileName);
                if (!File.Exists(importedFile))
                {
                    return null;
                }
            }
        }
        return importedFile;
    }

    /**
	 * Return the location where ANTLR will generate output files for a given
	 * file. This is a base directory and output files will be relative to
	 * here in some cases such as when -o option is used and input files are
	 * given relative to the input directory.
	 *
	 * @param fileNameWithPath path to input source
	 */
    public string GetOutputDirectory(string fileNameWithPath)
    {
        if (exact_output_dir)
        {
            return NewGetOutputDirectory(fileNameWithPath);
        }

        string outputDir;
        string fileDirectory;

        // Some files are given to us without a PATH but should should
        // still be written to the output directory in the relative path of
        // the output directory. The file directory is either the set of sub directories
        // or just or the relative path recorded for the parent grammar. This means
        // that when we write the tokens files, or the .java files for imported grammars
        // taht we will write them in the correct place.
        if ((fileNameWithPath == null) || (fileNameWithPath.LastIndexOf(Path.DirectorySeparatorChar) == -1))
        {
            // No path is included in the file name, so make the file
            // directory the same as the parent grammar (which might sitll be just ""
            // but when it is not, we will write the file in the correct place.
            fileDirectory = ".";

        }
        else
        {
            fileDirectory = fileNameWithPath.Substring(0, fileNameWithPath.LastIndexOf(Path.DirectorySeparatorChar));
        }
        if (haveOutputDir)
        {
            // -o /tmp /var/lib/t.g4 => /tmp/T.java
            // -o subdir/output /usr/lib/t.g4 => subdir/output/T.java
            // -o . /usr/lib/t.g4 => ./T.java
            if (fileDirectory != null &&

                    fileDirectory.StartsWith("~"))
            { // isAbsolute doesn't count this :(
              // somebody set the dir, it takes precendence; write new file there
                outputDir = (outputDirectory);
            }
            else
            {
                // -o /tmp subdir/t.g4 => /tmp/subdir/T.java
                if (fileDirectory != null)
                {
                    outputDir = Path.Combine(outputDirectory, fileDirectory);
                }
                else
                {
                    outputDir = (outputDirectory);
                }
            }
        }
        else
        {
            // they didn't specify a -o dir so just write to location
            // where grammar is, absolute or relative, this will only happen
            // with command line invocation as build tools will always
            // supply an output directory.
            outputDir = (fileDirectory);
        }
        return outputDir;
    }

    /** @since 4.7.1 in response to -Xexact-output-dir */
    public string NewGetOutputDirectory(string fileNameWithPath)
    {
        string outputDir;
        string fileDirectory;

        if (fileNameWithPath.LastIndexOf(Path.DirectorySeparatorChar) == -1)
        {
            // No path is included in the file name, so make the file
            // directory the same as the parent grammar (which might still be just ""
            // but when it is not, we will write the file in the correct place.
            fileDirectory = ".";
        }
        else
        {
            fileDirectory = fileNameWithPath.Substring(0, fileNameWithPath.LastIndexOf(Path.DirectorySeparatorChar));
        }
        if (haveOutputDir)
        {
            // -o /tmp /var/lib/t.g4 => /tmp/T.java
            // -o subdir/output /usr/lib/t.g4 => subdir/output/T.java
            // -o . /usr/lib/t.g4 => ./T.java
            // -o /tmp subdir/t.g4 => /tmp/T.java
            outputDir = outputDirectory;
        }
        else
        {
            // they didn't specify a -o dir so just write to location
            // where grammar is, absolute or relative, this will only happen
            // with command line invocation as build tools will always
            // supply an output directory.
            outputDir = fileDirectory;
        }
        return outputDir;
    }

    protected void WriteDOTFile(Grammar g, Rule r, string dot)
    {
        WriteDOTFile(g, r.g.name + "." + r.name, dot);
    }

    protected void WriteDOTFile(Grammar g, string name, string dot)
    {
        var fw = GetOutputFileWriter(g, name + ".dot");
        try
        {
            fw.Write(dot);
        }
        finally
        {
            fw.Close();
        }
    }

    public void Help()
    {
        Info("ANTLR Parser Generator  Version " + Tool.VERSION);
        foreach (var o in optionDefs)
        {
            var name = o.name + (o.argType != OptionArgType.NONE ? " ___" : "");
            var s = $"{name} {o.description}";// string.format(" %-19s %s", name, o.description);
            Info(s);
        }
    }

    public void Log(string component, string msg) => logMgr.Log(component, msg);
    public void Log(string msg) => Log(null, msg);

    public int getNumErrors() => ErrMgr.NumErrors;

    public void addListener(ANTLRToolListener tl)
    {
        if (tl != null) listeners.Add(tl);
    }
    public void removeListener(ANTLRToolListener tl) { listeners.Remove(tl); }
    public void removeListeners() { listeners.Clear(); }
    public List<ANTLRToolListener> getListeners() { return listeners; }

    public void Info(string msg)
    {
        if (listeners.Count == 0)
        {
            defaultListener.Info(msg);
            return;
        }
        foreach (ANTLRToolListener l in listeners) l.Info(msg);
    }
    public void Error(ANTLRMessage msg)
    {
        if (listeners.Count == 0)
        {
            defaultListener.Error(msg);
            return;
        }
        foreach (ANTLRToolListener l in listeners) l.Error(msg);
    }
    public void Warning(ANTLRMessage msg)
    {
        if (listeners.Count == 0)
        {
            defaultListener.Warning(msg);
        }
        else
        {
            foreach (var l in listeners) l.Warning(msg);
        }

        if (warnings_are_errors)
        {
            ErrMgr.Emit(ErrorType.WARNING_TREATED_AS_ERROR, new ANTLRMessage(ErrorType.WARNING_TREATED_AS_ERROR));
        }
    }

    public void Version()
    {
        Info("ANTLR Parser Generator  Version " + VERSION);
    }

    public static void Exit(int e) => Environment.Exit(e);

    public static void Panic() => throw new Error("ANTLR panic");

}
