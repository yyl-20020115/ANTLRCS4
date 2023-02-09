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

[TestClass]
public class TestCodeGeneration
{
    [TestMethod]
    public void TestArgDecl()
    { // should use template not string
        /*ErrorQueue equeue = */
        var e = new ErrorQueue();
        var g =
                "grammar T;\n" +
                "a[int xyz] : 'a' ;\n";
        var evals = GetEvalInfoForString(g, "int xyz");
        Console.Out.WriteLine(evals);
        for (int i = 0; i < evals.Count; i++)
        {
            var eval = evals[i];
            Assert.IsFalse(eval.StartsWith("<pojo:"), "eval should not be POJO: " + eval);
        }
    }

    [TestMethod]
    public void AssignTokenNamesToStringLiteralsInGeneratedParserRuleContexts()
    {
        var g =
            "grammar T;\n" +
            "root: 't1';\n" +
            "Token: 't1';";
        var evals = GetEvalInfoForString(g, "() { return getToken(");
        Assert.AreNotEqual(0, evals.Count);
    }

    [TestMethod]
    public void AssignTokenNamesToStringLiteralArraysInGeneratedParserRuleContexts()
    {
        var g =
            "grammar T;\n" +
                "root: 't1' 't1';\n" +
                "Token: 't1';";
        var evals = GetEvalInfoForString(g, "() { return getTokens(");
        Assert.AreNotEqual(0, evals.Count);
    }

    /** Add tags around each attribute/template/value write */
    public class DebugInterpreter : Interpreter
    {
        public List<string> evals = new();
        Antlr4.StringTemplate.Misc.ErrorManager ErrMgrCopy;
        int tab = 0;
        public DebugInterpreter(TemplateGroup group, Antlr4.StringTemplate.Misc.ErrorManager errMgr, bool debug)
        : base(group, errMgr, debug)
        {
            ErrMgrCopy = errMgr;
        }

        //@Override
        protected override int WriteObject(ITemplateWriter @out, TemplateFrame scope, object o, string[] options)
        {
            if (o is Template template)
            {
                var name = template.Name;
                name = name.Substring(1);
                if (!name.StartsWith("_sub"))
                {
                    try
                    {
                        @out.Write("<ST:" + name + ">");
                        evals.Add("<ST:" + name + ">");
                        int r = base.WriteObject(@out, scope, o, options);
                        @out.Write("</ST:" + name + ">");
                        evals.Add("</ST:" + name + ">");
                        return r;
                    }
                    catch (IOException ioe)
                    {
                        ErrMgrCopy.IOError(scope.Template, Antlr4.StringTemplate.Misc.ErrorType.WRITE_IO_ERROR, ioe);
                    }
                }
            }
            return base.WriteObject(@out, scope, o, options);
        }
#if false
		//@Override
		protected int writePOJO(ITemplateWriter @out, TemplateFrame scope, object o, string[] options){
			Type type = o.GetType();
			var name = type.Name;
			@out.Write("<pojo:"+name+">"+o.ToString()+"</pojo:"+name+">");
			evals.Add("<pojo:" + name + ">" + o.ToString() + "</pojo:" + name + ">");
			return base.WritePOJO(@out, scope, o, options);
		}
#endif
        public void Indent(ITemplateWriter @out)
        {
            for (int i = 1; i <= tab; i++)
            {
                @out.Write("\t");
            }
        }
    }

    public static List<string> GetEvalInfoForString(string grammarString, string pattern)
    {
        var equeue = new ErrorQueue();
        var g = new Grammar(grammarString);
        List<string> evals = new();
        if (g.ast != null && !g.ast.hasErrors)
        {
            var sem = new SemanticPipeline(g);
            sem.Process();

            var factory = new ParserATNFactory(g);
            if (g.IsLexer) factory = new LexerATNFactory((LexerGrammar)g);
            g.atn = factory.CreateATN();

            var gen = CodeGenerator.Create(g);
            var outputFileST = gen.GenerateParser();

            //			STViz viz = outputFileST.inspect();
            //			try {
            //				viz.waitForClose();
            //			}
            //			catch (Exception e) {
            //				e.printStackTrace();
            //			}

            bool debug = false;
            var interp =
                    new DebugInterpreter(outputFileST.Group,
                            outputFileST.impl.NativeGroup.ErrorManager,
                            debug);
            var scope = new TemplateFrame(outputFileST, null);
            var sw = new StringWriter();
            var @out = new AutoIndentWriter(sw);
            interp.Execute(@out, scope);

            foreach (var e in interp.evals)
            {
                if (e.Contains(pattern))
                {
                    evals.Add(e);
                }
            }
        }
        if (equeue.Count > 0)
        {
            Console.Error.WriteLine(equeue.ToString());
        }
        return evals;
    }
}
