/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using Antlr4.StringTemplate;
using org.antlr.v4.codegen;
using org.antlr.v4.codegen.model.chunk;
using org.antlr.v4.parse;
using System.Text;

namespace org.antlr.v4.tool;


/** Given a grammar file, show the dependencies on .tokens etc...
 *  Using ST, emit a simple "make compatible" list of dependencies.
 *  For example, combined grammar T.g (no token import) generates:
 *
 *  	TParser.java : T.g
 *  	T.tokens : T.g
 *  	TLexer.java : T.g
 *
 *  If we are using the listener pattern (-listener on the command line)
 *  then we Add:
 *
 *      TListener.java : T.g
 *      TBaseListener.java : T.g
 *
 *  If we are using the visitor pattern (-visitor on the command line)
 *  then we Add:
 *
 *      TVisitor.java : T.g
 *      TBaseVisitor.java : T.g
 *
 *  If "-lib libdir" is used on command-line with -depend and option
 *  tokenVocab=A in grammar, then include the path like this:
 *
 * 		T.g: libdir/A.tokens
 *
 *  Pay attention to -o as well:
 *
 * 		outputdir/TParser.java : T.g
 *
 *  So this output shows what the grammar depends on *and* what it generates.
 *
 *  Operate on one grammar file at a time.  If given a list of .g on the
 *  command-line with -depend, just emit the dependencies.  The grammars
 *  may depend on each other, but the order doesn't matter.  Build tools,
 *  reading in this output, will know how to organize it.
 *
 *  This code was obvious until I removed redundant "./" on front of files
 *  and had to escape spaces in filenames :(
 *
 *  I literally copied from v3 so might be slightly inconsistent with the
 *  v4 code base.
 */
public class BuildDependencyGenerator {
    protected Tool tool;
    protected Grammar g;
    protected CodeGenerator generator;
    protected TemplateGroup templates;

    public BuildDependencyGenerator(Tool tool, Grammar g) {
        this.tool = tool;
		this.g = g;
		generator = CodeGenerator.create(g);
    }

    /** From T.g return a list of File objects that
     *  name files ANTLR will emit from T.g.
     */
    public List<string> getGeneratedFileList() {
        List<string> files = new ();

        // Add generated recognizer; e.g., TParser.java
        if (generator.getTarget().needsHeader()) {
          files.Add(getOutputFile(generator.getRecognizerFileName(true)));
        }
        files.Add(getOutputFile(generator.getRecognizerFileName(false)));
        // Add output vocab file; e.g., T.tokens. This is always generated to
        // the base output directory, which will be just . if there is no -o option
        //
		files.Add(getOutputFile(generator.getVocabFileName()));
        // are we generating a .h file?
        Template headerExtST = null;
        Template extST = generator.getTemplates().GetInstanceOf("codeFileExtension");
        if (generator.getTemplates().IsDefined("headerFile")) {
            headerExtST = generator.getTemplates().GetInstanceOf("headerFileExtension");
            String suffix = Grammar.getGrammarTypeToFileNameSuffix(g.getType());
            String fileName = g.name + suffix + headerExtST.Render();
            files.Add(getOutputFile(fileName));
        }
        if ( g.isCombined() ) {
            // Add autogenerated lexer; e.g., TLexer.java TLexer.h TLexer.tokens

            String suffix = Grammar.getGrammarTypeToFileNameSuffix(ANTLRParser.LEXER);
            String lexer = g.name + suffix + extST.Render();
            files.Add(getOutputFile(lexer));
            String lexerTokens = g.name + suffix + CodeGenerator.VOCAB_FILE_EXTENSION;
            files.Add(getOutputFile(lexerTokens));

            // TLexer.h
            if (headerExtST != null) {
                String header = g.name + suffix + headerExtST.Render();
                files.Add(getOutputFile(header));
            }
        }

        if ( g.tool.gen_listener ) {
          // Add generated listener; e.g., TListener.java
          if (generator.getTarget().needsHeader()) {
            files.Add(getOutputFile(generator.getListenerFileName(true)));
          }
          files.Add(getOutputFile(generator.getListenerFileName(false)));

          // Add generated base listener; e.g., TBaseListener.java
          if (generator.getTarget().needsHeader()) {
            files.Add(getOutputFile(generator.getBaseListenerFileName(true)));
          }
          files.Add(getOutputFile(generator.getBaseListenerFileName(false)));
        }

        if ( g.tool.gen_visitor ) {
          // Add generated visitor; e.g., TVisitor.java
          if (generator.getTarget().needsHeader()) {
            files.Add(getOutputFile(generator.getVisitorFileName(true)));
          }
          files.Add(getOutputFile(generator.getVisitorFileName(false)));

          // Add generated base visitor; e.g., TBaseVisitor.java
          if (generator.getTarget().needsHeader()) {
            files.Add(getOutputFile(generator.getBaseVisitorFileName(true)));
          }
          files.Add(getOutputFile(generator.getBaseVisitorFileName(false)));
        }


		// handle generated files for imported grammars
		List<Grammar> imports = g.getAllImportedGrammars();
		if ( imports!=null ) {
			foreach (Grammar g in imports) {
//				File outputDir = tool.getOutputDirectory(g.fileName);
//				String fname = groomQualifiedFileName(outputDir.toString(), g.getRecognizerName() + extST.render());
//				files.Add(new File(outputDir, fname));
				files.Add(getOutputFile(g.fileName));
			}
		}

		if (files.Count==0) {
			return null;
		}
		return files;
	}

