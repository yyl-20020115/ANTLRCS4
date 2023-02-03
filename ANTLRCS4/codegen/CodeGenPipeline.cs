/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */
using Antlr4.StringTemplate;
using org.antlr.v4.tool;

namespace org.antlr.v4.codegen;

public class CodeGenPipeline
{
    public readonly Grammar g;
    public readonly CodeGenerator gen;

    public CodeGenPipeline(Grammar g, CodeGenerator gen)
    {
        this.g = g;
        this.gen = gen;
    }

    public void Process()
    {
        // all templates are generated in memory to report the most complete
        // error information possible, but actually writing output files stops
        // after the first error is reported
        int errorCount = g.Tools.ErrMgr.getNumErrors();

        if (g.isLexer())
        {
            if (gen.Target.NeedsHeader())
            {
                var lexer2 = gen.GenerateLexer(true); // Header file if needed.
                if (g.Tools.ErrMgr.getNumErrors() == errorCount)
                {
                    WriteRecognizer(lexer2, gen, true);
                }
            }
            var lexer = gen.GenerateLexer(false);
            if (g.Tools.ErrMgr.getNumErrors() == errorCount)
            {
                WriteRecognizer(lexer, gen, false);
            }
        }
        else
        {
            if (gen.Target.NeedsHeader())
            {
                var parser2 = gen.GenerateParser(true);
                if (g.Tools.ErrMgr.getNumErrors() == errorCount)
                {
                    WriteRecognizer(parser2, gen, true);
                }
            }
            var parser = gen.GenerateParser(false);
            if (g.Tools.ErrMgr.getNumErrors() == errorCount)
            {
                WriteRecognizer(parser, gen, false);
            }

            if (g.Tools.gen_listener)
            {
                if (gen.Target.NeedsHeader())
                {
                    var listener2 = gen.GenerateListener(true);
                    if (g.Tools.ErrMgr.getNumErrors() == errorCount)
                    {
                        gen.WriteListener(listener2, true);
                    }
                }
                var listener3 = gen.GenerateListener(false);
                if (g.Tools.ErrMgr.getNumErrors() == errorCount)
                {
                    gen.WriteListener(listener3, false);
                }

                if (gen.Target.NeedsHeader())
                {
                    var baseListener = gen.GenerateBaseListener(true);
                    if (g.Tools.ErrMgr.getNumErrors() == errorCount)
                    {
                        gen.WriteBaseListener(baseListener, true);
                    }
                }
                if (gen.Target.WantsBaseListener())
                {
                    var baseListener = gen.GenerateBaseListener(false);
                    if (g.Tools.ErrMgr.getNumErrors() == errorCount)
                    {
                        gen.WriteBaseListener(baseListener, false);
                    }
                }
            }
            if (g.Tools.gen_visitor)
            {
                if (gen.Target.NeedsHeader())
                {
                    var visitor2 = gen.GenerateVisitor(true);
                    if (g.Tools.ErrMgr.getNumErrors() == errorCount)
                    {
                        gen.WriteVisitor(visitor2, true);
                    }
                }
                var visitor = gen.GenerateVisitor(false);
                if (g.Tools.ErrMgr.getNumErrors() == errorCount)
                {
                    gen.WriteVisitor(visitor, false);
                }

                if (gen.Target.NeedsHeader())
                {
                    var baseVisitor = gen.GenerateBaseVisitor(true);
                    if (g.Tools.ErrMgr.getNumErrors() == errorCount)
                    {
                        gen.WriteBaseVisitor(baseVisitor, true);
                    }
                }
                if (gen.Target.WantsBaseVisitor())
                {
                    var baseVisitor = gen.GenerateBaseVisitor(false);
                    if (g.Tools.ErrMgr.getNumErrors() == errorCount)
                    {
                        gen.WriteBaseVisitor(baseVisitor, false);
                    }
                }
            }
        }
        gen.WriteVocabFile();
    }

    protected void WriteRecognizer(Template template, CodeGenerator gen, bool header)
    {
        if (g.Tools.Launch_ST_inspector)
        {
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

        gen.WriteRecognizer(template, header);
    }
}
