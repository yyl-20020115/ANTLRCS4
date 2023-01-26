/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */
namespace org.antlr.v4.test.tool;


public class TestCodeGeneration {
	[TestMethod] public void testArgDecl(){ // should use template not string
		/*ErrorQueue equeue = */new ErrorQueue();
		String g =
				"grammar T;\n" +
				"a[int xyz] : 'a' ;\n";
		List<String> evals = getEvalInfoForString(g, "int xyz");
		System.out.println(evals);
		for (int i = 0; i < evals.size(); i++) {
			String eval = evals.get(i);
			assertFalse(eval.startsWith("<pojo:"), "eval should not be POJO: "+eval);
		}
	}

	[TestMethod] public void AssignTokenNamesToStringLiteralsInGeneratedParserRuleContexts(){
		String g =
			"grammar T;\n" +
			"root: 't1';\n" +
			"Token: 't1';";
		List<String> evals = getEvalInfoForString(g, "() { return getToken(");
		assertNotEquals(0, evals.size());
	}

	[TestMethod] public void AssignTokenNamesToStringLiteralArraysInGeneratedParserRuleContexts(){
		String g =
			"grammar T;\n" +
				"root: 't1' 't1';\n" +
				"Token: 't1';";
		List<String> evals = getEvalInfoForString(g, "() { return getTokens(");
		assertNotEquals(0, evals.size());
	}

	/** Add tags around each attribute/template/value write */
	public static class DebugInterpreter extends Interpreter {
		List<String> evals = new ArrayList<String>();
		ErrorManager myErrMgrCopy;
		int tab = 0;
		public DebugInterpreter(STGroup group, ErrorManager errMgr, boolean debug) {
			super(group, errMgr, debug);
			myErrMgrCopy = errMgr;
		}

		@Override
		protected int writeObject(STWriter out, InstanceScope scope, Object o, String[] options) {
			if ( o is ST ) {
				String name = ((ST)o).getName();
				name = name.substring(1);
				if ( !name.startsWith("_sub") ) {
					try {
						out.write("<ST:" + name + ">");
						evals.add("<ST:" + name + ">");
						int r = super.writeObject(out, scope, o, options);
						out.write("</ST:" + name + ">");
						evals.add("</ST:" + name + ">");
						return r;
					} catch (IOException ioe) {
						myErrMgrCopy.IOError(scope.st, ErrorType.WRITE_IO_ERROR, ioe);
					}
				}
			}
			return super.writeObject(out, scope, o, options);
		}

		@Override
		protected int writePOJO(STWriter out, InstanceScope scope, Object o, String[] options){
			Class<?> type = o.getClass();
			String name = type.getSimpleName();
			out.write("<pojo:"+name+">"+o.ToString()+"</pojo:"+name+">");
			evals.add("<pojo:" + name + ">" + o.ToString() + "</pojo:" + name + ">");
			return super.writePOJO(out, scope, o, options);
		}

		public void indent(STWriter out){
			for (int i=1; i<=tab; i++) {
				out.write("\t");
			}
		}
	}

	public List<String> getEvalInfoForString(String grammarString, String pattern)  {
		ErrorQueue equeue = new ErrorQueue();
		Grammar g = new Grammar(grammarString);
		List<String> evals = new ArrayList<String>();
		if ( g.ast!=null && !g.ast.hasErrors ) {
			SemanticPipeline sem = new SemanticPipeline(g);
			sem.process();

			ATNFactory factory = new ParserATNFactory(g);
			if (g.isLexer()) factory = new LexerATNFactory((LexerGrammar) g);
			g.atn = factory.createATN();

			CodeGenerator gen = CodeGenerator.create(g);
			ST outputFileST = gen.generateParser();

//			STViz viz = outputFileST.inspect();
//			try {
//				viz.waitForClose();
//			}
//			catch (Exception e) {
//				e.printStackTrace();
//			}

			boolean debug = false;
			DebugInterpreter interp =
					new DebugInterpreter(outputFileST.groupThatCreatedThisInstance,
							outputFileST.impl.nativeGroup.errMgr,
							debug);
			InstanceScope scope = new InstanceScope(null, outputFileST);
			StringWriter sw = new StringWriter();
			AutoIndentWriter out = new AutoIndentWriter(sw);
			interp.exec(out, scope);

			for (String e : interp.evals) {
				if (e.contains(pattern)) {
					evals.add(e);
				}
			}
		}
		if ( equeue.size()>0 ) {
			System.err.println(equeue.ToString());
		}
		return evals;
	}
}
