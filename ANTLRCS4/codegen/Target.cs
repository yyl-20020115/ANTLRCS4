/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using Antlr4.StringTemplate;
using Antlr4.StringTemplate.Misc;
using org.antlr.v4.misc;
using org.antlr.v4.parse;
using org.antlr.v4.runtime;
using org.antlr.v4.tool;
using org.antlr.v4.tool.ast;
using System.Text;

namespace org.antlr.v4.codegen;

/** */
public abstract class Target
{
    private readonly static Dictionary<string, TemplateGroup> languageTemplates = new();

    protected readonly CodeGenerator gen;

    protected static readonly Dictionary<char, string> defaultCharValueEscape;
    static Target()
    {
        // https://docs.oracle.com/javase/tutorial/java/data/characters.html
        Dictionary<char, String> map = new();
        AddEscapedChar(map, '\t', 't');
        AddEscapedChar(map, '\b', 'b');
        AddEscapedChar(map, '\n', 'n');
        AddEscapedChar(map, '\r', 'r');
        AddEscapedChar(map, '\f', 'f');
        AddEscapedChar(map, '\'');
        AddEscapedChar(map, '\"');
        AddEscapedChar(map, '\\');
        defaultCharValueEscape = map;
    }

    protected Target(CodeGenerator gen)
    {
        this.gen = gen;
    }

    /** For pure strings of Unicode char, how can we display
	 *  it in the target language as a literal. Useful for dumping
	 *  predicates and such that may refer to chars that need to be escaped
	 *  when represented as strings.  Also, templates need to be escaped so
	 *  that the target language can hold them as a string.
	 *  Each target can have a different set in memory at same time.
	 */
    public virtual Dictionary<char, string> GetTargetCharValueEscape() => defaultCharValueEscape;

    protected static void AddEscapedChar(Dictionary<char, String> map, char key)
    {
        AddEscapedChar(map, key, key);
    }

    protected static void AddEscapedChar(Dictionary<char, String> map, char key, char representation)
    {
        map[key] = ("\\" + representation);
    }

    public virtual string GetLanguage() => gen.language;

    public virtual CodeGenerator GetCodeGenerator() => gen;

    /** ANTLR tool should check output templates / target are compatible with tool code generation.
	 *  For now, a simple string match used on x.y of x.y.z scheme. We use a method to avoid mismatches
	 *  between a template called VERSION. This value is checked against Tool.VERSION during load of templates.
	 *
	 *  This additional method forces all targets 4.3 and beyond to add this method.
	 *
	 * @since 4.3
	 */
    public virtual string GetVersion() => Tool.VERSION;

    public TemplateGroup GetTemplates()
    {
        lock (this)
        {
            var language = GetLanguage();
            if (!languageTemplates.TryGetValue(language, out var templates))
            {
                var version = GetVersion();
                if (version == null ||
                        !RuntimeMetaData.getMajorMinorVersion(version).Equals(RuntimeMetaData.getMajorMinorVersion(Tool.VERSION)))
                {
                    gen.tool.ErrMgr.toolError(tool.ErrorType.INCOMPATIBLE_TOOL_AND_TEMPLATES, version, Tool.VERSION, language);
                }
                templates = LoadTemplates();
                languageTemplates.Add(language, templates);
            }

            return templates;
        }
    }

    protected abstract HashSet<string> GetReservedWords();

    public virtual string EscapeIfNeeded(string identifier) => GetReservedWords().Contains(identifier) ? EscapeWord(identifier) : identifier;

    protected virtual string EscapeWord(string word) => word + "_";

    public virtual void GenFile(Grammar g, Template outputFileST, string fileName)
    {
        GetCodeGenerator().Write(outputFileST, fileName);
    }