	public string getOutputFile(String fileName) {
		string outputDir = tool.getOutputDirectory(g.fileName);
		if ( outputDir.Equals(".") ) {
			// pay attention to -o then
			outputDir = tool.getOutputDirectory(fileName);
		}
		if ( outputDir.Equals(".") ) {
			return (fileName);
		}
		if (outputDir.Equals(".")) {
			String fname = outputDir.ToString();
			int dot = fname.LastIndexOf('.');
			outputDir = (outputDir[(0)..dot]);
		}

		if (outputDir.IndexOf(' ') >= 0) { // has spaces?
			String escSpaces = outputDir.Replace(" ", "\\ ");
			outputDir = (escSpaces);
		}
		return Path.Combine(outputDir, fileName);
	}

    /**
     * Return a list of File objects that name files ANTLR will read
     * to process T.g; This can be .tokens files if the grammar uses the tokenVocab option
     * as well as any imported grammar files.
     */
    public List<string> getDependenciesFileList() {
        // Find all the things other than imported grammars
        List<string> files = getNonImportDependenciesFileList();

        // Handle imported grammars
        List<Grammar> imports = g.getAllImportedGrammars();
        if ( imports!=null ) {
			foreach (Grammar g in imports) {
				String libdir = tool.libDirectory;
				String fileName = groomQualifiedFileName(libdir, g.fileName);
				files.Add(fileName);
			}
		}

        if (files.Count==0) {
            return null;
        }
        return files;
    }

    /**
     * Return a list of File objects that name files ANTLR will read
     * to process T.g; This can only be .tokens files and only
     * if they use the tokenVocab option.
     *
     * @return List of dependencies other than imported grammars
     */
    public List<string> getNonImportDependenciesFileList() {
        List<string> files = new();

        // handle token vocabulary loads
        String tokenVocab = g.getOptionString("tokenVocab");
        if (tokenVocab != null) {
			String fileName =
				tokenVocab + CodeGenerator.VOCAB_FILE_EXTENSION;
			string vocabFile;
			if ( tool.libDirectory.Equals(".") ) {
				vocabFile = (fileName);
			}
			else {
				vocabFile = Path.Combine(tool.libDirectory, fileName);
			}
			files.Add(vocabFile);
		}

        return files;
    }

    public Template getDependencies() {
        loadDependencyTemplates();
        Template dependenciesST = templates.GetInstanceOf("dependencies");
        dependenciesST.Add("in", getDependenciesFileList());
        dependenciesST.Add("out", getGeneratedFileList());
        dependenciesST.Add("grammarFileName", g.fileName);
        return dependenciesST;
    }

    public void loadDependencyTemplates() {
        if (templates != null) return;
        String fileName = "org/antlr/v4/tool/templates/depend.stg";
        templates = new TemplateGroupFile(fileName, Encoding.UTF8);
    }

    public CodeGenerator getGenerator() {
        return generator;
    }

    public String groomQualifiedFileName(String outputDir, String fileName) {
        if (outputDir.Equals(".")) {
            return fileName;
        }
		else if (outputDir.IndexOf(' ') >= 0) { // has spaces?
            String escSpaces = outputDir.Replace(" ", "\\ ");
            return Path.Combine(escSpaces , fileName);
        }
		else {
            return Path.Combine(outputDir ,  fileName);
        }
    }
}
