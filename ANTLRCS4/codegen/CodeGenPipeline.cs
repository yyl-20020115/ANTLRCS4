/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */
using Antlr4.StringTemplate;
using org.antlr.v4.codegen.model.chunk;
using org.antlr.v4.tool;

namespace org.antlr.v4.codegen;

public class CodeGenPipeline {
	readonly Grammar g;
	readonly CodeGenerator gen;

	public CodeGenPipeline(Grammar g, CodeGenerator gen) {
		this.g = g;
		this.gen = gen;
	}

	public void process() {
		// all templates are generated in memory to report the most complete
		// error information possible, but actually writing output files stops
		// after the first error is reported
		int errorCount = g.tool.errMgr.getNumErrors();

		if ( g.isLexer() ) {
			if (gen.getTarget().needsHeader()) {
                Template lexer2 = gen.generateLexer(true); // Header file if needed.
				if (g.tool.errMgr.getNumErrors() == errorCount) {
					writeRecognizer(lexer2, gen, true);
				}
			}
            Template lexer = gen.generateLexer(false);
			if (g.tool.errMgr.getNumErrors() == errorCount) {
				writeRecognizer(lexer, gen, false);
			}
		}
		else {
			if (gen.getTarget().needsHeader()) {
                Template parser2 = gen.generateParser(true);
				if (g.tool.errMgr.getNumErrors() == errorCount) {
					writeRecognizer(parser2, gen, true);
				}
			}
            Template parser = gen.generateParser(false);
			if (g.tool.errMgr.getNumErrors() == errorCount) {
				writeRecognizer(parser, gen, false);
			}

			if ( g.tool.gen_listener ) {
				if (gen.getTarget().needsHeader()) {
                    Template listener2 = gen.generateListener(true);
					if (g.tool.errMgr.getNumErrors() == errorCount) {
						gen.writeListener(listener2, true);
					}
				}
                Template listener3 = gen.generateListener(false);
				if (g.tool.errMgr.getNumErrors() == errorCount) {
					gen.writeListener(listener3, false);
				}

				if (gen.getTarget().needsHeader()) {
                    Template baseListener = gen.generateBaseListener(true);
					if (g.tool.errMgr.getNumErrors() == errorCount) {
						gen.writeBaseListener(baseListener, true);
					}
				}
				if (gen.getTarget().wantsBaseListener()) {
                    Template baseListener = gen.generateBaseListener(false);
					if ( g.tool.errMgr.getNumErrors()==errorCount ) {
						gen.writeBaseListener(baseListener, false);
					}
				}
			}
			if ( g.tool.gen_visitor ) {
				if (gen.getTarget().needsHeader()) {
                    Template visitor2 = gen.generateVisitor(true);
					if (g.tool.errMgr.getNumErrors() == errorCount) {
						gen.writeVisitor(visitor2, true);
					}
				}
                Template visitor = gen.generateVisitor(false);
				if (g.tool.errMgr.getNumErrors() == errorCount) {
					gen.writeVisitor(visitor, false);
				}

				if (gen.getTarget().needsHeader()) {
                    Template baseVisitor = gen.generateBaseVisitor(true);
					if (g.tool.errMgr.getNumErrors() == errorCount) {
						gen.writeBaseVisitor(baseVisitor, true);
					}
				}
				if (gen.getTarget().wantsBaseVisitor()) {
                    Template baseVisitor = gen.generateBaseVisitor(false);
					if ( g.tool.errMgr.getNumErrors()==errorCount ) {
						gen.writeBaseVisitor(baseVisitor, false);
					}
				}
			}
		}
		gen.writeVocabFile();
	}

	protected void writeRecognizer(Template template, CodeGenerator gen, bool header) {
		if ( g.tool.launch_ST_inspector ) {
			//NOTICE: not supported
			//STViz viz = template.Inspect();
			//if (g.tool.ST_inspector_wait_for_close) {
			//	try {
			//		viz.waitForClose();
			//	}
			//	catch (Exception ex) {
			//		g.tool.errMgr.toolError(ErrorType.INTERNAL_ERROR, ex);
			//	}
			//}
		}

		gen.writeRecognizer(template, header);
	}
}
