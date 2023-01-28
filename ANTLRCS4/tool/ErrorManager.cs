/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using org.antlr.v4.runtime;
using org.antlr.v4.runtime.misc;
using org.antlr.v4.runtime.tree.pattern;

namespace org.antlr.v4.tool;

public class ErrorManager {
	private readonly static Dictionary<String, STGroupFile> loadedFormats = new ();

	public static readonly String FORMATS_DIR = "org/antlr/v4/tool/templates/messages/formats/";

	public Tool tool;
	public int errors;
	public int warnings;

	/** All errors that have been generated */
	public HashSet<ErrorType> errorTypes = EnumSet.noneOf(ErrorType);

    /** The group of templates that represent the current message format. */
    STGroup format;

    String formatName;

    ErrorBuffer initSTListener = new ErrorBuffer();

	public ErrorManager(Tool tool) {
		this.tool = tool;
	}

	public void resetErrorState() {
		errors = 0;
		warnings = 0;
	}

	public ST getMessageTemplate(ANTLRMessage msg) {
		ST messageST = msg.getMessageTemplate(tool.longMessages);
		ST locationST = getLocationFormat();
		ST reportST = getReportFormat(msg.getErrorType().severity);
		ST messageFormatST = getMessageFormat();

		bool locationValid = false;
		if (msg.line != -1) {
			locationST.add("line", msg.line);
			locationValid = true;
		}
		if (msg.charPosition != -1) {
			locationST.add("column", msg.charPosition);
			locationValid = true;
		}
		if (msg.fileName != null) {
			String displayFileName = msg.fileName;
			if (formatName.Equals("antlr")) {
				// Don't show path to file in messages in ANTLR format;
				// they're too long.
				File f = new File(msg.fileName);
				if ( f.exists() ) {
					displayFileName = f.getName();
				}
			}
			else {
				// For other message formats, use the full filename in the
				// message.  This assumes that these formats are intended to
				// be parsed by IDEs, and so they need the full path to
				// resolve correctly.
			}
			locationST.add("file", displayFileName);
			locationValid = true;
		}

		messageFormatST.add("id", msg.getErrorType().code);
		messageFormatST.add("text", messageST);

		if (locationValid) reportST.add("location", locationST);
		reportST.add("message", messageFormatST);
		//((DebugST)reportST).inspect();
//		reportST.impl.dump();
		return reportST;
	}

    /** Return a StringTemplate that refers to the current format used for
     * emitting messages.
     */
    public ST getLocationFormat() {
        return format.getInstanceOf("location");
    }

    public ST getReportFormat(ErrorSeverity severity) {
        ST st = format.getInstanceOf("report");
        st.add("type", severity.getText());
        return st;
    }

    public ST getMessageFormat() {
        return format.getInstanceOf("message");
    }
    public bool formatWantsSingleLineMessage() {
        return format.getInstanceOf("wantsSingleLineMessage").render().Equals("true");
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
        StackTraceElement[] stack = e.getStackTrace();
        int i = 0;
        for (; i < stack.length; i++) {
            StackTraceElement t = stack[i];
            if (!t.toString().contains("ErrorManager")) {
                break;
            }
        }
        StackTraceElement location = stack[i];
        return location;
    }

    // S U P P O R T  C O D E

	//@SuppressWarnings("fallthrough")
	public void emit(ErrorType etype, ANTLRMessage msg) {
		switch ( etype.severity ) {
			case ErrorSeverity.WARNING_ONE_OFF:
				if ( errorTypes.Contains(etype) ) break;
				// fall thru
			case ErrorSeverity.WARNING:
				warnings++;
				tool.warning(msg);
				break;
			case ErrorSeverity.ERROR_ONE_OFF:
				if ( errorTypes.Contains(etype) ) break;
				// fall thru
			case ErrorSeverity.ERROR:
				errors++;
				tool.error(msg);
				break;
      default:
        break;
		}
		errorTypes.add(etype);
	}

    /** The format gets reset either from the Tool if the user supplied a command line option to that effect
     *  Otherwise we just use the default "antlr".
     */
    public void setFormat(String formatName) {
		STGroupFile loadedFormat;

		lock (loadedFormats) {
			loadedFormat = loadedFormats.get(formatName);
			if (loadedFormat == null) {
				String fileName = FORMATS_DIR + formatName + STGroup.GROUP_FILE_EXTENSION;
				ClassLoader cl = Thread.currentThread().getContextClassLoader();
				URL url = cl.getResource(fileName);
				if (url == null) {
					cl = ErrorManager.getClassLoader();
					url = cl.getResource(fileName);
				}
				if (url == null && formatName.Equals("antlr")) {
					rawError("ANTLR installation corrupted; cannot find ANTLR messages format file " + fileName);
					panic();
				}
				else if (url == null) {
					rawError("no such message format file " + fileName + " retrying with default ANTLR format");
					setFormat("antlr"); // recurse on this rule, trying the default message format
					return;
				}
				loadedFormat = new STGroupFile(url, "UTF-8", '<', '>');
				loadedFormat.load();

				loadedFormats[formatName]= loadedFormat;
			}
		}

		this.formatName = formatName;
		this.format = loadedFormat;

		if (!initSTListener.errors.Count == 0) {
			rawError("ANTLR installation corrupted; can't load messages format file:\n" +
					initSTListener.toString());
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
        if (!format.isDefined("location")) {
            Console.Error.WriteLine("Format template 'location' not found in " + formatName);
            ok = false;
        }
        if (!format.isDefined("message")) {
            Console.Error.WriteLine("Format template 'message' not found in " + formatName);
            ok = false;
        }
        if (!format.isDefined("report")) {
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
		ST msgST = getMessageTemplate(msg);
		String outputMsg = msgST.render();
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
