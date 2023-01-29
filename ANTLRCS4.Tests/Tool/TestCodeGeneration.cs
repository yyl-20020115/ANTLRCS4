/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */
using Antlr4.StringTemplate;
using org.antlr.v4.automata;
using org.antlr.v4.codegen;
using org.antlr.v4.semantics;
using org.antlr.v4.test.runtime;
using org.antlr.v4.tool;

namespace org.antlr.v4.test.tool;


public class TestCodeGeneration {
	[TestMethod] public void testArgDecl(){ // should use template not string
		/*ErrorQueue equeue = */new ErrorQueue();
		String g =
				"grammar T;\n" +
				"a[int xyz] : 'a' ;\n";
		List<String> evals = getEvalInfoForString(g, "int xyz");
		Console.Out.WriteLine(evals);
		for (int i = 0; i < evals.Count; i++) {
			String eval = evals[i];
			Assert.IsFalse(eval.StartsWith("<pojo:"), "eval should not be POJO: "+eval);
		}
	}

	[TestMethod] public void AssignTokenNamesToStringLiteralsInGeneratedParserRuleContexts(){
		String g =
			"grammar T;\n" +
			"root: 't1';\n" +
			"Token: 't1';";
		List<String> evals = getEvalInfoForString(g, "() { return getToken(");
		Assert.AreNotEqual(0, evals.Count);
	}

	[TestMethod] public void AssignTokenNamesToStringLiteralArraysInGeneratedParserRuleContexts(){
		String g =
			"grammar T;\n" +
				"root: 't1' 't1';\n" +
				"Token: 't1';";
		List<String> evals = getEvalInfoForString(g, "() { return getTokens(");
		Assert.AreNotEqual(0, evals.Count);
	}

	/** Add tags around each attribute/template/value write */
	public class DebugInterpreter : Interpreter {
		public List<String> evals = new ();
	 	Antlr4.StringTemplate.Misc.ErrorManager myErrMgrCopy;
		int tab = 0;
		public DebugInterpreter(TemplateGroup group, Antlr4.StringTemplate.Misc.ErrorManager errMgr, bool debug)
		: base(group, errMgr, debug)
        {
			;
			myErrMgrCopy = errMgr;
		}

		//@Override
		protected int writeObject(ITemplateWriter @out, TemplateFrame scope, Object o, String[] options) {
			if ( o is Template ) {
				String name = ((Template)o).Name;
				name = name.Substring(1);
				if ( !name.StartsWith("_sub") ) {
					try {
						@out.Write("<ST:" + name + ">");
						evals.Add("<ST:" + name + ">");
						int r = base.WriteObject(@out, scope, o, options);
						@out.Write("</ST:" + name + ">");
						evals.Add("</ST:" + name + ">");
						return r;
					} catch (IOException ioe) {
						myErrMgrCopy.IOError(scope.Template,Antlr4.StringTemplate.Misc.ErrorType.WRITE_IO_ERROR, ioe);
					}
				}
			}
			return base.WriteObject(@out, scope, o, options);
		}
#if false
		//@Override
		protected int writePOJO(ITemplateWriter @out, TemplateFrame scope, Object o, String[] options){
			Type type = o.GetType();
			String name = type.Name;
			@out.Write("<pojo:"+name+">"+o.ToString()+"</pojo:"+name+">");
			evals.Add("<pojo:" + name + ">" + o.ToString() + "</pojo:" + name + ">");
			return base.WritePOJO(@out, scope, o, options);
		}
#endif
		public void indent(ITemplateWriter @out){
			for (int i=1; i<=tab; i++) {
				@out.Write("\t");
			}
		}
	}

	public List<String> getEvalInfoForString(String grammarString, String pattern)  {
		ErrorQueue equeue = new ErrorQueue();
		Grammar g = new Grammar(grammarString);
		List<String> evals = new ();
		if ( g.ast!=null && !g.ast.hasErrors ) {
			SemanticPipeline sem = new SemanticPipeline(g);
			sem.process();

			ATNFactory factory = new ParserATNFactory(g);
			if (g.isLexer()) factory = new LexerATNFactory((LexerGrammar) g);
			g.atn = factory.createATN();

			CodeGenerator gen = CodeGenerator.create(g);
			Template outputFileST = gen.generateParser();

//			STViz viz = outputFileST.inspect();
//			try {
//				viz.waitForClose();
//			}
//			catch (Exception e) {
//				e.printStackTrace();
//			}

			bool debug = false;
			DebugInterpreter interp =
					new DebugInterpreter(outputFileST.Group,
							outputFileST.impl.NativeGroup.ErrorManager,
							debug);
			TemplateFrame scope = new TemplateFrame( outputFileST,null);
			StringWriter sw = new StringWriter();
			AutoIndentWriter @out = new AutoIndentWriter(sw);
			interp.Execute(@out, scope);

			foreach (String e in interp.evals) {
				if (e.Contains(pattern)) {
					evals.Add(e);
				}
			}
		}
		if ( equeue.size()>0 ) {
			Console.Error.WriteLine(equeue.ToString());
		}
		return evals;
	}
}
