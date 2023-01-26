/*
 * Copyright (c) 2012-2017 The ANTLR Project. All rights reserved.
 * Use of this file is governed by the BSD 3-clause license that
 * can be found in the LICENSE.txt file in the project root.
 */

using System.Text;

namespace org.antlr.v4.runtime.misc;

public class LogManager {
    protected class Record {
		long timestamp;
		StackTraceElement location;
		String component;
		String msg;
		public Record() {
            timestamp = DateTime.Now.Millisecond;
			location = new Throwable().getStackTrace()[0];
		}

		//@Override
		public override String ToString() {
            StringBuilder buf = new StringBuilder();
            buf.append(new SimpleDateFormat("yyyy-MM-dd HH:mm:ss:SSS").format(new Date(timestamp)));
            buf.append(" ");
            buf.append(component);
            buf.append(" ");
            buf.append(location.getFileName());
            buf.append(":");
            buf.append(location.getLineNumber());
            buf.append(" ");
            buf.append(msg);
            return buf.toString();
		}
	}

	protected List<Record> records;

	public void log(String component, String msg) {
		Record r = new Record();
		r.component = component;
		r.msg = msg;
		if ( records==null ) {
			records = new ();
		}
		records.add(r);
	}

    public void log(String msg) { log(null, msg); }

    public void save(String filename){
        FileWriter fw = new FileWriter(filename);
        BufferedWriter bw = new BufferedWriter(fw);
        try {
            bw.write(toString());
        }
        finally {
            bw.close();
        }
    }

    public String save(){
        //String dir = System.getProperty("java.io.tmpdir");
        String dir = ".";
        String defaultFilename =
            dir + "/antlr-" +
            new SimpleDateFormat("yyyy-MM-dd-HH.mm.ss").format(new Date()) + ".log";
        save(defaultFilename);
        return defaultFilename;
    }

    //@Override
    public String toString() {
        if ( records==null ) return "";
        String nl = System.getProperty("line.separator");
        StringBuilder buf = new StringBuilder();
        for (Record r : records) {
            buf.append(r);
            buf.append(nl);
        }
        return buf.toString();
    }

    public static void TestMain(String[] args){
        LogManager mgr = new LogManager();
        mgr.log("atn", "test msg");
        mgr.log("dfa", "test msg 2");
        Console.WriteLine(mgr);
        mgr.save();
    }
}