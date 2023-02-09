/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using Antlr4.StringTemplate;
using org.antlr.v4.codegen.model;
using org.antlr.v4.runtime;
using org.antlr.v4.tool;

namespace org.antlr.v4.codegen;

/** General controller for code gen.  Can instantiate sub generator(s).
 */
public class CodeGenerator
{
    public static readonly string TEMPLATE_ROOT = "org/antlr/v4/tool/templates/codegen";
    public static readonly string VOCAB_FILE_EXTENSION = ".tokens";
    public static readonly string vocabFilePattern =
        "<tokens.keys:{t | <t>=<tokens.(t)>\n}>" +
        "<literals.keys:{t | <t>=<literals.(t)>\n}>";

    public readonly Grammar g;

    public readonly Tool tool;

    public readonly string language;

    private Target target;

    public int lineWidth = 72;

    public static CodeGenerator Create(Grammar g) 
        => Create(g.Tools, g, g.getLanguage());

    public static CodeGenerator Create(Tool tool, Grammar g, string language)
    {
        var targetName = "org.antlr.v4.codegen.target." + language + "Target";
        try
        {
            var c = Type.GetType(targetName);
            var ctor = c.GetConstructor(new Type[] { typeof(CodeGenerator) });
            var codeGenerator = new CodeGenerator(tool, g, language);
            codeGenerator.target = ctor.Invoke(new object[] { codeGenerator }) as Target;
            return codeGenerator;
        }
        catch (Exception e)
        {
            g.Tools.ErrMgr.ToolError(ErrorType.CANNOT_CREATE_TARGET_GENERATOR, e, language);
            return null;
        }
    }

    private CodeGenerator(Tool tool, Grammar g, string language)
    {
        this.g = g;
        this.tool = tool;
        this.language = language;
    }

    public Target Target => target;

    public TemplateGroup Templates => target.GetTemplates();

    // CREATE TEMPLATES BY WALKING MODEL

    private OutputModelController CreateController()
    {
        var factory = new ParserFactory(this);
        var controller = new OutputModelController(factory);
        factory.Controller = controller;
        return controller;
    }

    private Template Walk(OutputModelObject outputModel, bool header)
    {
        var walker = new OutputModelWalker(tool, Templates);
        return walker.Walk(outputModel, header);
    }

    public Template GenerateLexer() => GenerateLexer(false);
    public Template GenerateLexer(bool header) => Walk(CreateController().BuildLexerOutputModel(header), header);

    public Template GenerateParser() => GenerateParser(false);
    public Template GenerateParser(bool header) => Walk(CreateController().BuildParserOutputModel(header), header);

    public Template GenerateListener() => GenerateListener(false);
    public Template GenerateListener(bool header) => Walk(CreateController().BuildListenerOutputModel(header), header);

    public Template GenerateBaseListener() => GenerateBaseListener(false);
    public Template GenerateBaseListener(bool header) => Walk(CreateController().BuildBaseListenerOutputModel(header), header);

    public Template GenerateVisitor() => GenerateVisitor(false);
    public Template GenerateVisitor(bool header) => Walk(CreateController().BuildVisitorOutputModel(header), header);

    public Template GenerateBaseVisitor() => GenerateBaseVisitor(false);
    public Template GenerateBaseVisitor(bool header) => Walk(CreateController().BuildBaseVisitorOutputModel(header), header);

    /** Generate a token vocab file with all the token names/types.  For example:
	 *  ID=7
	 *  FOR=8
	 *  'for'=8
	 *
	 *  This is independent of the target language; used by antlr internally
	 */
    Template GetTokenVocabOutput()
    {
        var vocabFileST = new Template(vocabFilePattern);
        var tokens = new Dictionary<string, int>();
        // make constants for the token names
        foreach (var t in g.tokenNameToTypeMap.Keys)
        {
            int tokenType = g.tokenNameToTypeMap[t];
            if (tokenType >= Token.MIN_USER_TOKEN_TYPE)
            {
                tokens[t] = tokenType;
            }
        }
        vocabFileST.Add("tokens", tokens);

        // now dump the strings
        var literals = new Dictionary<string, int>();
        foreach (var literal in g.stringLiteralToTypeMap.Keys)
        {
            int tokenType = g.stringLiteralToTypeMap[literal];
            if (tokenType >= Token.MIN_USER_TOKEN_TYPE)
            {
                literals[literal] = tokenType;
            }
        }
        vocabFileST.Add("literals", literals);

        return vocabFileST;
    }

    public void WriteRecognizer(Template outputFileST, bool header) => target.GenFile(g, outputFileST, GetRecognizerFileName(header));

    public void WriteListener(Template outputFileST, bool header) => target.GenFile(g, outputFileST, GetListenerFileName(header));

    public void WriteBaseListener(Template outputFileST, bool header) => target.GenFile(g, outputFileST, GetBaseListenerFileName(header));

    public void WriteVisitor(Template outputFileST, bool header) => target.GenFile(g, outputFileST, GetVisitorFileName(header));

    public void WriteBaseVisitor(Template outputFileST, bool header) => target.GenFile(g, outputFileST, GetBaseVisitorFileName(header));

    public void WriteVocabFile()
    {
        // write out the vocab interchange file; used by antlr,
        // does not change per target
        var tokenVocabSerialization = GetTokenVocabOutput();
        var fileName = GetVocabFileName();
        if (fileName != null)
        {
            target.GenFile(g, tokenVocabSerialization, fileName);
        }
    }

    public void Write(Template code, string fileName)
    {
        try
        {
            //			long start = System.currentTimeMillis();
            var w = tool.GetOutputFileWriter(g, fileName);
            var wr = new AutoIndentWriter(w)
            {
                LineWidth = (lineWidth)
            };
            code.Write(wr);
            w.Close();
            //			long stop = System.currentTimeMillis();
        }
        catch (IOException ioe)
        {
            tool.ErrMgr.ToolError(ErrorType.CANNOT_WRITE_FILE,
                                  ioe,
                                  fileName);
        }
    }

    public string GetRecognizerFileName() => GetRecognizerFileName(false);
    public string GetListenerFileName() => GetListenerFileName(false);
    public string GetVisitorFileName() => GetVisitorFileName(false);
    public string GetBaseListenerFileName() => GetBaseListenerFileName(false);
    public string GetBaseVisitorFileName() => GetBaseVisitorFileName(false);
    public string GetRecognizerFileName(bool header) => target.GetRecognizerFileName(header);
    public string GetListenerFileName(bool header) => target.GetListenerFileName(header);
    public string GetVisitorFileName(bool header) => target.GetVisitorFileName(header);
    public string GetBaseListenerFileName(bool header) => target.GetBaseListenerFileName(header);
    public string GetBaseVisitorFileName(bool header) => target.GetBaseVisitorFileName(header);

    /** What is the name of the vocab file generated for this grammar?
	 *  Returns null if no .tokens file should be generated.
	 */
    public string GetVocabFileName() => g.name + VOCAB_FILE_EXTENSION;
    public string GetHeaderFileName()
    {
        var extST = Templates.GetInstanceOf("headerFileExtension");
        if (extST == null) return null;
        var recognizerName = g.GetRecognizerName();
        return recognizerName + extST.Render();
    }

}