    /** Get a meaningful name for a token type useful during code generation.
	 *  Literals without associated names are converted to the string equivalent
	 *  of their integer values. Used to generate x==ID and x==34 type comparisons
	 *  etc...  Essentially we are looking for the most obvious way to refer
	 *  to a token type in the generated code.
	 */
    public virtual string GetTokenTypeAsTargetLabel(Grammar g, int ttype)
    {
        var name = g.getTokenName(ttype);
        // If name is not valid, return the token type instead
        if (Grammar.INVALID_TOKEN_NAME.Equals(name))
        {
            return (ttype.ToString());
        }

        return name;
    }

    public virtual string[] GetTokenTypesAsTargetLabels(Grammar g, int[] ttypes)
    {
        var labels = new string[ttypes.Length];
        for (int i = 0; i < ttypes.Length; i++)
        {
            labels[i] = GetTokenTypeAsTargetLabel(g, ttypes[i]);
        }
        return labels;
    }

    /** Given a random string of Java unicode chars, return a new string with
	 *  optionally appropriate quote characters for target language and possibly
	 *  with some escaped characters.  For example, if the incoming string has
	 *  actual newline characters, the output of this method would convert them
	 *  to the two char sequence \n for Java, C, C++, ...  The new string has
	 *  double-quotes around it as well.  Example String in memory:
	 *
	 *     a"[newlinechar]b'c[carriagereturnchar]d[tab]e\f
	 *
	 *  would be converted to the valid Java s:
	 *
	 *     "a\"\nb'c\rd\te\\f"
	 *
	 *  or
	 *
	 *     a\"\nb'c\rd\te\\f
	 *
	 *  depending on the quoted arg.
	 */
    public virtual string GetTargetStringLiteralFromString(string s, bool quoted)
    {
        if (s == null) return null;
        var buffer = new StringBuilder();
        if (quoted)
        {
            buffer.Append('"');
        }
        for (int i = 0; i < s.Length;)
        {
            int c = char.ConvertToUtf32(s, i); // s.codePointAt(i);
            var escaped = c <= char.MaxValue ?
                (GetTargetCharValueEscape().TryGetValue((char)c, out var v) ? v : null) : null;
            if (c != '\'' && escaped != null)
            { // don't escape single quotes in strings for java
                buffer.Append(escaped);
            }
            else if (ShouldUseUnicodeEscapeForCodePointInDoubleQuotedString(c))
            {
                AppendUnicodeEscapedCodePoint(i, buffer);
            }
            else
            {
                buffer.Append(char.ConvertFromUtf32(c));//.appendCodePoint(c);
            }
            i += new Rune(c).Utf16SequenceLength;
        }
        if (quoted) buffer.Append('"');
        return buffer.ToString();
    }

    private void AppendUnicodeEscapedCodePoint(int codePoint, StringBuilder sb, bool escape)
    {
        if (escape) sb.Append('\\');
        AppendUnicodeEscapedCodePoint(codePoint, sb);
    }

    /**
	 * Escape the Unicode code point appropriately for this language
	 * and Append the escaped value to {@code sb}.
	 * It exists for flexibility and backward compatibility with external targets
	 * The static method {@link UnicodeEscapes#appendEscapedCodePoint(StringBuilder, int, String)} can be used as well
	 * if default escaping method (Java) is used or language is officially supported
	 */
    protected virtual void AppendUnicodeEscapedCodePoint(int codePoint, StringBuilder builder)
    {
        UnicodeEscapes.AppendEscapedCodePoint(builder, codePoint, GetLanguage());
    }

    public virtual string GetTargetStringLiteralFromString(string s)
        => GetTargetStringLiteralFromString(s, true);

    public virtual string GetTargetStringLiteralFromANTLRStringLiteral(CodeGenerator generator, String literal, bool addQuotes)
        => GetTargetStringLiteralFromANTLRStringLiteral(generator, literal, addQuotes, false);

