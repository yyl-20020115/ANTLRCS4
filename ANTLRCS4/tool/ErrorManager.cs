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

public class ErrorManager {
	private readonly static Dictionary<String, TemplateGroupFile> loadedFormats = new ();

	public static readonly String FORMATS_DIR = "org/antlr/v4/tool/templates/messages/formats/";

	public Tool tool;
	public int errors;
	public int warnings;

	/** All errors that have been generated */
	public HashSet<ErrorType> errorTypes = ErrorType.ErrorTypes.ToHashSet();

    /** The group of templates that represent the current message format. */
    TemplateGroup format;

    String formatName;

    ErrorBuffer initSTListener = new ErrorBuffer();

	public ErrorManager(Tool tool) {
		this.tool = tool;
	}

	public void resetErrorState() {
		errors = 0;
		warnings = 0;
	}

	public Template getMessageTemplate(ANTLRMessage msg) {
        Template messageST = msg.getMessageTemplate(tool.longMessages);
        Template locationST = getLocationFormat();
        Template reportST = getReportFormat(msg.getErrorType().severity);
        Template messageFormatST = getMessageFormat();

		bool locationValid = false;
		if (msg.line != -1) {
			locationST.Add("line", msg.line);
			locationValid = true;
		}
		if (msg.charPosition != -1) {
			locationST.Add("column", msg.charPosition);
			locationValid = true;
		}
		if (msg.fileName != null) {
			String displayFileName = msg.fileName;
			if (formatName.Equals("antlr")) {
				// Don't show path to file in messages in ANTLR format;
				// they're too long.
				string f = (msg.fileName);
				if (File.Exists(f) ) {
					displayFileName = f;
				}
			}
			else {
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
    public Template getLocationFormat() {
        return format.GetInstanceOf("location");
    }

    public Template getReportFormat(ErrorSeverity severity) {
        Template st = format.GetInstanceOf("report");
        st.Add("type", severity.ToString()/*.getText()*/);
        return st;
    }

    public Template getMessageFormat() {
        return format.GetInstanceOf("message");
    }
    public bool formatWantsSingleLineMessage() {
        return format.GetInstanceOf("wantsSingleLineMessage").Render().Equals("true");
    }

	public void info(String msg) { tool.info(msg); }

	public void syntaxError(ErrorType etype,
								   String fileName,
								   Token token,
								   RecognitionException antlrException,
								   params Object[] args)
	{
		ANTLRMessage msg = new GrammarSyntaxMessage(etype,fileName,token,antlrException,args);
		emit(etype, msg);
	}

	public static void fatalInternalError(String error, Exception e) {
		internalError(error, e);
		throw new RuntimeException(error, e);
	}

	public static void internalError(String error, Exception e) {
        StackTraceElement location = getLastNonErrorManagerCodeLocation(e);
		internalError("Exception "+e+"@"+location+": "+error);
    }

    public static void internalError(String error) {
        StackTraceElement location =
            getLastNonErrorManagerCodeLocation(new Exception());
        String msg = location+": "+error;
        Console.Error.WriteLine("internal error: "+msg);
    }

    /**
     * Raise a predefined message with some number of parameters for the StringTemplate but for which there
     * is no location information possible.
     * @param errorType The Message Descriptor
     * @param args The arguments to pass to the StringTemplate
     */
	public void toolError(ErrorType errorType, params Object[] args) {
		toolError(errorType, null, args);
	}

	public void toolError(ErrorType errorType, Exception e, params Object[] args) {
		ToolMessage msg = new ToolMessage(errorType, e, args);
		emit(errorType, msg);
	}

    public void grammarError(ErrorType etype,
							 String fileName,
							 Token token,
							 params Object[] args)
	{
        ANTLRMessage msg = new GrammarSemanticsMessage(etype,fileName,token,args);
		emit(etype, msg);

	}

	public void leftRecursionCycles(String fileName, ICollection<HashSet<Rule>> cycles) {
		errors++;
		ANTLRMessage msg = new LeftRecursionCyclesMessage(fileName, cycles);
		tool.error(msg);
	}

    public int getNumErrors() {
        return errors;
    }

    /** Return first non ErrorManager code location for generating messages */
    private static StackTraceElement getLastNonErrorManagerCodeLocation(Exception e) {
		StackTraceElement[] stack = new StackTraceElement[0];// e.getStackTrace();
        int i = 0;
        for (; i < stack.Length; i++) {
            StackTraceElement t = stack[i];
            if (!t.ToString().Contains("ErrorManager")) {
                break;
            }
        }
        StackTraceElement location = stack[i];
        return location;
    }

    // S U P P O R T  C O D E

	//@SuppressWarnings("fallthrough")
	public void emit(ErrorType etype, ANTLRMessage msg) {
		if(etype.severity == ErrorSeverity.WARNING_ONE_OFF)
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
    public void setFormat(String formatName) {
		TemplateGroupFile loadedFormat = null;

        lock (loadedFormats) {
			if (!loadedFormats.TryGetValue(formatName,out loadedFormat)) {
				String fileName = FORMATS_DIR + formatName + TemplateGroup.GroupFileExtension;
				string url = fileName;// cl.getResource(fileName);
				//if (url == null) {
				//	cl = ErrorManager.getClassLoader();
				//	url = cl.getResource(fileName);
				//}
				if (url == null && formatName.Equals("antlr")) {
					rawError("ANTLR installation corrupted; cannot find ANTLR messages format file " + fileName);
					panic();
				}
				else if (url == null) {
					rawError("no such message format file " + fileName + " retrying with default ANTLR format");
					setFormat("antlr"); // recurse on this rule, trying the default message format
					return;
				}
				loadedFormat = new TemplateGroupFile(url, Encoding.UTF8, '<', '>');
				loadedFormat.Load();

				loadedFormats[formatName]= loadedFormat;
			}
		}

		this.formatName = formatName;
		this.format = loadedFormat;

		if (initSTListener.Errors.Count > 0) {
			rawError("ANTLR installation corrupted; can't load messages format file:\n" +
					initSTListener.ToString());
			panic();
		}

		bool formatOK = verifyFormat();
		if (!formatOK && formatName.Equals("antlr")) {
			rawError("ANTLR installation corrupted; ANTLR messages format file " + formatName + ".stg incomplete");
			panic();
		}
		else if (!formatOK) {
			setFormat("antlr"); // recurse on this rule, trying the default message format
		}
	}

    /** Verify the message format template group */
    protected bool verifyFormat() {
        bool ok = true;
        if (!format.IsDefined("location")) {
            Console.Error.WriteLine("Format template 'location' not found in " + formatName);
            ok = false;
        }
        if (!format.IsDefined("message")) {
            Console.Error.WriteLine("Format template 'message' not found in " + formatName);
            ok = false;
        }
        if (!format.IsDefined("report")) {
            Console.Error.WriteLine("Format template 'report' not found in " + formatName);
            ok = false;
        }
        return ok;
    }

    /** If there are errors during ErrorManager init, we have no choice
     *  but to go to System.err.
     */
    static void rawError(String msg) {
        Console.Error.WriteLine(msg);
    }

    static void rawError(String msg, Exception e) {
        rawError(msg);
        //e.printStackTrace(System.err);
    }

	public void panic(ErrorType errorType, params Object[] args) {
		ToolMessage msg = new ToolMessage(errorType, args);
        Template msgST = getMessageTemplate(msg);
		String outputMsg = msgST.Render();
		if ( formatWantsSingleLineMessage() ) {
			outputMsg = outputMsg.Replace('\n', ' ');
		}
		panic(outputMsg);
	}

	public static void panic(String msg) {
		rawError(msg);
		panic();
	}

    public static void panic() {
        // can't call tool.panic since there may be multiple tools; just
        // one error manager
        throw new Error("ANTLR ErrorManager panic");
    }
}
