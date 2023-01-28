/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using Antlr4.StringTemplate;
using org.antlr.v4.codegen.model;
using org.antlr.v4.codegen.model.chunk;
using org.antlr.v4.runtime;
using org.antlr.v4.tool;
using System.Reflection;

namespace org.antlr.v4.codegen;

/** General controller for code gen.  Can instantiate sub generator(s).
 */
public class CodeGenerator {
	public static readonly String TEMPLATE_ROOT = "org/antlr/v4/tool/templates/codegen";
	public static readonly String VOCAB_FILE_EXTENSION = ".tokens";
	public static readonly String vocabFilePattern =
		"<tokens.keys:{t | <t>=<tokens.(t)>\n}>" +
		"<literals.keys:{t | <t>=<literals.(t)>\n}>";

	public readonly Grammar g;

	public readonly Tool tool;

	public readonly String language;

	private Target target;

	public int lineWidth = 72;

	public static CodeGenerator create(Grammar g) {
		return create(g.tool, g, g.getLanguage());
	}

	public static CodeGenerator create(Tool tool, Grammar g, String language) {
		String targetName = "org.antlr.v4.codegen.target."+language+"Target";
		try {
			Type c = Type.GetType(targetName);
			ConstructorInfo ctor = c.GetConstructor(new Type[] {typeof(CodeGenerator)});
			CodeGenerator codeGenerator = new CodeGenerator(tool, g, language);
			codeGenerator.target = ctor.Invoke(new object[] { codeGenerator }) as Target;
			return codeGenerator;
		}
		catch (Exception e) {
			g.tool.errMgr.toolError(ErrorType.CANNOT_CREATE_TARGET_GENERATOR, e, language);
			return null;
		}
	}

	private CodeGenerator(Tool tool, Grammar g, String language) {
		this.g = g;
		this.tool = tool;
		this.language = language;
	}

	public Target getTarget() {
		return target;
	}

	public TemplateGroup getTemplates() {
		return target.getTemplates();
	}

	// CREATE TEMPLATES BY WALKING MODEL

	private OutputModelController createController() {
		OutputModelFactory factory = new ParserFactory(this);
		OutputModelController controller = new OutputModelController(factory);
		factory.setController(controller);
		return controller;
	}

	private Template walk(OutputModelObject outputModel, bool header) {
		OutputModelWalker walker = new OutputModelWalker(tool, getTemplates());
		return walker.walk(outputModel, header);
	}

	public Template generateLexer() { return generateLexer(false); }
	public Template generateLexer(bool header) { return walk(createController().buildLexerOutputModel(header), header); }

	public Template generateParser() { return generateParser(false); }
	public Template generateParser(bool header) { return walk(createController().buildParserOutputModel(header), header); }

	public Template generateListener() { return generateListener(false); }
	public Template generateListener(bool header) { return walk(createController().buildListenerOutputModel(header), header); }

	public Template generateBaseListener() { return generateBaseListener(false); }
	public Template generateBaseListener(bool header) { return walk(createController().buildBaseListenerOutputModel(header), header); }

	public Template generateVisitor() { return generateVisitor(false); }
	public Template generateVisitor(bool header) { return walk(createController().buildVisitorOutputModel(header), header); }

	public Template generateBaseVisitor() { return generateBaseVisitor(false); }
	public Template generateBaseVisitor(bool header) { return walk(createController().buildBaseVisitorOutputModel(header), header); }

    /** Generate a token vocab file with all the token names/types.  For example:
	 *  ID=7
	 *  FOR=8
	 *  'for'=8
	 *
	 *  This is independent of the target language; used by antlr internally
	 */
    Template getTokenVocabOutput() {
		Template vocabFileST = new Template(vocabFilePattern);
		Dictionary<String,int> tokens = new Dictionary<String,int>();
        // make constants for the token names
        foreach (String t in g.tokenNameToTypeMap.Keys) {
			int tokenType = g.tokenNameToTypeMap[t];
			if ( tokenType>=Token.MIN_USER_TOKEN_TYPE) {
				tokens[t]= tokenType;
			}
		}
		vocabFileST.Add("tokens", tokens);

		// now dump the strings
		Dictionary<String,int> literals = new Dictionary<String,int>();
		foreach (String literal in g.stringLiteralToTypeMap.Keys) {
			int tokenType = g.stringLiteralToTypeMap[literal];
			if ( tokenType>=Token.MIN_USER_TOKEN_TYPE) {
				literals[literal] = tokenType;
			}
		}
		vocabFileST.Add("literals", literals);

		return vocabFileST;
	}

	public void writeRecognizer(Template outputFileST, bool header) {
		target.genFile(g, outputFileST, getRecognizerFileName(header));
	}

	public void writeListener(Template outputFileST, bool header) {
		target.genFile(g, outputFileST, getListenerFileName(header));
	}

	public void writeBaseListener(Template outputFileST, bool header) {
		target.genFile(g, outputFileST, getBaseListenerFileName(header));
	}

	public void writeVisitor(Template outputFileST, bool header) {
		target.genFile(g, outputFileST, getVisitorFileName(header));
	}

	public void writeBaseVisitor(Template outputFileST, bool header) {
		target.genFile(g, outputFileST, getBaseVisitorFileName(header));
	}

	public void writeVocabFile() {
		// write out the vocab interchange file; used by antlr,
		// does not change per target
		Template tokenVocabSerialization = getTokenVocabOutput();
		String fileName = getVocabFileName();
		if ( fileName!=null ) {
			target.genFile(g, tokenVocabSerialization, fileName);
		}
	}

	public void write(Template code, String fileName) {
		try {
//			long start = System.currentTimeMillis();
			TextWriter w = tool.getOutputFileWriter(g, fileName);
			ITemplateWriter wr = new AutoIndentWriter(w);
			wr.LineWidth=(lineWidth);
			code.Write(wr);
			w.Close();
//			long stop = System.currentTimeMillis();
		}
		catch (IOException ioe) {
			tool.errMgr.toolError(ErrorType.CANNOT_WRITE_FILE,
								  ioe,
								  fileName);
		}
	}

	public String getRecognizerFileName() { return getRecognizerFileName(false); }
	public String getListenerFileName() { return getListenerFileName(false); }
	public String getVisitorFileName() { return getVisitorFileName(false); }
	public String getBaseListenerFileName() { return getBaseListenerFileName(false); }
	public String getBaseVisitorFileName() { return getBaseVisitorFileName(false); }

	public String getRecognizerFileName(bool header) { return target.getRecognizerFileName(header); }
	public String getListenerFileName(bool header) { return target.getListenerFileName(header); }
	public String getVisitorFileName(bool header) { return target.getVisitorFileName(header); }
	public String getBaseListenerFileName(bool header) { return target.getBaseListenerFileName(header); }
	public String getBaseVisitorFileName(bool header) { return target.getBaseVisitorFileName(header); }

	/** What is the name of the vocab file generated for this grammar?
	 *  Returns null if no .tokens file should be generated.
	 */
	public String getVocabFileName() {
		return g.name+VOCAB_FILE_EXTENSION;
	}

	public String getHeaderFileName() {
        Template extST = getTemplates().GetInstanceOf("headerFileExtension");
		if ( extST==null ) return null;
		String recognizerName = g.getRecognizerName();
		return recognizerName+extST.Render();
	}

}