    /**
	 * <p>Convert from an ANTLR string literal found in a grammar file to an
	 * equivalent string literal in the target language.
	 *</p>
	 * <p>
	 * For Java, this is the translation {@code 'a\n"'} &rarr; {@code "a\n\""}.
	 * Expect single quotes around the incoming literal. Just flip the quotes
	 * and replace double quotes with {@code \"}.
	 * </p>
	 * <p>
	 * Note that we have decided to allow people to use '\"' without penalty, so
	 * we must build the target string in a loop as {@link String#replace}
	 * cannot handle both {@code \"} and {@code "} without a lot of messing
	 * around.
	 * </p>
	 */
    public virtual string GetTargetStringLiteralFromANTLRStringLiteral(
        CodeGenerator generator,
        string literal,
        bool addQuotes,
        bool escapeSpecial)
    {
        var builder = new StringBuilder();

        if (addQuotes) builder.Append('"');

        for (int i = 1; i < literal.Length - 1;)
        {
            int codePoint = char.ConvertToUtf32(literal, i);
            int toAdvance = new Rune(codePoint).Utf16SequenceLength;
            if (codePoint == '\\')
            {
                // Anything escaped is what it is! We assume that
                // people know how to escape characters correctly. However
                // we catch anything that does not need an escape in Java (which
                // is what the default implementation is dealing with and remove
                // the escape. The C target does this for instance.
                //
                int escapedCodePoint = char.ConvertToUtf32(literal, (i + toAdvance));
                toAdvance++;
                switch (escapedCodePoint)
                {
                    // Pass through any escapes that Java also needs
                    //
                    case 'n':
                    case 'r':
                    case 't':
                    case 'b':
                    case 'f':
                    case '\\':
                        // Pass the escape through
                        if (escapeSpecial && escapedCodePoint != '\\')
                        {
                            builder.Append('\\');
                        }
                        builder.Append('\\');
                        builder.Append(char.ConvertFromUtf32(escapedCodePoint));
                        break;

                    case 'u':    // Either unnnn or u{nnnnnn}
                        if (literal[(i + toAdvance)] == '{')
                        {
                            while (literal[(i + toAdvance)] != '}')
                            {
                                toAdvance++;
                            }
                            toAdvance++;
                        }
                        else
                        {
                            toAdvance += 4;
                        }
                        if (i + toAdvance <= literal.Length)
                        { // we might have an invalid \\uAB or something
                            var fullEscape = literal.Substring(i, toAdvance);
                            AppendUnicodeEscapedCodePoint(
                                CharSupport.GetCharValueFromCharInGrammarLiteral(fullEscape),
                                builder,
                                escapeSpecial);
                        }
                        break;
                    default:
                        if (ShouldUseUnicodeEscapeForCodePointInDoubleQuotedString(escapedCodePoint))
                        {
                            AppendUnicodeEscapedCodePoint(escapedCodePoint, builder, escapeSpecial);
                        }
                        else
                        {
                            builder.Append(new Rune(escapedCodePoint).ToString());
                        }
                        break;
                }
            }
            else
            {
                if (codePoint == 0x22)
                {
                    // ANTLR doesn't escape " in literal strings,
                    // but every other language needs to do so.
                    builder.Append("\\\"");
                }
                else if (ShouldUseUnicodeEscapeForCodePointInDoubleQuotedString(codePoint))
                {
                    AppendUnicodeEscapedCodePoint(codePoint, builder, escapeSpecial);
                }
                else
                {
                    builder.Append(new Rune(codePoint).ToString());
                }
            }
            i += toAdvance;
        }

        if (addQuotes) builder.Append('"');

        return builder.ToString();
    }

    protected virtual bool ShouldUseUnicodeEscapeForCodePointInDoubleQuotedString(int codePoint)
    {
        // We don't want anyone passing 0x0A (newline) or 0x22
        // (double-quote) here because Java treats \\u000A as
        // a literal newline and \\u0022 as a literal
        // double-quote, so Unicode escaping doesn't help.
        //assert codePoint != 0x0A && codePoint != 0x22;

        return
            codePoint < 0x20 || // control characters up to but not including space
            codePoint == 0x5C || // backslash
            codePoint >= 0x7F;   // DEL and beyond (keeps source code 7-bit US-ASCII)
    }

