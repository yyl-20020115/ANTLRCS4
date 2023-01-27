/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.codegen.model;
using org.antlr.v4.codegen.model.chunk;
using org.antlr.v4.tool;

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
			Type c = Type.GetType(targetName).asSubclass(Target);
			ConstructorInfo ctor = c.getConstructor(CodeGenerator);
			CodeGenerator codeGenerator = new CodeGenerator(tool, g, language);
			codeGenerator.target = ctor.newInstance(codeGenerator);
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

	public STGroup getTemplates() {
		return target.getTemplates();
	}

	// CREATE TEMPLATES BY WALKING MODEL

	private OutputModelController createController() {
		OutputModelFactory factory = new ParserFactory(this);
		OutputModelController controller = new OutputModelController(factory);
		factory.setController(controller);
		return controller;
	}

	private ST walk(OutputModelObject outputModel, bool header) {
		OutputModelWalker walker = new OutputModelWalker(tool, getTemplates());
		return walker.walk(outputModel, header);
	}

	public ST generateLexer() { return generateLexer(false); }
	public ST generateLexer(bool header) { return walk(createController().buildLexerOutputModel(header), header); }

	public ST generateParser() { return generateParser(false); }
	public ST generateParser(bool header) { return walk(createController().buildParserOutputModel(header), header); }

	public ST generateListener() { return generateListener(false); }
	public ST generateListener(bool header) { return walk(createController().buildListenerOutputModel(header), header); }

	public ST generateBaseListener() { return generateBaseListener(false); }
	public ST generateBaseListener(bool header) { return walk(createController().buildBaseListenerOutputModel(header), header); }

	public ST generateVisitor() { return generateVisitor(false); }
	public ST generateVisitor(bool header) { return walk(createController().buildVisitorOutputModel(header), header); }

	public ST generateBaseVisitor() { return generateBaseVisitor(false); }
	public ST generateBaseVisitor(bool header) { return walk(createController().buildBaseVisitorOutputModel(header), header); }

	/** Generate a token vocab file with all the token names/types.  For example:
	 *  ID=7
	 *  FOR=8
	 *  'for'=8
	 *
	 *  This is independent of the target language; used by antlr internally
	 */
	ST getTokenVocabOutput() {
		ST vocabFileST = new ST(vocabFilePattern);
		Map<String,int> tokens = new LinkedHashMap<String,int>();
        // make constants for the token names
        foreach (String t in g.tokenNameToTypeMap.keySet()) {
			int tokenType = g.tokenNameToTypeMap.get(t);
			if ( tokenType>=Token.MIN_USER_TOKEN_TYPE) {
				tokens.put(t, tokenType);
			}
		}
		vocabFileST.add("tokens", tokens);

		// now dump the strings
		Map<String,int> literals = new LinkedHashMap<String,int>();
		foreach (String literal in g.stringLiteralToTypeMap.keySet()) {
			int tokenType = g.stringLiteralToTypeMap.get(literal);
			if ( tokenType>=Token.MIN_USER_TOKEN_TYPE) {
				literals.put(literal, tokenType);
			}
		}
		vocabFileST.add("literals", literals);

		return vocabFileST;
	}

	public void writeRecognizer(ST outputFileST, bool header) {
		target.genFile(g, outputFileST, getRecognizerFileName(header));
	}

	public void writeListener(ST outputFileST, bool header) {
		target.genFile(g, outputFileST, getListenerFileName(header));
	}

	public void writeBaseListener(ST outputFileST, bool header) {
		target.genFile(g, outputFileST, getBaseListenerFileName(header));
	}

	public void writeVisitor(ST outputFileST, bool header) {
		target.genFile(g, outputFileST, getVisitorFileName(header));
	}

	public void writeBaseVisitor(ST outputFileST, bool header) {
		target.genFile(g, outputFileST, getBaseVisitorFileName(header));
	}

	public void writeVocabFile() {
		// write out the vocab interchange file; used by antlr,
		// does not change per target
		ST tokenVocabSerialization = getTokenVocabOutput();
		String fileName = getVocabFileName();
		if ( fileName!=null ) {
			target.genFile(g, tokenVocabSerialization, fileName);
		}
	}

	public void write(ST code, String fileName) {
		try {
//			long start = System.currentTimeMillis();
			Writer w = tool.getOutputFileWriter(g, fileName);
			STWriter wr = new AutoIndentWriter(w);
			wr.setLineWidth(lineWidth);
			code.write(wr);
			w.close();
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
		ST extST = getTemplates().getInstanceOf("headerFileExtension");
		if ( extST==null ) return null;
		String recognizerName = g.getRecognizerName();
		return recognizerName+extST.render();
	}

}
