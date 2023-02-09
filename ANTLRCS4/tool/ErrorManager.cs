/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using Antlr4.StringTemplate;
using Antlr4.StringTemplate.Misc;
using org.antlr.v4.runtime;
using org.antlr.v4.runtime.misc;
using org.antlr.v4.runtime.tree.pattern;
using System.Text;

namespace org.antlr.v4.tool;

public class ErrorManager
{
    private readonly static Dictionary<String, TemplateGroupFile> loadedFormats = new();

    public static readonly String FORMATS_DIR = "org/antlr/v4/tool/templates/messages/formats/";

    public Tool tool;
    public int errors;
    public int warnings;

    /** All errors that have been generated */
    public HashSet<ErrorType> errorTypes = ErrorType.ErrorTypes.ToHashSet();

    /** The group of templates that represent the current message format. */
    TemplateGroup format;

    String formatName;
    readonly ErrorBuffer initSTListener = new ();

    public ErrorManager(Tool tool)
    {
        this.tool = tool;
    }

    public void ResetErrorState()
    {
        errors = 0;
        warnings = 0;
    }

    public Template GetMessageTemplate(ANTLRMessage msg)
    {
        var messageST = msg.getMessageTemplate(tool.longMessages);
        var locationST = GetLocationFormat();
        var reportST = GetReportFormat(msg.getErrorType().severity);
        var messageFormatST = GetMessageFormat();

        var locationValid = false;
        if (msg.line != -1)
        {
            locationST.Add("line", msg.line);
            locationValid = true;
        }
        if (msg.charPosition != -1)
        {
            locationST.Add("column", msg.charPosition);
            locationValid = true;
        }
        if (msg.fileName != null)
        {
            var displayFileName = msg.fileName;
            if (formatName.Equals("antlr"))
            {
                // Don't show path to file in messages in ANTLR format;
                // they're too long.
                var f = (msg.fileName);
                if (File.Exists(f))
                {
                    displayFileName = f;
                }
            }
            else
            {
                // For other message formats, use the full filename in the
                // message.  This assumes that these formats are intended to
                // be parsed by IDEs, and so they need the full path to
                // resolve correctly.
            }
            locationST.Add("file", displayFileName);
            locationValid = true;
        }

        messageFormatST.Add("id", msg.getErrorType().code);
        messageFormatST.Add("text", messageST);

        if (locationValid) reportST.Add("location", locationST);
        reportST.Add("message", messageFormatST);
        //((DebugST)reportST).inspect();
        //		reportST.impl.dump();
        return reportST;
    }

    /** Return a StringTemplate that refers to the current format used for
     * emitting messages.
     */
    public Template GetLocationFormat()
    {
        return format.GetInstanceOf("location");
    }

    public Template GetReportFormat(ErrorSeverity severity)
    {
        var st = format.GetInstanceOf("report");
        st.Add("type", severity.ToString()/*.getText()*/);
        return st;
    }

    public Template GetMessageFormat()
    {
        return format.GetInstanceOf("message");
    }
    public bool FormatWantsSingleLineMessage()
    {
        return format.GetInstanceOf("wantsSingleLineMessage").Render().Equals("true");
    }

    public void Info(string msg) => tool.info(msg);

    public void SyntaxError(ErrorType etype,
                                   string fileName,
                                   Token token,
                                   RecognitionException antlrException,
                                   params Object[] args)
    {
        var msg = new GrammarSyntaxMessage(etype, fileName, token, antlrException, args);
        Emit(etype, msg);
    }

    public static void FatalInternalError(string error, Exception e)
    {
        InternalError(error, e);
        throw new RuntimeException(error, e);
    }

    public static void InternalError(string error, Exception e)
    {
        var location = GetLastNonErrorManagerCodeLocation(e);
        InternalError("Exception " + e + "@" + location + ": " + error);
    }

    public static void InternalError(string error)
    {
        var location =
            GetLastNonErrorManagerCodeLocation(new Exception());
        var msg = location + ": " + error;
        Console.Error.WriteLine("internal error: " + msg);
    }

    /**
     * Raise a predefined message with some number of parameters for the StringTemplate but for which there
     * is no location information possible.
     * @param errorType The Message Descriptor
     * @param args The arguments to pass to the StringTemplate
     */
    public void ToolError(ErrorType errorType, params Object[] args)
    {
        ToolError(errorType, null, args);
    }

    public void ToolError(ErrorType errorType, Exception e, params Object[] args)
    {
        var msg = new ToolMessage(errorType, e, args);
        Emit(errorType, msg);
    }

    public void GrammarError(ErrorType etype,
                             string fileName,
                             Token token,
                             params object[] args)
    {
        var msg = new GrammarSemanticsMessage(etype, fileName, token, args);
        Emit(etype, msg);

    }

    public void LeftRecursionCycles(String fileName, ICollection<HashSet<Rule>> cycles)
    {
        errors++;
        var msg = new LeftRecursionCyclesMessage(fileName, cycles);
        tool.error(msg);
    }

    public virtual int NumErrors => errors;