    /** Assume 16-bit char */
    public virtual string EncodeInt16AsCharEscape(int v)
    {
        if (v < char.MinValue || v > char.MaxValue)
        {
            throw new ArgumentException($"Cannot encode the specified value: {v}");
        }

        if (IsATNSerializedAsInts())
        {
            return v.ToString();// Integer.ToString(v);
        }

        char c = (char)v;
        //String escaped = getTargetCharValueEscape().get(c);
        if (GetTargetCharValueEscape().TryGetValue(c, out var escaped))
        {
            return escaped;
        }

        switch (char.GetUnicodeCategory(c))
        {
            case System.Globalization.UnicodeCategory.Control:
            case System.Globalization.UnicodeCategory.LineSeparator:
            case System.Globalization.UnicodeCategory.ParagraphSeparator:
                return EscapeChar(v);
            default:
                if (v <= 127)
                {
                    return c.ToString();  // ascii chars can be as-is, no encoding
                }
                // else we use hex encoding to ensure pure ascii chars generated
                return EscapeChar(v);
        }
    }

    protected virtual string EscapeChar(int v) => $"\\u%{v:X4}";

    public virtual string GetLoopLabel(GrammarAST ast) => "loop" + ast.token.getTokenIndex();

    public virtual string GetLoopCounter(GrammarAST ast) => "cnt" + ast.token.getTokenIndex();

    public virtual string GetListLabel(String label)
    {
        var st = GetTemplates().GetInstanceOf("ListLabelName");
        st.Add("label", label);
        return st.Render();
    }

    public virtual string GetRuleFunctionContextStructName(Rule r) => r.g.isLexer()
            ? GetTemplates().GetInstanceOf("LexerRuleContext").Render()
            : Utils.Capitalize(r.name) + GetTemplates().GetInstanceOf("RuleContextNameSuffix").Render();

    public virtual string GetAltLabelContextStructName(string label)
        => Utils.Capitalize(label) + GetTemplates().GetInstanceOf("RuleContextNameSuffix").Render();

    /** If we know which actual function, we can provide the actual ctx type.
	 *  This will contain implicit labels etc...  From outside, though, we
	 *  see only ParserRuleContext unless there are externally visible stuff
	 *  like args, locals, explicit labels, etc...
	 */
    public virtual string GetRuleFunctionContextStructName(model.RuleFunction function)
    {
        var r = function.rule;
        return r.g.isLexer()
            ? GetTemplates().GetInstanceOf("LexerRuleContext").Render()
            : Utils.Capitalize(r.name) + GetTemplates().GetInstanceOf("RuleContextNameSuffix").Render();
    }

    // should be same for all refs to same token like ctx.ID within single rule function
    // for literals like 'while', we gen _s<ttype>
    public virtual string GetImplicitTokenLabel(string tokenName)
    {
        var st = GetTemplates().GetInstanceOf("ImplicitTokenLabel");
        int ttype = GetCodeGenerator().g.getTokenType(tokenName);
        if (tokenName.StartsWith("'")) return "s" + ttype;
        var text = GetTokenTypeAsTargetLabel(GetCodeGenerator().g, ttype);
        st.Add("tokenName", text);
        return st.Render();
    }

    // x=(A|B)
    public virtual string GetImplicitSetLabel(string id)
    {
        var st = GetTemplates().GetInstanceOf("ImplicitSetLabel");
        st.Add("id", id);
        return st.Render();
    }

    public virtual string GetImplicitRuleLabel(string ruleName)
    {
        var st = GetTemplates().GetInstanceOf("ImplicitRuleLabel");
        st.Add("ruleName", ruleName);
        return st.Render();
    }

    public virtual string GetElementListName(string name)
    {
        var st = GetTemplates().GetInstanceOf("ElementListName");
        st.Add("elemName", GetElementName(name));
        return st.Render();
    }

    public virtual string GetElementName(string name)
    {
        if (".".Equals(name)) return "_wild";
        if (GetCodeGenerator().g.getRule(name) != null) return name;
        int ttype = GetCodeGenerator().g.getTokenType(name);
        if (ttype == Token.INVALID_TYPE) return name;
        return GetTokenTypeAsTargetLabel(GetCodeGenerator().g, ttype);
    }

    /** Generate TParser.java and TLexer.java from T.g4 if combined, else
	 *  just use T.java as output regardless of type.
	 */
    public virtual string GetRecognizerFileName(bool header)
    {
        var extST = GetTemplates().GetInstanceOf("codeFileExtension");
        var recognizerName = gen.g.getRecognizerName();
        return recognizerName + extST.Render();
    }

    /** A given grammar T, return the listener name such as
	 *  TListener.java, if we're using the Java target.
 	 */
    public virtual string GetListenerFileName(bool header)
    {
        //assert gen.g.name != null;
        var extST = GetTemplates().GetInstanceOf("codeFileExtension");
        var listenerName = gen.g.name + "Listener";
        return listenerName + extST.Render();
    }

    /** A given grammar T, return the visitor name such as
	 *  TVisitor.java, if we're using the Java target.
 	 */
    public virtual string GetVisitorFileName(bool header)
    {
        //assert gen.g.name != null;
        var extST = GetTemplates().GetInstanceOf("codeFileExtension");
        var listenerName = gen.g.name + "Visitor";
        return listenerName + extST.Render();
    }

    /** A given grammar T, return a blank listener implementation
	 *  such as TBaseListener.java, if we're using the Java target.
 	 */
    public virtual string GetBaseListenerFileName(bool header)
    {
        //assert gen.g.name != null;
        var extST = GetTemplates().GetInstanceOf("codeFileExtension");
        var listenerName = gen.g.name + "BaseListener";
        return listenerName + extST.Render();
    }

    /** A given grammar T, return a blank listener implementation
	 *  such as TBaseListener.java, if we're using the Java target.
 	 */
    public virtual string GetBaseVisitorFileName(bool header)
    {
        //assert gen.g.name != null;
        var extST = GetTemplates().GetInstanceOf("codeFileExtension");
        var listenerName = gen.g.name + "BaseVisitor";
        return listenerName + extST.Render();
    }

    /**
	 * Gets the maximum number of 16-bit unsigned integers that can be encoded
	 * in a single segment (a declaration in target language) of the serialized ATN.
	 * E.g., in C++, a small segment length results in multiple decls like:
	 *
	 *   static const int32_t serializedATNSegment1[] = {
	 *     0x7, 0x12, 0x2, 0x13, 0x7, 0x13, 0x2, 0x14, 0x7, 0x14, 0x2, 0x15, 0x7,
	 *        0x15, 0x2, 0x16, 0x7, 0x16, 0x2, 0x17, 0x7, 0x17, 0x2, 0x18, 0x7,
	 *        0x18, 0x2, 0x19, 0x7, 0x19, 0x2, 0x1a, 0x7, 0x1a, 0x2, 0x1b, 0x7,
	 *        0x1b, 0x2, 0x1c, 0x7, 0x1c, 0x2, 0x1d, 0x7, 0x1d, 0x2, 0x1e, 0x7,
	 *        0x1e, 0x2, 0x1f, 0x7, 0x1f, 0x2, 0x20, 0x7, 0x20, 0x2, 0x21, 0x7,
	 *        0x21, 0x2, 0x22, 0x7, 0x22, 0x2, 0x23, 0x7, 0x23, 0x2, 0x24, 0x7,
	 *        0x24, 0x2, 0x25, 0x7, 0x25, 0x2, 0x26,
	 *   };
	 *
	 * instead of one big one.  Targets are free to ignore this like JavaScript does.
	 *
	 * This is primarily needed by Java target to limit size of any single ATN string
	 * to 65k length.
	 *
	 * @see SerializedATN#getSegments
	 *
	 * @return the serialized ATN segment limit
	 */
    public virtual int GetSerializedATNSegmentLimit() => int.MaxValue;