    /** Return first non ErrorManager code location for generating messages */
    private static StackTraceElement GetLastNonErrorManagerCodeLocation(Exception e)
    {
        var stack = new StackTraceElement[0];// e.getStackTrace();
        int i = 0;
        for (; i < stack.Length; i++)
        {
            StackTraceElement t = stack[i];
            if (!t.ToString().Contains("ErrorManager"))
            {
                break;
            }
        }
        StackTraceElement location = stack[i];
        return location;
    }

    // S U P P O R T  C O D E

    //@SuppressWarnings("fallthrough")
    public void Emit(ErrorType etype, ANTLRMessage msg)
    {
        if (etype.severity == ErrorSeverity.WARNING_ONE_OFF)
        {
            if (errorTypes.Contains(etype)) goto exit_me;
        }
        if (etype.severity == ErrorSeverity.WARNING_ONE_OFF)
        {
            warnings++;
            tool.warning(msg);
            goto exit_me;
        }
        if (etype.severity == ErrorSeverity.ERROR_ONE_OFF)
        {
            if (errorTypes.Contains(etype)) goto exit_me;
        }
        if (etype.severity == ErrorSeverity.ERROR)
        {
            if (errorTypes.Contains(etype)) goto exit_me;
            errors++;
            tool.error(msg);
            goto exit_me;
        }
    exit_me:
        errorTypes.Add(etype);

        //switch (etype.severity)
        //{
        //	case ErrorSeverity.WARNING_ONE_OFF:
        //		if (errorTypes.Contains(etype)) break;
        //	// fall thru
        //	case ErrorSeverity.WARNING:
        //		warnings++;
        //		tool.warning(msg);
        //		break;
        //	case ErrorSeverity.ERROR_ONE_OFF:
        //		if (errorTypes.Contains(etype)) break;
        //	// fall thru
        //	case ErrorSeverity.ERROR:
        //		errors++;
        //		tool.error(msg);
        //		break;
        //	default:
        //		break;
        //}
    }

    /** The format gets reset either from the Tool if the user supplied a command line option to that effect
     *  Otherwise we just use the default "antlr".
     */
    public void SetFormat(string formatName)
    {
        TemplateGroupFile loadedFormat = null;

        lock (loadedFormats)
        {
            if (!loadedFormats.TryGetValue(formatName, out loadedFormat))
            {
                var fileName = FORMATS_DIR + formatName + TemplateGroup.GroupFileExtension;
                var url = fileName;// cl.getResource(fileName);
                                      //if (url == null) {
                                      //	cl = ErrorManager.getClassLoader();
                                      //	url = cl.getResource(fileName);
                                      //}
                if (url == null && formatName.Equals("antlr"))
                {
                    RawError("ANTLR installation corrupted; cannot find ANTLR messages format file " + fileName);
                    Panic();
                }
                else if (url == null)
                {
                    RawError("no such message format file " + fileName + " retrying with default ANTLR format");
                    SetFormat("antlr"); // recurse on this rule, trying the default message format
                    return;
                }
                loadedFormat = new TemplateGroupFile(url, Encoding.UTF8, '<', '>');
                loadedFormat.Load();

                loadedFormats[formatName] = loadedFormat;
            }
        }

        this.formatName = formatName;
        this.format = loadedFormat;

        if (initSTListener.Errors.Count > 0)
        {
            RawError("ANTLR installation corrupted; can't load messages format file:\n" +
                    initSTListener.ToString());
            Panic();
        }

        bool formatOK = VerifyFormat();
        if (!formatOK && formatName.Equals("antlr"))
        {
            RawError("ANTLR installation corrupted; ANTLR messages format file " + formatName + ".stg incomplete");
            Panic();
        }
        else if (!formatOK)
        {
            SetFormat("antlr"); // recurse on this rule, trying the default message format
        }
    }

    /** Verify the message format template group */
    protected bool VerifyFormat()
    {
        bool ok = true;
        if (!format.IsDefined("location"))
        {
            Console.Error.WriteLine("Format template 'location' not found in " + formatName);
            ok = false;
        }
        if (!format.IsDefined("message"))
        {
            Console.Error.WriteLine("Format template 'message' not found in " + formatName);
            ok = false;
        }
        if (!format.IsDefined("report"))
        {
            Console.Error.WriteLine("Format template 'report' not found in " + formatName);
            ok = false;
        }
        return ok;
    }

    /** If there are errors during ErrorManager init, we have no choice
     *  but to go to System.err.
     */
    static void RawError(String msg)
    {
        Console.Error.WriteLine(msg);
    }

    static void RawError(String msg, Exception e)
    {
        RawError(msg);
        //e.printStackTrace(System.err);
    }

    public void Panic(ErrorType errorType, params object[] args)
    {
        var msg = new ToolMessage(errorType, args);
        var msgST = GetMessageTemplate(msg);
        var outputMsg = msgST.Render();
        if (FormatWantsSingleLineMessage())
        {
            outputMsg = outputMsg.Replace('\n', ' ');
        }
        Panic(outputMsg);
    }

    public static void Panic(String msg)
    {
        RawError(msg);
        Panic();
    }

    public static void Panic()
    {
        // can't call tool.panic since there may be multiple tools; just
        // one error manager
        throw new Error("ANTLR ErrorManager panic");
    }
}