    /** How many bits should be used to do inline token type tests? Java assumes
	 *  a 64-bit word for bitsets.  Must be a valid wordsize for your target like
	 *  8, 16, 32, 64, etc...
	 *
	 *  @since 4.5
	 */
    public virtual int GetInlineTestSetWordSize() => 64;

    public virtual bool GrammarSymbolCausesIssueInGeneratedCode(GrammarAST idNode)
    {
        switch (idNode.getParent().getType())
        {
            case ANTLRParser.ASSIGN:
                switch (idNode.getParent().getParent().getType())
                {
                    case ANTLRParser.ELEMENT_OPTIONS:
                    case ANTLRParser.OPTIONS:
                        return false;

                    default:
                        break;
                }

                break;

            case ANTLRParser.AT:
            case ANTLRParser.ELEMENT_OPTIONS:
                return false;

            case ANTLRParser.LEXER_ACTION_CALL:
                if (idNode.getChildIndex() == 0)
                {
                    // first child is the command name which is part of the ANTLR language
                    return false;
                }

                // arguments to the command should be checked
                break;

            default:
                break;
        }

        return GetReservedWords().Contains(idNode.getText());
    }

    //@Deprecated
    protected virtual bool VisibleGrammarSymbolCausesIssueInGeneratedCode(GrammarAST idNode)
        => GetReservedWords().Contains(idNode.getText());

    public virtual bool TemplatesExist() => LoadTemplatesHelper(false) != null;
    public class STE : ITemplateErrorListener
    {
        public readonly Target target;
        public STE(Target target) => this.target = target;

        //@Override
        public void CompiletimeError(TemplateMessage msg) => ReportError(msg);

        //@Override
        public void RuntimeError(TemplateMessage msg) => ReportError(msg);

        //@Override
        public void IOError(TemplateMessage msg) => ReportError(msg);

        //@Override
        public void InternalError(TemplateMessage msg) => ReportError(msg);

        private void ReportError(TemplateMessage msg) => target.GetCodeGenerator().tool.ErrMgr.toolError(tool.ErrorType.STRING_TEMPLATE_WARNING, msg.Cause, msg.ToString());

    }
    protected TemplateGroup LoadTemplates()
    {
        var result = LoadTemplatesHelper(true);
        if (result == null) return null;
        result.RegisterRenderer(typeof(int), new NumberRenderer());
        result.RegisterRenderer(typeof(string), new StringRenderer());
        result.Listener = (new STE(this));

        return result;
    }

    private TemplateGroup LoadTemplatesHelper(bool reportErrorIfFail)
    {
        var language = GetLanguage();
        var groupFileName = CodeGenerator.TEMPLATE_ROOT + "/" + language + "/" + language + TemplateGroup.GroupFileExtension;
        try
        {
            return new TemplateGroupFile(groupFileName);
        }
        catch (ArgumentException iae)
        {
            if (reportErrorIfFail)
            {
                gen.tool.ErrMgr.toolError(tool.ErrorType.MISSING_CODE_GEN_TEMPLATES, iae, GetLanguage());
            }
            return null;
        }
    }

    /**
	 * @since 4.3
	 */
    public virtual bool WantsBaseListener() => true;

    /**
	 * @since 4.3
	 */
    public virtual bool WantsBaseVisitor() => true;

    /**
	 * @since 4.3
	 */
    public virtual bool SupportsOverloadedMethods() => true;

    public virtual bool IsATNSerializedAsInts() => true;

    /** @since 4.6 */
    public virtual bool NeedsHeader() => false;  // Override in targets that need header files.
}
